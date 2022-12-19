using ibcdatacsharp.DeviceList.TreeClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ibcdatacsharp.UI.Subjects
{
    /// <summary>
    /// Lógica de interacción para Subjects.xaml
    /// </summary>
    public partial class Subjects : Page
    {
        public ObservableCollection<Subject> subjectsList
        {
            get; set;
        }
        public Subjects()
        {
            InitializeComponent();
            DataContext = this;
            subjectsList = new ObservableCollection<Subject>
            {
                new Subject { nombre = "Pepe", centroMedico = "Hospital X", doctor = "Juan" },
                new Subject { nombre = "Pedro", centroMedico = "Hospital Y", doctor = "Juan" }
            };

        }
    }
}
