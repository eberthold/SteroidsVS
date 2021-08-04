using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeStructure.UI;
using SteroidsVS.CodeStructure.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

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
            var node = e.NodeContainer;
            if (node == null)
            {
                return;
            }

            var startLine = _textView.TextSnapshot.GetLineFromLineNumber(node.StartLineNumber);
            var endLine = _textView.TextSnapshot.GetLineFromLineNumber(node.EndLineNumber);
            var snapshotSpan = new SnapshotSpan(startLine.Start, endLine.End);

            _textView.DisplayTextLineContainingBufferPosition(snapshotSpan.Start, 30, ViewRelativePosition.Top);

            // get start and end of snapshot
            var lines = _textView.TextViewLines.GetTextViewLinesIntersectingSpan(snapshotSpan);
            if (lines.Count == 0)
            {
                return;
            }

            // clear adornments
            _adornmentLayer.RemoveAdornmentsByTag(HighlightAdornmentTag);

            // create new adornment
            var start = _textView.GetTextViewLineContainingBufferPosition(startLine.Start);
            var end = _textView.GetTextViewLineContainingBufferPosition(endLine.End);
            var adornerContent = new SelectionHintControl();
            Canvas.SetTop(adornerContent, start.Top);
            Canvas.SetLeft(adornerContent, 0);

            adornerContent.Height = Math.Max(start.Height, end.Top - start.Top);

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