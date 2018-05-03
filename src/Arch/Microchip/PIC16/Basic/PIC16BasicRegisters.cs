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

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using Common;

    /// <summary>
    /// This class implements the Basic PIC16 registers pool.
    /// </summary>
    public class PIC16BasicRegisters : PIC16Registers
    {

        private PIC16BasicRegisters()
        {
        }

        /// <summary>
        /// Creates the Basic PIC16 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(PIC pic)
        {
            LoadRegisters(pic ?? throw new ArgumentNullException(nameof(pic)));
            new PIC16BasicRegisters().SetCoreRegisters();
        }

        /// <summary> Register Page in STATUS register. </summary>
        public static PICRegisterBitFieldStorage RP0 { get; protected set; }

        /// <summary> Register Page in STATUS register. </summary>
        public static PICRegisterBitFieldStorage RP1 { get; protected set; }

        /// <summary> INDF special function register. </summary>
        public static PICRegisterStorage INDF { get; private set; }

        /// <summary> FSR pseudo-register (alias to FSRH:FSRL). </summary>
        public static PICRegisterStorage FSR { get; private set; }

        public override void SetCoreRegisters()
        {
            base.SetCoreRegisters();

            RP0 = GetBitField("RP0");
            RP1 = GetBitField("RP1");

            INDF = GetRegister("INDF");
            FSR = GetRegister("FSR");

            AddPseudoBSR();

            BSR = GetRegister("BSR");
            
            AddIndirectParents(true,
                (INDF, (FSRIndexedMode.INDF, FSR))
                );

            AddAlwaysAccessibleRegisters(true, INDF, PCL, STATUS, FSR, PCLATH, INTCON);
        }

        private void AddPseudoBSR()
        {
            var sfr = new SFRDef()
            {
                CName = "BSR",
                Desc = $"Pseudo-register BSR",
                NMMRID = "0xb",
                AddrFormatted = "0",
                MCLR = "------00",
                POR = "------00",
                Access = "------nn",
                NzWidthFormatted = "8"
            };

            var reg = new PICRegisterStorage(sfr, 0);
            reg.BitFields.Add(new PICRegisterBitFieldSortKey(0, 8), RP0);
            reg.BitFields.Add(new PICRegisterBitFieldSortKey(1, 8), RP1);

            AddRegister(reg);
        }

    }

}
