/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public static class Registers
    {
        public static  MachineRegister d0;
        public static  MachineRegister d1;
        public static  MachineRegister d2;
        public static  MachineRegister d3;
        public static  MachineRegister d4;
        public static  MachineRegister d5;
        public static  MachineRegister d6;
        public static  MachineRegister d7;

        public static  MachineRegister a0;
        public static  MachineRegister a1;
        public static  MachineRegister a2;
        public static  MachineRegister a3;
        public static  MachineRegister a4;
        public static  MachineRegister a5;
        public static  MachineRegister a6;
        public static  MachineRegister a7;

        public static readonly MachineRegister ccr;
        public static readonly MachineRegister sr;

        private static MachineRegister[] regs;

        static Registers()
        {
            d0 = new MachineRegister("d0", 0, PrimitiveType.Word32);
            d1 = new MachineRegister("d1", 1, PrimitiveType.Word32);
            d2 = new MachineRegister("d2", 2, PrimitiveType.Word32);
            d3 = new MachineRegister("d3", 3, PrimitiveType.Word32);
            d4 = new MachineRegister("d4", 4, PrimitiveType.Word32);
            d5 = new MachineRegister("d5", 5, PrimitiveType.Word32);
            d6 = new MachineRegister("d6", 6, PrimitiveType.Word32);
            d7 = new MachineRegister("d7", 7, PrimitiveType.Word32);

            a0 = new MachineRegister("a0", 8, PrimitiveType.Word32);
            a1 = new MachineRegister("a1", 9, PrimitiveType.Word32);
            a2 = new MachineRegister("a2", 10, PrimitiveType.Word32);
            a3 = new MachineRegister("a3", 11, PrimitiveType.Word32);
            a4 = new MachineRegister("a4", 12, PrimitiveType.Word32);
            a5 = new MachineRegister("a5", 13, PrimitiveType.Word32);
            a6 = new MachineRegister("a6", 14, PrimitiveType.Word32);
            a7 = new MachineRegister("a7", 15, PrimitiveType.Word32);

            ccr = new MachineRegister("ccr", 16, PrimitiveType.Byte);
            sr = new MachineRegister("sr", 17, PrimitiveType.Byte);

            regs = new MachineRegister[] { 
                d0, 
                d1, 
                d2, 
                d3, 
                d4, 
                d5, 
                d6, 
                d7, 

                a0, 
                a1, 
                a2, 
                a3, 
                a4, 
                a5, 
                a6, 
                a7, 
            };
        }

        public static MachineRegister GetRegister(int p)
        {
            return regs[p];
        }
    }

    [Flags]
    public enum FlagM : byte
    {
        CF = 1,
        VF = 2,
        ZF = 4,
        NF = 8,
        XF = 16,
    }

}
