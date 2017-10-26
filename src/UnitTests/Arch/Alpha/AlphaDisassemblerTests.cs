#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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
using Reko.Arch.Alpha;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Alpha
{
    [TestFixture]
    public class AlphaDisassemblerTests : DisassemblerTestBase<AlphaInstruction>
    {
        private AlphaArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new AlphaArchitecture();
            arch.Name = "alpha";
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr64(0x00100000); } }

        protected override ImageWriter CreateImageWriter(byte[] bytes)
        {
            return new LeImageWriter(bytes);
        }

        [Test]
        public void AlphaDis_lda()
        {
            var instr = DisassembleWord(0x23DEFFD0);
            Assert.AreEqual("lda\tr30,-30(r30)", instr.ToString());
        }

        [Test]
        public void AlphaDis_ldah()
        {
            var instr = DisassembleWord(0x241F029D);
            Assert.AreEqual("ldah\tr0,29D(zero)", instr.ToString());
        }

        [Test]
        public void AlphaDis_stq()
        {
            var instr = DisassembleWord(0xB53E0000);
            Assert.AreEqual("stq\tr9,0(r30)", instr.ToString());
        }

        [Test]
        public void AlphaDis_ldl()
        {
            var instr = DisassembleWord(0xA0008868);
            Assert.AreEqual("ldl\tr0,-7798(r0)", instr.ToString());
        }

        [Test]
        public void AlphaDis_br_self()
        {
            var instr = DisassembleWord(0xE63FFFFF);
            Assert.AreEqual("beq\tr17,0000000000100000", instr.ToString());
        }

        [Test]
        public void AlphaDis_E6200004()
        {
            var instr = DisassembleWord(0xE6200004);
            Assert.AreEqual("beq\tr17,0000000000100014", instr.ToString());
        }

        [Test]
        public void AlphaDis_bis()
        {
            var instr = DisassembleWord(0x47E03412);
            Assert.AreEqual("bis\tzero,01,r18", instr.ToString());
        }

        [Test]
        public void AlphaDis_D340028D()
        {
            var instr = DisassembleWord(0xD340028D);
            Assert.AreEqual("bsr\tr26,0000000000100A38", instr.ToString());
        }

        [Test]
        public void AlphaDis_C3E0001F()
        {
            var instr = DisassembleWord(0xC3E0001F);
            Assert.AreEqual("br\tzero,0000000000100080", instr.ToString());
        }
    }
}
