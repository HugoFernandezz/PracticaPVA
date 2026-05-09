using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public partial class FormLaboratorio : Form
    {
        List<Envase> listaEnvases = new List<Envase>();

        const float precioAlcohol = 0.15f;
        const float precioLavanda = 1;
        const float precioSandalo = 1.25f;
        const float precioBergamota = 8.2f;

        // Esta es la ruta a tu archivo local
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=LaboratorioBD;Integrated Security=True;TrustServerCertificate=True;"; 
        public FormLaboratorio()
        {
            InitializeComponent();

            InicializarBaseDeDatosCompleta();

            trackBarAlcohol.MouseCaptureChanged += VolverVerdeAlSoltar;
            trackBarLavanda.MouseCaptureChanged += VolverVerdeAlSoltar;
            trackBarSandalo.MouseCaptureChanged += VolverVerdeAlSoltar;
            trackBarBergamota.MouseCaptureChanged += VolverVerdeAlSoltar;

            numericUDAlcohol.MouseUp += VolverVerdeAlSoltar;
            numericUDLavanda.MouseUp += VolverVerdeAlSoltar;
            numericUDSandalo.MouseUp += VolverVerdeAlSoltar;
            numericUDBergamota.MouseUp += VolverVerdeAlSoltar;

            CargarEnvases();
            actualizarProgreso();
            
        }

        private bool ExisteTabla(SqlConnection conexion, string nombreTabla)
        {
            // Esta consulta busca en las tablas del sistema de SQL Server
            string checkQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = @NombreTabla";

            using (SqlCommand checkCmd = new SqlCommand(checkQuery, conexion))
            {
                // Usamos parámetros por seguridad
                checkCmd.Parameters.AddWithValue("@NombreTabla", nombreTabla);

                // Ejecutamos y convertimos el resultado a número
                int tableCount = (int)checkCmd.ExecuteScalar();

                // Si el conteo es mayor a 0, la tabla existe
                return tableCount > 0;
            }
        }

        private void InicializarBaseDeDatosCompleta()
        {
            // Tu cadena de conexión (asegúrate de que el Database=LaboratorioBD sea correcto)
            string connectionString = @"Server=.\SQLEXPRESS; Database=LaboratorioBD; Integrated Security=True; TrustServerCertificate=True;";

            // Buscamos el archivo. Al usar Path.Combine con el nombre, 
            // asumimos que el archivo se copia a la carpeta donde corre el .exe
            string rutaArchivoSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tablasYdatos.sql");

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    // Comprobamos si falta la tabla principal (Envases). 
                    // Si falta esta, asumimos que hay que correr todo el script.
                    if (!ExisteTabla(conexion, "Envases"))
                    {
                        if (!File.Exists(rutaArchivoSql))
                        {
                            MessageBox.Show("No se encontró el archivo de configuración: " + rutaArchivoSql +
                                "\n\nRecuerda marcar el archivo .sql en Visual Studio como 'Copiar si es posterior'.",
                                "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        string scriptCompleto = File.ReadAllText(rutaArchivoSql);

                        // Cortamos el script por los "GO"
                        string[] comandos = Regex.Split(scriptCompleto, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                        foreach (string comando in comandos)
                        {
                            if (!string.IsNullOrWhiteSpace(comando))
                            {
                                using (SqlCommand cmd = new SqlCommand(comando, conexion))
                                {
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }

                        MessageBox.Show("¡Base de datos configurada íntegramente con todas sus tablas y datos!",
                            "Sincronización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al inicializar las tablas: " + ex.Message, "Error de SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void CargarEnvases()
        {
            //listViewEnvases.Items.Clear();

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                // Consulta para traer los datos
                string query = "SELECT Id, Nombre, CapacidadMl, Precio FROM Envases";
                SqlCommand comando = new SqlCommand(query, conexion);

                try
                {
                    conexion.Open();
                    SqlDataReader reader = comando.ExecuteReader();
                    int i = 0; // Para los iconos

                    while (reader.Read())
                    {
                        int id = (int)reader["Id"];
                        string nombre = reader["Nombre"].ToString();
                        int capacidad = (int)reader["CapacidadMl"];
                        decimal precio = (decimal)reader["Precio"];

                        // Formateamos el texto como lo tenías antes
                        ListViewItem item = new ListViewItem($"{capacidad}mL {nombre}\n{precio:N2}€", i);

                        // IMPORTANTE: Guardamos la capacidad en el Tag para que los límites sigan funcionando
                        item.Tag = capacidad;

                        listViewEnvases.Items.Add(item);
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }
        }

        private void trackBarAlcohol_Scroll(object sender, EventArgs e)
        {
            //Modificamos los labels de los precios y lo limitamos a 2 decimales
            lblCAlcohol.Text = ((float)numericUDAlcohol.Value * precioAlcohol).ToString("0.00") + '€';
            actualizarPrecioFinal();

            RevisarLimites(trackBarAlcohol, numericUDAlcohol);
        }

        private void numericUDAlcohol_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDAlcohol.Value <= trackBarAlcohol.Maximum) trackBarAlcohol.Value = (int)numericUDAlcohol.Value;
            RevisarLimites(trackBarAlcohol, numericUDAlcohol);
        }

        private void trackBarLavanda_Scroll(object sender, EventArgs e)
        {
            //Modificamos los labels de los precios y lo limitamos a 2 decimales
            lblCLavanda.Text = ((float)numericUDLavanda.Value * precioLavanda).ToString("0.00") + '€';
            actualizarPrecioFinal();
            RevisarLimites(trackBarLavanda, numericUDLavanda);
        }

        private void numericUDLavanda_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDLavanda.Value <= trackBarLavanda.Maximum) trackBarLavanda.Value = (int)numericUDLavanda.Value;
            RevisarLimites(trackBarLavanda, numericUDLavanda);
        }

        private void trackBarSandalo_Scroll(object sender, EventArgs e)
        {
            //Modificamos los labels de los precios y lo limitamos a 2 decimales
            lblCSandalo.Text = ((float)numericUDSandalo.Value * precioSandalo).ToString("0.00") + '€';
            actualizarPrecioFinal();

            RevisarLimites(trackBarSandalo, numericUDSandalo);
        }

        private void numericUDSandalo_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDSandalo.Value <= trackBarSandalo.Maximum) trackBarSandalo.Value = (int)numericUDSandalo.Value;
            RevisarLimites(trackBarSandalo, numericUDSandalo);

        }

        private void trackBarBergamota_Scroll(object sender, EventArgs e)
        {
            //Modificamos los labels de los precios y lo limitamos a 2 decimales
            lblCBergamota.Text = ((float)numericUDBergamota.Value * precioBergamota).ToString("0.00") + '€';
            actualizarPrecioFinal();

            RevisarLimites(trackBarBergamota, numericUDBergamota);
        }

        private void numericUDBergamota_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDBergamota.Value <= trackBarBergamota.Maximum) trackBarBergamota.Value = (int)numericUDBergamota.Value;
            RevisarLimites(trackBarBergamota, numericUDBergamota);
        }


        private void lblNombre_MouseDown(object sender, MouseEventArgs e)
        {
            if (lblNombre.Text == "Nombre")
            {
                lblNombre.Text = string.Empty;
                lblNombre.ForeColor = Color.Black;
            }
        }

        private void lblNombre_Leave(object sender, EventArgs e)
        {
            if (lblNombre.Text == string.Empty)
            {
                lblNombre.ForeColor = Color.Silver;
                lblNombre.Text = "Nombre";
            }
        }

        private void lblEmail_Leave(object sender, EventArgs e)
        {
            if (lblEmail.Text == string.Empty)
            {
                lblEmail.ForeColor = Color.Silver;
                lblEmail.Text = "Email";
            }
        }

        private void lblEmail_MouseDown(object sender, MouseEventArgs e)
        {
            if (lblEmail.Text == "Email")
            {
                lblEmail.Text = string.Empty;
                lblEmail.ForeColor = Color.Black;
            }
        }
        private void RevisarLimites(TrackBar tb, NumericUpDown num)
        {
            int max = circularProgresBar.Maximum;
            int suma = trackBarAlcohol.Value + trackBarLavanda.Value + trackBarSandalo.Value + trackBarBergamota.Value;

            if (suma > max)
            {
                circularProgresBar.ProgressColor = Color.Red;
                int exceso = suma - max;
                if (tb.Value >= exceso) tb.Value -= exceso;
                else tb.Value = 0;
            }
            
                num.Value = tb.Value;
                actualizarProgreso();
        }

        public void actualizarProgreso()

        {
            circularProgresBar.Value = trackBarAlcohol.Value + trackBarBergamota.Value + trackBarLavanda.Value + trackBarSandalo.Value;
        }

        private void cambioCapacidad(object sender, EventArgs e)
        {
            if (listViewEnvases.SelectedItems.Count > 0)
            {
                int capacidadMl = (int)listViewEnvases.SelectedItems[0].Tag;

                int sumaActual = trackBarAlcohol.Value + trackBarLavanda.Value + trackBarSandalo.Value + trackBarBergamota.Value;

                if(sumaActual > capacidadMl)
                {
                    circularProgresBar.Value = 0;
                    btnReiniciarMezcla_Click(null, null);
                }

                circularProgresBar.Maximum = capacidadMl;

                lblCapacidad.Text = "Capacidad del envase (" + capacidadMl + " ml)";
                actualizarProgreso();
            }
        }

        private void btnReiniciarMezcla_Click(object sender, EventArgs e)
        {
            numericUDAlcohol.Value = 0;
            numericUDLavanda.Value = 0;
            numericUDSandalo.Value = 0;
            numericUDBergamota.Value = 0;
            actualizarProgreso();
        }

        private void VolverVerdeAlSoltar(object sender, EventArgs e)
        {
            circularProgresBar.ProgressColor = Color.Green;
            actualizarProgreso();
        }

        private void actualizarPrecioFinal()
        {

            //Para poder sumarlo tenemos que limpiar el texto ya que tienen '€'
            float alcohol = float.Parse(lblCAlcohol.Text.Replace("€", ""));
            float bergamota = float.Parse(lblCBergamota.Text.Replace("€", ""));
            float lavanda = float.Parse(lblCLavanda.Text.Replace("€", ""));
            float sandalo = float.Parse(lblCSandalo.Text.Replace("€", ""));

            //Una vez tenemos los datos 'limpios' ya los podemos sumar
            float total = alcohol + bergamota + lavanda + sandalo;
            lblCTotal.Text = total.ToString("0.00") + "€";
        }


       
    }
}
