using ibcdatacsharp.Common;
using System.Collections.ObjectModel;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda una lista de IMUs una lista de Camaras y una lista de Insoles
    public class DeviceListInfo: BaseObject
    {
        public ObservableCollection<IMUInfo> IMUs
        {
            get { return GetValue<ObservableCollection<IMUInfo>>("IMUs"); }
            set { SetValue("IMUs", value); }
        }
        public ObservableCollection<CameraInfo> cameras
        {
            get { return GetValue<ObservableCollection<CameraInfo>>("cameras"); }
            set { SetValue("cameras", value); }
        }
        public ObservableCollection<InsolesInfo> insoles
        {
            get { return GetValue<ObservableCollection<InsolesInfo>>("insoles"); }
            set { SetValue("insoles", value); }
        }
        public void checkJAUpdate()
        {
            foreach (var item in IMUs)
            {
                item.checkJAUpdate();
            }
        }
        public DeviceListInfo()
        {
            IMUs = new ObservableCollection<IMUInfo>();
            cameras = new ObservableCollection<CameraInfo>();
            insoles = new ObservableCollection<InsolesInfo>();
        }
    }
}
