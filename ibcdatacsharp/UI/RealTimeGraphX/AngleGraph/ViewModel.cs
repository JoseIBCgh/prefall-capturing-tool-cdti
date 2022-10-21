using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using System.Windows.Media;

namespace ibcdatacsharp.UI.RealTimeGraphX.AngleGraph
{
    // Modelo para el angle graph version RealTimeGraphX
    public class ViewModel
    {
        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> Controller { get; set; }

        public ViewModel()
        {
            Controller = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            Controller.Renderer = new ScrollingLineRenderer<WpfGraphDataSeries>();
            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "X",
                Stroke = Colors.Red,
            });
            Controller.Range.MinimumY = -200;
            Controller.Range.MaximumY = 200;
            Controller.Range.MaximumX = 1;
        }
        // Añade un punto
        public void update(int frame, double data)
        {
            double kframes = frame / 1000.0;
            Controller.PushData(kframes, data);
        }
        // Elimina todos los puntos
        public void clear()
        {
            Controller.Clear();
        }
    }
}
