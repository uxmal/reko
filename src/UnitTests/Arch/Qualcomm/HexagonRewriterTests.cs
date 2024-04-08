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
using Reko.Arch.Qualcomm;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Qualcomm
{
    [TestFixture]
    public class HexagonRewriterTests : RewriterTestBase
    {
        private readonly HexagonArchitecture arch;
        private readonly Address addrLoad;

        public HexagonRewriterTests()
        {
            this.arch = new HexagonArchitecture(CreateServiceContainer(), "hexagon", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr32(0x00100000);
        }

        public override Address LoadAddress => addrLoad;

        public override IProcessorArchitecture Architecture => arch;

        [Test]
        public void HexagonRw_abs()
        {
            Given_HexString("044504F3CCC08E80");
            AssertCode(     // { r13:r12 = abs(r15:r14); r4 = add(r4,r5) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r13_r12 = abs<int32>(r15_r14)",
                "2|L--|r4 = r4 + r5");
        }

        [Test]
        public void HexagonRw_add()
        {
            Given_HexString("1DF8FDBF");
            AssertCode(     // { r29 = add(r29,FFFFFFC0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r29 = r29 + 0xFFFFFFC0<32>");
        }

        [Test]
        public void HexagonRw_add_pc_imm()
        {
            Given_HexString("0EC2496A");
            AssertCode(     // { r14 = add(PC,00000004) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r14 = 00100004");
        }

        [Test]
        public void HexagonRw_allocframe()
        {
            Given_HexString("01C09DA0");
            AssertCode(     // { allocframe(+00000008) }
                "0|L--|00100000(4): 5 instructions",
                "1|L--|v3 = r29 - 8<i32>",
                "2|L--|Mem0[v3:word32] = r30",
                "3|L--|Mem0[v3 + 4<i32>:word32] = r31",
                "4|L--|r30 = v3",
                "5|L--|r29 = v3 - 8<i32>");
        }

        [Test]
        public void HexagonRw_and()
        {
            Given_HexString("E2C30076");
            AssertCode(     // { r2 = and(r0,0000001F) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = r0 & 0x1F<32>");
        }

        [Test]
        public void HexagonRw_assign_immediate()
        {
            Given_HexString("00C02072");
            AssertCode(     // { r0.h = 0000 }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = SEQ(0<16>, SLICE(r0, word16, 0))");
        }

        [Test]
        public void HexagonRw_assign_rev()
        {
            Given_HexString("00C09D6E");
            AssertCode(     // { r0 = rev }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = rev");
        }

        [Test]
        public void HexagonRw_aslh()
        {
            Given_HexString("0440027022340328");
            AssertCode(     // { r2 = sxth(r2); r3 = 00000000; r4 = aslh(r2) }
                "0|L--|00100000(8): 3 instructions",
                "1|L--|r2 = CONVERT(SLICE(r2, int16, 0), int16, int32)",
                "2|L--|r3 = 0<32>",
                "3|L--|r4 = r2 << 0x10<8>");
        }

        [Test]
        public void HexagonRw_asrh()
        {
            Given_HexString("044425F102402E7075300128");
            AssertCode(     // { r5 = r7; r1 = 00000000; r2 = asrh(r14); r4 = or(r5,r4) }
                "0|L--|00100000(12): 4 instructions",
                "1|L--|r5 = r7",
                "2|L--|r1 = 0<32>",
                "3|L--|r2 = r14 >> 0x10<32>",
                "4|L--|r4 = r5 | r4");
        }

        [Test]
        public void HexagonRw_callr_predicated()
        {
            Given_HexString("00C02051");
            AssertCode(     // { if (!p0) callr	r0 }
                "0|T--|00100000(4): 2 instructions",
                "1|T--|if (p0) branch 00100004",
                "2|T--|call r0 (0)");
        }

        [Test]
        public void HexagonRw_clrbit()
        {
            Given_HexString("20D0C08C");
            AssertCode(     // { r0 = clrbit(r0,00000000) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = __clear_bit<word32,word32>(r0, 0<32>)");
        }

        [Test]
        public void HexagonRw_crswap()
        {
            Given_HexString("00C01D65");
            AssertCode(     // { crswap(r29,sgp0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__crswap(r29, sgp0)");
        }

        [Test]
        public void HexagonRw_cswi()
        {
            Given_HexString("20C00164");
            AssertCode(     // { cswi(r1) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__cswi(r1)");
        }

        [Test]
        public void HexagonRw_dfclass()
        {
            Given_HexString("50C080DC");
            AssertCode(     // { p0 = dfclass(r1:r0,00000002) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|p0 = __dfclass(r1_r0, 2<32>)");
        }

        [Test]
        public void HexagonRw_dfcmp__gt()
        {
            Given_HexString("2042E0D2DE68FF5C4174007E18D89D42");
            AssertCode(     // { if (p0.new) memw(r29+12) = r24; if (p0.new) r1 = 000000A2; if (!p0.new) jump:nt 00012F08; p0 = dfcmp.gt(r1:r0,r3:r2) }
                "0|T--|00100000(16): 4 instructions",
                "1|L--|Mem0[r29 + 12<i32>:word32] = r24",
                "2|L--|r1 = 0xA2<32>",
                "3|T--|if (p0) branch 000FFFBC",
                "4|L--|p0 = r1_r0 > r3_r2");
        }

        [Test]
        public void HexagonRw_extractu_imm_imm()
        {
            Given_HexString("81C2408D");
            AssertCode(     // { r1 = extractu(r0,00000002,00000012) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = CONVERT(SLICE(r0, ui18, 2), ui18, uint32)");
        }

        [Test]
        public void HexagonRw_isync()
        {
            Given_HexString("02C0C057");
            AssertCode(     // { isync }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__isync()");
        }

        [Test]
        public void HexagonRw_jump()
        {
            Given_HexString("AAFFFF59");
            AssertCode(     // { jump	00009C40 }
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto 000FFF54");
        }

        [Test]
        [Ignore("Sequencing NYI")]
        public void HexagonRw_jump_conditional_new()
        {
            Given_HexString("1040607054C00224C0FF9097");
            AssertCode(     // { if (cmp.eq(r16.new,00000000)) jump:nt	000070A0; r16 = r0 }
                "0|L--|00100000(12): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void HexagonRw_jumpr()
        {
            Given_HexString("00C09C52");
            AssertCode(     // { jumpr	r28 }
                "0|T--|00100000(4): 1 instructions",
                "1|T--|goto r28");
        }

        [Test]
        public void HexagonRw_l2fetch()
        {
            Given_HexString("004200000A40007800D092A6");
            AssertCode(     // { l2fetch(r18,r17:r16); r10 = 00008000 }
                "0|L--|00100000(12): 2 instructions",
                "1|L--|__l2fetch<word32,word64>(r18, r17_r16)",
                "2|L--|r10 = 0x8000<32>");
        }

        [Test]
        public void HexagonRw_l2kill()
        {
            Given_HexString("00C020A8");
            AssertCode(     // { l2kill }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__l2kill()");
        }

        [Test]
        public void HexagonRw_setbit()
        {
            Given_HexString("02D8C28C");
            AssertCode(     // { r2 = setbit(r2,00000010) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r2 = __set_bit<word32,word32>(r2, 0x10<32>)");
        }

        [Test]
        public void HexagonRw_sfadd()
        {
            Given_HexString("12D812EB");
            AssertCode(     // { r18 = sfadd(r18,r24) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r18 = r18 + r24");
        }

        [Test]
        public void HexagonRw_sfcmp__ge()
        {
            Given_HexString("0042F3C76AC8205C");
            AssertCode(     // { if (!p0.new) jump:nt 0001EDE0; p0 = sfcmp.ge(r19,r2) }
                "0|T--|00100000(8): 2 instructions",
                "1|T--|if (p0) branch 001000D4",
                "2|L--|p0 = r19 >= r2");
        }

        [Test]
        public void HexagonRw_sfcmp__gt()
        {
            Given_HexString("8056E2C7 1448205C 2167007E A3C09347");
            AssertCode(     // { if (!p0.new) r3 = memw(r19+20); if (p0.new) r1 = 00000039; if (!p0.new) jump:nt 00021708; p0 = sfcmp.gt(r2,r22) }
                "0|T--|00100000(16): 4 instructions",
                "1|L--|r3 = Mem0[r19 + 20<i32>:word32]",
                "2|L--|r1 = 0x39<32>",
                "3|T--|if (p0) branch 00100028",
                "4|L--|p0 = r2 > r22");
        }

        [Test]
        public void HexagonRw_sfmax()
        {
            Given_HexString("03438EEB585819C41140777017C09891");
            AssertCode(     // { r23 = memw(r24); r17 = r23; r24 = addasl(r24,r25,00000002); r3 = sfmax(r14,r3) }
                "0|L--|00100000(16): 4 instructions",
                "1|L--|r23 = Mem0[r24:word32]",
                "2|L--|r17 = r23",
                "3|L--|r24 = r24 + (r25 << 2<32>)",
                "4|L--|r3 = fmaxf(r14, r3)");
        }

        [Test]
        public void HexagonRw_sfmin()
        {
            Given_HexString("274386EBC15F0078A211A3F1");
            AssertCode(     // { memw(r18+8) = 00000001; memw(r18+12) = 00000001; r1 = 000000FE; r7 = sfmin(r6,r3) }
                "0|L--|00100000(12): 4 instructions",
                "1|L--|Mem0[r18 + 8<i32>:word32] = 1<32>",
                "2|L--|Mem0[r18 + 12<i32>:word32] = 1<32>",
                "3|L--|r1 = 0xFE<32>",
                "4|L--|r7 = fminf(r6, r3)");
        }

        [Test]
        public void HexagonRw_sfmpy()
        {
            Given_HexString("025356EB18C06070");
            AssertCode(     // { r24 = r0; r2 = sfmpy(r22,r19) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r24 = r0",
                "2|L--|r2 = r22 * r19");
        }

        [Test]
        public void HexagonRw_sfsub()
        {
            Given_HexString("21DA04EB");
            AssertCode(     // { r1 = sfsub(r4,r26) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r4 - r26");
        }



        [Test]
        public void HexagonRw_store_rr()
        {
            Given_HexString("101CF4EB");
            AssertCode(     // { allocframe(00000008); memd(r29+496) = r17:r16 }
                "0|L--|00100000(4): 6 instructions",
                "1|L--|v3 = r29 - 8<i32>",
                "2|L--|Mem0[v3:word32] = r30",
                "3|L--|Mem0[v3 + 4<i32>:word32] = r31",
                "4|L--|r30 = v3",
                "5|L--|r29 = v3 - 8<i32>",
                "6|L--|Mem0[r29 - 16<i32>:word64] = r17_r16");
        }

        [Test]
        public void HexagonRw_syncht()
        {
            Given_HexString("00C040A8");
            AssertCode(     // { syncht }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__syncht()");
        }

        [Test]
        public void HexagonRw_load()
        {
            Given_HexString("051E0C3E");
            AssertCode(  // { r19:r18 = memd(r29); r17:r16 = memd(r29+8) }
                "0|L--|00100000(4): 2 instructions",
                "1|L--|r19_r18 = Mem0[r29:word64]",
                "2|L--|r17_r16 = Mem0[r29 + 8<i32>:word64]");
        }

        [Test]
        public void HexagonRw_memd_postincM()
        {
            Given_HexString("2E40C09114E0C09D");
            AssertCode(     // { r21:r20 = memd(r0++m0); r15:r14 = memd(r0+8) }
                "0|L--|00100000(8): 3 instructions",
                "1|L--|r0 = r0 + m0",
                "2|L--|r21_r20 = Mem0[r0:word64]",
                "3|L--|r15_r14 = Mem0[r0 + 8<i32>:word64]");
        }

        [Test]
        public void HexagonRw_memd_postinc_mreg()
        {
            Given_HexString("2E40CB9114C0CB9D");
            AssertCode(     // { r21:r20 = memd(r11++m0); r15:r14 = memd(r11+8) }
                "0|L--|00100000(8): 3 instructions",
                "1|L--|r11 = r11 + m0",
                "2|L--|r21_r20 = Mem0[r11:word64]",
                "3|L--|r15_r14 = Mem0[r11 + 8<i32>:word64]");
        }



        [Test]
        public void HexagonRw_memw_locked()
        {
            Given_HexString("01C00092");
            AssertCode(     // { r1 = memw_locked(r0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = __memw_locked(r0)");
        }

        [Test]
        public void HexagonRw_oreq()
        {
            Given_HexString("C0C1418E");
            AssertCode(     // { r0 |= asl(r1,00000001) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = r0 | r1 << 1<32>");
        }

        //[Test]
        public void HexagonRw_Read_Write_register_pair()
        {
            Given_HexString("104001F5 301CF4EB");
            AssertCode(     // { allocframe(00000018); memd(r29+496) = r17:r16; r17:r16 = combine(r1,r0) }
                "0|L--|00100000(8): 7 instructions",
                "1|L--|v@@@");
        }

        [Test]
        public void HexagonRw_rte()
        {
            Given_HexString("00C0E057");
            AssertCode(     // { rte }
                "0|R--|00100000(4): 2 instructions",
                "1|L--|__rte()",
                "2|R--|return (0,0)");
        }
        [Test]
        public void HexagonRw_stop()
        {
            Given_HexString("00C06064");
            AssertCode(     // { stop(r0) }
                "0|H--|00100000(4): 1 instructions",
                "1|H--|__stop(r0)");
        }

        [Test]
        public void HexagonRw_store_postincr()
        {
            Given_HexString("08C0C0AB");
            AssertCode(     // { memd(r0++#8) = r1:r0 }
                "0|L--|00100000(4): 2 instructions",
                "1|L--|Mem0[r0:word64] = r1_r0",
                "2|L--|r0 = r0 + 8<i32>");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

        [Test]
        public void HexagonRw_call()
        {
            Given_HexString("327DFF5B20C00078");
            AssertCode(     // { r0 = 00000001; call	00006AF8 }
                "0|T--|00100000(8): 2 instructions",
                "1|L--|r0 = 1<32>",
                "2|T--|call 000FFA64 (0)");
        }

        [Test]
        public void HexagonRw_convert_d2df()
        {
            Given_HexString("6040EA80C6F8A6BF");
            AssertCode(     // { r6 = add(r6,FFFFFBC6); r1:r0 = convert_d2df(r11:r10) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r6 = r6 + 0xFFFFFBC6<32>",
                "2|L--|r1_r0 = CONVERT(r11_r10, int64, real64)");
        }

        [Test]
        public void HexagonRw_convert_df2sf()
        {
            Given_HexString("3CC00088");
            AssertCode(
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r28 = CONVERT(r1_r0, real64, real32)");
        }

        [Test]
        public void HexagonRw_convert_df2w()
        {
            Given_HexString("2240E088E0CF9475");
            AssertCode(     // { p0 = cmp.gtu(r20,0000007F); r2 = convert_df2w(r1:r0):chop }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|p0 = r20 >u 0x7F<32>",
                "2|L--|r2 = truncf(CONVERT(r1_r0, real64, int32))");
        }

        [Test]
        public void HexagonRw_convert_df2w_chop()
        {
            Given_HexString("2240E088E0CF9475");
            AssertCode(     // { p0 = cmp.gtu(r20,0000007F); r2 = convert_df2w(r1:r0):chop }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|p0 = r20 >u 0x7F<32>",
                "2|L--|r2 = truncf(CONVERT(r1_r0, real64, int32))");
        }

        [Test]
        public void HexagonRw_convert_uw2sf()
        {
            Given_HexString("034353EB3B40808BE148007802C07170");
            AssertCode(     // { r2 = r17; r1 = 00000047; r27 = convert_uw2sf(r0):chop; r3 = sfmpy(r19,r3) }
                "0|L--|00100000(16): 4 instructions",
                "1|L--|r2 = r17",
                "2|L--|r1 = 0x47<32>",
                "3|L--|r27 = truncf(CONVERT(r0, uint32, real32))",
                "4|L--|r3 = r19 * r3");
        }

        [Test]
        public void HexagonRw_convert_uw2sf_chop()
        {
            Given_HexString("034353EB3B40808BE148007802C07170");
            AssertCode(     // { r2 = r17; r1 = 00000047; r27 = convert_uw2sf(r0):chop; r3 = sfmpy(r19,r3) }
                "0|L--|00100000(16): 4 instructions",
                "1|L--|r2 = r17",
                "2|L--|r1 = 0x47<32>",
                "3|L--|r27 = truncf(CONVERT(r0, uint32, real32))",
                "4|L--|r3 = r19 * r3");
        }

        [Test]
        public void HexagonRw_convert_w2sf()
        {
            Given_HexString("065802ED0240428B0BC59DA1");
            AssertCode(     // { memw(r29+44) = r5; r2 = convert_w2sf(r2); r6 = mpyi(r2,r24) }
                "0|L--|00100000(12): 3 instructions",
                "1|L--|Mem0[r29 + 44<i32>:word32] = r5",
                "2|L--|r2 = CONVERT(r2, int32, real32)",
                "3|L--|r6 = r2 * r24");
        }

        [Test]
        public void HexagonRw_extract()
        {
            Given_HexString("E3C1E38D");
            AssertCode(     // { r3 = extract(r3,00000001,00000019) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r3 = CONVERT(SLICE(r3, ui25, 1), ui25, int32)");
        }

        [Test]
        public void HexagonRw_dfcmp__uo()
        {
            Given_HexString("6040E0D200C09F52");
            AssertCode(     // { jumpr	r31; p0 = dfcmp.uo(r1:r0,r1:r0) }
                "0|T--|00100000(8): 2 instructions",
                "1|R--|return (0,0)",
                "2|L--|p0 = isunordered(r1_r0, r1_r0)");
        }

        [Test]
        public void HexagonRw_convert_sf2df()
        {
            Given_HexString("00409C8400C09F52");
            AssertCode(     // { jumpr	r31; r1:r0 = convert_sf2df(r28) }
                "0|T--|00100000(8): 2 instructions",
                "1|R--|return (0,0)",
                "2|L--|r1_r0 = CONVERT(r28, real32, real64)");
        }

        [Test]
        public void HexagonRw_convert_sf2uwf()
        {
            Given_HexString("3A40638B82C1005A");
            AssertCode(     // { call 0002162C; r26 = convert_sf2uwf(r3) }
                "0|T--|00100000(8): 2 instructions",
                "1|T--|call 00100304 (0)",
                "2|L--|r26 = CONVERT(r3, real32, uint32)");
        }

        [Test]
        [Ignore("Conditional assignment")]
        public void HexagonRw_bitsclr()
        {
            Given_HexString("AE408C8001478C85A7E627F9");
            AssertCode(     // { if (!p1.new) r7 = or(r7,r6); p1 = bitsclr(r12,00000007); r15:r14 = neg(r13:r12) }
                "0|L--|00100000(12): 3 instructions",
                "1|L--|@@@",
                "2|L--|@@@",
                "3|L--|@@@");
        }

        [Test]
        public void HexagonRw_brkpt()
        {
            Given_HexString("00C0206C");
            AssertCode(     // { brkpt }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__brkpt()");
        }

        [Test]
        public void HexagonRw_tlbp()
        {
            Given_HexString("05C0816C");
            AssertCode(     // { r5 = tlbp(r1) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r5 = __tlbp(r1)");
        }

        [Test]
        public void HexagonRw_tlbw()
        {
            Given_HexString("00C0026C");
            AssertCode(     // { tlbw(r3:r2,r0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__tlbw(r3_r2, r0)");
        }

        [Test]
        public void HexagonRw_togglebit()
        {
            Given_HexString("435FC38CAE7FFF5900C0007F");
            AssertCode(     // { nop; jump	0000A604; r3 = togglebit(r3,0000001E) }
                "0|T--|00100000(12): 3 instructions",
                "1|L--|nop",
                "2|T--|goto 000FFF5C",
                "3|L--|r3 = __invert_bit<word32,word32>(r3, 0x1E<32>)");
        }

        [Test]
        public void HexagonRw_trap0()
        {
            Given_HexString("00C00054");
            AssertCode(     // { trap0(00000000) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__trap0(0<32>)");
        }

        [Test]
        [Ignore("Conditional assignments")]
        public void HexagonRw_dfcmp__eq()
        {
            Given_HexString("0042E0D21848005C81C321F9");
            AssertCode(     // { if (!p0) r1 = or(r1,r3); if (p0.new) jump:nt	0000A83C; p0 = dfcmp.eq(r1:r0,r3:r2) }
                "0|T--|00100000(12): 3 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void HexagonRw_lsl()
        {
            Given_HexString("CC4A84C3CE4A8EC32BC00AB0");
            AssertCode(     // { r11 = add(r10,00000001); r15:r14 = lsl(r15:r14,r10); r13:r12 = lsl(r5:r4,r10) }
                "0|L--|00100000(12): 3 instructions",
                "1|L--|r11 = r10 + 1<32>",
                "2|L--|r15_r14 = r15_r14 << r10",
                "3|L--|r13_r12 = r5_r4 << r10");
        }

        [Test]
        public void HexagonRw_dcfetch()
        {
            Given_HexString("02478E8500415F5300C00194");
            AssertCode(     // { dcfetch	r1,00000000; if (p1) jumpr:nt	r31; p2 = bitsclr(r14,00000007) }
                "0|T--|00100000(12): 3 instructions",
                "1|L--|__dcfetch(r1, 0<32>)",
                "2|R--|return (0,0)",
                "3|L--|p2 = __bitsclr(r14, 7<32>)");
        }

        [Test]
        public void HexagonRw_dckill()
        {
            Given_HexString("00C000A2");
            AssertCode(     // { dckill }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dckill()");
        }

        [Test]
        public void HexagonRw_minu()
        {
            Given_HexString("9244A3D500C463F2");
            AssertCode(     // { p0 = cmp.gtu(r3,r4); r18 = minu(r3,r4) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|p0 = r3 >u r4",
                "2|L--|r18 = min<uint32>(r3, r4)");
        }

        [Test]
        public void HexagonRw_ciad()
        {
            Given_HexString("60C01A64");
            AssertCode(     // { ciad(r26) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ciad(r26)");
        }

        [Test]
        public void HexagonRw_cl0()
        {
            Given_HexString("4640408847404288044203F502C001F5");
            AssertCode(     // { r3:r2 = combine(r1,r0); r5:r4 = combine(r3,r2); r7 = cl0(r3:r2); r6 = cl0(r1:r0) }
                "0|L--|00100000(16): 4 instructions",
                "1|L--|r3_r2 = r1_r0",
                "2|L--|r5_r4 = r3_r2",
                "3|L--|r7 = __count_leading_zeros<word64>(r3_r2)",
                "4|L--|r6 = __count_leading_zeros<word64>(r1_r0)");
        }

        [Test]
        public void HexagonRw_clb()
        {
            Given_HexString("0740086A457A6C8864404476E3FF6F75");
            AssertCode(     // { p3 = cmp.gt(r15,FFFFFFFF); r4 = sub(00000003,r4); r5 = add(clb(r12),FFFFFFF4); r7 = USR }
                "0|L--|00100000(16): 4 instructions",
                "1|L--|p3 = r15 > 0xFFFFFFFF<32>",
                "2|L--|r4 = 3<32> - r4",
                "3|L--|r5 = __count_leading_bits<word32>(r12) + 0xFFFFFFF4<32>",
                "4|L--|r7 = USR");
        }


        [Test]
        public void HexagonRw_cmpb__eq()
        {
            Given_HexString("404110DD 21C000B0");
            AssertCode(     // { r1 = add(r0,00000001); p0 = cmpb.eq(r16,0A) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r1 = r0 + 1<32>",
                "2|L--|p0 = SLICE(r16, byte, 0) == SLICE(0xA<8>, byte, 0)");
        }

        [Test]
        public void HexagonRw_cmpb__gtu()
        {
            Given_HexString("E04146DDCE48005C2860887406EC8670");
            AssertCode(     // { if (!p0.new) r6 = zxtb(r6); if (!p0.new) r8 = add(r8,00000001); if (p0.new) jump:nt	00008EF0; p0 = cmpb.gtu(r6,0F) }
                "0|T--|00100000(16): 4 instructions",
                "1|L--|r6 = CONVERT(SLICE(r6, byte, 0), byte, uint32)",
                "2|L--|r8 = r8 + 1<32>",
                "3|T--|if (p0) branch 0010019C",
                "4|L--|p0 = SLICE(r6, byte, 0) >u SLICE(0xF<8>, byte, 0)");
        }

        [Test]
        public void HexagonRw_dfcmp__ge()
        {
            Given_HexString("4040E4D21AD8005C");
            AssertCode(     // { if (p0.new) jump:t	00008C64; p0 = dfcmp.ge(r5:r4,r1:r0) }
                "0|T--|00100000(8): 2 instructions",
                "1|T--|if (p0) branch 00100034",
                "2|L--|p0 = r5_r4 >= r1_r0");
        }

        [Test]
        public void HexagonRw_fastcorner9()
        {
            Given_HexString("0040016B0E42005C5661007E16E2807E");
            AssertCode(     // { if (!p0.new) r22 = 00000010; if (p0.new) r22 = 0000000A; if (p2) jump:nt	00008170; p0 = fastcorner9(p1,p0) }
                "0|T--|00100000(16): 4 instructions",
                "1|L--|r22 = 0x10<32>",
                "2|L--|r22 = 0xA<32>",
                "3|T--|if (p2) branch 0010001C",
                "4|L--|p0 = __fastcorner9(p1, p0)");
        }

        [Test]
        public void HexagonRw_mpy()
        {
            Given_HexString("244010ED 25C110B0");
            AssertCode(     // { r5 = add(r16,00000009); r4 = mpy(r16,r0) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r5 = r16 + 9<32>",
                "2|L--|r4 = SLICE(r16 *64 r0, word32, 32)");
        }

        [Test]
        public void HexagonRw_mpy_l_l()
        {
            Given_HexString("064608EC");
            AssertCode(
                "0|---|00100000(4): 1 instructions",
                "1|L--|r6 = SLICE(SLICE(r8, word16, 0) *64 SLICE(r6, word16, 0), word16, 32)");
        }

        [Test]
        public void HexagonRw_sxth()
        {
            Given_HexString("034123F1 01C0E270");
            AssertCode(     // { r1 = sxth(r2); r3 = or(r3,r1) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r1 = CONVERT(SLICE(r2, int16, 0), int16, int32)",
                "2|L--|r3 = r3 | r1");
        }

        [Test]
        public void HexagonRw_sxtw()
        {
            Given_HexString("00C05684");
            AssertCode(     // { r1:r0 = sxtw(r22) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1_r0 = CONVERT(SLICE(r22, int32, 0), int32, int64)");
        }

        [Test]
        public void HexagonRw_mpyi()
        {
            Given_HexString("C2C101DF");
            AssertCode(     // { r1 = add(r2,mpyi(00000018,r1)) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r1 = r2 + 0x18<32> * r1");
        }

        [Test]
        public void HexagonRw_load_indexed()
        {
            Given_HexString("00E0813A");
            AssertCode(     // { r0 = memw(r30+r0<<#2) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r0 = Mem0[r30 + r0 * 4<32>:word32]");
        }

        [Test]
        [Ignore("Conditional assignment")]
        public void HexagonRw_NOT()
        {
            Given_HexString(/*"10400275 10600274*/"10E08374");
            AssertCode(     // { if (!p0.new) r16 = add(r3,00000000); if (p0.new) r16 = add(r2,00000000); p0 = !cmp.eq(r2,00000000) }
                "0|L--|00100000(12): 1 instructions",
                "1|L--|@@@");
        }

        [Test]
        public void HexagonRw_any8()
        {
            Given_HexString("006604D20058205C0261827404C2C033");
            AssertCode(     // { if (!p0.new) r5:r4 = memd(r0+r2); if (!p0.new) r2 = add(r2,00000008); if (!p0.new) jump:t	00007704; p0 = any8(vcmpb.eq(r5:r4,r7:r6)) }
                "0|T--|00100000(16): 4 instructions",
                "1|L--|r5_r4 = Mem0[r0 + r2:word64]",
                "2|L--|r2 = r2 + 8<32>",
                "3|T--|if (p0) branch 00100000",
                "4|L--|p0 = __any8(vcmp__eq<byte[8]>(r5_r4, r7_r6))");
        }

        [Test]
        public void HexagonRw_ickill()
        {
            Given_HexString("00D0C056");
            AssertCode(     // { ickill }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__ickill()");
        }

        [Test]
        public void HexagonRw_mpyu()
        {
            Given_HexString("064D44E5C4C2E883");
            AssertCode(     // { r5:r4 = insert(r8,00000002,0000003A); r7:r6 = mpyu(r4,r13) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r5_r4 = __insert(r8, 2<32>, 0x3A<32>)",
                "2|L--|r7_r6 = r4 *u64 r13");
        }

        [Test]
        public void HexagonRw_neg()
        {
            Given_HexString("AE408A8026471CEF1CC361F1");
            AssertCode(     // { r28 = xor(r1,r3); r6 += add(r28,r7); r15:r14 = neg(r11:r10) }
                "0|L--|00100000(12): 3 instructions",
                "1|L--|r28 = r1 ^ r3",
                "2|L--|r6 = r6 + (r28 + r7)",
                "3|L--|r15_r14 = -r11_r10");
        }


        [Test]
        public void HexagonRw_ct0()
        {
            Given_HexString("8440448C02FFE2BF");
            AssertCode(     // { r2 = add(r2,FFFFFFF8); r4 = ct0(r4) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r2 = r2 + 0xFFFFFFF8<32>",
                "2|L--|r4 = __count_trailing_zeros<word32>(r4)");
        }

        [Test]
        [Ignore("Sequencer")]
        public void HexagonRw_maxu()
        {
            //$BUG: sequence this so jump is last instr.
            Given_HexString("9151C0D508C00058");
            AssertCode(     // { jump	00007254; r17 = maxu(r0,r17) }
                "0|T--|00100000(8): 2 instructions",
                "1|L--|r17 = maxu(r0, r17)",
                "2|L--|goto 00007254");
        }

        [Test]
        public void HexagonRw_dcinva()
        {
            Given_HexString("00C020A0");
            AssertCode(     // { dcinva(r0) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__dcinva(r0)");
        }

        [Test]
        public void HexagonRw_EQ()
        {
            Given_HexString("16C08361");
            AssertCode(     // { if (EQ(r3,00000000)) jump:nt	00005430 }
                "0|T--|00100000(4): 1 instructions",
                "1|T--|if (r3 == 0<32>) branch 0010002C");
        }

        [Test]
        public void HexagonRw_NE()
        {
            Given_HexString("3240016123C5028C");
            AssertCode(     // { r3 = lsr(r2,00000005); if (NE(r1,00000000)) jump:nt	00005494 }
                "0|T--|00100000(8): 2 instructions",
                "1|L--|r3 = r2 >>u 5<32>",
                "2|T--|if (r1 != 0<32>) branch 00100064");
        }

        [Test]
        public void HexagonRw_dczeroa()
        {
            Given_HexString("068406B0027CE2BF00C0C6A0");
            AssertCode(     // { dczeroa(r6); r2 = add(r2,FFFFFFE0); r6 = add(r6,00000020) }
                "0|L--|00100000(12): 3 instructions",
                "1|L--|__dczeroa(r6)",
                "2|L--|r2 = r2 + 0xFFFFFFE0<32>",
                "3|L--|r6 = r6 + 0x20<32>");
        }

        [Test]
        public void HexagonRw_start()
        {
            Given_HexString("20C06864");
            AssertCode(     // { start(r8) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|__start(r8)");
        }

        [Test]
        public void HexagonRw_vaslw()
        {
            Given_HexString("40425880 4440CE10 02C0007C");
            AssertCode(     // { r3:r2 = combine(00000000,00000000); if (!p0.new) jump:nt 00020400; p0 = cmp.gt(r14,00000000); r1:r0 = vaslw(r25:r24,00000002) }
                "0|T--|00100000(12): 4 instructions",
                "1|L--|r3_r2 = 0<64>",
                "2|T--|if (p0) branch 00100088",
                "3|L--|p0 = r14 > 0<32>",
                "4|L--|r1_r0 = __simd_shl<word64,word32>(r25_r24, 2<32>)");
        }

        [Test]
        public void HexagonRw_vavgh()
        {
            Given_HexString("004004854C410E8004C41CF7");
            AssertCode(     // { r4 = vavgh(r28,r4); r13:r12 = asl(r15:r14,00000001); p0 = tstbit(r4,00000000) }
                "0|L--|00100000(12): 3 instructions",
                "1|L--|r4 = vavgh<word16[2]>(r28, r4)",
                "2|L--|r13_r12 = r15_r14 << 1<32>",
                "3|L--|p0 = __bit<word32,word32>(r4, 0<32>)");
        }

        [Test]
        public void HexagonRw_vcmpb__eq()
        {
            Given_HexString("C0C604D2");
            AssertCode(     // { p0 = vcmpb.eq(r5:r4,r7:r6) }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|p0 = vcmp__eq<byte[8]>(r5_r4, r7_r6)");
        }

        [Test]
        public void HexagonRw_vmux()
        {
            Given_HexString("004800D102C602D1");
            AssertCode(     // { r3:r2 = vmux(p0,r3:r2,r7:r6); r1:r0 = vmux(p0,r1:r0,r9:r8) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r3_r2 = __vmux<byte[8]>(p0, r3_r2, r7_r6)",
                "2|L--|r1_r0 = __vmux<byte[8]>(p0, r1_r0, r9_r8)");
        }

        [Test]
        public void HexagonRw_vraddub()
        {
            Given_HexString("20544EEA1C481CF3A6429D9100C01C94");
            AssertCode(     // { dcfetch(r28,00000000); r6 = memw(r29+84); r28 = add(r28,r8); r1:r0 += vraddub(r15:r14,r21:r20) }
                "0|L--|00100000(16): 4 instructions",
                "1|L--|__dcfetch(r28, 0<32>)",
                "2|L--|r6 = Mem0[r29 + 84<i32>:word32]",
                "3|L--|r28 = r28 + r8",
                "4|L--|r1_r0 = r1_r0 + __vraddub<word64,word64>(r15_r14, r21_r20)");
        }

        [Test]
        public void HexagonRw_vsplatb()
        {
            Given_HexString("E7C0418C");
            //Given_HexString("80408261E740418C06C06070");
            AssertCode(     // { r6 = r0; r7 = vsplatb(r1); if (EQ(r2,00000000)) jump:nt	00005490 }
                "0|L--|00100000(4): 1 instructions",
                "1|L--|r7 = __vsplatb<byte[4]>(r1)");
        }
        [Test]
        public void HexagonRw_vsubh()
        {
            Given_HexString("014405F203C183F6");
            AssertCode(     // { r3 = vsubh(r1,r3); p1 = cmp.eq(r5,r4) }
                "0|L--|00100000(8): 2 instructions",
                "1|L--|r3 = __simd_sub<int16[2]>(r1, r3)",
                "2|L--|p1 = r5 == r4");
        }
    }
}
