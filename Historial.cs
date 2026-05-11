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
    public partial class Historial : Form
    {

        string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=LaboratorioBD;Integrated Security=True;TrustServerCertificate=True;";
        public Historial()
        {
            InitializeComponent();
            CargarDatos();
        }

        private void CargarDatos()
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
            
                    //Lo de INSULL es para que en la celda ponga un 0 en vez de dejarla vacia
                    string query = @"
                SELECT 
                    P.Id, 
                    P.NombreCliente AS [Cliente], 
                    E.Nombre AS [Envase],
                    -- Buscamos la cantidad de cada esencia por su ID
                    ISNULL((SELECT CantidadMl FROM DetallePerfumes WHERE PerfumeId = P.Id AND EsenciaId = 1), 0) AS [Alcohol (ml)],
                    ISNULL((SELECT CantidadMl FROM DetallePerfumes WHERE PerfumeId = P.Id AND EsenciaId = 2), 0) AS [Lavanda (ml)],
                    ISNULL((SELECT CantidadMl FROM DetallePerfumes WHERE PerfumeId = P.Id AND EsenciaId = 3), 0) AS [Sándalo (ml)],
                    ISNULL((SELECT CantidadMl FROM DetallePerfumes WHERE PerfumeId = P.Id AND EsenciaId = 4), 0) AS [Bergamota (ml)],
                    P.PrecioTotal AS [Total (€)],
                    P.FechaCreacion AS [Fecha]
                FROM Perfumes P
                JOIN Envases E ON P.EnvaseId = E.Id
                ORDER BY P.FechaCreacion DESC";
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conexion);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    // Asignamos el resultado al datagridviiew
                    dgvHistorial.DataSource = dt;
                    // Formateo visual opcional: poner el total con 2 decimales
                    dgvHistorial.Columns["Total (€)"].DefaultCellStyle.Format = "N2";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el historial detallado: " + ex.Message);
                }
            }
        }


    }
}
