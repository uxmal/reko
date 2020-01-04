#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
            Expect_Code("mrc\tp15,#0,r3,cr13,cr0,#2");
        }

        [Test]
        public void ThumbDis_mcr()
        {
            Given_Instructions(0xEE01, 0x3F10);
            Expect_Code("mcr\tp15,#0,r3,cr1,cr0,#0");
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
            Expect_Code("vmlal.u8\tq9,d11,d0");
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
            Expect_Code("vhadd.s8\td27,d1,d2");
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
            Expect_Code("stc\tp14,cr5,[r8,#&F4]");
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
            Expect_Code("vrshl.u64\td26,d29,d15");
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

        [Test]
        public void ThumbDis_vabdl()
        {
            Given_Instructions(0xEFE4, 0x778A);
            Expect_Code("vabdl.s32\tq11,d20,d10");
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
        public void ThumbDis_vrshl_s32()
        {
            Given_Instructions(0xEF67, 0x7565);
            Expect_Code("vrshl.s32\tq11,q10,q3");
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
            Given_Instructions(0xEF81, 0xE7FE);
            Expect_Code("vqshl.i64\tq7,q15,#1");
        }

        [Test]
        public void ThumbDis_vqshl_s32()
        {
            Given_Instructions(0xEfff, 0x4750);
            Expect_Code("vqshl.i32\tq10,q0,#&1F");
        }

        [Test]
        public void ThumbDis_vqshlu()
        {
            Given_Instructions(0xffff, 0x4650);
            Expect_Code("vqshlu.u32\tq10,q0,#&1F");
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

        [Test]
        public void ThumbDis_strh_unsigned()
        {
            Given_Instructions(0xF82A, 0xE009);
            Expect_Code("strh\tlr,[r10,r9]");
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


        [Test]
        public void ThumbDis_ands_w()
        {
            Given_Instructions(0xEA10, 0x0301);
            Expect_Code("ands.w\tr3,r0,r1");
        }

        [Test]
        public void ThumbDis_movs_w()
        {
            Given_Instructions(0xEA5F, 0x0B5B);
            Expect_Code("movs.w\tfp,fp,lsr #1");
        }

        [Test]
        public void ThumbDis_eor_w()
        {
            Given_Instructions(0xEA80, 0x000E);
            Expect_Code("eor.w\tr0,r0,lr");
        }

        [Test]
        public void ThumbDis_teqs_w()
        {
            Given_Instructions(0xEA9C, 0x0F00);
            Expect_Code("teqs.w\tip,r0");
        }

        [Test]
        public void ThumbDis_adds_w()
        {
            Given_Instructions(0xEB10, 0x0802);
            Expect_Code("adds.w\tr8,r0,r2");
        }

        [Test]
        public void ThumbDis_adc_w()
        {
            Given_Instructions(0xEB41, 0x0603);
            Expect_Code("adc.w\tr6,r1,r3");
        }

        [Test]
        public void ThumbDis_adcs_rxx()
        {
            Given_Instructions(0xEB54, 0x0032);
            Expect_Code("adcs.w\tr0,r4,r2,rrx");
        }

        [Test]
        public void ThumbDis_subs_w()
        {
            Given_Instructions(0xEBB6, 0x080A);
            Expect_Code("subs.w\tr8,r6,r10");
        }

        [Test]
        public void ThumbDis_sub_w()
        {
            Given_Instructions(0xEBA9, 0x0908);
            Expect_Code("sub.w\tr9,r9,r8");
        }

        [Test]
        public void ThumbDis_cmp_w()
        {
            Given_Instructions(0xEBB1, 0x1FA2);
            Expect_Code("cmp.w\tr1,r2,asr #6");
        }

        [Test]
        public void ThumbDis_ldrsb_w()
        {
            Given_Instructions(0xF91C, 0x3002);
            Expect_Code("ldrsb.w\tr3,[ip,r2]");
        }

        [Test]
        public void ThumbDis_rev16()
        {
            Given_Instructions(0xFA99, 0xF299);
            Expect_Code("rev16.w\tr2,r9");
        }

        [Test]
        public void ThumbDis_vldr_literal()
        {
            Given_Instructions(0xED9F, 0x0B08);
            Expect_Code("vldr\td0,[pc,#&20]");
        }

        [Test]
        public void ThumbDis_vsub_i64()
        {
            Given_Instructions(0xFF39, 0x6828);
            Expect_Code("vsub.i64\td6,d9,d24");
        }


        [Test]
        public void ThumbDis_ldrexd()
        {
            Given_Instructions(0xE8D1, 0x4573);
            Expect_Code("ldrexd\tr4,r5,[r1]");
        }

        [Test]
        public void ThumbDis_strexd()
        {
            Given_Instructions(0xE8C3, 0x347E);
            Expect_Code("strexd\tlr,r3,r4,[r3]");
        }

        [Test]
        public void ThumbDis_vmov_r_s()
        {
            Given_Instructions(0xEE11, 0xAA10);
            Expect_Code("vmov\tr10,s2");
        }

        [Test]
        public void ThumbDis_vmov_s_r()
        {
            Given_Instructions(0xEE04, 0x1A10);
            Expect_Code("vmov\ts8,r1");
        }


        [Test]
        public void ThumbDis_vmov_f32()
        {
            Given_Instructions(0xeeb0, 0x0a41);
            Expect_Code("vmov.f32\ts0,s2");
        }


        [Test]
        public void ThumbDis_stlb()
        {
            Given_Instructions(0xE8C0, 0x0088);
            Expect_Code("stlb\tr0,[r0]");
        }

        [Test]
        public void ThumbDis_vmla_f32()
        {
            Given_Instructions(0xEE01, 0x0A02);
            Expect_Code("vmla.f32\ts0,s2,s4");
        }

        [Test]
        public void ThumbDis_vld1_multi()
        {
            Given_Instructions(0xF92D, 0xF208);
            Expect_Code("vld1.i8\t{d15-d18},[sp],r8");
        }

        [Test]
        public void ThumbDis_stlh()
        {
            Given_Instructions(0xE8C8, 0x0094);
            Expect_Code("stlh\tr0,[r8]");
        }

        [Test]
        public void ThumbDis_ldah()
        {
            Given_Instructions(0xE8D2, 0x0093);
            Expect_Code("ldah\tr3,r0,[r2]");
        }

        [Test]
        public void ThumbDis_ldab()
        {
            Given_Instructions(0xE8DC, 0x0081);
            Expect_Code("ldab\tr1,r0,[ip]");
        }

        [Test]
        public void ThumbDis_vmls_i16()
        {
            Given_Instructions(0xFF1C, 0x6924);
            Expect_Code("vmls.u16\td6,d12,d20");
        }

        [Test]
        public void ThumbDis_vmlsl_scalar()
        {
            Given_Instructions(0xFFDB, 0x4668);
            Expect_Code("vmlsl.u16\tq10,d11,d0[3]");
        }

        [Test]
        public void ThumbDis_vmov_from_gp_pair()
        {
            Given_Instructions(0xEC46, 0x5B18);
            Expect_Code("vmov\td8,r5,r6");
        }

        [Test]
        public void ThumbDis_vmul_f64()
        {
            Given_Instructions(0xee21, 0x1b08);
            Expect_Code("vmul.f64\td1,d1,d8");
        }

        [Test]
        public void ThumbDis_vadd_f32()
        {
            Given_Instructions(0xee30, 0x0a01);
            Expect_Code("vadd.f32\ts0,s0,s2");
        }

        [Test]
        public void ThumbDis_vdiv_f32()
        {
            Given_Instructions(0xee80, 0x0a05);
            Expect_Code("vdiv.f32\ts0,s0,s10");
        }

        [Test]
        public void ThumbDis_vdiv_f64()
        {
            Given_Instructions(0xee80, 0x0b01);
            Expect_Code("vdiv.f64\td0,d0,d1");
        }

        [Test]
        public void ThumbDis_tst_w()
        {
            Given_Instructions(0xea10, 0x0f0a);
            Expect_Code("tst.w\tr0,r10");
        }

        [Test]
        public void ThumbDis_orns()
        {
            Given_Instructions(0xea7a, 0x0092);
            Expect_Code("orns\tr0,r10,r2,lsr #2");
        }

        [Test]
        public void ThumbDis_mvns_w()
        {
            Given_Instructions(0xea7f, 0x5c65);
            Expect_Code("mvns.w\tip,r5,asr #&15");
        }

        [Test]
        public void ThumbDis_pkhbt()
        {
            Given_Instructions(0xeac0, 0x0087);
            Expect_Code("pkhbt\tr0,r0,r7,lsl #2");
        }

        [Test]
        public void ThumbDis_pkhbt_2()
        {
            Given_Instructions(0xEAC6, 0x2E00);
            Expect_Code("pkhbt\tlr,r6,r0,lsl #8");
        }

        [Test]
        public void ThumbDis_mrrc()
        {
            Given_Instructions(0xec58, 0xbf00);
            Expect_Code("mrrc\tp15,#0,fp,r8,cr0");
        }

        [Test]
        public void ThumbDis_vpop_d()
        {
            Given_Instructions(0xecbd, 0x1b12);
            Expect_Code("vldmia\tsp!,{d1-d9}");
        }

        [Test]
        public void ThumbDis_vmov_f64_imm()
        {
            Given_Instructions(0xeeb0, 0x0b08);
            Expect_Code("vmov.f64\td0,#3.0");
        }

        [Test]
        public void ThumbDis_vmov_f32_imm()
        {
            Given_Instructions(0xeeb0, 0x2a00);
            Expect_Code("vmov.f32\ts4,#2.0F");
        }

        [Test]
        public void ThumbDis_vneg_f64()
        {
            Given_Instructions(0xeeb1, 0x8b48);
            Expect_Code("vneg.f64\td8,d8");
        }

        [Test]
        public void ThumbDis_vcmpe_f32()
        {
            Given_Instructions(0xeeb4, 0x0ac2);
            Expect_Code("vcmpe.f32\ts0,s4");
        }

        [Test]
        public void ThumbDis_vcmpe_f32_0()
        {
            Given_Instructions(0xeeb5, 0xdac0);
            Expect_Code("vcmpe.f32\ts26,#0.0F");
        }

        [Test]
        public void ThumbDis_vcvt_f64_f32()
        {
            Given_Instructions(0xeeb7, 0x0ac1);
            Expect_Code("vcvt.f64.f32\td0,s2");
        }

        [Test]
        public void ThumbDis_vcvt_f64_s32()
        {
            Given_Instructions(0xeeb8, 0x0bc0);
            Expect_Code("vcvt.f64.s32\td0,s0");
        }

        [Test]
        public void ThumbDis_vcvt_u32_f64()
        {
            Given_Instructions(0xeebc, 0x0bc2);
            Expect_Code("vcvt.u32.f64\ts0,d2");
        }

        [Test]
        public void ThumbDis_vmrs_cpsr()
        {
            Given_Instructions(0xeef1, 0xfa10);
            Expect_Code("vmrs\tcpsr,fpscr");
        }

        [Test]
        public void ThumbDis_vmax_f32()
        {
            Given_Instructions(0xef08, 0xbf00);
            Expect_Code("vmax.f32\td11,d8,d0");
        }

        [Test]
        public void ThumbDis_vrsrts_f32()
        {
            Given_Instructions(0xef6f, 0xffb6);
            Expect_Code("vrsqrts.f32\td31,d31,d22");
        }

        [Test]
        public void ThumbDis_vext_8()
        {
            Given_Instructions(0xefb4, 0x0286);
            Expect_Code("vext.i8\td0,d20,d6,#2");
        }

        [Test]
        public void ThumbDis_ssat_asr()
        {
            Given_Instructions(0xf321, 0x4648);
            Expect_Code("ssat\tr6,#8,r1,asr #&11");
        }

        [Test]
        public void ThumbDis_udf_w()
        {
            Given_Instructions(0xf7f1, 0xa801);
            Expect_Code("udf.w\t#&1801");
        }

        [Test]
        public void ThumbDis_vst2_sparse_writeback()
        {
            Given_Instructions(0xf903, 0x492d);
            Expect_Code("vst2.i8\t{d4,d6},[r3:128]");
        }

        [Test]
        public void ThumbDis_vst4_sparse_writeback()
        {
            Given_Instructions(0xf905, 0xf10d);
            Expect_Code("vst4.i8\t{d15,d17,d19,d21},[r5]");
        }



        [Test]
        public void ThumbDis_vst1_i8_single()
        {
            Given_Instructions(0xf9c3, 0xf00b);
            Expect_Code("vst1.i8\t{d31[0]},[r3],fp");
        }

        [Test]
        public void ThumbDis_vtbx_i8()
        {
            Given_Instructions(0xffb5, 0x4a63);
            Expect_Code("vtbx.i8\td4,{d5-d7},d19");
        }

        [Test]
        public void ThumbDis_vqshlu_i32()
        {
            Given_Instructions(0xffff, 0x4650);
            Expect_Code("vqshlu.u32\tq10,q0,#&1F");
        }

        [Test]
        public void ThumbDis_vqshrn_u64()
        {
            Given_Instructions(0xfff3, 0x491a);
            Expect_Code("vqshrn.u64\td20,q5,#&D");
        }

        [Test]
        public void ThumbDis_vcgt_s8()
        {
            Given_Instructions(0xfff1, 0xe004);
            Expect_Code("vcgt.s8\td30,d4,#0");
        }

        [Test]
        public void ThumbDis_vtrn_16()
        {
            Given_Instructions(0xffb6, 0x008e);
            Expect_Code("vtrn.i16\td0,d14");
        }

        [Test]
        public void ThumbDis_vsli_i64()
        {
            Given_Instructions(0xffb1, 0xb5b0);
            Expect_Code("vsli.i64\td11,d16,#&31");
        }

        [Test]
        public void ThumbDis_vrsra()
        {
            Given_Instructions(0xff97, 0xb3b0);
            Expect_Code("vrsra.u64\td11,d16,#&29");
        }

        [Test]
        public void ThumbDis_vcvtn_u32_f32()
        {
            Given_Instructions(0xfefd, 0xea4f);
            Expect_Code("vcvtn.u32.f32\ts29,s30");
        }

        [Test]
        public void ThumbDis_vorr_i16()
        {
            Given_Instructions(0xffc7, 0x491a);
            Expect_Code("vorr.i16\td20,#&FA00FA00FA00FA");
        }

        [Test]
        public void ThumbDis_vmov_i16()
        {
            Given_Instructions(0xffc7, 0x4a1f);
            Expect_Code("vmov.i16\td20,#&FF00FF00FF00FF00");
        }

        [Test]
        public void ThumbDis_ldc2l()
        {
            Given_Instructions(0xfd79, 0x9819);
            Expect_Code("ldc2l\tp8,cr9,[r9,-#&64]!");
        }

        [Test]
        public void ThumbDis_vaddw_u32()
        {
            Given_Instructions(0xFFA7, 0xF104);
            Expect_Code("vaddw.u32\tq7,q3,d4");
        }

        [Test]
        public void ThumbDis_vmov_d1_tworegs()
        {
            Given_Instructions(0xEC41, 0x0B11);
            Expect_Code("vmov\td1,r0,r1");
        }

        [Test]
        public void ThumbDis_vsri()
        {
            Given_Instructions(0xFFEC, 0x0431);
            Expect_Code("vsri.i32\td16,d17,#&14");
        }

#if BORED

        [Test]
        public void ThumbDis_vld4_single()
        {
            Given_Instructions(0xf9ad, 0x9805);
            Expect_Code("vld4.i32\t{d9[0]},[sp],r5");
        }

        [Test]
        public void ThumbDis_vld3_single()
        {
            Given_Instructions(0xf9ab, 0x4628);
            Expect_Code("vld3.i16\t{d4[0],d6[0],d8[0]},[fp],r8");
        }


        [Test]
        public void ThumbDis_vld3_i16_single()
        {
            Given_Instructions(0xf9ef, 0x4620);
            Expect_Code("vld3.i16\t{d20[0],d22[0],d24[0]},[pc],r0");
        }

        [Test]
        public void ThumbDis_vst3_i32_single()
        {
            Given_Instructions(0xf9c3, 0x9a02);
            Expect_Code("vst3.i32\t{d25[0],d26[0],d27[0]},[r3],r2");
        }

                [Test]
        public void ThumbDis_vst3_single_sparse()
        {
            Given_Instructions(0xf98f, 0x4668);
            Expect_Code("vst3.i16\t{d4[1],d6[1],d8[1]},[pc],r8");
        }
#endif
    }
}
