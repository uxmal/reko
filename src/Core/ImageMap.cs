#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.Core
{
	/// <summary>
	/// Describes the contents of the image in terms of regions. The image map is two-tier:
	/// Segments lie on the first level, and under these, we find the procedures.
	/// </summary>
	public class ImageMap
	{
        public event EventHandler MapChanged;

		private SortedList<Address,ImageMapItem> items;
        private SortedList<Address,ImageSegment> segments;

		public ImageMap(Address addrBase, long imageSize)
		{
            if (addrBase == null)
                throw new ArgumentNullException("addrBase");
            this.BaseAddress = addrBase;
            items = new SortedList<Address, ImageMapItem>(new ItemComparer());
            segments = new SortedList<Address, ImageSegment>(new ItemComparer());
			SetAddressSpan(addrBase, (uint) imageSize);
		}

        public ImageMap(Address addrBase, params ImageSegment [] segments)
        {
            if (addrBase == null)
                throw new ArgumentNullException("addrBase");
            this.BaseAddress = addrBase;
            this.items = new SortedList<Address, ImageMapItem>(new ItemComparer());
            this.segments = new SortedList<Address, ImageSegment>(new ItemComparer());
            foreach (var seg in segments)
            {
                this.AddSegment(seg);
            }
        }

        public Address BaseAddress { get; private set; }

        public long GetExtent()
        {
            var lastMem = segments.Values.Last().MemoryArea;
            return (lastMem.BaseAddress - BaseAddress) + lastMem.Length;
        }

        /// <summary>
        /// Adds an image map item at the specified address. 
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
                MapChanged.Fire(this);
                return itemNew;
            }
            else
            {
                long delta = addr - item.Address;
                Debug.Assert(delta >= 0, "TryFindItem is supposed to find a block whose address is <= the supplied address");
                if (delta > 0)
                {
                    if (delta < item.Size)
                    {
                        // Need to split the item.

                        itemNew.Size = (uint)(item.Size - delta);
                        item.Size = (uint)delta;
                        items.Add(itemNew.Address, itemNew);
                        MapChanged.Fire(this);
                        return itemNew;
                    }
                    else
                    {
                        items.Add(itemNew.Address, itemNew);
                        MapChanged.Fire(this);
                        return itemNew;
                    }
                }
                else
                {
                    if (itemNew.Size > 0 && itemNew.Size != item.Size)
                    {
                        Debug.Assert(item.Size >= itemNew.Size);
                        item.Size -= itemNew.Size;
                        item.Address += itemNew.Size;
                        items[itemNew.Address] = itemNew;
                        items[item.Address] = item;
                        MapChanged.Fire(this);
                        return itemNew;
                    }
                    if (item.GetType() != itemNew.GetType())    //$BUGBUG: replaces the type.
                    {
                        items[itemNew.Address] = itemNew;
                        itemNew.Size = item.Size;
                    }
                    MapChanged.Fire(this);
                    return item;
                }
            }
		}

        public void AddItemWithSize(Address addr, ImageMapItem itemNew)
        {
            ImageMapItem item;
            if (!TryFindItem(addr, out item))
            {
                throw new ArgumentException(string.Format("Address {0} is not within the image range.", addr));
            }
            long delta = addr - item.Address;
            Debug.Assert(delta >= 0, "Should have found an item at the supplied address.");
            if (delta > 0)
            {
                uint originalsize = item.Size;
                int afterOffset = (int)(delta + itemNew.Size);
                ImageMapItem itemAfter = null;
                if (item.Size > afterOffset)
                {
                    itemAfter = new ImageMapItem
                    {
                        Address = addr + itemNew.Size,
                        Size = (uint)(item.Size - afterOffset),
                        DataType = ChopBefore(item.DataType, afterOffset),
                    };
                }
                item.Size = (uint) delta;
                item.DataType = ChopAfter(item.DataType, (int)delta);      // Shrink the existing mofo.

                items.Add(addr, itemNew);
                if (itemAfter != null)
                {
                    items.Add(itemAfter.Address, itemAfter);
                }
            }
            else
            {
                if (!(item.DataType is UnknownType))
                    throw new NotSupportedException("Haven't handled this case yet.");
                items.Remove(item.Address);
                item.Address += itemNew.Size;
                item.Size -= itemNew.Size;

                items.Add(addr, itemNew);
                if (item.Size > 0)
                {
                    items.Add(item.Address, item);
                }
            }
            MapChanged.Fire(this);
        }

        private DataType ChopAfter(DataType type, int offset)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type is UnknownType || type is CodeType)
                return type;
            throw new NotImplementedException();
        }

        private DataType ChopBefore(DataType type, int offset)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type is UnknownType || type is CodeType)
                return type;
            throw new NotImplementedException();
        }

        public void TerminateItem(Address addr)
        {
            ImageMapItem item;
            if (!TryFindItem(addr, out item))
                return;
            long delta = addr - item.Address;
            if (delta == 0)
                return;
            
            // Need to split the item.
            var itemNew = new ImageMapItem { Address = addr, Size = (uint)(item.Size - delta) };
            items.Add(itemNew.Address, itemNew);

            item.Size = (uint)delta;
        }

        public void RemoveItem(Address addr)
        {
            ImageMapItem item;
            if (!TryFindItemExact(addr, out item))
                return;

            item.DataType = new UnknownType();

            ImageMapItem mergedItem = item;

            // Merge with previous item
            ImageMapItem prevItem;
            if (items.TryGetLowerBound((addr - 1), out prevItem) &&
                prevItem.DataType is UnknownType)
            {
                mergedItem = prevItem;

                mergedItem.Size = (uint)(item.EndAddress - mergedItem.Address);
                items.Remove(item.Address);
            }

            // Merge with next item
            ImageMapItem nextItem;
            if (items.TryGetUpperBound((addr + 1), out nextItem) &&
                nextItem.DataType is UnknownType)
            {
                mergedItem.Size = (uint)(nextItem.EndAddress - mergedItem.Address);
                items.Remove(nextItem.Address);
            }
        }

        public ImageSegment AddSegment(MemoryArea mem, string segmentName, AccessMode mode)
        {
            var segment = new ImageSegment(
                    segmentName,
                    mem,
                    mode);
            AddSegment(segment);
            return segment;
        }


        public ImageSegment AddSegment(Address addr, string segmentName, AccessMode access, uint contentSize)
        {
            return AddSegment(new ImageSegment(segmentName, addr, contentSize, access));
        }

        public ImageSegment AddSegment(ImageSegment segNew)
        {
            ImageSegment seg;
            if (!TryFindSegment(segNew.Address, out seg))
            {
                EnsureSegmentSize(segNew);
                segments.Add(segNew.Address, segNew);
                AddItem(segNew.Address, new ImageMapItem(segNew.Size) { DataType = new UnknownType() });
                MapChanged.Fire(this);
                Debug.Print("== New segment {0}", segNew.Name);
                DumpSections();
                return segNew;
            }
            long delta = segNew.Address - seg.Address;
			Debug.Assert(delta >= 0);
			if (delta > 0)
			{
				// Need to split the segment. //$REVIEW: or do we? x86 segments can overlap.

				var segSplit = new ImageSegment(segNew.Name, segNew.Address, segNew.MemoryArea, segNew.Access);
				segSplit.Size = (uint)(seg.Size - delta);
				seg.Size = (uint) delta;
                segments.Add(segNew.Address, segSplit);

				// And split any items in the segment

				AddItem(segSplit.Address, new ImageMapItem());
                MapChanged.Fire(this);
                Debug.Print("== Split segment into {0} and {1}", seg.Name, segSplit.Name);
                DumpSections();
                return segSplit;
            }
			return seg;
        }

        private void EnsureSegmentSize(ImageSegment seg)
        {
            Address addrAbove;
            if (seg.Size == 0)
            {
                if (!Segments.TryGetUpperBoundKey(seg.Address, out addrAbove))
                {
                    // No segment above this one, consume all remaining space.
                    seg.Size = (uint)((seg.MemoryArea.BaseAddress - seg.Address) + seg.MemoryArea.Length);
                }
                else
                {
                    seg.Size = (uint)(addrAbove - seg.Address);
                }
            }
        }

        public void SetAddressSpan(Address addr, uint size)
		{
			items.Clear();
			segments.Clear();

			//ImageMapSegment seg = new ImageMapSegment("Image base", size, AccessMode.ReadWrite);
			//seg.Address = addr;
			//segments.Add(addr, seg);

   //         ImageMapItem it = new ImageMapItem(size) { DataType = new UnknownType() };
			//it.Address = addr;
			//items.Add(addr, it);
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
		public bool TryFindSegment(Address addr, out ImageSegment segment)
		{
            if (!segments.TryGetLowerBound(addr, out segment))
                return false;
            if (segment.Address.ToLinear() == addr.ToLinear())
                return true;
            return segment.IsInRange(addr);
		}

        public bool IsValidAddress(Address address)
        {
            ImageSegment seg;
            return TryFindSegment(address, out seg);
        }

        public bool IsReadOnlyAddress(Address addr)
		{
			ImageSegment seg;
            return (TryFindSegment(addr, out seg) && (seg.Access & AccessMode.Write) == 0);
		}

		public bool IsExecutableAddress(Address addr)
		{
			ImageSegment seg;
            return (TryFindSegment(addr, out seg) && (seg.Access & AccessMode.Execute) != 0);
		}

        /// <summary>
        /// Given a linear address, returns an address whose selector, if any, contains the linear address.
        /// </summary>
        /// <param name="linearAddress"></param>
        /// <returns></returns>
		public Address MapLinearAddressToAddress(ulong linearAddress)
		{
            //$REVIEW: slow; use binary search at least?
            foreach (ImageSegment seg in segments.Values)
			{
                if (seg.IsInRange(linearAddress))
                {
                    long offset = (long)linearAddress- (long)seg.Address.ToLinear();
                    return seg.Address + offset;
                }
			}			
			throw new ArgumentOutOfRangeException(
                string.Format("Linear address {0:X8} is not in known segment.",
                linearAddress));
		}

        public ImageSegmentRenderer Renderer { get; set; }

		public SortedList<Address, ImageMapItem> Items
		{
			get { return items; }
		}

        public SortedList<Address, ImageSegment> Segments
		{
			get { return segments; }
		}

        // class ItemComparer //////////////////////////////////////////////////

        private class ItemComparer : IComparer<Address>
		{
			public virtual int Compare(Address a, Address b)
			{
                return a.ToLinear().CompareTo(b.ToLinear());
			}
		}

        [Conditional("DEBUG")]
        public void Dump()
        {
            foreach (var item in Items)
            {
                Debug.Print("Key: {0}, Value: size: {1}, Type: {2}", item.Key, item.Value.Size, item.Value.DataType);
            }
        }

        [Conditional("DEBUG")]
        public void DumpSections()
        {
            foreach (var item in Segments)
            {
                Debug.Print("Key: {0}, Value: name:{1,-18} size: {2:X8}, Access: {3}", item.Key, item.Value.Name,  item.Value.Size, item.Value.Access);
            }
        }
    }

	/// <summary>
	/// Represents a span of memory. 
    /// //$REVIEW: note the similarity to StructureField. It is not coincidental.
	/// </summary>
	public class ImageMapItem
	{
		public uint Size;
        public string Name;
        public DataType DataType;

        public ImageMapItem(uint size)
		{
			this.Size = size;
            DataType = new UnknownType();
        }

        public ImageMapItem()
		{
            DataType = new UnknownType();
		}

        public Address Address { get; set; }
        public Address EndAddress { get { return Address + Size; } }

        public bool IsInRange(Address addr)
		{
			return IsInRange(addr.ToLinear());
		}

		public bool IsInRange(ulong linearAddress)
		{
            ulong linItem = this.Address.ToLinear();
			return (linItem <= linearAddress && linearAddress < linItem + Size);
		}

		public override string ToString()
		{
            return string.Format("{0}, size: {1}, type:{2}", Address, Size, DataType);
		}
	}
}
