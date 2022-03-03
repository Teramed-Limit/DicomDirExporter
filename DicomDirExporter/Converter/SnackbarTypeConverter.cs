using System;
using System.Globalization;
using System.Windows.Data;
using DicomDirExporter.Enumerate;

namespace DicomDirExporter.Converter
{
    public class SnackbarTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string color;
            switch (value)
            {
                case SnackbarType.ERROR:
                    {
                        color = "#f44336";
                        break;
                    }
                case SnackbarType.SUCCESS:
                    {
                        color = "#4caf50";
                        break;
                    }
                case SnackbarType.INFO:
                    {
                        color = "#2196f3";
                        break;
                    }
                case SnackbarType.WARRING:
                    {
                        color = "#ff9800";
                        break;
                    }
                default:
                    {
                        color = "#4caf50";
                        break;
                    }
            }

            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}