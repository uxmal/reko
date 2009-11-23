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

using Decompiler;
using Decompiler.Core;
using Decompiler.Core.Assemblers;
using Decompiler.Arch.Intel;
using Decompiler.Assemblers.x86;
using Decompiler.Scanning;
using Decompiler.Loading;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Decompiler.UnitTests.Scanning
{
	[TestFixture]
	public class ScannerTests
	{
		private Program prog;
		private TestScanner scanner;

		public ScannerTests()
		{
		}

		private void SetupMockCodeWalker()
		{
			prog = new Program();
			prog.Architecture = new ArchitectureMock();
			prog.Image = new ProgramImage(new Address(0x1000), new byte[0x4000]);
			scanner = new TestScanner(prog);
			scanner.MockCodeWalker = new MockCodeWalker(new Address(0x1000));
			scanner.EnqueueEntryPoint(new EntryPoint(new Address(0x1000), prog.Architecture.CreateProcessorState()));
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
			Assert.IsTrue(re.Match(data, 0), "Should have matched");
		}

		[Test]
		public void CallGraphTree()
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Assembler asm = new IntelTextAssembler();
			asm.AssembleFragment(new Address(0xC00, 0), 
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
            prog.Image = asm.Image;
			Scanner scan = new Scanner(prog, null);
			EntryPoint ep = new EntryPoint(prog.Image.BaseAddress, new IntelState());
			prog.AddEntryPoint(ep);
			scan.EnqueueEntryPoint(ep);
			scan.ProcessQueues();
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
			Scanner scan = new Scanner(prog, null);
			scan.EnqueueProcedure(null, prog.Image.BaseAddress, null, prog.Architecture.CreateProcessorState());
			Assert.IsTrue(scan.ProcessItem());
			Assert.IsTrue(scan.ProcessItem());
			Assert.IsTrue(scan.ProcessItem());
			Assert.IsTrue(scan.ProcessItem());
//			Assert.IsTrue(scan.ProcessItem());
			DumpImageMap(prog.Image.Map);
		}

		[Test]
		public void ScanInterprocedureJump()
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			Assembler asm = new IntelTextAssembler();
			asm.Assemble(new Address(0xC00, 0x0000), FileUnitTester.MapTestPath("Fragments/multiple/jumpintoproc.asm"));
            prog.Image = asm.Image;
			Scanner scan = new Scanner(prog, null);
			scan.EnqueueEntryPoint(new EntryPoint(asm.StartAddress, new IntelState()));
			scan.ProcessQueues();
			using (FileUnitTester fut = new FileUnitTester("Scanning/ScanInterprocedureJump.txt"))
			{
				Dumper dumper = prog.Architecture.CreateDumper();
				dumper.Dump(prog, prog.Image.Map, fut.TextWriter);
                foreach (KeyValuePair<Address, Procedure> de in prog.Procedures)
				{
					fut.TextWriter.WriteLine("{0} (@ {1})", de.Value.Name, de.Key);
				}
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void ScanSimple()
		{
			SetupMockCodeWalker();

			scanner.MockCodeWalker.AddReturn(new Address(0x1004));
			scanner.ProcessQueues();

			Assert.AreEqual(1, prog.Procedures.Count);

		}

		[Test]
		public void ScanTwoProcedures()
		{
			SetupMockCodeWalker();

			scanner.MockCodeWalker.AddCall(new Address(0x1001), new Address(0x2000), new FakeProcessorState());
			scanner.MockCodeWalker.AddReturn(new Address(0x1002));

			scanner.MockCodeWalker.AddReturn(new Address(0x2002));

			scanner.ProcessQueues();

			Assert.AreEqual(2, prog.Procedures.Count);
		}

		[Test]
		public void ScanProcJumpingIntoOther()
		{
			SetupMockCodeWalker();
			scanner.MockCodeWalker.AddCall(new Address(0x1001), new Address(0x1100));
			scanner.MockCodeWalker.AddReturn(new Address(0x1002));

			scanner.MockCodeWalker.AddJump(new Address(0x1102), new Address(0x1103)); 
			scanner.MockCodeWalker.AddReturn(new Address(0x1110));

			scanner.ProcessQueues();
			scanner.EnqueueProcedure(null, new Address(0x2000), null, new FakeProcessorState());
			scanner.MockCodeWalker.AddCall(new Address(0x2001), new Address(0x1101));	// calls into middle of procedure already scanned.
			scanner.MockCodeWalker.AddReturn(new Address(0x2004));

			scanner.ProcessQueues();

			foreach (ImageMapItem item in prog.Image.Map.Items.Values)
			{
                ImageMapBlock b = item as ImageMapBlock;
				if (b != null)
					Console.WriteLine("{0}, part of {1}", b, b.Procedure);
			}

			foreach (Procedure proc in prog.Procedures.Values)
			{
				Console.WriteLine("{0}", proc);
			}
			Assert.AreEqual(3, prog.Procedures.Count);
			Procedure p2000 = prog.Procedures[new Address(0x2000)];
			Procedure p1100 = prog.Procedures[new Address(0x1100)];
			Procedure p1101 = prog.Procedures[new Address(0x1101)];
			Assert.IsNotNull(p2000);
			Assert.IsNotNull(p1100);
			Assert.IsNotNull(p1101);
			ImageMapBlock b1100 = GetBlockAt(0x1100);
			ImageMapBlock b1101 = GetBlockAt(0x1101);
			ImageMapBlock b1103 = GetBlockAt(0x1103);
			Assert.AreSame(p1100, b1100.Procedure);
			Assert.AreSame(p1101, b1101.Procedure);
			Assert.AreSame(p1101, b1103.Procedure);
		}

		private ImageMapBlock GetBlockAt(uint a)
		{
            ImageMapItem item;
			prog.Image.Map.TryFindItemExact(new Address(a), out item);
            return (ImageMapBlock) item;
		}

		[Test]
		[Ignore("Need to implement this feature")]
		public void ObeyDontDecompileUserProcedure()
		{
			//$REVIEW: a directive that introduces a procedure signature, but inhibits its decompilation.
		}

		private Program BuildTest(string srcFile)
		{
			Program prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
            Assembler asm = new IntelTextAssembler();
			asm.Assemble(new Address(0x0C00, 0x0000), FileUnitTester.MapTestPath(srcFile));
            prog.Image = asm.Image;
			return prog;
		}

		private void DumpImageMap(ImageMap map)
		{
			foreach (ImageMapItem item in map.Items.Values)
			{
				Console.WriteLine(item);
			}
		}

		private class TestScanner : Scanner
		{
			private MockCodeWalker mcw; 

			public TestScanner(Program prog) : base(prog, null)
			{
			}

			public override CodeWalker CreateCodeWalker(Address addr, ProcessorState state)
			{
				if (mcw != null)
				{
					mcw.SetWalkAddress(addr);
					return mcw;
				}
				else
					return base.CreateCodeWalker(addr, state);
			}

			public MockCodeWalker MockCodeWalker
			{
				get { return mcw; }
				set { mcw = value; }
			}
		}
	}
}
