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
            paintAreas();
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
        private void paintAreas()
        {
            int separation12 = -170;
            int separation23 = -90;
            int separation34 = -separation23;
            int separation45 = -separation12;
            byte alpha = 96;

            Color color15 = Color.FromArgb(96, Color.Yellow);
            Color color24 = Color.FromArgb(96, Color.YellowGreen);
            Color color3 = Color.FromArgb(96, Color.MediumPurple);

            plot.Plot.AddVerticalSpan(double.MinValue, separation12, color15);
            plot.Plot.AddVerticalSpan(separation12, separation23, color24);
            plot.Plot.AddVerticalSpan(separation23, separation34, color3);
            plot.Plot.AddVerticalSpan(separation34, separation45, color24);
            plot.Plot.AddVerticalSpan(separation45, double.MaxValue, color15);
        }

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