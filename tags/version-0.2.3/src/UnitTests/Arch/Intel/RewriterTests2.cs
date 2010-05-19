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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Arch.Intel;
using Decompiler.Scanning;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;

namespace Decompiler.UnitTests.Arch.Intel
{
	[TestFixture]
	public class RewriterTests2 : RewriterTestBase
	{
		public RewriterTests2()
		{
		}
	
		[Test]
		public void RwSwitch()
		{
			DoRewriteFile("Fragments/switch.asm");
			using (FileUnitTester fut = new FileUnitTester("Intel/RwSwitch.txt"))
			{
				prog.Procedures.Values[0].Write(false, fut.TextWriter);
			}
		}

		[Test]
		public void RwDivideTests()
		{
			Procedure proc = DoRewrite(@".i86
	mov	ebx,32
	mov eax,100
	cdq
	idiv ebx
	mov cx,[si]
	mov ax,[si+2]
	mov dx,[si+4]
	div cx
");
			using (FileUnitTester fut = new FileUnitTester("Intel/RwDivideTests.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void RwMemOperations()
		{
			DoRewriteFile("Fragments/memoperations.asm");
			using (FileUnitTester fut = new FileUnitTester("Intel/RwMemOperations.txt"))
			{
				prog.Procedures.Values[0].Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void RwCallTable()
		{
			DoRewriteFile("Fragments/multiple/calltables.asm");
			using (FileUnitTester fut = new FileUnitTester("Intel/RwCallTable.txt"))
			{
				Dumper dump = prog.Architecture.CreateDumper();
				dump.Dump(prog, prog.Image.Map, fut.TextWriter);
				fut.TextWriter.WriteLine();
				prog.CallGraph.Emit(fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void RwAlloca()
		{
			ConfigFile = "Fragments/multiple/alloca.xml";
			RunTest("Fragments/multiple/alloca.asm", "Intel/RwAlloca.txt");
		}

		[Test]
		public void RwStackVariables()
		{
			RunTest("Fragments/stackvars.asm", "Intel/RwStackVariables.txt");
		}

		[Test]
		public void RwDuff()
		{
			RunTest("Fragments/duffs_device.asm", "Intel/RwDuff.txt");
		}

		[Test]
		public void RwFactorial()
		{
			RunTest("Fragments/factorial.asm", "Intel/RwFactorial.txt");
		}

		[Test]
		public void RwLoopne()
		{
			RunTest("Fragments/loopne.asm", "Intel/RwLoopne.txt");
		}

		[Test]
		public void RwInterprocedureJump()
		{
			RunTest("Fragments/multiple/jumpintoproc.asm", "Intel/RwInterprocedureJump.txt");
		}

		[Test]
		public void RwPopNoPop()
		{
			RunTest("Fragments/multiple/popnopop.asm", "Intel/RwPopNoPop.txt");
		}

		[Test]
		public void RwMultiplication()
		{
			RunTest("Fragments/multiplication.asm", "Intel/RwMultiplication.txt");
		}

		[Test]
		public void RwStackPointerMessing()
		{
			RunTest("Fragments/multiple/stackpointermessing.asm", "Intel/RwStackPointerMessing.txt");
		}

		[Test]
		public void RwStringInstructions()
		{
			RunTest("Fragments/stringinstr.asm", "Intel/RwStringInstructions.txt");
		}

		[Test]
		public void RwTestCondition()
		{
			RunTest("Fragments/setcc.asm", "Intel/RwTestCondition.txt");
		}

		[Test]
		public void RwCopyFile()
		{
			RunTest("Fragments/copy_file.asm", "Intel/RwCopyFile.txt");
		}

		[Test]
		public void RwReadFile()
		{
			RunTest("Fragments/multiple/read_file.asm", "Intel/RwReadFile.txt");
		}


		[Test]
		public void RwProcIsolation()
		{
			RunTest("Fragments/multiple/procisolation.asm", "Intel/RwProcIsolation.txt");
		}

		private void RunTest(string sourceFile, string outputFile)
		{
			DoRewriteFile(sourceFile);
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				foreach (Procedure proc in prog.Procedures.Values)
					proc.Write(true, fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}
	}
}
