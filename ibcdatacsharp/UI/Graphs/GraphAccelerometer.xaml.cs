using ibcdatacsharp.UI.Device;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs
{
    /// <summary>
    /// Lógica de interacción para GraphAccelerometer.xaml
    /// </summary>
    public partial class GraphAccelerometer : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        private const DispatcherPriority CLEAR_PRIORITY = DispatcherPriority.Render;
        protected Device.Device device;

        const bool hasToRender = true;
        public Model3S model { get; private set; }
        public GraphAccelerometer()
        {
            InitializeComponent();
            model = new Model3S(plot, -50, 50, title: "Accelerometer", units: "m/s^2");
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            device = mainWindow.device;
            DataContext = this;
            this.plot.Plot.XLabel("Frames");
            this.plot.Plot.YLabel("m/s\xB2");
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawRealTimeData(double accX, double accY, double accZ)
        {
            double[] acc = new double[3] { accX, accY, accZ };

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(acc);
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
                model.updateData(accX, accY, accZ);
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
            await Application.Current.Dispatcher.InvokeAsync( () =>
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
