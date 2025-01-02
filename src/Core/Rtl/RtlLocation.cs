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

namespace Reko.Core.Rtl
{
    /// <summary>
    /// This class has finer granularity than <see cref="Address"/>,
    /// which is required when dealing with machine instructions that have been
    /// rewritten to multiple RTL instructions.
    /// </summary>
    public readonly struct RtlLocation : IComparable<RtlLocation>
    {
        public RtlLocation(Address addr, int index)
        {
            this.Address = addr;
            this.Index = index;
        }

        public Address Address { get; }
        public int Index { get; }

        public int CompareTo(RtlLocation that)
        {
            int cmp = this.Address.CompareTo(that.Address);
            if (cmp == 0)
            {
                cmp = this.Index - that.Index;
            }
            return cmp;
        }

        public override bool Equals(object? obj)
        {
            if (obj is RtlLocation that)
            {
                return 
                    this.Address == that.Address &&
                    this.Index == that.Index;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Address.GetHashCode() * 5 ^ Index.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Address}.{Index}";
        }

        public static RtlLocation Min(RtlLocation a, RtlLocation b)
        {
            if (a.CompareTo(b) < 0)
                return a;
            else
                return b;
        }

        public static bool operator == (RtlLocation a, RtlLocation b)
        {
            return a.Equals(b);
        }

        public static bool operator != (RtlLocation a, RtlLocation b)
        {
            return !(a.Equals(b));
        }

        public static bool operator < (RtlLocation a, RtlLocation b)
        {
            return a.CompareTo(b) < 0;
        }

        public static bool operator <= (RtlLocation a, RtlLocation b)
        {
            return a.CompareTo(b) <= 0;
        }

        public static bool operator >= (RtlLocation a, RtlLocation b)
        {
            return a.CompareTo(b) >= 0;
        }

        public static bool operator > (RtlLocation a, RtlLocation b)
        {
            return a.CompareTo(b) > 0;
        }

        public static RtlLocation Loc16(ushort uAddr, int index)
        {
            return new RtlLocation(Address.Ptr16(uAddr), index);
        }

        public static RtlLocation Loc32(uint uAddr, int index)
        {
            return new RtlLocation(Address.Ptr32(uAddr), index);
        }

        public static RtlLocation Loc64(ulong uAddr, int index)
        {
            return new RtlLocation(Address.Ptr64(uAddr), index);
        }
    }
}
