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
        public FormLaboratorio()
        {
            InitializeComponent();
            actualizarProgreso();
        }

        private void trackBarAlcohol_Scroll(object sender, EventArgs e)
        {
            numericUDAlcohol.Value = trackBarAlcohol.Value;
            actualizarProgreso();
        }

        private void numericUDAlcohol_ValueChanged(object sender, EventArgs e)
        {
            trackBarAlcohol.Value = (int)numericUDAlcohol.Value;
        }

        private void trackBarLavanda_Scroll(object sender, EventArgs e)
        {
            numericUDLavanda.Value = trackBarLavanda.Value;
            actualizarProgreso();
        }

        private void numericUDLavanda_ValueChanged(object sender, EventArgs e)
        {
            trackBarLavanda.Value = (int)numericUDLavanda.Value;
        }

        private void trackBarSandalo_Scroll(object sender, EventArgs e)
        {
            numericUDSandalo.Value = trackBarSandalo.Value;
            actualizarProgreso();
        }

        private void numericUDSandalo_ValueChanged(object sender, EventArgs e)
        {
            trackBarSandalo.Value = (int)numericUDSandalo.Value;
        }

        private void trackBarBergamota_Scroll(object sender, EventArgs e)
        {
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
    }
}
