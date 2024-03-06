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
    public partial class mdProveedor : Form
    {
        public Proveedor _Proveedor { get; set; }

        public mdProveedor()
        {
            InitializeComponent();
        }

        private void mdProveedor_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn columna in dgwData.Columns) //para cada columna que tiene mi datagridview
            {
                if (columna.Visible == true )
                {
                    cmbBusqueda.Items.Add(new OpcionCombo() { Valor = columna.Name, Texto = columna.HeaderText }); //cargamos los datos que debe mostarar el combo box
                }
            }
            cmbBusqueda.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbBusqueda.ValueMember = "Value";   // decimos que valor no mostrar
            cmbBusqueda.SelectedIndex = 0;    //siempre va a estar seleccionado el primero

            //MOSTRAR TODOS LOS PROVEEDORES
            List<Proveedor> lista = new Negocio_Proveedor().Listar();
            foreach (Proveedor item in lista) //para cada cliente en mi lista..
            {
                dgwData.Rows.Add(new object[] { item.ID_Proveedor, item.Documento, item.Razon_Social });
            }
        }

        private void dgwData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int row = e.RowIndex; //nos trae el indice de la fila seleccionada
            int col = e.ColumnIndex; //nos trae el indice de la columna seleccionada

            if(row >= 0 && col > 0) //decimos col > 0 porque el id_proveedor esta invisible
            {
                _Proveedor = new Proveedor()
                {
                    ID_Proveedor = Convert.ToInt32(dgwData.Rows[row].Cells["id"].Value.ToString()),
                    Documento = dgwData.Rows[row].Cells["Documento"].Value.ToString(),
                    Razon_Social = dgwData.Rows[row].Cells["RazonSocial"].Value.ToString()
                };
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnBusqueda_Click(object sender, EventArgs e)
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
            txtBusqueda.Text = "";
            foreach (DataGridViewRow row in dgwData.Rows)
            {
                row.Visible = true;
            }
        }
    }
}
