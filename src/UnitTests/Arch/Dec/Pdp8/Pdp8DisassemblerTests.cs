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
using Reko.Arch.Dec;
using Reko.Arch.Dec.Pdp8;
using Reko.Core;
using Reko.Core.Memory;

namespace Reko.UnitTests.Arch.Dec.Pdp8;

[TestFixture]
[Ignore("Not ready yet")]
public class Pdp8DisassemblerTests : DisassemblerTestBase<Pdp8Instruction>
{
    private readonly Pdp8Architecture arch;
    private readonly Address addrLoad;

    public Pdp8DisassemblerTests()
    {
        this.arch = new Pdp8Architecture(CreateServiceContainer(), "pdp8", []);
        this.addrLoad = Pdp8Architecture.Ptr12(0x100);
    }

    public override IProcessorArchitecture Architecture => arch;

    public override Address LoadAddress => addrLoad;

    private void AssertCode(string sExp, string octalwords)
    {
        var words = LincArchitecture.OctalStringToWords(octalwords);
        var mem = new Word16MemoryArea(addrLoad, words);
        var instr = Disassemble(mem);
        Assert.AreEqual(sExp, instr.ToString());
    }

    [Test]
    public void Pdp8Dis__cac()
    {
        AssertCode("_cac", "6702");
    }

    [Test]
    public void Pdp8Dis__osr()
    {
        AssertCode("_osr", "6704");
    }

    [Test]
    public void Pdp8Dis_acl()
    {
        AssertCode("acl", "7701");
    }

    [Test]
    public void Pdp8Dis_acs()
    {
        AssertCode("acs", "7403");
    }

    [Test]
    public void Pdp8Dis_asr()
    {
        AssertCode("asr", "7415");
    }

    [Test]
    public void Pdp8Dis_bsw()
    {
        AssertCode("bsw", "7002");
    }

    [Test]
    public void Pdp8Dis_caf()
    {
        AssertCode("caf", "6007");
    }

    [Test]
    public void Pdp8Dis_cam()
    {
        AssertCode("cam", "7621");
    }

    [Test]
    public void Pdp8Dis_ccar()
    {
        AssertCode("ccar", "6704");
    }

    [Test]
    public void Pdp8Dis_cdf()
    {
        AssertCode("cdf", "62n1");
    }

    [Test]
    public void Pdp8Dis_cdi()
    {
        AssertCode("cdi", "62n3");
    }

    [Test]
    public void Pdp8Dis_cep()
    {
        AssertCode("cep", "6106");
    }

    [Test]
    public void Pdp8Dis_cia()
    {
        AssertCode("cia", "7041");
    }

    [Test]
    public void Pdp8Dis_cif()
    {
        AssertCode("cif", "62n2");
    }

    [Test]
    public void Pdp8Dis_cint()
    {
        AssertCode("cint", "6204");
    }

    [Test]
    public void Pdp8Dis_cla_cll()
    {
        AssertCode("cla cll", "7300");
    }

    [Test]
    public void Pdp8Dis_cla_iac()
    {
        AssertCode("cla iac", "7201");
    }   

    [Test]
    public void Pdp8Dis_cla_swp()
    {
        AssertCode("cla swp", "7721");
    }

    [Test]
    public void Pdp8Dis_cla()
    {
        AssertCode("cla", "7200");
    }

    [Test]
    public void Pdp8Dis_cla_7600()
    {
        AssertCode("cla", "7600");
    }

    [Test]
    public void Pdp8Dis_cla_7601()
    {
        AssertCode("cla", "7601");
    }

    [Test]
    public void Pdp8Dis_clab()
    {
        AssertCode("clab", "6133");
    }

    [Test]
    public void Pdp8Dis_clba()
    {
        AssertCode("clba", "6136");
    }

    [Test]
    public void Pdp8Dis_clca()
    {
        AssertCode("clca", "6137");
    }

    [Test]
    public void Pdp8Dis_cldi()
    {
        AssertCode("cldi", "6132");
    }

    [Test]
    public void Pdp8Dis_clei()
    {
        AssertCode("clei", "6131");
    }

    [Test]
    public void Pdp8Dis_clen()
    {
        AssertCode("clen", "6134");
    }

    [Test]
    public void Pdp8Dis_clf()
    {
        AssertCode("clf", "6725");
    }

    [Test]
    public void Pdp8Dis_cll_ral()
    {
        AssertCode("cll ral", "7104");
    }

    [Test]
    public void Pdp8Dis_cll_rar()
    {
        AssertCode("cll rar", "7110");
    }

    [Test]
    public void Pdp8Dis_cll_rtl()
    {
        AssertCode("cll rtl", "7106");
    }

    [Test]
    public void Pdp8Dis_cll()
    {
        AssertCode("cll", "7100");
    }

    [Test]
    public void Pdp8Dis_cloe()
    {
        AssertCode("cloe", "6132");
    }

    [Test]
    public void Pdp8Dis_clsa()
    {
        AssertCode("clsa", "6135");
    }

    [Test]
    public void Pdp8Dis_clsk()
    {
        AssertCode("clsk", "6131");
    }

    [Test]
    public void Pdp8Dis_clsk_6133()
    {
        AssertCode("clsk", "6133");
    }

    [Test]
    public void Pdp8Dis_clt()
    {
        AssertCode("clt", "6712");
    }

    [Test]
    public void Pdp8Dis_clze()
    {
        AssertCode("clze", "6130");
    }

