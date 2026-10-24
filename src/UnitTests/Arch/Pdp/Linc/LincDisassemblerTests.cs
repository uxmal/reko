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
using Reko.Arch.Pdp;
using Reko.Arch.Pdp.Linc;
using Reko.Core;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch.Pdp.Linc;

public class LincDisassemblerTests : DisassemblerTestBase<LincInstruction>
{
    private readonly LincArchitecture arch;
    private readonly Address addrLoad;

    public LincDisassemblerTests()
    {
        this.arch = new LincArchitecture(CreateServiceContainer(), "linc", []);
        addrLoad = LincArchitecture.Ptr12(0x10);
    }

    private void AssertCode(string sExp, string octalwords)
    {
        var words = LincArchitecture.OctalStringToWords(octalwords);
        var mem = new Word16MemoryArea(addrLoad, words);
        var instr = Disassemble(mem);
        Assert.AreEqual(sExp, instr.ToString());
    }

    private void AssertMultipleCode(string sExp, string octalwords)
    {
        var words = LincArchitecture.OctalStringToWords(octalwords);
        var mem = new Word16MemoryArea(addrLoad, words);
        var rdr = arch.CreateImageReader(mem, addrLoad);
        var dasm = new LincDisassembler(arch, rdr);
        var sActual = string.Join(" # ", dasm);
        Assert.AreEqual(sExp, sActual);
    }

    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress => addrLoad;

    [Test]
    public void LincDis_ada()
    {
        AssertMultipleCode("ADA # 1234", "1100 1234");
        AssertMultipleCode("ADA i 0003", "1123");
    }

    [Test]
    public void LincDis_add()
    {
        AssertCode("ADD   1123", "3123");
    }

    [Test]
    public void LincDis_adm()
    {
        AssertCode("ADM i 0005", "1165");
    }

    [Test]
    public void LincDis_apo()
    {
        AssertCode("APO", "0451");
        AssertCode("APO i", "0471");
    }

    [Test]
    public void LincDis_atr()
    {
        AssertCode("ATR", "0014");
    }

    [Test]
    public void LincDis_axo()
    {
        AssertCode("AXO", "0001");
    }

    [Test]
    public void LincDis_aze()
    {
        AssertCode("AZE", "0450");
        AssertCode("AZE i", "0470");
    }

    [Test]
    public void LincDis_bcl()
    {
        AssertCode("BCL   0004", "1544");
    }

    [Test]
    public void LincDis_bco()
    {
        AssertCode("BCO   0004", "1644");
    }

    [Test]
    public void LincDis_bse()
    {
        AssertMultipleCode("BSE i # 4242", "1620 4242");
    }

    [Test]
    public void LincDis_chk()
    {
        AssertCode("CHK   0 3|500", "0707 3500");
        AssertCode("CHK i 1 4|501", "0737 4501");
    }

    [Test]
    public void LincDis_clr()
    {
        AssertCode("CLR", "0011");
    }

    [Test]
    public void LincDis_com()
    {
        AssertCode("COM", "0017");
    }

    [Test]
    public void LincDis_dis()
    {
        AssertCode("DIS   0007", "0147");
        AssertCode("DIS i 0007", "0167");
    }

    [Test]
    public void LincDis_djr()
    {
        // PDP-12
        AssertCode("DJR", "0006");
    }

    [Test]
    public void LincDis_dsc()
    {
        AssertCode("DSC   0007", "1747");
        AssertCode("DSC i 0007", "1767");
    }

    [Test]
    public void LincDis_flo()
    {
        AssertCode("FLO", "0454");
        AssertCode("FLO i", "0474");
    }

    [Test]
    public void LincDis_halt()
    {
        AssertCode("HALT", "0000");
    }

    [Test]
    public void LincDis_ibz()
    {
        AssertCode("IBZ", "0453");
        AssertCode("IBZ i", "0473");
    }

    [Test]
    public void LincDis_iof()
    {
        // PDP-12
        AssertCode("IOF", "0500 6002");
    }

    [Test]
    public void LincDis_ion()
    {
        // PDP-12
        AssertCode("ION", "0500 6001");
    }

    [Test]
    public void LincDis_kst()
    {
        AssertCode("KST", "0415");
        AssertCode("KST i", "0435");
    }

    [Test]
    public void LincDis_lam()
    {
        AssertCode("LAM i 0013", "1233");
    }

    [Test]
    public void LincDis_lda()
    {
        AssertMultipleCode("LDA i # 1234", "1020 1234");
    }

    [Test]
    public void LincDis_ldf()
    {
        // PDP-12
        AssertCode("LDF   4", "0644");
        AssertCode("LDF   37", "0677");
    }

    [Test]
    public void LincDis_ldh()
    {
        AssertCode("LDH i 0003", "1323");
    }

    [Test]
    public void LincDis_lif()
    {
        // PDP-12
        AssertCode("LIF   4", "0604");
        AssertCode("LIF   37", "0637");
    }

    [Test]
    public void LincDis_lmr()
    {
        AssertCode("LMR", "0500 6151");
    }

    [Test]
    public void LincDis_lsw()
    {
        AssertCode("LSW", "0517");
    }

    [Test]
    public void LincDis_lze()
    {
        AssertCode("LZE", "0452");
        AssertCode("LZE i", "0472");
    }

