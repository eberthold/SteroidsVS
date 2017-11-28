namespace Steroids.CodeStructure.Analyzers.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Extensions;
    using Steroids.Common.Helpers;
    using Steroids.Contracts;

    public class DocumentAnalyzerService : IDocumentAnalyzerService
    {
        private readonly TimeSpan _analysisStartDelay = TimeSpan.FromSeconds(1);
        private readonly IWpfTextView _textView;
        private readonly ISyntaxWalkerProvider _syntaxWalkerProvider;

        private Debouncer _debouncer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalyzerService"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="workspaceManager">The <see cref="IWorkspaceManager"/>.</param>
        /// <param name="syntaxWalkerProvider">The <see cref="ISyntaxWalkerProvider"/>.</param>
        public DocumentAnalyzerService(
            IWpfTextView textView,
            IWorkspaceManager workspaceManager,
            ISyntaxWalkerProvider syntaxWalkerProvider)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _syntaxWalkerProvider = syntaxWalkerProvider ?? throw new ArgumentNullException(nameof(syntaxWalkerProvider));

            workspaceManager.VsWorkspace.WorkspaceChanged += OnWorkspaceChanged;

            _debouncer = new Debouncer(() => Task.Run(Analysis), _analysisStartDelay);
            Task.Run(Analysis);
        }

        /// <inheritdoc />
        public event EventHandler AnalysisFinished;

        /// <inheritdoc />
        public IEnumerable<ICodeStructureNodeContainer> Nodes { get; private set; } = new List<ICodeStructureNodeContainer>();

        /// <inheritdoc />
        public IEnumerable<Diagnostic> Diagnostics { get; private set; } = new List<Diagnostic>();

        /// <summary>
        /// Gets triggered when something in the workspace changed.
        /// </summary>
        /// <param name="sender">The <see cref="sender"/>.</param>
        /// <param name="args">The <see cref="WorkspaceChangeEventArgs"/>.</param>
        private void OnWorkspaceChanged(object sender, WorkspaceChangeEventArgs args)
        {
            if (!_textView.VisualElement.IsVisible)
            {
                return;
            }

            _debouncer.Start();
        }

        /// <summary>
        /// Performs the document analysis, after the <see cref="AnalysisStartDelay"/> exceeded.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task Analysis()
        {
            var document = _textView.GetDocument();

            var tasks = new List<Task>
            {
                DocumentAnalysis(document),
                AnalyzeCodeStructureAsync(document)
            };

            await Task.WhenAll(tasks).ConfigureAwait(false);

            AnalysisFinished?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Does the code structure analysis.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task AnalyzeCodeStructureAsync(Document document)
        {
            // we want to remain responsive
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            var syntaxAnalyzer = _syntaxWalkerProvider.SyntaxAnalyzer;
            var rootNode = await document.GetSyntaxRootAsync(CancellationToken.None);

            await syntaxAnalyzer.Analyze(rootNode, CancellationToken.None);
            Nodes = syntaxAnalyzer.NodeList;

            // reset thread priority to normal
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
        }

        /// <summary>
        /// Analyses the document for existing errors, warnings and hints.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task DocumentAnalysis(Document document)
        {
            // we want to remain responsive
            Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;

            Diagnostics = await GetDiagnosticsAsync(document);

            // reset thread priority to normal
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
        }

        private async Task<IEnumerable<Diagnostic>> GetDiagnosticsAsync(Document document)
        {
            var compilation = await document.Project.GetCompilationAsync(CancellationToken.None);
            ImmutableArray<DiagnosticAnalyzer> analyzers = GetAnalyzersOfProject(document);

            ImmutableArray<Diagnostic> allDiagnostics = ImmutableArray<Diagnostic>.Empty;
            if (analyzers.Length > 0)
            {
                var analyzedCompilation = compilation.WithAnalyzers(analyzers);
                allDiagnostics = await analyzedCompilation.GetAllDiagnosticsAsync(CancellationToken.None);
            }
            else
            {
                allDiagnostics = compilation.GetDiagnostics();
            }

            return allDiagnostics.Where(x => x.Location.SourceTree.FilePath == document.FilePath);
        }

        /// <summary>
        /// Gets the list with all analysers from the project of the document.
        /// </summary>
        /// <param name="document">The <see cref="Document"/>.</param>
        /// <returns>The <see cref="ImmutableArray{T}"/> with all <see cref="DiagnosticAnalyzer"/>.</returns>
        private ImmutableArray<DiagnosticAnalyzer> GetAnalyzersOfProject(Document document)
        {
            var analyzerReferences = document.Project.AnalyzerReferences.SelectMany(x => x.GetAnalyzers(document.Project.Language));
            if (!analyzerReferences.Any())
            {
                return ImmutableArray<DiagnosticAnalyzer>.Empty;
            }

            return ImmutableArray<DiagnosticAnalyzer>.Empty.AddRange(analyzerReferences);
        }
    }
}
