#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work of:
 * Copyright (C) 1999-2017 John Källén.
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
using System.Collections.Generic;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// Interface for implementing PIC memory map.
    /// </summary>
    public interface IPICMemoryMap
    {
        /// <summary>
        /// Gets the target PIC for this memory map.
        /// </summary>
        /// <value>
        /// The target PIC.
        /// </value>
        PIC PIC { get; }

        /// <summary>
        /// Gets the PIC execution mode (set is effective for PIC18 only).
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        PICExecMode ExecMode { get; set; }

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
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="MemorySubDomain"/> enumeration.</param>
        bool HasSubDomain(MemorySubDomain subdom);

        /// <summary>
        /// Gets a data memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        IMemoryRegion GetDataRegion(string sregionName);

        /// <summary>
        /// Gets a data memory region given a memory byte address.
        /// </summary>
        /// <param name="aByteAddr">The data memory byte address.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        IMemoryRegion GetDataRegion(Address aByteAddr);

        /// <summary>
        /// Get a list of data regions.
        /// </summary>
        IReadOnlyList<IMemoryRegion> DataRegionsList { get; }

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        IEnumerable<IMemoryRegion> DataRegions { get; }

        /// <summary>
        /// Gets the data memory Emulator zone.
        /// Valid only if the mapper contains a sub-domain <seealso cref="MemorySubDomain.Emulator"/>.
        /// </summary>
        /// <value>
        /// The emulator zone/region.
        /// </value>
        IMemoryRegion EmulatorZone { get; }

        /// <summary>
        /// Gets the Linear Data Memory definition.
        /// Valid only if the mapper contains a sub-domain <seealso cref="MemorySubDomain.Linear"/>.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        ILinearRegion LinearSector { get; }

        /// <summary>
        /// Gets a program memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        IMemoryRegion GetProgramRegion(string sregionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual byte address.
        /// </summary>
        /// <param name="aVirtByteAddr">The memory byte address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        IMemoryRegion GetProgramRegion(Address aVirtByteAddr);

        /// <summary>
        /// Gets a list of program regions.
        /// </summary>
        IReadOnlyList<IMemoryRegion> ProgramRegionsList { get; }

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        IEnumerable<IMemoryRegion> ProgramRegions { get; }

    }

}
