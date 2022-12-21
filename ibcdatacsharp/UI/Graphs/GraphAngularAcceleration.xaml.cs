using ibcdatacsharp.UI.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para AngularAcceleration.xaml
    /// </summary>
    public partial class GraphAngularAcceleration : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        const bool hasToRender = true;
        public Model3S model { get; private set; }
        public GraphAngularAcceleration()
        {
            InitializeComponent();
            model = new Model3S(plot, -500, 500, title: "Acceleracion angular", units: "grados/s^2");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("degrees/s\xB2");
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
        public async void drawData(Vector3[] acc)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(acc);
            });
        }
        public async void drawData(GraphData data)
        {
            double[] x = new double[data.length];
            double[] y = new double[data.length];
            double[] z = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                // cambiar esto
                x[i] = ((FrameData2IMUs)data[i]).angularAcceleration.X;
                y[i] = ((FrameData2IMUs)data[i]).angularAcceleration.Y;
                z[i] = ((FrameData2IMUs)data[i]).angularAcceleration.Z;
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