    [Test]
    public void Pdp8Dis_cma()
    {
        AssertCode("cma", "7040");
    }

    [Test]
    public void Pdp8Dis_cml()
    {
        AssertCode("cml", "7020");
    }

    [Test]
    public void Pdp8Dis_cmp()
    {
        AssertCode("cmp", "6104");
    }

    [Test]
    public void Pdp8Dis_cuf()
    {
        AssertCode("cuf", "6264");
    }

    [Test]
    public void Pdp8Dis_cwcr()
    {
        AssertCode("cwcr", "6702");
    }

    [Test]
    public void Pdp8Dis_dad()
    {
        AssertCode("dad", "7443");
    }

    [Test]
    public void Pdp8Dis_dbcd()
    {
        AssertCode("dbcd", "65x7");
    }

    [Test]
    public void Pdp8Dis_dbci()
    {
        AssertCode("dbci", "65x3");
    }

    [Test]
    public void Pdp8Dis_dbco()
    {
        AssertCode("dbco", "65x5");
    }

    [Test]
    public void Pdp8Dis_dbdi()
    {
        AssertCode("dbdi", "65x0");
    }

    [Test]
    public void Pdp8Dis_dbdi_65x6()
    {
        AssertCode("dbdi", "65x6");
    }

    [Test]
    public void Pdp8Dis_dbei()
    {
        AssertCode("dbei", "65x1");
    }

    [Test]
    public void Pdp8Dis_dbei_65x5()
    {
        AssertCode("dbei", "65x5");
    }

    [Test]
    public void Pdp8Dis_dbrd()
    {
        AssertCode("dbrd", "65x2");
    }

    [Test]
    public void Pdp8Dis_dbrf()
    {
        AssertCode("dbrf", "65x1");
    }

    [Test]
    public void Pdp8Dis_dbri()
    {
        AssertCode("dbri", "65x4");
    }

    [Test]
    public void Pdp8Dis_dbro()
    {
        AssertCode("dbro", "65x7");
    }

    [Test]
    public void Pdp8Dis_dbsk()
    {
        AssertCode("dbsk", "65x2");
    }

    [Test]
    public void Pdp8Dis_dbso()
    {
        AssertCode("dbso", "65x6");
    }

    [Test]
    public void Pdp8Dis_dbtd()
    {
        AssertCode("dbtd", "65x4");
    }

    [Test]
    public void Pdp8Dis_dbtf()
    {
        AssertCode("dbtf", "65x3");
    }

    [Test]
    public void Pdp8Dis_dcea()
    {
        AssertCode("dcea", "6611");
    }

    [Test]
    public void Pdp8Dis_dchp()
    {
        AssertCode("dchp", "6737");
    }

    [Test]
    public void Pdp8Dis_dcim()
    {
        AssertCode("dcim", "6611");
    }

    [Test]
    public void Pdp8Dis_dcla()
    {
        AssertCode("dcla", "6751");
    }

    [Test]
    public void Pdp8Dis_dcls()
    {
        AssertCode("dcls", "6742");
    }

    [Test]
    public void Pdp8Dis_dcm()
    {
        AssertCode("dcm", "7575");
    }

    [Test]
    public void Pdp8Dis_dcma()
    {
        AssertCode("dcma", "6601");
    }

    [Test]
    public void Pdp8Dis_dcxa()
    {
        AssertCode("dcxa", "6641");
    }

    [Test]
    public void Pdp8Dis_deac()
    {
        AssertCode("deac", "6616");
    }

    [Test]
    public void Pdp8Dis_deal()
    {
        AssertCode("deal", "6615");
    }

    [Test]
    public void Pdp8Dis_dfsc()
    {
        AssertCode("dfsc", "6622");
    }

    [Test]
    public void Pdp8Dis_dfse()
    {
        AssertCode("dfse", "6621");
    }

    [Test]
    public void Pdp8Dis_dicd()
    {
        AssertCode("dicd", "6051");
    }

    [Test]
    public void Pdp8Dis_dilc()
    {
        AssertCode("dilc", "6050");
    }

    [Test]
    public void Pdp8Dis_dile()
    {
        AssertCode("dile", "6056");
    }

    [Test]
    public void Pdp8Dis_dilx()
    {
        AssertCode("dilx", "6053");
    }

    [Test]
    public void Pdp8Dis_dily()
    {
        AssertCode("dily", "6054");
    }

    [Test]
    public void Pdp8Dis_dima()
    {
        AssertCode("dima", "6616");
    }

    [Test]
    public void Pdp8Dis_diml()
    {
        AssertCode("diml", "6615");
    }

    [Test]
    public void Pdp8Dis_dire()
    {
        AssertCode("dire", "6057");
    }

    [Test]
    public void Pdp8Dis_disd()
    {
        AssertCode("disd", "6052");
    }

    [Test]
    public void Pdp8Dis_disk()
    {
        AssertCode("disk", "6623");
    }

    [Test]
    public void Pdp8Dis_dixy()
    {
        AssertCode("dixy", "6055");
    }

    [Test]
    public void Pdp8Dis_dlca()
    {
        AssertCode("dlca", "6755");
    }

    [Test]
    public void Pdp8Dis_dlda()
    {
        AssertCode("dlda", "6731");
    }

    [Test]
    public void Pdp8Dis_dldc()
    {
        AssertCode("dldc", "6732");
    }

    [Test]
    public void Pdp8Dis_dldr()
    {
        AssertCode("dldr", "6733");
    }

