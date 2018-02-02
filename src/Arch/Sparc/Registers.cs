#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.Sparc
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
        public static RegisterStorage l6;
        public static RegisterStorage l7;

        public static RegisterStorage i0;   // incoming parameters / return value to caller
        public static RegisterStorage i1;
        public static RegisterStorage i2;
        public static RegisterStorage i3;
        public static RegisterStorage i4;
        public static RegisterStorage i5;
        public static RegisterStorage i6;   // frame pointer
        public static RegisterStorage i7;   // return address - 8

        public static RegisterStorage y;

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

        public static RegisterStorage f16;
        public static RegisterStorage f17;
        public static RegisterStorage f18;
        public static RegisterStorage f19;
        public static RegisterStorage f20;
        public static RegisterStorage f21;
        public static RegisterStorage f22;
        public static RegisterStorage f23;

        public static RegisterStorage f24;
        public static RegisterStorage f25;
        public static RegisterStorage f26;
        public static RegisterStorage f27;
        public static RegisterStorage f28;
        public static RegisterStorage f29;
        public static RegisterStorage f30;
        public static RegisterStorage f31;

        public static FlagRegister psr;

        public static FlagGroupStorage N;
        public static FlagGroupStorage Z;
        public static FlagGroupStorage V;
        public static FlagGroupStorage C;

        public static FlagGroupStorage E;
        public static FlagGroupStorage L;
        public static FlagGroupStorage G;
        public static FlagGroupStorage U;

        public static RegisterStorage[] IntegerRegisters;
        public static RegisterStorage[] FloatRegisters;

        private static Dictionary<string, RegisterStorage> mpNameToReg;

        static Registers()
        {
            g0 = RegisterStorage.Reg32("g0", 0);
            g1 = RegisterStorage.Reg32("g1", 1);
            g2 = RegisterStorage.Reg32("g2", 2);
            g3 = RegisterStorage.Reg32("g3", 3);
            g4 = RegisterStorage.Reg32("g4", 4);
            g5 = RegisterStorage.Reg32("g5", 5);
            g6 = RegisterStorage.Reg32("g6", 6);
            g7 = RegisterStorage.Reg32("g7", 7);

            o0 = RegisterStorage.Reg32("o0", 8);   // outgoing paramter 0 / return valie from callee
            o1 = RegisterStorage.Reg32("o1", 9);
            o2 = RegisterStorage.Reg32("o2", 10);
            o3 = RegisterStorage.Reg32("o3", 11);
            o4 = RegisterStorage.Reg32("o4", 12);
            o5 = RegisterStorage.Reg32("o5", 13);
            sp = RegisterStorage.Reg32("sp", 14);   // stack pointer
            o7 = RegisterStorage.Reg32("o7", 15);

            l0 = RegisterStorage.Reg32("l0", 16);
            l1 = RegisterStorage.Reg32("l1", 17);
            l2 = RegisterStorage.Reg32("l2", 18);
            l3 = RegisterStorage.Reg32("l3", 19);
            l4 = RegisterStorage.Reg32("l4", 20);
            l5 = RegisterStorage.Reg32("l5", 21);
            l6 = RegisterStorage.Reg32("l6", 22);
            l7 = RegisterStorage.Reg32("l7", 23);

            i0 = RegisterStorage.Reg32("i0", 24);   // incoming parameters / return value to caller
            i1 = RegisterStorage.Reg32("i1", 25);
            i2 = RegisterStorage.Reg32("i2", 26);
            i3 = RegisterStorage.Reg32("i3", 27);
            i4 = RegisterStorage.Reg32("i4", 28);
            i5 = RegisterStorage.Reg32("i5", 29);
            i6 = RegisterStorage.Reg32("i6", 30);   // frame pointer
            i7 = RegisterStorage.Reg32("i7", 31);   // return address - 8

            y =  RegisterStorage.Reg32("y", 32);

            // Sparc floating point registers can contain integers, which is 
            // why they can't be real32. This also forces our hand into
            // making float-point versions of add, sub, mul, div. 

            f0 = RegisterStorage.Reg32("f0", 0);
            f1 = RegisterStorage.Reg32("f1", 1);
            f2 = RegisterStorage.Reg32("f2", 2);
            f3 = RegisterStorage.Reg32("f3", 3);
            f4 = RegisterStorage.Reg32("f4", 4);
            f5 = RegisterStorage.Reg32("f5", 5);
            f6 = RegisterStorage.Reg32("f6", 6);
            f7 = RegisterStorage.Reg32("f7", 7);

            f8 = RegisterStorage.Reg32("f8", 8);
            f9 = RegisterStorage.Reg32("f9", 9);
            f10= RegisterStorage.Reg32("f10", 10);
            f11= RegisterStorage.Reg32("f11", 11);
            f12= RegisterStorage.Reg32("f12", 12);
            f13= RegisterStorage.Reg32("f13", 13);
            f14= RegisterStorage.Reg32("f14", 14);
            f15= RegisterStorage.Reg32("f15", 15);

            f16 =RegisterStorage.Reg32("f16", 16);
            f17= RegisterStorage.Reg32("f17", 17);
            f18= RegisterStorage.Reg32("f18", 18);
            f19= RegisterStorage.Reg32("f19", 19);
            f20= RegisterStorage.Reg32("f20", 20);
            f21= RegisterStorage.Reg32("f21", 21);
            f22= RegisterStorage.Reg32("f22", 22);
            f23= RegisterStorage.Reg32("f23", 23);

            f24= RegisterStorage.Reg32("f24", 24);
            f25= RegisterStorage.Reg32("f25", 25);
            f26= RegisterStorage.Reg32("f26", 26);
            f27= RegisterStorage.Reg32("f27", 27);
            f28= RegisterStorage.Reg32("f28", 28);
            f29= RegisterStorage.Reg32("f29", 29);
            f30= RegisterStorage.Reg32("f30", 30);
            f31= RegisterStorage.Reg32("f31", 31);

            psr = new FlagRegister("psr", 64, PrimitiveType.Word32);

            N = new FlagGroupStorage(psr, (uint) FlagM.NF, "N", PrimitiveType.Bool);
            Z = new FlagGroupStorage(psr, (uint) FlagM.ZF, "Z", PrimitiveType.Bool);
            V = new FlagGroupStorage(psr, (uint) FlagM.VF, "V", PrimitiveType.Bool);
            C = new FlagGroupStorage(psr, (uint) FlagM.CF, "C", PrimitiveType.Bool);
 
            E = new FlagGroupStorage(psr, (uint) FlagM.EF, "E", PrimitiveType.Bool);
            L = new FlagGroupStorage(psr, (uint) FlagM.LF, "L", PrimitiveType.Bool);
            G = new FlagGroupStorage(psr, (uint) FlagM.GF, "G", PrimitiveType.Bool);
            U = new FlagGroupStorage(psr, (uint) FlagM.UF, "U", PrimitiveType.Bool);

            IntegerRegisters = new RegisterStorage[]
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
                l6, 
                l7, 

                i0,  
                i1, 
                i2, 
                i3, 
                i4, 
                i5, 
                i6, 
                i7, 

                y,
            };

            FloatRegisters = new RegisterStorage[] {
                f0 ,
                f1 ,
                f2 ,
                f3 ,
                f4 ,
                f5 ,
                f6 ,
                f7 ,
   
                f8 ,
                f9 ,
                f10,
                f11,
                f12,
                f13,
                f14,
                f15,
   
                f16,
                f17,
                f18,
                f19,
                f20,
                f21,
                f22,
                f23,
   
                f24,
                f25,
                f26,
                f27,
                f28,
                f29,
                f30,
                f31,
            };
            mpNameToReg = IntegerRegisters.Concat(FloatRegisters).ToDictionary(k => k.Name, v => v);
        }

        public static RegisterStorage GetRegister(uint r)
        {
            return IntegerRegisters[r & 0x1F];
        }

        public static RegisterStorage GetFpuRegister(int f)
        {
            return FloatRegisters[f];
        }

        public static RegisterStorage GetFpuRegister(uint f)
        {
            return FloatRegisters[f];
        }

        public static RegisterStorage GetRegister(string regName)
        {
            RegisterStorage reg;
            if (mpNameToReg.TryGetValue(regName, out reg))
                return reg;
            else
                return null;
        }
    }

    [Flags]
    public enum FlagM : uint
    {
        NF = 8,             // sign
        ZF = 4,             // zero
        VF = 2,             // overflow
        CF = 1,             // carry
        
        EF = 0x80,          // FPU equality
        LF = 0x40,          // FPU less than
        GF = 0x20,          // FPU greater than
        UF = 0x10,          // FPU unordered
    }
}
