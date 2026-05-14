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
    public partial class Exportar : Form
    {

        private Perfume ultimoPerfume;
        public Exportar(Perfume perfume)
        {
            InitializeComponent();
            ultimoPerfume = perfume;
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            ExportarExcel();
        }

        private void btnPDF_Click(object sender, EventArgs e)
        {
            ExportarPDF();
        }

        private void ExportarPDF()
        {

            //Configuramos el diálogo de guardado
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Archivos PDF (*.pdf)|*.pdf";
            sfd.FileName = "Resumen_Pedido.pdf";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (PdfWriter writer = new PdfWriter(sfd.FileName))
                    using (PdfDocument pdf = new PdfDocument(writer))
                    using (Document document = new Document(pdf))
                    {
                        // Título
                        document.Add(new Paragraph("RESUMEN DEL PEDIDO - LABORATORIO PERFUMES")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(20));

                        // Usamos los datos de "ultimoPedido" porque las etiquetas (Labels) ya están vacías
                        document.Add(new Paragraph($"Cliente: {ultimoPerfume.NombreCliente}"));
                        document.Add(new Paragraph($"Email: {ultimoPerfume.EmailCliente}"));
                        document.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}"));
                        document.Add(new Paragraph("\n"));

                        // Tabla de detalles
                        Table table = new Table(2).UseAllAvailableWidth();
                        table.AddHeaderCell("Concepto");
                        table.AddHeaderCell("Detalle");

                        table.AddCell("Envase");
                        table.AddCell($"{ultimoPerfume.Envase?.Nombre ?? "N/A"} ({ultimoPerfume.Envase?.CapacidadMl ?? 0}ml)");

                        table.AddCell("Alcohol");
                        table.AddCell($"{ultimoPerfume.Alcohol} ml");

                        table.AddCell("Lavanda");
                        table.AddCell($"{ultimoPerfume.Lavanda} ml");

                        table.AddCell("Sándalo");
                        table.AddCell($"{ultimoPerfume.Sandalo} ml");

                        table.AddCell("Bergamota");
                        table.AddCell($"{ultimoPerfume.Bergamota} ml");

                        document.Add(table);

                        // Total
                        document.Add(new Paragraph($"\nTOTAL A PAGAR: {ultimoPerfume.Precio:0.00}EUR")
                            .SetTextAlignment(TextAlignment.RIGHT)

                            .SetFontSize(14));
                    }

                    MessageBox.Show("PDF generado con éxito.", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    string errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    MessageBox.Show("Error real: " + errorReal, "Fallo detectado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ExportarExcel()
        {

            SaveFileDialog save = new SaveFileDialog { Filter = "Archivo Excel CSV (*.csv)|*.csv", FileName = "Ultimo_Pedido.csv" };

            if (save.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(save.FileName, false, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine("Cliente;Email;Envase;Alcohol;Bergamota;Lavanda;Sandalo;Total");

                    string envase = ultimoPerfume.Envase.Nombre;

                    sw.WriteLine($"{ultimoPerfume.NombreCliente};{ultimoPerfume.EmailCliente};{envase};{ultimoPerfume.Alcohol};{ultimoPerfume.Bergamota};{ultimoPerfume.Lavanda};{ultimoPerfume.Sandalo};{ultimoPerfume.Precio}€");
                }
                MessageBox.Show("Exportado con éxito.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


    }
}