    [Test]
    public void Pdp8Dis_dldw()
    {
        AssertCode("dldw", "6735");
    }

    [Test]
    public void Pdp8Dis_dlwc()
    {
        AssertCode("dlwc", "6753");
    }

    [Test]
    public void Pdp8Dis_dmac()
    {
        AssertCode("dmac", "6626");
    }

    [Test]
    public void Pdp8Dis_dmar()
    {
        AssertCode("dmar", "6603");
    }

    [Test]
    public void Pdp8Dis_dmaw()
    {
        AssertCode("dmaw", "6605");
    }

    [Test]
    public void Pdp8Dis_dmmt()
    {
        AssertCode("dmmt", "6646");
    }

    [Test]
    public void Pdp8Dis_dmnt()
    {
        AssertCode("dmnt", "6743");
    }

    [Test]
    public void Pdp8Dis_dpi()
    {
        AssertCode("dpi", "6100");
    }

    [Test]
    public void Pdp8Dis_dpic()
    {
        AssertCode("dpic", "7573");
    }

    [Test]
    public void Pdp8Dis_dpsz()
    {
        AssertCode("dpsz", "7451");
    }

    [Test]
    public void Pdp8Dis_drca()
    {
        AssertCode("drca", "6757");
    }

    [Test]
    public void Pdp8Dis_drda()
    {
        AssertCode("drda", "6734");
    }

    [Test]
    public void Pdp8Dis_drdc()
    {
        AssertCode("drdc", "6736");
    }

    [Test]
    public void Pdp8Dis_drds()
    {
        AssertCode("drds", "6741");
    }

    [Test]
    public void Pdp8Dis_drwc()
    {
        AssertCode("drwc", "6752");
    }

    [Test]
    public void Pdp8Dis_dsac()
    {
        AssertCode("dsac", "6612");
    }

    [Test]
    public void Pdp8Dis_dskd()
    {
        AssertCode("dskd", "6745");
    }

    [Test]
    public void Pdp8Dis_dske()
    {
        AssertCode("dske", "6747");
    }

    [Test]
    public void Pdp8Dis_dst()
    {
        AssertCode("dst", "7445");
    }

    [Test]
    public void Pdp8Dis_dtca()
    {
        AssertCode("dtca", "6762");
    }

    [Test]
    public void Pdp8Dis_dtla()
    {
        AssertCode("dtla", "6766");
    }

    [Test]
    public void Pdp8Dis_dtra()
    {
        AssertCode("dtra", "6761");
    }

    [Test]
    public void Pdp8Dis_dtrb()
    {
        AssertCode("dtrb", "6772");
    }

    [Test]
    public void Pdp8Dis_dtsf()
    {
        AssertCode("dtsf", "6771");
    }

    [Test]
    public void Pdp8Dis_dtxa()
    {
        AssertCode("dtxa", "6764");
    }

    [Test]
    public void Pdp8Dis_dtxb()
    {
        AssertCode("dtxb", "6774");
    }

    [Test]
    public void Pdp8Dis_dvi()
    {
        AssertCode("dvi", "7407");
    }

    [Test]
    public void Pdp8Dis_dxac()
    {
        AssertCode("dxac", "6645");
    }

    [Test]
    public void Pdp8Dis_dxal()
    {
        AssertCode("dxal", "6643");
    }

    [Test]
    public void Pdp8Dis_epi()
    {
        AssertCode("epi", "6103");
    }

    [Test]
    public void Pdp8Dis_gcrf()
    {
        AssertCode("gcrf", "6372");
    }

    [Test]
    public void Pdp8Dis_gctf()
    {
        AssertCode("gctf", "6362");
    }

    [Test]
    public void Pdp8Dis_gcuser()
    {
        AssertCode("gcuser", "6564");
    }

    [Test]
    public void Pdp8Dis_glk()
    {
        AssertCode("glk", "7204");
    }

    [Test]
    public void Pdp8Dis_grdb()
    {
        AssertCode("grdb", "6374");
    }

    [Test]
    public void Pdp8Dis_grsf()
    {
        AssertCode("grsf", "6371");
    }

    [Test]
    public void Pdp8Dis_gtf()
    {
        AssertCode("gtf", "6004");
    }

    [Test]
    public void Pdp8Dis_gtsf()
    {
        AssertCode("gtsf", "6361");
    }

    [Test]
    public void Pdp8Dis_hlt()
    {
        AssertCode("hlt", "7402");
    }

    [Test]
    public void Pdp8Dis_iac()
    {
        AssertCode("iac", "7001");
    }

    [Test]
    public void Pdp8Dis_iof()
    {
        AssertCode("iof", "6002");
    }

    [Test]
    public void Pdp8Dis_ion()
    {
        AssertCode("ion", "6001");
    }

    [Test]
    public void Pdp8Dis_isz()
    {
        AssertCode("isz i ", "2724");
    }


    [Test]
    public void Pdp8Dis_kcc()
    {
        AssertCode("kcc", "6032");
    }

    [Test]
    public void Pdp8Dis_kcf()
    {
        AssertCode("kcf", "6030");
    }

    [Test]
    public void Pdp8Dis_kie()
    {
        AssertCode("kie", "6035");
    }

    [Test]
    public void Pdp8Dis_krb()
    {
        AssertCode("krb", "6036");
    }

    [Test]
    public void Pdp8Dis_krs()
    {
        AssertCode("krs", "6034");
    }

