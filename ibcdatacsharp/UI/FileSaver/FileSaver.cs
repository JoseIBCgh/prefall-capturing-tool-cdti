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
        private const int FPS = 25;
        private const int RECORD_CSV_MS = 10;
        private const int RECORD_VIDEO_MS = 1000 / FPS;
        private const int PAUSE_VIDEO_POOL_MS = 10;
        private const int FRAME_HEIGHT = 480;
        private const int FRAME_WIDTH = 640;
        private System.Timers.Timer timerCsv;
        private Stopwatch stopwatchCSV;
        private int frameCsv;
        private CancellationTokenSource cancellationTokenSourceVideo;
        private CancellationToken cancellationTokenVideo;
        private Task videoTask;

        private CamaraViewport.CamaraViewport camaraViewport;
        private VirtualToolBar virtualToolBar;
        private Device.Device device;
        private VideoWriter? videoWriter;

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
            videoTask = appendVideoCallback();
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
                cancellationTokenSourceVideo.Cancel();

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
                initRecordCsv();
            }
            if (recordVideo)
            {
                string videoFile = baseFilename + ".avi";
                string pathVideoFile = path + "\\" + videoFile;
                videoWriter = new VideoWriter(pathVideoFile, FourCC.DIVX, FPS, new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                cancellationTokenSourceVideo = new CancellationTokenSource();
                cancellationTokenVideo = cancellationTokenSourceVideo.Token;
                initRecordVideo();
            }
        }
        // Añade una fila al csv
        private void appendCSV()
        {
            RawArgs rawArgs = device.rawData;
            //AngleArgs angleArgs = device.angleData;
            double elapsed = stopwatchCSV.Elapsed.TotalSeconds;
            string newLine = "1 " + elapsed.ToString() + " " + frameCsv.ToString() + " " +
                rawArgs.accelerometer[0].ToString() + " " + rawArgs.accelerometer[1].ToString() + " " + rawArgs.accelerometer[2].ToString() + " " +
                rawArgs.gyroscope[0].ToString() + " " + rawArgs.gyroscope[1].ToString() + " " + rawArgs.gyroscope[2].ToString() + " " +
                rawArgs.magnetometer[0].ToString() + " " + rawArgs.magnetometer[1].ToString() + " " + rawArgs.magnetometer[2].ToString() + "\n";
            csvData.Append(newLine);
            frameCsv++;
        }

        private async Task appendVideoCallback()
        {
            Stopwatch stopwatch = new Stopwatch();
            while (true)
            {
                if (cancellationTokenVideo.IsCancellationRequested)
                {
                    videoWriter.Release();
                    videoWriter = null;
                    return;
                }
                if (virtualToolBar.pauseState == PauseState.Pause)
                {
                    await Task.Delay(PAUSE_VIDEO_POOL_MS);
                }
                else
                {
                    stopwatch.Restart();
                    if (videoWriter != null)
                    {
                        Mat frame = camaraViewport.getCurrentFrame();
                        Mat frameResized = frame.Resize(new OpenCvSharp.Size(FRAME_WIDTH, FRAME_HEIGHT));
                        if (videoWriter != null)
                        {
                            videoWriter.Write(frameResized);
                        }
                    }
                    double elapsed = stopwatch.Elapsed.TotalSeconds;
                    int waitTime = (int)(RECORD_VIDEO_MS - elapsed);
                    if (waitTime > 0)
                    {
                        await Task.Delay(waitTime);
                    }
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
                videoWriter.Dispose();
                videoWriter = null;
            }
        }
    }
}
