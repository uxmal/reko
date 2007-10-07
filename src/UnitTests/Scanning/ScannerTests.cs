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
using Decompiler.Arch.Intel;
using Decompiler.Scanning;
using Decompiler.Loading;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Collections;

namespace Decompiler.UnitTests.Scanning
{
	[TestFixture]
	public class ScannerTests
	{
		public ScannerTests()
		{
		}

		[Test]
		public void BuildExpr()
		{
			Regexp re;
			re = Regexp.Compile("11.22");
			Debug.WriteLine(re);
			re = Regexp.Compile("34+32+33");
			Debug.WriteLine(re);
			re = Regexp.Compile(".*11221122");
			Debug.WriteLine(re);
			re = Regexp.Compile("11(22|23)*44");
			Assert.IsTrue(re.Match(new Byte [] { 0x11, 0x22, 0x22, 0x23, 0x44 }, 0));
			re = Regexp.Compile("(B8|B9)*0204");
			Assert.IsTrue(re.Match(new Byte [] { 0xB8, 0x02, 0x04 }, 0));
			re = Regexp.Compile("C390*");
			Assert.IsTrue(re.Match(new Byte [] { 0xC3, 0x90, 0x90, 0x90, 0xB8 }, 0));
		}

		[Test]
		public void MatchTest()
		{
			byte [] data = new byte [] {
										   0x30, 0x34, 0x32, 0x12, 0x55, 0xC3, 0xB8, 0x34, 0x00 
									   };

			Regexp re = Regexp.Compile(".*55C3");
			Assert.IsTrue(re.Match(data, 0));
		}

		[Test]
		public void CallGraphTree()
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Assembler asm = prog.Architecture.CreateAssembler();
			prog.Image = asm.AssembleFragment(prog, new Address(0xC00, 0), 
				@".i86
main proc
	call baz
	ret
main endp

foo proc
	ret
foo endp

bar proc
	ret
bar endp

baz proc
	call	foo
	call	bar
	jmp		foo
baz endp
");
			prog.ImageMap = new ImageMap(prog.Image);
			EntryPoint ep = new EntryPoint(prog.Image.BaseAddress, new IntelState());
			Scanner scan = new Scanner(prog, null);
			ArrayList eps = new ArrayList();
			eps.Add(ep);
			scan.Parse(eps);
			RewriterHost rw = new RewriterHost(prog, null, scan.SystemCalls, scan.VectorUses);
			rw.RewriteProgram();
		}

		/// <summary>
		/// Avoid promoting stumps that contain short sequences of code.
		/// </summary>
		[Test]
		public void DontPromoteStumps()
		{
			Program prog = BuildTest("Fragments/multiple/jumpintoproc2.asm");
			prog.ImageMap = new ImageMap(prog.Image);
			Scanner scan = new Scanner(prog, null);
			scan.EnqueueProcedure(null, prog.Image.BaseAddress, null, prog.Architecture.CreateProcessorState());
			Assert.IsTrue(scan.ProcessItem());
			Assert.IsTrue(scan.ProcessItem());
			Assert.IsTrue(scan.ProcessItem());
			Assert.IsTrue(scan.ProcessItem());
//			Assert.IsTrue(scan.ProcessItem());
			DumpImageMap(prog.ImageMap);
		}

		[Test]
		public void ScanInterprocedureJump()
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Assembler asm = prog.Architecture.CreateAssembler();
			prog.Image = asm.Assemble(prog, new Address(0xC00, 0x0000), FileUnitTester.MapTestPath("Fragments/multiple/jumpintoproc.asm"), null);
			prog.ImageMap = new ImageMap(prog.Image);
			EntryPoint ep = new EntryPoint(asm.StartAddress, new IntelState());
			ArrayList eps = new ArrayList();
			eps.Add(ep);
			Scanner scan = new Scanner(prog, null);
			scan.Parse(eps);

			using (FileUnitTester fut = new FileUnitTester("Scanning/ScanInterprocedureJump.txt"))
			{
				Dumper dumper = prog.Architecture.CreateDumper();
				dumper.Dump(prog, prog.ImageMap, fut.TextWriter);
				foreach (DictionaryEntry de in prog.Procedures)
				{
					Procedure proc = (Procedure) de.Value;
					fut.TextWriter.WriteLine("{0} (@ {1})", proc.Name, de.Key);
				}
				fut.AssertFilesEqual();
			}
		}

		[Test]
		[Ignore("Need to implement this feature")]
		public void ObeyDontDecompileUserProcedure()
		{
		}

		private Program BuildTest(string srcFile)
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Assembler asm = prog.Architecture.CreateAssembler();
			ArrayList entryPoints = new ArrayList();
			prog.Image = asm.Assemble(prog, new Address(0x0C00, 0x0000),FileUnitTester.MapTestPath(srcFile), entryPoints);
			return prog;
		}

		private void DumpImageMap(ImageMap map)
		{
			foreach (ImageMapItem item in map.Items.Values)
			{
				Console.WriteLine(item);
			}
		}
	}
}
