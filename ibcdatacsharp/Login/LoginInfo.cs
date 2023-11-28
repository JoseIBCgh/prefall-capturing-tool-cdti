using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.Login
{
    /// <summary>
    /// Guarda informacion sobre el usuario que se ha logueado.
    /// </summary>
    public static class LoginInfo
    {
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        public static string nombre;
        private static int? _selectedPacienteId = null;
        /// <summary>
        /// Paciente seleccionado en la lista de pacientes
        /// </summary>
        public static int? selectedPacienteId
        {
            get
            {
                return _selectedPacienteId;
            }
            set
            {
                _selectedPacienteId = value;
                if (value != null)
                {
                    Trace.WriteLine("Invoking selectedPacienteIdChanged");
                    selectedPacienteIdChanged?.Invoke(null, EventArgs.Empty);
                }
            }
        }
        /// <summary>
        /// Evento que se debe invocar cuando cambia el paciente seleccionado.
        /// </summary>
        public static EventHandler selectedPacienteIdChanged;
    }
}