    [Test]
    public void Pdp8Dis_ksf()
    {
        AssertCode("ksf", "6031");
    }

    [Test]
    public void Pdp8Dis_las()
    {
        AssertCode("las", "7604");
    }

    [Test]
    public void Pdp8Dis_lcar()
    {
        AssertCode("lcar", "6703");
    }

    [Test]
    public void Pdp8Dis_lcmr()
    {
        AssertCode("lcmr", "6705");
    }

    [Test]
    public void Pdp8Dis_ldbr()
    {
        AssertCode("ldbr", "6707");
    }

    [Test]
    public void Pdp8Dis_lfgr()
    {
        AssertCode("lfgr", "6706");
    }

    [Test]
    public void Pdp8Dis_lsr()
    {
        AssertCode("lsr", "7417");
    }

    [Test]
    public void Pdp8Dis_lwcr()
    {
        AssertCode("lwcr", "6701");
    }

    [Test]
    public void Pdp8Dis_mins()
    {
        AssertCode("mins", "6125");
    }

    [Test]
    public void Pdp8Dis_mint()
    {
        AssertCode("mint", "6115");
    }

    [Test]
    public void Pdp8Dis_mkcc()
    {
        AssertCode("mkcc", "6112");
    }

    [Test]
    public void Pdp8Dis_mkrs()
    {
        AssertCode("mkrs", "6114");
    }

    [Test]
    public void Pdp8Dis_mksf()
    {
        AssertCode("mksf", "6111");
    }

    [Test]
    public void Pdp8Dis_mqa()
    {
        AssertCode("mqa", "7501");
    }

    [Test]
    public void Pdp8Dis_mql()
    {
        AssertCode("mql", "7421");
    }

    [Test]
    public void Pdp8Dis_mtaf()
    {
        AssertCode("mtaf", "6712");
    }

    [Test]
    public void Pdp8Dis_mtcf()
    {
        AssertCode("mtcf", "6122");
    }

    [Test]
    public void Pdp8Dis_mtcm()
    {
        AssertCode("mtcm", "6714");
    }

    [Test]
    public void Pdp8Dis_mtcr()
    {
        AssertCode("mtcr", "6711");
    }

    [Test]
    public void Pdp8Dis_mtgo()
    {
        AssertCode("mtgo", "6722");
    }

    [Test]
    public void Pdp8Dis_mtkf()
    {
        AssertCode("mtkf", "6123");
    }

    [Test]
    public void Pdp8Dis_mtlc()
    {
        AssertCode("mtlc", "6716");
    }

    [Test]
    public void Pdp8Dis_mton()
    {
        AssertCode("mton", "6117");
    }

    [Test]
    public void Pdp8Dis_mtpc()
    {
        AssertCode("mtpc", "6124");
    }

    [Test]
    public void Pdp8Dis_mtpf()
    {
        AssertCode("mtpf", "6113");
    }

    [Test]
    public void Pdp8Dis_mtrc()
    {
        AssertCode("mtrc", "6724");
    }

    [Test]
    public void Pdp8Dis_mtrs()
    {
        AssertCode("mtrs", "6127");
    }

    [Test]
    public void Pdp8Dis_mtrs_6706()
    {
        AssertCode("mtrs", "6706");
    }

    [Test]
    public void Pdp8Dis_mtsf()
    {
        AssertCode("mtsf", "6121");
    }

    [Test]
    public void Pdp8Dis_mtsf_6701()
    {
        AssertCode("mtsf", "6701");
    }

    [Test]
    public void Pdp8Dis_mttr()
    {
        AssertCode("mttr", "6721");
    }

    [Test]
    public void Pdp8Dis_muy()
    {
        AssertCode("muy", "7405");
    }

    [Test]
    public void Pdp8Dis_nmi()
    {
        AssertCode("nmi", "7411");
    }

    [Test]
    public void Pdp8Dis_none()
    {
        AssertCode("none", "6116");
    }

    [Test]
    public void Pdp8Dis_none_6126()
    {
        AssertCode("none", "6126");
    }

    [Test]
    public void Pdp8Dis_nop()
    {
        AssertCode("nop", "7000");
    }

    [Test]
    public void Pdp8Dis_nop_7401()
    {
        AssertCode("nop", "7401");
    }

    [Test]
    public void Pdp8Dis_osr()
    {
        AssertCode("osr", "7404");
    }

    [Test]
    public void Pdp8Dis_pce()
    {
        AssertCode("pce", "6020");
    }

    [Test]
    public void Pdp8Dis_pcf()
    {
        AssertCode("pcf", "6022");
    }

    [Test]
    public void Pdp8Dis_pcie()
    {
        AssertCode("pcie", "6667");
    }

    [Test]
    public void Pdp8Dis_pclf_pstb()
    {
        AssertCode("pclf pstb", "6666");
    }

    [Test]
    public void Pdp8Dis_pclf()
    {
        AssertCode("pclf", "6662");
    }

    [Test]
    public void Pdp8Dis_plce()
    {
        AssertCode("plce", "6500");
    }

    [Test]
    public void Pdp8Dis_plcf_pllr()
    {
        AssertCode("plcf pllr", "6506");
    }

    [Test]
    public void Pdp8Dis_plcf()
    {
        AssertCode("plcf", "6502");
    }

    [Test]
    public void Pdp8Dis_pllr()
    {
        AssertCode("pllr", "6504");
    }

