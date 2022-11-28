using ibcdatacsharp.UI.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphMagnetometer.xaml
    /// </summary>
    public partial class GraphMagnetometer : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        public Model model { get; private set; }
        public GraphMagnetometer()
        {
            InitializeComponent();
            model = new Model(plot, -6000, 6000, title: "Magnetometer", units: "k(mT)");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(GraphData data)
        {
            double[] magX = new double[data.length];
            double[] magY = new double[data.length];
            double[] magZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                magX[i] = data[i].magX;
                magY[i] = data[i].magY;
                magZ[i] = data[i].magZ;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(magX, magY, magZ);
            });
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }
        // Devuelve los datos del magnetometro
        private double[] getData()
        {
            RawArgs rawArgs = device.rawData;
            return rawArgs.magnetometer;
        }
        // Actualiza los datos
        public async void onTick(object sender, EventArgs e)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(getData());
            });
        }
        // Borra el contenido de los graficos
        public async void clearData()
        {
            await Application.Current.Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                model.clear();
            });
        }
        // Actualiza el renderizado
        public async void onRender(object sender, EventArgs e)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.render();
            });
        }

        public async void render()
        {
            await Application.Current.Dispatcher.InvokeAsync( () =>
            {
                model.render();
            });
        }

        public async void drawRealTimeData(double accX, double accY, double accZ)
        {
            double[] acc = new double[3] { accX, accY, accZ };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(acc);
            });
        }
    }
}
