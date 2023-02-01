#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Core.Diagnostics;
using Reko.Core.Expressions;
using Reko.Core.Memory;
using Reko.Core.Output;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Gui.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;
using System.Windows.Forms;

namespace Reko.UserInterfaces.WindowsForms.Controls
{
    /// <summary>
    /// Displays the contents of memory and allows selection of memory ranges. 
    /// </summary>
    /// <remarks>
    /// Memory that has been identified is colored.
    /// <para>
    /// A memory cell is displayed with a one-pixel border on all sides to
    /// help present it with selections.
    /// </para>
    /// </remarks>
    public class MemoryControl : Control
    {
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
        private static readonly TraceSwitch trace = new TraceSwitch(nameof(MemoryControl), "Memory control painting operations");
        private static readonly TraceSwitch tracePaint = new TraceSwitch(nameof(MemoryControlPainter), "Memory control painting operations");

        private readonly MemoryControlPainter mcp;
        private Address addrTopVisible;     // address of topmost visible row.
        private uint wordSize;
        private uint cbRow;
        private IProcessorArchitecture arch;
        private MemoryArea mem;
        private SegmentMap segmentMap;
        private ImageMap imageMap;
        private IDictionary<Address, Procedure> procedureMap;
        private Encoding encoding;
        private IServiceProvider services;
        private Address addrSelected;
        private Address addrAnchor;
        private Address addrMin;
        private ulong memSize;

        private int cRows;              // total number of rows.
        private int yTopRow;            // index of topmost visible row
        private int cyPage;             // number of rows / page.
        private Size cellSize;          // size of cell in pixels.
        private Point ptDown;            // point at which mouse was clicked.
        private VScrollBar vscroller;

        private bool isMouseClicked;
        private bool isDragging;

        public MemoryControl()
        {
            mcp = new MemoryControlPainter(this);
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            vscroller = new VScrollBar();
            vscroller.Dock = DockStyle.Right;
            Controls.Add(vscroller);
            vscroller.Scroll += vscroller_Scroll;
            wordSize = 1;
            cbRow = 16;
            encoding = Encoding.ASCII;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public IServiceProvider Services {
            get { return services; }
            set { this.services = value; }
        }

        /// <summary>
        /// Returns the selection as an address range. Note that the range is 
        /// a closed interval in the address space.
        /// </summary>
        public AddressRange GetAddressRange()
        {
            if (SelectedAddress == null || addrAnchor == null)
            {
                return AddressRange.Empty;
            }
            else
            {
                if (SelectedAddress <= addrAnchor)
                {
                    return new AddressRange(SelectedAddress, addrAnchor);
                }
                else
                {
                    return new AddressRange(addrAnchor, SelectedAddress);
                }
            }
        }

        public uint BytesPerRow
        {
            get { return cbRow; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("BytesPerRow must be positive.");
                UpdateScroll();
                cbRow = value;
            }
        }

        private void CacheCellSize()
        {
            using (Graphics g = this.CreateGraphics())
            {
                SizeF cellF = g.MeasureString("M", Font, Width, StringFormat.GenericTypographic);
                cellSize = new Size((int)(cellF.Width + 0.5F), (int)(cellF.Height + 0.5F));
            }
        }

        private Size CellSize
        {
            get { return cellSize; }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & ~Keys.Modifiers)
            {
            case Keys.Down:
            case Keys.Up:
            case Keys.Left:
            case Keys.Right:
                return true;
            default:
                return base.IsInputKey(keyData);
            }
        }

        public Point AddressToPoint(Address addr)
        {
            if (addr == null || TopAddress == null)
                return Point.Empty;
            int cbOffset = (int)(addr - TopAddress);
            int yRow = cbOffset / (int)cbRow;
            if (yTopRow <= yRow && yRow < yTopRow + cyPage)
            {
                return new Point(60, yRow * cellSize.Height);
            }
            else
                return Point.Empty;
        }

        private bool IsVisible(Address addr)
        {
            if (addr == null || TopAddress == null)
                return false;
            int cbOffset = (int)(addr - TopAddress);
            int yRow = cbOffset / (int)cbRow;
            return (yTopRow <= yRow && yRow < yTopRow + cyPage);
        }

