#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Arch.CSky;
using Reko.Core;

namespace Reko.UnitTests.Arch.CSky
{
    public class CSkyRewriterTests : RewriterTestBase
    {
        private readonly CSkyArchitecture arch;
        private readonly Address addr;

        public CSkyRewriterTests()
        {
            this.arch = new CSkyArchitecture(CreateServiceContainer(), "csky", new());
            this.addr = Address.Ptr32(0x10_0000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        [Test]
        public void CSkyRw_abs()
        {
            Given_HexString("1FC4 1E02");
            AssertCode(     // abs\t1E,1F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = abs<int32>(r31)");
        }

        [Test]
        public void CSkyRw_addc_16()
        {
            Given_HexString("E161");
            AssertCode(     // abs\tr30,r31
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r7 = r7 + r8 + C",
                "2|L--|C = cond(r7)");
        }

        [Test]
        public void CSkyRw_addc_32()
        {
            Given_HexString("1EC7 5F00");
            AssertCode(     // abs\tr30,r31
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r31 = r30 + r24 + C",
                "2|L--|C = cond(r31)");
        }

        [Test]
        public void CSkyRw_addi()
        {
            Given_HexString("FF20");
            AssertCode(     // addi\tr0,0x100
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = r7 + 0x100<32>");
        }

        [Test]
        public void CSkyRw_addi_16()
        {
            Given_HexString("0220");
            AssertCode(     // addi\tr0,0x3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 + 3<32>");
        }

        [Test]
        public void CSkyRw_addi_16_b()
        {
            Given_HexString("065E");
            AssertCode(     // addi\tr0,r6,0x2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r6 + 2<32>");
        }

        [Test]
        public void CSkyRw_addi_r28()
        {
            Given_HexString("DFCF FFFF");
            AssertCode(     // addi\tr30,r28,0x40000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r30 = r28 + 0x40000<32>");
        }

        [Test]
        public void CSkyRw_addi_sp()
        {
            Given_HexString("FF1E");
            AssertCode(     // addi\tr30,r28,0x40000
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r6 = r14 + 0x3FC<32>");
        }

        [Test]
        public void CSkyRw_addi_sp_sp()
        {
            Given_HexString("1F17");
            AssertCode(     // addi\tr30,r28,0x40000
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r14 + 0x1FC<32>");
        }

        [Test]
        public void CSkyRw_addu_16()
        {
            Given_HexString("D463");
            AssertCode(     // addu\tr14,r14,0x1FC
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r15 = r15 + r5");
        }

        [Test]
        public void CSkyRw_addu_3_16()
        {
            Given_HexString("B85C");
            AssertCode(     // addu\tr14,r14,0x1FC
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = r4 + r6");
        }

        [Test]
        public void CSkyRw_addu_32()
        {
            Given_HexString("14C7 3500");
            AssertCode(     // addu\tr14,r14,0x1FC
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = r20 + r24");
        }

        [Test]
        public void CSkyRw_and_16()
        {
            Given_HexString("606A");
            AssertCode(     // and\tr8,r9
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r9 = r9 & r8");
        }

        [Test]
        public void CSkyRw_and_32()
        {
            Given_HexString("14C7 3120");
            AssertCode(     // and\tr17,r20,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r20 & r24");
        }

        [Test]
        public void CSkyRw_andi()
        {
            Given_HexString("31E7 FF2F");
            AssertCode(     // and\tr17,r20,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r25 = r17 & 0xFFF<32>");
        }

        [Test]
        public void CSkyRw_andn_16()
        {
            Given_HexString("6D6A");
            AssertCode(     // andn\tr9,r9
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r9 = r9 & ~r11");
        }

        [Test]
        public void CSkyRw_andn_32()
        {
            Given_HexString("14C7 5120");
            AssertCode(     // andn\tr17,r20,r24
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r20 & ~r24");
        }

        [Test]
        public void CSkyRw_andni()
        {
            Given_HexString("33E6 FF3F");
            AssertCode( // andni\tr17,r19,0xFFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r19 & ~0xFFF<32>");
        }

        [Test]
        public void CSkyRw_asr_16()
        {
            Given_HexString("4672");
            AssertCode(     // asr\tr9,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r9 = r9 >> r1");
        }

        [Test]
        public void CSkyRw_asr_32()
        {
            Given_HexString("21C6 9340");
            AssertCode(     // asr\tr19,r1,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = r1 >> r17");
        }

        [Test]
        public void CSkyRw_asrc()
        {
            Given_HexString("31C6 844C");
            AssertCode(     // asr\tr19,r1,r17
                "0|L--|00100000(4): 2 instructions",
                "1|L--|C = (r17 & 0x20000<32>) != 0<32>",
                "2|L--|r4 = r17 >> 18<i32>");
        }

        [Test]
        public void CSkyRw_asri_16()
        {
            Given_HexString("7F54");
            AssertCode(     // asri\tr3,r4,0x20
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r4 >> 31<i32>");
        }

        [Test]
        public void CSkyRw_asri_32()
        {
            Given_HexString("61C7 9148");
            AssertCode(      // asr\tr17,r1,0x1C
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r1 >> 27<i32>");
        }

        [Test]
        public void CSkyRw_bclri_16()
        {
            Given_HexString("9F3F");
            AssertCode(     // bclri\tr7,0x1F
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = __clear_bit<word32,int32>(r7, 31<i32>)");
        }

        [Test]
        public void CSkyRw_bclri_32()
        {
            Given_HexString("48C7 3228");
            AssertCode(     // bclri\tr7,0x1F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = __clear_bit<word32,int32>(r8, 18<i32>)");
        }

        [Test]
        public void CSkyRw_bez()
        {
            Given_HexString("11E9FFFF");
            AssertCode(     // bez\tr17,000FFFFE
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r17 == 0<32>) branch 000FFFFE");
        }

        [Test]
        public void CSkyRw_bf_16()
        {
            Given_HexString("FF0F");
            AssertCode(     // bez\tr17,000FFFFE
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (!C) branch 000FFFFE");
        }

        [Test]
        public void CSkyRw_bf_32()
        {
            Given_HexString("40E8FFFF");
            AssertCode(     // bez\tr17,000FFFFE
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (!C) branch 000FFFFE");
        }

        [Test]
        public void CSkyRw_bgenr()
        {
            Given_HexString("11C4 5C50");
            AssertCode(     // bgenr\tr28,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = 1<32> << r17");
        }

        [Test]
        public void CSkyRw_bhsz()
        {
            Given_HexString("B3E9 0080");
            AssertCode(     // bf\t000FFFFE
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r19 >= 0<32>) branch 000F0000");
        }

        [Test]
        public void CSkyRw_bkpt()
        {
            Given_HexString("0000");
            AssertCode(     // bkpt
                "0|L--|00100000(2): 1 instructions",
                "1|L--|__bkpt()");
        }

        [Test]
        public void CSkyRw_bmaski()
        {
            Given_HexString("20C6 3150");
            AssertCode(     // bmaski\tr17,0x12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = 0x3FFFF<32>");
        }

        [Test]
        public void CSkyRw_bmclr()
        {
            Given_HexString("00C0 2014");
            AssertCode(     // bmclr
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__bmclr()");
        }

        [Test]
        public void CSkyRw_bmset()
        {
            Given_HexString("00C0 2010");
            AssertCode(     // bmset
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__bmset()");
        }

        [Test]
        [Ignore("Hard to understand the manual")]
        public void CSkyRw_bpop_h()
        {
            Given_HexString("B414");
            AssertCode(     // bpop.h\tr5
                  "0|@@@");
        }

        [Test]
        [Ignore("Hard to understand the manual")]
        public void CSkyRw_bpop_w()
        {
            Given_HexString("B614");
            AssertCode(     // bpop.w\tr5
                  "0|@@@");
        }

        [Test]
        [Ignore("Hard to understand the manual")]
        public void CSkyRw_bpush_h()
        {
            Given_HexString("F414");
            AssertCode(     // bpush.h\tr5
                  "0|@@@");
        }

        [Test]
        [Ignore("Hard to understand the manual")]
        public void CSkyRw_bpush_w()
        {
            Given_HexString("F614");
            AssertCode(     // bpush.w\tr5
                  "0|@@@");
        }

        [Test]
        public void CSkyRw_br_16()
        {
            Given_HexString("0006");
            AssertCode(     // br\t000FFC00
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 000FFC00");
        }

        [Test]
        public void CSkyRw_br_32()
        {
            Given_HexString("00E8 0080");
            AssertCode(     // br\t000F0000
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000F0000");
        }

        [Test]
        public void CSkyRw_brev()
        {
            Given_HexString("11C4 1362");
            AssertCode(     // brev\tr19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r19 = __reverse_bits<word32>(r17)");
        }

        [Test]
        public void CSkyRw_bseti()
        {
            Given_HexString("BF3F");
            AssertCode(     // addi\tr0,0x3
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = __set_bit<word32,int32>(r7, 31<i32>)");
        }

        [Test]
        public void CSkyRw_bseti_16()
        {
            Given_HexString("BF3F");
            AssertCode(     // bseti\tr7,0x1F
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = __set_bit<word32,int32>(r7, 31<i32>)");
        }

        [Test]
        public void CSkyRw_bseti_32()
        {
            Given_HexString("48C7 5228");
            AssertCode(     // bclri\tr7,0x1F
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = __set_bit<word32,int32>(r8, 18<i32>)");
        }

        [Test]
        public void CSkyRw_bsr()
        {
            Given_HexString("00E2 0000");
            AssertCode(     // bsr\tFC100000
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call FC100000 (0)");
        }

        [Test]
        public void CSkyRw_bt_16()
        {
            Given_HexString("000A");
            AssertCode(     // bt\t000FFC00
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (C) branch 000FFC00");
        }

        [Test]
        public void CSkyRw_bt_32()
        {
            Given_HexString("60E8 0080");
            AssertCode(     // bt\t000F0000
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (C) branch 000F0000");
        }

        [Test]
        public void CSkyRw_btsti_16()
        {
            Given_HexString("D53F");
            AssertCode(     // btsti\tr7,0x15
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = __bit<word32,int32>(r7, 21<i32>)");
        }

        [Test]
        public void CSkyRw_btsti_32()
        {
            Given_HexString("31C6 8028");
            AssertCode(     // btsti\tr0,r17,0x0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|C = __bit<word32,word32>(r0, r17)");
        }

        [Test]
        public void CSkyRw_clrf()
        {
            Given_HexString("20C6 202C");
            AssertCode(     // clrf
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = !C ? 0<32> : r17");
        }

        [Test]
        public void CSkyRw_clrt()
        {
            Given_HexString("20C6 402C");
            AssertCode(     // clrt
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = C ? 0<32> : r17");
        }

        [Test]
        public void CSkyRw_cmphs()
        {
            Given_HexString("2064");
            AssertCode(     // cmphs\tr8,r0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = r8 >=u r0");
        }

        [Test]
        public void CSkyRw_cmphsi_16()
        {
            Given_HexString("1F3D");
            AssertCode(     // cmphsi\tr5,0x20
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = r5 >=u 0x20<32>");
        }

        [Test]
        public void CSkyRw_cmphsi_32()
        {
            Given_HexString("11EB FFFF");
            AssertCode(     // cmphsi\tr17,0x10000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|C = r17 >=u 0x10000<32>");
        }

        [Test]
        public void CSkyRw_cmplt()
        {
            Given_HexString("2164");
            AssertCode(     // cmplt\tr14,r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = r8 < r0");
        }

        [Test]
        public void CSkyRw_decf()
        {
            Given_HexString("33C6 950C");
            AssertCode(     // decf\tr17,r19,0x15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = !C ? r19 - 0x15<32> : r17");
        }

        [Test]
        public void CSkyRw_decgt()
        {
            Given_HexString("93C4 3710");
            AssertCode(     // decgt\tr23,r19,0x4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r23 = r19 - 4<32>",
                "2|L--|C = r23 > 0<32>");
        }

        [Test]
        public void CSkyRw_declt()
        {
            Given_HexString("93C4 5710");
            AssertCode(     // declt\tr23,r19,0x4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r23 = r19 - 4<32>",
                "2|L--|C = r23 < 0<32>");
        }

        [Test]
        public void CSkyRw_decne()
        {
            Given_HexString("93C4 9710");
            AssertCode(     // decne\tr23,r19,0x4
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r23 = r19 - 4<32>",
                "2|L--|C = r23 != 0<32>");
        }

        [Test]
        public void CSkyRw_dect()
        {
            Given_HexString("33C6 150D");
            AssertCode(     // decf\tr17,r19,0x15
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = C ? r19 - 0x15<32> : r17");
        }

        [Test]
        public void CSkyRw_divs()
        {
            Given_HexString("31C7 5780");
            AssertCode(     // divs\tr23,r17,r25
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r17 / r25");
        }

        [Test]
        public void CSkyRw_divu()
        {
            Given_HexString("31C7 3780");
            AssertCode(     // divu\tr23,r17,r25
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r17 /u r25");
        }

        [Test]
        public void CSkyRw_doze()
        {
            Given_HexString("00C0 2050");
            AssertCode(     // doze
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__doze()");
        }

        [Test]
        public void CSkyRw_ff0()
        {
            Given_HexString("13C4 357C");
            AssertCode(     // ff0\tr21,r19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __ff0(r19)");
        }

        [Test]
        public void CSkyRw_ff1()
        {
            Given_HexString("13C4 557C");
            AssertCode(     // ff1\tr21,r19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __ff1(r19)");
        }

        [Test]
        public void CSkyRw_grs()
        {
            Given_HexString("2ECE 0000");
            AssertCode(     // grs\tr17,r28,000C0000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = 000C0000");
        }

        [Test]
        public void CSkyRw_idly()
        {
            Given_HexString("20C3 201C");
            AssertCode(     // idly\t0x19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__interrupt_delay(0x19<32>)");
        }

        [Test]
        public void CSkyRw_incf()
        {
            Given_HexString("36C6 370C");
            AssertCode(     // incf\tr17,r22,0x17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = !C ? r22 + 0x17<32> : r17");
        }

        [Test]
        public void CSkyRw_inct()
        {
            Given_HexString("36C6 570C");
            AssertCode(     // inct\tr17,r22,0x17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = C ? r22 + 0x17<32> : r17");
        }

        [Test]
        public void CSkyRw_ins()
        {
            Given_HexString("37C6 815C");
            AssertCode(     // ins\tr17,r23,0x5,0x1
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r23, word5, 1)",
                "2|L--|r17 = SEQ(SLICE(r17, word26, 6), v5, SLICE(r17, bool, 0))");
        }

        [Test]
        [Ignore("Manual hard to understand")]
        public void CSkyRw_ipop()
        {
            Given_HexString("6314");
            AssertCode(     // ipop
                  "0|@@@");
        }

        [Test]
        [Ignore("Manual hard to understand")]
        public void CSkyRw_ipush()
        {
            Given_HexString("6214");
            AssertCode(     // ipush
                  "0|@@@");
        }

        [Test]
        public void CSkyRw_ixd()
        {
            Given_HexString("F6C6 9108");
            AssertCode(     // ixd\tr17,r22,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r22 + (r23 << 3<8>)");
        }

        [Test]
        public void CSkyRw_ixh()
        {
            Given_HexString("F6C6 3108");
            AssertCode(     // ixh\tr17,r22,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r22 + (r23 << 1<8>)");
        }

        [Test]
        public void CSkyRw_ixw()
        {
            Given_HexString("F6C6 5108");
            AssertCode(     // ixw\tr17,r22,r23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r22 + (r23 << 2<8>)");
        }

        [Test]
        public void CSkyRw_jmp_16()
        {
            Given_HexString("2478");
            AssertCode(     // jmp\tr9
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto r9");
        }

        [Test]
        public void CSkyRw_jmp_32()
        {
            Given_HexString("D1E8 0000");
            AssertCode(     // jmp\tr17
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r17");
        }

        [Test]
        public void CSkyRw_jmpi()
        {
            Given_HexString("C0EA 8000");
            AssertCode(     // jmpi\t00100100
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[0x00100100<p32>:word32]");
        }

        [Test]
        public void CSkyRw_jmpix_16()
        {
            Given_HexString("E13C");
            AssertCode(     // jmpix\tr4,0x18
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto Mem0[vbr + r4 * 0x18<u32>:ptr32]");
        }

        [Test]
        public void CSkyRw_jmpix_32()
        {
            Given_HexString("F1E90300");
            AssertCode(     // jmpix\tr17,0x28
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto Mem0[vbr + r17 * 0x28<u32>:ptr32]");
        }

        [Test]
        public void CSkyRw_jsr_16()
        {
            Given_HexString("F17B");
            AssertCode(     // jsr\tr12
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r12 (0)");
        }

        [Test]
        public void CSkyRw_jsr_32()
        {
            Given_HexString("E1E8 0000");
            AssertCode(     // jsr\tr1
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call r1 (0)");
        }

        [Test]
        public void CSkyRw_jsri()
        {
            Given_HexString("E0EA 8000");
            AssertCode(     // jsri\t00100100
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call Mem0[0x00100100<p32>:ptr32] (0)");
        }

        [Test]
        public void CSkyRw_ld_b_16()
        {
            Given_HexString("B084");
            AssertCode(     // ld.b\tr5,(r4,16)
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = Mem0[r4 + 16<i32>:byte]",
                "2|L--|r5 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void CSkyRw_ld_b_32()
        {
            Given_HexString("31DA 0008");
            AssertCode(     // ld.b\tr17,(r17,2048)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r17 + 2048<i32>:byte]",
                "2|L--|r17 = CONVERT(v4, byte, word32)");
        }

        [Test]
        public void CSkyRw_ld_bs()
        {
            Given_HexString("31DA 0048");
            AssertCode(     // ld.bs\tr17,(r17,2048)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r17 + 2048<i32>:int8]",
                "2|L--|r17 = CONVERT(v4, int8, int32)");
        }

        [Test]
        public void CSkyRw_ld_d()
        {
            Given_HexString("31DA 0038");
            AssertCode(     // ld.d\tr17,(r17,16384)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18_r17 = Mem0[r17 + 16384<i32>:word64]");
        }

        [Test]
        public void CSkyRw_ld_h_16()
        {
            Given_HexString("B08C");
            AssertCode(     // ld.h\tr5,(r4,32)
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = Mem0[r4 + 32<i32>:word16]",
                "2|L--|r5 = CONVERT(v5, word16, word32)");
        }

        [Test]
        public void CSkyRw_ld_h_32()
        {
            Given_HexString("31DA 0018");
            AssertCode(     // ld.h\tr17,(r17,4096)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r17 + 4096<i32>:word16]",
                "2|L--|r17 = CONVERT(v4, word16, word32)");
        }

        [Test]
        public void CSkyRw_ld_hs_32()
        {
            Given_HexString("31DA 0058");
            AssertCode(     // ld.hs\tr17,(r17,4096)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = Mem0[r17 + 4096<i32>:int16]",
                "2|L--|r17 = CONVERT(v4, int16, int32)");
        }

        [Test]
        public void CSkyRw_ld_w_16a()
        {
            Given_HexString("B094");
            AssertCode(     // ld.w\tr5,(r4,64)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = Mem0[r4 + 64<i32>:word32]");
        }

        [Test]
        public void CSkyRw_ld_w_16b()
        {
            Given_HexString("B09C");
            AssertCode(     // ld.w\tr5,(r14,16)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r5 = Mem0[r14 + 64<i32>:word32]");
        }

        [Test]
        public void CSkyRw_ld_w_32()
        {
            Given_HexString("31DA 0028");
            AssertCode(     // ld.w\tr17,(r17,8192)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = Mem0[r17 + 8192<i32>:word32]");
        }

        [Test]
        public void CSkyRw_ldex_w()
        {
            Given_HexString("31DA 0078");
            AssertCode(     // ldex.w\tr17,(r17,8192)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = __ldex<word32>(&Mem0[r17 + 8192<i32>:word32])");
        }

        [Test]
        [Ignore("Needs registerrange")]
        public void CSkyRw_ldm()
        {
            Given_HexString("37D2 351C");
            AssertCode(     // ld.hs
                  "0|@@@");
        }

        [Test]
        public void CSkyRw_ldr_b_a()
        {
            Given_HexString("F3D2 3100");
            AssertCode(     // ldr.b\tr17,(r19,r23)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23:byte]",
                "2|L--|r17 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_ldr_b_b()
        {
            Given_HexString("F3D2 5100");
            AssertCode(     // ldr.b\tr17,(r19,r23<<1)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23 * 2<32>:byte]",
                "2|L--|r17 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_ldr_b_c()
        {
            Given_HexString("F3D2 9100");
            AssertCode(     // ldr.b\tr17,(r19,r23<<2)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23 * 4<32>:byte]",
                "2|L--|r17 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_ldr_b_d()
        {
            Given_HexString("F3D2 1101");
            AssertCode(     // ldr.b\tr17,(r19,r23<<3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23 * 8<32>:byte]",
                "2|L--|r17 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_ldr_bs()
        {
            Given_HexString("F3D2 1111");
            AssertCode(     // ldr.bs\tr17,(r19,r23<<3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23 * 8<32>:int8]",
                "2|L--|r17 = CONVERT(v6, int8, int32)");
        }

        [Test]
        public void CSkyRw_ldr_h()
        {
            Given_HexString("F3D2 1105");
            AssertCode(     // ldr.h\tr17,(r19,r23<<3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23 * 8<32>:word16]",
                "2|L--|r17 = CONVERT(v6, word16, word32)");
        }

        [Test]
        public void CSkyRw_ldr_hs()
        {
            Given_HexString("F3D2 1115");
            AssertCode(     // ldr.hs\tr17,(r19,r23<<3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r19 + r23 * 8<32>:int16]",
                "2|L--|r17 = CONVERT(v6, int16, int32)");
        }

        [Test]
        public void CSkyRw_ldr_w()
        {
            Given_HexString("F3D2 1109");
            AssertCode(     // ldr.w\tr17,(r19,r23<<3)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = Mem0[r19 + r23 * 8<32>:word32]");
        }

        [Test]
        public void CSkyRw_lrs_b()
        {
            Given_HexString("22CE 0000");
            AssertCode(     // lrs.b\tr17,r28,000E0000
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r28 + 0x000E0000<p32>:byte]",
                "2|L--|r17 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void CSkyRw_lrs_h()
        {
            Given_HexString("26CE 0000");
            AssertCode(     // lrs.h\tr17,r28,000C0000
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r28 + 0x000C0000<p32>:word16]",
                "2|L--|r17 = CONVERT(v5, word16, word32)");
        }

        [Test]
        public void CSkyRw_lrs_w()
        {
            Given_HexString("2ACE 0000");
            AssertCode(     // lrs.w\tr17,r28,00080000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = Mem0[r28 + 0x00080000<p32>:word32]");
        }

        [Test]
        public void CSkyRw_lrw()
        {
            Given_HexString("2C11");
            AssertCode(     // lrw\tr1,(001003CC)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = Mem0[0x0010034C<p32>:word32]");
        }

        [Test]
        public void CSkyRw_lrw_32()
        {
            Given_HexString("88EAC000");
            AssertCode(        // lrw
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r8 = Mem0[0x00100300<p32>:word32]");
        }

        [Test]
        public void CSkyRw_lsl_16()
        {
            Given_HexString("D072");
            AssertCode(     // lsl\tr11,r4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r11 = r11 << r4");
        }

        [Test]
        public void CSkyRw_lsl_32()
        {
            Given_HexString("33C6 3740");
            AssertCode(     // lsl\tr23,r19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r19 << r17");
        }

        [Test]
        public void CSkyRw_lslc()
        {
            Given_HexString("33C6 374C");
            AssertCode(     // lslc\tr23,r19,0x12
                "0|L--|00100000(4): 2 instructions",
                "1|L--|C = (r19 & 0x20000<32>) != 0<32>",
                "2|L--|r23 = r19 >>u 18<i32>");
        }

        [Test]
        public void CSkyRw_lsli_16()
        {
            Given_HexString("FF46");
            AssertCode(     // lsli\tr7,r6,0x20
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = r6 << 31<i32>");
        }

        [Test]
        public void CSkyRw_lsli_32()
        {
            Given_HexString("33C6 3748");
            AssertCode(     // lsli\tr23,r19,0x12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r19 << 17<i32>");
        }

        [Test]
        public void CSkyRw_lsr_16()
        {
            Given_HexString("D172");
            AssertCode(     // lsr\tr11,r4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r11 = r11 >>u r4");
        }

        [Test]
        public void CSkyRw_lsr_32()
        {
            Given_HexString("33C6 5740");
            AssertCode(     // lsr\tr23,r19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r19 >>u r17");
        }

        [Test]
        public void CSkyRw_lsrc_32()
        {
            Given_HexString("33C6 574C");
            AssertCode(     // lsrc\tr23,r19,0x12
                "0|L--|00100000(4): 2 instructions",
                "1|L--|C = (r19 & 0x20000<32>) != 0<32>",
                "2|L--|r23 = r19 >>u 18<i32>");
        }

        [Test]
        public void CSkyRw_lsri_16()
        {
            Given_HexString("FF4E");
            AssertCode(     // lsri\tr7,r6,0x20
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = r6 >>u 31<i32>");
        }

        [Test]
        public void CSkyRw_lsri_32()
        {
            Given_HexString("33C6 5748");
            AssertCode(     // lsri\tr23,r19,0x12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r19 >>u 17<i32>");
        }

        [Test]
        public void CSkyRw_mfcr()
        {
            Given_HexString("3FC3 3760");
            AssertCode(     // mfcr\tr23,cr831
                "0|S--|00100000(4): 1 instructions",
                "1|L--|r23 = __read_control_register(cr831)");
        }

        [Test]
        public void CSkyRw_mfhi()
        {
            Given_HexString("00C4 379C");
            AssertCode(     // mfhi\tr23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = hi");
        }

        [Test]
        public void CSkyRw_mfhis()
        {
            Given_HexString("00C4 3798");
            AssertCode(     // mfhis\tr23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = __mfhis()");
        }

        [Test]
        public void CSkyRw_mflo()
        {
            Given_HexString("00C4 979C");
            AssertCode(     // mflo\tr23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = lo");
        }

        [Test]
        public void CSkyRw_mflos()
        {
            Given_HexString("00C4 9798");
            AssertCode(     // mflos\tr23
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = __mflos()");
        }

        [Test]
        public void CSkyRw_mov_16()
        {
            Given_HexString("9F6F");
            AssertCode(     // mov\tr14,r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r7");
        }

        [Test]
        public void CSkyRw_mov_32()
        {
            Given_HexString("13C4 3F48");
            AssertCode(     // mov\tr31,r19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r31 = r19");
        }

        /*
        [Test]
        public void CSkyRw_movf()
        {
            Given_HexString("3FC6 200C");
            AssertCode(     // mov\tr14,r7
                  "0|@@@");
        }
        */

        [Test]
        public void CSkyRw_movi_16()
        {
            Given_HexString("FF37");
            AssertCode(     // movi\tr7,0xFF
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = 0xFF<32>");
        }

        [Test]
        public void CSkyRw_movi_32()
        {
            Given_HexString("17EA FFFF");
            AssertCode(     // movi\tr23,0xFFFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = 0xFFFF<32>");
        }

        [Test]
        public void CSkyRw_mov()
        {
            Given_HexString("9F6F");
            AssertCode(     // mov\tr14,r7
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r7");
        }

        [Test]
        public void CSkyRw_movi()
        {
            Given_HexString("FF37");
            AssertCode(     // movi\tr7,0xFF
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = 0xFF<32>");
        }

        [Test]
        public void CSkyRw_movih()
        {
            Given_HexString("37EA FFFF");
            AssertCode(     // movih\tr23,0xFFFF
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = 0xFFFF0000<32>");
        }

        //[Test]
        //public void CSkyRw_movt()
        //{
        //    Given_HexString("3FC6 400C");
        //    AssertCode(     // mov\tr14,r7
        //        "0|@@@");
        //}

        [Test]
        public void CSkyRw_mtcr()
        {
            Given_HexString("37C2 3F64");
            AssertCode(     // mtcr\tcr567,r31
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__write_control_register(r31, cr567)");
        }

        [Test]
        public void CSkyRw_mthi()
        {
            Given_HexString("33C6 409C");
            AssertCode(     // mthi\tr0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|hi = r0");
        }

        [Test]
        public void CSkyRw_mtlo()
        {
            Given_HexString("33C6 009D");
            AssertCode(     // mtlo\tr0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|lo = r0");
        }

        [Test]
        public void CSkyRw_muls()
        {
            Given_HexString("34C6 208C");
            AssertCode(     // muls\tr20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|macc = r20 *s64 r17");
        }

        [Test]
        public void CSkyRw_mulsa()
        {
            Given_HexString("34C6 408C");
            AssertCode(     // mulsa\tr20,r17
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = r20 *s64 r17",
                "2|L--|macc = macc + v6");
        }

        [Test]
        public void CSkyRw_mulsh_16()
        {
            Given_HexString("0D7F");
            AssertCode(     // mulsh\tr12,r3
                "0|L--|00100000(2): 3 instructions",
                "1|L--|v5 = SLICE(r12, word16, 0)",
                "2|L--|v6 = SLICE(r3, word16, 0)",
                "3|L--|r12 = v5 *s32 v6");
        }

        [Test]
        public void CSkyRw_mulsh_32()
        {
            Given_HexString("34C6 3F90");
            AssertCode(     // mulsh\tr31,r20,r17
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v6 = SLICE(r20, word16, 0)",
                "2|L--|v7 = SLICE(r17, word16, 0)",
                "3|L--|r31 = v6 *s32 v7");
        }

        [Test]
        public void CSkyRw_mulsha()
        {
            Given_HexString("34C6 4090");
            AssertCode(     // mulsha\tr20,r17
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = SLICE(r20, word16, 0)",
                "2|L--|v6 = SLICE(r17, word16, 0)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|r20 = r20 + v7");
        }

        [Test]
        public void CSkyRw_mulshs()
        {
            Given_HexString("34C6 8090");
            AssertCode(     // mulshs\tr20,r17
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = SLICE(r20, word16, 0)",
                "2|L--|v6 = SLICE(r17, word16, 0)",
                "3|L--|v7 = v5 *s32 v6",
                "4|L--|r20 = r20 - v7");
        }

        [Test]
        public void CSkyRw_mulss()
        {
            Given_HexString("34C6 808C");
            AssertCode(     // mulss\tr20,r17
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = r20 *s64 r17",
                "2|L--|macc = macc - v6");
        }

        [Test]
        public void CSkyRw_mulswa()
        {
            Given_HexString("34C6 8094");
            AssertCode(     // mulswa\tr20,r17
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = SLICE(r20, word16, 0)",
                "2|L--|v6 = SLICE(r17, word16, 0)",
                "3|L--|v7 = SLICE(v5 *s48 v6, word32, 16)",
                "4|L--|r20 = r20 + v7");
        }

        [Test]
        public void CSkyRw_mulsws()
        {
            Given_HexString("34C6 0095");
            AssertCode(     // mulsws\tr20,r17
                "0|L--|00100000(4): 4 instructions",
                "1|L--|v5 = SLICE(r20, word16, 0)",
                "2|L--|v6 = SLICE(r17, word16, 0)",
                "3|L--|v7 = SLICE(v5 *s48 v6, word32, 16)",
                "4|L--|r20 = r20 - v7");
        }

        [Test]
        public void CSkyRw_mult_16()
        {
            Given_HexString("087F");
            AssertCode(     // mult\tr12,r2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r12 = r12 * r2");
        }

        [Test]
        public void CSkyRw_mult_32()
        {
            Given_HexString("34C6 3784");
            AssertCode(     // mult\tr23,r20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r20 * r17");
        }

        [Test]
        public void CSkyRw_mulu()
        {
            Given_HexString("34C6 2088");
            AssertCode(     // mulu\tr20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|macc = r20 *u64 r17");
        }

        [Test]
        public void CSkyRw_mulua()
        {
            Given_HexString("34C6 4088");
            AssertCode(     // mulua\tr20,r17
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = r20 *u64 r17",
                "2|L--|macc = macc + v6");
        }

        [Test]
        public void CSkyRw_mulus()
        {
            Given_HexString("34C6 8088");
            AssertCode(     // mulus\tr20,r17
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = r20 *u64 r17",
                "2|L--|macc = macc - v6");
        }

        [Test]
        public void CSkyRw_mvc()
        {
            Given_HexString("00C4 1505");
            AssertCode(     // mvc\tr21
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = C != 0<32>",
                "2|L--|r21 = CONVERT(v5, bool, word32)");
        }

        [Test]
        public void CSkyRw_mvcv_16()
        {
            Given_HexString("0367");
            AssertCode(     // mvcv\tr12
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r12 = CONVERT(!C, bool, word32)");
        }

        [Test]
        public void CSkyRw_mvcv_32()
        {
            Given_HexString("00C4 1506");
            AssertCode(     // mvcv\tr21
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = CONVERT(!C, bool, word32)");
        }

        [Test]
        public void CSkyRw_mvtc()
        {
            Given_HexString("00C4 009A");
            AssertCode(     // mvtc\tr0
                "0|L--|00100000(4): 1 instructions",
                "1|L--|C = V");
        }

        [Test]
        public void CSkyRw_nie()
        {
            Given_HexString("6014");
            AssertCode(     // nie
                "0|S--|00100000(2): 1 instructions",
                "1|L--|__nie()");
        }

        [Test]
        public void CSkyRw_nir()
        {
            Given_HexString("6114");
            AssertCode(     // nir
                "0|R--|00100000(2): 2 instructions",
                "1|L--|__interrup_nesting_return()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void CSkyRw_nor_16()
        {
            Given_HexString("066C");
            AssertCode(     // nor\tr0,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = ~(r0 | r1)");
        }

        [Test]
        public void CSkyRw_nor_32()
        {
            Given_HexString("34C6 9724");
            AssertCode(     // nor\tr23,r20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = ~(r20 | r17)");
        }

        [Test]
        public void CSkyRw_or_16()
        {
            Given_HexString("246E");
            AssertCode(     // or\tr8,r9
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r8 = r8 | r9");
        }

        [Test]
        public void CSkyRw_or_32()
        {
            Given_HexString("34C6 3724");
            AssertCode(     // or\tr23,r20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r20 | r17");
        }

        [Test]
        public void CSkyRw_ori()
        {
            Given_HexString("34EE 0080");
            AssertCode(     // ori\tr17,r20,0x8000
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r20 | 0x8000<32>");
        }

        [Test]
        public void CSkyRw_pldr()
        {
            Given_HexString("13D8 0168");
            AssertCode(     // pldr\t(r19,8196)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__pldr(&Mem0[r19 + 8196<i32>:word32])");
        }

        [Test]
        public void CSkyRw_pldw()
        {
            Given_HexString("13DC 0168");
            AssertCode(     // pldrw\t(r19,8196)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__pldrw(&Mem0[r19 + 8196<i32>:word32])");
        }

        [Test]
        public void CSkyRw_pop_16()
        {
            Given_HexString("9314");
            AssertCode(     // pop\tr4-r6,r15", 
                "0|L--|00100000(2): 9 instructions",
                "1|L--|r4 = Mem0[r14:word32]",
                "2|L--|r14 = r14 + 4<i32>",
                "3|L--|r5 = Mem0[r14:word32]",
                "4|L--|r14 = r14 + 4<i32>",
                "5|L--|r6 = Mem0[r14:word32]",
                "6|L--|r14 = r14 + 4<i32>",
                "7|L--|r15 = Mem0[r14:word32]",
                "8|L--|r14 = r14 + 4<i32>",
                "9|R--|return (0,0)");
        }

        [Test]
        public void CSkyRw_psrclr()
        {
            Given_HexString("80C02070");
            AssertCode(     // psrclr\t0x4
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__psrclr(4<32>)");
        }

        [Test]
        public void CSkyRw_push_16()
        {
            Given_HexString("D314");
            AssertCode(     // push\tr4-r6,r15",
                "0|L--|00100000(2): 8 instructions",
                "1|L--|r14 = r14 - 4<i32>",
                "2|L--|Mem0[r14:word32] = r15",
                "3|L--|r14 = r14 - 4<i32>",
                "4|L--|Mem0[r14:word32] = r6",
                "5|L--|r14 = r14 - 4<i32>",
                "6|L--|Mem0[r14:word32] = r5",
                "7|L--|r14 = r14 - 4<i32>",
                "8|L--|Mem0[r14:word32] = r4");
        }

        [Test]
        public void CSkyRw_revb_16()
        {
            Given_HexString("067B");
            AssertCode(     // revb\tr12,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r12 = __reverse_bytes<word32>(r1)");
        }

        [Test]
        public void CSkyRw_revb_32()
        {
            Given_HexString("11C4 9460");
            AssertCode(     // revb\tr20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r20 = __reverse_bytes<word32>(r17)");
        }

        [Test]
        public void CSkyRw_revh_16()
        {
            Given_HexString("077B");
            AssertCode(     // revh\tr12,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r12 = __reverse_word16s<word32>(r1)");
        }

        [Test]
        public void CSkyRw_revh_32()
        {
            Given_HexString("11C4 1461");
            AssertCode(     // revh\tr20,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r20 = __reverse_word16s<word32>(r17)");
        }

        [Test]
        public void CSkyRw_rfi()
        {
            Given_HexString("00C0 2044");
            AssertCode(     // rfi
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__rfi()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void CSkyRw_rotl_16()
        {
            Given_HexString("2772");
            AssertCode(     // rotl\tr8,r9
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r8 = __rol<word32,word32>(r8, r9)");
        }

        [Test]
        public void CSkyRw_rotl_32()
        {
            Given_HexString("33C6 1741");
            AssertCode(     // rotl\tr23,r19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = __rol<word32,word32>(r19, r17)");
        }

        [Test]
        public void CSkyRw_rotli()
        {
            Given_HexString("33C6 1749");
            AssertCode(     // rotli\tr23,r19,0x12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = __rol<word32,int32>(r19, 17<i32>)");
        }

        [Test]
        public void CSkyRw_rte()
        {
            Given_HexString("00C0 2040");
            AssertCode(     // rte
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__rte()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void CSkyRw_sce()
        {
            Given_HexString(
                "A0C0 2018" +   // sce\t0x5 
                "D463" +        // addu\tr15,r5
                "D663" +        // addu\tr15,r5
                "D461" +        // addu\tr15,r5
                "D661" +        // addu\tr15,r5
                "D661");        // addu\tr15,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|nop",
                "2|L--|00100004(2): 2 instructions",
                "3|T--|if (C) branch 00100006",
                "4|L--|r15 = r15 + r5",
                "5|L--|00100006(2): 2 instructions",
                "6|T--|if (!C) branch 00100008",
                "7|L--|r15 = r15 - r5",
                "8|L--|00100008(2): 2 instructions",
                "9|T--|if (C) branch 0010000A",
                "10|L--|r7 = r7 + r5",
                "11|L--|0010000A(2): 2 instructions",
                "12|T--|if (!C) branch 0010000C",
                "13|L--|r7 = r7 - r5",
                "14|L--|0010000C(2): 1 instructions",
                "15|L--|r7 = r7 - r5");
        }

        [Test]
        public void CSkyRw_se()
        {
            Given_HexString("00C0 2058");
            AssertCode(     // se
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__se()");
        }

        [Test]
        public void CSkyRw_sext()
        {
            Given_HexString("33C4 F25B");
            AssertCode(     // sext\tr1,r19,0x1,0x1F
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r19, word31, 0)",
                "2|L--|r1 = CONVERT(v5, word31, int32)");
        }

        [Test]
        public void CSkyRw_sextb()
        {
            Given_HexString("0677");
            AssertCode(     // sextb\tr12,r1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = SLICE(r1, byte, 0)",
                "2|L--|r12 = CONVERT(v5, byte, int32)");
        }

        [Test]
        public void CSkyRw_sexth()
        {
            Given_HexString("0777");
            AssertCode(     // sexth\tr12,r1
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = SLICE(r1, word16, 0)",
                "2|L--|r12 = CONVERT(v5, word16, int32)");
        }

        [Test]
        public void CSkyRw_srs_b()
        {
            Given_HexString("12CF 0100");
            AssertCode(     // srs.b\tr24,r28,000E0001
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r24, byte, 0)",
                "2|L--|Mem0[r28 + 0x000E0001<p32>:byte] = v5");
        }

        [Test]
        public void CSkyRw_srs_h()
        {
            Given_HexString("16CF 0100");
            AssertCode(     // srs.h\tr24,r28,000C0002
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r24, word16, 0)",
                "2|L--|Mem0[r28 + 0x000C0002<p32>:word16] = v5");
        }

        [Test]
        public void CSkyRw_srs_w()
        {
            Given_HexString("1ACF 0100");
            AssertCode(     // srs.w\tr24,r28,00080004
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r28 + 0x00080004<p32>:word32] = r24");
        }

        [Test]
        public void CSkyRw_srte()
        {
            Given_HexString("00C0 207C");
            AssertCode(     // srte
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__srte()",
                "2|R--|return (0,0)");
        }

        [Test]
        public void CSkyRw_st_b_16()
        {
            Given_HexString("5FA6");
            AssertCode(     // st.b\tr2,(r6,31)
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = SLICE(r2, byte, 0)",
                "2|L--|Mem0[r6 + 31<i32>:byte] = v5");
        }

        [Test]
        public void CSkyRw_st_b_32()
        {
            Given_HexString("33DE 0108");
            AssertCode(     // st.b\tr17,(r19,2049)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r17, byte, 0)",
                "2|L--|Mem0[r19 + 2049<i32>:byte] = v5");
        }

        [Test]
        public void CSkyRw_st_d_32()
        {
            Given_HexString("33DE 0138");
            AssertCode(     // st.d\tr17,(r19,16392)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r19 + 16392<i32>:word64] = r18_r17");
        }

        [Test]
        public void CSkyRw_st_h_16()
        {
            Given_HexString("5FAC");
            AssertCode(     // st.h\tr2,(r4,31)
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = SLICE(r2, word16, 0)",
                "2|L--|Mem0[r4 + 31<i32>:word16] = v5");
        }

        [Test]
        public void CSkyRw_st_h_32()
        {
            Given_HexString("33DE 0118");
            AssertCode(     // st.h\tr17,(r19,4098)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r17, word16, 0)",
                "2|L--|Mem0[r19 + 4098<i32>:word16] = v5");
        }

        [Test]
        public void CSkyRw_st_w_16()
        {
            Given_HexString("5FB6");
            AssertCode(     // st.w\tr2,(r6,124)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r6 + 124<i32>:word32] = r2");
        }

        [Test]
        public void CSkyRw_st_w_sp()
        {
            Given_HexString("5FBE");
            AssertCode(     // st.w\tr2,(r14,380)
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r14 + 380<i32>:word32] = r2");
        }

        [Test]
        public void CSkyRw_st_w_32()
        {
            Given_HexString("33DE 0128");
            AssertCode(     // st.w\tr17,(r19,8196)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r19 + 8196<i32>:word32] = r17");
        }

        [Test]
        public void CSkyRw_stex_w()
        {
            Given_HexString("33DE 0178");
            AssertCode(     // stex.w\tr17,(r19,8196)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__stex_w<word32>(r17, &Mem0[r19 + 8196<i32>:word32])");
        }

        [Test]
        public void CSkyRw_stop()
        {
            Given_HexString("00C0 2048");
            AssertCode(     // stop
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__stop()");
        }

        [Test]
        public void CSkyRw_str_b_a()
        {
            Given_HexString("33D6 3500");
            AssertCode(     // str.b\tr21,(r19,r17)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = SLICE(r21, byte, 0)",
                "2|L--|Mem0[r19 + r17:byte] = v6");
        }

        [Test]
        public void CSkyRw_str_b_b()
        {
            Given_HexString("33D6 5500");
            AssertCode(     // str.b\tr21,(r19,r17<<1)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = SLICE(r21, byte, 0)",
                "2|L--|Mem0[r19 + r17 * 2<32>:byte] = v6");
        }

        [Test]
        public void CSkyRw_str_b_c()
        {
            Given_HexString("33D6 9500");
            AssertCode(     // str.b\tr21,(r19,r17<<2)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = SLICE(r21, byte, 0)",
                "2|L--|Mem0[r19 + r17 * 4<32>:byte] = v6");
        }

        [Test]
        public void CSkyRw_str_b_d()
        {
            Given_HexString("33D6 1501");
            AssertCode(     // str.b\tr21,(r19,r17<<3)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = SLICE(r21, byte, 0)",
                "2|L--|Mem0[r19 + r17 * 8<32>:byte] = v6");
        }

        [Test]
        public void CSkyRw_str_h()
        {
            Given_HexString("33D6 3504");
            AssertCode(     // str.h\tr21,(r19,r17)
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v6 = SLICE(r21, word16, 0)",
                "2|L--|Mem0[r19 + r17:word16] = v6");
        }

        [Test]
        public void CSkyRw_str_w()
        {
            Given_HexString("33D6 3508");
            AssertCode(     // str.w\tr21,(r19,r17)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r19 + r17:word32] = r21");
        }

        [Test]
        public void CSkyRw_strap()
        {
            Given_HexString("00C0 2078");
            AssertCode(     // strap\tcr0,r0
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__strap()");
        }

        [Test]
        public void CSkyRw_subc_16()
        {
            Given_HexString("2762");
            AssertCode(     // subc\tr8,r9
                "0|L--|00100000(2): 2 instructions",
                "1|L--|r8 = r8 - r9 - C",
                "2|L--|C = cond(r8)");
        }

        [Test]
        public void CSkyRw_subc_32()
        {
            Given_HexString("33C6 1701");
            AssertCode(     // subc\tr23,r19,r17
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r23 = r19 - r17 - C",
                "2|L--|C = cond(r23)");
        }

        [Test]
        public void CSkyRw_subi_16()
        {
            Given_HexString("002A");
            AssertCode(     // subi\tr0,0x1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 - 1<32>");
            Given_HexString("012A");
            AssertCode(     // subi\tr0,0x2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 - 2<32>");
            Given_HexString("FF2A");
            AssertCode(     // subi\tr7,0x100
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r7 = r7 - 0x100<32>");
        }

        [Test]
        public void CSkyRw_subi_16_a()
        {
            Given_HexString("635A");
            AssertCode(     // subi\tr3,r2,0x1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r2 - 1<32>");
            Given_HexString("675A");
            AssertCode(     // subi\tr3,r2,0x2
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r2 - 2<32>");
            Given_HexString("7F5A");
            AssertCode(     // subi\tr3,r2,0x8
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r2 - 8<32>");
        }

        [Test]
        public void CSkyRw_subi_32()
        {
            Given_HexString("B5E60018");
            AssertCode(     // nyi
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = r21 - 0x800<32>");
        }

        [Test]
        public void CSkyRw_subi_sp()
        {
            Given_HexString("2014");
            AssertCode(     // subi\tr14,r14,0x0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r14 - 0<32>");
            Given_HexString("2114");
            AssertCode(     // subi\tr14,r14,0x4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r14 - 4<32>");
            Given_HexString("2115");
            AssertCode(     // subi\tr14,r14,0x84
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r14 - 0x84<32>");
            Given_HexString("3F17");
            AssertCode(     // subi\tr14,r14,0x1FC
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r14 - 0x1FC<32>");
        }

        [Test]
        public void CSkyRw_subu_16_a()
        {
            Given_HexString("0660");
            AssertCode(     // subu\tr0,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 - r1");
        }

        [Test]
        public void CSkyRw_subu_16_b()
        {
            Given_HexString("315B");
            AssertCode(     // subu\tr1,r3,r4
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r3 - r4");
        }

        [Test]
        public void CSkyRw_subu_32()
        {
            Given_HexString("33C6 9700");
            AssertCode(     // subu\tr23,r19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r19 - r17");
        }

        [Test]
        public void CSkyRw_sync()
        {
            Given_HexString("20C0 2004");
            AssertCode(     // sync\t0x1
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__sync()");
        }

        [Test]
        public void CSkyRw_trap()
        {
            Given_HexString("00C0 2024");
            AssertCode(     // trap\t0x1
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__syscall<word32>(1<32>)");
            Given_HexString("00C0 202C");
            AssertCode(     // trap\t0x3
                "0|T--|00100000(4): 1 instructions",
                "1|L--|__syscall<word32>(3<32>)");
        }

        [Test]
        public void CSkyRw_tst_16()
        {
            Given_HexString("026B");
            AssertCode(     // tst\tr12,r0
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = (r12 & r0) != 0<32>");
        }

        [Test]
        public void CSkyRw_tst_32()
        {
            Given_HexString("33C6 8020");
            AssertCode(     // tst\tr19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|C = (r19 & r17) != 0<32>");
        }

        [Test]
        public void CSkyRw_tstnbz_16()
        {
            Given_HexString("0768");
            AssertCode(     // tstnbz\tr1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|C = __tstnbz(r1)");
        }

        [Test]
        public void CSkyRw_tstnbz_32()
        {
            Given_HexString("13C4 0021");
            AssertCode(     // tstnbz\tr19
                "0|L--|00100000(4): 1 instructions",
                "1|L--|C = __tstnbz(r19)");
        }

        [Test]
        public void CSkyRw_vmulsh()
        {
            Given_HexString("33C6 20B0");
            AssertCode(     // vmulsh\tr19,r17
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v5 = SLICE(r19, word16, 16)",
                "2|L--|v6 = SLICE(r19, word16, 0)",
                "3|L--|v7 = SLICE(r17, word16, 16)",
                "4|L--|v8 = SLICE(r17, word16, 0)",
                "5|L--|hi = v5 *s32 v7",
                "6|L--|lo = v6 *s32 v8");
        }

        [Test]
        public void CSkyRw_vmulsha()
        {
            Given_HexString("33C6 40B0");
            AssertCode(     // vmulsha\tr19,r17
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v5 = SLICE(r19, word16, 16)",
                "2|L--|v6 = SLICE(r19, word16, 0)",
                "3|L--|v7 = SLICE(r17, word16, 16)",
                "4|L--|v8 = SLICE(r17, word16, 0)",
                "5|L--|hi = hi + v5 *s32 v7",
                "6|L--|lo = lo + v6 *s32 v8");
        }

        [Test]
        public void CSkyRw_vmulshs()
        {
            Given_HexString("33C6 80B0");
            AssertCode(     // vmulshs\tr19,r17
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v5 = SLICE(r19, word16, 16)",
                "2|L--|v6 = SLICE(r19, word16, 0)",
                "3|L--|v7 = SLICE(r17, word16, 16)",
                "4|L--|v8 = SLICE(r17, word16, 0)",
                "5|L--|hi = hi - v5 *s32 v7",
                "6|L--|lo = lo - v6 *s32 v8");
        }

        [Test]
        public void CSkyRw_vmulsw()
        {
            Given_HexString("33C6 20B4");
            AssertCode(     // vmulsw\tr19,r17
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v5 = SLICE(r19, word16, 16)",
                "2|L--|v6 = SLICE(r19, word16, 0)",
                "3|L--|v7 = SLICE(r17, word16, 16)",
                "4|L--|v8 = SLICE(r17, word16, 0)",
                "5|L--|hi = SLICE(v5 *s48 v7, word32, 16)",
                "6|L--|lo = SLICE(v6 *s48 v8, word32, 16)");
        }

        [Test]
        public void CSkyRw_vmulswa()
        {
            Given_HexString("33C6 40B4");
            AssertCode(     // vmulswa\tr19,r17
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v5 = SLICE(r19, word16, 16)",
                "2|L--|v6 = SLICE(r19, word16, 0)",
                "3|L--|v7 = SLICE(r17, word16, 16)",
                "4|L--|v8 = SLICE(r17, word16, 0)",
                "5|L--|hi = hi + SLICE(v5 *s48 v7, word32, 16)",
                "6|L--|lo = lo + SLICE(v6 *s48 v8, word32, 16)");
        }

        [Test]
        public void CSkyRw_vmulsws()
        {
            Given_HexString("33C6 80B4");
            AssertCode(     // vmulsws\tr19,r17
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v5 = SLICE(r19, word16, 16)",
                "2|L--|v6 = SLICE(r19, word16, 0)",
                "3|L--|v7 = SLICE(r17, word16, 16)",
                "4|L--|v8 = SLICE(r17, word16, 0)",
                "5|L--|hi = hi - SLICE(v5 *s48 v7, word32, 16)",
                "6|L--|lo = lo - SLICE(v6 *s48 v8, word32, 16)");
        }

        [Test]
        public void CSkyRw_wait()
        {
            Given_HexString("00C0 204C");
            AssertCode(     // wait
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__wait()");
        }

        [Test]
        public void CSkyRw_we()
        {
            Given_HexString("00C0 2054");
            AssertCode(     // we
                "0|S--|00100000(4): 1 instructions",
                "1|L--|__we()");
        }

        [Test]
        public void CSkyRw_xor_16()
        {
            Given_HexString("056C");
            AssertCode(     // xor\tr0,r1
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 ^ r1");
        }

        [Test]
        public void CSkyRw_xor_32()
        {
            Given_HexString("33C6 5724");
            AssertCode(     // xor\tr23,r19,r17
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r23 = r19 ^ r17");
        }

        [Test]
        public void CSkyRw_xori()
        {
            Given_HexString("33E6 0148");
            AssertCode(     // xori\tr17,r19,0x801
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r17 = r19 ^ 0x801<32>");
        }

        [Test]
        public void CSkyRw_xsr()
        {
            Given_HexString("33C6 034D");
            AssertCode(     // xsr\tr3,r19,0x12
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = __xsr(r19, 18<i32>)");
        }

        [Test]
        public void CSkyRw_xtb0()
        {
            Given_HexString("13C4 3570");
            AssertCode(     // xtb0\tr21,r19
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r19, byte, 0)",
                "2|L--|v6 = v4",
                "3|L--|r21 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_xtb1()
        {
            Given_HexString("13C4 5570");
            AssertCode(     // xtb1\tr21,r19
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r19, byte, 8)",
                "2|L--|v6 = v4",
                "3|L--|r21 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_xtb2()
        {
            Given_HexString("13C4 9570");
            AssertCode(     // xtb2\tr21,r19
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r19, byte, 16)",
                "2|L--|v6 = v4",
                "3|L--|r21 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_xtb3()
        {
            Given_HexString("13C4 1571");
            AssertCode(     // xtb3\tr21,r19
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r19, byte, 24)",
                "2|L--|v6 = v4",
                "3|L--|r21 = CONVERT(v6, byte, word32)");
        }

        [Test]
        public void CSkyRw_zext()
        {
            Given_HexString("13C6 9557");
            AssertCode(     // zext\tr16,r19,0x10,0x1C
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = SLICE(r19, word13, 0)",
                "2|L--|r16 = CONVERT(v5, word13, word32)");
        }

        [Test]
        public void CSkyRw_zextb()
        {
            Given_HexString("1077");
            AssertCode(     // zextb\tr12,r4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = SLICE(r4, byte, 0)",
                "2|L--|r12 = CONVERT(v5, byte, word32)");
        }

        [Test]
        public void CSkyRw_zexth()
        {
            Given_HexString("1177");
            AssertCode(     // zexth\tr12,r4
                "0|L--|00100000(2): 2 instructions",
                "1|L--|v5 = SLICE(r4, word16, 0)",
                "2|L--|r12 = CONVERT(v5, word16, word32)");
        }

        [Test]
        public void CSkyRw_fabsd()
        {
            Given_HexString("0AF4 C908"); // fabsd\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = fabs(vr10)");
        }

        [Test]
        public void CSkyRw_fabsm()
        {
            Given_HexString("0AF4 C910"); // fabsm\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = __simd_fabs<real32[2]>(vr10)");
        }

        [Test]
        public void CSkyRw_fabss()
        {
            Given_HexString("0AF4 C900"); // fabss\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = fabsf(v4)",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_faddd()
        {
            Given_HexString("4AF4 0908"); // faddd\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = vr10 + vr2");
        }

        [Test]
        public void CSkyRw_fcmpltd()
        {
            Given_HexString("4AF4 A009"); // fcmpltd\tvr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|C = vr10 < vr2");
        }

        [Test]
        public void CSkyRw_fdivd()
        {
            Given_HexString("4AF4 090B"); // fdivd\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = vr10 / vr2");
        }

        [Test]
        public void CSkyRw_fdtos()
        {
            Given_HexString("0AF4 C91A"); // fdtos\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(vr10, real64, real32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtosi()
        {
            Given_HexString("0AF4 0919"); // fdtosi.rn\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(round(vr10), real64, int32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtosi_rz()
        {
            Given_HexString("0AF4 2919");   // fdtosi.rz\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(trunc(vr10), real64, int32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtosi_rpi()
        {
            Given_HexString("0AF4 4919"); // fdtosi.rpi\tvr9,vr10
            AssertCode(
                 "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(ceil(vr10), real64, int32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtosi_rni()
        {
            Given_HexString("0AF4 6919"); // fdtosi.rni\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(floor(vr10), real64, int32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtoui_rn()
        {
            Given_HexString("0AF4 8919"); // fdtoui.rn\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(round(vr10), real64, uint32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtoui_rz()
        {
            Given_HexString("0AF4 A919");
            AssertCode(     // fdtoui.rz vr9,vr10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(trunc(vr10), real64, uint32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtoui_rpi()
        {
            Given_HexString("0AF4 C919");
            AssertCode(     // fdtoui.rpi\tvr9,vr10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(ceil(vr10), real64, uint32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fdtoui_rni()
        {
            Given_HexString("0AF4 E919");
            AssertCode(     // fdtoui.rni\tvr9,vr10
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v5 = CONVERT(floor(vr10), real64, uint32)",
                "2|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v5)");
        }

        [Test]
        public void CSkyRw_fldd()
        {
            Given_HexString("F5F5 F921"); // fldd\tvr9,(r21,1020)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = Mem0[r21 + 1020<i32>:real64]");
        }

        [Test]
        public void CSkyRw_fldm()
        {
            Given_HexString("F5F5 F922"); // fldm\tvr9,(r21,2040)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = Mem0[r21 + 2040<i32>:real64]");
        }

        [Test]
        [Ignore("Manual is unclear")]
        public void CSkyRw_fldmd()
        {
            Given_HexString("15F5 0931"); // fldmd\tvr9,(r15,1020)
            AssertCode("@@@");

            AssertCode("fldmd\tvr9,(r15,1020)", "35F5 0931");
            AssertCode("fldmd\tvr9,(r15,1020)", "F5F5 0931");
        }

        [Test]
        [Ignore("Manual is unclear")]
        public void CSkyRw_fldmm()
        {
            Given_HexString("15F5 0932"); // fldmm\tvr9,(r15,1020)
AssertCode("@@@");

            AssertCode("fldmm\tvr9,(r15,1020)", "35F5 0932");
            AssertCode("fldmm\tvr9,(r15,1020)", "F5F5 0932");
        }

        [Test]
        public void CSkyRw_fldrd_0()
        {
            Given_HexString("15F5 0929"); // fldrd\tvr9,(r21,r8)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = Mem0[r21 + r8:real64]");
        }

        [Test]
        public void CSkyRw_fldrd_1()
        {
            Given_HexString("15F5 2929");   // fldrd\tvr9,(r21,r8<<1)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = Mem0[r21 + r8 * 2<32>:real64]");
        }

        [Test]
        public void CSkyRw_fldrd_3()
        {
            Given_HexString("15F5 6929");       // fldrd\tvr9,(r21,r8<<3)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = Mem0[r21 + r8 * 8<32>:real64]");
        }

        [Test]
        public void CSkyRw_fmacd()
        {
            Given_HexString("4AF4 890A"); // fmacd\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = vr9 + vr10 * vr2");
        }

        [Test]
        public void CSkyRw_fmacm()
        {
            Given_HexString("4AF4 8912"); // fmacm\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = __simd_fmac<real32[2]>(vr9, vr10, vr2)");
        }

        [Test]
        public void CSkyRw_fmfvrh()
        {
            Given_HexString("4AF4 091B"); // fmfvrh\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = SLICE(vr10, word32, 32)");
        }

        [Test]
        public void CSkyRw_fmtvrl()
        {
            Given_HexString("4AF4 291B"); // fmtvrl\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r9 = SLICE(vr10, word32, 0)");
        }

        [Test]
        public void CSkyRw_fmovd()
        {
            Given_HexString("4AF4 8908"); // fmovd\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = vr10");
        }

        [Test]
        public void CSkyRw_fmtvrh()
        {
            Given_HexString("4AF4 491B"); // fmtvrh\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = SEQ(r10, SLICE(vr9, word32, 0))");
        }

        [Test]
        public void CSkyRw_fmuld()
        {
            Given_HexString("4AF4 090A"); // fmuld\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = vr10 * vr2");
        }

        [Test]
        public void CSkyRw_fnegs()
        {
            Given_HexString("4AF4 E900"); // fnegs\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = -v4",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fnmuld()
        {
            Given_HexString("4AF4 290A"); // fnmuld\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = -(vr10 * vr2)");
        }

        [Test]
        public void CSkyRw_frecipd()
        {
            Given_HexString("4AF4 290B"); // frecipd\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = 1.0 / vr10");
        }

        [Test]
        public void CSkyRw_frecips()
        {
            Given_HexString("4AF4 2903"); // frecipd\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = 1.0F / v4",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fsitod()
        {
            Given_HexString("4AF4 891A"); // fsitod\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(vr10, int32, 0)",
                "2|L--|vr9 = CONVERT(v4, int32, real64)");
        }

        [Test]
        public void CSkyRw_fsqrtd()
        {
            Given_HexString("4AF4 490B"); // fsqrtd\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = sqrt(vr10)");
        }

        [Test]
        public void CSkyRw_fsqrts()
        {
            Given_HexString("4AF4 4903"); // fsqrtd\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = sqrtf(v4)",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fstd()
        {
            Given_HexString("F5F5 F925"); // fstd\tvr9,(r21,1020)
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + 1020<i32>:real64] = vr9");
        }

        [Test]
        [Ignore("Manual is unclear")]
        public void CSkyRw_fstmd()
        {
            Given_HexString("15F5 0935"); // fstmd\tvr9,(r15,1020)
            AssertCode("@@@");

            AssertCode("fstmd\tvr9,(r15,1020)", "35F5 0935");
            AssertCode("fstmd\tvr9,(r15,1020)", "F5F5 0935");
        }

        [Test]
        [Ignore("Manual is unclear")]
        public void CSkyRw_fstmm()
        {
            Given_HexString("15F5 0936"); // fstmm\tvr9,(r15,1020)

            AssertCode("fstmm\tvr9,(r15,1020)", "35F5 0936");
            AssertCode("fstmm\tvr9,(r15,1020)", "F5F5 0936");
        }

        [Test]
        [Ignore("Manual is unclear")]
        public void CSkyRw_fstms()
        {
            Given_HexString("15F5 0934"); // fstmm\tvr9,(r15,1020)

            AssertCode("fstmm\tvr9,(r15,1020)", "35F5 0934");
            AssertCode("fstmm\tvr9,(r15,1020)", "F5F5 0934");
        }

        [Test]
        public void CSkyRw_fstod()
        {
            Given_HexString("15F4 E91A");
            AssertCode(     // fstod\tvr9,vr5
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(vr5, real32, 0)",
                "2|L--|vr9 = CONVERT(v4, real32, real64)");
        }

        [Test]
        public void CSkyRw_fstosi_rn()
        {
            Given_HexString("0AF4 0918"); // fstosi.rn\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = CONVERT(roundf(v4), real32, int32)",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fstosi_rz()
        {
            Given_HexString("0AF4 2918");   // fstosi.rz\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = CONVERT(truncf(v4), real32, int32)",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fstosi_rpi()
        {
            Given_HexString("0AF4 4918");   // fstosi.rpi\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = CONVERT(ceilf(v4), real32, int32)",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fstosi_rni()
        {
            Given_HexString("0AF4 6918");   // fstosi.rni\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|L--|v4 = SLICE(vr10, real32, 0)",
                "2|L--|v6 = CONVERT(floorf(v4), real32, int32)",
                "3|L--|vr9 = SEQ(SLICE(vr9, word32, 32), v6)");
        }

        [Test]
        public void CSkyRw_fstrs_0()
        {
            Given_HexString("15F5 092C");
            AssertCode(      // fstrs\tvr9,(r21,r8)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r8:real64] = vr9");
        }

        [Test]
        public void CSkyRw_fstrs_1()
        {
            Given_HexString("15F5 292C");
            AssertCode(     // fstrs\tvr9,(r21,r8<<1)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r8 * 2<32>:real64] = vr9");
        }

        [Test]
        public void CSkyRw_fstrs_3()
        {
            Given_HexString("15F5 692C");
            AssertCode(     // fstrs\tvr9,(r21,r8<<3)
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r21 + r8 * 8<32>:real64] = vr9");
        }

        [Test]
        public void CSkyRw_fsubd()
        {
            Given_HexString("4AF4 2908"); // fsubd\tvr9,vr10,vr2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|vr9 = vr10 - vr2");
        }

        [Test]
        public void CSkyRw_fuitod()
        {
            Given_HexString("4AF4 A91A"); // fuitod\tvr9,vr10
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|v4 = SLICE(vr10, uint32, 0)",
                "2|L--|vr9 = CONVERT(v4, uint32, real64)");
        }
    }
}
