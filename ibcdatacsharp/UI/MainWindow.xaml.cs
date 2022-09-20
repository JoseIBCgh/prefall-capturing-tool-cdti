using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ibcdatacsharp.DeviceList;
using ibcdatacsharp.DeviceList.TreeClasses;

namespace ibcdatacsharp
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
        }
        // Conecta los botones de la ToolBar
        private void initToolBarHandlers()
        {
            toolBar.Navigated += delegate (object sender, NavigationEventArgs e)
            {
                ToolBar toolBarClass = toolBar.Content as ToolBar;
                toolBarClass.scan.Click += new RoutedEventHandler(this.onScan);
            };
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
                IMUs[0].connected = true;
                IMUs[0].battery = 34;
                deviceListClass.showIMUs();
                deviceListClass.showCameras();
            }
            if (deviceList.Content == null)
            {
                deviceList.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    onScanFunction();
                };
            }
            else
            {
                onScanFunction();
            }
        }
    }
}
