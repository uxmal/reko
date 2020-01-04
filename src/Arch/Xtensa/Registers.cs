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

namespace Reko.Arch.Xtensa
{
    public class Registers
    {
        public static RegisterStorage a0  ;
        public static RegisterStorage a1  ;
        public static RegisterStorage a2  ;
        public static RegisterStorage a3  ;
        public static RegisterStorage a4  ;
        public static RegisterStorage a5  ;
        public static RegisterStorage a6  ;
        public static RegisterStorage a7  ;
        public static RegisterStorage a8  ;
        public static RegisterStorage a9  ;
        public static RegisterStorage a10 ;
        public static RegisterStorage a11 ;
        public static RegisterStorage a12 ;
        public static RegisterStorage a13 ;
        public static RegisterStorage a14 ;
        public static RegisterStorage a15 ;

        public static RegisterStorage b0;
        public static RegisterStorage b1;
        public static RegisterStorage b2;
        public static RegisterStorage b3;
        public static RegisterStorage b4;
        public static RegisterStorage b5;
        public static RegisterStorage b6;
        public static RegisterStorage b7;
        public static RegisterStorage b8;
        public static RegisterStorage b9;
        public static RegisterStorage b10;
        public static RegisterStorage b11;
        public static RegisterStorage b12;
        public static RegisterStorage b13;
        public static RegisterStorage b14;
        public static RegisterStorage b15;

        public static RegisterStorage f0;
        public static RegisterStorage f1;
        public static RegisterStorage f2;
        public static RegisterStorage f3;
        public static RegisterStorage f4;
        public static RegisterStorage f5;
        public static RegisterStorage f6;
        public static RegisterStorage f7;
        public static RegisterStorage f8;
        public static RegisterStorage f9;
        public static RegisterStorage f10;
        public static RegisterStorage f11;
        public static RegisterStorage f12;
        public static RegisterStorage f13;
        public static RegisterStorage f14;
        public static RegisterStorage f15;

        public static RegisterStorage SAR;

        static Registers()
        {
            a0 = new RegisterStorage( "a0", 0, 0, PrimitiveType.Word32);
            a1 = new RegisterStorage( "a1", 1, 0, PrimitiveType.Word32);
            a2 = new RegisterStorage( "a2", 2, 0, PrimitiveType.Word32);
            a3 = new RegisterStorage( "a3", 3, 0, PrimitiveType.Word32);
            a4 = new RegisterStorage( "a4", 4, 0, PrimitiveType.Word32);
            a5 = new RegisterStorage( "a5", 5, 0, PrimitiveType.Word32);
            a6 = new RegisterStorage( "a6", 6, 0, PrimitiveType.Word32);
            a7 = new RegisterStorage( "a7", 7, 0, PrimitiveType.Word32);
            a8 = new RegisterStorage( "a8", 8, 0, PrimitiveType.Word32);
            a9 = new RegisterStorage( "a9", 9, 0, PrimitiveType.Word32);
            a10 = new RegisterStorage("a10", 10, 0, PrimitiveType.Word32);
            a11 = new RegisterStorage("a11", 11, 0, PrimitiveType.Word32);
            a12 = new RegisterStorage("a12", 12, 0, PrimitiveType.Word32);
            a13 = new RegisterStorage("a13", 13, 0, PrimitiveType.Word32);
            a14 = new RegisterStorage("a14", 14, 0, PrimitiveType.Word32);
            a15 = new RegisterStorage("a15", 15, 0, PrimitiveType.Word32);

            b0 = new RegisterStorage("b0", 0x10, 0, PrimitiveType.Bool);
            b1 = new RegisterStorage("b1", 0x11, 0, PrimitiveType.Bool);
            b2 = new RegisterStorage("b2", 0x12, 0, PrimitiveType.Bool);
            b3 = new RegisterStorage("b3", 0x13, 0, PrimitiveType.Bool);
            b4 = new RegisterStorage("b4", 0x14, 0, PrimitiveType.Bool);
            b5 = new RegisterStorage("b5", 0x15, 0, PrimitiveType.Bool);
            b6 = new RegisterStorage("b6", 0x16, 0, PrimitiveType.Bool);
            b7 = new RegisterStorage("b7", 0x17, 0, PrimitiveType.Bool);
            b8 = new RegisterStorage("b8", 0x18, 0, PrimitiveType.Bool);
            b9 = new RegisterStorage("b9", 0x19, 0, PrimitiveType.Bool);
            b10 = new RegisterStorage("b10", 0x1A, 0, PrimitiveType.Bool);
            b11 = new RegisterStorage("b11", 0x1B, 0, PrimitiveType.Bool);
            b12 = new RegisterStorage("b12", 0x1C, 0, PrimitiveType.Bool);
            b13 = new RegisterStorage("b13", 0x1D, 0, PrimitiveType.Bool);
            b14 = new RegisterStorage("b14", 0x1E, 0, PrimitiveType.Bool);
            b15 = new RegisterStorage("b15", 0x1F, 0, PrimitiveType.Bool);

            f0 = new RegisterStorage("f0", 0x20, 0, PrimitiveType.Real32);
            f1 = new RegisterStorage("f1", 0x21, 0, PrimitiveType.Real32);
            f2 = new RegisterStorage("f2", 0x22, 0, PrimitiveType.Real32);
            f3 = new RegisterStorage("f3", 0x23, 0, PrimitiveType.Real32);
            f4 = new RegisterStorage("f4", 0x24, 0, PrimitiveType.Real32);
            f5 = new RegisterStorage("f5", 0x25, 0, PrimitiveType.Real32);
            f6 = new RegisterStorage("f6", 0x26, 0, PrimitiveType.Real32);
            f7 = new RegisterStorage("f7", 0x27, 0, PrimitiveType.Real32);
            f8 = new RegisterStorage("f8", 0x28, 0, PrimitiveType.Real32);
            f9 = new RegisterStorage("f9", 0x29, 0, PrimitiveType.Real32);
            f10 = new RegisterStorage("f10", 0x2A, 0, PrimitiveType.Real32);
            f11 = new RegisterStorage("f11", 0x2B, 0, PrimitiveType.Real32);
            f12 = new RegisterStorage("f12", 0x2C, 0, PrimitiveType.Real32);
            f13 = new RegisterStorage("f13", 0x2D, 0, PrimitiveType.Real32);
            f14 = new RegisterStorage("f14", 0x2E, 0, PrimitiveType.Real32);
            f15 = new RegisterStorage("f15", 0x2F, 0, PrimitiveType.Real32);

            SAR = new RegisterStorage("SAR", 0x103, 0, PrimitiveType.Word32);
        }
    }
}
