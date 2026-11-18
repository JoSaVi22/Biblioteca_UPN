using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public partial class FrmBiblioteca : Form
    {
        private GestorEstudiante gestorEstudiantes =
            new GestorEstudiante();

        private GestorLibro gestorLibro =
            new GestorLibro();

        private GestorBiblioteca gestorBiblioteca;

        public FrmBiblioteca()
        {
            InitializeComponent();
            TabContenido.Appearance = TabAppearance.FlatButtons;
            TabContenido.ItemSize = new Size(0, 1);
            TabContenido.SizeMode = TabSizeMode.Fixed;
            try
            {
                gestorLibro.CrearLibrosIniciales();
                gestorLibro.CargarLibros();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los libros: " + ex.Message);
            }
            gestorBiblioteca = new GestorBiblioteca(
                gestorEstudiantes,
                gestorLibro
            );
        }

        //Botones de navegación
        private void PicLogo_Click(object sender, EventArgs e)
        {
            TabContenido.SelectedTab = TabBienvenida;

        }
        private void BtnEstudiantes_Click(object sender, EventArgs e)
        {
            TabContenido.SelectedTab = TabEstudiantes;
        }
        private void BtnLibros_Click(object sender, EventArgs e)
        {
            TabContenido.SelectedTab = TabLibros;
        }
        private void BtnPrestamo_Click(object sender, EventArgs e)
        {
            TabContenido.SelectedTab = TabPrestamos;
        }
        private void BtnDevoluciones_Click(object sender, EventArgs e)
        {
            TabContenido.SelectedTab = TabDevoluciones;
        }

        //Botones de Estudiantes
        private void BtnRegistrarEstudiante_Click(object sender, EventArgs e)
        {
            if (CmbEstadoMatricula.SelectedItem == null)
            {
                MessageBox.Show("Seleccione el estado de matrícula.");
                return;
            }

            Estudiante estudiante = new Estudiante();

            estudiante.Codigo = TxtCodigoEstudiante.Text.Trim();
            estudiante.Nombres = TxtNombresEstudiante.Text.Trim();
            estudiante.Apellidos = TxtApellidosEstudiante.Text.Trim();
            estudiante.EstadodeMatricula = CmbEstadoMatricula.SelectedItem.ToString();

            bool registrado = gestorEstudiantes.RegistrarEstudiante(estudiante);

            if (!registrado)
            {
                MessageBox.Show(
                    "No se pudo registrar al estudiante. Verifique que el código tenga el formato N00 seguido de 6 números.",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            MostrarEstudiantes();

            TxtCodigoEstudiante.Clear();
            TxtNombresEstudiante.Clear();
            TxtApellidosEstudiante.Clear();
            CmbEstadoMatricula.SelectedIndex = -1;

            MessageBox.Show("Estudiante registrado correctamente.");
        }
        private void BtnMostrarEstudiante_Click(object sender, EventArgs e)
        {
            MostrarEstudiantes();
        }
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string codigo = TxtCodigoEstudiante.Text.Trim();
            string apellidos = TxtApellidosEstudiante.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo) &&
                string.IsNullOrWhiteSpace(apellidos))
            {
                MessageBox.Show("Ingrese un código o apellido del estudiante.");
                return;
            }

            Estudiante estudiante =
                gestorEstudiantes.BuscarEstudiante(codigo, apellidos);

            LimpiarSeleccion(DgvEstudiantes);

            if (estudiante == null)
            {
                MessageBox.Show("Estudiante no encontrado.");
                return;
            }

            int filaEncontrada = -1;

            for (int i = 0; i < DgvEstudiantes.Rows.Count; i++)
            {
                if (DgvEstudiantes.Rows[i].Cells[0].Value?.ToString()
                    == estudiante.Codigo)
                {
                    filaEncontrada = i;
                    break;
                }
            }

            if (filaEncontrada == -1)
            {
                filaEncontrada = DgvEstudiantes.Rows.Add(
                    estudiante.Codigo,
                    estudiante.Nombres,
                    estudiante.Apellidos,
                    estudiante.EstadodeMatricula
                );
            }

            DgvEstudiantes.Rows[filaEncontrada].Selected = true;
            DgvEstudiantes.CurrentCell =
                DgvEstudiantes.Rows[filaEncontrada].Cells[0];
        }
        private void MostrarEstudiantes()
        {
            DgvEstudiantes.Rows.Clear();

            foreach (Estudiante estudiante in gestorEstudiantes.ObtenerEstudiantes())
            {
                DgvEstudiantes.Rows.Add(
                    estudiante.Codigo,
                    estudiante.Nombres,
                    estudiante.Apellidos,
                    estudiante.EstadodeMatricula
                );
            }

            LimpiarSeleccion(DgvEstudiantes);
        }
        private void LimpiarSeleccion(DataGridView dgv)
        {
            dgv.ClearSelection();
            dgv.CurrentCell = null;
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (DgvEstudiantes.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un estudiante para eliminar.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar el estudiante seleccionado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            int fila = DgvEstudiantes.SelectedRows[0].Index;

            string codigo = DgvEstudiantes.Rows[fila]
                .Cells[0].Value?.ToString();

            bool eliminado = gestorEstudiantes.EliminarEstudiante(codigo);

            if (!eliminado)
            {
                MessageBox.Show("No se pudo eliminar al estudiante.");
                return;
            }

            MostrarEstudiantes();

            MessageBox.Show("Estudiante eliminado correctamente.");
        }
        private void BtnBurbujaEstudiante_Click(object sender, EventArgs e)
        {
            if (gestorEstudiantes.ObtenerEstudiantes().Count <= 1)
            {
                MessageBox.Show("No hay suficientes estudiantes para ordenar.");
                return;
            }

            gestorEstudiantes.OrdenarBurbujaEstudiante();

            MostrarEstudiantes();
        }
        private void BtnSeleccionEstudiante_Click(object sender, EventArgs e)
        {
            if (gestorEstudiantes.ObtenerEstudiantes().Count <= 1)
            {
                MessageBox.Show("No hay suficientes estudiantes para ordenar.");
                return;
            }

            gestorEstudiantes.OrdenarSeleccionEstudiante();

            MostrarEstudiantes();
        }

        //Botones de Libros
        private void BtnRegistrarLibro_Click(object sender, EventArgs e)
        {
            string titulo = TxtTituloLibro.Text.Trim();
            string area = TxtAreaLibro.Text.Trim();

            string resultado = gestorLibro.RegistrarLibro(titulo, area);

            MessageBox.Show(resultado);

            if (!resultado.StartsWith("Libro registrado correctamente"))
            {
                return;
            }

            MostrarLibros();

            TxtTituloLibro.Clear();
            TxtAreaLibro.Clear();
        }
        private void BtnMostrarLibros_Click(object sender, EventArgs e)
        {
            gestorLibro.RestaurarOrdenOriginal();
            MostrarLibros();
        }
        private void BtnBuscarLibro_Click(object sender, EventArgs e)
        {
            string titulo = TxtTituloLibro.Text.Trim();

            if (string.IsNullOrWhiteSpace(titulo))
            {
                MessageBox.Show("Ingrese el título del libro que desea buscar.");
                return;
            }

            DgvLibros.Rows.Clear();

            foreach (Libro libro in gestorLibro.ObtenerLibros())
            {
                if (libro.Titulo.IndexOf(titulo, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foreach (Ejemplar ejemplar in libro.ObtenerEjemplares())
                    {
                        DgvLibros.Rows.Add(
                            libro.CodigoLibro,
                            libro.Titulo,
                            libro.Area,
                            ejemplar.CodigoUL
                        );
                    }
                }
            }

            LimpiarSeleccion(DgvLibros);

            if (DgvLibros.Rows.Count == 0)
            {
                MessageBox.Show("No se encontraron libros.");
                return;
            }
        }
        private void BtnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (DgvLibros.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un libro para eliminar.");
                return;
            }

            string codigoUL = DgvLibros.SelectedRows[0]
                .Cells[3].Value?.ToString();

            if (string.IsNullOrWhiteSpace(codigoUL))
            {
                MessageBox.Show("Código UL no detectado.");
                return;
            }

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar el ejemplar seleccionado?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (respuesta != DialogResult.Yes)
            {
                return;
            }

            bool eliminado = gestorLibro.EliminarEjemplar(codigoUL);

            if (!eliminado)
            {
                MessageBox.Show("No se encontró el ejemplar.");
                return;
            }

            MostrarLibros();

            MessageBox.Show("Ejemplar eliminado correctamente.");
        }
        private void MostrarLibros()
        {
            DgvLibros.Rows.Clear();

            foreach (Libro libro in gestorLibro.ObtenerLibros())
            {
                foreach (Ejemplar ejemplar in libro.ObtenerEjemplares())
                {
                    DgvLibros.Rows.Add(
                        libro.CodigoLibro,
                        libro.Titulo,
                        libro.Area,
                        ejemplar.CodigoUL
                    );
                }
            }

            LimpiarSeleccion(DgvLibros);
        }
        private void BtnBurbujaLibro_Click(object sender, EventArgs e)
        {
            gestorLibro.OrdenarBurbujaLibros();
            MostrarLibros();
        }
        private void BtnSeleccionLibro_Click(object sender, EventArgs e)
        {
            gestorLibro.OrdenarSeleccionLibros();
            MostrarLibros();
        }

        //Botones de Préstamos
        private void BtnRegistrarPrestamo_Click(object sender, EventArgs e)
        {
            string codigoEstudiante = TxtCodigoEstPrestamo.Text.Trim();
            string codigoUL = TxtCodigoULPrestamo.Text.Trim();

            string resultado = gestorBiblioteca.RegistrarPrestamo(
                codigoEstudiante,
                codigoUL
            );

            MessageBox.Show(
                resultado,
                "Registro de Préstamo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            if (resultado == "Préstamo registrado correctamente.")
            {
                TxtCodigoEstPrestamo.Clear();
                TxtCodigoULPrestamo.Clear();

                MostrarPrestamos();
            }
        }
        private void BtnMostrarPrestamo_Click(object sender, EventArgs e)
        {
            MostrarPrestamos();
        }
        private void BtnBuscarPrestamo_Click(object sender, EventArgs e)
        {
            string codigoEstudiante = TxtCodigoEstPrestamo.Text.Trim();
            string codigoUL = TxtCodigoULPrestamo.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoEstudiante) &&
                string.IsNullOrWhiteSpace(codigoUL))
            {
                MessageBox.Show("Ingrese el código de estudiante o código UL.");
                return;
            }

            string codigo = !string.IsNullOrWhiteSpace(codigoEstudiante)
                ? codigoEstudiante
                : codigoUL;

            List<Prestamos> listaPrestamos =
                gestorBiblioteca.BuscarPrestamos(codigo);

            DgvPrestamos.Rows.Clear();

            if (listaPrestamos.Count == 0)
            {
                MessageBox.Show("No se encontraron préstamos.");
                return;
            }

            foreach (Prestamos prestamo in listaPrestamos)
            {
                DgvPrestamos.Rows.Add(
                    prestamo.CodigoEstudiante,
                    prestamo.CodigoULLibro,
                    prestamo.FechaPrestamo.ToString("dd/MM/yyyy"),
                    prestamo.FechaDevolucion.ToString("dd/MM/yyyy"),
                    prestamo.EstadoPrestamo
                );
            }

            LimpiarSeleccion(DgvPrestamos);
        }
        private void MostrarPrestamos()
        {
            DgvPrestamos.Rows.Clear();

            foreach (Prestamos prestamo in gestorBiblioteca.ObtenerPrestamos())
            {
                DgvPrestamos.Rows.Add(
                    prestamo.CodigoEstudiante,
                    prestamo.CodigoULLibro,
                    prestamo.FechaPrestamo.ToString("dd/MM/yyyy"),
                    prestamo.FechaDevolucion.ToString("dd/MM/yyyy"),
                    prestamo.EstadoPrestamo
                );
            }

            LimpiarSeleccion(DgvPrestamos);
        }

        //Botones de Devoluciones
        private void BtnRegistrarDevolucion_Click(object sender, EventArgs e)
        {
            string codigoUL = TxtCodigoULDevolucion.Text.Trim();

            string resultado = gestorBiblioteca.RegistrarDevolucion(codigoUL);

            MessageBox.Show(
                resultado,
                "Registro de Devolución",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            if (resultado == "Devolución registrada correctamente.")
            {
                TxtCodigoULDevolucion.Clear();

                List<Devoluciones> historial =
                    gestorBiblioteca.ObtenerDevoluciones();

                Devoluciones devolucion = historial[historial.Count - 1];

                TxtDevolucion.Text = CrearBoletaDevolucion(devolucion);
            }
        }
        private void BtnHistorialDevoluciones_Click(object sender, EventArgs e)
        {
            List<Devoluciones> historial =
        gestorBiblioteca.ObtenerDevoluciones();

            TxtDevolucion.Clear();

            if (historial.Count == 0)
            {
                TxtDevolucion.Text = "No hay devoluciones registradas.";
                return;
            }

            foreach (Devoluciones devolucion in historial)
            {
                TxtDevolucion.AppendText(
                    CrearBoletaDevolucion(devolucion) + "\r\n"
                );
            }
        }
        private string CrearBoletaDevolucion(Devoluciones devolucion)
        {
            return
                "      BOLETA DE DEVOLUCIÓN\r\n" +
                "------------------------------\r\n" +
                "Código del estudiante: " + devolucion.CodigoEstudiante + "\r\n" +
                "Código UL del libro: " + devolucion.CodigoUlLibro + "\r\n" +
                "Título del libro: " + devolucion.TituloLibro + "\r\n" +
                "Área del libro: " + devolucion.AreaLibro + "\r\n" +
                "Fecha de préstamo: " + devolucion.FechaPrestamo.ToString("dd/MM/yyyy") + "\r\n" +
                "Fecha de devolución prevista: " + devolucion.FechaDevolucionPrevista.ToString("dd/MM/yyyy") + "\r\n" +
                "Fecha de devolución real: " + devolucion.FechaDevolucionReal.ToString("dd/MM/yyyy") + "\r\n" +
                "Estado: " + devolucion.EstadoDevolucion + "\r\n" +
                "------------------------------\r\n";
        }
    }
}
