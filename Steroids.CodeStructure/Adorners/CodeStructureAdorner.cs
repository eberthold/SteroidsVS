using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeStructure.UI;

namespace Steroids.CodeStructure.Adorners
{
    /// <summary>
    /// The adorner which will display the code structure.
    /// </summary>
    public sealed class CodeStructureAdorner : ICodeStructureAdorner
    {
        private const string CodeStructureTag = "CodeStructure";

        private readonly IAdornmentLayer _adornmentLayer;
        private readonly CodeStructureView _indicatorView;
        private readonly IWpfTextView _textView;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureAdorner"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> upon which the adornment will be drawn.</param>
        /// <param name="adornmentLayer">The <see cref="IAdornmentLayer"/>.</param>
        /// <param name="viewFactory">The <see cref="CodeStructureViewFactory"/>.</param>
        public CodeStructureAdorner(
            IWpfTextView textView,
            IAdornmentLayer adornmentLayer,
            CodeStructureViewFactory viewFactory)
        {
            _adornmentLayer = adornmentLayer ?? throw new ArgumentNullException(nameof(adornmentLayer));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _indicatorView = viewFactory.Create();

            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportWidthChanged), OnSizeChanged);
            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportHeightChanged), OnSizeChanged);
            WeakEventManager<CodeStructureView, EventArgs>.AddHandler(_indicatorView, nameof(FrameworkElement.SizeChanged), OnSizeChanged);

            ShowAdorner();
        }

        /// <summary>
        /// Event handler for viewport layout changed event. Adds adornment at the top right corner of the viewport.
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
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