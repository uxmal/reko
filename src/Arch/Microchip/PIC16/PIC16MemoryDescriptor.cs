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

    public class PIC16MemoryDescriptor : IPICMemoryDescriptor
    {

        private static PIC16MemoryDescriptor memDescr;
        private PIC16MemoryMap memoryMap;


        #region Inner classes

        public class PIC16MemoryMap : MemoryMap
        {
            #region Locals

            #endregion

            #region Constructors

            /// <summary>
            /// Constructor that prevents a default instance of this class from being created.
            /// </summary>
            private PIC16MemoryMap() : base()
            {
            }

            /// <summary>
            /// Private constructor creating an instance of memory map for specified PIC.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            protected PIC16MemoryMap(PIC thePIC) : base(thePIC)
            {
            }

            /// <summary>
            /// Creates a new <see cref="PIC16MemoryMap"/> instance.
            /// </summary>
            /// <param name="thePIC">the PIC descriptor.</param>
            /// <returns>
            /// A <see cref="PIC16MemoryMap"/> instance.
            /// </returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="thePIC"/> is null.</exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if the PIC definition contains an invalid
            ///                                               data memory size (less than 12 bytes).</exception>
            /// <exception cref="InvalidOperationException">Thrown if the PIC definition does not permit to construct the memory map.</exception>
            public static PIC16MemoryMap Create(PIC thePIC)
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

            #endregion

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

            #endregion

        }

        #endregion


        /// <summary>
        /// Constructor that prevents a default instance of this class from being created.
        /// </summary>
        private PIC16MemoryDescriptor()
        {
        }

        /// <summary>
        /// Private constructor creating an instance of memory descriptor for specified PIC.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        private PIC16MemoryDescriptor(PIC pic)
        {
            memoryMap = PIC16MemoryMap.Create(pic);
            DeviceConfigDefinitions = PICDeviceConfigDefs.Create(pic);
        }

        /// <summary>
        /// Creates a new PICMemoryDefinitions.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        /// <returns>
        /// A <see cref="IPICMemoryDescriptor"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
        public static PIC16MemoryDescriptor Create(PIC pic)
        {
            memDescr = new PIC16MemoryDescriptor(pic ?? throw new ArgumentNullException(nameof(pic)));
            return memDescr;
        }


        #region IPICMemoryDescriptor interface

        /// <summary>
        /// The memory description associated with this memory descriptor.
        /// </summary>
        public IMemoryMap MemoryMap => memoryMap;

        public IPICDeviceConfigDefs DeviceConfigDefinitions { get; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public PICExecMode ExecMode
        {
            get => PICExecMode.Traditional;
            set => MemoryMap.ExecMode = PICExecMode.Traditional;
        }

        #endregion

        #region Internal API

        /// <summary>
        /// Translates a Mirrored Bank address to actual data memory address. If the address does not
        /// belong to Mirrored RAM it is returned as-is.
        /// </summary>
        /// <param name="addr">The address in the Memory Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        internal static PICDataAddress TranslateDataAddress(PICDataAddress addr) => memDescr?.MemoryMap.RemapDataAddress(addr);

        /// <summary>
        /// Translates a Mirrored Bank address to actual data memory address.
        /// If the address does not belong to a Mirrored RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The address in the Memory Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        internal static PICDataAddress TranslateDataAddress(uint uAddr) => TranslateDataAddress(PICDataAddress.Ptr(uAddr));

        #endregion

    }

}
