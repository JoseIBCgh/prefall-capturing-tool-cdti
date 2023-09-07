using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.Pacientes.Models
{
    public class Paciente
    {
        public string Nombre { get; set; }
        public Paciente(string Nombre) 
        { 
            this.Nombre = Nombre;
        }
    }
}
