#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Arch.Avr.Avr8
{
    public static class Registers
    {
        public static readonly RegisterStorage[] regs;
        public static List<(FlagM, char)> grfToString;

        static Registers()
        {
            var factory = new StorageFactory();
            ByteRegs = factory.RangeOfReg(32, i => $"r{i}", PrimitiveType.Byte);

            sreg = RegisterStorage.Reg8("sreg", 36);
            code = new RegisterStorage("code", 100, 0, PrimitiveType.SegmentSelector);
            StackRegister = RegisterStorage.Reg16("SP", 0x3D);
            x = factory.Reg("x", PrimitiveType.Word16);
            y = factory.Reg("y", PrimitiveType.Word16);
            z = factory.Reg("z", PrimitiveType.Word16);
            rampx = factory.Reg("rampx", PrimitiveType.Byte);
            rampy = factory.Reg("rampy", PrimitiveType.Byte);
            rampz = factory.Reg("rampz", PrimitiveType.Byte);
            ByteRegs[30] = RegisterStorage.Reg8("r30", z.Number);
            ByteRegs[31] = RegisterStorage.Reg8("r31", z.Number, 8);
            regs =
                ByteRegs
                .Concat(new[] {
                    x, y, z,
                    rampx, rampy, rampz, 
                    sreg
                }).ToArray();

            I = new FlagGroupStorage(sreg, (uint) FlagM.IF, nameof(I));
            T = new FlagGroupStorage(sreg, (uint) FlagM.TF, nameof(T));
            H = new FlagGroupStorage(sreg, (uint) FlagM.HF, nameof(H));
            V = new FlagGroupStorage(sreg, (uint) FlagM.VF, nameof(V));
            N = new FlagGroupStorage(sreg, (uint) FlagM.NF, nameof(N));
            C = new FlagGroupStorage(sreg, (uint) FlagM.CF, nameof(C));
            Z = new FlagGroupStorage(sreg, (uint) FlagM.ZF, nameof(Z));
            HSVNZC = new FlagGroupStorage(sreg, (uint) (FlagM.HF | FlagM.SF | FlagM.VF | FlagM.NF | FlagM.ZF | FlagM.CF), nameof(HSVNZC));
            SNZ = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.NF | FlagM.ZF), nameof(SNZ));
            SNZC = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.NF | FlagM.ZF | FlagM.CF), nameof(SNZC));
            SNZV = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.NF | FlagM.ZF | FlagM.VF), nameof(SNZV));
            SVNC = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.VF | FlagM.NF | FlagM.CF), nameof(SVNC));
            SVNZ = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.VF | FlagM.NF | FlagM.ZF), nameof(SVNZ));
            SVNZC = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.VF | FlagM.NF | FlagM.ZF | FlagM.CF), nameof(SVNZC));
            SVZC = new FlagGroupStorage(sreg, (uint) (FlagM.SF | FlagM.VF | FlagM.ZF | FlagM.CF), nameof(SVZC));
            VN = new FlagGroupStorage(sreg, (uint) (FlagM.VF | FlagM.NF), nameof(VN));
            grfToString = new List<(FlagM, char)>
            {
                (FlagM.IF, 'I'),
                (FlagM.TF, 'T'),
                (FlagM.HF, 'H'),
                (FlagM.SF, 'S'),
                (FlagM.VF, 'V'),
                (FlagM.NF, 'N'),
                (FlagM.ZF, 'Z'),
                (FlagM.CF, 'C'),
            };
        }

        public static RegisterStorage sreg { get; private set; }
        public static RegisterStorage x { get; }
        public static RegisterStorage y { get; }
        public static RegisterStorage z { get; }
        public static RegisterStorage rampx { get; }
        public static RegisterStorage rampy { get; }
        public static RegisterStorage rampz { get; }
        public static RegisterStorage code { get; }
        public static RegisterStorage StackRegister { get; }

        public static FlagGroupStorage I { get; }
        public static FlagGroupStorage T { get; }
        public static FlagGroupStorage H { get; }
        public static FlagGroupStorage V { get; }
        public static FlagGroupStorage N { get; }
        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage Z { get; }
        public static FlagGroupStorage HSVNZC { get; }
        public static FlagGroupStorage SNZC { get; }
        public static FlagGroupStorage SNZ { get; }
        public static FlagGroupStorage SNZV { get; }
        public static FlagGroupStorage SVNZ { get; }
        public static FlagGroupStorage SVNC { get; }
        public static FlagGroupStorage SVNZC { get; }
        public static FlagGroupStorage SVZC { get; }
        public static FlagGroupStorage VN { get; }

        public static RegisterStorage[] ByteRegs { get; }
    }
}
