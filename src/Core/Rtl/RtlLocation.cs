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
        /// <summary>
        /// Constructs an instance of <see cref="RtlLocation"/>.
        /// </summary>
        /// <param name="addr">The address of the location.</param>
        /// <param name="index">The index within the location.</param>
        public RtlLocation(Address addr, int index)
        {
            this.Address = addr;
            this.Index = index;
        }

        /// <summary>
        /// The address of the location.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// The index within the location.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Compares this instance with another <see cref="RtlLocation"/> and returns an integer that indicates
        /// whether this instance precedes, follows, or occurs in the same position in the sort order as the other instance.
        /// </summary>
        /// <param name="that">Other instance.</param>
        public int CompareTo(RtlLocation that)
        {
            int cmp = this.Address.CompareTo(that.Address);
            if (cmp == 0)
            {
                cmp = this.Index - that.Index;
            }
            return cmp;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Address.GetHashCode() * 5 ^ Index.GetHashCode();
        }

        /// <summary>
        /// Returns a string representation of the <see cref="RtlLocation"/> instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Address}.{Index}";
        }

        /// <summary>
        /// Computes the minimum of two <see cref="RtlLocation"/> instances.
        /// </summary>
        /// <param name="a">First instance.</param>
        /// <param name="b">Second instance.</param>
        public static RtlLocation Min(RtlLocation a, RtlLocation b)
        {
            if (a.CompareTo(b) < 0)
                return a;
            else
                return b;
        }

#pragma warning disable CS1591

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

#pragma warning restore CS1591

        /// <summary>
        /// Creates a <see cref="RtlLocation"/> instance from a 16-bit address and an index.
        /// </summary>
        /// <param name="uAddr">Address.</param>
        /// <param name="index">Index.</param>
        public static RtlLocation Loc16(ushort uAddr, int index)
        {
            return new RtlLocation(Address.Ptr16(uAddr), index);
        }

        /// <summary>
        /// Creates a <see cref="RtlLocation"/> instance from a 16-bit address and an index.
        /// </summary>
        /// <param name="uAddr">Address.</param>
        /// <param name="index">Index.</param>
        public static RtlLocation Loc32(uint uAddr, int index)
        {
            return new RtlLocation(Address.Ptr32(uAddr), index);
        }

        /// <summary>
        /// Creates a <see cref="RtlLocation"/> instance from a 16-bit address and an index.
        /// </summary>
        /// <param name="uAddr">Address.</param>
        /// <param name="index">Index.</param>
        public static RtlLocation Loc64(ulong uAddr, int index)
        {
            return new RtlLocation(Address.Ptr64(uAddr), index);
        }
    }
}
