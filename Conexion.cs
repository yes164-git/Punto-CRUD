using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;

namespace Punto
{
    internal class Conexion
    {
        private readonly string cadenaConexion = "Server=localhost;Database=PuntoDB;User ID=root;Password=;Port=3306;SslMode=None;";

        public MySqlConnection obtenerConexion()
        {
            MySqlConnection conectar = new MySqlConnection(cadenaConexion);
            try
            {
                conectar.Open();
                return conectar;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar con la base de datos MSQL" + ex.Message, "Error de conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
