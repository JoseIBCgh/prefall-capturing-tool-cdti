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
        public void ChangePauseState(PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                Image image = Helpers.GetChildOfType<Image>(pause);
                image.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/pause-icon.png"));
                TextBlock text = Helpers.GetChildOfType<TextBlock>(pause);
                text.Text = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                Image image = Helpers.GetChildOfType<Image>(pause);
                image.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/play-pause-icon.png"));
                TextBlock text = Helpers.GetChildOfType<TextBlock>(pause);
                text.Text = "Play";
            }
        }


    }
}
