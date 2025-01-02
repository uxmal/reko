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
using Reko.Arch.Mos6502;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mos6502
{
    [TestFixture]
    public class Mos65816RewriterTests : RewriterTestBase
    {
        private readonly Mos65816Architecture arch;
        private readonly Address addr;

        public Mos65816RewriterTests()
        {
            this.arch = new Mos65816Architecture(CreateServiceContainer(), "wdc65816", new());
            this.addr = Address.Ptr32(0x0400);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addr;

        /*
        [Test]
        public void Mos65816Rw_adc_directPage()
        {
            Given_HexString("6919");
            AssertCode(        // adc\t#$19
                "@@@");
        }

        [Test]
        public void Mos65816Rw_adc_stack_relative()
        {
            Given_HexString("6394");
            AssertCode(        // adc\t$94,s
                "@@@");
        }

        [Test]
        public void Mos65816Rw_adc_direct()
        {
            Given_HexString("6DE1F9");
            AssertCode(        // adc\t$F9E1
                "@@@");
        }

        [Test]
        public void Mos65816Rw_adc_indexed_y()
        {
            Given_HexString("795088");
            AssertCode(        // adc\t$8850,y
                "@@@");
        }

        [Test]
        public void Mos65816Rw_adc_indirectlong()
        {
            Given_HexString("676F");
            AssertCode(        // adc\t[$6F]
                "@@@");
        }

        [Test]
        public void Mos65816Rw_and()
        {
            Given_HexString("2336");
            AssertCode(        // and\t$36,s
                "@@@");
        }

        [Test]
        public void Mos65816Rw_and_longindirect()
        {
            Given_HexString("27E3");
            AssertCode(        // and\t[$E3]
                "@@@");
        }

        [Test]
        public void Mos65816Rw_asl()
        {
            Given_HexString("0A");
            AssertCode(        // asl
                "@@@");
        }

        [Test]
        public void Mos65816Rw_bcs()
        {
            Given_HexString("B037");
            AssertCode(        // bcs\t#$239
                "@@@");
        }

        [Test]
        public void Mos65816Rw_bit()
        {
            Given_HexString("8978");
            AssertCode(        // bit\t#$78
                "@@@");
        }

        [Test]
        public void Mos65816Rw_bmi()
        {
            Given_HexString("308D");
            AssertCode(        // bmi\t#$18F
                "@@@");
        }

        [Test]
        public void Mos65816Rw_bne()
        {
            Given_HexString("D0A6");
            AssertCode(        // bne\t#$1A8
                "@@@");
        }

        [Test]
        public void Mos65816Rw_brk()
        {
            Given_HexString("00");
            AssertCode(        // brk
                "@@@");
        }

        [Test]
        public void Mos65816Rw_blr()
        {
            Given_HexString("820740");
            AssertCode(        // brl\t#$420A
                "@@@");
        }

        [Test]
        public void Mos65816Rw_bvc()
        {
            Given_HexString("50CB");
            AssertCode(        // bvc\t#$1CD
                "@@@");
        }

        [Test]
        public void Mos65816Rw_clc()
        {
            Given_HexString("18");
            AssertCode(        // clc
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cld()
        {
            Given_HexString("D8");
            AssertCode(        // cld
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cli()
        {
            Given_HexString("58");
            AssertCode(        // cli
                "@@@");
        }

        [Test]
        public void Mos65816Rw_clv()
        {
            Given_HexString("B8");
            AssertCode(        // clv
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cmp()
        {
            Given_HexString("CDC682");
            AssertCode(        // cmp\t$82C6
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cmp_absolute_long()
        {
            Given_HexString("CF535B53");
            AssertCode(        // cmp\t$005B53
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cmp_absolute_long_x()
        {
            Given_HexString("DF82E2E1");
            AssertCode(        // cmp\t$00E282,x
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cmp_index()
        {
            Given_HexString("C11F");
            AssertCode(        // cmp\t($1F,x)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cmp_stack_index()
        {
            Given_HexString("D35A");
            AssertCode(        // cmp\t($5A,s),y
                "@@@");
        }

        [Test]
        public void Mos65816Rw_cpx()
        {
            Given_HexString("E021");
            AssertCode(        // cpx\t#$21
                "@@@");
        }

        [Test]
        public void Mos65816Rw_dec()
        {
            Given_HexString("DEB8F8");
            AssertCode(        // dec\t$F8B8,x
                "@@@");
        }

        [Test]
        public void Mos65816Rw_dey()
        {
            Given_HexString("88");
            AssertCode(        // dey
                "@@@");
        }

        [Test]
        public void Mos65816Rw_eor()
        {
            Given_HexString("4F7E143F");
            AssertCode(        // eor\t$00147E
                "@@@");
        }

        [Test]
        public void Mos65816Rw_eor_indexindirect()
        {
            Given_HexString("4174");
            AssertCode(        // eor\t($74,x)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_inc()
        {
            Given_HexString("1A");
            AssertCode(        // inc
                "@@@");
        }

        [Test]
        public void Mos65816Rw_inc_dirpg()
        {
            Given_HexString("E614");
            AssertCode(        // inc\t$14
                "@@@");
        }

        [Test]
        public void Mos65816Rw_iny()
        {
            Given_HexString("C8");
            AssertCode(        // iny
                "@@@");
        }

        [Test]
        public void Mos65816Rw_jmp()
        {
            Given_HexString("4CC5D0");
            AssertCode(        // jmp\t$D0C5
                "@@@");
        }

        [Test]
        public void Mos65816Rw_jmp_indirect()
        {
            Given_HexString("6C01BA");
            AssertCode(        // jmp\t($BA01)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_jmp_indirect_x()
        {
            Given_HexString("DC4330");
            AssertCode(        // jmp\t($3043,x)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_jsr_absidxind()
        {
            Given_HexString("FC1BBD");
            AssertCode(        // jsr\t($BD1B,x)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_jsr_indx()
        {
            Given_HexString("FCAFBA");
            AssertCode(        // jsr\t($BAAF,x)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_jsr_long()
        {
            Given_HexString("22DD86C1");
            AssertCode(        // jsr\t$C186DD
                "@@@");
        }

        [Test]
        public void Mos65816Rw_lda_directPageIndexedIndirectX()
        {
            Given_HexString("A1E5");
            AssertCode(        // lda\t($E5,x)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ldx_indexed()
        {
            Given_HexString("BEE240");
            AssertCode(        // ldx\t$40E2,y
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ldy()
        {
            Given_HexString("AC8FE6");
            AssertCode(        // ldy\t$E68F
                "@@@");
        }

        [Test]
        public void Mos65816Rw_lsr()
        {
            Given_HexString("4A");
            AssertCode(        // lsr
                "@@@");
        }

        [Test]
        public void Mos65816Rw_mvn()
        {
            Given_HexString("545D29");
            AssertCode(        // mvn\t#$5D,#$29
                "@@@");
        }

        [Test]
        public void Mos65816Rw_nop()
        {
            Given_HexString("EA");
            AssertCode(        // nop
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ora()
        {
            Given_HexString("1163");
            AssertCode(        // ora\t($63),y
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ora_direct_page()
        {
            Given_HexString("05C5");
            AssertCode(        // ora\t$C5
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ora_direct_page_indirect()
        {
            Given_HexString("124C");
            AssertCode(        // ora\t($4C)
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ora_imm()
        {
            Given_HexString("0977");
            AssertCode(        // ora\t#$77
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ora_long()
        {
            Given_HexString("0F4B1149");
            AssertCode(        // ora\t$49114B
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ora_stack_relative()
        {
            Given_HexString("03E0");
            AssertCode(        // ora\t$E0,s
                "@@@");
        }

        [Test]
        public void Mos65816Rw_phb()
        {
            Given_HexString("8B");
            AssertCode(        // phb
                "@@@");
        }

        [Test]
        public void Mos65816Rw_phk()
        {
            Given_HexString("4B");
            AssertCode(        // phk
                "@@@");
        }

        [Test]
        public void Mos65816Rw_plb()
        {
            Given_HexString("AB");
            AssertCode(        // plb
                "@@@");
        }

        [Test]
        public void Mos65816Rw_pld()
        {
            Given_HexString("2B");
            AssertCode(        // pld
                "@@@");
        }

        [Test]
        public void Mos65816Rw_plp()
        {
            Given_HexString("28");
            AssertCode(        // plp
                "@@@");
        }

        [Test]
        public void Mos65816Rw_plx()
        {
            Given_HexString("FA");
            AssertCode(        // plx
                "@@@");
        }

        [Test]
        public void Mos65816Rw_rep()
        {
            Given_HexString("C2FE");
            AssertCode(        // rep\t#$FE
                "@@@");
        }

        [Test]
        public void Mos65816Rw_rol()
        {
            Given_HexString("2A");
            AssertCode(        // rol
                "@@@");
        }

        [Test]
        public void Mos65816Rw_rol_directPage()
        {
            Given_HexString("2693");
            AssertCode(        // rol\t$93
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ror()
        {
            Given_HexString("6A");
            AssertCode(        // ror
                "@@@");
        }

        [Test]
        public void Mos65816Rw_ror_direct_page()
        {
            Given_HexString("6687");
            AssertCode(        // ror\t$87
                "@@@");
        }

        [Test]
        public void Mos65816Rw_rts()
        {
            Given_HexString("60");
            AssertCode(        // rts
                "@@@");
        }

        [Test]
        public void Mos65816Rw_sbc_abslongx()
        {
            Given_HexString("FF7F72D3");
            AssertCode(        // sbc\t$D3727F
                "@@@");
        }

        [Test]
        public void Mos65816Rw_sbc_s()
        {
            Given_HexString("E327");
            AssertCode(        // sbc\t$27,s
                "@@@");
        }

        [Test]
        public void Mos65816Rw_sei()
        {
            Given_HexString("78");
            AssertCode(        // sei
                "@@@");
        }

        [Test]
        public void Mos65816Rw_sep()
        {
            Given_HexString("E248");
            AssertCode(        // sep\t#$48
                "@@@");
        }

        [Test]
        public void Mos65816Rw_sta()
        {
            Given_HexString("95F2");
            AssertCode(        // sta\t$F2,x
                "@@@");
        }

        [Test]
        public void Mos65816Rw_stx()
        {
            Given_HexString("8E02C1");
            AssertCode(        // stx\t$C102
                "@@@");
        }

        [Test]
        public void Mos65816Rw_stx_dpy()
        {
            Given_HexString("9688");
            AssertCode(        // stx\t$88,y
                "@@@");
        }

        [Test]
        public void Mos65816Rw_sty()
        {
            Given_HexString("843C");
            AssertCode(        // sty\t$3C
                "@@@");
        }

        [Test]
        public void Mos65816Rw_stz()
        {
            Given_HexString("9CA0DE");
            AssertCode(        // stz\t$DEA0
                "@@@");
        }

        [Test]
        public void Mos65816Rw_stz_dpx()
        {
            Given_HexString("746C");
            AssertCode(        // stz\t$6C,x
                "@@@");
        }

        [Test]
        public void Mos65816Rw_tay()
        {
            Given_HexString("A8");
            AssertCode(        // tay
                "@@@");
        }

        [Test]
        public void Mos65816Rw_tcd()
        {
            Given_HexString("5B");
            AssertCode(        // tcd
                "@@@");
        }

        [Test]
        public void Mos65816Rw_tcs()
        {
            Given_HexString("1B");
            AssertCode(        // tcs
                "@@@");
        }

        [Test]
        public void Mos65816Rw_tsc()
        {
            Given_HexString("3B");
            AssertCode(        // tsc
                "@@@");
        }

        [Test]
        public void Mos65816Rw_txy()
        {
            Given_HexString("9B");
            AssertCode(        // txy
                "@@@");
        }

        [Test]
        public void Mos65816Rw_tyx()
        {
            Given_HexString("BB");
            AssertCode(        // tyx
                "@@@");
        }

        [Test]
        public void Mos65816Rw_wai()
        {
            Given_HexString("CB");
            AssertCode(        // wai
                "@@@");
        }

        [Test]
        public void Mos65816Rw_wdm()
        {
            Given_HexString("42");
            AssertCode(        // wdm
                "@@@");
        }

        [Test]
        public void Mos65816Rw_xba()
        {
            Given_HexString("EB");
            AssertCode(        // xba
                "@@@");
        }

        [Test]
        public void Mos65816Rw_xce()
        {
            Given_HexString("FB");
            AssertCode(        // xce
                "@@@");
        }
        */
    }
}
