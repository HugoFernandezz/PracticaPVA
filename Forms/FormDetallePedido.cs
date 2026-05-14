using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public partial class FormDetallePedido : Form
    {
        private int perfumeId;
        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=LaboratorioBD;Integrated Security=True;TrustServerCertificate=True;";

        public FormDetallePedido(int id)
        {
            InitializeComponent();
            this.perfumeId = id;
            CargarDetalles();
        }

        private void CargarDetalles()
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    
                    // 1. Obtener datos generales del perfume y el envase
                    string queryInfo = @"
                        SELECT P.NombreCliente, P.EmailCliente, P.PrecioTotal, P.FechaCreacion, 
                               E.Id as EnvaseId, E.Nombre as EnvaseNombre, E.CapacidadMl
                        FROM Perfumes P
                        JOIN Envases E ON P.EnvaseId = E.Id
                        WHERE P.Id = @id";
                    
                    SqlCommand cmdInfo = new SqlCommand(queryInfo, conexion);
                    cmdInfo.Parameters.AddWithValue("@id", perfumeId);
                    
                    using (SqlDataReader reader = cmdInfo.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            lblCliente.Text = "Cliente: " + reader["NombreCliente"].ToString();
                            lblMail.Text = "Email: " + reader["EmailCliente"].ToString();
                            lblEnvase.Text = "Envase: " + reader["EnvaseNombre"].ToString();
                            lblCapacidad.Text = "Capacidad: " + reader["CapacidadMl"].ToString() + " ml";

                            //Se comprueba antes que la fecha sea nula
                            string fecha = reader["FechaCreacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaCreacion"]).ToShortDateString() : "Sin fecha";
                            lblDetallePedido.Text = "Pedido #" + perfumeId + " - " + fecha;

                            
                            int envaseId = (int)reader["EnvaseId"];
                            AsignarImagen(envaseId);
                        }
                    }

                    // 2. Obtener desglose de ingredientes de la tabla MateriasPrimas
                    string queryEsencias = @"
                        SELECT M.Nombre, DP.CantidadMl
                        FROM DetallePerfumes DP
                        JOIN MateriasPrimas M ON DP.EsenciaId = M.Id
                        WHERE DP.PerfumeId = @id AND DP.CantidadMl > 0";
                    
                    SqlCommand cmdEsencias = new SqlCommand(queryEsencias, conexion);
                    cmdEsencias.Parameters.AddWithValue("@id", perfumeId);
                    
                    SqlDataAdapter adapter = new SqlDataAdapter(cmdEsencias);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // --- Lógica de llenado del TableLayoutPanel ---
                    tableLayoutPanel.Controls.Clear();
                    tableLayoutPanel.RowStyles.Clear();
                    tableLayoutPanel.RowCount = dt.Rows.Count;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        // Añadimos una fila con tamaño automático para que no se corten los textos
                        tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));

                        Label lblNombre = new Label { 
                            Text = dt.Rows[i]["Nombre"].ToString(), 
                            AutoSize = true, 
                            Font = new Font("Segoe UI", 10, FontStyle.Bold),
                            Padding = new Padding(0, 5, 0, 5)
                        };
                        
                        Label lblCant = new Label { 
                            Text = dt.Rows[i]["CantidadMl"].ToString() + " ml", 
                            AutoSize = true,
                            Font = new Font("Segoe UI", 10),
                            Padding = new Padding(0, 5, 0, 5)
                        };

                        // Añadimos los controles a las columnas 0 y 1 de la fila i
                        tableLayoutPanel.Controls.Add(lblNombre, 0, i);
                        tableLayoutPanel.Controls.Add(lblCant, 1, i);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar detalles: " + ex.Message);
                }
            }
        }

        private void AsignarImagen(int envaseId)
        {
            // Mapeo de IDs basado en el orden de Envases
            int index = -1;
            switch (envaseId)
            {
                case 1: index = 0; break;
                case 2: index = 1; break;
                case 4: index = 2; break;
                case 5: index = 3; break;
                case 6: index = 4; break;
                case 7: index = 5; break;
                case 8: index = 6; break;
                case 9: index = 7; break;
                case 11: index = 8; break;
                case 12: index = 9; break;
            }

            // Usamos el ImageList que configuraste en el diseñador
            if (index != -1 && index < imageListEnvasesDetalle.Images.Count)
            {
                pictureBox.Image = imageListEnvasesDetalle.Images[index];
            }
        }

        
    }
}
