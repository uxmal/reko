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

namespace Reko.Core.Lib
{
    /// <summary>
    /// Represents a semi-open bit range within a <see cref="Storage"/> as 
    /// two numbers.
    /// </summary>
    public readonly struct BitRange : IComparable<BitRange>
    {
        /// <summary>
        /// The empty bit range.
        /// </summary>
        public static readonly BitRange Empty = new(0, 0);

        /// <summary>
        /// Constructs a bit range.
        /// </summary>
        /// <param name="lsb">Inclusive lower endpoint of the range.</param>
        /// <param name="msb">Exclusive upper endpoint of the range.</param>
        public BitRange(int lsb, int msb)
        {
            Lsb = (short) lsb;
            Msb = (short) msb;
        }

        /// <summary>
        /// Inclusive lower endpoint of the range.
        /// </summary>
        public short Lsb { get; }

        /// <summary>
        /// Exclusive upper endpoint of the range.
        /// </summary>
        public short Msb { get; }


        /// <summary>
        /// Get the bitmask corresponding to the bitrange.
        /// </summary>
        /// <returns></returns>
        public ulong BitMask()
        {
            var low = 1ul << Lsb;
            return (Msb >= 64 ? 0ul : 1ul << Msb)
                - low;
        }

        /// <summary>
        /// Returns the extent of the bitrange.
        /// </summary>
        public int Extent
        {
            get { return Math.Max(Msb - Lsb, 0); }
        }

        /// <summary>
        /// True if the bit range is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return Lsb >= Msb; }
        }

        /// <summary>
        /// Compares this bit range to another bit range.
        /// </summary>
        /// <param name="that">The other bit range.</param>
        /// <returns></returns>
        public int CompareTo(BitRange that)
        {
            return Msb - Lsb - (that.Msb - that.Lsb);
        }

        /// <summary>
        /// Determines whether this bitrange covers another bit range.
        /// </summary>
        /// <param name="that">Another bit range.</param>
        /// <returns>Returns true if the other bit range is a subset
        /// of this bit range.</returns>
        public bool Covers(BitRange that)
        {
            return Lsb <= that.Lsb && Msb >= that.Msb;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is BitRange that)
            {
                return Lsb == that.Lsb && Msb == that.Msb;
            }
            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            if (IsEmpty)
                return 0;
            return Lsb.GetHashCode() ^ Msb.GetHashCode() * 5;
        }

        /// <summary>
        /// Returns true if this bit range contains the given bit position.
        /// </summary>
        /// <param name="bitpos">The bit position to test.</param>
        /// <returns>True if the bit range contains the bit position; otherwise false.
        /// </returns>
        public bool Contains(int bitpos)
        {
            return Lsb <= bitpos && bitpos < Msb;
        }

        /// <summary>
        /// Returns the intersection with another bit range.
        /// </summary>
        /// <param name="that">The other bit range.</param>
        /// <returns>The resulting intersection.</returns>
        public BitRange Intersect(BitRange that)
        {
            int lsb = Math.Max(Lsb, that.Lsb);
            int msb = Math.Min(Msb, that.Msb);
            return new BitRange(lsb, msb);
        }

        /// <summary>
        /// Creates a new bit range offset from this one.
        /// </summary>
        /// <param name="offset">Amount to offset.</param>
        /// <returns>Resulting offset image.</returns>
        public BitRange Offset(int offset)
        {
            return new BitRange(Lsb + offset, Msb + offset);
        }

        /// <summary>
        /// Check whether this range overlaps part or all of the bit range
        /// <paramref name="that"/>.
        /// </summary>
        /// <param name="that">Bit range to check.</param>
        /// <returns>True if this bit range overlaps part of <paramref name="that"/>;
        /// otherwise false.</returns>
        public bool Overlaps(BitRange that)
        {
            return that.Lsb < Msb && Lsb < that.Msb;
        }

#pragma warning disable CS1591

        public static BitRange operator |(BitRange a, BitRange b)
        {
            if (a.IsEmpty)
                return b;
            if (b.IsEmpty)
                return a;
            return new BitRange(
                Math.Min(a.Lsb, b.Lsb),
                Math.Max(a.Msb, b.Msb));
        }

        public static BitRange operator &(BitRange a, BitRange b)
        {
            return new BitRange(
                Math.Max(a.Lsb, b.Lsb),
                Math.Min(a.Msb, b.Msb));
        }


        public static BitRange operator -(BitRange a, BitRange b)
        {
            var d = a & b;
            if (d.IsEmpty)
                return a;
            if (d.Lsb == a.Lsb)
            {
                return new BitRange(d.Msb, a.Msb);
            }
            else if (d.Msb == a.Msb)
            {
                return new BitRange(a.Lsb, d.Lsb);
            }
            return a;
        }

        public static BitRange operator <<(BitRange a, int sh)
        {
            if (a.IsEmpty)
                return a;
            return new BitRange(a.Lsb + sh, a.Msb + sh);
        }

        public static bool operator ==(BitRange a, BitRange b)
        {
            return a.Lsb == b.Lsb && a.Msb == b.Msb;
        }

        public static bool operator !=(BitRange a, BitRange b)
        {
            return a.Lsb != b.Lsb || a.Msb != b.Msb;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            if (IsEmpty)
                return "[]";
            else
                return $"[{Lsb}..{Msb - 1}]";
        }
    }
}
