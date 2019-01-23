using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.Core.Extensions;
using Steroids.Core.Helpers;

namespace Steroids.CodeStructure.Analyzers.Services
{
    public class DocumentAnalyzerService : IDocumentAnalyzerService
    {
        private static readonly IReadOnlyCollection<string> _analyzeableContentTypes = new List<string>
        { "CSharp" };

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
            IsAnalyzeable = _analyzeableContentTypes.Contains(textView.TextSnapshot.ContentType.TypeName);

            WeakEventManager<ITextBuffer, EventArgs>.AddHandler(_textView.TextBuffer, nameof(ITextBuffer.PostChanged), OnTextChanged);
            _structureDebouncer = new Debouncer(Analysis, TimeSpan.FromSeconds(1.5));
            _structureDebouncer.Start();
        }

        /// <inheritdoc />
        public event EventHandler AnalysisFinished;

        /// <inheritdoc />
        public bool IsAnalyzeable { get; }

        /// <inheritdoc />
        public IEnumerable<ICodeStructureNodeContainer> Nodes { get; private set; }

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
                .ContinueWith(t => AnalysisFinished?.Invoke(this, EventArgs.Empty), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ConfigureAwait(false);
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
            var rootNode = await document.GetSyntaxRootAsync(CancellationToken.None).ConfigureAwait(false);

            await syntaxAnalyzer.Analyze(rootNode, CancellationToken.None).ConfigureAwait(false);
            Nodes = syntaxAnalyzer.NodeList;

            // reset thread priority to normal
            Thread.CurrentThread.Priority = ThreadPriority.Normal;
        }
    }
}
