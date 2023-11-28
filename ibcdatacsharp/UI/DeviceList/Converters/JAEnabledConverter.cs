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
    /// <summary>
    /// Clase para desactivar el JA si esta grabando
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    internal class JAEnabledConverter: IValueConverter
    {
        /// <summary>
        /// Convirte un valor en false si esta grabando
        /// </summary>
        /// <param name="value">Valor (tiene que ser un bool)</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">Parametro pasado</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>Si esta grabando devuelve false si no value</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool connected = (bool)value;
            if (!connected)
            {
                return false;
            }
            return ((MainWindow)System.Windows.Application.Current.MainWindow).virtualToolBar.recordState == ToolBar.Enums.RecordState.RecordStopped;
        }
        /// <summary>
        /// Revierte el metodo convert
        /// </summary>
        /// <param name="value">Valor (tiene que ser un bool)</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">Parametro pasado</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>Si value es true devuelve true. Sino devuelve true si esta grabando</returns>
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
