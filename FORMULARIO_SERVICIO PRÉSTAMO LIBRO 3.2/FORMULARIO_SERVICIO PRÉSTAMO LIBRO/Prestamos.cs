using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Prestamos
    {
        public string CodigoEstudiante { get; set; }
        public string CodigoULLibro { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public string EstadoPrestamo { get; set; }

        public Prestamos(string codigoEstudiante, string codigoULLibro)
        {
            CodigoEstudiante = codigoEstudiante;
            CodigoULLibro = codigoULLibro;
            FechaPrestamo = DateTime.Now;
            FechaDevolucion = FechaPrestamo.AddDays(3);
            EstadoPrestamo = "Prestado";
        }
    }
}
