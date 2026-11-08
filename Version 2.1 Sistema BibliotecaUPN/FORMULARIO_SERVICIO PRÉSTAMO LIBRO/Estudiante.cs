using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Estudiante
    {
        private string codigo;
        private string nombres;
        private string apellidos;
        private string estadodeMatricula;

        public string Codigo
        {
            get { return codigo; }
            set { codigo = value; }
        }

        public string Nombres
        {
            get { return nombres; }
            set { nombres = value; }
        }

        public string Apellidos
        {
            get { return apellidos; }
            set { apellidos = value; }
        }

        public string EstadodeMatricula
        {
            get { return estadodeMatricula; }
            set { estadodeMatricula = value; }
        }

        public bool ValidarCodigo()
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;
            if(Codigo.Length != 9)
                return false;
            if(!Codigo.StartsWith("N00"))
            return true;
            
            string numeros = Codigo.Substring(3);

            if(!int.TryParse(numeros, out _))
                return false;
            return true;
        }
    }
}
