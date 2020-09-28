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

using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Rtl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Types;
using Reko.Arch.Alpha;
using Reko.Core.Configuration;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class AlphaRewriterTests : RewriterTestBase
    {
        private static readonly AlphaArchitecture arch = new AlphaArchitecture("alpha");
        private static readonly Address addr = Address.Ptr32(0x00100000);
        
        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        private void RunTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.BitStringToUInt32(bits))
                .SelectMany(u => new byte[] { (byte)(u >> 24), (byte)(u >> 16), (byte)(u >> 8), (byte)u })
                .ToArray();
        }

        private void AssertCode(uint instr, params string[] sExp)
        {
            Given_UInt32s(instr);
            AssertCode(sExp);
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return new AlphaRewriter(
                arch, 
                new LeImageReader(mem, 0),
                binder,
                host);
        }

        [Test]
        public void AlphaRw_lda()
        {
            Given_HexString("B0FFDE23");	// lda	r30,-50(r30)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 - 0x0000000000000050");
        }

        [Test]
        public void AlphaRw_halt()
        {
            Given_HexString("00000000");	// halt
            AssertCode(
                "0|H--|00100000(4): 1 instructions",
                "1|H--|__halt()");
        }

        [Test]
        public void AlphaRw_ldwu()
        {
            Given_HexString("471EED31");	// ldwu	r15,1E47(r13)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = (word64) Mem0[r13 + 0x0000000000001E47:uint16]");
        }

        [Test]
        public void AlphaRw_bgt()
        {
            Given_HexString("FFFFFFFF");	// bgt	zero,029B247C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (0x0000000000000000 > 0x0000000000000000) branch 00100000");
        }

        [Test]
        public void AlphaRw_s8subl()
        {
            Given_HexString("77735C43");	// s8subl	r26,E3,r23
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = (word64) SLICE(r26 * 0x0000000000000008 - 0xE3, int32, 0)");

        }

        [Test]
        public void AlphaRw_jsr()
        {
            Given_HexString("67656469");	// jsr	r11,r4
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r11 = 00100004",
                "2|T--|goto r4");
        }

        [Test]
        public void AlphaRw_stw()
        {
            Given_HexString("44495434");	// stw	r2,4944(r20)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r20 + 0x0000000000004944:word16] = (word16) r2");
        }

        [Test]
        public void AlphaRw_ldf()
        {
            Given_HexString("00000080");	// ldf	f0,0(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) Mem0[r0:real32]");
        }

        [Test]
        public void AlphaRw_ldl()
        {
            Given_HexString("3E0AD7A3");	// ldl	r30,A3E(r23)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = (word64) Mem0[r23 + 0x0000000000000A3E:int32]");
        }

        [Test]
        public void AlphaRw_xor()
        {
            Given_HexString("01782A46");	// xor	r17,53,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r17 ^ 0x53");
        }

        [Test]
        public void AlphaRw_stq()
        {
            Given_HexString("00003EB5");	// stq	r9,0(r30)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30:word64] = r9");
        }

        [Test]
        public void AlphaRw_bis()
        {
            Given_HexString("0904F047");	// bis	zero,r0,r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r0");
        }

        [Test]
        public void AlphaRw_beq()
        {
            Given_HexString("290020E4");	// beq	r1,029B2A18
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r1 == 0x0000000000000000) branch 001000A8");
        }

        [Test]
        public void AlphaRw_zapnot()
        {
            Given_HexString("2A76404A");	// zapnot	r18,03,r10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __zapnot(r18, 0x03)");
        }

        [Test]
        public void AlphaRw_bne()
        {
            Given_HexString("130020F6");	// bne	r17,029B29D8
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r17 != 0x0000000000000000) branch 00100050");
        }

        [Test]
        public void AlphaRw_br()
        {
            Given_HexString("2800E0C3");	// br	zero,001000A4
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 001000A4");
        }

        [Test]
        public void AlphaRw_ldah()
        {
            Given_HexString("9B02DF24");	// ldah	r6,29B(zero)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = 0x00000000029B0000");
        }

        [Test]
        public void AlphaRw_stl()
        {
            Given_HexString("080020B0");	// stl	r1,8(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r0 + 0x0000000000000008:word32] = (word32) r1");
        }

        [Test]
        public void AlphaRw_bsr()
        {
            Given_HexString("1C0040D3");	// bsr	r26,029B2A60
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100074 (0)");
        }

        [Test]
        public void AlphaRw_ldq()
        {
            Given_HexString("10005EA7");	// ldq	r26,10(r30)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = Mem0[r30 + 0x0000000000000010:word64]");
        }

        [Test]
        public void AlphaRw_ret()
        {
            Given_HexString("0180FA6B");	// ret	zero,r26
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void AlphaRw_s8addl()
        {
            Given_HexString("41022140");	// s8addl	r1,r8,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (word64) SLICE(r1 * 0x0000000000000008 + r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_addl()
        {
            Given_HexString("0D000140");	// addl	r0,r8,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = (word64) SLICE(r0 + r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_subl()
        {
            Given_HexString("21018141");	// subl	r12,r8,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (word64) SLICE(r12 - r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_blbc()
        {
            Given_HexString("0A00A0E1");	// blbc	r13,029B2C04
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if ((r13 & 0x0000000000000001) == 0x0000000000000000) branch 0010002C");
        }

        [Test]
        public void AlphaRw_cmpult()
        {
            Given_HexString("AD938041");	// cmpult	r12,04,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = (word64) (r12 <u 0x04)");
        }

        [Test]
        public void AlphaRw_extwl()
        {
            Given_HexString("D852604A");	// extwl	r19,02,r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = __extwl(r19, 0x02)");
        }

        [Test]
        public void AlphaRw_sll()
        {
            Given_HexString("3317664A");	// sll	r19,30,r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = r19 << 0x30");
        }

        [Test]
        public void AlphaRw_insll()
        {
            Given_HexString("78D5004B");	// insll	r24,06,r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = __insll(r24, 0x06)");
        }

        [Test]
        public void AlphaRw_src()
        {
            Given_HexString("9117664A");	// src	r19,30,r17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = __src(r19, 0x30)");
        }

        [Test]
        public void AlphaRw_s4addl()
        {
            Given_HexString("5C003C42");	// s4addl	r17,r0,r28
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = (word64) SLICE(r17 * 0x0000000000000004 + r0, int32, 0)");
        }

        [Test]
        public void AlphaRw_jmp()
        {
            Given_HexString("0000FC6B");	// jmp	zero,r28
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r28");
        }

        [Test]
        public void AlphaRw_subq()
        {
            Given_HexString("2205E043");	// subq	zero,r0,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = -r0");
        }

        [Test]
        public void AlphaRw_cmovge()
        {
            Given_HexString("C2080044");	// cmovge	r0,r0,r2
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 < 0x0000000000000000) branch 00100004",
                "2|L--|r2 = r0");
        }

        [Test]
        public void AlphaRw_mulq()
        {
            Given_HexString("0204434C");	// mulq	r2,r24,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 * r24");
        }

        [Test]
        public void AlphaRw_srl()
        {
            Given_HexString("82D64448");	// srl	r2,26,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 >>u 0x26");
        }

        [Test]
        public void AlphaRw_s4subl()
        {
            Given_HexString("6E01AD41");	// s4subl	r13,r8,r14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r14 = (word64) SLICE(r13 * 0x0000000000000004 - r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_cmplt()
        {
            Given_HexString("A2091F40");	// cmplt	r0,r24,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (word64) (r0 < r24)");
        }

        [Test]
        public void AlphaRw_cmpule()
        {
            Given_HexString("B1B74042");	// cmpule	r18,05,r17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = (word64) (r18 <=u 0x05)");
        }

        [Test]
        public void AlphaRw_cmovne()
        {
            Given_HexString("C0045346");	// cmovne	r18,r24,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r18 == 0x0000000000000000) branch 00100004",
                "2|L--|r0 = r24");
        }

        [Test]
        public void AlphaRw_mull()
        {
            Given_HexString("1200234C");	// mull	r1,r24,r18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = (word64) SLICE(r1 * r24, int32, 0)");
        }

        [Test]
        public void AlphaRw_cmple()
        {
            Given_HexString("B60D9640");	// cmple	r4,r16,r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r22 = (word64) (r4 <= r16)");
        }

        [Test]
        public void AlphaRw_and()
        {
            Given_HexString("18F0E046");	// and	r23,07,r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = r23 & 0x07");
        }

        [Test]
        public void AlphaRw_blt()
        {
            Given_HexString("FB00E0EA");	// blt	r23,029B3898
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 < 0x0000000000000000) branch 001003F0");
        }

        [Test]
        public void AlphaRw_ble()
        {
            Given_HexString("6500C0EC");	// ble	r6,029B3740
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r6 <= 0x0000000000000000) branch 00100198");
        }

        [Test]
        public void AlphaRw_ldq_u()
        {
            Given_HexString("0000402C");	// ldq_u	r2,0(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = Mem0[r0:word64]");
        }

        [Test]
        public void AlphaRw_extbl()
        {
            Given_HexString("CD004048");	// extbl	r2,r0,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = __extbl(r2, r0)");
        }

        [Test]
        public void AlphaRw_bic()
        {
            Given_HexString("03F13F44");	// bic	r1,FF,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r1 & ~0xFF");
        }

        [Test]
        public void AlphaRw_blbs()
        {
            Given_HexString("020000F0");	// blbs	r0,029B39DC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if ((r0 & 0x0000000000000001) != 0x0000000000000000) branch 0010000C");
        }

        [Test]
        public void AlphaRw_bge()
        {
            Given_HexString("020020FA");	// bge	r17,029B3B78
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r17 >= 0x0000000000000000) branch 0010000C");
        }

        [Test]
        public void AlphaRw_insbl()
        {
            Given_HexString("65016649");	// insbl	r11,r16,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __insbl(r11, r16)");
        }

        [Test]
        public void AlphaRw_mskbl()
        {
            Given_HexString("44008648");	// mskbl	r4,r16,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __mskbl(r4, r16)");
        }

        [Test]
        public void AlphaRw_addq()
        {
            Given_HexString("10348041");	// addq	r12,01,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = r12 + 0x01");
        }

        [Test]
        public void AlphaRw_cmpeq()
        {
            Given_HexString("B2050142");	// cmpeq	r16,r8,r18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = (word64) (r16 == r8)");
        }

        [Test]
        public void AlphaRw_extqh()
        {
            Given_HexString("460FC748");	// extqh	r6,r24,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __extqh(r6, r24)");
        }

        [Test]
        public void AlphaRw_cmovlbc()
        {
            Given_HexString("D112A045");	// cmovlbc	r13,00,r17
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if ((r13 & 0x0000000000000001) != 0x0000000000000000) branch 00100004",
                "2|L--|r17 = 0x00");
        }

        [Test]
        public void AlphaRw_extll()
        {
            Given_HexString("C504A448");	// extll	r5,r0,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __extll(r5, r0)");
        }

        [Test]
        public void AlphaRw_extlh()
        {
            Given_HexString("460DC448");	// extlh	r6,r0,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __extlh(r6, r0)");
        }

        [Test]
        public void AlphaRw_inswl()
        {
            Given_HexString("6F736548");	// inswl	r3,2B,r15
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = __inswl(r3, 0x2B)");
        }

        [Test]
        public void AlphaRw_s8subq()
        {
            Given_HexString("61676542");	// s8subq	r19,r11,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r19 * 0x0000000000000008 - r11");
        }

        [Test]
        public void AlphaRw_s4subq()
        {
            Given_HexString("77657241");	// s4subq	r11,r19,r23
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r11 * 0x0000000000000004 - r19");
        }

        [Test]
        public void AlphaRw_stb()
        {
            Given_HexString("72616D3A");	// stb	r19,6172(r13)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r13 + 0x0000000000006172:byte] = (byte) r19");
        }

        [Test]
        public void AlphaRw_ldbu()
        {
            Given_HexString("20432B2B");	// ldbu	r25,4320(r11)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r25 = (word64) Mem0[r11 + 0x0000000000004320:byte]");
        }

        [Test]
        public void AlphaRw_fble()
        {
            Given_HexString("CDCCCCCC");	// fble	f6,028E5A40
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (f6 <= 0.0) branch 00433338");
        }

        [Test]
        public void AlphaRw_stq_u()
        {
            Given_HexString("69726D3D");	// stq_u	r11,7269(r13)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stq_u(Mem0[r13 + 0x0000000000007269:word64], r11)");
        }

        [Test]
        public void AlphaRw_stq_c()
        {
            Given_HexString("000080BF");	// stq_c	r28,0(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stq_c(Mem0[r0:word64], r28)");
        }

        [Test]
        public void AlphaRw_sts()
        {
            Given_HexString("9A999999");	// sts	f12,-6666(r25)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r25 - 0x0000000000006666:real32] = (real32) f12");
        }

        [Test]
        public void AlphaRw_ldg()
        {
            Given_HexString("06BD3786");	// ldg	f17,-42FA(r23)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f17 = Mem0[r23 - 0x00000000000042FA:real64]");
        }

        [Test]
        public void AlphaRw_lds()
        {
            Given_HexString("425F7089");	// lds	f11,5F42(r16)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f11 = (real64) Mem0[r16 + 0x0000000000005F42:real32]");
        }

        [Test]
        public void AlphaRw_adds_c()
        {
            Given_HexString("00008059");	// adds_c	f12,f0,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f12 + f0");
        }

        [Test]
        public void AlphaRw_ornot()
        {
            Given_HexString("04454045");	// ornot	r10,r10,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r10 | ~r2");
        }

        [Test]
        public void AlphaRw_ornot_neg1()
        {
            Given_HexString("04454545");	// ornot	r10,r10,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = 0xFFFFFFFFFFFFFFFF");
        }

        [Test]
        public void AlphaRw_addf_c()
        {
            Given_HexString("00004054");	// addf_c	f2,f0,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f2 + f0");
        }

        [Test]
        public void AlphaRw_stl_c()
        {
            Given_HexString("F2913FBA");	// stl_c	r17,-6E0E(zero)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stl_c(Mem0[0xFFFFFFFFFFFF91F2:word32], r17)");
        }

        [Test]
        public void AlphaRw_stt()
        {
            Given_HexString("6263E29F");	// stt	f31,6362(r2)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r2 + 0x0000000000006362:real64] = 0.0");
        }

        [Test]
        public void AlphaRw_fbgt()
        {
            Given_HexString("C8D8D8DC");	// fbgt	f6,023EBA64
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (f6 > 0.0) branch FFF36324");
        }

        [Test]
        public void AlphaRw_fblt()
        {
            Given_HexString("140D72CA");	// fblt	r19,02238BAC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (f19 < 0.0) branch FFD83454");
        }

        [Test]
        public void AlphaRw_stf()
        {
            Given_HexString("F619CE92");	// stf	f22,19F6(r14)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r14 + 0x00000000000019F6:real32] = (real32) f22");
        }

        [Test]
        public void AlphaRw_ldl_l()
        {
            Given_HexString("2C6BBFA8");	// ldl_l	r5,6B2C(zero)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __ldl_l(Mem0[0x0000000000006B2C:word32])");
        }

        [Test]
        public void AlphaRw_jsr_coroutine()
        {
            Given_HexString("F1E4AB69");	// jsr_coroutine	r13,r11
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r13 = 00100004",
                "2|T--|goto r11");
        }

        [Test]
        public void AlphaRw_ldt()
        {
            Given_HexString("8F06848D");	// ldt	f12,68F(r4)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f12 = Mem0[r4 + 0x000000000000068F:real64]");
        }

        [Test]
        public void AlphaRw_ldq_l()
        {
            Given_HexString("D70ADFAC");	// ldq_l	r6,AD7(zero)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __ldq_l(Mem0[0x0000000000000AD7:word64])");
        }

        [Test]
        public void AlphaRw_addq_v()
        {
            Given_HexString("062C0540");	// addq_v	r0,r9,r6
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r6 = r0 + r9",
                "2|T--|if (!OV(r6)) branch 00100004",
                "3|T--|__trap_overflow()");
        }

        [Test]
        public void AlphaRw_subf_s()
        {
            Given_HexString("3190CB57");	// subf_s	f30,f11,f17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f17 = f30 - f11");
        }

        [Test]
        public void AlphaRw_implver()
        {
            Given_HexString("9F4DDB47");	// implver	r30,r26,zero
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__implver(r30, r26)");
        }

        [Test]
        public void AlphaRw_mskqh()
        {
            Given_HexString("4B4E984B");	// mskqh	r28,r2,r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = __mskqh(r28, r2)");
        }

        [Test]
        public void AlphaRw_subf_uc()
        {
            Given_HexString("2E202054");	// subf_uc	f1,f0,f14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f14 = f1 - f0");
        }

        [Test]
        public void AlphaRw_cmpbge()
        {
            Given_HexString("F1510440");	// cmpbge	r0,22,r17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = __cmpbge(r0, 0x22)");
        }

        [Test]
        public void AlphaRw_subq_v()
        {
            Given_HexString("2B7D0240");	// subq_v	r0,13,r11
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r11 = r0 - 0x13",
                "2|T--|if (!OV(r11)) branch 00100004",
                "3|T--|__trap_overflow()");
        }

        [Test]
        public void AlphaRw_subl_v()
        {
            Given_HexString("3BA90040");	// subl_v	r0,r5,r27
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r27 = (word64) SLICE(r0 - r5, int32, 0)",
                "2|T--|if (!OV(r27)) branch 00100004",
                "3|T--|__trap_overflow()");
        }

        [Test]
        public void AlphaRw_addl_v()
        {
            Given_HexString("04B86940");	// addl_v	r3,4D,r4
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|r4 = (word64) SLICE(r3 + 0x4D, int32, 0)",
                "2|T--|if (!OV(r4)) branch 00100004",
                "3|T--|__trap_overflow()");
        }

        [Test]
        public void AlphaRw_trapb()
        {
            Given_HexString("0000FF63");    // trapb
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|__trap_barrier()");
        }

        [Test]
        public void AlphaRw_mskql()
        {
            Given_HexString("5B069B4B");	// mskql	r28,r24,r27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r27 = __mskql(r28, r24)");
        }

        [Test]
        public void AlphaRw_cpys_fnop()
        {
            Given_HexString("1F04FF5F");	// cpys	f31,f31,f31
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }


        [Test]
        public void AlphaRw_cpys()
        {
            Given_HexString("1004FF5F");	// cpys	f31,f31,f16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f16 = 0.0");
        }

        [Test]
        public void AlphaRw_mskwl()
        {
            Given_HexString("48020749");	// mskwl	r8,r24,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = __mskwl(r8, r24)");
        }

        [Test]
        public void AlphaRw_extql()
        {
            Given_HexString("DC06924B");	// extql	r28,r16,r28
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = __extql(r28, r16)");
        }

        [Test]
        public void AlphaRw_mskll()
        {
            Given_HexString("42044448");	// mskll	r2,r0,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __mskll(r2, r0)");
        }

        [Test]
        public void AlphaRw_extwh()
        {
            Given_HexString("460BC448");	// extwh	r6,r0,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __extwh(r6, r0)");
        }

        [Test]
        public void AlphaRw_cmovlt()
        {
            Given_HexString("9308E644");	// cmovlt	r7,r16,r19
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r7 >= 0x0000000000000000) branch 00100004",
                "2|L--|r19 = r16");
        }

        [Test]
        public void AlphaRw_cvtqt()
        {
            Given_HexString("C017E05B");	// cvtqt	f31,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = 0.0");
        }

        [Test]
        public void AlphaRw_cvtlq()
        {
            Given_HexString("0002E05F");	// cvtlq	f31,f0,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = 0");
        }

        [Test]
        public void AlphaRw_umulh()
        {
            Given_HexString("0006014C");	// umulh	r0,r8,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 *u r8 >>u 0x40");
        }

        [Test]
        public void AlphaRw_msklh()
        {
            Given_HexString("560CD74A");	// msklh	r22,r24,r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r22 = __msklh(r22, r24)");
        }

        [Test]
        public void AlphaRw_insql()
        {
            Given_HexString("7C07604B");	// insql	r27,r0,r28
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = __insql(r27, r0)");
        }

        [Test]
        public void AlphaRw_divs()
        {
            Given_HexString("60101258");	// divs	f0,f18,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 / f18");
        }

        [Test]
        public void AlphaRw_adds()
        {
            Given_HexString("00101158");	// adds	f0,f17,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 + f17");
        }

        [Test]
        public void AlphaRw_subs()
        {
            Given_HexString("2010405A");	// subs	f18,f0,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f18 - f0");
        }

        [Test]
        public void AlphaRw_muls()
        {
            Given_HexString("40104059");	// muls	f10,f0,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f10 * f0");
        }

        [Test]
        public void AlphaRw_cmptle()
        {
            Given_HexString("EB140B58");	// cmptle	f0,f11,f11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f11 = f0 <= f11 ? 2.0 : 0.0");
        }

        [Test]
        public void AlphaRw_cvttq_c()
        {
            Given_HexString("E005E05B");	// cvttq_c	f31,f0,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = 0");
        }

        [Test]
        public void AlphaRw_cmpteq()
        {
            Given_HexString("A0140258");	// cmpteq	f0,f2,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 == f2 ? 2.0 : 0.0");
        }

        [Test]
        public void AlphaRw_mult()
        {
            Given_HexString("40140158");	// mult	f0,f1,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 * f1");
        }

        [Test]
        public void AlphaRw_cvtts_zero()
        {
            Given_HexString("8015E05B");	// cvtts	f31,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = 0.0F");
        }

        [Test]
        public void AlphaRw_cvtts()
        {
            Given_HexString("8015605B");	// cvtts	f31,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real32) f27");
        }

        [Test]
        public void AlphaRw_fcmovne()
        {
            Given_HexString("6005415D");	// fcmovne	f10,f1,f0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (f10 == 0.0) branch 00100004",
                "2|L--|f0 = f1");
        }

        [Test]
        public void AlphaRw_cvtql()
        {
            Given_HexString("0106EA5F");	// cvtql	f31,f10,f1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f10 = 0");
        }

        [Test]
        public void AlphaRw_cvtqs()
        {
            Given_HexString("8517E55B");	// cvtqs	f31,f5,f5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f5 = 0.0F");
        }

        [Test]
        public void AlphaRw_addt()
        {
            Given_HexString("00140158");	// addt	f0,f1,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 + f1");
        }

        [Test]
        public void AlphaRw_subt()
        {
            Given_HexString("20140158");	// subt	f0,f1,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = f0 - f1");
        }

        [Test]
        public void AlphaRw_cpyse()
        {
            Given_HexString("4004305C");	// cpyse	f1,f16,f0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = __cpyse(f1, f16)");
        }

        [Test]
        public void AlphaRw_mult_c()
        {
            Given_HexString("5504545A");	// mult_c	f18,f20,f21
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f21 = f18 * f20");
        }

        [Test]
        public void AlphaRw_fcmoveq()
        {
            Given_HexString("4E05D45E");	// fcmoveq	f22,f20,f14
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (f22 != 0.0) branch 00100004",
                "2|L--|f14 = f20");
        }

        [Test]
        public void AlphaRw_cpysn()
        {
            Given_HexString("3604D65E");	// cpysn	f22,f22,f22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f22 = __cpysn(f22, f22)");
        }
    }
}