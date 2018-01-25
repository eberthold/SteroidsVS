namespace Steroids.CodeStructure.Adorners
{
    using System;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.ViewModels;
    using Steroids.CodeStructure.Views;

    public class FloatingDiagnosticHintsAdorner
    {
        private readonly IWpfTextView _parentView;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly FloatingDiagnosticHintsView _adorner;

        public FloatingDiagnosticHintsAdorner(
            IWpfTextView parentView,
            CodeQualityHintsViewModel viewModel)
        {
            _parentView = parentView;

            _adornmentLayer = parentView.GetAdornmentLayer(nameof(CodeStructureAdorner));
            _adorner = new FloatingDiagnosticHintsView();
            _adorner.DataContext = viewModel;

            _parentView.ViewportWidthChanged += OnSizeChanged;
            _parentView.ViewportHeightChanged += OnSizeChanged;
            _adorner.SizeChanged += OnSizeChanged;

            ShowAdorner();
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetPosition();
        }

        private void SetPosition()
        {
            _adorner.Width = _parentView.ViewportWidth;
            _adorner.Height = _parentView.ViewportHeight;
            Canvas.SetLeft(_adorner, _parentView.ViewportLeft);
            Canvas.SetTop(_adorner, _parentView.ViewportTop);
        }

        private void ShowAdorner()
        {
            SetPosition();
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _adorner, null);
        }
    }
}
