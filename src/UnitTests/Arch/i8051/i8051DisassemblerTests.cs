#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
using Reko.Arch.i8051;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.i8051
{
    [TestFixture]
    public class i8051DisassemblerTests : DisassemblerTestBase<i8051Instruction>
    {
        private i8051Architecture arch;

        public i8051DisassemblerTests()
        {
            this.arch = new i8051Architecture("8051");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return Address.Ptr16(0); }
        }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        [Test]
        public void I8051_dis_nop()
        {
            var instr = DisassembleBytes(0);
            Assert.AreEqual("nop", instr.ToString());
        }

        [Test]
        public void I8051_dis_ajmp()
        {
            var instr = DisassembleBytes(0xE1, 0xFF);
            Assert.AreEqual("ajmp\t07FF", instr.ToString());
        }

        [Test]
        public void I8051_dis_ljmp()
        {
            var instr = DisassembleBytes(0x02, 0x12, 0x34);
            Assert.AreEqual("ljmp\t1234", instr.ToString());
        }

        [Test]
        public void I8051_dis_jbc()
        {
            var instr = DisassembleBytes(0x10,0x82,0x0D);
            Assert.AreEqual("jbc\tP0.2,0010", instr.ToString());
        }

        [Test]
        public void I8051_dis_inc()
        {
            var instr = DisassembleBytes(0x08);
            Assert.AreEqual("inc\tR0", instr.ToString());

            instr = DisassembleBytes(0x04);
            Assert.AreEqual("inc\tA", instr.ToString());
        }

        [Test]
        public void I8051_dis_add()
        {
            var instr = DisassembleBytes(0x25, 0x25);
            Assert.AreEqual("add\tA,[0025]", instr.ToString());

            instr = DisassembleBytes(0x2B);
            Assert.AreEqual("add\tA,R3", instr.ToString());
        }

        [Test]
        public void I8051_dis_anl()
        {
            var instr = DisassembleBytes(0xB0, 0x93);
            Assert.AreEqual("anl\tC,/P1.3", instr.ToString());
        }

        [Test]
        public void I8051_dis_mov()
        {
            var instr = DisassembleBytes(0xA2,0x84);
            Assert.AreEqual("mov\tC,P0.4", instr.ToString());
        }

        [Test]
        public void I8051_dis_sjmp()
        {
            var instr = DisassembleBytes(0x80, 0xFE);
            Assert.AreEqual("sjmp\t0000", instr.ToString());
        }
    }
}
