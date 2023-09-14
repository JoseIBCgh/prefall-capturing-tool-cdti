using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ibcdatacsharp.UI.Pacientes
{
    /// <summary>
    /// Lógica de interacción para MessageRecorded.xaml
    /// </summary>
    public partial class MessageRecorded : Window
    {
        public string Nombre { get; set; }
        public int Id { get; set; }
        public List<string> Files { get; set; }
        public MessageRecorded(string Nombre, int Id, List<string> Files)
        {
            InitializeComponent();
            DataContext = this;
            this.Nombre = Nombre;
            this.Id = Id;
            this.Files = Files;

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
