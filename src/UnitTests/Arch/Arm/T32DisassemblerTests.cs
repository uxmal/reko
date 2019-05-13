#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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
using Reko.Core;
using Reko.Core.Machine;
using System.Collections.Generic;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    [Category(Categories.Capstone)]
    public class T32DisassemblerTests : ArmTestBase
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
            Assert.AreEqual("bl\t$00100410", instr.ToString());
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
            Expect_Code("ldrb\tr4,[r5,#&45]");
        }

        [Test]
        public void ThumbDis_b_T2_variant()
        {
            Given_Instructions(0xE005);
            Expect_Code("b\t$0010000E");
        }

        [Test]
        public void ThumbDis_b_T2_variant_backward()
        {
            Given_Instructions(0xE7FD);
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
            Expect_Code("b\t$001084C8");
        }

        [Test]
        public void ThumbDis_b_T4_variant_negative()
        {
            Given_Instructions(0xF7FF, 0xBFFD);
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

        [Test]
        public void ThumbDis_vrshl()
        {
            Given_Instructions(0xFF7F, 0xA52D);
            Expect_Code("vrshl\td26,d15,d29");
        }

        [Test]
        [Ignore("Too Complex :)")]
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

        [Test]
        public void ThumbDis_ldrd2()
        {
            Given_Instructions(0xE96F, 0x7994);
            Expect_Code("ldrd\tr7,r9,[pc,#&250]");
        }

        [Test]
        public void ThumbDis_vmla()
        {
            Given_Instructions(0xEF24, 0xE9EB);
            Expect_Code("vmla.i32\tq7,q10,q13");
        }

        [Test]
        [Ignore("FFS")]
        public void ThumbDis_EB1BE4CF()
        {
            Given_Instructions(0xEB1B, 0xE4CF);
            Expect_Code("@@@");
        }

        [Test]
        public void ThumbDis_orn_lsr()
        {
            Given_Instructions(0xEA63, 0x91D5);
            Expect_Code("orn\tr1,r3,r5,lsr #7");
        }

        [Test]
        public void ThumbDis_ldrex()
        {
            Given_Instructions(0xE856, 0xE6BB);
            Expect_Code("ldrex\tlr,[r6,#&2EC]");
        }

        [Test]
        public void ThumbDis_vcge()
        {
            Given_Instructions(0xFF1F, 0x2E45);
            Expect_Code("vcge.f16\tq1,q7,q2");
        }

        // A T32 decoder for the instruction EFE4778A(Unknown format specifier* in * integer when decoding vabdl) has not been implemented yet.
        [Test]
        public void ThumbDis_vabdl()
        {
            Given_Instructions(0xEFE4, 0x778A);
            Expect_Code("vabdl.i32\tq11,d20,d10");
        }

        [Test]
        public void ThumbDis_orr_asr()
        {
            Given_Instructions(0xEA47, 0xA767);
            Expect_Code("orr\tr7,r7,r7,asr #9");
        }

        [Test]
        public void ThumbDis_strex()
        {
            Given_Instructions(0xE843, 0x5560);
            Expect_Code("strex\tr5,r5,[r3,#&180]");
        }

        [Test]
        public void ThumbDis_b_T3_()
        {
            Given_Instructions(0xF262, 0x8DDD);
            Expect_Code("b\t$001A2BBE");
        }

        [Test]
        public void ThumbDis_b_T3()
        {
            Given_Instructions(0xF008, 0xBA62);
            Expect_Code("b\t$001084C8");
        }

        [Test]
        public void ThumbDis_asrs()
        {
            Given_Instructions(0x4125);
            Expect_Code("asrs\tr5,r4");
        }

        [Test]
        public void ThumbDis_rfeia()
        {
            Given_Instructions(0xE9BE, 0x94D3);
            Expect_Code("rfeia\tlr");
        }

        // A T32 decoder for the instruction F7811103(usatLslVariant) has not been implemented yet.
        [Test]
        [Ignore("Complex")]
        public void ThumbDis_F7811103()
        {
            Given_Instructions(0xF781, 0x1103);
            Expect_Code("@@@");
        }

        [Test]
        public void ThumbDis_stlexh()
        {
            Given_Instructions(0xE8CB, 0xA8D7);
            Expect_Code("stlexh\tr7,r10,[fp]");
        }

        [Test]
        public void ThumbDis_b()
        {
            Given_Instructions(0xF768, 0x8AE5);
            Expect_Code("b\t$000A85CE");
        }

        [Test]
        public void ThumbDis_bhs_T1()
        {
            Given_Instructions(0xD20A);
            Expect_Code("bhs\t$00100018");
        }

        [Test]
        public void ThumbDis_orr_ror()
        {
            Given_Instructions(0xEA42, 0x74F1);
            Expect_Code("orr\tr4,r2,r1,ror #&1F");
        }

        [Test]
        public void ThumbDis_vrshl_i32()
        {
            Given_Instructions(0xEF67, 0x7565);
            Expect_Code("vrshl.i32\tq11,q3,q10");
        }

        [Test]
        public void ThumbDis_ldrd_post()
        {
            Given_Instructions(0xE870, 0xFEAE);
            Expect_Code("ldrd\tpc,lr,[r0],-#&2B8");
        }

        [Test]
        public void ThumbDis_ldrsh_pc()
        {
            Given_Instructions(0xF93F, 0xAD4D);
            Expect_Code("ldrsh\tr10,[pc,#&DD]");
        }

        [Test]
        public void ThumbDis_rfedb()
        {
            Given_Instructions(0xE812, 0xB51D);
            Expect_Code("rfedb\tr2");
        }

        [Test]
        [Ignore("Complex")]
        public void ThumbDis_F90F24FB()
        {
            Given_Instructions(0xF90F, 0x24FB);
            Expect_Code("@@@");
        }

        //////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ThumbDis_ldaex()
        {
            Given_Instructions(0xE8DC, 0x10E4);
            Expect_Code("ldaex\tr1,[ip]");
        }

        [Test]
        public void ThumbDis_vnmul_f32()
        {
            Given_Instructions(0xEE28, 0x4AED);
            Expect_Code("vnmul.f32\ts8,s17,s27");
        }

        [Test]
        public void ThumbDis_rsb_asr()
        {
            Given_Instructions(0xEBDF, 0xDCE3);
            Expect_Code("rsbs\tip,pc,r3,asr #&17");
        }

        [Test]
        public void ThumbDis_vcvt_f32_u32()
        {
            Given_Instructions(0xFFA4, 0xBE38);
            Expect_Code("vcvt.f32.u32\td11,d24,#&1C");
        }

        [Test]
        public void ThumbDis_vshl()
        {
            Given_Instructions(0xEFFC, 0x3533);
            Expect_Code("vshl.i32\td19,d19,#&1C");
        }

        [Test]
        public void ThumbDis_vqshl()
        {
            Given_Instructions(0xEF80, 0xE7FE);
            Expect_Code("vqshl.i64\tq7,q15,#&40");
        }

        [Test]
        public void ThumbDis_bic_w()
        {
            Given_Instructions(0xEA23, 0x0101);
            Expect_Code("bic.w\tr1,r3,r1");
        }

        [Test]
        public void ThumbDis_add_w()
        {
            Given_Instructions(0xEB05, 0x1588);
            Expect_Code("add.w\tr5,r5,r8,lsl #6");
        }

        [Test]
        public void ThumbDis_and_w()
        {
            Given_Instructions(0xEA00, 0x2013);
            Expect_Code("and.w\tr0,r0,r3,lsr #8");
        }

        [Test]
        public void ThumbDis_ldrb_w()
        {
            Given_Instructions(0xF815, 0x0C01);
            Expect_Code("ldrb.w\tr0,[r5,-#&1]");
        }

        [Test]
        public void ThumbDis_str_w()
        {
            Given_Instructions(0xF840, 0x2021);
            Expect_Code("str.w\tr2,[r0,r1,lsl #2]");
        }

        [Test]
        public void ThumbDis_str_w_noshift()
        {
            Given_Instructions(0xF840, 0x2001);
            Expect_Code("str.w\tr2,[r0,r1]");
        }

        [Test]
        public void ThumbDis_ldr_w()
        {
            Given_Instructions(0xF850, 0x0021);
            Expect_Code("ldr.w\tr0,[r0,r1,lsl #2]");
        }

        [Test]
        public void ThumbDis_bics_w()
        {
            Given_Instructions(0xEA36, 0x0304);
            Expect_Code("bics.w\tr3,r6,r4");
        }

        [Test]
        public void ThumbDis_mov_w()
        {
            Given_Instructions(0xEA4F, 0x7AD2);
            Expect_Code("mov.w\tr10,r2,lsr #&1F");
        }

        [Test]
        public void ThumbDis_nop_w()
        {
            Given_Instructions(0xF3AF, 0x8000);
            Expect_Code("nop.w");
        }

        [Test]
        public void ThumbDis_push_1()
        {
            Given_Instructions(0xB570);
            Expect_Code("push\t{r4-r6,lr}");
        }

        [Test]
        public void ThumbDis_pop_1()
        {
            Given_Instructions(0xBD08);
            Expect_Code("pop\t{r3,pc}");
        }

        [Test]
        public void ThumbDis_nop()
        {
            Given_Instructions(0xBF00);
            Expect_Code("nop");
        }

        [Test]
        public void ThumbDis_bics_short()
        {
            Given_Instructions(0x439E);
            Expect_Code("bics\tr6,r3");
        }

        [Test]
        public void ThumbDis_mvns()
        {
            Given_Instructions(0x43DA);
            Expect_Code("mvns\tr2,r3");
        }

        [Test]
        public void ThumbDis_cbnz()
        {
            Given_Instructions(0xB92D);
            Expect_Code("cbnz\tr5,$0010000E");
        }

        [Test]
        public void ThumbDis_bne_backward()
        {
            Given_Instructions(0xD1FE);
            Expect_Code("bne\t$00100000");
        }

        [Test]
        public void ThumbDis_vorr_imm()
        {
            Given_Instructions(0xFFC4, 0x4B10);
            Expect_Code("vorr.i16\td20,#&C000C000C000C000");
        }

        [Test]
        public void ThumbDis_smmul()
        {
            Given_Instructions(0xFB5A, 0xF104);
            Expect_Code("smmul\tr1,r10,r4");
        }

        [Test]
        public void ThumbDis_smmul_invalid()
        {
            Given_Instructions(0xFB5F, 0xF104);
            Expect_Code("Invalid");
        }

        [Test]
        public void ThumbDis_ldrb_regression()
        {
            Given_Instructions(0xF895, 0x4045);
            Expect_Code("ldrb\tr4,[r5,#&45]");
        }

        [Test]
        public void ThumbDis_ldr_regression2()
        {
            Given_Instructions(0x6A6B);
            Expect_Code("ldr\tr3,[r5,#&24]");
        }

        //.data:00000016 ED04 E000  stc  0, cr14, [r4, #-0]
        //.data:0000001a ED24 E000  stc	0, cr14, [r4, #-0]
        //.data:0000001e ED9C E000  ldc	0, cr14, [r12]
        //.data:00000022 F0F3 4770  ; <UNDEFINED> instruction: 0xf0f34770
        //.data:00000026 F956 4C0F  ldr??.w r4, [r6, #-15]
        //.data:0000002a FDB2 2501  ldc2 5, cr2, [r2, #4]!
        //.data:0000002e FDE1 4631  stc2l	6, cr4, [r1, #196]!	; 0xc4
        //.data:00000032 FE04 E7E6  cdp2	7, 0, cr14, cr4, cr6, {7}
        //.data:00000036 FE2D 4604  cdp2	6, 2, cr4, cr13, cr4, {0}
        //.data:0000003a FE3B 2501  cdp2	5, 3, cr2, cr11, cr1, {0}
        //.data:0000003e FE6F E7FC  mcr2	7, 3, lr, cr15, cr12, {7}
    }
}
