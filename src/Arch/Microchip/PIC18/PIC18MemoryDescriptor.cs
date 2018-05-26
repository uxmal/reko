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

using Reko.Core;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;

    /// <summary>
    /// A factory class which implements the PIC18 memory descriptor based on the PIC XML definition.
    /// </summary>
    internal class PIC18MemoryDescriptor : PICMemoryDescriptor
    {

        public class PIC18MemoryMap : MemoryMap
        {

            private const string accessRAMRegionID = "accessram";
            private const string accessSFRRegionID = "accesssfr";
            private const string extendGPRERegionID = "gpre";

            internal IMemoryRegion AccessRAM;
            internal IMemoryRegion AccessSFR;
            internal IMemoryRegion ExtendedGPRE;
            internal static Address topAccessRAM = Address.Ptr32(0x100);


            /// <summary>
            /// Constructor that prevents a default instance of this class from being created.
            /// </summary>
            private PIC18MemoryMap() : base() { }

            /// <summary>
            /// Private constructor creating an instance of memory map for specified PIC.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            protected PIC18MemoryMap(PIC thePIC) : base(thePIC)
            {
                AccessSFR = GetDataRegion(accessSFRRegionID) ?? throw new InvalidOperationException($"Missing '{accessSFRRegionID}' data memory region.");
                AccessRAM = GetDataRegion(accessRAMRegionID) ?? throw new InvalidOperationException($"Missing '{accessRAMRegionID}' data memory region.");
                ExtendedGPRE = GetDataRegion(extendGPRERegionID);
            }

            /// <summary>
            /// Creates a new <see cref="IMemoryMap"/> interface for PIC18.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            /// <returns>
            /// A <see cref="IMemoryMap"/> interface instance.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="thePIC"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC definition contains an invalid
            ///                                               data memory size (less than 12 bytes).</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC definition does not permit to construct the memory map.</exception>
            public static IMemoryMap Create(PIC thePIC)
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
                if (!IsAccessRAMHigh(lAddr))
                    return lAddr;
                ulong uaddr = lAddr.ToLinear();
                uaddr -= AccessRAM.LogicalByteAddrRange.End.ToLinear();
                uaddr += AccessSFR.LogicalByteAddrRange.Begin.ToLinear();
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
                                AccessRAM = GetDataRegion(accessRAMRegionID) ?? throw new InvalidOperationException($"Missing '{accessRAMRegionID}' data memory region.");
                                ExtendedGPRE = null;
                                break;

                            case PICExecMode.Extended:
                                AccessRAM = GetDataRegion(accessRAMRegionID);
                                ExtendedGPRE = GetDataRegion(extendGPRERegionID) ?? throw new InvalidOperationException($"Missing '{extendGPRERegionID}' data memory region.");
                                break;
                        }
                    }
                }
            }
            private PICExecMode execMode = PICExecMode.Traditional;

            /// <summary>
            /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
            /// </summary>
            /// <param name="cAddr">The memory address to check.</param>
            /// <returns>
            /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
            /// </returns>
            public override bool IsAccessRAMLow(PICDataAddress cAddr) => AccessRAM.Contains(cAddr);

            /// <summary>
            /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
            /// </summary>
            /// <param name="uAddr">The memory address to check.</param>
            /// <returns>
            /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
            /// </returns>
            public override bool IsAccessRAMHigh(PICDataAddress uAddr)
            {
                var addr = Address.Ptr32(uAddr.ToUInt32());
                return (addr >= AccessRAM.LogicalByteAddrRange.End && addr < topAccessRAM);
            }

            /// <summary>
            /// Query if memory address <paramref name="uAddr"/> can be a FSR2 index
            /// </summary>
            /// <param name="uAddr">The memory address to check.</param>
            public override bool CanBeFSR2IndexAddress(ushort uAddr) => IsAccessRAMLow(PICDataAddress.Ptr(uAddr));

            #endregion

        }


        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PIC18MemoryDescriptor(PIC pic) : base(pic)
        {
        }

        public static void Create(PIC pic)
        {
            var memdesc = new PIC18MemoryDescriptor(pic ?? throw new ArgumentNullException(nameof(pic)));
            memdesc.Reset();
            memdesc.LoadMemDescr();
        }

        protected override IMemoryMap CreateMemoryMap()
            => PIC18MemoryMap.Create(pic);

    }

}
