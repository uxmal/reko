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

using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Maps program image addresses to relocated values (with correct primitive types).
    /// </summary>
    public class RelocationDictionary
    {
        private readonly SortedList<ulong, Constant> map = new();

        /// <summary>
        /// Retrieves a relocated value at the <paramref name="linAddress"/>.
        /// </summary>
        /// <param name="linAddress"></param>
        /// <returns></returns>
        public Constant? this[ulong linAddress]
        {
            get
            {
                if (map.TryGetValue(linAddress, out Constant? c))
                    return c;
                else
                    return null;
            }
        }

        /// <summary>
        /// Add a reference to a relocated pointer.
        /// </summary>
        /// <param name="linAddress">Address at which the relocation occurred.</param>
        /// <param name="pointer">Relocated pointer value.</param>
        public void AddPointerReference(ulong linAddress, uint pointer)
        {
            var c = Constant.Create(PrimitiveType.Ptr32, pointer);
            map.Add(linAddress, c);
        }

        /// <summary>
        /// Add a reference to a relocated segment selector.
        /// </summary>
        /// <param name="linAddress">Address at which the relocation occurred.</param>
        /// <param name="segmentSelector">Relocated selector value.</param>
        public void AddSegmentReference(ulong linAddress, ushort segmentSelector)
        {
            var c = Constant.Create(PrimitiveType.SegmentSelector, segmentSelector);
            map.Add(linAddress, c);
        }

        /// <summary>
        /// Returns true if this relocation dictionary contains a relocation
        /// at <paramref name="linAddress"/>.
        /// </summary>
        /// <param name="linAddress">Address to check.</param>
        /// <returns>True if there is a relocation at the given address; otherwise false.
        /// </returns>
        public bool Contains(uint linAddress)
        {
            return map.ContainsKey(linAddress);
        }

        /// <summary>
        /// Returns true if the specified address + length partially overlaps
        /// a relocation. 
        /// </summary>
        /// <returns>True if a relocated address is located in the specified
        /// range; otherwise false.
        /// </returns>
        public bool Overlaps(Address addr, uint length)
        {
            ulong linAddr = addr.ToLinear();
            ulong linAddrEnd = linAddr + length;
            if (map.TryGetLowerBoundKey(linAddr, out ulong linReloc))
            {
                // |-reloc----|
                //      |-addr----|
                var linRelocEnd = linReloc + (uint) map[linReloc].DataType.Size;
                if (linReloc < linAddr && linAddr < linRelocEnd)
                {
                    return true;
                }
            }
            if (map.TryGetUpperBoundKey(linAddr, out linReloc))
            {
                //     |-reloc----|
                // |-addr----|
                var linRelocEnd = linReloc + (uint) map[linReloc].DataType.Size;
                if (linReloc < linAddrEnd)
                {
                    if (linAddrEnd < linRelocEnd)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the number of items in the relocation dictionary.
        /// </summary>
        public int Count => map.Count;


        /// <summary>
        /// Dumps the contants of the relocation dictionary to debug output.
        /// </summary>
        /// <param name="addrBase"></param>
        [Conditional("DEBUG")]
        public void Dump(Address addrBase)
        {
            var lin = addrBase.ToLinear();
            foreach (var de in map)
            {
                Debug.Print("{0:X8} - {1}", de.Key - lin, de.Value);
            }
        }
    }
}
