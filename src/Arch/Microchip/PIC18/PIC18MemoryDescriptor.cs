#region License
/* 
 * Copyright (C) 2017-2025 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Expressions;
using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC18
{
    using Common;
    using Reko.Core.Expressions;

    /// <summary>
    /// A factory class which implements the PIC18 memory descriptor based on the PIC XML definition.
    /// </summary>
    internal class PIC18MemoryDescriptor : PICMemoryDescriptor
    {

        protected class PIC18MemoryMap : PICMemoryMap
        {
            private const string accessRAMRegionID = "accessram";
            private const string accessSFRRegionID = "accesssfr";
            private const string extendGPRERegionID = "gpre";

            internal IMemoryRegion? AccessRAM;
            internal IMemoryRegion AccessSFR;
            internal IMemoryRegion? ExtendedGPRE;


            /// <summary>
            /// Constructor that prevents a default instance of this class from being created.
            /// </summary>
            private PIC18MemoryMap() : base() 
            {
                AccessRAM = null!;
                AccessSFR = null!;
                ExtendedGPRE = null!;
            }

            /// <summary>
            /// Private constructor creating an instance of memory map for specified PIC.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            protected PIC18MemoryMap(IPICDescriptor thePIC) : base(thePIC)
            {
                AccessRAM = null!;
                AccessSFR = null!;
                ExtendedGPRE = null!;
                SetMaps();
            }

            /// <summary>
            /// Creates a new <see cref="IPICMemoryMap"/> interface for PIC18.
            /// </summary>
            /// <param name="pic">the PIC descriptor.</param>
            /// <returns>
            /// A <see cref="IPICMemoryMap"/> interface instance.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC definition contains an invalid
            ///                                               data memory size (less than 12 bytes).</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC definition does not permit to construct the memory map.</exception>
            public static IPICMemoryMap Create(IPICDescriptor pic)
            {
                if (pic is null)
                    throw new ArgumentNullException(nameof(pic));
                uint datasize = pic.DataSpaceSize;
                if (datasize < MinDataMemorySize)
                    throw new ArgumentOutOfRangeException($"Too low data memory size (less than {MinDataMemorySize} bytes). Check PIC definition.");

                switch (pic.GetInstructionSetID)
                {
                    case InstructionSetID.PIC18:
                    case InstructionSetID.PIC18_EXTENDED:
                    case InstructionSetID.PIC18_ENHANCED:
                        var map = new PIC18MemoryMap(pic);
                        if (!map.IsValid)
                            throw new InvalidOperationException($"Mapper cannot be constructed for '{pic.PICName}' device.");
                        return map;

                    default:
                        throw new InvalidOperationException($"Invalid PIC18 family: '{pic.PICName}'.");
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
                => throw new NotSupportedException("PIC18 has no remapped data region.");

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
                        dataMap = new DataMemoryMap(this, traits, execMode);
                        SetMaps();
                        switch (execMode)
                        {
                            case PICExecMode.Traditional:
                                if (AccessRAM is null)
                                    throw new InvalidOperationException($"Missing '{accessRAMRegionID}' data memory region.");
                                break;
                            case PICExecMode.Extended:
                                if (ExtendedGPRE is null)
                                    throw new InvalidOperationException($"Missing '{extendGPRERegionID}' data memory region.");
                                break;
                        }
                    }
                }
            }
            private PICExecMode execMode = PICExecMode.Traditional;

            /// <summary>
            /// Query if memory banked address <paramref name="bAddr"/> can be a FSR2 index
            /// </summary>
            /// <param name="bAddr">The memory banked address to check.</param>
            public override bool CanBeFSR2IndexAddress(PICBankedAddress bAddr)
                => (ExecMode == PICExecMode.Extended) && 
                bAddr.IsAccessRAMAddr && 
                ExtendedGPRE is not null && 
                ExtendedGPRE.Contains(bAddr.BankOffset);

            /// <summary>
            /// Creates a data memory banked address.
            /// </summary>
            /// <param name="bankSel">The data memory bank selector.</param>
            /// <param name="offset">The offset in the data memory bank.</param>
            /// <param name="access">True if Access addressing mode.</param>
            /// <returns>
            /// The new banked address.
            /// </returns>
            public override PICBankedAddress CreateBankedAddr(Constant bankSel, Constant offset, bool access)
                => new PIC18BankedAddress(bankSel, offset, access);

            public override bool TryGetAbsDataAddress(PICBankedAddress bAddr, out PICDataAddress absAddr)
            {
                if (bAddr is null)
                    throw new ArgumentNullException(nameof(bAddr));
                absAddr = null!;
                IMemoryRegion? regn = null;
                if (bAddr.IsAccessRAMAddr)
                {
                    if (AccessRAM?.Contains(bAddr.ToDataAddress(AccessRAM).ToAddress()) ?? false)
                    {
                        regn = AccessRAM;
                    }
                    else if (AccessSFR?.Contains(bAddr.ToDataAddress(AccessSFR).ToAddress()) ?? false)
                    {
                        regn = AccessSFR;
                    }
                }
                else if (bAddr.BankSelect.IsValid)
                {
                    regn = GetDataRegionBySelector(bAddr.BankSelect);
                }
                if (regn is not null)
                {
                    absAddr = bAddr.ToDataAddress(regn);
                }
                return absAddr is not null;
            }

            #endregion

            private void SetMaps()
            {
                AccessSFR = GetDataRegionByName(accessSFRRegionID) ?? throw new InvalidOperationException($"Missing '{accessSFRRegionID}' data memory region.");
                AccessRAM = GetDataRegionByName(accessRAMRegionID);
                ExtendedGPRE = GetDataRegionByName(extendGPRERegionID);
            }

        }


        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PIC18MemoryDescriptor() : base()
        {
        }

        public static void Create(IPICDescriptor pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            var memdesc = new PIC18MemoryDescriptor();
            memdesc.Reset();
            memdesc.LoadMemDescr(pic);
        }

        protected override IPICMemoryMap CreateMemoryMap(IPICDescriptor pic)
            => PIC18MemoryMap.Create(pic);

    }

}
