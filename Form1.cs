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

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public partial class FormLaboratorio : Form
    {
        List<Envase> listaEnvases = new List<Envase>();

        // Esta es la ruta a tu archivo local
        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=LaboratorioBD;Integrated Security=True;Encrypt=True;Encrypt=False";
        public FormLaboratorio()
        {
            InitializeComponent();

            CargarEnvases();
            actualizarProgreso();
            
        }

        private void CargarEnvases()
        {
            listViewEnvases.Items.Clear();

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
            RevisarLimites(trackBarAlcohol, numericUDAlcohol);
        }

        private void numericUDAlcohol_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDAlcohol.Value <= trackBarAlcohol.Maximum) trackBarAlcohol.Value = (int)numericUDAlcohol.Value;
            RevisarLimites(trackBarAlcohol, numericUDAlcohol);
        }

        private void trackBarLavanda_Scroll(object sender, EventArgs e)
        {
           RevisarLimites(trackBarLavanda, numericUDLavanda);
        }

        private void numericUDLavanda_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDLavanda.Value <= trackBarLavanda.Maximum) trackBarLavanda.Value = (int)numericUDLavanda.Value;
            RevisarLimites(trackBarLavanda, numericUDLavanda);
        }

        private void trackBarSandalo_Scroll(object sender, EventArgs e)
        {
            RevisarLimites(trackBarSandalo, numericUDSandalo);
        }

        private void numericUDSandalo_ValueChanged(object sender, EventArgs e)
        {
            if (numericUDSandalo.Value <= trackBarSandalo.Maximum) trackBarSandalo.Value = (int)numericUDSandalo.Value;
            RevisarLimites(trackBarSandalo, numericUDSandalo);

        }

        private void trackBarBergamota_Scroll(object sender, EventArgs e)
        {
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

        private void listViewEnvases_SelectedIndexChanged(object sender, EventArgs e)
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

        
    }
}
