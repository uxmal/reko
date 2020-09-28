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

using Reko.Arch.Mos6502;
using Reko.Core;
using Reko.Core.Machine;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Arch.Mos6502
{
    [TestFixture]   
    public class DisassemblerTests
    {
        private MachineInstruction RunTest(params byte[] bytes)
        {
            var image = new MemoryArea(Address.Ptr32(0x200), bytes);
            var rdr = new LeImageReader(image, 0);
            var dasm = new Disassembler(rdr);
            return dasm.First();
        }

        [Test]
        public void Dis6502_ora_zpx()
        {
            var instr = RunTest(0x1, 0x1);
            Assert.AreEqual("ora\t($01,x)", instr.ToString());
        }

        [Test]
        public void Dis6502_asl_zp()
        {
            var instr = RunTest(0x06, 0x06);
            Assert.AreEqual("asl\t$06", instr.ToString());
        }

        [Test]
        public void Dis6502_or_abs()
        {
            var instr = RunTest(0x0D, 0x34, 0x12);
            Assert.AreEqual("ora\t$1234", instr.ToString());
        }

        [Test]
        public void Dis6502_inc_absx()
        {
            var instr = RunTest(0xFE, 0x34, 0x12);
            Assert.AreEqual("inc\t$1234,x", instr.ToString());
        }

        [Test]
        public void Dis6502_and_largeaddress()
        {
            var instr = RunTest(0x3D, 0xC0, 0xAE);
            Assert.AreEqual("and\t$AEC0,x", instr.ToString());
        }

        [Test]
        public void Dis6502_rol_a()
        {
            var instr = RunTest(0x2A);
            Assert.AreEqual("rol\t", instr.ToString());
        }

        [Test]
        public void Dis6502_asl_a()
        {
            var instr = RunTest(0x0A);
            Assert.AreEqual("asl\t", instr.ToString());
        }

        [Test]
        public void Dis6502_sta_zpx()
        {
            var instr = RunTest(0x095, 0x04);
            Assert.AreEqual("sta\t$04,x", instr.ToString());
        }
    }
}
