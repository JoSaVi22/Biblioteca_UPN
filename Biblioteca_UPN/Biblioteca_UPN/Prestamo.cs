using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_UPN
{
    public class Prestamo
    {
        public string CodigoEstudiante { get; set; }
        public string CodigoUL { get; set; }
        public DateTime FechaPrestamo { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public bool Activo { get; set; }

        public Prestamo(string codigoEstudiante, string codigoUL)
        {
            CodigoEstudiante = codigoEstudiante;
            CodigoUL = codigoUL;
            FechaPrestamo = DateTime.Now;
            FechaDevolucion = FechaPrestamo.AddDays(3);
            Activo = true;
        }

        public bool VerificarPlazo()
        {
            return DateTime.Now.Date <= FechaDevolucion.Date;
        }

        public void RegistrarDevolucion()
        {
            Activo = false;
        }

        public string ConvertirATexto()
        {
            return CodigoEstudiante + "|" +
                   CodigoUL + "|" +
                   FechaPrestamo.ToString("O") + "|" +
                   FechaDevolucion.ToString("O") + "|" +
                   Activo;
        }

        public static Prestamo DesdeTexto(string linea)
        {
            string[] datos = linea.Split('|');

            if (datos.Length != 5)
                return null;

            if (!DateTime.TryParse(datos[2], out DateTime fechaPrestamo))
                return null;

            if (!DateTime.TryParse(datos[3], out DateTime fechaDevolucion))
                return null;

            if (!bool.TryParse(datos[4], out bool activo))
                return null;

            Prestamo prestamo = new Prestamo(datos[0], datos[1]);

            prestamo.FechaPrestamo = fechaPrestamo;
            prestamo.FechaDevolucion = fechaDevolucion;
            prestamo.Activo = activo;

            return prestamo;
        }
    }
}
