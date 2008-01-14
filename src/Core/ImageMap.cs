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

using System;
using System.Collections;
using System.Diagnostics;

namespace Decompiler.Core
{
	public delegate void ItemSplitHandler(object o, ItemSplitArgs isa);
	public delegate void SegmentSplitHandler(object o, SegmentSplitArgs ssa);

	/// <summary>
	/// Describes the contents of the image in terms of regions. The image map is two-tier:
	/// Segments lie on the first level, and under these, we find the procedures.
	/// </summary>
	public class ImageMap
	{
		private Map items;
		private Map segments;

		public event ItemSplitHandler ItemSplit;
		public event ItemSplitHandler ItemCoincides;
		public event SegmentSplitHandler SegmentSplit;

		public ImageMap(Address addrBase, int imageSize)
		{
			Init();
			SetAddressSpan(addrBase, imageSize);
		}

		public ImageMapItem AddItem(Address addr, ImageMapItem itemNew)
		{
			itemNew.Address = addr;
			ImageMapItem item = FindItem(addr);
			if (item == null)
			{
				// Outside of range.
				items.Add(itemNew.Address, itemNew);
				return itemNew;
			}
			else
			{
				int delta = addr - item.Address;
				Debug.Assert(delta >= 0);
				if (delta > 0)
				{
					// Need to split the item.

					itemNew.Size = item.Size - delta;
					item.Size = delta;
					items.Add(itemNew.Address, itemNew);

					OnSplitItem(item, itemNew);
					return itemNew;
				}
				else
				{
					OnItemCoincides(item, itemNew);
					return item;
				}
			}
		}

		public ImageMapSegment AddSegment(Address addr, string segmentName, AccessMode access)
		{
			ImageMapSegment seg = FindSegment(addr);
			if (seg == null)
			{
				ImageMapSegment segNew = new ImageMapSegment(segmentName, access);
				segNew.Address = addr;
				segNew.Size = -1;
				segments.Add(segNew.Address, segNew);
				return segNew;
			}
			int delta = addr - seg.Address;
			Debug.Assert(delta >= 0);
			if (delta > 0)
			{
				// Need to split the segment.

				ImageMapSegment segNew = new ImageMapSegment(segmentName, access);
				segNew.Address = addr;
				segNew.Size = seg.Size - delta;
				seg.Size = delta;
				segments.Add(segNew.Address, segNew);
				OnSplitSegment(seg, segNew);

				// And split any items in the segment.

				AddItem(addr, new ImageMapItem());
				return segNew;
			}
			return seg;
		}

		public void SetAddressSpan(Address addr, int size)
		{
			items.Clear();
			segments.Clear();

			ImageMapSegment seg = new ImageMapSegment("Image base", size, AccessMode.ReadWrite);
			seg.Address = addr;
			segments.Add(addr, seg);

			ImageMapItem it = new ImageMapItem(size);
			it.Address = addr;
			items.Add(addr, it);
		}

		public IDictionaryEnumerator LowerBound(Address addr)
		{
			int linAddr = addr.Linear;
			IDictionaryEnumerator e = items.GetEnumerator();
			IDictionaryEnumerator e2 = items.GetEnumerator();
			while (e.MoveNext())
			{
				if (((Address) e).Linear > linAddr)
					return e2;
				e2.MoveNext();
			}
			return e;
		}

		public IEnumerator LowerBoundEnumerator(Address addr)
		{
			return items.GetBoundedEnumerator(addr);
		}

		public ImageMapItem FindItem(Address addr)
		{
			return (ImageMapItem) items.LowerBound(addr);
		}

		public ImageMapItem FindItemExact(Address addr)
		{
			return (ImageMapItem) items[addr];
		}
		
		/// <summary>
		/// Returns the segment that contains the specified address.
		/// </summary>
		/// <param name="addr"></param>
		/// <returns></returns>
		public ImageMapSegment FindSegment(Address addr)
		{
			return (ImageMapSegment) segments.LowerBound(addr);
		}

		public IEnumerator GetItemEnumerator(Address addr)
		{
			return items.GetBoundedEnumerator(addr);
		}

		public IDictionaryEnumerator GetSegmentEnumerator(Address addr)
		{
			return segments.GetBoundedEnumerator(addr);
		}

		private void Init()
		{
			segments = new Map(new ItemComparer());
			items = new Map(new ItemComparer());
		}

		public bool IsReadOnlyAddress(Address addr)
		{
			ImageMapSegment seg = FindSegment(addr);
			return (seg != null && (seg.Access & AccessMode.Write) == 0);
		}

		public bool IsExecutableAddress(Address addr)
		{
			ImageMapSegment seg = FindSegment(addr);
			return (seg != null && (seg.Access & AccessMode.Execute) != 0);
		}

		public Address MapLinearAddressToAddress(int linearAddress)
		{
			foreach (ImageMapSegment seg in segments.Values)
			{
				if (seg.IsInRange(linearAddress))
					return new Address(seg.Address.seg, (uint) (linearAddress - seg.Address.Linear));
			}			
			throw new ArgumentOutOfRangeException("linear address {0:X8} exceeeds known address range");
		}

		private void OnItemCoincides(ImageMapItem item, ImageMapItem itemNew)
		{
			if (ItemCoincides != null)
			{
				ItemCoincides(this, new ItemSplitArgs(item, itemNew));
			}
		}

		private void OnSplitItem(ImageMapItem item, ImageMapItem itemNew)
		{
			if (ItemSplit != null)
			{
				ItemSplit(this, new ItemSplitArgs(item, itemNew));
			}
		}

		private void OnSplitSegment(ImageMapSegment seg, ImageMapSegment segNew)
		{
			if (SegmentSplit != null)
			{
				SegmentSplit(this, new SegmentSplitArgs(seg, segNew));
			}
		}

		public Map Items
		{
			get { return items; }
		}
		
		public Map Segments
		{
			get { return segments; }
		}

		// class ItemComparer //////////////////////////////////////////////////

		private class ItemComparer : IComparer
		{
			public virtual int Compare(object a, object b)
			{
				int diff = ((Address)a) - ((Address)b);
				return diff;
			}
		}
	}

	/// <summary>
	/// Represents a span of memory. 
	/// </summary>
	public class ImageMapItem
	{
		public Address Address;
		public int Size;

		public ImageMapItem(int size)
		{
			Debug.Assert(size > 0);
			this.Size = size;
		}

		public ImageMapItem()
		{
		}

		public bool IsInRange(Address addr)
		{
			return IsInRange(addr.Linear);
		}

		public bool IsInRange(int linearAddress)
		{
			int linItem = this.Address.Linear;
			return (linItem <= linearAddress && linearAddress < linItem + Size);
		}

		public override string ToString()
		{
			return Address.ToString() + ", size: " + Size;
		}
	}

	public class SegmentSplitArgs : EventArgs
	{
		public ImageMapSegment SegmentOld;
		public ImageMapSegment SegmentNew;

		public SegmentSplitArgs(ImageMapSegment seg, ImageMapSegment segNew)
		{
			SegmentOld = seg;
			SegmentNew = segNew;
		}
	}

	public class ItemSplitArgs : EventArgs
	{
		public ImageMapItem ItemOld;
		public ImageMapItem ItemNew;

		public ItemSplitArgs(ImageMapItem it, ImageMapItem itNew)
		{
			ItemOld = it;
			ItemNew = itNew;
		}
	}
}
