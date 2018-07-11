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
    public class A64RewriterTests
    {
        public class Arm64DisassemblerTests : RewriterTestBase
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
                AssertCode("b\t#&FFC00");
            }

            [Test]
            public void A64Rw_bl_label()
            {
                BuildTest("10010111 11111111 11111111 00000000");
                AssertCode("bl\t#&FFC00");
            }

            [Test]
            public void A64Rw_br_Xn()
            {
                BuildTest("11010110 00011111 00000011 11000000");
                AssertCode("br\tx30");
            }

            [Test]
            public void A64Rw_add_Wn_imm()
            {
                BuildTest("000 10001 01 011111111111 10001 10011");
                AssertCode("add\tw19,w17,#&7FF,lsl #&C");
            }

            [Test]
            public void A64Rw_adds_Xn_imm()
            {
                BuildTest("101 10001 00 011111111111 10001 10011");
                AssertCode("adds\tx19,x17,#&7FF");
            }

            [Test]
            public void A64Rw_subs_Wn_imm()
            {
                BuildTest("011 10001 00 011111111111 10001 10011");
                AssertCode("subs\tw19,w17,#&7FF");
            }

            [Test]
            public void A64Rw_sub_Xn_imm()
            {
                BuildTest("110 10001 00 011111111111 10001 10011");
                AssertCode("sub\tx19,x17,#&7FF");
            }

            [Test]
            public void A64Rw_and_Wn_imm()
            {
                Given_Instruction(0x120F3041);
                AssertCode("and\tw1,w2,#&3FFE0000");
            }

            [Test]
            public void A64Rw_and_Xn_imm()
            {
                Given_Instruction(0x920F3041);
                AssertCode("and\tx1,x2,#&3FFE0000");
            }

            [Test]
            public void A64Rw_ands_Xn_imm()
            {
                BuildTest("111 100100 0 010101 010101 00100 00111");
                AssertCode("ands\tx7,x4,#&FFFFF801");
            }

            [Test]
            public void A64Rw_movk_imm()
            {
                BuildTest("111 10010 100 1010 1010 1010 0100 00111"); // 87 54 95 F2");
                AssertCode("movk\tx7,#&AAA4");
            }

            [Test]
            public void A64Rw_ldp()
            {
                Given_Instruction(0x2D646C2F);
                AssertCode("ldp\ts47,s59,[x1,-#&E0]");
            }

            [Test]
            public void A64Rw_tbz()
            {
                Given_Instruction(0x36686372);
                AssertCode("tbz\tw18,#&D,#&100C6C");
            }

            [Test]
            public void A64Rw_adrp()
            {
                Given_Instruction(0xF00000E2);
                AssertCode("adrp\tx2,#&1F000");
            }

            [Test]
            public void A64Rw_ldr_UnsignedOffset()
            {
                Given_Instruction(0xF947E442);
                AssertCode("ldr\tx2,[x2,#&FC8]");
            }

            [Test]
            public void A64Rw_mov_reg64()
            {
                Given_Instruction(0xAA0103F4);
                AssertCode("mov\tx20,x1");
            }

            [Test]
            public void A64Rw_adrp_00001()
            {
                Given_Instruction(0xB0000001);
                AssertCode("adrp\tx1,#&1000");
            }

            [Test]
            public void A64Rw_mov_reg32()
            {
                Given_Instruction(0x2A0003F5);
                AssertCode("mov\tw21,w0");
            }

            [Test]
            public void A64Rw_movz_imm32()
            {
                Given_Instruction(0x528000C0);
                AssertCode("movz\tw0,#6");
            }

            [Test]
            public void A64Rw_ldrsw()
            {
                Given_Instruction(0xB9800033);
                AssertCode("ldrsw\tx19,[x1]");
            }

            [Test]
            public void A64Rw_add_reg()
            {
                Given_Instruction(0x8B130280);
                AssertCode("add\tx0,x20,x19");
            }

            [Test]
            public void A64Rw_add_reg_with_shift()
            {
                Given_Instruction(0x8B130A80);
                AssertCode("add\tx0,x20,x19,lsl #2");
            }

            [Test]
            public void A64Rw_cbz()
            {
                Given_Instruction(0xB4001341);
                AssertCode("cbz\tx1,#&100268");
            }

            [Test]
            public void A64Rw_ubfm()
            {
                Given_Instruction(0xD37DF29C);
                AssertCode("ubfm\tx28,x20,#0,#&3D");
            }

            [Test]
            public void A64Rw_ccmp_imm()
            {
                Given_Instruction(0xFA400B84);
                AssertCode("ccmp\tx28,#0,#4,EQ");
            }

            [Test]
            public void A64Rw_str_UnsignedImmediate()
            {
                Given_Instruction(0xF9000AE0);
                AssertCode("str\tx0,[x23,#&10]");
            }

            [Test]
            public void A64Rw_ret()
            {
                Given_Instruction(0xD65F03C0);
                AssertCode("ret\tx30");
            }

            [Test]
            public void A64Rw_str_reg()
            {
                Given_Instruction(0xB8356B7F);
                AssertCode("str\tw31,[x27,x21]");
            }

            [Test]
            public void A64Rw_nop()
            {
                Given_Instruction(0xD503201F);
                AssertCode("nop");
            }


            [Test]
            public void A64Rw_str_w32()
            {
                Given_Instruction(0xB9006FA0);
                AssertCode("str\tw0,[x29,#&6C]");
            }

            [Test]
            public void A64Rw_ldr()
            {
                Given_Instruction(0xF9400000);
                AssertCode("ldr\tx0,[x0]");
            }

            [Test]
            public void A64Rw_subs()
            {
                Given_Instruction(0xEB13001F);
                AssertCode("subs\tx31,x0,x19");
            }

            [Test]
            public void A64Rw_csinc()
            {
                Given_Instruction(0x1A9F17E0);
                AssertCode("csinc\tw0,w31,w31,NE");
            }

            [Test]
            public void A64Rw_ldrb()
            {
                Given_Instruction(0x39402260);
                AssertCode("ldrb\tw0,[x19,#&8]");
            }

            [Test]
            public void A64Rw_cbnz()
            {
                Given_Instruction(0x35000140);
                AssertCode("cbnz\tw0,#&100028");
            }

            [Test]
            public void A64Rw_strb()
            {
                Given_Instruction(0x39002260);
                AssertCode("strb\tw0,[x19,#&8]");
            }

            [Test]
            public void A64Rw_ldr_w32()
            {
                Given_Instruction(0xB9400001);
                AssertCode("ldr\tw1,[x0]");
            }

            [Test]
            public void A64Rw_cbnz_negative_offset()
            {
                Given_Instruction(0x35FFFE73);
                AssertCode("cbnz\tw19,#&FFFCC");
            }

            [Test]
            public void A64Rw_bne1()
            {
                Given_Instruction(0x54000401);
                AssertCode("b.ne\t#&100080");
            }

            [Test]
            public void A64Rw_ldr_reg_shift()
            {
                Given_Instruction(0xF8737AA3);
                AssertCode("ldr\tx3,[x21,x19,lsl,#3]");
            }

            [Test]
            public void A64Rw_blr()
            {
                Given_Instruction(0xD63F0060);
                AssertCode("blr\tx3");
            }

            [Test]
            public void A64Rw_bne_backward()
            {
                Given_Instruction(0x54FFFF21);
                AssertCode("b.ne\t#&FFFE4");
            }

            [Test]
            public void A64Rw_stp_preindex()
            {
                Given_Instruction(0xA9B87BFD);
                AssertCode("stp\tx29,x30,[x31,-#&40]!");
            }

            [Test]
            public void A64Rw_stp_w64()
            {
                Given_Instruction(0xA90153F3);
                AssertCode("stp\tx19,x20,[x31,#&8]");
            }

            [Test]
            public void A64Rw_ldp_w64()
            {
                Given_Instruction(0xA9446BB9);
                AssertCode("ldp\tx25,x26,[x29,#&20]");
            }

            [Test]
            public void A64Rw_ldp_post()
            {
                Given_Instruction(0xA8C17BFD);
                AssertCode("ldp\tx29,x30,[x31],#&8");
            }
        }
    }
}
