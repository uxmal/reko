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
using Reko.Arch.M16C;
using Reko.Core;
using System;

namespace Reko.UnitTests.Arch.M16C;

[TestFixture]
public class M16CRewriterTests : RewriterTestBase
{
    private readonly M16CArchitecture arch;
    private readonly Address addr;

    public M16CRewriterTests()
    {
        this.arch = new M16CArchitecture(CreateServiceContainer(), "m16c", new());
        this.addr = Address.Ptr32(0x00100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;


    [Test]
    public void M16cRw_adc()
    {
        Given_HexString("B005");
        AssertCode(     // adc.b	r0l,a1
            "0|L--|00000100(2): 2 instructions",
            "1|L--|a1 = a1 + r0l + C",
            "2|L--|OSZC = cond(a1)");
    }


    [Test]
    public void M16cRw_adcf()
    {
        Given_HexString("77EBFA");
        AssertCode(     // adcf.w	-6h[fb]
            "0|L--|00000100(3): 3 instructions",
            "1|L--|v5 = Mem0[fb - 6<i16>:word16] + C",
            "2|L--|Mem0[fb - 6<i16>:word16] = v5",
            "3|L--|OSZC = cond(v5)");
    }

    [Test]
    public void M16cRw_add()
    {
        Given_HexString("77413000");
        AssertCode(     // add.w	#30h,r1
            "0|L--|00000100(4): 2 instructions",
            "1|L--|r1 = r1 + 0x30<16>",
            "2|L--|OSZC = cond(r1)");
    }

    [Test]
    public void M16cRw_and()
    {
        Given_HexString("900F75C1");
        AssertCode(     // and.b	r0l,[0C175h]
            "0|L--|00000100(4): 3 instructions",
            "1|L--|v4 = Mem0[0xC175<p16>:byte] & r0l",
            "2|L--|Mem0[0xC175<p16>:byte] = v4",
            "3|L--|SZ = cond(v4)");
    }


    [Test]
    [Ignore("read docs to understand the modes")]
    public void M16cRw_band()
    {
        Given_HexString("7E4F7E55");
        AssertCode(     // band	[557Eh]
            "0|L--|000C0073(4): 1 instructions",
            "1|L--|@@@");
    }

    [Test]
    public void M16cRw_bclr()
    {
        Given_HexString("7E8F3700");
        AssertCode(     // bclr	bit,0037:16
            "0|L--|00000100(4): 1 instructions",
            "1|L--|Mem0[0x0006<p16>:byte] = __set_bit<byte,byte>(Mem0[0x0006<p16>:byte], 7<8>)");
    }

    [Test]
    [Ignore("read docs to understand the modes")]
    public void M16cRw_bnot()
    {
        Given_HexString("5710");
        AssertCode(     // bnot:s	#7h,10h[sb]
            "0|L--|000C01CB(2): 1 instructions",
            "1|L--|@@@");
    }

    [Test]
    public void M16cRw_brk()
    {
        Given_HexString("00");
        AssertCode(     // brk
            "0|H--|00000100(1): 1 instructions",
            "1|L--|__break()");
    }

    [Test]
    public void M16cRw_bset()
    {
        Given_HexString("7E9FE51F");
        AssertCode(     // bset	bit,1FE5:16
            "0|L--|00000100(4): 1 instructions",
            "1|L--|Mem0[0x03FC<p16>:byte] = __set_bit<byte,byte>(Mem0[0x03FC<p16>:byte], 5<8>)");
    }

    [Test]
    public void M16cRw_bmeq()
    {
        Given_HexString("7E2F401F02");
        AssertCode(     // bmeq	bit,1F40:16
            "0|L--|00000100(5): 1 instructions",
            "1|L--|Mem0[0x03E8<p16>:byte] = __write_bit<byte,byte>(Mem0[0x03E8<p16>:byte], 0<8>, Test(EQ,Z))");
    }


    [Test]
    public void M16cRw_bmne()
    {
        Given_HexString("7E2000FA");
        AssertCode(     // bmne	r0
            "0|L--|00000100(4): 1 instructions",
            "1|L--|r0 = __write_bit<word16,byte>(r0, 0<8>, Test(NE,Z))");
    }

    [Test]
    public void M16cRw_btst()
    {
        Given_HexString("7EBF241F");
        AssertCode(     // btst	bit,1F24:16
            "0|L--|00000100(4): 3 instructions",
            "1|L--|v3 = __bit<byte,byte>(Mem0[0x03E4<p16>:byte], 4<8>)",
            "2|L--|C = v3",
            "3|L--|Z = !v3");
    }

    [Test]
    public void M16cRw_cmp()
    {
        Given_HexString("D110");
        AssertCode(     // cmp.w:q	#1h,r0
            "0|L--|00000100(2): 1 instructions",
            "1|L--|OSZC = cond(r0 - 1<16>)");
    }

    [Test]
    public void M16cRw_cmp_fb_disp()
    {
        Given_HexString("778BF81800");
        AssertCode(     // cmp.w	#18h,-8h[fb]
            "0|L--|00000100(5): 1 instructions",
            "1|L--|OSZC = cond(Mem0[fb - 8<i16>:word16] - 0x18<16>)");
    }

    [Test]
    public void M16cRw_dec()
    {
        Given_HexString("FA");
        AssertCode(     // dec.w	a1
            "0|L--|00000100(1): 2 instructions",
            "1|L--|a1 = a1 - 1<i16>",
            "2|L--|SZ = cond(a1)");
    }

    [Test]
    public void M16cRw_divu_b()
    {
        Given_HexString("7CE00A");
        AssertCode(     // divu.b	#0Ah
            "0|L--|00000100(3): 4 instructions",
            "1|L--|v3 = r0",
            "2|L--|r0l = v3 /u 0xA<8>",
            "3|L--|r0h = v3 %u 0xA<8>",
            "4|L--|O = cond(r0l)");
    }

    [Test]
    public void M16cRw_divu_w()
    {
        Given_HexString("7DE00A00");
        AssertCode(     // divu.w	#0Ah
            "0|L--|00000100(4): 4 instructions",
            "1|L--|v3 = r2r0",
            "2|L--|r0 = v3 /u 0xA<16>",
            "3|L--|r2 = v3 %u 0xA<16>",
            "4|L--|O = cond(r0)");
    }

    [Test]
    public void M16cRw_enter()
    {
        Given_HexString("7CF202");
        AssertCode(     // enter	#2h
            "0|L--|00000100(3): 4 instructions",
            "1|L--|usp = usp - 2<16>",
            "2|L--|Mem0[usp:word16] = fb",
            "3|L--|fb = usp",
            "4|L--|usp = usp - 2<i16>");
    }

    [Test]
    public void M16cRw_exitd()
    {
        Given_HexString("7DF2");
        AssertCode(     // exitd
            "0|R--|00000100(2): 4 instructions",
            "1|L--|usp = fb",
            "2|L--|fb = Mem0[usp:word16]",
            "3|L--|usp = usp + 2<i16>",
            "4|R--|return (3,0)");
    }

    [Test]
    public void M16cRw_fset()
    {
        Given_HexString("EB64");
        AssertCode(     // fset	I
            "0|L--|00000100(2): 1 instructions",
            "1|L--|I = 0x40<16>");
    }

    [Test]
    public void M16cRw_jeq()
    {
        Given_HexString("6A04");
        AssertCode(     // jeq	000C017A
            "0|T--|00000100(2): 1 instructions",
            "1|T--|if (Test(EQ,Z)) branch 00000105");
    }

    [Test]
    public void M16cRw_jleu()
    {
        Given_HexString("6D06");
        AssertCode(     // jleu	000C119B
            "0|T--|00000100(2): 1 instructions",
            "1|T--|if (Test(ULE,ZC)) branch 00000107");
    }

    [Test]
    public void M16cRw_jmp()
    {
        Given_HexString("F45103");
        AssertCode(     // jmp.w	000C04C9
            "0|L--|00000100(3): 1 instructions",
            "1|T--|goto 00000452");
    }

    [Test]
    public void M16cRw_jmpi()
    {
        Given_HexString("7D00");
        AssertCode(     // jmpi.a	r2r0
            "0|T--|00000100(2): 1 instructions",
            "1|T--|goto r2r0");
    }

    [Test]
    public void M16cRw_jn()
    {
        Given_HexString("6B0C");
        AssertCode(     // jn	000C17E5
            "0|T--|00000100(2): 1 instructions",
            "1|T--|if (Test(LT,S)) branch 0000010D");
    }


    [Test]
    public void M16cRw_jne()
    {
        Given_HexString("6E07");
        AssertCode(     // jne	000C13DA
            "0|T--|00000100(2): 1 instructions",
            "1|T--|if (Test(NE,Z)) branch 00000108");
    }

    [Test]
    public void M16cRw_jsr()
    {
        Given_HexString("F53F0A");
        AssertCode(     // jsr.w	000C0BBA
            "0|L--|00000100(3): 1 instructions",
            "1|T--|call 00000B40 (3)");
    }

    [Test]
    public void M16cRw_ldc()
    {
        Given_HexString("EB400045");
        AssertCode(     // ldc	#4500h,isp
            "0|L--|00000100(4): 2 instructions",
            "1|L--|isp = 0x4500<16>",
            "2|L--|SZ = cond(0x4500<16>)");
    }

    [Test]
    public void M16cRw_lde()
    {
        Given_HexString("74A0");
        AssertCode(     // lde.b	[r1r0],r0l
            "0|L--|00000100(2): 2 instructions",
            "1|L--|r0l = Mem0[a1a0:byte]",
            "2|L--|SZ = cond(r0l)");
    }

    [Test]
    public void M16cRw_mov_mem_imm()
    {
        Given_HexString("B73D04");
        AssertCode(     // mov.b:s	#0h,[43Dh]
            "0|L--|00000100(3): 2 instructions",
            "1|L--|Mem0[0x043D<p16>:byte] = 0<8>",
            "2|L--|SZ = cond(0<8>)");
    }

    [Test]
    public void M16cRw_mov_reg_reg()
    {
        Given_HexString("B4");
        AssertCode(     // mov.b:s	#0h,r0l
            "0|L--|00000100(1): 2 instructions",
            "1|L--|r0l = 0<8>",
            "2|L--|SZ = cond(0<8>)");
    }

    [Test]
    public void M16cRw_mul()
    {
        Given_HexString("7D500A00");
        AssertCode(     // mul.w	#0Ah,r0
            "0|L--|00000100(4): 1 instructions",
            "1|L--|r0 = r0 *s32 0xA<16>");
    }

    [Test]
    public void M16cRw_not()
    {
        Given_HexString("7572");
        AssertCode(     // not.w	r2
            "0|L--|00000100(2): 2 instructions",
            "1|L--|r2 = ~r2",
            "2|L--|SZ = cond(r2)");
    }

    [Test]
    public void M16cRw_or_imm()
    {
        Given_HexString("763504");
        AssertCode(     // or.b	#4h,a1
            "0|L--|00000100(3): 2 instructions",
            "1|L--|a1 = a1 | 4<16>",
            "2|L--|SZ = cond(a1)");
    }

    [Test]
    public void M16cRw_popm()
    {
        Given_HexString("ED0A");
        AssertCode(     // popm	r3,r1
            "0|L--|00000100(2): 4 instructions",
            "1|L--|r3 = Mem0[usp:word16]",
            "2|L--|usp = usp + 2<i16>",
            "3|L--|r1 = Mem0[usp:word16]",
            "4|L--|usp = usp + 2<i16>");
    }

    [Test]
    public void M16cRw_popm_one()
    {
        Given_HexString("ED80");
        AssertCode(     // popm	fb
            "0|L--|00000100(2): 2 instructions",
            "1|L--|fb = Mem0[usp:word16]",
            "2|L--|usp = usp + 2<i16>");
    }


    [Test]
    public void M16cRw_push()
    {
        Given_HexString("C2");
        AssertCode(     // push.w:s	a0
            "0|L--|00000100(1): 2 instructions",
            "1|L--|usp = usp - 2<16>",
            "2|L--|Mem0[usp:word16] = a0");
    }


    [Test]
    public void M16cRw_pushm()
    {
        Given_HexString("ECFD");
        AssertCode(     // pushm	r0,r2,r3,a0,a1,sb,fb
            "0|L--|00000100(2): 14 instructions",
            "1|L--|usp = usp - 2<i16>",
            "2|L--|Mem0[usp:word16] = fb",
            "3|L--|usp = usp - 2<i16>",
            "4|L--|Mem0[usp:word16] = a1",
            "5|L--|usp = usp - 2<i16>",
            "6|L--|Mem0[usp:word16] = a0",
            "7|L--|usp = usp - 2<i16>",
            "8|L--|Mem0[usp:word16] = r3",
            "9|L--|usp = usp - 2<i16>",
            "10|L--|Mem0[usp:word16] = r2",
            "11|L--|usp = usp - 2<i16>",
            "12|L--|Mem0[usp:word16] = r1",
            "13|L--|usp = usp - 2<i16>",
            "14|L--|Mem0[usp:word16] = r0");
    }

    [Test]
    public void M16cRw_pushm_one()
    {
        Given_HexString("EC01");
        AssertCode(     // pushm	fb
            "0|L--|00000100(2): 2 instructions",
            "1|L--|usp = usp - 2<i16>",
            "2|L--|Mem0[usp:word16] = fb");
    }

    [Test]
    public void M16cRw_reit()
    {
        Given_HexString("FB");
        AssertCode(     // reit
            "0|L--|00000100(1): 2 instructions",
            "1|L--|__return_from_interrupt()",
            "2|R--|return (4,0)");
    }

    [Test]
    public void M16cRw_rolc()
    {
        Given_HexString("77A4E983");
        AssertCode(     // rolc.w	#83E9h,a0
            "0|L--|00000100(2): 3 instructions",
            "1|L--|v4 = C",
            "2|L--|C = (a0 & 1<16>) != 0<16>",
            "3|L--|a0 = __rcl<word16,int16>(a0, 1<i16>, v4)",
            "4|L--|00000102(2): 2 instructions",
            "5|L--|r3 = r3 >>u 8<8>",
            "6|L--|SZC = cond(r3)");
    }

    [Test]
    public void M16cRw_rot()
    {
        Given_HexString("7461");
        AssertCode(     // rot.b	r1h,r0h
            "0|L--|00000100(2): 2 instructions",
            "1|L--|r0h = __rol<byte,byte>(r0h, r1h)",
            "2|L--|SZC = cond(r0h)");
    }

    [Test]
    public void M16cRw_rts()
    {
        Given_HexString("F3");
        AssertCode(     // rts
            "0|R--|00000100(1): 1 instructions",
            "1|R--|return (3,0)");
    }

    [Test]
    public void M16cRw_sbb()
    {
        Given_HexString("B92B07");
        AssertCode(     // sbb.w	r2,7h[fb]
            "0|L--|00000100(3): 3 instructions",
            "1|L--|v6 = Mem0[fb + 7<i16>:word16] - r2 - C",
            "2|L--|Mem0[fb + 7<i16>:word16] = v6",
            "3|L--|OSZC = cond(v6)");
    }

    [Test]
    public void M16cRw_shl()
    {
        Given_HexString("E9A1");
        AssertCode(     // shl:q	#-6h,r1
            "0|L--|00000100(2): 2 instructions",
            "1|L--|r1 = r1 >>u 6<8>",
            "2|L--|SZC = cond(r1)");
    }

    [Test]
    public void M16cRw_sha()
    {
        Given_HexString("F000");
        AssertCode(     // sha.b:q	#0h,r0l
            "0|L--|00000100(2): 2 instructions",
            "1|L--|r0l = r0l << 0<i8>",
            "2|L--|SZC = cond(r0l)");
    }

    [Test]
    public void M16cRw_shl_l()
    {
        Given_HexString("EB82");
        AssertCode(     // shl.l	#2h,r2r0
            "0|L--|00000100(2): 2 instructions",
            "1|L--|r2r0 = r2r0 << 2<16>",
            "2|L--|SZC = cond(r2r0)");
    }

    [Test]
    public void M16cRw_smovf()
    {
        Given_HexString("7CE8");
        AssertCode(     // smovf.b
            "0|L--|00000100(2): 1 instructions",
            "1|L--|memcpy(a1, a0, r3)");
    }

    [Test]
    public void M16cRw_sstr()
    {
        Given_HexString("7CEA");
        AssertCode(     // sstr.b
            "0|L--|00000100(2): 1 instructions",
            "1|L--|__store_string(a1, r0l, r3)");
    }

    [Test]
    public void M16cRw_ste()
    {
        Given_HexString("752BEC");
        AssertCode(     // ste.w	-14h[fb],[a1a0]
            "0|L--|00000100(3): 2 instructions",
            "1|L--|v5 = Mem0[fb - 20<i16>:word16]",
            "2|L--|Mem0[a1a0:word16] = v5");
    }

    [Test]
    public void M16cRw_stnz()
    {
        Given_HexString("D40D");
        AssertCode(     // stnz	#0Dh,r0l
            "0|L--|00000100(2): 2 instructions",
            "1|T--|if (Test(EQ,Z)) branch 00000102",
            "2|L--|r0l = 0xD<8>");
    }

    [Test]
    public void M16cRw_stz()
    {
        Given_HexString("CD0AF5");
        AssertCode(     // stz	#0Ah,0F5h[sb]
            "0|L--|00000100(3): 2 instructions",
            "1|T--|if (Test(NE,Z)) branch 00000103",
            "2|L--|Mem0[sb + 245<i16>:byte] = 0xA<8>");
    }

    [Test]
    public void M16cRw_stzx()
    {
        Given_HexString("DF413C0442");
        AssertCode(     // stzx:s	#41h,#42h,[43Ch]
            "0|L--|00000100(5): 2 instructions",
            "1|L--|v4 = Test(EQ,Z) ? 0x41<8> : 0x42<8>",
            "2|L--|Mem0[0x043C<p16>:byte] = v4");
    }

    [Test]
    public void M16cRw_tst()
    {
        Given_HexString("809673");
        AssertCode(     // tst.b	73h[a1],[a0]
            "0|L--|00000100(3): 1 instructions",
            "1|L--|SZ = cond(Mem0[a0:byte] & Mem0[a1 + 115<i16>:byte])");
    }

    [Test]
    public void M16cRw_und()
    {
        Given_HexString("FF");
        AssertCode(     // und
            "0|H--|00000100(1): 1 instructions",
            "1|L--|__undefined()");
    }

    [Test]
    public void M16cRw_xor()
    {
        Given_HexString("77140100");
        AssertCode(     // xor.w	#1h,a0
            "0|L--|00000100(4): 2 instructions",
            "1|L--|a0 = a0 ^ 1<16>",
            "2|L--|SZ = cond(a0)");
    }
}
