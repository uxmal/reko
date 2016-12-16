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

namespace Reko.UnitTests.Analysis
{
	public class ValueNumberingTests : AnalysisTestBase
	{
        private Program program;
        private ProgramDataFlow progFlow;
        private List<ValueNumbering> vns;

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
				ValueNumbering vn = new ValueNumbering(ssa);
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
				ValueNumbering vn = new ValueNumbering(ssa);
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
				SsaState ssa = sst.SsaState;
				ValueNumbering vn = new ValueNumbering(ssa);
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
                SsaState ssa = sst.SsaState;
				DumpProc(ssa, fut.TextWriter);

				DeadCode.Eliminate(ssa);

				DumpProc(ssa, fut.TextWriter);

				ValueNumbering vn = new ValueNumbering(ssa);
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
				ValueNumbering vn = new ValueNumbering(ssa);
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
				Aliases alias = new Aliases(proc, program.Architecture);
				alias.Transform();
				SsaTransform sst = new SsaTransform(
                    program,
                    proc,
                    new HashSet<Procedure>(),
                    null,
                    new ProgramDataFlow(program));
                SsaState ssa = sst.Transform();
				DumpProc(ssa, fut.TextWriter);
				ValueNumbering vn = new ValueNumbering(ssa);
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
	}
}
