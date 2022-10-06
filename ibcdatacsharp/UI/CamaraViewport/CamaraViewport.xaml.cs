using ibcdatacsharp.UI.ToolBar.Enums;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Diagnostics;
using static System.Math;

namespace ibcdatacsharp.UI.CamaraViewport
{
    /// <summary>
    /// Lógica de interacción para CamaraViewport.xaml
    /// </summary>
    public partial class CamaraViewport : Page
    {
        private const int FRAME_HEIGHT = 480;
        private const int FRAME_WIDTH = 640;
        private const int VIDEO_FPS = 30;

        private VideoCapture videoCapture;
        private VideoWriter videoWriter;

        private bool recordPaused;

        private CancellationTokenSource cancellationTokenSourceDisplay;
        private CancellationToken cancellationTokenDisplay;
        private CancellationTokenSource cancellationTokenSourceRecord;
        private CancellationToken cancellationTokenRecord;
        private Task displayTask;
        private Task recordTask;

        public CamaraViewport()
        {
            InitializeComponent();
            setBlackImage();
        }
        // Comprueba si se esta grabano alguna camara
        public bool someCameraOpened()
        {
            return videoCapture != null;
        }
        // Pantalla en negro cuando no se graba
        private void setBlackImage()
        {
            Mat frame = new Mat(FRAME_HEIGHT, FRAME_WIDTH, MatType.CV_32F);
            imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
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
                    setBlackImage();
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
                }
                await Task.Delay(1000 / VIDEO_FPS);
            }
        }
        // Guarda el video
        private async Task recordCameraCallBack()
        {
            while (true)
            {
                if (cancellationTokenRecord.IsCancellationRequested)
                {
                    videoWriter.Release();
                    videoWriter = null;
                    return;
                }
                if (recordPaused)
                {
                    await Task.Delay(1000 / VIDEO_FPS);
                }
                else
                {
                    if (videoCapture != null)
                    {
                        Mat frame = new Mat();
                        videoCapture.Read(frame);
                        if (!frame.Empty())
                        {
                            Mat frameResized = frame.Resize(new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                            videoWriter.Write(frameResized);
                        }
                    }
                    await Task.Delay(1000 / VIDEO_FPS);
                }
            }
        }
        // Se ejecuta al cambiar el estado del boton pause
        public void onPause(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                recordPaused = true;
            }
            else if (pauseState == PauseState.Play)
            {
                recordPaused = false;
            }
        }
        // Se ejecuta al cambiar el estado del boton record
        public void onRecord(object sender, RecordState recordState)
        {
            // Empieza a grabar
            void startRecord()
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
                videoWriter = new VideoWriter(path, FourCC.MJPG, VIDEO_FPS, new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                cancellationTokenSourceRecord = new CancellationTokenSource();
                cancellationTokenRecord = cancellationTokenSourceRecord.Token;
                recordTask = recordCameraCallBack();
            }
            if (recordState == RecordState.RecordStopped)
            {
                cancellationTokenSourceRecord.Cancel();
            }
            else if (recordState == RecordState.Recording)
            {
                startRecord();
            }
        }
        // Cierra la camara y el video writer al cerrar la aplicacion
        public void onCloseApplication()
        {
            if(videoCapture != null)
            {
                videoCapture.Release();
            }
            if(videoWriter != null)
            {
                videoWriter.Release();
            }
        }
    }
}
