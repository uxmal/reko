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
using Reko.Core.Expressions;
using System;

namespace Reko.Arch.Microchip.PIC18
{
    using Common;

    /// <summary>
    /// A factory class which implements the PIC18 memory descriptor based on the PIC XML definition.
    /// </summary>
    public class PIC18MemoryDescriptor
    {

        #region Locals

        private static PIC18MemoryDescriptor memDescr;

        private const string AccessRAMRegionID = "accessram";
        private const string ExtendRAMRegionID = "gpre";
        private const string AccessSFRRegionID = "accesssfr";
        private IMemoryRegion AccessRAMLow;
        private IMemoryRegion AccessRAMHigh;

        private static Address topAccessRAM = Address.Ptr32(0x100);

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
            MemoryMap = Common.MemoryMap.Create(pic);
            DeviceConfigDefinitions = PICDevConfDefs.Create(pic);
        }

        /// <summary>
        /// Creates a new PICMemoryDefinitions.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        /// <returns>
        /// A <see cref="IPIC18MemoryDescriptor"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
        public static PIC18MemoryDescriptor Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            memDescr = new PIC18MemoryDescriptor(pic);
            memDescr.AccessRAMHigh = memDescr.MemoryMap.GetDataRegion(AccessSFRRegionID);
            if (memDescr.AccessRAMHigh is null)
                throw new InvalidOperationException($"Missing '{AccessSFRRegionID}' data memory region.");
            memDescr.AccessRAMLow = memDescr.MemoryMap.GetDataRegion(AccessRAMRegionID);
            if (memDescr.AccessRAMLow is null)
            {
                memDescr.AccessRAMLow = memDescr.MemoryMap.GetDataRegion(ExtendRAMRegionID);
                if (memDescr.AccessRAMLow is null)
                    throw new InvalidOperationException($"Missing either '{AccessRAMRegionID}' or '{ExtendRAMRegionID}' data memory region.");
            }
            return memDescr;
        }

        #endregion

        #region IPIC18MemoryDescriptor interface

        /// <summary>
        /// The memory description associated with this memory descriptor.
        /// </summary>
        public IMemoryMap MemoryMap { get; }

        public IDeviceConfigDefs DeviceConfigDefinitions { get; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public PICExecMode ExecMode
        {
            get => MemoryMap.ExecMode; 
            set
            {
                if (value != MemoryMap.ExecMode)
                {
                    MemoryMap.ExecMode = value;
                    switch (MemoryMap.ExecMode)
                    {
                        case PICExecMode.Traditional:
                            AccessRAMLow = MemoryMap.GetDataRegion(AccessRAMRegionID);
                            break;

                        case PICExecMode.Extended:
                            AccessRAMLow = MemoryMap.GetDataRegion(ExtendRAMRegionID);
                            break;
                    }
                    if (AccessRAMLow is null)
                        throw new InvalidOperationException($"Missing either '{AccessRAMRegionID}' or '{ExtendRAMRegionID}' data memory region.");
                }
            }
        }

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="addr">The address in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public PICDataAddress XlateAccessAddress(PICDataAddress addr)
        {
            if (!IsAccessRAMHigh(addr))
                return addr;
            ulong uaddr = addr.ToLinear();
            uaddr -= AccessRAMLow.LogicalByteAddress.End.ToLinear();
            uaddr += AccessRAMHigh.LogicalByteAddress.Begin.ToLinear();
            return PICDataAddress.Ptr((uint)uaddr);
        }

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="cAddr">The offset in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public PICDataAddress XlateAccessAddress(Constant cAddr)
        {
            return XlateAccessAddress(PICDataAddress.Ptr(cAddr.ToUInt32()));
        }

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public PICDataAddress XlateAccessAddress(uint uAddr)
        {
            return XlateAccessAddress(PICDataAddress.Ptr(uAddr));
        }

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public bool IsAccessRAMLow(PICDataAddress addr) => AccessRAMLow.Contains(addr);

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public bool IsAccessRAMLow(Constant cAddr) => AccessRAMLow.Contains(cAddr);

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public bool IsAccessRAMLow(uint uAddr) => AccessRAMLow.Contains(uAddr);

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public bool IsAccessRAMHigh(PICDataAddress addr) => IsAccessRAMHigh(addr.ToUInt32());

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public bool IsAccessRAMHigh(Constant cAddr) => IsAccessRAMHigh(cAddr.ToUInt32());

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public bool IsAccessRAMHigh(uint uAddr)
        {
            var addr = Address.Ptr32(uAddr);
            return (addr >= AccessRAMLow.LogicalByteAddress.End && addr < topAccessRAM);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="addr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static PICDataAddress TranslateAccessAddress(PICDataAddress addr) => memDescr?.XlateAccessAddress(addr);

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="cAddr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static PICDataAddress TranslateAccessAddress(Constant cAddr) => memDescr?.XlateAccessAddress(cAddr);

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static PICDataAddress TranslateAccessAddress(uint uAddr) => memDescr?.XlateAccessAddress(uAddr);

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMLow(PICDataAddress addr) => memDescr?.IsAccessRAMLow(addr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMLow(Constant cAddr) => memDescr?.IsAccessRAMLow(cAddr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMLow(ushort uAddr) => memDescr?.IsAccessRAMLow(uAddr) ?? false;

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMHigh(PICDataAddress addr) => memDescr?.IsAccessRAMHigh(addr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMHigh(Constant cAddr) => memDescr?.IsAccessRAMHigh(cAddr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMHigh(ushort uAddr) => memDescr?.IsAccessRAMHigh(uAddr) ?? false;

        #endregion

    }

}
