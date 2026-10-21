using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_UPN
{
    public class Estudiante
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string EstadoMatricula { get; set; }

        public Estudiante(string codigo, string nombre,
                          string apellidos, string estadoMatricula)
        {
            Codigo = codigo;
            Nombre = nombre;
            Apellidos = apellidos;
            EstadoMatricula = estadoMatricula;
        }

        public bool ValidarCodigo()
        {
            return !string.IsNullOrWhiteSpace(Codigo);
        }

        public bool EstaActivo()
        {
            return EstadoMatricula == "ACTIVO";
        }

        public string ConvertirATexto()
        {
            return Codigo + "|" + Nombre + "|" +
                   Apellidos + "|" + EstadoMatricula;
        }

        public static Estudiante DesdeTexto(string linea)
        {
            string[] datos = linea.Split('|');

            if (datos.Length != 4)
                return null;

            return new Estudiante(
                datos[0],
                datos[1],
                datos[2],
                datos[3]);
        }
    }
}
