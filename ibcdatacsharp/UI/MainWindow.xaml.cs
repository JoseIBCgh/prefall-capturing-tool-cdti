using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ibcdatacsharp.DeviceList.TreeClasses;
using System.Windows.Media.Imaging;

namespace ibcdatacsharp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Device.Device device;
        public MainWindow()
        {
            InitializeComponent();
            initIcon();
            initToolBarHandlers();
            initMenuHandlers();
            initDevice();
        }
        // Crea un IMU falso
        private void initDevice()
        {
            device = new Device.Device();
            graphWindow.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                GraphWindow.GraphWindow graphWindowClass = graphWindow.Content as GraphWindow.GraphWindow;
                device.rawData += graphWindowClass.onReceiveData;
            };
            angleGraph.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                AngleGraph.AngleGraph angleGraphClass = angleGraph.Content as AngleGraph.AngleGraph;
                device.angleData += angleGraphClass.onReceiveData;
            };
        }
        // Cambia el icono de la ventana
        private void initIcon()
        {
            Uri iconUri = new Uri("pack://application:,,,/UI/MenuBar/Icons/ibc-logo.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri);
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
                toolBarClass.record.Click += new RoutedEventHandler(onRecord);
                toolBarClass.capturedFiles.Click += new RoutedEventHandler(onCapturedFiles);
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
                menuBarClass.record.Click += new RoutedEventHandler(onRecord);
                menuBarClass.capturedFiles.Click += new RoutedEventHandler(onCapturedFiles);
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
                device.play();
            }
            deviceListLoadedCheck(onCaptureFunction);
        }
        // Conecta el boton Pause
        private void onPause(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Pause
            void onPauseFunction()
            {
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                device.pause(toolBarClass, menuBarClass);
            }
            deviceListLoadedCheck(onPauseFunction);
        }
        // Conecta el boton Stop
        private void onStop(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Stop
            void onStopFunction()
            {
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                GraphWindow.GraphWindow graphWindowClass = graphWindow.Content as GraphWindow.GraphWindow;
                AngleGraph.AngleGraph angleGraphClass = angleGraph.Content as AngleGraph.AngleGraph;
                device.stop(toolBarClass, menuBarClass, graphWindowClass, angleGraphClass);
            }
            deviceListLoadedCheck(onStopFunction);
        }
        // Conecta el boton Record
        private void onRecord(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Record
            void onRecordFunction()
            {
                ToolBar.ToolBar toolBarClass = toolBar.Content as ToolBar.ToolBar;
                MenuBar.MenuBar menuBarClass = menuBar.Content as MenuBar.MenuBar;
                device.record(toolBarClass, menuBarClass);
            }
            deviceListLoadedCheck(onRecordFunction);
        }
        // Conecta el boton Show Captured Files
        private void onCapturedFiles(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton Show Captured Files
            void onCapturedFilesFunction()
            {
                Trace.WriteLine("Show Captured Files");
            }
            deviceListLoadedCheck(onCapturedFilesFunction);
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
