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
    public class ArmDisassemblerTests : ArmTestBase
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
        [Ignore(ArmObsolete)]
        public void ArmDasm_swpb()
        {
            var instr = DisassembleBits("1110 00010 100 0001 0010 00001001 0011");
            Assert.AreEqual("swpb\tr2,r3,[r1]", instr.ToString());
        }


        [Test]
        [Ignore("Can't seem to find a definition for this in ARM docs?")]
        public void ArmDasm_cdp()
        {
            var instr = Disassemble32(0xFECED300);
            Assert.AreEqual("cdp2\tp3,#&C,c13,c14", instr.ToString());

            instr = Disassemble32(0x4EC4EC4F);
            Assert.AreEqual("cdpmi\tp12,#&C,c14,c4", instr.ToString());
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
            Assert.AreEqual("lsl\tr3,r5,lsl #4", instr.ToString());
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
            Assert.AreEqual("ldm\tfp!,{r0-r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldmia()
        {
            var instr = Disassemble32(0xE8BB000F);
            Assert.AreEqual("ldm\tfp!,{r0-r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldmia_2()
        {
            var instr = Disassemble32(0xE8BB000A);
            Assert.AreEqual("ldm\tfp!,{r1,r3}", instr.ToString());
        }

        [Test]
        public void ArmDasm_blx()
        {
            var instr = Disassemble32(0xFB000000);
            Assert.AreEqual("blx\t$0010000A", instr.ToString());
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

        ////////////////////////////////////////////////////////////////////////////

        [Test]
        public void ArmDasm_movt()
        {
            Disassemble32(0xE34001A9);
            Expect_Code("movt\tr0,#&1A9");
        }


        [Test]
        public void ArmDasm_str_preindex()
        {
            Disassemble32(0xE52D0004);
            Expect_Code("str\tr0,[sp,-#&4]!");
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
            Expect_Code("strd\tr0,[r0],-lr");
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

        
        // An A32 decoder for the instruction E6AF3073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF3073()
        {
            Disassemble32(0xE6AF3073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF2073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF2073()
        {
            Disassemble32(0xE6AF2073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF2072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF2072()
        {
            Disassemble32(0xE6EF2072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF2073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF2073()
        {
            Disassemble32(0xE6EF2073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction F2C00050 (AdvancedSimd) has not been implemented yet.
        [Test]
        public void ArmDasm_F2C00050()
        {
            Disassemble32(0xF2C00050);
            Expect_Code("@@@");
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

        
        // An A32 decoder for the instruction E6FF2072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF2072()
        {
            Disassemble32(0xE6FF2072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF3073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF3073()
        {
            Disassemble32(0xE6FF3073);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E6EF4074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF4074()
        {
            Disassemble32(0xE6EF4074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E1E00002 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1E00002()
        {
            Disassemble32(0xE1E00002);
            Expect_Code("@@@");
        }
        
        
        
        
        // An A32 decoder for the instruction E6EF1071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1071()
        {
            Disassemble32(0xE6EF1071);
            Expect_Code("@@@");
        }
                
        // An A32 decoder for the instruction ECF20B04 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ECF20B04()
        {
            Disassemble32(0xECF20B04);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ECFC0B04 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ECFC0B04()
        {
            Disassemble32(0xECFC0B04);
            Expect_Code("@@@");
        }

        
        
        
        
        
        
                
        // An A32 decoder for the instruction E7E7C450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E7C450()
        {
            Disassemble32(0xE7E7C450);
            Expect_Code("@@@");
        }

        
        
        
        
        
        
        
        // An A32 decoder for the instruction E6AF3070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF3070()
        {
            Disassemble32(0xE6AF3070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF5F30 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF5F30()
        {
            Disassemble32(0xE6BF5F30);
            Expect_Code("@@@");
        }

        
                
        
        
        // An A32 decoder for the instruction E6FF1077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF1077()
        {
            Disassemble32(0xE6FF1077);
            Expect_Code("@@@");
        }

        
        
        
        // An A32 decoder for the instruction ED2D8B04 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED2D8B04()
        {
            Disassemble32(0xED2D8B04);
            Expect_Code("@@@");
        }

                
        // An A32 decoder for the instruction E6AF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0070()
        {
            Disassemble32(0xE6AF0070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF0073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0073()
        {
            Disassemble32(0xE6AF0073);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6EF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0070()
        {
            Disassemble32(0xE6EF0070);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction E6EF3073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3073()
        {
            Disassemble32(0xE6EF3073);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF1074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1074()
        {
            Disassemble32(0xE6EF1074);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E33254 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E33254()
        {
            Disassemble32(0xE7E33254);
            Expect_Code("@@@");
        }

        
        
        
        
        // An A32 decoder for the instruction E6AF0076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0076()
        {
            Disassemble32(0xE6AF0076);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6AF0074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0074()
        {
            Disassemble32(0xE6AF0074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF007A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF007A()
        {
            Disassemble32(0xE6AF007A);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction EE070A90 (AdvancedSIMDandFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EE070A90()
        {
            Disassemble32(0xEE070A90);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF7076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF7076()
        {
            Disassemble32(0xE6FF7076);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF1077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1077()
        {
            Disassemble32(0xE6EF1077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF1075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1075()
        {
            Disassemble32(0xE6EF1075);
            Expect_Code("@@@");
        }

        
        
        
                
        // An A32 decoder for the instruction ED2D8B02 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED2D8B02()
        {
            Disassemble32(0xED2D8B02);
            Expect_Code("@@@");
        }

        
                // An A32 decoder for the instruction E7E72450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E72450()
        {
            Disassemble32(0xE7E72450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E73850 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E73850()
        {
            Disassemble32(0xE7E73850);
            Expect_Code("@@@");
        }

        
        
        
        
                // An A32 decoder for the instruction E6BF0073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF0073()
        {
            Disassemble32(0xE6BF0073);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6BF0075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF0075()
        {
            Disassemble32(0xE6BF0075);
            Expect_Code("@@@");
        }
        
                
        
        // An A32 decoder for the instruction F3C70E5F (AdvancedSimd) has not been implemented yet.
        [Test]
        public void ArmDasm_F3C70E5F()
        {
            Disassemble32(0xF3C70E5F);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF3070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3070()
        {
            Disassemble32(0xE6EF3070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF2072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF2072()
        {
            Disassemble32(0xE6AF2072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E0B1DB (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0B1DB()
        {
            Disassemble32(0xE7E0B1DB);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF3072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF3072()
        {
            Disassemble32(0xE6AF3072);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF5075()
        {
            Disassemble32(0xE6EF5075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF2077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF2077()
        {
            Disassemble32(0xE6AF2077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF9077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF9077()
        {
            Disassemble32(0xE6EF9077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF8078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF8078()
        {
            Disassemble32(0xE6EF8078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C52191 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C52191()
        {
            Disassemble32(0xE7C52191);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 07C33192 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_07C33192()
        {
            Disassemble32(0x07C33192);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C21013 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C21013()
        {
            Disassemble32(0xE7C21013);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C22011 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C22011()
        {
            Disassemble32(0xE7C22011);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E6EF8077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF8077()
        {
            Disassemble32(0xE6EF8077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E0715B (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0715B()
        {
            Disassemble32(0xE7E0715B);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF0075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0075()
        {
            Disassemble32(0xE6AF0075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 06EF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_06EF5075()
        {
            Disassemble32(0x06EF5075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C4E21F (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C4E21F()
        {
            Disassemble32(0xE7C4E21F);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C1E01C (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C1E01C()
        {
            Disassemble32(0xE7C1E01C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C12011 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C12011()
        {
            Disassemble32(0xE7C12011);
            Expect_Code("@@@");
        }

        
                // An A32 decoder for the instruction ECF50B04 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ECF50B04()
        {
            Disassemble32(0xECF50B04);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E00259 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E00259()
        {
            Disassemble32(0xE7E00259);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E02258 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E02258()
        {
            Disassemble32(0xE7E02258);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF7073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF7073()
        {
            Disassemble32(0xE6EF7073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EEE02B90 (AdvancedSIMDandFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EEE02B90()
        {
            Disassemble32(0xEEE02B90);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6AF4070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF4070()
        {
            Disassemble32(0xE6AF4070);
            Expect_Code("@@@");
        }

                // An A32 decoder for the instruction 96EF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_96EF5075()
        {
            Disassemble32(0x96EF5075);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF7077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF7077()
        {
            Disassemble32(0xE6EF7077);
            Expect_Code("@@@");
        }

                
        
        
        
        
        
                // An A32 decoder for the instruction E6EF3071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3071()
        {
            Disassemble32(0xE6EF3071);
            Expect_Code("@@@");
        }

        
        
        
        
        
        // An A32 decoder for the instruction E7E5C450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E5C450()
        {
            Disassemble32(0xE7E5C450);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E12256 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E12256()
        {
            Disassemble32(0xE7E12256);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction EC410B31 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EC410B31()
        {
            Disassemble32(0xEC410B31);
            Expect_Code("@@@");
        }

                
        
        
        // An A32 decoder for the instruction E6EFC074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFC074()
        {
            Disassemble32(0xE6EFC074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7EE00D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EE00D0()
        {
            Disassemble32(0xE7EE00D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF9078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF9078()
        {
            Disassemble32(0xE6EF9078);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction E7ED0850 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7ED0850()
        {
            Disassemble32(0xE7ED0850);
            Expect_Code("@@@");
        }
        
        
        
                // An A32 decoder for the instruction E6EF6076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF6076()
        {
            Disassemble32(0xE6EF6076);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF207C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF207C()
        {
            Disassemble32(0xE6EF207C);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EFC075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFC075()
        {
            Disassemble32(0xE6EFC075);
            Expect_Code("@@@");
        }

                // An A32 decoder for the instruction E7C0301F (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C0301F()
        {
            Disassemble32(0xE7C0301F);
            Expect_Code("@@@");
        }

        
                // An A32 decoder for the instruction 17C0301F (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_17C0301F()
        {
            Disassemble32(0x17C0301F);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6FF1072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF1072()
        {
            Disassemble32(0xE6FF1072);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction E6EF9079 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF9079()
        {
            Disassemble32(0xE6EF9079);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF6070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF6070()
        {
            Disassemble32(0xE6EF6070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF307C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF307C()
        {
            Disassemble32(0xE6EF307C);
            Expect_Code("@@@");
        }

        
        
                
                // An A32 decoder for the instruction E7E440D4 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E440D4()
        {
            Disassemble32(0xE7E440D4);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E77456 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E77456()
        {
            Disassemble32(0xE7E77456);
            Expect_Code("@@@");
        }

        
        
        
                
        // An A32 decoder for the instruction E7C2211F (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C2211F()
        {
            Disassemble32(0xE7C2211F);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C22091 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C22091()
        {
            Disassemble32(0xE7C22091);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6EF0075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0075()
        {
            Disassemble32(0xE6EF0075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF0074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0074()
        {
            Disassemble32(0xE6EF0074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C2C09E (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C2C09E()
        {
            Disassemble32(0xE7C2C09E);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF0072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0072()
        {
            Disassemble32(0xE6EF0072);
            Expect_Code("@@@");
        }

        
        
        
        // An A32 decoder for the instruction E7C21090 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C21090()
        {
            Disassemble32(0xE7C21090);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction ECF00B04 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ECF00B04()
        {
            Disassemble32(0xECF00B04);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EFC07C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFC07C()
        {
            Disassemble32(0xE6EFC07C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF0070()
        {
            Disassemble32(0xE6BF0070);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E791D2 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E791D2()
        {
            Disassemble32(0xE7E791D2);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF4074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF4074()
        {
            Disassemble32(0xE6FF4074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF0076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0076()
        {
            Disassemble32(0xE6EF0076);
            Expect_Code("@@@");
        }

        
        
        
                // An A32 decoder for the instruction E6AFA07B (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AFA07B()
        {
            Disassemble32(0xE6AFA07B);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E3402078 (Format missing for ARM instruction movt.) has not been implemented yet.
        [Test]

        public void ArmDasm_E3402078()
        {
            Disassemble32(0xE3402078);
            Expect_Code("@@@");
        }

        
        
        
        // An A32 decoder for the instruction E1C35005 (Format missing for ARM instruction bic.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1C35005()
        {
            Disassemble32(0xE1C35005);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E7C70011 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C70011()
        {
            Disassemble32(0xE7C70011);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF6074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF6074()
        {
            Disassemble32(0xE6EF6074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction D6EF6074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_D6EF6074()
        {
            Disassemble32(0xD6EF6074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7EB1652 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EB1652()
        {
            Disassemble32(0xE7EB1652);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 26EF6074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_26EF6074()
        {
            Disassemble32(0x26EF6074);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction E6FF1075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF1075()
        {
            Disassemble32(0xE6FF1075);
            Expect_Code("@@@");
        }

        
        
                
        
        
        // An A32 decoder for the instruction 07C51294 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_07C51294()
        {
            Disassemble32(0x07C51294);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EFE074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFE074()
        {
            Disassemble32(0xE6EFE074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C63314 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C63314()
        {
            Disassemble32(0xE7C63314);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7C23114 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C23114()
        {
            Disassemble32(0xE7C23114);
            Expect_Code("@@@");
        }

        
        
        
        // An A32 decoder for the instruction E7E01351 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E01351()
        {
            Disassemble32(0xE7E01351);
            Expect_Code("@@@");
        }

                
        // An A32 decoder for the instruction 17E04354 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_17E04354()
        {
            Disassemble32(0x17E04354);
            Expect_Code("@@@");
        }

                
        
        // An A32 decoder for the instruction E7DF3810 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7DF3810()
        {
            Disassemble32(0xE7DF3810);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 37DF3813 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_37DF3813()
        {
            Disassemble32(0x37DF3813);
            Expect_Code("@@@");
        }

        
                
        // An A32 decoder for the instruction E6EF5073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF5073()
        {
            Disassemble32(0xE6EF5073);
            Expect_Code("@@@");
        }

        
        
                
                // An A32 decoder for the instruction E7E32650 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E32650()
        {
            Disassemble32(0xE7E32650);
            Expect_Code("@@@");
        }
        
                // An A32 decoder for the instruction E6FF2074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF2074()
        {
            Disassemble32(0xE6FF2074);
            Expect_Code("@@@");
        }

        
        
        
        
        // An A32 decoder for the instruction E6EF5074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF5074()
        {
            Disassemble32(0xE6EF5074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EFA07A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFA07A()
        {
            Disassemble32(0xE6EFA07A);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF4074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF4074()
        {
            Disassemble32(0xE6AF4074);
            Expect_Code("@@@");
        }

        
        
        
        
        
        
        // An A32 decoder for the instruction E6FFB07B (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFB07B()
        {
            Disassemble32(0xE6FFB07B);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C12091 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C12091()
        {
            Disassemble32(0xE7C12091);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C02011 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C02011()
        {
            Disassemble32(0xE7C02011);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C03016 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C03016()
        {
            Disassemble32(0xE7C03016);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7C33018 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C33018()
        {
            Disassemble32(0xE7C33018);
            Expect_Code("@@@");
        }

        
                
        
        
        
        // An A32 decoder for the instruction E7E0235E (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0235E()
        {
            Disassemble32(0xE7E0235E);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction B6EF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_B6EF5075()
        {
            Disassemble32(0xB6EF5075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF1073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1073()
        {
            Disassemble32(0xE6EF1073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0070()
        {
            Disassemble32(0xE6FF0070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FFC07C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFC07C()
        {
            Disassemble32(0xE6FFC07C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF5071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF5071()
        {
            Disassemble32(0xE6EF5071);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction 26EF0073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_26EF0073()
        {
            Disassemble32(0x26EF0073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF3074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF3074()
        {
            Disassemble32(0xE6AF3074);
            Expect_Code("@@@");
        }

        
                
        // An A32 decoder for the instruction E7DF7819 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7DF7819()
        {
            Disassemble32(0xE7DF7819);
            Expect_Code("@@@");
        }

        
        
        
        
        
        
        // An A32 decoder for the instruction E7DFC815 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7DFC815()
        {
            Disassemble32(0xE7DFC815);
            Expect_Code("@@@");
        }

        
        
        
        // An A32 decoder for the instruction E7E000D9 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E000D9()
        {
            Disassemble32(0xE7E000D9);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E6FF0075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0075()
        {
            Disassemble32(0xE6FF0075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF0076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0076()
        {
            Disassemble32(0xE6FF0076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF4070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF4070()
        {
            Disassemble32(0xE6EF4070);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction EDDD7A0B (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EDDD7A0B()
        {
            Disassemble32(0xEDDD7A0B);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF3075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3075()
        {
            Disassemble32(0xE6EF3075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C63312 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C63312()
        {
            Disassemble32(0xE7C63312);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 07C4321E (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_07C4321E()
        {
            Disassemble32(0x07C4321E);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 17E000D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_17E000D0()
        {
            Disassemble32(0x17E000D0);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7C02013 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C02013()
        {
            Disassemble32(0xE7C02013);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C3E198 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C3E198()
        {
            Disassemble32(0xE7C3E198);
            Expect_Code("@@@");
        }

        
                        
        
        
        // An A32 decoder for the instruction E7C32091 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C32091()
        {
            Disassemble32(0xE7C32091);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C3209A (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C3209A()
        {
            Disassemble32(0xE7C3209A);
            Expect_Code("@@@");
        }

                // An A32 decoder for the instruction 17C41212 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_17C41212()
        {
            Disassemble32(0x17C41212);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E0C15C (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0C15C()
        {
            Disassemble32(0xE7E0C15C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E0E35C (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0E35C()
        {
            Disassemble32(0xE7E0E35C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EE071A10 (AdvancedSIMDandFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EE071A10()
        {
            Disassemble32(0xEE071A10);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E03153 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E03153()
        {
            Disassemble32(0xE7E03153);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EDDF1BEE (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EDDF1BEE()
        {
            Disassemble32(0xEDDF1BEE);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E11450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E11450()
        {
            Disassemble32(0xE7E11450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EE070A10 (AdvancedSIMDandFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EE070A10()
        {
            Disassemble32(0xEE070A10);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 06EFC07C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_06EFC07C()
        {
            Disassemble32(0x06EFC07C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E00254 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E00254()
        {
            Disassemble32(0xE7E00254);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6FF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF5075()
        {
            Disassemble32(0xE6FF5075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FFA07A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFA07A()
        {
            Disassemble32(0xE6FFA07A);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF0072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0072()
        {
            Disassemble32(0xE6AF0072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF9077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF9077()
        {
            Disassemble32(0xE6FF9077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF0078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0078()
        {
            Disassemble32(0xE6FF0078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF0077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0077()
        {
            Disassemble32(0xE6FF0077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 16EF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16EF0070()
        {
            Disassemble32(0x16EF0070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF8070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF8070()
        {
            Disassemble32(0xE6EF8070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF1070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1070()
        {
            Disassemble32(0xE6EF1070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF8072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF8072()
        {
            Disassemble32(0xE6EF8072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6F45071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6F45071()
        {
            Disassemble32(0xE6F45071);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EFB07E (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFB07E()
        {
            Disassemble32(0xE6EFB07E);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF0078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0078()
        {
            Disassemble32(0xE6EF0078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF3072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3072()
        {
            Disassemble32(0xE6EF3072);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction 16EF1071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16EF1071()
        {
            Disassemble32(0x16EF1071);
            Expect_Code("@@@");
        }
                // An A32 decoder for the instruction E6FF1071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF1071()
        {
            Disassemble32(0xE6FF1071);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF4073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF4073()
        {
            Disassemble32(0xE6EF4073);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E18350 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E18350()
        {
            Disassemble32(0xE7E18350);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E1C25005 (Format missing for ARM instruction bic.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1C25005()
        {
            Disassemble32(0xE1C25005);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ECD40B04 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ECD40B04()
        {
            Disassemble32(0xECD40B04);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF3074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3074()
        {
            Disassemble32(0xE6EF3074);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E421D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E421D0()
        {
            Disassemble32(0xE7E421D0);
            Expect_Code("@@@");
        }

        
        
        
        // An A32 decoder for the instruction 06EF1070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_06EF1070()
        {
            Disassemble32(0x06EF1070);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction D6FF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_D6FF0070()
        {
            Disassemble32(0xD6FF0070);
            Expect_Code("@@@");
        }

                // An A32 decoder for the instruction E7DF3814 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7DF3814()
        {
            Disassemble32(0xE7DF3814);
            Expect_Code("@@@");
        }

        
                // An A32 decoder for the instruction E7DF2814 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7DF2814()
        {
            Disassemble32(0xE7DF2814);
            Expect_Code("@@@");
        }

        [Test]
        public void ArmDasm_bic_reg()
        {
            Disassemble32(0xE1C12002);
            Expect_Code("bic\tr2,r1,r2");
        }

        // An A32 decoder for the instruction E6BF1079 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF1079()
        {
            Disassemble32(0xE6BF1079);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6BF1071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF1071()
        {
            Disassemble32(0xE6BF1071);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF7079 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF7079()
        {
            Disassemble32(0xE6EF7079);
            Expect_Code("@@@");
        }

                // An A32 decoder for the instruction E6BF2072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF2072()
        {
            Disassemble32(0xE6BF2072);
            Expect_Code("@@@");
        }
        
        
        // An A32 decoder for the instruction 41E00D80 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_41E00D80()
        {
            Disassemble32(0x41E00D80);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6FFC073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFC073()
        {
            Disassemble32(0xE6FFC073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 41E07D87 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_41E07D87()
        {
            Disassemble32(0x41E07D87);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF6073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF6073()
        {
            Disassemble32(0xE6FF6073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF4073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF4073()
        {
            Disassemble32(0xE6FF4073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF307C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF307C()
        {
            Disassemble32(0xE6FF307C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E1E0700C (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1E0700C()
        {
            Disassemble32(0xE1E0700C);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction 41E08E88 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_41E08E88()
        {
            Disassemble32(0x41E08E88);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E73852 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E73852()
        {
            Disassemble32(0xE7E73852);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E77854 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E77854()
        {
            Disassemble32(0xE7E77854);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E75852 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E75852()
        {
            Disassemble32(0xE7E75852);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E76851 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E76851()
        {
            Disassemble32(0xE7E76851);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E75851 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E75851()
        {
            Disassemble32(0xE7E75851);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E12157 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E12157()
        {
            Disassemble32(0xE7E12157);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7FA4154 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7FA4154()
        {
            Disassemble32(0xE7FA4154);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7FA9159 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7FA9159()
        {
            Disassemble32(0xE7FA9159);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7FAB151 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7FAB151()
        {
            Disassemble32(0xE7FAB151);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7FA1151 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7FA1151()
        {
            Disassemble32(0xE7FA1151);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6BF0F30 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF0F30()
        {
            Disassemble32(0xE6BF0F30);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EFB07B (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFB07B()
        {
            Disassemble32(0xE6EFB07B);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E023D3 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E023D3()
        {
            Disassemble32(0xE7E023D3);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6EF2070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF2070()
        {
            Disassemble32(0xE6EF2070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 41E03D83 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_41E03D83()
        {
            Disassemble32(0x41E03D83);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF3070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF3070()
        {
            Disassemble32(0xE6FF3070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E73450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E73450()
        {
            Disassemble32(0xE7E73450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF7077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF7077()
        {
            Disassemble32(0xE6FF7077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7EF0450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EF0450()
        {
            Disassemble32(0xE7EF0450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EFB079 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFB079()
        {
            Disassemble32(0xE6EFB079);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E73455 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E73455()
        {
            Disassemble32(0xE7E73455);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E72457 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E72457()
        {
            Disassemble32(0xE7E72457);
            Expect_Code("@@@");
        }

        [Test]
        public void ArmDasm_cmn_reg()
        {
            Disassemble32(0xE1700007);
            Expect_Code("cmn\tr0,r7");
        }

        // An A32 decoder for the instruction E6FF2076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF2076()
        {
            Disassemble32(0xE6FF2076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF5076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF5076()
        {
            Disassemble32(0xE6EF5076);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6FF2073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF2073()
        {
            Disassemble32(0xE6FF2073);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction E1E03003 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1E03003()
        {
            Disassemble32(0xE1E03003);
            Expect_Code("@@@");
        }

        
        
        
                
        
        
        // An A32 decoder for the instruction E7C1309F (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C1309F()
        {
            Disassemble32(0xE7C1309F);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E030D3 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E030D3()
        {
            Disassemble32(0xE7E030D3);
            Expect_Code("@@@");
        }

        
        
        
        
        
        
        
        
        // An A32 decoder for the instruction E6FFB073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFB073()
        {
            Disassemble32(0xE6FFB073);
            Expect_Code("@@@");
        }

        
                        
        
                        // An A32 decoder for the instruction E6FF0073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0073()
        {
            Disassemble32(0xE6FF0073);
            Expect_Code("@@@");
        }

        
        
        
        
        // An A32 decoder for the instruction E6FF1076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF1076()
        {
            Disassemble32(0xE6FF1076);
            Expect_Code("@@@");
        }
        
        
        // An A32 decoder for the instruction E6BF1F31 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF1F31()
        {
            Disassemble32(0xE6BF1F31);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF3F33 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF3F33()
        {
            Disassemble32(0xE6BF3F33);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF0078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF0078()
        {
            Disassemble32(0xE6BF0078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF8078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF8078()
        {
            Disassemble32(0xE6FF8078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7EF30D3 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EF30D3()
        {
            Disassemble32(0xE7EF30D3);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 16FF3074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16FF3074()
        {
            Disassemble32(0x16FF3074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF4071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF4071()
        {
            Disassemble32(0xE6FF4071);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction B6BF1071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_B6BF1071()
        {
            Disassemble32(0xB6BF1071);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction C6BF1071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_C6BF1071()
        {
            Disassemble32(0xC6BF1071);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E6FF4072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF4072()
        {
            Disassemble32(0xE6FF4072);
            Expect_Code("@@@");
        }
        
        [Test]
        public void ArmDasm_E3403458()
        {
            Disassemble32(0xE3403458);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF3073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF3073()
        {
            Disassemble32(0xE6BF3073);
            Expect_Code("@@@");
        }

        [Test]
        public void ArmDasm_E340B0C5()
        {
            Disassemble32(0xE340B0C5);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 16BF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16BF0070()
        {
            Disassemble32(0x16BF0070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 16BFA07A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16BFA07A()
        {
            Disassemble32(0x16BFA07A);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction B1C2000C (Format missing for ARM instruction bic.) has not been implemented yet.
        [Test]
        public void ArmDasm_B1C2000C()
        {
            Disassemble32(0xB1C2000C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF5073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF5073()
        {
            Disassemble32(0xE6FF5073);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6BF2F32 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF2F32()
        {
            Disassemble32(0xE6BF2F32);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E37057 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E37057()
        {
            Disassemble32(0xE7E37057);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction 86EF4074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_86EF4074()
        {
            Disassemble32(0x86EF4074);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction 86EF6076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_86EF6076()
        {
            Disassemble32(0x86EF6076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E33053 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E33053()
        {
            Disassemble32(0xE7E33053);
            Expect_Code("@@@");
        }

        
        
        // An A32 decoder for the instruction E7E32052 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E32052()
        {
            Disassemble32(0xE7E32052);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6EF7070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF7070()
        {
            Disassemble32(0xE6EF7070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 06EF8078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_06EF8078()
        {
            Disassemble32(0x06EF8078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E38058 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E38058()
        {
            Disassemble32(0xE7E38058);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF507A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF507A()
        {
            Disassemble32(0xE6EF507A);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E31051 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E31051()
        {
            Disassemble32(0xE7E31051);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF8075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF8075()
        {
            Disassemble32(0xE6EF8075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 86EF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_86EF0070()
        {
            Disassemble32(0x86EF0070);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction 86EF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_86EF5075()
        {
            Disassemble32(0x86EF5075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF807A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF807A()
        {
            Disassemble32(0xE6EF807A);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EFA070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFA070()
        {
            Disassemble32(0xE6EFA070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF2079 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF2079()
        {
            Disassemble32(0xE6EF2079);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF1076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1076()
        {
            Disassemble32(0xE6EF1076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF6078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF6078()
        {
            Disassemble32(0xE6EF6078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C31015 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C31015()
        {
            Disassemble32(0xE7C31015);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF0077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF0077()
        {
            Disassemble32(0xE6EF0077);
            Expect_Code("@@@");
        }
        
        
        
        // An A32 decoder for the instruction E6EFE07E (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EFE07E()
        {
            Disassemble32(0xE6EFE07E);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF5075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF5075()
        {
            Disassemble32(0xE6AF5075);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E101D4 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E101D4()
        {
            Disassemble32(0xE7E101D4);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E1C37007 (Format missing for ARM instruction bic.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1C37007()
        {
            Disassemble32(0xE1C37007);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E1CC7007 (Format missing for ARM instruction bic.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1CC7007()
        {
            Disassemble32(0xE1CC7007);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E7C73392 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C73392()
        {
            Disassemble32(0xE7C73392);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF5070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF5070()
        {
            Disassemble32(0xE6EF5070);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6AFC070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AFC070()
        {
            Disassemble32(0xE6AFC070);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6EF2076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF2076()
        {
            Disassemble32(0xE6EF2076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF207C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF207C()
        {
            Disassemble32(0xE6AF207C);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E71450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E71450()
        {
            Disassemble32(0xE7E71450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF5070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF5070()
        {
            Disassemble32(0xE6AF5070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF0078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF0078()
        {
            Disassemble32(0xE6AF0078);
            Expect_Code("@@@");
        }

                
                
                // An A32 decoder for the instruction E1E04514 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1E04514()
        {
            Disassemble32(0xE1E04514);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF3078 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3078()
        {
            Disassemble32(0xE6EF3078);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF9079 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF9079()
        {
            Disassemble32(0xE6AF9079);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7D73718 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7D73718()
        {
            Disassemble32(0xE7D73718);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7C57293 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C57293()
        {
            Disassemble32(0xE7C57293);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6FF7073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF7073()
        {
            Disassemble32(0xE6FF7073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF8070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF8070()
        {
            Disassemble32(0xE6FF8070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF6076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF6076()
        {
            Disassemble32(0xE6FF6076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF1072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF1072()
        {
            Disassemble32(0xE6AF1072);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6FF4076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF4076()
        {
            Disassemble32(0xE6FF4076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E76450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E76450()
        {
            Disassemble32(0xE7E76450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF2070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF2070()
        {
            Disassemble32(0xE6FF2070);
            Expect_Code("@@@");
        }

        
                // An A32 decoder for the instruction EC410B15 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EC410B15()
        {
            Disassemble32(0xEC410B15);
            Expect_Code("@@@");
        }

        
        
        
        
        // An A32 decoder for the instruction E7E00155 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E00155()
        {
            Disassemble32(0xE7E00155);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E7345A (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E7345A()
        {
            Disassemble32(0xE7E7345A);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E625D2 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E625D2()
        {
            Disassemble32(0xE7E625D2);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF207A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF207A()
        {
            Disassemble32(0xE6FF207A);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF2077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF2077()
        {
            Disassemble32(0xE6FF2077);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction 11E05C80 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_11E05C80()
        {
            Disassemble32(0x11E05C80);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 11E05000 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_11E05000()
        {
            Disassemble32(0x11E05000);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E1E05009 (Format missing for ARM instruction mvn.) has not been implemented yet.
        [Test]
        public void ArmDasm_E1E05009()
        {
            Disassemble32(0xE1E05009);
            Expect_Code("@@@");
        }

        [Test]
        public void ArmDasm_txt_lsr_pc()
        {
            Disassemble32(0x011AFF34);
            Expect_Code("tsteq\tr10,r4,lsr pc");
        }

        
        // An A32 decoder for the instruction E6FF6070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF6070()
        {
            Disassemble32(0xE6FF6070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF1075 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF1075()
        {
            Disassemble32(0xE6BF1075);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E6FF8073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF8073()
        {
            Disassemble32(0xE6FF8073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FFE073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFE073()
        {
            Disassemble32(0xE6FFE073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FFE070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFE070()
        {
            Disassemble32(0xE6FFE070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FFB070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFB070()
        {
            Disassemble32(0xE6FFB070);
            Expect_Code("@@@");
        }

        
                // An A32 decoder for the instruction EC432B17 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EC432B17()
        {
            Disassemble32(0xEC432B17);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED2D8B08 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED2D8B08()
        {
            Disassemble32(0xED2D8B08);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED947AA7 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED947AA7()
        {
            Disassemble32(0xED947AA7);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED957A20 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED957A20()
        {
            Disassemble32(0xED957A20);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED2D8B10 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED2D8B10()
        {
            Disassemble32(0xED2D8B10);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EE060A10 (AdvancedSIMDandFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EE060A10()
        {
            Disassemble32(0xEE060A10);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED2D8B06 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED2D8B06()
        {
            Disassemble32(0xED2D8B06);
            Expect_Code("@@@");
        }

        [Test]
        public void ArmDasm_E1E0C00C()
        {
            Disassemble32(0xE1E0C00C);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E75452 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E75452()
        {
            Disassemble32(0xE7E75452);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 16EF3073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16EF3073()
        {
            Disassemble32(0x16EF3073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF307A (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF307A()
        {
            Disassemble32(0xE6EF307A);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E001D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E001D0()
        {
            Disassemble32(0xE7E001D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EDDD3A30 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EDDD3A30()
        {
            Disassemble32(0xEDDD3A30);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED957A00 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED957A00()
        {
            Disassemble32(0xED957A00);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AFC07C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AFC07C()
        {
            Disassemble32(0xE6AFC07C);
            Expect_Code("@@@");
        }
                // An A32 decoder for the instruction E7E02153 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E02153()
        {
            Disassemble32(0xE7E02153);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FFA070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FFA070()
        {
            Disassemble32(0xE6FFA070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 96EF0070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_96EF0070()
        {
            Disassemble32(0x96EF0070);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E210D2 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E210D2()
        {
            Disassemble32(0xE7E210D2);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction ED2D8B0E (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_ED2D8B0E()
        {
            Disassemble32(0xED2D8B0E);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C2301F (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C2301F()
        {
            Disassemble32(0xE7C2301F);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EC410B17 (SystemRegister_LdSt_64bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EC410B17()
        {
            Disassemble32(0xEC410B17);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E30250 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E30250()
        {
            Disassemble32(0xE7E30250);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E37256 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E37256()
        {
            Disassemble32(0xE7E37256);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF3076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF3076()
        {
            Disassemble32(0xE6FF3076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction EE058A10 (AdvancedSIMDandFloatingPoint32bitMove) has not been implemented yet.
        [Test]
        public void ArmDasm_EE058A10()
        {
            Disassemble32(0xEE058A10);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E31251 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E31251()
        {
            Disassemble32(0xE7E31251);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6AF7077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF7077()
        {
            Disassemble32(0xE6AF7077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E71452 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E71452()
        {
            Disassemble32(0xE7E71452);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7EBA65A (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EBA65A()
        {
            Disassemble32(0xE7EBA65A);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7ED91D9 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7ED91D9()
        {
            Disassemble32(0xE7ED91D9);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF1072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF1072()
        {
            Disassemble32(0xE6EF1072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C1C093 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C1C093()
        {
            Disassemble32(0xE7C1C093);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AFA076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AFA076()
        {
            Disassemble32(0xE6AFA076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF0071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF0071()
        {
            Disassemble32(0xE6FF0071);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF6070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF6070()
        {
            Disassemble32(0xE6AF6070);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction 16BF6076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16BF6076()
        {
            Disassemble32(0x16BF6076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF4072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF4072()
        {
            Disassemble32(0xE6EF4072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E70450 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E70450()
        {
            Disassemble32(0xE7E70450);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C73212 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C73212()
        {
            Disassemble32(0xE7C73212);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E7E74452 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E74452()
        {
            Disassemble32(0xE7E74452);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E7E0C0D7 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0C0D7()
        {
            Disassemble32(0xE7E0C0D7);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7C02018 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7C02018()
        {
            Disassemble32(0xE7C02018);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E6EF407C (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF407C()
        {
            Disassemble32(0xE6EF407C);
            Expect_Code("@@@");
        }
        
        // An A32 decoder for the instruction E7F73453 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7F73453()
        {
            Disassemble32(0xE7F73453);
            Expect_Code("@@@");
        }
        
        
        // An A32 decoder for the instruction E7E0E0DC (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E0E0DC()
        {
            Disassemble32(0xE7E0E0DC);
            Expect_Code("@@@");
        }

        
        // An A32 decoder for the instruction E6E44076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6E44076()
        {
            Disassemble32(0xE6E44076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6AF6076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6AF6076()
        {
            Disassemble32(0xE6AF6076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 16FF4072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_16FF4072()
        {
            Disassemble32(0x16FF4072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF7071 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF7071()
        {
            Disassemble32(0xE6EF7071);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E4C0D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E4C0D0()
        {
            Disassemble32(0xE7E4C0D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6BF5070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6BF5070()
        {
            Disassemble32(0xE6BF5070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF2074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF2074()
        {
            Disassemble32(0xE6EF2074);
            Expect_Code("@@@");
        }
        // An A32 decoder for the instruction E6FF5072 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF5072()
        {
            Disassemble32(0xE6FF5072);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF3076 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF3076()
        {
            Disassemble32(0xE6EF3076);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction B7E70850 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_B7E70850()
        {
            Disassemble32(0xB7E70850);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E03253 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E03253()
        {
            Disassemble32(0xE7E03253);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF1070 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF1070()
        {
            Disassemble32(0xE6FF1070);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E007D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E007D0()
        {
            Disassemble32(0xE7E007D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E00950 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E00950()
        {
            Disassemble32(0xE7E00950);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6EF4077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6EF4077()
        {
            Disassemble32(0xE6EF4077);
            Expect_Code("@@@");
        }

        
        
        
                
        
        // An A32 decoder for the instruction E7E002D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E002D0()
        {
            Disassemble32(0xE7E002D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E000D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E000D0()
        {
            Disassemble32(0xE7E000D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7E00150 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E00150()
        {
            Disassemble32(0xE7E00150);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction 07E00854 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_07E00854()
        {
            Disassemble32(0x07E00854);
            Expect_Code("@@@");
        }

        [Test]
        public void ArmDasm_bic_asr_imm()
        {
            Disassemble32(0xE1C15FC1);
            Expect_Code("bic\tr5,r1,r1,asr #&1F");
        }

        // An A32 decoder for the instruction E7E12252 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7E12252()
        {
            Disassemble32(0xE7E12252);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction A6FF3073 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_A6FF3073()
        {
            Disassemble32(0xA6FF3073);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF3074 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF3074()
        {
            Disassemble32(0xE6FF3074);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7EF30D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EF30D0()
        {
            Disassemble32(0xE7EF30D0);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E6FF6077 (media1) has not been implemented yet.
        [Test]
        public void ArmDasm_E6FF6077()
        {
            Disassemble32(0xE6FF6077);
            Expect_Code("@@@");
        }

        // An A32 decoder for the instruction E7EF00D0 (media3) has not been implemented yet.
        [Test]
        public void ArmDasm_E7EF00D0()
        {
            Disassemble32(0xE7EF00D0);
            Expect_Code("@@@");
        }

                
        
        
            }
}