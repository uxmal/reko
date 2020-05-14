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
using Reko.Arch.Avr;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr32RewriterTests : RewriterTestBase
    {
        public Avr32RewriterTests()
        {
            this.Architecture = new Avr32Architecture("avr32");
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var rdr = Architecture.CreateImageReader(mem, 0);
            var state = Architecture.CreateProcessorState();
            return Architecture.CreateRewriter(rdr, state, binder, host);
        }

        private void Given_Instruction(string hexBytes)
        {
            Given_HexString(hexBytes);
        }

        [Test]
        public void Avr32Rw_cp_w()
        {
            Given_Instruction("103A");	// cp.w	r10,r8
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|VNZC = cond(r10 - r8)");
        }

        [Test]
        public void Avr32Rw_ld_w_postinc()
        {
            Given_Instruction("1B0B");	// ld.w	r11,sp++
            AssertCode(
                "0|L--|000026A0(2): 3 instructions",
                "1|L--|v3 = Mem0[sp:word32]",
                "2|L--|sp = sp + 4<i32>",
                "3|L--|r11 = v3");
        }

        [Test]
        public void Avr32Rw_mov()
        {
            Given_Instruction("3007");	// mov	r7,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = 0<32>");
        }


        [Test]
        public void Avr32Rw_pushm()
        {
            Given_Instruction("D431");	// pushm	r0-r3,r4-r7,lr
            AssertCode(
                "0|L--|00100000(2): 18 instructions",
                "1|L--|sp = sp - 4<i32>",
                "2|L--|Mem0[sp:word32] = r0",
                "3|L--|sp = sp - 4<i32>",
                "4|L--|Mem0[sp:word32] = r1",
                "5|L--|sp = sp - 4<i32>",
                "6|L--|Mem0[sp:word32] = r2",
                "7|L--|sp = sp - 4<i32>",
                "8|L--|Mem0[sp:word32] = r3",
                "9|L--|sp = sp - 4<i32>",
                "10|L--|Mem0[sp:word32] = r4",
                "11|L--|sp = sp - 4<i32>",
                "12|L--|Mem0[sp:word32] = r5",
                "13|L--|sp = sp - 4<i32>",
                "14|L--|Mem0[sp:word32] = r6",
                "15|L--|sp = sp - 4<i32>",
                "16|L--|Mem0[sp:word32] = r7",
                "17|L--|sp = sp - 4<i32>",
                "18|L--|Mem0[sp:word32] = lr");
        }

        [Test]
        public void Avr32Rw_sub3_0()
        {
            Given_Instruction("F8CB0000");	// sub	r11,r12,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r12");
        }


        [Test]
        public void Avr32Rw_sub3_neg4()
        {
            Given_Instruction("F8CBFFFC");	// sub	r11,r12,-4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r12 + 4<32>");
        }


        [Test]
        public void Avr32Rw_sum3_imm()
        {
            // aSSertCode("@@@", "FECCD140");
        }
    }
}
