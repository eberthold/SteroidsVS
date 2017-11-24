namespace Steroids.CodeStructure.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.CodeAnalysis;

    public class FloatingDiagnosticHint : Control
    {
        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register("Severity", typeof(DiagnosticSeverity), typeof(FloatingDiagnosticHint), new PropertyMetadata(DiagnosticSeverity.Info));

        static FloatingDiagnosticHint()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(FloatingDiagnosticHint), new FrameworkPropertyMetadata(typeof(FloatingDiagnosticHint)));
        }



        public DiagnosticSeverity Severity
        {
            get { return (DiagnosticSeverity)GetValue(SeverityProperty); }
            set { SetValue(SeverityProperty, value); }
        }
    }
}
