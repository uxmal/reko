/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Arch.Intel.MsDos;
using Decompiler.Arch.Intel.Assembler;
using Decompiler.Core;
using Decompiler.Core.Types;
using Decompiler.Loading;
using Decompiler.Scanning;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;
using NUnit.Framework;
using ProcessorState = Decompiler.Core.ProcessorState;

namespace Decompiler.UnitTests.Intel
{
	[TestFixture]	
	public class CodeWalkerTests
	{
		private Program BuildProgram()
		{
			Program prog = new Program();
			IntelAssembler asm = new IntelAssembler();
			ProgramImage img = asm.AssembleFragment(
				prog,
				new Address(0xB96, 0),
				"       .i86\r\n" +
				"mane	proc\r\n" +
				"		mov	cx,3\r\n" +
				"foo:	mov	ax,[bx]\r\n" +
				"		add	bx,2\r\n" +
				"		dec cx\r\n" +
				"		jnz foo\r\n" +
				"		ret\r\n");

			prog.Image = img;
			prog.Architecture = new IntelArchitecture(ProcessorMode.Real);
			return prog; 
		}

		[Test]
		public void FindJumps()
		{
			Program pgm = BuildProgram();
			Scanner sc = new Scanner(pgm, null);

			sc.EnqueueEntryPoint(new EntryPoint(pgm.Image.BaseAddress, new IntelState()));
			sc.ProcessQueues();
			Assert.AreEqual(4, pgm.Image.Map.Items.Count);
			StringBuilder sb = new StringBuilder();
			foreach (DictionaryEntry de in pgm.Image.Map.Items)
			{
				sb.Append(de.Value.ToString() + "\r\n");
			}
			Debug.WriteLine(sb.ToString());
			Assert.AreEqual(
				"ImageMapBlock: 0B96:0000, size: 0003\r\n" +
				"ImageMapBlock: 0B96:0003, size: 0008\r\n" + 
				"ImageMapBlock: 0B96:000B, size: 0001\r\n" +
				"0B96:000C, size: 0\r\n",
				sb.ToString());
		}

