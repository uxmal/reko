#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Core.Collections
{
    public class LexicographicalAddressComparer : IComparer<Address>, IEqualityComparer<Address>
    {
        public int Compare(Address? x, Address? y)
        {
            if (x is null)
                return (y is null) ? 0 : -1;
            else if (y is null)
                return 1;
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

        public bool Equals(Address? x, Address? y)
        {
            if (x is null)
                return y is null;
            else if (y is null)
                return false;
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

        public int GetHashCode([DisallowNull] Address obj)
        {
            return HashCode.Combine(obj.Selector, obj.Offset);
        }
    }
}
