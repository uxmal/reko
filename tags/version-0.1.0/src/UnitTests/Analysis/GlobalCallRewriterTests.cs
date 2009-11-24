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
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class GlobalCallRewriterTests
	{
		private Program prog;
		private GlobalCallRewriter gcr;
		private Procedure proc;
		private ProcedureFlow flow;

		[SetUp]
		public void Setup()
		{
			prog = new Program();
			prog.Architecture = new IntelArchitecture(ProcessorMode.ProtectedFlat);
			gcr = new GlobalCallRewriter(prog, null);
            proc = new Procedure("foo", prog.Architecture.CreateFrame());
			flow = new ProcedureFlow(proc, prog.Architecture);
		}

		[Test]
		public void RegisterArgument()
		{
			flow.MayUse[Registers.eax.Number] = true;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("void foo(Register word32 eax)", proc.Signature.ToString(proc.Name));
		}

		[Test]
		public void RegisterOutArgument()
		{
			flow.LiveOut[Registers.eax.Number] = true;		// becomes the return value.
			flow.LiveOut[Registers.ebx.Number] = true;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("Register word32 foo(Register out ptr32 ebxOut)", proc.Signature.ToString(proc.Name));
		}

		[Test]
		public void FpuArgument()
		{
			proc.Frame.EnsureFpuStackVariable(1, PrimitiveType.Real80);
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("void foo(FpuStack real80 rArg1)", proc.Signature.ToString(proc.Name));
		}

		[Test]
		public void FpuOutArgument()
		{
			flow.LiveOut[Registers.eax.Number] = true;
			proc.Frame.EnsureFpuStackVariable(0, PrimitiveType.Real80);
			proc.Frame.EnsureFpuStackVariable(1, PrimitiveType.Real80);
			proc.Signature.FpuStackDelta = 1;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("Register word32 foo(FpuStack real80 rArg0, FpuStack real80 rArg1, FpuStack out ptr32 rArg0Out)", proc.Signature.ToString(proc.Name));
		}

		[Test]
		public void NarrowedStackArgument()
		{
			Identifier arg = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word32);
			flow.StackArguments[arg] = 16;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("void foo(Stack uipr16 dwArg04)", proc.Signature.ToString(proc.Name));
		}


		// Ensure that UseInstructions for "out" parameters are generated even when a signature is pre-specified.
		[Test]
		public void GenerateUseInstructionsForSpecifiedSignature()
		{
            Procedure proc = new Procedure("foo", prog.Architecture.CreateFrame());
			proc.Signature = new ProcedureSignature(
				new Identifier("eax", 0, PrimitiveType.Word32, new RegisterStorage(Registers.eax)),
				new Identifier [] { 
					new Identifier("ecx", 1, PrimitiveType.Word32, new RegisterStorage(Registers.ecx)),
					new Identifier("edxOut", 2, PrimitiveType.Word32, 
									  new OutArgumentStorage(proc.Frame.EnsureRegister(Registers.edx)))});
			gcr.EnsureSignature(proc, null);
			gcr.AddUseInstructionsForOutArguments(proc);
			Assert.AreEqual(1, proc.ExitBlock.Statements.Count);
			Assert.AreEqual("use edx (=> edxOut)", proc.ExitBlock.Statements[0].Instruction.ToString());

		}

		[Test]
		public void GcrStackArguments()
		{
            Frame f = prog.Architecture.CreateFrame();
			f.ReturnAddressSize = PrimitiveType.Word16.Size;

			f.EnsureStackVariable(new Constant(PrimitiveType.Word16, 8), 2, PrimitiveType.Word16);
			f.EnsureStackVariable(new Constant(PrimitiveType.Word16, 6), 2, PrimitiveType.Word16);
			f.EnsureStackVariable(new Constant(PrimitiveType.Word16, 0x0E), 2, PrimitiveType.Word32);

			GlobalCallRewriter gcr = new GlobalCallRewriter(null, null);
			using (FileUnitTester fut = new FileUnitTester("Analysis/GcrStackParameters.txt"))
			{
				foreach (KeyValuePair<int,Identifier> de in gcr.GetSortedStackArguments(f))
				{
					fut.TextWriter.Write("{0:X4} ", de.Key);
					de.Value.Write(true, fut.TextWriter);
					fut.TextWriter.WriteLine();
				}
				fut.AssertFilesEqual();
			}
		}

		private class NestedProgram
		{
			public static Program Build()
			{
				ProgramMock m = new ProgramMock();
				m.Add(new MainFn());
				m.Add(new Leaf());
				return m.BuildProgram();
			}

			public class MainFn : ProcedureMock
			{
				private ArchitectureMock arch = new ArchitectureMock();

				protected override void BuildBody()
				{
					base.Call("Leaf");
					Store(Int32(0x320123), base.Register(0));
				}
			}

			public class Leaf : ProcedureMock
			{
				protected override void BuildBody()
				{
					Assign(Register(0), Int32(3));
					Return();
				}
			}
		}

	}
}
