/* 
 * Copyright (C) 1999-2007 John Källén.
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
	/// Displays memory and allows selection of memory ranges. 
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
		private Address addrTopVisible;		// address of topmost visible row.
		private int wordSize;
		private int cbRow;
		private ProgramImage image;
		private ImageMap imageMap;
		private Address addrSelected;

		private int cRows;				// total number of rows.
		private int yTopRow;			// index of topmost visible row
		private int cxPage;				// number of characters / page.
		private int cyPage;				// number of rows / page.
		private Size cellSize;			// size of cell in pixels.
		private Point ptDown;			 // point at which mouse was clicked.
		private HScrollBar hscroller;
		private VScrollBar vscroller;
		private Point hitTestPoint = new Point(0, 0);

		private const int AddressDisplaySize = 10;
		private const int BorderWidth = 6;

		public MemoryControl()
		{
			hscroller = new HScrollBar();
			vscroller = new VScrollBar();
			Controls.AddRange(new Control[] { hscroller, vscroller });
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
				SizeF cellF = g.MeasureString("X", Font);
				cellSize = new Size(2 + (int) cellF.Width, 2 * BorderWidth + (int) cellF.Height);
			}
		}

		private Size CellSize
		{
			get { return cellSize; }
		}

		private bool IsVisible(Address addr)
		{
			if (addr == null)
				return false;
			int cbOffset = addr - StartAddress;
			int yRow = cbOffset / cbRow;
			return (yTopRow <= yRow && yRow < yTopRow + cyPage);
		}

		private Brush GetBackgroundBrush(ImageMapItem item)
		{
			if (item is ImageMapBlock)
				return Brushes.Pink;
			return SystemBrushes.Window;
		} 

		private Brush GetForegroundBrush(ImageMapItem item)
		{
			return SystemBrushes.WindowText;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			Focus();
			CacheCellSize();
			ptDown = new Point(e.X, e.Y);

			int row = e.X / CellSize.Width;
			int col = e.Y / CellSize.Height;

			if (imageMap != null)
			{
				this.addrSelected = ImageMap.MapLinearAddressToAddress(addrTopVisible.Linear + row * BytesPerRow + col);
			}
		}

		protected override void OnPaint(PaintEventArgs pea)
		{
			if (image == null || imageMap == null)
				return;
			CacheCellSize();
			PaintWindow(pea.Graphics);
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
			StringBuilder sbCode = new StringBuilder(" ");

			// Draw the address part.

			rc.X = 0;
			string s = string.Format("{0} ", rdr.Address);
			rc.Width = (int) g.MeasureString(s, Font).Width;
			if (render)
			{
				g.FillRectangle(SystemBrushes.Window, 0, rc.Top, rc.Height, rc.Width);
				g.DrawString(s, Font, SystemBrushes.ControlText, new PointF(0, rc.Y + BorderWidth));
			}
			rc.X = rc.Width;

			ImageMapItem item = null;
			int rowBytesLeft = cbRow;
			do 
			{
				Address addr = rdr.Address;
				int linear = addr.Linear;

				item = ImageMap.FindItem(rdr.Address);
				if (item == null)
					break;
				int cbIn = (linear - item.Address.Linear);			// # of bytes 'inside' the block we are.
				int cbToDraw = item.Size - cbIn;
				bool fTopRow = (cbIn < cbRow);
				bool fBottomRow = (cbToDraw <= rowBytesLeft);

				// See if the chunk goes off the edge of the line. If so, clip it.
				if (cbToDraw > rowBytesLeft)
					cbToDraw = rowBytesLeft;

				// Now paint the bytes in this span.

				Brush fg = GetForegroundBrush(item);
				Brush bg = GetBackgroundBrush(item);
				for (int i = 0; i < cbToDraw; ++i)
				{
					Address addrByte = rdr.Address;
					byte b = rdr.ReadByte();
					char ch = (char) b;
					sbCode.Append(Char.IsControl(ch) ? '.' : ch);

					s = string.Format("{0:X2} ", b);
					int cx = (int) g.MeasureString(s, Font).Width + BorderWidth;
					Rectangle rcByte = new Rectangle(
						rc.Left,
						rc.Top,
						cx,
						rc.Height);
	
					if (render)
					{
						g.FillRectangle(
							bg, 
							rc.Left,
							rc.Top,
							cx,
							rc.Height);
						if (fTopRow)
						{
							g.FillRectangle(SystemBrushes.Window, rc.Left, rc.Top, cx, BorderWidth);
							if (i == 0)
								g.FillRectangle(SystemBrushes.Window, rc.Left, rc.Top, BorderWidth, rc.Height);
						}
						if (fBottomRow)
						{
							g.FillRectangle(SystemBrushes.Window, rc.Left, rc.Bottom - BorderWidth, cx, BorderWidth);
							if (i == cbToDraw - 1)
								g.FillRectangle(SystemBrushes.Window, rc.Left + cx - BorderWidth, rc.Top, BorderWidth, rc.Height);
						}
						g.DrawString(s, Font, SystemBrushes.WindowText, new PointF(rc.Left+BorderWidth, rc.Top+BorderWidth));
					}
					else
					{
						if (rcByte.Contains(hitTestPoint))
						{
							return addrByte;
						}
					}
					rc.X += cx;
				}
				rowBytesLeft -= cbToDraw;
			} while (rowBytesLeft > 0);

			g.DrawString(sbCode.ToString(), Font, SystemBrushes.WindowText, new PointF(rc.X + 8, rc.Top + BorderWidth));
			return null;
		}


		/// <summary>
		/// Paints the control's window area. Strategy is to find the spans that make up
		/// the whole segment, and paint them one at a time.
		/// </summary>
		/// <param name="g"></param>
		private void PaintWindow(Graphics g)
		{
			// Enumerate all segments visible on screen.

			ImageReader rdr = image.CreateReader(addrTopVisible);
			Rectangle rc = ClientRectangle;
			Size cell = CellSize;
			rc.Height = cell.Height;

			//$REVIEW: Ignore scrollbars for now.rc.X -= hscroller.Position;

			int laEnd = image.BaseAddress.Linear + image.Bytes.Length;
			
			IDictionaryEnumerator segs = ImageMap.GetSegmentEnumerator(addrTopVisible);
			ImageMapSegment seg = null;
			int laSegEnd = 0;
			while (rc.Top < this.Height && rdr.Address.Linear < laEnd)
			{
				if (rdr.Address.Linear >= laSegEnd)
				{
					if (!segs.MoveNext())
						return;
					seg = (ImageMapSegment) segs.Value;
					laSegEnd = seg.Address.Linear + seg.Size;
					rdr = image.CreateReader(seg.Address + (rdr.Address - seg.Address));
				}
				PaintLine(g, rc, rdr, true);
				rc.Y += CellSize.Height;
			}
		}


		[Browsable(false)]
		public ProgramImage Image
		{
			get { return image; }
			set { image = value; }
		}

		[Browsable(false)]
		public ImageMap ImageMap
		{
			get { return imageMap; }
			set { imageMap = value; }
		}
 
		private Address RoundToNearestRow(Address addr)
		{
			int rows = addr.Linear / cbRow;
			return ImageMap.MapLinearAddressToAddress(rows * cbRow);
		}

		[Browsable(false)]
		public Address SelectedAddress
		{
			get { return addrSelected; }
			set 
			{
				if (IsVisible(value) || IsVisible(addrSelected))
					Invalidate();
				addrSelected = value;
			}
		}
			
		public void ShowAddress(Address addr)
		{
			int cbOffset = addr - image.BaseAddress;
			if (cbOffset < 0)
				return;

			if (!IsVisible(addr))
			{
				StartAddress = addr;
				yTopRow = cbOffset / cbRow;
				vscroller.Value = yTopRow;
				Invalidate();
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Address StartAddress
		{
			get { return addrTopVisible; }
			set { addrTopVisible = RoundToNearestRow(value); UpdateScroll(); }
		}

		private void UpdateScroll()
		{
			if (addrTopVisible == null || image == null || imageMap == null)
			{
				hscroller.Visible = false;
				vscroller.Visible = false;
				return;
			}

			hscroller.Visible = true;
			vscroller.Visible = true;
			
			using (Graphics g = Graphics.FromHwnd(Handle))
			{
				cRows = (image.Bytes.Length + cbRow - 1) / cbRow;
				int nChunks = cbRow / wordSize;		// number of chunks per line.
				int cCols = (cbRow * 2) + (2 * nChunks) + AddressDisplaySize;

				hscroller.Minimum = 0;
				vscroller.Minimum = 0;
				SizeF sz = g.MeasureString("M", Font);
				cxPage = (int) (ClientRectangle.Width / sz.Width);
				cyPage = (ClientRectangle.Height / Font.Height) - 1;
				if (cyPage < 1)
					cyPage = 1;
				hscroller.LargeChange = cxPage; 
				vscroller.LargeChange = cyPage;
				hscroller.Maximum = cCols;
				vscroller.Maximum = cRows - cyPage;
			}
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
	}
}
