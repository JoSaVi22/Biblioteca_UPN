using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Ejemplar
    {
        public string CodigoUL { get; set; }
        public bool Disponible { get; set; }
        public Ejemplar (string codigoUL)
        {
            CodigoUL = codigoUL;
            Disponible = true;
        }
    }
}
