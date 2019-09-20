using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Steroids.CodeStructure.Analyzers;
using Steroids.Core.Editor;
using Steroids.Core.Tools;

namespace Steroids.Roslyn.StructureAnalysis
{
    public class DocumentAnalyzerService : IDocumentAnalyzerService
    {
        private readonly IEditor _editor;
        private readonly IRoslynTreeAnalyzer _syntaxAnalyzer;
        private readonly Debouncer _structureDebouncer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalyzerService"/> class.
        /// </summary>
        /// <param name="editor">The <see cref="IEditor"/>.</param>
        /// <param name="syntaxWalkerProvider">The <see cref="ISyntaxWalkerProvider"/>.</param>
        public DocumentAnalyzerService(
            IEditor editor)
        {
            _editor = editor ?? throw new ArgumentNullException(nameof(editor));
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
            AnalyzeCodeStructureAsync()
                .ContinueWith(t => AnalysisFinished?.Invoke(this, EventArgs.Empty), TaskContinuationOptions.OnlyOnRanToCompletion)
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
            catch
            {
                // TODO: logging
            }
        }
    }
}
