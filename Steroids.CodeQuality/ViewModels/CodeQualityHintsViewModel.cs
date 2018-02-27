using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio.Text.Editor;
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

        private IEnumerable<CodeHintLineEntry> _lineDiagnostics = Enumerable.Empty<CodeHintLineEntry>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeQualityHintsViewModel"/> class.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/>.</param>
        /// <param name="diagnosticProvider">The <see cref="IDiagnosticProvider"/>.</param>
        /// <param name="codeHintFactory">The <see cref="CodeHintFactory"/>.</param>
        public CodeQualityHintsViewModel(
            IWpfTextView textView,
            IDiagnosticProvider diagnosticProvider,
            CodeHintFactory codeHintFactory)
        {
            _diagnosticProvider = diagnosticProvider ?? throw new ArgumentNullException(nameof(diagnosticProvider));
            _textView = textView ?? throw new ArgumentNullException(nameof(textView));
            _codeHintFactory = codeHintFactory ?? throw new ArgumentNullException(nameof(codeHintFactory));

            WeakEventManager<IDiagnosticProvider, DiagnosticsChangedEventArgs>.AddHandler(_diagnosticProvider, nameof(IDiagnosticProvider.DiagnosticsChanged), OnDiagnosticsChanged);
            WeakEventManager<ITextView, TextViewLayoutChangedEventArgs>.AddHandler(_textView, nameof(ITextView.LayoutChanged), OnTextViewLayoutChanged);
        }

        public IEnumerable<CodeHintLineEntry> QualityHints
        {
            get { return _lineDiagnostics; }
            private set { Set(ref _lineDiagnostics, value); }
        }

        private void OnDiagnosticsChanged(object sender, DiagnosticsChangedEventArgs args)
        {
            var fileDiagnostics = _textView
                .ExtractRelatedDiagnostics(args.Diagnostics)
                .Where(x => x.IsActive);

            var lineDiagnostics = fileDiagnostics.ToLookup(x => x.Line);
            QualityHints = lineDiagnostics.Select(x => _codeHintFactory.Create(x, x.Key)).ToList();
        }

        private void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (var hint in QualityHints.ToList())
            {
                hint.RefreshPositions();
            }
        }
    }
}
