#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.X86
{
    [TestFixture]
    public class NasmSyntaxTests
    {
        private readonly X86ArchitectureFlat32 arch = new X86ArchitectureFlat32(new ServiceContainer(), "x86-protected-32", new Dictionary<string, object>());

        private X86Instruction Disassemble32(params byte[] bytes)
        {
            var mem = new ByteMemoryArea(Address.Ptr32(0x0010_0000), bytes);
            var rdr = mem.CreateLeReader(0);
            var dasm = arch.CreateDisassembler(rdr);
            return (X86Instruction) dasm.First();
        }

        private void AssertEqual(string sExpected, X86Instruction instr)
        {
            var sActual = instr.ToString("N");
            Assert.AreEqual(sExpected, sActual);
        }

        [Test]
        public void NasmSx_ShortOffset()
        {
            var instr = Disassemble32(0x22, 0x40, 0x01);
            AssertEqual("and\tal,[eax+0x1]", instr);
        }

        [Test]
        public void NasmSx_LongOffset()
        {
            var instr = Disassemble32(0x22, 0x80, 0x04, 0x03, 0x02, 0x01);
            AssertEqual("and\tal,[eax+0x1020304]", instr);
        }
    }
}
