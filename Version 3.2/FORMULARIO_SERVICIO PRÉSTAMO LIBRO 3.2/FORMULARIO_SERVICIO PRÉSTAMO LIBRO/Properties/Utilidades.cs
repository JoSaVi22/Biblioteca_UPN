using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FORMULARIO_SERVICIO_PRÉSTAMO_LIBRO.Properties
{
    public static class Utilidades
    {
        // 1. Validar si un campo está vacío
        public static bool ValidarCampoVacio(string texto)
        {
            return string.IsNullOrWhiteSpace(texto);
        }
        // 2. Mostrar un mensaje flotante
        public static void MostrarMensaje(string mensaje)
        {
            MessageBox.Show(mensaje, "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        // 3. Limpiar varias cajas de texto a la vez
        public static void LimpiarTextBox(params TextBox[] cajas)
        {
            foreach (TextBox txt in cajas)
            {
                if (txt != null) txt.Clear();
            }
        }
    }
}