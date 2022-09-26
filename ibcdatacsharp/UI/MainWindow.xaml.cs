using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ibcdatacsharp.DeviceList.TreeClasses;
using ibcdatacsharp.UI.ToolBar.Enums;
using ibcdatacsharp.UI.Common;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ibcdatacsharp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            initToolBarHandlers();
            initMenuHandlers();
        }
        // Conecta los botones de la ToolBar
        private void initToolBarHandlers()
        {
            toolBar.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                toolBarClass.scan.Click += new RoutedEventHandler(onScan);
                toolBarClass.connect.Click += new RoutedEventHandler(onConnect);
                toolBarClass.disconnect.Click += new RoutedEventHandler(onDisconnect);
                toolBarClass.openCamera.Click += new RoutedEventHandler(onOpenCamera);
                toolBarClass.capture.Click += new RoutedEventHandler(onCapture);
                toolBarClass.pause.Click += new RoutedEventHandler(onPause);
                toolBarClass.stop.Click += new RoutedEventHandler(onStop);
            };
        }
        // Conecta los botones del Menu
        private void initMenuHandlers()
        {
            menuBar.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                menuBarClass.scan.Click += new RoutedEventHandler(onScan);
                menuBarClass.connect.Click += new RoutedEventHandler(onConnect);
                menuBarClass.disconnect.Click += new RoutedEventHandler(onDisconnect);
                menuBarClass.openCamera.Click += new RoutedEventHandler(onOpenCamera);
                menuBarClass.capture.Click += new RoutedEventHandler(onCapture);
                menuBarClass.pause.Click += new RoutedEventHandler(onPause);
                menuBarClass.stop.Click += new RoutedEventHandler(onStop);
                menuBarClass.exit.Click += new RoutedEventHandler(onExit);
            };
        }
        // Funcion que llaman todos los handlers del ToolBar. Por si acaso el device list no se ha cargado.
        private void deviceListLoadedCheck(Action func)
        {
            if (deviceList.Content == null)
            {
                deviceList.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    func();
                };
            }
            else
            {
                func();
            }
        }
        // Conecta el boton scan
        private void onScan(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton scan
            void onScanFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                deviceListClass.clearAll();
                deviceListClass.addIMU(new IMUInfo("IMU1", "AX"));
                deviceListClass.addIMU(new IMUInfo("IMU2", "BX"));
                deviceListClass.addCamera(new CameraInfo(0));
                ObservableCollection<IMUInfo> IMUs = deviceListClass.getIMUs();
                deviceListClass.showIMUs();
                deviceListClass.showCameras();
                deviceListClass.hideInsoles(); //Por defecto estan escondidos pero si los muestras una vez los tienes que volver a esconder
            }
            deviceListLoadedCheck(onScanFunction);
        }
        // Conecta el boton connect
        private void onConnect(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton connect
            void onConnectFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                object selected = deviceListClass.treeView.SelectedItem;
                if (selected != null)
                {
                    if (selected is IMUInfo)
                    {
                        TreeViewItem treeViewItem = (TreeViewItem)deviceListClass.IMUs.ItemContainerGenerator.ContainerFromItem(selected);
                        deviceListClass.connectIMU(treeViewItem);
                    }
                    else if (selected is CameraInfo)
                    {
                        TreeViewItem treeViewItem = (TreeViewItem)deviceListClass.cameras.ItemContainerGenerator.ContainerFromItem(selected);
                        deviceListClass.connectCamera(treeViewItem);
                    }
                }
            }
            deviceListLoadedCheck(onConnectFunction);
        }
        // Conecta el boton disconnect
        private void onDisconnect(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton disconnect
            void onDisconnectFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                object selected = deviceListClass.treeView.SelectedItem;
                if (selected != null && selected is IMUInfo)
                {
                    TreeViewItem treeViewItem = (TreeViewItem)deviceListClass.IMUs.ItemContainerGenerator.ContainerFromItem(selected);
                    deviceListClass.disconnectIMU(treeViewItem);
                }
            }
            deviceListLoadedCheck(onDisconnectFunction);
        }
        // Conecta el boton Open Camera
        private void onOpenCamera(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Open Camera
            void onOpenCameraFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                object selected = deviceListClass.treeView.SelectedItem;
                if (selected != null && selected is CameraInfo)
                {
                    CameraInfo cameraInfo = (CameraInfo)selected;
                    int id = cameraInfo.number; //Id de la camara
                    Trace.WriteLine(id);
                }
            }
            deviceListLoadedCheck(onOpenCameraFunction);
        }
        // Conecta el boton Capture
        private void onCapture(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Capture
            void onCaptureFunction()
            {
                GraphWindow.GraphWindow graphWindowClass = graphWindow.Content as GraphWindow.GraphWindow;
                graphWindowClass.play();
            }
            deviceListLoadedCheck(onCaptureFunction);
        }
        // Conecta el boton Pause
        private void onPause(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Pause
            void onPauseFunction()
            {
                GraphWindow.GraphWindow graphWindowClass = graphWindow.Content as GraphWindow.GraphWindow;
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                graphWindowClass.pause(toolBarClass, menuBarClass);
            }
            deviceListLoadedCheck(onPauseFunction);
        }
        // Conecta el boton Stop
        private void onStop(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Stop
            void onStopFunction()
            {
                GraphWindow.GraphWindow graphWindowClass = graphWindow.Content as GraphWindow.GraphWindow;
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                graphWindowClass.stop(toolBarClass, menuBarClass);
            }
            deviceListLoadedCheck(onStopFunction);
        }
        // Conecta el menu Exit
        private void onExit(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el menú Exit
            void onExitFunction()
            {
                Application.Current.Shutdown();
            }
            deviceListLoadedCheck(onExitFunction);
        }
    }
}
