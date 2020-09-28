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

using Reko.Libraries.Microchip;
using System;

namespace Reko.Arch.MicrochipPIC.PIC16
{
    using System.Collections.Generic;
    using Common;

    /// <summary>
    /// This class implements the Basic PIC16 registers pool.
    /// </summary>
    public class PIC16BasicRegisters : PIC16Registers
    {
        private class PseudoSFR : ISFRRegister
        {
            public int ByteWidth => (BitWidth + 7) >> 3;

            public uint ImplMask { get; set; }

            public string AccessBits { get; set; }

            public string MCLR { get; set; }

            public string POR { get; set; }

            public bool IsIndirect { get; set; }

            public bool IsVolatile { get; set; }

            public bool IsHidden { get; set; }

            public bool IsLangHidden { get; set; }

            public bool IsIDEHidden { get; set; }

            public string NMMRID { get; set; }

            public bool IsNMMR => !string.IsNullOrWhiteSpace(NMMRID);

            public IEnumerable<ISFRBitField> BitFields { get { yield break; } }

            public uint Addr { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }

            public byte BitWidth { get; set; }
        }

        private PIC16BasicRegisters()
        {
        }

        public static PICRegisterBitFieldStorage RP0 { get; protected set; }
        public static PICRegisterBitFieldStorage RP1 { get; protected set; }
        public static PICRegisterStorage INDF { get; private set; }
        public static PICRegisterStorage FSR { get; private set; }

        /// <summary> Sets core registers for PIC16 Basic. </summary>
        /// <summary>
        /// Creates the Basic PIC16 registers.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static void Create(IPICDescriptor pic)
        {
            LoadRegisters(pic ?? throw new ArgumentNullException(nameof(pic)));
            var regs = new PIC16BasicRegisters();
            regs.SetCoreRegisters();
            regs.SetRegistersValuesAtPOR();
        }

        /// <summary>
        /// This method sets each of the standard "core" registers of the Basic PIC16.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition as provided by Microchip.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        protected override void SetCoreRegisters()
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

        /// <summary> Registers values at Power-On Reset time for PIC16 Basic. </summary>
        protected override void SetRegistersValuesAtPOR()
        {
            base.SetRegistersValuesAtPOR();
            AddRegisterAtPOR(GetRegisterResetValue(BSR));
            AddRegisterAtPOR(GetRegisterResetValue(FSR));
        }

        private void AddPseudoBSR()
        {
            ISFRRegister bsr = new PseudoSFR()
            {
                Name = "BSR",
                Description = "Pseudo-register BSR",
                NMMRID = "0xb",
                Addr = 0,
                MCLR = "------00",
                POR = "------00",
                AccessBits = "------nn",
                BitWidth = 8
            };

            var reg = new PICRegisterStorage(bsr, 0);
            reg.BitFields.Add(new PICRegisterBitFieldSortKey(0, 8), RP0);
            reg.BitFields.Add(new PICRegisterBitFieldSortKey(1, 8), RP1);

            AddRegister(reg);
        }

    }

}
