namespace Steroids.SharedUI.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            if (val == null)
            {
                return DependencyProperty.UnsetValue;
            }

            if (IsInverted)
            {
                return !val.Value ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return val.Value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
