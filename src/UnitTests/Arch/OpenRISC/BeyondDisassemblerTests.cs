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
using Reko.Arch.OpenRISC;
using Reko.Arch.OpenRISC.Beyond;
using Reko.Core;

namespace Reko.UnitTests.Arch.OpenRISC;

[TestFixture]
public class BeyondDisassemblerTests : DisassemblerTestBase<BeyondInstruction>
{
    private readonly BeyondArchitecture arch;
    private readonly Address addr;

    public BeyondDisassemblerTests()
    {
        this.arch = new BeyondArchitecture(CreateServiceContainer(), "beyond", []);
        this.addr = Address.Ptr32(0x1_0000);
    }

    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress => addr;

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = DisassembleHexBytes(hexBytes);
        Assert.AreEqual(sExpected, instr.ToString());
    }

    [Test]
    public void BeyondDis_bt_add()
    {
        AssertCode("bt.add\tr4,r7", "0887");
    }

    [Test]
    public void BeyondDis_bn_addc()
    {
        AssertCode("bn.addc\tr6,r24,r23", "64D8BF");
    }

    [Test]
    public void BeyondDis_bn_addi()
    {
        AssertCode("bn.addi\tr5,r3,0x0", "30A300");
    }

    [Test]
    public void BeyondDis_bn_andi()
    {
        AssertCode("bn.andi\tr23,r7,0xFFFFFF80", "36E780");
    }

    [Test]
    public void BeyondDis_bw_andi()
    {
        AssertCode("bw.andi\tr23,r3,0xFFFFFE00", "96E3FFFFFE00");
    }

    [Test]
    public void BeyondDis_bn_beqi()
    {
        AssertCode("bn.beqi\tr7,0x0,00010010", "400710");
    }

    [Test]
    public void BeyondDis_bw_beqi8()
    {
        AssertCode("bw.beqi\tr29,0xFFFFFFF6,0309187F", "A02D D308187F");
    }

    [Test]
    public void BeyondDis_bg_bleui()
    {
        AssertCode("bg.bleui\tr7,12,0000FF00", "D2187F00");
    }

    [Test]
    public void BeyondDis_bg_bne()
    {
        AssertCode("bg.bne\tr4,r4,0000FF48", "D2C64F48");
    }

    [Test]
    public void BeyondDis_bn_bnf()
    {
        AssertCode("bn.bnf\t0000FA80", "47 3A 80");
    }

    [Test]
    public void BeyondDis_bn_cmov()
    {
        AssertCode("bn.cmov\tr6,r6,r0", "64C602");
    }

    [Test]
    public void BeyondDis_bn_entri()
    {
        AssertCode("bn.entri\t0x8,0x80", "47A880");
    }

    [Test]
    public void BeyondDis_bn_extbz()
    {
        AssertCode("bn.extbz\tr24,r24", "3E18C0");
    }

    [Test]
    public void BeyondDis_bn_ff1()
    {
        AssertCode("bn.ff1\tr7,r7", "3E073C");
    }

    [Test]
    public void BeyondDis_bt_j()
    {
        AssertCode("bt.j\t000101A0", "0DA0");
    }

    [Test]
    public void BeyondDis_bn_j()
    {
        AssertCode("bn.j\t0000E720", "46E720");
    }

    [Test]
    public void BeyondDis_bn_jalr()
    {
        AssertCode("bn.jalr\tr7", "47D138");
    }

    [Test]
    public void BeyondDis_bn_lbz()
    {
        AssertCode("bn.lbz\tr5,(r5)", "24A300");
    }

    [Test]
    public void BeyondDis_bw_lbz()
    {
        AssertCode("bw.lbz\tr7,305419896(r0)", "84E012345678");
    }

    [Test]
    public void BeyondDis_bw_ld()
    {
        AssertCode("bw.ld\tr7,-231451016(r14)", "8CEEF2345678");
    }

    [Test]
    public void BeyondDis_bt_mov()
    {
        AssertCode("bt.mov\tr3,r6", "0466");
    }

    [Test]
    public void BeyondDis_bt_movi()
    {
        AssertCode("bt.movi\tr7,0x0", "00E0");
    }

    [Test]
    public void BeyondDis_bn_ori()
    {
        AssertCode("bn.ori\tr7,r0,0xC0", "38 E0 C0");
    }

    [Test]
    public void BeyondDis_bw_ori()
    {
        AssertCode("bw.ori\tr23,r23,0x100", "9AF700000100 ");
    }

    [Test]
    public void BeyondDis_bw_sfeqi()
    {
        AssertCode("bw.sfeqi\tr27,0xFFE00000", "9D9BFFE00000");
    }

    [Test]
    public void BeyondDis_bn_srai()
    {
        AssertCode("bn.srai\tr4,r4,0x8", "6C8442");
    }

    [Test]
    public void BeyondDis_bn_sw()
    {
        AssertCode("bn.sw\t28(r28),r4", "2C9C1C");
    }

    [Test]
    public void BeyondDis_bt_trap()
    {
        AssertCode("bt.trap\t0x0", "0000");
    }

    [Test]
    public void BeyondDis_bn_xor()
    {
        AssertCode("bn.xor\tr7,r7,r6", "60E732");
    }
}
