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

using Reko.Arch.Kalimba;
using Reko.Core;
using NUnit.Framework;

namespace Reko.UnitTests.Arch.Kalimba;

[TestFixture]
public class KalimbaDisassemblerTests : DisassemblerTestBase<KalimbaInstruction>
{
    public KalimbaDisassemblerTests() 
    {
        Architecture = new KalimbaArchitecture(CreateServiceContainer(), "kalimba", []);
        LoadAddress = Address.Ptr16(0x1000);
    }

    public override IProcessorArchitecture Architecture { get; }

    public override Address LoadAddress { get; }

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = base.DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(sExpected));   
    }

    [Test]
    public void KalimbaDasm_add_bank_A_000()
    {
        AssertCode("if neg r3 = Null + r1 ;; r2 = [i0,m0];", "4050 2134");
    }

    [Test]
    public void KalimbaDasm_add_bank_A_001()
    {
        AssertCode("if z r3 = r6 + i3 ;; [i0,m0] = r2;", "4458 A130");
    }

    [Test]
    public void KalimbaDasm_add_bank_A_010()
    {
        AssertCode("r3 = i0 + r1 ;; r2 = [i0,m0];", "4850 213F");
    }

    [Test]
    public void KalimbaDasm_add_bank_A_011()
    {
        AssertCode("if nz r3 = i0 + i3 ;; r2 = [i0,m0];", "4C50 2131");
    }

    [Test]
    public void KalimbaDasm_add_bank_B_000()
    {
        AssertCode("r3 = r10 + 2132;", "415C 2132");
    }

    [Test]
    public void KalimbaDasm_add_bank_B_010()
    {
        AssertCode("r3 = i0 + FFFF;", "4950 FFFF");
    }

    [Test]
    public void KalimbaDasm_add_bank_C1_000()
    {
        AssertCode("r3 = r3 + r0 ;; r2 = [i0,m0] ;; r3 = [i1,m0];", "4252 2134");
    }

    [Test]
    public void KalimbaDasm_add_bank_C1_001()
    {
        AssertCode("r3 = r3 + m1 ;; r2 = [i0,m0] ;; r3 = [i1,m0];", "4659 2134");
    }

    [Test]
    public void KalimbaDasm_add_bank_C2_000()
    {
        AssertCode("r3 = r3 + r9 ;; r2 = [i0,-000001] ;; r3 = [i1,-000001];", "435B 2134");
    }

    [Test]
    public void KalimbaDasm_add_bank_C2_001()
    {
        AssertCode("r3 = r3 + i0 ;; r2 = [i0,-000001] ;; r3 = [i1,-000001];", "4750 2134");
    }

    [Test]
    public void KalimbaDasm_add_bank_C2_010()
    {
        AssertCode("i5 = i5 + r1 ;; r2 = [i0,-000001] ;; r3 = [i1,-000001];", "5B53 2134");
    }

    [Test]
    public void KalimbaDasm_add_bank_C2_111()
    {
        AssertCode("i5 = i5 + i0 ;; r2 = [i0,-000001] ;; r3 = [i1,-000001];", "5F50 2134");
    }

    [Test]
    public void KalimbaDasm_and()
    {
        AssertCode("if nc r3 = r2 and r1 ;; r2 = [i0,m0];",                     "8054 2133");
        AssertCode("r3 = r2 and 2133;",                                         "8154 2133");
        AssertCode("r3 = r3 and r2 ;; r2 = [i0,m0] ;; r3 = [i0,m3];",           "8254 2133");
        AssertCode("r3 = r3 and r2 ;; r2 = [i0,-000001] ;; r3 = [i0,+000002];", "8354 2133");
    }

    [Test]
    public void KalimbaDasm_ash()
    {
        AssertCode("if nc r3 = r2 ashift r1 ;; r0 = [i0,m0];", "9054 0133");
        AssertCode("r3 = r2 ashift 2133;", "9154 2133");
        AssertCode("r3 = r3 ashift r2 ;; r2 = [i0,m0] ;; r3 = [i0,m3];", "9254 2133");
        AssertCode("r3 = r3 ashift r2 ;; r2 = [i0,-000001] ;; r3 = [i0,+000002];", "9354 2133");
    }

    //$REVIEW: documentation doesn't specify whether this is a 
    // absolute or relative call. Currently Presumed relative.
    [Test]
    public void KalimbaDasm_call_disp()
    {
        AssertCode("if z call 1000;", "E146 0000");
    }

    [Test]
    public void KalimbaDasm_call_reg()
    {
        AssertCode("call r4;", "E046 0000");
    }

    [Test]
    public void KalimbaDasm_div()
    {
        AssertCode("div = rMAC / r4;",      "D946 0000");
        AssertCode("r2 = divResult;",       "D946 0001");
        AssertCode("r2 = divRemainder;",    "D946 0002");
    }

    //$REVIEW: documentation doesn't specify whether this is a 
    // absolute or relative jump. Currently Presumed relative.
    [Test]
    public void KalimbaDasm_jump_disp()
    {
        AssertCode("if z jump 1000;", "DD46 0000");
    }

    [Test]
    public void KalimbaDasm_jump_reg()
    {
        AssertCode("jump r4;", "DC46 0000");
    }

    [Test]
    public void KalimbaDasm_ld_idx()
    {
        AssertCode("r2 = [r4,r0];", "D046 042F");
        AssertCode("if userdef r2 = [r4,r0];", "D046 842E");
    }

    [Test]
    public void KalimbaDasm_ld_offset()
    {
        AssertCode("r2 = [r4,+0000042F];", "D146 042F");
        AssertCode("r2 = [r4,-00007BD2];", "D146 842E");
    }

    [Test]
    public void KalimbaDasm_lsh()
    {
        AssertCode("if nc r3 = r2 lshift r1 ;; [i0,m0] = r0;",                      "8C54 8133");
        AssertCode("r3 = r2 lshift 2133;",                                          "8D54 2133");
        AssertCode("r3 = r3 lshift r2 ;; r2 = [i0,m0] ;; r3 = [i0,m3];",            "8E54 2133");
        AssertCode("r3 = r3 lshift r2 ;; r2 = [i0,-000001] ;; r3 = [i0,+000002];",  "8F54 2133");
    }

    [Test]
    public void KalimbaDasm_maca()
    {
        AssertCode("rMAC = rMAC + r2 * 213F (su);", "A954 213F");
    }

    [Test]
    public void KalimbaDasm_macs()
    {
        AssertCode("if nc rMAC = rMAC - r2 * r1 (us);", "B454 2133");
    }

    [Test]
    public void KalimbaDasm_rti()
    {
        AssertCode("if z rti;", "E246 0000");
    }

    [Test]
    public void KalimbaDasm_rts()
    {
        AssertCode("if z rts;", "DF46 0000");
    }

    [Test]
    public void KalimbaDasm_smulv()
    {
        AssertCode("if nc r3 = r2 * r1  (sat);",    "9C54 2133");
        AssertCode("r3 = r2 * 2133  (sat);",        "9D54 2133");
        AssertCode("r3 = r3 * r2  (sat);",          "9E54 2133");
        AssertCode("r3 = r3 * r2  (sat);",          "9F54 2133");
    }

    [Test]
    public void KalimbaDasm_st_offset()
    {
        AssertCode("[r4,r0] = r2;", "D446 042F");
        AssertCode("[r4,-00007BD2] = r2;", "D546 842E");
    }

}
