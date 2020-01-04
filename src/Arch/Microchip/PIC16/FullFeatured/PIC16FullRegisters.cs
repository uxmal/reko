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
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// This class implements the Full-featured PIC16 registers pool.
    /// </summary>
    public class PIC16FullRegisters : PIC16Registers
    {

        private PIC16FullRegisters()
        {
        }

        public static PICRegisterStorage INDF0 { get; private set; }
        public static PICRegisterStorage INDF1 { get; private set; }
        public static PICRegisterStorage FSR0L { get; private set; }
        public static PICRegisterStorage FSR0H { get; private set; }
        public static PICRegisterStorage FSR1L { get; private set; }
        public static PICRegisterStorage FSR1H { get; private set; }

        /// <summary> FSR0 pseudo-register (alias to FSR0H:FSR0L). </summary>
        public static PICRegisterStorage FSR0 { get; private set; }

        /// <summary> FSR1 pseudo-register (alias to FSR1H:FSR1L). </summary>
        public static PICRegisterStorage FSR1 { get; private set; }

        /// <summary>
        /// Creates the Full-Featured PIC16 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(IPICDescriptor pic)
        {
            LoadRegisters(pic ?? throw new ArgumentNullException(nameof(pic)));
            var regs = new PIC16FullRegisters();
            regs.SetCoreRegisters();
            regs.SetRegistersValuesAtPOR();
        }

        /// <summary>
        /// This method sets each of the standard "core" registers of the Full-featured PIC16.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition as provided by Microchip.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        protected override void SetCoreRegisters()
        {
            base.SetCoreRegisters();

            INDF0 = GetRegister("INDF0");
            INDF1 = GetRegister("INDF1");

            FSR0L = GetRegister("FSR0L");
            FSR0H = GetRegister("FSR0H");
            FSR1L = GetRegister("FSR1L");
            FSR1H = GetRegister("FSR1H");
            FSR0 = GetRegister("FSR0");
            FSR1 = GetRegister("FSR1");

            BSR = GetRegister("BSR");

            AddIndirectParents(true,
                (INDF0, (FSRIndexedMode.INDF, FSR0)),
                (INDF1, (FSRIndexedMode.INDF, FSR1))
                );

            AddAlwaysAccessibleRegisters(true, INDF0, INDF1, PCL, STATUS, FSR0L, FSR0H, FSR1L, FSR1H, BSR, WREG, PCLATH, INTCON);

            SetRegistersValuesAtPOR();
        }

        /// <summary> Registers values at Power-On Reset time for PIC16 Full-featured. </summary>
        protected override void SetRegistersValuesAtPOR()
        {
            base.SetRegistersValuesAtPOR();
            AddRegisterAtPOR(GetRegisterResetValue(BSR));
            AddRegisterAtPOR(GetRegisterResetValue(FSR0));
            AddRegisterAtPOR(GetRegisterResetValue(FSR1));
        }

    }

}
