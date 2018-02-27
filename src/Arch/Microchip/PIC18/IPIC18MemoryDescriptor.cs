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

using Reko.Core.Expressions;
using Reko.Core;
using Reko.Libraries.Microchip;

namespace Reko.Arch.Microchip.Common
{
    /// <summary>
    /// Interface for PIC18 memory definitions.
    /// </summary>
    public interface IPIC18MemoryDescriptor
    {
        /// <summary>
        /// Gets the PIC memory map (definitions of memory regions, translation of addresses).
        /// </summary>
        IPICMemoryMap PICMemoryMap { get; }

        /// <summary>
        /// Gets the PIC Device Configuration Registers definitions.
        /// </summary>
        IPICDevConfDefs PICConfDefinitions { get; }

        /// <summary>
        /// Gets or sets the PIC execution mode.
        /// </summary>
        /// <value>
        /// The PIC execution mode.
        /// </value>
        PICExecMode ExecMode { get; set; }

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="addr">The address in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        PICDataAddress XlateAccessAddress(PICDataAddress addr);

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="cAddr">The offset in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        PICDataAddress XlateAccessAddress(Constant cAddr);

        /// <summary>
        /// Translates an Access RAM Bank address to actual data memory address.
        /// If the address does not belong to Access RAM it is returned as-is.
        /// </summary>
        /// <param name="uAddr">The offset in the Access RAM Bank.</param>
        /// <returns>
        /// The actual data memory Address.
        /// </returns>
        PICDataAddress XlateAccessAddress(uint uAddr);

        /// <summary>
        /// Query if data memory address <paramref name="cAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="cAddr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        bool IsAccessRAMLow(Constant cAddr);

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        bool IsAccessRAMLow(PICDataAddress addr);

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM Low range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM Low, false if not.
        /// </returns>
        bool IsAccessRAMLow(uint uAddr);

        /// <summary>
        /// Query if data memory address <paramref name="addr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="addr">The data memory address to check.</param>
        /// <returns>
        /// True if <paramref name="addr"/> belongs to Access RAM High, false if not.
        /// </returns>
        bool IsAccessRAMHigh(PICDataAddress addr);

        /// <summary>
        /// Query if memory address <paramref name="cAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="cAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="cAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        bool IsAccessRAMHigh(Constant cAddr);

        /// <summary>
        /// Query if memory address <paramref name="uAddr"/> belongs to Access RAM High range.
        /// </summary>
        /// <param name="uAddr">The memory address to check.</param>
        /// <returns>
        /// True if <paramref name="uAddr"/> belongs to Access RAM High, false if not.
        /// </returns>
        bool IsAccessRAMHigh(uint uAddr);

        /// <summary>
        /// Gets the human's readable Device Configuration description corresponding to the specified memory value(s) at specified address..
        /// </summary>
        /// <param name="progAddr">The program space address.</param>
        /// <param name="configbytes">A variable-length parameters list containing the configuration bytes.</param>
        string GetDeviceConfigDescription(Address progAddr, params byte[] configbytes);

    }

}
