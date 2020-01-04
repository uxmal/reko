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

using NUnit.Framework;
using Reko.Arch.Pdp11;
using Reko.Core;
using Reko.Core.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Pdp11
{
    public class Pdp11InstructionComparerTests
    {
        private MachineInstruction[] DisassembleWords(params ushort[] words)
        {
            var bytes = new byte[words.Length * 2];
            LeImageWriter writer = new LeImageWriter(bytes);
            foreach (ushort word in words)
            {
                writer.WriteLeUInt16(word);
            }
            var image = new MemoryArea(Address.Ptr16(0x200), bytes);
            var rdr = new LeImageReader(image, 0);
            var arch = new Pdp11Architecture("pdp11");
            var dasm = new Pdp11Disassembler(rdr, arch);
            return dasm.ToArray();
        }

        [Test]
        public void Pdp11cmp_Regression1()
        {
            var mc = DisassembleWords(0x10E6, 0x10E6);
            var cmp = new Pdp11InstructionComparer(Normalize.Constants | Normalize.Registers);
            var h0 = cmp.GetHashCode(mc[0]);
            var h1 = cmp.GetHashCode(mc[1]);
            Assert.AreEqual(h0, h1);
            var eq = cmp.Equals(mc[0], mc[1]);
            Assert.IsTrue(eq);
        }

        [Test]
        public void Pdp11cmp_Regression2()
        {
            var mc = DisassembleWords(0x88E1, 0x88E1);
            var cmp = new Pdp11InstructionComparer(Normalize.Constants);
            var h0 = cmp.GetHashCode(mc[0]);
            var h1 = cmp.GetHashCode(mc[1]);
            Assert.AreEqual(h0, h1);
            var eq = cmp.Equals(mc[0], mc[1]);
            Assert.IsTrue(eq);
        }
    }
}
