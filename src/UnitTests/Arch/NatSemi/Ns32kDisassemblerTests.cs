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
using Reko.Arch.NatSemi;
using Reko.Core;

namespace Reko.UnitTests.Arch.NatSemi;

[TestFixture]
public class Ns32kDisassemblerTests : DisassemblerTestBase<Ns32kInstruction>
{
    private readonly Ns32kArchitecture arch;

    public Ns32kDisassemblerTests()
    {
        this.arch = new Ns32kArchitecture(CreateServiceContainer(), "ns32k", []);
        this.LoadAddress = Address.Ptr32(0x0010_0000);
    }

    public override IProcessorArchitecture Architecture => arch;


    public override Address LoadAddress { get; }

    private void AssertCode(string sExpected, string hexBytes)
    {
        var instr = base.DisassembleHexBytes(hexBytes);
        Assert.AreEqual(sExpected, instr.ToString());
    }

    [Test]
    public void Ns32kDis_absd()
    {
        AssertCode("absd\t8(sp),r7", "4E F3 C9 08");
    }

    [Test]
    public void Ns32kDis_absf()
    {
        AssertCode("absf\tf0,f2", "BE B5 00");
    }

    [Test]
    public void Ns32kDis_acbb()
    {
        AssertCode("acbb\t-1,r0,000FFFFD", "CC 07 7D");
    }

    [Test]
    public void Ns32kDis_addb()
    {
        AssertCode("addb\tr0,r1", "40 00");
    }

    [Test]
    public void Ns32kDis_addcb()
    {
        AssertCode("addcb\tH'20,r0", "10 A0 20");
    }

    [Test]
    public void Ns32kDis_addd()
    {
        AssertCode("addd\t4(sb),-4(fp)", "03 D6 04 7C");
    }

    [Test]
    public void Ns32kDis_addf()
    {
        AssertCode("addf\tf0,f7", "BE C1 01");
    }

    [Test]
    public void Ns32kDis_addl()
    {
        AssertCode("addl\tf2,16(sb)","BE 80 16 10");
    }

    [Test]
    public void Ns32kDis_addpb()
    {
        AssertCode("addpb\t5(sb),tos", "4E FC D5 05");
    }

    [Test]
    public void Ns32kDis_addqb()
    {
        AssertCode("addqb\t-8,r0", "0C 04");
    }

    [Test]
    public void Ns32kDis_addr()
    {
        AssertCode("addr\t4(fp),r0", "27 C0 04");
    }

    [Test]
    public void Ns32kDis_adjspd()
    {
        AssertCode("adjspd\t-4(fp)", "7F C5 7C");
    }

    [Test]
    public void Ns32kDis_andb()
    {
        AssertCode("andb\tr0,r1", "68 00");
    }

    [Test]
    public void Ns32kDis_ashb()
    {
        AssertCode("ashb\ttos,16(sb)", "4E 84 BE 10");
    }

    [Test]
    public void Ns32kDis_beq()
    {
        AssertCode("beq\t000FFFFE", "0A 7E");
    }

    [Test]
    public void Ns32kDis_bne()
    {
        AssertCode("bne\t000FFFFE", "1A 7E");
    }

    [Test]
    public void Ns32kDis_bcs()
    {
        AssertCode("bcs\t000FFFFE", "2A 7E");
    }

    [Test]
    public void Ns32kDis_bcc()
    {
        AssertCode("bcc\t000FFFFE", "3A 7E");
    }

    [Test]
    public void Ns32kDis_bhi()
    {
        AssertCode("bhi\t000FFFFE", "4A 7E");
    }

    [Test]
    public void Ns32kDis_bls()
    {
        AssertCode("bls\t000FFFFE", "5A 7E");
    }

    [Test]
    public void Ns32kDis_bgt()
    {
        AssertCode("bgt\t000FFFFE", "6A 7E");
    }

    [Test]
    public void Ns32kDis_ble()
    {
        AssertCode("ble\t000FFFFE", "7A 7E");
    }

    [Test]
    public void Ns32kDis_bfs()
    {
        AssertCode("bfs\t000FFFFE", "8A 7E");
    }

    [Test]
    public void Ns32kDis_bfc()
    {
        AssertCode("bfc\t000FFFFE", "9A 7E");
    }

    [Test]
    public void Ns32kDis_blo()
    {
        AssertCode("blo\t000FFFFE", "AA 7E");
    }

    [Test]
    public void Ns32kDis_bhs()
    {
        AssertCode("bhs\t000FFFFE", "BA 7E");
    }

    [Test]
    public void Ns32kDis_blt()
    {
        AssertCode("blt\t000FFFFE", "CA 7E");
    }

    [Test]
    public void Ns32kDis_bge()
    {
        AssertCode("bge\t000FFFFE", "DA 7E");
    }

