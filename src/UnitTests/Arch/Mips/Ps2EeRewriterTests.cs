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
using Reko.Arch.Mips;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Mips;

[TestFixture]
public class Ps2EeRewriterTests : RewriterTestBase
{
    private readonly MipsArchitecture arch;
    private readonly Address addrBase;
    public Ps2EeRewriterTests()
    {
        var options = new Dictionary<string, object>
        {
           { ProcessorOption.InstructionSet, "ps2ee"  }
        };
        this.arch = new MipsLe32Architecture(CreateServiceContainer(), "mips-32-le", options);
        this.addrBase = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => this.arch;
    public override Address LoadAddress => addrBase;

    [Test]
    public void MipsRw_cvt_s_w()
    {
        Given_HexString("20008046");
        AssertCode(     // cvt.s.w	f0,f0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f0 = CONVERT(f0, int32, real32)");
    }

    [Test]
    public void MipsRw_cvt_w_s()
    {
        Given_HexString("A4000046");
        AssertCode(     // cvt.w.s	f2,f0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|f2 = CONVERT(roundf(f0), real32, int32)");
    }

    [Test]
    public void MipsRw_div1()
    {
        Given_HexString("1A00E370");
        AssertCode(     // div1	r0,r7,r3
            "0|L--|00100000(4): 2 instructions",
            "1|L--|lo1 = r0 / r7",
            "2|L--|hi1 = r0 %s r7");
    }

    [Test]
    public void MipsRw_ei()
    {
        Given_HexString("38000042");
        AssertCode(     // ei
            "0|L--|00100000(4): 1 instructions",
            "1|L--|__enable_interrupts()");
    }

    [Test]
    public void MipsRw_mf0()
    {
        Given_HexString("00601040");
        AssertCode(     // mf0	r16
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r16 = CONVERT(__move_from_breakpoint_control_register(), word32, int128)");
    }

    [Test]
    public void MipsRw_mfhi1()
    {
        Given_HexString("10300070");
        AssertCode(     // mfhi1	r6
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r6 = SEQ(SLICE(r6, word64, 64), hi1)");
    }

    [Test]
    public void MipsRw_mflo1()
    {
        Given_HexString("12380070");
        AssertCode(     // mflo1	r7
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r7 = SEQ(SLICE(r7, word64, 64), lo1)");
    }

    [Test]
    public void MipsRw_mtlo1()
    {
        Given_HexString("13004070");
        AssertCode(     // mtlo1	r0
            "0|L--|00100000(4): 1 instructions",
            "1|L--|lo1 = SLICE(0<128>, word64, 0)");
    }

    [Test]
    public void MipsRw_multu1()
    {
        Given_HexString("19006570");
        AssertCode(     // multu1	r0,r3,r5
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r0 = r3 *u64 r5");
    }

    [Test]
    public void MipsRw_pcpyh()
    {
        Given_HexString("E91E0870");
        AssertCode(     // pcpyh	r3,r8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = __p_copy_halfword(r8)");
    }

    [Test]
    public void MipsRw_pcpyld()
    {
        Given_HexString("89434270");
        AssertCode(     // pcpyld	r8,r2,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = __p_copy_lower_dword(r2, r2)");
    }

    [Test]
    public void MipsRw_pcpyud()
    {
        Given_HexString("A9530771");
        AssertCode(     // pcpyud	r10,r8,r7
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r10 = __p_copy_upper_dword(r8, r7)");
    }

    [Test]
    public void MipsRw_pnor()
    {
        Given_HexString("E91C0270");
        AssertCode(     // pnor	r3,r0,r2
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r3 = __pnor(0<128>, r2)");
    }

    [Test]
    public void MipsRw_psubb()
    {
        Given_HexString("48126870");
        AssertCode(     // psubb	r2,r3,r8
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r2 = __simd_sub<byte>(r3, r8)");
    }

    [Test]
    public void MipsRw_psubw()
    {
        Given_HexString("48384370");
        AssertCode(     // psubw	r7,r2,r3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r7 = __simd_sub<word32>(r2, r3)");
    }

    [Test]
    public void MipsRw_pxor()
    {
        Given_HexString("C9444370");
        AssertCode(     // pxor	r8,r2,r3
            "0|L--|00100000(4): 1 instructions",
            "1|L--|r8 = __pxor(r2, r3)");
    }

    [Test]
    public void MipsRw_sq()
    {
        Given_HexString("0000407C");
        AssertCode(     // sq	r0,0000(r2)
            "0|L--|00100000(4): 2 instructions",
            "1|L--|v4 = SLICE(r2, ptr32, 0)",
            "2|L--|Mem0[v4:word128] = 0<128>");
    }
}
