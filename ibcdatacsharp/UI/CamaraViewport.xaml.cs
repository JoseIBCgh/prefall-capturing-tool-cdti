using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace ibcdatacsharp.UI
{
    // Ventana que muestra la grabacion de la camara
    public partial class CamaraViewport : System.Windows.Window
    {
        private const int VIDEO_FPS = 30;

        private VideoCapture videoCapture;
        private VideoWriter videoWriter;

        private bool recordPaused = false;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private Task cameraTask;
        public CamaraViewport()
        {
            InitializeComponent();
        }
        // Empieza a grabar la camara
        public void initializeCamara(int index)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            cameraTask = captureCameraCallback();
        }
        private void closingTasks()
        {
            videoCapture.Release();
            cancellationTokenSource.Cancel();
            if (videoWriter != null)
            {
                videoWriter.Dispose();
            }
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            closingTasks();
            Close();
        }
        // Cierra la camara al cerrar la ventana
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            closingTasks();

            base.OnClosing(e);
        }
        // Ejecuta continuamente
        private async Task captureCameraCallback()
        {
            while (true)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                Mat frame = new Mat();
                videoCapture.Read(frame);
                if (!frame.Empty())
                {
                    await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                    {
                        imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
                    }
                    );
                    if(videoWriter != null && !recordPaused)
                    {
                        videoWriter.Write(frame);
                    }
                }
            }
        }
        // Empieza a grabar
        private void onRecord(object sender, RoutedEventArgs e)
        {
            string getPath()
            {
                DateTime now = DateTime.Now;
                string year = now.Year.ToString();
                string month = now.Month.ToString().PadLeft(2, '0');
                string day = now.Day.ToString().PadLeft(2, '0');
                string hour = now.Hour.ToString().PadLeft(2, '0');
                string minute = now.Minute.ToString().PadLeft(2, '0');
                string second = now.Second.ToString().PadLeft(2, '0');
                string milisecond = now.Millisecond.ToString().PadLeft(3, '0');
                string filename = year + month + day + '-' + hour + '-' + minute + '-' + second + '-' + milisecond + ".avi";
                string folder = "C:\\Temp";
                return folder + "\\" + filename;
            }
            string path = getPath();
            videoWriter = new VideoWriter(path, FourCC.MJPG, VIDEO_FPS, new OpenCvSharp.Size(videoCapture.FrameWidth, videoCapture.FrameHeight));
            pause.IsEnabled = true;
            record.IsEnabled = false;
        }
        // Pausa la grabacion
        private void onPause(object sender, RoutedEventArgs e)
        {
            if(videoWriter != null)
            {
                recordPaused = !recordPaused;
            }
        }
        // Acaba la grabacion
        private void onStop(object sender, RoutedEventArgs e)
        {
            if (videoWriter != null)
            {
                videoWriter.Dispose();
                videoWriter = null;
                if (recordPaused)
                {
                    recordPaused = false;
                    pause.IsChecked = false;
                }
                pause.IsEnabled = false;
                record.IsEnabled = true;
                record.IsChecked = false;
            }
        }
    }
}
