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
        private VideoCapture videoCapture;
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
        // Cierra la camara y la ventana
        private void onClose(object sender, RoutedEventArgs e)
        {
            videoCapture.Release();

            cancellationTokenSource.Cancel();

            Close();
        }
        // Cierra la camara al cerrar la ventana
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            videoCapture.Release();

            cancellationTokenSource.Cancel();

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
                if (frame.Empty())
                {
                    return;
                }
                await Dispatcher.BeginInvoke(DispatcherPriority.Background, () =>
                {
                    imgViewport.Source = BitmapSourceConverter.ToBitmapSource(frame);
                }
                );
            }
        }
        // Empieza a grabar
        private void onRecord(object sender, RoutedEventArgs e)
        {

        }
    }
}
