using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Diagnostics;

namespace ibcdatacsharp.UI.GraphWindow
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int NUM_SERIES = 3;
        private const int MAX_POINTS = 100;
        private OxyColor[] colors = new OxyColor[] {OxyColors.Red, OxyColors.Green, OxyColors.Blue };

        public Model(double minY, double maxY, string titleY = "", string units = "")
        {
            SetupModel(minY, maxY, titleY, units);
        }
        // Inicializa el modelo
        private void SetupModel(double minY, double maxY, string titleY, string units)
        {
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minY, Maximum = maxY, Title = titleY, Unit = units, FontSize = 10 , IntervalLength = 20 });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "frames", Unit = "kFreq", FontSize = 10, IntervalLength = 200});

            for (int i = 0; i < NUM_SERIES; i++)
            {
                PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = colors[i] });
            }

        }

        public PlotModel PlotModel { get; private set; }
        // Añade un punto por linea
        public void update(int frame, double[] data)
        {
            double kframes = frame / 1000.0;
            for(int i = 0; i < NUM_SERIES; i++)
            {
                (PlotModel.Series[i] as LineSeries).Points.Add(new DataPoint(kframes, data[i]));
            }
            if((PlotModel.Series[0] as LineSeries).Points.Count > MAX_POINTS)
            {
                for (int i = 0; i < NUM_SERIES; i++)
                {
                    (PlotModel.Series[i] as LineSeries).Points.RemoveAt(0);
                }
            }
            PlotModel.InvalidatePlot(true);
            double minkframes = (PlotModel.Series[0] as LineSeries).Points[0].X;
            PlotModel.Axes[1].Reset();
            PlotModel.Axes[1].Maximum = kframes;
            PlotModel.Axes[1].Minimum = minkframes;
            //PlotModel.Axes[1].Zoom(minkframes, kframes);
            /*
            if (frame > MAX_POINTS)
            {
                double kmaxPoints = MAX_POINTS / 1000.0;
                PlotModel.Axes[1].Reset();
                PlotModel.Axes[1].Maximum = kframes;
                PlotModel.Axes[1].Minimum = (kframes - kmaxPoints);
                PlotModel.Axes[1].Zoom(PlotModel.Axes[1].Minimum, PlotModel.Axes[1].Maximum);
            }
            else
            {
                PlotModel.Axes[1].Reset();
                PlotModel.Axes[1].Maximum = kframes;
                PlotModel.Axes[1].Minimum = 0;
            }*/
        }
        // Borra todos los puntos de todas las lineas
        public void clear()
        {
            for(int i = 0; i < NUM_SERIES; i++)
            {
                (PlotModel.Series[i] as LineSeries).Points.Clear();
            }
        }
    }
}