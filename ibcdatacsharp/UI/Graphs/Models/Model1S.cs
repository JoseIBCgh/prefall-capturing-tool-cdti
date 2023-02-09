using OpenCvSharp.Flann;
using ScottPlot;
using ScottPlot.Drawing.Colormaps;
using ScottPlot.Plottable;
using System;
using System.Drawing;
using System.Windows.Markup;
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace ibcdatacsharp.UI.Graphs.Models
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model1S
    {
        public delegate void ValueEventHandler(object sender, double value);
        public event ValueEventHandler valueEvent;
        public event ValueEventHandler offsetEvent;
        private const int RIGHT_SEPARATION = 20;
        public WpfPlot plot { get; private set; }

        private const double MIN_Y = -360;
        private const double MAX_Y = 360;

        private Color frameColor = Color.Black;
        public VLine lineFrame { get; private set; }
        private const float verticalLineWidth = 0.5f;

        public Color dataColor = Color.Red;

        private CaptureModel captureModel;
        private ReplayModel replayModel;
        public Model1S(WpfPlot plot, Color color)
        {
            captureModel = new CaptureModel(this);
            replayModel = new ReplayModel(this);
            dataColor = color;
            this.plot = plot;
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            paintAreas();

            lineFrame = plot.Plot.AddVerticalLine(0, color: frameColor, width: verticalLineWidth, LineStyle.Dash);
            plot.Plot.Style(Style.Seaborn);
            plot.Refresh();
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
            clear();
            captureModel.initCapture();
        }
        public void updateData(double[] data)
        {
            clear();
            replayModel.updateData(data);
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            replayModel.updateIndex(index);
        }
        // Añade un punto
        public void updateData(double data)
        {
            captureModel.updateData(data);
        }
        public void updateData(float[] data, bool render = true)
        {
            captureModel.updateData(data, render);
        }
        // Actualiza el renderizado
        public void render()
        {
            plot.Render();
            /*
            int index = nextIndex - 1;
            if (index < 0)
            {
                index = 0;
            }
            else if(index > CAPACITY - 1)
            {
                index = CAPACITY - 1;
            }
            maxRenderIndex = index;
            plot.Render();
            */
        }
        // Borra todos los puntos
        public void clear()
        {
            plot.Plot.Clear(typeof(SignalPlot));
            plot.Render();
        }
        public double _offset { get; private set; }
        public double offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                offsetEvent?.Invoke(this, value);
            }
        }
        public void invokeValue(double value)
        {
            valueEvent?.Invoke(this, value);
        }
        class CaptureModel
        {
            Model1S model;
            private const int CAPACITY = 200;
            double[] values;
            SignalPlot signalPlot;
            private int nextIndex = 0;

            public CaptureModel(Model1S model)
            {
                this.model = model;
                model.offsetEvent += (sender, value) =>
                {
                    if (signalPlot != null)
                    {
                        signalPlot.OffsetY = value;
                        model.plot.Render();
                        int index = nextIndex % CAPACITY;
                        model.invokeValue(values[index] + model.offset);
                    }
                };
            }
            public void initCapture()
            {
                values = new double[CAPACITY];
                signalPlot = model.plot.Plot.AddSignal(values, color: model.dataColor);
                signalPlot.OffsetY = model._offset;
                nextIndex = 0;
                model.plot.Plot.SetAxisLimitsX(xMin: 0, xMax: CAPACITY);
            }
            public void updateData(float[] data, bool render = true)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    int index = (nextIndex + i) % CAPACITY;
                    values[index] = data[i];
                }
                nextIndex += data.Length;
                model.lineFrame.X = nextIndex % CAPACITY;
                model.invokeValue(data[data.Length - 1] + model.offset);
                if (render)
                {
                    model.plot.Render();
                }
            }
            public void updateData(double data)
            {
                int index = nextIndex % CAPACITY;
                values[index] = data;
                nextIndex++;
                model.lineFrame.X = nextIndex % CAPACITY;
                model.invokeValue(data + model.offset);
                model.plot.Render();
            }
        }
        class ReplayModel
        {
            Model1S model;
            double[] values;
            SignalPlot signalPlot;
            public ReplayModel(Model1S model)
            {
                this.model = model;
                model.offsetEvent += (sender, value) =>
                {
                    if (signalPlot != null)
                    {
                        signalPlot.OffsetY = value;
                        model.plot.Render();
                        int index = Math.Max(0, maxRenderIndex - 1);
                        model.invokeValue(values[index] + model.offset);
                    }
                };
            }
            public void updateData(double[] data)
            {
                values = data;
                signalPlot = model.plot.Plot.AddSignal(values, color: model.dataColor);
                signalPlot.OffsetY = model.offset;

                maxRenderIndex = 0;
                model.plot.Plot.SetAxisLimitsX(xMin: 0, xMax: values.Length);
                model.plot.Render();
            }
            // Cambia los datos a mostrar
            public void updateIndex(int index)
            {
                index = Math.Min(index, values.Length); //Por si acaso
                maxRenderIndex = index;
                //plot.Plot.SetAxisLimitsX(xMin: Math.Max(0, index - MAX_POINTS),
                //    xMax: Math.Max(index + RIGHT_SEPARATION, Math.Min(MAX_POINTS, values.Length)));
                model.plot.Render();
                model.invokeValue(values[index] + model.offset);
            }
            private int maxRenderIndex
            {
                set
                {
                    signalPlot.MaxRenderIndex = value;
                    model.lineFrame.X = value;
                    //lineData.Y = values[value];
                }
                get
                {
                    return signalPlot.MaxRenderIndex;
                }
            }
        }
    }  
}