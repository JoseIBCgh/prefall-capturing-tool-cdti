using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using ScottPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace ibcdatacsharp.UI.GraphWindow
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int MAX_POINTS = 100;
        private const int CAPACITY = 100000;
        readonly double[] valuesX = new double[CAPACITY];
        readonly double[] valuesY = new double[CAPACITY];
        readonly double[] valuesZ = new double[CAPACITY];
        readonly ScottPlot.Plottable.SignalPlot signalPlotX;
        readonly ScottPlot.Plottable.SignalPlot signalPlotY;
        readonly ScottPlot.Plottable.SignalPlot signalPlotZ;
        private int nextIndex = 0;
        private WpfPlot plot;

        public Model(WpfPlot plot,double minY, double maxY, string titleY = "", string units = "")
        {
            this.plot = plot;
            signalPlotX = plot.Plot.AddSignal(valuesX, color:Color.Red);
            signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green);
            signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue);
            SetupModel(minY, maxY, titleY, units);
            signalPlotX.MaxRenderIndex = nextIndex;
            signalPlotY.MaxRenderIndex = nextIndex;
            signalPlotZ.MaxRenderIndex = nextIndex;
            plot.Refresh();
        }
        // Inicializa el modelo
        private void SetupModel(double minY, double maxY, string titleY, string units)
        {
            plot.Plot.SetAxisLimits(yMin: minY, yMax:maxY);
            //PlotModel = new PlotModel();
            //PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minY, Maximum = maxY, Title = titleY, Unit = units, FontSize = 10 , IntervalLength = 20 });
            //PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "frames", Unit = "kFreq", FontSize = 10, IntervalLength = 200});
        }
        // Añade un punto por linea
        /* No funciona
        public void update(int frame, double[] data)
        {
            if (nextIndex < MAX_POINTS)
            {
                valuesX[nextIndex] = data[0];
                valuesY[nextIndex] = data[1];
                valuesZ[nextIndex] = data[2];
                signalPlotX.MaxRenderIndex = nextIndex;
                signalPlotY.MaxRenderIndex = nextIndex;
                signalPlotZ.MaxRenderIndex = nextIndex;
                plot.Plot.SetAxisLimits(xMin: Math.Max(0, nextIndex - MAX_POINTS), xMax: nextIndex);
            }
            else
            {
                for(int i = MAX_POINTS - 2; i >= 0; i--)
                {
                    int index_replacement = i + 1;
                    int index_replaced = i;
                    valuesX[index_replaced] = valuesX[index_replacement];
                    valuesY[index_replaced] = valuesY[index_replacement];
                    valuesZ[index_replaced] = valuesZ[index_replacement];
                }
                int last_index = MAX_POINTS - 1;
                valuesX[last_index] = data[0];
                valuesY[last_index] = data[1];
                valuesZ[last_index] = data[2];
            }
            plot.Render();
            nextIndex++;
        }
*/
        public void updateData(double[] data)
        {
            valuesX[nextIndex] = data[0];
            valuesY[nextIndex] = data[1];
            valuesZ[nextIndex] = data[2];
            nextIndex++;
        }
        public void render()
        {
            int index = nextIndex - 1;
            signalPlotX.MaxRenderIndex = index;
            signalPlotY.MaxRenderIndex = index;
            signalPlotZ.MaxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS), xMax: index);
            plot.Render();
        }
        // Borra todos los puntos de todas las lineas
        public void clear()
        {
            nextIndex = 0;
        }
    }
}