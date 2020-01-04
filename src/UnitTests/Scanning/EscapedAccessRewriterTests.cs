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

using NUnit.Framework;
using Reko.Arch.X86;
using Reko.Assemblers.x86;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Services;
using Reko.Core.Types;
using Reko.Loading;
using Reko.Scanning;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Reko.UnitTests.Scanning
{
	[TestFixture]
    [Ignore("This needs to be rewritten, as we are now more explicitly referring to the stack pointer")]
	public class EscapedAccessRewriterTests
	{
        private ServiceContainer sc;

        [SetUp]
        public void Setup()
        {
            sc = new ServiceContainer();
            sc.AddService<IFileSystemService>(new FileSystemServiceImpl());
        }

		[Test]
		public void EarRewriteFrameAccess()
		{
			RunTest("Fragments/escapedframe1.asm",  Address.SegPtr(0xB00, 0), "Scanning/EarRewriteFrameAccess.txt");
		}

		[Test]
		public void EarAutoArray32()
		{
			RunTest("Fragments/autoarray32.asm", Address.Ptr32(0x04000000), "Scanning/EarAutoArray32.txt");
		}

		[Test]
		public void EarInsertFrameReference()
		{
			Procedure proc = new Procedure(null,"foo", Address.Ptr32(0x00123400), new Frame(PrimitiveType.Word32));
			Block b = new Block(proc, "foo_1");
			proc.ControlGraph.AddEdge(proc.EntryBlock, b);
            proc.ControlGraph.AddEdge(b, proc.ExitBlock);
			EscapedAccessRewriter ear = new EscapedAccessRewriter(proc);
			ear.InsertFramePointerAssignment(new Mocks.FakeArchitecture());
			Block x = proc.EntryBlock.Succ[0];
			Assert.AreEqual(1, x.Statements.Count);
			Assert.AreEqual("fp = &foo_frame", x.Statements[0].Instruction.ToString());
		}

		private Program AssembleFile(string sourceFile, Address addr)
		{
            var ldr = new Loader(new ServiceContainer());
            var arch = new X86ArchitectureReal("x86-real-16");
            Program program = ldr.AssembleExecutable(
                 FileUnitTester.MapTestPath(sourceFile),
                 new X86TextAssembler(sc, arch),
                addr);
            var project = new Project { Programs = { program } };
			var scan = new Scanner(
                program, 
                new DynamicLinker(project, program, null), null);
			foreach (ImageSymbol ep in program.EntryPoints.Values)
			{
				scan.EnqueueImageSymbol(ep, true);
			}
			scan.ScanImage();
			return program;
		}

		private void RunTest(string sourceFile, Address addr, string outputFile)
		{
			Program program = AssembleFile(sourceFile, addr);
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				foreach (Procedure proc in program.Procedures.Values)
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
