using OpenCvSharp.Flann;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Drawing;
using System.Windows.Markup;

namespace ibcdatacsharp.UI.Graphs
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model1S
    {
        public delegate void ValueEventHandler(object sender, double value);
        public event ValueEventHandler valueEvent;
        private const int RIGHT_SEPARATION = 20;
        private const int MAX_POINTS = 100;
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;
        double[] values;
        SignalPlot signalPlot;
        private int nextIndex = 0;
        private WpfPlot plot;

        private const double MIN_Y = -360;
        private const double MAX_Y = 360;

        private Color frameColor = Color.Black;
        private VLine lineFrame;
        private const float verticalLineWidth = 0.5f;

        private Color dataColor = Color.Red;
        private HLine lineData;
        private const float horizontalLineWidth = 0.5f;
        public Model1S(WpfPlot plot)
        {
            this.plot = plot;
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax:MAX_POINTS);
            paintAreas();

            lineFrame = plot.Plot.AddVerticalLine(0, color: frameColor, width: verticalLineWidth, LineStyle.Dash);
            /*
            lineFrame.PositionLabel = true;
            lineFrame.PositionLabelBackground = frameColor;
            lineFrame.PositionFormatter = customFormatter;

            lineData = plot.Plot.AddHorizontalLine(0, color: dataColor, width: horizontalLineWidth, style: LineStyle.Dash);
            lineData.PositionLabel = true;
            lineData.PositionLabelBackground = dataColor;
            lineData.PositionFormatter = customFormatter;
            */
            plot.Plot.Style(Style.Seaborn);
            plot.Refresh();
        }
        static string customFormatter(double position)
        {
            if (position == 0)
                return "zero";
            else if (position > 0)
                return $"+{position:F2}";
            else
                return $"({Math.Abs(position):F2})";
        }
        // Pinta el fondo
        private void paintAreas()
        {
            int separation12 = -320;
            int separation23 = -160;
            int separation34 = -separation23;
            int separation45 = -separation12;
            byte alpha = 96;

            Color color15 = Color.FromArgb(alpha, Color.Yellow);
            Color color24 = Color.FromArgb(alpha, Color.YellowGreen);
            Color color3 = Color.FromArgb(alpha, Color.MediumPurple);

            plot.Plot.AddVerticalSpan(-360, separation12, color15);
            plot.Plot.AddVerticalSpan(separation12, separation23, color24);
            plot.Plot.AddVerticalSpan(separation23, separation34, color3);
            plot.Plot.AddVerticalSpan(separation34, separation45, color24);
            plot.Plot.AddVerticalSpan(separation45, 360, color15);
        }
        public void initCapture()
        {
            values = new double[CAPACITY];
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Plot.XAxis2.SetSizeLimit(max: 5);
            plot.Plot.Remove(signalPlot);
            signalPlot = plot.Plot.AddSignal(values, color: dataColor);
            signalPlot.OffsetY = _offset;
            nextIndex = 0;
            maxRenderIndex = nextIndex;
        }
        #region Replay
        // Añade todos los datos de golpe (solo para replay)
        public void updateData(double[] data)
        {
            values = data;
            plot.Plot.Remove(signalPlot);
            signalPlot = plot.Plot.AddSignal(values, color: dataColor);
            signalPlot.OffsetY = offset;

            maxRenderIndex = 0;
            //plot.Plot.SetAxisLimitsX(xMin: 0, xMax: Math.Min(MAX_POINTS, values.Length));
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax: values.Length);
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Render();
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            index = Math.Min(index, values.Length); //Por si acaso
            maxRenderIndex = index;
            //plot.Plot.SetAxisLimitsX(xMin: Math.Max(0, index - MAX_POINTS),
            //    xMax: Math.Max(index + RIGHT_SEPARATION, Math.Min(MAX_POINTS, values.Length)));
            plot.Render();
            valueEvent?.Invoke(this, values[index] + offset);
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
                signalPlot = plot.Plot.AddSignal(values, color: dataColor);
                signalPlot.OffsetY = offset;
            }
            values[nextIndex] = data;
            nextIndex++;
            valueEvent?.Invoke(this, data + offset);
        }
        public void updateData(float[] data, bool render = true)
        {
            if(nextIndex + data.Length >= CAPACITY) // No deberia pasar
            {
                CAPACITY = CAPACITY * GROW_FACTOR;
                Array.Resize(ref values, CAPACITY);
                plot.Plot.Remove(signalPlot);
                signalPlot = plot.Plot.AddSignal(values, color: dataColor);
                signalPlot.OffsetY = offset;
            }
            for(int i = 0; i < data.Length; i++)
            {
                values[nextIndex + i] = data[i];
            }
            nextIndex+= data.Length;
            valueEvent?.Invoke(this, data[data.Length - 1] + offset);
            if (render)
            {
                this.render();
            }
        }
        // Actualiza el renderizado
        public void render()
        {
            int index = nextIndex - 1;
            if (index < 0)
            {
                index = 0;
            }
            maxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
                xMax: Math.Max(index + RIGHT_SEPARATION, Math.Min(MAX_POINTS, values.Length)));
            plot.Render();
        }
        // Borra todos los puntos
        public void clear()
        {
            nextIndex = 0;
            maxRenderIndex = nextIndex;
            plot.Render();
        }
        // Usar esto para actualizar la line tambien
        private int maxRenderIndex
        {
            set
            {
                signalPlot.MaxRenderIndex = value;
                lineFrame.X = value;
                //lineData.Y = values[value];
            }
            get
            {
                return signalPlot.MaxRenderIndex;
            }
        }
        private double _offset = 0;
        public double offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                if (signalPlot != null)
                {
                    signalPlot.OffsetY = value;
                    plot.Render();
                    int index = Math.Max(0, maxRenderIndex - 1);
                    valueEvent?.Invoke(this, values[index] + offset);
                }
            }
        }
    }
}