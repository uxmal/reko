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

using Moq;
using NUnit.Framework;
using Reko.Arch.LatticeMico;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.LatticeMico
{
    [TestFixture]
    public class LatticeMico32RewriterTests : RewriterTestBase
    {
        private readonly LatticeMico32Architecture arch = new LatticeMico32Architecture("latticeMico32");
        private readonly Address addrLoad = Address.Ptr32(0x00100000);

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var rdr = arch.CreateImageReader(mem, 0);
            return arch.CreateRewriter(rdr, arch.CreateProcessorState(), binder, host);
        }

        [Test]
        public void Lm32Rw_Invalid()
        {
            Given_HexString("86C1ABFA"); // Invalid
            AssertCode(
                "0|---|00100000(4): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void Lm32Rw_reserved()
        {
            Given_HexString("CF83E9C1"); // reserved
            AssertCode(
                "0|---|00100000(4): 1 instructions",
                "1|---|<invalid>");
        }

        [Test]
        public void Lm32Rw_add()
        {
            Given_HexString("B4694800"); // add	r9,r3,r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r3 + r9");
        }

        [Test]
        public void Lm32Rw_addi()
        {
            Given_HexString("34C7DE88"); // addi	r7,r6,FFFFDE88
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r6 + 0xFFFFDE88");
        }

        [Test]
        public void Lm32Rw_andhi()
        {
            Given_HexString("636A76B3"); // andhi	r10,fp,000076B3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = fp & 0x76B30000");
        }

        [Test]
        public void Lm32Rw_andi()
        {
            Given_HexString("20753331"); // andi	r21,r3,00003331
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = r3 & 0x00003331");
        }

        [Test]
        public void Lm32Rw_b()
        {
            Given_HexString("C1000000");    // b r8
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r8");
        }

        [Test]
        public void Lm32Rw_b_ra()
        {
            Given_HexString("C3A00000");    // b ra
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void Lm32Rw_be()
        {
            Given_HexString("45EAD0A6"); // be	r10,r15,000F42AC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r10 == r15) branch 000F4298");
        }

        [Test]
        public void Lm32Rw_bg()
        {
            Given_HexString("4B1149E0"); // bg	r17,r24,00112934
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r17 > r24) branch 00112780");
        }

        [Test]
        public void Lm32Rw_bgu()
        {
            Given_HexString("558178B5"); // bgu	r1,r12,0011E3B4
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r1 >u r12) branch 0011E2D4");
        }

        [Test]
        public void Lm32Rw_bge()
        {
            Given_HexString("4DCC602B"); // bge	r12,r14,001181B0
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r12 >= r14) branch 001180AC");
        }

        [Test]
        public void Lm32Rw_bgeu()
        {
            Given_HexString("50CB124C"); // bgeu	r11,r6,00104950
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r11 >=u r6) branch 00104930");
        }

        [Test]
        public void Lm32Rw_bi()
        {
            Given_HexString("E048D36E"); // bi	F9334E8C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 01334DB8");
        }

        [Test]
        public void Lm32Rw_bne()
        {
            Given_HexString("5CE07589"); // bne	r0,r7,0011D694
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (0x00000000 != r7) branch 0011D624");
        }

        [Test]
        public void Lm32Rw_call()
        {
            Given_HexString("D8800000"); // call	r4
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call r4 (0)");
        }

        [Test]
        public void Lm32Rw_calli()
        {
            Given_HexString("FBFEF9A3"); // calli	FB83E6E8
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 000BE68C (0)");
        }

        [Test]
        public void Lm32Rw_cmpe()
        {
            Given_HexString("E6421000"); // cmpe	r1,r18,r18
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r18 == r2");
        }

        [Test]
        public void Lm32Rw_cmpei()
        {
            Given_HexString("676F2A16"); // cmpei	r15,fp,00002A16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = fp == 0x00002A16");
        }

        [Test]
        public void Lm32Rw_cmpg()
        {
            Given_HexString("EA1B9000"); // cmpg	r18,r16,fp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = r16 > fp");
        }

        [Test]
        public void Lm32Rw_cmpgi()
        {
            Given_HexString("6AE97BA3"); // cmpgi	r9,r23,00007BA3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r23 > 0x00007BA3");
        }

        [Test]
        public void Lm32Rw_cmpgei()
        {
            Given_HexString("6C31BAD9"); // cmpgei	r1,r0,FFFFBAD9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r1 >= 0xFFFFBAD9");
        }

        [Test]
        public void Lm32Rw_cmpge()
        {
            Given_HexString("EFA25000"); // cmpge	r10,ra,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = ra >= r2");
        }

        [Test]
        public void Lm32Rw_cmpgeu()
        {
            Given_HexString("F21AB800"); // cmpgeu	r23,r16,gp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r16 >=u gp");
        }

        [Test]
        public void Lm32Rw_cmpgeui()
        {
            Given_HexString("73C5A1E5"); // cmpgeui	r5,ea,FFFFA1E5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = ea >=u 0xFFFFA1E5");
        }

        [Test]
        public void Lm32Rw_cmpgui()
        {
            Given_HexString("74432EA1"); // cmpgui	r3,r2,00002EA1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r2 >u 0x00002EA1");
        }

        [Test]
        public void Lm32Rw_cmpne()
        {
            Given_HexString("FDFD5800"); // cmpne	r11,r15,ra
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r15 != ra");
        }

        [Test]
        public void Lm32Rw_cmpnei()
        {
            Given_HexString("7FA229E1"); // cmpnei	r2,ra,000029E1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = ra != 0x000029E1");
        }

        [Test]
        public void Lm32Rw_div()
        {
            Given_HexString("9F442800"); // div	r5,gp,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = gp / r4");
        }

        [Test]
        public void Lm32Rw_divu()
        {
            Given_HexString("8D2E1000"); // divu	r2,r9,r14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r9 /u r14");
        }

        [Test]
        public void Lm32Rw_lb()
        {
            Given_HexString("122DFFFF"); // lb	r13,(r17-1)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = (int32) Mem0[r17 - 1:int8]");
        }

        [Test]
        public void Lm32Rw_lbu()
        {
            Given_HexString("4025FFFF"); // lbu	r5,(r1-1)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = (word32) Mem0[r1 - 1:byte]");
        }

        [Test]
        public void Lm32Rw_lh()
        {
            Given_HexString("1D92FFFE"); // lh	r18,(r12-2)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = (int32) Mem0[r12 - 2:int16]");
        }

        [Test]
        public void Lm32Rw_lhu()
        {
            Given_HexString("2EC9FFFE"); // lhu	r9,(r22-2)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = (word32) Mem0[r22 - 2:word16]");
        }

        [Test]
        public void Lm32Rw_lw()
        {
            Given_HexString("284FFFFC"); // lw	r15,(r2-4)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r15 = Mem0[r2 - 4:word32]");
        }

        [Test]
        public void Lm32Rw_mod()
        {
            Given_HexString("D6DCB000"); // mod	r22,r22,sp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r22 = r22 % sp");
        }

        [Test]
        public void Lm32Rw_modu()
        {
            Given_HexString("C44B1800"); // modu	r3,r2,r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r2 % r11");
        }

        [Test]
        public void Lm32Rw_mul()
        {
            Given_HexString("8A472000"); // mul	r4,r18,r7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r18 * r7");
        }

        [Test]
        public void Lm32Rw_muli()
        {
            Given_HexString("097B47FD"); // muli	fp,r11,000047FD
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = r11 * 0x000047FD");
        }

        [Test]
        public void Lm32Rw_nor()
        {
            Given_HexString("85BDF800"); // nor	ba,r13,ra
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ba = ~(r13 | ra)");
        }

        [Test]
        public void Lm32Rw_nori()
        {
            Given_HexString("05C5AF45"); // nori	r5,r14,FFFFAF45
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = ~(r14 | 0xFFFFAF45)");
        }

        [Test]
        public void Lm32Rw_or()
        {
            Given_HexString("BABF5800"); // or	r11,r21,ba
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r21 | ba");
        }

        [Test]
        public void Lm32Rw_orhi()
        {
            Given_HexString("78308D9D"); // orhi	r16,r1,FFFF8D9D
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = r1 | 0x8D9D0000");
        }

        [Test]
        public void Lm32Rw_ori()
        {
            Given_HexString("3945A263"); // ori	r5,r10,FFFFA263
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r10 | 0xFFFFA263");
        }

        [Test]
        public void Lm32Rw_sb()
        {
            Given_HexString("30A30000"); // sb	(r5),r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r5:byte] = SLICE(r3, byte, 0)");
        }

        [Test]
        public void Lm32Rw_sextb()
        {
            Given_HexString("B396D800"); // sextb	fp,sp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = (int32) (int8) sp");
        }

        [Test]
        public void Lm32Rw_sexth()
        {
            Given_HexString("DCC84000"); // sexth	r8,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = (int32) (int16) r6");
        }

        [Test]
        public void Lm32Rw_sh()
        {
            Given_HexString("0EE60020"); // sh	(r23+32),r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r23 + 32:word16] = SLICE(r6, word16, 0)");
        }

        [Test]
        public void Lm32Rw_sli()
        {
            Given_HexString("3FC5000E"); // sli	r5,ea,0000000E
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = ea << 0x0000000E");
        }

        [Test]
        public void Lm32Rw_sri()
        {
            Given_HexString("16D9000D"); // sri	r25,r22,0000000D
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r25 = r22 >> 0x0000000D");
        }

        [Test]
        public void Lm32Rw_sru()
        {
            Given_HexString("82139000"); // sru	r18,r16,r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = r16 >>u r19");
        }

        [Test]
        public void Lm32Rw_srui()
        {
            Given_HexString("02C9B9A9"); // srui	r9,r22,FFFFB9A9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = r22 >>u 0x00000009");
        }

        [Test]
        public void Lm32Rw_sub()
        {
            Given_HexString("C8552000"); // sub	r4,r2,r21
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r2 - r21");
        }

        [Test]
        public void Lm32Rw_sw()
        {
            Given_HexString("5B530400"); // sw	(gp+1024),r19
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[gp + 1024:word32] = r19");
        }

        [Test]
        public void Lm32Rw_xnor()
        {
            Given_HexString("A7D03800"); // xnor	r7,ea,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = ~(ea ^ r16)");
        }

        [Test]
        public void Lm32Rw_xnori()
        {
            Given_HexString("270A27E3"); // xnori	r10,r24,000027E3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r10 = ~(r24 ^ 0x000027E3)");
        }

        [Test]
        public void Lm32Rw_xor()
        {
            Given_HexString("99843800"); // xor	r7,r12,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r12 ^ r4");
        }

        [Test]
        public void Lm32Rw_xori()
        {
            Given_HexString("1A41D9FB"); // xori	r1,r18,FFFFD9FB
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r18 ^ 0xFFFFD9FB");
        }
    }
}
