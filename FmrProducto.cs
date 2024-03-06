using Capa_Entidad;
using Capa_Negocio;
using ClosedXML.Excel;
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
using System.Windows.Media.Animation;

namespace Sistema_Ventas
{
    public partial class FmrProducto : Form
    {
        public FmrProducto()
        {
            InitializeComponent();
        }

        private void FmrProducto_Load(object sender, EventArgs e)
        {
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 1, Texto = "Activo" }); //cargamos los datos que debe mostarar el combo box
            cmbEstado.Items.Add(new OpcionCombo() { Valor = 0, Texto = "No Activo" });

            cmbEstado.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbEstado.ValueMember = "Value";   // decimos que valor no mostrar
            cmbEstado.SelectedIndex = 0;    //siempre va a estar seleccionado el primero


            List<Categoria> listaCategoria = new Negocio_Categoria().Listar();
            foreach (Categoria item in listaCategoria) //para cada rol en mi lista..
            {
                cmbCategoria.Items.Add(new OpcionCombo() { Valor = item.ID_Categoria, Texto = item.Descripcion });
            }
            cmbCategoria.DisplayMember = "Texto";  //decimos que valor mostrar
            cmbCategoria.ValueMember = "Value";   // decimos que valor no mostrar
            cmbCategoria.SelectedIndex = 0;    //siempre va a estar seleccionado el primero



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

