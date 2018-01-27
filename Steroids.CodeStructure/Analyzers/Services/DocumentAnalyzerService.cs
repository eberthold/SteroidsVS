using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeStructure.Extensions;
using Steroids.Common.Helpers;

namespace Steroids.CodeStructure.Analyzers.Services
{
    public class DocumentAnalyzerService : IDocumentAnalyzerService
    {
        private readonly IWpfTextView _textView;
        private readonly ISyntaxWalkerProvider _syntaxWalkerProvider;
        private readonly Debouncer _structureDebouncer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentAnalyzerService"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="syntaxWalkerProvider">The <see cref="ISyntaxWalkerProvider"/>.</param>
        public DocumentAnalyzerService(
            IWpfTextView textView,
            ISyntaxWalkerProvider syntaxWalkerProvider)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _syntaxWalkerProvider = syntaxWalkerProvider ?? throw new ArgumentNullException(nameof(syntaxWalkerProvider));

            WeakEventManager<ITextBuffer, EventArgs>.AddHandler(_textView.TextBuffer, nameof(ITextBuffer.PostChanged), OnTextChanged);
            _structureDebouncer = new Debouncer(Analysis, TimeSpan.FromSeconds(1.5));
            _structureDebouncer.Start();
        }

        /// <inheritdoc />
        public event EventHandler AnalysisFinished;

        /// <inheritdoc />
        public IEnumerable<ICodeStructureNodeContainer> Nodes { get; private set; } = new List<ICodeStructureNodeContainer>();

        /// <inheritdoc />
        public ProjectId ProjectId => _textView.GetDocument().Project.Id;

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!_textView.VisualElement.IsVisible)
            {
                return;
            }

            _structureDebouncer.Start();
        }

        /// <summary>
        /// Performs the document analysis, after the <see cref="AnalysisStartDelay"/> exceeded.
        /// </summary>
        private void Analysis()
        {
            var document = _textView.GetDocument();
            AnalyzeCodeStructureAsync(document)
                .ContinueWith(t => AnalysisFinished?.Invoke(this, EventArgs.Empty), TaskContinuationOptions.OnlyOnRanToCompletion);
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
    }
}
