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
using Steroids.Core.Extensions;

namespace Steroids.CodeQuality.ViewModels
{
    public class CodeQualityHintsViewModel : BindableBase
    {
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IWpfTextView _textView;
        private readonly CodeHintFactory _codeHintFactory;
        private readonly IOutliningManager _outliningManager;

        private IEnumerable<CodeHintLineEntry> _lineDiagnostics = Enumerable.Empty<CodeHintLineEntry>();
        private List<DiagnosticInfo> _lastDiagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeQualityHintsViewModel"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="diagnosticProvider">The <see cref="IDiagnosticProvider"/>.</param>
        /// <param name="codeHintFactory">The <see cref="CodeHintFactory"/>.</param>
        /// <param name="outliningManager">THe <see cref="IOutliningManager"/> for the <paramref name="textView"/>.</param>
        public CodeQualityHintsViewModel(
            IWpfTextView textView,
            IDiagnosticProvider diagnosticProvider,
            CodeHintFactory codeHintFactory,
            IOutliningManager outliningManager)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _codeHintFactory = codeHintFactory ?? throw new ArgumentNullException(nameof(codeHintFactory));
            _outliningManager = outliningManager ?? throw new ArgumentNullException(nameof(outliningManager));

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
            WeakEventManager<ITextView, TextViewLayoutChangedEventArgs>.AddHandler(_textView, nameof(ITextView.LayoutChanged), OnTextViewLayoutChanged);
            WeakEventManager<IOutliningManager, RegionsExpandedEventArgs>.AddHandler(_outliningManager, nameof(IOutliningManager.RegionsExpanded), OnRegionsExpanded);
            WeakEventManager<IOutliningManager, RegionsCollapsedEventArgs>.AddHandler(_outliningManager, nameof(IOutliningManager.RegionsCollapsed), OnRegionsCollapsed);
        }

        /// <summary>
        /// Gets all current relevant hints.
        /// </summary>
        public IEnumerable<CodeHintLineEntry> QualityHints
        {
            get { return _lineDiagnostics; }
            private set { Set(ref _lineDiagnostics, value); }
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
                var line = _textView.GetSpanForLineNumber(x.Line);
                return GetSpanForLine(line);
            });

            QualityHints = lineDiagnostics.Select(x => _codeHintFactory.Create(x, x.Key)).ToList();
        }

        private SnapshotSpan GetSpanForLine(ITextSnapshotLine line)
        {
            var region = _outliningManager.GetCollapsedRegions(line.Extent);
            if (!region.Any())
            {
                return line.Extent;
            }

            // I assume that the longest collapsed region is the outermost
            return region
                .Select(x => x.Extent.GetSpan(_textView.TextSnapshot))
                .ToDictionary(x => x.Length)
                .OrderByDescending(x => x.Key)
                .First()
                .Value;
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
