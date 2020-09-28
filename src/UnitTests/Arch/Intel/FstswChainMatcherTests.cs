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

using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;

namespace Reko.UnitTests.Arch.Intel
{
    [TestFixture]
    [Ignore("Disabled until we come up with a mechanism to perform architecture-specific expression simplifications, at which point this should go to Analysis")]
    public class FstswChainMatcherTests
    {
        IntelArchitecture arch;
        ProcedureBuilder emitter;
        X86Assembler asm;
        OperandRewriter orw;
        List<X86Instruction> instrs;

        [SetUp]
        public void Fstsw_Setup()
        {
            arch = new X86ArchitectureFlat32("x86-protected-32");
            asm = new X86Assembler(null, new DefaultPlatform(null, new X86ArchitectureFlat32("x86-protected-32")), Address.Ptr32(0x10000), new List<ImageSymbol>());
            Procedure proc = new Procedure(arch, "test", Address.Ptr32(0x00123400), arch.CreateFrame());
            orw = new OperandRewriter32(arch, new ExpressionEmitter(), proc.Frame, null);
            emitter = new ProcedureBuilder();
        }

        [Test]
        public void Fstsw_FailMatch()
        {
            asm.Mov(asm.ax, asm.Const(1));
            var m = GetMatcher();
            Assert.IsFalse(m.Matches(0));
        }

        [Test]
        public void Fstsw_MatchSahfSequence()
        {
            asm.Fstsw(asm.ax);
            asm.Sahf();
            var m = GetMatcher();
            Assert.IsTrue(m.Matches(0));
        }

        [Test]
        public void Fstsw_EmitSahf()
        {
            asm.Fstsw(asm.ax);
            asm.Sahf();
            var m = GetMatcher();
            m.Matches(0);
            m.Rewrite(emitter);
            Assert.AreEqual(1, emitter.Block.Statements.Count);
            Assert.AreEqual("SCZO = FPUF", emitter.Block.Statements[0].ToString());
            Assert.AreEqual(Reko.Arch.X86.Mnemonic.nop, instrs[1].code);
        }
        
        [Test]
        public void Fstsw_MatchTestAh40()
        {
            asm.Fstsw(asm.ax);
            asm.Test(asm.ah, asm.Const(0x40));
            asm.Jz("foo");
            var m = GetMatcher();
            Assert.IsTrue(m.Matches(0));
        }

        [Test]
        public void Fstsw_EmitTestAh40()
        {
            asm.Fstsw(asm.ax);
            asm.Test(asm.ah, asm.Const(0x40));
            asm.Jnz("foo");
            var m = GetMatcher();
            Assert.AreEqual(Reko.Arch.X86.Mnemonic.jnz, instrs[2].code);
            Assert.IsTrue(m.Matches(0));
            m.Rewrite(emitter);
            Assert.AreEqual(1, emitter.Block.Statements.Count);
            Assert.AreEqual("SCZO = FPUF", emitter.Block.Statements[0].ToString());
            Assert.AreEqual(Reko.Arch.X86.Mnemonic.jz, instrs[2].code);
        }

        [Test]
        public void Fstsw_EmitTestAx01()
        {
            asm.Fstsw(asm.ax);
            asm.Test(asm.ax, asm.Const(0x0100));
            asm.Jz("foo");
            var m = GetMatcher();
            Assert.IsTrue(m.Matches(0));
            m.Rewrite(emitter);
            Assert.AreEqual(Reko.Arch.X86.Mnemonic.jge, instrs[2].code);
        }

        private FstswChainMatcher GetMatcher()
        {
            Program lr = asm.GetImage();
            var dasm = arch.CreateDisassembler(
                lr.SegmentMap.Segments.Values.First().MemoryArea.CreateLeReader(0));
            instrs = new List<X86Instruction>();
            return new FstswChainMatcher(dasm.Cast<X86Instruction>().ToArray(), orw);
        }
    }
}
