#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Memory;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    [Category(Categories.UnitTests)]
    public class T32DisassemblerTests : ArmTestBase
    {
        private IEnumerator<MachineInstruction> dasm;

        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new ThumbArchitecture(new ServiceContainer(), "arm-thumb", new Dictionary<string, object>());
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
            var mem = new ByteMemoryArea(Address.Ptr32(0x00100000), w.ToArray());
            var arch = CreateArchitecture();
            this.dasm = CreateDisassembler(arch, mem.CreateLeReader(0));
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

        private void Expect_Code(string sExp, InstrClass iclassExp)
        {
            Assert.IsTrue(dasm.MoveNext());
            var instr = dasm.Current;
            Assert.AreEqual(sExp, instr.ToString());
            Assert.AreEqual(iclassExp, instr.InstructionClass);
        }

        protected override IEnumerator<MachineInstruction> CreateDisassembler(IProcessorArchitecture arch, EndianImageReader rdr)
        {
            return arch.CreateDisassembler(rdr).GetEnumerator();
        }

        // Handle the case when only 2 bytes are available but they are the first 
        // 2 bytes of a 4 byte instruction.
        [Test]
        public void ThumbDis_Incomplete_instr()
        {
            var instr = Disassemble16(0xE895);
            Assert.AreEqual("Invalid", instr.ToString());
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
        public void ThumbDis_sha1m()
        {
            //$REVIEW: encodingmaybe wrong
            AssertCode("sha1m.i32\tq12,q11,q13", "67EFEB9C");
        }

        [Test]
        public void ThumbDis_sha256h2()
        {
            AssertCode("sha256h2.i32\tq2,q6,q7", "1DFF4F4C");
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
        public void ThumbDis_ldab()
        {
            Given_Instructions(0xE8DC, 0x0081);
            Expect_Code("ldab\tr0,[ip]");
        }

        [Test]
        public void ThumbDis_ldah()
        {
            Given_Instructions(0xE8D2, 0x0093);
            Expect_Code("ldah\tr0,[r2]");
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
        public void ThumbDis_it_invalid_branch()
        {
            Given_Instructions(0xBF18, 0x4630, 0xD1FE);
            Expect_Code("it\tne");
            Expect_Code("movne\tr0,r6");
            Expect_Code("Invalid");
        }

        [Test]
        public void ThumbDis_it_invalid_jump()
        {
            Given_Instructions(0xBF18, 0xF1FE, 0x4242, 0x4630);
            Expect_Code("it\tne");
            Expect_Code("Invalid");
            Expect_Code("mov\tr0,r6");
        }

        [Test]
        public void ThumbDis_it_valid_jump()
        {
            Given_Instructions(0xBF18, 0x4630, 0xF1FE);
            Expect_Code("it\tne");
            Expect_Code("movne\tr0,r6");
            Expect_Code("Invalid");
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
        public void ThumbDis_qsub()
        {
            AssertCode("qsub\tr1,r6,r5", "85FAA621");
        }

        [Test]
        public void ThumbDis_rbit()
        {
            AssertCode("rbit\tr4,r2", "92FAA2F4");
        }

        [Test]
        public void ThumbDis_rbit_invalid()
        {
            AssertCode("Invalid", "92FAAF34");
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
        public void ThumbDis_mrs_2()
        {
            AssertCode("mrs\tr2,r9_usr", "E1F3E5A2");
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
        public void ThumbDis_srsdb()
        {
            Given_Instructions(0xE807, 0x01C9);
            Expect_Code("srsdb\tsp,#9");
        }

        [Test]
        public void ThumbDis_stc()
        {
            Given_Instructions(0xED88, 0x5E3D); // stc p14, c6, [r8, #0xf4]
            Expect_Code("stc\tp14,cr5,[r8,#&F4]");
        }

        [Test]
        public void ThumbDis_vmla_float()
        {
            AssertCode("vmla.f32\td4,d16,d21", "A0EF e541");
        }

        [Test]
        public void ThumbDis_vrev16()
        {
            AssertCode("vrev16.u8\td27,d8", "F0FF08B1");
        }

        [Test]
        public void ThumbDis_vrintp()
        {
            AssertCode("vrintp.f32\td23,d27", "FAFFAB77");
        }

        [Test]
        public void ThumbDis_vrintx()
        {
            AssertCode("vrintx.f32\ts6,s17", "B7EE683A");
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
        public void ThumbDis_orr_asr()
        {
            Given_Instructions(0xEA47, 0xA767);
            Expect_Code("orr\tr7,r7,r7,asr #9");
        }

        [Test]
        public void ThumbDis_vabdl()
        {
            Given_Instructions(0xEFE4, 0x778A);
            Expect_Code("vabdl.s32\tq11,d20,d10");
        }

        [Test]
        public void ThumbDis_vacge()
        {
            AssertCode("vacge.i32\tq8,q4,q13", "48FF7A0E");
        }

        [Test]
        public void ThumbDis_vbsl()
        {
            AssertCode("vbsl\td8,d12,d26", "1CFF3A81");
        }

        [Test]
        public void ThumbDis_vceq_register()
        {
            AssertCode("vceq.f16\td8,d15,d0", "1FEF008E");
        }

        [Test]
        public void ThumbDis_vcge_float()
        {
            Given_Instructions(0xFF1F, 0x2E45);
            Expect_Code("vcge.f16\tq1,q7,q2");
        }

        [Test]
        public void ThumbDis_vcgt_s8_d()
        {
            AssertCode("vcgt.s8\td30,d21,d18", "45EF A2E3");
        }

        [Test]
        public void ThumbDis_vcgt_invalid()
        {
            AssertCode("Invalid", "45EF E2E3");
        }

        [Test]
        public void ThumbDis_vcgt_s8_q()
        {
            AssertCode("vcgt.s8\tq15,q10,q9", "44EF E2E3");
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
        public void ThumbDis_usat()
        {
            Given_Instructions(0xF781, 0x1103);
            Expect_Code("usat\tr1,#4,r1,lsl #4");
        }

        [Test]
        public void ThumbDis_stlexh()
        {
            Given_Instructions(0xE8CB, 0xA8D7);
            Expect_Code("stlexh\tr10,r7,[fp]");
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
        public void ThumbDis_ldrsh_w_pre()
        {
            AssertCode("ldrsh.w\tr1,[r6,-#&2F]!", "36F92F1D");
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
        public void ThumbDis_vqdmlal()
        {
            AssertCode("vqdmlal.i32\tq5,d26,d1", "AAEF81A9");
        }

        [Test]
        public void ThumbDis_vqdmlsl_scalar()
        {
            AssertCode("vqdmlsl.s32\tq4,d26,d3[0]", "AAEFC387");
        }

        [Test]
        public void ThumbDis_vqdmulh()
        {
            AssertCode("vqdmulh.i32\tq7,q3,q12", "26EF68EB");
        }

        [Test]
        public void ThumbDis_vqdmulh_s16()
        {
            AssertCode("vqdmulh.s16\tq7,q5,d2[3]", "9AFF6AEC");
        }

        [Test]
        public void ThumbDis_vqdmull()
        {
            AssertCode("vqdmull.i32\tq1,d28,d25", "ACEFA92D");
        }

        [Test]
        public void ThumbDis_vqrdmlah()
        {
            AssertCode("vqrdmlah.i32\td30,d24,d2", "68FF92EB");
        }

        [Test]
        public void ThumbDis_vqrdmlsh()
        {
            AssertCode("vqrdmlsh.i32\td10,d28,d6", "2CFF96AC");
        }

        [Test]
        public void ThumbDis_vqrdmulh()
        {
            AssertCode("vqrdmulh.i32\tq9,q13,q9", "6AFFE22B");
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
        public void ThumbDis_vqsub()
        {
            AssertCode("vqsub.s32\td23,d25,d15", "69EF9F72");
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
        public void ThumbDis_bic_w()
        {
            Given_Instructions(0xEA23, 0x0101);
            Expect_Code("bic.w\tr1,r3,r1");
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
        public void ThumbDis_pli()
        {
            //$REVIEW: is this correct?
            AssertCode("pli\t[sp],-#&1BA", "9DF9BAF1");
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
            Expect_Code("nop", InstrClass.Linear | InstrClass.Padding);
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
        public void ThumbDis_bne_backward()
        {
            Given_Instructions(0xD1FE);
            Expect_Code("bne\t$00100000");
        }


        [Test]
        public void ThumbDis_cbnz()
        {
            Given_Instructions(0xB92D);
            Expect_Code("cbnz\tr5,$0010000E");
        }

        [Test]
        public void ThumbDis_clrex()
        {
            //  F3  BF 8F 2F

            AssertCode("clrex", "BFF3 2F8F");
        }

        [Test]
        public void ThumbDis_smc()
        {
            AssertCode("smc\t#&E", "FEF72E8D");
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
        //.data:00000022 F0F3 4770  ; <UNDEFINED> instruction: 0xf0f34770<32>
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
        [Ignore("Not ready")]
        public void ThumbDis_fstmiax()
        {
            AssertCode("fstmiax\t{d3-d12}", "86EC153B");
        }

        [Test]
        public void ThumbDis_teqs_w()
        {
            Given_Instructions(0xEA9C, 0x0F00);
            Expect_Code("teqs.w\tip,r0");
        }

        [Test]
        public void ThumbDis_tst()
        {
            Given_Instructions(0x420C);
            Expect_Code("tst\tr4,r1");
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
        public void ThumbDis_cmn_w()
        {
            AssertCode("cmn.w\tpc,lr,ip,ror #&13", "1EEBFCCF");
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
        public void ThumbDis_revsh()
        {
            AssertCode("revsh\tr10,r9", "91FAB9CA");
        }

        [Test]
        public void ThumbDis_vldr_literal()
        {
            Given_Instructions(0xED9F, 0x0B08);
            Expect_Code("vldr\td0,[pc,#&20]");
        }

        [Test]
        public void ThumbDis_vorr_imm()
        {
            Given_Instructions(0xFFC4, 0x4B10);
            Expect_Code("vorr.i16\td20,#&C000C000C000C000");
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
        public void ThumbDis_vmov_imm()
        {
            AssertCode("vmov.i32\td4,#&D9FFFF00D9FFFF", "85FF194D");
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
        public void ThumbDis_vmov_gpr_to_scalar()
        {
            AssertCode("vmov.i8\td31[7],r2", "6FEEF12B");
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
        public void ThumbDis_tst_w()
        {
            Given_Instructions(0xea10, 0x0f0a);
            Expect_Code("tst.w\tr0,r10");
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
        public void ThumbDis_vpadd()
        {
            AssertCode("vpadd.i32\td9,d21,d2", "25EF929B");
        }

        [Test]
        public void ThumbDis_vpadal()
        {
            AssertCode("vpadal.s8\td20,d5", "F0FF0546");
        }

        [Test]
        public void ThumbDis_vpop_d()
        {
            Given_Instructions(0xecbd, 0x1b12);
            Expect_Code("vldmia\tsp!,{d1-d9}");
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
        public void ThumbDis_vdup()
        {
            AssertCode("vdup.i16\tq15,ip", "AEEEBAC9");
        }

        [Test]
        public void ThumbDis_vld1_single()
        {
            Given_Instructions(0xf9ad, 0x9805);
            Expect_Code("vld1.i32\t{d9[0]},[sp],r5");
        }

        [Test]
        public void ThumbDis_vld3_single()
        {
            Given_Instructions(0xF9AB, 0x4628);
            Expect_Code("vld3.i16\t{d4[0],d6[0],d8[0]},[fp],r8");
        }

        [Test]
        public void ThumbDis_vld3_i16_single()
        {
            Given_Instructions(0xf9ef, 0x4620);
            Expect_Code("vld3.i16\t{d20[0],d22[0],d24[0]},[pc],r0");
        }

        [Test]
        public void ThumbDis_vmls_i16()
        {
            Given_Instructions(0xFF1C, 0x6924);
            Expect_Code("vmls.u16\td6,d12,d20");
        }

        [Test]
        public void ThumbDis_vmls_scalar()
        {
            AssertCode("vmls.i16\tq6,q3,d6[2]", "96FF66C4");
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
        public void ThumbDis_vmul_i32()
        {
            AssertCode("vmul.i32\td1,d0,d2", "20EF1219");
        }

        [Test]
        public void ThumbDis_vmull()
        {
            AssertCode("vmull.s16\tq14,d10,d1[0]", "DAEF41CA");
        }

        [Test]
        public void ThumbDis_vorr()
        {
            AssertCode("vorr\td21,d11,d1", "6BEF1151");
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
        public void ThumbDis_vst1_i8_single()
        {
            Given_Instructions(0xf9c3, 0xf00b);
            Expect_Code("vst1.i8\t{d31[0]},[r3],fp");
        }

        [Test]
        public void ThumbDis_vst2_sparse_writeback()
        {
            Given_Instructions(0xf903, 0x492d);
            Expect_Code("vst2.i8\t{d4,d6},[r3:128]");
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

        [Test]
        [Ignore("Aligned not working")]
        public void ThumbDis_vst4_aligned()
        {
            AssertCode("vst4.i8\t{d18[3],d20[3],d22[3],d24[3]},[r2:32]", "C2F97B23");
        }

        [Test]
        public void ThumbDis_vst4_sparse_writeback()
        {
            Given_Instructions(0xf905, 0xf10d);
            Expect_Code("vst4.i8\t{d15,d17,d19,d21},[r5]");
        }

        [Test]
        [Ignore("New decoder needed?")]
        public void ThumbDis_vstmia()
        {
            AssertCode("@@@", "8AECCA1B");
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

        // This file contains unit tests automatically generated by Reko decompiler.
        // Please copy the contents of this file and report it on GitHub, using the 
        // following URL: https://github.com/uxmal/reko/issues


 
        // Reko: a decoder for the instruction 12EF8423 at address 00182102 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_12EF8423()
        {
            AssertCode("@@@", "12EF8423");
        }
        // Reko: a decoder for the instruction 50EF951B at address 00140474 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_50EF951B()
        {
            AssertCode("@@@", "50EF951B");
        }
        // Reko: a decoder for the instruction 05F97FB2 at address 0018230C has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F905B27F)
        [Test]
        public void ThumbDis_05F97FB2()
        {
            AssertCode("@@@", "05F97FB2");
        }
        // Reko: a decoder for the instruction E2F971A2 at address 00142222 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9E2A271)
        [Test]
        public void ThumbDis_E2F971A2()
        {
            AssertCode("@@@", "E2F971A2");
        }
        // Reko: a decoder for the instruction 7AFFE22B at address 001803E6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
 
        // Reko: a decoder for the instruction 90EF494B at address 001A2118 has not been implemented. (Unimplemented '*' when decoding EF904B49)
        [Test]
        public void ThumbDis_90EF494B()
        {
            AssertCode("@@@", "90EF494B");
        }
        // Reko: a decoder for the instruction 4EEF961D at address 001604B2 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_4EEF961D()
        {
            AssertCode("@@@", "4EEF961D");
        }
        // Reko: a decoder for the instruction 4DFCF6FC at address 001A2E48 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4DFCF6FC()
        {
            AssertCode("@@@", "4DFCF6FC");
        }
        // Reko: a decoder for the instruction A6EF696B at address 001E0144 has not been implemented. (Unimplemented '*' when decoding EFA66B69)
        [Test]
        public void ThumbDis_A6EF696B()
        {
            AssertCode("@@@", "A6EF696B");
        }
        // Reko: a decoder for the instruction D2FD11B8 at address 001E2126 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D2FD11B8()
        {
            AssertCode("@@@", "D2FD11B8");
        }
        // Reko: a decoder for the instruction E5FC1548 at address 0014404C has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E5FC1548()
        {
            AssertCode("@@@", "E5FC1548");
        }
        // Reko: a decoder for the instruction B1FCCF2C at address 001202E6 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B1FCCF2C()
        {
            AssertCode("@@@", "B1FCCF2C");
        }
        // Reko: a decoder for the instruction 3AFFC661 at address 001A0236 has not been implemented. (Unimplemented '*' when decoding FF3A61C6)
        [Test]
        public void ThumbDis_3AFFC661()
        {
            AssertCode("@@@", "3AFFC661");
        }
        // Reko: a decoder for the instruction 74F9DFCF at address 001443D8 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_74F9DFCF()
        {
            AssertCode("@@@", "74F9DFCF");
        }
        // Reko: a decoder for the instruction 1AFFCFEB at address 001023F4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_1AFFCFEB()
        {
            AssertCode("@@@", "1AFFCFEB");
        }
        // Reko: a decoder for the instruction F5FFB8C8 at address 001C0064 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_F5FFB8C8()
        {
            AssertCode("@@@", "F5FFB8C8");
        }
        // Reko: a decoder for the instruction B8FC94DD at address 00182448 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B8FC94DD()
        {
            AssertCode("@@@", "B8FC94DD");
        }
        // Reko: a decoder for the instruction F7FDF39C at address 001422FA has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F7FDF39C()
        {
            AssertCode("@@@", "F7FDF39C");
        }
        // Reko: a decoder for the instruction 82F91D1B at address 0012AC0E has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9821B1D)
        [Test]
        public void ThumbDis_82F91D1B()
        {
            AssertCode("@@@", "82F91D1B");
        }
        // Reko: a decoder for the instruction 69EF9F72 at address 001A2E88 has not been implemented. (Unimplemented '*' when decoding EF69729F)

        // Reko: a decoder for the instruction 56F9D17E at address 001E2706 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_56F9D17E()
        {
            AssertCode("@@@", "56F9D17E");
        }
        // Reko: a decoder for the instruction E0FC6439 at address 001A03D8 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E0FC6439()
        {
            AssertCode("@@@", "E0FC6439");
        }
        // Reko: a decoder for the instruction C4EF7A2B at address 00180C50 has not been implemented. (Unimplemented '*immediate - T2' when decoding EFC42B7A)
        [Test]
        public void ThumbDis_C4EF7A2B()
        {
            AssertCode("@@@", "C4EF7A2B");
        }
        // Reko: a decoder for the instruction 22FEA00D at address 001667C6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_22FEA00D()
        {
            AssertCode("@@@", "22FEA00D");
        }
        // Reko: a decoder for the instruction DFE8C90F at address 00120CE4 has not been implemented. (Unimplemented '*' when decoding E8DF0FC9)
        [Test]
        public void ThumbDis_DFE8C90F()
        {
            AssertCode("@@@", "DFE8C90F");
        }
        // Reko: a decoder for the instruction E5F3B5AE at address 00120CF0 has not been implemented. (Unimplemented '*banked register' when decoding F3E5AEB5)
        [Test]
        public void ThumbDis_E5F3B5AE()
        {
            AssertCode("@@@", "E5F3B5AE");
        }
        // Reko: a decoder for the instruction 69FE6A8D at address 001620D4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_69FE6A8D()
        {
            AssertCode("@@@", "69FE6A8D");
        }
        // Reko: a decoder for the instruction A1F98515 at address 001445C0 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9A11585)
        [Test]
        public void ThumbDis_A1F98515()
        {
            AssertCode("@@@", "A1F98515");
        }
        // Reko: a decoder for the instruction 0BFDC51D at address 001445F6 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0BFDC51D()
        {
            AssertCode("@@@", "0BFDC51D");
        }
        // Reko: a decoder for the instruction 9EFCF6CC at address 001C0146 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_9EFCF6CC()
        {
            AssertCode("@@@", "9EFCF6CC");
        }
        // Reko: a decoder for the instruction 94FFCF8C at address 00182A9A has not been implemented. (Unimplemented '*' when decoding FF948CCF)
        [Test]
        public void ThumbDis_94FFCF8C()
        {
            AssertCode("@@@", "94FFCF8C");
        }
        // Reko: a decoder for the instruction FCFDBE39 at address 00142348 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FCFDBE39()
        {
            AssertCode("@@@", "FCFDBE39");
        }
        // Reko: a decoder for the instruction D8FE3709 at address 0016C200 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D8FE3709()
        {
            AssertCode("@@@", "D8FE3709");
        }
        // Reko: a decoder for the instruction 55F921EE at address 001E2C04 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_55F921EE()
        {
            AssertCode("@@@", "55F921EE");
        }
        // Reko: a decoder for the instruction 17FFBACC at address 001086DC has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_17FFBACC()
        {
            AssertCode("@@@", "17FFBACC");
        }
        // Reko: a decoder for the instruction 49FF1D7B at address 001C23F2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_49FF1D7B()
        {
            AssertCode("@@@", "49FF1D7B");
        }
        // Reko: a decoder for the instruction D3EFB64B at address 001671E4 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_D3EFB64B()
        {
            AssertCode("@@@", "D3EFB64B");
        }
        // Reko: a decoder for the instruction E9FFDA7D at address 001672BA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_E9FFDA7D()
        {
            AssertCode("@@@", "E9FFDA7D");
        }
        // Reko: a decoder for the instruction AEFF46F5 at address 001E41CE has not been implemented. (Unimplemented '*scalar' when decoding FFAEF546)
        [Test]
        public void ThumbDis_AEFF46F5()
        {
            AssertCode("@@@", "AEFF46F5");
        }
        // Reko: a decoder for the instruction 96FF9D2C at address 00162108 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_96FF9D2C()
        {
            AssertCode("@@@", "96FF9D2C");
        }
        // Reko: a decoder for the instruction 8FFEEA9C at address 001031B6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8FFEEA9C()
        {
            AssertCode("@@@", "8FFEEA9C");
        }
        // Reko: a decoder for the instruction 7EFF494C at address 00103226 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_7EFF494C()
        {
            AssertCode("@@@", "7EFF494C");
        }
        // Reko: a decoder for the instruction ADFF6864 at address 00182CD2 has not been implemented. (Unimplemented '*scalar' when decoding FFAD6468)
        [Test]
        public void ThumbDis_ADFF6864()
        {
            AssertCode("@@@", "ADFF6864");
        }
        // Reko: a decoder for the instruction EEFFBC2B at address 0014106E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_EEFFBC2B()
        {
            AssertCode("@@@", "EEFFBC2B");
        }
        // Reko: a decoder for the instruction 7EFFD152 at address 001A32A2 has not been implemented. (Unimplemented '*' when decoding FF7E52D1)
        [Test]
        public void ThumbDis_7EFFD152()
        {
            AssertCode("@@@", "7EFFD152");
        }
        // Reko: a decoder for the instruction 84F9A337 at address 0012B214 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F98437A3)
        [Test]
        public void ThumbDis_84F9A337()
        {
            AssertCode("@@@", "84F9A337");
        }
        // Reko: a decoder for the instruction E2F90521 at address 001813AE has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9E22105)
        [Test]
        public void ThumbDis_E2F90521()
        {
            AssertCode("@@@", "E2F90521");
        }
        // Reko: a decoder for the instruction C9F9074B at address 0016733E has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C94B07)
        [Test]
        public void ThumbDis_C9F9074B()
        {
            AssertCode("@@@", "C9F9074B");
        }
        // Reko: a decoder for the instruction 6EFC83D8 at address 00121B4E has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6EFC83D8()
        {
            AssertCode("@@@", "6EFC83D8");
        }
        // Reko: a decoder for the instruction 4EFDBF79 at address 001005B6 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4EFDBF79()
        {
            AssertCode("@@@", "4EFDBF79");
        }
        // Reko: a decoder for the instruction 5EEF90DD at address 00136580 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_5EEF90DD()
        {
            AssertCode("@@@", "5EEF90DD");
        }
        // Reko: a decoder for the instruction AAFDB6CC at address 0013658A has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_AAFDB6CC()
        {
            AssertCode("@@@", "AAFDB6CC");
        }
        // Reko: a decoder for the instruction 30FF35A2 at address 0016E7E2 has not been implemented. (Unimplemented '*' when decoding FF30A235)
        [Test]
        public void ThumbDis_30FF35A2()
        {
            AssertCode("@@@", "30FF35A2");
        }
        // Reko: a decoder for the instruction AFEFE74B at address 001A7488 has not been implemented. (Unimplemented '*' when decoding EFAF4BE7)
        [Test]
        public void ThumbDis_AFEFE74B()
        {
            AssertCode("@@@", "AFEFE74B");
        }
        // Reko: a decoder for the instruction 4BFDB469 at address 00182D56 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4BFDB469()
        {
            AssertCode("@@@", "4BFDB469");
        }
        // Reko: a decoder for the instruction 8FF9C88D at address 001410AC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8FF9C88D()
        {
            AssertCode("@@@", "8FF9C88D");
        }
        // Reko: a decoder for the instruction 61EF49FB at address 0012B21A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_61EF49FB()
        {
            AssertCode("@@@", "61EF49FB");
        }
        // Reko: a decoder for the instruction A4FE5268 at address 0014A0BC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A4FE5268()
        {
            AssertCode("@@@", "A4FE5268");
        }
        // Reko: a decoder for the instruction F6FE3EC9 at address 001C2D6E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F6FE3EC9()
        {
            AssertCode("@@@", "F6FE3EC9");
        }
        // Reko: a decoder for the instruction 52FF1CAE at address 0010AF3A has not been implemented. (Unimplemented '*' when decoding FF52AE1C)
        [Test]
        public void ThumbDis_52FF1CAE()
        {
            AssertCode("@@@", "52FF1CAE");
        }
        // Reko: a decoder for the instruction D3FEDA2D at address 001C2EFC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D3FEDA2D()
        {
            AssertCode("@@@", "D3FEDA2D");
        }
        // Reko: a decoder for the instruction 75FCA029 at address 001C01E4 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_75FCA029()
        {
            AssertCode("@@@", "75FCA029");
        }
        // Reko: a decoder for the instruction 04FF5F3B at address 001E5164 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_04FF5F3B()
        {
            AssertCode("@@@", "04FF5F3B");
        }
        // Reko: a decoder for the instruction CAFDF5CD at address 001C02A8 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_CAFDF5CD()
        {
            AssertCode("@@@", "CAFDF5CD");
        }
        // Reko: a decoder for the instruction 6DFF3A7F at address 00162918 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_6DFF3A7F()
        {
            AssertCode("@@@", "6DFF3A7F");
        }
        // Reko: a decoder for the instruction E9F93264 at address 001E5304 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9E96432)
        [Test]
        public void ThumbDis_E9F93264()
        {
            AssertCode("@@@", "E9F93264");
        }
        // Reko: a decoder for the instruction 38FFFFD2 at address 0016E934 has not been implemented. (Unimplemented '*' when decoding FF38D2FF)
        [Test]
        public void ThumbDis_38FFFFD2()
        {
            AssertCode("@@@", "38FFFFD2");
        }
        // Reko: a decoder for the instruction F4FFEC45 at address 001A7538 has not been implemented. (Unimplemented '*reg' when decoding FFF445EC)
        [Test]
        public void ThumbDis_F4FFEC45()
        {
            AssertCode("@@@", "F4FFEC45");
        }
        // Reko: a decoder for the instruction 36EF7A99 at address 001E5412 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 54FF96AF at address 00130A3E has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_54FF96AF()
        {
            AssertCode("@@@", "54FF96AF");
        }
        // Reko: a decoder for the instruction DCEF1598 at address 001C2F0E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_DCEF1598()
        {
            AssertCode("@@@", "DCEF1598");
        }
        // Reko: a decoder for the instruction 1EFCDCA9 at address 00184226 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 78EF2BB3 at address 001C02EA has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_78EF2BB3()
        {
            AssertCode("@@@", "78EF2BB3");
        }
        // Reko: a decoder for the instruction B1FD618D at address 001629C6 has not been implemented. (op1:op2=0b1111)

        // Reko: a decoder for the instruction E1FF59CC at address 0016E964 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_E1FF59CC()
        {
            AssertCode("@@@", "E1FF59CC");
        }
        // Reko: a decoder for the instruction ECFF4B19 at address 00182DA2 has not been implemented. (Unimplemented '*scalar' when decoding FFEC194B)
        [Test]
        public void ThumbDis_ECFF4B19()
        {
            AssertCode("@@@", "ECFF4B19");
        }
        // Reko: a decoder for the instruction 02ED335E at address 0010B51E has not been implemented. (Unimplemented '*offset variant' when decoding ED025E33)
        [Test]
        public void ThumbDis_02ED335E()
        {
            AssertCode("@@@", "02ED335E");
        }
        // Reko: a decoder for the instruction 6AFEF729 at address 001481D4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6AFEF729()
        {
            AssertCode("@@@", "6AFEF729");
        }
        // Reko: a decoder for the instruction E6F98F2E at address 001C30F2 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E6F98F2E()
        {
            AssertCode("@@@", "E6F98F2E");
        }
        // Reko: a decoder for the instruction 8FF987C3 at address 001009CC has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F98FC387)
        [Test]
        public void ThumbDis_8FF987C3()
        {
            AssertCode("@@@", "8FF987C3");
        }
        // Reko: a decoder for the instruction 26FC0928 at address 001C32A2 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_26FC0928()
        {
            AssertCode("@@@", "26FC0928");
        }
        // Reko: a decoder for the instruction 44FCD2BD at address 00100BC8 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_44FCD2BD()
        {
            AssertCode("@@@", "44FCD2BD");
        }
        // Reko: a decoder for the instruction 52EFF052 at address 00141120 has not been implemented. (Unimplemented '*' when decoding EF5252F0)
        [Test]
        public void ThumbDis_52EFF052()
        {
            AssertCode("@@@", "52EFF052");
        }
        // Reko: a decoder for the instruction 11FF2B81 at address 001A7704 has not been implemented. (Unimplemented '*' when decoding FF11812B)
        [Test]
        public void ThumbDis_11FF2B81()
        {
            AssertCode("@@@", "11FF2B81");
        }
        // Reko: a decoder for the instruction 4DFFB8BF at address 001C457E has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_4DFFB8BF()
        {
            AssertCode("@@@", "4DFFB8BF");
        }
        // Reko: a decoder for the instruction F1EF70E8 at address 0013108A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_F1EF70E8()
        {
            AssertCode("@@@", "F1EF70E8");
        }
        // Reko: a decoder for the instruction 95EC846E at address 00103410 has not been implemented. (Unimplemented '*' when decoding EC956E84)

        // Reko: a decoder for the instruction C6EF73DD at address 001AA1C4 has not been implemented. (Unimplemented '*immediate - T3' when decoding EFC6DD73)
        [Test]
        public void ThumbDis_C6EF73DD()
        {
            AssertCode("@@@", "C6EF73DD");
        }
        // Reko: a decoder for the instruction B3EE1B8B at address 0016EAD2 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction C3FA9E44 at address 001AA274 has not been implemented. (crc32-crc32h)
        [Test]
        public void ThumbDis_C3FA9E44()
        {
            AssertCode("@@@", "C3FA9E44");
        }
        // Reko: a decoder for the instruction A7EC3B2B at address 0016EBA0 has not been implemented. (Unimplemented '*' when decoding ECA72B3B)
        [Test]
        public void ThumbDis_A7EC3B2B()
        {
            AssertCode("@@@", "A7EC3B2B");
        }
        // Reko: a decoder for the instruction E7F377AF at address 0016EBAC has not been implemented. (Unimplemented '*banked register' when decoding F3E7AF77)
        [Test]
        public void ThumbDis_E7F377AF()
        {
            AssertCode("@@@", "E7F377AF");
        }
        // Reko: a decoder for the instruction 8AFF54B8 at address 0010B598 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_8AFF54B8()
        {
            AssertCode("@@@", "8AFF54B8");
        }
        // Reko: a decoder for the instruction 2AFC2C8D at address 0010BC84 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2AFC2C8D()
        {
            AssertCode("@@@", "2AFC2C8D");
        }
        // Reko: a decoder for the instruction 91FFB81C at address 00100BF6 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_91FFB81C()
        {
            AssertCode("@@@", "91FFB81C");
        }
        // Reko: a decoder for the instruction 4BFFBE93 at address 0013C2D8 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_4BFFBE93()
        {
            AssertCode("@@@", "4BFFBE93");
        }
        // Reko: a decoder for the instruction 3DF91C8F at address 0014163E has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_3DF91C8F()
        {
            AssertCode("@@@", "3DF91C8F");
        }
        // Reko: a decoder for the instruction A6FC0198 at address 001C049E has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A6FC0198()
        {
            AssertCode("@@@", "A6FC0198");
        }
        // Reko: a decoder for the instruction AFFCF81D at address 001C4B8E has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_AFFCF81D()
        {
            AssertCode("@@@", "AFFCF81D");
        }
        // Reko: a decoder for the instruction AEFD4199 at address 001AA3A8 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_AEFD4199()
        {
            AssertCode("@@@", "AEFD4199");
        }
        // Reko: a decoder for the instruction 6CFC1E68 at address 00184280 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6CFC1E68()
        {
            AssertCode("@@@", "6CFC1E68");
        }
        // Reko: a decoder for the instruction 80FC84CD at address 0016EBC8 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_80FC84CD()
        {
            AssertCode("@@@", "80FC84CD");
        }
        // Reko: a decoder for the instruction 88F9346C at address 001843BC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_88F9346C()
        {
            AssertCode("@@@", "88F9346C");
        }
        // Reko: a decoder for the instruction C5F9EC2F at address 0010BCEE has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C5F9EC2F()
        {
            AssertCode("@@@", "C5F9EC2F");
        }
        // Reko: a decoder for the instruction 2CEFF1B9 at address 0014A2CC has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_2CEFF1B9()
        {
            AssertCode("@@@", "2CEFF1B9");
        }
        // Reko: a decoder for the instruction FEFDFD78 at address 00141F28 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FEFDFD78()
        {
            AssertCode("@@@", "FEFDFD78");
        }
        // Reko: a decoder for the instruction 53F9C7DD at address 00141F2E has not been implemented. (LoadStoreSignedImmediatePreIndexed)

        // Reko: a decoder for the instruction DFFD51F8 at address 00141F8E has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DFFD51F8()
        {
            AssertCode("@@@", "DFFD51F8");
        }
        // Reko: a decoder for the instruction 47FF6A53 at address 00141FEA has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_47FF6A53()
        {
            AssertCode("@@@", "47FF6A53");
        }
        // Reko: a decoder for the instruction 1DFF734C at address 001761E0 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_1DFF734C()
        {
            AssertCode("@@@", "1DFF734C");
        }
        // Reko: a decoder for the instruction 8AFA82DD at address 001C4BCA has not been implemented. (Unimplemented '*' when decoding FA8ADD82)
        [Test]
        public void ThumbDis_8AFA82DD()
        {
            AssertCode("@@@", "8AFA82DD");
        }
        // Reko: a decoder for the instruction CBF934DE at address 0017E42A has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CBF934DE()
        {
            AssertCode("@@@", "CBF934DE");
        }
        // Reko: a decoder for the instruction 10FDA8FD at address 001AA754 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 2AFF124C at address 001AA9B6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_2AFF124C()
        {
            AssertCode("@@@", "2AFF124C");
        }
        // Reko: a decoder for the instruction 3AFFFD62 at address 001131BE has not been implemented. (Unimplemented '*' when decoding FF3A62FD)
        [Test]
        public void ThumbDis_3AFFFD62()
        {
            AssertCode("@@@", "3AFFFD62");
        }
        // Reko: a decoder for the instruction A2FE7F4D at address 001325D2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A2FE7F4D()
        {
            AssertCode("@@@", "A2FE7F4D");
        }
        // Reko: a decoder for the instruction 4BFC37FC at address 0013288A has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4BFC37FC()
        {
            AssertCode("@@@", "4BFC37FC");
        }
        // Reko: a decoder for the instruction 3FEE356B at address 00113644 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_3FEE356B()
        {
            AssertCode("@@@", "3FEE356B");
        }
        // Reko: a decoder for the instruction EBF92F34 at address 00132A56 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9EB342F)
        [Test]
        public void ThumbDis_EBF92F34()
        {
            AssertCode("@@@", "EBF92F34");
        }
        // Reko: a decoder for the instruction 3EFFB002 at address 00132A66 has not been implemented. (Unimplemented '*' when decoding FF3E02B0)
        [Test]
        public void ThumbDis_3EFFB002()
        {
            AssertCode("@@@", "3EFFB002");
        }
        // Reko: a decoder for the instruction D6FD65ED at address 00113820 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D6FD65ED()
        {
            AssertCode("@@@", "D6FD65ED");
        }
        // Reko: a decoder for the instruction 3AFE231C at address 001C0978 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3AFE231C()
        {
            AssertCode("@@@", "3AFE231C");
        }
        // Reko: a decoder for the instruction F4FD8A28 at address 00184632 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F4FD8A28()
        {
            AssertCode("@@@", "F4FD8A28");
        }
        // Reko: a decoder for the instruction A2F91E9E at address 001C11C6 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A2F91E9E()
        {
            AssertCode("@@@", "A2F91E9E");
        }
        // Reko: a decoder for the instruction 0EEFFB84 at address 00176946 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_0EEFFB84()
        {
            AssertCode("@@@", "0EEFFB84");
        }
        // Reko: a decoder for the instruction C7F91A44 at address 0016ECE6 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9C7441A)
        [Test]
        public void ThumbDis_C7F91A44()
        {
            AssertCode("@@@", "C7F91A44");
        }
        // Reko: a decoder for the instruction 46EF387B at address 001139D8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_46EF387B()
        {
            AssertCode("@@@", "46EF387B");
        }
        // Reko: a decoder for the instruction EFFFE33F at address 00183256 has not been implemented. (Unimplemented '*' when decoding FFEF3FE3)
        [Test]
        public void ThumbDis_EFFFE33F()
        {
            AssertCode("@@@", "EFFFE33F");
        }
        // Reko: a decoder for the instruction A5F94729 at address 00113C34 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9A52947)
        [Test]
        public void ThumbDis_A5F94729()
        {
            AssertCode("@@@", "A5F94729");
        }
        // Reko: a decoder for the instruction E2EEF55B at address 0010C554 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction CEF90F99 at address 0010C590 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9CE990F)
        [Test]
        public void ThumbDis_CEF90F99()
        {
            AssertCode("@@@", "CEF90F99");
        }
        // Reko: a decoder for the instruction C1EFCBDA at address 001EC592 has not been implemented. (Unimplemented '*' when decoding EFC1DACB)
        [Test]
        public void ThumbDis_C1EFCBDA()
        {
            AssertCode("@@@", "C1EFCBDA");
        }
        // Reko: a decoder for the instruction 12FF7051 at address 001ED224 has not been implemented. (Unimplemented '*register' when decoding FF125170)
        [Test]
        public void ThumbDis_12FF7051()
        {
            AssertCode("@@@", "12FF7051");
        }
        // Reko: a decoder for the instruction 3CFFAC0D at address 0010E52E has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_3CFFAC0D()
        {
            AssertCode("@@@", "3CFFAC0D");
        }
        // Reko: a decoder for the instruction 7DFFB0A2 at address 001AAC1E has not been implemented. (Unimplemented '*' when decoding FF7DA2B0)
        [Test]
        public void ThumbDis_7DFFB0A2()
        {
            AssertCode("@@@", "7DFFB0A2");
        }
        // Reko: a decoder for the instruction D8EFEF4C at address 0016EFCA has not been implemented. (Unimplemented '*' when decoding EFD84CEF)
        [Test]
        public void ThumbDis_D8EFEF4C()
        {
            AssertCode("@@@", "D8EFEF4C");
        }
        // Reko: a decoder for the instruction 5DFFD139 at address 0015A372 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction EBEE7528 at address 0015A398 has not been implemented. (Unimplemented '*' when decoding EEEB2875)
        [Test]
        public void ThumbDis_EBEE7528()
        {
            AssertCode("@@@", "EBEE7528");
        }
        // Reko: a decoder for the instruction 7DFEF3FC at address 001C6328 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_7DFEF3FC()
        {
            AssertCode("@@@", "7DFEF3FC");
        }
        // Reko: a decoder for the instruction 0CFD2C0D at address 00114346 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0CFD2C0D()
        {
            AssertCode("@@@", "0CFD2C0D");
        }
        // Reko: a decoder for the instruction AEF9E712 at address 0015A686 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9AE12E7)
        [Test]
        public void ThumbDis_AEF9E712()
        {
            AssertCode("@@@", "AEF9E712");
        }
        // Reko: a decoder for the instruction 94FFB9DB at address 00138DD8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_94FFB9DB()
        {
            AssertCode("@@@", "94FFB9DB");
        }
        // Reko: a decoder for the instruction B4FE737D at address 001ED252 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B4FE737D()
        {
            AssertCode("@@@", "B4FE737D");
        }
        // Reko: a decoder for the instruction DDFA97DA at address 00176A56 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_DDFA97DA()
        {
            AssertCode("@@@", "DDFA97DA");
        }
        // Reko: a decoder for the instruction 4DFF16D2 at address 00185946 has not been implemented. (Unimplemented '*' when decoding FF4DD216)
        [Test]
        public void ThumbDis_4DFF16D2()
        {
            AssertCode("@@@", "4DFF16D2");
        }
        // Reko: a decoder for the instruction C6F9EF59 at address 0010E592 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9C659EF)
        [Test]
        public void ThumbDis_C6F9EF59()
        {
            AssertCode("@@@", "C6F9EF59");
        }
        // Reko: a decoder for the instruction 42FCDAFC at address 0017F33C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_42FCDAFC()
        {
            AssertCode("@@@", "42FCDAFC");
        }
        // Reko: a decoder for the instruction D4FED21D at address 00185FDE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D4FED21D()
        {
            AssertCode("@@@", "D4FED21D");
        }
        // Reko: a decoder for the instruction 5EEFD9C1 at address 00150338 has not been implemented. (Unimplemented '*register' when decoding EF5EC1D9)
        [Test]
        public void ThumbDis_5EEFD9C1()
        {
            AssertCode("@@@", "5EEFD9C1");
        }
        // Reko: a decoder for the instruction D7F3358F at address 001C63D6 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D7F3358F()
        {
            AssertCode("@@@", "D7F3358F");
        }
        // Reko: a decoder for the instruction 4FEF2E94 at address 00148C48 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_4FEF2E94()
        {
            AssertCode("@@@", "4FEF2E94");
        }
        // Reko: a decoder for the instruction 2FFD6688 at address 00150840 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2FFD6688()
        {
            AssertCode("@@@", "2FFD6688");
        }
        // Reko: a decoder for the instruction AEFF1218 at address 001143A2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_AEFF1218()
        {
            AssertCode("@@@", "AEFF1218");
        }
        // Reko: a decoder for the instruction 17EFEF0C at address 0010E87A has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_17EFEF0C()
        {
            AssertCode("@@@", "17EFEF0C");
        }
        // Reko: a decoder for the instruction C7F93091 at address 0013E29C has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9C79130)
        [Test]
        public void ThumbDis_C7F93091()
        {
            AssertCode("@@@", "C7F93091");
        }
        // Reko: a decoder for the instruction F1EF7A6C at address 0015AF06 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_F1EF7A6C()
        {
            AssertCode("@@@", "F1EF7A6C");
        }
        // Reko: a decoder for the instruction B7FEC89C at address 00118424 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B7FEC89C()
        {
            AssertCode("@@@", "B7FEC89C");
        }
        // Reko: a decoder for the instruction CBFD9898 at address 0018C534 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_CBFD9898()
        {
            AssertCode("@@@", "CBFD9898");
        }
        // Reko: a decoder for the instruction 3EEF92A4 at address 001398F4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_3EEF92A4()
        {
            AssertCode("@@@", "3EEF92A4");
        }
        // Reko: a decoder for the instruction 97EF57DC at address 00148C54 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_97EF57DC()
        {
            AssertCode("@@@", "97EF57DC");
        }
        // Reko: a decoder for the instruction F5F33EA4 at address 00188972 has not been implemented. (Unimplemented '*banked register' when decoding F3F5A43E)
        [Test]
        public void ThumbDis_F5F33EA4()
        {
            AssertCode("@@@", "F5F33EA4");
        }
        // Reko: a decoder for the instruction 8AFDA718 at address 001890D4 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8AFDA718()
        {
            AssertCode("@@@", "8AFDA718");
        }
        // Reko: a decoder for the instruction E6FEA59C at address 001C6578 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E6FEA59C()
        {
            AssertCode("@@@", "E6FEA59C");
        }
        // Reko: a decoder for the instruction 82FCD3FC at address 001B23F4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_82FCD3FC()
        {
            AssertCode("@@@", "82FCD3FC");
        }
        // Reko: a decoder for the instruction 03FC8EB9 at address 00150896 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_03FC8EB9()
        {
            AssertCode("@@@", "03FC8EB9");
        }
        // Reko: a decoder for the instruction D5E8CBF2 at address 0017735E has not been implemented. (Unimplemented '*' when decoding E8D5F2CB)
        [Test]
        public void ThumbDis_D5E8CBF2()
        {
            AssertCode("@@@", "D5E8CBF2");
        }
        // Reko: a decoder for the instruction 7DFF4BDF at address 0010EA8E has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_7DFF4BDF()
        {
            AssertCode("@@@", "7DFF4BDF");
        }
        // Reko: a decoder for the instruction AFF9881E at address 0013E30E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_AFF9881E()
        {
            AssertCode("@@@", "AFF9881E");
        }
        // Reko: a decoder for the instruction 31EF97E4 at address 0018C544 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_31EF97E4()
        {
            AssertCode("@@@", "31EF97E4");
        }
        // Reko: a decoder for the instruction 8BFFE4B8 at address 001722C4 has not been implemented. (Unimplemented '*scalar' when decoding FF8BB8E4)
        [Test]
        public void ThumbDis_8BFFE4B8()
        {
            AssertCode("@@@", "8BFFE4B8");
        }
        // Reko: a decoder for the instruction 3FEF3B54 at address 001E99E4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_3FEF3B54()
        {
            AssertCode("@@@", "3FEF3B54");
        }
        // Reko: a decoder for the instruction CBFEC5AD at address 0013F724 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_CBFEC5AD()
        {
            AssertCode("@@@", "CBFEC5AD");
        }
        // Reko: a decoder for the instruction A9FD15CD at address 00149638 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A9FD15CD()
        {
            AssertCode("@@@", "A9FD15CD");
        }
        // Reko: a decoder for the instruction 9AFAB143 at address 001B496E has not been implemented. (Unimplemented '*' when decoding FA9A43B1)
        [Test]
        public void ThumbDis_9AFAB143()
        {
            AssertCode("@@@", "9AFAB143");
        }
        // Reko: a decoder for the instruction 33F9274E at address 001EDDD0 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_33F9274E()
        {
            AssertCode("@@@", "33F9274E");
        }
        // Reko: a decoder for the instruction A4F913CF at address 001541C2 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A4F913CF()
        {
            AssertCode("@@@", "A4F913CF");
        }
        // Reko: a decoder for the instruction 03EF90E2 at address 001C67AA has not been implemented. (Unimplemented '*' when decoding EF03E290)
        [Test]
        public void ThumbDis_03EF90E2()
        {
            AssertCode("@@@", "03EF90E2");
        }
        // Reko: a decoder for the instruction A0F977B5 at address 001B2504 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9A0B577)
        [Test]
        public void ThumbDis_A0F977B5()
        {
            AssertCode("@@@", "A0F977B5");
        }
        // Reko: a decoder for the instruction E5FD0628 at address 00178BC0 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E5FD0628()
        {
            AssertCode("@@@", "E5FD0628");
        }
        // Reko: a decoder for the instruction F4EF1B7C at address 00172C48 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_F4EF1B7C()
        {
            AssertCode("@@@", "F4EF1B7C");
        }
        // Reko: a decoder for the instruction 67EFE6B4 at address 0019C44A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_67EFE6B4()
        {
            AssertCode("@@@", "67EFE6B4");
        }
        // Reko: a decoder for the instruction 63FDBD9D at address 001496A4 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_63FDBD9D()
        {
            AssertCode("@@@", "63FDBD9D");
        }
        // Reko: a decoder for the instruction EFFF3168 at address 00118E78 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_EFFF3168()
        {
            AssertCode("@@@", "EFFF3168");
        }
        // Reko: a decoder for the instruction 1CFFB501 at address 001E9E98 has not been implemented. (Unimplemented '*register' when decoding FF1C01B5)
        [Test]
        public void ThumbDis_1CFFB501()
        {
            AssertCode("@@@", "1CFFB501");
        }
        // Reko: a decoder for the instruction 93EF2F7D at address 001C9B36 has not been implemented. (Unimplemented '*integer' when decoding EF937D2F)
        [Test]
        public void ThumbDis_93EF2F7D()
        {
            AssertCode("@@@", "93EF2F7D");
        }
        // Reko: a decoder for the instruction AEF9BDD8 at address 001508F2 has not been implemented. (Unimplemented 'single element from one lane - T3' when decoding F9AED8BD)
        [Test]
        public void ThumbDis_AEF9BDD8()
        {
            AssertCode("@@@", "AEF9BDD8");
        }
        // Reko: a decoder for the instruction 06FFC99C at address 0015437E has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_06FFC99C()
        {
            AssertCode("@@@", "06FFC99C");
        }
        // Reko: a decoder for the instruction 1BEFA6D3 at address 00150F66 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1BEFA6D3()
        {
            AssertCode("@@@", "1BEFA6D3");
        }
        // Reko: a decoder for the instruction 15FC820D at address 00150F7A has not been implemented. (op1:op2=0b0001)
 
        // Reko: a decoder for the instruction B2FE3A88 at address 0013F824 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B2FE3A88()
        {
            AssertCode("@@@", "B2FE3A88");
        }
        // Reko: a decoder for the instruction E9F965DE at address 0019C560 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E9F965DE()
        {
            AssertCode("@@@", "E9F965DE");
        }
        // Reko: a decoder for the instruction 56EE5FAB at address 00149732 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_56EE5FAB()
        {
            AssertCode("@@@", "56EE5FAB");
        }
        // Reko: a decoder for the instruction CEF9372E at address 0011EA22 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CEF9372E()
        {
            AssertCode("@@@", "CEF9372E");
        }
        // Reko: a decoder for the instruction 4AFD39E8 at address 0011EA74 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4AFD39E8()
        {
            AssertCode("@@@", "4AFD39E8");
        }
        // Reko: a decoder for the instruction 01FFD49E at address 001EA5BA has not been implemented. (Unimplemented '*' when decoding FF019ED4)
        [Test]
        public void ThumbDis_01FFD49E()
        {
            AssertCode("@@@", "01FFD49E");
        }
        // Reko: a decoder for the instruction 8CFCFB19 at address 001EE0CA has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8CFCFB19()
        {
            AssertCode("@@@", "8CFCFB19");
        }
        // Reko: a decoder for the instruction 17FFDFBB at address 001C6E28 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_17FFDFBB()
        {
            AssertCode("@@@", "17FFDFBB");
        }
        // Reko: a decoder for the instruction 68EFCAB3 at address 0015C58E has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_68EFCAB3()
        {
            AssertCode("@@@", "68EFCAB3");
        }
        // Reko: a decoder for the instruction 00FD24E9 at address 001C6F24 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_00FD24E9()
        {
            AssertCode("@@@", "00FD24E9");
        }
        // Reko: a decoder for the instruction 91FFE0DF at address 001C72E2 has not been implemented. (Unimplemented '*' when decoding FF91DFE0)
        [Test]
        public void ThumbDis_91FFE0DF()
        {
            AssertCode("@@@", "91FFE0DF");
        }
        // Reko: a decoder for the instruction D7FD5A48 at address 00172DD6 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D7FD5A48()
        {
            AssertCode("@@@", "D7FD5A48");
        }
        // Reko: a decoder for the instruction 48FF431B at address 001CC262 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_48FF431B()
        {
            AssertCode("@@@", "48FF431B");
        }
        // Reko: a decoder for the instruction C4F92E8C at address 00154920 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C4F92E8C()
        {
            AssertCode("@@@", "C4F92E8C");
        }
        // Reko: a decoder for the instruction 7DEFA383 at address 001549E2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_7DEFA383()
        {
            AssertCode("@@@", "7DEFA383");
        }
        // Reko: a decoder for the instruction EDECA3FB at address 00154B00 has not been implemented. (Unimplemented '*' when decoding ECEDFBA3)
        [Test]
        public void ThumbDis_EDECA3FB()
        {
            AssertCode("@@@", "EDECA3FB");
        }
        // Reko: a decoder for the instruction 5CEF351B at address 00154B5A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_5CEF351B()
        {
            AssertCode("@@@", "5CEF351B");
        }
        // Reko: a decoder for the instruction 78FE3F49 at address 00190842 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_78FE3F49()
        {
            AssertCode("@@@", "78FE3F49");
        }
        // Reko: a decoder for the instruction 9BFF6ABC at address 00190930 has not been implemented. (Unimplemented '*' when decoding FF9BBC6A)

        // Reko: a decoder for the instruction E3F95271 at address 00190972 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9E37152)
        [Test]
        public void ThumbDis_E3F95271()
        {
            AssertCode("@@@", "E3F95271");
        }
        // Reko: a decoder for the instruction 6DFE852D at address 0013F83A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6DFE852D()
        {
            AssertCode("@@@", "6DFE852D");
        }
        // Reko: a decoder for the instruction 99FFF448 at address 0013FB02 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_99FFF448()
        {
            AssertCode("@@@", "99FFF448");
        }
        // Reko: a decoder for the instruction A3F9651C at address 0013FB06 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A3F9651C()
        {
            AssertCode("@@@", "A3F9651C");
        }
        // Reko: a decoder for the instruction 36EFE221 at address 0013FB74 has not been implemented. (Unimplemented '*' when decoding EF3621E2)
        [Test]
        public void ThumbDis_36EFE221()
        {
            AssertCode("@@@", "36EFE221");
        }
        // Reko: a decoder for the instruction BDEFF2A8 at address 0013FBEC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_BDEFF2A8()
        {
            AssertCode("@@@", "BDEFF2A8");
        }
        // Reko: a decoder for the instruction 7CF99D0F at address 0019C5A0 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_7CF99D0F()
        {
            AssertCode("@@@", "7CF99D0F");
        }
        // Reko: a decoder for the instruction 4BFF26BC at address 0011EBF0 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_4BFF26BC()
        {
            AssertCode("@@@", "4BFF26BC");
        }
        // Reko: a decoder for the instruction A2EF6594 at address 001CC406 has not been implemented. (Unimplemented '*scalar' when decoding EFA29465)
        [Test]
        public void ThumbDis_A2EF6594()
        {
            AssertCode("@@@", "A2EF6594");
        }
        // Reko: a decoder for the instruction 18EB783F at address 001CC57A has not been implemented. (Unimplemented '*register' when decoding EB183F78)
        [Test]
        public void ThumbDis_18EB783F()
        {
            AssertCode("@@@", "18EB783F");
        }
        // Reko: a decoder for the instruction 29FD1C6D at address 001CC5A4 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_29FD1C6D()
        {
            AssertCode("@@@", "29FD1C6D");
        }
        // Reko: a decoder for the instruction 4EF93362 at address 001C9F46 has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F94E6233)
        [Test]
        public void ThumbDis_4EF93362()
        {
            AssertCode("@@@", "4EF93362");
        }
        // Reko: a decoder for the instruction 87FF91B8 at address 00149A04 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_87FF91B8()
        {
            AssertCode("@@@", "87FF91B8");
        }
        // Reko: a decoder for the instruction FFFC1CB9 at address 001CA1E6 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_FFFC1CB9()
        {
            AssertCode("@@@", "FFFC1CB9");
        }
        // Reko: a decoder for the instruction DCFC9A58 at address 001EE260 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DCFC9A58()
        {
            AssertCode("@@@", "DCFC9A58");
        }
        // Reko: a decoder for the instruction 20FD4A28 at address 0015C6DE has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_20FD4A28()
        {
            AssertCode("@@@", "20FD4A28");
        }
        // Reko: a decoder for the instruction D1F3F880 at address 0015CC1E has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D1F3F880()
        {
            AssertCode("@@@", "D1F3F880");
        }
        // Reko: a decoder for the instruction 78EF141D at address 00154B90 has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_78EF141D()
        {
            AssertCode("@@@", "78EF141D");
        }
        // Reko: a decoder for the instruction E9F991A0 at address 00178E20 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9E9A091)
        [Test]
        public void ThumbDis_E9F991A0()
        {
            AssertCode("@@@", "E9F991A0");
        }
        // Reko: a decoder for the instruction 17FFEB74 at address 0013FD28 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_17FFEB74()
        {
            AssertCode("@@@", "17FFEB74");
        }
        // Reko: a decoder for the instruction C9FF4489 at address 00190A12 has not been implemented. (Unimplemented '*scalar' when decoding FFC98944)
        [Test]
        public void ThumbDis_C9FF4489()
        {
            AssertCode("@@@", "C9FF4489");
        }
        // Reko: a decoder for the instruction F9F3ECAE at address 0013FE8E has not been implemented. (Unimplemented '*banked register' when decoding F3F9AEEC)
        [Test]
        public void ThumbDis_F9F3ECAE()
        {
            AssertCode("@@@", "F9F3ECAE");
        }
        // Reko: a decoder for the instruction 5AFCDB9D at address 00178F4C has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction C0FE28CD at address 00178F84 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C0FE28CD()
        {
            AssertCode("@@@", "C0FE28CD");
        }
        // Reko: a decoder for the instruction FBFFF89C at address 001CC640 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_FBFFF89C()
        {
            AssertCode("@@@", "FBFFF89C");
        }
        // Reko: a decoder for the instruction D3FA9E86 at address 001CC656 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D3FA9E86()
        {
            AssertCode("@@@", "D3FA9E86");
        }
        // Reko: a decoder for the instruction E9FC0E78 at address 001CC768 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E9FC0E78()
        {
            AssertCode("@@@", "E9FC0E78");
        }
        // Reko: a decoder for the instruction 40FE5BFD at address 001CC8B6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_40FE5BFD()
        {
            AssertCode("@@@", "40FE5BFD");
        }
        // Reko: a decoder for the instruction 8AFAA1D7 at address 001EA64C has not been implemented. (Unimplemented '*' when decoding FA8AD7A1)
        [Test]
        public void ThumbDis_8AFAA1D7()
        {
            AssertCode("@@@", "8AFAA1D7");
        }
        // Reko: a decoder for the instruction 0EFDBBEC at address 001CCA1A has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0EFDBBEC()
        {
            AssertCode("@@@", "0EFDBBEC");
        }
        // Reko: a decoder for the instruction 57FE4D1C at address 001CCAFC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_57FE4D1C()
        {
            AssertCode("@@@", "57FE4D1C");
        }
        // Reko: a decoder for the instruction 12EFFE1D at address 001BE83C has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_12EFFE1D()
        {
            AssertCode("@@@", "12EFFE1D");
        }
        // Reko: a decoder for the instruction A2F9DCFD at address 001BE888 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A2F9DCFD()
        {
            AssertCode("@@@", "A2F9DCFD");
        }
        // Reko: a decoder for the instruction DEE8D418 at address 00154BEC has not been implemented. (Unimplemented '*' when decoding E8DE18D4)
        [Test]
        public void ThumbDis_DEE8D418()
        {
            AssertCode("@@@", "DEE8D418");
        }
        // Reko: a decoder for the instruction A6F9AA74 at address 001510EE has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9A674AA)
        [Test]
        public void ThumbDis_A6F9AA74()
        {
            AssertCode("@@@", "A6F9AA74");
        }
        // Reko: a decoder for the instruction B7FFD52C at address 0019C774 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_B7FFD52C()
        {
            AssertCode("@@@", "B7FFD52C");
        }
        // Reko: a decoder for the instruction 7AFF4B8D at address 00190A90 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_7AFF4B8D()
        {
            AssertCode("@@@", "7AFF4B8D");
        }
        // Reko: a decoder for the instruction 5CFC544C at address 0013FF50 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 93EE91D9 at address 0011934E has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction EEF9C240 at address 00178FD0 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9EE40C2)
        [Test]
        public void ThumbDis_EEF9C240()
        {
            AssertCode("@@@", "EEF9C240");
        }
        // Reko: a decoder for the instruction 43F93D02 at address 0018A65E has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F943023D)
        [Test]
        public void ThumbDis_43F93D02()
        {
            AssertCode("@@@", "43F93D02");
        }
        // Reko: a decoder for the instruction EFFF40A5 at address 001EA678 has not been implemented. (Unimplemented '*scalar' when decoding FFEFA540)
        [Test]
        public void ThumbDis_EFFF40A5()
        {
            AssertCode("@@@", "EFFF40A5");
        }
        // Reko: a decoder for the instruction 64FC770C at address 001EA68C has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_64FC770C()
        {
            AssertCode("@@@", "64FC770C");
        }
        // Reko: a decoder for the instruction 9AEFF8FC at address 001731E8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_9AEFF8FC()
        {
            AssertCode("@@@", "9AEFF8FC");
        }
        // Reko: a decoder for the instruction 9FFDFDAC at address 001BE8E2 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_9FFDFDAC()
        {
            AssertCode("@@@", "9FFDFDAC");
        }
        // Reko: a decoder for the instruction 13EFA431 at address 001BE8EE has not been implemented. (Unimplemented '*' when decoding EF1331A4)
        [Test]
        public void ThumbDis_13EFA431()
        {
            AssertCode("@@@", "13EFA431");
        }
        // Reko: a decoder for the instruction C4EEFACB at address 0019C880 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction ADF3088C at address 0011F324 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_ADF3088C()
        {
            AssertCode("@@@", "ADF3088C");
        }
        // Reko: a decoder for the instruction EAFF3B5B at address 001DC032 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_EAFF3B5B()
        {
            AssertCode("@@@", "EAFF3B5B");
        }
        // Reko: a decoder for the instruction E3F9A3F0 at address 0011F396 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9E3F0A3)
        [Test]
        public void ThumbDis_E3F9A3F0()
        {
            AssertCode("@@@", "E3F9A3F0");
        }
        // Reko: a decoder for the instruction 52F9CABF at address 00119356 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_52F9CABF()
        {
            AssertCode("@@@", "52F9CABF");
        }
        // Reko: a decoder for the instruction E6FE12BD at address 0011F4A0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E6FE12BD()
        {
            AssertCode("@@@", "E6FE12BD");
        }
        // Reko: a decoder for the instruction E8FCC53C at address 0018A668 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E8FCC53C()
        {
            AssertCode("@@@", "E8FCC53C");
        }
        // Reko: a decoder for the instruction D7FD18FD at address 0018A672 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D7FD18FD()
        {
            AssertCode("@@@", "D7FD18FD");
        }
        // Reko: a decoder for the instruction 12FC1289 at address 0017916C has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 20EF1219 at address 001CA200 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction E9FC9058 at address 001F422E has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E9FC9058()
        {
            AssertCode("@@@", "E9FC9058");
        }
        // Reko: a decoder for the instruction 54EF208C at address 0017325E has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_54EF208C()
        {
            AssertCode("@@@", "54EF208C");
        }
        // Reko: a decoder for the instruction 92FAB1F4 at address 00173286 has not been implemented. (Unimplemented '*' when decoding FA92F4B1)
        [Test]
        public void ThumbDis_92FAB1F4()
        {
            AssertCode("@@@", "92FAB1F4");
        }
        // Reko: a decoder for the instruction 85EF09B9 at address 0017328C has not been implemented. (Unimplemented '*integer' when decoding EF85B909)
        [Test]
        public void ThumbDis_85EF09B9()
        {
            AssertCode("@@@", "85EF09B9");
        }
        // Reko: a decoder for the instruction 33EF7473 at address 001F1238 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_33EF7473()
        {
            AssertCode("@@@", "33EF7473");
        }
        // Reko: a decoder for the instruction 68FD3EA9 at address 0015F1F4 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_68FD3EA9()
        {
            AssertCode("@@@", "68FD3EA9");
        }
        // Reko: a decoder for the instruction 16FF8421 at address 00190AC2 has not been implemented. (Unimplemented '*' when decoding FF162184)
        [Test]
        public void ThumbDis_16FF8421()
        {
            AssertCode("@@@", "16FF8421");
        }
        // Reko: a decoder for the instruction E6ECB4EB at address 00190AD6 has not been implemented. (Unimplemented '*' when decoding ECE6EBB4)
        [Test]
        public void ThumbDis_E6ECB4EB()
        {
            AssertCode("@@@", "E6ECB4EB");
        }
        // Reko: a decoder for the instruction 78EFBA3E at address 001193EA has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)

        // Reko: a decoder for the instruction 5DFD798C at address 00190B8A has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 52EFD6EF at address 00190ED4 has not been implemented. (Unimplemented '*' when decoding EF52EFD6)
        [Test]
        public void ThumbDis_52EFD6EF()
        {
            AssertCode("@@@", "52EFD6EF");
        }
        // Reko: a decoder for the instruction 8CFFECF4 at address 00190EE8 has not been implemented. (Unimplemented '*scalar' when decoding FF8CF4EC)
        [Test]
        public void ThumbDis_8CFFECF4()
        {
            AssertCode("@@@", "8CFFECF4");
        }
        // Reko: a decoder for the instruction 60EF72CE at address 0011F622 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)

        // Reko: a decoder for the instruction 21FF1F82 at address 00179232 has not been implemented. (Unimplemented '*' when decoding FF21821F)
        [Test]
        public void ThumbDis_21FF1F82()
        {
            AssertCode("@@@", "21FF1F82");
        }
        // Reko: a decoder for the instruction DDFD2AF8 at address 001792AC has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DDFD2AF8()
        {
            AssertCode("@@@", "DDFD2AF8");
        }
        // Reko: a decoder for the instruction BDFDDA6D at address 0018AC44 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_BDFDDA6D()
        {
            AssertCode("@@@", "BDFDDA6D");
        }
        // Reko: a decoder for the instruction 04EF51ED at address 00179378 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_04EF51ED()
        {
            AssertCode("@@@", "04EF51ED");
        }
        // Reko: a decoder for the instruction 7EFCC84D at address 00179392 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7EFCC84D()
        {
            AssertCode("@@@", "7EFCC84D");
        }
        // Reko: a decoder for the instruction F5FDCAD8 at address 001793A4 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F5FDCAD8()
        {
            AssertCode("@@@", "F5FDCAD8");
        }
        // Reko: a decoder for the instruction 3FFFE073 at address 001511AE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_3FFFE073()
        {
            AssertCode("@@@", "3FFFE073");
        }
        // Reko: a decoder for the instruction 96FD35C8 at address 0019C8DC has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_96FD35C8()
        {
            AssertCode("@@@", "96FD35C8");
        }
        // Reko: a decoder for the instruction CBFD3979 at address 0015F24A has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_CBFD3979()
        {
            AssertCode("@@@", "CBFD3979");
        }
        // Reko: a decoder for the instruction 48EFDB89 at address 001F1690 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_48EFDB89()
        {
            AssertCode("@@@", "48EFDB89");
        }
        // Reko: a decoder for the instruction 1EEFDB7B at address 001DC092 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_1EEFDB7B()
        {
            AssertCode("@@@", "1EEFDB7B");
        }
        // Reko: a decoder for the instruction 0DEFE11B at address 001F1750 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_0DEFE11B()
        {
            AssertCode("@@@", "0DEFE11B");
        }
        // Reko: a decoder for the instruction 2EFFC88F at address 00119442 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_2EFFC88F()
        {
            AssertCode("@@@", "2EFFC88F");
        }
        // Reko: a decoder for the instruction D4FA8CD2 at address 0019CC48 has not been implemented. (crc32c-crc32cb)
        [Test]
        public void ThumbDis_D4FA8CD2()
        {
            AssertCode("@@@", "D4FA8CD2");
        }
        // Reko: a decoder for the instruction 5AFC50CD at address 00149D38 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 13EF196E at address 0011F6A4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_13EF196E()
        {
            AssertCode("@@@", "13EF196E");
        }
        // Reko: a decoder for the instruction ACFEF0D8 at address 001CCFE4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_ACFEF0D8()
        {
            AssertCode("@@@", "ACFEF0D8");
        }
        // Reko: a decoder for the instruction E4FFE19A at address 001EA8EC has not been implemented. (Unimplemented '*' when decoding FFE49AE1)
        [Test]
        public void ThumbDis_E4FFE19A()
        {
            AssertCode("@@@", "E4FFE19A");
        }
        // Reko: a decoder for the instruction A2FA8D64 at address 0018ACA4 has not been implemented. (Unimplemented '*' when decoding FAA2648D)
        [Test]
        public void ThumbDis_A2FA8D64()
        {
            AssertCode("@@@", "A2FA8D64");
        }
        // Reko: a decoder for the instruction 86F991C2 at address 001F4346 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F986C291)
        [Test]
        public void ThumbDis_86F991C2()
        {
            AssertCode("@@@", "86F991C2");
        }
        // Reko: a decoder for the instruction 11FCC46D at address 001793AA has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 9EEE718B at address 001B521A has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction 7FFD79DC at address 00151A3A has not been implemented. (load PC)
        [Test]
        public void ThumbDis_7FFD79DC()
        {
            AssertCode("@@@", "7FFD79DC");
        }
        // Reko: a decoder for the instruction AAFD575C at address 001DC0B6 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_AAFD575C()
        {
            AssertCode("@@@", "AAFD575C");
        }
        // Reko: a decoder for the instruction 61FF8B74 at address 00151FE2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_61FF8B74()
        {
            AssertCode("@@@", "61FF8B74");
        }
        // Reko: a decoder for the instruction 92EE185B at address 001F1A58 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 3DFC752C at address 001195C4 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_3DFC752C()
        {
            AssertCode("@@@", "3DFC752C");
        }
        // Reko: a decoder for the instruction 86EC153B at address 001F1B04 has not been implemented. (Unimplemented '*' when decoding EC863B15)
 
        // Reko: a decoder for the instruction F0FE11B9 at address 0019109E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F0FE11B9()
        {
            AssertCode("@@@", "F0FE11B9");
        }
        // Reko: a decoder for the instruction 12FE6E9D at address 001910E0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_12FE6E9D()
        {
            AssertCode("@@@", "12FE6E9D");
        }
        // Reko: a decoder for the instruction 64FF61DC at address 0019122E has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_64FF61DC()
        {
            AssertCode("@@@", "64FF61DC");
        }
        // Reko: a decoder for the instruction 0EFF3669 at address 00191260 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_0EFF3669()
        {
            AssertCode("@@@", "0EFF3669");
        }
        // Reko: a decoder for the instruction 1CEFCD23 at address 0011F6C0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1CEFCD23()
        {
            AssertCode("@@@", "1CEFCD23");
        }
        // Reko: a decoder for the instruction 6CFE9519 at address 001EA986 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6CFE9519()
        {
            AssertCode("@@@", "6CFE9519");
        }
        // Reko: a decoder for the instruction 54FED42D at address 001F439C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_54FED42D()
        {
            AssertCode("@@@", "54FED42D");
        }
        // Reko: a decoder for the instruction ECEED7DA at address 001EAA94 has not been implemented. (Unimplemented '*' when decoding EEECDAD7)
        [Test]
        public void ThumbDis_ECEED7DA()
        {
            AssertCode("@@@", "ECEED7DA");
        }
        // Reko: a decoder for the instruction E8EFEA8A at address 001B523E has not been implemented. (Unimplemented '*' when decoding EFE88AEA)
        [Test]
        public void ThumbDis_E8EFEA8A()
        {
            AssertCode("@@@", "E8EFEA8A");
        }
        // Reko: a decoder for the instruction 44FD4499 at address 001195E0 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_44FD4499()
        {
            AssertCode("@@@", "44FD4499");
        }
        // Reko: a decoder for the instruction 12FD5A99 at address 00119FEC has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction FEFD68E9 at address 001916DE has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FEFD68E9()
        {
            AssertCode("@@@", "FEFD68E9");
        }
        // Reko: a decoder for the instruction 32F963AF at address 001CD09C has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_32F963AF()
        {
            AssertCode("@@@", "32F963AF");
        }
        // Reko: a decoder for the instruction A6FF60DA at address 0015F2DC has not been implemented. (Unimplemented '*' when decoding FFA6DA60)
        [Test]
        public void ThumbDis_A6FF60DA()
        {
            AssertCode("@@@", "A6FF60DA");
        }
        // Reko: a decoder for the instruction 80FE3D19 at address 0018AEC6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_80FE3D19()
        {
            AssertCode("@@@", "80FE3D19");
        }
        // Reko: a decoder for the instruction D9F30C8D at address 001EAAB2 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D9F30C8D()
        {
            AssertCode("@@@", "D9F30C8D");
        }
        // Reko: a decoder for the instruction DDEC91DB at address 0017945A has not been implemented. (Unimplemented '*' when decoding ECDDDB91)
        [Test]
        public void ThumbDis_DDEC91DB()
        {
            AssertCode("@@@", "DDEC91DB");
        }
        // Reko: a decoder for the instruction ADF96244 at address 001DC62A has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9AD4462)
        [Test]
        public void ThumbDis_ADF96244()
        {
            AssertCode("@@@", "ADF96244");
        }
        // Reko: a decoder for the instruction 8DFDF4DD at address 001B52E6 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8DFDF4DD()
        {
            AssertCode("@@@", "8DFDF4DD");
        }
        // Reko: a decoder for the instruction 66FC0C8C at address 0019D540 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_66FC0C8C()
        {
            AssertCode("@@@", "66FC0C8C");
        }
        // Reko: a decoder for the instruction D7EE7B1B at address 00149DB6 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 77FC5C3D at address 001797F4 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_77FC5C3D()
        {
            AssertCode("@@@", "77FC5C3D");
        }
        // Reko: a decoder for the instruction 39FF385F at address 001F228C has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_39FF385F()
        {
            AssertCode("@@@", "39FF385F");
        }
        // Reko: a decoder for the instruction 64FC9678 at address 00174C5C has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_64FC9678()
        {
            AssertCode("@@@", "64FC9678");
        }
        // Reko: a decoder for the instruction D2FC4BE9 at address 00174DC6 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D2FC4BE9()
        {
            AssertCode("@@@", "D2FC4BE9");
        }
        // Reko: a decoder for the instruction D7FA8675 at address 001916E6 has not been implemented. (crc32c-crc32cb)
        [Test]
        public void ThumbDis_D7FA8675()
        {
            AssertCode("@@@", "D7FA8675");
        }
        // Reko: a decoder for the instruction 06FD92DC at address 00191A0C has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_06FD92DC()
        {
            AssertCode("@@@", "06FD92DC");
        }
        // Reko: a decoder for the instruction 7BFCB529 at address 00191B1E has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7BFCB529()
        {
            AssertCode("@@@", "7BFCB529");
        }
        // Reko: a decoder for the instruction 8BF9F6F7 at address 001CD110 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F98BF7F6)
        [Test]
        public void ThumbDis_8BF9F6F7()
        {
            AssertCode("@@@", "8BF9F6F7");
        }
        // Reko: a decoder for the instruction 06FD94DD at address 001F453A has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_06FD94DD()
        {
            AssertCode("@@@", "06FD94DD");
        }
        // Reko: a decoder for the instruction C0FCCE48 at address 0018AEEC has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C0FCCE48()
        {
            AssertCode("@@@", "C0FCCE48");
        }
        // Reko: a decoder for the instruction F0EEBA0B at address 0019E2DA has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction 65FF7B84 at address 0019E486 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_65FF7B84()
        {
            AssertCode("@@@", "65FF7B84");
        }
        // Reko: a decoder for the instruction 24FFE723 at address 001DC8F0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_24FFE723()
        {
            AssertCode("@@@", "24FFE723");
        }
        // Reko: a decoder for the instruction AFF98B0C at address 001F2378 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_AFF98B0C()
        {
            AssertCode("@@@", "AFF98B0C");
        }
        // Reko: a decoder for the instruction 5AEF5DB9 at address 00174E60 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_5AEF5DB9()
        {
            AssertCode("@@@", "5AEF5DB9");
        }
        // Reko: a decoder for the instruction 00FCF0F8 at address 00191D86 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_00FCF0F8()
        {
            AssertCode("@@@", "00FCF0F8");
        }
        // Reko: a decoder for the instruction 9FFF14B8 at address 0015F3FE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_9FFF14B8()
        {
            AssertCode("@@@", "9FFF14B8");
        }
        // Reko: a decoder for the instruction 0EFC805C at address 001F45B2 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0EFC805C()
        {
            AssertCode("@@@", "0EFC805C");
        }
        // Reko: a decoder for the instruction 17EC8D7E at address 0018AF82 has not been implemented. (Unimplemented '*' when decoding EC177E8D)
        [Test]
        public void ThumbDis_17EC8D7E()
        {
            AssertCode("@@@", "17EC8D7E");
        }
        // Reko: a decoder for the instruction 17FE3C19 at address 0019839A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_17FE3C19()
        {
            AssertCode("@@@", "17FE3C19");
        }
        // Reko: a decoder for the instruction B8FCDD19 at address 001EB04C has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B8FCDD19()
        {
            AssertCode("@@@", "B8FCDD19");
        }
        // Reko: a decoder for the instruction 20FF8EE1 at address 0019E4B6 has not been implemented. (Unimplemented '*' when decoding FF20E18E)
        [Test]
        public void ThumbDis_20FF8EE1()
        {
            AssertCode("@@@", "20FF8EE1");
        }
        // Reko: a decoder for the instruction E7F368A3 at address 001DC996 has not been implemented. (Unimplemented '*banked register' when decoding F3E7A368)
        [Test]
        public void ThumbDis_E7F368A3()
        {
            AssertCode("@@@", "E7F368A3");
        }
        // Reko: a decoder for the instruction CDFAA8F0 at address 00179910 has not been implemented. (crc32-crc32w)
        [Test]
        public void ThumbDis_CDFAA8F0()
        {
            AssertCode("@@@", "CDFAA8F0");
        }
        // Reko: a decoder for the instruction 4DFFBC43 at address 001F23E0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_4DFFBC43()
        {
            AssertCode("@@@", "4DFFBC43");
        }
        // Reko: a decoder for the instruction 02FC0548 at address 001B6E1C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_02FC0548()
        {
            AssertCode("@@@", "02FC0548");
        }
        // Reko: a decoder for the instruction 55FE1289 at address 0015F434 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_55FE1289()
        {
            AssertCode("@@@", "55FE1289");
        }
        // Reko: a decoder for the instruction 35FF19E2 at address 001F46D2 has not been implemented. (Unimplemented '*' when decoding FF35E219)
        [Test]
        public void ThumbDis_35FF19E2()
        {
            AssertCode("@@@", "35FF19E2");
        }
        // Reko: a decoder for the instruction 94FAA56E at address 0015F5FC has not been implemented. (Unimplemented '*' when decoding FA946EA5)
        [Test]
        public void ThumbDis_94FAA56E()
        {
            AssertCode("@@@", "94FAA56E");
        }
        // Reko: a decoder for the instruction 50FEBEAC at address 001EB982 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_50FEBEAC()
        {
            AssertCode("@@@", "50FEBEAC");
        }
        // Reko: a decoder for the instruction CDFCB83D at address 0018B07A has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CDFCB83D()
        {
            AssertCode("@@@", "CDFCB83D");
        }
        // Reko: a decoder for the instruction 58FF027C at address 00175842 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_58FF027C()
        {
            AssertCode("@@@", "58FF027C");
        }
        // Reko: a decoder for the instruction 2AFF1EA2 at address 001F4CF0 has not been implemented. (Unimplemented '*' when decoding FF2AA21E)
        [Test]
        public void ThumbDis_2AFF1EA2()
        {
            AssertCode("@@@", "2AFF1EA2");
        }
        // Reko: a decoder for the instruction 6AFE70BD at address 001B8268 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6AFE70BD()
        {
            AssertCode("@@@", "6AFE70BD");
        }
        // Reko: a decoder for the instruction 56FF2823 at address 0019A3DC has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_56FF2823()
        {
            AssertCode("@@@", "56FF2823");
        }
        // Reko: a decoder for the instruction DDFE7EDC at address 0017A2C2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_DDFE7EDC()
        {
            AssertCode("@@@", "DDFE7EDC");
        }
        // Reko: a decoder for the instruction C3EFED3C at address 0018B0FC has not been implemented. (Unimplemented '*' when decoding EFC33CED)
        [Test]
        public void ThumbDis_C3EFED3C()
        {
            AssertCode("@@@", "C3EFED3C");
        }
        // Reko: a decoder for the instruction 41FDCE6D at address 00175ACA has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_41FDCE6D()
        {
            AssertCode("@@@", "41FDCE6D");
        }
        // Reko: a decoder for the instruction B0FE1099 at address 001DD574 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B0FE1099()
        {
            AssertCode("@@@", "B0FE1099");
        }
        // Reko: a decoder for the instruction 2FFCCE78 at address 00191EC0 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2FFCCE78()
        {
            AssertCode("@@@", "2FFCCE78");
        }
        // Reko: a decoder for the instruction 2DFFA344 at address 001DD600 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_2DFFA344()
        {
            AssertCode("@@@", "2DFFA344");
        }
        // Reko: a decoder for the instruction 57FFE641 at address 001B830C has not been implemented. (Unimplemented '*' when decoding FF5741E6)
        [Test]
        public void ThumbDis_57FFE641()
        {
            AssertCode("@@@", "57FFE641");
        }
        // Reko: a decoder for the instruction 12FFD213 at address 0019A3FE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_12FFD213()
        {
            AssertCode("@@@", "12FFD213");
        }
        // Reko: a decoder for the instruction 88EF6C93 at address 0017A2D6 has not been implemented. (Unimplemented '*' when decoding EF88936C)
        [Test]
        public void ThumbDis_88EF6C93()
        {
            AssertCode("@@@", "88EF6C93");
        }
        // Reko: a decoder for the instruction 4EFECEDD at address 001CD4BA has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_4EFECEDD()
        {
            AssertCode("@@@", "4EFECEDD");
        }
        // Reko: a decoder for the instruction E1F975DA at address 0018B1A0 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9E1DA75)
        [Test]
        public void ThumbDis_E1F975DA()
        {
            AssertCode("@@@", "E1F975DA");
        }
        // Reko: a decoder for the instruction B8EFD634 at address 00175B12 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_B8EFD634()
        {
            AssertCode("@@@", "B8EFD634");
        }
        // Reko: a decoder for the instruction F0F349A9 at address 00175BA6 has not been implemented. (Unimplemented '*register' when decoding F3F0A949)
        [Test]
        public void ThumbDis_F0F349A9()
        {
            AssertCode("@@@", "F0F349A9");
        }
        // Reko: a decoder for the instruction 41FD9448 at address 001DD60E has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_41FD9448()
        {
            AssertCode("@@@", "41FD9448");
        }
        // Reko: a decoder for the instruction DAFCB5BC at address 00191FA0 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DAFCB5BC()
        {
            AssertCode("@@@", "DAFCB5BC");
        }
        // Reko: a decoder for the instruction 05FEB02D at address 001F4ED0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_05FEB02D()
        {
            AssertCode("@@@", "05FEB02D");
        }
        // Reko: a decoder for the instruction D2EE92EB at address 0017A2F8 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction BCFC2BC8 at address 0019A508 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BCFC2BC8()
        {
            AssertCode("@@@", "BCFC2BC8");
        }
        // Reko: a decoder for the instruction 3CEE71F9 at address 001CD660 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_3CEE71F9()
        {
            AssertCode("@@@", "3CEE71F9");
        }
        // Reko: a decoder for the instruction 64FF2DEC at address 00175C2C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_64FF2DEC()
        {
            AssertCode("@@@", "64FF2DEC");
        }
        // Reko: a decoder for the instruction BFECFD1B at address 00191FF2 has not been implemented. (Unimplemented '*' when decoding ECBF1BFD)
        [Test]
        public void ThumbDis_BFECFD1B()
        {
            AssertCode("@@@", "BFECFD1B");
        }
        // Reko: a decoder for the instruction 2EFDA0A8 at address 001B83FA has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2EFDA0A8()
        {
            AssertCode("@@@", "2EFDA0A8");
        }
        // Reko: a decoder for the instruction E7FD5AD9 at address 0018B27E has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E7FD5AD9()
        {
            AssertCode("@@@", "E7FD5AD9");
        }
        // Reko: a decoder for the instruction 2CFF79C2 at address 001CD682 has not been implemented. (Unimplemented '*' when decoding FF2CC279)
        [Test]
        public void ThumbDis_2CFF79C2()
        {
            AssertCode("@@@", "2CFF79C2");
        }
        // Reko: a decoder for the instruction E8FCBB7D at address 001DD6CE has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E8FCBB7D()
        {
            AssertCode("@@@", "E8FCBB7D");
        }
        // Reko: a decoder for the instruction 08EF853E at address 0019A678 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_08EF853E()
        {
            AssertCode("@@@", "08EF853E");
        }
        // Reko: a decoder for the instruction FEFE34BD at address 00175CA6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_FEFE34BD()
        {
            AssertCode("@@@", "FEFE34BD");
        }
        // Reko: a decoder for the instruction 3FFF490B at address 0019205A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_3FFF490B()
        {
            AssertCode("@@@", "3FFF490B");
        }
        // Reko: a decoder for the instruction 3BFF90BC at address 001F4F44 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_3BFF90BC()
        {
            AssertCode("@@@", "3BFF90BC");
        }
        // Reko: a decoder for the instruction DCFC55A9 at address 001B8434 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DCFC55A9()
        {
            AssertCode("@@@", "DCFC55A9");
        }
        // Reko: a decoder for the instruction 9BEF49C9 at address 0018B818 has not been implemented. (Unimplemented '*scalar' when decoding EF9BC949)
        [Test]
        public void ThumbDis_9BEF49C9()
        {
            AssertCode("@@@", "9BEF49C9");
        }
        // Reko: a decoder for the instruction CFFC1DE9 at address 001924C0 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CFFC1DE9()
        {
            AssertCode("@@@", "CFFC1DE9");
        }
        // Reko: a decoder for the instruction CBEF4279 at address 0019A6BC has not been implemented. (Unimplemented '*scalar' when decoding EFCB7942)
        [Test]
        public void ThumbDis_CBEF4279()
        {
            AssertCode("@@@", "CBEF4279");
        }
        // Reko: a decoder for the instruction 19EF9ADF at address 001DD7BA has not been implemented. (Unimplemented '*' when decoding EF19DF9A)
        [Test]
        public void ThumbDis_19EF9ADF()
        {
            AssertCode("@@@", "19EF9ADF");
        }
        // Reko: a decoder for the instruction E5EC1A9B at address 001DD80C has not been implemented. (Unimplemented '*' when decoding ECE59B1A)
        [Test]
        public void ThumbDis_E5EC1A9B()
        {
            AssertCode("@@@", "E5EC1A9B");
        }
        // Reko: a decoder for the instruction 5BFF3A29 at address 00175CFC has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 29FF5D03 at address 001B84A6 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_29FF5D03()
        {
            AssertCode("@@@", "29FF5D03");
        }
        // Reko: a decoder for the instruction 60FFD76B at address 00175D68 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_60FFD76B()
        {
            AssertCode("@@@", "60FFD76B");
        }
        // Reko: a decoder for the instruction 4EFCF609 at address 00175DA8 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4EFCF609()
        {
            AssertCode("@@@", "4EFCF609");
        }
        // Reko: a decoder for the instruction 9BFF9AED at address 0018B828 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_9BFF9AED()
        {
            AssertCode("@@@", "9BFF9AED");
        }
        // Reko: a decoder for the instruction DAEF5394 at address 0018B85A has not been implemented. (U=0)
        [Test]
        public void ThumbDis_DAEF5394()
        {
            AssertCode("@@@", "DAEF5394");
        }
        // Reko: a decoder for the instruction ABF96C4F at address 0019A82C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_ABF96C4F()
        {
            AssertCode("@@@", "ABF96C4F");
        }
        // Reko: a decoder for the instruction 3CFE94DD at address 001DD810 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3CFE94DD()
        {
            AssertCode("@@@", "3CFE94DD");
        }
        // Reko: a decoder for the instruction A9F9C741 at address 001DD9C4 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9A941C7)
        [Test]
        public void ThumbDis_A9F9C741()
        {
            AssertCode("@@@", "A9F9C741");
        }
        // Reko: a decoder for the instruction 83F9C3F4 at address 00175DD8 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F983F4C3)
        [Test]
        public void ThumbDis_83F9C3F4()
        {
            AssertCode("@@@", "83F9C3F4");
        }
        // Reko: a decoder for the instruction CBEC194A at address 001B85D2 has not been implemented. (Unimplemented '*' when decoding ECCB4A19)
        [Test]
        public void ThumbDis_CBEC194A()
        {
            AssertCode("@@@", "CBEC194A");
        }
        // Reko: a decoder for the instruction 8AFF33CD at address 001B8602 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_8AFF33CD()
        {
            AssertCode("@@@", "8AFF33CD");
        }
        // Reko: a decoder for the instruction BDFFAD13 at address 0018BCEE has not been implemented. (Unimplemented '*' when decoding FFBD13AD)
        [Test]
        public void ThumbDis_BDFFAD13()
        {
            AssertCode("@@@", "BDFFAD13");
        }
        // Reko: a decoder for the instruction 85F92DCB at address 001F50C4 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F985CB2D)
        [Test]
        public void ThumbDis_85F92DCB()
        {
            AssertCode("@@@", "85F92DCB");
        }
        // Reko: a decoder for the instruction 7BFC2E39 at address 001DD9FE has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7BFC2E39()
        {
            AssertCode("@@@", "7BFC2E39");
        }
        // Reko: a decoder for the instruction 8CFE4C7D at address 001B919A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8CFE4C7D()
        {
            AssertCode("@@@", "8CFE4C7D");
        }
        // Reko: a decoder for the instruction ABF9666F at address 001DE7CC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_ABF9666F()
        {
            AssertCode("@@@", "ABF9666F");
        }
        // Reko: a decoder for the instruction CAFA973D at address 0019351A has not been implemented. (crc32-crc32h)
        [Test]
        public void ThumbDis_CAFA973D()
        {
            AssertCode("@@@", "CAFA973D");
        }
        // Reko: a decoder for the instruction ACEF52BD at address 0018BEEC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_ACEF52BD()
        {
            AssertCode("@@@", "ACEF52BD");
        }
        // Reko: a decoder for the instruction D9EE508B at address 0019366C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction CDFA90BA at address 001F6B08 has not been implemented. (crc32-crc32h)
        [Test]
        public void ThumbDis_CDFA90BA()
        {
            AssertCode("@@@", "CDFA90BA");
        }
        // Reko: a decoder for the instruction 77FF4E31 at address 001F6B10 has not been implemented. (Unimplemented '*' when decoding FF77314E)
        [Test]
        public void ThumbDis_77FF4E31()
        {
            AssertCode("@@@", "77FF4E31");
        }
        // Reko: a decoder for the instruction CEEF65A3 at address 001D3346 has not been implemented. (Unimplemented '*' when decoding EFCEA365)
        [Test]
        public void ThumbDis_CEEF65A3()
        {
            AssertCode("@@@", "CEEF65A3");
        }
        // Reko: a decoder for the instruction A2F94CCE at address 001D3542 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A2F94CCE()
        {
            AssertCode("@@@", "A2F94CCE");
        }
        // Reko: a decoder for the instruction 26FFF7D9 at address 001D3B52 has not been implemented. (*vmul (integer and polynomial)
 
        // Reko: a decoder for the instruction 9EFCE248 at address 00100272 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_9EFCE248()
        {
            AssertCode("@@@", "9EFCE248");
        }
        // Reko: a decoder for the instruction 53EFB3F9 at address 00100276 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_53EFB3F9()
        {
            AssertCode("@@@", "53EFB3F9");
        }
        // Reko: a decoder for the instruction C8F9CFEC at address 001408C2 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C8F9CFEC()
        {
            AssertCode("@@@", "C8F9CFEC");
        }
        // Reko: a decoder for the instruction 25EE79FB at address 001809A6 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_25EE79FB()
        {
            AssertCode("@@@", "25EE79FB");
        }
        // Reko: a decoder for the instruction 45FF784C at address 001609DC has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_45FF784C()
        {
            AssertCode("@@@", "45FF784C");
        }
        // Reko: a decoder for the instruction 09FF57E9 at address 001002A4 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_09FF57E9()
        {
            AssertCode("@@@", "09FF57E9");
        }
        // Reko: a decoder for the instruction 76FF774C at address 001209CC has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_76FF774C()
        {
            AssertCode("@@@", "76FF774C");
        }
        // Reko: a decoder for the instruction 68FF61B3 at address 00180DB4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_68FF61B3()
        {
            AssertCode("@@@", "68FF61B3");
        }
        // Reko: a decoder for the instruction 7CFD01A8 at address 001C0F82 has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_7CFD01A8()
        {
            AssertCode("@@@", "7CFD01A8");
        }
        // Reko: a decoder for the instruction EDFF55FB at address 001007B8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_EDFF55FB()
        {
            AssertCode("@@@", "EDFF55FB");
        }
        // Reko: a decoder for the instruction 84FCDAA9 at address 001414B4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_84FCDAA9()
        {
            AssertCode("@@@", "84FCDAA9");
        }
        // Reko: a decoder for the instruction 9FFC778E at address 00121270 has not been implemented. (load PC)
        [Test]
        public void ThumbDis_9FFC778E()
        {
            AssertCode("@@@", "9FFC778E");
        }
        // Reko: a decoder for the instruction DFFE912C at address 00181444 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_DFFE912C()
        {
            AssertCode("@@@", "DFFE912C");
        }
        // Reko: a decoder for the instruction DBFD6FED at address 00121576 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DBFD6FED()
        {
            AssertCode("@@@", "DBFD6FED");
        }
        // Reko: a decoder for the instruction 75EF4D53 at address 00181528 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_75EF4D53()
        {
            AssertCode("@@@", "75EF4D53");
        }
        // Reko: a decoder for the instruction A8FD9959 at address 001222D6 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A8FD9959()
        {
            AssertCode("@@@", "A8FD9959");
        }
        // Reko: a decoder for the instruction C2F9831C at address 0016030E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C2F9831C()
        {
            AssertCode("@@@", "C2F9831C");
        }
        // Reko: a decoder for the instruction 2AFD5FC9 at address 00120830 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2AFD5FC9()
        {
            AssertCode("@@@", "2AFD5FC9");
        }
        // Reko: a decoder for the instruction A5FD22EC at address 001606E2 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A5FD22EC()
        {
            AssertCode("@@@", "A5FD22EC");
        }
        // Reko: a decoder for the instruction A7F9AC0F at address 001C0664 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A7F9AC0F()
        {
            AssertCode("@@@", "A7F9AC0F");
        }
        // Reko: a decoder for the instruction 6CFEB28C at address 001C06A2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6CFEB28C()
        {
            AssertCode("@@@", "6CFEB28C");
        }
        // Reko: a decoder for the instruction 77FE1948 at address 00102594 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_77FE1948()
        {
            AssertCode("@@@", "77FE1948");
        }
        // Reko: a decoder for the instruction D2EF3ACC at address 001A09B8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_D2EF3ACC()
        {
            AssertCode("@@@", "D2EF3ACC");
        }
        // Reko: a decoder for the instruction 87F9266E at address 00120B20 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_87F9266E()
        {
            AssertCode("@@@", "87F9266E");
        }
        // Reko: a decoder for the instruction 80F9E8CD at address 001C06BA has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_80F9E8CD()
        {
            AssertCode("@@@", "80F9E8CD");
        }
        // Reko: a decoder for the instruction 80F92D41 at address 001A09D2 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F980412D)
        [Test]
        public void ThumbDis_80F92D41()
        {
            AssertCode("@@@", "80F92D41");
        }
        // Reko: a decoder for the instruction 2AFF4BFD at address 001806E8 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_2AFF4BFD()
        {
            AssertCode("@@@", "2AFF4BFD");
        }
        // Reko: a decoder for the instruction 46FF5CFB at address 00180708 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_46FF5CFB()
        {
            AssertCode("@@@", "46FF5CFB");
        }
        // Reko: a decoder for the instruction 21FDD288 at address 001C0AE4 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_21FDD288()
        {
            AssertCode("@@@", "21FDD288");
        }
        // Reko: a decoder for the instruction C1ECFF9A at address 00102B54 has not been implemented. (Unimplemented '*' when decoding ECC19AFF)
        [Test]
        public void ThumbDis_C1ECFF9A()
        {
            AssertCode("@@@", "C1ECFF9A");
        }
        // Reko: a decoder for the instruction 1EEB143F at address 001219FA has not been implemented. (Unimplemented '*register' when decoding EB1E3F14)
        [Test]
        public void ThumbDis_1EEB143F()
        {
            AssertCode("@@@", "1EEB143F");
        }
        // Reko: a decoder for the instruction 8DFE8C5D at address 001615AE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8DFE8C5D()
        {
            AssertCode("@@@", "8DFE8C5D");
        }
        // Reko: a decoder for the instruction 29FD7C48 at address 00180F68 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_29FD7C48()
        {
            AssertCode("@@@", "29FD7C48");
        }
        // Reko: a decoder for the instruction A4ECDA4A at address 0014172C has not been implemented. (Unimplemented '*' when decoding ECA44ADA)
        [Test]
        public void ThumbDis_A4ECDA4A()
        {
            AssertCode("@@@", "A4ECDA4A");
        }
        // Reko: a decoder for the instruction DDFA94AA at address 001A1AC6 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_DDFA94AA()
        {
            AssertCode("@@@", "DDFA94AA");
        }
        // Reko: a decoder for the instruction 86FFFE48 at address 0010155C has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_86FFFE48()
        {
            AssertCode("@@@", "86FFFE48");
        }
        // Reko: a decoder for the instruction 85FFF0B8 at address 00180F7A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_85FFF0B8()
        {
            AssertCode("@@@", "85FFF0B8");
        }
        // Reko: a decoder for the instruction 09EF8604 at address 001C20C6 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_09EF8604()
        {
            AssertCode("@@@", "09EF8604");
        }
        // Reko: a decoder for the instruction 08FFF81B at address 001A27BE has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_08FFF81B()
        {
            AssertCode("@@@", "08FFF81B");
        }
        // Reko: a decoder for the instruction 9EF336AE at address 001220DC has not been implemented. (Unimplemented '*banked register' when decoding F39EAE36)
        [Test]
        public void ThumbDis_9EF336AE()
        {
            AssertCode("@@@", "9EF336AE");
        }
        // Reko: a decoder for the instruction 0DFD8CDD at address 001E1648 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0DFD8CDD()
        {
            AssertCode("@@@", "0DFD8CDD");
        }
        // Reko: a decoder for the instruction 7AEFA9DC at address 00101692 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_7AEFA9DC()
        {
            AssertCode("@@@", "7AEFA9DC");
        }
        // Reko: a decoder for the instruction CEFF48AA at address 00103102 has not been implemented. (Unimplemented '*' when decoding FFCEAA48)
        [Test]
        public void ThumbDis_CEFF48AA()
        {
            AssertCode("@@@", "CEFF48AA");
        }
        // Reko: a decoder for the instruction 84F9161B at address 001A284E has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9841B16)
        [Test]
        public void ThumbDis_84F9161B()
        {
            AssertCode("@@@", "84F9161B");
        }
        // Reko: a decoder for the instruction EBF9D35D at address 0014211A has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_EBF9D35D()
        {
            AssertCode("@@@", "EBF9D35D");
        }
        // Reko: a decoder for the instruction F3FC29F8 at address 0016214C has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_F3FC29F8()
        {
            AssertCode("@@@", "F3FC29F8");
        }
        // Reko: a decoder for the instruction 5CFD4E8C at address 00181066 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction F6FD5BA9 at address 00101B1C has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F6FD5BA9()
        {
            AssertCode("@@@", "F6FD5BA9");
        }
        // Reko: a decoder for the instruction 64FEEC1C at address 001A2ADE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_64FEEC1C()
        {
            AssertCode("@@@", "64FEEC1C");
        }
        // Reko: a decoder for the instruction 75EF1411 at address 001C238E has not been implemented. (Unimplemented '*register' when decoding EF751114)
        [Test]
        public void ThumbDis_75EF1411()
        {
            AssertCode("@@@", "75EF1411");
        }
        // Reko: a decoder for the instruction 72FF5822 at address 001224B6 has not been implemented. (Unimplemented '*' when decoding FF722258)
        [Test]
        public void ThumbDis_72FF5822()
        {
            AssertCode("@@@", "72FF5822");
        }
        // Reko: a decoder for the instruction 85EC9F3A at address 001E1F52 has not been implemented. (Unimplemented '*' when decoding EC853A9F)
        [Test]
        public void ThumbDis_85EC9F3A()
        {
            AssertCode("@@@", "85EC9F3A");
        }
        // Reko: a decoder for the instruction B7FEC70D at address 00162208 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B7FEC70D()
        {
            AssertCode("@@@", "B7FEC70D");
        }
        // Reko: a decoder for the instruction 95EFC6BC at address 00101B9A has not been implemented. (Unimplemented '*' when decoding EF95BCC6)
        [Test]
        public void ThumbDis_95EFC6BC()
        {
            AssertCode("@@@", "95EFC6BC");
        }
        // Reko: a decoder for the instruction B1FCBE79 at address 00103A72 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B1FCBE79()
        {
            AssertCode("@@@", "B1FCBE79");
        }
        // Reko: a decoder for the instruction 01FF713B at address 00122562 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_01FF713B()
        {
            AssertCode("@@@", "01FF713B");
        }
        // Reko: a decoder for the instruction 75FF2393 at address 001E2082 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_75FF2393()
        {
            AssertCode("@@@", "75FF2393");
        }
        // Reko: a decoder for the instruction E4FF43D2 at address 0018120C has not been implemented. (Unimplemented '*scalar' when decoding FFE4D243)
        [Test]
        public void ThumbDis_E4FF43D2()
        {
            AssertCode("@@@", "E4FF43D2");
        }
        // Reko: a decoder for the instruction FFEF91BB at address 00101BEC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_FFEF91BB()
        {
            AssertCode("@@@", "FFEF91BB");
        }
        // Reko: a decoder for the instruction 0EFF33EF at address 001C25AA has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_0EFF33EF()
        {
            AssertCode("@@@", "0EFF33EF");
        }
        // Reko: a decoder for the instruction 65FE1BF9 at address 0016242E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_65FE1BF9()
        {
            AssertCode("@@@", "65FE1BF9");
        }
        // Reko: a decoder for the instruction D8F302A1 at address 00142A3C has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D8F302A1()
        {
            AssertCode("@@@", "D8F302A1");
        }
        // Reko: a decoder for the instruction EEF96408 at address 00181668 has not been implemented. (Unimplemented 'single element from one lane - T3' when decoding F9EE0864)
        [Test]
        public void ThumbDis_EEF96408()
        {
            AssertCode("@@@", "EEF96408");
        }
        // Reko: a decoder for the instruction CAFA94B6 at address 00122AD4 has not been implemented. (crc32-crc32h)
        [Test]
        public void ThumbDis_CAFA94B6()
        {
            AssertCode("@@@", "CAFA94B6");
        }
        // Reko: a decoder for the instruction E4EFB8CD at address 001C2A9A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_E4EFB8CD()
        {
            AssertCode("@@@", "E4EFB8CD");
        }
        // Reko: a decoder for the instruction EDFF680D at address 00122B9C has not been implemented. (Unimplemented '*' when decoding FFED0D68)
        [Test]
        public void ThumbDis_EDFF680D()
        {
            AssertCode("@@@", "EDFF680D");
        }
        // Reko: a decoder for the instruction B9FE6CED at address 00142A42 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B9FE6CED()
        {
            AssertCode("@@@", "B9FE6CED");
        }
        // Reko: a decoder for the instruction 55EF850E at address 001C2ADA has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_55EF850E()
        {
            AssertCode("@@@", "55EF850E");
        }
        // Reko: a decoder for the instruction CAFC6BC8 at address 00122C22 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CAFC6BC8()
        {
            AssertCode("@@@", "CAFC6BC8");
        }
        // Reko: a decoder for the instruction CAF95142 at address 001818F6 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9CA4251)
        [Test]
        public void ThumbDis_CAF95142()
        {
            AssertCode("@@@", "CAF95142");
        }
        // Reko: a decoder for the instruction DFFEE2ED at address 00142A54 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_DFFEE2ED()
        {
            AssertCode("@@@", "DFFEE2ED");
        }
        // Reko: a decoder for the instruction 8FFE07CC at address 00142B5C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8FFE07CC()
        {
            AssertCode("@@@", "8FFE07CC");
        }
        // Reko: a decoder for the instruction E8F9250C at address 00142B9E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E8F9250C()
        {
            AssertCode("@@@", "E8F9250C");
        }
        // Reko: a decoder for the instruction 3AFFE34B at address 00142D44 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_3AFFE34B()
        {
            AssertCode("@@@", "3AFFE34B");
        }
        // Reko: a decoder for the instruction 80FFC58C at address 00142D94 has not been implemented. (Unimplemented '*' when decoding FF808CC5)
        [Test]
        public void ThumbDis_80FFC58C()
        {
            AssertCode("@@@", "80FFC58C");
        }
        // Reko: a decoder for the instruction 12FCADED at address 00142FDE has not been implemented. (op1:op2=0b0001)
        
        // Reko: a decoder for the instruction C8FDB4D9 at address 001430F6 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C8FDB4D9()
        {
            AssertCode("@@@", "C8FDB4D9");
        }
        // Reko: a decoder for the instruction 47FDD91C at address 00181AC8 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_47FDD91C()
        {
            AssertCode("@@@", "47FDD91C");
        }
        // Reko: a decoder for the instruction E3FEADDC at address 001E23A6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E3FEADDC()
        {
            AssertCode("@@@", "E3FEADDC");
        }
        // Reko: a decoder for the instruction 27FE3A8D at address 00162672 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_27FE3A8D()
        {
            AssertCode("@@@", "27FE3A8D");
        }
        // Reko: a decoder for the instruction 72FF8BE4 at address 001626BE has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_72FF8BE4()
        {
            AssertCode("@@@", "72FF8BE4");
        }
        // Reko: a decoder for the instruction 0FFF1A13 at address 001A388C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_0FFF1A13()
        {
            AssertCode("@@@", "0FFF1A13");
        }
        // Reko: a decoder for the instruction 54EF311C at address 00143442 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_54EF311C()
        {
            AssertCode("@@@", "54EF311C");
        }
        // Reko: a decoder for the instruction B2FD5D19 at address 001C367A has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B2FD5D19()
        {
            AssertCode("@@@", "B2FD5D19");
        }
        // Reko: a decoder for the instruction 54EFE7A3 at address 001231EE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_54EFE7A3()
        {
            AssertCode("@@@", "54EFE7A3");
        }
        // Reko: a decoder for the instruction 24FC6CDD at address 00101E6A has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_24FC6CDD()
        {
            AssertCode("@@@", "24FC6CDD");
        }
        // Reko: a decoder for the instruction DFFFE9AA at address 001040E0 has not been implemented. (Unimplemented '*' when decoding FFDFAAE9)
        [Test]
        public void ThumbDis_DFFFE9AA()
        {
            AssertCode("@@@", "DFFFE9AA");
        }
        // Reko: a decoder for the instruction 40FE86CC at address 00123658 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_40FE86CC()
        {
            AssertCode("@@@", "40FE86CC");
        }
        // Reko: a decoder for the instruction 83ECD2DB at address 00101F0E has not been implemented. (Unimplemented '*' when decoding EC83DBD2)
        [Test]
        public void ThumbDis_83ECD2DB()
        {
            AssertCode("@@@", "83ECD2DB");
        }
        // Reko: a decoder for the instruction 9FFC848C at address 001043E6 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_9FFC848C()
        {
            AssertCode("@@@", "9FFC848C");
        }
        // Reko: a decoder for the instruction 96F9E3F2 at address 001A40B4 has not been implemented. (PLI)
        [Test]
        public void ThumbDis_96F9E3F2()
        {
            AssertCode("@@@", "96F9E3F2");
        }
        // Reko: a decoder for the instruction 15FE529C at address 0014375C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_15FE529C()
        {
            AssertCode("@@@", "15FE529C");
        }
        // Reko: a decoder for the instruction CFF9E8EF at address 00181DBC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CFF9E8EF()
        {
            AssertCode("@@@", "CFF9E8EF");
        }
        // Reko: a decoder for the instruction D6EE91FB at address 00101F96 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 27FFEFBD at address 001C3CEA has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_27FFEFBD()
        {
            AssertCode("@@@", "27FFEFBD");
        }
        // Reko: a decoder for the instruction BBEF5E5D at address 00123898 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_BBEF5E5D()
        {
            AssertCode("@@@", "BBEF5E5D");
        }
        // Reko: a decoder for the instruction CBF3278E at address 001080AA has not been implemented. (Unimplemented '*' when decoding F3CB8E27)
        [Test]
        public void ThumbDis_CBF3278E()
        {
            AssertCode("@@@", "CBF3278E");
        }
        // Reko: a decoder for the instruction 41FD48FD at address 00123964 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_41FD48FD()
        {
            AssertCode("@@@", "41FD48FD");
        }
        // Reko: a decoder for the instruction 56EF492E at address 001C3D9C has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_56EF492E()
        {
            AssertCode("@@@", "56EF492E");
        }
        // Reko: a decoder for the instruction 9FEF678B at address 001E2DDC has not been implemented. (Unimplemented '*' when decoding EF9F8B67)
        [Test]
        public void ThumbDis_9FEF678B()
        {
            AssertCode("@@@", "9FEF678B");
        }
        // Reko: a decoder for the instruction 0DEE1F89 at address 001080B2 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_0DEE1F89()
        {
            AssertCode("@@@", "0DEE1F89");
        }
        // Reko: a decoder for the instruction 90EC573F at address 001631F4 has not been implemented. (Unimplemented '*' when decoding EC903F57)
        [Test]
        public void ThumbDis_90EC573F()
        {
            AssertCode("@@@", "90EC573F");
        }
        // Reko: a decoder for the instruction 6DFFC0CB at address 0016323A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_6DFFC0CB()
        {
            AssertCode("@@@", "6DFFC0CB");
        }
        // Reko: a decoder for the instruction ADFF3A68 at address 001C3E78 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_ADFF3A68()
        {
            AssertCode("@@@", "ADFF3A68");
        }
        // Reko: a decoder for the instruction F1FED9E9 at address 00123A28 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F1FED9E9()
        {
            AssertCode("@@@", "F1FED9E9");
        }
        // Reko: a decoder for the instruction D3FFE7A4 at address 00108452 has not been implemented. (Unimplemented '*scalar' when decoding FFD3A4E7)
        [Test]
        public void ThumbDis_D3FFE7A4()
        {
            AssertCode("@@@", "D3FFE7A4");
        }
        // Reko: a decoder for the instruction 6AEED2AB at address 001A4C54 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_6AEED2AB()
        {
            AssertCode("@@@", "6AEED2AB");
        }
        // Reko: a decoder for the instruction 1CEF39F4 at address 00163A96 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_1CEF39F4()
        {
            AssertCode("@@@", "1CEF39F4");
        }
        // Reko: a decoder for the instruction 54EFD2FD at address 00143F14 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_54EFD2FD()
        {
            AssertCode("@@@", "54EFD2FD");
        }
        // Reko: a decoder for the instruction EEF9785D at address 001A5998 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_EEF9785D()
        {
            AssertCode("@@@", "EEF9785D");
        }
        // Reko: a decoder for the instruction 27FD25CC at address 00104DEC has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_27FD25CC()
        {
            AssertCode("@@@", "27FD25CC");
        }
        // Reko: a decoder for the instruction AEFF770B at address 001E4AE4 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_AEFF770B()
        {
            AssertCode("@@@", "AEFF770B");
        }
        // Reko: a decoder for the instruction D4EFC854 at address 00144356 has not been implemented. (Unimplemented '*scalar' when decoding EFD454C8)
        [Test]
        public void ThumbDis_D4EFC854()
        {
            AssertCode("@@@", "D4EFC854");
        }
        // Reko: a decoder for the instruction 8CF90D7E at address 001087FC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8CF90D7E()
        {
            AssertCode("@@@", "8CF90D7E");
        }
        // Reko: a decoder for the instruction 3CFFAF14 at address 001A6524 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_3CFFAF14()
        {
            AssertCode("@@@", "3CFFAF14");
        }
        // Reko: a decoder for the instruction 48EFBC34 at address 001E4C04 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_48EFBC34()
        {
            AssertCode("@@@", "48EFBC34");
        }
        // Reko: a decoder for the instruction F4FC438C at address 001055F6 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_F4FC438C()
        {
            AssertCode("@@@", "F4FC438C");
        }
        // Reko: a decoder for the instruction F2EFD43C at address 0012498E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_F2EFD43C()
        {
            AssertCode("@@@", "F2EFD43C");
        }
        // Reko: a decoder for the instruction A5EFEE8A at address 001249DE has not been implemented. (Unimplemented '*' when decoding EFA58AEE)
        [Test]
        public void ThumbDis_A5EFEE8A()
        {
            AssertCode("@@@", "A5EFEE8A");
        }
        // Reko: a decoder for the instruction ACEF3C48 at address 00124A10 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_ACEF3C48()
        {
            AssertCode("@@@", "ACEF3C48");
        }
        // Reko: a decoder for the instruction 5BFFDDAF at address 00164942 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_5BFFDDAF()
        {
            AssertCode("@@@", "5BFFDDAF");
        }
        // Reko: a decoder for the instruction 53FF98E9 at address 001E4CB0 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction AEEEBAC9 at address 0018300E has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction C5F9E132 at address 001056B4 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9C532E1)
        [Test]
        public void ThumbDis_C5F9E132()
        {
            AssertCode("@@@", "C5F9E132");
        }
        // Reko: a decoder for the instruction 68FC36FC at address 00109618 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_68FC36FC()
        {
            AssertCode("@@@", "68FC36FC");
        }
        // Reko: a decoder for the instruction DAF395A4 at address 00144BB2 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_DAF395A4()
        {
            AssertCode("@@@", "DAF395A4");
        }
        // Reko: a decoder for the instruction 1EFD9EA9 at address 00165132 has not been implemented. (op1:op2=0b1001)
 
        // Reko: a decoder for the instruction 7BEF9172 at address 0016523E has not been implemented. (Unimplemented '*' when decoding EF7B7291)
        [Test]
        public void ThumbDis_7BEF9172()
        {
            AssertCode("@@@", "7BEF9172");
        }
        // Reko: a decoder for the instruction CFFF5C9C at address 001832D8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_CFFF5C9C()
        {
            AssertCode("@@@", "CFFF5C9C");
        }
        // Reko: a decoder for the instruction 98EF70EB at address 001097AE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_98EF70EB()
        {
            AssertCode("@@@", "98EF70EB");
        }
        // Reko: a decoder for the instruction 01FF5CA9 at address 0010593C has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_01FF5CA9()
        {
            AssertCode("@@@", "01FF5CA9");
        }
        // Reko: a decoder for the instruction 89FC8989 at address 001C5FBC has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_89FC8989()
        {
            AssertCode("@@@", "89FC8989");
        }
        // Reko: a decoder for the instruction BCFDB80C at address 001C5FEA has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_BCFDB80C()
        {
            AssertCode("@@@", "BCFDB80C");
        }
        // Reko: a decoder for the instruction 8AFF4B95 at address 0016538A has not been implemented. (Unimplemented '*scalar' when decoding FF8A954B)
        [Test]
        public void ThumbDis_8AFF4B95()
        {
            AssertCode("@@@", "8AFF4B95");
        }
        // Reko: a decoder for the instruction 73FCDA18 at address 00105974 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_73FCDA18()
        {
            AssertCode("@@@", "73FCDA18");
        }
        // Reko: a decoder for the instruction 5DFD88FC at address 001258C2 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 9AF944FC at address 001258DE has not been implemented. (PLI)
        [Test]
        public void ThumbDis_9AF944FC()
        {
            AssertCode("@@@", "9AF944FC");
        }
        // Reko: a decoder for the instruction 05FDCC8C at address 001C698E has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_05FDCC8C()
        {
            AssertCode("@@@", "05FDCC8C");
        }
        // Reko: a decoder for the instruction 13ECDA8F at address 001659BA has not been implemented. (Unimplemented '*' when decoding EC138FDA)
        [Test]
        public void ThumbDis_13ECDA8F()
        {
            AssertCode("@@@", "13ECDA8F");
        }
        // Reko: a decoder for the instruction 44FC737D at address 001258E0 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_44FC737D()
        {
            AssertCode("@@@", "44FC737D");
        }
        // Reko: a decoder for the instruction C7EFA119 at address 0010A104 has not been implemented. (Unimplemented '*integer' when decoding EFC719A1)
        [Test]
        public void ThumbDis_C7EFA119()
        {
            AssertCode("@@@", "C7EFA119");
        }
        // Reko: a decoder for the instruction 87FE884C at address 00106046 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_87FE884C()
        {
            AssertCode("@@@", "87FE884C");
        }
        // Reko: a decoder for the instruction B3FDBA3D at address 001837C2 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B3FDBA3D()
        {
            AssertCode("@@@", "B3FDBA3D");
        }
        // Reko: a decoder for the instruction DDFA96D3 at address 001C6BF8 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_DDFA96D3()
        {
            AssertCode("@@@", "DDFA96D3");
        }
        // Reko: a decoder for the instruction 4FFF01FB at address 001C6C12 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_4FFF01FB()
        {
            AssertCode("@@@", "4FFF01FB");
        }
        // Reko: a decoder for the instruction 58EF942E at address 0010A128 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_58EF942E()
        {
            AssertCode("@@@", "58EF942E");
        }
        // Reko: a decoder for the instruction F8FF7959 at address 00146006 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FFF85979)
        [Test]
        public void ThumbDis_F8FF7959()
        {
            AssertCode("@@@", "F8FF7959");
        }
        // Reko: a decoder for the instruction ABFF7F5C at address 00125BFE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_ABFF7F5C()
        {
            AssertCode("@@@", "ABFF7F5C");
        }
        // Reko: a decoder for the instruction ACFC098C at address 0018381A has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_ACFC098C()
        {
            AssertCode("@@@", "ACFC098C");
        }
        // Reko: a decoder for the instruction E5F98350 at address 0010A708 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9E55083)
        [Test]
        public void ThumbDis_E5F98350()
        {
            AssertCode("@@@", "E5F98350");
        }
        // Reko: a decoder for the instruction A4FDE7D9 at address 001C6C5C has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A4FDE7D9()
        {
            AssertCode("@@@", "A4FDE7D9");
        }
        // Reko: a decoder for the instruction 2AFF1C02 at address 001065C4 has not been implemented. (Unimplemented '*' when decoding FF2A021C)
        [Test]
        public void ThumbDis_2AFF1C02()
        {
            AssertCode("@@@", "2AFF1C02");
        }
        // Reko: a decoder for the instruction C6FF4888 at address 00125F62 has not been implemented. (Unimplemented '*scalar' when decoding FFC68848)
        [Test]
        public void ThumbDis_C6FF4888()
        {
            AssertCode("@@@", "C6FF4888");
        }
        // Reko: a decoder for the instruction 4CEFE5E3 at address 001E67A6 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_4CEFE5E3()
        {
            AssertCode("@@@", "4CEFE5E3");
        }
        // Reko: a decoder for the instruction 5EFF4953 at address 00183D90 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_5EFF4953()
        {
            AssertCode("@@@", "5EFF4953");
        }
        // Reko: a decoder for the instruction 86FFCCCA at address 001E721C has not been implemented. (Unimplemented '*' when decoding FF86CACC)
        [Test]
        public void ThumbDis_86FFCCCA()
        {
            AssertCode("@@@", "86FFCCCA");
        }
        // Reko: a decoder for the instruction D3FF9BCC at address 00126360 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_D3FF9BCC()
        {
            AssertCode("@@@", "D3FF9BCC");
        }
        // Reko: a decoder for the instruction E5EF83BB at address 001C78C0 has not been implemented. (Unimplemented '*integer' when decoding EFE5BB83)
        [Test]
        public void ThumbDis_E5EF83BB()
        {
            AssertCode("@@@", "E5EF83BB");
        }
        // Reko: a decoder for the instruction D0EF3414 at address 0012645A has not been implemented. (U=0)
        [Test]
        public void ThumbDis_D0EF3414()
        {
            AssertCode("@@@", "D0EF3414");
        }
        // Reko: a decoder for the instruction C1FC6999 at address 001675DE has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C1FC6999()
        {
            AssertCode("@@@", "C1FC6999");
        }
        // Reko: a decoder for the instruction 8FEF7E84 at address 001C794C has not been implemented. (U=0)
        [Test]
        public void ThumbDis_8FEF7E84()
        {
            AssertCode("@@@", "8FEF7E84");
        }
        // Reko: a decoder for the instruction D5F3D18D at address 0010B6AC has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D5F3D18D()
        {
            AssertCode("@@@", "D5F3D18D");
        }
        // Reko: a decoder for the instruction 08FEF84C at address 001E7C2E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_08FEF84C()
        {
            AssertCode("@@@", "08FEF84C");
        }
        // Reko: a decoder for the instruction E6FEB91C at address 001A9576 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E6FEB91C()
        {
            AssertCode("@@@", "E6FEB91C");
        }
        // Reko: a decoder for the instruction 87EE306B at address 001854C4 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_87EE306B()
        {
            AssertCode("@@@", "87EE306B");
        }
        // Reko: a decoder for the instruction 27FF8C74 at address 001E7C6A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_27FF8C74()
        {
            AssertCode("@@@", "27FF8C74");
        }
        // Reko: a decoder for the instruction FCEF187C at address 00147068 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_FCEF187C()
        {
            AssertCode("@@@", "FCEF187C");
        }
        // Reko: a decoder for the instruction B1EFB618 at address 001075C8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_B1EFB618()
        {
            AssertCode("@@@", "B1EFB618");
        }
        // Reko: a decoder for the instruction ECEF5219 at address 0010B844 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding EFEC1952)
        [Test]
        public void ThumbDis_ECEF5219()
        {
            AssertCode("@@@", "ECEF5219");
        }
        // Reko: a decoder for the instruction 9CEF581C at address 0010B8F4 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_9CEF581C()
        {
            AssertCode("@@@", "9CEF581C");
        }
        // Reko: a decoder for the instruction 97EFCA9F at address 001854D8 has not been implemented. (Unimplemented '*' when decoding EF979FCA)
        [Test]
        public void ThumbDis_97EFCA9F()
        {
            AssertCode("@@@", "97EFCA9F");
        }
        // Reko: a decoder for the instruction E1F3E5A2 at address 001E7D50 has not been implemented. (Unimplemented '*banked register' when decoding F3E1A2E5)
        
        // Reko: a decoder for the instruction D4FF9C8D at address 001E801A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_D4FF9C8D()
        {
            AssertCode("@@@", "D4FF9C8D");
        }
        // Reko: a decoder for the instruction 29FC786D at address 001E8048 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_29FC786D()
        {
            AssertCode("@@@", "29FC786D");
        }
        // Reko: a decoder for the instruction BAFEC33C at address 001C837A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_BAFEC33C()
        {
            AssertCode("@@@", "BAFEC33C");
        }
        // Reko: a decoder for the instruction 46EFB544 at address 00107EB4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_46EFB544()
        {
            AssertCode("@@@", "46EFB544");
        }
        // Reko: a decoder for the instruction ACFD0579 at address 001A97E0 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_ACFD0579()
        {
            AssertCode("@@@", "ACFD0579");
        }
        // Reko: a decoder for the instruction D5FA82CA at address 001A9808 has not been implemented. (crc32c-crc32cb)
        [Test]
        public void ThumbDis_D5FA82CA()
        {
            AssertCode("@@@", "D5FA82CA");
        }
        // Reko: a decoder for the instruction 5EFF6D21 at address 001A984C has not been implemented. (Unimplemented '*' when decoding FF5E216D)
        [Test]
        public void ThumbDis_5EFF6D21()
        {
            AssertCode("@@@", "5EFF6D21");
        }
        // Reko: a decoder for the instruction 49FC1C5D at address 001A98D4 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_49FC1C5D()
        {
            AssertCode("@@@", "49FC1C5D");
        }
        // Reko: a decoder for the instruction CCFEBDCC at address 001A9A88 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_CCFEBDCC()
        {
            AssertCode("@@@", "CCFEBDCC");
        }
        // Reko: a decoder for the instruction 47FD0928 at address 001A9A9A has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_47FD0928()
        {
            AssertCode("@@@", "47FD0928");
        }
        // Reko: a decoder for the instruction EEFDCBBC at address 001A9AD8 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_EEFDCBBC()
        {
            AssertCode("@@@", "EEFDCBBC");
        }
        // Reko: a decoder for the instruction A1F99974 at address 0010B966 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9A17499)
        [Test]
        public void ThumbDis_A1F99974()
        {
            AssertCode("@@@", "A1F99974");
        }
        // Reko: a decoder for the instruction C0EF837B at address 00147366 has not been implemented. (Unimplemented '*integer' when decoding EFC07B83)
        [Test]
        public void ThumbDis_C0EF837B()
        {
            AssertCode("@@@", "C0EF837B");
        }
        // Reko: a decoder for the instruction A2FD929C at address 001685BC has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A2FD929C()
        {
            AssertCode("@@@", "A2FD929C");
        }
        // Reko: a decoder for the instruction 51FD6F3D at address 001C84F4 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 48EFA9DB at address 001C8510 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_48EFA9DB()
        {
            AssertCode("@@@", "48EFA9DB");
        }
        // Reko: a decoder for the instruction 2BFE397C at address 00107FB2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_2BFE397C()
        {
            AssertCode("@@@", "2BFE397C");
        }
        // Reko: a decoder for the instruction 62FE6C7C at address 001A9B6E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_62FE6C7C()
        {
            AssertCode("@@@", "62FE6C7C");
        }
        // Reko: a decoder for the instruction EEFCE90D at address 001482D2 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EEFCE90D()
        {
            AssertCode("@@@", "EEFCE90D");
        }
        // Reko: a decoder for the instruction 39EFC9D1 at address 001C8EE2 has not been implemented. (Unimplemented '*' when decoding EF39D1C9)
        [Test]
        public void ThumbDis_39EFC9D1()
        {
            AssertCode("@@@", "39EFC9D1");
        }
        // Reko: a decoder for the instruction 94EF818D at address 00169252 has not been implemented. (Unimplemented '*integer' when decoding EF948D81)
        [Test]
        public void ThumbDis_94EF818D()
        {
            AssertCode("@@@", "94EF818D");
        }
        // Reko: a decoder for the instruction 18FC81CC at address 0010C9AE has not been implemented. (op1:op2=0b0001)
   
        // Reko: a decoder for the instruction 5AEF2ACE at address 00186640 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_5AEF2ACE()
        {
            AssertCode("@@@", "5AEF2ACE");
        }
        // Reko: a decoder for the instruction 1FFF9B59 at address 0014837A has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 7FFC3C8C at address 00114580 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7FFC3C8C()
        {
            AssertCode("@@@", "7FFC3C8C");
        }
        // Reko: a decoder for the instruction A5EFA32D at address 001A9D52 has not been implemented. (Unimplemented '*integer' when decoding EFA52DA3)
        [Test]
        public void ThumbDis_A5EFA32D()
        {
            AssertCode("@@@", "A5EFA32D");
        }
        // Reko: a decoder for the instruction 15FF8143 at address 001E8E14 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_15FF8143()
        {
            AssertCode("@@@", "15FF8143");
        }
        // Reko: a decoder for the instruction 81FDF668 at address 001A9DCE has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_81FDF668()
        {
            AssertCode("@@@", "81FDF668");
        }
        // Reko: a decoder for the instruction 85FF194D at address 00169746 has not been implemented. (Unimplemented '*immediate - T4' when decoding FF854D19)
 
        // Reko: a decoder for the instruction 9DFFC5A2 at address 00186712 has not been implemented. (Unimplemented '*scalar' when decoding FF9DA2C5)
        [Test]
        public void ThumbDis_9DFFC5A2()
        {
            AssertCode("@@@", "9DFFC5A2");
        }
        // Reko: a decoder for the instruction C4FE739D at address 001C9670 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C4FE739D()
        {
            AssertCode("@@@", "C4FE739D");
        }
        // Reko: a decoder for the instruction A4FE51CC at address 0011459A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A4FE51CC()
        {
            AssertCode("@@@", "A4FE51CC");
        }
        // Reko: a decoder for the instruction CFEF6DD8 at address 001C9AFA has not been implemented. (Unimplemented '*scalar' when decoding EFCFD86D)
        [Test]
        public void ThumbDis_CFEF6DD8()
        {
            AssertCode("@@@", "CFEF6DD8");
        }
        // Reko: a decoder for the instruction 07FFA681 at address 00169BAA has not been implemented. (Unimplemented '*' when decoding FF0781A6)
        [Test]
        public void ThumbDis_07FFA681()
        {
            AssertCode("@@@", "07FFA681");
        }
        // Reko: a decoder for the instruction 8AFD51ED at address 001A9DFE has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8AFD51ED()
        {
            AssertCode("@@@", "8AFD51ED");
        }
        // Reko: a decoder for the instruction EEEFDC34 at address 00148450 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_EEEFDC34()
        {
            AssertCode("@@@", "EEEFDC34");
        }
        // Reko: a decoder for the instruction 7FFF59BE at address 0010D29A has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1)
        [Test]
        public void ThumbDis_7FFF59BE()
        {
            AssertCode("@@@", "7FFF59BE");
        }
        // Reko: a decoder for the instruction DBE8DDFB at address 001280D6 has not been implemented. (Unimplemented '*' when decoding E8DBFBDD)
        [Test]
        public void ThumbDis_DBE8DDFB()
        {
            AssertCode("@@@", "DBE8DDFB");
        }
        // Reko: a decoder for the instruction 59EFA751 at address 001AA668 has not been implemented. (Unimplemented '*' when decoding EF5951A7)
        [Test]
        public void ThumbDis_59EFA751()
        {
            AssertCode("@@@", "59EFA751");
        }
        // Reko: a decoder for the instruction 41EF99F9 at address 001AA6CA has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_41EF99F9()
        {
            AssertCode("@@@", "41EF99F9");
        }
        // Reko: a decoder for the instruction 52FCB99C at address 001E956C has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction BDFDECBD at address 0016A388 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_BDFDECBD()
        {
            AssertCode("@@@", "BDFDECBD");
        }
        // Reko: a decoder for the instruction 18FDB188 at address 001E9896 has not been implemented. (op1:op2=0b1001)
    
        // Reko: a decoder for the instruction 7AF90B0F at address 0010D392 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_7AF90B0F()
        {
            AssertCode("@@@", "7AF90B0F");
        }
        // Reko: a decoder for the instruction CEEF6288 at address 0010D496 has not been implemented. (Unimplemented '*scalar' when decoding EFCE8862)
        [Test]
        public void ThumbDis_CEEF6288()
        {
            AssertCode("@@@", "CEEF6288");
        }
        // Reko: a decoder for the instruction B8FD81E9 at address 00148A9A has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B8FD81E9()
        {
            AssertCode("@@@", "B8FD81E9");
        }
        // Reko: a decoder for the instruction 28FED7ED at address 00148AB2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_28FED7ED()
        {
            AssertCode("@@@", "28FED7ED");
        }
        // Reko: a decoder for the instruction 7AF94E4E at address 00186BEE has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_7AF94E4E()
        {
            AssertCode("@@@", "7AF94E4E");
        }
        // Reko: a decoder for the instruction A2F9FF74 at address 0010D6F0 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9A274FF)
        [Test]
        public void ThumbDis_A2F9FF74()
        {
            AssertCode("@@@", "A2F9FF74");
        }
        // Reko: a decoder for the instruction 60FDD61D at address 00148ABA has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_60FDD61D()
        {
            AssertCode("@@@", "60FDD61D");
        }
        // Reko: a decoder for the instruction 4EEF7E3E at address 0016ABD2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_4EEF7E3E()
        {
            AssertCode("@@@", "4EEF7E3E");
        }
        // Reko: a decoder for the instruction 21EF304B at address 0010D74C has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_21EF304B()
        {
            AssertCode("@@@", "21EF304B");
        }
        // Reko: a decoder for the instruction 89FC5FCD at address 00129002 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_89FC5FCD()
        {
            AssertCode("@@@", "89FC5FCD");
        }
        // Reko: a decoder for the instruction 10FFA06C at address 0016B040 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_10FFA06C()
        {
            AssertCode("@@@", "10FFA06C");
        }
        // Reko: a decoder for the instruction D0EFEE88 at address 001E9F84 has not been implemented. (Unimplemented '*scalar' when decoding EFD088EE)
        [Test]
        public void ThumbDis_D0EFEE88()
        {
            AssertCode("@@@", "D0EFEE88");
        }
        // Reko: a decoder for the instruction E0F99FAF at address 0010D902 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E0F99FAF()
        {
            AssertCode("@@@", "E0F99FAF");
        }
        // Reko: a decoder for the instruction 47EFBA91 at address 00115968 has not been implemented. (Unimplemented '*register' when decoding EF4791BA)
        [Test]
        public void ThumbDis_47EFBA91()
        {
            AssertCode("@@@", "47EFBA91");
        }
        // Reko: a decoder for the instruction 67FD5418 at address 001EA2FC has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_67FD5418()
        {
            AssertCode("@@@", "67FD5418");
        }
        // Reko: a decoder for the instruction 1CFDBA38 at address 001EA35E has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction FEEFB85B at address 0016B0AA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_FEEFB85B()
        {
            AssertCode("@@@", "FEEFB85B");
        }
        // Reko: a decoder for the instruction C3FF4435 at address 0016B152 has not been implemented. (Unimplemented '*scalar' when decoding FFC33544)
        [Test]
        public void ThumbDis_C3FF4435()
        {
            AssertCode("@@@", "C3FF4435");
        }
        // Reko: a decoder for the instruction 3BEF2701 at address 0016B18A has not been implemented. (Unimplemented '*' when decoding EF3B0127)
        [Test]
        public void ThumbDis_3BEF2701()
        {
            AssertCode("@@@", "3BEF2701");
        }
        // Reko: a decoder for the instruction 9DFC2BBD at address 001CAFE8 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_9DFC2BBD()
        {
            AssertCode("@@@", "9DFC2BBD");
        }
        // Reko: a decoder for the instruction C9EFEFF7 at address 001AB290 has not been implemented. (Unimplemented '*' when decoding EFC9F7EF)
        [Test]
        public void ThumbDis_C9EFEFF7()
        {
            AssertCode("@@@", "C9EFEFF7");
        }
        // Reko: a decoder for the instruction 99FD6C68 at address 0010D920 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_99FD6C68()
        {
            AssertCode("@@@", "99FD6C68");
        }
        // Reko: a decoder for the instruction BEFC402D at address 00115B12 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BEFC402D()
        {
            AssertCode("@@@", "BEFC402D");
        }
        // Reko: a decoder for the instruction D6FAA2C8 at address 00148D54 has not been implemented. (crc32c-crc32cw)
        [Test]
        public void ThumbDis_D6FAA2C8()
        {
            AssertCode("@@@", "D6FAA2C8");
        }
        // Reko: a decoder for the instruction 40EF5263 at address 0016B242 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_40EF5263()
        {
            AssertCode("@@@", "40EF5263");
        }
        // Reko: a decoder for the instruction 65EFE3EC at address 001CAFF6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_65EFE3EC()
        {
            AssertCode("@@@", "65EFE3EC");
        }
        // Reko: a decoder for the instruction A6EFC96D at address 001292E2 has not been implemented. (Unimplemented '*' when decoding EFA66DC9)
        [Test]
        public void ThumbDis_A6EFC96D()
        {
            AssertCode("@@@", "A6EFC96D");
        }
        // Reko: a decoder for the instruction C0F9C983 at address 00148E70 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F9C083C9)
        [Test]
        public void ThumbDis_C0F9C983()
        {
            AssertCode("@@@", "C0F9C983");
        }
        // Reko: a decoder for the instruction E2EFC97D at address 001AB4A2 has not been implemented. (Unimplemented '*' when decoding EFE27DC9)
        [Test]
        public void ThumbDis_E2EFC97D()
        {
            AssertCode("@@@", "E2EFC97D");
        }
        // Reko: a decoder for the instruction E9EF5158 at address 00148F1C has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_E9EF5158()
        {
            AssertCode("@@@", "E9EF5158");
        }
        // Reko: a decoder for the instruction 6FFC9478 at address 001AB526 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6FFC9478()
        {
            AssertCode("@@@", "6FFC9478");
        }
        // Reko: a decoder for the instruction 3FFCCABD at address 001AB530 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_3FFCCABD()
        {
            AssertCode("@@@", "3FFCCABD");
        }
        // Reko: a decoder for the instruction C7FF30DC at address 001AB7F6 has not been implemented. (Unimplemented '*immediate - T3' when decoding FFC7DC30)
        [Test]
        public void ThumbDis_C7FF30DC()
        {
            AssertCode("@@@", "C7FF30DC");
        }
        // Reko: a decoder for the instruction ACF3E3AB at address 001AB846 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_ACF3E3AB()
        {
            AssertCode("@@@", "ACF3E3AB");
        }
        // Reko: a decoder for the instruction DFFFEE52 at address 001293F4 has not been implemented. (Unimplemented '*scalar' when decoding FFDF52EE)
        [Test]
        public void ThumbDis_DFFFEE52()
        {
            AssertCode("@@@", "DFFFEE52");
        }
        // Reko: a decoder for the instruction 6FFD9FD9 at address 001ABDAC has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6FFD9FD9()
        {
            AssertCode("@@@", "6FFD9FD9");
        }
        // Reko: a decoder for the instruction D9FDC2A9 at address 001294C8 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D9FDC2A9()
        {
            AssertCode("@@@", "D9FDC2A9");
        }
        // Reko: a decoder for the instruction E7FFC6EE at address 00129526 has not been implemented. (Unimplemented '*' when decoding FFE7EEC6)
        [Test]
        public void ThumbDis_E7FFC6EE()
        {
            AssertCode("@@@", "E7FFC6EE");
        }
        // Reko: a decoder for the instruction 82FE421D at address 0010DA72 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_82FE421D()
        {
            AssertCode("@@@", "82FE421D");
        }
        // Reko: a decoder for the instruction F1F370A7 at address 00129618 has not been implemented. (Unimplemented '*banked register' when decoding F3F1A770)
        [Test]
        public void ThumbDis_F1F370A7()
        {
            AssertCode("@@@", "F1F370A7");
        }
        // Reko: a decoder for the instruction BBFC530C at address 0012967A has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BBFC530C()
        {
            AssertCode("@@@", "BBFC530C");
        }
        // Reko: a decoder for the instruction C1F91FFD at address 001492F4 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C1F91FFD()
        {
            AssertCode("@@@", "C1F91FFD");
        }
        // Reko: a decoder for the instruction A7EF7CD8 at address 0014933E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_A7EF7CD8()
        {
            AssertCode("@@@", "A7EF7CD8");
        }
        // Reko: a decoder for the instruction E4EE5149 at address 00149344 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 0CEF7E04 at address 001296CC has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_0CEF7E04()
        {
            AssertCode("@@@", "0CEF7E04");
        }
        // Reko: a decoder for the instruction 05FF7B2F at address 001495A8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_05FF7B2F()
        {
            AssertCode("@@@", "05FF7B2F");
        }
        // Reko: a decoder for the instruction 32FF9A44 at address 0014966A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_32FF9A44()
        {
            AssertCode("@@@", "32FF9A44");
        }
        // Reko: a decoder for the instruction 03EFAC54 at address 00187A1E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_03EFAC54()
        {
            AssertCode("@@@", "03EFAC54");
        }
        // Reko: a decoder for the instruction EFFD2528 at address 001ABF76 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_EFFD2528()
        {
            AssertCode("@@@", "EFFD2528");
        }
        // Reko: a decoder for the instruction FBFF7258 at address 001CB4F8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_FBFF7258()
        {
            AssertCode("@@@", "FBFF7258");
        }

        // Reko: a decoder for the instruction 8BEF1FA8 at address 001AC2A2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_8BEF1FA8()
        {
            AssertCode("@@@", "8BEF1FA8");
        }
        // Reko: a decoder for the instruction CAFC427C at address 0016B908 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CAFC427C()
        {
            AssertCode("@@@", "CAFC427C");
        }
        // Reko: a decoder for the instruction 8CFE827C at address 0016B96A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8CFE827C()
        {
            AssertCode("@@@", "8CFE827C");
        }
        // Reko: a decoder for the instruction 4EEF791C at address 0016B9C8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_4EEF791C()
        {
            AssertCode("@@@", "4EEF791C");
        }
        // Reko: a decoder for the instruction 42FFD9E3 at address 0016BA1A has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_42FFD9E3()
        {
            AssertCode("@@@", "42FFD9E3");
        }
        // Reko: a decoder for the instruction 1FEB970F at address 0016BA56 has not been implemented. (Unimplemented '*register' when decoding EB1F0F97)
        [Test]
        public void ThumbDis_1FEB970F()
        {
            AssertCode("@@@", "1FEB970F");
        }
        // Reko: a decoder for the instruction 76F95B1E at address 001EB052 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_76F95B1E()
        {
            AssertCode("@@@", "76F95B1E");
        }
        // Reko: a decoder for the instruction 01EE3FBB at address 001EB138 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_01EE3FBB()
        {
            AssertCode("@@@", "01EE3FBB");
        }
        // Reko: a decoder for the instruction 53FEBBCC at address 001EB196 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_53FEBBCC()
        {
            AssertCode("@@@", "53FEBBCC");
        }
        // Reko: a decoder for the instruction BDFC6469 at address 001EB200 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BDFC6469()
        {
            AssertCode("@@@", "BDFC6469");
        }
        // Reko: a decoder for the instruction 8FFC013D at address 001EB216 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8FFC013D()
        {
            AssertCode("@@@", "8FFC013D");
        }
        // Reko: a decoder for the instruction D1FAA2D2 at address 001EB370 has not been implemented. (crc32c-crc32cw)
        [Test]
        public void ThumbDis_D1FAA2D2()
        {
            AssertCode("@@@", "D1FAA2D2");
        }
        // Reko: a decoder for the instruction 81FF399E at address 00149DD6 has not been implemented. (Unimplemented '*immediate - T5' when decoding FF819E39)
        [Test]
        public void ThumbDis_81FF399E()
        {
            AssertCode("@@@", "81FF399E");
        }
        // Reko: a decoder for the instruction 1FFECBDC at address 0011683E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_1FFECBDC()
        {
            AssertCode("@@@", "1FFECBDC");
        }
        // Reko: a decoder for the instruction A1EC69EB at address 001CB998 has not been implemented. (Unimplemented '*' when decoding ECA1EB69)
        [Test]
        public void ThumbDis_A1EC69EB()
        {
            AssertCode("@@@", "A1EC69EB");
        }
        // Reko: a decoder for the instruction E9EFB8B4 at address 001AC308 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_E9EFB8B4()
        {
            AssertCode("@@@", "E9EFB8B4");
        }
        // Reko: a decoder for the instruction DCE8CA1D at address 001CBE9E has not been implemented. (Unimplemented '*' when decoding E8DC1DCA)
        [Test]
        public void ThumbDis_DCE8CA1D()
        {
            AssertCode("@@@", "DCE8CA1D");
        }
        // Reko: a decoder for the instruction 93FC113C at address 00188110 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_93FC113C()
        {
            AssertCode("@@@", "93FC113C");
        }
        // Reko: a decoder for the instruction D7EFB5A4 at address 0010DBBA has not been implemented. (U=0)
        [Test]
        public void ThumbDis_D7EFB5A4()
        {
            AssertCode("@@@", "D7EFB5A4");
        }
        // Reko: a decoder for the instruction 9EEFCD85 at address 0016BB6E has not been implemented. (Unimplemented '*scalar' when decoding EF9E85CD)
        [Test]
        public void ThumbDis_9EEFCD85()
        {
            AssertCode("@@@", "9EEFCD85");
        }
        // Reko: a decoder for the instruction BFFD74D9 at address 0016BC10 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_BFFD74D9()
        {
            AssertCode("@@@", "BFFD74D9");
        }
        // Reko: a decoder for the instruction 2DFFA29C at address 00116E26 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_2DFFA29C()
        {
            AssertCode("@@@", "2DFFA29C");
        }
        // Reko: a decoder for the instruction 54FED42C at address 001CBF26 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_54FED42C()
        {
            AssertCode("@@@", "54FED42C");
        }
        // Reko: a decoder for the instruction 6CEFE351 at address 0014C2A0 has not been implemented. (Unimplemented '*' when decoding EF6C51E3)
        [Test]
        public void ThumbDis_6CEFE351()
        {
            AssertCode("@@@", "6CEFE351");
        }
        // Reko: a decoder for the instruction B6FE98A8 at address 0010DC1A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B6FE98A8()
        {
            AssertCode("@@@", "B6FE98A8");
        }
        // Reko: a decoder for the instruction 92EDE05E at address 001EB406 has not been implemented. (Unimplemented '*offset variant' when decoding ED925EE0)
        [Test]
        public void ThumbDis_92EDE05E()
        {
            AssertCode("@@@", "92EDE05E");
        }
        // Reko: a decoder for the instruction A8F97584 at address 0016BCB4 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9A88475)
        [Test]
        public void ThumbDis_A8F97584()
        {
            AssertCode("@@@", "A8F97584");
        }
        // Reko: a decoder for the instruction C6EE9F79 at address 0016BCBC has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_C6EE9F79()
        {
            AssertCode("@@@", "C6EE9F79");
        }
        // Reko: a decoder for the instruction 8EF9DA97 at address 001EB8D2 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F98E97DA)
        [Test]
        public void ThumbDis_8EF9DA97()
        {
            AssertCode("@@@", "8EF9DA97");
        }
        // Reko: a decoder for the instruction 19EEDE99 at address 001EB8E6 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_19EEDE99()
        {
            AssertCode("@@@", "19EEDE99");
        }
        // Reko: a decoder for the instruction A0FC23D8 at address 0012A640 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A0FC23D8()
        {
            AssertCode("@@@", "A0FC23D8");
        }
        // Reko: a decoder for the instruction 36FFAA8D at address 0010DDA0 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_36FFAA8D()
        {
            AssertCode("@@@", "36FFAA8D");
        }
        // Reko: a decoder for the instruction A3FDA2DC at address 0016BF6A has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A3FDA2DC()
        {
            AssertCode("@@@", "A3FDA2DC");
        }
        // Reko: a decoder for the instruction C9F3838A at address 0016BF74 has not been implemented. (Unimplemented '*' when decoding F3C98A83)
        [Test]
        public void ThumbDis_C9F3838A()
        {
            AssertCode("@@@", "C9F3838A");
        }
        // Reko: a decoder for the instruction 76EFE741 at address 001AD362 has not been implemented. (Unimplemented '*' when decoding EF7641E7)
        [Test]
        public void ThumbDis_76EFE741()
        {
            AssertCode("@@@", "76EFE741");
        }
        // Reko: a decoder for the instruction 08FD5798 at address 0010DDDC has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_08FD5798()
        {
            AssertCode("@@@", "08FD5798");
        }
        // Reko: a decoder for the instruction 46FD5298 at address 0012A932 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_46FD5298()
        {
            AssertCode("@@@", "46FD5298");
        }
        // Reko: a decoder for the instruction 2FEF67E3 at address 0010DF54 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2FEF67E3()
        {
            AssertCode("@@@", "2FEF67E3");
        }
        // Reko: a decoder for the instruction 56FFD77B at address 0010E7C6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_56FFD77B()
        {
            AssertCode("@@@", "56FFD77B");
        }
        // Reko: a decoder for the instruction 4FEF41DB at address 00188356 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_4FEF41DB()
        {
            AssertCode("@@@", "4FEF41DB");
        }
        // Reko: a decoder for the instruction 2FFDFF79 at address 001AD94E has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2FFDFF79()
        {
            AssertCode("@@@", "2FFDFF79");
        }
        // Reko: a decoder for the instruction B4FFA876 at address 0012ACA0 has not been implemented. (Unimplemented '*' when decoding FFB476A8)
        [Test]
        public void ThumbDis_B4FFA876()
        {
            AssertCode("@@@", "B4FFA876");
        }
        // Reko: a decoder for the instruction B9EE182B at address 0014D100 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction DEFF5899 at address 0016C548 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FFDE9958)
        [Test]
        public void ThumbDis_DEFF5899()
        {
            AssertCode("@@@", "DEFF5899");
        }
        // Reko: a decoder for the instruction CDFF5A88 at address 0016C5EA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_CDFF5A88()
        {
            AssertCode("@@@", "CDFF5A88");
        }
        // Reko: a decoder for the instruction 0AFC6A09 at address 0016C606 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0AFC6A09()
        {
            AssertCode("@@@", "0AFC6A09");
        }
        // Reko: a decoder for the instruction C2FE77D9 at address 0016C62A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C2FE77D9()
        {
            AssertCode("@@@", "C2FE77D9");
        }
        // Reko: a decoder for the instruction CFF9324B at address 0016C700 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9CF4B32)
        [Test]
        public void ThumbDis_CFF9324B()
        {
            AssertCode("@@@", "CFF9324B");
        }
        // Reko: a decoder for the instruction 16FF12DF at address 0018844E has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_16FF12DF()
        {
            AssertCode("@@@", "16FF12DF");
        }
        // Reko: a decoder for the instruction EEFFE7D8 at address 001ADA98 has not been implemented. (Unimplemented '*scalar' when decoding FFEED8E7)
        [Test]
        public void ThumbDis_EEFFE7D8()
        {
            AssertCode("@@@", "EEFFE7D8");
        }
        // Reko: a decoder for the instruction 10FE1139 at address 0014D7CC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_10FE1139()
        {
            AssertCode("@@@", "10FE1139");
        }
        // Reko: a decoder for the instruction F6F38681 at address 0014D83C has not been implemented. (Unimplemented '*register' when decoding F3F68186)
        [Test]
        public void ThumbDis_F6F38681()
        {
            AssertCode("@@@", "F6F38681");
        }
        // Reko: a decoder for the instruction 31FC9CCD at address 00118832 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_31FC9CCD()
        {
            AssertCode("@@@", "31FC9CCD");
        }
        // Reko: a decoder for the instruction 9FF3C4AC at address 001AE524 has not been implemented. (Unimplemented '*register' when decoding F39FACC4)
        [Test]
        public void ThumbDis_9FF3C4AC()
        {
            AssertCode("@@@", "9FF3C4AC");
        }
        // Reko: a decoder for the instruction ADF96A55 at address 001ED53E has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9AD556A)
        [Test]
        public void ThumbDis_ADF96A55()
        {
            AssertCode("@@@", "ADF96A55");
        }
        // Reko: a decoder for the instruction 8AFFE1BC at address 001ED7E8 has not been implemented. (Unimplemented '*' when decoding FF8ABCE1)
        [Test]
        public void ThumbDis_8AFFE1BC()
        {
            AssertCode("@@@", "8AFFE1BC");
        }
        // Reko: a decoder for the instruction E5F940BF at address 001ED7EE has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E5F940BF()
        {
            AssertCode("@@@", "E5F940BF");
        }
        // Reko: a decoder for the instruction 4DFF8DFB at address 0010F33A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_4DFF8DFB()
        {
            AssertCode("@@@", "4DFF8DFB");
        }
        // Reko: a decoder for the instruction FFEFDB7C at address 0010F366 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_FFEFDB7C()
        {
            AssertCode("@@@", "FFEFDB7C");
        }
        // Reko: a decoder for the instruction FBFF964D at address 0010F500 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_FBFF964D()
        {
            AssertCode("@@@", "FBFF964D");
        }
        // Reko: a decoder for the instruction D0FFE4DE at address 0010F69C has not been implemented. (Unimplemented '*' when decoding FFD0DEE4)
        [Test]
        public void ThumbDis_D0FFE4DE()
        {
            AssertCode("@@@", "D0FFE4DE");
        }
        // Reko: a decoder for the instruction A7FDEF6D at address 0014D8AA has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A7FDEF6D()
        {
            AssertCode("@@@", "A7FDEF6D");
        }
        // Reko: a decoder for the instruction 4AFCD7FD at address 0014D8CC has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4AFCD7FD()
        {
            AssertCode("@@@", "4AFCD7FD");
        }
        // Reko: a decoder for the instruction E3FEBE6C at address 0010F712 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E3FEBE6C()
        {
            AssertCode("@@@", "E3FEBE6C");
        }
        // Reko: a decoder for the instruction 6CFF674B at address 0016E464 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_6CFF674B()
        {
            AssertCode("@@@", "6CFF674B");
        }
        // Reko: a decoder for the instruction D8F35AAD at address 0018A0F8 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D8F35AAD()
        {
            AssertCode("@@@", "D8F35AAD");
        }
        // Reko: a decoder for the instruction A1FF43E5 at address 0018A122 has not been implemented. (Unimplemented '*scalar' when decoding FFA1E543)
        [Test]
        public void ThumbDis_A1FF43E5()
        {
            AssertCode("@@@", "A1FF43E5");
        }
        // Reko: a decoder for the instruction 52FFC144 at address 001AEA8A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_52FFC144()
        {
            AssertCode("@@@", "52FFC144");
        }
        // Reko: a decoder for the instruction E6FFDA0C at address 001AEB22 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_E6FFDA0C()
        {
            AssertCode("@@@", "E6FFDA0C");
        }
        // Reko: a decoder for the instruction 30EF167D at address 001190F4 has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_30EF167D()
        {
            AssertCode("@@@", "30EF167D");
        }
        // Reko: a decoder for the instruction 9AFE7ED8 at address 0011914A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_9AFE7ED8()
        {
            AssertCode("@@@", "9AFE7ED8");
        }
        // Reko: a decoder for the instruction 2FFF10BC at address 0014D8E6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_2FFF10BC()
        {
            AssertCode("@@@", "2FFF10BC");
        }
        // Reko: a decoder for the instruction 44FD46C9 at address 0010F7D0 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_44FD46C9()
        {
            AssertCode("@@@", "44FD46C9");
        }
        // Reko: a decoder for the instruction D7FF6DA9 at address 0018A17E has not been implemented. (Unimplemented '*scalar' when decoding FFD7A96D)
        [Test]
        public void ThumbDis_D7FF6DA9()
        {
            AssertCode("@@@", "D7FF6DA9");
        }
        // Reko: a decoder for the instruction EDFC736D at address 001AEC60 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EDFC736D()
        {
            AssertCode("@@@", "EDFC736D");
        }
        // Reko: a decoder for the instruction 2CFC73ED at address 001EE5FA has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2CFC73ED()
        {
            AssertCode("@@@", "2CFC73ED");
        }
        // Reko: a decoder for the instruction DDEFC95B at address 001EE740 has not been implemented. (Unimplemented '*' when decoding EFDD5BC9)
        [Test]
        public void ThumbDis_DDEFC95B()
        {
            AssertCode("@@@", "DDEFC95B");
        }
        // Reko: a decoder for the instruction 51EF0854 at address 0018A42C has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_51EF0854()
        {
            AssertCode("@@@", "51EF0854");
        }
        // Reko: a decoder for the instruction 92EFF8F8 at address 001AECC2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_92EFF8F8()
        {
            AssertCode("@@@", "92EFF8F8");
        }
        // Reko: a decoder for the instruction 5EFCC9FC at address 0014DA68 has not been implemented. (op1:op2=0b0001)
 
        // Reko: a decoder for the instruction 08FC908C at address 0012C9D6 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_08FC908C()
        {
            AssertCode("@@@", "08FC908C");
        }
        // Reko: a decoder for the instruction B6EFD4EC at address 0016F228 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_B6EFD4EC()
        {
            AssertCode("@@@", "B6EFD4EC");
        }
        // Reko: a decoder for the instruction 88FF4B8F at address 0014DB34 has not been implemented. (Unimplemented '*' when decoding FF888F4B)
        [Test]
        public void ThumbDis_88FF4B8F()
        {
            AssertCode("@@@", "88FF4B8F");
        }
        // Reko: a decoder for the instruction 13EFA62C at address 001AF53E has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_13EFA62C()
        {
            AssertCode("@@@", "13EFA62C");
        }
        // Reko: a decoder for the instruction 18EB675F at address 001AF56A has not been implemented. (Unimplemented '*register' when decoding EB185F67)
        [Test]
        public void ThumbDis_18EB675F()
        {
            AssertCode("@@@", "18EB675F");
        }
        // Reko: a decoder for the instruction 98FFCA68 at address 00119C26 has not been implemented. (Unimplemented '*scalar' when decoding FF9868CA)
        [Test]
        public void ThumbDis_98FFCA68()
        {
            AssertCode("@@@", "98FFCA68");
        }
        // Reko: a decoder for the instruction 39EFFD21 at address 00119C3A has not been implemented. (Unimplemented '*register' when decoding EF3921FD)
        [Test]
        public void ThumbDis_39EFFD21()
        {
            AssertCode("@@@", "39EFFD21");
        }
        // Reko: a decoder for the instruction 7EFF013D at address 001EF2A6 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_7EFF013D()
        {
            AssertCode("@@@", "7EFF013D");
        }
        // Reko: a decoder for the instruction 28FDCB28 at address 001EF342 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_28FDCB28()
        {
            AssertCode("@@@", "28FDCB28");
        }
        // Reko: a decoder for the instruction D4FA977C at address 0012D044 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D4FA977C()
        {
            AssertCode("@@@", "D4FA977C");
        }
        // Reko: a decoder for the instruction D8FF44A5 at address 0011066E has not been implemented. (Unimplemented '*scalar' when decoding FFD8A544)
        [Test]
        public void ThumbDis_D8FF44A5()
        {
            AssertCode("@@@", "D8FF44A5");
        }
        // Reko: a decoder for the instruction 3DFE079D at address 001EF550 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3DFE079D()
        {
            AssertCode("@@@", "3DFE079D");
        }
        // Reko: a decoder for the instruction 47FFD572 at address 0012D0BC has not been implemented. (Unimplemented '*' when decoding FF4772D5)

        // Reko: a decoder for the instruction 16FF9EF9 at address 0011068E has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction A4FEC6DD at address 00119D4C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A4FEC6DD()
        {
            AssertCode("@@@", "A4FEC6DD");
        }
        // Reko: a decoder for the instruction A4F92D62 at address 001EF620 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9A4622D)
        [Test]
        public void ThumbDis_A4F92D62()
        {
            AssertCode("@@@", "A4F92D62");
        }
        // Reko: a decoder for the instruction D0FA8872 at address 001EF628 has not been implemented. (crc32c-crc32cb)
        [Test]
        public void ThumbDis_D0FA8872()
        {
            AssertCode("@@@", "D0FA8872");
        }
        // Reko: a decoder for the instruction 2BEFBF33 at address 001AF90A has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2BEFBF33()
        {
            AssertCode("@@@", "2BEFBF33");
        }
        // Reko: a decoder for the instruction 03FEF38D at address 001CDF56 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_03FEF38D()
        {
            AssertCode("@@@", "03FEF38D");
        }
        // Reko: a decoder for the instruction 03EFD6AD at address 0012D478 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_03EFD6AD()
        {
            AssertCode("@@@", "03EFD6AD");
        }
        // Reko: a decoder for the instruction 72FFCA84 at address 0014E624 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_72FFCA84()
        {
            AssertCode("@@@", "72FFCA84");
        }
        // Reko: a decoder for the instruction C2ECC21A at address 00119FB0 has not been implemented. (Unimplemented '*' when decoding ECC21AC2)
        [Test]
        public void ThumbDis_C2ECC21A()
        {
            AssertCode("@@@", "C2ECC21A");
        }
        // Reko: a decoder for the instruction 05FC95CD at address 001EF9A4 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_05FC95CD()
        {
            AssertCode("@@@", "05FC95CD");
        }
        // Reko: a decoder for the instruction 18FFC87B at address 0016F79A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_18FFC87B()
        {
            AssertCode("@@@", "18FFC87B");
        }
        // Reko: a decoder for the instruction 08EF7EFD at address 0018B4E6 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_08EF7EFD()
        {
            AssertCode("@@@", "08EF7EFD");
        }
        // Reko: a decoder for the instruction 04EF47FC at address 001CE142 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_04EF47FC()
        {
            AssertCode("@@@", "04EF47FC");
        }
        // Reko: a decoder for the instruction E8F95610 at address 0012D5D2 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9E81056)
        [Test]
        public void ThumbDis_E8F95610()
        {
            AssertCode("@@@", "E8F95610");
        }
        // Reko: a decoder for the instruction AFFE25CC at address 0014E6F2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_AFFE25CC()
        {
            AssertCode("@@@", "AFFE25CC");
        }
        // Reko: a decoder for the instruction 2BFD3E3D at address 001AFCD4 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2BFD3E3D()
        {
            AssertCode("@@@", "2BFD3E3D");
        }
        // Reko: a decoder for the instruction 9DFDD24D at address 00110AA8 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_9DFDD24D()
        {
            AssertCode("@@@", "9DFDD24D");
        }
        // Reko: a decoder for the instruction A3F92C0C at address 0018B500 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A3F92C0C()
        {
            AssertCode("@@@", "A3F92C0C");
        }
        // Reko: a decoder for the instruction 9EEFC2C9 at address 001CE17E has not been implemented. (Unimplemented '*scalar' when decoding EF9EC9C2)
        [Test]
        public void ThumbDis_9EEFC2C9()
        {
            AssertCode("@@@", "9EEFC2C9");
        }
        // Reko: a decoder for the instruction 70FCB71D at address 0012D60E has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_70FCB71D()
        {
            AssertCode("@@@", "70FCB71D");
        }
        // Reko: a decoder for the instruction 91FAB9CA at address 0014E71E has not been implemented. (Unimplemented '*' when decoding FA91CAB9)

        // Reko: a decoder for the instruction CEFFBA1B at address 001AFD1E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_CEFFBA1B()
        {
            AssertCode("@@@", "CEFFBA1B");
        }
        // Reko: a decoder for the instruction 6FFF6BE4 at address 0018B53A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_6FFF6BE4()
        {
            AssertCode("@@@", "6FFF6BE4");
        }
        // Reko: a decoder for the instruction EBF96CDC at address 0012D61C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_EBF96CDC()
        {
            AssertCode("@@@", "EBF96CDC");
        }
        // Reko: a decoder for the instruction 7DF99EDF at address 0012D630 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_7DF99EDF()
        {
            AssertCode("@@@", "7DF99EDF");
        }
        // Reko: a decoder for the instruction 1EFD770D at address 001AFE7A has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction D1F32286 at address 0016FB3E has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D1F32286()
        {
            AssertCode("@@@", "D1F32286");
        }
        // Reko: a decoder for the instruction DAEF199C at address 0012D826 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_DAEF199C()
        {
            AssertCode("@@@", "DAEF199C");
        }
        // Reko: a decoder for the instruction 03FDADC9 at address 0014E8B8 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_03FDADC9()
        {
            AssertCode("@@@", "03FDADC9");
        }
        // Reko: a decoder for the instruction 98EF1DBD at address 0012DC30 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_98EF1DBD()
        {
            AssertCode("@@@", "98EF1DBD");
        }
        // Reko: a decoder for the instruction 34F9B6BE at address 001F005C has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_34F9B6BE()
        {
            AssertCode("@@@", "34F9B6BE");
        }
        // Reko: a decoder for the instruction 1AFF5B9C at address 0011A254 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_1AFF5B9C()
        {
            AssertCode("@@@", "1AFF5B9C");
        }
        // Reko: a decoder for the instruction A4FDAF5C at address 001700EC has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A4FDAF5C()
        {
            AssertCode("@@@", "A4FDAF5C");
        }
        // Reko: a decoder for the instruction 18EE581B at address 001CE524 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_18EE581B()
        {
            AssertCode("@@@", "18EE581B");
        }
        // Reko: a decoder for the instruction C6F98194 at address 00111012 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9C69481)
        [Test]
        public void ThumbDis_C6F98194()
        {
            AssertCode("@@@", "C6F98194");
        }
        // Reko: a decoder for the instruction B5EF1338 at address 0018B630 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_B5EF1338()
        {
            AssertCode("@@@", "B5EF1338");
        }
        // Reko: a decoder for the instruction 2CFCBA08 at address 0012DD56 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2CFCBA08()
        {
            AssertCode("@@@", "2CFCBA08");
        }
        // Reko: a decoder for the instruction 59EF786B at address 001B04C4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_59EF786B()
        {
            AssertCode("@@@", "59EF786B");
        }
        // Reko: a decoder for the instruction F6FC2878 at address 0018B664 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_F6FC2878()
        {
            AssertCode("@@@", "F6FC2878");
        }
        // Reko: a decoder for the instruction D1FA9775 at address 001F02F0 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D1FA9775()
        {
            AssertCode("@@@", "D1FA9775");
        }
        // Reko: a decoder for the instruction 8CFE54A8 at address 00111154 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8CFE54A8()
        {
            AssertCode("@@@", "8CFE54A8");
        }
        // Reko: a decoder for the instruction 48EF4E84 at address 0011ACB2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_48EF4E84()
        {
            AssertCode("@@@", "48EF4E84");
        }
        // Reko: a decoder for the instruction 8EFD2399 at address 001B063C has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8EFD2399()
        {
            AssertCode("@@@", "8EFD2399");
        }
        // Reko: a decoder for the instruction 23EF55A2 at address 001F0CD6 has not been implemented. (Unimplemented '*' when decoding EF23A255)
        [Test]
        public void ThumbDis_23EF55A2()
        {
            AssertCode("@@@", "23EF55A2");
        }
        // Reko: a decoder for the instruction 70EF5F91 at address 0017072A has not been implemented. (Unimplemented '*register' when decoding EF70915F)
        [Test]
        public void ThumbDis_70EF5F91()
        {
            AssertCode("@@@", "70EF5F91");
        }
        // Reko: a decoder for the instruction C8FDCC78 at address 001F0D78 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C8FDCC78()
        {
            AssertCode("@@@", "C8FDCC78");
        }
        // Reko: a decoder for the instruction E3EF17BD at address 0014F994 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_E3EF17BD()
        {
            AssertCode("@@@", "E3EF17BD");
        }
        // Reko: a decoder for the instruction A3F998F1 at address 0012EDEA has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9A3F198)
        [Test]
        public void ThumbDis_A3F998F1()
        {
            AssertCode("@@@", "A3F998F1");
        }
        // Reko: a decoder for the instruction 9FEC603E at address 001B07FA has not been implemented. (load PC)
        [Test]
        public void ThumbDis_9FEC603E()
        {
            AssertCode("@@@", "9FEC603E");
        }
        // Reko: a decoder for the instruction 1EFF2013 at address 001F163E has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1EFF2013()
        {
            AssertCode("@@@", "1EFF2013");
        }
        // Reko: a decoder for the instruction 4BFFD063 at address 0011BD90 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_4BFFD063()
        {
            AssertCode("@@@", "4BFFD063");
        }
        // Reko: a decoder for the instruction B7EE683A at address 001B097C has not been implemented. (Unimplemented '*' when decoding EEB73A68)

        // Reko: a decoder for the instruction 60EF6FF3 at address 001B0B74 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_60EF6FF3()
        {
            AssertCode("@@@", "60EF6FF3");
        }
        // Reko: a decoder for the instruction DBEF199C at address 001B0C26 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_DBEF199C()
        {
            AssertCode("@@@", "DBEF199C");
        }
        // Reko: a decoder for the instruction 4BFE40AC at address 001CF4B2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_4BFE40AC()
        {
            AssertCode("@@@", "4BFE40AC");
        }
        // Reko: a decoder for the instruction C1FA8724 at address 0014FA8A has not been implemented. (crc32-crc32b)
        [Test]
        public void ThumbDis_C1FA8724()
        {
            AssertCode("@@@", "C1FA8724");
        }
        // Reko: a decoder for the instruction F1FFF33C at address 001CFD82 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_F1FFF33C()
        {
            AssertCode("@@@", "F1FFF33C");
        }
        // Reko: a decoder for the instruction ACF91DE9 at address 00111CAE has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9ACE91D)
        [Test]
        public void ThumbDis_ACF91DE9()
        {
            AssertCode("@@@", "ACF91DE9");
        }
        // Reko: a decoder for the instruction 8CF9AA61 at address 00111D94 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F98C61AA)
        [Test]
        public void ThumbDis_8CF9AA61()
        {
            AssertCode("@@@", "8CF9AA61");
        }
        // Reko: a decoder for the instruction BAFCAE59 at address 001F1D64 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BAFCAE59()
        {
            AssertCode("@@@", "BAFCAE59");
        }
        // Reko: a decoder for the instruction E2FE3669 at address 0011BFA4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E2FE3669()
        {
            AssertCode("@@@", "E2FE3669");
        }
        // Reko: a decoder for the instruction 08FFB3BF at address 001F1FC6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_08FFB3BF()
        {
            AssertCode("@@@", "08FFB3BF");
        }
        // Reko: a decoder for the instruction 41EEB799 at address 001B0C8C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_41EEB799()
        {
            AssertCode("@@@", "41EEB799");
        }
        // Reko: a decoder for the instruction 18EEFB29 at address 0012F580 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_18EEFB29()
        {
            AssertCode("@@@", "18EEFB29");
        }
        // Reko: a decoder for the instruction 99EC178E at address 00111E5E has not been implemented. (Unimplemented '*' when decoding EC998E17)
        [Test]
        public void ThumbDis_99EC178E()
        {
            AssertCode("@@@", "99EC178E");
        }
        // Reko: a decoder for the instruction A0FE036D at address 0018C684 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A0FE036D()
        {
            AssertCode("@@@", "A0FE036D");
        }
        // Reko: a decoder for the instruction 60FE58F8 at address 001120C2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_60FE58F8()
        {
            AssertCode("@@@", "60FE58F8");
        }
        // Reko: a decoder for the instruction FAFCA7D8 at address 0018C70A has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_FAFCA7D8()
        {
            AssertCode("@@@", "FAFCA7D8");
        }
        // Reko: a decoder for the instruction 34FED939 at address 0012F9F6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_34FED939()
        {
            AssertCode("@@@", "34FED939");
        }
        // Reko: a decoder for the instruction 65FC692D at address 001F2002 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_65FC692D()
        {
            AssertCode("@@@", "65FC692D");
        }
        // Reko: a decoder for the instruction B1EEFFB9 at address 001D026E has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 47EF09EC at address 001120EC has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_47EF09EC()
        {
            AssertCode("@@@", "47EF09EC");
        }
        // Reko: a decoder for the instruction 12EFE3FB at address 0018C744 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_12EFE3FB()
        {
            AssertCode("@@@", "12EFE3FB");
        }
        // Reko: a decoder for the instruction 8CFF692F at address 00112410 has not been implemented. (Unimplemented '*' when decoding FF8C2F69)
        [Test]
        public void ThumbDis_8CFF692F()
        {
            AssertCode("@@@", "8CFF692F");
        }
        // Reko: a decoder for the instruction CAF91ADB at address 0012FC9E has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9CADB1A)
        [Test]
        public void ThumbDis_CAF91ADB()
        {
            AssertCode("@@@", "CAF91ADB");
        }
        // Reko: a decoder for the instruction 74FFA7DB at address 00150376 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_74FFA7DB()
        {
            AssertCode("@@@", "74FFA7DB");
        }
        // Reko: a decoder for the instruction 5BEF8603 at address 0012FFBE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_5BEF8603()
        {
            AssertCode("@@@", "5BEF8603");
        }
        // Reko: a decoder for the instruction 12FF4A03 at address 001503A4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_12FF4A03()
        {
            AssertCode("@@@", "12FF4A03");
        }
        // Reko: a decoder for the instruction D7EFA0DD at address 0018C9E0 has not been implemented. (Unimplemented '*integer' when decoding EFD7DDA0)
        [Test]
        public void ThumbDis_D7EFA0DD()
        {
            AssertCode("@@@", "D7EFA0DD");
        }
        // Reko: a decoder for the instruction CDEFD0CD at address 00150BC6 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_CDEFD0CD()
        {
            AssertCode("@@@", "CDEFD0CD");
        }
        // Reko: a decoder for the instruction A5EFCAEB at address 001D0A56 has not been implemented. (Unimplemented '*' when decoding EFA5EBCA)
        [Test]
        public void ThumbDis_A5EFCAEB()
        {
            AssertCode("@@@", "A5EFCAEB");
        }
        // Reko: a decoder for the instruction 9BFD478D at address 001B191A has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_9BFD478D()
        {
            AssertCode("@@@", "9BFD478D");
        }
        // Reko: a decoder for the instruction 21FD1998 at address 001F3CBC has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_21FD1998()
        {
            AssertCode("@@@", "21FD1998");
        }
        // Reko: a decoder for the instruction 39FCD619 at address 001F3E28 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_39FCD619()
        {
            AssertCode("@@@", "39FCD619");
        }
        // Reko: a decoder for the instruction 29FD061C at address 0018D366 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_29FD061C()
        {
            AssertCode("@@@", "29FD061C");
        }
        // Reko: a decoder for the instruction 47FC7B3D at address 0011D25C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_47FC7B3D()
        {
            AssertCode("@@@", "47FC7B3D");
        }
        // Reko: a decoder for the instruction CFF9D91E at address 00150E7C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CFF9D91E()
        {
            AssertCode("@@@", "CFF9D91E");
        }
        // Reko: a decoder for the instruction 82FE5568 at address 0011D422 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_82FE5568()
        {
            AssertCode("@@@", "82FE5568");
        }
        // Reko: a decoder for the instruction 1CFF3A81 at address 0011D440 has not been implemented. (Unimplemented '*register' when decoding FF1C813A)

        // Reko: a decoder for the instruction D2EFB97C at address 001D0C12 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_D2EFB97C()
        {
            AssertCode("@@@", "D2EFB97C");
        }
        // Reko: a decoder for the instruction 49FE18A8 at address 00150EDC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_49FE18A8()
        {
            AssertCode("@@@", "49FE18A8");
        }
        // Reko: a decoder for the instruction 14EF18A3 at address 001F3E60 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_14EF18A3()
        {
            AssertCode("@@@", "14EF18A3");
        }
        // Reko: a decoder for the instruction FCFC22EC at address 001D1068 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_FCFC22EC()
        {
            AssertCode("@@@", "FCFC22EC");
        }
        // Reko: a decoder for the instruction 61FF2F5D at address 001B27F2 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_61FF2F5D()
        {
            AssertCode("@@@", "61FF2F5D");
        }
        // Reko: a decoder for the instruction 21EF1472 at address 0011D94A has not been implemented. (Unimplemented '*' when decoding EF217214)
        [Test]
        public void ThumbDis_21EF1472()
        {
            AssertCode("@@@", "21EF1472");
        }
        // Reko: a decoder for the instruction C2EC65FA at address 001D10B4 has not been implemented. (Unimplemented '*' when decoding ECC2FA65)
        [Test]
        public void ThumbDis_C2EC65FA()
        {
            AssertCode("@@@", "C2EC65FA");
        }
        // Reko: a decoder for the instruction 9AFFE43A at address 0011DAE2 has not been implemented. (Unimplemented '*' when decoding FF9A3AE4)
        [Test]
        public void ThumbDis_9AFFE43A()
        {
            AssertCode("@@@", "9AFFE43A");
        }
        // Reko: a decoder for the instruction A7EF1AB8 at address 001B2F2E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_A7EF1AB8()
        {
            AssertCode("@@@", "A7EF1AB8");
        }
        // Reko: a decoder for the instruction AFEF759D at address 00113CA0 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_AFEF759D()
        {
            AssertCode("@@@", "AFEF759D");
        }
        // Reko: a decoder for the instruction 85EE71DB at address 00151B62 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 8EF91013 at address 001F5140 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F98E1310)
        [Test]
        public void ThumbDis_8EF91013()
        {
            AssertCode("@@@", "8EF91013");
        }
        // Reko: a decoder for the instruction 2FFC3DFC at address 001F51F6 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2FFC3DFC()
        {
            AssertCode("@@@", "2FFC3DFC");
        }
        // Reko: a decoder for the instruction 62FE31D8 at address 00131592 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_62FE31D8()
        {
            AssertCode("@@@", "62FE31D8");
        }
        // Reko: a decoder for the instruction D6FFC5DD at address 00131614 has not been implemented. (Unimplemented '*' when decoding FFD6DDC5)
        [Test]
        public void ThumbDis_D6FFC5DD()
        {
            AssertCode("@@@", "D6FFC5DD");
        }
        // Reko: a decoder for the instruction A2FE5BA9 at address 00152180 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A2FE5BA9()
        {
            AssertCode("@@@", "A2FE5BA9");
        }
        // Reko: a decoder for the instruction 04FFEA9F at address 00174708 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_04FFEA9F()
        {
            AssertCode("@@@", "04FFEA9F");
        }
        // Reko: a decoder for the instruction 2EFD5299 at address 001F5BE6 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2EFD5299()
        {
            AssertCode("@@@", "2EFD5299");
        }
        // Reko: a decoder for the instruction 94FEE3CC at address 001F5BF0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_94FEE3CC()
        {
            AssertCode("@@@", "94FEE3CC");
        }
        // Reko: a decoder for the instruction 1DFDBAD8 at address 0015220A has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction E8FE23AD at address 00174806 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E8FE23AD()
        {
            AssertCode("@@@", "E8FE23AD");
        }
        // Reko: a decoder for the instruction F2FD7A4C at address 001B43DA has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F2FD7A4C()
        {
            AssertCode("@@@", "F2FD7A4C");
        }
        // Reko: a decoder for the instruction C2F94DF2 at address 00131EB8 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9C2F24D)
        [Test]
        public void ThumbDis_C2F94DF2()
        {
            AssertCode("@@@", "C2F94DF2");
        }
        // Reko: a decoder for the instruction C8EC6E4B at address 001D2382 has not been implemented. (Unimplemented '*' when decoding ECC84B6E)
        [Test]
        public void ThumbDis_C8EC6E4B()
        {
            AssertCode("@@@", "C8EC6E4B");
        }
        // Reko: a decoder for the instruction 98EF6265 at address 0018DCCC has not been implemented. (Unimplemented '*scalar' when decoding EF986562)
        [Test]
        public void ThumbDis_98EF6265()
        {
            AssertCode("@@@", "98EF6265");
        }
        // Reko: a decoder for the instruction 10EF5B6E at address 001D291C has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_10EF5B6E()
        {
            AssertCode("@@@", "10EF5B6E");
        }
        // Reko: a decoder for the instruction 4BEE1CE9 at address 001D2F80 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_4BEE1CE9()
        {
            AssertCode("@@@", "4BEE1CE9");
        }
        // Reko: a decoder for the instruction 12FDBA48 at address 001B5DC8 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 9BEF4727 at address 001B5F5E has not been implemented. (Unimplemented '*' when decoding EF9B2747)
        [Test]
        public void ThumbDis_9BEF4727()
        {
            AssertCode("@@@", "9BEF4727");
        }
        // Reko: a decoder for the instruction A7F953EF at address 001F7B32 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A7F953EF()
        {
            AssertCode("@@@", "A7F953EF");
        }
        // Reko: a decoder for the instruction 22EF1C34 at address 0018F566 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_22EF1C34()
        {
            AssertCode("@@@", "22EF1C34");
        }
        // Reko: a decoder for the instruction 91FF497A at address 0018F7E4 has not been implemented. (Unimplemented '*' when decoding FF917A49)
        [Test]
        public void ThumbDis_91FF497A()
        {
            AssertCode("@@@", "91FF497A");
        }
        // Reko: a decoder for the instruction 78FFB7EC at address 001D3C1C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_78FFB7EC()
        {
            AssertCode("@@@", "78FFB7EC");
        }
        // Reko: a decoder for the instruction 2FFEB7B9 at address 001D3C72 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_2FFEB7B9()
        {
            AssertCode("@@@", "2FFEB7B9");
        }
        // Reko: a decoder for the instruction 1CFCC5AD at address 001D3CB0 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 35FCFC5C at address 001D45C8 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_35FCFC5C()
        {
            AssertCode("@@@", "35FCFC5C");
        }
        // Reko: a decoder for the instruction 14FCD6ED at address 0015F1A0 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 96FEDFD9 at address 00153BF2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_96FEDFD9()
        {
            AssertCode("@@@", "96FEDFD9");
        }
        // Reko: a decoder for the instruction 12FFAAAC at address 0017E5C4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_12FFAAAC()
        {
            AssertCode("@@@", "12FFAAAC");
        }
        // Reko: a decoder for the instruction 67FEEA5C at address 001D463A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_67FEEA5C()
        {
            AssertCode("@@@", "67FEEA5C");
        }
        // Reko: a decoder for the instruction C8F3928C at address 001F8A3C has not been implemented. (Unimplemented '*' when decoding F3C88C92)
        [Test]
        public void ThumbDis_C8F3928C()
        {
            AssertCode("@@@", "C8F3928C");
        }
        // Reko: a decoder for the instruction D7FD6138 at address 00153C80 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D7FD6138()
        {
            AssertCode("@@@", "D7FD6138");
        }
        // Reko: a decoder for the instruction 29FC497D at address 001D4B68 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_29FC497D()
        {
            AssertCode("@@@", "29FC497D");
        }
        // Reko: a decoder for the instruction 61FF3254 at address 00153CFE has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_61FF3254()
        {
            AssertCode("@@@", "61FF3254");
        }
        // Reko: a decoder for the instruction FCEE5309 at address 001D4BAE has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 42FD6A09 at address 001D4D40 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_42FD6A09()
        {
            AssertCode("@@@", "42FD6A09");
        }
        // Reko: a decoder for the instruction E3EE9268 at address 00153D34 has not been implemented. (Unimplemented '*' when decoding EEE36892)
        [Test]
        public void ThumbDis_E3EE9268()
        {
            AssertCode("@@@", "E3EE9268");
        }
        // Reko: a decoder for the instruction DEFDE4F8 at address 0017E706 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DEFDE4F8()
        {
            AssertCode("@@@", "DEFDE4F8");
        }
        // Reko: a decoder for the instruction 4BEFCF61 at address 001B7624 has not been implemented. (Unimplemented '*' when decoding EF4B61CF)
        [Test]
        public void ThumbDis_4BEFCF61()
        {
            AssertCode("@@@", "4BEFCF61");
        }
        // Reko: a decoder for the instruction 40EE70A9 at address 00153EF2 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_40EE70A9()
        {
            AssertCode("@@@", "40EE70A9");
        }
        // Reko: a decoder for the instruction ECF9A81C at address 0017ED6C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_ECF9A81C()
        {
            AssertCode("@@@", "ECF9A81C");
        }
        // Reko: a decoder for the instruction D7FE248D at address 00190822 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D7FE248D()
        {
            AssertCode("@@@", "D7FE248D");
        }
        // Reko: a decoder for the instruction 17FDB12D at address 0017ED84 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 19EF330B at address 001F998C has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_19EF330B()
        {
            AssertCode("@@@", "19EF330B");
        }
        // Reko: a decoder for the instruction BEEEE4C9 at address 001D5594 has not been implemented. (1110 - _HSD)
        [Test]
        public void ThumbDis_BEEEE4C9()
        {
            AssertCode("@@@", "BEEEE4C9");
        }
        // Reko: a decoder for the instruction 11FFB552 at address 001D56DA has not been implemented. (Unimplemented '*' when decoding FF1152B5)
        [Test]
        public void ThumbDis_11FFB552()
        {
            AssertCode("@@@", "11FFB552");
        }
        // Reko: a decoder for the instruction A1F9DC4C at address 001D5700 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A1F9DC4C()
        {
            AssertCode("@@@", "A1F9DC4C");
        }
        // Reko: a decoder for the instruction FCFE3958 at address 0015FE0A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_FCFE3958()
        {
            AssertCode("@@@", "FCFE3958");
        }
        // Reko: a decoder for the instruction 5BFF092C at address 0015FE34 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_5BFF092C()
        {
            AssertCode("@@@", "5BFF092C");
        }
        // Reko: a decoder for the instruction F0EEE94A at address 001B79C2 has not been implemented. (Unimplemented '*' when decoding EEF04AE9)
        [Test]
        public void ThumbDis_F0EEE94A()
        {
            AssertCode("@@@", "F0EEE94A");
        }
        // Reko: a decoder for the instruction 81FC38ED at address 001FA1B6 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_81FC38ED()
        {
            AssertCode("@@@", "81FC38ED");
        }
        // Reko: a decoder for the instruction FCFEFD79 at address 0015423C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_FCFEFD79()
        {
            AssertCode("@@@", "FCFEFD79");
        }
        // Reko: a decoder for the instruction 08FDA5B8 at address 001D5888 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_08FDA5B8()
        {
            AssertCode("@@@", "08FDA5B8");
        }
        // Reko: a decoder for the instruction 48FE899D at address 0015FE64 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_48FE899D()
        {
            AssertCode("@@@", "48FE899D");
        }
        // Reko: a decoder for the instruction 48FF7A0E at address 001342CC has not been implemented. (Unimplemented '*' when decoding FF480E7A)
 
        // Reko: a decoder for the instruction 27FD2F3D at address 001FA262 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_27FD2F3D()
        {
            AssertCode("@@@", "27FD2F3D");
        }
        // Reko: a decoder for the instruction DAFD9398 at address 001B7B7A has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DAFD9398()
        {
            AssertCode("@@@", "DAFD9398");
        }
        // Reko: a decoder for the instruction 1FEFE58B at address 001D5E46 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_1FEFE58B()
        {
            AssertCode("@@@", "1FEFE58B");
        }
        // Reko: a decoder for the instruction 4CFD1C0C at address 0019E8AE has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4CFD1C0C()
        {
            AssertCode("@@@", "4CFD1C0C");
        }
        // Reko: a decoder for the instruction 3BFC3F59 at address 0019E8BE has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_3BFC3F59()
        {
            AssertCode("@@@", "3BFC3F59");
        }
        // Reko: a decoder for the instruction 86FC4549 at address 00176F54 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_86FC4549()
        {
            AssertCode("@@@", "86FC4549");
        }
        // Reko: a decoder for the instruction 44FF9D1F at address 001FA42A has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_44FF9D1F()
        {
            AssertCode("@@@", "44FF9D1F");
        }
        // Reko: a decoder for the instruction 0DFF663B at address 001B7CB2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_0DFF663B()
        {
            AssertCode("@@@", "0DFF663B");
        }
        // Reko: a decoder for the instruction 26EFA36C at address 0019E93C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_26EFA36C()
        {
            AssertCode("@@@", "26EFA36C");
        }
        // Reko: a decoder for the instruction 70F9FFDF at address 001D609A has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_70F9FFDF()
        {
            AssertCode("@@@", "70F9FFDF");
        }
        // Reko: a decoder for the instruction 10EF67DB at address 00134724 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_10EF67DB()
        {
            AssertCode("@@@", "10EF67DB");
        }
        // Reko: a decoder for the instruction 00FF92DF at address 001D61E4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_00FF92DF()
        {
            AssertCode("@@@", "00FF92DF");
        }
        // Reko: a decoder for the instruction 04EFB6A1 at address 001770D0 has not been implemented. (Unimplemented '*register' when decoding EF04A1B6)
        [Test]
        public void ThumbDis_04EFB6A1()
        {
            AssertCode("@@@", "04EFB6A1");
        }
        // Reko: a decoder for the instruction 84FCE509 at address 001FA65E has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_84FCE509()
        {
            AssertCode("@@@", "84FCE509");
        }
        // Reko: a decoder for the instruction DEFD7AEC at address 001FA672 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DEFD7AEC()
        {
            AssertCode("@@@", "DEFD7AEC");
        }
        // Reko: a decoder for the instruction 82F9D12B at address 001FA696 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9822BD1)
        [Test]
        public void ThumbDis_82F9D12B()
        {
            AssertCode("@@@", "82F9D12B");
        }
        // Reko: a decoder for the instruction 92EF1A4D at address 00155206 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_92EF1A4D()
        {
            AssertCode("@@@", "92EF1A4D");
        }
        // Reko: a decoder for the instruction A0F9B00C at address 001FA762 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A0F9B00C()
        {
            AssertCode("@@@", "A0F9B00C");
        }
        // Reko: a decoder for the instruction 33EED379 at address 00177670 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_33EED379()
        {
            AssertCode("@@@", "33EED379");
        }
        // Reko: a decoder for the instruction 34EF7354 at address 00192910 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_34EF7354()
        {
            AssertCode("@@@", "34EF7354");
        }
        // Reko: a decoder for the instruction D4EFB094 at address 001FADA2 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_D4EFB094()
        {
            AssertCode("@@@", "D4EFB094");
        }
        // Reko: a decoder for the instruction DEEEDCE9 at address 001B8746 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction E3FD3428 at address 0017770E has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E3FD3428()
        {
            AssertCode("@@@", "E3FD3428");
        }
        // Reko: a decoder for the instruction 34EFC204 at address 0019298C has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_34EFC204()
        {
            AssertCode("@@@", "34EFC204");
        }
        // Reko: a decoder for the instruction A3EE7069 at address 0019F230 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction BAEF3BB8 at address 00155664 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_BAEF3BB8()
        {
            AssertCode("@@@", "BAEF3BB8");
        }
        // Reko: a decoder for the instruction AFFC6738 at address 00155724 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_AFFC6738()
        {
            AssertCode("@@@", "AFFC6738");
        }
        // Reko: a decoder for the instruction 79EF541C at address 001B880A has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_79EF541C()
        {
            AssertCode("@@@", "79EF541C");
        }
        // Reko: a decoder for the instruction ADF91CC5 at address 00177874 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9ADC51C)
        [Test]
        public void ThumbDis_ADF91CC5()
        {
            AssertCode("@@@", "ADF91CC5");
        }
        // Reko: a decoder for the instruction 9FF990FB at address 00134DE6 has not been implemented. (Unimplemented '* literal' when decoding F99FFB90)
        [Test]
        public void ThumbDis_9FF990FB()
        {
            AssertCode("@@@", "9FF990FB");
        }
        // Reko: a decoder for the instruction C5FC603D at address 00177CB8 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C5FC603D()
        {
            AssertCode("@@@", "C5FC603D");
        }
        // Reko: a decoder for the instruction 76EEBA0B at address 00155894 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_76EEBA0B()
        {
            AssertCode("@@@", "76EEBA0B");
        }
        // Reko: a decoder for the instruction 29EFBCBB at address 00135234 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_29EFBCBB()
        {
            AssertCode("@@@", "29EFBCBB");
        }
        // Reko: a decoder for the instruction 3BFF1B9E at address 001B8952 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1)
        [Test]
        public void ThumbDis_3BFF1B9E()
        {
            AssertCode("@@@", "3BFF1B9E");
        }
        // Reko: a decoder for the instruction 13EEB12B at address 001FAE30 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_13EEB12B()
        {
            AssertCode("@@@", "13EEB12B");
        }
        // Reko: a decoder for the instruction 27FF8A44 at address 0019F36A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_27FF8A44()
        {
            AssertCode("@@@", "27FF8A44");
        }
        // Reko: a decoder for the instruction E2FE904C at address 00177EB6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E2FE904C()
        {
            AssertCode("@@@", "E2FE904C");
        }
        // Reko: a decoder for the instruction D4FD2B18 at address 00177F8E has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D4FD2B18()
        {
            AssertCode("@@@", "D4FD2B18");
        }
        // Reko: a decoder for the instruction 42FFCD41 at address 001B8C5A has not been implemented. (Unimplemented '*' when decoding FF4241CD)
        [Test]
        public void ThumbDis_42FFCD41()
        {
            AssertCode("@@@", "42FFCD41");
        }
        // Reko: a decoder for the instruction CAEC645B at address 0019F82C has not been implemented. (Unimplemented '*' when decoding ECCA5B64)
        [Test]
        public void ThumbDis_CAEC645B()
        {
            AssertCode("@@@", "CAEC645B");
        }
        // Reko: a decoder for the instruction 99FC8F8C at address 00192DCA has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_99FC8F8C()
        {
            AssertCode("@@@", "99FC8F8C");
        }
        // Reko: a decoder for the instruction 9BFE90F9 at address 001BEED0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_9BFE90F9()
        {
            AssertCode("@@@", "9BFE90F9");
        }
        // Reko: a decoder for the instruction 6FEF8453 at address 0015591E has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_6FEF8453()
        {
            AssertCode("@@@", "6FEF8453");
        }
        // Reko: a decoder for the instruction A0ECF71A at address 0019317C has not been implemented. (Unimplemented '*' when decoding ECA01AF7)
        [Test]
        public void ThumbDis_A0ECF71A()
        {
            AssertCode("@@@", "A0ECF71A");
        }
        // Reko: a decoder for the instruction E7FF3A2C at address 001BEF56 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_E7FF3A2C()
        {
            AssertCode("@@@", "E7FF3A2C");
        }
        // Reko: a decoder for the instruction A9EFCDD8 at address 001B92EA has not been implemented. (Unimplemented '*scalar' when decoding EFA9D8CD)
        [Test]
        public void ThumbDis_A9EFCDD8()
        {
            AssertCode("@@@", "A9EFCDD8");
        }
        // Reko: a decoder for the instruction 1FEE588B at address 001784FA has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_1FEE588B()
        {
            AssertCode("@@@", "1FEE588B");
        }
        // Reko: a decoder for the instruction 95F3AEA1 at address 0019318A has not been implemented. (Unimplemented '*banked register' when decoding F395A1AE)
        [Test]
        public void ThumbDis_95F3AEA1()
        {
            AssertCode("@@@", "95F3AEA1");
        }
        // Reko: a decoder for the instruction EEEC67FB at address 001BEFBE has not been implemented. (Unimplemented '*' when decoding ECEEFB67)
        [Test]
        public void ThumbDis_EEEC67FB()
        {
            AssertCode("@@@", "EEEC67FB");
        }
        // Reko: a decoder for the instruction 1AEFCE33 at address 0013638A has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1AEFCE33()
        {
            AssertCode("@@@", "1AEFCE33");
        }
        // Reko: a decoder for the instruction A6F919A4 at address 00155BB2 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9A6A419)
        [Test]
        public void ThumbDis_A6F919A4()
        {
            AssertCode("@@@", "A6F919A4");
        }
        // Reko: a decoder for the instruction C9FAABA2 at address 001363E2 has not been implemented. (crc32-crc32w)
        [Test]
        public void ThumbDis_C9FAABA2()
        {
            AssertCode("@@@", "C9FAABA2");
        }
        // Reko: a decoder for the instruction DCEF4629 at address 001D7DFA has not been implemented. (Unimplemented '*scalar' when decoding EFDC2946)
        [Test]
        public void ThumbDis_DCEF4629()
        {
            AssertCode("@@@", "DCEF4629");
        }
        // Reko: a decoder for the instruction 01FFB9DF at address 0019326E has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_01FFB9DF()
        {
            AssertCode("@@@", "01FFB9DF");
        }
        // Reko: a decoder for the instruction E7F92B8F at address 001BF462 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E7F92B8F()
        {
            AssertCode("@@@", "E7F92B8F");
        }
        // Reko: a decoder for the instruction 42FF3DDB at address 001BF502 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_42FF3DDB()
        {
            AssertCode("@@@", "42FF3DDB");
        }
        // Reko: a decoder for the instruction 77FDC438 at address 001BF590 has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_77FDC438()
        {
            AssertCode("@@@", "77FDC438");
        }
        // Reko: a decoder for the instruction 8CEEF56B at address 001BF624 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 24FF2753 at address 001DE5D0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_24FF2753()
        {
            AssertCode("@@@", "24FF2753");
        }
        // Reko: a decoder for the instruction D6FCBA58 at address 00136412 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D6FCBA58()
        {
            AssertCode("@@@", "D6FCBA58");
        }
        // Reko: a decoder for the instruction C3FD2B78 at address 001B9A40 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C3FD2B78()
        {
            AssertCode("@@@", "C3FD2B78");
        }
        // Reko: a decoder for the instruction 9BFF37DD at address 001D7E14 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_9BFF37DD()
        {
            AssertCode("@@@", "9BFF37DD");
        }
        // Reko: a decoder for the instruction D2FCEA18 at address 00178C0C has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D2FCEA18()
        {
            AssertCode("@@@", "D2FCEA18");
        }
        // Reko: a decoder for the instruction D4FFE5CE at address 00193810 has not been implemented. (Unimplemented '*' when decoding FFD4CEE5)
        [Test]
        public void ThumbDis_D4FFE5CE()
        {
            AssertCode("@@@", "D4FFE5CE");
        }
        // Reko: a decoder for the instruction ADF95335 at address 001D8D2A has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9AD3553)
        [Test]
        public void ThumbDis_ADF95335()
        {
            AssertCode("@@@", "ADF95335");
        }
        // Reko: a decoder for the instruction 1FEFFB2F at address 001D8D68 has not been implemented. (Unimplemented '*' when decoding EF1F2FFB)
        [Test]
        public void ThumbDis_1FEFFB2F()
        {
            AssertCode("@@@", "1FEFFB2F");
        }
        // Reko: a decoder for the instruction 64EFF8DD at address 001DEA1A has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_64EFF8DD()
        {
            AssertCode("@@@", "64EFF8DD");
        }
        // Reko: a decoder for the instruction 14FFE87B at address 001B9BB2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_14FFE87B()
        {
            AssertCode("@@@", "14FFE87B");
        }
        // Reko: a decoder for the instruction E1F991AC at address 00136D54 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E1F991AC()
        {
            AssertCode("@@@", "E1F991AC");
        }
        // Reko: a decoder for the instruction 8CFC10C9 at address 00156DE4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8CFC10C9()
        {
            AssertCode("@@@", "8CFC10C9");
        }
        // Reko: a decoder for the instruction 17EE9C6B at address 001FCE1C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_17EE9C6B()
        {
            AssertCode("@@@", "17EE9C6B");
        }
        // Reko: a decoder for the instruction 3FFC2AE8 at address 001FD424 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_3FFC2AE8()
        {
            AssertCode("@@@", "3FFC2AE8");
        }
        // Reko: a decoder for the instruction A2F92DE8 at address 00193F44 has not been implemented. (Unimplemented 'single element from one lane - T3' when decoding F9A2E82D)
        [Test]
        public void ThumbDis_A2F92DE8()
        {
            AssertCode("@@@", "A2F92DE8");
        }
        // Reko: a decoder for the instruction 0CFFA89B at address 001DF40E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_0CFFA89B()
        {
            AssertCode("@@@", "0CFFA89B");
        }
        // Reko: a decoder for the instruction CAF9324F at address 001D8DD4 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CAF9324F()
        {
            AssertCode("@@@", "CAF9324F");
        }
        // Reko: a decoder for the instruction 49EFF49F at address 001FF262 has not been implemented. (Unimplemented '*' when decoding EF499FF4)
        [Test]
        public void ThumbDis_49EFF49F()
        {
            AssertCode("@@@", "49EFF49F");
        }
        // Reko: a decoder for the instruction 7CFC0D4D at address 001DF81A has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7CFC0D4D()
        {
            AssertCode("@@@", "7CFC0D4D");
        }
        // Reko: a decoder for the instruction 72FF6C2B at address 00193FAE has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_72FF6C2B()
        {
            AssertCode("@@@", "72FF6C2B");
        }
        // Reko: a decoder for the instruction 2FFCDEF9 at address 001575BC has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2FFCDEF9()
        {
            AssertCode("@@@", "2FFCDEF9");
        }
        // Reko: a decoder for the instruction 87F9C66C at address 001FD6AE has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_87F9C66C()
        {
            AssertCode("@@@", "87F9C66C");
        }
        // Reko: a decoder for the instruction 9BFE3149 at address 001FF41A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_9BFE3149()
        {
            AssertCode("@@@", "9BFE3149");
        }
        // Reko: a decoder for the instruction AEF90C4D at address 001DF9A6 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_AEF90C4D()
        {
            AssertCode("@@@", "AEF90C4D");
        }
        // Reko: a decoder for the instruction 81FC58C8 at address 001FFDF6 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_81FC58C8()
        {
            AssertCode("@@@", "81FC58C8");
        }
        // Reko: a decoder for the instruction 9DF3A98F at address 001D9988 has not been implemented. (Unimplemented '*banked register' when decoding F39D8FA9)
        [Test]
        public void ThumbDis_9DF3A98F()
        {
            AssertCode("@@@", "9DF3A98F");
        }
        // Reko: a decoder for the instruction A7F32589 at address 001DFF52 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A7F32589()
        {
            AssertCode("@@@", "A7F32589");
        }
        // Reko: a decoder for the instruction FDFDFCFD at address 00158256 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FDFDFCFD()
        {
            AssertCode("@@@", "FDFDFCFD");
        }
        // Reko: a decoder for the instruction E8FD038D at address 001BAF96 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E8FD038D()
        {
            AssertCode("@@@", "E8FD038D");
        }
        // Reko: a decoder for the instruction 10FEDA7D at address 001380DA has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_10FEDA7D()
        {
            AssertCode("@@@", "10FEDA7D");
        }
        // Reko: a decoder for the instruction 48FF30BB at address 0015854C has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_48FF30BB()
        {
            AssertCode("@@@", "48FF30BB");
        }
        // Reko: a decoder for the instruction A8EC714B at address 00194888 has not been implemented. (Unimplemented '*' when decoding ECA84B71)
        [Test]
        public void ThumbDis_A8EC714B()
        {
            AssertCode("@@@", "A8EC714B");
        }
        // Reko: a decoder for the instruction 5BEF7B13 at address 0017B0A8 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_5BEF7B13()
        {
            AssertCode("@@@", "5BEF7B13");
        }
        // Reko: a decoder for the instruction 9FFE99A9 at address 001BB43C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_9FFE99A9()
        {
            AssertCode("@@@", "9FFE99A9");
        }
        // Reko: a decoder for the instruction 7BFF1114 at address 001BB8E8 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_7BFF1114()
        {
            AssertCode("@@@", "7BFF1114");
        }
        // Reko: a decoder for the instruction B2FE5C39 at address 00138856 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B2FE5C39()
        {
            AssertCode("@@@", "B2FE5C39");
        }
        // Reko: a decoder for the instruction 8DFF5869 at address 0013888E has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FF8D6958)
        [Test]
        public void ThumbDis_8DFF5869()
        {
            AssertCode("@@@", "8DFF5869");
        }
        // Reko: a decoder for the instruction EBFD97FD at address 0017B680 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_EBFD97FD()
        {
            AssertCode("@@@", "EBFD97FD");
        }
        // Reko: a decoder for the instruction 91F92FFD at address 00194D0A has not been implemented. (PLI)
        [Test]
        public void ThumbDis_91F92FFD()
        {
            AssertCode("@@@", "91F92FFD");
        }
        // Reko: a decoder for the instruction A8EF7BD9 at address 00158A24 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding EFA8D97B)
        [Test]
        public void ThumbDis_A8EF7BD9()
        {
            AssertCode("@@@", "A8EF7BD9");
        }
        // Reko: a decoder for the instruction 53FEF4B8 at address 001DAAAE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_53FEF4B8()
        {
            AssertCode("@@@", "53FEF4B8");
        }
        // Reko: a decoder for the instruction FCFD650D at address 0017C0E6 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FCFD650D()
        {
            AssertCode("@@@", "FCFD650D");
        }
        // Reko: a decoder for the instruction 8CFDC84C at address 00159028 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8CFDC84C()
        {
            AssertCode("@@@", "8CFDC84C");
        }
        // Reko: a decoder for the instruction 5EEFAAA3 at address 0015903E has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_5EEFAAA3()
        {
            AssertCode("@@@", "5EEFAAA3");
        }
        // Reko: a decoder for the instruction 6EEFA1D1 at address 001DAADC has not been implemented. (Unimplemented '*' when decoding EF6ED1A1)
        [Test]
        public void ThumbDis_6EEFA1D1()
        {
            AssertCode("@@@", "6EEFA1D1");
        }
        // Reko: a decoder for the instruction 2EEFEC7E at address 001BBAF6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
  
        // Reko: a decoder for the instruction AFEFC0B4 at address 00138E9E has not been implemented. (Unimplemented '*scalar' when decoding EFAFB4C0)
        [Test]
        public void ThumbDis_AFEFC0B4()
        {
            AssertCode("@@@", "AFEFC0B4");
        }
        // Reko: a decoder for the instruction 2DFD5B9D at address 001DAF58 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2DFD5B9D()
        {
            AssertCode("@@@", "2DFD5B9D");
        }
        // Reko: a decoder for the instruction 07EF41B1 at address 00195828 has not been implemented. (Unimplemented '*' when decoding EF07B141)
        [Test]
        public void ThumbDis_07EF41B1()
        {
            AssertCode("@@@", "07EF41B1");
        }
        // Reko: a decoder for the instruction 1AFF32C4 at address 00138F4E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_1AFF32C4()
        {
            AssertCode("@@@", "1AFF32C4");
        }
        // Reko: a decoder for the instruction 4DEFD59D at address 001BBBD0 has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_4DEFD59D()
        {
            AssertCode("@@@", "4DEFD59D");
        }
        // Reko: a decoder for the instruction 33EFFDB9 at address 001DAFFC has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 9BFE51B9 at address 001DB008 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_9BFE51B9()
        {
            AssertCode("@@@", "9BFE51B9");
        }
        // Reko: a decoder for the instruction 64FF7F44 at address 0017C3E0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_64FF7F44()
        {
            AssertCode("@@@", "64FF7F44");
        }
        // Reko: a decoder for the instruction 9DF994F8 at address 001BBCD2 has not been implemented. (PLI)
        [Test]
        public void ThumbDis_9DF994F8()
        {
            AssertCode("@@@", "9DF994F8");
        }
        // Reko: a decoder for the instruction A6EF97EB at address 001BBDEC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_A6EF97EB()
        {
            AssertCode("@@@", "A6EF97EB");
        }
        // Reko: a decoder for the instruction 9BFAAF8D at address 0017C8D6 has not been implemented. (Unimplemented '*' when decoding FA9B8DAF)
        [Test]
        public void ThumbDis_9BFAAF8D()
        {
            AssertCode("@@@", "9BFAAF8D");
        }
        // Reko: a decoder for the instruction E6FEC5FD at address 0017C8F6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E6FEC5FD()
        {
            AssertCode("@@@", "E6FEC5FD");
        }
        // Reko: a decoder for the instruction 52F9351D at address 0017C920 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
 
        // Reko: a decoder for the instruction 53EE512B at address 001959CE has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_53EE512B()
        {
            AssertCode("@@@", "53EE512B");
        }
        // Reko: a decoder for the instruction 1AEBA81F at address 001598FC has not been implemented. (Unimplemented '*register' when decoding EB1A1FA8)
        [Test]
        public void ThumbDis_1AEBA81F()
        {
            AssertCode("@@@", "1AEBA81F");
        }
        // Reko: a decoder for the instruction BBEF3444 at address 00159942 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_BBEF3444()
        {
            AssertCode("@@@", "BBEF3444");
        }
        // Reko: a decoder for the instruction C8F92EBF at address 0017CA6E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C8F92EBF()
        {
            AssertCode("@@@", "C8F92EBF");
        }
        // Reko: a decoder for the instruction D5E8D00D at address 001BBEA6 has not been implemented. (Unimplemented '*' when decoding E8D50DD0)
        [Test]
        public void ThumbDis_D5E8D00D()
        {
            AssertCode("@@@", "D5E8D00D");
        }
        // Reko: a decoder for the instruction 16FCD88C at address 001BC0B8 has not been implemented. (op1:op2=0b0001)
  
        // Reko: a decoder for the instruction 0EEF302C at address 001DB7CE has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_0EEF302C()
        {
            AssertCode("@@@", "0EEF302C");
        }
        // Reko: a decoder for the instruction DAEF9BF4 at address 001BC0DE has not been implemented. (U=0)
        [Test]
        public void ThumbDis_DAEF9BF4()
        {
            AssertCode("@@@", "DAEF9BF4");
        }
        // Reko: a decoder for the instruction 72FC716D at address 0017CAF6 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_72FC716D()
        {
            AssertCode("@@@", "72FC716D");
        }
        // Reko: a decoder for the instruction 96EFCA6B at address 001DB82A has not been implemented. (Unimplemented '*' when decoding EF966BCA)
        [Test]
        public void ThumbDis_96EFCA6B()
        {
            AssertCode("@@@", "96EFCA6B");
        }
        // Reko: a decoder for the instruction 71EF0CBC at address 00195E36 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_71EF0CBC()
        {
            AssertCode("@@@", "71EF0CBC");
        }
        // Reko: a decoder for the instruction D6FA9116 at address 00159E3A has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D6FA9116()
        {
            AssertCode("@@@", "D6FA9116");
        }
        // Reko: a decoder for the instruction 12FF5DC9 at address 0013986C has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 10FDCB3C at address 0017CC4A has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction D0EEF0F9 at address 00195E4E has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction ACF95AD1 at address 001DC456 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9ACD15A)
        [Test]
        public void ThumbDis_ACF95AD1()
        {
            AssertCode("@@@", "ACF95AD1");
        }
        // Reko: a decoder for the instruction 6FFCDB8D at address 0015A804 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6FFCDB8D()
        {
            AssertCode("@@@", "6FFCDB8D");
        }
        // Reko: a decoder for the instruction E3F93B65 at address 0013A3D4 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9E3653B)
        [Test]
        public void ThumbDis_E3F93B65()
        {
            AssertCode("@@@", "E3F93B65");
        }
        // Reko: a decoder for the instruction 88FF61E2 at address 001962BA has not been implemented. (Unimplemented '*scalar' when decoding FF88E261)
        [Test]
        public void ThumbDis_88FF61E2()
        {
            AssertCode("@@@", "88FF61E2");
        }
        // Reko: a decoder for the instruction 59EF5132 at address 001BDB72 has not been implemented. (Unimplemented '*' when decoding EF593251)
        [Test]
        public void ThumbDis_59EF5132()
        {
            AssertCode("@@@", "59EF5132");
        }
        // Reko: a decoder for the instruction DEFF53BB at address 0015B6BA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_DEFF53BB()
        {
            AssertCode("@@@", "DEFF53BB");
        }
        // Reko: a decoder for the instruction 5BFDE418 at address 001DDDA8 has not been implemented. (op1:op2=0b1001)
   
        // Reko: a decoder for the instruction C5FEDF3D at address 00197BC6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C5FEDF3D()
        {
            AssertCode("@@@", "C5FEDF3D");
        }
        // Reko: a decoder for the instruction 1FEC34DE at address 0013CCD6 has not been implemented. (load PC)
        [Test]
        public void ThumbDis_1FEC34DE()
        {
            AssertCode("@@@", "1FEC34DE");
        }
        // Reko: a decoder for the instruction E8FD0859 at address 0013CD4C has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E8FD0859()
        {
            AssertCode("@@@", "E8FD0859");
        }
        // Reko: a decoder for the instruction 33FFD359 at address 0015D2E4 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 37FC1D38 at address 0013D344 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_37FC1D38()
        {
            AssertCode("@@@", "37FC1D38");
        }
        // Reko: a decoder for the instruction 40FDA7D9 at address 0019899A has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_40FDA7D9()
        {
            AssertCode("@@@", "40FDA7D9");
        }
        // Reko: a decoder for the instruction C7ECDBFB at address 0015D5A2 has not been implemented. (Unimplemented '*' when decoding ECC7FBDB)
        [Test]
        public void ThumbDis_C7ECDBFB()
        {
            AssertCode("@@@", "C7ECDBFB");
        }
        // Reko: a decoder for the instruction 0BFC8FAC at address 0015DA00 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0BFC8FAC()
        {
            AssertCode("@@@", "0BFC8FAC");
        }
        // Reko: a decoder for the instruction C6FF4CEE at address 00160882 has not been implemented. (Unimplemented '*' when decoding FFC6EE4C)
        [Test]
        public void ThumbDis_C6FF4CEE()
        {
            AssertCode("@@@", "C6FF4CEE");
        }
        // Reko: a decoder for the instruction 61EF403E at address 00140DD4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
  
        // Reko: a decoder for the instruction CFFD8C68 at address 001E1166 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_CFFD8C68()
        {
            AssertCode("@@@", "CFFD8C68");
        }
        // Reko: a decoder for the instruction F2F3F98B at address 00120F0E has not been implemented. (Unimplemented '*banked register' when decoding F3F28BF9)
        [Test]
        public void ThumbDis_F2F3F98B()
        {
            AssertCode("@@@", "F2F3F98B");
        }
        // Reko: a decoder for the instruction ECEFE76E at address 001A0CC4 has not been implemented. (Unimplemented '*' when decoding EFEC6EE7)
        [Test]
        public void ThumbDis_ECEFE76E()
        {
            AssertCode("@@@", "ECEFE76E");
        }
        // Reko: a decoder for the instruction 7BFCBA7D at address 00140E3A has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7BFCBA7D()
        {
            AssertCode("@@@", "7BFCBA7D");
        }
        // Reko: a decoder for the instruction 8DFDF6E8 at address 001E1188 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8DFDF6E8()
        {
            AssertCode("@@@", "8DFDF6E8");
        }
        // Reko: a decoder for the instruction CEEFFFED at address 001210B2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_CEEFFFED()
        {
            AssertCode("@@@", "CEEFFFED");
        }
        // Reko: a decoder for the instruction A8FF44BE at address 00140EEE has not been implemented. (Unimplemented '*' when decoding FFA8BE44)
        [Test]
        public void ThumbDis_A8FF44BE()
        {
            AssertCode("@@@", "A8FF44BE");
        }
        // Reko: a decoder for the instruction 6CFFCB6F at address 001E11EE has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_6CFFCB6F()
        {
            AssertCode("@@@", "6CFFCB6F");
        }
        // Reko: a decoder for the instruction EFFC42D9 at address 001015BE has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EFFC42D9()
        {
            AssertCode("@@@", "EFFC42D9");
        }
        // Reko: a decoder for the instruction 54FC9558 at address 001015C2 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction CDF9164C at address 001E1958 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CDF9164C()
        {
            AssertCode("@@@", "CDF9164C");
        }
        // Reko: a decoder for the instruction FAFFAB77 at address 001017FE has not been implemented. (Unimplemented '*' when decoding FFFA77AB)

        // Reko: a decoder for the instruction CBF313A4 at address 001E1C78 has not been implemented. (Unimplemented '*' when decoding F3CBA413)
        [Test]
        public void ThumbDis_CBF313A4()
        {
            AssertCode("@@@", "CBF313A4");
        }
        // Reko: a decoder for the instruction E5EF4DA8 at address 00121EA2 has not been implemented. (Unimplemented '*scalar' when decoding EFE5A84D)
        [Test]
        public void ThumbDis_E5EF4DA8()
        {
            AssertCode("@@@", "E5EF4DA8");
        }
        // Reko: a decoder for the instruction D5EEF95B at address 001A1662 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
  
        // Reko: a decoder for the instruction C8EF024B at address 001E1D12 has not been implemented. (Unimplemented '*integer' when decoding EFC84B02)
        [Test]
        public void ThumbDis_C8EF024B()
        {
            AssertCode("@@@", "C8EF024B");
        }
        // Reko: a decoder for the instruction 50EF1403 at address 001A22E0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_50EF1403()
        {
            AssertCode("@@@", "50EF1403");
        }
        // Reko: a decoder for the instruction 82FDFD29 at address 001A2534 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_82FDFD29()
        {
            AssertCode("@@@", "82FDFD29");
        }
        // Reko: a decoder for the instruction A9FDE2E9 at address 00141C0E has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A9FDE2E9()
        {
            AssertCode("@@@", "A9FDE2E9");
        }
        // Reko: a decoder for the instruction CEFD9D78 at address 001E2320 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_CEFD9D78()
        {
            AssertCode("@@@", "CEFD9D78");
        }
        // Reko: a decoder for the instruction F0FD2C79 at address 001C1C2A has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F0FD2C79()
        {
            AssertCode("@@@", "F0FD2C79");
        }
        // Reko: a decoder for the instruction 1EFEDECD at address 00162A58 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_1EFEDECD()
        {
            AssertCode("@@@", "1EFEDECD");
        }
        // Reko: a decoder for the instruction 8BEE3C6B at address 001E258A has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_8BEE3C6B()
        {
            AssertCode("@@@", "8BEE3C6B");
        }
        // Reko: a decoder for the instruction A1FF5E3D at address 001C1D76 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_A1FF5E3D()
        {
            AssertCode("@@@", "A1FF5E3D");
        }
        // Reko: a decoder for the instruction 76FF775E at address 00162B28 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1)
        [Test]
        public void ThumbDis_76FF775E()
        {
            AssertCode("@@@", "76FF775E");
        }
        // Reko: a decoder for the instruction C5F99B4B at address 0010255C has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C54B9B)
        [Test]
        public void ThumbDis_C5F99B4B()
        {
            AssertCode("@@@", "C5F99B4B");
        }
        // Reko: a decoder for the instruction D5EF19BD at address 00142506 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_D5EF19BD()
        {
            AssertCode("@@@", "D5EF19BD");
        }
        // Reko: a decoder for the instruction C1F9F61D at address 0010264E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C1F9F61D()
        {
            AssertCode("@@@", "C1F9F61D");
        }
        // Reko: a decoder for the instruction 34FC019D at address 0010270A has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_34FC019D()
        {
            AssertCode("@@@", "34FC019D");
        }
        // Reko: a decoder for the instruction 35FCDBFC at address 001830A8 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_35FCDBFC()
        {
            AssertCode("@@@", "35FCDBFC");
        }
        // Reko: a decoder for the instruction E5FCAFAC at address 001830C2 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E5FCAFAC()
        {
            AssertCode("@@@", "E5FCAFAC");
        }
        // Reko: a decoder for the instruction 32EFFF9D at address 001A2A70 has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_32EFFF9D()
        {
            AssertCode("@@@", "32EFFF9D");
        }
        // Reko: a decoder for the instruction D9FCC2E9 at address 001426A6 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D9FCC2E9()
        {
            AssertCode("@@@", "D9FCC2E9");
        }
        // Reko: a decoder for the instruction 3FFD8BC9 at address 00122E30 has not been implemented. (load PC)
        [Test]
        public void ThumbDis_3FFD8BC9()
        {
            AssertCode("@@@", "3FFD8BC9");
        }
        // Reko: a decoder for the instruction 4BFE48FD at address 001630B8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_4BFE48FD()
        {
            AssertCode("@@@", "4BFE48FD");
        }
        // Reko: a decoder for the instruction 4AEFFB7D at address 001638CE has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_4AEFFB7D()
        {
            AssertCode("@@@", "4AEFFB7D");
        }
        // Reko: a decoder for the instruction B1FC9C09 at address 001C2888 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B1FC9C09()
        {
            AssertCode("@@@", "B1FC9C09");
        }
        // Reko: a decoder for the instruction CAEF41CA at address 001A398A has not been implemented. (Unimplemented '*' when decoding EFCACA41)
  
        // Reko: a decoder for the instruction 61FE9A69 at address 00123784 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_61FE9A69()
        {
            AssertCode("@@@", "61FE9A69");
        }
        // Reko: a decoder for the instruction 3BF9211F at address 00143446 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_3BF9211F()
        {
            AssertCode("@@@", "3BF9211F");
        }
        // Reko: a decoder for the instruction AAEEF3F9 at address 001E2FE8 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction E3F93CC9 at address 001A3A12 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9E3C93C)

        // Reko: a decoder for the instruction 49EFEB1B at address 001841BC has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_49EFEB1B()
        {
            AssertCode("@@@", "49EFEB1B");
        }
        // Reko: a decoder for the instruction 6EEEB669 at address 001A4110 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_6EEEB669()
        {
            AssertCode("@@@", "6EEEB669");
        }
        // Reko: a decoder for the instruction 0EFF261C at address 001A4262 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_0EFF261C()
        {
            AssertCode("@@@", "0EFF261C");
        }
        // Reko: a decoder for the instruction 87EFB918 at address 0016402E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_87EFB918()
        {
            AssertCode("@@@", "87EFB918");
        }
        // Reko: a decoder for the instruction C6F98503 at address 001438FA has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F9C60385)
        [Test]
        public void ThumbDis_C6F98503()
        {
            AssertCode("@@@", "C6F98503");
        }
        // Reko: a decoder for the instruction 7FFF019C at address 001E3932 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_7FFF019C()
        {
            AssertCode("@@@", "7FFF019C");
        }
        // Reko: a decoder for the instruction 6CFD37A8 at address 001E3D58 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6CFD37A8()
        {
            AssertCode("@@@", "6CFD37A8");
        }
        // Reko: a decoder for the instruction 3CFF496B at address 001A4ADE has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_3CFF496B()
        {
            AssertCode("@@@", "3CFF496B");
        }
        // Reko: a decoder for the instruction DBFCE8DC at address 00143DB0 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DBFCE8DC()
        {
            AssertCode("@@@", "DBFCE8DC");
        }

        // Reko: a decoder for the instruction 6EEF5B3C at address 001E3D98 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_6EEF5B3C()
        {
            AssertCode("@@@", "6EEF5B3C");
        }
        // Reko: a decoder for the instruction 6AEF0FCB at address 001E3DC4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_6AEF0FCB()
        {
            AssertCode("@@@", "6AEF0FCB");
        }
        // Reko: a decoder for the instruction D1FDC418 at address 001A4B5A has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D1FDC418()
        {
            AssertCode("@@@", "D1FDC418");
        }
        // Reko: a decoder for the instruction FCEEFB1B at address 00123CB2 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction CDFC3B3D at address 001A4C9A has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CDFC3B3D()
        {
            AssertCode("@@@", "CDFC3B3D");
        }
        // Reko: a decoder for the instruction 0AFDC459 at address 001E3F90 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0AFDC459()
        {
            AssertCode("@@@", "0AFDC459");
        }
        // Reko: a decoder for the instruction 8DFC14D8 at address 00143F4A has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8DFC14D8()
        {
            AssertCode("@@@", "8DFC14D8");
        }
        // Reko: a decoder for the instruction 3EFF5622 at address 00123EE2 has not been implemented. (Unimplemented '*' when decoding FF3E2256)
        [Test]
        public void ThumbDis_3EFF5622()
        {
            AssertCode("@@@", "3EFF5622");
        }
        // Reko: a decoder for the instruction 79F9DC0D at address 001644F8 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
 
        // Reko: a decoder for the instruction A6F90D2C at address 001E4326 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A6F90D2C()
        {
            AssertCode("@@@", "A6F90D2C");
        }
        // Reko: a decoder for the instruction A0FC86AD at address 00185070 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A0FC86AD()
        {
            AssertCode("@@@", "A0FC86AD");
        }
        // Reko: a decoder for the instruction 13FEA2DC at address 0018524C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_13FEA2DC()
        {
            AssertCode("@@@", "13FEA2DC");
        }
        // Reko: a decoder for the instruction ADF9EAE4 at address 001A51B4 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9ADE4EA)
        [Test]
        public void ThumbDis_ADF9EAE4()
        {
            AssertCode("@@@", "ADF9EAE4");
        }
        // Reko: a decoder for the instruction 55FEF6E8 at address 00164960 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_55FEF6E8()
        {
            AssertCode("@@@", "55FEF6E8");
        }
        // Reko: a decoder for the instruction 62EF35F2 at address 001E4806 has not been implemented. (Unimplemented '*' when decoding EF62F235)
        [Test]
        public void ThumbDis_62EF35F2()
        {
            AssertCode("@@@", "62EF35F2");
        }
        // Reko: a decoder for the instruction 4CFFE704 at address 00185424 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_4CFFE704()
        {
            AssertCode("@@@", "4CFFE704");
        }
        // Reko: a decoder for the instruction E8EF1BF4 at address 0010433A has not been implemented. (U=0)
        [Test]
        public void ThumbDis_E8EF1BF4()
        {
            AssertCode("@@@", "E8EF1BF4");
        }
        // Reko: a decoder for the instruction 45FCE849 at address 001C4174 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_45FCE849()
        {
            AssertCode("@@@", "45FCE849");
        }
        // Reko: a decoder for the instruction 42EF791B at address 001E4E06 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_42EF791B()
        {
            AssertCode("@@@", "42EF791B");
        }
        // Reko: a decoder for the instruction 60EF9643 at address 001859A2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_60EF9643()
        {
            AssertCode("@@@", "60EF9643");
        }
        // Reko: a decoder for the instruction 4AFCC899 at address 00104342 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4AFCC899()
        {
            AssertCode("@@@", "4AFCC899");
        }
        // Reko: a decoder for the instruction CBF94047 at address 001448DC has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F9CB4740)
        [Test]
        public void ThumbDis_CBF94047()
        {
            AssertCode("@@@", "CBF94047");
        }
        // Reko: a decoder for the instruction CEEF3074 at address 00124CF4 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_CEEF3074()
        {
            AssertCode("@@@", "CEEF3074");
        }
        // Reko: a decoder for the instruction 44FE7AAD at address 00124D1C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_44FE7AAD()
        {
            AssertCode("@@@", "44FE7AAD");
        }
        // Reko: a decoder for the instruction 58EEF23B at address 001A54C2 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_58EEF23B()
        {
            AssertCode("@@@", "58EEF23B");
        }
        // Reko: a decoder for the instruction 87F906D1 at address 001E5A36 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F987D106)
        [Test]
        public void ThumbDis_87F906D1()
        {
            AssertCode("@@@", "87F906D1");
        }
        // Reko: a decoder for the instruction 75FF7414 at address 001E5A3E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_75FF7414()
        {
            AssertCode("@@@", "75FF7414");
        }
        // Reko: a decoder for the instruction 8BEC19BB at address 00104D38 has not been implemented. (Unimplemented '*' when decoding EC8BBB19)
        [Test]
        public void ThumbDis_8BEC19BB()
        {
            AssertCode("@@@", "8BEC19BB");
        }
        // Reko: a decoder for the instruction D2FD4FDC at address 00144C48 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D2FD4FDC()
        {
            AssertCode("@@@", "D2FD4FDC");
        }
        // Reko: a decoder for the instruction 85FE65EC at address 001863A0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_85FE65EC()
        {
            AssertCode("@@@", "85FE65EC");
        }
        // Reko: a decoder for the instruction EEECC52A at address 001863D8 has not been implemented. (Unimplemented '*' when decoding ECEE2AC5)
        [Test]
        public void ThumbDis_EEECC52A()
        {
            AssertCode("@@@", "EEECC52A");
        }
        // Reko: a decoder for the instruction AEFDDC89 at address 001A5A84 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_AEFDDC89()
        {
            AssertCode("@@@", "AEFDDC89");
        }
        // Reko: a decoder for the instruction 39FF107C at address 001654DC has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_39FF107C()
        {
            AssertCode("@@@", "39FF107C");
        }
        // Reko: a decoder for the instruction CCFC6618 at address 00125798 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CCFC6618()
        {
            AssertCode("@@@", "CCFC6618");
        }
        // Reko: a decoder for the instruction 44FC4B1D at address 001872E8 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_44FC4B1D()
        {
            AssertCode("@@@", "44FC4B1D");
        }
        // Reko: a decoder for the instruction 72FFDB8F at address 001E645C has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_72FFDB8F()
        {
            AssertCode("@@@", "72FFDB8F");
        }
        // Reko: a decoder for the instruction 47FCF1BC at address 00165AAE has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_47FCF1BC()
        {
            AssertCode("@@@", "47FCF1BC");
        }
        // Reko: a decoder for the instruction C3F3F486 at address 001C50A6 has not been implemented. (Unimplemented '*' when decoding F3C386F4)
        [Test]
        public void ThumbDis_C3F3F486()
        {
            AssertCode("@@@", "C3F3F486");
        }
        // Reko: a decoder for the instruction C1F98D32 at address 001257CA has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9C1328D)
        [Test]
        public void ThumbDis_C1F98D32()
        {
            AssertCode("@@@", "C1F98D32");
        }
        // Reko: a decoder for the instruction D0FA95F0 at address 001455C0 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D0FA95F0()
        {
            AssertCode("@@@", "D0FA95F0");
        }
        // Reko: a decoder for the instruction 5AFCA4C9 at address 0018746E has not been implemented. (op1:op2=0b0001)
 
        // Reko: a decoder for the instruction 6AFF891B at address 001C5130 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_6AFF891B()
        {
            AssertCode("@@@", "6AFF891B");
        }
        // Reko: a decoder for the instruction AAFF9E6B at address 00145720 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_AAFF9E6B()
        {
            AssertCode("@@@", "AAFF9E6B");
        }
        // Reko: a decoder for the instruction A2F92B9A at address 001C519C has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9A29A2B)
        [Test]
        public void ThumbDis_A2F92B9A()
        {
            AssertCode("@@@", "A2F92B9A");
        }
        // Reko: a decoder for the instruction 0FEF37D4 at address 00105A2E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_0FEF37D4()
        {
            AssertCode("@@@", "0FEF37D4");
        }
        // Reko: a decoder for the instruction E9EC2EFA at address 00187C64 has not been implemented. (Unimplemented '*' when decoding ECE9FA2E)
        [Test]
        public void ThumbDis_E9EC2EFA()
        {
            AssertCode("@@@", "E9EC2EFA");
        }
        // Reko: a decoder for the instruction 55FFAFF1 at address 001C5992 has not been implemented. (Unimplemented '*' when decoding FF55F1AF)
        [Test]
        public void ThumbDis_55FFAFF1()
        {
            AssertCode("@@@", "55FFAFF1");
        }
        // Reko: a decoder for the instruction E1FD86F9 at address 00145D54 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E1FD86F9()
        {
            AssertCode("@@@", "E1FD86F9");
        }
        // Reko: a decoder for the instruction E7FC5AB8 at address 001C59C6 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E7FC5AB8()
        {
            AssertCode("@@@", "E7FC5AB8");
        }
        // Reko: a decoder for the instruction 6FEF7AF4 at address 00166768 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_6FEF7AF4()
        {
            AssertCode("@@@", "6FEF7AF4");
        }
        // Reko: a decoder for the instruction E9F96D64 at address 0012609E has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9E9646D)
        [Test]
        public void ThumbDis_E9F96D64()
        {
            AssertCode("@@@", "E9F96D64");
        }
        // Reko: a decoder for the instruction 52FF3A2E at address 001C5B94 has not been implemented. (Unimplemented '*' when decoding FF522E3A)
        [Test]
        public void ThumbDis_52FF3A2E()
        {
            AssertCode("@@@", "52FF3A2E");
        }
        // Reko: a decoder for the instruction 8BEF436A at address 001E7138 has not been implemented. (Unimplemented '*' when decoding EF8B6A43)
        [Test]
        public void ThumbDis_8BEF436A()
        {
            AssertCode("@@@", "8BEF436A");
        }
        // Reko: a decoder for the instruction 18FF3D1F at address 001667F4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_18FF3D1F()
        {
            AssertCode("@@@", "18FF3D1F");
        }
        // Reko: a decoder for the instruction 87EF996D at address 001260AA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_87EF996D()
        {
            AssertCode("@@@", "87EF996D");
        }
        // Reko: a decoder for the instruction D9FA9D8E at address 001E7A50 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D9FA9D8E()
        {
            AssertCode("@@@", "D9FA9D8E");
        }
        // Reko: a decoder for the instruction A0F3F1A4 at address 001261AE has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A0F3F1A4()
        {
            AssertCode("@@@", "A0F3F1A4");
        }
        // Reko: a decoder for the instruction 6BEF1151 at address 00106670 has not been implemented. (Unimplemented '*register' when decoding EF6B5111)

        // Reko: a decoder for the instruction 6BFE5B48 at address 001C6198 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6BFE5B48()
        {
            AssertCode("@@@", "6BFE5B48");
        }
        // Reko: a decoder for the instruction 81FDFBA9 at address 001467F6 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_81FDFBA9()
        {
            AssertCode("@@@", "81FDFBA9");
        }
        // Reko: a decoder for the instruction 5BFC901C at address 0014684C has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 61EFE233 at address 00106AFE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_61EFE233()
        {
            AssertCode("@@@", "61EFE233");
        }
        // Reko: a decoder for the instruction 60FCB9E8 at address 00106B8A has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_60FCB9E8()
        {
            AssertCode("@@@", "60FCB9E8");
        }
        // Reko: a decoder for the instruction 9DFD822D at address 001468EA has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_9DFD822D()
        {
            AssertCode("@@@", "9DFD822D");
        }
        // Reko: a decoder for the instruction C1FE963C at address 001A7BA8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C1FE963C()
        {
            AssertCode("@@@", "C1FE963C");
        }
        // Reko: a decoder for the instruction 1FFF7AF1 at address 00167AF0 has not been implemented. (Unimplemented '*register' when decoding FF1FF17A)
        [Test]
        public void ThumbDis_1FFF7AF1()
        {
            AssertCode("@@@", "1FFF7AF1");
        }
        // Reko: a decoder for the instruction 9DFAB5FD at address 00167B08 has not been implemented. (Unimplemented '*' when decoding FA9DFDB5)
        [Test]
        public void ThumbDis_9DFAB5FD()
        {
            AssertCode("@@@", "9DFAB5FD");
        }
        // Reko: a decoder for the instruction 96FF712D at address 001079F2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_96FF712D()
        {
            AssertCode("@@@", "96FF712D");
        }
        // Reko: a decoder for the instruction 7EFF632B at address 001A7CB6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_7EFF632B()
        {
            AssertCode("@@@", "7EFF632B");
        }
        // Reko: a decoder for the instruction CFEF6768 at address 001E92E0 has not been implemented. (Unimplemented '*scalar' when decoding EFCF6867)
        [Test]
        public void ThumbDis_CFEF6768()
        {
            AssertCode("@@@", "CFEF6768");
        }
        // Reko: a decoder for the instruction 47EFE424 at address 00167C1C has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_47EFE424()
        {
            AssertCode("@@@", "47EFE424");
        }
        // Reko: a decoder for the instruction B4FC42BC at address 001A7D28 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B4FC42BC()
        {
            AssertCode("@@@", "B4FC42BC");
        }
        // Reko: a decoder for the instruction 01FC17F8 at address 001E936A has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_01FC17F8()
        {
            AssertCode("@@@", "01FC17F8");
        }
        // Reko: a decoder for the instruction C5F3F480 at address 001A7E04 has not been implemented. (Unimplemented '*' when decoding F3C580F4)
        [Test]
        public void ThumbDis_C5F3F480()
        {
            AssertCode("@@@", "C5F3F480");
        }
        // Reko: a decoder for the instruction 38FF7A84 at address 00107DFC has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_38FF7A84()
        {
            AssertCode("@@@", "38FF7A84");
        }
        // Reko: a decoder for the instruction 94FFCEF8 at address 001E9384 has not been implemented. (Unimplemented '*scalar' when decoding FF94F8CE)
        [Test]
        public void ThumbDis_94FFCEF8()
        {
            AssertCode("@@@", "94FFCEF8");
        }
        // Reko: a decoder for the instruction 2EFFA723 at address 00127836 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2EFFA723()
        {
            AssertCode("@@@", "2EFFA723");
        }
        // Reko: a decoder for the instruction F3FDF639 at address 00188CB4 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F3FDF639()
        {
            AssertCode("@@@", "F3FDF639");
        }
        // Reko: a decoder for the instruction 1DFE9AB9 at address 00147A34 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_1DFE9AB9()
        {
            AssertCode("@@@", "1DFE9AB9");
        }
        // Reko: a decoder for the instruction 1BF94BBE at address 00108212 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_1BF94BBE()
        {
            AssertCode("@@@", "1BF94BBE");
        }
        // Reko: a decoder for the instruction C7F335A0 at address 001E9C2A has not been implemented. (Unimplemented '*' when decoding F3C7A035)
        [Test]
        public void ThumbDis_C7F335A0()
        {
            AssertCode("@@@", "C7F335A0");
        }
        // Reko: a decoder for the instruction ECEC779B at address 001E9C42 has not been implemented. (Unimplemented '*' when decoding ECEC9B77)
        [Test]
        public void ThumbDis_ECEC779B()
        {
            AssertCode("@@@", "ECEC779B");
        }
        // Reko: a decoder for the instruction DBFCF47D at address 00127E06 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DBFCF47D()
        {
            AssertCode("@@@", "DBFCF47D");
        }
        // Reko: a decoder for the instruction 6EFE4F7C at address 00127E2A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6EFE4F7C()
        {
            AssertCode("@@@", "6EFE4F7C");
        }
        // Reko: a decoder for the instruction 1CFF0513 at address 00147F62 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1CFF0513()
        {
            AssertCode("@@@", "1CFF0513");
        }
        // Reko: a decoder for the instruction 7BFC3158 at address 001A8572 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7BFC3158()
        {
            AssertCode("@@@", "7BFC3158");
        }
        // Reko: a decoder for the instruction 97FCF3B9 at address 0010878C has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_97FCF3B9()
        {
            AssertCode("@@@", "97FCF3B9");
        }
        // Reko: a decoder for the instruction A8FDA949 at address 001A8B18 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A8FDA949()
        {
            AssertCode("@@@", "A8FDA949");
        }
        // Reko: a decoder for the instruction C8F9120D at address 00168C2C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C8F9120D()
        {
            AssertCode("@@@", "C8F9120D");
        }
        // Reko: a decoder for the instruction C0EE5D4B at address 001C75FA has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 3CFF2431 at address 001A8B90 has not been implemented. (Unimplemented '*' when decoding FF3C3124)
        [Test]
        public void ThumbDis_3CFF2431()
        {
            AssertCode("@@@", "3CFF2431");
        }
        // Reko: a decoder for the instruction B5EE109B at address 001A8E56 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction 5BFFB42F at address 001A8E92 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_5BFFB42F()
        {
            AssertCode("@@@", "5BFFB42F");
        }
        // Reko: a decoder for the instruction 70F9804D at address 001A8EC2 has not been implemented. (LoadStoreSignedImmediatePreIndexed)

        // Reko: a decoder for the instruction 48FF93FB at address 001C7612 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)

        // Reko: a decoder for the instruction 21FFBD2C at address 001C7670 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_21FFBD2C()
        {
            AssertCode("@@@", "21FFBD2C");
        }
        // Reko: a decoder for the instruction C4F3818E at address 001285DA has not been implemented. (Unimplemented '*' when decoding F3C48E81)
        [Test]
        public void ThumbDis_C4F3818E()
        {
            AssertCode("@@@", "C4F3818E");
        }
        // Reko: a decoder for the instruction C4FF4C02 at address 001489EE has not been implemented. (Unimplemented '*scalar' when decoding FFC4024C)
        [Test]
        public void ThumbDis_C4FF4C02()
        {
            AssertCode("@@@", "C4FF4C02");
        }
        // Reko: a decoder for the instruction AEF9720A at address 001895AC has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9AE0A72)
        [Test]
        public void ThumbDis_AEF9720A()
        {
            AssertCode("@@@", "AEF9720A");
        }
        // Reko: a decoder for the instruction BBFDE56D at address 00108CAC has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_BBFDE56D()
        {
            AssertCode("@@@", "BBFDE56D");
        }
        // Reko: a decoder for the instruction 98EFE77D at address 001A9006 has not been implemented. (Unimplemented '*' when decoding EF987DE7)
        [Test]
        public void ThumbDis_98EFE77D()
        {
            AssertCode("@@@", "98EFE77D");
        }
        // Reko: a decoder for the instruction 46EF52D4 at address 001A9032 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_46EF52D4()
        {
            AssertCode("@@@", "46EF52D4");
        }
        // Reko: a decoder for the instruction 19FF40E1 at address 001899A4 has not been implemented. (Unimplemented '*' when decoding FF19E140)
        [Test]
        public void ThumbDis_19FF40E1()
        {
            AssertCode("@@@", "19FF40E1");
        }
        // Reko: a decoder for the instruction 8EFCBB98 at address 00168F4E has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8EFCBB98()
        {
            AssertCode("@@@", "8EFCBB98");
        }
        // Reko: a decoder for the instruction E8F92CC5 at address 001C7CD2 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9E8C52C)
        [Test]
        public void ThumbDis_E8F92CC5()
        {
            AssertCode("@@@", "E8F92CC5");
        }
        // Reko: a decoder for the instruction 80F92902 at address 001C8740 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9800229)
        [Test]
        public void ThumbDis_80F92902()
        {
            AssertCode("@@@", "80F92902");
        }
        // Reko: a decoder for the instruction 2BEF5CA2 at address 00109D8C has not been implemented. (Unimplemented '*' when decoding EF2BA25C)
        [Test]
        public void ThumbDis_2BEF5CA2()
        {
            AssertCode("@@@", "2BEF5CA2");
        }
        // Reko: a decoder for the instruction D8FDFEDC at address 0016A012 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D8FDFEDC()
        {
            AssertCode("@@@", "D8FDFEDC");
        }
        // Reko: a decoder for the instruction C2F9A7DE at address 0016A056 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C2F9A7DE()
        {
            AssertCode("@@@", "C2F9A7DE");
        }
        // Reko: a decoder for the instruction 24FFF63E at address 001EBE04 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1)
        [Test]
        public void ThumbDis_24FFF63E()
        {
            AssertCode("@@@", "24FFF63E");
        }
        // Reko: a decoder for the instruction E3FD215C at address 001AA48A has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E3FD215C()
        {
            AssertCode("@@@", "E3FD215C");
        }
        // Reko: a decoder for the instruction 1EEF5F24 at address 001AA48E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_1EEF5F24()
        {
            AssertCode("@@@", "1EEF5F24");
        }
        // Reko: a decoder for the instruction B5F32E84 at address 001AA4C8 has not been implemented. (Unimplemented '*' when decoding F3B5842E)

        // Reko: a decoder for the instruction 8AECCA1B at address 0016A102 has not been implemented. (Unimplemented '*' when decoding EC8A1BCA)
  
        // Reko: a decoder for the instruction E7EFB038 at address 001EBE16 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_E7EFB038()
        {
            AssertCode("@@@", "E7EFB038");
        }
        // Reko: a decoder for the instruction 2DFF3624 at address 00109F40 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_2DFF3624()
        {
            AssertCode("@@@", "2DFF3624");
        }
        // Reko: a decoder for the instruction 38FDE608 at address 001EBF12 has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_38FDE608()
        {
            AssertCode("@@@", "38FDE608");
        }
        // Reko: a decoder for the instruction 58FF3BA4 at address 00109F9A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_58FF3BA4()
        {
            AssertCode("@@@", "58FF3BA4");
        }
        // Reko: a decoder for the instruction 0EEF78F2 at address 001C8970 has not been implemented. (Unimplemented '*' when decoding EF0EF278)
        [Test]
        public void ThumbDis_0EEF78F2()
        {
            AssertCode("@@@", "0EEF78F2");
        }
        // Reko: a decoder for the instruction DBE8F757 at address 001EBF74 has not been implemented. (Unimplemented '*' when decoding E8DB57F7)
        [Test]
        public void ThumbDis_DBE8F757()
        {
            AssertCode("@@@", "DBE8F757");
        }
        // Reko: a decoder for the instruction 55FC909C at address 00109FA0 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction D4FAA64A at address 001C8F60 has not been implemented. (crc32c-crc32cw)
        [Test]
        public void ThumbDis_D4FAA64A()
        {
            AssertCode("@@@", "D4FAA64A");
        }
        // Reko: a decoder for the instruction 10FC8A6D at address 0014A7B6 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 8BECA8AB at address 0010A13C has not been implemented. (Unimplemented '*' when decoding EC8BABA8)
        [Test]
        public void ThumbDis_8BECA8AB()
        {
            AssertCode("@@@", "8BECA8AB");
        }
        // Reko: a decoder for the instruction 2FFE3F99 at address 0016A7E6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_2FFE3F99()
        {
            AssertCode("@@@", "2FFE3F99");
        }
        // Reko: a decoder for the instruction B5FCDDBD at address 001C91C6 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B5FCDDBD()
        {
            AssertCode("@@@", "B5FCDDBD");
        }
        // Reko: a decoder for the instruction E0F9110A at address 001EC46C has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9E00A11)
        [Test]
        public void ThumbDis_E0F9110A()
        {
            AssertCode("@@@", "E0F9110A");
        }
        // Reko: a decoder for the instruction 53FCB7F8 at address 0014ACCC has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 21FE38F9 at address 0016ADF2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_21FE38F9()
        {
            AssertCode("@@@", "21FE38F9");
        }
        // Reko: a decoder for the instruction 05FF4D91 at address 0018B9D8 has not been implemented. (Unimplemented '*' when decoding FF05914D)
        [Test]
        public void ThumbDis_05FF4D91()
        {
            AssertCode("@@@", "05FF4D91");
        }
        // Reko: a decoder for the instruction EEFE3568 at address 001EC78A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EEFE3568()
        {
            AssertCode("@@@", "EEFE3568");
        }
        // Reko: a decoder for the instruction E4F9DC78 at address 001AAEC2 has not been implemented. (Unimplemented 'single element from one lane - T3' when decoding F9E478DC)
        [Test]
        public void ThumbDis_E4F9DC78()
        {
            AssertCode("@@@", "E4F9DC78");
        }
        // Reko: a decoder for the instruction A5F3678C at address 001AAF04 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A5F3678C()
        {
            AssertCode("@@@", "A5F3678C");
        }
        // Reko: a decoder for the instruction 3DEF6394 at address 0010A8EE has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_3DEF6394()
        {
            AssertCode("@@@", "3DEF6394");
        }
        // Reko: a decoder for the instruction 7EFEEEAC at address 0014B3AC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_7EFEEEAC()
        {
            AssertCode("@@@", "7EFEEEAC");
        }
        // Reko: a decoder for the instruction 3DEFCCC1 at address 0014B4A4 has not been implemented. (Unimplemented '*' when decoding EF3DC1CC)
        [Test]
        public void ThumbDis_3DEFCCC1()
        {
            AssertCode("@@@", "3DEFCCC1");
        }
        // Reko: a decoder for the instruction ADFDA05C at address 0012A520 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_ADFDA05C()
        {
            AssertCode("@@@", "ADFDA05C");
        }
        // Reko: a decoder for the instruction EBF94AAC at address 0018C480 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_EBF94AAC()
        {
            AssertCode("@@@", "EBF94AAC");
        }
        // Reko: a decoder for the instruction 68FF25C1 at address 001ECA02 has not been implemented. (Unimplemented '*' when decoding FF68C125)
        [Test]
        public void ThumbDis_68FF25C1()
        {
            AssertCode("@@@", "68FF25C1");
        }
        // Reko: a decoder for the instruction EFFE4DFD at address 0014B536 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EFFE4DFD()
        {
            AssertCode("@@@", "EFFE4DFD");
        }
        // Reko: a decoder for the instruction BBFE74DC at address 0012A9EC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_BBFE74DC()
        {
            AssertCode("@@@", "BBFE74DC");
        }
        // Reko: a decoder for the instruction 2BFD0EC8 at address 001ECF78 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2BFD0EC8()
        {
            AssertCode("@@@", "2BFD0EC8");
        }
        // Reko: a decoder for the instruction F7FD47EC at address 0016B56E has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F7FD47EC()
        {
            AssertCode("@@@", "F7FD47EC");
        }
        // Reko: a decoder for the instruction 42EFBC53 at address 0010B0FA has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_42EFBC53()
        {
            AssertCode("@@@", "42EFBC53");
        }
        // Reko: a decoder for the instruction 2DEFECF3 at address 001ED076 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2DEFECF3()
        {
            AssertCode("@@@", "2DEFECF3");
        }
        // Reko: a decoder for the instruction D2E8FA0E at address 0016B5BE has not been implemented. (Unimplemented '*' when decoding E8D20EFA)
        [Test]
        public void ThumbDis_D2E8FA0E()
        {
            AssertCode("@@@", "D2E8FA0E");
        }
        // Reko: a decoder for the instruction A0FDFB99 at address 0018CAB2 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A0FDFB99()
        {
            AssertCode("@@@", "A0FDFB99");
        }
        // Reko: a decoder for the instruction 76FF1E7C at address 0010B21A has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_76FF1E7C()
        {
            AssertCode("@@@", "76FF1E7C");
        }
        // Reko: a decoder for the instruction 75F9793D at address 001ED0DA has not been implemented. (LoadStoreSignedImmediatePreIndexed)

        // Reko: a decoder for the instruction 97FABCA1 at address 0010B748 has not been implemented. (Unimplemented '*' when decoding FA97A1BC)

        // Reko: a decoder for the instruction 1CF9350F at address 0018D0A2 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_1CF9350F()
        {
            AssertCode("@@@", "1CF9350F");
        }
        // Reko: a decoder for the instruction F6F7CA8F at address 001AC428 has not been implemented. (Unimplemented '*' when decoding F7F68FCA)
        [Test]
        public void ThumbDis_F6F7CA8F()
        {
            AssertCode("@@@", "F6F7CA8F");
        }
        // Reko: a decoder for the instruction C4FEB12C at address 001AC42C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C4FEB12C()
        {
            AssertCode("@@@", "C4FEB12C");
        }
        // Reko: a decoder for the instruction 39FFE9CB at address 0012BA56 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_39FFE9CB()
        {
            AssertCode("@@@", "39FFE9CB");
        }
        // Reko: a decoder for the instruction 36FE7F4C at address 0010B8DE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_36FE7F4C()
        {
            AssertCode("@@@", "36FE7F4C");
        }
        // Reko: a decoder for the instruction FCFC9CD9 at address 0014BF96 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_FCFC9CD9()
        {
            AssertCode("@@@", "FCFC9CD9");
        }
        // Reko: a decoder for the instruction 48EF7F7B at address 0016BDB8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_48EF7F7B()
        {
            AssertCode("@@@", "48EF7F7B");
        }
        // Reko: a decoder for the instruction 83EFBC8C at address 0016BDCA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_83EFBC8C()
        {
            AssertCode("@@@", "83EFBC8C");
        }
        // Reko: a decoder for the instruction 47FDD398 at address 0018D816 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_47FDD398()
        {
            AssertCode("@@@", "47FDD398");
        }
        // Reko: a decoder for the instruction A9FC9FDC at address 0018D8F4 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A9FC9FDC()
        {
            AssertCode("@@@", "A9FC9FDC");
        }
        // Reko: a decoder for the instruction A2F9A07E at address 0014C08C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A2F9A07E()
        {
            AssertCode("@@@", "A2F9A07E");
        }
        // Reko: a decoder for the instruction 89EFEB42 at address 001ACBDA has not been implemented. (Unimplemented '*scalar' when decoding EF8942EB)
        [Test]
        public void ThumbDis_89EFEB42()
        {
            AssertCode("@@@", "89EFEB42");
        }
        // Reko: a decoder for the instruction 8BEFE982 at address 0012BCD6 has not been implemented. (Unimplemented '*scalar' when decoding EF8B82E9)
        [Test]
        public void ThumbDis_8BEFE982()
        {
            AssertCode("@@@", "8BEFE982");
        }
        // Reko: a decoder for the instruction C8EFE258 at address 0014C59E has not been implemented. (Unimplemented '*scalar' when decoding EFC858E2)
        [Test]
        public void ThumbDis_C8EFE258()
        {
            AssertCode("@@@", "C8EFE258");
        }
        // Reko: a decoder for the instruction E9FE1E09 at address 0018E20C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E9FE1E09()
        {
            AssertCode("@@@", "E9FE1E09");
        }
        // Reko: a decoder for the instruction E7FDF80C at address 0010C412 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E7FDF80C()
        {
            AssertCode("@@@", "E7FDF80C");
        }
        // Reko: a decoder for the instruction 6AFEBA6C at address 0014CA24 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6AFEBA6C()
        {
            AssertCode("@@@", "6AFEBA6C");
        }
        // Reko: a decoder for the instruction 01FE9C89 at address 001ACD04 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_01FE9C89()
        {
            AssertCode("@@@", "01FE9C89");
        }
        // Reko: a decoder for the instruction 52FCB48C at address 001ACE40 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 90EF7454 at address 001EEBA4 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_90EF7454()
        {
            AssertCode("@@@", "90EF7454");
        }
        // Reko: a decoder for the instruction A3FF75B9 at address 001EEC2E has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FFA3B975)
        [Test]
        public void ThumbDis_A3FF75B9()
        {
            AssertCode("@@@", "A3FF75B9");
        }
        // Reko: a decoder for the instruction 3EF9388D at address 001EEC56 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_3EF9388D()
        {
            AssertCode("@@@", "3EF9388D");
        }
        // Reko: a decoder for the instruction D6FDDED8 at address 0010CAC4 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D6FDDED8()
        {
            AssertCode("@@@", "D6FDDED8");
        }
        // Reko: a decoder for the instruction 89F92A77 at address 0016CB82 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F989772A)
        [Test]
        public void ThumbDis_89F92A77()
        {
            AssertCode("@@@", "89F92A77");
        }
        // Reko: a decoder for the instruction FFFE585D at address 0014CB28 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_FFFE585D()
        {
            AssertCode("@@@", "FFFE585D");
        }
        // Reko: a decoder for the instruction E4F94192 at address 0018F058 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9E49241)
        [Test]
        public void ThumbDis_E4F94192()
        {
            AssertCode("@@@", "E4F94192");
        }
        // Reko: a decoder for the instruction 5FFCBA0E at address 0016CF06 has not been implemented. (load PC)
        [Test]
        public void ThumbDis_5FFCBA0E()
        {
            AssertCode("@@@", "5FFCBA0E");
        }
        // Reko: a decoder for the instruction 73FF6524 at address 0014CD76 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_73FF6524()
        {
            AssertCode("@@@", "73FF6524");
        }
        // Reko: a decoder for the instruction 76FC52F9 at address 0018FF0C has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_76FC52F9()
        {
            AssertCode("@@@", "76FC52F9");
        }
        // Reko: a decoder for the instruction C0FFF73C at address 00190524 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_C0FFF73C()
        {
            AssertCode("@@@", "C0FFF73C");
        }
        // Reko: a decoder for the instruction A4FD7CC8 at address 001F0A4C has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A4FD7CC8()
        {
            AssertCode("@@@", "A4FD7CC8");
        }
        // Reko: a decoder for the instruction 35EFB2E4 at address 001CD42A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_35EFB2E4()
        {
            AssertCode("@@@", "35EFB2E4");
        }
        // Reko: a decoder for the instruction 8EEF019D at address 00190778 has not been implemented. (Unimplemented '*integer' when decoding EF8E9D01)
        [Test]
        public void ThumbDis_8EEF019D()
        {
            AssertCode("@@@", "8EEF019D");
        }
        // Reko: a decoder for the instruction 95EFBB1D at address 0010F1FC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_95EFBB1D()
        {
            AssertCode("@@@", "95EFBB1D");
        }
        // Reko: a decoder for the instruction 89FF5E8B at address 001CDB00 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_89FF5E8B()
        {
            AssertCode("@@@", "89FF5E8B");
        }
        // Reko: a decoder for the instruction AAF9CD6F at address 001AF1FA has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_AAF9CD6F()
        {
            AssertCode("@@@", "AAF9CD6F");
        }
        // Reko: a decoder for the instruction 8EF9E642 at address 0012E65E has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F98E42E6)
        [Test]
        public void ThumbDis_8EF9E642()
        {
            AssertCode("@@@", "8EF9E642");
        }
        // Reko: a decoder for the instruction EDFC1159 at address 001AF32C has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EDFC1159()
        {
            AssertCode("@@@", "EDFC1159");
        }
        // Reko: a decoder for the instruction 51FC64BC at address 0012E79A has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 98FD4A39 at address 001AF426 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_98FD4A39()
        {
            AssertCode("@@@", "98FD4A39");
        }
        // Reko: a decoder for the instruction C7F90E0B at address 001AF5F4 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C70B0E)
        [Test]
        public void ThumbDis_C7F90E0B()
        {
            AssertCode("@@@", "C7F90E0B");
        }
        // Reko: a decoder for the instruction ACF9B0E5 at address 001AF6A8 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9ACE5B0)
        [Test]
        public void ThumbDis_ACF9B0E5()
        {
            AssertCode("@@@", "ACF9B0E5");
        }
        // Reko: a decoder for the instruction DAF33FAE at address 001AF6CC has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_DAF33FAE()
        {
            AssertCode("@@@", "DAF33FAE");
        }
        // Reko: a decoder for the instruction A9FCA859 at address 0010FD48 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A9FCA859()
        {
            AssertCode("@@@", "A9FCA859");
        }
        // Reko: a decoder for the instruction 16FE56CD at address 0014EFC4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_16FE56CD()
        {
            AssertCode("@@@", "16FE56CD");
        }
        // Reko: a decoder for the instruction 85FF5ABE at address 001CEEA8 has not been implemented. (Unimplemented '*immediate - T4' when decoding FF85BE5A)
        [Test]
        public void ThumbDis_85FF5ABE()
        {
            AssertCode("@@@", "85FF5ABE");
        }
        // Reko: a decoder for the instruction FAEF96AD at address 0012F77A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_FAEF96AD()
        {
            AssertCode("@@@", "FAEF96AD");
        }
        // Reko: a decoder for the instruction A6FFC538 at address 00110BF6 has not been implemented. (Unimplemented '*scalar' when decoding FFA638C5)
        [Test]
        public void ThumbDis_A6FFC538()
        {
            AssertCode("@@@", "A6FFC538");
        }
        // Reko: a decoder for the instruction E2FC7B89 at address 0014F292 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E2FC7B89()
        {
            AssertCode("@@@", "E2FC7B89");
        }
        // Reko: a decoder for the instruction AFFC2F19 at address 001F20E8 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_AFFC2F19()
        {
            AssertCode("@@@", "AFFC2F19");
        }
        // Reko: a decoder for the instruction CBEC027A at address 001B207C has not been implemented. (Unimplemented '*' when decoding ECCB7A02)
        [Test]
        public void ThumbDis_CBEC027A()
        {
            AssertCode("@@@", "CBEC027A");
        }
        // Reko: a decoder for the instruction FEF72E8D at address 001B2CAC has not been implemented. (Unimplemented '*' when decoding F7FE8D2E)

        // Reko: a decoder for the instruction E2FE5A1C at address 001F2AAC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E2FE5A1C()
        {
            AssertCode("@@@", "E2FE5A1C");
        }
        // Reko: a decoder for the instruction 87EFE7D5 at address 001B2D08 has not been implemented. (Unimplemented '*scalar' when decoding EF87D5E7)
        [Test]
        public void ThumbDis_87EFE7D5()
        {
            AssertCode("@@@", "87EFE7D5");
        }
        // Reko: a decoder for the instruction 2CFC01B8 at address 001D0E94 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2CFC01B8()
        {
            AssertCode("@@@", "2CFC01B8");
        }
        // Reko: a decoder for the instruction 18FFB424 at address 001926E0 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_18FFB424()
        {
            AssertCode("@@@", "18FFB424");
        }
        // Reko: a decoder for the instruction F0FF6BB2 at address 001F2F6E has not been implemented. (Unimplemented '*' when decoding FFF0B26B)
        [Test]
        public void ThumbDis_F0FF6BB2()
        {
            AssertCode("@@@", "F0FF6BB2");
        }
        // Reko: a decoder for the instruction EAEC286B at address 001926F6 has not been implemented. (Unimplemented '*' when decoding ECEA6B28)
        [Test]
        public void ThumbDis_EAEC286B()
        {
            AssertCode("@@@", "EAEC286B");
        }
        // Reko: a decoder for the instruction 40EFE03B at address 0019270C has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_40EFE03B()
        {
            AssertCode("@@@", "40EFE03B");
        }
        // Reko: a decoder for the instruction 73FDCA28 at address 00192F1C has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_73FDCA28()
        {
            AssertCode("@@@", "73FDCA28");
        }
        // Reko: a decoder for the instruction E7FF6BC4 at address 001D17A4 has not been implemented. (Unimplemented '*scalar' when decoding FFE7C46B)
        [Test]
        public void ThumbDis_E7FF6BC4()
        {
            AssertCode("@@@", "E7FF6BC4");
        }
        // Reko: a decoder for the instruction 9FF90AFC at address 001D184C has not been implemented. (Unimplemented '* literal' when decoding F99FFC0A)
        [Test]
        public void ThumbDis_9FF90AFC()
        {
            AssertCode("@@@", "9FF90AFC");
        }
        // Reko: a decoder for the instruction 0AFCC6FC at address 001D184E has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0AFCC6FC()
        {
            AssertCode("@@@", "0AFCC6FC");
        }
        // Reko: a decoder for the instruction C6FCA5B9 at address 001D1850 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C6FCA5B9()
        {
            AssertCode("@@@", "C6FCA5B9");
        }
        // Reko: a decoder for the instruction ABFFF0FD at address 00130C80 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_ABFFF0FD()
        {
            AssertCode("@@@", "ABFFF0FD");
        }
        // Reko: a decoder for the instruction 92FFD588 at address 001F3586 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_92FFD588()
        {
            AssertCode("@@@", "92FFD588");
        }
        // Reko: a decoder for the instruction BFFFC413 at address 0016FF7A has not been implemented. (Unimplemented '*' when decoding FFBF13C4)
        [Test]
        public void ThumbDis_BFFFC413()
        {
            AssertCode("@@@", "BFFFC413");
        }
        // Reko: a decoder for the instruction 36EF0D1E at address 00112BE8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)

        // Reko: a decoder for the instruction 8DEF55A8 at address 00131958 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_8DEF55A8()
        {
            AssertCode("@@@", "8DEF55A8");
        }
        // Reko: a decoder for the instruction A8FE31FD at address 00193D60 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A8FE31FD()
        {
            AssertCode("@@@", "A8FE31FD");
        }
        // Reko: a decoder for the instruction A3F9F2B5 at address 00151860 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9A3B5F2)
        [Test]
        public void ThumbDis_A3F9F2B5()
        {
            AssertCode("@@@", "A3F9F2B5");
        }
        // Reko: a decoder for the instruction C5FFEC29 at address 00112C0C has not been implemented. (Unimplemented '*scalar' when decoding FFC529EC)
        [Test]
        public void ThumbDis_C5FFEC29()
        {
            AssertCode("@@@", "C5FFEC29");
        }
        // Reko: a decoder for the instruction A0FCD9FC at address 001F3BD4 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A0FCD9FC()
        {
            AssertCode("@@@", "A0FCD9FC");
        }
        // Reko: a decoder for the instruction 75EFCE3B at address 0017114E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_75EFCE3B()
        {
            AssertCode("@@@", "75EFCE3B");
        }
        // Reko: a decoder for the instruction B9EE651A at address 001711A8 has not been implemented. (1001 - _HSD)
        [Test]
        public void ThumbDis_B9EE651A()
        {
            AssertCode("@@@", "B9EE651A");
        }
        // Reko: a decoder for the instruction E3F36385 at address 001B4DF6 has not been implemented. (Unimplemented '*banked register' when decoding F3E38563)
        [Test]
        public void ThumbDis_E3F36385()
        {
            AssertCode("@@@", "E3F36385");
        }
        // Reko: a decoder for the instruction 7AFF303C at address 00171310 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_7AFF303C()
        {
            AssertCode("@@@", "7AFF303C");
        }
        // Reko: a decoder for the instruction EBFC6BFD at address 00171318 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EBFC6BFD()
        {
            AssertCode("@@@", "EBFC6BFD");
        }
        // Reko: a decoder for the instruction 33EFAC8C at address 001D25C8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_33EFAC8C()
        {
            AssertCode("@@@", "33EFAC8C");
        }
        // Reko: a decoder for the instruction 4FFF88A1 at address 001B4EA0 has not been implemented. (Unimplemented '*' when decoding FF4FA188)
        [Test]
        public void ThumbDis_4FFF88A1()
        {
            AssertCode("@@@", "4FFF88A1");
        }
        // Reko: a decoder for the instruction ADECEEAB at address 00151C92 has not been implemented. (Unimplemented '*' when decoding ECADABEE)
        [Test]
        public void ThumbDis_ADECEEAB()
        {
            AssertCode("@@@", "ADECEEAB");
        }
        // Reko: a decoder for the instruction 20FEC74C at address 00194AE2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_20FEC74C()
        {
            AssertCode("@@@", "20FEC74C");
        }
        // Reko: a decoder for the instruction A7EF94CB at address 001D26A4 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_A7EF94CB()
        {
            AssertCode("@@@", "A7EF94CB");
        }
        // Reko: a decoder for the instruction 31EFE1FE at address 00194C2A has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
 
        // Reko: a decoder for the instruction E1FE281C at address 00194C2C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E1FE281C()
        {
            AssertCode("@@@", "E1FE281C");
        }
        // Reko: a decoder for the instruction A0F3DB8C at address 00194C46 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A0F3DB8C()
        {
            AssertCode("@@@", "A0F3DB8C");
        }
        // Reko: a decoder for the instruction 0AFC28BC at address 00194C4C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0AFC28BC()
        {
            AssertCode("@@@", "0AFC28BC");
        }
        // Reko: a decoder for the instruction 8EFCFCED at address 00194C64 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8EFCFCED()
        {
            AssertCode("@@@", "8EFCFCED");
        }
        // Reko: a decoder for the instruction 6BFD99DC at address 0017131A has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6BFD99DC()
        {
            AssertCode("@@@", "6BFD99DC");
        }
        // Reko: a decoder for the instruction 6FFF437F at address 001520B0 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_6FFF437F()
        {
            AssertCode("@@@", "6FFF437F");
        }
        // Reko: a decoder for the instruction 02FFAFCC at address 00113D86 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_02FFAFCC()
        {
            AssertCode("@@@", "02FFAFCC");
        }
        // Reko: a decoder for the instruction E4F90AAF at address 00113E98 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E4F90AAF()
        {
            AssertCode("@@@", "E4F90AAF");
        }
        // Reko: a decoder for the instruction FDEE43BA at address 001950A8 has not been implemented. (Unimplemented '*' when decoding EEFDBA43)
        [Test]
        public void ThumbDis_FDEE43BA()
        {
            AssertCode("@@@", "FDEE43BA");
        }
        // Reko: a decoder for the instruction DCEF3C9D at address 00114222 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_DCEF3C9D()
        {
            AssertCode("@@@", "DCEF3C9D");
        }
        // Reko: a decoder for the instruction EAEF657E at address 001D2DAE has not been implemented. (Unimplemented '*' when decoding EFEA7E65)
        [Test]
        public void ThumbDis_EAEF657E()
        {
            AssertCode("@@@", "EAEF657E");
        }
        // Reko: a decoder for the instruction 15F9BE3D at address 001B54D6 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_15F9BE3D()
        {
            AssertCode("@@@", "15F9BE3D");
        }
        // Reko: a decoder for the instruction 91EF6F78 at address 0013326C has not been implemented. (Unimplemented '*scalar' when decoding EF91786F)
        [Test]
        public void ThumbDis_91EF6F78()
        {
            AssertCode("@@@", "91EF6F78");
        }
        // Reko: a decoder for the instruction C7F93D9B at address 00171F94 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C79B3D)
        [Test]
        public void ThumbDis_C7F93D9B()
        {
            AssertCode("@@@", "C7F93D9B");
        }
        // Reko: a decoder for the instruction 0AFC5F0C at address 00152C8A has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0AFC5F0C()
        {
            AssertCode("@@@", "0AFC5F0C");
        }
        // Reko: a decoder for the instruction EAFC1D49 at address 001146AA has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EAFC1D49()
        {
            AssertCode("@@@", "EAFC1D49");
        }
        // Reko: a decoder for the instruction C8FDE3BD at address 00172054 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C8FDE3BD()
        {
            AssertCode("@@@", "C8FDE3BD");
        }
        // Reko: a decoder for the instruction 11F9E51F at address 001D3050 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_11F9E51F()
        {
            AssertCode("@@@", "11F9E51F");
        }
        // Reko: a decoder for the instruction 5AF9838E at address 0013338E has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_5AF9838E()
        {
            AssertCode("@@@", "5AF9838E");
        }
        // Reko: a decoder for the instruction 3BFF39C2 at address 00114E3C has not been implemented. (Unimplemented '*' when decoding FF3BC239)
        [Test]
        public void ThumbDis_3BFF39C2()
        {
            AssertCode("@@@", "3BFF39C2");
        }
        // Reko: a decoder for the instruction 32EF1C99 at address 00153830 has not been implemented. (*vmul (integer and polynomial)
 
        // Reko: a decoder for the instruction 6DEF1D2D at address 001965E4 has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_6DEF1D2D()
        {
            AssertCode("@@@", "6DEF1D2D");
        }
        // Reko: a decoder for the instruction 95EF67A5 at address 001F57B4 has not been implemented. (Unimplemented '*scalar' when decoding EF95A567)
        [Test]
        public void ThumbDis_95EF67A5()
        {
            AssertCode("@@@", "95EF67A5");
        }
        // Reko: a decoder for the instruction DCFC9B6D at address 001D405E has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DCFC9B6D()
        {
            AssertCode("@@@", "DCFC9B6D");
        }
        // Reko: a decoder for the instruction 19FEA8FD at address 00134386 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_19FEA8FD()
        {
            AssertCode("@@@", "19FEA8FD");
        }
        // Reko: a decoder for the instruction C5FC4058 at address 00172F54 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C5FC4058()
        {
            AssertCode("@@@", "C5FC4058");
        }
        // Reko: a decoder for the instruction 51FD485D at address 001B67BC has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction C9FFE9E9 at address 001B6A98 has not been implemented. (Unimplemented '*scalar' when decoding FFC9E9E9)
        [Test]
        public void ThumbDis_C9FFE9E9()
        {
            AssertCode("@@@", "C9FFE9E9");
        }
        // Reko: a decoder for the instruction 0AFE9EAD at address 00134BA4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_0AFE9EAD()
        {
            AssertCode("@@@", "0AFE9EAD");
        }
        // Reko: a decoder for the instruction 21FF1214 at address 00173426 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_21FF1214()
        {
            AssertCode("@@@", "21FF1214");
        }
        // Reko: a decoder for the instruction 48EFFAAE at address 001D4488 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_48EFFAAE()
        {
            AssertCode("@@@", "48EFFAAE");
        }
        // Reko: a decoder for the instruction 9EFC46E8 at address 00197732 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_9EFC46E8()
        {
            AssertCode("@@@", "9EFC46E8");
        }
        // Reko: a decoder for the instruction 3CFF7E6C at address 001981D2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_3CFF7E6C()
        {
            AssertCode("@@@", "3CFF7E6C");
        }
        // Reko: a decoder for the instruction 7EF965BD at address 001D501E has not been implemented. (LoadStoreSignedImmediatePreIndexed)

        // Reko: a decoder for the instruction EDEC6E3A at address 00117372 has not been implemented. (Unimplemented '*' when decoding ECED3A6E)
        [Test]
        public void ThumbDis_EDEC6E3A()
        {
            AssertCode("@@@", "EDEC6E3A");
        }
        // Reko: a decoder for the instruction D6FEEE1C at address 001984C2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D6FEEE1C()
        {
            AssertCode("@@@", "D6FEEE1C");
        }
        // Reko: a decoder for the instruction A7FE0EAC at address 00117438 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A7FE0EAC()
        {
            AssertCode("@@@", "A7FE0EAC");
        }
        // Reko: a decoder for the instruction 2CFF9F89 at address 001F6B58 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 48EFB1BF at address 001745BA has not been implemented. (Unimplemented '*' when decoding EF48BFB1)
        [Test]
        public void ThumbDis_48EFB1BF()
        {
            AssertCode("@@@", "48EFB1BF");
        }
        // Reko: a decoder for the instruction 60FF38D2 at address 00135C94 has not been implemented. (Unimplemented '*' when decoding FF60D238)
        [Test]
        public void ThumbDis_60FF38D2()
        {
            AssertCode("@@@", "60FF38D2");
        }
        // Reko: a decoder for the instruction 8DF90F5F at address 00135EDC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8DF90F5F()
        {
            AssertCode("@@@", "8DF90F5F");
        }
        // Reko: a decoder for the instruction 25EFCDEB at address 0017460A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_25EFCDEB()
        {
            AssertCode("@@@", "25EFCDEB");
        }
        // Reko: a decoder for the instruction 6FFC9D29 at address 001B814A has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6FFC9D29()
        {
            AssertCode("@@@", "6FFC9D29");
        }
        // Reko: a decoder for the instruction 8CFC2DBD at address 00136242 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8CFC2DBD()
        {
            AssertCode("@@@", "8CFC2DBD");
        }
        // Reko: a decoder for the instruction 50EF18EF at address 00174A32 has not been implemented. (Unimplemented '*' when decoding EF50EF18)
        [Test]
        public void ThumbDis_50EF18EF()
        {
            AssertCode("@@@", "50EF18EF");
        }
        // Reko: a decoder for the instruction BCFFA444 at address 00118EAC has not been implemented. (Unimplemented '*' when decoding FFBC44A4)
        [Test]
        public void ThumbDis_BCFFA444()
        {
            AssertCode("@@@", "BCFFA444");
        }
        // Reko: a decoder for the instruction 40FF300E at address 001B8A72 has not been implemented. (Unimplemented '*' when decoding FF400E30)
        [Test]
        public void ThumbDis_40FF300E()
        {
            AssertCode("@@@", "40FF300E");
        }
        // Reko: a decoder for the instruction 5FEF553B at address 00199DAA has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_5FEF553B()
        {
            AssertCode("@@@", "5FEF553B");
        }
        // Reko: a decoder for the instruction ECF90AE7 at address 00136A0A has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F9ECE70A)
        [Test]
        public void ThumbDis_ECF90AE7()
        {
            AssertCode("@@@", "ECF90AE7");
        }
        // Reko: a decoder for the instruction B5FD5CC9 at address 00136A1C has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B5FD5CC9()
        {
            AssertCode("@@@", "B5FD5CC9");
        }
        // Reko: a decoder for the instruction 5BFEA1DC at address 00199FB2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_5BFEA1DC()
        {
            AssertCode("@@@", "5BFEA1DC");
        }
        // Reko: a decoder for the instruction 85FAA621 at address 00157120 has not been implemented. (Unimplemented '*' when decoding FA8521A6)
   
        // Reko: a decoder for the instruction 0FFC035C at address 00136A58 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0FFC035C()
        {
            AssertCode("@@@", "0FFC035C");
        }
        // Reko: a decoder for the instruction E0FFE14F at address 001F81C0 has not been implemented. (Unimplemented '*' when decoding FFE04FE1)
        [Test]
        public void ThumbDis_E0FFE14F()
        {
            AssertCode("@@@", "E0FFE14F");
        }
        // Reko: a decoder for the instruction 80EF6523 at address 001572D2 has not been implemented. (Unimplemented '*' when decoding EF802365)
        [Test]
        public void ThumbDis_80EF6523()
        {
            AssertCode("@@@", "80EF6523");
        }
        // Reko: a decoder for the instruction 88FF6D0A at address 001B9434 has not been implemented. (Unimplemented '*' when decoding FF880A6D)
        [Test]
        public void ThumbDis_88FF6D0A()
        {
            AssertCode("@@@", "88FF6D0A");
        }
        // Reko: a decoder for the instruction 7BF9CE5D at address 00119844 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
 
        // Reko: a decoder for the instruction BBFE290D at address 001B98BE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_BBFE290D()
        {
            AssertCode("@@@", "BBFE290D");
        }
        // Reko: a decoder for the instruction C6FF66D4 at address 00176108 has not been implemented. (Unimplemented '*scalar' when decoding FFC6D466)

        // Reko: a decoder for the instruction 44FFE4A3 at address 00136E6C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_44FFE4A3()
        {
            AssertCode("@@@", "44FFE4A3");
        }
        // Reko: a decoder for the instruction A0FD110C at address 0011989C has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A0FD110C()
        {
            AssertCode("@@@", "A0FD110C");
        }
        // Reko: a decoder for the instruction 82FD9B5C at address 001D6E96 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_82FD9B5C()
        {
            AssertCode("@@@", "82FD9B5C");
        }
        // Reko: a decoder for the instruction 61EF3B1D at address 00157A8C has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_61EF3B1D()
        {
            AssertCode("@@@", "61EF3B1D");
        }
        // Reko: a decoder for the instruction A6FDBFAC at address 00176156 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_A6FDBFAC()
        {
            AssertCode("@@@", "A6FDBFAC");
        }
        // Reko: a decoder for the instruction E8EEFE18 at address 00136E92 has not been implemented. (Unimplemented '*' when decoding EEE818FE)
        [Test]
        public void ThumbDis_E8EEFE18()
        {
            AssertCode("@@@", "E8EEFE18");
        }
        // Reko: a decoder for the instruction 26EF2633 at address 0013705C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_26EF2633()
        {
            AssertCode("@@@", "26EF2633");
        }
        // Reko: a decoder for the instruction AFEC985A at address 001B9E1C has not been implemented. (Unimplemented '*' when decoding ECAF5A98)
        [Test]
        public void ThumbDis_AFEC985A()
        {
            AssertCode("@@@", "AFEC985A");
        }
        // Reko: a decoder for the instruction 3EEFD4FB at address 001B9E6A has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_3EEFD4FB()
        {
            AssertCode("@@@", "3EEFD4FB");
        }
        // Reko: a decoder for the instruction 47FFBAFE at address 00119E02 has not been implemented. (Unimplemented '*' when decoding FF47FEBA)
        [Test]
        public void ThumbDis_47FFBAFE()
        {
            AssertCode("@@@", "47FFBAFE");
        }
        // Reko: a decoder for the instruction 64FDA28D at address 001763CA has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_64FDA28D()
        {
            AssertCode("@@@", "64FDA28D");
        }
        // Reko: a decoder for the instruction 45FEA37C at address 00176420 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_45FEA37C()
        {
            AssertCode("@@@", "45FEA37C");
        }
        // Reko: a decoder for the instruction 42FFB0E9 at address 0017669C has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_42FFB0E9()
        {
            AssertCode("@@@", "42FFB0E9");
        }
        // Reko: a decoder for the instruction 1FEF008E at address 001D7138 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)

        // Reko: a decoder for the instruction DAEFC80D at address 0019AE66 has not been implemented. (Unimplemented '*' when decoding EFDA0DC8)
        [Test]
        public void ThumbDis_DAEFC80D()
        {
            AssertCode("@@@", "DAEFC80D");
        }
        // Reko: a decoder for the instruction E5F77787 at address 001D71D6 has not been implemented. (Unimplemented '*' when decoding F7E58777)
        [Test]
        public void ThumbDis_E5F77787()
        {
            AssertCode("@@@", "E5F77787");
        }
        // Reko: a decoder for the instruction 8DFF43AD at address 001B9FE0 has not been implemented. (Unimplemented '*' when decoding FF8DAD43)
        [Test]
        public void ThumbDis_8DFF43AD()
        {
            AssertCode("@@@", "8DFF43AD");
        }
        // Reko: a decoder for the instruction 73F9985E at address 001766FC has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_73F9985E()
        {
            AssertCode("@@@", "73F9985E");
        }
        // Reko: a decoder for the instruction 94FC1E0C at address 001BA17A has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_94FC1E0C()
        {
            AssertCode("@@@", "94FC1E0C");
        }
        // Reko: a decoder for the instruction 05FD3F7D at address 0019AE82 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_05FD3F7D()
        {
            AssertCode("@@@", "05FD3F7D");
        }
        // Reko: a decoder for the instruction 11FCAAA8 at address 001BA1E0 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 27EF3203 at address 001F9002 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_27EF3203()
        {
            AssertCode("@@@", "27EF3203");
        }
        // Reko: a decoder for the instruction D3FCB4A8 at address 0011A552 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D3FCB4A8()
        {
            AssertCode("@@@", "D3FCB4A8");
        }
        // Reko: a decoder for the instruction 04FDF198 at address 001BA468 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_04FDF198()
        {
            AssertCode("@@@", "04FDF198");
        }
        // Reko: a decoder for the instruction DDF35689 at address 001D7580 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_DDF35689()
        {
            AssertCode("@@@", "DDF35689");
        }
        // Reko: a decoder for the instruction 66FDEDD8 at address 00176CC6 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_66FDEDD8()
        {
            AssertCode("@@@", "66FDEDD8");
        }
        // Reko: a decoder for the instruction ECFFE0CF at address 001BA84C has not been implemented. (Unimplemented '*' when decoding FFECCFE0)
        [Test]
        public void ThumbDis_ECFFE0CF()
        {
            AssertCode("@@@", "ECFFE0CF");
        }
        // Reko: a decoder for the instruction 8DEFE56D at address 00137CA0 has not been implemented. (Unimplemented '*' when decoding EF8D6DE5)
        [Test]
        public void ThumbDis_8DEFE56D()
        {
            AssertCode("@@@", "8DEFE56D");
        }
        // Reko: a decoder for the instruction C5FF144E at address 001BA8C0 has not been implemented. (Unimplemented '*immediate - T4' when decoding FFC54E14)
        [Test]
        public void ThumbDis_C5FF144E()
        {
            AssertCode("@@@", "C5FF144E");
        }
        // Reko: a decoder for the instruction 5CFC9C7C at address 001D7A8A has not been implemented. (op1:op2=0b0001)
 
        // Reko: a decoder for the instruction 32EFD549 at address 0019B3FE has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction A2EFC5BB at address 001D7CCE has not been implemented. (Unimplemented '*' when decoding EFA2BBC5)
        [Test]
        public void ThumbDis_A2EFC5BB()
        {
            AssertCode("@@@", "A2EFC5BB");
        }
        // Reko: a decoder for the instruction 44EEB49B at address 0011AF0C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_44EEB49B()
        {
            AssertCode("@@@", "44EEB49B");
        }
        // Reko: a decoder for the instruction F4EF9EDD at address 0011B33E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_F4EF9EDD()
        {
            AssertCode("@@@", "F4EF9EDD");
        }
        // Reko: a decoder for the instruction 5DFE66DC at address 0019B40C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_5DFE66DC()
        {
            AssertCode("@@@", "5DFE66DC");
        }
        // Reko: a decoder for the instruction 21FF0423 at address 001F99C4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_21FF0423()
        {
            AssertCode("@@@", "21FF0423");
        }
        // Reko: a decoder for the instruction 74EED6D9 at address 00138472 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_74EED6D9()
        {
            AssertCode("@@@", "74EED6D9");
        }
        // Reko: a decoder for the instruction 32FFC6DD at address 001D83B6 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_32FFC6DD()
        {
            AssertCode("@@@", "32FFC6DD");
        }
        // Reko: a decoder for the instruction 84FF6922 at address 0019B6D4 has not been implemented. (Unimplemented '*scalar' when decoding FF842269)
        [Test]
        public void ThumbDis_84FF6922()
        {
            AssertCode("@@@", "84FF6922");
        }
        // Reko: a decoder for the instruction 6CFD54BD at address 001BB142 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6CFD54BD()
        {
            AssertCode("@@@", "6CFD54BD");
        }
        // Reko: a decoder for the instruction 0EEFE694 at address 001F9C8A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_0EEFE694()
        {
            AssertCode("@@@", "0EEFE694");
        }
        // Reko: a decoder for the instruction EDF3A4A6 at address 00177ADA has not been implemented. (Unimplemented '*banked register' when decoding F3EDA6A4)
        [Test]
        public void ThumbDis_EDF3A4A6()
        {
            AssertCode("@@@", "EDF3A4A6");
        }
        // Reko: a decoder for the instruction C8FA8699 at address 00158C0E has not been implemented. (crc32-crc32b)
        [Test]
        public void ThumbDis_C8FA8699()
        {
            AssertCode("@@@", "C8FA8699");
        }
        // Reko: a decoder for the instruction 57FC1CDD at address 001D8550 has not been implemented. (op1:op2=0b0001)
  
        // Reko: a decoder for the instruction 85FE1D38 at address 001BB296 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_85FE1D38()
        {
            AssertCode("@@@", "85FE1D38");
        }
        // Reko: a decoder for the instruction AEFE0F2D at address 0019B8C8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_AEFE0F2D()
        {
            AssertCode("@@@", "AEFE0F2D");
        }
        // Reko: a decoder for the instruction 2CFC710C at address 001D8C96 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2CFC710C()
        {
            AssertCode("@@@", "2CFC710C");
        }
        // Reko: a decoder for the instruction 78F991EE at address 001BBAB6 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_78F991EE()
        {
            AssertCode("@@@", "78F991EE");
        }
        // Reko: a decoder for the instruction 38FE393C at address 00178542 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_38FE393C()
        {
            AssertCode("@@@", "38FE393C");
        }
        // Reko: a decoder for the instruction 2AEF1AF2 at address 0011C2C4 has not been implemented. (Unimplemented '*' when decoding EF2AF21A)
        [Test]
        public void ThumbDis_2AEF1AF2()
        {
            AssertCode("@@@", "2AEF1AF2");
        }
        // Reko: a decoder for the instruction 2EFCD1AD at address 0011C2EE has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2EFCD1AD()
        {
            AssertCode("@@@", "2EFCD1AD");
        }
        // Reko: a decoder for the instruction 00FC5888 at address 001788F2 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_00FC5888()
        {
            AssertCode("@@@", "00FC5888");
        }
        // Reko: a decoder for the instruction 08FE0F7D at address 00178A7E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_08FE0F7D()
        {
            AssertCode("@@@", "08FE0F7D");
        }
        // Reko: a decoder for the instruction 50FF9681 at address 001FAB12 has not been implemented. (Unimplemented '*register' when decoding FF508196)
        [Test]
        public void ThumbDis_50FF9681()
        {
            AssertCode("@@@", "50FF9681");
        }
        // Reko: a decoder for the instruction 49FFDC4B at address 0011C56C has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_49FFDC4B()
        {
            AssertCode("@@@", "49FFDC4B");
        }
        // Reko: a decoder for the instruction 23FF1F02 at address 001FACA0 has not been implemented. (Unimplemented '*' when decoding FF23021F)
        [Test]
        public void ThumbDis_23FF1F02()
        {
            AssertCode("@@@", "23FF1F02");
        }
        // Reko: a decoder for the instruction E4EFDD8B at address 00139612 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_E4EFDD8B()
        {
            AssertCode("@@@", "E4EFDD8B");
        }
        // Reko: a decoder for the instruction B0EF9F34 at address 001FB0AA has not been implemented. (U=0)
        [Test]
        public void ThumbDis_B0EF9F34()
        {
            AssertCode("@@@", "B0EF9F34");
        }
        // Reko: a decoder for the instruction C5FF9ECC at address 0013998C has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_C5FF9ECC()
        {
            AssertCode("@@@", "C5FF9ECC");
        }
        // Reko: a decoder for the instruction D2EF9C6D at address 00159BD8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_D2EF9C6D()
        {
            AssertCode("@@@", "D2EF9C6D");
        }
        // Reko: a decoder for the instruction E5F98A10 at address 001BC29A has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9E5108A)
        [Test]
        public void ThumbDis_E5F98A10()
        {
            AssertCode("@@@", "E5F98A10");
        }
        // Reko: a decoder for the instruction 3AFEF2CD at address 001FB4E0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3AFEF2CD()
        {
            AssertCode("@@@", "3AFEF2CD");
        }
        // Reko: a decoder for the instruction A3F3B38A at address 00139A08 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A3F3B38A()
        {
            AssertCode("@@@", "A3F3B38A");
        }
        // Reko: a decoder for the instruction EBEE5AAA at address 001D9560 has not been implemented. (Unimplemented '*' when decoding EEEBAA5A)
        [Test]
        public void ThumbDis_EBEE5AAA()
        {
            AssertCode("@@@", "EBEE5AAA");
        }
        // Reko: a decoder for the instruction C2FF541D at address 001D9582 has not been implemented. (Unimplemented '*immediate - T4' when decoding FFC21D54)
        [Test]
        public void ThumbDis_C2FF541D()
        {
            AssertCode("@@@", "C2FF541D");
        }
        // Reko: a decoder for the instruction D3FF558C at address 0011C7CE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_D3FF558C()
        {
            AssertCode("@@@", "D3FF558C");
        }
        // Reko: a decoder for the instruction 00FDD4F8 at address 00139D3C has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_00FDD4F8()
        {
            AssertCode("@@@", "00FDD4F8");
        }
        // Reko: a decoder for the instruction D0FF613E at address 00179772 has not been implemented. (Unimplemented '*' when decoding FFD03E61)
        [Test]
        public void ThumbDis_D0FF613E()
        {
            AssertCode("@@@", "D0FF613E");
        }
        // Reko: a decoder for the instruction 87EC3BEA at address 001D96C4 has not been implemented. (Unimplemented '*' when decoding EC87EA3B)
        [Test]
        public void ThumbDis_87EC3BEA()
        {
            AssertCode("@@@", "87EC3BEA");
        }
        // Reko: a decoder for the instruction C7F99492 at address 0015A062 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9C79294)
        [Test]
        public void ThumbDis_C7F99492()
        {
            AssertCode("@@@", "C7F99492");
        }
        // Reko: a decoder for the instruction CDF90DFF at address 0015A090 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CDF90DFF()
        {
            AssertCode("@@@", "CDF90DFF");
        }
        // Reko: a decoder for the instruction 59FF6B94 at address 001FBC6C has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_59FF6B94()
        {
            AssertCode("@@@", "59FF6B94");
        }
        // Reko: a decoder for the instruction CFF93D7E at address 0011CE04 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CFF93D7E()
        {
            AssertCode("@@@", "CFF93D7E");
        }
        // Reko: a decoder for the instruction E1FE1DC8 at address 00179D32 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E1FE1DC8()
        {
            AssertCode("@@@", "E1FE1DC8");
        }
        // Reko: a decoder for the instruction 14F9322E at address 00179D3E has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_14F9322E()
        {
            AssertCode("@@@", "14F9322E");
        }
        // Reko: a decoder for the instruction F8EE36EB at address 001BCD60 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 43EF2DA4 at address 0011CEAE has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_43EF2DA4()
        {
            AssertCode("@@@", "43EF2DA4");
        }
        // Reko: a decoder for the instruction 5FFF1253 at address 00139DC6 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_5FFF1253()
        {
            AssertCode("@@@", "5FFF1253");
        }
        // Reko: a decoder for the instruction A6ECFF6A at address 001FBEF0 has not been implemented. (Unimplemented '*' when decoding ECA66AFF)
        [Test]
        public void ThumbDis_A6ECFF6A()
        {
            AssertCode("@@@", "A6ECFF6A");
        }
        // Reko: a decoder for the instruction ACEFA91D at address 00179F5E has not been implemented. (Unimplemented '*integer' when decoding EFAC1DA9)

        // Reko: a decoder for the instruction 07FDB939 at address 0017A3D8 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_07FDB939()
        {
            AssertCode("@@@", "07FDB939");
        }
        // Reko: a decoder for the instruction 82F9F463 at address 001BD584 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F98263F4)
        [Test]
        public void ThumbDis_82F9F463()
        {
            AssertCode("@@@", "82F9F463");
        }
        // Reko: a decoder for the instruction D7FA9BE0 at address 0013A22E has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D7FA9BE0()
        {
            AssertCode("@@@", "D7FA9BE0");
        }
        // Reko: a decoder for the instruction 6BFDC039 at address 0019D706 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6BFDC039()
        {
            AssertCode("@@@", "6BFDC039");
        }
        // Reko: a decoder for the instruction ABFA8B19 at address 001BD6EA has not been implemented. (Unimplemented '*' when decoding FAAB198B)
        [Test]
        public void ThumbDis_ABFA8B19()
        {
            AssertCode("@@@", "ABFA8B19");
        }
        // Reko: a decoder for the instruction F4EC0DBB at address 001D9F6E has not been implemented. (Unimplemented '*' when decoding ECF4BB0D)
        [Test]
        public void ThumbDis_F4EC0DBB()
        {
            AssertCode("@@@", "F4EC0DBB");
        }
        // Reko: a decoder for the instruction 83F950D4 at address 001FC29E has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F983D450)
        [Test]
        public void ThumbDis_83F950D4()
        {
            AssertCode("@@@", "83F950D4");
        }
        // Reko: a decoder for the instruction A4EFDFF4 at address 0017A4EC has not been implemented. (U=0)
        [Test]
        public void ThumbDis_A4EFDFF4()
        {
            AssertCode("@@@", "A4EFDFF4");
        }
        // Reko: a decoder for the instruction 43EF438E at address 001DA372 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_43EF438E()
        {
            AssertCode("@@@", "43EF438E");
        }
        // Reko: a decoder for the instruction A2EC8D6A at address 0019D9A0 has not been implemented. (Unimplemented '*' when decoding ECA26A8D)
        [Test]
        public void ThumbDis_A2EC8D6A()
        {
            AssertCode("@@@", "A2EC8D6A");
        }
        // Reko: a decoder for the instruction 35FF096D at address 0015A492 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_35FF096D()
        {
            AssertCode("@@@", "35FF096D");
        }
        // Reko: a decoder for the instruction DFEF08BD at address 001BD916 has not been implemented. (Unimplemented '*integer' when decoding EFDFBD08)
        [Test]
        public void ThumbDis_DFEF08BD()
        {
            AssertCode("@@@", "DFEF08BD");
        }
        // Reko: a decoder for the instruction 46FEC51C at address 0019D9B4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_46FEC51C()
        {
            AssertCode("@@@", "46FEC51C");
        }
        // Reko: a decoder for the instruction 43FF0011 at address 001BDE90 has not been implemented. (Unimplemented '*' when decoding FF431100)
        [Test]
        public void ThumbDis_43FF0011()
        {
            AssertCode("@@@", "43FF0011");
        }
        // Reko: a decoder for the instruction 2FFC0008 at address 0017B156 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2FFC0008()
        {
            AssertCode("@@@", "2FFC0008");
        }
        // Reko: a decoder for the instruction 1DEF3C49 at address 001DADFE has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_1DEF3C49()
        {
            AssertCode("@@@", "1DEF3C49");
        }
        // Reko: a decoder for the instruction 06FC8119 at address 001DAE18 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_06FC8119()
        {
            AssertCode("@@@", "06FC8119");
        }
        // Reko: a decoder for the instruction 11FD6B9D at address 0015A9B0 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 4BFDC399 at address 0019E12A has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4BFDC399()
        {
            AssertCode("@@@", "4BFDC399");
        }
        // Reko: a decoder for the instruction 58FE9BA9 at address 001FD820 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_58FE9BA9()
        {
            AssertCode("@@@", "58FE9BA9");
        }
        // Reko: a decoder for the instruction B3EF9FDB at address 0013BDC6 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_B3EF9FDB()
        {
            AssertCode("@@@", "B3EF9FDB");
        }
        // Reko: a decoder for the instruction 88F935FF at address 0013BDD2 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_88F935FF()
        {
            AssertCode("@@@", "88F935FF");
        }
        // Reko: a decoder for the instruction 35FF995F at address 0013BDD4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_35FF995F()
        {
            AssertCode("@@@", "35FF995F");
        }
        // Reko: a decoder for the instruction B2FCB87D at address 0015B398 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B2FCB87D()
        {
            AssertCode("@@@", "B2FCB87D");
        }
        // Reko: a decoder for the instruction 1CFC6D39 at address 001DBBB4 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 41FC120D at address 0019EEEE has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_41FC120D()
        {
            AssertCode("@@@", "41FC120D");
        }
        // Reko: a decoder for the instruction 7FFCCB39 at address 0013C25C has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7FFCCB39()
        {
            AssertCode("@@@", "7FFCCB39");
        }
        // Reko: a decoder for the instruction 43FEFB98 at address 001BF7DA has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_43FEFB98()
        {
            AssertCode("@@@", "43FEFB98");
        }
        // Reko: a decoder for the instruction E9F94BC8 at address 001DBD24 has not been implemented. (Unimplemented 'single element from one lane - T3' when decoding F9E9C84B)
        [Test]
        public void ThumbDis_E9F94BC8()
        {
            AssertCode("@@@", "E9F94BC8");
        }
        // Reko: a decoder for the instruction 54EFC9D1 at address 001DBD2A has not been implemented. (Unimplemented '*' when decoding EF54D1C9)
        [Test]
        public void ThumbDis_54EFC9D1()
        {
            AssertCode("@@@", "54EFC9D1");
        }
        // Reko: a decoder for the instruction 5AFE635C at address 0015BB94 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_5AFE635C()
        {
            AssertCode("@@@", "5AFE635C");
        }
        // Reko: a decoder for the instruction 7BEE372B at address 001DC254 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_7BEE372B()
        {
            AssertCode("@@@", "7BEE372B");
        }
        // Reko: a decoder for the instruction 99FE3B9D at address 0015BBB4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_99FE3B9D()
        {
            AssertCode("@@@", "99FE3B9D");
        }
        // Reko: a decoder for the instruction F5FEA1DD at address 0019FA40 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F5FEA1DD()
        {
            AssertCode("@@@", "F5FEA1DD");
        }
        // Reko: a decoder for the instruction 54FC29E9 at address 0019FA46 has not been implemented. (op1:op2=0b0001)
    
        // Reko: a decoder for the instruction 46FDAA78 at address 001FEA76 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_46FDAA78()
        {
            AssertCode("@@@", "46FDAA78");
        }
        // Reko: a decoder for the instruction 8FF93AA2 at address 0017D66C has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F98FA23A)
        [Test]
        public void ThumbDis_8FF93AA2()
        {
            AssertCode("@@@", "8FF93AA2");
        }
        // Reko: a decoder for the instruction 12FEB41C at address 001DCC44 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_12FEB41C()
        {
            AssertCode("@@@", "12FEB41C");
        }
        // Reko: a decoder for the instruction 88F9DE07 at address 0013D8EC has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F98807DE)
        [Test]
        public void ThumbDis_88F9DE07()
        {
            AssertCode("@@@", "88F9DE07");
        }
        // Reko: a decoder for the instruction 3CFF96AC at address 001DD918 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)

        // Reko: a decoder for the instruction 89EFEA4D at address 001FFB5A has not been implemented. (Unimplemented '*' when decoding EF894DEA)
        [Test]
        public void ThumbDis_89EFEA4D()
        {
            AssertCode("@@@", "89EFEA4D");
        }
        // Reko: a decoder for the instruction 89FD41DC at address 0013E54E has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_89FD41DC()
        {
            AssertCode("@@@", "89FD41DC");
        }
        // Reko: a decoder for the instruction A4F3ECA3 at address 001DD960 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A4F3ECA3()
        {
            AssertCode("@@@", "A4F3ECA3");
        }
        // Reko: a decoder for the instruction A6F9C81F at address 0017E3E0 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A6F9C81F()
        {
            AssertCode("@@@", "A6F9C81F");
        }
        // Reko: a decoder for the instruction 91FD8D18 at address 001DDDEE has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_91FD8D18()
        {
            AssertCode("@@@", "91FD8D18");
        }
        // Reko: a decoder for the instruction 37FCCF7D at address 0013EBBA has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_37FCCF7D()
        {
            AssertCode("@@@", "37FCCF7D");
        }
        // Reko: a decoder for the instruction 8AFE897D at address 0015E246 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8AFE897D()
        {
            AssertCode("@@@", "8AFE897D");
        }
        // Reko: a decoder for the instruction 41FF95C4 at address 001DEC78 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_41FF95C4()
        {
            AssertCode("@@@", "41FF95C4");
        }
        // Reko: a decoder for the instruction 7AEF6B73 at address 00120E54 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_7AEF6B73()
        {
            AssertCode("@@@", "7AEF6B73");
        }
        // Reko: a decoder for the instruction 2EFDB0D8 at address 001A06DA has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2EFDB0D8()
        {
            AssertCode("@@@", "2EFDB0D8");
        }
        // Reko: a decoder for the instruction C7F981BD at address 00102BE6 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C7F981BD()
        {
            AssertCode("@@@", "C7F981BD");
        }
        // Reko: a decoder for the instruction A8F94687 at address 00181232 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F9A88746)
        [Test]
        public void ThumbDis_A8F94687()
        {
            AssertCode("@@@", "A8F94687");
        }
        // Reko: a decoder for the instruction 17FE528D at address 001A0904 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_17FE528D()
        {
            AssertCode("@@@", "17FE528D");
        }
        // Reko: a decoder for the instruction F2EF9E94 at address 00102D2A has not been implemented. (U=0)
        [Test]
        public void ThumbDis_F2EF9E94()
        {
            AssertCode("@@@", "F2EF9E94");
        }
        // Reko: a decoder for the instruction B2EF5F6C at address 00102DC8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_B2EF5F6C()
        {
            AssertCode("@@@", "B2EF5F6C");
        }
        // Reko: a decoder for the instruction C2F957A7 at address 00160A1C has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F9C2A757)
        [Test]
        public void ThumbDis_C2F957A7()
        {
            AssertCode("@@@", "C2F957A7");
        }
        // Reko: a decoder for the instruction 1DFD76E9 at address 00161248 has not been implemented. (op1:op2=0b1001)
 
        // Reko: a decoder for the instruction 49FD83A9 at address 0016125A has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_49FD83A9()
        {
            AssertCode("@@@", "49FD83A9");
        }
        // Reko: a decoder for the instruction A1FF405E at address 00161272 has not been implemented. (Unimplemented '*' when decoding FFA15E40)
        [Test]
        public void ThumbDis_A1FF405E()
        {
            AssertCode("@@@", "A1FF405E");
        }
        // Reko: a decoder for the instruction 25FFC284 at address 00100E8E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_25FFC284()
        {
            AssertCode("@@@", "25FFC284");
        }
        // Reko: a decoder for the instruction E0EFBA78 at address 0010102E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_E0EFBA78()
        {
            AssertCode("@@@", "E0EFBA78");
        }
        // Reko: a decoder for the instruction 1FFF2603 at address 00161286 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1FFF2603()
        {
            AssertCode("@@@", "1FFF2603");
        }
        // Reko: a decoder for the instruction 07EDD55E at address 00104602 has not been implemented. (Unimplemented '*offset variant' when decoding ED075ED5)
        [Test]
        public void ThumbDis_07EDD55E()
        {
            AssertCode("@@@", "07EDD55E");
        }
        // Reko: a decoder for the instruction 65FECC3C at address 001046AC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_65FECC3C()
        {
            AssertCode("@@@", "65FECC3C");
        }
        // Reko: a decoder for the instruction 35FC617D at address 00141044 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_35FC617D()
        {
            AssertCode("@@@", "35FC617D");
        }
        // Reko: a decoder for the instruction 2CEF3C13 at address 001047BE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2CEF3C13()
        {
            AssertCode("@@@", "2CEF3C13");
        }
        // Reko: a decoder for the instruction 07FD87E8 at address 001048D0 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_07FD87E8()
        {
            AssertCode("@@@", "07FD87E8");
        }
        // Reko: a decoder for the instruction E6F9D881 at address 001012F4 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9E681D8)
        [Test]
        public void ThumbDis_E6F9D881()
        {
            AssertCode("@@@", "E6F9D881");
        }
        // Reko: a decoder for the instruction C9ECBC3B at address 00121610 has not been implemented. (Unimplemented '*' when decoding ECC93BBC)
        [Test]
        public void ThumbDis_C9ECBC3B()
        {
            AssertCode("@@@", "C9ECBC3B");
        }
        // Reko: a decoder for the instruction ADFA8436 at address 001013F0 has not been implemented. (Unimplemented '*' when decoding FAAD3684)
        [Test]
        public void ThumbDis_ADFA8436()
        {
            AssertCode("@@@", "ADFA8436");
        }
        // Reko: a decoder for the instruction 24EF954E at address 001C103C has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
 
        // Reko: a decoder for the instruction 59EFCB1C at address 001C1056 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_59EFCB1C()
        {
            AssertCode("@@@", "59EFCB1C");
        }
        // Reko: a decoder for the instruction 3AF9F2DE at address 001E0A50 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_3AF9F2DE()
        {
            AssertCode("@@@", "3AF9F2DE");
        }
        // Reko: a decoder for the instruction 8FEFD3DC at address 001E0ABA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_8FEFD3DC()
        {
            AssertCode("@@@", "8FEFD3DC");
        }
        // Reko: a decoder for the instruction 82FEDA8C at address 001E0C44 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_82FEDA8C()
        {
            AssertCode("@@@", "82FEDA8C");
        }
        // Reko: a decoder for the instruction 17EC3DDF at address 0014205A has not been implemented. (Unimplemented '*' when decoding EC17DF3D)
        [Test]
        public void ThumbDis_17EC3DDF()
        {
            AssertCode("@@@", "17EC3DDF");
        }
        // Reko: a decoder for the instruction C3F344A7 at address 001E0DF2 has not been implemented. (Unimplemented '*' when decoding F3C3A744)
        [Test]
        public void ThumbDis_C3F344A7()
        {
            AssertCode("@@@", "C3F344A7");
        }
        // Reko: a decoder for the instruction EEFE5949 at address 001E0DF6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EEFE5949()
        {
            AssertCode("@@@", "EEFE5949");
        }
        // Reko: a decoder for the instruction 3BFD105E at address 00121BEC has not been implemented. (Unimplemented '*preindexed variant' when decoding FD3B5E10)
        [Test]
        public void ThumbDis_3BFD105E()
        {
            AssertCode("@@@", "3BFD105E");
        }
        // Reko: a decoder for the instruction 4BEF9FCF at address 0010176A has not been implemented. (Unimplemented '*' when decoding EF4BCF9F)
        [Test]
        public void ThumbDis_4BEF9FCF()
        {
            AssertCode("@@@", "4BEF9FCF");
        }
        // Reko: a decoder for the instruction 63EF3931 at address 00182480 has not been implemented. (Unimplemented '*register' when decoding EF633139)
        [Test]
        public void ThumbDis_63EF3931()
        {
            AssertCode("@@@", "63EF3931");
        }
        // Reko: a decoder for the instruction A0F9B2B5 at address 001E0E94 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9A0B5B2)
        [Test]
        public void ThumbDis_A0F9B2B5()
        {
            AssertCode("@@@", "A0F9B2B5");
        }
        // Reko: a decoder for the instruction C4EC11FA at address 001E0EA2 has not been implemented. (Unimplemented '*' when decoding ECC4FA11)
        [Test]
        public void ThumbDis_C4EC11FA()
        {
            AssertCode("@@@", "C4EC11FA");
        }
        // Reko: a decoder for the instruction 7DF93A5D at address 001E0ED8 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
  
        // Reko: a decoder for the instruction 56FF3F0B at address 0010179E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_56FF3F0B()
        {
            AssertCode("@@@", "56FF3F0B");
        }
        // Reko: a decoder for the instruction 9DEF12E4 at address 001827EA has not been implemented. (U=0)
        [Test]
        public void ThumbDis_9DEF12E4()
        {
            AssertCode("@@@", "9DEF12E4");
        }
        // Reko: a decoder for the instruction C4EF57DE at address 0016164C has not been implemented. (Unimplemented '*immediate - T4' when decoding EFC4DE57)
        [Test]
        public void ThumbDis_C4EF57DE()
        {
            AssertCode("@@@", "C4EF57DE");
        }
        // Reko: a decoder for the instruction A5FCDFF9 at address 001A0BA2 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A5FCDFF9()
        {
            AssertCode("@@@", "A5FCDFF9");
        }
        // Reko: a decoder for the instruction F0F7BA8D at address 00123DF8 has not been implemented. (Unimplemented '*' when decoding F7F08DBA)
        [Test]
        public void ThumbDis_F0F7BA8D()
        {
            AssertCode("@@@", "F0F7BA8D");
        }
        // Reko: a decoder for the instruction 73FF7E89 at address 001E0F1E has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 13FD9378 at address 001E0F68 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 6EFFCF63 at address 001050D8 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_6EFFCF63()
        {
            AssertCode("@@@", "6EFFCF63");
        }
        // Reko: a decoder for the instruction DEEC29AB at address 001057F6 has not been implemented. (Unimplemented '*' when decoding ECDEAB29)
        [Test]
        public void ThumbDis_DEEC29AB()
        {
            AssertCode("@@@", "DEEC29AB");
        }
        // Reko: a decoder for the instruction CFF95CC9 at address 00101ABE has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9CFC95C)
        [Test]
        public void ThumbDis_CFF95CC9()
        {
            AssertCode("@@@", "CFF95CC9");
        }
        // Reko: a decoder for the instruction 05EFF994 at address 001C1542 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_05EFF994()
        {
            AssertCode("@@@", "05EFF994");
        }
        // Reko: a decoder for the instruction 8AF94DDB at address 00105B7C has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F98ADB4D)
        [Test]
        public void ThumbDis_8AF94DDB()
        {
            AssertCode("@@@", "8AF94DDB");
        }
        // Reko: a decoder for the instruction E2F99B6C at address 00161ABA has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E2F99B6C()
        {
            AssertCode("@@@", "E2F99B6C");
        }
        // Reko: a decoder for the instruction D5EF608B at address 001A1184 has not been implemented. (Unimplemented '*' when decoding EFD58B60)
        [Test]
        public void ThumbDis_D5EF608B()
        {
            AssertCode("@@@", "D5EF608B");
        }
        // Reko: a decoder for the instruction 80FEE33C at address 001E0FC2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_80FEE33C()
        {
            AssertCode("@@@", "80FEE33C");
        }
        // Reko: a decoder for the instruction BFFCB67D at address 001C1626 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BFFCB67D()
        {
            AssertCode("@@@", "BFFCB67D");
        }
        // Reko: a decoder for the instruction 19EF99FE at address 001E18E6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_19EF99FE()
        {
            AssertCode("@@@", "19EF99FE");
        }
        // Reko: a decoder for the instruction EAF9CFD0 at address 00125492 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9EAD0CF)
        [Test]
        public void ThumbDis_EAF9CFD0()
        {
            AssertCode("@@@", "EAF9CFD0");
        }
        // Reko: a decoder for the instruction 00FCF70C at address 001A11CC has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_00FCF70C()
        {
            AssertCode("@@@", "00FCF70C");
        }
        // Reko: a decoder for the instruction 80FF106E at address 00183BB4 has not been implemented. (Unimplemented '*immediate - T4' when decoding FF806E10)
        [Test]
        public void ThumbDis_80FF106E()
        {
            AssertCode("@@@", "80FF106E");
        }
        // Reko: a decoder for the instruction 25EF8DDC at address 00108536 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_25EF8DDC()
        {
            AssertCode("@@@", "25EF8DDC");
        }
        // Reko: a decoder for the instruction 41FCE249 at address 001833E2 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_41FCE249()
        {
            AssertCode("@@@", "41FCE249");
        }
        // Reko: a decoder for the instruction 06FD863D at address 001E2A20 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_06FD863D()
        {
            AssertCode("@@@", "06FD863D");
        }
        // Reko: a decoder for the instruction 8DEF110B at address 001437D6 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_8DEF110B()
        {
            AssertCode("@@@", "8DEF110B");
        }
        // Reko: a decoder for the instruction 33EF7013 at address 001C25AC has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_33EF7013()
        {
            AssertCode("@@@", "33EF7013");
        }
        // Reko: a decoder for the instruction 84EC104A at address 001C25EE has not been implemented. (Unimplemented '*' when decoding EC844A10)
        [Test]
        public void ThumbDis_84EC104A()
        {
            AssertCode("vstmia\tr4,{s8-s23}", "84EC104A");
        }
        // Reko: a decoder for the instruction 16EF776F at address 00183FDE has not been implemented. (Unimplemented '*' when decoding EF166F77)
        [Test]
        public void ThumbDis_16EF776F()
        {
            AssertCode("@@@", "16EF776F");
        }
        // Reko: a decoder for the instruction 5CFDC63C at address 001C34D6 has not been implemented. (op1:op2=0b1001)
 
        // Reko: a decoder for the instruction EFF9133D at address 00126064 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_EFF9133D()
        {
            AssertCode("@@@", "EFF9133D");
        }
        // Reko: a decoder for the instruction EEFE1469 at address 00162CE6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EEFE1469()
        {
            AssertCode("@@@", "EEFE1469");
        }
        // Reko: a decoder for the instruction A5FCDE29 at address 001E3076 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A5FCDE29()
        {
            AssertCode("@@@", "A5FCDE29");
        }
        // Reko: a decoder for the instruction 51EFC6CE at address 001E30B2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_51EFC6CE()
        {
            AssertCode("@@@", "51EFC6CE");
        }
        // Reko: a decoder for the instruction DEFFF34B at address 001E32F2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_DEFFF34B()
        {
            AssertCode("@@@", "DEFFF34B");
        }
        // Reko: a decoder for the instruction 87FFED1A at address 00106EE4 has not been implemented. (Unimplemented '*' when decoding FF871AED)
        [Test]
        public void ThumbDis_87FFED1A()
        {
            AssertCode("@@@", "87FFED1A");
        }
        // Reko: a decoder for the instruction 0FEF313C at address 001E340C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_0FEF313C()
        {
            AssertCode("@@@", "0FEF313C");
        }
        // Reko: a decoder for the instruction DEFFE50C at address 001A199E has not been implemented. (Unimplemented '*' when decoding FFDE0CE5)
        [Test]
        public void ThumbDis_DEFFE50C()
        {
            AssertCode("@@@", "DEFFE50C");
        }
        // Reko: a decoder for the instruction 2DEFDD3C at address 001C36AA has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_2DEFDD3C()
        {
            AssertCode("@@@", "2DEFDD3C");
        }
        // Reko: a decoder for the instruction ADF9E8CF at address 00107342 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_ADF9E8CF()
        {
            AssertCode("@@@", "ADF9E8CF");
        }
        // Reko: a decoder for the instruction 8BFC4BED at address 001A1A24 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8BFC4BED()
        {
            AssertCode("@@@", "8BFC4BED");
        }
        // Reko: a decoder for the instruction A9F3D7A9 at address 0010775E has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A9F3D7A9()
        {
            AssertCode("@@@", "A9F3D7A9");
        }
        // Reko: a decoder for the instruction 56FCE349 at address 001E36EC has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 2AEF2853 at address 001A1C02 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2AEF2853()
        {
            AssertCode("@@@", "2AEF2853");
        }
        // Reko: a decoder for the instruction 58FC6B08 at address 0010919E has not been implemented. (op1:op2=0b0001)
   
        // Reko: a decoder for the instruction C8FD102D at address 0010924E has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C8FD102D()
        {
            AssertCode("@@@", "C8FD102D");
        }
        // Reko: a decoder for the instruction 1CEDB65E at address 001632EC has not been implemented. (Unimplemented '*offset variant' when decoding ED1C5EB6)
        [Test]
        public void ThumbDis_1CEDB65E()
        {
            AssertCode("@@@", "1CEDB65E");
        }
        // Reko: a decoder for the instruction E8EFC534 at address 001632F8 has not been implemented. (Unimplemented '*scalar' when decoding EFE834C5)
        [Test]
        public void ThumbDis_E8EFC534()
        {
            AssertCode("@@@", "E8EFC534");
        }
        // Reko: a decoder for the instruction FFEF92AC at address 001A1EBE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_FFEF92AC()
        {
            AssertCode("@@@", "FFEF92AC");
        }
        // Reko: a decoder for the instruction 45F92132 at address 001846C2 has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F9453221)
        [Test]
        public void ThumbDis_45F92132()
        {
            AssertCode("@@@", "45F92132");
        }
        // Reko: a decoder for the instruction 8EF96811 at address 001846D0 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F98E1168)
        [Test]
        public void ThumbDis_8EF96811()
        {
            AssertCode("@@@", "8EF96811");
        }
        // Reko: a decoder for the instruction 01FED32D at address 001077F6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_01FED32D()
        {
            AssertCode("@@@", "01FED32D");
        }
        // Reko: a decoder for the instruction CDFC525C at address 0012685A has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CDFC525C()
        {
            AssertCode("@@@", "CDFC525C");
        }
        // Reko: a decoder for the instruction 30FE786C at address 001092A0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_30FE786C()
        {
            AssertCode("@@@", "30FE786C");
        }
        // Reko: a decoder for the instruction 80FF6775 at address 00143B46 has not been implemented. (Unimplemented '*scalar' when decoding FF807567)
        [Test]
        public void ThumbDis_80FF6775()
        {
            AssertCode("@@@", "80FF6775");
        }
        // Reko: a decoder for the instruction DAFDE588 at address 001077FA has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DAFDE588()
        {
            AssertCode("@@@", "DAFDE588");
        }
        // Reko: a decoder for the instruction 1DFF4F4C at address 00184844 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)

        // Reko: a decoder for the instruction 82F99249 at address 001092AA has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9824992)
        [Test]
        public void ThumbDis_82F99249()
        {
            AssertCode("@@@", "82F99249");
        }
        // Reko: a decoder for the instruction ECFD361D at address 0012A3AC has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_ECFD361D()
        {
            AssertCode("@@@", "ECFD361D");
        }
        // Reko: a decoder for the instruction F1EC3D3B at address 00109A90 has not been implemented. (Unimplemented '*' when decoding ECF13B3D)
        [Test]
        public void ThumbDis_F1EC3D3B()
        {
            AssertCode("@@@", "F1EC3D3B");
        }
        // Reko: a decoder for the instruction CDEE5B69 at address 00126FC6 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 1FEFDDEC at address 0010C2EE has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_1FEFDDEC()
        {
            AssertCode("@@@", "1FEFDDEC");
        }
        // Reko: a decoder for the instruction F7FF5C69 at address 001E3FE2 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FFF7695C)
        [Test]
        public void ThumbDis_F7FF5C69()
        {
            AssertCode("@@@", "F7FF5C69");
        }
        // Reko: a decoder for the instruction 0EEFE08B at address 001270A6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_0EEFE08B()
        {
            AssertCode("@@@", "0EEFE08B");
        }
        // Reko: a decoder for the instruction C2F98FAB at address 00185B10 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C2AB8F)
        [Test]
        public void ThumbDis_C2F98FAB()
        {
            AssertCode("@@@", "C2F98FAB");
        }
        // Reko: a decoder for the instruction ADFCFFAC at address 001E42CA has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_ADFCFFAC()
        {
            AssertCode("@@@", "ADFCFFAC");
        }
        // Reko: a decoder for the instruction BFFE0C8D at address 00109FE8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_BFFE0C8D()
        {
            AssertCode("@@@", "BFFE0C8D");
        }
        // Reko: a decoder for the instruction 51EF37C3 at address 001272D2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_51EF37C3()
        {
            AssertCode("@@@", "51EF37C3");
        }
        // Reko: a decoder for the instruction 4FEE34F9 at address 00185B84 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_4FEE34F9()
        {
            AssertCode("@@@", "4FEE34F9");
        }
        // Reko: a decoder for the instruction 36F92F1D at address 001275CC has not been implemented. (LoadStoreSignedImmediatePreIndexed)

        // Reko: a decoder for the instruction DDFF348B at address 0010A566 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_DDFF348B()
        {
            AssertCode("@@@", "DDFF348B");
        }
        // Reko: a decoder for the instruction 46EF4F2E at address 001C56B6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_46EF4F2E()
        {
            AssertCode("@@@", "46EF4F2E");
        }
        // Reko: a decoder for the instruction 63FF0E13 at address 001449D6 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_63FF0E13()
        {
            AssertCode("@@@", "63FF0E13");
        }
        // Reko: a decoder for the instruction A5EF8B2D at address 001275FE has not been implemented. (Unimplemented '*integer' when decoding EFA52D8B)
        [Test]
        public void ThumbDis_A5EF8B2D()
        {
            AssertCode("@@@", "A5EF8B2D");
        }
        // Reko: a decoder for the instruction B1FC76FD at address 0010CBF0 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B1FC76FD()
        {
            AssertCode("@@@", "B1FC76FD");
        }
        // Reko: a decoder for the instruction 83EFF37D at address 0010A624 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_83EFF37D()
        {
            AssertCode("@@@", "83EFF37D");
        }
        // Reko: a decoder for the instruction DCFF4B0A at address 001E4CCC has not been implemented. (Unimplemented '*' when decoding FFDC0A4B)
        [Test]
        public void ThumbDis_DCFF4B0A()
        {
            AssertCode("@@@", "DCFF4B0A");
        }
        // Reko: a decoder for the instruction 8FFE8D3D at address 001C5AB8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8FFE8D3D()
        {
            AssertCode("@@@", "8FFE8D3D");
        }
        // Reko: a decoder for the instruction AAFFD8EB at address 0012B622 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_AAFFD8EB()
        {
            AssertCode("@@@", "AAFFD8EB");
        }
        // Reko: a decoder for the instruction 74FF4223 at address 0012B66C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_74FF4223()
        {
            AssertCode("@@@", "74FF4223");
        }
        // Reko: a decoder for the instruction 62EFACA4 at address 001A4442 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_62EFACA4()
        {
            AssertCode("@@@", "62EFACA4");
        }
        // Reko: a decoder for the instruction 2DFCDC5E at address 0012B754 has not been implemented. (Unimplemented '*post-indexed' when decoding FC2D5EDC)
        [Test]
        public void ThumbDis_2DFCDC5E()
        {
            AssertCode("@@@", "2DFCDC5E");
        }
        // Reko: a decoder for the instruction CDEFCDDB at address 0010A966 has not been implemented. (Unimplemented '*' when decoding EFCDDBCD)
        [Test]
        public void ThumbDis_CDEFCDDB()
        {
            AssertCode("@@@", "CDEFCDDB");
        }
        // Reko: a decoder for the instruction EEF95A77 at address 00127672 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F9EE775A)
        [Test]
        public void ThumbDis_EEF95A77()
        {
            AssertCode("@@@", "EEF95A77");
        }
        // Reko: a decoder for the instruction 85EEF679 at address 0016588A has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction 3CFCB52D at address 0018402E has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_3CFCB52D()
        {
            AssertCode("@@@", "3CFCB52D");
        }
        // Reko: a decoder for the instruction EEF9C7FC at address 0012BFCE has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_EEF9C7FC()
        {
            AssertCode("@@@", "EEF9C7FC");
        }
        // Reko: a decoder for the instruction 82F97EE3 at address 00145498 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F982E37E)
        [Test]
        public void ThumbDis_82F97EE3()
        {
            AssertCode("@@@", "82F97EE3");
        }
        // Reko: a decoder for the instruction AFF9A61C at address 0012C180 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_AFF9A61C()
        {
            AssertCode("@@@", "AFF9A61C");
        }
        // Reko: a decoder for the instruction 78FFD08B at address 00127B88 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_78FFD08B()
        {
            AssertCode("@@@", "78FFD08B");
        }
        // Reko: a decoder for the instruction 56F9F16D at address 001E5680 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
  
        // Reko: a decoder for the instruction E2F9999A at address 0018430A has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9E29A99)
        [Test]
        public void ThumbDis_E2F9999A()
        {
            AssertCode("@@@", "E2F9999A");
        }
        // Reko: a decoder for the instruction E6FE196D at address 0018474E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E6FE196D()
        {
            AssertCode("@@@", "E6FE196D");
        }
        // Reko: a decoder for the instruction 5AEFF3EF at address 0010B8B0 has not been implemented. (Unimplemented '*' when decoding EF5AEFF3)
        [Test]
        public void ThumbDis_5AEFF3EF()
        {
            AssertCode("@@@", "5AEFF3EF");
        }
        // Reko: a decoder for the instruction CEEE958B at address 0010B8C4 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_CEEE958B()
        {
            AssertCode("@@@", "CEEE958B");
        }
        // Reko: a decoder for the instruction C0F92C32 at address 0010BA20 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9C0322C)
        [Test]
        public void ThumbDis_C0F92C32()
        {
            AssertCode("@@@", "C0F92C32");
        }
        // Reko: a decoder for the instruction 9CF946FC at address 0010BA80 has not been implemented. (PLI)
        [Test]
        public void ThumbDis_9CF946FC()
        {
            AssertCode("@@@", "9CF946FC");
        }
        // Reko: a decoder for the instruction E8FD6B48 at address 00127BD6 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E8FD6B48()
        {
            AssertCode("@@@", "E8FD6B48");
        }
        // Reko: a decoder for the instruction 92FAAF34 at address 001E5796 has not been implemented. (Unimplemented '*' when decoding FA9234AF)

        // Reko: a decoder for the instruction 87F9BE8F at address 0010D552 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_87F9BE8F()
        {
            AssertCode("@@@", "87F9BE8F");
        }
        // Reko: a decoder for the instruction 49FDA138 at address 001656A0 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_49FDA138()
        {
            AssertCode("@@@", "49FDA138");
        }
        // Reko: a decoder for the instruction B9FCC60D at address 00127D96 has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B9FCC60D()
        {
            AssertCode("@@@", "B9FCC60D");
        }
        // Reko: a decoder for the instruction 7DFFE08D at address 0010D5C4 has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_7DFFE08D()
        {
            AssertCode("@@@", "7DFFE08D");
        }
        // Reko: a decoder for the instruction E0F9C0FE at address 00165E42 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E0F9C0FE()
        {
            AssertCode("@@@", "E0F9C0FE");
        }
        // Reko: a decoder for the instruction BEEFF5AC at address 001E5872 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_BEEFF5AC()
        {
            AssertCode("@@@", "BEEFF5AC");
        }
        // Reko: a decoder for the instruction F3EFFD2C at address 0010B8B2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_F3EFFD2C()
        {
            AssertCode("@@@", "F3EFFD2C");
        }
        // Reko: a decoder for the instruction 2FFFB5E2 at address 0010BFE0 has not been implemented. (Unimplemented '*' when decoding FF2FE2B5)
        [Test]
        public void ThumbDis_2FFFB5E2()
        {
            AssertCode("@@@", "2FFFB5E2");
        }
        // Reko: a decoder for the instruction 42FD542C at address 00166B12 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_42FD542C()
        {
            AssertCode("@@@", "42FD542C");
        }
        // Reko: a decoder for the instruction D0FF7379 at address 0012DF22 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FFD07973)
        [Test]
        public void ThumbDis_D0FF7379()
        {
            AssertCode("@@@", "D0FF7379");
        }
        // Reko: a decoder for the instruction 1CEFF5DF at address 00144E10 has not been implemented. (Unimplemented '*' when decoding EF1CDFF5)
        [Test]
        public void ThumbDis_1CEFF5DF()
        {
            AssertCode("@@@", "1CEFF5DF");
        }
        // Reko: a decoder for the instruction D1FC1649 at address 001E637E has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D1FC1649()
        {
            AssertCode("@@@", "D1FC1649");
        }
        // Reko: a decoder for the instruction 35EF929B at address 00166B58 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)

        // Reko: a decoder for the instruction 65EEB1E9 at address 0012DF30 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_65EEB1E9()
        {
            AssertCode("@@@", "65EEB1E9");
        }
        // Reko: a decoder for the instruction F9FF20F4 at address 00114622 has not been implemented. (Unimplemented '*imm0' when decoding FFF9F420)
        [Test]
        public void ThumbDis_F9FF20F4()
        {
            AssertCode("@@@", "F9FF20F4");
        }
        // Reko: a decoder for the instruction 44FC7FAD at address 00135706 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_44FC7FAD()
        {
            AssertCode("@@@", "44FC7FAD");
        }
        // Reko: a decoder for the instruction DFE8F5BE at address 001153E0 has not been implemented. (Unimplemented '*' when decoding E8DFBEF5)
        [Test]
        public void ThumbDis_DFE8F5BE()
        {
            AssertCode("@@@", "DFE8F5BE");
        }
        // Reko: a decoder for the instruction 7AFEF369 at address 0010E04A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_7AFEF369()
        {
            AssertCode("@@@", "7AFEF369");
        }
        // Reko: a decoder for the instruction 04EF9322 at address 0010E08C has not been implemented. (Unimplemented '*' when decoding EF042293)
        [Test]
        public void ThumbDis_04EF9322()
        {
            AssertCode("@@@", "04EF9322");
        }
        // Reko: a decoder for the instruction 54FFD702 at address 0010E09A has not been implemented. (Unimplemented '*' when decoding FF5402D7)
        [Test]
        public void ThumbDis_54FFD702()
        {
            AssertCode("@@@", "54FFD702");
        }
        // Reko: a decoder for the instruction 10FF7EE9 at address 00187B3C has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction A2F96DD1 at address 001A5D98 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9A2D16D)
        [Test]
        public void ThumbDis_A2F96DD1()
        {
            AssertCode("@@@", "A2F96DD1");
        }
        // Reko: a decoder for the instruction 42FD042D at address 00187CBC has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_42FD042D()
        {
            AssertCode("@@@", "42FD042D");
        }
        // Reko: a decoder for the instruction 8AF998BE at address 00135C0E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8AF998BE()
        {
            AssertCode("@@@", "8AF998BE");
        }
        // Reko: a decoder for the instruction E5EF5FFB at address 0012E400 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_E5EF5FFB()
        {
            AssertCode("@@@", "E5EF5FFB");
        }
        // Reko: a decoder for the instruction C5EF626C at address 001E766A has not been implemented. (Unimplemented '*' when decoding EFC56C62)
        [Test]
        public void ThumbDis_C5EF626C()
        {
            AssertCode("@@@", "C5EF626C");
        }
        // Reko: a decoder for the instruction D9F3BEAA at address 001C8FE2 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D9F3BEAA()
        {
            AssertCode("@@@", "D9F3BEAA");
        }
        // Reko: a decoder for the instruction EEF36387 at address 00168246 has not been implemented. (Unimplemented '*banked register' when decoding F3EE8763)
        [Test]
        public void ThumbDis_EEF36387()
        {
            AssertCode("@@@", "EEF36387");
        }
        // Reko: a decoder for the instruction 0BFF80C4 at address 001C98CA has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_0BFF80C4()
        {
            AssertCode("@@@", "0BFF80C4");
        }
        // Reko: a decoder for the instruction 8FFD6CE8 at address 00188644 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8FFD6CE8()
        {
            AssertCode("@@@", "8FFD6CE8");
        }
        // Reko: a decoder for the instruction F8FEE9CC at address 001684AA has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F8FEE9CC()
        {
            AssertCode("@@@", "F8FEE9CC");
        }
        // Reko: a decoder for the instruction 80F9A379 at address 001E8602 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F98079A3)
        [Test]
        public void ThumbDis_80F9A379()
        {
            AssertCode("@@@", "80F9A379");
        }
        // Reko: a decoder for the instruction 8DF93522 at address 00147956 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F98D2235)
        [Test]
        public void ThumbDis_8DF93522()
        {
            AssertCode("@@@", "8DF93522");
        }
        // Reko: a decoder for the instruction CCEFF808 at address 00168558 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_CCEFF808()
        {
            AssertCode("@@@", "CCEFF808");
        }
        // Reko: a decoder for the instruction A3F99F3A at address 001886BA has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9A33A9F)
        [Test]
        public void ThumbDis_A3F99F3A()
        {
            AssertCode("@@@", "A3F99F3A");
        }
        // Reko: a decoder for the instruction 3BFED369 at address 001690F6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3BFED369()
        {
            AssertCode("@@@", "3BFED369");
        }
        // Reko: a decoder for the instruction 2CFFD29C at address 00188A2C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_2CFFD29C()
        {
            AssertCode("@@@", "2CFFD29C");
        }
        // Reko: a decoder for the instruction 74FC708D at address 00188A4C has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_74FC708D()
        {
            AssertCode("@@@", "74FC708D");
        }
        // Reko: a decoder for the instruction 5FFD0EFC at address 00136FF6 has not been implemented. (op1:op2=0b1001)
  
        // Reko: a decoder for the instruction 13EB16CF at address 0012EC74 has not been implemented. (Unimplemented '*register' when decoding EB13CF16)
        [Test]
        public void ThumbDis_13EB16CF()
        {
            AssertCode("@@@", "13EB16CF");
        }
        // Reko: a decoder for the instruction E5FEB87C at address 00188A6E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E5FEB87C()
        {
            AssertCode("@@@", "E5FEB87C");
        }
        // Reko: a decoder for the instruction 80FEB97C at address 001175A2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_80FEB97C()
        {
            AssertCode("@@@", "80FEB97C");
        }
        // Reko: a decoder for the instruction A2F98EB1 at address 001C860C has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9A2B18E)
        [Test]
        public void ThumbDis_A2F98EB1()
        {
            AssertCode("@@@", "A2F98EB1");
        }
        // Reko: a decoder for the instruction A3F9042D at address 00117772 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A3F9042D()
        {
            AssertCode("@@@", "A3F9042D");
        }
        // Reko: a decoder for the instruction E9F95C6E at address 0012F380 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E9F95C6E()
        {
            AssertCode("@@@", "E9F95C6E");
        }
        // Reko: a decoder for the instruction B6EFB324 at address 00169E36 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_B6EFB324()
        {
            AssertCode("@@@", "B6EFB324");
        }
        // Reko: a decoder for the instruction ABF9AF42 at address 001379EA has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9AB42AF)
        [Test]
        public void ThumbDis_ABF9AF42()
        {
            AssertCode("@@@", "ABF9AF42");
        }
        // Reko: a decoder for the instruction A2F9D32E at address 0010F0DA has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A2F9D32E()
        {
            AssertCode("@@@", "A2F9D32E");
        }
        // Reko: a decoder for the instruction ECFDE6DC at address 0012F402 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_ECFDE6DC()
        {
            AssertCode("@@@", "ECFDE6DC");
        }
        // Reko: a decoder for the instruction C0FCD69D at address 00169EE4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C0FCD69D()
        {
            AssertCode("@@@", "C0FCD69D");
        }
        // Reko: a decoder for the instruction 8FF97F67 at address 0010FCDE has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F98F677F)
        [Test]
        public void ThumbDis_8FF97F67()
        {
            AssertCode("@@@", "8FF97F67");
        }
        // Reko: a decoder for the instruction F2FFD90D at address 00188DA8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_F2FFD90D()
        {
            AssertCode("@@@", "F2FFD90D");
        }
        // Reko: a decoder for the instruction 6CFD087D at address 0012F680 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6CFD087D()
        {
            AssertCode("@@@", "6CFD087D");
        }
        // Reko: a decoder for the instruction CBF3B58D at address 00168D22 has not been implemented. (Unimplemented '*' when decoding F3CB8DB5)
        [Test]
        public void ThumbDis_CBF3B58D()
        {
            AssertCode("@@@", "CBF3B58D");
        }
        // Reko: a decoder for the instruction BFEFD54B at address 0013849E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_BFEFD54B()
        {
            AssertCode("@@@", "BFEFD54B");
        }
        // Reko: a decoder for the instruction 82EFD0C4 at address 001A9130 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_82EFD0C4()
        {
            AssertCode("@@@", "82EFD0C4");
        }
        // Reko: a decoder for the instruction 81F9036D at address 001189EC has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_81F9036D()
        {
            AssertCode("@@@", "81F9036D");
        }
        // Reko: a decoder for the instruction 1CFF3A7B at address 0013851E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_1CFF3A7B()
        {
            AssertCode("@@@", "1CFF3A7B");
        }
        // Reko: a decoder for the instruction 7EEF800B at address 00138554 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_7EEF800B()
        {
            AssertCode("@@@", "7EEF800B");
        }
        // Reko: a decoder for the instruction A9FA8571 at address 001A915E has not been implemented. (Unimplemented '*' when decoding FAA97185)
        [Test]
        public void ThumbDis_A9FA8571()
        {
            AssertCode("@@@", "A9FA8571");
        }
        // Reko: a decoder for the instruction 5CFE7F28 at address 00118AB8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_5CFE7F28()
        {
            AssertCode("@@@", "5CFE7F28");
        }
        // Reko: a decoder for the instruction 31EFF342 at address 0013911C has not been implemented. (Unimplemented '*' when decoding EF3142F3)
        [Test]
        public void ThumbDis_31EFF342()
        {
            AssertCode("@@@", "31EFF342");
        }
        // Reko: a decoder for the instruction 41FEFB9D at address 0010FF9C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_41FEFB9D()
        {
            AssertCode("@@@", "41FEFB9D");
        }
        // Reko: a decoder for the instruction 80EE10F9 at address 001CAEEC has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_80EE10F9()
        {
            AssertCode("@@@", "80EE10F9");
        }
        // Reko: a decoder for the instruction 5AEF1029 at address 0011045A has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_5AEF1029()
        {
            AssertCode("@@@", "5AEF1029");
        }
        // Reko: a decoder for the instruction 02FFEF61 at address 0012FAAC has not been implemented. (Unimplemented '*' when decoding FF0261EF)
        [Test]
        public void ThumbDis_02FFEF61()
        {
            AssertCode("@@@", "02FFEF61");
        }
        // Reko: a decoder for the instruction 52FEB8FC at address 0012FAB8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_52FEB8FC()
        {
            AssertCode("@@@", "52FEB8FC");
        }
        // Reko: a decoder for the instruction AEEC290A at address 0012FABC has not been implemented. (Unimplemented '*' when decoding ECAE0A29)
        [Test]
        public void ThumbDis_AEEC290A()
        {
            AssertCode("@@@", "AEEC290A");
        }
        // Reko: a decoder for the instruction ACFC891C at address 0012FAD4 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_ACFC891C()
        {
            AssertCode("@@@", "ACFC891C");
        }
        // Reko: a decoder for the instruction F4FFCA83 at address 0013062C has not been implemented. (Unimplemented '*' when decoding FFF483CA)
        [Test]
        public void ThumbDis_F4FFCA83()
        {
            AssertCode("@@@", "F4FFCA83");
        }
        // Reko: a decoder for the instruction 46FF6D13 at address 0013A4FE has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_46FF6D13()
        {
            AssertCode("@@@", "46FF6D13");
        }
        // Reko: a decoder for the instruction C4F9514B at address 0013091A has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C44B51)
        [Test]
        public void ThumbDis_C4F9514B()
        {
            AssertCode("@@@", "C4F9514B");
        }
        // Reko: a decoder for the instruction 10EFE984 at address 0016B752 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_10EFE984()
        {
            AssertCode("@@@", "10EFE984");
        }
        // Reko: a decoder for the instruction 94FF3DAD at address 001CBD08 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_94FF3DAD()
        {
            AssertCode("@@@", "94FF3DAD");
        }
        // Reko: a decoder for the instruction 36FF1AE2 at address 0011A6EE has not been implemented. (Unimplemented '*' when decoding FF36E21A)
        [Test]
        public void ThumbDis_36FF1AE2()
        {
            AssertCode("@@@", "36FF1AE2");
        }
        // Reko: a decoder for the instruction 51FC2148 at address 001CBE20 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 2DEFD7D4 at address 0011AD26 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_2DEFD7D4()
        {
            AssertCode("@@@", "2DEFD7D4");
        }
        // Reko: a decoder for the instruction F1EFF104 at address 001EA9AA has not been implemented. (U=0)
        [Test]
        public void ThumbDis_F1EFF104()
        {
            AssertCode("@@@", "F1EFF104");
        }
        // Reko: a decoder for the instruction 7FEF08C1 at address 00148A4C has not been implemented. (Unimplemented '*' when decoding EF7FC108)
        [Test]
        public void ThumbDis_7FEF08C1()
        {
            AssertCode("@@@", "7FEF08C1");
        }
        // Reko: a decoder for the instruction F9FE2CAD at address 00110FB6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F9FE2CAD()
        {
            AssertCode("@@@", "F9FE2CAD");
        }
        // Reko: a decoder for the instruction B3FD0C5D at address 0013AF1A has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B3FD0C5D()
        {
            AssertCode("@@@", "B3FD0C5D");
        }
        // Reko: a decoder for the instruction 71FE17C9 at address 0018BA60 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_71FE17C9()
        {
            AssertCode("@@@", "71FE17C9");
        }
        // Reko: a decoder for the instruction B3FEEC4D at address 001AAD12 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B3FEEC4D()
        {
            AssertCode("@@@", "B3FEEC4D");
        }
        // Reko: a decoder for the instruction 57F9E0BE at address 001313B6 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_57F9E0BE()
        {
            AssertCode("@@@", "57F9E0BE");
        }
        // Reko: a decoder for the instruction 58FF0AE4 at address 001313FC has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_58FF0AE4()
        {
            AssertCode("@@@", "58FF0AE4");
        }
        // Reko: a decoder for the instruction 18FD4739 at address 0018A674 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 54FCC9D9 at address 001104A0 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction C6FA948F at address 0013B598 has not been implemented. (crc32-crc32h)
        [Test]
        public void ThumbDis_C6FA948F()
        {
            AssertCode("@@@", "C6FA948F");
        }
        // Reko: a decoder for the instruction D6FD913D at address 001AB9E8 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D6FD913D()
        {
            AssertCode("@@@", "D6FD913D");
        }
        // Reko: a decoder for the instruction 69FFFAD3 at address 0013BC70 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_69FFFAD3()
        {
            AssertCode("@@@", "69FFFAD3");
        }
        // Reko: a decoder for the instruction ECFF6089 at address 0016C822 has not been implemented. (Unimplemented '*scalar' when decoding FFEC8960)
        [Test]
        public void ThumbDis_ECFF6089()
        {
            AssertCode("@@@", "ECFF6089");
        }
        // Reko: a decoder for the instruction D4EFC664 at address 0011C160 has not been implemented. (Unimplemented '*scalar' when decoding EFD464C6)
        [Test]
        public void ThumbDis_D4EFC664()
        {
            AssertCode("@@@", "D4EFC664");
        }
        // Reko: a decoder for the instruction A2F9AD3E at address 0018CA12 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A2F9AD3E()
        {
            AssertCode("@@@", "A2F9AD3E");
        }
        // Reko: a decoder for the instruction 5AFD6AAC at address 0016D300 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 4AFC8038 at address 0016D34C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4AFC8038()
        {
            AssertCode("@@@", "4AFC8038");
        }
        // Reko: a decoder for the instruction 2FFD872C at address 001EC4C0 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2FFD872C()
        {
            AssertCode("@@@", "2FFD872C");
        }
        // Reko: a decoder for the instruction C2F97BF3 at address 001CE386 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T1' when decoding F9C2F37B)
        
        // Reko: a decoder for the instruction 0BFC03A8 at address 00112D76 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0BFC03A8()
        {
            AssertCode("@@@", "0BFC03A8");
        }
        // Reko: a decoder for the instruction C1F93257 at address 0014BEC4 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T2' when decoding F9C15732)
        [Test]
        public void ThumbDis_C1F93257()
        {
            AssertCode("@@@", "C1F93257");
        }
        // Reko: a decoder for the instruction 71EF33B3 at address 0018CB44 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_71EF33B3()
        {
            AssertCode("@@@", "71EF33B3");
        }
        // Reko: a decoder for the instruction 29EF1CE2 at address 0013D3E2 has not been implemented. (Unimplemented '*' when decoding EF29E21C)
        [Test]
        public void ThumbDis_29EF1CE2()
        {
            AssertCode("@@@", "29EF1CE2");
        }
        // Reko: a decoder for the instruction EDFCDCB9 at address 001EC60A has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EDFCDCB9()
        {
            AssertCode("@@@", "EDFCDCB9");
        }
        // Reko: a decoder for the instruction 12F95B8F at address 0018D04A has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_12F95B8F()
        {
            AssertCode("@@@", "12F95B8F");
        }
        // Reko: a decoder for the instruction 04EF93A3 at address 001ACDFC has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_04EF93A3()
        {
            AssertCode("@@@", "04EF93A3");
        }
        // Reko: a decoder for the instruction C4FC8F68 at address 0013D620 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C4FC8F68()
        {
            AssertCode("@@@", "C4FC8F68");
        }
        // Reko: a decoder for the instruction DEF309A6 at address 0011DC68 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_DEF309A6()
        {
            AssertCode("@@@", "DEF309A6");
        }
        // Reko: a decoder for the instruction EFF99942 at address 001CE7BC has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9EF4299)
        [Test]
        public void ThumbDis_EFF99942()
        {
            AssertCode("@@@", "EFF99942");
        }
        // Reko: a decoder for the instruction E2EF8A9D at address 0013D7DA has not been implemented. (Unimplemented '*integer' when decoding EFE29D8A)
        [Test]
        public void ThumbDis_E2EF8A9D()
        {
            AssertCode("@@@", "E2EF8A9D");
        }
        // Reko: a decoder for the instruction 4BF92212 at address 0016CF9E has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F94B1222)
        [Test]
        public void ThumbDis_4BF92212()
        {
            AssertCode("@@@", "4BF92212");
        }
        // Reko: a decoder for the instruction 27FC1BED at address 0014C2EA has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_27FC1BED()
        {
            AssertCode("@@@", "27FC1BED");
        }
        // Reko: a decoder for the instruction E4F93F5C at address 00132F9C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E4F93F5C()
        {
            AssertCode("@@@", "E4F93F5C");
        }
        // Reko: a decoder for the instruction 2EEF5DE3 at address 00133012 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2EEF5DE3()
        {
            AssertCode("@@@", "2EEF5DE3");
        }
        // Reko: a decoder for the instruction 7FFFC94C at address 0013D804 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_7FFFC94C()
        {
            AssertCode("@@@", "7FFFC94C");
        }
        // Reko: a decoder for the instruction EBEFEC6B at address 0011DF6A has not been implemented. (Unimplemented '*' when decoding EFEB6BEC)
        [Test]
        public void ThumbDis_EBEFEC6B()
        {
            AssertCode("@@@", "EBEFEC6B");
        }
        // Reko: a decoder for the instruction D5FDC808 at address 00113E36 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D5FDC808()
        {
            AssertCode("@@@", "D5FDC808");
        }
        // Reko: a decoder for the instruction C6FDBF9C at address 00113E70 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C6FDBF9C()
        {
            AssertCode("@@@", "C6FDBF9C");
        }
        // Reko: a decoder for the instruction 89FC74BC at address 00133126 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_89FC74BC()
        {
            AssertCode("@@@", "89FC74BC");
        }
        // Reko: a decoder for the instruction 64FE273C at address 001CEB74 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_64FE273C()
        {
            AssertCode("@@@", "64FE273C");
        }
        // Reko: a decoder for the instruction 39FEDB79 at address 0016DB96 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_39FEDB79()
        {
            AssertCode("@@@", "39FEDB79");
        }
        // Reko: a decoder for the instruction 08EFC043 at address 00112582 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_08EFC043()
        {
            AssertCode("@@@", "08EFC043");
        }
        // Reko: a decoder for the instruction 38FF343C at address 0018DB64 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_38FF343C()
        {
            AssertCode("@@@", "38FF343C");
        }
        // Reko: a decoder for the instruction EDFFE054 at address 0011E58A has not been implemented. (Unimplemented '*scalar' when decoding FFED54E0)
        [Test]
        public void ThumbDis_EDFFE054()
        {
            AssertCode("@@@", "EDFFE054");
        }
        // Reko: a decoder for the instruction 7CFF4F33 at address 0011E5A2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_7CFF4F33()
        {
            AssertCode("@@@", "7CFF4F33");
        }
        // Reko: a decoder for the instruction 0AFC2C0C at address 0011E692 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_0AFC2C0C()
        {
            AssertCode("@@@", "0AFC2C0C");
        }
        // Reko: a decoder for the instruction DEFF7FEB at address 00112ABC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_DEFF7FEB()
        {
            AssertCode("@@@", "DEFF7FEB");
        }
        // Reko: a decoder for the instruction E0EF9578 at address 00112CC6 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_E0EF9578()
        {
            AssertCode("@@@", "E0EF9578");
        }
        // Reko: a decoder for the instruction CBFA8B1C at address 00133480 has not been implemented. (crc32-crc32b)
        [Test]
        public void ThumbDis_CBFA8B1C()
        {
            AssertCode("@@@", "CBFA8B1C");
        }
        // Reko: a decoder for the instruction 62FD330C at address 0018DBF6 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_62FD330C()
        {
            AssertCode("@@@", "62FD330C");
        }
        // Reko: a decoder for the instruction C6FCC27D at address 001AD1F4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C6FCC27D()
        {
            AssertCode("@@@", "C6FCC27D");
        }
        // Reko: a decoder for the instruction 12EFD313 at address 0011392C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_12EFD313()
        {
            AssertCode("@@@", "12EFD313");
        }
        // Reko: a decoder for the instruction 08FFB5CF at address 0018DCD4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_08FFB5CF()
        {
            AssertCode("@@@", "08FFB5CF");
        }
        // Reko: a decoder for the instruction 5CFFCD2F at address 0011E748 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_5CFFCD2F()
        {
            AssertCode("@@@", "5CFFCD2F");
        }
        // Reko: a decoder for the instruction 97FE9B09 at address 001CEF0C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_97FE9B09()
        {
            AssertCode("@@@", "97FE9B09");
        }
        // Reko: a decoder for the instruction 22EF33F2 at address 0018DEDC has not been implemented. (Unimplemented '*' when decoding EF22F233)
        [Test]
        public void ThumbDis_22EF33F2()
        {
            AssertCode("@@@", "22EF33F2");
        }
        // Reko: a decoder for the instruction EFFFFC1D at address 001334C4 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_EFFFFC1D()
        {
            AssertCode("@@@", "EFFFFC1D");
        }
        // Reko: a decoder for the instruction A7F3E5AA at address 0016E16A has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A7F3E5AA()
        {
            AssertCode("@@@", "A7F3E5AA");
        }
        // Reko: a decoder for the instruction 10FE7F38 at address 0011E75E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_10FE7F38()
        {
            AssertCode("@@@", "10FE7F38");
        }
        // Reko: a decoder for the instruction C8F927D9 at address 0013DCAC has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9C8D927)
        [Test]
        public void ThumbDis_C8F927D9()
        {
            AssertCode("@@@", "C8F927D9");
        }
        // Reko: a decoder for the instruction ECF98085 at address 0018DF28 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9EC8580)
        [Test]
        public void ThumbDis_ECF98085()
        {
            AssertCode("@@@", "ECF98085");
        }
        // Reko: a decoder for the instruction 38FF2694 at address 0016E212 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_38FF2694()
        {
            AssertCode("@@@", "38FF2694");
        }
        // Reko: a decoder for the instruction 18F9918F at address 0011E78E has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_18F9918F()
        {
            AssertCode("@@@", "18F9918F");
        }
        // Reko: a decoder for the instruction CCFD5C8D at address 0016EFCE has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_CCFD5C8D()
        {
            AssertCode("@@@", "CCFD5C8D");
        }
        // Reko: a decoder for the instruction E7FC9589 at address 0015E3E4 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E7FC9589()
        {
            AssertCode("@@@", "E7FC9589");
        }
        // Reko: a decoder for the instruction 63FD5699 at address 0014E276 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_63FD5699()
        {
            AssertCode("@@@", "63FD5699");
        }
        // Reko: a decoder for the instruction 62FC0408 at address 0016EFEE has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_62FC0408()
        {
            AssertCode("@@@", "62FC0408");
        }
        // Reko: a decoder for the instruction EDEF53F8 at address 0013E69C has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_EDEF53F8()
        {
            AssertCode("@@@", "EDEF53F8");
        }
        // Reko: a decoder for the instruction BBFC8F2C at address 0015E62E has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BBFC8F2C()
        {
            AssertCode("@@@", "BBFC8F2C");
        }
        // Reko: a decoder for the instruction 87FA82DF at address 001CFF5E has not been implemented. (Unimplemented '*' when decoding FA87DF82)
        [Test]
        public void ThumbDis_87FA82DF()
        {
            AssertCode("@@@", "87FA82DF");
        }
        // Reko: a decoder for the instruction C2FD98C8 at address 0014DEEA has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C2FD98C8()
        {
            AssertCode("@@@", "C2FD98C8");
        }
        // Reko: a decoder for the instruction 80FDF47D at address 001ADD5A has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_80FDF47D()
        {
            AssertCode("@@@", "80FDF47D");
        }
        // Reko: a decoder for the instruction 9AFD3F18 at address 001ADE18 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_9AFD3F18()
        {
            AssertCode("@@@", "9AFD3F18");
        }
        // Reko: a decoder for the instruction A5FF6C6E at address 0013EEF2 has not been implemented. (Unimplemented '*' when decoding FFA56E6C)
        [Test]
        public void ThumbDis_A5FF6C6E()
        {
            AssertCode("@@@", "A5FF6C6E");
        }
        // Reko: a decoder for the instruction 7AEF307D at address 0018EF7E has not been implemented. (*vmls (floating point))
        [Test]
        public void ThumbDis_7AEF307D()
        {
            AssertCode("@@@", "7AEF307D");
        }
        // Reko: a decoder for the instruction 72FC89B8 at address 0017E0C6 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_72FC89B8()
        {
            AssertCode("@@@", "72FC89B8");
        }
        // Reko: a decoder for the instruction A4F93F89 at address 0015EAC2 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9A4893F)
        [Test]
        public void ThumbDis_A4F93F89()
        {
            AssertCode("@@@", "A4F93F89");
        }
        // Reko: a decoder for the instruction 01FD4EB9 at address 0015ECF2 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_01FD4EB9()
        {
            AssertCode("@@@", "01FD4EB9");
        }
        // Reko: a decoder for the instruction 5AFE9B19 at address 0014ECD6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_5AFE9B19()
        {
            AssertCode("@@@", "5AFE9B19");
        }
        // Reko: a decoder for the instruction A7F9EBA9 at address 001D0576 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9A7A9EB)
        [Test]
        public void ThumbDis_A7F9EBA9()
        {
            AssertCode("@@@", "A7F9EBA9");
        }
        // Reko: a decoder for the instruction ABEFB50B at address 0013EF18 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_ABEFB50B()
        {
            AssertCode("@@@", "ABEFB50B");
        }
        // Reko: a decoder for the instruction A6EFB7BB at address 0015ED26 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_A6EFB7BB()
        {
            AssertCode("@@@", "A6EFB7BB");
        }
        // Reko: a decoder for the instruction 83FE412D at address 0013F15A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_83FE412D()
        {
            AssertCode("@@@", "83FE412D");
        }
        // Reko: a decoder for the instruction 00FF6921 at address 0013F502 has not been implemented. (Unimplemented '*' when decoding FF002169)
        [Test]
        public void ThumbDis_00FF6921()
        {
            AssertCode("@@@", "00FF6921");
        }
        // Reko: a decoder for the instruction 93EFC232 at address 0017ECEC has not been implemented. (Unimplemented '*scalar' when decoding EF9332C2)
        [Test]
        public void ThumbDis_93EFC232()
        {
            AssertCode("@@@", "93EFC232");
        }
        // Reko: a decoder for the instruction A3FF9E48 at address 001EE3DA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_A3FF9E48()
        {
            AssertCode("@@@", "A3FF9E48");
        }
        // Reko: a decoder for the instruction A7F391AD at address 0015F5E2 has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_A7F391AD()
        {
            AssertCode("@@@", "A7F391AD");
        }
        // Reko: a decoder for the instruction AAEF81D9 at address 0016F71C has not been implemented. (Unimplemented '*integer' when decoding EFAAD981)

        // Reko: a decoder for the instruction CFFE7CEC at address 001EE44E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_CFFE7CEC()
        {
            AssertCode("@@@", "CFFE7CEC");
        }
        // Reko: a decoder for the instruction C8FF996D at address 0018F62A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_C8FF996D()
        {
            AssertCode("@@@", "C8FF996D");
        }
        // Reko: a decoder for the instruction 16FE833D at address 0014F826 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_16FE833D()
        {
            AssertCode("@@@", "16FE833D");
        }
        // Reko: a decoder for the instruction 4AFD8DCC at address 0017EF16 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4AFD8DCC()
        {
            AssertCode("@@@", "4AFD8DCC");
        }
        // Reko: a decoder for the instruction EBF9C3CA at address 0017EF36 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9EBCAC3)
        [Test]
        public void ThumbDis_EBF9C3CA()
        {
            AssertCode("@@@", "EBF9C3CA");
        }
        // Reko: a decoder for the instruction 18FF090C at address 0015F092 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_18FF090C()
        {
            AssertCode("@@@", "18FF090C");
        }
        // Reko: a decoder for the instruction 42EF2DF4 at address 001AEDF4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_42EF2DF4()
        {
            AssertCode("@@@", "42EF2DF4");
        }
        // Reko: a decoder for the instruction 80F9E83C at address 0019EC76 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_80F9E83C()
        {
            AssertCode("@@@", "80F9E83C");
        }
        // Reko: a decoder for the instruction 2EEF8493 at address 00190052 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_2EEF8493()
        {
            AssertCode("@@@", "2EEF8493");
        }
        // Reko: a decoder for the instruction 57FFAF5B at address 0015F67E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_57FFAF5B()
        {
            AssertCode("@@@", "57FFAF5B");
        }
        // Reko: a decoder for the instruction 1DEF01D1 at address 0017037E has not been implemented. (Unimplemented '*' when decoding EF1DD101)
        [Test]
        public void ThumbDis_1DEF01D1()
        {
            AssertCode("@@@", "1DEF01D1");
        }
        // Reko: a decoder for the instruction 3EFEB51D at address 001EE93E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3EFEB51D()
        {
            AssertCode("@@@", "3EFEB51D");
        }
        // Reko: a decoder for the instruction 74EFC401 at address 0014F930 has not been implemented. (Unimplemented '*' when decoding EF7401C4)
        [Test]
        public void ThumbDis_74EFC401()
        {
            AssertCode("@@@", "74EFC401");
        }
        // Reko: a decoder for the instruction E5EF687F at address 0017F050 has not been implemented. (Unimplemented '*' when decoding EFE57F68)
        [Test]
        public void ThumbDis_E5EF687F()
        {
            AssertCode("@@@", "E5EF687F");
        }
        // Reko: a decoder for the instruction E7EF6203 at address 001904B0 has not been implemented. (Unimplemented '*' when decoding EFE70362)
        [Test]
        public void ThumbDis_E7EF6203()
        {
            AssertCode("@@@", "E7EF6203");
        }
        // Reko: a decoder for the instruction 6FEEF12B at address 001EEA6C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
  
        // Reko: a decoder for the instruction E7EFC72F at address 001705A8 has not been implemented. (Unimplemented '*' when decoding EFE72FC7)
        [Test]
        public void ThumbDis_E7EFC72F()
        {
            AssertCode("@@@", "E7EFC72F");
        }
        // Reko: a decoder for the instruction A5FC2728 at address 001EEB46 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A5FC2728()
        {
            AssertCode("@@@", "A5FC2728");
        }
        // Reko: a decoder for the instruction E0FFC6B8 at address 0015F936 has not been implemented. (Unimplemented '*scalar' when decoding FFE0B8C6)
        [Test]
        public void ThumbDis_E0FFC6B8()
        {
            AssertCode("@@@", "E0FFC6B8");
        }
        // Reko: a decoder for the instruction 6EFE0BBD at address 0017F55A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6EFE0BBD()
        {
            AssertCode("@@@", "6EFE0BBD");
        }
        // Reko: a decoder for the instruction 05FDB868 at address 0017F804 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_05FDB868()
        {
            AssertCode("@@@", "05FDB868");
        }
        // Reko: a decoder for the instruction 25FE3FBD at address 0013EF4E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_25FE3FBD()
        {
            AssertCode("@@@", "25FE3FBD");
        }
        // Reko: a decoder for the instruction 2DFE7EDC at address 0015F9FE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_2DFE7EDC()
        {
            AssertCode("@@@", "2DFE7EDC");
        }
        // Reko: a decoder for the instruction DDFE451D at address 00150468 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_DDFE451D()
        {
            AssertCode("@@@", "DDFE451D");
        }
        // Reko: a decoder for the instruction 18F973CD at address 00170EFC has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_18F973CD()
        {
            AssertCode("@@@", "18F973CD");
        }
        // Reko: a decoder for the instruction 22FFD1C9 at address 001AFEC0 has not been implemented. (*vmul (integer and polynomial)
 
        // Reko: a decoder for the instruction CBFAA623 at address 00171068 has not been implemented. (crc32-crc32w)
        [Test]
        public void ThumbDis_CBFAA623()
        {
            AssertCode("@@@", "CBFAA623");
        }
        // Reko: a decoder for the instruction CBF9CF62 at address 001EF094 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9CB62CF)
        [Test]
        public void ThumbDis_CBF9CF62()
        {
            AssertCode("@@@", "CBF9CF62");
        }
        // Reko: a decoder for the instruction C3FD0469 at address 001FE16C has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_C3FD0469()
        {
            AssertCode("@@@", "C3FD0469");
        }
        // Reko: a decoder for the instruction E5FF66A9 at address 001AFF16 has not been implemented. (Unimplemented '*scalar' when decoding FFE5A966)
        [Test]
        public void ThumbDis_E5FF66A9()
        {
            AssertCode("@@@", "E5FF66A9");
        }
        // Reko: a decoder for the instruction AFFC260C at address 001710A4 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_AFFC260C()
        {
            AssertCode("@@@", "AFFC260C");
        }
        // Reko: a decoder for the instruction 67EFEB9C at address 001710AA has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)

        // Reko: a decoder for the instruction 84FCF4D8 at address 001509E6 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_84FCF4D8()
        {
            AssertCode("@@@", "84FCF4D8");
        }
        // Reko: a decoder for the instruction C4F9F9DF at address 0015FF58 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C4F9F9DF()
        {
            AssertCode("@@@", "C4F9F9DF");
        }
        // Reko: a decoder for the instruction 61FFB9E9 at address 001AF0F2 has not been implemented. (*vmul (integer and polynomial)
        // Reko: a decoder for the instruction DCFD7598 at address 00151A4E has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_DCFD7598()
        {
            AssertCode("@@@", "DCFD7598");
        }
        // Reko: a decoder for the instruction 1EEC4E8F at address 001710F6 has not been implemented. (Unimplemented '*' when decoding EC1E8F4E)
        [Test]
        public void ThumbDis_1EEC4E8F()
        {
            AssertCode("@@@", "1EEC4E8F");
        }
        // Reko: a decoder for the instruction B2FE48AD at address 001DFD90 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_B2FE48AD()
        {
            AssertCode("@@@", "B2FE48AD");
        }
        // Reko: a decoder for the instruction 58FE3FFD at address 001B07C0 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_58FE3FFD()
        {
            AssertCode("@@@", "58FE3FFD");
        }
        // Reko: a decoder for the instruction 26FC612D at address 001D3B88 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_26FC612D()
        {
            AssertCode("@@@", "26FC612D");
        }
        // Reko: a decoder for the instruction 56FFEB04 at address 001EFC02 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_56FFEB04()
        {
            AssertCode("@@@", "56FFEB04");
        }
        // Reko: a decoder for the instruction EEEF77B8 at address 001DFA28 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_EEEF77B8()
        {
            AssertCode("@@@", "EEEF77B8");
        }
        // Reko: a decoder for the instruction 84F92ACF at address 001925B2 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_84F92ACF()
        {
            AssertCode("@@@", "84F92ACF");
        }
        // Reko: a decoder for the instruction 76FDC318 at address 001925FE has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_76FDC318()
        {
            AssertCode("@@@", "76FDC318");
        }
        // Reko: a decoder for the instruction E3FC2108 at address 00192998 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E3FC2108()
        {
            AssertCode("@@@", "E3FC2108");
        }
        // Reko: a decoder for the instruction 0AFD3D28 at address 001FFDEC has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0AFD3D28()
        {
            AssertCode("@@@", "0AFD3D28");
        }
        // Reko: a decoder for the instruction 8FF956BF at address 00192B22 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8FF956BF()
        {
            AssertCode("@@@", "8FF956BF");
        }
        // Reko: a decoder for the instruction 12EF1C84 at address 001FFF42 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_12EF1C84()
        {
            AssertCode("@@@", "12EF1C84");
        }
        // Reko: a decoder for the instruction 08FFF1DB at address 001513FC has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_08FFF1DB()
        {
            AssertCode("@@@", "08FFF1DB");
        }
        // Reko: a decoder for the instruction 58F9F0CF at address 00192B2A has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_58F9F0CF()
        {
            AssertCode("@@@", "58F9F0CF");
        }
        // Reko: a decoder for the instruction AFF9BEC0 at address 001F064C has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9AFC0BE)
        [Test]
        public void ThumbDis_AFF9BEC0()
        {
            AssertCode("@@@", "AFF9BEC0");
        }
        // Reko: a decoder for the instruction 70FEB67C at address 001936E4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_70FEB67C()
        {
            AssertCode("@@@", "70FEB67C");
        }
        // Reko: a decoder for the instruction D5FCD599 at address 001F0F92 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D5FCD599()
        {
            AssertCode("@@@", "D5FCD599");
        }
        // Reko: a decoder for the instruction 30FC6D6D at address 001724A0 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_30FC6D6D()
        {
            AssertCode("@@@", "30FC6D6D");
        }
        // Reko: a decoder for the instruction E2FED1A8 at address 001B19F2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E2FED1A8()
        {
            AssertCode("@@@", "E2FED1A8");
        }
        // Reko: a decoder for the instruction 0EFD9A2D at address 001B1AB0 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_0EFD9A2D()
        {
            AssertCode("@@@", "0EFD9A2D");
        }
        // Reko: a decoder for the instruction 80FF5428 at address 001D4DF6 has not been implemented. (Unimplemented '*immediate - T3' when decoding FF802854)
        [Test]
        public void ThumbDis_80FF5428()
        {
            AssertCode("@@@", "80FF5428");
        }
        // Reko: a decoder for the instruction 2FFD4C6C at address 001927D8 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2FFD4C6C()
        {
            AssertCode("@@@", "2FFD4C6C");
        }
        // Reko: a decoder for the instruction CEF9DFE1 at address 001B1C7E has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9CEE1DF)
        [Test]
        public void ThumbDis_CEF9DFE1()
        {
            AssertCode("@@@", "CEF9DFE1");
        }
        // Reko: a decoder for the instruction 61FD3F78 at address 001D4E52 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_61FD3F78()
        {
            AssertCode("@@@", "61FD3F78");
        }
        // Reko: a decoder for the instruction 89FF1CAC at address 00192BB0 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_89FF1CAC()
        {
            AssertCode("@@@", "89FF1CAC");
        }
        // Reko: a decoder for the instruction 93EF78B8 at address 001D430C has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_93EF78B8()
        {
            AssertCode("@@@", "93EF78B8");
        }
        // Reko: a decoder for the instruction 8FEFCA5B at address 00154C78 has not been implemented. (Unimplemented '*' when decoding EF8F5BCA)
        [Test]
        public void ThumbDis_8FEFCA5B()
        {
            AssertCode("@@@", "8FEFCA5B");
        }
        // Reko: a decoder for the instruction DBFA8D3C at address 001D6896 has not been implemented. (crc32c-crc32cb)
        [Test]
        public void ThumbDis_DBFA8D3C()
        {
            AssertCode("@@@", "DBFA8D3C");
        }
        // Reko: a decoder for the instruction A9EFC993 at address 001F2B50 has not been implemented. (Unimplemented '*' when decoding EFA993C9)
        [Test]
        public void ThumbDis_A9EFC993()
        {
            AssertCode("@@@", "A9EFC993");
        }
        // Reko: a decoder for the instruction E1EC4EEA at address 001D6B64 has not been implemented. (Unimplemented '*' when decoding ECE1EA4E)
        [Test]
        public void ThumbDis_E1EC4EEA()
        {
            AssertCode("@@@", "E1EC4EEA");
        }
        // Reko: a decoder for the instruction 28FD7468 at address 001F2D10 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_28FD7468()
        {
            AssertCode("@@@", "28FD7468");
        }
        // Reko: a decoder for the instruction 4AFD30EC at address 00154EA4 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4AFD30EC()
        {
            AssertCode("@@@", "4AFD30EC");
        }
        // Reko: a decoder for the instruction AFFEB959 at address 00154EAA has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_AFFEB959()
        {
            AssertCode("@@@", "AFFEB959");
        }
        // Reko: a decoder for the instruction CBFC4108 at address 001D6B9A has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CBFC4108()
        {
            AssertCode("@@@", "CBFC4108");
        }
        // Reko: a decoder for the instruction 44FC31BC at address 00154EC8 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_44FC31BC()
        {
            AssertCode("@@@", "44FC31BC");
        }
        // Reko: a decoder for the instruction 4BFF3C04 at address 00155000 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_4BFF3C04()
        {
            AssertCode("@@@", "4BFF3C04");
        }
        // Reko: a decoder for the instruction E9EF3AF8 at address 00194944 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_E9EF3AF8()
        {
            AssertCode("@@@", "E9EF3AF8");
        }
        // Reko: a decoder for the instruction 64FDD5D9 at address 001F308A has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_64FDD5D9()
        {
            AssertCode("@@@", "64FDD5D9");
        }
        // Reko: a decoder for the instruction 3EFF97A9 at address 001F3092 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 55F97C6E at address 00155084 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_55F97C6E()
        {
            AssertCode("@@@", "55F97C6E");
        }
        // Reko: a decoder for the instruction 9DFEBECC at address 001F317C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_9DFEBECC()
        {
            AssertCode("@@@", "9DFEBECC");
        }
        // Reko: a decoder for the instruction E4FC6078 at address 00194A4C has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_E4FC6078()
        {
            AssertCode("@@@", "E4FC6078");
        }
        // Reko: a decoder for the instruction BBEEE69A at address 00155276 has not been implemented. (1011 - _HSD)
        [Test]
        public void ThumbDis_BBEEE69A()
        {
            AssertCode("@@@", "BBEEE69A");
        }
        // Reko: a decoder for the instruction 5EFFC893 at address 00194A80 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_5EFFC893()
        {
            AssertCode("@@@", "5EFFC893");
        }
        // Reko: a decoder for the instruction BDEEF779 at address 00155296 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
   
        // Reko: a decoder for the instruction EDFD77AD at address 001B329A has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_EDFD77AD()
        {
            AssertCode("@@@", "EDFD77AD");
        }
        // Reko: a decoder for the instruction B8FC523D at address 001B32DC has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B8FC523D()
        {
            AssertCode("@@@", "B8FC523D");
        }
        // Reko: a decoder for the instruction D0EFF094 at address 001B3372 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_D0EFF094()
        {
            AssertCode("@@@", "D0EFF094");
        }
        // Reko: a decoder for the instruction AAEC680A at address 00195260 has not been implemented. (Unimplemented '*' when decoding ECAA0A68)
        [Test]
        public void ThumbDis_AAEC680A()
        {
            AssertCode("@@@", "AAEC680A");
        }
        // Reko: a decoder for the instruction CBF3D2A2 at address 001D7C04 has not been implemented. (Unimplemented '*' when decoding F3CBA2D2)
        [Test]
        public void ThumbDis_CBF3D2A2()
        {
            AssertCode("@@@", "CBF3D2A2");
        }
        // Reko: a decoder for the instruction 05FF149F at address 001D7EA2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_05FF149F()
        {
            AssertCode("@@@", "05FF149F");
        }
        // Reko: a decoder for the instruction 2DFF777E at address 0015600A has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=1 size=1x o1=1)
        [Test]
        public void ThumbDis_2DFF777E()
        {
            AssertCode("@@@", "2DFF777E");
        }
        // Reko: a decoder for the instruction 54F9661E at address 001B3BD4 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_54F9661E()
        {
            AssertCode("@@@", "54F9661E");
        }
        // Reko: a decoder for the instruction DAEF9BE4 at address 0015609E has not been implemented. (U=0)
        [Test]
        public void ThumbDis_DAEF9BE4()
        {
            AssertCode("@@@", "DAEF9BE4");
        }
        // Reko: a decoder for the instruction 23EEBC69 at address 001567CA has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_23EEBC69()
        {
            AssertCode("@@@", "23EEBC69");
        }
        // Reko: a decoder for the instruction 5DF9C33F at address 00195A94 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_5DF9C33F()
        {
            AssertCode("@@@", "5DF9C33F");
        }
        // Reko: a decoder for the instruction 79FCDA8D at address 00195AF2 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_79FCDA8D()
        {
            AssertCode("@@@", "79FCDA8D");
        }
        // Reko: a decoder for the instruction 02EF1C9F at address 001755DA has not been implemented. (Unimplemented '*' when decoding EF029F1C)
        [Test]
        public void ThumbDis_02EF1C9F()
        {
            AssertCode("@@@", "02EF1C9F");
        }
        // Reko: a decoder for the instruction C7FC14AD at address 00175602 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C7FC14AD()
        {
            AssertCode("@@@", "C7FC14AD");
        }
        // Reko: a decoder for the instruction F5FFA437 at address 001B3EBC has not been implemented. (Unimplemented '*' when decoding FFF537A4)
        [Test]
        public void ThumbDis_F5FFA437()
        {
            AssertCode("@@@", "F5FFA437");
        }
        // Reko: a decoder for the instruction EEFEA51C at address 001B3F66 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EEFEA51C()
        {
            AssertCode("@@@", "EEFEA51C");
        }
        // Reko: a decoder for the instruction 4DFC5B3D at address 00195F6E has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4DFC5B3D()
        {
            AssertCode("@@@", "4DFC5B3D");
        }
        // Reko: a decoder for the instruction 8DEE10AB at address 001B3174 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_8DEE10AB()
        {
            AssertCode("@@@", "8DEE10AB");
        }
        // Reko: a decoder for the instruction A6FE5E7C at address 001D8A2A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A6FE5E7C()
        {
            AssertCode("@@@", "A6FE5E7C");
        }
        // Reko: a decoder for the instruction A0F9EC20 at address 001D8A4C has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9A020EC)
        [Test]
        public void ThumbDis_A0F9EC20()
        {
            AssertCode("@@@", "A0F9EC20");
        }
        // Reko: a decoder for the instruction B2FDB559 at address 001B5B42 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B2FDB559()
        {
            AssertCode("@@@", "B2FDB559");
        }
        // Reko: a decoder for the instruction 68EF6383 at address 00158324 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_68EF6383()
        {
            AssertCode("@@@", "68EF6383");
        }
        // Reko: a decoder for the instruction 3DEF5101 at address 0015839E has not been implemented. (Unimplemented '*register' when decoding EF3D0151)
        [Test]
        public void ThumbDis_3DEF5101()
        {
            AssertCode("@@@", "3DEF5101");
        }
        // Reko: a decoder for the instruction A5F90DE9 at address 00177EFC has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9A5E90D)
        [Test]
        public void ThumbDis_A5F90DE9()
        {
            AssertCode("@@@", "A5F90DE9");
        }
        // Reko: a decoder for the instruction 5BF9D83E at address 00177F3E has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_5BF9D83E()
        {
            AssertCode("@@@", "5BF9D83E");
        }
        // Reko: a decoder for the instruction 14EFAA6B at address 00197070 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_14EFAA6B()
        {
            AssertCode("@@@", "14EFAA6B");
        }
        // Reko: a decoder for the instruction ECEFE02E at address 001B6592 has not been implemented. (Unimplemented '*' when decoding EFEC2EE0)
        [Test]
        public void ThumbDis_ECEFE02E()
        {
            AssertCode("@@@", "ECEFE02E");
        }
        // Reko: a decoder for the instruction E3FE9788 at address 00158EAE has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E3FE9788()
        {
            AssertCode("@@@", "E3FE9788");
        }
        // Reko: a decoder for the instruction 29EF9A73 at address 00177EC4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_29EF9A73()
        {
            AssertCode("@@@", "29EF9A73");
        }
        // Reko: a decoder for the instruction D9FF496D at address 001DC6C6 has not been implemented. (Unimplemented '*' when decoding FFD96D49)
        [Test]
        public void ThumbDis_D9FF496D()
        {
            AssertCode("@@@", "D9FF496D");
        }
        // Reko: a decoder for the instruction 14F983CF at address 001DC71A has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_14F983CF()
        {
            AssertCode("@@@", "14F983CF");
        }
        // Reko: a decoder for the instruction E9F9FD8A at address 001F6336 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T3' when decoding F9E98AFD)
        [Test]
        public void ThumbDis_E9F9FD8A()
        {
            AssertCode("@@@", "E9F9FD8A");
        }
        // Reko: a decoder for the instruction 8EF9972D at address 001F638E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8EF9972D()
        {
            AssertCode("@@@", "8EF9972D");
        }
        // Reko: a decoder for the instruction D9F3E78A at address 001B89E2 has not been implemented. (ExceptionReturn)
        [Test]
        public void ThumbDis_D9F3E78A()
        {
            AssertCode("@@@", "D9F3E78A");
        }
        // Reko: a decoder for the instruction E8FEFCD8 at address 001B8B10 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E8FEFCD8()
        {
            AssertCode("@@@", "E8FEFCD8");
        }
        // Reko: a decoder for the instruction 87FF43A5 at address 001F665C has not been implemented. (Unimplemented '*scalar' when decoding FF87A543)
        [Test]
        public void ThumbDis_87FF43A5()
        {
            AssertCode("@@@", "87FF43A5");
        }
        // Reko: a decoder for the instruction 45FFCD34 at address 001DDC2E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_45FFCD34()
        {
            AssertCode("@@@", "45FFCD34");
        }
        // Reko: a decoder for the instruction 15FCF6CC at address 0015A422 has not been implemented. (op1:op2=0b0001)
    
        // Reko: a decoder for the instruction C0F9A6BF at address 001DDD64 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C0F9A6BF()
        {
            AssertCode("@@@", "C0F9A6BF");
        }
        // Reko: a decoder for the instruction A0FC518D at address 001F6F82 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A0FC518D()
        {
            AssertCode("@@@", "A0FC518D");
        }
        // Reko: a decoder for the instruction 71FD2038 at address 00179FB6 has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_71FD2038()
        {
            AssertCode("@@@", "71FD2038");
        }
        // Reko: a decoder for the instruction 78FF78FB at address 00179FD0 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_78FF78FB()
        {
            AssertCode("@@@", "78FF78FB");
        }
        // Reko: a decoder for the instruction 00FD9A88 at address 00198FD4 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_00FD9A88()
        {
            AssertCode("@@@", "00FD9A88");
        }
        // Reko: a decoder for the instruction 1AFF84EB at address 001B92EE has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_1AFF84EB()
        {
            AssertCode("@@@", "1AFF84EB");
        }
        // Reko: a decoder for the instruction 90FE4E9C at address 001F7E4C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_90FE4E9C()
        {
            AssertCode("@@@", "90FE4E9C");
        }
        // Reko: a decoder for the instruction 13F9A0FD at address 001B9346 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_13F9A0FD()
        {
            AssertCode("@@@", "13F9A0FD");
        }
        // Reko: a decoder for the instruction 47EE3DE9 at address 0015A4BE has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_47EE3DE9()
        {
            AssertCode("@@@", "47EE3DE9");
        }
        // Reko: a decoder for the instruction D1E8F075 at address 0017A8D6 has not been implemented. (Unimplemented '*' when decoding E8D175F0)
        [Test]
        public void ThumbDis_D1E8F075()
        {
            AssertCode("@@@", "D1E8F075");
        }
        // Reko: a decoder for the instruction A2FA86D4 at address 0015AB22 has not been implemented. (Unimplemented '*' when decoding FAA2D486)
        [Test]
        public void ThumbDis_A2FA86D4()
        {
            AssertCode("@@@", "A2FA86D4");
        }
        // Reko: a decoder for the instruction A6EFD41B at address 001B94EE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_A6EFD41B()
        {
            AssertCode("@@@", "A6EFD41B");
        }
        // Reko: a decoder for the instruction B4FD744C at address 0017A940 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_B4FD744C()
        {
            AssertCode("@@@", "B4FD744C");
        }
        // Reko: a decoder for the instruction 0EFEF2BC at address 001B9564 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_0EFEF2BC()
        {
            AssertCode("@@@", "0EFEF2BC");
        }
        // Reko: a decoder for the instruction 60FFD079 at address 0017BFBC has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction F7F38EA7 at address 0017BFD0 has not been implemented. (Unimplemented '*register' when decoding F3F7A78E)
        [Test]
        public void ThumbDis_F7F38EA7()
        {
            AssertCode("@@@", "F7F38EA7");
        }
        // Reko: a decoder for the instruction 7BEF55A3 at address 001F95E2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_7BEF55A3()
        {
            AssertCode("@@@", "7BEF55A3");
        }
        // Reko: a decoder for the instruction 8DEF4BE9 at address 001F9602 has not been implemented. (Unimplemented '*scalar' when decoding EF8DE94B)
        [Test]
        public void ThumbDis_8DEF4BE9()
        {
            AssertCode("@@@", "8DEF4BE9");
        }
        // Reko: a decoder for the instruction 81FA80C1 at address 001BAD98 has not been implemented. (Unimplemented '*' when decoding FA81C180)
        [Test]
        public void ThumbDis_81FA80C1()
        {
            AssertCode("@@@", "81FA80C1");
        }
        // Reko: a decoder for the instruction D3FC5A4C at address 001F987A has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D3FC5A4C()
        {
            AssertCode("@@@", "D3FC5A4C");
        }
        // Reko: a decoder for the instruction 88FC7199 at address 0019AB08 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_88FC7199()
        {
            AssertCode("@@@", "88FC7199");
        }
        // Reko: a decoder for the instruction EAEF3F6C at address 0017DB46 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_EAEF3F6C()
        {
            AssertCode("@@@", "EAEF3F6C");
        }
        // Reko: a decoder for the instruction A0EFCD95 at address 001BB978 has not been implemented. (Unimplemented '*scalar' when decoding EFA095CD)
        [Test]
        public void ThumbDis_A0EFCD95()
        {
            AssertCode("@@@", "A0EFCD95");
        }
        // Reko: a decoder for the instruction A7EF4C7D at address 0015DA9E has not been implemented. (Unimplemented '*' when decoding EFA77D4C)
        [Test]
        public void ThumbDis_A7EF4C7D()
        {
            AssertCode("@@@", "A7EF4C7D");
        }
        // Reko: a decoder for the instruction 4BFFAFBC at address 0017DB8C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_4BFFAFBC()
        {
            AssertCode("@@@", "4BFFAFBC");
        }
        // Reko: a decoder for the instruction 1EFEF59C at address 001BB9A2 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_1EFEF59C()
        {
            AssertCode("@@@", "1EFEF59C");
        }
        // Reko: a decoder for the instruction 3CEF52D3 at address 001FAF4A has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_3CEF52D3()
        {
            AssertCode("@@@", "3CEF52D3");
        }
        // Reko: a decoder for the instruction ADFD647D at address 0017D0F6 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_ADFD647D()
        {
            AssertCode("@@@", "ADFD647D");
        }
        // Reko: a decoder for the instruction E2F99DFC at address 0017DE54 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E2F99DFC()
        {
            AssertCode("@@@", "E2F99DFC");
        }
        // Reko: a decoder for the instruction 8DEFEFAB at address 001BC01C has not been implemented. (Unimplemented '*' when decoding EF8DABEF)
        [Test]
        public void ThumbDis_8DEFEFAB()
        {
            AssertCode("@@@", "8DEFEFAB");
        }
        // Reko: a decoder for the instruction E2EEB47B at address 0019DA42 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_E2EEB47B()
        {
            AssertCode("@@@", "E2EEB47B");
        }
        // Reko: a decoder for the instruction 83FF9E38 at address 0019DA48 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_83FF9E38()
        {
            AssertCode("@@@", "83FF9E38");
        }
        // Reko: a decoder for the instruction 5FFEA36D at address 001BC558 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_5FFEA36D()
        {
            AssertCode("@@@", "5FFEA36D");
        }
        // Reko: a decoder for the instruction E6F95C4F at address 00140846 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E6F95C4F()
        {
            AssertCode("@@@", "E6F95C4F");
        }
        // Reko: a decoder for the instruction 69FD280C at address 00180A22 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_69FD280C()
        {
            AssertCode("@@@", "69FD280C");
        }
        // Reko: a decoder for the instruction E8F944FF at address 00180A66 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E8F944FF()
        {
            AssertCode("@@@", "E8F944FF");
        }
        // Reko: a decoder for the instruction 1FFDA43C at address 00120B4E has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction B7EE3299 at address 00180C30 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 2CFF3AA4 at address 00160BC8 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_2CFF3AA4()
        {
            AssertCode("@@@", "2CFF3AA4");
        }
        // Reko: a decoder for the instruction FEFFC2E5 at address 001813E4 has not been implemented. (Unimplemented '*' when decoding FFFEE5C2)
        [Test]
        public void ThumbDis_FEFFC2E5()
        {
            AssertCode("@@@", "FEFFC2E5");
        }
        // Reko: a decoder for the instruction 78F9EFAF at address 0014124A has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_78F9EFAF()
        {
            AssertCode("@@@", "78F9EFAF");
        }
        // Reko: a decoder for the instruction D6FD012D at address 001E135E has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D6FD012D()
        {
            AssertCode("@@@", "D6FD012D");
        }
        // Reko: a decoder for the instruction A1F907E1 at address 0016113A has not been implemented. (Unimplemented 'single 2-element structure from one lane - T1' when decoding F9A1E107)
        [Test]
        public void ThumbDis_A1F907E1()
        {
            AssertCode("@@@", "A1F907E1");
        }
        // Reko: a decoder for the instruction 83F90B6D at address 0016121A has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_83F90B6D()
        {
            AssertCode("@@@", "83F90B6D");
        }
        // Reko: a decoder for the instruction F1F72F87 at address 0016133A has not been implemented. (Unimplemented '*' when decoding F7F1872F)
        [Test]
        public void ThumbDis_F1F72F87()
        {
            AssertCode("@@@", "F1F72F87");
        }
        // Reko: a decoder for the instruction 4DFD28A9 at address 0016168C has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4DFD28A9()
        {
            AssertCode("@@@", "4DFD28A9");
        }
        // Reko: a decoder for the instruction DEEF6E5C at address 00161712 has not been implemented. (Unimplemented '*' when decoding EFDE5C6E)
        [Test]
        public void ThumbDis_DEEF6E5C()
        {
            AssertCode("@@@", "DEEF6E5C");
        }
        // Reko: a decoder for the instruction 45EFA684 at address 00141D7E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_45EFA684()
        {
            AssertCode("@@@", "45EFA684");
        }
        // Reko: a decoder for the instruction D9FE2A1D at address 001E13D6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D9FE2A1D()
        {
            AssertCode("@@@", "D9FE2A1D");
        }
        // Reko: a decoder for the instruction CCFFBBAB at address 001E13DA has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_CCFFBBAB()
        {
            AssertCode("@@@", "CCFFBBAB");
        }
        // Reko: a decoder for the instruction E2EF6D92 at address 001E13E2 has not been implemented. (Unimplemented '*scalar' when decoding EFE2926D)
        [Test]
        public void ThumbDis_E2EF6D92()
        {
            AssertCode("@@@", "E2EF6D92");
        }
        // Reko: a decoder for the instruction 9FEC289F at address 00161C80 has not been implemented. (load PC)
        [Test]
        public void ThumbDis_9FEC289F()
        {
            AssertCode("@@@", "9FEC289F");
        }
        // Reko: a decoder for the instruction 56EEFEC9 at address 001A1328 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_56EEFEC9()
        {
            AssertCode("@@@", "56EEFEC9");
        }
        // Reko: a decoder for the instruction 67FD97A8 at address 001829A4 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_67FD97A8()
        {
            AssertCode("@@@", "67FD97A8");
        }
        // Reko: a decoder for the instruction 30EFB631 at address 00161D4A has not been implemented. (Unimplemented '*register' when decoding EF3031B6)
        [Test]
        public void ThumbDis_30EFB631()
        {
            AssertCode("@@@", "30EFB631");
        }
        // Reko: a decoder for the instruction 8AFE7ED8 at address 001A14B8 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8AFE7ED8()
        {
            AssertCode("@@@", "8AFE7ED8");
        }
        // Reko: a decoder for the instruction 57FF5AB9 at address 001429B8 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 5DFF97CB at address 001A17EC has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_5DFF97CB()
        {
            AssertCode("@@@", "5DFF97CB");
        }
        // Reko: a decoder for the instruction 29FF79EF at address 001829D6 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_29FF79EF()
        {
            AssertCode("@@@", "29FF79EF");
        }
        // Reko: a decoder for the instruction BDFD7B9C at address 001E2CB2 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_BDFD7B9C()
        {
            AssertCode("@@@", "BDFD7B9C");
        }
        // Reko: a decoder for the instruction 4DFC9EED at address 001C269C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4DFC9EED()
        {
            AssertCode("@@@", "4DFC9EED");
        }
        // Reko: a decoder for the instruction 9DF34687 at address 001227FC has not been implemented. (Unimplemented '*register' when decoding F39D8746)
        [Test]
        public void ThumbDis_9DF34687()
        {
            AssertCode("@@@", "9DF34687");
        }
        // Reko: a decoder for the instruction 15EED29B at address 00122858 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_15EED29B()
        {
            AssertCode("@@@", "15EED29B");
        }
        // Reko: a decoder for the instruction A2FE5348 at address 001E122C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A2FE5348()
        {
            AssertCode("@@@", "A2FE5348");
        }
        // Reko: a decoder for the instruction CFF38786 at address 0016336E has not been implemented. (Unimplemented '*' when decoding F3CF8687)
        [Test]
        public void ThumbDis_CFF38786()
        {
            AssertCode("@@@", "CFF38786");
        }
        // Reko: a decoder for the instruction F5FE758D at address 0012302A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_F5FE758D()
        {
            AssertCode("@@@", "F5FE758D");
        }
        // Reko: a decoder for the instruction 8FF97C02 at address 001E15C8 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F98F027C)
        [Test]
        public void ThumbDis_8FF97C02()
        {
            AssertCode("@@@", "8FF97C02");
        }
        // Reko: a decoder for the instruction 55EF8113 at address 00163376 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_55EF8113()
        {
            AssertCode("@@@", "55EF8113");
        }
        // Reko: a decoder for the instruction 5EEE59B9 at address 00182BDE has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_5EEE59B9()
        {
            AssertCode("@@@", "5EEE59B9");
        }
        // Reko: a decoder for the instruction DEFFF2DD at address 0018345E has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_DEFFF2DD()
        {
            AssertCode("@@@", "DEFFF2DD");
        }
        // Reko: a decoder for the instruction C6FC689C at address 001839A8 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C6FC689C()
        {
            AssertCode("@@@", "C6FC689C");
        }
        // Reko: a decoder for the instruction 82FCC3FC at address 001A2A76 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_82FCC3FC()
        {
            AssertCode("@@@", "82FCC3FC");
        }
        // Reko: a decoder for the instruction E5F9AD14 at address 001A2F0A has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9E514AD)
        [Test]
        public void ThumbDis_E5F9AD14()
        {
            AssertCode("@@@", "E5F9AD14");
        }
        // Reko: a decoder for the instruction 1EEBFCCF at address 00183A6E has not been implemented. (Unimplemented '*register' when decoding EB1ECFFC)
   
        // Reko: a decoder for the instruction 7BEF8A2C at address 001E3D18 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_7BEF8A2C()
        {
            AssertCode("@@@", "7BEF8A2C");
        }
        // Reko: a decoder for the instruction CCF3E6A9 at address 001E43E8 has not been implemented. (Unimplemented '*' when decoding F3CCA9E6)
        [Test]
        public void ThumbDis_CCF3E6A9()
        {
            AssertCode("@@@", "CCF3E6A9");
        }
        // Reko: a decoder for the instruction CBF9CCFD at address 001E4A4E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CBF9CCFD()
        {
            AssertCode("@@@", "CBF9CCFD");
        }
        // Reko: a decoder for the instruction 4EEFAA44 at address 0010396E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_4EEFAA44()
        {
            AssertCode("@@@", "4EEFAA44");
        }
        // Reko: a decoder for the instruction C4EF33AB at address 00145114 has not been implemented. (Unimplemented '*immediate - T2' when decoding EFC4AB33)
        [Test]
        public void ThumbDis_C4EF33AB()
        {
            AssertCode("@@@", "C4EF33AB");
        }
        // Reko: a decoder for the instruction 60EF0183 at address 001E50E4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_60EF0183()
        {
            AssertCode("@@@", "60EF0183");
        }
        // Reko: a decoder for the instruction 14EFF8A9 at address 0014513A has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_14EFF8A9()
        {
            AssertCode("@@@", "14EFF8A9");
        }
        // Reko: a decoder for the instruction 9AFD0028 at address 00145176 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_9AFD0028()
        {
            AssertCode("@@@", "9AFD0028");
        }
        // Reko: a decoder for the instruction E8EEFE69 at address 00124C0A has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction ADF962CF at address 001A4CFE has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_ADF962CF()
        {
            AssertCode("@@@", "ADF962CF");
        }
        // Reko: a decoder for the instruction 93EF08E9 at address 00185D54 has not been implemented. (Unimplemented '*integer' when decoding EF93E908)
   
        // Reko: a decoder for the instruction D3EF9544 at address 001C55B8 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_D3EF9544()
        {
            AssertCode("@@@", "D3EF9544");
        }
        // Reko: a decoder for the instruction 1CFF3724 at address 00165B18 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_1CFF3724()
        {
            AssertCode("@@@", "1CFF3724");
        }
        // Reko: a decoder for the instruction 89FE892D at address 00125C50 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_89FE892D()
        {
            AssertCode("@@@", "89FE892D");
        }
        // Reko: a decoder for the instruction DAEF7074 at address 00186008 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_DAEF7074()
        {
            AssertCode("@@@", "DAEF7074");
        }
        // Reko: a decoder for the instruction CFF37585 at address 00125DCA has not been implemented. (Unimplemented '*' when decoding F3CF8575)
        [Test]
        public void ThumbDis_CFF37585()
        {
            AssertCode("@@@", "CFF37585");
        }
        // Reko: a decoder for the instruction 8AEFDE3D at address 001E6678 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_8AEFDE3D()
        {
            AssertCode("@@@", "8AEFDE3D");
        }
        // Reko: a decoder for the instruction 9CEE5979 at address 001A5C5C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 54FEDA28 at address 00105A64 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_54FEDA28()
        {
            AssertCode("@@@", "54FEDA28");
        }
        // Reko: a decoder for the instruction 6FFD980C at address 00105A72 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_6FFD980C()
        {
            AssertCode("@@@", "6FFD980C");
        }
        // Reko: a decoder for the instruction 13EBC40F at address 00166080 has not been implemented. (Unimplemented '*register' when decoding EB130FC4)
        [Test]
        public void ThumbDis_13EBC40F()
        {
            AssertCode("@@@", "13EBC40F");
        }
        // Reko: a decoder for the instruction 40EE31EB at address 001C5F24 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_40EE31EB()
        {
            AssertCode("@@@", "40EE31EB");
        }
        // Reko: a decoder for the instruction 38EF31C3 at address 0018630C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_38EF31C3()
        {
            AssertCode("@@@", "38EF31C3");
        }
        // Reko: a decoder for the instruction 6FFCDAA9 at address 00166570 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6FFCDAA9()
        {
            AssertCode("@@@", "6FFCDAA9");
        }
        // Reko: a decoder for the instruction F5FD1FAD at address 00125B30 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_F5FD1FAD()
        {
            AssertCode("@@@", "F5FD1FAD");
        }
        // Reko: a decoder for the instruction 53FE1B8D at address 001666F4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_53FE1B8D()
        {
            AssertCode("@@@", "53FE1B8D");
        }
        // Reko: a decoder for the instruction EAFE3629 at address 00146978 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EAFE3629()
        {
            AssertCode("@@@", "EAFE3629");
        }
        // Reko: a decoder for the instruction 71FD6A08 at address 00105ABC has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_71FD6A08()
        {
            AssertCode("@@@", "71FD6A08");
        }
        // Reko: a decoder for the instruction 27FDDFA8 at address 001A6B00 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_27FDDFA8()
        {
            AssertCode("@@@", "27FDDFA8");
        }
        // Reko: a decoder for the instruction 10F9D4ED at address 001A6B2E has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_10F9D4ED()
        {
            AssertCode("@@@", "10F9D4ED");
        }
        // Reko: a decoder for the instruction D7FC0F29 at address 001E78DC has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D7FC0F29()
        {
            AssertCode("@@@", "D7FC0F29");
        }
        // Reko: a decoder for the instruction ECECB59B at address 001C7552 has not been implemented. (Unimplemented '*' when decoding ECEC9BB5)
        [Test]
        public void ThumbDis_ECECB59B()
        {
            AssertCode("@@@", "ECECB59B");
        }
        // Reko: a decoder for the instruction 15EFBE52 at address 001E81B2 has not been implemented. (Unimplemented '*' when decoding EF1552BE)
        [Test]
        public void ThumbDis_15EFBE52()
        {
            AssertCode("@@@", "15EFBE52");
        }
        // Reko: a decoder for the instruction 20FF8454 at address 00167100 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_20FF8454()
        {
            AssertCode("@@@", "20FF8454");
        }
        // Reko: a decoder for the instruction 02FF768F at address 001265FA has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_02FF768F()
        {
            AssertCode("@@@", "02FF768F");
        }
        // Reko: a decoder for the instruction D5FAAD3B at address 001873D4 has not been implemented. (crc32c-crc32cw)
        [Test]
        public void ThumbDis_D5FAAD3B()
        {
            AssertCode("@@@", "D5FAAD3B");
        }
        // Reko: a decoder for the instruction 10EF6791 at address 001E8C4E has not been implemented. (Unimplemented '*' when decoding EF109167)
        [Test]
        public void ThumbDis_10EF6791()
        {
            AssertCode("@@@", "10EF6791");
        }
        // Reko: a decoder for the instruction E7F9584E at address 0014797A has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E7F9584E()
        {
            AssertCode("@@@", "E7F9584E");
        }
        // Reko: a decoder for the instruction 15EFF78B at address 00126E76 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_15EFF78B()
        {
            AssertCode("@@@", "15EFF78B");
        }
        // Reko: a decoder for the instruction CAFA890B at address 001C80E2 has not been implemented. (crc32-crc32b)
        [Test]
        public void ThumbDis_CAFA890B()
        {
            AssertCode("@@@", "CAFA890B");
        }
        // Reko: a decoder for the instruction 04FF5BC4 at address 00127330 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_04FF5BC4()
        {
            AssertCode("@@@", "04FF5BC4");
        }
        // Reko: a decoder for the instruction C2F974AB at address 00106858 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C2AB74)
        [Test]
        public void ThumbDis_C2F974AB()
        {
            AssertCode("@@@", "C2F974AB");
        }
        // Reko: a decoder for the instruction 7DFFA024 at address 00187648 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_7DFFA024()
        {
            AssertCode("@@@", "7DFFA024");
        }
        // Reko: a decoder for the instruction 83FE991C at address 001A6802 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_83FE991C()
        {
            AssertCode("@@@", "83FE991C");
        }
        // Reko: a decoder for the instruction 68FF08B3 at address 0018775E has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
 
        // Reko: a decoder for the instruction FBFE3948 at address 00187846 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_FBFE3948()
        {
            AssertCode("@@@", "FBFE3948");
        }
        // Reko: a decoder for the instruction F2F7F687 at address 00166A32 has not been implemented. (Unimplemented '*' when decoding F7F287F6)
        [Test]
        public void ThumbDis_F2F7F687()
        {
            AssertCode("@@@", "F2F7F687");
        }
        // Reko: a decoder for the instruction 92FFB038 at address 001068A2 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_92FFB038()
        {
            AssertCode("@@@", "92FFB038");
        }
        // Reko: a decoder for the instruction 4BFD635D at address 001C82DC has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4BFD635D()
        {
            AssertCode("@@@", "4BFD635D");
        }
        // Reko: a decoder for the instruction FAFD8678 at address 001E9644 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FAFD8678()
        {
            AssertCode("@@@", "FAFD8678");
        }
        // Reko: a decoder for the instruction 4BFF124B at address 001A7B7E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_4BFF124B()
        {
            AssertCode("@@@", "4BFF124B");
        }
        // Reko: a decoder for the instruction 9BEF6929 at address 001EA164 has not been implemented. (Unimplemented '*scalar' when decoding EF9B2969)
        [Test]
        public void ThumbDis_9BEF6929()
        {
            AssertCode("@@@", "9BEF6929");
        }
        // Reko: a decoder for the instruction 02FF318C at address 001EA1C4 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_02FF318C()
        {
            AssertCode("@@@", "02FF318C");
        }
        // Reko: a decoder for the instruction 77EFC3A3 at address 001073D2 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_77EFC3A3()
        {
            AssertCode("@@@", "77EFC3A3");
        }
        // Reko: a decoder for the instruction 69FCBC68 at address 00167AD4 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_69FCBC68()
        {
            AssertCode("@@@", "69FCBC68");
        }
        // Reko: a decoder for the instruction 2AEF8EDB at address 001A8948 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_2AEF8EDB()
        {
            AssertCode("@@@", "2AEF8EDB");
        }
        // Reko: a decoder for the instruction C0FFDCF8 at address 001A8CD8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_C0FFDCF8()
        {
            AssertCode("@@@", "C0FFDCF8");
        }
        // Reko: a decoder for the instruction E4F915F4 at address 001C9472 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9E4F415)
        [Test]
        public void ThumbDis_E4F915F4()
        {
            AssertCode("@@@", "E4F915F4");
        }
        // Reko: a decoder for the instruction 94F909FE at address 001081A4 has not been implemented. (PLI)
        [Test]
        public void ThumbDis_94F909FE()
        {
            AssertCode("@@@", "94F909FE");
        }
        // Reko: a decoder for the instruction 25EFDE73 at address 00188F20 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_25EFDE73()
        {
            AssertCode("@@@", "25EFDE73");
        }
        // Reko: a decoder for the instruction D9E8F0CB at address 0016862A has not been implemented. (Unimplemented '*' when decoding E8D9CBF0)
        [Test]
        public void ThumbDis_D9E8F0CB()
        {
            AssertCode("@@@", "D9E8F0CB");
        }
        // Reko: a decoder for the instruction 4DFEE8DD at address 0016863C has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_4DFEE8DD()
        {
            AssertCode("@@@", "4DFEE8DD");
        }
        // Reko: a decoder for the instruction 12FF14B9 at address 001EB760 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction AAFCA708 at address 001EB788 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_AAFCA708()
        {
            AssertCode("@@@", "AAFCA708");
        }
        // Reko: a decoder for the instruction AEF951FD at address 001C9CE0 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_AEF951FD()
        {
            AssertCode("@@@", "AEF951FD");
        }
        // Reko: a decoder for the instruction AEF95A54 at address 00168892 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9AE545A)
        [Test]
        public void ThumbDis_AEF95A54()
        {
            AssertCode("@@@", "AEF95A54");
        }
        // Reko: a decoder for the instruction D1FE36A8 at address 0016889A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_D1FE36A8()
        {
            AssertCode("@@@", "D1FE36A8");
        }
        // Reko: a decoder for the instruction 07FFD20E at address 00128A0A has not been implemented. (Unimplemented '*' when decoding FF070ED2)
        [Test]
        public void ThumbDis_07FFD20E()
        {
            AssertCode("@@@", "07FFD20E");
        }
        // Reko: a decoder for the instruction 65EF8B2E at address 00128A22 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)

        // Reko: a decoder for the instruction 04EF4B2E at address 00128A30 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_04EF4B2E()
        {
            AssertCode("@@@", "04EF4B2E");
        }
        // Reko: a decoder for the instruction 9FEF4E3F at address 001CA31E has not been implemented. (Unimplemented '*' when decoding EF9F3F4E)
        [Test]
        public void ThumbDis_9FEF4E3F()
        {
            AssertCode("@@@", "9FEF4E3F");
        }
        // Reko: a decoder for the instruction A1F97E72 at address 00128A8A has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9A1727E)
        [Test]
        public void ThumbDis_A1F97E72()
        {
            AssertCode("@@@", "A1F97E72");
        }
        // Reko: a decoder for the instruction 7FEF0AEB at address 00128C44 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_7FEF0AEB()
        {
            AssertCode("@@@", "7FEF0AEB");
        }
        // Reko: a decoder for the instruction 06FD40BC at address 00106BAC has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_06FD40BC()
        {
            AssertCode("@@@", "06FD40BC");
        }
        // Reko: a decoder for the instruction C2EE72D9 at address 001C798C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction E7F90195 at address 00168E20 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9E79501)
        [Test]
        public void ThumbDis_E7F90195()
        {
            AssertCode("@@@", "E7F90195");
        }
        // Reko: a decoder for the instruction C3F92492 at address 001AA086 has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9C39224)
        [Test]
        public void ThumbDis_C3F92492()
        {
            AssertCode("@@@", "C3F92492");
        }
        // Reko: a decoder for the instruction 19F9757E at address 00168F12 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_19F9757E()
        {
            AssertCode("@@@", "19F9757E");
        }
        // Reko: a decoder for the instruction 95EF9004 at address 00128E82 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_95EF9004()
        {
            AssertCode("@@@", "95EF9004");
        }
        // Reko: a decoder for the instruction 53FD85DD at address 00187BE6 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 91FFD65D at address 001C8684 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_91FFD65D()
        {
            AssertCode("@@@", "91FFD65D");
        }
        // Reko: a decoder for the instruction 95FD7D4C at address 00169C2A has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_95FD7D4C()
        {
            AssertCode("@@@", "95FD7D4C");
        }
        // Reko: a decoder for the instruction DEFC8D99 at address 001EBFB2 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DEFC8D99()
        {
            AssertCode("@@@", "DEFC8D99");
        }
        // Reko: a decoder for the instruction 36EEBB49 at address 001099DA has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_36EEBB49()
        {
            AssertCode("@@@", "36EEBB49");
        }
        // Reko: a decoder for the instruction D5FDC3CC at address 001EC122 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_D5FDC3CC()
        {
            AssertCode("@@@", "D5FDC3CC");
        }
        // Reko: a decoder for the instruction A7FA8743 at address 00189A72 has not been implemented. (Unimplemented '*' when decoding FAA74387)
        [Test]
        public void ThumbDis_A7FA8743()
        {
            AssertCode("@@@", "A7FA8743");
        }
        // Reko: a decoder for the instruction C3F92A1F at address 00109A70 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C3F92A1F()
        {
            AssertCode("@@@", "C3F92A1F");
        }
        // Reko: a decoder for the instruction E1EFC3A9 at address 001EC380 has not been implemented. (Unimplemented '*scalar' when decoding EFE1A9C3)
        [Test]
        public void ThumbDis_E1EFC3A9()
        {
            AssertCode("@@@", "E1EFC3A9");
        }
        // Reko: a decoder for the instruction 64FDF87D at address 00189B86 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_64FDF87D()
        {
            AssertCode("@@@", "64FDF87D");
        }
        // Reko: a decoder for the instruction 02FCE39D at address 00129E1A has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_02FCE39D()
        {
            AssertCode("@@@", "02FCE39D");
        }
        // Reko: a decoder for the instruction 39FC70D8 at address 00129E2C has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_39FC70D8()
        {
            AssertCode("@@@", "39FC70D8");
        }
        // Reko: a decoder for the instruction CAFC74A8 at address 00189EAE has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CAFC74A8()
        {
            AssertCode("@@@", "CAFC74A8");
        }
        // Reko: a decoder for the instruction A4FC0E6D at address 0012A9A6 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_A4FC0E6D()
        {
            AssertCode("@@@", "A4FC0E6D");
        }
        // Reko: a decoder for the instruction A7EEF5AB at address 0016AC6C has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction AEFC1E18 at address 0014BD00 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_AEFC1E18()
        {
            AssertCode("@@@", "AEFC1E18");
        }
        // Reko: a decoder for the instruction C9EF5559 at address 0014C0BA has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding EFC95955)
        [Test]
        public void ThumbDis_C9EF5559()
        {
            AssertCode("@@@", "C9EF5559");
        }
        // Reko: a decoder for the instruction 81FCD8E8 at address 0010B236 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_81FCD8E8()
        {
            AssertCode("@@@", "81FCD8E8");
        }
        // Reko: a decoder for the instruction EDFCC679 at address 0014C1E2 has not been implemented. (op1:op2=0b0110)
        [Test]
        public void ThumbDis_EDFCC679()
        {
            AssertCode("@@@", "EDFCC679");
        }
        // Reko: a decoder for the instruction FEF344A9 at address 001ED5AA has not been implemented. (Unimplemented '*register' when decoding F3FEA944)
        [Test]
        public void ThumbDis_FEF344A9()
        {
            AssertCode("@@@", "FEF344A9");
        }
        // Reko: a decoder for the instruction 06FDFB8D at address 001ED616 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_06FDFB8D()
        {
            AssertCode("@@@", "06FDFB8D");
        }
        // Reko: a decoder for the instruction 98FCB9CC at address 0012AF10 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_98FCB9CC()
        {
            AssertCode("@@@", "98FCB9CC");
        }
        // Reko: a decoder for the instruction 16FE7569 at address 001ACB76 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_16FE7569()
        {
            AssertCode("@@@", "16FE7569");
        }
        // Reko: a decoder for the instruction EBFE14E9 at address 0010B26E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_EBFE14E9()
        {
            AssertCode("@@@", "EBFE14E9");
        }
        // Reko: a decoder for the instruction 1CFFE3E3 at address 0018B31C has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_1CFFE3E3()
        {
            AssertCode("@@@", "1CFFE3E3");
        }
        // Reko: a decoder for the instruction 3BF9150E at address 0014C5EE has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_3BF9150E()
        {
            AssertCode("@@@", "3BF9150E");
        }
        // Reko: a decoder for the instruction 3CFC287D at address 0016B7FA has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_3CFC287D()
        {
            AssertCode("@@@", "3CFC287D");
        }
        // Reko: a decoder for the instruction E9EEFD7A at address 0018B496 has not been implemented. (Unimplemented '*' when decoding EEE97AFD)
        [Test]
        public void ThumbDis_E9EEFD7A()
        {
            AssertCode("@@@", "E9EEFD7A");
        }
        // Reko: a decoder for the instruction 34FCFBED at address 0012B3E0 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_34FCFBED()
        {
            AssertCode("@@@", "34FCFBED");
        }
        // Reko: a decoder for the instruction C3FF4A32 at address 001AD5F4 has not been implemented. (Unimplemented '*scalar' when decoding FFC3324A)
        [Test]
        public void ThumbDis_C3FF4A32()
        {
            AssertCode("@@@", "C3FF4A32");
        }
        // Reko: a decoder for the instruction 24EFE42C at address 0010BBE8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_24EFE42C()
        {
            AssertCode("@@@", "24EFE42C");
        }
        // Reko: a decoder for the instruction 47EF34D2 at address 001CD316 has not been implemented. (Unimplemented '*' when decoding EF47D234)
        [Test]
        public void ThumbDis_47EF34D2()
        {
            AssertCode("@@@", "47EF34D2");
        }
        // Reko: a decoder for the instruction 17FC771D at address 0016CC2A has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction AAFE42CD at address 0018CBB6 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_AAFE42CD()
        {
            AssertCode("@@@", "AAFE42CD");
        }
        // Reko: a decoder for the instruction 78FF5799 at address 0016D14C has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction DBFCE438 at address 0016D226 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_DBFCE438()
        {
            AssertCode("@@@", "DBFCE438");
        }
        // Reko: a decoder for the instruction 34EFD013 at address 0018CC98 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_34EFD013()
        {
            AssertCode("@@@", "34EFD013");
        }
        // Reko: a decoder for the instruction 03F9FAB2 at address 0018CCA8 has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F903B2FA)
        [Test]
        public void ThumbDis_03F9FAB2()
        {
            AssertCode("@@@", "03F9FAB2");
        }
        // Reko: a decoder for the instruction 99FD234D at address 001EED9A has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_99FD234D()
        {
            AssertCode("@@@", "99FD234D");
        }
        // Reko: a decoder for the instruction E1FF42A9 at address 001CDDA4 has not been implemented. (Unimplemented '*scalar' when decoding FFE1A942)
        [Test]
        public void ThumbDis_E1FF42A9()
        {
            AssertCode("@@@", "E1FF42A9");
        }
        // Reko: a decoder for the instruction A2FFFC5D at address 0014CC54 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_A2FFFC5D()
        {
            AssertCode("@@@", "A2FFFC5D");
        }
        // Reko: a decoder for the instruction 88FAAD53 at address 0014CD44 has not been implemented. (Unimplemented '*' when decoding FA8853AD)
        [Test]
        public void ThumbDis_88FAAD53()
        {
            AssertCode("@@@", "88FAAD53");
        }
        // Reko: a decoder for the instruction 55FC74EC at address 001CDFC0 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 16FEB089 at address 0016DB26 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_16FEB089()
        {
            AssertCode("@@@", "16FEB089");
        }
        // Reko: a decoder for the instruction AAEFC357 at address 0014CE2E has not been implemented. (Unimplemented '*' when decoding EFAA57C3)

        // Reko: a decoder for the instruction F4EEDBDB at address 0018D206 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
 
        // Reko: a decoder for the instruction 8FF9F559 at address 001EF782 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F98F59F5)
        [Test]
        public void ThumbDis_8FF9F559()
        {
            AssertCode("@@@", "8FF9F559");
        }
        // Reko: a decoder for the instruction FEFE5CDC at address 0012C76E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_FEFE5CDC()
        {
            AssertCode("@@@", "FEFE5CDC");
        }
        // Reko: a decoder for the instruction 96FDD21D at address 0014D376 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_96FDD21D()
        {
            AssertCode("@@@", "96FDD21D");
        }
        // Reko: a decoder for the instruction C8EF64CD at address 001AE8DA has not been implemented. (Unimplemented '*' when decoding EFC8CD64)
        [Test]
        public void ThumbDis_C8EF64CD()
        {
            AssertCode("@@@", "C8EF64CD");
        }
        // Reko: a decoder for the instruction EAF933B4 at address 0016E328 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9EAB433)
        [Test]
        public void ThumbDis_EAF933B4()
        {
            AssertCode("@@@", "EAF933B4");
        }
        // Reko: a decoder for the instruction 97FC5AA8 at address 001AE946 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_97FC5AA8()
        {
            AssertCode("@@@", "97FC5AA8");
        }
        // Reko: a decoder for the instruction 49EF58D3 at address 0012C812 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_49EF58D3()
        {
            AssertCode("@@@", "49EF58D3");
        }
        // Reko: a decoder for the instruction CFF9C15E at address 001EFA9C has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_CFF9C15E()
        {
            AssertCode("@@@", "CFF9C15E");
        }
        // Reko: a decoder for the instruction 02FDEC79 at address 0016E52E has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_02FDEC79()
        {
            AssertCode("@@@", "02FDEC79");
        }
        // Reko: a decoder for the instruction 07FCA1DC at address 0018DAB2 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_07FCA1DC()
        {
            AssertCode("@@@", "07FCA1DC");
        }
        // Reko: a decoder for the instruction 33FFF844 at address 0012C86A has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_33FFF844()
        {
            AssertCode("@@@", "33FFF844");
        }
        // Reko: a decoder for the instruction 80EC04AA at address 001AECF4 has not been implemented. (Unimplemented '*' when decoding EC80AA04)
        [Test]
        public void ThumbDis_80EC04AA()
        {
            AssertCode("@@@", "80EC04AA");
        }
        // Reko: a decoder for the instruction 29FF4CD4 at address 0018DB0E has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_29FF4CD4()
        {
            AssertCode("@@@", "29FF4CD4");
        }
        // Reko: a decoder for the instruction BDEFB774 at address 0018DB12 has not been implemented. (U=0)
        [Test]
        public void ThumbDis_BDEFB774()
        {
            AssertCode("@@@", "BDEFB774");
        }
        // Reko: a decoder for the instruction ABECA5BB at address 0016E55C has not been implemented. (Unimplemented '*' when decoding ECABBBA5)
        [Test]
        public void ThumbDis_ABECA5BB()
        {
            AssertCode("@@@", "ABECA5BB");
        }
        // Reko: a decoder for the instruction 41FC4BBD at address 0012CA3C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_41FC4BBD()
        {
            AssertCode("@@@", "41FC4BBD");
        }
        // Reko: a decoder for the instruction B4FC0DDC at address 0016EF6E has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B4FC0DDC()
        {
            AssertCode("@@@", "B4FC0DDC");
        }
        // Reko: a decoder for the instruction 5DFF59A9 at address 0010F132 has not been implemented. (*vmul (integer and polynomial)
 
        // Reko: a decoder for the instruction ABF9FD69 at address 0010F14C has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9AB69FD)
        [Test]
        public void ThumbDis_ABF9FD69()
        {
            AssertCode("@@@", "ABF9FD69");
        }
        // Reko: a decoder for the instruction 3EFFB5EC at address 0016F3A2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_3EFFB5EC()
        {
            AssertCode("@@@", "3EFFB5EC");
        }
        // Reko: a decoder for the instruction A9EF728B at address 0018E59A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_A9EF728B()
        {
            AssertCode("@@@", "A9EF728B");
        }
        // Reko: a decoder for the instruction F0FFE4E0 at address 0018E5B4 has not been implemented. (Unimplemented '*' when decoding FFF0E0E4)
        [Test]
        public void ThumbDis_F0FFE4E0()
        {
            AssertCode("@@@", "F0FFE4E0");
        }
        // Reko: a decoder for the instruction 60FCC73C at address 0016F6E6 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_60FCC73C()
        {
            AssertCode("@@@", "60FCC73C");
        }
        // Reko: a decoder for the instruction 4EFF725F at address 0010F95C has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_4EFF725F()
        {
            AssertCode("@@@", "4EFF725F");
        }
        // Reko: a decoder for the instruction 30EF198B at address 0018E8E8 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_30EF198B()
        {
            AssertCode("@@@", "30EF198B");
        }
        // Reko: a decoder for the instruction ACF96CD2 at address 0014E9CE has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9ACD26C)
        [Test]
        public void ThumbDis_ACF96CD2()
        {
            AssertCode("@@@", "ACF96CD2");
        }
        // Reko: a decoder for the instruction 4EEF3B1C at address 0010F9F0 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_4EEF3B1C()
        {
            AssertCode("@@@", "4EEF3B1C");
        }
        // Reko: a decoder for the instruction B2EFBCEC at address 0010FA66 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_B2EFBCEC()
        {
            AssertCode("@@@", "B2EFBCEC");
        }
        // Reko: a decoder for the instruction 4BEFB36E at address 0012D91C has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)
        [Test]
        public void ThumbDis_4BEFB36E()
        {
            AssertCode("@@@", "4BEFB36E");
        }
        // Reko: a decoder for the instruction 1CEFB1FD at address 0014EA1C has not been implemented. (*vmla (floating point))
        [Test]
        public void ThumbDis_1CEFB1FD()
        {
            AssertCode("@@@", "1CEFB1FD");
        }
        // Reko: a decoder for the instruction 2BFDCDE8 at address 0014EF26 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2BFDCDE8()
        {
            AssertCode("@@@", "2BFDCDE8");
        }
        // Reko: a decoder for the instruction C1FAACE8 at address 001CEDA4 has not been implemented. (crc32-crc32w)
        [Test]
        public void ThumbDis_C1FAACE8()
        {
            AssertCode("@@@", "C1FAACE8");
        }
        // Reko: a decoder for the instruction C7F9751B at address 0010E8F2 has not been implemented. (Unimplemented 'single 4-element structure from one lane - T3' when decoding F9C71B75)
        [Test]
        public void ThumbDis_C7F9751B()
        {
            AssertCode("@@@", "C7F9751B");
        }
        // Reko: a decoder for the instruction 98FDFA39 at address 0018F02C has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_98FDFA39()
        {
            AssertCode("@@@", "98FDFA39");
        }
        // Reko: a decoder for the instruction 81EED9A9 at address 0018F79A has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 1BF9784E at address 0018F7C0 has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_1BF9784E()
        {
            AssertCode("@@@", "1BF9784E");
        }
        // Reko: a decoder for the instruction E8F92BC5 at address 0016FE6E has not been implemented. (Unimplemented 'single 2-element structure from one lane - T2' when decoding F9E8C52B)
        [Test]
        public void ThumbDis_E8F92BC5()
        {
            AssertCode("@@@", "E8F92BC5");
        }
        // Reko: a decoder for the instruction A8F9A64C at address 001CFC54 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_A8F9A64C()
        {
            AssertCode("@@@", "A8F9A64C");
        }
        // Reko: a decoder for the instruction F8FF9EF8 at address 001F0352 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=1)
        [Test]
        public void ThumbDis_F8FF9EF8()
        {
            AssertCode("@@@", "F8FF9EF8");
        }
        // Reko: a decoder for the instruction 19FD376D at address 0012E238 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 76F9617E at address 0018F96C has not been implemented. (LoadStoreSignedUnprivileged)
        [Test]
        public void ThumbDis_76F9617E()
        {
            AssertCode("@@@", "76F9617E");
        }
        // Reko: a decoder for the instruction 11FCF348 at address 001CFC78 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 35FEF9CC at address 001F08A4 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_35FEF9CC()
        {
            AssertCode("@@@", "35FEF9CC");
        }
        // Reko: a decoder for the instruction 04FCAAB8 at address 0012E2A4 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_04FCAAB8()
        {
            AssertCode("@@@", "04FCAAB8");
        }
        // Reko: a decoder for the instruction 93FD902C at address 001B2296 has not been implemented. (op1:op2=0b1101)
        [Test]
        public void ThumbDis_93FD902C()
        {
            AssertCode("@@@", "93FD902C");
        }
        // Reko: a decoder for the instruction E8EFB90B at address 0012EC4A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_E8EFB90B()
        {
            AssertCode("@@@", "E8EFB90B");
        }
        // Reko: a decoder for the instruction 41FC1AFC at address 0011182A has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_41FC1AFC()
        {
            AssertCode("@@@", "41FC1AFC");
        }
        // Reko: a decoder for the instruction 19FD80DD at address 0012EDA6 has not been implemented. (op1:op2=0b1001)

        // Reko: a decoder for the instruction 73FC4569 at address 001D0412 has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_73FC4569()
        {
            AssertCode("@@@", "73FC4569");
        }
        // Reko: a decoder for the instruction BEEE439B at address 0012F234 has not been implemented. (1110 - _HSD)
        [Test]
        public void ThumbDis_BEEE439B()
        {
            AssertCode("@@@", "BEEE439B");
        }
        // Reko: a decoder for the instruction E0ECD82B at address 001F3604 has not been implemented. (Unimplemented '*' when decoding ECE02BD8)
        [Test]
        public void ThumbDis_E0ECD82B()
        {
            AssertCode("@@@", "E0ECD82B");
        }
        // Reko: a decoder for the instruction F4FFCEB4 at address 00112630 has not been implemented. (Unimplemented '*' when decoding FFF4B4CE)
        [Test]
        public void ThumbDis_F4FFCEB4()
        {
            AssertCode("@@@", "F4FFCEB4");
        }
        // Reko: a decoder for the instruction D1EFFC9C at address 001B2B08 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_D1EFFC9C()
        {
            AssertCode("@@@", "D1EFFC9C");
        }
        // Reko: a decoder for the instruction 88FF7909 at address 001F3730 has not been implemented. (Unimplemented 'D22_12,Q5_0,*signed result variant' when decoding FF880979)
        [Test]
        public void ThumbDis_88FF7909()
        {
            AssertCode("@@@", "88FF7909");
        }
        // Reko: a decoder for the instruction C7FED86D at address 00112A96 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_C7FED86D()
        {
            AssertCode("@@@", "C7FED86D");
        }
        // Reko: a decoder for the instruction A2FE633D at address 00112E6E has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A2FE633D()
        {
            AssertCode("@@@", "A2FE633D");
        }
        // Reko: a decoder for the instruction DDE8D906 at address 0012FE82 has not been implemented. (Unimplemented '*' when decoding E8DD06D9)
        [Test]
        public void ThumbDis_DDE8D906()
        {
            AssertCode("@@@", "DDE8D906");
        }
        // Reko: a decoder for the instruction 82FCDA59 at address 0012FF10 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_82FCDA59()
        {
            AssertCode("@@@", "82FCDA59");
        }
        // Reko: a decoder for the instruction EDFFF45C at address 00192ED0 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_EDFFF45C()
        {
            AssertCode("@@@", "EDFFF45C");
        }
        // Reko: a decoder for the instruction FEFD3539 at address 001B3CB0 has not been implemented. (op1:op2=0b1111)
        [Test]
        public void ThumbDis_FEFD3539()
        {
            AssertCode("@@@", "FEFD3539");
        }
        // Reko: a decoder for the instruction 07FFFF7C at address 00151D4A has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_07FFFF7C()
        {
            AssertCode("@@@", "07FFFF7C");
        }
        // Reko: a decoder for the instruction 53FFD414 at address 0019458C has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_53FFD414()
        {
            AssertCode("@@@", "53FFD414");
        }
        // Reko: a decoder for the instruction 96EE98CB at address 001D2DC2 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction E8FFC33E at address 00110876 has not been implemented. (Unimplemented '*' when decoding FFE83EC3)
        [Test]
        public void ThumbDis_E8FFC33E()
        {
            AssertCode("@@@", "E8FFC33E");
        }
        // Reko: a decoder for the instruction D3EEF3E9 at address 00194606 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)

        // Reko: a decoder for the instruction 1CF940AD at address 001D2DDE has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_1CF940AD()
        {
            AssertCode("@@@", "1CF940AD");
        }
        // Reko: a decoder for the instruction EEFDEE9C at address 00173458 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_EEFDEE9C()
        {
            AssertCode("@@@", "EEFDEE9C");
        }
        // Reko: a decoder for the instruction 47FC6A68 at address 001F4AD0 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_47FC6A68()
        {
            AssertCode("@@@", "47FC6A68");
        }
        // Reko: a decoder for the instruction F0F70C86 at address 00173464 has not been implemented. (Unimplemented '*' when decoding F7F0860C)
        [Test]
        public void ThumbDis_F0F70C86()
        {
            AssertCode("@@@", "F0F70C86");
        }
        // Reko: a decoder for the instruction 0AFF9EDB at address 001D2E7C has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_0AFF9EDB()
        {
            AssertCode("@@@", "0AFF9EDB");
        }
        // Reko: a decoder for the instruction B8FCAEEC at address 0012FABA has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B8FCAEEC()
        {
            AssertCode("@@@", "B8FCAEEC");
        }
        // Reko: a decoder for the instruction 2EFD27ED at address 001F4B30 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_2EFD27ED()
        {
            AssertCode("@@@", "2EFD27ED");
        }
        // Reko: a decoder for the instruction EFF9C752 at address 00173D6A has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9EF52C7)
        [Test]
        public void ThumbDis_EFF9C752()
        {
            AssertCode("@@@", "EFF9C752");
        }
        // Reko: a decoder for the instruction 77FF665C at address 001311F2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_77FF665C()
        {
            AssertCode("@@@", "77FF665C");
        }
        // Reko: a decoder for the instruction 14FFB329 at address 00131FA0 has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction 72EF0A2B at address 00132052 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_72EF0A2B()
        {
            AssertCode("@@@", "72EF0A2B");
        }
        // Reko: a decoder for the instruction 65EF05FE at address 00114922 has not been implemented. (AdvancedSimd3RegistersSameLength_opcE U=0)

        // Reko: a decoder for the instruction AFFD8F09 at address 00175212 has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_AFFD8F09()
        {
            AssertCode("@@@", "AFFD8F09");
        }
        // Reko: a decoder for the instruction 4AFC7968 at address 00152E70 has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_4AFC7968()
        {
            AssertCode("@@@", "4AFC7968");
        }
        // Reko: a decoder for the instruction 63FF3F53 at address 001952B4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_63FF3F53()
        {
            AssertCode("@@@", "63FF3F53");
        }
        // Reko: a decoder for the instruction 83FF3A9B at address 00132154 has not been implemented. (Unimplemented '*immediate - T2' when decoding FF839B3A)
        [Test]
        public void ThumbDis_83FF3A9B()
        {
            AssertCode("@@@", "83FF3A9B");
        }
        // Reko: a decoder for the instruction 63FD9A1D at address 001B6208 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_63FD9A1D()
        {
            AssertCode("@@@", "63FD9A1D");
        }
        // Reko: a decoder for the instruction D5EF4E2A at address 0015318A has not been implemented. (Unimplemented '*' when decoding EFD52A4E)
        [Test]
        public void ThumbDis_D5EF4E2A()
        {
            AssertCode("@@@", "D5EF4E2A");
        }
        // Reko: a decoder for the instruction 78EF17C3 at address 00116166 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_78EF17C3()
        {
            AssertCode("@@@", "78EF17C3");
        }
        // Reko: a decoder for the instruction D3EF1F0C at address 001534A8 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcC)
        [Test]
        public void ThumbDis_D3EF1F0C()
        {
            AssertCode("@@@", "D3EF1F0C");
        }
        // Reko: a decoder for the instruction 2DFCA7D9 at address 001321C8 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2DFCA7D9()
        {
            AssertCode("@@@", "2DFCA7D9");
        }
        // Reko: a decoder for the instruction 16EFDCDC at address 001535EE has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_16EFDCDC()
        {
            AssertCode("@@@", "16EFDCDC");
        }
        // Reko: a decoder for the instruction 05FD3C5C at address 0015362E has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_05FD3C5C()
        {
            AssertCode("@@@", "05FD3C5C");
        }
        // Reko: a decoder for the instruction 53FF4044 at address 00132256 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_53FF4044()
        {
            AssertCode("@@@", "53FF4044");
        }
        // Reko: a decoder for the instruction 28FCDA5E at address 001D5334 has not been implemented. (Unimplemented '*post-indexed' when decoding FC285EDA)
        [Test]
        public void ThumbDis_28FCDA5E()
        {
            AssertCode("@@@", "28FCDA5E");
        }
        // Reko: a decoder for the instruction 85FD511D at address 00153E8A has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_85FD511D()
        {
            AssertCode("@@@", "85FD511D");
        }
        // Reko: a decoder for the instruction 2CFCF158 at address 001167F6 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2CFCF158()
        {
            AssertCode("@@@", "2CFCF158");
        }
        // Reko: a decoder for the instruction 93ECB16F at address 001B7228 has not been implemented. (Unimplemented '*' when decoding EC936FB1)
        [Test]
        public void ThumbDis_93ECB16F()
        {
            AssertCode("@@@", "93ECB16F");
        }
        // Reko: a decoder for the instruction 1AF9A40F at address 001B7268 has not been implemented. (LoadStoreSignedImmediatePreIndexed)
        [Test]
        public void ThumbDis_1AF9A40F()
        {
            AssertCode("@@@", "1AF9A40F");
        }
        // Reko: a decoder for the instruction 4DFDDE6D at address 001D5490 has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4DFDDE6D()
        {
            AssertCode("@@@", "4DFDDE6D");
        }
        // Reko: a decoder for the instruction 54FFB79C at address 001F585C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_54FFB79C()
        {
            AssertCode("@@@", "54FFB79C");
        }
        // Reko: a decoder for the instruction CAFCECD9 at address 00116AB4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_CAFCECD9()
        {
            AssertCode("@@@", "CAFCECD9");
        }
        // Reko: a decoder for the instruction 69FDA588 at address 00175902 has not been implemented. (op1:op2=0b1010)
        [Test]
        public void ThumbDis_69FDA588()
        {
            AssertCode("@@@", "69FDA588");
        }
        // Reko: a decoder for the instruction 0EF92CE2 at address 00196034 has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F90EE22C)
        [Test]
        public void ThumbDis_0EF92CE2()
        {
            AssertCode("@@@", "0EF92CE2");
        }
        // Reko: a decoder for the instruction 8EFCBEBD at address 001759F4 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_8EFCBEBD()
        {
            AssertCode("@@@", "8EFCBEBD");
        }
        // Reko: a decoder for the instruction 9EFCEF38 at address 001962DC has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_9EFCEF38()
        {
            AssertCode("@@@", "9EFCEF38");
        }
        // Reko: a decoder for the instruction 82FF6C18 at address 00153FE2 has not been implemented. (Unimplemented '*scalar' when decoding FF82186C)
        [Test]
        public void ThumbDis_82FF6C18()
        {
            AssertCode("@@@", "82FF6C18");
        }
        // Reko: a decoder for the instruction E4FD5A6C at address 001D5A2A has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E4FD5A6C()
        {
            AssertCode("@@@", "E4FD5A6C");
        }
        // Reko: a decoder for the instruction E1F94ED4 at address 00116EA4 has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9E1D44E)
        [Test]
        public void ThumbDis_E1F94ED4()
        {
            AssertCode("@@@", "E1F94ED4");
        }
        // Reko: a decoder for the instruction 66FCE318 at address 001B7270 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_66FCE318()
        {
            AssertCode("@@@", "66FCE318");
        }
        // Reko: a decoder for the instruction 27FED20D at address 001D5B20 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_27FED20D()
        {
            AssertCode("@@@", "27FED20D");
        }
        // Reko: a decoder for the instruction 74FEC86C at address 001B7288 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_74FEC86C()
        {
            AssertCode("@@@", "74FEC86C");
        }
        // Reko: a decoder for the instruction 48FFF9EF at address 00133178 has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 1)
        [Test]
        public void ThumbDis_48FFF9EF()
        {
            AssertCode("@@@", "48FFF9EF");
        }
        // Reko: a decoder for the instruction C0F9EC5D at address 00196924 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_C0F9EC5D()
        {
            AssertCode("@@@", "C0F9EC5D");
        }
        // Reko: a decoder for the instruction E2FDE479 at address 001F85EC has not been implemented. (op1:op2=0b1110)
        [Test]
        public void ThumbDis_E2FDE479()
        {
            AssertCode("@@@", "E2FDE479");
        }
        // Reko: a decoder for the instruction 25FC5CBC at address 00176596 has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_25FC5CBC()
        {
            AssertCode("@@@", "25FC5CBC");
        }
        // Reko: a decoder for the instruction 8FFD2338 at address 001765A6 has not been implemented. (op1:op2=0b1100)
        [Test]
        public void ThumbDis_8FFD2338()
        {
            AssertCode("@@@", "8FFD2338");
        }
        // Reko: a decoder for the instruction C2F98D59 at address 001334F2 has not been implemented. (Unimplemented 'single 2-element structure from one lane - T3' when decoding F9C2598D)
        [Test]
        public void ThumbDis_C2F98D59()
        {
            AssertCode("@@@", "C2F98D59");
        }
        // Reko: a decoder for the instruction 89EEBBCB at address 001964BE has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_89EEBBCB()
        {
            AssertCode("@@@", "89EEBBCB");
        }
        // Reko: a decoder for the instruction D9EFC72E at address 0015470E has not been implemented. (Unimplemented '*' when decoding EFD92EC7)
        [Test]
        public void ThumbDis_D9EFC72E()
        {
            AssertCode("@@@", "D9EFC72E");
        }
        // Reko: a decoder for the instruction 12FF8FB3 at address 001B77AA has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_12FF8FB3()
        {
            AssertCode("@@@", "12FF8FB3");
        }
        // Reko: a decoder for the instruction 5AFF52F1 at address 001F9114 has not been implemented. (Unimplemented '*register' when decoding FF5AF152)
        [Test]
        public void ThumbDis_5AFF52F1()
        {
            AssertCode("@@@", "5AFF52F1");
        }
        // Reko: a decoder for the instruction 0DFF97F4 at address 001337CA has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_0DFF97F4()
        {
            AssertCode("@@@", "0DFF97F4");
        }
        // Reko: a decoder for the instruction 82FEB809 at address 00118436 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_82FEB809()
        {
            AssertCode("@@@", "82FEB809");
        }
        // Reko: a decoder for the instruction C8FC0C6D at address 001978D8 has not been implemented. (op1:op2=0b0100)
        [Test]
        public void ThumbDis_C8FC0C6D()
        {
            AssertCode("@@@", "C8FC0C6D");
        }
        // Reko: a decoder for the instruction 4BFFD869 at address 00118972 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_4BFFD869()
        {
            AssertCode("@@@", "4BFFD869");
        }
        // Reko: a decoder for the instruction 8AF9DC3C at address 00118994 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8AF9DC3C()
        {
            AssertCode("@@@", "8AF9DC3C");
        }
        // Reko: a decoder for the instruction 3FEFF859 at address 00197DBE has not been implemented. (*vmul (integer and polynomial)

        // Reko: a decoder for the instruction DBFF4482 at address 001B7F4C has not been implemented. (Unimplemented '*scalar' when decoding FFDB8244)
        [Test]
        public void ThumbDis_DBFF4482()
        {
            AssertCode("@@@", "DBFF4482");
        }
        // Reko: a decoder for the instruction 00FC249D at address 0017836C has not been implemented. (op1:op2=0b0000)
        [Test]
        public void ThumbDis_00FC249D()
        {
            AssertCode("@@@", "00FC249D");
        }
        // Reko: a decoder for the instruction F2EF1A38 at address 00135770 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opc8 U=0 L:Q=00)
        [Test]
        public void ThumbDis_F2EF1A38()
        {
            AssertCode("@@@", "F2EF1A38");
        }
        // Reko: a decoder for the instruction 51FD0B98 at address 00198C72 has not been implemented. (op1:op2=0b1001)
  
        // Reko: a decoder for the instruction D8FC29FD at address 001FA800 has not been implemented. (op1:op2=0b0101)
        [Test]
        public void ThumbDis_D8FC29FD()
        {
            AssertCode("@@@", "D8FC29FD");
        }
        // Reko: a decoder for the instruction 98EFE8A3 at address 00155FA2 has not been implemented. (Unimplemented '*' when decoding EF98A3E8)
        [Test]
        public void ThumbDis_98EFE8A3()
        {
            AssertCode("@@@", "98EFE8A3");
        }
        // Reko: a decoder for the instruction 3AFED42C at address 001FAE66 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_3AFED42C()
        {
            AssertCode("@@@", "3AFED42C");
        }
        // Reko: a decoder for the instruction EDF9EB68 at address 00136518 has not been implemented. (Unimplemented 'single element from one lane - T3' when decoding F9ED68EB)
        [Test]
        public void ThumbDis_EDF9EB68()
        {
            AssertCode("@@@", "EDF9EB68");
        }
        // Reko: a decoder for the instruction 03EF337B at address 001FAF6E has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_03EF337B()
        {
            AssertCode("@@@", "03EF337B");
        }
        // Reko: a decoder for the instruction 10FF1949 at address 001996BE has not been implemented. (*vmul (integer and polynomial)
        // Reko: a decoder for the instruction 0CFFA93B at address 0011B674 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_0CFFA93B()
        {
            AssertCode("@@@", "0CFFA93B");
        }
        // Reko: a decoder for the instruction 5CFFFF02 at address 001D6B1C has not been implemented. (Unimplemented '*' when decoding FF5C02FF)
        [Test]
        public void ThumbDis_5CFFFF02()
        {
            AssertCode("@@@", "5CFFFF02");
        }
        // Reko: a decoder for the instruction 75FC464C at address 00156AFC has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_75FC464C()
        {
            AssertCode("@@@", "75FC464C");
        }
        // Reko: a decoder for the instruction 1DFFAF21 at address 00199788 has not been implemented. (Unimplemented '*' when decoding FF1D21AF)
        [Test]
        public void ThumbDis_1DFFAF21()
        {
            AssertCode("@@@", "1DFFAF21");
        }
        // Reko: a decoder for the instruction 79FF8894 at address 001D6D1C has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_79FF8894()
        {
            AssertCode("@@@", "79FF8894");
        }
        // Reko: a decoder for the instruction D8EFAB39 at address 0019987A has not been implemented. (Unimplemented '*integer' when decoding EFD839AB)
        [Test]
        public void ThumbDis_D8EFAB39()
        {
            AssertCode("@@@", "D8EFAB39");
        }
        // Reko: a decoder for the instruction 80FEE61D at address 00118A4A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_80FEE61D()
        {
            AssertCode("@@@", "80FEE61D");
        }
        // Reko: a decoder for the instruction 92FABE2A at address 001B977A has not been implemented. (Unimplemented '*' when decoding FA922ABE)
        [Test]
        public void ThumbDis_92FABE2A()
        {
            AssertCode("@@@", "92FABE2A");
        }
        // Reko: a decoder for the instruction 51EF96A4 at address 001999E6 has not been implemented. (AdvancedSimd3RegistersSameLength_opc4)
        [Test]
        public void ThumbDis_51EF96A4()
        {
            AssertCode("@@@", "51EF96A4");
        }
        // Reko: a decoder for the instruction 6DFCEF09 at address 00156C4E has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_6DFCEF09()
        {
            AssertCode("@@@", "6DFCEF09");
        }
        // Reko: a decoder for the instruction D7FA93AD at address 00137272 has not been implemented. (crc32c-crc32ch)
        [Test]
        public void ThumbDis_D7FA93AD()
        {
            AssertCode("@@@", "D7FA93AD");
        }
        // Reko: a decoder for the instruction 18FC0C5D at address 001D7626 has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 9BEFA609 at address 001D76D6 has not been implemented. (Unimplemented '*integer' when decoding EF9B09A6)
        [Test]
        public void ThumbDis_9BEFA609()
        {
            AssertCode("@@@", "9BEFA609");
        }
        // Reko: a decoder for the instruction 23FC0C18 at address 001D775E has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_23FC0C18()
        {
            AssertCode("@@@", "23FC0C18");
        }
        // Reko: a decoder for the instruction 7FFF401F at address 0013751E has not been implemented. (AdvancedSimd3RegistersSameLength_opcF U=0 op1 = 0 Q=1)
        [Test]
        public void ThumbDis_7FFF401F()
        {
            AssertCode("@@@", "7FFF401F");
        }
        // Reko: a decoder for the instruction 29FC144C at address 0013752C has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_29FC144C()
        {
            AssertCode("@@@", "29FC144C");
        }
        // Reko: a decoder for the instruction 6AEF14F9 at address 00137C50 has not been implemented. (*vmul (integer and polynomial)
        [Test]
        public void ThumbDis_6AEF14F9()
        {
            AssertCode("@@@", "6AEF14F9");
        }
        // Reko: a decoder for the instruction 8FFEBFDD at address 001FC8CC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_8FFEBFDD()
        {
            AssertCode("@@@", "8FFEBFDD");
        }
        // Reko: a decoder for the instruction 1BEA390B at address 00157D24 has not been implemented. (ANDS, rotate right with extend variant on)
        [Test]
        public void ThumbDis_1BEA390B()
        {
            AssertCode("@@@", "1BEA390B");
        }
        // Reko: a decoder for the instruction BCFC97E9 at address 0011C0EC has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_BCFC97E9()
        {
            AssertCode("@@@", "BCFC97E9");
        }
        // Reko: a decoder for the instruction E2F925D0 at address 00199524 has not been implemented. (Unimplemented 'single element from one lane - T1' when decoding F9E2D025)
        [Test]
        public void ThumbDis_E2F925D0()
        {
            AssertCode("@@@", "E2F925D0");
        }
        // Reko: a decoder for the instruction B4F324A4 at address 0019ABEA has not been implemented. (Unimplemented '*' when decoding F3B4A424)
        [Test]
        public void ThumbDis_B4F324A4()
        {
            AssertCode("@@@", "B4F324A4");
        }
        // Reko: a decoder for the instruction 4BFD3C0D at address 0019AC1E has not been implemented. (op1:op2=0b1000)
        [Test]
        public void ThumbDis_4BFD3C0D()
        {
            AssertCode("@@@", "4BFD3C0D");
        }
        // Reko: a decoder for the instruction 44F9F442 at address 001BADD6 has not been implemented. (Unimplemented '*multiple single elements - T4' when decoding F94442F4)
        [Test]
        public void ThumbDis_44F9F442()
        {
            AssertCode("@@@", "44F9F442");
        }
        // Reko: a decoder for the instruction 79EFFE51 at address 00137CFE has not been implemented. (Unimplemented '*register' when decoding EF7951FE)
        [Test]
        public void ThumbDis_79EFFE51()
        {
            AssertCode("@@@", "79EFFE51");
        }
        // Reko: a decoder for the instruction A5F906E4 at address 001D85CE has not been implemented. (Unimplemented 'single element from one lane - T2' when decoding F9A5E406)
        [Test]
        public void ThumbDis_A5F906E4()
        {
            AssertCode("@@@", "A5F906E4");
        }
        // Reko: a decoder for the instruction 3AEF7D43 at address 001FD458 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_3AEF7D43()
        {
            AssertCode("@@@", "3AEF7D43");
        }
        // Reko: a decoder for the instruction E0ECF0FA at address 001FD4A2 has not been implemented. (Unimplemented '*' when decoding ECE0FAF0)
        [Test]
        public void ThumbDis_E0ECF0FA()
        {
            AssertCode("@@@", "E0ECF0FA");
        }
        // Reko: a decoder for the instruction 46EE9A39 at address 00137D48 has not been implemented. (AdvancedSimd8_16_32_bitElementMove)
        [Test]
        public void ThumbDis_46EE9A39()
        {
            AssertCode("@@@", "46EE9A39");
        }
        // Reko: a decoder for the instruction E1F94D9E at address 00173C3E has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_E1F94D9E()
        {
            AssertCode("@@@", "E1F94D9E");
        }
        // Reko: a decoder for the instruction A9FE74D9 at address 0019AC5A has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_A9FE74D9()
        {
            AssertCode("@@@", "A9FE74D9");
        }
        // Reko: a decoder for the instruction 6CEF0113 at address 0011CEB4 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)
        [Test]
        public void ThumbDis_6CEF0113()
        {
            AssertCode("@@@", "6CEF0113");
        }
        // Reko: a decoder for the instruction 45EFA2E3 at address 00158624 has not been implemented. (AdvancedSimd3RegistersSameLength_opc3)

        // Reko: a decoder for the instruction F1FFF23D at address 001D884A has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_F1FFF23D()
        {
            AssertCode("@@@", "F1FFF23D");
        }
        // Reko: a decoder for the instruction 2BEF5901 at address 001389B8 has not been implemented. (Unimplemented '*register' when decoding EF2B0159)
        [Test]
        public void ThumbDis_2BEF5901()
        {
            AssertCode("@@@", "2AEF5A01");
        }
        // Reko: a decoder for the instruction 33FEA6AD at address 0019ADDA has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_33FEA6AD()
        {
            AssertCode("@@@", "33FEA6AD");
        }
        // Reko: a decoder for the instruction 18EF4DEB at address 00174A34 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_18EF4DEB()
        {
            AssertCode("@@@", "18EF4DEB");
        }
        // Reko: a decoder for the instruction 88FFDFFD at address 00138ABE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_88FFDFFD()
        {
            AssertCode("@@@", "88FFDFFD");
        }
        // Reko: a decoder for the instruction 0FFF833C at address 0011D87C has not been implemented. (AdvancedSimd3RegistersSameLength_opcC)
        [Test]
        public void ThumbDis_0FFF833C()
        {
            AssertCode("@@@", "0FFF833C");
        }
        // Reko: a decoder for the instruction B1FC7269 at address 001D8C1A has not been implemented. (op1:op2=0b0111)
        [Test]
        public void ThumbDis_B1FC7269()
        {
            AssertCode("@@@", "B1FC7269");
        }
        // Reko: a decoder for the instruction 9EFFC2BF at address 00138FBE has not been implemented. (Unimplemented '*' when decoding FF9EBFC2)
        [Test]
        public void ThumbDis_9EFFC2BF()
        {
            AssertCode("@@@", "9EFFC2BF");
        }
        // Reko: a decoder for the instruction 37EF68EB at address 0011D8A2 has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
  
        // Reko: a decoder for the instruction 2DFC920C at address 001D8C2C has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2DFC920C()
        {
            AssertCode("@@@", "2DFC920C");
        }
        // Reko: a decoder for the instruction 8EF957FE at address 0011DE62 has not been implemented. (AdvancedSimdLdSingleStructureToAllLanes)
        [Test]
        public void ThumbDis_8EF957FE()
        {
            AssertCode("@@@", "8EF957FE");
        }
        // Reko: a decoder for the instruction 9EEF486A at address 0013994E has not been implemented. (Unimplemented '*' when decoding EF9E6A48)
        [Test]
        public void ThumbDis_9EEF486A()
        {
            AssertCode("@@@", "9EEF486A");
        }
        // Reko: a decoder for the instruction 36FD0128 at address 001BC82E has not been implemented. (VCMLA)
        [Test]
        public void ThumbDis_36FD0128()
        {
            AssertCode("@@@", "36FD0128");
        }
        // Reko: a decoder for the instruction 3BFFEBCD at address 0019BB7E has not been implemented. (*vabd (floating point))
        [Test]
        public void ThumbDis_3BFFEBCD()
        {
            AssertCode("@@@", "3BFFEBCD");
        }
        // Reko: a decoder for the instruction 2AFE15FD at address 0013A084 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_2AFE15FD()
        {
            AssertCode("@@@", "2AFE15FD");
        }
        // Reko: a decoder for the instruction 6EFE5EB9 at address 001D9252 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_6EFE5EB9()
        {
            AssertCode("@@@", "6EFE5EB9");
        }
        // Reko: a decoder for the instruction 04EF32CF at address 0015AEAE has not been implemented. (Unimplemented '*' when decoding EF04CF32)
        [Test]
        public void ThumbDis_04EF32CF()
        {
            AssertCode("@@@", "04EF32CF");
        }
        // Reko: a decoder for the instruction 63FEBABC at address 001BDDAC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_63FEBABC()
        {
            AssertCode("@@@", "63FEBABC");
        }
        // Reko: a decoder for the instruction 74F9E75D at address 0015BA1C has not been implemented. (LoadStoreSignedImmediatePreIndexed)

        // Reko: a decoder for the instruction FBEF16AD at address 001BE208 has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_FBEF16AD()
        {
            AssertCode("@@@", "FBEF16AD");
        }
        // Reko: a decoder for the instruction 8DFA88C0 at address 001BE212 has not been implemented. (Unimplemented '*' when decoding FA8DC088)
        [Test]
        public void ThumbDis_8DFA88C0()
        {
            AssertCode("@@@", "8DFA88C0");
        }
        // Reko: a decoder for the instruction 8EEFC15A at address 0015B47A has not been implemented. (Unimplemented '*' when decoding EF8E5AC1)
        [Test]
        public void ThumbDis_8EEFC15A()
        {
            AssertCode("@@@", "8EEFC15A");
        }
        // Reko: a decoder for the instruction 28FC6F0C at address 0015BE8E has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_28FC6F0C()
        {
            AssertCode("@@@", "28FC6F0C");
        }
        // Reko: a decoder for the instruction CAFF14AD at address 001DBDCC has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcD)
        [Test]
        public void ThumbDis_CAFF14AD()
        {
            AssertCode("@@@", "CAFF14AD");
        }
        // Reko: a decoder for the instruction 2FFCCC28 at address 001DC73A has not been implemented. (op1:op2=0b0010)
        [Test]
        public void ThumbDis_2FFCCC28()
        {
            AssertCode("@@@", "2FFCCC28");
        }
        // Reko: a decoder for the instruction 16EF9DB2 at address 001BF152 has not been implemented. (Unimplemented '*' when decoding EF16B29D)
        [Test]
        public void ThumbDis_16EF9DB2()
        {
            AssertCode("@@@", "16EF9DB2");
        }
        // Reko: a decoder for the instruction E3F9DBD2 at address 0015EBCE has not been implemented. (Unimplemented 'single 3-element structure from one lane - T1' when decoding F9E3D2DB)
        [Test]
        public void ThumbDis_E3F9DBD2()
        {
            AssertCode("@@@", "E3F9DBD2");
        }
        // Reko: a decoder for the instruction 7AFC14EC at address 0015EBDE has not been implemented. (op1:op2=0b0011)
        [Test]
        public void ThumbDis_7AFC14EC()
        {
            AssertCode("@@@", "7AFC14EC");
        }
        // Reko: a decoder for the instruction E2FEDF19 at address 0013D4DC has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_E2FEDF19()
        {
            AssertCode("@@@", "E2FEDF19");
        }
        // Reko: a decoder for the instruction C9FF317B at address 001BFBCE has not been implemented. (AdvancedSimdTwoRegistersAndShiftAmount_opcB)
        [Test]
        public void ThumbDis_C9FF317B()
        {
            AssertCode("@@@", "C9FF317B");
        }
        // Reko: a decoder for the instruction AEF38181 at address 0017C08C has not been implemented. (ChangeProcessorState)
        [Test]
        public void ThumbDis_AEF38181()
        {
            AssertCode("@@@", "AEF38181");
        }
        // Reko: a decoder for the instruction 98EF5144 at address 0013D94A has not been implemented. (U=0)
        [Test]
        public void ThumbDis_98EF5144()
        {
            AssertCode("@@@", "98EF5144");
        }
        // Reko: a decoder for the instruction A3FFE16C at address 0017C50C has not been implemented. (Unimplemented '*' when decoding FFA36CE1)
        [Test]
        public void ThumbDis_A3FFE16C()
        {
            AssertCode("@@@", "A3FFE16C");
        }
        // Reko: a decoder for the instruction 15FC1C18 at address 0017D7DE has not been implemented. (op1:op2=0b0001)

        // Reko: a decoder for the instruction 4BFF261B at address 0017D7EC has not been implemented. (AdvancedSimd3RegistersSameLength_opcB)
        [Test]
        public void ThumbDis_4BFF261B()
        {
            AssertCode("@@@", "4BFF261B");
        }
        // Reko: a decoder for the instruction 73FE238D at address 001DEE00 has not been implemented. (AdvancedSimdTwoScalarsAndExtension)
        [Test]
        public void ThumbDis_73FE238D()
        {
            AssertCode("@@@", "73FE238D");
        }
#endif

    }
}
