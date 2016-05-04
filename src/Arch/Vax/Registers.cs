#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

namespace Reko.Arch.Vax
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
        public static RegisterStorage r10 = new RegisterStorage("r10", 10, 0, PrimitiveType.Word32);
        public static RegisterStorage r11 = new RegisterStorage("r11", 11, 0, PrimitiveType.Word32);

        public static RegisterStorage ap = new RegisterStorage("ap", 12, 0, PrimitiveType.Word32);
        public static RegisterStorage fp = new RegisterStorage("fp", 13, 0, PrimitiveType.Word32);
        public static RegisterStorage sp = new RegisterStorage("sp", 14, 0, PrimitiveType.Word32);
        public static RegisterStorage pc = new RegisterStorage("pc", 15, 0, PrimitiveType.Word32);

        public static readonly FlagRegister psw = new FlagRegister("psw", PrimitiveType.UInt32);
    }



    [Flags]
    public enum FlagM
    {
        NF = 8,
        ZF = 4,
        VF = 2,
        CF = 1,

        NZVC = NF|ZF|VF|CF
    }
}
