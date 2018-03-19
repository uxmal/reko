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

using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Libraries.Microchip;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.Microchip.PIC18
{

    using Common;

    /// <summary>
    /// Values that represent indirect addressing using FSR indirect pseudo-registers.
    /// </summary>
    public enum IndirectRegOp : byte
    {
        /// <summary> No indirect read/write access. </summary>
        None,
        /// <summary> Indirect read/write using FSRx register (INDFx). </summary>
        INDF,
        /// <summary> Indirect read/write using post-incremented FSRx register (POSTINCx). </summary>
        POSTINC,
        /// <summary> Indirect read/write using post-decremented FSRx register (POSTDECx). </summary>
        POSTDEC,
        /// <summary> Indirect read/write using pre-incremented FSRx register (PREINCx). </summary>
        PREINC,
        /// <summary> Indirect read/write using FSRx register + WREG offset (PLUSWx). </summary>
        PLUSW
    }

    /// <summary>
    /// This class supports the PIC18 registers pool.
    /// </summary>
    public class PIC18Registers 
    {

        public static MemoryIdentifier GlobalStack = new MemoryIdentifier("Stack", PrimitiveType.Ptr32);
        public static MemoryIdentifier GlobalData = new MemoryIdentifier("Data", PrimitiveType.Byte);
        public static MemoryIdentifier GlobalCode = new MemoryIdentifier("Code", PrimitiveType.Ptr32);

        protected static Dictionary<PICRegisterStorage, (IndirectRegOp iop, PICRegisterStorage fsr)> indirectParents
            = new Dictionary<PICRegisterStorage, (IndirectRegOp, PICRegisterStorage)>();

        /// <summary> STATUS register. </summary>
        public static PICRegisterStorage STATUS { get; protected set; }

        /// <summary> Carry bit in STATUS register. </summary>
        public static PICBitFieldStorage C { get; protected set; }

        /// <summary> Digit-Carry bit in STATUS register. </summary>
        public static PICBitFieldStorage DC { get; protected set; }

        /// <summary> Zero bit in STATUS register. </summary>
        public static PICBitFieldStorage Z { get; protected set; }

        /// <summary> Overflow bit in STATUS register. </summary>
        public static PICBitFieldStorage OV { get; protected set; }

        /// <summary> Negative bit in STATUS register. </summary>
        public static PICBitFieldStorage N { get; protected set; }

        /// <summary> Power-Down bit in STATUS or PCON register. </summary>
        public static PICBitFieldStorage PD { get; protected set; }

        /// <summary> Timed-Out bit in STATUS or PCON register. </summary>
        public static PICBitFieldStorage TO { get; protected set; }

        /// <summary> FSR2L special function register. </summary>
        public static PICRegisterStorage FSR2L { get; protected set; }

        /// <summary> FSR2H special function register. </summary>
        public static PICRegisterStorage FSR2H { get; protected set; }

        /// <summary> PLUSW2 special function register. </summary>
        public static PICRegisterStorage PLUSW2 { get; protected set; }

        /// <summary> PREINC2 special function register. </summary>
        public static PICRegisterStorage PREINC2 { get; protected set; }

        /// <summary> POSTDEC2 special function register. </summary>
        public static PICRegisterStorage POSTDEC2 { get; protected set; }

        /// <summary> POSTINC2 special function register. </summary>
        public static PICRegisterStorage POSTINC2 { get; protected set; }

        /// <summary> INDF2 special function register. </summary>
        public static PICRegisterStorage INDF2 { get; protected set; }

        /// <summary> BSR special function register. </summary>
        public static PICRegisterStorage BSR { get; protected set; }

        /// <summary> FSR1L special function register. </summary>
        public static PICRegisterStorage FSR1L { get; protected set; }

        /// <summary> FSR1H special function register. </summary>
        public static PICRegisterStorage FSR1H { get; protected set; }

        /// <summary> PLUSW1 special function register. </summary>
        public static PICRegisterStorage PLUSW1 { get; protected set; }

        /// <summary> PREINC1 special function register. </summary>
        public static PICRegisterStorage PREINC1 { get; protected set; }

        /// <summary> POSTDEC1 special function register. </summary>
        public static PICRegisterStorage POSTDEC1 { get; protected set; }

        /// <summary> POSTINC1 special function register. </summary>
        public static PICRegisterStorage POSTINC1 { get; protected set; }

        /// <summary> INDF1 special function register. </summary>
        public static PICRegisterStorage INDF1 { get; protected set; }

        /// <summary> WREG special function register. </summary>
        public static PICRegisterStorage WREG { get; protected set; }

        /// <summary> FSR0L special function register. </summary>
        public static PICRegisterStorage FSR0L { get; protected set; }

        /// <summary> FSR0H special function register. </summary>
        public static PICRegisterStorage FSR0H { get; protected set; }

        /// <summary> PLUSW0 special function register. </summary>
        public static PICRegisterStorage PLUSW0 { get; protected set; }

        /// <summary> PREINC0 special function register. </summary>
        public static PICRegisterStorage PREINC0 { get; protected set; }

        /// <summary> POSTDEC0 special function register. </summary>
        public static PICRegisterStorage POSTDEC0 { get; protected set; }

        /// <summary> POSTINC0 special function register. </summary>
        public static PICRegisterStorage POSTINC0 { get; protected set; }

        /// <summary> INDF0 special function register. </summary>
        public static PICRegisterStorage INDF0 { get; protected set; }

        /// <summary> PRODL special function register. </summary>
        public static PICRegisterStorage PRODL { get; protected set; }

        /// <summary> PRODH special function register. </summary>
        public static PICRegisterStorage PRODH { get; protected set; }

        /// <summary> TABLAT special function register. </summary>
        public static PICRegisterStorage TABLAT { get; protected set; }

        /// <summary> TBLPTRL special function register. </summary>
        public static PICRegisterStorage TBLPTRL { get; protected set; }

        /// <summary> TBLPTRH special function register. </summary>
        public static PICRegisterStorage TBLPTRH { get; protected set; }

        /// <summary> TBLPTRU special function register. </summary>
        public static PICRegisterStorage TBLPTRU { get; protected set; }

        /// <summary> PCL special function register. </summary>
        public static PICRegisterStorage PCL { get; protected set; }

        /// <summary> PCLH special function register. </summary>
        public static PICRegisterStorage PCLATH { get; protected set; }

        /// <summary> PCLU special function register. </summary>
        public static PICRegisterStorage PCLATU { get; protected set; }

        /// <summary> STKPTR special function register. </summary>
        public static PICRegisterStorage STKPTR { get; protected set; }

        /// <summary> TOSL special function register. </summary>
        public static PICRegisterStorage TOSL { get; protected set; }

        /// <summary> TOSH special function register. </summary>
        public static PICRegisterStorage TOSH { get; protected set; }

        ///<summary> TOSU special function register.</summary>
        public static PICRegisterStorage TOSU { get; protected set; }

        #region Pseudo-registers

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

        /// <summary> TBLPTR pseudo-register (alias to TBLPTRL:TBLPTRH:TBLPTRL). </summary>
        public static PICRegisterStorage TBLPTR { get; protected set; }

        #region Shadow registers for some PIC18

        public static PICRegisterStorage STATUS_CSHAD { get; protected set; }
        public static PICRegisterStorage WREG_CSHAD { get; protected set; }
        public static PICRegisterStorage BSR_CSHAD { get; protected set; }

        #endregion

        /// <summary>
        /// Hardware Return Address Stack of the PIC.
        /// </summary>
        public static MemoryIdentifier HWStack { get; private set; }

        /// <summary>
        /// This method sets each of the standard "core" registers of the PIC18.
        /// They are retrieved from the registers symbol table which has been previously populated by loading the PIC definition.
        /// </summary>
        /// <remarks>
        /// This permits to still get a direct reference to standard registers and keeps having some flexibility on definitions.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if a register cannot be found in the symbol table.</exception>
        public virtual void SetCoreRegisters()
        {

            // *True* PIC registers

            STATUS = PICRegisters.GetRegister("STATUS");
            C = PICRegisters.GetBitField("C");
            DC = PICRegisters.GetBitField("DC");
            Z = PICRegisters.GetBitField("Z");
            OV = PICRegisters.GetBitField("OV");
            N = PICRegisters.GetBitField("Z");

            if (!PICRegisters.TryGetBitField("nPD", out var pd))
            {
                PICRegisters.TryGetBitField("PD", out pd);
            }
            PD = pd;
            if (!PICRegisters.TryGetBitField("nTO", out var to))
            {
                PICRegisters.TryGetBitField("TO", out to);
            }
            TO = to;

            FSR2L = PICRegisters.GetRegister("FSR2L");
            FSR2H = PICRegisters.GetRegister("FSR2H");
            PLUSW2 = PICRegisters.GetRegister("PLUSW2");
            PREINC2 = PICRegisters.GetRegister("PREINC2");
            POSTDEC2 = PICRegisters.GetRegister("POSTDEC2");
            POSTINC2 = PICRegisters.GetRegister("POSTINC2");
            INDF2 = PICRegisters.GetRegister("INDF2");

            BSR = PICRegisters.GetRegister("BSR");
            FSR1L = PICRegisters.GetRegister("FSR1L");
            FSR1H = PICRegisters.GetRegister("FSR1H");
            PLUSW1 = PICRegisters.GetRegister("PLUSW1");
            PREINC1 = PICRegisters.GetRegister("PREINC1");
            POSTDEC1 = PICRegisters.GetRegister("POSTDEC1");
            POSTINC1 = PICRegisters.GetRegister("POSTINC1");
            INDF1 = PICRegisters.GetRegister("INDF1");

            WREG = PICRegisters.GetRegister("WREG");
            FSR0L = PICRegisters.GetRegister("FSR0L");
            FSR0H = PICRegisters.GetRegister("FSR0H");
            PLUSW0 = PICRegisters.GetRegister("PLUSW0");
            PREINC0 = PICRegisters.GetRegister("PREINC0");
            POSTDEC0 = PICRegisters.GetRegister("POSTDEC0");
            POSTINC0 = PICRegisters.GetRegister("POSTINC0");
            INDF0 = PICRegisters.GetRegister("INDF0");

            PRODL = PICRegisters.GetRegister("PRODL");
            PRODH = PICRegisters.GetRegister("PRODH");
            TABLAT = PICRegisters.GetRegister("TABLAT");
            TBLPTRL = PICRegisters.GetRegister("TBLPTRL");
            TBLPTRH = PICRegisters.GetRegister("TBLPTRH");
            TBLPTRU = PICRegisters.GetRegister("TBLPTRU");
            PCL = PICRegisters.GetRegister("PCL");
            PCLATH = PICRegisters.GetRegister("PCLATH");
            PCLATU = PICRegisters.GetRegister("PCLATU");
            STKPTR = PICRegisters.GetRegister("STKPTR");
            TOSL = PICRegisters.GetRegister("TOSL");
            TOSH = PICRegisters.GetRegister("TOSH");
            TOSU = PICRegisters.GetRegister("TOSU");

            // *Pseudo* (joined) registers

            PROD = PICRegisters.GetRegister("PROD");
            FSR0 = PICRegisters.GetRegister("FSR0");
            FSR1 = PICRegisters.GetRegister("FSR1");
            FSR2 = PICRegisters.GetRegister("FSR2");
            TOS = PICRegisters.GetRegister("TOS");
            PCLAT = PICRegisters.GetRegister("PCLAT");
            TBLPTR = PICRegisters.GetRegister("TBLPTR");
            PRODH.BitAddress = 8;
            FSR0H.BitAddress = 8;
            FSR1H.BitAddress = 8;
            FSR2H.BitAddress = 8;
            TOSH.BitAddress = 8;
            TOSU.BitAddress = 16;
            PCLATH.BitAddress = 8;
            PCLATU.BitAddress = 16;
            TBLPTRH.BitAddress = 8;
            TBLPTRU.BitAddress = 16;

            // Registers used for indirect memory adressing modes. An other ugly aspect of Microchip PICs.

            indirectParents.Clear();
            indirectParents.Add(PLUSW0, (IndirectRegOp.PLUSW, FSR0));
            indirectParents.Add(PREINC0, (IndirectRegOp.PREINC, FSR0));
            indirectParents.Add(POSTDEC0, (IndirectRegOp.POSTDEC, FSR0));
            indirectParents.Add(POSTINC0, (IndirectRegOp.POSTINC, FSR0));
            indirectParents.Add(INDF0, (IndirectRegOp.INDF, FSR0));
            indirectParents.Add(PLUSW1, (IndirectRegOp.PLUSW, FSR1));
            indirectParents.Add(PREINC1, (IndirectRegOp.PREINC, FSR1));
            indirectParents.Add(POSTDEC1, (IndirectRegOp.POSTDEC, FSR1));
            indirectParents.Add(POSTINC1, (IndirectRegOp.POSTINC, FSR1));
            indirectParents.Add(INDF1, (IndirectRegOp.INDF, FSR1));
            indirectParents.Add(PLUSW2, (IndirectRegOp.PLUSW, FSR2));
            indirectParents.Add(PREINC2, (IndirectRegOp.PREINC, FSR2));
            indirectParents.Add(POSTDEC2, (IndirectRegOp.POSTDEC, FSR2));
            indirectParents.Add(POSTINC2, (IndirectRegOp.POSTINC, FSR2));
            indirectParents.Add(INDF2, (IndirectRegOp.INDF, FSR2));

            PICRegisters.TryGetRegister("STATUS_CSHAD", out var sta);
            STATUS_CSHAD = sta;
            PICRegisters.TryGetRegister("WREG_CSHAD", out var wre);
            WREG_CSHAD = wre;
            PICRegisters.TryGetRegister("BSR_CSHAD", out var bsr);
            BSR_CSHAD = bsr;

        }

        #endregion


        /// <summary>
        /// Query if '<paramref name="sfr"/>' register is an indirect register (INDFx, PLUSWx, POSTINCx,... ) and get the associated FSR register.
        /// Returns the indirect addressing mode if applicable, else return None if <paramref name="sfr"/> is not an indirect register.
        /// </summary>
        /// <param name="sfr">The register used in instruction's operand.</param>
        /// <param name="parentFSR">[out] The actual FSR index register if <paramref name="sfr"/> is an indirect register.</param>
        /// <returns>
        /// The indirect addressing mode, or None.
        /// </returns>
        public static IndirectRegOp IndirectOpMode(PICRegisterStorage sfr, out PICRegisterStorage parentFSR)
        {
            parentFSR = PICRegisterStorage.None;
            if (sfr is null)
                return IndirectRegOp.None;
            if (indirectParents.TryGetValue(sfr, out (IndirectRegOp indMode, PICRegisterStorage fsr) ent))
            {
                parentFSR = ent.fsr;
                return ent.indMode;
            }
            return IndirectRegOp.None;
        }

    }

}
