#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Blackfin
{
    public static class Registers
    {
        public static readonly RegisterStorage[] Data;
        public static readonly RegisterStorage[] Pointers;
        public static readonly RegisterStorage[] DataPointers;
        public static readonly RegisterStorage[] Indices;
        public static readonly RegisterStorage[] Bases;
        public static readonly RegisterStorage[] Ms;
        public static readonly RegisterStorage[] Ls;
        public static readonly RegisterStorage A0;
        public static readonly RegisterStorage A0_W;
        public static readonly RegisterStorage A0_X;
        public static readonly RegisterStorage A1;
        public static readonly RegisterStorage A1_W;
        public static readonly RegisterStorage A1_X;
        public static readonly RegisterStorage FP;
        public static readonly RegisterStorage SP;
        public static readonly RegisterStorage PC;
        public static readonly RegisterStorage ASTAT;
        public static readonly RegisterStorage RETS;
        public static readonly RegisterStorage LC0;
        public static readonly RegisterStorage LT0;
        public static readonly RegisterStorage LB0;
        public static readonly RegisterStorage LC1;
        public static readonly RegisterStorage LT1;
        public static readonly RegisterStorage LB1;
        public static readonly RegisterStorage CYCLES;
        public static readonly RegisterStorage CYCLES2;
        public static readonly RegisterStorage USP;
        public static readonly RegisterStorage SEQSTAT;
        public static readonly RegisterStorage SYSCFG;
        public static readonly RegisterStorage RETI;
        public static readonly RegisterStorage RETX;
        public static readonly RegisterStorage RETN;
        public static readonly RegisterStorage RETE;
        public static readonly RegisterStorage EMUDAT;

        public static readonly RegisterStorage[] AllReg;
        public static readonly RegisterStorage[] MostReg;

        public static readonly RegisterStorage[] RPIB;
        public static readonly RegisterStorage[] RPIB_Lo;
        public static readonly RegisterStorage[] RPI_Hi;

        public static readonly FlagGroupStorage AZ;         // 0
        public static readonly FlagGroupStorage AN;         // 1
        public static readonly FlagGroupStorage AC0_COPY;   // 2
        public static readonly FlagGroupStorage V_COPY;     // 3

        public static readonly FlagGroupStorage CC; //5
        public static readonly FlagGroupStorage AQ;

        public static readonly FlagGroupStorage RND_MOD; // 8

        public static readonly FlagGroupStorage AC0; // 12
        public static readonly FlagGroupStorage AC1; // 13

        public static readonly FlagGroupStorage AV0; // 16
        public static readonly FlagGroupStorage AV0S;
        public static readonly FlagGroupStorage AV1;
        public static readonly FlagGroupStorage AV1S;

        public static readonly FlagGroupStorage V; // 24
        public static readonly FlagGroupStorage VS;

        public static readonly FlagGroupStorage NZ;
        public static readonly FlagGroupStorage NZV;
        public static readonly FlagGroupStorage NZVC;

        public static Dictionary<uint, FlagGroupStorage> AStatFlags { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            Data = factory.RangeOfReg32(8, "R{0}");
            Pointers = factory.RangeOfReg32(8, "P{0}");
            Rename(ref Pointers[6], "SP");
            Rename(ref Pointers[7], "FP");
            SP = Pointers[6];
            FP = Pointers[7];
            DataPointers = Data.Concat(Pointers).ToArray();
            Indices = factory.RangeOfReg32(4, "I{0}");
            Ms = factory.RangeOfReg32(4, "M{0}");
            Bases = factory.RangeOfReg32(4, "B{0}");
            Ls = factory.RangeOfReg32(4, "L{0}");
            A0 = factory.Reg("A0", PrimitiveType.CreateWord(40));
            A1 = factory.Reg("A1", PrimitiveType.CreateWord(40));
            A0_W = RegisterStorage.Reg32("A0.W", A0.Number);
            A0_X = RegisterStorage.Reg8("A0.X", A0.Number, 32);
            A1_W = RegisterStorage.Reg32("A1.W", A1.Number);
            A1_X = RegisterStorage.Reg8("A1.X", A1.Number, 32);
            ASTAT = factory.Reg32("ASTAT");
            RETS = factory.Reg32("RETS");
            LC0 = factory.Reg32("LC0");
            LT0 = factory.Reg32("LT0");
            LB0 = factory.Reg32("LB0");
            LC1 = factory.Reg32("LC1");
            LT1 = factory.Reg32("LT1");
            LB1 = factory.Reg32("LB1");
            CYCLES = factory.Reg32("CYCLES");
            CYCLES2 = factory.Reg32("CYCLES2");
            USP = factory.Reg32("USP");
            SEQSTAT = factory.Reg32("SEQSTAT");
            SYSCFG = factory.Reg32("SYSCFG");
            RETI = factory.Reg32("RETI");
            RETX = factory.Reg32("RETX");
            RETN = factory.Reg32("RETN");
            RETE = factory.Reg32("RETE");
            EMUDAT = factory.Reg32("EMUDAT");

            PC = factory.Reg32("PC");

            RPIB = Data
                .Concat(Pointers)
                .Concat(Indices)
                .Concat(Ms)
                .Concat(Bases)
                .Concat(Ls)
                .ToArray();
            RPIB_Lo = RPIB
                .Select(r => RegisterStorage.Reg16(r.Name + ".L", r.Number, 0))
                .ToArray();
            RPI_Hi = Data
                .Concat(Pointers)
                .Concat(Indices)
                .Concat(Enumerable.Range(0, 8).Select(i => (RegisterStorage) null!))
                .Select(r => r is not null
                    ? RegisterStorage.Reg16(r.Name + ".H", r.Number, 16)
                    : null!)
                .ToArray();

            AllReg = MakeAllReg(true);
            MostReg = MakeAllReg(false);

            AZ = new FlagGroupStorage(ASTAT, 1 << 0, nameof(AZ));
            AN = new FlagGroupStorage(ASTAT, 1 << 1, nameof(AN));

            AC0_COPY = new FlagGroupStorage(ASTAT, 1 << 2, nameof(AC0_COPY));
            V_COPY = new FlagGroupStorage(ASTAT, 1 << 3, nameof(V_COPY));

            CC = new FlagGroupStorage(ASTAT, 1 << 5, nameof(CC));
            AQ = new FlagGroupStorage(ASTAT, 1 << 6, nameof(AQ));

            RND_MOD = new FlagGroupStorage(ASTAT, 1 << 8, nameof(RND_MOD));

            AC0 = new FlagGroupStorage(ASTAT, 1 << 12, nameof(AC0));
            AC1 = new FlagGroupStorage(ASTAT, 1 << 13, nameof(AC1));

            AV0 = new FlagGroupStorage(ASTAT, 1 << 16, nameof(AV0));
            AV0S = new FlagGroupStorage(ASTAT, 1 << 17, nameof(AV0S));
            AV1 = new FlagGroupStorage(ASTAT, 1 << 18, nameof(AV1));
            AV1S = new FlagGroupStorage(ASTAT, 1 << 19, nameof(AV1S));

            V = new FlagGroupStorage(ASTAT, 1 << 24, nameof(V));
            VS = new FlagGroupStorage(ASTAT, 1 << 25, nameof(VS));

            AStatFlags = new[]
            {
                AC0_COPY,
                V_COPY,

                CC,
                AQ,

                RND_MOD,

                AC0,
                AC1,

                AV0,
                AV0S,
                AV1,
                AV1S,

                V,
                VS
            }.ToDictionary(k => k.FlagGroupBits);

            NZ = new FlagGroupStorage(ASTAT, AN.FlagGroupBits | AZ.FlagGroupBits, nameof(NZ));
            NZV = new FlagGroupStorage(
                ASTAT,
                AN.FlagGroupBits |
                AZ.FlagGroupBits |
                V.FlagGroupBits,
                nameof(NZV));
            NZVC = new FlagGroupStorage(
                ASTAT,
                AN.FlagGroupBits |
                AZ.FlagGroupBits |
                V.FlagGroupBits |
                AC0.FlagGroupBits,
                nameof(NZVC));
        }

        private static RegisterStorage[] MakeAllReg(bool keepDataPtrRegs)
        {
            return Data.Select(d => keepDataPtrRegs ? d : null!)
                .Concat(Pointers.Select(p => keepDataPtrRegs ? p : null!))
                .Concat(Indices)
                .Concat(Ms)
                .Concat(Bases)
                .Concat(Ls)

                .Concat(new[] {
                    A0_X,
                    A0_W,
                    A1_X,
                    A1_W,
                    null!,
                    null!,
                    ASTAT,
                    RETS ,

                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,
                    null!,

                    LC0,
                    LT0,
                    LB0,
                    LC1,
                    LT1,
                    LB1,
                    CYCLES ,
                    CYCLES2,

                    USP,
                    SEQSTAT,
                    SYSCFG ,
                    RETI,
                    RETX,
                    RETN,
                    RETE,
                    EMUDAT,
                })
                .ToArray();
        }

        private static void Rename(ref RegisterStorage reg, string name)
        {
            var regNew = new RegisterStorage(name, reg.Number, (uint)reg.BitAddress, (PrimitiveType) reg.DataType);
            reg = regNew;
        }
    }
}
