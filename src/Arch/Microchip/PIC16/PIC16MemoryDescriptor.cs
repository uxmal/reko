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

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    internal class PIC16MemoryDescriptor : PICMemoryDescriptor
    {

        public class PIC16MemoryMap : MemoryMap
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
            /// <param name="thePIC">the PIC descriptor.</param>
            protected PIC16MemoryMap(PIC thePIC) : base(thePIC)
            {
            }

            /// <summary>
            /// Creates a new <see cref="IMemoryMap"/> interface for PIC16.
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
                    case InstructionSetID.PIC16:
                    case InstructionSetID.PIC16_ENHANCED:
                    case InstructionSetID.PIC16_FULLFEATURED:
                        var map = new PIC16MemoryMap(thePIC);
                        if (!map.IsValid)
                            throw new InvalidOperationException($"Mapper cannot be constructed for '{thePIC.Name}' device.");
                        return map;

                    default:
                        throw new InvalidOperationException($"Invalid PIC16 family: '{thePIC.Name}'.");
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
                set => dataMap = new DataMemoryMap(PIC, this, traits, PICExecMode.Traditional);
            }

            public override PICDataAddress RemapDataAddress(PICDataAddress lAddr)
            {
                if (lAddr == null)
                    throw new ArgumentNullException(nameof(lAddr));
                ushort uAddr = lAddr.ToUInt16();
                if (uAddr >= dataMap.remapTable.Length)
                    return lAddr;
                var mAddr = dataMap.remapTable[uAddr];
                return (mAddr == null ? lAddr : PICDataAddress.Ptr(mAddr));
            }

            /// <summary>
            /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
            /// </summary>
            /// <param name="cAddr">The memory address to check.</param>
            /// <returns>
            /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
            /// </returns>
            public override bool IsAccessRAMLow(PICDataAddress cAddr) => false;

            /// <summary>
            /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
            /// </summary>
            /// <param name="uAddr">The memory address to check.</param>
            /// <returns>
            /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
            /// </returns>
            public override bool IsAccessRAMHigh(PICDataAddress uAddr) => false;

            /// <summary>
            /// Query if memory address <paramref name="uAddr"/> can be a FSR2 index
            /// </summary>
            /// <param name="uAddr">The memory address to check.</param>
            public override bool CanBeFSR2IndexAddress(ushort uAddr) => false;

            #endregion

        }


        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PIC16MemoryDescriptor(PIC pic) : base(pic)
        {
        }

        public static void Create(PIC pic)
        {
            var memdesc = new PIC16MemoryDescriptor(pic ?? throw new ArgumentNullException(nameof(pic)));
            memdesc.Reset();
            memdesc.LoadMemDescr();
        }

        protected override IMemoryMap CreateMemoryMap()
            => PIC16MemoryMap.Create(pic);

    }

}
