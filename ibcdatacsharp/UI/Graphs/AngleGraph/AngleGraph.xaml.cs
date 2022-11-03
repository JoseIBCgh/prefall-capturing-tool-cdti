using ibcdatacsharp.UI.Device;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs.AngleGraph
{
    /// <summary>
    /// Lógica de interacción para AngleGraph.xaml
    /// </summary>
    public partial class AngleGraph : Page, GraphInterface
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
        public Model modelX { get; private set; }
        public Model modelY { get; private set; }
        public Model modelZ { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelX = new Model(angleX, titleY: "X Angle");
            modelY = new Model(angleY, titleY: "Y Angle");
            modelZ = new Model(angleZ, titleY: "Z Angle");
        }
        // Funcion para actualizar la grafica X
        public async Task updateX(double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.updateData(data);
            });
        }
        // Funcion para borrar los datos X
        public async Task clearX()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelX.clear();
            });
        }
        // Funcion para actualizar la grafica Y
        public async Task updateY(double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.updateData(data);
            });
        }
        // Funcion para borrar los datos Y
        public async Task clearY()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelY.clear();
            });
        }
        // Funcion para actualizar la grafica Z
        public async Task updateZ(double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.updateData(data);
            });
        }
        // Funcion para borrar los datos Z
        public async Task clearZ()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                modelZ.clear();
            });
        }
        // Actualiza los datos de los grafos
        public async void onTick(object sender, EventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            await Task.WhenAll(new Task[]
            {
                updateX(angleArgs.angle[0]),
                updateY(angleArgs.angle[1]),
                updateZ(angleArgs.angle[2])
            });
        }
        // Borra el contenido de los graficos
        public async void clearData()
        {
            await Task.WhenAll(new Task[] {
                clearX(),
                clearY(),
                clearZ(),
            });
        }
        // Actualiza el render del grafo X
        public async Task renderX()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.render();
            });
        }
        // Actualiza el render del grafo Y
        public async Task renderY()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.render();
            });
        }
        // Actualiza el render del grafo Z
        public async Task renderZ()
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.render();
            });
        }
        // Actualiza el render de los grafos
        public async void onRender(object sender, EventArgs e)
        {
            await Task.WhenAll(new Task[]
            {
                renderX(),
                renderY(),
                renderZ(),
            });
        }
    }
}
