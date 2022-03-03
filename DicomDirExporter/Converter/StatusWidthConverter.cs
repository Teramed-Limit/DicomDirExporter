using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DicomDirExporter.Converter
{
    public class StatusWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double winWidth = (double)values[0];
            return (double)winWidth - 650;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}