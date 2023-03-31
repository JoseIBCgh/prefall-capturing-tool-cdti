using ibcdatacsharp.Common;
using System.Collections.Generic;
using System.Windows.Documents;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda la información de una camara
    public class CameraInfo : BaseObject
    {
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        public int number
        {
            get { return GetValue<int>("number"); }
            set { SetValue("number", value); }
        }
        public List<double> fps
        {
            get { return GetValue<List<double>>("fps"); }
            set { SetValue("fps", value); }
        }
        public CameraInfo(int number, string name, List<double> fps)
        {
            this.number = number;
            this.name = name;
            this.fps = fps;
        }
    }
}
