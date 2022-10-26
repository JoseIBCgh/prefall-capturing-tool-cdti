using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ScottPlot;
using System;
using System.Drawing;
using System.Windows.Controls;

namespace ibcdatacsharp.UI.AngleGraph
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int MAX_POINTS = 100;
        private const int CAPACITY = 100000;
        readonly double[] values = new double[CAPACITY];
        readonly ScottPlot.Plottable.SignalPlot signalPlot;
        private int nextIndex = 0;
        private WpfPlot plot;

        public Model(WpfPlot plot,string titleY = "")
        {
            this.plot = plot;
            signalPlot = plot.Plot.AddSignal(values, color: Color.Red);
            SetupModel(titleY);
            signalPlot.MaxRenderIndex = nextIndex;
            plot.Refresh();
        }
        // Inicializa el modelo
        private void SetupModel(string titleY)
        {
            plot.Plot.SetAxisLimits(yMin: -200, yMax: 200);
            /*
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -200, Maximum = 200, Title = titleY, Unit = "degrees", FontSize = 10, IntervalLength = 20 });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "frames", Unit = "kFreq", FontSize = 10, IntervalLength = 200 });

            PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = color });

            paintAreas();

            PlotModel.Axes[1].Reset();
            PlotModel.Axes[1].Maximum = 1.0 / 1000.0;
            PlotModel.Axes[1].Minimum = 0;
            */
        }
        // Pinta el fondo
        /*
        private void paintAreas()
        {
            int separation12 = -170;
            int separation23 = -90;
            int separation34 = -separation23;
            int separation45 = -separation12;
            byte alpha = 96;

            OxyColor color15 = OxyColor.FromAColor(alpha, OxyColors.Yellow);
            OxyColor color24 = OxyColor.FromAColor(alpha, OxyColors.YellowGreen);
            OxyColor color3 = OxyColor.FromAColor(alpha, OxyColors.MediumPurple);

            AreaSeries area1 = new AreaSeries { Fill = color15, StrokeThickness = 0 };
            area1.Points.Add(new DataPoint(0, PlotModel.Axes[0].Minimum));
            area1.Points.Add(new DataPoint(int.MaxValue, PlotModel.Axes[0].Minimum));
            area1.Points2.Add(new DataPoint(0, separation12));
            area1.Points2.Add(new DataPoint(int.MaxValue, separation12));
            PlotModel.Series.Add(area1);

            AreaSeries area2 = new AreaSeries { Fill = color24, StrokeThickness = 0 };
            area2.Points.Add(new DataPoint(0, separation12));
            area2.Points.Add(new DataPoint(int.MaxValue, separation12));
            area2.Points2.Add(new DataPoint(0, separation23));
            area2.Points2.Add(new DataPoint(int.MaxValue, separation23));
            PlotModel.Series.Add(area2);

            AreaSeries area3 = new AreaSeries {Fill = color3, StrokeThickness=0 };
            area3.Points.Add(new DataPoint(0, separation23));
            area3.Points.Add(new DataPoint(int.MaxValue, separation23));
            area3.Points2.Add(new DataPoint(0, separation34));
            area3.Points2.Add(new DataPoint(int.MaxValue, separation34));
            PlotModel.Series.Add(area3);

            AreaSeries area4 = new AreaSeries { Fill = color24, StrokeThickness = 0 };
            area4.Points.Add(new DataPoint(0, separation34));
            area4.Points.Add(new DataPoint(int.MaxValue, separation34));
            area4.Points2.Add(new DataPoint(0, separation45));
            area4.Points2.Add(new DataPoint(int.MaxValue, separation45));
            PlotModel.Series.Add(area4);

            AreaSeries area5 = new AreaSeries { Fill = color15, StrokeThickness = 0 };
            area5.Points.Add(new DataPoint(0, separation45));
            area5.Points.Add(new DataPoint(int.MaxValue, separation45));
            area5.Points2.Add(new DataPoint(0, PlotModel.Axes[0].Maximum));
            area5.Points2.Add(new DataPoint(int.MaxValue, PlotModel.Axes[0].Maximum));
            PlotModel.Series.Add(area5);
        }
        */

        // Añade un punto
        public void updateData(double data)
        {
            values[nextIndex] = data;
            nextIndex++;
        }
        public void render()
        {
            int index = nextIndex - 1;
            signalPlot.MaxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS), xMax: index);
            plot.Render();
        }
        // Borra todos los puntos
        public void clear()
        {
            nextIndex = 0;
        }
    }
}