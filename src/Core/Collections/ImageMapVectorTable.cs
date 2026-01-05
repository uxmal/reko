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

using System.Collections.Generic;

namespace Reko.Core.Collections
{
    /// <summary>
    /// Represents a table of jumps or calls. Initially, it is empty.
    /// item, but further analysis may grow this size.
    /// </summary>
    public class ImageMapVectorTable : ImageMapItem
    {
        /// <summary>
        /// Constructs an instance of <see cref="ImageMapVectorTable"/> with the specified address and size.
        /// </summary>
        /// <param name="addr">Address of the table.</param>
        /// <param name="vector">Entries in the table.</param>
        /// <param name="size">Size of the backing memory.</param>
        public ImageMapVectorTable(Address addr, Address[] vector, int size)
            : base(addr)
        {
            Addresses = new List<Address>(vector);
            Size = (uint) size;
        }

        /// <summary>
        /// The destinations of the jump table
        /// </summary>
        public List<Address> Addresses { get; private set; }
    }
}