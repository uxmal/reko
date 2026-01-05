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
using Reko.Arch.Pdp.Memory;
using Reko.Arch.Pdp.Pdp1;
using Reko.Core;

namespace Reko.UnitTests.Arch.Pdp.Pdp1;

[TestFixture]
public class Pdp1DisassemblerTests : DisassemblerTestBase<Pdp1Instruction>
{
    private readonly Pdp1Architecture arch;
    public Pdp1DisassemblerTests()
    {
        this.arch = new Pdp1Architecture(CreateServiceContainer(), "pdp1", []);
        this.LoadAddress = Pdp10Architecture.Ptr18(0x100);
    }
    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress { get; }

    private void AssertCode(string sExp, string octalWord)
    {
        uint word = (uint) Pdp10Architecture.OctalStringToWord(octalWord);
        var mem = new Word18MemoryArea(LoadAddress, [word]);
        var i = Disassemble(mem);
        Assert.AreEqual(sExp, i.ToString());
    }

    [Test]
    public void Pdp1Dis_add()
    {
        AssertCode("add\t000040", "400040");
    }

    [Test]
    public void Pdp1Dis_add_i()
    {
        AssertCode("add.i\t000010", "410010");
    }

    [Test]
    public void Pdp1Dis_and()
    {
        AssertCode("and\t007000", "027000");
    }

    [Test]
    public void Pdp1Dis_and_i()
    {
        AssertCode("and.i\t006000", "036000");
    }

    [Test]
    public void Pdp1Dis_cal()
    {
        AssertCode("cal\t004000", "164000");
    }

    [Test]
    public void Pdp1Dis_cma()
    {
        AssertCode("cma", "761000");
    }

    [Test]
    public void Pdp1Dis_dac()
    {
        AssertCode("dac\t004000", "244000");
    }

    [Test]
    public void Pdp1Dis_dac_i()
    {
        AssertCode("dac.i\t000050", "250050");
    }

    [Test]
    public void Pdp1Dis_dap()
    {
        AssertCode("dap\t000000", "260000");
    }

    [Test]
    public void Pdp1Dis_dap_i()
    {
        AssertCode("dap.i\t002000", "272000");
    }

    [Test]
    public void Pdp1Dis_dip()
    {
        AssertCode("dip\t002222", "302222");
    }

    [Test]
    public void Pdp1Dis_dip_i()
    {
        AssertCode("dip.i\t006666", "316666");
    }

    [Test]
    public void Pdp1Dis_dio()
    {
        AssertCode("dio\t000660", "320660");
    }

    [Test]
    public void Pdp1Dis_dio_i()
    {
        AssertCode("dio.i\t000550", "330550");
    }

    [Test]
    public void Pdp1Dis_div()
    {
        AssertCode("div\t000030", "560030");
    }

    [Test]
    public void Pdp1Dis_div_i()
    {
        AssertCode("div.i\t000050", "570050");
    }

    [Test]
    public void Pdp1Dis_dzm()
    {
        AssertCode("dzm\t004444", "344444");
    }

    [Test]
    public void Pdp1Dis_dzm_i()
    {
        AssertCode("dzm.i\t000055", "350055");
    }

    [Test]
    public void Pdp1Dis_esm()
    {
        AssertCode("esm", "720055");
    }

    [Test]
    public void Pdp1Dis_idx()
    {
        AssertCode("idx\t004000", "444000");
    }

    [Test]
    public void Pdp1Dis_idx_i()
    {
        AssertCode("idx.i\t007000", "457000");
    }

    [Test]
    public void Pdp1Dis_ior()
    {
        AssertCode("ior\t003000", "043000");
    }

    [Test]
    public void Pdp1Dis_ior_i()
    {
        AssertCode("ior.i\t003000", "053000");
    }

    [Test]
    public void Pdp1Dis_isp()
    {
        AssertCode("isp\t007000", "467000");
    }

    [Test]
    public void Pdp1Dis_isp_i()
    {
        AssertCode("isp.i\t002220", "472220");
    }


