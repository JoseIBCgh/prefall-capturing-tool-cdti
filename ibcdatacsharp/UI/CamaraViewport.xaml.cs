using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace ibcdatacsharp.UI
{
    // Ventana que muestra la grabacion de la camara
    public partial class CamaraViewport : System.Windows.Window
    {
        private VideoCapture videoCapture;
        private Mat frame = new Mat();
        DispatcherTimer timer;
        public CamaraViewport()
        {
            InitializeComponent();
        }
        // Empieza a grabar la camara
        public void initializeCamara(int index)
        {
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            //double fps = videoCapture.Fps;
            double fps = 30;
            int sleepTime = (int)Math.Round(1000 / fps);
            timer = new DispatcherTimer();
            timer.Tick += updateImage;
            timer.Interval = TimeSpan.FromMilliseconds(sleepTime);
            timer.Start();
        }
        // Actualiza un frame
        private void updateImage(object sender, EventArgs e)
        {
            if (videoCapture.Read(frame))
            {
                imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
            }
        }
        // Cierra la camara al cerrar la ventana
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            videoCapture.Release();

            base.OnClosing(e);
        }
    }
}
