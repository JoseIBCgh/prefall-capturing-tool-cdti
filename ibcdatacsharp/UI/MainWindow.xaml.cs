using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using ibcdatacsharp.DeviceList.TreeClasses;
using System.Windows.Media.Imaging;
using DirectShowLib;
using System.Collections.Generic;
using OpenCvSharp;
using System.Threading.Tasks;
using ibcdatacsharp.UI.ToolBar;

//Actisense
using WisewalkSDK;
using static WisewalkSDK.Protocol_v3;
using System.Windows.Forms;
using Application = System.Windows.Application;
using System.Threading;
using System.IO.Ports;

namespace ibcdatacsharp.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {

        public Device.Device device;
        public VirtualToolBar virtualToolBar;
        public GraphManager graphManager;

        public FileSaver.FileSaver fileSaver;

        public event EventHandler initialized;

        //Wiseware API

        private const string pathDir = @"c:\Wiseware\Wisewalk-API\";

        private const int ColumnBatteryIndex = 2;
        private const int ColumnNPacketsIndex = 5;
        private const int ColumnTimespanIndex = 6;
        private const int ColumnAccIndex = 7;

        public Wisewalk api;
        private string portName = "";
        private int baudRate = 921600;
        private List<Wisewalk.ComPort> ports;
        private bool isConnected = false;
        private bool startStream = false;
        private bool startRecord = false;
        public Dictionary<string, WisewalkSDK.Device> devices_list;

        private byte counterUI = 0;

        private readonly ushort[] SampleRate = { 25, 50, 100, 200 };

        private string version = "";

        public List<int> counter;

        private List<Wisewalk.Dev> scanDevices = null;

        private List<Wisewalk.Dev> scanAux;

        private bool devConnected = false;

        private int indexDev = -1;

        private int indexSelected = -1;
        private short handlerSelected = -1;
        float[] acc = new float[3];


        string error = "";

        //end Wiseware API
        public MainWindow()
        {
            InitializeComponent();
            virtualToolBar = new VirtualToolBar();
            device = new Device.Device();
            fileSaver = new FileSaver.FileSaver();
            graphManager = new GraphManager();
            initIcon();
            initToolBarHandlers();
            initMenuHandlers();
            initialized?.Invoke(this, EventArgs.Empty);

            //Begin Wisewalk API
            ports = new List<Wisewalk.ComPort>();

            devices_list = new Dictionary<string, WisewalkSDK.Device>();

            counter = new List<int>();
            api = new Wisewalk();
            scanAux = new List<Wisewalk.Dev>();

            version = api.GetApiVersion();
            api.scanFinished += Api_scanFinished;
            api.deviceConnected += Api_deviceConnected;

            //End Wisewalk API
        }

        /** 
         * Métodos de Wiseware
         */

        //Callback de Escaneo

        private void ShowPorts()
        {

            string[] ports = SerialPort.GetPortNames();

            Trace.WriteLine("The following serial ports were found:");

            // Display each port name to the console.
            foreach (string port in ports)
            {
                Trace.WriteLine(port);
            }
        }


        private void Api_scanFinished(List<Wisewalk.Dev> devices)
        {
            scanDevices = devices;
            Trace.WriteLine("# of devices: " + devices.Count);
            ShowScanList(scanDevices);
           
        }

        private string GetMacAddress(List<Wisewalk.Dev> devices, int idx)
        {
            string mac = "";

            mac = devices[idx].mac[5].ToString("X2") + ":" + devices[idx].mac[4].ToString("X2") + ":" + devices[idx].mac[3].ToString("X2") + ":" +
                                    devices[idx].mac[2].ToString("X2") + ":" + devices[idx].mac[1].ToString("X2") + ":" + devices[idx].mac[0].ToString("X2");

            return mac;
        }

        private void ShowScanList(List<Wisewalk.Dev> devices)
        {

            for (int idx = 0; idx < devices.Count; idx++)
            {
                string macAddress = devices[idx].mac[5].ToString("X2") + ":" + devices[idx].mac[4].ToString("X2") + ":" + devices[idx].mac[3].ToString("X2") + ":" +
                                    devices[idx].mac[2].ToString("X2") + ":" + devices[idx].mac[1].ToString("X2") + ":" + devices[idx].mac[0].ToString("X2");


                Trace.WriteLine("MacAddress: ", " * " + macAddress);
            }

        }

        private void Api_deviceConnected(byte handler, WisewalkSDK.Device dev)
        {
            if (!devices_list.ContainsKey(handler.ToString()))
            {
                // Add new device to list
                WisewalkSDK.Device device = new WisewalkSDK.Device();

                devices_list.Add(handler.ToString(), device);

                // Update values
                devices_list[handler.ToString()].Id = dev.Id;
                devices_list[handler.ToString()].Name = dev.Name;
                devices_list[handler.ToString()].Connected = dev.Connected;
                devices_list[handler.ToString()].HeaderInfo = dev.HeaderInfo;
                devices_list[handler.ToString()].NPackets = 0;
                devices_list[handler.ToString()].sampleRate = dev.sampleRate;
                devices_list[handler.ToString()].offsetTime = dev.offsetTime;
                devices_list[handler.ToString()].Rtc = dev.Rtc;
                devices_list[handler.ToString()].Stream = false;
                devices_list[handler.ToString()].Record = false;

                counter.Add(0);

                Trace.WriteLine("DevList: " + devices_list[handler.ToString()].Id);


            }
            else
            {
                // Update info device
                devices_list[handler.ToString()].HeaderInfo = dev.HeaderInfo;
            }

            //ShowDevices(devices_list);

        }

        

