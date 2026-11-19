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
using Reko.Arch.ControlData;
using Reko.Core;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch.ControlData;

public class CDC6600RewriterTests : RewriterTestBase
{
    private readonly CDC6600Architecture arch;
    private readonly Address addr;

    public CDC6600RewriterTests()
    {
        this.arch = new CDC6600Architecture(CreateServiceContainer(), "cdc6600", []);
        this.addr = CDC6600Architecture.Ptr(0x1000);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    private void Given_OctalWords(string octalWords)
    {
        var words = OctalPattern.ToUInt64s(octalWords, 20);
        var mem = new Word64MemoryArea(addr, words);
        base.Given_MemoryArea(mem);
    }

    [Test]
    public void CDC6600Rw_GE()
    {
        Given_OctalWords("46226313170633052064");
        AssertCode(    // NO, IX3 X1 - X7, GE B0 B6 052064
            "0|L--|010000(1): 3 instructions",
            "1|L--|nop",
            "2|L--|X3 = X1 - X7",
            "3|T--|if (0<18> >= B6) branch 052064");
    }

    [Test]
    public void CDC6600Rw_IR()
    {
        Given_OctalWords("03471206524675446670");
        AssertCode(    // IR X5 120652, NO, NO
            "0|L--|010000(1): 4 instructions",
            "1|T--|if (!__is_in_range(X5, 0<60>)) micro_goto 2",
            "2|T--|goto 0xA1AA<60>",
            "3|L--|nop",
            "4|L--|nop");
    }

    [Test]
    public void CDC6600Rw_JP()
    {
        Given_OctalWords("02015636773165077136");
        AssertCode(    // JP B6 + 563677, FX6 X5 - X0, SX1 B3 - B6
            "0|L--|010000(1): 1 instructions",
            "1|T--|goto B6 + 0o563677<p18>");
    }

    [Test]
    public void CDC6600Rw_NG()
    {
        Given_OctalWords("03327413012335633164");
        AssertCode(    // NG X0 741301, AX3 B5 X6, DX1 X6 - X4
            "0|L--|010000(1): 4 instructions",
            "1|T--|if (X0 >= 0<60>) micro_goto 2",
            "2|T--|goto 0x3C2C1<60>",
            "3|L--|X3 = __asr_nominal(B5, X6)",
            "4|L--|X1 = __dsub(X6, X4)");
    }

    [Test]
    public void CDC6600Rw_NO()
    {
        Given_OctalWords("46401753011620613633");
        AssertCode(    // NO, SX3 A0 - B1, BX2 -X6 + X0, BX6 X3 - X3
            "0|L--|010000(1): 4 instructions",
            "1|L--|nop",
            "2|L--|X3 = CONVERT(A0, word18, int60) - CONVERT(B1, word18, int60)",
            "3|L--|X2 = X0 | ~X6",
            "4|L--|X6 = X3 ^ X3");
    }

    [Test]
    public void CDC6600Rw_NZ()
    {
        Given_OctalWords("23651031033716032105");
        AssertCode(    // AX6 B5 X1, NZ X6 337160, DX1 X0 + X5
            "0|L--|010000(1): 4 instructions",
            "1|L--|X6 = __asr_nominal(B5, X1)",
            "2|T--|if (X6 == 0<60>) micro_goto 3",
            "3|T--|goto 0x1BE70<60>",
            "4|L--|X1 = __dadd(X0, X5)");
    }

    [Test]
    public void CDC6600Rw_OR()
    {
        Given_OctalWords("03550743216420622646");
        AssertCode(    // OR X2 74321, SB2 A0 + B6, LX6 B4 X6
            "0|L--|010000(1): 4 instructions",
            "1|T--|if (!__is_out_of_range(X2, 0<60>)) micro_goto 2",
            "2|T--|goto 0x78D1<60>",
            "3|L--|B2 = A0 + B6",
            "4|L--|X6 = __lsl_nominal(B4, X6)");
    }

    [Test]
    public void CDC6600Rw_PL()
    {
        Given_OctalWords("03245065075127336376");
        AssertCode(    // PL X0 506507, SA3 B7 + 336376
            "0|L--|010000(1): 4 instructions",
            "1|T--|if (X0 < 0<60>) micro_goto 2",
            "2|T--|goto 0x28D47<60>",
            "3|L--|A3 = B7 + 0x1BCFE<18>",
            "4|L--|X3 = Mem0[A3:word60]");
    }

    [Test]
    public void CDC6600Rw_popcnt()
    {
        Given_OctalWords("47275774130564523511");
        AssertCode(    // CX2 X5, SX4 B1 - B3, NE B5 B1 523511
            "0|L--|010000(1): 3 instructions",
            "1|L--|X2 = __population_count<word60>(X5)",
            "2|L--|X4 = CONVERT(B1, word18, int60) - CONVERT(B3, word18, int60)",
            "3|T--|if (B5 != B1) branch 523511");
    }

    [Test]
    public void CDC6600Rw_PS()
    {
        Given_OctalWords("00034535636455332447");
        AssertCode(    // PS, SB5 A5 + B3, DX4 X4 + X7
            "0|L--|010000(1): 1 instructions",
            "1|L--|__program_stop()");
    }

    [Test]
    public void CDC6600Rw_REC()
    {
        Given_OctalWords("01136132463763346000");
        AssertCode(    // REC B4 + 613246, IX6 X3 - X3, NO
            "0|L--|010000(1): 4 instructions",
            "1|L--|v6 = B4 + 0x316A6<60>",
            "2|L--|__read_extended_core(A0, X0, v6)",
            "3|L--|X6 = X3 - X3",
            "4|L--|nop");
    }

    [Test]
    public void CDC6600Rw_RJ()
    {
        Given_OctalWords("46265474660100067551");
        AssertCode(    // NO, RJ 067551
            "0|L--|010000(1): 3 instructions",
            "1|L--|nop",
            "2|L--|X4 = __population_count<word60>(X6)",
            "3|T--|call 067551 (0)");
    }

    [Test]
    public void CDC6600Rw_WEC()
    {
        Given_OctalWords("01221166064604645540");
        AssertCode(    // WEC B0 + 116606, NO, RX5 X4 / X0
            "0|L--|010000(1): 4 instructions",
            "1|L--|v5 = 0<18> + 0x9D86<60>",
            "2|L--|__write_extended_core(X0, A0, v5)",
            "3|L--|nop",
            "4|L--|X5 = __fdiv_round(X4, X0)");
    }

    [Test]
    public void CDC6600Rw_ZR()
    {
        Given_OctalWords("03066035145002630004");
        AssertCode(    // ZR X1 603514, SA0 A0 + 630004
            "0|L--|010000(1): 3 instructions",
            "1|T--|if (X1 != 0<60>) micro_goto 2",
            "2|T--|goto 0x3074C<60>",
            "3|L--|A0 = A0 + 0x33004<18>");
    }

    [Test]
    public void CDC6600Rw_and()
    {
        Given_OctalWords("46331707531145616717");
        AssertCode(    // NO, BX4 X5 * X6, BX7 -X7 + X1
            "0|L--|010000(1): 3 instructions",
            "1|L--|nop",
            "2|L--|X4 = CONVERT(A5, word18, int60) + 0x1932E<60>",
            "3|L--|X7 = X1 | ~X7");
    }

    [Test]
    public void CDC6600Rw_asr()
    {
        Given_OctalWords("21010331172114143665");
        AssertCode(    // AX0 10, DX1 X1 - X7, AX1 41, MX6 65
            "0|L--|010000(1): 4 instructions",
            "1|L--|X0 = X0 >> 8<60>",
            "2|L--|X1 = __dsub(X1, X7)",
            "3|L--|X1 = X1 >> 0x21<60>",
            "4|L--|X6 = 0xFFFFFFFFFFFFF80<60>");
    }

    [Test]
    public void CDC6600Rw_asrn()
    {
        Given_OctalWords("23221172531715337117");
        AssertCode(    // AX2 B2 X1, BX2 -X3 - X5, BX1 -X3 - X5, IX1 X1 - X7
            "0|L--|010000(1): 4 instructions",
            "1|L--|X2 = __asr_nominal(B2, X1)",
            "2|L--|X2 = X5 ^ ~X3",
            "3|L--|X1 = X5 ^ ~X3",
            "4|L--|X1 = X1 - X7");
    }

    [Test]
    public void CDC6600Rw_dadd()
    {
        Given_OctalWords("32052563053176540162");
        AssertCode(    // DX0 X5 + X2, SA3 B0 + B5, FX7 X6 - X5, FX1 X6 * X2
            "0|L--|010000(1): 5 instructions",
            "1|L--|X0 = __dadd(X5, X2)",
            "2|L--|A3 = 0<18> + B5",
            "3|L--|X3 = Mem0[A3:word60]",
            "4|L--|X7 = X6 - X5",
            "5|L--|X1 = X6 * X2");
    }

    [Test]
    public void CDC6600Rw_dmul()
    {
        Given_OctalWords("42043323521675717630");
        AssertCode(    // DX0 X4 * X3, DX3 X5 + X2, BX7 -X7 + X5, BX6 -X0 - X3
            "0|L--|010000(1): 4 instructions",
            "1|L--|X0 = __dmul(X4, X3)",
            "2|L--|X3 = __dadd(X5, X2)",
            "3|L--|X7 = X5 | ~X7",
            "4|L--|X6 = X3 ^ ~X0");
    }

    [Test]
    public void CDC6600Rw_dsub()
    {
        Given_OctalWords("46456331046645746311");
        AssertCode(    // NO, DX1 X0 - X4, SB4 B5 + B7, NO
            "0|L--|010000(1): 4 instructions",
            "1|L--|nop",
            "2|L--|X1 = __dsub(X0, X4)",
            "3|L--|B4 = B5 + B7",
            "4|L--|nop");
    }

    [Test]
    public void CDC6600Rw_fadd()
    {
        Given_OctalWords("30447220016541746750");
        AssertCode(    // FX4 X4 + X7, LX0 B0 X1, SB4 A1 - B7, NO
            "0|L--|010000(1): 4 instructions",
            "1|L--|X4 = X4 + X7",
            "2|L--|X0 = __lsl_nominal(0<18>, X1)",
            "3|L--|B4 = A1 - B7",
            "4|L--|nop");
    }

    [Test]
    public void CDC6600Rw_fadd_round()
    {
        Given_OctalWords("34105476114526633741");
        AssertCode(    // RX1 X0 + X5, CX6 X1, RX2 X6 / X6, DX7 X4 - X1
            "0|L--|010000(1): 4 instructions",
            "1|L--|X1 = __fadd_round(X0, X5)",
            "2|L--|X6 = __population_count<word60>(X1)",
            "3|L--|X2 = __fdiv_round(X6, X6)",
            "4|L--|X7 = __dsub(X4, X1)");
    }

    [Test]
    public void CDC6600Rw_fdiv()
    {
        Given_OctalWords("44127615733065412176");
        AssertCode(    // FX1 X2 / X7, SB6 B5 + 330654, BX1 X7 + X6
            "0|L--|010000(1): 3 instructions",
            "1|L--|X1 = X2 / X7",
            "2|L--|B6 = B5 + 0x1B1AC<18>",
            "3|L--|X1 = X7 | X6");
    }

    [Test]
    public void CDC6600Rw_fdiv_round()
    {
        Given_OctalWords("45031665032555165673");
        AssertCode(    // RX0 X3 / X1, SB5 B0 + B3, ZX5 B5 X1, SB6 A7 - B3
            "0|L--|010000(1): 4 instructions",
            "1|L--|X0 = __fdiv_round(X3, X1)",
            "2|L--|B5 = 0<18> + B3",
            "3|L--|X5 = __round_normalize(B5, X1)",
            "4|L--|B6 = A7 - B3");
    }

    [Test]
    public void CDC6600Rw_fmul()
    {
        Given_OctalWords("40611565576746125276");
        AssertCode(    // FX6 X1 * X1, SA5 B5 + B7, SB4 B6 - B1, ZX2 B7 X6
            "0|L--|010000(1): 5 instructions",
            "1|L--|X6 = X1 * X1",
            "2|L--|A5 = B5 + B7",
            "3|L--|X5 = Mem0[A5:word60]",
            "4|L--|B4 = B6 - B1",
            "5|L--|X2 = __round_normalize(B7, X6)");
    }

    [Test]
    public void CDC6600Rw_fmul_round()
    {
        Given_OctalWords("00360200635605441006");
        AssertCode(    // PS, SA0 B5 + B4, RX0 X0 * X6
            "0|L--|010000(1): 1 instructions",
            "1|L--|__program_stop()");
    }

    [Test]
    public void CDC6600Rw_fsub()
    {
        Given_OctalWords("31437452677253106034");
        AssertCode(    // FX4 X3 - X7, RX2 X6 / X7, SX0 X3 + 106034
            "0|L--|010000(1): 3 instructions",
            "1|L--|X4 = X3 - X7",
            "2|L--|X2 = __fdiv_round(X6, X7)",
            "3|L--|X0 = X3 + 0x8C1C<60>");
    }

    [Test]
    public void CDC6600Rw_fsub_round()
    {
        Given_OctalWords("35063571774571257552");
        AssertCode(    // RX0 X6 - X3, SA1 B7 - B7, RX7 X1 / X2, SA5 B5 - B2
            "0|L--|010000(1): 6 instructions",
            "1|L--|X0 = __fsub_round(X6, X3)",
            "2|L--|A1 = B7 - B7",
            "3|L--|X1 = Mem0[A1:word60]",
            "4|L--|X7 = __fdiv_round(X1, X2)",
            "5|L--|A5 = B5 - B2",
            "6|L--|X5 = Mem0[A5:word60]");
    }

    [Test]
    public void CDC6600Rw_iadd()
    {
        Given_OctalWords("36132047463320246000");
        AssertCode(    // IX1 X3 + X2, EQ B2 B0 633202, NO
            "0|L--|010000(1): 4 instructions",
            "1|L--|X1 = X3 + X2",
            "2|T--|if (B2 != 0<18>) micro_goto 3",
            "3|T--|goto 633202",
            "4|L--|nop");
    }

    [Test]
    public void CDC6600Rw_lsl()
    {
        Given_OctalWords("20363571616437622073");
        AssertCode(    // LX3 63, SA1 B6 - B1, SB3 A7 + B6, LX0 B7 X3
            "0|L--|010000(1): 5 instructions",
            "1|L--|X3 = X3 << 0x33<60>",
            "2|L--|A1 = B6 - B1",
            "3|L--|X1 = Mem0[A1:word60]",
            "4|L--|B3 = A7 + B6",
            "5|L--|X0 = __lsl_nominal(B7, X3)");
    }
 
    [Test]
    public void CDC6600Rw_lsln()
    {
        Given_OctalWords("22101466765341646423");
        AssertCode(    // LX1 B0 X1, NO, SA4 X1 + B6, NO
            "0|L--|010000(1): 5 instructions",
            "1|L--|X1 = __lsl_nominal(0<18>, X1)",
            "2|L--|nop",
            "3|L--|A4 = SLICE(X1, word18, 0) + B6",
            "4|L--|X4 = Mem0[A4:word60]",
            "5|L--|nop");
    }

    [Test]
    public void CDC6600Rw_mask()
    {
        Given_OctalWords("43043533475533555275");
        AssertCode(    // MX0 43, SA3 X4 + B7, SA3 A3 - B5, SA2 A7 - B5
            "0|L--|010000(1): 7 instructions",
            "1|L--|X0 = 0xFFFFFFFFE000000<60>",
            "2|L--|A3 = SLICE(X4, word18, 0) + B7",
            "3|L--|X3 = Mem0[A3:word60]",
            "4|L--|A3 = A3 - B5",
            "5|L--|X3 = Mem0[A3:word60]",
            "6|L--|A2 = A7 - B5",
            "7|L--|X2 = Mem0[A2:word60]");
    }

    [Test]
    public void CDC6600Rw_mov()
    {
        Given_OctalWords("10065540021446240624");
        AssertCode(    // BX0 X6, SA0 A0 + B2, BX4 -X2, FX6 X2 * X4
            "0|L--|010000(1): 4 instructions",
            "1|L--|X0 = X6",
            "2|L--|A0 = A0 + B2",
            "3|L--|X4 = ~X2",
            "4|L--|X6 = X2 * X4");
    }

    [Test]
    public void CDC6600Rw_neg()
    {
        Given_OctalWords("14011214320541030211");
        AssertCode(    // BX0 -X1, AX4 32, NE B2 B1 030211
            "0|L--|010000(1): 3 instructions",
            "1|L--|X0 = ~X1",
            "2|L--|X4 = X4 >> 0x1A<60>",
            "3|T--|if (B2 != B1) branch 030211");
    }

    [Test]
    public void CDC6600Rw_normalize()
    {
        Given_OctalWords("24455002645545565064");
        AssertCode(    // NX4 B5 X5, PS, SB0 A6 - B4
            "0|L--|010000(1): 2 instructions",
            "1|L--|X4 = __normalize(B5, X5)",
            "2|L--|__program_stop()");
    }

    [Test]
    public void CDC6600Rw_or()
    {
        Given_OctalWords("12022746001070646353");
        AssertCode(    // BX0 X2 + X2, SX6 A0 + B0, BX7 X0, NO
            "0|L--|010000(1): 4 instructions",
            "1|L--|X0 = X2 | X2",
            "2|L--|X6 = CONVERT(A0, word18, int60)",
            "3|L--|X7 = X0",
            "4|L--|nop");
    }

    [Test]
    public void CDC6600Rw_pack()
    {
        Given_OctalWords("27605713722024047437");
        AssertCode(    // PX6 B0 X5, SX2 B4 + 220240, CX4 X7
            "0|L--|010000(1): 3 instructions",
            "1|L--|X6 = __pack(0<18>, X5)",
            "2|L--|X2 = CONVERT(B4, word18, int60) + 0x120A0<60>",
            "3|L--|X4 = __population_count<word60>(X7)");
    }

    [Test]
    public void CDC6600Rw_round_normalize()
    {
        Given_OctalWords("25677370156236506464");
        AssertCode(    // ZX6 B7 X7, IX0 X1 - X5, SB4 X6 + 506464
            "0|L--|010000(1): 3 instructions",
            "1|L--|X6 = __round_normalize(B7, X7)",
            "2|L--|X0 = X1 - X5",
            "3|L--|B4 = SLICE(X6, word18, 0) + 0x28D34<18>");
    }

    [Test]
    public void CDC6600Rw_unpack()
    {
        Given_OctalWords("26357144474025526220");
        AssertCode(    // UX3 B5 X7, BX4 -X7, FX2 X5 * X5, UX2 B2 X0
            "0|L--|010000(1): 4 instructions",
            "1|L--|X3 = __pack(B5, X7)",
            "2|L--|X4 = ~X7",
            "3|L--|X2 = X5 * X5",
            "4|L--|X2 = __pack(B2, X0)");
    }

    [Test]
    public void CDC6600Rw_xor()
    {
        Given_OctalWords("13062334441127763642");
        AssertCode(    // BX0 X6 - X2, DX4 X4 - X4, BX2 X7 * X7, SB6 X4 + B2
            "0|L--|010000(1): 4 instructions",
            "1|L--|X0 = X6 ^ X2",
            "2|L--|X4 = __dsub(X4, X4)",
            "3|L--|X2 = X7 & X7",
            "4|L--|B6 = SLICE(X4, word18, 0) + B2");
    }

    [Test]
    public void CDC6600Rw_xorn()
    {
        Given_OctalWords("17116043233413755174");
        AssertCode(    // BX1 -X6 - X1, EQ B1 B3 334137, SA1 A7 - B4
            "0|L--|010000(1): 5 instructions",
            "1|L--|X1 = X1 ^ ~X6",
            "2|T--|if (B1 != B3) micro_goto 3",
            "3|T--|goto 334137",
            "4|L--|A1 = A7 - B4",
            "5|L--|X1 = Mem0[A1:word60]");
    }
}
