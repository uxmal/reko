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

using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Describes the contents of the image in terms of regions. The image map
    /// is two-tier: Segments lie on the first level, and under these, we find
    /// the <see cref="ImageMapItem"/>s.
    /// </summary>
    public class ImageMap
    {
        /// <summary>
        /// Constructs an instance of <see cref="ImageMap"/>.
        /// </summary>
        /// <param name="addrBase">The base address of the image map.</param>
        public ImageMap(Address addrBase)
        {
            BaseAddress = addrBase;
            Items = new ConcurrentBTreeDictionary<Address, ImageMapItem>(new ItemComparer());
        }

        /// <summary>
        /// Constructs a copy if the image map.
        /// </summary>
        /// <param name="that"></param>
        public ImageMap(ImageMap that)
        {
            BaseAddress = that.BaseAddress;
            Items = new ConcurrentBTreeDictionary<Address, ImageMapItem>(that.Items);
        }

        /// <summary>
        /// The base address of the image map.
        /// </summary>
        public Address BaseAddress { get; }

        /// <summary>
        /// The items contained in the image map.
        /// </summary>
        public ConcurrentBTreeDictionary<Address, ImageMapItem> Items { get; }

        /// <summary>
        /// Adds an image map item at the specified address, possibly splitting
        /// items that were already at that location.
        /// </summary>
        /// <param name="addr">Address at which to add the item.</param>
        /// <param name="itemNew">The item to add.</param>
        /// <returns>An item corresponding to the added item.</returns>
        public ImageMapItem AddItem(Address addr, ImageMapItem itemNew)
        {
            itemNew.Address = addr;
            if (!TryFindItem(addr, out ImageMapItem? item))
            {
                // Outside of range.
                Items.Add(itemNew.Address, itemNew);
                return itemNew;
            }
            else
            {
                long delta = addr - item!.Address;
                Debug.Assert(delta >= 0, "TryFindItem is supposed to find a block whose address is <= the supplied address");
                if (delta > 0)
                {
                    if (delta < item.Size)
                    {
                        // Need to split the item.

                        itemNew.Size = (uint) (item.Size - delta);
                        item.Size = (uint) delta;
                        Items.Add(itemNew.Address, itemNew);
                        return itemNew;
                    }
                    else
                    {
                        Items.Add(itemNew.Address, itemNew);
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
                        return itemNew;
                    }
                    if (item.GetType() != itemNew.GetType())    //$BUGBUG: replaces the type.
                    {
                        Items[itemNew.Address] = itemNew;
                        itemNew.Size = item.Size;
                    }
                    return item;
                }
            }
        }

        /// <summary>
        /// Add a sized <paramref name="itemNew"/> to the image map at the given <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address at which to add the item.</param>
        /// <param name="itemNew">Item to add.</param>
        public void AddItemWithSize(Address addr, ImageMapItem itemNew)
        {
            if (!TryFindItem(addr, out var item))
            {
                throw new ArgumentException(string.Format("Address {0} is not within the image range.", addr));
            }
            // Do not split items with known data.
            if (!(item!.DataType is UnknownType || item.DataType is CodeType))
                return;
            long delta = addr - item.Address;
            Debug.Assert(delta >= 0, "Should have found an item at the supplied address.");
            if (delta > 0)
            {
                int afterOffset = (int) (delta + itemNew.Size);
                ImageMapItem? itemAfter = null;
                if (item.Size > afterOffset)
                {
                    itemAfter = new ImageMapItem(addr + itemNew.Size)
                    {
                        Size = (uint) (item.Size - afterOffset),
                        DataType = ChopBefore(item.DataType, afterOffset),
                    };
                }
                item.Size = (uint) delta;
                item.DataType = ChopAfter(item.DataType, (int) delta);      // Shrink the existing mofo.

                Items.Add(addr, itemNew);
                if (itemAfter is not null)
                {
                    Items.Add(itemAfter.Address, itemAfter);
                }
            }
            else
            {
                if (item.DataType is not UnknownType &&
                    item.DataType is not CodeType)
                {
                    var u = new Unifier();
                    if (u.AreCompatible(item.DataType, itemNew.DataType))
                    {
                        item.DataType = u.Unify(item.DataType, itemNew.DataType)!;
                    }
                    else
                    {
                        throw new NotSupportedException("Haven't handled this case yet.");
                    }
                }
                Items.Remove(item.Address);
                if (item.Size > 0 && itemNew.Size < item.Size)
                {
                    item.Address += itemNew.Size;
                    item.Size -= itemNew.Size;

                    Items.Add(addr, itemNew);
                    if (item.Size > 0 && !Items.ContainsKey(item.Address))
                    {
                        Items.Add(item.Address, item);
                    }
                }
                else
                {
                    Items.Add(addr, itemNew);
                }
            }
        }

        /// <summary>
        /// Make a copy of the image map.
        /// </summary>
        /// <returns></returns>
        public ImageMap Clone()
        {
            return new ImageMap(this);
        }

        private DataType ChopAfter(DataType type, int offset)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type is UnknownType || type is CodeType)
                return type;
            throw new NotImplementedException($"Cannot chop image map item of type {type}.");
        }

        private DataType ChopBefore(DataType type, int offset)
        {
            if (type is null)
                throw new ArgumentNullException(nameof(type));
            if (type is UnknownType || type is CodeType)
                return type;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Terminate any existing item at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address to terminate block at.</param>
        public void TerminateItem(Address addr)
        {
            if (!TryFindItem(addr, out ImageMapItem? item))
                return;
            long delta = addr - item.Address;
            if (delta == 0)
                return;

            // Need to split the item.
            var itemNew = new ImageMapItem(addr);
            if (item.Size != 0)
            {
                itemNew.Size = (uint) (item.Size - delta);
            }
            Items.Add(itemNew.Address, itemNew);

            item.Size = (uint) delta;
        }

        /// <summary>
        /// Remove the item that is exactly at address <paramref name="addr"/>.
        /// </summary>
        /// <param name="addr">Address of the item to be removed.</param>
        public void RemoveItem(Address addr)
        {
            if (!TryFindItemExact(addr, out ImageMapItem item))
                return;

            item.DataType = new UnknownType();

            ImageMapItem mergedItem = item;

            // Merge with previous item
            if (Items.TryGetLowerBound(addr - 1, out ImageMapItem? prevItem) &&
                prevItem.DataType is UnknownType &&
                prevItem.DataType.Size == 0 &&
                prevItem.EndAddress.Equals(item.Address))
            {
                mergedItem = prevItem;

                mergedItem.Size = (uint) (item.EndAddress - mergedItem.Address);
                Items.Remove(item.Address);
            }

            // Merge with next item

            if (Items.TryGetUpperBound(addr + 1, out ImageMapItem? nextItem) &&
                nextItem.DataType is UnknownType &&
                nextItem.DataType.Size == 0 &&
                mergedItem.EndAddress.Equals(nextItem.Address))
            {
                mergedItem.Size = (uint) (nextItem.EndAddress - mergedItem.Address);
                Items.Remove(nextItem.Address);
            }
        }

        /// <summary>
        /// Try to find a map item that contains the given address.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="item"></param>
        /// <returns></returns>
		public bool TryFindItem(Address addr, [MaybeNullWhen(false)] out ImageMapItem item)
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

        // class ItemComparer //////////////////////////////////////////////////

        private class ItemComparer : IComparer<Address>
        {
            public virtual int Compare(Address a, Address b)
            {
                return a.ToLinear().CompareTo(b.ToLinear());
            }
        }

        /// <summary>
        /// Debugging method.
        /// </summary>
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

        /// <summary>
        /// The size of the item in bytes.
        /// </summary>
        public uint Size
        { 
            get { return _size; } 
            set { if ((int) value < 0) throw new ArgumentException(); _size = value; } 
        }
        private uint _size;

        /// <summary>
        /// Optional name of the item.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Data type of the item.
        /// </summary>
        public DataType DataType { get; set; }

        /// <summary>
        /// Constructs an instance of <see cref="ImageMapItem"/>.
        /// </summary>
        /// <param name="addr">The address of the item.</param>
        /// <param name="size">The size of the item.</param>
        public ImageMapItem(Address addr, uint size)
        {
            Address = addr;
            Size = size;
            DataType = new UnknownType();
        }

        /// <summary>
        /// Constructs an instance of <see cref="ImageMapItem"/>
        /// of unknown size.
        /// </summary>
        /// <param name="addr"></param>
        public ImageMapItem(Address addr)
        {
            Address = addr;
            DataType = new UnknownType();
        }

        /// <summary>
        /// The address of the item.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        /// The end address of the item.
        /// </summary>
        public Address EndAddress => Address + Size;

        /// <summary>
        /// Determines whether the address is contained in the item.
        /// </summary>
        /// <param name="addr"></param>
        /// <returns>True if the address is in range.</returns>
        public bool IsInRange(Address addr)
        {
            return IsInRange(addr.ToLinear());
        }

        /// <summary>
        /// Determines whether the linear  address is contained in the item.
        /// </summary>
        /// <param name="linearAddress"></param>
        /// <returns>True if the address is in range.</returns>
        public bool IsInRange(ulong linearAddress)
        {
            ulong linItem = Address.ToLinear();
            return linItem <= linearAddress && linearAddress < linItem + Size;
        }

        /// <summary>
        /// Returns a string representation of the item.
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}, size: {1}, type:{2}", Address, Size, DataType);
        }
    }
}