    [Test]
    public void Ns32_bic()
    {
        AssertCode("bicb\tr0,3(sb)","88 06 03");
    }

    [Test]
    public void Ns32kDis_bicpsrb()
    {
        AssertCode("bicpsrb\tH'A2","7C A1 A2");
    }

    [Test]
    public void Ns32kDis_bicpsrw()
    {
        AssertCode("bicpsrw\tH'A200", "7D A1 A2 00");
    }

    [Test]
    public void Ns32kDis_bpt()
    {
        AssertCode("bpt", "F2");
    }

    [Test]
    public void Ns32kDis_br()
    {
        AssertCode("br\t0010000A", "EA 0A");
    }

    [Test]
    public void Ns32kDis_bsr()
    {
        AssertCode("bsr\t00100010", "02 10");
    }

    [Test]
    public void Ns32kDis_caseb()
    {
        AssertCode("caseb\t00100004[r7:b]", "7C E7 DF 04");
    }

    [Test]
    public void Ns32kDis_cbitw()
    {
        AssertCode("cbitw\tr0,0(r1)", "4E 49 02 00");
    }

    [Test]
    public void Ns32kDis_checkb()
    {
        AssertCode("checkb\tr0,4(sb),r2", "EE 80 D0 04");
    }

    [Test]
    public void Ns32kDis_cinv()
    {
        AssertCode("cinv\td,i,a,r3", "1E A7 1B ");
        AssertCode("cinv\ti,r3", "1E 27 19");
    }

    [Test]
    public void Ns32kDis_cmpb()
    {
        AssertCode("cmpb\t7(sb),4(r0)", "04 D2 07 04");
    }

    [Test]
    public void Ns32kDis_cmpf()
    {
        AssertCode("cmpf\tf0,f2", "BE 89 00");
    }

    [Test]
    public void Ns32kDis_cmpmw()
    {
        AssertCode("cmpmw\t10(r0),16(r1),6", "CE 45 42 0A 10 06");
    }

    [Test]
    public void Ns32kDis_cmpqb()
    {
        AssertCode("cmpqb\t-8,r0", "1C 04");
    }

    [Test]
    public void Ns32kDis_cmpsb()
    {
        AssertCode("cmpsb", "0E 04 00");
    }

    [Test]
    public void Ns32kDis_comb()
    {
        AssertCode("comb\tr0,-4(fp)", "4E 34 06 7C");
    }

    [Test]
    public void Ns32kDis_cvtp()
    {
        AssertCode("cvtp\tr0,32(sb),r2", "6E 83 D0 20");
    }

    [Test]
    public void Ns32kDis_cxp()
    {
        AssertCode("cxp\t1", "22 01");
    }

    [Test]
    public void Ns32kDis_cxpd()
    {
        AssertCode("cxpd\t8(sb)", "7F D0 08");
    }

    [Test]
    public void Ns32kDis_deiw()
    {
        AssertCode("deiw\tr2,r0", "CE 2D 10");
    }

    [Test]
    public void Ns32kDis_divd()
    {
        AssertCode("divd\t-6(fp),12(sb)", "CE BF C6 7A 0C");
    }

    [Test]
    public void Ns32kDis_divl()
    {
        AssertCode("divl\t-8(fp),16(sb)", "BE A0 C6 78 10");
    }

    [Test]
    public void Ns32kDis_dotf()
    {
        AssertCode("dotf\tf2,f3", "FE CD 10");
    }

    [Test]
    public void Ns32kDis_enter()
    {
        AssertCode("enter\t[r0,r2,r7],H'10", "82 85 10");
    }

    [Test]
    public void Ns32kDis_exit()
    {
        AssertCode("exit\t[r0,r2,r7]", "92 A1");
    }

    [Test]
    [Ignore("Too complex")]
    public void Ns32kDis_extb_complex()
    {
        AssertCode("extb\tr0,10(sb),0(sb)[r1:b], 5", "2E 00 D7 D1 0A 00 05");
    }

    [Test]
    public void Ns32kDis_extsw()
    {
        AssertCode("extsw\t16(sb),r2,4,7", "CE 8D D0 10 86");
    }

    [Test]
    public void Ns32kDis_ffsb()
    {
        AssertCode("ffsb\t-4(fp),tos", "6E C4 C5 7C");
    }

    [Test]
    public void Ns32kDis_floorfb()
    {
        AssertCode("floorfb\tf0,r0", "3E 3C 00");
    }

    [Test]
    public void Ns32kDis_floorld()
    {
        AssertCode("floorld\tf2,16(sb)", "3E BB 16 10");
    }

    [Test]
    public void Ns32kDis_ibitw()
    {
        AssertCode("ibitw\tr0,1(r1)", "4E 79 02 01");
    }

