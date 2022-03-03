using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using DicomDirExporter.Enumerate;

namespace DicomDirExporter.Converter
{
    public class UIQueryFieldTypeVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueUiQueryFieldType = value.ToString();
            var targetUiQueryFieldType = parameter.ToString();
            return valueUiQueryFieldType == targetUiQueryFieldType ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}