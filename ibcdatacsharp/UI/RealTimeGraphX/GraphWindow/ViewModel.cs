using RealTimeGraphX.DataPoints;
using RealTimeGraphX.Renderers;
using RealTimeGraphX.WPF;
using System.Collections.Generic;
using System.Windows.Media;

namespace ibcdatacsharp.UI.RealTimeGraphX.GraphWindow
{
    // Modelos para el grfico acc, gyr, mag version RealTimeGraphX
    public class ViewModel
    {
        public WpfGraphController<DoubleDataPoint, DoubleDataPoint> Controller { get; set; }

        public ViewModel(double minY, double maxY)
        {
            Controller = new WpfGraphController<DoubleDataPoint, DoubleDataPoint>();
            Controller.Renderer = new ScrollingLineRenderer<WpfGraphDataSeries>();
            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "X",
                Stroke = Colors.Red,
            });
            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Y",
                Stroke = Colors.Green,
            });
            Controller.DataSeriesCollection.Add(new WpfGraphDataSeries()
            {
                Name = "Z",
                Stroke = Colors.Blue,
            });
            Controller.Range.MinimumY = minY;
            Controller.Range.MaximumY = maxY;
            Controller.Range.MaximumX = 1;
        }
        // Añade un punto
        public void update(int frame, double[] data)
        {
            double kframes = frame / 1000.0;
            DoubleDataPoint kframesddp = new DoubleDataPoint(kframes);
            List<DoubleDataPoint> kframesList = new List<DoubleDataPoint>();
            List<DoubleDataPoint> dataList = new List<DoubleDataPoint>();
            for(int i = 0; i < data.Length; i++)
            {
                kframesList.Add(kframesddp);
                dataList.Add(new DoubleDataPoint(data[i]));
            }
            Controller.PushData(kframesList, dataList);
        }
        // Elimina todos los puntos
        public void clear()
        {
            Controller.Clear();
        }
    }
}
