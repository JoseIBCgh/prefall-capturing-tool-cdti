using ibcdatacsharp.UI.Graphs;
using ibcdatacsharp.UI.Graphs.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ibcdatacsharp.UI.Graphs.Sagital
{
    /// <summary>
    /// Lógica de interacción para GraphHip.xaml
    /// </summary>
    public partial class GraphAnkle : Page, GraphInterface
    {
        private const DispatcherPriority UPDATE_PRIORITY = DispatcherPriority.Render;
        public ModelSagital model { get; private set; }
        private const string INITIAL_OFFSET = "0.0";
        public GraphAnkle()
        {
            InitializeComponent();
            model = new ModelSagital(plot, "Dorsiflexion", "Plantarflexion");
            model.valueEvent += onUpdateAngle;
            DataContext = this;
        }
        public void initCapture()
        {
            model.initCapture();
        }
        public async void redrawData(float[] data)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                lock (model)
                {
                    model.redrawData(data);
                }
            });
        }
        public async void drawData(float[] data)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                lock (model)
                {
                    model.updateData(data);
                }
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
                angle[i] = ((FrameDataSagital)data[i]).rightAnkle;
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
        private void onSetOffset(object sender, RoutedEventArgs e)
        {
            string offset_point = offset.Text.Replace(",", ".");
            model.offset = float.Parse(offset_point, CultureInfo.InvariantCulture);
        }
        private void onClearOffset(object sender, RoutedEventArgs e)
        {
            model.offset = 0;
            offset.Text = INITIAL_OFFSET;
        }
    }
}
