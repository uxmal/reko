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
using Reko.Arch.Infineon;
using Reko.Arch.Infineon.M8C;
using Reko.Core;

namespace Reko.UnitTests.Arch.Infineon.M8C;

public class M8CDisassemblerTests : DisassemblerTestBase<M8CInstruction>
{
    private readonly M8CArchitecture arch;
    private readonly Address addrLoad;

    public M8CDisassemblerTests()
    {
        this.arch = new M8CArchitecture(base.CreateServiceContainer(), "m8c", []);
        this.addrLoad = Address.Ptr16(0x0100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addrLoad;

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = base.DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(sExpected));
    }

    [Test]
    public void M8CDis_adc()
    {
        AssertCode("adc	A,2F", "092F");
    }

    [Test]
    public void M8CDis_add()
    {
        AssertCode("add	[X+0x31],A", "0531");
        AssertCode("add	A,FE", "01FE");
        AssertCode("add	[0x79],A", "0479");
        AssertCode("add	A,[X+0x67]", "0367");
    }

    [Test]
    public void M8CDis_and()
    {
        AssertCode("and	[X+0x27],6D", "27276D");
    }

    [Test]
    public void M8CDis_asr()
    {
        AssertCode("asr	A", "67");
    }

    [Test]
    public void M8CDis_call()
    {
        AssertCode("call	042B", "932A");
    }

    [Test]
    public void M8CDis_cmp()
    {
        AssertCode("cmp	A,[0x21]", "3A21");
    }

    [Test]
    public void M8CDis_dec()
    {
        AssertCode("dec	[X+0x26]", "7B26");
    }

    [Test]
    public void M8CDis_inc()
    {
        AssertCode("inc	[X+0xDD]", "77DD");
    }

    [Test]
    public void M8CDis_index()
    {
        AssertCode("index	062C", "F52B");
    }

    [Test]
    public void M8CDis_jacc()
    {
        AssertCode("jacc	03CD", "E2CC");
    }

    [Test]
    public void M8CDis_jc()
    {
        AssertCode("jc	02C1", "C1C0");
    }

    [Test]
    public void M8CDis_jmp()
    {
        AssertCode("jmp	0640", "853F");
    }

    [Test]
    public void M8CDis_jnc()
    {
        AssertCode("jnc	FB0D", "DA0C");
    }

    [Test]
    public void M8CDis_jnz()
    {
        AssertCode("jnz	00FD", "BFFC");
    }

    [Test]
    public void M8CDis_jz()
    {
        AssertCode("jz	0809", "A708");
    }

    [Test]
    public void M8CDis_ljmp()
    {
        AssertCode("ljmp	7964", "7D7964");
    }
    [Test]
    public void M8CDis_mov()
    {
        AssertCode("mov	A,[X+0x83]", "5E83");
        AssertCode("mov	A,[0x11]", "5111");
        AssertCode("mov	[0x42],A", "5342");
    }

    [Test]
    public void M8CDis_mvi()
    {
        AssertCode("mvi	[[0x8D]++],A", "3F8D");
    }

    [Test]
    public void M8CDis_or()
    {
        AssertCode("or	[0xCD],90", "2ECD90");
        AssertCode("or	[X+0x41],BC", "2F41BC");
    }

    [Test]
    public void M8CDis_push()
    {
        AssertCode("push	X", "10");
    }

    [Test]
    public void M8CDis_ret()
    {
        AssertCode("ret", "7F");
    }

    [Test]
    public void M8CDis_rlc()
    {
        AssertCode("rlc	A", "6A");
    }

    [Test]
    public void M8CDis_rrc()
    {
        AssertCode("rrc	A", "6D");
    }

    [Test]
    public void M8CDis_sbb()
    {
        AssertCode("sbb	A,[X+0xC5]", "1BC5");
    }

    [Test]
    public void M8CDis_swap()
    {
        AssertCode("swap	A,X", "4B");
    }

    [Test]
    public void M8CDis_tst()
    {
        AssertCode("tst	[0x73],42", "477342");
    }

    [Test]
    public void M8CDis_xor()
    {
        AssertCode("xor	F,E2", "72E2");
    }

    

    [Test]
    public void M8CDis_lcall()
    {
        AssertCode("lcall	1802", "7C1802");
    }


    [Test]
    public void M8CDis_ssc()
    {
        AssertCode("ssc", "00");
    }

    [Test]
    public void M8CDis_sub()
    {
        AssertCode("sub	A,[0xB1]", "12B1");
    }



}
