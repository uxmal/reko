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
using Reko.Arch.Tms7000;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Tms7000
{
    [TestFixture]
    public class Tms7000DisassemblerTests : DisassemblerTestBase<Tms7000Instruction>
    {
        private Tms7000Architecture arch;

        public Tms7000DisassemblerTests()
        {
            this.arch = new Tms7000Architecture("tms7000");
        }

        public override IProcessorArchitecture Architecture
        {
            get { return arch; }
        }

        public override Address LoadAddress
        {
            get { return Address.Ptr16(0x0100); }
        }

        [Test]
        public void Tms7000_dis_nop()
        {
            var instr = DisassembleBytes(0);
            Assert.AreEqual("nop", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_add_b_a()
        {
            var instr = DisassembleBytes(0x68);
            Assert.AreEqual("add\tb,a", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_and_Rs_a()
        {
            var instr = DisassembleBytes(0x13, 0x42);
            Assert.AreEqual("and\tr66,a", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_andp_i_p()
        {
            var instr = DisassembleBytes(0xA3, 0xFE, 0x42);
            Assert.AreEqual("andp\t>FE,p66", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_djnz_b()
        {
            var instr = DisassembleBytes(0xCA, 0xFE);
            Assert.AreEqual("djnz\tb,@0100", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_lda_direct()
        {
            var instr = DisassembleBytes(0x8A, 0x12, 0x34);
            Assert.AreEqual("lda\t@1234", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_lda_direct_indexed()
        {
            var instr = DisassembleBytes(0xAA, 0x12, 0x34);
            Assert.AreEqual("lda\t@1234(b)", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_lda_indirect()
        {
            var instr = DisassembleBytes(0x9A, 0x12);
            Assert.AreEqual("lda\t*r18", instr.ToString());
        }

        [Test]
        public void Tms7000_dis_pop_st()
        {
            var instr = DisassembleBytes(0x08);
            Assert.AreEqual("pop\tst", instr.ToString());
        }
    }
}
