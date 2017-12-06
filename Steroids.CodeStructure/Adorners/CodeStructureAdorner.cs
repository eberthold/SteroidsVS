namespace Steroids.CodeStructure.Adorners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Formatting;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Controls;
    using Steroids.CodeStructure.ViewModels;
    using Steroids.CodeStructure.Views;

    /// <summary>
    /// The adorner which will display the code structure.
    /// </summary>
    public sealed class CodeStructureAdorner : ICodeStructureAdorner
    {
        private const string CodeStructureTag = "CodeStructure";
        private const string FloatingMarkerTag = "Marker";

        private readonly IAdornmentLayer _adornmentLayer;
        private readonly CodeStructureView _indicatorView;
        private readonly IWpfTextView _parentView;

        private IEnumerable<IGrouping<int, DiagnosticInfo>> _lastDiagnostics = new List<IGrouping<int, DiagnosticInfo>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureAdorner"/> class.
        /// </summary>
        /// <param name="parentView">The <see cref="IWpfTextView"/> upon which the adornment will be drawn.</param>
        public CodeStructureAdorner(IWpfTextView parentView)
        {
            _adornmentLayer = parentView.GetAdornmentLayer(nameof(CodeStructureAdorner));
            _parentView = parentView ?? throw new ArgumentNullException(nameof(parentView));
            _parentView.LayoutChanged += _parentView_LayoutChanged;
            _indicatorView = new CodeStructureView();

            _parentView.ViewportWidthChanged += OnSizeChanged;
            _parentView.ViewportHeightChanged += OnSizeChanged;
            _indicatorView.SizeChanged += OnSizeChanged;

            ShowAdorner();
        }

        private void _parentView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            AddOrUpdateDiagnosticLine(_lastDiagnostics, false);
        }

        public void SetDataContext(object context)
        {
            _indicatorView.DataContext = context;
        }

        public void AddOrUpdateDiagnosticLine(IEnumerable<IGrouping<int, DiagnosticInfo>> diagnostics, bool canUpdate = true)
        {
            if (canUpdate)
            {
                _lastDiagnostics = diagnostics;
            }

            int i = 0;
            var existingAdronments = _adornmentLayer.Elements.Where(x => x.Tag.Equals(FloatingMarkerTag)).ToList();
            foreach (var diagnostic in diagnostics)
            {
                var line = _parentView.TextSnapshot.Lines.ElementAt(diagnostic.Key);
                if (line == null)
                {
                    continue;
                }

                var textViewLine = _parentView.GetTextViewLineContainingBufferPosition(line.Extent.Start);
                if (textViewLine.VisibilityState < VisibilityState.PartiallyVisible)
                {
                    continue;
                }

                FloatingDiagnosticHint floatingHint;
                if (i < existingAdronments.Count && existingAdronments.Count > 0)
                {
                    floatingHint = existingAdronments[i].Adornment as FloatingDiagnosticHint;
                }
                else
                {
                    floatingHint = new FloatingDiagnosticHint();

                    _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, line.Extent, FloatingMarkerTag, floatingHint, null);
                }

                if (floatingHint == null)
                {
                    continue;
                }

                var previewDiagnostic = diagnostic.OrderByDescending(x => x.Severity).ThenBy(x => x.Column).First();

                //var left = Math.Max(_parentView.ViewportWidth / 2, Math.Min(textViewLine.TextRight + 4, _parentView.ViewportRight - 50));
                var left = Math.Min(textViewLine.TextRight + 4, _parentView.ViewportRight);
                floatingHint.Severity = diagnostic.Max(x => x.Severity);
                floatingHint.Code = previewDiagnostic.ErrorCode;
                floatingHint.Message = previewDiagnostic.Message;
                floatingHint.Width = _parentView.ViewportWidth - left;

                Canvas.SetLeft(floatingHint, left);
                Canvas.SetTop(floatingHint, textViewLine.TextTop - ((floatingHint.Height - textViewLine.Height) / 2));
                i++;
            }

            if (i < existingAdronments.Count)
            {
                var remainginAdornments = existingAdronments.Skip(i);
                _adornmentLayer.RemoveMatchingAdornments(x => remainginAdornments.Contains(x));
            }
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
            Canvas.SetZIndex(_indicatorView, 10);
            Canvas.SetLeft(_indicatorView, _parentView.ViewportRight - _indicatorView.ActualWidth);
            Canvas.SetTop(_indicatorView, _parentView.ViewportTop);
        }

        private void ShowAdorner()
        {
            SetPosition();
            _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, null, CodeStructureTag, _indicatorView, null);
        }
    }
}