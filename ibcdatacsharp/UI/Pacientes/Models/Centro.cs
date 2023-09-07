using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.Pacientes.Models
{
    public class Centro
    {
        public string Nombre { get; set; }
        public ICollection<Paciente> Pacientes { get; set;}
        public Centro(string Nombre) 
        {
            this.Nombre = Nombre;
            this.Pacientes = new List<Paciente>();
        }
    }
}
