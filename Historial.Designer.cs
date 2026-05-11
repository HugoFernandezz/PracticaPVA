namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    partial class Historial
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvHistorial = new System.Windows.Forms.DataGridView();
            this.btnEliminarRegistro = new System.Windows.Forms.Button();
            this.btnModificarRegistro = new System.Windows.Forms.Button();
            this.btnExportarTodo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvHistorial
            // 
            this.dgvHistorial.AllowUserToAddRows = false;
            this.dgvHistorial.AllowUserToDeleteRows = false;
            this.dgvHistorial.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHistorial.Location = new System.Drawing.Point(37, 75);
            this.dgvHistorial.Name = "dgvHistorial";
            this.dgvHistorial.Size = new System.Drawing.Size(765, 374);
            this.dgvHistorial.TabIndex = 0;
            // 
            // btnEliminarRegistro
            // 
            this.btnEliminarRegistro.Location = new System.Drawing.Point(341, 30);
            this.btnEliminarRegistro.Name = "btnEliminarRegistro";
            this.btnEliminarRegistro.Size = new System.Drawing.Size(131, 23);
            this.btnEliminarRegistro.TabIndex = 1;
            this.btnEliminarRegistro.Text = "Eliminar registro";
            this.btnEliminarRegistro.UseVisualStyleBackColor = true;
            // 
            // btnModificarRegistro
            // 
            this.btnModificarRegistro.Location = new System.Drawing.Point(519, 30);
            this.btnModificarRegistro.Name = "btnModificarRegistro";
            this.btnModificarRegistro.Size = new System.Drawing.Size(115, 23);
            this.btnModificarRegistro.TabIndex = 2;
            this.btnModificarRegistro.Text = "Modificar registro";
            this.btnModificarRegistro.UseVisualStyleBackColor = true;
            // 
            // btnExportarTodo
            // 
            this.btnExportarTodo.Location = new System.Drawing.Point(678, 29);
            this.btnExportarTodo.Name = "btnExportarTodo";
            this.btnExportarTodo.Size = new System.Drawing.Size(110, 23);
            this.btnExportarTodo.TabIndex = 3;
            this.btnExportarTodo.Text = "Exportar todo";
            this.btnExportarTodo.UseVisualStyleBackColor = true;
            // 
            // Historial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnExportarTodo);
            this.Controls.Add(this.btnModificarRegistro);
            this.Controls.Add(this.btnEliminarRegistro);
            this.Controls.Add(this.dgvHistorial);
            this.Name = "Historial";
            this.Text = "Historial";
            ((System.ComponentModel.ISupportInitialize)(this.dgvHistorial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvHistorial;
        private System.Windows.Forms.Button btnEliminarRegistro;
        private System.Windows.Forms.Button btnModificarRegistro;
        private System.Windows.Forms.Button btnExportarTodo;
    }
}