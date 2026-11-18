using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Devoluciones
    {
        public string CodigoUlLibro { get; set; }
        public string CodigoEstudiante { get; set; }
        public string TituloLibro { get; set; }
        public string AreaLibro { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucionPrevista { get; set; }
        public DateTime FechaDevolucionReal { get; set; }
        public string EstadoDevolucion { get; set; }
    }
}
