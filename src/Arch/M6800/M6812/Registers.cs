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
