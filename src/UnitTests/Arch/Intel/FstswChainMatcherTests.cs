#region License
/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Arch.X86;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Arch.Intel
{
    [TestFixture]
    [Ignore("Disabled until we come up with a mechanism to perform architecture-specific expression simplifications, at which point this should go to Analysis")]
    public class FstswChainMatcherTests
    {
        IntelArchitecture arch;
        ProcedureBuilder emitter;
        X86Assembler asm;
        OperandRewriter orw;
        List<IntelInstruction> instrs;

        [SetUp]
        public void Fstsw_Setup()
        {
            arch = new IntelArchitecture(ProcessorMode.Protected32);
            asm = new X86Assembler(arch, Address.Ptr32(0x10000), new List<EntryPoint>());
            Procedure proc = new Procedure("test", arch.CreateFrame());
            orw = new OperandRewriter32(arch, proc.Frame, null);
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
            Assert.AreEqual(Opcode.nop, instrs[1].code);
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
            Assert.AreEqual(Opcode.jnz, instrs[2].code);
            Assert.IsTrue(m.Matches(0));
            m.Rewrite(emitter);
            Assert.AreEqual(1, emitter.Block.Statements.Count);
            Assert.AreEqual("SCZO = FPUF", emitter.Block.Statements[0].ToString());
            Assert.AreEqual(Opcode.jz, instrs[2].code);
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
            Assert.AreEqual(Opcode.jge, instrs[2].code);
        }

        private FstswChainMatcher GetMatcher()
        {
            Program lr = asm.GetImage();
            X86Disassembler dasm = new X86Disassembler(
                lr.Image.CreateLeReader(0),
                PrimitiveType.Word32,
                PrimitiveType.Word32,
                false);
            instrs = new List<IntelInstruction>();
            return new FstswChainMatcher(dasm.ToArray(), orw);
        }
    }
}
