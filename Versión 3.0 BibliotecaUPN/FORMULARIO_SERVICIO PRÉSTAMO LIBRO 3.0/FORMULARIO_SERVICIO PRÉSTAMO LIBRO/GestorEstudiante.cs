using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class GestorEstudiante
    {
        private List<Estudiante> estudiantes = new List<Estudiante>();

        public bool RegistrarEstudiante(Estudiante estudiante)
        {
            if (estudiante == null)
                return false;

            if (!estudiante.ValidarCodigo())
                return false;

            estudiantes.Add(estudiante);
            return true;
        }
        public bool EliminarEstudiante(string codigo)
        {
            Estudiante estudiante = estudiantes.Find(
                e => e.Codigo == codigo
            );

            if(estudiante == null)
                return false;
            estudiantes.Remove(estudiante);
            return true;
        }

        public void OrdenarBurbujaEstudiante()
        {
            for (int i = 0; i < estudiantes.Count - 1; i++)
            {
                for (int j = 0; j < estudiantes.Count - i - 1; j++)
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
        }
        public void OrdenarSeleccionEstudiante()
        {
            for (int i = 0; i < estudiantes.Count - 1; i++)
            {
                int posicionMenor = i;
                for (int j = i + 1; j < estudiantes.Count; j++)
                {
                    if (string.Compare(
                        estudiantes[j].Apellidos,
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
    }
}
