using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ibcdatacsharp.UI.DeviceList.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    internal class JAEnabledConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool connected = (bool)value;
            if (!connected)
            {
                return false;
            }
            return ((MainWindow)System.Windows.Application.Current.MainWindow).virtualToolBar.recordState == ToolBar.Enums.RecordState.RecordStopped;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool)value;
            if (result)
            {
                return true;
            }
            return ((MainWindow)System.Windows.Application.Current.MainWindow).virtualToolBar.recordState == ToolBar.Enums.RecordState.Recording;
        }
    }
}
