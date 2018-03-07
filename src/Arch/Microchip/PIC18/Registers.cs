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
    /// This class implements the PIC18 registers pool.
    /// </summary>
    public class PIC18Registers : PICRegisters
    {

        #region Locals

        private static Dictionary<PICRegisterStorage, (IndirectRegOp iop, PICRegisterStorage fsr)> indirectParents
            = new Dictionary<PICRegisterStorage, (IndirectRegOp, PICRegisterStorage)>();

        private static HashSet<PICRegisterStorage> invalidMovfflDests
            = new HashSet<PICRegisterStorage>();

        #endregion

        #region PIC18 standard (core) registers and bit fields

        /// <summary>
        /// STATUS register.
        /// </summary>
        public static PICRegisterStorage STATUS { get; private set; }

        /// <summary>
        /// Carry bit in STATUS register.
        /// </summary>
        public static PICBitFieldStorage C { get; private set; }

        /// <summary>
        /// Digit-Carry bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage DC { get; private set; }

        /// <summary>
        /// Zero bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage Z { get; private set; }

        /// <summary>
        /// Overflow bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage OV { get; private set; }

        /// <summary>
        /// Negative bit in STATUS register..
        /// </summary>
        public static PICBitFieldStorage N { get; private set; }

        /// <summary>
        /// Power-Down bit in STATUS or PCON register..
        /// </summary>
        public static PICBitFieldStorage PD { get; private set; }

        /// <summary>
        /// Timed-Out bit in STATUS or PCON register..
        /// </summary>
        public static PICBitFieldStorage TO { get; private set; }

        /// <summary>
        /// FSR2L special function register.
        /// </summary>
        public static PICRegisterStorage FSR2L { get; private set; }

        /// <summary>
        /// FSR2H special function register.
        /// </summary>
        public static PICRegisterStorage FSR2H { get; private set; }

        /// <summary>
        /// PLUSW2 special function register.
        /// </summary>
        public static PICRegisterStorage PLUSW2 { get; private set; }

        /// <summary>
        /// PREINC2 special function register.
        /// </summary>
        public static PICRegisterStorage PREINC2 { get; private set; }

        /// <summary>
        /// POSTDEC2 special function register.
        /// </summary>
        public static PICRegisterStorage POSTDEC2 { get; private set; }

        /// <summary>
        /// POSTINC2 special function register.
        /// </summary>
        public static PICRegisterStorage POSTINC2 { get; private set; }

        /// <summary>
        /// INDF2 special function register.
        /// </summary>
        public static PICRegisterStorage INDF2 { get; private set; }

        /// <summary>
        /// BSR special function register.
        /// </summary>
        public static PICRegisterStorage BSR { get; private set; }

        /// <summary>
        /// FSR1L special function register.
        /// </summary>
        public static PICRegisterStorage FSR1L { get; private set; }

        /// <summary>
        /// FSR1H special function register.
        /// </summary>
        public static PICRegisterStorage FSR1H { get; private set; }

        /// <summary>
        /// PLUSW1 special function register.
        /// </summary>
        public static PICRegisterStorage PLUSW1 { get; private set; }

        /// <summary>
        /// PREINC1 special function register.
        /// </summary>
        public static PICRegisterStorage PREINC1 { get; private set; }

        /// <summary>
        /// POSTDEC1 special function register.
        /// </summary>
        public static PICRegisterStorage POSTDEC1 { get; private set; }

        /// <summary>
        /// POSTINC1 special function register.
        /// </summary>
        public static PICRegisterStorage POSTINC1 { get; private set; }

        /// <summary>
        /// INDF1 special function register.
        /// </summary>
        public static PICRegisterStorage INDF1 { get; private set; }

        /// <summary>
        /// WREG special function register.
        /// </summary>
        public static PICRegisterStorage WREG { get; private set; }

        /// <summary>
        /// FSR0L special function register.
        /// </summary>
        public static PICRegisterStorage FSR0L { get; private set; }

        /// <summary>
        /// FSR0H special function register.
        /// </summary>
        public static PICRegisterStorage FSR0H { get; private set; }

        /// <summary>
        /// PLUSW0 special function register.
        /// </summary>
        public static PICRegisterStorage PLUSW0 { get; private set; }

        /// <summary>
        /// PREINC0 special function register.
        /// </summary>
        public static PICRegisterStorage PREINC0 { get; private set; }

        /// <summary>
        /// POSTDEC0 special function register.
        /// </summary>
        public static PICRegisterStorage POSTDEC0 { get; private set; }

        /// <summary>
        /// POSTINC0 special function register.
        /// </summary>
        public static PICRegisterStorage POSTINC0 { get; private set; }

        /// <summary>
        /// INDF0 special function register.
        /// </summary>
        public static PICRegisterStorage INDF0 { get; private set; }

        /// <summary>
        /// PRODL special function register.
        /// </summary>
        public static PICRegisterStorage PRODL { get; private set; }

        /// <summary>
        /// PRODH special function register.
        /// </summary>
        public static PICRegisterStorage PRODH { get; private set; }

        /// <summary>
        /// TABLAT special function register.
        /// </summary>
        public static PICRegisterStorage TABLAT { get; private set; }

        /// <summary>
        /// TBLPTRL special function register.
        /// </summary>
        public static PICRegisterStorage TBLPTRL { get; private set; }

        /// <summary>
        /// TBLPTRH special function register.
        /// </summary>
        public static PICRegisterStorage TBLPTRH { get; private set; }

        /// <summary>
        /// TBLPTRU special function register.
        /// </summary>
        public static PICRegisterStorage TBLPTRU { get; private set; }

        /// <summary>
        /// PCL special function register.
        /// </summary>
        public static PICRegisterStorage PCL { get; private set; }

        /// <summary>
        /// PCLH special function register.
        /// </summary>
        public static PICRegisterStorage PCLATH { get; private set; }

        /// <summary>
        /// PCLU special function register.
        /// </summary>
        public static PICRegisterStorage PCLATU { get; private set; }

        /// <summary>
        /// STKPTR special function register.
        /// </summary>
        public static PICRegisterStorage STKPTR { get; private set; }

        /// <summary>
        /// TOSL special function register.
        /// </summary>
        public static PICRegisterStorage TOSL { get; private set; }

        /// <summary>
        /// TOSH special function register.
        /// </summary>
        public static PICRegisterStorage TOSH { get; private set; }

        ///<summary>
        /// TOSU special function register.
        ///</summary>
        public static PICRegisterStorage TOSU { get; private set; }

        #region Pseudo-registers

        /// <summary>
        /// PROD pseudo-register (alias to PRODH:PRODL).
        /// </summary>
        public static PICRegisterStorage PROD { get; private set; }

        /// <summary>
        /// FSR0 pseudo-register (alias to FSR0H:FSR0L).
        /// </summary>
        public static PICRegisterStorage FSR0 { get; private set; }

        /// <summary>
        /// FSR1 pseudo-register (alias to FSR1H:FSR1L).
        /// </summary>
        public static PICRegisterStorage FSR1 { get; private set; }

        /// <summary>
        /// FSR2 pseudo-register (alias to FSR2H:FSR2L).
        /// </summary>
        public static PICRegisterStorage FSR2 { get; private set; }

        /// <summary>
        /// TOS pseudo-register (alias to TOSU:TOSH:TOSL).
        /// </summary>
        public static PICRegisterStorage TOS { get; private set; }

        /// <summary>
        /// PC pseudo-register (alias to PCLATU:PCLATH:PCL).
        /// </summary>
        public static PICRegisterStorage PCLAT { get; private set; }

        /// <summary>
        /// TBLPTR pseudo-register (alias to TBLPTRL:TBLPTRH:TBLPTRL).
        /// </summary>
        public static PICRegisterStorage TBLPTR { get; private set; }

        #region Shadow registers for some PIC18

        public static PICRegisterStorage STATUS_CSHAD { get; private set; }
        public static PICRegisterStorage WREG_CSHAD { get; private set; }
        public static PICRegisterStorage BSR_CSHAD { get; private set; }

        #endregion

        #endregion

        public static MemoryIdentifier GlobalStack = new MemoryIdentifier("Stack", PrimitiveType.Ptr32);
        public static MemoryIdentifier GlobalData = new MemoryIdentifier("Data", PrimitiveType.Byte);
        public static MemoryIdentifier GlobalCode = new MemoryIdentifier("Code", PrimitiveType.Ptr32);

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
        protected override void SetCoreRegisters()
        {

            // *True* PIC registers

            STATUS = GetRegisterStorageByName("STATUS");
            C = GetBitFieldStorageByName("C");
            DC = GetBitFieldStorageByName("DC");
            Z = GetBitFieldStorageByName("Z");
            OV = GetBitFieldStorageByName("OV");
            N = GetBitFieldStorageByName("Z");

            PD = PeekBitFieldStorageByName("PD");
            if (PD is null)
                PD = PeekBitFieldStorageByName("nPD");
            TO = PeekBitFieldStorageByName("TO");
            if (TO is null)
                TO = PeekBitFieldStorageByName("nTO");

            FSR2L = GetRegisterStorageByName("FSR2L");
            FSR2H = GetRegisterStorageByName("FSR2H");
            PLUSW2 = GetRegisterStorageByName("PLUSW2");
            PREINC2 = GetRegisterStorageByName("PREINC2");
            POSTDEC2 = GetRegisterStorageByName("POSTDEC2");
            POSTINC2 = GetRegisterStorageByName("POSTINC2");
            INDF2 = GetRegisterStorageByName("INDF2");

            BSR = GetRegisterStorageByName("BSR");
            FSR1L = GetRegisterStorageByName("FSR1L");
            FSR1H = GetRegisterStorageByName("FSR1H");
            PLUSW1 = GetRegisterStorageByName("PLUSW1");
            PREINC1 = GetRegisterStorageByName("PREINC1");
            POSTDEC1 = GetRegisterStorageByName("POSTDEC1");
            POSTINC1 = GetRegisterStorageByName("POSTINC1");
            INDF1 = GetRegisterStorageByName("INDF1");

            WREG = GetRegisterStorageByName("WREG");
            FSR0L = GetRegisterStorageByName("FSR0L");
            FSR0H = GetRegisterStorageByName("FSR0H");
            PLUSW0 = GetRegisterStorageByName("PLUSW0");
            PREINC0 = GetRegisterStorageByName("PREINC0");
            POSTDEC0 = GetRegisterStorageByName("POSTDEC0");
            POSTINC0 = GetRegisterStorageByName("POSTINC0");
            INDF0 = GetRegisterStorageByName("INDF0");

            PRODL = GetRegisterStorageByName("PRODL");
            PRODH = GetRegisterStorageByName("PRODH");
            TABLAT = GetRegisterStorageByName("TABLAT");
            TBLPTRL = GetRegisterStorageByName("TBLPTRL");
            TBLPTRH = GetRegisterStorageByName("TBLPTRH");
            TBLPTRU = GetRegisterStorageByName("TBLPTRU");
            PCL = GetRegisterStorageByName("PCL");
            PCLATH = GetRegisterStorageByName("PCLATH");
            PCLATU = GetRegisterStorageByName("PCLATU");
            STKPTR = GetRegisterStorageByName("STKPTR");
            TOSL = GetRegisterStorageByName("TOSL");
            TOSH = GetRegisterStorageByName("TOSH");
            TOSU = GetRegisterStorageByName("TOSU");

            // *Pseudo* (joined) registers

            PROD = GetRegisterStorageByName("PROD");
            FSR0 = GetRegisterStorageByName("FSR0");
            FSR1 = GetRegisterStorageByName("FSR1");
            FSR2 = GetRegisterStorageByName("FSR2");
            TOS = GetRegisterStorageByName("TOS");
            PCLAT = GetRegisterStorageByName("PCLAT");
            TBLPTR = GetRegisterStorageByName("TBLPTR");
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

            // Shadow registers (if they exist).

            STATUS_CSHAD = PeekRegisterStorageByName("STATUS_CSHAD");
            WREG_CSHAD = PeekRegisterStorageByName("WREG_CSHAD");
            BSR_CSHAD = PeekRegisterStorageByName("BSR_CSHAD");

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

            // Some registers are invalid memory destination for some instructions.

            invalidMovfflDests.Clear();
            invalidMovfflDests.Add(PCL);
            invalidMovfflDests.Add(TOSL);
            invalidMovfflDests.Add(TOSH);
            invalidMovfflDests.Add(TOSU);

        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="pic">The PIC18 definition descriptor.</param>
        private PIC18Registers(PIC pic) : base(pic)
        {
        }

        /// <summary>
        /// Creates a new <see cref="PIC18Registers"/> instance.
        /// </summary>
        /// <param name="pic">The PIC definition.</param>
        /// <returns>
        /// A <see cref="PICRegistersBuilder"/> instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">Parameter <paramref name="pic"/> is null.</exception>
        public static PICRegisters Create(PIC pic)
        {
            if (pic is null)
                throw new ArgumentNullException(nameof(pic));
            registers = new PIC18Registers(pic);
            return registers;
        }

        #endregion

        #region Registers API - PIC18 specific

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

        /// <summary>
        /// Query if data memory absolute address corresponds to one of the MOVFFL forbidden destinations (per PIC data sheet)..
        /// </summary>
        /// <param name="dstaddr">The data memory absolute address.</param>
        /// <returns>
        /// True if forbidden, false allowed.
        /// </returns>
        public static bool NotAllowedMovlDest(ushort dstaddr)
            => invalidMovfflDests.Contains(GetRegisterBySizedAddr(PICDataAddress.Ptr(dstaddr), 8));

        #endregion

    }

}
