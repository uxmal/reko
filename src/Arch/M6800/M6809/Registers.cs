#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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

namespace Reko.Arch.M6800.M6809
{
    public class Registers
    {
        public static RegisterStorage X { get; }
        public static RegisterStorage Y { get; }
        public static RegisterStorage U { get; }
        public static RegisterStorage S { get; }
        public static RegisterStorage D { get; }
        public static RegisterStorage PCR { get; }
        public static RegisterStorage DP { get; }
        public static RegisterStorage CC { get; }

        public static RegisterStorage A { get; }
        public static RegisterStorage B { get; }

        public static readonly RegisterStorage[] AddrRegs;

        public static readonly FlagGroupStorage N;
        public static readonly FlagGroupStorage Z;
        public static readonly FlagGroupStorage V;
        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage NV;
        public static readonly FlagGroupStorage NZ;
        public static readonly FlagGroupStorage NZC;
        public static readonly FlagGroupStorage NZV;
        public static readonly FlagGroupStorage NZVC;
        public static readonly FlagGroupStorage ZC;

        static Registers()
        {
            var factory = new StorageFactory();
            X = factory.Reg16("x");
            Y = factory.Reg16("y");
            U = factory.Reg16("u");
            S = factory.Reg16("s");
            D = factory.Reg16("d");
            PCR = factory.Reg("pcr", PrimitiveType.Ptr16);
            DP = factory.Reg("dp", PrimitiveType.Byte);
            CC = factory.Reg("cc", PrimitiveType.Byte);

            A = new RegisterStorage("a", D.Number, 8, PrimitiveType.Byte);
            B = new RegisterStorage("b", D.Number, 0, PrimitiveType.Byte);

            AddrRegs = new RegisterStorage[] { X, Y, U, S };
            
            N = new FlagGroupStorage(CC, (uint)FlagM.N, "N", PrimitiveType.Bool); 
            Z = new FlagGroupStorage(CC, (uint)FlagM.Z, "Z", PrimitiveType.Bool); 
            V = new FlagGroupStorage(CC, (uint)FlagM.V, "V", PrimitiveType.Bool);
            C = new FlagGroupStorage(CC, (uint)FlagM.C, "C", PrimitiveType.Bool);

            NV = new FlagGroupStorage(CC, (uint) (FlagM.N| FlagM.V), "NV", PrimitiveType.Byte);
            NZ = new FlagGroupStorage(CC, (uint) (FlagM.N| FlagM.Z), "NZ", PrimitiveType.Byte);
            NZC = new FlagGroupStorage(CC, (uint) (FlagM.N| FlagM.Z| FlagM.C), "NZC", PrimitiveType.Byte);
            NZV = new FlagGroupStorage(CC, (uint) (FlagM.N| FlagM.Z| FlagM.V), "NZV", PrimitiveType.Byte);
            NZVC = new FlagGroupStorage(CC, (uint) (FlagM.N| FlagM.Z| FlagM.V|FlagM.C), "NZVC", PrimitiveType.Byte);
            ZC = new FlagGroupStorage(CC, (uint) (FlagM.Z|FlagM.C), "ZC", PrimitiveType.Byte);

        }
    }

    [Flags]
    public enum FlagM
    {
        E = 128,
        F = 64,
        H = 32,
        I = 16,
        N = 8,
        Z = 4,
        V = 2,
        C = 1,
    }
}