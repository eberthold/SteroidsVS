﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Steroids.CodeStructure.Analyzers;
using Steroids.CodeStructure.Resources.Strings;
using Steroids.Contracts.Core;
using Steroids.Core;
using Steroids.Core.CodeQuality;
using Steroids.Core.Editor;
using Steroids.Core.Framework;
using Steroids.Core.UI;

namespace Steroids.CodeStructure.UI
{
    /// <summary>
    /// The ViewModel for the code structure.
    /// </summary>
    public class CodeStructureViewModel : BindableBase
    {
        private readonly IEditor _editor;
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IDocumentAnalyzerService _documentAnalyzerService;
        private readonly IDispatcherService _dispatcherService;
        private bool _isOpen;
        private bool _isPaused;
        private ICodeStructureSyntaxAnalyzer _syntaxWalker;
        private CodeStructureItem _selectedNode;
        private List<SortedTree<CodeStructureItem>> _nodeCollection;
        private bool _isPinned;
        private DiagnosticSeverity _currentDiagnosticLevel;
        private ICollectionView _nodeListView;
        private string _filterText;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureViewModel"/> class.
        /// </summary>
        /// <param name="editor">The <see cref="IEditor"/>.</param>
        /// <param name="diagnosticProvider">The <see cref="IDiagnosticProvider"/>.</param>
        /// <param name="documentAnalyzerService">The <see cref="IDocumentAnalyzerService"/>.</param>
        /// <param name="spaceReservation">The <see cref="IAdornmentSpaceReservation"/>.</param>
        public CodeStructureViewModel(
            IEditor editor,
            IDiagnosticProvider diagnosticProvider,
            IDocumentAnalyzerService documentAnalyzerService,
            IAdornmentSpaceReservation spaceReservation,
            IDispatcherServiceFactory dispatcherServiceFactory)
        {
            _editor = editor;
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _documentAnalyzerService = documentAnalyzerService ?? throw new ArgumentNullException(nameof(documentAnalyzerService));
            _dispatcherService = dispatcherServiceFactory.Create();

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
            WeakEventManager<IDocumentAnalyzerService, EventArgs>.AddHandler(_documentAnalyzerService, nameof(IDocumentAnalyzerService.AnalysisFinished), OnAnalysisFinished);
            SpaceReservation = spaceReservation;
        }

        /// <summary>
        /// Fired when a highlight of a specific portion in code is requested.
        /// </summary>
        public event EventHandler<HighlightRequestedEventArgs> HighlightRequested;

        /// <summary>
        /// Gets or sets a value indicating whether the code structure view should stay open,
        /// when the user clicks somewhere else.
        /// </summary>
        public bool IsPinned
        {
            get => _isPinned;
            set => Set(ref _isPinned, value);
        }

        /// <summary>
        /// Tells if the used <see cref="IDocumentAnalyzerService"/> can analyze this document.
        /// </summary>
        public bool IsAnalyzeable => _documentAnalyzerService.IsAnalyzeable;

        /// <summary>
        /// Gets or sets the selected node.
        /// </summary>
        public CodeStructureItem SelectedNode
        {
            get => _selectedNode;
            set
            {
                Set(ref _selectedNode, value);
                ScrollToNode(value);
            }
        }

        /// <summary>
        /// The node list which supports filtering.
        /// </summary>
        public ICollectionView NodeListView
        {
            get => _nodeListView;
            set
            {
                value?.MoveCurrentToPosition(-1);
                Set(ref _nodeListView, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the list is opened or not.
        /// </summary>
        public bool IsOpen
        {
            get => _isOpen;
            set => Set(ref _isOpen, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is should temporarily avoid refreshing the tree.
        /// </summary>
        public bool IsPaused
        {
            get => _isPaused;
            set => Set(ref _isPaused, value);
        }

        /// <summary>
        /// Gets the count of leaf nodes in analyzed structure - means methods, properties etc.
        /// </summary>
        public string LeafCount
        {
            get
            {
                if (!IsAnalyzeable)
                {
                    return Strings.NotAvailable_Abbreviation;
                }

                return _documentAnalyzerService.Nodes?.Count(x => x.IsLeaf).ToString();
            }
        }

        /// <summary>
        /// Gets or sets the syntax walker.
        /// </summary>
        /// <value>
        /// The syntax walker.
        /// </value>
        public ICodeStructureSyntaxAnalyzer SyntaxWalker
        {
            get => _syntaxWalker;
            set => Set(ref _syntaxWalker, value);
        }

        /// <summary>
        /// The current Filter entered by the user.
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (!Set(ref _filterText, value))
                {
                    return;
                }

                NodeListView?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the current diagnostic level.
        /// </summary>
        public DiagnosticSeverity CurrentDiagnosticLevel
        {
            get => _currentDiagnosticLevel;
            set => Set(ref _currentDiagnosticLevel, value);
        }

        /// <summary>
        /// The collection of elements in the code structure.
        /// </summary>
        public List<SortedTree<CodeStructureItem>> NodeCollection
        {
            get => _nodeCollection;
            set => Set(ref _nodeCollection, value);
        }

        /// <summary>
        /// Gets the adornment space reservation.
        /// </summary>
        public IAdornmentSpaceReservation SpaceReservation { get; }

        private void OnAnalysisFinished(object sender, EventArgs args)
        {
            Application.Current.Dispatcher.Invoke(() => RefreshUi());
        }

        private void OnDiagnosticsChanged(object sender, DiagnosticsChangedEventArgs args)
        {
            var path = _editor.FilePath;
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            var fileDiagnostics = args.Diagnostics.Where(x => path.EndsWith(x?.Path ?? " ", StringComparison.OrdinalIgnoreCase) && x.IsActive);

            _dispatcherService.Dispatch(() =>
            {
                if (!fileDiagnostics.Any())
                {
                    CurrentDiagnosticLevel = DiagnosticSeverity.Hidden;
                    return;
                }

                CurrentDiagnosticLevel = fileDiagnostics.Max(x => x.Severity);
            });
        }

        private void RefreshUi()
        {
            _dispatcherService.Dispatch(() =>
            {
                RaisePropertyChanged(nameof(LeafCount));
                if (_documentAnalyzerService.Nodes == null)
                {
                    return;
                }

                NodeCollection = _documentAnalyzerService.Nodes.ToList();
                NodeListView = new ListCollectionView(NodeCollection)
                {
                    Filter = FilterNodes
                };
            });
        }

        private bool FilterNodes(object obj)
        {
            var isFilterActive = !string.IsNullOrEmpty(FilterText);
            if (!isFilterActive)
            {
                return true;
            }

            var node = obj as SortedTree<CodeStructureItem>;
            if (node == null)
            {
                return false;
            }

            if (isFilterActive && node.Data.IsMeta)
            {
                return false;
            }

            return node.Data.Name.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        /// <summary>
        /// Internal scrolling logic.
        /// </summary>
        /// <param name="nodeContainer">The <see cref="ICodeStructureItem"/>.</param>
        private void ScrollToNode(CodeStructureItem nodeContainer)
        {
            if (nodeContainer is null)
            {
                return;
            }

            HighlightRequested?.Invoke(this, new HighlightRequestedEventArgs(nodeContainer));
            _editor.SetCursorToLine(nodeContainer.StartLineNumber);
        }
    }
}