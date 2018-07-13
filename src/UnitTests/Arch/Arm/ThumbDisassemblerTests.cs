#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.Arm;
using Reko.Arch.Arm.AArch32;
using Reko.Core;
using Reko.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    [Category(Categories.Capstone)]
    public class ThumbDisassemblerTests : ArmTestBase
    {
        private IEnumerator<MachineInstruction> dasm;

        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new ThumbArchitecture("arm-thumb");
        }

        protected MachineInstruction Disassemble16(params ushort[] instrs)
        {
            Given_Instructions(instrs);
            Assert.IsTrue(dasm.MoveNext());
            var armInstr = dasm.Current;
            dasm.Dispose();
            return armInstr;
        }

        /// <summary>
        /// Establishes a disassembler instance for further tests.
        /// </summary>
        /// <param name="instrs"></param>
        private void Given_Instructions(params ushort[] instrs)
        {
            var w = new LeImageWriter();
            foreach (var instr in instrs)
            {
                w.WriteLeUInt16(instr);
            }
            var image = new MemoryArea(Address.Ptr32(0x00100000), w.ToArray());
            var arch = CreateArchitecture();
            this.dasm = CreateDisassembler(arch, image.CreateLeReader(0));
        }

        private void AssertCode(string sExp, params ushort[] instrs)
        {
            var instr = Disassemble16(instrs);
            Assert.AreEqual(sExp, instr.ToString());
        }

        private void Expect_Code(string sExp)
        {
            Assert.IsTrue(dasm.MoveNext());
            var instr = dasm.Current;
            Assert.AreEqual(sExp, instr.ToString());
        }

        protected override IEnumerator<MachineInstruction> CreateDisassembler(IProcessorArchitecture arch, EndianImageReader rdr)
        {
            return arch.CreateDisassembler(rdr).GetEnumerator();
        }

        [Test]
        public void ThumbDis_push()
        {
            var instr = Disassemble16(0xE92D, 0x4800);
            Assert.AreEqual("push.w\t{fp,lr}", instr.ToString());
        }

        [Test]
        public void ThumbDis_mov()
        {
            var instr = Disassemble16(0x46EB);
            Assert.AreEqual("mov\tfp,sp", instr.ToString());
        }

        [Test]
        public void ThumbDis_sub_sp()
        {
            var instr = Disassemble16(0xB082);
            Assert.AreEqual("sub\tsp,#8", instr.ToString());
        }

        [Test]
        public void ThumbDis_bl()
        {
            var instr = Disassemble16(0xF000, 0xFA06);
            Assert.AreEqual("bl\t$0010040C", instr.ToString());
        }

        [Test]
        public void ThumbDis_str()
        {
            var instr = Disassemble16(0x9000);
            Assert.AreEqual("str\tr0,[sp]", instr.ToString());
        }

        [Test]
        public void ThumbDis_ldr()
        {
            var instr = Disassemble16(0x9B00);
            Assert.AreEqual("ldr\tr3,[sp]", instr.ToString());
        }

        [Test]
        public void ThumbDis_ldr_displacement()
        {
            var instr = Disassemble16(0x9801);
            Assert.AreEqual("ldr\tr0,[sp,#&4]", instr.ToString());
        }

        [Test]
        public void ThumbDis_add_sp()
        {
            var instr = Disassemble16(0xB002);
            Assert.AreEqual("add\tsp,#8", instr.ToString());
        }

        [Test]
        public void ThumbDis_pop()
        {
            var instr = Disassemble16(0xE8BD, 0x8800);
            Assert.AreEqual("pop.w\t{fp,pc}", instr.ToString());
        }

        [Test]
        public void ThumbDis_add()
        {
            AssertCode("add\tsp,#8", 0xB002);
        }

        [Test]
        public void ThumbDis_adr()
        {
            AssertCode("adr\tr0,$00100080", 0xA020);
        }

        [Test]
        public void ThumbDis_add_sp_imm()
        {
            AssertCode("add\tr0,sp,#&80", 0xA820);
        }

        [Test]
        public void ThumbDis_addw()
        {
            AssertCode("add\tr3,sp,#&A48", 0xF60D, 0x2348);
        }

        [Test]
        public void ThumbDis_it_ne()
        {
            Given_Instructions(0xBF18, 0x4630, 0x4631);
            Expect_Code("it\tne");
            Expect_Code("movne\tr0,r6");
            Expect_Code("mov\tr1,r6");
        }

        [Test]
        public void ThumbDis_ite_eq()
        {
            Given_Instructions(0xBF0C, 0x4630, 0x4631, 0x4632);
            Expect_Code("ite\teq");
            Expect_Code("moveq\tr0,r6");
            Expect_Code("movne\tr1,r6");
            Expect_Code("mov\tr2,r6");
        }

        [Test]
        public void ThumbDis_itt_eq()
        {
            Given_Instructions(0xBF04, 0x4630, 0x4631, 0x4632);
            Expect_Code("itt\teq");
            Expect_Code("moveq\tr0,r6");
            Expect_Code("moveq\tr1,r6");
            Expect_Code("mov\tr2,r6");
        }

        [Test]
        public void ThumbDis_itttt_ne()
        {
            Given_Instructions(0xBF1F, 0x4630, 0x4631, 0x4632, 0x4633, 0x4634);
            Expect_Code("itttt\tne");
            Expect_Code("movne\tr0,r6");
            Expect_Code("movne\tr1,r6");
            Expect_Code("movne\tr2,r6");
            Expect_Code("movne\tr3,r6");
            Expect_Code("mov\tr4,r6");
        }

        [Test]
        public void ThumbDis_iteee_xx()
        {
            Given_Instructions(0xBF31, 0x4630, 0x4631, 0x4632, 0x4633, 0x4634);
            Expect_Code("iteee\tlo");
            Expect_Code("movlo\tr0,r6");
            Expect_Code("movhs\tr1,r6");
            Expect_Code("movhs\tr2,r6");
            Expect_Code("movhs\tr3,r6");
            Expect_Code("mov\tr4,r6");
        }

        [Test]
        public void ThumbDis_mrc()
        {
            Given_Instructions(0xEE1D, 0x3F50);
            Expect_Code("mrc\tp15,#0,r3,c13,c0,#2");
        }

        [Test]
        public void ThumbDis_mcr()
        {
            Given_Instructions(0xEE01, 0x3F10);
            Expect_Code("mcr\tp15,#0,r3,c1,c0,#0");
        }

        [Test]
        public void ThumbDis_ldrsb_pos_offset()
        {
            Given_Instructions(0xF914, 0x3B44);
            Expect_Code("ldrsb\tr3,[r4],#&44");
        }

        [Test]
        public void ThumbDis_ldrsb_neg_offset()
        {
            Given_Instructions(0xF914, 0x3944);
            Expect_Code("ldrsb\tr3,[r4],-#&44");
        }

        [Test]
        public void ThumbDis_ldrsb()
        {
            Given_Instructions(0xF991, 0x3000);  // ldrsb       r3,[r1]
            Expect_Code("ldrsb\tr3,[r1]");
        }

        [Test]
        public void ThumbDis_ldr_postindex()
        {
            Given_Instructions(0xF85D, 0xFB0C);  // ldr         pc,[sp],#0xC
            Expect_Code("ldr\tpc,[sp],#&C");
        }

        [Test]
        public void ThumbDis_ldrd()
        {
            Given_Instructions(0xF85D, 0xFB0C);  // ldr         pc,[sp],#0xC
            Expect_Code("ldr\tpc,[sp],#&C");
        }

        [Test]
        public void ThumbDis_movt()
        {
            Given_Instructions(0xF6CF, 0x7AFF);  // movt r10,#&FFFF
            Expect_Code("movt\tr10,#&FFFF");
        }

        [Test]
        public void ThumbDis_mov_imm()
        {
            Given_Instructions(0x2004);
            Expect_Code("mov\tr0,#4");
        }

        [Test]
        public void ThumbDis_ldr_imm()
        {
            Given_Instructions(0xF895, 0x4045);
            Expect_Code("ldrb\tr4,[r5,-#&45]");
        }

        [Test]
        public void ThumbDis_b_T2_variant()
        {
            Given_Instructions(0xE005);
            Expect_Code("b\t$0010000A");
        }

        [Test]
        public void ThumbDis_b_T2_variant_backward()
        {
            Given_Instructions(0xE7FF);
            Expect_Code("b\t$000FFFFE");
        }

        [Test]
        public void ThumbDis_vmlal()
        {
            Given_Instructions(0xFFCB, 0x2800);
            Expect_Code("vmlal.i8\tq9,d11,d0");
        }

        [Test]
        public void ThumbDis_b_T4_variant()
        {
            Given_Instructions(0xF008, 0xBA62);
            Expect_Code("b\t$00D084C4");
        }

        [Test]
        public void ThumbDis_b_T4_variant_negative()
        {
            Given_Instructions(0xF7FF, 0xBFFF);
            Expect_Code("b\t$000FFFFE");
        }

        [Test]
        public void ThumbDis_add_hireg()
        {
            Given_Instructions(0x440B);
            Expect_Code("adds\tr3,r1");
        }

        [Test]
        public void ThumbDis_ldr_literal()
        {
            Given_Instructions(0x4B11);
            Expect_Code("ldr\tr3,[pc,#&44]");
        }


        [Test]
        public void ThumbDis_ldr_literal_long()
        {
            Given_Instructions(0xF8DF, 0x90FC);
            Expect_Code("ldr\tr9,[pc,#&FC]");
        }

        [Test]
        public void ThumbDis_mov_r0_r0()
        {
            Given_Instructions(0x0000);
            Expect_Code("mov\tr0,r0");
        }

        [Test]
        public void ThumbDis_lsls()
        {
            Given_Instructions(0xFA12, 0xF000);
            Expect_Code("lsls\tr0,r2,r0");
        }

        [Test]
        public void ThumbDis_vhadd()
        {
            Given_Instructions(0xFF23, 0xF000);
            Expect_Code("vhadd.u32\td15,d3,d0");
        }

        [Test]
        public void ThumbDis_vhadd_unsigned()
        {
            Given_Instructions(0xFF41, 0xB002);
            Expect_Code("vhadd.u8\td27,d1,d2");
        }

        [Test]
        public void ThumbDis_vhadd_signed()
        {
            Given_Instructions(0xEF41, 0xB002);
            Expect_Code("vhadd.i8\td27,d1,d2");
        }

        [Test]
        public void ThumbDis_vseleq()
        {
            Given_Instructions(0xFE1E, 0x2B00);
            Expect_Code("vselvs.f64\td2,d14,d0");
        }

        [Test]
        public void ThumbDis_strh_idx()
        {
            Given_Instructions(0x5380);
            Expect_Code("strh\tr0,[r0,r6]");
        }

        [Test]
        public void ThumbDis_dsb()
        {
            Given_Instructions(0xF3BF, 0x8F4F);
            Expect_Code("dsb\tsy");
        }


        [Test]
        public void ThumbDis_isb()
        {
            Given_Instructions(0xF3BF, 0x8F6F);
            Expect_Code("isb\tsy");
        }

        [Test]
        public void ThumbDis_mrs()
        {
            Given_Instructions(0xF3EF, 0x8511);
            Expect_Code("mrs\tr5,cpsr");
        }

        [Test]
        public void ThumbDis_msr()
        {
            Given_Instructions(0xF385, 0x8811);
            Expect_Code("msr\tcpsr,r5");
        }

        [Test]
        public void ThumbDis_rsb()
        {
            Given_Instructions(0xF1C4, 0x01F4);   // rsb         r1,r4,#0xF4
            Expect_Code("rsb\tr1,r4,#&F4");
        }

        [Test]
        public void ThumbDis_vmla_float()
        {
            Given_Instructions(0xEFA0, 0x41E5);	// vmla.f32 d4, d16, d5[1]
            Expect_Code("vmla.f32\td4,d16,d21");
        }

        [Test]
        public void ThumbDis_stc()
        {
            Given_Instructions(0xED88, 0x5E3D); // stc p14, c6, [r8, #0xf4]
            Expect_Code("stc\tp14,c5,[r8,#&F4]");
        }

        [Test]
        public void ThumbDis_srsdb()
        {
            Given_Instructions(0xE807, 0x01C9);
            Expect_Code("srsdb\tsp,#9");
        }

        // ------------------------------------------

        // A T32decoder for the instruction E9F84571(Load/store dual (immediate pre-indexed)) has not been implemented yet.
        [Test]
        public void ThumbDis_E9F84571()
        {
            Given_Instructions(0xE9F8, 0x4571);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction E9F74D94(Load/store dual (immediate pre-indexed)) has not been implemented yet.
        [Test]
        public void ThumbDis_E9F74D94()
        {
            Given_Instructions(0xE9F7, 0x4D94);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction FF7FA52D(AdvancedSimd3RegistersSameLength_opc5) has not been implemented yet.
        [Test]
        public void ThumbDis_FF7FA52D()
        {
            Given_Instructions(0xFF7F, 0xA52D);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EFA98978(AdvancedSimdTwoRegistersAndShiftAmount_opc9) has not been implemented yet.
        [Test]
        public void ThumbDis_EFA98978()
        {
            Given_Instructions(0xEFA9, 0x8978);
            Expect_Code("@@@");
        }

        [Test]
        public void ThumbDis_setpan()
        {
            Given_Instructions(0xB61F);
            Expect_Code("setpan\t#1");
        }

        // A T32 decoder for the instruction E96F7994(Load/store dual (immediate pre-indexed)) has not been implemented yet.
        [Test]
        public void ThumbDis_E96F7994()
        {
            Given_Instructions(0xE96F, 0x7994);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EF24E9EB(*vmla) has not been implemented yet.
        [Test]
        public void ThumbDis_EF24E9EB()
        {
            Given_Instructions(0xEF24, 0xE9EB);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EB1BE4CF(DataProcessingShiftedRegister_opc8) has not been implemented yet.
        [Test]
        public void ThumbDis_EB1BE4CF()
        {
            Given_Instructions(0xEB1B, 0xE4CF);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EA6391D5(DataProcessingShiftedRegister_opc3) has not been implemented yet.
        [Test]
        public void ThumbDis_EA6391D5()
        {
            Given_Instructions(0xEA63, 0x91D5);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction E856E6BB(Load/store exclusive, load-acquire/store-release, table branch) has not been implemented yet.
        [Test]
        public void ThumbDis_E856E6BB()
        {
            Given_Instructions(0xE856, 0xE6BB);
            Expect_Code("ldrex\tlr,[r6,#&2EC]");
        }

        // A T32 decoder for the instruction E8771C85(Load/store dual (post-indexed)) has not been implemented yet.
        [Test]
        public void ThumbDis_E8771C85()
        {
            Given_Instructions(0xE877, 0x1C85);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F260A31E(B - T3 variant) has not been implemented yet.
        [Test]
        public void ThumbDis_F260A31E()
        {
            Given_Instructions(0xF260, 0xA31E);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction FF1F2E45(AdvancedSimd3RegistersSameLength_opcE) has not been implemented yet.
        [Test]
        public void ThumbDis_FF1F2E45()
        {
            Given_Instructions(0xFF1F, 0x2E45);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EFE4778A(Unknown format specifier* in * integer when decoding vabdl) has not been implemented yet.
        [Test]
        public void ThumbDis_EFE4778A()
        {
            Given_Instructions(0xEFE4, 0x778A);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EAD46F97(DataProcessingShiftedRegister_opc6) has not been implemented yet.
        [Test]
        public void ThumbDis_EAD46F97()
        {
            Given_Instructions(0xEAD4, 0x6F97);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F599A1DC(B - T3 variant) has not been implemented yet.
        [Test]
        public void ThumbDis_F599A1DC()
        {
            Given_Instructions(0xF599, 0xA1DC);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F189A670(B - T3 variant) has not been implemented yet.
        [Test]
        public void ThumbDis_F189A670()
        {
            Given_Instructions(0xF189, 0xA670);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EE99E813(AdvancedSimdAndFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ThumbDis_EE99E813()
        {
            Given_Instructions(0xEE99, 0xE813);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EA47A767(DataProcessingShiftedRegister_opc2) has not been implemented yet.
        [Test]
        public void ThumbDis_EA47A767()
        {
            Given_Instructions(0xEA47, 0xA767);
            Expect_Code("@@@");
        }

        [Test]
        public void ThumbDis_strex()
        {
            Given_Instructions(0xE843, 0x5560);
            Expect_Code("strex\tr5,r5,[r3,#&180]");
        }

        // A T32 decoder for the instruction F2628DDD(B - T3 variant) has not been implemented yet.
        [Test]
        public void ThumbDis_F2628DDD()
        {
            Given_Instructions(0xF262, 0x8DDD);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction 4125 (MOV, MOVS) has not been implemented yet.
        [Test]
        public void ThumbDis_asrs()
        {
            Given_Instructions(0x4125);
            Expect_Code("asrs\tr5,r4");
        }

        // A T32 decoder for the instruction EB364D00(DataProcessingShiftedRegister_opc9) has not been implemented yet.
        [Test]
        public void ThumbDis_EB364D00()
        {
            Given_Instructions(0xEB36, 0x4D00);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EACAA2AB(DataProcessingShiftedRegister_opc6) has not been implemented yet.
        [Test]
        public void ThumbDis_EACAA2AB()
        {
            Given_Instructions(0xEACA, 0xA2AB);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction E9BE94D3(RFE, RFEDA, RFEDB, RFEIA, RFEIB - T2) has not been implemented yet.
        [Test]
        public void ThumbDis_E9BE94D3()
        {
            Given_Instructions(0xE9BE, 0x94D3);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F7811103(usatLslVariant) has not been implemented yet.
        [Test]
        public void ThumbDis_F7811103()
        {
            Given_Instructions(0xF781, 0x1103);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction E8CBA8D7(Load/store exclusive, load-acquire/store-release, table branch) has not been implemented yet.
        [Test]
        public void ThumbDis_E8CBA8D7()
        {
            Given_Instructions(0xE8CB, 0xA8D7);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F7688AE5(B - T3 variant) has not been implemented yet.
        [Test]
        public void ThumbDis_F7688AE5()
        {
            Given_Instructions(0xF768, 0x8AE5);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EA4274F1(DataProcessingShiftedRegister_opc2) has not been implemented yet.
        [Test]
        public void ThumbDis_EA4274F1()
        {
            Given_Instructions(0xEA42, 0x74F1);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EF677565(AdvancedSimd3RegistersSameLength_opc5) has not been implemented yet.
        [Test]
        public void ThumbDis_EF677565()
        {
            Given_Instructions(0xEF67, 0x7565);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction E870FEAE(Load/store dual (post-indexed)) has not been implemented yet.
        [Test]
        public void ThumbDis_E870FEAE()
        {
            Given_Instructions(0xE870, 0xFEAE);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EF3868FA(Unknown format specifier* in * when decoding vtst) has not been implemented yet.
        [Test]
        public void ThumbDis_EF3868FA()
        {
            Given_Instructions(0xEF38, 0x68FA);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F93FAD4D(Unknown format specifier* in * literal when decoding ldrsh) has not been implemented yet.
        [Test]
        public void ThumbDis_F93FAD4D()
        {
            Given_Instructions(0xF93F, 0xAD4D);
            Expect_Code("@@@");
        }

        [Test]
        public void ThumbDis_rfedb()
        {
            Given_Instructions(0xE812, 0xB51D);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction EAB1DC22(DataProcessingShiftedRegister_opc5) has not been implemented yet.
        [Test]
        public void ThumbDis_EAB1DC22()
        {
            Given_Instructions(0xEAB1, 0xDC22);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F90F24FB(AdvancedSimdLdStMultipleStructures) has not been implemented yet.
        [Test]
        public void ThumbDis_F90F24FB()
        {
            Given_Instructions(0xF90F, 0x24FB);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction F362A792(B - T3 variant) has not been implemented yet.
        [Test]
        public void ThumbDis_F362A792()
        {
            Given_Instructions(0xF362, 0xA792);
            Expect_Code("@@@");
        }

        // A T32 decoder for the instruction FFA4BE38(AdvancedSimdTwoRegistersAndShiftAmount_opcE) has not been implemented yet.
        [Test]
        public void ThumbDis_FFA4BE38()
        {
            Given_Instructions(0xFFA4, 0xBE38);
            Expect_Code("@@@");
        }
    }
}
