#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Lib;
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.Sparc
{
    public static class Registers
    {
        public static RegisterStorage g0;
        public static RegisterStorage g1;
        public static RegisterStorage g2;
        public static RegisterStorage g3;
        public static RegisterStorage g4;
        public static RegisterStorage g5;
        public static RegisterStorage g6;
        public static RegisterStorage g7;

        public static RegisterStorage o0;   // outgoing paramter 0 / return valie from callee
        public static RegisterStorage o1;
        public static RegisterStorage o2;
        public static RegisterStorage o3;
        public static RegisterStorage o4;
        public static RegisterStorage o5;
        public static RegisterStorage sp;   // stack pointer
        public static RegisterStorage o7;

        public static RegisterStorage l0;
        public static RegisterStorage l1;
        public static RegisterStorage l2;
        public static RegisterStorage l3;
        public static RegisterStorage l4;
        public static RegisterStorage l5;
        public static RegisterStorage lp;
        public static RegisterStorage l7;

        public static RegisterStorage i0;   // incoming parameters / return value to caller
        public static RegisterStorage i1;
        public static RegisterStorage i2;
        public static RegisterStorage i3;
        public static RegisterStorage i4;
        public static RegisterStorage i5;
        public static RegisterStorage i6;   // frame pointer
        public static RegisterStorage i7;   // return address - 8
        
        private static RegisterStorage[] iRegs;

        static Registers()
        {
            g0 = new RegisterStorage("g0", 0, PrimitiveType.Word32);
            g1 = new RegisterStorage("g1", 0, PrimitiveType.Word32);
            g2 = new RegisterStorage("g2", 0, PrimitiveType.Word32);
            g3 = new RegisterStorage("g3", 0, PrimitiveType.Word32);
            g4 = new RegisterStorage("g4", 0, PrimitiveType.Word32);
            g5 = new RegisterStorage("g5", 0, PrimitiveType.Word32);
            g6 = new RegisterStorage("g6", 0, PrimitiveType.Word32);
            g7 = new RegisterStorage("g7", 0, PrimitiveType.Word32);

            o0 = new RegisterStorage("o0", 0, PrimitiveType.Word32);   // outgoing paramter 0 / return valie from callee
            o1 = new RegisterStorage("o1", 0, PrimitiveType.Word32);
            o2 = new RegisterStorage("o2", 0, PrimitiveType.Word32);
            o3 = new RegisterStorage("o3", 0, PrimitiveType.Word32);
            o4 = new RegisterStorage("o4", 0, PrimitiveType.Word32);
            o5 = new RegisterStorage("o5", 0, PrimitiveType.Word32);
            sp = new RegisterStorage("sp", 0, PrimitiveType.Word32);   // stack pointer
            o7 = new RegisterStorage("o7", 0, PrimitiveType.Word32);

            l0 = new RegisterStorage("l0", 0, PrimitiveType.Word32);
            l1 = new RegisterStorage("l1", 0, PrimitiveType.Word32);
            l2 = new RegisterStorage("l2", 0, PrimitiveType.Word32);
            l3 = new RegisterStorage("l3", 0, PrimitiveType.Word32);
            l4 = new RegisterStorage("l4", 0, PrimitiveType.Word32);
            l5 = new RegisterStorage("l5", 0, PrimitiveType.Word32);
            lp = new RegisterStorage("lp", 0, PrimitiveType.Word32);
            l7 = new RegisterStorage("l7", 0, PrimitiveType.Word32);

            i0 = new RegisterStorage("i0", 0, PrimitiveType.Word32);   // incoming parameters / return value to caller
            i1 = new RegisterStorage("i1", 0, PrimitiveType.Word32);
            i2 = new RegisterStorage("i2", 0, PrimitiveType.Word32);
            i3 = new RegisterStorage("i3", 0, PrimitiveType.Word32);
            i4 = new RegisterStorage("i4", 0, PrimitiveType.Word32);
            i5 = new RegisterStorage("i5", 0, PrimitiveType.Word32);
            i6 = new RegisterStorage("i6", 0, PrimitiveType.Word32);   // frame pointer
            i7 = new RegisterStorage("i7", 0, PrimitiveType.Word32);   // return address - 8

            iRegs = new RegisterStorage[]
            {
                g0, 
                g1, 
                g2, 
                g3, 
                g4, 
                g5, 
                g6, 
                g7, 

                o0,  
                o1, 
                o2, 
                o3, 
                o4, 
                o5, 
                sp, 
                o7, 

                l0, 
                l1, 
                l2, 
                l3, 
                l4, 
                l5, 
                lp, 
                l7, 

                i0,  
                i1, 
                i2, 
                i3, 
                i4, 
                i5, 
                i6, 
                i7, 
            };
        }

        public static RegisterStorage GetRegister(uint r)
        {
            return iRegs[r];
        }
    }
}
