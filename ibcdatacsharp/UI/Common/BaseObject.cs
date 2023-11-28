using System;
using System.Collections.Generic;

namespace ibcdatacsharp.Common
{
    /// <summary>
    /// Clase base de todos nodos de los TreeView
    /// </summary>
    [Serializable]
    public abstract class BaseObject : PropertyNotifier
    {
        private IDictionary<string, object> m_values = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// Devuelve una propiedad del objeto
        /// </summary>
        /// <param name="key">Nombre de la propiedad</param>
        /// <returns>Valor de la propiedad</returns>
        public T GetValue<T>(string key)
        {
            var value = GetValue(key);
            return (value is T) ? (T)value : default(T);
        }

        private object GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return m_values.ContainsKey(key) ? m_values[key] : null;
        }
        /// <summary>
        /// Actualiza una propiedad del objeto
        /// </summary>
        /// <param name="key">Nombre de la propiedad</param>
        /// <param name="value">Valor a actualizar</param>
        public void SetValue(string key, object value)
        {
            if (!m_values.ContainsKey(key))
            {
                m_values.Add(key, value);
            }
            else
            {
                m_values[key] = value;
            }
            OnPropertyChanged(key);
        }
        /// <summary>
        /// Notifica que una propiedad ha cambiado
        /// </summary>
        /// <param name="key">Nombre de la propiedad</param>
        public void NotifyChange(string key)
        {
            OnPropertyChanged(key);
        }
    }
}