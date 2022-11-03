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
