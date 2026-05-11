using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    public partial class ExportarTodo : Form
    {

        private DataGridView dgvHistorial;

        public ExportarTodo(DataGridView dgv)
        {
            InitializeComponent();
            dgvHistorial = dgv;
        }


        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportarExcel();
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            ExportarPDF();
        }

        private void ExportarExcel()
        {
            if (dgvHistorial.Rows.Count == 0)
            {
                MessageBox.Show("No hay datos en el historial para exportar.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog save = new SaveFileDialog { Filter = "Archivo Excel CSV (*.csv)|*.csv", FileName = "Historial_Completo.csv" };

            if (save.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(save.FileName, false, Encoding.UTF8))
                    {
                        //escribir cabeceras 
                        string columnas = "";
                        for (int i = 0; i < dgvHistorial.ColumnCount; i++)
                        {
                            columnas += dgvHistorial.Columns[i].HeaderText + (i < dgvHistorial.ColumnCount - 1 ? ";" : "");
                        }
                        sw.WriteLine(columnas);

                        //Escribir las filas
                        foreach (DataGridViewRow fila in dgvHistorial.Rows)
                        {
                            if (!fila.IsNewRow)
                            {
                                string linea = "";
                                for (int i = 0; i < dgvHistorial.ColumnCount; i++)
                                {
                                    linea += (fila.Cells[i].Value?.ToString() ?? "") + (i < dgvHistorial.ColumnCount - 1 ? ";" : "");
                                }
                                sw.WriteLine(linea);
                            }
                        }
                    }
                    MessageBox.Show("Historial exportado a Excel con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al exportar: " + ex.Message);
                }
            }
        }

        private void ExportarPDF()
        {
            if (dgvHistorial.Rows.Count == 0) return;

            SaveFileDialog sfd = new SaveFileDialog { Filter = "Archivos PDF (*.pdf)|*.pdf", FileName = "Historial_Detallado.pdf" };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (PdfWriter writer = new PdfWriter(sfd.FileName))
                    using (PdfDocument pdf = new PdfDocument(writer))
                    using (Document document = new Document(pdf))
                    {
                        document.Add(new Paragraph("HISTORIAL COMPLETO DE PEDIDOS")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(18));

                        document.Add(new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n"));

                        //tabla con el nºcolumnas necesarias
                        Table table = new Table(dgvHistorial.ColumnCount).UseAllAvailableWidth();

                        //cabeceras
                        foreach (DataGridViewColumn col in dgvHistorial.Columns)
                        {
                            table.AddHeaderCell(new Cell().Add(new Paragraph(col.HeaderText)));
                        }

                        //filas
                        foreach (DataGridViewRow row in dgvHistorial.Rows)
                        {
                            if (!row.IsNewRow)
                            {
                                foreach (DataGridViewCell cell in row.Cells)
                                {
                                    table.AddCell(cell.Value?.ToString() ?? "");
                                }
                            }
                        }

                        document.Add(table);
                    }
                    MessageBox.Show("PDF generado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar PDF: " + ex.Message);
                }
            }
        }


    }
}
