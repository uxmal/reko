#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Arch.Loongson;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Loongson
{
    [TestFixture]
    public class LoongArchRewriterTests : RewriterTestBase
    {
        public LoongArchRewriterTests()
        {
            this.Architecture = new LoongArch(CreateServiceContainer(), "loongArch", new()
            {
                { ProcessorOption.WordSize, 64 }
            });
            this.LoadAddress = Address.Ptr64(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }


        [Test]
        public void LoongArchRw_add_d()
        {
            Given_HexString("8CB51000");
            AssertCode(     // add.d	$r12,$r12,$r13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = r12 + r13");
        }

        [Test]
        public void LoongArchRw_add_w()
        {
            Given_HexString("A5181000");
            AssertCode(     // add.w	$r5,$r5,$r6
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r5, word32, 0)",
                "2|L--|v6 = SLICE(r6, word32, 0)",
                "3|L--|v7 = v4 + v6",
                "4|L--|r5 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_addi_d()
        {
            Given_HexString("2C30C002");
            AssertCode(     // addi.d	$r12,$r1,+0000000C
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = r1 + 0xC<64>");
        }

        [Test]
        public void LoongArchRw_addi_w()
        {
            Given_HexString("A5048002");
            AssertCode(     // addi.w	$r5,$r5,+00000001
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r5, word32, 0)",
                "2|L--|v5 = v4 + 1<i32>",
                "3|L--|r5 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void LoongArchRw_alsl_d()
        {
            Given_HexString("8C652D00");
            AssertCode(     // alsl.d	$r12,$r12,$r25,+00000002
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = (r12 << 3<8>) + r25");
        }

        [Test]
        public void LoongArchRw_alsl_w()
        {
            Given_HexString("360C0400");
            AssertCode(     // alsl.w	$r22,$r1,$r3,+00000000
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r1, word32, 0)",
                "2|L--|v6 = SLICE(r3, word32, 0)",
                "3|L--|v8 = (v4 << 1<8>) + v6",
                "4|L--|r22 = CONVERT(v8, word32, int64)");
        }

        [Test]
        public void LoongArchRw_alsl_wu()
        {
            Given_HexString("01000600");
            AssertCode(     // alsl.wu	$r0,$r0,$r0,+00000000
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = (0<32> << 1<8>) + 0<32>",
                "2|L--|r1 = CONVERT(v4, word32, word64)");
        }

        [Test]
        public void LoongArchRw_amadd_d()
        {
            Given_HexString("BC9B6138");
            AssertCode(     // amadd.d	$r28,$r6,$r29
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r28 = __atomic_add<word64>(r6, r29)");
        }

        [Test]
        public void LoongArchRw_amadd_db_d()
        {
            Given_HexString("7E826A38");
            AssertCode(     // amadd.db.d	$r30,$r0,$r19
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r30 = __atomic_add_db<word64>(0<64>, r19)");
        }

        [Test]
        public void LoongArchRw_amadd_db_w()
        {
            Given_HexString("CF536A38");
            AssertCode(     // amadd.db.w	$r15,$r20,$r30
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r30, word32, 0)",
                "2|L--|v7 = __atomic_add_db<word32>(r20, v5)",
                "3|L--|r15 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amadd_w()
        {
            Given_HexString("24576138");
            AssertCode(     // amadd.w	$r4,$r21,$r25
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r25, word32, 0)",
                "2|L--|v7 = __atomic_add<word32>(r21, v5)",
                "3|L--|r4 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amand_d()
        {
            Given_HexString("B5A56238");
            AssertCode(     // amand.d	$r21,$r9,$r13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r21 = __atomic_and<word64>(r9, r13)");
        }

        [Test]
        public void LoongArchRw_amand_db_w()
        {
            Given_HexString("50196B38");
            AssertCode(     // amand.db.w	$r16,$r6,$r10
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r10, word32, 0)",
                "2|L--|v7 = __atomic_and_db<word32>(r6, v5)",
                "3|L--|r16 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amand_w()
        {
            Given_HexString("EA606238");
            AssertCode(     // amand.w	$r10,$r24,$r7
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r7, word32, 0)",
                "2|L--|v7 = __atomic_and<word32>(r24, v5)",
                "3|L--|r10 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammax_d()
        {
            Given_HexString("08B16538");
            AssertCode(     // ammax.d	$r8,$r12,$r8
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r8 = __atomic_max<word64>(r12, r8)");
        }

        [Test]
        public void LoongArchRw_ammax_du()
        {
            Given_HexString("8AD16738");
            AssertCode(     // ammax.du	$r10,$r20,$r12
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r10 = __atomic_maxu<word64>(r20, r12)");
        }

        [Test]
        public void LoongArchRw_ammax_db_d()
        {
            Given_HexString("25A96E38");
            AssertCode(     // ammax.db.d	$r5,$r10,$r9
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r5 = __atomic_max_db<word64>(r10, r9)");
        }

        [Test]
        public void LoongArchRw_ammax_db_du()
        {
            Given_HexString("ACC07038");
            AssertCode(     // ammax.db.du	$r12,$r16,$r5
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = __atomic_maxu_db<word64>(r16, r5)");
        }

        [Test]
        public void LoongArchRw_ammax_db_w()
        {
            Given_HexString("9F0D6E38");
            AssertCode(     // ammax.db.w	$r31,$r3,$r12
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r12, word32, 0)",
                "2|L--|v7 = __atomic_max_db<word32>(r3, v5)",
                "3|L--|r31 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammax_db_wu()
        {
            Given_HexString("FB7F7038");
            AssertCode(     // ammax.db.wu	$r27,$r31,$r31
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r31, word32, 0)",
                "2|L--|v6 = __atomic_maxu_db<word32>(r31, v4)",
                "3|L--|r27 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammax_w()
        {
            Given_HexString("C3156538");
            AssertCode(     // ammax.w	$r3,$r5,$r14
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r14, word32, 0)",
                "2|L--|v7 = __atomic_max<word32>(r5, v5)",
                "3|L--|r3 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammax_wu()
        {
            Given_HexString("7D296738");
            AssertCode(     // ammax.wu	$r29,$r10,$r11
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r11, word32, 0)",
                "2|L--|v7 = __atomic_maxu<word32>(r10, v5)",
                "3|L--|r29 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammin_d()
        {
            Given_HexString("648B6638");
            AssertCode(     // ammin.d	$r4,$r2,$r27
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = __atomic_min<word64>(r2, r27)");
        }

        [Test]
        public void LoongArchRw_ammin_du()
        {
            Given_HexString("EBAF6838");
            AssertCode(     // ammin.du	$r11,$r11,$r31
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r11 = __atomic_minu<word64>(r11, r31)");
        }

        [Test]
        public void LoongArchRw_ammin_db_d()
        {
            Given_HexString("2FB46F38");
            AssertCode(     // ammin.db.d	$r15,$r13,$r1
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r15 = __atomic_min_db<word64>(r13, r1)");
        }

        [Test]
        public void LoongArchRw_ammin_db_du()
        {
            Given_HexString("90D67138");
            AssertCode(     // ammin.db.du	$r16,$r21,$r20
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r16 = __atomic_minu_db<word64>(r21, r20)");
        }

        [Test]
        public void LoongArchRw_ammin_db_w()
        {
            Given_HexString("5A596F38");
            AssertCode(     // ammin.db.w	$r26,$r22,$r10
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r10, word32, 0)",
                "2|L--|v7 = __atomic_minu_db<word32>(r22, v5)",
                "3|L--|r26 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammin_db_wu()
        {
            Given_HexString("9B337138");
            AssertCode(     // ammin.db.wu	$r27,$r12,$r28
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r28, word32, 0)",
                "2|L--|v7 = __atomic_minu_db<word32>(r12, v5)",
                "3|L--|r27 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammin_w()
        {
            Given_HexString("A55C6638");
            AssertCode(     // ammin.w	$r5,$r23,$r5
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r5, word32, 0)",
                "2|L--|v6 = __atomic_min<word32>(r23, v5)",
                "3|L--|r5 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ammin_wu()
        {
            Given_HexString("63256838");
            AssertCode(     // ammin.wu	$r3,$r9,$r11
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r11, word32, 0)",
                "2|L--|v7 = __atomic_minu<word32>(r9, v5)",
                "3|L--|r3 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amor_d()
        {
            Given_HexString("618F6338");
            AssertCode(     // amor.d	$r1,$r3,$r27
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r1 = __atomic_or<word64>(r3, r27)");
        }

        [Test]
        public void LoongArchRw_amor_db_d()
        {
            Given_HexString("94EC6C38");
            AssertCode(     // amor.db.d	$r20,$r27,$r4
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r20 = __atomic_or_db<word64>(r27, r4)");
        }

        [Test]
        public void LoongArchRw_amor_db_w()
        {
            Given_HexString("62346C38");
            AssertCode(     // amor.db.w	$r2,$r13,$r3
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r3, word32, 0)",
                "2|L--|v7 = __atomic_or_db<word32>(r13, v5)",
                "3|L--|r2 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amor_w()
        {
            Given_HexString("12566338");
            AssertCode(     // amor.w	$r18,$r21,$r16
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r16, word32, 0)",
                "2|L--|v7 = __atomic_or<word32>(r21, v5)",
                "3|L--|r18 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amswap_db_d()
        {
            Given_HexString("23D26938");
            AssertCode(     // amswap.db.d	$r3,$r20,$r17
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r3 = __atomic_swap_db<word64>(r20, r17)");
        }

        [Test]
        public void LoongArchRw_amswap_w()
        {
            Given_HexString("8F7E6038");
            AssertCode(     // amswap.w	$r15,$r31,$r20
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r20, word32, 0)",
                "2|L--|v7 = __atomic_swap<word32>(r31, v5)",
                "3|L--|r15 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amxor_d()
        {
            Given_HexString("17D96438");
            AssertCode(     // amxor.d	$r23,$r22,$r8
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r23 = __atomic_xor<word64>(r22, r8)");
        }

        [Test]
        public void LoongArchRw_amxor_db_d()
        {
            Given_HexString("F1E66D38");
            AssertCode(     // amxor.db.d	$r17,$r25,$r23
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r17 = __atomic_xor_db<word64>(r25, r23)");
        }

        [Test]
        public void LoongArchRw_amxor_db_w()
        {
            Given_HexString("3D686D38");
            AssertCode(     // amxor.db.w	$r29,$r26,$r1
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r1, word32, 0)",
                "2|L--|v7 = __atomic_xor_db<word32>(r26, v5)",
                "3|L--|r29 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_amxor_w()
        {
            Given_HexString("E1276438");
            AssertCode(     // amxor.w	$r1,$r9,$r31
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v5 = SLICE(r31, word32, 0)",
                "2|L--|v7 = __atomic_xor<word32>(r9, v5)",
                "3|L--|r1 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_and()
        {
            Given_HexString("ADB91400");
            AssertCode(     // and	$r13,$r13,$r14
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r13 = r13 & r14");
        }

        [Test]
        public void LoongArchRw_andi()
        {
            Given_HexString("8D0D4003");
            AssertCode(     // andi	$r13,$r12,00000003
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r13 = r12 & 3<64>");
        }

        [Test]
        public void LoongArchRw_andn()
        {
            Given_HexString("9CE71600");
            AssertCode(     // andn	$r28,$r28,$r25
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r28 = r28 & ~r25");
        }

        [Test]
        public void LoongArchRw_b()
        {
            Given_HexString("00000050");
            AssertCode(     // b	00000000
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|goto 0000000000100000");
        }

        [Test]
        public void LoongArchRw_beq()
        {
            Given_HexString("913D0058");
            AssertCode(     // beq	$r12,$r17,00062724
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r12 == r17) branch 000000000010003C");
        }

        [Test]
        public void LoongArchRw_beqz()
        {
            Given_HexString("75326441");
            AssertCode(     // beqz	FFD5747C
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r19 == 0<64>) branch FFFFFFFFFFE56430");
        }

        [Test]
        public void LoongArchRw_bge()
        {
            Given_HexString("8058FF67");
            AssertCode(     // bge	$r4,$r0,00064330
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r4 >= 0<64>) branch 00000000000FFF58");
        }

        [Test]
        public void LoongArchRw_bgeu()
        {
            Given_HexString("8549006C");
            AssertCode(     // bgeu	$r12,$r5,000642CC
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r12 >=u r5) branch 0000000000100048");
        }

        [Test]
        public void LoongArchRw_bitrev_d()
        {
            Given_HexString("14560000");
            AssertCode(     // bitrev.d	$r20,$r16
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r20 = __bitrev<word64>(r16)");
        }

        [Test]
        public void LoongArchRw_bitrev_w()
        {
            Given_HexString("44500000");
            AssertCode(     // bitrev.w	$r0,$r0
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r2, word32, 0)",
                "2|L--|v6 = __bitrev<word32>(v4)",
                "3|L--|r4 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_bl()
        {
            Given_HexString("00501554");
            AssertCode(     // bl	00062C54
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|call 0000000000101550 (0)");
        }

        [Test]
        public void LoongArchRw_blt()
        {
            Given_HexString("99358961");
            AssertCode(     // blt	$r12,$r25,0007894C
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r12 < r25) branch 0000000000118934");
        }

        [Test]
        public void LoongArchRw_bltu()
        {
            Given_HexString("A4300068");
            AssertCode(     // bltu	$r5,$r4,000642CC
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r5 <u r4) branch 0000000000100030");
        }

        [Test]
        public void LoongArchRw_bne()
        {
            Given_HexString("8FE1FF5F");
            AssertCode(     // bne	$r12,$r15,000616E0
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r12 != r15) branch 00000000000FFFE0");
        }

        [Test]
        public void LoongArchRw_bnez()
        {
            Given_HexString("A9852747");
            AssertCode(     // bnez	0027379C
                "0|T--|0000000000100000(4): 1 instructions",
                "1|T--|if (r13 != 0<64>) branch 0000000000372784");
        }

        [Test]
        public void LoongArchRw_break()
        {
            Given_HexString("07002A00");
            AssertCode(     // break	00000007
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|__break(7<64>)");
        }

        [Test]
        public void LoongArchRw_bstrpick_d()
        {
            Given_HexString("F4AAF000");
            AssertCode(     // bstrpick.d	$r20,$r23,00000030,0000002A
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r20 = __bstrpick<word64>(r23, 0x30<64>, 0x2A<64>)");
        }

        [Test]
        public void LoongArchRw_bytepick_d()
        {
            Given_HexString("42420F00");
            AssertCode(     // bytepick.d	$r2,$r18,$r16,+00000006
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r2 = __bytepick<word64>(r18, r16, 6<64>)");
        }

        [Test]
        public void LoongArchRw_bytepick_w()
        {
            Given_HexString("E4010800");
            AssertCode(     // bytepick.w	$r4,$r15,$r0,+00000000
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r15, word32, 0)",
                "2|L--|v6 = __bytepick<word32>(v4, 0<32>, 0<i32>)",
                "3|L--|r4 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_cacop()
        {
            Given_HexString("87E20006");
            AssertCode(     // cacop	00000007,$r20,+00000038
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|__cacop<word64>(7<64>, r20, 0x38<64>)");
        }

        [Test]
        public void LoongArchRw_clo_d()
        {
            Given_HexString("11200000");
            AssertCode(     // clo.d	$r0,$r0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r17 = __count_leading_ones<word64>(0<64>)");
        }

        [Test]
        public void LoongArchRw_clo_w()
        {
            Given_HexString("11100000");
            AssertCode(     // clo.w	$r0,$r0
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = __count_leading_ones<word32>(0<32>)",
                "2|L--|r17 = CONVERT(v4, word32, int64)");
        }

        [Test]
        public void LoongArchRw_clz_w()
        {
            Given_HexString("04140000");
            AssertCode(     // clz.w	$r4,$r0
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = __count_leading_zeros<word32>(0<32>)",
                "2|L--|r4 = CONVERT(v4, word32, int64)");
        }

        [Test]
        public void LoongArchRw_csrrd()
        {
            Given_HexString("0C800004");
            AssertCode(     // csrrd	$r12,00000020
                "0|S--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r12, word32, 0)",
                "2|L--|v5 = __csrrd<word32>(v4)",
                "3|L--|r12 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void LoongArchRw_csrwr()
        {
            Given_HexString("2C440004");
            AssertCode(     // csrwr	$r12,00000011
                "0|S--|0000000000100000(4): 1 instructions",
                "1|L--|__csrwr<word64>(0x11<64>, r12)");
        }

        [Test]
        public void LoongArchRw_div_d()
        {
            Given_HexString("EF412200");
            AssertCode(     // div.d	$r15,$r15,$r16
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r15 = r15 / r16");
        }

        [Test]
        public void LoongArchRw_div_du()
        {
            Given_HexString("8C142300");
            AssertCode(     // div.du	$r12,$r4,$r5
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = r4 /u r5");
        }

        [Test]
        public void LoongArchRw_div_w()
        {
            Given_HexString("6E3A2000");
            AssertCode(     // div.w	$r14,$r19,$r14
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r19, word32, 0)",
                "2|L--|v6 = SLICE(r14, word32, 0)",
                "3|L--|v7 = v4 / v6",
                "4|L--|r14 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_div_wu()
        {
            Given_HexString("21212100");
            AssertCode(     // div.wu	$r1,$r9,$r8
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r9, word32, 0)",
                "2|L--|v6 = SLICE(r8, word32, 0)",
                "3|L--|v8 = v4 /u v6",
                "4|L--|r1 = CONVERT(v8, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ext_w_b()
        {
            Given_HexString("845C0000");
            AssertCode(     // ext.w.b	$r4,$r4
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r4, int8, 0)",
                "2|L--|v5 = CONVERT(v4, int8, int32)",
                "3|L--|r4 = CONVERT(v5, int32, int64)");
        }

        [Test]
        public void LoongArchRw_ext_w_h()
        {
            Given_HexString("4D5A0000");
            AssertCode(     // ext.w.h	$r13,$r18
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r13, int16, 0)",
                "2|L--|v5 = CONVERT(v4, int16, int32)",
                "3|L--|r13 = CONVERT(v5, int32, int64)");
        }

        [Test]
        public void LoongArchRw_fabs_d()
        {
            Given_HexString("18091401");
            AssertCode(     // fabs.d	$f24,$f8
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f24 = fabs(f8)");
        }

        [Test]
        public void LoongArchRw_fadd_d()
        {
            Given_HexString("01000101");
            AssertCode(     // fadd.d	$f1,$f0,$f0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f1 = f0 + f0");
        }

        [Test]
        public void LoongArchRw_fcopysign_d()
        {
            Given_HexString("341F1301");
            AssertCode(     // fcopysign.d	$f20,$f25,$f7
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f20 = __fcopysign<real64>(f25, f7)");
        }

        [Test]
        public void LoongArchRw_fdiv_d()
        {
            Given_HexString("7D7E0701");
            AssertCode(     // fdiv.d	$f29,$f19,$f31
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f29 = f19 / f31");
        }

        [Test]
        public void LoongArchRw_fdiv_s()
        {
            Given_HexString("EAD50601");
            AssertCode(     // fdiv.s	$f10,$f15,$f21
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f15, word32, 0)",
                "2|L--|v6 = SLICE(f21, word32, 0)",
                "3|L--|v8 = v4 / v6",
                "4|L--|f10 = SEQ(SLICE(f10, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_fld_s()
        {
            Given_HexString("7461202B");
            AssertCode(     // fld.s	$f20,$r11,-000007E8
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r11 - 2024<i64>:real32]",
                "2|L--|f20 = SEQ(SLICE(f20, word32, 32), v5)");
        }

        [Test]
        public void LoongArchRw_fld_d()
        {
            Given_HexString("F60AAA2B");
            AssertCode(     // fld.d	$f22,$r23,-0000057E
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f22 = Mem0[r23 - 1406<i64>:real64]");
        }

        [Test]
        public void LoongArchRw_fldgt_d()
        {
            Given_HexString("9F9C7438");
            AssertCode(     // fldgt.d	$f31,$r4,$r7
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r4 >u r7) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|f31 = Mem0[r4:real64]");
        }

        [Test]
        public void LoongArchRw_fldgt_s()
        {
            Given_HexString("40457438");
            AssertCode(     // fldgt.s	$f0,$r10,$r17
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r10 >u r17) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r10:real32]",
                "4|L--|f0 = SEQ(SLICE(f0, word32, 32), v6)");
        }

        [Test]
        public void LoongArchRw_fldle_d()
        {
            Given_HexString("A4E27538");
            AssertCode(     // fldle.d	$f4,$r21,$r24
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r21 <=u r24) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|f4 = Mem0[r21:real64]");
        }

        [Test]
        public void LoongArchRw_fldle_s()
        {
            Given_HexString("D14D7538");
            AssertCode(     // fldle.s	$f17,$r14,$r19
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r14 <=u r19) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r14:real32]",
                "4|L--|f17 = SEQ(SLICE(f17, word32, 32), v6)");
        }

        [Test]
        public void LoongArchRw_fldx_d()
        {
            Given_HexString("BD5B3438");
            AssertCode(     // fldx.d	$f29,$r29,$r22
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f29 = Mem0[r29 + r22:real64]");
        }

        [Test]
        public void LoongArchRw_fldx_s()
        {
            Given_HexString("5F253038");
            AssertCode(     // fldx.s	$f31,$r10,$r9
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r10 + r9:real32]",
                "2|L--|f31 = SEQ(SLICE(f31, word32, 32), v6)");
        }

        [Test]
        public void LoongArchRw_fmax_d()
        {
            Given_HexString("61170901");
            AssertCode(     // fmax.d	$f1,$f27,$f5
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f1 = fmax(f27, f5)");
        }

        [Test]
        public void LoongArchRw_fmax_s()
        {
            Given_HexString("F8AC0801");
            AssertCode(     // fmax.s	$f24,$f7,$f11
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f7, word32, 0)",
                "2|L--|v6 = SLICE(f11, word32, 0)",
                "3|L--|v8 = fmaxf(v4, v6)",
                "4|L--|f24 = SEQ(SLICE(f24, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_fmaxa_d()
        {
            Given_HexString("B0090D01");
            AssertCode(     // fmaxa.d	$f16,$f13,$f2
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f16 = __fmaxa<real64>(f13, f2)");
        }

        [Test]
        public void LoongArchRw_fmaxa_s()
        {
            Given_HexString("65FC0C01");
            AssertCode(     // fmaxa.s	$f5,$f3,$f31
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f3, word32, 0)",
                "2|L--|v6 = SLICE(f31, word32, 0)",
                "3|L--|v8 = __fmaxa<real32>(v4, v6)",
                "4|L--|f5 = SEQ(SLICE(f5, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_fmin_d()
        {
            Given_HexString("A9670B01");
            AssertCode(     // fmin.d	$f9,$f29,$f25
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f9 = fmin(f29, f25)");
        }

        [Test]
        public void LoongArchRw_fmin_s()
        {
            Given_HexString("71CF0A01");
            AssertCode(     // fmin.s	$f17,$f27,$f19
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f27, word32, 0)",
                "2|L--|v6 = SLICE(f19, word32, 0)",
                "3|L--|v8 = fminf(v4, v6)",
                "4|L--|f17 = SEQ(SLICE(f17, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_fmina_d()
        {
            Given_HexString("09360F01");
            AssertCode(     // fmina.d	$f9,$f16,$f13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f9 = __fmina<real64>(f16, f13)");
        }

        [Test]
        public void LoongArchRw_fmina_s()
        {
            Given_HexString("45C00E01");
            AssertCode(     // fmina.s	$f5,$f2,$f16
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f2, word32, 0)",
                "2|L--|v6 = SLICE(f16, word32, 0)",
                "3|L--|v8 = __fmina<real32>(v4, v6)",
                "4|L--|f5 = SEQ(SLICE(f5, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_fmov_s()
        {
            Given_HexString("90971401");
            AssertCode(     // fmov.s	$f16,$f28
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(f28, word32, 0)",
                "2|L--|v6 = v4",
                "3|L--|f16 = SEQ(SLICE(f16, word32, 32), v6)");
        }

        [Test]
        public void LoongArchRw_fmul_d()
        {
            Given_HexString("73030501");
            AssertCode(     // fmul.d	$f19,$f27,$f0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f19 = f27 * f0");
        }

        [Test]
        public void LoongArchRw_fmul_s()
        {
            Given_HexString("C8FB0401");
            AssertCode(     // fmul.s	$f8,$f30,$f30
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f30, word32, 0)",
                "2|L--|v5 = SLICE(f30, word32, 0)",
                "3|L--|v7 = v4 * v5",
                "4|L--|f8 = SEQ(SLICE(f8, word32, 32), v7)");
        }

        [Test]
        public void LoongArchRw_fscaleb_d()
        {
            Given_HexString("42521101");
            AssertCode(     // fscaleb.d	$f2,$f18,$f20
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f2 = __fscaleb<real64>(f18, f20)");
        }

        [Test]
        public void LoongArchRw_fscaleb_s()
        {
            Given_HexString("DEBE1001");
            AssertCode(     // fscaleb.s	$f30,$f22,$f15
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f22, word32, 0)",
                "2|L--|v6 = SLICE(f15, word32, 0)",
                "3|L--|v8 = __fscaleb<real32>(v4, v6)",
                "4|L--|f30 = SEQ(SLICE(f30, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_fst_d()
        {
            Given_HexString("A24AD52B");
            AssertCode(     // fst.d	$f2,$r21,+00000552
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|Mem0[r21 + 1362<i64>:real64] = f2");
        }

        [Test]
        public void LoongArchRw_fst_s()
        {
            Given_HexString("A62F602B");
            AssertCode(     // fst.s	$f6,$r29,-000007F5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(f6, real32, 0)",
                "2|L--|Mem0[r29 - 2037<i64>:real32] = v4");
        }

        [Test]
        public void LoongArchRw_fstgt_d()
        {
            Given_HexString("6BEA7638");
            AssertCode(     // fstgt.d	$f11,$r19,$r26
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r19 >u r26) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|Mem0[r19:real64] = f11");
        }

        [Test]
        public void LoongArchRw_fstgt_s()
        {
            Given_HexString("04467638");
            AssertCode(     // fstgt.s	$f4,$r16,$r17
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f4, real32, 0)",
                "2|T--|if (r16 >u r17) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r16:real32] = v4");
        }

        [Test]
        public void LoongArchRw_fstle_d()
        {
            Given_HexString("2EE87738");
            AssertCode(     // fstle.d	$f14,$r1,$r26
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r1 <=u r26) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|Mem0[r1:real64] = f14");
        }

        [Test]
        public void LoongArchRw_fstle_s()
        {
            Given_HexString("855F7738");
            AssertCode(     // fstle.s	$f5,$r28,$r23
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f5, real32, 0)",
                "2|T--|if (r28 <=u r23) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r28:real32] = v4");
        }

        [Test]
        public void LoongArchRw_fstx_d()
        {
            Given_HexString("24773C38");
            AssertCode(     // fstx.d	$f4,$r25,$r29
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|Mem0[r25 + r29:real64] = f4");
        }

        [Test]
        public void LoongArchRw_fstx_s()
        {
            Given_HexString("32443838");
            AssertCode(     // fstx.s	$f18,$r1,$r17
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(f18, real32, 0)",
                "2|L--|Mem0[r1 + r17:real32] = v4");
        }

        [Test]
        public void LoongArchRw_fsub_d()
        {
            Given_HexString("FF5D0301");
            AssertCode(     // fsub.d	$f31,$f15,$f23
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|f31 = f15 - f23");
        }

        [Test]
        public void LoongArchRw_fsub_s()
        {
            Given_HexString("DE820201");
            AssertCode(     // fsub.s	$f30,$f22,$f0
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(f22, word32, 0)",
                "2|L--|v6 = SLICE(f0, word32, 0)",
                "3|L--|v8 = v4 - v6",
                "4|L--|f30 = SEQ(SLICE(f30, word32, 32), v8)");
        }

        [Test]
        public void LoongArchRw_jirl()
        {
            Given_HexString("96768B4C");
            AssertCode(     // jirl	$r20,$r22,+00008B74
                "0|T--|0000000000100000(4): 2 instructions",
                "1|L--|r20 = 0000000000100004",
                "2|T--|goto r22 + 0x8B74<64>");
        }

        [Test]
        public void LoongArchRw_jirl_same()
        {
            Given_HexString("2100004C");
            AssertCode(     // jirl	$r1,$r1,00000008
                "0|T--|0000000000100000(4): 2 instructions",
                "1|L--|r1 = 0000000000100004",
                "2|T--|goto 0000000000100004");
        }

        [Test]
        public void LoongArchRw_ld_b()
        {
            Given_HexString("2E070028");
            AssertCode(     // ld.b	$r14,$r25,+00000001
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r25 + 1<i64>:int8]",
                "2|L--|r14 = CONVERT(v5, int8, int64)");
        }

        [Test]
        public void LoongArchRw_ld_bu()
        {
            Given_HexString("A614002A");
            AssertCode(     // ld.bu	$r6,$r5,+00000005
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r5 + 5<i64>:byte]",
                "2|L--|r6 = CONVERT(v5, byte, int64)");
        }

        [Test]
        public void LoongArchRw_ld_h()
        {
            Given_HexString("26207E28");
            AssertCode(     // ld.h	$r6,$r1,-00000078
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r1 - 120<i64>:int16]",
                "2|L--|r6 = CONVERT(v5, int16, int64)");
        }

        [Test]
        public void LoongArchRw_ld_hu()
        {
            Given_HexString("90C0402A");
            AssertCode(     // ld.hu	$r16,$r4,+00000030
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r4 + 48<i64>:uint16]",
                "2|L--|r16 = CONVERT(v5, uint16, int64)");
        }

        [Test]
        public void LoongArchRw_ld_d()
        {
            Given_HexString("8E01D028");
            AssertCode(     // ld.d	$r14,$r12,+00000400
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r14 = Mem0[r12 + 1024<i64>:word64]");
        }

        [Test]
        public void LoongArchRw_ld_w()
        {
            Given_HexString("8E008028");
            AssertCode(     // ld.w	$r14,$r4,+00000000
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r4:int32]",
                "2|L--|r14 = CONVERT(v5, int32, int64)");
        }

        [Test]
        public void LoongArchRw_ld_wu()
        {
            Given_HexString("F56EB82A");
            AssertCode(     // ld.wu	$r21,$r23,-000001E5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r23 - 485<i64>:word32]",
                "2|L--|r21 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ldgt_b()
        {
            Given_HexString("20307838");
            AssertCode(     // ldgt.b	$r0,$r1,$r12
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r1 >u r12) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r1:int8]",
                "4|L--|r0 = CONVERT(v6, int8, int64)");
        }

        [Test]
        public void LoongArchRw_ldgt_d()
        {
            Given_HexString("99C97938");
            AssertCode(     // ldgt.d	$r25,$r12,$r18
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r12 >u r18) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|r25 = Mem0[r12:int64]");
        }

        [Test]
        public void LoongArchRw_ldgt_h()
        {
            Given_HexString("37D37838");
            AssertCode(     // ldgt.h	$r23,$r25,$r20
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r25 >u r20) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r25:int16]",
                "4|L--|r23 = CONVERT(v6, int16, int64)");
        }

        [Test]
        public void LoongArchRw_ldgt_w()
        {
            Given_HexString("840F7938");
            AssertCode(     // ldgt.w	$r4,$r28,$r3
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r28 >u r3) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r28:int32]",
                "4|L--|r4 = CONVERT(v6, int32, int64)");
        }

        [Test]
        public void LoongArchRw_ldle_b()
        {
            Given_HexString("E9107A38");
            AssertCode(     // ldle.b	$r9,$r7,$r4
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r7 <=u r4) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r7:int8]",
                "4|L--|r9 = CONVERT(v6, int8, int64)");
        }

        [Test]
        public void LoongArchRw_ldle_d()
        {
            Given_HexString("B8D97B38");
            AssertCode(     // ldle.d	$r24,$r13,$r22
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r13 <=u r22) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|r24 = Mem0[r13:int64]");
        }

        [Test]
        public void LoongArchRw_ldle_h()
        {
            Given_HexString("37C77A38");
            AssertCode(     // ldle.h	$r23,$r25,$r17
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r25 <=u r17) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r25:int16]",
                "4|L--|r23 = CONVERT(v6, int16, int64)");
        }

        [Test]
        public void LoongArchRw_ldle_w()
        {
            Given_HexString("A9377B38");
            AssertCode(     // ldle.w	$r9,$r29,$r13
                "0|L--|0000000000100000(4): 4 instructions",
                "1|T--|if (r29 <=u r13) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|v6 = Mem0[r29:int32]",
                "4|L--|r9 = CONVERT(v6, int32, int64)");
        }

        [Test]
        public void LoongArchRw_ldptr_d()
        {
            Given_HexString("84000026");
            AssertCode(     // ldptr.d	$r4,$r4,+00000000
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = Mem0[r4:word64]");
        }

        [Test]
        public void LoongArchRw_ldptr_w()
        {
            Given_HexString("8D810124");
            AssertCode(     // ldptr.w	$r13,$r12,+00000060
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r12 + 96<i64>:word32]",
                "2|L--|r13 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void LoongArchRw_ldx_b()
        {
            Given_HexString("2C5F0038");
            AssertCode(     // ldx.b	$r12,$r25,$r23
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r25 + r23:int8]",
                "2|L--|r12 = CONVERT(v6, int8, int64)");
        }

        [Test]
        public void LoongArchRw_ldx_bu()
        {
            Given_HexString("3C3C2038");
            AssertCode(     // ldx.bu	$r28,$r1,$r15
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r1 + r15:byte]",
                "2|L--|r28 = CONVERT(v6, byte, int64)");
        }

        [Test]
        public void LoongArchRw_ldx_h()
        {
            Given_HexString("15610438");
            AssertCode(     // ldx.h	$r21,$r8,$r24
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r8 + r24:int16]",
                "2|L--|r21 = CONVERT(v6, int16, int64)");
        }

        [Test]
        public void LoongArchRw_ldx_hu()
        {
            Given_HexString("5F4C2438");
            AssertCode(     // ldx.hu	$r31,$r2,$r19
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v6 = Mem0[r2 + r19:uint16]",
                "2|L--|r31 = CONVERT(v6, uint16, int64)");
        }


        [Test]
        public void LoongArchRw_ldx_d()
        {
            Given_HexString("8D350C38");
            AssertCode(     // ldx.d	$r13,$r12,$r13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r13 = Mem0[r12 + r13:word64]");
        }

        [Test]
        public void LoongArchRw_ldx_w()
        {
            Given_HexString("84300838");
            AssertCode(     // ldx.w	$r4,$r4,$r12
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = Mem0[r4 + r12:int32]",
                "2|L--|r4 = CONVERT(v5, int32, int64)");
        }

        [Test]
        public void LoongArchRw_lu12i_w()
        {
            Given_HexString("010C3814");
            AssertCode(     // lu12i.w	$r1,+0001C060
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r1 = 0x1C060000<64>");
        }

        [Test]
        public void LoongArchRw_lu12i_w_negative()
        {
            Given_HexString("81FFFF15");
            AssertCode(     // lu12i.w	$r1,+0001C060
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r1 = 0xFFFFFFFFFFFFC000<64>");
        }

        [Test]
        public void LoongArchRw_lu32i_d()
        {
            Given_HexString("48FFFF17");
            AssertCode(     // lu32i.d	$r8,-00000006
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r8 = SEQ(0xFFFFFFFA<32>, SLICE(r8, word32, 0))");
        }

        [Test]
        public void LoongArchRw_lu52i_d()
        {
            Given_HexString("84000003");
            AssertCode(     // lu52i.d	$r4,$r4,+00000000
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = SEQ(0<12>, SLICE(r4, ui52, 0))");
        }

        [Test]
        public void LoongArchRw_maskeqz()
        {
            Given_HexString("2D371300");
            AssertCode(     // maskeqz	$r13,$r25,$r13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r13 = r13 == 0<64> ? 0<64> : r25");
        }

        [Test]
        public void LoongArchRw_masknez()
        {
            Given_HexString("D0B61300");
            AssertCode(     // masknez	$r16,$r22,$r13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r16 = r13 != 0<64> ? 0<64> : r22");
        }

        [Test]
        public void LoongArchRw_mod_d()
        {
            Given_HexString("A6D92200");
            AssertCode(     // mod.d	$r6,$r13,$r22
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r6 = r13 %s r22");
        }

        [Test]
        public void LoongArchRw_mod_du()
        {
            Given_HexString("8C942300");
            AssertCode(     // mod.du	$r12,$r4,$r5
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = r4 %u r5");
        }

        [Test]
        public void LoongArchRw_mod_w()
        {
            Given_HexString("ACB42000");
            AssertCode(     // mod.w	$r12,$r5,$r13
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r5, word32, 0)",
                "2|L--|v6 = SLICE(r13, word32, 0)",
                "3|L--|v8 = v4 %s v6",
                "4|L--|r12 = CONVERT(v8, word32, int64)");
        }

        [Test]
        public void LoongArchRw_mod_wu()
        {
            Given_HexString("8EB52100");
            AssertCode(     // mod.wu	$r14,$r12,$r13
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r12, word32, 0)",
                "2|L--|v6 = SLICE(r13, word32, 0)",
                "3|L--|v8 = v4 %u v6",
                "4|L--|r14 = CONVERT(v8, word32, int64)");
        }

        [Test]
        public void LoongArchRw_mul_d()
        {
            Given_HexString("CFBD1D00");
            AssertCode(     // mul.d	$r15,$r14,$r15
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r15 = r14 * r15");
        }

        [Test]
        public void LoongArchRw_mul_w()
        {
            Given_HexString("18671C00");
            AssertCode(     // mul.w	$r24,$r24,$r25
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r24, word32, 0)",
                "2|L--|v6 = SLICE(r25, word32, 0)",
                "3|L--|v7 = v4 * v6",
                "4|L--|r24 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_mulh_d()
        {
            Given_HexString("2B601E00");
            AssertCode(     // mulh.d	$r11,$r1,$r24
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = r1 *s128 r24",
                "2|L--|r11 = SLICE(v5, int64, 64)");
        }
        
        [Test]
        public void LoongArchRw_mulh_du()
        {
            Given_HexString("21D31E00");
            AssertCode(     // mulh.du	$r1,$r25,$r20
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v5 = r25 *u128 r20",
                "2|L--|r1 = SLICE(v5, uint64, 64)");
        }

        [Test]
        public void LoongArchRw_mulh_w()
        {
            Given_HexString("17C31C00");
            AssertCode(     // mulh.w	$r23,$r24,$r16
                "0|L--|0000000000100000(4): 5 instructions",
                "1|L--|v4 = SLICE(r24, word32, 0)",
                "2|L--|v6 = SLICE(r16, word32, 0)",
                "3|L--|v7 = v4 *s64 v6",
                "4|L--|v9 = SLICE(v7, int32, 32)",
                "5|L--|r23 = CONVERT(v9, int32, int64)");
        }

        [Test]
        public void LoongArchRw_mulh_wu()
        {
            Given_HexString("1C331D00");
            AssertCode(     // mulh.wu	$r28,$r24,$r12
                "0|L--|0000000000100000(4): 5 instructions",
                "1|L--|v4 = SLICE(r24, word32, 0)",
                "2|L--|v6 = SLICE(r12, word32, 0)",
                "3|L--|v7 = v4 *u64 v6",
                "4|L--|v9 = SLICE(v7, uint32, 32)",
                "5|L--|r28 = CONVERT(v9, uint32, int64)");
        }

        [Test]
        public void LoongArchRw_mulw_d_w()
        {
            Given_HexString("CF2F1F00");
            AssertCode(     // mulw.d.w	$r15,$r30,$r11
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v3 = SLICE(r30, int32, 0)",
                "2|L--|v5 = SLICE(r11, int32, 0)",
                "3|L--|r15 = v3 *s64 v5");
        }

        [Test]
        public void LoongArchRw_mulw_d_wu()
        {
            Given_HexString("48E21F00");
            AssertCode(     // mulw.d.wu	$r8,$r18,$r24
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v3 = SLICE(r18, uint32, 0)",
                "2|L--|v5 = SLICE(r24, uint32, 0)",
                "3|L--|r8 = v3 *u64 v5");
        }

        [Test]
        public void LoongArchRw_nor()
        {
            Given_HexString("02101400");
            AssertCode(     // nor	$r0,$r0,$r4
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r2 = ~r4");
        }

        [Test]
        public void LoongArchRw_or_zero_zero()
        {
            Given_HexString("0C001500");
            AssertCode(     // or	$r12,$r0,$r0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = 0<64>");
        }

        [Test]
        public void LoongArchRw_ori()
        {
            Given_HexString("21508203");
            AssertCode(     // ori	$r1,$r1,00000094
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r1 = r1 | 0x94<64>");
        }

        [Test]
        public void LoongArchRw_ori_same()
        {
            Given_HexString("21008003");
            AssertCode(     // ori	$r1,$r1,00000000
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|nop");
        }

        [Test]
        public void LoongArchRw_orn()
        {
            Given_HexString("15001600");
            AssertCode(     // orn	$r21,$r0,$r0
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r21 = 0<64> | ~0<64>");
        }

        [Test]
        public void LoongArchRw_revb_d()
        {
            Given_HexString("813D0000");
            AssertCode(     // revb.d	$r1,$r12
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r1 = __rev_b<word64>(r12)");
        }

        [Test]
        public void LoongArchRw_revh_d()
        {
            Given_HexString("50450000");
            AssertCode(     // revh.d	$r16,$r10
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r16 = __rev_h<word64>(r10)");
        }

        [Test]
        public void LoongArchRw_rotr_d()
        {
            Given_HexString("5CD91B00");
            AssertCode(     // rotr.d	$r28,$r10,$r22
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r28 = __ror<word64,word64>(r10, r22)");
        }

        [Test]
        public void LoongArchRw_rotr_w()
        {
            Given_HexString("10661B00");
            AssertCode(     // rotr.w	$r16,$r16,$r25
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r16, word32, 0)",
                "2|L--|v6 = __ror<word32,word64>(v4, r25)",
                "3|L--|r16 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_rotri_d()
        {
            Given_HexString("E4F04D00");
            AssertCode(     // rotri.d	$r4,$r7,0000003C
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = __ror<word64,word64>(r7, 0x3C<64>)");
        }

        [Test]
        public void LoongArchRw_rotri_w()
        {
            Given_HexString("E4F04C00");
            AssertCode(     // rotri.w	$r4,$r7,0000001C
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r7, word32, 0)",
                "2|L--|v6 = __ror<word32,word64>(v4, 0x1C<64>)",
                "3|L--|r4 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_sll_d()
        {
            Given_HexString("84941800");
            AssertCode(     // sll.d	$r4,$r4,$r5
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = r4 << r5");
        }

        [Test]
        public void LoongArchRw_sll_w()
        {
            Given_HexString("A5151700");
            AssertCode(     // sll.w	$r5,$r13,$r5
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r13, word32, 0)",
                "2|L--|v6 = v4 << r5",
                "3|L--|r5 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_slli_d()
        {
            Given_HexString("8C614100");
            AssertCode(     // slli.d	$r12,$r12,00000018
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = r12 << 0x18<64>");
        }

        [Test]
        public void LoongArchRw_slli_w()
        {
            Given_HexString("A5884000");
            AssertCode(     // slli.w	$r5,$r5,00000002
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r5, word32, 0)",
                "2|L--|v5 = v4 << 2<64>",
                "3|L--|r5 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void LoongArchRw_slt()
        {
            Given_HexString("BD331200");
            AssertCode(     // slt	$r29,$r29,$r12
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r29 = r29 < r12 ? 1<64> : 0<64>");
        }

        [Test]
        public void LoongArchRw_slti()
        {
            Given_HexString("C3080002");
            AssertCode(     // slti	$r3,$r6,+00000002
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r3 = r6 < 2<64> ? 1<64> : 0<64>");
        }

        [Test]
        public void LoongArchRw_sltu()
        {
            Given_HexString("04901200");
            AssertCode(     // sltu	$r4,$r0,$r4
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = 0<64> <u r4 ? 1<64> : 0<64>");
        }

        [Test]
        public void LoongArchRw_sltui()
        {
            Given_HexString("FF074002");
            AssertCode(     // sltui	$r31,$r31,+00000001
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r31 = r31 <u 1<64> ? 1<64> : 0<64>");
        }

        [Test]
        public void LoongArchRw_sra_d()
        {
            Given_HexString("EE9C1900");
            AssertCode(     // sra.d	$r14,$r7,$r7
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r14 = r7 >> r7");
        }

        [Test]
        public void LoongArchRw_sra_w()
        {
            Given_HexString("2C321800");
            AssertCode(     // sra.w	$r12,$r17,$r12
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r17, word32, 0)",
                "2|L--|v6 = v4 >> r12",
                "3|L--|r12 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_srai_d()
        {
            Given_HexString("8CE14900");
            AssertCode(     // srai.d	$r12,$r12,00000038
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r12 = r12 >> 0x38<64>");
        }

        [Test]
        public void LoongArchRw_srai_w()
        {
            Given_HexString("99FD4800");
            AssertCode(     // srai.w	$r25,$r12,0000001F
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r12, word32, 0)",
                "2|L--|v6 = v4 >> 0x1F<64>",
                "3|L--|r25 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_srl_w()
        {
            Given_HexString("85941700");
            AssertCode(     // srl.w	$r5,$r4,$r5
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r4, word32, 0)",
                "2|L--|v6 = v4 >>u r5",
                "3|L--|r5 = CONVERT(v6, word32, int64)");
        }

        [Test]
        public void LoongArchRw_srli_d()
        {
            Given_HexString("A4814500");
            AssertCode(     // srli.d	$r4,$r13,00000020
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r4 = r13 >>u 0x20<64>");
        }

        [Test]
        public void LoongArchRw_srli_w()
        {
            Given_HexString("08A14400");
            AssertCode(     // srli.w	$r8,$r8,00000008
                "0|L--|0000000000100000(4): 3 instructions",
                "1|L--|v4 = SLICE(r8, word32, 0)",
                "2|L--|v5 = v4 >>u 8<64>",
                "3|L--|r8 = CONVERT(v5, word32, int64)");
        }

        [Test]
        public void LoongArchRw_st_b()
        {
            Given_HexString("8D110029");
            AssertCode(     // st.b	$r13,$r12,+00000004
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r13, byte, 0)",
                "2|L--|Mem0[r12 + 4<i64>:byte] = v4");
        }

        [Test]
        public void LoongArchRw_st_d()
        {
            Given_HexString("8E01D029");
            AssertCode(     // st.d	$r14,$r12,+00000400
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|Mem0[r12 + 1024<i64>:word64] = r14");
        }

        [Test]
        public void LoongArchRw_st_h()
        {
            Given_HexString("6C204029");
            AssertCode(     // st.h	$r12,$r3,+00000008
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r12, word16, 0)",
                "2|L--|Mem0[r3 + 8<i64>:word16] = v4");
        }

        [Test]
        public void LoongArchRw_st_w()
        {
            Given_HexString("80008029");
            AssertCode(     // st.w	$r0,$r4,+00000000
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|Mem0[r4:word32] = 0<32>");
        }

        [Test]
        public void LoongArchRw_stgt_b()
        {
            Given_HexString("7A7C7C38");
            AssertCode(     // stgt.b	$r26,$r3,$r31
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r26, byte, 0)",
                "2|T--|if (r3 >u r31) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r3:byte] = v4");
        }

        [Test]
        public void LoongArchRw_stgt_d()
        {
            Given_HexString("CD8E7D38");
            AssertCode(     // stgt.d	$r13,$r22,$r3
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r22 >u r3) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|Mem0[r22:word64] = r13");
        }

        [Test]
        public void LoongArchRw_stgt_h()
        {
            Given_HexString("149F7C38");
            AssertCode(     // stgt.h	$r20,$r24,$r7
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r20, word16, 0)",
                "2|T--|if (r24 >u r7) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r24:word16] = v4");
        }

        [Test]
        public void LoongArchRw_stle_b()
        {
            Given_HexString("4D017E38");
            AssertCode(     // stle.b	$r13,$r10,$r0
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r13, byte, 0)",
                "2|T--|if (r10 <=u r0) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r10:byte] = v4");
        }

        [Test]
        public void LoongArchRw_stle_d()
        {
            Given_HexString("8AC47F38");
            AssertCode(     // stle.d	$r10,$r4,$r17
                "0|L--|0000000000100000(4): 3 instructions",
                "1|T--|if (r4 <=u r17) micro_goto 2",
                "2|L--|__raise_exception(0xA<32>)",
                "3|L--|Mem0[r4:word64] = r10");
        }

        [Test]
        public void LoongArchRw_stle_h()
        {
            Given_HexString("D6C87E38");
            AssertCode(     // stle.h	$r22,$r6,$r18
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r22, word16, 0)",
                "2|T--|if (r6 <=u r18) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r6:word16] = v4");
        }

        [Test]
        public void LoongArchRw_stle_w()
        {
            Given_HexString("4C767F38");
            AssertCode(     // stle.w	$r12,$r18,$r29
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r12, word32, 0)",
                "2|T--|if (r18 <=u r29) micro_goto 3",
                "3|L--|__raise_exception(0xA<32>)",
                "4|L--|Mem0[r18:word32] = v4");
        }

        [Test]
        public void LoongArchRw_stptr_d()
        {
            Given_HexString("8D190F27");
            AssertCode(     // stptr.d	$r13,$r12,+000003C6
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|Mem0[r12 + 966<i64>:word64] = r13");
        }

        [Test]
        public void LoongArchRw_stptr_w()
        {
            Given_HexString("8D810125");
            AssertCode(     // stptr.w	$r13,$r12,+00000060
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r13, word32, 0)",
                "2|L--|Mem0[r12 + 96<i64>:word32] = v4");
        }

        [Test]
        public void LoongArchRw_stx_b()
        {
            Given_HexString("F1151038");
            AssertCode(     // stx.b	$r17,$r15,$r5
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r17, byte, 0)",
                "2|L--|Mem0[r15 + r5:byte] = v4");
        }

        [Test]
        public void LoongArchRw_stx_d()
        {
            Given_HexString("8D391C38");
            AssertCode(     // stx.d	$r13,$r12,$r14
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|Mem0[r12 + r14:word64] = r13");
        }

        [Test]
        public void LoongArchRw_stx_h()
        {
            Given_HexString("847D1438");
            AssertCode(     // stx.h	$r4,$r12,$r31
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r4, word16, 0)",
                "2|L--|Mem0[r12 + r31:word16] = v4");
        }

        [Test]
        public void LoongArchRw_stx_w()
        {
            Given_HexString("8E341838");
            AssertCode(     // stx.w	$r14,$r4,$r13
                "0|L--|0000000000100000(4): 2 instructions",
                "1|L--|v4 = SLICE(r14, word32, 0)",
                "2|L--|Mem0[r4 + r13:word32] = v4");
        }

        [Test]
        public void LoongArchRw_sub_d()
        {
            Given_HexString("63B41100");
            AssertCode(     // sub.d	$r3,$r3,$r13
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r3 = r3 - r13");
        }

        [Test]
        public void LoongArchRw_sub_w()
        {
            Given_HexString("8C5D1100");
            AssertCode(     // sub.w	$r12,$r12,$r23
                "0|L--|0000000000100000(4): 4 instructions",
                "1|L--|v4 = SLICE(r12, word32, 0)",
                "2|L--|v6 = SLICE(r23, word32, 0)",
                "3|L--|v7 = v4 - v6",
                "4|L--|r12 = CONVERT(v7, word32, int64)");
        }

        [Test]
        public void LoongArchRw_syscall()
        {
            Given_HexString("8F5F2B00");
            AssertCode(     // syscall	00005F8F
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|__syscall<word32>(0x5F8F<u32>)");
        }

        [Test]
        public void LoongArchRw_tlbwr()
        {
            Given_HexString("00304806");
            AssertCode(     // tlbwr
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|__tlbwr()");
        }

        [Test]
        public void LoongArchRw_xor()
        {
            Given_HexString("ADB91500");
            AssertCode(     // xor	$r13,$r13,$r14
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r13 = r13 ^ r14");
        }

        [Test]
        public void LoongArchRw_xori()
        {
            Given_HexString("E233F203");
            AssertCode(     // xori	$r2,$r31,00000C8C
                "0|L--|0000000000100000(4): 1 instructions",
                "1|L--|r2 = r31 ^ 0xC8C<64>");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues




































































































        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues



























































































































    }
}
