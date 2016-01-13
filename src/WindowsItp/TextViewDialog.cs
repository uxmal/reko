using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Reko.WindowsItp
{
    public partial class TextViewDialog : Form
    {
        private string text = "M";
        private string textStub;
        private StringFormat sf;
        private int xLow;
        private int xHigh;
        private TextFormatFlags tf;
        private int cxTotal;
        private int xCursor;

        public TextViewDialog()
        {
            InitializeComponent();
            this.sf = new StringFormat(StringFormat.GenericTypographic);
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces;
            tf = TextFormatFlags.NoPadding | TextFormatFlags.GlyphOverhangPadding;
        }

        private void TextViewDialog_Paint(object sender, PaintEventArgs e)
        {
            var sz = MeasureText(e.Graphics, text, this.Font);
            e.Graphics.FillRectangle(Brushes.Red, 0, 0, xCursor, sz.Height);
            e.Graphics.FillRectangle(Brushes.Black, xCursor, 0, 1, sz.Height);
            e.Graphics.FillRectangle(Brushes.Red, xCursor+1, 0, sz.Width - xCursor - 1, sz.Height);
            DrawText(e.Graphics, text, this.Font, new Point(0, 0), Color.Gray);

            //Debug.Print("== cursor {0} ==", xCursor);
            //e.Graphics.FillRectangle(Brushes.White, xCursor - 1, 0, 2, sz.Height);

            e.Graphics.FillRectangle(Brushes.Yellow, 0, sz.Height, sz.Width, sz.Height);
            DrawText(e.Graphics, textStub, this.Font, new Point(0, sz.Height), SystemColors.ControlText);
            e.Graphics.FillRectangle(Brushes.Black, 0, sz.Height * 2, cxTotal, 10);
        }

        private void DrawText(Graphics g, string text, Font font, Point pt, Color color)
        {
            TextRenderer.DrawText(g, text, font, pt, SystemColors.ControlText, tf);
        }

        private Size MeasureText(Graphics graphics, string text, Font font)
        {
            var sz = TextRenderer.MeasureText(graphics, text, this.Font, new Size(0, 0), tf);
            return sz;

            //var bounds = new RectangleF();
            //using (var textPath = new System.Drawing.Drawing2D.GraphicsPath())
            //{
            //    textPath.AddString(
            //        text,
            //        font.FontFamily,
            //        (int)font.Style,
            //        font.Size,
            //        new PointF(0, 0),
            //        StringFormat.GenericTypographic);
            //    bounds = textPath.GetBounds();
            //}
            //return new Size((int)bounds.Width, (int)bounds.Height);
        }

        private void TextViewDialog_MouseDown(object sender, MouseEventArgs e)
        {
            var g = CreateGraphics();
            int low = 0;
            int high = text.Length;
            var sz = MeasureText(g, text, this.Font);
            this.xLow = 0;
            this.xHigh = sz.Width;
            this.cxTotal = sz.Width;
            this.textStub = text;
            //Debug.Print("---");
            var pt = new Point(0, 0);
            //Debug.Print("Initial {0,4} {1,4} [{2,2}-{3,2}] {4}", xLow, xHigh, low, high, text);
            while (low < high - 1)
            {
                int mid = low + (high - low) / 2;
                textStub = mid >= text.Length ? text : text.Remove(mid);
                sz = MeasureText(g, textStub, this.Font);
                if (e.X < sz.Width)
                {
                    high = mid;
                    xHigh = sz.Width;
                }
                else
                {
                    low = mid;
                    xLow = sz.Width;
                }
                //Debug.Print("{0,4} {1,4} [{2,2}-{3,2}-{4,2}] {5}", e.X, sz.Width, low, mid, high, textStub);
            }
            //Debug.Print("Final {0,4} {1,4} [{2,2}-{3,2}] {4}", xLow, xHigh, low, high, textStub);
            int cx = (xHigh - xLow) / 2;
            int idx;
            if (e.X - xLow > cx)
            {
                xCursor = xHigh;
                idx = high;
            }
            else
            {
                xCursor = xLow;
                idx = low;
            }
            textStub = idx >= text.Length ? text : text.Remove(idx);

            g.Dispose();
            Invalidate();
        }
    }
}
