namespace Steroids.CodeStructure.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public class IndicatorButton : CheckBox
    {
        static IndicatorButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IndicatorButton), new FrameworkPropertyMetadata(typeof(IndicatorButton)));
        }
    }
}
