#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
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

using Reko.Libraries.Microchip;
using Reko.Core;
using Reko.Core.Expressions;

namespace Reko.Arch.MicrochipPIC.Common
{

    /// <summary>
    /// The <see cref="IMemoryRegion"/> interface permits to retrieve information for a given PIC memory region (data or program).
    /// </summary>
    public interface IMemoryRegion
    {
        /// <summary>
        /// Gets the unique name of the memory region.
        /// </summary>
        /// <value>
        /// The ID of the memory region as a string.
        /// </value>
        string RegionName { get; }

        /// <summary>
        /// Gets the logical byte addresses range.
        /// </summary>
        AddressRange LogicalByteAddrRange { get; }

        /// <summary>
        /// Gets the corresponding physical byte addresses range.
        /// </summary>
        AddressRange PhysicalByteAddrRange { get; }

        /// <summary>
        /// Gets the memory region total size in bytes.
        /// </summary>
        /// <value>
        /// The size of the memory region in number of bytes.
        /// </value>
        uint Size { get; }

        /// <summary>
        /// Gets the type of the memory region.
        /// </summary>
        /// <value>
        /// A value from <see cref="PICMemoryDomain"/> enumeration.
        /// </value>
        PICMemoryDomain TypeOfMemory { get; }

        /// <summary>
        /// Gets the subtype of the memory region.
        /// </summary>
        /// <value>
        /// A value from <see cref="PICMemorySubDomain"/> enumeration.
        /// </value>
        PICMemorySubDomain SubtypeOfMemory { get; }

        /// <summary>
        /// Gets the memory region traits.
        /// </summary>
        /// <value>
        /// The characteristics of the memory region.
        /// </value>
        IPICMemTrait Trait { get; }

        /// <summary>
        /// Checks whether the given memory fragment is contained in this memory region.
        /// </summary>
        /// <param name="aFragAddr">The starting address of the fragment.</param>
        /// <param name="Len">The length in bytes of the fragment. (default=0)</param>
        /// <returns>
        /// True if the fragment is contained in this memory region, false if not.
        /// </returns>
        bool Contains(Address aFragAddr, uint Len = 0);

        /// <summary>
        /// Checks whether the given memory fragment is contained in this memory region.
        /// </summary>
        /// <param name="cAddr">The starting integer address of the fragment.</param>
        /// <param name="Len">The length in bytes of the fragment. (default=0)</param>
        /// <returns>
        /// True if the fragment is contained in this memory region, false if not.
        /// </returns>
        bool Contains(Constant cAddr, uint Len = 0);

        /// <summary>
        /// Checks whether the given memory fragment is contained in this memory region.
        /// </summary>
        /// <param name="uAddr">The starting integer address of the fragment.</param>
        /// <param name="Len">The length in bytes of the fragment. (default=0)</param>
        /// <returns>
        /// True if the fragment is contained in this memory region, false if not.
        /// </returns>
        bool Contains(uint uAddr, uint Len = 0);

    }
}