            //MOSTRAR TODOS LOS PRODUCTOS
            List<Producto> lista = new Negocio_Producto().Listar();
            foreach (Producto item in lista) //para cada rol en mi lista..
            {
                dgwData.Rows.Add(new object[]
                {   "",
                    item.ID_Producto,
                    item.Codigo,
                    item.Nombre_Producto,
                    item.Descripcion,
                    item.obj_Categoria.ID_Categoria,
                    item.obj_Categoria.Descripcion,
                    item.stock,
                    item.Precio_Compra,
                    item.Precio_Venta,
                    item.Estado == true ?1 : 0, //en caso de que sea true, que mueste un 1. Caso contrario que muestre un 0 
                    item.Estado == true ? "Activo" : " No Activo"
                });
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            string Mensaje = string.Empty;
            Producto obj_Producto = new Producto()
            {
                ID_Producto = string.IsNullOrEmpty(txtId.Text) ? 0 : Convert.ToInt32(txtId.Text),
                Codigo = txtCodigo.Text,
                Nombre_Producto = txtNombre.Text,
                Descripcion = txtDescripcion.Text,
                obj_Categoria = new Categoria() { ID_Categoria = Convert.ToInt32(((OpcionCombo)cmbCategoria.SelectedItem).Valor) },
                Estado = Convert.ToInt32(((OpcionCombo)cmbEstado.SelectedItem).Valor) == 1 ? true : false
            };

            if (obj_Producto.ID_Producto == 0)
            {
                int idgenerado = new Negocio_Producto().Registrar(obj_Producto, out Mensaje);

                if (idgenerado != 0)
                {
                    dgwData.Rows.Add(new object[]
                    {
                     "",
                     idgenerado,
                     txtCodigo.Text,
                     txtNombre.Text,
                     txtDescripcion.Text,
                    ((OpcionCombo)cmbCategoria.SelectedItem).Valor.ToString(),
                    ((OpcionCombo)cmbCategoria.SelectedItem).Texto.ToString(),
                    "0",
                    "0.00",
                    "0.00",
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
                bool resultado = new Negocio_Producto().Editar(obj_Producto, out Mensaje);
                if (resultado == true)
                {
                    DataGridViewRow row = dgwData.Rows[Convert.ToInt32(txtIndice.Text)];
                    row.Cells["Id"].Value = txtId.Text;
                    row.Cells["Codigo"].Value = txtCodigo.Text;
                    row.Cells["Nombre"].Value = txtNombre.Text;
                    row.Cells["Descripcion"].Value = txtDescripcion.Text;
                    row.Cells["IdCategoria"].Value = ((OpcionCombo)cmbCategoria.SelectedItem).Valor.ToString();
                    row.Cells["Categoria"].Value = ((OpcionCombo)cmbCategoria.SelectedItem).Texto.ToString();
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
            txtCodigo.Text = "";
            txtNombre.Text = "";
            txtDescripcion.Text = "";
            cmbCategoria.SelectedIndex = 0;
            cmbEstado.SelectedIndex = 0;
            txtCodigo.Select();
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
                    txtCodigo.Text = dgwData.Rows[indice].Cells["Codigo"].Value.ToString(); //
                    txtNombre.Text = dgwData.Rows[indice].Cells["Nombre"].Value.ToString(); //
                    txtDescripcion.Text = dgwData.Rows[indice].Cells["Descripcion"].Value.ToString(); //

                    foreach (OpcionCombo oc in cmbCategoria.Items)
                    {
                        if (Convert.ToInt32(oc.Valor) == Convert.ToInt32(dgwData.Rows[indice].Cells["IdCategoria"].Value)) //convertimos a int para que lo que estemos igualando sea del mismo tipo. //Si es que el valor es igual al que se esta mostrando 
                        {
                            int indice_combo = cmbCategoria.Items.IndexOf(oc);
                            cmbCategoria.SelectedIndex = indice_combo;
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
                if (MessageBox.Show("¿Desea eliminar el producto?", "Mensaje", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {

                    string Mensaje = string.Empty;
                    Producto obj_Producto = new Producto()
                    {
                        ID_Producto = Convert.ToInt32(txtId.Text)
                    };

                    bool respuesta = new Negocio_Producto().Eliminar(obj_Producto, out Mensaje);

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
                MessageBox.Show("Debe seleccionar algun Usuario para eliminar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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


        private void btnExportar_Click(object sender, EventArgs e)
        {
            if (dgwData.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos para exportar.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                DataTable dt = new DataTable(); // creo una tabla
                foreach(DataGridViewColumn columna in dgwData.Columns) //recorro las columnas de mi datagridview
                {
                    if(columna.HeaderText != "" && columna.Visible) //si la cabecera de la columna es distinto a vacio
                    {
                        dt.Columns.Add(columna.HeaderText,typeof(string)); //agrego las cabeceras de las columnas y casteo a modo string
                    }
                }
                foreach(DataGridViewRow fila in dgwData.Rows)//recorro las filas de mi datagridview
                {
                    if (fila.Visible) //si la fila es visible
                    {
                        dt.Rows.Add(new object[]
                        {
                            fila.Cells[2].Value.ToString(), //codigo
                            fila.Cells[3].Value.ToString(),//nombre
                            fila.Cells[4].Value.ToString(),//descripcion
                            fila.Cells[6].Value.ToString(),//categoria
                            fila.Cells[7].Value.ToString(),//stock
                            fila.Cells[8].Value.ToString(),//precio compra
                            fila.Cells[9].Value.ToString(),//precio venta
                            fila.Cells[11].Value.ToString(),//estado
                        });
                    }
                }
                SaveFileDialog savefile = new SaveFileDialog(); //creamos savefiledialog para guardar el archivo
                savefile.FileName = string.Format("Reporte producto_{0}.xlsx", DateTime.Now.ToString("dd-MM-yyy")); //le indicamos el nombre y la fecha con horario 
                savefile.Filter = "Excel Files | *.xslx"; //permite mostrar a archivos con ese tipo de extension

                if(savefile.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        XLWorkbook wb = new XLWorkbook(); //creamos archivo excel
                        var hoja = wb.Worksheets.Add(dt, "Informe"); //agregamos una hoja
                        hoja.ColumnsUsed().AdjustToContents(); //indicamos que se ajuste el ancho de las columnas segun el valor que tengan
                        wb.SaveAs(savefile.FileName);
                        MessageBox.Show("Reporte generado.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch 
                    {
                        MessageBox.Show("Error al generar reporte.", "Mensaje", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
            }
        }
    }

}
