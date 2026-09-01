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
using Reko.Arch.Mips.Machine;
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Mips;

using static MipsGenerator;

public class Ps2EeDis_assemblerTests : DisassemblerTestBase<MipsInstruction>
{
    private readonly MipsArchitecture arch;
    private readonly Address addrBase;

    public Ps2EeDis_assemblerTests()
    {
        var options = new Dictionary<string, object>
        {
            { ProcessorOption.InstructionSet, "ps2ee" },
            { ProcessorOption.Endianness, "le" },
            { ProcessorOption.WordSize, 32 },
        };

        this.arch = new MipsLe32Architecture(CreateServiceContainer(), "mips-32-le", options);
        this.addrBase = Address.Ptr32(0x0010_0000);
        Reko.Core.Machine.Decoder.trace.Level = System.Diagnostics.TraceLevel.Verbose;
    }

    public override IProcessorArchitecture Architecture => this.arch;
    public override Address LoadAddress => addrBase;


    // Helper method for FPU instructions
    private uint FpuW(int fmt, int ft = 0, int fs = 0, int fd = 0, int funct = 0, int ccOrRm = 0)
    {
        const int COP1_OPCODE = 0x11;
        return (uint)((COP1_OPCODE << 26) | (fmt << 21) | (ft << 16) | (fs << 11) | (fd << 6) | (ccOrRm << 4) | (funct & 0xF)) |
               ((funct & 0x30) != 0 ? (uint)((funct & 0x3F) << 0) : 0);
    }

