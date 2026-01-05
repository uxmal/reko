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
using Reko.Arch.Oki;
using Reko.Arch.Oki.NX8_500;
using Reko.Core;

namespace Reko.UnitTests.Arch.Oki;

[TestFixture]
public class NX8_500DisassemblerTests : DisassemblerTestBase<NX8_500Instruction>
{
    private readonly NX8_500Architecture arch;

    public NX8_500DisassemblerTests()
    {
        this.arch = new NX8_500Architecture(CreateServiceContainer(), "nx8", new());
        this.LoadAddress = Address.Ptr16(0x0100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress { get; }

    private void AssertCode(string expected, string hex)
    {
        var instr = base.DisassembleHexBytes(hex);
        Assert.That(instr.ToString(), Is.EqualTo(expected));
    }

    [Test]
    public void Nx8Dasm_adc_Mx1()
    {
        AssertCode("adc\ta,[dsr:x1]", "A0F5");
    }

    [Test]
    public void Nx8Dasm_nop()
    {
        AssertCode("nop", "00");
    }

    [Test]
    public void Nx8Dasm_st_a_Mx1()
    {
        AssertCode("stb\ta,[dsr:x1]", "3D");
    }
}
