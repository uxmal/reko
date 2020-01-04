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

using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Reko.Core
{
	/// <summary>
	/// Maps program image addresses to relocated values (with correct primitive types).
	/// </summary>
	public class RelocationDictionary 
    {
        private SortedList<ulong, Constant> map = new SortedList<ulong, Constant>();

        /// <summary>
        /// Retrieves a relocated value at the <paramref name="linAddress"/>.
        /// </summary>
        /// <param name="linAddress"></param>
        /// <returns></returns>
        public Constant this[ulong linAddress]
        {
            get
            {
                Constant c;
                if (map.TryGetValue(linAddress, out c))
                    return c;
                else
                    return null;
            }
        }

        public void AddPointerReference(ulong linAddress, uint pointer)
		{
			var c = Constant.Create(PrimitiveType.Ptr32, pointer);
			map.Add(linAddress, c);
		}

		public void AddSegmentReference(ulong linAddress, ushort segmentSelector)
		{
            var c = Constant.Create(PrimitiveType.SegmentSelector, segmentSelector);
			map.Add(linAddress, c);
		}

		public bool Contains(uint linAddress)
		{
			return map.ContainsKey(linAddress);
		}

        /// <summary>
        /// Returns true if the specified address + length partially overlaps
        /// a relocation. 
        /// </summary>
        /// <returns></returns>
        public bool Overlaps(Address addr, uint length)
        {
            ulong linAddr = addr.ToLinear();
            ulong linAddrEnd = linAddr + length;
            ulong linReloc;
            if (map.TryGetLowerBoundKey(linAddr, out linReloc))
            {
                // |-reloc----|
                //      |-addr----|
                var linRelocEnd = linReloc + (uint)map[linReloc].DataType.Size;
                if (linReloc < linAddr && linAddr < linRelocEnd)
                {
                    return true;
                }
            }
            if (map.TryGetUpperBoundKey(linAddr, out linReloc))
            {
                //     |-reloc----|
                // |-addr----|
                var linRelocEnd = linReloc + (uint)map[linReloc].DataType.Size;
                if (linReloc < linAddrEnd)
                {
                    if (linAddrEnd < linRelocEnd)
                        return true;
                }
            }
            return false;
        }

        public int Count
        {
            get { return map.Count; }
        }

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
