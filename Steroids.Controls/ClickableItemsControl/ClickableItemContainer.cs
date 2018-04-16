using System.Windows;
using System.Windows.Controls;

namespace Steroids.Controls
{
    public class ClickableItemContainer : ContentControl
    {
        static ClickableItemContainer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ClickableItemContainer), new FrameworkPropertyMetadata(typeof(ClickableItemContainer)));
        }
    }
}
