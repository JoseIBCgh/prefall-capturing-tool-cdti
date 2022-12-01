using ibcdatacsharp.UI.Device;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para AngleGraphX.xaml
    /// </summary>
    public partial class AngleGraphX : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        public Model1S model { get; private set; }
        public AngleGraphX()
        {
            InitializeComponent();
            model = new Model1S(plot);
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;

            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("grados");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(GraphData data)
        {
            double[] angleX = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // Provisional hay que cambiarlo por los angulos
                angleX[i] = data[i].accX;
            }
            await update(angleX);
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }

        // Funcion para actualizar la grafica de golpe (para replay)
        public async Task update(double[] data)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(data);
            });
        }
        // Actualiza los datos de los grafos
        public async void onTick(object sender, EventArgs e)
        {
            AngleArgs angleArgs = device.angleData;
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(angleArgs.angle[0]);
            });
        }
        // Borra el contenido de los graficos
        public async void clearData()
        {
            await Dispatcher.BeginInvoke(CLEAR_PRIORITY, () =>
            {
                model.clear();
            });
        }
        // Actualiza el render de los grafos
        public async void onRender(object sender, EventArgs e)
        {
            await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.render();
            });
        }

        public async void drawRealTimeData(double angle)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(angle);
            });
        }
    }
}
