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
        /// Retrieves a relocated value at the <paramref name="imageOffset"/>.
        /// </summary>
        /// <param name="imageOffset"></param>
        /// <returns></returns>
        public Constant this[ulong imageOffset]
        {
            get
            {
                Constant c;
                if (map.TryGetValue(imageOffset, out c))
                    return c;
                else
                    return null;
            }
        }

        public void AddPointerReference(ulong linAddress, uint pointer)
		{
			var c = Constant.Create(PrimitiveType.Pointer32, pointer);
			map.Add(linAddress, c);
		}

		public void AddSegmentReference(ulong linAddress, ushort segmentSelector)
		{
            var c = Constant.Create(PrimitiveType.SegmentSelector, segmentSelector);
			map.Add(linAddress, c);
		}

		public bool Contains(uint imageOffset)
		{
			return map.ContainsKey(imageOffset);
		}

        /// <summary>
        /// Returns true if the specified address + length partially overlaps
        /// a relocation. 
        /// </summary>
        /// <returns></returns>
        public bool Overlaps(Address addr, uint length)
        {
            ulong linAddr = addr.ToLinear();
            ulong linAddrReloc;
            if (map.TryGetLowerBoundKey(linAddr, out linAddrReloc))
            {
                var relocLength = (uint)map[linAddrReloc].DataType.Size;
                if (linAddrReloc + relocLength > linAddr)
                {
                    if (linAddrReloc + relocLength <= linAddr + length)
                        return true;
                }
            }
            if (map.TryGetUpperBoundKey(linAddr, out linAddrReloc))
            {
                Debug.Assert(linAddr < linAddrReloc);
                var relocLength = (uint)map[linAddrReloc].DataType.Size;
                return
                    linAddr + length > linAddrReloc &&
                    linAddr + length < linAddrReloc + relocLength;
            }
            return false;
        }

        public int Count
        {
            get { return map.Count; }
        }
	}
}
