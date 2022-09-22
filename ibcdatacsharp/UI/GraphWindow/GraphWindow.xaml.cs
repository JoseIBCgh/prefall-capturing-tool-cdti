using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Diagnostics;
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
        public Model model_accelerometer { get; private set; }
        public Model model_gyroscope { get; private set; }
        public Model model_magnetometer { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            model_accelerometer = new Model(-80, 80, titleY : "Accelerometer", units : "m/s^2");
            model_gyroscope = new Model(-600, 600, titleY: "Gyroscope", units: "g/s^2");
            model_magnetometer = new Model(-4, 4, titleY: "Magnetometer", units: "k(mT)");
        }
        // Hay que llamar a esta funcion para que los graficos se actualicen
        private void startModels()
        {
            model_accelerometer.Start();
            model_gyroscope.Start();
            model_magnetometer.Start();
        }
        // Hace que los graficos se dejen de actualizar
        private void pauseModels()
        {
            model_accelerometer.Pause();
            model_gyroscope.Pause();
            model_magnetometer.Pause();
        }
        // Borra el contenido de los graficos
        private void clearModels()
        {
            model_accelerometer.ClearData();
            model_gyroscope.ClearData();
            model_magnetometer.ClearData();
        }
        // Funcion para actualizar la grafica del acelerometro
        public void updateAccelerometer(double x, double y, double z)
        {
            model_accelerometer.Queue(new[] { x, y, z });
        }
        // Funcion para actualizar la grafica del giroscopio
        public void updateGyroscope(double x, double y, double z)
        {
            model_gyroscope.Queue(new[] { x, y, z });
        }
        // Funcion para actualizar la grafica del magnetometro
        public void updateMagnetometer(double x, double y, double z)
        {
            model_magnetometer.Queue(new[] { x, y, z });
        }
        // Funcion que se llama al pulsar el boton capture. Activa la actualización de los graficos.
        public void play()
        {
            if (timer == null)
            {
                initTimer();
                timer.Start(); 
                startModels(); // Hace que los graficos se actualizen. Se tiene que llamar.
            }
        }
        // Funcion que se llama al pulsar el boton pause. Pausa los graficos si se estan actualizando
        // y los vuelve a actualizar si estan pausados.
        public PauseState pause()
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    pauseModels(); // Hace que los graficos se dejen de actualizar. Se tiene que llamar.
                    return PauseState.Play; //Cambia a play. Se tiene que llamar.
                }
                else
                {
                    timer.Start();
                    startModels(); // Hace que los graficos se actualizen. Se tiene que llamar.
                    return PauseState.Pause; //Cambia a pause. Se tiene que llamar.
                }
            }
            return PauseState.Pause; //Al principio esta en pause. 
        }
        // Funcion que se llama al pulsar el boton stop.
        public void stop()
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    pauseModels(); // Hace que los graficos se dejen de actualizar. Se tiene que llamar.
                }
                clearModels(); // Borra los datos de los graficos. El cambio se ve la proxima vez que se actualizen. Se tiene que llamar.
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
