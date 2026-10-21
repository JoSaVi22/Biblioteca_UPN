using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Biblioteca_UPN
{
    public partial class Form1 : Form
    {
        private void RedondearBoton(Button boton, int radio)
        {
            GraphicsPath ruta = new GraphicsPath();

            int diametro = radio * 2;
            int ancho = boton.Width;
            int alto = boton.Height;

            ruta.AddArc(0, 0, diametro, diametro, 180, 90);
            ruta.AddArc(ancho - diametro, 0, diametro, diametro, 270, 90);
            ruta.AddArc(ancho - diametro, alto - diametro,
                        diametro, diametro, 0, 90);
            ruta.AddArc(0, alto - diametro, diametro, diametro, 90, 90);

            ruta.CloseFigure();

            Region anterior = boton.Region;
            boton.Region = new Region(ruta);

            if (anterior != null)
                anterior.Dispose();

            ruta.Dispose();
        }
        private void EstilizarBotonMenu(Button boton, bool activo)
        {
            Color amarillo = Color.FromArgb(255, 198, 41);
            Color oscuro = Color.FromArgb(25, 27, 31);

            boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = amarillo;
            boton.FlatAppearance.MouseDownBackColor = amarillo;

            boton.UseVisualStyleBackColor = false;
            boton.Cursor = Cursors.Hand;
            boton.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            // Todos los botones empiezan oscuros
            boton.BackColor = oscuro;
            boton.ForeColor = Color.White;

            // Bordes redondeados
            RedondearBoton(boton, 10);

            // Cuando el cursor pasa encima
            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = amarillo;
                boton.ForeColor = oscuro;
            };

            // Cuando el cursor se retira
            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = oscuro;
                boton.ForeColor = Color.White;
            };
        }
        private GestorBiblioteca gestor = new GestorBiblioteca();
        private Color fondo = Color.FromArgb(243, 246, 250);

        public Form1()
        {
            InitializeComponent();
            tabContenido.SelectedTab = tabBienvenida;
            StartPosition = FormStartPosition.CenterScreen;

            EstilizarBotonMenu(btnEstudiantes, true);
            EstilizarBotonMenu(btnLibros, false);
            EstilizarBotonMenu(btnPrestamos, false);
            EstilizarBotonMenu(btnConsultas, false);
            EstilizarBotonMenu(btnDevoluciones, false);


            // Al entrar, ninguna tabla estará visible.
            dgvEstudinateConsulta.Visible = false;
            dgvLibrosConsulta.Visible = false;

            tabContenido.Appearance = TabAppearance.FlatButtons;
            tabContenido.ItemSize = new Size(0, 1);
            tabContenido.SizeMode = TabSizeMode.Fixed;

            gestor.CargarLibrosIniciales();
            // DISEÑO DE CAMPOS DE ESTUDIANTES
            EstilizarCampo(txtCodigoEstudiante);
            EstilizarCampo(txtNombreEstudiante);
            EstilizarCampo(txtApellidosEstudiante);
            // DISEÑO DEL COMBOBOX

            cboEstadoMatricula.Font =
                new Font("Segoe UI", 10);

            cboEstadoMatricula.BackColor = Color.White;
            cboEstadoMatricula.ForeColor =
                Color.FromArgb(35, 38, 43);

            cboEstadoMatricula.FlatStyle =
                FlatStyle.Flat;
        }

        private void EstilizarCampo(TextBox campo)
        {
            campo.Font = new Font("Segoe UI", 10);
            campo.BackColor = Color.White;
            campo.ForeColor = Color.FromArgb(35, 38, 43);
            campo.BorderStyle = BorderStyle.FixedSingle;

            campo.Enter += (s, e) =>
            {
                campo.BackColor = Color.FromArgb(255, 248, 224);
            };

            campo.Leave += (s, e) =>
            {
                campo.BackColor = Color.White;
            };
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x1328)
            {
                m.Result = (IntPtr)1;
                return;
            }

            base.WndProc(ref m);
        }
        private bool ValidarCodigoEstudiante(string codigo)
        {
            if (codigo.Length != 9)
            {
                return false;
            }

            if (!codigo.StartsWith("N00"))
            {
                return false;
            }

            for (int i = 3; i < codigo.Length; i++)
            {
                if (codigo[i] < '0' || codigo[i] > '9')
                {
                    return false;
                }
            }
            return true;
        }
        private bool ValidarNombreApellido(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            foreach (char letra in texto)
            {
                if (!char.IsLetter(letra) && letra != ' ')
                {
                    return false;
                }
            }

            return true;
        }
        private void btnRegistrarEstudiante_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigoEstudiante.Text;
            string nombre = txtNombreEstudiante.Text;
            string apellidos = txtApellidosEstudiante.Text;

            if (!ValidarCodigoEstudiante(codigo))
            {
                MessageBox.Show(
                    "El código debe comenzar con N00 y tener 6 dígitos adicionales.",
                    "Código inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!ValidarNombreApellido(nombre))
            {
                MessageBox.Show(
                    "El nombre debe contener solo letras.",
                    "Nombre inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!ValidarNombreApellido(apellidos))
            {
                MessageBox.Show(
                    "El apellido debe contener solo letras.",
                    "Apellido inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            string estado = cboEstadoMatricula.Text;

            string mensaje = gestor.RegistrarEstudiante(
                codigo, nombre, apellidos, estado);

            MessageBox.Show(mensaje, "Registro exitoso");

            if (mensaje == "Estudiante registrado correctamente y guardado.")
            {
                txtCodigoEstudiante.Clear();
                txtNombreEstudiante.Clear();
                txtApellidosEstudiante.Clear();
                cboEstadoMatricula.SelectedIndex = -1;
            }
        }

        private void btnMostrarEstudiantes_Click(object sender, EventArgs e)
        {
            MostrarEstudiantesPorCodigo();
        }
        private void MostrarEstudiantesPorCodigo()
        {
            int cantidad = gestor.ObtenerCantidadEstudiantes();

            dgvEstudiantes.Rows.Clear();

            if (cantidad == 0)
            {
                MessageBox.Show("No hay estudiantes registrados.");
                return;
            }

            Estudiante[] copia = new Estudiante[cantidad];

            for (int i = 0; i < cantidad; i++)
            {
                copia[i] = gestor.ObtenerEstudiantePorIndice(i);
            }

            Array.Sort(copia, (a, b) =>
                string.Compare(
                    a.Codigo,
                    b.Codigo,
                    StringComparison.OrdinalIgnoreCase));

            for (int i = 0; i < cantidad; i++)
            {
                Estudiante estudiante = copia[i];

                dgvEstudiantes.Rows.Add(
                    estudiante.Codigo,
                    estudiante.Nombre,
                    estudiante.Apellidos,
                    estudiante.EstadoMatricula
                );
            }
        }

        private void MostrarEstudiantesEnTabla()
        {
            dgvEstudiantes.Rows.Clear();

            int cantidad = gestor.ObtenerCantidadEstudiantes();

            if (cantidad == 0)
            {
                MessageBox.Show("No hay estudiantes registrados.");
                return;
            }

            for (int i = 0; i < cantidad; i++)
            {
                Estudiante estudiante = gestor.ObtenerEstudiantePorIndice(i);

                dgvEstudiantes.Rows.Add(
                    estudiante.Codigo,
                    estudiante.Nombre,
                    estudiante.Apellidos,
                    estudiante.EstadoMatricula
                );
            }
        }
        private void btnBuscarEstudiante_Click(object sender, EventArgs e)
        {
            string codigo = txtCodigoEstudiante.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigo))
            {
                MessageBox.Show("Ingrese el código del estudiante.");
                txtCodigoEstudiante.Focus();
                return;
            }

            Estudiante estudiante = gestor.BuscarEstudiante(codigo);

            if (estudiante == null)
            {
                MessageBox.Show("No se encontró al estudiante.");
                return;
            }

            txtNombreEstudiante.Text = estudiante.Nombre;
            txtApellidosEstudiante.Text = estudiante.Apellidos;
            cboEstadoMatricula.Text = estudiante.EstadoMatricula;

            MessageBox.Show("Estudiante encontrado correctamente.");
        }

        private void btnEstudiantes_Click(object sender, EventArgs e)
        {
            tabContenido.SelectedTab = tabEstudiantes;
        }

        private void btnLibros_Click(object sender, EventArgs e)
        {
            tabContenido.SelectedTab = tabLibros;
        }

        private void btnPrestamos_Click(object sender, EventArgs e)
        {
            tabContenido.SelectedTab = tabPrestamos;
        }

        private void btnConsultas_Click(object sender, EventArgs e)
        {
            tabContenido.SelectedTab = tabConsultas;
        }

        private void btnDevoluciones_Click(object sender, EventArgs e)
        {
            tabContenido.SelectedTab = tabDevoluciones;
        }


        private bool ValidarCodigoUL(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return false;

            if (codigoUL.Length != 8)
                return false;

            if (!codigoUL.StartsWith("UL"))
                return false;

            for (int i = 2; i < 8; i++)
            {
                if (!char.IsDigit(codigoUL[i]))
                    return false;
            }

            return true;
        }
        private void btnRegistrarLibro_Click(object sender, EventArgs e)
        {
            string titulo = txtTituloLibro.Text.Trim();
            string area = txtAreaLibro.Text.Trim();
            string codigoUL = txtCodigoUL.Text.Trim();
            
            if (!ValidarCodigoUL(txtCodigoUL.Text))
            {
                MessageBox.Show("El código UL debe empezar con UL y contener 6 dígitos después.");
                return;
            }

            string mensaje = gestor.RegistrarLibro(
                "", titulo, area, codigoUL);

            MessageBox.Show(mensaje);

            if (mensaje.Contains("correctamente"))
            {
                txtTituloLibro.Clear();
                txtAreaLibro.Clear();
                txtCodigoUL.Clear();

                // Actualizar automáticamente la tabla.
                dgvLibros.Rows.Clear();

                int cantidad = gestor.ObtenerCantidadLibros();

                for (int i = 0; i < cantidad; i++)
                {
                    Libro libro = gestor.ObtenerLibroPorIndice(i);

                    for (int j = 0;
                         j < libro.ObtenerCantidadEjemplares();
                         j++)
                    {
                        Ejemplar ejemplar =
                            libro.ObtenerEjemplarPorIndice(j);

                        dgvLibros.Rows.Add(
                            libro.CodigoLibro,
                            libro.Titulo,
                            libro.Area,
                            ejemplar.CodigoUL
                        );
                    }
                }

                dgvLibros.ClearSelection();
            }
        }

        private void btnMostrarLibros_Click(object sender, EventArgs e)
        {
            int cantidad = gestor.ObtenerCantidadLibros();

            if (cantidad == 0)
            {
                dgvLibros.Rows.Clear();
                MessageBox.Show("No hay libros registrados.");
                return;
            }

            // Mostrar los libros ordenados por su código
            MostrarLibrosPorCodigo();
        }
        private void MostrarLibrosEnTabla()
        {
            dgvLibros.Rows.Clear();

            int cantidad = gestor.ObtenerCantidadLibros();

            if (cantidad == 0)
            {
                MessageBox.Show("No hay libros registrados.");
                return;
            }

            for (int i = 0; i < cantidad; i++)
            {
                Libro libro = gestor.ObtenerLibroPorIndice(i);

                for (int j = 0; j < libro.ObtenerCantidadEjemplares(); j++)
                {
                    Ejemplar ejemplar = libro.ObtenerEjemplarPorIndice(j);

                    dgvLibros.Rows.Add(
                        libro.CodigoLibro,
                        libro.Titulo,
                        libro.Area,
                        ejemplar.CodigoUL
                    );
                }
            }
        }
        private void btnBuscarLibro_Click(object sender, EventArgs e)
        {
            string codigoUL = txtCodigoUL.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoUL))
            {
                MessageBox.Show("Ingrese el código UL del ejemplar.");
                txtCodigoUL.Focus();
                return;
            }

            Libro libro = gestor.BuscarLibroPorUL(codigoUL);

            if (libro == null)
            {
                MessageBox.Show("No se encontró ningún libro con ese código UL.");
                return;
            }

            dgvLibros.Rows.Clear();

            Ejemplar ejemplar = libro.BuscarEjemplar(codigoUL);

            if (ejemplar != null)
            {
                dgvLibros.Rows.Add(
                    libro.CodigoLibro,
                    libro.Titulo,
                    libro.Area,
                    ejemplar.CodigoUL
                );

                MessageBox.Show("Libro encontrado correctamente.");
            }
        }

        private void btnRegistrarPrestamo_Click(object sender, EventArgs e)
        {
            string codigoEstudiante = txtCodigoEstudiantePrestamo.Text.Trim();
            string codigoUL = txtCodigoULPrestamo.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoEstudiante) ||
                string.IsNullOrWhiteSpace(codigoUL))
            {
                MessageBox.Show("Ingrese el código del estudiante y el código UL.");
                return;
            }

            string mensaje = gestor.RegistrarPrestamo(
                codigoEstudiante, codigoUL);

            MessageBox.Show(mensaje);

            if (mensaje.Contains("correctamente"))
            {
                txtCodigoEstudiantePrestamo.Clear();
                txtCodigoULPrestamo.Clear();
            }
        }

        private void btnMostrarPrestamos_Click(object sender, EventArgs e)
        {
            dgvPrestamos.Rows.Clear();

            for (int i = 0; i < gestor.ObtenerCantidadPrestamos(); i++)
            {
                Prestamo prestamo = gestor.ObtenerPrestamoPorIndice(i);

                if (prestamo != null && prestamo.Activo)
                {
                    dgvPrestamos.Rows.Add(
                        prestamo.CodigoEstudiante,
                        prestamo.CodigoUL,
                        prestamo.FechaPrestamo.ToString("dd/MM/yyyy"),
                        prestamo.FechaDevolucion.ToString("dd/MM/yyyy"),
                        "Pendiente"
                    );
                }
            }

            if (dgvPrestamos.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No hay préstamos activos.",
                    "Préstamos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }

        private void btnVerHistorial_Click(object sender, EventArgs e)
        {
            dgvPrestamos.Rows.Clear();

            for (int i = 0; i < gestor.ObtenerCantidadPrestamos(); i++)
            {
                Prestamo prestamo = gestor.ObtenerPrestamoPorIndice(i);

                if (prestamo != null && !prestamo.Activo)
                {
                    dgvPrestamos.Rows.Add(
                        prestamo.CodigoEstudiante,
                        prestamo.CodigoUL,
                        prestamo.FechaPrestamo.ToString("dd/MM/yyyy"),
                        prestamo.FechaDevolucion.ToString("dd/MM/yyyy"),
                        "Devuelto"
                    );
                }
            }

            if (dgvPrestamos.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Todavía no hay préstamos devueltos.",
                    "Historial de préstamos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        private void btnConsultarEstudiante_Click(object sender, EventArgs e)
        {
            dgvLibrosConsulta.Visible = false;
            dgvEstudinateConsulta.Visible = true;
            dgvEstudinateConsulta.BringToFront();
            dgvEstudinateConsulta.Rows.Clear();

            string dato = txtConsultaEstudiante.Text.Trim();

            if (dato == "")
            {
                MessageBox.Show("Ingrese el código o los apellidos del estudiante.");
                return;
            }

            // Buscar por código
            if (rdbBuscarCodigo.Checked)
            {
                string resultado = gestor.ConsultarEstudiante(dato);

                if (resultado == "No se encontró al estudiante.")
                {
                    MessageBox.Show(resultado);
                    return;
                }

                string[] datos = resultado.Split('\n');

                string codigoEstudiante = datos[0].Replace("Código: ", "").Trim();
                string nombre = datos[1].Replace("Nombre: ", "").Trim();
                string apellidos = datos[2].Replace("Apellidos: ", "").Trim();
                string matricula = datos[3].Replace("Matrícula: ", "").Trim();

                dgvEstudinateConsulta.Rows.Add(
                    codigoEstudiante,
                    nombre,
                    apellidos,
                    matricula
                );
            }
            // Buscar por apellidos
            else if (rdbBuscarApellidos.Checked)
            {
                int cantidad = gestor.ObtenerCantidadEstudiantes();
                Estudiante[] encontrados = new Estudiante[cantidad];

                int totalEncontrados =
                    gestor.BuscarEstudiantesPorApellidos(dato, encontrados);

                if (totalEncontrados == 0)
                {
                    MessageBox.Show("No se encontraron estudiantes con esos apellidos.");
                    return;
                }

                for (int i = 0; i < totalEncontrados; i++)
                {
                    Estudiante estudiante = encontrados[i];

                    dgvEstudinateConsulta.Rows.Add(
                        estudiante.Codigo,
                        estudiante.Nombre,
                        estudiante.Apellidos,
                        estudiante.EstadoMatricula
                    );
                }
            }
        }

        private void btnConsultarLibro_Click(object sender, EventArgs e)
        {
            dgvEstudinateConsulta.Visible = false;
            dgvLibrosConsulta.Visible = true;
            dgvLibrosConsulta.BringToFront();

            // Limpiar resultados anteriores
            dgvLibrosConsulta.Rows.Clear();

            // Obtener el código UL ingresado
            string codigoUL = txtConsultaUL.Text.Trim();

            if (codigoUL == "")
            {
                MessageBox.Show("Ingrese el código UL del ejemplar.");
                return;
            }

            // Consultar el libro
            string resultado = gestor.ConsultarLibro(codigoUL);

            if (resultado == "No se encontró el ejemplar.")
            {
                MessageBox.Show(resultado);
                return;
            }

            // Separar los datos de la consulta
            string[] datos = resultado.Split('\n');

            string codigoLibro = datos[0]
                .Replace("Código del libro: ", "").Trim();

            string titulo = datos[1]
                .Replace("Título: ", "").Trim();

            string area = datos[2]
                .Replace("Área: ", "").Trim();

            string codigoEjemplar = datos[3]
                .Replace("Código UL: ", "").Trim();

            string disponibilidad = datos[4]
                .Replace("Disponibilidad: ", "").Trim();

            // Agregar una fila a la tabla
            dgvLibrosConsulta.Rows.Add(
                codigoLibro,
                titulo,
                area,
                codigoEjemplar,
                disponibilidad
            );
        }

        private void btnRegistrarDevolucion_Click(object sender, EventArgs e)
        {
            string codigoUL = txtCodigoULDevolucion.Text.Trim();

            if (string.IsNullOrWhiteSpace(codigoUL))
            {
                txtResultadoDevolucion.Text =
                    "Ingrese el código UL del ejemplar.";
                return;
            }

            Libro libro = gestor.BuscarLibroPorUL(codigoUL);
            Prestamo prestamo = null;

            for (int i = 0; i < gestor.ObtenerCantidadPrestamos(); i++)
            {
                Prestamo actual = gestor.ObtenerPrestamoPorIndice(i);

                if (actual != null &&
                    actual.Activo &&
                    string.Equals(
                        actual.CodigoUL,
                        codigoUL,
                        StringComparison.OrdinalIgnoreCase))
                {
                    prestamo = actual;
                    break;
                }
            }

            if (prestamo == null)
            {
                txtResultadoDevolucion.Text =
                    gestor.DevolverLibro(codigoUL);
                return;
            }

            Estudiante estudiante =
                gestor.BuscarEstudiante(prestamo.CodigoEstudiante);

            string mensaje = gestor.DevolverLibro(codigoUL);

            if (mensaje.Contains("correctamente"))
            {
                string nombreEstudiante = estudiante != null
                    ? estudiante.Nombre + " " + estudiante.Apellidos
                    : "No disponible";

                txtResultadoDevolucion.Text =
                    "DEVOLUCIÓN REGISTRADA CORRECTAMENTE" +
                    Environment.NewLine +
                    "----------------------------------" +
                    Environment.NewLine +
                    "DATOS DEL LIBRO" + Environment.NewLine +
                    "Código del libro: " + libro.CodigoLibro +
                    Environment.NewLine +
                    "Título: " + libro.Titulo +
                    Environment.NewLine +
                    "Área: " + libro.Area +
                    Environment.NewLine +
                    "Código UL: " + prestamo.CodigoUL +
                    Environment.NewLine + Environment.NewLine +
                    "DATOS DEL PRÉSTAMO" + Environment.NewLine +
                    "Código del estudiante: " +
                    prestamo.CodigoEstudiante +
                    Environment.NewLine +
                    "Nombre del estudiante: " + nombreEstudiante +
                    Environment.NewLine +
                    "Fecha del préstamo: " +
                    prestamo.FechaPrestamo.ToString("dd/MM/yyyy") +
                    Environment.NewLine +
                    "Fecha prevista de devolución: " +
                    prestamo.FechaDevolucion.ToString("dd/MM/yyyy") +
                    Environment.NewLine +
                    "Estado: Devuelto";

                txtCodigoULDevolucion.Clear();
            }
            else
            {
                txtResultadoDevolucion.Text = mensaje;
            }
        }

        private void btnEliminarLibro_Click(object sender, EventArgs e)
        {
            if (dgvLibros.CurrentRow == null ||
        dgvLibros.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Seleccione un libro de la tabla.");
                return;
            }

            object valor = dgvLibros.CurrentRow.Cells[0].Value;

            if (valor == null)
            {
                MessageBox.Show("No se pudo obtener el código del libro.");
                return;
            }

            string codigo = valor.ToString();

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar el libro con código " + codigo + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            string mensaje = gestor.EliminarLibro(codigo);

            MessageBox.Show(mensaje);

            if (mensaje == "Libro eliminado correctamente.")
            {
                dgvLibros.Rows.Clear();

                int cantidad = gestor.ObtenerCantidadLibros();

                for (int i = 0; i < cantidad; i++)
                {
                    Libro libro = gestor.ObtenerLibroPorIndice(i);

                    for (int j = 0;
                         j < libro.ObtenerCantidadEjemplares();
                         j++)
                    {
                        Ejemplar ejemplar =
                            libro.ObtenerEjemplarPorIndice(j);

                        dgvLibros.Rows.Add(
                            libro.CodigoLibro,
                            libro.Titulo,
                            libro.Area,
                            ejemplar.CodigoUL
                        );
                    }
                }

                dgvLibros.ClearSelection();
            }
        }

        private void btnEliminarEstudiante_Click(object sender, EventArgs e)
        {
            if (dgvEstudiantes.CurrentRow == null ||
                dgvEstudiantes.CurrentRow.IsNewRow)
            {
                MessageBox.Show("Seleccione un estudiante de la tabla.");
                return;
            }

            object valor = dgvEstudiantes.CurrentRow.Cells[0].Value;

            if (valor == null)
            {
                MessageBox.Show("Seleccione un estudiante válido.");
                return;
            }

            string codigo = valor.ToString();

            DialogResult respuesta = MessageBox.Show(
                "¿Está seguro de eliminar al estudiante con código "
                + codigo + "?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (respuesta != DialogResult.Yes)
                return;

            string mensaje = gestor.EliminarEstudiante(codigo);

            MessageBox.Show(mensaje);

            if (mensaje.Contains("correctamente"))
            {
                btnMostrarEstudiantes_Click(sender, e);
            }
        }

        private void btnOrdenarBurbuja_Click(object sender, EventArgs e)
        {
            gestor.OrdenarBurbujaEstudiantes();
            MostrarEstudiantesEnTabla();
        }

        private void btnOrdenarSeleccion_Click(object sender, EventArgs e)
        {
            gestor.OrdenarSeleccionEstudiantes();
            MostrarEstudiantesEnTabla();
        }

        private void btnOrdenarBurbujaLibros_Click(object sender, EventArgs e)
        {
            gestor.OrdenarBurbujaLibros();
            MostrarLibrosEnTabla();
        }
        private void btnOrdenarSeleccionLibros_Click(object sender, EventArgs e)
        {
            gestor.OrdenarSeleccionLibros();
            MostrarLibrosEnTabla();
        }

        private void picLogoUPN_Click(object sender, EventArgs e)
        {
            tabContenido.SelectedTab = tabBienvenida;
        }
        private void MostrarLibrosPorCodigo()
        {
            int cantidad = gestor.ObtenerCantidadLibros();

            Libro[] copia = new Libro[cantidad];

            for (int i = 0; i < cantidad; i++)
            {
                copia[i] = gestor.ObtenerLibroPorIndice(i);
            }

            Array.Sort(copia, (a, b) =>
                string.Compare(
                    a.CodigoLibro,
                    b.CodigoLibro,
                    StringComparison.OrdinalIgnoreCase));

            dgvLibros.Rows.Clear();

            foreach (Libro libro in copia)
            {
                for (int j = 0; j < libro.ObtenerCantidadEjemplares(); j++)
                {
                    Ejemplar ejemplar = libro.ObtenerEjemplarPorIndice(j);

                    dgvLibros.Rows.Add(
                        libro.CodigoLibro,
                        libro.Titulo,
                        libro.Area,
                        ejemplar.CodigoUL
                    );
                }
            }
        }
    }
}