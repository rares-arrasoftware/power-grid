using System.Globalization;
using System.Windows.Data;
using System;

namespace gui.Helpers
{
    public class LogIndexFormatter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int index)
                return $"[{index + 1:00000}] "; // starts from 00001

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
