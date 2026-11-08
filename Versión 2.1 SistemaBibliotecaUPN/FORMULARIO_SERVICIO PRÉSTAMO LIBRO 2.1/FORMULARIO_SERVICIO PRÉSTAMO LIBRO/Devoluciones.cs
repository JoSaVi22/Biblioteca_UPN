using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Devoluciones
    {
        private string codigoUlLibro;
        private string codigoEstudiante;
        private string tituloLibro;
        private string areaLibro;
        private DateTime fechaPrestamo;
        private DateTime fechaDevolucionPrevista;
        private DateTime fechaDevolucionReal;
        private string estadoDevolucion;
        
        public string CodigoUlLibro
        {
            get { return codigoUlLibro; }
            set { codigoUlLibro = value; }
        }

        public string CodigoEstudiante
        {
            get { return codigoEstudiante; }
            set { codigoEstudiante = value; }
        }

        public string TituloLibro
        {
            get { return tituloLibro; }
            set { tituloLibro = value; }
        }

        public string AreaLibro
        {
            get { return areaLibro; }
            set { areaLibro = value; }
        }

        public DateTime FechaPrestamo
        {
            get { return fechaPrestamo; }
            set { fechaPrestamo = value; }
        }

        public DateTime FechaDevolucionPrevista
        {
            get { return fechaDevolucionPrevista; }
            set { fechaDevolucionPrevista = value; }
        }

        public DateTime FechaDevolucionReal
        {
            get { return fechaDevolucionReal; }
            set { fechaDevolucionReal = value; }
        }

        public string EstadoDevolucion
        {
            get { return estadoDevolucion; }
            set { estadoDevolucion = value; }
        }
    }
}
