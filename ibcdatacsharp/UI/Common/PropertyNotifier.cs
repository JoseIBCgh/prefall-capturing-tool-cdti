using System;
using System.ComponentModel;

namespace ibcdatacsharp.Common
{
    /// <summary>
    /// Clase abstracta que implementa INotifyPropertyChanged
    /// </summary>
    [Serializable]
    public abstract class PropertyNotifier : INotifyPropertyChanged
    {
        public PropertyNotifier() : base() { }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Invoca un evento PropertyChangedEventHandler para una propiedad
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}