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
        static AlphaArchitecture arch = new AlphaArchitecture();
        private AlphaDisassembler dasm;
        private MemoryArea image;

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private void RunTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.ParseBitPattern(bits))
                .SelectMany(u => new byte[] { (byte)(u >> 24), (byte)(u >> 16), (byte)(u >> 8), (byte)u })
                .ToArray();
            dasm = new AlphaDisassembler(
                arch,
                new LeImageReader(new MemoryArea(Address.Ptr32(0x00100000), bytes), 0));
        }

        private void AssertCode(uint instr, params string[] sExp)
        {
            Rewrite(instr);
            AssertCode(sExp);
        }

        protected override MemoryArea RewriteCode(uint[] words)
        {
            byte[] bytes = words.SelectMany(w => new byte[]
            {
                (byte) w,
                (byte) (w >> 8),
                (byte) (w >> 16),
                (byte) (w >> 24),
            }).ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            dasm = new AlphaDisassembler(arch, image.CreateBeReader(LoadAddress));
            return image;
        }

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = OperatingEnvironmentElement.LoadHexBytes(hexBytes)
                .ToArray();
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder frame, IRewriterHost host)
        {
            return new AlphaRewriter(arch, new LeImageReader(this.image, 0), new AlphaProcessorState(arch), frame, host);
        }

        [Test]
        public void AlphaRw_lda()
        {
            RewriteCode("B0FFDE23");	// lda	r30,-50(r30)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r30 - 0x0000000000000050");
        }

        [Test]
        public void AlphaRw_halt()
        {
            RewriteCode("00000000");	// halt
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__halt()");
        }

        [Test]
        public void AlphaRw_ldwu()
        {
            RewriteCode("471EED31");	// ldwu	r15,1E47(r13)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = (word64) Mem0[r13 + 0x0000000000001E47:uint16]");
        }

        [Test]
        public void AlphaRw_bgt()
        {
            RewriteCode("FFFFFFFF");	// bgt	zero,029B247C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (0x0000000000000000 > 0x0000000000000000) branch 00100000");
        }

        [Test]
        public void AlphaRw_s8subl()
        {
            RewriteCode("77735C43");	// s8subl	r26,E3,r23
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = (word64) SLICE(r26 * 0x0000000000000008 - 0xE3, int32, 0)");

        }

        [Test]
        public void AlphaRw_jsr()
        {
            RewriteCode("67656469");	// jsr	r11,r4
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|L--|r11 = 00100004",
                "2|T--|goto r4");
        }

        [Test]
        public void AlphaRw_stw()
        {
            RewriteCode("44495434");	// stw	r2,4944(r20)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r20 + 0x0000000000004944:word16] = (word16) r2");
        }

        [Test]
        public void AlphaRw_ldf()
        {
            RewriteCode("00000080");	// ldf	f0,0(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|f0 = (real64) Mem0[r0:real32]");
        }

        [Test]
        public void AlphaRw_ldl()
        {
            RewriteCode("3E0AD7A3");	// ldl	r30,A3E(r23)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = (word64) Mem0[r23 + 0x0000000000000A3E:int32]");
        }

        [Test]
        public void AlphaRw_xor()
        {
            RewriteCode("01782A46");	// xor	r17,53,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r17 ^ 0x53");
        }

        [Test]
        public void AlphaRw_stq()
        {
            RewriteCode("00003EB5");	// stq	r9,0(r30)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r30:word64] = r9");
        }

        [Test]
        public void AlphaRw_bis()
        {
            RewriteCode("0904F047");	// bis	zero,r0,r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r0");
        }

        [Test]
        public void AlphaRw_beq()
        {
            RewriteCode("290020E4");	// beq	r1,029B2A18
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r1 == 0x0000000000000000) branch 001000A8");
        }

        [Test]
        public void AlphaRw_zapnot()
        {
            RewriteCode("2A76404A");	// zapnot	r18,03,r10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = __zapnot(r18, 0x03)");
        }

        [Test]
        public void AlphaRw_bne()
        {
            RewriteCode("130020F6");	// bne	r17,029B29D8
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r17 != 0x0000000000000000) branch 00100050");
        }

        [Test]
        public void AlphaRw_br()
        {
            RewriteCode("2800E0C3");	// br	zero,001000A4
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 001000A4");
        }

        [Test]
        public void AlphaRw_ldah()
        {
            RewriteCode("9B02DF24");	// ldah	r6,29B(zero)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = 0x00000000029B0000");
        }

        [Test]
        public void AlphaRw_stl()
        {
            RewriteCode("080020B0");	// stl	r1,8(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r0 + 0x0000000000000008:word32] = (word32) r1");
        }

        [Test]
        public void AlphaRw_bsr()
        {
            RewriteCode("1C0040D3");	// bsr	r26,029B2A60
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100074 (0)");
        }

        [Test]
        public void AlphaRw_ldq()
        {
            RewriteCode("10005EA7");	// ldq	r26,10(r30)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r26 = Mem0[r30 + 0x0000000000000010:word64]");
        }

        [Test]
        public void AlphaRw_ret()
        {
            RewriteCode("0180FA6B");	// ret	zero,r26
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void AlphaRw_s8addl()
        {
            RewriteCode("41022140");	// s8addl	r1,r8,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (word64) SLICE(r1 * 0x0000000000000008 + r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_addl()
        {
            RewriteCode("0D000140");	// addl	r0,r8,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = (word64) SLICE(r0 + r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_subl()
        {
            RewriteCode("21018141");	// subl	r12,r8,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = (word64) SLICE(r12 - r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_blbc()
        {
            RewriteCode("0A00A0E1");	// blbc	r13,029B2C04
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if ((r13 & 0x0000000000000001) == 0x0000000000000000) branch 0010002C");
        }

        [Test]
        public void AlphaRw_cmpult()
        {
            RewriteCode("AD938041");	// cmpult	r12,04,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = (word64) (r12 <u 0x04)");
        }

        [Test]
        public void AlphaRw_extwl()
        {
            RewriteCode("D852604A");	// extwl	r19,02,r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = __extwl(r19, 0x02)");
        }

        [Test]
        public void AlphaRw_sll()
        {
            RewriteCode("3317664A");	// sll	r19,30,r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = r19 << 0x30");
        }

        [Test]
        public void AlphaRw_insll()
        {
            RewriteCode("78D5004B");	// insll	r24,06,r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = __insll(r24, 0x06)");
        }

        [Test]
        public void AlphaRw_src()
        {
            RewriteCode("9117664A");	// src	r19,30,r17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = __src(r19, 0x30)");
        }

        [Test]
        public void AlphaRw_s4addl()
        {
            RewriteCode("5C003C42");	// s4addl	r17,r0,r28
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = (word64) SLICE(r17 * 0x0000000000000004 + r0, int32, 0)");
        }

        [Test]
        public void AlphaRw_jmp()
        {
            RewriteCode("0000FC6B");	// jmp	zero,r28
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r28");
        }

        [Test]
        public void AlphaRw_subq()
        {
            RewriteCode("2205E043");	// subq	zero,r0,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = -r0");
        }

        [Test]
        public void AlphaRw_cmovge()
        {
            RewriteCode("C2080044");	// cmovge	r0,r0,r2
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r0 < 0x0000000000000000) branch 00100004",
                "2|L--|r2 = r0");
        }

        [Test]
        public void AlphaRw_mulq()
        {
            RewriteCode("0204434C");	// mulq	r2,r24,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 * r24");
        }

        [Test]
        public void AlphaRw_srl()
        {
            RewriteCode("82D64448");	// srl	r2,26,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r2 >>u 0x26");
        }

        [Test]
        public void AlphaRw_s4subl()
        {
            RewriteCode("6E01AD41");	// s4subl	r13,r8,r14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r14 = (word64) SLICE(r13 * 0x0000000000000004 - r8, int32, 0)");
        }

        [Test]
        public void AlphaRw_cmplt()
        {
            RewriteCode("A2091F40");	// cmplt	r0,r24,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = (word64) (r0 < r24)");
        }

        [Test]
        public void AlphaRw_cmpule()
        {
            RewriteCode("B1B74042");	// cmpule	r18,05,r17
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = (word64) (r18 <=u 0x05)");
        }

        [Test]
        public void AlphaRw_cmovne()
        {
            RewriteCode("C0045346");	// cmovne	r18,r24,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (r18 == 0x0000000000000000) branch 00100004",
                "2|L--|r0 = r24");
        }

        [Test]
        public void AlphaRw_mull()
        {
            RewriteCode("1200234C");	// mull	r1,r24,r18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = (word64) SLICE(r1 * r24, int32, 0)");
        }

        [Test]
        public void AlphaRw_cmple()
        {
            RewriteCode("B60D9640");	// cmple	r4,r16,r22
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r22 = (word64) (r4 <= r16)");
        }

        [Test]
        public void AlphaRw_and()
        {
            RewriteCode("18F0E046");	// and	r23,07,r24
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r24 = r23 & 0x07");
        }

        [Test]
        public void AlphaRw_blt()
        {
            RewriteCode("FB00E0EA");	// blt	r23,029B3898
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 < 0x0000000000000000) branch 001003F0");
        }

        [Test]
        public void AlphaRw_ble()
        {
            RewriteCode("6500C0EC");	// ble	r6,029B3740
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r6 <= 0x0000000000000000) branch 00100198");
        }

        [Test]
        public void AlphaRw_ldq_u()
        {
            RewriteCode("0000402C");	// ldq_u	r2,0(r0)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = Mem0[r0:word64]");
        }

        [Test]
        public void AlphaRw_extbl()
        {
            RewriteCode("CD004048");	// extbl	r2,r0,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = __extbl(r2, r0)");
        }

        [Test]
        public void AlphaRw_bic()
        {
            RewriteCode("03F13F44");	// bic	r1,FF,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r1 & ~0xFF");
        }

        [Test]
        public void AlphaRw_blbs()
        {
            RewriteCode("020000F0");	// blbs	r0,029B39DC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if ((r0 & 0x0000000000000001) != 0x0000000000000000) branch 0010000C");
        }

        [Test]
        public void AlphaRw_bge()
        {
            RewriteCode("020020FA");	// bge	r17,029B3B78
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r17 >= 0x0000000000000000) branch 0010000C");
        }

        [Test]
        public void AlphaRw_insbl()
        {
            RewriteCode("65016649");	// insbl	r11,r16,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __insbl(r11, r16)");
        }

        [Test]
        public void AlphaRw_mskbl()
        {
            RewriteCode("44008648");	// mskbl	r4,r16,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __mskbl(r4, r16)");
        }

        [Test]
        public void AlphaRw_addq()
        {
            RewriteCode("10348041");	// addq	r12,01,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = r12 + 0x01");
        }

        [Test]
        public void AlphaRw_cmpeq()
        {
            RewriteCode("B2050142");	// cmpeq	r16,r8,r18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = (word64) (r16 == r8)");
        }

        [Test]
        public void AlphaRw_extqh()
        {
            RewriteCode("460FC748");	// extqh	r6,r24,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __extqh(r6, r24)");
        }

        [Test]
        public void AlphaRw_cmovlbc()
        {
            RewriteCode("D112A045");	// cmovlbc	r13,00,r17
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if ((r13 & 0x0000000000000001) != 0x0000000000000000) branch 00100004",
                "2|L--|r17 = 0x00");
        }

        [Test]
        public void AlphaRw_extll()
        {
            RewriteCode("C504A448");	// extll	r5,r0,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __extll(r5, r0)");
        }

        [Test]
        public void AlphaRw_extlh()
        {
            RewriteCode("460DC448");	// extlh	r6,r0,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = __extlh(r6, r0)");
        }

        [Test]
        public void AlphaRw_inswl()
        {
            RewriteCode("6F736548");	// inswl	r3,2B,r15
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = __inswl(r3, 0x2B)");
        }

        [Test]
        public void AlphaRw_s8subq()
        {
            RewriteCode("61676542");	// s8subq	r19,r11,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r19 * 0x0000000000000008 - r11");
        }

        [Test]
        public void AlphaRw_s4subq()
        {
            RewriteCode("77657241");	// s4subq	r11,r19,r23
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r11 * 0x0000000000000004 - r19");
        }
    }
}
