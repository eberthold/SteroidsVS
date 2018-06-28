using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.Models;
using Steroids.Contracts;
using Steroids.Core;
using Steroids.Core.Diagnostics.Contracts;
using Steroids.Core.Extensions;

namespace Steroids.CodeQuality.ViewModels
{
    public sealed class CodeQualityHintsViewModel : BindableBase, IDisposable
    {
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IQualityTextView _textView;
        private readonly CodeHintFactory _codeHintFactory;
        private readonly IOutliningManager _outliningManager;

        private IEnumerable<CodeHintLineEntry> _lineDiagnostics = Enumerable.Empty<CodeHintLineEntry>();
        private List<DiagnosticInfo> _lastDiagnostics = new List<DiagnosticInfo>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeQualityHintsViewModel"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IQualityTextView"/>.</param>
        /// <param name="diagnosticProvider">The <see cref="IDiagnosticProvider"/>.</param>
        /// <param name="codeHintFactory">The <see cref="CodeHintFactory"/>.</param>
        /// <param name="outliningManagerService">THe <see cref="IOutliningManagerService"/> for the <paramref name="textView"/>.</param>
        public CodeQualityHintsViewModel(
            IQualityTextView textView,
            IDiagnosticProvider diagnosticProvider,
            CodeHintFactory codeHintFactory,
            IOutliningManagerService outliningManagerService)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _codeHintFactory = codeHintFactory ?? throw new ArgumentNullException(nameof(codeHintFactory));
            _outliningManager = outliningManagerService.GetOutliningManager(_textView.TextView);

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
            WeakEventManager<ITextView, TextViewLayoutChangedEventArgs>.AddHandler(_textView.TextView, nameof(ITextView.LayoutChanged), OnTextViewLayoutChanged);

            _outliningManager.RegionsExpanded += OnRegionsExpanded;
            _outliningManager.RegionsCollapsed += OnRegionsCollapsed;
        }

        /// <summary>
        /// Gets all current relevant hints.
        /// </summary>
        public IEnumerable<CodeHintLineEntry> QualityHints
        {
            get { return _lineDiagnostics; }
            private set { Set(ref _lineDiagnostics, value); }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _outliningManager.RegionsExpanded -= OnRegionsExpanded;
            _outliningManager.RegionsCollapsed -= OnRegionsCollapsed;

            _disposed = true;
        }

        /// <summary>
        /// Recreates the code hints, when the items in the error list have changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="DiagnosticsChangedEventArgs"/>.</param>
        private void OnDiagnosticsChanged(object sender, DiagnosticsChangedEventArgs args)
        {
            // TODO: only recreate non existing hints and remove resolved hints.
            var fileDiagnostics = _textView
                .ExtractRelatedDiagnostics(args.Diagnostics)
                .Where(x => x.IsActive)
                .ToList();

            _lastDiagnostics = fileDiagnostics;
            UpdateDiagnostics(fileDiagnostics);
        }

        private void UpdateDiagnostics(List<DiagnosticInfo> fileDiagnostics)
        {
            var lineDiagnostics = fileDiagnostics.ToLookup(x =>
            {
                var line = _textView.TextView.GetSnapshotForLineNumber(x.LineNumber);
                return GetSpanForLine(line);
            });

            QualityHints = lineDiagnostics.Select(x => _codeHintFactory.Create(x, x.Key)).ToList();
        }

        private SnapshotSpan GetSpanForLine(ITextSnapshotLine line)
        {
            try
            {
                if (line == null)
                {
                    var firstPoint = new SnapshotPoint(_textView.TextView.TextSnapshot, 0);
                    return _textView.TextView.GetTextElementSpan(firstPoint);
                }

                var region = _outliningManager?.GetCollapsedRegions(line.Extent) ?? Enumerable.Empty<ICollapsible>();
                if (!region.Any())
                {
                    return line.Extent;
                }

                // I assume that the longest collapsed region is the outermost
                return region
                    .Select(x => x.Extent.GetSpan(_textView.TextView.TextSnapshot))
                    .ToDictionary(x => x.Length)
                    .OrderByDescending(x => x.Key)
                    .First()
                    .Value;
            }
            catch (ObjectDisposedException ex)
            {
                if (ex.ObjectName == "OutliningMnger")
                {
                    // TODO: when we have a logger service add logging
                }

                // I assume that this case seems to happen, if the TextView gets closed and we receive a
                // DiagnosticChanged event right in the timeframe before we dispose the whole container graph.
                return line.Extent;
            }
        }

        /// <summary>
        /// Ensures correct positioning of all existing hints.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextViewLayoutChangedEventArgs"/>.</param>
        private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var hint in QualityHints.ToList())
            {
                hint.RefreshPositions();
            }
        }

        private void OnRegionsCollapsed(object sender, RegionsCollapsedEventArgs e)
        {
            UpdateDiagnostics(_lastDiagnostics);
        }

        private void OnRegionsExpanded(object sender, RegionsExpandedEventArgs e)
        {
            UpdateDiagnostics(_lastDiagnostics);
        }
    }
}
