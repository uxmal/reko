#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using NUnit.Framework;
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Reko.Core.Lib;
using Reko.UnitTests.Mocks;
using Reko.Core.Types;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    [Ignore("May not be needed after all")]
    public class ValueNumberingTests : AnalysisTestBase
	{
        private Program program;
        private ProgramDataFlow progFlow;
        private List<ValueNumbering> vns;
        private FakeDecompilerEventListener listener;

        [SetUp]
        public void Setup()
        {
            this.listener = new FakeDecompilerEventListener();
            vns = new List<ValueNumbering>();
        }

        protected override void RunTest(Program program, TextWriter writer)
        {
            this.program = program;
            this.progFlow = new ProgramDataFlow(program);

            var sscf = new SccFinder<Procedure>(new ProcedureGraph(program), ProcessScc);
            foreach (var procedure in program.Procedures.Values)
            {
                sscf.Find(procedure);
            }

            foreach (var vn in vns)
            {
                vn.Procedure.Write(false, writer);
                vn.Write(writer);
                writer.WriteLine();
            }
        }

        void ProcessScc(IList<Procedure> scc)
        {
            var group = new HashSet<Procedure>(scc);
            foreach (var proc in scc)
            { 
                var sst = new SsaTransform(program, proc, group, null, progFlow);
                SsaState ssa = sst.Transform();
                var vp = new ValuePropagator(program.Architecture, ssa, listener);
                vp.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
                sst.AddUsesToExitBlock();
                vp.Transform();
			    var vn = new ValueNumbering(group, listener);
                vn.Compute(ssa);
                vns.Add(vn);
			}
		}

		[Test]
		public void VnSumTest()
		{
			Program prog = RewriteCodeFragment(
				@".i86
	push bp
	mov	 bp,sp
	mov	dx,3
	add dx,dx

	mov bx,3
	lea dx,[bx+3]
	mov sp,bp
	pop	bp
	ret
	");
			using (FileUnitTester fut = new FileUnitTester("Analysis/VnSumTest.txt"))
			{
				Procedure proc = prog.Procedures.Values[0];
                SsaTransform sst = new SsaTransform(
                    prog,
                    proc,
                    new HashSet<Procedure>(),
                    null,
                    new ProgramDataFlow(prog));
				SsaState ssa = sst.Transform();
				ValueNumbering vn = new ValueNumbering(new HashSet<Procedure> { proc }, listener);
				DumpProc(ssa, fut.TextWriter);
				vn.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void DumpProc(SsaState ssa, TextWriter writer)
		{
			ssa.Write(writer);
			ssa.Procedure.Write(false, writer);
		}

		[Test]
		public void VnMemoryTest()
		{
			Program program = RewriteCodeFragment(
				@".i86
	mov word ptr [bx+2],0
	mov si,[bx+4]
	mov ax,[bx+2]
	mov cx,[bx+2]
	mov dx,[bx+4]
	ret
");
			using (FileUnitTester fut = new FileUnitTester("Analysis/VnMemoryTest.txt"))
			{
				Procedure proc = program.Procedures.Values[0];
                SsaTransform sst = new SsaTransform(program, proc, new HashSet<Procedure>(),
                    null, new ProgramDataFlow(program));
                SsaState ssa = sst.Transform();
				ValueNumbering vn = new ValueNumbering(new HashSet<Procedure> { proc }, listener);
                vn.Compute(ssa);
				DumpProc(ssa, fut.TextWriter);
				vn.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void VnLoopTest()
		{
			Program program = this.RewriteCodeFragment(
				@".i86
	mov	ax,1
	mov	bx,1
isdone:
	cmp	ax,10
	jz  done

	inc ax
	inc bx
	jmp isdone
done:
	mov [0002],ax
	mov	[0004],bx
	ret
");
			using (FileUnitTester fut = new FileUnitTester("Analysis/VnLoopTest.txt"))
			{
				Procedure proc = program.Procedures.Values[0];
                SsaTransform sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    null,
                    new ProgramDataFlow(program));
                SsaState ssa = sst.Transform();
				DumpProc(ssa, fut.TextWriter);

				DeadCode.Eliminate(ssa);

				DumpProc(ssa, fut.TextWriter);

				ValueNumbering vn = new ValueNumbering(new HashSet<Procedure> { proc }, listener);
                vn.Compute(ssa);
				vn.Write(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void VnRedundantStore()
		{
			Program program = RewriteCodeFragment(
				@".i86
	mov	ax,2
isdone:
	cmp	bx,10
	jz  yay
boo: mov ax,3
	jmp done
yay:
	mov ax,3
done:
	ret
");
			using (FileUnitTester fut = new FileUnitTester("Analysis/VnRedundantStore.txt"))
			{
				Procedure proc = program.Procedures.Values[0];
				SsaTransform sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    null,
                    new ProgramDataFlow(program));
                SsaState ssa = sst.Transform();
				DumpProc(ssa, fut.TextWriter);
				ValueNumbering vn = new ValueNumbering(new HashSet<Procedure> { proc }, listener);
                vn.Compute(ssa);
				vn.Write(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void VnLoop()
		{
			Program program = RewriteCodeFragment(@".i86
	push ax
	jmp looptest
again:
    mov si,[0x302]
	mov ax,[si+04]
	add [si+06],ax
looptest:
    cmp	ax,bx
	jl again

	pop ax
	ret
");
			using (FileUnitTester fut = new FileUnitTester("Analysis/VnLoop.txt"))
			{
				Procedure proc = program.Procedures.Values[0];
				var gr = proc.CreateBlockDominatorGraph();
				SsaTransform sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    null,
                    new ProgramDataFlow(program));
                SsaState ssa = sst.Transform();
                sst.RenameFrameAccesses = true;
                sst.Transform();
				DumpProc(ssa, fut.TextWriter);
				ValueNumbering vn = new ValueNumbering(new HashSet<Procedure> { proc }, listener);
                vn.Compute(ssa);
				vn.Write(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void VnFactorial()
		{
			RunFileTest_x86_real("Fragments/factorial.asm", "Analysis/VnFactorial.txt");
		}

		[Test]
		public void VnReg00001()
		{
            RunFileTest_x86_real("Fragments/regression00001.asm", "Analysis/VnReg00001.txt");
		}

		[Test]
		public void VnStringInstructions()
		{
            RunFileTest_x86_real("Fragments/stringinstr.asm", "Analysis/VnStringInstructions.txt");
		}

        [Test]
        public void VnFactorial_CalleeCleanup()
        {
            var pb = new ProgramBuilder();
            pb.Add("main", m =>
            {
                var r1 = m.Frame.EnsureRegister(pb.Program.Architecture.GetRegister(1));
                var sp = m.Frame.EnsureRegister(pb.Program.Architecture.StackRegister);
                m.Assign(r1, 10);
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, r1);
                m.Call("foo", 4);
                m.Store(m.Word32(0x00123400), r1);
                m.Return();
            });
            pb.Add("foo", m =>
            {
                var r1 = m.Frame.EnsureRegister(pb.Program.Architecture.GetRegister(1));
                var r2 = m.Frame.EnsureRegister(pb.Program.Architecture.GetRegister(2));
                var sp = m.Frame.EnsureRegister(pb.Program.Architecture.StackRegister);
                m.Assign(r1, m.LoadDw(m.IAdd(sp, 8)));
                m.BranchIf(m.Le(r1, 1), "m00002");

                m.Label("m00001");
                m.Assign(sp, m.ISub(sp, 4));
                m.Store(sp, m.ISub(r1, 1));
                m.Call("foo", 0);
                m.Assign(r1, m.IMul(r1, m.ISub(sp, 8)));
                m.Goto("m00003");

                m.Label("m00002");
                m.Assign(r1, 1);

                m.Label("m00003");
                m.Assign(sp, m.IAdd(sp, 4));
                m.Return();
            });
            this.program = pb.BuildProgram();
            this.program.Platform = new DefaultPlatform(null, program.Architecture);
            this.progFlow = new ProgramDataFlow(program);

            var sscf = new SccFinder<Procedure>(new ProcedureGraph(program), ProcessScc);
            foreach (var procedure in program.Procedures.Values)
            {
                sscf.Find(procedure);
            }

            var writer = new StringWriter();
            foreach (var vn in vns)
            {
                vn.Procedure.Write(false, writer);
                vn.Write(writer);
                writer.WriteLine();
            }

            var sExp = "@@@";
            if (sExp != writer.ToString())
            {
                Console.WriteLine(writer.ToString());
                Assert.AreEqual(sExp, writer.ToString());
            }
        }
	}
}
