using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sistema_Ventas.Utilidades;
using Capa_Entidad;
using Capa_Negocio;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;

namespace Sistema_Ventas
{
    public partial class FmrUsuarios : Form
    {
        public FmrUsuarios()
        {
            InitializeComponent();
        }

        private void FmrUsuarios_Load(object sender, EventArgs e)
        {
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" }); //cargamos los datos que debe mostarar el combo box
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });

            cmbEstado.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbEstado.ValueMember = "Value";   // decimos que valor no mostrar
            cmbEstado.SelectedIndex = 0;    //siempre va a estar seleccionado el primero


            List<Rol> listaRol = new Negocio_Rol().Listar();
            foreach (Rol item in listaRol ) //para cada rol en mi lista..
            {
                cmbRol.Items.Add(new OpcionCombo() { Valor = item.ID_Rol, Texto = item.Descripcion });
            }
            cmbRol.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbRol.ValueMember = "Value";   // decimos que valor no mostrar
            cmbRol.SelectedIndex = 0;    //siempre va a estar seleccionado el primero



            foreach (DataGridViewColumn columna in dgwData.Columns) //para cada columna que tiene mi datagridview
            {
                if (columna.Visible == true && columna.Name != "btnSeleccionar")
                {
                    cmbBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText }); //cargamos los datos que debe mostarar el combo box
                }
            }
            cmbBusqueda.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbBusqueda.ValueMember = "Value";   // decimos que valor no mostrar
            cmbBusqueda.SelectedIndex = 0;    //siempre va a estar seleccionado el primero





            //MOSTRAR TODOS LOS USUARIOS
            List<Usuario> listaUsuario = new Negocio_Usuario().Listar();
            foreach (Usuario item in listaUsuario) //para cada rol en mi lista..
            {
                dgwData.Rows.Add(new object[] {"",item.ID_Usuario,item.Documento,item.Nombre_Completo,item.Correo,item.Clave,
                item.obj_Rol.ID_Rol,
                item.obj_Rol.Descripcion,
                item.Estado == true ?1 : 0, //en caso de que sea true, que mueste un 1. Caso contrario que muestre un 0 
                item.Estado == true ? "Activo" : " No Activo"
            }) ;
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;
            Usuario obj_Usuario = new Usuario()
            {
                ID_Usuario = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text),
                Documento = txtDocumento.Text,
                Nombre_Completo = txtNombreCompleto.Text,
                Correo = txtCorreo.Text,
                Clave = txtClave.Text,
                obj_Rol = new Rol() { ID_Rol = Convert.ToInt32(((OpcionCombo)cmbRol.SelectedItem).Valor)},
                Estado = Convert.ToInt32(((OpcionCombo)cmbEstado.SelectedItem).Valor) == 1 ? true : false
            };

            if(obj_Usuario.ID_Usuario == 0)
            {
                int idusuariogenerado = new Negocio_Usuario().Registrar(obj_Usuario, out Mensaje);

                if (idusuariogenerado != 0)
                {
                    dgwData.Rows.Add(new object[] {"",idusuariogenerado,txtDocumento.Text,txtNombreCompleto.Text,txtCorreo.Text,txtClave.Text,
                ((OpcionCombo)cmbRol.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cmbRol.SelectedItem).Texto.ToString(),
                ((OpcionCombo)cmbEstado.SelectedItem).Valor.ToString(),
                ((OpcionCombo)cmbEstado.SelectedItem).Texto.ToString(),
                });

                Limpiar();
                }
                else
                {
                    MessageBox.Show(Mensaje);
                }
            }
            else
            {
                bool resultado = new Negocio_Usuario().Editar(obj_Usuario, out Mensaje);
                if(resultado == true)
                {
                    DataGridViewRow row = dgwData.Rows[Convert.ToInt32(txtIndice.Text)];
                    row.Cells["Id"].Value = txtId.Text;
                    row.Cells["Documento"].Value = txtDocumento.Text;
                    row.Cells["NombreCompleto"].Value = txtNombreCompleto.Text;
                    row.Cells["Correo"].Value = txtCorreo.Text;
                    row.Cells["Clave"].Value = txtClave.Text;
                    row.Cells["IdRol"].Value = ((OpcionCombo)cmbRol.SelectedItem).Valor.ToString();
                    row.Cells["Rol"].Value = ((OpcionCombo)cmbRol.SelectedItem).Texto.ToString();
                    row.Cells["EstadoValor"].Value = ((OpcionCombo)cmbEstado.SelectedItem).Valor.ToString();
                    row.Cells["Estado"].Value = ((OpcionCombo)cmbEstado.SelectedItem).Texto.ToString();

                    Limpiar();
                }
                else
                {
                    MessageBox.Show(Mensaje);
                }
            }

        }

        private void Limpiar()
        {
            txtIndice.Text = "-1";
            txtId.Text = "";
            txtDocumento.Text = "";
            txtNombreCompleto.Text = "";
            txtCorreo.Text = "";
            txtClave.Text = "";
            txtConfirmarClave.Text = "";
            cmbRol.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            txtDocumento.Select();
        }

        private void dgwData_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) //si la cabecera es menor a 0 que retorne, que no haga nada
                return;

            if (e.ColumnIndex == 0)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All); //cellBounds obtiene las dimensiones de nuestra celda. DataGridViewPaintParts.all considere todas las dimensiones de la celda

                var w = Properties.Resources.check.Width; //en w estamos almacenando el ancho de la imagen que agregamos
                var h = Properties.Resources.check.Height; //en h estamos almacenando el alto de la imagen que agregamos
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2; // esto para pintar en el medio la imagen
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2; // esto para pintar en el medio la imagen

                e.Graphics.DrawImage(Properties.Resources.check, new Rectangle(x,y,w,h)); // aca le indicamos que pinte.
                e.Handled = true; // permite que la accion del "click" continue
            }



        }

        private void dgwData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgwData.Columns[e.ColumnIndex].Name == "btnSeleccionar") //aca le indicamos que si hizo click en el boton hara la siguiente serie de instrucciones
            {
                int indice = e.RowIndex; //almacenamos el indice de la fila que ha sido seleccionada

                if(indice >= 0) // si se selecciono una fila valida
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgwData.Rows[indice].Cells["Id"].Value.ToString(); //el contenido de la columna id se cargara en el textbox id
                    txtDocumento.Text = dgwData.Rows[indice].Cells["Documento"].Value.ToString(); //
                    txtNombreCompleto.Text = dgwData.Rows[indice].Cells["NombreCompleto"].Value.ToString(); //
                    txtCorreo.Text = dgwData.Rows[indice].Cells["Correo"].Value.ToString(); //
                    txtClave.Text = dgwData.Rows[indice].Cells["Clave"].Value.ToString(); //
                    txtConfirmarClave.Text = dgwData.Rows[indice].Cells["Documento"].Value.ToString(); //

                    foreach (OpcionCombo oc in cmbRol.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgwData.Rows[indice].Cells["IdRol"].Value)) //convertimos a int para que lo que estemos igualando sea del mismo tipo. //Si es que el valor es igual al que se esta mostrando 
                        {
                            int indice_combo = cmbRol.Items.IndexOf(oc);
                            cmbRol.SelectedIndex = indice_combo;
                            break;
                        }
                    }

                    foreach (OpcionCombo oc in cmbEstado.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgwData.Rows[indice].Cells["EstadoValor"].Value)) //convertimos a int para que lo que estemos igualando sea del mismo tipo. //Si es que el valor es igual al que se esta mostrando 
                        {
                            int indice_combo = cmbEstado.Items.IndexOf(oc);
                            cmbEstado.SelectedIndex = indice_combo;
                            break;
                        }
                    }

                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(txtId.Text) != 0)
            {               
                if (MessageBox.Show("¿Desea eliminar el usuario?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string Mensaje = string.Empty;
                    Usuario obj_Usuario = new Usuario()
                    {
                        ID_Usuario = Convert.ToInt32(txtId.Text)
                    };

                    bool respuesta = new Negocio_Usuario().Eliminar(obj_Usuario, out Mensaje);

                    if (respuesta == false)
                    {
                        // Eliminar la fila seleccionada del DataGridView
                        dgwData.Rows.RemoveAt(Convert.ToInt32(txtIndice.Text));

                        // Limpiar los campos después de eliminar
                        Limpiar();
                    }
                    else
                    {
                        MessageBox.Show(Mensaje, "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    }

                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar algun Usuario para eliminar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cmbBusqueda.SelectedItem).Valor.ToString();

            if(dgwData.Rows.Count > 0)
            {
                foreach(DataGridViewRow row in dgwData.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        row.Visible=false;
                    }
                }
            }
        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            foreach (DataGridViewRow row in dgwData.Rows)
            {
                row.Visible = true;
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }
    }

}