    [Test]
    public void Pdp8Dis_plpd()
    {
        AssertCode("plpd", "6505");
    }

    [Test]
    public void Pdp8Dis_plpu()
    {
        AssertCode("plpu", "6503");
    }

    [Test]
    public void Pdp8Dis_pls()
    {
        AssertCode("pls", "6026");
    }

    [Test]
    public void Pdp8Dis_plse()
    {
        AssertCode("plse", "6507");
    }

    [Test]
    public void Pdp8Dis_plsf()
    {
        AssertCode("plsf", "6501");
    }

    [Test]
    public void Pdp8Dis_ppc()
    {
        AssertCode("ppc", "6024");
    }

    [Test]
    public void Pdp8Dis_psf()
    {
        AssertCode("psf", "6021");
    }

    [Test]
    public void Pdp8Dis_psie()
    {
        AssertCode("psie", "6665");
    }

    [Test]
    public void Pdp8Dis_pske()
    {
        AssertCode("pske", "6663");
    }

    [Test]
    public void Pdp8Dis_pskf()
    {
        AssertCode("pskf", "6661");
    }

    [Test]
    public void Pdp8Dis_pstb()
    {
        AssertCode("pstb", "6664");
    }

    [Test]
    public void Pdp8Dis_ral()
    {
        AssertCode("ral", "7004");
    }

    [Test]
    public void Pdp8Dis_rar()
    {
        AssertCode("rar", "7010");
    }

    [Test]
    public void Pdp8Dis_rcar()
    {
        AssertCode("rcar", "6713");
    }

    [Test]
    public void Pdp8Dis_rcc()
    {
        AssertCode("rcc", "6016");
    }

    [Test]
    public void Pdp8Dis_rccb()
    {
        AssertCode("rccb", "6116");
    }

    [Test]
    public void Pdp8Dis_rccv()
    {
        AssertCode("rccv", "6113");
    }

    [Test]
    public void Pdp8Dis_rcf()
    {
        AssertCode("rcf", "6022");
    }

    [Test]
    public void Pdp8Dis_rcgb()
    {
        AssertCode("rcgb", "6114");
    }

    [Test]
    public void Pdp8Dis_rclc()
    {
        AssertCode("rclc", "6115");
    }

    [Test]
    public void Pdp8Dis_rcmr()
    {
        AssertCode("rcmr", "6715");
    }

    [Test]
    public void Pdp8Dis_rcni()
    {
        AssertCode("rcni", "6637");
    }

    [Test]
    public void Pdp8Dis_rcno()
    {
        AssertCode("rcno", "6635");
    }

    [Test]
    public void Pdp8Dis_rcra()
    {
        AssertCode("rcra", "6632");
    }

    [Test]
    public void Pdp8Dis_rcrb()
    {
        AssertCode("rcrb", "6634");
    }

    [Test]
    public void Pdp8Dis_rcrc()
    {
        AssertCode("rcrc", "6636");
    }

    [Test]
    public void Pdp8Dis_rcrd()
    {
        AssertCode("rcrd", "6674");
    }

    [Test]
    public void Pdp8Dis_rcrh()
    {
        AssertCode("rcrh", "6112");
    }

    [Test]
    public void Pdp8Dis_rcrl()
    {
        AssertCode("rcrl", "6111");
    }

    [Test]
    public void Pdp8Dis_rcsd()
    {
        AssertCode("rcsd", "6671");
    }

    [Test]
    public void Pdp8Dis_rcse()
    {
        AssertCode("rcse", "6672");
    }

    [Test]
    public void Pdp8Dis_rcsf()
    {
        AssertCode("rcsf", "6631");
    }

    [Test]
    public void Pdp8Dis_rcsi()
    {
        AssertCode("rcsi", "6675");
    }

    [Test]
    public void Pdp8Dis_rctf()
    {
        AssertCode("rctf", "6677");
    }

    [Test]
    public void Pdp8Dis_rctv()
    {
        AssertCode("rctv", "6110");
    }

    [Test]
    public void Pdp8Dis_rdbr()
    {
        AssertCode("rdbr", "6717");
    }

    [Test]
    public void Pdp8Dis_rdf()
    {
        AssertCode("rdf", "6214");
    }

    [Test]
    public void Pdp8Dis_rfc_rrb()
    {
        AssertCode("rfc rrb", "6016");
    }

    [Test]
    public void Pdp8Dis_rfc()
    {
        AssertCode("rfc", "6014");
    }

    [Test]
    public void Pdp8Dis_rfsr()
    {
        AssertCode("rfsr", "6716");
    }

    [Test]
    public void Pdp8Dis_rft()
    {
        AssertCode("rft", "6005");
    }

    [Test]
    public void Pdp8Dis_rib()
    {
        AssertCode("rib", "6234");
    }

    [Test]
    public void Pdp8Dis_rif()
    {
        AssertCode("rif", "6224");
    }

    [Test]
    public void Pdp8Dis_rmf()
    {
        AssertCode("rmf", "6244");
    }

    [Test]
    public void Pdp8Dis_rmsr()
    {
        AssertCode("rmsr", "6714");
    }

    [Test]
    public void Pdp8Dis_rpe()
    {
        AssertCode("rpe", "6010");
    }

    [Test]
    public void Pdp8Dis_rrb()
    {
        AssertCode("rrb", "6012");
    }

    [Test]
    public void Pdp8Dis_rsf()
    {
        AssertCode("rsf", "6011");
    }

