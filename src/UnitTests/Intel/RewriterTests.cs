/* 
 * Copyright (C) 1999-2007 John Källén.
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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Serialization;
using Decompiler.Arch.Intel;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Decompiler.UnitTests.Intel
{
	public class RewriterTestBase
	{
		private string configFile;
		protected Assembler asm; 
		protected Program prog;
		protected Scanner scanner;
		protected Address baseAddress;

		public RewriterTestBase()
		{
			baseAddress = new Address(0xC00, 0);
		}

		[SetUp]
		public void SetUp()
		{
			prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			asm = prog.Architecture.CreateAssembler();
			configFile = null;
		}

		public string ConfigFile
		{
			get { return configFile; }
			set { configFile = value; }
		}

		protected Procedure DoRewrite(string code)
		{
			prog.Image = asm.AssembleFragment(prog, baseAddress, code);
			prog.ImageMap = new ImageMap(prog.Image);
			DoRewriteCore();
			return prog.DfsProcedures[0];
		}

		private void DoRewriteCore()
		{
			DecompilerProject project = (configFile != null)
				? DecompilerProject.Load(FileUnitTester.MapTestPath(configFile))
				: null;
			scanner = new Scanner(prog, null);
			EntryPoint ep = new EntryPoint(baseAddress, prog.Architecture.CreateProcessorState());
			ArrayList eps = new ArrayList();
			eps.Add(ep);
			scanner.Parse(eps, project != null ? project.UserProcedures : null);
			RewriterHost rw = new RewriterHost(prog, null, scanner.SystemCalls, scanner.VectorUses);
			rw.RewriteProgram();
		}

		protected void DoRewriteFile(string relativePath)
		{
			prog.Image = asm.Assemble(prog, baseAddress, FileUnitTester.MapTestPath(relativePath), null);
			prog.ImageMap = new ImageMap(prog.Image);
			DoRewriteCore();
		}
	}

	/// <summary>
	/// Unit Tests for the Intel code rewriter.
	/// </summary>

	[TestFixture]
	public class RewriterTests : RewriterTestBase
	{
		[Test]
		public void SimpleTest()
		{
			DoRewrite(
				@"	.i86
	mov	ax,0x0000
	mov	cx,0x10
	add	ax,cx
	ret
");

			Assert.IsTrue(prog.Procedures.Count == 1);
			Assert.IsTrue(prog.DfsProcedures.Count == 1);
			Procedure proc = prog.DfsProcedures[0];
			Assert.IsTrue(proc.RpoBlocks.Count == 3);		// Entry, code, Exit

			Block block = proc.RpoBlocks[0].Succ[0];
			Assert.IsTrue(block.Statements.Count == 5);
			Assignment instr1 = (Assignment) block.Statements[0].Instruction;
			Assert.IsTrue(block.Statements[1].Instruction is Assignment);

			Assert.IsTrue(block.Succ[0] == proc.ExitBlock);
		}

		[Test]
		public void IfTest()
		{
			Procedure proc = DoRewrite(
				@"	.i86
	cmp	bx,ax
	jnz	not_eq

	mov	cx,3
	jmp	join
not_eq:
	mov	cx,2
join:
	ret
");
			Assert.IsTrue(proc.RpoBlocks.Count == 6);
			StringWriter sb = new StringWriter();
			proc.Write(true, sb);
		}

		[Test]
		public void RwDeadConditionals()
		{
			DoRewriteFile("Fragments/small_loop.asm");
			Procedure proc = prog.DfsProcedures[0];
			using (FileUnitTester fut = new FileUnitTester("Intel/RwDeadConditionals.txt"))
			{
				proc.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
			Assert.IsTrue(proc.RpoBlocks.Count == 5);
		}

		[Test]
		public void RwPseudoProcs()
		{
			DoRewriteFile("Fragments/pseudoprocs.asm");
			Procedure proc = prog.DfsProcedures[0];
			using (FileUnitTester fut = new FileUnitTester("Intel/RwPseudoProcs.txt"))
			{
				proc.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
			Assert.IsTrue(proc.RpoBlocks.Count == 3);
		}

		[Test]
		public void RwAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Intel/RwAddSubCarries.txt");
		}

		[Test]
		public void RwLongAddSub()
		{
			RunTest("Fragments/longaddsub.asm", "Intel/RwLongAddSub.txt");
		}

		[Test]
		public void RwEnterLeave()
		{
			RunTest("Fragments/enterleave.asm", "Intel/RwEnterLeave.txt");
		}

		[Test]
		public void RwReg00003()
		{
			RunTest("Fragments/regressions/r00003.asm", "Intel/RwReg00003.txt");
		}

		[Test]
		public void RwReg00005()
		{
			RunTest("Fragments/regressions/r00005.asm", "Intel/RwReg00005.txt");
		}

		[Test]
		public void RwSequenceShifts()
		{
			RunTest("Fragments/sequenceshift.asm", "Intel/RwSequenceShifts.txt");
		}

		[Test]
		public void RwLogical()
		{
			RunTest("Fragments/logical.asm", "Intel/RwLogical.txt");
		}

		[Test]
		public void RwNegsNots()
		{
			RunTest("Fragments/negsnots.asm", "Intel/RwNegsNots.txt");
		}

		[Test]
		public void RwFpuArgs()
		{
			RunTest("Fragments/multiple/fpuargs.asm", "Intel/RwFpuArgs.txt");
		}

		[Test]
		public void RwFpuOps()
		{
			RunTest("Fragments/fpuops.asm", "Intel/RwFpuOps.txt");
		}

		[Test]
		public void RwFpuReversibles()
		{
			RunTest("Fragments/fpureversibles.asm", "Intel/RwFpuReversibles.txt");
		}

		private void RunTest(string sourceFile, string outputFile)
		{
			DoRewriteFile(sourceFile);
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				foreach (Procedure proc in prog.DfsProcedures)
				{
					proc.Write(true, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				fut.AssertFilesEqual();
			}
		}
	}
}
