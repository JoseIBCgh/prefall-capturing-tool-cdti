using ibcdatacsharp.Common;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda la información de un IMU
    public class IMUInfo : BaseObject
    {
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        public string adress
        {
            get { return GetValue<string>("adress"); }
            set { SetValue("adress", value); }
        }
        public int? battery
        {
            get { return GetValue<int?>("battery"); }
            set { SetValue("battery", value); }
        }
        public bool connected
        {
            get { return GetValue<bool>("connected"); }
            set { SetValue("connected", value); }
        }
        public IMUInfo(string name, string adress)
        {
            this.name = name;
            this.adress = adress;
            this.battery = null;
            this.connected = false;
        }
    }
}
