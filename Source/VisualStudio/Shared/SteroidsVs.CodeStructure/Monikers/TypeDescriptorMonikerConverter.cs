using System;
using System.Globalization;
using System.Windows.Data;
using Microsoft.VisualStudio.Imaging;
using Steroids.CodeStructure.Analyzers;

namespace SteroidsVS.CodeStructure.Monikers
{
    public class TypeDescriptorMonikerConverter : IValueConverter
    {
        private TypeDescriptorMonikerResolver _monikerResolver;

        public TypeDescriptorMonikerConverter()
        {
            _monikerResolver = new TypeDescriptorMonikerResolver();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typeDescriptor = value as ITypeDescriptor;
            if (typeDescriptor is null)
            {
                return KnownMonikers.UnknownMember;
            }

            return _monikerResolver.GetMoniker(typeDescriptor);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
