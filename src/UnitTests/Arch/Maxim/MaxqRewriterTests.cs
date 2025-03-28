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
using Reko.Arch.Maxim;
using Reko.Core;

namespace Reko.UnitTests.Arch.Maxim;

[TestFixture]
public class MaxqRewriterTests : RewriterTestBase
{
    private readonly Address addr;
    private readonly MaxqArchitecture arch;

    public MaxqRewriterTests()
    {
        this.arch = new MaxqArchitecture(CreateServiceContainer(), "maxq", []);
        this.addr = Address.Ptr16(0x100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    [Test]
    public void MaxqRw_add()
    {
        Given_HexString("39CA");
        AssertCode(     // add	acc,a[3]
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc + a3",
            "2|L--|cszv = cond(acc)");
    }

    [Test]
    public void MaxqRw_addc()
    {
        Given_HexString("0AEA");
        AssertCode(     // addc	acc,acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc + acc + c",
            "2|L--|cszv = cond(acc)");
    }

    [Test]
    public void MaxqRw_and()
    {
        Given_HexString("011A");
        AssertCode(     // and	acc,#01
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc & 1<8>",
            "2|L--|sz = cond(acc)");
    }

    [Test]
    public void MaxqRw_cmp()
    {
        Given_HexString("03F8");
        AssertCode(     // cmp	m3[0]
            "0|L--|0100(1): 1 instructions",
            "1|L--|e = acc == __read_modreg(0<8>)");
    }

    [Test]
    public void MaxqRw_cpl_acc()
    {
        Given_HexString("1A8A");
        AssertCode(     // cpl	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = ~acc",
            "2|L--|sz = cond(acc)");
    }

    [Test]
    public void MaxqRw_lcall()
    {
        Given_HexString("653D");
        AssertCode(     // lcall	#65
            "0|T--|0100(1): 2 instructions",
            "1|L--|v4 = SEQ(pfx0, 0x65<8>)",
            "2|T--|call v4 (2)");
    }

    [Test]
    public void MaxqRw_ldjnz()
    {
        Given_HexString("FD4D");
        AssertCode(     // ldjnz	lc[0],#FD
            "0|T--|0100(1): 3 instructions",
            "1|L--|lc0 = lc0 - 1<i16>",
            "2|L--|v5 = SEQ(pfx0, 0xFD<8>)",
            "3|T--|if (lc0 != 0<16>) branch v5");
    }

    [Test]
    public void MaxqRw_ljump_nopfx()
    {
        Given_HexString("910C");
        AssertCode(     // move 
            "0|T--|0100(1): 2 instructions",
            "1|L--|v4 = SEQ(pfx0, 0x91<8>)",
            "2|T--|goto v4");
    }

    [Test]
    public void MaxqRw_ljump_c()
    {
        Given_HexString("642C");
        AssertCode(     // ljump	c,#64
            "0|T--|0100(1): 2 instructions",
            "1|L--|v5 = SEQ(pfx0, 0x64<8>)",
            "2|T--|if (Test(ULT,c)) branch v5");
    }

    [Test]
    public void MaxqRw_ljump_e()
    {
        Given_HexString("2C3C");
        AssertCode(     // ljump	e,#2C
            "0|T--|0100(1): 2 instructions",
            "1|L--|v5 = SEQ(pfx0, 0x2C<8>)",
            "2|T--|if (Test(EQ,c)) branch v5");
    }

    [Test]
    public void MaxqRw_move_nul()
    {
        Given_HexString("6E76");
        AssertCode(     // move	NUL,#6E
            "0|L--|0100(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void MaxqRw_move_pfx()
    {
        Given_HexString("002B");
        AssertCode(     // move	pfx[2],#00
            "0|L--|0100(1): 2 instructions",
            "1|L--|pfx2 = 0<8>",
            "2|L--|csze = cond(0<8>)");
    }

    [Test]
    public void MaxqRw_move_to_modulereg()
    {
        Given_HexString("0C91");
        AssertCode(     // move 
            "0|L--|0100(1): 3 instructions",
            "1|L--|v4 = ip",
            "2|L--|__write_modreg(0<8>, v4)",
            "3|L--|csze = cond(ip)");
    }

    [Test]
    public void MaxqRw_move_predec()
    {
        Given_HexString("011F");
        AssertCode(     // move	@++dp[0],#01
            "0|L--|0100(1): 4 instructions",
            "1|L--|v3 = 1<8>",
            "2|L--|dp0 = dp0 + 1<i16>",
            "3|L--|Mem0[dp0:word16] = v3",
            "4|L--|csze = cond(1<8>)");
    }

    [Test]
    public void MaxqRw_neg()
    {
        Given_HexString("9A8A");
        AssertCode(     // neg	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = -acc",
            "2|L--|sz = cond(acc)");
    }

    [Test]
    public void MaxqRw_nop()
    {
        Given_HexString("3ADA");
        AssertCode(     // nop
            "0|L--|0100(1): 1 instructions",
            "1|L--|nop");
    }

    [Test]
    public void MaxqRw_or()
    {
        Given_HexString("002A");
        AssertCode(     // or	acc,#00
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc | 0<8>",
            "2|L--|sz = cond(acc)");
    }

    [Test]
    public void MaxqRw_pop()
    {
        Given_HexString("0D89");
        AssertCode(     // pop	a[0]
            "0|L--|0100(1): 3 instructions",
            "1|L--|a0 = Mem0[sp:word16]",
            "2|L--|sp = sp - 2<i16>",
            "3|L--|csze = cond(a0)");
    }

    [Test]
    public void MaxqRw_push()
    {
        Given_HexString("3E8D");
        AssertCode(     // push	offs
            "0|L--|0100(1): 3 instructions",
            "1|L--|sp = sp + 2<i16>",
            "2|L--|offs = Mem0[sp:word16]",
            "3|L--|csze = cond(offs)");
    }

    [Test]
    public void MaxqRw_ret()
    {
        Given_HexString("0D8C");
        AssertCode(     // ret
            "0|R--|0100(1): 1 instructions",
            "1|R--|return (2,0)");
    }

    [Test]
    public void MaxqRw_ret_nc()
    {
        Given_HexString("07EC");
        AssertCode(     // ret	nc
            "0|R--|0100(1): 2 instructions",
            "1|T--|if (Test(ULT,c)) branch 0101",
            "2|R--|return (2,0)");
    }

    [Test]
    public void MaxqRw_reti()
    {
        Given_HexString("8D8C");
        AssertCode(     // reti
            "0|R--|0100(1): 2 instructions",
            "1|L--|__return_from_interrupt()",
            "2|R--|return (2,0)");
    }

    [Test]
    public void MaxqRw_reti_s()
    {
        Given_HexString("CDCC");
        AssertCode(     // reti	s
            "0|R--|0100(1): 3 instructions",
            "1|T--|if (Test(GE,z)) branch 0101",
            "2|L--|__return_from_interrupt()",
            "3|R--|return (2,0)");
    }

    [Test]
    public void MaxqRw_rr()
    {
        Given_HexString("CA8A");
        AssertCode(     // rr	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = __ror<word16,byte>(acc, 1<8>)",
            "2|L--|sz = cond(acc)");
    }

    [Test]
    public void MaxqRw_scall()
    {
        Given_HexString("79BD");
        AssertCode(     // scall	a[7]
            "0|T--|0100(1): 1 instructions",
            "1|T--|call a7 (2)");
    }

    [Test]
    public void MaxqRw_sla()
    {
        Given_HexString("2A8A");
        AssertCode(     // sla	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc << 1<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sla2()
    {
        Given_HexString("3A8A");
        AssertCode(     // sla2	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc << 2<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sla4()
    {
        Given_HexString("6A8A");
        AssertCode(     // sla4	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc << 4<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sr()
    {
        Given_HexString("AA8A");
        AssertCode(     // sr	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc >>u 1<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sra()
    {
        Given_HexString("FA8A");
        AssertCode(     // sra	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc >> 1<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sra2()
    {
        Given_HexString("EA8A");
        AssertCode(     // sra2	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc >> 2<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sra4()
    {
        Given_HexString("BA8A");
        AssertCode(     // sra4	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc >> 4<8>",
            "2|L--|csz = cond(acc)");
    }

    [Test]
    public void MaxqRw_sub()
    {
        Given_HexString("015A");
        AssertCode(     // sub	acc,#01
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc - 1<8>",
            "2|L--|cszv = cond(acc)");
    }

    [Test]
    public void MaxqRw_subb()
    {
        Given_HexString("00FA");
        AssertCode(     // subb	acc,m0[0]
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc - __read_modreg(0<8>) - c",
            "2|L--|cszv = cond(acc)");
    }

    [Test]
    public void MaxqRw_xch()
    {
        Given_HexString("8A8A");
        AssertCode(     // xch	acc
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = __exchange_bytes(acc)",
            "2|L--|s = cond(acc)");
    }

    [Test]
    public void MaxqRw_xor()
    {
        Given_HexString("733A");
        AssertCode(     // xor	acc,#73
            "0|L--|0100(1): 2 instructions",
            "1|L--|acc = acc ^ 0x73<8>",
            "2|L--|sz = cond(acc)");
    }

    [Test]
    public void MaxqRw_xor_c()
    {
        Given_HexString("2ABA");
        AssertCode(     // xor	c,acc.<2>
            "0|L--|0100(1): 2 instructions",
            "1|L--|c = c ^ __bit<word16,byte>(acc, 2<8>)",
            "2|L--|sz = cond(c)");
    }
}
