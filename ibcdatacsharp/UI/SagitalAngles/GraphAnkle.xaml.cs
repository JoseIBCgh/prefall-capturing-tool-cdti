using ibcdatacsharp.UI.Graphs;
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

namespace ibcdatacsharp.UI.SagitalAngles
{
    /// <summary>
    /// Lógica de interacción para GraphHip.xaml
    /// </summary>
    public partial class GraphAnkle : Page
    {
        public Model model { get; private set; }
        public GraphAnkle()
        {
            InitializeComponent();
            model = new Model(plot, "Dorsiflexion", "Plantarflexion");
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
    }
}
