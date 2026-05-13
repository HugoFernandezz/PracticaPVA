using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public class Envase
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int CapacidadMl { get; set; }
        public decimal Precio { get; set; }

        public override string ToString()
        {
            return $"{Nombre} - ${Precio}";
        }
    }
}