    [Test]
    public void Pdp8Dis_rtf()
    {
        AssertCode("rtf", "6005");
    }

    [Test]
    public void Pdp8Dis_rtl()
    {
        AssertCode("rtl", "7006");
    }

    [Test]
    public void Pdp8Dis_rtr()
    {
        AssertCode("rtr", "7012");
    }

    [Test]
    public void Pdp8Dis_rwcr()
    {
        AssertCode("rwcr", "6711");
    }

    [Test]
    public void Pdp8Dis_sca_cla()
    {
        AssertCode("sca cla", "7641");
    }

    [Test]
    public void Pdp8Dis_sca()
    {
        AssertCode("sca", "7441");
    }

    [Test]
    public void Pdp8Dis_scl()
    {
        AssertCode("scl", "7403");
    }

    [Test]
    public void Pdp8Dis_scsd()
    {
        AssertCode("scsd", "6406");
    }

    [Test]
    public void Pdp8Dis_scsi()
    {
        AssertCode("scsi", "6401");
    }

    [Test]
    public void Pdp8Dis_sgrr()
    {
        AssertCode("sgrr", "6404");
    }

    [Test]
    public void Pdp8Dis_sgt()
    {
        AssertCode("sgt", "6006");
    }

    [Test]
    public void Pdp8Dis_sgtt()
    {
        AssertCode("sgtt", "6405");
    }

    [Test]
    public void Pdp8Dis_shl()
    {
        AssertCode("shl", "7413");
    }

    [Test]
    public void Pdp8Dis_sint()
    {
        AssertCode("sint", "6254");
    }

    [Test]
    public void Pdp8Dis_skb()
    {
        AssertCode("skb", "7471");
    }

    [Test]
    public void Pdp8Dis_skcb()
    {
        AssertCode("skcb", "6722");
    }

    [Test]
    public void Pdp8Dis_skef()
    {
        AssertCode("skef", "6721");
    }

    [Test]
    public void Pdp8Dis_skjd()
    {
        AssertCode("skjd", "6723");
    }

    [Test]
    public void Pdp8Dis_skon()
    {
        AssertCode("skon", "6000");
    }

    [Test]
    public void Pdp8Dis_skp()
    {
        AssertCode("skp", "7410");
    }

    [Test]
    public void Pdp8Dis_sktr()
    {
        AssertCode("sktr", "6724");
    }

    [Test]
    public void Pdp8Dis_slcc()
    {
        AssertCode("slcc", "6412");
    }

    [Test]
    public void Pdp8Dis_slfl()
    {
        AssertCode("slfl", "6413");
    }

    [Test]
    public void Pdp8Dis_sma_cla()
    {
        AssertCode("sma cla", "7700");
    }

    [Test]
    public void Pdp8Dis_sma_snl()
    {
        AssertCode("sma snl", "7520");
    }

    [Test]
    public void Pdp8Dis_sma_sza()
    {
        AssertCode("sma sza", "7540");
    }

    [Test]
    public void Pdp8Dis_sma()
    {
        AssertCode("sma", "7500");
    }

    [Test]
    public void Pdp8Dis_smp_cmp()
    {
        AssertCode("smp cmp", "6105");
    }

    [Test]
    public void Pdp8Dis_smp()
    {
        AssertCode("smp", "6101");
    }

    [Test]
    public void Pdp8Dis_sna_cla()
    {
        AssertCode("sna cla", "7650");
    }

    [Test]
    public void Pdp8Dis_sna_szl()
    {
        AssertCode("sna szl", "7470");
    }

    [Test]
    public void Pdp8Dis_sna()
    {
        AssertCode("sna", "7450");
    }

    [Test]
    public void Pdp8Dis_snl()
    {
        AssertCode("snl", "7420");
    }

    [Test]
    public void Pdp8Dis_spa_cla()
    {
        AssertCode("spa cla", "7710");
    }

    [Test]
    public void Pdp8Dis_spa_sna()
    {
        AssertCode("spa sna", "7550");
    }

    [Test]
    public void Pdp8Dis_spa_szl()
    {
        AssertCode("spa szl", "7530");
    }

    [Test]
    public void Pdp8Dis_spa()
    {
        AssertCode("spa", "7510");
    }

    [Test]
    public void Pdp8Dis_spl()
    {
        AssertCode("spl", "6102");
    }

    [Test]
    public void Pdp8Dis_spo()
    {
        AssertCode("spo", "6107");
    }

    [Test]
    public void Pdp8Dis_srcd()
    {
        AssertCode("srcd", "6417");
    }

    [Test]
    public void Pdp8Dis_srq()
    {
        AssertCode("srq", "6003");
    }

    [Test]
    public void Pdp8Dis_srs1()
    {
        AssertCode("srs1", "6415");
    }

    [Test]
    public void Pdp8Dis_srs2()
    {
        AssertCode("srs2", "6414");
    }

    [Test]
    public void Pdp8Dis_srta()
    {
        AssertCode("srta", "6407");
    }

    [Test]
    public void Pdp8Dis_ssbe()
    {
        AssertCode("ssbe", "6416");
    }

    [Test]
    public void Pdp8Dis_ssca()
    {
        AssertCode("ssca", "6411");
    }

    [Test]
    public void Pdp8Dis_sscd()
    {
        AssertCode("sscd", "6400");
    }

    [Test]
    public void Pdp8Dis_ssrg()
    {
        AssertCode("ssrg", "6410");
    }

