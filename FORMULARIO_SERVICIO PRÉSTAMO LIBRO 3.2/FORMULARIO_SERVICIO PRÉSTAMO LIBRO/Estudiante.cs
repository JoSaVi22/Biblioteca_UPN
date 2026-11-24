using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Estudiante
    {
        public string Codigo { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string EstadodeMatricula { get; set; }

        public bool ValidarCodigo()
        {
            return !string.IsNullOrWhiteSpace(Codigo)
                && Codigo.Length == 9
                && Codigo.StartsWith("N00")
                && Codigo.Substring(3).All(char.IsDigit);
        }
    }
}
