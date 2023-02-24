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
using System.Text.RegularExpressions;
using System.Management;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using System.Drawing;
using MessageBox = System.Windows.MessageBox;
using System.Numerics;
using static WisewalkSDK.Wisewalk;
using ibcdatacsharp.EKF;
using Microsoft.VisualBasic.ApplicationServices;
using ibcdatacsharp.UI.Common;
using ibcdatacsharp.UI.Filters;
using ibcdatacsharp.UI.Graphs;

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

        private const string vidPattern = @"VID_([0-9A-F]{4})";
        private const string pidPattern = @"PID_([0-9A-F]{4})";

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
        public Dictionary<string, WisewalkSDK.Device> devices_list
        {
            get
            {
                return api.GetDevicesConnected();
            }
        }

        private byte counterUI = 0;

        private readonly ushort[] SampleRate = { 25, 50, 100, 200 };

        private string version = "";

        public List<int> counter;

        private List<Wisewalk.Dev> scanDevices = new List<Wisewalk.Dev>();

        private List<Wisewalk.Dev> conn_list_dev;

        private bool devConnected = false;

        private int indexDev = -1;

        private int indexSelected = -1;
        private short handlerSelected = -1;
        float[] acc = new float[3];

        List<ComPort> ports2;
        string error = "";

        string port_selected = "";

        struct ComPort // custom struct with our desired values
        {
            public string name;
            public string vid;
            public string pid;
            public string description;
        }

        public IMUInfo imuInfo;
        public List<int> devHandlers;
        public FilterManager filterManager;

        public SagitalAngles.SagitalAngles sagitalAngles;

        //end Wiseware API
        public MainWindow()
        {
            InitializeComponent();
            virtualToolBar = new VirtualToolBar();
            device = new Device.Device();
            fileSaver = new FileSaver.FileSaver();
            graphManager = new GraphManager();
            filterManager = new FilterManager();
            sagitalAngles = new SagitalAngles.SagitalAngles();
            initIcon();
            initToolBarHandlers();
            initMenuHandlers();
            initialized?.Invoke(this, EventArgs.Empty);

            //Begin Wisewalk API
            ports = new List<Wisewalk.ComPort>();
            devHandlers = new List<int>();
            //devices_list = new Dictionary<string, WisewalkSDK.Device>();

            counter = new List<int>();
            api = new Wisewalk();
            conn_list_dev = new List<Wisewalk.Dev>();
            imuInfo = new IMUInfo();

            version = api.GetApiVersion();
            api.scanFinished += Api_scanFinished;
            api.deviceConnected += Api_deviceConnected;
            api.onError += Api_onError;
            api.deviceDisconnected += Api_onDisconnect;
            api.updateDeviceRTC += Api_updateDeviceRTC;
            api.updateDeviceConfiguration += Api_updateDeviceConfiguration;
            api.updateDeviceInfo += Api_updateDeviceInfo;

            //End Wisewalk API
            //EKF.EKF.test();
            //Test linear acceleration
            //LinearAcceleration.test();
        }

        /** 
         * Métodos de Wiseware
         */
        private void Api_onDisconnect(byte deviceHandler)
        {
            Trace.WriteLine("Api_onDisconnect");
            Dispatcher.BeginInvoke(
                    () => (deviceList.Content as DeviceList.DeviceList).
                    disconnectIMU(deviceHandler)
                );
            //devices_list.Remove(deviceHandler.ToString());
        }
        private void Api_onError(byte deviceHandler, string error)
        {
            if (deviceHandler != 0xFF)
            {
                
                SetLogText(devices_list[deviceHandler.ToString()].Id, error);
            }
            else
            {
                SetLogText("", error);
            }
        }
        private void SetLogText(string device, string text)
        {


            var output = "";
            var message = "";

            if (device != "")
            {
                message = $"{DateTime.Now:HH:mm:ss.fff}   RX from [{device}]:   {text} ";
            }
            else
            {
                message = $"{DateTime.Now:HH:mm:ss.fff}:   {text} ";
            }

            output += message + Environment.NewLine;

            Trace.Write(output);


        }
        //Callback de Escaneo

        private List<ComPort> GetSerialPorts()
        {
            using (var searcher = new ManagementObjectSearcher
                ("SELECT * FROM WIN32_SerialPort"))
            {
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
                return ports.Select(p =>
                {
                    ComPort c = new ComPort();
                    c.name = p.GetPropertyValue("DeviceID").ToString();
                    c.vid = p.GetPropertyValue("PNPDeviceID").ToString();
                    c.description = p.GetPropertyValue("Caption").ToString();

                    Match mVID = Regex.Match(c.vid, vidPattern, RegexOptions.IgnoreCase);
                    Match mPID = Regex.Match(c.vid, pidPattern, RegexOptions.IgnoreCase);

                    if (mVID.Success)
                        c.vid = mVID.Groups[1].Value;
                    if (mPID.Success)
                        c.pid = mPID.Groups[1].Value;

                    return c;

                }).ToList();
            }
        }

        // Selección de puerto sin tener que ponerlo manualmente
        // En pruebas.
        private void ShowPorts()
        {


            ports = api.GetUsbDongles();
            foreach (Wisewalk.ComPort port in ports)
            {
                Match match1 = Regex.Match(port.description, "nRF52 USB CDC BLE*", RegexOptions.IgnoreCase);
                if (match1.Success)
                {
                    port_selected = port.name;
                    Trace.WriteLine(port.description);

                }
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
        private string GetMacAddress(Wisewalk.Dev device)
        {
            string mac = "";

            mac = device.mac[5].ToString("X2") + ":" + device.mac[4].ToString("X2") + ":" + device.mac[3].ToString("X2") + ":" +
                                    device.mac[2].ToString("X2") + ":" + device.mac[1].ToString("X2") + ":" + device.mac[0].ToString("X2");

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

        private async void Api_deviceConnected(byte handler, WisewalkSDK.Device dev)
        {
            
            //if (!devices_list.ContainsKey(handler.ToString())) { 
            // Add new device to list
            //WisewalkSDK.Device device = new WisewalkSDK.Device();

            //Trace.WriteLine("DevList: " + dev.Id + " handler: " + handler.ToString());

            //devices_list.Add(handler.ToString(), device);

            // Update values
            /*
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
            */

            await Dispatcher.BeginInvoke(
                () => (deviceList.Content as DeviceList.DeviceList).
                connectIMU(dev.Id, handler)
            );

            ushort sampleRate = 100;
            byte packetType = (byte)3;

            api.SetDeviceConfiguration(handler, sampleRate, packetType, out error);

            api.SetRTCDevice(handler, GetDateTime(), out error);

            counter.Add(0);

            Trace.WriteLine("DevList: " + devices_list[handler.ToString()].Id + " handler: " + handler.ToString());

            /*
            }
            else
            {
                // Update info device
                devices_list[handler.ToString()].HeaderInfo = dev.HeaderInfo;
            }

            //ShowDevices(devices_list);
            */
        }

        private void Api_updateDeviceInfo(byte deviceHandler, WisewalkSDK.Device dev)
        {
            //devices_list[deviceHandler.ToString()].HeaderInfo = dev.HeaderInfo;
            //devices_list[deviceHandler.ToString()].sampleRate = dev.sampleRate;
            //devices_list[deviceHandler.ToString()].offsetTime = dev.offsetTime;
            Dispatcher.BeginInvoke(
                () => (deviceList.Content as DeviceList.DeviceList).
                updateHeaderInfo(dev.Id, deviceHandler)
            );

            SetLogText(devices_list[deviceHandler.ToString()].Id, "Receive header info from " + dev.HeaderInfo.macAddress);

            //ShowDevices(devices_list);
        }

        void Api_updateDeviceConfiguration(byte deviceHandler, byte sampleRate, byte packetType)
        {
            Trace.WriteLine($"Configured {deviceHandler} {sampleRate} {packetType}");
        }

        void Api_updateDeviceRTC(byte deviceHandler, DateTime dateTime)
        {
            Trace.WriteLine(dateTime.ToString());
        }


        //Cálculo de Fecha y hora
        public DateTime GetDateTime()
        {
            return DateTime.Now;
            DateTime dateTime = new DateTime(2023, 2, 17, 10, 10, 10, 100);
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
                #region SAGITAL ANGLES
                toolBarClass.calculateMounting.Click += new RoutedEventHandler(onCalculateMounting);
                toolBarClass.saveFrontalReference.Click += new RoutedEventHandler(onSaveFrontalReference);
                #endregion SAGITAL ANGLES
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
                api.Open(port_selected, out error);

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

                    List<CameraInfo> cameras = new List<CameraInfo>();
                    for (int i = 0; i < names.Count; i++)
                    {
                        if (indices.Contains(i))
                        {
                            cameras.Add(new CameraInfo(i, names[i]));
                        }
                    }
                    deviceListClass.setCameras(cameras);

                    await Task.Delay(4000);

                    List<IMUInfo> imus = new List<IMUInfo>();
                    for (int i = 0; i < scanDevices.Count; i++)
                    {
                        imus.Add(new IMUInfo("ActiSense", GetMacAddress(scanDevices, i)));
                    }
                    /* 
                    //IMUS falsos
                    Random random = new Random();
                    imus.Add(new IMUInfo("ActiSense", random.NextSingle().ToString()));
                    imus.Add(new IMUInfo("ActiSense2", random.NextSingle().ToString()));
                    */

                    deviceListClass.setIMUs(imus);
                    MessageBox.Show(scanDevices.Count + " IMUs encontrados", "Scan Devices", MessageBoxButton.OK, MessageBoxImage.Information);
                }


                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                //deviceListClass.clearAll();
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
        private Dev findIMU(IMUInfo imuInfo)
        {
            return scanDevices.FirstOrDefault(de => GetMacAddress(de) == imuInfo.address);
        }
        // Conecta el boton connect
        private void onConnect(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton connect
            async void onConnectFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                IList<object> selectedItems = (IList<object>)deviceListClass.treeView.SelectedItems;
                List<IMUInfo> connectedIMUs = new List<IMUInfo>();
                List<object> selectedIMUs = new List<object>(); // Necesario porque deviceListClass.treeView.SelectedItems puede cambiar despues de clicar connect
                foreach (object selected in selectedItems)
                {
                    if (selected != null) // No se si se puede quitar
                    {
                        if (selected is IMUInfo)
                        {
                            selectedIMUs.Add(selected);
                            MultiSelectTreeViewItem treeViewItem = (MultiSelectTreeViewItem)deviceListClass.IMUs.ItemContainerGenerator.ContainerFromItem(selected);

                            //´Wise connecting
                            imuInfo = treeViewItem.DataContext as IMUInfo;
                            connectedIMUs.Add(imuInfo);
                        }
                        else if (selected is CameraInfo)
                        {
                            //MultiSelectTreeViewItem treeViewItem = (MultiSelectTreeViewItem)deviceListClass.cameras.ItemContainerGenerator.ContainerFromItem(selected);
                            //deviceListClass.connectCamera(treeViewItem);
                        }
                    }
                }

                //Trace.WriteLine("::OnConnect::: Imu seleccionado: " + imuInfo.id.ToString());

                conn_list_dev = new List<Dev>();
                // Operación atómica de conexión
                foreach (IMUInfo imu in connectedIMUs)
                {
                    conn_list_dev.Add(findIMU(imu));
                    devHandlers.Remove((int)imu.id);
                }
             
                
                if (!api.Connect(conn_list_dev, out error))
                {
                    Trace.WriteLine("Connect error " + error);
                }
                //api.SetDeviceConfiguration((byte)imuInfo.id, 100, 3, out error);
                /*
                await Task.Delay(4000);
                foreach (IMUInfo imu in connectedIMUs)
                {
                    api.SetDeviceConfiguration((byte)imu.id, 100, 3, out error);
                }
                await Task.Delay(1000);
                foreach (IMUInfo imu in connectedIMUs)
                {
                    api.SetRTCDevice((byte)imu.id, GetDateTime(), out error);
                }
                await Task.Delay(1000);

                deviceListClass.connectIMUs(selectedIMUs);
                */
                //api.SetRTCDevice((byte)imuInfo.id, GetDateTime(), out error);
                //await Task.Delay(1000);

                // Fin Operación atómica de conexión

                //EndWise

                //Borrar si existe
                foreach (IMUInfo imu in connectedIMUs)
                {
                    if (devHandlers.Contains((int)imuInfo.id))
                    {
                        devHandlers.Remove((int)imuInfo.id);

                    }
                }
            }
            deviceListLoadedCheck(onConnectFunction);
        }
        private byte handler(IMUInfo imu)
        {
            string handler = devices_list.Where(d => d.Value.Id == imu.address).FirstOrDefault().Key;
            return byte.Parse(handler);
        }
        public void startActiveDevices()
        {
            Trace.WriteLine("startActiveDevices");
            Helpers.printDevicesConnected();
            List<IMUInfo> activeIMUs = (deviceList.Content as DeviceList.DeviceList).IMUsUsed;
            Trace.WriteLine("num active IMUs " + activeIMUs.Count.ToString());

            foreach(IMUInfo imu in activeIMUs)
            {
                Trace.WriteLine(imu.address);
                Trace.WriteLine("handler: " + handler(imu).ToString());
                if(!api.StartStream(handler(imu), out error))
                {
                    Trace.WriteLine("error: " + error);
                }
            }
        }
        // Conecta el boton disconnect
        private void onDisconnect(object sender, EventArgs e)
        {
            // Funcion que se ejecuta al clicar el boton disconnect
            async void onDisconnectFunction()
            {
                DeviceList.DeviceList deviceListClass = deviceList.Content as DeviceList.DeviceList;
                IList<object> selectedItems = (IList<object>)deviceListClass.treeView.SelectedItems;
                List<string> IMUsToDisconnect = new List<string>();
                devHandlers = new List<int>();
                Trace.WriteLine("before disconnect");
                Helpers.printDevicesConnected();
                foreach (object selected in selectedItems)
                {
                    if (selected != null && selected is IMUInfo)
                    {
                        MultiSelectTreeViewItem treeViewItem = (MultiSelectTreeViewItem)deviceListClass.IMUs.ItemContainerGenerator.ContainerFromItem(selected);
                        //Begin Wise
                        IMUInfo imuInfo = treeViewItem.DataContext as IMUInfo;

                        devHandlers.Add(handler(imuInfo));
                        conn_list_dev.Remove(findIMU(imuInfo));
                        //devices_list.Remove(imuInfo.handler.ToString());
                        imuInfo.handler = null;
                        IMUsToDisconnect.Add(imuInfo.address);

                        
                    }
                }
                if (!api.Disconnect(devHandlers, out error))
                {
                    Trace.WriteLine("Disconnect error " + error);
                }
                
                await Task.Delay(4000);
                deviceListClass.disconnectIMUs(IMUsToDisconnect);
                //Dictionary<string, WisewalkSDK.Device> devicesConnected =  api.GetDevicesConnected();
                /*Trace.WriteLine("devices connected");
                foreach (KeyValuePair<string, WisewalkSDK.Device> device in devicesConnected)
                {
                    Trace.WriteLine(device.Key);
                    Trace.WriteLine(device.Value.Id);
                }
                */
                Trace.WriteLine("after disconnect");
                //Helpers.printDevicesConnected();
                
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
                IList<object> selectedItems = (IList<object>)deviceListClass.treeView.SelectedItems;
                foreach(object selected in selectedItems)
                {
                    if (selected != null && selected is CameraInfo)
                    {
                        MultiSelectTreeViewItem treeViewItem = (MultiSelectTreeViewItem)deviceListClass.cameras.ItemContainerGenerator.ContainerFromItem(selected);
                        CameraInfo cameraInfo = treeViewItem.DataContext as CameraInfo;
                        int id = cameraInfo.number; //Id de la camara
                        CamaraViewport.CamaraViewport camaraViewportClass = camaraViewport.Content as CamaraViewport.CamaraViewport;
                        if (!camaraViewportClass.someCameraOpened())
                        {
                            camaraViewportClass.Title = cameraInfo.name + " CAM " + id;
                            camaraViewportClass.initializeCamara(id);
                        }
                        break;
                    }
                }
            }
            deviceListLoadedCheck(onOpenCameraFunction);
        }
        // Funcion que se ejecuta al clicar el boton Capture
        private void onCapture(object sender, EventArgs e)
        {
            virtualToolBar.captureClick();
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
        #region SAGITAL ANGLES
        private void onCalculateMounting(object sender, EventArgs e)
        {
            sagitalAngles.calculateMounting();
        }
        private void onSaveFrontalReference(object sender, EventArgs e)
        {
            sagitalAngles.calculateVirtualOrientation();
        }
        #endregion
        // IMPORTANTE: La funcion eventHandler tiene que ser local
        public void readQuaternion(IMUInfo imu)
        {
            // usar imu para acceder a la informacion del imu
            var tokenSource = new CancellationTokenSource();
            CancellationToken ct = tokenSource.Token;
            void eventHandler(object sender, byte handler, Quaternion q)
            {
                if(imu.handler != null && handler == imu.handler)
                {
                    graphManager.captureManager.quaternionEvent -= eventHandler;
                    tokenSource.Cancel();
                    graphManager.captureManager.setReference(q);
                    string text = "Referencia creada con el valor del quaternion: " +
                        "x = " + q.X.ToString("0.##") +
                        ",y = " + q.Y.ToString("0.##") +
                        ",z = " + q.Z.ToString("0.##") +
                        ",w = " + q.W.ToString("0.##");
                    Task.Run(() => MessageBox.Show(text, "Set as Reference"));
                }
            }
            graphManager.captureManager.quaternionEvent += eventHandler;
            Task.Delay(1000).ContinueWith(t =>
            {
                if (!ct.IsCancellationRequested)
                {
                    string text = "No se han recibido datos del sensor " + imu.id.ToString();
                    graphManager.captureManager.quaternionEvent -= eventHandler;
                    MessageBox.Show(text, "Set as Reference");
                }
            });
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
