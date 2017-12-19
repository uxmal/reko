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

using Reko.Arch.X86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Types;
using Reko.Analysis;
using Reko.Evaluation;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class TrashRegisterFinderUpdateCodeTests
    {
        private ProgramBuilder p;
        private Program program;
        private ProgramDataFlow flow;
        private IntelArchitecture arch;
        private TrashedRegisterFinder trf;

        [SetUp]
        public void Setup()
        {
            arch = new X86ArchitectureFlat32();
            p = new ProgramBuilder();
        }

        private void RunTest(string sExp)
        {
            var sw = new StringWriter();
            program = p.BuildProgram(arch);
            RunTest(program, sw);
            try
            {
                Assert.AreEqual(sExp, sw.ToString());
            }
            catch
            {
                Console.WriteLine(sw);
                throw;
            }
        }

        private void RunTest(Program prog, TextWriter writer)
        {
            flow = new ProgramDataFlow(prog);
            trf = new TrashedRegisterFinder(prog, prog.Procedures.Values, this.flow, new FakeDecompilerEventListener());
            trf.Compute();
            trf.RewriteBasicBlocks();

            foreach (var proc in prog.Procedures.Values)
            {
                flow[proc].EmitRegisters(arch, "// Trashed", flow[proc].TrashedRegisters);
                proc.Write(false, writer);
                writer.WriteLine();
            }
        }

        [Test]
        public void Trfu_UpdateAllStackReferences()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var esp = m.Frame.EnsureRegister(Registers.esp);
                var ebp = m.Frame.EnsureRegister(Registers.ebp);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.Assign(eax, m.LoadDw(m.IAdd(ebp, 8)));
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });

            var sExp = @"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	esp = fp - 0x00000004
	dwLoc04 = ebp
	ebp = fp - 0x00000004
	eax = dwArg04
	ebp = dwLoc04
	esp = fp
	return
	// succ:  main_exit
main_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void Trfu_CallToFunction()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var ebx = m.Frame.EnsureRegister(Registers.ebx);
                var esp = m.Frame.EnsureRegister(Registers.esp);
                m.Assign(eax, 0x1234);
                m.Assign(ebx, 0x1234);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, eax);
                m.Call("foo", 4);
                m.Assign(ebx, eax);
                m.Return();
            });

            p.Add("foo", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(eax, 0x471100);
                m.Return();
            });

            var sExp = @"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	eax = 0x00001234
	ebx = 0x00001234
	esp = fp - 0x00000004
	dwLoc04 = 0x00001234
	call foo (retsize: 4; depth: 8)
	ebx = 0x00471100
	return
	// succ:  main_exit
main_exit:

// foo
// Return size: 0
void foo()
foo_entry:
	// succ:  l1
l1:
	eax = 0x00471100
	return
	// succ:  foo_exit
