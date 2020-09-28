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

using Reko.Arch.Arm;
using Reko.Core;
using Reko.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Reko.Arch.Arm.AArch32;

namespace Reko.UnitTests.Arch.Arm
{
    [TestFixture]
    public abstract class ArmTestBase
    {
        protected AArch32Instruction armInstr;

        protected static MachineInstruction Disassemble(byte[] bytes)
        {
            var image = new MemoryArea(Address.Ptr32(0x00100000), bytes);
            var dasm = new Arm32Architecture("arm32").CreateDisassembler(image.CreateLeReader(0));
            return dasm.First();
        }

        protected virtual IEnumerator<MachineInstruction> CreateDisassembler(IProcessorArchitecture arch, EndianImageReader rdr)
        {
            return arch.CreateDisassembler(rdr).GetEnumerator();
        }

        protected MachineInstruction Disassemble32(uint instr)
        {
            var image = new MemoryArea(Address.Ptr32(0x00100000), new byte[4]);
            LeImageWriter w = new LeImageWriter(image.Bytes);
            w.WriteLeUInt32(0, instr);
            var arch = CreateArchitecture();
            var dasm = CreateDisassembler(arch, image.CreateLeReader(0));
            Assert.IsTrue(dasm.MoveNext());
            this.armInstr = (AArch32Instruction)dasm.Current;
            dasm.Dispose();
            return armInstr;
        }

        protected MachineInstruction DisassembleBits(string bitPattern)
        {
            var image = new MemoryArea(Address.Ptr32(0x00100000), new byte[4]);
            LeImageWriter w = new LeImageWriter(image.Bytes);
            uint instr = ParseBitPattern(bitPattern);
            w.WriteLeUInt32(0, instr);
            var b = image.Bytes;
            //Debug.Print("Instruction bytes: {0:X2} {1:X2} {2:X2} {3:X2}", b[0], b[1], b[2], b[3]); // Spews in the unit tests
            var arch = CreateArchitecture();
            var dasm = arch.CreateDisassembler(image.CreateLeReader(0));
            return dasm.First();
        }

        protected abstract IProcessorArchitecture CreateArchitecture();

        protected static uint ParseBitPattern(string bitPattern)
        {
            int cBits = 0;
            uint instr = 0;
            for (int i = 0; i < bitPattern.Length; ++i)
            {
                switch (bitPattern[i])
                {
                case '0':
                case '1':
                    instr = (instr << 1) | (uint)(bitPattern[i] - '0');
                    ++cBits;
                    break;
                }
            }
            if (cBits != 32)
                throw new ArgumentException(
                    string.Format("Bit pattern didn't contain exactly 32 binary digits, but {0}.", cBits),
                    "bitPattern");
            return instr;
        }
    }

    [TestFixture]
    [Category(Categories.Capstone)]
    public class A32DisassemblerTests : ArmTestBase
    {
        private const string ArmObsolete = "Obsolete instrction? can't find it in ARM Architecture Reference Manual - ARMv8, for ARMv8";
        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new Arm32Architecture("arm32");
        }

        private void Expect_Code(string sExp)
        {
            Assert.AreEqual(sExp, armInstr.ToString());
        }

