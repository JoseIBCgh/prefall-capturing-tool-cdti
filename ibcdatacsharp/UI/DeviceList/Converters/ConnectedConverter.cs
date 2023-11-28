using System.Globalization;
using System;
using System.Windows.Data;
using System.Diagnostics;

namespace ibcdatacsharp.UI.DeviceList.Converters
{
    /// <summary>
    /// Clase para cambiar un bool en un circulo verde o rojo
    /// </summary>
    [ValueConversion(typeof(bool), typeof(string))]
    public class ConnectedConverter: IValueConverter
    {
        /// <summary>
        /// Convirte true en un circulo verde y false en uno rojo
        /// </summary>
        /// <param name="value">Valor (tiene que ser un bool)</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">Parametro pasado</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>circulo verde o rojo</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool connected = (bool)value;
            if (connected)
            {
                return new Uri("pack://application:,,,/UI/DeviceList/Icons/green-circle-icon.png");
            }
            else
            {
                return new Uri("pack://application:,,,/UI/DeviceList/Icons/red-circle-icon.png");
            }
        }
        /// <summary>
        /// Revierte el metodo convert
        /// </summary>
        /// <param name="value">Valor (tiene que ser una uri)</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">Parametro pasado</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>true o false</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Uri uriValue = value as Uri;
            string path = uriValue.AbsolutePath;
            return path.Contains("green-circle-icon.png"); //Nombre del fichero de boton verde
        }
    }
}