    [Test]
    public void Pdp1Dis_jda()
    {
        AssertCode("jda\t001234", "171234");
    }


    [Test]
    public void Pdp1Dis_jmp()
    {
        AssertCode("jmp\t006000", "606000");
    }

    [Test]
    public void Pdp1Dis_jmp_i()
    {
        AssertCode("jmp.i\t001234", "611234");
    }

    [Test]
    public void Pdp1Dis_jsp()
    {
        AssertCode("jsp\t004444", "624444");
    }

    [Test]
    public void Pdp1Dis_jsp_i()
    {
        AssertCode("jsp.i\t004242", "634242");
    }

    [Test]
    public void Pdp1Dis_lac()
    {
        AssertCode("lac\t004242", "204242");
    }

    [Test]
    public void Pdp1Dis_lac_i()
    {
        AssertCode("lac.i\t000300", "210300");
    }

    [Test]
    public void Pdp1Dis_lat()
    {
        AssertCode("lat", "762000");
    }

    [Test]
    public void Pdp1Dis_lap()
    {
        AssertCode("lap", "760100");
    }

    [Test]
    public void Pdp1Dis_hlt()
    {
        AssertCode("hlt", "760400");
    }

    [Test]
    public void Pdp1Dis_cla()
    {
        AssertCode("cla", "760200");
    }

    [Test]
    public void Pdp1Dis_clf()
    {
        AssertCode("clf\t4", "760004");
    }

    [Test]
    public void Pdp1Dis_cli()
    {
        AssertCode("cli", "764000");
    }



    [Test]
    public void Pdp1Dis_nop()
    {
        AssertCode("nop", "760000");
    }

    /* input/output instr */

    /* perforated tape reader */
    /* perforated tape punch */
    [Test]
    public void Pdp1Dis_ppa()
    {
        AssertCode("ppa", "720005");
    }

    [Test]
    public void Pdp1Dis_ppb()
    {
        AssertCode("ppb", "720006");
    }

    /* alphanumeric on-line typewriter */


    /* sequence break mode */
    [Test]
    public void Pdp1Dis_lsm()
    {
        AssertCode("lsm", "720054");
    }

    [Test]
    public void Pdp1Dis_cbs()
    {
        AssertCode("cbs", "720056");
    }

    [Test]
    public void Pdp1Dis_cks()
    {
        AssertCode("cks", "720033");
    }

    [Test]
    public void Pdp1Dis_law_pos()
    {
        AssertCode("law\t1234", "701234");
    }

    [Test]
    public void Pdp1Dis_law_neg()
    {
        AssertCode("law\t776544", "711234");
    }


    [Test]
    public void Pdp1Dis_lio()
    {
        AssertCode("lio\t006666", "226666");
    }

    [Test]
    public void Pdp1Dis_lio_i()
    {
        AssertCode("lio.i\t004321", "234321`");
    }

    [Test]
    public void Pdp1Dis_mul()
    {
        AssertCode("mul\t000030", "540030");
    }

    [Test]
    public void Pdp1Dis_mul_i()
    {
        AssertCode("mul.i\t000030", "550030");
    }

    [Test]
    public void Pdp1Dis_ral()
    {
        AssertCode("ral\t1", "661100");
    }

    [Test]
    public void Pdp1Dis_rar()
    {
        AssertCode("rar\t11", "671777");
    }

    [Test]
    public void Pdp1Dis_rcl()
    {
        AssertCode("rcl\t2", "663300");
    }

    [Test]
    public void Pdp1Dis_rcr()
    {
        AssertCode("rcr\t2", "673300");
    }

    [Test]
    public void Pdp1Dis_ril()
    {
        AssertCode("ril\t2", "662021");
    }

    [Test]
    public void Pdp1Dis_rir()
    {
        AssertCode("rir\t1", "672020");
    }

    [Test]
    public void Pdp1Dis_rpa()
    {
        AssertCode("rpa", "720001");
    }

    [Test]
    public void Pdp1Dis_rpb()
    {
        AssertCode("rpb", "720002");
    }

