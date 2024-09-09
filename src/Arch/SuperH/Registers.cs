#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using System.Collections.Generic;
using System.Linq;

namespace Reko.Arch.SuperH
{
    public static class Registers
    {
        public static readonly RegisterStorage r0 = RegisterStorage.Reg32("r0", 0);
        public static readonly RegisterStorage r1 = RegisterStorage.Reg32("r1", 1);
        public static readonly RegisterStorage r2 = RegisterStorage.Reg32("r2", 2);
        public static readonly RegisterStorage r3 = RegisterStorage.Reg32("r3", 3);

        public static readonly RegisterStorage r4 = RegisterStorage.Reg32("r4", 4);
        public static readonly RegisterStorage r5 = RegisterStorage.Reg32("r5", 5);
        public static readonly RegisterStorage r6 = RegisterStorage.Reg32("r6", 6);
        public static readonly RegisterStorage r7 = RegisterStorage.Reg32("r7", 7);

        public static readonly RegisterStorage r8 = RegisterStorage.Reg32("r8", 8);
        public static readonly RegisterStorage r9 = RegisterStorage.Reg32("r9", 9);
        public static readonly RegisterStorage r10 = RegisterStorage.Reg32("r10", 10);
        public static readonly RegisterStorage r11 = RegisterStorage.Reg32("r11", 11);

        public static readonly RegisterStorage r12 = RegisterStorage.Reg32("r12", 12);
        public static readonly RegisterStorage r13 = RegisterStorage.Reg32("r13", 13);
        public static readonly RegisterStorage r14 = RegisterStorage.Reg32("r14", 14);
        public static readonly RegisterStorage r15 = RegisterStorage.Reg32("r15", 15);

        public static readonly RegisterStorage fr0 = RegisterStorage.Reg32("fr0", 16, 0x000);
        public static readonly RegisterStorage fr1 = RegisterStorage.Reg32("fr1", 16, 0x020);
        public static readonly RegisterStorage fr2 = RegisterStorage.Reg32("fr2", 16, 0x040);
        public static readonly RegisterStorage fr3 = RegisterStorage.Reg32("fr3", 16, 0x060);

        public static readonly RegisterStorage fr4 = RegisterStorage.Reg32("fr4", 16, 0x080);
        public static readonly RegisterStorage fr5 = RegisterStorage.Reg32("fr5", 16, 0x0A0);
        public static readonly RegisterStorage fr6 = RegisterStorage.Reg32("fr6", 16, 0x0C0);
        public static readonly RegisterStorage fr7 = RegisterStorage.Reg32("fr7", 16, 0x0E0);

        public static readonly RegisterStorage fr8 = RegisterStorage.Reg32("fr8", 16,   0x100);
        public static readonly RegisterStorage fr9 = RegisterStorage.Reg32("fr9", 16,   0x120);
        public static readonly RegisterStorage fr10 = RegisterStorage.Reg32("fr10", 16, 0x140);
        public static readonly RegisterStorage fr11 = RegisterStorage.Reg32("fr11", 16, 0x160);

        public static readonly RegisterStorage fr12 = RegisterStorage.Reg32("fr12", 16, 0x180);
        public static readonly RegisterStorage fr13 = RegisterStorage.Reg32("fr13", 16, 0x1A0);
        public static readonly RegisterStorage fr14 = RegisterStorage.Reg32("fr14", 16, 0x1C0);
        public static readonly RegisterStorage fr15 = RegisterStorage.Reg32("fr15", 16, 0x1E0);

        public static readonly RegisterStorage dr0 = RegisterStorage.Reg64("dr0", 16, 0x000);
        public static readonly RegisterStorage dr2 = RegisterStorage.Reg64("dr2", 16, 0x040);
        public static readonly RegisterStorage dr4 = RegisterStorage.Reg64("dr4", 16, 0x080);
        public static readonly RegisterStorage dr6 = RegisterStorage.Reg64("dr6", 16, 0x0C0);

        public static readonly RegisterStorage dr8 =  RegisterStorage.Reg64("dr8", 16, 0x100);
        public static readonly RegisterStorage dr10 = RegisterStorage.Reg64("dr10", 16, 0x140);
        public static readonly RegisterStorage dr12 = RegisterStorage.Reg64("dr12", 16, 0x180);
        public static readonly RegisterStorage dr14 = RegisterStorage.Reg64("dr14", 16, 0x1C0);

        public static readonly RegisterStorage fv0 = new RegisterStorage("fv0", 16, 0x080, PrimitiveType.Word128);
        public static readonly RegisterStorage fv4 = new RegisterStorage("fv4", 16, 0x100, PrimitiveType.Word128);
        public static readonly RegisterStorage fv8 = new RegisterStorage("fv8", 16, 0x180, PrimitiveType.Word128);
        public static readonly RegisterStorage fv12 =new RegisterStorage("fv12", 16, 0x000, PrimitiveType.Word128);

        public static readonly RegisterStorage fpul = RegisterStorage.Reg32("fpul", 17);
        public static readonly RegisterStorage fpscr = RegisterStorage.Reg32("fpscr", 18);
        public static readonly RegisterStorage pr = RegisterStorage.Reg32("pr", 19);
        public static readonly RegisterStorage gbr = RegisterStorage.Reg32("gbr", 20);

        public static readonly RegisterStorage mac = RegisterStorage.Reg64("mac", 21);
        public static readonly RegisterStorage macl = RegisterStorage.Reg32("macl", 21);
        public static readonly RegisterStorage mach = RegisterStorage.Reg32("mach", 21, 32);

