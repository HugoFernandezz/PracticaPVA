using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;


namespace PF26_48848727Q_24470742F_77658838M_54800134N
{
    internal class CircularProgresBar : Control
    {
        private int _value = 0;
        private int _maximum = 100;

        public int Value
        {
            get => _value;
            set { _value = Math.Min(Maximum, Math.Max(0, value)); Invalidate(); }
        }

        public int Maximum
        {
            get => _maximum;
            set { _maximum = value; Invalidate(); }
        }

        public Color ProgressColor { get; set; } = Color.DodgerBlue;
        public Color BaseColor { get; set; } = Color.LightGray;
        public int LineWidth { get; set; } = 10;

        public void CircularProgressBar()
        {
            // Evita el parpadeo al redibujar
            this.DoubleBuffered = true;
            this.Size = new Size(150, 150);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Definir el área de dibujo restando el grosor de la línea
            int rectSize = Math.Min(Width, Height) - LineWidth;
            Rectangle rect = new Rectangle(LineWidth / 2, LineWidth / 2, rectSize, rectSize);

            // 1. Dibujar el círculo de fondo (la base)
            using (Pen penBase = new Pen(BaseColor, LineWidth))
            {
                e.Graphics.DrawEllipse(penBase, rect);
            }

            // 2. Calcular el ángulo del progreso (360 grados es el total)
            float sweepAngle = (float)Value / Maximum * 360;

            // 3. Dibujar el arco de progreso
            using (Pen penProgress = new Pen(ProgressColor, LineWidth))
            {
                // LineCap.Round hace que los extremos se vean redondeados y modernos
                penProgress.StartCap = LineCap.Round;
                penProgress.EndCap = LineCap.Round;

                // Empezamos en -90 grados para que inicie arriba al centro
                e.Graphics.DrawArc(penProgress, rect, -90, sweepAngle);
            }

            // 4. Dibujar el texto del porcentaje en el centro
            string text = $"{(int)((float)Value / Maximum * 100)}%";
            Font font = new Font("Segoe UI", rectSize / 5, FontStyle.Bold);
            Size textSize = TextRenderer.MeasureText(text, font);

            e.Graphics.DrawString(text, font, Brushes.Black,
                (Width - textSize.Width) / 2,
                (Height - textSize.Height) / 2);
        }
    }
}
