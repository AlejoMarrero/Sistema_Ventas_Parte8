using Capa_Entidad;
using Capa_Negocio;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Sistema_Ventas.Modales;
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
using Color = System.Drawing.Color;

namespace Sistema_Ventas
{
    public partial class FmrCompras : Form
    {
        private Usuario _Proveedor = new Usuario(); 

        public FmrCompras(Usuario oUsuario = null)
        {
            _Proveedor = oUsuario;
            InitializeComponent();
        }

        private void FmrCompras_Load(object sender, EventArgs e)
        {
            cmbTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Boleta", Texto = "Boleta" }); //cargamos los datos que debe mostarar el combo box
            cmbTipoDocumento.Items.Add(new OpcionCombo() { Valor = "Factura", Texto = "Factura" });
            cmbTipoDocumento.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbTipoDocumento.ValueMember = "Value";   // decimos que valor no mostrar
            cmbTipoDocumento.SelectedIndex = 0;    //siempre va a estar seleccionado el primero
            txtFecha.Text = DateTime.Now.ToString("dd/MM/yyyy"); //le paso el formato
            txtIdProveedor.Text = "0";
            txtIdProducto.Text = "0";
            //dgvdata.Rows.Clear();
            //if (dgvdata.Rows.Count == 0)
            //{
            //    dgvdata.Rows.Clear();
            //}
        }

        private void btnBuscarProveedor_Click(object sender, EventArgs e)
        {
            using(var modal = new mdProveedor())
            {
                var result = modal.ShowDialog(); //hacemos que se muestre el formulario y cualquier accion que hagamos se guarde en la variable result
                if (result == DialogResult.OK) 
                {
                    txtIdProveedor.Text = modal._Proveedor.ID_Proveedor.ToString();
                    txtNroDocumento.Text = modal._Proveedor.Documento;
                    txtRazonSocial.Text = modal._Proveedor.Razon_Social;
                }
                else
                {
                    txtNroDocumento.Select();
                }
            }
        }

        private void btnBuscarProducto_Click(object sender, EventArgs e)
        {
            using (var modal = new mdProducto())
            {
                var result = modal.ShowDialog(); //hacemos que se muestre el formulario y cualquier accion que hagamos se guarde en la variable result
                if (result == DialogResult.OK)
                {
                    txtIdProducto.Text = modal._Producto.ID_Producto.ToString();
                    txtCodProducto.Text = modal._Producto.Codigo;
                    txtProducto.Text = modal._Producto.Nombre_Producto;
                    txtPrecioCompra.Select();
                }
                else
                {
                    txtCodProducto.Select();
                }
            }
        }

