using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    internal class GestorBiblioteca
    {
        private GestorEstudiante gestorEstudiante;
        private GestorLibro gestorLibro;
        private string rutaBinarioPrestamos = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "prestamos.dat");
        private string rutaBinarioDevoluciones = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "devoluciones.dat");
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
                return "Error: El estudiante no se encuentra matriculado o no tiene matrícula ACTIVA para solicitar préstamos";
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
            gestorLibro.GuardarBinarioLibros();
            GuardarBinarioPrestamos();

            return "Préstamo registrado correctamente.";
        }

        // Obtener préstamos
        public List<Prestamos> ObtenerPrestamos()
        {
            return prestamos;
        }
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
        // Devuelve la cantidad de préstamos activos (pendientes de devolución)
        public int ContarPrestamosActivos()
        {
            int contador = 0;
            foreach (Prestamos p in prestamos)
            {
                if (string.Equals(p.EstadoPrestamo, "Prestado", StringComparison.OrdinalIgnoreCase))
                {
                    contador++;
                }
            }
            return contador;
        }
        // Devuelve el total histórico de préstamos registrados
        public int ContarTotalPrestamos() //NARUTO X SASUKE = NARUSASU
        {
            return prestamos.Count;
        }
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
            GuardarBinarioPrestamos();
            GuardarBinarioDevoluciones();

            return "Devolución registrada correctamente.";
        }

        // Obtener devoluciones
        public List<Devoluciones> ObtenerDevoluciones()
        {
            return devoluciones;
        }
        public void GuardarBinarioPrestamos()
        {
            using (FileStream fs = new FileStream(rutaBinarioPrestamos, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(prestamos.Count);
                    foreach (Prestamos p in prestamos)
                    {
                        bw.Write(p.CodigoEstudiante ?? "");
                        bw.Write(p.CodigoULLibro ?? "");
                        bw.Write(p.FechaPrestamo.ToBinary());
                        bw.Write(p.FechaDevolucion.ToBinary());
                        bw.Write(p.EstadoPrestamo ?? "Prestado");
                    }
                }
            }
        }
        public void LeerBinarioPrestamos()
        {
            if (!File.Exists(rutaBinarioPrestamos))
                return;

            List<Prestamos> prestamosCargados = new List<Prestamos>();
            try
            {
                using (FileStream fs = new FileStream(rutaBinarioPrestamos, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        int total = br.ReadInt32();
                        for (int i = 0; i < total; i++)
                        {
                            string codEst = br.ReadString();
                            string codUL = br.ReadString();
                            long fechaPrestamoTicks = br.ReadInt64();
                            long fechaDevolucionTicks = br.ReadInt64();
                            string estado = br.ReadString();
                            DateTime fechaPrestamo = (fechaPrestamoTicks >= DateTime.MinValue.Ticks && fechaPrestamoTicks <= DateTime.MaxValue.Ticks) ? new DateTime(fechaPrestamoTicks) : DateTime.Now;
                            DateTime fechaDevolucion = (fechaDevolucionTicks >= DateTime.MinValue.Ticks && fechaDevolucionTicks <= DateTime.MaxValue.Ticks) ? new DateTime(fechaDevolucionTicks) : DateTime.Now.AddDays(3);

                            Prestamos p = new Prestamos(codEst, codUL)
                            {
                                FechaPrestamo = fechaPrestamo, // Validar ticks
                                FechaDevolucion = fechaDevolucion, // Validar ticks
                                EstadoPrestamo = estado
                            };

                            prestamosCargados.Add(p);
                        }
                    }
                }

                prestamos = prestamosCargados;
            }
            catch (Exception ex)
            {
                //Si el archivo binario corrupto, dañado, incompleto, etc, evita que la aplicación colapse
                System.Diagnostics.Debug.WriteLine("Error al leer archivo binario de préstamos: " + ex.Message);
            }

        }
        public void GuardarBinarioDevoluciones()
        {
            using (FileStream fs = new FileStream(rutaBinarioPrestamos, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(devoluciones.Count);
                    foreach (Devoluciones d in devoluciones)
                    {
                        bw.Write(d.CodigoUlLibro ?? "");
                        bw.Write(d.CodigoEstudiante ?? "");
                        bw.Write(d.TituloLibro ?? "");
                        bw.Write(d.AreaLibro ?? "");
                        bw.Write(d.FechaPrestamo.ToBinary());
                        bw.Write(d.FechaDevolucionPrevista.ToBinary());
                        bw.Write(d.FechaDevolucionReal.ToBinary());
                        bw.Write(d.EstadoDevolucion ?? "Devuelto");
                    }
                }
            }
        }
        // Leer lista de devoluciones desde el archivo binario (protegido contra errores)
        public void LeerBinarioDevoluciones()
        {
            if (!File.Exists(rutaBinarioDevoluciones))
                return;

            List<Devoluciones> devolucionesCargadas = new List<Devoluciones>();

            try
            {
                using (FileStream fs = new FileStream(rutaBinarioDevoluciones, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        int total = br.ReadInt32();
                        for (int i = 0; i < total; i++)
                        {
                            string codUL = br.ReadString();
                            string codEst = br.ReadString();
                            string titulo = br.ReadString();
                            string area = br.ReadString();

                            long fPrestamoTicks = br.ReadInt64();
                            long fPrevistaTicks = br.ReadInt64();
                            long fRealTicks = br.ReadInt64();

                            string estado = br.ReadString();

                            // Validar que los ticks pertenezcan al rango permitido de DateTime
                            DateTime fPrestamo = (fPrestamoTicks >= DateTime.MinValue.Ticks && fPrestamoTicks <= DateTime.MaxValue.Ticks)
                                ? new DateTime(fPrestamoTicks)
                                : DateTime.Now;

                            DateTime fPrevista = (fPrevistaTicks >= DateTime.MinValue.Ticks && fPrevistaTicks <= DateTime.MaxValue.Ticks)
                                ? new DateTime(fPrevistaTicks)
                                : DateTime.Now.AddDays(3);

                            DateTime fReal = (fRealTicks >= DateTime.MinValue.Ticks && fRealTicks <= DateTime.MaxValue.Ticks)
                                ? new DateTime(fRealTicks)
                                : DateTime.Today;

                            Devoluciones dev = new Devoluciones
                            {
                                CodigoUlLibro = codUL,
                                CodigoEstudiante = codEst,
                                TituloLibro = titulo,
                                AreaLibro = area,
                                FechaPrestamo = fPrestamo,
                                FechaDevolucionPrevista = fPrevista,
                                FechaDevolucionReal = fReal,
                                EstadoDevolucion = estado
                            };

                            devolucionesCargadas.Add(dev);
                        }
                    }
                }
                devoluciones = devolucionesCargadas;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar binario de devoluciones: " + ex.Message);
            }
        }
    }
}