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
            var diagnostics = fileDiagnostics.GroupBy(x => x.Line);

            _adorner.AddOrUpdateDiagnosticLine(diagnostics);
        }
    }
}
