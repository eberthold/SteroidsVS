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

        private Dictionary<SnapshotSpan, IEnumerable<DiagnosticInfo>> _lastDiagnostics = new Dictionary<SnapshotSpan, IEnumerable<DiagnosticInfo>>();

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

        public void AddOrUpdateDiagnosticLine(Dictionary<SnapshotSpan, IEnumerable<DiagnosticInfo>> diagnostics, bool canUpdate = true)
        {
            if (canUpdate)
            {
                _lastDiagnostics = diagnostics;
            }

            _adornmentLayer.RemoveAdornmentsByTag(FloatingMarkerTag);
            foreach (var diagnostic in diagnostics)
            {
                var severity = DiagnosticSeverity.Hidden;
                switch (diagnostic.Value.Min(x => x.ErrorCategory))
                {
                    case __VSERRORCATEGORY.EC_ERROR:
                        severity = DiagnosticSeverity.Error;
                        break;
                    case __VSERRORCATEGORY.EC_WARNING:
                        severity = DiagnosticSeverity.Warning;
                        break;
                    case __VSERRORCATEGORY.EC_MESSAGE:
                        severity = DiagnosticSeverity.Info;
                        break;
                }

                var control = new FloatingDiagnosticHint()
                {
                    Severity = severity
                };

                var line = _parentView.TextSnapshot.Lines.FirstOrDefault(x => x.Extent == diagnostic.Key);
                if (line == null)
                {
                    continue;
                }

                var textViewLine = _parentView.GetTextViewLineContainingBufferPosition(line.Extent.Start);
                if (textViewLine.VisibilityState < VisibilityState.PartiallyVisible)
                {
                    continue;
                }

                Canvas.SetLeft(control, 0);
                Canvas.SetTop(control, textViewLine.TextTop);
                _adornmentLayer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, null, FloatingMarkerTag, control, null);
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