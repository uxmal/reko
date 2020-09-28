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
using Reko.Arch.Tms7000;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Tms7000
{
    [TestFixture]
    public class Tms7000RewriterTests : RewriterTestBase
    {
        private readonly Tms7000Architecture arch;

        public Tms7000RewriterTests()
        {
            this.arch = new Tms7000Architecture("tms7000");
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => Address.Ptr16(0x0100);

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return new Tms7000Rewriter(arch, new BeImageReader(mem, 0), new Tms7000State(arch), binder, host);
        }

        [Test]
        public void Tms7000Rw_nop()
        {
            Given_Bytes(0x00);
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void Tms7000rw_and()
        {
            Given_Bytes(0x63);
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|a = a & b",
                "2|L--|NZ = cond(a)",
                "3|L--|C = false");
        }

        [Test]
        public void Tms7000rw_add()
        {
            Given_Bytes(0x58, 0x32);
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|b = b + 0x32",
                "2|L--|CNZ = cond(b)");
        }

        [Test]
        public void Tms7000rw_andp()
        {
            Given_Bytes(0xA3, 0x03, 0x44);
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|p68 = p68 & 0x03",
                "2|L--|NZ = cond(p68)",
                "3|L--|C = false");
        }

        [Test]
        public void Tms7000rw_btjz()
        {
            Given_Bytes(0x27, 0x0F, 0x44);
            AssertCode(
                "0|T--|0100(3): 3 instructions",
                "1|L--|NZ = cond(a & ~0x0F)",
                "2|L--|C = false",
                "3|T--|if (Test(NE,Z)) branch 0147");
        }

        [Test]
        public void Tms7000rw_br_direct()
        {
            Given_Bytes(0x8C, 0x12, 0x34);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 1234");
        }

        [Test]
        public void Tms7000rw_br_indexed()
        {
            Given_Bytes(0xAC, 0x12, 0x34);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|goto 0x1234 + (uint16) b");
        }

        [Test]
        public void Tms7000rw_br_indirect()
        {
            Given_Bytes(0x9C, 0x12);
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto r18_r17");
        }

        [Test]
        public void Tms7000rw_call()
        {
            Given_Bytes(0x8E, 0x12, 0x34);
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|call 1234 (2)");
        }

        [Test]
        public void Tms7000rw_clr()
        {
            Given_Bytes(0xC5);
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|b = 0x00",
                "2|L--|C = false",
                "3|L--|N = false",
                "4|L--|Z = true");
        }

        [Test]
        public void Tms7000rw_tsta()
        {
            Given_Bytes(0xB0);
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|NZ = cond(a)",
                "2|L--|C = false");
        }

        [Test]
        public void Tms7000rw_dec()
        {
            Given_Bytes(0xB2);
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a - 0x01",
                "2|L--|CNZ = cond(a)");
        }

        [Test]
        public void Tms7000rw_decd()
        {
            Given_Bytes(0xDB, 0x12);
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|r18_r17 = r18_r17 - 0x0001",
                "2|L--|CNZ = cond(r18_r17)");
        }

        [Test]
        public void Tms7000rw_dint()
        {
            Given_Bytes(0x06);
            AssertCode(
                "0|L--|0100(1): 4 instructions",
                "1|L--|__dint()",
                "2|L--|C = false",
                "3|L--|N = false",
                "4|L--|Z = false");
        }

        [Test]
        public void Tms7000rw_djnz()
        {
            Given_Bytes(0xCA, 0xFE);
            AssertCode(
                "0|T--|0100(2): 2 instructions",
                "1|L--|b = b - 0x01",
                "2|T--|if (b != 0x00) branch 0100");
        }

        [Test]
        public void Tms7000Rw_jmp()
        {
            Given_Bytes(0xE0, 0xC0);
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 00C2");
        }


        [Test]
        public void Tms7000Rw_jl()
        {
            Given_Bytes(0xE7, 0xC0);
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 00C2");
        }

        [Test]
        public void Tms7000Rw_lda()
        {
            Given_Bytes(0x8A, 0x12, 0x34);
            AssertCode(
                 "0|L--|0100(3): 3 instructions",
                 "1|L--|a = Mem0[0x1234:byte]",
                 "2|L--|NZ = cond(a)",
                 "3|L--|C = false");
        }

        [Test]
        public void Tms7000Rw_ldsp()
        {
            Given_Bytes(0x0D);
            AssertCode(
                 "0|L--|0100(1): 1 instructions",
                 "1|L--|sp = b");
        }

        [Test]
        public void Tms7000Rw_mov()
        {
            Given_Bytes(0x52, 0x52);
            AssertCode(
                 "0|L--|0100(2): 3 instructions",
                 "1|L--|b = 0x52",
                 "2|L--|NZ = cond(b)",
                 "3|L--|C = false");
        }

        [Test]
        public void Tms7000Rw_movd_imm()
        {
            Given_Bytes(0x88, 0x12, 0x34, 0x0A);
            AssertCode(
                 "0|L--|0100(4): 3 instructions",
                 "1|L--|r10_r9 = 0x1234",
                 "2|L--|NZ = cond(r10_r9)",
                 "3|L--|C = false");
        }

        [Test]
        public void Tms7000Rw_movd_idx()
        {
            Given_Bytes(0xA8, 0x12, 0x34, 0x0A);
            AssertCode(
                 "0|L--|0100(4): 3 instructions",
                 "1|L--|r10_r9 = 0x1234 + (uint16) b",
                 "2|L--|NZ = cond(r10_r9)",
                 "3|L--|C = false");
        }

        [Test]
        public void Tms7000Rw_pop()
        {
            Given_Bytes(0xB9);
            AssertCode(
                 "0|L--|0100(1): 4 instructions",
                 "1|L--|a = Mem0[(ptr16) sp:byte]",
                 "2|L--|sp = sp - 0x01",
                 "3|L--|NZ = cond(a)",
                 "4|L--|C = false");
        }

        [Test]
        public void Tms7000Rw_push_st()
        {
            Given_Bytes(0x0E);
            AssertCode(
                 "0|L--|0100(1): 2 instructions",
                 "1|L--|sp = sp + 0x01",
                 "2|L--|Mem0[(ptr16) sp:byte] = st");
        }

        [Test]
        public void Tms7000Rw_reti()
        {
            Given_Bytes(0x0B);
            AssertCode(
                 "0|T--|0100(1): 1 instructions",
                 "1|T--|return (2,1)");
        }

        [Test]
        public void Tms7000Rw_rets()
        {
            Given_Bytes(0x0A);
            AssertCode(
                 "0|T--|0100(1): 1 instructions",
                 "1|T--|return (2,0)");
        }

        [Test]
        public void Tms7000Rw_rl()
        {
            Given_Bytes(0xBE);
            AssertCode(
                 "0|L--|0100(1): 2 instructions",
                 "1|L--|a = __rol(a)",
                 "2|L--|C = cond(a)");
        }

        [Test]
        public void Tms7000Rw_rlc()
        {
            Given_Bytes(0xBF);
            AssertCode(
                 "0|L--|0100(1): 2 instructions",
                 "1|L--|a = __rcl(a, C)",
                 "2|L--|C = cond(a)");
        }

        [Test]
        public void Tms7000Rw_sbb()
        {
            Given_Bytes(0x1B, 0x03);
            AssertCode(
                 "0|L--|0100(2): 2 instructions",
                 "1|L--|r3 = r3 - a - C",
                 "2|L--|C = cond(r3)");
        }

        [Test]
        public void Tms7000Rw_setc()
        {
            Given_Bytes(0x07);
            AssertCode(
                 "0|L--|0100(1): 3 instructions",
                 "1|L--|C = true",
                 "2|L--|N = false",
                 "3|L--|Z = true");
        }


        [Test]
        public void Tms7000Rw_sta()
        {
            Given_Bytes(0xAB, 0x12, 0x34);
            AssertCode(
                 "0|L--|0100(3): 1 instructions",
                 "1|L--|Mem0[0x1234 + (uint16) b:byte] = a");
        }


        [Test]
        public void Tms7000Rw_stsp()
        {
            Given_Bytes(0x09);
            AssertCode(
                 "0|L--|0100(1): 1 instructions",
                 "1|L--|b = sp");
        }

        [Test]
        public void Tms7000Rw_tstb()
        {
            Given_Bytes(0xC1);
            AssertCode(
                   "0|L--|0100(1): 2 instructions",
                   "1|L--|NZ = cond(b)",
                   "2|L--|C = false");
        }

        [Test]
        public void Tms7000Rw_xchb()
        {
            Given_Bytes(0xD6, 0x05);
            AssertCode(
                   "0|L--|0100(2): 3 instructions",
                   "1|L--|v4 = r5",
                   "2|L--|r5 = b",
                   "3|L--|b = v4");
        }
    }
}
