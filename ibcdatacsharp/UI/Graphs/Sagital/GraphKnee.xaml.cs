using ibcdatacsharp.UI.Graphs.Models;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Graphs.Sagital
{
    /// <summary>
    /// Lógica de interacción para GraphHip.xaml
    /// </summary>
    public partial class GraphKnee : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        public ModelSagital model { get; private set; }
        public GraphKnee()
        {
            InitializeComponent();
            model = new ModelSagital(plot, "Flexion", "Hyperextension");
            model.valueEvent += onUpdateAngle;
            DataContext = this;
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void drawData(float[] data)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.updateData(data);
            });
        }
        private void onUpdateAngle(object sender, double value)
        {
            angle.Text = value.ToString("000.000");
        }

        public async void drawData(GraphData data)
        {
            double[] angle = new double[data.length];
            for (int i = 0; i < data.length; i++)
            {
                angle[i] = ((FrameDataSagital)data[i]).rightKnee;
            }
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateData(angle);
            });
        }

        public async void onUpdateTimeLine(object sender, int frame)
        {
            await Application.Current.Dispatcher.BeginInvoke(UPDATE_PRIORITY, () =>
            {
                model.updateIndex(frame);
            });
        }

        public async void clearData()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                model.clear();
            });
        }
    }
}
