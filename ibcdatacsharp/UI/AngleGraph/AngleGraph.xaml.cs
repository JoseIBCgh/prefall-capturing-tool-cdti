using ibcdatacsharp.UI.ToolBar.Enums;
using OxyPlot;
using System;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.AngleGraph
{
    /// <summary>
    /// Lógica de interacción para AngleGraph.xaml
    /// </summary>
    public partial class AngleGraph : Page
    {
        DispatcherTimer timer; //Para los datos inventados
        public AngleGraph()
        {
            InitializeComponent();
            initModels();
            DataContext = this;
        }
        public Model modelX { get; private set; }
        public Model modelY { get; private set; }
        public Model modelZ { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelX = new Model(titleY: "X Angle");
            modelY = new Model(titleY: "Y Angle");
            modelZ = new Model(titleY: "Z Angle");
        }
        // Borra el contenido de los graficos
        private void clearModels()
        {
            clearX();
            clearY();
            clearZ();
        }
        // Funcion para actualizar la grafica del acelerometro
        public async void updateX(double data)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelX.update(data);
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async void clearX()
        {
            PlotModel model = modelX.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelX.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async void updateY(double data)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelY.update(data);
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async void clearY()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelY.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async void updateZ(double data)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelZ.update(data);
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async void clearZ()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelZ.clear();
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
            double x = (random.NextDouble() - 0.5) * 400;
            updateX(x); // Llamar a esta funcion para actualizar el angulo X

            double y = (random.NextDouble() - 0.5) * 400;
            updateY(y); // Llamar a esta funcion para actualizar el angulo Y

            double z = (random.NextDouble() - 0.5) * 400;
            updateZ(z); // Llamar a esta funcion para actualizar el angulo Z
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
