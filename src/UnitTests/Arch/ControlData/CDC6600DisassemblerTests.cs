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
using NUnit.Framework.Internal;
using Reko.Arch.ControlData;
using Reko.Arch.ControlData.CDC6600;
using Reko.Core;
using Reko.Core.Memory;
using Reko.Core.Services;
using System;

namespace Reko.UnitTests.Arch.ControlData;

[TestFixture]
public class CDC6600DisassemblerTests : DisassemblerTestBase<CDC6600Bundle>
{
    private readonly CDC6600Architecture arch;
    private readonly Address addr;

    public CDC6600DisassemblerTests()
    {
        this.arch = new CDC6600Architecture(CreateServiceContainer(), "cdc6600", []);
        this.addr = CDC6600Architecture.Ptr(0x1000);
    }

    public override IProcessorArchitecture Architecture => arch;
    public override Address LoadAddress => addr;

    public void Cdc6600Dasm_Gen()
    {
        var bytes = new byte[1024 * 8];
        var rnd = new Random(0x4711);
        var testSvc = arch.Services.RequireService<ITestGenerationService>();
        for (int i = 0; i < 1024; ++i)
        {
            long w = rnd.NextInt64() & 0x0FFFFFFF_FFFFFFFF;
            var s = Convert.ToString(w, 8).PadLeft(20, '0');
            testSvc.ReportMissingDecoder("CDC6600Dis", addr, "", s);
        }
    }

    private void AssertCode(string sExp, string octalWord)
    {
        ulong[] words = OctalPattern.ToUInt64s(octalWord, 20);
        var mem = new Word64MemoryArea(LoadAddress, words);
        var i = Disassemble(mem);
        Assert.That(i.ToString(), Is.EqualTo(sExp));
    }

    [Test]
    public void CDC6600Dis_EQ()
    {
        AssertCode("PS, EQ B7 B2 505725", "00251014470415505725");
    }

    [Test]
    public void CDC6600Dis_GE()
    {
        AssertCode("PS, GE B0 B6 052064", "00226313170633052064");
    }

    [Test]
    public void CDC6600Dis_IR()
    {
        AssertCode("IR X5 120652, NO, NO", "03471206524675446670");
    }

    [Test]
    public void CDC6600Dis_JP()
    {
        AssertCode("JP B6 + 563677, FX6 X5 - X0, SX1 B3 - B6", "02015636773165077136");
    }

    [Test]
    public void CDC6600Dis_NG()
    {
        AssertCode("NG X0 741301, AX3 B5 X6, DX1 X6 - X4", "03327413012335633164");
    }

    [Test]
    public void CDC6600Dis_NO()
    {
        AssertCode("NO, SX3 A0 - B1, BX2 -X6 + X0, BX6 X3 - X3", "46401753011620613633");
    }

    [Test]
    public void CDC6600Dis_NZ()
    {
        AssertCode("AX6 B5 X1, NZ X6 337160, DX1 X0 + X5", "23651031033716032105");
    }

    [Test]
    public void CDC6600Dis_OR()
    {
        AssertCode("OR X2 74321, SB2 A0 + B6, LX6 B4 X6", "03550743216420622646");
    }

    [Test]
    public void CDC6600Dis_PL()
    {
        AssertCode("PL X0 506507, SA3 B7 + 336376", "03245065075127336376");
    }

    [Test]
    public void CDC6600Dis_popcnt()
    {
        AssertCode("CX2 X5, SX4 B1 - B3, NE B5 B1 523511", "47275774130564523511");
    }


    [Test]
    public void CDC6600Dis_PS()
    {
        AssertCode("PS, SB5 A5 + B3, DX4 X4 + X7", "00034535636455332447");
    }

    [Test]
    public void CDC6600Dis_REC()
    {
        AssertCode("REC B4 + 613246, IX6 X3 - X3, NO", "01136132463763346000");
    }

    [Test]
    public void CDC6600Dis_RJ()
    {
        AssertCode("PS, RJ 067551", "00265474660100067551");
    }

    [Test]
    public void CDC6600Dis_WEC()
    {
        AssertCode("WEC B0 + 116606, NO, RX5 X4 / X0", "01221166064604645540");
    }

    [Test]
    public void CDC6600Dis_ZR()
    {
        AssertCode("ZR X1 603514, SA0 A0 + 630004", "03066035145002630004");
    }

    [Test]
    public void CDC6600Dis_and()
    {
        AssertCode("PS, BX4 X5 * X6, BX7 -X7 + X1", "00331707531145616717");
    }

    [Test]
    public void CDC6600Dis_asr()
    {
        AssertCode("AX0 10, DX1 X1 - X7, AX1 41, MX6 65", "21010331172114143665");
    }

    [Test]
    public void CDC6600Dis_asrn()
    {
        AssertCode("AX2 B2 X1, BX2 -X3 - X5, BX1 -X3 - X5, IX1 X1 - X7", "23221172531715337117");
    }

    [Test]
    public void CDC6600Dis_dadd()
    {
        AssertCode("DX0 X5 + X2, SA3 B0 + B5, FX7 X6 - X5, FX1 X6 * X2", "32052563053176540162");
    }

    [Test]
    public void CDC6600Dis_dmul()
    {
        AssertCode("DX0 X4 * X3, DX3 X5 + X2, BX7 -X7 + X5, BX6 -X0 - X3", "42043323521675717630");
    }

    [Test]
    public void CDC6600Dis_dsub()
    {
        AssertCode("NO, DX1 X0 - X4, SB4 B5 + B7, NO", "46456331046645746311");
    }

    [Test]
    public void CDC6600Dis_fadd()
    {
        AssertCode("FX4 X4 + X7, LX0 B0 X1, SB4 A1 - B7, NO", "30447220016541746750");
    }

    [Test]
    public void CDC6600Dis_fadd_round()
    {
        AssertCode("RX1 X0 + X5, CX6 X1, RX2 X6 / X6, DX7 X4 - X1", "34105476114526633741");
    }

    [Test]
    public void CDC6600Dis_fdiv()
    {
        AssertCode("FX1 X2 / X7, SB6 B5 + 330654, BX1 X7 + X6", "44127615733065412176");
    }

    [Test]
    public void CDC6600Dis_fdiv_round()
    {
        AssertCode("RX0 X3 / X1, SB5 B0 + B3, ZX5 B5 X1, SB6 A7 - B3", "45031665032555165673");
    }

    [Test]
    public void CDC6600Dis_fmul()
    {
        AssertCode("FX6 X1 * X1, SA5 B5 + B7, SB4 B6 - B1, ZX2 B7 X6", "40611565576746125276");
    }

    [Test]
    public void CDC6600Dis_fmul_round()
    {
        AssertCode("PS, SA0 B5 + B4, RX0 X0 * X6", "00360200635605441006");
    }

    [Test]
    public void CDC6600Dis_fsub()
    {
        AssertCode("FX4 X3 - X7, RX2 X6 / X7, SX0 X3 + 106034", "31437452677253106034");
    }

    [Test]
    public void CDC6600Dis_fsub_round()
    {
        AssertCode("RX0 X6 - X3, SA1 B7 - B7, RX7 X1 / X2, SA5 B5 - B2", "35063571774571257552");
    }

    [Test]
    public void CDC6600Dis_iadd()
    {
        AssertCode("IX1 X3 + X2, EQ B2 B0 633202, NO", "36132047463320246000");
    }

    [Test]
    public void CDC6600Dis_lsl()
    {
        AssertCode("LX3 63, SA1 B6 - B1, SB3 A7 + B6, LX0 B7 X3", "20363571616437622073");
    }

    [Test]
    public void CDC6600Dis_lsln()
    {
        AssertCode("LX1 B0 X1, NO, SA4 X1 + B6, NO", "22101466765341646423");
    }

    [Test]
    public void CDC6600Dis_mask()
    {
        AssertCode("MX0 43, SA3 X4 + B7, SA3 A3 - B5, SA2 A7 - B5", "43043533475533555275");
    }

    [Test]
    public void CDC6600Dis_mov()
    {
        AssertCode("BX0 X6, SA0 A0 + B2, BX4 -X2, FX6 X2 * X4", "10065540021446240624");
    }

    [Test]
    public void CDC6600Dis_neg()
    {
        AssertCode("BX0 -X1, AX4 32, NE B2 B1 030211", "14011214320541030211");
    }

    [Test]
    public void CDC6600Dis_normalize()
    {
        AssertCode("NX4 B5 X5, PS, SB0 A6 - B4", "24455002645545565064");
    }

    [Test]
    public void CDC6600Dis_or()
    {
        AssertCode("BX0 X2 + X2, SX2 A0 + B0, BX7 X0, NO", "12022742001070646353");
    }

    [Test]
    public void CDC6600Dis_pack()
    {
        AssertCode("PX6 B0 X5, SX2 B4 + 220240, CX4 X7", "27605713722024047437");
    }

    [Test]
    public void CDC6600Dis_round_normalize()
    {
        AssertCode("ZX6 B7 X7, IX0 X1 - X5, SB4 X6 + 506464", "25677370156236506464");
    }

    [Test]
    public void CDC6600Dis_unpack()
    {
        AssertCode("UX3 B5 X7, BX4 -X7, FX2 X5 * X5, UX2 B2 X0", "26357144474025526220");
    }

    [Test]
    public void CDC6600Dis_xor()
    {
        AssertCode("BX0 X6 - X2, DX4 X4 - X4, BX2 X7 * X7, SB6 X4 + B2", "13062334441127763642");
    }

    [Test]
    public void CDC6600Dis_xorn()
    {
        AssertCode("BX1 -X6 - X1, EQ B1 B3 334137, SA1 A7 - B4", "17116043233413755174");
    }

#if NOPE
    // Reko: a decoder for the instruction 03327413012335633164 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51575575563601534061()
    {
        AssertCode("@@@", "51575575563601534061");
    }

    // Reko: a decoder for the instruction 03327413012335633164 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 02634513046033154515 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02634513046033154515()
    {
        AssertCode("@@@", "02634513046033154515");
    }

    // Reko: a decoder for the instruction 02527320541757546055 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02527320541757546055()
    {
        AssertCode("@@@", "02527320541757546055");
    }

    // Reko: a decoder for the instruction 43775674477413340262 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43775674477413340262()
    {
        AssertCode("@@@", "43775674477413340262");
    }

    // Reko: a decoder for the instruction 11211672166407406217 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11211672166407406217()
    {
        AssertCode("@@@", "11211672166407406217");
    }

    // Reko: a decoder for the instruction 02676535647011263047 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02676535647011263047()
    {
        AssertCode("@@@", "02676535647011263047");
    }

    // Reko: a decoder for the instruction 35224531466652043137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35224531466652043137()
    {
        AssertCode("@@@", "35224531466652043137");
    }

    // Reko: a decoder for the instruction 06521172155265574356 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06521172155265574356()
    {
        AssertCode("@@@", "06521172155265574356");
    }

    // Reko: a decoder for the instruction 71733730232071561454 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71733730232071561454()
    {
        AssertCode("@@@", "71733730232071561454");
    }

    // Reko: a decoder for the instruction 06721116427505064255 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06721116427505064255()
    {
        AssertCode("@@@", "06721116427505064255");
    }

    // Reko: a decoder for the instruction 54223237730330623021 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54223237730330623021()
    {
        AssertCode("@@@", "54223237730330623021");
    }

    // Reko: a decoder for the instruction 33553561625361407353 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33553561625361407353()
    {
        AssertCode("@@@", "33553561625361407353");
    }

    // Reko: a decoder for the instruction 24402631215157773261 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24402631215157773261()
    {
        AssertCode("@@@", "24402631215157773261");
    }

    // Reko: a decoder for the instruction 22433410355011361020 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22433410355011361020()
    {
        AssertCode("@@@", "22433410355011361020");
    }

    // Reko: a decoder for the instruction 57011575740542157067 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57011575740542157067()
    {
        AssertCode("@@@", "57011575740542157067");
    }

    // Reko: a decoder for the instruction 37303143527573720632 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37303143527573720632()
    {
        AssertCode("@@@", "37303143527573720632");
    }

    // Reko: a decoder for the instruction 10424102401062376242 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10424102401062376242()
    {
        AssertCode("@@@", "10424102401062376242");
    }

    // Reko: a decoder for the instruction 20363571616437622073 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 03164200754136143506 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03164200754136143506()
    {
        AssertCode("@@@", "03164200754136143506");
    }

    // Reko: a decoder for the instruction 13057627661025746063 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13057627661025746063()
    {
        AssertCode("@@@", "13057627661025746063");
    }

    // Reko: a decoder for the instruction 10451251433766107005 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10451251433766107005()
    {
        AssertCode("@@@", "10451251433766107005");
    }

    // Reko: a decoder for the instruction 62634252325016736060 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62634252325016736060()
    {
        AssertCode("@@@", "62634252325016736060");
    }

    // Reko: a decoder for the instruction 41604137755072675523 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41604137755072675523()
    {
        AssertCode("@@@", "41604137755072675523");
    }

    // Reko: a decoder for the instruction 22147225573535554362 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22147225573535554362()
    {
        AssertCode("@@@", "22147225573535554362");
    }

    // Reko: a decoder for the instruction 04447011442347562735 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04447011442347562735()
    {
        AssertCode("@@@", "04447011442347562735");
    }

    // Reko: a decoder for the instruction 61062470325576125710 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61062470325576125710()
    {
        AssertCode("@@@", "61062470325576125710");
    }

    // Reko: a decoder for the instruction 22411535014421275073 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22411535014421275073()
    {
        AssertCode("@@@", "22411535014421275073");
    }

    // Reko: a decoder for the instruction 60132051540257324307 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60132051540257324307()
    {
        AssertCode("@@@", "60132051540257324307");
    }

    // Reko: a decoder for the instruction 30447220016541762750 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 03245065075127336376 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 05560440523746303672 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05560440523746303672()
    {
        AssertCode("@@@", "05560440523746303672");
    }

    // Reko: a decoder for the instruction 73504065240262113230 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73504065240262113230()
    {
        AssertCode("@@@", "73504065240262113230");
    }

    // Reko: a decoder for the instruction 16056464420147117622 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16056464420147117622()
    {
        AssertCode("@@@", "16056464420147117622");
    }

    // Reko: a decoder for the instruction 16652220462171741552 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16652220462171741552()
    {
        AssertCode("@@@", "16652220462171741552");
    }

    // Reko: a decoder for the instruction 63025514435413754766 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63025514435413754766()
    {
        AssertCode("@@@", "63025514435413754766");
    }

    // Reko: a decoder for the instruction 00660200762732662546 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_00660200762732662546()
    {
        AssertCode("@@@", "00660200762732662546");
    }

    // Reko: a decoder for the instruction 42131600333755137143 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42131600333755137143()
    {
        AssertCode("@@@", "42131600333755137143");
    }

    // Reko: a decoder for the instruction 53222111217571221015 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53222111217571221015()
    {
        AssertCode("@@@", "53222111217571221015");
    }

    // Reko: a decoder for the instruction 77356553451747704462 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77356553451747704462()
    {
        AssertCode("@@@", "77356553451747704462");
    }

    // Reko: a decoder for the instruction 51431317262207303171 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51431317262207303171()
    {
        AssertCode("@@@", "51431317262207303171");
    }

    // Reko: a decoder for the instruction 53277666762517321647 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53277666762517321647()
    {
        AssertCode("@@@", "53277666762517321647");
    }

    // Reko: a decoder for the instruction 24236765211605363367 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24236765211605363367()
    {
        AssertCode("@@@", "24236765211605363367");
    }

    // Reko: a decoder for the instruction 21010331172114143665 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 17426160141527107430 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17426160141527107430()
    {
        AssertCode("@@@", "17426160141527107430");
    }

    // Reko: a decoder for the instruction 61551166666432430040 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61551166666432430040()
    {
        AssertCode("@@@", "61551166666432430040");
    }

    // Reko: a decoder for the instruction 64413267166417035370 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64413267166417035370()
    {
        AssertCode("@@@", "64413267166417035370");
    }

    // Reko: a decoder for the instruction 74157162004035751043 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74157162004035751043()
    {
        AssertCode("@@@", "74157162004035751043");
    }

    // Reko: a decoder for the instruction 22604643764653275670 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22604643764653275670()
    {
        AssertCode("@@@", "22604643764653275670");
    }

    // Reko: a decoder for the instruction 35420721455312411133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35420721455312411133()
    {
        AssertCode("@@@", "35420721455312411133");
    }

    // Reko: a decoder for the instruction 46731444153154323365 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46731444153154323365()
    {
        AssertCode("@@@", "46731444153154323365");
    }

    // Reko: a decoder for the instruction 24567574310625741255 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24567574310625741255()
    {
        AssertCode("@@@", "24567574310625741255");
    }

    // Reko: a decoder for the instruction 46376250376053444252 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46376250376053444252()
    {
        AssertCode("@@@", "46376250376053444252");
    }

    // Reko: a decoder for the instruction 35117147627755433525 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35117147627755433525()
    {
        AssertCode("@@@", "35117147627755433525");
    }

    // Reko: a decoder for the instruction 36743152355433365666 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36743152355433365666()
    {
        AssertCode("@@@", "36743152355433365666");
    }

    // Reko: a decoder for the instruction 00751140572261576500 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_00751140572261576500()
    {
        AssertCode("@@@", "00751140572261576500");
    }

    // Reko: a decoder for the instruction 11544553617334225034 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11544553617334225034()
    {
        AssertCode("@@@", "11544553617334225034");
    }

    // Reko: a decoder for the instruction 21315415161636714506 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21315415161636714506()
    {
        AssertCode("@@@", "21315415161636714506");
    }

    // Reko: a decoder for the instruction 65270626642542127431 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65270626642542127431()
    {
        AssertCode("@@@", "65270626642542127431");
    }

    // Reko: a decoder for the instruction 13767100477360552522 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13767100477360552522()
    {
        AssertCode("@@@", "13767100477360552522");
    }

    // Reko: a decoder for the instruction 16561257244717304727 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16561257244717304727()
    {
        AssertCode("@@@", "16561257244717304727");
    }

    // Reko: a decoder for the instruction 24073574406001666160 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24073574406001666160()
    {
        AssertCode("@@@", "24073574406001666160");
    }

    // Reko: a decoder for the instruction 17143223143260520565 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17143223143260520565()
    {
        AssertCode("@@@", "17143223143260520565");
    }

    // Reko: a decoder for the instruction 71341757473521726363 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71341757473521726363()
    {
        AssertCode("@@@", "71341757473521726363");
    }

    // Reko: a decoder for the instruction 75572030035057131526 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75572030035057131526()
    {
        AssertCode("@@@", "75572030035057131526");
    }

    // Reko: a decoder for the instruction 36132047463320272741 at address 01000 has not been implemented.
    // Reko: a decoder for the instruction 14751631763576352137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14751631763576352137()
    {
        AssertCode("@@@", "14751631763576352137");
    }

    // Reko: a decoder for the instruction 27167743501742216335 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27167743501742216335()
    {
        AssertCode("@@@", "27167743501742216335");
    }

    // Reko: a decoder for the instruction 33745530155340236540 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33745530155340236540()
    {
        AssertCode("@@@", "33745530155340236540");
    }

    // Reko: a decoder for the instruction 26230274234461254137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26230274234461254137()
    {
        AssertCode("@@@", "26230274234461254137");
    }

    // Reko: a decoder for the instruction 37372255632600372767 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37372255632600372767()
    {
        AssertCode("@@@", "37372255632600372767");
    }

    // Reko: a decoder for the instruction 41714342553724324071 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41714342553724324071()
    {
        AssertCode("@@@", "41714342553724324071");
    }

