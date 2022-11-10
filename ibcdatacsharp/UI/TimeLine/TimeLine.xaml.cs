using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.TimeLine
{
    /// <summary>
    /// Lógica de interacción para TimeLine.xaml
    /// </summary>
    public partial class TimeLine : Page
    {
        private bool paused = true;
        private System.Timers.Timer timer;
        private Stopwatch stopwatch;
        private double deltaTime = 0;
        private const double PLAY_MS = 10;
        private const double UPDATE_TIME_MS = 50;

        public TimeLine()
        {
            InitializeComponent();
            model = new Model(timeLine, time, UPDATE_TIME_MS);
            DataContext = this;
            startPlay();
        }
        // Actualiza el timeline y el contador
        private void startPlay()
        {
            timer = new System.Timers.Timer();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            timer.Interval = PLAY_MS;
            timer.Elapsed += (sender, e) => tickPlay();
            timer.Start();
            deltaTime = 0;
            model.timeEvent -= onDrag;
            model.timeEvent += onDrag;
        }
        // Actualiza solo el contador
        private void startCapture()
        {
            timer = new System.Timers.Timer();
            stopwatch = new Stopwatch();
            stopwatch.Start();
            timer.Interval = UPDATE_TIME_MS;
            timer.Elapsed += (sender, e) => tickCapture();
            timer.Start();
            model.timeEvent -= onDrag;
        }
        public Model modelTime { get; private set; }
        public Model model { get; private set; }
        // Se ejecuta al pulsar el boton de pausa
        public void onPause(object sender, EventArgs e)
        {
            if (paused)
            {
                paused = false;
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/pause-icon.png"));
                timer.Start();
            }
            else
            {
                paused = true;
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/play-pause-icon.png"));
                timer.Stop();
            }
        }
        // Sirve para que el contador concuerde con el timeline (solo play)
        private void onDrag(object sender, double time)
        {
            deltaTime = time;
            stopwatch.Restart();
        }
        // Callback para actualizar contador y timeline
        public void tickPlay()
        {
            TimeSpan time = stopwatch.Elapsed;
            double totalTime = time.TotalSeconds + deltaTime;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
            {
                model.setTime(totalTime);
            });
        }
        // Callback para actualizar solo contador
        public void tickCapture()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
            {
                time.Text = model.formatTimer(stopwatch.Elapsed);
            });
        }
    }
}
