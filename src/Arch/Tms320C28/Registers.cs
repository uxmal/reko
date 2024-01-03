#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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

namespace Reko.Arch.Tms320C28
{
    public static class Registers
    {
        public static RegisterStorage  xt { get; } 
        public static RegisterStorage  p { get; } 
        public static RegisterStorage  acc { get; } 
        public static RegisterStorage  sp { get; } 
        public static RegisterStorage  dp { get; } 
        public static RegisterStorage  xar0 { get; }
        public static RegisterStorage  xar1 { get; }
        public static RegisterStorage  xar2 { get; }
        public static RegisterStorage  xar3 { get; }
        public static RegisterStorage  xar4 { get; }
        public static RegisterStorage  xar5 { get; }
        public static RegisterStorage  xar6 { get; }
        public static RegisterStorage  xar7 { get; }
        public static RegisterStorage  pc { get; } 
        public static RegisterStorage  rpc { get; }
        public static RegisterStorage  st0 { get; }
        public static RegisterStorage  st1 { get; }
        public static RegisterStorage  ier { get; }
        public static RegisterStorage  dbgier { get; }
        public static RegisterStorage  ifr { get; }

       public static RegisterStorage t { get; }
       public static RegisterStorage tl { get; }
       public static RegisterStorage ph { get; }
       public static RegisterStorage pl { get; }
       public static RegisterStorage ah { get; }
       public static RegisterStorage al { get; }
       public static RegisterStorage ar0h { get; }
       public static RegisterStorage ar0l { get; }
       public static RegisterStorage ar1h { get; }
       public static RegisterStorage ar1l { get; }
       public static RegisterStorage ar2h { get; }
       public static RegisterStorage ar2l { get; }
       public static RegisterStorage ar3h { get; }
       public static RegisterStorage ar3l { get; }
       public static RegisterStorage ar4h { get; }
       public static RegisterStorage ar4l { get; }
       public static RegisterStorage ar5h { get; }
       public static RegisterStorage ar5l { get; }
       public static RegisterStorage ar6h { get; }
       public static RegisterStorage ar6l { get; }
       public static RegisterStorage ar7h { get; }
       public static RegisterStorage ar7l { get; }

        static Registers()
        {
            var factory = new StorageFactory();
            xt = factory.Reg32("xt");
            p = factory.Reg32("p");
            acc = factory.Reg32("acc");
            sp = factory.Reg16("sp");
            dp = factory.Reg16("dp");
            xar0 = factory.Reg16("xar0");
            xar1 = factory.Reg16("xar1");
            xar2 = factory.Reg16("xar2");
            xar3 = factory.Reg16("xar3");
            xar4 = factory.Reg16("xar4");
            xar5 = factory.Reg16("xar5");
            xar6 = factory.Reg16("xar6");
            xar7 = factory.Reg16("xar7");
            pc = factory.Reg("pc", PrimitiveType.CreateWord(22));
            rpc = factory.Reg("rpc", PrimitiveType.CreateWord(22));
            st0 = factory.Reg16("st0");
            st1 = factory.Reg16("st1");

            ier = factory.Reg16("ier");
            dbgier = factory.Reg16("dbgier");
            ifr = factory.Reg16("ifr");

            t =  RegisterStorage.Reg16("t", xt.Number, 16);
            tl = RegisterStorage.Reg16("tl", xt.Number);
            ph = RegisterStorage.Reg16("ph", p.Number, 16);
            pl = RegisterStorage.Reg16("pl", p.Number);
            ah = RegisterStorage.Reg16("ah", acc.Number, 16);
            al = RegisterStorage.Reg16("al", acc.Number);

            ar0h = RegisterStorage.Reg16("ar0h", acc.Number, 16);
            ar0l = RegisterStorage.Reg16("ar0l", acc.Number);
            ar1h = RegisterStorage.Reg16("ar1h", acc.Number, 16);
            ar1l = RegisterStorage.Reg16("ar1l", acc.Number);
            ar2h = RegisterStorage.Reg16("ar2h", acc.Number, 16);
            ar2l = RegisterStorage.Reg16("ar2l", acc.Number);
            ar3h = RegisterStorage.Reg16("ar3h", acc.Number, 16);
            ar3l = RegisterStorage.Reg16("ar3l", acc.Number);
            ar4h = RegisterStorage.Reg16("ar4h", acc.Number, 16);
            ar4l = RegisterStorage.Reg16("ar4l", acc.Number);
            ar5h = RegisterStorage.Reg16("ar5h", acc.Number, 16);
            ar5l = RegisterStorage.Reg16("ar5l", acc.Number);
            ar6h = RegisterStorage.Reg16("ar6h", acc.Number, 16);
            ar6l = RegisterStorage.Reg16("ar6l", acc.Number);
            ar7h = RegisterStorage.Reg16("ar7h", acc.Number, 16);
            ar7l = RegisterStorage.Reg16("ar7l", acc.Number);






        }
    }
}
