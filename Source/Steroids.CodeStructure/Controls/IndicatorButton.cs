using System.Windows;
using System.Windows.Controls;

namespace Steroids.CodeStructure.Controls
{
    public class IndicatorButton : CheckBox
    {
        static IndicatorButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IndicatorButton), new FrameworkPropertyMetadata(typeof(IndicatorButton)));
        }
    }
}
