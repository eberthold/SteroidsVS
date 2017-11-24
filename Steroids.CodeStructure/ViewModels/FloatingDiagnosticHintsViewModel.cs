namespace Steroids.CodeStructure.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Microsoft.CodeAnalysis;
    using Microsoft.VisualStudio.Text.Editor;

    public class FloatingDiagnosticHintsViewModel
    {
        private readonly IWpfTextView _wpfTextView;

        public FloatingDiagnosticHintsViewModel(IWpfTextView wpfTextView)
        {
            _wpfTextView = wpfTextView ?? throw new ArgumentNullException(nameof(wpfTextView));
            Diagnostics = new ObservableCollection<Diagnostic>();
        }

        public ObservableCollection<Diagnostic> Diagnostics
        {
            get;
        }

        public void MergeDiagnostics(IEnumerable<Diagnostic> diagnostics)
        {
            // TZODO: merge logic here
        }
    }
}
