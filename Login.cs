using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Capa_Entidad;
using Capa_Negocio;

namespace Sistema_Ventas
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            Usuario obj_Usuario = new Negocio_Usuario().Listar().Where(u => u.Documento == txtDocumento.Text && u.Clave == txtClave.Text).FirstOrDefault(); //valida que los datos sean correctos 
            if (obj_Usuario != null)
            {
                Inicio form = new Inicio(obj_Usuario);
                form.Show(); //mostrar formulario inicial
                this.Hide(); // ocultar formulario de login
                form.FormClosing += Fmr_Cerrando; // si cerramoms el formulario inicial, mostramos nuevamente el fmr login

            }
            else MessageBox.Show("No se encontro el usuario","Mensaje",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);

        }

        private void Fmr_Cerrando(object sender, FormClosingEventArgs e)
        {
            txtDocumento.Text = "";
            txtClave.Text = "";
            this.Show(); // metodo para volver a abrir el formulario de login
        }
    }
}
