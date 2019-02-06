using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Steroids.CodeStructure.UI;
using SteroidsVS.CodeStructure.Controls;
using SteroidsVS.CodeStructure.Text;

namespace SteroidsVS.CodeStructure.Adorners
{
    /// <summary>
    /// The adorner which will display the code structure.
    /// </summary>
    public sealed class CodeStructureAdorner : ICodeStructureAdorner
    {
        private const string HighlightAdornmentTag = "HighlighterAdornment";
        private const string CodeStructureTag = "CodeStructure";

        private readonly IAdornmentLayer _adornmentLayer;
        private readonly CodeStructureViewModel _viewModel;
        private readonly ContentControl _indicatorView = new ContentControl();
        private readonly IWpfTextView _textView;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureAdorner"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment will be drawn.</param>
        /// <param name="adornmentLayer">The <see cref="IAdornmentLayer"/>.</param>
        /// <param name="viewModel">The <see cref="CodeStructureViewModel"/>.</param>
        public CodeStructureAdorner(
            IWpfTextView textView,
            IAdornmentLayer adornmentLayer,
            CodeStructureViewModel viewModel)
        {
            _adornmentLayer = adornmentLayer ?? throw new ArgumentNullException(nameof(adornmentLayer));
            _viewModel = viewModel;
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _indicatorView.Focusable = false;
            _indicatorView.Content = viewModel; 

            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportWidthChanged), OnSizeChanged);
            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportHeightChanged), OnSizeChanged);
            WeakEventManager<ContentControl, EventArgs>.AddHandler(_indicatorView, nameof(FrameworkElement.SizeChanged), OnSizeChanged);
            WeakEventManager<CodeStructureViewModel, HighlightRequestedEventArgs>.AddHandler(_viewModel, nameof(CodeStructureViewModel.HighlightRequested), OnHighlightRequested);

            ShowAdorner();
        }

        private void OnHighlightRequested(object sender, HighlightRequestedEventArgs e)
        {
            var node = e.NodeContainer.Node;
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

            // clear adornments
            _adornmentLayer.RemoveAdornmentsByTag(HighlightAdornmentTag);

            // create new adornment
            var adornerContent = new SelectionHintControl();
            Canvas.SetTop(adornerContent, startLine.TextTop);
            Canvas.SetLeft(adornerContent, 0);

            adornerContent.Height = Math.Max(startLine.Height, endLine.Top - startLine.Top);

            adornerContent.Width = Math.Max(0, _textView.ViewportWidth);
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, HighlightAdornmentTag, adornerContent, null);
        }

        /// <summary>
        /// Event handler for viewport layout changed event. Adds adornment at the top right corner of the viewport.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetPosition();
        }

        private void SetPosition()
        {
            _indicatorView.Height = _textView.ViewportHeight;
            Panel.SetZIndex(_indicatorView, 10);
            Canvas.SetLeft(_indicatorView, _textView.ViewportRight - _indicatorView.ActualWidth);
            Canvas.SetTop(_indicatorView, _textView.ViewportTop);
        }

        private void ShowAdorner()
        {
            SetPosition();
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, CodeStructureTag, _indicatorView, null);
        }
    }
}