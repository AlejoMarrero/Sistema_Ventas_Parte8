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
using FontAwesome.Sharp;

namespace Sistema_Ventas
{
    public partial class Inicio : Form
    { 
        private static Usuario usuario_actual;  //va a actuar como un usuario global para todos los metodos del formulario
        private static IconMenuItem menu_activo = null;
        private static Form formulario_activo = null;

        public Inicio(Usuario obj_Usuario = null)
        {
         if(obj_Usuario == null) usuario_actual = new Usuario() { Nombre_Completo = "ADMIN PREDEFINIDO", ID_Usuario = 1};
         else usuario_actual = obj_Usuario;
         InitializeComponent();
        }

        private void Inicio_Load(object sender, EventArgs e)
        {
            List<Permiso> lista_permisos = new Negocio_Permiso().Listar(usuario_actual.ID_Usuario); //listamos el a traves del id_usuario los permisos 

            foreach (IconMenuItem iconMenu in Menu.Items) //todo tipo de IconMenuItem del Menu lo almacena en la variable iconMenu
            {
                bool Encontrado = lista_permisos.Any(m => m.Nombre_Menu == iconMenu.Name); //Any determina si una lista contiene elementos
                
                if (Encontrado == false)
                {
                    iconMenu.Visible = false;
                }

            } 
            
            
            lblUsuario.Text = usuario_actual.Nombre_Completo;


        }

        private void AbrirFormulario(IconMenuItem menu, Form formulario) //recibe el menu seleccionado y el formulario que se debe de abrir
        {
            if (menu_activo != null)
            {
                menu_activo.BackColor = Color.White; //si dejamos de seleccionar el menu pasara a ser blanco nuevamente
            }
                menu.BackColor = Color.Silver; //si seleccionamos el menu pasara a ser silver
                menu_activo = menu;
            

            if(formulario_activo !=null) //si hay un formulario abierto, lo cerramos para abrir otro
            {
                formulario_activo.Close();
            }
                formulario_activo = formulario;
                formulario.TopLevel = false; // no mostrarse como ventana superior
                formulario.FormBorderStyle = FormBorderStyle.None; //sin ningun borde
                formulario.Dock = DockStyle.Fill; //que el contenedor se acomode dentro del formulario
                formulario.BackColor = Color.SteelBlue; //damos color al formulario

                contenedor.Controls.Add(formulario_activo); //agregamos formulario al contenedor
                formulario.Show(); //mostramos formulario

        }


        private void menuUsuario_Click(object sender, EventArgs e)
        {
            AbrirFormulario((IconMenuItem)sender, new FmrUsuarios()); // dos formas de enviar como parametro el menu : 1)(IconMenuItem)sender
        }

        private void SubMenuCategoria_Click(object sender, EventArgs e)// 2) nombre_parametro. En este caso menuMantenimiento
        {
            AbrirFormulario(menuMantenimiento, new FmrCategoria());
        }

        private void SubMenuProducto_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenimiento, new FmrProducto());
        }

        private void SubMenuRegistrarVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new FmrVentas());
        }

        private void SubMenuVerDetalleVenta_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuVentas, new FmrDetalleVentas());
        }

        private void SubMenuRegistrarCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuCompras, new FmrCompras(usuario_actual));
        }

        private void SubMenuVerDetalleCompra_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuCompras, new FmrDetalleCompras());
        }

        private void menuClientes_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuClientes, new FmrClientes());
        }

        private void menuProveedores_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuProveedores, new FmrProveedores());
        }

        private void menuReportes_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuReportes, new FmrReportes());
        }

        private void SubMenuNegocio_Click(object sender, EventArgs e)
        {
            AbrirFormulario(menuMantenimiento, new FmrNegocio());
        }
    }
}
