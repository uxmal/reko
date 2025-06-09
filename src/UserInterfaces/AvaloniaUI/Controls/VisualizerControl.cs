#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Reko.Core;
using Reko.Core.Diagnostics;
using Reko.Core.Memory;
using Reko.Core.Services;
using Reko.Gui.Visualizers;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Reko.UserInterfaces.AvaloniaUI.Controls
{
    public class VisualizerControl : Control, ILogicalScrollable
    {
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(VisualizerControl), "Trace events in VisualizerControl", "Warning");

        public static readonly DirectProperty<VisualizerControl, Visualizer?> VisualizerProperty =
            AvaloniaProperty.RegisterDirect<VisualizerControl, Visualizer?>(
                nameof(Visualizer),
                v => v.Visualizer,
                (v, vv) => v.Visualizer = vv);

        public static readonly DirectProperty<VisualizerControl, MemoryArea?> MemoryAreaProperty =
            AvaloniaProperty.RegisterDirect<VisualizerControl, MemoryArea?>(
                nameof(MemoryArea),
                v => v.MemoryArea,
                (v, vv) => v.MemoryArea = vv);


        /// <summary>
        /// The visualizer to use to render the contents of the control.
        /// </summary>
        public Visualizer? Visualizer
        {
            get => visualizer;
            set
            {
                if (this.visualizer == value)
                    return;
                this.visualizer = value;
                OnVisualizerChanged();
            }
        }
        private Visualizer? visualizer;

        public MemoryArea? MemoryArea
        {
            get => this.mem;
            set
            {
                if (this.mem == value)
                    return;
                this.mem = value;
                OnMemoryAreaChanged();
            }
        }
        private MemoryArea? mem;

        public Program? Program
        {
            get => this.program;
            set
            {
                if (this.program == value)
                    return;
                this.program = value;
                OnProgramChanged();
            }
        }
        private Program? program;


        private ByteMemoryArea? bmem;
        //private VScrollBar vscroll;
        private int pixelSize;
        private ISelectionService? selSvc;
        private Address addrTopVisible;

        private EventHandler? _scrollInvalidated;
        private bool canHorizontallyScroll;
        private bool canVerticallyScroll;
        private Size smallScrollIncrement;
        private Size largeScrollIncrement;
        private Size logicalExtent;
        private Size logicalViewport;
        private Vector scrollOffset;
        private bool updatingScroll;
        private bool enableVscroll;

        public VisualizerControl()
        {
            this.pixelSize = 2;
            this.LineLength = 64;
            this.enableVscroll = true;

            //this.vscroll = new VScrollBar();
            //this.vscroll.Dock = DockStyle.Right;
            //this.vscroll.ValueChanged += Vscroll_ValueChanged;
            //this.vscroll.Visible = true;
        }

        event EventHandler? ILogicalScrollable.ScrollInvalidated
        {
            add => _scrollInvalidated += value;
            remove => _scrollInvalidated -= value;
        }

        /// <summary>
        /// Number of bytes per line.
        /// </summary>
        public int LineLength { get; set; }

        /// <summary>
        /// Number of lines that fit on the current client area.
        /// </summary>
        public int LinesOnScreen => (int)Math.Ceiling(this.Height + (this.pixelSize - 1)) / pixelSize;

        /// <summary>
        /// The visualizer to use to render the contents of the control.
        /// </summary>

        bool ILogicalScrollable.CanHorizontallyScroll
        {
            get => canHorizontallyScroll;
            set
            {
                if (this.canHorizontallyScroll == value)
                    return;
                this.canHorizontallyScroll = value;
                InvalidateMeasure();
            }
        }


        bool ILogicalScrollable.CanVerticallyScroll {
            get => canVerticallyScroll;
            set
            {
                if (this.canVerticallyScroll == value)
                    return;
                this.canVerticallyScroll = value;
                InvalidateMeasure();
            }
        }

        bool ILogicalScrollable.IsLogicalScrollEnabled => enableVscroll;

        Size ILogicalScrollable.ScrollSize => smallScrollIncrement;

        Size ILogicalScrollable.PageScrollSize => largeScrollIncrement;

        Size IScrollable.Extent => logicalExtent;

        Vector IScrollable.Offset {
            get => scrollOffset;
            set {
                if (scrollOffset == value)
                    return;
                if (updatingScroll)
                    return;
                if (bmem is null)
                    return;
                updatingScroll = true;
                scrollOffset = ClampOffset(value);
                addrTopVisible = bmem.BaseAddress + (ulong)LineLength * (ulong) scrollOffset.Y;
                UpdateScrollbar();
                updatingScroll = false;

            }
        }

        Size IScrollable.Viewport => throw new NotImplementedException();

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            if (this.bmem is null)
                return;
            var pos = e.GetCurrentPoint(this).Position;
            var addr = AddressFromPosition(pos);
            if (addr is not null)
            {
                if (selSvc is not null)
                {
                    var ar = new AddressRange(addr.Value, addr.Value);
                    //$BUG: should be done by the interactor/viewmodel
                    selSvc.SetSelectedComponents(new[] { ar });
                }
            }
            base.OnPointerReleased(e);
        }

        protected override void OnSizeChanged(SizeChangedEventArgs e)
        {
            UpdateScrollbar();
            InvalidateVisual();
            base.OnSizeChanged(e);
        }


        public override void Render(DrawingContext g)
        {
            if (Width <= 0 || Height <= 0)
                return;

            //$TODO: styleable backround brush
            g.FillRectangle(Brushes.Black, base.Bounds);

            using var bytesImage = RenderVisualization();
            if (bytesImage is null)
                return;
            g.DrawImage(
                bytesImage,
                new Rect(
                    0, 0,
                    bytesImage.Size.Width * pixelSize,
                    bytesImage.Size.Height * pixelSize));

            RenderAnnotations(g);
        }
         
        private Bitmap? RenderVisualization()
        {
            if (Visualizer is null || bmem is null || program is null)
                return null;
                
            var bmp = new WriteableBitmap(
                new PixelSize(LineLength, this.LinesOnScreen),
                new Vector(96, 96),
                PixelFormat.Rgba8888);
            using var fb = bmp.Lock();

            var bgPattern = new byte[][] {
                new byte[] {0xFF, 0x7F, 0x00, 0x00},
                new byte[] {0xFF, 0x30, 0x00, 0x00} ,
             };
            var addrStart = bmem.BaseAddress + (ulong)scrollOffset.Y * (ulong)LineLength;
            trace.Verbose("Visualizer: {0} {1:X}", addrStart, scrollOffset.Y);
            var bytesOnScreen = LinesOnScreen * LineLength;
            var colors = Visualizer.RenderBuffer(program, bmem, addrStart, bytesOnScreen, null);
            int x = 0;
            int y = 0;
            var colorRow = new byte[LineLength * 4];
            int iDst = 0;
            int iBeginRow = 0;
            foreach (var byteColor in colors)
            {
                Color color;
                if (byteColor == 0)
                {
                    Array.Copy(bgPattern[((x & 2) ^ (y & 2)) >> 1], 0, colorRow, iDst, 4);
                }
                else
                {
                    color = Color.FromUInt32((uint)byteColor);
                }
                iDst += 4;
                x = (x + 1) % LineLength;
                if (x == 0)
                {
                    var gg = fb.Address + 3;
                    Marshal.Copy(colorRow, 0, fb.Address + iBeginRow, colorRow.Length);
                    iBeginRow += fb.RowBytes;
                    y = y + 1;
                }
            }
            //$TODO: continue port
            /*
            int cVoid = bytesOnScreen - colors.Length;
            if (cVoid > 0)
            {
                for (int i = 0; i < cVoid; ++i)
                {
                    var color = bgPattern[((x & 2) ^ (y & 2)) >> 1];
                    bmp.SetPixel(x, y, color);
                }
            }*/
            return bmp;
        }

        private void RenderAnnotations(DrawingContext g)
        {
            if (visualizer is null || bmem is null)
                return;
            //$TODO: finish porting this.
            /*
            var addrStart = bmem.BaseAddress + (long)scrollOffset.Y;
            var bytesOnScreen = LinesOnScreen * LineLength;
            var annotations = visualizer.RenderAnnotations(program, addrStart, bytesOnScreen, null);
            if (annotations.Length == 0)
                return;

            const int spacing = 20;
            int xStart = LineLength * pixelSize + spacing;
            int yStart = (int) g.MeasureString("M", Font).Height;
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
            */
        }

        protected virtual void OnVisualizerChanged()
        {
            if (visualizer is null)
            {
                LineLength = 16;
            }
            else
            {
                this.LineLength = visualizer.DefaultLineLength;
                this.smallScrollIncrement = new Size(Width, 1);
                this.pixelSize = visualizer.DefaultZoom;
            }
            this.InvalidateVisual();
        }

        protected virtual void OnProgramChanged()
        {
            UpdateScrollbar();
            this.InvalidateVisual();
        }

        protected virtual void OnMemoryAreaChanged()
        {
            //$TODO: what do do with non-byte-addressable memory areas?
            this.bmem = this.mem as ByteMemoryArea;
            UpdateScrollbar();
            this.InvalidateVisual();
        }

        private void Vscroll_ValueChanged(object sender, EventArgs e)
        {
            InvalidateVisual();
        }


        private void UpdateScrollbar()
        {
            var bytesOnScreen = LineLength * LinesOnScreen;
            if (program is null ||
                bmem is null ||
                bytesOnScreen >= bmem.Bytes.Length ||
                visualizer is not null && !visualizer.ShowScrollbar)
            {
                this.enableVscroll = false;
            }
            else
            {
                this.enableVscroll = true;
                this.logicalExtent = new Size(
                    this.Width,
                    (bmem.Bytes.Length + LineLength) /* - bytesOnScreen */ / LineLength);
                this.logicalViewport = new Size(
                    this.Width,
                    Math.Max(LinesOnScreen - 1, 1));
                this.largeScrollIncrement = new Size(LinesOnScreen, 1);
                this.smallScrollIncrement = new Size(Width, 1);
                trace.Verbose($"VisCtrl: mem bytes {bmem.Bytes.Length}, small = 1, large = {LinesOnScreen}, extent={logicalExtent}");
            }
        }

        /// <summary>
        /// Given an address, compute the position of the upper
        /// left corner of the corresponding pixel.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        private Point PositionFromAddress(ByteMemoryArea bmem, Address addr)
        {
            var offset = (addr - bmem.BaseAddress) - (scrollOffset.Y * LineLength);
            int x = (int) offset % this.LineLength;
            int y = (int) offset / this.LineLength;
            return new Point(x * pixelSize, y * pixelSize);
        }

        /// <summary>
        /// Given a client coordinate position, determine
        /// what address it corresponds to, or return null
        /// if there is no corresponding address.
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        private Address? AddressFromPosition(Point pt)
        {
            Debug.Assert(bmem is not null);
            int x = (int)Math.Round(pt.X / pixelSize);
            int y = (int)Math.Round(pt.Y / pixelSize);
            if (x < 0 || x >= this.LineLength)
                return null;
            return bmem.BaseAddress + (((long)scrollOffset.Y + y) * LineLength + x);
        }

        private Vector ClampOffset(Vector value)
        {
            var maxX = Math.Max(logicalExtent.Width - logicalViewport.Width, 0);
            var maxY = Math.Max(logicalExtent.Height - logicalViewport.Height, 0);
            return new Vector(Clamp(value.X, 0, maxX), Clamp(value.Y, 0, maxY));
            static double Clamp(double val, double min, double max) => val < min ? min : val > max ? max : val;

        }

        bool ILogicalScrollable.BringIntoView(Control target, Rect targetRect)
        {
            return false;
        }

        Control? ILogicalScrollable.GetControlInDirection(NavigationDirection direction, Control? from)
        {
            return null;
        }

        void ILogicalScrollable.RaiseScrollInvalidated(EventArgs e)
        {
            _scrollInvalidated?.Invoke(this, e);
        }
    }
}