    // Reko: a decoder for the instruction 67636031772123163355 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67636031772123163355()
    {
        AssertCode("@@@", "67636031772123163355");
    }

    // Reko: a decoder for the instruction 66133621336257126473 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66133621336257126473()
    {
        AssertCode("@@@", "66133621336257126473");
    }

    // Reko: a decoder for the instruction 60517045751221616055 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60517045751221616055()
    {
        AssertCode("@@@", "60517045751221616055");
    }

    // Reko: a decoder for the instruction 24522773457400202026 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24522773457400202026()
    {
        AssertCode("@@@", "24522773457400202026");
    }

    // Reko: a decoder for the instruction 74675034711576235233 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74675034711576235233()
    {
        AssertCode("@@@", "74675034711576235233");
    }

    // Reko: a decoder for the instruction 32456644007455271365 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32456644007455271365()
    {
        AssertCode("@@@", "32456644007455271365");
    }

    // Reko: a decoder for the instruction 56613721547654770152 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56613721547654770152()
    {
        AssertCode("@@@", "56613721547654770152");
    }

    // Reko: a decoder for the instruction 21142030570403673714 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21142030570403673714()
    {
        AssertCode("@@@", "21142030570403673714");
    }

    // Reko: a decoder for the instruction 33433165110541701261 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33433165110541701261()
    {
        AssertCode("@@@", "33433165110541701261");
    }

    // Reko: a decoder for the instruction 07203426667670707130 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07203426667670707130()
    {
        AssertCode("@@@", "07203426667670707130");
    }

    // Reko: a decoder for the instruction 25304454130726317473 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25304454130726317473()
    {
        AssertCode("@@@", "25304454130726317473");
    }

    // Reko: a decoder for the instruction 66344722167511336557 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66344722167511336557()
    {
        AssertCode("@@@", "66344722167511336557");
    }

    // Reko: a decoder for the instruction 42125564471071462736 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42125564471071462736()
    {
        AssertCode("@@@", "42125564471071462736");
    }

    // Reko: a decoder for the instruction 66542410005377266157 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66542410005377266157()
    {
        AssertCode("@@@", "66542410005377266157");
    }

    // Reko: a decoder for the instruction 62367600712236464506 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62367600712236464506()
    {
        AssertCode("@@@", "62367600712236464506");
    }

    // Reko: a decoder for the instruction 75513260356072072225 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75513260356072072225()
    {
        AssertCode("@@@", "75513260356072072225");
    }

    // Reko: a decoder for the instruction 47114462030037313332 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47114462030037313332()
    {
        AssertCode("@@@", "47114462030037313332");
    }

    // Reko: a decoder for the instruction 04020366676354047134 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04020366676354047134()
    {
        AssertCode("@@@", "04020366676354047134");
    }

    // Reko: a decoder for the instruction 05006565571016631003 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05006565571016631003()
    {
        AssertCode("@@@", "05006565571016631003");
    }

    // Reko: a decoder for the instruction 61126524524244464740 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61126524524244464740()
    {
        AssertCode("@@@", "61126524524244464740");
    }

    // Reko: a decoder for the instruction 74305416156517325007 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74305416156517325007()
    {
        AssertCode("@@@", "74305416156517325007");
    }

    // Reko: a decoder for the instruction 75532235446663513133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75532235446663513133()
    {
        AssertCode("@@@", "75532235446663513133");
    }

    // Reko: a decoder for the instruction 52510315751303772161 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52510315751303772161()
    {
        AssertCode("@@@", "52510315751303772161");
    }

    // Reko: a decoder for the instruction 75471674150566063300 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75471674150566063300()
    {
        AssertCode("@@@", "75471674150566063300");
    }

    // Reko: a decoder for the instruction 46611332037641703312 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46611332037641703312()
    {
        AssertCode("@@@", "46611332037641703312");
    }

    // Reko: a decoder for the instruction 37270724164120700511 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37270724164120700511()
    {
        AssertCode("@@@", "37270724164120700511");
    }

    // Reko: a decoder for the instruction 22552374160664365523 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22552374160664365523()
    {
        AssertCode("@@@", "22552374160664365523");
    }

    // Reko: a decoder for the instruction 23177027673743357174 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23177027673743357174()
    {
        AssertCode("@@@", "23177027673743357174");
    }

    // Reko: a decoder for the instruction 25151645206773400667 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25151645206773400667()
    {
        AssertCode("@@@", "25151645206773400667");
    }

    // Reko: a decoder for the instruction 61772452133302212642 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61772452133302212642()
    {
        AssertCode("@@@", "61772452133302212642");
    }

    // Reko: a decoder for the instruction 77623117737750613362 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77623117737750613362()
    {
        AssertCode("@@@", "77623117737750613362");
    }

    // Reko: a decoder for the instruction 47133377023176607770 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47133377023176607770()
    {
        AssertCode("@@@", "47133377023176607770");
    }

    // Reko: a decoder for the instruction 15215657227272722347 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15215657227272722347()
    {
        AssertCode("@@@", "15215657227272722347");
    }

    // Reko: a decoder for the instruction 12022742001070646353 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12022742001070646353()
    {
        AssertCode("@@@", "12022742001070646353");
    }

    // Reko: a decoder for the instruction 27275224600635230030 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27275224600635230030()
    {
        AssertCode("@@@", "27275224600635230030");
    }

    // Reko: a decoder for the instruction 63172443074435514226 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63172443074435514226()
    {
        AssertCode("@@@", "63172443074435514226");
    }

    // Reko: a decoder for the instruction 06023336605634352067 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06023336605634352067()
    {
        AssertCode("@@@", "06023336605634352067");
    }

    // Reko: a decoder for the instruction 41615601764316455156 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41615601764316455156()
    {
        AssertCode("@@@", "41615601764316455156");
    }

    // Reko: a decoder for the instruction 31312677134265363541 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 41177202245062511063 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41177202245062511063()
    {
        AssertCode("@@@", "41177202245062511063");
    }

    // Reko: a decoder for the instruction 17070033366361503604 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17070033366361503604()
    {
        AssertCode("@@@", "17070033366361503604");
    }

    // Reko: a decoder for the instruction 61043125176336705156 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61043125176336705156()
    {
        AssertCode("@@@", "61043125176336705156");
    }

    // Reko: a decoder for the instruction 54062261666420756465 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54062261666420756465()
    {
        AssertCode("@@@", "54062261666420756465");
    }

    // Reko: a decoder for the instruction 23563137343054774557 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23563137343054774557()
    {
        AssertCode("@@@", "23563137343054774557");
    }

    // Reko: a decoder for the instruction 50676672363173220527 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50676672363173220527()
    {
        AssertCode("@@@", "50676672363173220527");
    }

    // Reko: a decoder for the instruction 14354542563106105272 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14354542563106105272()
    {
        AssertCode("@@@", "14354542563106105272");
    }

    // Reko: a decoder for the instruction 10451535511026737017 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10451535511026737017()
    {
        AssertCode("@@@", "10451535511026737017");
    }

    // Reko: a decoder for the instruction 55526335013251417572 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55526335013251417572()
    {
        AssertCode("@@@", "55526335013251417572");
    }

    // Reko: a decoder for the instruction 67656042744073537471 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67656042744073537471()
    {
        AssertCode("@@@", "67656042744073537471");
    }

    // Reko: a decoder for the instruction 77333424504564360052 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77333424504564360052()
    {
        AssertCode("@@@", "77333424504564360052");
    }

    // Reko: a decoder for the instruction 63711004225460156204 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63711004225460156204()
    {
        AssertCode("@@@", "63711004225460156204");
    }

    // Reko: a decoder for the instruction 32052563053176540162 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 76011346455233727327 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76011346455233727327()
    {
        AssertCode("@@@", "76011346455233727327");
    }

    // Reko: a decoder for the instruction 05030112733010462322 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05030112733010462322()
    {
        AssertCode("@@@", "05030112733010462322");
    }

    // Reko: a decoder for the instruction 75212767326204071101 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75212767326204071101()
    {
        AssertCode("@@@", "75212767326204071101");
    }

    // Reko: a decoder for the instruction 36004464710567622401 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36004464710567622401()
    {
        AssertCode("@@@", "36004464710567622401");
    }

    // Reko: a decoder for the instruction 61644765345163707167 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61644765345163707167()
    {
        AssertCode("@@@", "61644765345163707167");
    }

    // Reko: a decoder for the instruction 22560273241117147362 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22560273241117147362()
    {
        AssertCode("@@@", "22560273241117147362");
    }

    // Reko: a decoder for the instruction 71654401500237305255 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71654401500237305255()
    {
        AssertCode("@@@", "71654401500237305255");
    }

    // Reko: a decoder for the instruction 11151263131535413415 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11151263131535413415()
    {
        AssertCode("@@@", "11151263131535413415");
    }

    // Reko: a decoder for the instruction 64707307750241711646 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64707307750241711646()
    {
        AssertCode("@@@", "64707307750241711646");
    }

    // Reko: a decoder for the instruction 54465714347261545265 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54465714347261545265()
    {
        AssertCode("@@@", "54465714347261545265");
    }

    // Reko: a decoder for the instruction 35440111407127376521 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35440111407127376521()
    {
        AssertCode("@@@", "35440111407127376521");
    }

    // Reko: a decoder for the instruction 60427243337065166243 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60427243337065166243()
    {
        AssertCode("@@@", "60427243337065166243");
    }

    // Reko: a decoder for the instruction 12670145311714163430 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12670145311714163430()
    {
        AssertCode("@@@", "12670145311714163430");
    }

    // Reko: a decoder for the instruction 26324326551601034215 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26324326551601034215()
    {
        AssertCode("@@@", "26324326551601034215");
    }

    // Reko: a decoder for the instruction 70155476321564603133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70155476321564603133()
    {
        AssertCode("@@@", "70155476321564603133");
    }

    // Reko: a decoder for the instruction 74575611215470316514 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74575611215470316514()
    {
        AssertCode("@@@", "74575611215470316514");
    }

    // Reko: a decoder for the instruction 43721620154456154666 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43721620154456154666()
    {
        AssertCode("@@@", "43721620154456154666");
    }

    // Reko: a decoder for the instruction 05653564656110411306 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05653564656110411306()
    {
        AssertCode("@@@", "05653564656110411306");
    }

    // Reko: a decoder for the instruction 15516546276230401757 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15516546276230401757()
    {
        AssertCode("@@@", "15516546276230401757");
    }

    // Reko: a decoder for the instruction 13712753211270646401 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13712753211270646401()
    {
        AssertCode("@@@", "13712753211270646401");
    }

    // Reko: a decoder for the instruction 53673446474305201206 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53673446474305201206()
    {
        AssertCode("@@@", "53673446474305201206");
    }

    // Reko: a decoder for the instruction 55406204475412205247 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55406204475412205247()
    {
        AssertCode("@@@", "55406204475412205247");
    }

    // Reko: a decoder for the instruction 10030352416470003445 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10030352416470003445()
    {
        AssertCode("@@@", "10030352416470003445");
    }

    // Reko: a decoder for the instruction 27535057476636012153 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27535057476636012153()
    {
        AssertCode("@@@", "27535057476636012153");
    }

    // Reko: a decoder for the instruction 53532145221011076701 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53532145221011076701()
    {
        AssertCode("@@@", "53532145221011076701");
    }

    // Reko: a decoder for the instruction 23037614205445433301 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23037614205445433301()
    {
        AssertCode("@@@", "23037614205445433301");
    }

    // Reko: a decoder for the instruction 72552627662634207735 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72552627662634207735()
    {
        AssertCode("@@@", "72552627662634207735");
    }

    // Reko: a decoder for the instruction 60753067607777701112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60753067607777701112()
    {
        AssertCode("@@@", "60753067607777701112");
    }

    // Reko: a decoder for the instruction 22544157524251105441 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22544157524251105441()
    {
        AssertCode("@@@", "22544157524251105441");
    }

    // Reko: a decoder for the instruction 64203113505465275642 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64203113505465275642()
    {
        AssertCode("@@@", "64203113505465275642");
    }

    // Reko: a decoder for the instruction 50402066320554427106 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50402066320554427106()
    {
        AssertCode("@@@", "50402066320554427106");
    }

    // Reko: a decoder for the instruction 00265474660160767551 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 47120647735503524305 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47120647735503524305()
    {
        AssertCode("@@@", "47120647735503524305");
    }

    // Reko: a decoder for the instruction 72773606261170125472 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72773606261170125472()
    {
        AssertCode("@@@", "72773606261170125472");
    }

    // Reko: a decoder for the instruction 02267003052244645454 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02267003052244645454()
    {
        AssertCode("@@@", "02267003052244645454");
    }

    // Reko: a decoder for the instruction 76164566250107141645 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76164566250107141645()
    {
        AssertCode("@@@", "76164566250107141645");
    }

    // Reko: a decoder for the instruction 50342657151344321214 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50342657151344321214()
    {
        AssertCode("@@@", "50342657151344321214");
    }

    // Reko: a decoder for the instruction 15532110751113724566 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15532110751113724566()
    {
        AssertCode("@@@", "15532110751113724566");
    }

    // Reko: a decoder for the instruction 73321526001674211377 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73321526001674211377()
    {
        AssertCode("@@@", "73321526001674211377");
    }

    // Reko: a decoder for the instruction 23644421774136727204 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23644421774136727204()
    {
        AssertCode("@@@", "23644421774136727204");
    }

    // Reko: a decoder for the instruction 73444052134051326421 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73444052134051326421()
    {
        AssertCode("@@@", "73444052134051326421");
    }

    // Reko: a decoder for the instruction 44670227461121236252 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44670227461121236252()
    {
        AssertCode("@@@", "44670227461121236252");
    }

    // Reko: a decoder for the instruction 41603002162605571326 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41603002162605571326()
    {
        AssertCode("@@@", "41603002162605571326");
    }

    // Reko: a decoder for the instruction 21237436157707532445 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21237436157707532445()
    {
        AssertCode("@@@", "21237436157707532445");
    }

    // Reko: a decoder for the instruction 06006301445043537167 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06006301445043537167()
    {
        AssertCode("@@@", "06006301445043537167");
    }

    // Reko: a decoder for the instruction 22566460670741775566 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22566460670741775566()
    {
        AssertCode("@@@", "22566460670741775566");
    }

    // Reko: a decoder for the instruction 45260637073675564637 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45260637073675564637()
    {
        AssertCode("@@@", "45260637073675564637");
    }

    // Reko: a decoder for the instruction 61560417310544243574 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61560417310544243574()
    {
        AssertCode("@@@", "61560417310544243574");
    }

    // Reko: a decoder for the instruction 65636212046416136401 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65636212046416136401()
    {
        AssertCode("@@@", "65636212046416136401");
    }

    // Reko: a decoder for the instruction 33271313343232326477 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33271313343232326477()
    {
        AssertCode("@@@", "33271313343232326477");
    }

    // Reko: a decoder for the instruction 27605713722024047437 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 60524553761222232541 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60524553761222232541()
    {
        AssertCode("@@@", "60524553761222232541");
    }

    // Reko: a decoder for the instruction 55744470164037773277 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55744470164037773277()
    {
        AssertCode("@@@", "55744470164037773277");
    }

    // Reko: a decoder for the instruction 20325227206366353766 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20325227206366353766()
    {
        AssertCode("@@@", "20325227206366353766");
    }

    // Reko: a decoder for the instruction 03746313553156343612 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03746313553156343612()
    {
        AssertCode("@@@", "03746313553156343612");
    }

    // Reko: a decoder for the instruction 35420203527757271031 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35420203527757271031()
    {
        AssertCode("@@@", "35420203527757271031");
    }

    // Reko: a decoder for the instruction 24443444274214075237 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24443444274214075237()
    {
        AssertCode("@@@", "24443444274214075237");
    }

    // Reko: a decoder for the instruction 13401223047123505327 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13401223047123505327()
    {
        AssertCode("@@@", "13401223047123505327");
    }

    // Reko: a decoder for the instruction 53115521454531563637 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53115521454531563637()
    {
        AssertCode("@@@", "53115521454531563637");
    }

    // Reko: a decoder for the instruction 66052461775210116633 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66052461775210116633()
    {
        AssertCode("@@@", "66052461775210116633");
    }

    // Reko: a decoder for the instruction 33075665054560570320 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33075665054560570320()
    {
        AssertCode("@@@", "33075665054560570320");
    }

    // Reko: a decoder for the instruction 11167271460134063115 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11167271460134063115()
    {
        AssertCode("@@@", "11167271460134063115");
    }

    // Reko: a decoder for the instruction 26317026110534714317 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26317026110534714317()
    {
        AssertCode("@@@", "26317026110534714317");
    }

    // Reko: a decoder for the instruction 42423440045573252615 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42423440045573252615()
    {
        AssertCode("@@@", "42423440045573252615");
    }

    // Reko: a decoder for the instruction 37325564215224446220 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37325564215224446220()
    {
        AssertCode("@@@", "37325564215224446220");
    }

    // Reko: a decoder for the instruction 16565735777420247332 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16565735777420247332()
    {
        AssertCode("@@@", "16565735777420247332");
    }

    // Reko: a decoder for the instruction 31472155127170556123 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_31472155127170556123()
    {
        AssertCode("@@@", "31472155127170556123");
    }

    // Reko: a decoder for the instruction 34234144625016302157 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34234144625016302157()
    {
        AssertCode("@@@", "34234144625016302157");
    }

    // Reko: a decoder for the instruction 43253207042204066256 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43253207042204066256()
    {
        AssertCode("@@@", "43253207042204066256");
    }

    // Reko: a decoder for the instruction 51422211156032142044 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51422211156032142044()
    {
        AssertCode("@@@", "51422211156032142044");
    }

    // Reko: a decoder for the instruction 30733175311074054070 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30733175311074054070()
    {
        AssertCode("@@@", "30733175311074054070");
    }

    // Reko: a decoder for the instruction 10546733471125676720 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10546733471125676720()
    {
        AssertCode("@@@", "10546733471125676720");
    }

    // Reko: a decoder for the instruction 10354045411723652275 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10354045411723652275()
    {
        AssertCode("@@@", "10354045411723652275");
    }

    // Reko: a decoder for the instruction 66075047312253725567 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66075047312253725567()
    {
        AssertCode("@@@", "66075047312253725567");
    }

    // Reko: a decoder for the instruction 21132014450433071011 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21132014450433071011()
    {
        AssertCode("@@@", "21132014450433071011");
    }

    // Reko: a decoder for the instruction 73147560762152620200 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73147560762152620200()
    {
        AssertCode("@@@", "73147560762152620200");
    }

    // Reko: a decoder for the instruction 16264426306116540535 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16264426306116540535()
    {
        AssertCode("@@@", "16264426306116540535");
    }

    // Reko: a decoder for the instruction 23230167012205343275 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23230167012205343275()
    {
        AssertCode("@@@", "23230167012205343275");
    }

    // Reko: a decoder for the instruction 01136645543020343602 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_01136645543020343602()
    {
        AssertCode("@@@", "01136645543020343602");
    }

    // Reko: a decoder for the instruction 12067577714760766415 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12067577714760766415()
    {
        AssertCode("@@@", "12067577714760766415");
    }

    // Reko: a decoder for the instruction 63552253245251457136 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63552253245251457136()
    {
        AssertCode("@@@", "63552253245251457136");
    }

    // Reko: a decoder for the instruction 12562363752513546745 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12562363752513546745()
    {
        AssertCode("@@@", "12562363752513546745");
    }

    // Reko: a decoder for the instruction 42520353166007114703 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42520353166007114703()
    {
        AssertCode("@@@", "42520353166007114703");
    }

