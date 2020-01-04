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

using Reko.Core.Expressions;

namespace Reko.Arch.MicrochipPIC.PIC18
{

    using Common;

    /// <summary>
    /// This class supports the PIC18 registers pool.
    /// </summary>
    public class PIC18Registers : PICRegisters
    {

        /// <summary> Overflow bit in STATUS register. </summary>
        public static PICRegisterBitFieldStorage OV { get; protected set; }

        /// <summary> Negative bit in STATUS register. </summary>
        public static PICRegisterBitFieldStorage N { get; protected set; }

        public static PICRegisterStorage FSR2L { get; protected set; }
        public static PICRegisterStorage FSR2H { get; protected set; }
        public static PICRegisterStorage PLUSW2 { get; protected set; }
        public static PICRegisterStorage PREINC2 { get; protected set; }
        public static PICRegisterStorage POSTDEC2 { get; protected set; }
        public static PICRegisterStorage POSTINC2 { get; protected set; }
        public static PICRegisterStorage INDF2 { get; protected set; }
        public static PICRegisterStorage FSR1L { get; protected set; }
        public static PICRegisterStorage FSR1H { get; protected set; }
        public static PICRegisterStorage PLUSW1 { get; protected set; }
        public static PICRegisterStorage PREINC1 { get; protected set; }
        public static PICRegisterStorage POSTDEC1 { get; protected set; }
        public static PICRegisterStorage POSTINC1 { get; protected set; }
        public static PICRegisterStorage INDF1 { get; protected set; }
        public static PICRegisterStorage FSR0L { get; protected set; }
        public static PICRegisterStorage FSR0H { get; protected set; }
        public static PICRegisterStorage PLUSW0 { get; protected set; }
        public static PICRegisterStorage PREINC0 { get; protected set; }
        public static PICRegisterStorage POSTDEC0 { get; protected set; }
        public static PICRegisterStorage POSTINC0 { get; protected set; }
        public static PICRegisterStorage INDF0 { get; protected set; }
        public static PICRegisterStorage PRODL { get; protected set; }
        public static PICRegisterStorage PRODH { get; protected set; }
        public static PICRegisterStorage TABLAT { get; protected set; }
        public static PICRegisterStorage TBLPTRL { get; protected set; }
        public static PICRegisterStorage TBLPTRH { get; protected set; }
        public static PICRegisterStorage TBLPTRU { get; protected set; }
        public static PICRegisterStorage PCLATU { get; protected set; }
        public static PICRegisterStorage TOSL { get; protected set; }
        public static PICRegisterStorage TOSH { get; protected set; }
        public static PICRegisterStorage TOSU { get; protected set; }


        /// <summary> PROD pseudo-register (alias to PRODH:PRODL). </summary>
        public static PICRegisterStorage PROD { get; protected set; }

        /// <summary> FSR0 pseudo-register (alias to FSR0H:FSR0L). </summary>
        public static PICRegisterStorage FSR0 { get; protected set; }

        /// <summary> FSR1 pseudo-register (alias to FSR1H:FSR1L). </summary>
        public static PICRegisterStorage FSR1 { get; protected set; }

        /// <summary> FSR2 pseudo-register (alias to FSR2H:FSR2L). </summary>
        public static PICRegisterStorage FSR2 { get; protected set; }

        /// <summary> TOS pseudo-register (alias to TOSU:TOSH:TOSL). </summary>
        public static PICRegisterStorage TOS { get; protected set; }

        /// <summary> PCLAT pseudo-register (alias to PCLATU:PCLATH:PCL). </summary>
        public static PICRegisterStorage PCLAT { get; protected set; }

        /// <summary> TBLPTR pseudo-register (alias to TBLPTRU:TBLPTRH:TBLPTRL). </summary>
        public static PICRegisterStorage TBLPTR { get; protected set; }


        public static PICRegisterStorage STATUS_CSHAD { get; protected set; }
        public static PICRegisterStorage WREG_CSHAD { get; protected set; }
        public static PICRegisterStorage BSR_CSHAD { get; protected set; }


        /// <summary> Hardware Return Address Stack of the PIC. </summary>
        public static MemoryIdentifier HWStack { get; private set; }

