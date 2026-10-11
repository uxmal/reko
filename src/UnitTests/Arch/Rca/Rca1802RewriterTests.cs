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
using Reko.Arch.Rca;
using Reko.Core;

namespace Reko.UnitTests.Arch.Rca;

public class Rca1802RewriterTests : RewriterTestBase
{
    private readonly Rca1802Architecture arch;
    private readonly Address addr;

    public Rca1802RewriterTests()
    {
        this.arch = new Rca1802Architecture(CreateServiceContainer(), "rca1802", []);
        this.addr = Address.Ptr16(0x0100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;


    [Test]
    public void Rca1802Rw_ADD()
    {
        Given_HexString("F4");
        AssertCode(     // ADD
            "0|L--|0100(1): 4 instructions",
            "1|L--|v5 = __readX(X)",
            "2|L--|v5 = Mem0[v5:byte]",
            "3|L--|D = v5 + D",
            "4|L--|DF = cond(D)");
    }

    [Test]
    public void Rca1802Rw_ADI()
    {
        Given_HexString("FCF8");
        AssertCode(     // ADI	F8
            "0|L--|0100(2): 2 instructions",
            "1|L--|D = D + 0xF8<8>",
            "2|L--|DF = cond(D)");
    }

    [Test]
    public void Rca1802Rw_AND()
    {
        Given_HexString("F2");
        AssertCode(     // AND
            "0|L--|0100(1): 4 instructions",
            "1|L--|v5 = __readX(X)",
            "2|L--|v5 = Mem0[v5:byte]",
            "3|L--|D = v5 & D",
            "4|L--|DF = cond(D)");
    }


    [Test]
    public void Rca1802Rw_ANI()
    {
        Given_HexString("FA1F");
        AssertCode(     // ANI	1F
            "0|L--|0100(2): 1 instructions",
            "1|L--|D = D & 0x1F<8>");
    }

    [Test]
    public void Rca1802Rw_B2()
    {
        Given_HexString("35A7");
        AssertCode(     // B2	00A7
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (__is_EF2_set()) branch 01A7");
    }

    [Test]
    public void Rca1802Rw_B3()
    {
        Given_HexString("3617");
        AssertCode(     // B3	0817
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (__is_EF3_set()) branch 0117");
    }

    [Test]
    public void Rca1802Rw_B4()
    {
        Given_HexString("3703");
        AssertCode(     // B4	0403
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (__is_EF4_set()) branch 0103");
    }

    [Test]
    public void Rca1802Rw_BDF()
    {
        Given_HexString("333E");
        AssertCode(     // BDF	013E
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (__is_DF_set()) branch 013E");
    }

    [Test]
    public void Rca1802Rw_BN1()
    {
        Given_HexString("3C11");
        AssertCode(     // BN1	0211
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (!__is_EF1_set()) branch 0111");
    }

    [Test]
    public void Rca1802Rw_BN2()
    {
        Given_HexString("3DD5");
        AssertCode(     // BN2	06D5
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (!__is_EF2_set()) branch 01D5");
    }

    [Test]
    public void Rca1802Rw_BN3()
    {
        Given_HexString("3E46");
        AssertCode(     // BN3	0146
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (!__is_EF3_set()) branch 0146");
    }

    [Test]
    public void Rca1802Rw_BN4()
    {
        Given_HexString("3FBD");
        AssertCode(     // BN4	01BD
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (!__is_EF4_set()) branch 01BD");
    }

    [Test]
    public void Rca1802Rw_BNF()
    {
        Given_HexString("3B05");
        AssertCode(     // BNF	0105
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (!__is_DF_set()) branch 0105");
    }

    [Test]
    public void Rca1802Rw_BNQ()
    {
        Given_HexString("392A");
        AssertCode(     // BNQ	022A
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (!__is_Q_set()) branch 012A");
    }

    [Test]
    public void Rca1802Rw_BNZ()
    {
        Given_HexString("3AC8");
        AssertCode(     // BNZ	01C8
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (D != 0<8>) branch 01C8");
    }

    [Test]
    public void Rca1802Rw_BQ()
    {
        Given_HexString("3172");
        AssertCode(     // BQ	0272
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (__is_Q_set()) branch 0172");
    }

    [Test]
    public void Rca1802Rw_BR()
    {
        Given_HexString("300F");
        AssertCode(     // BR	010F
            "0|T--|0100(2): 1 instructions",
            "1|T--|goto 010F");
    }

    [Test]
    public void Rca1802Rw_BZ()
    {
        Given_HexString("322E");
        AssertCode(     // BZ	012E
            "0|T--|0100(2): 1 instructions",
            "1|T--|if (D == 0<8>) branch 012E");
    }

    [Test]
    public void Rca1802Rw_CALL()
    {
        Given_HexString("D4023E");
        AssertCode(     // CALL	023E
            "0|T--|0100(3): 1 instructions",
            "1|T--|call 023E (2)");
    }

    [Test]
    public void Rca1802Rw_DEC()
    {
        Given_HexString("20");
        AssertCode(     // DEC	R0
            "0|L--|0100(1): 1 instructions",
            "1|L--|R0 = R0 - 1<16>");
    }

    [Test]
    public void Rca1802Rw_EXIT()
    {
        Given_HexString("D5");
        AssertCode(     // EXIT
            "0|R--|0100(1): 1 instructions",
            "1|R--|return (2,0)");
    }

    [Test]
    public void Rca1802Rw_GHI()
    {
        Given_HexString("98");
        AssertCode(     // GHI	R8
            "0|L--|0100(1): 1 instructions",
            "1|L--|D = SLICE(R8, byte, 8)");
    }

    [Test]
    public void Rca1802Rw_GLO()
    {
        Given_HexString("80");
        AssertCode(     // GLO	R0
            "0|L--|0100(1): 1 instructions",
            "1|L--|D = SLICE(R0, byte, 0)");
    }

    [Test]
    public void Rca1802Rw_IDL()
    {
        Given_HexString("00");
        AssertCode(     // IDL
            "0|L--|0100(1): 1 instructions",
            "1|L--|__idle()");
    }

    [Test]
    public void Rca1802Rw_INC()
    {
        Given_HexString("14");
        AssertCode(     // INC	R4
            "0|L--|0100(1): 1 instructions",
            "1|L--|R4 = R4 + 1<16>");
    }

    [Test]
    public void Rca1802Rw_INP1()
    {
        Given_HexString("69");
        AssertCode(     // INP1	09
            "0|L--|0100(1): 3 instructions",
            "1|L--|D = __input(1<8>)",
            "2|L--|X = D",
            "3|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_INP2()
    {
        Given_HexString("6A");
        AssertCode(     // INP2	0A
            "0|L--|0100(1): 3 instructions",
            "1|L--|D = __input(2<8>)",
            "2|L--|X = D",
            "3|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_INP4()
    {
        Given_HexString("6C");
        AssertCode(     // INP4	0C
            "0|L--|0100(1): 3 instructions",
            "1|L--|D = __input(4<8>)",
            "2|L--|X = D",
            "3|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_INP6()
    {
        Given_HexString("6E");
        AssertCode(     // INP6	0E
            "0|L--|0100(1): 3 instructions",
            "1|L--|D = __input(6<8>)",
            "2|L--|X = D",
            "3|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_IRX()
    {
        Given_HexString("60");
        AssertCode(     // IRX
            "0|L--|0100(1): 1 instructions",
            "1|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_LBDF()
    {
        Given_HexString("C32848");
        AssertCode(     // LBDF	2848
            "0|T--|0100(3): 1 instructions",
            "1|T--|if (__is_DF_set()) branch 2848");
    }

    [Test]
    public void Rca1802Rw_LBQ()
    {
        Given_HexString("C1D406");
        AssertCode(     // LBQ	D406
            "0|T--|0100(3): 1 instructions",
            "1|T--|if (__is_Q_set()) branch D406");
    }

    [Test]
    public void Rca1802Rw_LBR()
    {
        Given_HexString("C0F000");
        AssertCode(     // LBR	F000
            "0|T--|0100(3): 1 instructions",
            "1|T--|goto F000");
    }

    [Test]
    public void Rca1802Rw_LDA()
    {
        Given_HexString("40");
        AssertCode(     // LDA	0R0
            "0|L--|0100(1): 2 instructions",
            "1|L--|D = Mem0[R0:byte]",
            "2|L--|R0 = R0 + 1<i16>");
    }

    [Test]
    public void Rca1802Rw_LDI()
    {
        Given_HexString("F800");
        AssertCode(     // LDI	00
            "0|L--|0100(2): 1 instructions",
            "1|L--|D = 0<8>");
    }

    [Test]
    public void Rca1802Rw_LDN()
    {
        Given_HexString("01");
        AssertCode(     // LDN	R1
            "0|L--|0100(1): 1 instructions",
            "1|L--|D = Mem0[R1:byte]");
    }

    [Test]
    public void Rca1802Rw_LDX()
    {
        Given_HexString("F0");
        AssertCode(     // LDX
            "0|L--|0100(1): 2 instructions",
            "1|L--|v3 = __readX(X)",
            "2|L--|D = Mem0[v3:byte]");
    }

    [Test]
    public void Rca1802Rw_LDXA()
    {
        Given_HexString("72");
        AssertCode(     // LDXA
            "0|L--|0100(1): 3 instructions",
            "1|L--|v3 = __readX(X)",
            "2|L--|D = Mem0[v3:byte]",
            "3|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_LNBR()
    {
        Given_HexString("C82A8A");
        AssertCode(     // LNBR	2A8A
            "0|T--|0100(1): 1 instructions",
            "1|T--|goto 0103",
            "2|L--|0101(1): 1 instructions",
            "3|L--|R10 = R10 - 1<16>",
            "4|L--|0102(1): 1 instructions",
            "5|L--|D = SLICE(R10, byte, 0)");
    }

    [Test]
    public void Rca1802Rw_LSIE()
    {
        Given_HexString("CC");
        AssertCode(     // LSIE	F846
            "0|T--|0100(1): 1 instructions",
            "1|T--|if (__interrupts_enabled()) branch 0103");
    }

    [Test]
    public void Rca1802Rw_LSNF()
    {
        Given_HexString("C7A808");
        AssertCode(     // LSNF	A808
            "0|T--|0100(1): 1 instructions",
            "1|T--|if (!__is_DF_set()) branch 0103",
            "2|L--|0101(1): 1 instructions",
            "3|L--|R8 = SEQ(SLICE(R8, byte, 8), D)",
            "4|L--|0102(1): 1 instructions",
            "5|L--|D = Mem0[R8:byte]");
    }

    [Test]
    public void Rca1802Rw_LSNZ()
    {
        Given_HexString("C6BC02");
        AssertCode(     // LSNZ	D402
            "0|T--|0100(1): 1 instructions",
            "1|T--|if (D != 0<8>) branch 0103",
            "2|L--|0101(1): 1 instructions",
            "3|L--|R12 = SEQ(D, SLICE(R12, byte, 0))",
            "4|L--|0102(1): 1 instructions",
            "5|L--|D = Mem0[R2:byte]");
    }

    [Test]
    public void Rca1802Rw_LSZ()
    {
        Given_HexString("CE6467");
        AssertCode(     // LSZ	6467
            "0|T--|0100(1): 1 instructions",
            "1|T--|if (D == 0<8>) branch 0103",
            "2|L--|0101(1): 2 instructions",
            "3|L--|__output(4<8>, __readX(X))",
            "4|L--|X = __irx(X)",
            "5|L--|0102(1): 2 instructions",
            "6|L--|__output(7<8>, __readX(X))",
            "7|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_NBR()
    {
        Given_HexString("3805");
        AssertCode(     // NBR	01F8
            "0|T--|0100(1): 1 instructions",
            "1|T--|goto 0102",
            "2|L--|0101(1): 1 instructions",
            "3|L--|D = Mem0[R5:byte]");
    }

    [Test]
    public void Rca1802Rw_NOP()
    {
        Given_HexString("C4");
        AssertCode(     // NOP
            "0|L--|0100(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void Rca1802Rw_OR()
    {
        Given_HexString("F1");
        AssertCode(     // OR
            "0|L--|0100(1): 4 instructions",
            "1|L--|v5 = __readX(X)",
            "2|L--|v5 = Mem0[v5:byte]",
            "3|L--|D = v5 | D",
            "4|L--|DF = cond(D)");
    }

    [Test]
    public void Rca1802Rw_ORI()
    {
        Given_HexString("F97B");
        AssertCode(     // ORI	7B
            "0|L--|0100(2): 1 instructions",
            "1|L--|D = D | 0x7B<8>");
    }

    [Test]
    public void Rca1802Rw_OUT2()
    {
        Given_HexString("62");
        AssertCode(     // OUT2	02
            "0|L--|0100(1): 2 instructions",
            "1|L--|__output(2<8>, __readX(X))",
            "2|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_OUT4()
    {
        Given_HexString("64");
        AssertCode(     // OUT4	04
            "0|L--|0100(1): 2 instructions",
            "1|L--|__output(4<8>, __readX(X))",
            "2|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_OUT5()
    {
        Given_HexString("65");
        AssertCode(     // OUT5	05
            "0|L--|0100(1): 2 instructions",
            "1|L--|__output(5<8>, __readX(X))",
            "2|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_OUT6()
    {
        Given_HexString("66");
        AssertCode(     // OUT6	06
            "0|L--|0100(1): 2 instructions",
            "1|L--|__output(6<8>, __readX(X))",
            "2|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_OUT7()
    {
        Given_HexString("67");
        AssertCode(     // OUT7	07
            "0|L--|0100(1): 2 instructions",
            "1|L--|__output(7<8>, __readX(X))",
            "2|L--|X = __irx(X)");
    }

    [Test]
    public void Rca1802Rw_PHI()
    {
        Given_HexString("B3");
        AssertCode(     // PHI	3
            "0|L--|0100(1): 1 instructions",
            "1|L--|R3 = SEQ(D, SLICE(R3, byte, 0))");
    }

    [Test]
    public void Rca1802Rw_PLO()
    {
        Given_HexString("A3");
        AssertCode(     // PLO	R3
            "0|L--|0100(1): 1 instructions",
            "1|L--|R3 = SEQ(SLICE(R3, byte, 8), D)");
    }

    [Test]
    public void Rca1802Rw_REQ()
    {
        Given_HexString("7A");
        AssertCode(     // REQ
            "0|L--|0100(1): 1 instructions",
            "1|L--|__reset_q()");
    }

    [Test]
    public void Rca1802Rw_RET()
    {
        Given_HexString("70");
        AssertCode(     // RET
            "0|L--|0100(1): 1 instructions",
            "1|L--|__ret()");
    }

    [Test]
    public void Rca1802Rw_SAV()
    {
        Given_HexString("78");
        AssertCode(     // SAV
            "0|L--|0100(1): 1 instructions",
            "1|L--|Mem0[__readX(X):byte] = T");
    }

    [Test]
    public void Rca1802Rw_SDBI()
    {
        Given_HexString("7D18");
        AssertCode(     // SDBI	18
            "0|L--|0100(2): 2 instructions",
            "1|L--|D = __subc<byte,byte>(0x18<8>, D, DF)",
            "2|L--|DF = cond(D)");
    }

    [Test]
    public void Rca1802Rw_SEP()
    {
        Given_HexString("D3");
        AssertCode(     // SEP	R3
            "0|T--|0100(1): 1 instructions",
            "1|T--|goto R3");
    }

    [Test]
    public void Rca1802Rw_SEQ()
    {
        Given_HexString("7B");
        AssertCode(     // SEQ
            "0|L--|0100(1): 1 instructions",
            "1|L--|__set_q()");
    }

    [Test]
    public void Rca1802Rw_SEX()
    {
        Given_HexString("E7");
        AssertCode(     // SEX	R7
            "0|L--|0100(1): 1 instructions",
            "1|L--|X = 7<4>");
    }

    [Test]
    public void Rca1802Rw_SHL()
    {
        Given_HexString("FE");
        AssertCode(     // SHL
            "0|L--|0100(1): 2 instructions",
            "1|L--|DF = cond(D & 0x80<8>)",
            "2|L--|D = D << 1<8>");
    }

    [Test]
    public void Rca1802Rw_SHLC()
    {
        Given_HexString("7E");
        AssertCode(     // SHLC
            "0|L--|0100(1): 2 instructions",
            "1|L--|DF = cond(D & 0x80<8>)",
            "2|L--|D = __rcl<byte,byte>(D, 1<8>, DF)");
    }

    [Test]
    public void Rca1802Rw_SHR()
    {
        Given_HexString("F6");
        AssertCode(     // SHR
            "0|L--|0100(1): 2 instructions",
            "1|L--|DF = cond(D & 1<8>)",
            "2|L--|D = D >>u 1<8>");
    }

    [Test]
    public void Rca1802Rw_SHRC()
    {
        Given_HexString("76");
        AssertCode(     // SHRC
            "0|L--|0100(1): 2 instructions",
            "1|L--|DF = cond(D & 1<8>)",
            "2|L--|D = __rcr<byte,byte>(D, 1<8>, DF)");
    }

    [Test]
    public void Rca1802Rw_SM()
    {
        Given_HexString("F7");
        AssertCode(     // SM
            "0|L--|0100(1): 4 instructions",
            "1|L--|v5 = __readX(X)",
            "2|L--|v5 = Mem0[v5:byte]",
            "3|L--|D = v5 - D",
            "4|L--|DF = cond(D)");
    }

    [Test]
    public void Rca1802Rw_SMB()
    {
        Given_HexString("77");
        AssertCode(     // SMB
            "0|L--|0100(1): 4 instructions",
            "1|L--|v5 = __readX(X)",
            "2|L--|v5 = Mem0[v5:byte]",
            "3|L--|D = __subc<byte,byte>(v5, D, DF)",
            "4|L--|DF = cond(D)");
    }

    [Test]
    public void Rca1802Rw_SMI()
    {
        Given_HexString("FF42");
        AssertCode(     // SMI
            "0|L--|0100(2): 1 instructions",
            "1|L--|D = D - 0x42<8>");
    }

    [Test]
    public void Rca1802Rw_STR()
    {
        Given_HexString("55");
        AssertCode(     // STR	R5
            "0|L--|0100(1): 1 instructions",
            "1|L--|Mem0[R5:byte] = D");
    }

    [Test]
    public void Rca1802Rw_STXD()
    {
        Given_HexString("73");
        AssertCode(     // STXD
            "0|L--|0100(1): 3 instructions",
            "1|L--|v3 = __readX(X)",
            "2|L--|Mem0[v3:byte] = D",
            "3|L--|X = __drx(X)");
    }

    [Test]
    public void Rca1802Rw_XOR()
    {
        Given_HexString("F3");
        AssertCode(     // XOR
            "0|L--|0100(1): 4 instructions",
            "1|L--|v5 = __readX(X)",
            "2|L--|v5 = Mem0[v5:byte]",
            "3|L--|D = v5 ^ D",
            "4|L--|DF = cond(D)");
    }
}
