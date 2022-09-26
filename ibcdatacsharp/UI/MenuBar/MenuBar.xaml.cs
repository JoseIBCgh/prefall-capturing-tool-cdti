using ibcdatacsharp.UI.Common;
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
                Image image = Helpers.GetChildOfType<Image>(pause);
                image.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/pause-icon.png"));
                pause.Header = "Pause";
            }
            else if (pauseState == PauseState.Play)
            {
                Image image = Helpers.GetChildOfType<Image>(pause);
                image.Source = new BitmapImage(new Uri("pack://application:,,,/UI/ToolBar/Icons/play-pause-icon.png"));
                pause.Header = "Play";
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
