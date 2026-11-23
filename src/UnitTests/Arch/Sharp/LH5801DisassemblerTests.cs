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
using Reko.Arch.Sharp;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;

namespace Reko.UnitTests.Arch.Sharp;

public class LH5801DisassemblerTests : DisassemblerTestBase<LH5801Instruction>
{
    private readonly LH5801Architecture arch;
    private readonly Address addr;

    public LH5801DisassemblerTests()
    {
        this.arch = new LH5801Architecture(CreateServiceContainer(), "lh5801", []);
        this.addr = Address.Ptr16(0x1000);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addr;

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = DisassembleHexBytes(hexBytes);
        Assert.That(instr.ToString(), Is.EqualTo(sExpected));
    }

    public void LH5801Dis_Generate()
    {
        var buf = new byte[65536];
        var rnd = new Random(47212);
        rnd.NextBytes(buf);
        var mem = new ByteMemoryArea(addr, buf);
        var rdr = mem.CreateLeReader(0);
        var dasm = arch.CreateDisassembler(rdr);
        var testSvc = arch.Services.GetService<ITestGenerationService>();
        foreach (var instr in dasm)
        {
            if (instr.MnemonicAsString == "Nyi")
                continue;
            testSvc?.ReportMissingDecoder("LH5801Dis", instr.Address, rdr, instr.MnemonicAsString);
        }
    }

    [Test]
    public void LH5801Dis_adc_Mx()
    {
        AssertCode("adc\t(x)", "03");
    }

    [Test]
    public void LH5801Dis_adc_xl()
    {
        AssertCode("adc\txl", "02");
    }

    [Test]
    public void LH5801Dis_adi_a()
    {
        AssertCode("adi\ta,6Dh", "B36D");
    }

    [Test] public void LH5801Dis_adi_ME1() => AssertCode("adi\t#(81EEh),0A9h", "FDEF81EEA9");


    [Test]
    public void LH5801Dis_adr_x()
    {
        AssertCode("adr\tx", "FDCA");
    }

    [Test] public void LH5801Dis_and_Ma() => AssertCode("and\t(0C60Bh)", "A9C60B");

    [Test]
    public void LH5801Dis_ani_Maddr()
    {
        AssertCode("ani\t(0DAF6h),04h", "E9DAF604");
    }

    [Test] public void LH5801Dis_aex() => AssertCode("aex", "F1");

    [Test] public void LH5801Dis_att() => AssertCode("att", "FDEC");

    [Test] public void LH5801Dis_bch() => AssertCode("bch\t10F4h", "8EF4");

    [Test] public void LH5801Dis_bch_Back() => AssertCode("bch\t0F0Ch", "9EF4");

    [Test]
    public void LH5801Dis_bcs()
    {
        AssertCode("bcs\t1043h", "8343");
    }

    [Test]
    public void LH5801Dis_bii_My()
    {
        AssertCode("bii\t(y),92h", "5D92");
    }

    [Test]
    public void LH5801Dis_bit()
    {
        AssertCode("bit\t(u)", "2F");
    }

    [Test] public void LH5801Dis_bit_ME1() => AssertCode("bit\t#(7334h)", "FDAF7334");


    [Test]
    public void LH5801Dis_bvr()
    {
        AssertCode("bvr\t10A2h", "8DA2");
    }

    [Test]
    public void LH5801Dis_bvs()
    {
        AssertCode("bvs\t10EDh", "8FED");
    }

    [Test] public void LH5801Dis_bvs_neg() => AssertCode("bvs\t0F04h", "9FFC");

    [Test]
    public void LH5801Dis_bzr()
    {
        AssertCode("bzr\t0F4Ah", "99B6");
    }

    [Test]
    public void LH5801Dis_bzs()
    {
        AssertCode("bzs\t0FF4h", "9B0C");
    }

    [Test]
    public void LH5801Dis_cin()
    {
        AssertCode("cin", "F7");
    }

    [Test]
    public void LH5801Dis_cpa_Mx()
    {
        AssertCode("cpa\t(x)", "07");
    }

    [Test]
    public void LH5801Dis_cpa_xl()
    {
        AssertCode("cpa\txl", "06");
    }

    [Test]
    public void LH5801Dis_cpi()
    {
        AssertCode("cpi\tuh,0B3h", "6CB3");
    }

    [Test] public void LH5801Dis_dca_Mu() => AssertCode("dca\t(u)", "AC");

    [Test] public void LH5801Dis_dcs_E1() => AssertCode("dcs\t#(x)", "FD0C");

    [Test] public void LH5801Dis_dec_a() => AssertCode("dec\ta", "DF");

    [Test] public void LH5801Dis_dec_u() => AssertCode("dec\tu", "66");

    [Test]
    public void LH5801Dis_dec_ul()
    {
        AssertCode("dec\tul", "62");
    }

    [Test]
    public void LH5801Dis_drl()
    {
        AssertCode("drl", "D7");
    }

    [Test]
    public void LH5801Dis_drr()
    {
        AssertCode("drr", "D3");
    }

    [Test] public void LH5801Dis_eor_Maddr() => AssertCode("eor\t(0BC29h)", "ADBC29");

    [Test] public void LH5801Dis_eor_Mu() => AssertCode("eor\t(u)", "2D");

    [Test]
    public void LH5801Dis_eor_My()
    {
        AssertCode("eor\t(y)", "1D");
    }

    [Test] public void LH5801Dis_eai_a() => AssertCode("eai\ta,0C0h", "BDC0");

