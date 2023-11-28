using ibcdatacsharp.Login;
using ibcdatacsharp.UI.Pacientes;
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
    /// Comnando para subir un test
    /// </summary>
    public class SubirTestCommand : ICommand
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
        /// Funcion que ejecuta el comando. Sube el test mediante RemoteTransactions.SubirTest
        /// </summary>
        /// <param name="parameter">parametro pasado al comando(debe ser un objeto ibcdatacsharp.UI.Pacientes.Models.Test)</param>
        public void Execute(object? parameter)
        {
            Test test = (Test)parameter;
            RemoteTransactions.SubirTest(test.Path);
        }
    }
}