    [Test]
    public void LincDis_mtb()
    {
        AssertCode("MTB   0 3|500", "0703 3500");
        AssertCode("MTB i 1 4|501", "0733 4501");
    }

    [Test]
    public void LincDis_mul()
    {
        AssertMultipleCode("MUL # 1234", "1240 1234");
        AssertMultipleCode("MUL i # 1234", "1260 1234");
    }

    [Test]
    public void LincDis_nop()
    {
        AssertCode("NOP", "0016");
    }

    [Test]
    public void LincDis_pdp()
    {
        AssertCode("PDP", "0002");
    }
    
    [Test]
    public void LincDis_qac()
    {
        AssertCode("QAC", "0005");
    }

    [Test]
    public void LincDis_qlz()
    {
        AssertCode("QLZ", "0455");
        AssertCode("QLZ i", "0475");
    }

    [Test]
    public void LincDis_rcg()
    {
        AssertCode("RCG   0 3|500", "0700 3500");
        AssertCode("RCG i 1 4|501", "0730 4501");
    }

    [Test]
    public void LincDis_rdc()
    {
        AssertCode("RDC   0 3|500", "0701 3500");
        AssertCode("RDC i 1 4|501", "0731 4501");
    }

    [Test]
    public void LincDis_rde()
    {
        AssertCode("RDE   0 3|500", "0702 3500");
        AssertCode("RDE i 1 4|501", "0732 4501");
    }

    [Test]
    public void LincDis_rdf()
    {
        // PDP-12
        AssertCode("RDF", "0500 6214");
    }

    [Test]
    public void LincDis_rib()
    {
        // PDP-12
        AssertCode("RIB", "0500 6234");
    }

    [Test]
    public void LincDis_rif()
    {
        // PDP-12
        AssertCode("RIF", "0500 6224");
    }

    [Test]
    public void LincDis_rmf()
    {
        // PDP-12
        AssertCode("RMF", "0500 6244");
    }

    [Test]
    public void LincDis_rol()
    {
        AssertCode("ROL   1", "0241");
        AssertCode("ROL i 1", "0261");
    }

    [Test]
    public void LincDis_ror()
    {
        AssertCode("ROR   1", "0301");
        AssertCode("ROR i 1", "0321");
    }

    [Test]
    public void LincDis_rsw()
    {
        AssertCode("RSW", "0516");
    }

    [Test]
    public void LincDis_rta()
    {
        AssertCode("RTA", "0015");
    }

    [Test]
    public void LincDis_sae()
    {
        AssertCode("SAE   0004", "1444");
    }

    [Test]
    public void LincDis_sam()
    {
        AssertCode("SAM   4", "0104");
        AssertCode("SAM   37", "0137");
    }

    [Test]
    public void LincDis_scr()
    {
        AssertCode("SCR   3", "0343");
        AssertCode("SCR i 3", "0363");
    }

    [Test]
    public void LincDis_set()
    {
        AssertMultipleCode("SET   0000 # 0004", "0040 0004");
        AssertMultipleCode("SET i 0000 # 0024", "0060 0024");
    }

    [Test]
    public void LincDis_shd()
    {
        AssertMultipleCode("SHD i # 5500", "1420 5500");
    }

    [Test]
    public void LincDis_skp()
    {
        AssertMultipleCode("SKP", "0456");
        AssertMultipleCode("SKP i", "0476");
    }

    [Test]
    public void LincDis_sns()
    {
        AssertCode("SNS   0", "0440");
        AssertCode("SNS i 2", "0462");
    }

    [Test]
    public void LincDis_sro()
    {
        AssertCode("SRO i 0005", "1525");
    }

    [Test]
    public void LincDis_sta()
    {
        AssertCode("STA   0003", "1043");
    }

    [Test]
    public void LincDis_stc()
    {
        AssertCode("STC   0124", "4124");
    }

    [Test]
    public void LincDis_std()
    {
        AssertCode("STD", "0416");
        AssertCode("STD i", "0436");
    }

    [Test]
    public void LincDis_sth()
    {
        AssertCode("STH i 0007", "1367");
    }

    [Test]
    public void LincDis_sxl()
    {
        AssertCode("SXL   3", "0403");
        AssertCode("SXL i 3", "0423");
    }

    [Test]
    public void LincDis_tac()
    {
        AssertCode("TAC", "0003");
    }

    [Test]
    public void LincDis_tma()
    {
        AssertCode("TMA", "0023");
    }

    [Test]
    public void LincDis_twc()
    {
        AssertCode("TWC", "0417");
        AssertCode("TWC i", "0437");
    }

    [Test]
    public void LincDis_wcg()
    {
        AssertCode("WCG   0 3|500", "0705 3500");
        AssertCode("WCG i 1 4|501", "0735 4501");
    }

    [Test]
    public void LincDis_wrc()
    {
        AssertCode("WRC   0 3|500", "0704 3500");
        AssertCode("WRC i 1 4|501", "0734 4501");
    }

    [Test]
    public void LincDis_wri()
    {
        AssertCode("WRI   0 3|500", "0706 3500");
        AssertCode("WRI i 1 4|501", "0736 4501");
    }

    [Test]
    public void LincDis_xoa()
    {
        AssertCode("XOA", "0021");
    }

    [Test]
    public void LincDis_xsk()
    {
        AssertCode("XSK   0006", "0206");
        AssertCode("XSK i 0006", "0226");
    }
}
