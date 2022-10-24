#define PARALLEL

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
    /// Version del grafico acc, mag, gyr con la libreria RealTimeGraphX
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
        // Inicializa los modelos
        private void initModels()
        {
            modelAccelerometer = new ViewModel(-80, 80);
            modelGyroscope = new ViewModel(-600, 600);
            modelMagnetometer = new ViewModel(-4, 4);
        }
        // Funcion para actualizar la grafica del acelerometro
#if PARALLEL
        public async Task updateAccelerometer(int frame, double x, double y, double z)
#else
        public async void updateAccelerometer(int frame, double x, double y, double z)
#endif
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelAccelerometer.update(frame, new double[] { x, y, z });
            });
        }
#if PARALLEL
        public async Task clearAccelerometer()
#else
        public async void clearAccelerometer()
#endif
        // Funcion para borrar los datos del acelerometro
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelAccelerometer.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
#if PARALLEL
        public async Task updateGyroscope(int frame, double x, double y, double z)
#else
        public async void updateGyroscope(int frame, double x, double y, double z)
#endif
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelGyroscope.update(frame, new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del giroscopio
#if PARALLEL
        public async Task clearGyroscope()
#else
        public async void clearGyroscope()
#endif
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelGyroscope.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
#if PARALLEL
        public async Task updateMagnetometer(int frame, double x, double y, double z)
#else
        public async void updateMagnetometer(int frame, double x, double y, double z)
#endif
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelMagnetometer.update(frame, new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del magnetometro
#if PARALLEL
        public async Task clearMagnetometer()
#else
        public async void clearMagnetometer()
#endif
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelMagnetometer.clear();
            });
        }

        // Recive los datos del IMU inventado
#if PARALLEL
        public async void onTick(object sender, ElapsedEventArgs e)
#else
        public void onTick(object sender, ElapsedEventArgs e)
#endif
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
#if PARALLEL
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]),
                updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]),
                updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2])
            });
#else
            updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]);
            updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]);
            updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2]);
#endif
        }
        // Recive los datos del IMU inventado media timer
#if PARALLEL
        public async void onTick(object sender, EventArgs e)
#else
        public void onTick(object sender, EventArgs e)
#endif
        {
            RawArgs rawArgs = device.rawData;
            int frame = device.frame;
#if PARALLEL
            await Task.WhenAll(new Task[] {
                updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]),
                updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]),
                updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2])
            });
#else
            updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]);
            updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]);
            updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2]);
#endif
        }
#if PARALLEL
        public async void clearData()
#else
        public void clearData()
#endif
        {
#if PARALLEL
            await Task.WhenAll(new Task[] {
                clearAccelerometer(),
                clearGyroscope(),
                clearMagnetometer(),
            });
#else
            clearAccelerometer();
            clearGyroscope();
            clearMagnetometer();
#endif
        }
    }
}
