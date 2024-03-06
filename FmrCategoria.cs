using Capa_Entidad;
using Capa_Negocio;
using Sistema_Ventas.Utilidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sistema_Ventas
{
    public partial class FmrCategoria : Form
    {
        public FmrCategoria()
        {
            InitializeComponent();
        }

        private void FmrCategoria_Load(object sender, EventArgs e)
        {
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" }); //cargamos los datos que debe mostarar el combo box
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });

            cmbEstado.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbEstado.ValueMember = "Value";   // decimos que valor no mostrar
            cmbEstado.SelectedIndex = 0;    //siempre va a estar seleccionado el primero

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


            //MOSTRAR TODAS LAS CATEGORIAS
            List<Categoria> lista = new Negocio_Categoria().Listar();
            foreach (Categoria item in lista) //para cada rol en mi lista..
            {
                dgwData.Rows.Add(new object[] {"",item.ID_Categoria,
                item.Descripcion,
                item.Estado == true ?1 : 0, //en caso de que sea true, que mueste un 1. Caso contrario que muestre un 0 
                item.Estado == true ? "Activo" : " No Activo"
            });
            }


        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;
            Categoria obj = new Categoria()
            {
                // Comprobamos si el texto en txtId.Text es válido antes de convertirlo
                ID_Categoria = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text),
                Descripcion = txtDescripcion.Text,
                Estado = Convert.ToInt32(((OpcionCombo)cmbEstado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj.ID_Categoria == 0)
            {
                int id_generado = new Negocio_Categoria().Registrar(obj, out Mensaje);

                if (id_generado != 0)
                {
                    dgwData.Rows.Add(new object[] {"",id_generado,txtDescripcion.Text,
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
                bool resultado = new Negocio_Categoria().Editar(obj, out Mensaje);
                if (resultado == true)
                {
                    DataGridViewRow row = dgwData.Rows[Convert.ToInt32(txtIndice.Text)];
                    row.Cells["Id"].Value = txtId.Text;
                    row.Cells["Descripcion"].Value = txtDescripcion.Text;
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
            txtDescripcion.Text = "";
            cmbEstado.SelectedIndex = 0;
            txtDescripcion.Select();
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

                e.Graphics.DrawImage(Properties.Resources.check, new Rectangle(x, y, w, h)); // aca le indicamos que pinte.
                e.Handled = true; // permite que la accion del "click" continue
            } 
        }

        private void dgwData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgwData.Columns[e.ColumnIndex].Name == "btnSeleccionar") //aca le indicamos que si hizo click en el boton hara la siguiente serie de instrucciones
            {
                int indice = e.RowIndex; //almacenamos el indice de la fila que ha sido seleccionada

                if (indice >= 0) // si se selecciono una fila valida
                {
                    txtIndice.Text = indice.ToString();
                    txtId.Text = dgwData.Rows[indice].Cells["Id"].Value.ToString(); //el contenido de la columna id se cargara en el textbox id
                    txtDescripcion.Text = dgwData.Rows[indice].Cells["Descripcion"].Value.ToString(); //

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
                if (MessageBox.Show("¿Desea eliminar la categoria?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string Mensaje = string.Empty;
                    Categoria obj = new Categoria()
                    {
                        ID_Categoria = Convert.ToInt32(txtId.Text)
                    };

                    bool respuesta = new Negocio_Categoria().Eliminar(obj, out Mensaje);

                    if (respuesta)
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
                MessageBox.Show("Debe seleccionar alguna Categoria para eliminar.","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string columnaFiltro = ((OpcionCombo)cmbBusqueda.SelectedItem).Valor.ToString();

            if (dgwData.Rows.Count > 0)
            {
                foreach (DataGridViewRow row in dgwData.Rows)
                {
                    if (row.Cells[columnaFiltro].Value.ToString().Trim().ToUpper().Contains(txtBusqueda.Text.Trim().ToUpper()))
                    {
                        row.Visible = true;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
            }
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void btnLimpiarBuscador_Click(object sender, EventArgs e)
        {
            txtBusqueda.Text = "";
            foreach (DataGridViewRow row in dgwData.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
