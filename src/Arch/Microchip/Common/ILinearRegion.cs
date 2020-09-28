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

using Reko.Core;
using Reko.Libraries.Microchip;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// The <see cref="ILinearRegion"/> interface permits to retrieve information for the PIC Linear Access data memory region.
    /// </summary>
    public interface ILinearRegion
    {
        /// <summary>
        /// Gets the FSR byte address indirect range of the linear data memory region.
        /// </summary>
        /// <value>
        /// A tuple providing the start and end+1 virtual byte addresses of the data memory region.
        /// </value>
        AddressRange FSRByteAddress { get; }

        /// <summary>
        /// Gets the byte size of GPR memory banks.
        /// </summary>
        /// <value>
        /// The size of the memory banks in number of bytes.
        /// </value>
        int BankSize { get; }

        /// <summary>
        /// Gets the block byte range visible via the linear data region.
        /// </summary>
        /// <value>
        /// The addresses tuple (start, end) representing the GPR block range.
        /// </value>
        AddressRange BlockByteRange { get; }

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
        /// Gets the memory characteristics of the linear data memory region.
        /// </summary>
        /// <value>
        /// The characteristics.
        /// </value>
        IPICMemTrait Trait { get; }

        /// <summary>
        /// Gets the size in bytes of the linear data memory region.
        /// </summary>
        /// <value>
        /// The size.
        /// </value>
        int Size { get; }

        /// <summary>
        /// Remaps a FSR indirect address in the linear data region address to the corresponding GPR memory address.
        /// </summary>
        /// <param name="aFSRAddr">The linear data memory byte address.</param>
        /// <returns>
        /// The GPR data memory address or NOPHYSICAL_MEM.
        /// </returns>
        Address RemapAddress(Address aFSRAddr);

        /// <summary>
        /// Remap a FSR indirect address in linear data region address to the corresponding GPR bank number and
        /// offset.
        /// </summary>
        /// <param name="aFSRVirtAddr">The virtual data memory byte address.</param>
        /// <param name="gprBank">[out] The GPR bank number and offset.</param>
        /// <returns>
        /// True if it succeeds, false if it fails.
        /// </returns>
        bool RemapFSRIndirect(Address aFSRVirtAddr, out (byte BankNum, uint BankOffset) gprBank );

    }

}
