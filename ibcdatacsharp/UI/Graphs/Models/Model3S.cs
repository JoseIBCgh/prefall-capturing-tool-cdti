using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;

namespace ibcdatacsharp.UI.Graphs.Models
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

        private CaptureModel captureModel;
        private ReplayModel replayModel;

        public Model3S(WpfPlot plot, double minY, double maxY, string title = "", string units = "")
        {
            captureModel = new CaptureModel(this);
            replayModel = new ReplayModel(this);

            this.minY = minY;
            this.maxY = maxY;
            this.plot = plot;

            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);

            lineFrame = plot.Plot.AddVerticalLine(0, color: frameColor, width: verticalLineWidth, style: LineStyle.Dash);

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
            clear();
            captureModel.initCapture();
        }
        #region Replay
        // Añade todos los datos de golpe (solo para replay)
        public void updateData(double[] x, double[] y, double[] z)
        {
            clear();
            replayModel.updateData(x, y, z);
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            replayModel.updateIndex(index);
        }
        #endregion Replay
        // Esta version funciona mejor pero usa mas memoria. Si se sobrepasa la memoria incial hay que modificar el tamaño de las arrays.

        // Actualiza los datos
        public async void updateData(double[] data)
        {
            clear();
            captureModel.updateData(data);
        }
        public async void updateData(Vector3[] data, bool render = true)
        {
            captureModel.updateData(data, render);
        }
        // Actualiza el renderizado
        public async void render()
        {
            plot.Render();
        }
        // Borra todos los puntos de todas las lineas
        public void clear()
        {
            plot.Plot.Clear(typeof(SignalPlot));
            plot.Render();
        }
        class CaptureModel
        {
            Model3S model;
            private const int CAPACITY = 200;
            double[] valuesX;
            double[] valuesY;
            double[] valuesZ;
            SignalPlot signalPlotX;
            SignalPlot signalPlotY;
            SignalPlot signalPlotZ;
            private int nextIndex = 0;

            public CaptureModel(Model3S model)
            {
                this.model = model;
            }
            public void initCapture()
            {
                valuesX = new double[CAPACITY];
                valuesY = new double[CAPACITY];
                valuesZ = new double[CAPACITY];
                signalPlotX = model.plot.Plot.AddSignal(valuesX, color: model.xColor, label: "X");
                signalPlotY = model.plot.Plot.AddSignal(valuesY, color: model.yColor, label: "Y");
                signalPlotZ = model.plot.Plot.AddSignal(valuesZ, color: model.zColor, label: "Z");
                signalPlotX.MarkerSize = 0;
                signalPlotY.MarkerSize = 0;
                signalPlotZ.MarkerSize = 0;
                nextIndex = 0;
                model.plot.Plot.SetAxisLimitsX(xMin: 0, xMax: CAPACITY);
            }
            public void updateData(Vector3[] data, bool render = true)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int index = (nextIndex + i) % CAPACITY;
                    valuesX[index] = data[i].X;
                    valuesY[index] = data[i].Y;
                    valuesZ[index] = data[i].Z;
                }
                signalPlotX.Label = "X= " + data[data.Length - 1].X.ToString("0.##");
                signalPlotY.Label = "Y= " + data[data.Length - 1].Y.ToString("0.##");
                signalPlotZ.Label = "Z= " + data[data.Length - 1].Z.ToString("0.##");
                nextIndex += data.Length;
                model.lineFrame.X = nextIndex % CAPACITY;
                if (render)
                {
                    model.plot.Render();
                }
            }
            public void updateData(double[] data)
            {
                valuesX[nextIndex] = data[0];
                valuesY[nextIndex] = data[1];
                valuesZ[nextIndex] = data[2];
                signalPlotX.Label = "X= " + data[0].ToString("0.##");
                signalPlotY.Label = "Y= " + data[1].ToString("0.##");
                signalPlotZ.Label = "Z= " + data[2].ToString("0.##");
                nextIndex++;
                model.lineFrame.X = nextIndex % CAPACITY;
                model.plot.Render();
            }
        }
        class ReplayModel
        {
            Model3S model;
            double[] valuesX;
            double[] valuesY;
            double[] valuesZ;
            SignalPlot signalPlotX;
            SignalPlot signalPlotY;
            SignalPlot signalPlotZ;
            public ReplayModel(Model3S model)
            {
                this.model = model;
            }
            public void updateData(double[] x, double[] y, double[] z)
            {
                valuesX = x;
                valuesY = y;
                valuesZ = z;
                signalPlotX = model.plot.Plot.AddSignal(valuesX, color: model.xColor, label: "X");
                signalPlotY = model.plot.Plot.AddSignal(valuesY, color: model.yColor, label: "Y");
                signalPlotZ = model.plot.Plot.AddSignal(valuesZ, color: model.zColor, label: "Z");
                signalPlotX.MarkerSize = 0;
                signalPlotY.MarkerSize = 0;
                signalPlotZ.MarkerSize = 0;
                maxRenderIndex = 0;
                model.plot.Plot.SetAxisLimitsX(xMin: 0, xMax: valuesX.Length);
            }
            public void updateIndex(int index)
            {
                index = Math.Min(index, valuesX.Length); //Por si acaso
                maxRenderIndex = index;

                signalPlotX.Label = "X= " + valuesX[index].ToString("0.##");
                signalPlotY.Label = "Y= " + valuesY[index].ToString("0.##");
                signalPlotZ.Label = "Z= " + valuesZ[index].ToString("0.##");

                model.plot.Render();
            }
            private int maxRenderIndex
            {
                get
                {
                    return (int)model.lineFrame.X;
                }
                set
                {
                    signalPlotX.MaxRenderIndex = value;
                    signalPlotY.MaxRenderIndex = value;
                    signalPlotZ.MaxRenderIndex = value;
                    model.lineFrame.X = value;
                }
            }
        }
    }
}