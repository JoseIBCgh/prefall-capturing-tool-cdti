using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ibcdatacsharp.UI.MenuBar
{
    /// <summary>
    /// Lógica de interacción para Version.xaml
    /// </summary>
    public partial class Version : Window
    {
        public Version()
        {
            InitializeComponent();
            initIcon();
        }
        // Cambia el icono de la ventana
        private void initIcon()
        {
            Uri iconUri = new Uri("pack://application:,,,/UI/MenuBar/Icons/ibc-logo.png", UriKind.RelativeOrAbsolute);
            Icon = BitmapFrame.Create(iconUri);
        }
        // Cierra la ventana. (Al clicar Ok)
        private void close(object sender, RoutedEventArgs e)
        {
            GetWindow(this).Close();
        }
        // Abre el navegador a la pagina web
        private void hiperlinkRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = e.Uri.AbsoluteUri,
                UseShellExecute = true
            });
            e.Handled = true;
        }
        // Hace que el tamaño de la ventana sea el establecido en el layoutRoot en el xaml
        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            ClearValue(SizeToContentProperty);
            layoutRoot.ClearValue(WidthProperty);
            layoutRoot.ClearValue(HeightProperty);
            GetWindow(this).ResizeMode = ResizeMode.NoResize;
        }
    }
}
