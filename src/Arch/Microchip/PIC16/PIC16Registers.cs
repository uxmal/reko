#region License
/* 
 * Copyright (C) 2017-2020 Christian Hostelet.
 * inspired by work from:
 * Copyright (C) 1999-2020 John Källén.
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
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// This class supports the PIC16 registers pool.
    /// </summary>
    public abstract class PIC16Registers : PICRegisters
    {

        protected PIC16Registers() { }


        /// <summary> INTCON special function register. </summary>
        public static PICRegisterStorage INTCON { get; protected set; }

        /// <summary> Global Interrupt Enable in INTCON register. </summary>
        public static PICRegisterBitFieldStorage GIE { get; protected set; }


        /// <summary>
        /// Sets core registers common to all PIC16.
        /// </summary>
        protected override void SetCoreRegisters()
        {
            base.SetCoreRegisters();

            INTCON = GetRegister("INTCON");
            GIE = GetBitField("GIE");
        }

        /// <summary>
        /// Registers values at Power-On Reset time.
        /// </summary>
        protected override void SetRegistersValuesAtPOR()
        {
            base.SetRegistersValuesAtPOR();
            AddRegisterAtPOR(GetRegisterResetValue(INTCON));
        }

    }

    }
