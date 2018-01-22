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

using Microchip.Crownking;
using Reko.Arch.Microchip.Common;
using Reko.Core;
using Reko.Core.Expressions;
using System;

namespace Reko.Arch.Microchip.PIC18
{

    /// <summary>
    /// A factory class which implements the PIC18 memory mapper based on the PIC definition.
    /// </summary>
    public class PIC18MemoryMapper : IPICMemoryMapper
    {

        #region Locals

        private static PIC18MemoryMapper _mapper;

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
        private PIC18MemoryMapper()
        {
        }

        /// <summary>
        /// Private constructor creating an instance of memory mapper for specified PIC.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        private PIC18MemoryMapper(PIC pic)
        {
            MemoryMap = PICMemoryMap.Create(pic);
        }

        /// <summary>
        /// Creates a new PICMemoryMapper.
        /// </summary>
        /// <param name="pic">The target PIC.</param>
        /// <returns>
        /// A <see cref="IPICMemoryMap"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="pic"/> is null.</exception>
        public static PIC18MemoryMapper Create(PIC pic)
        {
            if (pic == null) throw new ArgumentNullException(nameof(pic));
            _mapper = new PIC18MemoryMapper(pic);
            _mapper.AccessRAMHigh = _mapper.MemoryMap.GetDataRegion(AccessSFRRegionID);
            if (_mapper.AccessRAMHigh == null)
                throw new InvalidOperationException($"Missing '{AccessSFRRegionID}' data memory region.");
            _mapper.AccessRAMLow = _mapper.MemoryMap.GetDataRegion(AccessRAMRegionID);
            if (_mapper.AccessRAMLow == null)
            {
                _mapper.AccessRAMLow = _mapper.MemoryMap.GetDataRegion(ExtendRAMRegionID);
                if (_mapper.AccessRAMLow == null)
                    throw new InvalidOperationException($"Missing '{AccessRAMRegionID}'/'{ExtendRAMRegionID}' data memory region.");
            }
            return _mapper;
        }

        #endregion

        #region IPICMemoryMapper interface

        /// <summary>
        /// The memory map associated with this memory mapper.
        /// </summary>
        public IPICMemoryMap MemoryMap { get; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        public PICExecMode ExecMode
        {
            get { return MemoryMap.ExecMode; }
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
                    if (AccessRAMLow == null)
                        throw new InvalidOperationException($"Missing '{AccessRAMRegionID}'/'{ExtendRAMRegionID}' data memory region.");
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
        public Address XlateAccessAddress(Address addr)
        {
            if (!IsAccessRAMHigh(addr))
                return addr;
            ulong uaddr = addr.ToLinear();
            uaddr -= AccessRAMLow.LogicalByteAddress.End.ToLinear();
            uaddr += AccessRAMHigh.LogicalByteAddress.Begin.ToLinear();
            return Address.Create(addr.DataType, uaddr);
        }

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="cAddr">The offset in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public Address XlateAccessAddress(Constant cAddr)
        {
            return XlateAccessAddress(Address.Ptr32(cAddr.ToUInt32()));
        }

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public Address XlateAccessAddress(uint uAddr)
        {
            return XlateAccessAddress(Address.Ptr32(uAddr));
        }

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public bool IsAccessRAMLow(Address addr) => AccessRAMLow.Contains(addr);

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
        public bool IsAccessRAMHigh(Address addr) => IsAccessRAMHigh(addr.ToUInt32());

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
            Address addr = Address.Ptr32(uAddr);
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
        public static Address TranslateAccessAddress(Address addr) => _mapper?.XlateAccessAddress(addr);

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="cAddr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static Address TranslateAccessAddress(Constant cAddr) => _mapper?.XlateAccessAddress(cAddr);

        /// <summary>
        /// Translates an Access Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        public static Address TranslateAccessAddress(uint uAddr) => _mapper?.XlateAccessAddress(uAddr);

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMLow(Address addr) => _mapper?.IsAccessRAMLow(addr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMLow(Constant cAddr) => _mapper?.IsAccessRAMLow(cAddr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMLow(ushort uAddr) => _mapper?.IsAccessRAMLow(uAddr) ?? false;

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMHigh(Address addr) => _mapper?.IsAccessRAMHigh(addr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMHigh(Constant cAddr) => _mapper?.IsAccessRAMHigh(cAddr) ?? false;

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        public static bool BelongsToAccessRAMHigh(ushort uAddr) => _mapper?.IsAccessRAMHigh(uAddr) ?? false;

        #endregion

    }

}
