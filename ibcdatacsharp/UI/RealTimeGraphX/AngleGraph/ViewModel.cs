using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using System.Windows.Media;

namespace ibcdatacsharp.UI.RealTimeGraphX.AngleGraph
{
    public class ViewModel
    {
        //Graph controller with timespan as X axis and double as Y.
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


            //We will attach the surface using WPF binding...
            //Controller.Surface = null;
        }
        public void update(int frame, double data)
        {
            double kframes = frame / 1000.0;
            Controller.PushData(kframes, data);
        }
        public void clear()
        {
            Controller.Clear();
        }
    }
}
