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
    public class CentroRoot : INotifyPropertyChanged
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
        public ObservableCollection<Auxiliar> Auxiliar { get; set; }
        public CentroRoot(string Nombre)
        {
            this.Nombre = Nombre;
            this.Auxiliar = new ObservableCollection<Auxiliar>();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
