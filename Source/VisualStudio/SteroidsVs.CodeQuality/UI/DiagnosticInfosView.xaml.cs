using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.Text.Editor;

namespace SteroidsVS.CodeQuality.UI
{
    public partial class DiagnosticInfosView : UserControl
    {
        public static readonly DependencyProperty TextViewProperty = DependencyProperty.Register("TextView", typeof(IWpfTextView), typeof(DiagnosticInfosView), new PropertyMetadata(null));

        public DiagnosticInfosView()
        {
            InitializeComponent();
        }

        public IWpfTextView TextView
        {
            get { return (IWpfTextView)GetValue(TextViewProperty); }
            set { SetValue(TextViewProperty, value); }
        }
    }
}
