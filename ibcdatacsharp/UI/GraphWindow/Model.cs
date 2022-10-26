//#define MOVE_DATA

using ScottPlot;
using System;
using System.Drawing;

namespace ibcdatacsharp.UI.GraphWindow
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int MAX_POINTS = 100;
#if MOVE_DATA
        private const int EXTRA = 20;
        private const int CAPACITY = MAX_POINTS + EXTRA;
        readonly double[] valuesX;
        readonly double[] valuesY;
        readonly double[] valuesZ;
        readonly ScottPlot.Plottable.SignalPlot signalPlotX;
        readonly ScottPlot.Plottable.SignalPlot signalPlotY;
        readonly ScottPlot.Plottable.SignalPlot signalPlotZ;
#else
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;
        
        double[] valuesX;
        double[] valuesY;
        double[] valuesZ;
        ScottPlot.Plottable.SignalPlot signalPlotX;
        ScottPlot.Plottable.SignalPlot signalPlotY;
        ScottPlot.Plottable.SignalPlot signalPlotZ;
#endif

        private int nextIndex = 0;
        private WpfPlot plot;

        public Model(WpfPlot plot,double minY, double maxY, string titleY = "", string units = "")
        {
            valuesX = new double[CAPACITY];
            valuesY = new double[CAPACITY];
            valuesZ = new double[CAPACITY];
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
#if MOVE_DATA
        public void updateData(double[] data)
        {
            if(nextIndex >= CAPACITY) //No deberia de pasar
            {
                moveData();
            }
            valuesX[nextIndex] = data[0];
            valuesY[nextIndex] = data[1];
            valuesZ[nextIndex] = data[2];
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
                    valuesX[index_replaced] = valuesX[index_replacement];
                    valuesY[index_replaced] = valuesY[index_replacement];
                    valuesZ[index_replaced] = valuesZ[index_replacement];
                }
                nextIndex = MAX_POINTS;
            }
        }
        public void render()
        {
            if (nextIndex <= MAX_POINTS)
            {
                int index = nextIndex - 1;
                signalPlotX.MaxRenderIndex = index;
                signalPlotY.MaxRenderIndex = index;
                signalPlotZ.MaxRenderIndex = index;
                plot.Plot.SetAxisLimits(xMin: 0, xMax: index);
            }
            else
            {
                moveData();
            }
            plot.Render();
        }
#else
        // Esta version funciona mejor pero usa mas memoria. Si se sobrepasa la memoria incial hay que modificar el tamaño de las arrays.
        public void updateData(double[] data)
        {
            if(nextIndex >= CAPACITY) // No deberia pasar
            {
                CAPACITY = CAPACITY * GROW_FACTOR;
                Array.Resize(ref valuesX, CAPACITY);
                Array.Resize(ref valuesY, CAPACITY);
                Array.Resize(ref valuesZ, CAPACITY);
                plot.Plot.Remove(signalPlotX);
                plot.Plot.Remove(signalPlotY);
                plot.Plot.Remove(signalPlotZ);
                signalPlotX = plot.Plot.AddSignal(valuesX, color: Color.Red);
                signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green);
                signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue);
            }
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
#endif
        // Borra todos los puntos de todas las lineas
        public void clear()
        {
            nextIndex = 0;
        }
    }
}