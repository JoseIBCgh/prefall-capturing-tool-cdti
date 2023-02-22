using OpenCvSharp.Flann;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Input;
using System.Windows.Markup;

namespace ibcdatacsharp.UI.Graphs.Models
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class ModelSagital
    {
        public delegate void ValueEventHandler(object sender, double value);
        public event ValueEventHandler valueEvent;
        public event ValueEventHandler offsetEvent;
        private const int RIGHT_SEPARATION = 20;
        private const int MAX_POINTS = 100;
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;
        double[] values;
        SignalPlot signalPlot;
        private int nextIndex = 0;
        private WpfPlot plot;

        private const double MIN_Y = -180;
        private const double MAX_Y = 180;

        private Color frameColor = Color.Black;
        private VLine lineFrame;
        private const float verticalLineWidth = 0.5f;

        private Color zeroColor = Color.SkyBlue;
        private HLine lineZero;
        private const float zeroLineWidth = 0.5f;

        private Color dataColor = Color.Blue;

        private CaptureModel captureModel;
        private ReplayModel replayModel;
        public ModelSagital(WpfPlot plot, string upper = "", string lower = "")
        {
            captureModel = new CaptureModel(this);
            replayModel = new ReplayModel(this);
            this.plot = plot;
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);

            Annotation upperAnnotation = plot.Plot.AddAnnotation(upper, -10, 10);
            upperAnnotation.BackgroundColor = Color.Transparent;
            upperAnnotation.BorderColor = Color.Transparent;
            upperAnnotation.BorderWidth = 0;
            upperAnnotation.Shadow = false;
            Annotation lowerAnnotation = plot.Plot.AddAnnotation(lower, -10, -10);
            lowerAnnotation.BackgroundColor = Color.Transparent;
            lowerAnnotation.BorderColor = Color.Transparent;
            lowerAnnotation.BorderWidth = 0;
            lowerAnnotation.Shadow = false;

            lineFrame = plot.Plot.AddVerticalLine(0, color: frameColor, width: verticalLineWidth, LineStyle.Dash);
            lineZero = plot.Plot.AddHorizontalLine(0, color: zeroColor, width: zeroLineWidth, LineStyle.Dash);

            plot.Plot.Style(Style.Seaborn);
            plot.Refresh();
        }
        public void initCapture()
        {
            clear();
            captureModel.initCapture();
        }
        #region Replay
        // Añade todos los datos de golpe (solo para replay)
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
        #endregion Replay
        public void updateData(float[] data, bool render = true)
        {
            captureModel.updateData(data, render);
        }
        public void redrawData(float[] data)
        {
            captureModel.redrawData(data);
        }
        // Actualiza el renderizado
        public void render()
        {
            plot.Render();
        }
        // Borra todos los puntos
        public void clear(bool render = true)
        {
            plot.Plot.Clear(typeof(SignalPlot));
            if (render)
            {
                plot.Render();
            }
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
                offsetEvent?.Invoke(this, value);
            }
        }
        public void invokeValue(double value)
        {
            valueEvent?.Invoke(this, value);
        }
        public static int captureCapacity
        {
            get
            {
                return CaptureModel.CAPACITY;
            }
        }
        class CaptureModel
        {
            ModelSagital model;
            public const int CAPACITY = 200;
            double[] values;
            SignalPlot signalPlot;
            private int nextIndex = 0;

            public CaptureModel(ModelSagital model)
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
            public void redrawData(float[] data)
            {
                int init_index = nextIndex % CAPACITY - data.Length;
                Trace.WriteLine("init index = "+ init_index);
                if(init_index < 0)
                {
                    init_index += CAPACITY;
                }
                Trace.WriteLine("init index definitive = " + init_index);
                for (int i = 0; i < data.Length; i++)
                {
                    int index =  (init_index + i) % CAPACITY;
                    values[index] = data[i];
                }
                model.invokeValue(data[data.Length - 1] + model.offset);
                model.plot.Render();
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
            ModelSagital model;
            double[] values;
            SignalPlot signalPlot;
            public ReplayModel(ModelSagital model)
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