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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Implementation of <see cref="IComparer{T}"/> that compares 
    /// <see cref="Address"/>es lexicographically. That is: it first 
    /// compares the segment selector values (if any), and only if they differ
    /// does it compare the address offsets.
    /// </summary>
    /// <remarks>
    /// This is no different then the default comparer for <see cref="Address"/>,
    /// if the addresses are linear. However, when comparing following real-mode addresses
    /// <c>0000:0100</c> and <c>0001:00F0</c>, according to lexicographical ordering
    /// <c>0000:0100</c> is considered "less than" <c>0001:00F0</c>. while the default
    /// address comparer treats them as equal.
    /// </remarks>
    public class LexicographicalAddressComparer : IComparer<Address>, IEqualityComparer<Address>
    {
        /// <summary>
        /// Compares two addresses and returns whether they are less than, equal, or larger than
        /// each other.
        /// </summary>
        /// <param name="x">The first address to compare.</param>
        /// <param name="y">The second address to compare.</param>
        /// <returns>A negative value if <paramref name="x"/> is lexicographically
        /// less than <paramref name="y"/>, a positive value if <paramref name="x"/> is 
        /// lexicographically greater than <paramref name="y"/>, or 0 if they are equal.
        /// </returns>
        public int Compare(Address x, Address y)
        {
            int d;
            if (x.Selector.HasValue)
            {
                if (!y.Selector.HasValue)
                    return 1;
                d = x.Selector.Value.CompareTo(y.Selector.Value);
                if (d != 0)
                    return d;
            }
            else if (!y.Selector.HasValue)
                return 1;
            return x.Offset.CompareTo(y.Offset);
        }

        /// <summary>
        /// Compares two address for lexicographical equality.
        /// </summary>
        /// <param name="x">First value to compare.</param>
        /// <param name="y">Second value to compare.</param>
        /// <returns>True if the addresses are equal; otherwise false.</returns>
        public bool Equals(Address x, Address y)
        {
            if (x.Selector.HasValue != y.Selector.HasValue)
                return false;
            if (x.Selector.HasValue)
            {
                Debug.Assert(y.Selector.HasValue);
                if (x.Selector.Value != y.Selector.Value)
                    return false;
            }
            return x.Offset == y.Offset;
        }

        /// <summary>
        /// Computes the lexicographical hash code of an address.
        /// </summary>
        /// <param name="obj">Address whose hash is to be computed.</param>
        /// <returns>The lexicographical hash value of the address.</returns>
        public int GetHashCode(Address obj)
        {
            return HashCode.Combine(obj.Selector, obj.Offset);
        }
    }
}