    [Test]
    public void Pdp1Dis_rrb()
    {
        AssertCode("rrb", "720030");
    }

    [Test]
    public void Pdp1Dis_sad()
    {
        AssertCode("sad\t001212", "501212");
    }

    [Test]
    public void Pdp1Dis_sad_i()
    {
        AssertCode("sad.i\t001234", "511234");
    }

    [Test]
    public void Pdp1Dis_sal()
    {
        AssertCode("sal\t2", "665050");
    }

    [Test]
    public void Pdp1Dis_sar()
    {
        AssertCode("sar\t2", "675500");
    }

    [Test]
    public void Pdp1Dis_sas()
    {
        AssertCode("sas\t001212", "521212");
    }

    [Test]
    public void Pdp1Dis_sas_i()
    {
        AssertCode("sas.i\t004242", "534242");
    }

    [Test]
    public void Pdp1Dis_scl()
    {
        AssertCode("scl\t3", "667700");
    }

    [Test]
    public void Pdp1Dis_scr()
    {
        AssertCode("scr\t3", "677700");
    }

    [Test]
    public void Pdp1Dis_sil()
    {
        AssertCode("sil\t2", "666600");
    }

    [Test]
    public void Pdp1Dis_sir()
    {
        AssertCode("sir\t2", "676600");
    }

    [Test]
    public void Pdp1Dis_sma()
    {
        AssertCode("sma", "640400");
    }

    [Test]
    public void Pdp1Dis_sma_i()
    {
        AssertCode("sma.i", "650400");
    }

    [Test]
    public void Pdp1Dis_spa()
    {
        AssertCode("spa", "640200");
    }

    [Test]
    public void Pdp1Dis_spa_i()
    {
        AssertCode("spa.i", "650200");
    }

    [Test]
    public void Pdp1Dis_spi()
    {
        AssertCode("spi", "642000");
    }

    [Test]
    public void Pdp1Dis_spi_i()
    {
        AssertCode("spi.i", "652000");
    }

    [Test]
    public void Pdp1Dis_stf()
    {
        AssertCode("stf\t3", "760013");
    }

    [Test]
    public void Pdp1Dis_sub()
    {
        AssertCode("sub\t000030", "420030");
    }

    [Test]
    public void Pdp1Dis_sub_i()
    {
        AssertCode("sub.i\t000030", "430030");
    }

    [Test]
    public void Pdp1Dis_sza()
    {
        AssertCode("sza", "640100");
    }

    [Test]
    public void Pdp1Dis_sza_i()
    {
        AssertCode("sza.i", "650100");
    }

    [Test]
    public void Pdp1Dis_szo()
    {
        AssertCode("szo", "641000");
    }

    [Test]
    public void Pdp1Dis_szo_i()
    {
        AssertCode("szo.i", "651000");
    }

    [Test]
    public void Pdp1Dis_szf()
    {
        AssertCode("szf\t7", "640007");
    }

    [Test]
    public void Pdp1Dis_szf_i()
    {
        AssertCode("szf.i\t7", "650007");
    }

    [Test]
    public void Pdp1Dis_szs()
    {
        AssertCode("szs\t7", "640070");
    }

    [Test]
    public void Pdp1Dis_szs_i()
    {
        AssertCode("szs.i\t7", "650070");
    }


    [Test]
    public void Pdp1Dis_tyi()
    {
        AssertCode("tyi", "720004");
    }

    [Test]
    public void Pdp1Dis_tyo()
    {
        AssertCode("tyo", "730003");
    }

    [Test]
    public void Pdp1Dis_xct()
    {
        AssertCode("xct\t004242", "104242");
    }

    [Test]
    public void Pdp1Dis_xct_i()
    {
        AssertCode("xct.i\t004242", "114242");
    }

    [Test]
    public void Pdp1Dis_xor()
    {
        AssertCode("xor\t000100", "060100");
    }

    [Test]
    public void Pdp1Dis_xor_i()
    {
        AssertCode("xor.i\t005000", "075000");
    }
}