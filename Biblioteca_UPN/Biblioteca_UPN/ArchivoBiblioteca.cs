using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca_UPN
{
    public class ArchivoBiblioteca
    {
        private string carpeta;

        public ArchivoBiblioteca()
        {
            carpeta = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments),
                "BibliotecaUPN");

            if (!Directory.Exists(carpeta))
            {
                Directory.CreateDirectory(carpeta);
            }
        }

        public void Guardar(string nombreArchivo, string contenido)
        {
            string ruta = Path.Combine(carpeta, nombreArchivo);

            File.WriteAllText(
                ruta,
                contenido,
                Encoding.UTF8);
        }

        public string Leer(string nombreArchivo)
        {
            string ruta = Path.Combine(carpeta, nombreArchivo);

            if (!File.Exists(ruta))
            {
                return "";
            }

            return File.ReadAllText(ruta, Encoding.UTF8);
        }
        public string ObtenerRuta()
        {
            return carpeta;
        }
    }
}