        [Test]
        public void ArmDasm_andseq()
        {
            var instr = Disassemble32(0x00121003);
            Assert.AreEqual("andseq\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void ArmDasm_b()
        {
            var instr = Disassemble32(0xEAFFFFFE);
            Assert.AreEqual("b\t$00100000", instr.ToString());
        }

        [Test]
        public void ArmDasm_bl()
        {
            var instr = Disassemble32(0xCBFFFFAA);
            Assert.AreEqual("blgt\t$000FFEB0", instr.ToString());
        }

        [Test]
        public void ArmDasm_Andne_rr()
        {
            var instr = Disassemble32(0x10021003);
            Assert.AreEqual("andne\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void ArmDasm_eorshs_rr()
        {
            var instr = Disassemble32(0x20321003);
            Assert.AreEqual("eorshs\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void ArmDasm_subccs_rr_lsl_3()
        {
            var instr = Disassemble32(0x30521183);
            Assert.AreEqual("subslo\tr1,r2,r3,lsl #3", instr.ToString());
        }

        [Test]
        public void ArmDasm_rsbmis_rr_lsr_32()
        {
            var instr = Disassemble32(0x40721023);
            Assert.AreEqual("rsbsmi\tr1,r2,r3,lsr #&20", instr.ToString());
        }

        [Test]
        public void ArmDasm_addpls_rr_asr_32()
        {
            var instr = Disassemble32(0x50921043);
            Assert.AreEqual("addspl\tr1,r2,r3,asr #&20", instr.ToString());
        }

        [Test]
        public void ArmDasm_adcvss_rr_rrx_32()
        {
            var instr = Disassemble32(0x60B21063);
            Assert.AreEqual("adcsvs\tr1,r2,r3,rrx", instr.ToString());
        }

        [Test]
        public void ArmDasm_sbcvcs_r1_r2_r3_lsl_r4()
        {
            var instr = Disassemble32(0x70D21413);
            Assert.AreEqual("sbcsvc\tr1,r2,r3,lsl r4", instr.ToString());
        }

        [Test]
        public void ArmDasm_rschi_r1_r2_imm7()
        {
            var instr = Disassemble32(0x82E21007u);
            Assert.AreEqual("rschi\tr1,r2,#7", instr.ToString());
        }

        [Test]
        public void ArmDasm_tstxx_r1_imm4()
        {
            var instr = Disassemble32(0x93110F01u);
            Assert.AreEqual("tstls\tr1,#4", instr.ToString());
        }

        [Test]
        public void ArmDasm_mulges_r13_r14_r15()
        {
            var instr = Disassemble32(0xA01D8F9Eu);
            Assert.AreEqual("mulsge\tsp,lr,pc", instr.ToString());
        }

        [Test]
        public void ArmDasm_mlalt_r11_r12_r13_r14()
        {
            var instr = Disassemble32(0xB02BED9Cu);
            Assert.AreEqual("mlalt\tfp,ip,sp,lr", instr.ToString());
        }

        [Test]
        public void ArmDasm_AllZeroes()
        {
            var instr = Disassemble32(0x00000000u);
            Assert.AreEqual("andeq\tr0,r0,r0", instr.ToString());
        }

        [Test]
        public void ArmDasm_strgt_r1_r2()
        {
            var instr = DisassembleBits("1100 01 011000 0010 0001 000000000000");
            Assert.AreEqual("strgt\tr1,[r2]", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldrble_r4_r6_off()
        {
            var instr = DisassembleBits("1101 01 010101 0110 0100 000100100011");
            Assert.AreEqual("ldrble\tr4,[r6,-#&123]", instr.ToString());
        }

        [Test]
        public void ArmDasm_strb_r5_r9_post_r1()
        {
            //var instr = DisassembleBits("1110 01 100100 1001 0101 00000 000 0001");
            var instr = Disassemble32(0xE6495001);
            Assert.AreEqual("strb\tr5,[r9],-r1", instr.ToString());
        }

        [Test]
        public void ArmDasm_strb_r5_r9_post_r1_lsr_3_writeback()
        {
            var instr = DisassembleBits("1110 01 110110 1001 0101 00001 000 0001");
            instr = Disassemble32(0xE7695081);
            Assert.AreEqual("strb\tr5,[r9,-r1,lsl #1]!", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldr_r5_r7_pos_off()
        {
            var instr = DisassembleBits("1101 01 011101 0111 0101 000100100011");
            Assert.AreEqual("ldrble\tr5,[r7,#&123]", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldrble_r5_r7_neg_r1()
        {
            var instr = Disassemble32(0xD7575001);
            Assert.AreEqual("ldrble\tr5,[r7,-r1]", instr.ToString());
        }

        [Test]
        public void ArmDasm_swpb()
        {
            var instr = DisassembleBits("1110 00010 100 0001 0010 00001001 0011");
            Assert.AreEqual("swpb\tr2,r3,[r1]", instr.ToString());
        }

        [Test]
        public void ArmDasm_swpb_2()
        {
            Disassemble32(0xE1409190);
            Expect_Code("swpb\tr9,r0,[r0]");
        }

        [Test]
        public void ArmDasm_cdp()
        {
            var instr = Disassemble32(0xFECED300);
            Assert.AreEqual("cdp2\tp3,#&C,cr13,cr14,cr0,#0", instr.ToString());

            instr = Disassemble32(0x4EC4EC4F);
            Assert.AreEqual("cdpmi\tp12,#&C,cr14,cr4,cr15,#2", instr.ToString());
        }

        [Test]
        public void ArmDasm_setend()
        {
            var instr = DisassembleBits("11110 0010000 000 1 00000000 0000 0000");
            Assert.AreEqual("setend\tle", instr.ToString());
            instr = DisassembleBits("11110 0010000 000 1 00000010 0000 0000");
            Assert.AreEqual("setend\tbe", instr.ToString());
        }

        [Test]
        public void ArmDasm_mov_pc_r14()
        {
            var instr = DisassembleBits("1110 00011010 0000 1111 000000001110");
            Assert.AreEqual("mov\tpc,lr", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldr()
        {
            var instr = Disassemble32(0xE5940008);
            Assert.AreEqual("ldr\tr0,[r4,#&8]", instr.ToString());
        }

        [Test]
        public void ArmDasm_lsl()
        {
            var instr = Disassemble32(0xE1A03205);
            Assert.AreEqual("lsl\tr3,r5,#4", instr.ToString());
        }

        [Test]
        public void ArmDasm_mov_2()
        {
            var instr = Disassemble32(0xE1a03000);
            Assert.AreEqual("mov\tr3,r0", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldm()
        {
            var instr = Disassemble32(0xE89B000F);
            Assert.AreEqual("ldm\tfp,{r0-r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldmia_writeback()
        {
            var instr = Disassemble32(0xE8BB000F);
            Assert.AreEqual("ldm\tfp!,{r0-r3}", instr.ToString());
        }


        [Test]
        public void ArmDasm_ldmia()
        {
            var instr = Disassemble32(0xE89B000F);
            Assert.AreEqual("ldm\tfp,{r0-r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldmia_2()
        {
            var instr = Disassemble32(0xE8BB000A);
            Assert.AreEqual("ldm\tfp!,{r1,r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_pop_many()
        {
            var instr = Disassemble32(0xE8BD000A);
            Assert.AreEqual("pop\t{r1,r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_push_one()
        {
            var instr = Disassemble32(0xE52DE004);
            Expect_Code("push\tlr");
        }

        [Test]
        public void ArmDasm_pop_one()
        {
            var instr = Disassemble32(0xE49DE004);
            Expect_Code("pop\tlr");
        }

        [Test]
        public void ArmDasm_blx()
        {
            var instr = Disassemble32(0xFB000000);
            Expect_Code("blx\t$0010000A");
        }

        [Test]
        public void ArmDasm_blx_reg()
        {
            Disassemble32(0xE12FFF35);
            Expect_Code("blx\tr5");
        }

        [Test]
        public void ArmDasm_svc()
        {
            var instr = Disassemble32(0xEF000011);
            Assert.AreEqual("svc\t#&11", instr.ToString());
        }

        [Test]
        public void ArmDasm_stm()
        {
            var instr = Disassemble32(0xE92CCFF3);
            Assert.AreEqual("stmdb\tip!,{r0-r1,r4-fp,lr-pc}", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldrsb()
        {
            var instr = Disassemble32(0xE1F322D1);
            Assert.AreEqual("ldrsb\tr2,[r3,#&21]!", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldrsb_positive_indexed()
        {
            var instr = Disassemble32(0xE19120D3);
            Assert.AreEqual("ldrsb\tr2,[r1,r3]", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldrsb_negative_indexed()
        {
            var instr = Disassemble32(0xE11120D3);
            Assert.AreEqual("ldrsb\tr2,[r1,-r3]", instr.ToString());
        }

        [Test]
        public void ArmDasm_mov()
        {
            var instr = Disassemble32(0xE3A0B000);
            Assert.AreEqual("mov\tfp,#0", instr.ToString());
        }

        [Test]
        public void ArmDasm_mov_a2_encoding()
        {
            var instr = Disassemble32(0xE30A9BCD);
            Assert.AreEqual("mov\tr9,#&ABCD", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldr_pc_relative()
        {
            var instr = Disassemble32(0xE59F0010);
            Assert.AreEqual("ldr\tr0,[pc,#&10]", instr.ToString());
        }

        [Test]
        public void ArmDasm_orr()
        {
            var instr = Disassemble32(0xe3812001);
            Assert.AreEqual("orr\tr2,r1,#1", instr.ToString());
        }

        [Test]
        public void ArmDasm_mvn()
        {
            var instr = Disassemble32(0xe3e03102);
            Assert.AreEqual("mvn\tr3,#&80000000", instr.ToString());
        }

        [Test]
        public void ArmDasm_cmn()
        {
            var instr = Disassemble32(0xE3730001);
            Assert.AreEqual("cmn\tr3,#1", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldr_post()
        {
            var instr = Disassemble32(0xE4D43001);
            Assert.AreEqual("ldrb\tr3,[r4],#&1", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldrls_pc()
        {
            var instr = Disassemble32(0x979FF103);
            Assert.AreEqual("ldrls\tpc,[pc,r3,lsl #2]", instr.ToString());
        }

        [Test]
        public void ArmDasm_sxth()
        {
            Disassemble32(0xE6BF2072);
            Expect_Code("sxth\tr2,r2");
        }

        [Test]
        public void ArmDasm_uxtab()
        {
            Disassemble32(0xE6E44076);
            Expect_Code("uxtab\tr4,r4,r6");
        }

        [Test]
        public void ArmDasm_uxtah()
        {
            Disassemble32(0xE6F45071);
            Expect_Code("uxtah\tr5,r4,r1");
        }

        [Test]
        public void ArmDasm_rev()
        {
            Disassemble32(0xE6BF2F32);
            Expect_Code("rev\tr2,r2");  //$REVIEW: check for accuracy
        }

        [Test]
        public void ArmDasm_uxth()
        {
            Disassemble32(0xE6FF8070);
            Expect_Code("uxth\tr8,r0");
        }

        [Test]
        public void ArmDasm_uxtb()
        {
            Disassemble32(0xE6EF307A);
            Expect_Code("uxtb\tr3,r10");
        }

        [Test]
        public void ArmDasm_uxtb_ror()
        {
            Disassemble32(0xE6EF347A);
            Expect_Code("uxtb\tr3,r10,ror #8");
        }

        [Test]
        public void ArmDasm_ubfxlt()
        {
            Disassemble32(0xB7E70850);
            Expect_Code("ubfxlt\tr0,r0,#&10,#8");
        }

        [Test]
        public void ArmDasm_sxtb()
        {
            Disassemble32(0xE6AF2072);
            Expect_Code("sxtb\tr2,r2");
        }

        [Test]
        public void ArmDasm_movt()
        {
            Disassemble32(0xE34001A9);
            Expect_Code("movt\tr0,#&1A9");
        }


        [Test]
        public void ArmDasm_str_preindex()
        {
            Disassemble32(0xE52D0008);
            Expect_Code("str\tr0,[sp,-#&8]!");
        }

        [Test]
        public void ArmDasm_strb_PostIndex()
        {
            Disassemble32(0xE4C23001);
            Expect_Code("strb\tr3,[r2],#&1");
        }

        [Test]
        public void ArmDasm_strb_offset()
        {
            Disassemble32(0xE5C23001);
            Expect_Code("strb\tr3,[r2,#&1]");
        }

        [Test]
        public void ArmDasm_ldrb_preindex()
        {
            Disassemble32(0xE5F12001);
            Expect_Code("ldrb\tr2,[r1,#&1]!");
        }

        [Test]
        public void ArmDasm_strb()
        {
            Disassemble32(0xE5C12000);
            Expect_Code("strb\tr2,[r1]");
        }

        [Test]
        public void ArmDasm_ldr_pc_postindex()
        {
            Disassemble32(0xE5BEF008);
            Expect_Code("ldr\tpc,[lr,#&8]!");
        }

        [Test]
        public void ArmDasm_strh_postindex()
        {
            Disassemble32(0xE000A0B8);
            Expect_Code("strh\tr10,[r0],-r8");
        }

        [Test]
        public void ArmDasm_cmp_reg_reg()
        {
            Disassemble32(0xE1530004);
            Expect_Code("cmp\tr3,r4");
        }

        [Test]
        public void ArmDasm_strd()
        {
            Disassemble32(0xE0000FFE);
            Expect_Code("strd\tr0,r1,[r0],-lr");
        }

        [Test]
        public void ArmDasm_orr_ror()
        {
            Disassemble32(0x218220E3);
            Expect_Code("orrhs\tr2,r2,r3,ror #1");
        }

        [Test]
        public void ArmDasm_cmp_reg_lsr()
        {
            Disassemble32(0xE1500121);
            Expect_Code("cmp\tr0,r1,lsr #2");
        }

        [Test]
        public void ArmDasm_tst_ror_imm()
        {
            Disassemble32(0xE11200EC);
            Expect_Code("tst\tr2,ip,ror #1");
        }

        [Test]
        public void ArmDasm_ldr_preindex()
        {
            Disassemble32(0xE5343004);
            Expect_Code("ldr\tr3,[r4,-#&4]!");
        }

        [Test]
        public void ArmDasm_vmov_imm()
        {
            Disassemble32(0xF2C00050);
            Expect_Code("vmov.i32\tq8,#0");
        }

        [Test]
        public void ArmDasm_mvn_reg()
        {
            Disassemble32(0xE1E0C003);
            Expect_Code("mvn\tip,r3");
        }

        [Test]
        public void ArmDasm_movt_imm()
        {
            Disassemble32(0xE34F2FFF);
            Expect_Code("movt\tr2,#&FFFF");
        }

        [Test]
        public void ArmDasm_bfi()
        {
            Disassemble32(0xE7C3E198);
            Expect_Code("bfi\tlr,r8,#3,#1");
        }

        [Test]
        public void ArmDasm_vstmdb()
        {
            Disassemble32(0xED2D8B02);
            Expect_Code("vstmdb\tsp!,{d8}");
        }

        [Test]
        public void ArmDasm_vmov_neg1()
        {
            Disassemble32(0xF3C70E5F);
            Expect_Code("vmov.i8\tq8,#&FFFFFFFFFFFFFFFF");
        }

        [Test]
        public void ArmDasm_ubfx()
        {
            Disassemble32(0xE7E00259);
            Expect_Code("ubfx\tr0,r9,#4,#1");
        }

        [Test]
        public void ArmDasm_vdup()
        {
            Disassemble32(0xEEE02B90);
            Expect_Code("vdup.i8\tq8,r2");
        }

        [Test]
        public void ArmDasm_vldr_sp()
        {
            Disassemble32(0xEDDD7A0B);
            Expect_Code("vldr\ts15,[sp,#&2C]");
        }

        [Test]
        public void ArmDasm_vmov_s14_r1()
        {
            Disassemble32(0xEE071A10);
            Expect_Code("vmov\ts14,r1");
        }

        [Test]
        public void ArmDasm_vldr_pc_relative()
        {
            Disassemble32(0xEDDF1BEE);
            Expect_Code("vldr\td17,[pc,#&3B8]");
        }

        [Test]
        public void ArmDasm_vldmia_d()
        {
            Disassemble32(0xECD40B04);
            Expect_Code("vldmia\tr4,{d16-d17}");
        }

        [Test]
        public void ArmDasm_vldmia_s()
        {
            Disassemble32(0xECD40A04);
            Expect_Code("vldmia\tr4,{s1-s4}");
        }

        [Test]
        public void ArmDasm_bic_reg()
        {
            Disassemble32(0xE1C12002);
            Expect_Code("bic\tr2,r1,r2");
        }

        [Test]
        public void ArmDasm_cmn_reg()
        {
            Disassemble32(0xE1700007);
            Expect_Code("cmn\tr0,r7");
        }

        [Test]
        public void ArmDasm_mvn_lsl_reg()
        {
            Disassemble32(0xE1E04514);
            Expect_Code("mvn\tr0,r4,lsl r5");
        }

        [Test]
        public void ArmDasm_txt_lsr_pc()
        {
            Disassemble32(0x011AFF34);
            Expect_Code("tsteq\tr10,r4,lsr pc");
        }

        [Test]
        public void ArmDasm_bic_asr_imm()
        {
            Disassemble32(0xE1C15FC1);
            Expect_Code("bic\tr5,r1,r1,asr #&1F");
        }

        [Test]
        public void ArmDasm_ldmda()
        {
            Disassemble32(0xE811EB85);
            Expect_Code("ldmda\tr1,{r0,r2,r7-r9,fp,sp-pc}");
        }

        [Test]
        public void ArmDasm_ldrls_pc_relative_shift()
        {
            Disassemble32(0x979FF103);   // ldrls\tpc,[pc,r3,lsl #2]
            Expect_Code("ldrls\tpc,[pc,r3,lsl #2]");
        }

        [Test]
        public void ArmDasm_vsub_f64()
        {
            Disassemble32(0xEE711BE0);
            Expect_Code("vsub.f64\td17,d17,d16");
        }

        [Test]
        public void ArmDasm_vstr()
        {
            Disassemble32(0xEDCD0B29);
            Expect_Code("vstr\td16,[sp,#&A4]");
        }

        [Test]
        public void ArmDasm_cpsid()
        {
            Disassemble32(0xF10C0080);
            Expect_Code("cpsid\t#0");
        }

        [Test]
        public void ArmDasm_dmb()
        {
            Disassemble32(0xF57FF05F);
            Expect_Code("dmb\tsy");
        }

        [Test]
        public void ArmDasm_mcr()
        {
            Disassemble32(0xEE070F58);
            Expect_Code("mcr\tp15,#0,r0,cr7,cr8,#2");
        }

        [Test]
        public void ArmDasm_mrc()
        {
            Disassemble32(0xEE123F10);
            Expect_Code("mrc\tp15,#0,r3,cr2,cr0,#0");
        }

        [Test]
        public void ArmDasm_sbfx()
        {
            Disassemble32(0xE7A9C35C);
            Expect_Code("sbfx\tip,ip,#6,#&A");
        }

        [Test]
        public void ArmDasm_smlabb()
        {
            Disassemble32(0xE10E3B88);
            Expect_Code("smlabb\tlr,r8,fp,r3");
        }

        [Test]
        public void ArmDasm_smlabt()
        {
            Disassemble32(0xE10F54CC);
            Expect_Code("smlabt\tpc,ip,r4,r5");
        }

        [Test]
        public void ArmDasm_smlalbb()
        {
            Disassemble32(0xE1409280);
            Expect_Code("smlalbb\tr9,r0,r0,r2");
        }

        [Test]
        public void ArmDasm_smlalbt()
        {
            Disassemble32(0xE14090C0);
            Expect_Code("smlalbt\tr9,r0,r0,r0");
        }

        [Test]
        public void ArmDasm_smlaltb()
        {
            Disassemble32(0xE14091A0);
            Expect_Code("smlaltb\tr9,r0,r0,r1");
        }

        [Test]
        public void ArmDasm_smlaltt()
        {
            Disassemble32(0xE140ABEC);
            Expect_Code("smlaltt\tr10,r0,ip,fp");
        }

        [Test]
        public void ArmDasm_smlatb()
        {
            Disassemble32(0xE10C6CA0);
            Expect_Code("smlatb\tip,r0,ip,r6");
        }


        [Test]
        public void ArmDasm_smlatteq()
        {
            Disassemble32(0x010BDAE4);
            Expect_Code("smlatteq\tfp,r4,r10,sp");
        }

        [Test]
        public void ArmDasm_smlawb()
        {
            Disassemble32(0xE12D5980);
            Expect_Code("smlawb\tsp,r0,r9,r5");
        }

        [Test]
        public void ArmDasm_smlawt()
        {
            Disassemble32(0x012D06CC);
            Expect_Code("smlawteq\tsp,ip,r6,r0");
        }

        [Test]
        public void ArmDasm_smulbb()
        {
            Disassemble32(0xE1600380);
            Expect_Code("smulbb\tr0,r0,r3");
        }

        [Test]
        public void ArmDasm_smulbt()
        {
            Disassemble32(0xE168DBCC);
            Expect_Code("smulbt\tr8,ip,fp");
        }

        [Test]
        public void ArmDasm_smultb()
        {
            Disassemble32(0xE16C69AC);
            Expect_Code("smultb\tip,ip,r9");
        }

        [Test]
        public void ArmDasm_smultt()
        {
            Disassemble32(0xE168DBE0);
            Expect_Code("smultt\tr8,r0,fp");
        }

        [Test]
        public void ArmDasm_smulwb()
        {
            Disassemble32(0xE12E5BA8);
            Expect_Code("smulwb\tlr,r8,fp");
        }

        [Test]
        public void ArmDasm_smulwt()
        {
            Disassemble32(0xE1206AEC);
            Expect_Code("smulwt\tr0,ip,r10");
        }

        [Test]
        public void ArmDasm_vabs_f64()
        {
            Disassemble32(0xEEB09BC9);
            Expect_Code("vabs.f64\td9,d9");
        }

        [Test]
        public void ArmDasm_vcmp_f32()
        {
            Disassemble32(0xEEF47A47);
            Expect_Code("vcmp.f32\ts15,s14");
        }

        [Test]
        public void ArmDasm_vcmpe()
        {
            Disassemble32(0xEEB49AE7);
            Expect_Code("vcmpe.f32\ts18,s15");
        }

        [Test]
        public void ArmDasm_vcvt_f64_s32()
        {
            Disassemble32(0xEEF80BE7);
            Expect_Code("vcvt.f64.s32\td16,s15");
        }

        [Test]
        public void ArmDasm_vext_8()
        {
            Disassemble32(0xF2F068E2);
            Expect_Code("vext.u8\tq11,q8,q9,#8");
        }

        [Test]
        public void ArmDasm_vext_64()
        {
            Disassemble32(0xF2F208C8);
            Expect_Code("vext.u8\tq8,q9,q4,#8");
        }

        [Test]
        public void ArmDasm_vshr_3()
        {
            Disassemble32(0xF3F340D0);
            Expect_Code("vshr.i64\tq10,q0,#&D");
        }

        [Test]
        public void ArmDasm_vmax_s32()
        {
            Disassemble32(0xF26006E2);
            Expect_Code("vmax.s32\tq8,q8,q9");
        }

        [Test]
        public void ArmDasm_vmin_s32()
        {
            Disassemble32(0xF26446F0);
            Expect_Code("vmin.s32\tq10,q10,q8");
        }

        [Test]
        public void ArmDasm_vmrs()
        {
            Disassemble32(0xEEF1FA10);
            Expect_Code("vmrs\tpc,#1");
        }

        [Test]
        public void ArmDasm_vneg_f64()
        {
            Disassemble32(0xEEF10B60);
            Expect_Code("vneg.f64\td16,d16");
        }

        [Test]
        public void ArmDasm_vpadd_i32()
        {
            Disassemble32(0xF2622BB2);
            Expect_Code("vpadd.i32\td18,d18,d18");
        }

        [Test]
        public void ArmDasm_vpmax()
        {
            Disassemble32(0xF2600AA0);
            Expect_Code("vpmax.s32\td16,d16,d16");
        }

        [Test]
        public void ArmDasm_vpmax_u16()
        {
            Disassemble32(0xF3522AA2);
            Expect_Code("vpmax.u16\td18,d18,d18");
        }

        [Test]
        public void ArmDasm_vpmin_s32()
        {
            Disassemble32(0xF2644AB4);
            Expect_Code("vpmin.s32\td20,d20,d20");
        }

        [Test]
        public void ArmDasm_vshl_u32()
        {
            Disassemble32(0xF36424E2);
            Expect_Code("vshl.u32\tq9,q9,q10");
        }

        [Test]
        public void ArmDasm_vsqrt_f64()
        {
            Disassemble32(0xEEB1CBE0);
            Expect_Code("vsqrt.f64\td12,d16");
        }

        [Test]
        public void ArmDasm_vstmia_upd()
        {
            Disassemble32(0xECE30B04);
            Expect_Code("vstmia\tr3!,{d16-d17}");
        }

        [Test]
        public void ArmDasm_vstmia()
        {
            Disassemble32(0xECC00B04);
            Expect_Code("vstmia\tr0,{d16-d17}");
        }
        
        [Test]
        public void ArmDasm_invalid_vstmia()
        {
            Disassemble32(0x0CA0BBC0);
            Expect_Code("Invalid");
        }

        [Test]
        public void ArmDasm_ldrbtgt()
        {
            Disassemble32(0xC47A0000);
            Expect_Code("ldrbtgt\tr0,[r10]");
        }

        [Test]
        public void ArmDasm_ldrsht()
        {
            Disassemble32(0xE0FE50FC);
            Expect_Code("ldrsht\tr5,[lr],#&C");
        }

        [Test]
        public void ArmDasm_ldrd()
        {
            Disassemble32(0xE1C722D8);
            Expect_Code("ldrd\tr2,r3,[r7,#&28]");
        }

        [Test]
        public void ArmDasm_ldrht()
        {
            Disassemble32(0xE0FD52B4);
            Expect_Code("ldrht\tr5,[sp],#&24");
        }

        [Test]
        public void ArmDasm_ldrt()
        {
            Disassemble32(0x44340000);
            Expect_Code("ldrtmi\tr0,[r4]");
        }

        [Test]
        public void ArmDasm_strd_r3()
        {
            Disassemble32(0xE04343F8);
            Expect_Code("strd\tr4,r5,[r3],-#&38");
        }

        [Test]
        public void ArmDasm_strht()
        {
            Disassemble32(0xE0E051B0);
            Expect_Code("strht\tr5,[r0],#&10");
        }

        [Test]
        public void ArmDasm_sxtab()
        {
            Disassemble32(0xE6A55078);
            Expect_Code("sxtab\tr5,r5,r8");
        }

        [Test]
        public void ArmDasm_sxtah()
        {
            Disassemble32(0xE6B6A07A);
            Expect_Code("sxtah\tr10,r6,r10");
        }

        [Test]
        public void ArmDasm_vadd_f64()
        {
            Disassemble32(0xEE377B20);
            Expect_Code("vadd.f64\td7,d7,d16");
        }

        [Test]
        public void ArmDasm_vdiv_f64()
        {
            Disassemble32(0xEE817BA0);
            Expect_Code("vdiv.f64\td7,d17,d16");
        }

        [Test]
        public void ArmDasm_vmla()
        {
            Disassemble32(0xEE476A86);
            Expect_Code("vmla.f32\ts13,s15,s12");
        }

        [Test]
        public void ArmDasm_EE017BE0()
        {
            Disassemble32(0xEE017BE0);
            Expect_Code("vmls.f64\td7,d17,d16");
        }

        [Test]
        public void ArmDasm_vmov_i32()
        {
            Disassemble32(0xEE102B90);
            Expect_Code("vmov.i32\tr2,d16[0]");
        }

        [Test]
        public void ArmDasm_vmvn_imm()
        {
            Disassemble32(0xF2C04077);
            Expect_Code("vmvn.i32\tq10,#7");
        }

        [Test]
        public void ArmDasm_vnmls_f32()
        {
            Disassemble32(0xEE567A87);
            Expect_Code("vnmls.f32\ts15,s13,s14");
        }

        [Test]
        public void ArmDasm_msr()
        {
            Disassemble32(0xE121F001);
            Expect_Code("msr\tcpsr,r1");
        }

        [Test]
        public void ArmDasm_qadd()
        {
            Disassemble32(0xE10FB85C);
            Expect_Code("qadd\tfp,ip,pc");
        }

        [Test]
        public void ArmDasm_qdadd()
        {
            Disassemble32(0x01408E50);
            Expect_Code("qdaddeq\tr8,r0,r0");
        }

        [Test]
        public void ArmDasm_qdsub()
        {
            Disassemble32(0xE168DA50);
            Expect_Code("qdsub\tsp,r0,r8");
        }

        [Test]
        public void ArmDasm_qsub()
        {
            Disassemble32(0xE12D6650);
            Expect_Code("qsub\tr6,r0,sp");
        }



        [Test]
        public void ArmDasm_msr_imm()
        {
            Disassemble32(0xA36A6BDD);
            Expect_Code("msrge\tspsr,#&BDD");
        }

        [Test]
        public void ArmDasm_ldr_literal()
        {
            Disassemble32(0xE59F5254);
            Expect_Code("ldr\tr5,[pc,#&254]");
        }

        [Test]
        public void ArmDasm_strh()
        {
            Disassemble32(0xE1C320B0);
            Expect_Code("strh\tr2,[r3]");
        }

        [Test]
        public void ArmDasm_ldrh()
        {
            Disassemble32(0xE1D041BC);
            Expect_Code("ldrh\tr4,[r0,#&1C]");
        }

        [Test]
        public void ArmDasm_strh_pre_imm()
        {
            Disassemble32(0xE16230B2);
            Expect_Code("strh\tr3,[r2,-#&2]!");
        }

        [Test]
        public void ArmDasm_stm_user()
        {
            Disassemble32(0x39435246);
            Expect_Code("stmlo\tr3,{r1-r2,r6,r9,ip,lr}^");
        }

        [Test]
        public void ArmDasm_mrrc()
        {
            Disassemble32(0xEC5F5554);
            Expect_Code("mrrc\tp5,#5,r5,pc,cr4");
        }

        [Test]
        public void ArmDasm_ldm_user()
        {
            Disassemble32(0x3958414D);
            Expect_Code("ldmlo\tr8,{r0,r2-r3,r6,r8,lr}^");
        }

        [Test]
        public void ArmDasm_usax()
        {
            Disassemble32(0x36535054);
            Expect_Code("usaxlo\tr5,r3,r4");
        }

        [Test]
        public void ArmDasm_eret()
        {
            Disassemble32(0xE167B760);
            Expect_Code("eret");
        }

        [Test]
        public void ArmDasm_smc()
        {
            Disassemble32(0xE167C970);
            Expect_Code("smc\t#0");
        }

        [Test]
        public void ArmDasm_smlsldx()
        {
            Disassemble32(0x47434774);
            Expect_Code("smlsldxmi\tr3,r4,r7,r4");
        }

        [Test]
        public void ArmDasm_strh_imm_pre()
        {
            Disassemble32(0x0167C9F0);
            Expect_Code("ldrsheq\tip,[r7,-#&90]!");
        }

        [Test]
        public void ArmDasm_msr_banked_register()
        {
            Disassemble32(0x0167B70C);
            Expect_Code("msreq\tr11_usr,ip");
        }

        [Test]
        public void ArmDasm_qsub16()
        {
            Disassemble32(0x66206174);
            Expect_Code("qsub16vs\tr6,r0,r4");
        }

        [Test]
        public void ArmDasm_uqasxvs()
        {
            Disassemble32(0x66664F3A);
            Expect_Code("uqasxvs\tr4,r6,r10");
        }

        [Test]
        public void ArmDasm_ldrh_imm()
        {
            Disassemble32(0x00DA85B8);
            Expect_Code("ldrheq\tr8,[r10],#&58");
        }

        [Test]
        public void ArmDasm_ldrsh_imm()
        {
            Disassemble32(0x00DAC7F0);
            Expect_Code("ldrsheq\tip,[r10],#&70");
        }

        [Test]
        public void ArmDasm_hvc()
        {
            Disassemble32(0xE14C7472);
            Expect_Code("hvc\t#&C742");
        }

        [Test]
        public void ArmDasm_stc()
        {
            Disassemble32(0x2C645520);
            Expect_Code("stchs\tp5,cr5,[r4],-#&80");
        }

        [Test]
        public void ArmDasm_bkpt()
        {
            Disassemble32(0x01262B70);
            Expect_Code("bkpteq\t#&62B0");
        }

        [Test]
        public void ArmDasm_vmov_neg()
        {
            Disassemble32(0xF2C14E3E);
            Expect_Code("vmov.i64\td20,#&FFFFFFFF00");
        }

        [Test]
        public void ArmDasm_wfe_eq()
        {
            Disassemble32(0x03200002);
            Expect_Code("wfeeq");
        }

        [Test]
        public void ArmDasm_ssub8()
        {
            Disassemble32(0x161803F3);
            Expect_Code("ssub8ne\tr0,r8,r3");
        }

        [Test]
        public void ArmDasm_shsub8()
        {
            Disassemble32(0xE63D01F8);
            Expect_Code("shsub8\tr0,sp,r8");
        }

        [Test]
        public void ArmDasm_ldc()
        {
            Disassemble32(0x0CBF140F);
            Expect_Code("ldceq\tp4,cr1,[pc],#&3C");
        }

        [Test]
        public void ArmDasm_ldrsh_invalid_writeback()
        {
            Disassemble32(0x01FF4EF7);
            Expect_Code("Invalid");
        }

        [Test]
        public void ArmDasm_shsub8_vs()
        {
            Disassemble32(0x663501F8);
            Expect_Code("shsub8vs\tr0,r5,r8");
        }

        [Test]
        public void ArmDasm_uhsub8_eq()
        {
            Disassemble32(0x06782BF0);
            Expect_Code("uhsub8eq\tr2,r8,r0");
        }

        [Test]
        public void ArmDasm_vstr_s()
        {
            Disassemble32(0xBD4A1AE7);
            Expect_Code("vstrlt\ts3,[r10,-#&39C]");
        }

        [Test]
        public void ArmDasm_usat()
        {
            Disassemble32(0xE6E38012);
            Expect_Code("usat\tr8,#3,r2");
        }

        [Test]
        public void ArmDasm_vstr_2()
        {
            Disassemble32(0x0D465B6D);
            Expect_Code("vstreq\td21,[r6,-#&1B4]");
        }

        [Test]
        public void ArmDasm_vstmdb_lt()
        {
            Disassemble32(0xBD672A19);
            Expect_Code("vstmdblt\tr7!,{s5-s29}");
        }

        [Test]
        public void ArmDasm_vaddl_u32()
        {
            Disassemble32(0xF3AF8000);
            Expect_Code("vaddl.u32\tq4,d15,d0");
        }

        [Test]
        public void ArmDasm_vmov_imm_2()
        {
            Disassemble32(0xF2C14E3E);
            Expect_Code("vmov.i64\td20,#&FFFFFFFF00");
        }

        [Test]
        public void ArmDasm_vmax_u16()
        {
            Disassemble32(0xF35226E0);
            Expect_Code("vmax.u16\tq9,q9,q8");
        }

        [Test]
        public void ArmDasm_vcvt_f32_u32()
        {
            Disassemble32(0xEEB86A44);
            Expect_Code("vcvt.f32.u32\ts12,s8");
        }

        [Test]
        public void ArmDasm_vmoveq_f32()
        {
            Disassemble32(0x0EF07A47);
            Expect_Code("vmoveq.f32\ts15,s14");
        }

        [Test]
        public void ArmDasm_vcvtr_s32_f32()
        {
            Disassemble32(0xEEFD9AE9);
            Expect_Code("vcvtr.s32.f32\ts19,s19");
        }

        [Test]
        public void ArmDasm_vmov_f32_16()
        {
            Disassemble32(0xEEB37A00);
            Expect_Code("vmov.f32\ts14,#16.0F");
        }

        [Test]
        public void ArmDasm_vcvtr_u32_f32()
        {
            Disassemble32(0xEEFC7AE7);
            Expect_Code("vcvtr.u32.f32\ts15,s15");
        }

        [Test]
        public void ArmDasm_vcvt_f64_f32()
        {
            Disassemble32(0xEEF70AC7);
            Expect_Code("vcvt.f64.f32\td16,s14");
        }

        [Test]
        public void ArmDasm_vmov_i16_imm()
        {
            Disassemble32(0xF2C20A50);
            Expect_Code("vmov.i16\tq8,#&2000200020002000");
        }

        [Test]
        public void ArmDasm_vldr_s()
        {
            Disassemble32(0xED1B9A6C);
            Expect_Code("vldr\ts18,[fp,-#&1B0]");
        }

        [Test]
        public void ArmDasm_vmov_f64()
        {
            Disassemble32(0xEEB07B4A);
            Expect_Code("vmov.f64\td7,d10");
        }

        [Test]
        public void ArmDasm_vcmpe_f64_0()
        {
            Disassemble32(0xEEB59BC0);
            Expect_Code("vcmpe.f64\td9,#0.0");
        }

        [Test]
        public void ArmDasm_vmov_f64_30()
        {
            Disassemble32(0xEEB39B0E);
            Expect_Code("vmov.f64\td9,#30.0");
        }

        [Test]
        public void ArmDasm_vmov_f64_0_5()
        {
            Disassemble32(0xEEF61B00);
            Expect_Code("vmov.f64\td17,#0.5");
        }

        [Test]
        public void ArmDasm_vadd_i32_q()
        {
            Disassemble32(0xF26028E4);
            Expect_Code("vadd.i32\tq9,q8,q10");
        }

        [Test]
        public void ArmDasm_vmovgt_f64_m1()
        {
            Disassemble32(0xCEFF0B00);
            Expect_Code("vmovgt.f64\td16,#-1.0");
        }

        [Test]
        public void ArmDasm_vcmpe_f64_d_0()
        {
            Disassemble32(0xEEF50BC0);
            Expect_Code("vcmpe.f64\td16,#0.0");
        }

        [Test]
        public void ArmDasm_vmov_f64_m1()
        {
            Disassemble32(0xEEBF8B00);
            Expect_Code("vmov.f64\td8,#-1.0");
        }

        [Test]
        public void ArmDasm_vcmpe_f32_0()
        {
            Disassemble32(0xEEB58AC0);
            Expect_Code("vcmpe.f32\ts16,#0.0F");
        }

        [Test]
        public void ArmDasm_ldc2l()
        {
            Disassemble32(0xFDF001A9);
            Expect_Code("ldc2l\tp1,cr0,[r0,#&2A4]!");
        }

        [Test]
        public void ArmDasm_mrc2()
        {
            Disassemble32(0xFEB068F8);
            Expect_Code("mrc2\tp8,#5,r6,cr0,cr8,#7");
        }

        [Test]
        public void ArmDasm_stc2()
        {
            Disassemble32(0xFD0080EB);
            Expect_Code("stc2\tp0,cr8,[r0,-#&3AC]");
        }

        [Test]
        public void ArmDasm_vld4_32()
        {
            Disassemble32(0xF42B009B);
            Expect_Code("vld4.i32\t{d0-d3},[fp:64],fp");
        }

        [Test]
        public void ArmDasm_stc2l()
        {
            Disassemble32(0xFC684C25);
            Expect_Code("stc2l\tp12,cr4,[r8],-#&94");
        }

        [Test]
        public void ArmDasm_vbit()
        {
            Disassemble32(0xF320019B);
            Expect_Code("vbit\td0,d16,d11");
        }

        [Test]
        public void ArmDasm_vld4_i16()
        {
            Disassemble32(0xF42B006A);
            Expect_Code("vld4.i16\t{d0-d3},[fp:128],r10");
        }

        [Test]
        public void ArmDasm_vld2_8()
        {
            Disassemble32(0xF4663303);
            Expect_Code("vld2.i8\t{d19-d22},[r6],r3");
        }

        [Test]
        public void ArmDasm_vst3_16()
        {
            Disassemble32(0xF4000547);
            Expect_Code("vst3.i16\t{d0,d2,d4,d6},[r0],r7");
        }

        [Test]
        public void ArmDasm_vhadd_u32_d()
        {
            Disassemble32(0xF3640009);
            Expect_Code("vhadd.u32\td16,d4,d9");
        }

        [Test]
        public void ArmDasm_vmov_u8_indexed()
        {
            Disassemble32(0xEED01B90);
            Expect_Code("vmov.u8\tr1,d16[0]");
        }

        [Test]
        public void ArmDasm_vsub_i16_q()
        {
            Disassemble32(0xF35008E2);
            Expect_Code("vsub.i16\tq8,q8,q9");
        }

        [Test]
        public void ArmDasm_vmov_d_rr()
        {
            Disassemble32(0xEC432B17);
            Expect_Code("vmov\td7,r2,r3");
        }

        [Test]
        public void ArmDasm_vmov_rr_d()
        {
            Disassemble32(0xEC510B16);
            Expect_Code("vmov\tr0,r1,d6");
        }

        [Test]
        public void ArmDasm_stchs()
        {
            Disassemble32(0x2D207325);
            Expect_Code("stchs\tp3,cr7,[r0,-#&94]!");
        }

        [Test]
        public void ArmDasm_pld()
        {
            Disassemble32(0xF5D0F020);
            Expect_Code("pld\t[r0,#&20]");
        }


        //@@@@ make rewriters for these

        [Test]
        public void ArmDasm_vmax_f32()
        {
            Disassemble32(0xF2000F0B);
            Expect_Code("vmax.f32\td0,d0,d11");
        }

        [Test]
        public void ArmDasm_vrhadd_u8()
        {
            Disassemble32(0xF30DF1A5);
            Expect_Code("vrhadd.u8\td15,d29,d21");
        }

        [Test]
        public void ArmDasm_vext_u8()
        {
            Disassemble32(0xF2F5F1A5);
            Expect_Code("vext.u8\td31,d21,d21,#1");
        }

        [Test]
        public void ArmDasm_vshr_u8()
        {
            Disassemble32(0xF3882030);
            Expect_Code("vshr.i8\td2,d16,#8");
        }

        [Test]
        public void ArmDasm_vshr_1()
        {
            Disassemble32(0xF3892010);
            Expect_Code("vshr.i8\td2,d0,#7");
        }

        [Test]
        public void ArmDasm_vsli()
        {
            Disassemble32(0xF3EFA570);
            Expect_Code("vsli.i32\tq13,q8,#&F");
        }

        [Test]
        public void ArmDasm_4614B538()
        {
            Disassemble32(0x4614B538);
            Expect_Code("sasxmi\tfp,r4,r8");
        }

        [Test]
        public void ArmDasm_shsub16()
        {
            Disassemble32(0x4630447C);
            Expect_Code("shsub16mi\tr4,r0,ip");
        }

        [Test]
        public void ArmDasm_F3934620()
        {
            Disassemble32(0xF3934620);
            Expect_Code("vrsubhn.i32\td4,q1,q8");
        }

        [Test]
        public void ArmDasm_pkhbt()
        {
            Disassemble32(0xE68B9D92);
            Expect_Code("pkhbt\tr9,fp,r2,lsl #&1B");
        }

        [Test]
        public void ArmDasm_vfnma()
        {
            Disassemble32(0x0E92EBC3);
            Expect_Code("vfnmaeq.f64\td14,d18,d3");
        }

        [Test]
        public void ArmDasm_uadd16()
        {
            Disassemble32(0x4658E019);
            Expect_Code("uadd16mi\tlr,r8,r9");
        }

        [Test]
        public void ArmDasm_vfma()
        {
            Disassemble32(0x0EEDFB02);
            Expect_Code("vfmaeq.f64\td31,d13,d2");
        }

        [Test]
        public void ArmDasm_stl()
        {
            Disassemble32(0x0180F894);
            Expect_Code("stleq\tr4,[r0]");
        }

        [Test]
        public void ArmDasm_stlex()
        {
            Disassemble32(0x01850293);
            Expect_Code("stlexeq\tr0,r3,[r5]");
        }

        [Test]
        public void ArmDasm_ldaex()
        {
            Disassemble32(0x019B0293);
            Expect_Code("ldaexeq\tr0,[fp]");
        }

        [Test]
        public void ArmDasm_ssub16()
        {
            Disassemble32(0x0618D07E);
            Expect_Code("ssub16eq\tsp,r8,lr");
        }

        [Test]
        public void ArmDasm_yield()
        {
            Disassemble32(0xE320F101);
            Expect_Code("yield");
        }

        [Test]
        public void ArmDasm_ssat()
        {
            Disassemble32(0xE6B80693);
            Expect_Code("ssat\tr0,#&18,r3,lsl #&D");
        }

        [Test]
        public void ArmDasm_stlh()
        {
            Disassemble32(0xE1E4F895);
            Expect_Code("stlh\tr5,[r4]");
        }

        [Test]
        public void ArmDasm_sdiv()
        {
            Disassemble32(0xE718E014);
            Expect_Code("sdiv\tr8,r4,r0");
        }

        [Test]
        public void ArmDasm_smlad()
        {
            Disassemble32(0xE7040414);
            Expect_Code("smlad\tr4,r4,r4,r0");
        }

        [Test]
        public void ArmDasm_smladx()
        {
            Disassemble32(0xE708E03A);
            Expect_Code("smladx\tr8,r10,r0,lr");
        }

        [Test]
        public void ArmDasm_smlsd()
        {
            Disassemble32(0xE708E050);
            Expect_Code("smlsd\tr8,r0,r0,lr");
        }

        [Test]
        public void ArmDasm_smlsdx()
        {
            Disassemble32(0xE708BE78);
            Expect_Code("smlsdx\tr8,r8,lr,fp");
        }

        [Test]
        public void ArmDasm_smmla()
        {
            Disassemble32(0xE75AE016);
            Expect_Code("smmla\tr10,r6,r0,lr");
        }

        [Test]
        public void ArmDasm_smmlar()
        {
            Disassemble32(0xE750E036);
            Expect_Code("smmlar\tr0,r6,r0,lr");
        }

        [Test]
        public void ArmDasm_smmlsr()
        {
            Disassemble32(0xE75846F0);
            Expect_Code("smmlsr\tr8,r0,r6,r4");
        }

        [Test]
        public void ArmDasm_sxtab16()
        {
            Disassemble32(0xE688D379);
            Expect_Code("sxtab16\tsp,r8,r9");
        }

        [Test]
        public void ArmDasm_usada8()
        {
            Disassemble32(0xE7816818);
            Expect_Code("usada8\tr1,r8,r8,r6");
        }

        [Test]
        public void ArmDasm_vmov_from_2_gp_regs_to_2_single_floats()
        {
            Disassemble32(0xEC4B0A12);
            Expect_Code("vmov\ts4,s5,r0,fp");
        }

        [Test]
        public void ArmDasm_sev()
        {
            Disassemble32(0xE3209804);
            Expect_Code("sev");
        }

        [Test]
        public void ArmDasm_sevl()
        {
            Disassemble32(0xE3209805);
            Expect_Code("sevl");
        }

        [Test]
        public void ArmDasm_uhsax()
        {
            Disassemble32(0xE67A2652);
            Expect_Code("uhsax\tr2,r10,r2");
        }

        [Test]
        public void ArmDasm_shadd8()
        {
            Disassemble32(0xE6304999);
            Expect_Code("shadd8\tr4,r0,r9");
        }

        [Test]
        public void ArmDasm_shasx()
        {
            Disassemble32(0xE634493C);
            Expect_Code("shasx\tr4,r4,ip");
        }

        [Test]
        public void ArmDasm_uadd8()
        {
            Disassemble32(0xE6524995);
            Expect_Code("uadd8\tr4,r2,r5");
        }

        [Test]
        public void ArmDasm_sxtb16()
        {
            Disassemble32(0xE68F4979);
            Expect_Code("sxtb16\tr4,pc,r9,ror #&10");
        }

        [Test]
        public void ArmDasm_shsax()
        {
            Disassemble32(0xE6300055);
            Expect_Code("shsax\tr0,r0,r5");
        }

        [Test]
        public void ArmDasm_sel()
        {
            Disassemble32(0xE68CF6B2);
            Expect_Code("sel");
        }

        [Test]
        public void ArmDasm_vcvtt_f32_f16()
        {
            Disassemble32(0xEEB20AC0);
            Expect_Code("vcvtt.f32.f16\ts0,s0");
        }

        [Test]
        public void ArmDasm_vcvtb_f64_f16()
        {
            Disassemble32(0xEEB23B4C);
            Expect_Code("vcvtb.f64.f16\td3,s24");
        }

        [Test]
        public void ArmDasm_vrintx_f64()
        {
            Disassemble32(0xEEB71B41);
            Expect_Code("vrintx.f64\td1,d1");
        }

        [Test]
        public void ArmDasm_vmov_u16_indexed()
        {
            Disassemble32(0xEEB81B30);
            Expect_Code("vmov.u16\tr1,d8[2]");
        }

        [Test]
        public void ArmDasm_vmov_16_indexed()
        {
            Disassemble32(0xEE04EBB6);
            Expect_Code("vmov.i16\td20[0],lr");
        }

        [Test]
        public void ArmDasm_ldrexb()
        {
            Disassemble32(0xE1D64293);
            Expect_Code("ldrexb\tr4,[r6]");
        }

        [Test]
        public void ArmDasm_ldrexh()
        {
            Disassemble32(0xE1F3429D);
            Expect_Code("ldrexh\tr4,[r3]");
        }

        [Test]
        public void ArmDasm_vmov_from_2_floats()
        {
            Disassemble32(0xEC514A16);
            Expect_Code("vmov\tr4,r1,s12,s13");
        }

        [Test]
        public void ArmDasm_uxtab16()
        {
            Disassemble32(0xE6C04778);
            Expect_Code("uxtab16\tr4,r0,r8,ror #8");
        }

        [Test]
        public void ArmDasm_vfnms_f64()
        {
            Disassemble32(0xEED9EBA1);
            Expect_Code("vfnms.f64\td30,d25,d17");
        }

        [Test]
        public void ArmDasm_vmla_i8()
        {
            Disassemble32(0xF2002901);
            Expect_Code("vmla.i8\td2,d0,d1");
        }

        [Test]
        public void ArmDasm_vrecps_f32()
        {
            Disassemble32(0xF2000F7C);
            Expect_Code("vrecps.f32\tq0,q0,q14");
        }

        [Test]
        public void ArmDasm_vmul_i8()
        {
            Disassemble32(0xF2002913);
            Expect_Code("vmul.i8\td2,d0,d3");
        }

        [Test]
        public void ArmDasm_vfma_f32()
        {
            Disassemble32(0xF2002C15);
            Expect_Code("vfma.f32\td2,d0,d5");
        }

        [Test]
        public void ArmDasm_vrhadd()
        {
            Disassemble32(0xF2449100);
            Expect_Code("vrhadd.s8\td25,d4,d0");
        }

        [Test]
        public void ArmDasm_vqshlu_i64()
        {
            Disassemble32(0xF3E8F7B4);
            Expect_Code("vqshlu.i64\td31,d20,#&28");
        }

        [Test]
        public void ArmDasm_vabal()
        {
            Disassemble32(0xF2814583);
            Expect_Code("vabal.s8\tq2,d17,d3");
        }

        [Test]
        public void ArmDasm_vbic_i32()
        {
            Disassemble32(0xF2814571);
            Expect_Code("vbic.i32\tq2,#&11000000110000");
        }

        [Test]
        public void ArmDasm_vqshl()
        {
            Disassemble32(0xF2E8F710);
            Expect_Code("vqshl.i32\td31,d0,#8");
        }

        [Test]
        public void ArmDasm_vqadd_u8()
        {
            Disassemble32(0xF3058010);
            Expect_Code("vqadd.u8\td8,d5,d0");
        }

        [Test]
        public void ArmDasm_vhsub_u8()
        {
            Disassemble32(0xF3014284);
            Expect_Code("vhsub.u8\td4,d17,d4");
        }

        [Test]
        public void ArmDasm_vacgt_f32()
        {
            Disassemble32(0xF3650E3F);
            Expect_Code("vacgt.f32\td16,d5,d31");
        }

        //////////////////////////

#if BORED
        /// If you're bored and want something to do, why not implement a 
        /// A32 decoder or 10? :)

        // An A32 decoder for the instruction F4E7F270 (AdvancedSimdElementLoadStore) has not been implemented yet.
        [Test]
        public void ArmDasm_F4E7F270()
        {
            Disassemble32(0xF4E7F270);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction F4E7B940 (AdvancedSimdElementLoadStore) has not been implemented yet.
        [Test]
        public void ArmDasm_F4E7B940()
        {
            Disassemble32(0xF4E7B940);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction F4E7F370 (AdvancedSimdElementLoadStore) has not been implemented yet.
        [Test]
        public void ArmDasm_F4E7F370()
        {
            Disassemble32(0xF4E7F370);
            Expect_Code("@@@");
        }

#endif

    }
}