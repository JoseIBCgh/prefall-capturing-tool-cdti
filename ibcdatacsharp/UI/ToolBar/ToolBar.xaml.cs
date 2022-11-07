using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ibcdatacsharp.UI.ToolBar
{
    /// <summary>
    /// Lógica de interacción para ToolBar.xaml
    /// </summary>
    public partial class ToolBar : Page
    {
        public ToolBar()
        {
            InitializeComponent();
            deactivateButtons();
        }
        private void deactivateButtons()
        {
            connect.IsEnabled = false;
            disconnect.IsEnabled = false;
            openCamera.IsEnabled = false;
            record.IsEnabled = false;
            capture.IsEnabled = false;
            pause.IsEnabled = false;
            stop.IsEnabled = false;
            capturedFiles.IsEnabled = false;
        }
        public void activateButtons()
        {
            connect.IsEnabled = true;
            disconnect.IsEnabled = true;
            openCamera.IsEnabled = true;
            record.IsEnabled = true;
            capture.IsEnabled = true;
            pause.IsEnabled = true;
            stop.IsEnabled = true;
            capturedFiles.IsEnabled = true;
        }
        // Cambia el icono del boton Pause
        public void changePauseState(PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/pause-blue-icon.png"));
                pauseText.Text = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/play-pause-blue-icon.png"));
                pauseText.Text = "Play";
            }
        }
        // Cambia el icono del boton Record
        public void changeRecordState(RecordState recordState)
        {
            if (recordState == RecordState.RecordStopped)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/Blue/record-stop-blue-icon.png"));
                recordText.Text = "Record Stopped";
            }
            else if (recordState == RecordState.Recording)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/record-recording-icon.png"));
                recordText.Text = "Recording...";
            }
        }
    }
}
