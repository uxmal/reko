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

using Reko.Arch.Pdp11;
using Reko.Core;
using Reko.Core.Machine;
using Reko.Core.Output;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Reko.UnitTests.Arch.Pdp11
{
    [TestFixture]
    public class DisassemblerTests
    {
        private MachineInstructionWriterOptions options;

        [SetUp]
        public void Setup()
        {
            this.options = MachineInstructionWriterOptions.None;
        }

        private void RunTest(string expected, params ushort[] words)
        {
            var instr = RunTest(words);
            var r = new StringRenderer();
            r.Address = instr.Address;
            instr.Render(r, options);
            Assert.AreEqual(expected, r.ToString());
        }

        private void Given_ResolvePcRelativeAddress()
        {
            this.options = MachineInstructionWriterOptions.ResolvePcRelativeAddress;
        }

        private MachineInstruction RunTest(params ushort[] words)
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
            return dasm.First();
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
            var instr = RunTest(0x7209);
            Assert.AreEqual("div\t@r1,r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_xor()
        {
            var instr = RunTest(0x7811);
            Assert.AreEqual("xor\t(r1)+,r0", instr.ToString());
        }

        [Test]
        public void Pdp11dis_jsr()
        {
            var instr = RunTest(0x0957, 0x1234);
            Assert.AreEqual("jsr\tr5,#1234", instr.ToString());
        }

        [Test]
        public void Pdp11dis_jsr_relative()
        {
            RunTest("jsr\tpc,0582(pc)", 0x09F7, 0x0582);
        }

        [Test]
        public void Pdp11dis_jsr_abs()
        {
            var instr = RunTest(0x09DF, 0x200);
            Assert.AreEqual("jsr\tpc,@#0200", instr.ToString());
        }

        [Test]
        public void Pdp11dis_mov_sp_abs()
        {
            var instr = RunTest(0x119F, 0x1234);
            Assert.AreEqual("mov\tsp,@#1234", instr.ToString());
        }

        [Test]
        public void Pdp11dis_mov_ind_ind()
        {
            var instr = RunTest(0x6CB5, 0x0030, 0x0040);
            Assert.AreEqual("add\t0030(r2),0040(r5)", instr.ToString());
        }

        [Test]
        public void Pdp11dis_clr_abs()
        {
            var instr = RunTest(0x0A1F, 0x0030);
            Assert.AreEqual("clr\t@#0030", instr.ToString());
        }

        [Test]
        public void Pdp11dis_imm()
        {
            var instr = RunTest(0x15DF, 0x0030, 0x0040);
            Assert.AreEqual("mov\t#0030,@#0040", instr.ToString());
        }

        [Test]
        public void Pdp11dis_sub_r_r()
        {
            var instr = RunTest(0xE083);
            Assert.AreEqual("sub\tr2,r3", instr.ToString());
        }

        [Test]
        public void Pdp11dis_asr()
        {
            var instr = RunTest(0x0C8A);
            Assert.AreEqual("asr\t@r2", instr.ToString());
        }

        [Test]
        public void Pdp11dis_movb()
        {
            var instr = RunTest(0x92A3);
            Assert.AreEqual("movb\t@r2,-(r3)", instr.ToString());
        }

        [Test]
        public void Pdp11dis_clr_post()
        {
            RunTest("clrb\t(r2)+", 0x8A12);
        }

        [Test]
        public void Pdp11dis_dec()
        {
            RunTest("dec\tr0", 0x0AC0);
        }

        [Test]
        public void Pdp11dis_beq()
        {
            RunTest("beq\t01FE", 0x03FE);
        }

        [Test]
        public void Pdp11dis_sob()
        {
            RunTest("sob\tr3,0184", 0x7EFF);
        }

        [Test]
        public void Pdp11dis_nop()
        {
            RunTest("nop", 0x00A0);
        }

        [Test]
        public void Pdp11dis_bis()
        {
            RunTest("bis\t#2000,@#0024", 0x55DF, 0x2000, 0x0024);
        }

        [Test]
        public void Pdp11dis_setflags()
        {
            RunTest("setflags\t#04", 0x00B4);
        }

        [Test]
        public void Pdp11dis_mul2()
        {
            RunTest("mul\tr0,r3", 0xF0C0);
        }

        [Test]
        public void Pdp11dis_stcdi()
        {
            RunTest("stcdi\tac4,@-(r4)", 0xFBAC);
        }

        [Test]
        public void Pdp11dis_clr_pcrel_deferred()
        {
            RunTest("clr\t@0010(pc)", 0x0A3F, 0x0010);
        }

        [Test]
        public void Pdp11dis_clr_pcrel_deferred_resolveAddress()
        {
            Given_ResolvePcRelativeAddress();
            RunTest("clr\t@(0214)", 0x0A3F, 0x0010);
        }

        [Test]
        public void Pdp11dis_clr_pcrel_resolveAddress()
        {
            Given_ResolvePcRelativeAddress();
            RunTest("clr\t@#0214", 0x0A37, 0x0010);
        }
    }
}
