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

using Decompiler.Arch.Sparc;
using Decompiler.Core;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Sparc
{
    [TestFixture]
    public class SparcDisassemblerTests
    {
        private static SparcInstruction DisassembleWord(byte[] a)
        {
            ProgramImage img = new ProgramImage(new Address(0x00100000), a);
            return Disassemble(img);
        }

        private static SparcInstruction DisassembleWord(uint instr)
        {
            var bytes = new byte[4];
            new BeImageWriter(bytes).WriteBeUInt32(0, instr);
            var img = new ProgramImage(new Address(0x00100000), bytes);
            return Disassemble(img);
        }

        private static SparcInstruction Disassemble(ProgramImage img)
        {
            var arch = new SparcArchitecture(PrimitiveType.Word32);
            var dasm = new SparcDisassembler(arch, img.CreateReader(0U));
            var instr = dasm.Disassemble();
            return instr;
        }

        private void AssertInstruction(uint word, string expected)
        {
            var instr = DisassembleWord(word);
            Assert.AreEqual(expected, instr.ToString());
        }

        [Test]
        public void SparcDis_call()
        {
            AssertInstruction(0x7FFFFFFF, "call\t000FFFFC");
        }

        [Test]
        public void SparcDis_addc()
        {
            AssertInstruction(0x80400004, "addc\t%g0,%g4,%g0");
        }
    }
}
