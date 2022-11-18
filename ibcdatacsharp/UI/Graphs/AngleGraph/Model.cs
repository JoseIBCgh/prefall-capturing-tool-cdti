using ScottPlot;
using System;
using System.Drawing;

namespace ibcdatacsharp.UI.Graphs.AngleGraph
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int MAX_POINTS = 100;
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;
        double[] values;
        ScottPlot.Plottable.SignalPlot signalPlot;
        private int nextIndex = 0;
        private WpfPlot plot;

        private const double MIN_Y = -200;
        private const double MAX_Y = 200;

        public Model(WpfPlot plot)
        {
            this.plot = plot;
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Plot.XAxis2.SetSizeLimit(max: 5, pad: 0);
            plot.Plot.XAxis.SetSizeLimit(pad: 0);
            paintAreas();
            plot.Refresh();
        }
        // Pinta el fondo
        private void paintAreas()
        {
            int separation12 = -170;
            int separation23 = -90;
            int separation34 = -separation23;
            int separation45 = -separation12;
            byte alpha = 96;

            Color color15 = Color.FromArgb(alpha, Color.Yellow);
            Color color24 = Color.FromArgb(alpha, Color.YellowGreen);
            Color color3 = Color.FromArgb(alpha, Color.MediumPurple);

            plot.Plot.AddVerticalSpan(double.MinValue, separation12, color15);
            plot.Plot.AddVerticalSpan(separation12, separation23, color24);
            plot.Plot.AddVerticalSpan(separation23, separation34, color3);
            plot.Plot.AddVerticalSpan(separation34, separation45, color24);
            plot.Plot.AddVerticalSpan(separation45, double.MaxValue, color15);
        }
        public void initCapture()
        {
            values = new double[CAPACITY];
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Plot.XAxis2.SetSizeLimit(max: 5);
            plot.Plot.Remove(signalPlot);
            signalPlot = plot.Plot.AddSignal(values, color: Color.Red);
            nextIndex = 0;
            signalPlot.MaxRenderIndex = nextIndex;
        }
        #region Replay
        // Añade todos los datos de golpe (solo para replay)
        public void updateData(double[] data)
        {
            values = data;
            plot.Plot.Remove(signalPlot);
            signalPlot = plot.Plot.AddSignal(values, color: Color.Red);
            signalPlot.MaxRenderIndex = 0;
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax: Math.Min(MAX_POINTS, values.Length));
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Render();
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            index = Math.Min(index, values.Length); //Por si acaso
            signalPlot.MaxRenderIndex = index;
            plot.Plot.SetAxisLimitsX(xMin: Math.Max(0, index - MAX_POINTS), 
                xMax: Math.Max(index, Math.Min(MAX_POINTS, values.Length)));
            plot.Render();
        }
        #endregion Replay

        // Añade un punto
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
        // Actualiza el renderizado
        public void render()
        {
            int index = nextIndex - 1;
            signalPlot.MaxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
                xMax: Math.Max(index, Math.Min(MAX_POINTS, values.Length)));
            plot.Render();
        }
        // Borra todos los puntos
        public void clear()
        {
            nextIndex = 0;
        }
    }
}