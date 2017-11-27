#region License
/* 
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

using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Microchip.PIC18
{
    public static class Registers
    {

        /// <summary>STATUS register. </summary>
        public static readonly FlagRegister status = new FlagRegister("STATUS", 0x3FD8, PrimitiveType.Byte);

        /// <summary>Carry bit in STATUS register. </summary>
        public static readonly FlagGroupStorage C = new FlagGroupStorage(status, (uint) FlagM.C, "C", PrimitiveType.Bool);

        /// <summary>Digit-Carry bit in STATUS register.. </summary>
        public static readonly FlagGroupStorage DC = new FlagGroupStorage(status, (uint)FlagM.DC, "DC", PrimitiveType.Bool);

        /// <summary>Zero bit in STATUS register.. </summary>
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(status, (uint)FlagM.Z, "Z", PrimitiveType.Bool);

        /// <summary>Overflow bit in STATUS register.. </summary>
        public static readonly FlagGroupStorage OV = new FlagGroupStorage(status, (uint)FlagM.OV, "OV", PrimitiveType.Bool);

        /// <summary>Negative bit in STATUS register.. </summary>
        public static readonly FlagGroupStorage N = new FlagGroupStorage(status, (uint)FlagM.N, "N", PrimitiveType.Bool);

        /// <summary>Power-down bit in STATUS register.. </summary>
        public static readonly FlagGroupStorage PD = new FlagGroupStorage(status, (uint)FlagM.PD, "PD", PrimitiveType.Bool);

        /// <summary>Time-out bit in STATUS register.. </summary>
        public static readonly FlagGroupStorage TO = new FlagGroupStorage(status, (uint)FlagM.TO, "TO", PrimitiveType.Bool);



        /// <summary>FSR2L special function register. </summary>
        public static readonly RegisterStorage fsr2l = new RegisterStorage("FSR2L", 0x3FD9, 0, PrimitiveType.Byte);

        /// <summary>FSR2H special function register. </summary>
        public static readonly RegisterStorage fsr2h = new RegisterStorage("FSR2H", 0x3FDA, 0, PrimitiveType.Byte);

        /// <summary>PLUSW2 special function register. </summary>
        public static readonly RegisterStorage plusw2 = new RegisterStorage("PLUSW2", 0x3FDB, 0, PrimitiveType.Byte);

        /// <summary>PREINC2 special function register. </summary>
        public static readonly RegisterStorage preinc2 = new RegisterStorage("PREINC2", 0x3FDC, 0, PrimitiveType.Byte);

        /// <summary>POSTDEC2 special function register. </summary>
        public static readonly RegisterStorage postdec2 = new RegisterStorage("POSTDEC2", 0x3FDD, 0, PrimitiveType.Byte);

        /// <summary>POSTINC2 special function register. </summary>
        public static readonly RegisterStorage postinc2 = new RegisterStorage("POSTINC2", 0x3FDE, 0, PrimitiveType.Byte);

        /// <summary>INDF2 special function register. </summary>
        public static readonly RegisterStorage indf2 = new RegisterStorage("INDF2", 0x3FDF, 0, PrimitiveType.Byte);

        /// <summary>BSR special function register. </summary>
        public static readonly RegisterStorage bsr = new RegisterStorage("BSR", 0x3FE0, 0, PrimitiveType.Byte);

        /// <summary>FSR1L special function register. </summary>
        public static readonly RegisterStorage fsr1l = new RegisterStorage("FSR1L", 0x3FE1, 0, PrimitiveType.Byte);

        /// <summary>FSR1H special function register. </summary>
        public static readonly RegisterStorage fsr1h = new RegisterStorage("FSR1H", 0x3FE2, 0, PrimitiveType.Byte);

        /// <summary>PLUSW1 special function register. </summary>
        public static readonly RegisterStorage plusw1 = new RegisterStorage("PLUSW1", 0x3FE3, 0, PrimitiveType.Byte);

        /// <summary>PREINC1 special function register. </summary>
        public static readonly RegisterStorage preinc1 = new RegisterStorage("PREINC1", 0x3FE4, 0, PrimitiveType.Byte);

        /// <summary>POSTDEC1 special function register. </summary>
        public static readonly RegisterStorage postdec1 = new RegisterStorage("POSTDEC1", 0x3FE5, 0, PrimitiveType.Byte);

        /// <summary>POSTINC1 special function register. </summary>
        public static readonly RegisterStorage postinc1 = new RegisterStorage("POSTINC1", 0x3FE6, 0, PrimitiveType.Byte);

        /// <summary>INDF1 special function register. </summary>
        public static readonly RegisterStorage indf1 = new RegisterStorage("INDF1", 0x3FE7, 0, PrimitiveType.Byte);

        /// <summary>WREG special function register. </summary>
        public static readonly RegisterStorage wreg = new RegisterStorage("WREG", 0x3FE8, 0, PrimitiveType.Byte);

        /// <summary>FSR0L special function register. </summary>
        public static readonly RegisterStorage fsr0l = new RegisterStorage("FSR0L", 0x3FE9, 0, PrimitiveType.Byte);

        /// <summary>FSR0H special function register. </summary>
        public static readonly RegisterStorage fsr0h = new RegisterStorage("FSR0H", 0x3FEA, 0, PrimitiveType.Byte);

        /// <summary>PLUSW0 special function register. </summary>
        public static readonly RegisterStorage plusw0 = new RegisterStorage("PLUSW0", 0x3FEB, 0, PrimitiveType.Byte);

        /// <summary>PREINC0 special function register. </summary>
        public static readonly RegisterStorage preinc0 = new RegisterStorage("PREINC0", 0x3FEC, 0, PrimitiveType.Byte);

        /// <summary>POSTDEC0 special function register. </summary>
        public static readonly RegisterStorage postdec0 = new RegisterStorage("POSTDEC0", 0x3FED, 0, PrimitiveType.Byte);

        /// <summary>POSTINC0 special function register. </summary>
        public static readonly RegisterStorage postinc0 = new RegisterStorage("POSTINC0", 0x3FEE, 0, PrimitiveType.Byte);

        /// <summary>INDF0 special function register. </summary>
        public static readonly RegisterStorage indf0 = new RegisterStorage("INDF0", 0x3FEF, 0, PrimitiveType.Byte);


        /// <summary>PRODL special function register. </summary>
        public static readonly RegisterStorage prodl = new RegisterStorage("PRODL", 0x3FF3, 0, PrimitiveType.Byte);

        /// <summary>PRODH special function register. </summary>
        public static readonly RegisterStorage prodh = new RegisterStorage("PRODH", 0x3FF4, 0, PrimitiveType.Byte);

        /// <summary>TABLAT special function register. </summary>
        public static readonly RegisterStorage tablat = new RegisterStorage("TABLAT", 0x3FF5, 0, PrimitiveType.Byte);

        /// <summary>TBLPTRL special function register. </summary>
        public static readonly RegisterStorage tblptrl = new RegisterStorage("TBLPTRL", 0x3FF6, 0, PrimitiveType.Byte);

        /// <summary>TBLPTRH special function register. </summary>
        public static readonly RegisterStorage tblptrh = new RegisterStorage("TBLPTRH", 0x3FF7, 0, PrimitiveType.Byte);

        /// <summary>TBLPTRU special function register. </summary>
        public static readonly RegisterStorage tblptru = new RegisterStorage("TBLPTRU", 0x3FF8, 0, PrimitiveType.Byte);

        /// <summary>PCL special function register. </summary>
        public static readonly RegisterStorage pcl = new RegisterStorage("PCL", 0x3FF9, 0, PrimitiveType.Byte);

        /// <summary>PCLH special function register. </summary>
        public static readonly RegisterStorage pclh = new RegisterStorage("PCLH", 0x3FFA, 0, PrimitiveType.Byte);

        /// <summary>PCLU special function register. </summary>
        public static readonly RegisterStorage pclu = new RegisterStorage("PCLU", 0x3FFB, 0, PrimitiveType.Byte);

        /// <summary>STKPTR special function register. </summary>
        public static readonly RegisterStorage stkptr = new RegisterStorage("STKPTR", 0x3FFC, 0, PrimitiveType.Byte);

        /// <summary>TOSL special function register. </summary>
        public static readonly RegisterStorage tosl = new RegisterStorage("TOSL", 0x3FFD, 0, PrimitiveType.Byte);

        /// <summary>TOSH special function register. </summary>
        public static readonly RegisterStorage tosh = new RegisterStorage("TOSH", 0x3FFE, 0, PrimitiveType.Byte);

        /// <summary>TOSU special function register. </summary>
        public static readonly RegisterStorage tosu = new RegisterStorage("TOSU", 0x3FFF, 0, PrimitiveType.Byte);

        internal static readonly Dictionary<string, RegisterStorage> RegsByName;
        internal static readonly Dictionary<Address, RegisterStorage> RegsByAddr;

        static Registers()
        {
            RegsByName = new[]
            {
                status,
                fsr2l, fsr2h, plusw2, preinc2, postdec2, postinc2, indf2, bsr,
                fsr1l, fsr1h, plusw1, preinc1, postdec1, postinc1, indf1, wreg,
                fsr0l, fsr0h, plusw0, preinc0, postdec0, postinc0, indf0,
                prodl, prodh, tablat, tblptrl, tblptrh, tblptru,
                pcl, pclh, pclu, stkptr, tosl, tosh, tosu,
            }.ToDictionary(r => r.Name);

            RegsByAddr = new[]
            {
                status,
                fsr2l, fsr2h, plusw2, preinc2, postdec2, postinc2, indf2, bsr,
                fsr1l, fsr1h, plusw1, preinc1, postdec1, postinc1, indf1, wreg,
                fsr0l, fsr0h, plusw0, preinc0, postdec0, postinc0, indf0,
                prodl, prodh, tablat, tblptrl, tblptrh, tblptru,
                pcl, pclh, pclu, stkptr, tosl, tosh, tosu,
            }.ToDictionary(r => Address.Ptr32((uint)r.Number));
        }

        public static RegisterStorage GetRegister(string name)
        {
            RegisterStorage reg;
            if (RegsByName.TryGetValue(name, out reg))
                return reg;
            return RegisterStorage.None;
        }

        public static RegisterStorage GetRegister(Address address)
        {
            RegisterStorage reg;
            if (RegsByAddr.TryGetValue(address, out reg))
                return reg;
            return RegisterStorage.None;
        }

        public static RegisterStorage GetRegister(int i)
        {
            RegisterStorage reg;
            if (RegsByAddr.TryGetValue(Address.Ptr32((uint)i), out reg))
                return reg;
            return RegisterStorage.None;
        }

        public static int Max
        {
            get { return RegsByAddr.Count; }
        }
    }

}
