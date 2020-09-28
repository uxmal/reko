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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Reko.Arch.Arc;
using Reko.Core;
using Reko.Core.Rtl;

namespace Reko.UnitTests.Arch.Arc
{
    [TestFixture]
    public class ARCompactRewriterTests : RewriterTestBase
    {
        private ARCompactArchitecture arch;
        private Address addr;

        [SetUp]
        public void Setup()
        {
            this.arch = new ARCompactArchitecture("arCompact");
            arch.LoadUserOptions(new Dictionary<string, object>
            {
                { "Endianness", "be" }
            });
            this.addr = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(MemoryArea mem, IStorageBinder binder, IRewriterHost host)
        {
            var state = new ARCompactState(arch);
            var rdr = arch.CreateImageReader(mem, 0);
            return new ARCompactRewriter(arch, rdr, state, binder, host);
        }

        [Test]
        public void ARCompactRw_adc()
        {
            Given_HexString("2001E180"); // adc	r0,r48,r6
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = r48 + r6 + C",
                "2|L--|ZNCV = cond(r0)");
        }

        [Test]
        public void ARCompactRw_add()
        {
            Given_HexString("20C00301"); // add.eq	r0,r0,r12
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|L--|r0 = r0 + r12");
        }

        [Test]
        public void ARCompactRw_add_s_imm()
        {
            Given_HexString("E043"); // add_s	r0,r0,00000043
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 + 0x00000043");
        }

        [Test]
        public void ARCompactRw_add1()
        {
            Given_HexString("2414804A"); // add1	r10,r4,r1
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r10 = r4 + (r1 << 0x01)",
                "2|L--|ZNCV = cond(r10)");
        }

        [Test]
        public void ARCompactRw_add1_s()
        {
            Given_HexString("7934"); // add1_s	r1,r1,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 + (r1 << 0x01)");
        }