    // Reko: a decoder for the instruction 41726307720722265564 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41726307720722265564()
    {
        AssertCode("@@@", "41726307720722265564");
    }

    // Reko: a decoder for the instruction 27063514606703062116 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27063514606703062116()
    {
        AssertCode("@@@", "27063514606703062116");
    }

    // Reko: a decoder for the instruction 40661664223151433750 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40661664223151433750()
    {
        AssertCode("@@@", "40661664223151433750");
    }

    // Reko: a decoder for the instruction 30300025141332131017 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30300025141332131017()
    {
        AssertCode("@@@", "30300025141332131017");
    }

    // Reko: a decoder for the instruction 13237470026151501037 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13237470026151501037()
    {
        AssertCode("@@@", "13237470026151501037");
    }

    // Reko: a decoder for the instruction 31373352213662331357 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_31373352213662331357()
    {
        AssertCode("@@@", "31373352213662331357");
    }

    // Reko: a decoder for the instruction 27530526662334055030 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27530526662334055030()
    {
        AssertCode("@@@", "27530526662334055030");
    }

    // Reko: a decoder for the instruction 26363655475371074101 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26363655475371074101()
    {
        AssertCode("@@@", "26363655475371074101");
    }

    // Reko: a decoder for the instruction 60223645150605762643 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60223645150605762643()
    {
        AssertCode("@@@", "60223645150605762643");
    }

    // Reko: a decoder for the instruction 42367566441066023224 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42367566441066023224()
    {
        AssertCode("@@@", "42367566441066023224");
    }

    // Reko: a decoder for the instruction 26760672065726063475 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26760672065726063475()
    {
        AssertCode("@@@", "26760672065726063475");
    }

    // Reko: a decoder for the instruction 71724705257220304270 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71724705257220304270()
    {
        AssertCode("@@@", "71724705257220304270");
    }

    // Reko: a decoder for the instruction 51015445423353543360 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51015445423353543360()
    {
        AssertCode("@@@", "51015445423353543360");
    }

    // Reko: a decoder for the instruction 05477356174637741444 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05477356174637741444()
    {
        AssertCode("@@@", "05477356174637741444");
    }

    // Reko: a decoder for the instruction 52656154727747402547 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52656154727747402547()
    {
        AssertCode("@@@", "52656154727747402547");
    }

    // Reko: a decoder for the instruction 15074577341761615674 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15074577341761615674()
    {
        AssertCode("@@@", "15074577341761615674");
    }

    // Reko: a decoder for the instruction 46573024327551503126 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46573024327551503126()
    {
        AssertCode("@@@", "46573024327551503126");
    }

    // Reko: a decoder for the instruction 21416260775261611670 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21416260775261611670()
    {
        AssertCode("@@@", "21416260775261611670");
    }

    // Reko: a decoder for the instruction 51115423631466127677 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51115423631466127677()
    {
        AssertCode("@@@", "51115423631466127677");
    }

    // Reko: a decoder for the instruction 36170321765251133425 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36170321765251133425()
    {
        AssertCode("@@@", "36170321765251133425");
    }

    // Reko: a decoder for the instruction 66710270212474466374 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66710270212474466374()
    {
        AssertCode("@@@", "66710270212474466374");
    }

    // Reko: a decoder for the instruction 54256101126443762722 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54256101126443762722()
    {
        AssertCode("@@@", "54256101126443762722");
    }

    // Reko: a decoder for the instruction 12633423713161013172 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12633423713161013172()
    {
        AssertCode("@@@", "12633423713161013172");
    }

    // Reko: a decoder for the instruction 22741553537752213125 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22741553537752213125()
    {
        AssertCode("@@@", "22741553537752213125");
    }

    // Reko: a decoder for the instruction 42326302267475603557 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42326302267475603557()
    {
        AssertCode("@@@", "42326302267475603557");
    }

    // Reko: a decoder for the instruction 02626031626634220311 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02626031626634220311()
    {
        AssertCode("@@@", "02626031626634220311");
    }

    // Reko: a decoder for the instruction 27033103117163535054 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27033103117163535054()
    {
        AssertCode("@@@", "27033103117163535054");
    }

    // Reko: a decoder for the instruction 10445506401740431273 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10445506401740431273()
    {
        AssertCode("@@@", "10445506401740431273");
    }

    // Reko: a decoder for the instruction 44342105234120735743 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44342105234120735743()
    {
        AssertCode("@@@", "44342105234120735743");
    }

    // Reko: a decoder for the instruction 06126643307617053423 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06126643307617053423()
    {
        AssertCode("@@@", "06126643307617053423");
    }

    // Reko: a decoder for the instruction 16170450176653206072 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16170450176653206072()
    {
        AssertCode("@@@", "16170450176653206072");
    }

    // Reko: a decoder for the instruction 27710665261160126250 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27710665261160126250()
    {
        AssertCode("@@@", "27710665261160126250");
    }

    // Reko: a decoder for the instruction 57005270374533165666 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57005270374533165666()
    {
        AssertCode("@@@", "57005270374533165666");
    }

    // Reko: a decoder for the instruction 56132614052020764001 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56132614052020764001()
    {
        AssertCode("@@@", "56132614052020764001");
    }

    // Reko: a decoder for the instruction 42736765600010022406 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42736765600010022406()
    {
        AssertCode("@@@", "42736765600010022406");
    }

    // Reko: a decoder for the instruction 72426700142214236315 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72426700142214236315()
    {
        AssertCode("@@@", "72426700142214236315");
    }

    // Reko: a decoder for the instruction 50013036167532570677 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50013036167532570677()
    {
        AssertCode("@@@", "50013036167532570677");
    }

    // Reko: a decoder for the instruction 73230327747215462673 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73230327747215462673()
    {
        AssertCode("@@@", "73230327747215462673");
    }

    // Reko: a decoder for the instruction 07042500722020071352 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07042500722020071352()
    {
        AssertCode("@@@", "07042500722020071352");
    }

    // Reko: a decoder for the instruction 57351460517355562426 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57351460517355562426()
    {
        AssertCode("@@@", "57351460517355562426");
    }

    // Reko: a decoder for the instruction 02015636773165077136 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 15604360120664045047 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15604360120664045047()
    {
        AssertCode("@@@", "15604360120664045047");
    }

    // Reko: a decoder for the instruction 64031531326177045510 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64031531326177045510()
    {
        AssertCode("@@@", "64031531326177045510");
    }

    // Reko: a decoder for the instruction 05150420650453124155 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05150420650453124155()
    {
        AssertCode("@@@", "05150420650453124155");
    }

    // Reko: a decoder for the instruction 34224653221261236206 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34224653221261236206()
    {
        AssertCode("@@@", "34224653221261236206");
    }

    // Reko: a decoder for the instruction 03007410155513507001 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03007410155513507001()
    {
        AssertCode("@@@", "03007410155513507001");
    }

    // Reko: a decoder for the instruction 24347511770543455064 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24347511770543455064()
    {
        AssertCode("@@@", "24347511770543455064");
    }

    // Reko: a decoder for the instruction 24500305452511517132 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24500305452511517132()
    {
        AssertCode("@@@", "24500305452511517132");
    }

    // Reko: a decoder for the instruction 01136132463763361134 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 21574400715724227400 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21574400715724227400()
    {
        AssertCode("@@@", "21574400715724227400");
    }

    // Reko: a decoder for the instruction 37264730065004206460 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37264730065004206460()
    {
        AssertCode("@@@", "37264730065004206460");
    }

    // Reko: a decoder for the instruction 64273275515052774336 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64273275515052774336()
    {
        AssertCode("@@@", "64273275515052774336");
    }

    // Reko: a decoder for the instruction 26274477326777541151 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26274477326777541151()
    {
        AssertCode("@@@", "26274477326777541151");
    }

    // Reko: a decoder for the instruction 33060123614116251324 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33060123614116251324()
    {
        AssertCode("@@@", "33060123614116251324");
    }

    // Reko: a decoder for the instruction 65474371112436063752 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65474371112436063752()
    {
        AssertCode("@@@", "65474371112436063752");
    }

    // Reko: a decoder for the instruction 62527504511521426424 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62527504511521426424()
    {
        AssertCode("@@@", "62527504511521426424");
    }

    // Reko: a decoder for the instruction 42621507447225370035 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42621507447225370035()
    {
        AssertCode("@@@", "42621507447225370035");
    }

    // Reko: a decoder for the instruction 32533042127042040330 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32533042127042040330()
    {
        AssertCode("@@@", "32533042127042040330");
    }

    // Reko: a decoder for the instruction 45502267400077330020 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45502267400077330020()
    {
        AssertCode("@@@", "45502267400077330020");
    }

    // Reko: a decoder for the instruction 26163211476246553153 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26163211476246553153()
    {
        AssertCode("@@@", "26163211476246553153");
    }

    // Reko: a decoder for the instruction 42041254342207144461 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42041254342207144461()
    {
        AssertCode("@@@", "42041254342207144461");
    }

    // Reko: a decoder for the instruction 42716356331721422346 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42716356331721422346()
    {
        AssertCode("@@@", "42716356331721422346");
    }

    // Reko: a decoder for the instruction 66421145035165456635 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66421145035165456635()
    {
        AssertCode("@@@", "66421145035165456635");
    }

    // Reko: a decoder for the instruction 43132477134604477313 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43132477134604477313()
    {
        AssertCode("@@@", "43132477134604477313");
    }

    // Reko: a decoder for the instruction 30662124073004545127 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30662124073004545127()
    {
        AssertCode("@@@", "30662124073004545127");
    }

    // Reko: a decoder for the instruction 61600270663401712673 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61600270663401712673()
    {
        AssertCode("@@@", "61600270663401712673");
    }

    // Reko: a decoder for the instruction 27651236652207370040 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27651236652207370040()
    {
        AssertCode("@@@", "27651236652207370040");
    }

    // Reko: a decoder for the instruction 45232554767052164064 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45232554767052164064()
    {
        AssertCode("@@@", "45232554767052164064");
    }

    // Reko: a decoder for the instruction 27567643142603567531 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27567643142603567531()
    {
        AssertCode("@@@", "27567643142603567531");
    }

    // Reko: a decoder for the instruction 30463671106342045315 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30463671106342045315()
    {
        AssertCode("@@@", "30463671106342045315");
    }

    // Reko: a decoder for the instruction 27142417537741165707 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27142417537741165707()
    {
        AssertCode("@@@", "27142417537741165707");
    }

    // Reko: a decoder for the instruction 35247562115047001111 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35247562115047001111()
    {
        AssertCode("@@@", "35247562115047001111");
    }

    // Reko: a decoder for the instruction 52702511030070710166 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52702511030070710166()
    {
        AssertCode("@@@", "52702511030070710166");
    }

    // Reko: a decoder for the instruction 76261677173565225557 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76261677173565225557()
    {
        AssertCode("@@@", "76261677173565225557");
    }

    // Reko: a decoder for the instruction 41452015612433741064 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41452015612433741064()
    {
        AssertCode("@@@", "41452015612433741064");
    }

    // Reko: a decoder for the instruction 70160125664006774667 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70160125664006774667()
    {
        AssertCode("@@@", "70160125664006774667");
    }

    // Reko: a decoder for the instruction 72462330322527016507 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72462330322527016507()
    {
        AssertCode("@@@", "72462330322527016507");
    }

    // Reko: a decoder for the instruction 53120363462240637017 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53120363462240637017()
    {
        AssertCode("@@@", "53120363462240637017");
    }

    // Reko: a decoder for the instruction 75654517407737763121 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75654517407737763121()
    {
        AssertCode("@@@", "75654517407737763121");
    }

    // Reko: a decoder for the instruction 54734534654331746066 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54734534654331746066()
    {
        AssertCode("@@@", "54734534654331746066");
    }

    // Reko: a decoder for the instruction 57533070226370131264 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57533070226370131264()
    {
        AssertCode("@@@", "57533070226370131264");
    }

    // Reko: a decoder for the instruction 75763266230106357351 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75763266230106357351()
    {
        AssertCode("@@@", "75763266230106357351");
    }

    // Reko: a decoder for the instruction 07667733100341137677 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07667733100341137677()
    {
        AssertCode("@@@", "07667733100341137677");
    }

    // Reko: a decoder for the instruction 77155420052114734124 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77155420052114734124()
    {
        AssertCode("@@@", "77155420052114734124");
    }

    // Reko: a decoder for the instruction 76272043045414066756 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76272043045414066756()
    {
        AssertCode("@@@", "76272043045414066756");
    }

    // Reko: a decoder for the instruction 74662144457763313674 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74662144457763313674()
    {
        AssertCode("@@@", "74662144457763313674");
    }

    // Reko: a decoder for the instruction 65273113655043035602 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65273113655043035602()
    {
        AssertCode("@@@", "65273113655043035602");
    }

    // Reko: a decoder for the instruction 02717060554146374074 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02717060554146374074()
    {
        AssertCode("@@@", "02717060554146374074");
    }

    // Reko: a decoder for the instruction 60266206555600360013 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60266206555600360013()
    {
        AssertCode("@@@", "60266206555600360013");
    }

    // Reko: a decoder for the instruction 12424560337574540223 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12424560337574540223()
    {
        AssertCode("@@@", "12424560337574540223");
    }

    // Reko: a decoder for the instruction 60656757042016373727 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60656757042016373727()
    {
        AssertCode("@@@", "60656757042016373727");
    }

    // Reko: a decoder for the instruction 33624721705663757457 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33624721705663757457()
    {
        AssertCode("@@@", "33624721705663757457");
    }

    // Reko: a decoder for the instruction 35711527432273165265 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35711527432273165265()
    {
        AssertCode("@@@", "35711527432273165265");
    }

    // Reko: a decoder for the instruction 52716134717575434667 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52716134717575434667()
    {
        AssertCode("@@@", "52716134717575434667");
    }

    // Reko: a decoder for the instruction 41242041447456602703 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41242041447456602703()
    {
        AssertCode("@@@", "41242041447456602703");
    }

    // Reko: a decoder for the instruction 02063113201000733047 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 53537711164530546261 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53537711164530546261()
    {
        AssertCode("@@@", "53537711164530546261");
    }

    // Reko: a decoder for the instruction 03775356205466007723 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03775356205466007723()
    {
        AssertCode("@@@", "03775356205466007723");
    }

    // Reko: a decoder for the instruction 36251032723607674414 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36251032723607674414()
    {
        AssertCode("@@@", "36251032723607674414");
    }

    // Reko: a decoder for the instruction 76251537766734546137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76251537766734546137()
    {
        AssertCode("@@@", "76251537766734546137");
    }

    // Reko: a decoder for the instruction 52602707771301154746 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52602707771301154746()
    {
        AssertCode("@@@", "52602707771301154746");
    }

    // Reko: a decoder for the instruction 04040347003723752431 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04040347003723752431()
    {
        AssertCode("@@@", "04040347003723752431");
    }

    // Reko: a decoder for the instruction 04576402165760162766 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04576402165760162766()
    {
        AssertCode("@@@", "04576402165760162766");
    }

    // Reko: a decoder for the instruction 64126202550155424451 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64126202550155424451()
    {
        AssertCode("@@@", "64126202550155424451");
    }

    // Reko: a decoder for the instruction 73565330016270367503 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73565330016270367503()
    {
        AssertCode("@@@", "73565330016270367503");
    }

    // Reko: a decoder for the instruction 74422373623625123546 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74422373623625123546()
    {
        AssertCode("@@@", "74422373623625123546");
    }

    // Reko: a decoder for the instruction 55662770517101414550 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55662770517101414550()
    {
        AssertCode("@@@", "55662770517101414550");
    }

    // Reko: a decoder for the instruction 34654277637034562032 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34654277637034562032()
    {
        AssertCode("@@@", "34654277637034562032");
    }

    // Reko: a decoder for the instruction 03161407500063366646 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03161407500063366646()
    {
        AssertCode("@@@", "03161407500063366646");
    }

    // Reko: a decoder for the instruction 33710301461055546612 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33710301461055546612()
    {
        AssertCode("@@@", "33710301461055546612");
    }

    // Reko: a decoder for the instruction 36215550362537753535 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36215550362537753535()
    {
        AssertCode("@@@", "36215550362537753535");
    }

    // Reko: a decoder for the instruction 23531305376412374401 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23531305376412374401()
    {
        AssertCode("@@@", "23531305376412374401");
    }

    // Reko: a decoder for the instruction 52216454605241001133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52216454605241001133()
    {
        AssertCode("@@@", "52216454605241001133");
    }

    // Reko: a decoder for the instruction 44054013201412352014 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44054013201412352014()
    {
        AssertCode("@@@", "44054013201412352014");
    }

    // Reko: a decoder for the instruction 32061662434207752643 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32061662434207752643()
    {
        AssertCode("@@@", "32061662434207752643");
    }

    // Reko: a decoder for the instruction 30734730404405674112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30734730404405674112()
    {
        AssertCode("@@@", "30734730404405674112");
    }

    // Reko: a decoder for the instruction 74377653215152102100 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74377653215152102100()
    {
        AssertCode("@@@", "74377653215152102100");
    }

    // Reko: a decoder for the instruction 35237062627547416265 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35237062627547416265()
    {
        AssertCode("@@@", "35237062627547416265");
    }

    // Reko: a decoder for the instruction 40551307656277203664 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40551307656277203664()
    {
        AssertCode("@@@", "40551307656277203664");
    }

    // Reko: a decoder for the instruction 75417007473454157264 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75417007473454157264()
    {
        AssertCode("@@@", "75417007473454157264");
    }

    // Reko: a decoder for the instruction 02423450303272006566 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02423450303272006566()
    {
        AssertCode("@@@", "02423450303272006566");
    }

    // Reko: a decoder for the instruction 50726131216261506662 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50726131216261506662()
    {
        AssertCode("@@@", "50726131216261506662");
    }

    // Reko: a decoder for the instruction 12077337662051257106 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12077337662051257106()
    {
        AssertCode("@@@", "12077337662051257106");
    }

    // Reko: a decoder for the instruction 74071702450255500337 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74071702450255500337()
    {
        AssertCode("@@@", "74071702450255500337");
    }

    // Reko: a decoder for the instruction 65466361052705222036 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65466361052705222036()
    {
        AssertCode("@@@", "65466361052705222036");
    }

    // Reko: a decoder for the instruction 65013114163557116066 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65013114163557116066()
    {
        AssertCode("@@@", "65013114163557116066");
    }

    // Reko: a decoder for the instruction 74356331652706025723 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74356331652706025723()
    {
        AssertCode("@@@", "74356331652706025723");
    }

    // Reko: a decoder for the instruction 31217471112236522246 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_31217471112236522246()
    {
        AssertCode("@@@", "31217471112236522246");
    }

    // Reko: a decoder for the instruction 26553606725201373104 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26553606725201373104()
    {
        AssertCode("@@@", "26553606725201373104");
    }

    // Reko: a decoder for the instruction 66500463612321007564 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66500463612321007564()
    {
        AssertCode("@@@", "66500463612321007564");
    }

    // Reko: a decoder for the instruction 24446714435160627601 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24446714435160627601()
    {
        AssertCode("@@@", "24446714435160627601");
    }

    // Reko: a decoder for the instruction 13370407064555545773 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13370407064555545773()
    {
        AssertCode("@@@", "13370407064555545773");
    }

    // Reko: a decoder for the instruction 45067545077420414304 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45067545077420414304()
    {
        AssertCode("@@@", "45067545077420414304");
    }

    // Reko: a decoder for the instruction 42511510320102611335 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42511510320102611335()
    {
        AssertCode("@@@", "42511510320102611335");
    }

