using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.Pacientes.Models
{
    public class CentroRoot
    {
        public string Nombre { get; set; }
        public ICollection<Auxiliar> Auxiliar { get; set; }
        public CentroRoot(string Nombre)
        {
            this.Nombre = Nombre;
            this.Auxiliar = new List<Auxiliar>();
        }
    }
}
