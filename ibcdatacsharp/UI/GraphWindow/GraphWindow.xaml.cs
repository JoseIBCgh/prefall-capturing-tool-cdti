using ibcdatacsharp.UI.ToolBar.Enums;
using OxyPlot.Series;
using OxyPlot;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Page
    {
        DispatcherTimer timer; //Para los datos inventados
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
        // Borra el contenido de los graficos
        private void clearModels()
        {
            clearAccelerometer();
            clearGyroscope();
            clearMagnetometer();
        }
        // Funcion para actualizar la grafica del acelerometro
        public async void updateAccelerometer(double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelAccelerometer.update(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async void clearAccelerometer()
        {
            PlotModel model = modelAccelerometer.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelAccelerometer.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async void updateGyroscope(double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelGyroscope.update(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async void clearGyroscope()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelGyroscope.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async void updateMagnetometer(double x, double y, double z)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelMagnetometer.update(new double[] { x, y, z });
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async void clearMagnetometer()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelMagnetometer.clear();
            });
        }
        // Funcion que se llama al pulsar el boton capture. Activa la actualización de los graficos.
        public void play()
        {
            if (timer == null)
            {
                initTimer();
                timer.Start(); 
            }
        }
        // Funcion que se llama al pulsar el boton pause. Pausa los graficos si se estan actualizando
        // y los vuelve a actualizar si estan pausados.
        public void pause(ToolBar.ToolBar toolBar, MenuBar.MenuBar menuBar)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    toolBar.changePauseState(PauseState.Play); //Cambia la ToolBar a play. Se tiene que llamar.
                    menuBar.changePauseState(PauseState.Play); //Cambia el Menu a play. Se tiene que llamar.
                }
                else
                {
                    timer.Start();
                    toolBar.changePauseState(PauseState.Pause); //Cambia la ToolBar a pause. Se tiene que llamar.
                    menuBar.changePauseState(PauseState.Pause); //Cambia el Menu a pause. Se tiene que llamar.
                }
            }
        }
        // Funcion que se llama al pulsar el boton stop.
        public void stop(ToolBar.ToolBar toolBar, MenuBar.MenuBar menuBar)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                clearModels(); // Borra los datos de los graficos. El cambio se ve la proxima vez que se actualizen. Se tiene que llamar.
                toolBar.changePauseState(PauseState.Pause); // Cambia la ToolBar a pause. Se tiene que llamar.
                menuBar.changePauseState(PauseState.Pause); // Cambia el Menu a pause. Se tiene que llamar.
                timer = null;
            }
        }
        // Funcion para generar datos inventados
        private void streamData(object sender, EventArgs e)
        {
            Random random = new Random();
            double x = 50.0 + (random.NextDouble() - 0.5) * 20;
            double y = 0.0 + (random.NextDouble() - 0.5) * 20;
            double z = -50.0 + (random.NextDouble() - 0.5) * 20;
            updateAccelerometer(x, y, z); // Llamar a esta funcion para actualizar el acelerometro

            x = 300.0 + (random.NextDouble() - 0.5) * 100;
            y = 0.0 + (random.NextDouble() - 0.5) * 100;
            z = -300.0 + (random.NextDouble() - 0.5) * 100;
            updateGyroscope(x, y, z); // Llamar a esta funcion para actualizar el giroscopio

            x = 2.5 + (random.NextDouble() - 0.5);
            y = 0.0 + (random.NextDouble() - 0.5);
            z = -2.5 + (random.NextDouble() - 0.5);
            updateMagnetometer(x, y, z); // Llamar a esta funcion para actualizar el magnetometro
        }
        // Funcion para inicializar el timer para los datos inventados
        private void initTimer()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(streamData);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
        }
    }
}
