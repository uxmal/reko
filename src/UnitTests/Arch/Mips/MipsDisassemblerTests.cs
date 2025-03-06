#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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

using Reko.Arch.Mips;
using Reko.Core;
using NUnit.Framework;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Expressions;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MipsDisassemblerTests : DisassemblerTestBase<MipsInstruction>
    {
        private MipsProcessorArchitecture arch;
        private MipsDisassemblerTests Registers;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture(new ServiceContainer(), "mips-be-32", new Dictionary<string, object>());
            Registers = this;
        }

        // Provide convenient aliases for registers used in test.
        private RegisterStorage cc0 { get { return arch.ccRegs[0]; } }
        private RegisterStorage r1 { get { return arch.GeneralRegs[1]; } }
        private RegisterStorage r2 { get { return arch.GeneralRegs[2]; } }
        private RegisterStorage r3 { get { return arch.GeneralRegs[3]; } }
        private RegisterStorage r4 { get { return arch.GeneralRegs[4]; } }
        private RegisterStorage r5 { get { return arch.GeneralRegs[5]; } }
        private RegisterStorage r6 { get { return arch.GeneralRegs[6]; } }
        private RegisterStorage r7 { get { return arch.GeneralRegs[7]; } }
        private RegisterStorage r8 { get { return arch.GeneralRegs[8]; } }
        private RegisterStorage r9 { get { return arch.GeneralRegs[9]; } }
        private RegisterStorage r10 { get { return arch.GeneralRegs[10]; } }
        private RegisterStorage r12 { get { return arch.GeneralRegs[12]; } }
        private RegisterStorage r30 { get { return arch.GeneralRegs[30]; } }
        private RegisterStorage r21 { get { return arch.GeneralRegs[21]; } }
        private RegisterStorage sp { get { return arch.GeneralRegs[29]; } }
        private RegisterStorage ra { get { return arch.GeneralRegs[31]; } }
        private RegisterStorage f0 { get { return arch.fpuRegs[0]; } }
        private RegisterStorage f1 { get { return arch.fpuRegs[1]; } }
        private RegisterStorage f12 { get { return arch.fpuRegs[12]; } }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        private void Given_Mips_v6_Architecture()
        {
            arch = new MipsBe32Architecture(
                new ServiceContainer(), 
                "mips-be-32",
                new Dictionary<string, object> {
                    { ProcessorOption.Endianness, "be" },
                    { ProcessorOption.InstructionSet, "v6" },
                    { ProcessorOption.WordSize, 32 },
                });
        }

        private void Given_Mips64_Architecture()
        {
            arch = new MipsBe64Architecture(
                new ServiceContainer(),
                "mips-be-64",
                new Dictionary<string, object> {
                    { ProcessorOption.Endianness, "be" },
                    { ProcessorOption.WordSize, 64 },
                });
        }

        private void Given_Mips64_v6_Architecture()
        {
            arch = new MipsBe64Architecture(
                new ServiceContainer(), 
                "mips-be-32",
                new Dictionary<string, object> { 
                    { ProcessorOption.Endianness, "be" },
                    { ProcessorOption.InstructionSet, "v6" },
                    { ProcessorOption.WordSize, 32 }
                });
        }

        private void VerifyRegisterOperand(MachineOperand op, RegisterStorage reg, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(RegisterStorage), op);
            var opReg = op as RegisterStorage;
            Assert.AreEqual(reg, opReg);
            Assert.AreEqual(type, opReg.DataType);
            Assert.AreEqual(type, opReg.DataType);
        }

        private void VerifyIndirectOperand(MachineOperand op, RegisterStorage reg, int offset, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(IndirectOperand), op);
            var opReg = op as IndirectOperand;
            Assert.AreEqual(reg, opReg.Base);
            Assert.AreEqual(offset, opReg.IntOffset());
            Assert.AreEqual(type, opReg.DataType);
        }

        private void VerifyImmediateOperand(MachineOperand op, Constant val, PrimitiveType type)
        {
            Assert.IsTrue(op is Constant);
            var opReg = op as Constant;
            Assert.AreEqual(type, opReg.DataType);
            Assert.AreEqual(val.GetValue(), opReg.GetValue());
        }

        private void VerifyAddressOperand(MachineOperand op, Address addr, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(Address), op);
            var opReg = op as Address?;
            Assert.AreEqual(type, opReg.Value.DataType);
            Assert.AreEqual(addr, opReg);
        }

        private void AssertCode(string sExp, uint wInstr)
        {
            var instr = DisassembleWord(wInstr);
            Assert.AreEqual(sExp, instr.ToString());
        }

        [Test]
        public void MipsDis_addi()
        {
            var instr = DisassembleWord(0b001000_00001_00010_1111111111111000);
            Assert.AreEqual("addi\tr2,r1,-00000008", instr.ToString());
            Assert.AreEqual(Mnemonic.addi, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r1, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[2], Constant.Int32(-8), PrimitiveType.Int32);
        }

        [Test]
        public void MipsDis_addiu()
        {
            var instr = DisassembleWord(0b001001_00011_00110_1111111111111000);
            Assert.AreEqual("addiu\tr6,r3,-00000008", instr.ToString());
            Assert.AreEqual(Mnemonic.addiu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r6, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[2], Constant.Int32(-8), PrimitiveType.Int32);
        }

        [Test]
        public void MipsDis_addu()
        {
            var instr = DisassembleWord(0b000000_00011_00110_11110_00000_100001);
            Assert.AreEqual("addu\tr30,r3,r6", instr.ToString());
            Assert.AreEqual(Mnemonic.addu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r6, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_and()
        {
            var instr = DisassembleWord(0b000000_00011_00110_11110_00000_100100);
            Assert.AreEqual("and\tr30,r3,r6", instr.ToString());
            Assert.AreEqual(Mnemonic.and, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r6, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_andi()
        {
            var instr = DisassembleWord(0b001100_00011_00110_1111000000100100);
            Assert.AreEqual("andi\tr6,r3,0000F024", instr.ToString());
            Assert.AreEqual(Mnemonic.andi, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r6, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[2], Constant.Word32(0xF024), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_beq()
        {
            var instr = DisassembleWord(0b000100_00011_00110_0000000000000001);
            Assert.AreEqual("beq\tr3,r6,00100008", instr.ToString());
            Assert.AreEqual(Mnemonic.beq, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r6, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[2], Address.Ptr32(0x100008), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_beql()
        {
            var instr = DisassembleWord(0b010100_00011_00110_0000000000000001);
            Assert.AreEqual("beql\tr3,r6,00100008", instr.ToString());
            Assert.AreEqual(Mnemonic.beql, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r6, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[2], Address.Ptr32(0x100008), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bgez()
        {
            var instr = DisassembleWord(0b000001_00011_00001_1111111111111110);
            Assert.AreEqual("bgez\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bgez, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bgezal()
        {
            var instr = DisassembleWord(0b000001_00011_10001_1111111111111110);
            Assert.AreEqual("bgezal\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bgezal, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bgezall()
        {
            var instr = DisassembleWord(0b000001_00011_10011_1111111111111110);
            Assert.AreEqual("bgezall\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bgezall, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bgezl()
        {
            var instr = DisassembleWord(0b000001_00011_00011_1111111111111110);
            Assert.AreEqual("bgezl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bgezl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bgtz()
        {
            var instr = DisassembleWord(0b000111_00011_00000_1111111111111110);
            Assert.AreEqual("bgtz\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bgtz, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bgtzl()
        {
            var instr = DisassembleWord(0b010111_00011_00000_1111111111111110);
            Assert.AreEqual("bgtzl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bgtzl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_blez()
        {
            var instr = DisassembleWord(0b000110_00011_00000_1111111111111110);
            Assert.AreEqual("blez\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.blez, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_blezl()
        {
            var instr = DisassembleWord(0b010110_00011_00000_1111111111111110);
            Assert.AreEqual("blezl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.blezl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bltz()
        {
            var instr = DisassembleWord(0b000001_00011_00000_1111111111111110);
            Assert.AreEqual("bltz\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bltz, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bltzal()
        {
            var instr = DisassembleWord(0b000001_00011_10000_1111111111111110);
            Assert.AreEqual("bltzal\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bltzal, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bltzall()
        {
            var instr = DisassembleWord(0b000001_00011_10010_1111111111111110);
            Assert.AreEqual("bltzall\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bltzall, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bltzl()
        {
            var instr = DisassembleWord(0b000001_00011_00010_1111111111111110);
            Assert.AreEqual("bltzl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bltzl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bne()
        {
            var instr = DisassembleWord(0b000101_00011_00010_1111111111111110);
            Assert.AreEqual("bne\tr3,r2,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bne, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r2, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[2], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_bnel()
        {
            var instr = DisassembleWord(0b010101_00011_00010_1111111111111110);
            Assert.AreEqual("bnel\tr3,r2,000FFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.bnel, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r2, PrimitiveType.Word32);
            VerifyAddressOperand(instr.Operands[2], Address.Ptr32(0x000FFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_break()
        {
            var instr = DisassembleWord(0b000000_00011000101111111111_001101);
            Assert.AreEqual("break\t00018BFF", instr.ToString());
            Assert.AreEqual(Mnemonic.@break, instr.Mnemonic);
            VerifyImmediateOperand(instr.Operands[0], Constant.Word32(0x18BFF), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dadd()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00010_11110_00000_101100);
            Assert.AreEqual("dadd\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Mnemonic.dadd, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r30, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r2, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_daddi()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b011000_00011_00010_1111000000101100);
            Assert.AreEqual("daddi\tr2,r3,-00000FD4", instr.ToString());
            Assert.AreEqual(Mnemonic.daddi, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r2, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Int64(-0xFD4), PrimitiveType.Int64);
        }

        [Test]
        public void MipsDis_daddiu()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b011001_00011_00010_1111000000101100);
            Assert.AreEqual("daddiu\tr2,r3,-00000FD4", instr.ToString());
            Assert.AreEqual(Mnemonic.daddiu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r2, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Int64(-0xFD4), PrimitiveType.Int64);
        }

        [Test]
        public void MipsDis_daddu()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00010_11110_00000_101101);
            Assert.AreEqual("daddu\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Mnemonic.daddu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r30, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r2, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_ddiv()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011110);
            Assert.AreEqual("ddiv\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.ddiv, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_ddivu()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011111);
            Assert.AreEqual("ddivu\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.ddivu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_div()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011010);
            Assert.AreEqual("div\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.div, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_divu()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011011);
            Assert.AreEqual("divu\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.divu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dmult()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011100);
            Assert.AreEqual("dmult\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.dmult, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_dmultu()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011101);
            Assert.AreEqual("dmultu\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.dmultu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_dsll()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00000_00101_00111_01001_111000);
            Assert.AreEqual("dsll\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Mnemonic.dsll, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsl32()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00000_00101_00111_01001_111100);
            Assert.AreEqual("dsll32\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Mnemonic.dsll32, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsllv()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00111_00000_010100);
            Assert.AreEqual("dsllv\tr7,r5,r3", instr.ToString());
            Assert.AreEqual(Mnemonic.dsllv, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r3, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_dsra()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00000_00101_00111_01001_111011);
            Assert.AreEqual("dsra\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Mnemonic.dsra, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsra32()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00000_00101_00111_01001_111111);
            Assert.AreEqual("dsra32\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Mnemonic.dsra32, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsrav()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00111_00000_010111);
            Assert.AreEqual("dsrav\tr7,r5,r3", instr.ToString());
            Assert.AreEqual(Mnemonic.dsrav, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r3, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_dsrl()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00000_00101_00111_01001_111010);
            Assert.AreEqual("dsrl\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Mnemonic.dsrl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsrl32()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00000_00101_00111_01001_111110);
            Assert.AreEqual("dsrl32\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Mnemonic.dsrl32, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyImmediateOperand(instr.Operands[2], Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsrlv()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00101_00111_00000_010110);
            Assert.AreEqual("dsrlv\tr7,r5,r3", instr.ToString());
            Assert.AreEqual(Mnemonic.dsrlv, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r3, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_dsub()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00010_11110_00000_101110);
            Assert.AreEqual("dsub\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Mnemonic.dsub, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r30, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r2, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_dsubu()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b000000_00011_00010_11110_00000_101111);
            Assert.AreEqual("dsubu\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Mnemonic.dsubu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r30, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word64);
            VerifyRegisterOperand(instr.Operands[2], Registers.r2, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_j()
        {
            var instr = DisassembleWord(0b000010_11111111111111111111111111);
            Assert.AreEqual("j\t0FFFFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.j, instr.Mnemonic);
            VerifyAddressOperand(instr.Operands[0], Address.Ptr32(0x0FFFFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_jal()
        {
            var instr = DisassembleWord(0b000011_11111111111111111111111111);
            Assert.AreEqual("jal\t0FFFFFFC", instr.ToString());
            Assert.AreEqual(Mnemonic.jal, instr.Mnemonic);
            VerifyAddressOperand(instr.Operands[0], Address.Ptr32(0x0FFFFFFC), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_jr()
        {
            var instr = DisassembleWord(0b000000_01001_000000000000000_001000);
            Assert.AreEqual("jr\tr9", instr.ToString());
            Assert.AreEqual(Mnemonic.jr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r9, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_jalr()
        {
            var instr = DisassembleWord(0b000000_01001_00000_11111_00000_001001);
            Assert.AreEqual("jalr\tra,r9", instr.ToString());
            Assert.AreEqual(Mnemonic.jalr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.ra, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r9, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lb()
        {
            var instr = DisassembleWord(0b100000_01001_00011_1111111111001000);
            Assert.AreEqual("lb\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lb, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.SByte);
        }

        [Test]
        public void MipsDis_lbu()
        {
            var instr = DisassembleWord(0b100100_01001_00011_1111111111001000);
            Assert.AreEqual("lbu\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lbu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_lh()
        {
            var instr = DisassembleWord(0b100001_01001_00011_1111111111001000);
            Assert.AreEqual("lh\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lh, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Int16);
        }

        [Test]
        public void MipsDis_lhu()
        {
            var instr = DisassembleWord(0b100101_01001_00011_1111111111001000);
            Assert.AreEqual("lhu\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lhu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word16);
        }

        [Test]
        public void MipsDis_lw()
        {
            var instr = DisassembleWord(0b100011_01001_00011_1111111111001000);
            Assert.AreEqual("lw\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lw, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lwu()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b100111_01001_00011_1111111111001000);
            Assert.AreEqual("lwu\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lwu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lwl()
        {
            var instr = DisassembleWord(0b100010_01001_00011_1111111111001000);
            Assert.AreEqual("lwl\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lwl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lwr()
        {
            var instr = DisassembleWord(0b100110_01001_00011_1111111111001000);
            Assert.AreEqual("lwr\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.lwr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_loads_64bit()
        {
            Given_Mips64_Architecture();

            var instr = DisassembleWord(0b110111_01001_00011_1111111111001000);
            Assert.AreEqual("ld\tr3,-0038(r9)", instr.ToString());

            instr = DisassembleWord(0b011010_01001_00011_1111111111001000);
            Assert.AreEqual("ldl\tr3,-0038(r9)", instr.ToString());

            instr = DisassembleWord(0b011011_01001_00011_1111111111001000);
            Assert.AreEqual("ldr\tr3,-0038(r9)", instr.ToString());

            instr = DisassembleWord(0b100111_01001_00011_1111111111001000);
            Assert.AreEqual("lwu\tr3,-0038(r9)", instr.ToString());
        }

        [Test]
        public void MipsDis_ll()
        {
            MipsInstruction instr;
            instr = DisassembleWord(0b110000_01001_00011_1111111111001000);
            Assert.AreEqual("ll\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.ll, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lld()
        {
            var instr = DisassembleWord(0b110100_01001_00011_1111111111001000);
            Assert.AreEqual("illegal", instr.ToString());

            Given_Mips64_Architecture();
            instr = DisassembleWord(0b110100_01001_00011_1111111111001000);
            Assert.AreEqual("lld\tr3,-0038(r9)", instr.ToString());
        }

        [Test]
        public void MipsDis_ll_v6()
        {
            Given_Mips_v6_Architecture();

            MipsInstruction instr;
            instr = DisassembleWord(0b011111_01001_00011_111111100_0_110110);
            Assert.AreEqual("ll\tr3,-0004(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.ll, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x4, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lld_v6()
        {
            Given_Mips64_v6_Architecture();

            var instr = DisassembleWord(0b011111_01001_00011_111111100_0_110111);
            Assert.AreEqual("lld\tr3,-0004(r9)", instr.ToString());
        }

        [Test]
        public void MipsDis_lui()
        {
            var instr = DisassembleWord(0b001111_00000_00011_1111111111001000);
            Assert.AreEqual("lui\tr3,-0038", instr.ToString());
            Assert.AreEqual(Mnemonic.lui, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[1], Constant.Int16(-0x38), PrimitiveType.Int16);
        }

        [Test]
        public void MipsDis_mfhi()
        {
            var instr = DisassembleWord(0b000000_00000_00000_01010_00000_010000);
            Assert.AreEqual("mfhi\tr10", instr.ToString());
            Assert.AreEqual(Mnemonic.mfhi, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mflo()
        {
            var instr = DisassembleWord(0b000000_00000_00000_01010_00000_010010);
            Assert.AreEqual("mflo\tr10", instr.ToString());
            Assert.AreEqual(Mnemonic.mflo, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mthi()
        {
            var instr = DisassembleWord(0b000000_01010_00000_00000_00000_010001);
            Assert.AreEqual("mthi\tr10", instr.ToString());
            Assert.AreEqual(Mnemonic.mthi, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mtlo()
        {
            var instr = DisassembleWord(0b000000_01010_00000_00000_00000_010011);
            Assert.AreEqual("mtlo\tr10", instr.ToString());
            Assert.AreEqual(Mnemonic.mtlo, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_movnz()
        {
            var instr = DisassembleWord(0b000000_00001_00101_01010_00000_001011);
            Assert.AreEqual("movn\tr10,r1,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.movn, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r10, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_movz()
        {
            var instr = DisassembleWord(0b000000_00001_00101_01010_00000_001010);
            Assert.AreEqual("movz\tr10,r1,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.movz, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r10, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mult()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011000);
            Assert.AreEqual("mult\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.mult, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_multu()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00000_00000_011001);
            Assert.AreEqual("multu\tr3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.multu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_or()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00111_00000_100101);
            Assert.AreEqual("or\tr7,r3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.or, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_nor()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00111_00000_100111);
            Assert.AreEqual("nor\tr7,r3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.nor, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_ori()
        {
            var instr = DisassembleWord(0b001101_00011_00101_0011100000100111);
            Assert.AreEqual("ori\tr5,r3,00003827", instr.ToString());
            Assert.AreEqual(Mnemonic.ori, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[2], Constant.Word32(0x3827), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_sb()
        {
            var instr = DisassembleWord(0b101000_01001_00011_1111111111001000);
            Assert.AreEqual("sb\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.sb, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_sh()
        {
            var instr = DisassembleWord(0b101001_01001_00011_1111111111001000);
            Assert.AreEqual("sh\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.sh, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word16);
        }

        [Test]
        public void MipsDis_sw()
        {
            var instr = DisassembleWord(0b101011_01001_00011_1111111111001000);
            Assert.AreEqual("sw\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.sw, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_swl()
        {
            var instr = DisassembleWord(0b101010_01001_00011_1111111111001000);
            Assert.AreEqual("swl\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.swl, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_swr()
        {
            var instr = DisassembleWord(0b101110_01001_00011_1111111111001000);
            Assert.AreEqual("swr\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Mnemonic.swr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r9, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_stores_64bit()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b111111_01001_00011_1111111111001000);
            Assert.AreEqual("sd\tr3,-0038(r9)", instr.ToString());

            instr = DisassembleWord(0b101100_01001_00011_1111111111001000);
            Assert.AreEqual("sdl\tr3,-0038(r9)", instr.ToString());

            instr = DisassembleWord(0b101101_01001_00011_1111111111001000);
            Assert.AreEqual("sdr\tr3,-0038(r9)", instr.ToString());
        }

        [Test]
        public void MipsDis_sc()
        {
            var instr = DisassembleWord(0b111000_01010_10101_1111111111001000);
            Assert.AreEqual("sc\tr21,-0038(r10)", instr.ToString());
            Assert.AreEqual(Mnemonic.sc, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r21, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.r10, -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_sc_64bit()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0b111100_01001_00011_1111111111001000);
            Assert.AreEqual("scd\tr3,-0038(r9)", instr.ToString());
        }



        [Test]
        public void MipsDis_xor()
        {
            var instr = DisassembleWord(0b000000_00011_00101_00111_00000_100110);
            Assert.AreEqual("xor\tr7,r3,r5", instr.ToString());
            Assert.AreEqual(Mnemonic.xor, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_xori()
        {
            var instr = DisassembleWord(0b001110_00011_00101_0011100000100111);
            Assert.AreEqual("xori\tr5,r3,00003827", instr.ToString());
            Assert.AreEqual(Mnemonic.xori, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[2], Constant.Word32(0x3827), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_nop()
        {
            var instr = DisassembleWord(0); // nop
            Assert.AreEqual("nop", instr.ToString());
            Assert.AreEqual(Mnemonic.nop, instr.Mnemonic);
        }

        [Test]
        public void MipsDis_sltu()
        {
            var instr = DisassembleWord(0x0144402B);
            Assert.AreEqual("sltu\tr8,r10,r4", instr.ToString());
            Assert.AreEqual(Mnemonic.sltu, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r8, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r10, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r4, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_sllv()
        {
            var instr = DisassembleWord(0x01011004);
            Assert.AreEqual("sllv\tr2,r1,r8", instr.ToString());
            Assert.AreEqual(Mnemonic.sllv, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[2], Registers.r8, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mfc0()
        {
            var instr = DisassembleWord(0x40024800);
            Assert.AreEqual("mfc0\tr2,r9", instr.ToString());
            Assert.AreEqual(Mnemonic.mfc0, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.r9, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mtc1()
        {
            var instr = DisassembleWord(0x448C0800);
            Assert.AreEqual("mtc1\tr12,f1", instr.ToString());
            Assert.AreEqual(Mnemonic.mtc1, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r12, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], Registers.f1, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_swc1()
        {
            var instr = DisassembleWord(0xE7AC0030);
            Assert.AreEqual("swc1\tf12,0030(sp)", instr.ToString());
            Assert.AreEqual(Mnemonic.swc1, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.f12, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.Operands[1], Registers.sp, 0x30, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_cfc1()
        {
            var instr = DisassembleWord(0x4443F800);
            Assert.AreEqual("cfc1\tr3,FCSR", instr.ToString());
            Assert.AreEqual(Mnemonic.cfc1, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], this.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], arch.FCSR, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_rdhwr()
        {
            // Test only the known ones, we'll have to see how this changes things later on with dynamic custom registers
            var instr = DisassembleWord(0b011111_00000_00011_00000_00000_111011);
            Assert.AreEqual("rdhwr\tr3,00", instr.ToString()); // CPU number
            Assert.AreEqual(Mnemonic.rdhwr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[1], Constant.Byte(0x0), PrimitiveType.Byte);

            instr = DisassembleWord(0b011111_00000_00011_00001_00000_111011);
            Assert.AreEqual("rdhwr\tr3,01", instr.ToString()); // SYNCI step size
            Assert.AreEqual(Mnemonic.rdhwr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[1], Constant.Byte(0x1), PrimitiveType.Byte);

            instr = DisassembleWord(0b011111_00000_00011_00010_00000_111011);
            Assert.AreEqual("rdhwr\tr3,02", instr.ToString()); // Cycle counter
            Assert.AreEqual(Mnemonic.rdhwr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[1], Constant.Byte(0x2), PrimitiveType.Byte);

            instr = DisassembleWord(0b011111_00000_00011_00011_00000_111011);
            Assert.AreEqual("rdhwr\tr3,03", instr.ToString()); // Cycle counter resolution
            Assert.AreEqual(Mnemonic.rdhwr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[1], Constant.Byte(0x3), PrimitiveType.Byte);

            instr = DisassembleWord(0b011111_00000_00011_11101_00000_111011);
            Assert.AreEqual("rdhwr\tr3,1D", instr.ToString()); // OS-specific, thread local pointer on Linux
            Assert.AreEqual(Mnemonic.rdhwr, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.Operands[1], Constant.Byte(0x1D), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_ctc1()
        {
            var instr = DisassembleWord(0x44C1F800);
            Assert.AreEqual("ctc1\tr1,FCSR", instr.ToString());
            Assert.AreEqual(Mnemonic.ctc1, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], this.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], arch.FCSR, PrimitiveType.Word32);

            instr = DisassembleWord(0x44C3F800);
            Assert.AreEqual("ctc1\tr3,FCSR", instr.ToString());
            Assert.AreEqual(Mnemonic.ctc1, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], this.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.Operands[1], arch.FCSR, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_bc1f()
        {
            var instr = DisassembleWord(0x45000012);
            Assert.AreEqual("bc1f\tcc0,0010004C", instr.ToString());
            Assert.AreEqual(Mnemonic.bc1f, instr.Mnemonic);
            VerifyRegisterOperand(instr.Operands[0], Registers.cc0, PrimitiveType.Bool);
            VerifyAddressOperand(instr.Operands[1], Address.Ptr32(0x0010004C), PrimitiveType.Ptr32);
        }

        [Test]
        public void MipsDis_c_eq_d()
        {
            var instr = DisassembleWord(0x46202032);
            Assert.AreEqual("c.eq.d\tcc0,f4,f0", instr.ToString());
            Assert.AreEqual(Mnemonic.c_eq_d, instr.Mnemonic);
        }

        [Test]
        public void MipsDis_lwc1()
        {
            var instr = DisassembleWord(0xC4230004);
            Assert.AreEqual("lwc1\tf3,0004(r1)", instr.ToString());
            Assert.AreEqual(Mnemonic.lwc1, instr.Mnemonic);
        }

        [Test]
        public void MipsDis_cvt()
        {
            var instr = DisassembleWord(0x46a00861);
            Assert.AreEqual("cvt.d.l\tf1,f1", instr.ToString());
            Assert.AreEqual(Mnemonic.cvt_d_l, instr.Mnemonic);

            instr = DisassembleWord(0x46200820);
            Assert.AreEqual("cvt.s.d\tf0,f1", instr.ToString());
            Assert.AreEqual(Mnemonic.cvt_s_d, instr.Mnemonic);
        }

        [Test]
        public void MipsDis_sd_64()
        {
            Given_Mips64_Architecture();
            var instr = DisassembleWord(0xFFBF0020);
            Assert.AreEqual("sd\tra,0020(sp)", instr.ToString());
        }

        [Test]
        public void MipsDis_movt()
        {
            AssertCode("movt\tr1,r0,fcc0", 0x00010101); // 00
        }

        [Test]
        public void MipsDis_movf()
        {
            AssertCode("movf\tr4,r0,fcc1", 0x00040101); // 00
        }

        [Test]
        public void MipsDis_tgeiu()
        {
            AssertCode("tgeiu\tr0,+00000000", 0x04090000); // 01
        }

        [Test]
        public void MipsDis_jalx()
        {
            AssertCode("jalx\t01BDC880", 0x746f7220);
        }

        [Test]
        public void MipsDis_cache()
        {
            AssertCode("cache\t02,0000(r26)", 0xbf420000);
        }

        [Test]
        public void MipsDis_mul()
        {
            AssertCode("mul\tr4,r4,r5", 0x70852002);
        }

        [Test]
        public void MipsDis_seh()
        {
            AssertCode("seh\tr2,r2", 0x7C021620);
        }

        [Test]
        public void MipsDis_lwxc1()
        {
            AssertCode("lwxc1\tf0,r1(r10)", 0x4d410000);
        }

        [Test]
        [Ignore("Requires MIPS processor option support")]
        public void MipsDis_cache_3()
        {
            AssertCode("cache	0x1,-29256(t8)", 0xbf018db8);
        }

        [Test]
        [Ignore("Requires MIPS processor option support")]
        public void MipsDis_cache_1()
        {
            AssertCode("cache	0x1,-29416(t8)", 0xbf018d18);
        }

        [Test]
        public void MipsDis_swc2()
        {
            AssertCode("swc2\tr0,-2A84(r12)", 0xe980d57c);
        }

        [Test]
        public void MipsDis_sdc2()
        {
            AssertCode("sdc2\tr6,-340D(r30)", 0xfbc6cbf3);
        }


        [Test]
        public void MipsDis_ldc2()
        {
            AssertCode("ldc2\tr12,341F(r30)", 0xdbcc341f);
        }

        [Test]
        public void MipsDis_cache2()
        {
            AssertCode("cache	08,0000(r6)", 0xBCC80000);
        }

        [Test]
        public void MipsDis_madd_s()
        {
            AssertCode("madd.s\tf29,f4,f9,f0", 0x4C804F60);
        }

        [Test]
        public void MipsDis_msub()
        {
            AssertCode("msub\tr8,r7", 0x71070004);
        }

        [Test]
        public void MipsDis_sdc2_preV6()
        {
            AssertCode("sdc2\tr20,-0754(r6)", 0xF8D4F8AC);
        }

        [Test]
        public void MipsDis_prefx()
        {
            AssertCode("prefx\t13,r8(r3)", 0x4C68994F);
        }

        [Test]
        public void MipsDis_swxc1()
        {
            AssertCode("swxc1\tf19,r9(r8)", 0x4D0999C8);
        }

        [Test]
        public void MipsDis_ldxc1()
        {
            AssertCode("ldxc1\tf19,r9(r13)", 0x4DA99A41);
        }

        [Test]
        public void MipsDis_clz()
        {
            AssertCode("clz\tr14,r7", 0x70E17060);
        }

        [Test]
        public void MipsDis_sdbbp()
        {
            AssertCode("sdbbp\t000AF1C8", 0x72BC723F);
        }

        [Test]
        public void MipsDis_luxc1()
        {
            AssertCode("luxc1\tf5,r12(r26)", 0x4F4C4945);
        }

        [Test]
        public void MipsDis_clo()
        {
            AssertCode("clo\tr24,r12", 0x7186C4E1);
        }

        [Test]
        public void MipsDis_wsbh()
        {
            AssertCode("wsbh\tr18,r20", 0x7C1490A0);
        }
    }
}