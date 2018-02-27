using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Steroids.CodeStructure.UI
{
    public class LevelToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = value as int?;
            if (level == null)
            {
                return Binding.DoNothing;
            }

            var realLevel = level.Value;

            var currentThickness = parameter as Thickness?;
            if (currentThickness.HasValue)
            {
                new Thickness(realLevel * 12, currentThickness.Value.Top, currentThickness.Value.Right, currentThickness.Value.Bottom);
            }

            return new Thickness(realLevel * 12, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
