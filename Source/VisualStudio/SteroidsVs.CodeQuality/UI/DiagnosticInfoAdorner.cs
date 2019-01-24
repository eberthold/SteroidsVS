using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;
using Steroids.CodeQuality.UI;

namespace SteroidsVS.CodeQuality.UI
{
    public class DiagnosticInfoAdorner
    {
        private readonly IWpfTextView _textView;
        private readonly IAdornmentLayer _adornmentLayer;
        private readonly DiagnosticInfosView _adorner;

        public DiagnosticInfoAdorner(
            IWpfTextView textView,
            IAdornmentLayer adornmentLayer,
            DiagnosticInfosViewModel viewModel)
        {
            _textView = textView;
            _adornmentLayer = adornmentLayer;

            _adorner = new DiagnosticInfosView
            {
                DataContext = viewModel
            };

            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportWidthChanged), OnSizeChanged);
            WeakEventManager<ITextView, EventArgs>.AddHandler(_textView, nameof(ITextView.ViewportHeightChanged), OnSizeChanged);
            WeakEventManager<DiagnosticInfosView, EventArgs>.AddHandler(_adorner, nameof(FrameworkElement.SizeChanged), OnSizeChanged);

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
