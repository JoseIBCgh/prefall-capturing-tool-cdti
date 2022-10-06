using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.ToolBar.Enums;
using OxyPlot;
using System;
using System.Threading.Tasks;
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
        public async Task updateX(double data)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelX.update(data);
            });
        }
        // Funcion para borrar los datos del acelerometro
        public async Task clearX()
        {
            PlotModel model = modelX.PlotModel;
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelX.clear();
            });
        }
        // Funcion para actualizar la grafica del giroscopio
        public async Task updateY(double data)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelY.update(data);
            });
        }
        // Funcion para borrar los datos del giroscopio
        public async Task clearY()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelY.clear();
            });
        }
        // Funcion para actualizar la grafica del magnetometro
        public async Task updateZ(double data)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelZ.update(data);
            });
        }
        // Funcion para borrar los datos del magnetometro
        public async Task clearZ()
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelZ.clear();
            });
        }
        // Recive los datos del IMU inventado
        public async void onReceiveData(object sender, AngleArgs args)
        {
            await updateX(args.angle[0]);
            await updateY(args.angle[1]);
            await updateZ(args.angle[2]);
        }
        // Borra el contenido de los graficos
        public async void onClearData(object sender)
        {
            await clearX();
            await clearY();
            await clearZ();
        }
    }
}