    // Reko: a decoder for the instruction 42623310033030166354 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42623310033030166354()
    {
        AssertCode("@@@", "42623310033030166354");
    }

    // Reko: a decoder for the instruction 06663243704565003060 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 56352401533737475506 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56352401533737475506()
    {
        AssertCode("@@@", "56352401533737475506");
    }

    // Reko: a decoder for the instruction 67102742627007535360 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67102742627007535360()
    {
        AssertCode("@@@", "67102742627007535360");
    }

    // Reko: a decoder for the instruction 74744065506373010406 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74744065506373010406()
    {
        AssertCode("@@@", "74744065506373010406");
    }

    // Reko: a decoder for the instruction 11344202273342512040 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11344202273342512040()
    {
        AssertCode("@@@", "11344202273342512040");
    }

    // Reko: a decoder for the instruction 60236405166446221521 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60236405166446221521()
    {
        AssertCode("@@@", "60236405166446221521");
    }

    // Reko: a decoder for the instruction 30054315212731233766 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30054315212731233766()
    {
        AssertCode("@@@", "30054315212731233766");
    }

    // Reko: a decoder for the instruction 47730571301062474227 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47730571301062474227()
    {
        AssertCode("@@@", "47730571301062474227");
    }

    // Reko: a decoder for the instruction 34320552033440520756 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34320552033440520756()
    {
        AssertCode("@@@", "34320552033440520756");
    }

    // Reko: a decoder for the instruction 05552664307543177234 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05552664307543177234()
    {
        AssertCode("@@@", "05552664307543177234");
    }

    // Reko: a decoder for the instruction 04275146675633316446 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04275146675633316446()
    {
        AssertCode("@@@", "04275146675633316446");
    }

    // Reko: a decoder for the instruction 14102107600472663740 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14102107600472663740()
    {
        AssertCode("@@@", "14102107600472663740");
    }

    // Reko: a decoder for the instruction 10271626241337176704 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10271626241337176704()
    {
        AssertCode("@@@", "10271626241337176704");
    }

    // Reko: a decoder for the instruction 47030621644410101257 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47030621644410101257()
    {
        AssertCode("@@@", "47030621644410101257");
    }

    // Reko: a decoder for the instruction 05670652727517353441 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05670652727517353441()
    {
        AssertCode("@@@", "05670652727517353441");
    }

    // Reko: a decoder for the instruction 13020336045107164243 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13020336045107164243()
    {
        AssertCode("@@@", "13020336045107164243");
    }

    // Reko: a decoder for the instruction 14546307574270335231 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14546307574270335231()
    {
        AssertCode("@@@", "14546307574270335231");
    }

    // Reko: a decoder for the instruction 53100356647544622527 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53100356647544622527()
    {
        AssertCode("@@@", "53100356647544622527");
    }

    // Reko: a decoder for the instruction 52016246154617125470 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52016246154617125470()
    {
        AssertCode("@@@", "52016246154617125470");
    }

    // Reko: a decoder for the instruction 36401357312575367325 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36401357312575367325()
    {
        AssertCode("@@@", "36401357312575367325");
    }

    // Reko: a decoder for the instruction 71475306742471220317 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71475306742471220317()
    {
        AssertCode("@@@", "71475306742471220317");
    }

    // Reko: a decoder for the instruction 73042571637001613276 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73042571637001613276()
    {
        AssertCode("@@@", "73042571637001613276");
    }

    // Reko: a decoder for the instruction 52353374541777704471 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52353374541777704471()
    {
        AssertCode("@@@", "52353374541777704471");
    }

    // Reko: a decoder for the instruction 73524560454316752421 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73524560454316752421()
    {
        AssertCode("@@@", "73524560454316752421");
    }

    // Reko: a decoder for the instruction 55631422652147477446 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55631422652147477446()
    {
        AssertCode("@@@", "55631422652147477446");
    }

    // Reko: a decoder for the instruction 71645662413540610234 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71645662413540610234()
    {
        AssertCode("@@@", "71645662413540610234");
    }

    // Reko: a decoder for the instruction 57731426712261420376 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57731426712261420376()
    {
        AssertCode("@@@", "57731426712261420376");
    }

    // Reko: a decoder for the instruction 62455255040060213002 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62455255040060213002()
    {
        AssertCode("@@@", "62455255040060213002");
    }

    // Reko: a decoder for the instruction 40130351676344710045 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40130351676344710045()
    {
        AssertCode("@@@", "40130351676344710045");
    }

    // Reko: a decoder for the instruction 06117272745207311744 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06117272745207311744()
    {
        AssertCode("@@@", "06117272745207311744");
    }

    // Reko: a decoder for the instruction 33532634167373420635 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33532634167373420635()
    {
        AssertCode("@@@", "33532634167373420635");
    }

    // Reko: a decoder for the instruction 53407555416771516704 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53407555416771516704()
    {
        AssertCode("@@@", "53407555416771516704");
    }

    // Reko: a decoder for the instruction 04342533003053553334 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04342533003053553334()
    {
        AssertCode("@@@", "04342533003053553334");
    }

    // Reko: a decoder for the instruction 05414565373220126260 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05414565373220126260()
    {
        AssertCode("@@@", "05414565373220126260");
    }

    // Reko: a decoder for the instruction 34477627454202404450 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34477627454202404450()
    {
        AssertCode("@@@", "34477627454202404450");
    }

    // Reko: a decoder for the instruction 57766122461425210216 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57766122461425210216()
    {
        AssertCode("@@@", "57766122461425210216");
    }

    // Reko: a decoder for the instruction 16604346324673752466 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16604346324673752466()
    {
        AssertCode("@@@", "16604346324673752466");
    }

    // Reko: a decoder for the instruction 42001477156555601505 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42001477156555601505()
    {
        AssertCode("@@@", "42001477156555601505");
    }

    // Reko: a decoder for the instruction 47412020126320564475 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47412020126320564475()
    {
        AssertCode("@@@", "47412020126320564475");
    }

    // Reko: a decoder for the instruction 77602116064614070200 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77602116064614070200()
    {
        AssertCode("@@@", "77602116064614070200");
    }

    // Reko: a decoder for the instruction 06762607652525327122 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06762607652525327122()
    {
        AssertCode("@@@", "06762607652525327122");
    }

    // Reko: a decoder for the instruction 21323466633501442337 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21323466633501442337()
    {
        AssertCode("@@@", "21323466633501442337");
    }

    // Reko: a decoder for the instruction 25154737413044377716 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25154737413044377716()
    {
        AssertCode("@@@", "25154737413044377716");
    }

    // Reko: a decoder for the instruction 36042320716007022220 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36042320716007022220()
    {
        AssertCode("@@@", "36042320716007022220");
    }

    // Reko: a decoder for the instruction 05474225642321653031 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05474225642321653031()
    {
        AssertCode("@@@", "05474225642321653031");
    }

    // Reko: a decoder for the instruction 25717334146632523151 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25717334146632523151()
    {
        AssertCode("@@@", "25717334146632523151");
    }

    // Reko: a decoder for the instruction 35063571774571257552 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 22101466765341601423 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 64043134362602673130 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64043134362602673130()
    {
        AssertCode("@@@", "64043134362602673130");
    }

    // Reko: a decoder for the instruction 65342216644144715311 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65342216644144715311()
    {
        AssertCode("@@@", "65342216644144715311");
    }

    // Reko: a decoder for the instruction 63471171462703037754 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63471171462703037754()
    {
        AssertCode("@@@", "63471171462703037754");
    }

    // Reko: a decoder for the instruction 66502453762752045402 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66502453762752045402()
    {
        AssertCode("@@@", "66502453762752045402");
    }

    // Reko: a decoder for the instruction 03102217042211764314 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03102217042211764314()
    {
        AssertCode("@@@", "03102217042211764314");
    }

    // Reko: a decoder for the instruction 14231614205770131542 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14231614205770131542()
    {
        AssertCode("@@@", "14231614205770131542");
    }

    // Reko: a decoder for the instruction 26033316605133326111 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26033316605133326111()
    {
        AssertCode("@@@", "26033316605133326111");
    }

    // Reko: a decoder for the instruction 45461076551271605417 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45461076551271605417()
    {
        AssertCode("@@@", "45461076551271605417");
    }

    // Reko: a decoder for the instruction 17676341026503343052 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17676341026503343052()
    {
        AssertCode("@@@", "17676341026503343052");
    }

    // Reko: a decoder for the instruction 20340054210520214725 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20340054210520214725()
    {
        AssertCode("@@@", "20340054210520214725");
    }

    // Reko: a decoder for the instruction 03324672531767425164 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03324672531767425164()
    {
        AssertCode("@@@", "03324672531767425164");
    }

    // Reko: a decoder for the instruction 51243553164166646613 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51243553164166646613()
    {
        AssertCode("@@@", "51243553164166646613");
    }

    // Reko: a decoder for the instruction 02633371450117352603 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02633371450117352603()
    {
        AssertCode("@@@", "02633371450117352603");
    }

    // Reko: a decoder for the instruction 35154277445074376123 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35154277445074376123()
    {
        AssertCode("@@@", "35154277445074376123");
    }

    // Reko: a decoder for the instruction 47275774130564523511 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 64422012754723167716 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64422012754723167716()
    {
        AssertCode("@@@", "64422012754723167716");
    }

    // Reko: a decoder for the instruction 66732622540213226147 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66732622540213226147()
    {
        AssertCode("@@@", "66732622540213226147");
    }

    // Reko: a decoder for the instruction 40701145266222206315 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40701145266222206315()
    {
        AssertCode("@@@", "40701145266222206315");
    }

    // Reko: a decoder for the instruction 67045421532213740460 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67045421532213740460()
    {
        AssertCode("@@@", "67045421532213740460");
    }

    // Reko: a decoder for the instruction 67066027244773163617 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67066027244773163617()
    {
        AssertCode("@@@", "67066027244773163617");
    }

    // Reko: a decoder for the instruction 65250654460552454436 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65250654460552454436()
    {
        AssertCode("@@@", "65250654460552454436");
    }

    // Reko: a decoder for the instruction 05143650404333431310 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05143650404333431310()
    {
        AssertCode("@@@", "05143650404333431310");
    }

    // Reko: a decoder for the instruction 17270062332011631555 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17270062332011631555()
    {
        AssertCode("@@@", "17270062332011631555");
    }

    // Reko: a decoder for the instruction 12567213356750316367 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12567213356750316367()
    {
        AssertCode("@@@", "12567213356750316367");
    }

    // Reko: a decoder for the instruction 74600356577542113763 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74600356577542113763()
    {
        AssertCode("@@@", "74600356577542113763");
    }

    // Reko: a decoder for the instruction 12102521436133706570 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12102521436133706570()
    {
        AssertCode("@@@", "12102521436133706570");
    }

    // Reko: a decoder for the instruction 30246603262703761632 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30246603262703761632()
    {
        AssertCode("@@@", "30246603262703761632");
    }

    // Reko: a decoder for the instruction 46241517553356332542 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46241517553356332542()
    {
        AssertCode("@@@", "46241517553356332542");
    }

    // Reko: a decoder for the instruction 25216524355166314051 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25216524355166314051()
    {
        AssertCode("@@@", "25216524355166314051");
    }

    // Reko: a decoder for the instruction 46147656142666424236 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46147656142666424236()
    {
        AssertCode("@@@", "46147656142666424236");
    }

    // Reko: a decoder for the instruction 11014503557473621757 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11014503557473621757()
    {
        AssertCode("@@@", "11014503557473621757");
    }

    // Reko: a decoder for the instruction 13721436431512135452 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13721436431512135452()
    {
        AssertCode("@@@", "13721436431512135452");
    }

    // Reko: a decoder for the instruction 16521021644010477034 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16521021644010477034()
    {
        AssertCode("@@@", "16521021644010477034");
    }

    // Reko: a decoder for the instruction 42113426743017421317 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42113426743017421317()
    {
        AssertCode("@@@", "42113426743017421317");
    }

    // Reko: a decoder for the instruction 34705056173362451317 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34705056173362451317()
    {
        AssertCode("@@@", "34705056173362451317");
    }

    // Reko: a decoder for the instruction 32332201646645541434 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32332201646645541434()
    {
        AssertCode("@@@", "32332201646645541434");
    }

    // Reko: a decoder for the instruction 00360200635605441006 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 17710364037525547430 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17710364037525547430()
    {
        AssertCode("@@@", "17710364037525547430");
    }

    // Reko: a decoder for the instruction 63425102435705366172 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63425102435705366172()
    {
        AssertCode("@@@", "63425102435705366172");
    }

    // Reko: a decoder for the instruction 20134722324763170072 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20134722324763170072()
    {
        AssertCode("@@@", "20134722324763170072");
    }

    // Reko: a decoder for the instruction 20734464126341577251 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20734464126341577251()
    {
        AssertCode("@@@", "20734464126341577251");
    }

    // Reko: a decoder for the instruction 36775311710204511203 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36775311710204511203()
    {
        AssertCode("@@@", "36775311710204511203");
    }

    // Reko: a decoder for the instruction 00630177346223015526 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_00630177346223015526()
    {
        AssertCode("@@@", "00630177346223015526");
    }

    // Reko: a decoder for the instruction 76750464172677606564 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76750464172677606564()
    {
        AssertCode("@@@", "76750464172677606564");
    }

    // Reko: a decoder for the instruction 61477307006266733612 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61477307006266733612()
    {
        AssertCode("@@@", "61477307006266733612");
    }

    // Reko: a decoder for the instruction 05677675424467661332 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05677675424467661332()
    {
        AssertCode("@@@", "05677675424467661332");
    }

    // Reko: a decoder for the instruction 12431413250321113342 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12431413250321113342()
    {
        AssertCode("@@@", "12431413250321113342");
    }

    // Reko: a decoder for the instruction 65424217464605726642 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65424217464605726642()
    {
        AssertCode("@@@", "65424217464605726642");
    }

    // Reko: a decoder for the instruction 76477741246561455673 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76477741246561455673()
    {
        AssertCode("@@@", "76477741246561455673");
    }

    // Reko: a decoder for the instruction 57757113165364711041 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57757113165364711041()
    {
        AssertCode("@@@", "57757113165364711041");
    }

    // Reko: a decoder for the instruction 75474126157023302745 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75474126157023302745()
    {
        AssertCode("@@@", "75474126157023302745");
    }

    // Reko: a decoder for the instruction 13062334441127763642 at address 01000 has not been implemented.
    

    // Reko: a decoder for the instruction 21314737002305211566 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21314737002305211566()
    {
        AssertCode("@@@", "21314737002305211566");
    }

    // Reko: a decoder for the instruction 11402052476133757077 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11402052476133757077()
    {
        AssertCode("@@@", "11402052476133757077");
    }

    // Reko: a decoder for the instruction 62213534644422315021 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62213534644422315021()
    {
        AssertCode("@@@", "62213534644422315021");
    }

    // Reko: a decoder for the instruction 64145154274320014076 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64145154274320014076()
    {
        AssertCode("@@@", "64145154274320014076");
    }

    // Reko: a decoder for the instruction 24045132114772362110 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24045132114772362110()
    {
        AssertCode("@@@", "24045132114772362110");
    }

    // Reko: a decoder for the instruction 34262041312603555250 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34262041312603555250()
    {
        AssertCode("@@@", "34262041312603555250");
    }

    // Reko: a decoder for the instruction 22206675210160055031 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22206675210160055031()
    {
        AssertCode("@@@", "22206675210160055031");
    }

    // Reko: a decoder for the instruction 42206060666175333103 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42206060666175333103()
    {
        AssertCode("@@@", "42206060666175333103");
    }

    // Reko: a decoder for the instruction 20703142111527402754 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20703142111527402754()
    {
        AssertCode("@@@", "20703142111527402754");
    }

    // Reko: a decoder for the instruction 70057306074377206511 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70057306074377206511()
    {
        AssertCode("@@@", "70057306074377206511");
    }

    // Reko: a decoder for the instruction 51624557177170012542 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51624557177170012542()
    {
        AssertCode("@@@", "51624557177170012542");
    }

    // Reko: a decoder for the instruction 72110651173100464150 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72110651173100464150()
    {
        AssertCode("@@@", "72110651173100464150");
    }

    // Reko: a decoder for the instruction 02650722705073643630 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02650722705073643630()
    {
        AssertCode("@@@", "02650722705073643630");
    }

    // Reko: a decoder for the instruction 13100670234415010273 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13100670234415010273()
    {
        AssertCode("@@@", "13100670234415010273");
    }

    // Reko: a decoder for the instruction 23651031033716032105 at address 01000 has not been implemented.
   

    // Reko: a decoder for the instruction 04241646556612123324 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04241646556612123324()
    {
        AssertCode("@@@", "04241646556612123324");
    }

    // Reko: a decoder for the instruction 23221172531715337117 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 75143576154227412117 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75143576154227412117()
    {
        AssertCode("@@@", "75143576154227412117");
    }

    // Reko: a decoder for the instruction 70460575351473706534 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70460575351473706534()
    {
        AssertCode("@@@", "70460575351473706534");
    }

    // Reko: a decoder for the instruction 41722107302457571066 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41722107302457571066()
    {
        AssertCode("@@@", "41722107302457571066");
    }

    // Reko: a decoder for the instruction 32640062146204704767 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32640062146204704767()
    {
        AssertCode("@@@", "32640062146204704767");
    }

    // Reko: a decoder for the instruction 64364400026676721342 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64364400026676721342()
    {
        AssertCode("@@@", "64364400026676721342");
    }

    // Reko: a decoder for the instruction 03507235736774123204 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03507235736774123204()
    {
        AssertCode("@@@", "03507235736774123204");
    }

    // Reko: a decoder for the instruction 72026576230412317342 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72026576230412317342()
    {
        AssertCode("@@@", "72026576230412317342");
    }

    // Reko: a decoder for the instruction 23656661112736314112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23656661112736314112()
    {
        AssertCode("@@@", "23656661112736314112");
    }

    // Reko: a decoder for the instruction 37151366017727071056 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37151366017727071056()
    {
        AssertCode("@@@", "37151366017727071056");
    }

    // Reko: a decoder for the instruction 55553525370567163346 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55553525370567163346()
    {
        AssertCode("@@@", "55553525370567163346");
    }

    // Reko: a decoder for the instruction 55612235167644144412 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55612235167644144412()
    {
        AssertCode("@@@", "55612235167644144412");
    }

    // Reko: a decoder for the instruction 04113610010452446361 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04113610010452446361()
    {
        AssertCode("@@@", "04113610010452446361");
    }

    // Reko: a decoder for the instruction 57106113330304544610 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57106113330304544610()
    {
        AssertCode("@@@", "57106113330304544610");
    }

    // Reko: a decoder for the instruction 56012101253033014302 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56012101253033014302()
    {
        AssertCode("@@@", "56012101253033014302");
    }

    // Reko: a decoder for the instruction 30331002066111407266 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30331002066111407266()
    {
        AssertCode("@@@", "30331002066111407266");
    }

    // Reko: a decoder for the instruction 44024162213071602405 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44024162213071602405()
    {
        AssertCode("@@@", "44024162213071602405");
    }

    // Reko: a decoder for the instruction 43077140002417372644 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43077140002417372644()
    {
        AssertCode("@@@", "43077140002417372644");
    }

    // Reko: a decoder for the instruction 61405260373304005324 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61405260373304005324()
    {
        AssertCode("@@@", "61405260373304005324");
    }

    // Reko: a decoder for the instruction 64205221740126311724 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64205221740126311724()
    {
        AssertCode("@@@", "64205221740126311724");
    }

    // Reko: a decoder for the instruction 35546714331321512647 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35546714331321512647()
    {
        AssertCode("@@@", "35546714331321512647");
    }