    private void AssertCode(string expected, string hexBytes)
    {
        var instr = DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(expected));
    }

    private void AssertCode(string sExp, uint wInstr)
    {
        var instr = DisassembleWord(wInstr);
        Assert.AreEqual(sExp, instr.ToString());
    }

    [Test]
    public void Ps2EeDis_add_s()
    {
        AssertCode("add.s\tf2,f2,f2", "80100246");
    }

    [Test]
    public void Ps2EeDis_cop0_transfers()
    {
        Assert.AreEqual("mfc0\tr2,status", DisassembleWord(W(0x10, rs: 0, rt: 2, rd: 12)).ToString());
        Assert.AreEqual("mtc0\tr2,status", DisassembleWord(W(0x10, rs: 4, rt: 2, rd: 12)).ToString());
        Assert.AreEqual("mfc0\tr2,count", DisassembleWord(W(0x10, rs: 0, rt: 2, rd: 9)).ToString());
    }

    [Test]
    public void Ps2EeDis_cop0_operations()
    {
        Assert.AreEqual("tlbr", DisassembleWord(W(0x10, rs: 0x10, funct: 0x01)).ToString());
        Assert.AreEqual("tlbwi", DisassembleWord(W(0x10, rs: 0x10, funct: 0x02)).ToString());
        Assert.AreEqual("tlbwr", DisassembleWord(W(0x10, rs: 0x10, funct: 0x06)).ToString());
        Assert.AreEqual("tlbp", DisassembleWord(W(0x10, rs: 0x10, funct: 0x08)).ToString());
        Assert.AreEqual("eret", DisassembleWord(W(0x10, rs: 0x10, funct: 0x18)).ToString());
        Assert.AreEqual("ei", DisassembleWord(W(0x10, rs: 0x10, funct: 0x38)).ToString());
        Assert.AreEqual("di", DisassembleWord(W(0x10, rs: 0x10, funct: 0x39)).ToString());
    }

    [Test]
    public void Ps2EeDis_cop2_transfers()
    {
        Assert.AreEqual("qmfc2\tr2,vf1", DisassembleWord(Cop2W(1, rt: 2, rd: 1)).ToString());
        Assert.AreEqual("cfc2\tr2,vi5", DisassembleWord(Cop2W(2, rt: 2, rd: 5)).ToString());
        Assert.AreEqual("qmtc2\tr2,vf3", DisassembleWord(Cop2W(5, rt: 2, rd: 3)).ToString());
        Assert.AreEqual("ctc2\tr2,vi8", DisassembleWord(Cop2W(6, rt: 2, rd: 8)).ToString());
    }

    [Test]
    public void Ps2EeDis_cvt_s_w()
    {
        AssertCode("cvt.s.w\tf0,f0", "20008046");
    }

    [Test]
    public void Ps2EeDis_cvt_w_s()
    {
        AssertCode("cvt.w.s\tf2,f0", "A4000046");
    }

    [Test]
    public void Ps2EeDis_cache_pref()
    {
        AssertCode("cache\t1F,0010(r4)", W(0x2F, rs: 4, rt: 31, rest: 0x10));
        AssertCode("pref\t03,00F0(r25)", W(0x33, rs: 25, rt: 3, rest: 0xF0));
    }

    [Test]
    public void Ps2EeDis_di()
    {
        AssertCode("di", "39000042");
    }

    [Test]
    public void Ps2EeDis_div1()
    {
        AssertCode("div1\tr7,r3", "1A00E370");
    }

    [Test]
    public void Ps2EeDis_ei()
    {
        AssertCode("ei", "38000042");
    }

    [Test]
    public void Ps2EeDis_fpu_arithmetic()
    {
        Assert.AreEqual("add.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x00)).ToString());
        Assert.AreEqual("sub.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x01)).ToString());
        Assert.AreEqual("mul.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x02)).ToString());
        Assert.AreEqual("div.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x03)).ToString());
        Assert.AreEqual("sqrt.s\tf1,f2", DisassembleWord(FpuW(0x10, fs: 2, fd: 1, funct: 0x04)).ToString());
        Assert.AreEqual("abs.s\tf1,f2", DisassembleWord(FpuW(0x10, fs: 2, fd: 1, funct: 0x05)).ToString());
        Assert.AreEqual("mov.s\tf1,f2", DisassembleWord(FpuW(0x10, fs: 2, fd: 1, funct: 0x06)).ToString());
        Assert.AreEqual("neg.s\tf1,f2", DisassembleWord(FpuW(0x10, fs: 2, fd: 1, funct: 0x07)).ToString());
        Assert.AreEqual("rsqrt.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x16)).ToString());
    }

    [Test]
    public void Ps2EeDis_fpu_transfers()
    {
        Assert.AreEqual("mfc1\tr2,f12", DisassembleWord(FpuW(0x00, fs: 12, ft: 2)).ToString());
        Assert.AreEqual("mtc1\tr2,f13", DisassembleWord(FpuW(0x04, fs: 13, ft: 2)).ToString());
    }


    [Test]
    public void Ps2EeDis_fpu_mac_pipeline()
    {
        Assert.AreEqual("adda.s\tf2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, funct: 0x18)).ToString());
        Assert.AreEqual("mula.s\tf2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, funct: 0x1A)).ToString());
        Assert.AreEqual("madd.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x1C)).ToString());
        Assert.AreEqual("msub.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x1D)).ToString());
        Assert.AreEqual("madda.s\tf2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x1E)).ToString());
    }

    [Test]
    public void Ps2EeDis_fpu_conversions_and_minmax()
    {
        Assert.AreEqual("cvt.w.s\tf1,f2", DisassembleWord(FpuW(0x10, fs: 2, fd: 1, funct: 0x24)).ToString());
        Assert.AreEqual("cvt.s.w\tf1,f2", DisassembleWord(FpuW(0x14, fs: 2, fd: 1, funct: 0x20)).ToString());
        Assert.AreEqual("max.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x28)).ToString());
        Assert.AreEqual("min.s\tf1,f2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 1, funct: 0x29)).ToString());
    }

    [Test]
    public void Ps2EeDis_fpu_compares_and_branches()
    {
        Assert.AreEqual("c.lt.s\tf2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 0, funct: 0x34)).ToString());
        Assert.AreEqual("c.eq.s\tf2,f3", DisassembleWord(FpuW(0x10, ft: 3, fs: 2, fd: 0, funct: 0x32, ccOrRm: 1)).ToString());
        var instr = DisassembleWord((0x11 << 26) | (0x08 << 21) | (1 << 16) | 1);
        Assert.AreEqual("bc1t\t00100008", instr.ToString());
        Assert.AreEqual(InstrClass.CondJump | InstrClass.Delay, instr.InstructionClass);
    }




    [Test]
    public void Ps2EeDis_lq()
    {
        AssertCode("lq\tr3,0000(r4)", "00008378");
    }

    [Test]
    public void Ps2EeDis_lqc2_sqc2()
    {
        Assert.AreEqual("lqc2\tvf16,0020(r4)",
            DisassembleWord(W(0x36, rs: 4, rt: 16, rest: 0x20)).ToString());
        Assert.AreEqual("sqc2\tvf17,0030(r5)",
            DisassembleWord(W(0x3E, rs: 5, rt: 17, rest: 0x30)).ToString());
    }

    [Test]
    public void Ps2EeDis_mfhi1()
    {
        AssertCode("mfhi1\tr2", "10100070");
    }

    [Test]
    public void Ps2EeDis_mfc0()
    {
        AssertCode("mfc0\tr2,taglo", "00E00240");
    }

    [Test]
    public void Ps2EeDis_mflo1()
    {
        AssertCode("mflo1\tr7", "12380070");
    }

    [Test]
    public void Ps2EeDis_mtlo1()
    {
        AssertCode("mtlo1\tr0", "13004070");
    }

    [Test]
    public void Ps2EeDis_mmi_accumulators()
    {
        AssertCode("madd\tr6,r4,r5", MmiW(4, 5, 6, 0, 0x00));
        AssertCode("maddu\tr6,r4,r5", MmiW(4, 5, 6, 0, 0x01));
        AssertCode("plzcw\tr6,r4", MmiW(4, 5, 6, 0, 0x04));
        AssertCode("mfhi1\tr6", MmiW(4, 5, 6, 0, 0x10));
        AssertCode("mthi1\tr6", MmiW(4, 5, 6, 0, 0x11));
        AssertCode("mflo1\tr6", MmiW(4, 5, 6, 0, 0x12));
        AssertCode("mtlo1\tr6", MmiW(4, 5, 6, 0, 0x13));
        AssertCode("mult1\tr6,r4,r5", MmiW(4, 5, 6, 0, 0x18));
        AssertCode("multu1\tr6,r4,r5", MmiW(4, 5, 6, 0, 0x19));
        AssertCode("div1\tr4,r5", MmiW(4, 5, 6, 0, 0x1A));
        AssertCode("divu1\tr4,r5", MmiW(4, 5, 6, 0, 0x1B));
        AssertCode("madd1\tr6,r4,r5", MmiW(4, 5, 6, 0, 0x20));
    }

    [Test]
    public void Ps2EeDis_mmi_parallel_shifts()
    {
        AssertCode("psllh\tr2,r3,04", MmiW(0, 3, 2, 4, 0x34));
        AssertCode("psrlh\tr2,r3,04", MmiW(0, 3, 2, 4, 0x36));
        AssertCode("psrah\tr2,r3,04", MmiW(0, 3, 2, 4, 0x37));
        AssertCode("psllw\tr2,r3,04", MmiW(0, 3, 2, 4, 0x3C));
        AssertCode("psrlw\tr2,r3,04", MmiW(0, 3, 2, 4, 0x3E));
        AssertCode("psraw\tr2,r3,04", MmiW(0, 3, 2, 4, 0x3F));
    }

    [Test]
    public void Ps2EeDis_mmi0_list()
    {
        // MMI0 list entries are selected by the sa field with funct=0x08.
        AssertCode("paddw\tr6,r4,r5", MmiW(4, 5, 6, 0x00, 0x08));
        AssertCode("psubw\tr6,r4,r5", MmiW(4, 5, 6, 0x01, 0x08));
        AssertCode("pcgtw\tr6,r4,r5", MmiW(4, 5, 6, 0x02, 0x08));
        AssertCode("pmaxw\tr6,r4,r5", MmiW(4, 5, 6, 0x03, 0x08));
        AssertCode("paddh\tr6,r4,r5", MmiW(4, 5, 6, 0x04, 0x08));
        AssertCode("pcgth\tr6,r4,r5", MmiW(4, 5, 6, 0x06, 0x08));
        AssertCode("paddb\tr6,r4,r5", MmiW(4, 5, 6, 0x08, 0x08));
        AssertCode("paddsw\tr6,r4,r5", MmiW(4, 5, 6, 0x10, 0x08));
        AssertCode("pextlw\tr6,r4,r5", MmiW(4, 5, 6, 0x12, 0x08));
        AssertCode("ppacw\tr6,r4,r5", MmiW(4, 5, 6, 0x13, 0x08));
        AssertCode("pextlh\tr6,r4,r5", MmiW(4, 5, 6, 0x16, 0x08));
        AssertCode("ppacb\tr6,r4,r5", MmiW(4, 5, 6, 0x1B, 0x08));
        AssertCode("pext5\tr6,r5", MmiW(0, 5, 6, 0x1E, 0x08));
        AssertCode("ppac5\tr6,r5", MmiW(0, 5, 6, 0x1F, 0x08));
    }

    [Test]
    public void Ps2EeDis_mmi1_list()
    {
        // MMI1 list entries have funct=0x28.
        AssertCode("pabsw\tr6,r5", MmiW(0, 5, 6, 0x01, 0x28));
        AssertCode("pceqw\tr6,r4,r5", MmiW(4, 5, 6, 0x02, 0x28));
        AssertCode("pminw\tr6,r4,r5", MmiW(4, 5, 6, 0x03, 0x28));
        AssertCode("padsbh\tr6,r4,r5", MmiW(4, 5, 6, 0x04, 0x28));
        AssertCode("pabsh\tr6,r5", MmiW(0, 5, 6, 0x05, 0x28));
        AssertCode("pceqb\tr6,r4,r5", MmiW(4, 5, 6, 0x0A, 0x28));
        AssertCode("padduw\tr6,r4,r5", MmiW(4, 5, 6, 0x10, 0x28));
        AssertCode("pextuw\tr6,r4,r5", MmiW(4, 5, 6, 0x12, 0x28));
        AssertCode("pextub\tr6,r4,r5", MmiW(4, 5, 6, 0x1A, 0x28));
        AssertCode("qfsrv\tr6,r4,r5", MmiW(4, 5, 6, 0x1B, 0x28));
    }

    [Test]
    public void Ps2EeDis_mmi2_list()
    {
        // MMI2 list entries have funct=0x09.
        AssertCode("pmaddw\tr6,r4,r5", MmiW(4, 5, 6, 0x00, 0x09));
        AssertCode("psllvw\tr6,r4,r5", MmiW(4, 5, 6, 0x02, 0x09));
        AssertCode("pmfhi\tr6", MmiW(0, 0, 6, 0x08, 0x09));
        AssertCode("pmflo\tr6", MmiW(0, 0, 6, 0x09, 0x09));
        AssertCode("pinth\tr6,r4,r5", MmiW(4, 5, 6, 0x0A, 0x09));
        AssertCode("pmultw\tr6,r4,r5", MmiW(4, 5, 6, 0x0C, 0x09));
        AssertCode("pdivw\tr4,r5", MmiW(4, 5, 0, 0x0D, 0x09));
        AssertCode("pcpyld\tr6,r4,r5", MmiW(4, 5, 6, 0x0E, 0x09));
        AssertCode("phmadh\tr6,r4,r5", MmiW(4, 5, 6, 0x11, 0x09));
        AssertCode("pand\tr6,r4,r5", MmiW(4, 5, 6, 0x12, 0x09));
        AssertCode("pxor\tr6,r4,r5", MmiW(4, 5, 6, 0x13, 0x09));
        AssertCode("pexeh\tr6,r5", MmiW(0, 5, 6, 0x1A, 0x09));
        AssertCode("prevh\tr6,r5", MmiW(0, 5, 6, 0x1B, 0x09));
        AssertCode("prot3w\tr6,r5", MmiW(0, 5, 6, 0x1F, 0x09));
    }

    [Test]
    public void Ps2EeDis_mmi3_list()
    {
        // MMI3 list entries have funct=0x29.
        AssertCode("pmadduw\tr6,r4,r5", MmiW(4, 5, 6, 0x00, 0x29));
        AssertCode("psravw\tr6,r4,r5", MmiW(4, 5, 6, 0x03, 0x29));
        AssertCode("pmthi\tr4", MmiW(4, 5, 6, 0x08, 0x29));
        AssertCode("pmtlo\tr4", MmiW(4, 5, 6, 0x09, 0x29));
        AssertCode("pinteh\tr6,r4,r5", MmiW(4, 5, 6, 0x0A, 0x29));
        AssertCode("pmultuw\tr6,r4,r5", MmiW(4, 5, 6, 0x0C, 0x29));
        AssertCode("pdivuw\tr4,r5", MmiW(4, 5, 6, 0x0D, 0x29));
        AssertCode("pcpyud\tr6,r4,r5", MmiW(4, 5, 6, 0x0E, 0x29));
        AssertCode("por\tr6,r4,r5", MmiW(4, 5, 6, 0x12, 0x29));
        AssertCode("pnor\tr6,r4,r5", MmiW(4, 5, 6, 0x13, 0x29));
        AssertCode("pexch\tr6,r5", MmiW(4, 5, 6, 0x1A, 0x29));
        AssertCode("pcpyh\tr6,r5", MmiW(4, 5, 6, 0x1B, 0x29));
        AssertCode("pexcw\tr6,r5", MmiW(4, 5, 6, 0x1E, 0x29));
    }


    [Test]
    public void Ps2EeDis_mov_s()
    {
        AssertCode("mov.s\tf15,f0", "C6030046");
    }

    [Test]
    public void Ps2EeDis_mul_s()
    {
        AssertCode("mul.s\tf0,f12,f0", "02600046");
    }

    [Test]
    public void Ps2EeDis_multu1()
    {
        AssertCode("multu1\tr0,r3,r5", "19006570");
    }

    [Test]
    public void Ps2EeDis_pand()
    {
        AssertCode("pand\tr2,r2,r3", "89144370");
    }

    [Test]
    public void Ps2EeDis_pcpyh()
    {
        AssertCode("pcpyh\tr3,r8", "E91E0870");
    }

    [Test]
    public void Ps2EeDis_pcpyld()
    {
        AssertCode("pcpyld\tr8,r3,r3", "89436370");
    }

    [Test]
    public void Ps2EeDis_pcpyud()
    {
        AssertCode("pcpyud\tr10,r8,r7", "A9530771");
    }

    [Test]
    public void Ps2EeDis_pmfhl_pmthl()
    {
        AssertCode("pmfhl.lw\tr6", MmiW(0, 0, 6, 0, 0x30));
        AssertCode("pmfhl.uw\tr6", MmiW(0, 0, 6, 1, 0x30));
        AssertCode("pmthl.lw\tr4", MmiW(4, 0, 0, 0, 0x31));
    }


    [Test]
    public void Ps2EeDis_pnor()
    {
        AssertCode("pnor\tr3,r0,r3", "E91C0370");
    }

    [Test]
    public void Ps2EeDis_psubb()
    {
        AssertCode("psubb\tr2,r3,r8", "48126870");
    }

    [Test]
    public void Ps2EeDis_psubw()
    {
        AssertCode("psubw\tr7,r2,r3", "48384370");
    }

    [Test]
    public void Ps2EeDis_pxor()
    {
        AssertCode("pxor\tr8,r2,r3", "C9444370");
    }

    [Test]
    public void Ps2EeDis_shift_amount_register()
    {
        AssertCode("mfsa\tr2", W(0, rd: 2, funct: 0x28));
        AssertCode("mtsa\tr4", W(0, rs: 4, funct: 0x29));
        AssertCode("mtsab\tr4,-00000008", W(1, rs: 4, rt: 24, rest: 0xFFF8 & 0xFFFF));
        AssertCode("mtsah\tr4,-00000008", W(1, rs: 4, rt: 25, rest: 0xFFF8 & 0xFFFF));
    }


    [Test]
    public void Ps2EeDis_sq()
    {
        AssertCode("sq\tr3,0000(r4)", "0000837C");
    }

    [Test]
    public void Ps2EeDis_vcallms()
    {
        Assert.AreEqual("vcallms\t00001238",
            DisassembleWord(Cop2W(0x10, funct: 0x38, rest: 0x1238 << 3)).ToString());
        Assert.AreEqual("vcallmsr",
            DisassembleWord(Cop2W(0x10, funct: 0x39)).ToString());
    }

    [Test]
    public void Ps2EeDis_vu_integer_pipe()
    {
        // VIADD id, is, it
        Assert.AreEqual("viadd\tvi3,vi4,vi5",
            DisassembleWord(Cop2W(0x10, rt: 5, rd: 4, sa: 3, funct: 0x30)).ToString());
        // VIADDI it, is, imm5
        Assert.AreEqual("viaddi\tvi4,vi5,+00000006",
            DisassembleWord(Cop2W(0x10, rt: 4, rd: 5, funct: 0x32, rest: 6 << 6)).ToString());
        Assert.AreEqual("viand\tvi3,vi4,vi5",
            DisassembleWord(Cop2W(0x10, rt: 5, rd: 4, sa: 3, funct: 0x34)).ToString());
    }

    [Test]
    public void Ps2EeDis_bc2()
    {
        var instr = DisassembleWord(Cop2W(8, rt: 1, rest: 1));
        Assert.AreEqual("bc2t\t00100008", instr.ToString());
        Assert.AreEqual(InstrClass.CondJump | InstrClass.Delay, instr.InstructionClass);
    }

    [Test]
    public void Ps2EeDis_vu_broadcast_forms()
    {
        // Field layout of the upper ops: ft=[20:16], fs=[15:11],
        // fd=[10:6]; the rs field carries the x/y/z/w destination mask.
        // VADDx.w vf1, vf2, vf3 (broadcast source lane w)
        Assert.AreEqual("vaddx.w\tvf1,vf2,vf3",
            DisassembleWord(Cop2W(0x11, rt: 3, rd: 2, sa: 1, funct: 0x00)).ToString());
        Assert.AreEqual("vmulz.y\tvf4,vf5,vf6",
            DisassembleWord(Cop2W(0x14, rt: 6, rd: 5, sa: 4, funct: 0x1A)).ToString());
        Assert.AreEqual("vmaxw.xyz\tvf7,vf8,vf9",
            DisassembleWord(Cop2W(0x1E, rt: 9, rd: 8, sa: 7, funct: 0x13)).ToString());
    }

    [Test]
    public void Ps2EeDis_vu_scalar_forms()
    {
        // VMUL.q vf1, vf2, Q
        Assert.AreEqual("vmulq.xyzw\tvf1,vf2,q",
            DisassembleWord(Cop2W(0x1F, rd: 2, sa: 1, funct: 0x1C)).ToString());
        // VADD.i vf3, vf4, I
        Assert.AreEqual("vaddi.xyzw\tvf3,vf4,i",
            DisassembleWord(Cop2W(0x1F, rd: 4, sa: 3, funct: 0x22)).ToString());
    }

    [Test]
    public void Ps2EeDis_vu_vector_forms()
    {
        // VADD vf1, vf2, vf3
        Assert.AreEqual("vadd.xyzw\tvf1,vf2,vf3",
            DisassembleWord(Cop2W(0x1F, rt: 3, rd: 2, sa: 1, funct: 0x28)).ToString());
        Assert.AreEqual("vopmsub\tvf4,vf2,vf3",
            DisassembleWord(Cop2W(0x10, rt: 3, rd: 2, sa: 4, funct: 0x2E)).ToString());
        Assert.AreEqual("vmini.xyzw\tvf1,vf2,vf3",
            DisassembleWord(Cop2W(0x1F, rt: 3, rd: 2, sa: 1, funct: 0x2F)).ToString());
    }



    [Test]
    public void Ps2EeDis_vu_special2_conversions()
    {
        // Index into the special2 table = sa<<2 | funct[1:0], with
        // funct[5:4] = 11; conversions write their result to the ft field.
        // VFTOI12 vf1, vf2: index 22 = sa 5, col 2.
        Assert.AreEqual("vftoi12.xyzw\tvf1,vf2",
            DisassembleWord(Cop2W(0x1F, rt: 1, rd: 2, sa: 5, funct: 0x3E)).ToString());
        // VITOF15 vf3, vf4: index 19 = sa 4, col 3.
        Assert.AreEqual("vitof15.xyzw\tvf3,vf4",
            DisassembleWord(Cop2W(0x1F, rt: 3, rd: 4, sa: 4, funct: 0x3F)).ToString());
    }

    [Test]
    public void Ps2EeDis_vu_special2_acc_forms()
    {
        // VMULAx.x ACC, vf1, vf2: index 24 = sa 6, col 0.
        Assert.AreEqual("vmulax.xyzw\tacc,vf1,vf2",
            DisassembleWord(Cop2W(0x1F, rt: 2, rd: 1, sa: 6, funct: 0x3C)).ToString());
        // VCLIPw vf1, vf2: index 31 = sa 7, col 3.
        Assert.AreEqual("vclipw\tvf1,vf2",
            DisassembleWord(Cop2W(0x10, rt: 2, rd: 1, sa: 7, funct: 0x3F)).ToString());
        // VADDA vf3, vf4: index 40 = sa 10, col 0.
        Assert.AreEqual("vadda.xyzw\tacc,vf3,vf4",
            DisassembleWord(Cop2W(0x1F, rt: 4, rd: 3, sa: 10, funct: 0x3C)).ToString());
    }

    [Test]
    public void Ps2EeDis_vnop()
    {
        // VNOP: index 47 = sa 11, col 3.
        Assert.AreEqual("vnop",
            DisassembleWord(Cop2W(0x10, sa: 11, funct: 0x3F)).ToString());
    }

    [Test]
    public void Ps2EeDis_vu_special2_move_efu()
    {
        // VMOVE vf1, vf2: index 48 = sa 12, col 0; dest in ft.
        Assert.AreEqual("vmove.xyzw\tvf1,vf2",
            DisassembleWord(Cop2W(0x1F, rt: 1, rd: 2, sa: 12, funct: 0x3C)).ToString());
        // VDIV Q, vf1, vf2: index 56 = sa 14, col 0.
        Assert.AreEqual("vdiv\tq,vf1,vf2",
            DisassembleWord(Cop2W(0x10, rt: 2, rd: 1, sa: 14, funct: 0x3C)).ToString());
        // VWAITQ: index 59 = sa 14, col 3.
        Assert.AreEqual("vwaitq",
            DisassembleWord(Cop2W(0x10, sa: 14, funct: 0x3F)).ToString());
        // VMTIR vi5, vf6.z
        Assert.AreEqual("vmtir.z\tvi5,vf6",
            DisassembleWord(Cop2W(0x12, rt: 5, rd: 6, sa: 15, funct: 0x3C)).ToString());
    }

    [Test]
    public void Ps2EeDis_vu_memory_ops()
    {
        // VLQI vf1, (vi5): index 52 = sa 13, col 0.
        Assert.AreEqual("vlqi.xyzw\tvf1,vi5",
            DisassembleWord(Cop2W(0x1F, rt: 1, rd: 5, sa: 13, funct: 0x3C)).ToString());
        // VSQD vf2, (vi6): index 55 = sa 13, col 3.
        Assert.AreEqual("vsqd.xyzw\tvf2,vi6",
            DisassembleWord(Cop2W(0x1F, rt: 6, rd: 2, sa: 13, funct: 0x3F)).ToString());
    }

    [Test]
    public void Ps2EeDis_real_world_dot_product_sequence()
    {
        // Sequence observed in a shipping PS2 title: LQC2 loads followed
        // by a dot product accumulation into ACC.
        Assert.AreEqual("vmulax.xyzw\tacc,vf1,vf16",
            DisassembleWord(0x4BF009BC).ToString());
        Assert.AreEqual("vmadday.xyzw\tacc,vf2,vf16",
            DisassembleWord(0x4BF010BD).ToString());
        Assert.AreEqual("vmaddaz.xyzw\tacc,vf3,vf16",
            DisassembleWord(0x4BF018BE).ToString());
    }

}
