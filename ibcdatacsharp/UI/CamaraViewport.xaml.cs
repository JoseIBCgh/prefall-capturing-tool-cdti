using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace ibcdatacsharp.UI
{
    // Ventana que muestra la grabacion de la camara
    public partial class CamaraViewport : System.Windows.Window
    {
        private VideoCapture videoCapture;
        private DispatcherTimer timer;

        private Thread cameraThread;
        private bool useThread = true;
        public CamaraViewport()
        {
            InitializeComponent();
        }
        // Empieza a grabar la camara
        public void initializeCamara(int index)
        {
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            if (useThread)
            {
                cameraThread = new Thread(new ThreadStart(captureCameraCallback));
                cameraThread.Start();
            }
            else
            {
                //double fps = videoCapture.Fps;
                double fps = 30;
                int sleepTime = (int)Math.Round(1000 / fps);
                timer = new DispatcherTimer();
                timer.Tick += updateImage;
                timer.Interval = TimeSpan.FromMilliseconds(sleepTime);
                timer.Start();
            }
        }
        // Actualiza un frame
        private void updateImage(object sender, EventArgs e)
        {
            Mat frame = new Mat();
            if (videoCapture.Read(frame))
            {
                imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
            }
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            videoCapture.Release();

            if (cameraThread != null)
            {
                cameraThread.Interrupt();
            }

            Close();
        }
        // Cierra la camara al cerrar la ventana
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            videoCapture.Release();

            if(cameraThread != null)
            {
                cameraThread.Interrupt();
            }

            base.OnClosing(e);
        }
        // Ejecuta continuamente
        private void captureCameraCallback()
        {
            while (true)
            {
                Mat frame = new Mat();
                videoCapture.Read(frame);
                if (frame.Empty())
                {
                    return;
                }
                Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
                }
                );
            }
        }
    }
}
