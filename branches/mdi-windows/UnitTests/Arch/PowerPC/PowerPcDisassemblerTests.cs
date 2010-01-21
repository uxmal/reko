/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Arch.PowerPC;
using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Decompiler.UnitTests.Arch.PowerPC
{
    [TestFixture]
    public class PowerPcDisassemblerTests
    {
        [Test]
        public void IllegalOpcode()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 00, 00, 00, 00 });
            Assert.AreEqual(Opcode.illegal, instr.Opcode);
        }

        [Test]
        public void DisassembleOri()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 0x60, 0x1F, 0x44, 0x44 });
            Assert.AreEqual(Opcode.ori, instr.Opcode);
            Assert.AreEqual(3, instr.Operands);
            Assert.AreEqual("ori\tr31,r0,4444",instr.ToString());
        }

        [Test]
        public void DisassembleOris()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 0x64, 0x1F, 0x44, 0x44 });
            Assert.AreEqual("oris\tr31,r0,4444", instr.ToString());
        }

        [Test]
        public void DisassembleAddi()
        {
            PowerPcInstruction instr = DisassembleWord(new byte[] { 0x38, 0x1F, 0xFF, 0xFC });
            Assert.AreEqual("addi\tr0,r31,-0004", instr.ToString());
        }

        [Test]
        public void DisassembleOr()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 0);
            Assert.AreEqual("or\tr1,r2,r3", instr.ToString());
        }

        [Test]
        public void DisassembleOrDot()
        {
            PowerPcInstruction instr = DisassembleX(31, 2, 1, 3, 444, 1);
            Assert.AreEqual("or.\tr1,r2,r3", instr.ToString());
        }

        private PowerPcInstruction DisassembleX(uint op, uint rs, uint ra, uint rb, uint xo, uint rc)
        {
            uint w =
                (op << 26) |
                (rs << 21) |
                (ra << 16) |
                (rb << 11) |
                (xo << 1) |
                rc;
            ProgramImage img = new ProgramImage(new Address(0x00100000), new byte[4]);
            img.WriteBeUint32(0, w);
            return Disassemble(img);
        }
        private static PowerPcInstruction DisassembleWord(byte[] a)
        {
            ProgramImage img = new ProgramImage(new Address(0x00100000), a);
            return Disassemble(img);
        }

        private static PowerPcInstruction Disassemble(ProgramImage img)
        {
            PowerPcArchitecture arch = new PowerPcArchitecture(PrimitiveType.Word32);
            PowerPcDisassembler dasm = new PowerPcDisassembler(arch, img.CreateReader(0U), arch.WordWidth);
            PowerPcInstruction instr = dasm.Disassemble();
            return instr;
        }
    }
}
