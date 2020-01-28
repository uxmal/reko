#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using Reko.Arch.Tlcs;
using Reko.Arch.Tlcs.Tlcs900;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Tlcs
{
    [TestFixture]
    public class Tlcs900DisassemblerTests : DisassemblerTestBase<Tlcs900Instruction>
    {
        private readonly Tlcs900Architecture arch;

        public Tlcs900DisassemblerTests()
        {
            this.Architecture = new Tlcs900Architecture("tlcs900");
            this.LoadAddress = Address.Ptr32(0x00010000);
        }

        public override IProcessorArchitecture Architecture { get; }

        public override Address LoadAddress { get; }

        private void AssertCode(string sExp, string hexBytes)
        {
            var i = DisassembleHexBytes(hexBytes);
            Assert.AreEqual(sExp, i.ToString());
        }

        [Test]
        public void Tlcs900_dis_nop()
        {
            AssertCode("nop", "00");
        }

        [Test]
        public void Tlcs900_dis_push_RR()
        {
            AssertCode("push\twa", "28");
        }

        [Test]
        public void Tlcs900_dis_ld_reg_byte()
        {
            AssertCode("ld\ta,b", "CA89");
        }

        [Test]
        public void Tlcs900_dis_add_reg_indirect_word()
        {
            AssertCode("add\tbc,(xhl)", "9381");
        }

        [Test]
        public void Tlcs900_dis_sub_reg_indexed_8()
        {
            AssertCode("sub\tde,(xsp-0x04)", "9FFCA2");
        }

        [Test]
        public void Tlcs900_dis_xor_reg_indexed_16()
        {
            AssertCode("xor\tde,(xsp-0x04)", "D31DFCFFD2");
        }

        [Test]
        public void Tlcs900_dis_inc_reg()
        {
            AssertCode("inc\t00000004,xbc", "E964");
        }

        [Test]
        public void Tlcs900_dis_inc_predec()
        {
            AssertCode("inc\t00000001,(4:-xde)", "E40A61");
        }

        [Test]
        public void Tlcs900_dis_ld_absolute()
        {
            AssertCode("ld\txbc,(0000069C)", "E29C060021");
        }

        [Test]
        public void Tlcs900_dis_store_to_mem()
        {
            AssertCode("ld\t(xbc+0x26),xwa", "B92660");
        }

        [Test]
        public void Tlcs900_dis_lda()
        {
            AssertCode("lda\tix,(xbc+0x26)", "B92624");
            AssertCode("lda\txiz,(xbc+0x26)", "B92636");
        }

        [Test]
        public void Tlcs900_dis_ext()
        {
            AssertCode("extz\thl", "DB12");
            AssertCode("exts\txde", "EA13");
        }

        [Test]
        public void Tlcs900_dis_jr()
        {
            AssertCode("jr\tZ,00010030", "662E");
            AssertCode("jr\t0000FF00", "78FDFE");
        }

        [Test]
        public void Tlcs900_dis_push_pop_sr()
        {
            AssertCode("push\tsr", "02");
            AssertCode("pop\tsr", "03");
        }

        [Test]
        public void Tlcs900_dis_push_ei_nn()
        {
            AssertCode("ei\t05", "0605");
        }

        [Test]
        public void Tlcs900_dis_jp_abs()
        {
            AssertCode("jp\t00001234", "1A3412");
            AssertCode("jp\t00123456", "1B563412");
        }

        [Test]
        public void Tlcs900_dis_call_abs()
        {
            AssertCode("call\t00001234", "1C3412");
            AssertCode("call\t00123456", "1D563412");
        }

        [Test]
        public void Tlcs900_dis_calr()
        {
            AssertCode("calr\t00010200", "1EFD01");
        }

        [Test]
        public void Tlcs900_dis_swi()
        {
            AssertCode("swi\t05", "FD");
        }

        [Test]
        public void Tlcs900_dis_xor_imm()
        {
            AssertCode("xor\tiz,1234", "DECD3412");
        }

        [Test]
        public void Tlcs900_dis_cp_imm()
        {
            AssertCode("cp\tiz,0005", "DEDD");
        }

        [Test]
        public void Tlcs900_dis_cp()
        {
            AssertCode("cp\te,(xsp-0x3E)", "8FC2F5");
        }

        [Test]
        public void Tlcs900_dis_rlc()
        {
            AssertCode("rlc\t04,e", "CDE804");
            AssertCode("rlc\ta,e", "CDF8");
        }

        [Test]
        public void Tlcs900_dis_ld_reg()
        {
            AssertCode("ld\txbc,xiy", "ED89");
            AssertCode("ld\txiy,xbc", "ED99");
        }

        [Test]
        public void Tlcs900_dis_scc()
        {
            AssertCode("scc\tGE,iy", "DD79");
        }

        [Test]
        public void Tlcs900_dis_jp_cc()
        {
            AssertCode("jp\tGE,(xwa)", "B0D9");
        }

        [Test]
        public void Tlcs900_dis_call_cc()
        {
            AssertCode("call\tGE,(xbc+0x14)", "B914E9");
        }

        [Test]
        public void Tlcs900_dis_ret_cc()
        {
            AssertCode("ret\tGE", "B0F9");
        }

        [Test]
        public void Tlcs900_dis_ret()
        {
            AssertCode("ret", "B0F8");
        }

        [Test]
        public void Tlcs900_dis_sll_mem()
        {
            AssertCode("sll\t(xwa+0x0C)", "A80C7E");
        }

        [Test]
        public void Tlcs900_dis_pop_mem()
        {
            AssertCode("pop\t(xde+0x7C)", "BA7C06");
        }

        [Test]
        public void Tlcs900_dis_link()
        {
            AssertCode("link\txiy,0124", "ED0C2401");
        }

        [Test]
        public void Tlcs900_dis_unlk()
        {
            AssertCode("unlk\txiy", "ED0D");
        }

        [Test]
        public void Tlcs900_dis_cpl()
        {
            AssertCode("cpl\thl", "DB06");
        }

        [Test]
        public void Tlcs900_dis_post_inc()
        {
            AssertCode("ld\twa,(xhl+:2)", "D50D20");
        }

        [Test]
        public void Tlcs900_dis_post_inc_odd()
        {
            //$REVIEW: increment and data size don't match. Is this OK?
            AssertCode("ld\ta,(xhl+:4)", "C50E21");
        }

        [Test]
        public void Tlcs900_dis_djnz()
        {
            AssertCode("djnz\tbc,0000FFF0", "D91CED");
        }

        [Test]
        public void Tlcs900_dis_daa()
        {
            AssertCode("daa\tb", "CA10");
        }

        [Test]
        public void Tlcs900_dis_paa()
        {
            AssertCode("paa\txde", "EA14");
        }

        [Test]
        public void Tlcs900_dis_ld_reg_imm()
        {
            AssertCode("ld\txde,12345678", "EA0378563412");
        }

        [Test]
        public void Tlcs900_dis_pop_r()
        {
            AssertCode("pop\tl", "CF05");
        }

        [Test]
        public void Tlcs900_dis_bs1x()
        {
            AssertCode("bs1f\ta,hl", "DB0E");
            AssertCode("bs1b\ta,hl", "DB0F");
        }

        [Test]
        public void Tlcs900_dis_push_n()
        {
            AssertCode("push\t0E", "090E");
            AssertCode("push\t1234", "0B3412");
        }

        [Test]
        public void Tlcs900_dis_ldir()
        {
            AssertCode("ldirw", "9311");
            AssertCode("ldir",  "8311");
        }

        [Test]
        public void Tlcs900_dis_muls()
        {
            AssertCode("muls\tde,bc","D94A");
        }

        [Test]
        public void Tlcs900_dis_ld_mem_imm()
        {
            AssertCode("ld\t(00006DA0),00", "﻿F1A06D0000");
        }

        [Test]
        public void Tlcs900_dis_ld_d_i3()
        {
            AssertCode("ld\td,04", "﻿CCAC");
        }
    }
}
