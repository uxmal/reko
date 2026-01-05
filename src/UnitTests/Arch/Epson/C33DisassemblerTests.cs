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
using Reko.Arch.Epson;
using Reko.Core;
using System;

namespace Reko.UnitTests.Arch.Epson;

[TestFixture]
public class C33DisassemblerTests : DisassemblerTestBase<C33Instruction>
{
    private readonly C33Architecture arch;
    private readonly Address addr;

    public C33DisassemblerTests() 
    {
        this.arch = new C33Architecture(CreateServiceContainer(), "c33", []);
        this.addr = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => this.arch;

    public override Address LoadAddress => this.addr;

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = base.DisassembleHexBytes(hexBytes);
        Assert.AreEqual(sExpected, instr.ToString());
    }

    [Test]
    public void C33Dasm_nop()
    {
        AssertCode("nop", "0000");
    }

    [Test]
    public void C33Dasm_add_imm()
    {
        AssertCode("add\t%r4,0x14", "4461");
    }

    [Test]
    public void C33Dasm_add_reg_ext()
    {
        AssertCode("add\t%r9,%r14,0x1044", "44D0 E922");
    }

    [Test]
    public void C33Dasm_add_sp_ext()
    {
        AssertCode("add\t%sp,0x10443A4", "44D0 E980");
    }

    [Test]
    public void C33Dasm_and_imm_ext()
    {
        AssertCode("and\t%r4,0xFFFD6AAA", "AAD5 A472");
    }

    [Test]
    public void C33Dasm_and_imm_ext_ext()
    {
        AssertCode("and\t%r4,0xFEDCBA98", "DBDF EAD2 8471");
    }

    [Test]
    public void C33Dasm_and_imm_ext_ext_ext()
    {
        AssertCode("Invalid", "FFDF DBDF EAD2 8471");
    }

    [Test]
    public void C33Dasm_call_disp()
    {
        AssertCode("call\t000FFFFE", "FF1C");
    }

    [Test]
    public void C33Dasm_call_disp_ext()
    {
        AssertCode("call\t00100000", "FFDF FE1C");
    }

    [Test]
    public void C33Dasm_ld_h()
    {
        AssertCode("ld.h\t%r1,[%r3]", "3128");
    }

    [Test]
    public void C33Dasm_ld_h_ext()
    {
        AssertCode("ld.h\t%r1,[%r3+0x1FFF]", "FFDF 3128");
    }

    [Test]
    public void C33Dasm_ld_h_sp()
    {
        AssertCode("ld.h\t%r1,[%sp+0x7E]", "F14B");
    }

    [Test]
    public void C33Dasm_ld_h_sp_ext()
    {
        AssertCode("ld.h\t%r1,[%sp+0x7FFFF]", "FFDF F14B");
    }

    [Test]
    public void C33Dasm_ld_w_sp()
    {
        AssertCode("ld.w\t%r1,[%sp+0xFC]", "F153");
    }

    [Test]
    public void C33Dasm_ld_w_sp_ext()
    {
        AssertCode("ld.w\t%r1,[%sp+0x7FFFF]", "FFDF F153");
    }
}
