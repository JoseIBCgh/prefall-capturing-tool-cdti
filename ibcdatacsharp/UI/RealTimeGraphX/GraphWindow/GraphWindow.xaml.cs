using ibcdatacsharp.UI.Device;
using System.Threading.Tasks;
using System.Timers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.RealTimeGraphX.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Page
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        private Device.Device device;
        public GraphWindow()
        {
            InitializeComponent();
            initModels();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        public ViewModel modelAccelerometer { get; private set; }
        public ViewModel modelGyroscope { get; private set; }
        public ViewModel modelMagnetometer { get; private set; }

        private void initModels()
        {
            modelAccelerometer = new ViewModel(-80, 80);
            modelGyroscope = new ViewModel(-600, 600);
            modelMagnetometer = new ViewModel(-4, 4);
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateAccelerometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelAccelerometer.update(frame, new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async Task clearAccelerometer()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelAccelerometer.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateGyroscope(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelGyroscope.update(frame, new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async Task clearGyroscope()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelGyroscope.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateMagnetometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelMagnetometer.update(frame, new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async Task clearMagnetometer()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelMagnetometer.clear();
            });
        }

        // Recive los datos del IMU inventado
        public async void onTick(object sender, ElapsedEventArgs e)
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]),
                updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]),
                updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2])
            });
        }
        // Recive los datos del IMU inventado media timer
        public async void onTick(object sender, EventArgs e)
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]),
                updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]),
                updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2])
            });
        }
        public async void clearData()
        {
            await Task.WhenAll(new Task[] {
                clearAccelerometer(),
                clearGyroscope(),
                clearMagnetometer(),
            });
        }
    }
}
