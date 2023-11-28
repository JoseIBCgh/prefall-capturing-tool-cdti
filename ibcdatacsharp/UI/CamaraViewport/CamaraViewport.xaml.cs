// Dejar sin comentar solo una
#define TASK // Solo esta version esta actualizada
//#define THREAD
//#define TIMER // no funciona bien

using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Navigation;
using System.Diagnostics;

namespace ibcdatacsharp.UI.CamaraViewport
{

    // Task version

#if TASK
    /// <summary>
    /// Vista de la camara. Tiene 3 versiones con tareas, threads y timers. La version con tareas funciona mejor.
    /// </summary>
    public partial class CamaraViewport : Page
    {
        private TimeLine.TimeLine timeLine;
        private VideoCapture videoCapture;

        private CancellationTokenSource cancellationTokenSourceDisplay;
        private CancellationToken cancellationTokenDisplay;
        private Task displayTask;

        /// <summary>
        /// Evento que se tiene que llamar cuando se cambia la camara abierta
        /// </summary>
        public event EventHandler cameraChanged;

        private Mat _currentFrame;
        /// <summary>
        /// fps de la camara abierta
        /// </summary>
        public int fps { get; private set; }
        /// <summary>
        /// Frame que se muestra en el viewport
        /// </summary>
        public Mat currentFrame
        {
            get
            {
                lock (_currentFrame)
                {
                    return _currentFrame;
                }
            }
            set
            {
                lock (_currentFrame)
                {
                    _currentFrame = value;
                }
            }
        }
        public CamaraViewport()
        {
            InitializeComponent();
            _currentFrame = getBlackImage(); // Acceder directamente porque no estaba inicializado (Error sino)
            imgViewport.Source = BitmapSourceConverter.ToBitmapSource(currentFrame);
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.timeLine.Content == null)
            {
                mainWindow.timeLine.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                };
            }
            else
            {
                timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
            }
        }
        /// <summary>
        /// Inicia la reproduccion de un video
        /// </summary>
        /// <param name="path">path de fichero de video</param>
        public void initReplay(string path)
        {
            endCameraTask(); // Dejar de usar la camara
            imgViewport.Visibility = Visibility.Collapsed;
            videoViewport.Visibility = Visibility.Visible;
            videoViewport.Source = new Uri(path);
            videoViewport.LoadedBehavior = MediaState.Pause;
            videoViewport.ScrubbingEnabled = true;
            timeLine.model.timeEvent -= onUpdateTimeLine;
            timeLine.model.timeEvent += onUpdateTimeLine;
        }
        /// <summary>
        /// Termina la reproduccion de un fichero. Limpia las variables
        /// </summary>
        private void clearReplay()
        {
            if (videoViewport.Source != null)
            {
                imgViewport.Visibility = Visibility.Visible;
                videoViewport.Visibility = Visibility.Collapsed;
                videoViewport.Source = null;
                timeLine.model.timeEvent -= onUpdateTimeLine;
            }
        }
        /// <summary>
        /// Handler que se llama cuando se actualiza el timeline. Actualiza la posicion del video.
        /// </summary>
        /// <param name="sender">Objeto que ha enviado el evento</param>
        /// <param name="time">instanse de tiempo a actualizar</param>
        public void onUpdateTimeLine(object sender, double time)
        {
            videoViewport.Position = TimeSpan.FromSeconds(time);
        }
        /// <summary>
        /// Comprueba si hay alguna camara grabando
        /// </summary>
        public bool someCameraOpened()
        {
            return videoCapture != null;
        }
        // Pantalla en negro cuando no se graba
        private Mat getBlackImage()
        {
            MatType matType = (MatType)(Config.MAT_TYPE == null?Config.DEFAULT_MAT_TYPE:Config.MAT_TYPE);
            Mat frame = new Mat(Config.FRAME_HEIGHT, Config.FRAME_WIDTH, matType);
            return frame;
        }
        /// <summary>
        /// inicia el streaming de una camara.
        /// </summary>
        /// <param name="index">Indice OpenCV de la API DSHOW de la camara</param>
        /// <param name="fps">fps a stremear</param>
        public async void initializeCamara(int index, int fps)
        {
            this.fps = fps;
            Trace.WriteLine("fps selected = " + fps);
            // Quitar la imagen de la grabacion anterior
            currentFrame = getBlackImage();
            imgViewport.Source = BitmapSourceConverter.ToBitmapSource(currentFrame);
            
            clearReplay();

            cancellationTokenSourceDisplay = new CancellationTokenSource();
            cancellationTokenDisplay = cancellationTokenSourceDisplay.Token;
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            videoCapture.Set(VideoCaptureProperties.Fps, this.fps);
            cameraChanged?.Invoke(this, EventArgs.Empty);
            await Task.Run(() => displayCameraCallback());
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            endCameraTask();
        }
        private void endCameraTask()
        {
            if (videoCapture != null)
            {
                cancellationTokenSourceDisplay.Cancel();
            }
        }

        // Actualiza la imagen
        private async Task displayCameraCallback()
        {
            while (true)
            {
                if (cancellationTokenDisplay.IsCancellationRequested)
                {
                    videoCapture.Release();
                    videoCapture = null;
                    currentFrame = getBlackImage();
                    await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                    {
                        imgViewport.Source = BitmapSourceConverter.ToBitmapSource(getBlackImage());
                    });
                    cameraChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }
                //Mat frame = new Mat();
                videoCapture.Read(currentFrame);
                if (!currentFrame.Empty())
                {
                    //currentFrame = frame;
                    await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                    {
                        imgViewport.Source = BitmapSourceConverter.ToBitmapSource(currentFrame);
                    }
                    );
                }
                //await Task.Delay(1000 / fps);
            }
        }
        /// <summary>
        /// Funcion que se tiene que llamar cuando se va a cerrar la aplicacion. Libera la camara que este stremeando.
        /// </summary>
        public void onCloseApplication()
        {
            if(videoCapture != null)
            {
                videoCapture.Release();
            }
        }
    }
