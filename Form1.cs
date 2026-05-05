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

            ListViewItem envase2 = new ListViewItem("30mL Cubo Modernista\n9,80€", 0);
            envase2.Tag = 30;

            ListViewItem envase3 = new ListViewItem("30mL Gota Orgánica\n12,40€", 0);
            envase3.Tag = 30;

            ListViewItem envase4 = new ListViewItem("50mL Prisma Hexagonal\n18,60€", 0);
            envase4.Tag = 50;

            ListViewItem envase5 = new ListViewItem("50mL Esfera Clásica\n15,90", 0);
            envase5.Tag = 50;

            ListViewItem envase6 = new ListViewItem("50mL Frasco de Botica\n10, 20€", 0);
            envase6.Tag = 50;

            ListViewItem envase7 = new ListViewItem("100mL Torre Estilizada\n24,50€", 0);
            envase7.Tag = 100;

            ListViewItem envase8 = new ListViewItem("100mL Óvalo Ergonómico\n21,80€", 0);
            envase8.Tag = 100;

            ListViewItem envase9 = new ListViewItem("100mL Bloque de Lujo\n32€", 0);
            envase9.Tag = 100;

            listViewEnvases.Items.Add(envase1);
            listViewEnvases.Items.Add(envase2);
            listViewEnvases.Items.Add(envase3);
            listViewEnvases.Items.Add(envase4);
            listViewEnvases.Items.Add(envase5);
            listViewEnvases.Items.Add(envase6);
            listViewEnvases.Items.Add(envase7);
            listViewEnvases.Items.Add(envase8);
            listViewEnvases.Items.Add(envase9);

        }

        private void trackBarAlcohol_Scroll(object sender, EventArgs e)
        {
            int suma = trackBarAlcohol.Value + trackBarLavanda.Value + trackBarSandalo.Value + trackBarBergamota.Value;

            if (suma > 100)
            {
                trackBarAlcohol.Value -= (suma - 100);
            }

            numericUDAlcohol.Value = trackBarAlcohol.Value;
            actualizarProgreso();
        }

        private void numericUDAlcohol_ValueChanged(object sender, EventArgs e)
        {
            trackBarAlcohol.Value = (int)numericUDAlcohol.Value;
        }

        private void trackBarLavanda_Scroll(object sender, EventArgs e)
        {
            int suma = trackBarAlcohol.Value + trackBarLavanda.Value + trackBarSandalo.Value + trackBarBergamota.Value;

            if (suma > 100)
            {
                trackBarLavanda.Value -= (suma - 100);
            }

            numericUDLavanda.Value = trackBarLavanda.Value;
            actualizarProgreso();
        }

        private void numericUDLavanda_ValueChanged(object sender, EventArgs e)
        {
            trackBarLavanda.Value = (int)numericUDLavanda.Value;
        }

        private void trackBarSandalo_Scroll(object sender, EventArgs e)
        {
            int suma = trackBarAlcohol.Value + trackBarLavanda.Value + trackBarSandalo.Value + trackBarBergamota.Value;

            if (suma > 100)
            {
                trackBarSandalo.Value -= (suma - 100);
            }

            numericUDSandalo.Value = trackBarSandalo.Value;
            actualizarProgreso();
        }

        private void numericUDSandalo_ValueChanged(object sender, EventArgs e)
        {
            trackBarSandalo.Value = (int)numericUDSandalo.Value;
        }

        private void trackBarBergamota_Scroll(object sender, EventArgs e)
        {
            int suma = trackBarAlcohol.Value + trackBarLavanda.Value + trackBarSandalo.Value + trackBarBergamota.Value;

            if (suma > 100)
            {
                trackBarBergamota.Value -= (suma - 100);
            }

            numericUDBergamota.Value = trackBarBergamota.Value;
            actualizarProgreso();
        }

        private void numericUDBergamota_ValueChanged(object sender, EventArgs e)
        {
            trackBarBergamota.Value = (int)numericUDBergamota.Value;
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

        private void progressBar_Click(object sender, EventArgs e)
        {

        }

        public void actualizarProgreso()
        {
            int sumaActual = trackBarAlcohol.Value + trackBarBergamota.Value +
                      trackBarLavanda.Value + trackBarSandalo.Value;

            // Actualizamos el nuevo control circular
            circularProgresBar.Value = sumaActual;

            if (sumaActual > circularProgresBar.Maximum)
            {
                circularProgresBar.ProgressColor = Color.Red;
            }
            else
            {
                circularProgresBar.ProgressColor = Color.Green;
            }
        }

        private void listViewEnvases_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listViewEnvases.SelectedItems.Count > 0) 
            {
                int capacidadMl = (int)listViewEnvases.SelectedItems[0].Tag;

                circularProgresBar.Maximum = capacidadMl;
                actualizarProgreso();
            }
        }
    }
}
