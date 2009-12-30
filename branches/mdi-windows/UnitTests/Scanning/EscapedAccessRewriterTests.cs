/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Loading;
using Decompiler.Scanning;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Scanning
{
	[TestFixture]
	public class EscapedAccessRewriterTests
	{
		[Test]
		public void EarRewriteFrameAccess()
		{
			RunTest("Fragments/escapedframe1.asm",  new Address(0xB00, 0), "Scanning/EarRewriteFrameAccess.txt");
		}

		[Test]
		public void EarAutoArray32()
		{
			RunTest("Fragments/autoarray32.asm", new Address(0x04000000), "Scanning/EarAutoArray32.txt");
		}

		[Test]
		public void EarInsertFrameReference()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Word32));
			Block b = new Block(proc, "foo_1");
			proc.AddEdge(proc.EntryBlock, b);
			proc.AddEdge(b, proc.ExitBlock);
			EscapedAccessRewriter ear = new EscapedAccessRewriter(proc);
			ear.InsertFramePointerAssignment(new Mocks.ArchitectureMock());
			Block x = proc.EntryBlock.Succ[0];
			Assert.AreEqual(1, x.Statements.Count);
			Assert.AreEqual("fp = &foo_frame", x.Statements[0].Instruction.ToString());
		}

		private Program AssembleFile(string sourceFile, Address addr)
		{
            AssemblerLoader ldr = new AssemblerLoader(
                new IntelTextAssembler(),
                FileUnitTester.MapTestPath(sourceFile));
            Program prog = ldr.Load(addr);
			Scanner scan = new Scanner(prog, null);
			foreach (EntryPoint ep in ldr.EntryPoints)
			{
				scan.EnqueueEntryPoint(ep);
			}
			scan.ProcessQueues();
			RewriterHost host = new RewriterHost(prog, null, scan.SystemCalls, scan.VectorUses);
			host.RewriteProgram();
			return prog;
		}

		private void RunTest(string sourceFile, Address addr, string outputFile)
		{
			Program prog = AssembleFile(sourceFile, addr);
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					fut.TextWriter.WriteLine("= Before ==========");
					proc.Write(true, fut.TextWriter);
					if (proc.Frame.Escapes)
					{
						fut.TextWriter.WriteLine("The frame of procedure {0} escapes", proc.Name);
						EscapedAccessRewriter ear = new EscapedAccessRewriter(proc);
						ear.Transform();
						fut.TextWriter.WriteLine("= After ==========");
						proc.Write(true, fut.TextWriter);
					}
					else
						fut.TextWriter.WriteLine("The frame of procedure {0} doesn't escape", proc.Name);
					fut.TextWriter.WriteLine();

				}
				fut.AssertFilesEqual();
			}
		}
	}
}
