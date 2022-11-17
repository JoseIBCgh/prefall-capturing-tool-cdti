﻿using ScottPlot;
using System;
using System.Drawing;

namespace ibcdatacsharp.UI.Graphs.GraphWindow
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int MAX_POINTS = 100;
        private int CAPACITY = 100000; //Usar un valor sufientemente grande para que en la mayoria de los casos no haya que cambiar el tamaño de los arrays
        private const int GROW_FACTOR = 2;
        
        double[] valuesX;
        double[] valuesY;
        double[] valuesZ;
        ScottPlot.Plottable.SignalPlot signalPlotX;
        ScottPlot.Plottable.SignalPlot signalPlotY;
        ScottPlot.Plottable.SignalPlot signalPlotZ;

        private int nextIndex = 0;
        private WpfPlot plot;

        private double minY;
        private double maxY;

        public Model(WpfPlot plot,double minY, double maxY, string title = "", string units = "")
        {
            this.minY = minY;
            this.maxY = maxY;
            valuesX = new double[CAPACITY];
            valuesY = new double[CAPACITY];
            valuesZ = new double[CAPACITY];
            this.plot = plot;
            signalPlotX = plot.Plot.AddSignal(valuesX, color:Color.Red);
            signalPlotY = plot.Plot.AddSignal(valuesY, color: Color.Green);
            signalPlotZ = plot.Plot.AddSignal(valuesZ, color: Color.Blue);
            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);
            //plot.Plot.Title(title);
            signalPlotX.MaxRenderIndex = nextIndex;
            signalPlotY.MaxRenderIndex = nextIndex;
            signalPlotZ.MaxRenderIndex = nextIndex;
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
            signalPlotX.MaxRenderIndex = 0;
            signalPlotY.MaxRenderIndex = 0;
            signalPlotZ.MaxRenderIndex = 0;
            plot.Plot.SetAxisLimitsX(xMin: 0, xMax: Math.Min(MAX_POINTS, valuesX.Length));
            plot.Plot.SetAxisLimitsY(yMin: minY, yMax: maxY);
            plot.Render();
        }
        // Cambia los datos a mostrar
        public void updateIndex(int index)
        {
            index = Math.Min(index, valuesX.Length); //Por si acaso
            signalPlotX.MaxRenderIndex = index;
            signalPlotY.MaxRenderIndex = index;
            signalPlotZ.MaxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
                xMax: Math.Max(index, Math.Min(MAX_POINTS, valuesX.Length)));
            plot.Render();
        }
        #endregion Replay
        // Esta version funciona mejor pero usa mas memoria. Si se sobrepasa la memoria incial hay que modificar el tamaño de las arrays.

        // Actualiza los datos
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
        // Actualiza el renderizado
        public void render()
        {
            int index = nextIndex - 1;
            signalPlotX.MaxRenderIndex = index;
            signalPlotY.MaxRenderIndex = index;
            signalPlotZ.MaxRenderIndex = index;
            plot.Plot.SetAxisLimits(xMin: Math.Max(0, index - MAX_POINTS),
                xMax: Math.Max(index, Math.Min(MAX_POINTS, valuesX.Length)));
            plot.Render();
        }
        // Borra todos los puntos de todas las lineas
        public void clear()
        {
            nextIndex = 0;
        }
    }
}