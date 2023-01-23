﻿using OpenCvSharp.Flann;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Drawing;
using System.Windows.Markup;

namespace ibcdatacsharp.UI.SagitalAngles
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
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

        private const double MIN_Y = -180;
        private const double MAX_Y = 180;

        private Color frameColor = Color.Black;
        private VLine lineFrame;
        private const float verticalLineWidth = 0.5f;

        private Color zeroColor = Color.SkyBlue;
        private HLine lineZero;
        private const float zeroLineWidth = 0.5f;

        private Color dataColor = Color.Blue;
        public Model(WpfPlot plot, string upper = "", string lower = "")
        {
            this.plot = plot;
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax:MAX_POINTS);

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

            initCapture();

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
            values = new double[CAPACITY];
            plot.Plot.SetAxisLimitsY(yMin: MIN_Y, yMax: MAX_Y);
            plot.Plot.XAxis2.SetSizeLimit(max: 5);
            plot.Plot.Remove(signalPlot);
            signalPlot = plot.Plot.AddSignal(values, color: dataColor);
            signalPlot.OffsetY = _offset;
            nextIndex = 0;
            maxRenderIndex = nextIndex;
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