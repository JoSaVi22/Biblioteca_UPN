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

        private List<Prestamos> prestamos = new List<Prestamos>();
        private List<Devoluciones> devoluciones = new List<Devoluciones>();

        public GestorBiblioteca(GestorEstudiante gestorEstudiante, GestorLibro gestorLibro)
        {
            this.gestorEstudiante = gestorEstudiante;
            this.gestorLibro = gestorLibro;
        }

        // Buscar un ejemplar por su código UL
        private Ejemplar BuscarEjemplar(string codigoUL, out Libro libroEncontrado)
        {
            libroEncontrado = null;

            foreach (Libro libro in gestorLibro.ObtenerLibros())
            {
                Ejemplar ejemplar = libro.ObtenerEjemplares().Find(
                    e => string.Equals(e.CodigoUL, codigoUL,
                    StringComparison.OrdinalIgnoreCase));

                if (ejemplar != null)
                {
                    libroEncontrado = libro;
                    return ejemplar;
                }
            }

            return null;
        }

        // Registrar préstamo
        public string RegistrarPrestamo(string codigoEstudiante, string codigoULLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoEstudiante) ||
                string.IsNullOrWhiteSpace(codigoULLibro))
            {
                return "Error: Ingrese el código del estudiante y del libro.";
            }

            codigoEstudiante = codigoEstudiante.Trim();
            codigoULLibro = codigoULLibro.Trim();

            Estudiante estudiante = gestorEstudiante.ObtenerEstudiantes().Find(
                e => string.Equals(e.Codigo, codigoEstudiante,
                StringComparison.OrdinalIgnoreCase));

            if (estudiante == null)
                return "Error: Estudiante no encontrado.";

            if (!string.Equals(estudiante.EstadodeMatricula, "ACTIVO",
                StringComparison.OrdinalIgnoreCase))
            {
                return "Error: El estudiante no se encuentra matriculado o está inactivo.";
            }

            Libro libroEncontrado;
            Ejemplar ejemplar = BuscarEjemplar(codigoULLibro, out libroEncontrado);

            if (ejemplar == null)
                return "Ejemplar no encontrado.";

            if (!ejemplar.Disponible)
                return "Ejemplar no disponible.";

            Prestamos nuevoPrestamo = new Prestamos(
                estudiante.Codigo,
                ejemplar.CodigoUL);

            prestamos.Add(nuevoPrestamo);

            ejemplar.Disponible = false;
            gestorLibro.GuardarLibros();

            return "Préstamo registrado correctamente.";
        }

        // Obtener préstamos
        public List<Prestamos> ObtenerPrestamos()
        {
            return prestamos;
        }

        // Buscar préstamos por código de estudiante o código UL
        public List<Prestamos> BuscarPrestamos(string codigo)
        {
            List<Prestamos> encontrados = new List<Prestamos>();

            if (string.IsNullOrWhiteSpace(codigo))
                return encontrados;

            codigo = codigo.Trim();

            foreach (Prestamos prestamo in prestamos)
            {
                if (string.Equals(prestamo.CodigoEstudiante, codigo,
                    StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(prestamo.CodigoULLibro, codigo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    encontrados.Add(prestamo);
                }
            }

            return encontrados;
        }

        // Registrar devolución
        public string RegistrarDevolucion(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return "El código UL no puede estar vacío.";

            codigoUL = codigoUL.Trim();

            Prestamos prestamo = prestamos.Find(
                p => string.Equals(p.CodigoULLibro, codigoUL,
                StringComparison.OrdinalIgnoreCase)
                && string.Equals(p.EstadoPrestamo, "Prestado",
                StringComparison.OrdinalIgnoreCase));

            if (prestamo == null)
                return "Préstamo no encontrado para el código UL proporcionado.";

            Libro libroEncontrado;
            Ejemplar ejemplar = BuscarEjemplar(codigoUL, out libroEncontrado);

            if (ejemplar == null)
                return "Ejemplar no encontrado.";

            Devoluciones devolucion = new Devoluciones
            {
                CodigoUlLibro = prestamo.CodigoULLibro,
                CodigoEstudiante = prestamo.CodigoEstudiante,
                TituloLibro = libroEncontrado.Titulo,
                AreaLibro = libroEncontrado.Area,
                FechaPrestamo = prestamo.FechaPrestamo,
                FechaDevolucionPrevista = prestamo.FechaDevolucion,
                FechaDevolucionReal = DateTime.Today,
                EstadoDevolucion = "Devuelto"
            };

            devoluciones.Add(devolucion);

            prestamo.EstadoPrestamo = "Devuelto";
            ejemplar.Disponible = true;

            gestorLibro.GuardarLibros();

            return "Devolución registrada correctamente.";
        }

        // Obtener devoluciones
        public List<Devoluciones> ObtenerDevoluciones()
        {
            return devoluciones;
        }
    }
}