    // Reko: a decoder for the instruction 07447210132371313071 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07447210132371313071()
    {
        AssertCode("@@@", "07447210132371313071");
    }

    // Reko: a decoder for the instruction 23671613007320641464 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23671613007320641464()
    {
        AssertCode("@@@", "23671613007320641464");
    }

    // Reko: a decoder for the instruction 40071105257424550256 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40071105257424550256()
    {
        AssertCode("@@@", "40071105257424550256");
    }

    // Reko: a decoder for the instruction 12032367347700602507 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12032367347700602507()
    {
        AssertCode("@@@", "12032367347700602507");
    }

    // Reko: a decoder for the instruction 63665766061010244013 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63665766061010244013()
    {
        AssertCode("@@@", "63665766061010244013");
    }

    // Reko: a decoder for the instruction 74123116114504140144 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74123116114504140144()
    {
        AssertCode("@@@", "74123116114504140144");
    }

    // Reko: a decoder for the instruction 17023125551237733255 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17023125551237733255()
    {
        AssertCode("@@@", "17023125551237733255");
    }

    // Reko: a decoder for the instruction 25730773612047731252 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25730773612047731252()
    {
        AssertCode("@@@", "25730773612047731252");
    }

    // Reko: a decoder for the instruction 62761402761533124543 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62761402761533124543()
    {
        AssertCode("@@@", "62761402761533124543");
    }

    // Reko: a decoder for the instruction 30241502710704742047 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30241502710704742047()
    {
        AssertCode("@@@", "30241502710704742047");
    }

    // Reko: a decoder for the instruction 73773660637012177063 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73773660637012177063()
    {
        AssertCode("@@@", "73773660637012177063");
    }

    // Reko: a decoder for the instruction 44502032024222213371 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44502032024222213371()
    {
        AssertCode("@@@", "44502032024222213371");
    }

    // Reko: a decoder for the instruction 34272613134041463424 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34272613134041463424()
    {
        AssertCode("@@@", "34272613134041463424");
    }

    // Reko: a decoder for the instruction 63556320642557044372 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63556320642557044372()
    {
        AssertCode("@@@", "63556320642557044372");
    }

    // Reko: a decoder for the instruction 42152553602722037327 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42152553602722037327()
    {
        AssertCode("@@@", "42152553602722037327");
    }

    // Reko: a decoder for the instruction 51451273026223310445 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51451273026223310445()
    {
        AssertCode("@@@", "51451273026223310445");
    }

    // Reko: a decoder for the instruction 17116043233413755174 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 32700233367631700622 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32700233367631700622()
    {
        AssertCode("@@@", "32700233367631700622");
    }

    // Reko: a decoder for the instruction 61501427434340165234 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61501427434340165234()
    {
        AssertCode("@@@", "61501427434340165234");
    }

    // Reko: a decoder for the instruction 40126732316407615164 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40126732316407615164()
    {
        AssertCode("@@@", "40126732316407615164");
    }

    // Reko: a decoder for the instruction 11632574144162407100 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11632574144162407100()
    {
        AssertCode("@@@", "11632574144162407100");
    }

    // Reko: a decoder for the instruction 32443350070500076273 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32443350070500076273()
    {
        AssertCode("@@@", "32443350070500076273");
    }

    // Reko: a decoder for the instruction 60162740610425160364 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60162740610425160364()
    {
        AssertCode("@@@", "60162740610425160364");
    }

    // Reko: a decoder for the instruction 13702051312360171635 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13702051312360171635()
    {
        AssertCode("@@@", "13702051312360171635");
    }

    // Reko: a decoder for the instruction 23432215555473376301 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23432215555473376301()
    {
        AssertCode("@@@", "23432215555473376301");
    }

    // Reko: a decoder for the instruction 00034535636455332447 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 37555574301750635562 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37555574301750635562()
    {
        AssertCode("@@@", "37555574301750635562");
    }

    // Reko: a decoder for the instruction 76265217515243113101 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76265217515243113101()
    {
        AssertCode("@@@", "76265217515243113101");
    }

    // Reko: a decoder for the instruction 25234605745003111234 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25234605745003111234()
    {
        AssertCode("@@@", "25234605745003111234");
    }

    // Reko: a decoder for the instruction 11737240542720410733 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11737240542720410733()
    {
        AssertCode("@@@", "11737240542720410733");
    }

    // Reko: a decoder for the instruction 03066035145002630004 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 22371710725626741440 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22371710725626741440()
    {
        AssertCode("@@@", "22371710725626741440");
    }

    // Reko: a decoder for the instruction 16451037020712517235 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16451037020712517235()
    {
        AssertCode("@@@", "16451037020712517235");
    }

    // Reko: a decoder for the instruction 51067142601402075071 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51067142601402075071()
    {
        AssertCode("@@@", "51067142601402075071");
    }

    // Reko: a decoder for the instruction 47504073466775712664 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47504073466775712664()
    {
        AssertCode("@@@", "47504073466775712664");
    }

    // Reko: a decoder for the instruction 67050546046430032137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67050546046430032137()
    {
        AssertCode("@@@", "67050546046430032137");
    }

    // Reko: a decoder for the instruction 46767254653527072460 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46767254653527072460()
    {
        AssertCode("@@@", "46767254653527072460");
    }

    // Reko: a decoder for the instruction 70062157370057614450 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70062157370057614450()
    {
        AssertCode("@@@", "70062157370057614450");
    }

    // Reko: a decoder for the instruction 62133565677764003512 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62133565677764003512()
    {
        AssertCode("@@@", "62133565677764003512");
    }

    // Reko: a decoder for the instruction 10516317520756133451 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10516317520756133451()
    {
        AssertCode("@@@", "10516317520756133451");
    }

    // Reko: a decoder for the instruction 00226313170633052064 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 60625727034605462234 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60625727034605462234()
    {
        AssertCode("@@@", "60625727034605462234");
    }

    // Reko: a decoder for the instruction 23137310715011106137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23137310715011106137()
    {
        AssertCode("@@@", "23137310715011106137");
    }

    // Reko: a decoder for the instruction 73066654264243010403 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73066654264243010403()
    {
        AssertCode("@@@", "73066654264243010403");
    }

    // Reko: a decoder for the instruction 46401753011620613633 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 20131324004326664664 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20131324004326664664()
    {
        AssertCode("@@@", "20131324004326664664");
    }

    // Reko: a decoder for the instruction 77371661361712265612 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77371661361712265612()
    {
        AssertCode("@@@", "77371661361712265612");
    }

    // Reko: a decoder for the instruction 17334626345240361641 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17334626345240361641()
    {
        AssertCode("@@@", "17334626345240361641");
    }

    // Reko: a decoder for the instruction 66663460266714427477 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66663460266714427477()
    {
        AssertCode("@@@", "66663460266714427477");
    }

    // Reko: a decoder for the instruction 14127256736535130037 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14127256736535130037()
    {
        AssertCode("@@@", "14127256736535130037");
    }

    // Reko: a decoder for the instruction 75377507473506402361 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75377507473506402361()
    {
        AssertCode("@@@", "75377507473506402361");
    }

    // Reko: a decoder for the instruction 36042760460112057325 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36042760460112057325()
    {
        AssertCode("@@@", "36042760460112057325");
    }

    // Reko: a decoder for the instruction 22150604610571506404 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22150604610571506404()
    {
        AssertCode("@@@", "22150604610571506404");
    }

    // Reko: a decoder for the instruction 64546156477766666562 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64546156477766666562()
    {
        AssertCode("@@@", "64546156477766666562");
    }

    // Reko: a decoder for the instruction 01735301441314437112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_01735301441314437112()
    {
        AssertCode("@@@", "01735301441314437112");
    }

    // Reko: a decoder for the instruction 21671160014307537005 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21671160014307537005()
    {
        AssertCode("@@@", "21671160014307537005");
    }

    // Reko: a decoder for the instruction 07356122354544214603 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07356122354544214603()
    {
        AssertCode("@@@", "07356122354544214603");
    }

    // Reko: a decoder for the instruction 70574275134751420022 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70574275134751420022()
    {
        AssertCode("@@@", "70574275134751420022");
    }

    // Reko: a decoder for the instruction 23115651604464111244 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23115651604464111244()
    {
        AssertCode("@@@", "23115651604464111244");
    }

    // Reko: a decoder for the instruction 32627220652734606714 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32627220652734606714()
    {
        AssertCode("@@@", "32627220652734606714");
    }

    // Reko: a decoder for the instruction 47671277342005717421 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47671277342005717421()
    {
        AssertCode("@@@", "47671277342005717421");
    }

    // Reko: a decoder for the instruction 22742710140437070100 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22742710140437070100()
    {
        AssertCode("@@@", "22742710140437070100");
    }

    // Reko: a decoder for the instruction 51546552706146754670 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51546552706146754670()
    {
        AssertCode("@@@", "51546552706146754670");
    }

    // Reko: a decoder for the instruction 74756523345400103367 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74756523345400103367()
    {
        AssertCode("@@@", "74756523345400103367");
    }

    // Reko: a decoder for the instruction 13307467760606757603 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13307467760606757603()
    {
        AssertCode("@@@", "13307467760606757603");
    }

    // Reko: a decoder for the instruction 23417476452540126235 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23417476452540126235()
    {
        AssertCode("@@@", "23417476452540126235");
    }

    // Reko: a decoder for the instruction 01252115611627153774 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_01252115611627153774()
    {
        AssertCode("@@@", "01252115611627153774");
    }

    // Reko: a decoder for the instruction 61707616264672447724 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61707616264672447724()
    {
        AssertCode("@@@", "61707616264672447724");
    }

    // Reko: a decoder for the instruction 51572517446763045510 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51572517446763045510()
    {
        AssertCode("@@@", "51572517446763045510");
    }

    // Reko: a decoder for the instruction 33256137141413046033 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33256137141413046033()
    {
        AssertCode("@@@", "33256137141413046033");
    }

    // Reko: a decoder for the instruction 17567315771606127247 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17567315771606127247()
    {
        AssertCode("@@@", "17567315771606127247");
    }

    // Reko: a decoder for the instruction 74567617163013673213 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74567617163013673213()
    {
        AssertCode("@@@", "74567617163013673213");
    }

    // Reko: a decoder for the instruction 45403327160240146545 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45403327160240146545()
    {
        AssertCode("@@@", "45403327160240146545");
    }

    // Reko: a decoder for the instruction 24430413442321567724 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24430413442321567724()
    {
        AssertCode("@@@", "24430413442321567724");
    }

    // Reko: a decoder for the instruction 74034502224310501225 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74034502224310501225()
    {
        AssertCode("@@@", "74034502224310501225");
    }

    // Reko: a decoder for the instruction 24660651004375123774 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24660651004375123774()
    {
        AssertCode("@@@", "24660651004375123774");
    }

    // Reko: a decoder for the instruction 54547477460155552644 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54547477460155552644()
    {
        AssertCode("@@@", "54547477460155552644");
    }

    // Reko: a decoder for the instruction 14204530656365774761 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14204530656365774761()
    {
        AssertCode("@@@", "14204530656365774761");
    }

    // Reko: a decoder for the instruction 04264465171726313553 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04264465171726313553()
    {
        AssertCode("@@@", "04264465171726313553");
    }

    // Reko: a decoder for the instruction 42346300447603657613 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42346300447603657613()
    {
        AssertCode("@@@", "42346300447603657613");
    }

    // Reko: a decoder for the instruction 56622476154715511500 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56622476154715511500()
    {
        AssertCode("@@@", "56622476154715511500");
    }

    // Reko: a decoder for the instruction 25657037302373162702 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25657037302373162702()
    {
        AssertCode("@@@", "25657037302373162702");
    }

    // Reko: a decoder for the instruction 42766661507565551673 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42766661507565551673()
    {
        AssertCode("@@@", "42766661507565551673");
    }

    // Reko: a decoder for the instruction 35351377371601165176 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35351377371601165176()
    {
        AssertCode("@@@", "35351377371601165176");
    }

    // Reko: a decoder for the instruction 10635527501205560142 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10635527501205560142()
    {
        AssertCode("@@@", "10635527501205560142");
    }

    // Reko: a decoder for the instruction 60201637151763546212 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60201637151763546212()
    {
        AssertCode("@@@", "60201637151763546212");
    }

    // Reko: a decoder for the instruction 32211736432275656531 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32211736432275656531()
    {
        AssertCode("@@@", "32211736432275656531");
    }

    // Reko: a decoder for the instruction 76305125663560247117 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76305125663560247117()
    {
        AssertCode("@@@", "76305125663560247117");
    }

    // Reko: a decoder for the instruction 24455002645545565064 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 47744327371003033626 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47744327371003033626()
    {
        AssertCode("@@@", "47744327371003033626");
    }

    // Reko: a decoder for the instruction 32442305556647703033 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32442305556647703033()
    {
        AssertCode("@@@", "32442305556647703033");
    }

    // Reko: a decoder for the instruction 65326101344500030350 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65326101344500030350()
    {
        AssertCode("@@@", "65326101344500030350");
    }

    // Reko: a decoder for the instruction 13507526346447456060 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13507526346447456060()
    {
        AssertCode("@@@", "13507526346447456060");
    }

    // Reko: a decoder for the instruction 35363167242027461555 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35363167242027461555()
    {
        AssertCode("@@@", "35363167242027461555");
    }

    // Reko: a decoder for the instruction 00331707531145616717 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 04664604314340171760 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04664604314340171760()
    {
        AssertCode("@@@", "04664604314340171760");
    }

    // Reko: a decoder for the instruction 30077653127010033470 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30077653127010033470()
    {
        AssertCode("@@@", "30077653127010033470");
    }

    // Reko: a decoder for the instruction 10065540021446240624 at address 01000 has not been implemented.
    

    // Reko: a decoder for the instruction 41344525047435101342 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41344525047435101342()
    {
        AssertCode("@@@", "41344525047435101342");
    }

    // Reko: a decoder for the instruction 62163127734346603153 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62163127734346603153()
    {
        AssertCode("@@@", "62163127734346603153");
    }

    // Reko: a decoder for the instruction 50472652341121012017 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50472652341121012017()
    {
        AssertCode("@@@", "50472652341121012017");
    }

    // Reko: a decoder for the instruction 47053625443064730223 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47053625443064730223()
    {
        AssertCode("@@@", "47053625443064730223");
    }

    // Reko: a decoder for the instruction 30562332573655027071 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 43015501602647233073 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43015501602647233073()
    {
        AssertCode("@@@", "43015501602647233073");
    }

    // Reko: a decoder for the instruction 34741343717302444722 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34741343717302444722()
    {
        AssertCode("@@@", "34741343717302444722");
    }

    // Reko: a decoder for the instruction 70552050604002746267 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70552050604002746267()
    {
        AssertCode("@@@", "70552050604002746267");
    }

    // Reko: a decoder for the instruction 71544727662105757533 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71544727662105757533()
    {
        AssertCode("@@@", "71544727662105757533");
    }

    // Reko: a decoder for the instruction 61207660002737175702 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61207660002737175702()
    {
        AssertCode("@@@", "61207660002737175702");
    }

    // Reko: a decoder for the instruction 26565031044144176445 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26565031044144176445()
    {
        AssertCode("@@@", "26565031044144176445");
    }

    // Reko: a decoder for the instruction 61364256560123223545 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61364256560123223545()
    {
        AssertCode("@@@", "61364256560123223545");
    }

    // Reko: a decoder for the instruction 72050415256106237612 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72050415256106237612()
    {
        AssertCode("@@@", "72050415256106237612");
    }

    // Reko: a decoder for the instruction 63403551172740313243 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63403551172740313243()
    {
        AssertCode("@@@", "63403551172740313243");
    }

    // Reko: a decoder for the instruction 10330215551661646700 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10330215551661646700()
    {
        AssertCode("@@@", "10330215551661646700");
    }

    // Reko: a decoder for the instruction 46502263055550312560 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46502263055550312560()
    {
        AssertCode("@@@", "46502263055550312560");
    }

    // Reko: a decoder for the instruction 45520614007716541023 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45520614007716541023()
    {
        AssertCode("@@@", "45520614007716541023");
    }

    // Reko: a decoder for the instruction 60410673676054650621 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60410673676054650621()
    {
        AssertCode("@@@", "60410673676054650621");
    }

    // Reko: a decoder for the instruction 61216042661267531242 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61216042661267531242()
    {
        AssertCode("@@@", "61216042661267531242");
    }

    // Reko: a decoder for the instruction 00251014470415505725 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 51570634302321704161 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51570634302321704161()
    {
        AssertCode("@@@", "51570634302321704161");
    }

    // Reko: a decoder for the instruction 61773412633342672627 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61773412633342672627()
    {
        AssertCode("@@@", "61773412633342672627");
    }

    // Reko: a decoder for the instruction 60056434066515202666 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60056434066515202666()
    {
        AssertCode("@@@", "60056434066515202666");
    }

    // Reko: a decoder for the instruction 21522617665412471761 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21522617665412471761()
    {
        AssertCode("@@@", "21522617665412471761");
    }

    // Reko: a decoder for the instruction 52521442631344507450 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52521442631344507450()
    {
        AssertCode("@@@", "52521442631344507450");
    }

    // Reko: a decoder for the instruction 54150421767551513356 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54150421767551513356()
    {
        AssertCode("@@@", "54150421767551513356");
    }

    // Reko: a decoder for the instruction 23021531117255602215 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23021531117255602215()
    {
        AssertCode("@@@", "23021531117255602215");
    }

    // Reko: a decoder for the instruction 17754010715355230424 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17754010715355230424()
    {
        AssertCode("@@@", "17754010715355230424");
    }

    // Reko: a decoder for the instruction 64630366534474134671 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64630366534474134671()
    {
        AssertCode("@@@", "64630366534474134671");
    }

    // Reko: a decoder for the instruction 57373071646066461542 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57373071646066461542()
    {
        AssertCode("@@@", "57373071646066461542");
    }

    // Reko: a decoder for the instruction 02726212635534001460 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02726212635534001460()
    {
        AssertCode("@@@", "02726212635534001460");
    }

    // Reko: a decoder for the instruction 63342150161447715772 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63342150161447715772()
    {
        AssertCode("@@@", "63342150161447715772");
    }

    // Reko: a decoder for the instruction 13471557301225116124 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13471557301225116124()
    {
        AssertCode("@@@", "13471557301225116124");
    }

    // Reko: a decoder for the instruction 33734262175643263346 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33734262175643263346()
    {
        AssertCode("@@@", "33734262175643263346");
    }

    // Reko: a decoder for the instruction 60511301161037664236 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60511301161037664236()
    {
        AssertCode("@@@", "60511301161037664236");
    }

    // Reko: a decoder for the instruction 44222403023213237760 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44222403023213237760()
    {
        AssertCode("@@@", "44222403023213237760");
    }

    // Reko: a decoder for the instruction 03346540335500056670 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03346540335500056670()
    {
        AssertCode("@@@", "03346540335500056670");
    }

    // Reko: a decoder for the instruction 15644776072171050454 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15644776072171050454()
    {
        AssertCode("@@@", "15644776072171050454");
    }

    // Reko: a decoder for the instruction 15723711272725552274 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15723711272725552274()
    {
        AssertCode("@@@", "15723711272725552274");
    }

    // Reko: a decoder for the instruction 20373352736745316145 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20373352736745316145()
    {
        AssertCode("@@@", "20373352736745316145");
    }

    // Reko: a decoder for the instruction 11102152422501751431 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11102152422501751431()
    {
        AssertCode("@@@", "11102152422501751431");
    }

