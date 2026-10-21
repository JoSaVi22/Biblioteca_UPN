using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_UPN
{
    public class Ejemplar
    {
        public string CodigoUL { get; set; }
        public bool Disponible { get; set; }

        public Ejemplar(string codigoUL, bool disponible)
        {
            CodigoUL = codigoUL;
            Disponible = disponible;
        }

        public bool VerificarDisponibilidad()
        {
            return Disponible;
        }

        public void ActualizarDisponibilidad(bool estado)
        {
            Disponible = estado;
        }
    }
}
