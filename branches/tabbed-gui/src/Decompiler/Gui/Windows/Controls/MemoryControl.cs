#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
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

        private MemoryControlPainter mcp;
		private Address addrTopVisible;		// address of topmost visible row.
		private uint wordSize;
		private uint cbRow;
        private IProcessorArchitecture arch;
		private LoadedImage image;
        private ImageMap imageMap;
		private Address addrSelected;
        private Address addrAnchor;

		private int cRows;				// total number of rows.
		private int yTopRow;			// index of topmost visible row
		private int cyPage;				// number of rows / page.
		private Size cellSize;			// size of cell in pixels.
		private Point ptDown;			 // point at which mouse was clicked.
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
		}

        public IServiceProvider Services { get; set; }

        /// <summary>
        /// Returns the selection as an address range. Note that the range is 
        /// a closed interval in the address space.
        /// </summary>
        public AddressRange GetAddressRange()
        {
            Debug.Print("GetAddressRange: sel: {0}, anchor {1}", addrSelected, addrAnchor);

            if (addrSelected == null || addrAnchor == null)
            {
                return AddressRange.Empty;
            }
            else
            {
                if (addrSelected <= addrAnchor)
                {
                    return new AddressRange(addrSelected, addrAnchor);
                }
                else
                {
                    return new AddressRange(addrAnchor, addrSelected);
                }
            }
        }

        public void SetAddressRange(Address addrAnchor, Address addrSelected)
        {
            this.addrAnchor = addrAnchor;
            this.addrSelected = addrSelected;
            Invalidate();
            OnSelectionChanged();
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
				cellSize = new Size((int) (cellF.Width + 0.5F), (int) (cellF.Height + 0.5F));
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
				return base.IsInputKey (keyData);
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
            if (!image.IsValidLinearAddress(linAddr))
                return;
            Address addr = imageMap.MapLinearAddressToAddress(linAddr);
			if (!IsVisible(SelectedAddress))
			{
                Address newTopAddress = TopAddress + offset;
                if (image.IsValidAddress(newTopAddress))
                {
                    TopAddress = newTopAddress;
                }
			}
            if ((modifiers & Keys.Shift) != Keys.Shift)
            {
                addrAnchor = addr;
            }
            addrSelected = addr;
            Invalidate();
            OnSelectionChanged();
		}


		protected override void OnKeyDown(KeyEventArgs e)
		{
//            Debug.WriteLine(string.Format("K: {0}, D: {1}, M: {2}", e.KeyCode, e.KeyData, e.Modifiers));
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
                    if (ContextMenu != null && addrRange.IsValid)
                    {
                        ContextMenu.Show(this, AddressToPoint(addrRange.Begin));
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
            if (image == null)
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
            Debug.Print("We be draggin'");
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
            if (image != null)
            {
                AffectSelection(e);
            }
            isDragging = false;
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            var newTopAddress = TopAddress + (int)(e.Delta > 0 ? -cbRow : cbRow);
            if (image.IsValidAddress(newTopAddress))
            {
                TopAddress = newTopAddress;
            }
        }

        private void AffectSelection(MouseEventArgs e)
        {
            using (Graphics g = this.CreateGraphics())
            {
                var addrClicked = mcp.PaintWindow(g, cellSize, new Point(e.X, e.Y), false);
                if (ShouldChangeSelection(e, addrClicked))
                {
                    if (!isDragging || addrClicked != null)
                        addrSelected = addrClicked;
                    if ((Control.ModifierKeys & Keys.Shift) != Keys.Shift &&
                        !isDragging)
                    {
                        addrAnchor = addrSelected;
                    }
                    Debug.Print("Selection: {0}-{1}", addrAnchor, addrSelected);
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
            if (addr == null || addrAnchor == null || addrSelected == null)
                return false;
            var linAddr = addr.ToLinear();
            var linAnch = addrAnchor.ToLinear();
            var linSel = addrSelected.ToLinear();
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
			if (image == null || imageMap == null || arch == null)
			{
				pea.Graphics.FillRectangle(SystemBrushes.Window, ClientRectangle);
			}
			else
			{
                CacheCellSize();
                mcp.PaintWindow(pea.Graphics, cellSize, ptDown, true);
			}
		}

		protected virtual void OnSelectionChanged()
		{
            var eh = SelectionChanged;
            if (eh != null)
                eh(this, new SelectionChangedEventArgs(GetAddressRange()));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateScroll();
		}

		[Browsable(false)]
		public LoadedImage ProgramImage
		{
			get { return image; }
			set { 
				image = value;
				if (image != null)
				{
					TopAddress = image.BaseAddress;
					UpdateScroll();
				}
				else
				{
					UpdateScroll();
					Invalidate();
				}
			}
		}

        [Browsable(false)]
        public ImageMap ImageMap
        {
            get { return imageMap; }
            set { 
                if (imageMap != null) 
                    imageMap.MapChanged -= imageMap_MapChanged;
                imageMap = value;
                if (imageMap != null)
                    imageMap.MapChanged += imageMap_MapChanged;
                Invalidate();
            }
        }

        [Browsable(false)]
        public IProcessorArchitecture Architecture
        {
            get { return arch; }
            set { arch = value; Invalidate(); }
        }

		private Address RoundToNearestRow(Address addr)
		{
			ulong rows = addr.ToLinear() / cbRow;
			return imageMap.MapLinearAddressToAddress(rows * cbRow);
		}

		[Browsable(false)]
		public Address SelectedAddress
		{
			get { return addrSelected; }
			set 
			{
				addrSelected = value;
                addrAnchor = value;
				if (IsVisible(value) || IsVisible(addrSelected))
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
                addrTopVisible = value;
                if (value != null)
                    addrTopVisible -= (int)(value.ToLinear() % cbRow);
				UpdateScroll();
				Invalidate();
			}
		}

		public void ShowAddress(Address addr)
		{
            if (image == null || addr < image.BaseAddress)
                return;

            uint cbOffset = (uint)(addr - image.BaseAddress);
			if (cbOffset < 0)
				return;

			if (!IsVisible(addr))
			{
				TopAddress = RoundToNearestRow(addr);
                yTopRow = (int)(cbOffset / cbRow);
				vscroller.Value = yTopRow;
				Invalidate();
			}
		}

		private void UpdateScroll()
		{
			if (addrTopVisible == null || image == null || CellSize.Height <= 0)
			{
				vscroller.Enabled = false;
				return;
			}

			vscroller.Enabled = true;
			cRows = (image.Bytes.Length + (int)cbRow - 1) / (int) cbRow;
			int nChunks = (int)(cbRow / wordSize);		// number of chunks per line.

			vscroller.Minimum = 0;
			int h = Font.Height;
			cyPage = Math.Max((Height / CellSize.Height) - 1, 1);
			vscroller.LargeChange = cyPage;
            vscroller.Maximum = cRows;
            int newValue = (int)((addrTopVisible.ToLinear() - image.BaseAddress.ToLinear()) / cbRow);
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

        public void imageMap_MapChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

		private void vscroller_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.Type == ScrollEventType.ThumbTrack)
				return;
            Address newTopAddress = imageMap.MapLinearAddressToAddress(
                image.BaseAddress.ToLinear() + (uint)e.NewValue * cbRow);
            if (image.IsValidAddress(newTopAddress))
            {
                TopAddress = newTopAddress;
            }
		}

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
                //$TODO: these should be user-configurable.
                codeTheme = new BrushTheme { Background = Brushes.Pink, Foreground = SystemBrushes.WindowText, StartMarker = Brushes.Red };
                dataTheme = new BrushTheme { Background = Brushes.LightBlue, Foreground = SystemBrushes.WindowText, StartMarker = Brushes.Blue };
                defaultTheme = new BrushTheme { Background = SystemBrushes.Window, Foreground = SystemBrushes.ControlText };
                selectTheme = new BrushTheme { Background = SystemBrushes.Highlight, Foreground = SystemBrushes.HighlightText };

                if (ctrl.arch == null || ctrl.imageMap == null)
                    return null;
                // Enumerate all segments visible on screen.

                ulong laEnd = ctrl.ProgramImage.BaseAddress.ToLinear() + (uint) ctrl.image.Bytes.Length;
                if (ctrl.addrTopVisible.ToLinear() >= laEnd || !ctrl.image.IsValidAddress(ctrl.addrTopVisible))
                    return null;
                ImageReader rdr = ctrl.arch.CreateImageReader(ctrl.ProgramImage, ctrl.addrTopVisible);
                Rectangle rc = ctrl.ClientRectangle;
                Size cell = ctrl.CellSize;
                rc.Height = cell.Height;

                ulong laSegEnd = 0;
                while (rc.Top < ctrl.Height && rdr.Address.ToLinear() < laEnd)
                {
                    ImageMapSegment seg;
                    if (!ctrl.ImageMap.TryFindSegment(ctrl.addrTopVisible, out seg))
                        return null;
                    if (rdr.Address.ToLinear() >= laSegEnd)
                    {
                        laSegEnd = seg.Address.ToLinear() + seg.Size;
                        rdr = ctrl.arch.CreateImageReader(ctrl.ProgramImage, seg.Address + (rdr.Address - seg.Address));
                    }
                    Address addr = PaintLine(g, rc, rdr, ptAddr, render);
                    if (addr != null)
                        return addr;
                    rc.Offset(0, ctrl.CellSize.Height);
                }
                return null;
            }

            /// <summary>
            /// Paints a line of the memory control, starting with the address. 
            /// </summary>
            /// <remarks>
            /// The strategy is to find any items present at the current address, and try
            /// to paint as many adjacent items as possible.
            /// </remarks>
            /// <param name="g"></param>
            /// <param name="rc"></param>
            /// <param name="rdr"></param>
            private Address PaintLine(Graphics g, Rectangle rc, ImageReader rdr, Point ptAddr, bool render)
            {
                StringBuilder sbCode = new StringBuilder(" ");

                // Draw the address part.

                rc.X = 0;
                string s = string.Format("{0}", rdr.Address);
                int cx = (int) g.MeasureString(s + "X", ctrl.Font, rc.Width, StringFormat.GenericTypographic).Width;
                if (!render && new Rectangle(rc.X, rc.Y, cx, rc.Height).Contains(ctrl.ptDown))
                {
                    return rdr.Address;
                }
                else
                {
                    g.FillRectangle(SystemBrushes.Window, rc.X, rc.Y, cx, rc.Height);
                    g.DrawString(s, ctrl.Font, SystemBrushes.ControlText, rc.X, rc.Y, StringFormat.GenericTypographic);
                }
                cx -= cellSize.Width / 2;
                rc = new Rectangle(cx, rc.Top, rc.Width - cx, rc.Height);

                uint rowBytesLeft = ctrl.cbRow;
                ulong linearSelected = ctrl.addrSelected != null ? ctrl.addrSelected.ToLinear() : ~0UL;
                ulong linearAnchor = ctrl.addrAnchor != null ? ctrl.addrAnchor.ToLinear() : ~0UL;
                ulong linearBeginSelection = Math.Min(linearSelected, linearAnchor);
                ulong linearEndSelection = Math.Max(linearSelected, linearAnchor);

                do
                {
                    Address addr = rdr.Address;
                    ulong linear = addr.ToLinear();

                    ImageMapItem item;
                    if (!ctrl.ImageMap.TryFindItem(addr, out item))
                        break;
                    ulong cbIn = (linear - item.Address.ToLinear());			// # of bytes 'inside' the block we are.
                    uint cbToDraw = 16; // item.Size - cbIn;

                    // See if the chunk goes off the edge of the line. If so, clip it.
                    if (cbToDraw > rowBytesLeft)
                        cbToDraw = rowBytesLeft;

                    // Now paint the bytes in this span.

                    for (int i = 0; i < cbToDraw; ++i)
                    {
                        Address addrByte = rdr.Address;
                        ctrl.ImageMap.TryFindItem(addrByte, out item);
                        bool isSelected = linearBeginSelection <= addrByte.ToLinear() && addrByte.ToLinear() <= linearEndSelection;
                        bool isCursor = addrByte.ToLinear() == linearSelected;
                        if (rdr.IsValid)
                        {
                            byte b = rdr.ReadByte();
                            s = string.Format("{0:X2}", b);
                            char ch = (char) b;
                            sbCode.Append(Char.IsControl(ch) ? '.' : ch);
                        }
                        else
                        {
                            s = "??";
                            sbCode.Append(' ');
                        }

                        cx = cellSize.Width * 3;
                        Rectangle rcByte = new Rectangle(
                            rc.Left,
                            rc.Top,
                            cx,
                            rc.Height);

                        if (!render && rcByte.Contains(ptAddr))
                            return addrByte;

                        var theme = GetBrushTheme(item, isSelected);
                        g.FillRectangle(theme.Background, rc.Left, rc.Top, cx, rc.Height);
                        if (!isSelected && theme.StartMarker != null && addrByte.ToLinear() == item.Address.ToLinear())
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
                        g.DrawString(s, ctrl.Font, theme.Foreground, rc.Left + cellSize.Width / 2, rc.Top, StringFormat.GenericTypographic);
                        if (isCursor)
                        {
                            ControlPaint.DrawFocusRectangle(g, rc);
                        }
                        rc = new Rectangle(rc.X + cx, rc.Y, rc.Width - cx, rc.Height);
                    }
                    rowBytesLeft -= cbToDraw;
                } while (rowBytesLeft > 0);

                if (render)
                {
                    g.FillRectangle(SystemBrushes.Window, rc);
                    g.DrawString(sbCode.ToString(), ctrl.Font, SystemBrushes.WindowText, rc.X + cellSize.Width, rc.Top, StringFormat.GenericTypographic);
                }
                return null;
            }

            private BrushTheme GetBrushTheme(ImageMapItem item, bool selected)
            {
                if (selected)
                    return selectTheme;
                if (item is ImageMapBlock)
                    return codeTheme;
                if (item.DataType != null && !(item.DataType is UnknownType))
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