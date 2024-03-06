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

namespace Sistema_Ventas.Modales
{
    public partial class mdProducto : Form
    {
        public Producto _Producto { get; set; }

        public mdProducto()
        {
            InitializeComponent();
        }

        private void mdProducto_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgwData.Columns) //para cada columna que tiene mi datagridview
            {
                if (columna.Visible == true)
                {
                    cmbBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText }); //cargamos los datos que debe mostarar el combo box
                }
            }
            cmbBusqueda.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbBusqueda.ValueMember = "Value";   // decimos que valor no mostrar
            cmbBusqueda.SelectedIndex = 0;    //siempre va a estar seleccionado el primero

            //MOSTRAR TODOS LOS PRODUCTOS
            List<Producto> lista = new Negocio_Producto().Listar();
            foreach (Producto item in lista) //para cada rol en mi lista..
            {
                dgwData.Rows.Add(new object[]
                {   item.ID_Producto,
                    item.Codigo,
                    item.Nombre_Producto,
                    item.obj_Categoria.Descripcion,
                    item.stock,
                    item.Precio_Compra,
                    item.Precio_Venta,
                });
            }
        }

        private void dgwData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex; //nos trae el indice de la fila seleccionada
            int col = e.ColumnIndex; //nos trae el indice de la columna seleccionada

            if (row >= 0 && col > 0) //decimos col > 0 porque el id_proveedor esta invisible
            {
                _Producto = new Producto()
                {
                    ID_Producto = Convert.ToInt32(dgwData.Rows[row].Cells["id"].Value.ToString()),
                    Codigo = dgwData.Rows[row].Cells["Codigo"].Value.ToString(),
                    Nombre_Producto = dgwData.Rows[row].Cells["Nombre"].Value.ToString(),
                    stock = Convert.ToInt32(dgwData.Rows[row].Cells["Stock"].Value.ToString()),
                    Precio_Compra = Convert.ToDecimal(dgwData.Rows[row].Cells["Precio_Compra"].Value.ToString()),
                    Precio_Venta = Convert.ToDecimal(dgwData.Rows[row].Cells["Precio_Venta"].Value.ToString()),
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
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
