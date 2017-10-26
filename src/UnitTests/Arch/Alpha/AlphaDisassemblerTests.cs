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
            Assert.AreEqual("lda\tr30,-48(r30)", instr.ToString());
        }

        /*
        [Test]
        public void AlphaDis_606012061()
        {
            var instr = DisassembleWord(0x241F029D);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3040739328()
        {
            var instr = DisassembleWord(0xB53E0000);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3042836488()
        {
            var instr = DisassembleWord(0xB55E0008);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3044933648()
        {
            var instr = DisassembleWord(0xB57E0010);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3047030808()
        {
            var instr = DisassembleWord(0xB59E0018);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3076390944()
        {
            var instr = DisassembleWord(0xB75E0020);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_1206912009()
        {
            var instr = DisassembleWord(0x47F00409);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_1207043082()
        {
            var instr = DisassembleWord(0x47F2040A);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_2684389480()
        {
            var instr = DisassembleWord(0xA0008868);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_1207108619()
        {
            var instr = DisassembleWord(0x47F3040B);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3860856836()
        {
            var instr = DisassembleWord(0xE6200004);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_1205875730()
        {
            var instr = DisassembleWord(0x47E03412);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_1073754129()
        {
            var instr = DisassembleWord(0x40003011);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3544187533()
        {
            var instr = DisassembleWord(0xD340028D);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_3286237215()
        {
            var instr = DisassembleWord(0xC3E0001F);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_631177885()
        {
            var instr = DisassembleWord(0x259F029D);
            Assert.AreEqual("@@@", instr.ToString());
        }

        [Test]
        public void AlphaDis_1206518800()
        {
            var instr = DisassembleWord(0x47EA0410);
            Assert.AreEqual("@@@", instr.ToString());
        }
        */
    }
}
