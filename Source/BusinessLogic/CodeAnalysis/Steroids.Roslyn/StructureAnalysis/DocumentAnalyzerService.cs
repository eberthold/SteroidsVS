using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Steroids.CodeStructure.Analyzers;
using Steroids.Core.Editor;
using Steroids.Core.Framework;
using Steroids.Core.Tools;

namespace Steroids.Roslyn.StructureAnalysis
{
    public class DocumentAnalyzerService : IDocumentAnalyzerService
    {
        private readonly IEditor _editor;
        private readonly ICancellationService _cancellationService;
        private readonly IRoslynTreeAnalyzer _syntaxAnalyzer;
        private readonly Debouncer _structureDebouncer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalyzerService"/> class.
        /// </summary>
        public DocumentAnalyzerService(
            IEditor editor,
            ICancellationServiceFactory cancellationServiceFactory)
        {
            if (cancellationServiceFactory is null)
            {
                throw new ArgumentNullException(nameof(cancellationServiceFactory));
            }

            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
            _cancellationService = cancellationServiceFactory.Create();

            _syntaxAnalyzer = TreeAnalyzerFactory.Create(editor.ContentType);
            IsAnalyzeable = _syntaxAnalyzer is object;

            _editor.ContentChanged += OnContentChanged;
            _structureDebouncer = new Debouncer(Analysis, TimeSpan.FromSeconds(1.5));
            _structureDebouncer.Start();
        }

        /// <inheritdoc />
        public event EventHandler AnalysisFinished;

        /// <inheritdoc />
        public bool IsAnalyzeable { get; }

        /// <inheritdoc />
        public IEnumerable<SortedTree<CodeStructureItem>> Nodes { get; private set; }

        private void OnContentChanged(object sender, EventArgs e)
        {
            _structureDebouncer.Start();
        }

        /// <summary>
        /// Performs the document analysis, after the <see cref="AnalysisStartDelay"/> exceeded.
        /// </summary>
        private void Analysis()
        {
            var token = _cancellationService.GetNewTokenAndCancelOldOnes();
            _ = AnalyzeCodeStructureAsync(token).ConfigureAwait(false);
        }

        /// <summary>
        /// Does the code structure analysis.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private async Task AnalyzeCodeStructureAsync(CancellationToken token)
        {
            try
            {
                var content = await _editor.GetRawEditorContentAsync().ConfigureAwait(false);
                var tree = _syntaxAnalyzer.ParseText(content);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                await _syntaxAnalyzer.Analyze(tree.GetRoot(), CancellationToken.None).ConfigureAwait(false);

                if (token.IsCancellationRequested)
                {
                    return;
                }

                Nodes = _syntaxAnalyzer.NodeList;
                AnalysisFinished?.Invoke(this, EventArgs.Empty);
            }
            catch
            {
                // TODO: logging
            }
        }
    }
}
