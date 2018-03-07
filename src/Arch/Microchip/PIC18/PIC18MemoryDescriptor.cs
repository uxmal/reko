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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    /// <summary>
    /// A factory class which implements the PIC18 memory descriptor based on the PIC XML definition.
    /// </summary>
    public class PIC18MemoryDescriptor : IPICMemoryDescriptor
    {

        #region Locals

        private static PIC18MemoryDescriptor memDescr;

        #endregion

        #region Inner classes

        public class PIC18MemoryMap : MemoryMap
        {
            #region Locals

            private const string accessRAMRegionID = "accessram";
            private const string extendRAMRegionID = "gpre";
            private const string accessSFRRegionID = "accesssfr";

            internal IMemoryRegion AccessRAMLow;
            internal IMemoryRegion AccessRAMHigh;
            internal static Address topAccessRAM = Address.Ptr32(0x100);

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor that prevents a default instance of this class from being created.
            /// </summary>
            private PIC18MemoryMap() : base()
            {
            }

            /// <summary>
            /// Private constructor creating an instance of memory map for specified PIC.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            protected PIC18MemoryMap(PIC thePIC) : base(thePIC)
            {
                AccessRAMHigh = GetDataRegion(accessSFRRegionID);
                if (AccessRAMHigh is null)
                    throw new InvalidOperationException($"Missing '{accessSFRRegionID}' data memory region.");
                AccessRAMLow = GetDataRegion(accessRAMRegionID);
                if (AccessRAMLow is null)
                {
                    AccessRAMLow = GetDataRegion(extendRAMRegionID);
                    if (AccessRAMLow is null)
                        throw new InvalidOperationException($"Missing either '{accessRAMRegionID}' or '{extendRAMRegionID}' data memory region.");
                }
            }

            /// <summary>
            /// Creates a new <see cref="PIC18MemoryMap"/> instance.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            /// <returns>
            /// A <see cref="PIC18MemoryMap"/> instance.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="thePIC"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC definition contains an invalid
            ///                                               data memory size (less than 12 bytes).</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC definition does not permit to construct the memory map.</exception>
            public static PIC18MemoryMap Create(PIC thePIC)
            {
                if (thePIC is null)
                    throw new ArgumentNullException(nameof(thePIC));
                uint datasize = thePIC.DataSpace?.EndAddr ?? 0;
                if (datasize < MinDataMemorySize)
                    throw new ArgumentOutOfRangeException($"Too low data memory size (less than {MinDataMemorySize} bytes). Check PIC definition.");

                switch (thePIC.GetInstructionSetID)
                {
                    case InstructionSetID.PIC18:
                    case InstructionSetID.PIC18_EXTENDED:
                    case InstructionSetID.PIC18_ENHANCED:
                        var map = new PIC18MemoryMap(thePIC);
                        if (!map.IsValid)
                            throw new InvalidOperationException($"Mapper cannot be constructed for '{thePIC.Name}' device.");
                        return map;

                    default:
                        throw new InvalidOperationException($"Invalid PIC18 family: '{thePIC.Name}'.");
                }
            }

            #endregion

            #region MemoryMap implementation

            /// <summary>
            /// Remap a data memory address.
            /// </summary>
            /// <param name="lAddr">The logical memory address.</param>
            /// <returns>
            /// The physical memory address.
            /// </returns>
            public override PICDataAddress RemapDataAddress(PICDataAddress lAddr)
            {
                var addr = Address.Ptr32(lAddr.ToUInt32());
                if ((addr < AccessRAMLow.LogicalByteAddrRange.End || addr > topAccessRAM))
                    return lAddr;
                ulong uaddr = lAddr.ToLinear();
                uaddr -= AccessRAMLow.LogicalByteAddrRange.End.ToLinear();
                uaddr += AccessRAMHigh.LogicalByteAddrRange.Begin.ToLinear();
                return PICDataAddress.Ptr((uint)uaddr);
            }

            /// <summary>
            /// Gets or sets the PIC execution mode.
            /// </summary>
            /// <value>
            /// The PIC execution mode.
            /// </value>
            public override PICExecMode ExecMode
            {
                get => execMode;
                set
                {
                    if (InstructionSetID == InstructionSetID.PIC18)
                        value = PICExecMode.Traditional;
                    if (value != execMode)
                    {
                        execMode = value;
                        dataMap = new DataMemoryMap(PIC, this, traits, execMode);
                        switch (execMode)
                        {
                            case PICExecMode.Traditional:
                                AccessRAMLow = GetDataRegion(accessRAMRegionID);
                                break;

                            case PICExecMode.Extended:
                                AccessRAMLow = GetDataRegion(extendRAMRegionID);
                                break;
                        }
                        if (AccessRAMLow is null)
                            throw new InvalidOperationException($"Missing either '{accessRAMRegionID}' or '{extendRAMRegionID}' data memory region.");
                    }
                }
            }
            private PICExecMode execMode = PICExecMode.Traditional;

            #endregion

            #region Helpers

            /// <summary>
            /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
            /// </summary>
            /// <param name="cAddr">The memory address to check.</param>
            /// <returns>
            /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
            /// </returns>
            internal bool IsAccessRAMLow(Address cAddr) => AccessRAMLow.Contains(cAddr);

            /// <summary>
            /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
            /// </summary>
            /// <param name="uAddr">The memory address to check.</param>
            /// <returns>
            /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
            /// </returns>
            public bool IsAccessRAMHigh(Address uAddr)
            {
                var addr = Address.Ptr32(uAddr.ToUInt32());
                return (addr >= AccessRAMLow.LogicalByteAddrRange.End && addr < topAccessRAM);
            }

            #endregion

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PIC18MemoryDescriptor()
        {
        }

        /// <summary>
        /// Private constructor creating an instance of memory descriptor for specified PIC.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        private PIC18MemoryDescriptor(PIC pic)
        {
            memoryMap = PIC18MemoryMap.Create(pic);
            DeviceConfigDefinitions = DeviceConfigDefs.Create(pic);
        }

        /// <summary>
        /// Creates a new PICMemoryDefinitions.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        /// <returns>
        /// A <see cref="IPICMemoryDescriptor"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
        public static PIC18MemoryDescriptor Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            memDescr = new PIC18MemoryDescriptor(pic);
            return memDescr;
        }

        #endregion

        #region IPICMemoryDescriptor interface

        /// <summary>
        /// The memory description associated with this memory descriptor.
        /// </summary>
        public IMemoryMap MemoryMap => memoryMap;
        private PIC18MemoryMap memoryMap;

        public IDeviceConfigDefs DeviceConfigDefinitions { get; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        public PICExecMode ExecMode
        {
            get => MemoryMap.ExecMode;
            set => MemoryMap.ExecMode = value;
        }

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public bool IsAccessRAMLow(PICDataAddress cAddr) => memoryMap.IsAccessRAMLow(cAddr);

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public bool IsAccessRAMHigh(PICDataAddress uAddr) => memoryMap.IsAccessRAMHigh(uAddr);

        #endregion

        #region Internal API

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="addr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        internal static PICDataAddress RemapDataAddress(PICDataAddress addr) => memDescr?.MemoryMap.RemapDataAddress(addr);

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        internal static PICDataAddress RemapDataAddress(uint uAddr) => memDescr?.MemoryMap.RemapDataAddress(PICDataAddress.Ptr(uAddr));

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> can be a FSR2 index
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        internal static bool CanBeFSR2IndexAddress(ushort uAddr) => memDescr?.memoryMap.IsAccessRAMLow(PICDataAddress.Ptr(uAddr)) ?? false;

        #endregion

    }

}
