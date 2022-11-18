﻿// Dejar sin comentar solo una
#define TASK
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
using ibcdatacsharp.UI.Timer;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using ibcdatacsharp.UI.Common;
using System.Windows.Media.Imaging;
using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.ToolBar;
using System.Windows.Navigation;
using ibcdatacsharp.UI.Graphs;
using System.Diagnostics;

namespace ibcdatacsharp.UI.CamaraViewport
{
    /// <summary>
    /// Lógica de interacción para CamaraViewport.xaml
    /// </summary>
// Task version

#if TASK
    public partial class CamaraViewport : Page
    {
        private TimeLine.TimeLine timeLine;
        private VideoCapture videoCapture;

        private CancellationTokenSource cancellationTokenSourceDisplay;
        private CancellationToken cancellationTokenDisplay;
        private Task displayTask;

        private Mat _currentFrame;
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
        private BitmapSource[] clipImages;
        private int lastIndexDisplay = -1;

        public CamaraViewport()
        {
            InitializeComponent();
            _currentFrame = getBlackImage();
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
        public void initReplay(Uri video)
        {
            //clipImages = frames;
            imgViewport.Visibility = Visibility.Collapsed;
            videoViewport.Visibility = Visibility.Visible;
            videoViewport.Source = video;
            videoViewport.LoadedBehavior = MediaState.Pause;
            videoViewport.ScrubbingEnabled = true;
            //videoViewport.Stop();
            timeLine.model.timeEvent -= onUpdateTimeLine;
            timeLine.model.timeEvent += onUpdateTimeLine;
        }
        public void onUpdateTimeLine(object sender, double time)
        {
            /*
            double timePerFrame = 1.0 / Config.VIDEO_FPS_SAVE;
            int index = (int)Math.Round(time / timePerFrame); //Round ???
            index = Math.Max(index, 0);
            index = Math.Min(index, clipImages.Length - 1);
            if(index != lastIndexDisplay)
            {
                BitmapSource image = clipImages[index];
                await Dispatcher.BeginInvoke(DispatcherPriority.Normal, () =>
                {
                    imgViewport.Source = image;
                }
                );
            }
            */
            Trace.WriteLine(time);
            videoViewport.Position = TimeSpan.FromSeconds(time);
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
            displayTask = displayCameraCallback();
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
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
                await Task.Delay(1000 / Config.VIDEO_FPS);
            }
        }
        // Cierra la camara y el video writer al cerrar la aplicacion
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
