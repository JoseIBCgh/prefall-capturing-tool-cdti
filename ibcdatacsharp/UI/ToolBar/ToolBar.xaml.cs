using ibcdatacsharp.UI.Common;
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
        }
        // Cambia el icono del boton Pause
        public void changePauseState(PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/pause-icon.png"));
                pauseText.Text = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                pauseImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/play-pause-icon.png"));
                pauseText.Text = "Play";
            }
        }
        // Cambia el icono del boton Record
        public void changeRecordState(RecordState recordState)
        {
            if (recordState == RecordState.RecordStopped)
            {
                recordImage.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/record-stop-icon.png"));
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
