using ibcdatacsharp.UI.Device;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
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
        public Model1S modelX { get; private set; }
        public Model1S modelY { get; private set; }
        public Model1S modelZ { get; private set; }
        // Funcion para inicializar los graficos
        private void initModels()
        {
            modelX = new Model1S(angleX);
            modelY = new Model1S(angleY);
            modelZ = new Model1S(angleZ);
        }
        public void initCapture()
        {
            modelX.initCapture();
            modelY.initCapture();
            modelZ.initCapture();
        }
        public async void drawData(GraphData data)
        {
            double[] angleX = new double[data.length];
            double[] angleY = new double[data.length];
            double[] angleZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // Provisional hay que cambiarlo por los angulos
                angleX[i] = ((FrameData1IMU)data[i]).accX;
                angleY[i] = ((FrameData1IMU)data[i]).accY;
                angleZ[i] = ((FrameData1IMU)data[i]).accZ;
            }
            await Task.WhenAll(new Task[]
            {
                updateX(angleX),
                updateY(angleY),
                updateZ(angleZ)
            });
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Task.WhenAll(new Task[]
            {
                updateIndexX(frame),
                updateIndexY(frame),
                updateIndexZ(frame),
            });
        }
        // Funciones para hacer cambiar el indice de cada grafo
        #region Update Index
        public async Task updateIndexX(int frame)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.updateIndex(frame);
            });
        }
        public async Task updateIndexY(int frame)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelY.updateIndex(frame);
            });
        }
        public async Task updateIndexZ(int frame)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelZ.updateIndex(frame);
            });
        }
        #endregion Update Index
        // Funciones para hacer el update y clear de cada grafo
        #region Update, Clear graph
        // Funcion para actualizar la grafica X
        public async Task updateX(double data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                modelX.updateData(data);
            });
        }
        // Funcion para actualizar la grafica X de golpe (para replay)
        public async Task updateX(double[] data)
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
        // Funcion para actualizar la grafica Y de golpe (para replay)
        public async Task updateY(double[] data)
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
        // Funcion para actualizar la grafica Z de golpe(para replay)
        public async Task updateZ(double[] data)
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
        #endregion Update, Clear graph
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
        // Funciones para hacer el render de cada grafo
        #region Render graph
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
        #endregion Render graph
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

        public void drawRealTimeData(double accX, double accY, double accZ)
        {
            throw new NotImplementedException();
        }
        public bool hasToRender { get; set; }
    }
}
