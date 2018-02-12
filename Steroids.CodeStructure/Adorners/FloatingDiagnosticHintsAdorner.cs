namespace Steroids.CodeStructure.Adorners
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.ViewModels;
    using Steroids.CodeStructure.Views;

    public class FloatingDiagnosticHintsAdorner
    {
        private readonly IWpfTextView _textView;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly FloatingDiagnosticHintsView _adorner;

        public FloatingDiagnosticHintsAdorner(
            IWpfTextView textView,
            IAdornmentLayer adornmentLayer,
            CodeQualityHintsViewModel viewModel)
        {
            _textView = textView;
            _adornmentLayer = adornmentLayer;

            _adorner = new FloatingDiagnosticHintsView
            {
                DataContext = viewModel
            };

            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportWidthChanged), OnSizeChanged);
            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportHeightChanged), OnSizeChanged);
            WeakEventManager<FloatingDiagnosticHintsView, EventArgs>.AddHandler(_adorner, nameof(FrameworkElement.SizeChanged), OnSizeChanged);

            ShowAdorner();
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            SetPosition();
        }

        private void SetPosition()
        {
            _adorner.Width = _textView.ViewportWidth;
            _adorner.Height = _textView.ViewportHeight;
            Canvas.SetLeft(_adorner, _textView.ViewportLeft);
            Canvas.SetTop(_adorner, _textView.ViewportTop);
        }

        private void ShowAdorner()
        {
            SetPosition();
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, null, _adorner, null);
        }
    }
}
