#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class Mips16eRewriterTests : RewriterTestBase
    {
        private MipsProcessorArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture(
                CreateServiceContainer(),
                "mips-be-32",
                new Dictionary<string, object> { { "decoder", "mips16e" } });
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        private void AssertCode(string sHex, params string[] sExp)
        {
            Given_HexString(sHex);
            AssertCode(sExp);
        }

        [Test]
        public void Mips16eRw_addi_sp()
        {
            AssertCode("63CC",        // addi\tsp,-000001A0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|sp = sp + -416<i32>");
        }

        [Test]
        public void Mips16eRw_addiu()
        {
            AssertCode("4166",        // addiu\tr17,r3,00000006
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = r3 + 6<32>");
        }

        [Test]
        public void Mips16eRw_addiu_sp()
        {
            AssertCode("01D1",        // addiu\tr17,sp,00000344
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = sp + 0x344<32>");
        }

        [Test]
        public void Mips16eRw_addiu8()
        {
            AssertCode("4E7E",        // addiu\tr6,0000007E
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = r6 + 0x7E<32>");
        }

        [Test]
        public void Mips16eRw_addu()
        {
            AssertCode("E295",        // addu\tr5,r2,r4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = r2 + r4");
        }

        [Test]
        public void Mips16eRw_b()
        {
            AssertCode("1127",        // b\t00100250
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 00100250");
        }

        [Test]
        public void Mips16eRw_beqz()
        {
            AssertCode("236B",        // beqz\tr3,001000D8
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r3 == 0<32>) branch 001000D8");
        }

        [Test]
        public void Mips16eRw_bnez()
        {
            AssertCode("2DEA",        // bnez\tr5,000FFFD6
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r5 != 0<32>) branch 000FFFD6");
        }

        [Test]
        public void Mips16eRw_cmpi()
        {
            AssertCode("764B",        // cmpi\tr6,0000004B
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r24 = r6 ^ 0x4B<32>");
        }

        [Test]
        public void Mips16eRw_jal_0()
        {
            AssertCode("18000000",        // jal\t00000000
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 00000000 (0)");
        }

        [Test]
        public void Mips16eRw_jal_1()
        {
            AssertCode("1BFFFFFF",        // jal\t0FFFFFFC
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 0FFFFFFC (0)");
        }

        [Test]
        [Ignore("need a way to switch back/forth between MIPS16e and regular MIPS")]
        public void Mips16eRw_jalx_0()
        {
            AssertCode("1C000000",        // jalx\t00000000
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void Mips16eDis_la_ext()
        {
            AssertCode("F010 0811",     // la\tr16,000F8011
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = 000F8011");
        }

        [Test]
        public void Mips16eRw_lb()
        {
            AssertCode("86EE",        // lb\tr7,000E(r7)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = CONVERT(Mem0[r7 + 14<i32>:int8], int8, word32)");
        }

        [Test]
        public void Mips16eRw_lbu()
        {
            AssertCode("A1BE",        // lbu\tr5,001E(r5)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = CONVERT(Mem0[r5 + 30<i32>:byte], byte, word32)");
        }

        [Test]
        public void Mips16eRw_lh()
        {
            AssertCode("8888",        // lh\tr4,0010(r4)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = CONVERT(Mem0[r4 + 16<i32>:int16], int16, word32)");
        }

        [Test]
        public void Mips16eRw_lhu()
        {
            AssertCode("AE47",        // lhu\tr2,000E(r2)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = CONVERT(Mem0[r2 + 14<i32>:uint16], uint16, word32)");
        }

        [Test]
        public void Mips16eRw_lwpc()
        {
            AssertCode("B4D9",        // lw\tr4,0364(pc)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = Mem0[0x00100364<p32>:word32]");
        }

        [Test]
        public void Mips16eRw_lwpc_2()
        {
            AssertCode("B17D",        // lw\tr17,01F4(pc)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = Mem0[0x001001F4<p32>:word32]");
        }

        [Test]
        public void Mips16eRw_lwsp()
        {
            AssertCode("94FC",        // lw\tr4,03F0(sp)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = Mem0[sp + 1008<i32>:word32]");
        }

        [Test]
        public void Mips16eRw_li()
        {
            AssertCode("6CB3",        // li\tr4,000000B3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = 0xB3<32>");
        }

        [Test]
        public void Mips16eRw_mflo()
        {
            AssertCode("E992",        // mflo\tr17
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = lo");
        }

        [Test]
        public void Mips16eRw_move_from_wide()
        {
            AssertCode("677A",        // move\tr3,r26
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r26");
        }

        [Test]
        public void Mips16eRw_move_to_wide()
        {
            AssertCode("657A",        // move\tr27,r2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r27 = r2");
        }

        [Test]
        public void Mips16eRw_neg()
        {
            AssertCode("ECEB",        // neg\tr4,r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = -r7");
        }

        [Test]
        public void Mips16eRw_save()
        {
            AssertCode("64F0",        // save\tra,r17,r16,+00000080
                "0|L--|00100000(2): 4 instructions",
                "1|L--|Mem0[sp - 4<i32>:word32] = ra",
                "2|L--|Mem0[sp - 8<i32>:word32] = r17",
                "3|L--|Mem0[sp - 12<i32>:word32] = r16",
                "4|L--|sp = sp - 12<i32>");
        }

        [Test]
        public void Mips16eRw_sb()
        {
            AssertCode("C275",        // sb\tr3,0015(r3)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r3 + 21<i32>:byte] = SLICE(r3, byte, 0)");
        }

        [Test]
        public void Mips16eRw_sh()
        {
            AssertCode("C84D",        // sh\tr2,001A(r2)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r2 + 26<i32>:word16] = SLICE(r2, word16, 0)");
        }

        [Test]
        public void Mips16eRw_sll()
        {
            AssertCode("31E4",        // sll\tr17,r7,+00000001
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = r7 << 1<i32>");
        }

        [Test]
        public void Mips16eRw_slti()
        {
            AssertCode("50B8",        // slti\tr16,000000B8
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r24 = r16 < 0xB8<32>");
        }

        [Test]
        public void Mips16eRw_sltiu()
        {
            AssertCode("5E0A",        // sltiu\tr6,0000000A
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r24 = r6 <u 0xA<32>");
        }

        [Test]
        public void Mips16eRw_sra()
        {
            AssertCode("341F",        // sra\tr4,r16,+00000007
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = r16 >> 7<i32>");
        }

        [Test]
        public void Mips16eRw_srav()
        {
            AssertCode("EB87",        // srav\tr4,r3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = r4 >> r3");
        }

        [Test]
        public void Mips16eRw_srl()
        {
            AssertCode("33F2",        // srl\tr3,r7,+00000004
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r7 >>u 4<i32>");
        }

        [Test]
        public void Mips16eRw_subu()
        {
            AssertCode("E1BF",        // subu\tr7,r17,r5
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = r17 - r5");
        }

        [Test]
        public void Mips16eRw_swrasp()
        {
            AssertCode("62F3",        // sw\tra,03CC(sp)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[sp + 972<i32>:ptr32] = ra");
        }

        [Test]
        public void Mips16eRw_xor()
        {
            AssertCode("EA8E",        // xor\tr2,r4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = r2 ^ r4");
        }
    }
}
