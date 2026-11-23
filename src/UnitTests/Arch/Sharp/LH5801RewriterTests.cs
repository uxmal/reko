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
using Reko.Arch.Sharp;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Sharp;

public class LH5801RewriterTests : RewriterTestBase
{
    private readonly LH5801Architecture arch;
    private readonly Address addr;

    public LH5801RewriterTests()
    {
        this.arch = new LH5801Architecture(CreateServiceContainer(), "lh5801", []);
        this.addr = Address.Ptr16(0x1000);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    [Test]
    public void LH5801Rw_adc_Mx()
    {
        Given_HexString("03");
        AssertCode(      // adc\t(x)
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = __addc<byte,byte>(a, Mem0[x:byte], C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_adc_xl()
    {
        Given_HexString("02");
        AssertCode(      // adc\txl
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = __addc<byte,byte>(a, xl, C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_adi_a()
    {
        Given_HexString("B36D");
        AssertCode(      // adi\ta,6Dh
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = __addc<byte,byte>(a, 0x6D<8>, C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_adi_ME1()
    {
        Given_HexString("FDEF81EEA9");
        AssertCode(      // adi\t#(81EEh),0A9h
            "0|L--|1000(5): 3 instructions",
            "1|L--|v3 = Mem0[1<16>:0x81EE<p16>:byte] + 0xA9<8>",
            "2|L--|Mem0[1<16>:0x81EE<p16>:byte] = v3",
            "3|L--|CZVH = cond(v3)");
    }

    [Test]
    public void LH5801Rw_adr_x()
    {
        Given_HexString("FDCA");
        AssertCode(      // adr\tx
            "0|L--|1000(2): 3 instructions",
            "1|L--|x = a",
            "2|L--|x = x + CONVERT(a, byte, word16)",
            "3|L--|CZVH = cond(x)");
    }

    [Test]
    public void LH5801Rw_and_Ma()
    {
        Given_HexString("A9C60B");
        AssertCode(      // and\t(0C60Bh)"
            "0|L--|1000(3): 2 instructions",
            "1|L--|a = a & Mem0[0xC60B<p16>:byte]",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_ani_Maddr()
    {
        Given_HexString("E9DAF604");
        AssertCode(      // ani\t(0DAF6h),04h
            "0|L--|1000(4): 3 instructions",
            "1|L--|v3 = Mem0[0xDAF6<p16>:byte] & 4<8>",
            "2|L--|Mem0[0xDAF6<p16>:byte] = v3",
            "3|L--|Z = cond(v3)");
    }

    [Test]
    public void LH5801Rw_aex()
    {
        Given_HexString("F1");
        AssertCode(      // aex
            "0|L--|1000(1): 1 instructions",
            "1|L--|a = __exchange_nybbles(a)");
    }

    [Test]
    public void LH5801Rw_att()
    {
        Given_HexString("FDEC");
        AssertCode(      // att
            "0|L--|1000(2): 1 instructions",
            "1|L--|__write_t_reg(a)");
    }

    [Test]
    public void LH5801Rw_bch()
    {
        Given_HexString("8EF4");
        AssertCode(      // bch\t10F4h
            "0|L--|1000(2): 1 instructions",
            "1|T--|goto 10F4");
    }

    [Test]
    public void LH5801Rw_bch_Back()
    {
        Given_HexString("9EF4");
        AssertCode(      // bch\t0F0Ch
            "0|L--|1000(2): 1 instructions",
            "1|T--|goto 0F0C");
    }

    [Test]
    public void LH5801Rw_bcs()
    {
        Given_HexString("8343");
        AssertCode(      // bcs\t1043h
            "0|L--|1000(2): 1 instructions",
            "1|T--|if (Test(ULT,C)) branch 1043");
    }

    [Test]
    public void LH5801Rw_bii_My()
    {
        Given_HexString("5D92");
        AssertCode(      // bii\t(y),92h
            "0|L--|1000(2): 2 instructions",
            "1|L--|v4 = Mem0[y:byte] & 0x92<8>",
            "2|L--|Z = cond(v4)");
    }

    [Test]
    public void LH5801Rw_bit()
    {
        Given_HexString("2F");
        AssertCode(      // bit\t(u)
            "0|L--|1000(1): 2 instructions",
            "1|L--|v5 = a & Mem0[u:byte]",
            "2|L--|Z = cond(v5)");
    }

    [Test]
    public void LH5801Rw_bit_ME1()
    {
        Given_HexString("FDAF7334");
        AssertCode(      // bit\t#(7334h)"
            "0|L--|1000(4): 2 instructions",
            "1|L--|v4 = a & Mem0[1<16>:0x7334<p16>:byte]",
            "2|L--|Z = cond(v4)");
    }

    [Test]
    public void LH5801Rw_bvr()
    {
        Given_HexString("8DA2");
        AssertCode(      // bvr\t10A2h
            "0|L--|1000(2): 1 instructions",
            "1|T--|if (Test(NO,V)) branch 10A2");
    }

    [Test]
    public void LH5801Rw_bvs()
    {
        Given_HexString("8FED");
        AssertCode(      // bvs\t10EDh
            "0|L--|1000(2): 1 instructions",
            "1|T--|if (Test(OV,V)) branch 10ED");
    }

    [Test]
    public void LH5801Rw_bvs_neg()
    {
        Given_HexString("9FFC");
        AssertCode(      // bvs\t0F04h
            "0|L--|1000(2): 1 instructions",
            "1|T--|if (Test(OV,V)) branch 0F04");
    }

    [Test]
    public void LH5801Rw_bzr()
    {
        Given_HexString("99B6");
        AssertCode(      // bzr\t0F4Ah
            "0|L--|1000(2): 1 instructions",
            "1|T--|if (Test(NE,Z)) branch 0F4A");
    }

    [Test]
    public void LH5801Rw_bzs()
    {
        Given_HexString("9B0C");
        AssertCode(      // bzs\t100Ch
            "0|L--|1000(2): 1 instructions",
            "1|T--|if (Test(EQ,Z)) branch 0FF4");
    }

    [Test]
    public void LH5801Rw_cin()
    {
        Given_HexString("F7");
        AssertCode(      // cin
            "0|L--|1000(1): 2 instructions",
            "1|L--|CZVH = cond(a - Mem0[x:byte])",
            "2|L--|x = x + 1<i16>");
    }

    [Test]
    public void LH5801Rw_cpa_Mx()
    {
        Given_HexString("07");
        AssertCode(      // cpa\t(x)
            "0|L--|1000(1): 1 instructions",
            "1|L--|CZVH = cond(a - Mem0[x:byte])");
    }

    [Test]
    public void LH5801Rw_cpa_xl()
    {
        Given_HexString("06");
        AssertCode(      // cpa\txl
            "0|L--|1000(1): 1 instructions",
            "1|L--|CZVH = cond(a - xl)");
    }

    [Test]
    public void LH5801Rw_cpi()
    {
        Given_HexString("6CB3");
        AssertCode(      // cpi\tuh,0B3h
            "0|L--|1000(2): 1 instructions",
            "1|L--|CZVH = cond(uh - 0xB3<8>)");
    }

    [Test]
    public void LH5801Rw_dca_Mu()
    {
        Given_HexString("AC");
        AssertCode(      // dca\t(u)"
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = __decimal_add(a, Mem0[u:byte], C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_dcs_E1()
    {
        Given_HexString("FD0C");
        AssertCode(      // dcs\t#(x)"
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = __decimal_subtract(a, Mem0[1<16>:x:byte], C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_dec_a()
    {
        Given_HexString("DF");
        AssertCode(      // dec\ta
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = a - 1<i8>",
            "2|L--|CZVH = cond(a)");
    }


    [Test]
    public void LH5801Rw_dec_u()
    {
        Given_HexString("66");
        AssertCode(      // dec\tu
            "0|L--|1000(1): 1 instructions",
            "1|L--|u = u - 1<i16>");
    }

    [Test]
    public void LH5801Rw_dec_ul()
    {
        Given_HexString("62");
        AssertCode(      // dec\tul
            "0|L--|1000(1): 2 instructions",
            "1|L--|ul = ul - 1<i8>",
            "2|L--|CZVH = cond(ul)");
    }

    [Test]
    public void LH5801Rw_drl()
    {
        Given_HexString("D7");
        AssertCode(      // drl
            "0|L--|1000(1): 1 instructions",
            "1|L--|__digit_rotate_left(a, &Mem0[x:byte])");
    }

    [Test]
    public void LH5801Rw_drl_ME1()
    {
        Given_HexString("FDD7");
        AssertCode(      // drl
            "0|L--|1000(2): 1 instructions",
            "1|L--|__digit_rotate_left(a, &Mem0[1<16>:x:byte])");
    }

    [Test]
    public void LH5801Rw_drr()
    {
        Given_HexString("D3");
        AssertCode(      // drr
            "0|L--|1000(1): 1 instructions",
            "1|L--|__digit_rotate_right(a, &Mem0[x:byte])");
    }

    [Test]
    public void LH5801Rw_eor_Maddr()
    {
        Given_HexString("ADBC29");
        AssertCode(      // eor\t(0BC29h)"
            "0|L--|1000(3): 2 instructions",
            "1|L--|a = a ^ Mem0[0xBC29<p16>:byte]",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_eor_Mu()
    {
        Given_HexString("2D");
        AssertCode(      // eor\t(u)"
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = a ^ Mem0[u:byte]",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_eor_My()
    {
        Given_HexString("1D");
        AssertCode(      // eor\t(y)
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = a ^ Mem0[y:byte]",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_eai_a()
    {
        Given_HexString("BDC0");
        AssertCode(      // eai\ta,0C0h
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = a ^ 0xC0<8>",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_eor_ME1()
    {
        Given_HexString("FD1D");
        AssertCode(      // eor\t#(y)"
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = a ^ Mem0[1<16>:y:byte]",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_inc_a()
    {
        Given_HexString("DD");
        AssertCode(      // inc\ta
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = a + 1<i8>",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_inc_u()
    {
        Given_HexString("64");
        AssertCode(      // inc\tu
            "0|L--|1000(1): 1 instructions",
            "1|L--|u = u + 1<i16>");
    }


    [Test]
    public void LH5801Rw_inc_uh()
    {
        Given_HexString("FD60");
        AssertCode(      // inc\tuh
            "0|L--|1000(2): 2 instructions",
            "1|L--|uh = uh + 1<i8>",
            "2|L--|CZVH = cond(uh)");
    }

    [Test]
    public void LH5801Rw_inc_xh()
    {
        Given_HexString("FD40");
        AssertCode(      // inc\txh
            "0|L--|1000(2): 2 instructions",
            "1|L--|xh = xh + 1<i8>",
            "2|L--|CZVH = cond(xh)");
    }

    [Test]
    public void LH5801Rw_inc_y()
    {
        Given_HexString("54");
        AssertCode(      // inc\ty
            "0|L--|1000(1): 1 instructions",
            "1|L--|y = y + 1<i16>");
    }

    [Test]
    public void LH5801Rw_inc_yl()
    {
        Given_HexString("50");
        AssertCode(      // inc\tyl
            "0|L--|1000(1): 2 instructions",
            "1|L--|yl = yl + 1<i8>",
            "2|L--|CZVH = cond(yl)");
    }

    [Test]
    public void LH5801Rw_lda_Mx()
    {
        Given_HexString("05");
        AssertCode(      // lda\t(x)
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = Mem0[x:byte]",
            "2|L--|Z = cond(a == 0<8>)");
    }

    [Test]
    public void LH5801Rw_lda_uh()
    {
        Given_HexString("A4");
        AssertCode(      // lda\tuh
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = uh",
            "2|L--|Z = cond(a == 0<8>)");
    }

    [Test]
    public void LH5801Rw_lda_xl()
    {
        Given_HexString("04");
        AssertCode(      // lda\txl
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = xl",
            "2|L--|Z = cond(a == 0<8>)");
    }

    [Test]
    public void LH5801Rw_lde()
    {
        Given_HexString("57");
        AssertCode(      // lde\ty
            "0|L--|1000(1): 3 instructions",
            "1|L--|a = Mem0[y:byte]",
            "2|L--|Z = cond(a == 0<8>)",
            "3|L--|y = y - 1<i16>");
    }

    [Test]
    public void LH5801Rw_ldi_s()
    {
        Given_HexString("AAFB15");
        AssertCode(      // ldi\ts,0FB15h
            "0|L--|1000(3): 1 instructions",
            "1|L--|s = 0xFB15<16>");
    }

    [Test]
    public void LH5801Rw_ldi_xl()
    {
        Given_HexString("4AC3");
        AssertCode(      // ldi\txl,0C3h
            "0|L--|1000(2): 1 instructions",
            "1|L--|xl = 0xC3<8>");
    }

    [Test]
    public void LH5801Rw_ldx_s()
    {
        Given_HexString("FD18");
        AssertCode(      // ldx\ts
            "0|L--|1000(2): 1 instructions",
            "1|L--|x = s");
    }


    [Test]
    public void LH5801Rw_lin()
    {
        Given_HexString("65");
        AssertCode(      // lin\tu
            "0|L--|1000(1): 3 instructions",
            "1|L--|a = Mem0[u:byte]",
            "2|L--|Z = cond(a == 0<8>)",
            "3|L--|u = u + 1<i16>");
    }

    [Test]
    public void LH5801Rw_lop()
    {
        Given_HexString("98F5");
        AssertCode(      // lop\tul,0F0Bh
            "0|L--|1000(2): 2 instructions",
            "1|L--|ul = ul - 1<8>",
            "2|T--|if (ul >=u 0<8>) branch 0F0B");
    }

    [Test]
    public void LH5801Rw_nop()
    {
        Given_HexString("38");
        AssertCode(      // nop
            "0|L--|1000(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void LH5801Rw_ora_Mx()
    {
        Given_HexString("0B");
        AssertCode(      // ora\t(x)
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = a | Mem0[x:byte]",
            "2|L--|Z = cond(a)");
    }

    [Test]
    public void LH5801Rw_off()
    {
        Given_HexString("FD4C");
        AssertCode(      // off
            "0|L--|1000(2): 1 instructions",
            "1|L--|__off()");
    }

    [Test]
    public void LH5801Rw_ori_Madd()
    {
        Given_HexString("EB5E0A2D");
        AssertCode(      // ori\t(5E0Ah),2Dh
            "0|L--|1000(4): 3 instructions",
            "1|L--|v3 = Mem0[0x5E0A<p16>:byte] | 0x2D<8>",
            "2|L--|Mem0[0x5E0A<p16>:byte] = v3",
            "3|L--|Z = cond(v3)");
    }

    [Test]
    public void LH5801Rw_ori_Mu()
    {
        Given_HexString("6BBA");
        AssertCode(      // ori\t(u),0BAh
            "0|L--|1000(2): 3 instructions",
            "1|L--|v4 = Mem0[u:byte] | 0xBA<8>",
            "2|L--|Mem0[u:byte] = v4",
            "3|L--|Z = cond(v4)");
    }

    [Test]
    public void LH5801Rw_pop_y()
    {
        Given_HexString("FD1A");
        AssertCode(      // pop\ty
            "0|L--|1000(2): 2 instructions",
            "1|L--|s = s + 2<i16>",
            "2|L--|y = Mem0[s:word16]");
    }


    [Test]
    public void LH5801Rw_psh()
    {
        Given_HexString("FDC8");
        AssertCode(      // psh\ta
            "0|L--|1000(2): 2 instructions",
            "1|L--|Mem0[s:byte] = a",
            "2|L--|s = s - 1<i16>");
    }

    [Test]
    public void LH5801Rw_rdp()
    {
        Given_HexString("FDC0");
        AssertCode(      // rdp
            "0|L--|1000(2): 1 instructions",
            "1|L--|__reset_disp()");
    }

    [Test]
    public void LH5801Rw_rec()
    {
        Given_HexString("F9");
        AssertCode(      // rec
            "0|L--|1000(1): 1 instructions",
            "1|L--|C = 0<8>");
    }

    [Test]
    public void LH5801Rw_rol()
    {
        Given_HexString("DB");
        AssertCode(      // rol
            "0|L--|1000(1): 3 instructions",
            "1|L--|v5 = (a & 0x80<8>) != 0<8>",
            "2|L--|a = __rcl<byte,byte>(a, 1<8>, C)",
            "3|L--|C = cond(v5)");
    }

    [Test]
    public void LH5801Rw_ror()
    {
        Given_HexString("D1");
        AssertCode(      // ror
            "0|L--|1000(1): 3 instructions",
            "1|L--|v5 = (a & 1<8>) != 0<8>",
            "2|L--|a = __rcr<byte,byte>(a, 1<8>, C)",
            "3|L--|C = cond(v5)");
    }

    [Test]
    public void LH5801Rw_rpu()
    {
        Given_HexString("E3");
        AssertCode(      // rpu
            "0|L--|1000(1): 1 instructions",
            "1|L--|__reset_pu()");
    }

    [Test]
    public void LH5801Rw_rpv()
    {
        Given_HexString("B8");
        AssertCode(      // rpv
            "0|L--|1000(1): 1 instructions",
            "1|L--|__reset_pv()");
    }

    [Test]
    public void LH5801Rw_rti()
    {
        Given_HexString("8A");
        AssertCode(      // rti
            "0|R--|1000(1): 2 instructions",
            "1|L--|__return_from_interrupt()",
            "2|R--|return (2,0)");
    }

    [Test]
    public void LH5801Rw_rtn()
    {
        Given_HexString("9A");
        AssertCode(      // rtn
            "0|R--|1000(1): 1 instructions",
            "1|R--|return (2,0)");
    }

    [Test]
    public void LH5801Rw_sbc_uh()
    {
        Given_HexString("A0");
        AssertCode(      // sbc\tuh
            "0|L--|1000(1): 2 instructions",
            "1|L--|a = __subc<byte,byte>(a, uh, C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_sbi_a()
    {
        Given_HexString("B1FC");
        AssertCode(      // sbi\ta,0FCh
            "0|L--|1000(2): 2 instructions",
            "1|L--|a = __subc<byte,byte>(a, 0xFC<8>, C)",
            "2|L--|CZVH = cond(a)");
    }

    [Test]
    public void LH5801Rw_sde()
    {
        Given_HexString("53");
        AssertCode(      // sde\ty
            "0|L--|1000(1): 2 instructions",
            "1|L--|Mem0[y:byte] = a",
            "2|L--|y = y - 1<i16>");
    }

    [Test]
    public void LH5801Rw_sec()
    {
        Given_HexString("FB");
        AssertCode(      // sec
            "0|L--|1000(1): 1 instructions",
            "1|L--|C = 1<8>");
    }

    [Test]
    public void LH5801Rw_shl()
    {
        Given_HexString("D9");
        AssertCode(      // shl
            "0|L--|1000(1): 3 instructions",
            "1|L--|v5 = (a & 0x80<8>) != 0<8>",
            "2|L--|a = a << 1<8>",
            "3|L--|C = cond(v5)");
    }

    [Test]
    public void LH5801Rw_shr()
    {
        Given_HexString("D5");
        AssertCode(      // shr
            "0|L--|1000(1): 3 instructions",
            "1|L--|v5 = (a & 1<8>) != 0<8>",
            "2|L--|a = a >>u 1<8>",
            "3|L--|C = cond(v5)");
    }



    [Test]
    public void LH5801Rw_sin()
    {
        Given_HexString("51");
        AssertCode(      // sin\ty
            "0|L--|1000(1): 2 instructions",
            "1|L--|Mem0[y:byte] = a",
            "2|L--|y = y + 1<i16>");
    }

    [Test]
    public void LH5801Rw_sjp()
    {
        Given_HexString("BEE94B");
        AssertCode(      // sjp\t0E94Bh
            "0|T--|1000(3): 1 instructions",
            "1|T--|call E94B (2)");
    }

    [Test]
    public void LH5801Rw_spu()
    {
        Given_HexString("E1");
        AssertCode(      // spu
            "0|L--|1000(1): 1 instructions",
            "1|L--|__set_pu()");
    }

    [Test]
    public void LH5801Rw_spv()
    {
        Given_HexString("A8");
        AssertCode(      // spv
            "0|L--|1000(1): 1 instructions",
            "1|L--|__set_pv()");
    }

    [Test]
    public void LH5801Rw_sta_Maddr()
    {
        Given_HexString("AE196B");
        AssertCode(      // sta\t(196Bh)
            "0|L--|1000(3): 1 instructions",
            "1|L--|Mem0[0x196B<p16>:byte] = a");
    }

    [Test]
    public void LH5801Rw_sta_Mx()
    {
        Given_HexString("0E");
        AssertCode(      // sta\t(x)
            "0|L--|1000(1): 1 instructions",
            "1|L--|Mem0[x:byte] = a");
    }

    [Test]
    public void LH5801Rw_sta_xh()
    {
        Given_HexString("08");
        AssertCode(      // sta\txh
            "0|L--|1000(1): 1 instructions",
            "1|L--|xh = a");
    }

    [Test]
    public void LH5801Rw_sta_xl()
    {
        Given_HexString("0A");
        AssertCode(      // sta\txl
            "0|L--|1000(1): 1 instructions",
            "1|L--|xl = a");
    }

    [Test]
    public void LH5801Rw_stx_s()
    {
        Given_HexString("FD4E");
        AssertCode(      // stx\ts
            "0|L--|1000(2): 1 instructions",
            "1|L--|Mem0[s:word16] = x");
    }

    [Test]
    public void LH5801Rw_tin()
    {
        Given_HexString("F5");
        AssertCode(      // tin
            "0|L--|1000(1): 4 instructions",
            "1|L--|v5 = Mem0[x:byte]",
            "2|L--|Mem0[y:byte] = v5",
            "3|L--|x = x + 1<16>",
            "4|L--|y = y + 1<16>");
    }

    [Test]
    public void LH5801Rw_vej()
    {
        Given_HexString("D0");
        AssertCode(      // vej
            "0|L--|1000(1): 1 instructions",
            "1|T--|call FFD0 (2)");
    }

    [Test]
    public void LH5801Rw_vhs()
    {
        Given_HexString("C7E4");
        AssertCode(      // vhs
            "0|L--|1000(2): 2 instructions",
            "1|T--|if (Test(ULT,H)) branch 1002",
            "2|T--|call FFE4 (2)");
    }

    [Test]
    public void LH5801Rw_vmj()
    {
        Given_HexString("CDC0");
        AssertCode(      // vmj
            "0|L--|1000(2): 1 instructions",
            "1|T--|call FFC0 (2)");
    }

    [Test]
    public void LH5801Rw_vvs()
    {
        Given_HexString("CFE4");
        AssertCode(      // vvs
            "0|L--|1000(2): 2 instructions",
            "1|T--|if (Test(NO,V)) branch 1002",
            "2|T--|call FFE4 (2)");
    }

    [Test]
    public void LH5801Rw_vzs()
    {
        Given_HexString("CBF2");
        AssertCode(      // vzs
            "0|L--|1000(2): 2 instructions",
            "1|T--|if (Test(NE,Z)) branch 1002",
            "2|T--|call FFF2 (2)");
    }
}
