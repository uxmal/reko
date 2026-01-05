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
using Reko.Arch.Avr;
using Reko.Arch.Avr.Avr32;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Avr
{
    [TestFixture]
    public class Avr32DisassemblerTests : DisassemblerTestBase<Avr32Instruction>
    {
        private readonly Avr32Architecture arch;

        public Avr32DisassemblerTests()
        {
            this.arch = new Avr32Architecture(CreateServiceContainer(), "avr32", new Dictionary<string, object>());
            this.LoadAddress = Address.Ptr32(0x00100000);
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void Avr32Dis_abs()
        {
            AssertCode("abs\tr11", "5C4B");
        }

        [Test]
        public void Avr32Dis_acall()
        {
            AssertCode("acall\t+00000154", "D550");
        }

        [Test]
        public void Avr32Dis_acr()
        {
            AssertCode("acr\tr5", "5C05");
        }

        [Test]
        public void Avr32Dis_adc()
        {
            AssertCode("adc\tr1,r2,r3", "E403 0041");
        }

        [Test]
        public void Avr32Dis_add2()
        {
            AssertCode("add\tr8,r10", "1408");
        }

        [Test]
        public void Avr32Dis_add3()
        {
            AssertCode("add\tr9,r8,r9<<3", "F0090039");
        }

        [Test]
        public void Avr32Dis_add_cond()
        {
            AssertCode("addle\tr1,r2,r3", "E5D3 EA01");
        }

        [Test]
        public void Avr32Dis_addabs()
        {
            AssertCode("addabs\tr3,r4,r5", "E805 0E43");
        }

        [Test]
        public void Avr32Dis_addhh_w()
        {
            AssertCode("addhh.w\tr11,r12:t,sp:t", "F80D 0E3B");
        }

        [Test]
        public void Avr32Dis_and_shl()
        {
            AssertCode("and\tr11,r12,sp<<30", "F9ED 01EB");
        }

        [Test]
        public void Avr32Dis_and_shr()
        {
            AssertCode("and\tr11,r12,sp>>30", "F9ED 03EB");
        }

        [Test]
        public void Avr32Dis_and_cond()
        {
            AssertCode("andcs\tr11,r12,sp", "F9DD E32B");
        }

        [Test]
        public void Avr32Dis_andh()
        {
            AssertCode("andh\tsp,0123,COH", "E61D 0123");
        }

        [Test]
        public void Avr32Dis_andh_COH()
        {
            AssertCode("andh\tr12,8000,COH", "E61C8000");
        }

        [Test]
        public void Avr32Dis_andl()
        {
            AssertCode("andl\tsp,0123,COH", "E21D 0123");
        }

        [Test]
        public void Avr32Dis_andn()
        {
            AssertCode("andn\tr12,r11", "168C");
        }

        [Test]
        public void Avr32Dis_asr2()
        {
            AssertCode("asr\tr8,+00000001", "A158");
        }

        [Test]
        public void Avr32Dis_asr3()
        {
            AssertCode("asr\tr8,r8,r9", "F0090848");
        }

        [Test]
        public void Avr32Dis_bfexts()
        {
            AssertCode("bfexts\tr11,r12,+00000009,+00000010", "F7DC B130");
        }

        [Test]
        public void Avr32Dis_bfextu()
        {
            AssertCode("bfextu\tr11,r12,+00000009,+00000010", "F7DC C130");
            AssertCode("bfextu\tr2,r7,+00000001,+00000001", "E5D7C021");
            AssertCode("invalid", "E5D7C020");
            AssertCode("invalid", "E5D7C39F");
        }

        [Test]
        public void Avr32Dis_bfins()
        {
            AssertCode("bfins\tr9,r2,+00000009,+00000010", "F3D2 D130");
        }

        [Test]
        public void Avr32Dis_bld()
        {
            AssertCode("bld\tr11,+00000014", "EDBB0014");
        }

        [Test]
        public void Avr32Dis_breq()
        {
            AssertCode("breq\t00100008", "C040");
        }

        [Test]
        public void Avr32Dis_breq_long()
        {
            AssertCode("breq\t001001BA", "E08000DD");
        }

        [Test]
        public void Avr32Dis_brmi_long()
        {
            AssertCode("brmi\t000FFE5C", "FE96FF2E");
        }

        [Test]
        public void Avr32Dis_breakpoint()
        {
            AssertCode("breakpoint", "D673");
        }

        [Test]
        public void Avr32Dis_brev()
        {
            AssertCode("brev\tr3", "5C93");
        }

        [Test]
        public void Avr32Dis_bst()
        {
            AssertCode("bst\tr3,+0000001F", "EFB3001F");
        }

        [Test]
        public void Avr32Dis_cache()
        {
            AssertCode("cache\tr10[-1024],15", "F413 AC00");
        }

        [Test]
        public void Avr32Dis_casts_b()
        {
            AssertCode("casts.b\tr3", "5C63");
        }

        [Test]
        public void Avr32Dis_casts_h()
        {
            AssertCode("casts.h\tr4", "5C84");
        }

        [Test]
        public void Avr32Dis_castu_b()
        {
            AssertCode("castu.b\tr12", "5C5C");
        }

        [Test]
        public void Avr32Dis_castu_h()
        {
            AssertCode("castu.h\tr3", "5C73");
        }

        [Test]
        public void Avr32Dis_cbr()
        {
            AssertCode("cbr\tr8,+00000001", "A1D8");
        }

        [Test]
        public void Avr32Dis_clz()
        {
            AssertCode("clz\tr12,r11", "F60C 1200");
        }

        [Test]
        public void Avr32Dis_com()
        {
            AssertCode("com\tr3", "5CD3");
        }

        [Test]
        public void Avr32Dis_cop()
        {
            AssertCode("cop\t05,cr1,cr2,cr3,7F", "E7AF B123");
        }

        [Test]
        public void Avr32Dis_cp_b()
        {
            AssertCode("cp.b\tr9,r8", "F0091800");
        }

        [Test]
        public void Avr32Dis_cp_h()
        {
            AssertCode("cp.h\tr9,r8", "F009 1900");
        }

        [Test]
        public void Avr32Dis_cp_w()
        {
            AssertCode("cp.w\tr1,r2", "0431");
            AssertCode("cp.w\tr8,FFFFFFE0", "5A08");
            AssertCode("cp.w\tr3,FFFF8000", "FE53 8000");
        }

        [Test]
        public void Avr32Dis_cpc()
        {
            AssertCode("cpc\tr9", "5C29");
            AssertCode("cpc\tr10,r11", "F60A1300");
        }

        [Test]
        public void Avr32Dis_csrf()
        {
            AssertCode("csrf\t15", "D553");
        }

        [Test]
        public void Avr32Dis_csrfcz()
        {
            AssertCode("csrfcz\t15", "D153");
        }

        [Test]
        public void Avr32Dis_divs()
        {
            AssertCode("divs\tr2,r3,r8", "E6080C02");
        }

        [Test]
        public void Avr32Dis_divu()
        {
            AssertCode("divu\tr2,r3,r8", "E6080D02");
        }

        [Test]
        public void Avr32Dis_eor3_lsl()
        {
            AssertCode("eor\tr12,r11,r9<<4", "F7E9204C");
        }

        [Test]
        public void Avr32Dis_eor3_lsr()
        {
            AssertCode("eor\tr12,r11,r9>>4", "F7E9224C");
        }

        [Test]
        public void Avr32Dis_eorcond()
        {
            AssertCode("eorle\tr10,r11,r12", "F7DCEA4A");
        }

        [Test]
        public void Avr32Dis_eorh()
        {
            AssertCode("eorh\tr8,0002", "EE180002");
        }

        [Test]
        public void Avr32Dis_eorl()
        {
            AssertCode("eorl\tr8,0002", "EC180002");
        }

        [Test]
        public void Avr32Dis_frs()
        {
            AssertCode("frs", "D743");
        }

        [Test]
        public void Avr32Dis_icall()
        {
            AssertCode("icall\tr4", "5D14");
        }

        [Test]
        public void Avr32Dis_incjosp()
        {
            AssertCode("incjosp\t-00000001", "D6F3");
        }

        [Test]
        public void Avr32Dis_ld_d()
        {
            AssertCode("ld.d\tr3:r2,lr++", "BD03");
            AssertCode("ld.d\tr3:r2,lr[0]", "BD12");
            AssertCode("ld.d\tr1:r0,--lr", "BD00");
            AssertCode("ld.d\tr11:r10,r12[0]", "F8EA0000");
            AssertCode("ld.d\tr1:r0,r12[r3<<3]", "F8030231");
        }

        [Test]
        public void Avr32Dis_ld_sb()
        {
            AssertCode("ld.sb\tr1,sp[-32768]", "FB218000");
            AssertCode("ld.sb\tr1,sp[r2<<3]", "FA020631");
        }

        [Test]
        public void Avr32Dis_ld_sb_cond()
        {
            AssertCode("ld.sbge\tr1,sp[-256]", "FBF1 4700");
        }

        [Test]
        public void Avr32Dis_ld_ub()
        {
            AssertCode("ld.ub\tr1,sp++", "1B31");
            AssertCode("ld.ub\tr1,--sp", "1B71");
            AssertCode("ld.ub\tr9,r8[0]", "1189");
            AssertCode("ld.ub\tr1,sp[-32768]", "FB31 8000");
            AssertCode("ld.ub\tr1,sp[r2<<3]", "FA02 0731");
        }

        [Test]
        public void Avr32Dis_ld_ub_cond()
        {
            AssertCode("ld.ubvs\tr1,sp[-256]", "FBF1 C900");
        }

        [Test]
        public void Avr32Dis_ld_sh()
        {
            AssertCode("ld.sh\tr1,sp++", "1B11");
            AssertCode("ld.sh\tr1,--sp", "1B51");
            AssertCode("ld.sh\tr1,sp[-2]", "9A71");
            AssertCode("ld.sh\tr1,sp[-32768]", "FB01 8000");
            AssertCode("ld.sh\tr1,sp[r2<<3]", "FA02 0431");
        }

        [Test]
        public void Avr32Dis_ld_sh_cond()
        {
            AssertCode("ld.shlt\tr1,sp[-512]", "FBF1 5300");
        }

        [Test]
        public void Avr32Dis_ld_uh()
        {
            AssertCode("ld.uh\tr1,sp++", "1B21");
            AssertCode("ld.uh\tr1,--sp", "1B61");
            AssertCode("ld.uh\tr1,r5[-2]", "8AF1");
            AssertCode("ld.uh\tr1,sp[-32768]", "FB11 8000");
            AssertCode("ld.uh\tr1,sp[r2<<3]", "FA02 0531");
        }

        [Test]
        public void Avr32Dis_ld_uh_cond()
        {
            AssertCode("ld.uhlt\tr1,sp[-512]", "FBF1 5500");
        }

        [Test]
        public void Avr32Dis_ld_w()
        {
            AssertCode("ld.w\tr1,sp++", "1B01");
            AssertCode("ld.w\tr1,--sp", "1B41");
            AssertCode("ld.w\tr1,r5[-64]", "6B01");
            AssertCode("ld.w\tr1,sp[-32768]", "FAF1 8000");
            AssertCode("ld.w\tr1,sp[r2<<3]", "FA02 0331");
            AssertCode("ld.w\tr1,sp[r2:b<<2]", "FA02 0FB1");
        }

        [Test]
        public void Avr32Dis_ld_w_cond()
        {
            AssertCode("ld.whi\tr1,sp[-1024]", "FBF1 B100");
        }

        [Test]
        public void Avr32Dis_ldc_d()
        {
            AssertCode("ldc.d\t07,cr14,sp[-1024]", "E9AD FE80");
            AssertCode("ldc.d\t07,cr14,sp++", "EFAD EE50");
            AssertCode("ldc.d\t07,cr14,sp[r2<<3]", "EFAD FE72");
        }

        [Test]
        public void Avr32Dis_ldc_w()
        {
            AssertCode("ldc.w\t07,cr14,sp[-1024]", "E9AD EE80");
            AssertCode("ldc.w\t07,cr14,sp++", "EFAD EE40");
            AssertCode("ldc.w\t07,cr15,sp[r2<<3]", "EFAD FF32");
        }

        [Test]
        public void Avr32Dis_ldc0_d()
        {
            AssertCode("ldc0.d\tcr14,sp[-512]", "F3AD FE80");
        }

        [Test]
        public void Avr32Dis_ldc0_w()
        {
            AssertCode("ldc0.w\tcr15,sp[-512]", "F1AD FF80");
        }

        [Test]
        public void Avr32Dis_ldcm_d()
        {
            AssertCode("ldcm.d\t07,sp,cr0-cr14", "EDAD E4FF");
            AssertCode("ldcm.d\t07,sp++,cr0-cr14", "EDAD F4FF");
        }

        [Test]
        public void Avr32Dis_ldcm_w()
        {
            AssertCode("ldcm.w\t07,sp++,cr0-cr7", "EDAD F0FF");
            AssertCode("ldcm.w\t07,sp++,cr8-cr15", "EDAD F1FF");
        }

        [Test]
        public void Avr32Dis_lddpc()
        {
            AssertCode("lddpc\tr6,pc[20]", "4856");
        }

        [Test]
        public void Avr32Dis_lddpc_to_pc()
        {
            var instr = DisassembleHexBytes("485F");
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
            Assert.AreEqual("lddpc\tpc,pc[20]", instr.ToString());
        }

        [Test]
        public void Avr32Dis_lddsp()
        {
            AssertCode("lddsp\tr1,sp[256]", "4401");
        }

        [Test]
        public void Avr32Dis_ldins_b()
        {
            AssertCode("ldins.b\tr1:t,sp[-2048]", "FBD1 4800");
            AssertCode("ldins.b\tr1:u,sp[-2048]", "FBD1 5800");
            AssertCode("ldins.b\tr1:l,sp[-2048]", "FBD1 6800");
            AssertCode("ldins.b\tr1:b,sp[-2048]", "FBD1 7800");
        }

        [Test]
        public void Avr32Dis_ldins_h()
        {
            AssertCode("ldins.h\tr1:b,sp[-2048]", "FBD1 0800");
            AssertCode("ldins.h\tr1:t,sp[-2048]", "FBD1 1800");
        }

        [Test]
        public void Avr32Dis_ldm_post()
        {
            var instr = DisassembleHexBytes("E3CD8040");
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
            Assert.AreEqual("ldm\tsp++,r6,pc", instr.ToString());
        }

        [Test]
        public void Avr32Dis_ldmts_post()
        {
            var instr = DisassembleHexBytes("E7CD8040");
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
            Assert.AreEqual("ldmts\tsp++,r6,pc", instr.ToString());
        }

        [Test]
        public void Avr32Dis_ldswp_sh()
        {
            AssertCode("ldswp.sh\tr1,sp[-2048]", "FBD1 2800");
        }

        [Test]
        public void Avr32Dis_ldswp_uh()
        {
            AssertCode("ldswp.uh\tr1,sp[-2048]", "FBD1 3800");
        }

        [Test]
        public void Avr32Dis_ldswp_w()
        {
            AssertCode("ldswp.w\tr1,sp[-2048]", "FBD1 8800");
        }

        [Test]
        public void Avr32Dis_lsl2()
        {
            AssertCode("lsl\tr11,+00000001", "A17B");
        }

        [Test]
        public void Avr32Dis_lsl_imm()
        {
            AssertCode("lsl\tr12,r11,00000001", "F60C1501");
        }

        [Test]
        public void Avr32Dis_lsr2()
        {
            AssertCode("lsr\tr10,+00000008", "A98A");
        }

        [Test]
        public void Avr32Dis_lsr_long()
        {
            AssertCode("lsr\tr7,r11,00000014", "F6071614");
        }

        [Test]
        public void Avr32Dis_mac()
        {
            AssertCode("mac\tr1,r2,r3", "E403 0341");
        }

        [Test]
        public void Avr32Dis_machh_d()
        {
            AssertCode("machh.d\tr1,r2:t,r3:t", "E403 05B1");
        }

        [Test]
        public void Avr32Dis_machh_w()
        {
            AssertCode("machh.w\tr1,r2:t,r3:t", "E403 04B1");
        }

        [Test]
        public void Avr32Dis_macs_d()
        {
            AssertCode("macs.d\tr1,r2,r3", "E403 0541");
        }

        [Test]
        public void Avr32Dis_macsathh_w()
        {
            AssertCode("macsathh.w\tr1,r2:t,r3:t", "E40306B1");
        }

        [Test]
        public void Avr32Dis_macu_d()
        {
            AssertCode("macu.d\tr1,r2,r3", "E4030741");
        }

        [Test]
        public void Avr32Dis_macwh_d()
        {
            AssertCode("macwh.d\tr1,r2,r3:t", "E4030C91");
        }

        [Test]
        public void Avr32Dis_max()
        {
            AssertCode("max\tr1,r2,r3", "E4030C41");
        }

        [Test]
        public void Avr32Dis_mcall()
        {
            AssertCode("mcall\tr6[480]", "F0160078");
        }

        [Test]
        public void Avr32Dis_memc()
        {
            AssertCode("memc\tFFFFC000,11", "F618 C000");
        }

        [Test]
        public void Avr32Dis_mems()
        {
            AssertCode("mems\tFFFFC000,11", "F818 C000");
        }

        [Test]
        public void Avr32Dis_memt()
        {
            AssertCode("memt\tFFFFC000,11", "FA18 C000");
        }

        [Test]
        public void Avr32Dis_mfdr()
        {
            AssertCode("mfdr\tr1,00000080", "E5B1 0080");
        }

        [Test]
        public void Avr32Dis_mfsr()
        {
            AssertCode("mfsr\tr1,00000080", "E1B1 0080");
        }

        [Test]
        public void Avr32Dis_min()
        {
            AssertCode("min\tr1,r2,r3", "E403 0D41");
        }

        [Test]
        public void Avr32Dis_mov()
        {
            AssertCode("mov\tr7,FFFFFFFE", "3FE7");
            AssertCode("mov\tr8,000007D9", "E06807D9");
            AssertCode("mov\tr10,sp", "1A9A");
        }

        [Test]
        public void Avr32Dis_movcond()
        {
            AssertCode("movne\tr12,r8", "F00C1710");
        }

        [Test]
        public void Avr32Dis_movh()
        {
            AssertCode("movh\tlr,1892", "FC1E1892");
        }

        [Test]
        public void Avr32Dis_mtdr()
        {
            AssertCode("mtdr\t00000080,r2", "E7B2 0080");
        }

        [Test]
        public void Avr32Dis_mtsr()
        {
            AssertCode("mtsr\t00000080,r2", "E3B2 0080");
        }

        [Test]
        public void Avr32Dis_mul()
        {
            AssertCode("mul\tr1,r2", "A531");
            AssertCode("mul\tr1,r2,r3", "E4030241");
            AssertCode("mul\tr1,r2,FFFFFF80", "E4011080");
        }

        [Test]
        public void Avr32Dis_mulhh_w()
        {
            AssertCode("mulhh.w\tr1,r2:t,r3:b", "E40307A1");
        }

        [Test]
        public void Avr32Dis_mulnhh_w()
        {
            AssertCode("mulnhh.w\tr1,r2:b,r3:t", "E4030191");
        }

        [Test]
        public void Avr32Dis_mulnwh_d()
        {
            AssertCode("mulnwh.d\tr1,r2:b,r3:t", "E4030291");
        }

        [Test]
        public void Avr32Dis_muls_d()
        {
            AssertCode("muls.d\tr1,r2,r3", "E403 0441");
        }

        [Test]
        public void Avr32Dis_mulsathh_h()
        {
            AssertCode("mulsathh.h\tr1,r2:b,r3:t", "E403 0891");
        }

        [Test]
        public void Avr32Dis_mulsathh_w()
        {
            AssertCode("mulsathh.w\tr1,r2:b,r3:t", "E403 0991");
        }

        [Test]
        public void Avr32Dis_mulsatrndhh_w()
        {
            AssertCode("mulsatrndhh.h\tr1,r2:b,r3:t", "E403 0A91");
        }

        [Test]
        public void Avr32Dis_mulsatrndwh_w()
        {
            AssertCode("mulsatrndwh.w\tr1,r2:b,r3:t", "E403 0B91");
        }

        [Test]
        public void Avr32Dis_mulsatwh_w()
        {
            AssertCode("mulsatwh.w\tr1,r2,r3:t", "E403 0E91");
        }

        [Test]
        public void Avr32Dis_mulu_d()
        {
            AssertCode("mulu.d\tr4,r11,r8", "F6080644");
        }

        [Test]
        public void Avr32Dis_mulwh_d()
        {
            AssertCode("mulwh.d\tr1,r2,r3:t", "E403 0D91");
        }

        [Test]
        public void Avr32Dis_musfr()
        {
            AssertCode("musfr\tr1", "5D31");
        }

        [Test]
        public void Avr32Dis_mustr()
        {
            AssertCode("mustr\tr1", "5D21");
        }

        [Test]
        public void Avr32Dis_mvcr_d()
        {
            AssertCode("mvcr.d\t07,lr,cr14", "EFAE EE10");
        }

        [Test]
        public void Avr32Dis_mvcr_w()
        {
            AssertCode("mvcr.w\t07,lr,cr15", "EFAE EF00");
        }

        [Test]
        public void Avr32Dis_mvrc_d()
        {
            AssertCode("mvrc.d\t07,cr14,lr", "EFAE EE30");
        }

        [Test]
        public void Avr32Dis_mvrc_w()
        {
            AssertCode("mvrc.w\t07,cr15,lr", "EFAE EF20");
        }

        [Test]
        public void Avr32Dis_neg()
        {
            AssertCode("neg\tr1", "5C31");
        }

        [Test]
        public void Avr32Dis_nop()
        {
            AssertCode("nop", "D703");
        }

        [Test]
        public void Avr32Dis_or()
        {
            AssertCode("or\tr1,r2", "0441");
            AssertCode("or\tr12,r10,r11<<1", "F5EB101C");
            AssertCode("or\tr1,r2,r3>>17", "E5E31311");
        }

        [Test]
        public void Avr32Dis_or_cond()
        {
            AssertCode("orlt\tr1,r2,r3", "E5D3E531");
        }

        [Test]
        public void Avr32Dis_orh()
        {
            AssertCode("orh\tr1,8000", "EA118000");
        }

        [Test]
        public void Avr32Dis_orl()
        {
            AssertCode("orl\tr1,8000", "E8118000");
        }

        [Test]
        public void Avr32Dis_pabs_sb()
        {
            AssertCode("pabs.sb\tr1,r2", "E002 23E1");
        }

        [Test]
        public void Avr32Dis_pabs_sw()
        {
            AssertCode("pabs.sw\tr1,r2", "E002 23F1");
        }

        [Test]
        public void Avr32Dis_packsh_sb()
        {
            AssertCode("packsh.sb\tr1,r2,r3", "E403 24D1");
        }

        [Test]
        public void Avr32Dis_packsh_ub()
        {
            AssertCode("packsh.ub\tr1,r2,r3", "E403 24C1");
        }

        [Test]
        public void Avr32Dis_packw_sh()
        {
            AssertCode("packw.sh\tr1,r2,r3", "E403 2471");
        }

        [Test]
        public void Avr32Dis_padd_b()
        {
            AssertCode("padd.b\tr1,r2,r3", "E403 2301");
        }

        [Test]
        public void Avr32Dis_padd_h()
        {
            AssertCode("padd.h\tr1,r2,r3", "E403 2001");
        }

        [Test]
        public void Avr32Dis_paddh_sh()
        {
            AssertCode("paddh.sh\tr1,r2,r3", "E403 20C1");
        }

        [Test]
        public void Avr32Dis_paddh_ub()
        {
            AssertCode("paddh.ub\tr1,r2,r3", "E403 2361");
        }

        [Test]
        public void Avr32Dis_padds_sb()
        {
            AssertCode("padds.sb\tr1,r2,r3", "E403 2321");
        }

        [Test]
        public void Avr32Dis_padds_sh()
        {
            AssertCode("padds.sh\tr1,r2,r3", "E403 2041");
        }

        [Test]
        public void Avr32Dis_padds_ub()
        {
            AssertCode("padds.ub\tr1,r2,r3", "E403 2341");
        }

        [Test]
        public void Avr32Dis_padds_uh()
        {
            AssertCode("padds.uh\tr1,r2,r3", "E403 2081");
        }

        [Test]
        public void Avr32Dis_paddsub_h()
        {
            AssertCode("paddsub.h\tr1,r2:t,r3:t", "E403 2131");
        }

        [Test]
        public void Avr32Dis_paddsubh_sh()
        {
            AssertCode("paddsubh.sh\tr1,r2:t,r3:t", "E403 22B1");
        }

        [Test]
        public void Avr32Dis_paddsubs_sh()
        {
            AssertCode("paddsubs.sh\tr1,r2:t,r3:t", "E403 21B1");
        }

        [Test]
        public void Avr32Dis_paddsubs_uh()
        {
            AssertCode("paddsubs.uh\tr1,r2:t,r3:t", "E403 2231");
        }

        [Test]
        public void Avr32Dis_paddx_h()
        {
            AssertCode("paddx.h\tr1,r2,r3", "E403 2021");
        }

        [Test]
        public void Avr32Dis_paddxh_sh()
        {
            AssertCode("paddxh.sh\tr1,r2,r3", "E403 20E1");
        }

        [Test]
        public void Avr32Dis_paddxs_sh()
        {
            AssertCode("paddxs.sh\tr1,r2,r3", "E403 2061");
        }

        [Test]
        public void Avr32Dis_paddxs_uh()
        {
            AssertCode("paddxs.uh\tr1,r2,r3", "E403 20A1");
        }

        [Test]
        public void Avr32Dis_pasr_b()
        {
            AssertCode("pasr.b\tr1,r2,03", "E403 2411");
        }

        [Test]
        public void Avr32Dis_pasr_h()
        {
            AssertCode("pasr.h\tr1,r2,03", "E403 2441");
        }

        [Test]
        public void Avr32Dis_pavg_sh()
        {
            AssertCode("pavg.sh\tr1,r2,r3", "E403 23D1");
        }

        [Test]
        public void Avr32Dis_pavg_ub()
        {
            AssertCode("pavg.ub\tr1,r2,r3", "E403 23C1");
        }

        [Test]
        public void Avr32Dis_plsl_b()
        {
            AssertCode("plsl.b\tr1,r2,03", "E403 2421");
        }

        [Test]
        public void Avr32Dis_plsl_h()
        {
            AssertCode("plsl.h\tr1,r2,03", "E403 2451");
        }

        [Test]
        public void Avr32Dis_plsr_b()
        {
            AssertCode("plsr.b\tr1,r2,03", "E403 2431");
        }

        [Test]
        public void Avr32Dis_plsr_h()
        {
            AssertCode("plsr.h\tr1,r2,03", "E403 2461");
        }

        [Test]
        public void Avr32Dis_pmax_sh()
        {
            AssertCode("pmax.sh\tr1,r2,r3", "E403 2391");
        }

        [Test]
        public void Avr32Dis_pmax_ub()
        {
            AssertCode("pmax.ub\tr1,r2,r3", "E403 2381");
        }

        [Test]
        public void Avr32Dis_pmin_sh()
        {
            AssertCode("pmin.sh\tr1,r2,r3", "E403 23B1");
        }

        [Test]
        public void Avr32Dis_pmin_ub()
        {
            AssertCode("pmin.ub\tr1,r2,r3", "E403 23A1");
        }

        [Test]
        public void Avr32Dis_popjc()
        {
            AssertCode("popjc", "D713");
        }

        [Test]
        public void Avr32Dis_popm_pc()
        {
            var instr = DisassembleHexBytes("D822");
            Assert.AreEqual("popm\tr4-r7,pc", instr.ToString());
            Assert.AreEqual(InstrClass.Transfer, instr.InstructionClass);
        }

        [Test]
        public void Avr32Dis_pref()
        {
            AssertCode("pref\tr9[-32768]", "F21D8000");
        }

        [Test]
        public void Avr32Dis_psad()
        {
            AssertCode("psad\tr1,r2,r3", "E403 2401");
        }

        [Test]
        public void Avr32Dis_psub_b()
        {
            AssertCode("psub.b\tr1,r2,r3", "E403 2311");
        }

        [Test]
        public void Avr32Dis_psub_h()
        {
            AssertCode("psub.h\tr1,r2,r3", "E403 2011");
        }

        [Test]
        public void Avr32Dis_psubadd_h()
        {
            AssertCode("psubadd.h\tr1,r2:t,r3:t", "E403 2171");
        }

        [Test]
        public void Avr32Dis_psubaddh_sh()
        {
            AssertCode("psubaddh.sh\tr1,r2:t,r3:t", "E403 22F1");
        }

        [Test]
        public void Avr32Dis_psubadds_sh()
        {
            AssertCode("psubadds.sh\tr1,r2:t,r3:t", "E403 21F1");
        }

        [Test]
        public void Avr32Dis_psubadds_uh()
        {
            AssertCode("psubadds.uh\tr1,r2:t,r3:t", "E403 2271");
        }

        [Test]
        public void Avr32Dis_psubh_sh()
        {
            AssertCode("psubh.sh\tr1,r2,r3", "E403 20D1");
        }

        [Test]
        public void Avr32Dis_psubh_ub()
        {
            AssertCode("psubh.ub\tr1,r2,r3", "E403 2371");
        }

        [Test]
        public void Avr32Dis_psubs_sb()
        {
            AssertCode("psubs.sb\tr1,r2,r3", "E403 2331");
        }

        [Test]
        public void Avr32Dis_psubs_sh()
        {
            AssertCode("psubs.sh\tr1,r2,r3", "E403 2051");
        }

        [Test]
        public void Avr32Dis_psubs_ub()
        {
            AssertCode("psubs.ub\tr1,r2,r3", "E403 2351");
        }

        [Test]
        public void Avr32Dis_psubs_uh()
        {
            AssertCode("psubs.uh\tr1,r2,r3", "E403 2091");
        }

        [Test]
        public void Avr32Dis_psubx_h()
        {
            AssertCode("psubx.h\tr1,r2,r3", "E403 2031");
        }

        [Test]
        public void Avr32Dis_psubxh_sh()
        {
            AssertCode("psubxh.sh\tr1,r2,r3", "E403 20F1");
        }

        [Test]
        public void Avr32Dis_psubxs_sh()
        {
            AssertCode("psubxs.sh\tr1,r2,r3", "E403 2071");
        }

        [Test]
        public void Avr32Dis_psubxs_uh()
        {
            AssertCode("psubxs.uh\tr1,r2,r3", "E403 20B1");
        }

        [Test]
        public void Avr32Dis_punpcksb_h()
        {
            AssertCode("punpcksb.h\tr1,r2:t", "E403 24B1");
        }

        [Test]
        public void Avr32Dis_punpckub_h()
        {
            AssertCode("punpckub.h\tr1,r2:t", "E403 2491");
        }

        [Test]
        public void Avr32Dis_pushjc()
        {
            AssertCode("pushjc", "D723");
        }

        [Test]
        public void Avr32Dis_pushm()
        {
            AssertCode("pushm\tr4-r7,lr", "D421");
        }

        [Test]
        public void Avr32Dis_rcall()
        {
            AssertCode("rcall\t000FFD12", "C89E");
        }

        [Test]
        public void Avr32Dis_retd()
        {
            AssertCode("retd", "D623");
        }

        [Test]
        public void Avr32Dis_rete()
        {
            AssertCode("rete", "D603");
        }

        [Test]
        public void Avr32Dis_reteq()
        {
            AssertCode("reteq\tr11", "5E0B");
        }

        [Test]
        public void Avr32Dis_retj()
        {
            AssertCode("retj", "D633");
        }

        [Test]
        public void Avr32Dis_retmi()
        {
            AssertCode("retmi\tsp", "5E6D");
        }

        [Test]
        public void Avr32Dis_rets()
        {
            AssertCode("rets", "D613");
        }

        [Test]
        public void Avr32Dis_retss()
        {
            AssertCode("retss", "D763");
        }

        [Test]
        public void Avr32Dis_rjmp()
        {
            AssertCode("rjmp\t00100006", "C038");
        }

        [Test]
        public void Avr32Dis_rol()
        {
            AssertCode("rol\tr1", "5CF1");
        }

        [Test]
        public void Avr32Dis_ror()
        {
            AssertCode("ror\tr1", "5D01");
        }

        [Test]
        public void Avr32Dis_rsub()
        {
            AssertCode("rsub\tr6,pc", "1E26");
            AssertCode("rsub\tr9,r9,00000001", "F2091101");
        }

        [Test]
        public void Avr32Dis_rsub_cond()
        {
            AssertCode("rsublt\tr1,FFFFFFC0", "FBB1 05C0");
        }

        [Test]
        public void Avr32Dis_satadd_h()
        {
            AssertCode("satadd.h\tr1,r2,r3", "E403 02C1");
        }

        [Test]
        public void Avr32Dis_satadd_w()
        {
            AssertCode("satadd.w\tr1,r2,r3", "E403 00C1");
        }

        [Test]
        public void Avr32Dis_satrnds()
        {
            AssertCode("satrnds\tr1>>16,+00000010", "F3B1 0210");
        }

        [Test]
        public void Avr32Dis_satrndu()
        {
            AssertCode("satrndu\tr1>>16,+00000010", "F3B1 0610");
        }

        [Test]
        public void Avr32Dis_sats()
        {
            AssertCode("sats\tr1>>16,+00000010", "F1B1 0210");
        }

        [Test]
        public void Avr32Dis_satsub_h()
        {
            AssertCode("satsub.h\tr1,r2,r3", "E403 03C1");
        }

        [Test]
        public void Avr32Dis_satsub_w()
        {
            AssertCode("satsub.w\tr1,r2,r3", "E403 01C1");
            AssertCode("satsub.w\tr1,r2,FFFF8000", "E4D1 8000");    // Seems to be a bug in the manual
        }

        [Test]
        public void Avr32Dis_satu()
        {
            AssertCode("satu\tr1>>16,+00000010", "F1B1 0610");
        }

        [Test]
        public void Avr32Dis_sbc()
        {
            AssertCode("sbc\tr1,r2,r3", "E403 0141");
        }

        [Test]
        public void Avr32Dis_sbr()
        {
            AssertCode("sbr\tr1,+00000011", "B1B1");
        }

        [Test]
        public void Avr32Dis_scall()
        {
            AssertCode("scall", "D733");
        }

        [Test]
        public void Avr32Dis_scr()
        {
            AssertCode("scr\tr1", "5C11");
        }

        [Test]
        public void Avr32Dis_sleep()
        {
            AssertCode("sleep\t80", "E9B0 0080");
        }

        [Test]
        public void Avr32Dis_sr_cond()
        {
            AssertCode("srlt\tr1", "5F51");
        }

        [Test]
        public void Avr32Dis_sscall()
        {
            AssertCode("sscall", "D753");
        }

        [Test]
        public void Avr32Dis_ssrf()
        {
            AssertCode("ssrf\t10", "D303");
        }

        [Test]
        public void Avr32Dis_st_b()
        {
            AssertCode("st.b\tr7++,r9", "0EC9");
            AssertCode("st.b\t--r7,r9", "0EF9");
            AssertCode("st.b\tr8[0],r9", "B089");
            AssertCode("st.b\tsp[-32768],r2", "FB62 8000");
            AssertCode("st.b\tsp[r2<<3],r3", "FA02 0B33");
        }

        [Test]
        public void Avr32Dis_st_b_cond()
        {
            AssertCode("st.blt\tsp[-256],r1", "FBF1 5F00");
        }

        [Test]
        public void Avr32Dis_st_d()
        {
            AssertCode("st.d\tsp++,r5:r4", "BB24");
            AssertCode("st.d\t--sp,r5:r4", "BB25");
            AssertCode("st.d\tsp[0],r5:r4", "BB15");
            AssertCode("st.d\tr7[-36],r9:r8", "EEE9FFDC");
            AssertCode("st.d\tsp[r2<<3],r3:r2", "FA02 0833");
        }

        [Test]
        public void Avr32Dis_st_h()
        {
            AssertCode("st.h\tsp++,r2", "1AB2");
            AssertCode("st.h\t--sp,r2", "1AE2");
            AssertCode("st.h\tsp[-2],r2", "BA72");
            AssertCode("st.h\tsp[-32768],r2", "FB52 8000");
            AssertCode("st.h\tsp[r2<<3],r3", "FA02 0A33");
        }

        [Test]
        public void Avr32Dis_st_h_cond()
        {
            AssertCode("st.hlt\tsp[-512],r2", "FBF2 5D00");
        }

        [Test]
        public void Avr32Dis_st_w()
        {
            AssertCode("st.w\tr12++,sp", "18AD");
            AssertCode("st.w\t--sp,r10", "1ADA");
            AssertCode("st.w\tr10[0],r9", "9509");
            AssertCode("st.w\tr7[-8],r8", "EF48FFF8");
            AssertCode("st.w\tr11[r10<<2],r8", "F60A0928");
        }

        [Test]
        public void Avr32Dis_st_w_cond()
        {
            AssertCode("st.wlt\tsp[-1024],r2", "FBF2 5B00");
        }

        [Test]
        public void Avr32Dis_stc_d()
        {
            AssertCode("stc.d\t07,sp[-1024],cr14", "EBAD FE80");
            AssertCode("stc.d\t07,--sp,cr14", "EFAD EE70");
            AssertCode("stc.d\t07,sp[r2<<3],cr15", "EFAD FFF2");
        }

        [Test]
        public void Avr32Dis_stc_w()
        {
            AssertCode("stc.w\t07,sp[-1024],cr14", "EBAD EE80");
            AssertCode("stc.w\t07,--sp,cr14", "EFAD EE60");
            AssertCode("stc.w\t07,sp[r2<<3],cr15", "EFAD FFB2");
        }

        [Test]
        public void Avr32Dis_stc0_d()
        {
            AssertCode("stc0.d\tsp[-512],cr14", "F7AD FE80");
        }

        [Test]
        public void Avr32Dis_stc0_w()
        {
            AssertCode("stc0.w\tsp[-512],cr15", "F5AD FF80");
        }

        [Test]
        public void Avr32Dis_stcm_d()
        {
            AssertCode("stcm.d\t07,--sp,cr0-cr14", "EDAD F5FF");
        }

        [Test]
        public void Avr32Dis_stcm_w()
        {
            AssertCode("stcm.w\t07,--sp,cr0-cr7", "EDAD F2FF");
            AssertCode("stcm.w\t07,--sp,cr8-cr15", "EDAD F3FF");
        }

        [Test]
        public void Avr32Dis_stcond()
        {
            AssertCode("stcond\tsp[-32768],r2", "FB72 8000");
        }

        [Test]
        public void Avr32Dis_stdsp()
        {
            AssertCode("stdsp\tsp[256],r1", "5401");
        }

        [Test]
        public void Avr32Dis_sthh_w()
        {
            AssertCode("sthh.w\tsp[-512],r2:t,r3:t", "E5E3 F80D");
            AssertCode("sthh.w\tsp[r1<<3],r2:t,r3:t", "E5E3 B13D");
        }

        [Test]
        public void Avr32Dis_stm()
        {
            AssertCode("stm\t--sp,r0-r7,lr", "EBCD40FF");
            AssertCode("stm\t--sp,r6,lr", "EBCD4040");
        }

        [Test]
        public void Avr32Dis_stmts()
        {
            AssertCode("stmts\t--sp,r0-pc", "EFCD FFFF");
        }

        [Test]
        public void Avr32Dis_stswp_h()
        {
            AssertCode("stswp.h\tsp[-2048],r2", "FBD2 9800");
        }

        [Test]
        public void Avr32Dis_stswp_w()
        {
            AssertCode("stswp.w\tsp[-2048],r2", "FBD2 A800");
        }

        [Test]
        public void Avr32Dis_sub()
        {
            AssertCode("sub\tr1,r2", "0411");
            AssertCode("sub\tr1,r2,r3<<3", "E403 0131");
            AssertCode("sub\tsp,00000024", "209D");
            AssertCode("sub\tr12,00000069", "269C");
            AssertCode("sub\tr1,FFFF8000", "FE318000");
            AssertCode("sub\tr3,r2,FFFF8000", "E4C38000");

            AssertCode("sub\tr9,r7,r4", "EE040109 ");
        }

        [Test]
        public void Avr32Dis_sub_cond()
        {
            AssertCode("subcs\tr12,FFFFFFEA", "F7BC03EA");
            AssertCode("subcs\tr1,r2,r3", "E5D3E311");
        }

        [Test]
        public void Avr32Dis_subhh_w()
        {
            AssertCode("subhh.w\tr1,r2:t,r3:t", "E403 0F31");
        }

        [Test]
        public void Avr32Dis_swap_b()
        {
            AssertCode("swap.b\tr1", "5CB1");
        }

        [Test]
        public void Avr32Dis_swap_bh()
        {
            AssertCode("swap.bh\tr1", "5CC1");
        }

        [Test]
        public void Avr32Dis_swap_h()
        {
            AssertCode("swap.h\tr1", "5CA1");
        }

        [Test]
        public void Avr32Dis_sync()
        {
            AssertCode("sync\t80", "EBB0 0080");
        }

        [Test]
        public void Avr32Dis_tlbr()
        {
            AssertCode("tlbr", "D643");
        }

        [Test]
        public void Avr32Dis_tlbs()
        {
            AssertCode("tlbs", "D653");
        }

        [Test]
        public void Avr32Dis_tlbw()
        {
            AssertCode("tlbw", "D663");
        }

        [Test]
        public void Avr32Dis_tnbz()
        {
            AssertCode("tnbz\tr1", "5CE1");
        }

        [Test]
        public void Avr32Dis_tst()
        {
            AssertCode("tst\tr1,r2", "0471");
        }

        [Test]
        public void Avr32Dis_xchg()
        {
            AssertCode("xchg\tr1,r2,r3", "E403 0B41");
        }
    }
}