foo_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void Trfu_DontCopyRegistersUnlessFramepointer()
        {
            p.Add("main", m =>
            {
                var ecx = m.Frame.EnsureRegister(Registers.ecx);
                var esi = m.Frame.EnsureRegister(Registers.esi);
                m.Assign(esi, ecx);
                m.Call("foo", 4);
                m.Assign(esi, m.IAdd(esi, 1));
                m.Return();
            });

            p.Add("foo", m =>
            {
                var esp = m.Frame.EnsureRegister(Registers.esp);
                var esi = m.Frame.EnsureRegister(Registers.esi);
                var ecx = m.Frame.EnsureRegister(Registers.ecx);

                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, esi);
                m.Assign(ecx, m.ISub(ecx, 1));
                m.Assign(esi, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });

            var sExp = @"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	esi = ecx
	call foo (retsize: 4; depth: 4)
	esi = esi + 0x00000001
	return
	// succ:  main_exit
main_exit:

// foo
// Return size: 0
void foo()
foo_entry:
	// succ:  l1
l1:
	esp = fp - 0x00000004
	dwLoc04 = esi
	ecx = ecx - 0x00000001
	esi = dwLoc04
	esp = fp
	return
	// succ:  foo_exit
foo_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void Trfu_PromoteMemoryAccessParametersToApiCalls()
        {
            p.Add("main", m =>
            {
                var esp = m.Frame.EnsureRegister(Registers.esp);
                var ebp = m.Frame.EnsureRegister(Registers.ebp);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(ebp, esp);
                m.SideEffect(m.Fn(
                    new ProcedureConstant(PrimitiveType.Word32, new ExternalProcedure("strcpy", null)),
                    m.LoadDw(m.IAdd(ebp, 8)),
                    m.LoadDw(m.IAdd(ebp, 12))));
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });

            var sExp =
@"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	esp = fp - 0x00000004
	dwLoc04 = ebp
	ebp = fp - 0x00000004
	strcpy(dwArg04, dwArg08)
	ebp = dwLoc04
	esp = fp
	return
	// succ:  main_exit
main_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void Trfu_PromoteMemoryAccessParametersToApiCallsWithLocalPushes()
        {
            p.Add("main", m =>
            {
                var esp = m.Frame.EnsureRegister(Registers.esp);
                var ebp = m.Frame.EnsureRegister(Registers.ebp);
                var eax = m.Frame.EnsureRegister(Registers.eax);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, ebp);
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, m.Word32(55));
                m.Assign(esp, m.ISub(esp, 4));
                m.Store(esp, m.Word32(45));
                m.Assign(
                    eax,
                    m.Fn(
                        new ProcedureConstant(PrimitiveType.Word32, new ExternalProcedure("add", null)),
                        m.LoadDw(esp),
                        m.LoadDw(m.IAdd(esp, 4))));
                m.Assign(esp, m.IAdd(esp, 8));
                m.Assign(ebp, m.LoadDw(esp));
                m.Assign(esp, m.IAdd(esp, 4));
                m.Return();
            });

            var sExp =
@"// main
// Return size: 0
void main()
main_entry:
	// succ:  l1
l1:
	esp = fp - 0x00000004
	dwLoc04 = ebp
	esp = fp - 0x00000008
	dwLoc08 = 0x00000037
	esp = fp - 0x0000000C
	dwLoc0C = 0x0000002D
	eax = add(0x0000002D, 0x00000037)
	esp = fp - 0x00000004
	ebp = dwLoc04
	esp = fp
	return
	// succ:  main_exit
main_exit:

";
            RunTest(sExp);
        }

        [Test]
        [Ignore("Unhappy about this, but we probably want to do this later in the analysis")]
        public void Trfu_ReplaceLongAdds()
        {
            p.Add("main", m =>
            {
                var eax = m.Frame.EnsureRegister(Registers.eax);
                var edx = m.Frame.EnsureRegister(Registers.edx);
                var ebx = m.Frame.EnsureRegister(Registers.ebx);
                var ecx = m.Frame.EnsureRegister(Registers.ecx);
                var SCZO = m.Frame.EnsureFlagGroup(Registers.eflags, (uint) (FlagM.ZF | FlagM.CF | FlagM.SF | FlagM.OF), "SCZO", PrimitiveType.Byte);
                var C = m.Frame.EnsureFlagGroup(Registers.eflags, (uint) (FlagM.CF), "SCZO", PrimitiveType.Bool);

                m.Assign(eax, m.IAdd(eax, ecx));
                m.Assign(SCZO, m.Cond(eax));
                m.Assign(edx, m.IAdd(m.IAdd(edx, ebx), C));
                m.Assign(SCZO, m.Cond(edx));
                m.Return();
            });

            var sExp = @"// main
void main()
main_entry:
	// succ:  l1
l1:
	edx_eax = edx_eax + ebx_ecx
	SCZO = cond(edx_eax)
	return
	// succ: main_exit
main_exit:

";
            RunTest(sExp);
        }

        [Test]
        public void Trfu_ProcIsolation()
        {
            AnalysisTestBase.RunTest("Fragments/multiple/procisolation.asm", RunTest, "Analysis/TrfuProcIsolation.txt");
        }

        [Test]
        public void Trfu_PushedRegisters()
        {
            AnalysisTestBase.RunTest("Fragments/multiple/pushed_registers.asm", RunTest, "Analysis/TrfuPushedRegisters.txt");
        }

        [Test]
        [Ignore("A spurious goto statement is being emitted. Analysis-branch should clean this up")]
        public void Trfu_WhileLoop()
        {
            AnalysisTestBase.RunTest("Fragments/while_loop.asm", RunTest, "Analysis/TrfuWhileLoop.txt");
        }

        [Test]
        public void Trfu_Reg00005()
        {
            AnalysisTestBase.RunTest("Fragments/regressions/r00005.asm", RunTest, "Analysis/TrfuReg00005.txt");
        }

        [Test]
        public void Trfu_LiveLoopMock()
        {
            p.Add(new LiveLoopMock().Procedure);

            var sExp = @"// LiveLoopMock
// Return size: 0
void LiveLoopMock()
LiveLoopMock_entry:
	goto loop
	// succ:  loop
l1:
	return y
	// succ:  LiveLoopMock_exit
loop:
	y = i
	i = i + 1
	branch Mem0[i:byte] != 0 loop
	goto l1
	// succ:  l1 loop
LiveLoopMock_exit:

";
            RunTest(sExp);
        }
    }
}
