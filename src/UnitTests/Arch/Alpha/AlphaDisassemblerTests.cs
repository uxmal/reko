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
using Reko.Arch.Alpha;
using Reko.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            this.arch = new AlphaArchitecture("alpha");
        }

        public override IProcessorArchitecture Architecture { get { return arch; } }

        public override Address LoadAddress { get { return Address.Ptr64(0x00100000); } }

        private void AssertCode(string sExp, string sActual)
        {
            Assert.AreEqual(sExp, sActual);
        }

        private void AssertCode(string testName, uint opcode, string sExp, string sActual)
        {
            if (sExp == sActual)
                return;

            var trace = new System.Diagnostics.StackTrace();
            var caller = trace.GetFrame(1);
            var callerName = caller.GetMethod().Name;
            Debug.Print(
@"        public void {0}()
        {{
            var instr = DisassembleWord({1:X8});
            AssertCode(""{2}"", instr.ToString());
        }}
", testName, opcode, sActual.Replace("\t", "\\t"));
    }

    [Test]
        public void AlphaDis_lda()
        {
            var instr = DisassembleWord(0x23DEFFD0);
            AssertCode("lda\tr30,-30(r30)", instr.ToString());
        }

        [Test]
        public void AlphaDis_ldah()
        {
            var instr = DisassembleWord(0x241F029D);
            AssertCode("ldah\tr0,29D(zero)", instr.ToString());
        }

        [Test]
        public void AlphaDis_stq()
        {
            var instr = DisassembleWord(0xB53E0000);
            AssertCode("stq\tr9,0(r30)", instr.ToString());
        }

        [Test]
        public void AlphaDis_ldl()
        {
            var instr = DisassembleWord(0xA0008868);
            AssertCode("ldl\tr0,-7798(r0)", instr.ToString());
        }

        [Test]
        public void AlphaDis_br_self()
        {
            var instr = DisassembleWord(0xE63FFFFF);
            AssertCode("beq\tr17,0000000000100000", instr.ToString());
        }

        [Test]
        public void AlphaDis_beq()
        {
            var instr = DisassembleWord(0xE6200004);
            AssertCode("beq\tr17,0000000000100014", instr.ToString());
        }

        [Test]
        public void AlphaDis_bis()
        {
            var instr = DisassembleWord(0x47E03412);
            AssertCode("bis\tzero,01,r18", instr.ToString());
        }

        [Test]
        public void AlphaDis_bsr()
        {
            var instr = DisassembleWord(0xD340028D);
            AssertCode("bsr\tr26,0000000000100A38", instr.ToString());
        }

        [Test]
        public void AlphaDis_br()
        {
            var instr = DisassembleWord(0xC3E0001F);
            AssertCode("br\tzero,0000000000100080", instr.ToString());
        }

        [Test]
        public void AlphaDis_zapnot()
        {
            var instr = DisassembleWord(0x4A40762A);
            AssertCode("zapnot\tr18,03,r10", instr.ToString());
        }

        [Test]
        public void AlphaDis_jsr()
        {
            var instr = DisassembleWord(0x6B404000);
            AssertCode("jsr\tr26,r0", instr.ToString());
        }

        [Test]
        public void AlphaDis_mull()
        {
            var instr = DisassembleWord(0x4C230012);
            AssertCode("mull\tr1,r24,r18", instr.ToString());
        }

        [Test]
        public void AlphaDis_stt()
        {
            var instr = DisassembleWord(0x9E1E0290);
            AssertCode("stt\tf16,290(r30)", instr.ToString());
        }

        [Test]
        public void AlphaDis_00()
        {
            var instr = DisassembleWord(0x00905A4D);
            AssertCode("invalid", instr.ToString());
        }

        [Test]
        public void AlphaDis_halt()
        {
            var instr = DisassembleWord(0x00000000);
            AssertCode("halt", instr.ToString());
        }

        [Test]
        public void AlphaDis_fble()
        {
            var instr = DisassembleWord(0xCDCCCCCC);
            AssertCode("fble\tf14,0000000000433334", instr.ToString());
        }
    }
}
