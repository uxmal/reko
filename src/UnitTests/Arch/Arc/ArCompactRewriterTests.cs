#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
        private MemoryArea image;

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

        protected override MemoryArea RewriteCode(string hexBytes)
        {
            var bytes = HexStringToBytes(hexBytes);
            this.image = new MemoryArea(LoadAddress, bytes);
            return image;
        }

        protected override IEnumerable<RtlInstructionCluster> GetRtlStream(IStorageBinder binder, IRewriterHost host)
        {
            var state = new ARCompactState(arch);
            var rdr = arch.CreateImageReader(image, 0);
            return new ARCompactRewriter(arch, rdr, state, binder, host);
        }

        [Test]
        public void ARCompactRw_not()
        {
            RewriteCode("252F808A"); // not.f	r5,r2
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r5 = ~r2",
                "2|L--|ZN = cond(r5)");
        }

        [Test]
        public void ARCompactRw_push_s()
        {
            RewriteCode("C5E1"); // push_s	r13
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|sp = sp - 4",
                "2|L--|Mem0[sp:word32] = r13");
        }

        [Test]
        public void ARCompactRw_st()
        {
            RewriteCode("1BFCB000"); // st	r0,[fp,-4]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|Mem0[fp - 4:word32] = r0");
        }

        [Test]
        public void ARCompactRw_st_aw()
        {
            RewriteCode("1cfc b6c8"); // st.aw   fp,[sp,-4]
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|sp = sp - 4",
                "2|L--|Mem0[sp:word32] = fp");
        }

        [Test]
        public void ARCompactRw_stw_s()
        {
            RewriteCode("B6C8"); // stw_s	r14,[r14,16]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|Mem0[r14 + 16:word16] = SLICE(r14, word16, 0)");
        }

        [Test]
        public void ARCompactRw_mov()
        {
            RewriteCode("230A3700"); // mov	fp,sp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|fp = sp");
        }

        [Test]
        public void ARCompactRw_mov_s()
        {
            RewriteCode("7608"); // mov_s	r14,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r14 = r0");
        }

        [Test]
        public void ARCompactRw_bl()
        {
            RewriteCode("0B0A0000"); // bl	00000308
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|call 00100308 (0)");
        }

        [Test]
        public void ARCompactRw_bgt()
        {
            RewriteCode("000070C9"); // bgt	000E1814
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GT,ZNV)) branch 001E1800");
        }

        [Test]
        public void ARCompactRw_bge()
        {
            RewriteCode("0300240A"); // bge	0004831C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (Test(GE,ZN)) branch 00148300");
        }

        [Test]
        public void ARCompactRw_ld_ab()
        {
            RewriteCode("1404341B"); // ld.ab	fp,[sp,4]
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|fp = Mem0[sp:word32]",
                "2|L--|sp = sp + 4");
        }

        [Test]
        public void ARCompactRw_pop_s()
        {
            RewriteCode("C0D1"); // pop_s	blink
            AssertCode(
                "0|L--|00100000(2): 2 instructions",
                "1|L--|blink = Mem0[sp:word32]",
                "2|L--|sp = sp + 4");
        }

        [Test]
        public void ARCompactRw_j_s_blink()
        {
            RewriteCode("7EE0"); // j_s	[blink]
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|return (0,0)");
        }

        [Test]
        public void ARCompactRw_beq_d()
        {
            RewriteCode("0000C5E1"); // beq.d	FFF8B82C
            AssertCode(
                "0|TD-|00100000(4): 1 instructions",
                "1|TD-|if (Test(EQ,Z)) branch 0008B800");
        }

        [Test]
        public void ARCompactRw_sub_s()
        {
            RewriteCode("C1A1"); // sub_s	sp,sp,00000004
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|sp = sp - 0x00000004");
        }

        [Test]
        public void ARCompactRw_bic()
        {
            RewriteCode("2006D7F8"); // bic	r56,r40,blink
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r56 = r40 & ~blink",
                "2|L--|ZN = cond(r56)");
        }

        [Test]
        public void ARCompactRw_ld_pcl()
        {
            RewriteCode("D7F8"); // ld_s	r15,[pcl,992]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r15 = Mem0[0x001003E0:word32]");
        }

        [Test]
        public void ARCompactRw_ldw_s()
        {
            RewriteCode("910B"); // ldw_s	r0,[r1,22]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = (word32) Mem0[r1 + 22:word16]");
        }

        [Test]
        public void ARCompactRw_breq()
        {
            RewriteCode("0DFFE080"); // breq	r53,r2,0000004E
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r53 == r2) branch 000FFFFE");
        }

        [Test]
        public void ARCompactRw_bleq()
        {
            RewriteCode("0F800001"); // bleq	000007DC
            AssertCode(
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100004",
                "2|T--|call 00100780 (0)");
        }

        [Test]
        public void ARCompactRw_bne_s()
        {
            RewriteCode("F403"); // bne_s	0000005A
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|if (Test(NE,Z)) branch 00100006");
        }

        [Test]
        public void ARCompactRw_b()
        {
            RewriteCode("00010043"); // b	0060085C
            AssertCode(
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 00700800");
        }

        [Test]
        public void ARCompactRw_b_s()
        {
            RewriteCode("F013"); // b_s	0000007E
            AssertCode(
                "0|T--|00100000(2): 1 instructions",
                "1|T--|goto 00100026");
        }

        [Test]
        public void ARCompactRw_cmp_s()
        {
            RewriteCode("E080"); // cmp_s	r0,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|ZNCV = cond(r0 - 0x00000000)");
        }

        [Test]
        public void ARCompactRw_lr()
        {
            RewriteCode("202A0F8012345678"); // lr	r0,[65603]
            AssertCode(
                "0|L--|00100000(8): 1 instructions",
                "1|L--|r0 = __load_aux_reg(0x12345678)");
        }

        [Test]
        public void ARCompactRw_or()
        {
            RewriteCode("2005A000"); // or.f	r0,r16,r0
            AssertCode(
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r0 = r16 | r0");
        }

        [Test]
        public void ARCompactRw_bmsk_s()
        {
            RewriteCode("B8C0"); // bmsk_s	r0,r0,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|r0 = __bitmask(r0, 0x00000000)");
        }

        //////////////////////////////////////////////////////////////////

#if BORED
        [Test]
        public void ARCompactRw_add()
        {
            RewriteCode("27C07708"); // add.vc	pcl,pcl,sp
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add_s()
        {
            RewriteCode("E043"); // add_s	r0,r0,00000043
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_lsr_s()
        {
            RewriteCode("B823"); // lsr_s	r0,r0,00000003
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_beq_s()
        {
            RewriteCode("F205"); // beq_s	0000013E
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bmsk()
        {
            RewriteCode("20530001"); // bmsk	r1,r0,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bss()
        {
            RewriteCode("00027150"); // bss	000E2922
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_ldb()
        {
            RewriteCode("13FEB0C0"); // ldb.x	r0,[fp,-2]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_extb_s()
        {
            RewriteCode("780F"); // extb_s	r0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sub()
        {
            RewriteCode("21020012"); // sub	r18,r1,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bhs_s()
        {
            RewriteCode("F747"); // bhs_s	00000182
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bpl()
        {
            RewriteCode("0000D803"); // bpl	FFFB017C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bhi()
        {
            RewriteCode("004C000D"); // bhi	000001D0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_asl_s()
        {
            RewriteCode("6E12"); // asl_s	r0,r14,00000002
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sr()
        {
            RewriteCode("212B0000"); // sr	r1,[]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bhi_s()
        {
            RewriteCode("F738"); // bhi_s	00000188
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_st_s()
        {
            RewriteCode("A707"); // st_s	r0,[r15,28]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bclr_s()
        {
            RewriteCode("B9A1"); // bclr_s	r1,r1,00000001
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bset_s()
        {
            RewriteCode("B982"); // bset_s	r1,r1,00000002
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_asl()
        {
            RewriteCode("2F800000"); // asl	r0,r7,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bsc()
        {
            RewriteCode("00008011"); // bsc	FFF001D4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bne()
        {
            RewriteCode("00008702"); // bne	FFF0E208
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bvs()
        {
            RewriteCode("00008707"); // bvs	FFF0E22C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_adc()
        {
            RewriteCode("2001E180"); // adc	r0,r48,r6
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blt_s()
        {
            RewriteCode("F683"); // blt_s	0000025A
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_and()
        {
            RewriteCode("20440401"); // and	r1,r0,00000010
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_jl()
        {
            RewriteCode("20220F80"); // jl.d	[537286000]
            AssertCode(
                "0|L--|00100000(8): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blcc()
        {
            RewriteCode("0F802006"); // blcc	00040A34
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }



        [Test]
        public void ARCompactRw_bl_s()
        {
            RewriteCode("FFFF"); // bl_s	00000334
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brhs()
        {
            RewriteCode("0FFF2005"); // brhs	r23,r0,0000043A
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blal()
        {
            RewriteCode("0F800000"); // blal	00000AC0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_flag()
        {
            RewriteCode("20690040"); // flag	r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_nop()
        {
            RewriteCode("78E0"); // nop
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_ldw()
        {
            RewriteCode("1080216A"); // ldw.x	r42,[r16]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_j()
        {
            RewriteCode("202007C0"); // j.d	[blink]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bmi()
        {
            RewriteCode("07C01404"); // bmi	00028BF0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bvc()
        {
            RewriteCode("00801408"); // bvc	000284B8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_mulu64()
        {
            RewriteCode("2845007E"); // mulu64	r0,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_ble()
        {
            RewriteCode("00C0140C"); // ble	000285D8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bcc()
        {
            RewriteCode("02802046"); // bcc	000411CC
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bcs()
        {
            RewriteCode("02802045"); // bcs	000411FC
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bls_s()
        {
            RewriteCode("F7C5"); // bls_s	000007CE
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blo_s()
        {
            RewriteCode("F792"); // blo_s	000007F0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_stb()
        {
            RewriteCode("1E011012"); // stb.ab	r0,[r14,1]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_ldb_s()
        {
            RewriteCode("67A9"); // ldb_s	r1,[r15,0]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_stb_s()
        {
            RewriteCode("A820"); // stb_s	r1,[r0]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_or_s()
        {
            RewriteCode("7EA5"); // or_s	r14,r14,r13
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_lsr()
        {
            RewriteCode("2A41008D"); // lsr	r13,r2,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_jeq()
        {
            RewriteCode("20E007C1"); // jeq.d	[blink]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bpnz()
        {
            RewriteCode("0002242F"); // bpnz.d	00048886
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_lpne()
        {
            RewriteCode("20E804A2"); // lpne	000008B8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blmi()
        {
            RewriteCode("0A041104"); // blmi	00022A9C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brlo()
        {
            RewriteCode("0A051104"); // brlo	r10,r4,000008A0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_lp()
        {
            RewriteCode("20A80180"); // lp	000008E0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blgt()
        {
            RewriteCode("0A0407C9"); // blgt	00010300
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_btst_s()
        {
            RewriteCode("B8E0"); // btst_s	r0,00000000
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add3()
        {
            RewriteCode("25561C40"); // add3	r0,r13,00000031
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_cmp()
        {
            RewriteCode("244C8000"); // cmp	r4,00000000
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_btst()
        {
            RewriteCode("261190C0"); // btst	r14,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blt()
        {
            RewriteCode("0204202B"); // blt.d	00040CF0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brlt()
        {
            RewriteCode("0FFFC0A2"); // brlt	r39,r2,00000B1A
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sbc()
        {
            RewriteCode("20032204"); // sbc	r4,r16,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bset()
        {
            RewriteCode("204F07C1"); // bset	r1,r0,0000001F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add2()
        {
            RewriteCode("21150003"); // add2	r3,r1,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add2_s()
        {
            RewriteCode("7915"); // add2_s	r1,r1,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blne()
        {
            RewriteCode("0F8C0002"); // blne	0000168C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bclr()
        {
            RewriteCode("27100000"); // bclr	r0,r7,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add1()
        {
            RewriteCode("2414004A"); // add1	r10,r4,r1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bge_s()
        {
            RewriteCode("F653"); // bge_s	00001196
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blcs()
        {
            RewriteCode("0F802005"); // blcs	00041A40
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brk_s()
        {
            RewriteCode("7FFF"); // brk_s
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_rcmp()
        {
            RewriteCode("208D13FC"); // rcmp	r8,-000000F1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_extb()
        {
            RewriteCode("202F2407"); // extb	r16,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_xor()
        {
            RewriteCode("24071F29"); // xor	r41,r12,lp_count
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_ble_s()
        {
            RewriteCode("F6D2"); // ble_s	00001648
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_extw_s()
        {
            RewriteCode("7ED0"); // extw_s	r14,r14
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_ror()
        {
            RewriteCode("28430200"); // ror	r0,r0,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bls()
        {
            RewriteCode("0000420E"); // bls	00085DCC
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_max()
        {
            RewriteCode("2088706A"); // max	r56,r56,-0000057F
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bgt_s()
        {
            RewriteCode("F60B"); // bgt_s	00001F52
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blvc()
        {
            RewriteCode("0C00F788"); // blvc	FFFF1380
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_stw()
        {
            RewriteCode("1A282004"); // stw	r0,[r18,40]
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_and_s()
        {
            RewriteCode("7D04"); // and_s	r13,r13,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_asr_s()
        {
            RewriteCode("691B"); // asr_s	r0,r1,00000003
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_rsub()
        {
            RewriteCode("200E8080"); // rsub	r0,r0,r2
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_jl_s()
        {
            RewriteCode("7A40"); // jl_s	[r2]
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blpl()
        {
            RewriteCode("0FC02843"); // blpl	00053A90
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brne_s()
        {
            RewriteCode("EBD4"); // brne_s	r3,+00000000,00003154
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bbit1()
        {
            RewriteCode("0F8371CF"); // bbit1	pcl,r7,0000329E
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_trap0()
        {
            RewriteCode("226F003F"); // trap0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_tst()
        {
            RewriteCode("230B2140"); // tst	r19,r5
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_min()
        {
            RewriteCode("23092640"); // min	r0,r19,r25
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_breq_s()
        {
            RewriteCode("E83E"); // breq_s	r0,+00000000,000036AC
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blle()
        {
            RewriteCode("0FC01BEC"); // blle.d	0003B658
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bic_s()
        {
            RewriteCode("78E6"); // bic_s	r0,r0,r15
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add1_s()
        {
            RewriteCode("7934"); // add1_s	r1,r1,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sub2()
        {
            RewriteCode("249830C1"); // sub2	sp,sp,+00000043
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_add3_s()
        {
            RewriteCode("7936"); // add3_s	r1,r1,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blge()
        {
            RewriteCode("0800F20A"); // blge	FFFE8790
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brge()
        {
            RewriteCode("0FC7B883"); // brge	blink,r34,00004D66
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bxor()
        {
            RewriteCode("25922053"); // bxor	r21,r21,+000004C1
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sub3()
        {
            RewriteCode("20192104"); // sub3	r4,r16,r4
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blvs()
        {
            RewriteCode("0F90F007"); // blvs	FFFE615C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_mul64()
        {
            RewriteCode("2F840000"); // mul64	r7,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_extw()
        {
            RewriteCode("202F2408"); // extw	r16,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_asr()
        {
            RewriteCode("2D421200"); // asr	r0,r13,r8
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_divaw()
        {
            RewriteCode("2C48F02B"); // divaw	r43,lp_count,r0
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_mul64_s()
        {
            RewriteCode("790C"); // mul64_s	r1,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_bllt()
        {
            RewriteCode("0FC0202B"); // bllt.d	0004A65C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_blpnz()
        {
            RewriteCode("094070CF"); // blpnz	000EB854
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_subsdw()
        {
            RewriteCode("282F0341"); // subsdw	r1,r0,r13
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_xor_s()
        {
            RewriteCode("7B07"); // xor_s	r3,r3,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_neg_s()
        {
            RewriteCode("7813"); // neg_s	r0,r0
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_brne()
        {
            RewriteCode("0F8101E1"); // brne	r7,r7,0000B99C
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sexw_s()
        {
            RewriteCode("792E"); // sexw_s	r1,r1
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_trap_s()
        {
            RewriteCode("795E"); // trap_s
            AssertCode(
                "0|L--|00100000(2): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_sexb()
        {
            RewriteCode("202F2405"); // sexb	r16,r16
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void ARCompactRw_addsdw()
        {
            RewriteCode("2F28E0D0"); // addsdw	r16,r55,r3
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|@@@");
        }

#endif

    }
}