    [Test]
    public void Pdp8Dis_ssro()
    {
        AssertCode("ssro", "6402");
    }

    [Test]
    public void Pdp8Dis_ssto()
    {
        AssertCode("ssto", "6403");
    }

    [Test]
    public void Pdp8Dis_sta()
    {
        AssertCode("sta", "7240");
    }

    [Test]
    public void Pdp8Dis_stl()
    {
        AssertCode("stl", "7120");
    }

    [Test]
    public void Pdp8Dis_suf()
    {
        AssertCode("suf", "6274");
    }

    [Test]
    public void Pdp8Dis_swab()
    {
        AssertCode("swab", "7431");
    }

    [Test]
    public void Pdp8Dis_swba()
    {
        AssertCode("swba", "7447");
    }

    [Test]
    public void Pdp8Dis_swp()
    {
        AssertCode("swp", "7521");
    }

    [Test]
    public void Pdp8Dis_sza_cla()
    {
        AssertCode("sza cla", "7640");
    }

    [Test]
    public void Pdp8Dis_sza_snl()
    {
        AssertCode("sza snl", "7460");
    }

    [Test]
    public void Pdp8Dis_sza()
    {
        AssertCode("sza", "7440");
    }

    [Test]
    public void Pdp8Dis_szl()
    {
        AssertCode("szl", "7430");
    }

    [Test]
    public void Pdp8Dis_tad()
    {
        AssertCode("tad", "1042");
    }

    [Test]
    public void Pdp8Dis_tcf()
    {
        AssertCode("tcf", "6042");
    }

    [Test]
    public void Pdp8Dis_tfl()
    {
        AssertCode("tfl", "6040");
    }

    [Test]
    public void Pdp8Dis_tls()
    {
        AssertCode("tls", "6046");
    }

    [Test]
    public void Pdp8Dis_tpc()
    {
        AssertCode("tpc", "6044");
    }

    [Test]
    public void Pdp8Dis_tsf()
    {
        AssertCode("tsf", "6041");
    }

    [Test]
    public void Pdp8Dis_tsk()
    {
        AssertCode("tsk", "6045");
    }

    [Test]
    public void Pdp8Dis_uddi()
    {
        AssertCode("uddi", "6365");
    }

    [Test]
    public void Pdp8Dis_udei()
    {
        AssertCode("udei", "6364");
    }

    [Test]
    public void Pdp8Dis_udla()
    {
        AssertCode("udla", "6363");
    }

    [Test]
    public void Pdp8Dis_udld()
    {
        AssertCode("udld", "6367");
    }

    [Test]
    public void Pdp8Dis_udls()
    {
        AssertCode("udls", "6357");
    }

    [Test]
    public void Pdp8Dis_udra()
    {
        AssertCode("udra", "6356");
    }

    [Test]
    public void Pdp8Dis_udrd()
    {
        AssertCode("udrd", "6366");
    }

    [Test]
    public void Pdp8Dis_udsc()
    {
        AssertCode("udsc", "6353");
    }

    [Test]
    public void Pdp8Dis_udsf()
    {
        AssertCode("udsf", "6361");
    }

    [Test]
    public void Pdp8Dis_udss()
    {
        AssertCode("udss", "6351");
    }

    [Test]
    public void Pdp8Dis_wtmn()
    {
        AssertCode("wtmn", "6064");
    }

    [Test]
    public void Pdp8Dis_wtrs()
    {
        AssertCode("wtrs", "6072");
    }

    [Test]
    public void Pdp8Dis_wtrx()
    {
        AssertCode("wtrx", "6052");
    }

    [Test]
    public void Pdp8Dis_wtsc()
    {
        AssertCode("wtsc", "6054");
    }

    [Test]
    public void Pdp8Dis_wtse()
    {
        AssertCode("wtse", "6074");
    }



