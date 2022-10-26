using ibcdatacsharp.UI.Device;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.GraphWindow
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
        public Model modelAccelerometer { get; private set; }
        public Model modelGyroscope { get; private set; }
        public Model modelMagnetometer { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelAccelerometer = new Model(accelerometer ,-80, 80, titleY : "Accelerometer", units : "m/s^2");
            modelGyroscope = new Model(gyroscope ,-600, 600, titleY: "Gyroscope", units: "g/s^2");
            modelMagnetometer = new Model(magnetometer ,-4, 4, titleY: "Magnetometer", units: "k(mT)");
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateAccelerometer(int frame, double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelAccelerometer.updateData(new double[] { x, y, z });
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
                modelGyroscope.updateData(new double[] { x, y, z });
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
                modelMagnetometer.updateData(new double[] { x, y, z });
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
            //await updateAccelerometer(frame, rawArgs.accelerometer[0], rawArgs.accelerometer[1], rawArgs.accelerometer[2]);
            //await updateMagnetometer(frame, rawArgs.magnetometer[0], rawArgs.magnetometer[1], rawArgs.magnetometer[2]);
            //await updateGyroscope(frame, rawArgs.gyroscope[0], rawArgs.gyroscope[1], rawArgs.gyroscope[2]);
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
        // Borra el contenido de los graficos
        public async void clearData()
        {
            //await clearAccelerometer();
            //await clearGyroscope();
            //await clearMagnetometer();
            await Task.WhenAll(new Task[] {
                clearAccelerometer(),
                clearGyroscope(),
                clearMagnetometer(),
            });
        }
        public async Task renderAcceletometer()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelAccelerometer.render();
            });
        }
        public async Task renderGyroscope()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelGyroscope.render();
            });
        }
        public async Task renderMagnetometer()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelMagnetometer.render();
            });
        }
        public async void onRender(object sender, EventArgs e)
        {
            await Task.WhenAll(new Task[]
            {
                renderAcceletometer(),
                renderGyroscope(),
                renderMagnetometer(),
            });
        }
    }
}
