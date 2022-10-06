using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.ToolBar.Enums;
using OxyPlot;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Page
    {
        public GraphWindow()
        {
            InitializeComponent();
            initModels();
            DataContext = this;
        }
        public Model modelAccelerometer { get; private set; }
        public Model modelGyroscope { get; private set; }
        public Model modelMagnetometer { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelAccelerometer = new Model(-80, 80, titleY : "Accelerometer", units : "m/s^2");
            modelGyroscope = new Model(-600, 600, titleY: "Gyroscope", units: "g/s^2");
            modelMagnetometer = new Model(-4, 4, titleY: "Magnetometer", units: "k(mT)");
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateAccelerometer(double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelAccelerometer.update(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async Task clearAccelerometer()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelAccelerometer.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateGyroscope(double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelGyroscope.update(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async Task clearGyroscope()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelGyroscope.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateMagnetometer(double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelMagnetometer.update(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async Task clearMagnetometer()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelMagnetometer.clear();
            });
        }
        // Recive los datos del IMU inventado
        public async void onReceiveData(object sender, RawArgs args)
        {
            await updateAccelerometer(args.accelerometer[0], args.accelerometer[1], args.accelerometer[2]);
            await updateMagnetometer(args.magnetometer[0], args.magnetometer[1], args.magnetometer[2]);
            await updateGyroscope(args.gyroscope[0], args.gyroscope[1], args.gyroscope[2]);
        }
        // Borra el contenido de los graficos
        public async void onClearData(object sender)
        {
            await clearAccelerometer();
            await clearGyroscope();
            await clearMagnetometer();
        }
    }
}
