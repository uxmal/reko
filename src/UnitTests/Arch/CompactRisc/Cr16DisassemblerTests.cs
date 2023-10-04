#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.CompactRisc;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reko.UnitTests.Arch.CompactRisc
{
    [TestFixture]
    public class Cr16DisassemblerTests : DisassemblerTestBase<Cr16Instruction>
    {
        private readonly Cr16Architecture arch;
        private readonly Address addrLoad;

        public Cr16DisassemblerTests()
        {
            this.arch = new Cr16Architecture(CreateServiceContainer(), "cr16c", new Dictionary<string, object>());
            this.addrLoad = Address.Ptr32(0x8000);
        }

        public override IProcessorArchitecture Architecture => arch;
        public override Address LoadAddress => addrLoad;

        // [Test]
        public void Cr16Dasm_Gen()
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x8000), new byte[1024]);
            var rnd = new Random(0x4711);
            rnd.NextBytes(mem.Bytes);
            var rdr = mem.CreateBeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            dasm.Take(100).ToArray();
        }

        private void AssertCode(string sExpected, string hexBytes)
        {
            var instr = base.DisassembleHexBytes(hexBytes);
            if (!instr.ToString().Contains("nvalid"))
                return;
            Assert.AreEqual(sExpected, instr.ToString());
        }

        [Test]
        public void Cr16Dasm_addb_imm()
        {
            AssertCode("addb\t$1,r1", "1030");
        }

        [Test]
        public void Cr16Dasm_addd_imm32()
        {
            AssertCode("addd\t$56781234,(r5,r4)", "240078563412");
        }

        [Test]
        public void Cr16Dasm_addd_imm4()
        {
            AssertCode("addd\t$2,sp", "2F60");
        }

        [Test]
        public void Cr16Dasm_addub()
        {
            AssertCode("addub\t$3,r3", "362C");
        }

        [Test]
        public void Cr16Dasm_addw_imm4_16()
        {
            AssertCode("addw\t$1,r1", "1232");
        }

        [Test]
        public void Cr16Dasm_andb_imm4()
        {
            AssertCode("andb\t$34,r11", "B020 3412");
        }

        [Test]
        public void Cr16Dasm_andd_imm32()
        {
            AssertCode("andd\t$56781234,(r1,r0)", "400078563412");
        }

        [Test]
        public void Cr16Dasm_andw_imm16()
        {
            AssertCode("andw\t$2345,r11", "B322 4523");
        }

        [Test]
        public void Cr16Dasm_ashub_left()
        {
            AssertCode("ashub\t$0,r0", "0040");
        }

        [Test]
        public void Cr16Dasm_ashud_right()
        {
            AssertCode("ashud\t$-30,(r7,r6)", "E64F");
        }

        [Test]
        public void Cr16Dasm_ashuw_left()
        {
            AssertCode("ashuw\t$8,r1", "8142");
        }

        [Test]
        public void Cr16Dasm_ashuw_reg_reg()
        {
            AssertCode("ashuw\tr0,r1", "0145");
        }

        [Test]
        public void Cr16Dasm_bal_disp24()
        {
            AssertCode("bal\tra,00248100", "24C0 0001");
        }

        [Test]
        public void Cr16Dasm_bne()
        {
            AssertCode("bne\t00008056", "1B12");
        }

        [Test]
        public void Cr16Dasm_bne0b()
        {
            AssertCode("bne0b\tr10,00008002", "0A0D");
        }

        [Test]
        public void Cr16Dasm_br()
        {
            AssertCode("beq\t0000800E", "0710");
        }

        [Test]
        public void Cr16Dasm_br_disp24()
        {
            AssertCode("br\t0000830C", "1000E0000C03");
        }

        [Test]
        public void Cr16Dasm_br_disp24_2()
        {
            AssertCode("br\t0000867C", "1000E0007C06");
        }

        [Test]
        public void Cr16Dasm_bra_cond_disp8()
        {
            AssertCode("br\t0000C68A", "E018 4523");
        }

        [Test]
        public void Cr16Dasm_cbitb_abs20()
        {
            AssertCode("cbitb\tr8,(0x042345)", "84684523");
        }

        [Test]
        public void Cr16Dasm_cbitb_rp_disp0()
        {
            AssertCode("cbitb\t$0,(r1,r0)", "006A");
        }

        [Test]
        public void Cr16Dasm_cbitb_rp_disp20()
        {
            AssertCode("@@@", "1000195A1C9F");
        }

        [Test]
        public void Cr16Dasm_cbitw_abs20()
        {
            AssertCode("cbitw\t$3,(0x032345)", "436F 4523");
        }

        [Test]
        public void Cr16Dasm_cbitw_abs20_rel()
        {
            AssertCode("cbitw\t$C,0xC2345(r6)", "6C6C 4523");
        }

        [Test]
        public void Cr16Dasm_cbitw_rp()
        {
            AssertCode("cbitw\t$9,(r10,r9)", "496E 4523");
        }

        [Test]
        public void Cr16Dasm_cbitw_disp16()
        {
            AssertCode("cbitw\t$2,0x2345(r3,r2)", "7269 4523");
        }

        [Test]
        public void Cr16Dasm_cmpb_imm4()
        {
            AssertCode("cmpb\t$0,r0", "0050");
        }

        [Test]
        public void Cr16Dasm_cmpb_reg_reg()
        {
            AssertCode("cmpb\tr2,r0", "0251");
        }

        [Test]
        public void Cr16Dasm_cmpd_imm4()
        {
            AssertCode("cmpd\t$0,(r1,r0)", "0056");
        }

        [Test]
        public void Cr16Dasm_cmpd_rp_rp()
        {
            AssertCode("cmpd\t(r1,r0),(r3,r2)", "2057");
        }

        [Test]
        public void Cr16Dasm_cmpw_imm4()
        {
            AssertCode("cmpw\t$0,r0", "0052");
        }

        [Test]
        public void Cr16Dasm_cmpw_imm16()
        {
            AssertCode("cmpw\t$2345,r0", "B052 4523");
        }

        [Test]
        public void Cr16Dasm_cmpw_reg_reg()
        {
            AssertCode("cmpw\tr0,r2", "2053");
        }

        [Test]
        public void Cr16Dasm_excp()
        {
            AssertCode("excp\tDVZ", "C600");
        }

        [Test]
        public void Cr16Dasm_jal_rp()
        {
            AssertCode("jal\tra,(r1,r0)", "D000");
        }

        [Test]
        public void Cr16Dasm_jr()
        {
            AssertCode("jr\t(r1,r0)", "E00A");
        }

        [Test]
        public void Cr16Dasm_loadb_abs20()
        {
            AssertCode("loadb\t(0x012345),r0", "0188 4523");
        }

        [Test]
        public void Cr16Dasm_loadb_abs24()
        {
            AssertCode("loadb\t(0x0108A8),r0", "12000071A808");
        }

        [Test]
        public void Cr16Dasm_loadb_rp()
        {
            AssertCode("loadb\t(r1,r0),r0", "00B0");
        }

        [Test]
        public void Cr16Dasm_loadd_abs20()
        {
            AssertCode("loadd\t(0x012345),r6", "61874523");
        }

        [Test]
        public void Cr16Dasm_loadw_abs20()
        {
            AssertCode("loadw\t(0x0F2345),r1", "1F89 4523");
        }

        [Test]
        public void Cr16Dasm_loadw_abs24()
        {
            AssertCode("loadw\t(0x010680),r3", "120030F18006");
        }

        [Test]
        public void Cr16Dasm_lpr()
        {
            AssertCode("lpr\tr0,CFG", "14008000");
        }

        [Test]
        public void Cr16Dasm_lprd()
        {
            AssertCode("lprd\t(r1,r0),INTBASEL", "1400A010");
        }

        [Test]
        public void Cr16Dasm_lshb_neg()
        {
            AssertCode("lshb\t$3,r0", "B009");
        }

        [Test]
        public void Cr16Dasm_lshd_right()
        {
            AssertCode("@@@", "2E4B");
        }

        [Test]
        public void Cr16Dasm_nop()
        {
            AssertCode("@@@", "002C");
        }

        [Test]
        public void Cr16Dasm_lshw_right()
        {
            AssertCode("lshw\t$-1,r4", "1449");
        }

        [Test]
        public void Cr16Dasm_movb_imm()
        {
            AssertCode("movb\t$1,r2", "1258");
        }

        [Test]
        public void Cr16Dasm_movb_reg()
        {
            AssertCode("movb\tr1,r2", "1259");
        }

        [Test]
        public void Cr16Dasm_movd_imm4_16()
        {
            AssertCode("movd\t$0,(r5,r4)", "0454");
        }

        [Test]
        public void Cr16Dasm_movd_imm20()
        {
            AssertCode("movd\t$12345,(r3,r2)", "2105 4523");
        }

        [Test]
        public void Cr16Dasm_movd_imm32()
        {
            AssertCode("movd\t$56781234,(r1,r0)", "700078563412");
        }

        [Test]
        public void Cr16Dasm_movd_rp_rp()
        {
            AssertCode("movd\t(r7,r6),(r5,r4)", "4655");
        }

        [Test]
        public void Cr16Dasm_movw_imm4_reg()
        {
            AssertCode("movw\t$1,r2", "125A");
        }

        [Test]
        public void Cr16Dasm_movw_reg_reg()
        {
            AssertCode("movw\tr7,r4", "745B");
        }

        [Test]
        public void Cr16Dasm_movzw()
        {
            AssertCode("movzw\tr4,(r5,r4)", "445F");
        }

        [Test]
        public void Cr16Dasm_mulb_reg_reg()
        {
            AssertCode("mulb\tr7,r2", "7265");
        }

        [Test]
        public void Cr16Dasm_mulsb()
        {
            AssertCode("@@@", "040B");
        }

        [Test]
        public void Cr16Dasm_muluw()
        {
            AssertCode("muluw\tr0,ra", "0E63");
        }

        [Test]
        public void Cr16Dasm_ord()
        {
            AssertCode("@@@", "14008090");
        }

        [Test]
        public void Cr16Dasm_orw_imm4_16_reg()
        {
            AssertCode("orw\t$2345,r11", "B0264523");
        }

        [Test]
        public void Cr16Dasm_pop()
        {
            AssertCode("pop\t$0,r7", "0702");
        }

        [Test]
        public void Cr16Dasm_popret()
        {
            AssertCode("popret\t$1,ra", "1E03");    //$REVIEW:$1?
        }

        [Test]
        public void Cr16Dasm_push()
        {
            AssertCode("push\t$1,ra", "1E01");
        }

        [Test]
        public void Cr16Dasm_push_3_r8()
        {
            AssertCode("push\t$3,r8", "3801");
        }

        [Test]
        public void Cr16Dasm_sbitb()
        {
            AssertCode("sbitb\t$0,(r1,r0)", "2072 4523");
        }

        [Test]
        public void Cr16Dasm_sbitb_abs20_rel()
        {
            AssertCode("sbitb\t$2,0x22345(r7)", "7270 4523");
        }

        [Test]
        public void Cr16Dasm_sbitw_abs20()
        {
            AssertCode("sbitw\t$4,(0x042345)", "74774523");
        }

        [Test]
        public void Cr16Dasm_sbitw_abs20_rel()
        {
            AssertCode("sbitw\t$8,0x82345(r6)", "6874 4523");
        }

        [Test]
        public void Cr16Dasm_sbitw_rp_disp0()
        {
            AssertCode("sbitw\t$2,(r3,r2)", "7276");
        }

        [Test]
        public void Cr16Dasm_sbitw_rp_disp16()
        {
            AssertCode("sbitw\t$0,0x3456(r1,r0)", "2071 5634");
        }

        [Test]
        public void Cr16Dasm_sne()
        {
            AssertCode("sne\tr0", "1008");
        }

        [Test]
        public void Cr16Dasm_store_indexed()
        {
            AssertCode("storb\tr3,[r12](r7,r6)", "33FE");
        }

        [Test]
        public void Cr16Dasm_storb()
        {
            AssertCode("storb\tr2,(0x0108A0)", "13002071A008");
        }

        [Test]
        public void Cr16Dasm_storb_imm_abs20_rel()
        {
            AssertCode("storb\t$B,0x33456(r11)", "B384 5634");
        }

        [Test]
        public void Cr16Dasm_storb_imm_rp()
        {
            AssertCode("storb\t$8,(ra)", "8E82");
        }

        [Test]
        public void Cr16Dasm_storb_imm_rp_disp16()
        {
            AssertCode("storb\t$0,0x3456(r3,r2)", "0283 5634");
        }

        [Test]
        public void Cr16Dasm_storb_rp_disp4()
        {
            AssertCode("storb\tr12,6(r7,r6)", "C6F6");
        }

        [Test]
        public void Cr16Dasm_storb_rp_disp20()
        {
            AssertCode("storb\tr0,0x10371(r1,r0)", "130002517103");
        }

        [Test]
        public void Cr16Dasm_storb_sp()
        {
            AssertCode("storb\tr11,0x2345(sp)", "BFFF 4523");
        }

        [Test]
        public void Cr16Dasm_storb_12001E11C173()
        {
            AssertCode("storb\tr1,0x173C1(r2,r1)", "12001E11C173");
        }

        [Test]
        public void Cr16Dasm_storb_abs20()
        {
            AssertCode("storb\tr0,(0x0F2345)", "0FC8 4523");
        }

        [Test]
        public void Cr16Dasm_storb_abs20_rel()
        {
            AssertCode("storb\tr0,0x53456(r0)", "05CB 5634");
        }

        [Test]
        public void Cr16Dasm_storb_abs24()
        {
            AssertCode("storb\t$0,(0x010588)", "120000318805");
        }

        [Test]
        public void Cr16Dasm_storw_abs20()
        {
            AssertCode("storw\tr0,(0x0F2345)", "0FC9 4523");
        }

        [Test]
        public void Cr16Dasm_storw_abs20_relAC5()
        {
            AssertCode("storw\tr0,0xA3456(r0)", "0AC5 5634");
        }

        [Test]
        public void Cr16Dasm_storw_imm_abs20()
        {
            AssertCode("storw\t$0,(0x013456)", "01C1 5634");
        }

        [Test]
        public void Cr16Dasm_storw_imm_rp_disp0()
        {
            AssertCode("storw\t$0,(r1,r0)", "00C2");
        }

        [Test]
        public void Cr16Dasm_storw_rp()
        {
            AssertCode("storw\tr0,0x1A(r1,r0)", "00DD");
        }

        [Test]
        public void Cr16Dasm_storw_rp_disp20()
        {
            AssertCode("storw\tr0,0x10380(r1,r0)", "130002D18003");
        }

        [Test]
        public void Cr16Dasm_stord_abs20()
        {
            AssertCode("stord\t(r1,r0),(0x033456)", "03C7 5634");
        }

        [Test]
        public void Cr16Dasm_stord_abs20_rel()
        {
            AssertCode("stord\tr10,0x73456(r10)", "A7CD 5634");
        }

        [Test]
        public void Cr16Dasm_stord_regression1()
        {
            AssertCode("stord\tr7,0x59000(r7)", "75CD 0090");
        }

        [Test]
        public void Cr16Dasm_subw_reg_reg()
        {
            AssertCode("subw\tr1,r0", "013B");
        }

        [Test]
        public void Cr16Dasm_tbit()
        {
            AssertCode("tbit\t$3,r0", "0306");
        }

        [Test]
        public void Cr16Dasm_tbitb_rp()
        {
            AssertCode("tbitb\t$0,(r1,r0)", "107A");
        }

        [Test]
        public void Cr16Dasm_tbitw_abs20()
        {
            AssertCode("tbitw\t$8,(0x082345)", "487F 4523");
        }

        [Test]
        public void Cr16Dasm_tbitw_abs20_rel()
        {
            AssertCode("tbitw\t$E,0xE2345(r7)", "7E7C 4523");
        }

        [Test]
        public void Cr16Dasm_tbitw_disp16()
        {
            AssertCode("tbitw\t$3,0x2345(r4,r3)", "7379 4523");
        }

        // Reko: a decoder for the instruction 140002A0 at address 0000C262 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A0()
        {
            AssertCode("@@@", "140002A0");
        }

        [Test]
        public void Cr16Dasm_xorw()
        {
            AssertCode("xorw\t$FFFF,r9", "912A");
        }

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues

        // Reko: a decoder for the instruction 404B at address 00004C54 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_404B()
        {
            AssertCode("@@@", "404B");
        }
        // Reko: a decoder for the instruction 0D4B at address 0000BAF8 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_0D4B()
        {
            AssertCode("@@@", "0D4B");
        }
        

        // Reko: a decoder for the instruction 14008E90 at address 0000D52E has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14008E90()
        {
            AssertCode("@@@", "14008E90");
        }
        // Reko: a decoder for the instruction F84B at address 0000D532 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F84B()
        {
            AssertCode("@@@", "F84B");
        }
        // Reko: a decoder for the instruction 14008090 at address 0000C0B8 has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)

        // Reko: a decoder for the instruction D24B at address 0000B43A has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_D24B()
        {
            AssertCode("@@@", "D24B");
        }
        // Reko: a decoder for the instruction 084B at address 0000C1BE has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_084B()
        {
            AssertCode("@@@", "084B");
        }
        // Reko: a decoder for the instruction 004B at address 0000C68E has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_004B()
        {
            AssertCode("@@@", "004B");
        }
        // Reko: a decoder for the instruction 140080A0 at address 0000C676 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140080A0()
        {
            AssertCode("@@@", "140080A0");
        }
        // Reko: a decoder for the instruction C24B at address 0000BE22 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_C24B()
        {
            AssertCode("@@@", "C24B");
        }
        // Reko: a decoder for the instruction 864B at address 00005E48 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_864B()
        {
            AssertCode("@@@", "864B");
        }
        // Reko: a decoder for the instruction 1000195A1C9F at address 00004AB4 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp)  disp20 4 dest (rp)  3 pos imm 20 dest disp 4)

        // Reko: a decoder for the instruction 140025B0 at address 00002E22 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140025B0()
        {
            AssertCode("@@@", "140025B0");
        }
        // Reko: a decoder for the instruction 1400A5B0 at address 000033B4 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A5B0()
        {
            AssertCode("@@@", "1400A5B0");
        }
        // Reko: a decoder for the instruction 2E4B at address 00004C5A has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)

        // Reko: a decoder for the instruction 744A at address 00004C5E has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_744A()
        {
            AssertCode("@@@", "744A");
        }
        // Reko: a decoder for the instruction DE4B at address 0000B442 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_DE4B()
        {
            AssertCode("@@@", "DE4B");
        }
        // Reko: a decoder for the instruction F24B at address 0000B6BE has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F24B()
        {
            AssertCode("@@@", "F24B");
        }
        // Reko: a decoder for the instruction 140024B0 at address 0000B6CA has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140024B0()
        {
            AssertCode("@@@", "140024B0");
        }
        // Reko: a decoder for the instruction 140028B0 at address 0000BB02 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140028B0()
        {
            AssertCode("@@@", "140028B0");
        }
        // Reko: a decoder for the instruction 14002CB0 at address 0000C1A4 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14002CB0()
        {
            AssertCode("@@@", "14002CB0");
        }
        // Reko: a decoder for the instruction 1400A0B0 at address 0000C1B8 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A0B0()
        {
            AssertCode("@@@", "1400A0B0");
        }
        // Reko: a decoder for the instruction 0A4B at address 0000C1C4 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_0A4B()
        {
            AssertCode("@@@", "0A4B");
        }
        // Reko: a decoder for the instruction 14002090 at address 0000C210 has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14002090()
        {
            AssertCode("@@@", "14002090");
        }
        // Reko: a decoder for the instruction 140042B0 at address 0000C23E has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140042B0()
        {
            AssertCode("@@@", "140042B0");
        }
        // Reko: a decoder for the instruction 1400D2B0 at address 0000C250 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400D2B0()
        {
            AssertCode("@@@", "1400D2B0");
        }

        // Reko: a decoder for the instruction 804B at address 0000C272 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_804B()
        {
            AssertCode("@@@", "804B");
        }
        // Reko: a decoder for the instruction 140020A0 at address 0000C274 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A0()
        {
            AssertCode("@@@", "140020A0");
        }
        // Reko: a decoder for the instruction 024B at address 0000C2B2 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_024B()
        {
            AssertCode("@@@", "024B");
        }
        // Reko: a decoder for the instruction 140002B0 at address 0000C2C8 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002B0()
        {
            AssertCode("@@@", "140002B0");
        }
        // Reko: a decoder for the instruction 844A at address 0000C2D6 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_844A()
        {
            AssertCode("@@@", "844A");
        }
        // Reko: a decoder for the instruction 140024A0 at address 0000C2E0 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140024A0()
        {
            AssertCode("@@@", "140024A0");
        }
        // Reko: a decoder for the instruction 140042A0 at address 0000C2E6 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140042A0()
        {
            AssertCode("@@@", "140042A0");
        }
        // Reko: a decoder for the instruction 140026A0 at address 0000C300 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140026A0()
        {
            AssertCode("@@@", "140026A0");
        }
        // Reko: a decoder for the instruction 140060B0 at address 0000C31C has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140060B0()
        {
            AssertCode("@@@", "140060B0");
        }
        // Reko: a decoder for the instruction 824A at address 0000C32A has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_824A()
        {
            AssertCode("@@@", "824A");
        }
        // Reko: a decoder for the instruction 140008A0 at address 0000C354 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140008A0()
        {
            AssertCode("@@@", "140008A0");
        }
        // Reko: a decoder for the instruction 140080B0 at address 0000C370 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140080B0()
        {
            AssertCode("@@@", "140080B0");
        }
        // Reko: a decoder for the instruction 140040A0 at address 0000C388 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140040A0()
        {
            AssertCode("@@@", "140040A0");
        }
        // Reko: a decoder for the instruction 140004A0 at address 0000C38E has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140004A0()
        {
            AssertCode("@@@", "140004A0");
        }
        // Reko: a decoder for the instruction 884B at address 0000C392 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_884B()
        {
            AssertCode("@@@", "884B");
        }
        // Reko: a decoder for the instruction 140046A0 at address 0000C3A8 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140046A0()
        {
            AssertCode("@@@", "140046A0");
        }
        // Reko: a decoder for the instruction 140028A0 at address 0000C3FC has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140028A0()
        {
            AssertCode("@@@", "140028A0");
        }
        // Reko: a decoder for the instruction 140062A0 at address 0000C4FA has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140062A0()
        {
            AssertCode("@@@", "140062A0");
        }
        // Reko: a decoder for the instruction 140020B0 at address 0000C516 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020B0()
        {
            AssertCode("@@@", "140020B0");
        }
        // Reko: a decoder for the instruction 824B at address 0000C532 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_824B()
        {
            AssertCode("@@@", "824B");
        }
        // Reko: a decoder for the instruction 140060A0 at address 0000C548 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140060A0()
        {
            AssertCode("@@@", "140060A0");
        }
        // Reko: a decoder for the instruction 140004B0 at address 0000C58C has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140004B0()
        {
            AssertCode("@@@", "140004B0");
        }
        // Reko: a decoder for the instruction 140006A0 at address 0000C688 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140006A0()
        {
            AssertCode("@@@", "140006A0");
        }
        // Reko: a decoder for the instruction 804A at address 0000C6A2 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_804A()
        {
            AssertCode("@@@", "804A");
        }
        // Reko: a decoder for the instruction 140064B0 at address 0000C6AC has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140064B0()
        {
            AssertCode("@@@", "140064B0");
        }
        // Reko: a decoder for the instruction F04B at address 0000C8FA has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F04B()
        {
            AssertCode("@@@", "F04B");
        }
        // Reko: a decoder for the instruction 1400E0A0 at address 0000C9F8 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400E0A0()
        {
            AssertCode("@@@", "1400E0A0");
        }
        // Reko: a decoder for the instruction 1400E2B0 at address 0000CA6E has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400E2B0()
        {
            AssertCode("@@@", "1400E2B0");
        }
        // Reko: a decoder for the instruction FE4B at address 0000CA7C has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_FE4B()
        {
            AssertCode("@@@", "FE4B");
        }
        // Reko: a decoder for the instruction F44B at address 0000D534 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F44B()
        {
            AssertCode("@@@", "F44B");
        }



#if BORED
        // Reko: a decoder for the instruction 120004B02C00 at address 03B6 has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120004B02C00()
        {
            AssertCode("@@@", "120004B02C00");
        }
        // Reko: a decoder for the instruction 125A at address 03C2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_125A()
        {
            AssertCode("@@@", "125A");
        }
        // Reko: a decoder for the instruction 0296 at address 03CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0296()
        {
            AssertCode("@@@", "0296");
        }
        // Reko: a decoder for the instruction B052 at address 03D0 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B052()
        {
            AssertCode("@@@", "B052");
        }
        // Reko: a decoder for the instruction B05A at address 03E2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B05A()
        {
            AssertCode("@@@", "B05A");
        }
        // Reko: a decoder for the instruction FEFF at address 03E4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_FEFF()
        {
            AssertCode("@@@", "FEFF");
        }
        // Reko: a decoder for the instruction BF60 at address 03EC has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BF60()
        {
            AssertCode("@@@", "BF60");
        }
        // Reko: a decoder for the instruction FCFF at address 03EE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_FCFF()
        {
            AssertCode("@@@", "FCFF");
        }
        // Reko: a decoder for the instruction B254 at address 03FA has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B254()
        {
            AssertCode("@@@", "B254");
        }
        // Reko: a decoder for the instruction 00C3 at address 0408 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_00C3()
        {
            AssertCode("@@@", "00C3");
        }
        // Reko: a decoder for the instruction 1400B05A0020 at address 040A has not been implemented. (Fmt1 2 ZZ ope 5  res - no operation 4)
        [Test]
        public void Cr16Dasm_1400B05A0020()
        {
            AssertCode("@@@", "1400B05A0020");
        }
        // Reko: a decoder for the instruction 08DB at address 0410 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08DB()
        {
            AssertCode("@@@", "08DB");
        }
        // Reko: a decoder for the instruction 0054 at address 0412 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0054()
        {
            AssertCode("@@@", "0054");
        }
        // Reko: a decoder for the instruction 08EF at address 0414 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_08EF()
        {
            AssertCode("@@@", "08EF");
        }
        // Reko: a decoder for the instruction 3400 at address 0416 has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3400()
        {
            AssertCode("@@@", "3400");
        }
        // Reko: a decoder for the instruction 08C3 at address 0418 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_08C3()
        {
            AssertCode("@@@", "08C3");
        }
        // Reko: a decoder for the instruction 905A at address 041C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_905A()
        {
            AssertCode("@@@", "905A");
        }
        // Reko: a decoder for the instruction 08DF at address 041E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_08DF()
        {
            AssertCode("@@@", "08DF");
        }
        // Reko: a decoder for the instruction 2A00 at address 0420 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2A00()
        {
            AssertCode("@@@", "2A00");
        }
        // Reko: a decoder for the instruction 2C00 at address 0424 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2C00()
        {
            AssertCode("@@@", "2C00");
        }
        // Reko: a decoder for the instruction 2000 at address 0428 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2000()
        {
            AssertCode("@@@", "2000");
        }
        // Reko: a decoder for the instruction 0AB0 at address 042A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AB0()
        {
            AssertCode("@@@", "0AB0");
        }
        // Reko: a decoder for the instruction 0D5A at address 042C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0D5A()
        {
            AssertCode("@@@", "0D5A");
        }
        // Reko: a decoder for the instruction 165A at address 043E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_165A()
        {
            AssertCode("@@@", "165A");
        }
        // Reko: a decoder for the instruction BE5A at address 0440 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BE5A()
        {
            AssertCode("@@@", "BE5A");
        }
        // Reko: a decoder for the instruction B032 at address 0448 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B032()
        {
            AssertCode("@@@", "B032");
        }
        // Reko: a decoder for the instruction D0FF at address 044A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D0FF()
        {
            AssertCode("@@@", "D0FF");
        }
        // Reko: a decoder for the instruction 1A60 at address 0450 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1A60()
        {
            AssertCode("@@@", "1A60");
        }
        // Reko: a decoder for the instruction 2131 at address 045A has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2131()
        {
            AssertCode("@@@", "2131");
        }
        // Reko: a decoder for the instruction 3031 at address 0462 has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3031()
        {
            AssertCode("@@@", "3031");
        }
        // Reko: a decoder for the instruction 204C at address 046E has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_204C()
        {
            AssertCode("@@@", "204C");
        }
        // Reko: a decoder for the instruction 4061 at address 0470 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4061()
        {
            AssertCode("@@@", "4061");
        }
        // Reko: a decoder for the instruction 00A0 at address 0472 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00A0()
        {
            AssertCode("@@@", "00A0");
        }
        // Reko: a decoder for the instruction E00A at address 0474 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_E00A()
        {
            AssertCode("@@@", "E00A");
        }
        // Reko: a decoder for the instruction 08D7 at address 0486 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08D7()
        {
            AssertCode("@@@", "08D7");
        }
        // Reko: a decoder for the instruction 8F60 at address 0488 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8F60()
        {
            AssertCode("@@@", "8F60");
        }
        // Reko: a decoder for the instruction 9052 at address 048A has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9052()
        {
            AssertCode("@@@", "9052");
        }
        // Reko: a decoder for the instruction 28A8 at address 048E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28A8()
        {
            AssertCode("@@@", "28A8");
        }
        // Reko: a decoder for the instruction 0854 at address 049A has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0854()
        {
            AssertCode("@@@", "0854");
        }
        // Reko: a decoder for the instruction 4F60 at address 049E has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4F60()
        {
            AssertCode("@@@", "4F60");
        }
        // Reko: a decoder for the instruction 1896 at address 04A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1896()
        {
            AssertCode("@@@", "1896");
        }
        // Reko: a decoder for the instruction 1FD0 at address 04A6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD0()
        {
            AssertCode("@@@", "1FD0");
        }
        // Reko: a decoder for the instruction 0152 at address 04A8 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0152()
        {
            AssertCode("@@@", "0152");
        }
        // Reko: a decoder for the instruction B152 at address 04AC has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B152()
        {
            AssertCode("@@@", "B152");
        }
        // Reko: a decoder for the instruction 1A54 at address 04B8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1A54()
        {
            AssertCode("@@@", "1A54");
        }
        // Reko: a decoder for the instruction 0A61 at address 04BA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0A61()
        {
            AssertCode("@@@", "0A61");
        }
        // Reko: a decoder for the instruction 08E8 at address 04C2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08E8()
        {
            AssertCode("@@@", "08E8");
        }
        // Reko: a decoder for the instruction 0D52 at address 04D4 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0D52()
        {
            AssertCode("@@@", "0D52");
        }
        // Reko: a decoder for the instruction 0106 at address 04DA has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_0106()
        {
            AssertCode("@@@", "0106");
        }
        // Reko: a decoder for the instruction BD5A at address 04DC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BD5A()
        {
            AssertCode("@@@", "BD5A");
        }
        // Reko: a decoder for the instruction 1F90 at address 04E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F90()
        {
            AssertCode("@@@", "1F90");
        }
        // Reko: a decoder for the instruction B179 at address 04E4 has not been implemented. (Fmt16 2 ZZ  tbitw(rp) disp16 4 dest(rp) 4 pos imm 16 dest disp)
        [Test]
        public void Cr16Dasm_B179()
        {
            AssertCode("@@@", "B179");
        }
        // Reko: a decoder for the instruction 0752 at address 04EA has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0752()
        {
            AssertCode("@@@", "0752");
        }
        // Reko: a decoder for the instruction 78D7 at address 04EE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_78D7()
        {
            AssertCode("@@@", "78D7");
        }
        // Reko: a decoder for the instruction 0896 at address 04F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0896()
        {
            AssertCode("@@@", "0896");
        }
        // Reko: a decoder for the instruction 1052 at address 04F2 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1052()
        {
            AssertCode("@@@", "1052");
        }
        // Reko: a decoder for the instruction 08C2 at address 04FC has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_08C2()
        {
            AssertCode("@@@", "08C2");
        }
        // Reko: a decoder for the instruction 3000 at address 0500 has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3000()
        {
            AssertCode("@@@", "3000");
        }
        // Reko: a decoder for the instruction 28AF at address 0502 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_28AF()
        {
            AssertCode("@@@", "28AF");
        }
        // Reko: a decoder for the instruction 089F at address 050A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_089F()
        {
            AssertCode("@@@", "089F");
        }
        // Reko: a decoder for the instruction 3200 at address 050C has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3200()
        {
            AssertCode("@@@", "3200");
        }
        // Reko: a decoder for the instruction 3C00 at address 0528 has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3C00()
        {
            AssertCode("@@@", "3C00");
        }
        // Reko: a decoder for the instruction 18C3 at address 0536 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_18C3()
        {
            AssertCode("@@@", "18C3");
        }
        // Reko: a decoder for the instruction 08D6 at address 0540 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08D6()
        {
            AssertCode("@@@", "08D6");
        }
        // Reko: a decoder for the instruction E8D6 at address 0544 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E8D6()
        {
            AssertCode("@@@", "E8D6");
        }
        // Reko: a decoder for the instruction 28C3 at address 0548 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_28C3()
        {
            AssertCode("@@@", "28C3");
        }
        // Reko: a decoder for the instruction E018 at address 0552 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_E018()
        {
            AssertCode("@@@", "E018");
        }
        // Reko: a decoder for the instruction FFFE at address 0554 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_FFFE()
        {
            AssertCode("@@@", "FFFE");
        }
        // Reko: a decoder for the instruction 48C3 at address 0556 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_48C3()
        {
            AssertCode("@@@", "48C3");
        }
        // Reko: a decoder for the instruction F7FE at address 055C has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_F7FE()
        {
            AssertCode("@@@", "F7FE");
        }
        // Reko: a decoder for the instruction EFFE at address 0564 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_EFFE()
        {
            AssertCode("@@@", "EFFE");
        }
        // Reko: a decoder for the instruction 38C3 at address 0566 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_38C3()
        {
            AssertCode("@@@", "38C3");
        }
        // Reko: a decoder for the instruction E7FE at address 056C has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_E7FE()
        {
            AssertCode("@@@", "E7FE");
        }
        // Reko: a decoder for the instruction D89F at address 056E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D89F()
        {
            AssertCode("@@@", "D89F");
        }
        // Reko: a decoder for the instruction 010E at address 05A0 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_010E()
        {
            AssertCode("@@@", "010E");
        }
        // Reko: a decoder for the instruction 090A at address 05A4 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_090A()
        {
            AssertCode("@@@", "090A");
        }
        // Reko: a decoder for the instruction 145A at address 05A8 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_145A()
        {
            AssertCode("@@@", "145A");
        }
        // Reko: a decoder for the instruction 035A at address 05AA has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_035A()
        {
            AssertCode("@@@", "035A");
        }
        // Reko: a decoder for the instruction 2400 at address 05B4 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2400()
        {
            AssertCode("@@@", "2400");
        }
        // Reko: a decoder for the instruction 2600 at address 05CA has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2600()
        {
            AssertCode("@@@", "2600");
        }
        // Reko: a decoder for the instruction 2800 at address 05CE has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2800()
        {
            AssertCode("@@@", "2800");
        }
        // Reko: a decoder for the instruction 2200 at address 05D2 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2200()
        {
            AssertCode("@@@", "2200");
        }
        // Reko: a decoder for the instruction 245A at address 05D6 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_245A()
        {
            AssertCode("@@@", "245A");
        }
        // Reko: a decoder for the instruction 945A at address 05EE has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_945A()
        {
            AssertCode("@@@", "945A");
        }
        // Reko: a decoder for the instruction 9252 at address 060A has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9252()
        {
            AssertCode("@@@", "9252");
        }
        // Reko: a decoder for the instruction D254 at address 060E has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D254()
        {
            AssertCode("@@@", "D254");
        }
        // Reko: a decoder for the instruction CF60 at address 063E has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_CF60()
        {
            AssertCode("@@@", "CF60");
        }
        // Reko: a decoder for the instruction 0A54 at address 0646 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0A54()
        {
            AssertCode("@@@", "0A54");
        }
        // Reko: a decoder for the instruction 1108 at address 065A has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1108()
        {
            AssertCode("@@@", "1108");
        }
        // Reko: a decoder for the instruction 1008 at address 0664 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1008()
        {
            AssertCode("@@@", "1008");
        }
        // Reko: a decoder for the instruction 0052 at address 0666 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0052()
        {
            AssertCode("@@@", "0052");
        }
        // Reko: a decoder for the instruction 029A at address 066A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_029A()
        {
            AssertCode("@@@", "029A");
        }
        // Reko: a decoder for the instruction 4033 at address 0672 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4033()
        {
            AssertCode("@@@", "4033");
        }
        // Reko: a decoder for the instruction 4053 at address 0674 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_4053()
        {
            AssertCode("@@@", "4053");
        }
        // Reko: a decoder for the instruction 2452 at address 0678 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2452()
        {
            AssertCode("@@@", "2452");
        }
        // Reko: a decoder for the instruction 42DB at address 067C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42DB()
        {
            AssertCode("@@@", "42DB");
        }
        // Reko: a decoder for the instruction 005A at address 067E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_005A()
        {
            AssertCode("@@@", "005A");
        }
        // Reko: a decoder for the instruction EE0A at address 0680 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_EE0A()
        {
            AssertCode("@@@", "EE0A");
        }
        // Reko: a decoder for the instruction 029F at address 06A2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_029F()
        {
            AssertCode("@@@", "029F");
        }
        // Reko: a decoder for the instruction FBFF at address 06B0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_FBFF()
        {
            AssertCode("@@@", "FBFF");
        }
        // Reko: a decoder for the instruction 389F at address 06B8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_389F()
        {
            AssertCode("@@@", "389F");
        }
        // Reko: a decoder for the instruction 2897 at address 06BC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2897()
        {
            AssertCode("@@@", "2897");
        }
        // Reko: a decoder for the instruction 045A at address 06BE has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_045A()
        {
            AssertCode("@@@", "045A");
        }
        // Reko: a decoder for the instruction 0018 at address 0722 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_0018()
        {
            AssertCode("@@@", "0018");
        }
        // Reko: a decoder for the instruction 1018 at address 073A has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_1018()
        {
            AssertCode("@@@", "1018");
        }
        // Reko: a decoder for the instruction 189F at address 073E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_189F()
        {
            AssertCode("@@@", "189F");
        }
        // Reko: a decoder for the instruction 1208 at address 0744 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1208()
        {
            AssertCode("@@@", "1208");
        }
        // Reko: a decoder for the instruction 0252 at address 0746 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0252()
        {
            AssertCode("@@@", "0252");
        }
        // Reko: a decoder for the instruction 1552 at address 0754 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1552()
        {
            AssertCode("@@@", "1552");
        }
        // Reko: a decoder for the instruction 0552 at address 0758 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0552()
        {
            AssertCode("@@@", "0552");
        }
        // Reko: a decoder for the instruction 2E00 at address 0766 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2E00()
        {
            AssertCode("@@@", "2E00");
        }
        // Reko: a decoder for the instruction 1733 at address 0768 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1733()
        {
            AssertCode("@@@", "1733");
        }
        // Reko: a decoder for the instruction 0894 at address 0778 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0894()
        {
            AssertCode("@@@", "0894");
        }
        // Reko: a decoder for the instruction 78DF at address 0782 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_78DF()
        {
            AssertCode("@@@", "78DF");
        }
        // Reko: a decoder for the instruction 7033 at address 0786 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7033()
        {
            AssertCode("@@@", "7033");
        }
        // Reko: a decoder for the instruction 1894 at address 078C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1894()
        {
            AssertCode("@@@", "1894");
        }
        // Reko: a decoder for the instruction 173B at address 078E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_173B()
        {
            AssertCode("@@@", "173B");
        }
        // Reko: a decoder for the instruction 2894 at address 079A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2894()
        {
            AssertCode("@@@", "2894");
        }
        // Reko: a decoder for the instruction 489F at address 079C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_489F()
        {
            AssertCode("@@@", "489F");
        }
        // Reko: a decoder for the instruction 1452 at address 07A0 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1452()
        {
            AssertCode("@@@", "1452");
        }
        // Reko: a decoder for the instruction 2733 at address 07A8 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2733()
        {
            AssertCode("@@@", "2733");
        }
        // Reko: a decoder for the instruction 0254 at address 07E4 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0254()
        {
            AssertCode("@@@", "0254");
        }
        // Reko: a decoder for the instruction 28EF at address 07E6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_28EF()
        {
            AssertCode("@@@", "28EF");
        }
        // Reko: a decoder for the instruction 025A at address 07FC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_025A()
        {
            AssertCode("@@@", "025A");
        }
        // Reko: a decoder for the instruction 1890 at address 07FE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1890()
        {
            AssertCode("@@@", "1890");
        }
        // Reko: a decoder for the instruction 7153 at address 0806 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7153()
        {
            AssertCode("@@@", "7153");
        }
        // Reko: a decoder for the instruction D308 at address 0808 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_D308()
        {
            AssertCode("@@@", "D308");
        }
        // Reko: a decoder for the instruction 0352 at address 080A has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0352()
        {
            AssertCode("@@@", "0352");
        }
        // Reko: a decoder for the instruction 015A at address 0812 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_015A()
        {
            AssertCode("@@@", "015A");
        }
        // Reko: a decoder for the instruction 18D0 at address 0814 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_18D0()
        {
            AssertCode("@@@", "18D0");
        }
        // Reko: a decoder for the instruction 48A2 at address 0818 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48A2()
        {
            AssertCode("@@@", "48A2");
        }
        // Reko: a decoder for the instruction E461 at address 081A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_E461()
        {
            AssertCode("@@@", "E461");
        }
        // Reko: a decoder for the instruction 48E2 at address 081C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48E2()
        {
            AssertCode("@@@", "48E2");
        }
        // Reko: a decoder for the instruction 2033 at address 081E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2033()
        {
            AssertCode("@@@", "2033");
        }
        // Reko: a decoder for the instruction 08D4 at address 0820 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08D4()
        {
            AssertCode("@@@", "08D4");
        }
        // Reko: a decoder for the instruction 713B at address 083E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_713B()
        {
            AssertCode("@@@", "713B");
        }
        // Reko: a decoder for the instruction 075A at address 0840 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_075A()
        {
            AssertCode("@@@", "075A");
        }
        // Reko: a decoder for the instruction 0890 at address 0856 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0890()
        {
            AssertCode("@@@", "0890");
        }
        // Reko: a decoder for the instruction 033B at address 085A has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_033B()
        {
            AssertCode("@@@", "033B");
        }
        // Reko: a decoder for the instruction D7FE at address 08A6 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_D7FE()
        {
            AssertCode("@@@", "D7FE");
        }
        // Reko: a decoder for the instruction 0294 at address 0A52 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0294()
        {
            AssertCode("@@@", "0294");
        }
        // Reko: a decoder for the instruction 129F at address 0A54 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_129F()
        {
            AssertCode("@@@", "129F");
        }
        // Reko: a decoder for the instruction 1033 at address 0A60 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1033()
        {
            AssertCode("@@@", "1033");
        }
        // Reko: a decoder for the instruction 103B at address 0AD6 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_103B()
        {
            AssertCode("@@@", "103B");
        }
        // Reko: a decoder for the instruction 1296 at address 0B34 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1296()
        {
            AssertCode("@@@", "1296");
        }
        // Reko: a decoder for the instruction 04D0 at address 0B6E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04D0()
        {
            AssertCode("@@@", "04D0");
        }
        // Reko: a decoder for the instruction 02AF at address 0B7A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_02AF()
        {
            AssertCode("@@@", "02AF");
        }
        // Reko: a decoder for the instruction 22AF at address 0BF8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_22AF()
        {
            AssertCode("@@@", "22AF");
        }
        // Reko: a decoder for the instruction 0A52 at address 0C14 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0A52()
        {
            AssertCode("@@@", "0A52");
        }
        // Reko: a decoder for the instruction BA52 at address 0C1C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BA52()
        {
            AssertCode("@@@", "BA52");
        }
        // Reko: a decoder for the instruction A8DF at address 0C28 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_A8DF()
        {
            AssertCode("@@@", "A8DF");
        }
        // Reko: a decoder for the instruction 7027 at address 0C2E has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7027()
        {
            AssertCode("@@@", "7027");
        }
        // Reko: a decoder for the instruction 0008 at address 0C32 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0008()
        {
            AssertCode("@@@", "0008");
        }
        // Reko: a decoder for the instruction C8A8 at address 0C42 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C8A8()
        {
            AssertCode("@@@", "C8A8");
        }
        // Reko: a decoder for the instruction 0261 at address 0C56 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0261()
        {
            AssertCode("@@@", "0261");
        }
        // Reko: a decoder for the instruction 3260 at address 0C58 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3260()
        {
            AssertCode("@@@", "3260");
        }
        // Reko: a decoder for the instruction B25A at address 0C6E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B25A()
        {
            AssertCode("@@@", "B25A");
        }
        // Reko: a decoder for the instruction 3A20 at address 0C70 has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3A20()
        {
            AssertCode("@@@", "3A20");
        }
        // Reko: a decoder for the instruction 20D0 at address 0C72 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20D0()
        {
            AssertCode("@@@", "20D0");
        }
        // Reko: a decoder for the instruction 2254 at address 0C74 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2254()
        {
            AssertCode("@@@", "2254");
        }
        // Reko: a decoder for the instruction BC54 at address 0C9A has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BC54()
        {
            AssertCode("@@@", "BC54");
        }
        // Reko: a decoder for the instruction 3800 at address 0C9C has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3800()
        {
            AssertCode("@@@", "3800");
        }
        // Reko: a decoder for the instruction 2C61 at address 0C9E has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2C61()
        {
            AssertCode("@@@", "2C61");
        }
        // Reko: a decoder for the instruction 0040 at address 0CA8 has not been implemented. (0100 0000 0xxx xxxx  Fmt9 1 ZZ  ashub cnt(left +), reg 4 dest reg 3 count imm)
        [Test]
        public void Cr16Dasm_0040()
        {
            AssertCode("@@@", "0040");
        }
        // Reko: a decoder for the instruction 215A at address 0CB4 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_215A()
        {
            AssertCode("@@@", "215A");
        }
        // Reko: a decoder for the instruction 0133 at address 0CB6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0133()
        {
            AssertCode("@@@", "0133");
        }
        // Reko: a decoder for the instruction B122 at address 0CB8 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B122()
        {
            AssertCode("@@@", "B122");
        }
        // Reko: a decoder for the instruction FDFF at address 0CC8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_FDFF()
        {
            AssertCode("@@@", "FDFF");
        }
        // Reko: a decoder for the instruction 789F at address 0CFA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_789F()
        {
            AssertCode("@@@", "789F");
        }
        // Reko: a decoder for the instruction 289A at address 0D02 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_289A()
        {
            AssertCode("@@@", "289A");
        }
        // Reko: a decoder for the instruction 2FD0 at address 0D04 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FD0()
        {
            AssertCode("@@@", "2FD0");
        }
        // Reko: a decoder for the instruction A8AC at address 0D06 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A8AC()
        {
            AssertCode("@@@", "A8AC");
        }
        // Reko: a decoder for the instruction 0733 at address 0D0A has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0733()
        {
            AssertCode("@@@", "0733");
        }
        // Reko: a decoder for the instruction 0F90 at address 0D0C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F90()
        {
            AssertCode("@@@", "0F90");
        }
        // Reko: a decoder for the instruction 7053 at address 0D0E has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7053()
        {
            AssertCode("@@@", "7053");
        }
        // Reko: a decoder for the instruction 5F90 at address 0D12 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5F90()
        {
            AssertCode("@@@", "5F90");
        }
        // Reko: a decoder for the instruction 753B at address 0D14 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_753B()
        {
            AssertCode("@@@", "753B");
        }
        // Reko: a decoder for the instruction B552 at address 0D16 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B552()
        {
            AssertCode("@@@", "B552");
        }
        // Reko: a decoder for the instruction A361 at address 0D22 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A361()
        {
            AssertCode("@@@", "A361");
        }
        // Reko: a decoder for the instruction 08AC at address 0D40 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08AC()
        {
            AssertCode("@@@", "08AC");
        }
        // Reko: a decoder for the instruction B45A at address 0D4E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B45A()
        {
            AssertCode("@@@", "B45A");
        }
        // Reko: a decoder for the instruction 28E2 at address 0D6C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28E2()
        {
            AssertCode("@@@", "28E2");
        }
        // Reko: a decoder for the instruction 915A at address 0D86 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_915A()
        {
            AssertCode("@@@", "915A");
        }
        // Reko: a decoder for the instruction 58AF at address 0DA6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_58AF()
        {
            AssertCode("@@@", "58AF");
        }
        // Reko: a decoder for the instruction 2F91 at address 0DC2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2F91()
        {
            AssertCode("@@@", "2F91");
        }
        // Reko: a decoder for the instruction 28D0 at address 0DC4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28D0()
        {
            AssertCode("@@@", "28D0");
        }
        // Reko: a decoder for the instruction 0F91 at address 0DE4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F91()
        {
            AssertCode("@@@", "0F91");
        }
        // Reko: a decoder for the instruction 08D0 at address 0DE8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08D0()
        {
            AssertCode("@@@", "08D0");
        }
        // Reko: a decoder for the instruction BD54 at address 0E24 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BD54()
        {
            AssertCode("@@@", "BD54");
        }
        // Reko: a decoder for the instruction 2D61 at address 0E28 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2D61()
        {
            AssertCode("@@@", "2D61");
        }
        // Reko: a decoder for the instruction 729A at address 0E2A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_729A()
        {
            AssertCode("@@@", "729A");
        }
        // Reko: a decoder for the instruction 1752 at address 0E34 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1752()
        {
            AssertCode("@@@", "1752");
        }
        // Reko: a decoder for the instruction 48AF at address 0E38 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_48AF()
        {
            AssertCode("@@@", "48AF");
        }
        // Reko: a decoder for the instruction 04B0 at address 0E3C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B0()
        {
            AssertCode("@@@", "04B0");
        }
        // Reko: a decoder for the instruction A29B at address 0E62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A29B()
        {
            AssertCode("@@@", "A29B");
        }
        // Reko: a decoder for the instruction 08DC at address 0E6E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08DC()
        {
            AssertCode("@@@", "08DC");
        }
        // Reko: a decoder for the instruction 18DD at address 0E70 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_18DD()
        {
            AssertCode("@@@", "18DD");
        }
        // Reko: a decoder for the instruction A233 at address 0E74 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A233()
        {
            AssertCode("@@@", "A233");
        }
        // Reko: a decoder for the instruction 18DF at address 0E80 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_18DF()
        {
            AssertCode("@@@", "18DF");
        }
        // Reko: a decoder for the instruction 0227 at address 0E86 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0227()
        {
            AssertCode("@@@", "0227");
        }
        // Reko: a decoder for the instruction 0208 at address 0E8A has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0208()
        {
            AssertCode("@@@", "0208");
        }
        // Reko: a decoder for the instruction B227 at address 0E94 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B227()
        {
            AssertCode("@@@", "B227");
        }
        // Reko: a decoder for the instruction A8DA at address 0EA0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A8DA()
        {
            AssertCode("@@@", "A8DA");
        }
        // Reko: a decoder for the instruction 28AC at address 0EDE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28AC()
        {
            AssertCode("@@@", "28AC");
        }
        // Reko: a decoder for the instruction 78DA at address 0EE4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_78DA()
        {
            AssertCode("@@@", "78DA");
        }
        // Reko: a decoder for the instruction C8AC at address 0F1A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C8AC()
        {
            AssertCode("@@@", "C8AC");
        }
        // Reko: a decoder for the instruction 08AF at address 0F20 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_08AF()
        {
            AssertCode("@@@", "08AF");
        }
        // Reko: a decoder for the instruction 00B0 at address 0F24 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00B0()
        {
            AssertCode("@@@", "00B0");
        }
        // Reko: a decoder for the instruction 0CF0 at address 0F26 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CF0()
        {
            AssertCode("@@@", "0CF0");
        }
        // Reko: a decoder for the instruction A89A at address 0F2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A89A()
        {
            AssertCode("@@@", "A89A");
        }
        // Reko: a decoder for the instruction 7A3B at address 0F30 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7A3B()
        {
            AssertCode("@@@", "7A3B");
        }
        // Reko: a decoder for the instruction 0FE0 at address 0F34 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FE0()
        {
            AssertCode("@@@", "0FE0");
        }
        // Reko: a decoder for the instruction BB5A at address 0F38 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BB5A()
        {
            AssertCode("@@@", "BB5A");
        }
        // Reko: a decoder for the instruction 7A53 at address 0F40 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7A53()
        {
            AssertCode("@@@", "7A53");
        }
        // Reko: a decoder for the instruction 0FA0 at address 0F54 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FA0()
        {
            AssertCode("@@@", "0FA0");
        }
        // Reko: a decoder for the instruction 0361 at address 0F56 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0361()
        {
            AssertCode("@@@", "0361");
        }
        // Reko: a decoder for the instruction C361 at address 0F58 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_C361()
        {
            AssertCode("@@@", "C361");
        }
        // Reko: a decoder for the instruction B5FE at address 0F86 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_B5FE()
        {
            AssertCode("@@@", "B5FE");
        }
        // Reko: a decoder for the instruction 04B1 at address 0F88 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B1()
        {
            AssertCode("@@@", "04B1");
        }
        // Reko: a decoder for the instruction B9FE at address 0F90 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_B9FE()
        {
            AssertCode("@@@", "B9FE");
        }
        // Reko: a decoder for the instruction 9FFE at address 0FB4 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_9FFE()
        {
            AssertCode("@@@", "9FFE");
        }
        // Reko: a decoder for the instruction 175A at address 0FBC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_175A()
        {
            AssertCode("@@@", "175A");
        }
        // Reko: a decoder for the instruction E9FE at address 1000 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_E9FE()
        {
            AssertCode("@@@", "E9FE");
        }
        // Reko: a decoder for the instruction 2052 at address 1026 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2052()
        {
            AssertCode("@@@", "2052");
        }
        // Reko: a decoder for the instruction 089A at address 1060 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_089A()
        {
            AssertCode("@@@", "089A");
        }
        // Reko: a decoder for the instruction 0033 at address 1062 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0033()
        {
            AssertCode("@@@", "0033");
        }
        // Reko: a decoder for the instruction 1FFC at address 1074 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FFC()
        {
            AssertCode("@@@", "1FFC");
        }
        // Reko: a decoder for the instruction 789A at address 1080 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_789A()
        {
            AssertCode("@@@", "789A");
        }
        // Reko: a decoder for the instruction 7733 at address 1082 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7733()
        {
            AssertCode("@@@", "7733");
        }
        // Reko: a decoder for the instruction A8AF at address 1084 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_A8AF()
        {
            AssertCode("@@@", "A8AF");
        }
        // Reko: a decoder for the instruction BC5A at address 108C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BC5A()
        {
            AssertCode("@@@", "BC5A");
        }
        // Reko: a decoder for the instruction 3890 at address 1092 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3890()
        {
            AssertCode("@@@", "3890");
        }
        // Reko: a decoder for the instruction 0333 at address 1094 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0333()
        {
            AssertCode("@@@", "0333");
        }
        // Reko: a decoder for the instruction 38D0 at address 1096 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_38D0()
        {
            AssertCode("@@@", "38D0");
        }
        // Reko: a decoder for the instruction 3753 at address 1098 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_3753()
        {
            AssertCode("@@@", "3753");
        }
        // Reko: a decoder for the instruction 353B at address 109E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_353B()
        {
            AssertCode("@@@", "353B");
        }
        // Reko: a decoder for the instruction 08E2 at address 10C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08E2()
        {
            AssertCode("@@@", "08E2");
        }
        // Reko: a decoder for the instruction F4FF at address 10E2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F4FF()
        {
            AssertCode("@@@", "F4FF");
        }
        // Reko: a decoder for the instruction 6018 at address 1114 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_6018()
        {
            AssertCode("@@@", "6018");
        }
        // Reko: a decoder for the instruction 0D53 at address 111A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0D53()
        {
            AssertCode("@@@", "0D53");
        }
        // Reko: a decoder for the instruction 1C9F at address 111E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1C9F()
        {
            AssertCode("@@@", "1C9F");
        }
        // Reko: a decoder for the instruction 0153 at address 1122 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0153()
        {
            AssertCode("@@@", "0153");
        }
        // Reko: a decoder for the instruction 7C90 at address 1128 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7C90()
        {
            AssertCode("@@@", "7C90");
        }
        // Reko: a decoder for the instruction 0FE2 at address 112E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FE2()
        {
            AssertCode("@@@", "0FE2");
        }
        // Reko: a decoder for the instruction DFD0 at address 1130 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFD0()
        {
            AssertCode("@@@", "DFD0");
        }
        // Reko: a decoder for the instruction 2F90 at address 1142 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2F90()
        {
            AssertCode("@@@", "2F90");
        }
        // Reko: a decoder for the instruction 2753 at address 1144 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2753()
        {
            AssertCode("@@@", "2753");
        }
        // Reko: a decoder for the instruction 4CA2 at address 114C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CA2()
        {
            AssertCode("@@@", "4CA2");
        }
        // Reko: a decoder for the instruction 0CA2 at address 1156 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CA2()
        {
            AssertCode("@@@", "0CA2");
        }
        // Reko: a decoder for the instruction 8061 at address 1158 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8061()
        {
            AssertCode("@@@", "8061");
        }
        // Reko: a decoder for the instruction 0CE2 at address 115A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CE2()
        {
            AssertCode("@@@", "0CE2");
        }
        // Reko: a decoder for the instruction 0C90 at address 115C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0C90()
        {
            AssertCode("@@@", "0C90");
        }
        // Reko: a decoder for the instruction 703B at address 115E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_703B()
        {
            AssertCode("@@@", "703B");
        }
        // Reko: a decoder for the instruction 0CD0 at address 1160 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CD0()
        {
            AssertCode("@@@", "0CD0");
        }
        // Reko: a decoder for the instruction 8A61 at address 1162 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8A61()
        {
            AssertCode("@@@", "8A61");
        }
        // Reko: a decoder for the instruction 0C94 at address 1170 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0C94()
        {
            AssertCode("@@@", "0C94");
        }
        // Reko: a decoder for the instruction 7CD4 at address 1174 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7CD4()
        {
            AssertCode("@@@", "7CD4");
        }
        // Reko: a decoder for the instruction 1CC3 at address 117E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_1CC3()
        {
            AssertCode("@@@", "1CC3");
        }
        // Reko: a decoder for the instruction 1F92 at address 1182 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F92()
        {
            AssertCode("@@@", "1F92");
        }
        // Reko: a decoder for the instruction 0C9F at address 1190 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0C9F()
        {
            AssertCode("@@@", "0C9F");
        }
        // Reko: a decoder for the instruction 0C9A at address 11A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0C9A()
        {
            AssertCode("@@@", "0C9A");
        }
        // Reko: a decoder for the instruction 0253 at address 11AE has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0253()
        {
            AssertCode("@@@", "0253");
        }
        // Reko: a decoder for the instruction 1152 at address 11B2 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1152()
        {
            AssertCode("@@@", "1152");
        }
        // Reko: a decoder for the instruction 2CDF at address 11B6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2CDF()
        {
            AssertCode("@@@", "2CDF");
        }
        // Reko: a decoder for the instruction 0CC2 at address 11CA has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_0CC2()
        {
            AssertCode("@@@", "0CC2");
        }
        // Reko: a decoder for the instruction 0FE4 at address 11E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FE4()
        {
            AssertCode("@@@", "0FE4");
        }
        // Reko: a decoder for the instruction B55A at address 11EC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B55A()
        {
            AssertCode("@@@", "B55A");
        }
        // Reko: a decoder for the instruction 2C97 at address 11F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2C97()
        {
            AssertCode("@@@", "2C97");
        }
        // Reko: a decoder for the instruction 3FA4 at address 11F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FA4()
        {
            AssertCode("@@@", "3FA4");
        }
        // Reko: a decoder for the instruction AFA4 at address 1206 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFA4()
        {
            AssertCode("@@@", "AFA4");
        }
        // Reko: a decoder for the instruction 0154 at address 1214 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0154()
        {
            AssertCode("@@@", "0154");
        }
        // Reko: a decoder for the instruction 1FE2 at address 1216 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FE2()
        {
            AssertCode("@@@", "1FE2");
        }
        // Reko: a decoder for the instruction 0CDF at address 1236 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0CDF()
        {
            AssertCode("@@@", "0CDF");
        }
        // Reko: a decoder for the instruction 8C9F at address 123A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8C9F()
        {
            AssertCode("@@@", "8C9F");
        }
        // Reko: a decoder for the instruction 0852 at address 1240 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0852()
        {
            AssertCode("@@@", "0852");
        }
        // Reko: a decoder for the instruction 8753 at address 124C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_8753()
        {
            AssertCode("@@@", "8753");
        }
        // Reko: a decoder for the instruction D008 at address 124E has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_D008()
        {
            AssertCode("@@@", "D008");
        }
        // Reko: a decoder for the instruction 783B at address 1258 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_783B()
        {
            AssertCode("@@@", "783B");
        }
        // Reko: a decoder for the instruction 7CD0 at address 125E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7CD0()
        {
            AssertCode("@@@", "7CD0");
        }
        // Reko: a decoder for the instruction 2CA2 at address 1262 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CA2()
        {
            AssertCode("@@@", "2CA2");
        }
        // Reko: a decoder for the instruction 4261 at address 1264 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4261()
        {
            AssertCode("@@@", "4261");
        }
        // Reko: a decoder for the instruction 2CE2 at address 1266 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CE2()
        {
            AssertCode("@@@", "2CE2");
        }
        // Reko: a decoder for the instruction 0CD4 at address 126C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CD4()
        {
            AssertCode("@@@", "0CD4");
        }
        // Reko: a decoder for the instruction BBFE at address 1272 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_BBFE()
        {
            AssertCode("@@@", "BBFE");
        }
        // Reko: a decoder for the instruction A5FE at address 1288 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_A5FE()
        {
            AssertCode("@@@", "A5FE");
        }
        // Reko: a decoder for the instruction 873B at address 1298 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_873B()
        {
            AssertCode("@@@", "873B");
        }
        // Reko: a decoder for the instruction 085A at address 129A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_085A()
        {
            AssertCode("@@@", "085A");
        }
        // Reko: a decoder for the instruction 0F92 at address 129E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F92()
        {
            AssertCode("@@@", "0F92");
        }
        // Reko: a decoder for the instruction F0FF at address 12D2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F0FF()
        {
            AssertCode("@@@", "F0FF");
        }
        // Reko: a decoder for the instruction FC61 at address 12D8 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_FC61()
        {
            AssertCode("@@@", "FC61");
        }
        // Reko: a decoder for the instruction 2FE0 at address 12DA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FE0()
        {
            AssertCode("@@@", "2FE0");
        }
        // Reko: a decoder for the instruction 8CA0 at address 12DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CA0()
        {
            AssertCode("@@@", "8CA0");
        }
        // Reko: a decoder for the instruction CCA2 at address 12E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CCA2()
        {
            AssertCode("@@@", "CCA2");
        }
        // Reko: a decoder for the instruction 0C96 at address 12E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0C96()
        {
            AssertCode("@@@", "0C96");
        }
        // Reko: a decoder for the instruction 100067029D03 at address 12F4 has not been implemented. (Fmr3a 3 bra cond disp24 24 dest disp*2 4 cond imm 4 ope 0)
        [Test]
        public void Cr16Dasm_100067029D03()
        {
            AssertCode("@@@", "100067029D03");
        }
        // Reko: a decoder for the instruction 985A at address 1358 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_985A()
        {
            AssertCode("@@@", "985A");
        }
        // Reko: a decoder for the instruction 2FA2 at address 136A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FA2()
        {
            AssertCode("@@@", "2FA2");
        }
        // Reko: a decoder for the instruction 0FA2 at address 137C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FA2()
        {
            AssertCode("@@@", "0FA2");
        }
        // Reko: a decoder for the instruction 0853 at address 13C6 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0853()
        {
            AssertCode("@@@", "0853");
        }
        // Reko: a decoder for the instruction 8CDF at address 13CE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8CDF()
        {
            AssertCode("@@@", "8CDF");
        }
        // Reko: a decoder for the instruction B7F8 at address 13DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B7F8()
        {
            AssertCode("@@@", "B7F8");
        }
        // Reko: a decoder for the instruction 0CC3 at address 1400 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0CC3()
        {
            AssertCode("@@@", "0CC3");
        }
        // Reko: a decoder for the instruction 01FF at address 144C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_01FF()
        {
            AssertCode("@@@", "01FF");
        }
        // Reko: a decoder for the instruction 99FE at address 1458 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_99FE()
        {
            AssertCode("@@@", "99FE");
        }
        // Reko: a decoder for the instruction AFE6 at address 1464 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFE6()
        {
            AssertCode("@@@", "AFE6");
        }
        // Reko: a decoder for the instruction 7B53 at address 146C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7B53()
        {
            AssertCode("@@@", "7B53");
        }
        // Reko: a decoder for the instruction AFA6 at address 1494 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFA6()
        {
            AssertCode("@@@", "AFA6");
        }
        // Reko: a decoder for the instruction 0FA4 at address 149E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FA4()
        {
            AssertCode("@@@", "0FA4");
        }
        // Reko: a decoder for the instruction E3FE at address 14A4 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_E3FE()
        {
            AssertCode("@@@", "E3FE");
        }
        // Reko: a decoder for the instruction D5FE at address 14B2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_D5FE()
        {
            AssertCode("@@@", "D5FE");
        }
        // Reko: a decoder for the instruction 2FFE at address 14C4 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_2FFE()
        {
            AssertCode("@@@", "2FFE");
        }
        // Reko: a decoder for the instruction 1DFE at address 14D6 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_1DFE()
        {
            AssertCode("@@@", "1DFE");
        }
        // Reko: a decoder for the instruction AC90 at address 1506 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AC90()
        {
            AssertCode("@@@", "AC90");
        }
        // Reko: a decoder for the instruction A033 at address 150E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A033()
        {
            AssertCode("@@@", "A033");
        }
        // Reko: a decoder for the instruction 1032 at address 1514 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1032()
        {
            AssertCode("@@@", "1032");
        }
        // Reko: a decoder for the instruction 1254 at address 151A has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1254()
        {
            AssertCode("@@@", "1254");
        }
        // Reko: a decoder for the instruction 0FE6 at address 1536 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FE6()
        {
            AssertCode("@@@", "0FE6");
        }
        // Reko: a decoder for the instruction 1D54 at address 1538 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1D54()
        {
            AssertCode("@@@", "1D54");
        }
        // Reko: a decoder for the instruction FFE4 at address 153A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FFE4()
        {
            AssertCode("@@@", "FFE4");
        }
        // Reko: a decoder for the instruction 7FD2 at address 154E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FD2()
        {
            AssertCode("@@@", "7FD2");
        }
        // Reko: a decoder for the instruction 8FD2 at address 1554 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FD2()
        {
            AssertCode("@@@", "8FD2");
        }
        // Reko: a decoder for the instruction 8CA2 at address 155A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CA2()
        {
            AssertCode("@@@", "8CA2");
        }
        // Reko: a decoder for the instruction 2FA6 at address 1560 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FA6()
        {
            AssertCode("@@@", "2FA6");
        }
        // Reko: a decoder for the instruction A861 at address 1566 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A861()
        {
            AssertCode("@@@", "A861");
        }
        // Reko: a decoder for the instruction 8CE2 at address 1568 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CE2()
        {
            AssertCode("@@@", "8CE2");
        }
        // Reko: a decoder for the instruction 0F94 at address 156A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F94()
        {
            AssertCode("@@@", "0F94");
        }
        // Reko: a decoder for the instruction 073B at address 156C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_073B()
        {
            AssertCode("@@@", "073B");
        }
        // Reko: a decoder for the instruction 0FA6 at address 1570 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FA6()
        {
            AssertCode("@@@", "0FA6");
        }
        // Reko: a decoder for the instruction A061 at address 1572 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A061()
        {
            AssertCode("@@@", "A061");
        }
        // Reko: a decoder for the instruction AF92 at address 1584 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AF92()
        {
            AssertCode("@@@", "AF92");
        }
        // Reko: a decoder for the instruction 0A33 at address 1586 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0A33()
        {
            AssertCode("@@@", "0A33");
        }
        // Reko: a decoder for the instruction ACD4 at address 1588 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACD4()
        {
            AssertCode("@@@", "ACD4");
        }
        // Reko: a decoder for the instruction 8053 at address 15C4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_8053()
        {
            AssertCode("@@@", "8053");
        }
        // Reko: a decoder for the instruction B9F6 at address 15DA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B9F6()
        {
            AssertCode("@@@", "B9F6");
        }
        // Reko: a decoder for the instruction 1C90 at address 15E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1C90()
        {
            AssertCode("@@@", "1C90");
        }
        // Reko: a decoder for the instruction 1FD2 at address 15E2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD2()
        {
            AssertCode("@@@", "1FD2");
        }
        // Reko: a decoder for the instruction ACDF at address 15F0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ACDF()
        {
            AssertCode("@@@", "ACDF");
        }
        // Reko: a decoder for the instruction 7C9F at address 15F4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7C9F()
        {
            AssertCode("@@@", "7C9F");
        }
        // Reko: a decoder for the instruction D108 at address 160A has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_D108()
        {
            AssertCode("@@@", "D108");
        }
        // Reko: a decoder for the instruction 8261 at address 161C has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8261()
        {
            AssertCode("@@@", "8261");
        }
        // Reko: a decoder for the instruction 1C94 at address 1620 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1C94()
        {
            AssertCode("@@@", "1C94");
        }
        // Reko: a decoder for the instruction 4133 at address 1622 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4133()
        {
            AssertCode("@@@", "4133");
        }
        // Reko: a decoder for the instruction 1CD4 at address 1624 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1CD4()
        {
            AssertCode("@@@", "1CD4");
        }
        // Reko: a decoder for the instruction 1753 at address 165A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1753()
        {
            AssertCode("@@@", "1753");
        }
        // Reko: a decoder for the instruction 8FA4 at address 1660 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FA4()
        {
            AssertCode("@@@", "8FA4");
        }
        // Reko: a decoder for the instruction 5F92 at address 1664 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5F92()
        {
            AssertCode("@@@", "5F92");
        }
        // Reko: a decoder for the instruction 8FE4 at address 1688 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FE4()
        {
            AssertCode("@@@", "8FE4");
        }
        // Reko: a decoder for the instruction EDFE at address 168E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_EDFE()
        {
            AssertCode("@@@", "EDFE");
        }
        // Reko: a decoder for the instruction 1F96 at address 1694 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F96()
        {
            AssertCode("@@@", "1F96");
        }
        // Reko: a decoder for the instruction 0FB0 at address 169A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FB0()
        {
            AssertCode("@@@", "0FB0");
        }
        // Reko: a decoder for the instruction C9FE at address 16B2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_C9FE()
        {
            AssertCode("@@@", "C9FE");
        }
        // Reko: a decoder for the instruction 63FE at address 16C4 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_63FE()
        {
            AssertCode("@@@", "63FE");
        }
        // Reko: a decoder for the instruction CBF4 at address 17C8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CBF4()
        {
            AssertCode("@@@", "CBF4");
        }
        // Reko: a decoder for the instruction 0396 at address 18C2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0396()
        {
            AssertCode("@@@", "0396");
        }
        // Reko: a decoder for the instruction 039F at address 18CA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_039F()
        {
            AssertCode("@@@", "039F");
        }
        // Reko: a decoder for the instruction 6890 at address 18EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6890()
        {
            AssertCode("@@@", "6890");
        }
        // Reko: a decoder for the instruction 0652 at address 18F4 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0652()
        {
            AssertCode("@@@", "0652");
        }
        // Reko: a decoder for the instruction 0653 at address 18F8 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0653()
        {
            AssertCode("@@@", "0653");
        }
        // Reko: a decoder for the instruction 1632 at address 1906 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1632()
        {
            AssertCode("@@@", "1632");
        }
        // Reko: a decoder for the instruction 68D0 at address 1908 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_68D0()
        {
            AssertCode("@@@", "68D0");
        }
        // Reko: a decoder for the instruction 9460 at address 190A has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9460()
        {
            AssertCode("@@@", "9460");
        }
        // Reko: a decoder for the instruction A4F0 at address 190E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A4F0()
        {
            AssertCode("@@@", "A4F0");
        }
        // Reko: a decoder for the instruction 9032 at address 1912 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9032()
        {
            AssertCode("@@@", "9032");
        }
        // Reko: a decoder for the instruction 0B5A at address 1928 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0B5A()
        {
            AssertCode("@@@", "0B5A");
        }
        // Reko: a decoder for the instruction B753 at address 192A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_B753()
        {
            AssertCode("@@@", "B753");
        }
        // Reko: a decoder for the instruction 28A2 at address 194C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28A2()
        {
            AssertCode("@@@", "28A2");
        }
        // Reko: a decoder for the instruction 18C2 at address 198C has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_18C2()
        {
            AssertCode("@@@", "18C2");
        }
        // Reko: a decoder for the instruction 9060 at address 1990 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9060()
        {
            AssertCode("@@@", "9060");
        }
        // Reko: a decoder for the instruction 2061 at address 1996 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2061()
        {
            AssertCode("@@@", "2061");
        }
        // Reko: a decoder for the instruction A0F0 at address 199A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0F0()
        {
            AssertCode("@@@", "A0F0");
        }
        // Reko: a decoder for the instruction 9260 at address 19B0 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9260()
        {
            AssertCode("@@@", "9260");
        }
        // Reko: a decoder for the instruction 42B0 at address 19B4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B0()
        {
            AssertCode("@@@", "42B0");
        }
        // Reko: a decoder for the instruction 40F0 at address 19B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_40F0()
        {
            AssertCode("@@@", "40F0");
        }
        // Reko: a decoder for the instruction 4FE4 at address 19E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FE4()
        {
            AssertCode("@@@", "4FE4");
        }
        // Reko: a decoder for the instruction 1F95 at address 19EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F95()
        {
            AssertCode("@@@", "1F95");
        }
        // Reko: a decoder for the instruction 1027 at address 19EE has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1027()
        {
            AssertCode("@@@", "1027");
        }
        // Reko: a decoder for the instruction 0108 at address 19F2 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0108()
        {
            AssertCode("@@@", "0108");
        }
        // Reko: a decoder for the instruction 1025 at address 19F8 has not been implemented. (Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1025()
        {
            AssertCode("@@@", "1025");
        }
        // Reko: a decoder for the instruction B027 at address 1A00 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B027()
        {
            AssertCode("@@@", "B027");
        }
        // Reko: a decoder for the instruction 0A9F at address 1A28 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0A9F()
        {
            AssertCode("@@@", "0A9F");
        }
        // Reko: a decoder for the instruction 9832 at address 1A30 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9832()
        {
            AssertCode("@@@", "9832");
        }
        // Reko: a decoder for the instruction 8FD0 at address 1A32 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FD0()
        {
            AssertCode("@@@", "8FD0");
        }
        // Reko: a decoder for the instruction 0A90 at address 1A38 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0A90()
        {
            AssertCode("@@@", "0A90");
        }
        // Reko: a decoder for the instruction DFA4 at address 1A3A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFA4()
        {
            AssertCode("@@@", "DFA4");
        }
        // Reko: a decoder for the instruction 7F90 at address 1A40 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F90()
        {
            AssertCode("@@@", "7F90");
        }
        // Reko: a decoder for the instruction 0753 at address 1A42 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0753()
        {
            AssertCode("@@@", "0753");
        }
        // Reko: a decoder for the instruction 8AA2 at address 1A4A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8AA2()
        {
            AssertCode("@@@", "8AA2");
        }
        // Reko: a decoder for the instruction A45A at address 1A4E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A45A()
        {
            AssertCode("@@@", "A45A");
        }
        // Reko: a decoder for the instruction 0AD0 at address 1A74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AD0()
        {
            AssertCode("@@@", "0AD0");
        }
        // Reko: a decoder for the instruction 2AA2 at address 1A76 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AA2()
        {
            AssertCode("@@@", "2AA2");
        }
        // Reko: a decoder for the instruction C261 at address 1A78 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_C261()
        {
            AssertCode("@@@", "C261");
        }
        // Reko: a decoder for the instruction 2AE2 at address 1A7A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AE2()
        {
            AssertCode("@@@", "2AE2");
        }
        // Reko: a decoder for the instruction 1A94 at address 1A7C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1A94()
        {
            AssertCode("@@@", "1A94");
        }
        // Reko: a decoder for the instruction 7133 at address 1A7E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7133()
        {
            AssertCode("@@@", "7133");
        }
        // Reko: a decoder for the instruction 1AD4 at address 1A80 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1AD4()
        {
            AssertCode("@@@", "1AD4");
        }
        // Reko: a decoder for the instruction 2F92 at address 1A82 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2F92()
        {
            AssertCode("@@@", "2F92");
        }
        // Reko: a decoder for the instruction 723B at address 1A84 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_723B()
        {
            AssertCode("@@@", "723B");
        }
        // Reko: a decoder for the instruction 2FD2 at address 1A86 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FD2()
        {
            AssertCode("@@@", "2FD2");
        }
        // Reko: a decoder for the instruction CD61 at address 1A88 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_CD61()
        {
            AssertCode("@@@", "CD61");
        }
        // Reko: a decoder for the instruction 2F93 at address 1A96 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2F93()
        {
            AssertCode("@@@", "2F93");
        }
        // Reko: a decoder for the instruction 2127 at address 1A98 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2127()
        {
            AssertCode("@@@", "2127");
        }
        // Reko: a decoder for the instruction 0D82 at address 1AA8 has not been implemented. (Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_0D82()
        {
            AssertCode("@@@", "0D82");
        }
        // Reko: a decoder for the instruction 0AC3 at address 1AD8 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0AC3()
        {
            AssertCode("@@@", "0AC3");
        }
        // Reko: a decoder for the instruction 7A9F at address 1ADC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7A9F()
        {
            AssertCode("@@@", "7A9F");
        }
        // Reko: a decoder for the instruction 095A at address 1AE0 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_095A()
        {
            AssertCode("@@@", "095A");
        }
        // Reko: a decoder for the instruction 9753 at address 1AE2 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9753()
        {
            AssertCode("@@@", "9753");
        }
        // Reko: a decoder for the instruction D208 at address 1AF4 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_D208()
        {
            AssertCode("@@@", "D208");
        }
        // Reko: a decoder for the instruction 0A94 at address 1B0A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0A94()
        {
            AssertCode("@@@", "0A94");
        }
        // Reko: a decoder for the instruction 0AD4 at address 1B0E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AD4()
        {
            AssertCode("@@@", "0AD4");
        }
        // Reko: a decoder for the instruction 1AC3 at address 1B2E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_1AC3()
        {
            AssertCode("@@@", "1AC3");
        }
        // Reko: a decoder for the instruction 0290 at address 1B5E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0290()
        {
            AssertCode("@@@", "0290");
        }
        // Reko: a decoder for the instruction B752 at address 1BA8 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B752()
        {
            AssertCode("@@@", "B752");
        }
        // Reko: a decoder for the instruction 0554 at address 1BB0 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0554()
        {
            AssertCode("@@@", "0554");
        }
        // Reko: a decoder for the instruction 975A at address 1BD8 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_975A()
        {
            AssertCode("@@@", "975A");
        }
        // Reko: a decoder for the instruction B75A at address 1BE0 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B75A()
        {
            AssertCode("@@@", "B75A");
        }
        // Reko: a decoder for the instruction 729B at address 1BF2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_729B()
        {
            AssertCode("@@@", "729B");
        }
        // Reko: a decoder for the instruction 7233 at address 1BF6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7233()
        {
            AssertCode("@@@", "7233");
        }
        // Reko: a decoder for the instruction 08EC at address 1C00 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08EC()
        {
            AssertCode("@@@", "08EC");
        }
        // Reko: a decoder for the instruction 0C54 at address 1C02 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0C54()
        {
            AssertCode("@@@", "0C54");
        }
        // Reko: a decoder for the instruction C8EF at address 1C26 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C8EF()
        {
            AssertCode("@@@", "C8EF");
        }
        // Reko: a decoder for the instruction 855A at address 1C4C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_855A()
        {
            AssertCode("@@@", "855A");
        }
        // Reko: a decoder for the instruction B65A at address 1C50 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B65A()
        {
            AssertCode("@@@", "B65A");
        }
        // Reko: a decoder for the instruction 189B at address 1C68 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_189B()
        {
            AssertCode("@@@", "189B");
        }
        // Reko: a decoder for the instruction 18DA at address 1C6E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_18DA()
        {
            AssertCode("@@@", "18DA");
        }
        // Reko: a decoder for the instruction 589F at address 1CC8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_589F()
        {
            AssertCode("@@@", "589F");
        }
        // Reko: a decoder for the instruction 38AF at address 1CD0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_38AF()
        {
            AssertCode("@@@", "38AF");
        }
        // Reko: a decoder for the instruction 053B at address 1CF0 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_053B()
        {
            AssertCode("@@@", "053B");
        }
        // Reko: a decoder for the instruction 58DF at address 1CF2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_58DF()
        {
            AssertCode("@@@", "58DF");
        }
        // Reko: a decoder for the instruction 38EF at address 1CFE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_38EF()
        {
            AssertCode("@@@", "38EF");
        }
        // Reko: a decoder for the instruction BA54 at address 1D0E has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BA54()
        {
            AssertCode("@@@", "BA54");
        }
        // Reko: a decoder for the instruction 0C52 at address 1D1E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0C52()
        {
            AssertCode("@@@", "0C52");
        }
        // Reko: a decoder for the instruction 4C52 at address 1D22 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4C52()
        {
            AssertCode("@@@", "4C52");
        }
        // Reko: a decoder for the instruction 140025B02897 at address 1D8A has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140025B02897()
        {
            AssertCode("@@@", "140025B02897");
        }
        // Reko: a decoder for the instruction D89A at address 1DCA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D89A()
        {
            AssertCode("@@@", "D89A");
        }
        // Reko: a decoder for the instruction D8DF at address 1DCC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D8DF()
        {
            AssertCode("@@@", "D8DF");
        }
        // Reko: a decoder for the instruction B054 at address 1F26 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B054()
        {
            AssertCode("@@@", "B054");
        }
        // Reko: a decoder for the instruction 115A at address 1F2E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_115A()
        {
            AssertCode("@@@", "115A");
        }
        // Reko: a decoder for the instruction 0C5A at address 1F34 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0C5A()
        {
            AssertCode("@@@", "0C5A");
        }
        // Reko: a decoder for the instruction A053 at address 1F38 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_A053()
        {
            AssertCode("@@@", "A053");
        }
        // Reko: a decoder for the instruction B8DF at address 1F58 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_B8DF()
        {
            AssertCode("@@@", "B8DF");
        }
        // Reko: a decoder for the instruction B133 at address 1F62 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B133()
        {
            AssertCode("@@@", "B133");
        }
        // Reko: a decoder for the instruction 18D4 at address 1F64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_18D4()
        {
            AssertCode("@@@", "18D4");
        }
        // Reko: a decoder for the instruction BA3B at address 1FB0 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_BA3B()
        {
            AssertCode("@@@", "BA3B");
        }
        // Reko: a decoder for the instruction 1A53 at address 1FB4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1A53()
        {
            AssertCode("@@@", "1A53");
        }
        // Reko: a decoder for the instruction 2FA0 at address 2022 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FA0()
        {
            AssertCode("@@@", "2FA0");
        }
        // Reko: a decoder for the instruction FBFE at address 203E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_FBFE()
        {
            AssertCode("@@@", "FBFE");
        }
        // Reko: a decoder for the instruction 97FE at address 208E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_97FE()
        {
            AssertCode("@@@", "97FE");
        }
        // Reko: a decoder for the instruction F061 at address 20A2 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_F061()
        {
            AssertCode("@@@", "F061");
        }
        // Reko: a decoder for the instruction 20A0 at address 20A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20A0()
        {
            AssertCode("@@@", "20A0");
        }
        // Reko: a decoder for the instruction 7C9A at address 20BE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7C9A()
        {
            AssertCode("@@@", "7C9A");
        }
        // Reko: a decoder for the instruction 4FA0 at address 20C6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FA0()
        {
            AssertCode("@@@", "4FA0");
        }
        // Reko: a decoder for the instruction 2CAC at address 20D0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CAC()
        {
            AssertCode("@@@", "2CAC");
        }
        // Reko: a decoder for the instruction 0452 at address 20D2 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0452()
        {
            AssertCode("@@@", "0452");
        }
        // Reko: a decoder for the instruction 0CAF at address 20D6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0CAF()
        {
            AssertCode("@@@", "0CAF");
        }
        // Reko: a decoder for the instruction AD61 at address 2122 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_AD61()
        {
            AssertCode("@@@", "AD61");
        }
        // Reko: a decoder for the instruction 4C9F at address 2124 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4C9F()
        {
            AssertCode("@@@", "4C9F");
        }
        // Reko: a decoder for the instruction 2CEF at address 2130 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2CEF()
        {
            AssertCode("@@@", "2CEF");
        }
        // Reko: a decoder for the instruction DCEF at address 2168 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DCEF()
        {
            AssertCode("@@@", "DCEF");
        }
        // Reko: a decoder for the instruction AFA0 at address 2172 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFA0()
        {
            AssertCode("@@@", "AFA0");
        }
        // Reko: a decoder for the instruction 1CDF at address 2180 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1CDF()
        {
            AssertCode("@@@", "1CDF");
        }
        // Reko: a decoder for the instruction 5C9F at address 2196 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5C9F()
        {
            AssertCode("@@@", "5C9F");
        }
        // Reko: a decoder for the instruction 3CAF at address 219E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3CAF()
        {
            AssertCode("@@@", "3CAF");
        }
        // Reko: a decoder for the instruction 5CDF at address 21C0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5CDF()
        {
            AssertCode("@@@", "5CDF");
        }
        // Reko: a decoder for the instruction 3CEF at address 21CC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3CEF()
        {
            AssertCode("@@@", "3CEF");
        }
        // Reko: a decoder for the instruction 3CA2 at address 2240 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3CA2()
        {
            AssertCode("@@@", "3CA2");
        }
        // Reko: a decoder for the instruction 3CE2 at address 2244 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3CE2()
        {
            AssertCode("@@@", "3CE2");
        }
        // Reko: a decoder for the instruction 140005B02C97 at address 2262 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140005B02C97()
        {
            AssertCode("@@@", "140005B02C97");
        }
        // Reko: a decoder for the instruction D3FE at address 2298 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_D3FE()
        {
            AssertCode("@@@", "D3FE");
        }
        // Reko: a decoder for the instruction 7CDF at address 22A4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7CDF()
        {
            AssertCode("@@@", "7CDF");
        }
        // Reko: a decoder for the instruction 0CEF at address 22AC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0CEF()
        {
            AssertCode("@@@", "0CEF");
        }
        // Reko: a decoder for the instruction 1FE0 at address 22CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FE0()
        {
            AssertCode("@@@", "1FE0");
        }
        // Reko: a decoder for the instruction 729F at address 22F0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_729F()
        {
            AssertCode("@@@", "729F");
        }
        // Reko: a decoder for the instruction DCA0 at address 233C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DCA0()
        {
            AssertCode("@@@", "DCA0");
        }
        // Reko: a decoder for the instruction 4FA2 at address 238E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FA2()
        {
            AssertCode("@@@", "4FA2");
        }
        // Reko: a decoder for the instruction 689A at address 23CC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_689A()
        {
            AssertCode("@@@", "689A");
        }
        // Reko: a decoder for the instruction 70F0 at address 23F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_70F0()
        {
            AssertCode("@@@", "70F0");
        }
        // Reko: a decoder for the instruction 7023 at address 240A has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7023()
        {
            AssertCode("@@@", "7023");
        }
        // Reko: a decoder for the instruction 7FF0 at address 2412 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FF0()
        {
            AssertCode("@@@", "7FF0");
        }
        // Reko: a decoder for the instruction 1054 at address 2414 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1054()
        {
            AssertCode("@@@", "1054");
        }
        // Reko: a decoder for the instruction 4454 at address 2418 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4454()
        {
            AssertCode("@@@", "4454");
        }
        // Reko: a decoder for the instruction F461 at address 241A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_F461()
        {
            AssertCode("@@@", "F461");
        }
        // Reko: a decoder for the instruction 02DF at address 243C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_02DF()
        {
            AssertCode("@@@", "02DF");
        }
        // Reko: a decoder for the instruction 429F at address 2440 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_429F()
        {
            AssertCode("@@@", "429F");
        }
        // Reko: a decoder for the instruction 0D9A at address 2482 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D9A()
        {
            AssertCode("@@@", "0D9A");
        }
        // Reko: a decoder for the instruction 0D9F at address 2488 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0D9F()
        {
            AssertCode("@@@", "0D9F");
        }
        // Reko: a decoder for the instruction 7D9A at address 2492 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7D9A()
        {
            AssertCode("@@@", "7D9A");
        }
        // Reko: a decoder for the instruction 2DAC at address 24A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DAC()
        {
            AssertCode("@@@", "2DAC");
        }
        // Reko: a decoder for the instruction 0DAF at address 24AC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0DAF()
        {
            AssertCode("@@@", "0DAF");
        }
        // Reko: a decoder for the instruction 0DDF at address 24DA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0DDF()
        {
            AssertCode("@@@", "0DDF");
        }
        // Reko: a decoder for the instruction 0D94 at address 24DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D94()
        {
            AssertCode("@@@", "0D94");
        }
        // Reko: a decoder for the instruction 7DD4 at address 24E2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7DD4()
        {
            AssertCode("@@@", "7DD4");
        }
        // Reko: a decoder for the instruction ADEF at address 2518 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ADEF()
        {
            AssertCode("@@@", "ADEF");
        }
        // Reko: a decoder for the instruction DC61 at address 2520 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_DC61()
        {
            AssertCode("@@@", "DC61");
        }
        // Reko: a decoder for the instruction 1DDF at address 2530 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1DDF()
        {
            AssertCode("@@@", "1DDF");
        }
        // Reko: a decoder for the instruction 0DD4 at address 2538 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DD4()
        {
            AssertCode("@@@", "0DD4");
        }
        // Reko: a decoder for the instruction 5D9F at address 2546 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5D9F()
        {
            AssertCode("@@@", "5D9F");
        }
        // Reko: a decoder for the instruction 3DAF at address 254E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3DAF()
        {
            AssertCode("@@@", "3DAF");
        }
        // Reko: a decoder for the instruction 2D97 at address 2560 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2D97()
        {
            AssertCode("@@@", "2D97");
        }
        // Reko: a decoder for the instruction 5DDF at address 2570 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5DDF()
        {
            AssertCode("@@@", "5DDF");
        }
        // Reko: a decoder for the instruction 3DEF at address 257C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3DEF()
        {
            AssertCode("@@@", "3DEF");
        }
        // Reko: a decoder for the instruction 3DA2 at address 25F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DA2()
        {
            AssertCode("@@@", "3DA2");
        }
        // Reko: a decoder for the instruction 3DE2 at address 25F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DE2()
        {
            AssertCode("@@@", "3DE2");
        }
        // Reko: a decoder for the instruction 140005B02D97 at address 2612 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140005B02D97()
        {
            AssertCode("@@@", "140005B02D97");
        }
        // Reko: a decoder for the instruction 2DEF at address 2636 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2DEF()
        {
            AssertCode("@@@", "2DEF");
        }
        // Reko: a decoder for the instruction 75FE at address 263E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_75FE()
        {
            AssertCode("@@@", "75FE");
        }
        // Reko: a decoder for the instruction C1FE at address 264A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_C1FE()
        {
            AssertCode("@@@", "C1FE");
        }
        // Reko: a decoder for the instruction 4D9F at address 264E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4D9F()
        {
            AssertCode("@@@", "4D9F");
        }
        // Reko: a decoder for the instruction 51FE at address 2656 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_51FE()
        {
            AssertCode("@@@", "51FE");
        }
        // Reko: a decoder for the instruction 7D9F at address 2658 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7D9F()
        {
            AssertCode("@@@", "7D9F");
        }
        // Reko: a decoder for the instruction 7DDF at address 2662 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7DDF()
        {
            AssertCode("@@@", "7DDF");
        }
        // Reko: a decoder for the instruction 0DEF at address 266A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0DEF()
        {
            AssertCode("@@@", "0DEF");
        }
        // Reko: a decoder for the instruction 0DC3 at address 2672 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0DC3()
        {
            AssertCode("@@@", "0DC3");
        }
        // Reko: a decoder for the instruction 11FE at address 2684 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_11FE()
        {
            AssertCode("@@@", "11FE");
        }
        // Reko: a decoder for the instruction 83FE at address 268A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_83FE()
        {
            AssertCode("@@@", "83FE");
        }
        // Reko: a decoder for the instruction 5DFE at address 269C has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_5DFE()
        {
            AssertCode("@@@", "5DFE");
        }
        // Reko: a decoder for the instruction 6DFE at address 26AE has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_6DFE()
        {
            AssertCode("@@@", "6DFE");
        }
        // Reko: a decoder for the instruction F8FF at address 26B6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F8FF()
        {
            AssertCode("@@@", "F8FF");
        }
        // Reko: a decoder for the instruction CCA0 at address 26C2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CCA0()
        {
            AssertCode("@@@", "CCA0");
        }
        // Reko: a decoder for the instruction CFE2 at address 26C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CFE2()
        {
            AssertCode("@@@", "CFE2");
        }
        // Reko: a decoder for the instruction 0D54 at address 26C6 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0D54()
        {
            AssertCode("@@@", "0D54");
        }
        // Reko: a decoder for the instruction 0FD0 at address 26DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD0()
        {
            AssertCode("@@@", "0FD0");
        }
        // Reko: a decoder for the instruction 0C61 at address 2708 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0C61()
        {
            AssertCode("@@@", "0C61");
        }
        // Reko: a decoder for the instruction 9732 at address 270A has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9732()
        {
            AssertCode("@@@", "9732");
        }
        // Reko: a decoder for the instruction 0282 at address 2710 has not been implemented. (Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_0282()
        {
            AssertCode("@@@", "0282");
        }
        // Reko: a decoder for the instruction 0A53 at address 2728 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0A53()
        {
            AssertCode("@@@", "0A53");
        }
        // Reko: a decoder for the instruction 0CB0 at address 2734 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB0()
        {
            AssertCode("@@@", "0CB0");
        }
        // Reko: a decoder for the instruction B89F at address 273A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_B89F()
        {
            AssertCode("@@@", "B89F");
        }
        // Reko: a decoder for the instruction 7B33 at address 273E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7B33()
        {
            AssertCode("@@@", "7B33");
        }
        // Reko: a decoder for the instruction BA53 at address 274A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_BA53()
        {
            AssertCode("@@@", "BA53");
        }
        // Reko: a decoder for the instruction AB3B at address 275C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_AB3B()
        {
            AssertCode("@@@", "AB3B");
        }
        // Reko: a decoder for the instruction 489A at address 2762 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_489A()
        {
            AssertCode("@@@", "489A");
        }
        // Reko: a decoder for the instruction 2461 at address 2766 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2461()
        {
            AssertCode("@@@", "2461");
        }
        // Reko: a decoder for the instruction D8EF at address 27FA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D8EF()
        {
            AssertCode("@@@", "D8EF");
        }
        // Reko: a decoder for the instruction 789B at address 283C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_789B()
        {
            AssertCode("@@@", "789B");
        }
        // Reko: a decoder for the instruction 93FE at address 285A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_93FE()
        {
            AssertCode("@@@", "93FE");
        }
        // Reko: a decoder for the instruction 8C60 at address 2888 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8C60()
        {
            AssertCode("@@@", "8C60");
        }
        // Reko: a decoder for the instruction CFE4 at address 288A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CFE4()
        {
            AssertCode("@@@", "CFE4");
        }
        // Reko: a decoder for the instruction 2A61 at address 28D0 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2A61()
        {
            AssertCode("@@@", "2A61");
        }
        // Reko: a decoder for the instruction 0082 at address 28DA has not been implemented. (Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_0082()
        {
            AssertCode("@@@", "0082");
        }
        // Reko: a decoder for the instruction 4FA4 at address 28E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FA4()
        {
            AssertCode("@@@", "4FA4");
        }
        // Reko: a decoder for the instruction C89A at address 28F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C89A()
        {
            AssertCode("@@@", "C89A");
        }
        // Reko: a decoder for the instruction 0C53 at address 28F4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0C53()
        {
            AssertCode("@@@", "0C53");
        }
        // Reko: a decoder for the instruction B018 at address 28F6 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_B018()
        {
            AssertCode("@@@", "B018");
        }
        // Reko: a decoder for the instruction 9232 at address 28FC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9232()
        {
            AssertCode("@@@", "9232");
        }
        // Reko: a decoder for the instruction A89F at address 290A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_A89F()
        {
            AssertCode("@@@", "A89F");
        }
        // Reko: a decoder for the instruction 7A33 at address 290E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7A33()
        {
            AssertCode("@@@", "7A33");
        }
        // Reko: a decoder for the instruction AC53 at address 291A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_AC53()
        {
            AssertCode("@@@", "AC53");
        }
        // Reko: a decoder for the instruction C8DF at address 291E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C8DF()
        {
            AssertCode("@@@", "C8DF");
        }
        // Reko: a decoder for the instruction 1A3B at address 2930 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1A3B()
        {
            AssertCode("@@@", "1A3B");
        }
        // Reko: a decoder for the instruction 0B52 at address 2960 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0B52()
        {
            AssertCode("@@@", "0B52");
        }
        // Reko: a decoder for the instruction 8C61 at address 2968 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8C61()
        {
            AssertCode("@@@", "8C61");
        }
        // Reko: a decoder for the instruction B053 at address 2972 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_B053()
        {
            AssertCode("@@@", "B053");
        }
        // Reko: a decoder for the instruction D408 at address 2974 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_D408()
        {
            AssertCode("@@@", "D408");
        }
        // Reko: a decoder for the instruction BA5A at address 29B8 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BA5A()
        {
            AssertCode("@@@", "BA5A");
        }
        // Reko: a decoder for the instruction 7B3B at address 29EA has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7B3B()
        {
            AssertCode("@@@", "7B3B");
        }
        // Reko: a decoder for the instruction CBFE at address 29FC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_CBFE()
        {
            AssertCode("@@@", "CBFE");
        }
        // Reko: a decoder for the instruction C7FE at address 2A08 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_C7FE()
        {
            AssertCode("@@@", "C7FE");
        }
        // Reko: a decoder for the instruction 38A2 at address 2A26 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_38A2()
        {
            AssertCode("@@@", "38A2");
        }
        // Reko: a decoder for the instruction 38E2 at address 2A2A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_38E2()
        {
            AssertCode("@@@", "38E2");
        }
        // Reko: a decoder for the instruction 140005B02897 at address 2A46 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140005B02897()
        {
            AssertCode("@@@", "140005B02897");
        }
        // Reko: a decoder for the instruction DDFE at address 2A96 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_DDFE()
        {
            AssertCode("@@@", "DDFE");
        }
        // Reko: a decoder for the instruction C7FD at address 2AEC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C7FD()
        {
            AssertCode("@@@", "C7FD");
        }
        // Reko: a decoder for the instruction 4FD1 at address 2B22 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FD1()
        {
            AssertCode("@@@", "4FD1");
        }
        // Reko: a decoder for the instruction 4452 at address 2B40 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4452()
        {
            AssertCode("@@@", "4452");
        }
        // Reko: a decoder for the instruction A018 at address 2B42 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_A018()
        {
            AssertCode("@@@", "A018");
        }
        // Reko: a decoder for the instruction B29F at address 2B54 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_B29F()
        {
            AssertCode("@@@", "B29F");
        }
        // Reko: a decoder for the instruction 4F91 at address 2BF2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F91()
        {
            AssertCode("@@@", "4F91");
        }
        // Reko: a decoder for the instruction 545A at address 2D18 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_545A()
        {
            AssertCode("@@@", "545A");
        }
        // Reko: a decoder for the instruction 5053 at address 2D60 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_5053()
        {
            AssertCode("@@@", "5053");
        }
        // Reko: a decoder for the instruction 0FC2 at address 2D9A has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_0FC2()
        {
            AssertCode("@@@", "0FC2");
        }
        // Reko: a decoder for the instruction 445A at address 2DCC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_445A()
        {
            AssertCode("@@@", "445A");
        }
        // Reko: a decoder for the instruction 02C3 at address 2ED2 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_02C3()
        {
            AssertCode("@@@", "02C3");
        }
        // Reko: a decoder for the instruction B7FE at address 2EF6 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_B7FE()
        {
            AssertCode("@@@", "B7FE");
        }
        // Reko: a decoder for the instruction ABFE at address 2F02 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_ABFE()
        {
            AssertCode("@@@", "ABFE");
        }
        // Reko: a decoder for the instruction ADFE at address 2F22 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_ADFE()
        {
            AssertCode("@@@", "ADFE");
        }
        // Reko: a decoder for the instruction E8FF at address 2F4E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E8FF()
        {
            AssertCode("@@@", "E8FF");
        }
        // Reko: a decoder for the instruction 2561 at address 2F62 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2561()
        {
            AssertCode("@@@", "2561");
        }
        // Reko: a decoder for the instruction 5FE0 at address 2F64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FE0()
        {
            AssertCode("@@@", "5FE0");
        }
        // Reko: a decoder for the instruction 9A9F at address 2F66 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_9A9F()
        {
            AssertCode("@@@", "9A9F");
        }
        // Reko: a decoder for the instruction B85A at address 2F76 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B85A()
        {
            AssertCode("@@@", "B85A");
        }
        // Reko: a decoder for the instruction FAFE at address 2F78 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_FAFE()
        {
            AssertCode("@@@", "FAFE");
        }
        // Reko: a decoder for the instruction 6833 at address 2F7A has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6833()
        {
            AssertCode("@@@", "6833");
        }
        // Reko: a decoder for the instruction 055A at address 2F7C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_055A()
        {
            AssertCode("@@@", "055A");
        }
        // Reko: a decoder for the instruction 7533 at address 2F86 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7533()
        {
            AssertCode("@@@", "7533");
        }
        // Reko: a decoder for the instruction 653B at address 2F88 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_653B()
        {
            AssertCode("@@@", "653B");
        }
        // Reko: a decoder for the instruction 7AAF at address 2F8A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7AAF()
        {
            AssertCode("@@@", "7AAF");
        }
        // Reko: a decoder for the instruction 6A9F at address 2F8E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6A9F()
        {
            AssertCode("@@@", "6A9F");
        }
        // Reko: a decoder for the instruction 6FD4 at address 2F92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FD4()
        {
            AssertCode("@@@", "6FD4");
        }
        // Reko: a decoder for the instruction B260 at address 2F94 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B260()
        {
            AssertCode("@@@", "B260");
        }
        // Reko: a decoder for the instruction CFE6 at address 2F9C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CFE6()
        {
            AssertCode("@@@", "CFE6");
        }
        // Reko: a decoder for the instruction CFA0 at address 2FA0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CFA0()
        {
            AssertCode("@@@", "CFA0");
        }
        // Reko: a decoder for the instruction 9E54 at address 2FA4 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9E54()
        {
            AssertCode("@@@", "9E54");
        }
        // Reko: a decoder for the instruction 2E61 at address 2FA6 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2E61()
        {
            AssertCode("@@@", "2E61");
        }
        // Reko: a decoder for the instruction EEB0 at address 2FA8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EEB0()
        {
            AssertCode("@@@", "EEB0");
        }
        // Reko: a decoder for the instruction EFF4 at address 2FAA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EFF4()
        {
            AssertCode("@@@", "EFF4");
        }
        // Reko: a decoder for the instruction 62B0 at address 2FAC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_62B0()
        {
            AssertCode("@@@", "62B0");
        }
        // Reko: a decoder for the instruction 2A9F at address 2FAE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2A9F()
        {
            AssertCode("@@@", "2A9F");
        }
        // Reko: a decoder for the instruction 3253 at address 2FC0 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_3253()
        {
            AssertCode("@@@", "3253");
        }
        // Reko: a decoder for the instruction 2FD5 at address 2FC4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FD5()
        {
            AssertCode("@@@", "2FD5");
        }
        // Reko: a decoder for the instruction AFE8 at address 2FCE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFE8()
        {
            AssertCode("@@@", "AFE8");
        }
        // Reko: a decoder for the instruction BF94 at address 2FD0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BF94()
        {
            AssertCode("@@@", "BF94");
        }
        // Reko: a decoder for the instruction 9532 at address 2FD4 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9532()
        {
            AssertCode("@@@", "9532");
        }
        // Reko: a decoder for the instruction D061 at address 2FDC has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D061()
        {
            AssertCode("@@@", "D061");
        }
        // Reko: a decoder for the instruction 92B0 at address 2FE2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_92B0()
        {
            AssertCode("@@@", "92B0");
        }
        // Reko: a decoder for the instruction C951 at address 2FE4 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_C951()
        {
            AssertCode("@@@", "C951");
        }
        // Reko: a decoder for the instruction 22B0 at address 2FEA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_22B0()
        {
            AssertCode("@@@", "22B0");
        }
        // Reko: a decoder for the instruction 9FB4 at address 2FEC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FB4()
        {
            AssertCode("@@@", "9FB4");
        }
        // Reko: a decoder for the instruction 9251 at address 2FEE has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9251()
        {
            AssertCode("@@@", "9251");
        }
        // Reko: a decoder for the instruction 30B0 at address 2FF2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_30B0()
        {
            AssertCode("@@@", "30B0");
        }
        // Reko: a decoder for the instruction 9FA0 at address 2FF4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FA0()
        {
            AssertCode("@@@", "9FA0");
        }
        // Reko: a decoder for the instruction 29B0 at address 2FF6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_29B0()
        {
            AssertCode("@@@", "29B0");
        }
        // Reko: a decoder for the instruction 2351 at address 2FF8 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2351()
        {
            AssertCode("@@@", "2351");
        }
        // Reko: a decoder for the instruction 30B1 at address 2FFC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_30B1()
        {
            AssertCode("@@@", "30B1");
        }
        // Reko: a decoder for the instruction 29B1 at address 2FFE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_29B1()
        {
            AssertCode("@@@", "29B1");
        }
        // Reko: a decoder for the instruction B423 at address 3004 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B423()
        {
            AssertCode("@@@", "B423");
        }
        // Reko: a decoder for the instruction 0061 at address 3008 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0061()
        {
            AssertCode("@@@", "0061");
        }
        // Reko: a decoder for the instruction 7061 at address 300A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_7061()
        {
            AssertCode("@@@", "7061");
        }
        // Reko: a decoder for the instruction 4090 at address 300C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4090()
        {
            AssertCode("@@@", "4090");
        }
        // Reko: a decoder for the instruction 4E53 at address 300E has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_4E53()
        {
            AssertCode("@@@", "4E53");
        }
        // Reko: a decoder for the instruction 9F93 at address 3014 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9F93()
        {
            AssertCode("@@@", "9F93");
        }
        // Reko: a decoder for the instruction 9053 at address 3016 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9053()
        {
            AssertCode("@@@", "9053");
        }
        // Reko: a decoder for the instruction 2260 at address 3026 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2260()
        {
            AssertCode("@@@", "2260");
        }
        // Reko: a decoder for the instruction 2060 at address 3028 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2060()
        {
            AssertCode("@@@", "2060");
        }
        // Reko: a decoder for the instruction BFD4 at address 302A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFD4()
        {
            AssertCode("@@@", "BFD4");
        }
        // Reko: a decoder for the instruction 4FDA at address 302E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FDA()
        {
            AssertCode("@@@", "4FDA");
        }
        // Reko: a decoder for the instruction 42B2 at address 3032 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B2()
        {
            AssertCode("@@@", "42B2");
        }
        // Reko: a decoder for the instruction 90B2 at address 3034 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B2()
        {
            AssertCode("@@@", "90B2");
        }
        // Reko: a decoder for the instruction 9451 at address 3036 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9451()
        {
            AssertCode("@@@", "9451");
        }
        // Reko: a decoder for the instruction 42B3 at address 303A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B3()
        {
            AssertCode("@@@", "42B3");
        }
        // Reko: a decoder for the instruction 90B3 at address 303C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B3()
        {
            AssertCode("@@@", "90B3");
        }
        // Reko: a decoder for the instruction 42B4 at address 3042 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B4()
        {
            AssertCode("@@@", "42B4");
        }
        // Reko: a decoder for the instruction 90B4 at address 3044 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B4()
        {
            AssertCode("@@@", "90B4");
        }
        // Reko: a decoder for the instruction 42B5 at address 304A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B5()
        {
            AssertCode("@@@", "42B5");
        }
        // Reko: a decoder for the instruction 90B5 at address 304C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B5()
        {
            AssertCode("@@@", "90B5");
        }
        // Reko: a decoder for the instruction 42B6 at address 3052 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B6()
        {
            AssertCode("@@@", "42B6");
        }
        // Reko: a decoder for the instruction 90B6 at address 3054 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B6()
        {
            AssertCode("@@@", "90B6");
        }
        // Reko: a decoder for the instruction 42B7 at address 305A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B7()
        {
            AssertCode("@@@", "42B7");
        }
        // Reko: a decoder for the instruction 90B7 at address 305C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B7()
        {
            AssertCode("@@@", "90B7");
        }
        // Reko: a decoder for the instruction 8260 at address 3062 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8260()
        {
            AssertCode("@@@", "8260");
        }
        // Reko: a decoder for the instruction 8060 at address 3064 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8060()
        {
            AssertCode("@@@", "8060");
        }
        // Reko: a decoder for the instruction 90B0 at address 3068 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B0()
        {
            AssertCode("@@@", "90B0");
        }
        // Reko: a decoder for the instruction 0408 at address 306C has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0408()
        {
            AssertCode("@@@", "0408");
        }
        // Reko: a decoder for the instruction A908 at address 3070 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_A908()
        {
            AssertCode("@@@", "A908");
        }
        // Reko: a decoder for the instruction 4921 at address 3072 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4921()
        {
            AssertCode("@@@", "4921");
        }
        // Reko: a decoder for the instruction 42B1 at address 3078 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42B1()
        {
            AssertCode("@@@", "42B1");
        }
        // Reko: a decoder for the instruction 90B1 at address 307A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90B1()
        {
            AssertCode("@@@", "90B1");
        }
        // Reko: a decoder for the instruction 4F9A at address 3082 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F9A()
        {
            AssertCode("@@@", "4F9A");
        }
        // Reko: a decoder for the instruction 1260 at address 3084 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1260()
        {
            AssertCode("@@@", "1260");
        }
        // Reko: a decoder for the instruction 093B at address 3090 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_093B()
        {
            AssertCode("@@@", "093B");
        }
        // Reko: a decoder for the instruction 9653 at address 3092 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9653()
        {
            AssertCode("@@@", "9653");
        }
        // Reko: a decoder for the instruction 0FA8 at address 3096 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FA8()
        {
            AssertCode("@@@", "0FA8");
        }
        // Reko: a decoder for the instruction 40DF at address 3098 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_40DF()
        {
            AssertCode("@@@", "40DF");
        }
        // Reko: a decoder for the instruction 9054 at address 30A8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9054()
        {
            AssertCode("@@@", "9054");
        }
        // Reko: a decoder for the instruction 60B0 at address 30AC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60B0()
        {
            AssertCode("@@@", "60B0");
        }
        // Reko: a decoder for the instruction 6FF4 at address 30AE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FF4()
        {
            AssertCode("@@@", "6FF4");
        }
        // Reko: a decoder for the instruction C2B0 at address 30B0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C2B0()
        {
            AssertCode("@@@", "C2B0");
        }
        // Reko: a decoder for the instruction 4260 at address 30CA has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4260()
        {
            AssertCode("@@@", "4260");
        }
        // Reko: a decoder for the instruction 5260 at address 30D2 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5260()
        {
            AssertCode("@@@", "5260");
        }
        // Reko: a decoder for the instruction 6260 at address 30DA has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6260()
        {
            AssertCode("@@@", "6260");
        }
        // Reko: a decoder for the instruction 7260 at address 30E2 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7260()
        {
            AssertCode("@@@", "7260");
        }
        // Reko: a decoder for the instruction 7FD1 at address 3100 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FD1()
        {
            AssertCode("@@@", "7FD1");
        }
        // Reko: a decoder for the instruction 1FE4 at address 3110 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FE4()
        {
            AssertCode("@@@", "1FE4");
        }
        // Reko: a decoder for the instruction 3D9F at address 3112 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3D9F()
        {
            AssertCode("@@@", "3D9F");
        }
        // Reko: a decoder for the instruction 3A3B at address 311C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3A3B()
        {
            AssertCode("@@@", "3A3B");
        }
        // Reko: a decoder for the instruction 3B27 at address 3122 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3B27()
        {
            AssertCode("@@@", "3B27");
        }
        // Reko: a decoder for the instruction AF91 at address 3128 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AF91()
        {
            AssertCode("@@@", "AF91");
        }
        // Reko: a decoder for the instruction 7F92 at address 312A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F92()
        {
            AssertCode("@@@", "7F92");
        }
        // Reko: a decoder for the instruction 0353 at address 312E has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0353()
        {
            AssertCode("@@@", "0353");
        }
        // Reko: a decoder for the instruction 8DA0 at address 3132 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8DA0()
        {
            AssertCode("@@@", "8DA0");
        }
        // Reko: a decoder for the instruction 7892 at address 3134 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7892()
        {
            AssertCode("@@@", "7892");
        }
        // Reko: a decoder for the instruction BD9F at address 313C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BD9F()
        {
            AssertCode("@@@", "BD9F");
        }
        // Reko: a decoder for the instruction AB33 at address 314C has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_AB33()
        {
            AssertCode("@@@", "AB33");
        }
        // Reko: a decoder for the instruction BDDF at address 314E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BDDF()
        {
            AssertCode("@@@", "BDDF");
        }
        // Reko: a decoder for the instruction BB52 at address 3160 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BB52()
        {
            AssertCode("@@@", "BB52");
        }
        // Reko: a decoder for the instruction 0DA0 at address 3166 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DA0()
        {
            AssertCode("@@@", "0DA0");
        }
        // Reko: a decoder for the instruction 0092 at address 3168 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0092()
        {
            AssertCode("@@@", "0092");
        }
        // Reko: a decoder for the instruction 9A52 at address 3182 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9A52()
        {
            AssertCode("@@@", "9A52");
        }
        // Reko: a decoder for the instruction 2DAF at address 3192 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2DAF()
        {
            AssertCode("@@@", "2DAF");
        }
        // Reko: a decoder for the instruction A03B at address 3198 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A03B()
        {
            AssertCode("@@@", "A03B");
        }
        // Reko: a decoder for the instruction 133B at address 31B6 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_133B()
        {
            AssertCode("@@@", "133B");
        }
        // Reko: a decoder for the instruction 3DDF at address 31B8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3DDF()
        {
            AssertCode("@@@", "3DDF");
        }
        // Reko: a decoder for the instruction 4FA6 at address 31C0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FA6()
        {
            AssertCode("@@@", "4FA6");
        }
        // Reko: a decoder for the instruction 6DAF at address 31D6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6DAF()
        {
            AssertCode("@@@", "6DAF");
        }
        // Reko: a decoder for the instruction 6061 at address 31DA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_6061()
        {
            AssertCode("@@@", "6061");
        }
        // Reko: a decoder for the instruction 9432 at address 31DC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9432()
        {
            AssertCode("@@@", "9432");
        }
        // Reko: a decoder for the instruction 1460 at address 31E0 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1460()
        {
            AssertCode("@@@", "1460");
        }
        // Reko: a decoder for the instruction 4461 at address 31E2 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4461()
        {
            AssertCode("@@@", "4461");
        }
        // Reko: a decoder for the instruction B060 at address 31EE has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B060()
        {
            AssertCode("@@@", "B060");
        }
        // Reko: a decoder for the instruction 6090 at address 31F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6090()
        {
            AssertCode("@@@", "6090");
        }
        // Reko: a decoder for the instruction 6253 at address 31F4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6253()
        {
            AssertCode("@@@", "6253");
        }
        // Reko: a decoder for the instruction 263B at address 31FA has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_263B()
        {
            AssertCode("@@@", "263B");
        }
        // Reko: a decoder for the instruction 60D0 at address 31FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60D0()
        {
            AssertCode("@@@", "60D0");
        }
        // Reko: a decoder for the instruction 4DAF at address 3206 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4DAF()
        {
            AssertCode("@@@", "4DAF");
        }
        // Reko: a decoder for the instruction 2433 at address 320E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2433()
        {
            AssertCode("@@@", "2433");
        }
        // Reko: a decoder for the instruction 7F91 at address 3232 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F91()
        {
            AssertCode("@@@", "7F91");
        }
        // Reko: a decoder for the instruction ADAF at address 3244 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ADAF()
        {
            AssertCode("@@@", "ADAF");
        }
        // Reko: a decoder for the instruction 8D9F at address 324C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8D9F()
        {
            AssertCode("@@@", "8D9F");
        }
        // Reko: a decoder for the instruction 5018 at address 325C has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_5018()
        {
            AssertCode("@@@", "5018");
        }
        // Reko: a decoder for the instruction 8DEF at address 327C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8DEF()
        {
            AssertCode("@@@", "8DEF");
        }
        // Reko: a decoder for the instruction 4DDF at address 3298 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4DDF()
        {
            AssertCode("@@@", "4DDF");
        }
        // Reko: a decoder for the instruction 8233 at address 32A8 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8233()
        {
            AssertCode("@@@", "8233");
        }
        // Reko: a decoder for the instruction 02B0 at address 32AE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02B0()
        {
            AssertCode("@@@", "02B0");
        }
        // Reko: a decoder for the instruction A445 at address 32B2 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_A445()
        {
            AssertCode("@@@", "A445");
        }
        // Reko: a decoder for the instruction 402B at address 32B4 has not been implemented. (Fmt15 1 ZZ  xorw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_402B()
        {
            AssertCode("@@@", "402B");
        }
        // Reko: a decoder for the instruction 2023 at address 32B8 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2023()
        {
            AssertCode("@@@", "2023");
        }
        // Reko: a decoder for the instruction A1FE at address 32C2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_A1FE()
        {
            AssertCode("@@@", "A1FE");
        }
        // Reko: a decoder for the instruction 225A at address 32C4 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_225A()
        {
            AssertCode("@@@", "225A");
        }
        // Reko: a decoder for the instruction A045 at address 32D0 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_A045()
        {
            AssertCode("@@@", "A045");
        }
        // Reko: a decoder for the instruction 202B at address 32D2 has not been implemented. (Fmt15 1 ZZ  xorw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_202B()
        {
            AssertCode("@@@", "202B");
        }
        // Reko: a decoder for the instruction 4F90 at address 32D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F90()
        {
            AssertCode("@@@", "4F90");
        }
        // Reko: a decoder for the instruction 4023 at address 32D6 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4023()
        {
            AssertCode("@@@", "4023");
        }
        // Reko: a decoder for the instruction 9490 at address 32E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9490()
        {
            AssertCode("@@@", "9490");
        }
        // Reko: a decoder for the instruction 2D9F at address 32E8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2D9F()
        {
            AssertCode("@@@", "2D9F");
        }
        // Reko: a decoder for the instruction 8223 at address 32EC has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8223()
        {
            AssertCode("@@@", "8223");
        }
        // Reko: a decoder for the instruction 2261 at address 32F4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2261()
        {
            AssertCode("@@@", "2261");
        }
        // Reko: a decoder for the instruction 6261 at address 32F6 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_6261()
        {
            AssertCode("@@@", "6261");
        }
        // Reko: a decoder for the instruction 92D0 at address 32F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_92D0()
        {
            AssertCode("@@@", "92D0");
        }
        // Reko: a decoder for the instruction 84D0 at address 32FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_84D0()
        {
            AssertCode("@@@", "84D0");
        }
        // Reko: a decoder for the instruction 1832 at address 32FC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1832()
        {
            AssertCode("@@@", "1832");
        }
        // Reko: a decoder for the instruction 9132 at address 32FE has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9132()
        {
            AssertCode("@@@", "9132");
        }
        // Reko: a decoder for the instruction 1233 at address 3306 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1233()
        {
            AssertCode("@@@", "1233");
        }
        // Reko: a decoder for the instruction 2252 at address 3308 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2252()
        {
            AssertCode("@@@", "2252");
        }
        // Reko: a decoder for the instruction 55FE at address 330E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_55FE()
        {
            AssertCode("@@@", "55FE");
        }
        // Reko: a decoder for the instruction 28D2 at address 331E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28D2()
        {
            AssertCode("@@@", "28D2");
        }
        // Reko: a decoder for the instruction 48A0 at address 3322 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48A0()
        {
            AssertCode("@@@", "48A0");
        }
        // Reko: a decoder for the instruction 009C at address 3330 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_009C()
        {
            AssertCode("@@@", "009C");
        }
        // Reko: a decoder for the instruction 08A0 at address 333C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08A0()
        {
            AssertCode("@@@", "08A0");
        }
        // Reko: a decoder for the instruction C061 at address 333E has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_C061()
        {
            AssertCode("@@@", "C061");
        }
        // Reko: a decoder for the instruction 08E0 at address 3340 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08E0()
        {
            AssertCode("@@@", "08E0");
        }
        // Reko: a decoder for the instruction 08A4 at address 3342 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08A4()
        {
            AssertCode("@@@", "08A4");
        }
        // Reko: a decoder for the instruction C8E4 at address 3346 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C8E4()
        {
            AssertCode("@@@", "C8E4");
        }
        // Reko: a decoder for the instruction FFFD at address 3350 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FFFD()
        {
            AssertCode("@@@", "FFFD");
        }
        // Reko: a decoder for the instruction 00C2 at address 3352 has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_00C2()
        {
            AssertCode("@@@", "00C2");
        }
        // Reko: a decoder for the instruction D9FE at address 3360 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_D9FE()
        {
            AssertCode("@@@", "D9FE");
        }
        // Reko: a decoder for the instruction 89FE at address 3368 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_89FE()
        {
            AssertCode("@@@", "89FE");
        }
        // Reko: a decoder for the instruction 1400A5B00DAF at address 33B4 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A5B00DAF()
        {
            AssertCode("@@@", "1400A5B00DAF");
        }
        // Reko: a decoder for the instruction 95FE at address 33EE has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_95FE()
        {
            AssertCode("@@@", "95FE");
        }
        // Reko: a decoder for the instruction A23B at address 33F2 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A23B()
        {
            AssertCode("@@@", "A23B");
        }
        // Reko: a decoder for the instruction 4FD4 at address 3402 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FD4()
        {
            AssertCode("@@@", "4FD4");
        }
        // Reko: a decoder for the instruction 02A6 at address 3404 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02A6()
        {
            AssertCode("@@@", "02A6");
        }
        // Reko: a decoder for the instruction 229F at address 3406 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_229F()
        {
            AssertCode("@@@", "229F");
        }
        // Reko: a decoder for the instruction 0FD5 at address 3416 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD5()
        {
            AssertCode("@@@", "0FD5");
        }
        // Reko: a decoder for the instruction 7D92 at address 341A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7D92()
        {
            AssertCode("@@@", "7D92");
        }
        // Reko: a decoder for the instruction 7FD6 at address 341C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FD6()
        {
            AssertCode("@@@", "7FD6");
        }
        // Reko: a decoder for the instruction D043 at address 3428 has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_D043()
        {
            AssertCode("@@@", "D043");
        }
        // Reko: a decoder for the instruction 7D98 at address 342A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7D98()
        {
            AssertCode("@@@", "7D98");
        }
        // Reko: a decoder for the instruction 4018 at address 342E has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_4018()
        {
            AssertCode("@@@", "4018");
        }
        // Reko: a decoder for the instruction 083B at address 343C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_083B()
        {
            AssertCode("@@@", "083B");
        }
        // Reko: a decoder for the instruction 7853 at address 3456 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7853()
        {
            AssertCode("@@@", "7853");
        }
        // Reko: a decoder for the instruction 0F95 at address 345C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F95()
        {
            AssertCode("@@@", "0F95");
        }
        // Reko: a decoder for the instruction 2F94 at address 3468 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2F94()
        {
            AssertCode("@@@", "2F94");
        }
        // Reko: a decoder for the instruction 4252 at address 346A has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4252()
        {
            AssertCode("@@@", "4252");
        }
        // Reko: a decoder for the instruction 1021 at address 346E has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1021()
        {
            AssertCode("@@@", "1021");
        }
        // Reko: a decoder for the instruction 803B at address 3482 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_803B()
        {
            AssertCode("@@@", "803B");
        }
        // Reko: a decoder for the instruction 6F94 at address 3494 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6F94()
        {
            AssertCode("@@@", "6F94");
        }
        // Reko: a decoder for the instruction 0CA4 at address 349E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CA4()
        {
            AssertCode("@@@", "0CA4");
        }
        // Reko: a decoder for the instruction 2CAA at address 34A0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CAA()
        {
            AssertCode("@@@", "2CAA");
        }
        // Reko: a decoder for the instruction 20F0 at address 34B8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20F0()
        {
            AssertCode("@@@", "20F0");
        }
        // Reko: a decoder for the instruction 9228 at address 34C6 has not been implemented. (ZZ xorb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9228()
        {
            AssertCode("@@@", "9228");
        }
        // Reko: a decoder for the instruction 922A at address 34D4 has not been implemented. (ZZ xorw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_922A()
        {
            AssertCode("@@@", "922A");
        }
        // Reko: a decoder for the instruction ACA0 at address 34DA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACA0()
        {
            AssertCode("@@@", "ACA0");
        }
        // Reko: a decoder for the instruction DAAF at address 34DC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DAAF()
        {
            AssertCode("@@@", "DAAF");
        }
        // Reko: a decoder for the instruction 9A98 at address 34E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9A98()
        {
            AssertCode("@@@", "9A98");
        }
        // Reko: a decoder for the instruction 0953 at address 34EC has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0953()
        {
            AssertCode("@@@", "0953");
        }
        // Reko: a decoder for the instruction 0952 at address 34F2 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0952()
        {
            AssertCode("@@@", "0952");
        }
        // Reko: a decoder for the instruction 2AA6 at address 34FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AA6()
        {
            AssertCode("@@@", "2AA6");
        }
        // Reko: a decoder for the instruction 4DA8 at address 34FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DA8()
        {
            AssertCode("@@@", "4DA8");
        }
        // Reko: a decoder for the instruction 0AA6 at address 3506 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AA6()
        {
            AssertCode("@@@", "0AA6");
        }
        // Reko: a decoder for the instruction 0AE6 at address 350C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AE6()
        {
            AssertCode("@@@", "0AE6");
        }
        // Reko: a decoder for the instruction 0DA8 at address 350E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DA8()
        {
            AssertCode("@@@", "0DA8");
        }
        // Reko: a decoder for the instruction 0DE8 at address 3512 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DE8()
        {
            AssertCode("@@@", "0DE8");
        }
        // Reko: a decoder for the instruction 0AAA at address 3514 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AAA()
        {
            AssertCode("@@@", "0AAA");
        }
        // Reko: a decoder for the instruction 0AEA at address 3518 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AEA()
        {
            AssertCode("@@@", "0AEA");
        }
        // Reko: a decoder for the instruction 0A98 at address 351A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0A98()
        {
            AssertCode("@@@", "0A98");
        }
        // Reko: a decoder for the instruction 903B at address 351C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_903B()
        {
            AssertCode("@@@", "903B");
        }
        // Reko: a decoder for the instruction 0AD8 at address 351E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AD8()
        {
            AssertCode("@@@", "0AD8");
        }
        // Reko: a decoder for the instruction 0DAA at address 3520 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DAA()
        {
            AssertCode("@@@", "0DAA");
        }
        // Reko: a decoder for the instruction 0DA4 at address 352E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DA4()
        {
            AssertCode("@@@", "0DA4");
        }
        // Reko: a decoder for the instruction 2DA6 at address 353A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DA6()
        {
            AssertCode("@@@", "2DA6");
        }
        // Reko: a decoder for the instruction 4CAF at address 353C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4CAF()
        {
            AssertCode("@@@", "4CAF");
        }
        // Reko: a decoder for the instruction 0DA6 at address 354E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DA6()
        {
            AssertCode("@@@", "0DA6");
        }
        // Reko: a decoder for the instruction 0DE6 at address 3552 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DE6()
        {
            AssertCode("@@@", "0DE6");
        }
        // Reko: a decoder for the instruction 0D98 at address 3554 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D98()
        {
            AssertCode("@@@", "0D98");
        }
        // Reko: a decoder for the instruction 0DD8 at address 3558 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DD8()
        {
            AssertCode("@@@", "0DD8");
        }
        // Reko: a decoder for the instruction 0DEA at address 355E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DEA()
        {
            AssertCode("@@@", "0DEA");
        }
        // Reko: a decoder for the instruction AD92 at address 356E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AD92()
        {
            AssertCode("@@@", "AD92");
        }
        // Reko: a decoder for the instruction A9FE at address 357A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_A9FE()
        {
            AssertCode("@@@", "A9FE");
        }
        // Reko: a decoder for the instruction 7F96 at address 357C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F96()
        {
            AssertCode("@@@", "7F96");
        }
        // Reko: a decoder for the instruction A73B at address 357E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A73B()
        {
            AssertCode("@@@", "A73B");
        }
        // Reko: a decoder for the instruction 195A at address 3580 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_195A()
        {
            AssertCode("@@@", "195A");
        }
        // Reko: a decoder for the instruction 2CAF at address 358A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2CAF()
        {
            AssertCode("@@@", "2CAF");
        }
        // Reko: a decoder for the instruction E261 at address 35AA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_E261()
        {
            AssertCode("@@@", "E261");
        }
        // Reko: a decoder for the instruction 4DA0 at address 35AC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DA0()
        {
            AssertCode("@@@", "4DA0");
        }
        // Reko: a decoder for the instruction 305A at address 35E6 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_305A()
        {
            AssertCode("@@@", "305A");
        }
        // Reko: a decoder for the instruction 0FC3 at address 35F0 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0FC3()
        {
            AssertCode("@@@", "0FC3");
        }
        // Reko: a decoder for the instruction 3F94 at address 35F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F94()
        {
            AssertCode("@@@", "3F94");
        }
        // Reko: a decoder for the instruction 4352 at address 35F6 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4352()
        {
            AssertCode("@@@", "4352");
        }
        // Reko: a decoder for the instruction 0608 at address 3602 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0608()
        {
            AssertCode("@@@", "0608");
        }
        // Reko: a decoder for the instruction 6FD2 at address 3604 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FD2()
        {
            AssertCode("@@@", "6FD2");
        }
        // Reko: a decoder for the instruction 8BFE at address 3608 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_8BFE()
        {
            AssertCode("@@@", "8BFE");
        }
        // Reko: a decoder for the instruction 8DA6 at address 360A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8DA6()
        {
            AssertCode("@@@", "8DA6");
        }
        // Reko: a decoder for the instruction A753 at address 360C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_A753()
        {
            AssertCode("@@@", "A753");
        }
        // Reko: a decoder for the instruction 0861 at address 3618 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0861()
        {
            AssertCode("@@@", "0861");
        }
        // Reko: a decoder for the instruction 8DE6 at address 361A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8DE6()
        {
            AssertCode("@@@", "8DE6");
        }
        // Reko: a decoder for the instruction 2D98 at address 361C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2D98()
        {
            AssertCode("@@@", "2D98");
        }
        // Reko: a decoder for the instruction 2DD8 at address 3620 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DD8()
        {
            AssertCode("@@@", "2DD8");
        }
        // Reko: a decoder for the instruction 2DAA at address 3622 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DAA()
        {
            AssertCode("@@@", "2DAA");
        }
        // Reko: a decoder for the instruction 7FD0 at address 362A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FD0()
        {
            AssertCode("@@@", "7FD0");
        }
        // Reko: a decoder for the instruction ADD2 at address 3630 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ADD2()
        {
            AssertCode("@@@", "ADD2");
        }
        // Reko: a decoder for the instruction 0DE0 at address 3654 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DE0()
        {
            AssertCode("@@@", "0DE0");
        }
        // Reko: a decoder for the instruction 0DE4 at address 365A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DE4()
        {
            AssertCode("@@@", "0DE4");
        }
        // Reko: a decoder for the instruction 105A at address 368A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_105A()
        {
            AssertCode("@@@", "105A");
        }
        // Reko: a decoder for the instruction 7018 at address 36A6 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_7018()
        {
            AssertCode("@@@", "7018");
        }
        // Reko: a decoder for the instruction 6F90 at address 36C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6F90()
        {
            AssertCode("@@@", "6F90");
        }
        // Reko: a decoder for the instruction 0DD2 at address 36EA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DD2()
        {
            AssertCode("@@@", "0DD2");
        }
        // Reko: a decoder for the instruction 2CA6 at address 3736 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CA6()
        {
            AssertCode("@@@", "2CA6");
        }
        // Reko: a decoder for the instruction 483B at address 374E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_483B()
        {
            AssertCode("@@@", "483B");
        }
        // Reko: a decoder for the instruction 2C9F at address 3750 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2C9F()
        {
            AssertCode("@@@", "2C9F");
        }
        // Reko: a decoder for the instruction 8253 at address 375A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_8253()
        {
            AssertCode("@@@", "8253");
        }
        // Reko: a decoder for the instruction 7F94 at address 3762 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F94()
        {
            AssertCode("@@@", "7F94");
        }
        // Reko: a decoder for the instruction 4752 at address 3764 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4752()
        {
            AssertCode("@@@", "4752");
        }
        // Reko: a decoder for the instruction 0CA0 at address 3776 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CA0()
        {
            AssertCode("@@@", "0CA0");
        }
        // Reko: a decoder for the instruction 8353 at address 3782 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_8353()
        {
            AssertCode("@@@", "8353");
        }
        // Reko: a decoder for the instruction 5008 at address 3784 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_5008()
        {
            AssertCode("@@@", "5008");
        }
        // Reko: a decoder for the instruction 1D5A at address 3796 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1D5A()
        {
            AssertCode("@@@", "1D5A");
        }
        // Reko: a decoder for the instruction 0461 at address 37B0 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0461()
        {
            AssertCode("@@@", "0461");
        }
        // Reko: a decoder for the instruction 0A9A at address 37D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0A9A()
        {
            AssertCode("@@@", "0A9A");
        }
        // Reko: a decoder for the instruction 7898 at address 37D6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7898()
        {
            AssertCode("@@@", "7898");
        }
        // Reko: a decoder for the instruction 28A6 at address 37E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28A6()
        {
            AssertCode("@@@", "28A6");
        }
        // Reko: a decoder for the instruction 4AA8 at address 37E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4AA8()
        {
            AssertCode("@@@", "4AA8");
        }
        // Reko: a decoder for the instruction 08A6 at address 37F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08A6()
        {
            AssertCode("@@@", "08A6");
        }
        // Reko: a decoder for the instruction 08E6 at address 37F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08E6()
        {
            AssertCode("@@@", "08E6");
        }
        // Reko: a decoder for the instruction 0AA8 at address 37F6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AA8()
        {
            AssertCode("@@@", "0AA8");
        }
        // Reko: a decoder for the instruction 0AE8 at address 37FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AE8()
        {
            AssertCode("@@@", "0AE8");
        }
        // Reko: a decoder for the instruction 08AA at address 37FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08AA()
        {
            AssertCode("@@@", "08AA");
        }
        // Reko: a decoder for the instruction 08EA at address 3800 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08EA()
        {
            AssertCode("@@@", "08EA");
        }
        // Reko: a decoder for the instruction 0898 at address 3802 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0898()
        {
            AssertCode("@@@", "0898");
        }
        // Reko: a decoder for the instruction 08D8 at address 3806 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08D8()
        {
            AssertCode("@@@", "08D8");
        }
        // Reko: a decoder for the instruction 0AA4 at address 3816 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AA4()
        {
            AssertCode("@@@", "0AA4");
        }
        // Reko: a decoder for the instruction 19FE at address 383A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_19FE()
        {
            AssertCode("@@@", "19FE");
        }
        // Reko: a decoder for the instruction 2CC3 at address 383C has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_2CC3()
        {
            AssertCode("@@@", "2CC3");
        }
        // Reko: a decoder for the instruction 6FFD at address 385C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FFD()
        {
            AssertCode("@@@", "6FFD");
        }
        // Reko: a decoder for the instruction 27FD at address 3862 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_27FD()
        {
            AssertCode("@@@", "27FD");
        }
        // Reko: a decoder for the instruction 0BFE at address 386A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_0BFE()
        {
            AssertCode("@@@", "0BFE");
        }
        // Reko: a decoder for the instruction 05FE at address 38AC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_05FE()
        {
            AssertCode("@@@", "05FE");
        }
        // Reko: a decoder for the instruction 25FE at address 38B0 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_25FE()
        {
            AssertCode("@@@", "25FE");
        }
        // Reko: a decoder for the instruction 013B at address 38B2 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_013B()
        {
            AssertCode("@@@", "013B");
        }
        // Reko: a decoder for the instruction CBFC at address 38E2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CBFC()
        {
            AssertCode("@@@", "CBFC");
        }
        // Reko: a decoder for the instruction 205A at address 38E4 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_205A()
        {
            AssertCode("@@@", "205A");
        }
        // Reko: a decoder for the instruction B3FE at address 38FA has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_B3FE()
        {
            AssertCode("@@@", "B3FE");
        }
        // Reko: a decoder for the instruction A3FE at address 390A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_A3FE()
        {
            AssertCode("@@@", "A3FE");
        }
        // Reko: a decoder for the instruction F3FD at address 391E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F3FD()
        {
            AssertCode("@@@", "F3FD");
        }
        // Reko: a decoder for the instruction DFFD at address 3932 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFFD()
        {
            AssertCode("@@@", "DFFD");
        }
        // Reko: a decoder for the instruction E7FD at address 3938 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E7FD()
        {
            AssertCode("@@@", "E7FD");
        }
        // Reko: a decoder for the instruction AFFD at address 393E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFFD()
        {
            AssertCode("@@@", "AFFD");
        }
        // Reko: a decoder for the instruction AFD0 at address 3940 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFD0()
        {
            AssertCode("@@@", "AFD0");
        }
        // Reko: a decoder for the instruction EBFC at address 3946 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EBFC()
        {
            AssertCode("@@@", "EBFC");
        }
        // Reko: a decoder for the instruction BC9F at address 3954 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BC9F()
        {
            AssertCode("@@@", "BC9F");
        }
        // Reko: a decoder for the instruction 3C9F at address 3974 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3C9F()
        {
            AssertCode("@@@", "3C9F");
        }
        // Reko: a decoder for the instruction 3045 at address 397C has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_3045()
        {
            AssertCode("@@@", "3045");
        }
        // Reko: a decoder for the instruction 022B at address 397E has not been implemented. (Fmt15 1 ZZ  xorw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_022B()
        {
            AssertCode("@@@", "022B");
        }
        // Reko: a decoder for the instruction 4290 at address 3994 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4290()
        {
            AssertCode("@@@", "4290");
        }
        // Reko: a decoder for the instruction 6C9F at address 3996 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6C9F()
        {
            AssertCode("@@@", "6C9F");
        }
        // Reko: a decoder for the instruction 1623 at address 399A has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1623()
        {
            AssertCode("@@@", "1623");
        }
        // Reko: a decoder for the instruction 8CAF at address 399E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8CAF()
        {
            AssertCode("@@@", "8CAF");
        }
        // Reko: a decoder for the instruction 6661 at address 39A2 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_6661()
        {
            AssertCode("@@@", "6661");
        }
        // Reko: a decoder for the instruction 8661 at address 39A4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8661()
        {
            AssertCode("@@@", "8661");
        }
        // Reko: a decoder for the instruction 46D0 at address 39A6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_46D0()
        {
            AssertCode("@@@", "46D0");
        }
        // Reko: a decoder for the instruction 12D0 at address 39A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12D0()
        {
            AssertCode("@@@", "12D0");
        }
        // Reko: a decoder for the instruction 423B at address 39B8 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_423B()
        {
            AssertCode("@@@", "423B");
        }
        // Reko: a decoder for the instruction 6CAF at address 39CE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6CAF()
        {
            AssertCode("@@@", "6CAF");
        }
        // Reko: a decoder for the instruction 2661 at address 39D2 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2661()
        {
            AssertCode("@@@", "2661");
        }
        // Reko: a decoder for the instruction ACAF at address 39D6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ACAF()
        {
            AssertCode("@@@", "ACAF");
        }
        // Reko: a decoder for the instruction A461 at address 39DA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A461()
        {
            AssertCode("@@@", "A461");
        }
        // Reko: a decoder for the instruction 0231 at address 39E6 has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0231()
        {
            AssertCode("@@@", "0231");
        }
        // Reko: a decoder for the instruction 16D0 at address 39F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_16D0()
        {
            AssertCode("@@@", "16D0");
        }
        // Reko: a decoder for the instruction 24F0 at address 39F6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_24F0()
        {
            AssertCode("@@@", "24F0");
        }
        // Reko: a decoder for the instruction 2004 at address 39FE has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_2004()
        {
            AssertCode("@@@", "2004");
        }
        // Reko: a decoder for the instruction 2EF3 at address 3A00 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2EF3()
        {
            AssertCode("@@@", "2EF3");
        }
        // Reko: a decoder for the instruction 224C at address 3A08 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_224C()
        {
            AssertCode("@@@", "224C");
        }
        // Reko: a decoder for the instruction 6E04 at address 3A0E has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_6E04()
        {
            AssertCode("@@@", "6E04");
        }
        // Reko: a decoder for the instruction 1132 at address 3A10 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1132()
        {
            AssertCode("@@@", "1132");
        }
        // Reko: a decoder for the instruction 12DF at address 3A12 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_12DF()
        {
            AssertCode("@@@", "12DF");
        }
        // Reko: a decoder for the instruction 0004 at address 3A24 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0004()
        {
            AssertCode("@@@", "0004");
        }
        // Reko: a decoder for the instruction 2EF4 at address 3A26 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2EF4()
        {
            AssertCode("@@@", "2EF4");
        }
        // Reko: a decoder for the instruction 209F at address 3A32 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_209F()
        {
            AssertCode("@@@", "209F");
        }
        // Reko: a decoder for the instruction 1232 at address 3A36 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1232()
        {
            AssertCode("@@@", "1232");
        }
        // Reko: a decoder for the instruction 20DF at address 3A38 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_20DF()
        {
            AssertCode("@@@", "20DF");
        }
        // Reko: a decoder for the instruction 1053 at address 3A46 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1053()
        {
            AssertCode("@@@", "1053");
        }
        // Reko: a decoder for the instruction 0908 at address 3A48 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0908()
        {
            AssertCode("@@@", "0908");
        }
        // Reko: a decoder for the instruction 2B3B at address 3A52 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2B3B()
        {
            AssertCode("@@@", "2B3B");
        }
        // Reko: a decoder for the instruction BCDF at address 3A54 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BCDF()
        {
            AssertCode("@@@", "BCDF");
        }
        // Reko: a decoder for the instruction 3FD0 at address 3A5C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FD0()
        {
            AssertCode("@@@", "3FD0");
        }
        // Reko: a decoder for the instruction ECAF at address 3A5E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ECAF()
        {
            AssertCode("@@@", "ECAF");
        }
        // Reko: a decoder for the instruction 2B52 at address 3A78 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2B52()
        {
            AssertCode("@@@", "2B52");
        }
        // Reko: a decoder for the instruction A008 at address 3A7A has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_A008()
        {
            AssertCode("@@@", "A008");
        }
        // Reko: a decoder for the instruction 935A at address 3A82 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_935A()
        {
            AssertCode("@@@", "935A");
        }
        // Reko: a decoder for the instruction 2333 at address 3A84 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2333()
        {
            AssertCode("@@@", "2333");
        }
        // Reko: a decoder for the instruction 3CDF at address 3A86 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3CDF()
        {
            AssertCode("@@@", "3CDF");
        }
        // Reko: a decoder for the instruction DCAF at address 3A90 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DCAF()
        {
            AssertCode("@@@", "DCAF");
        }
        // Reko: a decoder for the instruction 1FD1 at address 3A98 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD1()
        {
            AssertCode("@@@", "1FD1");
        }
        // Reko: a decoder for the instruction 8045 at address 3AB6 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_8045()
        {
            AssertCode("@@@", "8045");
        }
        // Reko: a decoder for the instruction A023 at address 3ABA has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A023()
        {
            AssertCode("@@@", "A023");
        }
        // Reko: a decoder for the instruction 7490 at address 3AC8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7490()
        {
            AssertCode("@@@", "7490");
        }
        // Reko: a decoder for the instruction 1223 at address 3ACC has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1223()
        {
            AssertCode("@@@", "1223");
        }
        // Reko: a decoder for the instruction D261 at address 3AD2 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D261()
        {
            AssertCode("@@@", "D261");
        }
        // Reko: a decoder for the instruction 72D0 at address 3AD4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_72D0()
        {
            AssertCode("@@@", "72D0");
        }
        // Reko: a decoder for the instruction 14D0 at address 3AD6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14D0()
        {
            AssertCode("@@@", "14D0");
        }
        // Reko: a decoder for the instruction 9632 at address 3AD8 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9632()
        {
            AssertCode("@@@", "9632");
        }
        // Reko: a decoder for the instruction 6CDF at address 3ADA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6CDF()
        {
            AssertCode("@@@", "6CDF");
        }
        // Reko: a decoder for the instruction 2F95 at address 3AE4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2F95()
        {
            AssertCode("@@@", "2F95");
        }
        // Reko: a decoder for the instruction 3F90 at address 3AE6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F90()
        {
            AssertCode("@@@", "3F90");
        }
        // Reko: a decoder for the instruction 3233 at address 3AE8 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3233()
        {
            AssertCode("@@@", "3233");
        }
        // Reko: a decoder for the instruction 69FE at address 3AF2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_69FE()
        {
            AssertCode("@@@", "69FE");
        }
        // Reko: a decoder for the instruction 065A at address 3B12 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_065A()
        {
            AssertCode("@@@", "065A");
        }
        // Reko: a decoder for the instruction 0098 at address 3B78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0098()
        {
            AssertCode("@@@", "0098");
        }
        // Reko: a decoder for the instruction D9FD at address 3B7E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D9FD()
        {
            AssertCode("@@@", "D9FD");
        }
        // Reko: a decoder for the instruction CBFD at address 3B98 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CBFD()
        {
            AssertCode("@@@", "CBFD");
        }
        // Reko: a decoder for the instruction 1F94 at address 3B9A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F94()
        {
            AssertCode("@@@", "1F94");
        }
        // Reko: a decoder for the instruction 17FE at address 3BAC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_17FE()
        {
            AssertCode("@@@", "17FE");
        }
        // Reko: a decoder for the instruction B7FD at address 3BB0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B7FD()
        {
            AssertCode("@@@", "B7FD");
        }
        // Reko: a decoder for the instruction 09FE at address 3BDC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_09FE()
        {
            AssertCode("@@@", "09FE");
        }
        // Reko: a decoder for the instruction 06C2 at address 3BE8 has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_06C2()
        {
            AssertCode("@@@", "06C2");
        }
        // Reko: a decoder for the instruction 04F0 at address 3BEE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04F0()
        {
            AssertCode("@@@", "04F0");
        }
        // Reko: a decoder for the instruction 33FD at address 3C28 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_33FD()
        {
            AssertCode("@@@", "33FD");
        }
        // Reko: a decoder for the instruction 0233 at address 3C30 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0233()
        {
            AssertCode("@@@", "0233");
        }
        // Reko: a decoder for the instruction E061 at address 3C3C has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_E061()
        {
            AssertCode("@@@", "E061");
        }
        // Reko: a decoder for the instruction 0E61 at address 3C4C has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0E61()
        {
            AssertCode("@@@", "0E61");
        }
        // Reko: a decoder for the instruction 0EB0 at address 3C4E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0EB0()
        {
            AssertCode("@@@", "0EB0");
        }
        // Reko: a decoder for the instruction 8345 at address 3C52 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_8345()
        {
            AssertCode("@@@", "8345");
        }
        // Reko: a decoder for the instruction 302B at address 3C54 has not been implemented. (Fmt15 1 ZZ  xorw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_302B()
        {
            AssertCode("@@@", "302B");
        }
        // Reko: a decoder for the instruction 0A23 at address 3C56 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0A23()
        {
            AssertCode("@@@", "0A23");
        }
        // Reko: a decoder for the instruction FBFC at address 3C60 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FBFC()
        {
            AssertCode("@@@", "FBFC");
        }
        // Reko: a decoder for the instruction 2152 at address 3C68 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2152()
        {
            AssertCode("@@@", "2152");
        }
        // Reko: a decoder for the instruction 1098 at address 3D9E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1098()
        {
            AssertCode("@@@", "1098");
        }
        // Reko: a decoder for the instruction 5DFD at address 3DB6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5DFD()
        {
            AssertCode("@@@", "5DFD");
        }
        // Reko: a decoder for the instruction 0454 at address 3DB8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0454()
        {
            AssertCode("@@@", "0454");
        }
        // Reko: a decoder for the instruction 4FD3 at address 3DCC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FD3()
        {
            AssertCode("@@@", "4FD3");
        }
        // Reko: a decoder for the instruction AC9F at address 3DE4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AC9F()
        {
            AssertCode("@@@", "AC9F");
        }
        // Reko: a decoder for the instruction B232 at address 3E4A has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B232()
        {
            AssertCode("@@@", "B232");
        }
        // Reko: a decoder for the instruction 433B at address 3E52 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_433B()
        {
            AssertCode("@@@", "433B");
        }
        // Reko: a decoder for the instruction 2353 at address 3E54 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2353()
        {
            AssertCode("@@@", "2353");
        }
        // Reko: a decoder for the instruction 2552 at address 3E58 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2552()
        {
            AssertCode("@@@", "2552");
        }
        // Reko: a decoder for the instruction 0553 at address 3E5C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0553()
        {
            AssertCode("@@@", "0553");
        }
        // Reko: a decoder for the instruction 4661 at address 3E82 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4661()
        {
            AssertCode("@@@", "4661");
        }
        // Reko: a decoder for the instruction 02C2 at address 3E86 has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_02C2()
        {
            AssertCode("@@@", "02C2");
        }
        // Reko: a decoder for the instruction 02F0 at address 3E94 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02F0()
        {
            AssertCode("@@@", "02F0");
        }
        // Reko: a decoder for the instruction 1F93 at address 3F0A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F93()
        {
            AssertCode("@@@", "1F93");
        }
        // Reko: a decoder for the instruction C5FE at address 3F26 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_C5FE()
        {
            AssertCode("@@@", "C5FE");
        }
        // Reko: a decoder for the instruction 3052 at address 3F60 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3052()
        {
            AssertCode("@@@", "3052");
        }
        // Reko: a decoder for the instruction B352 at address 3F6E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B352()
        {
            AssertCode("@@@", "B352");
        }
        // Reko: a decoder for the instruction 1A33 at address 3F88 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1A33()
        {
            AssertCode("@@@", "1A33");
        }
        // Reko: a decoder for the instruction 0531 at address 3F8E has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0531()
        {
            AssertCode("@@@", "0531");
        }
        // Reko: a decoder for the instruction 965A at address 3FA8 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_965A()
        {
            AssertCode("@@@", "965A");
        }
        // Reko: a decoder for the instruction 8633 at address 3FAA has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8633()
        {
            AssertCode("@@@", "8633");
        }
        // Reko: a decoder for the instruction 62D0 at address 3FAC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_62D0()
        {
            AssertCode("@@@", "62D0");
        }
        // Reko: a decoder for the instruction 1432 at address 3FB2 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1432()
        {
            AssertCode("@@@", "1432");
        }
        // Reko: a decoder for the instruction 4CDF at address 3FB4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4CDF()
        {
            AssertCode("@@@", "4CDF");
        }
        // Reko: a decoder for the instruction 50F0 at address 3FBA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_50F0()
        {
            AssertCode("@@@", "50F0");
        }
        // Reko: a decoder for the instruction B832 at address 3FBC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B832()
        {
            AssertCode("@@@", "B832");
        }
        // Reko: a decoder for the instruction B852 at address 3FDC has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B852()
        {
            AssertCode("@@@", "B852");
        }
        // Reko: a decoder for the instruction 1004 at address 3FE6 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_1004()
        {
            AssertCode("@@@", "1004");
        }
        // Reko: a decoder for the instruction 01B0 at address 3FEA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_01B0()
        {
            AssertCode("@@@", "01B0");
        }
        // Reko: a decoder for the instruction EC9F at address 3FFE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_EC9F()
        {
            AssertCode("@@@", "EC9F");
        }
        // Reko: a decoder for the instruction 1732 at address 4012 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1732()
        {
            AssertCode("@@@", "1732");
        }
        // Reko: a decoder for the instruction B73B at address 4014 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B73B()
        {
            AssertCode("@@@", "B73B");
        }
        // Reko: a decoder for the instruction B833 at address 401E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B833()
        {
            AssertCode("@@@", "B833");
        }
        // Reko: a decoder for the instruction 9633 at address 402A has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9633()
        {
            AssertCode("@@@", "9633");
        }
        // Reko: a decoder for the instruction 6233 at address 403A has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6233()
        {
            AssertCode("@@@", "6233");
        }
        // Reko: a decoder for the instruction 20B0 at address 4044 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20B0()
        {
            AssertCode("@@@", "20B0");
        }
        // Reko: a decoder for the instruction 1045 at address 4050 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1045()
        {
            AssertCode("@@@", "1045");
        }
        // Reko: a decoder for the instruction 7290 at address 4068 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7290()
        {
            AssertCode("@@@", "7290");
        }
        // Reko: a decoder for the instruction 6023 at address 406E has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6023()
        {
            AssertCode("@@@", "6023");
        }
        // Reko: a decoder for the instruction 70D0 at address 407A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_70D0()
        {
            AssertCode("@@@", "70D0");
        }
        // Reko: a decoder for the instruction 9B33 at address 4094 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9B33()
        {
            AssertCode("@@@", "9B33");
        }
        // Reko: a decoder for the instruction 9B32 at address 4096 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9B32()
        {
            AssertCode("@@@", "9B32");
        }
        // Reko: a decoder for the instruction 35FD at address 40A2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_35FD()
        {
            AssertCode("@@@", "35FD");
        }
        // Reko: a decoder for the instruction A3FD at address 4134 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A3FD()
        {
            AssertCode("@@@", "A3FD");
        }
        // Reko: a decoder for the instruction 73FC at address 4164 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_73FC()
        {
            AssertCode("@@@", "73FC");
        }
        // Reko: a decoder for the instruction 05FD at address 41B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_05FD()
        {
            AssertCode("@@@", "05FD");
        }
        // Reko: a decoder for the instruction CDFC at address 41EE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CDFC()
        {
            AssertCode("@@@", "CDFC");
        }
        // Reko: a decoder for the instruction C5FC at address 41F6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C5FC()
        {
            AssertCode("@@@", "C5FC");
        }
        // Reko: a decoder for the instruction B132 at address 41FC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B132()
        {
            AssertCode("@@@", "B132");
        }
        // Reko: a decoder for the instruction E5FD at address 420C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E5FD()
        {
            AssertCode("@@@", "E5FD");
        }
        // Reko: a decoder for the instruction 12B0 at address 4232 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12B0()
        {
            AssertCode("@@@", "12B0");
        }
        // Reko: a decoder for the instruction 6461 at address 4242 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_6461()
        {
            AssertCode("@@@", "6461");
        }
        // Reko: a decoder for the instruction 12F0 at address 4252 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12F0()
        {
            AssertCode("@@@", "12F0");
        }
        // Reko: a decoder for the instruction 3F93 at address 4278 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F93()
        {
            AssertCode("@@@", "3F93");
        }
        // Reko: a decoder for the instruction 51FC at address 4288 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_51FC()
        {
            AssertCode("@@@", "51FC");
        }
        // Reko: a decoder for the instruction 29FB at address 43B0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_29FB()
        {
            AssertCode("@@@", "29FB");
        }
        // Reko: a decoder for the instruction EBFE at address 43BE has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_EBFE()
        {
            AssertCode("@@@", "EBFE");
        }
        // Reko: a decoder for the instruction 2FE2 at address 43CC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FE2()
        {
            AssertCode("@@@", "2FE2");
        }
        // Reko: a decoder for the instruction D2AF at address 43EC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D2AF()
        {
            AssertCode("@@@", "D2AF");
        }
        // Reko: a decoder for the instruction 2DA0 at address 43F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DA0()
        {
            AssertCode("@@@", "2DA0");
        }
        // Reko: a decoder for the instruction 2D92 at address 440A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2D92()
        {
            AssertCode("@@@", "2D92");
        }
        // Reko: a decoder for the instruction B252 at address 440C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B252()
        {
            AssertCode("@@@", "B252");
        }
        // Reko: a decoder for the instruction 1308 at address 4410 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1308()
        {
            AssertCode("@@@", "1308");
        }
        // Reko: a decoder for the instruction 3121 at address 4438 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3121()
        {
            AssertCode("@@@", "3121");
        }
        // Reko: a decoder for the instruction B15A at address 443E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B15A()
        {
            AssertCode("@@@", "B15A");
        }
        // Reko: a decoder for the instruction C7FF at address 4440 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C7FF()
        {
            AssertCode("@@@", "C7FF");
        }
        // Reko: a decoder for the instruction 2133 at address 4442 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2133()
        {
            AssertCode("@@@", "2133");
        }
        // Reko: a decoder for the instruction EFFF at address 4446 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_EFFF()
        {
            AssertCode("@@@", "EFFF");
        }
        // Reko: a decoder for the instruction 5D9C at address 4456 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5D9C()
        {
            AssertCode("@@@", "5D9C");
        }
        // Reko: a decoder for the instruction 5FD1 at address 4458 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FD1()
        {
            AssertCode("@@@", "5FD1");
        }
        // Reko: a decoder for the instruction 1D9F at address 4466 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1D9F()
        {
            AssertCode("@@@", "1D9F");
        }
        // Reko: a decoder for the instruction 1DDC at address 4472 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DDC()
        {
            AssertCode("@@@", "1DDC");
        }
        // Reko: a decoder for the instruction 5561 at address 448E has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_5561()
        {
            AssertCode("@@@", "5561");
        }
        // Reko: a decoder for the instruction 5061 at address 4492 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_5061()
        {
            AssertCode("@@@", "5061");
        }
        // Reko: a decoder for the instruction 40D0 at address 4494 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_40D0()
        {
            AssertCode("@@@", "40D0");
        }
        // Reko: a decoder for the instruction 5F91 at address 449A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5F91()
        {
            AssertCode("@@@", "5F91");
        }
        // Reko: a decoder for the instruction 2861 at address 44B2 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_2861()
        {
            AssertCode("@@@", "2861");
        }
        // Reko: a decoder for the instruction 1092 at address 44B8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1092()
        {
            AssertCode("@@@", "1092");
        }
        // Reko: a decoder for the instruction 1FD4 at address 44BA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD4()
        {
            AssertCode("@@@", "1FD4");
        }
        // Reko: a decoder for the instruction 32A0 at address 44BE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_32A0()
        {
            AssertCode("@@@", "32A0");
        }
        // Reko: a decoder for the instruction 3FE6 at address 44C0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FE6()
        {
            AssertCode("@@@", "3FE6");
        }
        // Reko: a decoder for the instruction 74D2 at address 44C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_74D2()
        {
            AssertCode("@@@", "74D2");
        }
        // Reko: a decoder for the instruction 84E0 at address 44C6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_84E0()
        {
            AssertCode("@@@", "84E0");
        }
        // Reko: a decoder for the instruction EDAF at address 44E0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_EDAF()
        {
            AssertCode("@@@", "EDAF");
        }
        // Reko: a decoder for the instruction CDAF at address 44EE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_CDAF()
        {
            AssertCode("@@@", "CDAF");
        }
        // Reko: a decoder for the instruction 6D9F at address 44F2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6D9F()
        {
            AssertCode("@@@", "6D9F");
        }
        // Reko: a decoder for the instruction 2933 at address 4500 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2933()
        {
            AssertCode("@@@", "2933");
        }
        // Reko: a decoder for the instruction 6490 at address 4524 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6490()
        {
            AssertCode("@@@", "6490");
        }
        // Reko: a decoder for the instruction 9153 at address 4536 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9153()
        {
            AssertCode("@@@", "9153");
        }
        // Reko: a decoder for the instruction 9D9F at address 4548 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_9D9F()
        {
            AssertCode("@@@", "9D9F");
        }
        // Reko: a decoder for the instruction 2DC3 at address 456E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_2DC3()
        {
            AssertCode("@@@", "2DC3");
        }
        // Reko: a decoder for the instruction 3FA2 at address 4578 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FA2()
        {
            AssertCode("@@@", "3FA2");
        }
        // Reko: a decoder for the instruction 03E0 at address 457A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_03E0()
        {
            AssertCode("@@@", "03E0");
        }
        // Reko: a decoder for the instruction 13D2 at address 457E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_13D2()
        {
            AssertCode("@@@", "13D2");
        }
        // Reko: a decoder for the instruction 2DDC at address 4582 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DDC()
        {
            AssertCode("@@@", "2DDC");
        }
        // Reko: a decoder for the instruction AD9F at address 45A0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AD9F()
        {
            AssertCode("@@@", "AD9F");
        }
        // Reko: a decoder for the instruction 20AF at address 45AA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_20AF()
        {
            AssertCode("@@@", "20AF");
        }
        // Reko: a decoder for the instruction 02EF at address 45B8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_02EF()
        {
            AssertCode("@@@", "02EF");
        }
        // Reko: a decoder for the instruction ADDC at address 45BC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ADDC()
        {
            AssertCode("@@@", "ADDC");
        }
        // Reko: a decoder for the instruction F3FE at address 45C6 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_F3FE()
        {
            AssertCode("@@@", "F3FE");
        }
        // Reko: a decoder for the instruction 1200FC612A55 at address 45D2 has not been implemented. (Fmt2 3 ZZ ope 6  loadb (rrp) disp20 4 src (rrp) 4 dest reg 20 src disp 4)
        [Test]
        public void Cr16Dasm_1200FC612A55()
        {
            AssertCode("@@@", "1200FC612A55");
        }
        // Reko: a decoder for the instruction 0AAF at address 45E2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0AAF()
        {
            AssertCode("@@@", "0AAF");
        }
        // Reko: a decoder for the instruction EAAF at address 45F2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_EAAF()
        {
            AssertCode("@@@", "EAAF");
        }
        // Reko: a decoder for the instruction 4EA0 at address 45FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4EA0()
        {
            AssertCode("@@@", "4EA0");
        }
        // Reko: a decoder for the instruction 2E92 at address 4608 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2E92()
        {
            AssertCode("@@@", "2E92");
        }
        // Reko: a decoder for the instruction 1408 at address 460E has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1408()
        {
            AssertCode("@@@", "1408");
        }
        // Reko: a decoder for the instruction 4121 at address 4636 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4121()
        {
            AssertCode("@@@", "4121");
        }
        // Reko: a decoder for the instruction B222 at address 4640 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B222()
        {
            AssertCode("@@@", "B222");
        }
        // Reko: a decoder for the instruction 0E9F at address 464C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0E9F()
        {
            AssertCode("@@@", "0E9F");
        }
        // Reko: a decoder for the instruction 7E9F at address 4658 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7E9F()
        {
            AssertCode("@@@", "7E9F");
        }
        // Reko: a decoder for the instruction 3127 at address 4664 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3127()
        {
            AssertCode("@@@", "3127");
        }
        // Reko: a decoder for the instruction 78D0 at address 4696 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_78D0()
        {
            AssertCode("@@@", "78D0");
        }
        // Reko: a decoder for the instruction A2AF at address 46C6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_A2AF()
        {
            AssertCode("@@@", "A2AF");
        }
        // Reko: a decoder for the instruction 2AA0 at address 46CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AA0()
        {
            AssertCode("@@@", "2AA0");
        }
        // Reko: a decoder for the instruction 2A92 at address 46DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2A92()
        {
            AssertCode("@@@", "2A92");
        }
        // Reko: a decoder for the instruction 08E4 at address 4724 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08E4()
        {
            AssertCode("@@@", "08E4");
        }
        // Reko: a decoder for the instruction 0A9C at address 4732 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0A9C()
        {
            AssertCode("@@@", "0A9C");
        }
        // Reko: a decoder for the instruction 1ADC at address 4768 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1ADC()
        {
            AssertCode("@@@", "1ADC");
        }
        // Reko: a decoder for the instruction 3900 at address 4776 has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3900()
        {
            AssertCode("@@@", "3900");
        }
        // Reko: a decoder for the instruction 0AD2 at address 4778 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AD2()
        {
            AssertCode("@@@", "0AD2");
        }
        // Reko: a decoder for the instruction 88AF at address 484E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_88AF()
        {
            AssertCode("@@@", "88AF");
        }
        // Reko: a decoder for the instruction 289F at address 4878 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_289F()
        {
            AssertCode("@@@", "289F");
        }
        // Reko: a decoder for the instruction 2091 at address 488C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2091()
        {
            AssertCode("@@@", "2091");
        }
        // Reko: a decoder for the instruction 28DF at address 488E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_28DF()
        {
            AssertCode("@@@", "28DF");
        }
        // Reko: a decoder for the instruction E2AF at address 4912 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E2AF()
        {
            AssertCode("@@@", "E2AF");
        }
        // Reko: a decoder for the instruction 6EA0 at address 491A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6EA0()
        {
            AssertCode("@@@", "6EA0");
        }
        // Reko: a decoder for the instruction 0E9C at address 496C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0E9C()
        {
            AssertCode("@@@", "0E9C");
        }
        // Reko: a decoder for the instruction 4EEF at address 4972 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4EEF()
        {
            AssertCode("@@@", "4EEF");
        }
        // Reko: a decoder for the instruction EC54 at address 498C has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_EC54()
        {
            AssertCode("@@@", "EC54");
        }
        // Reko: a decoder for the instruction 6CA0 at address 4990 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6CA0()
        {
            AssertCode("@@@", "6CA0");
        }
        // Reko: a decoder for the instruction 8EA0 at address 49AE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8EA0()
        {
            AssertCode("@@@", "8EA0");
        }
        // Reko: a decoder for the instruction 0E9A at address 4A06 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0E9A()
        {
            AssertCode("@@@", "0E9A");
        }
        // Reko: a decoder for the instruction 06D0 at address 4A12 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_06D0()
        {
            AssertCode("@@@", "06D0");
        }
        // Reko: a decoder for the instruction C2AF at address 4A4A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C2AF()
        {
            AssertCode("@@@", "C2AF");
        }
        // Reko: a decoder for the instruction 1C92 at address 4A62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1C92()
        {
            AssertCode("@@@", "1C92");
        }
        // Reko: a decoder for the instruction 2021 at address 4A90 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2021()
        {
            AssertCode("@@@", "2021");
        }
        // Reko: a decoder for the instruction 0CA8 at address 4AA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CA8()
        {
            AssertCode("@@@", "0CA8");
        }
        // Reko: a decoder for the instruction 1000195A1C9F at address 4AB4 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000195A1C9F()
        {
            AssertCode("@@@", "1000195A1C9F");
        }
        // Reko: a decoder for the instruction 0B53 at address 4AC2 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0B53()
        {
            AssertCode("@@@", "0B53");
        }
        // Reko: a decoder for the instruction 7045 at address 4ACA has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_7045()
        {
            AssertCode("@@@", "7045");
        }
        // Reko: a decoder for the instruction 8023 at address 4ACE has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8023()
        {
            AssertCode("@@@", "8023");
        }
        // Reko: a decoder for the instruction 2027 at address 4AD6 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2027()
        {
            AssertCode("@@@", "2027");
        }
        // Reko: a decoder for the instruction 7039 at address 4AEA has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7039()
        {
            AssertCode("@@@", "7039");
        }
        // Reko: a decoder for the instruction 0845 at address 4AEC has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0845()
        {
            AssertCode("@@@", "0845");
        }
        // Reko: a decoder for the instruction AC54 at address 4B0C has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_AC54()
        {
            AssertCode("@@@", "AC54");
        }
        // Reko: a decoder for the instruction CEA0 at address 4B2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CEA0()
        {
            AssertCode("@@@", "CEA0");
        }
        // Reko: a decoder for the instruction 4EDF at address 4B82 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4EDF()
        {
            AssertCode("@@@", "4EDF");
        }
        // Reko: a decoder for the instruction 6EDF at address 4B8A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6EDF()
        {
            AssertCode("@@@", "6EDF");
        }
        // Reko: a decoder for the instruction 7E54 at address 4BA8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7E54()
        {
            AssertCode("@@@", "7E54");
        }
        // Reko: a decoder for the instruction 4E61 at address 4BAA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4E61()
        {
            AssertCode("@@@", "4E61");
        }
        // Reko: a decoder for the instruction 3F00 at address 4BAE has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3F00()
        {
            AssertCode("@@@", "3F00");
        }
        // Reko: a decoder for the instruction A04B at address 4BB2 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_A04B()
        {
            AssertCode("@@@", "A04B");
        }
        // Reko: a decoder for the instruction D64B at address 4BB6 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_D64B()
        {
            AssertCode("@@@", "D64B");
        }
        // Reko: a decoder for the instruction 62AF at address 4BC0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_62AF()
        {
            AssertCode("@@@", "62AF");
        }
        // Reko: a decoder for the instruction 82AF at address 4BD0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_82AF()
        {
            AssertCode("@@@", "82AF");
        }
        // Reko: a decoder for the instruction 68A0 at address 4BD8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_68A0()
        {
            AssertCode("@@@", "68A0");
        }
        // Reko: a decoder for the instruction 3892 at address 4BE6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3892()
        {
            AssertCode("@@@", "3892");
        }
        // Reko: a decoder for the instruction 1608 at address 4BEC has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1608()
        {
            AssertCode("@@@", "1608");
        }
        // Reko: a decoder for the instruction 6221 at address 4C14 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6221()
        {
            AssertCode("@@@", "6221");
        }
        // Reko: a decoder for the instruction B332 at address 4C1A has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B332()
        {
            AssertCode("@@@", "B332");
        }
        // Reko: a decoder for the instruction B322 at address 4C1E has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B322()
        {
            AssertCode("@@@", "B322");
        }
        // Reko: a decoder for the instruction 289C at address 4C2A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_289C()
        {
            AssertCode("@@@", "289C");
        }
        // Reko: a decoder for the instruction 1252 at address 4C2C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1252()
        {
            AssertCode("@@@", "1252");
        }
        // Reko: a decoder for the instruction 6654 at address 4C38 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6654()
        {
            AssertCode("@@@", "6654");
        }
        // Reko: a decoder for the instruction F252 at address 4C3E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F252()
        {
            AssertCode("@@@", "F252");
        }
        // Reko: a decoder for the instruction 5060 at address 4C42 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5060()
        {
            AssertCode("@@@", "5060");
        }
        // Reko: a decoder for the instruction 404B at address 4C54 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_404B()
        {
            AssertCode("@@@", "404B");
        }
        // Reko: a decoder for the instruction 2E4B at address 4C5A has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_2E4B()
        {
            AssertCode("@@@", "2E4B");
        }
        // Reko: a decoder for the instruction 744A at address 4C5E has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_744A()
        {
            AssertCode("@@@", "744A");
        }
        // Reko: a decoder for the instruction 0654 at address 4C68 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0654()
        {
            AssertCode("@@@", "0654");
        }
        // Reko: a decoder for the instruction C8AF at address 4C6C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C8AF()
        {
            AssertCode("@@@", "C8AF");
        }
        // Reko: a decoder for the instruction 6C98 at address 4C7A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6C98()
        {
            AssertCode("@@@", "6C98");
        }
        // Reko: a decoder for the instruction 2632 at address 4C7C has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2632()
        {
            AssertCode("@@@", "2632");
        }
        // Reko: a decoder for the instruction B660 at address 4C80 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B660()
        {
            AssertCode("@@@", "B660");
        }
        // Reko: a decoder for the instruction 1200ACAA0A56 at address 4C82 has not been implemented. (Fmt2 3 ZZ ope 10  loadd (rrp) disp20 4 src (rrp) 4 dest rp 20 src disp 4)
        [Test]
        public void Cr16Dasm_1200ACAA0A56()
        {
            AssertCode("@@@", "1200ACAA0A56");
        }
        // Reko: a decoder for the instruction 1660 at address 4C8A has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1660()
        {
            AssertCode("@@@", "1660");
        }
        // Reko: a decoder for the instruction 9254 at address 4C8E has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9254()
        {
            AssertCode("@@@", "9254");
        }
        // Reko: a decoder for the instruction A261 at address 4C90 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A261()
        {
            AssertCode("@@@", "A261");
        }
        // Reko: a decoder for the instruction 2660 at address 4CB6 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2660()
        {
            AssertCode("@@@", "2660");
        }
        // Reko: a decoder for the instruction A654 at address 4CC2 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A654()
        {
            AssertCode("@@@", "A654");
        }
        // Reko: a decoder for the instruction B654 at address 4CC6 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B654()
        {
            AssertCode("@@@", "B654");
        }
        // Reko: a decoder for the instruction 1200E81BB654 at address 4CC8 has not been implemented. (Fmt2 3 ZZ ope 1  storb imm(rp) disp20 4 dest(rp) 4 src imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1200E81BB654()
        {
            AssertCode("@@@", "1200E81BB654");
        }
        // Reko: a decoder for the instruction 1200EA1D0000 at address 4CCE has not been implemented. (Fmt2 3 ZZ ope 1  storb imm(rp) disp20 4 dest(rp) 4 src imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1200EA1D0000()
        {
            AssertCode("@@@", "1200EA1D0000");
        }
        // Reko: a decoder for the instruction 4FD0 at address 4CDE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FD0()
        {
            AssertCode("@@@", "4FD0");
        }
        // Reko: a decoder for the instruction 8404 at address 4CE4 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_8404()
        {
            AssertCode("@@@", "8404");
        }
        // Reko: a decoder for the instruction 7A04 at address 4CEE has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_7A04()
        {
            AssertCode("@@@", "7A04");
        }
        // Reko: a decoder for the instruction 7004 at address 4CF8 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_7004()
        {
            AssertCode("@@@", "7004");
        }
        // Reko: a decoder for the instruction 6604 at address 4D02 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_6604()
        {
            AssertCode("@@@", "6604");
        }
        // Reko: a decoder for the instruction 5052 at address 4D5C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5052()
        {
            AssertCode("@@@", "5052");
        }
        // Reko: a decoder for the instruction 0804 at address 4D60 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0804()
        {
            AssertCode("@@@", "0804");
        }
        // Reko: a decoder for the instruction 0CA6 at address 4D62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CA6()
        {
            AssertCode("@@@", "0CA6");
        }
        // Reko: a decoder for the instruction F20F at address 4D68 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_F20F()
        {
            AssertCode("@@@", "F20F");
        }
        // Reko: a decoder for the instruction 3C92 at address 4D6A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3C92()
        {
            AssertCode("@@@", "3C92");
        }
        // Reko: a decoder for the instruction E40F at address 4D76 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_E40F()
        {
            AssertCode("@@@", "E40F");
        }
        // Reko: a decoder for the instruction 4152 at address 4D7A has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4152()
        {
            AssertCode("@@@", "4152");
        }
        // Reko: a decoder for the instruction CC0F at address 4D8E has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_CC0F()
        {
            AssertCode("@@@", "CC0F");
        }
        // Reko: a decoder for the instruction 0C98 at address 4D90 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0C98()
        {
            AssertCode("@@@", "0C98");
        }
        // Reko: a decoder for the instruction 980F at address 4D96 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_980F()
        {
            AssertCode("@@@", "980F");
        }
        // Reko: a decoder for the instruction 4DAA at address 4DA2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DAA()
        {
            AssertCode("@@@", "4DAA");
        }
        // Reko: a decoder for the instruction 4433 at address 4DB2 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4433()
        {
            AssertCode("@@@", "4433");
        }
        // Reko: a decoder for the instruction 5352 at address 4DB6 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5352()
        {
            AssertCode("@@@", "5352");
        }
        // Reko: a decoder for the instruction 4052 at address 4DC0 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4052()
        {
            AssertCode("@@@", "4052");
        }
        // Reko: a decoder for the instruction F7FF at address 4DC6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F7FF()
        {
            AssertCode("@@@", "F7FF");
        }
        // Reko: a decoder for the instruction 3453 at address 4DC8 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_3453()
        {
            AssertCode("@@@", "3453");
        }
        // Reko: a decoder for the instruction 5A0F at address 4DD4 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_5A0F()
        {
            AssertCode("@@@", "5A0F");
        }
        // Reko: a decoder for the instruction 4A0F at address 4DE4 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_4A0F()
        {
            AssertCode("@@@", "4A0F");
        }
        // Reko: a decoder for the instruction 2E08 at address 4DEE has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_2E08()
        {
            AssertCode("@@@", "2E08");
        }
        // Reko: a decoder for the instruction D407 at address 4DF8 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_D407()
        {
            AssertCode("@@@", "D407");
        }
        // Reko: a decoder for the instruction 3152 at address 4E04 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3152()
        {
            AssertCode("@@@", "3152");
        }
        // Reko: a decoder for the instruction BC0B at address 4E08 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_BC0B()
        {
            AssertCode("@@@", "BC0B");
        }
        // Reko: a decoder for the instruction 00A4 at address 4E1A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00A4()
        {
            AssertCode("@@@", "00A4");
        }
        // Reko: a decoder for the instruction 520D at address 4E2C has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_520D()
        {
            AssertCode("@@@", "520D");
        }
        // Reko: a decoder for the instruction 0123 at address 4E32 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0123()
        {
            AssertCode("@@@", "0123");
        }
        // Reko: a decoder for the instruction 1352 at address 4E40 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1352()
        {
            AssertCode("@@@", "1352");
        }
        // Reko: a decoder for the instruction E00E at address 4E44 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_E00E()
        {
            AssertCode("@@@", "E00E");
        }
        // Reko: a decoder for the instruction 3452 at address 4E5C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3452()
        {
            AssertCode("@@@", "3452");
        }
        // Reko: a decoder for the instruction 48A8 at address 4E7E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48A8()
        {
            AssertCode("@@@", "48A8");
        }
        // Reko: a decoder for the instruction 0CE6 at address 4E8A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CE6()
        {
            AssertCode("@@@", "0CE6");
        }
        // Reko: a decoder for the instruction 08A8 at address 4E8C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08A8()
        {
            AssertCode("@@@", "08A8");
        }
        // Reko: a decoder for the instruction 0CAA at address 4E92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CAA()
        {
            AssertCode("@@@", "0CAA");
        }
        // Reko: a decoder for the instruction 0CEA at address 4E96 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CEA()
        {
            AssertCode("@@@", "0CEA");
        }
        // Reko: a decoder for the instruction 0CD8 at address 4E9C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CD8()
        {
            AssertCode("@@@", "0CD8");
        }
        // Reko: a decoder for the instruction 28AA at address 4E9E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28AA()
        {
            AssertCode("@@@", "28AA");
        }
        // Reko: a decoder for the instruction 28A4 at address 4EAC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28A4()
        {
            AssertCode("@@@", "28A4");
        }
        // Reko: a decoder for the instruction 28E8 at address 4EAE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28E8()
        {
            AssertCode("@@@", "28E8");
        }
        // Reko: a decoder for the instruction 3D9C at address 4EBE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3D9C()
        {
            AssertCode("@@@", "3D9C");
        }
        // Reko: a decoder for the instruction D018 at address 4EC2 has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_D018()
        {
            AssertCode("@@@", "D018");
        }
        // Reko: a decoder for the instruction 560E at address 4EC4 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_560E()
        {
            AssertCode("@@@", "560E");
        }
        // Reko: a decoder for the instruction 1454 at address 4ED0 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1454()
        {
            AssertCode("@@@", "1454");
        }
        // Reko: a decoder for the instruction 6DA4 at address 4ED4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DA4()
        {
            AssertCode("@@@", "6DA4");
        }
        // Reko: a decoder for the instruction 4DEA at address 4ED8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DEA()
        {
            AssertCode("@@@", "4DEA");
        }
        // Reko: a decoder for the instruction 2352 at address 4EDA has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2352()
        {
            AssertCode("@@@", "2352");
        }
        // Reko: a decoder for the instruction 1E0F at address 4EDE has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_1E0F()
        {
            AssertCode("@@@", "1E0F");
        }
        // Reko: a decoder for the instruction 30F0 at address 4EE4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_30F0()
        {
            AssertCode("@@@", "30F0");
        }
        // Reko: a decoder for the instruction 1654 at address 4EEA has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1654()
        {
            AssertCode("@@@", "1654");
        }
        // Reko: a decoder for the instruction 6DEA at address 4EEE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DEA()
        {
            AssertCode("@@@", "6DEA");
        }
        // Reko: a decoder for the instruction 7C98 at address 4F24 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7C98()
        {
            AssertCode("@@@", "7C98");
        }
        // Reko: a decoder for the instruction 0D9C at address 4F66 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D9C()
        {
            AssertCode("@@@", "0D9C");
        }
        // Reko: a decoder for the instruction 1D9B at address 4F74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1D9B()
        {
            AssertCode("@@@", "1D9B");
        }
        // Reko: a decoder for the instruction A60D at address 4F88 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_A60D()
        {
            AssertCode("@@@", "A60D");
        }
        // Reko: a decoder for the instruction C742 at address 4F94 has not been implemented. (0100 0010 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(left +), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_C742()
        {
            AssertCode("@@@", "C742");
        }
        // Reko: a decoder for the instruction B732 at address 4F96 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B732()
        {
            AssertCode("@@@", "B732");
        }
        // Reko: a decoder for the instruction 0088 at address 4F98 has not been implemented. (Fmt12 2 ZZ  loadb abs20 20 src abs 4 dest reg)
        [Test]
        public void Cr16Dasm_0088()
        {
            AssertCode("@@@", "0088");
        }
        // Reko: a decoder for the instruction 9006 at address 4FA2 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_9006()
        {
            AssertCode("@@@", "9006");
        }
        // Reko: a decoder for the instruction B726 at address 4FAC has not been implemented. (ZZ orw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B726()
        {
            AssertCode("@@@", "B726");
        }
        // Reko: a decoder for the instruction B35A at address 4FB0 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B35A()
        {
            AssertCode("@@@", "B35A");
        }
        // Reko: a decoder for the instruction 023B at address 4FC0 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_023B()
        {
            AssertCode("@@@", "023B");
        }
        // Reko: a decoder for the instruction 4FFD at address 509A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FFD()
        {
            AssertCode("@@@", "4FFD");
        }
        // Reko: a decoder for the instruction 009F at address 50A0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_009F()
        {
            AssertCode("@@@", "009F");
        }
        // Reko: a decoder for the instruction 1C0B at address 50A8 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1C0B()
        {
            AssertCode("@@@", "1C0B");
        }
        // Reko: a decoder for the instruction 6DAA at address 50AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DAA()
        {
            AssertCode("@@@", "6DAA");
        }
        // Reko: a decoder for the instruction 463B at address 50B0 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_463B()
        {
            AssertCode("@@@", "463B");
        }
        // Reko: a decoder for the instruction F00A at address 50D4 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_F00A()
        {
            AssertCode("@@@", "F00A");
        }
        // Reko: a decoder for the instruction 2454 at address 50D8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2454()
        {
            AssertCode("@@@", "2454");
        }
        // Reko: a decoder for the instruction B00A at address 50E2 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_B00A()
        {
            AssertCode("@@@", "B00A");
        }
        // Reko: a decoder for the instruction 5A0A at address 5138 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_5A0A()
        {
            AssertCode("@@@", "5A0A");
        }
        // Reko: a decoder for the instruction 75FC at address 5164 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_75FC()
        {
            AssertCode("@@@", "75FC");
        }
        // Reko: a decoder for the instruction A40D at address 5182 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_A40D()
        {
            AssertCode("@@@", "A40D");
        }
        // Reko: a decoder for the instruction 8C0C at address 5202 has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_8C0C()
        {
            AssertCode("@@@", "8C0C");
        }
        // Reko: a decoder for the instruction 1006 at address 5210 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_1006()
        {
            AssertCode("@@@", "1006");
        }
        // Reko: a decoder for the instruction 2DA4 at address 5250 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DA4()
        {
            AssertCode("@@@", "2DA4");
        }
        // Reko: a decoder for the instruction 2DE8 at address 5252 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DE8()
        {
            AssertCode("@@@", "2DE8");
        }
        // Reko: a decoder for the instruction 8082 at address 5294 has not been implemented. (Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_8082()
        {
            AssertCode("@@@", "8082");
        }
        // Reko: a decoder for the instruction 8609 at address 529E has not been implemented. (Fmt9 1 ZZ  lshb cnt(right -), reg 4 dest reg 3 count imm)
        [Test]
        public void Cr16Dasm_8609()
        {
            AssertCode("@@@", "8609");
        }
        // Reko: a decoder for the instruction 2090 at address 52A0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2090()
        {
            AssertCode("@@@", "2090");
        }
        // Reko: a decoder for the instruction 309F at address 52A8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_309F()
        {
            AssertCode("@@@", "309F");
        }
        // Reko: a decoder for the instruction 2631 at address 52B2 has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2631()
        {
            AssertCode("@@@", "2631");
        }
        // Reko: a decoder for the instruction 20A6 at address 52B4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20A6()
        {
            AssertCode("@@@", "20A6");
        }
        // Reko: a decoder for the instruction 4630 at address 52BA has not been implemented. (ZZ addb imm4/16,reg 4 dest reg 4 src imm 15/16 1/2)
        [Test]
        public void Cr16Dasm_4630()
        {
            AssertCode("@@@", "4630");
        }
        // Reko: a decoder for the instruction 20AA at address 52BC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20AA()
        {
            AssertCode("@@@", "20AA");
        }
        // Reko: a decoder for the instruction 8630 at address 52C2 has not been implemented. (ZZ addb imm4/16,reg 4 dest reg 4 src imm 15/16 1/2)
        [Test]
        public void Cr16Dasm_8630()
        {
            AssertCode("@@@", "8630");
        }
        // Reko: a decoder for the instruction 00AF at address 52C4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_00AF()
        {
            AssertCode("@@@", "00AF");
        }
        // Reko: a decoder for the instruction 0631 at address 52D0 has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0631()
        {
            AssertCode("@@@", "0631");
        }
        // Reko: a decoder for the instruction 60F0 at address 52DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60F0()
        {
            AssertCode("@@@", "60F0");
        }
        // Reko: a decoder for the instruction 6092 at address 52E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6092()
        {
            AssertCode("@@@", "6092");
        }
        // Reko: a decoder for the instruction 60A2 at address 52F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60A2()
        {
            AssertCode("@@@", "60A2");
        }
        // Reko: a decoder for the instruction 864B at address 52FA has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_864B()
        {
            AssertCode("@@@", "864B");
        }
        // Reko: a decoder for the instruction 6093 at address 530E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6093()
        {
            AssertCode("@@@", "6093");
        }
        // Reko: a decoder for the instruction 080A at address 533E has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_080A()
        {
            AssertCode("@@@", "080A");
        }
        // Reko: a decoder for the instruction D608 at address 5350 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_D608()
        {
            AssertCode("@@@", "D608");
        }
        // Reko: a decoder for the instruction FE09 at address 5356 has not been implemented. (Fmt9 1 ZZ  lshb cnt(right -), reg 4 dest reg 3 count imm)
        [Test]
        public void Cr16Dasm_FE09()
        {
            AssertCode("@@@", "FE09");
        }
        // Reko: a decoder for the instruction 6095 at address 536A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6095()
        {
            AssertCode("@@@", "6095");
        }
        // Reko: a decoder for the instruction 6098 at address 5384 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6098()
        {
            AssertCode("@@@", "6098");
        }
        // Reko: a decoder for the instruction 6D9A at address 53B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6D9A()
        {
            AssertCode("@@@", "6D9A");
        }
        // Reko: a decoder for the instruction 4DA4 at address 53B8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DA4()
        {
            AssertCode("@@@", "4DA4");
        }
        // Reko: a decoder for the instruction ADAA at address 53DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ADAA()
        {
            AssertCode("@@@", "ADAA");
        }
        // Reko: a decoder for the instruction 7098 at address 53E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7098()
        {
            AssertCode("@@@", "7098");
        }
        // Reko: a decoder for the instruction 2FD4 at address 53FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FD4()
        {
            AssertCode("@@@", "2FD4");
        }
        // Reko: a decoder for the instruction A208 at address 5424 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_A208()
        {
            AssertCode("@@@", "A208");
        }
        // Reko: a decoder for the instruction A63B at address 542C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A63B()
        {
            AssertCode("@@@", "A63B");
        }
        // Reko: a decoder for the instruction A1FC at address 549C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A1FC()
        {
            AssertCode("@@@", "A1FC");
        }
        // Reko: a decoder for the instruction 3F92 at address 549E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F92()
        {
            AssertCode("@@@", "3F92");
        }
        // Reko: a decoder for the instruction 4F94 at address 54A0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F94()
        {
            AssertCode("@@@", "4F94");
        }
        // Reko: a decoder for the instruction 3FD2 at address 54A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FD2()
        {
            AssertCode("@@@", "3FD2");
        }
        // Reko: a decoder for the instruction 42A6 at address 54AE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42A6()
        {
            AssertCode("@@@", "42A6");
        }
        // Reko: a decoder for the instruction 00AA at address 5514 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00AA()
        {
            AssertCode("@@@", "00AA");
        }
        // Reko: a decoder for the instruction 1854 at address 551A has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1854()
        {
            AssertCode("@@@", "1854");
        }
        // Reko: a decoder for the instruction 6861 at address 552A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_6861()
        {
            AssertCode("@@@", "6861");
        }
        // Reko: a decoder for the instruction 8DEA at address 552C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8DEA()
        {
            AssertCode("@@@", "8DEA");
        }
        // Reko: a decoder for the instruction 4008 at address 5552 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_4008()
        {
            AssertCode("@@@", "4008");
        }
        // Reko: a decoder for the instruction 7FFB at address 55BE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FFB()
        {
            AssertCode("@@@", "7FFB");
        }
        // Reko: a decoder for the instruction 51F8 at address 55D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_51F8()
        {
            AssertCode("@@@", "51F8");
        }
        // Reko: a decoder for the instruction 87FA at address 55DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_87FA()
        {
            AssertCode("@@@", "87FA");
        }
        // Reko: a decoder for the instruction 53FA at address 5610 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_53FA()
        {
            AssertCode("@@@", "53FA");
        }
        // Reko: a decoder for the instruction 4BFA at address 5618 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4BFA()
        {
            AssertCode("@@@", "4BFA");
        }
        // Reko: a decoder for the instruction 0D92 at address 5622 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D92()
        {
            AssertCode("@@@", "0D92");
        }
        // Reko: a decoder for the instruction C9F7 at address 562A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C9F7()
        {
            AssertCode("@@@", "C9F7");
        }
        // Reko: a decoder for the instruction 8BF8 at address 562E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8BF8()
        {
            AssertCode("@@@", "8BF8");
        }
        // Reko: a decoder for the instruction 6FF9 at address 5638 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FF9()
        {
            AssertCode("@@@", "6FF9");
        }
        // Reko: a decoder for the instruction D406 at address 563E has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_D406()
        {
            AssertCode("@@@", "D406");
        }
        // Reko: a decoder for the instruction 6052 at address 5640 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6052()
        {
            AssertCode("@@@", "6052");
        }
        // Reko: a decoder for the instruction 4208 at address 5644 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_4208()
        {
            AssertCode("@@@", "4208");
        }
        // Reko: a decoder for the instruction C000 at address 5648 has not been implemented. (Fmt11 1 ZZ  excp 4 vect imm)
        [Test]
        public void Cr16Dasm_C000()
        {
            AssertCode("@@@", "C000");
        }
        // Reko: a decoder for the instruction 5BF9 at address 564C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5BF9()
        {
            AssertCode("@@@", "5BF9");
        }
        // Reko: a decoder for the instruction 8406 at address 56C8 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_8406()
        {
            AssertCode("@@@", "8406");
        }
        // Reko: a decoder for the instruction 2FA4 at address 5718 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FA4()
        {
            AssertCode("@@@", "2FA4");
        }
        // Reko: a decoder for the instruction C3FA at address 574A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C3FA()
        {
            AssertCode("@@@", "C3FA");
        }
        // Reko: a decoder for the instruction F5FE at address 5762 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_F5FE()
        {
            AssertCode("@@@", "F5FE");
        }
        // Reko: a decoder for the instruction A3FA at address 576A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A3FA()
        {
            AssertCode("@@@", "A3FA");
        }
        // Reko: a decoder for the instruction FFF9 at address 5774 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FFF9()
        {
            AssertCode("@@@", "FFF9");
        }
        // Reko: a decoder for the instruction C3F6 at address 577E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C3F6()
        {
            AssertCode("@@@", "C3F6");
        }
        // Reko: a decoder for the instruction 8E07 at address 5792 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_8E07()
        {
            AssertCode("@@@", "8E07");
        }
        // Reko: a decoder for the instruction 1DF6 at address 5814 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DF6()
        {
            AssertCode("@@@", "1DF6");
        }
        // Reko: a decoder for the instruction BDFB at address 581C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BDFB()
        {
            AssertCode("@@@", "BDFB");
        }
        // Reko: a decoder for the instruction 60B2 at address 5826 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60B2()
        {
            AssertCode("@@@", "60B2");
        }
        // Reko: a decoder for the instruction 6851 at address 5828 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6851()
        {
            AssertCode("@@@", "6851");
        }
        // Reko: a decoder for the instruction 60B3 at address 582E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60B3()
        {
            AssertCode("@@@", "60B3");
        }
        // Reko: a decoder for the instruction 3060 at address 5836 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3060()
        {
            AssertCode("@@@", "3060");
        }
        // Reko: a decoder for the instruction A0B2 at address 5844 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B2()
        {
            AssertCode("@@@", "A0B2");
        }
        // Reko: a decoder for the instruction A851 at address 5846 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_A851()
        {
            AssertCode("@@@", "A851");
        }
        // Reko: a decoder for the instruction F606 at address 584A has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_F606()
        {
            AssertCode("@@@", "F606");
        }
        // Reko: a decoder for the instruction A0B3 at address 584C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B3()
        {
            AssertCode("@@@", "A0B3");
        }
        // Reko: a decoder for the instruction FE06 at address 5852 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_FE06()
        {
            AssertCode("@@@", "FE06");
        }
        // Reko: a decoder for the instruction A0B4 at address 5854 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B4()
        {
            AssertCode("@@@", "A0B4");
        }
        // Reko: a decoder for the instruction EE06 at address 585A has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_EE06()
        {
            AssertCode("@@@", "EE06");
        }
        // Reko: a decoder for the instruction A0B5 at address 585C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B5()
        {
            AssertCode("@@@", "A0B5");
        }
        // Reko: a decoder for the instruction 0C07 at address 5862 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_0C07()
        {
            AssertCode("@@@", "0C07");
        }
        // Reko: a decoder for the instruction A0B6 at address 5864 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B6()
        {
            AssertCode("@@@", "A0B6");
        }
        // Reko: a decoder for the instruction FC06 at address 586A has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_FC06()
        {
            AssertCode("@@@", "FC06");
        }
        // Reko: a decoder for the instruction A0B7 at address 586C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B7()
        {
            AssertCode("@@@", "A0B7");
        }
        // Reko: a decoder for the instruction EC06 at address 5872 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_EC06()
        {
            AssertCode("@@@", "EC06");
        }
        // Reko: a decoder for the instruction A0B0 at address 5876 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B0()
        {
            AssertCode("@@@", "A0B0");
        }
        // Reko: a decoder for the instruction 8A51 at address 5878 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_8A51()
        {
            AssertCode("@@@", "8A51");
        }
        // Reko: a decoder for the instruction 0308 at address 587A has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_0308()
        {
            AssertCode("@@@", "0308");
        }
        // Reko: a decoder for the instruction AA08 at address 587E has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_AA08()
        {
            AssertCode("@@@", "AA08");
        }
        // Reko: a decoder for the instruction 3A21 at address 5880 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3A21()
        {
            AssertCode("@@@", "3A21");
        }
        // Reko: a decoder for the instruction D206 at address 5886 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_D206()
        {
            AssertCode("@@@", "D206");
        }
        // Reko: a decoder for the instruction A0B1 at address 5888 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0B1()
        {
            AssertCode("@@@", "A0B1");
        }
        // Reko: a decoder for the instruction 1060 at address 5890 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1060()
        {
            AssertCode("@@@", "1060");
        }
        // Reko: a decoder for the instruction 603B at address 5892 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_603B()
        {
            AssertCode("@@@", "603B");
        }
        // Reko: a decoder for the instruction 8A06 at address 58A2 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_8A06()
        {
            AssertCode("@@@", "8A06");
        }
        // Reko: a decoder for the instruction 1031 at address 58AE has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1031()
        {
            AssertCode("@@@", "1031");
        }
        // Reko: a decoder for the instruction 14C2 at address 58C0 has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_14C2()
        {
            AssertCode("@@@", "14C2");
        }
        // Reko: a decoder for the instruction 03B0 at address 58F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_03B0()
        {
            AssertCode("@@@", "03B0");
        }
        // Reko: a decoder for the instruction 0B3B at address 591A has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0B3B()
        {
            AssertCode("@@@", "0B3B");
        }
        // Reko: a decoder for the instruction B404 at address 5940 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_B404()
        {
            AssertCode("@@@", "B404");
        }
        // Reko: a decoder for the instruction 4DF8 at address 59C0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DF8()
        {
            AssertCode("@@@", "4DF8");
        }
        // Reko: a decoder for the instruction 80B0 at address 59E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80B0()
        {
            AssertCode("@@@", "80B0");
        }
        // Reko: a decoder for the instruction 60B1 at address 59E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60B1()
        {
            AssertCode("@@@", "60B1");
        }
        // Reko: a decoder for the instruction 3BFE at address 59EE has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_3BFE()
        {
            AssertCode("@@@", "3BFE");
        }
        // Reko: a decoder for the instruction 04C2 at address 5A04 has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_04C2()
        {
            AssertCode("@@@", "04C2");
        }
        // Reko: a decoder for the instruction A9F7 at address 5A64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A9F7()
        {
            AssertCode("@@@", "A9F7");
        }
        // Reko: a decoder for the instruction 0DF6 at address 5ABC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DF6()
        {
            AssertCode("@@@", "0DF6");
        }
        // Reko: a decoder for the instruction B7F5 at address 5AE8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B7F5()
        {
            AssertCode("@@@", "B7F5");
        }
        // Reko: a decoder for the instruction CDF5 at address 5B70 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CDF5()
        {
            AssertCode("@@@", "CDF5");
        }
        // Reko: a decoder for the instruction 1DD2 at address 5B80 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DD2()
        {
            AssertCode("@@@", "1DD2");
        }
        // Reko: a decoder for the instruction B022 at address 5B82 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B022()
        {
            AssertCode("@@@", "B022");
        }
        // Reko: a decoder for the instruction 2FF3 at address 5B8A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FF3()
        {
            AssertCode("@@@", "2FF3");
        }
        // Reko: a decoder for the instruction 7FF6 at address 5B8E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FF6()
        {
            AssertCode("@@@", "7FF6");
        }
        // Reko: a decoder for the instruction 77F4 at address 5C1C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_77F4()
        {
            AssertCode("@@@", "77F4");
        }
        // Reko: a decoder for the instruction 1DF5 at address 5C20 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DF5()
        {
            AssertCode("@@@", "1DF5");
        }
        // Reko: a decoder for the instruction 3082 at address 5CA4 has not been implemented. (Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_3082()
        {
            AssertCode("@@@", "3082");
        }
        // Reko: a decoder for the instruction 3DF4 at address 5D00 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DF4()
        {
            AssertCode("@@@", "3DF4");
        }
        // Reko: a decoder for the instruction 0BFB at address 5D0E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0BFB()
        {
            AssertCode("@@@", "0BFB");
        }
        // Reko: a decoder for the instruction F5EF at address 5D1C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F5EF()
        {
            AssertCode("@@@", "F5EF");
        }
        // Reko: a decoder for the instruction 3BF1 at address 5D2A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3BF1()
        {
            AssertCode("@@@", "3BF1");
        }
        // Reko: a decoder for the instruction 120000B06CFC at address 5D2C has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120000B06CFC()
        {
            AssertCode("@@@", "120000B06CFC");
        }
        // Reko: a decoder for the instruction 0CEC at address 5D32 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CEC()
        {
            AssertCode("@@@", "0CEC");
        }
        // Reko: a decoder for the instruction D7EF at address 5D3A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D7EF()
        {
            AssertCode("@@@", "D7EF");
        }
        // Reko: a decoder for the instruction C7F7 at address 5D42 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C7F7()
        {
            AssertCode("@@@", "C7F7");
        }
        // Reko: a decoder for the instruction 13F6 at address 5D48 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_13F6()
        {
            AssertCode("@@@", "13F6");
        }
        // Reko: a decoder for the instruction 89F9 at address 5D50 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_89F9()
        {
            AssertCode("@@@", "89F9");
        }
        // Reko: a decoder for the instruction 05F6 at address 5D56 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_05F6()
        {
            AssertCode("@@@", "05F6");
        }
        // Reko: a decoder for the instruction 120000B060FC at address 5D58 has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120000B060FC()
        {
            AssertCode("@@@", "120000B060FC");
        }
        // Reko: a decoder for the instruction ABEF at address 5D66 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ABEF()
        {
            AssertCode("@@@", "ABEF");
        }
        // Reko: a decoder for the instruction C5F0 at address 5D7C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C5F0()
        {
            AssertCode("@@@", "C5F0");
        }
        // Reko: a decoder for the instruction 3FFA at address 5DCE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FFA()
        {
            AssertCode("@@@", "3FFA");
        }
        // Reko: a decoder for the instruction 3FE2 at address 5DD2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FE2()
        {
            AssertCode("@@@", "3FE2");
        }
        // Reko: a decoder for the instruction D3FC at address 5DE4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D3FC()
        {
            AssertCode("@@@", "D3FC");
        }
        // Reko: a decoder for the instruction E3F2 at address 5DF0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E3F2()
        {
            AssertCode("@@@", "E3F2");
        }
        // Reko: a decoder for the instruction 59FB at address 5DF8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_59FB()
        {
            AssertCode("@@@", "59FB");
        }
        // Reko: a decoder for the instruction 80F0 at address 5DFA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80F0()
        {
            AssertCode("@@@", "80F0");
        }
        // Reko: a decoder for the instruction 6C94 at address 5E36 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6C94()
        {
            AssertCode("@@@", "6C94");
        }
        // Reko: a decoder for the instruction 6CA4 at address 5E46 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6CA4()
        {
            AssertCode("@@@", "6CA4");
        }
        // Reko: a decoder for the instruction 6C95 at address 5E58 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6C95()
        {
            AssertCode("@@@", "6C95");
        }
        // Reko: a decoder for the instruction 9FF0 at address 5E7C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FF0()
        {
            AssertCode("@@@", "9FF0");
        }
        // Reko: a decoder for the instruction 1DF1 at address 5E8A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DF1()
        {
            AssertCode("@@@", "1DF1");
        }
        // Reko: a decoder for the instruction 2DF0 at address 5E94 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DF0()
        {
            AssertCode("@@@", "2DF0");
        }
        // Reko: a decoder for the instruction 19F3 at address 5EE4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_19F3()
        {
            AssertCode("@@@", "19F3");
        }
        // Reko: a decoder for the instruction DBF2 at address 5EE8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DBF2()
        {
            AssertCode("@@@", "DBF2");
        }
        // Reko: a decoder for the instruction 59EF at address 5F0C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_59EF()
        {
            AssertCode("@@@", "59EF");
        }
        // Reko: a decoder for the instruction 49EF at address 5F1C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_49EF()
        {
            AssertCode("@@@", "49EF");
        }
        // Reko: a decoder for the instruction 81F8 at address 5F22 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_81F8()
        {
            AssertCode("@@@", "81F8");
        }
        // Reko: a decoder for the instruction 6BF2 at address 5F28 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6BF2()
        {
            AssertCode("@@@", "6BF2");
        }
        // Reko: a decoder for the instruction 7FF9 at address 5F2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FF9()
        {
            AssertCode("@@@", "7FF9");
        }
        // Reko: a decoder for the instruction C1FA at address 5F32 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C1FA()
        {
            AssertCode("@@@", "C1FA");
        }
        // Reko: a decoder for the instruction 51F9 at address 5F44 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_51F9()
        {
            AssertCode("@@@", "51F9");
        }
        // Reko: a decoder for the instruction 4060 at address 5F48 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4060()
        {
            AssertCode("@@@", "4060");
        }
        // Reko: a decoder for the instruction 49F9 at address 5F4C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_49F9()
        {
            AssertCode("@@@", "49F9");
        }
        // Reko: a decoder for the instruction 41F9 at address 5F54 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_41F9()
        {
            AssertCode("@@@", "41F9");
        }
        // Reko: a decoder for the instruction 3BF9 at address 5F5A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3BF9()
        {
            AssertCode("@@@", "3BF9");
        }
        // Reko: a decoder for the instruction 7060 at address 5F5E has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7060()
        {
            AssertCode("@@@", "7060");
        }
        // Reko: a decoder for the instruction 33F9 at address 5F62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_33F9()
        {
            AssertCode("@@@", "33F9");
        }
        // Reko: a decoder for the instruction 6060 at address 5F66 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6060()
        {
            AssertCode("@@@", "6060");
        }
        // Reko: a decoder for the instruction 2BF9 at address 5F6A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2BF9()
        {
            AssertCode("@@@", "2BF9");
        }
        // Reko: a decoder for the instruction 23F9 at address 5F72 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_23F9()
        {
            AssertCode("@@@", "23F9");
        }
        // Reko: a decoder for the instruction 1892 at address 5FA8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1892()
        {
            AssertCode("@@@", "1892");
        }
        // Reko: a decoder for the instruction 9452 at address 5FEA has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9452()
        {
            AssertCode("@@@", "9452");
        }
        // Reko: a decoder for the instruction B452 at address 5FEE has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B452()
        {
            AssertCode("@@@", "B452");
        }
        // Reko: a decoder for the instruction A108 at address 5FF6 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_A108()
        {
            AssertCode("@@@", "A108");
        }
        // Reko: a decoder for the instruction 689F at address 602A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_689F()
        {
            AssertCode("@@@", "689F");
        }
        // Reko: a decoder for the instruction 4A61 at address 6042 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4A61()
        {
            AssertCode("@@@", "4A61");
        }
        // Reko: a decoder for the instruction 1652 at address 6044 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1652()
        {
            AssertCode("@@@", "1652");
        }
        // Reko: a decoder for the instruction 34D0 at address 604C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_34D0()
        {
            AssertCode("@@@", "34D0");
        }
        // Reko: a decoder for the instruction 2D4C at address 60A4 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_2D4C()
        {
            AssertCode("@@@", "2D4C");
        }
        // Reko: a decoder for the instruction ED61 at address 60A6 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_ED61()
        {
            AssertCode("@@@", "ED61");
        }
        // Reko: a decoder for the instruction 675A at address 60BE has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_675A()
        {
            AssertCode("@@@", "675A");
        }
        // Reko: a decoder for the instruction D290 at address 60DA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D290()
        {
            AssertCode("@@@", "D290");
        }
        // Reko: a decoder for the instruction 6D53 at address 60DC has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6D53()
        {
            AssertCode("@@@", "6D53");
        }
        // Reko: a decoder for the instruction 643B at address 60E2 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_643B()
        {
            AssertCode("@@@", "643B");
        }
        // Reko: a decoder for the instruction 42D0 at address 60E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42D0()
        {
            AssertCode("@@@", "42D0");
        }
        // Reko: a decoder for the instruction 6433 at address 60F6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6433()
        {
            AssertCode("@@@", "6433");
        }
        // Reko: a decoder for the instruction D653 at address 610A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_D653()
        {
            AssertCode("@@@", "D653");
        }
        // Reko: a decoder for the instruction 2EA0 at address 6156 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2EA0()
        {
            AssertCode("@@@", "2EA0");
        }
        // Reko: a decoder for the instruction AE92 at address 6164 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AE92()
        {
            AssertCode("@@@", "AE92");
        }
        // Reko: a decoder for the instruction 2121 at address 6192 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2121()
        {
            AssertCode("@@@", "2121");
        }
        // Reko: a decoder for the instruction A133 at address 619C has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A133()
        {
            AssertCode("@@@", "A133");
        }
        // Reko: a decoder for the instruction 4EA4 at address 61AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4EA4()
        {
            AssertCode("@@@", "4EA4");
        }
        // Reko: a decoder for the instruction E8AF at address 61B6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E8AF()
        {
            AssertCode("@@@", "E8AF");
        }
        // Reko: a decoder for the instruction 68AF at address 61BA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_68AF()
        {
            AssertCode("@@@", "68AF");
        }
        // Reko: a decoder for the instruction 4EAF at address 61C2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4EAF()
        {
            AssertCode("@@@", "4EAF");
        }
        // Reko: a decoder for the instruction 1C91 at address 623C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1C91()
        {
            AssertCode("@@@", "1C91");
        }
        // Reko: a decoder for the instruction 3C94 at address 6242 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3C94()
        {
            AssertCode("@@@", "3C94");
        }
        // Reko: a decoder for the instruction 0225 at address 6258 has not been implemented. (Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0225()
        {
            AssertCode("@@@", "0225");
        }
        // Reko: a decoder for the instruction 0AEC at address 6268 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AEC()
        {
            AssertCode("@@@", "0AEC");
        }
        // Reko: a decoder for the instruction 8AAF at address 6274 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8AAF()
        {
            AssertCode("@@@", "8AAF");
        }
        // Reko: a decoder for the instruction 9D52 at address 627E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9D52()
        {
            AssertCode("@@@", "9D52");
        }
        // Reko: a decoder for the instruction F652 at address 628C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F652()
        {
            AssertCode("@@@", "F652");
        }
        // Reko: a decoder for the instruction C018 at address 628E has not been implemented. (Fmt21 1 ZZ  bra cond disp8 8 dest disp * 2 4 cond imm)
        [Test]
        public void Cr16Dasm_C018()
        {
            AssertCode("@@@", "C018");
        }
        // Reko: a decoder for the instruction 9332 at address 6294 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9332()
        {
            AssertCode("@@@", "9332");
        }
        // Reko: a decoder for the instruction 8352 at address 6296 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8352()
        {
            AssertCode("@@@", "8352");
        }
        // Reko: a decoder for the instruction 6FD1 at address 629C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FD1()
        {
            AssertCode("@@@", "6FD1");
        }
        // Reko: a decoder for the instruction 6333 at address 62A2 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6333()
        {
            AssertCode("@@@", "6333");
        }
        // Reko: a decoder for the instruction 7352 at address 62A4 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7352()
        {
            AssertCode("@@@", "7352");
        }
        // Reko: a decoder for the instruction A308 at address 62A6 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_A308()
        {
            AssertCode("@@@", "A308");
        }
        // Reko: a decoder for the instruction 8552 at address 62AE has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8552()
        {
            AssertCode("@@@", "8552");
        }
        // Reko: a decoder for the instruction BD52 at address 62C4 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BD52()
        {
            AssertCode("@@@", "BD52");
        }
        // Reko: a decoder for the instruction 8652 at address 62D0 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8652()
        {
            AssertCode("@@@", "8652");
        }
        // Reko: a decoder for the instruction 9FC3 at address 62E2 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_9FC3()
        {
            AssertCode("@@@", "9FC3");
        }
        // Reko: a decoder for the instruction 2AAF at address 62E6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2AAF()
        {
            AssertCode("@@@", "2AAF");
        }
        // Reko: a decoder for the instruction 0AEF at address 62FC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0AEF()
        {
            AssertCode("@@@", "0AEF");
        }
        // Reko: a decoder for the instruction A0E0 at address 6300 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A0E0()
        {
            AssertCode("@@@", "A0E0");
        }
        // Reko: a decoder for the instruction 08D2 at address 6306 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08D2()
        {
            AssertCode("@@@", "08D2");
        }
        // Reko: a decoder for the instruction 78DC at address 6308 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_78DC()
        {
            AssertCode("@@@", "78DC");
        }
        // Reko: a decoder for the instruction 0745 at address 6316 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0745()
        {
            AssertCode("@@@", "0745");
        }
        // Reko: a decoder for the instruction 7032 at address 6326 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7032()
        {
            AssertCode("@@@", "7032");
        }
        // Reko: a decoder for the instruction 335A at address 6342 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_335A()
        {
            AssertCode("@@@", "335A");
        }
        // Reko: a decoder for the instruction 255A at address 6354 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_255A()
        {
            AssertCode("@@@", "255A");
        }
        // Reko: a decoder for the instruction 6032 at address 638C has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6032()
        {
            AssertCode("@@@", "6032");
        }
        // Reko: a decoder for the instruction 0445 at address 6390 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0445()
        {
            AssertCode("@@@", "0445");
        }
        // Reko: a decoder for the instruction 48DF at address 6392 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_48DF()
        {
            AssertCode("@@@", "48DF");
        }
        // Reko: a decoder for the instruction 455A at address 639E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_455A()
        {
            AssertCode("@@@", "455A");
        }
        // Reko: a decoder for the instruction 244C at address 63AC has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_244C()
        {
            AssertCode("@@@", "244C");
        }
        // Reko: a decoder for the instruction 48E6 at address 63AE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48E6()
        {
            AssertCode("@@@", "48E6");
        }
        // Reko: a decoder for the instruction 3E00 at address 63CA has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3E00()
        {
            AssertCode("@@@", "3E00");
        }
        // Reko: a decoder for the instruction 5427 at address 63CC has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5427()
        {
            AssertCode("@@@", "5427");
        }
        // Reko: a decoder for the instruction 1427 at address 63DA has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1427()
        {
            AssertCode("@@@", "1427");
        }
        // Reko: a decoder for the instruction 0661 at address 63EC has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0661()
        {
            AssertCode("@@@", "0661");
        }
        // Reko: a decoder for the instruction 68EF at address 63EE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_68EF()
        {
            AssertCode("@@@", "68EF");
        }
        // Reko: a decoder for the instruction 6D5A at address 649E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6D5A()
        {
            AssertCode("@@@", "6D5A");
        }
        // Reko: a decoder for the instruction E9FD at address 64A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E9FD()
        {
            AssertCode("@@@", "E9FD");
        }
        // Reko: a decoder for the instruction 623B at address 64AA has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_623B()
        {
            AssertCode("@@@", "623B");
        }
        // Reko: a decoder for the instruction E3FD at address 64B2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E3FD()
        {
            AssertCode("@@@", "E3FD");
        }
        // Reko: a decoder for the instruction B632 at address 64B4 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B632()
        {
            AssertCode("@@@", "B632");
        }
        // Reko: a decoder for the instruction 275A at address 64BA has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_275A()
        {
            AssertCode("@@@", "275A");
        }
        // Reko: a decoder for the instruction D7FD at address 64BE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D7FD()
        {
            AssertCode("@@@", "D7FD");
        }
        // Reko: a decoder for the instruction 8AEF at address 64C6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8AEF()
        {
            AssertCode("@@@", "8AEF");
        }
        // Reko: a decoder for the instruction B9FD at address 64CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B9FD()
        {
            AssertCode("@@@", "B9FD");
        }
        // Reko: a decoder for the instruction 93FD at address 64E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_93FD()
        {
            AssertCode("@@@", "93FD");
        }
        // Reko: a decoder for the instruction FAFF at address 64EE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_FAFF()
        {
            AssertCode("@@@", "FAFF");
        }
        // Reko: a decoder for the instruction 120000B068FC at address 64F8 has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120000B068FC()
        {
            AssertCode("@@@", "120000B068FC");
        }
        // Reko: a decoder for the instruction 05B0 at address 652A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_05B0()
        {
            AssertCode("@@@", "05B0");
        }
        // Reko: a decoder for the instruction 02EC at address 654C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02EC()
        {
            AssertCode("@@@", "02EC");
        }
        // Reko: a decoder for the instruction 9752 at address 6562 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9752()
        {
            AssertCode("@@@", "9752");
        }
        // Reko: a decoder for the instruction F8C3 at address 659A has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_F8C3()
        {
            AssertCode("@@@", "F8C3");
        }
        // Reko: a decoder for the instruction 0080 at address 65A0 has not been implemented. (Fmt15 1 ZZ  res - undefined trap)
        [Test]
        public void Cr16Dasm_0080()
        {
            AssertCode("@@@", "0080");
        }
        // Reko: a decoder for the instruction FF7F at address 65A8 has not been implemented. (Fmt12 2 ZZ  tbitw abs20 20 dest abs 4 pos imm)
        [Test]
        public void Cr16Dasm_FF7F()
        {
            AssertCode("@@@", "FF7F");
        }
        // Reko: a decoder for the instruction 48EF at address 6656 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_48EF()
        {
            AssertCode("@@@", "48EF");
        }
        // Reko: a decoder for the instruction 8883 at address 666E has not been implemented. (Fmt16 2 ZZ  storb imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_8883()
        {
            AssertCode("@@@", "8883");
        }
        // Reko: a decoder for the instruction 2AEF at address 6706 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2AEF()
        {
            AssertCode("@@@", "2AEF");
        }
        // Reko: a decoder for the instruction 5FFE at address 6714 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_5FFE()
        {
            AssertCode("@@@", "5FFE");
        }
        // Reko: a decoder for the instruction 33FE at address 6728 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_33FE()
        {
            AssertCode("@@@", "33FE");
        }
        // Reko: a decoder for the instruction 84AF at address 6772 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_84AF()
        {
            AssertCode("@@@", "84AF");
        }
        // Reko: a decoder for the instruction 04AF at address 677C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_04AF()
        {
            AssertCode("@@@", "04AF");
        }
        // Reko: a decoder for the instruction D4AF at address 6786 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_D4AF()
        {
            AssertCode("@@@", "D4AF");
        }
        // Reko: a decoder for the instruction 04A0 at address 67E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04A0()
        {
            AssertCode("@@@", "04A0");
        }
        // Reko: a decoder for the instruction 0CE0 at address 67EA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CE0()
        {
            AssertCode("@@@", "0CE0");
        }
        // Reko: a decoder for the instruction 04A2 at address 67EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04A2()
        {
            AssertCode("@@@", "04A2");
        }
        // Reko: a decoder for the instruction 04A4 at address 67F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04A4()
        {
            AssertCode("@@@", "04A4");
        }
        // Reko: a decoder for the instruction 0CE4 at address 67F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CE4()
        {
            AssertCode("@@@", "0CE4");
        }
        // Reko: a decoder for the instruction 04A6 at address 67F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04A6()
        {
            AssertCode("@@@", "04A6");
        }
        // Reko: a decoder for the instruction 04A8 at address 67F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04A8()
        {
            AssertCode("@@@", "04A8");
        }
        // Reko: a decoder for the instruction 0CE8 at address 67FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CE8()
        {
            AssertCode("@@@", "0CE8");
        }
        // Reko: a decoder for the instruction 04AA at address 67FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04AA()
        {
            AssertCode("@@@", "04AA");
        }
        // Reko: a decoder for the instruction 04AC at address 6800 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04AC()
        {
            AssertCode("@@@", "04AC");
        }
        // Reko: a decoder for the instruction 24AF at address 681C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_24AF()
        {
            AssertCode("@@@", "24AF");
        }
        // Reko: a decoder for the instruction C8E0 at address 685E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C8E0()
        {
            AssertCode("@@@", "C8E0");
        }
        // Reko: a decoder for the instruction A8EF at address 694C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_A8EF()
        {
            AssertCode("@@@", "A8EF");
        }
        // Reko: a decoder for the instruction FC0A at address 6970 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_FC0A()
        {
            AssertCode("@@@", "FC0A");
        }
        // Reko: a decoder for the instruction 520A at address 6974 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_520A()
        {
            AssertCode("@@@", "520A");
        }
        // Reko: a decoder for the instruction 080B at address 697A has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_080B()
        {
            AssertCode("@@@", "080B");
        }
        // Reko: a decoder for the instruction 1000005A5802 at address 697E has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000005A5802()
        {
            AssertCode("@@@", "1000005A5802");
        }
        // Reko: a decoder for the instruction 03FE at address 699A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_03FE()
        {
            AssertCode("@@@", "03FE");
        }
        // Reko: a decoder for the instruction FBFD at address 69A2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FBFD()
        {
            AssertCode("@@@", "FBFD");
        }
        // Reko: a decoder for the instruction 0E54 at address 69A8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0E54()
        {
            AssertCode("@@@", "0E54");
        }
        // Reko: a decoder for the instruction 42AF at address 69BE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_42AF()
        {
            AssertCode("@@@", "42AF");
        }
        // Reko: a decoder for the instruction 64A0 at address 69C6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_64A0()
        {
            AssertCode("@@@", "64A0");
        }
        // Reko: a decoder for the instruction 1492 at address 69D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1492()
        {
            AssertCode("@@@", "1492");
        }
        // Reko: a decoder for the instruction E4EA at address 69E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E4EA()
        {
            AssertCode("@@@", "E4EA");
        }
        // Reko: a decoder for the instruction E2EA at address 69E2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E2EA()
        {
            AssertCode("@@@", "E2EA");
        }
        // Reko: a decoder for the instruction E2E4 at address 69E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E2E4()
        {
            AssertCode("@@@", "E2E4");
        }
        // Reko: a decoder for the instruction E2EC at address 69E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E2EC()
        {
            AssertCode("@@@", "E2EC");
        }
        // Reko: a decoder for the instruction 0494 at address 69E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0494()
        {
            AssertCode("@@@", "0494");
        }
        // Reko: a decoder for the instruction 1022 at address 69EE has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1022()
        {
            AssertCode("@@@", "1022");
        }
        // Reko: a decoder for the instruction 343F at address 69F8 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_343F()
        {
            AssertCode("@@@", "343F");
        }
        // Reko: a decoder for the instruction 04D2 at address 69FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04D2()
        {
            AssertCode("@@@", "04D2");
        }
        // Reko: a decoder for the instruction 04C3 at address 69FC has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_04C3()
        {
            AssertCode("@@@", "04C3");
        }
        // Reko: a decoder for the instruction 04D7 at address 6A08 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04D7()
        {
            AssertCode("@@@", "04D7");
        }
        // Reko: a decoder for the instruction 04EC at address 6A0C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04EC()
        {
            AssertCode("@@@", "04EC");
        }
        // Reko: a decoder for the instruction 04EF at address 6A0E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_04EF()
        {
            AssertCode("@@@", "04EF");
        }
        // Reko: a decoder for the instruction 14C3 at address 6A28 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_14C3()
        {
            AssertCode("@@@", "14C3");
        }
        // Reko: a decoder for the instruction 04DF at address 6A2E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_04DF()
        {
            AssertCode("@@@", "04DF");
        }
        // Reko: a decoder for the instruction C243 at address 6B2C has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_C243()
        {
            AssertCode("@@@", "C243");
        }
        // Reko: a decoder for the instruction 5232 at address 6B2E has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5232()
        {
            AssertCode("@@@", "5232");
        }
        // Reko: a decoder for the instruction 2F00 at address 6B34 has not been implemented. (Fmt23 3 ZZ  addd imm32, rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_2F00()
        {
            AssertCode("@@@", "2F00");
        }
        // Reko: a decoder for the instruction F722 at address 6B38 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F722()
        {
            AssertCode("@@@", "F722");
        }
        // Reko: a decoder for the instruction 7252 at address 6B40 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7252()
        {
            AssertCode("@@@", "7252");
        }
        // Reko: a decoder for the instruction 4AAF at address 6B50 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4AAF()
        {
            AssertCode("@@@", "4AAF");
        }
        // Reko: a decoder for the instruction CAD4 at address 6B70 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CAD4()
        {
            AssertCode("@@@", "CAD4");
        }
        // Reko: a decoder for the instruction 7ADF at address 6B72 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7ADF()
        {
            AssertCode("@@@", "7ADF");
        }
        // Reko: a decoder for the instruction 40A0 at address 6B8C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_40A0()
        {
            AssertCode("@@@", "40A0");
        }
        // Reko: a decoder for the instruction 4092 at address 6B92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4092()
        {
            AssertCode("@@@", "4092");
        }
        // Reko: a decoder for the instruction B432 at address 6B94 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B432()
        {
            AssertCode("@@@", "B432");
        }
        // Reko: a decoder for the instruction 20EA at address 6BAA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20EA()
        {
            AssertCode("@@@", "20EA");
        }
        // Reko: a decoder for the instruction 28EA at address 6BAC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28EA()
        {
            AssertCode("@@@", "28EA");
        }
        // Reko: a decoder for the instruction 28E4 at address 6BAE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28E4()
        {
            AssertCode("@@@", "28E4");
        }
        // Reko: a decoder for the instruction 28EC at address 6BB0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28EC()
        {
            AssertCode("@@@", "28EC");
        }
        // Reko: a decoder for the instruction 2094 at address 6BB2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2094()
        {
            AssertCode("@@@", "2094");
        }
        // Reko: a decoder for the instruction 1222 at address 6BB8 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1222()
        {
            AssertCode("@@@", "1222");
        }
        // Reko: a decoder for the instruction 20D2 at address 6BC4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20D2()
        {
            AssertCode("@@@", "20D2");
        }
        // Reko: a decoder for the instruction 20D7 at address 6BD2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20D7()
        {
            AssertCode("@@@", "20D7");
        }
        // Reko: a decoder for the instruction 20EC at address 6BD6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20EC()
        {
            AssertCode("@@@", "20EC");
        }
        // Reko: a decoder for the instruction 20EF at address 6BD8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_20EF()
        {
            AssertCode("@@@", "20EF");
        }
        // Reko: a decoder for the instruction 10C3 at address 6BF2 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_10C3()
        {
            AssertCode("@@@", "10C3");
        }
        // Reko: a decoder for the instruction 925A at address 6BF6 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_925A()
        {
            AssertCode("@@@", "925A");
        }
        // Reko: a decoder for the instruction 1200FC612855 at address 6C1C has not been implemented. (Fmt2 3 ZZ ope 6  loadb (rrp) disp20 4 src (rrp) 4 dest reg 20 src disp 4)
        [Test]
        public void Cr16Dasm_1200FC612855()
        {
            AssertCode("@@@", "1200FC612855");
        }
        // Reko: a decoder for the instruction 80E0 at address 6C74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80E0()
        {
            AssertCode("@@@", "80E0");
        }
        // Reko: a decoder for the instruction 7333 at address 6CA8 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7333()
        {
            AssertCode("@@@", "7333");
        }
        // Reko: a decoder for the instruction 2AD4 at address 6CBA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AD4()
        {
            AssertCode("@@@", "2AD4");
        }
        // Reko: a decoder for the instruction 733B at address 6CE8 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_733B()
        {
            AssertCode("@@@", "733B");
        }
        // Reko: a decoder for the instruction 42EF at address 6D0E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_42EF()
        {
            AssertCode("@@@", "42EF");
        }
        // Reko: a decoder for the instruction B652 at address 6D58 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B652()
        {
            AssertCode("@@@", "B652");
        }
        // Reko: a decoder for the instruction 5AC3 at address 6DB2 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_5AC3()
        {
            AssertCode("@@@", "5AC3");
        }
        // Reko: a decoder for the instruction FAC3 at address 6DB6 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_FAC3()
        {
            AssertCode("@@@", "FAC3");
        }
        // Reko: a decoder for the instruction 1E92 at address 6E54 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1E92()
        {
            AssertCode("@@@", "1E92");
        }
        // Reko: a decoder for the instruction 1000C41F1E9F at address 6E66 has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000C41F1E9F()
        {
            AssertCode("@@@", "1000C41F1E9F");
        }
        // Reko: a decoder for the instruction 1633 at address 6E70 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1633()
        {
            AssertCode("@@@", "1633");
        }
        // Reko: a decoder for the instruction 4248 at address 6E7A has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_4248()
        {
            AssertCode("@@@", "4248");
        }
        // Reko: a decoder for the instruction 2523 at address 6E7E has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2523()
        {
            AssertCode("@@@", "2523");
        }
        // Reko: a decoder for the instruction 1545 at address 6E80 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1545()
        {
            AssertCode("@@@", "1545");
        }
        // Reko: a decoder for the instruction 2EAF at address 6E84 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2EAF()
        {
            AssertCode("@@@", "2EAF");
        }
        // Reko: a decoder for the instruction 0EEF at address 6E8A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0EEF()
        {
            AssertCode("@@@", "0EEF");
        }
        // Reko: a decoder for the instruction 0EC3 at address 6E9E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0EC3()
        {
            AssertCode("@@@", "0EC3");
        }
        // Reko: a decoder for the instruction C8FF at address 6EB4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C8FF()
        {
            AssertCode("@@@", "C8FF");
        }
        // Reko: a decoder for the instruction 4FDC at address 6EB8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FDC()
        {
            AssertCode("@@@", "4FDC");
        }
        // Reko: a decoder for the instruction 0F96 at address 6EE2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F96()
        {
            AssertCode("@@@", "0F96");
        }
        // Reko: a decoder for the instruction A2A6 at address 6EFA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A2A6()
        {
            AssertCode("@@@", "A2A6");
        }
        // Reko: a decoder for the instruction A2A0 at address 6F02 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A2A0()
        {
            AssertCode("@@@", "A2A0");
        }
        // Reko: a decoder for the instruction 7292 at address 6F04 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7292()
        {
            AssertCode("@@@", "7292");
        }
        // Reko: a decoder for the instruction 7FDA at address 6F06 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FDA()
        {
            AssertCode("@@@", "7FDA");
        }
        // Reko: a decoder for the instruction 3F3F at address 6F0E has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3F3F()
        {
            AssertCode("@@@", "3F3F");
        }
        // Reko: a decoder for the instruction 403F at address 6F14 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_403F()
        {
            AssertCode("@@@", "403F");
        }
        // Reko: a decoder for the instruction C15A at address 6F18 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_C15A()
        {
            AssertCode("@@@", "C15A");
        }
        // Reko: a decoder for the instruction 7FD3 at address 6F1C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FD3()
        {
            AssertCode("@@@", "7FD3");
        }
        // Reko: a decoder for the instruction 8DAF at address 6F1E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8DAF()
        {
            AssertCode("@@@", "8DAF");
        }
        // Reko: a decoder for the instruction 0F9C at address 6F26 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F9C()
        {
            AssertCode("@@@", "0F9C");
        }
        // Reko: a decoder for the instruction 0FDD at address 6F2C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FDD()
        {
            AssertCode("@@@", "0FDD");
        }
        // Reko: a decoder for the instruction 2FDB at address 6F30 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FDB()
        {
            AssertCode("@@@", "2FDB");
        }
        // Reko: a decoder for the instruction 3F9A at address 6F32 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F9A()
        {
            AssertCode("@@@", "3F9A");
        }
        // Reko: a decoder for the instruction B454 at address 6F36 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B454()
        {
            AssertCode("@@@", "B454");
        }
        // Reko: a decoder for the instruction 4FEF at address 6F3C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4FEF()
        {
            AssertCode("@@@", "4FEF");
        }
        // Reko: a decoder for the instruction B554 at address 6F40 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B554()
        {
            AssertCode("@@@", "B554");
        }
        // Reko: a decoder for the instruction D561 at address 6F44 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D561()
        {
            AssertCode("@@@", "D561");
        }
        // Reko: a decoder for the instruction 5FEF at address 6F46 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5FEF()
        {
            AssertCode("@@@", "5FEF");
        }
        // Reko: a decoder for the instruction B354 at address 6F54 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B354()
        {
            AssertCode("@@@", "B354");
        }
        // Reko: a decoder for the instruction D461 at address 6F62 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D461()
        {
            AssertCode("@@@", "D461");
        }
        // Reko: a decoder for the instruction 214C at address 6F70 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_214C()
        {
            AssertCode("@@@", "214C");
        }
        // Reko: a decoder for the instruction 1461 at address 6F76 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_1461()
        {
            AssertCode("@@@", "1461");
        }
        // Reko: a decoder for the instruction BFD6 at address 6F86 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFD6()
        {
            AssertCode("@@@", "BFD6");
        }
        // Reko: a decoder for the instruction AFD6 at address 6F96 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFD6()
        {
            AssertCode("@@@", "AFD6");
        }
        // Reko: a decoder for the instruction CFD6 at address 6FA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CFD6()
        {
            AssertCode("@@@", "CFD6");
        }
        // Reko: a decoder for the instruction 5439 at address 6FAA has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5439()
        {
            AssertCode("@@@", "5439");
        }
        // Reko: a decoder for the instruction 4847 at address 6FAC has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_4847()
        {
            AssertCode("@@@", "4847");
        }
        // Reko: a decoder for the instruction 273B at address 6FAE has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_273B()
        {
            AssertCode("@@@", "273B");
        }
        // Reko: a decoder for the instruction 2DDF at address 6FB0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2DDF()
        {
            AssertCode("@@@", "2DDF");
        }
        // Reko: a decoder for the instruction 4D3F at address 6FBA has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4D3F()
        {
            AssertCode("@@@", "4D3F");
        }
        // Reko: a decoder for the instruction 2FA8 at address 6FBE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FA8()
        {
            AssertCode("@@@", "2FA8");
        }
        // Reko: a decoder for the instruction 4FA8 at address 6FCA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FA8()
        {
            AssertCode("@@@", "4FA8");
        }
        // Reko: a decoder for the instruction 4FE8 at address 6FCE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FE8()
        {
            AssertCode("@@@", "4FE8");
        }
        // Reko: a decoder for the instruction 1FD3 at address 6FD8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD3()
        {
            AssertCode("@@@", "1FD3");
        }
        // Reko: a decoder for the instruction 483F at address 6FDC has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_483F()
        {
            AssertCode("@@@", "483F");
        }
        // Reko: a decoder for the instruction 5252 at address 6FE2 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5252()
        {
            AssertCode("@@@", "5252");
        }
        // Reko: a decoder for the instruction 4CE6 at address 6FFC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CE6()
        {
            AssertCode("@@@", "4CE6");
        }
        // Reko: a decoder for the instruction 3CD8 at address 6FFE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3CD8()
        {
            AssertCode("@@@", "3CD8");
        }
        // Reko: a decoder for the instruction ACE0 at address 7000 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACE0()
        {
            AssertCode("@@@", "ACE0");
        }
        // Reko: a decoder for the instruction 2CD2 at address 7002 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CD2()
        {
            AssertCode("@@@", "2CD2");
        }
        // Reko: a decoder for the instruction 4F9B at address 700C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F9B()
        {
            AssertCode("@@@", "4F9B");
        }
        // Reko: a decoder for the instruction ACA6 at address 7014 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACA6()
        {
            AssertCode("@@@", "ACA6");
        }
        // Reko: a decoder for the instruction BC98 at address 7018 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BC98()
        {
            AssertCode("@@@", "BC98");
        }
        // Reko: a decoder for the instruction BFD3 at address 701A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFD3()
        {
            AssertCode("@@@", "BFD3");
        }
        // Reko: a decoder for the instruction 7C92 at address 701E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7C92()
        {
            AssertCode("@@@", "7C92");
        }
        // Reko: a decoder for the instruction 1D92 at address 702A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1D92()
        {
            AssertCode("@@@", "1D92");
        }
        // Reko: a decoder for the instruction 9DC3 at address 7046 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_9DC3()
        {
            AssertCode("@@@", "9DC3");
        }
        // Reko: a decoder for the instruction 5DC3 at address 7052 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_5DC3()
        {
            AssertCode("@@@", "5DC3");
        }
        // Reko: a decoder for the instruction 473F at address 7058 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_473F()
        {
            AssertCode("@@@", "473F");
        }
        // Reko: a decoder for the instruction 4F9C at address 705C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F9C()
        {
            AssertCode("@@@", "4F9C");
        }
        // Reko: a decoder for the instruction 6452 at address 705E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6452()
        {
            AssertCode("@@@", "6452");
        }
        // Reko: a decoder for the instruction D84B at address 7064 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_D84B()
        {
            AssertCode("@@@", "D84B");
        }
        // Reko: a decoder for the instruction AFE4 at address 7072 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFE4()
        {
            AssertCode("@@@", "AFE4");
        }
        // Reko: a decoder for the instruction 4A21 at address 707C has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4A21()
        {
            AssertCode("@@@", "4A21");
        }
        // Reko: a decoder for the instruction 2039 at address 7086 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2039()
        {
            AssertCode("@@@", "2039");
        }
        // Reko: a decoder for the instruction 0847 at address 7088 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_0847()
        {
            AssertCode("@@@", "0847");
        }
        // Reko: a decoder for the instruction 4021 at address 7098 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4021()
        {
            AssertCode("@@@", "4021");
        }
        // Reko: a decoder for the instruction F422 at address 70AC has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F422()
        {
            AssertCode("@@@", "F422");
        }
        // Reko: a decoder for the instruction 493F at address 70B4 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_493F()
        {
            AssertCode("@@@", "493F");
        }
        // Reko: a decoder for the instruction 4A3F at address 70CA has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4A3F()
        {
            AssertCode("@@@", "4A3F");
        }
        // Reko: a decoder for the instruction 0345 at address 70DA has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0345()
        {
            AssertCode("@@@", "0345");
        }
        // Reko: a decoder for the instruction 932A at address 70DC has not been implemented. (ZZ xorw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_932A()
        {
            AssertCode("@@@", "932A");
        }
        // Reko: a decoder for the instruction 40B0 at address 70E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_40B0()
        {
            AssertCode("@@@", "40B0");
        }
        // Reko: a decoder for the instruction 20B1 at address 70EA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20B1()
        {
            AssertCode("@@@", "20B1");
        }
        // Reko: a decoder for the instruction 1091 at address 70EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1091()
        {
            AssertCode("@@@", "1091");
        }
        // Reko: a decoder for the instruction 7048 at address 7118 has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_7048()
        {
            AssertCode("@@@", "7048");
        }
        // Reko: a decoder for the instruction 8732 at address 711C has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8732()
        {
            AssertCode("@@@", "8732");
        }
        // Reko: a decoder for the instruction 8FD6 at address 711E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FD6()
        {
            AssertCode("@@@", "8FD6");
        }
        // Reko: a decoder for the instruction B0B0 at address 712A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B0B0()
        {
            AssertCode("@@@", "B0B0");
        }
        // Reko: a decoder for the instruction 3239 at address 7156 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3239()
        {
            AssertCode("@@@", "3239");
        }
        // Reko: a decoder for the instruction 2847 at address 7158 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_2847()
        {
            AssertCode("@@@", "2847");
        }
        // Reko: a decoder for the instruction 673B at address 715A has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_673B()
        {
            AssertCode("@@@", "673B");
        }
        // Reko: a decoder for the instruction 3033 at address 715C has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3033()
        {
            AssertCode("@@@", "3033");
        }
        // Reko: a decoder for the instruction C20F at address 716C has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_C20F()
        {
            AssertCode("@@@", "C20F");
        }
        // Reko: a decoder for the instruction 4B3F at address 717A has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4B3F()
        {
            AssertCode("@@@", "4B3F");
        }
        // Reko: a decoder for the instruction 4C3F at address 7188 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4C3F()
        {
            AssertCode("@@@", "4C3F");
        }
        // Reko: a decoder for the instruction 1F9B at address 7196 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F9B()
        {
            AssertCode("@@@", "1F9B");
        }
        // Reko: a decoder for the instruction 213B at address 7198 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_213B()
        {
            AssertCode("@@@", "213B");
        }
        // Reko: a decoder for the instruction 7A0F at address 71A2 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_7A0F()
        {
            AssertCode("@@@", "7A0F");
        }
        // Reko: a decoder for the instruction 260E at address 71AE has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_260E()
        {
            AssertCode("@@@", "260E");
        }
        // Reko: a decoder for the instruction 1C0E at address 71B8 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_1C0E()
        {
            AssertCode("@@@", "1C0E");
        }
        // Reko: a decoder for the instruction 513F at address 71C2 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_513F()
        {
            AssertCode("@@@", "513F");
        }
        // Reko: a decoder for the instruction AFA8 at address 71CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFA8()
        {
            AssertCode("@@@", "AFA8");
        }
        // Reko: a decoder for the instruction ACE6 at address 71D0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACE6()
        {
            AssertCode("@@@", "ACE6");
        }
        // Reko: a decoder for the instruction BF93 at address 71D2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BF93()
        {
            AssertCode("@@@", "BF93");
        }
        // Reko: a decoder for the instruction BCD8 at address 71D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BCD8()
        {
            AssertCode("@@@", "BCD8");
        }
        // Reko: a decoder for the instruction BF92 at address 71DA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BF92()
        {
            AssertCode("@@@", "BF92");
        }
        // Reko: a decoder for the instruction BCD2 at address 71DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BCD2()
        {
            AssertCode("@@@", "BCD2");
        }
        // Reko: a decoder for the instruction 7F9B at address 71EE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F9B()
        {
            AssertCode("@@@", "7F9B");
        }
        // Reko: a decoder for the instruction AF93 at address 71F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AF93()
        {
            AssertCode("@@@", "AF93");
        }
        // Reko: a decoder for the instruction 503F at address 71FA has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_503F()
        {
            AssertCode("@@@", "503F");
        }
        // Reko: a decoder for the instruction BF9C at address 71FE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BF9C()
        {
            AssertCode("@@@", "BF9C");
        }
        // Reko: a decoder for the instruction 4B52 at address 7200 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4B52()
        {
            AssertCode("@@@", "4B52");
        }
        // Reko: a decoder for the instruction B008 at address 720C has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_B008()
        {
            AssertCode("@@@", "B008");
        }
        // Reko: a decoder for the instruction BF9B at address 7212 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BF9B()
        {
            AssertCode("@@@", "BF9B");
        }
        // Reko: a decoder for the instruction 7F93 at address 7214 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F93()
        {
            AssertCode("@@@", "7F93");
        }
        // Reko: a decoder for the instruction 0145 at address 7234 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0145()
        {
            AssertCode("@@@", "0145");
        }
        // Reko: a decoder for the instruction 1B53 at address 7242 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1B53()
        {
            AssertCode("@@@", "1B53");
        }
        // Reko: a decoder for the instruction 6FA8 at address 725E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FA8()
        {
            AssertCode("@@@", "6FA8");
        }
        // Reko: a decoder for the instruction 9F9A at address 72A2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9F9A()
        {
            AssertCode("@@@", "9F9A");
        }
        // Reko: a decoder for the instruction B93B at address 72A6 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B93B()
        {
            AssertCode("@@@", "B93B");
        }
        // Reko: a decoder for the instruction 8F9B at address 72A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8F9B()
        {
            AssertCode("@@@", "8F9B");
        }
        // Reko: a decoder for the instruction 0DB8 at address 72C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DB8()
        {
            AssertCode("@@@", "0DB8");
        }
        // Reko: a decoder for the instruction 4CA6 at address 72D6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CA6()
        {
            AssertCode("@@@", "4CA6");
        }
        // Reko: a decoder for the instruction 0D96 at address 72DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D96()
        {
            AssertCode("@@@", "0D96");
        }
        // Reko: a decoder for the instruction 0D93 at address 72F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D93()
        {
            AssertCode("@@@", "0D93");
        }
        // Reko: a decoder for the instruction 423F at address 7312 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_423F()
        {
            AssertCode("@@@", "423F");
        }
        // Reko: a decoder for the instruction 9827 at address 7326 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9827()
        {
            AssertCode("@@@", "9827");
        }
        // Reko: a decoder for the instruction AF9C at address 7330 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AF9C()
        {
            AssertCode("@@@", "AF9C");
        }
        // Reko: a decoder for the instruction 4A52 at address 7332 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4A52()
        {
            AssertCode("@@@", "4A52");
        }
        // Reko: a decoder for the instruction ABFB at address 733A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ABFB()
        {
            AssertCode("@@@", "ABFB");
        }
        // Reko: a decoder for the instruction BF96 at address 733C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BF96()
        {
            AssertCode("@@@", "BF96");
        }
        // Reko: a decoder for the instruction A3FB at address 7342 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A3FB()
        {
            AssertCode("@@@", "A3FB");
        }
        // Reko: a decoder for the instruction 99FB at address 734C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_99FB()
        {
            AssertCode("@@@", "99FB");
        }
        // Reko: a decoder for the instruction AC92 at address 7370 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AC92()
        {
            AssertCode("@@@", "AC92");
        }
        // Reko: a decoder for the instruction AFD4 at address 7372 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFD4()
        {
            AssertCode("@@@", "AFD4");
        }
        // Reko: a decoder for the instruction BFD5 at address 7376 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFD5()
        {
            AssertCode("@@@", "BFD5");
        }
        // Reko: a decoder for the instruction 155A at address 7380 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_155A()
        {
            AssertCode("@@@", "155A");
        }
        // Reko: a decoder for the instruction 8FFE at address 739A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_8FFE()
        {
            AssertCode("@@@", "8FFE");
        }
        // Reko: a decoder for the instruction 523F at address 739E has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_523F()
        {
            AssertCode("@@@", "523F");
        }
        // Reko: a decoder for the instruction 3BFB at address 73AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3BFB()
        {
            AssertCode("@@@", "3BFB");
        }
        // Reko: a decoder for the instruction 7FD4 at address 73CC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FD4()
        {
            AssertCode("@@@", "7FD4");
        }
        // Reko: a decoder for the instruction AC98 at address 73CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AC98()
        {
            AssertCode("@@@", "AC98");
        }
        // Reko: a decoder for the instruction AFD5 at address 73D0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFD5()
        {
            AssertCode("@@@", "AFD5");
        }
        // Reko: a decoder for the instruction CFFE at address 73D6 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_CFFE()
        {
            AssertCode("@@@", "CFFE");
        }
        // Reko: a decoder for the instruction D9FC at address 73E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D9FC()
        {
            AssertCode("@@@", "D9FC");
        }
        // Reko: a decoder for the instruction 0FDF at address 73EE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0FDF()
        {
            AssertCode("@@@", "0FDF");
        }
        // Reko: a decoder for the instruction 1FDF at address 73FC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1FDF()
        {
            AssertCode("@@@", "1FDF");
        }
        // Reko: a decoder for the instruction 0E5A at address 7400 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0E5A()
        {
            AssertCode("@@@", "0E5A");
        }
        // Reko: a decoder for the instruction 6FD6 at address 7402 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FD6()
        {
            AssertCode("@@@", "6FD6");
        }
        // Reko: a decoder for the instruction 6F92 at address 7404 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6F92()
        {
            AssertCode("@@@", "6F92");
        }
        // Reko: a decoder for the instruction 2FE4 at address 7414 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FE4()
        {
            AssertCode("@@@", "2FE4");
        }
        // Reko: a decoder for the instruction 942A at address 741E has not been implemented. (ZZ xorw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_942A()
        {
            AssertCode("@@@", "942A");
        }
        // Reko: a decoder for the instruction 50B1 at address 742A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_50B1()
        {
            AssertCode("@@@", "50B1");
        }
        // Reko: a decoder for the instruction 0091 at address 742C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0091()
        {
            AssertCode("@@@", "0091");
        }
        // Reko: a decoder for the instruction 860B at address 7444 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_860B()
        {
            AssertCode("@@@", "860B");
        }
        // Reko: a decoder for the instruction F052 at address 746E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F052()
        {
            AssertCode("@@@", "F052");
        }
        // Reko: a decoder for the instruction AC0D at address 7472 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_AC0D()
        {
            AssertCode("@@@", "AC0D");
        }
        // Reko: a decoder for the instruction B460 at address 7488 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B460()
        {
            AssertCode("@@@", "B460");
        }
        // Reko: a decoder for the instruction 1FD6 at address 7492 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD6()
        {
            AssertCode("@@@", "1FD6");
        }
        // Reko: a decoder for the instruction 5F9F at address 7496 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5F9F()
        {
            AssertCode("@@@", "5F9F");
        }
        // Reko: a decoder for the instruction A20C at address 74BC has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_A20C()
        {
            AssertCode("@@@", "A20C");
        }
        // Reko: a decoder for the instruction 1154 at address 74C2 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1154()
        {
            AssertCode("@@@", "1154");
        }
        // Reko: a decoder for the instruction A161 at address 74C4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A161()
        {
            AssertCode("@@@", "A161");
        }
        // Reko: a decoder for the instruction 825A at address 74D2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_825A()
        {
            AssertCode("@@@", "825A");
        }
        // Reko: a decoder for the instruction 640F at address 74DC has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_640F()
        {
            AssertCode("@@@", "640F");
        }
        // Reko: a decoder for the instruction 0AB1 at address 74F0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AB1()
        {
            AssertCode("@@@", "0AB1");
        }
        // Reko: a decoder for the instruction 2048 at address 74F6 has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_2048()
        {
            AssertCode("@@@", "2048");
        }
        // Reko: a decoder for the instruction 10007233B252 at address 74FC has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_10007233B252()
        {
            AssertCode("@@@", "10007233B252");
        }
        // Reko: a decoder for the instruction 3A0F at address 7506 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_3A0F()
        {
            AssertCode("@@@", "3A0F");
        }
        // Reko: a decoder for the instruction A00D at address 750C has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_A00D()
        {
            AssertCode("@@@", "A00D");
        }
        // Reko: a decoder for the instruction 3554 at address 7514 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3554()
        {
            AssertCode("@@@", "3554");
        }
        // Reko: a decoder for the instruction A561 at address 7516 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A561()
        {
            AssertCode("@@@", "A561");
        }
        // Reko: a decoder for the instruction 5FE4 at address 7518 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FE4()
        {
            AssertCode("@@@", "5FE4");
        }
        // Reko: a decoder for the instruction 0AB2 at address 751A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AB2()
        {
            AssertCode("@@@", "0AB2");
        }
        // Reko: a decoder for the instruction 100F at address 7530 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_100F()
        {
            AssertCode("@@@", "100F");
        }
        // Reko: a decoder for the instruction 760D at address 7536 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_760D()
        {
            AssertCode("@@@", "760D");
        }
        // Reko: a decoder for the instruction 4054 at address 7540 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4054()
        {
            AssertCode("@@@", "4054");
        }
        // Reko: a decoder for the instruction 0AB3 at address 7546 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AB3()
        {
            AssertCode("@@@", "0AB3");
        }
        // Reko: a decoder for the instruction 61FC at address 7570 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_61FC()
        {
            AssertCode("@@@", "61FC");
        }
        // Reko: a decoder for the instruction 7043 at address 7586 has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_7043()
        {
            AssertCode("@@@", "7043");
        }
        // Reko: a decoder for the instruction 12C3 at address 758E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_12C3()
        {
            AssertCode("@@@", "12C3");
        }
        // Reko: a decoder for the instruction AF9D at address 75AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AF9D()
        {
            AssertCode("@@@", "AF9D");
        }
        // Reko: a decoder for the instruction 1A52 at address 75AC has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1A52()
        {
            AssertCode("@@@", "1A52");
        }
        // Reko: a decoder for the instruction 000D at address 75B0 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_000D()
        {
            AssertCode("@@@", "000D");
        }
        // Reko: a decoder for the instruction 715A at address 75BA has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_715A()
        {
            AssertCode("@@@", "715A");
        }
        // Reko: a decoder for the instruction 7123 at address 75BC has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7123()
        {
            AssertCode("@@@", "7123");
        }
        // Reko: a decoder for the instruction 1039 at address 75C0 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1039()
        {
            AssertCode("@@@", "1039");
        }
        // Reko: a decoder for the instruction B722 at address 75C4 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B722()
        {
            AssertCode("@@@", "B722");
        }
        // Reko: a decoder for the instruction 4E3F at address 75CA has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4E3F()
        {
            AssertCode("@@@", "4E3F");
        }
        // Reko: a decoder for the instruction 7E0B at address 75D6 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7E0B()
        {
            AssertCode("@@@", "7E0B");
        }
        // Reko: a decoder for the instruction 840D at address 75E6 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_840D()
        {
            AssertCode("@@@", "840D");
        }
        // Reko: a decoder for the instruction 2AB0 at address 75F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AB0()
        {
            AssertCode("@@@", "2AB0");
        }
        // Reko: a decoder for the instruction 7248 at address 75F8 has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_7248()
        {
            AssertCode("@@@", "7248");
        }
        // Reko: a decoder for the instruction 845A at address 75FC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_845A()
        {
            AssertCode("@@@", "845A");
        }
        // Reko: a decoder for the instruction 7433 at address 75FE has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7433()
        {
            AssertCode("@@@", "7433");
        }
        // Reko: a decoder for the instruction 480E at address 760C has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_480E()
        {
            AssertCode("@@@", "480E");
        }
        // Reko: a decoder for the instruction 2554 at address 7610 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2554()
        {
            AssertCode("@@@", "2554");
        }
        // Reko: a decoder for the instruction 2AB1 at address 7616 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AB1()
        {
            AssertCode("@@@", "2AB1");
        }
        // Reko: a decoder for the instruction 10007433B452 at address 7622 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_10007433B452()
        {
            AssertCode("@@@", "10007433B452");
        }
        // Reko: a decoder for the instruction 220E at address 7632 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_220E()
        {
            AssertCode("@@@", "220E");
        }
        // Reko: a decoder for the instruction 3254 at address 763A has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3254()
        {
            AssertCode("@@@", "3254");
        }
        // Reko: a decoder for the instruction 2AB2 at address 7640 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AB2()
        {
            AssertCode("@@@", "2AB2");
        }
        // Reko: a decoder for the instruction F00F at address 7656 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_F00F()
        {
            AssertCode("@@@", "F00F");
        }
        // Reko: a decoder for the instruction F80D at address 765C has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_F80D()
        {
            AssertCode("@@@", "F80D");
        }
        // Reko: a decoder for the instruction 4554 at address 7666 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4554()
        {
            AssertCode("@@@", "4554");
        }
        // Reko: a decoder for the instruction 2AB3 at address 766C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2AB3()
        {
            AssertCode("@@@", "2AB3");
        }
        // Reko: a decoder for the instruction 6F9B at address 767A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6F9B()
        {
            AssertCode("@@@", "6F9B");
        }
        // Reko: a decoder for the instruction 2CEA at address 7686 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CEA()
        {
            AssertCode("@@@", "2CEA");
        }
        // Reko: a decoder for the instruction 2DEA at address 768C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DEA()
        {
            AssertCode("@@@", "2DEA");
        }
        // Reko: a decoder for the instruction 415A at address 768E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_415A()
        {
            AssertCode("@@@", "415A");
        }
        // Reko: a decoder for the instruction 2DA8 at address 76AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DA8()
        {
            AssertCode("@@@", "2DA8");
        }
        // Reko: a decoder for the instruction 6A0F at address 76C8 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_6A0F()
        {
            AssertCode("@@@", "6A0F");
        }
        // Reko: a decoder for the instruction 1D96 at address 76CA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1D96()
        {
            AssertCode("@@@", "1D96");
        }
        // Reko: a decoder for the instruction 844D at address 76D4 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_844D()
        {
            AssertCode("@@@", "844D");
        }
        // Reko: a decoder for the instruction 824A at address 76D6 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_824A()
        {
            AssertCode("@@@", "824A");
        }
        // Reko: a decoder for the instruction 844B at address 76DC has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_844B()
        {
            AssertCode("@@@", "844B");
        }
        // Reko: a decoder for the instruction 844C at address 76E8 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_844C()
        {
            AssertCode("@@@", "844C");
        }
        // Reko: a decoder for the instruction BFDB at address 7708 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFDB()
        {
            AssertCode("@@@", "BFDB");
        }
        // Reko: a decoder for the instruction BFFA at address 7712 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFFA()
        {
            AssertCode("@@@", "BFFA");
        }
        // Reko: a decoder for the instruction F752 at address 7714 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F752()
        {
            AssertCode("@@@", "F752");
        }
        // Reko: a decoder for the instruction 280E at address 7718 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_280E()
        {
            AssertCode("@@@", "280E");
        }
        // Reko: a decoder for the instruction 3E0A at address 7720 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_3E0A()
        {
            AssertCode("@@@", "3E0A");
        }
        // Reko: a decoder for the instruction 1354 at address 7726 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1354()
        {
            AssertCode("@@@", "1354");
        }
        // Reko: a decoder for the instruction 3FE4 at address 772A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FE4()
        {
            AssertCode("@@@", "3FE4");
        }
        // Reko: a decoder for the instruction F452 at address 773A has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F452()
        {
            AssertCode("@@@", "F452");
        }
        // Reko: a decoder for the instruction 100D at address 7744 has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_100D()
        {
            AssertCode("@@@", "100D");
        }
        // Reko: a decoder for the instruction B532 at address 7748 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B532()
        {
            AssertCode("@@@", "B532");
        }
        // Reko: a decoder for the instruction 5FD2 at address 774C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FD2()
        {
            AssertCode("@@@", "5FD2");
        }
        // Reko: a decoder for the instruction 2154 at address 774E has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2154()
        {
            AssertCode("@@@", "2154");
        }
        // Reko: a decoder for the instruction 10001D944122 at address 7760 has not been implemented. (Fmt2 3 ZZ ope 9  sbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_10001D944122()
        {
            AssertCode("@@@", "10001D944122");
        }
        // Reko: a decoder for the instruction D609 at address 776A has not been implemented. (Fmt9 1 ZZ  lshb cnt(right -), reg 4 dest reg 3 count imm)
        [Test]
        public void Cr16Dasm_D609()
        {
            AssertCode("@@@", "D609");
        }
        // Reko: a decoder for the instruction C809 at address 7778 has not been implemented. (Fmt9 1 ZZ  lshb cnt(right -), reg 4 dest reg 3 count imm)
        [Test]
        public void Cr16Dasm_C809()
        {
            AssertCode("@@@", "C809");
        }
        // Reko: a decoder for the instruction 2752 at address 7782 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2752()
        {
            AssertCode("@@@", "2752");
        }
        // Reko: a decoder for the instruction BA0E at address 7786 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_BA0E()
        {
            AssertCode("@@@", "BA0E");
        }
        // Reko: a decoder for the instruction C00E at address 778E has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_C00E()
        {
            AssertCode("@@@", "C00E");
        }
        // Reko: a decoder for the instruction 9A32 at address 7792 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9A32()
        {
            AssertCode("@@@", "9A32");
        }
        // Reko: a decoder for the instruction AFD2 at address 7794 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFD2()
        {
            AssertCode("@@@", "AFD2");
        }
        // Reko: a decoder for the instruction 23B0 at address 779C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_23B0()
        {
            AssertCode("@@@", "23B0");
        }
        // Reko: a decoder for the instruction 8123 at address 77AA has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8123()
        {
            AssertCode("@@@", "8123");
        }
        // Reko: a decoder for the instruction 1DD3 at address 77AC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DD3()
        {
            AssertCode("@@@", "1DD3");
        }
        // Reko: a decoder for the instruction F24B at address 77B0 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F24B()
        {
            AssertCode("@@@", "F24B");
        }
        // Reko: a decoder for the instruction 3222 at address 77B2 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3222()
        {
            AssertCode("@@@", "3222");
        }
        // Reko: a decoder for the instruction 3252 at address 77BA has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3252()
        {
            AssertCode("@@@", "3252");
        }
        // Reko: a decoder for the instruction 7FF8 at address 77C2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FF8()
        {
            AssertCode("@@@", "7FF8");
        }
        // Reko: a decoder for the instruction 413F at address 77C6 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_413F()
        {
            AssertCode("@@@", "413F");
        }
        // Reko: a decoder for the instruction 4E0D at address 77EC has not been implemented. (Fmt15 1 ZZ  bne0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_4E0D()
        {
            AssertCode("@@@", "4E0D");
        }
        // Reko: a decoder for the instruction 1FA4 at address 77F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FA4()
        {
            AssertCode("@@@", "1FA4");
        }
        // Reko: a decoder for the instruction 9C0C at address 780A has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_9C0C()
        {
            AssertCode("@@@", "9C0C");
        }
        // Reko: a decoder for the instruction 380C at address 7810 has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_380C()
        {
            AssertCode("@@@", "380C");
        }
        // Reko: a decoder for the instruction 2A60 at address 781A has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2A60()
        {
            AssertCode("@@@", "2A60");
        }
        // Reko: a decoder for the instruction 720C at address 7834 has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_720C()
        {
            AssertCode("@@@", "720C");
        }
        // Reko: a decoder for the instruction 0E0C at address 783A has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_0E0C()
        {
            AssertCode("@@@", "0E0C");
        }
        // Reko: a decoder for the instruction 3A60 at address 7844 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3A60()
        {
            AssertCode("@@@", "3A60");
        }
        // Reko: a decoder for the instruction 04B2 at address 7846 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B2()
        {
            AssertCode("@@@", "04B2");
        }
        // Reko: a decoder for the instruction 4A0C at address 785C has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_4A0C()
        {
            AssertCode("@@@", "4A0C");
        }
        // Reko: a decoder for the instruction E60B at address 7862 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_E60B()
        {
            AssertCode("@@@", "E60B");
        }
        // Reko: a decoder for the instruction BA32 at address 7866 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BA32()
        {
            AssertCode("@@@", "BA32");
        }
        // Reko: a decoder for the instruction 4A60 at address 786E has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4A60()
        {
            AssertCode("@@@", "4A60");
        }
        // Reko: a decoder for the instruction 04B3 at address 7870 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B3()
        {
            AssertCode("@@@", "04B3");
        }
        // Reko: a decoder for the instruction 004B at address 7880 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_004B()
        {
            AssertCode("@@@", "004B");
        }
        // Reko: a decoder for the instruction 140082B00257 at address 788C has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140082B00257()
        {
            AssertCode("@@@", "140082B00257");
        }
        // Reko: a decoder for the instruction F00B at address 7894 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_F00B()
        {
            AssertCode("@@@", "F00B");
        }
        // Reko: a decoder for the instruction 25F9 at address 78AC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_25F9()
        {
            AssertCode("@@@", "25F9");
        }
        // Reko: a decoder for the instruction 07F9 at address 78CA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_07F9()
        {
            AssertCode("@@@", "07F9");
        }
        // Reko: a decoder for the instruction 443F at address 78CE has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_443F()
        {
            AssertCode("@@@", "443F");
        }
        // Reko: a decoder for the instruction D752 at address 78DA has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D752()
        {
            AssertCode("@@@", "D752");
        }
        // Reko: a decoder for the instruction 560C at address 78E4 has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_560C()
        {
            AssertCode("@@@", "560C");
        }
        // Reko: a decoder for the instruction D252 at address 78FC has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D252()
        {
            AssertCode("@@@", "D252");
        }
        // Reko: a decoder for the instruction 580E at address 7900 has not been implemented. (Fmt15 1 ZZ  beq0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_580E()
        {
            AssertCode("@@@", "580E");
        }
        // Reko: a decoder for the instruction 420B at address 7906 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_420B()
        {
            AssertCode("@@@", "420B");
        }
        // Reko: a decoder for the instruction 03B1 at address 7916 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_03B1()
        {
            AssertCode("@@@", "03B1");
        }
        // Reko: a decoder for the instruction 1000B25A1F00 at address 7922 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000B25A1F00()
        {
            AssertCode("@@@", "1000B25A1F00");
        }
        // Reko: a decoder for the instruction F322 at address 7944 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F322()
        {
            AssertCode("@@@", "F322");
        }
        // Reko: a decoder for the instruction 4332 at address 7946 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4332()
        {
            AssertCode("@@@", "4332");
        }
        // Reko: a decoder for the instruction F2FF at address 7950 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F2FF()
        {
            AssertCode("@@@", "F2FF");
        }
        // Reko: a decoder for the instruction 3807 at address 795C has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_3807()
        {
            AssertCode("@@@", "3807");
        }
        // Reko: a decoder for the instruction 2C07 at address 7968 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_2C07()
        {
            AssertCode("@@@", "2C07");
        }
        // Reko: a decoder for the instruction 4233 at address 7978 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4233()
        {
            AssertCode("@@@", "4233");
        }
        // Reko: a decoder for the instruction 1332 at address 7984 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1332()
        {
            AssertCode("@@@", "1332");
        }
        // Reko: a decoder for the instruction A60B at address 798E has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A60B()
        {
            AssertCode("@@@", "A60B");
        }
        // Reko: a decoder for the instruction B20B at address 7994 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B20B()
        {
            AssertCode("@@@", "B20B");
        }
        // Reko: a decoder for the instruction 4AB0 at address 799C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4AB0()
        {
            AssertCode("@@@", "4AB0");
        }
        // Reko: a decoder for the instruction 7448 at address 79A2 has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_7448()
        {
            AssertCode("@@@", "7448");
        }
        // Reko: a decoder for the instruction 4861 at address 79A4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4861()
        {
            AssertCode("@@@", "4861");
        }
        // Reko: a decoder for the instruction 7A5A at address 79B8 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7A5A()
        {
            AssertCode("@@@", "7A5A");
        }
        // Reko: a decoder for the instruction 8A23 at address 79BA has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8A23()
        {
            AssertCode("@@@", "8A23");
        }
        // Reko: a decoder for the instruction A4D0 at address 79BC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A4D0()
        {
            AssertCode("@@@", "A4D0");
        }
        // Reko: a decoder for the instruction 2E60 at address 79C8 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2E60()
        {
            AssertCode("@@@", "2E60");
        }
        // Reko: a decoder for the instruction 6453 at address 79CA has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6453()
        {
            AssertCode("@@@", "6453");
        }
        // Reko: a decoder for the instruction AE0C at address 79CE has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_AE0C()
        {
            AssertCode("@@@", "AE0C");
        }
        // Reko: a decoder for the instruction 3FD4 at address 79E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FD4()
        {
            AssertCode("@@@", "3FD4");
        }
        // Reko: a decoder for the instruction 1353 at address 79EA has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1353()
        {
            AssertCode("@@@", "1353");
        }
        // Reko: a decoder for the instruction 4DAC at address 79F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DAC()
        {
            AssertCode("@@@", "4DAC");
        }
        // Reko: a decoder for the instruction 2FE6 at address 79FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FE6()
        {
            AssertCode("@@@", "2FE6");
        }
        // Reko: a decoder for the instruction E4A6 at address 7A02 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E4A6()
        {
            AssertCode("@@@", "E4A6");
        }
        // Reko: a decoder for the instruction 2498 at address 7A08 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2498()
        {
            AssertCode("@@@", "2498");
        }
        // Reko: a decoder for the instruction 123B at address 7A0A has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_123B()
        {
            AssertCode("@@@", "123B");
        }
        // Reko: a decoder for the instruction 3499 at address 7A0C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3499()
        {
            AssertCode("@@@", "3499");
        }
        // Reko: a decoder for the instruction 233B at address 7A18 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_233B()
        {
            AssertCode("@@@", "233B");
        }
        // Reko: a decoder for the instruction 4022 at address 7A36 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4022()
        {
            AssertCode("@@@", "4022");
        }
        // Reko: a decoder for the instruction 4F92 at address 7A48 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F92()
        {
            AssertCode("@@@", "4F92");
        }
        // Reko: a decoder for the instruction 5F94 at address 7A4A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5F94()
        {
            AssertCode("@@@", "5F94");
        }
        // Reko: a decoder for the instruction 543B at address 7A4C has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_543B()
        {
            AssertCode("@@@", "543B");
        }
        // Reko: a decoder for the instruction 4FD2 at address 7A4E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FD2()
        {
            AssertCode("@@@", "4FD2");
        }
        // Reko: a decoder for the instruction 513B at address 7A58 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_513B()
        {
            AssertCode("@@@", "513B");
        }
        // Reko: a decoder for the instruction 100C at address 7A62 has not been implemented. (Fmt15 1 ZZ  beq0b disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_100C()
        {
            AssertCode("@@@", "100C");
        }
        // Reko: a decoder for the instruction 3A3F at address 7A6C has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3A3F()
        {
            AssertCode("@@@", "3A3F");
        }
        // Reko: a decoder for the instruction 42EA at address 7A82 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42EA()
        {
            AssertCode("@@@", "42EA");
        }
        // Reko: a decoder for the instruction 3B3F at address 7A8A has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3B3F()
        {
            AssertCode("@@@", "3B3F");
        }
        // Reko: a decoder for the instruction 3C3F at address 7AA6 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3C3F()
        {
            AssertCode("@@@", "3C3F");
        }
        // Reko: a decoder for the instruction CBFA at address 7AAC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CBFA()
        {
            AssertCode("@@@", "CBFA");
        }
        // Reko: a decoder for the instruction B608 at address 7AB4 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_B608()
        {
            AssertCode("@@@", "B608");
        }
        // Reko: a decoder for the instruction 0DAC at address 7AC6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DAC()
        {
            AssertCode("@@@", "0DAC");
        }
        // Reko: a decoder for the instruction 40AF at address 7ACC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_40AF()
        {
            AssertCode("@@@", "40AF");
        }
        // Reko: a decoder for the instruction 9033 at address 7AE2 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9033()
        {
            AssertCode("@@@", "9033");
        }
        // Reko: a decoder for the instruction 34F0 at address 7AEC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_34F0()
        {
            AssertCode("@@@", "34F0");
        }
        // Reko: a decoder for the instruction 2653 at address 7AF8 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2653()
        {
            AssertCode("@@@", "2653");
        }
        // Reko: a decoder for the instruction 3FFC at address 7B00 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FFC()
        {
            AssertCode("@@@", "3FFC");
        }
        // Reko: a decoder for the instruction 0FD2 at address 7B2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD2()
        {
            AssertCode("@@@", "0FD2");
        }
        // Reko: a decoder for the instruction 1FBC at address 7B30 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FBC()
        {
            AssertCode("@@@", "1FBC");
        }
        // Reko: a decoder for the instruction 3C0B at address 7B36 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3C0B()
        {
            AssertCode("@@@", "3C0B");
        }
        // Reko: a decoder for the instruction 1C06 at address 7B42 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_1C06()
        {
            AssertCode("@@@", "1C06");
        }
        // Reko: a decoder for the instruction 40AA at address 7B64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_40AA()
        {
            AssertCode("@@@", "40AA");
        }
        // Reko: a decoder for the instruction 4FBC at address 7BBC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FBC()
        {
            AssertCode("@@@", "4FBC");
        }
        // Reko: a decoder for the instruction BFFE at address 7BC8 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_BFFE()
        {
            AssertCode("@@@", "BFFE");
        }
        // Reko: a decoder for the instruction B80A at address 7BCE has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_B80A()
        {
            AssertCode("@@@", "B80A");
        }
        // Reko: a decoder for the instruction 20D4 at address 7C08 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20D4()
        {
            AssertCode("@@@", "20D4");
        }
        // Reko: a decoder for the instruction 824B at address 7C0C has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_824B()
        {
            AssertCode("@@@", "824B");
        }
        // Reko: a decoder for the instruction 20D5 at address 7C0E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20D5()
        {
            AssertCode("@@@", "20D5");
        }
        // Reko: a decoder for the instruction 4D96 at address 7C10 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4D96()
        {
            AssertCode("@@@", "4D96");
        }
        // Reko: a decoder for the instruction 4123 at address 7C18 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4123()
        {
            AssertCode("@@@", "4123");
        }
        // Reko: a decoder for the instruction 1D94 at address 7C1E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1D94()
        {
            AssertCode("@@@", "1D94");
        }
        // Reko: a decoder for the instruction 4122 at address 7C20 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4122()
        {
            AssertCode("@@@", "4122");
        }
        // Reko: a decoder for the instruction 8FF0 at address 7C26 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FF0()
        {
            AssertCode("@@@", "8FF0");
        }
        // Reko: a decoder for the instruction 884B at address 7C28 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_884B()
        {
            AssertCode("@@@", "884B");
        }
        // Reko: a decoder for the instruction 8FF1 at address 7C2A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FF1()
        {
            AssertCode("@@@", "8FF1");
        }
        // Reko: a decoder for the instruction 265A at address 7C2E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_265A()
        {
            AssertCode("@@@", "265A");
        }
        // Reko: a decoder for the instruction 383F at address 7C3E has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_383F()
        {
            AssertCode("@@@", "383F");
        }
        // Reko: a decoder for the instruction 4723 at address 7C46 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4723()
        {
            AssertCode("@@@", "4723");
        }
        // Reko: a decoder for the instruction EA07 at address 7C50 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_EA07()
        {
            AssertCode("@@@", "EA07");
        }
        // Reko: a decoder for the instruction 6A06 at address 7C58 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_6A06()
        {
            AssertCode("@@@", "6A06");
        }
        // Reko: a decoder for the instruction 02E6 at address 7C5C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02E6()
        {
            AssertCode("@@@", "02E6");
        }
        // Reko: a decoder for the instruction 393F at address 7C68 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_393F()
        {
            AssertCode("@@@", "393F");
        }
        // Reko: a decoder for the instruction 2DD2 at address 7C6A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DD2()
        {
            AssertCode("@@@", "2DD2");
        }
        // Reko: a decoder for the instruction 73FD at address 7C6E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_73FD()
        {
            AssertCode("@@@", "73FD");
        }
        // Reko: a decoder for the instruction 4FD6 at address 7C74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FD6()
        {
            AssertCode("@@@", "4FD6");
        }
        // Reko: a decoder for the instruction 59F5 at address 7C78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_59F5()
        {
            AssertCode("@@@", "59F5");
        }
        // Reko: a decoder for the instruction 0C0A at address 7C80 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_0C0A()
        {
            AssertCode("@@@", "0C0A");
        }
        // Reko: a decoder for the instruction E206 at address 7C88 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_E206()
        {
            AssertCode("@@@", "E206");
        }
        // Reko: a decoder for the instruction 80E2 at address 7CB6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80E2()
        {
            AssertCode("@@@", "80E2");
        }
        // Reko: a decoder for the instruction 804B at address 7CCE has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_804B()
        {
            AssertCode("@@@", "804B");
        }
        // Reko: a decoder for the instruction 0FF1 at address 7CD0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FF1()
        {
            AssertCode("@@@", "0FF1");
        }
        // Reko: a decoder for the instruction 9FF2 at address 7CD2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FF2()
        {
            AssertCode("@@@", "9FF2");
        }
        // Reko: a decoder for the instruction 0FF3 at address 7CD8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FF3()
        {
            AssertCode("@@@", "0FF3");
        }
        // Reko: a decoder for the instruction 465A at address 7CDC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_465A()
        {
            AssertCode("@@@", "465A");
        }
        // Reko: a decoder for the instruction 373F at address 7CE8 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_373F()
        {
            AssertCode("@@@", "373F");
        }
        // Reko: a decoder for the instruction DFFE at address 7CF4 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_DFFE()
        {
            AssertCode("@@@", "DFFE");
        }
        // Reko: a decoder for the instruction 0C0B at address 7D08 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0C0B()
        {
            AssertCode("@@@", "0C0B");
        }
        // Reko: a decoder for the instruction 5A06 at address 7D10 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_5A06()
        {
            AssertCode("@@@", "5A06");
        }
        // Reko: a decoder for the instruction 1A04 at address 7D1C has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_1A04()
        {
            AssertCode("@@@", "1A04");
        }
        // Reko: a decoder for the instruction 8DDF at address 7D38 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8DDF()
        {
            AssertCode("@@@", "8DDF");
        }
        // Reko: a decoder for the instruction 80D8 at address 7D42 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80D8()
        {
            AssertCode("@@@", "80D8");
        }
        // Reko: a decoder for the instruction 6007 at address 7D4E has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_6007()
        {
            AssertCode("@@@", "6007");
        }
        // Reko: a decoder for the instruction 0FFC at address 7D6A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FFC()
        {
            AssertCode("@@@", "0FFC");
        }
        // Reko: a decoder for the instruction 1200A411405F at address 7D6E has not been implemented. (Fmt2 3 ZZ ope 1  storb imm(rp) disp20 4 dest(rp) 4 src imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1200A411405F()
        {
            AssertCode("@@@", "1200A411405F");
        }
        // Reko: a decoder for the instruction 1300151F4DDF at address 7D90 has not been implemented. (Fmt2 3 ZZ ope 1  storw imm (rp) disp20 4 dest (rp) 4 src imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1300151F4DDF()
        {
            AssertCode("@@@", "1300151F4DDF");
        }
        // Reko: a decoder for the instruction 7DC3 at address 7DA4 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_7DC3()
        {
            AssertCode("@@@", "7DC3");
        }
        // Reko: a decoder for the instruction 1FAF at address 7DA8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1FAF()
        {
            AssertCode("@@@", "1FAF");
        }
        // Reko: a decoder for the instruction 2FAF at address 7DAE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2FAF()
        {
            AssertCode("@@@", "2FAF");
        }
        // Reko: a decoder for the instruction 3FAF at address 7DB4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3FAF()
        {
            AssertCode("@@@", "3FAF");
        }
        // Reko: a decoder for the instruction 13003FAF3400 at address 7DBC has not been implemented. (Fmt2 3 ZZ ope 10  stord (rrp) disp20 4 dest (rrp) 4 src rp 20 dest disp 4)
        [Test]
        public void Cr16Dasm_13003FAF3400()
        {
            AssertCode("@@@", "13003FAF3400");
        }
        // Reko: a decoder for the instruction D209 at address 7DCE has not been implemented. (Fmt9 1 ZZ  lshb cnt(right -), reg 4 dest reg 3 count imm)
        [Test]
        public void Cr16Dasm_D209()
        {
            AssertCode("@@@", "D209");
        }
        // Reko: a decoder for the instruction EBF3 at address 7DD6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EBF3()
        {
            AssertCode("@@@", "EBF3");
        }
        // Reko: a decoder for the instruction B5FC at address 7DDC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B5FC()
        {
            AssertCode("@@@", "B5FC");
        }
        // Reko: a decoder for the instruction E404 at address 7DE4 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_E404()
        {
            AssertCode("@@@", "E404");
        }
        // Reko: a decoder for the instruction C5F7 at address 7DF0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C5F7()
        {
            AssertCode("@@@", "C5F7");
        }
        // Reko: a decoder for the instruction 8BF3 at address 7DF8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8BF3()
        {
            AssertCode("@@@", "8BF3");
        }
        // Reko: a decoder for the instruction 0F93 at address 7E12 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F93()
        {
            AssertCode("@@@", "0F93");
        }
        // Reko: a decoder for the instruction 0FD6 at address 7E18 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD6()
        {
            AssertCode("@@@", "0FD6");
        }
        // Reko: a decoder for the instruction 180A at address 7E20 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_180A()
        {
            AssertCode("@@@", "180A");
        }
        // Reko: a decoder for the instruction 2FAA at address 7E2A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FAA()
        {
            AssertCode("@@@", "2FAA");
        }
        // Reko: a decoder for the instruction 5F98 at address 7E32 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5F98()
        {
            AssertCode("@@@", "5F98");
        }
        // Reko: a decoder for the instruction 1FD5 at address 7E40 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FD5()
        {
            AssertCode("@@@", "1FD5");
        }
        // Reko: a decoder for the instruction 2FEA at address 7E48 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FEA()
        {
            AssertCode("@@@", "2FEA");
        }
        // Reko: a decoder for the instruction 503B at address 7E50 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_503B()
        {
            AssertCode("@@@", "503B");
        }
        // Reko: a decoder for the instruction DFF1 at address 7E5C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFF1()
        {
            AssertCode("@@@", "DFF1");
        }
        // Reko: a decoder for the instruction 45F7 at address 7E68 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_45F7()
        {
            AssertCode("@@@", "45F7");
        }
        // Reko: a decoder for the instruction 433F at address 7E70 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_433F()
        {
            AssertCode("@@@", "433F");
        }
        // Reko: a decoder for the instruction 1808 at address 7E7A has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1808()
        {
            AssertCode("@@@", "1808");
        }
        // Reko: a decoder for the instruction E804 at address 7E82 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_E804()
        {
            AssertCode("@@@", "E804");
        }
        // Reko: a decoder for the instruction 0604 at address 7EA6 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0604()
        {
            AssertCode("@@@", "0604");
        }
        // Reko: a decoder for the instruction 2354 at address 7EB0 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2354()
        {
            AssertCode("@@@", "2354");
        }
        // Reko: a decoder for the instruction 1000845B8DD6 at address 7EC2 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000845B8DD6()
        {
            AssertCode("@@@", "1000845B8DD6");
        }
        // Reko: a decoder for the instruction 7AE6 at address 7ED0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7AE6()
        {
            AssertCode("@@@", "7AE6");
        }
        // Reko: a decoder for the instruction 8FF6 at address 7ED4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FF6()
        {
            AssertCode("@@@", "8FF6");
        }
        // Reko: a decoder for the instruction BC07 at address 7EDC has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_BC07()
        {
            AssertCode("@@@", "BC07");
        }
        // Reko: a decoder for the instruction 3054 at address 7F3C has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3054()
        {
            AssertCode("@@@", "3054");
        }
        // Reko: a decoder for the instruction 4154 at address 7F66 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4154()
        {
            AssertCode("@@@", "4154");
        }
        // Reko: a decoder for the instruction 804A at address 7F78 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_804A()
        {
            AssertCode("@@@", "804A");
        }
        // Reko: a decoder for the instruction 824D at address 7F7C has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_824D()
        {
            AssertCode("@@@", "824D");
        }
        // Reko: a decoder for the instruction 884C at address 7F8C has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_884C()
        {
            AssertCode("@@@", "884C");
        }
        // Reko: a decoder for the instruction 8DE8 at address 7F96 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8DE8()
        {
            AssertCode("@@@", "8DE8");
        }
        // Reko: a decoder for the instruction 8CEF at address 7F98 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8CEF()
        {
            AssertCode("@@@", "8CEF");
        }
        // Reko: a decoder for the instruction 3E3F at address 7F9E has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3E3F()
        {
            AssertCode("@@@", "3E3F");
        }
        // Reko: a decoder for the instruction 0D95 at address 7FA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0D95()
        {
            AssertCode("@@@", "0D95");
        }
        // Reko: a decoder for the instruction 3A08 at address 7FAC has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_3A08()
        {
            AssertCode("@@@", "3A08");
        }
        // Reko: a decoder for the instruction E7F5 at address 7FC6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E7F5()
        {
            AssertCode("@@@", "E7F5");
        }
        // Reko: a decoder for the instruction 01F2 at address 7FD0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_01F2()
        {
            AssertCode("@@@", "01F2");
        }
        // Reko: a decoder for the instruction 4F93 at address 7FF8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F93()
        {
            AssertCode("@@@", "4F93");
        }
        // Reko: a decoder for the instruction 2453 at address 7FFA has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2453()
        {
            AssertCode("@@@", "2453");
        }
        // Reko: a decoder for the instruction 403B at address 8002 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_403B()
        {
            AssertCode("@@@", "403B");
        }
        // Reko: a decoder for the instruction 0FD3 at address 8004 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD3()
        {
            AssertCode("@@@", "0FD3");
        }
        // Reko: a decoder for the instruction 413B at address 8006 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_413B()
        {
            AssertCode("@@@", "413B");
        }
        // Reko: a decoder for the instruction 2054 at address 800C has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2054()
        {
            AssertCode("@@@", "2054");
        }
        // Reko: a decoder for the instruction 5208 at address 8014 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_5208()
        {
            AssertCode("@@@", "5208");
        }
        // Reko: a decoder for the instruction 2025 at address 801E has not been implemented. (Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2025()
        {
            AssertCode("@@@", "2025");
        }
        // Reko: a decoder for the instruction C452 at address 8020 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_C452()
        {
            AssertCode("@@@", "C452");
        }
        // Reko: a decoder for the instruction 1400E0906000 at address 802E has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400E0906000()
        {
            AssertCode("@@@", "1400E0906000");
        }
        // Reko: a decoder for the instruction 1020 at address 8038 has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1020()
        {
            AssertCode("@@@", "1020");
        }
        // Reko: a decoder for the instruction 4533 at address 8044 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4533()
        {
            AssertCode("@@@", "4533");
        }
        // Reko: a decoder for the instruction 1532 at address 8048 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1532()
        {
            AssertCode("@@@", "1532");
        }
        // Reko: a decoder for the instruction 5653 at address 805C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_5653()
        {
            AssertCode("@@@", "5653");
        }
        // Reko: a decoder for the instruction 5533 at address 8062 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5533()
        {
            AssertCode("@@@", "5533");
        }
        // Reko: a decoder for the instruction 5453 at address 806C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_5453()
        {
            AssertCode("@@@", "5453");
        }
        // Reko: a decoder for the instruction 4633 at address 8076 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4633()
        {
            AssertCode("@@@", "4633");
        }
        // Reko: a decoder for the instruction 0FE8 at address 8080 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FE8()
        {
            AssertCode("@@@", "0FE8");
        }
        // Reko: a decoder for the instruction E3EF at address 808A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E3EF()
        {
            AssertCode("@@@", "E3EF");
        }
        // Reko: a decoder for the instruction 29F1 at address 8098 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_29F1()
        {
            AssertCode("@@@", "29F1");
        }
        // Reko: a decoder for the instruction 2045 at address 80AA has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_2045()
        {
            AssertCode("@@@", "2045");
        }
        // Reko: a decoder for the instruction 902A at address 80AC has not been implemented. (ZZ xorw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_902A()
        {
            AssertCode("@@@", "902A");
        }
        // Reko: a decoder for the instruction 52B1 at address 80BA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_52B1()
        {
            AssertCode("@@@", "52B1");
        }
        // Reko: a decoder for the instruction 3291 at address 80BC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3291()
        {
            AssertCode("@@@", "3291");
        }
        // Reko: a decoder for the instruction 7253 at address 80C0 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7253()
        {
            AssertCode("@@@", "7253");
        }
        // Reko: a decoder for the instruction B1EF at address 80C4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_B1EF()
        {
            AssertCode("@@@", "B1EF");
        }
        // Reko: a decoder for the instruction B2B0 at address 80F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B2B0()
        {
            AssertCode("@@@", "B2B0");
        }
        // Reko: a decoder for the instruction A2B1 at address 80FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A2B1()
        {
            AssertCode("@@@", "A2B1");
        }
        // Reko: a decoder for the instruction 69EF at address 810E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_69EF()
        {
            AssertCode("@@@", "69EF");
        }
        // Reko: a decoder for the instruction 2FD6 at address 8114 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FD6()
        {
            AssertCode("@@@", "2FD6");
        }
        // Reko: a decoder for the instruction B9F0 at address 8118 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B9F0()
        {
            AssertCode("@@@", "B9F0");
        }
        // Reko: a decoder for the instruction EFA8 at address 811C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EFA8()
        {
            AssertCode("@@@", "EFA8");
        }
        // Reko: a decoder for the instruction D1FE at address 812A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_D1FE()
        {
            AssertCode("@@@", "D1FE");
        }
        // Reko: a decoder for the instruction 31F4 at address 8132 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_31F4()
        {
            AssertCode("@@@", "31F4");
        }
        // Reko: a decoder for the instruction 5FD6 at address 8138 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FD6()
        {
            AssertCode("@@@", "5FD6");
        }
        // Reko: a decoder for the instruction 95F0 at address 813C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_95F0()
        {
            AssertCode("@@@", "95F0");
        }
        // Reko: a decoder for the instruction 7FDB at address 814A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FDB()
        {
            AssertCode("@@@", "7FDB");
        }
        // Reko: a decoder for the instruction 4F3F at address 8154 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4F3F()
        {
            AssertCode("@@@", "4F3F");
        }
        // Reko: a decoder for the instruction 49F3 at address 815A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_49F3()
        {
            AssertCode("@@@", "49F3");
        }
        // Reko: a decoder for the instruction 6DF0 at address 8164 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DF0()
        {
            AssertCode("@@@", "6DF0");
        }
        // Reko: a decoder for the instruction 4753 at address 8166 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_4753()
        {
            AssertCode("@@@", "4753");
        }
        // Reko: a decoder for the instruction 5606 at address 816A has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_5606()
        {
            AssertCode("@@@", "5606");
        }
        // Reko: a decoder for the instruction 4045 at address 819C has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4045()
        {
            AssertCode("@@@", "4045");
        }
        // Reko: a decoder for the instruction 4139 at address 81AE has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4139()
        {
            AssertCode("@@@", "4139");
        }
        // Reko: a decoder for the instruction 1847 at address 81B0 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_1847()
        {
            AssertCode("@@@", "1847");
        }
        // Reko: a decoder for the instruction 473B at address 81B2 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_473B()
        {
            AssertCode("@@@", "473B");
        }
        // Reko: a decoder for the instruction 1433 at address 81B8 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1433()
        {
            AssertCode("@@@", "1433");
        }
        // Reko: a decoder for the instruction 07EF at address 81C0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_07EF()
        {
            AssertCode("@@@", "07EF");
        }
        // Reko: a decoder for the instruction 1206 at address 81C6 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_1206()
        {
            AssertCode("@@@", "1206");
        }
        // Reko: a decoder for the instruction 4039 at address 8208 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4039()
        {
            AssertCode("@@@", "4039");
        }
        // Reko: a decoder for the instruction 0433 at address 8212 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0433()
        {
            AssertCode("@@@", "0433");
        }
        // Reko: a decoder for the instruction 6FEF at address 821A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6FEF()
        {
            AssertCode("@@@", "6FEF");
        }
        // Reko: a decoder for the instruction 10000018A803 at address 821E has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_10000018A803()
        {
            AssertCode("@@@", "10000018A803");
        }
        // Reko: a decoder for the instruction 110000185403 at address 8226 has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_110000185403()
        {
            AssertCode("@@@", "110000185403");
        }
        // Reko: a decoder for the instruction 705A at address 822C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_705A()
        {
            AssertCode("@@@", "705A");
        }
        // Reko: a decoder for the instruction 2406 at address 8234 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_2406()
        {
            AssertCode("@@@", "2406");
        }
        // Reko: a decoder for the instruction 87FD at address 8244 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_87FD()
        {
            AssertCode("@@@", "87FD");
        }
        // Reko: a decoder for the instruction 984B at address 826C has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_984B()
        {
            AssertCode("@@@", "984B");
        }
        // Reko: a decoder for the instruction F9FF at address 8270 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F9FF()
        {
            AssertCode("@@@", "F9FF");
        }
        // Reko: a decoder for the instruction 4F9F at address 827C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4F9F()
        {
            AssertCode("@@@", "4F9F");
        }
        // Reko: a decoder for the instruction 4253 at address 8280 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_4253()
        {
            AssertCode("@@@", "4253");
        }
        // Reko: a decoder for the instruction CC06 at address 8284 has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_CC06()
        {
            AssertCode("@@@", "CC06");
        }
        // Reko: a decoder for the instruction 30D0 at address 8292 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_30D0()
        {
            AssertCode("@@@", "30D0");
        }
        // Reko: a decoder for the instruction 65F1 at address 82A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_65F1()
        {
            AssertCode("@@@", "65F1");
        }
        // Reko: a decoder for the instruction 1DEF at address 82B4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1DEF()
        {
            AssertCode("@@@", "1DEF");
        }
        // Reko: a decoder for the instruction 31FD at address 82BE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_31FD()
        {
            AssertCode("@@@", "31FD");
        }
        // Reko: a decoder for the instruction A5F9 at address 82C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A5F9()
        {
            AssertCode("@@@", "A5F9");
        }
        // Reko: a decoder for the instruction 8DFE at address 82D2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_8DFE()
        {
            AssertCode("@@@", "8DFE");
        }
        // Reko: a decoder for the instruction 1000015BF149 at address 8312 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000015BF149()
        {
            AssertCode("@@@", "1000015BF149");
        }
        // Reko: a decoder for the instruction 1120 at address 8318 has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1120()
        {
            AssertCode("@@@", "1120");
        }
        // Reko: a decoder for the instruction FDC3 at address 8336 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_FDC3()
        {
            AssertCode("@@@", "FDC3");
        }
        // Reko: a decoder for the instruction 1F8B at address 8348 has not been implemented. (Fmt13 2 ZZ  loadb abs20 rel 20 src abs 4 dest reg 1 src rs)
        [Test]
        public void Cr16Dasm_1F8B()
        {
            AssertCode("@@@", "1F8B");
        }
        // Reko: a decoder for the instruction 353F at address 835A has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_353F()
        {
            AssertCode("@@@", "353F");
        }
        // Reko: a decoder for the instruction 19FB at address 8366 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_19FB()
        {
            AssertCode("@@@", "19FB");
        }
        // Reko: a decoder for the instruction 61EE at address 8370 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_61EE()
        {
            AssertCode("@@@", "61EE");
        }
        // Reko: a decoder for the instruction 57EE at address 837A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_57EE()
        {
            AssertCode("@@@", "57EE");
        }
        // Reko: a decoder for the instruction 0431 at address 837C has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0431()
        {
            AssertCode("@@@", "0431");
        }
        // Reko: a decoder for the instruction 4345 at address 8380 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4345()
        {
            AssertCode("@@@", "4345");
        }
        // Reko: a decoder for the instruction 962A at address 8384 has not been implemented. (ZZ xorw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_962A()
        {
            AssertCode("@@@", "962A");
        }
        // Reko: a decoder for the instruction 6523 at address 8386 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6523()
        {
            AssertCode("@@@", "6523");
        }
        // Reko: a decoder for the instruction 2339 at address 838A has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2339()
        {
            AssertCode("@@@", "2339");
        }
        // Reko: a decoder for the instruction 3546 at address 838C has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_3546()
        {
            AssertCode("@@@", "3546");
        }
        // Reko: a decoder for the instruction 2291 at address 839C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2291()
        {
            AssertCode("@@@", "2291");
        }
        // Reko: a decoder for the instruction 7353 at address 83A4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7353()
        {
            AssertCode("@@@", "7353");
        }
        // Reko: a decoder for the instruction 9604 at address 83A8 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_9604()
        {
            AssertCode("@@@", "9604");
        }
        // Reko: a decoder for the instruction 0A39 at address 83B0 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0A39()
        {
            AssertCode("@@@", "0A39");
        }
        // Reko: a decoder for the instruction AFFC at address 83B2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFFC()
        {
            AssertCode("@@@", "AFFC");
        }
        // Reko: a decoder for the instruction 3FBC at address 83DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FBC()
        {
            AssertCode("@@@", "3FBC");
        }
        // Reko: a decoder for the instruction 3246 at address 83DE has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_3246()
        {
            AssertCode("@@@", "3246");
        }
        // Reko: a decoder for the instruction 52B0 at address 83EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_52B0()
        {
            AssertCode("@@@", "52B0");
        }
        // Reko: a decoder for the instruction 7453 at address 83F8 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_7453()
        {
            AssertCode("@@@", "7453");
        }
        // Reko: a decoder for the instruction 0139 at address 8404 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0139()
        {
            AssertCode("@@@", "0139");
        }
        // Reko: a decoder for the instruction 3DED at address 841A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DED()
        {
            AssertCode("@@@", "3DED");
        }
        // Reko: a decoder for the instruction 1E60 at address 842C has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1E60()
        {
            AssertCode("@@@", "1E60");
        }
        // Reko: a decoder for the instruction 45FC at address 8436 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_45FC()
        {
            AssertCode("@@@", "45FC");
        }
        // Reko: a decoder for the instruction D1F8 at address 843C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D1F8()
        {
            AssertCode("@@@", "D1F8");
        }
        // Reko: a decoder for the instruction 13F1 at address 8444 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_13F1()
        {
            AssertCode("@@@", "13F1");
        }
        // Reko: a decoder for the instruction 3FD6 at address 844C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FD6()
        {
            AssertCode("@@@", "3FD6");
        }
        // Reko: a decoder for the instruction 81ED at address 8450 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_81ED()
        {
            AssertCode("@@@", "81ED");
        }
        // Reko: a decoder for the instruction 77ED at address 845A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_77ED()
        {
            AssertCode("@@@", "77ED");
        }
        // Reko: a decoder for the instruction 43F1 at address 846A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_43F1()
        {
            AssertCode("@@@", "43F1");
        }
        // Reko: a decoder for the instruction 00E0 at address 846E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00E0()
        {
            AssertCode("@@@", "00E0");
        }
        // Reko: a decoder for the instruction 8B23 at address 8470 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8B23()
        {
            AssertCode("@@@", "8B23");
        }
        // Reko: a decoder for the instruction E3F0 at address 8480 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E3F0()
        {
            AssertCode("@@@", "E3F0");
        }
        // Reko: a decoder for the instruction 7F9C at address 848E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F9C()
        {
            AssertCode("@@@", "7F9C");
        }
        // Reko: a decoder for the instruction 6752 at address 8490 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6752()
        {
            AssertCode("@@@", "6752");
        }
        // Reko: a decoder for the instruction D404 at address 8494 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_D404()
        {
            AssertCode("@@@", "D404");
        }
        // Reko: a decoder for the instruction D5F9 at address 849C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D5F9()
        {
            AssertCode("@@@", "D5F9");
        }
        // Reko: a decoder for the instruction D7FA at address 84A2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D7FA()
        {
            AssertCode("@@@", "D7FA");
        }
        // Reko: a decoder for the instruction D7F3 at address 84AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D7F3()
        {
            AssertCode("@@@", "D7F3");
        }
        // Reko: a decoder for the instruction 2D94 at address 84AC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2D94()
        {
            AssertCode("@@@", "2D94");
        }
        // Reko: a decoder for the instruction 4222 at address 84AE has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4222()
        {
            AssertCode("@@@", "4222");
        }
        // Reko: a decoder for the instruction 91F7 at address 84D8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_91F7()
        {
            AssertCode("@@@", "91F7");
        }
        // Reko: a decoder for the instruction 8242 at address 84F2 has not been implemented. (0100 0010 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(left +), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_8242()
        {
            AssertCode("@@@", "8242");
        }
        // Reko: a decoder for the instruction F05A at address 850A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F05A()
        {
            AssertCode("@@@", "F05A");
        }
        // Reko: a decoder for the instruction 8052 at address 850E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8052()
        {
            AssertCode("@@@", "8052");
        }
        // Reko: a decoder for the instruction A9EC at address 8528 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A9EC()
        {
            AssertCode("@@@", "A9EC");
        }
        // Reko: a decoder for the instruction 33F0 at address 8530 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_33F0()
        {
            AssertCode("@@@", "33F0");
        }
        // Reko: a decoder for the instruction 75F4 at address 8536 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_75F4()
        {
            AssertCode("@@@", "75F4");
        }
        // Reko: a decoder for the instruction 95EC at address 853C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_95EC()
        {
            AssertCode("@@@", "95EC");
        }
        // Reko: a decoder for the instruction 23F2 at address 8542 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_23F2()
        {
            AssertCode("@@@", "23F2");
        }
        // Reko: a decoder for the instruction 85EC at address 854C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_85EC()
        {
            AssertCode("@@@", "85EC");
        }
        // Reko: a decoder for the instruction F3EB at address 8552 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F3EB()
        {
            AssertCode("@@@", "F3EB");
        }
        // Reko: a decoder for the instruction 75EC at address 855C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_75EC()
        {
            AssertCode("@@@", "75EC");
        }
        // Reko: a decoder for the instruction 0FEF at address 8562 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0FEF()
        {
            AssertCode("@@@", "0FEF");
        }
        // Reko: a decoder for the instruction FBF1 at address 856A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FBF1()
        {
            AssertCode("@@@", "FBF1");
        }
        // Reko: a decoder for the instruction E7EF at address 8570 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E7EF()
        {
            AssertCode("@@@", "E7EF");
        }
        // Reko: a decoder for the instruction 57EC at address 857A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_57EC()
        {
            AssertCode("@@@", "57EC");
        }
        // Reko: a decoder for the instruction 37FA at address 8594 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_37FA()
        {
            AssertCode("@@@", "37FA");
        }
        // Reko: a decoder for the instruction 725A at address 85B2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_725A()
        {
            AssertCode("@@@", "725A");
        }
        // Reko: a decoder for the instruction 3232 at address 85B6 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3232()
        {
            AssertCode("@@@", "3232");
        }
        // Reko: a decoder for the instruction EBF9 at address 85E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EBF9()
        {
            AssertCode("@@@", "EBF9");
        }
        // Reko: a decoder for the instruction 3090 at address 8614 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3090()
        {
            AssertCode("@@@", "3090");
        }
        // Reko: a decoder for the instruction 325A at address 8616 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_325A()
        {
            AssertCode("@@@", "325A");
        }
        // Reko: a decoder for the instruction E84B at address 861C has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_E84B()
        {
            AssertCode("@@@", "E84B");
        }
        // Reko: a decoder for the instruction 57FC at address 8624 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_57FC()
        {
            AssertCode("@@@", "57FC");
        }
        // Reko: a decoder for the instruction A3EB at address 862E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A3EB()
        {
            AssertCode("@@@", "A3EB");
        }
        // Reko: a decoder for the instruction 69F1 at address 8642 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_69F1()
        {
            AssertCode("@@@", "69F1");
        }
        // Reko: a decoder for the instruction 81EB at address 8650 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_81EB()
        {
            AssertCode("@@@", "81EB");
        }
        // Reko: a decoder for the instruction 1FC3 at address 8660 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_1FC3()
        {
            AssertCode("@@@", "1FC3");
        }
        // Reko: a decoder for the instruction 6BEB at address 8666 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6BEB()
        {
            AssertCode("@@@", "6BEB");
        }
        // Reko: a decoder for the instruction F5EE at address 866E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_F5EE()
        {
            AssertCode("@@@", "F5EE");
        }
        // Reko: a decoder for the instruction 59EB at address 8678 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_59EB()
        {
            AssertCode("@@@", "59EB");
        }
        // Reko: a decoder for the instruction EDF6 at address 8682 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EDF6()
        {
            AssertCode("@@@", "EDF6");
        }
        // Reko: a decoder for the instruction 77F5 at address 8688 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_77F5()
        {
            AssertCode("@@@", "77F5");
        }
        // Reko: a decoder for the instruction 25F6 at address 868E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_25F6()
        {
            AssertCode("@@@", "25F6");
        }
        // Reko: a decoder for the instruction 33F8 at address 8694 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_33F8()
        {
            AssertCode("@@@", "33F8");
        }
        // Reko: a decoder for the instruction DFF8 at address 869A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFF8()
        {
            AssertCode("@@@", "DFF8");
        }
        // Reko: a decoder for the instruction 2431 at address 869C has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2431()
        {
            AssertCode("@@@", "2431");
        }
        // Reko: a decoder for the instruction 1023 at address 86A8 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1023()
        {
            AssertCode("@@@", "1023");
        }
        // Reko: a decoder for the instruction 2A39 at address 86AA has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2A39()
        {
            AssertCode("@@@", "2A39");
        }
        // Reko: a decoder for the instruction A046 at address 86AC has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_A046()
        {
            AssertCode("@@@", "A046");
        }
        // Reko: a decoder for the instruction B091 at address 86BA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B091()
        {
            AssertCode("@@@", "B091");
        }
        // Reko: a decoder for the instruction 93FA at address 86CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_93FA()
        {
            AssertCode("@@@", "93FA");
        }
        // Reko: a decoder for the instruction AFFF at address 86D0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AFFF()
        {
            AssertCode("@@@", "AFFF");
        }
        // Reko: a decoder for the instruction CFEF at address 86DA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_CFEF()
        {
            AssertCode("@@@", "CFEF");
        }
        // Reko: a decoder for the instruction 1FBF at address 8700 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1FBF()
        {
            AssertCode("@@@", "1FBF");
        }
        // Reko: a decoder for the instruction 1046 at address 8704 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1046()
        {
            AssertCode("@@@", "1046");
        }
        // Reko: a decoder for the instruction 5033 at address 8706 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5033()
        {
            AssertCode("@@@", "5033");
        }
        // Reko: a decoder for the instruction C091 at address 8712 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C091()
        {
            AssertCode("@@@", "C091");
        }
        // Reko: a decoder for the instruction CFAF at address 8722 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_CFAF()
        {
            AssertCode("@@@", "CFAF");
        }
        // Reko: a decoder for the instruction 2539 at address 872E has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2539()
        {
            AssertCode("@@@", "2539");
        }
        // Reko: a decoder for the instruction 5847 at address 8730 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_5847()
        {
            AssertCode("@@@", "5847");
        }
        // Reko: a decoder for the instruction 0339 at address 8734 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0339()
        {
            AssertCode("@@@", "0339");
        }
        // Reko: a decoder for the instruction 3847 at address 8736 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_3847()
        {
            AssertCode("@@@", "3847");
        }
        // Reko: a decoder for the instruction 4FE9 at address 8748 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FE9()
        {
            AssertCode("@@@", "4FE9");
        }
        // Reko: a decoder for the instruction 6FE8 at address 874C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FE8()
        {
            AssertCode("@@@", "6FE8");
        }
        // Reko: a decoder for the instruction 73F7 at address 8754 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_73F7()
        {
            AssertCode("@@@", "73F7");
        }
        // Reko: a decoder for the instruction CBF1 at address 875C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CBF1()
        {
            AssertCode("@@@", "CBF1");
        }
        // Reko: a decoder for the instruction 02D0 at address 876A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02D0()
        {
            AssertCode("@@@", "02D0");
        }
        // Reko: a decoder for the instruction B422 at address 876C has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B422()
        {
            AssertCode("@@@", "B422");
        }
        // Reko: a decoder for the instruction 363F at address 8790 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_363F()
        {
            AssertCode("@@@", "363F");
        }
        // Reko: a decoder for the instruction E9F4 at address 879C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E9F4()
        {
            AssertCode("@@@", "E9F4");
        }
        // Reko: a decoder for the instruction 41EC at address 87AC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_41EC()
        {
            AssertCode("@@@", "41EC");
        }
        // Reko: a decoder for the instruction 1BEA at address 87B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1BEA()
        {
            AssertCode("@@@", "1BEA");
        }
        // Reko: a decoder for the instruction C1EE at address 87BC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_C1EE()
        {
            AssertCode("@@@", "C1EE");
        }
        // Reko: a decoder for the instruction DBF9 at address 87C2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DBF9()
        {
            AssertCode("@@@", "DBF9");
        }
        // Reko: a decoder for the instruction EDE9 at address 87D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EDE9()
        {
            AssertCode("@@@", "EDE9");
        }
        // Reko: a decoder for the instruction 1DFA at address 87DA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DFA()
        {
            AssertCode("@@@", "1DFA");
        }
        // Reko: a decoder for the instruction D9EE at address 87E2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_D9EE()
        {
            AssertCode("@@@", "D9EE");
        }
        // Reko: a decoder for the instruction 2FC3 at address 87FC has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_2FC3()
        {
            AssertCode("@@@", "2FC3");
        }
        // Reko: a decoder for the instruction E3E6 at address 8802 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E3E6()
        {
            AssertCode("@@@", "E3E6");
        }
        // Reko: a decoder for the instruction C1E9 at address 8810 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C1E9()
        {
            AssertCode("@@@", "C1E9");
        }
        // Reko: a decoder for the instruction 25F5 at address 8816 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_25F5()
        {
            AssertCode("@@@", "25F5");
        }
        // Reko: a decoder for the instruction FBFA at address 881C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FBFA()
        {
            AssertCode("@@@", "FBFA");
        }
        // Reko: a decoder for the instruction F3FA at address 8824 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F3FA()
        {
            AssertCode("@@@", "F3FA");
        }
        // Reko: a decoder for the instruction 1F9C at address 882C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F9C()
        {
            AssertCode("@@@", "1F9C");
        }
        // Reko: a decoder for the instruction 6152 at address 882E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6152()
        {
            AssertCode("@@@", "6152");
        }
        // Reko: a decoder for the instruction 3BE8 at address 8832 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3BE8()
        {
            AssertCode("@@@", "3BE8");
        }
        // Reko: a decoder for the instruction 97E9 at address 883A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_97E9()
        {
            AssertCode("@@@", "97E9");
        }
        // Reko: a decoder for the instruction C5FB at address 8840 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C5FB()
        {
            AssertCode("@@@", "C5FB");
        }
        // Reko: a decoder for the instruction 87E9 at address 884A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_87E9()
        {
            AssertCode("@@@", "87E9");
        }
        // Reko: a decoder for the instruction 7DE9 at address 8854 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7DE9()
        {
            AssertCode("@@@", "7DE9");
        }
        // Reko: a decoder for the instruction 05FA at address 885A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_05FA()
        {
            AssertCode("@@@", "05FA");
        }
        // Reko: a decoder for the instruction C84B at address 885C has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_C84B()
        {
            AssertCode("@@@", "C84B");
        }
        // Reko: a decoder for the instruction 8032 at address 8866 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8032()
        {
            AssertCode("@@@", "8032");
        }
        // Reko: a decoder for the instruction E5EC at address 887E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E5EC()
        {
            AssertCode("@@@", "E5EC");
        }
        // Reko: a decoder for the instruction 1DD7 at address 888C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DD7()
        {
            AssertCode("@@@", "1DD7");
        }
        // Reko: a decoder for the instruction 3D3F at address 88AA has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3D3F()
        {
            AssertCode("@@@", "3D3F");
        }
        // Reko: a decoder for the instruction 8FA6 at address 88B0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FA6()
        {
            AssertCode("@@@", "8FA6");
        }
        // Reko: a decoder for the instruction 2BF6 at address 88B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2BF6()
        {
            AssertCode("@@@", "2BF6");
        }
        // Reko: a decoder for the instruction 0FAF at address 88B8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0FAF()
        {
            AssertCode("@@@", "0FAF");
        }
        // Reko: a decoder for the instruction CDE8 at address 88F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CDE8()
        {
            AssertCode("@@@", "CDE8");
        }
        // Reko: a decoder for the instruction D3E8 at address 88FE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D3E8()
        {
            AssertCode("@@@", "D3E8");
        }
        // Reko: a decoder for the instruction ABFC at address 8904 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ABFC()
        {
            AssertCode("@@@", "ABFC");
        }
        // Reko: a decoder for the instruction 6DC3 at address 890E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_6DC3()
        {
            AssertCode("@@@", "6DC3");
        }
        // Reko: a decoder for the instruction E5FE at address 8944 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_E5FE()
        {
            AssertCode("@@@", "E5FE");
        }
        // Reko: a decoder for the instruction 75E8 at address 894C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_75E8()
        {
            AssertCode("@@@", "75E8");
        }
        // Reko: a decoder for the instruction 6BE8 at address 895E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6BE8()
        {
            AssertCode("@@@", "6BE8");
        }
        // Reko: a decoder for the instruction 97FC at address 8964 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_97FC()
        {
            AssertCode("@@@", "97FC");
        }
        // Reko: a decoder for the instruction 61E8 at address 8970 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_61E8()
        {
            AssertCode("@@@", "61E8");
        }
        // Reko: a decoder for the instruction 53E8 at address 897E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_53E8()
        {
            AssertCode("@@@", "53E8");
        }
        // Reko: a decoder for the instruction 23EC at address 898A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_23EC()
        {
            AssertCode("@@@", "23EC");
        }
        // Reko: a decoder for the instruction 4DEF at address 8990 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4DEF()
        {
            AssertCode("@@@", "4DEF");
        }
        // Reko: a decoder for the instruction 37E8 at address 899A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_37E8()
        {
            AssertCode("@@@", "37E8");
        }
        // Reko: a decoder for the instruction 41E5 at address 89A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_41E5()
        {
            AssertCode("@@@", "41E5");
        }
        // Reko: a decoder for the instruction 1BE6 at address 89AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1BE6()
        {
            AssertCode("@@@", "1BE6");
        }
        // Reko: a decoder for the instruction 25EE at address 89B0 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_25EE()
        {
            AssertCode("@@@", "25EE");
        }
        // Reko: a decoder for the instruction FFEB at address 89B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FFEB()
        {
            AssertCode("@@@", "FFEB");
        }
        // Reko: a decoder for the instruction F1EB at address 89BC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F1EB()
        {
            AssertCode("@@@", "F1EB");
        }
        // Reko: a decoder for the instruction 24A0 at address 89E6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_24A0()
        {
            AssertCode("@@@", "24A0");
        }
        // Reko: a decoder for the instruction 1000FC614A55 at address 8A36 has not been implemented. (Fmt2 3 ZZ ope 6  cbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000FC614A55()
        {
            AssertCode("@@@", "1000FC614A55");
        }
        // Reko: a decoder for the instruction B127 at address 8A82 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B127()
        {
            AssertCode("@@@", "B127");
        }
        // Reko: a decoder for the instruction 0645 at address 8B7C has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0645()
        {
            AssertCode("@@@", "0645");
        }
        // Reko: a decoder for the instruction 68DF at address 8B7E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_68DF()
        {
            AssertCode("@@@", "68DF");
        }
        // Reko: a decoder for the instruction 6753 at address 8B8A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6753()
        {
            AssertCode("@@@", "6753");
        }
        // Reko: a decoder for the instruction 063B at address 8B92 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_063B()
        {
            AssertCode("@@@", "063B");
        }
        // Reko: a decoder for the instruction 1DC3 at address 8BD2 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_1DC3()
        {
            AssertCode("@@@", "1DC3");
        }
        // Reko: a decoder for the instruction 1E94 at address 8CCA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1E94()
        {
            AssertCode("@@@", "1E94");
        }
        // Reko: a decoder for the instruction 2122 at address 8CCC has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2122()
        {
            AssertCode("@@@", "2122");
        }
        // Reko: a decoder for the instruction 4EEC at address 8CD2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4EEC()
        {
            AssertCode("@@@", "4EEC");
        }
        // Reko: a decoder for the instruction 8C92 at address 8D34 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8C92()
        {
            AssertCode("@@@", "8C92");
        }
        // Reko: a decoder for the instruction 7052 at address 8D3E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7052()
        {
            AssertCode("@@@", "7052");
        }
        // Reko: a decoder for the instruction 533F at address 8D46 has not been implemented. (Fmt15 1 ZZ  subcw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_533F()
        {
            AssertCode("@@@", "533F");
        }
        // Reko: a decoder for the instruction 1448 at address 8D5E has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_1448()
        {
            AssertCode("@@@", "1448");
        }
        // Reko: a decoder for the instruction 4FF0 at address 8D72 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FF0()
        {
            AssertCode("@@@", "4FF0");
        }
        // Reko: a decoder for the instruction 7752 at address 8D7E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7752()
        {
            AssertCode("@@@", "7752");
        }
        // Reko: a decoder for the instruction 2FF1 at address 8D82 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FF1()
        {
            AssertCode("@@@", "2FF1");
        }
        // Reko: a decoder for the instruction 024B at address 8D86 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_024B()
        {
            AssertCode("@@@", "024B");
        }
        // Reko: a decoder for the instruction 7152 at address 8D8E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7152()
        {
            AssertCode("@@@", "7152");
        }
        // Reko: a decoder for the instruction 2FF2 at address 8D92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FF2()
        {
            AssertCode("@@@", "2FF2");
        }
        // Reko: a decoder for the instruction 4B5A at address 8DBE has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4B5A()
        {
            AssertCode("@@@", "4B5A");
        }
        // Reko: a decoder for the instruction 043B at address 8DD4 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_043B()
        {
            AssertCode("@@@", "043B");
        }
        // Reko: a decoder for the instruction A408 at address 8DE2 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_A408()
        {
            AssertCode("@@@", "A408");
        }
        // Reko: a decoder for the instruction 1621 at address 8DF8 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1621()
        {
            AssertCode("@@@", "1621");
        }
        // Reko: a decoder for the instruction B95A at address 8E00 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B95A()
        {
            AssertCode("@@@", "B95A");
        }
        // Reko: a decoder for the instruction 475A at address 8E06 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_475A()
        {
            AssertCode("@@@", "475A");
        }
        // Reko: a decoder for the instruction B353 at address 8E16 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_B353()
        {
            AssertCode("@@@", "B353");
        }
        // Reko: a decoder for the instruction B208 at address 8E26 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_B208()
        {
            AssertCode("@@@", "B208");
        }
        // Reko: a decoder for the instruction 1853 at address 8E2A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1853()
        {
            AssertCode("@@@", "1853");
        }
        // Reko: a decoder for the instruction 8461 at address 8E38 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8461()
        {
            AssertCode("@@@", "8461");
        }
        // Reko: a decoder for the instruction 4CE0 at address 8E44 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CE0()
        {
            AssertCode("@@@", "4CE0");
        }
        // Reko: a decoder for the instruction 2CA4 at address 8E46 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CA4()
        {
            AssertCode("@@@", "2CA4");
        }
        // Reko: a decoder for the instruction 8CE4 at address 8E4A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CE4()
        {
            AssertCode("@@@", "8CE4");
        }
        // Reko: a decoder for the instruction ACAA at address 8E50 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACAA()
        {
            AssertCode("@@@", "ACAA");
        }
        // Reko: a decoder for the instruction ACEA at address 8E5A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACEA()
        {
            AssertCode("@@@", "ACEA");
        }
        // Reko: a decoder for the instruction 4CA0 at address 8E9A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CA0()
        {
            AssertCode("@@@", "4CA0");
        }
        // Reko: a decoder for the instruction 61FE at address 8EC2 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_61FE()
        {
            AssertCode("@@@", "61FE");
        }
        // Reko: a decoder for the instruction 59FE at address 8ECA has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_59FE()
        {
            AssertCode("@@@", "59FE");
        }
        // Reko: a decoder for the instruction E4A0 at address 8EEA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E4A0()
        {
            AssertCode("@@@", "E4A0");
        }
        // Reko: a decoder for the instruction 049F at address 8F0C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_049F()
        {
            AssertCode("@@@", "049F");
        }
        // Reko: a decoder for the instruction 64AF at address 8F2E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_64AF()
        {
            AssertCode("@@@", "64AF");
        }
        // Reko: a decoder for the instruction C4AF at address 8F42 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C4AF()
        {
            AssertCode("@@@", "C4AF");
        }
        // Reko: a decoder for the instruction 2CA0 at address 8F4C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CA0()
        {
            AssertCode("@@@", "2CA0");
        }
        // Reko: a decoder for the instruction 0C92 at address 8F5C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0C92()
        {
            AssertCode("@@@", "0C92");
        }
        // Reko: a decoder for the instruction 0AE0 at address 8FB6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AE0()
        {
            AssertCode("@@@", "0AE0");
        }
        // Reko: a decoder for the instruction 08A2 at address 8FB8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_08A2()
        {
            AssertCode("@@@", "08A2");
        }
        // Reko: a decoder for the instruction 0AE2 at address 8FBA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AE2()
        {
            AssertCode("@@@", "0AE2");
        }
        // Reko: a decoder for the instruction 0AE4 at address 8FBE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AE4()
        {
            AssertCode("@@@", "0AE4");
        }
        // Reko: a decoder for the instruction ADE0 at address 9016 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ADE0()
        {
            AssertCode("@@@", "ADE0");
        }
        // Reko: a decoder for the instruction B854 at address 9022 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B854()
        {
            AssertCode("@@@", "B854");
        }
        // Reko: a decoder for the instruction DAEF at address 9060 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DAEF()
        {
            AssertCode("@@@", "DAEF");
        }
        // Reko: a decoder for the instruction C461 at address 9070 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_C461()
        {
            AssertCode("@@@", "C461");
        }
        // Reko: a decoder for the instruction 0E94 at address 9138 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0E94()
        {
            AssertCode("@@@", "0E94");
        }
        // Reko: a decoder for the instruction 0ED4 at address 9142 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0ED4()
        {
            AssertCode("@@@", "0ED4");
        }
        // Reko: a decoder for the instruction 4026 at address 914A has not been implemented. (ZZ orw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4026()
        {
            AssertCode("@@@", "4026");
        }
        // Reko: a decoder for the instruction 2492 at address 9188 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2492()
        {
            AssertCode("@@@", "2492");
        }
        // Reko: a decoder for the instruction 004D at address 919C has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_004D()
        {
            AssertCode("@@@", "004D");
        }
        // Reko: a decoder for the instruction 249F at address 91AA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_249F()
        {
            AssertCode("@@@", "249F");
        }
        // Reko: a decoder for the instruction 349F at address 91AE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_349F()
        {
            AssertCode("@@@", "349F");
        }
        // Reko: a decoder for the instruction 323B at address 91B2 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_323B()
        {
            AssertCode("@@@", "323B");
        }
        // Reko: a decoder for the instruction 2092 at address 91EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2092()
        {
            AssertCode("@@@", "2092");
        }
        // Reko: a decoder for the instruction 90FF at address 9212 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_90FF()
        {
            AssertCode("@@@", "90FF");
        }
        // Reko: a decoder for the instruction 3FEF at address 921A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3FEF()
        {
            AssertCode("@@@", "3FEF");
        }
        // Reko: a decoder for the instruction 6FAF at address 9252 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6FAF()
        {
            AssertCode("@@@", "6FAF");
        }
        // Reko: a decoder for the instruction 1860 at address 925C has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1860()
        {
            AssertCode("@@@", "1860");
        }
        // Reko: a decoder for the instruction 8861 at address 925E has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8861()
        {
            AssertCode("@@@", "8861");
        }
        // Reko: a decoder for the instruction 0690 at address 9262 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0690()
        {
            AssertCode("@@@", "0690");
        }
        // Reko: a decoder for the instruction 30DF at address 9270 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_30DF()
        {
            AssertCode("@@@", "30DF");
        }
        // Reko: a decoder for the instruction AF9F at address 927A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AF9F()
        {
            AssertCode("@@@", "AF9F");
        }
        // Reko: a decoder for the instruction A407 at address 9282 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_A407()
        {
            AssertCode("@@@", "A407");
        }
        // Reko: a decoder for the instruction 0F9F at address 9284 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0F9F()
        {
            AssertCode("@@@", "0F9F");
        }
        // Reko: a decoder for the instruction 3A00 at address 928E has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3A00()
        {
            AssertCode("@@@", "3A00");
        }
        // Reko: a decoder for the instruction DA07 at address 929E has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_DA07()
        {
            AssertCode("@@@", "DA07");
        }
        // Reko: a decoder for the instruction 3600 at address 92A2 has not been implemented. (Fmt23 3 ZZ  subd imm32,rp 4 dest rp 32 src imm)
        [Test]
        public void Cr16Dasm_3600()
        {
            AssertCode("@@@", "3600");
        }
        // Reko: a decoder for the instruction 2A08 at address 92A8 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_2A08()
        {
            AssertCode("@@@", "2A08");
        }
        // Reko: a decoder for the instruction 3808 at address 92B2 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_3808()
        {
            AssertCode("@@@", "3808");
        }
        // Reko: a decoder for the instruction 3E08 at address 92BC has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_3E08()
        {
            AssertCode("@@@", "3E08");
        }
        // Reko: a decoder for the instruction 5408 at address 92C6 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_5408()
        {
            AssertCode("@@@", "5408");
        }
        // Reko: a decoder for the instruction 8807 at address 92D0 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_8807()
        {
            AssertCode("@@@", "8807");
        }
        // Reko: a decoder for the instruction 5808 at address 92DA has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_5808()
        {
            AssertCode("@@@", "5808");
        }
        // Reko: a decoder for the instruction 8208 at address 92E4 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_8208()
        {
            AssertCode("@@@", "8208");
        }
        // Reko: a decoder for the instruction 9008 at address 92EE has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_9008()
        {
            AssertCode("@@@", "9008");
        }
        // Reko: a decoder for the instruction 8E08 at address 9302 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_8E08()
        {
            AssertCode("@@@", "8E08");
        }
        // Reko: a decoder for the instruction 1E08 at address 930C has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_1E08()
        {
            AssertCode("@@@", "1E08");
        }
        // Reko: a decoder for the instruction 4FAF at address 930E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4FAF()
        {
            AssertCode("@@@", "4FAF");
        }
        // Reko: a decoder for the instruction 02D1 at address 931A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02D1()
        {
            AssertCode("@@@", "02D1");
        }
        // Reko: a decoder for the instruction 8454 at address 931C has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8454()
        {
            AssertCode("@@@", "8454");
        }
        // Reko: a decoder for the instruction 02D3 at address 9328 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02D3()
        {
            AssertCode("@@@", "02D3");
        }
        // Reko: a decoder for the instruction 7FAF at address 932A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7FAF()
        {
            AssertCode("@@@", "7FAF");
        }
        // Reko: a decoder for the instruction 8608 at address 9340 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_8608()
        {
            AssertCode("@@@", "8608");
        }
        // Reko: a decoder for the instruction 1F9F at address 9342 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1F9F()
        {
            AssertCode("@@@", "1F9F");
        }
        // Reko: a decoder for the instruction C207 at address 934A has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_C207()
        {
            AssertCode("@@@", "C207");
        }
        // Reko: a decoder for the instruction ED5A at address 9350 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_ED5A()
        {
            AssertCode("@@@", "ED5A");
        }
        // Reko: a decoder for the instruction 5D52 at address 9362 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5D52()
        {
            AssertCode("@@@", "5D52");
        }
        // Reko: a decoder for the instruction 2A07 at address 9366 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_2A07()
        {
            AssertCode("@@@", "2A07");
        }
        // Reko: a decoder for the instruction 2007 at address 9370 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_2007()
        {
            AssertCode("@@@", "2007");
        }
        // Reko: a decoder for the instruction 6D52 at address 9372 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6D52()
        {
            AssertCode("@@@", "6D52");
        }
        // Reko: a decoder for the instruction 6E07 at address 9376 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_6E07()
        {
            AssertCode("@@@", "6E07");
        }
        // Reko: a decoder for the instruction 6407 at address 9380 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_6407()
        {
            AssertCode("@@@", "6407");
        }
        // Reko: a decoder for the instruction 7D52 at address 9382 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7D52()
        {
            AssertCode("@@@", "7D52");
        }
        // Reko: a decoder for the instruction 8407 at address 9390 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_8407()
        {
            AssertCode("@@@", "8407");
        }
        // Reko: a decoder for the instruction 8D52 at address 9392 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8D52()
        {
            AssertCode("@@@", "8D52");
        }
        // Reko: a decoder for the instruction AE07 at address 9396 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_AE07()
        {
            AssertCode("@@@", "AE07");
        }
        // Reko: a decoder for the instruction F807 at address 93B2 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_F807()
        {
            AssertCode("@@@", "F807");
        }
        // Reko: a decoder for the instruction AD52 at address 93B4 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_AD52()
        {
            AssertCode("@@@", "AD52");
        }
        // Reko: a decoder for the instruction F007 at address 93C2 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_F007()
        {
            AssertCode("@@@", "F007");
        }
        // Reko: a decoder for the instruction C006 at address 93CA has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_C006()
        {
            AssertCode("@@@", "C006");
        }
        // Reko: a decoder for the instruction E407 at address 93D4 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_E407()
        {
            AssertCode("@@@", "E407");
        }
        // Reko: a decoder for the instruction CD52 at address 93D6 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_CD52()
        {
            AssertCode("@@@", "CD52");
        }
        // Reko: a decoder for the instruction B006 at address 93DA has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_B006()
        {
            AssertCode("@@@", "B006");
        }
        // Reko: a decoder for the instruction DC07 at address 93E4 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_DC07()
        {
            AssertCode("@@@", "DC07");
        }
        // Reko: a decoder for the instruction DD52 at address 93E6 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_DD52()
        {
            AssertCode("@@@", "DD52");
        }
        // Reko: a decoder for the instruction BA07 at address 93EA has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_BA07()
        {
            AssertCode("@@@", "BA07");
        }
        // Reko: a decoder for the instruction B007 at address 93F4 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_B007()
        {
            AssertCode("@@@", "B007");
        }
        // Reko: a decoder for the instruction FD52 at address 93F6 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_FD52()
        {
            AssertCode("@@@", "FD52");
        }
        // Reko: a decoder for the instruction 8E06 at address 93FA has not been implemented. (Fmt15 1 ZZ  tbit cnt 4 src reg 4 pos imm)
        [Test]
        public void Cr16Dasm_8E06()
        {
            AssertCode("@@@", "8E06");
        }
        // Reko: a decoder for the instruction C807 at address 9406 has not been implemented. (Fmt15 1 ZZ  tbit reg, reg 4 src reg 4 pos reg)
        [Test]
        public void Cr16Dasm_C807()
        {
            AssertCode("@@@", "C807");
        }
        // Reko: a decoder for the instruction 3F9F at address 9412 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3F9F()
        {
            AssertCode("@@@", "3F9F");
        }
        // Reko: a decoder for the instruction 1133 at address 9416 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1133()
        {
            AssertCode("@@@", "1133");
        }
        // Reko: a decoder for the instruction 313B at address 9418 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_313B()
        {
            AssertCode("@@@", "313B");
        }
        // Reko: a decoder for the instruction 6F9F at address 942E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6F9F()
        {
            AssertCode("@@@", "6F9F");
        }
        // Reko: a decoder for the instruction 613B at address 9434 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_613B()
        {
            AssertCode("@@@", "613B");
        }
        // Reko: a decoder for the instruction 7F9F at address 943C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7F9F()
        {
            AssertCode("@@@", "7F9F");
        }
        // Reko: a decoder for the instruction 8F9F at address 944A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8F9F()
        {
            AssertCode("@@@", "8F9F");
        }
        // Reko: a decoder for the instruction 813B at address 9450 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_813B()
        {
            AssertCode("@@@", "813B");
        }
        // Reko: a decoder for the instruction BF9F at address 9458 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BF9F()
        {
            AssertCode("@@@", "BF9F");
        }
        // Reko: a decoder for the instruction BFDF at address 945C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BFDF()
        {
            AssertCode("@@@", "BFDF");
        }
        // Reko: a decoder for the instruction A13B at address 950E has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A13B()
        {
            AssertCode("@@@", "A13B");
        }
        // Reko: a decoder for the instruction 1D52 at address 9520 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1D52()
        {
            AssertCode("@@@", "1D52");
        }
        // Reko: a decoder for the instruction 0FD4 at address 953A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD4()
        {
            AssertCode("@@@", "0FD4");
        }
        // Reko: a decoder for the instruction 6033 at address 953C has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6033()
        {
            AssertCode("@@@", "6033");
        }
        // Reko: a decoder for the instruction 8033 at address 9544 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8033()
        {
            AssertCode("@@@", "8033");
        }
        // Reko: a decoder for the instruction 0FD7 at address 9546 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD7()
        {
            AssertCode("@@@", "0FD7");
        }
        // Reko: a decoder for the instruction 185A at address 95A4 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_185A()
        {
            AssertCode("@@@", "185A");
        }
        // Reko: a decoder for the instruction 4833 at address 95A6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4833()
        {
            AssertCode("@@@", "4833");
        }
        // Reko: a decoder for the instruction 80D0 at address 95A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80D0()
        {
            AssertCode("@@@", "80D0");
        }
        // Reko: a decoder for the instruction 3553 at address 95B6 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_3553()
        {
            AssertCode("@@@", "3553");
        }
        // Reko: a decoder for the instruction DA53 at address 95BA has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_DA53()
        {
            AssertCode("@@@", "DA53");
        }
        // Reko: a decoder for the instruction AFDF at address 95C0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AFDF()
        {
            AssertCode("@@@", "AFDF");
        }
        // Reko: a decoder for the instruction 9FDF at address 95C8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_9FDF()
        {
            AssertCode("@@@", "9FDF");
        }
        // Reko: a decoder for the instruction 43A0 at address 95D0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_43A0()
        {
            AssertCode("@@@", "43A0");
        }
        // Reko: a decoder for the instruction 1FFF at address 9610 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1FFF()
        {
            AssertCode("@@@", "1FFF");
        }
        // Reko: a decoder for the instruction 7FBF at address 961E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7FBF()
        {
            AssertCode("@@@", "7FBF");
        }
        // Reko: a decoder for the instruction 14004FDF5000 at address 9646 has not been implemented. (Fmt1 2 ZZ ope 13  macqw 4 src2 reg 4 src1 reg 4 dest rp 4)
        [Test]
        public void Cr16Dasm_14004FDF5000()
        {
            AssertCode("@@@", "14004FDF5000");
        }
        // Reko: a decoder for the instruction 2FFF at address 964C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2FFF()
        {
            AssertCode("@@@", "2FFF");
        }
        // Reko: a decoder for the instruction 8FDF at address 9664 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8FDF()
        {
            AssertCode("@@@", "8FDF");
        }
        // Reko: a decoder for the instruction A839 at address 9686 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A839()
        {
            AssertCode("@@@", "A839");
        }
        // Reko: a decoder for the instruction 8FFF at address 9688 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8FFF()
        {
            AssertCode("@@@", "8FFF");
        }
        // Reko: a decoder for the instruction 1090 at address 9692 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1090()
        {
            AssertCode("@@@", "1090");
        }
        // Reko: a decoder for the instruction 2F9F at address 969A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2F9F()
        {
            AssertCode("@@@", "2F9F");
        }
        // Reko: a decoder for the instruction 2153 at address 96A2 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2153()
        {
            AssertCode("@@@", "2153");
        }
        // Reko: a decoder for the instruction 6345 at address 96C0 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_6345()
        {
            AssertCode("@@@", "6345");
        }
        // Reko: a decoder for the instruction 5FBF at address 96C6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5FBF()
        {
            AssertCode("@@@", "5FBF");
        }
        // Reko: a decoder for the instruction A039 at address 96CE has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A039()
        {
            AssertCode("@@@", "A039");
        }
        // Reko: a decoder for the instruction 0546 at address 96D2 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0546()
        {
            AssertCode("@@@", "0546");
        }
        // Reko: a decoder for the instruction 2FBF at address 96D8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2FBF()
        {
            AssertCode("@@@", "2FBF");
        }
        // Reko: a decoder for the instruction 82D0 at address 96EC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_82D0()
        {
            AssertCode("@@@", "82D0");
        }
        // Reko: a decoder for the instruction 12D1 at address 96EE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12D1()
        {
            AssertCode("@@@", "12D1");
        }
        // Reko: a decoder for the instruction B033 at address 96F6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B033()
        {
            AssertCode("@@@", "B033");
        }
        // Reko: a decoder for the instruction 6123 at address 9700 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6123()
        {
            AssertCode("@@@", "6123");
        }
        // Reko: a decoder for the instruction 1423 at address 9718 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1423()
        {
            AssertCode("@@@", "1423");
        }
        // Reko: a decoder for the instruction F261 at address 972A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_F261()
        {
            AssertCode("@@@", "F261");
        }
        // Reko: a decoder for the instruction DB53 at address 973A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_DB53()
        {
            AssertCode("@@@", "DB53");
        }
        // Reko: a decoder for the instruction 2290 at address 9746 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2290()
        {
            AssertCode("@@@", "2290");
        }
        // Reko: a decoder for the instruction 8653 at address 976C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_8653()
        {
            AssertCode("@@@", "8653");
        }
        // Reko: a decoder for the instruction 6FDF at address 9782 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_6FDF()
        {
            AssertCode("@@@", "6FDF");
        }
        // Reko: a decoder for the instruction D253 at address 97AC has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_D253()
        {
            AssertCode("@@@", "D253");
        }
        // Reko: a decoder for the instruction 345A at address 97E6 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_345A()
        {
            AssertCode("@@@", "345A");
        }
        // Reko: a decoder for the instruction 645A at address 984C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_645A()
        {
            AssertCode("@@@", "645A");
        }
        // Reko: a decoder for the instruction 745A at address 986E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_745A()
        {
            AssertCode("@@@", "745A");
        }
        // Reko: a decoder for the instruction C45A at address 990C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_C45A()
        {
            AssertCode("@@@", "C45A");
        }
        // Reko: a decoder for the instruction D45A at address 992A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D45A()
        {
            AssertCode("@@@", "D45A");
        }
        // Reko: a decoder for the instruction E45A at address 9948 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_E45A()
        {
            AssertCode("@@@", "E45A");
        }
        // Reko: a decoder for the instruction F45A at address 9966 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F45A()
        {
            AssertCode("@@@", "F45A");
        }
        // Reko: a decoder for the instruction 100069339FDF at address 9986 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_100069339FDF()
        {
            AssertCode("@@@", "100069339FDF");
        }
        // Reko: a decoder for the instruction 4FBF at address 99B0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4FBF()
        {
            AssertCode("@@@", "4FBF");
        }
        // Reko: a decoder for the instruction 39FC at address 99B8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_39FC()
        {
            AssertCode("@@@", "39FC");
        }
        // Reko: a decoder for the instruction 8FBF at address 99C8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8FBF()
        {
            AssertCode("@@@", "8FBF");
        }
        // Reko: a decoder for the instruction 21FC at address 99D0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_21FC()
        {
            AssertCode("@@@", "21FC");
        }
        // Reko: a decoder for the instruction 9F9F at address 99D2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_9F9F()
        {
            AssertCode("@@@", "9F9F");
        }
        // Reko: a decoder for the instruction 8FAF at address 99EE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8FAF()
        {
            AssertCode("@@@", "8FAF");
        }
        // Reko: a decoder for the instruction 42D1 at address 99F8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42D1()
        {
            AssertCode("@@@", "42D1");
        }
        // Reko: a decoder for the instruction 9FFC at address 9A22 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FFC()
        {
            AssertCode("@@@", "9FFC");
        }
        // Reko: a decoder for the instruction FD5A at address 9A28 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_FD5A()
        {
            AssertCode("@@@", "FD5A");
        }
        // Reko: a decoder for the instruction 395A at address 9A3C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_395A()
        {
            AssertCode("@@@", "395A");
        }
        // Reko: a decoder for the instruction C9F9 at address 9A42 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C9F9()
        {
            AssertCode("@@@", "C9F9");
        }
        // Reko: a decoder for the instruction 495A at address 9A44 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_495A()
        {
            AssertCode("@@@", "495A");
        }
        // Reko: a decoder for the instruction 9D53 at address 9A46 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9D53()
        {
            AssertCode("@@@", "9D53");
        }
        // Reko: a decoder for the instruction C1F9 at address 9A4A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C1F9()
        {
            AssertCode("@@@", "C1F9");
        }
        // Reko: a decoder for the instruction 0DF9 at address 9A4E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DF9()
        {
            AssertCode("@@@", "0DF9");
        }
        // Reko: a decoder for the instruction B7F9 at address 9A54 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B7F9()
        {
            AssertCode("@@@", "B7F9");
        }
        // Reko: a decoder for the instruction 7D5A at address 9A5A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7D5A()
        {
            AssertCode("@@@", "7D5A");
        }
        // Reko: a decoder for the instruction A7F9 at address 9A64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A7F9()
        {
            AssertCode("@@@", "A7F9");
        }
        // Reko: a decoder for the instruction DD5A at address 9A6A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_DD5A()
        {
            AssertCode("@@@", "DD5A");
        }
        // Reko: a decoder for the instruction 97F9 at address 9A74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_97F9()
        {
            AssertCode("@@@", "97F9");
        }
        // Reko: a decoder for the instruction CD5A at address 9A7A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_CD5A()
        {
            AssertCode("@@@", "CD5A");
        }
        // Reko: a decoder for the instruction 87F9 at address 9A84 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_87F9()
        {
            AssertCode("@@@", "87F9");
        }
        // Reko: a decoder for the instruction 595A at address 9A8E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_595A()
        {
            AssertCode("@@@", "595A");
        }
        // Reko: a decoder for the instruction 79F9 at address 9A92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_79F9()
        {
            AssertCode("@@@", "79F9");
        }
        // Reko: a decoder for the instruction BFBF at address 9AA4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BFBF()
        {
            AssertCode("@@@", "BFBF");
        }
        // Reko: a decoder for the instruction CF9F at address 9AAC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_CF9F()
        {
            AssertCode("@@@", "CF9F");
        }
        // Reko: a decoder for the instruction 64D0 at address 9AC8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_64D0()
        {
            AssertCode("@@@", "64D0");
        }
        // Reko: a decoder for the instruction 65F8 at address 9ACE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_65F8()
        {
            AssertCode("@@@", "65F8");
        }
        // Reko: a decoder for the instruction 695A at address 9AE2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_695A()
        {
            AssertCode("@@@", "695A");
        }
        // Reko: a decoder for the instruction AD5A at address 9AEC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_AD5A()
        {
            AssertCode("@@@", "AD5A");
        }
        // Reko: a decoder for the instruction 15F9 at address 9AF6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_15F9()
        {
            AssertCode("@@@", "15F9");
        }
        // Reko: a decoder for the instruction 03F9 at address 9B08 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_03F9()
        {
            AssertCode("@@@", "03F9");
        }
        // Reko: a decoder for the instruction 295A at address 9B0C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_295A()
        {
            AssertCode("@@@", "295A");
        }
        // Reko: a decoder for the instruction FBF8 at address 9B10 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FBF8()
        {
            AssertCode("@@@", "FBF8");
        }
        // Reko: a decoder for the instruction 795A at address 9B12 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_795A()
        {
            AssertCode("@@@", "795A");
        }
        // Reko: a decoder for the instruction F5F8 at address 9B16 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F5F8()
        {
            AssertCode("@@@", "F5F8");
        }
        // Reko: a decoder for the instruction 8D5A at address 9B1C has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8D5A()
        {
            AssertCode("@@@", "8D5A");
        }
        // Reko: a decoder for the instruction E5F8 at address 9B26 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E5F8()
        {
            AssertCode("@@@", "E5F8");
        }
        // Reko: a decoder for the instruction DDF8 at address 9B2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DDF8()
        {
            AssertCode("@@@", "DDF8");
        }
        // Reko: a decoder for the instruction CBF8 at address 9B40 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CBF8()
        {
            AssertCode("@@@", "CBF8");
        }
        // Reko: a decoder for the instruction 895A at address 9B42 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_895A()
        {
            AssertCode("@@@", "895A");
        }
        // Reko: a decoder for the instruction C5F8 at address 9B46 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C5F8()
        {
            AssertCode("@@@", "C5F8");
        }
        // Reko: a decoder for the instruction 3D5A at address 9B54 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3D5A()
        {
            AssertCode("@@@", "3D5A");
        }
        // Reko: a decoder for the instruction AFF8 at address 9B5C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFF8()
        {
            AssertCode("@@@", "AFF8");
        }
        // Reko: a decoder for the instruction A9F8 at address 9B62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A9F8()
        {
            AssertCode("@@@", "A9F8");
        }
        // Reko: a decoder for the instruction 5D5A at address 9B68 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5D5A()
        {
            AssertCode("@@@", "5D5A");
        }
        // Reko: a decoder for the instruction C3FE at address 9B6E has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_C3FE()
        {
            AssertCode("@@@", "C3FE");
        }
        // Reko: a decoder for the instruction 97F8 at address 9B74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_97F8()
        {
            AssertCode("@@@", "97F8");
        }
        // Reko: a decoder for the instruction 91F8 at address 9B7A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_91F8()
        {
            AssertCode("@@@", "91F8");
        }
        // Reko: a decoder for the instruction 4D5A at address 9B80 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4D5A()
        {
            AssertCode("@@@", "4D5A");
        }
        // Reko: a decoder for the instruction 2D5A at address 9B92 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2D5A()
        {
            AssertCode("@@@", "2D5A");
        }
        // Reko: a decoder for the instruction 71F8 at address 9B9A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_71F8()
        {
            AssertCode("@@@", "71F8");
        }
        // Reko: a decoder for the instruction 6BF8 at address 9BA0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6BF8()
        {
            AssertCode("@@@", "6BF8");
        }
        // Reko: a decoder for the instruction D95A at address 9BA2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D95A()
        {
            AssertCode("@@@", "D95A");
        }
        // Reko: a decoder for the instruction 5DF8 at address 9BAE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5DF8()
        {
            AssertCode("@@@", "5DF8");
        }
        // Reko: a decoder for the instruction A95A at address 9BB0 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A95A()
        {
            AssertCode("@@@", "A95A");
        }
        // Reko: a decoder for the instruction 57F8 at address 9BB4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_57F8()
        {
            AssertCode("@@@", "57F8");
        }
        // Reko: a decoder for the instruction 4FF8 at address 9BBC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FF8()
        {
            AssertCode("@@@", "4FF8");
        }
        // Reko: a decoder for the instruction C95A at address 9BBE has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_C95A()
        {
            AssertCode("@@@", "C95A");
        }
        // Reko: a decoder for the instruction 49F8 at address 9BC2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_49F8()
        {
            AssertCode("@@@", "49F8");
        }
        // Reko: a decoder for the instruction 41F8 at address 9BCA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_41F8()
        {
            AssertCode("@@@", "41F8");
        }
        // Reko: a decoder for the instruction E95A at address 9BCC has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_E95A()
        {
            AssertCode("@@@", "E95A");
        }
        // Reko: a decoder for the instruction 3BF8 at address 9BD0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3BF8()
        {
            AssertCode("@@@", "3BF8");
        }
        // Reko: a decoder for the instruction 5FDF at address 9BDE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5FDF()
        {
            AssertCode("@@@", "5FDF");
        }
        // Reko: a decoder for the instruction 7FEF at address 9BEE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7FEF()
        {
            AssertCode("@@@", "7FEF");
        }
        // Reko: a decoder for the instruction D491 at address 9C0E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D491()
        {
            AssertCode("@@@", "D491");
        }
        // Reko: a decoder for the instruction 0A04 at address 9C14 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0A04()
        {
            AssertCode("@@@", "0A04");
        }
        // Reko: a decoder for the instruction 425A at address 9C16 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_425A()
        {
            AssertCode("@@@", "425A");
        }
        // Reko: a decoder for the instruction 6E54 at address 9C20 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_6E54()
        {
            AssertCode("@@@", "6E54");
        }
        // Reko: a decoder for the instruction 264C at address 9C26 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_264C()
        {
            AssertCode("@@@", "264C");
        }
        // Reko: a decoder for the instruction A660 at address 9C28 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A660()
        {
            AssertCode("@@@", "A660");
        }
        // Reko: a decoder for the instruction 8FE2 at address 9C2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FE2()
        {
            AssertCode("@@@", "8FE2");
        }
        // Reko: a decoder for the instruction A953 at address 9C3A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_A953()
        {
            AssertCode("@@@", "A953");
        }
        // Reko: a decoder for the instruction 540A at address 9C56 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_540A()
        {
            AssertCode("@@@", "540A");
        }
        // Reko: a decoder for the instruction 7545 at address 9C5E has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_7545()
        {
            AssertCode("@@@", "7545");
        }
        // Reko: a decoder for the instruction 3527 at address 9C60 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3527()
        {
            AssertCode("@@@", "3527");
        }
        // Reko: a decoder for the instruction 1000103B5CDF at address 9C64 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000103B5CDF()
        {
            AssertCode("@@@", "1000103B5CDF");
        }
        // Reko: a decoder for the instruction 6CAA at address 9C72 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6CAA()
        {
            AssertCode("@@@", "6CAA");
        }
        // Reko: a decoder for the instruction 8CEA at address 9C78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CEA()
        {
            AssertCode("@@@", "8CEA");
        }
        // Reko: a decoder for the instruction 52F0 at address 9C7C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_52F0()
        {
            AssertCode("@@@", "52F0");
        }
        // Reko: a decoder for the instruction 1345 at address 9CBE has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1345()
        {
            AssertCode("@@@", "1345");
        }
        // Reko: a decoder for the instruction 5327 at address 9CC0 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5327()
        {
            AssertCode("@@@", "5327");
        }
        // Reko: a decoder for the instruction 1000203B3CDF at address 9CC4 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000203B3CDF()
        {
            AssertCode("@@@", "1000203B3CDF");
        }
        // Reko: a decoder for the instruction 3039 at address 9CFC has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3039()
        {
            AssertCode("@@@", "3039");
        }
        // Reko: a decoder for the instruction 0346 at address 9D00 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0346()
        {
            AssertCode("@@@", "0346");
        }
        // Reko: a decoder for the instruction 1B52 at address 9D10 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1B52()
        {
            AssertCode("@@@", "1B52");
        }
        // Reko: a decoder for the instruction 1B45 at address 9D1E has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1B45()
        {
            AssertCode("@@@", "1B45");
        }
        // Reko: a decoder for the instruction 1000603BBCDF at address 9D24 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000603BBCDF()
        {
            AssertCode("@@@", "1000603BBCDF");
        }
        // Reko: a decoder for the instruction 4CEA at address 9D3A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CEA()
        {
            AssertCode("@@@", "4CEA");
        }
        // Reko: a decoder for the instruction B0F0 at address 9D3E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B0F0()
        {
            AssertCode("@@@", "B0F0");
        }
        // Reko: a decoder for the instruction 0946 at address 9D60 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0946()
        {
            AssertCode("@@@", "0946");
        }
        // Reko: a decoder for the instruction 9CDF at address 9D62 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_9CDF()
        {
            AssertCode("@@@", "9CDF");
        }
        // Reko: a decoder for the instruction 2633 at address 9D6A has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2633()
        {
            AssertCode("@@@", "2633");
        }
        // Reko: a decoder for the instruction A353 at address 9D78 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_A353()
        {
            AssertCode("@@@", "A353");
        }
        // Reko: a decoder for the instruction 4E60 at address 9D88 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4E60()
        {
            AssertCode("@@@", "4E60");
        }
        // Reko: a decoder for the instruction 8FA2 at address 9D8A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FA2()
        {
            AssertCode("@@@", "8FA2");
        }
        // Reko: a decoder for the instruction 9E90 at address 9D92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9E90()
        {
            AssertCode("@@@", "9E90");
        }
        // Reko: a decoder for the instruction 9FD0 at address 9D94 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FD0()
        {
            AssertCode("@@@", "9FD0");
        }
        // Reko: a decoder for the instruction 6A53 at address 9DAE has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6A53()
        {
            AssertCode("@@@", "6A53");
        }
        // Reko: a decoder for the instruction 409F at address 9DB8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_409F()
        {
            AssertCode("@@@", "409F");
        }
        // Reko: a decoder for the instruction 509F at address 9DBC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_509F()
        {
            AssertCode("@@@", "509F");
        }
        // Reko: a decoder for the instruction 0327 at address 9DC4 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0327()
        {
            AssertCode("@@@", "0327");
        }
        // Reko: a decoder for the instruction 1000403B3CDF at address 9DC8 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000403B3CDF()
        {
            AssertCode("@@@", "1000403B3CDF");
        }
        // Reko: a decoder for the instruction 6CEA at address 9DF2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6CEA()
        {
            AssertCode("@@@", "6CEA");
        }
        // Reko: a decoder for the instruction 8039 at address 9E02 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8039()
        {
            AssertCode("@@@", "8039");
        }
        // Reko: a decoder for the instruction 940A at address 9E1C has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_940A()
        {
            AssertCode("@@@", "940A");
        }
        // Reko: a decoder for the instruction 920A at address 9E20 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_920A()
        {
            AssertCode("@@@", "920A");
        }
        // Reko: a decoder for the instruction 7945 at address 9E24 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_7945()
        {
            AssertCode("@@@", "7945");
        }
        // Reko: a decoder for the instruction 3927 at address 9E26 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3927()
        {
            AssertCode("@@@", "3927");
        }
        // Reko: a decoder for the instruction 1000023B9CDF at address 9E2A has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000023B9CDF()
        {
            AssertCode("@@@", "1000023B9CDF");
        }
        // Reko: a decoder for the instruction 4CAA at address 9E3A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CAA()
        {
            AssertCode("@@@", "4CAA");
        }
        // Reko: a decoder for the instruction 92F0 at address 9E44 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_92F0()
        {
            AssertCode("@@@", "92F0");
        }
        // Reko: a decoder for the instruction 9C9F at address 9E46 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_9C9F()
        {
            AssertCode("@@@", "9C9F");
        }
        // Reko: a decoder for the instruction 9239 at address 9E64 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9239()
        {
            AssertCode("@@@", "9239");
        }
        // Reko: a decoder for the instruction 2946 at address 9E68 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_2946()
        {
            AssertCode("@@@", "2946");
        }
        // Reko: a decoder for the instruction E052 at address 9E86 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_E052()
        {
            AssertCode("@@@", "E052");
        }
        // Reko: a decoder for the instruction 2146 at address 9EB8 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_2146()
        {
            AssertCode("@@@", "2146");
        }
        // Reko: a decoder for the instruction AFFE at address 9ECC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_AFFE()
        {
            AssertCode("@@@", "AFFE");
        }
        // Reko: a decoder for the instruction A152 at address 9EEA has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A152()
        {
            AssertCode("@@@", "A152");
        }
        // Reko: a decoder for the instruction 980A at address 9EF0 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_980A()
        {
            AssertCode("@@@", "980A");
        }
        // Reko: a decoder for the instruction 960A at address 9EF4 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_960A()
        {
            AssertCode("@@@", "960A");
        }
        // Reko: a decoder for the instruction 1000803B9CDF at address 9EFE has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000803B9CDF()
        {
            AssertCode("@@@", "1000803B9CDF");
        }
        // Reko: a decoder for the instruction 7833 at address 9F0C has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7833()
        {
            AssertCode("@@@", "7833");
        }
        // Reko: a decoder for the instruction BB32 at address 9F12 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BB32()
        {
            AssertCode("@@@", "BB32");
        }
        // Reko: a decoder for the instruction D852 at address 9F20 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D852()
        {
            AssertCode("@@@", "D852");
        }
        // Reko: a decoder for the instruction 90F0 at address 9F32 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90F0()
        {
            AssertCode("@@@", "90F0");
        }
        // Reko: a decoder for the instruction 8139 at address 9F52 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_8139()
        {
            AssertCode("@@@", "8139");
        }
        // Reko: a decoder for the instruction 1B46 at address 9F54 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1B46()
        {
            AssertCode("@@@", "1B46");
        }
        // Reko: a decoder for the instruction F3FF at address 9F5C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F3FF()
        {
            AssertCode("@@@", "F3FF");
        }
        // Reko: a decoder for the instruction 0FFE at address 9F64 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_0FFE()
        {
            AssertCode("@@@", "0FFE");
        }
        // Reko: a decoder for the instruction 605A at address 9F6A has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_605A()
        {
            AssertCode("@@@", "605A");
        }
        // Reko: a decoder for the instruction 2032 at address 9F74 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2032()
        {
            AssertCode("@@@", "2032");
        }
        // Reko: a decoder for the instruction F7FD at address 9F7C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F7FD()
        {
            AssertCode("@@@", "F7FD");
        }
        // Reko: a decoder for the instruction 9C0A at address 9F8A has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_9C0A()
        {
            AssertCode("@@@", "9C0A");
        }
        // Reko: a decoder for the instruction 9A0A at address 9F8E has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_9A0A()
        {
            AssertCode("@@@", "9A0A");
        }
        // Reko: a decoder for the instruction 1000803B3CDF at address 9F98 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000803B3CDF()
        {
            AssertCode("@@@", "1000803B3CDF");
        }
        // Reko: a decoder for the instruction F6FF at address 9FAC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F6FF()
        {
            AssertCode("@@@", "F6FF");
        }
        // Reko: a decoder for the instruction 8645 at address 9FB0 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_8645()
        {
            AssertCode("@@@", "8645");
        }
        // Reko: a decoder for the instruction 3627 at address 9FB2 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3627()
        {
            AssertCode("@@@", "3627");
        }
        // Reko: a decoder for the instruction 2139 at address 9FEA has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2139()
        {
            AssertCode("@@@", "2139");
        }
        // Reko: a decoder for the instruction 77FD at address 9FFC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_77FD()
        {
            AssertCode("@@@", "77FD");
        }
        // Reko: a decoder for the instruction 6133 at address 9FFE has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6133()
        {
            AssertCode("@@@", "6133");
        }
        // Reko: a decoder for the instruction 6DFD at address A006 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DFD()
        {
            AssertCode("@@@", "6DFD");
        }
        // Reko: a decoder for the instruction 7DFD at address A00E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7DFD()
        {
            AssertCode("@@@", "7DFD");
        }
        // Reko: a decoder for the instruction 4733 at address A010 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4733()
        {
            AssertCode("@@@", "4733");
        }
        // Reko: a decoder for the instruction F9FB at address A024 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F9FB()
        {
            AssertCode("@@@", "F9FB");
        }
        // Reko: a decoder for the instruction 1000703BA959 at address A04E has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000703BA959()
        {
            AssertCode("@@@", "1000703BA959");
        }
        // Reko: a decoder for the instruction 0939 at address A054 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0939()
        {
            AssertCode("@@@", "0939");
        }
        // Reko: a decoder for the instruction 9346 at address A058 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_9346()
        {
            AssertCode("@@@", "9346");
        }
        // Reko: a decoder for the instruction 7832 at address A07C has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7832()
        {
            AssertCode("@@@", "7832");
        }
        // Reko: a decoder for the instruction EFFC at address A084 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EFFC()
        {
            AssertCode("@@@", "EFFC");
        }
        // Reko: a decoder for the instruction 1000703BA459 at address A0AE has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000703BA459()
        {
            AssertCode("@@@", "1000703BA459");
        }
        // Reko: a decoder for the instruction 0439 at address A0B4 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0439()
        {
            AssertCode("@@@", "0439");
        }
        // Reko: a decoder for the instruction 4946 at address A0B8 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4946()
        {
            AssertCode("@@@", "4946");
        }
        // Reko: a decoder for the instruction 4FFE at address A0DA has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_4FFE()
        {
            AssertCode("@@@", "4FFE");
        }
        // Reko: a decoder for the instruction 3832 at address A0DC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3832()
        {
            AssertCode("@@@", "3832");
        }
        // Reko: a decoder for the instruction 8FFC at address A0E4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FFC()
        {
            AssertCode("@@@", "8FFC");
        }
        // Reko: a decoder for the instruction E290 at address A11C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E290()
        {
            AssertCode("@@@", "E290");
        }
        // Reko: a decoder for the instruction 1B32 at address A11E has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1B32()
        {
            AssertCode("@@@", "1B32");
        }
        // Reko: a decoder for the instruction 0E52 at address A128 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_0E52()
        {
            AssertCode("@@@", "0E52");
        }
        // Reko: a decoder for the instruction 6045 at address A13A has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_6045()
        {
            AssertCode("@@@", "6045");
        }
        // Reko: a decoder for the instruction 0A27 at address A13C has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0A27()
        {
            AssertCode("@@@", "0A27");
        }
        // Reko: a decoder for the instruction 1000203BACDF at address A140 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000203BACDF()
        {
            AssertCode("@@@", "1000203BACDF");
        }
        // Reko: a decoder for the instruction 6053 at address A148 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6053()
        {
            AssertCode("@@@", "6053");
        }
        // Reko: a decoder for the instruction 1000103BE959 at address A176 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000103BE959()
        {
            AssertCode("@@@", "1000103BE959");
        }
        // Reko: a decoder for the instruction 9A46 at address A180 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_9A46()
        {
            AssertCode("@@@", "9A46");
        }
        // Reko: a decoder for the instruction 0204 at address A19C has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0204()
        {
            AssertCode("@@@", "0204");
        }
        // Reko: a decoder for the instruction 6945 at address A1A4 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_6945()
        {
            AssertCode("@@@", "6945");
        }
        // Reko: a decoder for the instruction 9A27 at address A1A6 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9A27()
        {
            AssertCode("@@@", "9A27");
        }
        // Reko: a decoder for the instruction 1000235B133B at address A1AA has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000235B133B()
        {
            AssertCode("@@@", "1000235B133B");
        }
        // Reko: a decoder for the instruction 6353 at address A1B4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6353()
        {
            AssertCode("@@@", "6353");
        }
        // Reko: a decoder for the instruction 0291 at address A1DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0291()
        {
            AssertCode("@@@", "0291");
        }
        // Reko: a decoder for the instruction 1290 at address A1E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1290()
        {
            AssertCode("@@@", "1290");
        }
        // Reko: a decoder for the instruction 6245 at address A1E4 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_6245()
        {
            AssertCode("@@@", "6245");
        }
        // Reko: a decoder for the instruction 2A27 at address A1E6 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2A27()
        {
            AssertCode("@@@", "2A27");
        }
        // Reko: a decoder for the instruction 1000023BACDF at address A1EA has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000023BACDF()
        {
            AssertCode("@@@", "1000023BACDF");
        }
        // Reko: a decoder for the instruction A2F0 at address A204 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A2F0()
        {
            AssertCode("@@@", "A2F0");
        }
        // Reko: a decoder for the instruction 4339 at address A224 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4339()
        {
            AssertCode("@@@", "4339");
        }
        // Reko: a decoder for the instruction 3A46 at address A228 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_3A46()
        {
            AssertCode("@@@", "3A46");
        }
        // Reko: a decoder for the instruction F4F2 at address A24C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F4F2()
        {
            AssertCode("@@@", "F4F2");
        }
        // Reko: a decoder for the instruction 0090 at address A24E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0090()
        {
            AssertCode("@@@", "0090");
        }
        // Reko: a decoder for the instruction 0A46 at address A298 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0A46()
        {
            AssertCode("@@@", "0A46");
        }
        // Reko: a decoder for the instruction 3633 at address A2A4 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3633()
        {
            AssertCode("@@@", "3633");
        }
        // Reko: a decoder for the instruction 9932 at address A2AC has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9932()
        {
            AssertCode("@@@", "9932");
        }
        // Reko: a decoder for the instruction B952 at address A2AE has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B952()
        {
            AssertCode("@@@", "B952");
        }
        // Reko: a decoder for the instruction E0B0 at address A2BA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E0B0()
        {
            AssertCode("@@@", "E0B0");
        }
        // Reko: a decoder for the instruction 1291 at address A2C8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1291()
        {
            AssertCode("@@@", "1291");
        }
        // Reko: a decoder for the instruction 8290 at address A2CA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8290()
        {
            AssertCode("@@@", "8290");
        }
        // Reko: a decoder for the instruction 1000103BACDF at address A2D4 has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000103BACDF()
        {
            AssertCode("@@@", "1000103BACDF");
        }
        // Reko: a decoder for the instruction EE61 at address A322 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_EE61()
        {
            AssertCode("@@@", "EE61");
        }
        // Reko: a decoder for the instruction E004 at address A330 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_E004()
        {
            AssertCode("@@@", "E004");
        }
        // Reko: a decoder for the instruction B8F2 at address A332 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B8F2()
        {
            AssertCode("@@@", "B8F2");
        }
        // Reko: a decoder for the instruction 0E90 at address A334 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0E90()
        {
            AssertCode("@@@", "0E90");
        }
        // Reko: a decoder for the instruction 83FD at address A390 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_83FD()
        {
            AssertCode("@@@", "83FD");
        }
        // Reko: a decoder for the instruction 0633 at address A3B2 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0633()
        {
            AssertCode("@@@", "0633");
        }
        // Reko: a decoder for the instruction 4DFD at address A3C6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DFD()
        {
            AssertCode("@@@", "4DFD");
        }
        // Reko: a decoder for the instruction D3FD at address A3CA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D3FD()
        {
            AssertCode("@@@", "D3FD");
        }
        // Reko: a decoder for the instruction 4CA4 at address A3D6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CA4()
        {
            AssertCode("@@@", "4CA4");
        }
        // Reko: a decoder for the instruction 3046 at address A400 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_3046()
        {
            AssertCode("@@@", "3046");
        }
        // Reko: a decoder for the instruction ACFF at address A41C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ACFF()
        {
            AssertCode("@@@", "ACFF");
        }
        // Reko: a decoder for the instruction E0A0 at address A428 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E0A0()
        {
            AssertCode("@@@", "E0A0");
        }
        // Reko: a decoder for the instruction 2095 at address A42A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2095()
        {
            AssertCode("@@@", "2095");
        }
        // Reko: a decoder for the instruction 2FDF at address A42C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2FDF()
        {
            AssertCode("@@@", "2FDF");
        }
        // Reko: a decoder for the instruction AE0F at address A432 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_AE0F()
        {
            AssertCode("@@@", "AE0F");
        }
        // Reko: a decoder for the instruction B00F at address A43A has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_B00F()
        {
            AssertCode("@@@", "B00F");
        }
        // Reko: a decoder for the instruction BA0A at address A440 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_BA0A()
        {
            AssertCode("@@@", "BA0A");
        }
        // Reko: a decoder for the instruction B20F at address A446 has not been implemented. (Fmt15 1 ZZ  bne0w disp4 (2-32) 4 src reg 4 dest disp*2+)
        [Test]
        public void Cr16Dasm_B20F()
        {
            AssertCode("@@@", "B20F");
        }
        // Reko: a decoder for the instruction 7FDF at address A44E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_7FDF()
        {
            AssertCode("@@@", "7FDF");
        }
        // Reko: a decoder for the instruction 0482 at address A472 has not been implemented. (Fmt15 1 ZZ  storb imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_0482()
        {
            AssertCode("@@@", "0482");
        }
        // Reko: a decoder for the instruction B633 at address A4A6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B633()
        {
            AssertCode("@@@", "B633");
        }
        // Reko: a decoder for the instruction C661 at address A4AC has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_C661()
        {
            AssertCode("@@@", "C661");
        }
        // Reko: a decoder for the instruction 06DF at address A4D0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_06DF()
        {
            AssertCode("@@@", "06DF");
        }
        // Reko: a decoder for the instruction 340B at address A4D2 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_340B()
        {
            AssertCode("@@@", "340B");
        }
        // Reko: a decoder for the instruction 0083 at address A4DA has not been implemented. (Fmt16 2 ZZ  storb imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0083()
        {
            AssertCode("@@@", "0083");
        }
        // Reko: a decoder for the instruction 71D2 at address A508 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_71D2()
        {
            AssertCode("@@@", "71D2");
        }
        // Reko: a decoder for the instruction FA43 at address A50C has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_FA43()
        {
            AssertCode("@@@", "FA43");
        }
        // Reko: a decoder for the instruction 2FEF at address A51A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2FEF()
        {
            AssertCode("@@@", "2FEF");
        }
        // Reko: a decoder for the instruction AA33 at address A51E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_AA33()
        {
            AssertCode("@@@", "AA33");
        }
        // Reko: a decoder for the instruction 439F at address A52C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_439F()
        {
            AssertCode("@@@", "439F");
        }
        // Reko: a decoder for the instruction 4FDF at address A530 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_4FDF()
        {
            AssertCode("@@@", "4FDF");
        }
        // Reko: a decoder for the instruction 5A53 at address A53C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_5A53()
        {
            AssertCode("@@@", "5A53");
        }
        // Reko: a decoder for the instruction FA08 at address A540 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_FA08()
        {
            AssertCode("@@@", "FA08");
        }
        // Reko: a decoder for the instruction 849F at address A566 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_849F()
        {
            AssertCode("@@@", "849F");
        }
        // Reko: a decoder for the instruction EA53 at address A576 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_EA53()
        {
            AssertCode("@@@", "EA53");
        }
        // Reko: a decoder for the instruction 749F at address A57C has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_749F()
        {
            AssertCode("@@@", "749F");
        }
        // Reko: a decoder for the instruction 360B at address A57E has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_360B()
        {
            AssertCode("@@@", "360B");
        }
        // Reko: a decoder for the instruction 4490 at address A58C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4490()
        {
            AssertCode("@@@", "4490");
        }
        // Reko: a decoder for the instruction 1453 at address A58E has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1453()
        {
            AssertCode("@@@", "1453");
        }
        // Reko: a decoder for the instruction 1953 at address A59C has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1953()
        {
            AssertCode("@@@", "1953");
        }
        // Reko: a decoder for the instruction AFAF at address A5D6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AFAF()
        {
            AssertCode("@@@", "AFAF");
        }
        // Reko: a decoder for the instruction BA60 at address A5DA has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BA60()
        {
            AssertCode("@@@", "BA60");
        }
        // Reko: a decoder for the instruction AFEF at address A5DE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_AFEF()
        {
            AssertCode("@@@", "AFEF");
        }
        // Reko: a decoder for the instruction 0561 at address A61E has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0561()
        {
            AssertCode("@@@", "0561");
        }
        // Reko: a decoder for the instruction 3FDF at address A648 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_3FDF()
        {
            AssertCode("@@@", "3FDF");
        }
        // Reko: a decoder for the instruction 549F at address A652 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_549F()
        {
            AssertCode("@@@", "549F");
        }
        // Reko: a decoder for the instruction 1952 at address A65E has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1952()
        {
            AssertCode("@@@", "1952");
        }
        // Reko: a decoder for the instruction 1B5A at address A66E has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1B5A()
        {
            AssertCode("@@@", "1B5A");
        }
        // Reko: a decoder for the instruction 2A5A at address A670 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2A5A()
        {
            AssertCode("@@@", "2A5A");
        }
        // Reko: a decoder for the instruction AE53 at address A69A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_AE53()
        {
            AssertCode("@@@", "AE53");
        }
        // Reko: a decoder for the instruction 0ADF at address A6FE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0ADF()
        {
            AssertCode("@@@", "0ADF");
        }
        // Reko: a decoder for the instruction D661 at address A720 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D661()
        {
            AssertCode("@@@", "D661");
        }
        // Reko: a decoder for the instruction DFEF at address A72A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DFEF()
        {
            AssertCode("@@@", "DFEF");
        }
        // Reko: a decoder for the instruction A490 at address A738 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A490()
        {
            AssertCode("@@@", "A490");
        }
        // Reko: a decoder for the instruction A833 at address A73A has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A833()
        {
            AssertCode("@@@", "A833");
        }
        // Reko: a decoder for the instruction 8AD0 at address A740 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8AD0()
        {
            AssertCode("@@@", "8AD0");
        }
        // Reko: a decoder for the instruction 00BF at address A744 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_00BF()
        {
            AssertCode("@@@", "00BF");
        }
        // Reko: a decoder for the instruction 12BF at address A74A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_12BF()
        {
            AssertCode("@@@", "12BF");
        }
        // Reko: a decoder for the instruction 1051 at address A74E has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1051()
        {
            AssertCode("@@@", "1051");
        }
        // Reko: a decoder for the instruction 1030 at address A754 has not been implemented. (ZZ addb imm4/16,reg 4 dest reg 4 src imm 15/16 1/2)
        [Test]
        public void Cr16Dasm_1030()
        {
            AssertCode("@@@", "1030");
        }
        // Reko: a decoder for the instruction 24D1 at address A762 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_24D1()
        {
            AssertCode("@@@", "24D1");
        }
        // Reko: a decoder for the instruction 26D1 at address A764 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_26D1()
        {
            AssertCode("@@@", "26D1");
        }
        // Reko: a decoder for the instruction E490 at address A77E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E490()
        {
            AssertCode("@@@", "E490");
        }
        // Reko: a decoder for the instruction 649F at address A7A6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_649F()
        {
            AssertCode("@@@", "649F");
        }
        // Reko: a decoder for the instruction B0D0 at address A7F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B0D0()
        {
            AssertCode("@@@", "B0D0");
        }
        // Reko: a decoder for the instruction 4160 at address A804 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4160()
        {
            AssertCode("@@@", "4160");
        }
        // Reko: a decoder for the instruction 1FEF at address A806 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_1FEF()
        {
            AssertCode("@@@", "1FEF");
        }
        // Reko: a decoder for the instruction 0DFE at address A85A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_0DFE()
        {
            AssertCode("@@@", "0DFE");
        }
        // Reko: a decoder for the instruction 87FE at address A862 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_87FE()
        {
            AssertCode("@@@", "87FE");
        }
        // Reko: a decoder for the instruction 59FC at address A874 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_59FC()
        {
            AssertCode("@@@", "59FC");
        }
        // Reko: a decoder for the instruction 46BF at address A87A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_46BF()
        {
            AssertCode("@@@", "46BF");
        }
        // Reko: a decoder for the instruction 22BF at address A87E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_22BF()
        {
            AssertCode("@@@", "22BF");
        }
        // Reko: a decoder for the instruction 2451 at address A882 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2451()
        {
            AssertCode("@@@", "2451");
        }
        // Reko: a decoder for the instruction 17FD at address A886 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_17FD()
        {
            AssertCode("@@@", "17FD");
        }
        // Reko: a decoder for the instruction 11FD at address A88E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_11FD()
        {
            AssertCode("@@@", "11FD");
        }
        // Reko: a decoder for the instruction 21FE at address A8A0 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_21FE()
        {
            AssertCode("@@@", "21FE");
        }
        // Reko: a decoder for the instruction 1BFE at address A8A8 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_1BFE()
        {
            AssertCode("@@@", "1BFE");
        }
        // Reko: a decoder for the instruction 5FAF at address A8C4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_5FAF()
        {
            AssertCode("@@@", "5FAF");
        }
        // Reko: a decoder for the instruction 45BF at address A8C8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_45BF()
        {
            AssertCode("@@@", "45BF");
        }
        // Reko: a decoder for the instruction 1451 at address A8D0 has not been implemented. (Fmt15 1 ZZ  cmpb reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1451()
        {
            AssertCode("@@@", "1451");
        }
        // Reko: a decoder for the instruction 45B0 at address A8E0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_45B0()
        {
            AssertCode("@@@", "45B0");
        }
        // Reko: a decoder for the instruction F5FD at address A8EE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F5FD()
        {
            AssertCode("@@@", "F5FD");
        }
        // Reko: a decoder for the instruction A7FC at address A902 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A7FC()
        {
            AssertCode("@@@", "A7FC");
        }
        // Reko: a decoder for the instruction B9FC at address A906 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_B9FC()
        {
            AssertCode("@@@", "B9FC");
        }
        // Reko: a decoder for the instruction A9FD at address A91A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A9FD()
        {
            AssertCode("@@@", "A9FD");
        }
        // Reko: a decoder for the instruction 7BFC at address A924 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7BFC()
        {
            AssertCode("@@@", "7BFC");
        }
        // Reko: a decoder for the instruction 31FE at address A94C has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_31FE()
        {
            AssertCode("@@@", "31FE");
        }
        // Reko: a decoder for the instruction 7096 at address A996 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7096()
        {
            AssertCode("@@@", "7096");
        }
        // Reko: a decoder for the instruction 140B at address A99C has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_140B()
        {
            AssertCode("@@@", "140B");
        }
        // Reko: a decoder for the instruction 20E2 at address A9A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20E2()
        {
            AssertCode("@@@", "20E2");
        }
        // Reko: a decoder for the instruction 20E4 at address A9A6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20E4()
        {
            AssertCode("@@@", "20E4");
        }
        // Reko: a decoder for the instruction 20E6 at address A9A8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20E6()
        {
            AssertCode("@@@", "20E6");
        }
        // Reko: a decoder for the instruction 20E8 at address A9AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20E8()
        {
            AssertCode("@@@", "20E8");
        }
        // Reko: a decoder for the instruction EA61 at address A9B8 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_EA61()
        {
            AssertCode("@@@", "EA61");
        }
        // Reko: a decoder for the instruction 7804 at address A9D2 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_7804()
        {
            AssertCode("@@@", "7804");
        }
        // Reko: a decoder for the instruction 2491 at address AA04 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2491()
        {
            AssertCode("@@@", "2491");
        }
        // Reko: a decoder for the instruction 169F at address AA38 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_169F()
        {
            AssertCode("@@@", "169F");
        }
        // Reko: a decoder for the instruction 16DF at address AA3E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_16DF()
        {
            AssertCode("@@@", "16DF");
        }
        // Reko: a decoder for the instruction 8490 at address AA5E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8490()
        {
            AssertCode("@@@", "8490");
        }
        // Reko: a decoder for the instruction 9B5A at address AAB6 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9B5A()
        {
            AssertCode("@@@", "9B5A");
        }
        // Reko: a decoder for the instruction 7633 at address AACA has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_7633()
        {
            AssertCode("@@@", "7633");
        }
        // Reko: a decoder for the instruction 100B at address AAFC has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_100B()
        {
            AssertCode("@@@", "100B");
        }
        // Reko: a decoder for the instruction 0E0B at address AB08 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0E0B()
        {
            AssertCode("@@@", "0E0B");
        }
        // Reko: a decoder for the instruction 0A0B at address AB24 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0A0B()
        {
            AssertCode("@@@", "0A0B");
        }
        // Reko: a decoder for the instruction 060B at address AB44 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_060B()
        {
            AssertCode("@@@", "060B");
        }
        // Reko: a decoder for the instruction 040B at address AB56 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_040B()
        {
            AssertCode("@@@", "040B");
        }
        // Reko: a decoder for the instruction 020B at address AB66 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_020B()
        {
            AssertCode("@@@", "020B");
        }
        // Reko: a decoder for the instruction 000B at address AB78 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_000B()
        {
            AssertCode("@@@", "000B");
        }
        // Reko: a decoder for the instruction F5FF at address AB80 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F5FF()
        {
            AssertCode("@@@", "F5FF");
        }
        // Reko: a decoder for the instruction FE0A at address AB88 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_FE0A()
        {
            AssertCode("@@@", "FE0A");
        }
        // Reko: a decoder for the instruction FA0A at address ABAA has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_FA0A()
        {
            AssertCode("@@@", "FA0A");
        }
        // Reko: a decoder for the instruction F80A at address ABBC has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_F80A()
        {
            AssertCode("@@@", "F80A");
        }
        // Reko: a decoder for the instruction F1FF at address ABC4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_F1FF()
        {
            AssertCode("@@@", "F1FF");
        }
        // Reko: a decoder for the instruction F60A at address ABCC has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_F60A()
        {
            AssertCode("@@@", "F60A");
        }
        // Reko: a decoder for the instruction F40A at address ABDC has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_F40A()
        {
            AssertCode("@@@", "F40A");
        }
        // Reko: a decoder for the instruction 2232 at address ABF6 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2232()
        {
            AssertCode("@@@", "2232");
        }
        // Reko: a decoder for the instruction 120B at address AC12 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_120B()
        {
            AssertCode("@@@", "120B");
        }
        // Reko: a decoder for the instruction EFEF at address AC22 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_EFEF()
        {
            AssertCode("@@@", "EFEF");
        }
        // Reko: a decoder for the instruction BA9F at address AC3A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_BA9F()
        {
            AssertCode("@@@", "BA9F");
        }
        // Reko: a decoder for the instruction 0891 at address AC78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0891()
        {
            AssertCode("@@@", "0891");
        }
        // Reko: a decoder for the instruction 2053 at address AC7E has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_2053()
        {
            AssertCode("@@@", "2053");
        }
        // Reko: a decoder for the instruction 38D1 at address ACA2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_38D1()
        {
            AssertCode("@@@", "38D1");
        }
        // Reko: a decoder for the instruction 9A60 at address ACBC has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9A60()
        {
            AssertCode("@@@", "9A60");
        }
        // Reko: a decoder for the instruction 0FD1 at address ACDC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD1()
        {
            AssertCode("@@@", "0FD1");
        }
        // Reko: a decoder for the instruction 160B at address ACE0 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_160B()
        {
            AssertCode("@@@", "160B");
        }
        // Reko: a decoder for the instruction 180B at address ACEA has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_180B()
        {
            AssertCode("@@@", "180B");
        }
        // Reko: a decoder for the instruction 1A0B at address ACF4 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1A0B()
        {
            AssertCode("@@@", "1A0B");
        }
        // Reko: a decoder for the instruction 1E0B at address AD08 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1E0B()
        {
            AssertCode("@@@", "1E0B");
        }
        // Reko: a decoder for the instruction 200B at address AD12 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_200B()
        {
            AssertCode("@@@", "200B");
        }
        // Reko: a decoder for the instruction 220B at address AD1C has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_220B()
        {
            AssertCode("@@@", "220B");
        }
        // Reko: a decoder for the instruction 0FD8 at address AD22 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD8()
        {
            AssertCode("@@@", "0FD8");
        }
        // Reko: a decoder for the instruction 240B at address AD26 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_240B()
        {
            AssertCode("@@@", "240B");
        }
        // Reko: a decoder for the instruction 0FD9 at address AD2C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FD9()
        {
            AssertCode("@@@", "0FD9");
        }
        // Reko: a decoder for the instruction 260B at address AD30 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_260B()
        {
            AssertCode("@@@", "260B");
        }
        // Reko: a decoder for the instruction 0FDA at address AD36 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FDA()
        {
            AssertCode("@@@", "0FDA");
        }
        // Reko: a decoder for the instruction 280B at address AD3A has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_280B()
        {
            AssertCode("@@@", "280B");
        }
        // Reko: a decoder for the instruction 0FDB at address AD40 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FDB()
        {
            AssertCode("@@@", "0FDB");
        }
        // Reko: a decoder for the instruction 2A0B at address AD44 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2A0B()
        {
            AssertCode("@@@", "2A0B");
        }
        // Reko: a decoder for the instruction 0FDC at address AD4A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FDC()
        {
            AssertCode("@@@", "0FDC");
        }
        // Reko: a decoder for the instruction 2C0B at address AD4E has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2C0B()
        {
            AssertCode("@@@", "2C0B");
        }
        // Reko: a decoder for the instruction 2E0B at address AD58 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2E0B()
        {
            AssertCode("@@@", "2E0B");
        }
        // Reko: a decoder for the instruction 300B at address AD64 has not been implemented. (Fmt15 1 ZZ  mulsb reg,reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_300B()
        {
            AssertCode("@@@", "300B");
        }
        // Reko: a decoder for the instruction AA61 at address AD8C has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_AA61()
        {
            AssertCode("@@@", "AA61");
        }
        // Reko: a decoder for the instruction FA61 at address AD8E has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_FA61()
        {
            AssertCode("@@@", "FA61");
        }
        // Reko: a decoder for the instruction 2A90 at address AD90 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2A90()
        {
            AssertCode("@@@", "2A90");
        }
        // Reko: a decoder for the instruction 3027 at address AD9E has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3027()
        {
            AssertCode("@@@", "3027");
        }
        // Reko: a decoder for the instruction 4660 at address ADB2 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4660()
        {
            AssertCode("@@@", "4660");
        }
        // Reko: a decoder for the instruction CDF6 at address ADDA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CDF6()
        {
            AssertCode("@@@", "CDF6");
        }
        // Reko: a decoder for the instruction B233 at address ADDE has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_B233()
        {
            AssertCode("@@@", "B233");
        }
        // Reko: a decoder for the instruction 16C2 at address AE02 has not been implemented. (Fmt15 1 ZZ  storw imm(rp) disp0 4 dest(rp) 4 src imm)
        [Test]
        public void Cr16Dasm_16C2()
        {
            AssertCode("@@@", "16C2");
        }
        // Reko: a decoder for the instruction E3F6 at address AE24 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E3F6()
        {
            AssertCode("@@@", "E3F6");
        }
        // Reko: a decoder for the instruction 81F7 at address AE3E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_81F7()
        {
            AssertCode("@@@", "81F7");
        }
        // Reko: a decoder for the instruction 9BFD at address AE46 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9BFD()
        {
            AssertCode("@@@", "9BFD");
        }
        // Reko: a decoder for the instruction C861 at address AE50 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_C861()
        {
            AssertCode("@@@", "C861");
        }
        // Reko: a decoder for the instruction 4291 at address AE78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4291()
        {
            AssertCode("@@@", "4291");
        }
        // Reko: a decoder for the instruction 40D1 at address AE90 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_40D1()
        {
            AssertCode("@@@", "40D1");
        }
        // Reko: a decoder for the instruction 629F at address AEA0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_629F()
        {
            AssertCode("@@@", "629F");
        }
        // Reko: a decoder for the instruction 62DF at address AEA6 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_62DF()
        {
            AssertCode("@@@", "62DF");
        }
        // Reko: a decoder for the instruction 5253 at address AEAE has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_5253()
        {
            AssertCode("@@@", "5253");
        }
        // Reko: a decoder for the instruction 253B at address AEB2 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_253B()
        {
            AssertCode("@@@", "253B");
        }
        // Reko: a decoder for the instruction 2860 at address AED8 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2860()
        {
            AssertCode("@@@", "2860");
        }
        // Reko: a decoder for the instruction C7FB at address AEE6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C7FB()
        {
            AssertCode("@@@", "C7FB");
        }
        // Reko: a decoder for the instruction F3FC at address AEEE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F3FC()
        {
            AssertCode("@@@", "F3FC");
        }
        // Reko: a decoder for the instruction 9A5A at address AEFA has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_9A5A()
        {
            AssertCode("@@@", "9A5A");
        }
        // Reko: a decoder for the instruction 6A04 at address AF7E has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_6A04()
        {
            AssertCode("@@@", "6A04");
        }
        // Reko: a decoder for the instruction C29F at address AFA2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C29F()
        {
            AssertCode("@@@", "C29F");
        }
        // Reko: a decoder for the instruction 22AA at address AFA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_22AA()
        {
            AssertCode("@@@", "22AA");
        }
        // Reko: a decoder for the instruction 1E54 at address AFA8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1E54()
        {
            AssertCode("@@@", "1E54");
        }
        // Reko: a decoder for the instruction 2745 at address AFB4 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_2745()
        {
            AssertCode("@@@", "2745");
        }
        // Reko: a decoder for the instruction 2727 at address AFBA has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2727()
        {
            AssertCode("@@@", "2727");
        }
        // Reko: a decoder for the instruction DC52 at address AFC0 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_DC52()
        {
            AssertCode("@@@", "DC52");
        }
        // Reko: a decoder for the instruction 3632 at address AFC6 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3632()
        {
            AssertCode("@@@", "3632");
        }
        // Reko: a decoder for the instruction E8EA at address AFD4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E8EA()
        {
            AssertCode("@@@", "E8EA");
        }
        // Reko: a decoder for the instruction A8EA at address B036 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A8EA()
        {
            AssertCode("@@@", "A8EA");
        }
        // Reko: a decoder for the instruction 100002591239 at address B076 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100002591239()
        {
            AssertCode("@@@", "100002591239");
        }
        // Reko: a decoder for the instruction 2746 at address B07E has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_2746()
        {
            AssertCode("@@@", "2746");
        }
        // Reko: a decoder for the instruction 100009117052 at address B0A6 has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_100009117052()
        {
            AssertCode("@@@", "100009117052");
        }
        // Reko: a decoder for the instruction 02A4 at address B0B2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02A4()
        {
            AssertCode("@@@", "02A4");
        }
        // Reko: a decoder for the instruction 42AA at address B0B4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42AA()
        {
            AssertCode("@@@", "42AA");
        }
        // Reko: a decoder for the instruction E2AA at address B0F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E2AA()
        {
            AssertCode("@@@", "E2AA");
        }
        // Reko: a decoder for the instruction E661 at address B0F8 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_E661()
        {
            AssertCode("@@@", "E661");
        }
        // Reko: a decoder for the instruction 62EA at address B0FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_62EA()
        {
            AssertCode("@@@", "62EA");
        }
        // Reko: a decoder for the instruction 0545 at address B118 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0545()
        {
            AssertCode("@@@", "0545");
        }
        // Reko: a decoder for the instruction 5127 at address B11E has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5127()
        {
            AssertCode("@@@", "5127");
        }
        // Reko: a decoder for the instruction D052 at address B124 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D052()
        {
            AssertCode("@@@", "D052");
        }
        // Reko: a decoder for the instruction E2A4 at address B128 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E2A4()
        {
            AssertCode("@@@", "E2A4");
        }
        // Reko: a decoder for the instruction 62AA at address B12A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_62AA()
        {
            AssertCode("@@@", "62AA");
        }
        // Reko: a decoder for the instruction 82EA at address B130 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_82EA()
        {
            AssertCode("@@@", "82EA");
        }
        // Reko: a decoder for the instruction 6E61 at address B132 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_6E61()
        {
            AssertCode("@@@", "6E61");
        }
        // Reko: a decoder for the instruction 1EF0 at address B134 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1EF0()
        {
            AssertCode("@@@", "1EF0");
        }
        // Reko: a decoder for the instruction 529F at address B136 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_529F()
        {
            AssertCode("@@@", "529F");
        }
        // Reko: a decoder for the instruction 100005591539 at address B150 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100005591539()
        {
            AssertCode("@@@", "100005591539");
        }
        // Reko: a decoder for the instruction 5446 at address B156 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_5446()
        {
            AssertCode("@@@", "5446");
        }
        // Reko: a decoder for the instruction 42DF at address B15A has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_42DF()
        {
            AssertCode("@@@", "42DF");
        }
        // Reko: a decoder for the instruction 10000A137052 at address B174 has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_10000A137052()
        {
            AssertCode("@@@", "10000A137052");
        }
        // Reko: a decoder for the instruction 42A4 at address B17C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42A4()
        {
            AssertCode("@@@", "42A4");
        }
        // Reko: a decoder for the instruction 14F0 at address B188 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14F0()
        {
            AssertCode("@@@", "14F0");
        }
        // Reko: a decoder for the instruction 3032 at address B1A4 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3032()
        {
            AssertCode("@@@", "3032");
        }
        // Reko: a decoder for the instruction 1000181C42A4 at address B1E6 has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000181C42A4()
        {
            AssertCode("@@@", "1000181C42A4");
        }
        // Reko: a decoder for the instruction 5027 at address B238 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5027()
        {
            AssertCode("@@@", "5027");
        }
        // Reko: a decoder for the instruction 0FF6 at address B23E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FF6()
        {
            AssertCode("@@@", "0FF6");
        }
        // Reko: a decoder for the instruction 5A04 at address B254 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_5A04()
        {
            AssertCode("@@@", "5A04");
        }
        // Reko: a decoder for the instruction 2404 at address B280 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_2404()
        {
            AssertCode("@@@", "2404");
        }
        // Reko: a decoder for the instruction AA60 at address B2A0 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_AA60()
        {
            AssertCode("@@@", "AA60");
        }
        // Reko: a decoder for the instruction AE61 at address B2A4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_AE61()
        {
            AssertCode("@@@", "AE61");
        }
        // Reko: a decoder for the instruction 3B5A at address B2A8 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3B5A()
        {
            AssertCode("@@@", "3B5A");
        }
        // Reko: a decoder for the instruction 1653 at address B2B4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_1653()
        {
            AssertCode("@@@", "1653");
        }
        // Reko: a decoder for the instruction 6C04 at address B2B8 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_6C04()
        {
            AssertCode("@@@", "6C04");
        }
        // Reko: a decoder for the instruction 6204 at address B2C2 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_6204()
        {
            AssertCode("@@@", "6204");
        }
        // Reko: a decoder for the instruction 149F at address B2CA has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_149F()
        {
            AssertCode("@@@", "149F");
        }
        // Reko: a decoder for the instruction 64DF at address B2D0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_64DF()
        {
            AssertCode("@@@", "64DF");
        }
        // Reko: a decoder for the instruction 0E04 at address B2D8 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0E04()
        {
            AssertCode("@@@", "0E04");
        }
        // Reko: a decoder for the instruction 9004 at address B2DE has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_9004()
        {
            AssertCode("@@@", "9004");
        }
        // Reko: a decoder for the instruction B154 at address B2F6 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B154()
        {
            AssertCode("@@@", "B154");
        }
        // Reko: a decoder for the instruction 8161 at address B2FA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8161()
        {
            AssertCode("@@@", "8161");
        }
        // Reko: a decoder for the instruction 1FE6 at address B2FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FE6()
        {
            AssertCode("@@@", "1FE6");
        }
        // Reko: a decoder for the instruction EFA6 at address B314 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EFA6()
        {
            AssertCode("@@@", "EFA6");
        }
        // Reko: a decoder for the instruction 2A4C at address B326 has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_2A4C()
        {
            AssertCode("@@@", "2A4C");
        }
        // Reko: a decoder for the instruction 6153 at address B33A has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6153()
        {
            AssertCode("@@@", "6153");
        }
        // Reko: a decoder for the instruction 6553 at address B344 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_6553()
        {
            AssertCode("@@@", "6553");
        }
        // Reko: a decoder for the instruction 1604 at address B364 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_1604()
        {
            AssertCode("@@@", "1604");
        }
        // Reko: a decoder for the instruction 900A at address B386 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_900A()
        {
            AssertCode("@@@", "900A");
        }
        // Reko: a decoder for the instruction 580A at address B390 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_580A()
        {
            AssertCode("@@@", "580A");
        }
        // Reko: a decoder for the instruction 8C0A at address B39A has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_8C0A()
        {
            AssertCode("@@@", "8C0A");
        }
        // Reko: a decoder for the instruction 5C0A at address B3A4 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_5C0A()
        {
            AssertCode("@@@", "5C0A");
        }
        // Reko: a decoder for the instruction 880A at address B3AE has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_880A()
        {
            AssertCode("@@@", "880A");
        }
        // Reko: a decoder for the instruction 600A at address B3B8 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_600A()
        {
            AssertCode("@@@", "600A");
        }
        // Reko: a decoder for the instruction 840A at address B3C2 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_840A()
        {
            AssertCode("@@@", "840A");
        }
        // Reko: a decoder for the instruction 640A at address B3CC has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_640A()
        {
            AssertCode("@@@", "640A");
        }
        // Reko: a decoder for the instruction 800A at address B3D6 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_800A()
        {
            AssertCode("@@@", "800A");
        }
        // Reko: a decoder for the instruction 680A at address B3E0 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_680A()
        {
            AssertCode("@@@", "680A");
        }
        // Reko: a decoder for the instruction 7C0A at address B3EA has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_7C0A()
        {
            AssertCode("@@@", "7C0A");
        }
        // Reko: a decoder for the instruction 6C0A at address B3F4 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_6C0A()
        {
            AssertCode("@@@", "6C0A");
        }
        // Reko: a decoder for the instruction 780A at address B3FE has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_780A()
        {
            AssertCode("@@@", "780A");
        }
        // Reko: a decoder for the instruction 700A at address B408 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_700A()
        {
            AssertCode("@@@", "700A");
        }
        // Reko: a decoder for the instruction 740A at address B412 has not been implemented. (Fmt15 1 ZZ  Jcondb (rp) 4 dest rp*2 4 cond imm)
        [Test]
        public void Cr16Dasm_740A()
        {
            AssertCode("@@@", "740A");
        }
        // Reko: a decoder for the instruction E260 at address B430 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_E260()
        {
            AssertCode("@@@", "E260");
        }
        // Reko: a decoder for the instruction A260 at address B438 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A260()
        {
            AssertCode("@@@", "A260");
        }
        // Reko: a decoder for the instruction D24B at address B43A has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_D24B()
        {
            AssertCode("@@@", "D24B");
        }
        // Reko: a decoder for the instruction AE60 at address B440 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_AE60()
        {
            AssertCode("@@@", "AE60");
        }
        // Reko: a decoder for the instruction DE4B at address B442 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_DE4B()
        {
            AssertCode("@@@", "DE4B");
        }
        // Reko: a decoder for the instruction 5FB6 at address B456 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FB6()
        {
            AssertCode("@@@", "5FB6");
        }
        // Reko: a decoder for the instruction 0404 at address B47C has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_0404()
        {
            AssertCode("@@@", "0404");
        }
        // Reko: a decoder for the instruction 4132 at address B480 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4132()
        {
            AssertCode("@@@", "4132");
        }
        // Reko: a decoder for the instruction 4A45 at address B484 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4A45()
        {
            AssertCode("@@@", "4A45");
        }
        // Reko: a decoder for the instruction 5A27 at address B486 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5A27()
        {
            AssertCode("@@@", "5A27");
        }
        // Reko: a decoder for the instruction D452 at address B48C has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_D452()
        {
            AssertCode("@@@", "D452");
        }
        // Reko: a decoder for the instruction 48AA at address B494 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48AA()
        {
            AssertCode("@@@", "48AA");
        }
        // Reko: a decoder for the instruction 68EA at address B49A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_68EA()
        {
            AssertCode("@@@", "68EA");
        }
        // Reko: a decoder for the instruction 1000423B2039 at address B4BA has not been implemented. (Fmt3 3 ZZ ope 3  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000423B2039()
        {
            AssertCode("@@@", "1000423B2039");
        }
        // Reko: a decoder for the instruction C89F at address B4D0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_C89F()
        {
            AssertCode("@@@", "C89F");
        }
        // Reko: a decoder for the instruction E89F at address B4D4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E89F()
        {
            AssertCode("@@@", "E89F");
        }
        // Reko: a decoder for the instruction 00FF at address B4DC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_00FF()
        {
            AssertCode("@@@", "00FF");
        }
        // Reko: a decoder for the instruction A027 at address B4E2 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A027()
        {
            AssertCode("@@@", "A027");
        }
        // Reko: a decoder for the instruction 1204 at address B4EE has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_1204()
        {
            AssertCode("@@@", "1204");
        }
        // Reko: a decoder for the instruction 100045590539 at address B518 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100045590539()
        {
            AssertCode("@@@", "100045590539");
        }
        // Reko: a decoder for the instruction 5146 at address B51E has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_5146()
        {
            AssertCode("@@@", "5146");
        }
        // Reko: a decoder for the instruction 4145 at address B530 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4145()
        {
            AssertCode("@@@", "4145");
        }
        // Reko: a decoder for the instruction 48EA at address B55C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_48EA()
        {
            AssertCode("@@@", "48EA");
        }
        // Reko: a decoder for the instruction 100041590139 at address B568 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100041590139()
        {
            AssertCode("@@@", "100041590139");
        }
        // Reko: a decoder for the instruction 1246 at address B570 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1246()
        {
            AssertCode("@@@", "1246");
        }
        // Reko: a decoder for the instruction 1F91 at address B580 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F91()
        {
            AssertCode("@@@", "1F91");
        }
        // Reko: a decoder for the instruction 4245 at address B588 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4245()
        {
            AssertCode("@@@", "4245");
        }
        // Reko: a decoder for the instruction 100043590339 at address B5BE has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100043590339()
        {
            AssertCode("@@@", "100043590339");
        }
        // Reko: a decoder for the instruction 3146 at address B5C4 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_3146()
        {
            AssertCode("@@@", "3146");
        }
        // Reko: a decoder for the instruction 1000CFD4165B at address B5E0 has not been implemented. (Fmt2 3 ZZ ope 13  tbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000CFD4165B()
        {
            AssertCode("@@@", "1000CFD4165B");
        }
        // Reko: a decoder for the instruction 5292 at address B5F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5292()
        {
            AssertCode("@@@", "5292");
        }
        // Reko: a decoder for the instruction 5FD0 at address B5F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FD0()
        {
            AssertCode("@@@", "5FD0");
        }
        // Reko: a decoder for the instruction 1C54 at address B608 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1C54()
        {
            AssertCode("@@@", "1C54");
        }
        // Reko: a decoder for the instruction 4C61 at address B60A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4C61()
        {
            AssertCode("@@@", "4C61");
        }
        // Reko: a decoder for the instruction C8EA at address B60C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C8EA()
        {
            AssertCode("@@@", "C8EA");
        }
        // Reko: a decoder for the instruction C0F0 at address B626 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C0F0()
        {
            AssertCode("@@@", "C0F0");
        }
        // Reko: a decoder for the instruction 0146 at address B632 has not been implemented. (0100 0110 xxxx xxxx  Fmt15 1 ZZ  lshw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_0146()
        {
            AssertCode("@@@", "0146");
        }
        // Reko: a decoder for the instruction 3432 at address B66C has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3432()
        {
            AssertCode("@@@", "3432");
        }
        // Reko: a decoder for the instruction 3F91 at address B674 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F91()
        {
            AssertCode("@@@", "3F91");
        }
        // Reko: a decoder for the instruction CF94 at address B67A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_CF94()
        {
            AssertCode("@@@", "CF94");
        }
        // Reko: a decoder for the instruction 355A at address B6A2 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_355A()
        {
            AssertCode("@@@", "355A");
        }
        // Reko: a decoder for the instruction DFFB at address B6AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFFB()
        {
            AssertCode("@@@", "DFFB");
        }
        // Reko: a decoder for the instruction 140024B00456 at address B6CA has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140024B00456()
        {
            AssertCode("@@@", "140024B00456");
        }
        // Reko: a decoder for the instruction 0EDF at address B6DC has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0EDF()
        {
            AssertCode("@@@", "0EDF");
        }
        // Reko: a decoder for the instruction 77FB at address B6E2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_77FB()
        {
            AssertCode("@@@", "77FB");
        }
        // Reko: a decoder for the instruction FDFB at address B6EE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_FDFB()
        {
            AssertCode("@@@", "FDFB");
        }
        // Reko: a decoder for the instruction 77FC at address B6FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_77FC()
        {
            AssertCode("@@@", "77FC");
        }
        // Reko: a decoder for the instruction 0453 at address B700 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_0453()
        {
            AssertCode("@@@", "0453");
        }
        // Reko: a decoder for the instruction 14DF at address B710 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_14DF()
        {
            AssertCode("@@@", "14DF");
        }
        // Reko: a decoder for the instruction 3DFC at address B720 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DFC()
        {
            AssertCode("@@@", "3DFC");
        }
        // Reko: a decoder for the instruction 91FB at address B746 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_91FB()
        {
            AssertCode("@@@", "91FB");
        }
        // Reko: a decoder for the instruction A652 at address B748 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_A652()
        {
            AssertCode("@@@", "A652");
        }
        // Reko: a decoder for the instruction F3FB at address B76A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_F3FB()
        {
            AssertCode("@@@", "F3FB");
        }
        // Reko: a decoder for the instruction 615A at address B770 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_615A()
        {
            AssertCode("@@@", "615A");
        }
        // Reko: a decoder for the instruction 75FB at address B776 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_75FB()
        {
            AssertCode("@@@", "75FB");
        }
        // Reko: a decoder for the instruction EFFB at address B782 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EFFB()
        {
            AssertCode("@@@", "EFFB");
        }
        // Reko: a decoder for the instruction 47FB at address B790 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_47FB()
        {
            AssertCode("@@@", "47FB");
        }
        // Reko: a decoder for the instruction BFFB at address B79E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_BFFB()
        {
            AssertCode("@@@", "BFFB");
        }
        // Reko: a decoder for the instruction 5054 at address B7A0 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5054()
        {
            AssertCode("@@@", "5054");
        }
        // Reko: a decoder for the instruction EFA4 at address B7A4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_EFA4()
        {
            AssertCode("@@@", "EFA4");
        }
        // Reko: a decoder for the instruction 0FB6 at address B7AE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FB6()
        {
            AssertCode("@@@", "0FB6");
        }
        // Reko: a decoder for the instruction 2132 at address B880 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2132()
        {
            AssertCode("@@@", "2132");
        }
        // Reko: a decoder for the instruction 100003592339 at address B8B8 has not been implemented. (Fmt2 3 ZZ ope 5  cbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100003592339()
        {
            AssertCode("@@@", "100003592339");
        }
        // Reko: a decoder for the instruction 4432 at address B8EA has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4432()
        {
            AssertCode("@@@", "4432");
        }
        // Reko: a decoder for the instruction E5FC at address B8F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E5FC()
        {
            AssertCode("@@@", "E5FC");
        }
        // Reko: a decoder for the instruction 5432 at address B8F4 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5432()
        {
            AssertCode("@@@", "5432");
        }
        // Reko: a decoder for the instruction 87FC at address B8FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_87FC()
        {
            AssertCode("@@@", "87FC");
        }
        // Reko: a decoder for the instruction 2BFC at address B906 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2BFC()
        {
            AssertCode("@@@", "2BFC");
        }
        // Reko: a decoder for the instruction C3FB at address B910 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C3FB()
        {
            AssertCode("@@@", "C3FB");
        }
        // Reko: a decoder for the instruction C3FD at address B91C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C3FD()
        {
            AssertCode("@@@", "C3FD");
        }
        // Reko: a decoder for the instruction 4D60 at address B93A has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4D60()
        {
            AssertCode("@@@", "4D60");
        }
        // Reko: a decoder for the instruction 9FFD at address B940 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FFD()
        {
            AssertCode("@@@", "9FFD");
        }
        // Reko: a decoder for the instruction 12000FD1035B at address B956 has not been implemented. (Fmt2 3 ZZ ope 13  loadw (rp) disp20 4 src (rp) 4 dest reg 20 src disp 4)
        [Test]
        public void Cr16Dasm_12000FD1035B()
        {
            AssertCode("@@@", "12000FD1035B");
        }
        // Reko: a decoder for the instruction C7FA at address B968 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_C7FA()
        {
            AssertCode("@@@", "C7FA");
        }
        // Reko: a decoder for the instruction 3FC3 at address B96A has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_3FC3()
        {
            AssertCode("@@@", "3FC3");
        }
        // Reko: a decoder for the instruction EFC3 at address B972 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_EFC3()
        {
            AssertCode("@@@", "EFC3");
        }
        // Reko: a decoder for the instruction 10002FD1235B at address B97C has not been implemented. (Fmt2 3 ZZ ope 13  tbitb (rp) disp20 4 dest (rp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_10002FD1235B()
        {
            AssertCode("@@@", "10002FD1235B");
        }
        // Reko: a decoder for the instruction FFC3 at address B984 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_FFC3()
        {
            AssertCode("@@@", "FFC3");
        }
        // Reko: a decoder for the instruction 11001FD1135B at address B98E has not been implemented. (Fmt2 3 ZZ ope 13  tbitw(rp) disp20 4 dest(rp) 4 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_11001FD1135B()
        {
            AssertCode("@@@", "11001FD1135B");
        }
        // Reko: a decoder for the instruction AFC3 at address B996 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_AFC3()
        {
            AssertCode("@@@", "AFC3");
        }
        // Reko: a decoder for the instruction CFC3 at address B99E has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_CFC3()
        {
            AssertCode("@@@", "CFC3");
        }
        // Reko: a decoder for the instruction BFC3 at address B9A6 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_BFC3()
        {
            AssertCode("@@@", "BFC3");
        }
        // Reko: a decoder for the instruction DFC3 at address B9AE has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_DFC3()
        {
            AssertCode("@@@", "DFC3");
        }
        // Reko: a decoder for the instruction 8FC3 at address B9B6 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_8FC3()
        {
            AssertCode("@@@", "8FC3");
        }
        // Reko: a decoder for the instruction 7FC3 at address B9BE has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_7FC3()
        {
            AssertCode("@@@", "7FC3");
        }
        // Reko: a decoder for the instruction 4FC3 at address B9CE has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_4FC3()
        {
            AssertCode("@@@", "4FC3");
        }
        // Reko: a decoder for the instruction 6FC3 at address B9D6 has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_6FC3()
        {
            AssertCode("@@@", "6FC3");
        }
        // Reko: a decoder for the instruction 5FC3 at address B9DE has not been implemented. (Fmt16 2 ZZ  storw imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_5FC3()
        {
            AssertCode("@@@", "5FC3");
        }
        // Reko: a decoder for the instruction 8E61 at address B9FA has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8E61()
        {
            AssertCode("@@@", "8E61");
        }
        // Reko: a decoder for the instruction 4ED0 at address B9FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4ED0()
        {
            AssertCode("@@@", "4ED0");
        }
        // Reko: a decoder for the instruction 5EF0 at address BA0A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5EF0()
        {
            AssertCode("@@@", "5EF0");
        }
        // Reko: a decoder for the instruction 50DF at address BA34 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_50DF()
        {
            AssertCode("@@@", "50DF");
        }
        // Reko: a decoder for the instruction 203B at address BAB4 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_203B()
        {
            AssertCode("@@@", "203B");
        }
        // Reko: a decoder for the instruction 80A0 at address BAF0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_80A0()
        {
            AssertCode("@@@", "80A0");
        }
        // Reko: a decoder for the instruction 8FEF at address BAF2 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8FEF()
        {
            AssertCode("@@@", "8FEF");
        }
        // Reko: a decoder for the instruction 0D4B at address BAF8 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_0D4B()
        {
            AssertCode("@@@", "0D4B");
        }
        // Reko: a decoder for the instruction 140028B08FE0 at address BB02 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140028B08FE0()
        {
            AssertCode("@@@", "140028B08FE0");
        }
        // Reko: a decoder for the instruction DFAF at address BB08 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DFAF()
        {
            AssertCode("@@@", "DFAF");
        }
        // Reko: a decoder for the instruction BD60 at address BB2C has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BD60()
        {
            AssertCode("@@@", "BD60");
        }
        // Reko: a decoder for the instruction 50EA at address BB2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_50EA()
        {
            AssertCode("@@@", "50EA");
        }
        // Reko: a decoder for the instruction 0CB1 at address BB4A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB1()
        {
            AssertCode("@@@", "0CB1");
        }
        // Reko: a decoder for the instruction 2CB2 at address BB56 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB2()
        {
            AssertCode("@@@", "2CB2");
        }
        // Reko: a decoder for the instruction 4FE2 at address BB62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FE2()
        {
            AssertCode("@@@", "4FE2");
        }
        // Reko: a decoder for the instruction 2CB3 at address BB64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB3()
        {
            AssertCode("@@@", "2CB3");
        }
        // Reko: a decoder for the instruction 2CB4 at address BB6E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB4()
        {
            AssertCode("@@@", "2CB4");
        }
        // Reko: a decoder for the instruction 4FE6 at address BB76 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FE6()
        {
            AssertCode("@@@", "4FE6");
        }
        // Reko: a decoder for the instruction 2CB5 at address BB78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB5()
        {
            AssertCode("@@@", "2CB5");
        }
        // Reko: a decoder for the instruction 2CB6 at address BB82 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB6()
        {
            AssertCode("@@@", "2CB6");
        }
        // Reko: a decoder for the instruction 4FEA at address BB8A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FEA()
        {
            AssertCode("@@@", "4FEA");
        }
        // Reko: a decoder for the instruction 2CB7 at address BB8C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB7()
        {
            AssertCode("@@@", "2CB7");
        }
        // Reko: a decoder for the instruction 4FEC at address BB94 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4FEC()
        {
            AssertCode("@@@", "4FEC");
        }
        // Reko: a decoder for the instruction 2CB8 at address BB96 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2CB8()
        {
            AssertCode("@@@", "2CB8");
        }
        // Reko: a decoder for the instruction DCB9 at address BBA2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DCB9()
        {
            AssertCode("@@@", "DCB9");
        }
        // Reko: a decoder for the instruction ECBA at address BBAE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ECBA()
        {
            AssertCode("@@@", "ECBA");
        }
        // Reko: a decoder for the instruction ACBB at address BBB6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACBB()
        {
            AssertCode("@@@", "ACBB");
        }
        // Reko: a decoder for the instruction 6CBC at address BBBE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6CBC()
        {
            AssertCode("@@@", "6CBC");
        }
        // Reko: a decoder for the instruction A661 at address BBC4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_A661()
        {
            AssertCode("@@@", "A661");
        }
        // Reko: a decoder for the instruction 4CBD at address BBC6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CBD()
        {
            AssertCode("@@@", "4CBD");
        }
        // Reko: a decoder for the instruction 2CBF at address BBCE has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_2CBF()
        {
            AssertCode("@@@", "2CBF");
        }
        // Reko: a decoder for the instruction 8CBF at address BBD8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_8CBF()
        {
            AssertCode("@@@", "8CBF");
        }
        // Reko: a decoder for the instruction DFA0 at address BBE6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFA0()
        {
            AssertCode("@@@", "DFA0");
        }
        // Reko: a decoder for the instruction DFA2 at address BBEA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFA2()
        {
            AssertCode("@@@", "DFA2");
        }
        // Reko: a decoder for the instruction DFA6 at address BBF2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFA6()
        {
            AssertCode("@@@", "DFA6");
        }
        // Reko: a decoder for the instruction DFA8 at address BBF6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFA8()
        {
            AssertCode("@@@", "DFA8");
        }
        // Reko: a decoder for the instruction DFAA at address BBFA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFAA()
        {
            AssertCode("@@@", "DFAA");
        }
        // Reko: a decoder for the instruction DFAC at address BBFE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_DFAC()
        {
            AssertCode("@@@", "DFAC");
        }
        // Reko: a decoder for the instruction 0D61 at address BC20 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_0D61()
        {
            AssertCode("@@@", "0D61");
        }
        // Reko: a decoder for the instruction BC60 at address BC26 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BC60()
        {
            AssertCode("@@@", "BC60");
        }
        // Reko: a decoder for the instruction 10000FAF2400 at address BC28 has not been implemented. (Fmt2 3 ZZ ope 10  sbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_10000FAF2400()
        {
            AssertCode("@@@", "10000FAF2400");
        }
        // Reko: a decoder for the instruction 5E04 at address BC72 has not been implemented. (Fmt12 2 ZZ  addd imm20, rp 20 src imm 4 dest rp)
        [Test]
        public void Cr16Dasm_5E04()
        {
            AssertCode("@@@", "5E04");
        }
        // Reko: a decoder for the instruction 14002090BF60 at address BC8A has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14002090BF60()
        {
            AssertCode("@@@", "14002090BF60");
        }
        // Reko: a decoder for the instruction 8FA0 at address BCA2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FA0()
        {
            AssertCode("@@@", "8FA0");
        }
        // Reko: a decoder for the instruction 8FE0 at address BCA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FE0()
        {
            AssertCode("@@@", "8FE0");
        }
        // Reko: a decoder for the instruction 04B4 at address BCFC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B4()
        {
            AssertCode("@@@", "04B4");
        }
        // Reko: a decoder for the instruction 04B5 at address BD10 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B5()
        {
            AssertCode("@@@", "04B5");
        }
        // Reko: a decoder for the instruction 04B6 at address BD24 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B6()
        {
            AssertCode("@@@", "04B6");
        }
        // Reko: a decoder for the instruction 04B7 at address BD38 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B7()
        {
            AssertCode("@@@", "04B7");
        }
        // Reko: a decoder for the instruction 04B8 at address BD4C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B8()
        {
            AssertCode("@@@", "04B8");
        }
        // Reko: a decoder for the instruction 04B9 at address BD62 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04B9()
        {
            AssertCode("@@@", "04B9");
        }
        // Reko: a decoder for the instruction 04BA at address BD76 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04BA()
        {
            AssertCode("@@@", "04BA");
        }
        // Reko: a decoder for the instruction 04BB at address BD8C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04BB()
        {
            AssertCode("@@@", "04BB");
        }
        // Reko: a decoder for the instruction 04BC at address BDA0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04BC()
        {
            AssertCode("@@@", "04BC");
        }
        // Reko: a decoder for the instruction 04BD at address BDB4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04BD()
        {
            AssertCode("@@@", "04BD");
        }
        // Reko: a decoder for the instruction 04BF at address BDC8 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_04BF()
        {
            AssertCode("@@@", "04BF");
        }
        // Reko: a decoder for the instruction 14008090BF60 at address BDFE has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14008090BF60()
        {
            AssertCode("@@@", "14008090BF60");
        }
        // Reko: a decoder for the instruction C24B at address BE22 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_C24B()
        {
            AssertCode("@@@", "C24B");
        }
        // Reko: a decoder for the instruction 404C at address BE2C has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_404C()
        {
            AssertCode("@@@", "404C");
        }
        // Reko: a decoder for the instruction 6CB1 at address BE44 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6CB1()
        {
            AssertCode("@@@", "6CB1");
        }
        // Reko: a decoder for the instruction 4CB2 at address BE4C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4CB2()
        {
            AssertCode("@@@", "4CB2");
        }
        // Reko: a decoder for the instruction ACB4 at address BE5C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ACB4()
        {
            AssertCode("@@@", "ACB4");
        }
        // Reko: a decoder for the instruction 8CB5 at address BE64 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CB5()
        {
            AssertCode("@@@", "8CB5");
        }
        // Reko: a decoder for the instruction 8CB6 at address BE6C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CB6()
        {
            AssertCode("@@@", "8CB6");
        }
        // Reko: a decoder for the instruction 8CB7 at address BE74 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CB7()
        {
            AssertCode("@@@", "8CB7");
        }
        // Reko: a decoder for the instruction D861 at address BE7C has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D861()
        {
            AssertCode("@@@", "D861");
        }
        // Reko: a decoder for the instruction 8CB8 at address BE80 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CB8()
        {
            AssertCode("@@@", "8CB8");
        }
        // Reko: a decoder for the instruction 8CB9 at address BE8C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CB9()
        {
            AssertCode("@@@", "8CB9");
        }
        // Reko: a decoder for the instruction 8FE6 at address BE96 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FE6()
        {
            AssertCode("@@@", "8FE6");
        }
        // Reko: a decoder for the instruction 8CBA at address BE98 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CBA()
        {
            AssertCode("@@@", "8CBA");
        }
        // Reko: a decoder for the instruction 8FE8 at address BEA2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FE8()
        {
            AssertCode("@@@", "8FE8");
        }
        // Reko: a decoder for the instruction 8CBB at address BEA4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CBB()
        {
            AssertCode("@@@", "8CBB");
        }
        // Reko: a decoder for the instruction 8FA8 at address BEAA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FA8()
        {
            AssertCode("@@@", "8FA8");
        }
        // Reko: a decoder for the instruction 8FEA at address BEAE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FEA()
        {
            AssertCode("@@@", "8FEA");
        }
        // Reko: a decoder for the instruction 8CBC at address BEB0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CBC()
        {
            AssertCode("@@@", "8CBC");
        }
        // Reko: a decoder for the instruction 8FAA at address BEB6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FAA()
        {
            AssertCode("@@@", "8FAA");
        }
        // Reko: a decoder for the instruction 8FEC at address BEBA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FEC()
        {
            AssertCode("@@@", "8FEC");
        }
        // Reko: a decoder for the instruction 8CBD at address BEBC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8CBD()
        {
            AssertCode("@@@", "8CBD");
        }
        // Reko: a decoder for the instruction 8FAC at address BEC2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8FAC()
        {
            AssertCode("@@@", "8FAC");
        }
        // Reko: a decoder for the instruction 3FA6 at address BF04 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FA6()
        {
            AssertCode("@@@", "3FA6");
        }
        // Reko: a decoder for the instruction 3061 at address BF06 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_3061()
        {
            AssertCode("@@@", "3061");
        }
        // Reko: a decoder for the instruction 1FA0 at address BF20 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FA0()
        {
            AssertCode("@@@", "1FA0");
        }
        // Reko: a decoder for the instruction 1861 at address BF22 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_1861()
        {
            AssertCode("@@@", "1861");
        }
        // Reko: a decoder for the instruction 8D61 at address BF28 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_8D61()
        {
            AssertCode("@@@", "8D61");
        }
        // Reko: a decoder for the instruction 10000FAF2800 at address BF30 has not been implemented. (Fmt2 3 ZZ ope 10  sbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_10000FAF2800()
        {
            AssertCode("@@@", "10000FAF2800");
        }
        // Reko: a decoder for the instruction 0CB2 at address BF92 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB2()
        {
            AssertCode("@@@", "0CB2");
        }
        // Reko: a decoder for the instruction 0CB3 at address BFA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB3()
        {
            AssertCode("@@@", "0CB3");
        }
        // Reko: a decoder for the instruction 0CB4 at address BFBA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB4()
        {
            AssertCode("@@@", "0CB4");
        }
        // Reko: a decoder for the instruction 0CB5 at address BFCE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB5()
        {
            AssertCode("@@@", "0CB5");
        }
        // Reko: a decoder for the instruction 0CB6 at address BFE2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB6()
        {
            AssertCode("@@@", "0CB6");
        }
        // Reko: a decoder for the instruction 0CB7 at address BFF6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB7()
        {
            AssertCode("@@@", "0CB7");
        }
        // Reko: a decoder for the instruction 0CB8 at address C00A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB8()
        {
            AssertCode("@@@", "0CB8");
        }
        // Reko: a decoder for the instruction 0CB9 at address C01E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CB9()
        {
            AssertCode("@@@", "0CB9");
        }
        // Reko: a decoder for the instruction 0CBA at address C034 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CBA()
        {
            AssertCode("@@@", "0CBA");
        }
        // Reko: a decoder for the instruction 0CBB at address C048 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CBB()
        {
            AssertCode("@@@", "0CBB");
        }
        // Reko: a decoder for the instruction 0CBC at address C05E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CBC()
        {
            AssertCode("@@@", "0CBC");
        }
        // Reko: a decoder for the instruction 0CBD at address C072 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CBD()
        {
            AssertCode("@@@", "0CBD");
        }
        // Reko: a decoder for the instruction 0CBF at address C086 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_0CBF()
        {
            AssertCode("@@@", "0CBF");
        }
        // Reko: a decoder for the instruction A1FB at address C0D6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A1FB()
        {
            AssertCode("@@@", "A1FB");
        }
        // Reko: a decoder for the instruction 3DFD at address C0DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DFD()
        {
            AssertCode("@@@", "3DFD");
        }
        // Reko: a decoder for the instruction 77FE at address C0EA has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_77FE()
        {
            AssertCode("@@@", "77FE");
        }
        // Reko: a decoder for the instruction 14002CB06D5E at address C112 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14002CB06D5E()
        {
            AssertCode("@@@", "14002CB06D5E");
        }
        // Reko: a decoder for the instruction 1400A0B00C61 at address C126 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A0B00C61()
        {
            AssertCode("@@@", "1400A0B00C61");
        }
        // Reko: a decoder for the instruction 084B at address C12C has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_084B()
        {
            AssertCode("@@@", "084B");
        }
        // Reko: a decoder for the instruction 024D at address C17C has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_024D()
        {
            AssertCode("@@@", "024D");
        }
        // Reko: a decoder for the instruction 140020905802 at address C17E has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020905802()
        {
            AssertCode("@@@", "140020905802");
        }
        // Reko: a decoder for the instruction 140042B00256 at address C23E has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140042B00256()
        {
            AssertCode("@@@", "140042B00256");
        }
        // Reko: a decoder for the instruction 1400D2B00256 at address C250 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400D2B00256()
        {
            AssertCode("@@@", "1400D2B00256");
        }
        // Reko: a decoder for the instruction 4D61 at address C25A has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4D61()
        {
            AssertCode("@@@", "4D61");
        }
        // Reko: a decoder for the instruction 24B0 at address C25C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_24B0()
        {
            AssertCode("@@@", "24B0");
        }
        // Reko: a decoder for the instruction 140002A04200 at address C262 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A04200()
        {
            AssertCode("@@@", "140002A04200");
        }
        // Reko: a decoder for the instruction 22A0 at address C270 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_22A0()
        {
            AssertCode("@@@", "22A0");
        }
        // Reko: a decoder for the instruction 140020A09C60 at address C274 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A09C60()
        {
            AssertCode("@@@", "140020A09C60");
        }
        // Reko: a decoder for the instruction E0FF at address C296 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_E0FF()
        {
            AssertCode("@@@", "E0FF");
        }
        // Reko: a decoder for the instruction 140020A00255 at address C2AC has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A00255()
        {
            AssertCode("@@@", "140020A00255");
        }
        // Reko: a decoder for the instruction 62A0 at address C2C2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_62A0()
        {
            AssertCode("@@@", "62A0");
        }
        // Reko: a decoder for the instruction 140002B0B260 at address C2C8 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002B0B260()
        {
            AssertCode("@@@", "140002B0B260");
        }
        // Reko: a decoder for the instruction 844A at address C2D6 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_844A()
        {
            AssertCode("@@@", "844A");
        }
        // Reko: a decoder for the instruction 44A0 at address C2DE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_44A0()
        {
            AssertCode("@@@", "44A0");
        }
        // Reko: a decoder for the instruction 140024A02AA2 at address C2E0 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140024A02AA2()
        {
            AssertCode("@@@", "140024A02AA2");
        }
        // Reko: a decoder for the instruction 140042A0804B at address C2E6 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140042A0804B()
        {
            AssertCode("@@@", "140042A0804B");
        }
        // Reko: a decoder for the instruction 140042A01400 at address C2FC has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140042A01400()
        {
            AssertCode("@@@", "140042A01400");
        }
        // Reko: a decoder for the instruction 26A0 at address C302 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_26A0()
        {
            AssertCode("@@@", "26A0");
        }
        // Reko: a decoder for the instruction 140060B0B060 at address C31C has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140060B0B060()
        {
            AssertCode("@@@", "140060B0B060");
        }
        // Reko: a decoder for the instruction 140002A00AA4 at address C334 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A00AA4()
        {
            AssertCode("@@@", "140002A00AA4");
        }
        // Reko: a decoder for the instruction 140020A0864B at address C33A has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A0864B()
        {
            AssertCode("@@@", "140020A0864B");
        }
        // Reko: a decoder for the instruction 140020A01400 at address C350 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A01400()
        {
            AssertCode("@@@", "140020A01400");
        }
        // Reko: a decoder for the instruction 60A0 at address C36A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_60A0()
        {
            AssertCode("@@@", "60A0");
        }
        // Reko: a decoder for the instruction 140080B0B060 at address C370 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140080B0B060()
        {
            AssertCode("@@@", "140080B0B060");
        }
        // Reko: a decoder for the instruction 42A0 at address C386 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_42A0()
        {
            AssertCode("@@@", "42A0");
        }
        // Reko: a decoder for the instruction 140040A04AA6 at address C388 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140040A04AA6()
        {
            AssertCode("@@@", "140040A04AA6");
        }
        // Reko: a decoder for the instruction 140004A0884B at address C38E has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140004A0884B()
        {
            AssertCode("@@@", "140004A0884B");
        }
        // Reko: a decoder for the instruction B860 at address C39A has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B860()
        {
            AssertCode("@@@", "B860");
        }
        // Reko: a decoder for the instruction 284C at address C39E has not been implemented. (0100 110x xxxx xxxx  Fmt20 1 ZZ  ashud cnt(left +), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_284C()
        {
            AssertCode("@@@", "284C");
        }
        // Reko: a decoder for the instruction E861 at address C3A0 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_E861()
        {
            AssertCode("@@@", "E861");
        }
        // Reko: a decoder for the instruction 140004A01400 at address C3A4 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140004A01400()
        {
            AssertCode("@@@", "140004A01400");
        }
        // Reko: a decoder for the instruction 46A0 at address C3AA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_46A0()
        {
            AssertCode("@@@", "46A0");
        }
        // Reko: a decoder for the instruction 140020A02AA8 at address C3DC has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A02AA8()
        {
            AssertCode("@@@", "140020A02AA8");
        }
        // Reko: a decoder for the instruction 140002A0864B at address C3E2 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A0864B()
        {
            AssertCode("@@@", "140002A0864B");
        }
        // Reko: a decoder for the instruction 06A0 at address C3F6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_06A0()
        {
            AssertCode("@@@", "06A0");
        }
        // Reko: a decoder for the instruction 140002A01400 at address C3F8 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A01400()
        {
            AssertCode("@@@", "140002A01400");
        }
        // Reko: a decoder for the instruction 28A0 at address C3FE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_28A0()
        {
            AssertCode("@@@", "28A0");
        }
        // Reko: a decoder for the instruction 140040A04AAA at address C430 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140040A04AAA()
        {
            AssertCode("@@@", "140040A04AAA");
        }
        // Reko: a decoder for the instruction 140020A02AAC at address C484 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A02AAC()
        {
            AssertCode("@@@", "140020A02AAC");
        }
        // Reko: a decoder for the instruction 140020A02AAF at address C4D8 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A02AAF()
        {
            AssertCode("@@@", "140020A02AAF");
        }
        // Reko: a decoder for the instruction 140002A0884B at address C4E0 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A0884B()
        {
            AssertCode("@@@", "140002A0884B");
        }
        // Reko: a decoder for the instruction 140020B0B060 at address C516 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020B0B060()
        {
            AssertCode("@@@", "140020B0B060");
        }
        // Reko: a decoder for the instruction 140040A0824B at address C52E has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140040A0824B()
        {
            AssertCode("@@@", "140040A0824B");
        }
        // Reko: a decoder for the instruction 140040A01400 at address C544 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140040A01400()
        {
            AssertCode("@@@", "140040A01400");
        }
        // Reko: a decoder for the instruction 59FD at address C554 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_59FD()
        {
            AssertCode("@@@", "59FD");
        }
        // Reko: a decoder for the instruction 140020A0A005 at address C564 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A0A005()
        {
            AssertCode("@@@", "140020A0A005");
        }
        // Reko: a decoder for the instruction 78FC at address C56A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_78FC()
        {
            AssertCode("@@@", "78FC");
        }
        // Reko: a decoder for the instruction 82A0 at address C57E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_82A0()
        {
            AssertCode("@@@", "82A0");
        }
        // Reko: a decoder for the instruction 140004B0B460 at address C58C has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140004B0B460()
        {
            AssertCode("@@@", "140004B0B460");
        }
        // Reko: a decoder for the instruction 140062A0804B at address C59C has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140062A0804B()
        {
            AssertCode("@@@", "140062A0804B");
        }
        // Reko: a decoder for the instruction BE54 at address C5BA has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_BE54()
        {
            AssertCode("@@@", "BE54");
        }
        // Reko: a decoder for the instruction CE61 at address C5BE has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_CE61()
        {
            AssertCode("@@@", "CE61");
        }
        // Reko: a decoder for the instruction 2DA2 at address C5C6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DA2()
        {
            AssertCode("@@@", "2DA2");
        }
        // Reko: a decoder for the instruction 6DA6 at address C686 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DA6()
        {
            AssertCode("@@@", "6DA6");
        }
        // Reko: a decoder for the instruction 140006A06055 at address C688 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140006A06055()
        {
            AssertCode("@@@", "140006A06055");
        }
        // Reko: a decoder for the instruction 140064B0B460 at address C6AC has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140064B0B460()
        {
            AssertCode("@@@", "140064B0B460");
        }
        // Reko: a decoder for the instruction 140002A06055 at address C6BC has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140002A06055()
        {
            AssertCode("@@@", "140002A06055");
        }
        // Reko: a decoder for the instruction 6DA8 at address C6E8 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DA8()
        {
            AssertCode("@@@", "6DA8");
        }
        // Reko: a decoder for the instruction ECFF at address C740 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ECFF()
        {
            AssertCode("@@@", "ECFF");
        }
        // Reko: a decoder for the instruction 0AA0 at address C7F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0AA0()
        {
            AssertCode("@@@", "0AA0");
        }
        // Reko: a decoder for the instruction 71FA at address C810 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_71FA()
        {
            AssertCode("@@@", "71FA");
        }
        // Reko: a decoder for the instruction 2DB0 at address C816 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DB0()
        {
            AssertCode("@@@", "2DB0");
        }
        // Reko: a decoder for the instruction 140080A01C56 at address C82E has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140080A01C56()
        {
            AssertCode("@@@", "140080A01C56");
        }
        // Reko: a decoder for the instruction 2DB1 at address C838 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DB1()
        {
            AssertCode("@@@", "2DB1");
        }
        // Reko: a decoder for the instruction 140080A02C56 at address C850 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140080A02C56()
        {
            AssertCode("@@@", "140080A02C56");
        }
        // Reko: a decoder for the instruction 29FA at address C858 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_29FA()
        {
            AssertCode("@@@", "29FA");
        }
        // Reko: a decoder for the instruction 2DB2 at address C85A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DB2()
        {
            AssertCode("@@@", "2DB2");
        }
        // Reko: a decoder for the instruction 140020A06000 at address C872 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140020A06000()
        {
            AssertCode("@@@", "140020A06000");
        }
        // Reko: a decoder for the instruction F0FE at address C888 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_F0FE()
        {
            AssertCode("@@@", "F0FE");
        }
        // Reko: a decoder for the instruction FFDF at address C8A4 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_FFDF()
        {
            AssertCode("@@@", "FFDF");
        }
        // Reko: a decoder for the instruction 02E2 at address C8B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02E2()
        {
            AssertCode("@@@", "02E2");
        }
        // Reko: a decoder for the instruction FD61 at address C8C4 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_FD61()
        {
            AssertCode("@@@", "FD61");
        }
        // Reko: a decoder for the instruction 140004B00456 at address C8EA has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140004B00456()
        {
            AssertCode("@@@", "140004B00456");
        }
        // Reko: a decoder for the instruction 140046A0F04B at address C8F6 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140046A0F04B()
        {
            AssertCode("@@@", "140046A0F04B");
        }
        // Reko: a decoder for the instruction 68E0 at address C902 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_68E0()
        {
            AssertCode("@@@", "68E0");
        }
        // Reko: a decoder for the instruction 4860 at address C906 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4860()
        {
            AssertCode("@@@", "4860");
        }
        // Reko: a decoder for the instruction 4C60 at address C948 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4C60()
        {
            AssertCode("@@@", "4C60");
        }
        // Reko: a decoder for the instruction 6AE0 at address C97E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6AE0()
        {
            AssertCode("@@@", "6AE0");
        }
        // Reko: a decoder for the instruction F143 at address C99A has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_F143()
        {
            AssertCode("@@@", "F143");
        }
        // Reko: a decoder for the instruction E043 at address C9EA has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_E043()
        {
            AssertCode("@@@", "E043");
        }
        // Reko: a decoder for the instruction 1400E0A0BF60 at address C9F8 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400E0A0BF60()
        {
            AssertCode("@@@", "1400E0A0BF60");
        }
        // Reko: a decoder for the instruction F04B at address CA04 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F04B()
        {
            AssertCode("@@@", "F04B");
        }
        // Reko: a decoder for the instruction 9BFE at address CA44 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_9BFE()
        {
            AssertCode("@@@", "9BFE");
        }
        // Reko: a decoder for the instruction F1FE at address CA62 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_F1FE()
        {
            AssertCode("@@@", "F1FE");
        }
        // Reko: a decoder for the instruction 1400E2B00256 at address CA6E has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400E2B00256()
        {
            AssertCode("@@@", "1400E2B00256");
        }
        // Reko: a decoder for the instruction 140024A0FE4B at address CA78 has not been implemented. (Fmt1 2 ZZ ope 10  xord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_140024A0FE4B()
        {
            AssertCode("@@@", "140024A0FE4B");
        }
        // Reko: a decoder for the instruction FE4B at address CA88 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_FE4B()
        {
            AssertCode("@@@", "FE4B");
        }
        // Reko: a decoder for the instruction 8C54 at address CAD8 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8C54()
        {
            AssertCode("@@@", "8C54");
        }
        // Reko: a decoder for the instruction A8FF at address CB2E has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_A8FF()
        {
            AssertCode("@@@", "A8FF");
        }
        // Reko: a decoder for the instruction 02A0 at address CB36 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02A0()
        {
            AssertCode("@@@", "02A0");
        }
        // Reko: a decoder for the instruction 0292 at address CB3A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0292()
        {
            AssertCode("@@@", "0292");
        }
        // Reko: a decoder for the instruction 2FE8 at address CB46 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2FE8()
        {
            AssertCode("@@@", "2FE8");
        }
        // Reko: a decoder for the instruction D8A6 at address CB48 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D8A6()
        {
            AssertCode("@@@", "D8A6");
        }
        // Reko: a decoder for the instruction 3FD5 at address CB6A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FD5()
        {
            AssertCode("@@@", "3FD5");
        }
        // Reko: a decoder for the instruction 5FD4 at address CB78 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5FD4()
        {
            AssertCode("@@@", "5FD4");
        }
        // Reko: a decoder for the instruction 6FE6 at address CB7E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FE6()
        {
            AssertCode("@@@", "6FE6");
        }
        // Reko: a decoder for the instruction 1245 at address CB9A has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1245()
        {
            AssertCode("@@@", "1245");
        }
        // Reko: a decoder for the instruction 3FEC at address CBAA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FEC()
        {
            AssertCode("@@@", "3FEC");
        }
        // Reko: a decoder for the instruction B622 at address CBB6 has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B622()
        {
            AssertCode("@@@", "B622");
        }
        // Reko: a decoder for the instruction 6FA6 at address CBC4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FA6()
        {
            AssertCode("@@@", "6FA6");
        }
        // Reko: a decoder for the instruction 3F95 at address CBD2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3F95()
        {
            AssertCode("@@@", "3F95");
        }
        // Reko: a decoder for the instruction 4333 at address CBD6 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4333()
        {
            AssertCode("@@@", "4333");
        }
        // Reko: a decoder for the instruction 7FA0 at address CBFC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FA0()
        {
            AssertCode("@@@", "7FA0");
        }
        // Reko: a decoder for the instruction E652 at address CBFE has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_E652()
        {
            AssertCode("@@@", "E652");
        }
        // Reko: a decoder for the instruction 07B1 at address CC02 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_07B1()
        {
            AssertCode("@@@", "07B1");
        }
        // Reko: a decoder for the instruction 27B0 at address CC0E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_27B0()
        {
            AssertCode("@@@", "27B0");
        }
        // Reko: a decoder for the instruction 6248 at address CC14 has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_6248()
        {
            AssertCode("@@@", "6248");
        }
        // Reko: a decoder for the instruction 100027602FAC at address CC1C has not been implemented. (Fmt2 3 ZZ ope 6  cbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_100027602FAC()
        {
            AssertCode("@@@", "100027602FAC");
        }
        // Reko: a decoder for the instruction 1400A2B0224C at address CC22 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A2B0224C()
        {
            AssertCode("@@@", "1400A2B0224C");
        }
        // Reko: a decoder for the instruction 22B1 at address CC2E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_22B1()
        {
            AssertCode("@@@", "22B1");
        }
        // Reko: a decoder for the instruction 2439 at address CC34 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2439()
        {
            AssertCode("@@@", "2439");
        }
        // Reko: a decoder for the instruction 4A47 at address CC36 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_4A47()
        {
            AssertCode("@@@", "4A47");
        }
        // Reko: a decoder for the instruction 363B at address CC38 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_363B()
        {
            AssertCode("@@@", "363B");
        }
        // Reko: a decoder for the instruction 1321 at address CC44 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1321()
        {
            AssertCode("@@@", "1321");
        }
        // Reko: a decoder for the instruction 2145 at address CC72 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_2145()
        {
            AssertCode("@@@", "2145");
        }
        // Reko: a decoder for the instruction 1400A2B0005F at address CC7A has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A2B0005F()
        {
            AssertCode("@@@", "1400A2B0005F");
        }
        // Reko: a decoder for the instruction 5A47 at address CC92 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_5A47()
        {
            AssertCode("@@@", "5A47");
        }
        // Reko: a decoder for the instruction 0DF0 at address CC9C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DF0()
        {
            AssertCode("@@@", "0DF0");
        }
        // Reko: a decoder for the instruction 1D60 at address CC9E has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1D60()
        {
            AssertCode("@@@", "1D60");
        }
        // Reko: a decoder for the instruction 1FAA at address CCAA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FAA()
        {
            AssertCode("@@@", "1FAA");
        }
        // Reko: a decoder for the instruction 7FE0 at address CCB4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7FE0()
        {
            AssertCode("@@@", "7FE0");
        }
        // Reko: a decoder for the instruction F120 at address CCC0 has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_F120()
        {
            AssertCode("@@@", "F120");
        }
        // Reko: a decoder for the instruction 8632 at address CCD6 has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8632()
        {
            AssertCode("@@@", "8632");
        }
        // Reko: a decoder for the instruction 1760 at address CCD8 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1760()
        {
            AssertCode("@@@", "1760");
        }
        // Reko: a decoder for the instruction A223 at address CCE0 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A223()
        {
            AssertCode("@@@", "A223");
        }
        // Reko: a decoder for the instruction 1539 at address CCE6 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1539()
        {
            AssertCode("@@@", "1539");
        }
        // Reko: a decoder for the instruction 1400A4B0244C at address CCF6 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A4B0244C()
        {
            AssertCode("@@@", "1400A4B0244C");
        }
        // Reko: a decoder for the instruction 1FA2 at address CCFC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FA2()
        {
            AssertCode("@@@", "1FA2");
        }
        // Reko: a decoder for the instruction 34B0 at address CD00 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_34B0()
        {
            AssertCode("@@@", "34B0");
        }
        // Reko: a decoder for the instruction 14B1 at address CD04 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14B1()
        {
            AssertCode("@@@", "14B1");
        }
        // Reko: a decoder for the instruction B320 at address CD1C has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B320()
        {
            AssertCode("@@@", "B320");
        }
        // Reko: a decoder for the instruction 4545 at address CD36 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_4545()
        {
            AssertCode("@@@", "4545");
        }
        // Reko: a decoder for the instruction 1400A4B0225F at address CD3E has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400A4B0225F()
        {
            AssertCode("@@@", "1400A4B0225F");
        }
        // Reko: a decoder for the instruction 44B1 at address CD50 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_44B1()
        {
            AssertCode("@@@", "44B1");
        }
        // Reko: a decoder for the instruction 4939 at address CD56 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_4939()
        {
            AssertCode("@@@", "4939");
        }
        // Reko: a decoder for the instruction 9A47 at address CD58 has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_9A47()
        {
            AssertCode("@@@", "9A47");
        }
        // Reko: a decoder for the instruction 563B at address CD5A has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_563B()
        {
            AssertCode("@@@", "563B");
        }
        // Reko: a decoder for the instruction 3521 at address CD62 has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3521()
        {
            AssertCode("@@@", "3521");
        }
        // Reko: a decoder for the instruction 47B0 at address CD72 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_47B0()
        {
            AssertCode("@@@", "47B0");
        }
        // Reko: a decoder for the instruction 6448 at address CD78 has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_6448()
        {
            AssertCode("@@@", "6448");
        }
        // Reko: a decoder for the instruction 6933 at address CD7E has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_6933()
        {
            AssertCode("@@@", "6933");
        }
        // Reko: a decoder for the instruction 9353 at address CD80 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_9353()
        {
            AssertCode("@@@", "9353");
        }
        // Reko: a decoder for the instruction 1445 at address CD8C has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_1445()
        {
            AssertCode("@@@", "1445");
        }
        // Reko: a decoder for the instruction A423 at address CD90 has not been implemented. (Fmt15 1 ZZ  andw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_A423()
        {
            AssertCode("@@@", "A423");
        }
        // Reko: a decoder for the instruction 1239 at address CD98 has not been implemented. (Fmt15 1 ZZ  subb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1239()
        {
            AssertCode("@@@", "1239");
        }
        // Reko: a decoder for the instruction 2A47 at address CD9A has not been implemented. (0100 0111 xxxx xxxx  Fmt15 1 ZZ  lshd reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_2A47()
        {
            AssertCode("@@@", "2A47");
        }
        // Reko: a decoder for the instruction 5153 at address CDBA has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_5153()
        {
            AssertCode("@@@", "5153");
        }
        // Reko: a decoder for the instruction 1FA6 at address CDDA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FA6()
        {
            AssertCode("@@@", "1FA6");
        }
        // Reko: a decoder for the instruction 4161 at address CDDC has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4161()
        {
            AssertCode("@@@", "4161");
        }
        // Reko: a decoder for the instruction AFE0 at address CDF2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_AFE0()
        {
            AssertCode("@@@", "AFE0");
        }
        // Reko: a decoder for the instruction 14B0 at address CDF4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14B0()
        {
            AssertCode("@@@", "14B0");
        }
        // Reko: a decoder for the instruction 1DF0 at address CDF6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DF0()
        {
            AssertCode("@@@", "1DF0");
        }
        // Reko: a decoder for the instruction 3954 at address CDFC has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3954()
        {
            AssertCode("@@@", "3954");
        }
        // Reko: a decoder for the instruction 4961 at address CDFE has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_4961()
        {
            AssertCode("@@@", "4961");
        }
        // Reko: a decoder for the instruction 14B2 at address CE04 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14B2()
        {
            AssertCode("@@@", "14B2");
        }
        // Reko: a decoder for the instruction 1DF2 at address CE06 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1DF2()
        {
            AssertCode("@@@", "1DF2");
        }
        // Reko: a decoder for the instruction 2D60 at address CE28 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_2D60()
        {
            AssertCode("@@@", "2D60");
        }
        // Reko: a decoder for the instruction 02F1 at address CE2C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02F1()
        {
            AssertCode("@@@", "02F1");
        }
        // Reko: a decoder for the instruction 73FE at address CE30 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_73FE()
        {
            AssertCode("@@@", "73FE");
        }
        // Reko: a decoder for the instruction B120 at address CE3A has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_B120()
        {
            AssertCode("@@@", "B120");
        }
        // Reko: a decoder for the instruction 0CD2 at address CE4C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0CD2()
        {
            AssertCode("@@@", "0CD2");
        }
        // Reko: a decoder for the instruction 7522 at address CE5C has not been implemented. (ZZ andw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_7522()
        {
            AssertCode("@@@", "7522");
        }
        // Reko: a decoder for the instruction 5245 at address CE60 has not been implemented. (0100 0101 xxxx xxxx  Fmt15 1 ZZ  ashuw reg, reg 4 dest reg 4 count reg)
        [Test]
        public void Cr16Dasm_5245()
        {
            AssertCode("@@@", "5245");
        }
        // Reko: a decoder for the instruction 14002AB008E0 at address CE66 has not been implemented. (Fmt1 2 ZZ ope 11  andd rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14002AB008E0()
        {
            AssertCode("@@@", "14002AB008E0");
        }
        // Reko: a decoder for the instruction D8E6 at address CE6C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_D8E6()
        {
            AssertCode("@@@", "D8E6");
        }
        // Reko: a decoder for the instruction 3FA8 at address CE6E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3FA8()
        {
            AssertCode("@@@", "3FA8");
        }
        // Reko: a decoder for the instruction 5032 at address CE7A has not been implemented. (ZZ addw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5032()
        {
            AssertCode("@@@", "5032");
        }
        // Reko: a decoder for the instruction 6FAA at address CE7E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FAA()
        {
            AssertCode("@@@", "6FAA");
        }
        // Reko: a decoder for the instruction 0F9A at address CE84 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0F9A()
        {
            AssertCode("@@@", "0F9A");
        }
        // Reko: a decoder for the instruction ACEF at address CE90 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_ACEF()
        {
            AssertCode("@@@", "ACEF");
        }
        // Reko: a decoder for the instruction 27B1 at address CEA0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_27B1()
        {
            AssertCode("@@@", "27B1");
        }
        // Reko: a decoder for the instruction 815A at address CEA6 has not been implemented. (ZZ movw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_815A()
        {
            AssertCode("@@@", "815A");
        }
        // Reko: a decoder for the instruction 1248 at address CEAA has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_1248()
        {
            AssertCode("@@@", "1248");
        }
        // Reko: a decoder for the instruction 10002760E018 at address CEBA has not been implemented. (Fmt2 3 ZZ ope 6  cbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_10002760E018()
        {
            AssertCode("@@@", "10002760E018");
        }
        // Reko: a decoder for the instruction 35FE at address CEC0 has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_35FE()
        {
            AssertCode("@@@", "35FE");
        }
        // Reko: a decoder for the instruction 4F98 at address CECA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4F98()
        {
            AssertCode("@@@", "4F98");
        }
        // Reko: a decoder for the instruction 5108 at address CF40 has not been implemented. (Fmt15 1 ZZ  Scond (reg) 4 dest reg 4 cond imm)
        [Test]
        public void Cr16Dasm_5108()
        {
            AssertCode("@@@", "5108");
        }
        // Reko: a decoder for the instruction 2325 at address CF50 has not been implemented. (Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2325()
        {
            AssertCode("@@@", "2325");
        }
        // Reko: a decoder for the instruction 3221 at address CF5A has not been implemented. (Fmt15 1 ZZ  andb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3221()
        {
            AssertCode("@@@", "3221");
        }
        // Reko: a decoder for the instruction 1220 at address CF70 has not been implemented. (ZZ andb imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_1220()
        {
            AssertCode("@@@", "1220");
        }
        // Reko: a decoder for the instruction 9290 at address CF9C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9290()
        {
            AssertCode("@@@", "9290");
        }
        // Reko: a decoder for the instruction 90D0 at address CF9E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_90D0()
        {
            AssertCode("@@@", "90D0");
        }
        // Reko: a decoder for the instruction 9F90 at address CFA6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9F90()
        {
            AssertCode("@@@", "9F90");
        }
        // Reko: a decoder for the instruction D161 at address CFF0 has not been implemented. (Fmt15 1 ZZ  addd rp, rp 4 dest rp 4 src rp)
        [Test]
        public void Cr16Dasm_D161()
        {
            AssertCode("@@@", "D161");
        }
        // Reko: a decoder for the instruction 12B1 at address D020 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12B1()
        {
            AssertCode("@@@", "12B1");
        }
        // Reko: a decoder for the instruction 3D60 at address D024 has not been implemented. (ZZ addd imm4_16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_3D60()
        {
            AssertCode("@@@", "3D60");
        }
        // Reko: a decoder for the instruction 12B2 at address D026 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12B2()
        {
            AssertCode("@@@", "12B2");
        }
        // Reko: a decoder for the instruction 14F2 at address D028 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14F2()
        {
            AssertCode("@@@", "14F2");
        }
        // Reko: a decoder for the instruction 67FC at address D03C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_67FC()
        {
            AssertCode("@@@", "67FC");
        }
        // Reko: a decoder for the instruction 12B3 at address D03E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_12B3()
        {
            AssertCode("@@@", "12B3");
        }
        // Reko: a decoder for the instruction 14F3 at address D040 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_14F3()
        {
            AssertCode("@@@", "14F3");
        }
        // Reko: a decoder for the instruction 5D54 at address D048 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_5D54()
        {
            AssertCode("@@@", "5D54");
        }
        // Reko: a decoder for the instruction 02B4 at address D04C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_02B4()
        {
            AssertCode("@@@", "02B4");
        }
        // Reko: a decoder for the instruction 04F4 at address D04E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_04F4()
        {
            AssertCode("@@@", "04F4");
        }
        // Reko: a decoder for the instruction 47B1 at address D054 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_47B1()
        {
            AssertCode("@@@", "47B1");
        }
        // Reko: a decoder for the instruction 9448 at address D05A has not been implemented. (0100 1000 xxxx xxxx  Fmt15 1 ZZ  ashud reg, rp 4 dest rp 4 count reg)
        [Test]
        public void Cr16Dasm_9448()
        {
            AssertCode("@@@", "9448");
        }
        // Reko: a decoder for the instruction 1225 at address D098 has not been implemented. (Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1225()
        {
            AssertCode("@@@", "1225");
        }
        // Reko: a decoder for the instruction 4D54 at address D14A has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_4D54()
        {
            AssertCode("@@@", "4D54");
        }
        // Reko: a decoder for the instruction 53FB at address D150 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_53FB()
        {
            AssertCode("@@@", "53FB");
        }
        // Reko: a decoder for the instruction E9FC at address D168 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_E9FC()
        {
            AssertCode("@@@", "E9FC");
        }
        // Reko: a decoder for the instruction 35FB at address D16E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_35FB()
        {
            AssertCode("@@@", "35FB");
        }
        // Reko: a decoder for the instruction 303B at address D170 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_303B()
        {
            AssertCode("@@@", "303B");
        }
        // Reko: a decoder for the instruction 2125 at address D26E has not been implemented. (Fmt15 1 ZZ  orb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2125()
        {
            AssertCode("@@@", "2125");
        }
        // Reko: a decoder for the instruction C552 at address D272 has not been implemented. (ZZ cmpw imm4_16, reg 4 src reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_C552()
        {
            AssertCode("@@@", "C552");
        }
        // Reko: a decoder for the instruction 1400D2906200 at address D280 has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_1400D2906200()
        {
            AssertCode("@@@", "1400D2906200");
        }
        // Reko: a decoder for the instruction 1D90 at address D294 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1D90()
        {
            AssertCode("@@@", "1D90");
        }
        // Reko: a decoder for the instruction DF9F at address D2B0 has not been implemented. (MemAccessDecoder - 0xF)
        [Test]
        public void Cr16Dasm_DF9F()
        {
            AssertCode("@@@", "DF9F");
        }
        // Reko: a decoder for the instruction D553 at address D2B4 has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_D553()
        {
            AssertCode("@@@", "D553");
        }
        // Reko: a decoder for the instruction 6FFE at address D2CC has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_6FFE()
        {
            AssertCode("@@@", "6FFE");
        }
        // Reko: a decoder for the instruction 19FD at address D2D4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_19FD()
        {
            AssertCode("@@@", "19FD");
        }
        // Reko: a decoder for the instruction 84B0 at address D2F2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_84B0()
        {
            AssertCode("@@@", "84B0");
        }
        // Reko: a decoder for the instruction 86F0 at address D2F4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_86F0()
        {
            AssertCode("@@@", "86F0");
        }
        // Reko: a decoder for the instruction 6FE0 at address D2FA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6FE0()
        {
            AssertCode("@@@", "6FE0");
        }
        // Reko: a decoder for the instruction 2DFE at address D30A has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_2DFE()
        {
            AssertCode("@@@", "2DFE");
        }
        // Reko: a decoder for the instruction ADFC at address D340 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ADFC()
        {
            AssertCode("@@@", "ADFC");
        }
        // Reko: a decoder for the instruction 1063 at address D35C has not been implemented. (Fmt15 1 ZZ  muluw reg, rp 4 dest rp 4 src reg)
        [Test]
        public void Cr16Dasm_1063()
        {
            AssertCode("@@@", "1063");
        }
        // Reko: a decoder for the instruction 2567 at address D35E has not been implemented. (Fmt15 1 ZZ  mulw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_2567()
        {
            AssertCode("@@@", "2567");
        }
        // Reko: a decoder for the instruction 3467 at address D360 has not been implemented. (Fmt15 1 ZZ  mulw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3467()
        {
            AssertCode("@@@", "3467");
        }
        // Reko: a decoder for the instruction 5133 at address D364 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_5133()
        {
            AssertCode("@@@", "5133");
        }
        // Reko: a decoder for the instruction 3333 at address D372 has not been implemented. (Fmt15 1 ZZ  addw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_3333()
        {
            AssertCode("@@@", "3333");
        }
        // Reko: a decoder for the instruction 3053 at address D38E has not been implemented. (Fmt15 1 ZZ  cmpw reg, reg 4 src reg 4 src reg)
        [Test]
        public void Cr16Dasm_3053()
        {
            AssertCode("@@@", "3053");
        }
        // Reko: a decoder for the instruction 1527 at address D394 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_1527()
        {
            AssertCode("@@@", "1527");
        }
        // Reko: a decoder for the instruction 152A at address D3C0 has not been implemented. (ZZ xorw imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_152A()
        {
            AssertCode("@@@", "152A");
        }
        // Reko: a decoder for the instruction 343B at address D3E8 has not been implemented. (Fmt15 1 ZZ  subw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_343B()
        {
            AssertCode("@@@", "343B");
        }
        // Reko: a decoder for the instruction 1043 at address D41E has not been implemented. (0100 0011 xxxx xxxx  Fmt15 1 ZZ  ashuw cnt(right -), reg 4 dest reg 4 count imm)
        [Test]
        public void Cr16Dasm_1043()
        {
            AssertCode("@@@", "1043");
        }
        // Reko: a decoder for the instruction 032B at address D420 has not been implemented. (Fmt15 1 ZZ  xorw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_032B()
        {
            AssertCode("@@@", "032B");
        }
        // Reko: a decoder for the instruction 9027 at address D514 has not been implemented. (Fmt15 1 ZZ  orw reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_9027()
        {
            AssertCode("@@@", "9027");
        }
        // Reko: a decoder for the instruction 8E90 at address D530 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_8E90()
        {
            AssertCode("@@@", "8E90");
        }
        // Reko: a decoder for the instruction F84B at address D532 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F84B()
        {
            AssertCode("@@@", "F84B");
        }
        // Reko: a decoder for the instruction F44B at address D534 has not been implemented. (0100 101x xxxx xxxx  Fmt20 1 ZZ  lshd cnt(right -), rp 4 dest rp 5 count imm)
        [Test]
        public void Cr16Dasm_F44B()
        {
            AssertCode("@@@", "F44B");
        }
        // Reko: a decoder for the instruction 002C at address D554 has not been implemented. (ZZ addub imm4_16, reg 4 dest reg 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_002C()
        {
            AssertCode("@@@", "002C");
        }
        // Reko: a decoder for the instruction 8054 at address D564 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_8054()
        {
            AssertCode("@@@", "8054");
        }
        // Reko: a decoder for the instruction C054 at address D580 has not been implemented. (ZZ movd imm4 / imm16, rp 4 dest rp 4 src imm 15 / 16 1 / 2)
        [Test]
        public void Cr16Dasm_C054()
        {
            AssertCode("@@@", "C054");
        }
        // Reko: a decoder for the instruction 2DE0 at address D586 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DE0()
        {
            AssertCode("@@@", "2DE0");
        }
        // Reko: a decoder for the instruction 00A2 at address D588 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00A2()
        {
            AssertCode("@@@", "00A2");
        }
        // Reko: a decoder for the instruction 0DE2 at address D58A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0DE2()
        {
            AssertCode("@@@", "0DE2");
        }
        // Reko: a decoder for the instruction 1000D06120A0 at address D5C2 has not been implemented. (Fmt2 3 ZZ ope 6  cbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000D06120A0()
        {
            AssertCode("@@@", "1000D06120A0");
        }
        // Reko: a decoder for the instruction 20A2 at address D5CA has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_20A2()
        {
            AssertCode("@@@", "20A2");
        }
        // Reko: a decoder for the instruction 2DE2 at address D5CC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DE2()
        {
            AssertCode("@@@", "2DE2");
        }
        // Reko: a decoder for the instruction 00A1 at address D608 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_00A1()
        {
            AssertCode("@@@", "00A1");
        }
        // Reko: a decoder for the instruction 4DE2 at address D622 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DE2()
        {
            AssertCode("@@@", "4DE2");
        }
        // Reko: a decoder for the instruction 1000D0612DE0 at address D63A has not been implemented. (Fmt2 3 ZZ ope 6  cbitb (rrp) disp20 4 dest (rrp) 3 pos imm 20 dest disp 4)
        [Test]
        public void Cr16Dasm_1000D0612DE0()
        {
            AssertCode("@@@", "1000D0612DE0");
        }
        // Reko: a decoder for the instruction 1400D0612DE0 at address D67A has not been implemented. (Fmt1 2 ZZ ope 6  res - no operation 4)
        [Test]
        public void Cr16Dasm_1400D0612DE0()
        {
            AssertCode("@@@", "1400D0612DE0");
        }
        // Reko: a decoder for the instruction 2DE4 at address D684 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DE4()
        {
            AssertCode("@@@", "2DE4");
        }
        // Reko: a decoder for the instruction 10001D02EE0A at address D690 has not been implemented. (Fmr3a 3 bra cond disp24 24 dest disp*2 4 cond imm 4 ope 0)
        [Test]
        public void Cr16Dasm_10001D02EE0A()
        {
            AssertCode("@@@", "10001D02EE0A");
        }
        // Reko: a decoder for the instruction 2DD0 at address D6C0 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_2DD0()
        {
            AssertCode("@@@", "2DD0");
        }
        // Reko: a decoder for the instruction 5DE4 at address D6C4 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5DE4()
        {
            AssertCode("@@@", "5DE4");
        }
        // Reko: a decoder for the instruction 4DD2 at address D78E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_4DD2()
        {
            AssertCode("@@@", "4DD2");
        }
        // Reko: a decoder for the instruction 6DD4 at address D9FC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_6DD4()
        {
            AssertCode("@@@", "6DD4");
        }
        // Reko: a decoder for the instruction 3DD1 at address DDE2 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_3DD1()
        {
            AssertCode("@@@", "3DD1");
        }
        // Reko: a decoder for the instruction 5DD1 at address DE24 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_5DD1()
        {
            AssertCode("@@@", "5DD1");
        }
        // Reko: a decoder for the instruction 0F83 at address E038 has not been implemented. (Fmt16 2 ZZ  storb imm(rp) disp16 4 dest(rp) 4 src imm 16 dest disp)
        [Test]
        public void Cr16Dasm_0F83()
        {
            AssertCode("@@@", "0F83");
        }
        // Reko: a decoder for the instruction A3A0 at address E03C has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_A3A0()
        {
            AssertCode("@@@", "A3A0");
        }
        // Reko: a decoder for the instruction 1300A4B02800 at address E03E has not been implemented. (Fmt3 3 ZZ ope 11  stord abs24 24 dest abs 4 src rp 4)
        [Test]
        public void Cr16Dasm_1300A4B02800()
        {
            AssertCode("@@@", "1300A4B02800");
        }
        // Reko: a decoder for the instruction 140000520A10 at address E068 has not been implemented. (Fmt1 2 ZZ ope 5  res - no operation 4)
        [Test]
        public void Cr16Dasm_140000520A10()
        {
            AssertCode("@@@", "140000520A10");
        }
        // Reko: a decoder for the instruction 7F9A at address E078 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_7F9A()
        {
            AssertCode("@@@", "7F9A");
        }
        // Reko: a decoder for the instruction ADA0 at address E0B6 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_ADA0()
        {
            AssertCode("@@@", "ADA0");
        }
        // Reko: a decoder for the instruction 0FB3 at address E0CE has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FB3()
        {
            AssertCode("@@@", "0FB3");
        }
        // Reko: a decoder for the instruction 1F9A at address E0DC has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1F9A()
        {
            AssertCode("@@@", "1F9A");
        }
        // Reko: a decoder for the instruction 120024B03000 at address E0E4 has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120024B03000()
        {
            AssertCode("@@@", "120024B03000");
        }
        // Reko: a decoder for the instruction 120044B03C00 at address E0FE has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120044B03C00()
        {
            AssertCode("@@@", "120044B03C00");
        }
        // Reko: a decoder for the instruction 9FDA at address E114 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_9FDA()
        {
            AssertCode("@@@", "9FDA");
        }
        // Reko: a decoder for the instruction 1FF3 at address E12A has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1FF3()
        {
            AssertCode("@@@", "1FF3");
        }
        // Reko: a decoder for the instruction 0131 at address E152 has not been implemented. (Fmt15 1 ZZ  addb reg, reg 4 dest reg 4 src reg)
        [Test]
        public void Cr16Dasm_0131()
        {
            AssertCode("@@@", "0131");
        }
        // Reko: a decoder for the instruction 1AB2 at address E158 has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_1AB2()
        {
            AssertCode("@@@", "1AB2");
        }
        // Reko: a decoder for the instruction 0FF2 at address E15E has not been implemented. (MemAccessDecoder)
        [Test]
        public void Cr16Dasm_0FF2()
        {
            AssertCode("@@@", "0FF2");
        }
        // Reko: a decoder for the instruction 120024B03C00 at address E1B2 has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120024B03C00()
        {
            AssertCode("@@@", "120024B03C00");
        }
        // Reko: a decoder for the instruction 120004B02800 at address E1EC has not been implemented. (Fmt3 3 ZZ ope 11  loadd abs24 24 src abs 4 dest rp 4)
        [Test]
        public void Cr16Dasm_120004B02800()
        {
            AssertCode("@@@", "120004B02800");
        }
        // Reko: a decoder for the instruction 1000E51C0F83 at address E208 has not been implemented. (Fmt3 3 ZZ ope 1  res - no operation 4)
        [Test]
        public void Cr16Dasm_1000E51C0F83()
        {
            AssertCode("@@@", "1000E51C0F83");
        }
        // Reko: a decoder for the instruction 14007F9AE018 at address E216 has not been implemented. (Fmt1 2 ZZ ope 9  ord rp, rp 4 dest rp 4 src rp 4)
        [Test]
        public void Cr16Dasm_14007F9AE018()
        {
            AssertCode("@@@", "14007F9AE018");
        }
        // Reko: a decoder for the instruction 67FE at address E21C has not been implemented. (MemAccessDecoder - 0xE)
        [Test]
        public void Cr16Dasm_67FE()
        {
            AssertCode("@@@", "67FE");
        }
#endif
    }
}
