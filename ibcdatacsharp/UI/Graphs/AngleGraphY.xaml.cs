using ibcdatacsharp.UI.Device;
using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para AngleGraphX.xaml
    /// </summary>
    public partial class AngleGraphY : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;

        const bool hasToRender = true;
        public Model1S model { get; private set; }
        public AngleGraphY()
        {
            InitializeComponent();
            model = new Model1S(plot);
            model.valueEvent += onUpdateAngle;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;

            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("degrees");
        }
        private void onUpdateAngle(object sender, double value)
        {
            angle.Text = value.ToString("000.000");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(GraphData data)
        {
            double[] angleY = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // Provisional hay que cambiarlo por los angulos
                angleY[i] = ((FrameData2IMUs)data[i]).angleY;
            }
            await update(angleY);
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
            if (hasToRender)
            {
                await Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
                {
                    model.render();
                });
            }
        }

        public async void drawRealTimeData(double angle)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(angle);
            });
        }
        public async void drawData(float[] angle)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(angle);
            });
        }

        private void onSetOffset(object sender, RoutedEventArgs e)
        {
            string offset_point = offset.Text.Replace(",", ".");
            model.offset = float.Parse(offset_point, CultureInfo.InvariantCulture);
        }
        private void onClearOffset(object sender, RoutedEventArgs e)
        {
            model.offset = 0;
            offset.Text = "000.000";
        }
    }
}
