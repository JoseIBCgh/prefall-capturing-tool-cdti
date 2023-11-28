using System.Globalization;
using System;
using System.Windows.Data;

namespace ibcdatacsharp.UI.DeviceList.Converters
{
    /// <summary>
    /// Clase para añadir '%' al numero
    /// </summary>
    [ValueConversion(typeof(int), typeof(string))]
    public class BatteryConverter : IValueConverter
    {
        /// <summary>
        /// Añade '%' al numero
        /// </summary>
        /// <param name="value">Valor (tiene que ser un int o null)</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">Parametro pasado</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>value + '%'</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                int battery = (int)value;
                return battery.ToString()+"%";
            }
        }
        /// <summary>
        /// Quita '%' al numero
        /// </summary>
        /// <param name="value">Valor (tiene que ser un int o null)</param>
        /// <param name="targetType">Type</param>
        /// <param name="parameter">Parametro pasado</param>
        /// <param name="culture">CultureInfo</param>
        /// <returns>value - '%'</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if(strValue == string.Empty)
            {
                return null;
            }
            else
            {
                string strNum = strValue.Remove(strValue.IndexOf("%"));
                return int.Parse(strNum);
            }
        }
    }
}