    string q =
@"
nop,7000
iac,7001
ral,7004
rtl,7006
rar,7010
rtr,7012
bsw,7002
cml,7020
cma,7040
cia,7041
cll,7100
cll ral,7104
cll rtl,7106
cll rar,7110
stl,7120
cla,7200
cla iac,7201
glk,7204
cla cll,7300
sta,7240
hlt,7402
osr,7404
skp,7410
snl,7420
szl,7430
sza,7440
sna,7450,
sza snl,7460
sna szl,7470
sma,7500
spa,7510
sma snl,7520
spa szl,7530
sma sza,7540
spa sna,7550
cla,7600
las,7604
sza cla,7640
sna cla,7650
sma cla,7700
spa cla,7710
nop,7401
mql,7421
mqa,7501
swp,7521
cla,7601
cam,7621
acl,7701
cla swp,7721
ion,6001
iof,6002
skon,6000
srq,6003
gtf,6004
rtf,6005
sgt,6006
caf,6007

// KM8-E memory extension

gtf,6004
rft,6005
cdf,62n1
cif,62n2
cdi,62n3
rdf,6214
rif,6224
rib,6234
rmf,6244

// Mode switching instructions
swab,7431
swba,7447
skb,7471

// Standard instructions

cam,7621
mqa,7501
acl,7701
mql,7421
swp,7521

// Mode A instructions

sca,7441
sca cla,7641
scl,7403
muy,7405
dvi,7407
nmi,7411
shl,7413
asr,7415
lsr,7417

// Mode B instructions

acs,7403
muy,7405
dvi,7407
nmi,7411
shl,7413
asr,7415
lsr,7417

// Double precision instructions

dad,7443
dst,7445
dpic,7573
dcm,7575
dpsz,7451

// TTY keyboard/reader

kcf,6030
ksf,6031
kcc,6032
krs,6034
kie,6035
krb,6036

// TTY printer/punch

tfl,6040
tsf,6041
tcf,6042
tpc,6044
tsk,6045
tls,6046

// PR8-E Paper tape readers

rpe,6010
rsf,6011
rrb,6012
rfc,6014
rcc,6016
pce,6020

// PP8-E Paper tape punch

rpe,6010
pce,6020
psf,6021
rcf,6022
ppc,6024
pls,6026

// PC8-E Reader / Punch

rpe,6010
rsf,6011
rrb,6012
rfc,6014
rfc rrb,6016
pce,6020
psf,6021
pcf,6022
ppc,6024
pls,6026

// TC08-P DECtape Control

dtra,6761
dtca,6762
dtxa,6764
dtla,6766
dtsf,6771
dtrb,6772
dtxb,6774

// TC58 DECmagtape system

mtsf,6701
mtcr,6711
mttr,6721
mtaf,6712
mtrc,6724
mtcm,6714
mtlc,6716
_osr,6704
mtrs,6706
mtgo,6722
_cac,6702

// RK08-P Control and RK01 Disk Drive and Control

dlda,6731
dldc,6732
dldr,6733
drda,6734
dldw,6735
drdc,6736
dchp,6737
drds,6741
dcls,6742
dmnt,6743
dskd,6745
dske,6747
dcla,6751
drwc,6752
dlwc,6753
dlca,6755
drca,6757

// DF32-D Disk file and Control

dcma,6601
dmar,6603
dmaw,6605
dcea,6611
dsac,6612
deal,6615
deac,6616
dfse,6621
dfsc,6622
dmac,6626

// RF08 Disk File

dcim,6611
diml,6615
dima,6616
dfse,6621
disk,6623
dcxa,6641
dxal,6643
dxac,6645
dmmt,6646

// TM8-E/F control

lwcr,6701
cwcr,6702
lcar,6703
ccar,6704
lcmr,6705
lfgr,6706
ldbr,6707
rwcr,6711
clt,6712
rcar,6713
rmsr,6714
rcmr,6715
rfsr,6716
rdbr,6717
skef,6721
skcb,6722
skjd,6723
sktr,6724
clf,6725

// LE-8 Line printer

pskf,6661
pclf,6662
pske,6663
pstb,6664
psie,6665
pclf pstb,6666
pcie,6667

// CR8-E Card reader and control

rcsf,6631
rcra,6632
rcrb,6634
rcno,6635
rcrc,6636
rcni,6637
rcsd,6671
rcse,6672
rcrd,6674
rcsi,6675
rctf,6677

// XY8-E Incremental plotter control

plce,6500
plsf,6501
plcf,6502
plpu,6503
pllr,6504
plpd,6505
plcf pllr,6506
plse,6507

// VC8-E CRT display control

dilc,6050
dicd,6051
disd,6052
dilx,6053
dily,6054
dixy,6055
dile,6056
dire,6057

// VW01 Writing tablet

wtsc,6054
wtrx,6052
wtrs,6072
wtse,6074
wtmn,6064

// DC02-F 8-channel multiple teletype control

mtpf,6113
mint,6115
mton,6117
mtkf,6123
mins,6125
mtrs,6127
mksf,6111
mkcc,6112
mkrs,6114
none,6116
mtsf,6121
mtcf,6122
mtpc,6124
none,6126

// BB08-P general purpose interface unit

gtsf,6361
gctf,6362
gcuser,6564
grsf,6371
gcrf,6372
grdb,6374

// Universal Digit Controller

udss,6351
udsc,6353
udra,6356
udls,6357
udsf,6361
udla,6363
udei,6364
uddi,6365
udrd,6366
udld,6367

// DR8-EA 12-Channel buffered digital I/O

dbdi,65x0
dbei,65x1
dbsk,65x2
dbci,65x3
dbri,65x4
dbco,65x5
dbso,65x6
dbro,65x7

// MP8E-Memory parity

dpi,6100
smp,6101
epi,6103
cmp,6104
smp cmp,6105
cep,6106
spo,6107

// Synchronous Modem Interface

sgtt,6405
sgrr,6404
sscd,6400
scsd,6406
ssro,6402
scsi,6401
srta,6407
slcc,6412
ssrg,6410
ssca,6411
srs2,6414
srs1,6415
slfl,6413
ssbe,6416
srcd,6417
ssto,6403

// KM8-E Time-share

cint,6204
sint,6254
cuf,6264
suf,6274

// DK8-EP Programmable real time clock

clze,6130
clsk,6131
cloe,6132
clab,6133
clen,6134
clsa,6135
clba,6136
clca,6137

// DK8-EA Line frequency clock

clei,6131
cldi,6132
clsk,6133

// DK8-EC Crystal clock

clei,6131
cldi,6132
clsk,6133

// KP8-E Power fail detect

spl,6102

// DP8-EP Reduncy check option

rctv,6110
rcrl,6111
rcrh,6112
rccv,6113
rcgb,6114
rclc,6115
rccb,6116

// DR8-E Interprocessor buffer

dbrf,65x1
dbrd,65x2
dbtf,65x3
dbtd,65x4
dbei,65x5
dbdi,65x6
dbcd,65x7";











}
