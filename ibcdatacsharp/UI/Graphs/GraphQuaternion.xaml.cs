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
    /// Lógica de interacción para GraphQuaternion.xaml
    /// </summary>
    public partial class GraphQuaternion : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;
        const bool hasToRender = true;
        public Model4S model { get; private set; }
        public GraphQuaternion()
        {
            InitializeComponent();
            model = new Model4S(plot, -1, 1, title: "Quaternion");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("Quaternion");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawRealTimeData(double w, double x, double y, double z)
        {
            double[] data = new double[4] { w, x, y, z };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(data);
            });

        }
        public async void drawRealTimeData(Quaternion q)
        {
            double[] data = new double[4] { q.W, q.X, q.Y, q.Z };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(data);
            });

        }
        public async void drawData(Quaternion[] q)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(q);
            });
        }
        // Temporal
        public async void drawData(GraphData data)
        {
            double[] accX = new double[data.length];
            double[] accY = new double[data.length];
            double[] accZ = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                accX[i] = ((FrameData1IMU)data[i]).accX;
                accY[i] = ((FrameData1IMU)data[i]).accY;
                accZ[i] = ((FrameData1IMU)data[i]).accZ;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(accX, accY, accZ, accX);
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
