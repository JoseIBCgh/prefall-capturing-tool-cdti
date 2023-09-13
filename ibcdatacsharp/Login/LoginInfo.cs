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
    public static class LoginInfo
    {
        public static string nombre;
        private static int? _selectedPacienteId = null;
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

        public static EventHandler selectedPacienteIdChanged;
    }
}