    [Test]
    public void Ns32kDis_indexb()
    {
        AssertCode("indexb\tr0,20(sb),-4(fp)", "2E 04 D6 14 7C");
    }

    [Test]
    public void Ns32kDis_insw()
    {
        AssertCode("insw\tr0,r2,0(r1),7", "AE 41 12 00 07");
    }

    [Test]
    public void Ns32kDis_inssw()
    {
        AssertCode("inssw\tr2,16(sb),4,7", "CE 89 16 10 86");
    }

    [Test]
    public void Ns32kDis_jsr()
    {
        AssertCode("jsr\t0(4(sb))", "7F 96 04 00");
    }

    [Test]
    public void Ns32kDis_jump()
    {
        AssertCode("jump\t0(-8(fp))", "7F 82 78 00");
    }

    [Test]
    public void Ns32kDis_lfsr()
    {
        AssertCode("lfsr\tr0", "3E 0F 00");
    }

    [Test]
    public void Ns32kDis_lmr()
    {
        AssertCode("lmr\tptb0,r0", "1E 0B 06");
    }

    [Test]
    public void Ns32kDis_logbf()
    {
        AssertCode("logbf\tf3,f2", "FE 95 18");
    }

    [Test]
    public void Ns32kDis_lprw()
    {
        AssertCode("lprw\tmod,4(sb)", "ED D7 04");
    }

    [Test]
    public void Ns32kDis_lshb()
    {
        AssertCode("lshb\t-4(fp),8(sb)", "4E 94 C6 7C 08");
    }

    [Test]
    public void Ns32kDis_meiw()
    {
        AssertCode("meiw\tr2,10(sb)", "CE A5 16 0A");
    }

    [Test]
    public void Ns32kDis_modb()
    {
        AssertCode("modb\t4(sb),8(sb)", "CE B8 D6 04 08");
    }

    [Test]
    public void Ns32kDis_movf()
    {
        AssertCode("movf\tf0,8(sb)", "BE 85 06 08");
    }

    [Test]
    public void Ns32kDis_movd()
    {
        AssertCode("movd\tr0,8(sb)", "97 06 08");
    }

    [Test]
    public void Ns32kDis_movbf()
    {
        AssertCode("movbf\t2,f0", "3E 04 A0 02");
    }

    [Test]
    public void Ns32kDis_movdl()
    {
        AssertCode("movdl\t16(sb),f2", "3E 83 D0 10");
    }

    [Test]
    public void Ns32kDis_movfl()
    {
        AssertCode("movfl\t8(sb),f0", "3E 1B D0 08");
    }

    [Test]
    public void Ns32kDis_movlf()
    {
        AssertCode("movlf\tf0,12(sb)", "3E 96 06 0C");
    }

    [Test]
    public void Ns32kDis_movmw()
    {
        AssertCode("movmw\t10(r0),16(r1),6", "CE 41 42 0A 10 06");
    }

    [Test]
    public void Ns32kDis_movqw()
    {
        AssertCode("movqw\t7,tos", "DD BB");
    }

    [Test]
    public void Ns32_movst()
    {
        AssertCode("movst", "0E 80 00");
    }

    [Test]
    public void Ns32kDis_movsub()
    {
        AssertCode("movsub\t5(sp),9(sb)", "AE 8C CE 05 09");
    }

    [Test]
    public void Ns32kDis_movusb()
    {
        AssertCode("movusb\t9(sb),5(sp)", "AE 5C D6 09 05");
    }

    [Test]
    public void Ns32kDis_movxbw()
    {
        AssertCode("movxbw\t2(sb),r0", "CE 10 D0 02");
    }

    [Test]
    public void Ns32kDis_movxwd()
    {
        AssertCode("movxwd\tH'FE00,tos", "CE DD A5 FE 00");
    }

    [Test]
    public void Ns32kDis_movzbw()
    {
        AssertCode("movzbw\t-4(fp),r0", "CE 14 C0 7C");
    }

    [Test]
    public void Ns32kDis_muld()
    {
        AssertCode("muld\t4(-4(fp)),3(sb)", "CE A3 86 7C 04 03");
    }

    [Test]
    public void Ns32kDis_mull()
    {
        AssertCode("mull\t-8(fp),8(sb)", "BE B0 C6 78 08");
    }

    [Test]
    public void Ns32kDis_negf()
    {
        AssertCode("negf\tf0,f2", "BE 95 00");
    }

    [Test]
    public void Ns32kDis_negw()
    {
        AssertCode("negw\t4(sb),6(sb)", "4E A1 D6 04 06");
    }

    [Test]
    public void Ns32kDis_nop()
    {
        AssertCode("nop", "A2");
    }

    [Test]
    public void Ns32kDis_notw()
    {
        AssertCode("notw\t10(r1),tos", "4E E5 4D 0A");
    }

