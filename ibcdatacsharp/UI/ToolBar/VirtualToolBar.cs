using ibcdatacsharp.UI.FileSaver;
using ibcdatacsharp.UI.ToolBar.Enums;
using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Navigation;
using Windows.Media.Editing;
using Windows.Storage;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Windows.UI.Composition;
using System.Collections.Generic;
using Windows.Graphics.Imaging;
using ibcdatacsharp.UI.Graphs;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using ibcdatacsharp.UI.Common;
using OpenCvSharp.WpfExtensions;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ibcdatacsharp.UI.ToolBar
{
    // Mantiene el estado para la ToolBar y la MenuBar
    public class VirtualToolBar
    {
        public PauseState pauseState { get; set; } 
        public RecordState recordState { get; set; }

        private ToolBar toolBar;
        private MenuBar.MenuBar menuBar;
        private CamaraViewport.CamaraViewport camaraViewport;
        private TimeLine.TimeLine timeLine;

        private SavingMenu saveMenu;
        private GraphManager graphManager;

        public delegate void PauseEventHandler(object sender, PauseState args);
        public delegate void RecordEventHandler(object sender, RecordState args);
        public delegate void StopEventHandler(object sender);
        public event PauseEventHandler pauseEvent;
        // Se laza cuando se empieza o termina de grabar
        public event RecordEventHandler recordEvent;
        public event StopEventHandler stopEvent;
        // Se lanza cuando se configuran los ficheros de grabar
        public event EventHandler<SaveArgs> saveEvent;

        private bool buttonsEnabled = false;

        public VirtualToolBar()
        {
            pauseState = PauseState.Play;
            recordState = RecordState.RecordStopped;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.initialized += (sender, args) => finishInit();
        }
        // Para solucionar problemas de dependencias
        private void finishInit()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            graphManager = mainWindow.graphManager;
            
            if (mainWindow.toolBar.Content == null)
            {
                mainWindow.toolBar.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    toolBar = mainWindow.toolBar.Content as ToolBar;
                    if (toolBar.savingMenu.Content == null)
                    {
                        toolBar.savingMenu.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            saveMenu = toolBar.savingMenu.Content as SavingMenu;
                        };
                    }
                    else
                    {
                        saveMenu = toolBar.savingMenu.Content as SavingMenu;
                    }
                };
            }
            else
            {
                toolBar = mainWindow.toolBar.Content as ToolBar;
                if (toolBar.savingMenu.Content == null)
                {
                    toolBar.savingMenu.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        saveMenu = toolBar.savingMenu.Content as SavingMenu;
                    };
                }
                else
                {
                    saveMenu = toolBar.savingMenu.Content as SavingMenu;
                }
            }
            if (mainWindow.menuBar.Content == null)
            {
                mainWindow.menuBar.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    menuBar = mainWindow.menuBar.Content as MenuBar.MenuBar;
                };
            }
            else
            {
                menuBar = mainWindow.menuBar.Content as MenuBar.MenuBar;
            }
            if(mainWindow.camaraViewport.Content == null)
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
        }
        public void onScanClick()
        {
            if (!buttonsEnabled)
            {
                toolBar.activateButtons();
                menuBar.activateButtons();
                buttonsEnabled = true;
            }
        }
        // Se ejecuta al clicar el boton pause
        public void pauseClick()
        {
            if(pauseState == PauseState.Play)
            {
                pauseState = PauseState.Pause;
                toolBar.changePauseState(PauseState.Play);
                menuBar.changePauseState(PauseState.Play);
                if(pauseEvent != null)
                {
                    pauseEvent?.Invoke(this, PauseState.Pause);
                }
            }
            else if(pauseState == PauseState.Pause)
            {
                pauseState = PauseState.Play;
                toolBar.changePauseState(PauseState.Pause);
                menuBar.changePauseState(PauseState.Pause);
                if (pauseEvent != null)
                {
                    pauseEvent?.Invoke(this, PauseState.Play);
                }
            }
        }
        // Se ejecuta al clicar record
        public void recordClick()
        {
            if(recordState == RecordState.RecordStopped)
            {
                toolBar.savingMenu.Visibility = Visibility.Visible;
                saveMenu.ok.Click += continueRecord;
            }
        }
        private void continueRecord(object sender, RoutedEventArgs e)
        {
            if(saveEvent != null)
            {
                saveEvent?.Invoke(this, new SaveArgs { directory = saveMenu.route.Text, csv = (bool)saveMenu.csv.IsChecked, video = (bool)saveMenu.video.IsChecked });
            }
            saveMenu.ok.Click -= continueRecord;
            toolBar.savingMenu.Visibility = Visibility.Collapsed;
            //((MainWindow)Application.Current.MainWindow).Focus();
            recordState = RecordState.Recording;
            toolBar.changeRecordState(RecordState.Recording);
            menuBar.changeRecordState(RecordState.Recording);
            if (recordEvent != null)
            {
                recordEvent?.Invoke(this, RecordState.Recording);
            }
        }
        // Se ejecuta al clicar stop
        public void stopClick()
        {
            if(pauseState == PauseState.Pause)
            {
                pauseState = PauseState.Play;
                toolBar.changePauseState(PauseState.Pause);
                menuBar.changePauseState(PauseState.Pause);
            }
            if(recordState == RecordState.Recording)
            {
                recordState = RecordState.RecordStopped;
                toolBar.changeRecordState(RecordState.RecordStopped);
                menuBar.changeRecordState(RecordState.RecordStopped);
                if(recordEvent != null)
                {
                    recordEvent?.Invoke(this, RecordState.RecordStopped);
                }
            }
            if(stopEvent != null)
            {
                stopEvent?.Invoke(this);
            }
        }
        // Abre los ficheros (csv y avi)
        public async void openClick()
        {
            async Task<BitmapSource[]> extractVideo(string filename)
            {
                IEnumerable<TimeSpan> getTimespans(double videoDuration)
                {
                    List<TimeSpan> timeSpans = new List<TimeSpan>();
                    double timePerFrame = 1.0 / Config.VIDEO_FPS_SAVE;
                    double time = 0;
                    while(time < videoDuration)
                    {
                        timeSpans.Add(TimeSpan.FromSeconds(time));
                        time += timePerFrame;
                    }
                    return timeSpans;
                }
                StorageFile file = await StorageFile.GetFileFromPathAsync(filename);
                MediaClip clip = await MediaClip.CreateFromFileAsync(file);
                MediaComposition composition = new MediaComposition();
                composition.Clips.Add(clip);
                IReadOnlyList<ImageStream> frames = await composition.GetThumbnailsAsync(
                    getTimespans(clip.OriginalDuration.TotalSeconds), Config.FRAME_WIDTH, Config.FRAME_HEIGHT, 
                    VideoFramePrecision.NearestFrame);
                BitmapSource[] framesConverted = new BitmapSource[frames.Count];
                for (int i = 0; i < frames.Count; i++)
                {
                    framesConverted[i] = Helpers.Bitmap2BitmapImage(Helpers.UWP2WPF(frames[i]));
                }
                return framesConverted;
            }
            GraphData extractCSV(string filename)
            {
                using (var reader = new StreamReader(filename))
                {
                    List<FrameData> data = new List<FrameData>();
                    int linesToSkip = Config.csvHeader.Split('\n').Length - 1; //Hay un salto de linea al final del header
                    Trace.WriteLine("linesToSkip", linesToSkip.ToString());
                    for(int i = 0; i < linesToSkip; i++)
                    {
                        reader.ReadLine();
                    }
                    while (!reader.EndOfStream)
                    {
                        string line = reader.ReadLine();
                        data.Add(new FrameData(line));
                    }
                    return new GraphData(data);
                }
            }
            void setTimeLineLimits(GraphData csvData, BitmapSource[] videoData)
            {
                // Funcion para obtener la longitud del timeLine (se puede cambiar)
                double resultLength(double csvLength, double videoLength)
                {
                    return Math.Max(csvLength, videoLength);
                }
                double csvLength = csvData.maxTime;
                double videoLength = (double)videoData.Length / Config.VIDEO_FPS_SAVE;
                timeLine.model.updateLimits(0, resultLength(csvLength, videoLength));
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.Filter = "Allowed Files(*.txt;*.csv;*.avi)|*.txt;*.csv;*.avi";
            if(openFileDialog.ShowDialog() == true)
            {
                string[] files = openFileDialog.FileNames;
                if(files.Length == 2)
                {
                    BitmapSource[] videoData;
                    GraphData csvData;
                    string file1 = files[0];
                    if (Path.GetExtension(file1) == ".avi")
                    {
                        string file2 = files[1];
                        if(Path.GetExtension(file2) == ".csv" || Path.GetExtension(file2) == ".txt")
                        {
                            videoData = await extractVideo(file1);
                            csvData = extractCSV(file2);
                            setTimeLineLimits(csvData, videoData);
                            graphManager.initReplay(csvData);
                            camaraViewport.initReplay(videoData);
                            timeLine.tickPlay();
                            MessageBox.Show("Ficheros " + file1 + " " + file2 + "cargados.");
                        }
                        else
                        {
                            MessageBox.Show("El fichero de datos tiene que ser .csv o .txt", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else if(Path.GetExtension(file1) == ".csv" || Path.GetExtension(file1) == ".txt")
                    {
                        string file2 = files[1];
                        if (Path.GetExtension(file2) == ".avi")
                        {
                            videoData = await extractVideo(file2);
                            csvData = extractCSV(file1);
                            setTimeLineLimits(csvData, videoData);
                            graphManager.initReplay(csvData);
                            camaraViewport.initReplay(videoData);
                            timeLine.tickPlay();
                            MessageBox.Show("Ficheros " + file1 + " " + file2 + " cargados.");
                        }
                        else
                        {
                            MessageBox.Show("El fichero de video tiene que ser .avi", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("El fichero de datos tiene que ser .csv o .txt y el fichero de video tiene que ser .avi", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if(files.Length == 1)
                {
                    string file = files[0];
                    string extension = Path.GetExtension(file);
                    if(extension == ".avi")
                    {
                        BitmapSource[] videoData = await extractVideo(file);
                        timeLine.model.updateLimits(0, (double)videoData.Length / Config.VIDEO_FPS_SAVE);
                        camaraViewport.initReplay(videoData);
                        timeLine.tickPlay();
                        MessageBox.Show("Fichero " + file + " cargado.");
                    }
                    else if(extension == ".csv" || extension == ".txt")
                    {
                        GraphData csvData = extractCSV(file);
                        timeLine.model.updateLimits(0, csvData.maxTime);
                        graphManager.initReplay(csvData);
                        timeLine.tickPlay();
                        MessageBox.Show("Fichero " + file + " cargado.");
                    }
                    else
                    {
                        MessageBox.Show("Solo se admiten ficheros .csv, .txt o .avi", "Error de formato", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Selecciona 1 o 2 ficheros", "Error de numero de ficheros", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
