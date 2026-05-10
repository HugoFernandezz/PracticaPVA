using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    internal class Perfume
    {
        private Envase envase;

        private List<float> esencias;

        public string NombreCliente { get; set; }
        public string EmailCliente { get; set; }

        public Envase Envase => envase;
        public float Alcohol => esencias[0];
        public float Lavanda => esencias[1];
        public float Sandalo => esencias[2];
        public float Bergamota => esencias[3];

        public decimal Precio { get; set; }

        public Perfume(Envase envase, float alcohol, float lavanda, float sandalo, float bergamota)
        {
            this.envase = envase;
            this.esencias = new List<float> { alcohol, lavanda, sandalo, bergamota };
        }
    }
}
