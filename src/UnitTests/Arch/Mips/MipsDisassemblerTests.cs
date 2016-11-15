﻿#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Arch.Mips
{
    [TestFixture]
    public class MipsDisassemblerTests : DisassemblerTestBase<MipsInstruction>
    {
        private MipsProcessorArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new MipsBe32Architecture();
            arch.Name = "mips-be-32";
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr32(0x00100000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new BeImageWriter(bytes);
        }

        private void Given_Mips_v6_Architecture()
        {
            arch = new MipsBe32Architecture();
            arch.Name = "mipsv6-be-32";
        }

        private void VerifyRegisterOperand(MachineOperand op, RegisterStorage reg, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(RegisterOperand), op);
            RegisterOperand opReg = op as RegisterOperand;
            Assert.AreEqual(reg, opReg.Register);
            Assert.AreEqual(type, opReg.Register.DataType);
            Assert.AreEqual(type, opReg.Width);
        }
        private void VerifyIndirectOperand(MachineOperand op, RegisterStorage reg, int offset, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(IndirectOperand), op);
            IndirectOperand opReg = op as IndirectOperand;
            Assert.AreEqual(reg, opReg.Base);
            Assert.AreEqual(offset, opReg.Offset);
            Assert.AreEqual(type, opReg.Width);
        }
        private void VerifyImmediateOperand(MachineOperand op, Constant val, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(ImmediateOperand), op);
            ImmediateOperand opReg = op as ImmediateOperand;
            Assert.AreEqual(type, opReg.Width);
            Assert.AreEqual(type, opReg.Value.DataType);
            Assert.AreEqual(val.GetValue(), opReg.Value.GetValue());
        }
        private void VerifyAddressOperand(MachineOperand op, Address addr, PrimitiveType type)
        {
            Assert.IsAssignableFrom(typeof(AddressOperand), op);
            AddressOperand opReg = op as AddressOperand;
            Assert.AreEqual(type, opReg.Width);
            Assert.AreEqual(addr, opReg.Address);
        }


        [Test]
        public void MipsDis_addi()
        {
            var instr = DisassembleBits("001000 00001 00010 1111111111111000");
            Assert.AreEqual("addi\tr2,r1,-00000008", instr.ToString());
            Assert.AreEqual(Opcode.addi, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r1, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Int32(-8), PrimitiveType.Int32);
        }

        [Test]
        public void MipsDis_addiu()
        {
            var instr = DisassembleBits("001001 00011 00110 1111111111111000");
            Assert.AreEqual("addiu\tr6,r3,-00000008", instr.ToString());
            Assert.AreEqual(Opcode.addiu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r6, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Int32(-8), PrimitiveType.Int32);
        }

        [Test]
        public void MipsDis_addu()
        {
            var instr = DisassembleBits("000000 00011 00110 11110 00000 100001");
            Assert.AreEqual("addu\tr30,r3,r6", instr.ToString());
            Assert.AreEqual(Opcode.addu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r6, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_and()
        {
            var instr = DisassembleBits("000000 00011 00110 11110 00000 100100");
            Assert.AreEqual("and\tr30,r3,r6", instr.ToString());
            Assert.AreEqual(Opcode.and, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r6, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_andi()
        {
            var instr = DisassembleBits("001100 00011 00110 1111000000100100");
            Assert.AreEqual("andi\tr6,r3,0000F024", instr.ToString());
            Assert.AreEqual(Opcode.andi, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r6, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Word32(0xF024), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_beq()
        {
            var instr = DisassembleBits("000100 00011 00110 0000000000000001");
            Assert.AreEqual("beq\tr3,r6,00100008", instr.ToString());
            Assert.AreEqual(Opcode.beq, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r6, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op3, Address.Ptr32(0x100008), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_beql()
        {
            var instr = DisassembleBits("010100 00011 00110 0000000000000001");
            Assert.AreEqual("beql\tr3,r6,00100008", instr.ToString());
            Assert.AreEqual(Opcode.beql, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r6, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op3, Address.Ptr32(0x100008), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bgez()
        {
            var instr = DisassembleBits("000001 00011 00001 1111111111111110");
            Assert.AreEqual("bgez\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bgez, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bgezal()
        {
            var instr = DisassembleBits("000001 00011 10001 1111111111111110");
            Assert.AreEqual("bgezal\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bgezal, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bgezall()
        {
            var instr = DisassembleBits("000001 00011 10011 1111111111111110");
            Assert.AreEqual("bgezall\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bgezall, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bgezl()
        {
            var instr = DisassembleBits("000001 00011 00011 1111111111111110");
            Assert.AreEqual("bgezl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bgezl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bgtz()
        {
            var instr = DisassembleBits("000111 00011 00000 1111111111111110");
            Assert.AreEqual("bgtz\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bgtz, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bgtzl()
        {
            var instr = DisassembleBits("010111 00011 00000 1111111111111110");
            Assert.AreEqual("bgtzl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bgtzl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_blez()
        {
            var instr = DisassembleBits("000110 00011 00000 1111111111111110");
            Assert.AreEqual("blez\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.blez, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_blezl()
        {
            var instr = DisassembleBits("010110 00011 00000 1111111111111110");
            Assert.AreEqual("blezl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.blezl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bltz()
        {
            var instr = DisassembleBits("000001 00011 00000 1111111111111110");
            Assert.AreEqual("bltz\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bltz, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bltzal()
        {
            var instr = DisassembleBits("000001 00011 10000 1111111111111110");
            Assert.AreEqual("bltzal\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bltzal, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bltzall()
        {
            var instr = DisassembleBits("000001 00011 10010 1111111111111110");
            Assert.AreEqual("bltzall\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bltzall, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bltzl()
        {
            var instr = DisassembleBits("000001 00011 00010 1111111111111110");
            Assert.AreEqual("bltzl\tr3,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bltzl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op2, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bne()
        {
            var instr = DisassembleBits("000101 00011 00010 1111111111111110");
            Assert.AreEqual("bne\tr3,r2,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bne, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r2, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op3, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_bnel()
        {
            var instr = DisassembleBits("010101 00011 00010 1111111111111110");
            Assert.AreEqual("bnel\tr3,r2,000FFFFC", instr.ToString());
            Assert.AreEqual(Opcode.bnel, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r2, PrimitiveType.Word32);
            VerifyAddressOperand(instr.op3, Address.Ptr32(0x000FFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_break()
        {
            var instr = DisassembleBits("000000 00011000101111111111 001101");
            Assert.AreEqual("break\t00018BFF", instr.ToString());
            Assert.AreEqual(Opcode.@break, instr.opcode);
            VerifyImmediateOperand(instr.op1, Constant.Word32(0x18BFF), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dadd()
        {
            var instr = DisassembleBits("000000 00011 00010 11110 00000 101100");
            Assert.AreEqual("dadd\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Opcode.dadd, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r2, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_daddi()
        {
            var instr = DisassembleBits("011000 00011 00010 1111000000101100");
            Assert.AreEqual("daddi\tr2,r3,-00000FD4", instr.ToString());
            Assert.AreEqual(Opcode.daddi, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Int32(-0xFD4), PrimitiveType.Int32);
        }

        [Test]
        public void MipsDis_daddiu()
        {
            var instr = DisassembleBits("011001 00011 00010 1111000000101100");
            Assert.AreEqual("daddiu\tr2,r3,-00000FD4", instr.ToString());
            Assert.AreEqual(Opcode.daddiu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Int32(-0xFD4), PrimitiveType.Int32);
        }

        [Test]
        public void MipsDis_daddu()
        {
            var instr = DisassembleBits("000000 00011 00010 11110 00000 101101");
            Assert.AreEqual("daddu\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Opcode.daddu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r2, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_ddiv()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011110");
            Assert.AreEqual("ddiv\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.ddiv, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_ddivu()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011111");
            Assert.AreEqual("ddivu\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.ddivu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_div()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011010");
            Assert.AreEqual("div\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.div, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_divu()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011011");
            Assert.AreEqual("divu\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.divu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dmult()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011100");
            Assert.AreEqual("dmult\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.dmult, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dmultu()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011101");
            Assert.AreEqual("dmultu\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.dmultu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dsll()
        {
            var instr = DisassembleBits("000000 00000 00101 00111 01001 111000");
            Assert.AreEqual("dsll\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Opcode.dsll, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsl32()
        {
            var instr = DisassembleBits("000000 00000 00101 00111 01001 111100");
            Assert.AreEqual("dsll32\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Opcode.dsll32, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsllv()
        {
            var instr = DisassembleBits("000000 00011 00101 00111 00000 010100");
            Assert.AreEqual("dsllv\tr7,r5,r3", instr.ToString());
            Assert.AreEqual(Opcode.dsllv, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r3, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dsra()
        {
            var instr = DisassembleBits("000000 00000 00101 00111 01001 111011");
            Assert.AreEqual("dsra\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Opcode.dsra, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsra32()
        {
            var instr = DisassembleBits("000000 00000 00101 00111 01001 111111");
            Assert.AreEqual("dsra32\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Opcode.dsra32, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsrav()
        {
            var instr = DisassembleBits("000000 00011 00101 00111 00000 010111");
            Assert.AreEqual("dsrav\tr7,r5,r3", instr.ToString());
            Assert.AreEqual(Opcode.dsrav, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r3, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dsrl()
        {
            var instr = DisassembleBits("000000 00000 00101 00111 01001 111010");
            Assert.AreEqual("dsrl\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Opcode.dsrl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsrl32()
        {
            var instr = DisassembleBits("000000 00000 00101 00111 01001 111110");
            Assert.AreEqual("dsrl32\tr7,r5,09", instr.ToString());
            Assert.AreEqual(Opcode.dsrl32, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Byte(0x09), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_dsrlv()
        {
            var instr = DisassembleBits("000000 00011 00101 00111 00000 010110");
            Assert.AreEqual("dsrlv\tr7,r5,r3", instr.ToString());
            Assert.AreEqual(Opcode.dsrlv, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r3, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dsub()
        {
            var instr = DisassembleBits("000000 00011 00010 11110 00000 101110");
            Assert.AreEqual("dsub\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Opcode.dsub, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r2, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_dsubu()
        {
            var instr = DisassembleBits("000000 00011 00010 11110 00000 101111");
            Assert.AreEqual("dsubu\tr30,r3,r2", instr.ToString());
            Assert.AreEqual(Opcode.dsubu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r30, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r2, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_j()
        {
            var instr = DisassembleBits("000010 11111111111111111111111111");
            Assert.AreEqual("j\t0FFFFFFC", instr.ToString());
            Assert.AreEqual(Opcode.j, instr.opcode);
            VerifyAddressOperand(instr.op1, Address.Ptr32(0x0FFFFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_jal()
        {
            var instr = DisassembleBits("000011 11111111111111111111111111");
            Assert.AreEqual("jal\t0FFFFFFC", instr.ToString());
            Assert.AreEqual(Opcode.jal, instr.opcode);
            VerifyAddressOperand(instr.op1, Address.Ptr32(0x0FFFFFFC), PrimitiveType.Pointer32);
        }

        [Test]
        public void MipsDis_jr()
        {
            var instr = DisassembleBits("000000 01001 000000000000000 001000");
            Assert.AreEqual("jr\tr9", instr.ToString());
            Assert.AreEqual(Opcode.jr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r9, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_jalr()
        {
            var instr = DisassembleBits("000000 01001 00000 11111 00000 001001");
            Assert.AreEqual("jalr\tra,r9", instr.ToString());
            Assert.AreEqual(Opcode.jalr, instr.opcode);
            VerifyRegisterOperand(instr.op1, "ra", PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r9, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lb()
        {
            var instr = DisassembleBits("100000 01001 00011 1111111111001000");
            Assert.AreEqual("lb\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lb, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.SByte);
        }

        [Test]
        public void MipsDis_lbu()
        {
            var instr = DisassembleBits("100100 01001 00011 1111111111001000");
            Assert.AreEqual("lbu\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lbu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_ld()
        {
            var instr = DisassembleBits("110111 01001 00011 1111111111001000");
            Assert.AreEqual("ld\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.ld, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_ldl()
        {
            var instr = DisassembleBits("011010 01001 00011 1111111111001000");
            Assert.AreEqual("ldl\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.ldl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_ldr()
        {
            var instr = DisassembleBits("011011 01001 00011 1111111111001000");
            Assert.AreEqual("ldr\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.ldr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_lh()
        {
            var instr = DisassembleBits("100001 01001 00011 1111111111001000");
            Assert.AreEqual("lh\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lh, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Int16);
        }

        [Test]
        public void MipsDis_lhu()
        {
            var instr = DisassembleBits("100101 01001 00011 1111111111001000");
            Assert.AreEqual("lhu\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lhu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word16);
        }

        [Test]
        public void MipsDis_lw()
        {
            var instr = DisassembleBits("100011 01001 00011 1111111111001000");
            Assert.AreEqual("lw\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lw, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lwu()
        {
            var instr = DisassembleBits("100111 01001 00011 1111111111001000");
            Assert.AreEqual("lwu\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lwu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lwl()
        {
            var instr = DisassembleBits("100010 01001 00011 1111111111001000");
            Assert.AreEqual("lwl\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lwl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lwr()
        {
            var instr = DisassembleBits("100110 01001 00011 1111111111001000");
            Assert.AreEqual("lwr\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lwr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_ll()
        {
            MipsInstruction instr;
            instr = DisassembleBits("110000 01001 00011 1111111111001000");
            Assert.AreEqual("ll\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.ll, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lld()
        {
            var instr = DisassembleBits("110100 01001 00011 1111111111001000");
            Assert.AreEqual("lld\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lld, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_ll_v6()
        {
            Given_Mips_v6_Architecture();

            MipsInstruction instr;
            instr = DisassembleBits("011111 01001 00011 111111100 0 110110");
            Assert.AreEqual("ll\tr3,-0004(r9)", instr.ToString());
            Assert.AreEqual(Opcode.ll, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x4, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_lld_v6()
        {
            Given_Mips_v6_Architecture();

            var instr = DisassembleBits("011111 01001 00011 111111100 0 110111");
            Assert.AreEqual("lld\tr3,-0004(r9)", instr.ToString());
            Assert.AreEqual(Opcode.lld, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x4, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_lui()
        {
            var instr = DisassembleBits("001111 00000 00011 1111111111001000");
            Assert.AreEqual("lui\tr3,-0038", instr.ToString());
            Assert.AreEqual(Opcode.lui, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op2, Constant.Int16(-0x38), PrimitiveType.Int16);
        }

        [Test]
        public void MipsDis_mfhi()
        {
            var instr = DisassembleBits("000000 00000 00000 01010 00000 010000");
            Assert.AreEqual("mfhi\tr10", instr.ToString());
            Assert.AreEqual(Opcode.mfhi, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mflo()
        {
            var instr = DisassembleBits("000000 00000 00000 01010 00000 010010");
            Assert.AreEqual("mflo\tr10", instr.ToString());
            Assert.AreEqual(Opcode.mflo, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mthi()
        {
            var instr = DisassembleBits("000000 01010 00000 00000 00000 010001");
            Assert.AreEqual("mthi\tr10", instr.ToString());
            Assert.AreEqual(Opcode.mthi, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mtlo()
        {
            var instr = DisassembleBits("000000 01010 00000 00000 00000 010011");
            Assert.AreEqual("mtlo\tr10", instr.ToString());
            Assert.AreEqual(Opcode.mtlo, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r10, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_movnz()
        {
            var instr = DisassembleBits("000000 00001 00101 01010 00000 001011");
            Assert.AreEqual("movn\tr10,r1,r5", instr.ToString());
            Assert.AreEqual(Opcode.movn, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r10, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_movz()
        {
            var instr = DisassembleBits("000000 00001 00101 01010 00000 001010");
            Assert.AreEqual("movz\tr10,r1,r5", instr.ToString());
            Assert.AreEqual(Opcode.movz, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r10, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mult()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011000");
            Assert.AreEqual("mult\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.mult, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_multu()
        {
            var instr = DisassembleBits("000000 00011 00101 00000 00000 011001");
            Assert.AreEqual("multu\tr3,r5", instr.ToString());
            Assert.AreEqual(Opcode.multu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_or()
        {
            var instr = DisassembleBits("000000 00011 00101 00111 00000 100101");
            Assert.AreEqual("or\tr7,r3,r5", instr.ToString());
            Assert.AreEqual(Opcode.or, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_nor()
        {
            var instr = DisassembleBits("000000 00011 00101 00111 00000 100111");
            Assert.AreEqual("nor\tr7,r3,r5", instr.ToString());
            Assert.AreEqual(Opcode.nor, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_ori()
        {
            var instr = DisassembleBits("001101 00011 00101 0011100000100111");
            Assert.AreEqual("ori\tr5,r3,00003827", instr.ToString());
            Assert.AreEqual(Opcode.ori, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Word32(0x3827), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_sb()
        {
            var instr = DisassembleBits("101000 01001 00011 1111111111001000");
            Assert.AreEqual("sb\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.sb, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_sd()
        {
            var instr = DisassembleBits("111111 01001 00011 1111111111001000");
            Assert.AreEqual("sd\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.sd, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_sdl()
        {
            var instr = DisassembleBits("101100 01001 00011 1111111111001000");
            Assert.AreEqual("sdl\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.sdl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_sdr()
        {
            var instr = DisassembleBits("101101 01001 00011 1111111111001000");
            Assert.AreEqual("sdr\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.sdr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_sh()
        {
            var instr = DisassembleBits("101001 01001 00011 1111111111001000");
            Assert.AreEqual("sh\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.sh, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word16);
        }

        [Test]
        public void MipsDis_sw()
        {
            var instr = DisassembleBits("101011 01001 00011 1111111111001000");
            Assert.AreEqual("sw\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.sw, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_swl()
        {
            var instr = DisassembleBits("101010 01001 00011 1111111111001000");
            Assert.AreEqual("swl\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.swl, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_swr()
        {
            var instr = DisassembleBits("101110 01001 00011 1111111111001000");
            Assert.AreEqual("swr\tr3,-0038(r9)", instr.ToString());
            Assert.AreEqual(Opcode.swr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r9", -0x38, PrimitiveType.Word32);
        }
        
        [Test]
        public void MipsDis_sc()
        {
            var instr = DisassembleBits("111000 01010 10101 1111111111001000");
            Assert.AreEqual("sc\tr21,-0038(r10)", instr.ToString());
            Assert.AreEqual(Opcode.sc, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r21, PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "r10", -0x38, PrimitiveType.Word32);
        }
        [Test]
        public void MipsDis_scd()
        {
            var instr = DisassembleBits("111100 01010 10101 1111111111001000");
            Assert.AreEqual("scd\tr21,-0038(r10)", instr.ToString());
            Assert.AreEqual(Opcode.scd, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r21, PrimitiveType.Word64);
            VerifyIndirectOperand(instr.op2, "r10", -0x38, PrimitiveType.Word64);
        }

        [Test]
        public void MipsDis_xor()
        {
            var instr = DisassembleBits("000000 00011 00101 00111 00000 100110");
            Assert.AreEqual("xor\tr7,r3,r5", instr.ToString());
            Assert.AreEqual(Opcode.xor, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r7, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r5, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_xori()
        {
            var instr = DisassembleBits("001110 00011 00101 0011100000100111");
            Assert.AreEqual("xori\tr5,r3,00003827", instr.ToString());
            Assert.AreEqual(Opcode.xori, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r5, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op3, Constant.Word32(0x3827), PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_nop()
        {
            var instr = DisassembleWord(0); // nop
            Assert.AreEqual("nop", instr.ToString());
            Assert.AreEqual(Opcode.nop, instr.opcode);
        }

        [Test]
        public void MipsDis_sltu()
        {
            var instr = DisassembleWord(0x0144402B);
            Assert.AreEqual("sltu\tr8,r10,r4", instr.ToString());
            Assert.AreEqual(Opcode.sltu, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r8, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r10, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r4, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_sllv()
        {
            var instr = DisassembleWord(0x01011004);
            Assert.AreEqual("sllv\tr2,r1,r8", instr.ToString());
            Assert.AreEqual(Opcode.sllv, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r1, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op3, Registers.r8, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mfc0()
        {
            var instr = DisassembleWord(0x40024800);
            Assert.AreEqual("mfc0\tr2,r9", instr.ToString());
            Assert.AreEqual(Opcode.mfc0, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r2, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, Registers.r9, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_mtc1()
        {
            var instr = DisassembleWord(0x448C0800);
            Assert.AreEqual("mtc1\tr12,f1", instr.ToString());
            Assert.AreEqual(Opcode.mtc1, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r12, PrimitiveType.Word32);
            VerifyRegisterOperand(instr.op2, "f1", PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_swc1()
        {
            var instr = DisassembleWord(0xE7AC0030);
            Assert.AreEqual("swc1\tf12,0030(sp)", instr.ToString());
            Assert.AreEqual(Opcode.swc1, instr.opcode);
            VerifyRegisterOperand(instr.op1, "f12", PrimitiveType.Word32);
            VerifyIndirectOperand(instr.op2, "sp", 0x30, PrimitiveType.Word32);
        }

        [Test]
        public void MipsDis_cle_d()
        {
            var instr = DisassembleWord(0x462C003E);
            Assert.AreEqual("c.le.d\tcc0,f0,f12", instr.ToString());
            Assert.AreEqual(Opcode.c_le_d, instr.opcode);
        }

        [Test]
        public void MipsDis_cfc1()
        {
            var instr = DisassembleWord(0x4443F800);
            Assert.AreEqual("cfc1\tr3,FCSR", instr.ToString());
            Assert.AreEqual(Opcode.cfc1, instr.opcode);
        }

        [Test]
        public void MipsDis_rdhwr()
        {
            // Test only the known ones, we'll have to see how this changes things later on with dynamic custom registers
            var instr = DisassembleBits("011111 00000 00011 00000 00000 111011");
            Assert.AreEqual("rdhwr\tr3,00", instr.ToString()); // CPU number
            Assert.AreEqual(Opcode.rdhwr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op2, Constant.Byte(0x0), PrimitiveType.Byte);

            instr = DisassembleBits("011111 00000 00011 00001 00000 111011");
            Assert.AreEqual("rdhwr\tr3,01", instr.ToString()); // SYNCI step size
            Assert.AreEqual(Opcode.rdhwr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op2, Constant.Byte(0x1), PrimitiveType.Byte);

            instr = DisassembleBits("011111 00000 00011 00010 00000 111011");
            Assert.AreEqual("rdhwr\tr3,02", instr.ToString()); // Cycle counter
            Assert.AreEqual(Opcode.rdhwr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op2, Constant.Byte(0x2), PrimitiveType.Byte);

            instr = DisassembleBits("011111 00000 00011 00011 00000 111011");
            Assert.AreEqual("rdhwr\tr3,03", instr.ToString()); // Cycle counter resolution
            Assert.AreEqual(Opcode.rdhwr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op2, Constant.Byte(0x3), PrimitiveType.Byte);

            instr = DisassembleBits("011111 00000 00011 11101 00000 111011");
            Assert.AreEqual("rdhwr\tr3,1D", instr.ToString()); // OS-specific, thread local pointer on Linux
            Assert.AreEqual(Opcode.rdhwr, instr.opcode);
            VerifyRegisterOperand(instr.op1, Registers.r3, PrimitiveType.Word32);
            VerifyImmediateOperand(instr.op2, Constant.Byte(0x1D), PrimitiveType.Byte);
        }

        [Test]
        public void MipsDis_instrs1()
        {
            Assert.AreEqual("ctc1\tr1,FCSR", DisassembleWord(0x44C1F800).ToString());
            Assert.AreEqual("cvt.w.d\tf0,f12", DisassembleWord(0x46206024).ToString());
            Assert.AreEqual("ctc1\tr3,FCSR", DisassembleWord(0x44C3F800).ToString());
            Assert.AreEqual("bc1f\tcc0,0010004C", DisassembleWord(0x45000012).ToString());
            Assert.AreEqual("add.d\tf0,f12,f0", DisassembleWord(0x46206000).ToString());
            Assert.AreEqual("cfc1\tr3,FCSR", DisassembleWord(0x4443F800).ToString());
            Assert.AreEqual("cvt.w.d\tf2,f0", DisassembleWord(0x462000A4).ToString());
        }
    }
}
