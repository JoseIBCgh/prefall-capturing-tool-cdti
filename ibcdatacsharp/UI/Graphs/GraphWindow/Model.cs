using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Drawing;

namespace ibcdatacsharp.UI.Graphs.GraphWindow
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
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

        private HSpan line;
        private double lineWidth = 0.5;
        public Model(WpfPlot plot,double minY, double maxY, string title = "", string units = "")
        {
            this.minY = minY;
            this.maxY = maxY;
            this.plot = plot;
            line = plot.Plot.AddHorizontalSpan(0 - lineWidth, 0 + lineWidth, Color.Red);
            plot.Plot.SetAxisLimitsX(xMin: 0, MAX_POINTS);


            plot.Plot.Style(Style.Seaborn);
            
            plot.Plot.XLabel("Frames");
            plot.Plot.YLabel("Data");

            plot.Refresh();
        }
        public void initCapture()
        {
            valuesX = new double[CAPACITY];
            valuesY = new double[CAPACITY];
            valuesZ = new double[CAPACITY];
            plot.Plot.Remove(signalPlotX);
            plot.Plot.Remove(signalPlotY);
            plot.Plot.Remove(signalPlotZ);
            signalPlotX = plot.Plot.AddSignal(valuesX, color: Color.Red);
            signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green);
            signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue);
            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);
            nextIndex = 0;
            maxRenderIndex = nextIndex;
            plot.Refresh();
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
            signalPlotX = plot.Plot.AddSignal(valuesX, color: Color.Red);
            signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green);
            signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue);
            maxRenderIndex = 0;
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax: Math.Min(MAX_POINTS, valuesX.Length));
            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);
            plot.Render();
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            index = Math.Min(index, valuesX.Length); //Por si acaso
            maxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
                xMax: Math.Max(index + RIGHT_SEPARATION, Math.Min(MAX_POINTS, valuesX.Length)));
            plot.Render();
        }
        #endregion Replay
        // Esta version funciona mejor pero usa mas memoria. Si se sobrepasa la memoria incial hay que modificar el tamaño de las arrays.

        // Actualiza los datos
        public async void updateData(double[] data)
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
        // Actualiza el renderizado
        public async void render()
        {
            int index = nextIndex - 1;
            if(index < 0)
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
            set
            {
                signalPlotX.MaxRenderIndex = value;
                signalPlotY.MaxRenderIndex = value;
                signalPlotZ.MaxRenderIndex = value;
                line.X1 = value - lineWidth;
                line.X2 = value + lineWidth;
            }
        }
    }
}