    // Reko: a decoder for the instruction 24137351606002004633 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24137351606002004633()
    {
        AssertCode("@@@", "24137351606002004633");
    }

    // Reko: a decoder for the instruction 52237313631343165742 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52237313631343165742()
    {
        AssertCode("@@@", "52237313631343165742");
    }

    // Reko: a decoder for the instruction 12331507724622671574 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12331507724622671574()
    {
        AssertCode("@@@", "12331507724622671574");
    }

    // Reko: a decoder for the instruction 33506046355161404567 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33506046355161404567()
    {
        AssertCode("@@@", "33506046355161404567");
    }

    // Reko: a decoder for the instruction 36561527103530670653 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36561527103530670653()
    {
        AssertCode("@@@", "36561527103530670653");
    }

    // Reko: a decoder for the instruction 16445215716222372425 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16445215716222372425()
    {
        AssertCode("@@@", "16445215716222372425");
    }

    // Reko: a decoder for the instruction 45441750055234204056 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 62615456620107540245 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62615456620107540245()
    {
        AssertCode("@@@", "62615456620107540245");
    }

    // Reko: a decoder for the instruction 73332443071734650615 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73332443071734650615()
    {
        AssertCode("@@@", "73332443071734650615");
    }

    // Reko: a decoder for the instruction 21611373777363454162 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21611373777363454162()
    {
        AssertCode("@@@", "21611373777363454162");
    }

    // Reko: a decoder for the instruction 36654626236306730762 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36654626236306730762()
    {
        AssertCode("@@@", "36654626236306730762");
    }

    // Reko: a decoder for the instruction 35654135370053226320 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35654135370053226320()
    {
        AssertCode("@@@", "35654135370053226320");
    }

    // Reko: a decoder for the instruction 34747201240337343520 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34747201240337343520()
    {
        AssertCode("@@@", "34747201240337343520");
    }

    // Reko: a decoder for the instruction 42043323521675717630 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 36464171075536710147 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36464171075536710147()
    {
        AssertCode("@@@", "36464171075536710147");
    }

    // Reko: a decoder for the instruction 10734715430430621366 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10734715430430621366()
    {
        AssertCode("@@@", "10734715430430621366");
    }

    // Reko: a decoder for the instruction 64102456472404571420 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64102456472404571420()
    {
        AssertCode("@@@", "64102456472404571420");
    }

    // Reko: a decoder for the instruction 11625333127262017257 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11625333127262017257()
    {
        AssertCode("@@@", "11625333127262017257");
    }

    // Reko: a decoder for the instruction 53727445107540434402 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53727445107540434402()
    {
        AssertCode("@@@", "53727445107540434402");
    }

    // Reko: a decoder for the instruction 42576146322527413215 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42576146322527413215()
    {
        AssertCode("@@@", "42576146322527413215");
    }

    // Reko: a decoder for the instruction 57525364330231567046 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57525364330231567046()
    {
        AssertCode("@@@", "57525364330231567046");
    }

    // Reko: a decoder for the instruction 00473251274111705202 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 46456331046645746311 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 62267260556127007054 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62267260556127007054()
    {
        AssertCode("@@@", "62267260556127007054");
    }

    // Reko: a decoder for the instruction 21555545504533614760 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21555545504533614760()
    {
        AssertCode("@@@", "21555545504533614760");
    }

    // Reko: a decoder for the instruction 26511614601734033764 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26511614601734033764()
    {
        AssertCode("@@@", "26511614601734033764");
    }

    // Reko: a decoder for the instruction 26205300564563241315 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26205300564563241315()
    {
        AssertCode("@@@", "26205300564563241315");
    }

    // Reko: a decoder for the instruction 65423755453032623756 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65423755453032623756()
    {
        AssertCode("@@@", "65423755453032623756");
    }

    // Reko: a decoder for the instruction 54434116232062355065 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54434116232062355065()
    {
        AssertCode("@@@", "54434116232062355065");
    }

    // Reko: a decoder for the instruction 41301453636334250273 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41301453636334250273()
    {
        AssertCode("@@@", "41301453636334250273");
    }

    // Reko: a decoder for the instruction 53467176606061617244 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53467176606061617244()
    {
        AssertCode("@@@", "53467176606061617244");
    }

    // Reko: a decoder for the instruction 74746302613440721511 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74746302613440721511()
    {
        AssertCode("@@@", "74746302613440721511");
    }

    // Reko: a decoder for the instruction 77006120057124623160 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77006120057124623160()
    {
        AssertCode("@@@", "77006120057124623160");
    }

    // Reko: a decoder for the instruction 21706655261304357705 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21706655261304357705()
    {
        AssertCode("@@@", "21706655261304357705");
    }

    // Reko: a decoder for the instruction 67461333447117422564 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67461333447117422564()
    {
        AssertCode("@@@", "67461333447117422564");
    }

    // Reko: a decoder for the instruction 26112544045765016131 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26112544045765016131()
    {
        AssertCode("@@@", "26112544045765016131");
    }

    // Reko: a decoder for the instruction 41427110010346700474 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41427110010346700474()
    {
        AssertCode("@@@", "41427110010346700474");
    }

    // Reko: a decoder for the instruction 26567703235770252156 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26567703235770252156()
    {
        AssertCode("@@@", "26567703235770252156");
    }

    // Reko: a decoder for the instruction 33264757061605143704 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33264757061605143704()
    {
        AssertCode("@@@", "33264757061605143704");
    }

    // Reko: a decoder for the instruction 54210166602307721012 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54210166602307721012()
    {
        AssertCode("@@@", "54210166602307721012");
    }

    // Reko: a decoder for the instruction 65041431740335240371 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65041431740335240371()
    {
        AssertCode("@@@", "65041431740335240371");
    }

    // Reko: a decoder for the instruction 07640123574174051546 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07640123574174051546()
    {
        AssertCode("@@@", "07640123574174051546");
    }

    // Reko: a decoder for the instruction 13344255665147066002 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13344255665147066002()
    {
        AssertCode("@@@", "13344255665147066002");
    }

    // Reko: a decoder for the instruction 62513164214073246073 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62513164214073246073()
    {
        AssertCode("@@@", "62513164214073246073");
    }

    // Reko: a decoder for the instruction 15415355430166275035 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15415355430166275035()
    {
        AssertCode("@@@", "15415355430166275035");
    }

    // Reko: a decoder for the instruction 10017464023636042764 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10017464023636042764()
    {
        AssertCode("@@@", "10017464023636042764");
    }

    // Reko: a decoder for the instruction 50720326756704757424 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50720326756704757424()
    {
        AssertCode("@@@", "50720326756704757424");
    }

    // Reko: a decoder for the instruction 17127727173764263213 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17127727173764263213()
    {
        AssertCode("@@@", "17127727173764263213");
    }

    // Reko: a decoder for the instruction 55640227121551776524 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55640227121551776524()
    {
        AssertCode("@@@", "55640227121551776524");
    }

    // Reko: a decoder for the instruction 16547073334127246772 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16547073334127246772()
    {
        AssertCode("@@@", "16547073334127246772");
    }

    // Reko: a decoder for the instruction 45263116043135414612 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45263116043135414612()
    {
        AssertCode("@@@", "45263116043135414612");
    }

    // Reko: a decoder for the instruction 15503762366307225757 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15503762366307225757()
    {
        AssertCode("@@@", "15503762366307225757");
    }

    // Reko: a decoder for the instruction 37253467220324346452 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37253467220324346452()
    {
        AssertCode("@@@", "37253467220324346452");
    }

    // Reko: a decoder for the instruction 30703136602564576211 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30703136602564576211()
    {
        AssertCode("@@@", "30703136602564576211");
    }

    // Reko: a decoder for the instruction 16761270650015125462 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16761270650015125462()
    {
        AssertCode("@@@", "16761270650015125462");
    }

    // Reko: a decoder for the instruction 27027037322415567137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27027037322415567137()
    {
        AssertCode("@@@", "27027037322415567137");
    }

    // Reko: a decoder for the instruction 54167762016401111523 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54167762016401111523()
    {
        AssertCode("@@@", "54167762016401111523");
    }

    // Reko: a decoder for the instruction 77116057027557232452 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77116057027557232452()
    {
        AssertCode("@@@", "77116057027557232452");
    }

    // Reko: a decoder for the instruction 12621617323347032675 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12621617323347032675()
    {
        AssertCode("@@@", "12621617323347032675");
    }

    // Reko: a decoder for the instruction 62252041260454457114 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62252041260454457114()
    {
        AssertCode("@@@", "62252041260454457114");
    }

    // Reko: a decoder for the instruction 11544174360207317776 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11544174360207317776()
    {
        AssertCode("@@@", "11544174360207317776");
    }

    // Reko: a decoder for the instruction 30643734625434745751 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30643734625434745751()
    {
        AssertCode("@@@", "30643734625434745751");
    }

    // Reko: a decoder for the instruction 54207361161336152300 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54207361161336152300()
    {
        AssertCode("@@@", "54207361161336152300");
    }

    // Reko: a decoder for the instruction 63015431706237777114 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63015431706237777114()
    {
        AssertCode("@@@", "63015431706237777114");
    }

    // Reko: a decoder for the instruction 40660403424260505440 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40660403424260505440()
    {
        AssertCode("@@@", "40660403424260505440");
    }

    // Reko: a decoder for the instruction 71517762732623200152 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71517762732623200152()
    {
        AssertCode("@@@", "71517762732623200152");
    }

    // Reko: a decoder for the instruction 36774504167511001465 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36774504167511001465()
    {
        AssertCode("@@@", "36774504167511001465");
    }

    // Reko: a decoder for the instruction 45466542725156266112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45466542725156266112()
    {
        AssertCode("@@@", "45466542725156266112");
    }

    // Reko: a decoder for the instruction 74055743443001423573 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74055743443001423573()
    {
        AssertCode("@@@", "74055743443001423573");
    }

    // Reko: a decoder for the instruction 14213403661433461450 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14213403661433461450()
    {
        AssertCode("@@@", "14213403661433461450");
    }

    // Reko: a decoder for the instruction 27577375566745026620 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27577375566745026620()
    {
        AssertCode("@@@", "27577375566745026620");
    }

    // Reko: a decoder for the instruction 11111725344254550013 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11111725344254550013()
    {
        AssertCode("@@@", "11111725344254550013");
    }

    // Reko: a decoder for the instruction 60111603372224171370 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60111603372224171370()
    {
        AssertCode("@@@", "60111603372224171370");
    }

    // Reko: a decoder for the instruction 70604370314762661154 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70604370314762661154()
    {
        AssertCode("@@@", "70604370314762661154");
    }

    // Reko: a decoder for the instruction 40440622250165526175 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40440622250165526175()
    {
        AssertCode("@@@", "40440622250165526175");
    }

    // Reko: a decoder for the instruction 56015352520233711521 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56015352520233711521()
    {
        AssertCode("@@@", "56015352520233711521");
    }

    // Reko: a decoder for the instruction 61742474652462362353 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61742474652462362353()
    {
        AssertCode("@@@", "61742474652462362353");
    }

    // Reko: a decoder for the instruction 64343471330572211017 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64343471330572211017()
    {
        AssertCode("@@@", "64343471330572211017");
    }

    // Reko: a decoder for the instruction 46154020635464232553 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46154020635464232553()
    {
        AssertCode("@@@", "46154020635464232553");
    }

    // Reko: a decoder for the instruction 44712066443332440554 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44712066443332440554()
    {
        AssertCode("@@@", "44712066443332440554");
    }

    // Reko: a decoder for the instruction 65377251614570345656 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65377251614570345656()
    {
        AssertCode("@@@", "65377251614570345656");
    }

    // Reko: a decoder for the instruction 60475257205701377735 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60475257205701377735()
    {
        AssertCode("@@@", "60475257205701377735");
    }

    // Reko: a decoder for the instruction 70300076730516332162 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70300076730516332162()
    {
        AssertCode("@@@", "70300076730516332162");
    }

    // Reko: a decoder for the instruction 75602160153441275466 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75602160153441275466()
    {
        AssertCode("@@@", "75602160153441275466");
    }

    // Reko: a decoder for the instruction 57252542406214114057 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57252542406214114057()
    {
        AssertCode("@@@", "57252542406214114057");
    }

    // Reko: a decoder for the instruction 02263612776334461702 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02263612776334461702()
    {
        AssertCode("@@@", "02263612776334461702");
    }

    // Reko: a decoder for the instruction 43512732241237050256 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43512732241237050256()
    {
        AssertCode("@@@", "43512732241237050256");
    }

    // Reko: a decoder for the instruction 34033621761727160403 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34033621761727160403()
    {
        AssertCode("@@@", "34033621761727160403");
    }

    // Reko: a decoder for the instruction 21170654334004322510 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21170654334004322510()
    {
        AssertCode("@@@", "21170654334004322510");
    }

    // Reko: a decoder for the instruction 22432606325013432334 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22432606325013432334()
    {
        AssertCode("@@@", "22432606325013432334");
    }

    // Reko: a decoder for the instruction 63533232614304427354 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63533232614304427354()
    {
        AssertCode("@@@", "63533232614304427354");
    }

    // Reko: a decoder for the instruction 72400646265736131775 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72400646265736131775()
    {
        AssertCode("@@@", "72400646265736131775");
    }

    // Reko: a decoder for the instruction 14211302544774032047 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14211302544774032047()
    {
        AssertCode("@@@", "14211302544774032047");
    }

    // Reko: a decoder for the instruction 56736527366773577432 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56736527366773577432()
    {
        AssertCode("@@@", "56736527366773577432");
    }

    // Reko: a decoder for the instruction 62374021515517655153 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62374021515517655153()
    {
        AssertCode("@@@", "62374021515517655153");
    }

    // Reko: a decoder for the instruction 33511741717002641405 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33511741717002641405()
    {
        AssertCode("@@@", "33511741717002641405");
    }

    // Reko: a decoder for the instruction 27174573215201050710 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27174573215201050710()
    {
        AssertCode("@@@", "27174573215201050710");
    }

    // Reko: a decoder for the instruction 61010267605724436702 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61010267605724436702()
    {
        AssertCode("@@@", "61010267605724436702");
    }

    // Reko: a decoder for the instruction 37153223525270011773 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37153223525270011773()
    {
        AssertCode("@@@", "37153223525270011773");
    }

    // Reko: a decoder for the instruction 13247321324512313435 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13247321324512313435()
    {
        AssertCode("@@@", "13247321324512313435");
    }

    // Reko: a decoder for the instruction 42016255774525373045 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42016255774525373045()
    {
        AssertCode("@@@", "42016255774525373045");
    }

    // Reko: a decoder for the instruction 25574715142226456450 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25574715142226456450()
    {
        AssertCode("@@@", "25574715142226456450");
    }

    // Reko: a decoder for the instruction 60501337441432120464 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60501337441432120464()
    {
        AssertCode("@@@", "60501337441432120464");
    }

    // Reko: a decoder for the instruction 41145410574667646516 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41145410574667646516()
    {
        AssertCode("@@@", "41145410574667646516");
    }

    // Reko: a decoder for the instruction 13251265214506660523 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13251265214506660523()
    {
        AssertCode("@@@", "13251265214506660523");
    }

    // Reko: a decoder for the instruction 01221166064604645540 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 52064031422067514375 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52064031422067514375()
    {
        AssertCode("@@@", "52064031422067514375");
    }

    // Reko: a decoder for the instruction 54437047430516654567 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54437047430516654567()
    {
        AssertCode("@@@", "54437047430516654567");
    }

    // Reko: a decoder for the instruction 14031777611253732361 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14031777611253732361()
    {
        AssertCode("@@@", "14031777611253732361");
    }

    // Reko: a decoder for the instruction 12602342400047025227 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12602342400047025227()
    {
        AssertCode("@@@", "12602342400047025227");
    }

    // Reko: a decoder for the instruction 05101232350243332254 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05101232350243332254()
    {
        AssertCode("@@@", "05101232350243332254");
    }

    // Reko: a decoder for the instruction 45751344107526007417 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45751344107526007417()
    {
        AssertCode("@@@", "45751344107526007417");
    }

    // Reko: a decoder for the instruction 21655223752057147201 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21655223752057147201()
    {
        AssertCode("@@@", "21655223752057147201");
    }

    // Reko: a decoder for the instruction 11265612416407345772 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11265612416407345772()
    {
        AssertCode("@@@", "11265612416407345772");
    }

    // Reko: a decoder for the instruction 12101424043621530104 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12101424043621530104()
    {
        AssertCode("@@@", "12101424043621530104");
    }

    // Reko: a decoder for the instruction 11745133343353660267 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11745133343353660267()
    {
        AssertCode("@@@", "11745133343353660267");
    }

    // Reko: a decoder for the instruction 66202263104645455371 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66202263104645455371()
    {
        AssertCode("@@@", "66202263104645455371");
    }

    // Reko: a decoder for the instruction 37412053760761357322 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37412053760761357322()
    {
        AssertCode("@@@", "37412053760761357322");
    }

    // Reko: a decoder for the instruction 31620152345343411633 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_31620152345343411633()
    {
        AssertCode("@@@", "31620152345343411633");
    }

    // Reko: a decoder for the instruction 65543652501562653573 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65543652501562653573()
    {
        AssertCode("@@@", "65543652501562653573");
    }

    // Reko: a decoder for the instruction 64205661664727115427 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64205661664727115427()
    {
        AssertCode("@@@", "64205661664727115427");
    }

    // Reko: a decoder for the instruction 71122106305042456770 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71122106305042456770()
    {
        AssertCode("@@@", "71122106305042456770");
    }

    // Reko: a decoder for the instruction 04423615222105340443 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04423615222105340443()
    {
        AssertCode("@@@", "04423615222105340443");
    }

    // Reko: a decoder for the instruction 10452531746775452777 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10452531746775452777()
    {
        AssertCode("@@@", "10452531746775452777");
    }

    // Reko: a decoder for the instruction 74016203150053573434 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74016203150053573434()
    {
        AssertCode("@@@", "74016203150053573434");
    }

    // Reko: a decoder for the instruction 15723322511464405761 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15723322511464405761()
    {
        AssertCode("@@@", "15723322511464405761");
    }

    // Reko: a decoder for the instruction 21227401241631423353 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21227401241631423353()
    {
        AssertCode("@@@", "21227401241631423353");
    }

    // Reko: a decoder for the instruction 75355027174736020645 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75355027174736020645()
    {
        AssertCode("@@@", "75355027174736020645");
    }

    // Reko: a decoder for the instruction 71735237256637243776 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71735237256637243776()
    {
        AssertCode("@@@", "71735237256637243776");
    }

    // Reko: a decoder for the instruction 46610647733503454671 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46610647733503454671()
    {
        AssertCode("@@@", "46610647733503454671");
    }

    // Reko: a decoder for the instruction 45241305670132141312 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45241305670132141312()
    {
        AssertCode("@@@", "45241305670132141312");
    }

    // Reko: a decoder for the instruction 42546204607067341114 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42546204607067341114()
    {
        AssertCode("@@@", "42546204607067341114");
    }

    // Reko: a decoder for the instruction 66572761222260250577 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66572761222260250577()
    {
        AssertCode("@@@", "66572761222260250577");
    }

    // Reko: a decoder for the instruction 70505346374510207573 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70505346374510207573()
    {
        AssertCode("@@@", "70505346374510207573");
    }

    // Reko: a decoder for the instruction 44567200047633053212 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44567200047633053212()
    {
        AssertCode("@@@", "44567200047633053212");
    }

    // Reko: a decoder for the instruction 43043533475533555275 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 37157360075242433736 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37157360075242433736()
    {
        AssertCode("@@@", "37157360075242433736");
    }