        private void txtCodProducto_KeyDown(object sender, KeyEventArgs e) //evento key down : cuando precionemos enter nos va a cargar el producto
        {
            if(e.KeyData == Keys.Enter) //si el teclado que presionamos es un enter
            {
                Producto oProducto  = new Negocio_Producto().Listar().Where(p => p.Codigo == txtCodProducto.Text && p.Estado == true).FirstOrDefault();//p es un alias (representa a una clase dentro de la lista)
                
                if(oProducto != null)
                {
                    txtCodProducto.BackColor = Color.Honeydew; //cambiamos el backcolor
                    txtIdProducto.Text = oProducto.ID_Producto.ToString();
                    txtProducto.Text = oProducto.Nombre_Producto;
                    txtPrecioCompra.Select();
                }
                else
                {
                    txtCodProducto.BackColor = Color.MistyRose; //cambiamos el backcolor
                    txtIdProducto.Text = "0";
                    txtProducto.Text = "";
                }
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            decimal preciocompra = 0;
            decimal precioventa = 0;
            bool producto_existe = false; //para verificar si existe el producto en nuestra lista

            if(int.Parse(txtIdProducto.Text) == 0) //verificar si se selecciono un producto
            {
                MessageBox.Show("Debe seleccionar un producto","Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(!decimal.TryParse(txtPrecioCompra.Text, out preciocompra)) //intenta convertir el precio de compra a un decimal 
            {
                MessageBox.Show("Precio Compra - Formato moneda incorrecto.","Mensaje",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioCompra.Select();
                return;
            }

            if (!decimal.TryParse(txtPrecioVenta.Text, out precioventa)) //intenta convertir el precio de venta a un decimal 
            {
                MessageBox.Show("Precio Venta - Formato moneda incorrecto.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                txtPrecioVenta.Select();
                return;
            }

            foreach(DataGridViewRow fila in dgvdata.Rows) //recorremos el datagridview
            {
                if (fila.Cells["IdProducto"].Value != null && fila.Cells["IdProducto"].Value.ToString() == txtIdProducto.Text)  //si el idproducto esta en el text
                {
                    producto_existe = true; //la variable existe
                    break;
                }
            }

            if(!producto_existe) //niega si es falso
            {
                dgvdata.Rows.Add(new object[]
                {
                    txtIdProducto.Text,
                    txtProducto.Text, 
                    preciocompra.ToString("0.00"),
                    precioventa.ToString("0.00"),
                    txtCantidad.Value.ToString(),
                    (txtCantidad.Value * preciocompra).ToString("0.00")
                });
                calcularTotal();
                LimpiarProducto();
                txtProducto.Select();
            }
        }
        private void LimpiarProducto()
        {
            txtIdProducto.Text = "0";
            txtCodProducto.Text = "";
            txtCodProducto.BackColor = Color.White;
            txtProducto.Text = "";
            txtPrecioCompra.Text = "";
            txtPrecioVenta.Text = "";
            txtCantidad.Value = 1;
        }

        private void calcularTotal()
        {
            decimal total = 0;
            if(dgvdata.Rows.Count > 0) //si hago algo en la tabla
            {
                foreach (DataGridViewRow row in dgvdata.Rows) //por cada fila que recorra
                {
                    if (row.Cells["IdProducto"].Value != null)
                    {
                        total += Convert.ToDecimal(row.Cells["SubTotal"].Value.ToString()); //acumula el valor del subtotal de la fila en el total
                    }
                }
            }
            txtTotalAPagar.Text = total.ToString("0.00");
        }

        private void dgvdata_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) //si la cabecera es menor a 0 que retorne, que no haga nada
                return;

            if (e.ColumnIndex == 6)
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All); //cellBounds obtiene las dimensiones de nuestra celda. DataGridViewPaintParts.all considere todas las dimensiones de la celda

                var w = Properties.Resources.tacho_basura.Width; //en w estamos almacenando el ancho de la imagen que agregamos
                var h = Properties.Resources.tacho_basura.Height; //en h estamos almacenando el alto de la imagen que agregamos
                var x = e.CellBounds.Left + (e.CellBounds.Width - w) / 2; // esto para pintar en el medio la imagen
                var y = e.CellBounds.Top + (e.CellBounds.Height - h) / 2; // esto para pintar en el medio la imagen

                e.Graphics.DrawImage(Properties.Resources.tacho_basura, new Rectangle(x, y, w, h)); // aca le indicamos que pinte.
                e.Handled = true; // permite que la accion del "click" continue
            }
        }

        private void dgvdata_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvdata.Columns[e.ColumnIndex].Name == "btnEliminar") //aca le indicamos que si hizo click en el boton hara la siguiente serie de instrucciones
            {
                int indice = e.RowIndex; //almacenamos el indice de la fila que ha sido seleccionada

                if (indice >= 0) // si se selecciono una fila valida
                {
                    dgvdata.Rows.RemoveAt(indice);
                    calcularTotal();
                }
            }
        }

        private void txtPrecioCompra_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar)) //e.KeyChar es la tecla que esta presionando el usuario
            {
                e.Handled = false; //controlador no se active
            }
            else
            {
                if (txtPrecioCompra.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")  //elimina los espacios del inicio y fin del txt (Trim). Length calcula el total del texto en el txt y la tecla es igual a un punto
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")//si esta borrando o escribiendo un punto, quiero que mi controlador sea false
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void txtPrecioVenta_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar)) //e.KeyChar es la tecla que esta presionando el usuario
            {
                e.Handled = false; //controlador no se active
            }
            else
            {
                if (txtPrecioVenta.Text.Trim().Length == 0 && e.KeyChar.ToString() == ".")  //elimina los espacios del inicio y fin del txt (Trim). Length calcula el total del texto en el txt y la tecla es igual a un punto
                {
                    e.Handled = true;
                }
                else
                {
                    if (Char.IsControl(e.KeyChar) || e.KeyChar.ToString() == ".")//si esta borrando o escribiendo un punto, quiero que mi controlador sea false
                    {
                        e.Handled = false;
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {

            if (Convert.ToInt32(txtIdProveedor.Text) == 0) //no se selecciono ningun proveedor
            {
                MessageBox.Show("Debe seleccionar un proveedor.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (dgvdata.Rows.Count < 1) //si la cantidad de filas es menor a 1
            {
                MessageBox.Show("Debe ingresar productos en la compra.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            DataTable detalle_compra = new DataTable();

            detalle_compra.Columns.Add("IdProducto", typeof(int));
            detalle_compra.Columns.Add("PrecioCompra", typeof(decimal));
            detalle_compra.Columns.Add("PrecioVenta", typeof(decimal));
            detalle_compra.Columns.Add("Cantidad", typeof(int));
            detalle_compra.Columns.Add("SubTotal", typeof(decimal));

            foreach (DataGridViewRow row in dgvdata.Rows)
            {
                detalle_compra.Rows.Add(
                    new object[]
                    {
                        row.Cells["IdProducto"].Value.ToString(),
                        row.Cells["PrecioCompra"].Value.ToString(),
                        row.Cells["PrecioVenta"].Value.ToString(),
                        row.Cells["Cantidad"].Value.ToString(),
                        row.Cells["SubTotal"].Value.ToString(),
                    });
            }
        }
    }
}
