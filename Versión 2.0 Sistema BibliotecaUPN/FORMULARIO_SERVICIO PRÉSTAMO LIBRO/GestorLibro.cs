using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class GestorLibro
    {
        private List<Libro> libro = new List<Libro>();

        private int siguienteCodigoLibro = 1;
        private int siguienteCodigoUL = 1;

        public string RegistrarLibro(
            string titulo,
            string area)
        {
            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(area))
            {
                return "Ingrese el título y el área del libro.";
            }

            titulo = titulo.Trim();
            area = area.Trim();

            Libro libroEncontrado = libro.Find(
                l => string.Equals(
                    l.Titulo,
                    titulo,
                    StringComparison.OrdinalIgnoreCase)
            );

            if (libroEncontrado == null)
            {
                string codigoLibro =
                    "L" + siguienteCodigoLibro.ToString("D3");

                libroEncontrado = new Libro(
                    codigoLibro,
                    titulo,
                    area
                );

                libro.Add(libroEncontrado);
                siguienteCodigoLibro++;
            }
            else if (!string.Equals(
                libroEncontrado.Area,
                area,
                StringComparison.OrdinalIgnoreCase))
            {
                return "El título ya existe con otra área.";
            }

            string codigoUL =
                "UL" + siguienteCodigoUL.ToString("D6");

            Ejemplar nuevoEjemplar = new Ejemplar(codigoUL);

            libroEncontrado.AgregarEjemplar(nuevoEjemplar);

            siguienteCodigoUL++;

            return "Libro registrado correctamente.\n" +
                   "Código del libro: " +
                   libroEncontrado.CodigoLibro + "\n" +
                   "Código UL del ejemplar: " + codigoUL;
        }

        public List<Libro> ObtenerLibros()
        {
            return libro;
        }
    }
}
