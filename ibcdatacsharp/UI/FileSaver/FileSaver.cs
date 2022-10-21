# define VIDEO_MULTIMEDIA_TIMER // Usar un multimedia timer para grabar el video
//# define VIDEO_BUFFER // Guardar los frames en memoria y guardarlos en disco al final

using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.Timer;
using ibcdatacsharp.UI.ToolBar.Enums;
using OpenCvSharp;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI.FileSaver
{
    internal class FileSaver
    {
        private const int FPS = 30;
        private const int RECORD_CSV_MS = 10;
        private const int RECORD_VIDEO_MS = 1000 / FPS;
        private const int FRAME_HEIGHT = 480;
        private const int FRAME_WIDTH = 640;
        private TimerMeasure? timerCsv;
#if VIDEO_MULTIMEDIA_TIMER
        private Timer.Timer? timerVideo;
#else
        private System.Windows.Threading.DispatcherTimer timerVideo;
#endif
        private CamaraViewport.CamaraViewport camaraViewport;
        private Device.Device device;
#if VIDEO_BUFFER
        private VideoBuffer videoBuffer;
#else
        private VideoWriter? videoWriter;
#endif
        private string? path;
        private string? csvFile;
        private bool recordCSV;
        private bool recordVideo;
        private const string csvHeader = @"DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT
            TIME	TIME	ACC_X	ACC_Y	ACC_Z	GYR_X	GYR_Y	GYR_Z	MAG_X	MAG_Y	MAG_Z
            FRAME_NUMBERS	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG
            ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL
        ITEM	0	0	x	x	x	x	x	x	x	x	x
";
        private StringBuilder? csvData;
        public FileSaver()
        {
            recordCSV = false;
            recordVideo = false;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.camaraViewport.Content == null)
            {
                mainWindow.camaraViewport.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    camaraViewport = mainWindow.camaraViewport.Content as CamaraViewport.CamaraViewport;
                };
            }
            else
            {
                camaraViewport = mainWindow.camaraViewport.Content as CamaraViewport.CamaraViewport;
            }
            device = mainWindow.device;
            mainWindow.virtualToolBar.saveEvent += onSaveInfo;
            mainWindow.virtualToolBar.recordEvent += onRecord;
#if VIDEO_BUFFER
            videoBuffer = new VideoBuffer(FRAME_WIDTH, FRAME_HEIGHT);
#endif
        }
        // Inicializa el timer para grabar CSV
        private void initTimerRecordCsv()
        {
            if (timerCsv == null)
            {
                timerCsv = new TimerMeasure();
                timerCsv.Mode = TimerMode.Periodic;
                timerCsv.Period = RECORD_CSV_MS;
                timerCsv.Tick += appendCSV;

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent += timerCsv.onPause;

                if (mainWindow.virtualToolBar.pauseState == PauseState.Play)
                {
                    timerCsv.Start();
                }
            }
        }
        private void onPauseVideo(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                timerVideo.Stop();
            }
            else if (pauseState == PauseState.Play)
            {
                timerVideo.Start();
            }
        }
        // Inicializa el timer para grabar video
        private void initTimerRecordVideo()
        {
            if (timerVideo == null)
            {
#if VIDEO_MULTIMEDIA_TIMER
                timerVideo = new Timer.Timer();
                timerVideo.Mode = TimerMode.Periodic;
                timerVideo.Period = RECORD_VIDEO_MS;
                timerVideo.Tick += appendVideo;

#else
                timerVideo = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Send);
                timerVideo.Tick += appendVideo;
                timerVideo.Interval = new TimeSpan(0, 0, 0, 0, RECORD_VIDEO_MS);

#endif
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent += onPauseVideo;

                if (mainWindow.virtualToolBar.pauseState == PauseState.Play)
                {
                    timerVideo.Start();
                }
            }
        }
        // Se llama cuando se empieza o termina de grabar
        public void onRecord(object sender, RecordState recordState)
        {
            if(recordState == RecordState.Recording)
            {
                initFiles();
            }
            else if(recordState == RecordState.RecordStopped)
            {
                stopRecording();
            }
        }
        // Acciones para terminar de grabar
        private async void stopRecording()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (recordCSV)
            {
                timerCsv.Dispose();
                mainWindow.virtualToolBar.pauseEvent -= timerCsv.onPause;
                timerCsv = null;
                await saveCsvFile();
                recordCSV = false;
            }
            if (recordVideo)
            {
#if VIDEO_MULTIMEDIA
                timerVideo.Dispose();
#endif
                mainWindow.virtualToolBar.pauseEvent -= onPauseVideo;
                timerVideo = null;
#if VIDEO_BUFFER
                videoBuffer.saveFrames();
#else
                videoWriter.Release();
                videoWriter = null;
#endif
                recordVideo = false;
            }
        }
        // inicializa los ficheros para guardar csv y video
        private void initFiles()
        {
            string fileName()
            {
                DateTime now = DateTime.Now;
                string year = now.Year.ToString();
                string month = now.Month.ToString().PadLeft(2, '0');
                string day = now.Day.ToString().PadLeft(2, '0');
                string hour = now.Hour.ToString().PadLeft(2, '0');
                string minute = now.Minute.ToString().PadLeft(2, '0');
                string second = now.Second.ToString().PadLeft(2, '0');
                string milisecond = now.Millisecond.ToString().PadLeft(3, '0');
                string filename = year + month + day + '-' + hour + '-' + minute + '-' + second + '-' + milisecond;
                return filename;
            }
            string baseFilename = fileName();
            if (recordCSV)
            {
                csvFile = baseFilename + ".csv";
                csvData = new StringBuilder();
                csvData.Append(csvHeader);
                initTimerRecordCsv();
            }
            if (recordVideo)
            {
                string videoFile = baseFilename + ".avi";
                string pathVideoFile = path + "\\" + videoFile;
#if VIDEO_BUFFER
                videoBuffer.initVideoWriter(pathVideoFile, FourCC.DIVX, FPS);
#else
                videoWriter = new VideoWriter(pathVideoFile, FourCC.DIVX, FPS, new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
#endif
                initTimerRecordVideo();
            }
        }
        // Añade una fila al csv
        private void appendCSV(object sender, FrameArgs frameArgs)
        {
            RawArgs rawArgs = device.rawData;
            //AngleArgs angleArgs = device.angleData;
            string newLine = "1 " + frameArgs.elapsed.ToString() + " " + frameArgs.frame.ToString() + " " +
                rawArgs.accelerometer[0].ToString() + " " + rawArgs.accelerometer[1].ToString() + " " + rawArgs.accelerometer[2].ToString() + " " +
                rawArgs.gyroscope[0].ToString() + " " + rawArgs.gyroscope[1].ToString() + " " + rawArgs.gyroscope[2].ToString() + " " +
                rawArgs.magnetometer[0].ToString() + " " + rawArgs.magnetometer[1].ToString() + " " + rawArgs.magnetometer[2].ToString() + "\n";
            csvData.Append(newLine);
        }

        // Añade un frame al video
        private void appendVideo(object sender, EventArgs e)
        {
#if VIDEO_BUFFER
            Mat frame = camaraViewport.getCurrentFrame();
            videoBuffer.addFrame(frame, resize:false);
#else
            if (videoWriter != null)
            {
                Mat frame = camaraViewport.getCurrentFrame();
                //Mat frameResized = frame.Resize(new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                if (videoWriter != null)
                {
                    Task.Run(() => { videoWriter.Write(frame); });
                }
            }
#endif
        }
        // Guarda el csv
        private async Task saveCsvFile()
        {
            string filePath = path + "\\" + csvFile;
            await File.WriteAllTextAsync(filePath, csvData.ToString());
        }
        // Se llama al seleccionar las opciones de grabacion
        public void onSaveInfo(object sender, SaveArgs args)
        {
            recordVideo = args.video;
            recordCSV = args.csv;
            path = args.directory;
        }
        // Se llama cuando se cierra la aplicacion. Para guardar lo grabado
        public void onCloseApplication()
        {
            stopRecording();
        }
    }
}
