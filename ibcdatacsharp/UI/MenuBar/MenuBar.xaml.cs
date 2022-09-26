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
        private void showVersion(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "(c) 2022 IBC Biomechanics\n\nIMU Data Adquisition Tool v0.9.3\n\nhttp://ibc.bio";
            string caption = "IBC Biomechanics Research";
            MessageBoxButton button = MessageBoxButton.OK;
            MessageBoxImage icon = MessageBoxImage.Information;
            MessageBoxResult result;

            result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
        }
    }
}
