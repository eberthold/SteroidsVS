namespace Steroids.CodeStructure.Adorners
{
    using System;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.ViewModels;
    using Steroids.CodeStructure.Views;

    /// <summary>
    /// The adorner which will display the code structure.
    /// </summary>
    public sealed class CodeStructureAdorner
    {
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly CodeStructureView _indicatorView;
        private readonly IWpfTextView _parentView;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureAdorner"/> class.
        /// </summary>
        /// <param name="parentView">The <see cref="IWpfTextView"/> upon which the adornment will be drawn.</param>
        /// <param name="viewModel">The <see cref="ICodeStructureViewModel"/>.</param>
        public CodeStructureAdorner(
            IWpfTextView parentView,
            CodeStructureViewModel viewModel)
        {
            _adornmentLayer = parentView.GetAdornmentLayer(nameof(CodeStructureAdorner));
            _parentView = parentView ?? throw new ArgumentNullException(nameof(parentView));
            _indicatorView = new CodeStructureView();
            _indicatorView.DataContext = viewModel;

            _parentView.ViewportWidthChanged += OnSizeChanged;
            _parentView.ViewportHeightChanged += OnSizeChanged;
            _indicatorView.SizeChanged += OnSizeChanged;

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
            _indicatorView.Height = _parentView.ViewportHeight;
            Canvas.SetLeft(_indicatorView, _parentView.ViewportRight - _indicatorView.ActualWidth);
            Canvas.SetTop(_indicatorView, _parentView.ViewportTop);
        }

        private void ShowAdorner()
        {
            SetPosition();
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _indicatorView, null);
        }
    }
}