        //Cálculo de Fecha y hora
        public DateTime GetDateTime()
        {
            DateTime dateTime = new DateTime(2022, 11, 8, 13, 0, 0, 0);
            return dateTime;
        }

        //End métodos de WiseWare

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
            //Escaneo en el puerto com6

            void getIMUs()
            {
                
                ShowPorts();
                api.Open("COM6", out error);

                if (!api.ScanDevices(out error))
                {
                    // Error
                    Trace.WriteLine("", "Error to scan devices - " + error);
                }
                else
                {
                    Thread.Sleep(2000);
                }

            }

            // Funcion que se ejecuta al clicar el boton scan
            void onScanFunction()
            {
                // Añade las camaras al TreeView
                async void addCameras(DeviceList.DeviceList deviceListClass)
                {
                    // Devuelve el nombre de todas las camaras conectadas
                    List<string> cameraNames()
                    {
                        List<DsDevice> devices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
                        List<string> cameraNames = new List<string>();
                        foreach (DsDevice device in devices)
                        {
                            cameraNames.Add(device.Name);
                        }
                        return cameraNames;
                    }
                    // Devuelve una lista de indice OpenCV de las camaras disponibles
                     List<int> cameraIndices(int maxIndex = 10)
                    {
                        List<int> indices = new List<int>();
                        VideoCapture capture = new VideoCapture();
                        for(int index = 0; index < maxIndex; index++)
                        {
                            capture.Open(index, VideoCaptureAPIs.DSHOW);
                            if (capture.IsOpened())
                            {
                                indices.Add(index);
                                capture.Release();
                            }
                        }
                        return indices;
                    }
                    List<string> names = await Task.Run(() => cameraNames());
                    //names.ForEach(n => Trace.WriteLine(n));
                    List<int> indices = await Task.Run(() => cameraIndices(names.Count));
                    //indices.ForEach(i => Trace.WriteLine(i));
                    await Task.Run(() => getIMUs()); //necesario para escanear IMUs


                    for (int i = 0; i < names.Count; i++)
                    {
                        if (indices.Contains(i))
                        {
                            deviceListClass.addCamera(new CameraInfo(i, names[i]));
                        }
                    }

                    Thread.Sleep(4000);
                    for (int i = 0; i < scanDevices.Count; i++)
                    {
                        deviceListClass.addIMU(new IMUInfo(i, "ActiSense", GetMacAddress(scanDevices, i)));
                    }
                }


                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                deviceListClass.clearAll();
                addCameras(deviceListClass);
                deviceListClass.showCameras();
                // Añade datos inventados quitar
                deviceListClass.showIMUs();
                deviceListClass.showInsoles();
                
                deviceListClass.addInsole(new InsolesInfo("Insole", "Left"));
                
            }
            deviceListLoadedCheck(onScanFunction);
            virtualToolBar.onScanClick();
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

                        //´Wise connecting
                        IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;

                        for (var i = 0; i < scanDevices.Count; i++)
                        {
                            if (i == imuInfo.id)
                            {
                                scanAux.Add(scanDevices[i]);
                            }
                        }
                        
                        // Operación atómica de conexión

                        api.Connect(scanAux, out error);
                        //api.SetDeviceConfiguration((byte)imuInfo.id, 100, 3, out error);
                        Thread.Sleep(1000);
                        api.SetDevicesConfigurations(100, 3, out error);
                        Thread.Sleep(1000);
                        api.SetRTCDevices(GetDateTime(), out error);
                        //api.SetRTCDevice((byte)imuInfo.id, GetDateTime(), out error);
                        Thread.Sleep(1000);

                        // Fin Operación atómica de conexión

                        //EndWise

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
                    //Begin Wise
                    IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;

                    for (int i = 0; i < scanAux.Count; i++)
                    {
                        if (i == imuInfo.id)
                        {
                            List<int> devHandlers = new List<int>();
                            devHandlers.Add(i);
                            api.Disconnect(devHandlers, out error);
                        }
                    }
                    //End Wise

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
                    CamaraViewport.CamaraViewport camaraViewportClass = camaraViewport.Content as CamaraViewport.CamaraViewport;
                    if (!camaraViewportClass.someCameraOpened())
                    {
                        camaraViewportClass.Title = cameraInfo.name + " CAM " + id;
                        camaraViewportClass.initializeCamara(id);
                    }
                }
            }
            deviceListLoadedCheck(onOpenCameraFunction);
        }
        // Funcion que se ejecuta al clicar el boton Capture
        private void onCapture(object sender, EventArgs e)
        {
            graphManager.initCapture(); 
        }
        // Funcion que se ejecuta al clicar el boton Pause
        private void onPause(object sender, EventArgs e)
        {
            virtualToolBar.pauseClick();
        }
        // Funcion que se ejecuta al clicar el boton Stop
        private void onStop(object sender, EventArgs e)
        {
            virtualToolBar.stopClick();
        }
        // Funcion que se ejecuta al clicar el boton Record
        private void onRecord(object sender, EventArgs e)
        {
            
            virtualToolBar.recordClick();
        }
        // Funcion que se ejecuta al clicar el boton Show Captured Files
        private void onCapturedFiles(object sender, EventArgs e)
        {
            virtualToolBar.openClick();
        }
        // Funcion que se ejecuta al clicar el menú Exit
        private void onExit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        // Funcion que se ejecuta al cerrar la ventana
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            CamaraViewport.CamaraViewport camaraViewportClass = camaraViewport.Content as CamaraViewport.CamaraViewport;
            camaraViewportClass.onCloseApplication();
            fileSaver.onCloseApplication();
            base.OnClosing(e);
        }
    }
}
