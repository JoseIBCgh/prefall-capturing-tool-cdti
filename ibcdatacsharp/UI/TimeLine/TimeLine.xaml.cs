using System;
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
        private DispatcherTimer timer;
        public TimeLine()
        {
            InitializeComponent();
            modelTime = new Model(0, 1000);
            modelTime.currentFrameEvent += onCurrentFrameMove;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(increaseFrame);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 10);

            DataContext = this;
        }
        public Model modelTime { get; private set; }
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
        // Mientras se mueve la linea manualmente detiene el timer
        private void onCurrentFrameMove(object sender, CurrentFrameArgs e)
        {
            if (e.start && !paused)
            {
                timer.Stop();
            }
            else if(e.end && !paused)
            {
                timer.Start();
            }
        }
        // Funcion periodica que avanza al siguiente frame
        public async void increaseFrame(object sender, EventArgs e)
        {
            await Task.Run(async () => await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
            {
                modelTime.increaseFrame();
            }));
        }
    }
}
