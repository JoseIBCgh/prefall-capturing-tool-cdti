﻿using ibcdatacsharp.DeviceList.TreeClasses;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ibcdatacsharp.DeviceList
{
    public partial class DeviceList : Page
    {
        public DeviceList()
        {
            InitializeComponent();
            lastSelectedItem = null;
        }
        public TreeViewItem? lastSelectedItem
        {
            get;
            set;
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
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            connectIMU((TreeViewItem)sender);
        }
        private void onCameraDoubleClick(object sender, MouseButtonEventArgs args)
        {
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                {
                    return;
                }
            }
            connectCamera((TreeViewItem)sender);
        }
        #endregion
        // Funcion que se llama al conectar un IMU (doble click o boton connect) para cambiar el TreeView
        public void connectIMU(TreeViewItem treeViewItem)
        {
            IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;
            imuInfo.connected = true;
            treeViewItem.Foreground = new SolidColorBrush(Colors.Green);
        }
        // Funcion que se llama al conectar una camara (doble click o boton connect) para cambiar el TreeView
        public void connectCamera(TreeViewItem treeViewItem)
        {
            int calculateFps(int number)
            {
                return 120;
            }
            CameraInfo cameraInfo = treeViewItem.DataContext as CameraInfo;
            cameraInfo.fps = calculateFps(cameraInfo.number);
        }
    }
}