    // Reko: a decoder for the instruction 45177424604407136744 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45177424604407136744()
    {
        AssertCode("@@@", "45177424604407136744");
    }

    // Reko: a decoder for the instruction 52337621713442443142 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52337621713442443142()
    {
        AssertCode("@@@", "52337621713442443142");
    }

    // Reko: a decoder for the instruction 34131064257644666307 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34131064257644666307()
    {
        AssertCode("@@@", "34131064257644666307");
    }

    // Reko: a decoder for the instruction 25332035473247771646 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25332035473247771646()
    {
        AssertCode("@@@", "25332035473247771646");
    }

    // Reko: a decoder for the instruction 20273066231535356127 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20273066231535356127()
    {
        AssertCode("@@@", "20273066231535356127");
    }

    // Reko: a decoder for the instruction 71603704646407432112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71603704646407432112()
    {
        AssertCode("@@@", "71603704646407432112");
    }

    // Reko: a decoder for the instruction 05335102723443312500 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05335102723443312500()
    {
        AssertCode("@@@", "05335102723443312500");
    }

    // Reko: a decoder for the instruction 53224424273364271304 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53224424273364271304()
    {
        AssertCode("@@@", "53224424273364271304");
    }

    // Reko: a decoder for the instruction 34524257130655305221 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34524257130655305221()
    {
        AssertCode("@@@", "34524257130655305221");
    }

    // Reko: a decoder for the instruction 16704271552015305655 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16704271552015305655()
    {
        AssertCode("@@@", "16704271552015305655");
    }

    // Reko: a decoder for the instruction 37171020416654266716 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37171020416654266716()
    {
        AssertCode("@@@", "37171020416654266716");
    }

    // Reko: a decoder for the instruction 04316460047126074314 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04316460047126074314()
    {
        AssertCode("@@@", "04316460047126074314");
    }

    // Reko: a decoder for the instruction 13356115737234000415 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13356115737234000415()
    {
        AssertCode("@@@", "13356115737234000415");
    }

    // Reko: a decoder for the instruction 63612067445573702063 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63612067445573702063()
    {
        AssertCode("@@@", "63612067445573702063");
    }

    // Reko: a decoder for the instruction 43405204336532324416 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43405204336532324416()
    {
        AssertCode("@@@", "43405204336532324416");
    }

    // Reko: a decoder for the instruction 47070531721456431532 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47070531721456431532()
    {
        AssertCode("@@@", "47070531721456431532");
    }

    // Reko: a decoder for the instruction 33134502463474242740 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33134502463474242740()
    {
        AssertCode("@@@", "33134502463474242740");
    }

    // Reko: a decoder for the instruction 64646762424742512374 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64646762424742512374()
    {
        AssertCode("@@@", "64646762424742512374");
    }

    // Reko: a decoder for the instruction 01444055131003316222 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_01444055131003316222()
    {
        AssertCode("@@@", "01444055131003316222");
    }

    // Reko: a decoder for the instruction 12712722736420717530 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12712722736420717530()
    {
        AssertCode("@@@", "12712722736420717530");
    }

    // Reko: a decoder for the instruction 52427167410630156133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52427167410630156133()
    {
        AssertCode("@@@", "52427167410630156133");
    }

    // Reko: a decoder for the instruction 64261672056576152245 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64261672056576152245()
    {
        AssertCode("@@@", "64261672056576152245");
    }

    // Reko: a decoder for the instruction 30712165434262661406 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30712165434262661406()
    {
        AssertCode("@@@", "30712165434262661406");
    }

    // Reko: a decoder for the instruction 33434670631706137544 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33434670631706137544()
    {
        AssertCode("@@@", "33434670631706137544");
    }

    // Reko: a decoder for the instruction 55267370210320562256 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55267370210320562256()
    {
        AssertCode("@@@", "55267370210320562256");
    }

    // Reko: a decoder for the instruction 77563037115542215227 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77563037115542215227()
    {
        AssertCode("@@@", "77563037115542215227");
    }

    // Reko: a decoder for the instruction 25035155271565076172 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25035155271565076172()
    {
        AssertCode("@@@", "25035155271565076172");
    }

    // Reko: a decoder for the instruction 34331421056304654337 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34331421056304654337()
    {
        AssertCode("@@@", "34331421056304654337");
    }

    // Reko: a decoder for the instruction 67747433306442635476 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67747433306442635476()
    {
        AssertCode("@@@", "67747433306442635476");
    }

    // Reko: a decoder for the instruction 13470453341630523221 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13470453341630523221()
    {
        AssertCode("@@@", "13470453341630523221");
    }

    // Reko: a decoder for the instruction 54003061415533026524 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54003061415533026524()
    {
        AssertCode("@@@", "54003061415533026524");
    }

    // Reko: a decoder for the instruction 00163672476561004431 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_sub18()
    {
        AssertCode("PS, SB", "00163672476561004431");
    }

    // Reko: a decoder for the instruction 51277503243224111627 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51277503243224111627()
    {
        AssertCode("@@@", "51277503243224111627");
    }

    // Reko: a decoder for the instruction 21324673202503571232 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21324673202503571232()
    {
        AssertCode("@@@", "21324673202503571232");
    }

    // Reko: a decoder for the instruction 47146742063714161564 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47146742063714161564()
    {
        AssertCode("@@@", "47146742063714161564");
    }

    // Reko: a decoder for the instruction 51062036602331774776 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51062036602331774776()
    {
        AssertCode("@@@", "51062036602331774776");
    }

    // Reko: a decoder for the instruction 70055022017662073575 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70055022017662073575()
    {
        AssertCode("@@@", "70055022017662073575");
    }

    // Reko: a decoder for the instruction 26223274426727016606 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26223274426727016606()
    {
        AssertCode("@@@", "26223274426727016606");
    }

    // Reko: a decoder for the instruction 31551466033700656557 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_31551466033700656557()
    {
        AssertCode("@@@", "31551466033700656557");
    }

    // Reko: a decoder for the instruction 72640466313150160754 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72640466313150160754()
    {
        AssertCode("@@@", "72640466313150160754");
    }

    // Reko: a decoder for the instruction 50352042743506743122 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50352042743506743122()
    {
        AssertCode("@@@", "50352042743506743122");
    }

    // Reko: a decoder for the instruction 35060662220370152213 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35060662220370152213()
    {
        AssertCode("@@@", "35060662220370152213");
    }

    // Reko: a decoder for the instruction 66446573334527574316 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66446573334527574316()
    {
        AssertCode("@@@", "66446573334527574316");
    }

    // Reko: a decoder for the instruction 57251542067774551034 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57251542067774551034()
    {
        AssertCode("@@@", "57251542067774551034");
    }

    // Reko: a decoder for the instruction 10334226116774262634 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10334226116774262634()
    {
        AssertCode("@@@", "10334226116774262634");
    }

    // Reko: a decoder for the instruction 73751611653267035101 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73751611653267035101()
    {
        AssertCode("@@@", "73751611653267035101");
    }

    // Reko: a decoder for the instruction 62420415011766651625 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62420415011766651625()
    {
        AssertCode("@@@", "62420415011766651625");
    }

    // Reko: a decoder for the instruction 45507750570502166653 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45507750570502166653()
    {
        AssertCode("@@@", "45507750570502166653");
    }

    // Reko: a decoder for the instruction 23733100176337234133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23733100176337234133()
    {
        AssertCode("@@@", "23733100176337234133");
    }

    // Reko: a decoder for the instruction 22213202535241255576 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22213202535241255576()
    {
        AssertCode("@@@", "22213202535241255576");
    }

    // Reko: a decoder for the instruction 63540276615265464120 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63540276615265464120()
    {
        AssertCode("@@@", "63540276615265464120");
    }

    // Reko: a decoder for the instruction 41173714542202564370 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41173714542202564370()
    {
        AssertCode("@@@", "41173714542202564370");
    }

    // Reko: a decoder for the instruction 77215500167211507525 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77215500167211507525()
    {
        AssertCode("@@@", "77215500167211507525");
    }

    // Reko: a decoder for the instruction 15227044011400120660 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15227044011400120660()
    {
        AssertCode("@@@", "15227044011400120660");
    }

    // Reko: a decoder for the instruction 77537326453033624133 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77537326453033624133()
    {
        AssertCode("@@@", "77537326453033624133");
    }

    // Reko: a decoder for the instruction 31437452677253106034 at address 01000 has not been implemented.
    // Reko: a decoder for the instruction 60702034431333156666 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60702034431333156666()
    {
        AssertCode("@@@", "60702034431333156666");
    }

    // Reko: a decoder for the instruction 45404626162553355775 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45404626162553355775()
    {
        AssertCode("@@@", "45404626162553355775");
    }

    // Reko: a decoder for the instruction 50654113305162102201 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50654113305162102201()
    {
        AssertCode("@@@", "50654113305162102201");
    }

    // Reko: a decoder for the instruction 10110216341403026422 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10110216341403026422()
    {
        AssertCode("@@@", "10110216341403026422");
    }

    // Reko: a decoder for the instruction 42775346352401111256 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42775346352401111256()
    {
        AssertCode("@@@", "42775346352401111256");
    }

    // Reko: a decoder for the instruction 15252561516263442332 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15252561516263442332()
    {
        AssertCode("@@@", "15252561516263442332");
    }

    // Reko: a decoder for the instruction 45662631745074735113 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45662631745074735113()
    {
        AssertCode("@@@", "45662631745074735113");
    }

    // Reko: a decoder for the instruction 46320326527271471403 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46320326527271471403()
    {
        AssertCode("@@@", "46320326527271471403");
    }

    // Reko: a decoder for the instruction 44001752617117745343 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44001752617117745343()
    {
        AssertCode("@@@", "44001752617117745343");
    }

    // Reko: a decoder for the instruction 10466347052405451713 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10466347052405451713()
    {
        AssertCode("@@@", "10466347052405451713");
    }

    // Reko: a decoder for the instruction 25374302541746457503 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25374302541746457503()
    {
        AssertCode("@@@", "25374302541746457503");
    }

    // Reko: a decoder for the instruction 30417541756164722733 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30417541756164722733()
    {
        AssertCode("@@@", "30417541756164722733");
    }

    // Reko: a decoder for the instruction 42152442142465413111 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42152442142465413111()
    {
        AssertCode("@@@", "42152442142465413111");
    }

    // Reko: a decoder for the instruction 05164011443566161332 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05164011443566161332()
    {
        AssertCode("@@@", "05164011443566161332");
    }

    // Reko: a decoder for the instruction 07127151422055703177 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07127151422055703177()
    {
        AssertCode("@@@", "07127151422055703177");
    }

    // Reko: a decoder for the instruction 66173760170053671052 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66173760170053671052()
    {
        AssertCode("@@@", "66173760170053671052");
    }

    // Reko: a decoder for the instruction 23562603434012522315 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23562603434012522315()
    {
        AssertCode("@@@", "23562603434012522315");
    }

    // Reko: a decoder for the instruction 03550743216420622646 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 27410231525475635545 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27410231525475635545()
    {
        AssertCode("@@@", "27410231525475635545");
    }

    // Reko: a decoder for the instruction 55616070210646123616 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_55616070210646123616()
    {
        AssertCode("@@@", "55616070210646123616");
    }

    // Reko: a decoder for the instruction 21152710605534014030 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21152710605534014030()
    {
        AssertCode("@@@", "21152710605534014030");
    }

    // Reko: a decoder for the instruction 56776107156362401655 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56776107156362401655()
    {
        AssertCode("@@@", "56776107156362401655");
    }

    // Reko: a decoder for the instruction 21761103546465625137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21761103546465625137()
    {
        AssertCode("@@@", "21761103546465625137");
    }

    // Reko: a decoder for the instruction 75657140564411123556 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75657140564411123556()
    {
        AssertCode("@@@", "75657140564411123556");
    }

    // Reko: a decoder for the instruction 01405553273407531000 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_01405553273407531000()
    {
        AssertCode("@@@", "01405553273407531000");
    }

    // Reko: a decoder for the instruction 64425073544074404766 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64425073544074404766()
    {
        AssertCode("@@@", "64425073544074404766");
    }

    // Reko: a decoder for the instruction 52134301305611371712 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52134301305611371712()
    {
        AssertCode("@@@", "52134301305611371712");
    }

    // Reko: a decoder for the instruction 16071656567732556057 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16071656567732556057()
    {
        AssertCode("@@@", "16071656567732556057");
    }

    // Reko: a decoder for the instruction 75515534714712647034 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75515534714712647034()
    {
        AssertCode("@@@", "75515534714712647034");
    }

    // Reko: a decoder for the instruction 41142361523520216233 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_41142361523520216233()
    {
        AssertCode("@@@", "41142361523520216233");
    }

    // Reko: a decoder for the instruction 47547165565672010072 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47547165565672010072()
    {
        AssertCode("@@@", "47547165565672010072");
    }

    // Reko: a decoder for the instruction 03170043126047643342 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_03170043126047643342()
    {
        AssertCode("@@@", "03170043126047643342");
    }

    // Reko: a decoder for the instruction 11216266026441640112 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11216266026441640112()
    {
        AssertCode("@@@", "11216266026441640112");
    }

    // Reko: a decoder for the instruction 62105741076266301306 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62105741076266301306()
    {
        AssertCode("@@@", "62105741076266301306");
    }

    // Reko: a decoder for the instruction 13235627752773442126 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13235627752773442126()
    {
        AssertCode("@@@", "13235627752773442126");
    }

    // Reko: a decoder for the instruction 37514021635141257035 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37514021635141257035()
    {
        AssertCode("@@@", "37514021635141257035");
    }

    // Reko: a decoder for the instruction 67705624613350021432 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67705624613350021432()
    {
        AssertCode("@@@", "67705624613350021432");
    }

    // Reko: a decoder for the instruction 33414112636075427631 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33414112636075427631()
    {
        AssertCode("@@@", "33414112636075427631");
    }

    // Reko: a decoder for the instruction 50067024737213034617 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50067024737213034617()
    {
        AssertCode("@@@", "50067024737213034617");
    }

    // Reko: a decoder for the instruction 51206372654410030607 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_51206372654410030607()
    {
        AssertCode("@@@", "51206372654410030607");
    }

    // Reko: a decoder for the instruction 10113530156106612604 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10113530156106612604()
    {
        AssertCode("@@@", "10113530156106612604");
    }

    // Reko: a decoder for the instruction 53772562271503606165 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53772562271503606165()
    {
        AssertCode("@@@", "53772562271503606165");
    }

    // Reko: a decoder for the instruction 40625775530736261077 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40625775530736261077()
    {
        AssertCode("@@@", "40625775530736261077");
    }

    // Reko: a decoder for the instruction 26466053115314762153 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26466053115314762153()
    {
        AssertCode("@@@", "26466053115314762153");
    }

    // Reko: a decoder for the instruction 01035461610074532065 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 33063762276624700262 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33063762276624700262()
    {
        AssertCode("@@@", "33063762276624700262");
    }

    // Reko: a decoder for the instruction 54276627577754026337 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54276627577754026337()
    {
        AssertCode("@@@", "54276627577754026337");
    }

    // Reko: a decoder for the instruction 63440603714016360013 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63440603714016360013()
    {
        AssertCode("@@@", "63440603714016360013");
    }

    // Reko: a decoder for the instruction 53536707405016512343 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53536707405016512343()
    {
        AssertCode("@@@", "53536707405016512343");
    }

    // Reko: a decoder for the instruction 70733114152457141326 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70733114152457141326()
    {
        AssertCode("@@@", "70733114152457141326");
    }

    // Reko: a decoder for the instruction 54650034166052331011 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54650034166052331011()
    {
        AssertCode("@@@", "54650034166052331011");
    }

    // Reko: a decoder for the instruction 43500771552033400100 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_43500771552033400100()
    {
        AssertCode("@@@", "43500771552033400100");
    }

    // Reko: a decoder for the instruction 26305374677015517721 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26305374677015517721()
    {
        AssertCode("@@@", "26305374677015517721");
    }

    // Reko: a decoder for the instruction 20421057736733372432 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20421057736733372432()
    {
        AssertCode("@@@", "20421057736733372432");
    }

    // Reko: a decoder for the instruction 46370260765372766663 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46370260765372766663()
    {
        AssertCode("@@@", "46370260765372766663");
    }

    // Reko: a decoder for the instruction 44127615733065412176 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 12201742706721323564 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_12201742706721323564()
    {
        AssertCode("@@@", "12201742706721323564");
    }

    // Reko: a decoder for the instruction 14262100045160150032 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14262100045160150032()
    {
        AssertCode("@@@", "14262100045160150032");
    }

    // Reko: a decoder for the instruction 21210204653611747073 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21210204653611747073()
    {
        AssertCode("@@@", "21210204653611747073");
    }

    // Reko: a decoder for the instruction 53322321674437244527 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53322321674437244527()
    {
        AssertCode("@@@", "53322321674437244527");
    }

    // Reko: a decoder for the instruction 05671362373214002335 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05671362373214002335()
    {
        AssertCode("@@@", "05671362373214002335");
    }

    // Reko: a decoder for the instruction 73535050623033476724 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73535050623033476724()
    {
        AssertCode("@@@", "73535050623033476724");
    }

    // Reko: a decoder for the instruction 40170222637514516160 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_40170222637514516160()
    {
        AssertCode("@@@", "40170222637514516160");
    }

    // Reko: a decoder for the instruction 25777163407005353346 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25777163407005353346()
    {
        AssertCode("@@@", "25777163407005353346");
    }

    // Reko: a decoder for the instruction 37204010676137045562 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37204010676137045562()
    {
        AssertCode("@@@", "37204010676137045562");
    }

    // Reko: a decoder for the instruction 13532067622613336556 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13532067622613336556()
    {
        AssertCode("@@@", "13532067622613336556");
    }

    // Reko: a decoder for the instruction 46247422003127150504 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46247422003127150504()
    {
        AssertCode("@@@", "46247422003127150504");
    }

    // Reko: a decoder for the instruction 45216534333041074067 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45216534333041074067()
    {
        AssertCode("@@@", "45216534333041074067");
    }

    // Reko: a decoder for the instruction 60503243417366565311 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60503243417366565311()
    {
        AssertCode("@@@", "60503243417366565311");
    }

    // Reko: a decoder for the instruction 23371030620125403657 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23371030620125403657()
    {
        AssertCode("@@@", "23371030620125403657");
    }

    // Reko: a decoder for the instruction 54011745263653222703 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54011745263653222703()
    {
        AssertCode("@@@", "54011745263653222703");
    }

    // Reko: a decoder for the instruction 65616567601717527630 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65616567601717527630()
    {
        AssertCode("@@@", "65616567601717527630");
    }

    // Reko: a decoder for the instruction 40611565576746125276 at address 01000 has not been implemented.
    
    // Reko: a decoder for the instruction 54355615700345526330 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54355615700345526330()
    {
        AssertCode("@@@", "54355615700345526330");
    }

    // Reko: a decoder for the instruction 75377477204346037375 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75377477204346037375()
    {
        AssertCode("@@@", "75377477204346037375");
    }

    // Reko: a decoder for the instruction 34634022045162642027 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34634022045162642027()
    {
        AssertCode("@@@", "34634022045162642027");
    }

    // Reko: a decoder for the instruction 64143750206337550560 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64143750206337550560()
    {
        AssertCode("@@@", "64143750206337550560");
    }

    // Reko: a decoder for the instruction 77345401700450054562 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77345401700450054562()
    {
        AssertCode("@@@", "77345401700450054562");
    }

    // Reko: a decoder for the instruction 30200656145137050525 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30200656145137050525()
    {
        AssertCode("@@@", "30200656145137050525");
    }

