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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Analysis;
using Reko.UnitTests;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace Reko.UnitTests.Analysis
{
    public class AliasTests : AnalysisTestBase
    {
        private Procedure proc;
        private Aliases alias;

        [SetUp]
        public void Setup()
        {
            var arch = new X86ArchitectureReal("x86-real-16");
            proc = Procedure.Create(arch, "foo", Address.Ptr32(0x100), arch.CreateFrame());
            alias = new Aliases(proc, arch);
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            var dfa = new DataFlowAnalysis(program, null, new FakeDecompilerEventListener());
            var eventListener = new FakeDecompilerEventListener();
            var trf = new TrashedRegisterFinder(program, program.Procedures.Values, dfa.ProgramDataFlow, eventListener);
            trf.Compute();
            trf.RewriteBasicBlocks();
            RegisterLiveness.Compute(program, dfa.ProgramDataFlow, eventListener);
            foreach (Procedure proc in program.Procedures.Values)
            {
                LongAddRewriter larw = new LongAddRewriter(proc, program.Architecture);
                larw.Transform();
                Aliases alias = new Aliases(proc, program.Architecture, dfa.ProgramDataFlow);
                alias.Transform();
                alias.Write(writer);
                proc.Write(false, writer);
                writer.WriteLine();
            }
        }

        [Test]
        public void AlFactorialReg()
        {
            RunFileTest("Fragments/factorial_reg.asm", "Analysis/AlFactorialReg.txt");
        }

        [Test]
        public void AlAddSubCarries()
        {
            RunFileTest("Fragments/addsubcarries.asm", "Analysis/AlAddSubCarries.txt");
        }

        [Test]
        public void AlPreservedAlias()
        {
            RunFileTest("Fragments/multiple/preserved_alias.asm", "Analysis/AlPreservedAlias.txt");
        }

        [Test]
        public void AlReg00011()
        {
            RunFileTest("Fragments/regressions/r00011.asm", "Analysis/AlReg00011.txt");
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
            Identifier edx_eax = proc.Frame.EnsureSequence(edx.Storage, eax.Storage, PrimitiveType.Word64);
            Assignment ass = alias.CreateAliasInstruction(edx_eax, eax);
            Assert.AreEqual("eax = (word32) edx_eax (alias)", ass.ToString());
        }

        [Test]
        public void AliasSequenceToSlice()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
            Identifier edx_eax = proc.Frame.EnsureSequence(edx.Storage, eax.Storage, PrimitiveType.Word64);
            Identifier dh = proc.Frame.EnsureRegister(Registers.dh);
            Assignment ass = alias.CreateAliasInstruction(edx_eax, dh);
            Assert.AreEqual("dh = SLICE(edx_eax, byte, 40) (alias)", ass.ToString());
        }

        [Test]
        public void AliasSequenceToMkSequence()
        {
            Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
            Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
            Identifier edx_eax = proc.Frame.EnsureSequence(edx.Storage, eax.Storage, PrimitiveType.Word64);
            Assignment ass = alias.CreateAliasInstruction(eax, edx_eax);
            Assert.AreEqual("edx_eax = SEQ(edx, eax) (alias)", ass.ToString());

        }

        [Test]
        public void AliasStackArgument()
        {
            Identifier argOff = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word16);
            Identifier argPtr = proc.Frame.EnsureStackArgument(4, PrimitiveType.Ptr32);
            Assignment ass = alias.CreateAliasInstruction(argOff, argPtr);
            Assert.AreEqual("ptrArg04 = DPB(ptrArg04, wArg04, 0) (alias)", ass.ToString());
        }

        [Test]
        public void AliasFlags()
        {
            var sExp =
@"Mem0:Global memory (aliases:)
fp:fp (aliases:)
SZ:Flags (aliases: CZ C)
CZ:Flags (aliases: SZ C)
C:Flags (aliases: SZ CZ)
al:al (aliases:)
esi:esi (aliases:)
r63:r63 (aliases:)
// ProcedureBuilder
// Return size: 0
void ProcedureBuilder()
ProcedureBuilder_entry:
	// succ:  l1
l1:
	SZ = cond(esi & esi)
	C = false
	CZ = C
	al = Test(ULE,CZ)
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:

";
            RunStringTest(sExp, m =>
            {
                var scz = m.Frame.EnsureFlagGroup(Registers.eflags, 7, "SZ", PrimitiveType.Byte);
                var cz = m.Frame.EnsureFlagGroup(Registers.eflags, 3, "CZ", PrimitiveType.Byte);
                var c = m.Frame.EnsureFlagGroup(Registers.eflags, 1, "C", PrimitiveType.Bool);
                var al = m.Reg8("al", 0);
                var esi = m.Reg32("esi", 6);
                m.Assign(scz, m.Cond(m.And(esi, esi)));
                m.Assign(c, Constant.False());
                m.Alias(cz, c);
                m.Assign(al, m.Test(ConditionCode.ULE, cz));
                m.Return();
            });
        }
    }
}
