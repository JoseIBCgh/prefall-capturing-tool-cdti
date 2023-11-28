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
    /// <summary>
    /// Lista de dispositivos
    /// </summary>
    public partial class DeviceList : Page
    {
        private const int MAX_IMU_USED = 2;
        private MainWindow mainWindow;
        //private const Key multiselectKey = Key.LeftCtrl;
        //private bool multiSelectionKeyPressed = false;
        //public List<TreeViewItem> selected { get;private set; } 
        /// <summary>
        /// Constructor. Asigna los eventos pertinentes
        /// </summary>
        public DeviceList()
        {
            InitializeComponent();
            mainWindow = Application.Current.MainWindow as MainWindow;
            baseItem.IsExpanded = true;
            mainWindow.virtualToolBar.recordChanged += (s, e) =>
            {
                VM.checkJAUpdate();
            };
            //selected = new List<TreeViewItem>();
            //this.KeyDown += new KeyEventHandler(onKeyDownHandler);
            //this.KeyUp += new KeyEventHandler(onKeyUpHandler);
        }

        // Funciones para eliminar todos los elementos de IMU, camara y Insoles
        #region clear
        /// <summary>
        /// Borra todo
        /// </summary>
        public void clearAll()
        {
            clearIMUs();
            clearCameras();
            clearInsoles();
        }
        /// <summary>
        /// Borra los IMUs
        /// </summary>
        public void clearIMUs()
        {
            VM.IMUs.Clear();
        }
        /// <summary>
        /// Borra las camaras
        /// </summary>
        public void clearCameras()
        {
            VM.cameras.Clear();
        }
        /// <summary>
        /// Borra las Insoles
        /// </summary>
        public void clearInsoles()
        {
            VM.insoles.Clear();
        }
        #endregion
        // Funciones para get y set la coleccion entera de IMU, camara y Insoles
        // y funciones para añadir un elemento a la coleccion
        #region getters setters
        #region IMU
        /// <summary>
        /// Busca un IMU por id
        /// </summary>
        /// <param name="id">id del IMU</param>
        /// <returns>Devuelve el primer IMU con id == id</returns>
        public IMUInfo getIMU(int id)
        {
            return VM.IMUs.Where((imu) => imu.id == id).First();
        }
        /// <summary>
        /// Devuelve todos los IMUs
        /// </summary>
        /// <returns>La coleccion de IMUs.</returns>
        public ObservableCollection<IMUInfo> getIMUs()
        {
            return VM.IMUs;
        }
        /// <summary>
        /// Asigna todos los IMUs. Reemplaza los antiguos.
        /// </summary>
        /// <param name="IMUs">ObservableCollection de IMUs</param>
        public void setIMUs(ObservableCollection<IMUInfo> IMUs)
        {
            VM.IMUs = IMUs;
        }
        /// <summary>
        /// Añade todos los IMUs. Borra de la antigua lista los que no estan en la nueva lista y no estan conectados. Asigna un ID a los nuevos.
        /// </summary>
        /// <param name="IMUs">IMUs a añadir</param>
        public void setIMUs(List<IMUInfo> IMUs)
        {
            // Quitar los IMUs que no se han escaneado 
            // Los que estaban conectados no los escanea de alli => imu.connected ||
            List<IMUInfo> IMUsToRemove = new List<IMUInfo>();
            foreach(IMUInfo imuOld in VM.IMUs)
            {
                if (!IMUs.Any(imuNew => imuNew.address == imuOld.address) && !imuOld.connected)
                {
                    IMUsToRemove.Add(imuOld);
                }
            }
            foreach(IMUInfo imu in IMUsToRemove)
            {
                IMUInfo.removeIMU(imu);
                VM.IMUs.Remove(imu);
            }
            //VM.IMUs = new ObservableCollection<IMUInfo>(VM.IMUs.Where(imu => imu.connected || IMUs.Any(newImu => newImu.address == imu.address)));
            foreach (IMUInfo imu in IMUs)
            {
                if (!VM.IMUs.Any(imuOld => imuOld.address == imu.address))
                {
                    imu.setID();
                    VM.IMUs.Add(imu);
                }
            }
        }
        /*
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
        */
        #endregion
        #region Cameras
        /// <summary>
        /// Obtiene las camaras
        /// </summary>
        /// <returns>La coleccion de las camaras</returns>
        public ObservableCollection<CameraInfo> getCameras()
        {
            return VM.cameras;
        }
        /// <summary>
        /// Asigna todas las camaras. Reemplaza las antiguas.
        /// </summary>
        /// <param name="cameras">Nueva coleccion de camaras</param>
        public void setCameras(ObservableCollection<CameraInfo> cameras)
        {
            VM.cameras = cameras;
        }
        /// <summary>
        /// Añade una lista de camaras. Borra las antiguas que no estan en la nueva lista.Cambia los indices si han cambiado.
        /// </summary>
        /// <param name="cameras">Camaras a añadir</param>
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
        /// <summary>
        /// Añade una camara
        /// </summary>
        /// <param name="camera">Camara a añadir</param>
        public void addCamera(CameraInfo camera)
        {
            if (!camerainList(camera))
            {
                VM.cameras.Add(camera);
            }
        }
        /// <summary>
        /// Busca una camara en la lista
        /// </summary>
        /// <param name="camera">Camara a buscar</param>
        /// <returns>True si la camara esta en la lista</returns>
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
        /// <summary>
        /// Devuelve la coleccion de insoles
        /// </summary>
        /// <returns>La coleccion de Insoles</returns>
        public ObservableCollection<InsolesInfo> getInsoles()
        {
            return VM.insoles;
        }
        /// <summary>
        /// Asigna la coleccion de insoles
        /// </summary>
        /// <param name="insoles">Nueva coleccion</param>
        public void setInsoles(ObservableCollection<InsolesInfo> insoles)
        {
            VM.insoles = insoles;
        }
        /// <summary>
        /// Añade una insole nueva
        /// </summary>
        /// <param name="insole">Insole a añadir</param>
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
        /// <summary>
        /// Muestra todos los IMUs
        /// </summary>
        public void showIMUs()
        {
            IMUs.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Esconde todos los IMUs
        /// </summary>
        public void hideIMUs()
        {
            IMUs.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region cameras
        /// <summary>
        /// Muestra todas las camaras
        /// </summary>
        public void showCameras()
        {
            cameras.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Esconde todas las camaras
        /// </summary>
        public void hideCameras()
        {
            cameras.Visibility = Visibility.Collapsed;
        }
        #endregion
        #region insoles
        /// <summary>
        /// Muestra todos los insoles
        /// </summary>
        public void showInsoles()
        {
            insoles.Visibility = Visibility.Visible;
        }
        /// <summary>
        /// Esconde todos los insoles
        /// </summary>
        public void hideInsoles()
        {
            insoles.Visibility = Visibility.Collapsed;
        }
        #endregion
        #endregion
        // Funciones que manejan el hacer doble click sobre un IMU o una Camara
        #region double click handlers
        /// <summary>
        /// Si se descomenta la linea connecta los IMUs al hacer doble click
        /// </summary>
        /// <param name="sender">Objeto que envia el evento</param>
        /// <param name="args">argumentos</param>
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
        /// <summary>
        /// Si se descomenta la linea connecta las camaras al hacer doble click
        /// </summary>
        /// <param name="sender">Objeto que envia el evento</param>
        /// <param name="args">argumentos</param>
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
        /// <summary>
        /// Cambia el handler de un IMU en la lista
        /// </summary>
        /// <param name="mac">MAC del IMU</param>
        /// <param name="handler">nuevo handler</param>
        public void setIMUHandler(string mac, byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.address == mac).First();
            imuInfo.handler = handler;
        }
        /// <summary>
        /// Connecta un IMU en la lista y cambia su handler
        /// </summary>
        /// <param name="mac">MAC del IMU</param>
        /// <param name="handler">nuevo handler</param>
        public void connectIMU(string mac, byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.address == mac).First();
            imuInfo.handler = handler;
            imuInfo.connected = true;
        }
        /// <summary>
        /// Actualiza la bateria y firmware de un IMU
        /// </summary>
        /// <param name="mac">MAC del IMU</param>
        /// <param name="handler">nuevo handler</param>
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
        /// <summary>
        /// Connecta una lista de IMUs
        /// </summary>
        /// <param name="treeViewItems">Lista de objetos a conectar</param>
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
        /// <summary>
        /// Desconecta un IMU de la lista, borra la bateria y firmware
        /// </summary>
        /// <param name="handler">handler del IMU</param>
        public void disconnectIMU(byte handler)
        {
            IMUInfo imuInfo = VM.IMUs.Where((imu) => imu.handler == handler).First();
            imuInfo.connected = false;
            imuInfo.battery = null;
            imuInfo.fw = null;
        }
        /// <summary>
        /// Desconecta un conjunto de IMUs de la lista
        /// </summary>
        /// <param name="IMUsToDisconnect">lista de MACs</param>
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
        /// <summary>
        /// Funcion que se llama al desconectar un IMU para cambiar el TreeView
        /// </summary>
        /// <param name="treeViewItem">MultiSelectTreeViewItem cuyo DataContext contiene el IMUInfo</param>
        public void disconnectIMU(MultiSelectTreeViewItem treeViewItem)
        {
            IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;
            imuInfo.connected = false;
            imuInfo.battery = null;
            imuInfo.fw = null;
            treeViewItem.Foreground = new SolidColorBrush(Colors.Black);
        }
        /// <summary>
        /// Descomentar para limitar el maximo numero de IMUs que puede usarse
        /// </summary>
        /// <param name="sender">objeto que envia el evento</param>
        /// <param name="e">argumentos del evento</param>
        private void onCheckIMU(object sender, RoutedEventArgs e)
        {
            /*
            if (numIMUsUsed > MAX_IMU_USED)
            {
                MessageBox.Show("Solo puedes seleccionar dos IMUs", caption: null,
                    button: MessageBoxButton.OK, icon: MessageBoxImage.Warning);
                (sender as CheckBox).IsChecked = false;
            }
            */
        }
        /// <summary>
        /// numero de IMUs usados
        /// </summary>
        /// <returns>Numero de IMUs usados</returns>
        public int numIMUsUsed
        {
            get
            {
                return IMUsUsed.Count;
            }
        }
        /// <summary>
        /// IMUs usados
        /// </summary>
        /// <returns>Lista de IMUs usados</returns>
        public List<IMUInfo> IMUsUsed
        {
            get
            {
                return VM.IMUs.Where(i => i.used && i.connected).ToList();
            }
        }
        /// <summary>
        /// IMUs no usados
        /// </summary>
        /// <returns>Lista de IMUs no usados</returns>
        public List<IMUInfo> IMUsUnused
        {
            get
            {
                return VM.IMUs.Where(i => !i.used || !i.connected).ToList();
            }
        }
        /// <summary>
        /// Handler que settea el IMU como referencia
        /// </summary>
        /// <param name="sender">Objeto que envia el evento. Tiene que ser un MenuItem con un IMUInfo como DataContext</param>
        /// <param name="e">argumentos del evento</param>
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
        /// <summary>
        /// Handler que calibra el IMU
        /// </summary>
        /// <param name="sender">Objeto que envia el evento. Tiene que ser un MenuItem con un IMUInfo como DataContext</param>
        /// <param name="e">argumentos del evento</param>
        private void calibDevice(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;
            IMUInfo imu = (IMUInfo)menuItem.DataContext;
            mainWindow.calibDevice(imu);
        }
        /// <summary>
        /// Comprueba si un IMU tiene DISMAG a true
        /// </summary>
        /// <param name="MAC">MAC del IMU a comprobar</param>
        public bool IsDISMAG(string MAC)
        {
            return VM.IMUs.First(i => i.address == MAC).DISMAG;
        }
    }
}
