using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Steroids.Core.CodeQuality;

namespace SteroidsVS.CodeQuality.UI
{
    public class DiagnosticInfoControl : Control
    {
        public static readonly DependencyProperty SeverityProperty = DependencyProperty.Register("Severity", typeof(DiagnosticSeverity), typeof(DiagnosticInfoControl), new PropertyMetadata(DiagnosticSeverity.Info));
        public static readonly DependencyProperty DiagnosticsProperty = DependencyProperty.Register("Diagnostics", typeof(IEnumerable<DiagnosticInfo>), typeof(DiagnosticInfoControl), new PropertyMetadata(new List<DiagnosticInfo>()));
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register("Code", typeof(string), typeof(DiagnosticInfoControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty MessageProperty = DependencyProperty.Register("Message", typeof(string), typeof(DiagnosticInfoControl), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof(double), typeof(DiagnosticInfoControl), new PropertyMetadata(1.0));

        static DiagnosticInfoControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DiagnosticInfoControl), new FrameworkPropertyMetadata(typeof(DiagnosticInfoControl)));
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

        public double ScaleFactor
        {
            get { return (double)GetValue(ScaleFactorProperty); }
            set { SetValue(ScaleFactorProperty, value); }
        }
    }
}
