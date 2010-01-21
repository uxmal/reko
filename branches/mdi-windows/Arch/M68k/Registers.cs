/* 
 * Copyright (C) 1999-2010 John Källén.
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
using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.Arch.M68k
{
    public static class Registers
    {
        public static  DataRegister d0;
        public static  DataRegister d1;
        public static  DataRegister d2;
        public static  DataRegister d3;
        public static  DataRegister d4;
        public static  DataRegister d5;
        public static  DataRegister d6;
        public static  DataRegister d7;

        public static  AddressRegister a0;
        public static  AddressRegister a1;
        public static  AddressRegister a2;
        public static  AddressRegister a3;
        public static  AddressRegister a4;
        public static  AddressRegister a5;
        public static  AddressRegister a6;
        public static  AddressRegister a7;

        public static readonly MachineRegister ccr;
        public static readonly MachineRegister sr;

        private static MachineRegister[] regs;

        static Registers()
        {
            d0 = new DataRegister("d0", 0, PrimitiveType.Word32);
            d1 = new DataRegister("d1", 1, PrimitiveType.Word32);
            d2 = new DataRegister("d2", 2, PrimitiveType.Word32);
            d3 = new DataRegister("d3", 3, PrimitiveType.Word32);
            d4 = new DataRegister("d4", 4, PrimitiveType.Word32);
            d5 = new DataRegister("d5", 5, PrimitiveType.Word32);
            d6 = new DataRegister("d6", 6, PrimitiveType.Word32);
            d7 = new DataRegister("d7", 7, PrimitiveType.Word32);

            a0 = new AddressRegister("a0", 8, PrimitiveType.Word32);
            a1 = new AddressRegister("a1", 9, PrimitiveType.Word32);
            a2 = new AddressRegister("a2", 10, PrimitiveType.Word32);
            a3 = new AddressRegister("a3", 11, PrimitiveType.Word32);
            a4 = new AddressRegister("a4", 12, PrimitiveType.Word32);
            a5 = new AddressRegister("a5", 13, PrimitiveType.Word32);
            a6 = new AddressRegister("a6", 14, PrimitiveType.Word32);
            a7 = new AddressRegister("a7", 15, PrimitiveType.Word32);

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

    public class AddressRegister : MachineRegister
    {
        public AddressRegister(string name, int number, PrimitiveType dt) : base(name, number, dt)
        {
        }
    }

    public class DataRegister : MachineRegister
    {
        public DataRegister(string name, int number, PrimitiveType dt) : base(name, number, dt)
        {
        }
    }
}
