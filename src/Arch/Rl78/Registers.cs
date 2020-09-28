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

namespace Reko.Arch.Rl78
{
    public static class Registers
    {
        public static readonly RegisterStorage ax;
        public static readonly RegisterStorage bc;
        public static readonly RegisterStorage de;
        public static readonly RegisterStorage hl;
        public static readonly RegisterStorage sp;
        public static readonly RegisterStorage psw;
        public static readonly RegisterStorage es;
        public static readonly RegisterStorage cs;

        public static readonly RegisterStorage[] ByteRegs;

        public static readonly RegisterStorage x;
        public static readonly RegisterStorage a;
        public static readonly RegisterStorage c;
        public static readonly RegisterStorage b;
        public static readonly RegisterStorage e;
        public static readonly RegisterStorage d;
        public static readonly RegisterStorage l;
        public static readonly RegisterStorage h;

        public static readonly FlagGroupStorage cy;

        static Registers()
        {
            var f = new StorageFactory();
            ax = f.Reg16("ax");
            bc = f.Reg16("bc");
            de = f.Reg16("de");
            hl = f.Reg16("hl");
            sp = f.Reg16("sp");
            psw = f.Reg("psw", PrimitiveType.Byte);
            es = f.Reg("es", PrimitiveType.Byte);
            cs = f.Reg("cs", PrimitiveType.Byte);

            x = new RegisterStorage("x", ax.Number, 0, PrimitiveType.Byte);
            a = new RegisterStorage("a", ax.Number, 8, PrimitiveType.Byte);
            c = new RegisterStorage("c", ax.Number, 0, PrimitiveType.Byte);
            b = new RegisterStorage("b", ax.Number, 8, PrimitiveType.Byte);
            e = new RegisterStorage("e", ax.Number, 0, PrimitiveType.Byte);
            d = new RegisterStorage("d", ax.Number, 8, PrimitiveType.Byte);
            l = new RegisterStorage("l", ax.Number, 0, PrimitiveType.Byte);
            h = new RegisterStorage("h", ax.Number, 8, PrimitiveType.Byte);

            ByteRegs = new RegisterStorage[8] { x, a, c, b, e, d, l, h };

            cy = new FlagGroupStorage(psw, (uint) FlagM.CF, "cy", PrimitiveType.Bool);
        }
    }

    [Flags]
    public enum FlagM
    {
        CF = 1,
        AC = 0x10,
        ZF = 0x40, 
    }
}
