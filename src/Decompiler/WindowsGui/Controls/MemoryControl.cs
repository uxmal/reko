/* 
 * Copyright (C) 1999-2008 John Källén.
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

using Decompiler;
using Decompiler.Core;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Decompiler.WindowsGui.Controls
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
		public event EventHandler SelectionChanged;

		private Address addrTopVisible;		// address of topmost visible row.
		private int wordSize;
		private int cbRow;
		private ProgramImage image;
		private Address addrSelected;
		private Address addrAnchor;			// The start of the selection.
		private Address addrFocus;			// The end of the 

		private int cRows;				// total number of rows.
		private int yTopRow;			// index of topmost visible row
		private int cyPage;				// number of rows / page.
		private Size cellSize;			// size of cell in pixels.
		private Point ptDown;			 // point at which mouse was clicked.
		private VScrollBar vscroller;

		public MemoryControl()
		{
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
			vscroller = new VScrollBar();
			vscroller.Dock = DockStyle.Right;
			Controls.Add(vscroller);
			vscroller.Scroll += new ScrollEventHandler(vscroller_Scroll);
			wordSize = 1;
			cbRow = 16;
		}

		public int BytesPerRow
		{
			get { return cbRow; }
			set 
			{
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

		protected override CreateParams CreateParams
		{
			get
			{
				const int WS_EX_CLIENTEDGE = 0x00000200;
				CreateParams c = base.CreateParams;
				c.ExStyle |= WS_EX_CLIENTEDGE;
				return c;
			}
		}

		private Brush GetBackgroundBrush(ImageMapItem item, bool selected)
		{
			if (selected)
				return SystemBrushes.Highlight;
			if (item is ImageMapBlock)
				return Brushes.Pink;
			return SystemBrushes.Window;
		} 

		private Brush GetForegroundBrush(ImageMapItem item, bool selected)
		{
			if (selected)
				return SystemBrushes.HighlightText;
			return SystemBrushes.WindowText;
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

		private bool IsVisible(Address addr)
		{
			if (addr == null)
				return false;
			int cbOffset = addr - TopAddress;
			int yRow = cbOffset / cbRow;
			return (yTopRow <= yRow && yRow < yTopRow + cyPage);
		}

		private void MoveSelection(int offset)
		{
			int linAddr = SelectedAddress.Linear + offset;
			Address addr = image.Map.MapLinearAddressToAddress(linAddr);
			if (!image.IsValidAddress(addr))
				return;
			if (IsVisible(SelectedAddress))
			{
				TopAddress += offset;
			}
			SelectedAddress = addr;
			Invalidate();
		}


		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
			case Keys.Down:
				MoveSelection(cbRow);
				break;
			case Keys.Up:
				MoveSelection(-cbRow);
				break;
			case Keys.Left:
				MoveSelection(-wordSize);
				break;
			case Keys.Right:
				MoveSelection(wordSize);
				break;
			}
			base.OnKeyDown(e);

		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			Focus();
			CacheCellSize();
			if (image == null)
				return;

			ptDown = new Point(e.X, e.Y);
			using (Graphics g = this.CreateGraphics())
			{
				addrSelected = PaintWindow(g, false);
				Invalidate();
			}
			OnSelectionChanged();
		}

		protected override void OnPaint(PaintEventArgs pea)
		{
			if (image == null)
			{
				pea.Graphics.FillRectangle(SystemBrushes.Window, ClientRectangle);
			}
			else
			{
				CacheCellSize();
				PaintWindow(pea.Graphics, true);
			}
		}

		protected virtual void OnSelectionChanged()
		{
			if (SelectionChanged != null)
				SelectionChanged(this, EventArgs.Empty);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			UpdateScroll();
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
		private Address PaintLine(Graphics g, Rectangle rc, ImageReader rdr, bool render)
		{
			if (!render)
				render.ToString();	//$DEBUG
			StringBuilder sbCode = new StringBuilder(" ");

			// Draw the address part.

			rc.X = 0;
			string s = string.Format("{0}", rdr.Address);
			int cx = (int) g.MeasureString(s + "X", Font, rc.Width, StringFormat.GenericTypographic).Width;
			if (!render && new Rectangle(rc.X, rc.Y, cx, rc.Height).Contains(ptDown))
			{
				return rdr.Address;
			}
			else
			{
				g.FillRectangle(SystemBrushes.Window, rc.X, rc.Y, cx, rc.Height);
				g.DrawString(s, Font, SystemBrushes.ControlText, rc.X, rc.Y, StringFormat.GenericTypographic);
			}
			cx -= cellSize.Width / 2;
			rc = new Rectangle(cx, rc.Top, rc.Width - cx, rc.Height);

			int rowBytesLeft = cbRow;
			int linearSelected = addrSelected != null ? addrSelected.Linear : -1;
			do 
			{
				Address addr = rdr.Address;
				int linear = addr.Linear;

				ImageMapItem item = ProgramImage.Map.FindItem(addr);
				if (item == null)
					break;
				int cbIn = (linear - item.Address.Linear);			// # of bytes 'inside' the block we are.
				int cbToDraw = 16; // item.Size - cbIn;

				// See if the chunk goes off the edge of the line. If so, clip it.
				if (cbToDraw > rowBytesLeft)
					cbToDraw = rowBytesLeft;

				// Now paint the bytes in this span.

				for (int i = 0; i < cbToDraw; ++i)
				{
					Address addrByte = rdr.Address;
					item = ProgramImage.Map.FindItem(addrByte);
					bool isSelected = addrByte.Linear == linearSelected;
					Brush fg = GetForegroundBrush(item, isSelected);
					Brush bg = GetBackgroundBrush(item, isSelected);

					byte b = rdr.ReadByte();
					char ch = (char) b;
					sbCode.Append(Char.IsControl(ch) ? '.' : ch);

					s = string.Format("{0:X2}", b);
					cx = cellSize.Width * 3;
					Rectangle rcByte = new Rectangle(
						rc.Left,
						rc.Top,
						cx,
						rc.Height);
	
					if (!render && rcByte.Contains(ptDown))
						return addrByte;

					g.FillRectangle(bg, rc.Left, rc.Top, cx, rc.Height);
					g.DrawString(s, Font, fg, rc.Left + cellSize.Width / 2, rc.Top, StringFormat.GenericTypographic);
					rc = new Rectangle(rc.X + cx, rc.Y, rc.Width - cx, rc.Height);
				}
				rowBytesLeft -= cbToDraw;
			} while (rowBytesLeft > 0);

			if (render)
			{
				g.FillRectangle(SystemBrushes.Window, rc);
				g.DrawString(sbCode.ToString(), Font, SystemBrushes.WindowText, rc.X + cellSize.Width, rc.Top, StringFormat.GenericTypographic);
			}
			return null;
		}

		/// <summary>
		/// Paints the control's window area. Strategy is to find the spans that make up
		/// the whole segment, and paint them one at a time.
		/// </summary>
		/// <param name="g"></param>
		public Address PaintWindow(Graphics g, bool render)
		{
			// Enumerate all segments visible on screen.

			ImageReader rdr = image.CreateReader(addrTopVisible);
			Rectangle rc = ClientRectangle;
			Size cell = CellSize;
			rc.Height = cell.Height;

			int laEnd = image.BaseAddress.Linear + image.Bytes.Length;
			
			IDictionaryEnumerator segs = ProgramImage.Map.GetSegmentEnumerator(addrTopVisible);
			ImageMapSegment seg = null;
			int laSegEnd = 0;
			while (rc.Top < this.Height && rdr.Address.Linear < laEnd)
			{
				if (rdr.Address.Linear >= laSegEnd)
				{
					if (!segs.MoveNext())
						return null;
					seg = (ImageMapSegment) segs.Value;
					laSegEnd = seg.Address.Linear + seg.Size;
					rdr = image.CreateReader(seg.Address + (rdr.Address - seg.Address));
				}
				Address addr = PaintLine(g, rc, rdr, render);
				if (addr != null)
					return addr;
				rc.Offset(0, CellSize.Height);
			}
			return null;
		}


		[Browsable(false)]
		public ProgramImage ProgramImage
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

		private Address RoundToNearestRow(Address addr)
		{
			int rows = addr.Linear / cbRow;
			return image.Map.MapLinearAddressToAddress(rows * cbRow);
		}

		[Browsable(false)]
		public Address SelectedAddress
		{
			get { return addrSelected; }
			set 
			{
				addrSelected = value;
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
				UpdateScroll();
				Invalidate();
			}
		}

		public void ShowAddress(Address addr)
		{
			int cbOffset = addr - image.BaseAddress;
			if (cbOffset < 0)
				return;

			if (!IsVisible(addr))
			{
				TopAddress = RoundToNearestRow(addr);
				yTopRow = cbOffset / cbRow;
				vscroller.Value = yTopRow;
				Invalidate();
			}
		}

		private void UpdateScroll()
		{
			if (addrTopVisible == null || image == null)
			{
				vscroller.Enabled = false;
				return;
			}

			vscroller.Enabled = true;
			cRows = (image.Bytes.Length + cbRow - 1) / cbRow;
			int nChunks = cbRow / wordSize;		// number of chunks per line.

			vscroller.Minimum = 0;
			int h = Font.Height;
			cyPage = Math.Max((ClientRectangle.Height / Font.Height) - 1, 1);
			vscroller.LargeChange = cyPage;
			vscroller.Maximum = cRows - cyPage;
			vscroller.Value = (addrTopVisible.Linear - image.BaseAddress.Linear) / cbRow;
		}

		public int WordSize
		{
			get { return wordSize; }
			set 
			{	
				wordSize = value;
				UpdateScroll();
				Invalidate();
			}
		}

		private void vscroller_Scroll(object sender, ScrollEventArgs e)
		{
			if (e.Type == ScrollEventType.ThumbTrack)
				return;
			TopAddress = image.Map.MapLinearAddressToAddress(image.BaseAddress.Linear + e.NewValue * cbRow);
		}
	}
}