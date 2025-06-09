#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;

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
        public static readonly FlagGroupStorage CO;
        public static readonly FlagGroupStorage CZ;
        public static readonly FlagGroupStorage CZP;
        public static readonly FlagGroupStorage SCZ;
        public static readonly FlagGroupStorage SCZO;
        public static readonly FlagGroupStorage SCZOP;
        public static readonly FlagGroupStorage SCZDOP;
        public static readonly FlagGroupStorage SO;
        public static readonly FlagGroupStorage SZ;
        public static readonly FlagGroupStorage SZO;
        public static readonly FlagGroupStorage SZP;

        public static readonly RegisterStorage eflags;

        public static readonly RegisterStorage FPUF;
        public static readonly RegisterStorage FPST;    // virtual register; the x87 FPU stack pointer.
        public static readonly FlagGroupStorage C0;
        public static readonly FlagGroupStorage C1;
        public static readonly FlagGroupStorage C2;
        public static readonly FlagGroupStorage C3;

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
        public static readonly RegisterStorage r16;
        public static readonly RegisterStorage r17;
        public static readonly RegisterStorage r18;
        public static readonly RegisterStorage r19;
        public static readonly RegisterStorage r20;
        public static readonly RegisterStorage r21;
        public static readonly RegisterStorage r22;
        public static readonly RegisterStorage r23;
        public static readonly RegisterStorage r24;
        public static readonly RegisterStorage r25;
        public static readonly RegisterStorage r26;
        public static readonly RegisterStorage r27;
        public static readonly RegisterStorage r28;
        public static readonly RegisterStorage r29;
        public static readonly RegisterStorage r30;
        public static readonly RegisterStorage r31;

        public static readonly RegisterStorage r8d;
        public static readonly RegisterStorage r9d;
        public static readonly RegisterStorage r10d;
        public static readonly RegisterStorage r11d;
        public static readonly RegisterStorage r12d;
        public static readonly RegisterStorage r13d;
        public static readonly RegisterStorage r14d;
        public static readonly RegisterStorage r15d;
        public static readonly RegisterStorage r16d;
        public static readonly RegisterStorage r17d;
        public static readonly RegisterStorage r18d;
        public static readonly RegisterStorage r19d;
        public static readonly RegisterStorage r20d;
        public static readonly RegisterStorage r21d;
        public static readonly RegisterStorage r22d;
        public static readonly RegisterStorage r23d;
        public static readonly RegisterStorage r24d;
        public static readonly RegisterStorage r25d;
        public static readonly RegisterStorage r26d;
        public static readonly RegisterStorage r27d;
        public static readonly RegisterStorage r28d;
        public static readonly RegisterStorage r29d;
        public static readonly RegisterStorage r30d;
        public static readonly RegisterStorage r31d;

        public static readonly RegisterStorage r8w;
        public static readonly RegisterStorage r9w;
        public static readonly RegisterStorage r10w;
        public static readonly RegisterStorage r11w;
        public static readonly RegisterStorage r12w;
        public static readonly RegisterStorage r13w;
        public static readonly RegisterStorage r14w;
        public static readonly RegisterStorage r15w;
        public static readonly RegisterStorage r16w;
        public static readonly RegisterStorage r17w;
        public static readonly RegisterStorage r18w;
        public static readonly RegisterStorage r19w;
        public static readonly RegisterStorage r20w;
        public static readonly RegisterStorage r21w;
        public static readonly RegisterStorage r22w;
        public static readonly RegisterStorage r23w;
        public static readonly RegisterStorage r24w;
        public static readonly RegisterStorage r25w;
        public static readonly RegisterStorage r26w;
        public static readonly RegisterStorage r27w;
        public static readonly RegisterStorage r28w;
        public static readonly RegisterStorage r29w;
        public static readonly RegisterStorage r30w;
        public static readonly RegisterStorage r31w;

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
        public static readonly RegisterStorage r16b;
        public static readonly RegisterStorage r17b;
        public static readonly RegisterStorage r18b;
        public static readonly RegisterStorage r19b;
        public static readonly RegisterStorage r20b;
        public static readonly RegisterStorage r21b;
        public static readonly RegisterStorage r22b;
        public static readonly RegisterStorage r23b;
        public static readonly RegisterStorage r24b;
        public static readonly RegisterStorage r25b;
        public static readonly RegisterStorage r26b;
        public static readonly RegisterStorage r27b;
        public static readonly RegisterStorage r28b;
        public static readonly RegisterStorage r29b;
        public static readonly RegisterStorage r30b;
        public static readonly RegisterStorage r31b;

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
        public static readonly RegisterStorage xmm16;
        public static readonly RegisterStorage xmm17;
        public static readonly RegisterStorage xmm18;
        public static readonly RegisterStorage xmm19;
        public static readonly RegisterStorage xmm20;
        public static readonly RegisterStorage xmm21;
        public static readonly RegisterStorage xmm22;
        public static readonly RegisterStorage xmm23;
        public static readonly RegisterStorage xmm24;
        public static readonly RegisterStorage xmm25;
        public static readonly RegisterStorage xmm26;
        public static readonly RegisterStorage xmm27;
        public static readonly RegisterStorage xmm28;
        public static readonly RegisterStorage xmm29;
        public static readonly RegisterStorage xmm30;
        public static readonly RegisterStorage xmm31;

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
        public static readonly RegisterStorage ymm16;
        public static readonly RegisterStorage ymm17;
        public static readonly RegisterStorage ymm18;
        public static readonly RegisterStorage ymm19;
        public static readonly RegisterStorage ymm20;
        public static readonly RegisterStorage ymm21;
        public static readonly RegisterStorage ymm22;
        public static readonly RegisterStorage ymm23;
        public static readonly RegisterStorage ymm24;
        public static readonly RegisterStorage ymm25;
        public static readonly RegisterStorage ymm26;
        public static readonly RegisterStorage ymm27;
        public static readonly RegisterStorage ymm28;
        public static readonly RegisterStorage ymm29;
        public static readonly RegisterStorage ymm30;
        public static readonly RegisterStorage ymm31;

        public static readonly RegisterStorage zmm0;
        public static readonly RegisterStorage zmm1;
        public static readonly RegisterStorage zmm2;
        public static readonly RegisterStorage zmm3;
        public static readonly RegisterStorage zmm4;
        public static readonly RegisterStorage zmm5;
        public static readonly RegisterStorage zmm6;
        public static readonly RegisterStorage zmm7;
        public static readonly RegisterStorage zmm8;
        public static readonly RegisterStorage zmm9;
        public static readonly RegisterStorage zmm10;
        public static readonly RegisterStorage zmm11;
        public static readonly RegisterStorage zmm12;
        public static readonly RegisterStorage zmm13;
        public static readonly RegisterStorage zmm14;
        public static readonly RegisterStorage zmm15;
        public static readonly RegisterStorage zmm16;
        public static readonly RegisterStorage zmm17;
        public static readonly RegisterStorage zmm18;
        public static readonly RegisterStorage zmm19;
        public static readonly RegisterStorage zmm20;
        public static readonly RegisterStorage zmm21;
        public static readonly RegisterStorage zmm22;
        public static readonly RegisterStorage zmm23;
        public static readonly RegisterStorage zmm24;
        public static readonly RegisterStorage zmm25;
        public static readonly RegisterStorage zmm26;
        public static readonly RegisterStorage zmm27;
        public static readonly RegisterStorage zmm28;
        public static readonly RegisterStorage zmm29;
        public static readonly RegisterStorage zmm30;
        public static readonly RegisterStorage zmm31;

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

        public static readonly RegisterStorage k0;
        public static readonly RegisterStorage k1;
        public static readonly RegisterStorage k2;
        public static readonly RegisterStorage k3;
        public static readonly RegisterStorage k4;
        public static readonly RegisterStorage k5;
        public static readonly RegisterStorage k6;
        public static readonly RegisterStorage k7;

        public static readonly RegisterStorage rip;
        public static readonly RegisterStorage eip;
        public static readonly RegisterStorage ip;
        public static readonly RegisterStorage riz;
        public static readonly RegisterStorage eiz;

        public static readonly RegisterStorage Top;     // The x87 stack pointer is modelled explicitly.
        public static readonly Identifier ST;

        public static readonly RegisterStorage mxcsr;

        internal static readonly Dictionary<StorageDomain, RegisterStorage[]> SubRegisters;

        internal static readonly RegisterStorage[] All;

        internal static readonly RegisterStorage[] Gp64BitRegisters;
        internal static readonly RegisterStorage[] Gp32BitRegisters;
        internal static readonly RegisterStorage[] Gp16BitRegisters;
        internal static readonly RegisterStorage[] Gp8BitRegisters;
        internal static readonly RegisterStorage[] Gp8BitRegisters_Rex;
        internal static readonly RegisterStorage[] XmmRegisters;
        internal static readonly RegisterStorage[] YmmRegisters;
        internal static readonly RegisterStorage[] ZmmRegisters;
        internal static readonly RegisterStorage[] MaskRegisters;

        public const int ControlRegisterMin = 80;
        public const int DebugRegisterMin = 89;

        internal static readonly FlagGroupStorage[] EflagsBits;
        internal static readonly FlagGroupStorage[] FpuFlagsBits;

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
            r16 = factory.Reg64("r16");
            r17 = factory.Reg64("r17");
            r18 = factory.Reg64("r18");
            r19 = factory.Reg64("r19");
            r20 = factory.Reg64("r20");
            r21 = factory.Reg64("r21");
            r22 = factory.Reg64("r22");
            r23 = factory.Reg64("r23");
            r24 = factory.Reg64("r24");
            r25 = factory.Reg64("r25");
            r26 = factory.Reg64("r26");
            r27 = factory.Reg64("r27");
            r28 = factory.Reg64("r28");
            r29 = factory.Reg64("r29");
            r30 = factory.Reg64("r30");
            r31 = factory.Reg64("r31");

            eax = RegisterStorage.Reg32("eax", rax.Number);
            ecx = RegisterStorage.Reg32("ecx", rcx.Number);
            edx = RegisterStorage.Reg32("edx", rdx.Number);
            ebx = RegisterStorage.Reg32("ebx", rbx.Number);
            esp = RegisterStorage.Reg32("esp", rsp.Number);
            ebp = RegisterStorage.Reg32("ebp", rbp.Number);
            esi = RegisterStorage.Reg32("esi", rsi.Number);
            edi = RegisterStorage.Reg32("edi", rdi.Number);
            ax = RegisterStorage.Reg16("ax", rax.Number);
            cx = RegisterStorage.Reg16("cx", rcx.Number);
            dx = RegisterStorage.Reg16("dx", rdx.Number);
            bx = RegisterStorage.Reg16("bx", rbx.Number);
            sp = RegisterStorage.Reg16("sp", rsp.Number);
            bp = RegisterStorage.Reg16("bp", rbp.Number);
            si = RegisterStorage.Reg16("si", rsi.Number);
            di = RegisterStorage.Reg16("di", rdi.Number);
            al = RegisterStorage.Reg8("al", rax.Number);
            cl = RegisterStorage.Reg8("cl", rcx.Number);
            dl = RegisterStorage.Reg8("dl", rdx.Number);
            bl = RegisterStorage.Reg8("bl", rbx.Number);
            ah = RegisterStorage.Reg8("ah", rax.Number, 8);
            ch = RegisterStorage.Reg8("ch", rcx.Number, 8);
            dh = RegisterStorage.Reg8("dh", rdx.Number, 8);
            bh = RegisterStorage.Reg8("bh", rbx.Number, 8);
            es = factory.Reg("es", PrimitiveType.SegmentSelector);
            cs = factory.Reg("cs", PrimitiveType.SegmentSelector);
            ss = factory.Reg("ss", PrimitiveType.SegmentSelector);
            ds = factory.Reg("ds", PrimitiveType.SegmentSelector);
            fs = factory.Reg("fs", PrimitiveType.SegmentSelector);
            gs = factory.Reg("gs", PrimitiveType.SegmentSelector);
            rip = factory.Reg64("rip");

            eip = RegisterStorage.Reg32("eip", rip.Number);
            ip = RegisterStorage.Reg32("ip", rip.Number);

            // Not a real architectural register, but used in some SIB encodings.
            riz = factory.Reg64("riz");
            eiz = RegisterStorage.Reg32("eiz", riz.Number);

            eflags = factory.Reg32("eflags");
            S = FlagRegister("S", eflags, FlagM.SF);
            C = FlagRegister("C", eflags, FlagM.CF);
            Z = FlagRegister("Z", eflags, FlagM.ZF);
            D = FlagRegister("D", eflags, FlagM.DF);
            O = FlagRegister("O", eflags, FlagM.OF);
            P = FlagRegister("P", eflags, FlagM.PF);
            CO = FlagRegister("CO", eflags, FlagM.CF | FlagM.OF);
            CZ = FlagRegister("CZ", eflags, FlagM.CF | FlagM.ZF);
            CZP = FlagRegister("CZP", eflags, FlagM.CF | FlagM.ZF | FlagM.PF);
            SCZ = FlagRegister("SCZ", eflags, FlagM.SF | FlagM.CF | FlagM.ZF);
            SCZO = FlagRegister("SCZO", eflags, FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.OF);
            SCZOP = FlagRegister("SCZOP", eflags, FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.OF | FlagM.PF);
            SCZDOP = FlagRegister("SCZDOP", eflags, FlagM.SF | FlagM.CF | FlagM.ZF | FlagM.DF | FlagM.OF | FlagM.PF);
            SO = FlagRegister("SO", eflags, FlagM.SF | FlagM.OF);
            SZ = FlagRegister("SZ", eflags, FlagM.SF | FlagM.ZF);
            SZO = FlagRegister("SZO", eflags, FlagM.SF | FlagM.ZF | FlagM.OF);
            SZP = FlagRegister("SZP", eflags, FlagM.SF | FlagM.ZF | FlagM.PF);
            EflagsBits = new FlagGroupStorage[] { S, C, Z, D, O, P };



            FPUF = factory.Reg("FPUF", PrimitiveType.Word16);
            FPST = factory.Reg("FPST", PrimitiveType.Byte);
            C0 = FlagRegister("C0", FPUF, 0x0100);
            C1 = FlagRegister("C1", FPUF, 0x0200);
            C2 = FlagRegister("C2", FPUF, 0x0400);
            C3 = FlagRegister("C3", FPUF, 0x4000);
            FpuFlagsBits = new FlagGroupStorage[] { C0, C1, C2, C3 };

            r8d = RegisterStorage.Reg32("r8d", r8.Number);
            r9d = RegisterStorage.Reg32("r9d", r9.Number);
            r10d = RegisterStorage.Reg32("r10d", r10.Number);
            r11d = RegisterStorage.Reg32("r11d", r11.Number);
            r12d = RegisterStorage.Reg32("r12d", r12.Number);
            r13d = RegisterStorage.Reg32("r13d", r13.Number);
            r14d = RegisterStorage.Reg32("r14d", r14.Number);
            r15d = RegisterStorage.Reg32("r15d", r15.Number);
            r16d = RegisterStorage.Reg32("r16d", r16.Number);
            r17d = RegisterStorage.Reg32("r17d", r17.Number);
            r18d = RegisterStorage.Reg32("r18d", r18.Number);
            r19d = RegisterStorage.Reg32("r19d", r19.Number);
            r20d = RegisterStorage.Reg32("r20d", r20.Number);
            r21d = RegisterStorage.Reg32("r21d", r21.Number);
            r22d = RegisterStorage.Reg32("r22d", r22.Number);
            r23d = RegisterStorage.Reg32("r23d", r23.Number);
            r24d = RegisterStorage.Reg32("r24d", r24.Number);
            r25d = RegisterStorage.Reg32("r25d", r25.Number);
            r26d = RegisterStorage.Reg32("r26d", r26.Number);
            r27d = RegisterStorage.Reg32("r27d", r27.Number);
            r28d = RegisterStorage.Reg32("r28d", r28.Number);
            r29d = RegisterStorage.Reg32("r29d", r29.Number);
            r30d = RegisterStorage.Reg32("r30d", r30.Number);
            r31d = RegisterStorage.Reg32("r31d", r31.Number);


            r8w = RegisterStorage.Reg16("r8w", r8.Number);
            r9w = RegisterStorage.Reg16("r9w", r9.Number);
            r10w = RegisterStorage.Reg16("r10w", r10.Number);
            r11w = RegisterStorage.Reg16("r11w", r11.Number);
            r12w = RegisterStorage.Reg16("r12w", r12.Number);
            r13w = RegisterStorage.Reg16("r13w", r13.Number);
            r14w = RegisterStorage.Reg16("r14w", r14.Number);
            r15w = RegisterStorage.Reg16("r15w", r15.Number);
            r16w = RegisterStorage.Reg32("r16w", r16.Number);
            r17w = RegisterStorage.Reg32("r17w", r17.Number);
            r18w = RegisterStorage.Reg32("r18w", r18.Number);
            r19w = RegisterStorage.Reg32("r19w", r19.Number);
            r20w = RegisterStorage.Reg32("r20w", r20.Number);
            r21w = RegisterStorage.Reg32("r21w", r21.Number);
            r22w = RegisterStorage.Reg32("r22w", r22.Number);
            r23w = RegisterStorage.Reg32("r23w", r23.Number);
            r24w = RegisterStorage.Reg32("r24w", r24.Number);
            r25w = RegisterStorage.Reg32("r25w", r25.Number);
            r26w = RegisterStorage.Reg32("r26w", r26.Number);
            r27w = RegisterStorage.Reg32("r27w", r27.Number);
            r28w = RegisterStorage.Reg32("r28w", r28.Number);
            r29w = RegisterStorage.Reg32("r29w", r29.Number);
            r30w = RegisterStorage.Reg32("r30w", r30.Number);
            r31w = RegisterStorage.Reg32("r31w", r31.Number);

            spl = RegisterStorage.Reg8("spl", rsp.Number);
            bpl = RegisterStorage.Reg8("bpl", rbp.Number);
            sil = RegisterStorage.Reg8("sil", rsi.Number);
            dil = RegisterStorage.Reg8("dil", rdi.Number);

            r8b = RegisterStorage.Reg8("r8b",    r8.Number);
            r9b = RegisterStorage.Reg8("r9b",    r9.Number);
            r10b = RegisterStorage.Reg8("r10b", r10.Number);
            r11b = RegisterStorage.Reg8("r11b", r11.Number);
            r12b = RegisterStorage.Reg8("r12b", r12.Number);
            r13b = RegisterStorage.Reg8("r13b", r13.Number);
            r14b = RegisterStorage.Reg8("r14b", r14.Number);
            r15b = RegisterStorage.Reg8("r15b", r15.Number);
            r16b = RegisterStorage.Reg32("r16b", r16.Number);
            r17b = RegisterStorage.Reg32("r17b", r17.Number);
            r18b = RegisterStorage.Reg32("r18b", r18.Number);
            r19b = RegisterStorage.Reg32("r19b", r19.Number);
            r20b = RegisterStorage.Reg32("r20b", r20.Number);
            r21b = RegisterStorage.Reg32("r21b", r21.Number);
            r22b = RegisterStorage.Reg32("r22b", r22.Number);
            r23b = RegisterStorage.Reg32("r23b", r23.Number);
            r24b = RegisterStorage.Reg32("r24b", r24.Number);
            r25b = RegisterStorage.Reg32("r25b", r25.Number);
            r26b = RegisterStorage.Reg32("r26b", r26.Number);
            r27b = RegisterStorage.Reg32("r27b", r27.Number);
            r28b = RegisterStorage.Reg32("r28b", r28.Number);
            r29b = RegisterStorage.Reg32("r29b", r29.Number);
            r30b = RegisterStorage.Reg32("r30b", r30.Number);
            r31b = RegisterStorage.Reg32("r31b", r31.Number);

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

            zmm0 = factory.Reg("zmm0", PrimitiveType.Word512);
            zmm1 = factory.Reg("zmm1", PrimitiveType.Word512);
            zmm2 = factory.Reg("zmm2", PrimitiveType.Word512);
            zmm3 = factory.Reg("zmm3", PrimitiveType.Word512);
            zmm4 = factory.Reg("zmm4", PrimitiveType.Word512);
            zmm5 = factory.Reg("zmm5", PrimitiveType.Word512);
            zmm6 = factory.Reg("zmm6", PrimitiveType.Word512);
            zmm7 = factory.Reg("zmm7", PrimitiveType.Word512);
            zmm8 = factory.Reg("zmm8", PrimitiveType.Word512);
            zmm9 = factory.Reg("zmm9", PrimitiveType.Word512);
            zmm10 = factory.Reg("zmm10", PrimitiveType.Word512);
            zmm11 = factory.Reg("zmm11", PrimitiveType.Word512);
            zmm12 = factory.Reg("zmm12", PrimitiveType.Word512);
            zmm13 = factory.Reg("zmm13", PrimitiveType.Word512);
            zmm14 = factory.Reg("zmm14", PrimitiveType.Word512);
            zmm15 = factory.Reg("zmm15", PrimitiveType.Word512);
            zmm16 = factory.Reg("zmm16", PrimitiveType.Word512);
            zmm17 = factory.Reg("zmm17", PrimitiveType.Word512);
            zmm18 = factory.Reg("zmm18", PrimitiveType.Word512);
            zmm19 = factory.Reg("zmm19", PrimitiveType.Word512);
            zmm20 = factory.Reg("zmm20", PrimitiveType.Word512);
            zmm21 = factory.Reg("zmm21", PrimitiveType.Word512);
            zmm22 = factory.Reg("zmm22", PrimitiveType.Word512);
            zmm23 = factory.Reg("zmm23", PrimitiveType.Word512);
            zmm24 = factory.Reg("zmm24", PrimitiveType.Word512);
            zmm25 = factory.Reg("zmm25", PrimitiveType.Word512);
            zmm26 = factory.Reg("zmm26", PrimitiveType.Word512);
            zmm27 = factory.Reg("zmm27", PrimitiveType.Word512);
            zmm28 = factory.Reg("zmm28", PrimitiveType.Word512);
            zmm29 = factory.Reg("zmm29", PrimitiveType.Word512);
            zmm30 = factory.Reg("zmm30", PrimitiveType.Word512);
            zmm31 = factory.Reg("zmm31", PrimitiveType.Word512);

            ymm0 = new RegisterStorage("ymm0", zmm0.Number, 0, PrimitiveType.Word256);
            ymm1 = new RegisterStorage("ymm1", zmm1.Number, 0, PrimitiveType.Word256);
            ymm2 = new RegisterStorage("ymm2", zmm2.Number, 0, PrimitiveType.Word256);
            ymm3 = new RegisterStorage("ymm3", zmm3.Number, 0, PrimitiveType.Word256);
            ymm4 = new RegisterStorage("ymm4", zmm4.Number, 0, PrimitiveType.Word256);
            ymm5 = new RegisterStorage("ymm5", zmm5.Number, 0, PrimitiveType.Word256);
            ymm6 = new RegisterStorage("ymm6", zmm6.Number, 0, PrimitiveType.Word256);
            ymm7 = new RegisterStorage("ymm7", zmm7.Number, 0, PrimitiveType.Word256);
            ymm8 = new RegisterStorage("ymm8", zmm8.Number, 0, PrimitiveType.Word256);
            ymm9 = new RegisterStorage("ymm9", zmm9.Number, 0, PrimitiveType.Word256);
            ymm10 = new RegisterStorage("ymm10", zmm10.Number, 0, PrimitiveType.Word256);
            ymm11 = new RegisterStorage("ymm11", zmm11.Number, 0, PrimitiveType.Word256);
            ymm12 = new RegisterStorage("ymm12", zmm12.Number, 0, PrimitiveType.Word256);
            ymm13 = new RegisterStorage("ymm13", zmm13.Number, 0, PrimitiveType.Word256);
            ymm14 = new RegisterStorage("ymm14", zmm14.Number, 0, PrimitiveType.Word256);
            ymm15 = new RegisterStorage("ymm15", zmm15.Number, 0, PrimitiveType.Word256);
            ymm16 = new RegisterStorage("ymm16", zmm16.Number, 0, PrimitiveType.Word256);
            ymm17 = new RegisterStorage("ymm17", zmm17.Number, 0, PrimitiveType.Word256);
            ymm18 = new RegisterStorage("ymm18", zmm18.Number, 0, PrimitiveType.Word256);
            ymm19 = new RegisterStorage("ymm19", zmm19.Number, 0, PrimitiveType.Word256);
            ymm20 = new RegisterStorage("ymm20", zmm20.Number, 0, PrimitiveType.Word256);
            ymm21 = new RegisterStorage("ymm21", zmm21.Number, 0, PrimitiveType.Word256);
            ymm22 = new RegisterStorage("ymm22", zmm22.Number, 0, PrimitiveType.Word256);
            ymm23 = new RegisterStorage("ymm23", zmm23.Number, 0, PrimitiveType.Word256);
            ymm24 = new RegisterStorage("ymm24", zmm24.Number, 0, PrimitiveType.Word256);
            ymm25 = new RegisterStorage("ymm25", zmm25.Number, 0, PrimitiveType.Word256);
            ymm26 = new RegisterStorage("ymm26", zmm26.Number, 0, PrimitiveType.Word256);
            ymm27 = new RegisterStorage("ymm27", zmm27.Number, 0, PrimitiveType.Word256);
            ymm28 = new RegisterStorage("ymm28", zmm28.Number, 0, PrimitiveType.Word256);
            ymm29 = new RegisterStorage("ymm29", zmm29.Number, 0, PrimitiveType.Word256);
            ymm30 = new RegisterStorage("ymm30", zmm30.Number, 0, PrimitiveType.Word256);
            ymm31 = new RegisterStorage("ymm31", zmm31.Number, 0, PrimitiveType.Word256);

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
            xmm16 = new RegisterStorage("xmm16", ymm16.Number, 0, PrimitiveType.Word128);
            xmm17 = new RegisterStorage("xmm17", ymm17.Number, 0, PrimitiveType.Word128);
            xmm18 = new RegisterStorage("xmm18", ymm18.Number, 0, PrimitiveType.Word128);
            xmm19 = new RegisterStorage("xmm19", ymm19.Number, 0, PrimitiveType.Word128);
            xmm20 = new RegisterStorage("xmm20", ymm20.Number, 0, PrimitiveType.Word128);
            xmm21 = new RegisterStorage("xmm21", ymm21.Number, 0, PrimitiveType.Word128);
            xmm22 = new RegisterStorage("xmm22", ymm22.Number, 0, PrimitiveType.Word128);
            xmm23 = new RegisterStorage("xmm23", ymm23.Number, 0, PrimitiveType.Word128);
            xmm24 = new RegisterStorage("xmm24", ymm24.Number, 0, PrimitiveType.Word128);
            xmm25 = new RegisterStorage("xmm25", ymm25.Number, 0, PrimitiveType.Word128);
            xmm26 = new RegisterStorage("xmm26", ymm26.Number, 0, PrimitiveType.Word128);
            xmm27 = new RegisterStorage("xmm27", ymm27.Number, 0, PrimitiveType.Word128);
            xmm28 = new RegisterStorage("xmm28", ymm28.Number, 0, PrimitiveType.Word128);
            xmm29 = new RegisterStorage("xmm29", ymm29.Number, 0, PrimitiveType.Word128);
            xmm30 = new RegisterStorage("xmm30", ymm30.Number, 0, PrimitiveType.Word128);
            xmm31 = new RegisterStorage("xmm31", ymm31.Number, 0, PrimitiveType.Word128);

            k0 = factory.Reg64("k0");
            k1 = factory.Reg64("k1");
            k2 = factory.Reg64("k2");
            k3 = factory.Reg64("k3");
            k4 = factory.Reg64("k4");
            k5 = factory.Reg64("k5");
            k6 = factory.Reg64("k6");
            k7 = factory.Reg64("k7");

            // Pseudo registers used to reify the x87 FPU stack. Top is the 
            // index into the FPU stack, while ST is the memory identifier that
            // identifies the address space that the FPU stack constitutes.
            Top = factory.Reg("Top", PrimitiveType.SByte);
            ST = new Identifier("ST", PrimitiveType.Ptr32, new MemoryStorage("x87Stack", StorageDomain.Register + 400));

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
                r16,
                r17,
                r18,
                r19,
                r20,
                r21,
                r22,
                r23,
                r24,
                r25,
                r26,
                r27,
                r28,
                r29,
                r30,
                r31,

                r8d,
                r9d,
                r10d,
                r11d,
                r12d,
                r13d,
                r14d,
                r15d,
                r16d,
                r17d,
                r18d,
                r19d,
                r20d,
                r21d,
                r22d,
                r23d,
                r24d,
                r25d,
                r26d,
                r27d,
                r28d,
                r29d,
                r30d,
                r31d,

                r8w,
                r9w,
                r10w,
                r11w,
                r12w,
                r13w,
                r14w,
                r15w,
                r16w,
                r17w,
                r18w,
                r19w,
                r20w,
                r21w,
                r22w,
                r23w,
                r24w,
                r25w,
                r26w,
                r27w,
                r28w,
                r29w,
                r30w,
                r31w,

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
                r16b,
                r17b,
                r18b,
                r19b,
                r20b,
                r21b,
                r22b,
                r23b,
                r24b,
                r25b,
                r26b,
                r27b,
                r28b,
                r29b,
                r30b,
                r31b,

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
                 xmm16,
                 xmm17,
                 xmm18,
                 xmm19,
                 xmm20,
                 xmm21,
                 xmm22,
                 xmm23,
                 xmm24,
                 xmm25,
                 xmm26,
                 xmm27,
                 xmm28,
                 xmm29,
                 xmm30,
                 xmm31,

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
                 ymm16,
                 ymm17,
                 ymm18,
                 ymm19,
                 ymm20,
                 ymm21,
                 ymm22,
                 ymm23,
                 ymm24,
                 ymm25,
                 ymm26,
                 ymm27,
                 ymm28,
                 ymm29,
                 ymm30,
                 ymm31,

                 zmm0,
                 zmm1,
                 zmm2,
                 zmm3,
                 zmm4,
                 zmm5,
                 zmm6,
                 zmm7,
                 zmm8,
                 zmm9,
                 zmm10,
                 zmm11,
                 zmm12,
                 zmm13,
                 zmm14,
                 zmm15,
                 zmm16,
                 zmm17,
                 zmm18,
                 zmm19,
                 zmm20,
                 zmm21,
                 zmm22,
                 zmm23,
                 zmm24,
                 zmm25,
                 zmm26,
                 zmm27,
                 zmm28,
                 zmm29,
                 zmm30,
                 zmm31,
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
                { r16.Domain, new [] { r16, r16d, r16w, r16b, } },
                { r17.Domain, new [] { r17, r17d, r17w, r17b, } },
                { r18.Domain, new [] { r18, r18d, r18w, r18b, } },
                { r19.Domain, new [] { r19, r19d, r19w, r19b, } },
                { r20.Domain, new [] { r20, r20d, r20w, r20b, } },
                { r21.Domain, new [] { r21, r21d, r21w, r21b, } },
                { r22.Domain, new [] { r22, r22d, r22w, r22b, } },
                { r23.Domain, new [] { r23, r23d, r23w, r23b, } },
                { r24.Domain, new [] { r24, r24d, r24w, r24b, } },
                { r25.Domain, new [] { r25, r25d, r25w, r25b, } },
                { r26.Domain, new [] { r26, r26d, r26w, r26b, } },
                { r27.Domain, new [] { r27, r27d, r27w, r27b, } },
                { r28.Domain, new [] { r28, r28d, r28w, r28b, } },
                { r29.Domain, new [] { r29, r29d, r29w, r29b, } },
                { r30.Domain, new [] { r30, r30d, r30w, r30b, } },
                { r31.Domain, new [] { r31, r31d, r31w, r31b, } },

                { zmm0.Domain, new [] { zmm0, ymm0, xmm0 } },
                { zmm1.Domain, new [] { zmm1, ymm1, xmm1 } },
                { zmm2.Domain, new [] { zmm2, ymm2, xmm2 } },
                { zmm3.Domain, new [] { zmm3, ymm3, xmm3 } },
                { zmm4.Domain, new [] { zmm4, ymm4, xmm4 } },
                { zmm5.Domain, new [] { zmm5, ymm5, xmm5 } },
                { zmm6.Domain, new [] { zmm6, ymm6, xmm6 } },
                { zmm7.Domain, new [] { zmm7, ymm7, xmm7 } },
                { zmm8.Domain, new [] { zmm8, ymm8, xmm8 } },
                { zmm9.Domain, new [] { zmm9, ymm9, xmm9 } },
                { zmm10.Domain, new [] { zmm10, ymm10, xmm10 } },
                { zmm11.Domain, new [] { zmm11, ymm11, xmm11 } },
                { zmm12.Domain, new [] { zmm12, ymm12, xmm12 } },
                { zmm13.Domain, new [] { zmm13, ymm13, xmm13 } },
                { zmm14.Domain, new [] { zmm14, ymm14, xmm14 } },
                { zmm15.Domain, new [] { zmm15, ymm15, xmm15 } },
                { zmm16.Domain, new [] { zmm16, ymm16, xmm16 } },
                { zmm17.Domain, new [] { zmm17, ymm17, xmm17 } },
                { zmm18.Domain, new [] { zmm18, ymm18, xmm18 } },
                { zmm19.Domain, new [] { zmm19, ymm19, xmm19 } },
                { zmm20.Domain, new [] { zmm20, ymm20, xmm20 } },
                { zmm21.Domain, new [] { zmm21, ymm21, xmm21 } },
                { zmm22.Domain, new [] { zmm22, ymm22, xmm22 } },
                { zmm23.Domain, new [] { zmm23, ymm23, xmm23 } },
                { zmm24.Domain, new [] { zmm24, ymm24, xmm24 } },
                { zmm25.Domain, new [] { zmm25, ymm25, xmm25 } },
                { zmm26.Domain, new [] { zmm26, ymm26, xmm26 } },
                { zmm27.Domain, new [] { zmm27, ymm27, xmm27 } },
                { zmm28.Domain, new [] { zmm28, ymm28, xmm28 } },
                { zmm29.Domain, new [] { zmm29, ymm29, xmm29 } },
                { zmm30.Domain, new [] { zmm30, ymm30, xmm30 } },
                { zmm31.Domain, new [] { zmm31, ymm31, xmm31 } },
            };

            Gp64BitRegisters = new[]
            {
                rax, rcx, rdx, rbx, rsp, rbp, rsi, rdi,
                r8,  r9,  r10, r11, r12, r13, r14, r15,
                r16, r17, r18, r19, r20, r21, r22, r23,
                r24, r25, r26, r27, r28, r29, r30, r31
            };

            Gp32BitRegisters = new[]
            {
                eax,  ecx,  edx,  ebx,  esp,  ebp,  esi,  edi,
                r8d,  r9d,  r10d, r11d, r12d, r13d, r14d, r15d,
                r16d, r17d, r18d, r19d, r20d, r21d, r22d, r23d,
                r24d, r25d, r26d, r27d, r28d, r29d, r30d, r31d
            };

            Gp16BitRegisters = new[]
            {
                ax,   cx,   dx,   bx,   sp,   bp,   si,   di,
                r8w,  r9w,  r10w, r11w, r12w, r13w, r14w, r15w,
                r16w, r17w, r18w, r19w, r20w, r21w, r22w, r23w,
                r24w, r25w, r26w, r27w, r28w, r29w, r30w, r31w
            };

            Gp8BitRegisters = new[]
            {
                al,   cl,   dl,   bl,   ah,   ch,   dh,   bh,
            };

            Gp8BitRegisters_Rex = new[]
            {
                al,   cl,   dl,   bl,   spl,  bpl,  sil,  dil,
                r8b,  r9b,  r10b, r11b, r12b, r13b, r14b, r15b,
                r16b, r17b, r18b, r19b, r20b, r21b, r22b, r23b,
                r24b, r25b, r26b, r27b, r28b, r29b, r30b, r31b
            };

            XmmRegisters = new[]
            {
                xmm0, xmm1, xmm2, xmm3, xmm4, xmm5, xmm6, xmm7,
                xmm8, xmm9, xmm10, xmm11, xmm12, xmm13, xmm14, xmm15,
                xmm16, xmm17, xmm18, xmm19, xmm20, xmm21, xmm22, xmm23,
                xmm24, xmm25, xmm26, xmm27, xmm28, xmm29, xmm30, xmm31,
            };

            YmmRegisters = new[]
            {
                ymm0, ymm1, ymm2, ymm3, ymm4, ymm5, ymm6, ymm7,
                ymm8, ymm9, ymm10, ymm11, ymm12, ymm13, ymm14, ymm15,
                ymm16, ymm17, ymm18, ymm19, ymm20, ymm21, ymm22, ymm23,
                ymm24, ymm25, ymm26, ymm27, ymm28, ymm29, ymm30, ymm31,
            };

            ZmmRegisters = new[]
            {
                zmm0, zmm1, zmm2, zmm3, zmm4, zmm5, zmm6, zmm7,
                zmm8, zmm9, zmm10, zmm11, zmm12, zmm13, zmm14, zmm15,
                zmm16, zmm17, zmm18, zmm19, zmm20, zmm21, zmm22, zmm23,
                zmm24, zmm25, zmm26, zmm27, zmm28, zmm29, zmm30, zmm31,
            };

            MaskRegisters = new[] { k0, k1, k2, k3, k4, k5, k6, k7 };
        }

        private static FlagGroupStorage FlagRegister(string name, RegisterStorage freg, FlagM grf)
        {
            return new FlagGroupStorage(freg, (uint)grf, name);
        }

        private static FlagGroupStorage FlagRegister(string name, RegisterStorage freg, uint grf)
        {
            return new FlagGroupStorage(freg, (uint) grf, name);
        }

        public static RegisterStorage GetRegister(string name)
        {
            for (int i = 0; i < All.Length; ++i)
            {
                if (All[i] is not null && String.Compare(All[i].Name, name, true) == 0)
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
