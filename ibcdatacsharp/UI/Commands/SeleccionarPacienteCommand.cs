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
    /// <summary>
    /// Commando para seleccionar un paciente
    /// </summary>
    public class SeleccionarPacienteCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        /// <summary>
        /// Funcion que determina cuando puede ejecutarse el comando
        /// </summary>
        /// <param name="parameter">parametro pasado al comando</param>
        /// <returns>true</returns>
        public bool CanExecute(object? parameter)
        {
            return true;
        }
        /// <summary>
        /// Cambia el paciente seleccionado en LoginInfo
        /// </summary>
        /// <param name="parameter">parametro pasado al comando</param>
        public void Execute(object? parameter)
        {
            Paciente paciente = (Paciente)parameter;
            LoginInfo.selectedPacienteId = paciente.Id;
        }
    }
}
