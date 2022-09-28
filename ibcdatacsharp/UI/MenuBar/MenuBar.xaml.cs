using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ibcdatacsharp.UI.MenuBar
{
    /// <summary>
    /// Lógica de interacción para MenuBar.xaml
    /// </summary>
    public partial class MenuBar : Page
    {
        public MenuBar()
        {
            InitializeComponent();
        }
        // Cambia el icono del boton pause
        public void changePauseState(PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/pause-icon.png"));
                pause.Header = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/play-pause-icon.png"));
                pause.Header = "Play";
            }
        }
        // Cambia el icono del boton Record
        public void changeRecordState(RecordState recordState)
        {
            if (recordState == RecordState.RecordStopped)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/record-stop-icon.png"));
                record.Header = "Record Stopped";
            }
            else if (recordState == RecordState.Recording)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/record-recording-icon.png"));
                record.Header = "Recording...";
            }
        }
        // Muestra la ventana de la version
        private void showVersion(object sender, RoutedEventArgs e)
        {
            Version version = new Version();
            version.ShowDialog();
        }
    }
}
