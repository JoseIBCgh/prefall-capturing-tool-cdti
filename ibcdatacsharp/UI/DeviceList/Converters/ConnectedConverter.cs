using System.Globalization;
using System;
using System.Windows.Data;

namespace ibcdatacsharp.UI.DeviceList.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class ConnectedConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool connected = (bool)value;
            if (connected)
            {
                return "YES";
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            return strValue == "YES";
        }
    }
}
