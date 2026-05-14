using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public partial class FormLaboratorio : Form
    {
        List<Envase> listaEnvases = new List<Envase>();
        List<Perfume> listaPerfumes = new List<Perfume>();
        private Form formActivo = null;
        private Perfume perfume = null;

        float precioAlcohol;
        float precioLavanda;
        float precioSandalo;
        float precioBergamota;

        public FormLaboratorio()
        {
            InitializeComponent();

            DatabaseService.InicializarBaseDeDatosCompleta();


            //Esto sirve para que el circulo que indica el progreso se vuelva verde
            //al soltar los trackbars
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

        private void CargarEnvases()
        {
            //listViewEnvases.Items.Clear();

            List<Envase> envases = DatabaseService.CargarEnvases();
            int i = 0; // Para los iconos

            foreach (Envase envase in envases)
            {
                // Formateamos el texto como lo tenías antes
                ListViewItem item = new ListViewItem($"{envase.CapacidadMl}mL {envase.Nombre}\n{envase.Precio:N2}€", i);

                // IMPORTANTE: Guardamos el objeto en el Tag para que los límites sigan funcionando
                item.Tag = envase;

                listViewEnvases.Items.Add(item);
                i++;
            }
        }

        private void CargarPreciosBaseDeDatos()
        {
            DatabaseService.CargarPreciosBaseDeDatos(out precioAlcohol, out precioLavanda, out precioSandalo, out precioBergamota);
        }

        private void AbrirFormHijo(Form formHijo)
        {
            using (LoginAdmin login = new LoginAdmin())
            {
                
                if (login.ShowDialog() == DialogResult.OK)
                {
                    if (formActivo != null) formActivo.Close();

                    pnlInicio.Visible = false;
                    formActivo = formHijo;
                    formHijo.TopLevel = false;
                    formHijo.FormBorderStyle = FormBorderStyle.None;
                    formHijo.Dock = DockStyle.Fill;

                    pnlContenedor.Controls.Add(formHijo);
                    formHijo.BringToFront();
                    formHijo.Show();
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
                actualizarPrecioFinal();
            }
        }

        private void btnReiniciarMezcla_Click(object sender, EventArgs e)
        {
            numericUDAlcohol.Value = 0;
            numericUDLavanda.Value = 0;
            numericUDSandalo.Value = 0;
            numericUDBergamota.Value = 0;

            lblCAlcohol.Text = "0.00€";
            lblCLavanda.Text = "0.00€";
            lblCSandalo.Text = "0.00€";
            lblCBergamota.Text = "0.00€";


            actualizarProgreso();
            actualizarPrecioFinal();
        }

        private void VolverVerdeAlSoltar(object sender, EventArgs e)
        {
            circularProgresBar.ProgressColor = Color.Green;
            actualizarProgreso();
        }

        private void actualizarPrecioFinal()
        {
            float precioEnvase = 0f;

            if (listViewEnvases.SelectedItems.Count > 0)
            {
                Envase envaseSeleccionado = (Envase)listViewEnvases.SelectedItems[0].Tag;
                precioEnvase = (float)envaseSeleccionado.Precio;
            }

            lblEnvase.Text = precioEnvase.ToString("0.00") + "€";

            //Para poder sumarlo tenemos que limpiar el texto ya que tienen '€'
            float alcohol = float.Parse(lblCAlcohol.Text.Replace("€", ""));
            float bergamota = float.Parse(lblCBergamota.Text.Replace("€", ""));
            float lavanda = float.Parse(lblCLavanda.Text.Replace("€", ""));
            float sandalo = float.Parse(lblCSandalo.Text.Replace("€", ""));

            //Una vez tenemos los datos 'limpios' ya los podemos sumar
            float total = alcohol + bergamota + lavanda + sandalo+ precioEnvase;
            lblCTotal.Text = total.ToString("0.00") + "€";

        }

        private void btnFinPedido_Click(object sender, EventArgs e)
        {
            if (comprobarCampos() == false)
            {
                return;
            }

            //Preparo los datos como hicimos en el metodo anterior
            decimal total = decimal.Parse(lblCTotal.Text.Replace("€", ""));

            //Recogemos los datos de los labels
            string nombre = lblNombre.Text;
            string email = lblEmail.Text;

            //Recuperamos el objeto envase
            Envase envase = (Envase)listViewEnvases.SelectedItems[0].Tag;

            try
            {
                //Conectamos con la BD e insertamos
                DatabaseService.GuardarPedido(nombre, email, envase, total,
                    (int)numericUDAlcohol.Value, (int)numericUDLavanda.Value,
                    (int)numericUDSandalo.Value, (int)numericUDBergamota.Value);

                //Insertamos en la lista de perfumes el perfume y habilitamos la opcion de exportar
                perfume = new Perfume(envase, (float)numericUDAlcohol.Value, (float)numericUDLavanda.Value, (float)numericUDSandalo.Value, (float)numericUDBergamota.Value);
                btnExportar.Enabled = true;
                btnHistorial.Enabled = true;
                btnExportar.BackColor = Color.DarkGreen;
                MessageBox.Show("Pedido guardado con éxito en la base de datos.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                limpiarForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar: " + ex.Message);
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

        private void lblNombre_Enter(object sender, EventArgs e)
        {
            if (lblNombre.Text == "Nombre")
            {
                lblNombre.Text = string.Empty;
                lblNombre.ForeColor = Color.Black;
            }
        }

        private void lblEmail_Enter(object sender, EventArgs e)
        {
            if (lblEmail.Text == "Email")
            {
                lblEmail.Text = string.Empty;
                lblEmail.ForeColor = Color.Black;
            }
        }

        private void limpiarForm()
        {
            //Reiniciamos los campos del cliente
            lblNombre.Text = "Nombre";
            lblNombre.ForeColor = Color.Silver;

            lblEmail.Text = "Email";
            lblEmail.ForeColor = Color.Silver;

            //Desseleccioanmos el envase del ListView
            if (listViewEnvases.SelectedItems.Count > 0)
            {
                listViewEnvases.SelectedItems[0].Selected = false;
            }

            //Reiniciar la mezcla y los precios
            btnReiniciarMezcla_Click(null, null);
        }

        private void btnExportar_Click(object sender, EventArgs e)
        {

            Exportar exportarform = new Exportar(perfume);

            exportarform.ShowDialog();
            
        }

        

        

        private void btnHistorial_Click(object sender, EventArgs e)
        {
            AbrirFormHijo(new Historial());
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (formActivo != null)
            {
                formActivo.Close();
                formActivo = null;
            }
            //Hacemos visible el form del inicio
            pnlInicio.Visible = true;

            //Lo traemos al frente por si acaso
            pnlInicio.BringToFront();
        }

        
    }
}
