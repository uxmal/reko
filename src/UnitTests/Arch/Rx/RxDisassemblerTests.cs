#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Arch.Renesas;
using Reko.Arch.Renesas.Rx;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Rx
{
    [TestFixture]
    public class RxDisassemblerTests : DisassemblerTestBase<RxInstruction>
    {
        public RxDisassemblerTests()
        {
            var options = new Dictionary<string, object>()
            {
                { ProcessorOption.Endianness, "big" }
            };
            this.Architecture = new RxArchitecture(CreateServiceContainer(), "rxv2", options);
            this.LoadAddress = Address.Ptr32(0x0010_0000);
        }

        public override IProcessorArchitecture Architecture { get; }
        public override Address LoadAddress { get; }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void RxDis_abs_dest()
        {
            AssertCode("abs\tr4", "7E24");
        }

        [Test]
        public void RxDis_abs_src_dest()
        {
            AssertCode("abs\tr3,r4", "FC0F34");
        }

        [Test]
        public void RxDis_adc_simm8_dest()
        {
            AssertCode("adc\t#0FFFFFFFFh,r4", "FD7424 FF");
        }

        [Test]
        public void RxDis_adc_simm24_dest()
        {
            AssertCode("adc\t#0FFA6789Ah,r4", "FD7C24 A6789A");
        }

        [Test]
        public void RxDis_adc_simm32_dest()
        {
            AssertCode("adc\t#56789ABCh,r4", "FD7024 56789ABC");
        }

        [Test]
        public void RxDis_adc_src_dest()
        {
            AssertCode("adc\tr3,sp", "FC0B30");
        }

        [Test]
        public void RxDis_adc_src_dest_mem()
        {
            AssertCode("adc.l\t508[r3],sp",    "06A10230 7F");
            AssertCode("adc.l\t-30876[r3],sp", "06A20230 8764");
        }

        [Test]
        public void RxDis_add_imm()
        {
            AssertCode("add\t#0Fh,r3", "62F3");
        }

        [Test]
        public void RxDis_add_ub_src()
        {
            AssertCode("Invalid", "4823");
            AssertCode("add.ub\t-1[r2],r3", "4923FF");
            AssertCode("add.ub\t-1[r2],r3", "4A23FFFF");
            AssertCode("Invalid", "4B23");
        }

        [Test]
        public void RxDis_add_notub_src()
        {
            AssertCode("add.b\t[r2],r3",   "060823");
            AssertCode("add.w\t-2[r2],r3", "064923FF");
            AssertCode("add.l\t-4[r2],r3", "068923FF");
        }

        [Test]
        public void RxDis_add_simm()
        {
            AssertCode("add\t#12345678h,r2,r3",   "702312345678");
            AssertCode("add\t#0FFFFFF80h,r2,r3",  "712380");
            AssertCode("add\t#0FFFF8000h,r2,r3",  "72238000");
            AssertCode("add\t#0FF800000h,r2,r3", "7323800000");
        }

        [Test]
        public void RxDis_add3()
        {
            AssertCode("add\tr2,r3,r4", "FF2234");
        }

        [Test]
        public void RxDis_and_imm3_reg()
        {
            AssertCode("and\t#3h,r4", "6434");
        }

        [Test]
        public void RxDis_and_simm_reg()
        {
            AssertCode("and\t#0FFFFFF81h,r4", "752481");
            AssertCode("and\t#0FFFF8001h,r4", "76248001");
            AssertCode("and\t#0FF800001h,r4", "7724800001");
            AssertCode("and\t#80000001h,r4",  "742480000001");
        }

        [Test]
        public void RxDis_and_ub()
        {
            AssertCode("and.ub\t[r3],r4", "5034");
            AssertCode("and.ub\t-1[r3],r4", "5134FF");
            AssertCode("and.ub\t-32768[r3],r4", "52348000");
        }


        [Test]
        public void RxDis_and_not_ub()
        {
            AssertCode("and.b\t[r3],r4", "061034");
            AssertCode("and.w\t-256[r3],r4", "06513480");
            AssertCode("and.l\t-32768[r3],r4", "0692348000");
            AssertCode("and.uw\tr3,r4", "06D334");
        }

        [Test]
        public void RxDis_and_3_reg()
        {
            AssertCode("and\tr2,r3,r1", "FF4231");
        }

        [Test]
        public void RxDis_bclr_imm3()
        {
            AssertCode("bclr.ub\t#7h,[r3]",       "F03F");
            AssertCode("bclr.ub\t#7h,-128[r3]",   "F13F80");
            AssertCode("bclr.ub\t#7h,-32768[r3]", "F23F8000");
        }

        [Test]
        public void RxDis_bclr_reg()
        {
            AssertCode("bclr.w\tr12,[r3]", "FC643C");
            AssertCode("bclr.w\tr12,-256[r3]", "FC653C 80");
            AssertCode("bclr.w\tr12,-32768[r3]", "FC663C 8000");
            AssertCode("bclr\tr12,r3", "FC673C 8000");
        }

        [Test]
        public void RxDis_bclr_imm5()
        {
            AssertCode("bclr\t#19h,r3", "7B93");
        }

        [Test]
        public void RxDis_beq_disp3()
        {
            AssertCode("beq.s\t00100007", "17");
            AssertCode("bne.s\t00100007", "1F");
        }

        [Test]
        public void RxDis_beq_disp8()
        {
            AssertCode("beq.b\t000FFFFF", "20FF");
        }

        [Test]
        public void RxDis_beq_disp16()
        {
            AssertCode("beq.w\t000FFFFF", "3AFFFF");
            AssertCode("bne.w\t000FFFFF", "3BFFFF");
        }

        [Test]
        public void RxDis_bmcnd_3()
        {
            AssertCode("bmne.b\t#7h,-1[r3]", "FCFD31 FF");
            AssertCode("bmne.b\t#7h,-32768[r3]", "FCFE31 8000");
        }

        [Test]
        public void RxDis_bnot_imm3()
        {
            AssertCode("bnot.b\t#4h,[r4]", "FCF04F");
            AssertCode("bnot.b\t#7h,-128[r4]", "FCFD4F80");
        }

        [Test]
        public void RxDis_bnot_reg()
        {
            AssertCode("bnot.b\t#4h,[r4]", "FCF04F");
            AssertCode("bnot.b\t#7h,-128[r4]", "FCFD4F80");
        }

        [Test]
        public void RxDis_bnot_imm5_reg()
        {
            AssertCode("bnot\t#1Fh,r3", "FDFFF3");
        }

        [Test]
        public void RxDis_bra_s()
        {
            AssertCode("bra.s\t00100008", "08");
        }

        [Test]
        public void RxDis_bra_b()
        {
            AssertCode("bra.b\t00100008", "2E08");
        }

        [Test]
        public void RxDis_bra_l()
        {
            AssertCode("bra.l\tr4", "7F44");
        }

        [Test]
        public void RxDis_bsr_w()
        {
            AssertCode("bsr.w\t000F8000", "398000");
        }

        [Test]
        public void RxDis_bsr_l()
        {
            AssertCode("bsr.l\tr5", "7F55");
        }

        [Test]
        public void RxDis_btst_imm3()
        {
            AssertCode("btst\t#7h,r7", "F487");
        }

        [Test]
        public void RxDis_clrpsw()
        {
            AssertCode("clrpsw\tU", "7FB9");
        }

        [Test]
        public void RxDis_cmp_imm4_reg()
        {
            AssertCode("cmp\t#3h,r4", "6134");
        }

        [Test]
        public void RxDis_cmp_imm8_reg()
        {
            AssertCode("cmp\t#0FFh,r3", "7553 FF");
        }

        [Test]
        public void RxDis_div_simm()
        {
            AssertCode("div\t#123456h,r4", "FD7C84 123456");
        }

        [Test]
        public void RxDis_divu_uimm()
        {
            AssertCode("divu\t#123456h,r4", "FD7C94 123456");
        }

        [Test]
        public void RxDis_emaca()
        {
            AssertCode("emaca\tr12,r2,a0", "FD07C2");
            AssertCode("emaca\tr12,r2,a1", "FD0FC2");
        }

        [Test]
        public void RxDis_emsba()
        {
            AssertCode("emsba\tr12,r2,a0", "FD47C23");
            AssertCode("emsba\tr12,r2,a1", "FD4FC23");
        }

        [Test]
        public void RxDis_emul()
        {
            AssertCode("emul\t#42h,r3", "FD7463 42");
        }

        [Test]
        public void RxDis_emula()
        {
            AssertCode("emula\tr6,r3,a0", "FD0363 42");
        }

        [Test]
        public void RxDis_emulu()
        {
            AssertCode("emulu\t#123456h,r3", "FD7C73 123456");
            AssertCode("emulu.l\t4660[r3],r4", "FC1E34 1234");
            AssertCode("emulu\tr2,r3", "06630723");
        }

        [Test]
        public void RxDis_fadd_imm()
        {
            AssertCode("fadd\t#1.5,r4", "FD7224 3FC00000");
        }

        [Test]
        public void RxDis_fadd_mem()
        {
            AssertCode("fadd.l\t18[r8],r9", "FC8989 12");
        }

        [Test]
        public void RxDis_fadd_reg()
        {
            AssertCode("fadd\tr2,r3,r1", "FFA231");
        }

        [Test]
        public void RxDis_fcmp_imm()
        {
            AssertCode("fcmp\t#-1,r15", "FD721F BF800000");
        }

        [Test]
        public void RxDis_fcmp_regs()
        {
            AssertCode("fcmp\tr2,r3", "FC8723");
        }

        [Test]
        public void RxDis_fdiv_imm()
        {
            AssertCode("fdiv\t#4,r3", "FD7243 40800000");
        }

        [Test]
        public void RxDis_fmul()
        {
            AssertCode("fmul\t#16,r3", "FD7233 41800000");
            AssertCode("fmul.l\t4660[r10],r11", "FC8EAB 1234");
            AssertCode("fmul\tr2,r3,r1", "FFB231");
        }

        [Test]
        public void RxDis_fsqrt()
        {
            AssertCode("fsqrt.l\t16962[r2],r3", "FCA223 4242");
        }

        [Test]
        public void RxDis_fsub()
        {
            AssertCode("fsub\t#-0.25,r3",       "FD7203 BE800000");
            AssertCode("fsub.l\t-16768[r4],r3", "FC8243 BE80");
            AssertCode("fsub\tr2,r3,r1", "FF8231");
        }

        [Test]
        public void RxDis_ftoi()
        {
            AssertCode("ftoi.l\t291[r3],r4", "FC9634 0123");
        }

        [Test]
        public void RxDis_ftou()
        {
            AssertCode("ftou.l\t291[r3],r4", "FCA634 0123");
        }

        [Test]
        public void RxDis_int()
        {
            AssertCode("int\t#42h", "7560 42");
        }

        [Test]
        public void RxDis_itof()
        {
            AssertCode("itof.l\t66[r2],r3", "FC4523 42");
            AssertCode("itof.w\t-128[r2],r3", "06621123 8000");
        }

        [Test]
        public void RxDis_jmp()
        {
            AssertCode("jmp\tr3", "7F03");
        }

        [Test]
        public void RxDis_jsr()
        {
            AssertCode("jsr\tr3", "7F13");
        }

        [Test]
        public void RxDis_machi()
        {
            AssertCode("machi\tr2,r3,a1", "FD0C23");
        }

        [Test]
        public void RxDis_maclh()
        {
            AssertCode("maclh\tr2,r3,a1", "FD0E23");
        }

        [Test]
        public void RxDis_maclo()
        {
            AssertCode("maclo\tr2,r3,a1", "FD0D23");
        }

        [Test]
        public void RxDis_max()
        {
            AssertCode("max\t#1234h,r4",      "FD7844 1234");
            AssertCode("max.uw\t4660[r2],r3", "FC1223 1234");
        }

        [Test]
        public void RxDis_min()
        {
            AssertCode("min\t#1234h,r4",      "FD7854 1234");
            AssertCode("min.uw\t4660[r2],r3", "FC1623 1234");
            AssertCode("min.w\t18[r2],r3",    "06620523 1234");
        }

        [Test]
        public void RxDis_mov()
        {
            AssertCode("mov.b\tr3,31[r2]", "87AB");
            AssertCode("mov.w\t31[r2],r3", "9FAB");
            AssertCode("mov\t#0Fh,r3", "66F3");
            AssertCode("mov.w\t#42h,31[r2]", "3DAF 42");
            AssertCode("mov\t#42h,r3", "7543 42");
            AssertCode("mov\t#4242h,r3", "FB3A 4242");
            AssertCode("mov.l\tr2,r3", "EF23");
            AssertCode("mov.l\t#5678h,4660[r3]", "FA3A 1234 5678");
            AssertCode("mov.l\t4660[r2],r3", "EE23 1234");
            AssertCode("mov.w\t[r5,r2],r3", "FE5523");
            AssertCode("mov.l\tr3,4660[r2]", "EB23 1234");
            AssertCode("mov.w\tr3,[r5,r2]", "FE1523");
            AssertCode("mov.l\t4660[r2],22136[r3]", "EA23 1234 5678");
            AssertCode("mov.w\t[r2+],r3", "FD2123");
            AssertCode("mov.w\t[-r2],r3", "FD2523");
            AssertCode("mov.w\tr3,[r2+]", "FD2923");
            AssertCode("mov.w\tr3,[-r2]", "FD2D23");
        }

        [Test]
        public void RxDis_movco()
        {
            AssertCode("movco\tr3,r2", "FD2723");
        }

        [Test]
        public void RxDis_movli()
        {
            AssertCode("movli\tr3,r2", "FD2F23");
        }

        [Test]
        public void RxDis_movu()
        {
            AssertCode("movu.b\t3[r3],r7", "BFAB");
            AssertCode("movu.w\t4660[r3],r4", "5E34 1234");
            AssertCode("movu.w\t[r3,r4],r2", "FED342");
            AssertCode("movu.w\t[-r3],r4", "FD3D34");
        }

        [Test]
        public void RxDis_msbhi()
        {
            AssertCode("msbhi\tr2,r3,a1", "FD4C23");
        }

        [Test]
        public void RxDis_msblh()
        {
            AssertCode("msblh\tr2,r3,a1", "FD4E23");
        }

        [Test]
        public void RxDis_msblo()
        {
            AssertCode("msblo\tr2,r3,a1", "FD4D23");
        }

        [Test]
        public void RxDis_mul()
        {
            AssertCode("mul\t#0Fh,r3", "63F3");  
            AssertCode("mul\t#123456h,r3", "7713 123456");
            AssertCode("mul.ub\t4660[r2],r3", "4E23 1234");
            AssertCode("mul.b\t4660[r2],r3", "06CE23 1234");
            AssertCode("mul\tr2,r3,r1", "FF3231");
        }

        [Test]
        public void RxDis_mulhi()
        {
            AssertCode("mulhi\tr2,r3,a1", "FD0823");
        }

        [Test]
        public void RxDis_mullh()
        {
            AssertCode("mullh\tr2,r3,a1", "FD0A23");
        }

        [Test]
        public void RxDis_mullo()
        {
            AssertCode("mullo\tr2,r3,a1", "FD0923");
        }

        [Test]
        public void RxDis_mvfacgu()
        {
            AssertCode("mvfacgu\t#1h,a1,r3", "FD1FF3");
        }

        [Test]
        public void RxDis_mvfachi()
        {
            AssertCode("mvfachi\t#1h,a1,r3", "FD1FC3");
        }

        [Test]
        public void RxDis_mvfaclo()
        {
            AssertCode("mvfaclo\t#1h,a1,r3", "FD1FD3");
        }

        [Test]
        public void RxDis_mvfacmi()
        {
            AssertCode("mvfacmi\t#1h,a1,r3", "FD1FE3");
        }

        [Test]
        public void RxDis_mvfc()
        {
            AssertCode("mvfc\tEXTB,r3", "FD6AD3");
        }

        [Test]
        public void RxDis_mvtacgu()
        {
            AssertCode("mvtacgu\tr3,a1", "FD17B3");
        }

        [Test]
        public void RxDis_mvtachi()
        {
            AssertCode("mvtachi\tr3,a1", "FD1783");
        }

        [Test]
        public void RxDis_mvtaclo()
        {
            AssertCode("mvtaclo\tr3,a1", "FD1793");
        }

        [Test]
        public void RxDis_mvtc()
        {
            AssertCode("mvtc\t#12345678h,FPSW", "FD7303 12345678");
            AssertCode("mvtc\tr10,FPSW", "FD68A3");
        }

        [Test]
        public void RxDis_mvtipl()
        {
            AssertCode("mvtipl\t#0h", "75700F");
        }

        [Test]
        public void RxDis_neg()
        {
            AssertCode("neg\tr3", "7E13");
            AssertCode("neg\tr3,r4", "FC0734");
        }

        [Test]
        public void RxDis_not()
        {
            AssertCode("not\tr3", "7E03");
            AssertCode("not\tr3,r4", "FC3B34");
        }

        [Test]
        public void RxDis_or()
        {
            AssertCode("or\t#0Ah,r10", "65AA");
            AssertCode("or\t#1234h,r4", "7634 1234");
            AssertCode("or.ub\t52[r2],r3", "5523 34");
            AssertCode("or.b\t52[r2],r2", "06D522 34");
            AssertCode("or\tr2,r3,r1", "FF5231");
        }

        [Test]
        public void RxDis_pop()
        {
            AssertCode("pop\tr10", "7EBA");
        }

        [Test]
        public void RxDis_popc()
        {
            AssertCode("popc\tISP", "7EEA");
        }

        [Test]
        public void RxDis_popm()
        {
            AssertCode("popm\tr2-r4", "6F24");
        }

        [Test]
        public void RxDis_push()
        {
            AssertCode("push\tr3", "7E93");
            AssertCode("push.w\t4660[r3]", "F639 1234");
        }

        [Test]
        public void RxDis_pushc()
        {
            AssertCode("pushc\tBPC", "7EC9");
        }

        [Test]
        public void RxDis_pushm()
        {
            AssertCode("pushm\tr2-r4", "6E24");
        }

        [Test]
        public void RxDis_racl()
        {
            AssertCode("racl\t#2h,a1", "FD1990");
        }

        [Test]
        public void RxDis_racw()
        {
            AssertCode("racw\t#2h,a0", "FD1810");
        }

        [Test]
        public void RxDis_rdacl()
        {
            AssertCode("rdacl\t#2h,a1", "FD19D0");
        }

        [Test]
        public void RxDis_rdacw()
        {
            AssertCode("rdacw\t#2h,a0", "FD18D0");
        }

        [Test]
        public void RxDis_revl()
        {
            AssertCode("revl\tr2,r3", "FD6723");
        }

        [Test]
        public void RxDis_revw()
        {
            AssertCode("revw\tr2,r3", "FD6523");
        }

        [Test]
        public void RxDis_rmpa()
        {
            AssertCode("rmpa.l", "7F8E");
        }

        [Test]
        public void RxDis_rolc()
        {
            AssertCode("rolc\tr2", "7E52");
        }

        [Test]
        public void RxDis_rorc()
        {
            AssertCode("rorc\tr2", "7E42");
        }

        [Test]
        public void RxDis_rotl()
        {
            AssertCode("rotl\t#1Fh,r3", "FD6FF3");
            AssertCode("rotl\tr3,r4", "FD6634");
        }

        [Test]
        public void RxDis_rotr()
        {
            AssertCode("rotr\t#1Fh,r3", "FD6DF3");
            AssertCode("rotr\tr3,r4", "FD6434");
        }

        [Test]
        public void RxDis_round()
        {
            AssertCode("round.l\t-2[r2],r3", "FC9923 FE");
        }

        [Test]
        public void RxDis_rte()
        {
            AssertCode("rte", "7F95");
        }

        [Test]
        public void RxDis_rtfi()
        {
            AssertCode("rtfi", "7F94");
        }

        [Test]
        public void RxDis_rts()
        {
            AssertCode("rts", "02");
        }

        [Test]
        public void RxDis_rtsd()
        {
            AssertCode("rtsd\t#2h", "6702");
            AssertCode("rtsd\t#2h,r1-r3", "3F13 02");
        }

        [Test]
        public void RxDis_sat()
        {
            AssertCode("sat\tr2", "7E32");
        }

        [Test]
        public void RxDis_satr()
        {
            AssertCode("satr", "7F93");
        }

        [Test]
        public void RxDis_sbb()
        {
            AssertCode("sbb\tr2,r3", "FC0323");
            AssertCode("sbb.l\t4660[r2],r3", "06A20023 1234");
        }

        [Test]
        public void RxDis_sceq()
        {
            AssertCode("sceq.b\t2[r3]", "FCD130 02");
        }

        [Test]
        public void RxDis_scmpu()
        {
            AssertCode("scmpu", "7F83");
        }

        [Test]
        public void RxDis_setpsw()
        {
            AssertCode("setpsw\tC", "7FA0");
        }

        [Test]
        public void RxDis_shar()
        {
            AssertCode("shar\t#1Fh,r3", "6BF3");
            AssertCode("shar\tr3,r2", "FD6132");
            AssertCode("shar\t#11h,r3,r2", "FDB132");
        }

        [Test]
        public void RxDis_shll()
        {
            AssertCode("shll\t#1Fh,r3", "6DF3");
            AssertCode("shll\tr3,r2", "FD6232");
            AssertCode("shll\t#11h,r3,r2", "FDD132");
        }

        [Test]
        public void RxDis_shlr()
        {
            AssertCode("shlr\t#1Fh,r3", "69F3");
            AssertCode("shlr\tr3,r2", "FD6032");
            AssertCode("shlr\t#11h,r3,r2", "FD9132");
        }

        [Test]
        public void RxDis_smovb()
        {
            AssertCode("smovb", "7F8B");
        }

        [Test]
        public void RxDis_smovf()
        {
            AssertCode("smovf", "7F8F");
        }

        [Test]
        public void RxDis_smovu()
        {
            AssertCode("smovu", "7F87");
        }

        [Test]
        public void RxDis_sstr()
        {
            AssertCode("sstr.w", "7F89");
        }

        [Test]
        public void RxDis_stnz()
        {
            AssertCode("stnz\t#1234h,r3", "FD78F3 1234");
            AssertCode("stnz\tr2,r3", "FC4B23");
        }

        [Test]
        public void RxDis_stz()
        {
            AssertCode("stz\t#1234h,r3", "FD78E3 1234");
            AssertCode("stnz\tr2,r3", "FC4B23");
        }

        [Test]
        public void RxDis_sub()
        {
            AssertCode("sub\t#2h,r3", "6023");
            AssertCode("sub.ub\t-128[r2],r3", "4123 80");
            AssertCode("sub.b\t-128[r2],r3", "06C123 80");
            AssertCode("sub\tr2,r3,r1", "FF0231");
        }

        [Test]
        public void RxDis_suntil()
        {
            AssertCode("suntil.l", "7F82");
        }

        [Test]
        public void RxDis_swhile()
        {
            AssertCode("swhile.w", "7F85");
        }

        [Test]
        public void RxDis_tst()
        {
            AssertCode("tst\t#1234h,r3", "FD78C3 1234");
            AssertCode("tst.ub\t18[r2],r3", "FC3123 12");
            AssertCode("tst.w\t[r2],r3", "06610C23 12");
        }

        [Test]
        public void RxDis_utof()
        {
            AssertCode("utof.ub\t18[r2],r3", "FC5523 12");
            AssertCode("utof.w\t18[r2],r3", "06611523 12");
        }

        [Test]
        public void RxDis_wait()
        {
            AssertCode("wait", "7F96");
        }

        [Test]
        public void RxDis_xchg()
        {
            AssertCode("xchg.ub\t18[r2],r3", "FC4123 12");
            AssertCode("xchg.w\t[r2],r3", "06611023 12");
        }

        [Test]
        public void RxDis_xor()
        {
            AssertCode("xor\t#12h,r3", "FD74D3 12");
            AssertCode("xor.ub\t18[r2],r3", "FC3523 12");
            AssertCode("xor.w\t18[r2],r3", "06610D23 12");
        }

    }
}
