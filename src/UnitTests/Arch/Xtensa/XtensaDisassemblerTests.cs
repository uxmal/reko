#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.Xtensa;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace Reko.UnitTests.Arch.Xtensa
{
    [TestFixture]
    public class XtensaDisassemblerTests : DisassemblerTestBase<XtensaInstruction>
    {
        private readonly XtensaArchitecture arch;

        public XtensaDisassemblerTests()
        {
            this.arch = new XtensaArchitecture(new ServiceContainer(), "xtensa", new Dictionary<string, object>());
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private void AssertCode(string sExp, uint uInstr)
        {
            var i = DisassembleWord(uInstr);
            Assert.AreEqual(sExp, i.ToString());
        }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Xtdasm_generate()
        {
            var rnd = new Random(4711);
            var buf = new byte[1_000_000];
            rnd.NextBytes(buf);
            var mem = new ByteMemoryArea(LoadAddress, buf);
            var rdr = new LeImageReader(mem, 0);
            var dasm = new XtensaDisassembler(arch, rdr);
            dasm.Take(400).ToArray();
        }

        [Test]
        public void Xtdasm_add()
        {
            AssertCode("add\ta4,a2,a3", 0x804230);
        }

        [Test]
        public void Xtdasm_add_n()
        {
            AssertCode("add.n\ta4,a4,a12", 0x44CA);
        }

        [Test]
        public void Xtdasm_addi()
        {
            AssertCode("addi\ta4,a3,FFFFFFFD", 0xFDC342);
        }

        [Test]
        public void Xtdasm_addi_n()
        {
            AssertCode("addi.n\ta4,a4,-01", 0x440B);
        }

        [Test]
        public void Xtdasm_addmi()
        {
            AssertCode("addmi\ta2,a4,FFFFF000", 0xF0D422);
        }
        [Test]

        public void Xtdasm_addx4()
        {
            AssertCode("addx4\ta2,a2,a3", 0xA02230);
        }

        [Test]
        public void Xtdasm_all4()
        {
            AssertCode("all4\tb7,b4", "709400");
        }

        [Test]
        public void Xtdasm_and()
        {
            AssertCode("and\ta4,a6,a4", 0x104640);
        }

        [Test]
        public void Xtdasm_andbc()
        {
            AssertCode("andbc\tb4,b0,b2", 0x124020);
        }

        [Test]
        public void Xtdasm_any4()
        {
            AssertCode("any4\tb8,b0", "808000");
            AssertCode("invalid", "808300");
        }

        [Test]
        public void Xtdasm_beqz_n()
        {
            AssertCode("beqz.n\ta14,00100013", 0xFE8C);
        }

        [Test]
        public void Xtdasm_bf()
        {
            AssertCode("bf\tb0,00100055", "760051");
        }


        [Test]
        public void Xtdasm_bgeui()
        {
            AssertCode("bgeui\ta4,00000004,00100016", 0x1244f6);
        }

        [Test]
        public void Xtdasm_bltu()
        {
            AssertCode("bltu\ta4,a2,00100012", 0x0E3427);
        }

        [Test]
        public void Xtdasm_bne()
        {
            AssertCode("bne\ta4,a5,000FFFFC", 0xF89457);
        }

        [Test]
        public void Xtdasm_bnei()
        {
            AssertCode("bnei\ta13,-00000001,00100023", 0x1f0d66);
        }

        [Test]
        public void Xtdasm_bnone()
        {
            AssertCode("bnone\ta11,a2,000FFFFA", 0xf60b27);
        }

        [Test]
        public void Xtdasm_break()
        {
            AssertCode("break\t01,00", 0x004100);
        }

        [Test]
        public void Xtdasm_call0()
        {
            AssertCode("call0\t00100B24", 0x00B205);
            AssertCode("call0\t000FFD98", "45 D9 FF");
        }

        [Test]
        public void Xtdasm_callx0()
        {
            AssertCode("callx0\ta0", 0x0000C0);
        }

        [Test]
        public void Xtdasm_ceil_s()
        {
            AssertCode("ceil.s\ta11,f12,4.0", 0xBABC20);
        }

        [Test]
        public void Xtdasm_clamps()
        {
            AssertCode("clamps\ta1,a0,07", "001033");
        }




        [Test]
        public void Xtdasm_dhi()
        {
            AssertCode("dhi\ta12,03C8", "627CF2");
        }

        [Test]
        public void Xtdasm_dhu()
        {
            AssertCode("dhu\ta9,09", "827992");
        }

        [Test]
        public void Xtdasm_dhwb()
        {
            AssertCode("dhwb\ta9,0148", "427952");
        }

        [Test]
        public void Xtdasm_dhwbi()
        {
            AssertCode("dhwbi\ta9,0150", "527954");
        }

        [Test]
        public void Xtdasm_dii()
        {
            AssertCode("dii\ta9,0208", "727982");
        }

        [Test]
        public void Xtdasm_diu()
        {
            AssertCode("diu\ta14,05", "827E53");
        }

        [Test]
        public void Xtdasm_dpfr()
        {
            AssertCode("dpfr\ta2,0008", "027202");
        }

        [Test]
        public void Xtdasm_dpfro()
        {
            AssertCode("dpfro\ta12,030C", "227CC3");
        }

        [Test]
        public void Xtdasm_dpfw()
        {
            AssertCode("dpfw\ta9,0020", "127908");
        }

        [Test]
        public void Xtdasm_dpfwo()
        {
            AssertCode("dpfwo\ta9,00A0", "327928");
        }

        [Test]
        public void Xtdasm_dsync()
        {
            AssertCode("dsync", "302000");
        }


        [Test]
        public void Xtdasm_entry()
        {
            AssertCode("entry\ta14,5328", "365EA6");
        }

        [Test]
        public void Xtdasm_esync()
        {
            AssertCode("esync", "202000");
        }

        [Test]
        public void Xtdasm_excw()
        {
            AssertCode("excw", "802800");
        }

        [Test]
        public void Xtdasm_extui()
        {
            AssertCode("extui\ta2,a2,00,04", 0x342020);
        }

        [Test]
        public void Xtdasm_float_s()
        {
            AssertCode("float.s\tf5,a1,32768.0", "F051CA");
        }

        [Test]
        public void Xtdasm_floor_s()
        {
            AssertCode("floor.s\ta5,f3,1024.0", "A053AA");
        }

        [Test]
        public void Xtdasm_iii()
        {
            AssertCode("iii\ta12,0310", "F27CC4");
        }

        [Test]
        public void Xtdasm_iitlb()
        {
            AssertCode("iitlb\ta0", "404050");
        }

        [Test]
        public void Xtdasm_ill()
        {
            AssertCode("ill", 0x000000);
        }

        [Test]
        public void Xtdasm_l32i_n()
        {
            AssertCode("l32i.n\ta4,a13,1C", 0x7D48);
        }

        [Test]
        public void Xtdasm_l32r()
        {
            AssertCode("l32r\ta7,000FFFFC", 0xFFFF71);
            AssertCode("l32r\ta2,000FFFF8", 0xFFFE21);
        }

        [Test]
        public void Xtdasm_l8ui()
        {
            AssertCode("l8ui\ta2,a1,0003", 0x030122);
        }

        [Test]
        public void Xtdasm_memw()
        {
            AssertCode("memw", 0x0020C0);
        }

        [Test]
        public void Xtdasm_mov_n()
        {
            AssertCode("mov.n\ta3,a1", 0x013D);
        }

        [Test]
        public void Xtdasm_mov_s()
        {
            AssertCode("mov.s\tf3,f1", "0031FA");
        }

        [Test]
        public void Xtdasm_movi()
        {
            AssertCode("movi\ta9,000003A0", 0xA0A392);
        }

        [Test]
        public void Xtdasm_movi_n()
        {
            AssertCode("movi.n\ta3,-20", 0x036C);
            AssertCode("movi.n\ta3,-01", 0xF37C);
            AssertCode("movi.n\ta3,+20", 0x032C);
            AssertCode("movi.n\ta3,+5F", 0xF35C);
        }

        [Test]
        public void Xtdasm_s32i_n()
        {
            AssertCode("s32i.n\ta3,a13,1C", 0x7d39);
        }





        [Test]
        public void Xtdasm_srli()
        {
            AssertCode("srli\ta4,a2,04", 0x414420);
        }







        [Test]
        public void Xtdasm_slli()
        {
            AssertCode("slli\ta4,a12,14", 0x114c40);
        }


        [Test]
        public void Xtdasm_j()
        {
            AssertCode("j\t0010000E", 0x000286);
        }







        [Test]
        public void Xtdasm_l32i()
        {
            AssertCode("l32i\ta5,a1,0374", 0xDD2152);
        }


        [Test]
        public void Xtdasm_movnez()
        {
            AssertCode("movnez\ta2,a5,a3", 0x932530);
        }

        [Test]
        public void Xtdasm_mulsh()
        {
            AssertCode("mulsh\ta7,a0,a2", "2070B2");
        }

        [Test]
        public void Xtdasm_muluh()
        {
            AssertCode("muluh\ta0,a0,a2", "2000A2");
        }



        [Test]
        public void Xtdasm_mull()
        {
            AssertCode("mull\ta2,a4,a2", 0x822420);
        }

        [Test]
        public void Xtdasm_wsr_excsave2()
        {
            AssertCode("wsr\ta0,EXCSAVE2", 0x13D200);
        }

        [Test]
        public void Xtdasm_rfi()
        {
            AssertCode("rfi\t02", 0x003210);
        }

        [Test]
        public void Xtdasm_bbsi()
        {
            AssertCode("bbsi\ta4,00,000FFFEB", 0xE7E407);
        }

        [Test]
        public void Xtdasm_rsr()
        {
            AssertCode("rsr\ta7,EXCCAUSE", 0x03e870);
        }

        [Test]
        public void Xtdasm_ball()
        {
            AssertCode("ball\ta3,a6,00100029", 0x254367);
        }

        [Test]
        public void Xtdasm_s8i()
        {
            AssertCode("s8i\ta4,a2,0027", 0x274242);
            AssertCode("s8i\ta10,a0,0000", 0x0040a2);
        }


        [Test]
        public void Xtdasm_bnez_n()
        {
            AssertCode("bnez.n\ta2,00100007", 0x32CC);
        }

        [Test]
        public void Xtdasm_rfe()
        {
            AssertCode("rfe", 0x003000);
        }

        [Test]
        public void Xtdasm_bnez()
        {
            AssertCode("bnez\ta6,000FFFFA", 0xff6656);
        }

        [Test]
        public void Xtdasm_bany()
        {
            AssertCode("bany\ta12,a3,00100012", 0x0e8c37);
        }

        [Test]
        public void Xtdasm_beqz()
        {
            AssertCode("beqz\ta0,0010000C", 0x008016);
        }


        [Test]
        public void Xtdasm_addx2()
        {
            AssertCode("addx2\ta6,a0,a0", 0x906000);
        }

        [Test]
        public void Xtdasm_bltui()
        {
            AssertCode("bltui\ta4,00000007,000FFFED", 0xe974b6);
        }

        [Test]
        public void Xtdasm_ssa8l()
        {
            AssertCode("ssa8l\ta3", 0x402300);
        }

        [Test]
        public void Xtdasm_ssl()
        {
            AssertCode("ssl\ta2", 0x401200);
        }


        [Test]
        public void Xtdasm_moveqz_s()
        {
            AssertCode("moveqz.s\tf15,f12,a0", 0x8bfc00);
        }

        [Test]
        public void Xtdasm_movf()
        {
            AssertCode("movf\ta3,a2,b9", "9032C3");
        }

        [Test]
        public void Xtdasm_movf_s()
        {
            AssertCode("movf.s\tf11,f2,b9", "90B2CB");
        }

        [Test]
        public void Xtdasm_movgez()
        {
            AssertCode("movgez\ta4,a5,a3", 0xb34530);
        }

        [Test]
        public void Xtdasm_movsp()
        {
            AssertCode("movsp\ta14,a7", "E01700");
        }

        [Test]
        public void Xtdasm_msub_s()
        {
            AssertCode("msub.s\tf15,f14,f4", "40FE5A");
        }




        [Test]
        public void Xtdasm_bnall()
        {
            AssertCode("bnall\ta5,a6,0010001E", 0x1ac567);
        }

        [Test]
        public void Xtdasm_ssi()
        {
            AssertCode("ssi\tf7,a6,0000", 0x004673);
        }

        [Test]
        public void Xtdasm_add_s()
        {
            AssertCode("add.s\tf4,f6,f0", 0x0a4600);
        }



        [Test]
        public void Xtdasm_s32ri()
        {
            AssertCode("s32ri\ta0,a15,0300", 0xc0ff02);
        }


        [Test]
        public void Xtdasm_mul16u()
        {
            AssertCode("mul16u\ta5,a6,a5", 0xc15650);
        }



        [Test]
        public void Xtdasm_jx()
        {
            AssertCode("jx\ta9", 0x0009a0);
        }

        [Test]
        public void Xtdasm_ipf()
        {
            AssertCode("ipf\ta2,035C", "C272D7");
        }

        [Test]
        public void Xtdasm_isync()
        {
            AssertCode("isync", 0x002000);
        }

        [Test]
        public void Xtdasm_bbs()
        {
            AssertCode("bbs\ta3,a2,00100059", 0x55d327);
        }

        [Test]
        public void Xtdasm_bbc()
        {
            AssertCode("bbc\ta2,a4,00100008", 0x045247);
        }

        [Test]
        public void Xtdasm_l32ai()
        {
            AssertCode("l32ai\ta0,a2,010C", "02B243");
        }

        [Test]
        public void Xtdasm_l32e()
        {
            AssertCode("l32e\ta0,a4,-00000030", 0x094400);
        }

        [Test]
        public void Xtdasm_lddec()
        {
            AssertCode("lddec\tmr1,a11", "141B90");
        }

        [Test]
        public void Xtdasm_ldinc()
        {
            AssertCode("ldinc\tmr1,a1", "441180");
        }

        [Test]
        public void Xtdasm_ldpte()
        {
            AssertCode("ldpte", 0xf1f810);
        }

        [Test]
        public void Xtdasm_loop()
        {
            AssertCode("loop\ta0,00100100", "7680FC");
        }

        [Test]
        public void Xtdasm_lsi()
        {
            AssertCode("lsi\tf9,a15,03FC", "93 0F FF");
        }

        [Test]

        public void Xtdasm_lsiu()
        {
            AssertCode("lsiu\tf3,a1,0000", 0x008133);
        }

        [Test]
        public void Xtdasm_min()
        {
            AssertCode("min\ta2,a0,a1", 0x432010);
        }

        [Test]
        public void Xtdasm_mul_aa_ll()
        {
            AssertCode("mul.aa.ll\ta0,a1", "147074");
        }

        [Test]
        public void Xtdasm_mul_ad_ll()
        {
            AssertCode("mul.ad.ll\ta12,mr2", "149C34");
        }

        [Test]
        public void Xtdasm_mula_da_hh()
        {
            AssertCode("mula.da.hh\tmr0,a0", "04056B");
        }

        [Test]
        public void Xtdasm_mula_da_hh_ldinc()
        {
            AssertCode("mula.da.hh.ldinc\tmr1,a1,mr1,a0", "04510B");
        }

        [Test]
        public void Xtdasm_mul_da_hl()
        {
            AssertCode("mul.da.hl\tmr1,a0", "044565");
        }

        [Test]
        public void Xtdasm_mul_da_ll()
        {
            AssertCode("mul.da.ll\tmr0,a14", "E43764");
        }

        [Test]
        public void Xtdasm_mula_aa_hl()
        {
            AssertCode("mula.aa.hl\ta0,a13", "D41079");
        }

        [Test]
        public void Xtdasm_mula_aa_ll()
        {
            AssertCode("mula.aa.ll\ta4,a6", "641478");
        }

        [Test]
        public void Xtdasm_mula_ad_lh()
        {
            AssertCode("mula.ad.lh\ta13,mr3", "44FD3A");
        }

        [Test]
        public void Xtdasm_mula_da_hl()
        {
            AssertCode("mula.da.hl\tmr1,a2", "247569");
        }

        [Test]
        public void Xtdasm_mula_da_lh()
        {
            AssertCode("mula.da.lh\tmr1,a13", "D4FE6A");
        }

        [Test]
        public void Xtdasm_mula_da_hl_lddec()
        {
            AssertCode("mula.da.hl.lddec\tmr0,a5,mr1,a0", "044559");
        }

        [Test]
        public void Xtdasm_mula_da_lh_ldinc()
        {
            AssertCode("mula.da.lh.ldinc\tmr0,a12,mr0,a0", "040C0A");
        }

        [Test]
        public void Xtdasm_mula_da_ll()
        {
            AssertCode("mula.da.ll\tmr1,a0", "044568");
        }

        [Test]
        public void Xtdasm_mula_da_ll_lldec()
        {
            AssertCode("mula.da.ll.lddec\tmr1,a1,mr1,a15", "F45158");
        }

        [Test]
        public void Xtdasm_mula_da_ll_ldinc()
        {
            AssertCode("mula.da.ll.ldinc\tmr0,a12,mr0,a0", "040C08");
        }

        [Test]
        public void Xtdasm_mula_dd_hh_lddec()
        {
            AssertCode("mula.dd.hh.lddec\tmr3,a15,mr1,a6", "64FF1B");
        }

        [Test]
        public void Xtdasm_mula_dd_lh()
        {
            AssertCode("mula.dd.lh\tmr1,mr2", "14462A");
        }

        [Test]
        public void Xtdasm_mula_dd_ll()
        {
            AssertCode("mula.dd.ll\tmr0,mr2", "140028");
        }

        [Test]
        public void Xtdasm_muls_aa_ll()
        {
            AssertCode("muls.aa.ll\ta4,a1", "14047C");
        }

        [Test]
        public void Xtdasm_muls_ad_hh()
        {
            AssertCode("muls.ad.hh\ta10,mr3", "743A3F");
        }

        [Test]
        public void Xtdasm_muls_ad_hl()
        {
            AssertCode("muls.ad.hl\ta6,mr3", "E4F63D");
        }

        [Test]
        public void Xtdasm_muls_dd_hl()
        {
            AssertCode("muls.dd.hl\tmr1,mr3", "64FF2D");
        }

        [Test]
        public void Xtdasm_muls_dd_lh()
        {
            AssertCode("muls.dd.lh\tmr1,mr2", "04492E");
        }

        [Test]
        public void Xtdasm_muls_dd_ll()
        {
            AssertCode("muls.dd.ll\tmr0,mr2", "04012C");
        }

        [Test]
        public void Xtdasm_neg()
        {
            AssertCode("neg\ta2,a2", 0x602020);
        }

        [Test]
        public void Xtdasm_nop()
        {
            AssertCode("nop", "F0 20 00");
        }

        [Test]
        public void Xtdasm_nop_n()
        {
            AssertCode("nop.n", "3D F0 00");
        }

        [Test]
        public void Xtdasm_nsau()
        {
            AssertCode("nsau\ta3,a3", 0x40f330);
        }

        [Test]
        public void Xtdasm_oeq_s()
        {
            AssertCode("oeq.s\tb14,f1,f12", "C0E12B");
        }

        [Test]
        public void Xtdasm_olt_s()
        {
            AssertCode("olt.s\tb7,f4,f12", "C0744B");
        }

        [Test]
        public void Xtdasm_or()
        {
            AssertCode("or\ta1,a1,a1", 0x201110);
        }

        [Test]
        public void Xtdasm_orbc()
        {
            AssertCode("orbc\tb0,b0,b0", 0x320000);
        }

        [Test]
        public void Xtdasm_pitlb()
        {
            AssertCode("pitlb\ta0,a0", "005050");
        }

        [Test]
        public void Xtdasm_quou()
        {
            AssertCode("quou\ta1,a6,a0", 0xc21600);
        }

        [Test]
        public void Xtdasm_rdtlb0()
        {
            AssertCode("rdtlb0\ta3,a1", "30B150");
        }


        [Test]
        public void Xtdasm_rems()
        {
            AssertCode("rems\ta3,a1,a0", 0xF23100);
        }

        [Test]
        public void Xtdasm_ret()
        {
            AssertCode("ret", 0x000080);
        }

        //$TODO: avoid overreaching narrow instrs (we read 3 bytes today when we should be reading 2, and 
        // only if needed the next byte.

        [Test]
        public void Xtdasm_ret_n()
        {
            AssertCode("ret.n", "0DF000");
        }

        [Test]
        public void Xtdasm_rfr()
        {
            AssertCode("rfr\ta3,f1", "4031FA");
        }


        [Test]
        public void Xtdasm_ritlb1()
        {
            AssertCode("ritlb1\ta12,a4", "C07450");
        }

        [Test]
        public void Xtdasm_rotw()
        {
            AssertCode("rotw\t-06", "208940");
        }

        [Test]
        public void Xtdasm_round_s()
        {
            AssertCode("round.s\ta4,f1,1.0", "00418A");
        }

        [Test]
        public void Xtdasm_rsil()
        {
            AssertCode("rsil\ta4,01", 0x006140);
        }

        [Test]
        public void Xtdasm_rsr_epc1()
        {
            AssertCode("rsr\ta6,EPC1", 0x03b160);
        }

        [Test]
        public void Xtdasm_rsync()
        {
            AssertCode("rsync", "102000");
        }

        [Test]
        public void Xtdasm_rur()
        {
            AssertCode("rur\ta2,user132", "4028E3");
        }

        [Test]
        public void Xtdasm_s32c1i()
        {
            AssertCode("s32c1i\ta12,a2,0004", "C2E201");
        }

        [Test]
        public void Xtdasm_s32e()
        {
            AssertCode("s32e\ta2,a0,-00000040", 0x490020);
        }

        [Test]
        public void Xtdasm_s32i()
        {
            AssertCode("s32i\ta13,a1,0394", 0xE561D2);
            AssertCode("s32i\ta0,a1,039C", 0xE76102);
        }

        [Test]
        public void Xtdasm_sext()
        {
            AssertCode("sext\ta8,a7,08", "108723");
        }

        [Test]
        public void Xtdasm_ssa8b()
        {
            AssertCode("ssa8b\ta2", "003240");
        }

        [Test]
        public void Xtdasm_sub()
        {
            AssertCode("sub\ta1,a1,a9", 0xC01190);
        }

        [Test]
        public void Xtdasm_subx2()
        {
            AssertCode("subx2\ta11,a11,a9", 0xD0BB90);
        }

        [Test]
        public void Xtdasm_subx4()
        {
            AssertCode("subx4\ta5,a5,a3", 0xE05530);
        }

        [Test]
        public void Xtdasm_subx8()
        {
            AssertCode("subx8\ta3,a13,a13", 0xF03DD0);
        }






        [Test]
        public void Xtdasm_reserved()
        {
            AssertCode("reserved", 0xFE9200);
            // 00 92 fe
        }

        [Test]
        public void Xtdasm_sll()
        {
            AssertCode("sll\ta7,a6", 0xa17600);
        }

        [Test]
        public void Xtdasm_srai()
        {
            AssertCode("srai\ta3,a10,10", 0x3130a0);
        }

        [Test]
        public void Xtdasm_src()
        {
            AssertCode("src\ta6,a7,a6", 0x816760);
        }

        [Test]
        public void Xtdasm_srl()
        {
            AssertCode("srl\ta2,a8", 0x912080);
        }

        [Test]
        public void Xtdasm_ssai()
        {
            AssertCode("ssai\t08", 0x404800);
        }

        [Test]
        public void Xtdasm_ssr()
        {
            AssertCode("ssr\ta5", 0x400500);
        }

        [Test]
        public void Xtdasm_trunc_s()
        {
            AssertCode("trunc.s\ta15,f14,512.0", "90FE9A");
        }

        [Test]
        public void Xtdasm_ueq_s()
        {
            AssertCode("ueq.s\tb11,f0,f2", 0x3bb020);
        }

        [Test]
        public void Xtdasm_ufloat_s()
        {
            AssertCode("ufloat.s\tf0,a4,8192.0", "D004DA");
        }

        [Test]
        public void Xtdasm_ule_s()
        {
            AssertCode("ule.s\tb0,f1,f0", "00017B");
        }

        [Test]
        public void Xtdasm_ult_s()
        {
            AssertCode("ult.s\tb7,f0,f12", "C0705B");
        }

        [Test]
        public void Xtdasm_umul_aa_hl()
        {
            AssertCode("umul.aa.hl\ta1,a0", "040171");
        }

        [Test]
        public void Xtdasm_umul_aa_lh()
        {
            AssertCode("umul.aa.lh\ta1,a4", "440172");
        }

        [Test]
        public void Xtdasm_umul_aa_ll()
        {
            AssertCode("umul.aa.ll\ta1,a7", "744170");
        }

        [Test]
        public void Xtdasm_un_s()
        {
            AssertCode("un.s\tb0,f0,f4", "40001B");
        }

        [Test]
        public void Xtdasm_utrunc_s()
        {
            AssertCode("utrunc.s\ta5,f1,1.0", "0051EA");
        }

        [Test]
        public void Xtdasm_waiti()
        {
            AssertCode("waiti\t02", "007200");
        }

        [Test]
        public void Xtdasm_wdtlb()
        {
            AssertCode("wdtlb\ta4,a3", "40E350");
        }

        [Test]
        public void Xtdasm_wer()
        {
            AssertCode("wer\ta2,a0", "207040");
        }

        [Test]
        public void Xtdasm_witlb()
        {
            AssertCode("witlb\ta4,a3", "406350");
        }

        [Test]
        public void Xtdasm_wsr()
        {
            AssertCode("wsr\ta2,VECBASE", 0x13E720);
        }

        [Test]
        public void Xtdasm_wsr_intenable()
        {
            AssertCode("wsr\ta0,INTENABLE", "00E413");
        }

        [Test]
        public void Xtdasm_wur()
        {
            AssertCode("wur\ta0,user14", "00E0F3");
        }

        [Test]
        public void Xtdasm_xor()
        {
            AssertCode("xor\ta5,a5,a4", 0x305540);
        }
    }
}

