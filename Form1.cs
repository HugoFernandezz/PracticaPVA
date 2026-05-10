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
        List<Perfume> listaPerfumes = new List<Perfume>();

        float precioAlcohol;
        float precioLavanda;
        float precioSandalo;
        float precioBergamota;

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
            CargarPreciosBaseDeDatos();
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

                        // IMPORTANTE: Guardamos el objeto en el Tag para que los límites sigan funcionando
                        item.Tag = new Envase { Id = id, Nombre = nombre, CapacidadMl = capacidad, Precio = precio };

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
            //Comprobamos que no este ya el maximo de la capacidad
            if (circularProgresBar.Value < circularProgresBar.Maximum)
            {
                lblCAlcohol.Text = ((float)trackBarAlcohol.Value * precioAlcohol).ToString("0.00") + '€';
                actualizarPrecioFinal();
            }
            //Limita el trackbar al maximo de la capacidad elegida en el envase y añade la funcionalidad de que se ponga de color rojo
            RevisarLimites(trackBarAlcohol, numericUDAlcohol);
        }

        private void numericUDAlcohol_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDAlcohol.Value <= trackBarAlcohol.Maximum) trackBarAlcohol.Value = (int)numericUDAlcohol.Value;
            RevisarLimites(trackBarAlcohol, numericUDAlcohol);
        }

        private void trackBarLavanda_Scroll(object sender, EventArgs e)
        {
            if (circularProgresBar.Value < circularProgresBar.Maximum)
            {
                lblCLavanda.Text = ((float)trackBarLavanda.Value * precioLavanda).ToString("0.00") + '€';
                actualizarPrecioFinal();
            }
            RevisarLimites(trackBarLavanda, numericUDLavanda);
        }

        private void numericUDLavanda_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDLavanda.Value <= trackBarLavanda.Maximum) trackBarLavanda.Value = (int)numericUDLavanda.Value;
            RevisarLimites(trackBarLavanda, numericUDLavanda);
        }

        private void trackBarSandalo_Scroll(object sender, EventArgs e)
        {
            if (circularProgresBar.Value < circularProgresBar.Maximum)
            {
                lblCSandalo.Text = ((float)trackBarSandalo.Value * precioSandalo).ToString("0.00") + '€';
                actualizarPrecioFinal();
            }
            RevisarLimites(trackBarSandalo, numericUDSandalo);
        }

        private void numericUDSandalo_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDSandalo.Value <= trackBarSandalo.Maximum) trackBarSandalo.Value = (int)numericUDSandalo.Value;
            RevisarLimites(trackBarSandalo, numericUDSandalo);

        }

        private void trackBarBergamota_Scroll(object sender, EventArgs e)
        {
            if (circularProgresBar.Value < circularProgresBar.Maximum)
            {
                lblCBergamota.Text = ((float)trackBarBergamota.Value * precioBergamota).ToString("0.00") + '€';
                actualizarPrecioFinal();
            }
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
                //recupero el objeto seleccionado de la lista y me quedo con sus mL maximos que es lo que me interesa
                Envase envaseSeleccionado = (Envase)listViewEnvases.SelectedItems[0].Tag;
                int capacidadMl = envaseSeleccionado.CapacidadMl;

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

        private void btnFinPedido_Click(object sender, EventArgs e)
        {
            if (comprobarCampos() == false)
            {
                return;
            }

            //Preparo los datos como hicimos en el metodo anterior
            float alcohol = float.Parse(lblCAlcohol.Text.Replace("€", ""));
            float bergamota = float.Parse(lblCBergamota.Text.Replace("€", ""));
            float lavanda = float.Parse(lblCLavanda.Text.Replace("€", ""));
            float sandalo = float.Parse(lblCSandalo.Text.Replace("€", ""));
            decimal total = decimal.Parse(lblCTotal.Text.Replace("€", ""));

            //Recogemos los datos de los labels
            string nombre = lblNombre.Text;
            string email = lblEmail.Text;

            //Recuperamos el objeto envase
            Envase envase = (Envase)listViewEnvases.SelectedItems[0].Tag;


            //Conectamos con ls BD e insertamos
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    //INSERT para el Perfume
                    //Usamos OUTPUT INSERTED.Id para obtener el ID que nos asigne la base de datos automáticamente
                    string queryPerfume = "INSERT INTO Perfumes (NombreCliente, EmailCliente, EnvaseId, PrecioTotal) " +
                                          "OUTPUT INSERTED.Id " +
                                          "VALUES (@nom, @em, @envId, @total)";

                    SqlCommand cmd = new SqlCommand(queryPerfume, conexion);

                    cmd.Parameters.AddWithValue("@nom", nombre);
                    cmd.Parameters.AddWithValue("@em", email);
                    cmd.Parameters.AddWithValue("@envId", envase.Id);
                    cmd.Parameters.AddWithValue("@total", total);

                    //Ejecutamos y guardamos el ID generado
                    int idGenerado = (int)cmd.ExecuteScalar();

                    //Hacemos un INSERT por cada ingrediente que tenga cantidad > 0
                    GuardarDetalle(idGenerado, 1, (int)numericUDAlcohol.Value, conexion);
                    GuardarDetalle(idGenerado, 2, (int)numericUDLavanda.Value, conexion);
                    GuardarDetalle(idGenerado, 3, (int)numericUDSandalo.Value, conexion);
                    GuardarDetalle(idGenerado, 4, (int)numericUDBergamota.Value, conexion);

                    //Insertamos en la lista de perfumes el perfume y habilitamos la opcion de exportar
                    listaPerfumes.Add(new Perfume(envase, (float)numericUDAlcohol.Value, (float)numericUDBergamota.Value, (float)numericUDLavanda.Value, (float)numericUDSandalo.Value));
                    btnExportar.Enabled = true;
                    btnExportar.BackColor = Color.DarkGreen;
                    MessageBox.Show("Pedido guardado con éxito en la base de datos.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar: " + ex.Message);
                }
            }

        }

        private bool comprobarCampos()
        {

            //Comprobaciones de que no falten campos
            if (lblEmail.Text == string.Empty || lblEmail.Text == "Email" || lblNombre.Text == string.Empty || lblNombre.Text == "Nombre")
            {
                MessageBox.Show("Por favor, complete todos los campos obligatorios.", "Campos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Comprobacion de que hay alguna esencia seleccionada
            if (lblCTotal.Text == "0.00€")
            {
                MessageBox.Show("Por favor, añada alguna esencia al perfume.", "Camposicion incompleta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            //Comprobacion de que hay algun envase seleccionado
            if (listViewEnvases.SelectedItems.Count == 0) 
            {
                MessageBox.Show("Por favor, seleccione el envase que quiera para su perfume.", "Envase no seleccionado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            } 


            return true;
        }

        private void GuardarDetalle(int perfumeId, int esenciaId, int cantidad, SqlConnection conexion)
        {
            if (cantidad <= 0) return; // Si no hay cantidad, no guardamos fila
            string query = "INSERT INTO DetallePerfumes (PerfumeId, EsenciaId, CantidadMl) VALUES (@pId, @eId, @cant)";
            SqlCommand cmd = new SqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@pId", perfumeId);
            cmd.Parameters.AddWithValue("@eId", esenciaId);
            cmd.Parameters.AddWithValue("@cant", cantidad);
            cmd.ExecuteNonQuery();
        }

        //Cargamos los precios de las esencias desde la base de datos
        private void CargarPreciosBaseDeDatos()
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    string query = "SELECT Nombre, PrecioPorMl FROM MateriasPrimas";
                    SqlCommand cmd = new SqlCommand(query, conexion);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string nombre = reader["Nombre"].ToString().ToLower();
                        float precio = Convert.ToSingle(reader["PrecioPorMl"]);
                        if (nombre.Contains("alcohol")) precioAlcohol = precio;
                        else if (nombre.Contains("lavanda")) precioLavanda = precio;
                        else if (nombre.Contains("sandalo")) precioSandalo = precio;
                        else if (nombre.Contains("bergamota")) precioBergamota = precio;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudieron cargar los precios: " + ex.Message);
                }
            }
        }
    }
}
