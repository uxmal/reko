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
using Reko.Core.Types;

namespace Reko.Arch.Microchip.PIC16
{
    using Common;

    /// <summary>
    /// This class supports the PIC16 registers pool.
    /// </summary>
    public abstract class PIC16Registers
    {

        public static MemoryIdentifier GlobalStack = new MemoryIdentifier("Stack", PrimitiveType.Ptr32);
        public static MemoryIdentifier GlobalData = new MemoryIdentifier("Data", PrimitiveType.Byte);
        public static MemoryIdentifier GlobalCode = new MemoryIdentifier("Code", PrimitiveType.Ptr32);

        public abstract void SetCoreRegisters();

        // Below properties are definitions common to all PIC16 core registers and bit fields. 

        /// <summary>
        /// PCL special function register.
        /// </summary>
        public static PICRegisterStorage PCL { get; protected set; }

        /// <summary> STATUS register. </summary>
        public static PICRegisterStorage STATUS { get; protected set; }

        /// <summary> Carry bit in STATUS register. </summary>
        public static PICBitFieldStorage C { get; protected set; }

        /// <summary> Digit-Carry bit in STATUS register. </summary>
        public static PICBitFieldStorage DC { get; protected set; }

        /// <summary> Zero bit in STATUS register. </summary>
        public static PICBitFieldStorage Z { get; protected set; }

        /// <summary> Power-Down bit in STATUS or PCON register. </summary>
        public static PICBitFieldStorage PD { get; protected set; }

        /// <summary> Timed-Out bit in STATUS or PCON register. </summary>
        public static PICBitFieldStorage TO { get; protected set; }

        /// <summary> WREG special function register. </summary>
        public static PICRegisterStorage WREG { get; protected set; }

        /// <summary> PCLATH special function register. </summary>
        public static PICRegisterStorage PCLATH { get; protected set; }

        /// <summary> INTCON special function register. </summary>
        public static PICRegisterStorage INTCON { get; protected set; }

        /// <summary> STKPTR pseudo-register. </summary>
        public static PICRegisterStorage STKPTR { get; protected set; }

    }

}
