using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class GestorEstudiante
    {
        private List<Estudiante> estudiantes = new List<Estudiante>();
        private string rutaBinarioEstudiantes = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "estudiantes.dat");

        public bool RegistrarEstudiante(Estudiante estudiante)
        {
            if (estudiante == null || !estudiante.ValidarCodigo())
                return false;

            estudiantes.Add(estudiante);
            GuardarBinarioEstudiantes();
            return true;
        }
        //Inicia la búsqueda desde el índice 0
        public Estudiante BuscarEstudiante(string codigo, string apellidos)
        {
            return BuscarEstudianteRecursivo(codigo, apellidos, 0);
        }
        // Método recursivo para buscar un estudiante por código o apellidos
        private Estudiante BuscarEstudianteRecursivo(string codigo, string apellidos, int indice)
        {
            if (indice >= estudiantes.Count)
            {
                return null;
            }

            Estudiante actual = estudiantes[indice];
            // 2. Caso Base: Se encontró coincidencia por código o apellido
            bool coincideCodigo = !string.IsNullOrWhiteSpace(codigo) &&
                                 actual.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase);

            bool coincideApellido = !string.IsNullOrWhiteSpace(apellidos) &&
                                   (actual.Apellidos ?? "").IndexOf(apellidos, StringComparison.OrdinalIgnoreCase) >= 0;

            if (coincideCodigo || coincideApellido)
            {
                return actual; // Retorna el estudiante encontrado
            }
        // 3. Paso Recursivo: Evaluar la siguiente posición (indice + 1)
            return BuscarEstudianteRecursivo(codigo, apellidos, indice + 1);
        }
        public void OrdenarEstudiantesRecursivo()
        {
            if (estudiantes == null || estudiantes.Count <= 1)
                return;

            // Inicia el ordenamiento recursivo: n = total de elementos, i = índice 0
            OrdenarEstudiantesPasadaRecursiva(estudiantes.Count, 0);
        }

        // Método recursivo
        private void OrdenarEstudiantesPasadaRecursiva(int n, int i)
        {
            // 1. Caso Base General: Si la sublista tiene tamaño 1 o menos, finaliza todo el ordenamiento
            if (n <= 1)
                return;

            // 2. Caso Base de la Pasada: Si 'i' llegó al límite (n - 1) de la pasada actual
            if (i == n - 1)
            {
                // Reiniciamos 'i' en 0 y reducimos el límite a (n - 1) para la siguiente pasada
                OrdenarEstudiantesPasadaRecursiva(n - 1, 0);
                return;
            }

            // 3. Comparación e intercambio entre posición i e i + 1
            string ap1 = estudiantes[i].Apellidos ?? "";
            string ap2 = estudiantes[i + 1].Apellidos ?? "";

            if (string.Compare(ap1, ap2, StringComparison.OrdinalIgnoreCase) > 0)
            {
                Estudiante temp = estudiantes[i];
                estudiantes[i] = estudiantes[i + 1];
                estudiantes[i + 1] = temp;
            }

            // 4. Paso Recursivo Interno: Avanza al siguiente elemento (i + 1)
            OrdenarEstudiantesPasadaRecursiva(n, i + 1);
        }
        public bool EliminarEstudiante(string codigo)
        {
            Estudiante estudiante = estudiantes.Find(e => string.Equals(e.Codigo, codigo, StringComparison.OrdinalIgnoreCase));

            if (estudiante == null)
                return false;

            estudiantes.Remove(estudiante);
            GuardarBinarioEstudiantes();
            return true;
        }
        // Devuelve el total de estudiantes registrados
        public int ContarEstudiantes()
        {
            return estudiantes.Count;
        }
        // Devuelve cuántos estudiantes tienen estado "ACTIVO"
        public int ContarEstudiantesActivos()
        {
            int contador = 0;
            foreach (Estudiante est in estudiantes)
            {
                if (est != null && string.Equals(est.EstadodeMatricula, "ACTIVO", StringComparison.OrdinalIgnoreCase))
                {
                    contador++;
                }
            }
            return contador;
        }

        public void OrdenarBurbujaEstudiante()
        {
            for (int i = 0; i < estudiantes.Count - 1; i++)
            {
                for (int j = 0; j < estudiantes.Count - i - 1; j++)
                {
                    if (string.Compare(estudiantes[j].Apellidos,
                        estudiantes[j + 1].Apellidos,
                        StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        Estudiante temporal = estudiantes[j];
                        estudiantes[j] = estudiantes[j + 1];
                        estudiantes[j + 1] = temporal;
                    }
                }
            }
        }

        public void OrdenarSeleccionEstudiante()
        {
            for (int i = 0; i < estudiantes.Count - 1; i++)
            {
                int posicionMenor = i;

                for (int j = i + 1; j < estudiantes.Count; j++)
                {
                    if (string.Compare(estudiantes[j].Apellidos,
                        estudiantes[posicionMenor].Apellidos,
                        StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        posicionMenor = j;
                    }
                }

                if (posicionMenor != i)
                {
                    Estudiante temporal = estudiantes[i];
                    estudiantes[i] = estudiantes[posicionMenor];
                    estudiantes[posicionMenor] = temporal;
                }
            }
        }

        public List<Estudiante> ObtenerEstudiantes()
        {
            return estudiantes;
        }
        public void GuardarBinarioEstudiantes()
        { 
            using (FileStream fs = new FileStream(rutaBinarioEstudiantes, FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    bw.Write(estudiantes.Count);
                    foreach (Estudiante est in estudiantes)
                    {
                        bw.Write(est.Codigo ?? "");
                        bw.Write(est.Nombres ?? "");
                        bw.Write(est.Apellidos ?? "");
                        bw.Write(est.EstadodeMatricula ?? "");
                    }
                }
            }
        }
        public void LeerBinarioEstudiantes()
        {
            if (!File.Exists(rutaBinarioEstudiantes))
                return;
            List<Estudiante> estudiantesCargados = new List<Estudiante>();
            using (FileStream fs = new FileStream(rutaBinarioEstudiantes, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    int cantidad = br.ReadInt32();
                    estudiantes.Clear();
                    for (int i = 0; i < cantidad; i++)
                    {
                        string codigo = br.ReadString();
                        string nombres = br.ReadString();
                        string apellidos = br.ReadString();
                        string estadoMatricula = br.ReadString();

                        Estudiante est = new Estudiante
                        {
                            Codigo = codigo,
                            Nombres = nombres,
                            Apellidos = apellidos,
                            EstadodeMatricula = estadoMatricula
                        };
                        estudiantesCargados.Add(est);
                    }
                }
            }
            estudiantes = estudiantesCargados;
        }
    }
}
