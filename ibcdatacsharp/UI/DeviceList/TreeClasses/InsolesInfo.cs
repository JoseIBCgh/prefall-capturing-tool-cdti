using ibcdatacsharp.Common;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    /// <summary>
    /// Guarda la información de una Insole
    /// </summary>
    public class InsolesInfo : BaseObject
    {
        /// <summary>
        /// Nombre de la Insole
        /// </summary>
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        /// <summary>
        /// Lado de la Insole (L or R)
        /// </summary>
        public string side
        {
            get { return GetValue<string>("side"); }
            set { SetValue("side", value); }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Nombre de la insole</param>
        /// <param name="side">Lado de la insole</param>
        public InsolesInfo(string name, string side)
        {
            this.name = name;
            this.side = side;
        }
    }
}
