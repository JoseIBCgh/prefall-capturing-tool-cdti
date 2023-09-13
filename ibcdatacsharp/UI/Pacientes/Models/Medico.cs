using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.Pacientes.Models
{
    public class Medico : INotifyPropertyChanged
    {
        public string Nombre { get; set; }
        private bool isSelected;
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Centro> Centros { get; set; }
        public Medico(string Nombre)
        {
            this.Nombre = Nombre;
            this.Centros = new ObservableCollection<Centro>();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
