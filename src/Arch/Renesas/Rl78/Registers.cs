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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Arch.Renesas.Rl78
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
        public static readonly RegisterStorage[] WordRegs;

        public static readonly RegisterStorage x;
        public static readonly RegisterStorage a;
        public static readonly RegisterStorage c;
        public static readonly RegisterStorage b;
        public static readonly RegisterStorage e;
        public static readonly RegisterStorage d;
        public static readonly RegisterStorage l;
        public static readonly RegisterStorage h;

        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage cy;
        public static readonly FlagGroupStorage CZ;
        public static readonly FlagGroupStorage Z;

        public static readonly ReadOnlyDictionary<string, RegisterStorage> GpRegsByName;

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

            x = RegisterStorage.Reg8("x", ax.Number);
            a = RegisterStorage.Reg8("a", ax.Number, 8);
            c = RegisterStorage.Reg8("c", bc.Number);
            b = RegisterStorage.Reg8("b", bc.Number, 8);
            e = RegisterStorage.Reg8("e", de.Number);
            d = RegisterStorage.Reg8("d", de.Number, 8);
            l = RegisterStorage.Reg8("l", hl.Number);
            h = RegisterStorage.Reg8("h", hl.Number, 8);

            WordRegs = new RegisterStorage[5] { ax, bc, de, hl, sp};
            ByteRegs = new RegisterStorage[8] { x, a, c, b, e, d, l, h };

            GpRegsByName = new ReadOnlyDictionary<string, RegisterStorage>(WordRegs.Concat(ByteRegs)
                .ToDictionary(r => r.Name));

            C = new FlagGroupStorage(psw, (uint) FlagM.CF, nameof(C));
            cy = new FlagGroupStorage(psw, (uint) FlagM.CF, nameof(cy));
            CZ = new FlagGroupStorage(psw, (uint) (FlagM.CF | FlagM.ZF), nameof(CZ));
            Z = new FlagGroupStorage(psw, (uint) FlagM.ZF, nameof(Z));
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
