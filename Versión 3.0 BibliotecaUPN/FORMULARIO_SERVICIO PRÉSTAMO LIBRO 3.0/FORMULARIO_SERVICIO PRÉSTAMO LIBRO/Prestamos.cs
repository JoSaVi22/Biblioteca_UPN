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
        private DateTime fechaPrestamo;
        private DateTime fechaDevolucion;
        private string estadoPrestamo;

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
        public DateTime FechaPrestamo
        {
            get { return fechaPrestamo; }
            set { fechaPrestamo = value; }
        }
        public DateTime FechaDevolucion
        {
            get { return fechaDevolucion; }
            set { fechaDevolucion = value; }
        }
        public string EstadoPrestamo
        {
            get { return estadoPrestamo; }
            set { estadoPrestamo = value; }
        }

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
