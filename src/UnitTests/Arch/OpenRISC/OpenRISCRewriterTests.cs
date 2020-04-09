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
using Reko.Arch.OpenRISC;
using Reko.Core;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.OpenRISC
{
    [TestFixture]
    public class OpenRISCRewriterTests : RewriterTestBase
    {
        private OpenRISCArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new OpenRISCArchitecture("openRisc");
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                arch.CreateImageReader(mem, 0),
                new OpenRISCState(arch),
                binder,
                host);
        }

        [Test]
        public void OpenRiscRw_add()
        {
            Given_HexString("E0432800");	// l.add	r2,r3,r5
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r3 + r5",
                "2|L--|CV = cond(r2)");
        }

        [Test]
        public void OpenRiscRw_addc()
        {
            Given_HexString("E0432801");	// l.addc	r2,r3,r5
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r2 = r3 + r5 + C",
                "2|L--|CV = cond(r2)");
        }

        [Test]
        public void OpenRiscRw_addi()
        {
            Given_HexString("9C21FFF8");	// l.addi	r1,r1,-00000008
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 - 8",
                "2|L--|CV = cond(r1)");
        }

        [Test]
        public void OpenRiscRw_addic()
        {
            Given_HexString("A021FFF8");	// l.addic	r1,r1,-00000008
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r1 = r1 + -8 + C",
                "2|L--|CV = cond(r1)");
        }

        [Test]
        public void OpenRiscRw_adrp()
        {
            Given_HexString("0A007761");	// l.adrp	r16,0EED2428
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = 0EFC2000");
        }

        [Test]
        public void OpenRiscRw_and()
        {
            Given_HexString("E0C63803");	// l.and	r6,r6,r7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = r6 & r7");
        }

        [Test]
        public void OpenRiscRw_andi()
        {
            Given_HexString("A4840001");	// l.andi	r4,r4,00000001
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r4 & 0x00000001");
        }

        [Test]
        public void OpenRiscRw_bf()
        {
            Given_HexString("10000027");	// l.bf	0010009C
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (F) branch 0010009C");
        }

        [Test]
        public void OpenRiscRw_bnf()
        {
            Given_HexString("0C00000E");	// l.bnf	00100038
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (!F) branch 00100038");
        }

        [Test]
        public void OpenRiscRw_cmov()
        {
            Given_HexString("E042B00E");	// l.cmov	0000483C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = F ? r2 : r22");
        }

        [Test]
        public void OpenRiscRw_j_0()
        {
            Given_HexString("00000000");	// l.j	00000000
            AssertCode(
                "0|H--|00100000(4): 1 instructions",
                "1|TD-|goto 00100000");
        }

        [Test]
        public void OpenRiscRw_j()
        {
            Given_HexString("00000420");	// l.j	00001080
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto 00101080");
        }

        [Test]
        public void OpenRiscRw_jal()
        {
            Given_HexString("04002124");	// l.jal	0000C8A0
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call 00108490 (0)");
        }

        [Test]
        public void OpenRiscRw_jalr()
        {
            Given_HexString("48005800");	// l.jalr	r11
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|call r11 (0)");
        }

        [Test]
        public void OpenRiscRw_jr()
        {
            Given_HexString("44004000");	// l.jr	r8
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|goto r8");
        }

        [Test]
        public void OpenRiscRw_jr_r9()
        {
            Given_HexString("44004800");	// l.jr	r9
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void OpenRiscRw_lbs()
        {
            Given_HexString("91610002");	// l.lbs	r11,2(r1)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = (int32) Mem0[r1 + 2:int8]");
        }

        [Test]
        public void OpenRiscRw_lbz()
        {
            Given_HexString("8C620000");	// l.lbz	r3,0(r2)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = (word32) Mem0[r2:byte]");
        }

        [Test]
        public void OpenRiscRw_lbz_off()
        {
            Given_HexString("8C62FFFF");	// l.lbz	r3,-1(r2)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = (word32) Mem0[r2 - 1:byte]");
        }

        [Test]
        public void OpenRiscRw_lf()
        {
            Given_HexString("69740A00");	// l.lf	r11,2560(r20)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = Mem0[r20 + 2560:real32]");
        }

        [Test]
        public void OpenRiscRw_lhz()
        {
            Given_HexString("97DC0018");	// l.lhz	r30,24(r28)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = (word32) Mem0[r28 + 24:word16]");
        }

        [Test]
        public void OpenRiscRw_lwa()
        {
            Given_HexString("6E746572");	// l.lwa	r19,25970(r20)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = __atomic_load_w32(r20 + 25970)");
        }

        [Test]
        public void OpenRiscRw_lwz()
        {
            Given_HexString("84620000");	// l.lwz	r3,0(r2)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = Mem0[r2:word32]");
        }

        [Test]
        public void OpenRiscRw_maci()
        {
            Given_HexString("4D697373");	// l.maci	r9,+00007373
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v3 = r9 * 29555",
                "2|L--|MACHI_MACLO = MACHI_MACLO + v3");
        }

        [Test]
        public void OpenRiscRw_macrc()
        {
            Given_HexString("18010000");	// l.macrc	r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = MACLO",
                "2|L--|MACHI_MACLO = 0x0000000000000000");
        }

        [Test]
        public void OpenRiscRw_mfspr()
        {
            Given_HexString("B4830000");	// l.mfspr	r4,r3,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = __mfspr(VR)");
        }

        [Test]
        public void OpenRiscRw_mtspr()
        {
            Given_HexString("C0032000");	// l.mtspr	r3,r4,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__mtspr(VR, r4)");
        }

        [Test]
        public void OpenRiscRw_nop()
        {
            Given_HexString("15000000");	// l.nop
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void OpenRiscRw_or()
        {
            Given_HexString("E0E74004");	// l.or	r7,r7,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r7 | r8");
        }

        [Test]
        public void OpenRiscRw_ori()
        {
            Given_HexString("A8A51540");	// l.ori	r5,r5,00001540
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r5 | 0x00001540");
        }

        [Test]
        public void OpenRiscRw_rfe()
        {
            Given_HexString("24000000");	// l.rfe
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void OpenRiscRw_sb()
        {
            Given_HexString("D8021800");	// l.sb	0(r2),r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r2:byte] = SLICE(r3, byte, 0)");
        }

        [Test]
        public void OpenRiscRw_sh()
        {
            Given_HexString("DC165800");	// l.sh	0(r22),r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r22:word16] = SLICE(r11, word16, 0)");
        }

        [Test]
        public void OpenRiscRw_sw()
        {
            Given_HexString("D7E14FFC");	// l.sw	-4(r1),r9
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r1 - 4:word32] = r9");
        }

        [Test]
        public void OpenRiscRw_sfeq()
        {
            Given_HexString("E4032000");	// l.sfeq	r3,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r3 == r4");
        }

        [Test]
        public void OpenRiscRw_sfeqi()
        {
            Given_HexString("BC050000");	// l.sfeqi	r5,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r5 == 0x00000000");
        }

        [Test]
        public void OpenRiscRw_sfges()
        {
            Given_HexString("E5652000");	// l.sfges	r5,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r5 >= r4");
        }

        [Test]
        public void OpenRiscRw_sfgesi()
        {
            Given_HexString("BD630000");	// l.sfgesi	r3,+00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r3 >= 0");
        }

        [Test]
        public void OpenRiscRw_sfgeu()
        {
            Given_HexString("E4715800");	// l.sfgeu	r17,r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r17 >=u r11");
        }

        [Test]
        public void OpenRiscRw_sfgtsi()
        {
            Given_HexString("BD460003");	// l.sfgtsi	r6,+00000003
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r6 > 3");
        }

        [Test]
        public void OpenRiscRw_sfgtu()
        {
            Given_HexString("E4454000");	// l.sfgtu	r5,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r5 >u r8");
        }

        [Test]
        public void OpenRiscRw_sfgtui()
        {
            Given_HexString("BC580003");	// l.sfgtui	r24,00000003
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r24 >u 0x00000003");
        }

        [Test]
        public void OpenRiscRw_sfles()
        {
            Given_HexString("E5A35800");	// l.sfles	r3,r11
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r3 <= r11");
        }

        [Test]
        public void OpenRiscRw_sflesi()
        {
            Given_HexString("BDA80013");	// l.sflesi	r8,+00000013
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r8 <= 19");
        }

        [Test]
        public void OpenRiscRw_sfleu()
        {
            Given_HexString("E4A41800");	// l.sfleu	r4,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r4 <=u r3");
        }

        [Test]
        public void OpenRiscRw_sfleui()
        {
            Given_HexString("BCA50001");	// l.sfleui	r5,00000001
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r5 <=u 0x00000001");
        }

        [Test]
        public void OpenRiscRw_sfltsi()
        {
            Given_HexString("BD830000");	// l.sfltsi	r3,+00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r3 < 0");
        }

        [Test]
        public void OpenRiscRw_sflts()
        {
            Given_HexString("E5850000");	// l.sflts	r5,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r5 < 0x00000000");
        }

        [Test]
        public void OpenRiscRw_sfltu()
        {
            Given_HexString("E4886800");	// l.sfltu	r8,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r8 <u r13");
        }

        [Test]
        public void OpenRiscRw_sfltui()
        {
            Given_HexString("BC840055");	// l.sfltui	r4,00000055
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r4 <u 0x00000055");
        }

        [Test]
        public void OpenRiscRw_sfne()
        {
            Given_HexString("E4223000");	// l.sfne	r2,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r2 != r6");
        }

        [Test]
        public void OpenRiscRw_sfnei()
        {
            Given_HexString("BC230000");	// l.sfnei	r3,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|F = r3 != 0x00000000");
        }

        [Test]
        public void OpenRiscRw_slli()
        {
            Given_HexString("B8630004");	// l.slli	r3,r3,00000004
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = r3 << 0x00000004");
        }

        [Test]
        public void OpenRiscRw_srai()
        {
            Given_HexString("B8A50098");	// l.srai	r5,r5,00000018
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r5 >> 0x00000018");
        }

        [Test]
        public void OpenRiscRw_srli()
        {
            Given_HexString("B8A30042");	// l.srli	r5,r3,00000002
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r3 >>u 0x00000002");
        }

        [Test]
        public void OpenRiscRw_sub()
        {
            Given_HexString("E0805802");	// l.sub	r4,r0,r11
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r4 = 0x00000000 - r11",
                "2|L--|CV = cond(r4)");
        }

        [Test]
        public void OpenRiscRw_sys()
        {
            Given_HexString("20006372");	// l.sys	00006372
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__syscall(0x00006372)");
        }

        [Test]
        public void OpenRiscRw_xor()
        {
            Given_HexString("E18C3005");	// l.xor	r12,r12,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r12 = r12 ^ r6");
        }

        [Test]
        public void OpenRiscRw_xori()
        {
            Given_HexString("ACC6FFFF");	// l.xori	r6,r6,0000FFFF
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r6 = r6 ^ 0x0000FFFF");
        }

        [Test]
        public void OpenRiscRw_l_sll()
        {
            Given_HexString("E0EB3808");	// l.sll	r7,r11,r7
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = r11 << r7");
        }

        [Test]
        public void OpenRiscRw_l_mul()
        {
            Given_HexString("E2028306");	// l.mul	r16,r2,r16
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r16 = r2 *s r16",
                "2|L--|V = cond(r16)");
        }

        [Test]
        public void OpenRiscRw_l_srl()
        {
            Given_HexString("E0AB2848");	// l.srl	r5,r11,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = r11 >>u r5");
        }

        [Test]
        public void OpenRiscRw_l_sra()
        {
            Given_HexString("E16B7088");	// l.sra	r11,r11,r14
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r11 = r11 >> r14");
        }

        [Test]
        public void OpenRiscRw_csync()
        {
            Given_HexString("23000000");  // l.csync
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__csync()");
        }

        [Test]
        public void OpenRiscRw_psync()
        {
            Given_HexString("22800000");  // l.psync
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__psync()");
        }

        [Test]
        public void OpenRiscRw_l_msync()
        {
            Given_HexString("22000000");	// l.msync
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__msync()");
        }
    }
}
