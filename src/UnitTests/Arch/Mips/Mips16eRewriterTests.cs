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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
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
            this.arch = new MipsBe32Architecture("mips-be-32");
            this.addr = Address.Ptr32(0x00100000);
            this.arch.LoadUserOptions(new Dictionary<string, object> { { "decoder", "mips16e" } });
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                arch.Endianness.CreateImageReader(mem, mem.BaseAddress),
                arch.CreateProcessorState(),
                binder,
                host);
        }

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
                "1|L--|sp = sp + -416");
        }

        [Test]
        public void Mips16eRw_addiu()
        {
            AssertCode("4166",        // addiu\tr17,r3,00000006
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = r3 + 0x00000006");
        }

        [Test]
        public void Mips16eRw_addiu_sp()
        {
            AssertCode("01D1",        // addiu\tr17,sp,00000344
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = sp + 0x00000344");
        }

        [Test]
        public void Mips16eRw_addiu8()
        {
            AssertCode("4E7E",        // addiu\tr6,0000007E
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = r6 + 0x0000007E");
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
                "1|T--|if (r3 == 0x00000000) branch 001000D8");
        }

        [Test]
        public void Mips16eRw_bnez()
        {
            AssertCode("2DEA",        // bnez\tr5,000FFFD6
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r5 != 0x00000000) branch 000FFFD6");
        }

        [Test]
        public void Mips16eRw_cmpi()
        {
            AssertCode("764B",        // cmpi\tr6,0000004B
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r24 = r6 ^ 0x0000004B");
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
        public void Mips16eRw_lb()
        {
            AssertCode("86EE",        // lb\tr7,000E(r7)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = (word32) Mem0[r7 + 14:int8]");
        }

        [Test]
        public void Mips16eRw_lbu()
        {
            AssertCode("A1BE",        // lbu\tr5,001E(r5)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = (word32) Mem0[r5 + 30:byte]");
        }

        [Test]
        public void Mips16eRw_lh()
        {
            AssertCode("8888",        // lh\tr4,0010(r4)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = (word32) Mem0[r4 + 16:int16]");
        }

        [Test]
        public void Mips16eRw_lhu()
        {
            AssertCode("AE47",        // lhu\tr2,000E(r2)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r2 = (word32) Mem0[r2 + 14:uint16]");
        }

        [Test]
        public void Mips16eRw_lwpc()
        {
            AssertCode("B4D9",        // lw\tr4,0364(pc)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = Mem0[0x00100364:word32]");
        }

        [Test]
        public void Mips16eRw_lwpc_2()
        {
            AssertCode("B17D",        // lw\tr17,01F4(pc)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = Mem0[0x001001F4:word32]");
        }

        [Test]
        public void Mips16eRw_lwsp()
        {
            AssertCode("94FC",        // lw\tr4,03F0(sp)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = Mem0[sp + 1008:word32]");
        }

        [Test]
        public void Mips16eRw_li()
        {
            AssertCode("6CB3",        // li\tr4,000000B3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = 0x000000B3");
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
                "1|L--|Mem0[sp - 4:word32] = ra",
                "2|L--|Mem0[sp - 8:word32] = r17",
                "3|L--|Mem0[sp - 12:word32] = r16",
                "4|L--|sp = sp - 12");
        }

        [Test]
        public void Mips16eRw_sb()
        {
            AssertCode("C275",        // sb\tr3,0015(r3)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r3 + 21:byte] = (byte) r3");
        }

        [Test]
        public void Mips16eRw_sh()
        {
            AssertCode("C84D",        // sh\tr2,001A(r2)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r2 + 26:word16] = (word16) r2");
        }

        [Test]
        public void Mips16eRw_sll()
        {
            AssertCode("31E4",        // sll\tr17,r7,+00000001
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r17 = r7 << 1");
        }

        [Test]
        public void Mips16eRw_slti()
        {
            AssertCode("50B8",        // slti\tr16,000000B8
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r24 = r16 < 0x000000B8");
        }

        [Test]
        public void Mips16eRw_sltiu()
        {
            AssertCode("5E0A",        // sltiu\tr6,0000000A
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r24 = r6 <u 0x0000000A");
        }

        [Test]
        public void Mips16eRw_sra()
        {
            AssertCode("341F",        // sra\tr4,r16,+00000007
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r4 = r16 >> 7");
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
                "1|L--|r3 = r7 >>u 4");
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
                "1|L--|Mem0[sp + 972:ptr32] = ra");
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
