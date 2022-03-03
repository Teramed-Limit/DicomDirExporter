using System;
using System.Globalization;
using System.Windows.Data;

namespace DicomDirExporter.Converter
{
    public class PercentToAngleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var percent = (double)value;
            if (percent >= 100) return 360.0D;
            return percent / 100 * 360;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}