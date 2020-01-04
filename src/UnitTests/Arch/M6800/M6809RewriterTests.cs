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
using Reko.Arch.M6800;
using Reko.Core;
using Reko.Core.Configuration;
using Reko.Core.Rtl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.M6800
{
    [TestFixture]
    public class M6809RewriterTests : RewriterTestBase
    {
        private M6809Architecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new Reko.Arch.M6800.M6809Architecture("m6809");
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            return arch.CreateRewriter(
                new BeImageReader(mem, mem.BaseAddress),
                arch.CreateProcessorState(),
                binder,
                host);
        }

        [Test]
        public void M6809Rw_abx()
        {
            Given_HexString("3A"); // abx
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|x = x + (uint16) b");
        }

        [Test]
        public void M6809Rw_adca()
        {
            Given_HexString("A928"); // adca	$08,y
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|a = a + Mem0[y + 8:byte] + C",
                "2|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6809Rw_adcb()
        {
            Given_HexString("C91F"); // adcb	#$1F
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|b = b + 0x1F + C",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6809Rw_adda_postinc2()
        {
            Given_HexString("AB81"); // adda ,x++
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a + Mem0[x:byte]",
                "2|L--|x = x + 2",
                "3|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6809Rw_addb_postinc1()
        {
            Given_HexString("EBA0"); // addb ,y+
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|b = b + Mem0[y:byte]",
                "2|L--|y = y + 1",
                "3|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6809Rw_addd_predec2()
        {
            Given_HexString("E3C3"); // addd ,--u
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|u = u - 2",
                "2|L--|d = d + Mem0[u:word16]",
                "3|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6809Rw_anda_predec1()
        {
            Given_HexString("A4E2"); // anda ,-s
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|s = s - 1",
                "2|L--|a = a & Mem0[s:byte]",
                "3|L--|NZ = cond(a)",
                "4|L--|V = false");
        }

        [Test]
        public void M6809Rw_andb_const_offset()
        {
            Given_HexString("E48880"); // andb -$80,x
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|b = b & Mem0[x - 128:byte]",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6809Rw_andcc()
        {
            Given_HexString("1CFE"); // andcc #$FE
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|cc = cc & 0xFE");
        }

        [Test]
        public void M6809Rw_asl_acc_offset()
        {
            Given_HexString("68A6"); // asl a,y
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|v4 = Mem0[y + (int16) a:byte] << 0x01",
                "2|L--|Mem0[y + (int16) a:byte] = v4",
                "3|L--|NZVC = cond(v4)");
        }

        [Test]
        public void M6809Rw_asr_pc_offset()
        {
            Given_HexString("678CFC"); // asr -$04,pc
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|v2 = Mem0[0x00FF:byte] << 0x01",
                "2|L--|Mem0[0x00FF:byte] = v2",
                "3|L--|NZC = cond(v2)");
        }

        [Test]
        public void M6809Rw_bcc()
        {
            Given_HexString("24FC"); // bcc $00FE
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 00FE");
        }

        [Test]
        public void M6809Rw_bita_indirect_extended()
        {
            Given_HexString("A59F1234"); // bita [$1234]
            AssertCode(
                "0|L--|0100(4): 4 instructions",
                "1|L--|v3 = Mem0[Mem0[0x1234:ptr16]:byte]",
                "2|L--|v3 = a & v3",
                "3|L--|NZ = cond(v3)",
                "4|L--|V = false");
        }

        [Test]
        public void M6809Rw_bra()
        {
            Given_HexString("20FE"); // bra $0100
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto 0100");
        }

        [Test]
        public void M6809Rw_bsr()
        {
            Given_HexString("8D80"); // bsr $0100
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|call 0082 (2)");
        }

        [Test]
        public void M6809Rw_clra()
        {
            Given_HexString("4F"); // bsr $0100
            AssertCode(
                "0|L--|0100(1): 5 instructions",
                "1|L--|a = 0x00",
                "2|L--|N = false",
                "3|L--|Z = true",
                "4|L--|V = false",
                "5|L--|C = false");
        }

        [Test]
        public void M6809Rw_clr_postInc_indirect()
        {
            Given_HexString("6FB1"); // clr [,y++]
            AssertCode(
                "0|L--|0100(2): 7 instructions",
                "1|L--|v3 = 0x00",
                "2|L--|Mem0[Mem0[y:ptr16]:byte] = v3",
                "3|L--|y = y + 2",
                "4|L--|N = false",
                "5|L--|Z = true",
                "6|L--|V = false",
                "7|L--|C = false");
        }

        [Test]
        public void M6809Rw_cmpa_preDec_indirect()
        {
            Given_HexString("A1D3"); // cmpa [,--u]
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|u = u - 2",
                "2|L--|v4 = Mem0[Mem0[u:ptr16]:byte]",
                "3|L--|v4 = a - v4",
                "4|L--|NZVC = cond(v4)");
        }

        [Test]
        public void M6809Rw_com_indexed_indirect()
        {
            Given_HexString("639B"); // com [d,x]
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|v4 = ~Mem0[Mem0[x + d:ptr16]:byte]",
                "2|L--|Mem0[Mem0[x + d:ptr16]:byte] = v4",
                "3|L--|NZ = cond(v4)",
                "4|L--|V = false",
                "5|L--|C = true");
        }

        [Test]
        public void M6809Rw_cwai()
        {
            Given_HexString("3C26"); // cwai	#$26
            AssertCode(
                "0|L--|0100(2): 17 instructions",
                "1|L--|cc = cc & 0x26",
                "2|L--|s = s - 2",
                "3|L--|Mem0[s:ptr16] = pcr",
                "4|L--|s = s - 2",
                "5|L--|Mem0[s:word16] = u",
                "6|L--|s = s - 2",
                "7|L--|Mem0[s:word16] = y",
                "8|L--|s = s - 2",
                "9|L--|Mem0[s:word16] = x",
                "10|L--|s = s - 1",
                "11|L--|Mem0[s:byte] = dp",
                "12|L--|s = s - 1",
                "13|L--|Mem0[s:byte] = b",
                "14|L--|s = s - 1",
                "15|L--|Mem0[s:byte] = a",
                "16|L--|s = s - 1",
                "17|L--|Mem0[s:byte] = cc");
        }

        [Test]
        public void M6809Rw_daa()
        {
            Given_HexString("19"); // daa
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = __daa(a)");
        }

        [Test]
        public void M6809Rw_deca()
        {
            Given_HexString("4A"); // deca
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|a = a - 1",
                "2|L--|NZV = cond(a)");
        }

        [Test]
        public void M6809Rw_dec_extend()
        {
            Given_HexString("7A1234"); // dec $1234
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|v2 = Mem0[0x1234:byte] - 1",
                "2|L--|Mem0[0x1234:byte] = v2",
                "3|L--|NZV = cond(v2)");
        }

        [Test]
        public void M6809Rw_inc_postInc_1()
        {
            Given_HexString("6CA0"); //inc ,y+
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|v3 = Mem0[y:byte] + 1",
                "2|L--|Mem0[y:byte] = v3",
                "3|L--|y = y + 1",
                "4|L--|NZV = cond(v3)");
        }

        [Test]
        public void M6809Rw_jmp()
        {
            Given_HexString("6E84"); // jmp	,x
            AssertCode(
                "0|T--|0100(2): 1 instructions",
                "1|T--|goto x");
        }

        [Test]
        public void M6809Rw_jsr()
        {
            Given_HexString("BDA928"); // jsr	$A928
            AssertCode(
                "0|T--|0100(3): 1 instructions",
                "1|T--|call A928 (2)");
        }

        [Test]
        public void M6809Rw_ldy()
        {
            Given_HexString("10AE04"); // ldy 4,x
            AssertCode(
                "0|L--|0100(3): 3 instructions",
                "1|L--|y = Mem0[x + 4:word16]",
                "2|L--|NZ = cond(y)",
                "3|L--|V = false");
        }

        [Test]
        public void M6809Rw_leas()
        {
            Given_HexString("320D"); // leas	$0D,x
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|s = x + 13");
        }

        [Test]
        public void M6809Rw_leau()
        {
            Given_HexString("335E"); // leau	-$02,u
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|u = u - 2");
        }

        [Test]
        public void M6809Rw_leax()
        {
            Given_HexString("3001"); // leax	$01,x
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|x = x + 1",
                "2|L--|Z = cond(x)");
        }

        [Test]
        public void M6809Rw_leay()
        {
            Given_HexString("318CE4"); // leay	-$1C,pcr
            AssertCode(
                "0|L--|0100(3): 2 instructions",
                "1|L--|y = 00E7",
                "2|L--|Z = cond(y)");
        }

        [Test]
        public void M6809Rw_lsl_predec2()
        {
            Given_HexString("68E3"); // lsl ,--s
            AssertCode(
                "0|L--|0100(2): 4 instructions",
                "1|L--|s = s - 2",
                "2|L--|v3 = Mem0[s:byte] << 0x01",
                "3|L--|Mem0[s:byte] = v3",
                "4|L--|NZVC = cond(v3)");
        }

        [Test]
        public void M6809Rw_lsr()
        {
            Given_HexString("0412"); // lsr	>$12
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|v3 = Mem0[dp + 0x12:byte] >>u 0x01",
                "2|L--|Mem0[dp + 0x12:byte] = v3",
                "3|L--|NZC = cond(v3)");
        }

        [Test]
        public void M6809Rw_mul()
        {
            Given_HexString("3D"); // mul
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|d = a *u b",
                "2|L--|ZC = cond(d)");
        }

        [Test]
        public void M6809Rw_neg_dir()
        {
            Given_HexString("0042");
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|v3 = -Mem0[dp + 0x42:byte]",
                "2|L--|Mem0[dp + 0x42:byte] = v3",
                "3|L--|NZVC = cond(v3)");
        }

        [Test]
        public void M6809Rw_ora()
        {
            Given_HexString("9ACE"); // ora	>$CE
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|a = a | Mem0[dp + 0xCE:byte]",
                "2|L--|NZ = cond(a)",
                "3|L--|V = false");
        }

        [Test]
        public void M6809Rw_orcc()
        {
            Given_HexString("1A7E"); // orcc	#$7E
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|cc = cc | 0x7E");
        }

        [Test]
        public void M6809Rw_puls()
        {
            Given_HexString("35AA"); // puls	pcr,y,dp,a
            AssertCode(
                "0|L--|0100(2): 7 instructions",
                "1|L--|a = Mem0[s:byte]",
                "2|L--|s = s + 1",
                "3|L--|dp = Mem0[s:byte]",
                "4|L--|s = s + 1",
                "5|L--|y = Mem0[s:word16]",
                "6|L--|s = s + 2",
                "7|T--|return (2,0)");
        }

        [Test]
        public void M6809Rw_pulu()
        {
            Given_HexString("37B7"); // pulu	pcr,y,x,b,a,cc
            AssertCode(
                "0|L--|0100(2): 11 instructions",
                "1|L--|cc = Mem0[u:byte]",
                "2|L--|u = u + 1",
                "3|L--|a = Mem0[u:byte]",
                "4|L--|u = u + 1",
                "5|L--|b = Mem0[u:byte]",
                "6|L--|u = u + 1",
                "7|L--|x = Mem0[u:word16]",
                "8|L--|u = u + 2",
                "9|L--|y = Mem0[u:word16]",
                "10|L--|u = u + 2",
                "11|T--|return (2,0)");
        }

        [Test]
        public void M6809Rw_pshs()
        {
            Given_HexString("34A7"); // pshs	pcr,y,b,a,cc
            AssertCode(
                "0|L--|0100(2): 10 instructions",
                "1|L--|s = s - 2",
                "2|L--|Mem0[s:ptr16] = pcr",
                "3|L--|s = s - 2",
                "4|L--|Mem0[s:word16] = y",
                "5|L--|s = s - 1",
                "6|L--|Mem0[s:byte] = b",
                "7|L--|s = s - 1",
                "8|L--|Mem0[s:byte] = a",
                "9|L--|s = s - 1",
                "10|L--|Mem0[s:byte] = cc");
        }

        [Test]
        public void M6809Rw_pshu()
        {
            Given_HexString("3602"); // pshu	a
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|u = u - 1",
                "2|L--|Mem0[u:byte] = a");
        }

        [Test]
        public void M6809Rw_rol()
        {
            Given_HexString("59"); // rol	b
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|b = __rol(b, 0x01)",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6809Rw_ror()
        {
            Given_HexString("0630"); // ror	>$30
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|v3 = __ror(Mem0[dp + 0x30:byte], 0x01)",
                "2|L--|Mem0[dp + 0x30:byte] = v3",
                "3|L--|NZC = cond(v3)");
        }

        [Test]
        public void M6809Rw_rti()
        {
            Given_HexString("3B"); // rti
            AssertCode(
                "0|T--|0100(1): 15 instructions",
                "1|L--|cc = Mem0[s:byte]",
                "2|L--|s = s + 1",
                "3|L--|a = Mem0[s:byte]",
                "4|L--|s = s + 1",
                "5|L--|b = Mem0[s:byte]",
                "6|L--|s = s + 1",
                "7|L--|dp = Mem0[s:byte]",
                "8|L--|s = s + 1",
                "9|L--|x = Mem0[s:word16]",
                "10|L--|s = s + 2",
                "11|L--|y = Mem0[s:word16]",
                "12|L--|s = s + 2",
                "13|L--|u = Mem0[s:word16]",
                "14|L--|s = s + 2",
                "15|T--|return (2,0)");
        }

        [Test]
        public void M6809Rw_rts()
        {
            Given_HexString("39"); // rts
            AssertCode(
                "0|T--|0100(1): 1 instructions",
                "1|T--|return (2,0)");
        }

        [Test]
        public void M6809Rw_sbca()
        {
            Given_HexString("A282"); // sbca	,-x
            AssertCode(
                "0|L--|0100(2): 3 instructions",
                "1|L--|x = x - 1",
                "2|L--|a = a - Mem0[x:byte] - C",
                "3|L--|NZVC = cond(a)");
        }

        [Test]
        public void M6809Rw_sbcb()
        {
            Given_HexString("E227"); // sbcb	$07,y
            AssertCode(
                "0|L--|0100(2): 2 instructions",
                "1|L--|b = b - Mem0[y + 7:byte] - C",
                "2|L--|NZVC = cond(b)");
        }

        [Test]
        public void M6809Rw_sex()
        {
            Given_HexString("1D"); // sex
            AssertCode(
                "0|L--|0100(1): 2 instructions",
                "1|L--|d = (int16) b",
                "2|L--|NZ = cond(d)");
        }

        [Test]
        public void M6809Rw_sta()
        {
            Given_HexString("B7FF23"); // sta $FF23
            AssertCode(
                "0|L--|0100(3): 4 instructions",
                "1|L--|v3 = a",
                "2|L--|Mem0[0xFF23:byte] = v3",
                "3|L--|NZ = cond(v3)",
                "4|L--|V = false");
        }

        [Test]
        public void M6809Rw_std()
        {
            Given_HexString("EDC1"); // std ,u++
            AssertCode(
                "0|L--|0100(2): 5 instructions",
                "1|L--|v4 = d",
                "2|L--|Mem0[u:word16] = v4",
                "3|L--|u = u + 2",
                "4|L--|NZ = cond(v4)",
                "5|L--|V = false");
        }

        [Test]
        public void M6809Rw_subd_imm()
        {
            Given_HexString("831234"); // subd #$1234
            AssertCode(
                "0|L--|0100(3): 2 instructions",
                "1|L--|d = d - 0x1234",
                "2|L--|NZVC = cond(d)");
        }

        [Test]
        public void M6809Rw_swi()
        {
            Given_HexString("3F"); // swi
            AssertCode(
                "0|L--|0100(1): 17 instructions",
                "1|L--|s = s - 2",
                "2|L--|Mem0[s:ptr16] = pcr",
                "3|L--|s = s - 2",
                "4|L--|Mem0[s:word16] = u",
                "5|L--|s = s - 2",
                "6|L--|Mem0[s:word16] = y",
                "7|L--|s = s - 2",
                "8|L--|Mem0[s:word16] = x",
                "9|L--|s = s - 1",
                "10|L--|Mem0[s:byte] = dp",
                "11|L--|s = s - 1",
                "12|L--|Mem0[s:byte] = b",
                "13|L--|s = s - 1",
                "14|L--|Mem0[s:byte] = a",
                "15|L--|s = s - 1",
                "16|L--|Mem0[s:byte] = cc",
                "17|T--|goto Mem0[0xFFFA:word16]");
        }

        [Test]
        public void M6809Rw_sync()
        {
            Given_HexString("13"); // sync
            AssertCode(
                "0|L--|0100(1): 1 instructions",
                "1|L--|__sync()");
        }

        [Test]
        public void M6809Rw_tfr()
        {
            Given_HexString("1F9B"); // tfr	b,dp
            AssertCode(
                "0|L--|0100(2): 1 instructions",
                "1|L--|dp = b");
        }

        [Test]
        public void M6809Rw_tst()
        {
            Given_HexString("5D"); // tst	b
            AssertCode(
                "0|L--|0100(1): 3 instructions",
                "1|L--|b = b - 0x00",
                "2|L--|NZ = cond(b)",
                "3|L--|V = false");
        }

        [Test]
        public void M6809Rw_tst_mem()
        {
            Given_HexString("7EB593"); // tst	$B593
            AssertCode(
                "0|L--|0100(3): 4 instructions",
                "1|L--|v2 = Mem0[0xB593:byte]",
                "2|L--|v2 = v2 - 0x00",
                "3|L--|NZ = cond(v2)",
                "4|L--|V = false");
        }


    }
}
