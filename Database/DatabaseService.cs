using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    internal static class DatabaseService
    {
        // Esta es la ruta a tu archivo local
        public static string ConnectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=LaboratorioBD;Integrated Security=True;TrustServerCertificate=True;";

        private static bool TieneDatos(SqlConnection conexion, string nombreTabla)
        {
            string query = $"SELECT COUNT(*) FROM {nombreTabla}";
            try
            {
                using (SqlCommand cmd = new SqlCommand(query, conexion))
                {
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private static bool ExisteTabla(SqlConnection conexion, string nombreTabla)
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

        public static void InicializarBaseDeDatosCompleta()
        {
            // Conectamos primero a master para asegurar que la base de datos existe
            string connectionMaster = @"Server=.\SQLEXPRESS; Database=master; Integrated Security=True; TrustServerCertificate=True;";
            string connectionDb = @"Server=.\SQLEXPRESS; Database=LaboratorioBD; Integrated Security=True; TrustServerCertificate=True;";

            // Buscamos el archivo SQL
            string rutaArchivoSql = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tablasYdatos.sql");

            try
            {
                // 1. Crear la base de datos si no existe
                using (SqlConnection conexionMaster = new SqlConnection(connectionMaster))
                {
                    conexionMaster.Open();
                    string queryCreateDB = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LaboratorioBD') CREATE DATABASE LaboratorioBD;";
                    using (SqlCommand cmd = new SqlCommand(queryCreateDB, conexionMaster))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // 2. Conectar a LaboratorioBD para verificar tablas e insertar datos
                using (SqlConnection conexion = new SqlConnection(connectionDb))
                {
                    conexion.Open();

                    // Comprobamos si falta la tabla de MateriasPrimas o si está vacía.
                    if (!ExisteTabla(conexion, "MateriasPrimas") || !TieneDatos(conexion, "MateriasPrimas"))
                    {
                        if (!File.Exists(rutaArchivoSql))
                        {
                            MessageBox.Show("No se encontró el archivo de configuración: " + rutaArchivoSql +
                                "\n\nRecuerda marcar el archivo .sql en Visual Studio como 'Copiar si es posterior'.",
                                "Archivo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Leemos con Unicode (UTF-16) porque el archivo tiene BOM FF-FE de SQL Server
                        string scriptCompleto = File.ReadAllText(rutaArchivoSql, Encoding.Unicode);

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

                        MessageBox.Show("¡Base de datos y tablas configuradas automáticamente!",
                            "Sincronización Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al inicializar la base de datos: " + ex.Message, "Error de SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static List<Envase> CargarEnvases()
        {
            List<Envase> envases = new List<Envase>();

            using (SqlConnection conexion = new SqlConnection(ConnectionString))
            {
                // Consulta para traer los datos
                string query = "SELECT Id, Nombre, CapacidadMl, Precio FROM Envases";
                SqlCommand comando = new SqlCommand(query, conexion);

                try
                {
                    conexion.Open();
                    SqlDataReader reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        int id = (int)reader["Id"];
                        string nombre = reader["Nombre"].ToString();
                        int capacidad = (int)reader["CapacidadMl"];
                        decimal precio = (decimal)reader["Precio"];

                        envases.Add(new Envase { Id = id, Nombre = nombre, CapacidadMl = capacidad, Precio = precio });
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar: " + ex.Message);
                }
            }

            return envases;
        }

        //Cargamos los precios de las esencias desde la base de datos
        public static void CargarPreciosBaseDeDatos(out float precioAlcohol, out float precioLavanda, out float precioSandalo, out float precioBergamota)
        {
            precioAlcohol = 0;
            precioLavanda = 0;
            precioSandalo = 0;
            precioBergamota = 0;

            using (SqlConnection conexion = new SqlConnection(ConnectionString))
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

        public static void GuardarDetalle(int perfumeId, int esenciaId, int cantidad, SqlConnection conexion)
        {
            if (cantidad <= 0) return; // Si no hay cantidad, no guardamos fila
            string query = "INSERT INTO DetallePerfumes (PerfumeId, EsenciaId, CantidadMl) VALUES (@pId, @eId, @cant)";
            SqlCommand cmd = new SqlCommand(query, conexion);
            cmd.Parameters.AddWithValue("@pId", perfumeId);
            cmd.Parameters.AddWithValue("@eId", esenciaId);
            cmd.Parameters.AddWithValue("@cant", cantidad);
            cmd.ExecuteNonQuery();
        }

        public static int GuardarPedido(string nombre, string email, Envase envase, decimal total,
            int cantAlcohol, int cantLavanda, int cantSandalo, int cantBergamota)
        {
            using (SqlConnection conexion = new SqlConnection(ConnectionString))
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
                GuardarDetalle(idGenerado, 1, cantAlcohol, conexion);
                GuardarDetalle(idGenerado, 2, cantLavanda, conexion);
                GuardarDetalle(idGenerado, 3, cantSandalo, conexion);
                GuardarDetalle(idGenerado, 4, cantBergamota, conexion);

                return idGenerado;
            }
        }
    }
}
