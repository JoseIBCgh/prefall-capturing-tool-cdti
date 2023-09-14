﻿using ibcdatacsharp.UI.Device;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.ToolBar.Enums;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ibcdatacsharp.Login;

namespace ibcdatacsharp.UI.FileSaver
{
    public class FileSaver
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
        private DeviceList.DeviceList deviceList;

        private string? path;
        private string? csvFile;
        private string? videoFile;
        private bool recordCSV;
        private bool recordVideo;
        private StringBuilder? csvData = new StringBuilder();
        private int? id = null;

        public delegate void filesAddedEvent(object sender, int id, List<string> files);
        public event filesAddedEvent filesAdded;
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
            if (mainWindow.deviceList.Content == null)
            {
                mainWindow.deviceList.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
                };
            }
            else
            {
                deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
            }
            virtualToolBar = mainWindow.virtualToolBar;
            device = mainWindow.device;
            mainWindow.virtualToolBar.saveEvent += onSaveInfo;
            mainWindow.virtualToolBar.stopEvent += onStopRecording;
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

            //Eliminar estas líneas para grabar manualmente
            //timerCsv.Interval = RECORD_CSV_MS;
            //timerCsv.Elapsed += (sender, e) => appendCSV();

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
            timerVideo.Interval = 1000 / camaraViewport.fps;
            timerVideo.Elapsed += (sender, e) => appendVideo();

            virtualToolBar.pauseEvent += onPauseVideo;

            if (virtualToolBar.pauseState == PauseState.Play)
            {
                timerVideo.Start();
            }
        }
        // Acciones para terminar de grabar
        private void onStopRecording(object sender)
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            string message = "";
            bool show = false;
            List<string> files = new List<string>();
            if (recordCSV)
            {
                //timerCsv.Stop();
                //timerCsv = null;
                //mainWindow.virtualToolBar.pauseEvent -= onPauseCsv;
                saveCsvFile();
                recordCSV = false;
                message += "Csv grabado en " + csvFile + ".\n";
                show = true;
                files.Add(path + Path.DirectorySeparatorChar + csvFile);
            }
            if (recordVideo)
            {
                timerVideo.Stop();
                timerVideo = null;
                mainWindow.virtualToolBar.pauseEvent -= onPauseVideo;
                videoWriter.Release();
                videoWriter = null;

                recordVideo = false;
                message += "Video grabado en " + videoFile + ".\n";
                show=true;
                files.Add(path + Path.DirectorySeparatorChar + videoFile);
            }
            if (show)
            {
                MessageBox.Show(message, caption: "Info", button: MessageBoxButton.OK, icon: MessageBoxImage.Information);
                filesAdded?.Invoke(this, id.Value, files);
            }
        }
        private string fileName()
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
        // inicializa los ficheros para guardar csv y video
        private void initFiles()
        {
            string baseFilename = fileName();
            if (recordCSV)
            {
                csvFile = baseFilename + ".txt";
                csvData = new StringBuilder();
                id = LoginInfo.selectedPacienteId;
                var jsonObject = new
                {
                    id = LoginInfo.selectedPacienteId
                };
                string jsonString = JsonConvert.SerializeObject(jsonObject);
                csvData.Append(jsonString);
                switch (deviceList.numIMUsUsed)
                {
                    case 0:
                        MessageBox.Show("You didn't select any IMU. This is for debug porpuses only", "Debug Mode", MessageBoxButton.OK, MessageBoxImage.Warning);
                        break;
                    case 1:
                        csvData.Append(Config.csvHeader1IMU);
                        break;
                    case 2:
                        csvData.Append(Config.csvHeader2IMUs);
                        break;
                    case 4:
                        csvData.Append(Config.csvHeaderSagital);
                        break;
                    default:
                        throw new Exception("try to record with " + deviceList.numIMUsUsed.ToString() + " IMUs");
                }
                //initRecordCsv();
            }
            if (recordVideo)
            {
                videoFile = baseFilename + ".avi";
                string pathVideoFile = path + Path.DirectorySeparatorChar + videoFile;
                videoWriter = new VideoWriter(pathVideoFile, FourCC.DIVX, camaraViewport.fps, new OpenCvSharp.Size(Config.FRAME_WIDTH, Config.FRAME_HEIGHT));
                initRecordVideo();
            }
        }

        //Añadde fila en un csv de forma manual

        public void appendCSVManual(string dataline)
        {
           csvData.Append(dataline);
           
        }

        // Añade una fila al csv
        private void appendCSV()
        {
            RawArgs rawArgs = device.rawData;
            //AngleArgs angleArgs = device.angleData;
            double elapsed = timeLine.elapsed; // Usar tiempo del timeLine para sincronizar.
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
                if(Config.MAT_TYPE == null)
                {
                    Config.MAT_TYPE = Config.DEFAULT_MAT_TYPE;
                }
                if(frame.Type() != Config.MAT_TYPE)
                {
                    frame.ConvertTo(frame, (MatType)Config.MAT_TYPE);
                }
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
            string filePath = path + Path.DirectorySeparatorChar + csvFile;
            await File.WriteAllTextAsync(filePath, csvData.ToString());
        }
        // Se llama al seleccionar las opciones de grabacion
        public void onSaveInfo(object sender, SaveArgs args)
        {
            recordVideo = args.video;
            recordCSV = args.csv;
            path = args.directory;
            initFiles();
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
        public void saveFakeFileSagital(string path, string filename)
        {
            float randomAngle()
            {
                Random random = new Random();
                float n = random.NextSingle();
                return n * 360 - 180;
            }
            csvData = new StringBuilder();
            csvData.Append(Config.csvHeaderSagital);
            int numFrames = 1000;
            for(int i = 0; i < numFrames; i++)
            {
                string line = "1 " + (0.01 * i).ToString("F2") + " " + (i).ToString() + " " +
                                randomAngle().ToString("F2") + " " + randomAngle().ToString("F2") + " " +
                                randomAngle().ToString("F2") + "\n";
                csvData.Append(line);
            }
            string filePath = path + Path.DirectorySeparatorChar + filename;
            File.WriteAllTextAsync(filePath, csvData.ToString());
        }
    }
}
