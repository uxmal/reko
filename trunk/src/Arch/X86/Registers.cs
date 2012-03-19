#region License
/* 
 * Copyright (C) 1999-2012 John Källén.
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

using Decompiler.Core.Machine;
using Decompiler.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.Arch.X86
{
    /// <summary>
    /// The registers of an Intel ia32 machine.
    /// </summary>
    public class Registers
    {
        public static readonly Intel32AccRegister eax;
        public static readonly Intel32AccRegister ecx;
        public static readonly Intel32AccRegister edx;
        public static readonly Intel32AccRegister ebx;
        public static readonly Intel32Register esp;
        public static readonly Intel32Register ebp;
        public static readonly Intel32Register esi;
        public static readonly Intel32Register edi;
        public static readonly Intel16AccRegister ax;
        public static readonly Intel16AccRegister cx;
        public static readonly Intel16AccRegister dx;
        public static readonly Intel16AccRegister bx;
        public static readonly Intel16Register sp;
        public static readonly Intel16Register bp;
        public static readonly Intel16Register si;
        public static readonly Intel16Register di;
        public static readonly IntelLoByteRegister al;
        public static readonly IntelLoByteRegister cl;
        public static readonly IntelLoByteRegister dl;
        public static readonly IntelLoByteRegister bl;
        public static readonly IntelHiByteRegister ah;
        public static readonly IntelHiByteRegister ch;
        public static readonly IntelHiByteRegister dh;
        public static readonly IntelHiByteRegister bh;
        public static readonly SegmentRegister es;
        public static readonly SegmentRegister cs;
        public static readonly SegmentRegister ss;
        public static readonly SegmentRegister ds;
        public static readonly SegmentRegister fs;
        public static readonly SegmentRegister gs;

        public static readonly FlagRegister S;
        public static readonly FlagRegister C;
        public static readonly FlagRegister Z;
        public static readonly FlagRegister D;
        public static readonly FlagRegister O;
        public static readonly FlagRegister P;

        public static readonly MachineRegister FPUF;
        public static readonly MachineRegister FPST;    // virtual register; the x87 FPU stack pointer.

        public static readonly Intel64AccRegister rax;
        public static readonly Intel64AccRegister rcx;
        public static readonly Intel64AccRegister rdx;
        public static readonly Intel64AccRegister rbx;
        public static readonly Intel64Register rsp;
        public static readonly Intel64Register rbp;
        public static readonly Intel64Register rsi;
        public static readonly Intel64Register rdi;

        public static readonly Intel64Register r8;
        public static readonly Intel64Register r9;
        public static readonly Intel64Register r10;
        public static readonly Intel64Register r11;
        public static readonly Intel64Register r12;
        public static readonly Intel64Register r13;
        public static readonly Intel64Register r14;
        public static readonly Intel64Register r15;

        public static readonly Intel64Register r8d;
        public static readonly Intel64Register r9d;
        public static readonly Intel64Register r10d;
        public static readonly Intel64Register r11d;
        public static readonly Intel64Register r12d;
        public static readonly Intel64Register r13d;
        public static readonly Intel64Register r14d;
        public static readonly Intel64Register r15d;

        public static readonly Intel64Register r8b;
        public static readonly Intel64Register r9b;
        public static readonly Intel64Register r10b;
        public static readonly Intel64Register r11b;
        public static readonly Intel64Register r12b;
        public static readonly Intel64Register r13b;
        public static readonly Intel64Register r14b;
        public static readonly Intel64Register r15b;


        private static readonly MachineRegister[] regs;

        static Registers()
        {
            eax = new Intel32AccRegister("eax", 0, 8, 16, 20);
            ecx = new Intel32AccRegister("ecx", 1, 9, 17, 21);
            edx = new Intel32AccRegister("edx", 2, 10, 18, 22);
            ebx = new Intel32AccRegister("ebx", 3, 11, 19, 23);
            esp = new Intel32Register("esp", 4, 12, -1, -1);
            ebp = new Intel32Register("ebp", 5, 13, -1, -1);
            esi = new Intel32Register("esi", 6, 14, -1, -1);
            edi = new Intel32Register("edi", 7, 15, -1, -1);
            ax = new Intel16AccRegister("ax", 8, 0, 16, 20);
            cx = new Intel16AccRegister("cx", 9, 1, 17, 21);
            dx = new Intel16AccRegister("dx", 10, 2, 18, 22);
            bx = new Intel16AccRegister("bx", 11, 3, 19, 23);
            sp = new Intel16Register("sp", 12, 4, -1, -1);
            bp = new Intel16Register("bp", 13, 5, -1, -1);
            si = new Intel16Register("si", 14, 6, -1, -1);
            di = new Intel16Register("di", 15, 7, -1, -1);
            al = new IntelLoByteRegister("al", 16, 0, 8, 16, 20);
            cl = new IntelLoByteRegister("cl", 17, 1, 9, 17, 21);
            dl = new IntelLoByteRegister("dl", 18, 2, 10, 18, 22);
            bl = new IntelLoByteRegister("bl", 19, 3, 11, 19, 23);
            ah = new IntelHiByteRegister("ah", 20, 0, 8, 16, 20);
            ch = new IntelHiByteRegister("ch", 21, 1, 9, 17, 21);
            dh = new IntelHiByteRegister("dh", 22, 2, 10, 18, 22);
            bh = new IntelHiByteRegister("bh", 23, 3, 11, 19, 23);
            es = new SegmentRegister("es", 24);
            cs = new SegmentRegister("cs", 25);
            ss = new SegmentRegister("ss", 26);
            ds = new SegmentRegister("ds", 27);
            fs = new SegmentRegister("fs", 28);
            gs = new SegmentRegister("gs", 29);
            S = new FlagRegister("S", 32);
            C = new FlagRegister("C", 33);
            Z = new FlagRegister("Z", 34);
            D = new FlagRegister("D", 35);
            O = new FlagRegister("O", 36);
            P = new FlagRegister("P", 37);
            FPUF = new MachineRegister("FPUF", 38, PrimitiveType.Byte);
            FPST = new MachineRegister("FPST", 38, PrimitiveType.Byte);
            rax = new Intel64AccRegister("rax", 0, 8, 16, 20);
            rcx = new Intel64AccRegister("rcx", 1, 9, 17, 21);
            rdx = new Intel64AccRegister("rdx", 2, 10, 18, 22);
            rbx = new Intel64AccRegister("rbx", 3, 11, 19, 23);
            rsp = new Intel64Register("rsp", 4, 12, -1, -1);
            rbp = new Intel64Register("rbp", 5, 13, -1, -1);
            rsi = new Intel64Register("rsi", 6, 14, -1, -1);
            rdi = new Intel64Register("rdi", 7, 15, -1, -1);

            regs = new MachineRegister[] {
				eax,
				ecx,
				edx,
				ebx, 
				esp,
				ebp,
				esi,
				edi,

				ax ,
				cx ,
				dx ,
				bx ,
				sp ,
				bp ,
				si ,
				di ,

				al ,
				cl ,
				dl ,
				bl ,
				ah ,
				ch ,
				dh ,
				bh ,

				es ,
				cs ,
				ss ,
				ds ,
				fs ,
				gs ,
				null,
				null,

				S ,
				C ,
				Z ,
				D ,

				O ,
                P,
                FPUF,
			};
        }

        public static MachineRegister GetRegister(int i)
        {
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        public static MachineRegister GetRegister(string name)
        {
            for (int i = 0; i < regs.Length; ++i)
            {
                if (regs[i] != null && String.Compare(regs[i].Name, name, true) == 0)
                    return regs[i];
            }
            return MachineRegister.None;
        }

        public static int Max
        {
            get { return regs.Length; }
        }
    }
}
