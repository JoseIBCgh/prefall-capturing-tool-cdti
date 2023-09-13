using ibcdatacsharp.Login;
using ibcdatacsharp.UI.Pacientes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ibcdatacsharp.UI.Commands
{
    public class SeleccionarPacienteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Paciente paciente = (Paciente)parameter;
            LoginInfo.selectedPacienteId = paciente.Id;
        }
    }
}
