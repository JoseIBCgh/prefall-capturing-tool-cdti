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
            model = new Model(plot, -4, 4, titleY: "Magnetometer", units: "k(mT)");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
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
    }
}
