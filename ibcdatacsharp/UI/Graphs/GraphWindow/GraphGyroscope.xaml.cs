using ibcdatacsharp.UI.Device;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs.GraphWindow
{
    /// <summary>
    /// Lógica de interacción para GraphGyroscope.xaml
    /// </summary>
    public partial class GraphGyroscope : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        public Model model { get; private set; }
        public GraphGyroscope()
        {
            InitializeComponent();
            model = new Model(plot, -600, 600, title: "Gyroscope", units: "g/s^2");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
        }
        public async void drawData(GraphData data)
        {
            double[] gyrX = new double[data.length];
            double[] gyrY = new double[data.length];
            double[] gyrZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                gyrX[i] = data[i].gyrX;
                gyrY[i] = data[i].gyrY;
                gyrZ[i] = data[i].gyrZ;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(gyrX, gyrY, gyrZ);
            });
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }
        // Devuelve los datos del Giroscopio
        private double[] getData()
        {
            RawArgs rawArgs = device.rawData;
            return rawArgs.gyroscope;
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
    }
}