        public static readonly RegisterStorage sr = RegisterStorage.Reg32("sr", 22);
        public static readonly RegisterStorage tbr = RegisterStorage.Reg32("tbr", 23);
        public static readonly RegisterStorage vbr = RegisterStorage.Reg32("vbr", 24);
        public static readonly RegisterStorage spc = RegisterStorage.Reg32("spc", 25);
        public static readonly RegisterStorage mod = RegisterStorage.Reg32("mod", 26);
        public static readonly RegisterStorage rs = RegisterStorage.Reg32("rs", 27);
        public static readonly RegisterStorage dsr = RegisterStorage.Reg32("dsr", 28);
        public static readonly RegisterStorage dbr = RegisterStorage.Reg32("dbr", 29);
        public static readonly RegisterStorage ssr = RegisterStorage.Reg32("ssr", 30);
        public static readonly RegisterStorage sgr = RegisterStorage.Reg32("sgr", 31);

        public static readonly RegisterStorage r0_bank = RegisterStorage.Reg32("r0_bank", 32);
        public static readonly RegisterStorage r1_bank = RegisterStorage.Reg32("r1_bank", 33);
        public static readonly RegisterStorage r2_bank = RegisterStorage.Reg32("r2_bank", 34);
        public static readonly RegisterStorage r3_bank = RegisterStorage.Reg32("r3_bank", 35);
        public static readonly RegisterStorage r4_bank = RegisterStorage.Reg32("r4_bank", 36);
        public static readonly RegisterStorage r5_bank = RegisterStorage.Reg32("r5_bank", 37);
        public static readonly RegisterStorage r6_bank = RegisterStorage.Reg32("r6_bank", 38);
        public static readonly RegisterStorage r7_bank = RegisterStorage.Reg32("r7_bank", 39);
        public static readonly RegisterStorage r8_bank = RegisterStorage.Reg32("r8_bank", 40);
        public static readonly RegisterStorage r9_bank = RegisterStorage.Reg32("r9_bank", 41);
        public static readonly RegisterStorage r10_bank = RegisterStorage.Reg32("r10_bank", 42);
        public static readonly RegisterStorage r11_bank = RegisterStorage.Reg32("r11_bank", 43);
        public static readonly RegisterStorage r12_bank = RegisterStorage.Reg32("r12_bank", 44);
        public static readonly RegisterStorage r13_bank = RegisterStorage.Reg32("r13_bank", 45);
        public static readonly RegisterStorage r14_bank = RegisterStorage.Reg32("r14_bank", 46);
        public static readonly RegisterStorage r15_bank = RegisterStorage.Reg32("r15_bank", 47);

        public static readonly RegisterStorage xmtrx = new RegisterStorage("xmtrx", 48, 0, PrimitiveType.CreateWord(512));

        public static FlagGroupStorage T = new FlagGroupStorage(sr, 1, nameof(T));
        public static FlagGroupStorage S = new FlagGroupStorage(sr, 2, nameof(S));

        public static readonly RegisterStorage[] gpregs = new[]
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

        public static readonly RegisterStorage[] rbank = new[]
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


        public static readonly RegisterStorage[] fpregs = new[]
        {
             fr0,
             fr1,
             fr2,
             fr3,

             fr4,
             fr5,
             fr6,
             fr7,

             fr8,
             fr9,
             fr10,
             fr11,

             fr12,
             fr13,
             fr14,
             fr15,
        };

        public static readonly RegisterStorage[] dfpregs = new[]
        {
            dr0, dr2, dr4, dr6, dr8, dr10, dr12, dr14
        };

        public static readonly RegisterStorage[] vfpregs = new[]
        {
            fv0, fv4, fv8, fv12,
        };

        public static readonly RegisterStorage a0;
        public static readonly RegisterStorage a1;
        public static readonly RegisterStorage m0;
        public static readonly RegisterStorage m1;
        public static readonly RegisterStorage x0;
        public static readonly RegisterStorage x1;
        public static readonly RegisterStorage y0;
        public static readonly RegisterStorage y1;
        public static readonly RegisterStorage re;

        public static readonly Dictionary<StorageDomain, RegisterStorage> RegistersByDomain;

        public static RegisterStorage[] XdRegisters { get; }
        public static RegisterStorage[] XfRegisters { get; }
        public static RegisterStorage[] DspRegisters { get; }

        static Registers()
        {
            var factory = new StorageFactory(StorageDomain.Register + 0x60);
            XdRegisters = factory.RangeOfReg(8, i => $"xd{i * 2}", PrimitiveType.Word64);
            var n = XdRegisters[0].Number;
            XfRegisters = XdRegisters.SelectMany(r => new[]
            {
                new RegisterStorage($"xf{2*(r.Number-n)}", r.Number, 0, PrimitiveType.Word32),
                new RegisterStorage($"xf{2*(r.Number-n)+1}", r.Number, 32, PrimitiveType.Word32),
            }).ToArray();

            a0 = factory.Reg32("a0");
            a1 = factory.Reg32("a1");
            m0 = factory.Reg32("m0");
            m1 = factory.Reg32("m1");
            x0 = factory.Reg32("x0");
            x1 = factory.Reg32("x1");
            y0 = factory.Reg32("y0");
            y1 = factory.Reg32("y1");
            re = factory.Reg32("re");
            DspRegisters = new RegisterStorage[] { 
                a0,
                a1,
                x0,
                x1,
                y0,
                y1,
                re,
            };

            RegistersByDomain = gpregs.ToDictionary(r => r.Domain);
        }
    }
}