        private void MoveSelection(int offset, Keys modifiers)
        {
            if (SelectedAddress == null)
                return;
            ulong linAddr = (ulong)((long)SelectedAddress.ToLinear() + offset);
            if (!mem.IsValidLinearAddress(linAddr))
                return;
            Address addr = segmentMap.MapLinearAddressToAddress(linAddr);
            if (!IsVisible(SelectedAddress))
            {
                Address newTopAddress = TopAddress + offset;
                if (mem.IsValidAddress(newTopAddress))
                {
                    TopAddress = newTopAddress;
                }
            }
            if ((modifiers & Keys.Shift) != Keys.Shift)
            {
                addrAnchor = addr;
            }
            SelectedAddress = addr;
            Invalidate();
            OnSelectionChanged();
        }


        protected override void OnKeyDown(KeyEventArgs e)
        {
            //            trace.Verbose(string.Format("K: {0}, D: {1}, M: {2}", e.KeyCode, e.KeyData, e.Modifiers));
            switch (e.KeyCode)
            {
            case Keys.Down:
                MoveSelection((int)cbRow, e.Modifiers);
                break;
            case Keys.Up:
                MoveSelection(-(int)cbRow, e.Modifiers);
                break;
            case Keys.Left:
                MoveSelection(-(int)wordSize, e.Modifiers);
                break;
            case Keys.Right:
                MoveSelection((int)wordSize, e.Modifiers);
                break;
            default:
                switch (e.KeyData)
                {
                case Keys.Shift | Keys.F10:
                    var addrRange = GetAddressRange();
                    if (this.ContextMenuStrip != null && addrRange.IsValid)
                    {
                        this.ContextMenuStrip.Show(this, AddressToPoint(addrRange.Begin));
                    }
                    break;
                default:
                    base.OnKeyDown(e);
                    return;
                }
                break;
            }
            e.Handled = true;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            CacheCellSize();
            base.OnHandleCreated(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (mem == null)
                return;
            this.isMouseClicked = true;
            ptDown = new Point(e.X, e.Y);
            Focus();
            CacheCellSize();

            AffectSelection(e);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!isMouseClicked)
                return;
            if (Math.Abs(ptDown.X - e.X) < 3 && Math.Abs(ptDown.Y - e.Y) < 3)
                return;
            isDragging = true;
            Capture = true;
            AffectSelection(e);
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isDragging)
            {
                Capture = false;
            }
            isMouseClicked = false;
            if (mem != null)
            {
                AffectSelection(e);
            }
            isDragging = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var newTopAddress = TopAddress + (int)(e.Delta > 0 ? -cbRow : cbRow);
            if (mem.IsValidAddress(newTopAddress))
            {
                TopAddress = newTopAddress;
            }
        }

