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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Motorola.M6812
{
    public static class Registers
    {
        public static RegisterStorage d = RegisterStorage.Reg16("d", 0);
        public static RegisterStorage a = RegisterStorage.Reg8("a", 0, 8);
        public static RegisterStorage b = RegisterStorage.Reg8("b", 0);

        public static RegisterStorage x = RegisterStorage.Reg16("x", 1);
        public static RegisterStorage y = RegisterStorage.Reg16("y", 2);
        public static RegisterStorage sp = RegisterStorage.Reg16("sp", 3);
        public static RegisterStorage pc = RegisterStorage.Reg16("pc", 4);

        public static RegisterStorage ccr = RegisterStorage.Reg8("ccr", 5);

        public static readonly FlagGroupStorage C = new FlagGroupStorage(ccr, (uint) FlagM.CF, nameof(C));
        public static readonly FlagGroupStorage V = new FlagGroupStorage(ccr, (uint) FlagM.VF, nameof(V));
        public static readonly FlagGroupStorage Z = new FlagGroupStorage(ccr, (uint) FlagM.ZF, nameof(Z));
        public static readonly FlagGroupStorage N = new FlagGroupStorage(ccr, (uint) FlagM.NF, nameof(N));
        public static readonly FlagGroupStorage NV = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.VF), nameof(NV));
        public static readonly FlagGroupStorage NZ = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF), nameof(NZ));
        public static readonly FlagGroupStorage NZC = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF | FlagM.CF), nameof(NZC));
        public static readonly FlagGroupStorage NZV = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF | FlagM.VF), nameof(NZV));
        public static readonly FlagGroupStorage NZVC = new FlagGroupStorage(ccr, (uint) (FlagM.NF| FlagM.ZF | FlagM.VF | FlagM.CF), nameof(NZVC));
        public static readonly FlagGroupStorage ZC = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.CF), nameof(ZC));
        public static readonly FlagGroupStorage ZV = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.VF), nameof(ZV));
        public static readonly FlagGroupStorage ZVC = new FlagGroupStorage(ccr, (uint) (FlagM.ZF | FlagM.VF | FlagM.CF), nameof(ZVC));

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
