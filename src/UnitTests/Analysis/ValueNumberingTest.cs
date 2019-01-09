#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

namespace Reko.UnitTests.Analysis
{
	//[Ignore("Value number doesn't seem to be used anymore; this test and its associated class should probably die")]
	public class ValueNumberingTests : AnalysisTestBase
	{
        private SegmentMap segmentMap = new SegmentMap(Address.Ptr32(0));

		//[Test]
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
				Aliases alias = new Aliases(proc);
				alias.Transform();
				var gr = proc.CreateBlockDominatorGraph();
                SsaTransform sst = new SsaTransform(
                    new ProgramDataFlow(), 
                    proc,
                    null,
                    gr,
                    new HashSet<RegisterStorage>());
				SsaState ssa = sst.SsaState;
				ValueNumbering vn = new ValueNumbering(ssa.Identifiers, segmentMap, null);
				DumpProc(proc, ssa, fut.TextWriter);
				vn.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		private void DumpProc(Procedure proc, SsaState ssa, TextWriter writer)
		{
			ssa.Write(writer);
			proc.Write(false, writer);
		}

		//[Test]
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
				var gr = proc.CreateBlockDominatorGraph();
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(new ProgramDataFlow(), proc,  null, gr,
                    new HashSet<RegisterStorage>());
				SsaState ssa = sst.SsaState;
				ValueNumbering vn = new ValueNumbering(ssa.Identifiers, segmentMap, null);
				DumpProc(proc, ssa, fut.TextWriter);
				vn.Write(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		//[Test]
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
				var gr = proc.CreateBlockDominatorGraph();
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(new ProgramDataFlow(), proc, null, gr,
                    new HashSet<RegisterStorage>());
                SsaState ssa = sst.SsaState;
				DumpProc(proc, ssa, fut.TextWriter);

				DeadCode.Eliminate(proc, ssa);

				DumpProc(proc, ssa, fut.TextWriter);

				ValueNumbering vn = new ValueNumbering(ssa.Identifiers, segmentMap, null);
				vn.Write(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		//[Test]
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
				var gr = proc.CreateBlockDominatorGraph();
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(new ProgramDataFlow(), proc, null, gr,
                    new HashSet<RegisterStorage>());
                SsaState ssa = sst.SsaState;
				DumpProc(proc, ssa, fut.TextWriter);
				ValueNumbering vn = new ValueNumbering(ssa.Identifiers, segmentMap, null);
				vn.Write(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		//[Test]
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
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(new ProgramDataFlow(), proc, null, gr,
                    new HashSet<RegisterStorage>());
                SsaState ssa = sst.SsaState;
				DumpProc(proc, ssa, fut.TextWriter);
				ValueNumbering vn = new ValueNumbering(ssa.Identifiers, segmentMap, null);
				vn.Write(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		//[Test]
		public void VnFactorial()
		{
			RunFileTest("Fragments/factorial.asm", "Analysis/VnFactorial.txt");
		}

		//[Test]
		public void VnReg00001()
		{
			RunFileTest("Fragments/regression00001.asm", "Analysis/VnReg00001.txt");
		}

		//[Test]
		public void VnStringInstructions()
		{
			RunFileTest("Fragments/stringinstr.asm", "Analysis/VnStringInstructions.txt");
		}

		protected override void RunTest(Program program, TextWriter writer)
		{
            var progFlow = new ProgramDataFlow();
			foreach (Procedure proc in program.Procedures.Values)
			{
				var gr = proc.CreateBlockDominatorGraph();
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(progFlow, proc, null, gr,
                    new HashSet<RegisterStorage>());
                SsaState ssa = sst.SsaState;
				DumpProc(proc, ssa, writer);
				ValueNumbering vn = new ValueNumbering(ssa.Identifiers, segmentMap, null);
				vn.Write(writer);
				writer.WriteLine();
			}
		}
	}
}
