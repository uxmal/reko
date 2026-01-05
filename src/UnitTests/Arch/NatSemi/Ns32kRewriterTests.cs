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
using Reko.Arch.NatSemi;
using Reko.Core;

namespace Reko.UnitTests.Arch.NatSemi;

[TestFixture]
public class Ns32kRewriterTests : RewriterTestBase
{
    private readonly Ns32kArchitecture arch;

    public Ns32kRewriterTests()
    {
        this.arch = new Ns32kArchitecture(CreateServiceContainer(), "ns32k", []);
        this.LoadAddress = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => arch;


    public override Address LoadAddress { get; }

    [Test]
    public void Ns32kRw_absd()
    {
        Given_HexString("4E F3 C9 08");
        AssertCode(    // absd\t8(sp),r7
            "0|L--|00100000(4): 2 instructions",
            "1|L--|r7 = abs<word32>(Mem0[sp + 8<i32>:word32])",
            "2|L--|F = cond(r7)");
    }

    [Test]
    public void Ns32kRw_absf()
    {
        Given_HexString("BE B5 00");
        AssertCode(    // absf\tf0,f2
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|v6 = fabsf(v4)",
            "3|L--|f2 = SEQ(SLICE(f2, word32, 32), v6)");
    }

    [Test]
    public void Ns32kRw_acbb()
    {
        Given_HexString("CC 07 7D");
        AssertCode(    // acbb\t-1,r0,000FFFFD
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(r0, byte, 0)",
            "2|L--|v5 = v4 - 1<i8>",
            "3|L--|r0 = SEQ(SLICE(r0, word24, 8), v5)",
            "4|T--|if (v5 == 0<8>) branch 000FFFFD");
    }

    [Test]
    public void Ns32kRw_addb()
    {
        Given_HexString("40 00");
        AssertCode(    // addb\tr0,r1
            "0|L--|00100000(2): 5 instructions",
            "1|L--|v4 = SLICE(r1, byte, 0)",
            "2|L--|v6 = SLICE(r0, byte, 0)",
            "3|L--|v7 = v4 + v6",
            "4|L--|r1 = SEQ(SLICE(r1, word24, 8), v7)",
            "5|L--|CF = cond(v7)");
    }

    [Test]
    public void Ns32kRw_addcb()
    {
        Given_HexString("10 A0 20");
        AssertCode(    // addcb\tH'20,r0
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(r0, byte, 0)",
            "2|L--|v6 = v4 + 0x20<8> + C",
            "3|L--|r0 = SEQ(SLICE(r0, word24, 8), v6)",
            "4|L--|CF = cond(v6)");
    }

    [Test]
    public void Ns32kRw_addd()
    {
        Given_HexString("03 D6 04 7C");
        AssertCode(    // addd\t4(sb),-4(fp)
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v5 = Mem0[fp - 4<i32>:word32] + Mem0[sb + 4<i32>:word32]",
            "2|L--|Mem0[fp - 4<i32>:word32] = v5",
            "3|L--|CF = cond(v5)");
    }

    [Test]
    public void Ns32kRw_addf()
    {
        Given_HexString("BE C1 01");
        AssertCode(    // addf\tf0,f7++
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(f7, real32, 0)",
            "2|L--|v6 = SLICE(f0, real32, 0)",
            "3|L--|v7 = v4 + v6",
            "4|L--|f7 = SEQ(SLICE(f7, word32, 32), v7)");
    }

    [Test]
    public void Ns32kRw_addl()
    {
        Given_HexString("BE 80 16 10");
        AssertCode(    // addl\tf2,16(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = Mem0[sb + 16<i32>:real64] + f2",
            "2|L--|Mem0[sb + 16<i32>:word64] = v5");
    }

    [Test]
    public void Ns32kRw_addpb()
    {
        Given_HexString("4E FC D5 05");
        AssertCode(    // addpb\t5(sb),tos
            "0|L--|00100000(4): 6 instructions",
            "1|L--|v4 = Mem0[sp:byte]",
            "2|L--|sp = sp + 1<i32>",
            "3|L--|v6 = __add_packed_bcd<byte>(v4, Mem0[sb + 5<i32>:byte])",
            "4|L--|Mem0[sp:byte] = v6",
            "5|L--|C = cond(v6)",
            "6|L--|F = 0<32>");
    }

    [Test]
    public void Ns32kRw_addqb()
    {
        Given_HexString("0C 04");
        AssertCode(    // addqb\t-8,r0
            "0|L--|00100000(2): 4 instructions",
            "1|L--|v4 = SLICE(r0, byte, 0)",
            "2|L--|v5 = v4 + -8<i8>",
            "3|L--|r0 = SEQ(SLICE(r0, word24, 8), v5)",
            "4|L--|CF = cond(v5)");
    }

    [Test]
    public void Ns32kRw_addr()
    {
        Given_HexString("27 C0 04");
        AssertCode(    // addr\t4(fp),r0
            "0|L--|00100000(3): 1 instructions",
            "1|L--|r0 = fp + 4<i32>");
    }

    [Test]
    public void Ns32kRw_adjspd()
    {
        Given_HexString("7F C5 7C");
        AssertCode(    // adjspd\t-4(fp)
            "0|L--|00100000(3): 1 instructions",
            "1|L--|sp = sp - Mem0[fp - 4<i32>:word32]");
    }

    [Test]
    public void Ns32kRw_andb()
    {
        Given_HexString("68 00");
        AssertCode(    // andb\tr0,r1
            "0|L--|00100000(2): 4 instructions",
            "1|L--|v4 = SLICE(r1, byte, 0)",
            "2|L--|v6 = SLICE(r0, byte, 0)",
            "3|L--|v7 = v4 & v6",
            "4|L--|r1 = SEQ(SLICE(r1, word24, 8), v7)");
    }

    [Test]
    public void Ns32kRw_ashb()
    {
        Given_HexString("4E 84 BE 10");
        AssertCode(    // ashb\ttos,16(sb)
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v4 = Mem0[sp:byte]",
            "2|L--|sp = sp + 1<i32>",
            "3|L--|v6 = __shift_arithmetic<byte,byte>(Mem0[sb + 16<i32>:byte], v4)",
            "4|L--|Mem0[sb + 16<i32>:byte] = v6");
    }

    [Test]
    public void Ns32kRw_beq()
    {
        Given_HexString("0A 7E");
        AssertCode(    // beq\t000FFFFE
            "0|L--|00100000(2): 1 instructions",
            "1|T--|if (Test(EQ,Z)) branch 000FFFFE");
    }

    [Test]
    public void Ns32kRw_bcc()
    {
        Given_HexString("3A 7E");
        AssertCode(    // bcc\t000FFFFE
            "0|L--|00100000(2): 1 instructions",
            "1|T--|if (Test(UGE,C)) branch 000FFFFE");
    }

    [Test]
    public void Ns32kRw_bgt()
    {
        Given_HexString("6A 7E");
        AssertCode(    // bgt\t000FFFFE
            "0|L--|00100000(2): 1 instructions",
            "1|T--|if (Test(GT,N)) branch 000FFFFE");
    }

    [Test]
    public void Ns32kRw_ble()
    {
        Given_HexString("7A 7E");
        AssertCode(    // ble\t000FFFFE
            "0|L--|00100000(2): 1 instructions",
            "1|T--|if (Test(LE,N)) branch 000FFFFE");
    }

    [Test]
    public void Ns32kRw_bfs()
    {
        Given_HexString("8A 7E");
        AssertCode(    // bfs\t000FFFFE
            "0|L--|00100000(2): 1 instructions",
            "1|T--|if (F) branch 000FFFFE");
    }

    [Test]
    public void Ns32kRw_bfc()
    {
        Given_HexString("9A 7E");
        AssertCode(    // bfc\t000FFFFE
            "0|L--|00100000(2): 1 instructions",
            "1|T--|if (!F) branch 000FFFFE");
    }

    [Test]
    public void Ns32k_bic()
    {
        Given_HexString("88 06 03");
        AssertCode(    // bicb\tr0,3(sb)
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v5 = SLICE(r0, byte, 0)",
            "2|L--|v6 = Mem0[sb + 3<i32>:byte] & ~v5",
            "3|L--|Mem0[sb + 3<i32>:byte] = v6");
    }

    [Test]
    public void Ns32kRw_bicpsrb()
    {
        Given_HexString("7C A1 22");
        AssertCode(    // bicpsrb\tH'22
            "0|L--|00100000(3): 1 instructions",
            "1|L--|__bit_clear_psr(CONVERT(0x22<8>, byte, word16))");
    }

    [Test]
    public void Ns32kRw_bicpsrw()
    {
        Given_HexString("7D A1 00 22");
        AssertCode(    // bicpsrw\tH'22
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__bit_clear_psr(0x22<16>)");
    }

    [Test]
    public void Ns32kRw_bpt()
    {
        Given_HexString("F2");
        AssertCode(    // bpt
            "0|L--|00100000(1): 1 instructions",
            "1|L--|__breakpoint_trap()");
    }

    [Test]
    public void Ns32kRw_br()
    {
        Given_HexString("EA 0A");
        AssertCode(    // br\t0010000A
            "0|L--|00100000(2): 1 instructions",
            "1|T--|goto 0010000A");
    }

    [Test]
    public void Ns32kRw_bsr()
    {
        Given_HexString("02 10");
        AssertCode(    // bsr\t00100010
            "0|L--|00100000(2): 1 instructions",
            "1|T--|call 00100010 (4)");
    }

    [Test]
    public void Ns32kRw_caseb()
    {
        Given_HexString("7C E7 DF 04");
        AssertCode(    // caseb\t00100004[r7:b]
            "0|L--|00100000(4): 1 instructions",
            "1|T--|goto 0x00100000<p32> + CONVERT(Mem0[0x00100004<p32> + r7 * 1<32>:byte], byte, int32)");
    }

    [Test]
    public void Ns32kRw_cbitw()
    {
        Given_HexString("4E 49 02 00");
        AssertCode(    // cbitw\tr0,0(r1)
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v4 = SLICE(r0, int8, 0)",
            "2|L--|v6 = __clear_bit<word16,int8>(Mem0[r1:word16], v4)",
            "3|L--|Mem0[r1:word16] = v6",
            "4|L--|F = cond(v6)");
    }

    [Test]
    public void Ns32kRw_checkb()
    {
        Given_HexString("EE 80 D0 04");
        AssertCode(    // checkb\tr0,4(sb),r2
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v4 = SLICE(r2, byte, 0)",
            "2|L--|F = v4 >=u Mem0[r0:byte] && v4 <u Mem0[r0 + 1<32>:byte]",
            "3|L--|v7 = v4 - Mem0[r0 + 1<32>:byte]",
            "4|L--|r0 = SEQ(SLICE(r0, word24, 8), v7)");
    }

    [Test]
    public void Ns32kRw_cinv()
    {
        Given_HexString("1E A7 1B ");
        AssertCode(    // cinv\td,i,a,r3
            "0|L--|00100000(3): 1 instructions",
            "1|L--|__cache_invalidate(\"d,i,a\", r3)");
        Given_HexString("1E 27 19");
        AssertCode(    // cinv\ti,r3
            "0|L--|00100000(3): 1 instructions",
            "1|L--|__cache_invalidate(\"i\", r3)");
    }

    [Test]
    public void Ns32kRw_cmpb()
    {
        Given_HexString("04 D2 07 04");
        AssertCode(    // cmpb\t7(sb),4(r0)
            "0|L--|00100000(4): 1 instructions",
            "1|L--|LNZ = cond(Mem0[sb + 7<i32>:byte] - Mem0[r0 + 4<i32>:byte])");
    }

    [Test]
    public void Ns32kRw_cmpf()
    {
        Given_HexString("BE 89 00");
        AssertCode(    // cmpf\tf0,f2
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|v6 = SLICE(f2, real32, 0)",
            "3|L--|NZ = cond(v4 - v6)",
            "4|L--|L = 0<32>");
    }

    [Test]
    public void Ns32kRw_cmpmw()
    {
        Given_HexString("CE 45 42 0A 10 06");
        AssertCode(    // cmpmw\t10(r0),16(r1),6
            "0|L--|00100000(6): 1 instructions",
            "1|L--|LNZ = __compare_multiple<word16>(Mem0[r0 + 10<i32>:word16], Mem0[r1 + 16<i32>:word16], 6<32>)");
    }

    [Test]
    public void Ns32kRw_cmpqb()
    {
        Given_HexString("1C 04");
        AssertCode(    // cmpqb\t-8,r0
            "0|L--|00100000(2): 2 instructions",
            "1|L--|v4 = SLICE(r0, byte, 0)",
            "2|L--|LNZ = cond(-8<i8> - v4)");
    }

    [Test]
    public void Ns32kRw_cmpsb()
    {
        Given_HexString("0E 04 00");
        AssertCode(    // cmpsb
            "0|L--|00100000(3): 8 instructions",
            "1|L--|v7 = SLICE(r4, byte, 0)",
            "2|T--|if (r0 == 0<32>) branch 00100003",
            "3|L--|v8 = Mem0[r1:byte]",
            "4|L--|v9 = Mem0[r2:byte]",
            "5|L--|r1 = r1 + 1<i32>",
            "6|L--|r2 = r1 + 1<i32>",
            "7|L--|r0 = r0 - 1<32>",
            "8|T--|goto 00100000");
    }

    [Test]
    public void Ns32kRw_comb()
    {
        Given_HexString("4E 34 06 7C");
        AssertCode(    // comb\tr0,-4(fp)
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v4 = SLICE(r0, byte, 0)",
            "2|L--|v6 = ~v4",
            "3|L--|Mem0[fp - 4<i32>:byte] = v6");
    }

    [Test]
    public void Ns32kRw_cvtp()
    {
        Given_HexString("6E 83 D0 20");
        AssertCode(    // cvtp\tr0,32(sb),r2
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = (sb + 32<i32>) * 8<32> + r0",
            "2|L--|Mem0[sb + 32<i32>:word32] = v5");
    }

    [Test]
    public void Ns32kRw_cxp()
    {
        Given_HexString("22 01");
        AssertCode(    // cxp\t1
            "0|L--|00100000(2): 1 instructions",
            "1|L--|__call_external_procedure(1<32>)");
    }

    [Test]
    public void Ns32kRw_cxpd()
    {
        Given_HexString("7F D0 08");
        AssertCode(    // cxpd\t8(sb)
            "0|L--|00100000(3): 1 instructions",
            "1|L--|__call_external_procedure_descriptor(Mem0[sb + 8<i32>:ptr32])");
    }

    [Test]
    public void Ns32kRw_deiw()
    {
        Given_HexString("CE 2D 10");
        AssertCode(    // deiw\tr2,r0
            "0|L--|00100000(3): 8 instructions",
            "1|L--|v4 = SLICE(r2, word16, 0)",
            "2|L--|v7 = SLICE(r1, word16, 0)",
            "3|L--|v8 = SLICE(r0, word16, 0)",
            "4|L--|v9 = SEQ(v7, v8)",
            "5|L--|v10 = v9 /u v4",
            "6|L--|v11 = v9 %u v4",
            "7|L--|r0 = SEQ(SLICE(r0, word16, 16), v10)",
            "8|L--|r1 = SEQ(SLICE(r1, word16, 16), v11)");
    }

    [Test]
    public void Ns32kRw_divd()
    {
        Given_HexString("CE BF C6 7A 0C");
        AssertCode(    // divd\t-6(fp),12(sb)
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v5 = Mem0[sb + 12<i32>:word32] / Mem0[fp - 6<i32>:word32]",
            "2|L--|Mem0[sb + 12<i32>:word32] = v5");
    }

    [Test]
    public void Ns32kRw_divl()
    {
        Given_HexString("BE A0 C6 78 10");
        AssertCode(    // divl\t-8(fp),16(sb)
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v5 = Mem0[sb + 16<i32>:real64] / Mem0[fp - 8<i32>:real64]",
            "2|L--|Mem0[sb + 16<i32>:real64] = v5");
    }

    [Test]
    public void Ns32kRw_dotf()
    {
        Given_HexString("FE CD 10");
        AssertCode(    // dotf\tf2,f3
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(f3, real32, 0)",
            "2|L--|v6 = SLICE(f2, real32, 0)",
            "3|L--|v8 = SLICE(f0, real32, 0)",
            "4|L--|f0 = SEQ(SLICE(f0, word32, 32), v8 + v4 * v6)");
    }

    [Test]
    public void Ns32kRw_dotl()
    {
        Given_HexString("FE CC 10");
        AssertCode(    // dotl\tf2,f3
            "0|L--|00100000(3): 1 instructions",
            "1|L--|f0 = f0 + f3 * f2");
    }

    [Test]
    public void Ns32kRw_enter()
    {
        Given_HexString("82 85 10");
        AssertCode(    // enter\t[r0,r2,r7],H'10
            "0|L--|00100000(3): 10 instructions",
            "1|L--|sp = sp - 4<i32>",
            "2|L--|Mem0[sp:word32] = fp",
            "3|L--|fp = sp",
            "4|L--|sp = sp - 0x10<32>",
            "5|L--|sp = sp - 4<i32>",
            "6|L--|Mem0[sp:word32] = r0",
            "7|L--|sp = sp - 4<i32>",
            "8|L--|Mem0[sp:word32] = r2",
            "9|L--|sp = sp - 4<i32>",
            "10|L--|Mem0[sp:word32] = r7");
    }

    [Test]
    public void Ns32kRw_exit()
    {
        Given_HexString("92 A1");
        AssertCode(    // exit\t[r0,r2,r7]
            "0|L--|00100000(2): 9 instructions",
            "1|L--|Mem0[sp:word32] = r7",
            "2|L--|sp = sp + 4<i32>",
            "3|L--|Mem0[sp:word32] = r2",
            "4|L--|sp = sp + 4<i32>",
            "5|L--|Mem0[sp:word32] = r0",
            "6|L--|sp = sp + 4<i32>",
            "7|L--|sp = fp",
            "8|L--|fp = Mem0[sp:word32]",
            "9|L--|sp = sp + 4<i32>");
    }

    [Test]
    [Ignore("Too complex")]
    public void Ns32kRw_extb_complex()
    {
        Given_HexString("2E 00 D7 D1 0A 00 05");
        AssertCode(    // extb\tr0,10(sb),0(sb)[r1:b], 5
            "@@@");
    }

    [Test]
    public void Ns32kRw_extsw()
    {
        Given_HexString("CE 8D D0 10 86");
        AssertCode(    // extsw\t16(sb),r2,4,7
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v5 = __extract_field<word16>(Mem0[sb + 16<i32>:word16], 4<8>, 7<8>)",
            "2|L--|r2 = SEQ(SLICE(r2, word16, 16), v5)");
    }

    [Test]
    public void Ns32kRw_ffsb()
    {
        Given_HexString("6E C4 C5 7C");
        AssertCode(    // ffsb\t-4(fp),tos
            "0|L--|00100000(4): 3 instructions",
            "1|L--|F = Mem0[fp - 4<i32>:byte] == 0<8>",
            "2|L--|v6 = __find_first_set_bit<byte>(Mem0[fp - 4<i32>:byte], Mem0[sp:byte])",
            "3|L--|Mem0[sp:byte] = v6");
    }

    [Test]
    public void Ns32kRw_floorfb()
    {
        Given_HexString("3E 3C 00");
        AssertCode(    // floorfb\tf0,r0
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|v6 = CONVERT(floorf(v4), real32, int8)",
            "3|L--|r0 = SEQ(SLICE(r0, word24, 8), v6)");
    }

    [Test]
    public void Ns32kRw_floorld()
    {
        Given_HexString("3E BB 16 10");
        AssertCode(    // floorld\tf2,16(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = CONVERT(floor(f2), real64, int32)",
            "2|L--|Mem0[sb + 16<i32>:int32] = v5");
    }

    [Test]
    public void Ns32kRw_ibitw()
    {
        Given_HexString("4E 79 02 01");
        AssertCode(    // ibitw\tr0,1(r1)
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v4 = SLICE(r0, int8, 0)",
            "2|L--|v6 = __invert_bit<word16,int8>(Mem0[r1 + 1<i32>:word16], v4)",
            "3|L--|Mem0[r1 + 1<i32>:word16] = v6",
            "4|L--|F = cond(v6)");
    }

    [Test]
    public void Ns32kRw_indexb()
    {
        Given_HexString("2E 04 D6 14 7C");
        AssertCode(    // indexb\tr0,20(sb),-4(fp)
            "0|L--|00100000(5): 1 instructions",
            "1|L--|r0 = r0 *32 (Mem0[sb + 20<i32>:byte] + 1<8>) + Mem0[fp - 4<i32>:byte]");
    }

    [Test]
    public void Ns32kRw_insw()
    {
        Given_HexString("AE 41 12 00 07");
        AssertCode(    // insw\tr0,r2,0(r1),7
            "0|L--|00100000(5): 4 instructions",
            "1|L--|v4 = SLICE(r0, word16, 0)",
            "2|L--|v6 = SLICE(r2, word16, 0)",
            "3|L--|v8 = __insert_field<word16>(v6, v4, Mem0[r1:byte], 7<32>)",
            "4|L--|r2 = SEQ(SLICE(r2, word16, 16), v8)");
    }

    [Test]
    public void Ns32kRw_inssw()
    {
        Given_HexString("CE 89 16 10 86");
        AssertCode(    // inssw\tr2,16(sb),4,7
            "0|L--|00100000(5): 3 instructions",
        "1|L--|v4 = SLICE(r2, word16, 0)",
        "2|L--|v6 = __insert_field<word16>(Mem0[sb + 16<i32>:word16], v4, 4<8>, 7<8>)",
        "3|L--|Mem0[sb + 16<i32>:word16] = v6");
    }

    [Test]
    public void Ns32kRw_jsr()
    {
        Given_HexString("7F 96 04 00");
        AssertCode(    // jsr\t0(4(sb))
            "0|L--|00100000(4): 1 instructions",
            "1|T--|call Mem0[Mem0[sb + 4<i32>:ptr32]:ptr32] (4)");
    }

    [Test]
    public void Ns32kRw_jump()
    {
        Given_HexString("7F 82 78 00");
        AssertCode(    // jump\t0(-8(fp))
            "0|L--|00100000(4): 1 instructions",
            "1|T--|goto Mem0[Mem0[fp - 8<i32>:ptr32]:ptr32]");
    }

    [Test]
    public void Ns32kRw_lfsr()
    {
        Given_HexString("3E 0F 00");
        AssertCode(    // lfsr\tr0
            "0|L--|00100000(3): 1 instructions",
            "1|L--|FSR = r0");
    }

    [Test]
    public void Ns32kRw_lmr()
    {
        Given_HexString("1E 0B 06");
        AssertCode(    // lmr\tptb0,r0
            "0|L--|00100000(3): 1 instructions",
            "1|L--|__load_mmu_register<word32>(ptb0, r0)");
    }

    [Test]
    public void Ns32kRw_logbf()
    {
        Given_HexString("FE 95 18");
        AssertCode(    // logbf\tf3,f2
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(f3, real32, 0)",
            "2|L--|v6 = __logarithm_binary<real32>(v4)",
            "3|L--|f2 = SEQ(SLICE(f2, word32, 32), v6)");
    }

    [Test]
    public void Ns32kRw_lprw()
    {
        Given_HexString("ED D7 04");
        AssertCode(    // lprw\tmod,4(sb)
            "0|L--|00100000(3): 2 instructions",
            "1|L--|v4 = SLICE(mod, word16, 0)",
            "2|L--|__load_processor_register<word16>(v4, Mem0[sb + 4<i32>:word16])");
    }

    [Test]
    public void Ns32kRw_lshb()
    {
        Given_HexString("4E 94 C6 7C 08");
        AssertCode(    // lshb\t-4(fp),8(sb)
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v5 = __shift_logical<byte,byte>(Mem0[sb + 8<i32>:byte], Mem0[fp - 4<i32>:byte])",
            "2|L--|Mem0[sb + 8<i32>:byte] = v5");
    }

    [Test]
    public void Ns32kRw_meiw()
    {
        Given_HexString("CE A5 16 0A");
        AssertCode(    // meiw\tr2,10(sb)
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v4 = SLICE(r2, word16, 0)",
            "2|L--|v6 = Mem0[sb + 10<i32>:word16] *u32 v4",
            "3|L--|Mem0[sb + 10<i32>:uint32] = v6");
    }

    [Test]
    public void Ns32kRw_modb()
    {
        Given_HexString("CE B8 D6 04 08");
        AssertCode(    // modb\t4(sb),8(sb)
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v4 = Mem0[sb + 8<i32>:byte] %s Mem0[sb + 4<i32>:byte]",
            "2|L--|Mem0[sb + 8<i32>:byte] = v4");
    }

    [Test]
    public void Ns32kRw_movf()
    {
        Given_HexString("BE 85 06 08");
        AssertCode(    // movf\tf0,8(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|Mem0[sb + 8<i32>:real32] = v4");
    }

    [Test]
    public void Ns32kRw_movd()
    {
        Given_HexString("97 06 08");
        AssertCode(    // movd\tr0,8(sb)
            "0|L--|00100000(3): 1 instructions",
            "1|L--|Mem0[sb + 8<i32>:word32] = r0");
    }

    [Test]
    public void Ns32kRw_movbf()
    {
        Given_HexString("3E 04 A0 02");
        AssertCode(    // movbf\t2,f0
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = CONVERT(2<8>, byte, real32)",
            "2|L--|f0 = SEQ(SLICE(f0, word32, 32), v4)");
    }

    [Test]
    public void Ns32kRw_movdl()
    {
        Given_HexString("3E 83 D0 10");
        AssertCode(    // movdl\t16(sb),f2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f2 = CONVERT(Mem0[sb + 16<i32>:int32], int32, real64)");
    }

    [Test]
    public void Ns32kRw_movfl()
    {
        Given_HexString("3E 1B D0 08");
        AssertCode(    // movfl\t8(sb),f0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f0 = CONVERT(Mem0[sb + 8<i32>:real32], real32, real64)");
    }

    [Test]
    public void Ns32kRw_movlf()
    {
        Given_HexString("3E 96 06 0C");
        AssertCode(    // movlf\tf0,12(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = CONVERT(f0, real64, real32)",
            "2|L--|Mem0[sb + 12<i32>:real32] = v5");
    }

    [Test]
    public void Ns32kRw_movmw()
    {
        Given_HexString("CE 41 42 0A 10 06");
        AssertCode(    // movmw\t10(r0),16(r1),6
             "0|L--|00100000(6): 1 instructions",
"1|L--|__move_multiple<word16>(Mem0[r0 + 10<i32>:word16], Mem0[r1 + 16<i32>:word16], 6<32>)"
);
    }

    [Test]
    public void Ns32kRw_movqw()
    {
        Given_HexString("DD BB");
        AssertCode(    // movqw\t7,tos
            "0|L--|00100000(2): 3 instructions",
            "1|L--|sp = sp - 2<i32>",
            "2|L--|v4 = 7<i16>",
            "3|L--|Mem0[sp:int16] = v4");
    }

    [Test]
    public void Ns32kRw_movsub()
    {
        Given_HexString("AE 8C CE 05 09");
        AssertCode(    // movsub\t5(sp),9(sb)
            "0|L--|00100000(5): 1 instructions",
            "1|L--|__move_to_user_space<byte>(Mem0[sp + 5<i32>:ptr32], Mem0[sp + 5<i32>:ptr32])");
    }

    [Test]
    public void Ns32kRw_movusb()
    {
        Given_HexString("AE 5C D6 09 05");
        AssertCode(    // movusb\t9(sb),5(sp)
            "0|L--|00100000(5): 1 instructions",
            "1|L--|__move_to_supervisor_space<byte>(Mem0[sb + 9<i32>:ptr32], Mem0[sb + 9<i32>:ptr32])");
    }

    [Test]
    public void Ns32kRw_movxbw()
    {
        Given_HexString("CE 10 D0 02");
        AssertCode(    // movxbw\t2(sb),r0
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = CONVERT(Mem0[sb + 2<i32>:byte], byte, int16)",
            "2|L--|r0 = SEQ(SLICE(r0, word16, 16), v5)");
    }

    [Test]
    public void Ns32kRw_movzbw()
    {
        Given_HexString("CE 14 C0 7C");
        AssertCode(    // movzbw\t-4(fp),r0
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = CONVERT(Mem0[fp - 4<i32>:byte], byte, word16)",
            "2|L--|r0 = SEQ(SLICE(r0, word16, 16), v5)");
    }

    [Test]
    public void Ns32kRw_muld()
    {
        Given_HexString("CE A3 86 7C 04 03");
        AssertCode(    // muld\t4(-4(fp)),3(sb)
            "0|L--|00100000(6): 2 instructions",
            "1|L--|v5 = Mem0[sb + 3<i32>:word32] * Mem0[Mem0[fp - 4<i32>:ptr32] + 4<i32>:word32]",
            "2|L--|Mem0[sb + 3<i32>:word32] = v5");
    }

    [Test]
    public void Ns32kRw_mull()
    {
        Given_HexString("BE B0 C6 78 08");
        AssertCode(    // mull\t-8(fp),8(sb)
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v5 = Mem0[sb + 8<i32>:real64] * Mem0[fp - 8<i32>:real64]",
            "2|L--|Mem0[sb + 8<i32>:real64] = v5");
    }

    [Test]
    public void Ns32kRw_negf()
    {
        Given_HexString("BE 95 00");
        AssertCode(    // negf\tf0,f2
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|v6 = -v4",
            "3|L--|f2 = SEQ(SLICE(f2, word32, 32), v6)");
    }

    [Test]
    public void Ns32kRw_negw()
    {
        Given_HexString("4E A1 D6 04 06");
        AssertCode(    // negw\t4(sb),6(sb)
            "0|L--|00100000(5): 3 instructions",
            "1|L--|v4 = -Mem0[sb + 4<i32>:word16]",
            "2|L--|Mem0[sb + 6<i32>:word16] = v4",
            "3|L--|CF = cond(v4)");
    }

    [Test]
    public void Ns32kRw_nop()
    {
        Given_HexString("A2");
        AssertCode(    // nop
            "0|L--|00100000(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void Ns32kRw_notw()
    {
        Given_HexString("4E E5 4D 0A");
        AssertCode(    // notw\t10(r1),tos
            "0|L--|00100000(4): 3 instructions",
            "1|L--|sp = sp - 2<i32>",
            "2|L--|v5 = CONVERT(Mem0[r1 + 10<i32>:word16] == 0<16>, bool, word16)",
            "3|L--|Mem0[sp:word16] = v5");
    }

    [Test]
    public void Ns32kRw_orb()
    {
        Given_HexString("98 C6 7A 0B");
        AssertCode(    // orb\t-6(fp),11(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = Mem0[sb + 11<i32>:byte] | Mem0[fp - 6<i32>:byte]",
            "2|L--|Mem0[sb + 11<i32>:byte] = v5");
    }

    [Test]
    public void Ns32kRw_polyf()
    {
        Given_HexString("FE C9 10");
        AssertCode(    // polyf\tf2,f3
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(f2, real32, 0)",
            "2|L--|v6 = SLICE(f3, real32, 0)",
            "3|L--|v8 = SLICE(f0, real32, 0)",
            "4|L--|f0 = SEQ(SLICE(f0, word32, 32), v8 * v4 + v6)");
    }

    [Test]
    public void Ns32kRw_quow()
    {
        Given_HexString("CE B1 D6 04 08");
        AssertCode(    // quow\t4(sb),8(sb)
            "0|L--|00100000(5): 2 instructions",
            "1|L--|v4 = Mem0[sb + 8<i32>:word16] / Mem0[sb + 4<i32>:word16]",
            "2|L--|Mem0[sb + 8<i32>:word16] = v4");
    }

    [Test]
    public void Ns32kRw_rdval()
    {
        Given_HexString("1E 03 40 82 00");
        AssertCode(    // rdval\t512(r0)
            "0|L--|00100000(5): 1 instructions",
            "1|L--|F = __validate_read_address(Mem0[r0 + 512<i32>:ptr32])");
    }

    [Test]
    public void Ns32kRw_remb()
    {
        Given_HexString("CE B4 D6 04 08");
        AssertCode(    // remb\t4(sb),8(sb)
            "0|L--|00100000(5): 2 instructions",
        "1|L--|v4 = Mem0[sb + 8<i32>:byte] %s Mem0[sb + 4<i32>:byte]",
        "2|L--|Mem0[sb + 8<i32>:byte] = v4");
    }

    [Test]
    public void Ns32kRw_restore()
    {
        Given_HexString("72 A1");
        AssertCode(    // restore\t[r0,r2,r7]
            "0|L--|00100000(2): 6 instructions",
            "1|L--|Mem0[sp:word32] = r7",
            "2|L--|sp = sp + 4<i32>",
            "3|L--|Mem0[sp:word32] = r2",
            "4|L--|sp = sp + 4<i32>",
            "5|L--|Mem0[sp:word32] = r0",
            "6|L--|sp = sp + 4<i32>");
    }

    [Test]
    public void Ns32kRw_ret()
    {
        Given_HexString("12 10");
        AssertCode(    // ret\tH'10
            "0|L--|00100000(2): 1 instructions",
            "1|R--|return (4,16)");
    }

    [Test]
    public void Ns32kRw_reti()
    {
        Given_HexString("52");
        AssertCode(    // reti
            "0|L--|00100000(1): 2 instructions",
            "1|L--|__return_from_interrupt()",
            "2|R--|return (4,0)");
    }

    [Test]
    public void Ns32kRw_rett()
    {
        Given_HexString("42 10");
        AssertCode(    // rett\tH'10
            "0|L--|00100000(2): 2 instructions",
            "1|L--|__return_from_trap()",
            "2|R--|return (4,0)");
    }

    [Test]
    [Ignore("FD = -3,  so this is a negative offset, but incorrectly encoded?")]
    public void Ns32kRw_rotb()
    {
        Given_HexString("4E 40 A6 FD 10");
        AssertCode(    // rotb\t-3,16(sp)
            "@@@");
    }

    [Test]
    public void Ns32kRw_roundfb()
    {
        Given_HexString("3E 24 00");
        AssertCode(    // roundfb\tf0,r0
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|v6 = CONVERT(roundf(v4), real32, byte)",
            "3|L--|r0 = SEQ(SLICE(r0, word24, 8), v6)");
    }

    [Test]
    public void Ns32kRw_roundld()
    {
        Given_HexString("3E A3 16 0C");
        AssertCode(    // roundld\tf2,12(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = CONVERT(round(f2), real64, word32)",
            "2|L--|Mem0[sb + 12<i32>:word32] = v5");
    }

    [Test]
    public void Ns32kRw_rxp()
    {
        Given_HexString("32 10");
        AssertCode(    // rxp\tH'10
            "0|L--|00100000(2): 2 instructions",
            "1|L--|__return_external_procedure()",
            "2|R--|return (4,16)");
    }

    [Test]
    public void Ns32kRw_seqb()
    {
        Given_HexString("3C 00");
        AssertCode(    // seqb\tr0
            "0|L--|00100000(2): 2 instructions",
            "1|L--|v5 = CONVERT(Test(EQ,Z), bool, byte)",
            "2|L--|r0 = SEQ(SLICE(r0, word24, 8), v5)");
    }

    [Test]
    public void Ns32kRw_slow()
    {
        Given_HexString("3D D5 0A");
        AssertCode(    // slow\t10(sb)
            "0|L--|00100000(3): 2 instructions",
            "1|L--|v5 = CONVERT(Test(ULT,LZ), bool, word16)",
            "2|L--|Mem0[sb + 10<i32>:word16] = v5");
    }

    [Test]
    public void Ns32kRw_shid()
    {
        Given_HexString("3F BA");
        AssertCode(    // shid\ttos
            "0|L--|00100000(2): 2 instructions",
            "1|L--|v5 = CONVERT(Test(UGT,L), bool, word32)",
            "2|L--|Mem0[sp:word32] = v5"
    );
    }

    [Test]
    public void Ns32kRw_save()
    {
        Given_HexString("62 85");
        AssertCode(    // save\t[r0,r2,r7]
            "0|L--|00100000(2): 6 instructions",
            "1|L--|sp = sp - 4<i32>",
            "2|L--|Mem0[sp:word32] = r0",
            "3|L--|sp = sp - 4<i32>",
            "4|L--|Mem0[sp:word32] = r2",
            "5|L--|sp = sp - 4<i32>",
            "6|L--|Mem0[sp:word32] = r7");
    }

    [Test]
    public void Ns32kRw_sbitw()
    {
        Given_HexString("4E 59 02 01");
        AssertCode(    // sbitw\tr0,1(r1)
            "0|L--|00100000(4): 4 instructions",
            "1|L--|v4 = SLICE(r0, int8, 0)",
            "2|L--|v6 = __set_bit<word16,int8>(Mem0[r1 + 1<i32>:word16], v4)",
            "3|L--|Mem0[r1 + 1<i32>:word16] = v6",
            "4|L--|F = cond(v6)");
    }

    [Test]
    public void Ns32kRw_scalbf()
    {
        Given_HexString("FE 91 18");
        AssertCode(    // scalbf\tf3,f2
            "0|L--|00100000(3): 4 instructions",
            "1|L--|v4 = SLICE(f3, real32, 0)",
            "2|L--|v6 = SLICE(f2, real32, 0)",
            "3|L--|v7 = v6 * powf(2.0F, v4)",
            "4|L--|f2 = SEQ(SLICE(f2, word32, 32), v7)");
    }

    [Test]
    public void Ns32kRw_setcfg()
    {
        Given_HexString("0E 8B 03");
        AssertCode(    // setcfg\t[i,f,m]
            "0|L--|00100000(3): 1 instructions",
            "1|L--|__set_configuration(\"[i,f,m]\")");
    }

    [Test]
    public void Ns32kRw_sfsr()
    {
        Given_HexString("3E F7 05");
        AssertCode(    // sfsr\ttos
            "0|L--|00100000(3): 1 instructions",
            "1|L--|Mem0[sp:word16] = FSR");
    }

    [Test]
    public void Ns32kRw_skpsb()
    {
        Given_HexString("0E 0C 06");
        AssertCode(    // skpsb\tu
            "0|L--|00100000(3): 7 instructions",
            "1|L--|v6 = SLICE(r4, byte, 0)",
            "2|T--|if (r0 == 0<32>) branch 00100003",
            "3|L--|v7 = Mem0[r1:byte]",
            "4|T--|if (v7 == v6) branch 00100003",
            "5|L--|r1 = r1 + 1<i32>",
            "6|L--|r0 = r0 - 1<32>",
            "7|T--|goto 00100000");
    }

    [Test]
    public void Ns32kRw_smr()
    {
        Given_HexString("1E 0F 06");
        AssertCode(    // smr\tptb0,r0
            "0|L--|00100000(3): 1 instructions",
            "1|L--|r0 = __store_mmu_register<word32>(ptb0)");
    }

    [Test]
    public void Ns32kRw_sprw()
    {
        Given_HexString("AD D7 04");
        AssertCode(    // sprw\tmod,4(sb)
            "0|S--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(mod, word16, 0)",
            "2|L--|v6 = __store_processor_register<word16>(v4)",
            "3|L--|Mem0[sb + 4<i32>:word16] = v6");
    }

    [Test]
    public void Ns32kRw_subl()
    {
        Given_HexString("BE 90 16 10");
        AssertCode(    // subl\tf2,16(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = Mem0[sb + 16<i32>:real64] - f2",
            "2|L--|Mem0[sb + 16<i32>:real64] = v5");
    }

    [Test]
    public void Ns32kRw_subd()
    {
        Given_HexString("A3 D6 04 14");
        AssertCode(    // subd\t4(sb),20(sb)
            "0|L--|00100000(4): 3 instructions",
            "1|L--|v4 = Mem0[sb + 20<i32>:word32] - Mem0[sb + 4<i32>:word32]",
            "2|L--|Mem0[sb + 20<i32>:word32] = v4",
            "3|L--|CF = cond(v4)");
    }

    [Test]
    public void Ns32kRw_subcw()
    {
        Given_HexString("31 BE 78");
        AssertCode(    // subcw\ttos,-8(fp)
            "0|L--|00100000(3): 5 instructions",
            "1|L--|v5 = Mem0[sp:word16]",
            "2|L--|sp = sp + 2<i32>",
            "3|L--|v7 = Mem0[fp - 8<i32>:word16] - v5 - C",
            "4|L--|Mem0[fp - 8<i32>:word16] = v7",
            "5|L--|CF = cond(v7)");
    }

    [Test]
    public void Ns32kRw_subpd()
    {
        Given_HexString("4E 6F A0 00 00 00 99");
        AssertCode(    // subpd\tH'99,r1
            "0|L--|00100000(7): 3 instructions",
            "1|L--|r1 = __sub_packed_bcd<word32>(r1, 0x99<32>)",
            "2|L--|C = cond(r1)",
            "3|L--|F = 0<32>");
    }

    [Test]
    public void Ns32kRw_tbitw()
    {
        Given_HexString("75 02 00");
        AssertCode(    // tbitw\tr0,0(r1)
            "0|L--|00100000(3): 2 instructions",
            "1|L--|v4 = SLICE(r0, int8, 0)",
            "2|L--|F = __bit<word16,int8>(Mem0[r1:word16], v4)");
    }

    [Test]
    public void Ns32kRw_truncfb()
    {
        Given_HexString("3E 2C 00");
        AssertCode(    // truncfb\tf0,r0
            "0|L--|00100000(3): 3 instructions",
            "1|L--|v4 = SLICE(f0, real32, 0)",
            "2|L--|v6 = CONVERT(truncf(v4), real32, byte)",
            "3|L--|r0 = SEQ(SLICE(r0, word24, 8), v6)");
    }

    [Test]
    public void Ns32kRw_truncld()
    {
        Given_HexString("3E AB 16 08");
        AssertCode(    // truncld\tf2,8(sb)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v5 = CONVERT(trunc(f2), real64, word32)",
            "2|L--|Mem0[sb + 8<i32>:word32] = v5");
    }

    [Test]
    public void Ns32kRw_wait()
    {
        Given_HexString("B2");
        AssertCode(    // wait
            "0|L--|00100000(1): 1 instructions",
            "1|L--|__wait()");
    }

    [Test]
    public void Ns32kRw_wrval()
    {
        Given_HexString("1E 07 40 82 00");
        AssertCode(    // wrval\t512(r0)
            "0|L--|00100000(5): 1 instructions",
            "1|L--|F = __validate_write_address(Mem0[r0 + 512<i32>:ptr32])");
    }

    [Test]
    public void Ns32kRw_xorb()
    {
        Given_HexString("38 C6 78 7C");
        AssertCode(    // xorb\t-8(fp),-4(fp)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = Mem0[fp - 4<i32>:byte] ^ Mem0[fp - 8<i32>:byte]",
            "2|L--|Mem0[fp - 4<i32>:byte] = v4");
    }
}
