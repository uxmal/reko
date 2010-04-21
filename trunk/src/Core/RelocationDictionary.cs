/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;

namespace Decompiler.Core
{
	/// <summary>
	/// Maps program image addresses to relocated values (with correct primitive types).
	/// </summary>
	public class RelocationDictionary 
    {
        private Dictionary<int, Constant> map = new Dictionary<int, Constant>();

        public Constant this[int imageOffset]
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

		public void AddPointerReference(int imageOffset, uint pointer)
		{
			Constant c = new Constant(PrimitiveType.Pointer32, pointer);
			map.Add(imageOffset, c);
		}

		public void AddSegmentReference(int imageOffset, ushort segmentSelector)
		{
			Constant c = new Constant(PrimitiveType.SegmentSelector, segmentSelector);
			map.Add(imageOffset, c);
		}

		public bool Contains(int imageOffset)
		{
			return map.ContainsKey(imageOffset);
		}

        public int Count
        {
            get { return map.Count; }
        }
	}
}
