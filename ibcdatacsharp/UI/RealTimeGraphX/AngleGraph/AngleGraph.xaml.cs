#define PARALLEL

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
#if PARALLEL
        public async Task updateX(int frame, double data) 
#else
        public async void updateX(int frame, double data)
#endif
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.update(frame, data);
            });
        }
        // Funcion para borrar los datos del acelerometro
#if PARALLEL
        public async Task clearX()
#else
        public async void clearX()
#endif
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelX.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
#if PARALLEL
        public async Task updateY(int frame, double data)
#else
        public async void updateY(int frame, double data)
#endif
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.update(frame, data);
            });
        }
        // Funcion para borrar los datos del giroscopio
#if PARALLEL
        public async Task clearY()
#else
        public async void clearY()
#endif
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelY.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
#if PARALLEL
        public async Task updateZ(int frame, double data)
#else
        public async void updateZ(int frame, double data)
#endif
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.update(frame, data);
            });
        }
        // Funcion para borrar los datos del magnetometro
#if PARALLEL
        public async Task clearZ()
#else
        public async void clearZ()
#endif
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelZ.clear();
            });
        }
        // Recive los datos del IMU inventado
#if PARALLEL
        public async void onTick(object sender, ElapsedEventArgs e)
#else
        public void onTick(object sender, ElapsedEventArgs e)
#endif
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
#if PARALLEL
            await Task.WhenAll(new Task[]
            {
                updateX(frame, angleArgs.angle[0]),
                updateY(frame, angleArgs.angle[1]),
                updateZ(frame, angleArgs.angle[2])
            });
#else
            updateX(frame, angleArgs.angle[0]);
            updateY(frame, angleArgs.angle[1]);
            updateZ(frame, angleArgs.angle[2]);
#endif
        }
        // Recive los datos del IMU inventado media timer
#if PARALLEL
        public async void onTick(object sender, EventArgs e)
#else
        public void onTick(object sender, EventArgs e)
#endif
        {
            AngleArgs angleArgs = device.angleData;
            int frame = device.frame;
#if PARALLEL
            await Task.WhenAll(new Task[]
            {
                updateX(frame, angleArgs.angle[0]),
                updateY(frame, angleArgs.angle[1]),
                updateZ(frame, angleArgs.angle[2])
            });
#else
            updateX(frame, angleArgs.angle[0]);
            updateY(frame, angleArgs.angle[1]);
            updateZ(frame, angleArgs.angle[2]);
#endif
        }
        // Borra el contenido de los graficos
#if PARALLEL
        public async void clearData()
#else
        public void clearData()
#endif
        {
#if PARALLEL
            await Task.WhenAll(new Task[] {
                clearX(),
                clearY(),
                clearZ(),
            });
#else
            clearX();
            clearY();
            clearZ();
#endif
        }
    }
}
