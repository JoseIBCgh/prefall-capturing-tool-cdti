//#define MOVE_DATA

using ScottPlot;
using System;
using System.Drawing;

namespace ibcdatacsharp.UI.AngleGraph
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int MAX_POINTS = 100;

#if MOVE_DATA
        private const int EXTRA = 20;
        private const int CAPACITY = MAX_POINTS + EXTRA;
        readonly double[] values;
        readonly ScottPlot.Plottable.SignalPlot signalPlot;
#else
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;
        double[] values;
        ScottPlot.Plottable.SignalPlot signalPlot;
#endif 
        private int nextIndex = 0;
        private WpfPlot plot;

        public Model(WpfPlot plot,string titleY = "")
        {
            values = new double[CAPACITY];
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

#if MOVE_DATA
        public void updateData(double data)
        {
            if(nextIndex >= CAPACITY) //No deberia de pasar
            {
                moveData();
            }
            values[nextIndex] = data;
            nextIndex++;
        }
        private void moveData()
        {
            int displacement = nextIndex - MAX_POINTS;
            if(displacement > 0)
            {
                for (int i = 0; i < MAX_POINTS; i++)
                {
                    int index_replacement = i + displacement;
                    int index_replaced = i;
                    values[index_replaced] = values[index_replacement];
                }
                nextIndex = MAX_POINTS;
            }
        }
        public void render()
        {
            if (nextIndex <= MAX_POINTS)
            {
                int index = nextIndex - 1;
                signalPlot.MaxRenderIndex = index;
                plot.Plot.SetAxisLimits(xMin: 0, xMax: index);
            }
            else
            {
                moveData();
            }
            plot.Render();
        }
#else
        public void updateData(double data)
        {
            if (nextIndex >= CAPACITY) // No deberia pasar
            {
                CAPACITY = CAPACITY * GROW_FACTOR;
                Array.Resize(ref values, CAPACITY);
                plot.Plot.Remove(signalPlot);
                signalPlot = plot.Plot.AddSignal(values, color: Color.Red);
            }
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
#endif
        // Borra todos los puntos
        public void clear()
        {
            nextIndex = 0;
        }
    }
}