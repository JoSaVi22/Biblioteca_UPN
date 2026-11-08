using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Prestamos
    {
        private string codigoEstudiante;
        private string codigoULLibro;
        public string CodigoEstudiante
        {
            get { return codigoEstudiante; }
            set { codigoEstudiante = value; }
        }
        public string CodigoULLibro
        {
            get { return codigoULLibro; }
            set { codigoULLibro = value; }
        }
    }
}
