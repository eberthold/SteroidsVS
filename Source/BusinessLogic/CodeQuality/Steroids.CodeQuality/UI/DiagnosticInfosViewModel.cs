using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Steroids.CodeQuality.LineHandling;
using Steroids.Core;
using Steroids.Core.CodeQuality;
using Steroids.Core.Editor;

namespace Steroids.CodeQuality.UI
{
    public sealed class DiagnosticInfosViewModel : BindableBase, IDisposable
    {
        private readonly IDiagnosticProvider _diagnosticProvider;

        private List<DiagnosticInfo> _lastDiagnostics = new List<DiagnosticInfo>();
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticInfosViewModel"/> class.
        /// </summary>
        /// <param name="editor">The <see cref="IEditor"/>.</param>
        /// <param name="diagnosticProvider">The <see cref="IDiagnosticProvider"/>.</param>
        public DiagnosticInfosViewModel(
            IEditor editor,
            IDiagnosticProvider diagnosticProvider)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            Editor = editor ?? throw new ArgumentNullException(nameof(editor));

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);

            Editor.FoldingManager.Expanded += OnCodeExpanded;
            Editor.FoldingManager.Collapsed += OnCodeCollapsed;

            OnDiagnosticsChanged(this, new DiagnosticsChangedEventArgs(_diagnosticProvider.CurrentDiagnostics));
        }

        /// <summary>
        /// The list of current <see cref="DiagnosticInfoLine"/>.
        /// </summary>
        public ObservableCollection<DiagnosticInfoLine> DiagnosticInfoLines { get; }
            = new ObservableCollection<DiagnosticInfoLine>();

        /// <summary>
        /// The text view for which the diagnostics are evaluated.
        /// </summary>
        public IEditor Editor { get; private set; }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            Editor.FoldingManager.Expanded -= OnCodeExpanded;
            Editor.FoldingManager.Collapsed -= OnCodeCollapsed;

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
            var fileDiagnostics = Editor
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
                .ToDictionary(x => x, x => Editor.GetComputedLineNumber(x));

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
                    ?? new DiagnosticInfoLine(lineDiagnostics.Key, lineDiagnostics.OrderBy(x => x).ToList());

                if (!DiagnosticInfoLines.Contains(line))
                {
                    DiagnosticInfoLines.Add(line);
                }
                else
                {
                    line.DiagnosticInfos = lineDiagnostics.OrderBy(x => x).ToList();
                }
            }
        }

        /// <summary>
        /// Triggered when a section of code was collapsed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        private void OnCodeCollapsed(object sender, EventArgs e)
        {
            UpdateDiagnostics(_lastDiagnostics);
        }

        /// <summary>
        /// Triggered when a section of code was expanded.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/>.</param>
        private void OnCodeExpanded(object sender, EventArgs e)
        {
            UpdateDiagnostics(_lastDiagnostics);
        }
    }
}
