using ibcdatacsharp.Common;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda la información de un IMU
    public class IMUInfo : BaseObject
    {
        private static int nextID = 0;
        public int id
        {
            get { return GetValue<int>("id"); }
            set { SetValue("id", value); }
        }
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        public string address
        {
            get { return GetValue<string>("address"); }
            set { SetValue("address", value); }
        }
        public int? battery
        {
            get { return GetValue<int?>("battery"); }
            set { SetValue("battery", value); }
        }
        public bool connected
        {
            get { return GetValue<bool>("connected"); }
            set { 
                SetValue("connected", value);
                if (!value)
                {
                    if (used)
                    {
                        used = false;
                    }
                }
            }
        }
        public bool used
        {
            get { return GetValue<bool>("used"); }
            set { SetValue("used", value); }
        }
        public string? fw
        {
            get { return GetValue<string>("fw"); }
            set { SetValue("fw", value); }
        }

        public byte? handler { get; set; }

        public void checkJAUpdate()
        {
            NotifyChange("connected");
        }
        public IMUInfo() { }
        public IMUInfo(string name, string address)
        {
            this.id = nextID;
            this.name = name;
            this.address = address;
            this.battery = null;
            this.connected = false;
            this.used = false;
            this.fw = null;
            nextID++;
        }
    }
}
