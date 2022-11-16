using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.TimeLine
{
    /// <summary>
    /// Lógica de interacción para TimeLine.xaml
    /// </summary>
    public enum TimerMode
    {
        CAPTURE,
        PLAY
    }
    public partial class TimeLine : Page
    {
        private TimerMode timerMode;
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
        }
        // Actualiza el timeline y el contador
        private void startPlay(object sender, EventArgs e)
        {
            // Condicion para permitir inicializar el contador y timeline
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                stopwatch = new Stopwatch();
                stopwatch.Start();
                timer.Interval = PLAY_MS;
                timer.Elapsed += tickPlay;
                timer.Start();
                deltaTime = model.time;
                paused = false;
                // Quitar primero el evento para que solo se añada una vez
                model.dragEvent -= onDrag;
                model.dragEvent += onDrag;
                start.Click -= moveToStart;
                start.Click += moveToStart;
                end.Click -= moveToEnd;
                end.Click += moveToEnd;
                pause.Click -= onPause;
                pause.Click += onPause;
                timerMode = TimerMode.PLAY;
            }
        }
        // Actualiza solo el contador
        public void startCapture()
        {
            // Condicion para permitir inicializar el contador
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                stopwatch = new Stopwatch();
                stopwatch.Start();
                timer.Interval = UPDATE_TIME_MS;
                timer.Elapsed += tickCapture;
                timer.Start();
                // Quitar los eventos si estaban
                model.dragEvent -= onDrag;
                start.Click -= moveToStart;
                end.Click -= moveToEnd;
                pause.Click -= onPause;
                timerMode = TimerMode.CAPTURE;
            }
        }
        public Model model { get; private set; }
        // Mueve la barra al principio (modo play)
        public void moveToStart(object sender, EventArgs e)
        {
            if (timerMode == TimerMode.PLAY)
            {
                model.moveToStart();
            }
        }
        // Mueve la barra al final(modo play)
        public void moveToEnd(object sender, EventArgs e)
        {
            if (timerMode == TimerMode.PLAY)
            {
                model.moveToEnd();
            }
        }
        // Pausa el timer (modo capture)
        public void Pause()
        {
            if (timerMode == TimerMode.CAPTURE)
            {
                paused = true;
            }
        }
        // Inicia el timer(modo capture)
        public void Start()
        {
            if (timerMode == TimerMode.CAPTURE)
            {
                paused = false;
            }
        }
        // Detiene el timer (modo capture)
        public void Stop()
        {
            if (timerMode == TimerMode.CAPTURE)
            {
                timer.Elapsed -= tickCapture;
                timer.Dispose();
                timer = null;
                stopwatch = null;
            }
        }
        // Se ejecuta al pulsar el boton de pausa
        private void onPause(object sender, EventArgs e)
        {
            paused = !paused;
        }
        // Sirve para que el contador concuerde con el timeline (solo play)
        private void onDrag(object sender, double time)
        {
            if (timerMode == TimerMode.PLAY)
            {
                deltaTime = time;
                if (stopwatch.IsRunning)
                {
                    stopwatch.Restart();
                }
                else
                {
                    stopwatch.Reset();
                }
            }
        }
        // Callback para actualizar contador y timeline
        public void tickPlay(object sender, ElapsedEventArgs e)
        {
            TimeSpan time = stopwatch.Elapsed;
            double totalTime = time.TotalSeconds + deltaTime;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
            {
                model.setTime(totalTime);
            });
        }
        // Callback para actualizar solo contador
        public void tickCapture(object sender, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
            {
                time.Text = model.formatTimer(stopwatch.Elapsed);
            });
        }
        // No acceder directamente
        private bool _paused;
        private bool paused
        {
            get
            {
                return _paused;
            }
            set
            {
                _paused = value;
                if (_paused)
                {
                    pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/play-pause-icon.png"));
                    timer.Stop();
                    stopwatch.Stop();
                }
                else
                {
                    pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/pause-icon.png"));
                    timer.Start();
                    stopwatch.Start();
                }
            }
        }
    }
}
