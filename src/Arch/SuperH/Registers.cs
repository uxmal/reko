#region License
/* 
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.SuperH
{
    public static class Registers
    {
        public static RegisterStorage r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word32);
        public static RegisterStorage r1 = new RegisterStorage("r1", 1, 0, PrimitiveType.Word32);
        public static RegisterStorage r2 = new RegisterStorage("r2", 2, 0, PrimitiveType.Word32);
        public static RegisterStorage r3 = new RegisterStorage("r3", 3, 0, PrimitiveType.Word32);

        public static RegisterStorage r4 = new RegisterStorage("r4", 4, 0, PrimitiveType.Word32);
        public static RegisterStorage r5 = new RegisterStorage("r5", 5, 0, PrimitiveType.Word32);
        public static RegisterStorage r6 = new RegisterStorage("r6", 6, 0, PrimitiveType.Word32);
        public static RegisterStorage r7 = new RegisterStorage("r7", 7, 0, PrimitiveType.Word32);

        public static RegisterStorage r8 = new RegisterStorage("r8", 8, 0, PrimitiveType.Word32);
        public static RegisterStorage r9 = new RegisterStorage("r9", 9, 0, PrimitiveType.Word32);
        public static RegisterStorage r10 = new RegisterStorage("r10", 10, 10, PrimitiveType.Word32);
        public static RegisterStorage r11 = new RegisterStorage("r11", 11, 11, PrimitiveType.Word32);

        public static RegisterStorage r12 = new RegisterStorage("r12", 12, 12, PrimitiveType.Word32);
        public static RegisterStorage r13 = new RegisterStorage("r13", 13, 13, PrimitiveType.Word32);
        public static RegisterStorage r14 = new RegisterStorage("r14", 14, 14, PrimitiveType.Word32);
        public static RegisterStorage r15 = new RegisterStorage("r15", 15, 15, PrimitiveType.Word32);

        public static RegisterStorage fr0 = new RegisterStorage("fr0", 16, 0x000, PrimitiveType.Word32);
        public static RegisterStorage fr1 = new RegisterStorage("fr1", 16, 0x020, PrimitiveType.Word32);
        public static RegisterStorage fr2 = new RegisterStorage("fr2", 16, 0x040, PrimitiveType.Word32);
        public static RegisterStorage fr3 = new RegisterStorage("fr3", 16, 0x060, PrimitiveType.Word32);

        public static RegisterStorage fr4 = new RegisterStorage("fr4", 16, 0x080, PrimitiveType.Word32);
        public static RegisterStorage fr5 = new RegisterStorage("fr5", 16, 0x0A0, PrimitiveType.Word32);
        public static RegisterStorage fr6 = new RegisterStorage("fr6", 16, 0x0C0, PrimitiveType.Word32);
        public static RegisterStorage fr7 = new RegisterStorage("fr7", 16, 0x0E0, PrimitiveType.Word32);

        public static RegisterStorage fr8 = new RegisterStorage("fr8", 16,   0x100, PrimitiveType.Word32);
        public static RegisterStorage fr9 = new RegisterStorage("fr9", 16,   0x120, PrimitiveType.Word32);
        public static RegisterStorage fr10 = new RegisterStorage("fr10", 16, 0x140, PrimitiveType.Word32);
        public static RegisterStorage fr11 = new RegisterStorage("fr11", 16, 0x160, PrimitiveType.Word32);

        public static RegisterStorage fr12 = new RegisterStorage("fr12", 16, 0x180, PrimitiveType.Word32);
        public static RegisterStorage fr13 = new RegisterStorage("fr13", 16, 0x1A0, PrimitiveType.Word32);
        public static RegisterStorage fr14 = new RegisterStorage("fr14", 16, 0x1C0, PrimitiveType.Word32);
        public static RegisterStorage fr15 = new RegisterStorage("fr15", 16, 0x1E0, PrimitiveType.Word32);

        public static RegisterStorage dr0 = new RegisterStorage("dr0", 16, 0x000, PrimitiveType.Word64);
        public static RegisterStorage dr2 = new RegisterStorage("dr2", 16, 0x040, PrimitiveType.Word64);
        public static RegisterStorage dr4 = new RegisterStorage("dr4", 16, 0x080, PrimitiveType.Word64);
        public static RegisterStorage dr6 = new RegisterStorage("dr6", 16, 0x0C0, PrimitiveType.Word64);

        public static RegisterStorage dr8 = new RegisterStorage("dr8",   16, 0x100, PrimitiveType.Word64);
        public static RegisterStorage dr10 = new RegisterStorage("dr10", 16, 0x140, PrimitiveType.Word64);
        public static RegisterStorage dr12 = new RegisterStorage("dr12", 16, 0x180, PrimitiveType.Word64);
        public static RegisterStorage dr14 = new RegisterStorage("dr14", 16, 0x1C0, PrimitiveType.Word64);

        public static RegisterStorage fv0 = new RegisterStorage("fv0", 16, 0x080, PrimitiveType.Word128);
        public static RegisterStorage fv4 = new RegisterStorage("fv4", 16, 0x100, PrimitiveType.Word128);
        public static RegisterStorage fv8 = new RegisterStorage("fv8", 16, 0x180, PrimitiveType.Word128);
        public static RegisterStorage fv12 = new RegisterStorage("fv12", 16, 0x000, PrimitiveType.Word128);

        public static RegisterStorage fpul = new RegisterStorage("fpul", 17, 0, PrimitiveType.Word32);
        public static RegisterStorage fpscr = new RegisterStorage("fpscr", 18, 0, PrimitiveType.Word32);
        public static RegisterStorage pr = new RegisterStorage("pr", 19, 0, PrimitiveType.Word32);
        public static RegisterStorage gbr = new RegisterStorage("gbr", 20, 0, PrimitiveType.Word32);
        public static RegisterStorage mac = new RegisterStorage("mac", 21, 0, PrimitiveType.Word64);
        public static RegisterStorage macl = new RegisterStorage("macl", 21, 0, PrimitiveType.Word32);
        public static RegisterStorage mach = new RegisterStorage("mach", 21, 32, PrimitiveType.Word32);
        public static RegisterStorage sr = new RegisterStorage("sr", 22, 0, PrimitiveType.Word32);
        public static RegisterStorage tbr = new RegisterStorage("tbr", 23, 0, PrimitiveType.Word32);
        public static RegisterStorage vbr = new RegisterStorage("vbr", 24, 0, PrimitiveType.Word32);
        public static RegisterStorage spc = new RegisterStorage("spc", 25, 0, PrimitiveType.Word32);
        public static RegisterStorage mod = new RegisterStorage("mod", 26, 0, PrimitiveType.Word32);
        public static RegisterStorage rs = new RegisterStorage("rs", 27, 0, PrimitiveType.Word32);
        public static RegisterStorage dsr = new RegisterStorage("dsr", 28, 0, PrimitiveType.Word32);
        public static RegisterStorage dbr = new RegisterStorage("dbr", 29, 0, PrimitiveType.Word32);
        public static RegisterStorage ssr = new RegisterStorage("ssr", 30, 0, PrimitiveType.Word32);
        public static RegisterStorage sgr = new RegisterStorage("sgr", 31, 0, PrimitiveType.Word32);

        public static RegisterStorage r0_bank = new RegisterStorage("r0_bank", 32, 0, PrimitiveType.Word32);
        public static RegisterStorage r1_bank = new RegisterStorage("r1_bank", 33, 0, PrimitiveType.Word32);
        public static RegisterStorage r2_bank = new RegisterStorage("r2_bank", 34, 0, PrimitiveType.Word32);
        public static RegisterStorage r3_bank = new RegisterStorage("r3_bank", 35, 0, PrimitiveType.Word32);
        public static RegisterStorage r4_bank = new RegisterStorage("r4_bank", 36, 0, PrimitiveType.Word32);
        public static RegisterStorage r5_bank = new RegisterStorage("r5_bank", 37, 0, PrimitiveType.Word32);
        public static RegisterStorage r6_bank = new RegisterStorage("r6_bank", 38, 0, PrimitiveType.Word32);
        public static RegisterStorage r7_bank = new RegisterStorage("r7_bank", 39, 0, PrimitiveType.Word32);
        public static RegisterStorage r8_bank = new RegisterStorage("r8_bank", 40, 0, PrimitiveType.Word32);
        public static RegisterStorage r9_bank = new RegisterStorage("r9_bank", 41, 0, PrimitiveType.Word32);
        public static RegisterStorage r10_bank = new RegisterStorage("r10_bank", 42, 0, PrimitiveType.Word32);
        public static RegisterStorage r11_bank = new RegisterStorage("r11_bank", 43, 0, PrimitiveType.Word32);
        public static RegisterStorage r12_bank = new RegisterStorage("r12_bank", 44, 0, PrimitiveType.Word32);
        public static RegisterStorage r13_bank = new RegisterStorage("r13_bank", 45, 0, PrimitiveType.Word32);
        public static RegisterStorage r14_bank = new RegisterStorage("r14_bank", 46, 0, PrimitiveType.Word32);
        public static RegisterStorage r15_bank = new RegisterStorage("r15_bank", 47, 0, PrimitiveType.Word32);

        public static RegisterStorage xmtrx = new RegisterStorage("xmtrx", 48, 0, PrimitiveType.CreateWord(512));

        public static FlagGroupStorage T = new FlagGroupStorage(sr, 1, "T", PrimitiveType.Bool);
        public static FlagGroupStorage S = new FlagGroupStorage(sr, 2, "S", PrimitiveType.Bool);

        public static RegisterStorage[] gpregs = new[]
        {
             r0,
             r1,
             r2,
             r3,

             r4,
             r5,
             r6,
             r7,

             r8 ,
             r9 ,
             r10,
             r11,

             r12 ,
             r13 ,
             r14 ,
             r15 ,
        };

        public static RegisterStorage[] rbank = new[]
        {
             r0_bank,
             r1_bank,
             r2_bank,
             r3_bank,

             r4_bank,
             r5_bank,
             r6_bank,
             r7_bank,

             r8_bank,
             r9_bank,
             r10_bank,
             r11_bank,

             r12_bank,
             r13_bank,
             r14_bank,
             r15_bank,
        };


        public static RegisterStorage[] fpregs = new[]
        {
             fr0,
             fr1,
             fr2,
             fr3,

             fr4,
             fr5,
             fr6,
             fr7,

             fr8 ,
             fr9 ,
             fr10,
             fr11,

             fr12 ,
             fr13 ,
             fr14 ,
             fr15 ,
        };

        public static RegisterStorage[] dfpregs = new[]
        {
            dr0, dr2, dr4, dr6, dr8, dr10, dr12, dr14
        };

        public static RegisterStorage[] vfpregs = new[]
        {
            fv0, fv4, fv8, fv12,
        };
    }
}
