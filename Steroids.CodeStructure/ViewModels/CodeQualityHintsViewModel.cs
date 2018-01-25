using Steroids.Common;
using Steroids.CodeStructure.Models;
namespace Steroids.CodeStructure.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Microsoft.VisualStudio.Text.Editor;
    using Steroids.CodeStructure.Adorners;
    using Steroids.CodeStructure.Analyzers;
    using Steroids.CodeStructure.Analyzers.Services;
    using Steroids.CodeStructure.Helpers;

    public class CodeQualityHintsViewModel : BindableBase
    {
        private readonly IDiagnosticProvider _diagnosticProvider;
        private readonly IWpfTextView _textView;
        private readonly CodeStructureAdorner _adorner;
        private IEnumerable<CodeHintLineEntry> _lineDiagnostics = Enumerable.Empty<CodeHintLineEntry>();

        public CodeQualityHintsViewModel(
            IWpfTextView textView,
            IDiagnosticProvider diagnosticProvider,
            CodeStructureAdorner adorner)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _adorner = adorner ?? throw new ArgumentNullException(nameof(adorner));

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
            WeakEventManager<ITextView, TextViewLayoutChangedEventArgs>.AddHandler(_textView, nameof(ITextView.LayoutChanged), OnTextViewLayoutChanged);
        }

        private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var hint in QualityHints.ToList())
            {
                hint.RefreshPositions();
            }
        }

        public IEnumerable<CodeHintLineEntry> QualityHints
        {
            get { return _lineDiagnostics; }
            private set { Set(ref _lineDiagnostics, value); }
        }

        private void OnDiagnosticsChanged(object sender, DiagnosticsChangedEventArgs args)
        {
            var fileDiagnostics = IWpfTextViewHelper
                .GetDiagnosticsOfEditor(_textView, args.Diagnostics)
                .Where(x => x.IsActive);

            foreach (var diagnostic in fileDiagnostics)
            {
                diagnostic.LineSpan = _textView.TextSnapshot.Lines.ElementAt(diagnostic.Line);
            }

            var lineDiagnostics = fileDiagnostics.ToLookup(x => x.Line);
            QualityHints = lineDiagnostics.Select(x => new CodeHintLineEntry(_textView, x, x.Key));
        }
    }
}
