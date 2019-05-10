using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.Core.Editor;
using SteroidsVS.SharedUI.Editor;

namespace SteroidsVS.Editor
{
    /// <summary>
    /// Turns a <see cref="IWpfTextView"/> into an <see cref="IEditor"/>.
    /// </summary>
    public class WpfTextViewAdapter : IEditor
    {
        private readonly IWpfTextView _textView;
        private readonly IOutliningManagerService _outliningManagerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfTextViewAdapter"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="outliningManagerService">The <see cref="IOutliningManagerService"/>.</param>
        /// <param name="foldingManager">The <see cref="IFoldingManager"/>.</param>
        public WpfTextViewAdapter(
            IWpfTextView textView,
            IOutliningManagerService outliningManagerService,
            IFoldingManager foldingManager)
        {
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _outliningManagerService = outliningManagerService ?? throw new ArgumentNullException(nameof(outliningManagerService));
            FoldingManager = foldingManager ?? throw new ArgumentNullException(nameof(foldingManager));

            WeakEventManager<ITextBuffer, EventArgs>.AddHandler(_textView.TextBuffer, nameof(ITextBuffer.PostChanged), OnTextPostChanged);
        }

        /// <inheritdoc />
        public event EventHandler ContentChanged;

        /// <inheritdoc />
        public string ContentType => _textView.TextBuffer.ContentType.TypeName;

        /// <inheritdoc />
        public string FilePath
        {
            get
            {
                if (_textView.TextBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out var document) && document != null)
                {
                    return document.FilePath;
                }

                if (_textView.TextDataModel.DocumentBuffer.Properties.TryGetProperty<ITextDocument>(typeof(ITextDocument), out var dataModelDocument) && dataModelDocument != null)
                {
                    return dataModelDocument.FilePath;
                }

                return _textView?.GetDocument()?.FilePath ?? string.Empty;
            }
        }

        /// <inheritdoc />
        public IFoldingManager FoldingManager { get; }

        /// <inheritdoc />
        public int GetComputedLineNumber(int absoluteLineNumber)
        {
            try
            {
                // a line could not get any further than 0 even by collapsing regions
                if (absoluteLineNumber == 0)
                {
                    return 0;
                }

                // if no snapshot line found return 0
                var lineSnapshot = _textView.GetSnapshotForLineNumber(absoluteLineNumber);
                if (lineSnapshot == null)
                {
                    return 0;
                }

                // if no collapsed region than line number fits as normal
                var outliningManager = _outliningManagerService.GetOutliningManager(_textView);
                var snapshotSpan = lineSnapshot.Extent;
                var region = outliningManager?.GetCollapsedRegions(snapshotSpan) ?? Enumerable.Empty<ICollapsible>();
                if (!region.Any())
                {
                    return absoluteLineNumber;
                }

                // I assume that the longest collapsed region is the outermost
                var regionSnapshot = region
                    .Select(x => x.Extent.GetSpan(_textView.TextSnapshot))
                    .ToDictionary(x => x.Length)
                    .OrderByDescending(x => x.Key)
                    .First()
                    .Value;

                var collapsedLineNumber = _textView.TextSnapshot.GetLineNumberFromPosition(regionSnapshot.End.Position);
                return collapsedLineNumber;
            }
            catch (ObjectDisposedException ex)
            {
                if (ex.ObjectName == "OutliningMnger")
                {
                    // TODO: when we have a logger service add logging
                }

                // I assume that this case seems to happen, if the TextView gets closed and we receive a
                // DiagnosticChanged event right in the time frame before we dispose the whole container graph.
                return absoluteLineNumber;
            }
        }

        /// <inheritdoc />
        public Task<string> GetRawEditorContentAsync()
        {
            var text = _textView.TextDataModel.DocumentBuffer.CurrentSnapshot.GetText();
            return Task.FromResult(text);
        }

        /// <inheritdoc />
        public void SetCursorToLine(int absoluteLineNumber)
        {
            var snapshotSpan = _textView.GetSnapshotForLineNumber(absoluteLineNumber);
            _textView.Caret.MoveTo(snapshotSpan.Start, PositionAffinity.Successor);
        }

        private void OnTextPostChanged(object sender, EventArgs e)
        {
            ContentChanged?.Invoke(this, e);
        }
    }
}
