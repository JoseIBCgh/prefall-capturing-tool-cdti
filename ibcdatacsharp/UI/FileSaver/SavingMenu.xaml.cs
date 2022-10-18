using System.Windows;
using System.Windows.Forms;

namespace ibcdatacsharp.UI.FileSaver
{
    /// <summary>
    /// Lógica de interacción para SavingMenu.xaml
    /// </summary>
    public partial class SavingMenu : Window
    {
        public SavingMenu()
        {
            InitializeComponent();
        }
        private void changeDirectory(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                route.Text = folderBrowserDialog.SelectedPath;
            }
        }
    }
}
