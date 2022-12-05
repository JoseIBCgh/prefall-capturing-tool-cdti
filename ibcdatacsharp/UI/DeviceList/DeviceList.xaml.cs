using ibcdatacsharp.DeviceList.TreeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ibcdatacsharp.UI.DeviceList
{
    public partial class DeviceList : Page
    {
        private const int MAX_IMU_USED = 2;
        //private const Key multiselectKey = Key.LeftCtrl;
        //private bool multiSelectionKeyPressed = false;
        //public List<TreeViewItem> selected { get;private set; } 
        public DeviceList()
        {
            InitializeComponent();
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
        public ObservableCollection<IMUInfo> getIMUs()
        {
            return VM.IMUs;
        }
        public void setIMUs(ObservableCollection<IMUInfo> IMUs)
        {
            VM.IMUs = IMUs;
        }
        public void addIMU(IMUInfo IMU)
        {
            VM.IMUs.Add(IMU);
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
        public void addCamera(CameraInfo camera)
        {
            VM.cameras.Add(camera);
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
            VM.insoles.Add(insole);
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
        public void connectIMUs(IList<object> treeViewItems)
        {
            foreach(object item in treeViewItems)
            {
                if (item is IMUInfo)
                {
                    MultiSelectTreeViewItem treeViewItem = (MultiSelectTreeViewItem)IMUs.ItemContainerGenerator.ContainerFromItem(item);
                    IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;
                    imuInfo.connected = true;
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
        // Funcion que se llama al desconectar un IMU para cambiar el TreeView
        public void disconnectIMU(MultiSelectTreeViewItem treeViewItem)
        {
            IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;
            imuInfo.connected = false;
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
                return VM.IMUs.Where(i => i.used).ToList();
            }
        }
        public List<IMUInfo> IMUsUnused
        {
            get
            {
                return VM.IMUs.Where(i => !i.used).ToList();
            }
        }
    }
}
