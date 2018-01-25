using Steroids.CodeStructure.Views;
namespace Steroids.CodeStructure.Adorners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Views;
    using Steroids.Common.Helpers;

    /// <summary>
    /// The adorner which will display the code structure.
    /// </summary>
    public sealed class CodeStructureAdorner : ICodeStructureAdorner
    {
        private const string CodeStructureTag = "CodeStructure";

        private readonly IAdornmentLayer _adornmentLayer;
        private readonly CodeStructureView _indicatorView;
        private readonly IWpfTextView _parentView;
        private readonly Debouncer _floatingMarkerDebouncer;

        private ILookup<int, DiagnosticInfo> _lastDiagnostics = new List<DiagnosticInfo>().ToLookup(x => 0);

        private List<ITrackingSpan> _trackingSpans = new List<ITrackingSpan>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeStructureAdorner"/> class.
        /// </summary>
        /// <param name="parentView">The <see cref="IWpfTextView"/> upon which the adornment will be drawn.</param>
        public CodeStructureAdorner(IWpfTextView parentView)
        {
            _adornmentLayer = parentView.GetAdornmentLayer(nameof(CodeStructureAdorner));
            _parentView = parentView ?? throw new ArgumentNullException(nameof(parentView));
            _parentView.LayoutChanged += ParentView_LayoutChanged;
            _indicatorView = new CodeStructureView();

            _floatingMarkerDebouncer = new Debouncer(RefreshFloatingMarkers, TimeSpan.FromSeconds(0.1));

            _parentView.ViewportWidthChanged += OnSizeChanged;
            _parentView.ViewportHeightChanged += OnSizeChanged;
            _indicatorView.SizeChanged += OnSizeChanged;

            ShowAdorner();
        }

        private void ParentView_LayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            if (e.VerticalTranslation)
            {
                RefreshFloatingMarkers();
                return;
            }

            _floatingMarkerDebouncer.Start();
        }

        public void SetDataContext(object context)
        {
            _indicatorView.DataContext = context;
        }

        public void AddOrUpdateDiagnosticLine(IEnumerable<DiagnosticInfo> diagnostics)
        {
            _lastDiagnostics = diagnostics.ToLookup(x => x.Line);
            _floatingMarkerDebouncer.Start();
        }

        private void RefreshFloatingMarkers()
        {
            
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