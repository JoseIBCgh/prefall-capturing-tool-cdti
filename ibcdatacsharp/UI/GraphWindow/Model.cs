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
    public class Model : INotifyPropertyChanged, IDisposable
    {
        private const int NUM_SERIES = 3;
        private bool disposed;
        private readonly Timer timer;
        private int frames = 0;
        private Queue<double[]> queue = new Queue<double[]>();
        private OxyColor[] colors = new OxyColor[] {OxyColors.Red, OxyColors.Green, OxyColors.Blue };

        public Model(double minY, double maxY, string titleY = "", string units = "")
        {
            timer = new Timer(OnTimerElapsed);
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

            RaisePropertyChanged("PlotModel");

        }
        // Hace que el modelo se actualize periodicamente
        public void Start()
        {
            timer.Change(1000, 20);
        }
        // Hace que el modelo se deje de actualizar
        public void Pause()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public int TotalNumberOfPoints { get; private set; }

        public PlotModel PlotModel { get; private set; }

        // Añade los nuevos puntos a la cola. Se consumiran periodicamente.
        public void Queue(double[] points)
        {
            queue.Enqueue(points);
        }
        // Handler para el timer
        private void OnTimerElapsed(object state)
        {
            lock (PlotModel.SyncRoot)
            {
                Update();
            }

            PlotModel.InvalidatePlot(true);
        }
        // Actualiza el modelo hasta que no quedan puntos en la cola 
        public void Update()
        {
            int n = 0;

            double[] points;
            while (queue.TryDequeue(out points))
            {
                for(int i = 0; i < NUM_SERIES; i++)
                {
                    var s = (LineSeries)PlotModel.Series[i];
                    s.Points.Add(new DataPoint(frames / 1000.0, points[i]));
                }
                frames++;
            }

            for (int i = 0; i < NUM_SERIES; i++)
            {
                var s = (LineSeries)PlotModel.Series[i];
                if (s.Points.Count > 50)
                {
                    s.Points.RemoveRange(0, s.Points.Count - 50);
                }
                n += s.Points.Count;
            }

            if (TotalNumberOfPoints != n)
            {
                TotalNumberOfPoints = n;
                RaisePropertyChanged("TotalNumberOfPoints");
            }
        }
        // Borra todos los puntos del modelo
        public void ClearData()
        {
            for (int i = 0; i < NUM_SERIES; i++)
            {
                var s = (LineSeries)PlotModel.Series[i];
                s.Points.Clear();
            }
            frames = 0;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.timer.Dispose();
                }
            }

            disposed = true;
        }
    }
}