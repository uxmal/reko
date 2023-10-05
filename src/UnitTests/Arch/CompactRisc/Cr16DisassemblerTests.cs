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
            //if (!instr.ToString().Contains("nvalid"))
            //    return;
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
            AssertCode("br\tFFFFA344", "E018 4523");
        }

        [Test]
        public void Cr16Dasm_cbitb_abs20()
        {
            AssertCode("cbitb\tr8,(0x042345)", "84684523");
        }

        [Test]
        public void Cr16Dasm_cbitb_regression()
        {
            AssertCode("cbitb\t$7,1(r8,r7)", "376B0100");
        }

        [Test]
        public void Cr16Dasm_cbitb_rp_disp0()
        {
            AssertCode("cbitb\t$0,(r1,r0)", "006A");
        }

        [Test]
        public void Cr16Dasm_cbitb_rp_disp20()
        {
            AssertCode("cbitb\t$1,0xA9F1C(r2,r1)", "1000195A1C9F");
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
            AssertCode("lshd\t$-18,ra", "2E4B");
        }

        [Test]
        public void Cr16Dasm_nop()
        {
            AssertCode("nop", "002C");
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
            AssertCode("mulsb\tr0,r4", "040B");
        }

        [Test]
        public void Cr16Dasm_muluw()
        {
            AssertCode("muluw\tr0,ra", "0E63");
        }

        [Test]
        public void Cr16Dasm_ord()
        {
            AssertCode("ord\t(r9,r8),(r1,r0)", "14008090");
        }

        [Test]
        public void Cr16Dasm_orw_imm4_16_reg()
        {
            AssertCode("orw\t$2345,r11", "B0264523");
        }

        [Test]
        public void Cr16Dasm_pop()
        {
            AssertCode("pop\t$1,r7", "0702");
        }

        [Test]
        public void Cr16Dasm_popret()
        {
            AssertCode("popret\t$2,ra", "1E03");    //$REVIEW:$1?
        }

        [Test]
        public void Cr16Dasm_push()
        {
            AssertCode("push\t$2,ra", "1E01");
        }

        [Test]
        public void Cr16Dasm_push_3_r8()
        {
            AssertCode("push\t$4,r8", "3801");
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
        public void Cr16Dasm_sbitb_regression1()
        {
            AssertCode("sbitb\t$7,1(r8,r7)", "2773 0100");
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
            AssertCode("stord\t(r11,r10),0x73456(r10)", "A7CD 5634");
        }

        [Test]
        public void Cr16Dasm_stord_regression1()
        {
            AssertCode("stord\t(r8,r7),0x59000(r7)", "75CD 0090");
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

        [Test]
        public void Cr16Dasm_xord()
        {
            AssertCode("xord\t(r1,r0),(r3,r2)", "140002A0");
        }

        [Test]
        public void Cr16Dasm_xorw()
        {
            AssertCode("xorw\t$FFFF,r9", "912A");
        }
    }
}
