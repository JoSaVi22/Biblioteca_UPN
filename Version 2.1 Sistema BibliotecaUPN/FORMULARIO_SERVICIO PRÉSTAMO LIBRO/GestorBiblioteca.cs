using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    internal class GestorBiblioteca
    {
        private GestorEstudiante gestorEstudiante;
        private GestorLibro gestorLibro;
        private List<Prestamos> prestamos =
            new List<Prestamos>();
        private List<Devoluciones> devoluciones =
            new List<Devoluciones>();

        public GestorBiblioteca(GestorEstudiante gestorEstudiante, GestorLibro gestorLibro)
        {
            this.gestorEstudiante = gestorEstudiante;
            this.gestorLibro = gestorLibro;
        }
        //Registrar el préstamo
        public string RegistrarPrestamo(
            string codigoEstudiante,
            string codigoULLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoEstudiante) || string.IsNullOrWhiteSpace(codigoULLibro))
            {
                return "Error: Código de estudiante o código de libro no puede estar vacío.";
            }
            codigoEstudiante = codigoEstudiante.Trim();
            codigoULLibro = codigoULLibro.Trim();

            Estudiante estudiante =
                gestorEstudiante.ObtenerEstudiantes().Find(
                    e => string.Equals(
                        e.Codigo, codigoEstudiante,
                        StringComparison.OrdinalIgnoreCase));
            if(estudiante == null)
            {
                return "Error: Estudiante no encontrado.";
            }

            Ejemplar ejemplarEncontrado = null;
            foreach (Libro libro in gestorLibro.ObtenerLibros())
            {
                ejemplarEncontrado = libro.ObtenerEjemplares().Find(
                    e => string.Equals(
                        e.CodigoUL, codigoULLibro,
                        StringComparison.OrdinalIgnoreCase));

                if (ejemplarEncontrado != null)
                {
                    break;
                }
            }
            if(ejemplarEncontrado == null)
            {
                return "Ejemplar no encontrado.";
            }

            if(!ejemplarEncontrado.Disponible)
            {
                return "Ejemplar no disponible.";
            }
            
            Prestamos nuevoPrestamo = new Prestamos(
                estudiante.Codigo,
                ejemplarEncontrado.CodigoUL);
            
            prestamos.Add(nuevoPrestamo);

            ejemplarEncontrado.Disponible = false;
            gestorLibro.GuardarLibros();

            return "Préstamo registrado correctamente.";
        }
        public List<Prestamos> ObtenerPrestamos()
        {
            return prestamos;
        }
        public List<Prestamos> BuscarPrestamos(string codigo)
        {
            List<Prestamos> encontrados = new List<Prestamos>();

            if(string.IsNullOrWhiteSpace(codigo))
            {
                return encontrados;
            }
            codigo = codigo.Trim();
            
            foreach(Prestamos prestamo in prestamos)
            {
                if (string.Equals(
                    prestamo.CodigoEstudiante,
                    codigo,
                    StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(
                       prestamo.CodigoULLibro,
                       codigo,
                       StringComparison.OrdinalIgnoreCase))
                {
                    encontrados.Add(prestamo);
                }
            }
            return encontrados;
        }

        //Devoluciones
        public string RegistrarDevolucion(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
            {
                return "El Código UL no puede estar vacío.";
            }

            codigoUL = codigoUL.Trim();

            Prestamos prestamoEncontrado = prestamos.Find(
                p => string.Equals(
                    p.CodigoULLibro, codigoUL,
                    StringComparison.OrdinalIgnoreCase)
                && p.EstadoPrestamo == "Prestado"
                );

            if (prestamoEncontrado == null)
            {
                return "Préstamo no encontrado para el código UL proporcionado.";
            }

            Ejemplar ejemplarEncontrado = null;
            Libro libroEncontrado = null;

            foreach (Libro libro in gestorLibro.ObtenerLibros())
            {
                ejemplarEncontrado = libro.ObtenerEjemplares().Find(
                    e => string.Equals(
                        e.CodigoUL, codigoUL,
                        StringComparison.OrdinalIgnoreCase)); 

                if (ejemplarEncontrado != null)
                {
                    libroEncontrado = libro;
                    break;
                }
            }

            if (ejemplarEncontrado == null)
            {
                return "Ejemplar no encontrado.";
            }

            Devoluciones devolucion = new Devoluciones();

            devolucion.CodigoUlLibro = prestamoEncontrado.CodigoULLibro;
            devolucion.CodigoEstudiante = prestamoEncontrado.CodigoEstudiante;
            devolucion.TituloLibro = libroEncontrado.Titulo;
            devolucion.AreaLibro = libroEncontrado.Area;
            devolucion.FechaPrestamo = prestamoEncontrado.FechaPrestamo;
            devolucion.FechaDevolucionPrevista = prestamoEncontrado.FechaDevolucion;
            devolucion.FechaDevolucionReal = DateTime.Today;
            devolucion.EstadoDevolucion = "Devuelto";

            devoluciones.Add(devolucion);

            prestamoEncontrado.EstadoPrestamo = "Devuelto";
            ejemplarEncontrado.Disponible = true;

            gestorLibro.GuardarLibros();
            return "Devolución registrada correctamente.";

        }
        public List<Devoluciones> ObtenerDevoluciones()
        {
            return devoluciones;
        }
    }
}
