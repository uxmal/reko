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
        private MemoryArea mem;

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return baseAddress; }
        }

        protected override IEnumerable<RtlInstructionCluster> GetInstructionStream(IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(new LeImageReader(mem, 0), new Arm64State(arch), binder, host);
        }

        private void BuildTest(params string[] bitStrings)
        {
            var bytes = bitStrings.Select(bits => base.ParseBitPattern(bits))
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            mem = new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }



        private void BuildTest(params uint[] words)
        {
            var bytes = words
                .SelectMany(u => new byte[] { (byte)u, (byte)(u >> 8), (byte)(u >> 16), (byte)(u >> 24) })
                .ToArray();
            mem = new MemoryArea(Address.Ptr32(0x00100000), bytes);
        }


        private void Given_Instruction(uint wInstr)
        {
            BuildTest(wInstr);
        }


        [Test]
        public void A64Rw_b_label()
        {
            BuildTest("00010111 11111111 11111111 00000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000FFC00");
        }

        [Test]
        public void A64Rw_bl_label()
        {
            BuildTest("10010111 11111111 11111111 00000000");
            AssertCode(     // bl\t#&FFC00
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 000FFC00 (0)");
        }

        [Test]
        public void A64Rw_br_Xn()
        {
            BuildTest("11010110 00011111 00000011 11000000");
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto x30");
        }

        [Test]
        public void A64Rw_add_Wn_imm()
        {
            BuildTest("000 10001 01 011111111111 10001 10011");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|w19 = w17 + (0x000007FF << 12)");
        }

        [Test]
        public void A64Rw_adds_Xn_imm()
        {
            BuildTest("101 10001 00 011111111111 10001 10011");
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|x19 = x17 + 0x00000000000007FF",
                "2|L--|NZCV = cond(x19)");
        }

        [Test]
        public void A64Rw_subs_Wn_imm()
        {
            BuildTest("011 10001 00 011111111111 10001 10011");
            AssertCode( // subs\tw19, w17,#&7FF
                "0|L--|00100000(4): 2 instructions",
                "1|L--|w19 = w17 - 0x000007FF",
                "2|L--|NZCV = cond(w19)");
        }

        [Test]
        public void A64Rw_sub_Xn_imm()
        {
            BuildTest("110 10001 00 011111111111 10001 10011");
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
            BuildTest("111 100100 0 010101 010101 00100 00111");
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
            BuildTest("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
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
        public void A64Rw_ubfm()
        {
            Given_Instruction(0xD37DF29C);
            AssertCode(     // ubfm\tx28,x20,#0,#&3D
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
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
        public void A64Rw_csinc()
        {
            Given_Instruction(0x1A9F17E0);
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
            AssertCode(     // stp\tx29,x30,[x31,-#&40]!
                "0|L--|00100000(4): 4 instructions",
                "1|L--|x31 = x31 + -64",
                "2|L--|Mem0[x31:word64] = x29",
                "3|L--|x31 = x31 + 8",
                "4|L--|Mem0[x31:word64] = x30");
        }

        [Test]
        public void A64Rw_stp_w64()
        {
            Given_Instruction(0xA90153F3);
            AssertCode(     // stp\tx19,x20,[x31,#&8]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x31 + 8",
                "2|L--|Mem0[v5:word64] = x19",
                "3|L--|v5 = v5 + 8",
                "4|L--|Mem0[v5:word64] = x20");
        }

        [Test]
        public void A64Rw_ldp_w64()
        {
            Given_Instruction(0xA9446BB9);
            AssertCode(     // ldp\tx25,x26,[x29,#&20]
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = x29 + 32",
                "2|L--|x25 = Mem0[v5:word64]",
                "3|L--|v5 = v5 + 8",
                "4|L--|x26 = Mem0[v5:word64]");
        }

        [Test]
        public void A64Rw_ldp_post()
        {
            Given_Instruction(0xA8C17BFD);
            AssertCode(     // ldp\tx29,x30,[x31],#&8
                "0|L--|00100000(4): 4 instructions",
                "1|L--|x29 = Mem0[x31:word64]",
                "2|L--|x31 = x31 + 8",
                "3|L--|x30 = Mem0[x31:word64]",
                "4|L--|x31 = x31 + 8");
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
                 "0|L--|00100000(4): 4 instructions",
                 "1|L--|",
                 "2|L--|",
                 "3|L--|",
                 "4|L--|@@@");
        }

        [Test]
        public void A64Rw_sbfm()
        {
            Given_Instruction(0x13017E73);
            AssertCode(     // sbfm\tw19,w19,#1,#&1F
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|@@@");
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
        public void A64Rw_sbfm_2()
        {
            Given_Instruction(0x937D7C63);
            AssertCode(     // sbfm\tx3,x3,#&3,#&20
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|@@@");
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
        public void A64Rw_sbfm_0_1F()
        {
            Given_Instruction(0x93407C18);
            AssertCode(     // sbfm\tx24,x0,#0,#&1F@@
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|@@@");
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
                 "1|L--|x10 = 0xFFFFFFFFFFFF");
        }

        [Test]
        public void A64Rw_sturb_off()
        {
            Given_Instruction(0x381FF09F);
            AssertCode(     // sturb\tw31,[x4,-#&1]
                 "0|L--|00100000(4): 1 instructions",
                 "1|L--|Mem0[x4 + -1:byte] = 0x00");
        }
    }
}