    [Test]
    public void Ns32kDis_orb()
    {
        AssertCode("orb\t-6(fp),11(sb)", "98 C6 7A 0B");
    }

    [Test]
    public void Ns32kDis_polyf()
    {
        AssertCode("polyf\tf2,f3", "FE C9 10");
    }

    [Test]
    public void Ns32kDis_quow()
    {
        AssertCode("quow\t4(sb),8(sb)", "CE B1 D6 04 08");
    }

    [Test]
    public void Ns32kDis_rdval()
    {
        AssertCode("rdval\t512(r0)", "1E 03 40 82 00");
    }

    [Test]
    public void Ns32kDis_remb()
    {
        AssertCode("remb\t4(sb),8(sb)", "CE B4 D6 04 08");
    }

    [Test]
    public void Ns32kDis_restore()
    {
        AssertCode("restore\t[r0,r2,r7]", "72 A1");
    }

    [Test]
    public void Ns32kDis_ret()
    {
        AssertCode("ret\tH'10", "12 10");
    }

    [Test]
    public void Ns32kDis_reti()
    {
        AssertCode("reti", "52");
    }

    [Test]
    public void Ns32kDis_rett()
    {
        AssertCode("rett\tH'10", "42 10");
    }


    //$TODO: signed immediates
    [Test]
    public void Ns32kDis_rotb()
    {
        AssertCode("rotb\tH'FD,16(sp)", "4E 40 A6 FD 10");
    }

    [Test]
    public void Ns32kDis_roundfb()
    {
        AssertCode("roundfb\tf0,r0", "3E 24 00");
    }

    [Test]
    public void Ns32kDis_roundld()
    {
        AssertCode("roundld\tf2,12(sb)", "3E A3 16 0C");
    }

    [Test]
    public void Ns32kDis_rxp()
    {
        AssertCode("rxp\tH'10", "32 10");
    }

    [Test]
    public void Ns32kDis_seqb()
    {
        AssertCode("seqb\tr0", "3C 00");
    }

    [Test]
    public void Ns32kDis_slow()
    {
        AssertCode("slow\t10(sb)", "3D D5 0A");
    }

    [Test]
    public void Ns32kDis_shid()
    {
        AssertCode("shid\ttos", "3F BA");
    }

    [Test]
    public void Ns32kDis_save()
    {
        AssertCode("save\t[r0,r2,r7]", "62 85");
    }

    [Test]
    public void Ns32kDis_sbitw()
    {
        AssertCode("sbitw\tr0,1(r1)", "4E 59 02 01");
    }

    [Test]
    public void Ns32kDis_scalbf()
    {
        AssertCode("scalbf\tf3,f2", "FE 91 18");
    }

    [Test]
    public void Ns32kDis_setcfg()
    {
        AssertCode("setcfg\t[i,f,m]", "0E 8B 03");
    }

    [Test]
    public void Ns32kDis_sfsr()
    {
        AssertCode("sfsr\ttos", "3E F7 05");
    }

    [Test]
    public void Ns32kDis_skpsb()
    {
        AssertCode("skpsb\tu", "0E 0C 06");
    }

    [Test]
    public void Ns32kDis_smr()
    {
        AssertCode("smr\tptb0,r0", "1E 0F 06");
    }

    [Test]
    public void Ns32kDis_sprw()
    {
        AssertCode("sprw\tmod,4(sb)", "AD D7 04");
    }

    [Test]
    public void Ns32kDis_subl()
    {
        AssertCode("subl\tf2,16(sb)", "BE 90 16 10");
    }

    [Test]
    public void Ns32kDis_subd()
    {
        AssertCode("subd\t4(sb),20(sb)", "A3 D6 04 14");
    }

    [Test]
    public void Ns32kDis_subcw()
    {
        AssertCode("subcw\ttos,-8(fp)", "31 BE 78");
    }

    [Test]
    public void Ns32kDis_subpd()
    {
        AssertCode("subpd\tH'99,r1", "4E 6F A0 00 00 00 99");
    }

    [Test]
    public void Ns32kDis_tbitw()
    {
        AssertCode("tbitw\tr0,0(r1)", "75 02 00");
    }

    [Test]
    public void Ns32kDis_truncfb()
    {
        AssertCode("truncfb\tf0,r0", "3E 2C 00");
    }

    [Test]
    public void Ns32kDis_truncld()
    {
        AssertCode("truncld\tf2,8(sb)", "3E AB 16 08");
    }

    [Test]
    public void Ns32kDis_wait()
    {
        AssertCode("wait", "B2");
    }

    [Test]
    public void Ns32kDis_wrval()
    {
        AssertCode("wrval\t512(r0)", "1E 07 40 82 00");
    }

    [Test]
    public void Ns32kDis_xorb()
    {
        AssertCode("xorb\t-8(fp),-4(fp)", "38 C6 78 7C");
    }
}
