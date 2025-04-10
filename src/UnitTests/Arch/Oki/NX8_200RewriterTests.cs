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
using NUnit.Framework.Internal;
using Reko.Arch.Oki;
using Reko.Core;
using Reko.Core.Machine;

namespace Reko.UnitTests.Arch.Oki;

public class NX8_200RewriterTests : RewriterTestBase
{
    private readonly NX8_200Architecture arch;

    public NX8_200RewriterTests()
    {
        this.arch = new NX8_200Architecture(CreateServiceContainer(), "nx8/200", []);
        this.LoadAddress = Address.Ptr16(0x1000);
    }

    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress { get; }

    [Test]
    public void NX8_200Rw_adc()
    {
        Given_HexString("6734121A");
        AssertCode(     // adc	a,er2
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(1): 2 instructions",
            "3|L--|a = a + er2 + C",
            "4|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_adcb()
    {
        Given_HexString("1E");
        AssertCode(     // adcb	a,r6
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) + r6 + C)",
            "2|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_addb()
    {
        Given_HexString("C50782");
        AssertCode(     // addb	a,07h
            "0|L--|1000(3): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) + Mem0[0x0007<p16>:byte])",
            "2|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_add()
    {
        Given_HexString("6734128600E2");
        AssertCode(     // add	a,#0E200h
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(3): 2 instructions",
            "3|L--|a = a + 0xE200<16>",
            "4|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_and()
    {
        Given_HexString("673412C10000D03FF5");
        AssertCode(     // and	[dsr:x2],#0F53Fh
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(6): 2 instructions",
            "3|L--|Mem0[x2:word16] = Mem0[x2:word16] & 0xF53F<16>",
            "4|L--|Z = cond(Mem0[x2:word16] & 0xF53F<16>)");
    }

    [Test]
    public void NX8_200Rw_andb()
    {
        Given_HexString("D637");
        AssertCode(     // andb	a,#037h
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) & 0x37<8>)",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void NX8_200Rw_brk()
    {
        Given_HexString("FF");
        AssertCode(     // brk
            "0|H--|1000(1): 1 instructions",
            "1|L--|__brk()");
    }

    [Test]
    public void NX8_200Rw_cal()
    {
        Given_HexString("326D45");
        AssertCode(     // cal	456D
            "0|T--|1000(3): 1 instructions",
            "1|T--|call 456D (2)");
    }

    [Test]
    public void NX8_200Rw_clr()
    {
        Given_HexString("673412A415");
        AssertCode(     // clr	lrb
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(2): 1 instructions",
            "3|L--|lrb = 0<16>");
    }

    [Test]
    public void NX8_200Rw_clrb()
    {
        Given_HexString("FA");
        AssertCode(     // clrb	a
            "0|L--|1000(1): 1 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), 0<8>)");
    }

    [Test]
    public void NX8_200Rw_cmp()
    {
        Given_HexString("A0C05555");
        AssertCode(     // cmp	ssp,#05555h
            "0|L--|1000(4): 1 instructions",
            "1|L--|CZ = cond(ssp - 0x5555<16>)");
    }

    [Test]
    public void NX8_200Rw_cmpb()
    {
        Given_HexString("C663");
        AssertCode(     // cmpb	a,#063h
            "0|L--|1000(2): 1 instructions",
            "1|L--|CZ = cond(SLICE(a, byte, 0) - 0x63<8>)");
    }

    [Test]
    public void NX8_200Rw_cmpc()
    {
        Given_HexString("449E448E");
        AssertCode(     // cmpc	a,#08E44h
            "0|L--|1000(4): 1 instructions",
            "1|L--|CZ = cond(a - 0x8E44<16>)");
    }

    [Test]
    public void NX8_200Rw_daa()
    {
        Given_HexString("93");
        AssertCode(     // daa
            "0|L--|1000(1): 3 instructions",
            "1|L--|v4 = __daa(SLICE(a, byte, 0))",
            "2|L--|a = SEQ(SLICE(a, byte, 8), v4)",
            "3|L--|C = cond(v4)");
    }

    [Test]
    public void NX8_200Rw_das()
    {
        Given_HexString("94");
        AssertCode(     // das
            "0|L--|1000(1): 3 instructions",
            "1|L--|v4 = __das(SLICE(a, byte, 0))",
            "2|L--|a = SEQ(SLICE(a, byte, 8), v4)",
            "3|L--|C = cond(v4)");
    }

    [Test]
    public void NX8_200Rw_dec_dp()
    {
        Given_HexString("82");
        AssertCode(     // dec	dp
            "0|L--|1000(1): 2 instructions",
            "1|L--|dp = dp - 1<16>",
            "2|L--|Z = cond(dp)");
    }

    [Test]
    public void NX8_200Rw_dec_mem()
    {
        Given_HexString("C5D617");
        AssertCode(     // dec 0D6h
            "0|L--|1000(3): 2 instructions",
            "1|L--|Mem0[0x00D6<p16>:word16] = Mem0[0x00D6<p16>:word16] - 1<16>",
            "2|L--|Z = cond(Mem0[0x00D6<p16>:word16] - 1<16>)");
    }

    [Test]
    public void NX8_200Rw_decb()
    {
        Given_HexString("C217");
        AssertCode(     // decb	[dsr:dp]
            "0|L--|1000(2): 2 instructions",
            "1|L--|Mem0[dp:word16] = Mem0[dp:word16] - 1<16>",
            "2|L--|Z = cond(Mem0[dp:word16] - 1<16>)");
    }

    [Test]
    public void NX8_200Rw_div()
    {
        Given_HexString("9037");
        AssertCode(     // div
            "0|L--|1000(2): 3 instructions",
            "1|L--|er0_a = er0_a /u er2",
            "2|L--|er1 = er0_a %u er2",
            "3|L--|CZ = cond(er0_a)");
    }

    [Test]
    public void NX8_200Rw_divb()
    {
        Given_HexString("A236");
        AssertCode(     // divb
            "0|L--|1000(2): 3 instructions",
            "1|L--|a = a /u r0",
            "2|L--|r1 = a %u r0",
            "3|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_extnd()
    {
        Given_HexString("F8");
        AssertCode(     // extnd
            "0|L--|1000(1): 2 instructions",
            "1|L--|v4 = SLICE(a, byte, 0)",
            "2|L--|a = CONVERT(v4, int8, int16)");
    }

    [Test]
    public void NX8_200Rw_j()
    {
        Given_HexString("034C07");
        AssertCode(     // j	074C
            "0|T--|1000(3): 1 instructions",
            "1|T--|goto 074C");
    }

    [Test]
    public void NX8_200Rw_jbr()
    {
        Given_HexString("D83308");
        AssertCode(     // jbr	off 033h.0,08E0
            "0|L--|1000(3): 1 instructions",
            "1|T--|if (!__bit<byte,byte>(Mem0[lrb + 51<i16>:byte], 0<8>)) branch 100B");
    }

    [Test]
    public void NX8_200Rw_jbs()
    {
        Given_HexString("E89E02");
        AssertCode(     // jbs	off 09Eh.0,0988
            "0|L--|1000(3): 1 instructions",
            "1|T--|if (__bit<byte,byte>(Mem0[lrb + 158<i16>:byte], 0<8>)) branch 1005");
    }


    [Test]
    public void NX8_200Rw_jeq()
    {
        Given_HexString("C90B");
        AssertCode(     // jeq	100D
            "0|T--|1000(2): 1 instructions",
            "1|T--|if (Test(EQ,Z)) branch 100D");
    }

    [Test]
    public void NX8_200Rw_jge()
    {
        Given_HexString("CDC6");
        AssertCode(     // jge	075C
            "0|T--|1000(2): 1 instructions",
            "1|T--|if (Test(UGE,C)) branch 0FC8");
    }

    [Test]
    public void NX8_200Rw_jlt()
    {
        Given_HexString("CA59");
        AssertCode(     // jlt	105B
            "0|T--|1000(2): 1 instructions",
            "1|T--|if (Test(ULT,C)) branch 105B");
    }

    [Test]
    public void NX8_200Rw_jne()
    {
        Given_HexString("CE07");
        AssertCode(     // jne	1009
            "0|T--|1000(2): 1 instructions",
            "1|T--|if (Test(NE,Z)) branch 1009");
    }

    [Test]
    public void NX8_200Rw_jrnz()
    {
        Given_HexString("30F6");
        AssertCode(     // jrnz	dp,09BF
            "0|T--|1000(2): 4 instructions",
            "1|L--|v4 = SLICE(dp, byte, 0)",
            "2|L--|v4 = v4 - 1<8>",
            "3|L--|dp = SEQ(SLICE(dp, byte, 8), v4)",
            "4|T--|if (v4 != 0<8>) branch 0FF8");
    }

    [Test]
    public void NX8_200Rw_lb()
    {
        Given_HexString("78");
        AssertCode(     // lb	a,r0
            "0|L--|1000(1): 1 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), r0)");
    }

    [Test]
    public void NX8_200Rw_lc()
    {
        Given_HexString("92A8");
        AssertCode(     // lc	a,dp
            "0|L--|1000(2): 1 instructions",
            "1|L--|a = dp");
    }


    [Test]
    public void NX8_200Rw_mb()
    {
        Given_HexString("C52C2C");
        AssertCode(     // mb	C,02Ch.4
            "0|L--|1000(3): 1 instructions",
            "1|L--|C = __bit<byte,byte>(Mem0[0x002C<p16>:byte], 4<8>)");
    }

    [Test]
    public void NX8_200Rw_mbr()
    {
        Given_HexString("C4AE21");
        AssertCode(     // mbr	C,off 0AEh
            "0|L--|1000(3): 2 instructions",
            "1|L--|v6 = SLICE(a, byte, 0)",
            "2|L--|C = __bit<byte,byte>(Mem0[lrb + 174<i16>:byte], v6)");
    }

    [Test]
    public void NX8_200Rw_mov()
    {
        Given_HexString("A0987F04");
        AssertCode(     // mov	ssp,#047Fh
            "0|L--|1000(4): 1 instructions",
            "1|L--|ssp = 0x47F<16>");
    }

    [Test]
    public void NX8_200Rw_movb()
    {
        Given_HexString("C5D49849");
        AssertCode(     // movb	0D4h,#049h
            "0|L--|1000(4): 1 instructions",
            "1|L--|Mem0[0x00D4<p16>:byte] = 0x49<8>");
    }

    [Test]
    public void NX8_200Rw_mul()
    {
        Given_HexString("9035");
        AssertCode(     // mul
            "0|L--|1000(2): 2 instructions",
            "1|L--|er1_a = a *u32 er0",
            "2|L--|Z = cond(er1_a)");
    }

    [Test]
    public void NX8_200Rw_mulb()
    {
        Given_HexString("A234");
        AssertCode(     // mulb
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = SLICE(a, byte, 0) *u16 r0",
            "2|L--|Z = cond(a)");
    }


    [Test]
    public void NX8_200Rw_nop()
    {
        Given_HexString("00");
        AssertCode(     // nop
            "0|L--|1000(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void NX8_200Rw_or()
    {
        // 673412C10000D03FF5
        Given_HexString("673412E62040");
        AssertCode(     // or	a,#01520h
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(3): 2 instructions",
            "3|L--|a = a | 0x4020<16>",
            "4|L--|Z = cond(a)");
    }

    [Test]
    public void NX8_200Rw_pops()
    {
        Given_HexString("65");
        AssertCode(     // pops	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|ssp = ssp + 2<i16>",
            "2|L--|a = Mem0[ssp:word16]");
    }

    [Test]
    public void NX8_200Rw_pushs()
    {
        Given_HexString("55");
        AssertCode(     // pushs	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|Mem0[ssp:word16] = a",
            "2|L--|ssp = ssp - 2<i16>");
    }

    [Test]
    public void NX8_200Rw_rb()
    {
        Given_HexString("C59F0F");
        AssertCode(     // rb	09Fh.7
            "0|L--|1000(3): 1 instructions",
            "1|L--|Mem0[0x009F<p16>:byte] = __write_bit<byte,byte>(Mem0[0x009F<p16>:byte], 7<8>, false)");
    }

    [Test]
    public void NX8_200Rw_rc()
    {
        Given_HexString("95");
        AssertCode(     // rc
            "0|L--|1000(1): 1 instructions",
            "1|L--|C = false");
    }

    [Test]
    public void NX8_200Rw_rol()
    {
        Given_HexString("33");
        AssertCode(     // rol	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), __rcl<byte,byte>(SLICE(a, byte, 0), 1<8>, C))",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_rolb()
    {
        Given_HexString("33");
        AssertCode(     // rolb	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), __rcl<byte,byte>(SLICE(a, byte, 0), 1<8>, C))",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_ror()
    {
        Given_HexString("43");
        AssertCode(     // ror	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), __rcr<byte,byte>(SLICE(a, byte, 0), 1<8>, C))",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_rt()
    {
        Given_HexString("01");
        AssertCode(     // rt
            "0|R--|1000(1): 1 instructions",
            "1|R--|return (2,0)");
    }

    [Test]
    public void NX8_200Rw_rti()
    {
        Given_HexString("02");
        AssertCode(     // rti
            "0|R--|1000(1): 2 instructions",
            "1|L--|__return_from_interrupt()",
            "2|R--|return (2,0)");
    }

    [Test]
    public void NX8_200Rw_sb()
    {
        Given_HexString("C59E18");
        AssertCode(     // sb	09Eh.0
            "0|L--|1000(3): 1 instructions",
            "1|L--|Mem0[0x009E<p16>:byte] = __write_bit<byte,byte>(Mem0[0x009E<p16>:byte], 0<8>, true)");
    }

    [Test]
    public void NX8_200Rw_sbc()
    {
        Given_HexString("67341238");
        AssertCode(     // sbc	a,er0
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(1): 2 instructions",
            "3|L--|a = a - er0 - C",
            "4|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_sbcb()
    {
        Given_HexString("3C");
        AssertCode(     // sbcb	a,r4
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) - r4 - C)",
            "2|L--|CZ = cond(a)");
    }

    [Test]
    public void NX8_200Rw_sbr()
    {
        Given_HexString("C0100111");
        AssertCode(     // sbr	272[dsr:x1]
            "0|L--|1000(4): 2 instructions",
            "1|L--|v4 = SLICE(a, byte, 0)",
            "2|L--|Mem0[x1 + 272<i16>:byte] = __write_bit<byte,byte>(Mem0[x1 + 272<i16>:byte], v4, true)");
    }



    [Test]
    public void NX8_200Rw_sc()
    {
        Given_HexString("85");
        AssertCode(     // sc
            "0|L--|1000(1): 1 instructions",
            "1|L--|C = true");
    }

    [Test]
    public void NX8_200Rw_scal()
    {
        Given_HexString("313D");
        AssertCode(     // scal	0482
            "0|T--|1000(2): 1 instructions",
            "1|T--|call 103F (2)");
    }

    [Test]
    public void NX8_200Rw_sll()
    {
        Given_HexString("53");
        AssertCode(     // sll	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) << 1<8>)",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_sllb()
    {
        Given_HexString("53");
        AssertCode(     // sllb	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) << 1<8>)",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_sra()
    {
        Given_HexString("73");
        AssertCode(     // sra	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) >> 1<8>)",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_srab()
    {
        Given_HexString("73");
        AssertCode(     // srab	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) >> 1<8>)",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_srl()
    {
        Given_HexString("63");
        AssertCode(     // srl	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) >>u 1<8>)",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_srlb()
    {
        Given_HexString("63");
        AssertCode(     // srlb	a
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) >>u 1<8>)",
            "2|L--|C = cond(a)");
    }

    [Test]
    public void NX8_200Rw_st()
    {
        Given_HexString("674312D51A");
        AssertCode(     // st	a,01Ah
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1243<16>",
            "2|L--|1003(2): 1 instructions",
            "3|L--|Mem0[0x001A<p16>:word16] = a");
    }

    [Test]
    public void NX8_200Rw_stb()
    {
        Given_HexString("D2");
        AssertCode(     // stb	a,[dsr:dp]
            "0|L--|1000(1): 1 instructions",
            "1|L--|Mem0[dp:byte] = SLICE(a, byte, 0)");
    }

    [Test]
    public void NX8_200Rw_swap()
    {
        Given_HexString("67341283");
        AssertCode(     // swap
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(1): 1 instructions",
            "3|L--|a = __swap_bytes(a)");
    }

    [Test]
    public void NX8_200Rw_swapb()
    {
        Given_HexString("83");
        AssertCode(     // swapb
            "0|L--|1000(1): 1 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), __swap_nybbles(SLICE(a, byte, 0)))");
    }

    [Test]
    public void NX8_200Rw_tbr()
    {
        Given_HexString("C004CE13");
        AssertCode(     // tbr	52740[dsr:x1]
            "0|L--|1000(4): 1 instructions",
            "1|L--|Z = __bit<byte,byte>(Mem0[x1 + -12796<i16>:byte], SLICE(a, byte, 0))");
    }

    [Test]
    public void NX8_200Rw_vcal()
    {
        Given_HexString("11");
        AssertCode(     // vcal	002A
            "0|T--|1000(1): 1 instructions",
            "1|T--|call 002A (2)");
    }

    [Test]
    public void NX8_200Rw_xchg()
    {
        Given_HexString("B5B810");
        AssertCode(     // xchg	a,0B8h
            "0|L--|1000(3): 3 instructions",
            "1|L--|v3 = a",
            "2|L--|a = Mem0[0x00B8<p16>:word16]",
            "3|L--|Mem0[0x00B8<p16>:word16] = v3");
    }

    [Test]
    public void NX8_200Rw_xchgb()
    {
        Given_HexString("2010");
        AssertCode(     // xchgb	a,r0
            "0|L--|1000(2): 3 instructions",
            "1|L--|v3 = SLICE(a, byte, 0)",
            "2|L--|a = SEQ(SLICE(a, byte, 8), r0)",
            "3|L--|r0 = v3");
    }

    [Test]
    public void NX8_200Rw_xnbl()
    {
        Given_HexString("B3EC");
        AssertCode(     // xnbl	off 0ECh
            "0|L--|1000(2): 3 instructions",
            "1|L--|v4 = SLICE(a, byte, 0)",
            "2|L--|v4 = __exchange_nybble(lrb + 236<i16>, v4)",
            "3|L--|a = SEQ(SLICE(a, byte, 8), v4)");
    }

    [Test]
    public void NX8_200Rw_xor()
    {
        Given_HexString("673412F6FFFF");
        AssertCode(     // xor	a,#0FFFFh
            "0|L--|1000(3): 1 instructions",
            "1|L--|a = 0x1234<16>",
            "2|L--|1003(3): 2 instructions",
            "3|L--|a = a ^ 0xFFFF<16>",
            "4|L--|Z = cond(a)");
    }

    [Test]
    public void NX8_200Rw_xorb()
    {
        Given_HexString("F6F0");
        AssertCode(     // xorb	a,#0F0h
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = SEQ(SLICE(a, byte, 8), SLICE(a, byte, 0) ^ 0xF0<8>)",
            "2|L--|Z = cond(a)");
    }
}
