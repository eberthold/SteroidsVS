using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Text.Outlining;
using Steroids.CodeQuality.Models;
using Steroids.CodeQuality.UI;
using Steroids.Contracts;
using Steroids.Contracts.UI;
using Steroids.Core;
using Steroids.Core.Diagnostics.Contracts;
using Steroids.Core.Extensions;

namespace Steroids.CodeQuality.ViewModels
{
    public sealed class CodeQualityHintsViewModel : BindableBase, IDisposable
    {
        private readonly IDiagnosticProvider _diagnosticProvider;

        private readonly CodeHintFactory _codeHintFactory;

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
        /// <param name="adornmentSpaceReservation">The <see cref="IAdornmentSpaceReservation"/>.</param>
        public CodeQualityHintsViewModel(
            IQualityTextView textView,
            IDiagnosticProvider diagnosticProvider,
            CodeHintFactory codeHintFactory,
            IOutliningManagerService outliningManagerService,
            IAdornmentSpaceReservation adornmentSpaceReservation)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _codeHintFactory = codeHintFactory ?? throw new ArgumentNullException(nameof(codeHintFactory));
            TextView = textView ?? throw new ArgumentNullException(nameof(textView));
            AdornmentSpaceReservation = adornmentSpaceReservation ?? throw new ArgumentNullException(nameof(adornmentSpaceReservation));
            OutliningManager = outliningManagerService.GetOutliningManager(TextView.TextView);

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);

            OutliningManager.RegionsExpanded += OnRegionsExpanded;
            OutliningManager.RegionsCollapsed += OnRegionsCollapsed;
        }

        public IOutliningManager OutliningManager { get; private set; }

        /// <summary>
        /// Gets all current relevant hints.
        /// </summary>
        public IEnumerable<CodeHintLineEntry> QualityHints
        {
            get { return _lineDiagnostics; }
            private set { Set(ref _lineDiagnostics, value); }
        }

        /// <summary>
        /// The list of current <see cref="DiagnosticInfoLine"/>.
        /// </summary>
        public ObservableCollection<DiagnosticInfoLine> DiagnosticInfoLines { get; }
            = new ObservableCollection<DiagnosticInfoLine>();

        public IAdornmentSpaceReservation AdornmentSpaceReservation { get; }

        public IQualityTextView TextView { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            OutliningManager.RegionsExpanded -= OnRegionsExpanded;
            OutliningManager.RegionsCollapsed -= OnRegionsCollapsed;

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
            var fileDiagnostics = TextView
                .ExtractRelatedDiagnostics(args.Diagnostics)
                .Where(x => x.IsActive)
                .ToList();

            _lastDiagnostics = fileDiagnostics;
            UpdateDiagnostics(fileDiagnostics);
        }

        private void UpdateDiagnostics(IReadOnlyCollection<DiagnosticInfo> fileDiagnostics)
        {
            var lineMap = fileDiagnostics
                .Select(x => x.LineNumber)
                .Distinct()
                .ToDictionary(x => x, x => DiagnosticInfoPlacementCalculator.GetRealLineNumber(TextView.TextView, x, OutliningManager));

            foreach (var diagnostic in fileDiagnostics)
            {
                if (!lineMap.ContainsKey(diagnostic.LineNumber))
                {
                    diagnostic.ComputedLineNumber = diagnostic.LineNumber;
                    continue;
                }

                diagnostic.ComputedLineNumber = lineMap[diagnostic.LineNumber];
            }

            // remove unused lines
            foreach (var line in DiagnosticInfoLines.Where(x => !lineMap.Values.Contains(x.LineNumber)).ToList())
            {
                DiagnosticInfoLines.Remove(line);
            }

            // update existing lines or add new ones
            foreach (var lineDiagnostics in fileDiagnostics.GroupBy(x => x.ComputedLineNumber))
            {
                var line = DiagnosticInfoLines.FirstOrDefault(x => x.LineNumber == lineDiagnostics.Key)
                    ?? new DiagnosticInfoLine(lineDiagnostics.Key, lineDiagnostics.ToList());

                if (!DiagnosticInfoLines.Contains(line))
                {
                    DiagnosticInfoLines.Add(line);
                }
                else
                {
                    line.DiagnosticInfos = lineDiagnostics.ToList();
                }
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
