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
using Reko.Core.Expressions;
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

        public static readonly FlagGroupStorage S;
        public static readonly FlagGroupStorage C;
        public static readonly FlagGroupStorage Z;
        public static readonly FlagGroupStorage D;
        public static readonly FlagGroupStorage O;
        public static readonly FlagGroupStorage P;

        public static readonly RegisterStorage eflags;

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

        public static readonly RegisterStorage ymm0;
        public static readonly RegisterStorage ymm1;
        public static readonly RegisterStorage ymm2;
        public static readonly RegisterStorage ymm3;
        public static readonly RegisterStorage ymm4;
        public static readonly RegisterStorage ymm5;
        public static readonly RegisterStorage ymm6;
        public static readonly RegisterStorage ymm7;
        public static readonly RegisterStorage ymm8;
        public static readonly RegisterStorage ymm9;
        public static readonly RegisterStorage ymm10;
        public static readonly RegisterStorage ymm11;
        public static readonly RegisterStorage ymm12;
        public static readonly RegisterStorage ymm13;
        public static readonly RegisterStorage ymm14;
        public static readonly RegisterStorage ymm15;

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
        public static readonly RegisterStorage eip;     
        public static readonly RegisterStorage ip;
        
        public static readonly RegisterStorage Top;     // The x87 stack pointer is modelled explicitly.
        public static readonly MemoryIdentifier ST;

        public static readonly RegisterStorage mxcsr;

        internal static readonly Dictionary<StorageDomain, RegisterStorage[]> SubRegisters;

        internal static readonly RegisterStorage[] All;

        internal static readonly RegisterStorage[] Gp64BitRegisters;

        public const int ControlRegisterMin = 80;
        public const int DebugRegisterMin = 89;

        internal static readonly FlagGroupStorage[] EflagsBits;

        static Registers()
        {
            var factory = new StorageFactory();
            rax = factory.Reg64("rax");
            rcx = factory.Reg64("rcx");
            rdx = factory.Reg64("rdx");
            rbx = factory.Reg64("rbx");
            rsp = factory.Reg64("rsp");
            rbp = factory.Reg64("rbp");
            rsi = factory.Reg64("rsi");
            rdi = factory.Reg64("rdi");
            r8 =  factory.Reg64("r8");
            r9 =  factory.Reg64("r9");
            r10 = factory.Reg64("r10");
            r11 = factory.Reg64("r11");
            r12 = factory.Reg64("r12");
            r13 = factory.Reg64("r13");
            r14 = factory.Reg64("r14");
            r15 = factory.Reg64("r15");

            eax = new RegisterStorage("eax", rax.Number, 0, PrimitiveType.Word32);
            ecx = new RegisterStorage("ecx", rcx.Number, 0, PrimitiveType.Word32);
            edx = new RegisterStorage("edx", rdx.Number, 0, PrimitiveType.Word32);
            ebx = new RegisterStorage("ebx", rbx.Number, 0, PrimitiveType.Word32);
            esp = new RegisterStorage("esp", rsp.Number, 0, PrimitiveType.Word32);
            ebp = new RegisterStorage("ebp", rbp.Number, 0, PrimitiveType.Word32);
            esi = new RegisterStorage("esi", rsi.Number, 0, PrimitiveType.Word32);
            edi = new RegisterStorage("edi", rdi.Number, 0, PrimitiveType.Word32);
            ax = new RegisterStorage("ax", rax.Number, 0, PrimitiveType.Word16);
            cx = new RegisterStorage("cx", rcx.Number, 0, PrimitiveType.Word16);
            dx = new RegisterStorage("dx", rdx.Number, 0, PrimitiveType.Word16);
            bx = new RegisterStorage("bx", rbx.Number, 0, PrimitiveType.Word16);
            sp = new RegisterStorage("sp", rsp.Number, 0, PrimitiveType.Word16);
            bp = new RegisterStorage("bp", rbp.Number, 0, PrimitiveType.Word16);
            si = new RegisterStorage("si", rsi.Number, 0, PrimitiveType.Word16);
            di = new RegisterStorage("di", rdi.Number, 0, PrimitiveType.Word16);
            al = new RegisterStorage("al", rax.Number, 0, PrimitiveType.Byte);
            cl = new RegisterStorage("cl", rcx.Number, 0, PrimitiveType.Byte);
            dl = new RegisterStorage("dl", rdx.Number, 0, PrimitiveType.Byte);
            bl = new RegisterStorage("bl", rbx.Number, 0, PrimitiveType.Byte);
            ah = new RegisterStorage("ah", rax.Number, 8, PrimitiveType.Byte);
            ch = new RegisterStorage("ch", rcx.Number, 8, PrimitiveType.Byte);
            dh = new RegisterStorage("dh", rdx.Number, 8, PrimitiveType.Byte);
            bh = new RegisterStorage("bh", rbx.Number, 8, PrimitiveType.Byte);
            es = factory.Reg("es", PrimitiveType.SegmentSelector);
            cs = factory.Reg("cs", PrimitiveType.SegmentSelector);
            ss = factory.Reg("ss", PrimitiveType.SegmentSelector);
            ds = factory.Reg("ds", PrimitiveType.SegmentSelector);
            fs = factory.Reg("fs", PrimitiveType.SegmentSelector);
            gs = factory.Reg("gs", PrimitiveType.SegmentSelector);
            rip = factory.Reg64("rip");
            eip = new RegisterStorage("eip", rip.Number, 0, PrimitiveType.Word32);
            ip = new RegisterStorage("ip", rip.Number, 0, PrimitiveType.Word32);
            eflags = factory.Reg32("eflags");
            S = FlagRegister("S", eflags, FlagM.SF);
            C = FlagRegister("C", eflags, FlagM.CF);
            Z = FlagRegister("Z", eflags, FlagM.ZF);
            D = FlagRegister("D", eflags, FlagM.DF);
            O = FlagRegister("O", eflags, FlagM.OF);
            P = FlagRegister("P", eflags, FlagM.PF);
            EflagsBits = new FlagGroupStorage[] { S, C, Z, D, O, P };

            FPUF = factory.Reg("FPUF", PrimitiveType.Byte);
            FPST = factory.Reg("FPST", PrimitiveType.Byte); 

            r8d = new RegisterStorage("r8d", r8.Number, 0, PrimitiveType.Word32);
            r9d = new RegisterStorage("r9d", r9.Number, 0, PrimitiveType.Word32);
            r10d = new RegisterStorage("r10d", r10.Number, 0, PrimitiveType.Word32);
            r11d = new RegisterStorage("r11d", r11.Number, 0, PrimitiveType.Word32);
            r12d = new RegisterStorage("r12d", r12.Number, 0, PrimitiveType.Word32);
            r13d = new RegisterStorage("r13d", r13.Number, 0, PrimitiveType.Word32);
            r14d = new RegisterStorage("r14d", r14.Number, 0, PrimitiveType.Word32);
            r15d = new RegisterStorage("r15d", r15.Number, 0, PrimitiveType.Word32);
            r8w = new RegisterStorage("r8w", r8.Number, 0, PrimitiveType.Word16);
            r9w = new RegisterStorage("r9w", r9.Number, 0, PrimitiveType.Word16);
            r10w = new RegisterStorage("r10w", r10.Number, 0, PrimitiveType.Word16);
            r11w = new RegisterStorage("r11w", r11.Number, 0, PrimitiveType.Word16);
            r12w = new RegisterStorage("r12w", r12.Number, 0, PrimitiveType.Word16);
            r13w = new RegisterStorage("r13w", r13.Number, 0, PrimitiveType.Word16);
            r14w = new RegisterStorage("r14w", r14.Number, 0, PrimitiveType.Word16);
            r15w = new RegisterStorage("r15w", r15.Number, 0, PrimitiveType.Word16);

            spl = new RegisterStorage("spl", rsp.Number, 0, PrimitiveType.Byte);
            bpl = new RegisterStorage("bpl", rbp.Number, 0, PrimitiveType.Byte);
            sil = new RegisterStorage("sil", rsi.Number, 0, PrimitiveType.Byte);
            dil = new RegisterStorage("dil", rdi.Number, 0, PrimitiveType.Byte);

            r8b = new RegisterStorage("r8b",    r8.Number, 0, PrimitiveType.Byte);
            r9b = new RegisterStorage("r9b",    r9.Number, 0, PrimitiveType.Byte);
            r10b = new RegisterStorage("r10b", r10.Number, 0, PrimitiveType.Byte);
            r11b = new RegisterStorage("r11b", r11.Number, 0, PrimitiveType.Byte);
            r12b = new RegisterStorage("r12b", r12.Number, 0, PrimitiveType.Byte);
            r13b = new RegisterStorage("r13b", r13.Number, 0, PrimitiveType.Byte);
            r14b = new RegisterStorage("r14b", r14.Number, 0, PrimitiveType.Byte);
            r15b = new RegisterStorage("r15b", r15.Number, 0, PrimitiveType.Byte);

            mm0 = factory.Reg64("mm0");
            mm1 = factory.Reg64("mm1");
            mm2 = factory.Reg64("mm2");
            mm3 = factory.Reg64("mm3");
            mm4 = factory.Reg64("mm4");
            mm5 = factory.Reg64("mm5");
            mm6 = factory.Reg64("mm6");
            mm7 = factory.Reg64("mm7");
            mm8 = factory.Reg64("mm8");
            mm9 = factory.Reg64("mm9");
            mm10 = factory.Reg64("mm10");
            mm11 = factory.Reg64("mm11");
            mm12 = factory.Reg64("mm12");
            mm13 = factory.Reg64("mm13");
            mm14 = factory.Reg64("mm14");
            mm15 = factory.Reg64("mm15");

            ymm0 = factory.Reg("ymm0", PrimitiveType.Word256);
            ymm1 = factory.Reg("ymm1", PrimitiveType.Word256);
            ymm2 = factory.Reg("ymm2", PrimitiveType.Word256);
            ymm3 = factory.Reg("ymm3", PrimitiveType.Word256);
            ymm4 = factory.Reg("ymm4", PrimitiveType.Word256);
            ymm5 = factory.Reg("ymm5", PrimitiveType.Word256);
            ymm6 = factory.Reg("ymm6", PrimitiveType.Word256);
            ymm7 = factory.Reg("ymm7", PrimitiveType.Word256);
            ymm8 = factory.Reg("ymm8", PrimitiveType.Word256);
            ymm9 = factory.Reg("ymm9", PrimitiveType.Word256);
            ymm10 = factory.Reg("ymm10", PrimitiveType.Word256);
            ymm11 = factory.Reg("ymm11", PrimitiveType.Word256);
            ymm12 = factory.Reg("ymm12", PrimitiveType.Word256);
            ymm13 = factory.Reg("ymm13", PrimitiveType.Word256);
            ymm14 = factory.Reg("ymm14", PrimitiveType.Word256);
            ymm15 = factory.Reg("ymm15", PrimitiveType.Word256);

            xmm0 = new RegisterStorage("xmm0", ymm0.Number, 0, PrimitiveType.Word128);
            xmm1 = new RegisterStorage("xmm1", ymm1.Number, 0, PrimitiveType.Word128);
            xmm2 = new RegisterStorage("xmm2", ymm2.Number, 0, PrimitiveType.Word128);
            xmm3 = new RegisterStorage("xmm3", ymm3.Number, 0, PrimitiveType.Word128);
            xmm4 = new RegisterStorage("xmm4", ymm4.Number, 0, PrimitiveType.Word128);
            xmm5 = new RegisterStorage("xmm5", ymm5.Number, 0, PrimitiveType.Word128);
            xmm6 = new RegisterStorage("xmm6", ymm6.Number, 0, PrimitiveType.Word128);
            xmm7 = new RegisterStorage("xmm7", ymm7.Number, 0, PrimitiveType.Word128);
            xmm8 = new RegisterStorage("xmm8", ymm8.Number, 0, PrimitiveType.Word128);
            xmm9 = new RegisterStorage("xmm9", ymm9.Number, 0, PrimitiveType.Word128);
            xmm10 = new RegisterStorage("xmm10", ymm10.Number, 0, PrimitiveType.Word128);
            xmm11 = new RegisterStorage("xmm11", ymm11.Number, 0, PrimitiveType.Word128);
            xmm12 = new RegisterStorage("xmm12", ymm12.Number, 0, PrimitiveType.Word128);
            xmm13 = new RegisterStorage("xmm13", ymm13.Number, 0, PrimitiveType.Word128);
            xmm14 = new RegisterStorage("xmm14", ymm14.Number, 0, PrimitiveType.Word128);
            xmm15 = new RegisterStorage("xmm15", ymm15.Number, 0, PrimitiveType.Word128);

            // Pseudo registers used to reify the x87 FPU stack. Top is the 
            // index into the FPU stack, while ST is the memory identifier that
            // identifies the address space that the FPU stack constitutes.
            Top = factory.Reg("Top", PrimitiveType.SByte);
            ST = new MemoryIdentifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));

            // Control registers: 80 - 88
            // Debug registers: 89 - 96
            mxcsr = factory.Reg32("mxcsr");

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

				eflags,
				rip,
				eip,
				ip,
                FPUF,
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

                 ymm0 ,
                 ymm1 ,
                 ymm2 ,
                 ymm3 ,
                 ymm4 ,
                 ymm5 ,
                 ymm6 ,
                 ymm7 ,
                 ymm8 ,
                 ymm9 ,
                 ymm10,
                 ymm11,
                 ymm12,
                 ymm13,
                 ymm14,
                 ymm15,
            };

            // For each register storage domain, arrange the registers in order of size.
            SubRegisters = new Dictionary<StorageDomain, RegisterStorage[]>
            {
                { rax.Domain, new [] { rax, eax, ax, al, ah,  } },
                { rcx.Domain, new [] { rcx, ecx, cx, cl, ch,  } },
                { rdx.Domain, new [] { rdx, edx, dx, dl, dh,  } },
                { rbx.Domain, new [] { rbx, ebx, bx, bl, bh,  } },
                { rsp.Domain, new [] { rsp, esp, sp, spl, } },
                { rbp.Domain, new [] { rbp, ebp, bp, bpl, } },
                { rsi.Domain, new [] { rsi, esi, si, sil, } },
                { rdi.Domain, new [] { rdi, edi, di, dil, } },
                { r8.Domain, new []  { r8,  r8d, r8w, r8b, } },
                { r9.Domain, new []  { r9,  r9d, r9w, r9b, } },
                { r10.Domain, new [] { r10, r10d, r10w, r10b, } },
                { r11.Domain, new [] { r11, r11d, r11w, r11b, } },
                { r12.Domain, new [] { r12, r12d, r12w, r12b, } },
                { r13.Domain, new [] { r13, r13d, r13w, r13b, } },
                { r14.Domain, new [] { r14, r14d, r14w, r14b, } },
                { r15.Domain, new [] { r15, r15d, r15w, r15b, } },
                { ymm0.Domain, new [] { xmm0 } },
                { ymm1.Domain, new [] { xmm1 } },
                { ymm2.Domain, new [] { xmm2 } },
                { ymm3.Domain, new [] { xmm3 } },
                { ymm4.Domain, new [] { xmm4 } },
                { ymm5.Domain, new [] { xmm5 } },
                { ymm6.Domain, new [] { xmm6 } },
                { ymm7.Domain, new [] { xmm7 } },
                { ymm8.Domain, new [] { xmm8 } },
                { ymm9.Domain, new [] { xmm9 } },
                { ymm10.Domain, new [] { xmm10 } },
                { ymm11.Domain, new [] { xmm11 } },
                { ymm12.Domain, new [] { xmm12 } },
                { ymm13.Domain, new [] { xmm13 } },
                { ymm14.Domain, new [] { xmm14 } },
                { ymm15.Domain, new [] { xmm15 } },
            };

            Gp64BitRegisters = new[]
            {
                rax, rcx, rdx, rbx, rsp, rbp, rsi, rdi, r8, r9, r10, r11, r12, r13, r14, r15,
            };
        }

        private static FlagGroupStorage FlagRegister(string name, RegisterStorage freg, FlagM grf)
        {
            return new FlagGroupStorage(freg, (uint)grf, name, PrimitiveType.Bool);
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
