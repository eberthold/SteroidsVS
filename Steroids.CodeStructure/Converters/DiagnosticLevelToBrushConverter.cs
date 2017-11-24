namespace Steroids.CodeStructure.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;
    using System.Windows.Media;
    using Microsoft.CodeAnalysis;

    public class DiagnosticLevelToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the Brush to use, when no diagnostics are present.
        /// </summary>
        public Brush None { get; set; }

        /// <summary>
        /// Gets or sets the Brush to use, when a warning is present.
        /// </summary>
        public Brush Warning { get; set; }

        /// <summary>
        /// Gets or sets the Brush to use, when an error is present.
        /// </summary>
        public Brush Error { get; set; }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var level = value as DiagnosticSeverity?;
            if (level == null)
            {
                return None;
            }

            switch (level)
            {
                case DiagnosticSeverity.Warning:
                    return Warning;

                case DiagnosticSeverity.Error:
                    return Error;

                default:
                    return None;
            }
        }

        /// <inheritdoc />
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
