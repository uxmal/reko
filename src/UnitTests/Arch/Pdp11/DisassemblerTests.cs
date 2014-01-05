#region License
/* 
 * Copyright (C) 1999-2014 John Källén.
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

using Decompiler.Arch.Pdp11;
using Decompiler.Core;
using Decompiler.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Pdp11
{
    [TestFixture]   
    public class DisassemblerTests
    {
        private MachineInstruction RunTest(params ushort [] words)
        {
            var bytes = new byte[words.Length * 2];
            LeImageWriter writer = new LeImageWriter(bytes);
            foreach (ushort word in words)
            {
                writer.WriteLeUInt16(word);
            }
            var image = new LoadedImage(new Address(0x200), bytes);
            var rdr = new LeImageReader(image, 0);
            var arch = new Pdp11Architecture();
            var dasm = new Pdp11Disassembler(rdr, arch);
            return dasm.DisassembleInstruction();
        }

        [Test]
        public void Pdp11dis_mul()
        {
            var instr = RunTest(0x7000);
            Assert.AreEqual("mul\tr0,r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_div()
        {
            var instr = RunTest(0x7208);
            Assert.AreEqual("div\t(r0),r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_xor()
        {
            var instr = RunTest(0x7811);
            Assert.AreEqual("xor\t(r1)+,r0", instr.ToString());
        }
    }
}
