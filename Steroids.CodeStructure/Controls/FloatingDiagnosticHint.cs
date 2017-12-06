namespace Steroids.CodeStructure.Controls
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.CodeAnalysis;
    using Steroids.CodeStructure.Analyzers;

    public class FloatingDiagnosticHint : Control
    {
        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register("Severity", typeof(DiagnosticSeverity), typeof(FloatingDiagnosticHint), new PropertyMetadata(DiagnosticSeverity.Info));
        public static readonly DependencyProperty DiagnosticsProperty = DependencyProperty.Register("Diagnostics", typeof(IEnumerable<DiagnosticInfo>), typeof(FloatingDiagnosticHint), new PropertyMetadata(new List<DiagnosticInfo>()));
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(FloatingDiagnosticHint), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(FloatingDiagnosticHint), new PropertyMetadata(string.Empty));

        static FloatingDiagnosticHint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingDiagnosticHint), new FrameworkPropertyMetadata(typeof(FloatingDiagnosticHint)));
        }

        public string Code
        {
            get { return (string)GetValue(CodeProperty); }
            set { SetValue(CodeProperty, value); }
        }

        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }

        public IEnumerable<DiagnosticInfo> Diagnostics
        {
            get { return (IEnumerable<DiagnosticInfo>)GetValue(DiagnosticsProperty); }
            set { SetValue(DiagnosticsProperty, value); }
        }

        public DiagnosticSeverity Severity
        {
            get { return (DiagnosticSeverity)GetValue(SeverityProperty); }
            set { SetValue(SeverityProperty, value); }
        }
    }
}
