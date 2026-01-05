#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

using Reko.Core;

namespace Reko.Core
{
    /// <summary>
    /// Represents a hit when searching in a <see cref="Program"/>.
    /// </summary>
    public class AddressSearchHit
    {
        /// <summary>
        /// Constructs an instance of <see cref="AddressSearchHit"/>.
        /// </summary>
        /// <param name="program">Program in which the hit occurred.</param>
        /// <param name="address">Address at which the hit occurred.</param>
        /// <param name="length">The length of the hit object.</param>
        public AddressSearchHit(Program program, Address address, int length)
        {
            this.Program = program;
            this.Address = address;
            this.Length = length;
        }

        /// <summary>
        /// The program in which the hit was found.
        /// </summary>
        public Program Program { get; }

        /// <summary>
        /// The address at which the hit was found.
        /// </summary>
        public Address Address { get; }

        /// <summary>
        /// The length of the hit in storage units.
        /// </summary>
        public int Length { get; }
    }
}
