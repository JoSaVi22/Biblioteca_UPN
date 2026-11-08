using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO
{
    public class Libro
    {
        public string CodigoLibro { get; set; }
        public string Titulo { get; set; }
        public string Area { get; set; }

        private List<Ejemplar> ejemplares =
            new List<Ejemplar>();
        
        public Libro(string codigoLibro, string titulo, string area)
        {
            CodigoLibro = codigoLibro;
            Titulo = titulo;
            Area = area;
        }
        public void AgregarEjemplar(Ejemplar ejemplar)
        {
            if (ejemplar != null)
            {
                ejemplares.Add(ejemplar);
            }
        }
        public List<Ejemplar> ObtenerEjemplares()
        {
            return ejemplares;
        }
    }
}