    // Reko: a decoder for the instruction 25016316230254404525 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25016316230254404525()
    {
        AssertCode("@@@", "25016316230254404525");
    }

    // Reko: a decoder for the instruction 50075762246003130316 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50075762246003130316()
    {
        AssertCode("@@@", "50075762246003130316");
    }

    // Reko: a decoder for the instruction 70514442611054145771 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70514442611054145771()
    {
        AssertCode("@@@", "70514442611054145771");
    }

    // Reko: a decoder for the instruction 04361166336644517651 at address 01000 has not been implemented.
    

    // Reko: a decoder for the instruction 10117324225402513653 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10117324225402513653()
    {
        AssertCode("@@@", "10117324225402513653");
    }

    // Reko: a decoder for the instruction 20412630443274527203 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20412630443274527203()
    {
        AssertCode("@@@", "20412630443274527203");
    }

    // Reko: a decoder for the instruction 64532460711653716716 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64532460711653716716()
    {
        AssertCode("@@@", "64532460711653716716");
    }

    // Reko: a decoder for the instruction 05650676465457476170 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_05650676465457476170()
    {
        AssertCode("@@@", "05650676465457476170");
    }

    // Reko: a decoder for the instruction 02046734661752236471 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02046734661752236471()
    {
        AssertCode("@@@", "02046734661752236471");
    }

    // Reko: a decoder for the instruction 15715063142422372250 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15715063142422372250()
    {
        AssertCode("@@@", "15715063142422372250");
    }

    // Reko: a decoder for the instruction 46162611362056544442 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_46162611362056544442()
    {
        AssertCode("@@@", "46162611362056544442");
    }

    // Reko: a decoder for the instruction 35632232413222354156 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35632232413222354156()
    {
        AssertCode("@@@", "35632232413222354156");
    }

    // Reko: a decoder for the instruction 56151447740230135733 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56151447740230135733()
    {
        AssertCode("@@@", "56151447740230135733");
    }

    // Reko: a decoder for the instruction 61755235630070042576 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61755235630070042576()
    {
        AssertCode("@@@", "61755235630070042576");
    }

    // Reko: a decoder for the instruction 56431542443326046627 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56431542443326046627()
    {
        AssertCode("@@@", "56431542443326046627");
    }

    // Reko: a decoder for the instruction 17703520716112302530 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_17703520716112302530()
    {
        AssertCode("@@@", "17703520716112302530");
    }

    // Reko: a decoder for the instruction 45164666527360540245 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45164666527360540245()
    {
        AssertCode("@@@", "45164666527360540245");
    }

    // Reko: a decoder for the instruction 65155262634153567325 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_65155262634153567325()
    {
        AssertCode("@@@", "65155262634153567325");
    }

    // Reko: a decoder for the instruction 76613116146707434462 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76613116146707434462()
    {
        AssertCode("@@@", "76613116146707434462");
    }

    // Reko: a decoder for the instruction 25720015634517171452 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25720015634517171452()
    {
        AssertCode("@@@", "25720015634517171452");
    }

    // Reko: a decoder for the instruction 76232033272414460267 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76232033272414460267()
    {
        AssertCode("@@@", "76232033272414460267");
    }

    // Reko: a decoder for the instruction 63662737264172525775 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63662737264172525775()
    {
        AssertCode("@@@", "63662737264172525775");
    }

    // Reko: a decoder for the instruction 47050602771724361762 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47050602771724361762()
    {
        AssertCode("@@@", "47050602771724361762");
    }

    // Reko: a decoder for the instruction 02414307625201522646 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02414307625201522646()
    {
        AssertCode("@@@", "02414307625201522646");
    }

    // Reko: a decoder for the instruction 14306675302376422411 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14306675302376422411()
    {
        AssertCode("@@@", "14306675302376422411");
    }

    // Reko: a decoder for the instruction 60110430567020025550 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_60110430567020025550()
    {
        AssertCode("@@@", "60110430567020025550");
    }

    // Reko: a decoder for the instruction 57617176677027272172 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_57617176677027272172()
    {
        AssertCode("@@@", "57617176677027272172");
    }

    // Reko: a decoder for the instruction 62054034640061621432 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62054034640061621432()
    {
        AssertCode("@@@", "62054034640061621432");
    }

    // Reko: a decoder for the instruction 32527257322307505301 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32527257322307505301()
    {
        AssertCode("@@@", "32527257322307505301");
    }

    // Reko: a decoder for the instruction 61221456371704610053 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61221456371704610053()
    {
        AssertCode("@@@", "61221456371704610053");
    }

    // Reko: a decoder for the instruction 50416417105401370173 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_50416417105401370173()
    {
        AssertCode("@@@", "50416417105401370173");
    }

    // Reko: a decoder for the instruction 11566267446160302314 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_11566267446160302314()
    {
        AssertCode("@@@", "11566267446160302314");
    }

    // Reko: a decoder for the instruction 14674466414642635231 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14674466414642635231()
    {
        AssertCode("@@@", "14674466414642635231");
    }

    // Reko: a decoder for the instruction 67520736413142725465 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67520736413142725465()
    {
        AssertCode("@@@", "67520736413142725465");
    }

    // Reko: a decoder for the instruction 73410760327755506326 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73410760327755506326()
    {
        AssertCode("@@@", "73410760327755506326");
    }

    // Reko: a decoder for the instruction 25546723116307425504 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25546723116307425504()
    {
        AssertCode("@@@", "25546723116307425504");
    }

    // Reko: a decoder for the instruction 25677370156236506464 at address 01000 has not been implemented.
    

    // Reko: a decoder for the instruction 73566375765413165022 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73566375765413165022()
    {
        AssertCode("@@@", "73566375765413165022");
    }

    // Reko: a decoder for the instruction 72151454754730461007 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72151454754730461007()
    {
        AssertCode("@@@", "72151454754730461007");
    }

    // Reko: a decoder for the instruction 77607260411677141560 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77607260411677141560()
    {
        AssertCode("@@@", "77607260411677141560");
    }

    // Reko: a decoder for the instruction 22000173236151311333 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_22000173236151311333()
    {
        AssertCode("@@@", "22000173236151311333");
    }

    // Reko: a decoder for the instruction 42476602651256656272 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42476602651256656272()
    {
        AssertCode("@@@", "42476602651256656272");
    }

    // Reko: a decoder for the instruction 27161142401725317755 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27161142401725317755()
    {
        AssertCode("@@@", "27161142401725317755");
    }

    // Reko: a decoder for the instruction 10650723606361025203 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10650723606361025203()
    {
        AssertCode("@@@", "10650723606361025203");
    }

    // Reko: a decoder for the instruction 01317307112115432137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_01317307112115432137()
    {
        AssertCode("@@@", "01317307112115432137");
    }

    // Reko: a decoder for the instruction 61440502262103700762 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61440502262103700762()
    {
        AssertCode("@@@", "61440502262103700762");
    }

    // Reko: a decoder for the instruction 61020001373720664432 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_61020001373720664432()
    {
        AssertCode("@@@", "61020001373720664432");
    }

    // Reko: a decoder for the instruction 31056342027556047604 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_31056342027556047604()
    {
        AssertCode("@@@", "31056342027556047604");
    }

    // Reko: a decoder for the instruction 04477744063211333221 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04477744063211333221()
    {
        AssertCode("@@@", "04477744063211333221");
    }

    // Reko: a decoder for the instruction 52345174426100546301 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52345174426100546301()
    {
        AssertCode("@@@", "52345174426100546301");
    }

    // Reko: a decoder for the instruction 30013671671230174145 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30013671671230174145()
    {
        AssertCode("@@@", "30013671671230174145");
    }

    // Reko: a decoder for the instruction 14663630302645150652 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14663630302645150652()
    {
        AssertCode("@@@", "14663630302645150652");
    }

    // Reko: a decoder for the instruction 66272354600677455206 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66272354600677455206()
    {
        AssertCode("@@@", "66272354600677455206");
    }

    // Reko: a decoder for the instruction 02461642132563307117 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_02461642132563307117()
    {
        AssertCode("@@@", "02461642132563307117");
    }

    // Reko: a decoder for the instruction 21637331425426763656 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21637331425426763656()
    {
        AssertCode("@@@", "21637331425426763656");
    }

    // Reko: a decoder for the instruction 33255332655465150263 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33255332655465150263()
    {
        AssertCode("@@@", "33255332655465150263");
    }

    // Reko: a decoder for the instruction 52353113400536722470 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_52353113400536722470()
    {
        AssertCode("@@@", "52353113400536722470");
    }

    // Reko: a decoder for the instruction 74067745322535105111 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_74067745322535105111()
    {
        AssertCode("@@@", "74067745322535105111");
    }

    // Reko: a decoder for the instruction 45031665032555165673 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 72730126461211707216 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72730126461211707216()
    {
        AssertCode("@@@", "72730126461211707216");
    }

    // Reko: a decoder for the instruction 27657113231272357770 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27657113231272357770()
    {
        AssertCode("@@@", "27657113231272357770");
    }

    // Reko: a decoder for the instruction 47464414620747546137 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47464414620747546137()
    {
        AssertCode("@@@", "47464414620747546137");
    }

    // Reko: a decoder for the instruction 73035205425535606350 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73035205425535606350()
    {
        AssertCode("@@@", "73035205425535606350");
    }

    // Reko: a decoder for the instruction 26305104473263644333 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_26305104473263644333()
    {
        AssertCode("@@@", "26305104473263644333");
    }

    // Reko: a decoder for the instruction 75204527326253731460 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_75204527326253731460()
    {
        AssertCode("@@@", "75204527326253731460");
    }

    // Reko: a decoder for the instruction 42010561476556563360 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42010561476556563360()
    {
        AssertCode("@@@", "42010561476556563360");
    }

    // Reko: a decoder for the instruction 45026503456741072170 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45026503456741072170()
    {
        AssertCode("@@@", "45026503456741072170");
    }

    // Reko: a decoder for the instruction 04125634474215512223 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04125634474215512223()
    {
        AssertCode("@@@", "04125634474215512223");
    }

    // Reko: a decoder for the instruction 21632261560701777720 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_21632261560701777720()
    {
        AssertCode("@@@", "21632261560701777720");
    }

    // Reko: a decoder for the instruction 15240377457355337240 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15240377457355337240()
    {
        AssertCode("@@@", "15240377457355337240");
    }

    // Reko: a decoder for the instruction 30476333137604476317 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30476333137604476317()
    {
        AssertCode("@@@", "30476333137604476317");
    }

    // Reko: a decoder for the instruction 33405216562554733421 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33405216562554733421()
    {
        AssertCode("@@@", "33405216562554733421");
    }

    // Reko: a decoder for the instruction 63022364332360526026 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63022364332360526026()
    {
        AssertCode("@@@", "63022364332360526026");
    }

    // Reko: a decoder for the instruction 25137567036250521525 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25137567036250521525()
    {
        AssertCode("@@@", "25137567036250521525");
    }

    // Reko: a decoder for the instruction 33425261342710402201 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_33425261342710402201()
    {
        AssertCode("@@@", "33425261342710402201");
    }

    // Reko: a decoder for the instruction 67761223270611240461 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67761223270611240461()
    {
        AssertCode("@@@", "67761223270611240461");
    }

    // Reko: a decoder for the instruction 15560150046254766630 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15560150046254766630()
    {
        AssertCode("@@@", "15560150046254766630");
    }

    // Reko: a decoder for the instruction 14011214320541030211 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 72564315133111257014 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_72564315133111257014()
    {
        AssertCode("@@@", "72564315133111257014");
    }

    // Reko: a decoder for the instruction 54200133144363235312 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54200133144363235312()
    {
        AssertCode("@@@", "54200133144363235312");
    }

    // Reko: a decoder for the instruction 27630605162214256321 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_27630605162214256321()
    {
        AssertCode("@@@", "27630605162214256321");
    }

    // Reko: a decoder for the instruction 45556363140314555747 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45556363140314555747()
    {
        AssertCode("@@@", "45556363140314555747");
    }

    // Reko: a decoder for the instruction 42616357740743756064 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_42616357740743756064()
    {
        AssertCode("@@@", "42616357740743756064");
    }

    // Reko: a decoder for the instruction 64620740726640415772 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_64620740726640415772()
    {
        AssertCode("@@@", "64620740726640415772");
    }

    // Reko: a decoder for the instruction 62102207211063567662 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_62102207211063567662()
    {
        AssertCode("@@@", "62102207211063567662");
    }

    // Reko: a decoder for the instruction 54545322200453176670 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54545322200453176670()
    {
        AssertCode("@@@", "54545322200453176670");
    }

    // Reko: a decoder for the instruction 71340212764752505004 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71340212764752505004()
    {
        AssertCode("@@@", "71340212764752505004");
    }

    // Reko: a decoder for the instruction 36044407210164316511 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_36044407210164316511()
    {
        AssertCode("@@@", "36044407210164316511");
    }

    // Reko: a decoder for the instruction 73074271142121774603 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_73074271142121774603()
    {
        AssertCode("@@@", "73074271142121774603");
    }

    // Reko: a decoder for the instruction 06136271310130144650 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_06136271310130144650()
    {
        AssertCode("@@@", "06136271310130144650");
    }

    // Reko: a decoder for the instruction 44727725612272406210 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_44727725612272406210()
    {
        AssertCode("@@@", "44727725612272406210");
    }

    // Reko: a decoder for the instruction 07260305064077066221 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07260305064077066221()
    {
        AssertCode("@@@", "07260305064077066221");
    }

    // Reko: a decoder for the instruction 71012074504634753066 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_71012074504634753066()
    {
        AssertCode("@@@", "71012074504634753066");
    }

    // Reko: a decoder for the instruction 15537325447701656467 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_15537325447701656467()
    {
        AssertCode("@@@", "15537325447701656467");
    }

    // Reko: a decoder for the instruction 34476107214754251426 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_34476107214754251426()
    {
        AssertCode("@@@", "34476107214754251426");
    }

    // Reko: a decoder for the instruction 24436325006564307370 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24436325006564307370()
    {
        AssertCode("@@@", "24436325006564307370");
    }

    // Reko: a decoder for the instruction 53552341402474317067 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53552341402474317067()
    {
        AssertCode("@@@", "53552341402474317067");
    }

    // Reko: a decoder for the instruction 00676012606144160167 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_00676012606144160167()
    {
        AssertCode("@@@", "00676012606144160167");
    }

    // Reko: a decoder for the instruction 67650176774203626376 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67650176774203626376()
    {
        AssertCode("@@@", "67650176774203626376");
    }

    // Reko: a decoder for the instruction 47770653423076715642 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47770653423076715642()
    {
        AssertCode("@@@", "47770653423076715642");
    }

    // Reko: a decoder for the instruction 32143074224301524620 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_32143074224301524620()
    {
        AssertCode("@@@", "32143074224301524620");
    }

    // Reko: a decoder for the instruction 54461740173430344601 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54461740173430344601()
    {
        AssertCode("@@@", "54461740173430344601");
    }

    // Reko: a decoder for the instruction 10523712031036176707 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_10523712031036176707()
    {
        AssertCode("@@@", "10523712031036176707");
    }

    // Reko: a decoder for the instruction 26357144474025526220 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 25464214546312635333 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_25464214546312635333()
    {
        AssertCode("@@@", "25464214546312635333");
    }

    // Reko: a decoder for the instruction 77755001121447536010 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_77755001121447536010()
    {
        AssertCode("@@@", "77755001121447536010");
    }

    // Reko: a decoder for the instruction 20555731524251013463 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_20555731524251013463()
    {
        AssertCode("@@@", "20555731524251013463");
    }

    // Reko: a decoder for the instruction 37426074533424464357 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_37426074533424464357()
    {
        AssertCode("@@@", "37426074533424464357");
    }

    // Reko: a decoder for the instruction 54020544540404323640 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_54020544540404323640()
    {
        AssertCode("@@@", "54020544540404323640");
    }

    // Reko: a decoder for the instruction 30337264774454440317 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_30337264774454440317()
    {
        AssertCode("@@@", "30337264774454440317");
    }

    // Reko: a decoder for the instruction 04306540724507174101 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_04306540724507174101()
    {
        AssertCode("@@@", "04306540724507174101");
    }

    // Reko: a decoder for the instruction 45116002635150210211 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_45116002635150210211()
    {
        AssertCode("@@@", "45116002635150210211");
    }

    // Reko: a decoder for the instruction 34105476114526633741 at address 01000 has not been implemented.
    // Reko: a decoder for the instruction 53666542211417600142 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_53666542211417600142()
    {
        AssertCode("@@@", "53666542211417600142");
    }

    // Reko: a decoder for the instruction 63432353615154137123 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_63432353615154137123()
    {
        AssertCode("@@@", "63432353615154137123");
    }

    // Reko: a decoder for the instruction 70177026454706251731 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70177026454706251731()
    {
        AssertCode("@@@", "70177026454706251731");
    }

    // Reko: a decoder for the instruction 67252701607521532115 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67252701607521532115()
    {
        AssertCode("@@@", "67252701607521532115");
    }

    // Reko: a decoder for the instruction 07373136043363676730 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07373136043363676730()
    {
        AssertCode("@@@", "07373136043363676730");
    }

    // Reko: a decoder for the instruction 76343321005025272130 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_76343321005025272130()
    {
        AssertCode("@@@", "76343321005025272130");
    }

    // Reko: a decoder for the instruction 56154100417025725146 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_56154100417025725146()
    {
        AssertCode("@@@", "56154100417025725146");
    }

    // Reko: a decoder for the instruction 24161632747604004410 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_24161632747604004410()
    {
        AssertCode("@@@", "24161632747604004410");
    }

    // Reko: a decoder for the instruction 16635546642367601644 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_16635546642367601644()
    {
        AssertCode("@@@", "16635546642367601644");
    }

    // Reko: a decoder for the instruction 47444450522440220502 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_47444450522440220502()
    {
        AssertCode("@@@", "47444450522440220502");
    }

    // Reko: a decoder for the instruction 14023416041151603462 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14023416041151603462()
    {
        AssertCode("@@@", "14023416041151603462");
    }

    // Reko: a decoder for the instruction 35453224266435436474 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35453224266435436474()
    {
        AssertCode("@@@", "35453224266435436474");
    }

    // Reko: a decoder for the instruction 03471206524675446670 at address 01000 has not been implemented.

    // Reko: a decoder for the instruction 70316541236155537737 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_70316541236155537737()
    {
        AssertCode("@@@", "70316541236155537737");
    }

    // Reko: a decoder for the instruction 14723456144034636213 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_14723456144034636213()
    {
        AssertCode("@@@", "14723456144034636213");
    }

    // Reko: a decoder for the instruction 13304465723214410644 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_13304465723214410644()
    {
        AssertCode("@@@", "13304465723214410644");
    }

    // Reko: a decoder for the instruction 07140456341403747134 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_07140456341403747134()
    {
        AssertCode("@@@", "07140456341403747134");
    }

    // Reko: a decoder for the instruction 66366216153661465525 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_66366216153661465525()
    {
        AssertCode("@@@", "66366216153661465525");
    }

    // Reko: a decoder for the instruction 35207073672626557313 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_35207073672626557313()
    {
        AssertCode("@@@", "35207073672626557313");
    }

    // Reko: a decoder for the instruction 23652706725357275046 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_23652706725357275046()
    {
        AssertCode("@@@", "23652706725357275046");
    }

    // Reko: a decoder for the instruction 67551340624557445744 at address 01000 has not been implemented.
    [Test]
    public void CDC6600Dis_67551340624557445744()
    {
        AssertCode("@@@", "67551340624557445744");
    }
#endif
}
