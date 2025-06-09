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

using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;
    using Reko.Core.Expressions;

    internal class PIC16MemoryDescriptor : PICMemoryDescriptor
    {

        public class PIC16MemoryMap : PICMemoryMap
        {

            /// <summary>
            /// Constructor that prevents a default instance of this class from being created.
            /// </summary>
            private PIC16MemoryMap() : base()
            {
            }

            /// <summary>
            /// Creates a new instance of PIC16 memory map for specified PIC.
            /// </summary>
            /// <param name="pic">the PIC descriptor.</param>
            protected PIC16MemoryMap(IPICDescriptor pic) : base(pic)
            {
            }

            /// <summary>
            /// Creates a new <see cref="IPICMemoryMap"/> interface for PIC16.
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
                    case InstructionSetID.PIC16:
                    case InstructionSetID.PIC16_ENHANCED:
                    case InstructionSetID.PIC16_FULLFEATURED:
                        var map = new PIC16MemoryMap(pic);
                        if (!map.IsValid)
                            throw new InvalidOperationException($"Mapper cannot be constructed for '{pic.PICName}' device.");
                        return map;

                    default:
                        throw new InvalidOperationException($"Invalid PIC16 family: '{pic.PICName}'.");
                }
            }

            #region MemoryMap implementation

            /// <summary>
            /// Gets or sets the PIC execution mode.
            /// </summary>
            /// <value>
            /// The PIC execution mode.
            /// </value>
            public override PICExecMode ExecMode
            {
                get => PICExecMode.Traditional;
                set => dataMap = new DataMemoryMap(this, traits, PICExecMode.Traditional);
            }

            public override PICDataAddress RemapDataAddress(PICDataAddress lAddr)
            {
                ushort uAddr = lAddr?.ToUInt16() ?? throw new ArgumentNullException(nameof(lAddr));
                if (uAddr >= dataMap.remapTable.Length)
                    return lAddr;
                var mAddr = dataMap.remapTable[uAddr];
                return (mAddr is null ? lAddr : PICDataAddress.Ptr(mAddr.Value));
            }

            /// <summary>
            /// Query if memory banked address <paramref name="bAddr"/> can be a FSR2 index.
            /// </summary>
            /// <param name="bAddr">The memory banked address to check.</param>
            public override bool CanBeFSR2IndexAddress(PICBankedAddress bAddr) => false;

            /// <summary>
            /// Creates a data memory banked address.
            /// </summary>
            /// <param name="bankSel">The data memory bank selector.</param>
            /// <param name="offset">The offset in the data memory bank.</param>
            /// <param name="access">Ignored.</param>
            /// <returns>
            /// The new banked address.
            /// </returns>
            public override PICBankedAddress CreateBankedAddr(Constant bankSel, Constant offset, bool access)
                => new PIC16BankedAddress(bankSel, offset);

            public override bool TryGetAbsDataAddress(PICBankedAddress bAddr, out PICDataAddress absAddr)
            {
                if (bAddr is null)
                    throw new ArgumentNullException(nameof(bAddr));
                absAddr = null!;
                IMemoryRegion? regn = null;
                if (PICRegisters.TryGetAlwaysAccessibleRegister(bAddr, out var reg))
                {
                    regn = PICMemoryDescriptor.GetDataRegionByAddress(reg!.Traits.RegAddress.Addr!.ToAddress());
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

        }


        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PIC16MemoryDescriptor() : base()
        {
        }

        public static void Create(IPICDescriptor pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            var memdesc = new PIC16MemoryDescriptor();
            memdesc.Reset();
            memdesc.LoadMemDescr(pic);
        }

        protected override IPICMemoryMap CreateMemoryMap(IPICDescriptor pic)
            => PIC16MemoryMap.Create(pic);

    }

}
