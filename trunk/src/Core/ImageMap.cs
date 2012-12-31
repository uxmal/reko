#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core.Lib;
using System;
using System.Collections.Generic;
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
		private Map<Address,ImageMapItem> items;
        private Map<Address,ImageMapSegment> segments;

		public event ItemSplitHandler ItemSplit;
		public event ItemSplitHandler ItemCoincides;
		public event SegmentSplitHandler SegmentSplit;

		public ImageMap(Address addrBase, int imageSize)
		{
            if (addrBase == null)
                throw new ArgumentNullException("addrBase");
            items = new Map<Address, ImageMapItem>(new ItemComparer());
            segments = new Map<Address, ImageMapSegment>(new ItemComparer());
			SetAddressSpan(addrBase, (uint) imageSize);
		}

        /// <summary>
        /// Adds an image map item at the specified address. If  <paramref name="addr"/> is 
        /// located inside of an existing address, a notification is sent that that location has
        /// been split.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="itemNew"></param>
        /// <returns></returns>
		public ImageMapItem AddItem(Address addr, ImageMapItem itemNew)
		{
			itemNew.Address = addr;
			ImageMapItem item;
            if (!TryFindItem(addr, out item))
            {
                // Outside of range.
                items.Add(itemNew.Address, itemNew);
                return itemNew;
            }
            else
            {
                int delta = addr - item.Address;
                Debug.Assert(delta >= 0, "TryFindItem is supposed to find a block whose address is <= the supplied address");
                if (delta > 0)
                {
                    // Need to split the item.

                    itemNew.Size = (uint) (item.Size - delta);
                    item.Size = (uint) delta;
                    items.Add(itemNew.Address, itemNew);

                    OnSplitItem(item, itemNew);
                    return itemNew;
                }
                else
                {
                    OnItemCoincides(item, itemNew);
                    if (item.GetType() != itemNew.GetType())
                    {
                        items[itemNew.Address] = itemNew;
                        itemNew.Size = item.Size;
                    }
                    return item;
                }
            }
		}

        public void TerminateItem(Address addr)
        {
            ImageMapItem item;
            if (!TryFindItem(addr, out item))
                return;
            int delta = addr - item.Address;
            if (delta == 0)
                return;
            
            // Need to split the item.
            var itemNew = new ImageMapItem { Address = addr, Size = (uint)(item.Size - delta) };
            items.Add(itemNew.Address, itemNew);

            item.Size = (uint)delta;
            OnSplitItem(item, itemNew);
        }

		public ImageMapSegment AddSegment(Address addr, string segmentName, AccessMode access)
		{
			ImageMapSegment seg;
            if (!TryFindSegment(addr, out seg))
			{
				ImageMapSegment segNew = new ImageMapSegment(segmentName, access);
				segNew.Address = addr;
				segNew.Size = ~0U;
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
				segNew.Size = (uint)(seg.Size - delta);
				seg.Size = (uint) delta;
				segments.Add(segNew.Address, segNew);
				OnSplitSegment(seg, segNew);

				// And split any items in the segment.

				AddItem(addr, new ImageMapItem());
				return segNew;
			}
			return seg;
		}

		public void SetAddressSpan(Address addr, uint size)
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

		public bool TryFindItem(Address addr, out ImageMapItem item)
		{
			return items.TryGetLowerBound(addr, out item);
		}

		public bool TryFindItemExact(Address addr, out ImageMapItem item)
		{
            return items.TryGetValue(addr, out item);
		}
		
		/// <summary>
		/// Returns the segment that contains the specified address.
		/// </summary>
		/// <param name="addr"></param>
		/// <returns></returns>
		public bool TryFindSegment(Address addr, out ImageMapSegment segment)
		{
            return segments.TryGetLowerBound(addr, out segment);
		}

		public bool IsReadOnlyAddress(Address addr)
		{
			ImageMapSegment seg;
            return (TryFindSegment(addr, out seg) && (seg.Access & AccessMode.Write) == 0);
		}

		public bool IsExecutableAddress(Address addr)
		{
			ImageMapSegment seg ;
            return (TryFindSegment(addr, out seg) && (seg.Access & AccessMode.Execute) != 0);
		}

        /// <summary>
        /// Given a linear address, returns an address whose selector, if any, contains the linear address.
        /// </summary>
        /// <param name="linearAddress"></param>
        /// <returns></returns>
		public Address MapLinearAddressToAddress(uint linearAddress)
		{
			foreach (ImageMapSegment seg in segments.Values)
			{
                if (seg.IsInRange(linearAddress))
                {
                    if (seg.Address.Selector != 0)
                    {
                        return new Address(seg.Address.Selector, (uint)(linearAddress - seg.Address.Linear));
                    }
                    else
                        return new Address((uint)linearAddress);
                }
			}			
			throw new ArgumentOutOfRangeException("Linear address {0:X8} exceeeds known address range.");
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

		public Map<Address, ImageMapItem> Items
		{
			get { return items; }
		}

        public Map<Address, ImageMapSegment> Segments
		{
			get { return segments; }
		}

		// class ItemComparer //////////////////////////////////////////////////

		private class ItemComparer : IComparer<Address>
		{
			public virtual int Compare(Address a, Address b)
			{
                return a - b;
			}
		}

    }

	/// <summary>
	/// Represents a span of memory. 
	/// </summary>
	public class ImageMapItem
	{
		public Address Address;
		public uint Size;

		public ImageMapItem(uint size)
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

		public bool IsInRange(uint linearAddress)
		{
			uint linItem = this.Address.Linear;
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
