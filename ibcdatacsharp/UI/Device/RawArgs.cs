namespace ibcdatacsharp.UI.Device
{
    /// <summary>
    /// Datos del acc, mag y gyr
    /// </summary>
    public class RawArgs
    {
        /// <summary>
        /// Datos del acelerometro
        /// </summary>
        public double[] accelerometer { get; set; }
        /// <summary>
        /// Datos del giroscopio
        /// </summary>
        public double[] gyroscope { get; set; }
        /// <summary>
        /// Datos del magnetometro
        /// </summary>
        public double[] magnetometer { get; set; }
    }
}
