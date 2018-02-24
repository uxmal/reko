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
        IReadOnlyList<IMemoryRegion> DataRegions { get; }

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        IEnumerable<IMemoryRegion> GetDataRegions { get; }

        /// <summary>
        /// Gets the data memory Emulator zone. Valid only if <seealso cref="HasEmulatorZone"/> is true.
        /// </summary>
        /// <value>
        /// The emulator zone/region.
        /// </value>
        IMemoryRegion EmulatorZone { get; }

        /// <summary>
        /// Gets the Linear Data Memory definition. Valid only if <seealso cref="HasLinear"/> is true.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        ILinearRegion LinearSector { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more SFR (Special Functions Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more SFR data sectors, false if not.
        /// </value>
        bool HasSFR { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more GPR (General Purpose Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more GPR data sectors, false if not.
        /// </value>
        bool HasGPR { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more DPR (Dual-Port Register) data sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more DPR data sectors, false if not.
        /// </value>
        bool HasDPR { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more NMMR (Non-Memory-Mapped Register) definitions.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more NMMR (Non-Memory-Mapped Register) definitions, false if not.
        /// </value>
        bool HasNMMR { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a zone reserved for Emulator.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a zone reserved for Emulator, false if not.
        /// </value>
        bool HasEmulatorZone { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Linear Access data sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Linear Access data sector, false if not.
        /// </value>
        bool HasLinear { get; }

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
        IReadOnlyList<IMemoryRegion> ProgramRegions { get; }

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        IEnumerable<IMemoryRegion> GetProgramRegions { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Debugger program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Debugger program sectors, false if not.
        /// </value>
        bool HasDebug { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more Code program sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more Code program sectors, false if not.
        /// </value>
        bool HasCode { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has one or more External Code program sectors.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has one or more External Code program sectors, false if not.
        /// </value>
        bool HasExtCode { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Calibration program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Calibration program sector, false if not.
        /// </value>
        bool HasCalibration { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Configuration Fuses program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Configuration Fuses program sector, false if not.
        /// </value>
        bool HasConfigFuse { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device ID program sector, false if not.
        /// </value>
        bool HasDeviceID { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Data EEPROM program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Data EEPROM program sector, false if not.
        /// </value>
        bool HasEEData { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a User ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a User ID program sector, false if not.
        /// </value>
        bool HasUserID { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Revision ID program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Revision ID program sector, false if not.
        /// </value>
        bool HasRevisionID { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device Information Area program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device Information Area program sector, false if not.
        /// </value>
        bool HasDIA { get; }

        /// <summary>
        /// Gets a value indicating whether this PIC memory map has a Device Configuration Information program sector.
        /// </summary>
        /// <value>
        /// True if this PIC memory map has a Device Configuration Information program sector, false if not.
        /// </value>
        bool HasDCI { get; }

    }

}
