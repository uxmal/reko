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
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// Interface for implementing PIC memory map modelization.
    /// </summary>
    public interface IPICMemoryMap
    {
        /// <summary>
        /// Gets the target PIC for this memory map.
        /// </summary>
        /// <value>
        /// The target PIC.
        /// </value>
        IPICDescriptor PIC { get; }

        /// <summary>
        /// Gets the PIC execution mode (set is effective for PIC18 only).
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        PICExecMode ExecMode { get; set; }

        /// <summary>
        /// Gets a value indicating whether this map is valid. Meaning it contains all the mandatory information.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Gets the instruction set identifier of the target PIC.
        /// </summary>
        /// <value>
        /// A value from the <see cref="InstructionSetID"/> enumeration.
        /// </value>
        InstructionSetID InstructionSetID { get; }

        /// <summary>
        /// Query if the memory mapper has a region of given sub-domain type.
        /// </summary>
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="PICMemorySubDomain"/> enumeration.</param>
        bool HasSubDomain(PICMemorySubDomain subdom);

        /// <summary>
        /// Memory sub-domain location and word sizes.
        /// </summary>
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="PICMemorySubDomain"/>
        ///                      enumeration.</param>
        /// <returns>
        /// A Tuple containing the location size and wordsize. Returns (0,0) if the subdomain does not exist.
        /// </returns>
        (uint LocSize, uint WordSize) SubDomainSizes(PICMemorySubDomain subdom);

        /// <summary>
        /// Gets a program memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        IMemoryRegion GetProgramRegionByName(string sregionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual byte address.
        /// </summary>
        /// <param name="absByteAddr">The memory byte address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        IMemoryRegion GetProgramRegionByAddress(Address absByteAddr);

        /// <summary>
        /// Gets a data memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        IMemoryRegion GetDataRegionByName(string sregionName);

        /// <summary>
        /// Gets a data memory region given a memory byte address.
        /// </summary>
        /// <param name="absByteAddr">The data memory byte address.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        IMemoryRegion GetDataRegionByAddress(Address absByteAddr);

        /// <summary>
        /// Gets a data memory region given a bank selector.
        /// </summary>
        /// <param name="bankSel">The data memory bank selector.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        IMemoryRegion GetDataRegionBySelector(Constant bankSel);

        /// <summary>
        /// Gets a list of program regions.
        /// </summary>
        IReadOnlyList<IMemoryRegion> ProgramRegionsList { get; }

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        IEnumerable<IMemoryRegion> ProgramRegions { get; }

        /// <summary>
        /// Get a list of data regions.
        /// </summary>
        IReadOnlyList<IMemoryRegion> DataRegionsList { get; }

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        IEnumerable<IMemoryRegion> DataRegions { get; }

        /// <summary>
        /// Gets the Linear Data Memory definition.
        /// Valid only if the mapper contains a sub-domain <seealso cref="PICMemorySubDomain.Linear"/>.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        ILinearRegion LinearSector { get; }

        /// <summary>
        /// Remap an absolute data memory address.
        /// </summary>
        /// <param name="absAddr">The absolute data memory address.</param>
        /// <returns>
        /// The actual physical memory address.
        /// </returns>
        PICDataAddress RemapDataAddress(PICDataAddress absAddr);

        /// <summary>
        /// Try to translate a data memory banked address to an absolute data meory address.
        /// </summary>
        /// <param name="bAddr">The memory banked address to translate.</param>
        /// <param name="absAddr">[out] The resulting absolute data memory address.</param>
        /// <returns>
        /// True if successfully translated banked address to absolute address.
        /// </returns>
        bool TryGetAbsDataAddress(PICBankedAddress bAddr, out PICDataAddress absAddr);

        /// <summary>
        /// Query if memory banked address <paramref name="bAddr"/> can be a FSR2 index
        /// </summary>
        /// <param name="bAddr">The memory banked address to check.</param>
        bool CanBeFSR2IndexAddress(PICBankedAddress bAddr);

        /// <summary>
        /// Creates data memory banked address.
        /// </summary>
        /// <param name="bankSel">The data memory bank selector.</param>
        /// <param name="offset">The offset in the bank.</param>
        /// <param name="access">True if Access Mode adressing.</param>
        /// <returns>
        /// The new banked address.
        /// </returns>
        PICBankedAddress CreateBankedAddr(Constant bankSel, Constant offset, bool access);

    }

}
