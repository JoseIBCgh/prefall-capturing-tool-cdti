using ibcdatacsharp.UI.Device;
using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.RealTimeGraphX.AngleGraph
{
    /// <summary>
    /// Version de angle graph con la libreria RealTimeGraphX
    /// </summary>
    public partial class AngleGraph : Page
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        private Device.Device device;
        public AngleGraph()
        {
            InitializeComponent();
            initModels();
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        public ViewModel modelX { get; private set; }
        public ViewModel modelY { get; private set; }
        public ViewModel modelZ { get; private set; }
        // Funcion para incializar los graficos
        private void initModels()
        {
            modelX = new ViewModel();
            modelY = new ViewModel();
            modelZ = new ViewModel();
        }
        // Funcion para actualizar la grafica del acelerometro
        public async Task updateX(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.update(frame, data);
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async Task clearX()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelX.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateY(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.update(frame, data);
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async Task clearY()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelY.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateZ(int frame, double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.update(frame, data);
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async Task clearZ()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelZ.clear();
            });
        }
        // Recive los datos del IMU inventado
        public async void onTick(object sender, ElapsedEventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
            //await updateX(frame, angleArgs.angle[0]);
            //await updateY(frame, angleArgs.angle[1]);
            //await updateZ(frame, angleArgs.angle[2]);
            await Task.WhenAll(new Task[]
            {
                updateX(frame, angleArgs.angle[0]),
                updateY(frame, angleArgs.angle[1]),
                updateZ(frame, angleArgs.angle[2])
            });
        }
        // Recive los datos del IMU inventado media timer
        public async void onTick(object sender, EventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
            await Task.WhenAll(new Task[]
            {
                updateX(frame, angleArgs.angle[0]),
                updateY(frame, angleArgs.angle[1]),
                updateZ(frame, angleArgs.angle[2])
            });
        }
        // Borra el contenido de los graficos
        public async void clearData()
        {
            //await clearX();
            //await clearY();
            //await clearZ();
            await Task.WhenAll(new Task[] {
                clearX(),
                clearY(),
                clearZ(),
            });
        }
    }
}
