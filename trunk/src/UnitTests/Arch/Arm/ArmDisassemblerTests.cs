#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Arch.Arm;
using Decompiler.Core;
using Decompiler.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Arm
{
    public abstract class ArmTestBase
    {
        protected static ArmInstruction Disassemble(byte[] bytes)
        {
            var image = new LoadedImage(new Address(0x00100000), bytes);
            var dasm = new ArmDisassembler3(new ArmProcessorArchitecture(), image.CreateReader(0));
            var instr = dasm.Disassemble();
            return instr;
        }

        protected static ArmInstruction Disassemble(uint instr)
        {
            var image = new LoadedImage(new Address(0x00100000), new byte[4]);
            LeImageWriter w = new LeImageWriter(image.Bytes);
            w.WriteLeUInt32(0, instr);
            var dasm = new ArmDisassembler2(new ArmProcessorArchitecture(), image.CreateReader(0));
            return dasm.Disassemble();
        }

        protected MachineInstruction DisassembleBits(string bitPattern)
        {
            var image = new LoadedImage(new Address(0x00100000), new byte[4]);
            LeImageWriter w = new LeImageWriter(image.Bytes);
            uint instr = ParseBitPattern(bitPattern);
            w.WriteLeUInt32(0, instr);
            var arch = CreateArchitecture();
            var dasm = arch.CreateDisassembler(image.CreateReader(0));
            return dasm.DisassembleInstruction();
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
                throw new ArgumentException("Bit pattern didn't contain exactly 32 binary digits.", "bitPattern");
            return instr;
        }
    }

    [TestFixture]
    public class ArmDisassemblerTests : ArmTestBase
    {
        protected override IProcessorArchitecture CreateArchitecture()
        {
            return new ArmProcessorArchitecture();
        }

        [Test]
        public void ArmDasm_Cond_Eq()
        {
            var instr = Disassemble(0x00111111);
            Assert.AreEqual(Condition.eq, instr.Cond);
        }

        [Test]
        public void ArmDasm_b()
        {
            var instr = Disassemble(0xEAFFFFFE);
            Assert.AreEqual("b\t$00100000", instr.ToString());
        }

        [Test]
        public void ArmDasm_bl()
        {
            var instr = Disassemble(0xCBFFFFAA);
            Assert.AreEqual("blgt\t$000FFEB0", instr.ToString());
        }

        [Test]
        public void ArmDasm_Andne_rr()
        {
            var instr = Disassemble(0x10021003);
            Assert.AreEqual("andne\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void ArmDasm_eorcss_rr()
        {
            var instr = Disassemble(0x20321003);
            Assert.AreEqual("eorcss\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void ArmDasm_subccs_rr_lsl_3()
        {
            var instr = Disassemble(0x30521183);
            Assert.AreEqual("subccs\tr1,r2,r3,lsl #3", instr.ToString());
        }

        [Test]
        public void ArmDasm_rsbmis_rr_lsr_32()
        {
            var instr = Disassemble(0x40721023);
            Assert.AreEqual("rsbmis\tr1,r2,r3,lsr #&20", instr.ToString());
        }

        [Test]
        public void ArmDasm_addpls_rr_asr_32()
        {
            var instr = Disassemble(0x50921043);
            Assert.AreEqual("addpls\tr1,r2,r3,asr #&20", instr.ToString());
        }

        [Test]
        public void ArmDasm_adcvss_rr_rrx_32()
        {
            var instr = Disassemble(0x60B21063);
            Assert.AreEqual("adcvss\tr1,r2,r3,rrx #1", instr.ToString());
        }

        [Test]
        public void ArmDasm_sbcvcs_r1_r2_r3_lsl_r4()
        {
            var instr = Disassemble(0x70D21413);
            Assert.AreEqual("sbcvcs\tr1,r2,r3,lsl r4", instr.ToString());
        }

        [Test]
        public void ArmDasm_rschi_r1_r2_imm7()
        {
            var instr = Disassemble(0x82E21007u);
            Assert.AreEqual("rschi\tr1,r2,#7", instr.ToString());
        }

        [Test]
        public void ArmDasm_tstxx_r1_imm4()
        {
            var instr = Disassemble(0x93110F01u);
            Assert.AreEqual("tstls\tr1,#4", instr.ToString());
        }

        [Test]
        public void ArmDasm_mulges_r13_r14_r15()
        {
            var instr = Disassemble(0xA01D8F9Eu);
            Assert.AreEqual("mulges\tr13,r14,r15", instr.ToString());
        }

        [Test]
        public void ArmDasm_mlalt_r11_r12_r13_r14()
        {
            var instr = Disassemble(0xB02BED9Cu);
            Assert.AreEqual("mlalt\tr11,r12,r13,r14", instr.ToString());
        }

        [Test]
        public void ArmDasm_AllZeroes()
        {
            var instr = Disassemble(0x00000000u);
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
            Assert.AreEqual("ldrble\tr4,[r6,#&123]", instr.ToString());
        }

        [Test]
        public void ArmDasm_strb_r5_r9_post_r1()
        {
            var instr = DisassembleBits("1110 01 100100 1001 0101 00000 000 0001");
            Assert.AreEqual("strb\tr5,[r9],r1", instr.ToString());
        }

        [Test]
        public void ArmDasm_strb_r5_r9_post_r1_lsr_3_writeback()
        {
            var instr = DisassembleBits("1110 01 110110 1001 0101 00001 000 0001");
            Assert.AreEqual("strb\tr5,[r9,r1,lsl #1]!", instr.ToString());
        }

        [Test]
        public void ArmDasm_ldr_r5_r7_neg_off()
        {
            var instr = DisassembleBits("1101 01 011101 0111 0101 000100100011");
            Assert.AreEqual("ldrble\tr5,[r7,-#&123]", instr.ToString());
        }

        [Test]
        public void ldr_r5_r7_neg_r1()
        {
            var instr = DisassembleBits("1101 01 111101 0111 0101 00000 000 0001");
            Assert.AreEqual("ldrble\tr5,[r7,-r1]", instr.ToString());
        }

        [Test]
        public void ArmDasm_Swpb()
        {
            var instr = DisassembleBits("1110 00010 100 0001 0010 00001001 0011");
            Assert.AreEqual("swpb\tr2,r3,[r1]", instr.ToString());
        }

        [Test]
        public void ArmDasm_Cmp()
        {
            var instr = DisassembleBits("1110 0001 0100 0101 0110 000000000000");
            Assert.AreEqual("cmp\tr5,r0", instr.ToString());
        }

        [Test]
        public void ArmDasm_setend()
        {
            var instr = DisassembleBits("11110 0010000 000 1 00000000 0000 0000");
            Assert.AreEqual("setendle", instr.ToString());
            instr = DisassembleBits("11110 0010000 000 1 00000010 0000 0000");
            Assert.AreEqual("setendbe", instr.ToString());
        }

        [Test]
        public void ArmDasm_mov_pc_r14()
        {
            var instr = DisassembleBits("1110 00011010 0000 1111 000000001110");
            Assert.AreEqual("mov\tr15,r14", instr.ToString());
        }
    }
}
