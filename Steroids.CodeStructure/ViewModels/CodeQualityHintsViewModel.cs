namespace Steroids.CodeStructure.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Adorners;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Analyzers.Services;
    using Steroids.CodeStructure.Helpers;

    public class CodeQualityHintsViewModel
    {
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IWpfTextView _textView;
        private readonly CodeStructureAdorner _adorner;

        public CodeQualityHintsViewModel(
            IWpfTextView textView,
            IDiagnosticProvider diagnosticProvider,
            CodeStructureAdorner adorner)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _adorner = adorner ?? throw new ArgumentNullException(nameof(adorner));

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
        }

        private void OnDiagnosticsChanged(object sender, DiagnosticsChangedEventArgs args)
        {
            var fileDiagnostics = IWpfTextViewHelper.GetDiagnosticsOfEditor(_textView, args.Diagnostics);
            var lineDiagnostics = fileDiagnostics.GroupBy(x => x.Line);
            var dictionary = new Dictionary<SnapshotSpan, IEnumerable<DiagnosticInfo>>();

            foreach (var diagnostic in lineDiagnostics)
            {
                var line = _textView.TextBuffer.CurrentSnapshot.GetLineFromLineNumber(diagnostic.Key);
                var snapshot = line.Extent;
                dictionary.Add(snapshot, diagnostic);
            }

            _adorner.AddOrUpdateDiagnosticLine(dictionary);
        }
    }
}
