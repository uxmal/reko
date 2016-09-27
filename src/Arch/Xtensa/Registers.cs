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

        static Registers()
        {
            a0 = new RegisterStorage( "a0", 0, 0, PrimitiveType.Word32);
            a1 = new RegisterStorage( "a1", 0, 0, PrimitiveType.Word32);
            a2 = new RegisterStorage( "a2", 0, 0, PrimitiveType.Word32);
            a3 = new RegisterStorage( "a3", 0, 0, PrimitiveType.Word32);
            a4 = new RegisterStorage( "a4", 0, 0, PrimitiveType.Word32);
            a5 = new RegisterStorage( "a5", 0, 0, PrimitiveType.Word32);
            a6 = new RegisterStorage( "a6", 0, 0, PrimitiveType.Word32);
            a7 = new RegisterStorage( "a7", 0, 0, PrimitiveType.Word32);
            a8 = new RegisterStorage( "a8", 0, 0, PrimitiveType.Word32);
            a9 = new RegisterStorage( "a9", 0, 0, PrimitiveType.Word32);
            a10 = new RegisterStorage("a10", 0, 0, PrimitiveType.Word32);
            a11 = new RegisterStorage("a11", 0, 0, PrimitiveType.Word32);
            a12 = new RegisterStorage("a12", 0, 0, PrimitiveType.Word32);
            a13 = new RegisterStorage("a13", 0, 0, PrimitiveType.Word32);
            a14 = new RegisterStorage("a14", 0, 0, PrimitiveType.Word32);
            a15 = new RegisterStorage("a15", 0, 0, PrimitiveType.Word32);
        }
    }
}
