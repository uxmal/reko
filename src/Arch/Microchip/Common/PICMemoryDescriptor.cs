#region License
/* 
 * Copyright (C) 2017-2018 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2018 John Källén.
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
using System;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.Common
{
    /// <summary>
    /// This class provides information on the PIC memory.
    /// </summary>
    public class PICMemoryDescriptor
    {

        protected PIC pic;
        private static IPICDeviceConfigDefs deviceConfigDefinitions = null;
        private static IMemoryMap memoryMap = null;

        protected PICMemoryDescriptor(PIC pic)
        {
            this.pic = pic;
        }

        protected virtual IMemoryMap CreateMemoryMap()
            => throw new NotImplementedException("Missing PIC specific memory map creator.");


        public static bool IsValid => ((deviceConfigDefinitions != null) && (memoryMap?.IsValid ?? false));

        public static PICExecMode ExecMode
        {
            get => memoryMap?.ExecMode ?? PICExecMode.Traditional;
            set => memoryMap.ExecMode = value;
        }

        /// <summary>
        /// Gets the instruction set identifier of the target PIC.
        /// </summary>
        /// <value>
        /// A value from the <see cref="InstructionSetID"/> enumeration.
        /// </value>
        public static InstructionSetID InstructionSetID => memoryMap?.InstructionSetID ?? InstructionSetID.UNDEFINED;

        /// <summary>
        /// Query if the memory mapper has a region of given sub-domain type.
        /// </summary>
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="MemorySubDomain"/> enumeration.</param>
        public static bool HasSubDomain(MemorySubDomain subdom) => memoryMap?.HasSubDomain(subdom) ?? false;

        /// <summary>
        /// Memory sub-domain location and word sizes.
        /// </summary>
        /// <param name="subdom">The sub-domain of interest. A value from <see cref="MemorySubDomain"/>
        ///                      enumeration.</param>
        /// <returns>
        /// A Tuple containing the location size and wordsize. Returns (0,0) if the subdomain does not exist.
        /// </returns>
        public static (uint LocSize, uint WordSize) SubDomainSizes(MemorySubDomain subdom) => memoryMap?.SubDomainSizes(subdom)?? (0,0);

        /// <summary>
        /// Gets a data memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public static IMemoryRegion GetDataRegion(string sregionName) => memoryMap?.GetDataRegion(sregionName);

        /// <summary>
        /// Gets a data memory region given a memory byte address.
        /// </summary>
        /// <param name="aByteAddr">The data memory byte address.</param>
        /// <returns>
        /// The data memory region or null.
        /// </returns>
        public static IMemoryRegion GetDataRegion(Address aByteAddr) => memoryMap?.GetDataRegion(aByteAddr);

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool IsAccessRAMLow(PICDataAddress cAddr) => memoryMap?.IsAccessRAMLow(cAddr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        static bool IsAccessRAMHigh(PICDataAddress uAddr) => memoryMap?.IsAccessRAMHigh(uAddr) ?? false;

        /// <summary>
        /// Get a list of data regions.
        /// </summary>
        public static IReadOnlyList<IMemoryRegion> DataRegionsList => memoryMap?.DataRegionsList;

        /// <summary>
        /// Enumerates the data regions.
        /// </summary>
        public static IEnumerable<IMemoryRegion> DataRegions => memoryMap?.DataRegions;

        /// <summary>
        /// Gets the data memory Emulator zone.
        /// Valid only if the mapper contains a sub-domain <seealso cref="MemorySubDomain.Emulator"/>.
        /// </summary>
        /// <value>
        /// The emulator zone/region.
        /// </value>
        public static IMemoryRegion EmulatorZone => memoryMap?.EmulatorZone;

        /// <summary>
        /// Gets the Linear Data Memory definition.
        /// Valid only if the mapper contains a sub-domain <seealso cref="MemorySubDomain.Linear"/>.
        /// </summary>
        /// <value>
        /// The Linear Data Memory region.
        /// </value>
        public static ILinearRegion LinearSector => memoryMap?.LinearSector;

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="addr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static PICDataAddress RemapDataAddress(PICDataAddress addr) => memoryMap?.RemapDataAddress(addr);

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static PICDataAddress RemapDataAddress(uint uAddr) => RemapDataAddress(PICDataAddress.Ptr(uAddr));

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> can be a FSR2 index
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        public static bool CanBeFSR2IndexAddress(ushort uAddr) => memoryMap?.CanBeFSR2IndexAddress(uAddr) ?? false;

        /// <summary>
        /// Gets a program memory region given its name ID.
        /// </summary>
        /// <param name="sregionName">Name ID of the memory region.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public static IMemoryRegion GetProgramRegion(string sregionName) => memoryMap?.GetProgramRegion(sregionName);

        /// <summary>
        /// Gets a program memory region given a memory virtual byte address.
        /// </summary>
        /// <param name="aVirtByteAddr">The memory byte address.</param>
        /// <returns>
        /// The program memory region.
        /// </returns>
        public static IMemoryRegion GetProgramRegion(Address aVirtByteAddr) => memoryMap?.GetProgramRegion(aVirtByteAddr);

        /// <summary>
        /// Gets a list of program regions.
        /// </summary>
        public static IReadOnlyList<IMemoryRegion> ProgramRegionsList => memoryMap?.ProgramRegionsList;

        /// <summary>
        /// Enumerates the program regions.
        /// </summary>
        public static IEnumerable<IMemoryRegion> ProgramRegions => memoryMap?.ProgramRegions;

        /// <summary>
        /// Remap a program memory address.
        /// </summary>
        /// <param name="lAddr">The logical memory address.</param>
        /// <returns>
        /// The physical memory address.
        /// </returns>
        public static PICProgAddress RemapProgramAddress(PICProgAddress lAddr) => memoryMap?.RemapProgramAddress(lAddr);

        /// <summary>
        /// Gets a Device Configuration Register by its name.
        /// </summary>
        /// <param name="name">The name of the register.</param>
        /// <returns>
        /// A <see cref="PICDevConfigRegister"/> instance or null.
        /// </returns>
        public static PICDevConfigRegister GetDCR(string name) => deviceConfigDefinitions?.GetDCR(name);

        /// <summary>
        /// Gets a Device Configuration Register by its memory address.
        /// </summary>
        /// <param name="addr">The memory address.</param>
        /// <returns>
        /// A <see cref="PICDevConfigRegister"/> instance or null.
        /// </returns>
        public static PICDevConfigRegister GetDCR(Address addr) => deviceConfigDefinitions?.GetDCR(addr);

        /// <summary>
        /// Gets a Device Configuration Field by its name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns>
        /// A <see cref="DevConfigField"/> instance or null.
        /// </returns>
        public static DevConfigField GetDCRField(string name) => deviceConfigDefinitions?.GetDCRField(name);

        /// <summary>
        /// Renders the Device Configuration Register state given its value.
        /// </summary>
        /// <param name="dcr">The Device Configuration Register of interest.</param>
        /// <param name="value">The value assigned to this register.</param>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public static string RenderDeviceConfigRegister(PICDevConfigRegister dcr, int value) => deviceConfigDefinitions.Render(dcr, value);

        /// <summary>
        /// Renders the Device Configuration Register state at given address for given value.
        /// </summary>
        /// <param name="addr">The Device Configuration Register memory address.</param>
        /// <param name="value">The value assigned to this register.</param>
        /// <returns>
        /// A human-readable string.
        /// </returns>
        public static string RenderDeviceConfigRegister(Address addr, int value) => deviceConfigDefinitions?.Render(addr, value);


        protected void Reset()
        {
            deviceConfigDefinitions = null;
            memoryMap = null;
        }

        protected void LoadMemDescr()
        {
            deviceConfigDefinitions = PICDeviceConfigDefs.Create(pic);
            memoryMap = CreateMemoryMap();
        }

    }

}
