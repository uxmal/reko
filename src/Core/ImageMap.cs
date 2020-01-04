#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
	/// Describes the contents of the image in terms of regions. The image map
    /// is two-tier: Segments lie on the first level, and under these, we find
    /// the procedures.
	/// </summary>
	public class ImageMap
	{
        public event EventHandler MapChanged;

        public ImageMap(Address addrBase)
        {
            this.BaseAddress = addrBase ?? throw new ArgumentNullException(nameof(addrBase));
            this.Items = new ConcurrentBTreeDictionary<Address, ImageMapItem>(new ItemComparer());
        }

        public Address BaseAddress { get; }

        public ConcurrentBTreeDictionary<Address, ImageMapItem> Items { get; }

        /// <summary>
        /// Adds an image map item at the specified address. 
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="itemNew"></param>
        /// <returns></returns>
        public ImageMapItem AddItem(Address addr, ImageMapItem itemNew)
		{
			itemNew.Address = addr;
            if (!TryFindItem(addr, out ImageMapItem item))
            {
                // Outside of range.
                Items.Add(itemNew.Address, itemNew);
                FireMapChanged();
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
                        Items.Add(itemNew.Address, itemNew);
                        FireMapChanged();
                        return itemNew;
                    }
                    else
                    {
                        Items.Add(itemNew.Address, itemNew);
                        FireMapChanged();
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
                        Items[itemNew.Address] = itemNew;
                        Items[item.Address] = item;
                        FireMapChanged();
                        return itemNew;
                    }
                    if (item.GetType() != itemNew.GetType())    //$BUGBUG: replaces the type.
                    {
                        Items[itemNew.Address] = itemNew;
                        itemNew.Size = item.Size;
                    }
                    FireMapChanged();
                    return item;
                }
            }
		}

        public void AddItemWithSize(Address addr, ImageMapItem itemNew)
        {
            if (!TryFindItem(addr, out var item))
            {
                throw new ArgumentException(string.Format("Address {0} is not within the image range.", addr));
            }
            // Do not split items with known data.
            if (!(item.DataType is UnknownType || item.DataType is CodeType))
                return;
            long delta = addr - item.Address;
            Debug.Assert(delta >= 0, "Should have found an item at the supplied address.");
            if (delta > 0)
            {
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

                Items.Add(addr, itemNew);
                if (itemAfter != null)
                {
                    Items.Add(itemAfter.Address, itemAfter);
                }
            }
            else
            {
                if (!(item.DataType is UnknownType) &&
                    !(item.DataType is CodeType))
                {
                    var u = new Unifier();
                    if (u.AreCompatible(item.DataType, itemNew.DataType))
                    {
                        item.DataType = u.Unify(item.DataType, itemNew.DataType);
                    }
                    else
                    {
                        throw new NotSupportedException("Haven't handled this case yet.");
                    }
                }
                Items.Remove(item.Address);
                item.Address += itemNew.Size;
                item.Size -= itemNew.Size;

                Items.Add(addr, itemNew);
                if (item.Size > 0 && !Items.ContainsKey(item.Address))
                {
                    Items.Add(item.Address, item);
                }
            }
            FireMapChanged();
        }

        private DataType ChopAfter(DataType type, int offset)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type is UnknownType || type is CodeType)
                return type;
            throw new NotImplementedException(string.Format("Cannot chop image map item of type {0}.", type));
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
            if (!TryFindItem(addr, out ImageMapItem item))
                return;
            long delta = addr - item.Address;
            if (delta == 0)
                return;

            // Need to split the item.
            var itemNew = new ImageMapItem { Address = addr };
            if (item.Size != 0)
            {
                itemNew.Size = (uint) (item.Size - delta);
            }
            Items.Add(itemNew.Address, itemNew);

            item.Size = (uint)delta;
        }

        public void RemoveItem(Address addr)
        {
            if (!TryFindItemExact(addr, out ImageMapItem item))
                return;

            item.DataType = new UnknownType();

            ImageMapItem mergedItem = item;

            // Merge with previous item
            if (Items.TryGetLowerBound((addr - 1), out ImageMapItem prevItem) &&
                prevItem.DataType is UnknownType ut &&
                prevItem.DataType.Size == 0 &&
                prevItem.EndAddress.Equals(item.Address))
            {
                mergedItem = prevItem;

                mergedItem.Size = (uint)(item.EndAddress - mergedItem.Address);
                Items.Remove(item.Address);
            }

            // Merge with next item
            
            if (Items.TryGetUpperBound((addr + 1), out ImageMapItem nextItem) &&
                nextItem.DataType is UnknownType &&
                nextItem.DataType.Size == 0 &&
                mergedItem.EndAddress.Equals(nextItem.Address))
            {
                mergedItem.Size = (uint)(nextItem.EndAddress - mergedItem.Address);
                Items.Remove(nextItem.Address);
            }

            FireMapChanged();
        }

        /// <summary>
        /// Try to find a map item that contains the given address.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="item"></param>
        /// <returns></returns>
		public bool TryFindItem(Address addr, out ImageMapItem item)
		{
			return Items.TryGetLowerBound(addr, out item);
		}

        /// <summary>
        /// Try to find a map item that starts with the given address.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="item"></param>
        /// <returns></returns>
		public bool TryFindItemExact(Address addr, out ImageMapItem item)
		{
            return Items.TryGetValue(addr, out item);
		}

        //$TODO: the following code is a stopgap to prevent unnecessary reloading
        // of the user interface during background operations. In the future,
        // Reko will need to handle this in a better way, but the changes required
        // are rather large. See GitHub [issue #567](https://github.com/uxmal/reko/issues/567)
        // for the gory details.
        private bool mapChangedEventHandlerPaused;
        private bool mapChangedPendingEvents;

        private void FireMapChanged()
        {
            if (!mapChangedEventHandlerPaused) MapChanged.Fire(this);
            else mapChangedPendingEvents = true;
        }

        public void PauseEventHandler()
        {
            mapChangedEventHandlerPaused = true;
        }

        public void UnpauseEventHandler()
        {
            mapChangedEventHandlerPaused = false;
            if (mapChangedPendingEvents) MapChanged.Fire(this);
            mapChangedPendingEvents = false;
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
    }

	/// <summary>
	/// Represents a span of memory. 
    /// //$REVIEW: note the similarity to StructureField. It is not coincidental.
    /// Can these be merged?
	/// </summary>
	public class ImageMapItem
	{
        private uint _size;
        public uint Size { get { return _size; } set { if ((int) value < 0) throw new ArgumentException(); _size = value; } }
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
