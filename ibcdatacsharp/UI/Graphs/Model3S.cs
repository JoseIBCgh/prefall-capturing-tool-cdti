using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace ibcdatacsharp.UI.Graphs
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model3S
    {
        private const int RIGHT_SEPARATION = 20;
        private const int MAX_POINTS = 100;
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;

        double[] valuesX;
        double[] valuesY;
        double[] valuesZ;
        SignalPlot signalPlotX;
        SignalPlot signalPlotY;
        SignalPlot signalPlotZ;

        private int nextIndex = 0;
        private WpfPlot plot;

        private double minY;
        private double maxY;

        private Color frameColor = Color.Black;
        private VLine lineFrame;
        private const float verticalLineWidth = 0.5f;

        private const float horizontalLineWidth = 0.5f;
        private Color xColor = Config.colorX;
        private HLine lineX;
        private Color yColor = Config.colorY;
        private HLine lineY;
        private Color zColor = Config.colorZ;
        private HLine lineZ;

        public Model3S(WpfPlot plot, double minY, double maxY, string title = "", string units = "")
        {
            this.minY = minY;
            this.maxY = maxY;
            this.plot = plot;

            lineFrame = plot.Plot.AddVerticalLine(0, color: frameColor, width: verticalLineWidth, style:LineStyle.Dash);
            //lineFrame.PositionLabel = true;
            //lineFrame.PositionLabelBackground = frameColor;
            //lineFrame.PositionFormatter = customFormatter;
            /*
            lineX = plot.Plot.AddHorizontalLine(0, color: xColor, width: horizontalLineWidth, style: LineStyle.Dash);
            lineX.PositionLabel = true;
            lineX.PositionLabelBackground = xColor;
            lineX.PositionFormatter = customFormatter;

            lineY = plot.Plot.AddHorizontalLine(0, color: yColor, width: horizontalLineWidth, style: LineStyle.Dash);
            lineY.PositionLabel = true;
            lineY.PositionLabelBackground = yColor;
            lineY.PositionFormatter = customFormatter;

            lineZ = plot.Plot.AddHorizontalLine(0, color: zColor, width: horizontalLineWidth, style: LineStyle.Dash);
            lineZ.PositionLabel = true;
            lineZ.PositionLabelBackground = zColor;
            lineZ.PositionFormatter = customFormatter;
            */

            plot.Plot.SetAxisLimitsX(xMin: 0, MAX_POINTS);
            plot.Plot.Legend(location: Alignment.UpperRight);


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
        public void initCapture()
        {
            valuesX = new double[CAPACITY];
            valuesY = new double[CAPACITY];
            valuesZ = new double[CAPACITY];
            plot.Plot.Remove(signalPlotX);
            plot.Plot.Remove(signalPlotY);
            plot.Plot.Remove(signalPlotZ);
            signalPlotX = plot.Plot.AddSignal(valuesX, color: xColor, label: "X");
            signalPlotY = plot.Plot.AddSignal(valuesY, color: yColor, label: "Y");
            signalPlotZ = plot.Plot.AddSignal(valuesZ, color: zColor, label: "Z");
            signalPlotX.MarkerSize = 0;
            signalPlotY.MarkerSize = 0;
            signalPlotZ.MarkerSize = 0;
            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);
            nextIndex = 0;
            maxRenderIndex = nextIndex;
        }
        #region Replay
        // Añade todos los datos de golpe (solo para replay)
        public void updateData(double[] x, double[] y, double[] z)
        {
            valuesX = x;
            valuesY = y;
            valuesZ = z;
            plot.Plot.Remove(signalPlotX);
            plot.Plot.Remove(signalPlotY);
            plot.Plot.Remove(signalPlotZ);
            signalPlotX = plot.Plot.AddSignal(valuesX, color: xColor, label: "X");
            signalPlotY = plot.Plot.AddSignal(valuesY, color: yColor, label: "Y");
            signalPlotZ = plot.Plot.AddSignal(valuesZ, color: zColor, label: "Z");
            signalPlotX.MarkerSize = 0;
            signalPlotY.MarkerSize = 0;
            signalPlotZ.MarkerSize = 0;
            maxRenderIndex = 0;
            //plot.Plot.SetAxisLimitsX(xMin: 0, xMax: Math.Min(MAX_POINTS, valuesX.Length));
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax: valuesX.Length);
            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            index = Math.Min(index, valuesX.Length); //Por si acaso
            maxRenderIndex = index;

            signalPlotX.Label = "X= " + valuesX[index].ToString("0.##");
            signalPlotY.Label = "Y= " + valuesY[index].ToString("0.##");
            signalPlotZ.Label = "Z= " + valuesZ[index].ToString("0.##");

            //plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
            //    xMax: Math.Max(index + RIGHT_SEPARATION, Math.Min(MAX_POINTS, valuesX.Length)));
            plot.Render();
        }
        #endregion Replay
        // Esta version funciona mejor pero usa mas memoria. Si se sobrepasa la memoria incial hay que modificar el tamaño de las arrays.

        // Actualiza los datos
        public async void updateData(double[] data)
        {
            if (nextIndex >= CAPACITY) // No deberia pasar
            {
                CAPACITY = CAPACITY * GROW_FACTOR;
                Array.Resize(ref valuesX, CAPACITY);
                Array.Resize(ref valuesY, CAPACITY);
                Array.Resize(ref valuesZ, CAPACITY);
                plot.Plot.Remove(signalPlotX);
                plot.Plot.Remove(signalPlotY);
                plot.Plot.Remove(signalPlotZ);
                signalPlotX = plot.Plot.AddSignal(valuesX, color: Color.Red, label: "X");
                signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green, label: "Y");
                signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue, label: "Z");
                signalPlotX.MarkerSize = 0;
                signalPlotY.MarkerSize = 0;
                signalPlotZ.MarkerSize = 0;
            }
            valuesX[nextIndex] = data[0];
            valuesY[nextIndex] = data[1];
            valuesZ[nextIndex] = data[2];
            signalPlotX.Label = "X= " + data[0].ToString("0.##");
            signalPlotY.Label = "Y= " + data[1].ToString("0.##");
            signalPlotZ.Label = "Z= " + data[2].ToString("0.##");
            nextIndex++;
        }
        public async void updateData(Vector3[] data, bool render = true)
        {
            if (nextIndex + data.Length >= CAPACITY) // No deberia pasar
            {
                CAPACITY = CAPACITY * GROW_FACTOR;
                Array.Resize(ref valuesX, CAPACITY);
                Array.Resize(ref valuesY, CAPACITY);
                Array.Resize(ref valuesZ, CAPACITY);
                plot.Plot.Remove(signalPlotX);
                plot.Plot.Remove(signalPlotY);
                plot.Plot.Remove(signalPlotZ);
                signalPlotX = plot.Plot.AddSignal(valuesX, color: Color.Red, label: "X");
                signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green, label: "Y");
                signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue, label: "Z");
                signalPlotX.MarkerSize = 0;
                signalPlotY.MarkerSize = 0;
                signalPlotZ.MarkerSize = 0;
            }
            for (int i = 0; i < data.Length; i++)
            {
                valuesX[nextIndex + i] = data[i].X;
                valuesY[nextIndex + i ] = data[i].Y;
                valuesZ[nextIndex + i ] = data[i].Z;
            }
            signalPlotX.Label = "X= " + data[data.Length - 1].X.ToString("0.##");
            signalPlotY.Label = "Y= " + data[data.Length - 1].Y.ToString("0.##");
            signalPlotZ.Label = "Z= " + data[data.Length - 1].Z.ToString("0.##");
            nextIndex+= data.Length;
            if (render)
            {
                this.render();
            }
        }
        // Actualiza el renderizado
        public async void render()
        {
            int index = nextIndex - 1;
            if (index < 0)
            {
                index = 0;
            }
            maxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
                xMax: Math.Max(index + RIGHT_SEPARATION, Math.Min(MAX_POINTS, valuesX.Length)));
            plot.Render();
        }
        // Borra todos los puntos de todas las lineas
        public void clear()
        {
            nextIndex = 0;
            maxRenderIndex = nextIndex;
            plot.Render();
        }
        // Usar esto para actualizar la line tambien
        private int maxRenderIndex
        {
            get
            {
                return (int)lineFrame.X;
            }
            set
            {
                signalPlotX.MaxRenderIndex = value;
                signalPlotY.MaxRenderIndex = value;
                signalPlotZ.MaxRenderIndex = value;
                lineFrame.X = value;
                //lineX.Y = valuesX[value];
                //lineY.Y = valuesY[value];
                //lineZ.Y = valuesZ[value];
            }
        }
    }
}