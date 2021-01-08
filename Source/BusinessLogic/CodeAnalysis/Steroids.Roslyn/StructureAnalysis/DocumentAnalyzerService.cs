using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Steroids.CodeStructure.Analyzers;
using Steroids.Core.Editor;
using Steroids.Core.Tools;

namespace Steroids.Roslyn.StructureAnalysis
{
    public class DocumentAnalyzerService : IDocumentAnalyzerService
    {
        private readonly IEditor _editor;
        private readonly ILogger _logger;
        private readonly IRoslynTreeAnalyzer _syntaxAnalyzer;
        private readonly Debouncer _structureDebouncer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalyzerService"/> class.
        /// </summary>
        /// <param name="editor">The <see cref="IEditor"/>.</param>
        /// <param name="syntaxWalkerProvider">The <see cref="ISyntaxWalkerProvider"/>.</param>
        public DocumentAnalyzerService(
            IEditor editor,
            ILogger logger)
        {
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _syntaxAnalyzer = TreeAnalyzerFactory.Create(editor.ContentType);
            IsAnalyzeable = _syntaxAnalyzer is object;

            _editor.ContentChanged += OnContentChanged;
            _structureDebouncer = new Debouncer(TimeSpan.FromSeconds(1.5));
        }

        /// <inheritdoc />
        public event EventHandler AnalysisFinished;

        /// <inheritdoc />
        public bool IsAnalyzeable { get; }

        /// <inheritdoc />
        public IEnumerable<SortedTree<CodeStructureItem>> Nodes { get; private set; }

        private void OnContentChanged(object sender, EventArgs e)
        {
            _structureDebouncer.Debounce(Analysis);
        }

        /// <summary>
        /// Performs the document analysis, after the <see cref="AnalysisStartDelay"/> exceeded.
        /// </summary>
        private void Analysis()
        {
            _ = AnalyzeCodeStructureAsync()
                .ContinueWith(t => AnalysisFinished?.Invoke(this, EventArgs.Empty), TaskScheduler.Current)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Does the code structure analysis.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task AnalyzeCodeStructureAsync()
        {
            try
            {
                var content = await _editor.GetRawEditorContentAsync().ConfigureAwait(false);
                var tree = _syntaxAnalyzer.ParseText(content);

                await _syntaxAnalyzer.Analyze(tree.GetRoot(), CancellationToken.None).ConfigureAwait(false);
                Nodes = _syntaxAnalyzer.NodeList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while analysing {_editor.FilePath}");
            }
        }
    }
}
