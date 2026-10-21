using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_UPN
{
    public class Libro
    {
        public string CodigoLibro { get; set; }
        public string Titulo { get; set; }
        public string Area { get; set; }

        private Ejemplar[] ejemplares;
        private int cantidadEjemplares;

        public Libro(string codigoLibro, string titulo, string area)
        {
            CodigoLibro = codigoLibro;
            Titulo = titulo;
            Area = area;

            ejemplares = new Ejemplar[20];
            cantidadEjemplares = 0;
        }

        public bool AgregarEjemplar(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return false;

            codigoUL = codigoUL.Trim();

            if (BuscarEjemplar(codigoUL) != null)
                return false;

            if (cantidadEjemplares >= ejemplares.Length)
            {
                Ejemplar[] nuevo =
                    new Ejemplar[ejemplares.Length * 2];

                Array.Copy(ejemplares, nuevo, ejemplares.Length);
                ejemplares = nuevo;
            }

            ejemplares[cantidadEjemplares] =
                new Ejemplar(codigoUL, true);

            cantidadEjemplares++;

            return true;
        }

        public Ejemplar BuscarEjemplar(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return null;

            for (int i = 0; i < cantidadEjemplares; i++)
            {
                if (string.Equals(
                    ejemplares[i].CodigoUL,
                    codigoUL.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ejemplares[i];
                }
            }

            return null;
        }

        public bool VerificarDisponibilidad(string codigoUL)
        {
            Ejemplar ejemplar = BuscarEjemplar(codigoUL);

            if (ejemplar == null)
                return false;

            return ejemplar.VerificarDisponibilidad();
        }

        public bool ActualizarDisponibilidad(string codigoUL, bool estado)
        {
            Ejemplar ejemplar = BuscarEjemplar(codigoUL);

            if (ejemplar == null)
                return false;

            ejemplar.ActualizarDisponibilidad(estado);
            return true;
        }

        public int ObtenerCantidadEjemplares()
        {
            return cantidadEjemplares;
        }

        public Ejemplar ObtenerEjemplarPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidadEjemplares)
                return null;

            return ejemplares[indice];
        }

        public string ConvertirATexto()
        {
            string linea = CodigoLibro + "|" + Titulo + "|" + Area;

            for (int i = 0; i < cantidadEjemplares; i++)
            {
                linea += "|" + ejemplares[i].CodigoUL +
                         "|" + ejemplares[i].Disponible;
            }

            return linea;
        }

        public static Libro DesdeTexto(string linea)
        {
            string[] datos = linea.Split('|');

            if (datos.Length < 3 ||
                (datos.Length - 3) % 2 != 0)
            {
                return null;
            }

            Libro libro = new Libro(datos[0], datos[1], datos[2]);

            for (int i = 3; i < datos.Length; i += 2)
            {
                if (!bool.TryParse(datos[i + 1], out bool disponible))
                    return null;

                if (!libro.AgregarEjemplar(datos[i]))
                    return null;

                libro.ActualizarDisponibilidad(datos[i], disponible);
            }

            return libro;
        }
    }
}
