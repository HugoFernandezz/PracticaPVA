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
    public partial class FormEditarCampo : Form
    {
        public string NuevoValor { get; private set; }
        public FormEditarCampo(string nombreCampo, string valorActual)
        {
            InitializeComponent();

            this.Label.Text = "Editando campo: " + nombreCampo;
            this.TextBox.Text = valorActual;
            this.Text = "Modificar " + nombreCampo;

            //Hacemos que al pulsar Enter se guarde automáticamente
            this.AcceptButton = btnGuardar;
            this.CancelButton = btnCancelar;
        }
        private void btnGuardar_Click(object sender, EventArgs e)
        {
            
            NuevoValor = TextBox.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
