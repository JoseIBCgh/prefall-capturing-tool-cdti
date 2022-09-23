using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;

namespace ibcdatacsharp.UI.GraphWindow
{
    // Modelo de los graficos del acelerometro, giroscopio y magnetometro
    public class Model
    {
        private const int NUM_SERIES = 3;
        private OxyColor[] colors = new OxyColor[] {OxyColors.Red, OxyColors.Green, OxyColors.Blue };

        public Model(double minY, double maxY, string titleY = "", string units = "")
        {
            SetupModel(minY, maxY, titleY, units);
        }
        // Inicializa el modelo
        private void SetupModel(double minY, double maxY, string titleY, string units)
        {
            PlotModel = new PlotModel();
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = minY, Maximum = maxY, Title = titleY, Unit = units, FontSize = 10 , IntervalLength = 20 });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "frames", Unit = "kFreq", FontSize = 10, IntervalLength = 200});

            for (int i = 0; i < NUM_SERIES; i++)
            {
                PlotModel.Series.Add(new LineSeries { LineStyle = LineStyle.Solid, Color = colors[i] });
            }

        }

        public PlotModel PlotModel { get; private set; }

        // Borra todos los puntos del modelo
        public void ClearData()
        {
            for (int i = 0; i < NUM_SERIES; i++)
            {
                var s = (LineSeries)PlotModel.Series[i];
                s.Points.Clear();
            }
        }
    }
}