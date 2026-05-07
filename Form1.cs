using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public partial class FormLaboratorio : Form
    {
        List<Envase> listaEnvases = new List<Envase>();
        public FormLaboratorio()
        {
            InitializeComponent();

            CargarEnvases();
            actualizarProgreso();
            
        }

        private void CargarEnvases()
        {
            listViewEnvases.Items.Clear();

            ListViewItem envase1 = new ListViewItem("20mL Vial Minimalista\n5,50€", 0);
            envase1.Tag = 20;

            ListViewItem envase2 = new ListViewItem("30mL Cubo Modernista\n9,80€", 1);
            envase2.Tag = 30;

            ListViewItem envase3 = new ListViewItem("30mL Gota Orgánica\n12,40€", 2);
            envase3.Tag = 30;

            ListViewItem envase4 = new ListViewItem("50mL Prisma Hexagonal\n18,60€", 3);
            envase4.Tag = 50;

            ListViewItem envase5 = new ListViewItem("50mL Esfera Clásica\n15,90", 4);
            envase5.Tag = 50;

            ListViewItem envase6 = new ListViewItem("50mL Frasco de Botica\n10, 20€", 5);
            envase6.Tag = 50;

            ListViewItem envase7 = new ListViewItem("100mL Torre Estilizada\n24,50€", 6);
            envase7.Tag = 100;

            ListViewItem envase8 = new ListViewItem("100mL Óvalo Ergonómico\n21,80€", 7);
            envase8.Tag = 100;

            ListViewItem envase9 = new ListViewItem("100mL Bloque de Lujo\n32€", 8);
            envase9.Tag = 100;

            ListViewItem envase10 = new ListViewItem("200mL Bloque de Lujo\n50€", 9);
            envase10.Tag = 100;

            listViewEnvases.Items.Add(envase1);
            listViewEnvases.Items.Add(envase2);
            listViewEnvases.Items.Add(envase3);
            listViewEnvases.Items.Add(envase4);
            listViewEnvases.Items.Add(envase5);
            listViewEnvases.Items.Add(envase6);
            listViewEnvases.Items.Add(envase7);
            listViewEnvases.Items.Add(envase8);
            listViewEnvases.Items.Add(envase9);
            listViewEnvases.Items.Add(envase10);

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
