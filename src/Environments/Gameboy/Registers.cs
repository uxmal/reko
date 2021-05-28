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
using System.Text;

#pragma warning disable IDE1006

namespace Reko.Environments.Gameboy
{
    public static class Registers
    {
        public static RegisterStorage[] GpRegisters { get; }
        public static RegisterStorage af { get; }
        public static RegisterStorage bc { get; }
        public static RegisterStorage de { get; }
        public static RegisterStorage hl { get; }
        public static RegisterStorage sp { get; }

         public static RegisterStorage a { get; }
         public static RegisterStorage f { get; }
         public static RegisterStorage b { get; }
         public static RegisterStorage c { get; }
         public static RegisterStorage d { get; }
         public static RegisterStorage e { get; }
         public static RegisterStorage h { get; }
         public static RegisterStorage l { get; }

        public static FlagGroupStorage C { get; }
        public static FlagGroupStorage H { get; }
        public static FlagGroupStorage N { get; }
        public static FlagGroupStorage Z { get; }

        public static FlagGroupStorage HC { get; }

        public static FlagGroupStorage ZC { get; }
        public static FlagGroupStorage ZH { get; }
        public static FlagGroupStorage ZHC { get; }
        public static FlagGroupStorage ZNHC { get; }


        static Registers()
        {
            var factory = new StorageFactory();
            af = factory.Reg16("af");
            bc = factory.Reg16("bc");
            de = factory.Reg16("de");
            hl = factory.Reg16("hl");
            sp = factory.Reg16("sp");
            a = new RegisterStorage("a", af.Number, 8, PrimitiveType.Byte);
            f = new RegisterStorage("f", af.Number, 0, PrimitiveType.Byte);
            b = new RegisterStorage("b", bc.Number, 8, PrimitiveType.Byte);
            c = new RegisterStorage("c", bc.Number, 0, PrimitiveType.Byte);
            d = new RegisterStorage("d", de.Number, 8, PrimitiveType.Byte);
            e = new RegisterStorage("e", de.Number, 0, PrimitiveType.Byte);
            h = new RegisterStorage("h", hl.Number, 8, PrimitiveType.Byte);
            l = new RegisterStorage("l", hl.Number, 0, PrimitiveType.Byte);

            GpRegisters = new RegisterStorage[]
            {
                af,
                bc,
                de,
                hl,
                sp,
                a,
                f,
                b,
                c,
                d,
                e,
                h,
                l,
            };

            C = new FlagGroupStorage(af, (uint) FlagM.CF, "C", PrimitiveType.Bool);
            H = new FlagGroupStorage(af, (uint) FlagM.HF, "H", PrimitiveType.Bool);
            N = new FlagGroupStorage(af, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            Z = new FlagGroupStorage(af, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            HC = new FlagGroupStorage(af, (uint) (FlagM.HF | FlagM.CF), "HC", PrimitiveType.Byte);
            ZC = new FlagGroupStorage(af, (uint) (FlagM.ZF | FlagM.CF), "ZC", PrimitiveType.Byte);
            ZH = new FlagGroupStorage(af, (uint) (FlagM.ZF | FlagM.HF | FlagM.CF), "ZH", PrimitiveType.Byte);
            ZHC = new FlagGroupStorage(af, (uint) (FlagM.ZF | FlagM.HF | FlagM.CF), "ZHC", PrimitiveType.Byte);
            ZNHC = new FlagGroupStorage(af, (uint) (FlagM.ZF | FlagM.HF | FlagM.NF | FlagM.CF), "ZNHC", PrimitiveType.Byte);
        }

        [Flags]
        public enum FlagM
        {
            ZF = 0x80,
            NF = 0x40,
            HF = 0x20,
            CF = 0x10,
        }
    }
}
