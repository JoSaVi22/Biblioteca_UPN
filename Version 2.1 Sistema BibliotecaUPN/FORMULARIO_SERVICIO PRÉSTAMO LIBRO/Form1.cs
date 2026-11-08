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

        private void FrmBiblioteca_Load(object sender, EventArgs e)
        {

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
            estudiante.EstadodeMatricula =
                CmbEstadoMatricula.SelectedItem.ToString();

            bool registrado =
                gestorEstudiantes.RegistrarEstudiante(estudiante);

            if (registrado)
            {
                DgvEstudiantes.Rows.Add(
                    estudiante.Codigo,
                    estudiante.Nombres,
                    estudiante.Apellidos,
                    estudiante.EstadodeMatricula
                );

                DgvEstudiantes.ClearSelection();
                DgvEstudiantes.CurrentCell = null;

                TxtCodigoEstudiante.Clear();
                TxtNombresEstudiante.Clear();
                TxtApellidosEstudiante.Clear();
                CmbEstadoMatricula.SelectedIndex = -1;

                MessageBox.Show("Estudiante registrado correctamente.");
            }
            else
            {
                MessageBox.Show(
                    "No se pudo registrar al estudiante. Verifique que el código tenga el formato \"N00\" seguido de 5 números.",
                    "ERROR",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void BtnMostrarEstudiante_Click(object sender, EventArgs e)
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
            DgvEstudiantes.ClearSelection();
            DgvEstudiantes.CurrentCell = null;

            if (gestorEstudiantes.ObtenerEstudiantes().Count == 0)
            {
                MessageBox.Show(
                    "No hay estudiantes registrados.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }

            MessageBox.Show(
                "Estudiantes mostrados correctamente.",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            string codigo = TxtCodigoEstudiante.Text.Trim();
            string apellidos = TxtApellidosEstudiante.Text.Trim();

            if (codigo == "" && apellidos == "")
            {
                MessageBox.Show("Ingrese un código o apellido del estudiante.");
                return;
            }

            DgvEstudiantes.ClearSelection();
            DgvEstudiantes.CurrentCell = null;

            bool encontrado = false;

            foreach (DataGridViewRow fila in DgvEstudiantes.Rows)
            {
                if (fila.IsNewRow)
                    continue;

                string codigoTabla = fila.Cells[0].Value?.ToString() ?? "";
                string apellidosTabla = fila.Cells[2].Value?.ToString() ?? "";

                bool coincideCodigo = codigo != "" &&
                    codigoTabla.Equals(codigo, StringComparison.OrdinalIgnoreCase);

                bool coincideApellidos = apellidos != "" &&
                    apellidosTabla.IndexOf(apellidos, StringComparison.OrdinalIgnoreCase) >= 0;
                if (coincideCodigo || coincideApellidos)
                {
                    fila.Selected = true;
                    DgvEstudiantes.CurrentCell = fila.Cells[0];

                    encontrado = true;
                    break;
                }
            }
            if (encontrado)
            {
                MessageBox.Show("Estudiante encontrado.");
            }
            else
            {
                MessageBox.Show("Estudiante no encontrado.");
            }
        }
        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if(DgvEstudiantes.SelectedRows.Count == 0)
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

            if (respuesta == DialogResult.Yes)
            {
                int fila = DgvEstudiantes.SelectedRows[0].Index;

                string codigo = DgvEstudiantes.Rows[fila]
                    .Cells[0].Value?.ToString();
                bool eliminado = gestorEstudiantes.EliminarEstudiante(codigo);

                if (eliminado)
                {
                    DgvEstudiantes.Rows.RemoveAt(fila);
                    MessageBox.Show(
                        "Estudiante eliminado correctamente.",
                        "Éxito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show(
                        "No se pudo eliminar al estudiante. No se encontró registro.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }   
        }
        private void BtnBurbujaEstudiante_Click(object sender, EventArgs e)
        {
            if (gestorEstudiantes.ObtenerEstudiantes().Count <= 1)
            {
                MessageBox.Show(
                    "No hay suficientes estudiantes para ordenar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            gestorEstudiantes.OrdenarBurbujaEstudiante();

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

            DgvEstudiantes.ClearSelection();
            DgvEstudiantes.CurrentCell = null;

            MessageBox.Show(
                "Estudiantes ordenados por apellidos (Burbuja).",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void BtnSeleccionEstudiante_Click(object sender, EventArgs e)
        {
            if(gestorEstudiantes.ObtenerEstudiantes().Count <= 1)
            {
                MessageBox.Show(
                    "No hay suficientes estudiantes para ordenar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            gestorEstudiantes.OrdenarSeleccionEstudiante();
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
            DgvEstudiantes.ClearSelection();
            DgvEstudiantes.CurrentCell = null;

            MessageBox.Show(
                "Estudiantes ordenados por apellidos (Selección).",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        //Botones de Libros
        private void BtnRegistrarLibro_Click(object sender, EventArgs e)
        {
            string titulo = TxtTituloLibro.Text.Trim();
            string area = TxtAreaLibro.Text.Trim();

            string resultado = gestorLibro.RegistrarLibro(titulo,area);

            bool registrado = resultado.StartsWith( "Libro registrado correctamente");

            MessageBox.Show(
                resultado,
                registrado ? "Éxito" : "Error",
                MessageBoxButtons.OK,
                registrado ? MessageBoxIcon.Information : MessageBoxIcon.Error
            );

            if(registrado)
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

                DgvLibros.ClearSelection();
                DgvLibros.CurrentCell = null;

                TxtTituloLibro.Clear();
                TxtAreaLibro.Clear();
            }
        }
        private void BtnMostrarLibros_Click(object sender, EventArgs e)
        {
            gestorLibro.RestaurarOrdenOriginal();
            MostrarLibros();

            MessageBox.Show(
                "Libros mostrados correctamente.",
                "Éxito",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void BtnBuscarLibro_Click(object sender, EventArgs e)
        {
            string titulo = TxtTituloLibro.Text.Trim();
            if (titulo == "")
            {
                MessageBox.Show("Ingrese el título del libro.");
                return;
            }
            DgvLibros.Rows.Clear();
            int encontrados = 0;
            foreach (Libro libro in gestorLibro.ObtenerLibros())
            {
                if (libro.Titulo.IndexOf(
                    titulo,
                    StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foreach (Ejemplar ejemplar in libro.ObtenerEjemplares())
                    {
                        DgvLibros.Rows.Add(
                            libro.CodigoLibro,
                            libro.Titulo,
                            libro.Area,
                            ejemplar.CodigoUL
                        );
                        encontrados++;
                    }
                }
            }
            DgvLibros.ClearSelection();
            DgvLibros.CurrentCell = null;
            if (encontrados == 0)
            {
                MessageBox.Show("No se encontraron libros.");
            }
            else
            {
                MessageBox.Show("Búsqueda realizada correctamente.");
            }
        }
        private void BtnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (DgvLibros.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione un libro para eliminar.");
                return;
            }
            string codigoUL = DgvLibros.SelectedRows[0].Cells[3].Value?.ToString();
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
            if(respuesta == DialogResult.Yes)
            {
                bool eliminado = gestorLibro.EliminarEjemplar(codigoUL);
                if (eliminado)
                {
                    DgvLibros.Rows.Remove(DgvLibros.SelectedRows[0]);
                    MessageBox.Show("Ejemplar eliminado correctamente.",
                        "Exito",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                else
                {
                    MessageBox.Show("No se encontró el ejemplar.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }
        private void MostrarLibros()
        {
            DgvLibros.Rows.Clear();

            if (gestorLibro.ObtenerLibros().Count == 0)
            {
                MessageBox.Show(
                    "No hay libros registrados.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
                return;
            }
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

            DgvLibros.ClearSelection();
            DgvLibros.CurrentCell = null;
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

            string resultado = gestorBiblioteca.RegistrarPrestamo(codigoEstudiante, codigoUL);
            
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

                DgvPrestamos.Rows.Clear();

                List<Prestamos> listaPrestamos =
                    gestorBiblioteca.ObtenerPrestamos();
                
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

                DgvPrestamos.ClearSelection();
                DgvPrestamos.CurrentCell = null;
            }
        }
        private void BtnMostrarPrestamo_Click(object sender, EventArgs e)
        {
            DgvPrestamos.Rows.Clear();

            List <Prestamos> listaPrestamos =
                gestorBiblioteca.ObtenerPrestamos();

            if(listaPrestamos.Count == 0)
            {
                MessageBox.Show(
                    "No hay préstamos registrados.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
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

            DgvPrestamos.ClearSelection();
            DgvPrestamos.CurrentCell = null;

        }
        private void BtnBuscarPrestamo_Click(object sender, EventArgs e)
        {
            DgvPrestamos.Rows.Clear();
            string codigoEstudiante = TxtCodigoEstPrestamo.Text.Trim();
            string codigoUL = TxtCodigoULPrestamo.Text.Trim();

            if(string.IsNullOrWhiteSpace(codigoEstudiante) &&
               string.IsNullOrWhiteSpace(codigoUL))
            {
                MessageBox.Show("Ingrese el código de estudiante o código UL.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            List<Prestamos> listaPrestamos;
            if(!string.IsNullOrWhiteSpace(codigoEstudiante))
            {
                listaPrestamos =
                    gestorBiblioteca.BuscarPrestamos(codigoEstudiante);
            }
            else
            {
                listaPrestamos =
                    gestorBiblioteca.BuscarPrestamos(codigoUL);
            }

            if(listaPrestamos.Count == 0)
            {
                MessageBox.Show(
                    "No se encontraron préstamos.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
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
            DgvPrestamos.ClearSelection();
            DgvPrestamos.CurrentCell = null;
        }

        //Botones de Devoluciones
        private void BtnRegistrarDevolucion_Click(object sender, EventArgs e)
        {
            string codigoUL = TxtCodigoULDevolucion.Text.Trim();
            string resultado =
                gestorBiblioteca.RegistrarDevolucion(codigoUL);

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

                Devoluciones devolucion =
                    historial[historial.Count - 1];
                TxtDevolucion.Text =
                    "    BOLETA DE DEVOLUCIÓN\r\n" +
                    "------------------------------ \r\n" +
                    "Código del estudiante: "
                    + devolucion.CodigoEstudiante + "\r\n" +
                    "Código UL del libro: "
                    + devolucion.CodigoUlLibro + "\r\n" +
                    "Título del libro: "
                    + devolucion.TituloLibro + "\r\n" +
                    "Área del libro: "
                    + devolucion.AreaLibro + "\r\n" +
                    "Fecha de préstamo: "
                    + devolucion.FechaPrestamo.ToString("dd/MM/yyyy") + "\r\n" +
                    "Fecha de devolución prevista: "
                    + devolucion.FechaDevolucionPrevista.ToString("dd/MM/yyyy") + "\r\n" +
                    "Fecha de devolución real: "
                    + devolucion.FechaDevolucionReal.ToString("dd/MM/yyyy") + "\r\n" +
                    "Estado: "
                    + devolucion.EstadoDevolucion + "\r\n" +
                    "------------------------------";
            }
        }
        private void BtnHistorialDevoluciones_Click(object sender, EventArgs e)
        {
            List<Devoluciones> historial =
                gestorBiblioteca.ObtenerDevoluciones();
           
            TxtDevolucion.Clear();

            if(historial.Count == 0)
            {
                TxtDevolucion.Text =
                    "No hay devoluciones registradas.";
                return;
            }

            foreach (Devoluciones devolucion in historial)
            {
                TxtDevolucion.AppendText(
                    "      BOLETA DE DEVOLUCIÓN\r\n" +
                    "------------------------------ \r\n" +
                    "Código del estudiante: " +
                    devolucion.CodigoEstudiante + "\r\n" +
                    "Código UL del libro: " +
                    devolucion.CodigoUlLibro + "\r\n" +
                    "Título del libro: " +
                    devolucion.TituloLibro + "\r\n" +
                    "Área del libro: " +
                    devolucion.AreaLibro + "\r\n" +
                    "Fecha de préstamo: " +
                    devolucion.FechaPrestamo.ToString("dd/MM/yyyy") + "\r\n" +
                    "Fecha de devolución prevista: " +
                    devolucion.FechaDevolucionPrevista.ToString("dd/MM/yyyy") + "\r\n" +
                    "Fecha de devolución real: " +
                    devolucion.FechaDevolucionReal.ToString("dd/MM/yyyy") + "\r\n" +
                    "Estado: " +
                    devolucion.EstadoDevolucion + "\r\n" +
                    "------------------------------\r\n\r\n"
                );
            }
        }

        
    }
}
