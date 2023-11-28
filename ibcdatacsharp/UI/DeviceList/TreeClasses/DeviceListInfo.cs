using ibcdatacsharp.Common;
using System.Collections.ObjectModel;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    /// <summary>
    /// Guarda una lista de IMUs, una lista de camaras y una lista de insoles
    /// </summary>
    public class DeviceListInfo: BaseObject
    {
        /// <summary>
        /// Lista de IMUs
        /// </summary>
        public ObservableCollection<IMUInfo> IMUs
        {
            get { return GetValue<ObservableCollection<IMUInfo>>("IMUs"); }
            set { SetValue("IMUs", value); }
        }
        /// <summary>
        /// Lista de camaras
        /// </summary>
        public ObservableCollection<CameraInfo> cameras
        {
            get { return GetValue<ObservableCollection<CameraInfo>>("cameras"); }
            set { SetValue("cameras", value); }
        }
        /// <summary>
        /// Lista de insoles
        /// </summary>
        public ObservableCollection<InsolesInfo> insoles
        {
            get { return GetValue<ObservableCollection<InsolesInfo>>("insoles"); }
            set { SetValue("insoles", value); }
        }
        /// <summary>
        /// Llama a checkJAUpdate para cada IMU de la lista
        /// </summary>
        public void checkJAUpdate()
        {
            foreach (var item in IMUs)
            {
                item.checkJAUpdate();
            }
        }
        /// <summary>
        /// Inicializa las 3 listas vacias
        /// </summary>
        public DeviceListInfo()
        {
            IMUs = new ObservableCollection<IMUInfo>();
            cameras = new ObservableCollection<CameraInfo>();
            insoles = new ObservableCollection<InsolesInfo>();
        }
    }
}
