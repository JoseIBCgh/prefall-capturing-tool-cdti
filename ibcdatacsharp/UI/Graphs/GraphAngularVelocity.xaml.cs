using ibcdatacsharp.UI.Device;
using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para AngularVelocity.xaml
    /// </summary>
    public partial class GraphAngularVelocity : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        const bool hasToRender = true;
        public Model3S model { get; private set; }
        public GraphAngularVelocity()
        {
            InitializeComponent();
            model = new Model3S(plot, -25, 25, title: "Velocidad angular", units: "grados/s");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("degrees/s");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawRealTimeData(double x, double y, double z)
        {
            double[] data = new double[3] { x, y, z };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(data);
            });

        }
        public async void drawRealTimeData(Vector3 data)
        {
            double[] array = new double[3] { data.X, data.Y, data.Z };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(array);
            });

        }
        public async void drawData(Vector3[] vel)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(vel);
            });
        }
        public async void drawData(GraphData data)
        {
            double[] x = new double[data.length];
            double[] y = new double[data.length];
            double[] z = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // Cambiar esto
                x[i] = ((FrameData2IMUs)data[i]).angularVelocity.X;
                y[i] = ((FrameData2IMUs)data[i]).angularVelocity.Y;
                z[i] = ((FrameData2IMUs)data[i]).angularVelocity.Z;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(x, y, z);
            });
        }
        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }
        // Devuelve los datos del Acelerometro
        private double[] getData()
        {
            RawArgs rawArgs = device.rawData;
            return rawArgs.accelerometer;
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
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.clear();
            });
        }

        //Actualiza el render

        public async void render()
        {
            if (hasToRender)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    model.render();
                });
            }
        }

        // Actualiza el render
        public async void onRender(object sender, EventArgs e)
        {
            if (hasToRender)
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    model.render();
                });
            }
        }
    }
}
