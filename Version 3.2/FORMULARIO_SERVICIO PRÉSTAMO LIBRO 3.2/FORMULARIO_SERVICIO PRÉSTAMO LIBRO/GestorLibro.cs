using System;
using System.Collections.Generic;
using System.IO;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class GestorLibro
    {
        private List<Libro> libro = new List<Libro>();
        private List<Libro> librosOriginales = new List<Libro>();

        private string rutaArchivo = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "libros.txt"
        );
        // Ruta para el archivo binario
        private string rutaBinario = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        "libros.dat"
        );

        private int siguienteCodigoLibro = 1;
        private int siguienteCodigoUL = 1;

        public string RegistrarLibro(string titulo, string area)
        {
            if (string.IsNullOrWhiteSpace(titulo) ||
                string.IsNullOrWhiteSpace(area))
                return "Ingrese el título y el área del libro.";

            titulo = titulo.Trim();
            area = area.Trim();

            Libro libroEncontrado = libro.Find(l =>
                string.Equals(l.Titulo, titulo,
                StringComparison.OrdinalIgnoreCase));

            if (libroEncontrado == null)
            {
                string codigoLibro = "L" + siguienteCodigoLibro.ToString("D3");

                libroEncontrado = new Libro(codigoLibro, titulo, area);

                libro.Add(libroEncontrado);
                librosOriginales.Add(libroEncontrado);

                siguienteCodigoLibro++;
            }
            else if (!string.Equals(libroEncontrado.Area, area,
                StringComparison.OrdinalIgnoreCase))
            {
                return "El título ya existe con otra área.";
            }

            string codigoUL = "UL" + siguienteCodigoUL.ToString("D6");

            libroEncontrado.AgregarEjemplar(new Ejemplar(codigoUL));
            siguienteCodigoUL++;
            GuardarBinarioLibros();
            return "Libro registrado correctamente.\n" +
                   "Código del libro: " + libroEncontrado.CodigoLibro + "\n" +
                   "Código UL del ejemplar: " + codigoUL;
        }

        public Libro BuscarLibro(string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
                return null;

            return libro.Find(l =>
                l.Titulo.IndexOf(titulo.Trim(),
                StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void RestaurarOrdenOriginal()
        {
            libro.Clear();
            libro.AddRange(librosOriginales);
        }

        public List<Libro> ObtenerLibros()
        {
            return libro;
        }

        public void GuardarLibros()
        {
            List<string> datos = new List<string>();

            foreach (Libro libroActual in libro)
            {
                foreach (Ejemplar ejemplar in libroActual.ObtenerEjemplares())
                {
                    datos.Add(
                        $"{libroActual.CodigoLibro}|{libroActual.Titulo}|{libroActual.Area}|{ejemplar.CodigoUL}|{ejemplar.Disponible}"
                    );
                }
            }

            File.WriteAllLines(rutaArchivo, datos);
        }

        public void CrearLibrosIniciales()
        {
            if (File.Exists(rutaArchivo))
                return;

            string[] titulos =
            {
                "Calculo diferencial", "Algebra lineal",
                "Programacion en Python", "Anatomia humana",
                "Salud Mental", "Fundamentos de enfermeria",
                "Administracion de empresas", "Contabilidad financiera",
                "Derecho civil", "Derecho constitucional",
                "Diseno arquitectonico", "Estadistica basica"
            };

            string[] areas =
            {
                "Ingenieria", "Ingenieria", "Ingenieria",
                "Salud", "Salud", "Salud",
                "Negocios", "Negocios",
                "Derecho", "Derecho",
                "Arquitectura", "General"
            };

            for (int i = 0; i < titulos.Length; i++)
            {
                    RegistrarLibro(titulos[i], areas[i]);
            }

            GuardarLibros();
        }

        public void CargarLibros()
        {
            if (!File.Exists(rutaArchivo))
                return;

            string[] lineas = File.ReadAllLines(rutaArchivo);
            List<Libro> librosCargados = new List<Libro>();

            int mayorLibro = 0;
            int mayorUL = 0;

            foreach (string linea in lineas)
            {
                string[] datos = linea.Split('|');

                if (datos.Length != 5)
                    throw new FormatException("Registro incompleto.");

                int numeroLibro;
                int numeroUL;
                bool disponible;

                if (!datos[0].StartsWith("L") ||
                    !int.TryParse(datos[0].Substring(1), out numeroLibro) ||
                    numeroLibro <= 0 ||
                    string.IsNullOrWhiteSpace(datos[1]) ||
                    string.IsNullOrWhiteSpace(datos[2]) ||
                    !datos[3].StartsWith("UL") ||
                    !int.TryParse(datos[3].Substring(2), out numeroUL) ||
                    numeroUL <= 0 ||
                    !bool.TryParse(datos[4], out disponible))
                {
                    throw new FormatException("Datos inválidos.");
                }

                Libro encontrado = librosCargados.Find(
                    l => l.CodigoLibro == datos[0]);

                if (encontrado == null)
                {
                    encontrado = new Libro(datos[0], datos[1], datos[2]);
                    librosCargados.Add(encontrado);
                }

                Ejemplar ejemplar = new Ejemplar(datos[3]);
                ejemplar.Disponible = disponible;

                encontrado.AgregarEjemplar(ejemplar);

                if (numeroLibro > mayorLibro)
                    mayorLibro = numeroLibro;

                if (numeroUL > mayorUL)
                    mayorUL = numeroUL;
            }

            libro = librosCargados;
            librosOriginales = new List<Libro>(librosCargados);

            siguienteCodigoLibro = mayorLibro + 1;
            siguienteCodigoUL = mayorUL + 1;
        }
        // Devuelve el total de títulos de libros registrados
        public int ContarLibros()
        {
            return libro.Count;
        }
        // Devuelve el total acumulado de ejemplares (disponibles y prestados)
        public int ContarEjemplaresTotales()
        {
            int contador = 0;
            foreach (Libro l in libro)
            {
                contador += l.ObtenerEjemplares().Count;
            }
            return contador;
        }
        // Devuelve la cantidad de ejemplares disponibles para préstamo
        public int ContarEjemplaresDisponibles()
        {
            int contador = 0;
            foreach (Libro l in libro)
            {
                foreach (Ejemplar ej in l.ObtenerEjemplares())
                {
                    if (ej.Disponible)
                    {
                        contador++;
                    }
                }
            }
            return contador;
        }
        public bool EliminarEjemplar(string codigoUL)
        {
            foreach (Libro libroActual in libro)
            {
                if (libroActual.EliminarEjemplar(codigoUL))
                {
                    GuardarLibros();
                    return true;
                }
            }
            return false;
        }

        public void OrdenarBurbujaLibros()
        {
            for (int i = 0; i < libro.Count - 1; i++)
            {
                for (int j = 0; j < libro.Count - 1 - i; j++)
                {
                    if (string.Compare(libro[j].Titulo,
                        libro[j + 1].Titulo,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        Libro temporal = libro[j];
                        libro[j] = libro[j + 1];
                        libro[j + 1] = temporal;
                    }
                }
            }
        }

        public void OrdenarSeleccionLibros()
        {
            for (int i = 0; i < libro.Count - 1; i++)
            {
                int menor = i;

                for (int j = i + 1; j < libro.Count; j++)
                {
                    if (string.Compare(libro[j].Titulo,
                        libro[menor].Titulo,
                        StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        menor = j;
                    }
                }

                if (menor != i)
                {
                    Libro temporal = libro[i];
                    libro[i] = libro[menor];
                    libro[menor] = temporal;
                }
            }
        }
        //Los metodos GuardarBinarioLibros y LeerBinarioLibros permiten guardar y leer los datos
        //de los libros en un archivo binario, respectivamente.
        //Pero dentro del código, sin necesidad de poner botones
        //al guardarce automaticamente cada vez que se registra un libro  
        public void GuardarBinarioLibros()
        {
            using (FileStream fs = new FileStream(rutaBinario, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(libro.Count);
                    foreach (Libro libroActual in libro)
                    {
                        bw.Write(libroActual.CodigoLibro ?? "");
                        bw.Write(libroActual.Titulo ?? "");
                        bw.Write(libroActual.Area ?? "");

                        List<Ejemplar> ejemplares = libroActual.ObtenerEjemplares();
                        bw.Write(ejemplares.Count);
                        foreach (Ejemplar ej in ejemplares)
                        {
                            bw.Write(ej.CodigoUL ?? "");
                            bw.Write(ej.Disponible);
                        }
                    }
                }
            }
        }

        public void LeerBinarioLibros()
        {
            if (!File.Exists(rutaBinario)) return;

            List<Libro> librosCargados = new List<Libro>();
            int mayorLibro = 0, mayorUL = 0;

            using (FileStream fs = new FileStream(rutaBinario, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int totalLibros = br.ReadInt32();
                    for (int i = 0; i < totalLibros; i++)
                    {
                        string codigoLibro = br.ReadString();
                        string titulo = br.ReadString();
                        string area = br.ReadString();

                        Libro libroNuevo = new Libro(codigoLibro, titulo, area);

                        int numLibro = 0;
                        if (!string.IsNullOrEmpty(codigoLibro) && codigoLibro.StartsWith("L"))
                        {
                            int.TryParse(codigoLibro.Substring(1), out numLibro);
                            if (numLibro > mayorLibro) mayorLibro = numLibro;
                        }

                        int totalEjemplares = br.ReadInt32();
                        for (int j = 0; j < totalEjemplares; j++)
                        {
                            string codigoUL = br.ReadString();
                            bool disponible = br.ReadBoolean();

                            Ejemplar ejemplar = new Ejemplar(codigoUL);
                            ejemplar.Disponible = disponible;
                            libroNuevo.AgregarEjemplar(ejemplar);

                            int numUL = 0;
                            if (!string.IsNullOrEmpty(codigoUL) && codigoUL.StartsWith("UL"))
                            {
                                int.TryParse(codigoUL.Substring(2), out numUL);
                                if (numUL > mayorUL) mayorUL = numUL;
                            }
                        }
                        librosCargados.Add(libroNuevo);
                    }
                }
            }

            libro = librosCargados;
            librosOriginales = new List<Libro>(librosCargados);
            siguienteCodigoLibro = mayorLibro + 1;
            siguienteCodigoUL = mayorUL + 1;
            GuardarBinarioLibros();
        }
    }
}
