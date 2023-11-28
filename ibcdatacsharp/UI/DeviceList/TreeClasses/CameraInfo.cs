using ibcdatacsharp.Common;
using System.Collections.Generic;
using System.Windows.Documents;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    /// <summary>
    /// Guarda la informacion de una camara
    /// </summary>
    public class CameraInfo : BaseObject
    {
        /// <summary>
        /// Nombre de la camara
        /// </summary>
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        /// <summary>
        /// Numero de la camara (de opencv)
        /// </summary>
        public int number
        {
            get { return GetValue<int>("number"); }
            set { SetValue("number", value); }
        }
        /// <summary>
        /// Conjunto de fps disponibles para la camara
        /// </summary>
        public List<int> fpsAvailable
        {
            get { return GetValue<List<int>>("fpsAvailable"); }
            set { SetValue("fpsAvailable", value); }
        }
        /// <summary>
        /// fps seleccionados
        /// </summary>
        public int fps
        {
            get { return GetValue<int>("fps"); }
            set { SetValue("fps", value); }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">Numero de la camara (de opencv)</param>
        /// <param name="name">Nombre de la camara</param>
        /// <param name="fpsAvailable">Lista de fps disponibles para la camara</param>
        public CameraInfo(int number, string name, List<int> fpsAvailable)
        {
            this.number = number;
            this.name = name;
            this.fpsAvailable = fpsAvailable;
        }
    }
}