        private void AffectSelection(MouseEventArgs e)
        {
            using (Graphics g = this.CreateGraphics())
            {
                var addrClicked = mcp.PaintWindow(g, cellSize, new Point(e.X, e.Y), false);
                trace.Verbose($"AffectSelection: {addrClicked}");
                if (ShouldChangeSelection(e, addrClicked))
                {
                    if (!isDragging || addrClicked != null)
                        this.addrSelected = addrClicked;
                    if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift &&
                        !isDragging)
                    {
                        addrAnchor = addrClicked;
                    }
                    trace.Inform("AffectSelection: {0}-{1}", addrAnchor, SelectedAddress);
                    Invalidate();
                    OnSelectionChanged();
                }
            }
        }

        private bool ShouldChangeSelection(MouseEventArgs e, Address addrClicked)
        {
            if (e.Button == MouseButtons.Left)
                return true;
            if (addrClicked == null)
                return true;

            return !IsAddressInSelection(addrClicked);
        }

        private bool IsAddressInSelection(Address addr)
        {
            if (addr == null || addrAnchor == null || SelectedAddress == null)
                return false;
            var linAddr = addr.ToLinear();
            var linAnch = addrAnchor.ToLinear();
            var linSel = SelectedAddress.ToLinear();
            if (linAnch <= linSel)
            {
                return linAnch <= linAddr && linAddr <= linSel;
            }
            else
            {
                return linSel <= linAddr && linAddr <= linAnch;
            }
        }

        protected override void OnPaint(PaintEventArgs pea)
        {
            if (mem is null || segmentMap is null || imageMap is null || arch is null)
            {
                pea.Graphics.FillRectangle(SystemBrushes.Window, ClientRectangle);
            }
            else
            {
                CacheCellSize();
                trace.Inform($"OnPaint: addrAnchor: {addrAnchor} addrSel: {SelectedAddress}");
                var sw = new Stopwatch();
                sw.Start();
                mcp.PaintWindow(pea.Graphics, cellSize, ptDown, true);
                sw.Stop();
                trace.Inform($"OnPaint: {sw.ElapsedMilliseconds}ms");
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            // We paint our own background.
        }

        protected virtual void OnSelectionChanged()
        {
            var ar = GetAddressRange();
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(addrAnchor, SelectedAddress));
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateScroll();
        }

        private void ChangeMemoryArea(ImageSegment seg)
        {
            mem = seg.MemoryArea;
            if (mem is not null)
            {
                this.addrMin = Address.Max(mem.BaseAddress, seg.Address);
                this.memSize = Math.Min((ulong) mem.Length, seg.Size);
                TopAddress = addrMin;
                UpdateScroll();
            }
            else
            {
                UpdateScroll();
                Invalidate();
            }
        }

        [Browsable(false)]
        public ImageMap ImageMap
        {
            get { return imageMap; }
            set
            {
                if (this.imageMap == value)
                    return;
                if (imageMap != null)
                    imageMap.MapChanged -= imageMap_MapChanged;
                imageMap = value;
                if (imageMap != null)
                    imageMap.MapChanged += imageMap_MapChanged;
                Invalidate();
            }
        }

        [Browsable(false)]
        public IDictionary<Address, Procedure> Procedures
        {
            get { return procedureMap; }
            set
            {
                this.procedureMap = value;
                Invalidate();
            }
        }

        [Browsable(false)]
        public SegmentMap SegmentMap
        {
            get { return segmentMap; }
            set
            {
                if (segmentMap != null)
                    segmentMap.MapChanged -= imageMap_MapChanged;
                segmentMap = value;
                if (segmentMap != null)
                    segmentMap.MapChanged += imageMap_MapChanged;
                Invalidate();
            }
        }

        [Browsable(false)]
        public IProcessorArchitecture Architecture
        {
            get { return arch; }
            set { arch = value; Invalidate(); }
        }

        [Browsable(false)]
        public Encoding Encoding
        {
            get { return encoding; }
            set {
                encoding = (Encoding)value.Clone();
                encoding.DecoderFallback = new DecoderReplacementFallback(".");
                Invalidate();
            }
        }

        private Address RoundToNearestRow(Address addr)
        {
            ulong rows = addr.ToLinear() / cbRow;
            return segmentMap.MapLinearAddressToAddress(rows * cbRow);
        }

        [Browsable(false)]
        public Address SelectedAddress
        {
            get { return this.addrSelected; }
            set
            {
                if (this.addrSelected == value)
                    return;
                trace.Verbose($"Setting SelectedAddress to {value}");
                this.addrSelected = value;
                addrAnchor = value;
                var seg = GetSegmentFromAddress(value);
                if (seg != null)
                {
                    ChangeMemoryArea(seg);
                }
                if (IsVisible(value) || IsVisible(this.addrSelected))
                    Invalidate();
                OnSelectionChanged();
            }
        }

        [Browsable(false)]
        public Address TopAddress
        {
            get { return addrTopVisible; }
            set
            {
                if (addrTopVisible == value)
                    return;
                trace.Inform($"TopAddress: new value {TopAddress}");
                addrTopVisible = value;
                if (value != null)
                    addrTopVisible -= (int)(value.ToLinear() % cbRow);
                UpdateScroll();
                Invalidate();
            }
        }

        private ImageSegment GetSegmentFromAddress(Address addr)
        {
            if (ImageMap is null)
                return null;
            if (!SegmentMap.TryFindSegment(addr, out ImageSegment seg))
                return null;
            return seg;
        }


        private void UpdateScroll()
        {
            if (this.TopAddress is null || mem is null || CellSize.Height <= 0)
            {
                vscroller.Enabled = false;
                return;
            }

            vscroller.Enabled = true;
            cRows = (int) ((memSize + cbRow - 1) / cbRow);
            int nChunks = (int)(cbRow / wordSize);      // number of chunks per line.

            vscroller.Minimum = 0;
            int h = Font.Height;
            cyPage = Math.Max((Height / CellSize.Height) - 1, 1);
            vscroller.LargeChange = cyPage;
            vscroller.Maximum = cRows;
            int newValue = (int)((TopAddress.ToLinear() - this.addrMin.ToLinear()) / cbRow);
            vscroller.Value = Math.Max(Math.Min(newValue, vscroller.Maximum), vscroller.Minimum);
        }

        public uint WordSize
        {
            get { return wordSize; }
            set
            {
                wordSize = value;
                UpdateScroll();
                Invalidate();
            }
        }

        private void imageMap_MapChanged(object sender, EventArgs e)
        {
            if (InvokeRequired)
                BeginInvoke(new Action(Invalidate));
            else
                Invalidate();
        }

        private void vscroller_Scroll(object sender, ScrollEventArgs e)
        {
            Address newTopAddress = segmentMap.MapLinearAddressToAddress(
                addrMin.ToLinear() + (uint)e.NewValue * cbRow);
            if (mem.IsValidAddress(newTopAddress))
            {
                TopAddress = newTopAddress;
            }
        }

        [SupportedOSPlatform("windows")]
        public class MemoryControlPainter
        {
            private MemoryControl ctrl;
            private Size cellSize;
            private BrushTheme codeTheme;
            private BrushTheme dataTheme;
            private BrushTheme defaultTheme;
            private BrushTheme selectTheme;

            public MemoryControlPainter(MemoryControl ctrl)
            {
                this.ctrl = ctrl;
            }

            /// <summary>
            /// Paints the control's window area. Strategy is to find the spans that make up
            /// the whole segment, and paint them one at a time.
            /// </summary>
            /// <param name="g"></param>
            public Address PaintWindow(Graphics g, Size cellSize, Point ptAddr, bool render)
            {
                this.cellSize = cellSize;
                GetColorPreferences();

                if (ctrl.arch == null || ctrl.imageMap == null)
                    return null;
                // Enumerate all segments visible on screen.

                ulong laEnd = ctrl.addrMin.ToLinear() + ctrl.memSize;
                if (ctrl.TopAddress.ToLinear() >= laEnd)
                    return null;
                var addrStart = Address.Max(ctrl.TopAddress, ctrl.mem.BaseAddress);
                EndianImageReader rdr = ctrl.arch.CreateImageReader(ctrl.mem, addrStart);
                Rectangle rcClient = ctrl.ClientRectangle;
                Rectangle rc = rcClient;
                Size cell = ctrl.CellSize;
                rc.Height = cell.Height;

                ulong laSegEnd = 0;
                while (rc.Top < ctrl.Height && (laEnd == 0 || rdr.Address.ToLinear() < laEnd))
                {
                    if (ctrl.SegmentMap.TryFindSegment(ctrl.TopAddress, out ImageSegment seg))
                    {
                        if (rdr.Address.ToLinear() >= laSegEnd)
                        {
                            laSegEnd = seg.Address.ToLinear() + seg.Size;
                            rdr = ctrl.arch.CreateImageReader(ctrl.mem, seg.Address + (rdr.Address - seg.Address));
                        }
                    }
                    var formatter = ctrl.mem.Formatter;
                    var linePainter = new LinePainter(this, g, rc, ptAddr, render);
                    var addr = linePainter.Paint(formatter, rdr, ctrl.Encoding);
                    if (!render && addr != null)
                        return addr;
                    rc.Offset(0, ctrl.CellSize.Height);
                }
                var dyLeft = rcClient.Bottom - rc.Top;
                if (dyLeft > 0)
                {
                    var theme = GetBrushTheme(null, false);
                    g.FillRectangle(theme.Background, rcClient.X, rc.Top, rcClient.Width, dyLeft);
                }
                return null;
            }

            private void GetColorPreferences()
            {
                var uiPrefs = this.ctrl.Services.RequireService<IUiPreferencesService>();
                var wind = uiPrefs.Styles[UiStyles.MemoryWindow];
                var code = uiPrefs.Styles[UiStyles.MemoryCode];
                var data = uiPrefs.Styles[UiStyles.MemoryData];
                var heur = uiPrefs.Styles[UiStyles.MemoryHeuristic];
                codeTheme = new BrushTheme { Background = (Brush)code.Background, Foreground = (Brush)code.Foreground ?? SystemBrushes.ControlText, StartMarker = Brushes.Red };
                dataTheme = new BrushTheme { Background = (Brush) data.Background, Foreground = (Brush) data.Foreground ?? SystemBrushes.ControlText, StartMarker = Brushes.Blue };
                defaultTheme = new BrushTheme { Background = (Brush) wind.Background ?? SystemBrushes.Window, Foreground = (Brush) wind.Foreground ?? SystemBrushes.ControlText };
                selectTheme = new BrushTheme { Background = SystemBrushes.Highlight, Foreground = SystemBrushes.HighlightText };
            }

            /// <summary>
            /// Paints a line of the memory control, starting with the address. 
            /// </summary>
            /// <remarks>

            private class LinePainter : IMemoryFormatterOutput
            {
                private readonly MemoryControlPainter mcp;
                private readonly Graphics g;
                private readonly Point ptAddr;
                private readonly bool render;
                private Rectangle rc;
                private Address addrHit;
                private int prepadding;

                public LinePainter(MemoryControlPainter mcp, Graphics g, Rectangle rc, Point ptAddr, bool render)
                {
                    this.mcp = mcp;
                    this.g = g;
                    this.rc = rc;
                    this.ptAddr = ptAddr;
                    this.render = render;
                    tracePaint.Verbose($"MemoryControl..ctor: {rc}");
                }

                public Address Paint(MemoryFormatter fmt, EndianImageReader rdr, Encoding enc)
                {
                    fmt.RenderLine(rdr, enc, this);
                    return addrHit;
                }

                public void BeginLine()
                {
                    rc.X = 0;
                    addrHit = null;
                    prepadding = 1;
                }

                public void EndLine(Constant[] bytes)
                {
                    tracePaint.Verbose($"MemoryControl.EndLine {rc}");
                    g.FillRectangle(mcp.defaultTheme.Background, rc);
                }

                public void RenderAddress(Address addr)
                {
                    if (addrHit != null)
                        return;
                    string s = addr.ToString();
                    int cx = (int) g.MeasureString(s + "X", mcp.ctrl.Font, rc.Width, StringFormat.GenericTypographic).Width;
                    var rcAddr = new Rectangle(rc.X, rc.Y, cx, rc.Height);
                    if (!render && rcAddr.Contains(mcp.ctrl.ptDown))
                    {
                        this.addrHit = addr;
                        return;
                    }
                    g.FillRectangle(mcp.defaultTheme.Background, rcAddr);
                    g.DrawString(s, mcp.ctrl.Font, mcp.defaultTheme.Foreground, rcAddr.X, rcAddr.Y, StringFormat.GenericTypographic);
                    cx -= mcp.cellSize.Width / 2;
                    rc = new Rectangle(cx, rc.Top, rc.Width - cx, rc.Height);
                    tracePaint.Verbose($"MemoryControl.RenderAddress: {rc}");
                }

                public void RenderFillerSpan(int nChunks, int cellsPerChunk)
                {
                    if (addrHit != null)
                        return;

                    int cx = mcp.cellSize.Width * (1 + cellsPerChunk) * nChunks;
                    if (cx <= 0)
                        return;
                    var rcFiller = new Rectangle(
                        rc.Left,
                        rc.Top,
                        cx,
                        rc.Height);
                    var theme = mcp.GetBrushTheme(null, false);
                    g.FillRectangle(theme.Background, rcFiller);
                    rc = new Rectangle(rc.X + rcFiller.Width, rc.Y, rc.Width - rcFiller.Width, rc.Height);
                    tracePaint.Verbose($"MemoryControl.RenderFillerSpan: {rc}");
                }

                public void RenderUnit(Address addUnit, string s)
                {
                    if (addrHit != null)
                        return;

                    int cx = mcp.cellSize.Width * (s.Length + 1);
                    var rcUnit = new RectangleF(
                        rc.Left,
                        rc.Top,
                        cx,
                        rc.Height);
                    if (!render && rcUnit.Contains(ptAddr))
                    {
                        this.addrHit = addUnit;
                        return;
                    }

                    ulong linearSelected = mcp.ctrl.SelectedAddress != null ? mcp.ctrl.SelectedAddress.ToLinear() : ~0UL;
                    ulong linearAnchor = mcp.ctrl.addrAnchor != null ? mcp.ctrl.addrAnchor.ToLinear() : ~0UL;
                    ulong linearBeginSelection = Math.Min(linearSelected, linearAnchor);
                    ulong linearEndSelection = Math.Max(linearSelected, linearAnchor);
                    mcp.ctrl.ImageMap.TryFindItem(addUnit, out var item);
                    bool isProcEntry = mcp.ctrl.Procedures?.ContainsKey(addUnit) ?? false;
                    bool isSelected = linearBeginSelection <= addUnit.ToLinear() && addUnit.ToLinear() <= linearEndSelection;
                    bool isCursor = addUnit.ToLinear() == linearSelected;

                    var theme = mcp.GetBrushTheme(item, isSelected);
                    g.FillRectangle(theme.Background, rcUnit);
                    if (!isSelected &&
                        theme.StartMarker != null)
                    {
                        if (item != null &&
                        addUnit.ToLinear() == item.Address.ToLinear())
                        {
                            var pts = new Point[]
                            {
                            rc.Location,
                            rc.Location,
                            rc.Location,
                            };
                            pts[1].Offset(4, 0);
                            pts[2].Offset(0, 4);
                            g.FillClosedCurve(theme.StartMarker, pts);
                        }
                        if (isProcEntry)
                        {
                            var pts = new Point[]
                            {
                                new Point(rc.Left,rc.Bottom),
                                new Point(rc.Left+4,rc.Bottom),
                                new Point(rc.Left, rc.Bottom-4)
                            };
                            g.FillClosedCurve(theme.StartMarker, pts);
                        }
                    }
                    g.DrawString(s, mcp.ctrl.Font, theme.Foreground, rc.Left + mcp.cellSize.Width / 2, rc.Top, StringFormat.GenericTypographic);
                    if (isCursor)
                    {
                        ControlPaint.DrawFocusRectangle(g, rc);
                    }
                    rc = new Rectangle(rc.X + cx, rc.Y, rc.Width - cx, rc.Height);
                    tracePaint.Verbose($"MemoryControl.RenderUnit: {rc}");
                }

                public void RenderUnitAsText(Address addr, string sUnit)
                {
                    if (!render) return;
                    var cx = sUnit.Length * mcp.cellSize.Width;
                    var rcUnit = new Rectangle(
                        rc.Left,
                        rc.Top,
                        cx,
                        rc.Height);
                    g.FillRectangle(mcp.defaultTheme.Background, rcUnit);
                    g.DrawString(sUnit, mcp.ctrl.Font, mcp.defaultTheme.Foreground, rcUnit.X, rcUnit.Top, StringFormat.GenericTypographic);
                    rc = new Rectangle(rc.X + cx, rc.Top, rc.Width - cx, rc.Height);
                    tracePaint.Verbose($"RenderUnitAsText: {rc}");
                }

                public void RenderTextFillerSpan(int padding)
                {
                    var cx = (padding + prepadding) * mcp.cellSize.Width;
                    var rcPadding = new Rectangle(rc.Left, rc.Top, cx, rc.Height);
                    g.FillRectangle(mcp.defaultTheme.Background, rcPadding);
                    prepadding = 0;
                    rc = new Rectangle(rc.X + cx, rc.Top, rc.Width - cx, rc.Height);
                    tracePaint.Verbose($"RenderTextFillerSpan: {rc}");
                }
            }

            private BrushTheme GetBrushTheme(ImageMapItem item, bool selected)
            {
                if (item is null)
                    return defaultTheme;
                if (selected)
                    return selectTheme;
                if (item is ImageMapBlock)
                    return codeTheme;
                if (item.DataType != null &&
                    (item.DataType is not UnknownType ut ||
                     ut.Size > 0))
                    return dataTheme;
                if (item is ImageMapVectorTable)
                    return dataTheme;
                return defaultTheme;
            }
        }

        private class BrushTheme
        {
            public Brush Foreground;
            public Brush Background;
            public Brush StartMarker;
        }
    }
}