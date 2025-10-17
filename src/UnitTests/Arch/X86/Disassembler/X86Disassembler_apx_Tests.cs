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
using Reko.Arch.X86;
using Reko.Core;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.X86.Disassembler;

[TestFixture]
public class X86Disassembler_apx_Tests : DisassemblerTestBase<X86Instruction>
{
    private readonly X86ArchitectureFlat64 arch;
    private readonly Address addr;

    public X86Disassembler_apx_Tests()
    {
        this.addr = Address.Ptr64(0x10000);
        var options = new Dictionary<string, object>
        {
            { "apx", true }
        };
        this.arch = new X86ArchitectureFlat64(CreateServiceContainer(), "x86-flat-64", options);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    private void AssertCode(string sExpected, string sHexBytes)
    {
        var instr = DisassembleHexBytes(sHexBytes);
        Assert.AreEqual(sExpected, instr.ToString());
    }

    [Test]
    public void X86Dis_rex2_add()
    {
        AssertCode("add\trax,rsp", "D50803C4");
        AssertCode("add\tr8,rsp",  "D50C03C4");
    }

    [Test]
    public void X86Dis_rex2_add_sib()
    {
        AssertCode("add\tr8,[r14+r11*8]",  "D50F0304DE");
        AssertCode("add\tr24,[r30+r27*8]", "D57F0304DE");
    }

    [Test]
    public void X86Dis_rex2_illegal_opcodes()
    {
        AssertCode("illegal", "D508 40 03C4");
        AssertCode("illegal", "D508 72 45");
        AssertCode("illegal", "D508 A0 42");
        AssertCode("illegal", "40 D50803C4");   // REX prefix before REX2
    }
}

