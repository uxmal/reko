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
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Arm
{
    [TestFixture]
    public class ArmDisassemblerTests
    {
        private static ArmInstruction Disassemble(byte[] bytes)
        {
            var image = new ProgramImage(new Address(0x00100000), bytes);
            var dasm = new ArmDisassembler2(new ArmProcessorArchitecture(), image.CreateReader(0));
            var instr = dasm.Disassemble();
            return instr;
        }

        private static ArmInstruction Disassemble(uint instr)
        {
            var image = new ProgramImage(new Address(0x00100000), new byte[4]);
            var dasm = new ArmDisassembler2(new ArmProcessorArchitecture(), image.CreateReader(0));
            return dasm.Disassemble(instr);
        }

        [Test]
        public void Branch()
        {
            var instr = Disassemble(new byte[] { 0xFE, 0xFF, 0xFF, 0xEA, });
            Assert.AreEqual(Opcode.b, instr.Opcode);
            Assert.AreEqual(Condition.al, instr.Cond);
            Assert.AreEqual("b\t$00100000", instr.ToString());
        }

        [Test]
        public void AllZeroes()
        {
            var instr = Disassemble(0x00000000u);
            Assert.AreEqual("andeq\tr0,r0,r0", instr.ToString());
        }

        [Test]
        public void AndWithShift()
        {
            var instr = Disassemble(0xE0100266u);
            Assert.AreEqual("ands\tr0,r0,r0", instr.ToString());
        }

        [Test]
        public void Swpb()
        {
            var instr = Disassemble(0xE1412093u);
            Assert.AreEqual("swpb\tr2,r3,[r1]", instr.ToString());
        }
    }
}
