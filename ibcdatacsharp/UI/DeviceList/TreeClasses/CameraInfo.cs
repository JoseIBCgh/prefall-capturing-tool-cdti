using ibcdatacsharp.Common;


namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda la información de una camara
    public class CameraInfo : BaseObject
    {
        public int number
        {
            get { return GetValue<int>("number"); }
            set { SetValue("number", value); }
        }
        public int? fps
        {
            get { return GetValue<int?>("fps"); }
            set { SetValue("fps", value); }
        }
        public CameraInfo(int number)
        {
            this.number = number;
            this.fps = null;
        }
    }
}
