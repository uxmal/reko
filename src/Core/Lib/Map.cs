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

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Decompiler.Core.Lib
{
	/// <summary>
	/// Represents a collection of key-and-value pairs in sorted order. Lookups are just
	/// as efficient as with the SortedList class, while adding and removing elements are much faster.
	/// </summary>
    public class Map<K,V> : SortedList<K,V>
    {
        private IComparer<K> cmp;

        public Map() : this(Comparer<K>.Default)
        {
        }

        public Map(IComparer<K> cmp) : base(cmp)
        {
            this.cmp = cmp;
        }

        public bool TryGetLowerBound(K key, out V value)
        {
            int lo = 0;
            int hi = base.Count - 1;
            value = default(V);
            bool set = false;
            while (lo <= hi)
            {
                int mid = (hi - lo) / 2 + lo;
                K k = base.Keys[mid];
                int c = cmp.Compare(k, key);
                if (c == 0)
                {
                    value = base.Values[mid];
                    return true;
                }
                if (c < 0)
                {
                    value = base.Values[mid];
                    set = true;
                    lo = mid + 1;
                }
                else
                {
                    hi = mid - 1;
                }
            }
            return set;
        }
    }
}
