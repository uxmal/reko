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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.M6800.M6812
{
    public static class Registers
    {
        public static RegisterStorage d = new RegisterStorage("d", 0, 0, PrimitiveType.Word16);
        public static RegisterStorage a = new RegisterStorage("a", 0, 8, PrimitiveType.Byte);
        public static RegisterStorage b = new RegisterStorage("b", 0, 0, PrimitiveType.Byte);

        public static RegisterStorage x = new RegisterStorage("x", 1, 0, PrimitiveType.Word16);
        public static RegisterStorage y = new RegisterStorage("y", 2, 0, PrimitiveType.Word16);
        public static RegisterStorage sp = new RegisterStorage("sp", 3, 0, PrimitiveType.Word16);
        public static RegisterStorage pc = new RegisterStorage("pc", 4, 0, PrimitiveType.Word16);

        public static RegisterStorage ccr = new RegisterStorage("ccr", 5, 0, PrimitiveType.Byte);

        public static readonly FlagGroupStorage C = new FlagGroupStorage(ccr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
        public static readonly FlagGroupStorage V = new FlagGroupStorage(ccr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(ccr, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
        public static readonly FlagGroupStorage N = new FlagGroupStorage(ccr, (uint) FlagM.NF, "N", PrimitiveType.Bool);
        public static readonly FlagGroupStorage NV = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.VF), "NV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZ = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF), "NZ", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZC = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF | FlagM.CF), "NZC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZV = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF | FlagM.VF), "NZV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage NZVC = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF | FlagM.VF | FlagM.CF), "NZVC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage ZC = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.CF), "ZC", PrimitiveType.Byte);
        public static readonly FlagGroupStorage ZV = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.VF), "ZV", PrimitiveType.Byte);
        public static readonly FlagGroupStorage ZVC = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.VF | FlagM.CF), "ZVC", PrimitiveType.Byte);

    }

    [Flags]
    public enum FlagM : byte
    {
        CF = 1,             // carry
        VF = 2,             // overflow
        ZF = 4,             // zero
        NF = 8,             // sign
    }
}
