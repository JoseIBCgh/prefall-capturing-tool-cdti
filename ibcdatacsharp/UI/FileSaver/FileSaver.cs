//# define VIDEO_MULTIMEDIA_TIMER

using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.Timer;
using ibcdatacsharp.UI.ToolBar.Enums;
using OpenCvSharp;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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
        private Timer.Timer timerRecordCsv;
#if VIDEO_MULTIMEDIA_TIMER
        private Timer.Timer timerRecordVideo;
#else
        private System.Windows.Threading.DispatcherTimer timerRecordVideo;
#endif
        private CamaraViewport.CamaraViewport camaraViewport;
        private Device.Device device;
        private VideoWriter videoWriter;
        private string path;
        private string csvFile;
        private bool recordCSV;
        private bool recordVideo;
        private const string csvHeader = @"DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT	DEFAULT
            TIME	TIME	ACC_X	ACC_Y	ACC_Z	GYR_X	GYR_Y	GYR_Z	MAG_X	MAG_Y	MAG_Z
            FRAME_NUMBERS	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG	ANALOG
            ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL	ORIGINAL
        ITEM	0	0	x	x	x	x	x	x	x	x	x
";
        private StringBuilder csvData;
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
        }
        // Inicializa el timer para grabar CSV
        private void initTimerRecordCsv()
        {
            if (timerRecordCsv == null)
            {
                timerRecordCsv = new Timer.Timer();
                timerRecordCsv.Mode = TimerMode.Periodic;
                timerRecordCsv.Period = RECORD_CSV_MS;
                timerRecordCsv.Tick += appendCSV;

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent += timerRecordCsv.onPause;
            }
            timerRecordCsv.Start();
        }
        // Inicializa el timer para grabar video
        private void initTimerRecordVideo()
        {
#if VIDEO_MULTIMEDIA_TIMER
            if (timerRecordVideo == null)
            {
                timerRecordVideo = new Timer.Timer();
                timerRecordVideo.Mode = TimerMode.Periodic;
                timerRecordVideo.Period = RECORD_VIDEO_MS;
                timerRecordVideo.Tick += appendVideo;

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent += timerRecordVideo.onPause;
            }
#else
            if (timerRecordVideo == null)
            {
                timerRecordVideo = new System.Windows.Threading.DispatcherTimer(System.Windows.Threading.DispatcherPriority.Send);
                timerRecordVideo.Tick += appendVideo;
                timerRecordVideo.Interval = new TimeSpan(0, 0, 0, 0, RECORD_VIDEO_MS);

                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent += delegate (object sender, PauseState pauseState)
                {
                    if (timerRecordVideo != null)
                    {
                        if (pauseState == PauseState.Pause)
                        {
                            timerRecordVideo.Stop();
                        }
                        else if (pauseState == PauseState.Play)
                        {
                            timerRecordVideo.Start();
                        }
                    }
                };
            }
#endif
            timerRecordVideo.Start();
        }
        // Se llama cuando se empieza o termina de grabar
        public async void onRecord(object sender, RecordState recordState)
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
        private async void stopRecording()
        {
            if (recordCSV)
            {
                timerRecordCsv.stopAndReset(this);
                await saveCsvFile();
            }
            if (recordVideo)
            {
#if VIDEO_MULTIMEDIA_TIMER
                timerRecordVideo.stopAndReset(this);
#else
                timerRecordVideo.Stop();
#endif
                if (videoWriter != null)
                {
                    videoWriter.Release();
                    videoWriter = null;
                }
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
                videoWriter = new VideoWriter(pathVideoFile, FourCC.MJPG, FPS, new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
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
#if VIDEO_MULTIMEDIA_TIMER
        // Añade un frame al video
        private void appendVideo(object sender, FrameArgs frameArgs)
        {
            if (videoWriter != null)
            {
                Mat frame = camaraViewport.getCurrentFrame();
                Mat frameResized = frame.Resize(new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                if (videoWriter != null)
                {
                    videoWriter.Write(frameResized);
                }
            }
        }
#else
        // Añade un frame al video
        private void appendVideo(object sender, EventArgs e)
        {
            if (videoWriter != null)
            {
                Mat frame = camaraViewport.getCurrentFrame();
                Mat frameResized = frame.Resize(new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                if (videoWriter != null)
                {
                    videoWriter.Write(frameResized);
                }
            }
        }
#endif
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
