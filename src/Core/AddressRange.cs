#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
	/// Describes a memory address range [begin...end)
	/// </summary>
	public class AddressRange
	{
		private Address addrBegin;
		private Address addrEnd;
        private static AddressRange e = new AddressRange(Address.Ptr32(0), Address.Ptr32(0));

		public AddressRange(Address addrBegin, Address addrEnd)
		{
            this.addrBegin = addrBegin ?? throw new ArgumentNullException(nameof(addrBegin));
            this.addrEnd = addrEnd ?? throw new ArgumentNullException(nameof(addrEnd));
        }

        /// <summary>
        /// Gets the beginning address (inclusive) of the memory range.
        /// </summary>
        public Address Begin => addrBegin; 

        /// <summary>
        /// Gets the ending address (exclusive) of the memory range.
        /// </summary>
		public Address End => addrEnd; 

        /// <summary>
        /// Gets a value indicating whether this memory range is valid.
        /// </summary>
        public bool IsValid => this != e; 

        /// <summary>
        /// Gets the empty/null memory range.
        /// </summary>
        public static AddressRange Empty => e; 

    }
}