        /// <summary>
        /// This method sets each of the standard "core" registers of the PIC18.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition
        /// as providd by Microchip.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        protected override void SetCoreRegisters()
        {

            base.SetCoreRegisters();

            OV = GetBitField("OV");
            N = GetBitField("N");

            FSR2L = GetRegister("FSR2L");
            FSR2H = GetRegister("FSR2H");
            PLUSW2 = GetRegister("PLUSW2");
            PREINC2 = GetRegister("PREINC2");
            POSTDEC2 = GetRegister("POSTDEC2");
            POSTINC2 = GetRegister("POSTINC2");
            INDF2 = GetRegister("INDF2");

            BSR = GetRegister("BSR");
            FSR1L = GetRegister("FSR1L");
            FSR1H = GetRegister("FSR1H");
            PLUSW1 = GetRegister("PLUSW1");
            PREINC1 = GetRegister("PREINC1");
            POSTDEC1 = GetRegister("POSTDEC1");
            POSTINC1 = GetRegister("POSTINC1");
            INDF1 = GetRegister("INDF1");

            FSR0L = GetRegister("FSR0L");
            FSR0H = GetRegister("FSR0H");
            PLUSW0 = GetRegister("PLUSW0");
            PREINC0 = GetRegister("PREINC0");
            POSTDEC0 = GetRegister("POSTDEC0");
            POSTINC0 = GetRegister("POSTINC0");
            INDF0 = GetRegister("INDF0");

            PRODL = GetRegister("PRODL");
            PRODH = GetRegister("PRODH");
            TABLAT = GetRegister("TABLAT");
            TBLPTRL = GetRegister("TBLPTRL");
            TBLPTRH = GetRegister("TBLPTRH");
            TBLPTRU = GetRegister("TBLPTRU");
            PCLATU = GetRegister("PCLATU");
            TOSL = GetRegister("TOSL");
            TOSH = GetRegister("TOSH");
            TOSU = GetRegister("TOSU");

            // *Pseudo* (joined) registers

            PROD = GetRegister("PROD");
            FSR0 = GetRegister("FSR0");
            FSR1 = GetRegister("FSR1");
            FSR2 = GetRegister("FSR2");
            TOS = GetRegister("TOS");
            PCLAT = GetRegister("PCLAT");
            TBLPTR = GetRegister("TBLPTR");

            // Registers used for indirect memory adressing modes. An other ugly aspect of Microchip PICs.

            AddIndirectParents(true,
                    (PLUSW0, (FSRIndexedMode.PLUSW, FSR0)),
                    (PREINC0, (FSRIndexedMode.PREINC, FSR0)),
                    (POSTDEC0, (FSRIndexedMode.POSTDEC, FSR0)),
                    (POSTINC0, (FSRIndexedMode.POSTINC, FSR0)),
                    (INDF0, (FSRIndexedMode.INDF, FSR0)),
                    (PLUSW1, (FSRIndexedMode.PLUSW, FSR1)),
                    (PREINC1, (FSRIndexedMode.PREINC, FSR1)),
                    (POSTDEC1, (FSRIndexedMode.POSTDEC, FSR1)),
                    (POSTINC1, (FSRIndexedMode.POSTINC, FSR1)),
                    (INDF1, (FSRIndexedMode.INDF, FSR1)),
                    (PLUSW2, (FSRIndexedMode.PLUSW, FSR2)),
                    (PREINC2, (FSRIndexedMode.PREINC, FSR2)),
                    (POSTDEC2, (FSRIndexedMode.POSTDEC, FSR2)),
                    (POSTINC2, (FSRIndexedMode.POSTINC, FSR2)),
                    (INDF2, (FSRIndexedMode.INDF, FSR2)));

            TryGetRegister("STATUS_CSHAD", out var sta);
            STATUS_CSHAD = sta;
            TryGetRegister("WREG_CSHAD", out var wre);
            WREG_CSHAD = wre;
            TryGetRegister("BSR_CSHAD", out var bsr);
            BSR_CSHAD = bsr;

        }

        /// <summary>
        /// Registers values at Power-On Reset time for all PIC18.
        /// </summary>
        /// <param name="rlist">The list of register/value pairs.</param>
        protected override void SetRegistersValuesAtPOR()
        {
            base.SetRegistersValuesAtPOR();
            AddRegisterAtPOR(GetRegisterResetValue(BSR));
            AddRegisterAtPOR(GetRegisterResetValue(FSR0));
            AddRegisterAtPOR(GetRegisterResetValue(FSR1));
            AddRegisterAtPOR(GetRegisterResetValue(FSR2));
            AddRegisterAtPOR(GetRegisterResetValue(PROD));
            AddRegisterAtPOR(GetRegisterResetValue(PCLAT));
            AddRegisterAtPOR(GetRegisterResetValue(TOS));
            AddRegisterAtPOR(GetRegisterResetValue(TBLPTR));
        }

    }

}
