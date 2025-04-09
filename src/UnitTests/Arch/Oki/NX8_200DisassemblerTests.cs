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
using Reko.Arch.Oki;
using Reko.Arch.Oki.NX8_200;
using Reko.Core;

namespace Reko.UnitTests.Arch.Oki;

public class NX8_200DisassemblerTests : DisassemblerTestBase<NX8_200Instruction>
{
    private readonly NX8_200Architecture arch;

    public NX8_200DisassemblerTests()
    {
        this.arch = new NX8_200Architecture(CreateServiceContainer(), "nx8/200", []);
        this.LoadAddress = Address.Ptr16(0x1000);
    }

    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress { get; }

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(sExpected));
    }

    [Test]
    public void NX8_200Dasm_adcb_a_pswh()
    {
        AssertCode("adcb\ta,pswh", "A292");
    }

    [Test]
    public void NX8_200Dasm_addb_a_pswh()
    {
        AssertCode("addb\ta,pswh", "A282");
    }

    [Test]
    public void NX8_200Dasm_addb_pswh_a()
    {
        AssertCode("addb\tpswh,a", "A281");
    }

    [Test]
    public void NX8_200Dasm_rb_n8()
    {
        AssertCode("rb\t09Eh.0", "C59E08");
    }

    [Test]
    public void NX8_200Dasm_clr_n8()
    {
        AssertCode("clr\t0D4h", "C5D415");
    }

    [Test]
    public void NX8_200Dasm_mb_n8()
    {
        AssertCode("mb\tC,02Ch.4", "C52C2C");
    }

    [Test]
    public void NX8_200Dasm_mov_ssp_imm()
    {
        AssertCode("mov\tssp,#047Fh", "A0987F04");
    }


}
