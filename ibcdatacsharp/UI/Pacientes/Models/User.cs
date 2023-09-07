using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.Pacientes.Models
{
    public class User
    {
        public string Nombre { get; set; }
        public ICollection<Centro> Centros { get; set; }
        public User(string Nombre)
        {
            this.Nombre = Nombre;
            this.Centros = new List<Centro>();
        }
    }
}
