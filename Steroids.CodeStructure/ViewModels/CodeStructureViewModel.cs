using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Steroids.CodeStructure.Analyzers;
using Steroids.CodeStructure.Analyzers.Services;
using Steroids.CodeStructure.Controls;
using Steroids.CodeStructure.Extensions;
using Steroids.Contracts;
using Steroids.Core;
using Steroids.Core.Extensions;

namespace Steroids.CodeStructure.ViewModels
{
    public class CodeStructureViewModel : BindableBase
    {
        private const string HighlightAdornmentTag = "HighlighterAdornment";

        private readonly IWpfTextView _textView;
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IDocumentAnalyzerService _documentAnalyzerService;
        private readonly IAdornmentLayer _adornmentLayer;

        private SelectionHintControl _adornerContent;
        private bool _isListVisible;
        private bool _isPaused;
        private ICodeStructureSyntaxAnalyzer _syntaxWalker;
        private ICodeStructureNodeContainer _selectedNode;
        private ICollectionView _nodeCollection;
        private bool _isPinned;
        private DiagnosticSeverity _currentDiagnosticLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureViewModel"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="adornmentLayer">The <see cref="IAdornmentLayer"/>.</param>
        /// <param name="diagnosticProvider">The <see cref="IDiagnosticProvider"/>.</param>
        /// <param name="documentAnalyzerService">The <see cref="IDocumentAnalyzerService"/>.</param>
        /// <param name="diagnosticHintsViewModel">The view model which should display all our diagnostic hints.</param>
        public CodeStructureViewModel(
            IWpfTextView textView,
            IAdornmentLayer adornmentLayer,
            IDiagnosticProvider diagnosticProvider,
            IDocumentAnalyzerService documentAnalyzerService)
        {
            _textView = textView;
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _documentAnalyzerService = documentAnalyzerService ?? throw new ArgumentNullException(nameof(documentAnalyzerService));
            _adornmentLayer = adornmentLayer ?? throw new ArgumentNullException(nameof(adornmentLayer));

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
            WeakEventManager<IDocumentAnalyzerService, EventArgs>.AddHandler(_documentAnalyzerService, nameof(IDocumentAnalyzerService.AnalysisFinished), OnAnalysisFinished);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the code structure view should stay open,
        /// when the user clicks somewhere else.
        /// </summary>
        public bool IsPinned
        {
            get { return _isPinned; }
            set { Set(ref _isPinned, value); }
        }

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        public ICodeStructureNodeContainer SelectedNode
        {
            get
            {
                return _selectedNode;
            }

            set
            {
                Set(ref _selectedNode, value);
                ScrollToNode(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the list is opened or not.
        /// </summary>
        public bool IsListVisible
        {
            get { return _isListVisible; }
            set { Set(ref _isListVisible, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is should temporarily avoid refreshing the tree.
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
            set { Set(ref _isPaused, value); }
        }

        /// <summary>
        /// Gets the count of leaf nodes in analyzed structure - means methods, properties etc.
        /// </summary>
        public int? LeafCount
        {
            get { return _documentAnalyzerService?.Nodes.OfType<ICodeStructureLeaf>().Count(); }
        }

        /// <summary>
        /// Gets or sets the syntax walker.
        /// </summary>
        /// <value>
        /// The syntax walker.
        /// </value>
        public ICodeStructureSyntaxAnalyzer SyntaxWalker
        {
            get { return _syntaxWalker; }
            set { Set(ref _syntaxWalker, value); }
        }

        /// <summary>
        /// Gets or sets the current diagnostic level.
        /// </summary>
        public DiagnosticSeverity CurrentDiagnosticLevel
        {
            get { return _currentDiagnosticLevel; }
            set { Set(ref _currentDiagnosticLevel, value); }
        }

        public ICollectionView NodeCollection
        {
            get { return _nodeCollection; }
            set { Set(ref _nodeCollection, value); }
        }

        private void OnAnalysisFinished(object sender, EventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() => RefreshUi());
        }

        private void OnDiagnosticsChanged(object sender, DiagnosticsChangedEventArgs args)
        {
            var path = _textView.GetDocument()?.FilePath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var fileDiagnostics = args.Diagnostics.Where(x => string.Equals(x.Path, path, StringComparison.OrdinalIgnoreCase) && x.IsActive);
            if (!fileDiagnostics.Any())
            {
                CurrentDiagnosticLevel = DiagnosticSeverity.Hidden;
                return;
            }

            CurrentDiagnosticLevel = fileDiagnostics.Max(x => x.Severity);
        }

        private void RefreshUi()
        {
            RaisePropertyChanged(nameof(LeafCount));
            NodeCollection = CollectionViewSource.GetDefaultView(_documentAnalyzerService?.Nodes ?? new List<ICodeStructureNodeContainer>());
        }

        /// <summary>
        /// Internal scrolling logic.
        /// </summary>
        /// <param name="nodeContainer">The <see cref="ICodeStructureNodeContainer"/>.</param>
        private void ScrollToNode(ICodeStructureNodeContainer nodeContainer)
        {
            var node = nodeContainer?.Node;
            if (node == null)
            {
                return;
            }

            // convert to Snapshotspan and bring into view
            var snapshotSpan = node.FullSpan.ToSnapshotSpan(_textView.TextSnapshot);
            _textView.DisplayTextLineContainingBufferPosition(snapshotSpan.Start, 30, ViewRelativePosition.Top);

            // get start and end of snapshot
            var lines = _textView.TextViewLines.GetTextViewLinesIntersectingSpan(snapshotSpan);
            if (lines.Count == 0)
            {
                return;
            }

            ITextViewLine startLine = lines[0];
            ITextViewLine endLine = lines[lines.Count - 1];

            // skip empty leading lines
            while (string.IsNullOrWhiteSpace(startLine.Extent.GetText()) || startLine.Extent.GetText().StartsWith("/"))
            {
                var index = _textView.TextViewLines.GetIndexOfTextLine(startLine) + 1;
                if (index >= _textView.TextViewLines.Count)
                {
                    break;
                }

                startLine = _textView.TextViewLines[_textView.TextViewLines.GetIndexOfTextLine(startLine) + 1];
            }

            // TODO: that ui stuff has to move to a non view model class.
            // clear adornments
            _adornmentLayer.RemoveAdornmentsByTag(HighlightAdornmentTag);

            // create new adornment
            _adornerContent = new SelectionHintControl();
            Canvas.SetTop(_adornerContent, startLine.TextTop);
            Canvas.SetLeft(_adornerContent, 0);

            _adornerContent.Height = Math.Max(startLine.Height, endLine.Top - startLine.Top);

            _adornerContent.Width = Math.Max(0, _textView.ViewportWidth);
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, HighlightAdornmentTag, _adornerContent, null);
        }
    }
}