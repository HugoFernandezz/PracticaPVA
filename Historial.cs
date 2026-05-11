using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iText.Layout;

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
                    P.EmailCliente AS [Email],
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

        private void btnExportarTodo_Click(object sender, EventArgs e)
        {
            ExportarTodo exportarTodoForm = new ExportarTodo(dgvHistorial);

            exportarTodoForm.ShowDialog();
        }

        private void btnEliminarRegistro_Click(object sender, EventArgs e)
        {
            //Veriricar que el usuario tenga alguna fila seleccionada
            if (dgvHistorial.SelectedRows.Count > 0)
            {
                //Un dialog para confirmar la accion
                DialogResult result = MessageBox.Show(
                    $"¿Estás seguro de que deseas eliminar {dgvHistorial.SelectedRows.Count} pedido(s) permanentemente?",
                    "Confirmar eliminación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    using (SqlConnection conexion = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conexion.Open();

                            //Recorremos todas las filas seleccionadas (por si seleccionas varias)
                            foreach (DataGridViewRow fila in dgvHistorial.SelectedRows)
                            {
                                //Obtenemos el ID del perfume de la celda "Id"
                                int idEliminar = Convert.ToInt32(fila.Cells["Id"].Value);

                                /* IMPORTANTE: como el perfume lleva asociado "DetallesPerfume" los cuales se
                                 * asocia mediante una FK, debemos eliminar primero estos registros de DetallesPerfume
                                 */
                                string sqlDetalles = "DELETE FROM DetallePerfumes WHERE PerfumeId = @id";
                                using (SqlCommand cmdDet = new SqlCommand(sqlDetalles, conexion))
                                {
                                    cmdDet.Parameters.AddWithValue("@id", idEliminar);
                                    cmdDet.ExecuteNonQuery();
                                }
                                // Ahora ya podemos borrar directamente el perfume
                                string sqlPerfume = "DELETE FROM Perfumes WHERE Id = @id";
                                using (SqlCommand cmdPerf = new SqlCommand(sqlPerfume, conexion))
                                {
                                    cmdPerf.Parameters.AddWithValue("@id", idEliminar);
                                    cmdPerf.ExecuteNonQuery();
                                }
                            }
                            MessageBox.Show("Eliminación completada con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            //Recargamos los datos para q se actualice el componente del gridview
                            CargarDatos();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar: " + ex.Message, "Error de SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una o varias filas completas haciendo clic en la parte izquierda de la tabla.",
                                "Selección necesaria", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnModificarRegistro_Click(object sender, EventArgs e)
        {
            if (dgvHistorial.CurrentCell == null) return;
            //Recuperamos todo lo que necesitamos de la celda que esta seleccionada
            string columnaVisible = dgvHistorial.CurrentCell.OwningColumn.Name;
            int idPerfume = Convert.ToInt32(dgvHistorial.CurrentRow.Cells["Id"].Value);
            string valorActual = dgvHistorial.CurrentCell.Value.ToString();

            //Como en las columnas que aparecen en el gridView no son los nombre reales que tienen
            //Las columnas en la base de datos, tenemos que hacer esta "conversion"
            string columnaSQL = "";
            if (columnaVisible == "Cliente") columnaSQL = "NombreCliente";
            else if (columnaVisible == "Email") columnaSQL = "EmailCliente";
            if (string.IsNullOrEmpty(columnaSQL))
            {
                MessageBox.Show("Este campo no se puede editar directamente para proteger la integridad de los costes.", "Aviso");
                return;
            }
            //Abrimos el Form de editar campo
            using (FormEditarCampo popup = new FormEditarCampo(columnaVisible, valorActual))
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    string nuevoValor = popup.NuevoValor;
                    //Le pasamos el id del perfume, la columna que tiene q modificar y el nuevo valor
                    ActualizarBaseDeDatos(idPerfume, columnaSQL, nuevoValor);
                    CargarDatos();
                }
            }
        }

        private void ActualizarBaseDeDatos(int id, string columnaSQL, string nuevoValor)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    // Usamos el nombre de la columna dinámicamente y el valor por parámetro
                    string sql = $"UPDATE Perfumes SET {columnaSQL} = @valor WHERE Id = @id";
                    using (SqlCommand cmd = new SqlCommand(sql, conexion))
                    {
                        cmd.Parameters.AddWithValue("@valor", nuevoValor);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("¡Dato actualizado con éxito!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar la base de datos: " + ex.Message);
                }
            }
        }

    }
}

