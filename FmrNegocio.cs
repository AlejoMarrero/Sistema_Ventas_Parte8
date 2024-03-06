using Capa_Entidad;
using Capa_Negocio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Ventas
{
    public partial class FmrNegocio : Form
    {
        public FmrNegocio()
        {
            InitializeComponent();
        }

        public Image ByteToImagen(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream();
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = new Bitmap(ms);

            return image;
        }

        private void FmrNegocio_Load(object sender, EventArgs e)
        {
            bool obtenido = true;
            byte[] byteimage = new Negocio_Negocio().ObtenerLogo(out obtenido);

            if (obtenido)
                picLogo.Image = ByteToImagen(byteimage);
            
            Negocio datos = new Negocio_Negocio().ObtenerDatos();
            txtNombreNegocio.Text = datos.Nombre;
            txtCUIT.Text = datos.CUIT;
            txtDireccion.Text = datos.Direccion;
        }

        private void btnSubir_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files|*.jpg;*.jpeg;*.png";
            if(openFileDialog.ShowDialog() == DialogResult.OK) 
            {
                byte[] byteImage = File.ReadAllBytes(openFileDialog.FileName);
                bool respuesta = new Negocio_Negocio().ActualizaLogo(byteImage, out mensaje);   
                
                if (respuesta)
                    picLogo.Image = ByteToImagen(byteImage);
                else
                   MessageBox.Show(mensaje,"Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
            }
        }

        private void btnGuardarCambios_Click(object sender, EventArgs e)
        {
            string mensaje = string.Empty;

            Negocio obj = new Negocio() 
            {
                Nombre = txtNombreNegocio.Text,
                CUIT = txtCUIT.Text,
                Direccion = txtDireccion.Text
            };

            bool respuesta = new Negocio_Negocio().GuardarDatos(obj, out mensaje);

            if(respuesta)
                MessageBox.Show("Los cambios fueron guardados", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("No se pudieron guardar los cambios", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}
