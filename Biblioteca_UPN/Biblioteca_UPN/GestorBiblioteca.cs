using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_UPN
{
    public class GestorBiblioteca
    {
        private Estudiante[] estudiantes;
        private Libro[] libros;
        private Prestamo[] prestamos;

        private int cantidadEstudiantes;
        private int cantidadLibros;
        private int cantidadPrestamos;

        private ArchivoBiblioteca archivo;

        public GestorBiblioteca()
        {
            estudiantes = new Estudiante[10];
            libros = new Libro[10];
            prestamos = new Prestamo[10];

            cantidadEstudiantes = 0;
            cantidadLibros = 0;
            cantidadPrestamos = 0;

            archivo = new ArchivoBiblioteca();

            CargarDatos();
        }

        // AUMENTAR LA CAPACIDAD DE LOS ARREGLOS
        private void AsegurarCapacidadEstudiantes()
        {
            if (cantidadEstudiantes >= estudiantes.Length)
            {
                Estudiante[] nuevo =
                    new Estudiante[estudiantes.Length * 2];

                Array.Copy(estudiantes, nuevo, estudiantes.Length);
                estudiantes = nuevo;
            }
        }
        private void AsegurarCapacidadLibros()
        {
            if (cantidadLibros >= libros.Length)
            {
                Libro[] nuevo = new Libro[libros.Length * 2];

                Array.Copy(libros, nuevo, libros.Length);
                libros = nuevo;
            }
        }
        private void AsegurarCapacidadPrestamos()
        {
            if (cantidadPrestamos >= prestamos.Length)
            {
                Prestamo[] nuevo =
                    new Prestamo[prestamos.Length * 2];

                Array.Copy(prestamos, nuevo, prestamos.Length);
                prestamos = nuevo;
            }
        }

        // REGISTRAR ESTUDIANTES
        public string RegistrarEstudiante(
            string codigo, string nombre,
            string apellidos, string estadoMatricula)
        {
            if (string.IsNullOrWhiteSpace(codigo) ||
                string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(estadoMatricula))
            {
                return "Complete todos los datos del estudiante.";
            }

            codigo = codigo.Trim();
            nombre = nombre.Trim();
            apellidos = apellidos.Trim();
            estadoMatricula = estadoMatricula.Trim().ToUpper();

            Estudiante nuevo = new Estudiante(
                codigo, nombre, apellidos, estadoMatricula);

            if (!nuevo.ValidarCodigo())
                return "El código del estudiante no es válido.";

            if (estadoMatricula != "ACTIVO" &&
                estadoMatricula != "INACTIVO")
            {
                return "La matrícula debe ser ACTIVO o INACTIVO.";
            }

            if (BuscarEstudiante(codigo) != null)
                return "El código del estudiante ya existe.";

            AsegurarCapacidadEstudiantes();

            estudiantes[cantidadEstudiantes] = nuevo;
            cantidadEstudiantes++;

            GuardarEstudiantes();

            return "Estudiante registrado correctamente y guardado.";
        }
        public Estudiante BuscarEstudiante(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            for (int i = 0; i < cantidadEstudiantes; i++)
            {
                if (string.Equals(
                    estudiantes[i].Codigo, codigo.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return estudiantes[i];
                }
            }
            return null;
        }
        public int BuscarEstudiantesPorApellidos(
            string apellidos,
            Estudiante[] encontrados)
        {
            int cantidadEncontrados = 0;

            if (string.IsNullOrWhiteSpace(apellidos))
                return 0;

            for (int i = 0; i < cantidadEstudiantes; i++)
            {
                if (estudiantes[i].Apellidos.IndexOf(
                    apellidos.Trim(),
                    StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    encontrados[cantidadEncontrados] = estudiantes[i];
                    cantidadEncontrados++;
                }
            }

            return cantidadEncontrados;
        }
        public string EliminarEstudiante(string codigo)
        {
            for (int i = 0; i < cantidadEstudiantes; i++)
            {
                if (estudiantes[i].Codigo == codigo)
                {
                    // Mover los estudiantes para cubrir el espacio.
                    for (int j = i; j < cantidadEstudiantes - 1; j++)
                    {
                        estudiantes[j] = estudiantes[j + 1];
                    }

                    // Limpiar la última posición.
                    estudiantes[cantidadEstudiantes - 1] = null;
                    cantidadEstudiantes--;

                    // Guardar los cambios en el archivo.
                    GuardarEstudiantes();

                    return "Estudiante eliminado correctamente.";
                }
            }
            return "No se encontró el estudiante seleccionado.";
        }

        public void OrdenarBurbujaEstudiantes()
        {
            for (int i = 0; i < cantidadEstudiantes - 1; i++)
            {
                for (int j = 0; j < cantidadEstudiantes - 1 - i; j++)
                {
                    if (string.Compare(
                        estudiantes[j].Apellidos,
                        estudiantes[j + 1].Apellidos,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        Estudiante temporal = estudiantes[j];
                        estudiantes[j] = estudiantes[j + 1];
                        estudiantes[j + 1] = temporal;
                    }
                }
            }
            GuardarEstudiantes();
        }
        public void OrdenarSeleccionEstudiantes()
        {
            for (int i = 0; i < cantidadEstudiantes - 1; i++)
            {
                int indiceMenor = i;

                for (int j = i + 1; j < cantidadEstudiantes; j++)
                {
                    if (string.Compare(
                        estudiantes[j].Apellidos,
                        estudiantes[indiceMenor].Apellidos,
                        StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        indiceMenor = j;
                    }
                }

                if (indiceMenor != i)
                {
                    Estudiante temporal = estudiantes[i];
                    estudiantes[i] = estudiantes[indiceMenor];
                    estudiantes[indiceMenor] = temporal;
                }
            }
            GuardarEstudiantes();
        }


        // REGISTRAR LIBROS Y EJEMPLARES
        public string RegistrarLibro(string codigoLibro, string titulo, string area, string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(area) ||
                string.IsNullOrWhiteSpace(codigoUL))
            {
                return "Complete todos los datos del libro.";
            }

            titulo = titulo.Trim();
            area = area.Trim();
            codigoUL = codigoUL.Trim();

            // Verificar que el código UL no esté registrado.
            if (BuscarLibroPorUL(codigoUL) != null)
                return "El código UL ya está registrado.";

            // Buscar si el título ya existe en la misma área.
            for (int i = 0; i < cantidadLibros; i++)
            {
                if (string.Equals(
                        libros[i].Titulo,
                        titulo,
                        StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(
                        libros[i].Area,
                        area,
                        StringComparison.OrdinalIgnoreCase))
                {
                    if (!libros[i].AgregarEjemplar(codigoUL))
                        return "No se pudo registrar el ejemplar.";

                    GuardarLibros();

                    return "Ejemplar agregado al libro existente correctamente.";
                }
            }


            // Generar el siguiente código correlativo.
            int numero = 1;

            for (int i = 0; i < cantidadLibros; i++)
            {
                string codigoExistente = libros[i].CodigoLibro;

                if (codigoExistente.Length == 4 &&
                    codigoExistente.StartsWith("L") &&
                    int.TryParse(codigoExistente.Substring(1), out int valor))
                {
                    if (valor >= numero)
                        numero = valor + 1;
                }
            }

            string nuevoCodigo = "L" + numero.ToString("D3");

            // Crear un nuevo título con su primer ejemplar.
            Libro nuevo = new Libro(nuevoCodigo, titulo, area);

            if (!nuevo.AgregarEjemplar(codigoUL))
                return "No se pudo registrar el ejemplar.";

            AsegurarCapacidadLibros();

            libros[cantidadLibros] = nuevo;
            cantidadLibros++;

            GuardarLibros();

            return "Libro y ejemplar registrados y guardados correctamente. Código: "
                + nuevoCodigo;
        }
        public void CargarLibrosIniciales()
        {
            // Si ya existen libros guardados, no volver a cargarlos.
            if (cantidadLibros > 0)
                return;

            string[] titulos =
            {
        "Calculo diferencial",
        "Algebra lineal",
        "Programacion en Python",
        "Anatomia humana",
        "Salud Mental",
        "Fundamentos de enfermeria",
        "Administracion de empresas",
        "Contabilidad financiera",
        "Derecho civil",
        "Derecho constitucional",
        "Diseno arquitectonico",
        "Estadistica basica"
    };

            string[] areas =
            {
        "Ingenieria",
        "Ingenieria",
        "Ingenieria",
        "Salud",
        "Salud",
        "Salud",
        "Negocios",
        "Negocios",
        "Derecho",
        "Derecho",
        "Arquitectura",
        "General"
    };

            for (int i = 0; i < titulos.Length; i++)
            {
                for (int j = 1; j <= 4; j++)
                {
                    string codigoUL =
                        "UL" + (i * 4 + j).ToString("D6");

                    RegistrarLibro(
                        "",
                        titulos[i],
                        areas[i],
                        codigoUL
                    );
                }
            }
        }
        public int ObtenerCantidadLibros()
        {
            return cantidadLibros;
        }
        public Libro ObtenerLibroPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidadLibros)
            {
                return null;
            }

            return libros[indice];
        }
        public string EliminarLibro(string codigoLibro)
        {
            for (int i = 0; i < cantidadLibros; i++)
            {
                if (libros[i].CodigoLibro == codigoLibro)
                {
                    for (int j = i; j < cantidadLibros - 1; j++)
                    {
                        libros[j] = libros[j + 1];
                    }

                    libros[cantidadLibros - 1] = null;
                    cantidadLibros--;

                    GuardarLibros();

                    return "Libro eliminado correctamente.";
                }
            }
            return "No se encontró el libro seleccionado.";
        }

        public string AgregarEjemplar(string codigoLibro, string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoLibro) ||
                string.IsNullOrWhiteSpace(codigoUL))
            {
                return "Ingrese el código del libro y el código UL.";
            }

            codigoUL = codigoUL.Trim();

            if (BuscarLibroPorUL(codigoUL) != null)
                return "El código UL ya está registrado.";

            Libro libro = BuscarLibro(codigoLibro);

            if (libro == null)
                return "No se encontró el libro.";

            if (libro.AgregarEjemplar(codigoUL))
            {
                GuardarLibros();
                return "Ejemplar agregado y guardado correctamente.";
            }
            return "No se pudo agregar el ejemplar.";
        }

        public Libro BuscarLibro(string codigoLibro)
        {
            if (string.IsNullOrWhiteSpace(codigoLibro))
                return null;

            for (int i = 0; i < cantidadLibros; i++)
            {
                if (string.Equals(
                    libros[i].CodigoLibro, codigoLibro.Trim(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return libros[i];
                }
            }
            return null;
        }

        public Libro BuscarLibroPorUL(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return null;

            for (int i = 0; i < cantidadLibros; i++)
            {
                if (libros[i].BuscarEjemplar(codigoUL.Trim()) != null)
                    return libros[i];
            }
            return null;
        }
        public void OrdenarBurbujaLibros()
        {
            for (int i = 0; i < cantidadLibros - 1; i++)
            {
                for (int j = 0; j < cantidadLibros - 1 - i; j++)
                {
                    if (string.Compare(
                        libros[j].Titulo,
                        libros[j + 1].Titulo,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        Libro temporal = libros[j];
                        libros[j] = libros[j + 1];
                        libros[j + 1] = temporal;
                    }
                }
            }
            GuardarLibros();
        }
        public void OrdenarSeleccionLibros()
        {
            for (int i = 0; i < cantidadLibros - 1; i++)
            {
                int indiceMenor = i;

                for (int j = i + 1; j < cantidadLibros; j++)
                {
                    if (string.Compare(
                        libros[j].Titulo,
                        libros[indiceMenor].Titulo,
                        StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        indiceMenor = j;
                    }
                }

                if (indiceMenor != i)
                {
                    Libro temporal = libros[i];
                    libros[i] = libros[indiceMenor];
                    libros[indiceMenor] = temporal;
                }
            }

            GuardarLibros();
        }

        // REGISTRAR PRÉSTAMOS
        public string RegistrarPrestamo(
            string codigoEstudiante, string codigoUL)
        {
            Estudiante estudiante = BuscarEstudiante(codigoEstudiante);

            if (estudiante == null)
                return "El estudiante no está registrado.";

            if (!estudiante.EstaActivo())
                return "El estudiante no tiene matrícula activa.";

            Libro libro = BuscarLibroPorUL(codigoUL);

            if (libro == null)
                return "No se encontró el ejemplar con ese código UL.";

            codigoUL = codigoUL.Trim();

            if (!libro.VerificarDisponibilidad(codigoUL))
                return "El ejemplar no está disponible.";

            AsegurarCapacidadPrestamos();

            Prestamo nuevo = new Prestamo(
                estudiante.Codigo, codigoUL);

            prestamos[cantidadPrestamos] = nuevo;
            cantidadPrestamos++;

            libro.ActualizarDisponibilidad(codigoUL, false);

            GuardarLibros();
            GuardarPrestamos();

            return "Préstamo registrado y guardado correctamente.\n" +
                   "Fecha prevista de devolución: " +
                   nuevo.FechaDevolucion.ToShortDateString();
        }

        // REGISTRAR DEVOLUCIONES
        public string DevolverLibro(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return "Ingrese el código UL del ejemplar.";

            codigoUL = codigoUL.Trim();

            for (int i = 0; i < cantidadPrestamos; i++)
            {
                if (string.Equals(
                    prestamos[i].CodigoUL, codigoUL,
                    StringComparison.OrdinalIgnoreCase) &&
                    prestamos[i].Activo)
                {
                    Libro libro = BuscarLibroPorUL(codigoUL);

                    if (libro == null)
                        return "No se encontró el libro.";

                    prestamos[i].RegistrarDevolucion();

                    libro.ActualizarDisponibilidad(codigoUL, true);

                    GuardarLibros();
                    GuardarPrestamos();

                    return "Devolución registrada y guardada correctamente.";
                }
            }

            return "No existe un préstamo activo para ese código UL.";
        }

        // MOSTRAR REGISTROS
        public Estudiante ObtenerEstudiantePorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidadEstudiantes)
                return null;

            return estudiantes[indice];
        }

        public int ObtenerCantidadEstudiantes()
        {
            return cantidadEstudiantes;
        }


        public Prestamo ObtenerPrestamoPorIndice(int indice)
        {
            if (indice < 0 || indice >= cantidadPrestamos)
                return null;

            return prestamos[indice];
        }

        public int ObtenerCantidadPrestamos()
        {
            return cantidadPrestamos;
        }
        // CONSULTAR REGISTROS
        public string ConsultarEstudiante(string codigo)
        {
            Estudiante estudiante = BuscarEstudiante(codigo);

            if (estudiante == null)
                return "No se encontró al estudiante.";

            return "Código: " + estudiante.Codigo +
                   "\nNombre: " + estudiante.Nombre +
                   "\nApellidos: " + estudiante.Apellidos +
                   "\nMatrícula: " + estudiante.EstadoMatricula;
        }

        public string ConsultarLibro(string codigoUL)
        {
            Libro libro = BuscarLibroPorUL(codigoUL);

            if (libro == null)
                return "No se encontró el ejemplar.";

            Ejemplar ejemplar = libro.BuscarEjemplar(codigoUL);

            return "Código del libro: " + libro.CodigoLibro +
                   "\nTítulo: " + libro.Titulo +
                   "\nÁrea: " + libro.Area +
                   "\nCódigo UL: " + ejemplar.CodigoUL +
                   "\nDisponibilidad: " +
                   (ejemplar.VerificarDisponibilidad()
                       ? "DISPONIBLE" : "PRESTADO");
        }

        // GUARDAR DATOS EN ARCHIVOS
        private void GuardarEstudiantes()
        {
            StringBuilder contenido = new StringBuilder();

            for (int i = 0; i < cantidadEstudiantes; i++)
            {
                contenido.AppendLine(
                    estudiantes[i].ConvertirATexto());
            }

            archivo.Guardar("estudiantes.txt", contenido.ToString());
        }
        private void GuardarLibros()
        {
            StringBuilder contenido = new StringBuilder();

            for (int i = 0; i < cantidadLibros; i++)
            {
                contenido.AppendLine(libros[i].ConvertirATexto());
            }

            archivo.Guardar("libros.txt", contenido.ToString());
        }

        private void GuardarPrestamos()
        {
            StringBuilder contenido = new StringBuilder();

            for (int i = 0; i < cantidadPrestamos; i++)
            {
                contenido.AppendLine(
                    prestamos[i].ConvertirATexto());
            }

            archivo.Guardar("prestamos.txt", contenido.ToString());
        }

        // CARGAR DATOS AL INICIAR EL SISTEMA
        private void CargarDatos()
        {
            MigrarCodigosULAntiguos();

            string textoEstudiantes = archivo.Leer("estudiantes.txt");
            string[] lineasEstudiantes = textoEstudiantes.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lineasEstudiantes.Length; i++)
            {
                Estudiante estudiante =
                    Estudiante.DesdeTexto(lineasEstudiantes[i]);

                if (estudiante != null &&
                    BuscarEstudiante(estudiante.Codigo) == null)
                {
                    AsegurarCapacidadEstudiantes();
                    estudiantes[cantidadEstudiantes] = estudiante;
                    cantidadEstudiantes++;
                }
            }

            string textoLibros = archivo.Leer("libros.txt");
            string[] lineasLibros = textoLibros.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lineasLibros.Length; i++)
            {
                Libro libro = Libro.DesdeTexto(lineasLibros[i]);

                if (libro != null &&
                    BuscarLibro(libro.CodigoLibro) == null)
                {
                    AsegurarCapacidadLibros();
                    libros[cantidadLibros] = libro;
                    cantidadLibros++;
                }
            }

            string textoPrestamos = archivo.Leer("prestamos.txt");
            string[] lineasPrestamos = textoPrestamos.Split(
                new[] { "\r\n", "\n" },
                StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lineasPrestamos.Length; i++)
            {
                Prestamo prestamo =
                    Prestamo.DesdeTexto(lineasPrestamos[i]);

                if (prestamo != null)
                {
                    AsegurarCapacidadPrestamos();
                    prestamos[cantidadPrestamos] = prestamo;
                    cantidadPrestamos++;
                }
            }
        }
        private string ActualizarCodigoUL(string codigoUL)
        {
            if (string.IsNullOrWhiteSpace(codigoUL))
                return codigoUL;

            // El código antiguo tiene UL + 4 dígitos.
            if (codigoUL.StartsWith("UL") &&
                codigoUL.Length == 6)
            {
                string numeros = codigoUL.Substring(2);
                bool sonNumeros = true;

                for (int i = 0; i < numeros.Length; i++)
                {
                    if (!char.IsDigit(numeros[i]))
                    {
                        sonNumeros = false;
                        break;
                    }
                }

                if (sonNumeros)
                    return "UL" + numeros.PadLeft(6, '0');
            }

            return codigoUL;
        }

        private string ActualizarLineaLibro(string linea)
        {
            string[] datos = linea.Split('|');

            // Los códigos UL están en las posiciones 3, 5, 7...
            for (int i = 3; i + 1 < datos.Length; i += 2)
            {
                datos[i] = ActualizarCodigoUL(datos[i]);
            }

            return string.Join("|", datos);
        }

        private string ActualizarLineaPrestamo(string linea)
        {
            string[] datos = linea.Split('|');

            // El código UL del préstamo está en la posición 1.
            if (datos.Length == 5)
                datos[1] = ActualizarCodigoUL(datos[1]);

            return string.Join("|", datos);
        }

        private void MigrarCodigosULAntiguos()
        {
            // Actualizar códigos guardados en libros.txt.
            string textoLibros = archivo.Leer("libros.txt");

            if (!string.IsNullOrEmpty(textoLibros))
            {
                string[] lineasLibros = textoLibros.Split(
                    new[] { "\r\n", "\n" },
                    StringSplitOptions.None);

                bool huboCambios = false;

                for (int i = 0; i < lineasLibros.Length; i++)
                {
                    string lineaOriginal = lineasLibros[i];

                    if (string.IsNullOrWhiteSpace(lineaOriginal))
                        continue;

                    lineasLibros[i] = ActualizarLineaLibro(lineaOriginal);

                    if (lineasLibros[i] != lineaOriginal)
                        huboCambios = true;
                }

                if (huboCambios)
                {
                    archivo.Guardar(
                        "libros.txt",
                        string.Join(Environment.NewLine, lineasLibros));
                }
            }

            // Actualizar también las referencias de prestamos.txt.
            string textoPrestamos = archivo.Leer("prestamos.txt");

            if (!string.IsNullOrEmpty(textoPrestamos))
            {
                string[] lineasPrestamos = textoPrestamos.Split(
                    new[] { "\r\n", "\n" },
                    StringSplitOptions.None);

                bool huboCambios = false;

                for (int i = 0; i < lineasPrestamos.Length; i++)
                {
                    string lineaOriginal = lineasPrestamos[i];

                    if (string.IsNullOrWhiteSpace(lineaOriginal))
                        continue;

                    lineasPrestamos[i] =
                        ActualizarLineaPrestamo(lineaOriginal);

                    if (lineasPrestamos[i] != lineaOriginal)
                        huboCambios = true;
                }

                if (huboCambios)
                {
                    archivo.Guardar(
                        "prestamos.txt",
                        string.Join(Environment.NewLine, lineasPrestamos));
                }
            }
        }
    }
}
