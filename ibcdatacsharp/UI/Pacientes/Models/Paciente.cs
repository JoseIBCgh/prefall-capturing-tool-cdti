using ibcdatacsharp.Login;
using ibcdatacsharp.UI.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ibcdatacsharp.UI.Pacientes.Models
{
    public class Paciente : INotifyPropertyChanged
    {
        public string Nombre { get; set; }
        public int Id { get; set; }
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
        public SeleccionarPacienteCommand seleccionarPacienteCommand { get; set; }
        public Paciente(string Nombre, int Id) 
        { 
            this.Nombre = Nombre;
            this.Id = Id;
            this.seleccionarPacienteCommand = new SeleccionarPacienteCommand();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
