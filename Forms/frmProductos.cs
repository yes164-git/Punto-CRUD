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
        private void btnEditar_Click(object sender, EventArgs e)
        {
            // VALIDACIÓN DE SEGURIDAD: Verificar que haya un ID seleccionado
            if (string.IsNullOrWhiteSpace(lblId.Text))
            {
                MessageBox.Show("Por favor, seleccione primero un producto de la tabla para editar.", "Ningún Producto Seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // VALIDACIÓN DE CAMPOS VACÍOS
            if (string.IsNullOrWhiteSpace(txtCodigo.Text) || string.IsNullOrWhiteSpace(txtNombre.Text) ||
                string.IsNullOrWhiteSpace(txtPrecio.Text) || string.IsNullOrWhiteSpace(txtStock.Text))
            {
                MessageBox.Show("Por favor, llene todos los campos antes de actualizar.", "Campos Vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // VALIDACIÓN NUMÉRICA SEGURA (TryParse)
            decimal precioValido;
            int stockValido;

            if (!decimal.TryParse(txtPrecio.Text, out precioValido))
            {
                MessageBox.Show("El precio debe ser un número válido.", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPrecio.Focus();
                return;
            }

            if (!int.TryParse(txtStock.Text, out stockValido))
            {
                MessageBox.Show("El stock debe ser un número entero.", "Formato Incorrecto", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtStock.Focus();
                return;
            }

            // ACTUALIZACIÓN EN LA BASE DE DATOS (Usando la estructura real)
            Conexion conBD = new Conexion();

            using (MySqlConnection conexion = conBD.obtenerConexion())
            {
                if (conexion != null)
                {
                    try
                    {
                        string query = "UPDATE productos SET codigo = @codigo, descripcion = @descripcion, precio = @precio, stock = @stock WHERE producto_id = @id";

                        MySqlCommand comando = new MySqlCommand(query, conexion);

                        // Pasamos los parámetros ordenados
                        comando.Parameters.AddWithValue("@codigo", txtCodigo.Text);
                        comando.Parameters.AddWithValue("@descripcion", txtNombre.Text);
                        comando.Parameters.AddWithValue("@precio", precioValido);
                        comando.Parameters.AddWithValue("@stock", stockValido);
                        comando.Parameters.AddWithValue("@id", lblId.Text); // El ID inhabilitado que frena el UPDATE

                        int filasAfectadas = comando.ExecuteNonQuery();

                        if (filasAfectadas > 0)
                        {
                            MessageBox.Show("¡Producto actualizado exitosamente!", "Éxito al Editar", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LimpiarFormulario();
                            CargarProductos(); // Refresca tu DataGridView inmediatamente
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el producto para actualizar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al actualizar en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            // VALIDACIÓN DE SEGURIDAD: Verificar que haya un ID seleccionado en la tabla
            if (string.IsNullOrWhiteSpace(lblId.Text))
            {
                MessageBox.Show("Por favor, seleccione primero un producto de la tabla para poder eliminarlo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Confirmar primero la acción mediante un MessageBox interactivo
            DialogResult respuesta = MessageBox.Show(
                "¿Está completamente seguro de que desea eliminar este producto permanentemente de la base de datos?",
                "Confirmar Eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Si el usuario acepta (hace clic en el botón "Sí")
            if (respuesta == DialogResult.Yes)
            {
                Conexion conBD = new Conexion();

                using (MySqlConnection conexion = conBD.obtenerConexion())
                {
                    if (conexion != null)
                    {
                        try
                        {
                            // LO QUE TE PIDEN: Eliminar permanentemente usando la cláusula WHERE y la llave primaria (producto_id)
                            string query = "DELETE FROM productos WHERE producto_id = @id";

                            MySqlCommand comando = new MySqlCommand(query, conexion);
                            comando.Parameters.AddWithValue("@id", lblId.Text); // Pasamos el ID bloqueado de forma segura

                            // Ejecutamos la eliminación en MySQL
                            int filasAfectadas = comando.ExecuteNonQuery();

                            if (filasAfectadas > 0)
                            {
                                MessageBox.Show("El producto ha sido eliminado permanentemente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // Actualizar la rejilla visual de datos inmediatamente
                                LimpiarFormulario(); // Deja las cajas en blanco para el siguiente movimiento
                                CargarProductos();   // Vuelve a llenar el DataGridView para que el producto ya no aparezca
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Error al eliminar el producto en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            // Si el usuario presiona "No", el código de arriba se ignora por completo y no pasa nada.
        }
    }
}

