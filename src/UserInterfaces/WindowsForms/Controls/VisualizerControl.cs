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
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    public class VisualizerControl : Control
    {
        private Visualizer visualizer;
        private MemoryArea mem;
        private int pixelSize;

        public VisualizerControl()
        {
            this.pixelSize = 2;
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

        protected override void OnPaint(PaintEventArgs e)
        {
            using (var bmp = new Bitmap(Width, Height, e.Graphics))
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(Brushes.Black, ClientRectangle);
                var bytesImage = RenderVisualization();
                g.DrawImage(bytesImage, 0, 0);

                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
                e.Graphics.DrawImage(
                    bmp,
                    new Rectangle(0, 0, bmp.Width * pixelSize, bmp.Height * pixelSize));
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
            int offset = 0; //$TODO: scrollbar
            var addrStart = mem.BaseAddress + offset;
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
                this.pixelSize = visualizer.DefaultZoom;
            }
            this.Invalidate();
        }

        protected virtual void OnMemoryAreaChanged()
        {
            this.Invalidate();
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

        bool ShowAddressRange { get; }

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
    }

    public class DefaultVisualizer : Visualizer
    {
        public int DefaultLineLength => 64;

        public int DefaultZoom => 2;

        public bool IsLineLengthFixed => false;
        public bool TrackSelection => true;
        public bool ShowAddressRange => true;
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
    }
}
