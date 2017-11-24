namespace Steroids.CodeStructure.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.CodeAnalysis.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Controls;
    using Steroids.CodeStructure.Extensions;
    using Steroids.Common;
    using Steroids.Contracts;
    using Steroids.Contracts.Services;

    public class CodeStructureViewModel : BindableBase
    {
        private const string AdornmentName = "HintAdorner";

        private readonly IWpfTextView _textView;
        private readonly IWorkspaceManager _workspaceManager;
        private readonly IDocumentAnalyzerService _diagnosticCollectorService;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly FloatingDiagnosticHintsViewModel _diagnosticHintsViewModel;

        private SelectionHintControl _adornerContent;
        private CancellationTokenSource _cts;
        private bool _isListVisible;
        private bool _isPaused;
        private ICodeStructureSyntaxAnalyzer _syntaxWalker;
        private ICodeStructureNodeContainer _selectedNode;
        private DiagnosticSeverity _currentDiagnosticLevel;
        private double _analysisTime;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureViewModel"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="workspaceManager">The <see cref="IWorkspaceManager"/>.</param>
        /// <param name="diagnosticCollectorService">The <see cref="IDocumentAnalyzerService"/>.</param>
        /// <param name="diagnosticHintsViewModel">The view model which should display all our diagnostic hints.</param>
        public CodeStructureViewModel(
            IWpfTextView textView,
            IWorkspaceManager workspaceManager,
            IDocumentAnalyzerService diagnosticCollectorService,
            FloatingDiagnosticHintsViewModel diagnosticHintsViewModel)
        {
            _textView = textView;
            _workspaceManager = workspaceManager ?? throw new ArgumentNullException(nameof(workspaceManager));
            _diagnosticCollectorService = diagnosticCollectorService ?? throw new ArgumentNullException(nameof(diagnosticCollectorService));
            _diagnosticHintsViewModel = diagnosticHintsViewModel ?? throw new ArgumentNullException(nameof(diagnosticHintsViewModel));

            _textView.TextViewModel.VisualBuffer.PostChanged += OnTextViewDataBufferChanged;
            _adornmentLayer = textView.GetAdornmentLayer("SelectionHintAdornment");

            SetSyntaxWalker();
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
                _selectedNode = value;
                ScrollToNode(value);
                _selectedNode = null;  // one day we will synchronize the cursor position with the selected node.
                RaisePropertyChanged();
            }
        }

        public double AnalysisTime
        {
            get { return _analysisTime; }
            set { Set(ref _analysisTime, value); }
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
            get { return SyntaxWalker.NodeList.OfType<ICodeStructureLeaf>().Count(); }
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

        /// <summary>
        /// Cancels the token source and aplly  a new one.
        /// </summary>
        private void CancelAndRenewTokenSource()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }

            _cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Gets called when the data buffer changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void OnTextViewDataBufferChanged(object sender, EventArgs e)
        {
            RefreshStructure();
        }

        /// <summary>
        /// Refreshing the file structure
        /// </summary>
        private void RefreshStructure()
        {
            if (SyntaxWalker == null)
            {
                return;
            }

            CancelAndRenewTokenSource();
            var token = _cts.Token;

            if (IsPaused)
            {
                return;
            }

            Task.Run(() => Analysis(token));
        }

        private async Task Analysis(CancellationToken token)
        {
            // because analysis is a bit expensive we wait for further user input and may cancel current analysis.
            await Task.Delay(TimeSpan.FromSeconds(1.5), token);
            if (token.IsCancellationRequested)
            {
                return;
            }

            var document = GetDocument();
            var tasks = new List<Task>();

            tasks.Add(ProcessAnalysis(document, token));
            tasks.Add(AnalyzeDocument(document, token));

            await Task.WhenAll(tasks);
        }

        private async Task ProcessAnalysis(Document document, CancellationToken token)
        {
            // we want to remain responsive
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            var analysisResult = await _diagnosticCollectorService.GetDiagnosticsAsync(document, token);
            CurrentDiagnosticLevel = GetDiagnosticLevel(analysisResult, document);
            AnalysisTime = Math.Round(analysisResult.AnalyzerTelemetryInfo.Sum(x => x.Value.ExecutionTime.TotalMilliseconds), 0);

            // reset thread priority to normal
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
        }

        private async Task AnalyzeDocument(Document document, CancellationToken token)
        {
            var rootNode = await document.GetSyntaxRootAsync();
            await SyntaxWalker.Analyze(rootNode, token);
            RaisePropertyChanged(nameof(LeafCount));
        }

        /// <summary>
        /// Gets the highest <see cref="DiagnosticSeverity"/> in this document.
        /// </summary>
        /// <param name="analysisResult">The <see cref="AnalysisResult"/>.</param>
        /// <param name="document">The current <see cref="Document"/>.</param>
        /// <returns>The <see cref="DiagnosticSeverity"/>.</returns>
        private DiagnosticSeverity GetDiagnosticLevel(AnalysisResult analysisResult, Document document)
        {
            if (analysisResult == null)
            {
                return DiagnosticSeverity.Hidden;
            }

            var diagnostics = analysisResult.GetAllDiagnostics().Where(x => x.Location.SourceTree.FilePath == document.FilePath).ToList();
            _diagnosticHintsViewModel.MergeDiagnostics(diagnostics);
            if (!diagnostics.Any())
            {
                return DiagnosticSeverity.Hidden;
            }

            return diagnostics.Max(x => x.Severity);
        }

        private Document GetDocument()
        {
            var caretPosition = _textView.Caret.Position.BufferPosition;
            return caretPosition.Snapshot.GetOpenDocumentInCurrentContextWithChanges();
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

            ITextViewLine startLine = lines.First();
            ITextViewLine endLine = lines.Last();

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
            _adornmentLayer.RemoveAllAdornments();

            // create new adornment
            _adornerContent = new SelectionHintControl();
            Canvas.SetTop(_adornerContent, startLine.TextTop);
            Canvas.SetLeft(_adornerContent, 0);

            _adornerContent.Height = Math.Max(0, endLine.Top - startLine.Top);

            _adornerContent.Width = Math.Max(0, _textView.ViewportWidth);
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, null, _adornerContent, null);
        }

        /// <summary>
        /// Sets the syntax walker.
        /// </summary>
        private void SetSyntaxWalker()
        {
            SyntaxWalker = new TypeGroupedSyntaxAnalyzer();
            RefreshStructure();
        }
    }
}