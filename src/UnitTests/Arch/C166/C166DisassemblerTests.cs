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
using Reko.Arch.C166;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.C166
{
    [TestFixture]
    public class C166DisassemblerTests : DisassemblerTestBase<C166Instruction>
    {
        private readonly C166Architecture arch;
        private readonly Address addr;

        public C166DisassemblerTests()
        {
            this.arch = new C166Architecture(CreateServiceContainer(), "c166", null);
            this.addr = Address.Ptr16(0x0100);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addr;

        private void AssertCode(string sExp, string hexBytes)
        {
            var instr = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void C166Dis()
        {
            var mem = new ByteMemoryArea(addr, new byte[0xF000]);
            var rnd = new Random(0x4711);
            rnd.NextBytes(mem.Bytes);
            foreach (var instr in arch.CreateDisassembler(mem.CreateLeReader(0)).Take(300))
                ;
        }

        [Test]
        public void C166Dis_add_mem_reg()
        {
            AssertCode("add\t[0x1234],[0xFEFE]", "047F3412");
        }

        [Test]
        public void C166Dis_add_reg_data16()
        {
            AssertCode("add\tDPP1,4242", "0601 4242");
        }

        [Test]
        public void C166Dis_add_reg_reg()
        {
            AssertCode("add\tr3,r6", "0036");
        }

        [Test]
        public void C166Dis_add_reg_regdeferred()
        {
            AssertCode("add\tr15,[r1]", "08F9");
        }

        [Test]
        public void C166Dis_add_reg_regdeferred_postinc()
        {
            AssertCode("add\tr15,[r1]", "08FD");
        }

        [Test]
        public void C166Dis_add_reg_short_imm()
        {
            AssertCode("add\tr7,04", "0874");
        }

        [Test]
        public void C166Dis_and()
        {
            AssertCode("and\tr10,r15", "60AF");
        }

        [Test]
        public void C166Dis_andb()
        {
            AssertCode("andb\trl1,[0x1234]", "63F2 3412");
        }

        [Test]
        public void C166Dis_ashr()
        {
            AssertCode("ashr\tr7,000A", "BCA7");
        }

        [Test]
        public void C166Dis_bclr()
        {
            AssertCode("bclr\t[0xFEF4]:12", "CE7A");
        }

        [Test]
        public void C166Dis_bcmp()
        {
            AssertCode("bcmp\t[0xFE68]:2,[0xFE2C]:1", "2A163412");
        }

        [Test]
        public void C166Dis_bfldl()
        {
            AssertCode("bfldl\t[0xFE24],34,56", "0A123456");
        }

        [Test]
        public void C166Dis_bmovn()
        {
            AssertCode("bmovn\t[0xFFBA]:4,r14:2", "3AFEDD24");
        }

        [Test]
        public void C166Dis_bset()
        {
            AssertCode("bset\tP1:13", "DF82");
        }

        [Test]
        public void C166Dis_bxor()
        {
            AssertCode("bxor\tr13:13,[0xFEE8]:14", "7A74FDED");
        }

        [Test]
        public void C166Dis_calla()
        {
            AssertCode("calla\tcc_NZ,1234", "CA30 3412");
        }

        [Test]
        public void C166Dis_calli()
        {
            AssertCode("calli\tcc_NV,[r2]", "AB52");
        }

        [Test]
        public void C166Dis_callr()
        {
            AssertCode("callr\t000A", "BB84");
        }

        [Test]
        public void C166Dis_calls()
        {
            AssertCode("calls\t01,1234", "DA013412");
        }

        [Test]
        public void C166Dis_cmpd1_mem()
        {
            AssertCode("cmpd1\tr2,[0x1234]", "A2F2 3412");
        }

        [Test]
        public void C166Dis_cmpd2()
        {
            AssertCode("cmpd2\tr10,0008", "B08A");
        }

        [Test]
        public void C166Dis_cmpi1()
        {
            AssertCode("cmpi1\tr3,000B", "80B3");
        }

        [Test]
        public void C166Dis_cmpi1_data16()
        {
            AssertCode("cmpi1\tr0,1234", "86F0 3412");
        }

        [Test]
        public void C166Dis_cmpi2_mm()
        {
            AssertCode("cmpi2\tr7,[0x4242]", "92F74242");
        }

        [Test]
        public void C166Dis_cpl()
        {
            AssertCode("cpl\tr11", "91B0");
        }

        [Test]
        public void C166Dis_diswdt()
        {
            AssertCode("diswdt", "A55AA5A5");
        }

        [Test]
        public void C166Dis_div()
        {
            AssertCode("div\tr1", "4B11");
        }

        [Test]
        public void C166Dis_divl()
        {
            AssertCode("divl\tr1", "6B11");
        }

        [Test]
        public void C166Dis_divu()
        {
            AssertCode("divu\tr3", "5B33");
        }

        [Test]
        public void C166Dis_einit()
        {
            AssertCode("einit", "B54AB5B5");
        }

        [Test]
        public void C166Dis_jmpa()
        {
            AssertCode("jmpa\tcc_Z,1234", "EA20 3412");
        }

        [Test]
        public void C166Dis_jmpi()
        {
            AssertCode("jmpi\tcc_SLT,[r4]", "9CC4");
        }

        [Test]
        public void C166Dis_jmpr_uc()
        {
            AssertCode("jmpr\tcc_UC,00F4", "0DF9");
        }

        [Test]
        public void C166Dis_jmpr()
        {
            AssertCode("jmpr\tcc_Z,01FE", "2D7E");
        }

        [Test]
        public void C166Dis_jmps()
        {
            AssertCode("jmps\t00,1234", "FA00 3412");
        }

        [Test]
        public void C166Dis_mov_ind_reg()
        {
            AssertCode("mov\t[r8],r15", "B8F8");
        }

        [Test]
        public void C166Dis_mov_ind_mem()
        {
            AssertCode("cmpd1\tr3,[0x1234]", "A2F33412");
        }

        [Test]
        public void C166Dis_mov_indreg_mem()
        {
            AssertCode("mov\t[r15],[0x1234]", "840F3412");
        }

        [Test]
        public void C166Dis_mov_predec()
        {
            AssertCode("mov\t[-r9],r10", "88A9");
        }

        [Test]
        public void C166Dis_mov_ind_postinc()
        {
            AssertCode("mov\t[r5],[r12+]", "E85C");
        }

        [Test]
        public void C166Dis_mov_reg_data16()
        {
            AssertCode("mov\tr4,1234", "E6F43412");
        }

        [Test]
        public void C166Dis_mov_reg_mdisp()
        {
            AssertCode("mov\tr4,[r2+0x1234]", "D4423412");
        }

        [Test]
        public void C166Dis_mov_reg_postinc()
        {
            AssertCode("mov\tr7,[r4+]", "9874");
        }

        [Test]
        public void C166Dis_movb_predec()
        {
            AssertCode("movb\t[-r12],rh4", "899C");
        }

        [Test]
        public void C166Dis_movb_postinc_ind()
        {
            AssertCode("movb\t[r11+],[r10]", "D9BA");
        }

        [Test]
        public void C166Dis_movbs_mem_reg()
        {
            AssertCode("movbs\t[0x1234],[0xFF16]", "D58B3412");
        }

        [Test]
        public void C166Dis_movbs_reg_mem()
        {
            AssertCode("movbs\tr6,[0x1234]", "D2F63412");
        }

        [Test]
        public void C166Dis_movbs_reg_reg()
        {
            AssertCode("movbs\tr3,rh0", "D013");
        }

        [Test]
        public void C166Dis_movbz_reg_reg()
        {
            AssertCode("movbz\tr13,rh2", "C05D");
        }

        [Test]
        public void C166Dis_mul()
        {
            AssertCode("mul\tr1,r13", "0B1D");
        }

        [Test]
        public void C166Dis_mulu()
        {
            AssertCode("mulu\tr14,r11", "1BEB");
        }

        [Test]
        public void C166Dis_neg()
        {
            AssertCode("neg\tr3", "8130");
            AssertCode("Invalid", "8131");
        }

        [Test]
        public void C166Dis_pcall()
        {
            AssertCode("pcall\tDP2,1234", "E2E13412");
        }

        [Test]
        public void C166Dis_pop_mem()
        {
            AssertCode("pop\tCC1IC", "FCBD");
        }

        [Test]
        public void C166Dis_prior()
        {
            AssertCode("prior\tr12,r13", "2BCD");
        }

        [Test]
        public void C166Dis_push_mem()
        {
            AssertCode("push\t[0xFEF6]", "EC7B");
        }

        [Test]
        public void C166Dis_ret()
        {
            AssertCode("ret", "CB00");
        }

        [Test]
        public void C166Dis_reti()
        {
            AssertCode("reti", "FB88");
        }

        [Test]
        public void C166Dis_retp()
        {
            AssertCode("retp\t[0xFE7C]", "EB3E");
        }

        [Test]
        public void C166Dis_rol()
        {
            AssertCode("rol\tr0,r15", "0C0F");
        }

        [Test]
        public void C166Dis_rol_data4()
        {
            AssertCode("rol\tr10,000A", "1CAA");
        }

        [Test]
        public void C166Dis_ror_imm()
        {
            AssertCode("ror\tr8,01", "3C18");
        }

        [Test]
        public void C166Dis_ror_reg()
        {
            AssertCode("ror\tr2,r0", "2C20");
        }

        [Test]
        public void C166Dis_scxt_imm()
        {
            AssertCode("scxt\tS1CON,1234", "C6DC 3412");
        }

        [Test]
        public void C166Dis_scxt_mem()
        {
            AssertCode("scxt\tr7,[0x1234]", "D6F7 3412");
        }

        [Test]
        public void C166Dis_shl_imm()
        {
            AssertCode("shl\tr15,05", "5C5F");
        }

        [Test]
        public void C166Dis_shr_reg_reg()
        {
            AssertCode("shr\tr14,r15", "6CEF");
        }

        [Test]
        public void C166Dis_srvwdt()
        {
            AssertCode("srvwdt", "A758A7A7");
        }

        [Test]
        public void C166Dis_subc()
        {
            AssertCode("subc\tr8,r9", "3089");
        }

        [Test]
        public void C166Dis_trap()
        {
            AssertCode("trap\t07", "9B0E");
        }

        [Test]
        public void C166Dis_xor()
        {
            AssertCode("xor\t[0x1234],[0xFE7C]", "543E 3412");
        }

        [Test]
        public void C166Dis_xorb_imm()
        {
            AssertCode("xorb\t[0xFE70],0F", "5738 0F42");
        }
    }
}
