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
                        string query = "SELECT COUNT(*) FROM usuarios WHERE usuario = @usuario AND password = @password";

                        MySqlCommand comando = new MySqlCommand(query, conexion);

                        comando.Parameters.AddWithValue("@usuario", txtUser.Text);
                        comando.Parameters.AddWithValue("@password", txtPassword.Text);

                        int existe = Convert.ToInt32(comando.ExecuteScalar());
                        if (existe > 0)
                        {
                            MessageBox.Show("¡Bienvenido al sistema!", "Ingreso Exitoso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Aquí abrirías tu siguiente formulario (Menú Principal)
                            frmPrincipal menu = new frmPrincipal();
                            menu.Show();
                            this.Hide(); // Oculta el Login
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
