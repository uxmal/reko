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
using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;

namespace Reko.UnitTests.Arch.Mos6502
{
    [TestFixture]
    public class Mos65816DisassemblerTests : DisassemblerTestBase<Instruction>
    {
        private readonly Address addrLoad;
        private readonly Mos65816Architecture arch;

        public Mos65816DisassemblerTests()
        {
            this.addrLoad = Address.Ptr16(0x200);
            this.arch = new Mos65816Architecture(CreateServiceContainer(), "mos65816", new());
        }

        public override IProcessorArchitecture Architecture => arch;

        public override Address LoadAddress => addrLoad;

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExpected, instr.ToString());
        }

        public void Mos65816Dis_Fuzz()
        {
            for (int i = 0; i < 2; ++i)
            {
                var rnd = new Random(0x4711 + i);
                var mem = new ByteMemoryArea(addrLoad, new byte[0xC000]);
                rnd.NextBytes(mem.Bytes);
                var rdr = mem.CreateLeReader(0);
                var dasm = arch.CreateDisassembler(rdr);
                var svc = arch.Services.GetService<ITestGenerationService>();
                bool fail = false;
                foreach (var instr in dasm)
                {
                    try
                    {
                        instr.ToString();
                    }
                    catch
                    {
                        fail = true;
                        svc?.ReportMissingDecoder("Mos65816", instr.Address, rdr, instr.MnemonicAsString);
                    }
                }
                Assert.IsFalse(fail);
            }
        }

        [Test]
        public void Mos65816_adc_directPage()
        {
            AssertCode("adc\t#$19", "6919");
        }

        [Test]
        public void Mos65816_adc_stack_relative()
        {
            AssertCode("adc\t$94,s", "6394");
        }

        [Test]
        public void Mos65816_adc_direct()
        {
            AssertCode("adc\t$F9E1", "6DE1F9");
        }

        [Test]
        public void Mos65816_adc_indexed_y()
        {
            AssertCode("adc\t$8850,y", "795088");
        }

        [Test]
        public void Mos65816_adc_indirectlong()
        {
            AssertCode("adc\t[$6F]", "676F");
        }

        [Test]
        public void Mos65816_and()
        {
            AssertCode("and\t$36,s", "2336");
        }

        [Test]
        public void Mos65816_and_longindirect()
        {
            AssertCode("and\t[$E3]", "27E3");
        }

        [Test]
        public void Mos65816_asl()
        {
            AssertCode("asl", "0A");
        }

        [Test]
        public void Mos65816_bcs()
        {
            AssertCode("bcs\t#$239", "B037");
        }

        [Test]
        public void Mos65816_bit()
        {
            AssertCode("bit\t#$78", "8978");
        }

        [Test]
        public void Mos65816_bmi()
        {
            AssertCode("bmi\t#$18F", "308D");
        }

        [Test]
        public void Mos65816_bne()
        {
            AssertCode("bne\t#$1A8", "D0A6");
        }

        [Test]
        public void Mos65816_brk()
        {
            AssertCode("brk", "00");
        }

        [Test]
        public void Mos65816_blr()
        {
            AssertCode("brl\t#$420A", "820740");
        }

        [Test]
        public void Mos65816_bvc()
        {
            AssertCode("bvc\t#$1CD", "50CB");
        }

        [Test]
        public void Mos65816_clc()
        {
            AssertCode("clc", "18");
        }

        [Test]
        public void Mos65816_cld()
        {
            AssertCode("cld", "D8");
        }

        [Test]
        public void Mos65816_cli()
        {
            AssertCode("cli", "58");
        }

        [Test]
        public void Mos65816_clv()
        {
            AssertCode("clv", "B8");
        }

        [Test]
        public void Mos65816_cmp()
        {
            AssertCode("cmp\t$82C6", "CDC682");
        }

        [Test]
        public void Mos65816_cmp_absolute_long()
        {
            AssertCode("cmp\t$535B53", "CF535B53");
        }

        [Test]
        public void Mos65816_cmp_absolute_long_x()
        {
            AssertCode("cmp\t$E1E282,x", "DF82E2E1");
        }

        [Test]
        public void Mos65816_cmp_index()
        {
            AssertCode("cmp\t($1F,x)", "C11F");
        }

        [Test]
        public void Mos65816_cmp_stack_index()
        {
            AssertCode("cmp\t($5A,s),y", "D35A");
        }

        [Test]
        public void Mos65816_cpx()
        {
            AssertCode("cpx\t#$21", "E021");
        }

        [Test]
        public void Mos65816_dec()
        {
            AssertCode("dec\t$F8B8,x", "DEB8F8");
        }

        [Test]
        public void Mos65816_dey()
        {
            AssertCode("dey", "88");
        }

        [Test]
        public void Mos65816_eor()
        {
            AssertCode("eor\t$3F147E", "4F7E143F");
        }

        [Test]
        public void Mos65816_eor_indexindirect()
        {
            AssertCode("eor\t($74,x)", "4174");
        }

        [Test]
        public void Mos65816_inc()
        {
            AssertCode("inc", "1A");
        }

        [Test]
        public void Mos65816_inc_dirpg()
        {
            AssertCode("inc\t$14", "E614");
        }

        [Test]
        public void Mos65816_iny()
        {
            AssertCode("iny", "C8");
        }

        [Test]
        public void Mos65816_jmp()
        {
            AssertCode("jmp\t$D0C5", "4CC5D0");
        }

        [Test]
        public void Mos65816_jmp_indirect()
        {
            AssertCode("jmp\t($BA01)", "6C01BA");
        }

        [Test]
        public void Mos65816_jmp_indirect_x()
        {
            AssertCode("jmp\t($3043,x)", "DC4330");
        }

        [Test]
        public void Mos65816_jsr_absidxind()
        {
            AssertCode("jsr\t($BD1B,x)", "FC1BBD");
        }

        [Test]
        public void Mos65816_jsr_indx()
        {
            AssertCode("jsr\t($BAAF,x)", "FCAFBA");
        }

        [Test]
        public void Mos65816_jsr_long()
        {
            AssertCode("jsr\t$C186DD", "22DD86C1");
        }

        [Test]
        public void Mos65816_lda_directPageIndexedIndirectX()
        {
            AssertCode("lda\t($E5,x)", "A1E5");
        }

        [Test]
        public void Mos65816_ldx_indexed()
        {
            AssertCode("ldx\t$40E2,y", "BEE240");
        }

        [Test]
        public void Mos65816_ldy()
        {
            AssertCode("ldy\t$E68F", "AC8FE6");
        }

        [Test]
        public void Mos65816_lsr()
        {
            AssertCode("lsr", "4A");
        }

        [Test]
        public void Mos65816_mvn()
        {
            AssertCode("mvn\t#$5D,#$29", "545D29");
        }

        [Test]
        public void Mos65816_nop()
        {
            AssertCode("nop", "EA");
        }

        [Test]
        public void Mos65816_ora()
        {
            AssertCode("ora\t($63),y", "1163");
        }

        [Test]
        public void Mos65816_ora_direct_page()
        {
            AssertCode("ora\t$C5", "05C5");
        }

        [Test]
        public void Mos65816_ora_direct_page_indirect()
        {
            AssertCode("ora\t($4C)", "124C");
        }

        [Test]
        public void Mos65816_ora_imm()
        {
            AssertCode("ora\t#$77", "0977");
        }

        [Test]
        public void Mos65816_ora_long()
        {
            AssertCode("ora\t$49114B", "0F4B1149");
        }

        [Test]
        public void Mos65816_ora_stack_relative()
        {
            AssertCode("ora\t$E0,s", "03E0");
        }

        [Test]
        public void Mos65816_phb()
        {
            AssertCode("phb", "8B");
        }

        [Test]
        public void Mos65816_phk()
        {
            AssertCode("phk", "4B");
        }

        [Test]
        public void Mos65816_plb()
        {
            AssertCode("plb", "AB");
        }

        [Test]
        public void Mos65816_pld()
        {
            AssertCode("pld", "2B");
        }

        [Test]
        public void Mos65816_plp()
        {
            AssertCode("plp", "28");
        }

        [Test]
        public void Mos65816_plx()
        {
            AssertCode("plx", "FA");
        }

        [Test]
        public void Mos65816_rep()
        {
            AssertCode("rep\t#$FE", "C2FE");
        }

        [Test]
        public void Mos65816_rol()
        {
            AssertCode("rol", "2A");
        }

        [Test]
        public void Mos65816_rol_directPage()
        {
            AssertCode("rol\t$93", "2693");
        }

        [Test]
        public void Mos65816_ror()
        {
            AssertCode("ror", "6A");
        }

        [Test]
        public void Mos65816_ror_direct_page()
        {
            AssertCode("ror\t$87", "6687");
        }

        [Test]
        public void Mos65816_rts()
        {
            AssertCode("rts", "60");
        }

        [Test]
        public void Mos65816_sbc_abslongx()
        {
            AssertCode("sbc\t$D3727F,x", "FF7F72D3");
        }

        [Test]
        public void Mos65816_sbc_s()
        {
            AssertCode("sbc\t$27,s", "E327");
        }

        [Test]
        public void Mos65816_sei()
        {
            AssertCode("sei", "78");
        }

        [Test]
        public void Mos65816_sep()
        {
            AssertCode("sep\t#$48", "E248");
        }

        [Test]
        public void Mos65816_sta()
        {
            AssertCode("sta\t$F2,x", "95F2");
        }

        [Test]
        public void Mos65816_stx()
        {
            AssertCode("stx\t$C102", "8E02C1");
        }

        [Test]
        public void Mos65816_stx_dpy()
        {
            AssertCode("stx\t$88,y", "9688");
        }

        [Test]
        public void Mos65816_sty()
        {
            AssertCode("sty\t$3C", "843C");
        }

        [Test]
        public void Mos65816_stz()
        {
            AssertCode("stz\t$DEA0", "9CA0DE");
        }

        [Test]
        public void Mos65816_stz_dpx()
        {
            AssertCode("stz\t$6C,x", "746C");
        }

        [Test]
        public void Mos65816_tay()
        {
            AssertCode("tay", "A8");
        }

        [Test]
        public void Mos65816_tcd()
        {
            AssertCode("tcd", "5B");
        }

        [Test]
        public void Mos65816_tcs()
        {
            AssertCode("tcs", "1B");
        }

        [Test]
        public void Mos65816_tsc()
        {
            AssertCode("tsc", "3B");
        }

        [Test]
        public void Mos65816_txy()
        {
            AssertCode("txy", "9B");
        }

        [Test]
        public void Mos65816_tyx()
        {
            AssertCode("tyx", "BB");
        }

        [Test]
        public void Mos65816_wai()
        {
            AssertCode("wai", "CB");
        }

        [Test]
        public void Mos65816_wdm()
        {
            AssertCode("wdm", "42");
        }

        [Test]
        public void Mos65816_xba()
        {
            AssertCode("xba", "EB");
        }

        [Test]
        public void Mos65816_xce()
        {
            AssertCode("xce", "FB");
        }
    }
}
