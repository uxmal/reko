#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.UnitTests;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Decompiler.UnitTests.Analysis
{
    public class AliasTests : AnalysisTestBase
    {
        private Procedure proc;
        private Aliases alias;

        [SetUp]
        public void Setup()
        {
            var arch = new IntelArchitecture(ProcessorMode.Real);
            proc = new Procedure("foo", arch.CreateFrame());
            alias = new Aliases(proc, arch);
        }

        protected override void RunTest(Program prog, TextWriter writer)
        {
            var dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
            var eventListener = new FakeDecompilerEventListener();
            var trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, dfa.ProgramDataFlow, eventListener);
            trf.Compute();
            trf.RewriteBasicBlocks();
            RegisterLiveness rl = RegisterLiveness.Compute(prog, dfa.ProgramDataFlow, eventListener);
            foreach (Procedure proc in prog.Procedures.Values)
            {
                LongAddRewriter larw = new LongAddRewriter(proc, prog.Architecture);
                larw.Transform();
                Aliases alias = new Aliases(proc, prog.Architecture, dfa.ProgramDataFlow);
                alias.Transform();
                alias.Write(writer);
                proc.Write(false, writer);
                writer.WriteLine();
            }
        }

        [Test]
        public void AlFactorialReg()
        {
            RunTest("Fragments/factorial_reg.asm", "Analysis/AlFactorialReg.txt");
        }

        [Test]
        public void AlAddSubCarries()
        {
            RunTest("Fragments/addsubcarries.asm", "Analysis/AlAddSubCarries.txt");
        }

        [Test]
        public void AlPreservedAlias()
        {
            RunTest("Fragments/multiple/preserved_alias.asm", "Analysis/AlPreservedAlias.txt");
        }

        [Test]
        public void AlReg00011()
        {
            RunTest("Fragments/regressions/r00011.asm", "Analysis/AlReg00011.txt");
        }

        [Test]
        public void AliasWideToNarrow()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier ax = proc.Frame.EnsureRegister(Registers.ax);
            Assignment ass = alias.CreateAliasInstruction(eax, ax);
            Assert.AreEqual("ax = (word16) eax (alias)", ass.ToString());
        }

        [Test]
        public void AliasWideToNarrowSlice()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier ah = proc.Frame.EnsureRegister(Registers.ah);
            Assignment ass = alias.CreateAliasInstruction(eax, ah);
            Assert.AreEqual("ah = SLICE(eax, byte, 8) (alias)", ass.ToString());
        }

        [Test]
        public void AliasSequenceToElement()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
            Identifier edx_eax = proc.Frame.EnsureSequence(edx, eax, PrimitiveType.Word64);
            Assignment ass = alias.CreateAliasInstruction(edx_eax, eax);
            Assert.AreEqual("eax = (word32) edx_eax (alias)", ass.ToString());
        }

        [Test]
        public void AliasSequenceToSlice()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
            Identifier edx_eax = proc.Frame.EnsureSequence(edx, eax, PrimitiveType.Word64);
            Identifier dh = proc.Frame.EnsureRegister(Registers.dh);
            Assignment ass = alias.CreateAliasInstruction(edx_eax, dh);
            Assert.AreEqual("dh = SLICE(edx_eax, byte, 40) (alias)", ass.ToString());
        }

        [Test]
        public void AliasSequenceToMkSequence()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
            Identifier edx_eax = proc.Frame.EnsureSequence(edx, eax, PrimitiveType.Word64);
            Assignment ass = alias.CreateAliasInstruction(eax, edx_eax);
            Assert.AreEqual("edx_eax = SEQ(edx, eax) (alias)", ass.ToString());

        }

        [Test]
        public void AliasStackArgument()
        {
            Identifier argOff = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word16);
            Identifier argSeg = proc.Frame.EnsureStackArgument(6, PrimitiveType.Word16);
            Identifier argPtr = proc.Frame.EnsureStackArgument(4, PrimitiveType.Pointer32);
            Assignment ass = alias.CreateAliasInstruction(argOff, argPtr);
            Assert.AreEqual("ptrArg04 = DPB(ptrArg04, wArg04, 0, 16) (alias)", ass.ToString());
        }
    }
}
