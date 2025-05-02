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

using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Implements a Disjoint-set Data Structure.
    /// </summary>
    public class DisjointPartition<T> where T : class
    {
        private readonly Dictionary<T, Set> items;

        /// <summary>
        /// Creates an empty disjoint partition.
        /// </summary>
        public DisjointPartition()
        {
            items = new Dictionary<T, Set>();
        }

        /// <summary>
        /// Adds an item to the partition.
        /// </summary>
        /// <param name="item"></param>
        public void Add(T item)
        {
            items.Add(item, new Set(item));
        }

        /// <summary>
        /// Unifies two sets.
        /// </summary>
        /// <param name="x">First set.</param>
        /// <param name="y">Second set.</param>
        public void Union(T x, T y)
        {
            Set xRoot = FindSet(items[x]);
            Set yRoot = FindSet(items[y]);
            if (xRoot.rank > yRoot.rank)
            {
                yRoot.parent = xRoot;
            }
            else if (xRoot.rank < yRoot.rank)
            {
                xRoot.parent = yRoot;
            }
            else if (xRoot != yRoot)
            {
                yRoot.parent = xRoot;
                ++xRoot.rank;
            }
        }

        /// <summary>
        /// Find the representative of the set containing the item.
        /// </summary>
        /// <param name="item">Item whose representative is to be found.</param>
        /// <returns>The representative.
        /// </returns>
        public T Find(T item)
        {
            Set x = items[item];
            return FindSet(x).item;
        }

        private Set FindSet(Set x)
        {
            if (x.parent == x)
                return x;
            else
            {
                x.parent = FindSet(x.parent);
                return x.parent;
            }
        }

        private class Set
        {
            public Set(T item) { this.item = item;  parent = this; rank = 0; }
            public T item;
            public Set parent;
            public int rank;
        }
    }
}

