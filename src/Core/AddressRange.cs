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

namespace Reko.Core
{
	/// <summary>
	/// Describes a half-open memory address range [begin...end)
	/// </summary>
	public class AddressRange
	{
        /// <summary>
        /// Creates an instance of the <see cref="AddressRange"/> class.
        /// </summary>
        /// <param name="addrBegin">The inclusive beginning of the range.</param>
        /// <param name="addrEnd">The exclusing end of the range.</param>
		public AddressRange(Address addrBegin, Address addrEnd)
		{
            this.Begin = addrBegin;
            this.End = addrEnd;
        }

        /// <summary>
        /// Gets the beginning address (inclusive) of the memory range.
        /// </summary>
        public Address Begin { get; }

        /// <summary>
        /// Gets the ending address (exclusive) of the memory range.
        /// </summary>
		public Address End { get; }

        /// <summary>
        /// Gets a value indicating whether this memory range is valid.
        /// </summary>
        public bool IsValid => this.Begin <= this.End; 

        /// <summary>
        /// The length of this address range in storage units.
        /// </summary>
        public long Length => this.End - this.Begin;

        /// <summary>
        /// Compares two address ranges for equality.
        /// </summary>
        /// <param name="left">One address range</param>
        /// <param name="right">Another address range.</param>
        /// <returns>True if the ranges are equal.</returns>
        public static bool operator == (AddressRange left, AddressRange right)
        {
            return left.Begin == right.Begin && left.End == right.End;
        }

        /// <summary>
        /// Compares two address ranges for inequality.
        /// </summary>
        /// <param name="left">One address range</param>
        /// <param name="right">Another address range.</param>
        /// <returns>True if the ranges are not equal.</returns>
        public static bool operator != (AddressRange left, AddressRange right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is AddressRange that && this == that;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(Begin, End);

        /// <summary>
        /// Gets the empty/null memory range.
        /// </summary>
        public static AddressRange Empty { get; } = new AddressRange(Address.Ptr32(1), Address.Ptr32(0)); 
    }
}
