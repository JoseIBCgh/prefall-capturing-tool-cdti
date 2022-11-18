using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.ToolBar.Enums;
using OpenCvSharp;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI.FileSaver
{
    internal class FileSaver
    {
        private const int RECORD_CSV_MS = 10;
        private const int RECORD_VIDEO_MS = 1000 / Config.VIDEO_FPS_SAVE;
        private System.Timers.Timer timerCsv;
        private Stopwatch stopwatchCSV;
        private int frameCsv;
        private System.Timers.Timer timerVideo;

        private CamaraViewport.CamaraViewport camaraViewport;
        private TimeLine.TimeLine timeLine;
        private VirtualToolBar virtualToolBar;
        private Device.Device device;
        private VideoWriter? videoWriter;

        private string? path;
        private string? csvFile;
        private bool recordCSV;
        private bool recordVideo;
        private StringBuilder? csvData;
        public FileSaver()
        {
            recordCSV = false;
            recordVideo = false;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.initialized += (sender, args) => finishInit();
        }
        // Para solucionar problemas de dependencias
        private void finishInit()
        {
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
            virtualToolBar = mainWindow.virtualToolBar;
            device = mainWindow.device;
            mainWindow.virtualToolBar.saveEvent += onSaveInfo;
            mainWindow.virtualToolBar.recordEvent += onRecord;
        }
        private void onPauseCsv(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                timerCsv.Stop();
                stopwatchCSV.Stop();
            }
            else if (pauseState == PauseState.Play)
            {
                timerCsv.Start();
                stopwatchCSV.Start();
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
        // Inicializa el timer para grabar CSV
        private void initRecordCsv()
        {
            timerCsv = new System.Timers.Timer();
            timerCsv.Interval = RECORD_CSV_MS;
            timerCsv.Elapsed += (sender, e) => appendCSV();

            virtualToolBar.pauseEvent += onPauseCsv;

            if(stopwatchCSV == null)
            {
                stopwatchCSV = new Stopwatch();
            }
            if (virtualToolBar.pauseState == PauseState.Play)
            {
                timerCsv.Start();
                stopwatchCSV.Restart();
            }
            frameCsv = 0;
        }
        // Inicializa el timer para grabar video
        private void initRecordVideo()
        {
            timerVideo = new System.Timers.Timer();
            timerVideo.Interval = RECORD_VIDEO_MS;
            timerVideo.Elapsed += (sender, e) => appendVideo();

            virtualToolBar.pauseEvent += onPauseVideo;

            if (virtualToolBar.pauseState == PauseState.Play)
            {
                timerVideo.Start();
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
        private void stopRecording()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (recordCSV)
            {
                timerCsv.Stop();
                timerCsv = null;
                mainWindow.virtualToolBar.pauseEvent -= onPauseCsv;
                saveCsvFile();
                recordCSV = false;
            }
            if (recordVideo)
            { 
                timerVideo.Stop();
                timerVideo = null;
                mainWindow.virtualToolBar.pauseEvent -= onPauseVideo;
                videoWriter.Release();
                videoWriter = null;

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
                csvData.Append(Config.csvHeader);
                initRecordCsv();
            }
            if (recordVideo)
            {
                string videoFile = baseFilename + ".avi";
                string pathVideoFile = path + "\\" + videoFile;
                videoWriter = new VideoWriter(pathVideoFile, FourCC.DIVX, Config.VIDEO_FPS_SAVE, new OpenCvSharp.Size(Config.FRAME_WIDTH, Config.FRAME_HEIGHT));
                initRecordVideo();
            }
        }
        // Añade una fila al csv
        private void appendCSV()
        {
            RawArgs rawArgs = device.rawData;
            //AngleArgs angleArgs = device.angleData;
            double elapsed = timeLine.elapsed;
            string newLine = "1 " + elapsed.ToString() + " " + frameCsv.ToString() + " " +
                rawArgs.accelerometer[0].ToString() + " " + rawArgs.accelerometer[1].ToString() + " " + rawArgs.accelerometer[2].ToString() + " " +
                rawArgs.gyroscope[0].ToString() + " " + rawArgs.gyroscope[1].ToString() + " " + rawArgs.gyroscope[2].ToString() + " " +
                rawArgs.magnetometer[0].ToString() + " " + rawArgs.magnetometer[1].ToString() + " " + rawArgs.magnetometer[2].ToString() + "\n";
            csvData.Append(newLine);
            frameCsv++;
        }

        private void appendVideo()
        {
            if (videoWriter != null)
            {
                Mat frame = camaraViewport.currentFrame;
                Mat frameResized = frame.Resize(new OpenCvSharp.Size(Config.FRAME_WIDTH, Config.FRAME_HEIGHT));
                if (videoWriter != null)
                {
                    videoWriter.Write(frameResized);
                }
            }
        }
        // Guarda el csv
        private async void saveCsvFile()
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
            if (recordCSV)
            {
                timerCsv.Stop();
                saveCsvFile();
            }
            if (recordVideo)
            {
                timerVideo.Stop();
                videoWriter.Dispose();
                videoWriter = null;
            }
        }
    }
}
