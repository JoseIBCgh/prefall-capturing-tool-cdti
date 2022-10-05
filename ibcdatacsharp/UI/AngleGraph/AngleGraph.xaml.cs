using ibcdatacsharp.UI.Device;
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
        // Recive los datos del IMU inventado
        public void onReceiveData(object sender, AngleArgs args)
        {
            updateX(args.angle[0]);
            updateY(args.angle[1]);
            updateZ(args.angle[2]);
        }
        // Borra el contenido de los graficos
        public void onClearData(object sender)
        {
            clearX();
            clearY();
            clearZ();
        }
    }
}
