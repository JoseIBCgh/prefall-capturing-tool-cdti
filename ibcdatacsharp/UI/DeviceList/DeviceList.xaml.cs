using ibcdatacsharp.DeviceList.TreeClasses;
using ScottPlot.Statistics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ibcdatacsharp.UI.DeviceList
{
    public partial class DeviceList : Page
    {
        private const int MAX_IMU_USED = 2;
        private MainWindow mainWindow;
        //private const Key multiselectKey = Key.LeftCtrl;
        //private bool multiSelectionKeyPressed = false;
        //public List<TreeViewItem> selected { get;private set; } 
        public DeviceList()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow as MainWindow;
            baseItem.IsExpanded = true;
            //selected = new List<TreeViewItem>();
            //this.KeyDown += new KeyEventHandler(onKeyDownHandler);
            //this.KeyUp += new KeyEventHandler(onKeyUpHandler);
        }

        // Funciones para eliminar todos los elementos de IMU, camara y Insoles
        #region clear
        public void clearAll()
        {
            clearIMUs();
            clearCameras();
            clearInsoles();
        }
        public void clearIMUs()
        {
            VM.IMUs.Clear();
        }
        public void clearCameras()
        {
            VM.cameras.Clear();
        }
        public void clearInsoles()
        {
            VM.insoles.Clear();
        }
        #endregion
        // Funciones para get y set la coleccion entera de IMU, camara y Insoles
        // y funciones para añadir un elemento a la coleccion
        #region getters setters
        #region IMU
        public IMUInfo getIMU(int id)
        {
            return VM.IMUs.Where((imu) => imu.id == id).First();
        }
        public ObservableCollection<IMUInfo> getIMUs()
        {
            return VM.IMUs;
        }
        public void setIMUs(ObservableCollection<IMUInfo> IMUs)
        {
            VM.IMUs = IMUs;
        }
        public void setIMUs(List<IMUInfo> IMUs)
        {
            // Quitar los IMUs que no se han escaneado 
            // Los que estaban conectados no los escanea de alli => imu.connected ||
            VM.IMUs = new ObservableCollection<IMUInfo>(VM.IMUs.Where(imu => imu.connected || IMUs.Any(newImu => newImu.address == imu.address)));
            foreach (IMUInfo imu in IMUs)
            {
                if (!VM.IMUs.Any(imuOld => imuOld.address == imu.address))
                {
                    VM.IMUs.Add(imu);
                }
                else // Cambiar el id del IMU si es diferente
                {
                    int index = VM.IMUs.ToList().FindIndex(imuOld => imuOld.address == imu.address);
                    if (VM.IMUs[index].id != imu.id)
                    {
                        VM.IMUs[index].id = imu.id;
                    }
                }
            }
        }
        public void addIMU(IMUInfo imu)
        {
            if (VM.IMUs.Any(imuOld => imuOld.address == imu.address))
            {
                return;
                int index = VM.IMUs.ToList().FindIndex(imuOld => imuOld.address == imu.address);
                if (VM.IMUs[index].id != imu.id)
                {
                    VM.IMUs[index].id = imu.id;
                }
            }
            if(VM.IMUs.Any(imuOld => imuOld.id == imu.id))
            {
                int maxID = VM.IMUs.Max(imuOld => imuOld.id);
                imu.id = maxID + 1;
                VM.IMUs.Add(imu);
            }
            else
            {
                VM.IMUs.Add(imu);
            }
        }
        #endregion
        #region Cameras
        public ObservableCollection<CameraInfo> getCameras()
        {
            return VM.cameras;
        }
        public void setCameras(ObservableCollection<CameraInfo> cameras)
        {
            VM.cameras = cameras;
        }
        public void setCameras(List<CameraInfo> cameras)
        {
            VM.cameras = new ObservableCollection<CameraInfo>(VM.cameras.Where(cam => cameras.Any(newCamera => newCamera.name == cam.name)));
            foreach (CameraInfo cam in cameras)
            {
                if (!VM.cameras.Any(camOld => camOld.name == cam.name))
                {
                    VM.cameras.Add(cam);
                }
                else // Cambiar el numero de la camara si es diferente (no se si con las camaras es necesario)
                {
                    int index = VM.cameras.ToList().FindIndex(camOld => camOld.name == cam.name);
                    if (VM.cameras[index].number != cam.number)
                    {
                        VM.cameras[index].number = cam.number;
                    }
                }
            }
        }
        public void addCamera(CameraInfo camera)
        {
            if (!camerainList(camera))
            {
                VM.cameras.Add(camera);
            }
        }
        private bool camerainList(CameraInfo camera)
        {
            foreach (CameraInfo cameraInList in VM.cameras)
            {
                if (camera.number == cameraInList.number) // Tiene que identificar a una camara de forma unica
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #region Insoles
        public ObservableCollection<InsolesInfo> getInsoles()
        {
            return VM.insoles;
        }
        public void setInsoles(ObservableCollection<InsolesInfo> insoles)
        {
            VM.insoles = insoles;
        }
        public void addInsole(InsolesInfo insole)
        {
            if (!VM.insoles.Contains(insole))
            {
                VM.insoles.Add(insole);
            }
        }
        #endregion
        #endregion
        // Funciones para mostrar y esconder el header y los elementos de IMU, camara y Insoles
        #region show hide
        #region IMUs
        public void showIMUs()
        {
            IMUs.Visibility = Visibility.Visible;
        }
        public void hideIMUs()
        {
            IMUs.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region cameras
        public void showCameras()
        {
            cameras.Visibility = Visibility.Visible;
        }
        public void hideCameras()
        {
            cameras.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region insoles
        public void showInsoles()
        {
            insoles.Visibility = Visibility.Visible;
        }
        public void hideInsoles()
        {
            insoles.Visibility = Visibility.Collapsed;
        }
        #endregion
        #endregion
        // Funciones que manejan el hacer doble click sobre un IMU o una Camara
        #region double click handlers
        private void onIMUDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is MultiSelectTreeViewItem)
            {
                if (!((MultiSelectTreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            //connectIMU((MultiSelectTreeViewItem)sender);
        }
        private void onCameraDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is MultiSelectTreeViewItem)
            {
                if (!((MultiSelectTreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            //connectCamera((MultiSelectTreeViewItem)sender);
        }
        #endregion
        public void setIMUHandler(string mac, byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.address == mac).First();
            imuInfo.handler = handler;
        }
        public void connectIMU(string mac, byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.address == mac).First();
            imuInfo.handler = handler;
            imuInfo.connected = true;
        }
        public void updateHeaderInfo(string mac, byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.address == mac).First();
            Dictionary<string, WisewalkSDK.Device> devicesConnected =  mainWindow.api.GetDevicesConnected();
            WisewalkSDK.Device device = devicesConnected[handler.ToString()];
            imuInfo.battery = device.HeaderInfo.battery;
            imuInfo.fw = device.HeaderInfo.fwVersion;
            //imuInfo.battery = mainWindow.devices_list[handler.ToString()].HeaderInfo.battery;
            //imuInfo.fw = mainWindow.devices_list[handler.ToString()].HeaderInfo.fwVersion;
        }
        public void connectIMUs(List<object> treeViewItems)
        {
            foreach(object item in treeViewItems)
            {
                if (item is IMUInfo)
                {
                    MultiSelectTreeViewItem treeViewItem = (MultiSelectTreeViewItem)IMUs.ItemContainerGenerator.ContainerFromItem(item);
                    IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;
                    imuInfo.connected = true;
                    // No estoy seguro de que esta sea la lista y este key

                    imuInfo.battery = mainWindow.devices_list[imuInfo.handler.ToString()].HeaderInfo.battery;
                    imuInfo.fw = mainWindow.devices_list[imuInfo.handler.ToString()].HeaderInfo.fwVersion;
                    treeViewItem.Foreground = new SolidColorBrush(Colors.Green);
                }
            }
        }
        // Funcion que se llama al conectar una camara (doble click o boton connect) para cambiar el TreeView
        public void connectCamera(MultiSelectTreeViewItem treeViewItem)
        {
            int calculateFps(int number)
            {
                return 120;
            }
            CameraInfo cameraInfo = treeViewItem.DataContext as CameraInfo;
            cameraInfo.fps = calculateFps(cameraInfo.number);
        }
        public void disconnectIMU(byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.handler == handler).First();
            imuInfo.connected = false;
            imuInfo.battery = null;
            imuInfo.fw = null;
        }
        public void disconnectIMUs(List<string> IMUsToDisconnect)
        {
            foreach (string mac in IMUsToDisconnect)
            {
                IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.address == mac).First();
                imuInfo.connected = false;
                imuInfo.battery = null;
                imuInfo.fw = null;
            }
        }
        // Funcion que se llama al desconectar un IMU para cambiar el TreeView
        public void disconnectIMU(MultiSelectTreeViewItem treeViewItem)
        {
            IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;
            imuInfo.connected = false;
            imuInfo.battery = null;
            imuInfo.fw = null;
            treeViewItem.Foreground = new SolidColorBrush(Colors.Black);
        }
        private void onCheckIMU(object sender, RoutedEventArgs e)
        {
            if(numIMUsUsed > MAX_IMU_USED)
            {
                MessageBox.Show("Solo puedes seleccionar dos IMUs", caption:null, 
                    button:MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                (sender as CheckBox).IsChecked = false;
            }
        }
        public int numIMUsUsed
        {
            get
            {
                return IMUsUsed.Count;
            }
        }
        public List<IMUInfo> IMUsUsed
        {
            get
            {
                return VM.IMUs.Where(i => i.used && i.connected).ToList();
            }
        }
        public List<IMUInfo> IMUsUnused
        {
            get
            {
                return VM.IMUs.Where(i => !i.used || !i.connected).ToList();
            }
        }

        private void setIMUasReference(object sender, RoutedEventArgs e)
        {
            // Otra forma de sacarlo. Hay que establer la propiedad Tag al valor que quieras en el xaml
            /*
            string id = ((MenuItem)sender).Tag.ToString();
            */
            MenuItem menuItem = (MenuItem)sender;
            IMUInfo imu = (IMUInfo)menuItem.DataContext;
            mainWindow.readQuaternion(imu);
        }
    }
}
