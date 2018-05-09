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
    }

    public interface Visualizer
    {
        /// <summary>
        /// Preferred number of bytes per line for this visualization.
        /// </summary>
        int DefaultLineLength { get; }

        /// <summary>
        /// Preferred number of pixels per byte
        /// </summary>
        int DefaultZoom { get; }

        /// <summary>
        /// If set, this visualization requires always having a certain width.
        /// </summary>
        bool IsLineLengthFixed { get; }

        /// <summary>
        /// If true, this visualization wants to track the current selection
        /// of bytes.
        /// </summary>
        bool TrackSelection { get; }

        bool ShowScrollbar { get; }

        /// <summary>
        /// Render the <paramref name="length"/> bytes of the memory area <paramref name="mem"/> starting
        /// at address <paramref name="addrStart"/>. If the visualizer supports it, it can
        /// render the current mouse position given by <paramref name="mouse"/>.
        /// </summary>
        /// <param name="mem">A memory area containing the bytes to render</param>
        /// <param name="addrStart">The starting area from which to start rendering</param>
        /// <param name="length">The number of bytes to render</param>
        /// <param name="mouse">The offset from addrStart where the mouse cursor is located</param>
        /// <returns>An array of bytes in ARGB format. Missing bytes are rendered with the 
        /// ARGB value 0; this is in contrast with the ARGB value for 'black' which is
        /// 0xFF000000.</returns>
        int[] RenderBuffer(MemoryArea mem, Address addrStart, int length, int? mouse);

        VisualAnnotation[] RenderAnnotations(Address addrStart, int length, int? mouse);
    }

    public class VisualAnnotation
    {
        public Address Address;
        public string Text;
        public int LineColor;
        public int TextColor;
    }

    public class DefaultVisualizer : Visualizer
    {
        public int DefaultLineLength => 64;

        public int DefaultZoom => 2;

        public bool IsLineLengthFixed => false;
        public bool TrackSelection => true;
        public bool ShowScrollbar => true;

        public int[] RenderBuffer(MemoryArea mem, Address addrStart, int length, int? mouse)
        {
            var iStart = addrStart - mem.BaseAddress;
            var iEnd = Math.Min(iStart + length, mem.Bytes.Length);
            var colors = new int[iEnd - iStart];
            for (int i = 0; i < colors.Length; ++i)
            {
                if (i + iStart < 0)
                {
                    colors[i] = 0;
                }
                else
                {
                    // Render pixel in a heat map color
                    // code taken from
                    // http://stackoverflow.com/questions/20792445/calculate-rgb-value-for-a-range-of-values-to-create-heat-map
                    var ratio = 2 * mem.Bytes[i + iStart] / 255;
                    var b = Convert.ToInt32(Math.Max(0, 255 * (1 - ratio)));
                    var r = Convert.ToInt32(Math.Max(0, 255 * (ratio - 1)));
                    var g = 255 - b - r;
                    colors[i] =  ~0x00FFFFFF | (r << 16) | (g << 8) | b;
                }
            }
            return colors;
        }

        public VisualAnnotation[] RenderAnnotations(Address addrStart, int length, int? mouse)
        {
            return new VisualAnnotation[]
            {
                new VisualAnnotation { Address = Address.Ptr32(0x00123450), Text = "Line 1" },
                new VisualAnnotation { Address = Address.Ptr32(0x00123650), Text = "Line 2" },
                new VisualAnnotation { Address = Address.Ptr32(0x0012365F), Text = "Line 3" },
                new VisualAnnotation { Address = Address.Ptr32(0x00123663), Text = "Line 4" },
                new VisualAnnotation { Address = Address.Ptr32(0x00124450), Text = "Line 5" },
            }.Where(a => addrStart <= a.Address && a.Address < addrStart  + length)
            .ToArray();
        }
    }
}