		[Test]
		public void WalkFactorial()
		{
			using (FileUnitTester fut = new FileUnitTester("intel/WalkFactorial.txt"))
			{
				Program prog = new Program();
				Loader ld = new Loader(prog);
				ld.Assemble(FileUnitTester.MapTestPath("fragments/Factorial.asm"), new IntelArchitecture(ProcessorMode.Real), new Address(0x0C00, 0));
				Scanner sc = new Scanner(prog, null);
				foreach (EntryPoint ep in ld.EntryPoints)
				{
					sc.EnqueueEntryPoint(ep);
				}
				sc.ProcessQueues();
				prog.DumpAssembler(fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void WalkSwitch()
		{
			RunAsmTest("fragments/switch.asm", "intel/WalkSwitch.txt", ProcessorMode.Real, new Address(0x0C00, 0));
		}

		[Test]
		public void WalkSwitch32()
		{
			RunAsmTest("fragments/switch32.asm", "intel/WalkSwitch32.txt", ProcessorMode.ProtectedFlat, new Address(0x01000000));
		}

		[Test]
		public void WalkCallVector()
		{
			RunAsmTest("fragments/multiple/calltables.asm", "intel/WalkCallTables.txt", ProcessorMode.Real, new Address(0xC00, 0));
		}

		[Test]
		public void WalkServiceCall()
		{
			// Checks to see if a sequence return value (es:bx) trashes the state appropriately.
			IntelState state = new IntelState();
			state.Set(Registers.es, new Value(PrimitiveType.Word16, 0));	
			state.Set(Registers.es, new Value(PrimitiveType.Word16, 0));

			state.Set(Registers.ah, new Value(PrimitiveType.Word16, 0x2F));
			IntelInstruction instr = new IntelInstruction(Opcode.@int, PrimitiveType.Word16, PrimitiveType.Word16,
				new ImmediateOperand(PrimitiveType.Byte, 0x21));

			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.Real);
			TestCodeWalkerListener listener = new TestCodeWalkerListener();
			IntelCodeWalker cw = new IntelCodeWalker(arch, new MsdosPlatform(arch), null, state, listener);
			cw.WalkInstruction(new Address(0x100, 0x100), instr, null);
			Assert.IsFalse(state.Get(Registers.es).IsValid, "should have trashed ES");
			Assert.IsFalse(state.Get(Registers.bx).IsValid, "should have trashed BX");
			Assert.AreEqual(1, listener.SystemCalls.Count);
		}

		[Test]
		public void WalkBswap()
		{
			IntelState state = new IntelState();
			state.Set(Registers.ebp, new Value(PrimitiveType.Word32, 0x12345678));
			IntelInstruction instr = new IntelInstruction(Opcode.bswap, PrimitiveType.Word32, PrimitiveType.Word32, 
				new RegisterOperand(Registers.ebp));

			IntelArchitecture arch = new IntelArchitecture(ProcessorMode.ProtectedFlat);
			IntelCodeWalker cw = new IntelCodeWalker(arch, null, null, state, null);
			cw.WalkInstruction(new Address(0x100000), instr, null);
			Assert.AreSame(Value.Invalid,  state.Get(Registers.ebp));
		}

		private class TestCodeWalkerListener : ICodeWalkerListener
		{
			private SortedList syscalls = new SortedList();

			#region ICodeWalkerListener Members

			public void OnJumpTable(Decompiler.Core.ProcessorState st, Address addrInstr, Address addrTable, ushort segBase, PrimitiveType stride)
			{
				// TODO:  Add TestCodeWalkerListener.OnJumpTable implementation
			}

			public void OnIllegalOpcode(Address addrIllegal)
			{
				// TODO:  Add TestCodeWalkerListener.OnIllegalOpcode implementation
			}

			public void OnProcessExit(Address addrTerm)
			{
				// TODO:  Add TestCodeWalkerListener.OnProcessExit implementation
			}

			public void OnSystemServiceCall(Address addrInstr, SystemService svc)
			{
				syscalls[addrInstr] = svc;
			}

			public void OnBranch(Decompiler.Core.ProcessorState st, Address addrInstr, Address addrTerm, Address addrBranch)
			{
				// TODO:  Add TestCodeWalkerListener.OnBranch implementation
			}

			public void OnJump(Decompiler.Core.ProcessorState st, Address addrInstr, Address addrTerm, Address addrJump)
			{
				// TODO:  Add TestCodeWalkerListener.OnJump implementation
			}

			public void OnTrampoline(Decompiler.Core.ProcessorState st, Address addrInstr, Address addrGlob)
			{
				// TODO:  Add TestCodeWalkerListener.OnTrampoline implementation
			}

			public void OnJumpPointer(Decompiler.Core.ProcessorState st, Address segBase, Address addrPtr, PrimitiveType stride)
			{
				// TODO:  Add TestCodeWalkerListener.OnJumpPointer implementation
			}

			public void OnProcedurePointer(Decompiler.Core.ProcessorState st, Address addrBase, Address addrPtr, PrimitiveType stride)
			{
				// TODO:  Add TestCodeWalkerListener.OnProcedurePointer implementation
			}

			public void OnReturn(Address addrTerm)
			{
				// TODO:  Add TestCodeWalkerListener.OnReturn implementation
			}

			public void OnGlobalVariable(Address addr, PrimitiveType width, Value v)
			{
				// TODO:  Add TestCodeWalkerListener.OnGlobalVariable implementation
			}

			public void OnProcedureTable(Decompiler.Core.ProcessorState st, Address addrInstr, Address addrTable, ushort segBase, PrimitiveType stride)
			{
				// TODO:  Add TestCodeWalkerListener.OnProcedureTable implementation
			}

			public void OnProcedure(Decompiler.Core.ProcessorState st, Address addr)
			{
				// TODO:  Add TestCodeWalkerListener.OnProcedure implementation
			}

			public void Warn(string format, params object [] args)
			{
			}

			#endregion

			public SortedList SystemCalls { get { return syscalls; } }
		}

		private void RunAsmTest(string sourceFile, string outputFile, ProcessorMode mode, Address addrBase)
		{
			using (FileUnitTester fut = new FileUnitTester(outputFile))
			{
				Program prog = new Program();
				prog.Architecture = new IntelArchitecture(mode);
				Loader ld = new Loader(prog);
				ld.Assemble(FileUnitTester.MapTestPath(sourceFile), prog.Architecture, addrBase);
				Scanner sc = new Scanner(prog, null);
				foreach (EntryPoint ep in ld.EntryPoints)
				{
					sc.EnqueueEntryPoint(ep);
				}
				sc.ProcessQueues();
				Dumper d = prog.Architecture.CreateDumper();
				d.Dump(prog, prog.Image.Map, fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}
	}
}
