/* 
 * Copyright (C) 1999-2009 John Källén.
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Decompiler.Gui.Windows.Controls
{
	/// <summary>
	/// Displays an image map graphically.
	/// </summary>
	public class ImageMapControl : Control
	{
		private ImageMap map;
		private ImageMapSegment segSelected;

		public event EventHandler SelectedItemChanged;

		public ImageMapControl()
		{
		}

		public ImageMap ImageMap
		{
			get { return map; }
			set 
			{
				map = value;
				Invalidate();
			}
		}

		protected override void OnPaint(PaintEventArgs pea)
		{
			base.OnPaint(pea);
			if (map == null)
				return;

			ImageMapSegment [] segs = ExtractSegments();
			int imageSize = ImageSize(segs);
			
			Matrix m = new Matrix();
			m.Scale(1.0F, (float)Height / (float) imageSize);
			pea.Graphics.Transform = m;

			Rectangle rc = ClientRectangle;
			int start = segs[0].Address.Linear;
			foreach (ImageMapSegment seg in segs)
			{
				rc.Y = seg.Address.Linear - start;
				rc.Height = seg.Size;

				PaintSegment(seg, pea.Graphics, rc);
			}

		}

		private void PaintSegment(ImageMapSegment seg, Graphics g, Rectangle rc)
		{
			g.FillRectangle(Brushes.White, rc);
			g.DrawRectangle(seg == segSelected ? Pens.Red : Pens.Black, rc);
		}

		private ImageMapSegment [] ExtractSegments()
		{
			ImageMapSegment [] segs = new ImageMapSegment[map.Segments.Count];
            map.Segments.Values.CopyTo(segs, 0);
            return segs;
		}

		private int ImageSize(ImageMapSegment [] segs)
		{
			Address addrStart = segs[0].Address;
			Address addrEnd = segs[segs.Length - 1].Address;
			return segs[segs.Length - 1].Size + (addrEnd - addrStart);
		}

		protected override void OnMouseDown(MouseEventArgs me)
		{
			base.OnMouseDown(me);
			ImageMapSegment [] mapSegments = ExtractSegments();
			float scaleFactor = (float) Height / (float) ImageSize(mapSegments);
			int start = mapSegments[0].Address.Linear;
			foreach (ImageMapSegment seg in mapSegments)
			{
				float y = scaleFactor * (seg.Address.Linear - start);
				float dy = scaleFactor * seg.Size;
				if (y <= me.Y && me.Y < y + dy)
				{
					SelectedSegment = seg;
					return;
				}
			}
		}

		protected void OnSelectedItemChanged()
		{
			if (SelectedItemChanged != null)
			{
				SelectedItemChanged(this, new EventArgs());
			}
		}

		public ImageMapSegment SelectedSegment
		{
			get { return segSelected; }
			set 
			{
				segSelected = value;
				Invalidate();
				OnSelectedItemChanged();
			}
		}
	}
}