        [Test]
        public void ARCompactRw_add2()
        {
            Given_HexString("21158003"); // add2.f	r3,r1,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r3 = r1 + (r0 << 0x02)",
                "2|L--|ZNCV = cond(r3)");
        }

        [Test]
        public void ARCompactRw_add2_s()
        {
            Given_HexString("7915"); // add2_s	r1,r1,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 + (r0 << 0x02)");
        }

        [Test]
        public void ARCompactRw_add3_imm()
        {
            Given_HexString("25561C40"); // add3	r0,r13,00000031
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r13 + (0x00000031 << 0x03)");
        }

        [Test]
        public void ARCompactRw_add3_s()
        {
            Given_HexString("7936"); // add3_s	r1,r1,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = r1 + (r1 << 0x03)");
        }

        [Test]
        public void ARCompactRw_addsdw()
        {
            Given_HexString("2F28E0D0"); // addsdw	r16,r55,r3
            AssertCode(
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v2 = r55",
                "2|L--|v3 = r3",
                "3|L--|r16 = __addsdw(v2, v3)",
                "4|L--|ZNV = cond(r16)",
                "5|L--|S = cond(r16)");
        }

        [Test]
        public void ARCompactRw_and()
        {
            Given_HexString("20440401"); // and	r1,r0,00000010
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r0 & 0x00000010");
        }

        [Test]
        public void ARCompactRw_and_s()
        {
            Given_HexString("7D04"); // and_s	r13,r13,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r13 = r13 & r0");
        }

        [Test]
        public void ARCompactRw_asl()
        {
            Given_HexString("2F800000"); // asl	r0,r7,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r7 << r0");
        }

        [Test]
        public void ARCompactRw_asl_s()
        {
            Given_HexString("6E12"); // asl_s	r0,r14,00000002
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r14 << 0x00000002");
        }

        [Test]
        public void ARCompactRw_asr()
        {
            Given_HexString("2D421200"); // asr	r0,r13,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r13 >> r8");
        }

        [Test]
        public void ARCompactRw_asr_s()
        {
            Given_HexString("691B"); // asr_s	r0,r1,00000003
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r1 >> 0x00000003");
        }

        [Test]
        public void ARCompactRw_bbit1()
        {
            Given_HexString("0E8101CF"); // bbit1	r15,r7,0000329E
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (__bit(r6, r7)) branch 00100080");
        }

        [Test]
        public void ARCompactRw_bclr()
        {
            Given_HexString("27108000"); // bclr	r0,r7,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = __bclr(r7, r0)",
                "2|L--|ZN = cond(r0)");
        }

        [Test]
        public void ARCompactRw_bclr_s()
        {
            Given_HexString("B9A1"); // bclr_s	r1,r1,00000001
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = __bclr(r1, 0x00000001)");
        }

        [Test]
        public void ARCompactRw_bic_s()
        {
            Given_HexString("78E6"); // bic_s	r0,r0,r15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 & ~r15");
        }


        [Test]
        public void ARCompactRw_bmsk()
        {
            Given_HexString("20530001"); // bmsk	r1,r0,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = __bitmask(r0, 0x00000000)");
        }

        [Test]
        public void ARCompactRw_bset()
        {
            Given_HexString("204F07C1"); // bset	r1,r0,0000001F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = __bset(r0, 0x0000001F)");
        }

        [Test]
        public void ARCompactRw_bset_s()
        {
            Given_HexString("B982"); // bset_s	r1,r1,00000002
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = __bset(r1, 0x00000002)");
        }

        [Test]
        public void ARCompactRw_btst()
        {
            Given_HexString("261190C0"); // btst	r14,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ZN = cond(__btst(r14, r3))");
        }

        [Test]
        public void ARCompactRw_btst_s()
        {
            Given_HexString("B8E0"); // btst_s	r0,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|ZN = cond(__btst(r0, 0x00000000))");
        }

        [Test]
        public void ARCompactRw_bxor()
        {
            Given_HexString("25922053"); // bxor	r21,r21,+000004C1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r21 = __bxor(r21, 1217)");
        }

        [Test]
        public void ARCompactRw_bcc()
        {
            Given_HexString("02802046"); // bcc	000411CC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 00140A80");
        }

        [Test]
        public void ARCompactRw_bcs()
        {
            Given_HexString("02802045"); // bcs	000411FC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 00140A80");
        }

        [Test]
        public void ARCompactRw_beq_d()
        {
            Given_HexString("0000C5E1"); // beq.d	FFF8B82C
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(EQ,Z)) branch 0008B800");
        }

        [Test]
        public void ARCompactRw_beq_s()
        {
            Given_HexString("F205"); // beq_s	0000013E
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(EQ,Z)) branch 0010000A");
        }

        [Test]
        public void ARCompactRw_bge()
        {
            Given_HexString("0300240A"); // bge	0004831C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,ZN)) branch 00148300");
        }

        [Test]
        public void ARCompactRw_bge_s()
        {
            Given_HexString("F653"); // bge_s	00001196
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(GE,ZN)) branch 00100026");
        }

        [Test]
        public void ARCompactRw_bgt()
        {
            Given_HexString("000070C9"); // bgt	000E1814
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GT,ZNV)) branch 001E1800");
        }

        [Test]
        public void ARCompactRw_bgt_s()
        {
            Given_HexString("F60B"); // bgt_s	00001F52
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(GT,ZNV)) branch 00100016");
        }

        [Test]
        public void ARCompactRw_bhi()
        {
            Given_HexString("004C000D"); // bhi	000001D0
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(UGT,ZC)) branch 0010004C");
        }

        [Test]
        public void ARCompactRw_bhi_s()
        {
            Given_HexString("F738"); // bhi_s	00000188
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(UGT,ZC)) branch 000FFFF0");
        }

        [Test]
        public void ARCompactRw_bhs_s()
        {
            Given_HexString("F747"); // bhs_s	00000182
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(UGE,C)) branch 0010000E");
        }

        [Test]
        public void ARCompactRw_blo_s()
        {
            Given_HexString("F792"); // blo_s	000007F0
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(ULT,C)) branch 00100024");
        }

        [Test]
        public void ARCompactRw_bls()
        {
            Given_HexString("0000420E"); // bls	00085DCC
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(ULE,ZC)) branch 00184000");
        }

        [Test]
        public void ARCompactRw_bls_s()
        {
            Given_HexString("F7C5"); // bls_s	000007CE
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(ULE,ZC)) branch 0010000A");
        }

        [Test]
        public void ARCompactRw_bmi()
        {
            Given_HexString("07C01404"); // bmi	00028BF0
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(LT,N)) branch 001287C0");
        }

        [Test]
        public void ARCompactRw_bne()
        {
            Given_HexString("00008702"); // bne	FFF0E208
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 0000E000");
        }

        [Test]
        public void ARCompactRw_bpl()
        {
            Given_HexString("0000D803"); // bpl	FFFB017C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,N)) branch 000B0000");
        }

        [Test]
        public void ARCompactRw_bpnz()
        {
            Given_HexString("0002242F"); // bpnz.d	00048886
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(GT,ZN)) branch 00148002");
        }

        [Test]
        public void ARCompactRw_bvc()
        {
            Given_HexString("00801408"); // bvc	000284B8
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(NO,V)) branch 00128080");
        }


        [Test]
        public void ARCompactRw_bvs()
        {
            Given_HexString("00008707"); // bvs	FFF0E22C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(OV,V)) branch 0000E000");
        }

        [Test]
        public void ARCompactRw_bl()
        {
            Given_HexString("0B0A0000"); // bl	00000308
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100308 (0)");
        }

        [Test]
        public void ARCompactRw_blal()
        {
            Given_HexString("0F800000"); // blal	00000AC0
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100780 (0)");
        }

        [Test]
        public void ARCompactRw_blcc()
        {
            Given_HexString("0F802006"); // blcc	00040A34
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(ULT,C)) branch 00100004",
                "2|T--|call 00140780 (0)");
        }

        [Test]
        public void ARCompactRw_blcs()
        {
            Given_HexString("0F802005"); // blcs	00041A40
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(UGE,C)) branch 00100004",
                "2|T--|call 00140780 (0)");
        }

        [Test]
        public void ARCompactRw_bleq()
        {
            Given_HexString("0F800001"); // bleq	000007DC
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|T--|call 00100780 (0)");
        }

        [Test]
        public void ARCompactRw_blge()
        {
            Given_HexString("0800F20A"); // blge	FFFE8790
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(LT,ZN)) branch 00100004",
                "2|T--|call 000E4000 (0)");
        }

        [Test]
        public void ARCompactRw_blgt()
        {
            Given_HexString("0A0407C9"); // blgt	00010300
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,ZNV)) branch 00100004",
                "2|T--|call 0010FA04 (0)");
        }

        [Test]
        public void ARCompactRw_blle()
        {
            Given_HexString("0FC01BEC"); // blle.d	0003B658
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|T--|if (Test(GT,ZNV)) branch 00100004",
                "2|TD-|call 00137FC0 (0)");
        }

        [Test]
        public void ARCompactRw_bllt()
        {
            Given_HexString("0FC0202B"); // bllt.d	0004A65C
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,ZN)) branch 00100004",
                "2|TD-|call 001407C0 (0)");
        }

        [Test]
        public void ARCompactRw_blmi()
        {
            Given_HexString("0A041104"); // blmi	00022A9C
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(GE,N)) branch 00100004",
                "2|T--|call 00122204 (0)");
        }

        [Test]
        public void ARCompactRw_blne()
        {
            Given_HexString("0F8C0002"); // blne	0000168C
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|T--|call 0010078C (0)");
        }

        [Test]
        public void ARCompactRw_blpl()
        {
            Given_HexString("0FC02843"); // blpl	00053A90
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(LT,N)) branch 00100004",
                "2|T--|call 00150FC0 (0)");
        }

        [Test]
        public void ARCompactRw_blpnz()
        {
            Given_HexString("094070CF"); // blpnz	000EB854
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(LE,ZN)) branch 00100004",
                "2|T--|call 001E1940 (0)");
        }

        [Test]
        public void ARCompactRw_blvc()
        {
            Given_HexString("0C00F788"); // blvc	FFFF1380
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(OV,V)) branch 00100004",
                "2|T--|call 000EF400 (0)");
        }

        [Test]
        public void ARCompactRw_blvs()
        {
            Given_HexString("0F90F007"); // blvs	FFFE615C
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(NO,V)) branch 00100004",
                "2|T--|call 000E0790 (0)");
        }

        [Test]
        public void ARCompactRw_bic()
        {
            Given_HexString("2006D7F8"); // bic	r56,r40,blink
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r56 = r40 & ~blink",
                "2|L--|ZN = cond(r56)");
        }

        [Test]
        public void ARCompactRw_extb()
        {
            Given_HexString("202F2407"); // extb	r16,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = (word32) SLICE(r16, byte, 0)");
        }

        [Test]
        public void ARCompactRw_extb_s()
        {
            Given_HexString("780F"); // extb_s	r0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (word32) SLICE(r0, byte, 0)");
        }

        [Test]
        public void ARCompactRw_extw()
        {
            Given_HexString("202F2408"); // extw	r16,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = (word32) SLICE(r16, word16, 0)");
        }

        [Test]
        public void ARCompactRw_extw_s()
        {
            Given_HexString("7ED0"); // extw_s	r14,r14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = (word32) SLICE(r14, word16, 0)");
        }

        [Test]
        public void ARCompactRw_flag()
        {
            Given_HexString("20690040"); // flag	r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__flag(r1)");
        }


        [Test]
        public void ARCompactRw_ld_ab()
        {
            Given_HexString("1404341B"); // ld.ab	fp,[sp,4]
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|fp = Mem0[sp:word32]",
                "2|L--|sp = sp + 4");
        }

        [Test]
        public void ARCompactRw_ld_as()
        {
            Given_HexString("10B20601"); // ld.as	r1,[r0,50]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = Mem0[r0 + 200:word32]");
        }

        [Test]
        public void ARCompactRw_ld_pcl()
        {
            Given_HexString("D7F8"); // ld_s	r15,[pcl,992]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r15 = Mem0[0x001003E0:word32]");
        }

        [Test]
        public void ARCompactRw_ldb_s()
        {
            Given_HexString("67A9"); // ldb_s	r1,[r15,0]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = (word32) Mem0[r15:byte]");
        }

        [Test]
        public void ARCompactRw_ldw_s()
        {
            Given_HexString("910B"); // ldw_s	r0,[r1,22]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (word32) Mem0[r1 + 22:word16]");
        }

        [Test]
        public void ARCompactRw_ldw_x()
        {
            Given_HexString("1080216A"); // ldw.x	r42,[r16]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r42 = (word32) Mem0[r16:int16]");
        }

        [Test]
        public void ARCompactRw_bmsk_s()
        {
            Given_HexString("B8C0"); // bmsk_s	r0,r0,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = __bitmask(r0, 0x00000000)");
        }

        [Test]
        public void ARCompactRw_ble()
        {
            Given_HexString("00C0140C"); // ble	000285D8
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(LE,ZNV)) branch 001280C0");
        }

        [Test]
        public void ARCompactRw_ble_s()
        {
            Given_HexString("F6D2"); // ble_s	00001648
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(LE,ZNV)) branch 00100024");
        }


        [Test]
        public void ARCompactRw_blt()
        {
            Given_HexString("0204202B"); // blt.d	00040CF0
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(LT,ZN)) branch 00140204");
        }

        [Test]
        public void ARCompactRw_blt_s()
        {
            Given_HexString("F683"); // blt_s	0000025A
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(LT,ZN)) branch 00100006");
        }

        [Test]
        public void ARCompactRw_bne_s()
        {
            Given_HexString("F403"); // bne_s	0000005A
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100006");
        }

        [Test]
        public void ARCompactRw_bss()
        {
            Given_HexString("00027150"); // bss	000E2922
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (__saturated(S)) branch 001E2802");
        }

        [Test]
        public void ARCompactRw_bl_s()
        {
            Given_HexString("FFFF"); // bl_s	00000334
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call 000FFFFC (0)");
        }

        [Test]
        public void ARCompactRw_breq()
        {
            Given_HexString("0DFFE080"); // breq	r53,r2,0000004E
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r53 == r2) branch 000FFFFE");
        }

        [Test]
        public void ARCompactRw_breq_s()
        {
            Given_HexString("E83E"); // breq_s	r0,+00000000,000036AC
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r0 == 0) branch 0010007C");
        }

        [Test]
        public void ARCompactRw_brge()
        {
            Given_HexString("0FC7B883"); // brge	blink,r34,00004D66
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (blink >= r34) branch 000FFFC6");
        }

        [Test]
        public void ARCompactRw_brhs()
        {
            Given_HexString("0FFF2005"); // brhs	r23,r0,0000043A
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r23 >=u r0) branch 001000FE");
        }

        [Test]
        public void ARCompactRw_brlo()
        {
            Given_HexString("0A051104"); // brlo	r10,r4,000008A0
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r10 <u r4) branch 00100004");
        }

        [Test]
        public void ARCompactRw_brlt()
        {
            Given_HexString("0FFFC0A2"); // brlt	r39,r2,00000B1A
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r39 < r2) branch 000FFFFE");
        }

        [Test]
        public void ARCompactRw_brne()
        {
            Given_HexString("0F8101E1"); // brne	r7,r7,0000B99C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r7 != r7) branch 00100080");
        }

        [Test]
        public void ARCompactRw_brne_s()
        {
            Given_HexString("EBD4"); // brne_s	r3,+00000000,00003154
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (r3 != 0) branch 000FFFA8");
        }

        [Test]
        public void ARCompactRw_brk_s()
        {
            Given_HexString("7FFF"); // brk_s
            AssertCode(
                "0|H--|00100000(2): 1 instructions",
                "1|H--|__brk()");
        }

        [Test]
        public void ARCompactRw_cmp()
        {
            Given_HexString("244C8000"); // cmp	r4,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ZNCV = cond(r4 - 0x00000000)");
        }

        [Test]
        public void ARCompactRw_cmp_s()
        {
            Given_HexString("E080"); // cmp_s	r0,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|ZNCV = cond(r0 - 0x00000000)");
        }

        [Test]
        public void ARCompactRw_divaw()
        {
            Given_HexString("2C48F02B"); // divaw	r43,lp_count,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r43 = __divaw(lp_count, r0)");
        }

        [Test]
        public void ARCompactRw_j_d_blink()
        {
            Given_HexString("202007C0"); // j.d	[blink]
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|return (0,0)");
        }

        [Test]
        public void ARCompactRw_j_s_blink()
        {
            Given_HexString("7EE0"); // j_s	[blink]
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void ARCompactRw_jeq_d_blink()
        {
            Given_HexString("20E007C1"); // jeq.d	[blink]
            AssertCode(
                "0|TD-|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|TD-|return (0,0)");
        }

        public void ARCompactRw_jl()
        {
            Given_HexString("20220F80"); // jl.d	[537240524]
            AssertCode(
                "0|L--|00100000(8): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_jl_s()
        {
            Given_HexString("7A40"); // jl_s	[r2]
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|call r2 (0)");
        }

        [Test]
        public void ARCompactRw_ldb_x()
        {
            Given_HexString("13FEB0C0"); // ldb.x	r0,[fp,-2]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = (word32) Mem0[fp - 2:int8]");
        }

        [Test]
        public void ARCompactRw_lp()
        {
            Given_HexString("20A80180"); // lp	000008E0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|LP_START = 00100004",
                "2|L--|LP_END = 0010000C");
        }

        [Test]
        public void ARCompactRw_lpne()
        {
            Given_HexString("20E804A2"); // lpne	000008B8
            AssertCode(
                "0|L--|00100000(4): 3 instructions",
                "1|T--|if (Test(EQ,Z)) branch 00100004",
                "2|L--|LP_START = 00100004",
                "3|L--|LP_END = 00100024");
        }

        [Test]
        public void ARCompactRw_lr()
        {
            Given_HexString("202A0F8012345678"); // lr	r0,[65603]
            AssertCode(
                "0|L--|00100000(8): 1 instructions",
                "1|L--|r0 = __load_aux_reg(0x12345678)");
        }

        [Test]
        public void ARCompactRw_lsr()
        {
            Given_HexString("2A41008D"); // lsr	r13,r2,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r13 = r2 >>u r2");
        }

        [Test]
        public void ARCompactRw_lsr_s()
        {
            Given_HexString("B823"); // lsr_s	r0,r0,00000003
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = r0 >>u 0x00000003");
        }

        [Test]
        public void ARCompactRw_max()
        {
            Given_HexString("2088706A"); // max	r56,r56,-0000057F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r56 = max(r56, -1407)");
        }

        [Test]
        public void ARCompactRw_min()
        {
            Given_HexString("23092640"); // min	r0,r19,r25
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = min(r19, r25)");
        }

        [Test]
        public void ARCompactRw_mov()
        {
            Given_HexString("230A3700"); // mov	fp,sp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = sp");
        }

        [Test]
        public void ARCompactRw_mov_s()
        {
            Given_HexString("7608"); // mov_s	r14,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r0");
        }

        [Test]
        public void ARCompactRw_mul64()
        {
            Given_HexString("2F840000"); // mul64	r7,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|mhi_mlo = r7 *s r0");
        }

        [Test]
        public void ARCompactRw_mul64_s()
        {
            Given_HexString("790C"); // mul64_s	r1,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|mhi_mlo = r1 *s r0");
        }

        [Test]
        public void ARCompactRw_mulu64()
        {
            Given_HexString("2845007E"); // mulu64	r0,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|mhi_mlo = r0 *u r1");
        }

        [Test]
        public void ARCompactRw_neg_s()
        {
            Given_HexString("7813"); // neg_s	r0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = -r0");
        }

        [Test]
        public void ARCompactRw_nop()
        {
            Given_HexString("78E0"); // nop
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void ARCompactRw_not()
        {
            Given_HexString("252F808A"); // not.f	r5,r2
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = ~r2",
                "2|L--|ZN = cond(r5)");
        }

        [Test]
        public void ARCompactRw_or()
        {
            Given_HexString("2005A000"); // or.f	r0,r16,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = r16 | r0",
                "2|L--|ZN = cond(r0)");
        }

        [Test]
        public void ARCompactRw_or_s()
        {
            Given_HexString("7EA5"); // or_s	r14,r14,r13
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r14 | r13");
        }

        [Test]
        public void ARCompactRw_pop_s()
        {
            Given_HexString("C0D1"); // pop_s	blink
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|blink = Mem0[sp:word32]",
                "2|L--|sp = sp + 4");
        }

        [Test]
        public void ARCompactRw_push_s()
        {
            Given_HexString("C5E1"); // push_s	r13
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|sp = sp - 4",
                "2|L--|Mem0[sp:word32] = r13");
        }

        [Test]
        public void ARCompactRw_rcmp()
        {
            Given_HexString("208D13FC"); // rcmp	r8,-000000F1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ZNCV = cond(-241 - r8)");
        }

        [Test]
        public void ARCompactRw_ror()
        {
            Given_HexString("28430200"); // ror	r0,r0,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __ror(r0, r8)");
        }

        [Test]
        public void ARCompactRw_rsub()
        {
            Given_HexString("200E8080"); // rsub	r0,r0,r2
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = r2 - r0",
                "2|L--|ZNCV = cond(r0)");
        }

        [Test]
        public void ARCompactRw_sbc()
        {
            Given_HexString("20032204"); // sbc	r4,r16,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r16 - r8 - C");
        }

        [Test]
        public void ARCompactRw_sexb()
        {
            Given_HexString("202F2405"); // sexb	r16,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r16 = (int32) SLICE(r16, int8, 0)");
        }

        [Test]
        public void ARCompactRw_sexw_s()
        {
            Given_HexString("792E"); // sexw_s	r1,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r1 = (int32) SLICE(r1, int16, 0)");
        }

        [Test]
        public void ARCompactRw_sr()
        {
            Given_HexString("212B0000"); // sr	r1,[]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__store_aux_reg(0x00000000, r1)");
        }

        [Test]
        public void ARCompactRw_st()
        {
            Given_HexString("1BFCB000"); // st	r0,[fp,-4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[fp - 4:word32] = r0");
        }

        [Test]
        public void ARCompactRw_st_as()
        {
            Given_HexString("1B980018"); // st.as	r0,[r3,24]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r3 + 96:word32] = r0");
        }

        [Test]
        public void ARCompactRw_st_aw()
        {
            Given_HexString("1cfc b6c8"); // st.aw   fp,[sp,-4]
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|sp = sp - 4",
                "2|L--|Mem0[sp:word32] = fp");
        }

        [Test]
        public void ARCompactRw_st_s()
        {
            Given_HexString("A707"); // st_s	r0,[r15,28]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r15 + 28:word32] = r0");
        }

        [Test]
        public void ARCompactRw_stb_ab()
        {
            Given_HexString("1E011012"); // stb.ab	r0,[r14,1]
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r14:byte] = SLICE(r0, byte, 0)",
                "2|L--|r14 = r14 + 1");
        }

        [Test]
        public void ARCompactRw_stb_s()
        {
            Given_HexString("A820"); // stb_s	r1,[r0]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r0:byte] = SLICE(r1, byte, 0)");
        }

        [Test]
        public void ARCompactRw_stw()
        {
            Given_HexString("1A282004"); // stw	r0,[r18,40]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r18 + 40:word16] = SLICE(r0, word16, 0)");
        }

        [Test]
        public void ARCompactRw_stw_as()
        {
            Given_HexString("1857805C"); // stw.as	r1,[r0,-41]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[r0 - 164:word16] = SLICE(r1, word16, 0)");
        }

        [Test]
        public void ARCompactRw_stw_s()
        {
            Given_HexString("B6C8"); // stw_s	r14,[r14,16]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r14 + 16:word16] = SLICE(r14, word16, 0)");
        }

        [Test]
        public void ARCompactRw_sub()
        {
            Given_HexString("21020012"); // sub	r18,r1,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = r1 - r0");
        }

        [Test]
        public void ARCompactRw_sub_s()
        {
            Given_HexString("C1A1"); // sub_s	sp,sp,00000004
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|sp = sp - 0x00000004");
        }

        [Test]
        public void ARCompactRw_sub2()
        {
            Given_HexString("249830C1"); // sub2	sp,sp,+00000043
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|sp = sp - (67 << 0x02)");
        }

        [Test]
        public void ARCompactRw_sub3()
        {
            Given_HexString("20192104"); // sub3	r4,r16,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r4 = r16 - (r4 << 0x03)");
        }

        [Test]
        public void ARCompactRw_subdw()
        {
            Given_HexString("282F0341"); // subsdw	r1,r0,r13
            AssertCode(
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v2 = r0",
                "2|L--|v3 = r13",
                "3|L--|r1 = __subsdw(v2, v3)",
                "4|L--|ZNV = cond(r1)",
                "5|L--|S = cond(r1)");
        }

        [Test]
        public void ARCompactRw_trap_s()
        {
            Given_HexString("795E"); // trap_s
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|__syscall()");
        }

        [Test]
        public void ARCompactRw_trap0()
        {
            Given_HexString("226F003F"); // trap0
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|__syscall()");
        }

        [Test]
        public void ARCompactRw_tst()
        {
            Given_HexString("230B2140"); // tst	r19,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|ZN = cond(r19 & r5)");
        }

        [Test]
        public void ARCompactRw_xor()
        {
            Given_HexString("24071F29"); // xor	r41,r12,lp_count
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r41 = r12 ^ lp_count");
        }

        [Test]
        public void ARCompactRw_xor_s()
        {
            Given_HexString("7B07"); // xor_s	r3,r3,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r3 = r3 ^ r0");
        }


        
    }
}
