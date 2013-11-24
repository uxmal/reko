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
    public static class Registers
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

        public static readonly RegisterStorage FPUF;
        public static readonly RegisterStorage FPST;    // virtual register; the x87 FPU stack pointer.

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

        public static readonly Intel32Register r8w;
        public static readonly Intel32Register r9w;
        public static readonly Intel32Register r10w;
        public static readonly Intel32Register r11w;
        public static readonly Intel32Register r12w;
        public static readonly Intel32Register r13w;
        public static readonly Intel32Register r14w;
        public static readonly Intel32Register r15w;

        public static readonly IntelLoByteRegister spl;
        public static readonly IntelLoByteRegister bpl;
        public static readonly IntelLoByteRegister sil;
        public static readonly IntelLoByteRegister dil;
        public static readonly IntelLoByteRegister r8b;
        public static readonly IntelLoByteRegister r9b;
        public static readonly IntelLoByteRegister r10b;
        public static readonly IntelLoByteRegister r11b;
        public static readonly IntelLoByteRegister r12b;
        public static readonly IntelLoByteRegister r13b;
        public static readonly IntelLoByteRegister r14b;
        public static readonly IntelLoByteRegister r15b;

        public static readonly SseRegister xmm0;
        public static readonly SseRegister xmm1;
        public static readonly SseRegister xmm2;
        public static readonly SseRegister xmm3;
        public static readonly SseRegister xmm4;
        public static readonly SseRegister xmm5;
        public static readonly SseRegister xmm6;
        public static readonly SseRegister xmm7;
        public static readonly SseRegister xmm8;
        public static readonly SseRegister xmm9;
        public static readonly SseRegister xmm10;
        public static readonly SseRegister xmm11;
        public static readonly SseRegister xmm12;
        public static readonly SseRegister xmm13;
        public static readonly SseRegister xmm14;
        public static readonly SseRegister xmm15;

        private static readonly RegisterStorage[] regs;

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
            FPUF = new RegisterStorage("FPUF", 38, PrimitiveType.Byte);
            FPST = new RegisterStorage("FPST", 38, PrimitiveType.Byte); 

            rax = new Intel64AccRegister("rax", 40, 0, 8, 16, 20);
            rcx = new Intel64AccRegister("rcx", 41, 1, 9, 17, 21);
            rdx = new Intel64AccRegister("rdx", 42, 2, 10, 18, 22);
            rbx = new Intel64AccRegister("rbx", 43, 3, 11, 19, 23);
            rsp = new Intel64Register("rsp", 4, 12, -1, -1);
            rbp = new Intel64Register("rbp",  5, 13, -1, -1);
            rsi = new Intel64Register("rsi",  6, 14, -1, -1);
            rdi = new Intel64Register("rdi",  7, 15, -1, -1);
            r8 = new Intel64Register("r8",  8, 12, -1, -1);
            r9 = new Intel64Register("r9", 9, 13, -1, -1);
            r10 = new Intel64Register("r10", 10, 14, -1, -1);
            r11 = new Intel64Register("r11", 11, 15, -1, -1);
            r12 = new Intel64Register("r12", 12, 15, -1, -1);
            r13 = new Intel64Register("r13", 13, 15, -1, -1);
            r14 = new Intel64Register("r14", 14, 15, -1, -1);
            r15 = new Intel64Register("r15", 15, 15, -1, -1);
            r8d = new Intel64Register("r8d", 8, 12, -1, -1);
            r9d = new Intel64Register("r9d", 9, 13, -1, -1);
            r10d = new Intel64Register("r10d", 10, 14, -1, -1);
            r11d = new Intel64Register("r11d", 11, 15, -1, -1);
            r12d = new Intel64Register("r12d", 12, 15, -1, -1);
            r13d = new Intel64Register("r13d", 13, 15, -1, -1);
            r14d = new Intel64Register("r14d", 14, 15, -1, -1);
            r15d = new Intel64Register("r15d", 15, 15, -1, -1);
            r8w = new Intel32Register("r8w", 8, 12, -1, -1);
            r9w = new Intel32Register("r9w", 9, 13, -1, -1);
            r10w = new Intel32Register("r10w", 10, 14, -1, -1);
            r11w = new Intel32Register("r11w", 11, 15, -1, -1);
            r12w = new Intel32Register("r12w", 12, 15, -1, -1);
            r13w = new Intel32Register("r13w", 13, 15, -1, -1);
            r14w = new Intel32Register("r14w", 14, 15, -1, -1);
            r15w = new Intel32Register("r15w", 15, 15, -1, -1);

            spl = new IntelLoByteRegister("spl", 8, 12, -1, -1, -1);
            bpl = new IntelLoByteRegister("bpl", 9, 13, -1, -1, -1);
            sil = new IntelLoByteRegister("sil", 8, 12, -1, -1, -1);
            dil = new IntelLoByteRegister("dil", 9, 13, -1, -1, -1);

            r8b = new IntelLoByteRegister("r8b", 8, 12, -1, -1, -1);
            r9b = new IntelLoByteRegister("r9b", 9, 13, -1, -1, -1);
            r10b = new IntelLoByteRegister("r10b", 10, 14, -1, -1, -1);
            r11b = new IntelLoByteRegister("r11b", 11, 15, -1, -1, -1);
            r12b = new IntelLoByteRegister("r12b", 12, 15, -1, -1, -1);
            r13b = new IntelLoByteRegister("r13b", 13, 15, -1, -1, -1);
            r14b = new IntelLoByteRegister("r14b", 14, 15, -1, -1, -1);
            r15b = new IntelLoByteRegister("r15b", 15, 15, -1, -1, -1);

            xmm0 = new SseRegister("xmm0", 0);
            xmm1 = new SseRegister("xmm1", 1);
            xmm2 = new SseRegister("xmm2", 2);
            xmm3 = new SseRegister("xmm3", 3);
            xmm4 = new SseRegister("xmm4", 4);
            xmm5 = new SseRegister("xmm5", 5);
            xmm6 = new SseRegister("xmm6", 6);
            xmm7 = new SseRegister("xmm7", 7);
            xmm8 = new SseRegister("xmm8", 8);
            xmm9 = new SseRegister("xmm9", 9);
            xmm10 = new SseRegister("xmm10", 10);
            xmm11 = new SseRegister("xmm11", 11);
            xmm12 = new SseRegister("xmm12", 12);
            xmm13 = new SseRegister("xmm13", 13);
            xmm14 = new SseRegister("xmm14", 14);
            xmm15 = new SseRegister("xmm15", 15);

            regs = new RegisterStorage[] {
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

                // 16
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

                // 32
				S ,
				C ,
				Z ,
				D ,

				O ,
                P,
                FPUF,
                null,

                // 40
                rax,
                rcx,
                rdx,
                rbx,
                rsp,
                rbp,
                rsi,
                rdi,
                r8,
                r9,
                r10,
                r11,
                r12,
                r13,
                r14,
                r15,
                r8d,
                r9d,
                r10d,
                r11d,
                r12d,
                r13d,
                r14d,
                r15d,

                r8w,
                r9w,
                r10w,
                r11w,
                r12w,
                r13w,
                r14w,
                r15w,

                spl,
                bpl,
                sil,
                dil,

                r8b,
                r9b,
                r10b,
                r11b,
                r12b,
                r13b,
                r14b,
                r15b,

			};
        }

        public static RegisterStorage GetRegister(int i)
        {
            return (0 <= i && i < regs.Length)
                ? regs[i]
                : null;
        }

        public static RegisterStorage GetRegister(string name)
        {
            for (int i = 0; i < regs.Length; ++i)
            {
                if (regs[i] != null && String.Compare(regs[i].Name, name, true) == 0)
                    return regs[i];
            }
            return RegisterStorage.None;
        }

        public static int Max
        {
            get { return regs.Length; }
        }
    }
}
