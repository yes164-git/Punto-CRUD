using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;

namespace Punto.Forms
{
    public partial class frmProductos : Form
    {
        public frmProductos()
        {
            InitializeComponent();
        }

        private void frmProductos_Load(object sender, System.EventArgs e)
        {
            CargarProductos();
        }
        private void CargarProductos()
        {
            Conexion conBD = new Conexion();

            using (MySqlConnection conexion = conBD.obtenerConexion())
            {
                if (conexion != null)
                {
                    try
                    {
                        string query = "SELECT producto_id, codigo, descripcion, precio, stock FROM productos";

                        MySqlCommand comando = new MySqlCommand(query, conexion);

                        MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);

                        DataTable tablaVirtual = new DataTable();

                        adaptador.Fill(tablaVirtual);

                        dgvProductos.DataSource = tablaVirtual;
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al cargar el catálogo de productos: " + ex.Message,
                                        "Error de Carga", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnNuevo_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text) || string.IsNullOrWhiteSpace(txtPrecio.Text) ||
                string.IsNullOrWhiteSpace(txtStock.Text) || cmbCategorias.SelectedIndex == -1)
            {
                MessageBox.Show("Por favor, llene todos los campos y seleccione una categoría.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // (TryParse)
            decimal precioValido;
            int stockValido;

            if (!decimal.TryParse(txtPrecio.Text, out precioValido))
            {
                MessageBox.Show("El precio debe ser un número válido (ejemplo: 15.50).", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPrecio.Focus();
                return;
            }

            if (!int.TryParse(txtStock.Text, out stockValido))
            {
                MessageBox.Show("El stock debe ser un número entero (ejemplo: 10).", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtStock.Focus();
                return;
            }

            Conexion conBD = new Conexion();

            using (MySqlConnection conexion = conBD.obtenerConexion())
            {
                if (conexion != null)
                {
                    try
                    {
                        string query = "INSERT INTO productos (descripcion, precio, stock, categoria) VALUES (@descripcion, @precio, @stock, @categoria)";

                        MySqlCommand comando = new MySqlCommand(query, conexion);

                        comando.Parameters.AddWithValue("@descripcion", txtNombre.Text);
                        comando.Parameters.AddWithValue("@precio", precioValido);
                        comando.Parameters.AddWithValue("@stock", stockValido);
                        comando.Parameters.AddWithValue("@categoria", cmbCategorias.Text);

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("¡Producto guardado exitosamente!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LimpiarFormulario();

                            CargarProductos();
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al guardar en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void LimpiarFormulario()
        {
            txtCodigo.Clear();
            txtNombre.Clear();
            txtPrecio.Clear();
            txtStock.Clear();
            cmbCategorias.SelectedIndex = -1;
            txtCodigo.Focus();
        }

        private void dgvProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow fila = dgvProductos.Rows[e.RowIndex];

                lblId.Text = fila.Cells[0].Value.ToString(); 
                txtCodigo.Text = fila.Cells[1].Value.ToString();
                txtNombre.Text = fila.Cells[2].Value.ToString(); 
                txtPrecio.Text = fila.Cells[3].Value.ToString(); 
                txtStock.Text = fila.Cells[4].Value.ToString();
            }
        }
    }
}

