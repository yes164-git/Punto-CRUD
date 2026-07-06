using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace Punto.Forms
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUser.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Por favor, llene todos los campos (Usuario y Contraseña).", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                txtUser.Focus();

                return;
            }
            MessageBox.Show("Campos validados correctamente. Buscando en la base de datos...", "Éxito");

            Conexion conBD = new Conexion();

            using (MySqlConnection conexion = conBD.obtenerConexion())
            {
                if (conexion != null)
                {
                    try
                    {
                        string query = "SELECT COUNT(*) FROM usuarios WHERE username = @username AND PASSWORD = @PASSWORD";

                        MySqlCommand comando = new MySqlCommand(query, conexion);

                        comando.Parameters.AddWithValue("@username", txtUser.Text);
                        comando.Parameters.AddWithValue("@PASSWORD", txtPassword.Text);

                        object resultado = comando.ExecuteScalar();

                        // Validamos que no sea nulo y convertimos de forma segura
                        if (resultado != null && Convert.ToInt32(resultado) > 0)
                        {
                            // ¡Aquí pones tu código de acceso exitoso!
                            MessageBox.Show("¡Bienvenido al sistema!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            frmProductos pantallaPrincipal = new frmProductos();
                            pantallaPrincipal.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usuario o contraseña incorrectos.", "Error de Credenciales", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (MySqlException ex)
                    {
                        MessageBox.Show("Error al buscar el usuario: " + ex.Message, "Error de Consulta", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