    [Test] public void LH5801Dis_eor_ME1() => AssertCode("eor\t#(y)", "FD1D");
    [Test] public void LH5801Dis_inc_a() => AssertCode("inc\ta", "DD");
    [Test] public void LH5801Dis_inc_u() => AssertCode("inc\tu", "64");

    [Test] public void LH5801Dis_inc_uh() => AssertCode("inc\tuh", "FD60");

    [Test] public void LH5801Dis_inc_xh() => AssertCode("inc\txh", "FD40");

    [Test]
    public void LH5801Dis_inc_y()
    {
        AssertCode("inc\ty", "54");
    }

    [Test]
    public void LH5801Dis_inc_yl()
    {
        AssertCode("inc\tyl", "50");
    }

    [Test]
    public void LH5801Dis_lda_Mx()
    {
        AssertCode("lda\t(x)", "05");
    }

    [Test] public void LH5801Dis_lda_uh() => AssertCode("lda\tuh", "A4");

    [Test]
    public void LH5801Dis_lda_xl()
    {
        AssertCode("lda\txl", "04");
    }

    [Test] public void LH5801Dis_lde() => AssertCode("lde\ty", "57");

    [Test]
    public void LH5801Dis_ldi_s()
    {
        AssertCode("ldi\ts,0FB15h", "AAFB15");
    }

    [Test]
    public void LH5801Dis_ldi_xl()
    {
        AssertCode("ldi\txl,0C3h", "4AC3");
    }

    [Test] public void LH5801Dis_ldx_s() => AssertCode("ldx\ts", "FD18");

    [Test] public void LH5801Dis_lin() => AssertCode("lin\tu", "65");

    [Test] public void LH5801Dis_lop() => AssertCode("lop\tul,0F0Bh", "98F5");

    [Test] public void LH5801Dis_nop() => AssertCode("nop", "38");

    [Test]
    public void LH5801Dis_ora_Mx()
    {
        AssertCode("ora\t(x)", "0B");
    }

    [Test] public void LH5801Dis_off() => AssertCode("off", "FD4C");

    [Test]
    public void LH5801Dis_ori_Madd()
    {
        AssertCode("ori\t(5E0Ah),2Dh", "EB5E0A2D");
    }

    [Test] public void LH5801Dis_ori_Mu() => AssertCode("ori\t(u),0BAh", "6BBA");


    [Test] public void LH5801Dis_pop_y() => AssertCode("pop\ty", "FD1A");

    [Test] public void LH5801Dis_psh() => AssertCode("psh\ta", "FDC8");

    [Test] public void LH5801Dis_rdp() => AssertCode("rdp", "FDC0");

    [Test]
    public void LH5801Dis_rec()
    {
        AssertCode("rec", "F9");
    }

    [Test] public void LH5801Dis_rol() => AssertCode("rol", "DB");

    [Test]
    public void LH5801Dis_ror()
    {
        AssertCode("ror", "D1");
    }

    [Test] public void LH5801Dis_rpu() => AssertCode("rpu", "E3");

    [Test]
    public void LH5801Dis_rpv()
    {
        AssertCode("rpv", "B8");
    }

    [Test]
    public void LH5801Dis_rti()
    {
        AssertCode("rti", "8A");
    }

    [Test] public void LH5801Dis_rtn() => AssertCode("rtn", "9A");

    [Test]
    public void LH5801Dis_sbc_uh()
    {
        AssertCode("sbc\tuh", "A0");
    }

    [Test] public void LH5801Dis_sbi_a() => AssertCode("sbi\ta,0FCh", "B1FC");


    [Test] public void LH5801Dis_sde() => AssertCode("sde\ty", "53");

    [Test] public void LH5801Dis_sec() => AssertCode("sec", "FB");

    [Test]
    public void LH5801Dis_shl()
    {
        AssertCode("shl", "D9");
    }

    [Test] public void LH5801Dis_shr() => AssertCode("shr", "D5");


    [Test] public void LH5801Dis_sin() => AssertCode("sin\ty", "51");

    [Test] public void LH5801Dis_sjp() => AssertCode("sjp\t0E94Bh", "BEE94B");

    [Test]
    public void LH5801Dis_spu()
    {
        AssertCode("spu", "E1");
    }

    [Test] public void LH5801Dis_spv() => AssertCode("spv", "A8");

    [Test]
    public void LH5801Dis_sta_Maddr()
    {
        AssertCode("sta\t(196Bh)", "AE196B");
    }

    [Test]
    public void LH5801Dis_sta_Mx()
    {
        AssertCode("sta\t(x)", "0E");
    }

    [Test]
    public void LH5801Dis_sta_xh()
    {
        AssertCode("sta\txh", "08");
    }

    [Test]
    public void LH5801Dis_sta_xl()
    {
        AssertCode("sta\txl", "0A");
    }

    [Test] public void LH5801Dis_stx_s() => AssertCode("stx\ts", "FD4E");

    [Test] public void LH5801Dis_tin() => AssertCode("tin", "F5");

    [Test]
    public void LH5801Dis_vej()
    {
        AssertCode("vej\t0D0h", "D0");
    }

    [Test] public void LH5801Dis_vhs() => AssertCode("vhs\t0D0h", "C7D0");


    [Test]
    public void LH5801Dis_vmj()
    {
        AssertCode("vmj\t12h", "CD12");
    }

    [Test] public void LH5801Dis_vvs() => AssertCode("vvs\t0E4h", "CFE4");
    [Test] public void LH5801Dis_vzs() => AssertCode("vzs\t0E0h", "CBE0");
}
