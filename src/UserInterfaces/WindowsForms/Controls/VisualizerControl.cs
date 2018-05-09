#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using Reko.Core;
using Reko.Gui.Visualizers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class VisualizerControl : Control
    {
        private Visualizer visualizer;
        private MemoryArea mem;
        private VScrollBar vscroll;
        private int pixelSize;

        public VisualizerControl()
        {
            this.pixelSize = 2;

            this.vscroll = new VScrollBar();
            this.vscroll.Dock = DockStyle.Right;
            this.vscroll.ValueChanged += Vscroll_ValueChanged;
            this.vscroll.Visible = true;

            this.Controls.Add(vscroll);
        }

  

        public int LineLength {get; set; }

        public int LinesOnScreen => (this.Height + (this.pixelSize - 1)) / pixelSize;

        public MemoryArea MemoryArea
        {
            get {  return mem;}
            set { mem = value; OnMemoryAreaChanged(); }
        }

        public Visualizer Visualizer
        {
            get { return visualizer; }
            set { visualizer = value; OnVisualizerChanged(); }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            var bytesOnScreen = LineLength * LinesOnScreen;
            if (bytesOnScreen >= mem.Bytes.Length ||
                visualizer != null && !visualizer.ShowScrollbar)
            {
                this.vscroll.Value = 0;
                this.vscroll.Enabled = false;
            }
            else
            {
                this.vscroll.Enabled = true;
                this.vscroll.Maximum = mem.Bytes.Length - bytesOnScreen;
                this.vscroll.LargeChange = bytesOnScreen;
                this.vscroll.SmallChange = LineLength;
            }
            Invalidate();
            base.OnSizeChanged(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var bmp = new Bitmap(Width, Height, e.Graphics))
            using (var g = Graphics.FromImage(bmp))
            using (var bytesImage = RenderVisualization())
            {
                g.FillRectangle(Brushes.Black, ClientRectangle);

                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                g.DrawImage(
                    bytesImage,
                    new Rectangle(0, 0, bytesImage.Width * pixelSize, bytesImage.Height * pixelSize));

                RenderAnnotations(g);

                e.Graphics.DrawImage(bmp, 0, 0);
            }
        }

        private Image RenderVisualization()
        {
            Bitmap bmp = new Bitmap(
                this.LineLength,
                this.LinesOnScreen,
                PixelFormat.Format32bppArgb);
            if (Visualizer == null || MemoryArea == null)
                return bmp;

            var bgPattern = new[] { Color.FromArgb(0x7F,0,0), Color.FromArgb(0x30,0x00,0x00) };
            var addrStart = mem.BaseAddress + vscroll.Value;
            var bytesOnScreen = LinesOnScreen * LineLength;
            var colors = visualizer.RenderBuffer(mem, addrStart, bytesOnScreen, null);
            int x = 0;
            int y = 0;
            foreach (var byteColor in colors)
            {
                Color color;
                if (byteColor == 0)
                {
                    color = bgPattern[((x & 2) ^ (y & 2)) >> 1];
                }
                else
                {
                    color = Color.FromArgb(byteColor);
                }
                bmp.SetPixel(x, y, color);
                x = (x + 1) % LineLength;
                if (x == 0)
                {
                    y = y + 1;
                }
            }
            int cVoid = bytesOnScreen - colors.Length;
            if (cVoid > 0)
            {
                for (int i = 0; i < cVoid; ++i)
                {
                    var color = bgPattern[((x & 2) ^ (y & 2)) >> 1];
                    bmp.SetPixel(x, y, color);
                }
            }
            return bmp;
        }

        private void RenderAnnotations(Graphics g)
        {
            if (visualizer == null)
                return;
            var addrStart = mem.BaseAddress + vscroll.Value;
            var bytesOnScreen = LinesOnScreen * LineLength;
            var annotations = visualizer.RenderAnnotations(addrStart, bytesOnScreen, null);
            if (annotations.Length == 0)
                return;

            const int spacing = 20;
            int xStart = LineLength * pixelSize + spacing;
            int yStart = (int)g.MeasureString("M", Font).Height;
            int xOffs = 5;
            int yOffs = yStart;
            foreach (var annotation in annotations)
            {
                var txtColor = annotation.TextColor != 0 
                    ? Color.FromArgb(annotation.TextColor) 
                    : Color.White;
                using (var textBrush = new SolidBrush(txtColor))
                {
                    g.DrawString(annotation.Text, this.Font, textBrush, xStart + 10, (yStart + yOffs) / 2);
                }

                // Draw a line from the address to the text

                var pt = PositionFromAddress(annotation.Address);
                var lineColor = annotation.LineColor != 0
                    ? Color.FromArgb(annotation.LineColor)
                    : Color.White;
                using (var pen = new Pen(lineColor))
                {
                    g.DrawLines(pen, new Point[]
                    {
                        new Point(xStart + xOffs, (yStart + yOffs) / 2 + yStart / 2),

                        new Point(xStart + xOffs - 4 - spacing, (yStart + yOffs) / 2 + yStart / 2), // left
                        new Point(xStart + xOffs - 4 - spacing, ((pt.Y / 10) * 9) + this.pixelSize / 2), // down
                        new Point(pt.X + this.pixelSize / 2, ((pt.Y / 10) * 9) + this.pixelSize / 2), // left
                        new Point(pt.X + this.pixelSize / 2, pt.Y+ this.pixelSize / 2)
                    });
                }

                yOffs += 2 * yStart + 5;
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // We paint our own background in OnPaint to avoid flicker.
        }

        // Property changed handlers
        protected virtual void OnVisualizerChanged()
        {
            if (visualizer == null)
            {
                LineLength = 16;
            }
            else
            {
                this.LineLength = visualizer.DefaultLineLength;
                this.vscroll.SmallChange = LineLength;
                this.pixelSize = visualizer.DefaultZoom;
            }
            this.Invalidate();
        }

        protected virtual void OnMemoryAreaChanged()
        {
            this.Invalidate();
        }

        private void Vscroll_ValueChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        private Point PositionFromAddress(Address addr)
        {
            var offset = (addr - mem.BaseAddress) - vscroll.Value;
            int x = (int)offset % this.LineLength;
            int y = (int)offset / this.LineLength;
            return new Point(x * pixelSize, y * pixelSize);
        }

        private Address AddressFromPosition(Point pt)
        {
            int x = pt.X / pixelSize;
            int y = pt.Y / pixelSize;
            if (x < 0 || x >= this.LineLength)
                return null;
            return mem.BaseAddress + (vscroll.Value + y * LineLength + x);
        }
    }
}
