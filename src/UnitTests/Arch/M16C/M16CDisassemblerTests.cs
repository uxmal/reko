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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reko.Arch.M16C;
using Reko.Core;

namespace Reko.UnitTests.Arch.M16C;

[TestFixture]
public class M16CDisassemblerTests : DisassemblerTestBase<M16CInstruction>
{
    private readonly M16CArchitecture arch;
    private readonly Address addr;

    public M16CDisassemblerTests()
    {
        this.arch = new M16CArchitecture(CreateServiceContainer(), "m16c", new());
        this.addr = Address.Ptr16(0x100);
        Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    private void AssertCode(string sExpected, string sHexBytes)
    {
        var instr = base.DisassembleHexBytes(sHexBytes);
        Assert.AreEqual(sExpected, instr.ToString());
    }

    [Test]
    public void M16cDis_abs_b_r0l()
    {
        AssertCode("abs.b\tr0l", "76F0");
    }

    [Test]
    public void M16cDis_abs_w_r1()
    {
        AssertCode("abs.w\tr1", "77F1");
    }

    [Test]
    public void M16cDis_adc_b_imm_r1l()
    {
        AssertCode("adc.b\t#42h,r1l", "766242");
    }

    [Test]
    public void M16cDis_adc_w_src_dst()
    {
        AssertCode("adc.w\tr3,a0", "B134");
    }

    [Test]
    public void M16cDis_adcf_mem_a0()
    {
        AssertCode("adcf.b\t[a0]", "76E6");
    }

    [Test]
    public void M16cDis_add_w_imm_dsp8()
    {
        AssertCode("add.w\t#1234h,80h[a0]", "774880 3412");
    }

    [Test]
    public void M16cDis_add_b_q_imm_dsp8()
    {
        AssertCode("add.b:q\t#4h,0FCh[sb]", "C84AFC");
    }

    [Test]
    public void M16cDis_add_b_s_imm()
    {
        AssertCode("add.b:s\t#42h,r0h", "8342");
        AssertCode("add.b:s\t#42h,r0l", "8442");
        AssertCode("add.b:s\t#42h,0FCh[sb]", "8542FC");
        AssertCode("add.b:s\t#42h,-4h[fb]", "8642FC");
        AssertCode("add.b:s\t#42h,[1234h]", "87423412");
    }

    [Test]
    public void M16dasm_add_b_g()
    {
        AssertCode("add.b\t1234[a0],5678[a1]", "A0CD34127856");
    }

    [Test]
    public void M16dasm_add_b_s_reg()
    {
        AssertCode("add.b:s\t", "20");
        AssertCode("add.b:s\t", "24");
    }

    [Test]
    public void M16cDasm_add_imm_sp()
    {
        AssertCode("add.b\t#-80h,usp", "7CEB80");
        AssertCode("add.w\t#-8000h,usp", "7DEB0080");
    }

    [Test]
    public void M16cDasm_add_q_imm_sp()
    {
        AssertCode("add:q\t#-8h,usp", "7DB8");
    }

    [Test]
    public void M16cDasm_adjnz()
    {
        AssertCode("adjnz.w:q\t#-8h,80h[a1],0082", "F9898080");
    }

    [Test]
    public void M16cDasm_and_imm()
    {
        AssertCode("and.w\t#0EFFEh,-80h[fb]", "772B80FEEF");
    }

    [Test]
    public void M16cDasm_and_b_s()
    {
        AssertCode("and.b:s\t#42h,r0h", "9342");
        AssertCode("and.b:s\t#42h,[1234h]", "97423412");
    }

    [Test]
    public void M16cDasm_and()
    {
        AssertCode("and.b\tr1h,a0", "9034");
        AssertCode("and.w\tr3,a0", "9134");
    }

    [Test]
    public void M16cDasm_and_b_s_reg()
    {
        AssertCode("and.b:s\tr0h,r0l", "10");
        AssertCode("and.b:s\tr0l,r0h", "14");
    }

    [Test]
    public void M16cDasm_band()
    {
        AssertCode("band\t[a1]", "7E47");
    }

    [Test]
    public void M16cDasm_bclr()
    {
        AssertCode("bclr\t[a0]", "7E86");
    }

    [TestCase("7E2000FA", "bmne\t#0h,r0")]
    [TestCase("7E210106", "bmge\t#1h,r1")]
    [TestCase("7E230300", "bmgeu\t#3h,r3")]
    [TestCase("7E2603", "bmn\t[a0]")]
    [TestCase("7E2F341204", "bmle\t[1234h]")]
    public void M16cDasm_bmcnd(string hexBytes, string expected)
    {
        AssertCode(expected, hexBytes);
    }

    [Test]
    public void M16cDasm_bmcnd_c()
    {
        AssertCode("bmgt\tC", "7DDC");
    }

    [Test]
    public void M16cDasm_bnot_s()
    {
        AssertCode("bnot:s\t#7h,80h[sb]", "5780");
    }

    [Test]
    public void M16cDasm_brk()
    {
        AssertCode("brk", "00");
    }

    [Test]
    public void M16cDasm_bset()
    {
        AssertCode("bset\t[1234h]", "7E9F3412");
    }

    [Test]
    public void M16Dasm_cmp()
    {
        AssertCode("cmp.w\t#1h,r0", "C042");

    }

    [Test]
    public void M16Dasm_cmp_q_imm()
    {
        AssertCode("cmp.w:q\t#1h,r0", "D110");
    }

    [Test]
    public void M16Dasm_cmp_imm()
    {
        AssertCode("cmp.w:q\t#41h,[043Ch]", "778F3C0441");
    }

    [Test]
    public void M16Dasm_cmp_b_s_imm()
    {
        AssertCode("cmp.b:s\t#41h,[043Ch]", "E380");
    }

    [Test]
    public void M16cDasm_enter()
    {
        AssertCode("enter\t#80h", "7CF280");
    }

    [Test]
    public void M16cDasm_exitd()
    {
        AssertCode("exitd", "7DF2");
    }

    [Test]
    public void M16cDasm_fset()
    {
        AssertCode("fset\tZ", "EB24");
    }

    [Test]
    public void M16cDasm_jeq()
    {
        AssertCode("jeq\t0100", "6AFF");
    }

    [Test]
    public void M16cDasm_jle()
    {
        AssertCode("jle\t0100", "7DC8FE");
    }

    [Test]
    public void M16cDasm_jmp_w()
    {
        AssertCode("jmp.w\t0100", "F4FFFF");
    }

    [Test]
    public void M16cDasm_jsr_a()
    {
        AssertCode("jsr.a\t000C17B4", "FDB4170C7D");
    }

    [Test]
    public void M16cDasm_jsr_w()
    {
        AssertCode("jsr.w\t0100", "F5FFFF");
    }

    [TestCase("EB003412", "Invalid")]
    [TestCase("EB103412", "ldc\t#1234h,intbl")]
    [TestCase("EB403412", "ldc\t#1234h,isp")]
    [TestCase("EB703412", "ldc\t#1234h,fb")]
    public void M16cDasm_ldc_imm(string hexbytes, string expected)
    {
        AssertCode(expected, hexbytes);
    }

    [Test]
    public void M16cDasm_lde()
    {
        AssertCode("lde.w\t[a1a0],r2", "75A2");
    }

    [Test]
    public void M16cDasm_mov()
    {
        AssertCode("mov.b\tr0l,r0h", "7201");
        AssertCode("mov.w\tr0,r1", "7301");
    }

    [Test]
    public void M16cDasm_mov_b_s()
    {
        AssertCode("mov.b:s\t#42h,r0h", "C342");
        AssertCode("mov.b:s\t#42h,r0l", "C442");
        AssertCode("mov.b:s\t#42h,[1234h]", "C7423412");
    }

    [Test]
    public void M16cDasm_mov_b_z()
    {
        AssertCode("mov.b:z\t#0h,r0l", "B4");
        AssertCode("mov.b:z\t#0h,[1234h]", "B73412");
    }

    [Test]
    public void M16cDasm_mov_s_imm()
    {
        AssertCode("mov.w:s\t#1234h,a0", "A23412");
        AssertCode("mov.w:s\t#1234h,a1", "AA3412");
        AssertCode("mov.b:s\t#80h,a0", "E280");
        AssertCode("mov.b:s\t#80h,a1", "EA80");
    }

    [Test]
    public void M16cDasm_mov_s_src_dst()
    {
        AssertCode("mov.b:s\t[1234h],a0", "333412");
    }

    [Test]
    public void M16cDasm_mov_s_r0x_dst()
    {
        AssertCode("mov.b:s\tr0l,80h[sb]", "0180");
        AssertCode("mov.b:s\tr0l,-80h[fb]", "0280");
        AssertCode("mov.b:s\tr0l,[1234h]", "033412");
        AssertCode("mov.b:s\tr0h,80h[sb]", "0580");
        AssertCode("mov.b:s\tr0h,-80h[fb]", "0680");
        AssertCode("mov.b:s\tr0h,[1234h]", "073412");
    }

    [Test]
    public void M16cDasm_mov_s_src_r0x()
    {
        AssertCode("mov.b:s\t80h[sb],r0l", "0980");
        AssertCode("mov.b:s\t-80h[fb],r0l", "0A80");
        AssertCode("mov.b:s\t[1234h],r0l", "0B3412");
        AssertCode("mov.b:s\t80h[sb],r0h", "0D80");
        AssertCode("mov.b:s\t-80h[fb],r0h", "0E80");
        AssertCode("mov.b:s\t[1234h],r0h", "0F3412");
    }

    [Test]
    public void M16cDasm_mov_w()
    {
        AssertCode("mov.w\t#55h,r1", "75C15500");
    }

    [Test]
    public void M16cDasm_mov_w_q()
    {
        AssertCode("mov.w:q\t#5h,r2", "D952");
    }


    [TestCase("7C500A", "mul.b\t#0Ah,r0l")]
    [TestCase("7C510A", "Invalid")]
    [TestCase("7C520A", "mul.b\t#0Ah,r1l")]
    [TestCase("7C530A", "Invalid")]
    public void M16cDasm_mul_b_imm(string hexBytes, string sExpected)
    {
        AssertCode(sExpected, hexBytes);
    }


    [Test]
    public void M16cDasm_or_imm()
    {
        AssertCode("or.b\t#42h,a1", "763542");
        AssertCode("or.w\t#1234h,a1", "77353412");
    }

    [Test]
    public void M16cDasm_pop()
    {
        AssertCode("pop.b\tr0h", "74D1");
        AssertCode("pop.w\tr2", "75D2");
    }

    [Test]
    public void M16cDasm_pop_b_s()
    {
        AssertCode("pop.b:s\tr0l", "92");
        AssertCode("pop.b:s\tr0h", "9A");
    }

    [Test]
    public void M16cDasm_pop_w_s()
    {
        AssertCode("pop.w:s\ta0", "D2");
        AssertCode("pop.w:s\ta1", "DA");
    }

    [Test]
    public void M16cDasm_popm()
    {
        AssertCode("popm\ta0,r2,r0", "ED15");
    }

    [Test]
    public void M16cDasm_push()
    {
        AssertCode("push.b\tr0h", "7441");
        AssertCode("push.w\tr2", "7542");
    }

    [Test]
    public void M16cDasm_push_b_s()
    {
        AssertCode("push.b:s\tr0l", "82");
        AssertCode("push.b:s\tr0h", "8A");
    }

    [Test]
    public void M16cDasm_push_w_s()
    {
        AssertCode("push.w:s\ta0", "C2");
        AssertCode("push.w:s\ta1", "CA");
    }

    [Test]
    public void M16cDasm_pushm()
    {
        AssertCode("pushm\tfb,a1,r3", "EC15");
    }

    [Test]
    public void M16cDasm_rolc()
    {
        AssertCode("rolc.w\ta0", "77A4E983");
    }

    [TestCase("7461", "rot.b\tr1h,r0h")]
    [TestCase("7561", "Invalid")]
    [TestCase("7462", "rot.b\tr1h,r1l")]
    [TestCase("7463", "Invalid")]
    [TestCase("7563", "rot.w\tr1h,r3")]
    public void M16cDasm_rot(string hexBytes, string expected)
    {
        AssertCode(expected, hexBytes);
    }

    [Test]
    public void M16cDasm_shl_l()
    {
        AssertCode("shl.l\t#2h,r2r0", "EB82");
        AssertCode("shl.l\t#2h,r3r1", "EB92");
    }


    [Test]
    public void M16cDasm_ste()
    {
        AssertCode("ste.w\t80h[a0],[12345h]", "750880452301");
    }

    [Test]
    public void M16cDasm_stzx()
    {
        AssertCode("stzx\t#41h,#42h,[43Ch]", "DF413C0442");
    }
}
