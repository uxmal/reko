#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Arch.Padauk;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.UnitTests.Arch.Padauk.PDK15
{
    [TestFixture]
    public class PadaukRewriter_PDK15Tests : RewriterTestBase
    {
        private readonly PadaukArchitecture arch;
        private readonly Address addrLoad;

        public PadaukRewriter_PDK15Tests()
        {
            this.arch = new PadaukArchitecture(
                CreateServiceContainer(),
                "padauk",
                new()
                {
                    { ProcessorOption.InstructionSet, "15" }
                });
            this.addrLoad = Address.Ptr16(0x100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;


        [Test]
        public void Pdk15Rw_add_a_imm()
        {
            Given_HexString("FF50");
            AssertCode(     // add	a,+00FF
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a + 0xFF<8>",
                "2|L--|ZCAV = cond(a)");
        }

        [Test]
        public void Pdk15Rw_add_a_mem()
        {
            Given_HexString("4E18");
            AssertCode(     // add	a,[0x4E]
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a + Mem0[0x004E<p16>:byte]",
                "2|L--|ZCAV = cond(a)");
        }

        [Test]
        public void Pdk15Rw_addc()
        {
            Given_HexString("6000");
            AssertCode(     // addc	a
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a + C",
                "2|L--|ZCAV = cond(a)");
        }

        [Test]
        public void Pdk15Rw_and_a_imm()
        {
            Given_HexString("FE54");
            AssertCode(     // and	a,0xFE
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a & 0xFE<8>",
                "2|L--|Z = cond(a)");
        }

        [Test]
        public void Pdk15Rw_call()
        {
            Given_HexString("4F70");
            AssertCode(     // call	004F
                "0|T--|0100(1): 1 instructions",
                "1|T--|call 004F (2)");
        }

        [Test]
        public void Pdk15Rw_ceqsn()
        {
            Given_HexString("002E");
            AssertCode(     // ceqsn	a,[0x0]
                "0|T--|0100(1): 1 instructions",
                "1|T--|if (a == Mem0[null:byte]) branch 0102");
        }

        [Test]
        public void Pdk15rw_clear()
        {
            Given_HexString("0126");
            AssertCode( // clear [0x1]
                "0|L--|0100(1): 1 instructions",
                "1|L--|Mem0[0x0001<p16>:byte] = 0<8>");
        }

        [Test]
        public void Pdk15Rw_dzsn()
        {
            Given_HexString("6300");
            AssertCode(     // dzsn	a
                "0|T--|0100(1): 2 instructions",
                "1|L--|a = a - 1<8>",
                "2|T--|if (a == 0<8>) branch 0102");
        }

        [Test]
        public void Pdk15Rw_engint()
        {
            Given_HexString("7800");
            AssertCode(     // engint
                "0|L--|0100(1): 1 instructions",
                "1|L--|__enable_global_interrupts()");
        }

        [Test]
        public void Pdk15Rw_goto()
        {
            Given_HexString("1D60");
            AssertCode(     // goto	001D
                "0|T--|0100(1): 1 instructions",
                "1|T--|goto 001D");
        }

        [Test]
        public void Pdk15Rw_idxm_m()
        {
            Given_HexString("0007");
            AssertCode(     // idxm	[[0x0]],a
                "0|L--|0100(1): 2 instructions",
                "1|L--|v4 = Mem0[null:byte]",
                "2|L--|Mem0[v4:byte] = a");
        }

        [Test]
        public void Pdk15Rw_inc()
        {
            Given_HexString("0024");
            AssertCode(     // inc	[0x0]
                "0|L--|0100(1): 3 instructions",
                "1|L--|v3 = Mem0[null:byte] + 1<8>",
                "2|L--|Mem0[null:byte] = v3",
                "3|L--|ZCAV = cond(v3)");
        }

        [Test]
        public void Pdk15Rw_mov()
        {
            Given_HexString("B357");
            AssertCode(     // mov	a,0xB3
                "0|L--|0100(1): 1 instructions",
                "1|L--|a = 0xB3<8>");
        }

        [Test]
        public void Pdk15Rw_mov_IO_a()
        {
            Given_HexString("2101");
            AssertCode(     // mov	IO(0x21),a
                "0|L--|0100(1): 1 instructions",
                "1|L--|__out(0x21<8>, a)");
        }

        [Test]
        public void Pdk15Rw_or()
        {
            Given_HexString("031D");
            AssertCode(     // or	a,[0x3]
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a | Mem0[0x0003<p16>:byte]",
                "2|L--|Z = cond(a)");
        }

        [Test]
        public void Pdk15Rw_popaf()
        {
            Given_HexString("7300");
            AssertCode(     // popaf
                "0|L--|0100(1): 4 instructions",
                "1|L--|sp = sp - 1<i16>",
                "2|L--|f = Mem0[sp:byte]",
                "3|L--|sp = sp - 1<i16>",
                "4|L--|a = Mem0[sp:byte]");
        }

        [Test]
        public void Pdk15Rw_pushaf()
        {
            Given_HexString("7200");
            AssertCode(     // pushaf
                "0|L--|0100(1): 4 instructions",
                "1|L--|Mem0[sp:byte] = a",
                "2|L--|sp = sp + 1<i16>",
                "3|L--|Mem0[sp:byte] = f",
                "4|L--|sp = sp + 1<i16>");
        }

        [Test]
        public void Pdk15Rw_ret_n()
        {
            Given_HexString("4D02");
            AssertCode(     // ret	0x4D
                "0|R--|0100(1): 2 instructions",
                "1|L--|a = 0x4D<8>",
                "2|R--|return (2,0)");
        }

        [Test]
        public void Pdk15Rw_set0()
        {
            Given_HexString("053B");
            AssertCode(     // set0	IO(0x5).6
                "0|L--|0100(1): 1 instructions",
                "1|L--|__out_bit(5<8>, 6<8>, false)");
        }

        [Test]
        public void Pdk15Rw_set1()
        {
            Given_HexString("043F");
            AssertCode(     // set1	IO(0x4).6
                "0|L--|0100(1): 1 instructions",
                "1|L--|__out_bit(4<8>, 6<8>, true)");
        }

        [Test]
        public void Pdk15Rw_sl()
        {
            Given_HexString("B32B");
            AssertCode(     // sl	[0x179]
                "0|L--|0100(1): 3 instructions",
                "1|L--|v3 = Mem0[0x00B3<p16>:byte] << 1<8>",
                "2|L--|Mem0[0x00B3<p16>:byte] = v3",
                "3|L--|C = cond(v3)");
        }

        [Test]
        public void Pdk15Rw_slc()
        {
            Given_HexString("B42D");
            AssertCode(     // slc	[0x180]
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = __rcl<byte,byte>(Mem0[0x00B4<p16>:byte], 1<8>, C)",
                "2|L--|Mem0[0x00B4<p16>:byte] = v4",
                "3|L--|C = cond(v4)");
        }

        [Test]
        public void Pdk15Rw_sr()
        {
            Given_HexString("032A");
            AssertCode(     // sr	[0x3]
                "0|L--|0100(1): 3 instructions",
                "1|L--|v3 = Mem0[0x0003<p16>:byte] >>u 1<8>",
                "2|L--|Mem0[0x0003<p16>:byte] = v3",
                "3|L--|C = cond(v3)");
        }

        [Test]
        public void Pdk15Rw_sub()
        {
            Given_HexString("8051");
            AssertCode(     // sub	a,0x80
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a - 0x80<8>",
                "2|L--|ZCAV = cond(a)");
        }

        [Test]
        public void Pdk15Rw_subc()
        {
            Given_HexString("6100");
            AssertCode(     // subc	a
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a - C",
                "2|L--|ZCAV = cond(a)");
        }

        [Test]
        public void Pdk15Rw_swap()
        {
            Given_HexString("6E00");
            AssertCode(     // swap	a
                "0|L--|0100(1): 1 instructions",
                "1|L--|a = __swap_nybbles(a)");
        }

        [Test]
        public void Pdk15Rw_t0sn()
        {
            Given_HexString("8030");
            AssertCode(     // t0sn	IO(0x0).1
                "0|T--|0100(1): 2 instructions",
                "1|L--|v3 = __in_bit(0<8>, 1<8>)",
                "2|T--|if (!v3) branch 0102");
        }

        [Test]
        public void Pdk15Rw_xch()
        {
            Given_HexString("0027");
            AssertCode(     // xch	[0x0]
                "0|L--|0100(1): 3 instructions",
                "1|L--|v3 = Mem0[null:byte]",
                "2|L--|Mem0[null:byte] = a",
                "3|L--|a = v3");
        }

        [Test]
        public void Pdk15Rw_xor()
        {
            Given_HexString("8A16");
            AssertCode(     // xor	[0x008A],a
                "0|L--|0100(1): 3 instructions",
                "1|L--|v4 = Mem0[0x008A<p16>:byte] ^ a",
                "2|L--|Mem0[0x008A<p16>:byte] = v4",
                "3|L--|Z = cond(v4)");
        }

        [Test]
        public void Pdk15Rw_stopexe()
        {
            Given_HexString("7700");
            AssertCode(     // stopexe
                "0|L--|0100(1): 1 instructions",
                "1|L--|__stopexe()");
        }
    }
}
