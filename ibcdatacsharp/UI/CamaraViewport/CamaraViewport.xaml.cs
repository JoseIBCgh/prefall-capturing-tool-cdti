using ibcdatacsharp.UI.ToolBar.Enums;
using OpenCvSharp.WpfExtensions;
using OpenCvSharp;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private Task cameraTask;
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
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;
            videoCapture = new VideoCapture(index, VideoCaptureAPIs.DSHOW);
            cameraTask = captureCameraCallback();
        }
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            if (videoCapture != null)
            {
                videoCapture.Release();
                cancellationTokenSource.Cancel();
            }
        }
        // Ejecuta continuamente
        private async Task captureCameraCallback()
        {
            while (true)
            {

                if (cancellationToken.IsCancellationRequested)
                {
                    videoCapture = null; //Si se hace en onClose puede dar errores de referencia
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
                    if (videoWriter != null && !recordPaused)
                    {
                        videoWriter.Write(frame);
                    }
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
            }
            if (recordState == RecordState.RecordStopped)
            {
                if (videoWriter != null)
                {
                    videoWriter.Dispose();
                    videoWriter = null;
                }
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
                videoWriter.Dispose();
            }
        }
    }
}