#endif
#if THREAD
    public partial class CamaraViewport : Page
    {
        private VideoCapture videoCapture;

        private CancellationTokenSource cancellationTokenSourceDisplay;
        private CancellationToken cancellationTokenDisplay;
        private Thread displayThread;

        public CamaraViewport()
        {
            InitializeComponent();
            imgViewport.Source = BitmapSourceConverter.ToBitmapSource(getBlackImage());
        }
        // Comprueba si se esta grabano alguna camara
        public bool someCameraOpened()
        {
            return videoCapture != null;
        }
        // Pantalla en negro cuando no se graba
        private Mat getBlackImage()
        {
            Mat frame = new Mat(Config.FRAME_HEIGHT, Config.FRAME_WIDTH, MatType.CV_32F);
            return frame;
        }
        // Empieza a grabar la camara
        public void initializeCamara(int index)
        {
            cancellationTokenSourceDisplay = new CancellationTokenSource();
            cancellationTokenDisplay = cancellationTokenSourceDisplay.Token;
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            displayThread = new Thread(displayCameraCallback);
            displayThread.IsBackground = true;
            displayThread.Start();
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            if (videoCapture != null)
            {
                cancellationTokenSourceDisplay.Cancel();
            }
        }
        // Devuelve el frame de la camara o negro si no esta encendida
        public Mat getCurrentFrame()
        {
            if(videoCapture != null)
            {
                Mat frame = new Mat();
                videoCapture.Read(frame);
                if (!frame.Empty())
                {
                    Mat frameResized = frame.Resize(new OpenCvSharp.Size(Config.FRAME_WIDTH, Config.FRAME_HEIGHT));
                    return frameResized;
                }
                else
                {
                    return getBlackImage();
                }
            }
            else
            {
                return getBlackImage();
            }
        }
        // Actualiza la imagen
        private void displayCameraCallback()
        {
            while (true)
            {
                if (cancellationTokenDisplay.IsCancellationRequested)
                {
                    videoCapture.Release();
                    videoCapture = null;
                    Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
                    {
                        imgViewport.Source = BitmapSourceConverter.ToBitmapSource(getBlackImage());
                    });
                    return;
                }
                Mat frame = new Mat();
                videoCapture.Read(frame);
                if (!frame.Empty())
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
                    {
                        imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
                    }
                    );
                }
                Thread.Sleep(1000 / Config.VIDEO_FPS);
            }
        }
        // Cierra la camara y el video writer al cerrar la aplicacion
        public void onCloseApplication()
        {
            if (videoCapture != null)
            {
                videoCapture.Release();
            }
        }
    }
#endif
#if TIMER
    public partial class CamaraViewport : Page
    {
        public const int FRAME_HEIGHT = 480;
        public const int FRAME_WIDTH = 640;
        private const int VIDEO_FPS = 30;

        private VideoCapture videoCapture;

        private CancellationTokenSource cancellationTokenSourceDisplay;
        private CancellationToken cancellationTokenDisplay;
        private Thread displayThread;
        private Timer.Timer timer;

        public CamaraViewport()
        {
            InitializeComponent();
            imgViewport.Source = BitmapSourceConverter.ToBitmapSource(getBlackImage());
        }
        // Comprueba si se esta grabano alguna camara
        public bool someCameraOpened()
        {
            return videoCapture != null;
        }
        // Pantalla en negro cuando no se graba
        private Mat getBlackImage()
        {
            Mat frame = new Mat(FRAME_HEIGHT, FRAME_WIDTH, MatType.CV_32F);
            return frame;
        }
        // Empieza a grabar la camara
        public void initializeCamara(int index)
        {
            cancellationTokenSourceDisplay = new CancellationTokenSource();
            cancellationTokenDisplay = cancellationTokenSourceDisplay.Token;
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            timer = new Timer.Timer();
            timer.Mode = TimerMode.Periodic;
            timer.Period = 1000 / VIDEO_FPS;
            timer.Tick += displayCameraCallback;
            timer.Start();
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            if (videoCapture != null)
            {
                timer.Dispose();
                timer = null;
                videoCapture.Release();
                videoCapture = null;
                Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
                {
                    imgViewport.Source = BitmapSourceConverter.ToBitmapSource(getBlackImage());
                });
            }
        }
        // Devuelve el frame de la camara o negro si no esta encendida
        public Mat getCurrentFrame()
        {
            if (videoCapture != null)
            {
                Mat frame = new Mat();
                videoCapture.Read(frame);
                if (!frame.Empty())
                {
                    Mat frameResized = frame.Resize(new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                    return frameResized;
                }
                else
                {
                    return getBlackImage();
                }
            }
            else
            {
                return getBlackImage();
            }
        }
        // Actualiza la imagen
        private void displayCameraCallback(object sender, EventArgs e)
        {
            Mat frame = new Mat();
            videoCapture.Read(frame);
            if (!frame.Empty())
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Render, () =>
                {
                    imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
                }
                );
            }
        }
        // Cierra la camara y el video writer al cerrar la aplicacion
        public void onCloseApplication()
        {
            if (videoCapture != null)
            {
                timer.Dispose();
                videoCapture.Release();
            }
        }
    }
#endif
}
