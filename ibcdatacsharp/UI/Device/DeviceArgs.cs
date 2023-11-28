using ibcdatacsharp.UI.Timer;
using System;

namespace ibcdatacsharp.UI.Device
{
    /// <summary>
    /// Conjunto de todos los datos del dispositivo
    /// </summary>
    public class DeviceArgs : EventArgs
    {
        /// <summary>
        /// Datos del timer
        /// </summary>
        public FrameArgs frameArgs { get; set; }
        /// <summary>
        /// Datos de acc, mag y gyr
        /// </summary>
        public RawArgs rawArgs { get; set; }
        /// <summary>
        /// Datos de los angulos
        /// </summary>
        public AngleArgs angleArgs { get; set; }
    }
}
