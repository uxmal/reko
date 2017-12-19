#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Core.Machine;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.Arch.X86
{
    /// <summary>
    /// The registers of an x86 machine.
    /// </summary>
    public static class Registers
    {
        public static readonly RegisterStorage eax;
        public static readonly RegisterStorage ecx;
        public static readonly RegisterStorage edx;
        public static readonly RegisterStorage ebx;
        public static readonly RegisterStorage esp;
        public static readonly RegisterStorage ebp;
        public static readonly RegisterStorage esi;
        public static readonly RegisterStorage edi;
        public static readonly RegisterStorage ax;
        public static readonly RegisterStorage cx;
        public static readonly RegisterStorage dx;
        public static readonly RegisterStorage bx;
        public static readonly RegisterStorage sp;
        public static readonly RegisterStorage bp;
        public static readonly RegisterStorage si;
        public static readonly RegisterStorage di;
        public static readonly RegisterStorage al;
        public static readonly RegisterStorage cl;
        public static readonly RegisterStorage dl;
        public static readonly RegisterStorage bl;
        public static readonly RegisterStorage ah;
        public static readonly RegisterStorage ch;
        public static readonly RegisterStorage dh;
        public static readonly RegisterStorage bh;
        public static readonly RegisterStorage es;
        public static readonly RegisterStorage cs;
        public static readonly RegisterStorage ss;
        public static readonly RegisterStorage ds;
        public static readonly RegisterStorage fs;
        public static readonly RegisterStorage gs;

        public static readonly RegisterStorage S;
        public static readonly RegisterStorage C;
        public static readonly RegisterStorage Z;
        public static readonly RegisterStorage D;
        public static readonly RegisterStorage O;
        public static readonly RegisterStorage P;

        public static readonly Reko.Core.FlagRegister eflags;

        public static readonly RegisterStorage FPUF;
        public static readonly RegisterStorage FPST;    // virtual register; the x87 FPU stack pointer.

        public static readonly RegisterStorage rax;
        public static readonly RegisterStorage rcx;
        public static readonly RegisterStorage rdx;
        public static readonly RegisterStorage rbx;
        public static readonly RegisterStorage rsp;
        public static readonly RegisterStorage rbp;
        public static readonly RegisterStorage rsi;
        public static readonly RegisterStorage rdi;

        public static readonly RegisterStorage r8;
        public static readonly RegisterStorage r9;
        public static readonly RegisterStorage r10;
        public static readonly RegisterStorage r11;
        public static readonly RegisterStorage r12;
        public static readonly RegisterStorage r13;
        public static readonly RegisterStorage r14;
        public static readonly RegisterStorage r15;

        public static readonly RegisterStorage r8d;
        public static readonly RegisterStorage r9d;
        public static readonly RegisterStorage r10d;
        public static readonly RegisterStorage r11d;
        public static readonly RegisterStorage r12d;
        public static readonly RegisterStorage r13d;
        public static readonly RegisterStorage r14d;
        public static readonly RegisterStorage r15d;

        public static readonly RegisterStorage r8w;
        public static readonly RegisterStorage r9w;
        public static readonly RegisterStorage r10w;
        public static readonly RegisterStorage r11w;
        public static readonly RegisterStorage r12w;
        public static readonly RegisterStorage r13w;
        public static readonly RegisterStorage r14w;
        public static readonly RegisterStorage r15w;

        public static readonly RegisterStorage spl;
        public static readonly RegisterStorage bpl;
        public static readonly RegisterStorage sil;
        public static readonly RegisterStorage dil;
        public static readonly RegisterStorage r8b;
        public static readonly RegisterStorage r9b;
        public static readonly RegisterStorage r10b;
        public static readonly RegisterStorage r11b;
        public static readonly RegisterStorage r12b;
        public static readonly RegisterStorage r13b;
        public static readonly RegisterStorage r14b;
        public static readonly RegisterStorage r15b;

        public static readonly RegisterStorage xmm0;
        public static readonly RegisterStorage xmm1;
        public static readonly RegisterStorage xmm2;
        public static readonly RegisterStorage xmm3;
        public static readonly RegisterStorage xmm4;
        public static readonly RegisterStorage xmm5;
        public static readonly RegisterStorage xmm6;
        public static readonly RegisterStorage xmm7;
        public static readonly RegisterStorage xmm8;
        public static readonly RegisterStorage xmm9;
        public static readonly RegisterStorage xmm10;
        public static readonly RegisterStorage xmm11;
        public static readonly RegisterStorage xmm12;
        public static readonly RegisterStorage xmm13;
        public static readonly RegisterStorage xmm14;
        public static readonly RegisterStorage xmm15;

        public static readonly RegisterStorage mm0;
        public static readonly RegisterStorage mm1;
        public static readonly RegisterStorage mm2;
        public static readonly RegisterStorage mm3;
        public static readonly RegisterStorage mm4;
        public static readonly RegisterStorage mm5;
        public static readonly RegisterStorage mm6;
        public static readonly RegisterStorage mm7;
        public static readonly RegisterStorage mm8;
        public static readonly RegisterStorage mm9;
        public static readonly RegisterStorage mm10;
        public static readonly RegisterStorage mm11;
        public static readonly RegisterStorage mm12;
        public static readonly RegisterStorage mm13;
        public static readonly RegisterStorage mm14;
        public static readonly RegisterStorage mm15;

        public static readonly RegisterStorage rip;

        internal static readonly Dictionary<RegisterStorage, Dictionary<uint, RegisterStorage>> SubRegisters;

        internal static readonly RegisterStorage[] All;

        internal static readonly RegisterStorage[] Gp64BitRegisters;

        static Registers()
        {
            eax = new RegisterStorage("eax", 0, 0, PrimitiveType.Word32);
            ecx = new RegisterStorage("ecx", 1, 0, PrimitiveType.Word32);
            edx = new RegisterStorage("edx", 2, 0, PrimitiveType.Word32);
            ebx = new RegisterStorage("ebx", 3, 0, PrimitiveType.Word32);
            esp = new RegisterStorage("esp", 4, 0, PrimitiveType.Word32);
            ebp = new RegisterStorage("ebp", 5, 0, PrimitiveType.Word32);
            esi = new RegisterStorage("esi", 6, 0, PrimitiveType.Word32);
            edi = new RegisterStorage("edi", 7, 0, PrimitiveType.Word32);
            ax = new RegisterStorage("ax", 0, 0, PrimitiveType.Word16);
            cx = new RegisterStorage("cx", 1, 0, PrimitiveType.Word16);
            dx = new RegisterStorage("dx", 2, 0, PrimitiveType.Word16);
            bx = new RegisterStorage("bx", 3, 0, PrimitiveType.Word16);
            sp = new RegisterStorage("sp", 4, 0, PrimitiveType.Word16);
            bp = new RegisterStorage("bp", 5, 0, PrimitiveType.Word16);
            si = new RegisterStorage("si", 6, 0, PrimitiveType.Word16);
            di = new RegisterStorage("di", 7, 0, PrimitiveType.Word16);
            al = new RegisterStorage("al", 0, 0, PrimitiveType.Byte);
            cl = new RegisterStorage("cl", 1, 0, PrimitiveType.Byte);
            dl = new RegisterStorage("dl", 2, 0, PrimitiveType.Byte);
            bl = new RegisterStorage("bl", 3, 0, PrimitiveType.Byte);
            ah = new RegisterStorage("ah", 0, 8, PrimitiveType.Byte);
            ch = new RegisterStorage("ch", 1, 8, PrimitiveType.Byte);
            dh = new RegisterStorage("dh", 2, 8, PrimitiveType.Byte);
            bh = new RegisterStorage("bh", 3, 8, PrimitiveType.Byte);
            es = SegmentRegister("es", 24);
            cs = SegmentRegister("cs", 25);
            ss = SegmentRegister("ss", 26);
            ds = SegmentRegister("ds", 27);
            fs = SegmentRegister("fs", 28);
            gs = SegmentRegister("gs", 29);
            S = FlagRegister("S", 32);
            C = FlagRegister("C", 33);
            Z = FlagRegister("Z", 34);
            D = FlagRegister("D", 35);
            O = FlagRegister("O", 36);
            P = FlagRegister("P", 37);
            eflags = new Core.FlagRegister("eflags", 38, PrimitiveType.Word32);
            FPUF = new RegisterStorage("FPUF", 39, 0, PrimitiveType.Byte);
            FPST = new RegisterStorage("FPST", 39, 0, PrimitiveType.Byte); 

            rax = new RegisterStorage("rax", 0, 0, PrimitiveType.Word64);
            rcx = new RegisterStorage("rcx", 1, 0, PrimitiveType.Word64);
            rdx = new RegisterStorage("rdx", 2, 0, PrimitiveType.Word64);
            rbx = new RegisterStorage("rbx", 3, 0, PrimitiveType.Word64);
            rsp = new RegisterStorage("rsp", 4, 0, PrimitiveType.Word64);
            rbp = new RegisterStorage("rbp", 5, 0, PrimitiveType.Word64);
            rsi = new RegisterStorage("rsi", 6, 0, PrimitiveType.Word64);
            rdi = new RegisterStorage("rdi", 7, 0, PrimitiveType.Word64);
            r8 = new RegisterStorage("r8",   8, 0, PrimitiveType.Word64);
            r9 = new RegisterStorage("r9", 9, 0, PrimitiveType.Word64);
            r10 = new RegisterStorage("r10", 10, 0, PrimitiveType.Word64);
            r11 = new RegisterStorage("r11", 11, 0, PrimitiveType.Word64);
            r12 = new RegisterStorage("r12", 12, 0, PrimitiveType.Word64);
            r13 = new RegisterStorage("r13", 13, 0, PrimitiveType.Word64);
            r14 = new RegisterStorage("r14", 14, 0, PrimitiveType.Word64);
            r15 = new RegisterStorage("r15", 15, 0, PrimitiveType.Word64);
            r8d = new RegisterStorage("r8d", 8, 0, PrimitiveType.Word32);
            r9d = new RegisterStorage("r9d", 9, 0, PrimitiveType.Word32);
            r10d = new RegisterStorage("r10d", 10, 0, PrimitiveType.Word32);
            r11d = new RegisterStorage("r11d", 11, 0, PrimitiveType.Word32);
            r12d = new RegisterStorage("r12d", 12, 0, PrimitiveType.Word32);
            r13d = new RegisterStorage("r13d", 13, 0, PrimitiveType.Word32);
            r14d = new RegisterStorage("r14d", 14, 0, PrimitiveType.Word32);
            r15d = new RegisterStorage("r15d", 15, 0, PrimitiveType.Word32);
            r8w = new RegisterStorage("r8w", 8, 0, PrimitiveType.Word16);
            r9w = new RegisterStorage("r9w", 9, 0, PrimitiveType.Word16);
            r10w = new RegisterStorage("r10w", 10, 0, PrimitiveType.Word16);
            r11w = new RegisterStorage("r11w", 11, 0, PrimitiveType.Word16);
            r12w = new RegisterStorage("r12w", 12, 0, PrimitiveType.Word16);
            r13w = new RegisterStorage("r13w", 13, 0, PrimitiveType.Word16);
            r14w = new RegisterStorage("r14w", 14, 0, PrimitiveType.Word16);
            r15w = new RegisterStorage("r15w", 15, 0, PrimitiveType.Word16);

            spl = new RegisterStorage("spl", 4, 0, PrimitiveType.Byte);
            bpl = new RegisterStorage("bpl", 5, 0, PrimitiveType.Byte);
            sil = new RegisterStorage("sil", 6, 0, PrimitiveType.Byte);
            dil = new RegisterStorage("dil", 7, 0, PrimitiveType.Byte);

            r8b = new RegisterStorage("r8b",    8, 0, PrimitiveType.Byte);
            r9b = new RegisterStorage("r9b",    9, 0, PrimitiveType.Byte);
            r10b = new RegisterStorage("r10b", 10, 0, PrimitiveType.Byte);
            r11b = new RegisterStorage("r11b", 11, 0, PrimitiveType.Byte);
            r12b = new RegisterStorage("r12b", 12, 0, PrimitiveType.Byte);
            r13b = new RegisterStorage("r13b", 13, 0, PrimitiveType.Byte);
            r14b = new RegisterStorage("r14b", 14, 0, PrimitiveType.Byte);
            r15b = new RegisterStorage("r15b", 15, 0, PrimitiveType.Byte);

            mm0 = new  RegisterStorage("mm0", 40, 0, PrimitiveType.Word64);
            mm1 = new  RegisterStorage("mm1", 41, 0, PrimitiveType.Word64);
            mm2 = new  RegisterStorage("mm2", 42, 0, PrimitiveType.Word64);
            mm3 = new  RegisterStorage("mm3", 43, 0, PrimitiveType.Word64);
            mm4 = new  RegisterStorage("mm4", 44, 0, PrimitiveType.Word64);
            mm5 = new  RegisterStorage("mm5", 45, 0, PrimitiveType.Word64);
            mm6 = new  RegisterStorage("mm6", 46, 0, PrimitiveType.Word64);
            mm7 = new  RegisterStorage("mm7", 47, 0, PrimitiveType.Word64);
            mm8 = new  RegisterStorage("mm8", 48, 0, PrimitiveType.Word64);
            mm9 = new  RegisterStorage("mm9", 49, 0, PrimitiveType.Word64);
            mm10 = new RegisterStorage("mm10", 50, 0, PrimitiveType.Word64);
            mm11 = new RegisterStorage("mm11", 51, 0, PrimitiveType.Word64);
            mm12 = new RegisterStorage("mm12", 52, 0, PrimitiveType.Word64);
            mm13 = new RegisterStorage("mm13", 53, 0, PrimitiveType.Word64);
            mm14 = new RegisterStorage("mm14", 54, 0, PrimitiveType.Word64);
            mm15 = new RegisterStorage("mm15", 55, 0, PrimitiveType.Word64);
                       
            xmm0 = new RegisterStorage("xmm0", 60, 0, PrimitiveType.Word128);
            xmm1 = new RegisterStorage("xmm1", 61, 0, PrimitiveType.Word128);
            xmm2 = new RegisterStorage("xmm2", 62, 0, PrimitiveType.Word128);
            xmm3 = new RegisterStorage("xmm3", 63, 0, PrimitiveType.Word128);
            xmm4 = new RegisterStorage("xmm4", 64, 0, PrimitiveType.Word128);
            xmm5 = new RegisterStorage("xmm5", 65, 0, PrimitiveType.Word128);
            xmm6 = new RegisterStorage("xmm6", 66, 0, PrimitiveType.Word128);
            xmm7 = new RegisterStorage("xmm7", 67, 0, PrimitiveType.Word128);
            xmm8 = new RegisterStorage("xmm8", 68, 0, PrimitiveType.Word128);
            xmm9 = new RegisterStorage("xmm9", 69, 0, PrimitiveType.Word128);
            xmm10 = new RegisterStorage("xmm10", 70, 0, PrimitiveType.Word128);
            xmm11 = new RegisterStorage("xmm11", 71, 0, PrimitiveType.Word128);
            xmm12 = new RegisterStorage("xmm12", 72, 0, PrimitiveType.Word128);
            xmm13 = new RegisterStorage("xmm13", 73, 0, PrimitiveType.Word128);
            xmm14 = new RegisterStorage("xmm14", 74, 0, PrimitiveType.Word128);
            xmm15 = new RegisterStorage("xmm15", 75, 0, PrimitiveType.Word128);

            rip = new RegisterStorage("rip", 23, 0, PrimitiveType.Pointer64);

            All = new RegisterStorage[] {
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

                mm0 ,
                mm1 ,
                mm2 ,
                mm3 ,
                mm4 ,
                mm5 ,
                mm6 ,
                mm7 ,
                mm8 ,
                mm9 ,
                mm10,
                mm11,
                mm12,
                mm13,
                mm14,
                mm15,

                 xmm0 ,
                 xmm1 ,
                 xmm2 ,
                 xmm3 ,
                 xmm4 ,
                 xmm5 ,
                 xmm6 ,
                 xmm7 ,
                 xmm8 ,
                 xmm9 ,
                 xmm10,
                 xmm11,
                 xmm12,
                 xmm13,
                 xmm14,
                 xmm15,
			};

            SubRegisters = new Dictionary<RegisterStorage, Dictionary<uint, RegisterStorage>>
            {
                {
                    rax,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, al },
                        { 0x0808, ah },
                        { 0x0010, ax },
                        { 0x0020, eax },
                    }
                },
                {
                    rcx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, cl },
                        { 0x0808, ch },
                        { 0x0010, cx },
                        { 0x0020, ecx },
                    }
                },
                {
                    rdx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, dl },
                        { 0x0808, dh },
                        { 0x0010, dx },
                        { 0x0020, edx },
                    }
                },
                {
                    rbx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, bl },
                        { 0x0808, bh },
                        { 0x0010, bx },
                        { 0x0020, ebx },
                    }
                },
                {
                    rsp,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, spl },
                        { 0x0010, sp },
                        { 0x0020, esp },
                    }
                },
                {
                    rbp,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, bpl },
                        { 0x0010, bp },
                        { 0x0020, ebp },
                    }
                },
                {
                    rsi,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, sil },
                        { 0x0010, si },
                        { 0x0020, esi },
                    }
                },
                {
                    rdi,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, dil },
                        { 0x0010, di },
                        { 0x0020, edi },
                    }
                },
                {
                    r8,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r8b },
                        { 0x0010, r8w },
                        { 0x0020, r8d },
                    }
                },
                {
                    r9,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r9b },
                        { 0x0010, r9w },
                        { 0x0020, r9d },
                    }
                },
                {
                    r10,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r10b },
                        { 0x0010, r10w },
                        { 0x0020, r10d },
                    }
                },
                {
                    r11,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r11b },
                        { 0x0010, r11w },
                        { 0x0020, r11d },
                    }
                },
                {
                    r12,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r12b },
                        { 0x0010, r12w },
                        { 0x0020, r12d },
                    }
                },
                {
                    r13,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r13b },
                        { 0x0010, r13w },
                        { 0x0020, r13d },
                    }
                },
                {
                    r14,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r14b },
                        { 0x0010, r14w },
                        { 0x0020, r14d },
                    }
                },
                {
                    r15,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r15b },
                        { 0x0010, r15w },
                        { 0x0020, r15d },
                    }
                },
                {
                    eax,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, al },
                        { 0x0808, ah },
                        { 0x0010, ax },
                    }
                },
                {
                    ecx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, cl },
                        { 0x0808, ch },
                        { 0x0010, cx },
                    }
                },
                {
                    edx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, dl },
                        { 0x0808, dh },
                        { 0x0010, dx },
                    }
                },
                {
                    ebx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, bl },
                        { 0x0808, bh },
                        { 0x0010, bx },
                    }
                },
                {
                    esp,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, spl },
                        { 0x0010, sp },
                    }
                },
                {
                    ebp,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, bpl },
                        { 0x0010, bp },
                    }
                },
                {
                    esi,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, sil },
                        { 0x0010, si },
                    }
                },
                {
                    edi,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, dil },
                        { 0x0010, di },
                    }
                },
                {
                    r8d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r8b },
                        { 0x0010, r8w },
                    }
                },
                {
                    r9d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r9b },
                        { 0x0010, r9w },
                    }
                },
                {
                    r10d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r10b },
                        { 0x0010, r10w },
                    }
                },
                {
                    r11d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r11b },
                        { 0x0010, r11w },
                    }
                },
                {
                    r12d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r12b },
                        { 0x0010, r12w },
                    }
                },
                {
                    r13d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r13b },
                        { 0x0010, r13w },
                    }
                },
                {
                    r14d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r14b },
                        { 0x0010, r14w },
                    }
                },
                {
                    r15d,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r15b },
                        { 0x0010, r15w },
                    }
                },
                {
                    ax,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, al },
                        { 0x0808, ah },
                    }
                },
                {
                    cx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, cl },
                        { 0x0808, ch },
                    }
                },
                {
                    dx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, dl },
                        { 0x0808, dh },
                    }
                },
                {
                    bx,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, bl },
                        { 0x0808, bh },
                    }
                },
                {
                    sp,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, spl },
                    }
                },
                {
                    bp,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, bpl },
                    }
                },
                {
                    si,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, sil },
                    }
                },
                {
                    di,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, dil },
                    }
                },
                {
                    r8w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r8b },
                    }
                },
                {
                    r9w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r9b },
                    }
                },
                {
                    r10w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r10b },
                    }
                },
                {
                    r11w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r11b },
                    }
                },
                {
                    r12w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r12b },
                    }
                },
                {
                    r13w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r13b },
                    }
                },
                {
                    r14w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r14b },
                    }
                },
                {
                    r15w,
                    new Dictionary<uint, RegisterStorage>
                    {
                        { 0x0008, r15b },
                    }
                },
            };

            Gp64BitRegisters = new[]
            {
                rax, rcx, rdx, rbx, rsp, rbp, rsi, rdi, r8, r9, r10, r11, r12, r13, r14, r15,
            };
        }

        private static  RegisterStorage FlagRegister(string name, int grf)
        {
            return new RegisterStorage(name, grf, 0, PrimitiveType.Bool);
        }

        private static RegisterStorage SegmentRegister(string name, int reg)
        {
            return new RegisterStorage(name, reg, 0, PrimitiveType.SegmentSelector);
        }

        public static RegisterStorage GetRegister(int i)
        {
            return (0 <= i && i < All.Length)
                ? All[i]
                : null;
        }

        public static RegisterStorage GetRegister(string name)
        {
            for (int i = 0; i < All.Length; ++i)
            {
                if (All[i] != null && String.Compare(All[i].Name, name, true) == 0)
                    return All[i];
            }
            return RegisterStorage.None;
        }

        public static int Max
        {
            get { return All.Length; }
        }
    }
}
