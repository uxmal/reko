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

using NUnit.Framework;
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch64;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public class A64RewriterTests : RewriterTestBase
    {
        private Arm64Architecture arch = new Arm64Architecture("aarch64");
        private Address baseAddress = Address.Ptr64(0x00100000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => baseAddress;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(new LeImageReader(mem, 0), new Arm64State(arch), binder, host);
        }

        private void Given_Instruction(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.BitStringToUInt32(bits))
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            base.Given_MemoryArea(new MemoryArea(Address.Ptr32(0x00100000), bytes));
        }

        private void Given_Instruction(params uint[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            Given_MemoryArea(new MemoryArea(Address.Ptr32(0x00100000), bytes));
        }

        [Test]
        public void A64Rw_b_label()
        {
            Given_Instruction("00010111 11111111 11111111 00000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000FFC00");
        }

        [Test]
        public void A64Rw_bl_label()
        {
            Given_Instruction("10010111 11111111 11111111 00000000");
            AssertCode(     // bl\t#&FFC00
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 000FFC00 (0)");
        }

        [Test]
        public void A64Rw_br_Xn()
        {
            Given_Instruction("11010110 00011111 00000011 11000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto x30");
        }

        [Test]
        public void A64Rw_add_Wn_imm()
        {
            Given_Instruction("000 10001 01 011111111111 10001 10011");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w19 = w17 + (0x000007FF << 12)");
        }

        [Test]
        public void A64Rw_adds_Xn_imm()
        {
            Given_Instruction("101 10001 00 011111111111 10001 10011");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|x19 = x17 + 0x00000000000007FF",
                "2|L--|NZCV = cond(x19)");
        }

        [Test]
        public void A64Rw_subs_Wn_imm()
        {
            Given_Instruction("011 10001 00 011111111111 10001 10011");
            AssertCode( // subs\tw19, w17,#&7FF
                "0|L--|00100000(4): 2 instructions",
                "1|L--|w19 = w17 - 0x000007FF",
                "2|L--|NZCV = cond(w19)");
        }

        [Test]
        public void A64Rw_sub_Xn_imm()
        {
            Given_Instruction("110 10001 00 011111111111 10001 10011");
            AssertCode( // sub\tx19,x17,#&7FF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x19 = x17 - 0x00000000000007FF");
        }

        [Test]
        public void A64Rw_and_Wn_imm()
        {
            Given_Instruction(0x120F3041);
            AssertCode( // and\tw1,w2,#&3FFE0000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w1 = w2 & 0x3FFE0000");
        }

        [Test]
        public void A64Rw_and_Xn_imm()
        {
            Given_Instruction(0x920F3041);
            AssertCode(     // and\tx1,x2,#&3FFE0000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x1 = x2 & 0x3FFE00003FFE0000");
        }

        [Test]
        public void A64Rw_ands_Xn_imm()
        {
            Given_Instruction("111 100100 0 010101 010101 00100 00111");
            AssertCode(     // ands\tx7,x4,#&FFFFF801
                "0|L--|00100000(4): 4 instructions",
                "1|L--|x7 = x4 & 0xFFFFF801FFFFF801",
                "2|L--|NZ = cond(x7)",
                "3|L--|C = false",
                "4|L--|V = false");
        }

        [Test]
        public void A64Rw_movk_imm()
        {
            Given_Instruction("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
            AssertCode(     // movk\tx7,#&AAA4
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x7 = DPB(x7, 0xAAA4, 0)");
        }

        [Test]
        public void A64Rw_ldp()
        {
            Given_Instruction(0x2D646C2F);
            AssertCode(     // ldp\ts15,27,[x1,-#&E0]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x1 + -224",
                "2|L--|s15 = Mem0[v5:word32]",
                "3|L--|v5 = v5 + 4",
                "4|L--|s27 = Mem0[v5:word32]");
        }

        [Test]
        public void A64Rw_tbz()
        {
            Given_Instruction(0x36686372);
            AssertCode(     // tbz\tw18,#&D,#&100C6C
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if ((w18 & 0x00002000) == 0x00000000) branch 00100C6C");
        }

        [Test]
        public void A64Rw_adrp()
        {
            Given_Instruction(0xF00000E2);
            AssertCode(     // adrp\tx2,#&1F000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = 000000000011F000");
        }

        [Test]
        public void A64Rw_ldr_UnsignedOffset()
        {
            Given_Instruction(0xF947E442);
            AssertCode(     // ldr\tx2,[x2,#&FC8]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = Mem0[x2 + 4040:word64]");
        }

        [Test]
        public void A64Rw_mov_reg64()
        {
            Given_Instruction(0xAA0103F4);
            AssertCode(     // mov\tx20,x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x20 = x1");
        }

        [Test]
        public void A64Rw_adrp_00001()
        {
            Given_Instruction(0xB0000001);
            AssertCode(     // adrp\tx1,#&1000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x1 = 0000000000101000");
        }

        [Test]
        public void A64Rw_mov_reg32()
        {
            Given_Instruction(0x2A0003F5);
            AssertCode(     // mov\tw21,w0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w21 = w0");
        }

        [Test]
        public void A64Rw_movz_imm32()
        {
            Given_Instruction(0x528000C0);
            AssertCode(     // movz\tw0,#6
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = 0x00000006");
        }

        [Test]
        public void A64Rw_ldrsw()
        {
            Given_Instruction(0xB9800033);
            AssertCode(     // ldrsw\tx19,[x1]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[x1:int32]",
                "2|L--|x19 = (word64) v4");
        }

        [Test]
        public void A64Rw_add_reg()
        {
            Given_Instruction(0x8B130280);
            AssertCode(     // add\tx0,x20,x19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = x20 + x19");
        }

        [Test]
        public void A64Rw_add_reg_with_shift()
        {
            Given_Instruction(0x8B130A80);
            AssertCode( // add\tx0,x20,x19,lsl #2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = x20 + (x19 << 2)");
        }

        [Test]
        public void A64Rw_cbz()
        {
            Given_Instruction(0xB4001341);
            AssertCode(     // cbz\tx1,#&100268
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (x1 == 0x0000000000000000) branch 00100268");
        }

        [Test]
        public void A64Rw_lsl()
        {
            Given_Instruction(0xD37DF29C);
            AssertCode(     // ubfm\tx28,x20,#0,#&3D
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x28 = x20 << 3");
        }

        [Test]
        public void A64Rw_ccmp_imm()
        {
            Given_Instruction(0xFA400B84);
            AssertCode(     // ccmp\tx28,#0,#4,EQ
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(NE,Z)",
                "2|L--|NZCV = 0x04",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(x28 - 0x0000000000000000)");
        }

        [Test]
        public void A64Rw_str_UnsignedImmediate()
        {
            Given_Instruction(0xF9000AE0);
            AssertCode(     // str\tx0,[x23,#&10]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x23 + 16:word64] = x0");
        }

        [Test]
        public void A64Rw_ret()
        {
            Given_Instruction(0xD65F03C0);
            AssertCode(     // ret\tx30
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void A64Rw_str_reg()
        {
            Given_Instruction(0xB8356B7F);
            AssertCode(     // str\tw31,[x27,x21]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x27 + x21:word32] = 0x00000000");
        }

        [Test]
        public void A64Rw_nop()
        {
            Given_Instruction(0xD503201F);
            AssertCode(     // nop
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }


        [Test]
        public void A64Rw_str_w32()
        {
            Given_Instruction(0xB9006FA0);
            AssertCode(     // str\tw0,[x29,#&6C]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x29 + 108:word32] = w0");
        }

        [Test]
        public void A64Rw_ldr()
        {
            Given_Instruction(0xF9400000);
            AssertCode(     // ldr\tx0,[x0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = Mem0[x0:word64]");
        }

        [Test]
        public void A64Rw_subs()
        {
            Given_Instruction(0xEB13001F);
            AssertCode(     // subs\tx31,x0,x19
                "0|L--|00100000(4): 2 instructions",
                "1|L--|x31 = x0 - x19",
                "2|L--|NZCV = cond(x31)");
        }

        [Test]
        public void A64Rw_cset()
        {
            Given_Instruction(0x1A9F17E0);
            AssertCode(     // csinc\tw0,w31,w31,NE
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = (word32) Test(EQ,Z)");
        }

        public void A64Rw_cinc()
        {
            Given_Instruction(0x1A88A518);
            AssertCode(     // csinc\tw0,w31,w31,NE
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = (word32) Test(EQ,Z)");
        }


        [Test]
        public void A64Rw_ldrb()
        {
            Given_Instruction(0x39402260);
            AssertCode(     // ldrb\tw0,[x19,#&8]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[x19 + 8:byte]",
                "2|L--|w0 = (word32) v4");
        }

        [Test]
        public void A64Rw_cbnz()
        {
            Given_Instruction(0x35000140);
            AssertCode(     // cbnz\tw0,#&100028
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (w0 != 0x00000000) branch 00100028");
        }

        [Test]
        public void A64Rw_strb()
        {
            Given_Instruction(0x39002260);
            AssertCode(     // strb\tw0,[x19,#&8]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x19 + 8:byte] = (byte) w0");
        }

        [Test]
        public void A64Rw_ldr_w32()
        {
            Given_Instruction(0xB9400001);
            AssertCode(     // ldr\tw1,[x0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w1 = Mem0[x0:word32]");
        }

        [Test]
        public void A64Rw_cbnz_negative_offset()
        {
            Given_Instruction(0x35FFFE73);
            AssertCode(     // cbnz\tw19,#&FFFCC
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (w19 != 0x00000000) branch 000FFFCC");
        }

        [Test]
        public void A64Rw_bne1()
        {
            Given_Instruction(0x54000401);
            AssertCode(     // b.ne\t#&100080
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100080");
        }

        [Test]
        public void A64Rw_ldr_reg_shift()
        {
            Given_Instruction(0xF8737AA3);
            AssertCode(     // ldr\tx3,[x21,x19,lsl,#3]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = Mem0[x21 + x19 * 8:word64]");
        }

        [Test]
        public void A64Rw_blr()
        {
            Given_Instruction(0xD63F0060);
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call x3 (0)");
        }

        [Test]
        public void A64Rw_bne_backward()
        {
            Given_Instruction(0x54FFFF21);
            AssertCode(     // b.ne\t#&FFFE4
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 000FFFE4");
        }

        [Test]
        public void A64Rw_stp_preindex()
        {
            Given_Instruction(0xA9B87BFD);
            AssertCode(     // stp\tx29,x30,[x31,-#&80]!
                "0|L--|00100000(4): 4 instructions",
                "1|L--|sp = sp + -128",
                "2|L--|Mem0[sp:word64] = x29",
                "3|L--|sp = sp + 8",
                "4|L--|Mem0[sp:word64] = x30");
        }

        [Test]
        public void A64Rw_ldp_w64()
        {
            Given_Instruction(0xA9446BB9);
            AssertCode(     // ldp\tx25,x26,[x29,#&40]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x29 + 64",
                "2|L--|x25 = Mem0[v5:word64]",
                "3|L--|v5 = v5 + 8",
                "4|L--|x26 = Mem0[v5:word64]");
        }

        [Test]
        public void A64Rw_ldp_post()
        {
            Given_Instruction(0xA8C17BFD);
            AssertCode(     // ldp\tx29,x30,[x31],#&8
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v5 = sp",
                "2|L--|x29 = Mem0[v5:word64]",
                "3|L--|v5 = v5 + 8",
                "4|L--|x30 = Mem0[v5:word64]",
                "5|L--|sp = sp + 16");
        }

        [Test]
        public void A64Rw_ldr_64_neg_lit()
        {
            Given_Instruction(0x18FFFFE0);
            AssertCode(     // ldr\tw0,#&FFFFC
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w0 = Mem0[0x000FFFFC:word32]");
        }

        [Test]
        public void A64Rw_movn()
        {
            Given_Instruction(0x12800000);
            AssertCode(     // movn\tw0,#0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w0 = 0xFFFFFFFF");
        }

        [Test]
        public void A64Rw_asr()
        {
            Given_Instruction(0x13017E73);
            AssertCode(     // sbfm\tw19,w19,#1,#&1F
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w19 = w19 >> 1");
        }

        [Test]
        public void A64Rw_and_reg()
        {
            Given_Instruction(0x8A140000);
            AssertCode(     // and\tx0,x0,x20
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x0 = x0 & x20");
        }

        [Test]
        public void A64Rw_and_reg_reg()
        {
            Given_Instruction(0x0A000020);
            AssertCode(     // and\tw0,w1,w0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w0 = w1 & w0");
        }

        [Test]
        public void A64Rw_sfbiz()
        {
            Given_Instruction(0x937D7C63);
            AssertCode(     // sbfm\tx3,x3,#&3,#&20
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x3 = __sbfiz(x3, 3)");
        }

        [Test]
        public void A64Rw_mul_64()
        {
            Given_Instruction(0x9B017C14);
            AssertCode(     // mul\tx20,x0,x1
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x20 = x0 * x1");
        }

        [Test]
        public void A64Rw_madd_64()
        {
            Given_Instruction(0x9B013C14);
            AssertCode(     // madd\tx20,x0,x1,x15
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x20 = x15 + x0 * x1");
        }

        [Test]
        public void A64Rw_ands_64_reg()
        {
            Given_Instruction(0xEA010013);
            AssertCode(     // ands\tx19,x0,x1
                 "0|L--|00100000(4): 4 instructions",
                 "1|L--|x19 = x0 & x1",
                 "2|L--|NZ = cond(x19)",
                 "3|L--|C = false",
                 "4|L--|V = false");
        }

        [Test]
        public void A64Rw_test_64_reg()
        {
            Given_Instruction(0xEA01001F);
            AssertCode(     // test\tx31,x0,x1
                 "0|L--|00100000(4): 3 instructions",
                 "1|L--|NZ = cond(x0 & x1)",
                 "2|L--|C = false",
                 "3|L--|V = false");
        }

        [Test]
        public void A64Rw_ldurb()
        {
            Given_Instruction(0x385F9019);
            AssertCode(     // ldurb\tw25,[x0,-#&7]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v4 = Mem0[x0 + -7:byte]",
                 "2|L--|w25 = (word32) v4");
        }

        [Test]
        public void A64Rw_strb_postidx()
        {
            Given_Instruction(0x38018C14);
            AssertCode(     // strb\tw20,[x0,#&18]!
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|x0 = x0 + 24",
                 "2|L--|Mem0[x0:byte] = (byte) w20");
        }

        [Test]
        public void A64Rw_strb_preidx_sp()
        {
            Given_Instruction(0x38018FFF);
            AssertCode(     // strb\tw31,[sp,#&18]!
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|sp = sp + 24",
                 "2|L--|Mem0[sp:byte] = 0x00");
        }

        [Test]
        public void A64Rw_ldrh_32_off0()
        {
            Given_Instruction(0x79400021);
            AssertCode(     // ldrh\tw1,[x1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v4 = Mem0[x1:word16]",
                 "2|L--|w1 = (word32) v4");
        }

        [Test]
        public void A64Rw_add_extension()
        {
            Given_Instruction(0x8B34C2D9);
            AssertCode(     // add\tx25,x22,w20,sxtw #0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x25 = x22 + (int64) ((int32) w20)");
        }

        [Test]
        public void A64Rw_madd_32()
        {
            Given_Instruction(0x1B003C21);
            AssertCode(     // madd\tw1,w1,w0,w15
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w1 = w15 + w1 * w0");
        }

        [Test]
        public void A64Rw_mneg_32()
        {
            Given_Instruction(0x1B00FC21);
            AssertCode(     // mneg\tw1,w1,w0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w1 = -(w1 * w0)");
        }

        [Test]
        public void A64Rw_msub_32()
        {
            Given_Instruction(0x1B00BC21);
            AssertCode(     // msub\tw1,w1,w0,w15
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w1 = w15 - w1 * w0");
        }

        [Test]
        public void A64Rw_strb_post_idx()
        {
            Given_Instruction(0x38001410);
            AssertCode(     // strb\tw16,[x0],#&1
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|Mem0[x0:byte] = (byte) w16",
                 "2|L--|x0 = x0 + 1");
        }

        [Test]
        public void A64Rw_strb_post_idx_zero()
        {
            Given_Instruction(0x3800141F);
            AssertCode(     // strb\tw31,[x0],#&1
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|Mem0[x0:byte] = 0x00",
                 "2|L--|x0 = x0 + 1");
        }

        [Test]
        public void A64Rw_strb_post_sp()
        {
            Given_Instruction(0x381FC7FF);
            AssertCode(     // strb\tw31,[sp],-#&4
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|Mem0[sp:byte] = 0x00",
                 "2|L--|sp = sp + -4");
        }

        [Test]
        public void A64Rw_msub_64()
        {
            Given_Instruction(0x9B038441);
            AssertCode(     // msub\tx1,x2,x3,x1
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x1 = x1 - x2 * x3");
        }

        [Test]
        public void A64Rw_ldur_64_negative_offset()
        {
            Given_Instruction(0xF85F8260);
            AssertCode(     // ldur\tx0,[x19,-#&8]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x0 = Mem0[x19 + -8:word64]");
        }

        [Test]
        public void A64Rw_strb_indexed()
        {
            Given_Instruction(0x3820483F);
            AssertCode(     // strb\tw31,[x1,w0,uxtw]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x1 + (uint64) ((uint32) w0):byte] = 0x00");
        }

        [Test]
        public void A64Rw_str_q0()
        {
            Given_Instruction(0x3D8027A0);
            AssertCode(     // str\tq0,[x29,#&90]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x29 + 144:word128] = q0");
        }

        [Test]
        public void A64Rw_cmp_32_uxtb()
        {
            Given_Instruction(0x6B20031F);
            AssertCode(     // cmp\tw0,w0,uxtb #0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|NZCV = cond(w24 - (uint32) ((byte) w0))");
        }

        [Test]
        public void A64Rw_ldrb_idx()
        {
            Given_Instruction(0x38616857);
            AssertCode(     // ldrb\tw23,[x2,x1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x2 + x1:byte]",
                 "2|L--|w23 = (word32) v5");
        }

        [Test]
        public void A64Rw_strb_index()
        {
            Given_Instruction(0x38216A63);
            AssertCode(     // strb\tw3,[x19,x1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x19 + x1:byte] = (byte) w3");
        }

        [Test]
        public void A64Rw_strb_zero()
        {
            Given_Instruction(0x38216A7F);
            AssertCode(     // strb\tw3,[x19,x1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x19 + x1:byte] = 0x00");
        }

        [Test]
        public void A64Rw_add_ext_reg_sxtw()
        {
            Given_Instruction(0x8B33D2D3);
            AssertCode(     // add\tx19,x22,w19,sxtw #4
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x19 = x22 + (int64) ((int32) w19)");
        }

        [Test]
        public void A64Rw_ldr_64_preidx()
        {
            Given_Instruction(0xF8410E81);
            AssertCode(     // ldr\tx1,[x20,#&10]!
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|x20 = x20 + 16",
                 "2|L--|x1 = Mem0[x20:word64]");
        }

        [Test]
        public void A64Rw_ldrb_post()
        {
            Given_Instruction(0x38401420);
            AssertCode(     // ldrb\tw0,[x1],#&1
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = Mem0[x1:byte]",
                "2|L--|w0 = (word32) v4",
                "3|L--|x1 = x1 + 1");
        }

        [Test]
        public void A64Rw_strb_uxtw()
        {
            Given_Instruction(0x38344B23);
            AssertCode(     // strb\tw3,[x25,w20,uxtw]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x25 + (uint64) ((uint32) w20):byte] = (byte) w3");
        }

        [Test]
        public void A64Rw_sdiv()
        {
            Given_Instruction(0x1AC00F03);
            AssertCode(     // sdiv\tw3,w24,w0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|w3 = w24 / w0");
        }


        [Test]
        public void A64Rw_ldrb_preidx()
        {
            Given_Instruction(0x38401C41);
            AssertCode(     // ldrb\tw1,[x2,#&1]!
                 "0|L--|00100000(4): 3 instructions",
                 "1|L--|x2 = x2 + 1",
                 "2|L--|v4 = Mem0[x2:byte]",
                 "3|L--|w1 = (word32) v4");
        }

        [Test]
        public void A64Rw_ldrb_idx_uxtw()
        {
            Given_Instruction(0x38614873);
            AssertCode(     // ldrb\tw19,[x3,w1,uxtw]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[x3 + (uint64) ((uint32) w1):byte]",
                "2|L--|w19 = (word32) v5");
        }

        [Test]
        public void A64Rw_ldrh_32_idx_lsl()
        {
            Given_Instruction(0x787B7B20);
            AssertCode(     // ldrh\tw0,[x25,x27,lsl #1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x25 + x27 * 2:word16]",
                 "2|L--|w0 = (word32) v5");
        }

        [Test]
        public void A64Rw_ldrh_32_sxtw()
        {
            Given_Instruction(0x7876D800);
            AssertCode(     // ldrh\tw0,[x0,w22,sxtw #1]
                 "0|L--|00100000(4): 2 instructions",
                 "1|L--|v5 = Mem0[x0 + (int64) ((int32) w22):word16]",
                 "2|L--|w0 = (word32) v5");
        }

        public void A64Rw_3873C800()
        {
            Given_Instruction(0x3873C800);
            AssertCode(     // @@@
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|@@@");
        }

        [Test]
        public void A64Rw_movn_imm()
        {
            Given_Instruction(0x9280000A);
            AssertCode(     // movn\tx10,#0
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|x10 = 0xFFFFFFFFFFFFFFFF");
        }

        [Test]
        public void A64Rw_sturb_off()
        {
            Given_Instruction(0x381FF09F);
            AssertCode(     // sturb\tw31,[x4,-#&1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x4 + -1:byte] = 0x00");
        }

        [Test]
        public void A64Rw_adr()
        {
            Given_Instruction(0x10000063);
            AssertCode(     // adr\tx3,#&10000C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = 0010000C");
        }

        [Test]
        public void A64Rw_orn()
        {
            Given_Instruction(0x2A2200F8);
            AssertCode(     // orn\tw24,w7,w2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w24 = w7 | ~w2");
        }

        [Test]
        public void A64Rw_mvn()
        {
            Given_Instruction(0x2A2203F8);
            AssertCode(     // mvn\tw24,w2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w24 = ~w2");
        }

        [Test]
        public void A64Rw_sdiv_64()
        {
            Given_Instruction(0x9AC20C62);
            AssertCode(     // sdiv\tx2,x3,x2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = x3 / x2");
        }

        [Test]
        public void A64Rw_eor_reg_32()
        {
            Given_Instruction(0x4A140074);
            AssertCode(     // eor\tw20,w3,w20
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w20 = w3 ^ w20");
        }

        [Test]
        public void A64Rw_sub_reg_ext_64()
        {
            Given_Instruction(0xCB214F18);
            AssertCode(     // sub\tx24,x24,w1,uxtw #3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x24 = x24 - (uint64) ((word32) w1)");
        }

        [Test]
        public void A64Rw_bic_reg_32()
        {
            Given_Instruction(0x0A350021);
            AssertCode(     // bic\tw1,w1,w21
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w1 = w1 & ~w21");
        }

        [Test]
        public void A64Rw_umulh()
        {
            Given_Instruction(0x9BC57C00);
            AssertCode(     // umulh\tx0,w0,w5
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = SLICE(w0 *u w5, uint64, 64)");
        }

        [Test]
        public void A64Rw_lsrv()
        {
            Given_Instruction(0x1AC22462);
            AssertCode(     // lsrv\tw2,w3,w2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w2 = w3 >>u w2");
        }

        [Test]
        public void A64Rw_smull()
        {
            Given_Instruction(0x9B237C43);
            AssertCode(     // smull\tx3,w2,w3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = (int64) (w2 *s w3)");
        }

        [Test]
        public void A64Rw_smaddl()
        {
            Given_Instruction(0x9B233C43);
            AssertCode(     // smaddl\tx3,w2,w3,x15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x3 = x15 + (int64) (w2 *s w3)");
        }

        [Test]
        public void A64Rw_strh_reg()
        {
            Given_Instruction(0x78206A62);
            AssertCode(     // strh\tw2,[x19,x0]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x19 + x0:word16] = (word16) w2");
        }

        [Test]
        public void A64Rw_stp_r64_pre()
        {
            Given_Instruction(0x6DB73BEF);
            AssertCode(     // stp\td15,d14,[sp,-#&90]!
                "0|L--|00100000(4): 4 instructions",
                "1|L--|sp = sp + -144",
                "2|L--|Mem0[sp:word64] = d15",
                "3|L--|sp = sp + 8",
                "4|L--|Mem0[sp:word64] = d14");
        }

        [Test]
        public void A64Rw_ldr_r64_off()
        {
            Given_Instruction(0xFD45E540);
            AssertCode(     // ldr\td0,[x10,#&BC8]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = Mem0[x10 + 3016:word64]");
        }

        [Test]
        public void A64Rw_str_r64_imm()
        {
            Given_Instruction(0xFD001BE0);
            AssertCode(     // str\td0,[sp,#&30]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[sp + 48:word64] = d0");
        }

        [Test]
        public void A64Rw_scvtf_int()
        {
            Given_Instruction(0x1E220120);
            AssertCode(     // scvtf\ts0,w9
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = (real32) (int32) w9");
        }

        [Test]
        public void A64Rw_mov_vector_to_vector()
        {
            Given_Instruction(0x4EA31C68);
            AssertCode(     // @@@
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q8 = q3");
        }

        [Test]
        public void A64Rw_ldp_w32()
        {
            Given_Instruction(0x296107A2);
            AssertCode(     // ldp\tw2,w1,[x29,-#&F8]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x29 + -248",
                "2|L--|w2 = Mem0[v5:word32]",
                "3|L--|v5 = v5 + 4",
                "4|L--|w1 = Mem0[v5:word32]");
        }

        [Test]
        public void A64Rw_scvtf_r32()
        {
            Given_Instruction(0x5E21D82F);
            AssertCode(     // scvtf\ts15,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s15 = (real32) (int32) s1");
        }

        [Test]
        public void A64Rw_stp_r32()
        {
            Given_Instruction(0x2D010FE2);
            AssertCode(     // stp\ts2,s3,[sp,#&8]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = sp + 8",
                "2|L--|Mem0[v5:word32] = s2",
                "3|L--|v5 = v5 + 4",
                "4|L--|Mem0[v5:word32] = s3");
        }

        [Test]
        public void A64Rw_fcvtms_f32_to_i32()
        {
            Given_Instruction(0x1E300003);
            AssertCode(     // fcvtms\tw3,s0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = s0",
                "2|L--|w3 = (int32) floorf(v4)");
        }

        [Test]
        public void A64Rw_udiv_w32()
        {
            Given_Instruction(0x1ADA0908);
            AssertCode(     // udiv\tw8,w8,w26
            "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = w8 /u w26");
        }

        [Test]
        public void A64Rw_rev16_w32()
        {
            Given_Instruction(0x5AC0056B);
            AssertCode(     // rev16\tw11,w11
            "0|L--|00100000(4): 1 instructions",
                "1|L--|w11 = __rev16(w11)");
        }

        [Test]
        public void A64Rw_add_32_ext()
        {
            Given_Instruction(0x0B20A1EF);
            AssertCode(     // add\tw15,w15,w0,sxth #0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w15 = w15 + (int32) ((int16) w0)");
        }

        [Test]
        public void A64Rw_scvtf_i32_to_f32()
        {
            Given_Instruction(0x1E6202E0);
            AssertCode(     // scvtf\td0,w23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d0 = (real64) (int32) w23");
        }

        [Test]
        public void A64Rw_fmov_f64_to_i64()
        {
            Given_Instruction(0x9E6701B0);
            AssertCode(     // fmov\td16,x13
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d16 = x13");
        }

        [Test]
        public void A64Rw_sxtl()
        {
            Given_Instruction(0x0F10A673);
            AssertCode(     // sxtl\tv19.4h,v19.4h
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = d19",
                "2|L--|q19 = __sxtl_i16(v2)");
        }

        [Test]
        public void A64Rw_fcvtzs_vector_real32()
        {
            Given_Instruction(0x4EA1BAB5);
            AssertCode(     // fcvtzs\tv21.4s,v21.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q21",
                "2|L--|q21 = __trunc_f32(v2)");
        }

        [Test]
        public void A64Rw_fmul_real32()
        {
            Given_Instruction(0x1E210B25);
            AssertCode(     // fmul\ts5,s25,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s5 = s25 * s1");
        }

        [Test]
        public void A64Rw_fcvtzs_i32_from_f32()
        {
            Given_Instruction(0x1E380069);
            AssertCode(     // fcvtzs\tw3,s9
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = s9",
                "2|L--|w3 = (int32) truncf(v4)");
        }

        [Test]
        public void A64Rw_ucvtf_real32_int32()
        {
            Given_Instruction(0x1E230101);
            AssertCode(     // ucvtf\ts1,w8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = (real32) (uint32) w8");
        }

        [Test]
        public void A64Rw_fcsel()
        {
            Given_Instruction(0x1E2B1C00);
            AssertCode(     // fcsel\ts0,s0,s11,NE
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = Test(NE,Z) ? s0 : s11");
        }

        [Test]
        public void A64Rw_fcvtps_f32_to_i32()
        {
            Given_Instruction(0x1E280008);
            AssertCode(     // fcvtps\tw8,s0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = s0",
                "2|L--|w8 = (int32) ceilf(v4)");
        }

        [Test]
        public void A64Rw_fcmp_f32()
        {
            Given_Instruction(0x1E222060);
            AssertCode(     // fcmp\ts3,s2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(s3 - s2)");
        }

        [Test]
        public void A64Rw_fabs_f32()
        {
            Given_Instruction(0x1E20C021);
            AssertCode(     // fabs\ts1,s1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = s1",
                "2|L--|s1 = fabsf(v3)");
        }

        [Test]
        public void A64Rw_fneg_f32()
        {
            Given_Instruction(0x1E214021);
            AssertCode(     // fneg\ts1,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = -s1");
        }

        [Test]
        public void A64Rw_fsqrt()
        {
            Given_Instruction(0x1E21C001);
            AssertCode(     // fsqrt\ts1,s0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = s0",
                "2|L--|s1 = sqrtf(v2)");
        }

        [Test]
        public void A64Rw_fmov_i32_to_f32()
        {
            Given_Instruction(0x1E2703E1);
            AssertCode(     // fmov\ts1,w31
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = 0x00000000");
        }

        [Test]
        public void A64Rw_fcvt_f32_to_f64()
        {
            Given_Instruction(0x1E22C041);
            AssertCode(     // fcvt\td1,s2
                "0|L--|00100000(4): 1 instructions",
                "1|L--|d1 = (real64) (real32) s2");
        }

        [Test]
        public void A64Rw_fcvt_f64_to_f32()
        {
            Given_Instruction(0x1E624000);
            AssertCode(     // fcvt\ts0,d0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = (real32) (real64) d0");
        }

        [Test]
        public void A64Rw_mov_w128()
        {
            Given_Instruction(0x4EA91D22);
            AssertCode(     // mov\tv2.16b,v9.16b
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q2 = q9");
        }

        [Test]
        public void A64Rw_uxtl()
        {
            Given_Instruction(0x2F08A400);
            AssertCode(     // uxtl\tv0.8h,v0.8b
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = d0",
                "2|L--|q0 = __uxtl_u16(v2)");
        }

        [Test]
        public void A64Rw_xtn()
        {
            Given_Instruction(0x0E612A10);
            AssertCode(     // xtn\tv16.4h,v16.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q16",
                "2|L--|d16 = __xtn_i16(v2)");
        }


        [Test]
        public void A64Rw_fmov_f32_to_w32()
        {
            Given_Instruction(0x1E26002B);
            AssertCode(     // fmov\tw11,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w11 = s1");
        }

        [Test]
        public void A64Rw_fmov_vector_immediate()
        {
            Given_Instruction(0x4F03F600);
            AssertCode(     // fmov\tv0.4s,#1.0F
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 1.0F",
                "2|L--|q0 = __fmov_f32(v2)");
        }

        [Test]
        public void A64Rw_dup_element_w32()
        {
            Given_Instruction(0x4E0406E2);
            AssertCode(     // dup\tv2.4s,v23.s[0]
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q23[0]",
                "2|L--|q2 = __dup_i32(v2)");
        }

        [Test]
        public void A64Rw_fadd_vector_f32()
        {
            Given_Instruction(0x4E30D4D0);
            AssertCode(     // fadd\tv16.4s,v6.4s,v16.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q6",
                "2|L--|v3 = q16",
                "3|L--|q16 = __fadd_f32(v2, v3)");
        }

        [Test]
        public void A64Rw_scvtf_vector_i32()
        {
            Given_Instruction(0x4E21DA10);
            AssertCode(     // scvtf\tv16.4s,v16.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = q16",
                "2|L--|q16 = __scvtf_i32(v3)");
        }

        [Test]
        public void A64Rw_mul_vector_i16()
        {
            Given_Instruction(0x4E609C20);
            AssertCode(     // mul\tv0.8h,v1.8h,v0.8h
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q0",
                "3|L--|q0 = __mul_i16(v2, v3)");
        }

        [Test]
        public void A64Rw_addv_i32()
        {
            Given_Instruction(0x4EB1B821);
            AssertCode(     // addv\ts1,v1.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|s1 = __sum_i32(v2)");
        }

        [Test]
        public void A64Rw_mov_vector_element_i16()
        {
            Given_Instruction(0x6E0A5633);
            AssertCode(     // mov\tv19.h[2],v17.h[5]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q19[2] = q17[5]");
        }

        [Test]
        public void A64Rw_add_vector_i32()
        {
            Given_Instruction(0x4EA28482);
            AssertCode(     // add\tv2.4s,v4.4s,v2.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q4",
                "2|L--|v3 = q2",
                "3|L--|q2 = __add_i32(v2, v3)");
        }

        [Test]
        public void A64Rw_fmul_vector_f32()
        {
            Given_Instruction(0x6E30DC90);
            AssertCode(     // fmul\tv16.4s,v4.4s,v16.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q4",
                "2|L--|v3 = q16",
                "3|L--|q16 = __fmul_f32(v2, v3)");
        }

        [Test]
        public void A64Rw_ccmp_w32()
        {
            Given_Instruction(0x7A43B900);
            AssertCode(     // ccmp\tw8,#3,#0,LT
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(GE,NZV)",
                "2|L--|NZCV = 0x00",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(w8 - 0x00000003)");
        }

        [Test]
        public void A64Rw_ucvtf_f32()
        {
            Given_Instruction(0x7E21D821);
            AssertCode(     // ucvtf\ts1,s1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s1 = (real32) (uint32) s1");
        }

        [Test]
        public void A64Rw_clz()
        {
            Given_Instruction(0xDAC01002);
            AssertCode(     // clz\tx2,x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = __clz(x0)");
        }

        [Test]
        public void A64Rw_stp_zero_offset()
        {
            Given_Instruction(0xA9007EBF);
            AssertCode(     // stp\tx31,x31,[x21]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = x21",
                "2|L--|Mem0[v3:word64] = 0x0000000000000000",
                "3|L--|v3 = v3 + 8",
                "4|L--|Mem0[v3:word64] = 0x0000000000000000");
        }

        [Test]
        public void A64Rw_ld2()
        {
            Given_Instruction(0x4C4081C1);	// ld2	{v1.8b,v2.8b},[x14]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ld2(x14, out v1, out v2)");
        }

        [Test]
        public void A64Rw_ld3()
        {
            Given_Instruction(0x0C404565);
            AssertCode(     // ld3\t{v5.4h,v6.4h,v7.4h},[x11]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ld3(x11, out v5, out v6, out v7)");
        }

        [Test]
        public void A64Rw_movi_b()
        {
            Given_Instruction(0x0F00E460);
            AssertCode(     // movi\tv0.8b,#&3030303
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 0x0303030303030303",
                "2|L--|d0 = __movi_i8(v2)");
        }

        [Test]
        public void A64Rw_movi_w16()
        {
            Given_Instruction(0x4F008441);
            AssertCode(     // movi\tv1.8h,#&20002
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 0x0002000200020002",
                "2|L--|q1 = __movi_i16(v2)");
        }

        [Test]
        public void A64Rw_movi_w64_0()
        {
            Given_Instruction(0x6F00E401);
            AssertCode(     // movi\tv1.2d,#0
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = 0x0000000000000000",
                "2|L--|q1 = __movi_i64(v2)");
        }


        [Test]
        public void A64Rw_scvtf_fixed()
        {
            Given_Instruction(0x1E18C008);
            AssertCode(     // scvtf\ts8,w0,#&10
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s8 = __scvtf_fixed(w0, 16)");
        }

        [Test]
        public void A64Rw_umlal_vector()
        {
            Given_Instruction(0x2E208045);
            AssertCode(     // umlal\tv5.4h,v2.8b,v0.8b
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = d2",
                "2|L--|v3 = d0",
                "3|L--|q5 = __umlal_u8(v2, v3, q5)");
        }

 

   

        [Test]
        public void A64Rw_fmov_vector_hiword()
        {
            Given_Instruction(0x9EAF0060);
            AssertCode(     // fmov\tq0.d[1],x3
                "0|L--|00100000(4): 1 instructions",
                "1|L--|q0[1] = x3");
        }

        [Test]
        public void A64Rw_smaxv()
        {
            Given_Instruction(0x4EB0A800);
            AssertCode(     // smaxv\ts0,v0.4s
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q0",
                "2|L--|s0 = __smax_i32(v2)");
        }

        [Test]
        public void A64Rw_smax_vector()
        {
            Given_Instruction(0x4EA16400);
            AssertCode(     // smax\tv0.4s,v0.4s,v1.4s
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q0",
                "2|L--|v3 = q1",
                "3|L--|q0 = __smax_i32(v2, v3)");
        }

        [Test]
        public void A64Rw_umull_vector()
        {
            Given_Instruction(0x2E20C084);
            AssertCode(     // umull\tv4.8h,v4.8b,v0.8b
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = d4",
                "2|L--|v3 = d0",
                "3|L--|q4 = __mull_i8(v2, v3)");
        }

        [Test]
        public void A64Rw_uaddw_i16()
        {
            Given_Instruction(0x6E231082);
            AssertCode(     // uaddw\tv2.8h,v4.8h,v3.16b
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q4",
                "2|L--|v3 = q3",
                "3|L--|q2 = __uaddw_u16(v2, v3)");
        }

        [Test]
        public void A64Rw_add_with_extension()
        {
            Given_Instruction(0x8B3F63E0);	// add	x0,sp,x31,uxtx #0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = sp + 0x0000000000000000");
        }


        [Test]
        public void A64Rw_bfm()
        {
            Given_Instruction(0x33101D28);	// bfm	w8,w9,#&10,#7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = __bfm(w9, 16, 7)");
        }

        [Test]
        public void A64Rw_csinv()
        {
            Given_Instruction(0x5A9F03E8);	// csinv	w8,w31,w31,EQ
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = Test(EQ,Z) ? 0x00000000 : ~0x00000000");
        }

        [Test]
        public void A64Rw_csel()
        {
            Given_Instruction(0x9A8903E2);	// csel	x2,x31,x9,EQ
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x2 = Test(EQ,Z) ? 0x0000000000000000 : x9");
        }

        [Test]
        public void A64Rw_fdiv()
        {
            Given_Instruction(0x1E201820);	// fdiv	s0,s1,s0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = s1 / s0");
        }

        [Test]
        public void A64Rw_ubfm()
        {
            Given_Instruction(0x5302096B);	// ubfm	w11,w11,#2,#2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w11 = __ubfm(w11, 2, 2)");
        }

        [Test]
        public void A64Rw_fsub()
        {
            Given_Instruction(0x1E203920);	// fsub	s0,s9,s0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = s9 - s0");
        }

        [Test]
        public void A64Rw_sxtb()
        {
            Given_Instruction(0x13001F6A);	// sxtb	w10,w27
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w10 = (int32) (int8) w27");
        }

        [Test]
        public void A64Rw_fmax()
        {
            Given_Instruction(0x1E2A4800);	// fmax	s0,s0,s10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = fmaxf(s0, s10)");
        }


        [Test]
        public void A64Rw_add()
        {
            Given_Instruction(0x8B8C6D6D);	// add	x13,x11,x12,asr #&1B
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x13 = x11 + (x12 >> 27)");
        }

     

        [Test]
        public void A64Rw_csneg()
        {
            Given_Instruction(0x5A895528);	// csneg	w8,w9,w9,PL
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w8 = Test(GE,N) ? w9 : -w9");
        }


        [Test]
        public void A64Rw_fmin()
        {
            Given_Instruction(0x1E285800);	// fmin	s0,s0,s8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = fminf(s0, s8)");
        }

        [Test]
        public void A64Rw_rorv()
        {
            Given_Instruction(0x1AC92D09);	// rorv	w9,w8,w9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w9 = __ror(w8, w9)");
        }

        [Test]
        public void A64Rw_cmp()
        {
            Given_Instruction(0x6B98069F);	// cmp	w20,w24,asr #1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|NZCV = cond(w20 - (w24 >> 1))");
        }

        [Test]
        public void A64Rw_ldpsw()
        {
            Given_Instruction(0x69404222);	// ldpsw	x2,x16,[x17]
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x17",
                "2|L--|x2 = (int64) Mem0[v5:int32]",
                "3|L--|v5 = v5 + 4",
                "4|L--|x16 = (int64) Mem0[v5:int32]");
        }

        [Test]
        public void A64Rw_fnmul()
        {
            Given_Instruction(0x1E227800);	// fnmul	s0,s0,s2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s0 = -(s0 * s2)");
        }


        [Test]
        public void A64Rw_cmeq()
        {
            Given_Instruction(0x6E208C21);	// cmeq	v1.16b,v1.16b,v0.16b
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v2 = q1",
                "2|L--|v3 = q0",
                "3|L--|q1 = __cmeq(v2, v3)");
        }

        [Test]
        public void A64Rw_st1()
        {
            Given_Instruction(0x0D0041B5);	// st1	{v21.h}[0],[x13]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x13:word16] = v21[0]");
        }

        [Test]
        public void A64Rw_not()
        {
            Given_Instruction(0x6E205821);	// not	v1.16b,v1.16b
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q1",
                "2|L--|q1 = __not_i8(v2)");
        }


        [Test]
        public void A64Rw_uxtb()
        {
            Given_Instruction(0x53001E63);	// uxtb	w3,w19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w3 = (uint32) (uint8) w19");
        }

        [Test]
        public void A64Rw_uxth()
        {
            Given_Instruction(0x53003C00);	// uxth	w0,w0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w0 = (uint32) (uint16) w0");
        }

        [Test]
        public void A64Rw_csinc()
        {
            Given_Instruction(0x9A800660);	// csinc	x0,x19,x0,EQ
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = Test(EQ,Z) ? x19 : x0 + 0x0000000000000001");
        }

        [Test]
        public void A64Rw_fmadd_f32()
        {
            Given_Instruction(0x1F0000B9);
            AssertCode(     // fmadd\ts25,s5,s0,s0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s25 = __fmaddf(s5, s0, s0)");
        }


        [Test]
        public void A64Rw_fmsub_f32()
        {
            Given_Instruction(0x1F00A08B);
            AssertCode(     // fmsub\ts11,s16,s0,s8
                "0|L--|00100000(4): 1 instructions",
                "1|L--|s11 = __fmsubf(s4, s0, s8)");
        }


        [Test]
        public void A64Rw_fmadd_f16()
        {
            Given_Instruction(0x1FD61F00);
            AssertCode(     // fmadd\th0,h24,h22,h7
                "0|L--|00100000(4): 1 instructions",
                "1|L--|h0 = __fmaddf(h24, h22, h7)");
        }

        [Test]
        public void A64Rw_prfm()
        {
            Given_Instruction(0xD8545280);
            AssertCode(     // prfm\t#0,#&1A8A50"
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__prfm(0x00, 0x001A8A50)");
        }


        [Test]
        public void A64Rw_ld1r_i8()
        {
            Given_Instruction(0x4D40C220);
            AssertCode(     // ld1r\t{v0.16b},[x17]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ld1r(x17, out v0)");
        }

        [Test]
        public void A64Rw_shrn_i8()
        {
            Given_Instruction(0x0F0E8463);
            AssertCode(     // shrn\tv3.8b,v3.8h,#2
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v2 = q3",
                "2|L--|d3 = __shrn_i16(v2, 2)");
        }

        [Test]
        public void A64Rw_ccmp_reg()
        {
            Given_Instruction(0x7A42D020);
            AssertCode(     // ccmp\tw1,w2,#0,LE
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(GT,NZV)",
                "2|L--|NZCV = 0x00",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(w1 - w2)");
        }

        [Test]
        public void A64Rw_str_sxtx()
        {
            Given_Instruction(0x3CBBEBC8);
            AssertCode( // str	q8, [x30,x27,sxtx]
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[x30 + x27:word128] = q8");
        }

        [Test]
        public void A64Rw_svc()
        {
            Given_Instruction(0xD41B7B61);	// svc	#&DBDB
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__supervisor_call(0xDBDB)");
        }

        [Test]
        public void A64Rw_mrs()
        {
            Given_Instruction(0xD53B0020);	// mrs	x0,CTR_EL0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|x0 = __mrs(CTR_EL0)");
        }

        [Test]
        public void A64Rw_dsb()
        {
            Given_Instruction(0xD5033F9F);	// dsb	#&F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dsb_sy()");
        }

        [Test]
        public void A64Rw_isb()
        {
            Given_Instruction(0xD5033FDF);	// isb	#&F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__isb_sy()");
        }

        [Test]
        public void A64Rw_smc()
        {
            Given_Instruction(0xD4000003);	// smc	#0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__secure_monitor_call(0x0000)");
        }

        [Test]
        public void A64Rw_msr()
        {
            Given_Instruction(0xD50343DF);	// msr	pstate,#3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__msr(pstate, 0x03)");
        }

        [Test]
        public void A64Rw_ccmn()
        {
            Given_Instruction(0x3A4D09C0);	// ccmn	w14,#&D,#0,EQ
            AssertCode(
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v3 = Test(NE,Z)",
                "2|L--|NZCV = 0x00",
                "3|T--|if (v3) branch 00100004",
                "4|L--|NZCV = cond(w14 + 0x0000000D)");
        }
    }
}