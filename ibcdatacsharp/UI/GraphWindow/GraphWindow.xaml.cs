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
        int framesAccelerometer = 0;
        int framesGyroscope = 0;
        int framesMagnetometer = 0;
        const int MAX_POINTS = 100; 
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
            PlotModel model = modelAccelerometer.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                double kframes = framesAccelerometer / 1000.0;
                (model.Series[0] as LineSeries).Points.Add(new DataPoint(kframes, x));
                (model.Series[1] as LineSeries).Points.Add(new DataPoint(kframes, y));
                (model.Series[2] as LineSeries).Points.Add(new DataPoint(kframes, z));
                
                framesAccelerometer++;
                kframes = kframes + 1 / 1000.0;

                model.InvalidatePlot(true);
                if (framesAccelerometer > MAX_POINTS)
                {
                    double kmaxPoints = MAX_POINTS / 1000.0;
                    model.Axes[1].Reset();
                    model.Axes[1].Maximum = kframes;
                    model.Axes[1].Minimum = (kframes - kmaxPoints);
                    model.Axes[1].Zoom(model.Axes[1].Minimum, model.Axes[1].Maximum);
                }
                else
                {
                    model.Axes[1].Reset();
                    model.Axes[1].Maximum = kframes;
                    model.Axes[1].Minimum = 0;
                }
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async void clearAccelerometer()
        {
            PlotModel model = modelAccelerometer.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                (model.Series[0] as LineSeries).Points.Clear();
                (model.Series[1] as LineSeries).Points.Clear();
                (model.Series[2] as LineSeries).Points.Clear();

                framesAccelerometer = 0;
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async void updateGyroscope(double x, double y, double z)
        {
            PlotModel model = modelGyroscope.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                double kframes = framesGyroscope / 1000.0;
                (model.Series[0] as LineSeries).Points.Add(new DataPoint(kframes, x));
                (model.Series[1] as LineSeries).Points.Add(new DataPoint(kframes, y));
                (model.Series[2] as LineSeries).Points.Add(new DataPoint(kframes, z));

                framesGyroscope++;
                kframes = kframes + 1 / 1000.0;

                model.InvalidatePlot(true);
                if (framesGyroscope > MAX_POINTS)
                {
                    double kmaxPoints = MAX_POINTS / 1000.0;
                    model.Axes[1].Reset();
                    model.Axes[1].Maximum = kframes;
                    model.Axes[1].Minimum = (kframes - kmaxPoints);
                    model.Axes[1].Zoom(model.Axes[1].Minimum, model.Axes[1].Maximum);
                }
                else
                {
                    model.Axes[1].Reset();
                    model.Axes[1].Maximum = kframes;
                    model.Axes[1].Minimum = 0;
                }
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async void clearGyroscope()
        {
            PlotModel model = modelGyroscope.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                (model.Series[0] as LineSeries).Points.Clear();
                (model.Series[1] as LineSeries).Points.Clear();
                (model.Series[2] as LineSeries).Points.Clear();

                framesGyroscope = 0;
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async void updateMagnetometer(double x, double y, double z)
        {
            PlotModel model = modelMagnetometer.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                double kframes = framesMagnetometer / 1000.0;
                (model.Series[0] as LineSeries).Points.Add(new DataPoint(kframes, x));
                (model.Series[1] as LineSeries).Points.Add(new DataPoint(kframes, y));
                (model.Series[2] as LineSeries).Points.Add(new DataPoint(kframes, z));

                framesMagnetometer++;
                kframes = kframes + 1 / 1000.0;

                model.InvalidatePlot(true);
                if (framesMagnetometer > MAX_POINTS)
                {
                    double kmaxPoints = MAX_POINTS / 1000.0;
                    model.Axes[1].Reset();
                    model.Axes[1].Maximum = kframes;
                    model.Axes[1].Minimum = (kframes - kmaxPoints);
                    model.Axes[1].Zoom(model.Axes[1].Minimum, model.Axes[1].Maximum);
                }
                else
                {
                    model.Axes[1].Reset();
                    model.Axes[1].Maximum = kframes;
                    model.Axes[1].Minimum = 0;
                }
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async void clearMagnetometer()
        {
            PlotModel model = modelMagnetometer.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                (model.Series[0] as LineSeries).Points.Clear();
                (model.Series[1] as LineSeries).Points.Clear();
                (model.Series[2] as LineSeries).Points.Clear();

                framesMagnetometer = 0;
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
        public void pause(ToolBar.ToolBar toolBar)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    toolBar.ChangePauseState(PauseState.Play); //Cambia a play. Se tiene que llamar.
                }
                else
                {
                    timer.Start();
                    toolBar.ChangePauseState(PauseState.Pause); //Cambia a pause. Se tiene que llamar.
                }
            }
        }
        // Funcion que se llama al pulsar el boton stop.
        public void stop(ToolBar.ToolBar toolBar)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                clearModels(); // Borra los datos de los graficos. El cambio se ve la proxima vez que se actualizen. Se tiene que llamar.
                toolBar.ChangePauseState(PauseState.Pause); // Cambia a pause. Se tiene que llamar.
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
