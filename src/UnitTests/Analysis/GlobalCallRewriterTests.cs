#region License
/* 
 * Copyright (C) 1999-2017 John Källén.
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

using Reko.Arch.X86;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Rhino.Mocks;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class GlobalCallRewriterTests
	{
		private Program program;
		private GlobalCallRewriter gcr;
		private Procedure proc;
		private ProcedureFlow flow;

		[SetUp]
		public void Setup()
		{
			program = new Program();
			program.Architecture = new X86ArchitectureFlat32();
            program.Platform = new DefaultPlatform(null, program.Architecture);
			gcr = new GlobalCallRewriter(program, null, new FakeDecompilerEventListener());
            proc = new Procedure("foo", program.Architecture.CreateFrame());
			flow = new ProcedureFlow(proc, program.Architecture);
		}

		[Test]
		public void RegisterArgument()
		{
            flow.MayUse.Add(Registers.eax); ;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("void foo(Register word32 eax)", proc.Signature.ToString(proc.Name));
		}

		[Test]
		public void RegisterOutArgument()
		{
			flow.LiveOut.Add(Registers.eax);		// becomes the return value.
			flow.LiveOut.Add(Registers.ebx);
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
            flow.LiveOut.Add(Registers.eax);
			proc.Frame.EnsureFpuStackVariable(0, PrimitiveType.Real80);
			proc.Frame.EnsureFpuStackVariable(1, PrimitiveType.Real80);
			proc.Signature.FpuStackDelta = 1;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("Register word32 foo(FpuStack real80 rArg0, FpuStack real80 rArg1, FpuStack out ptr32 rArg0Out)", proc.Signature.ToString(proc.Name));
		}

		[Test]
		public void NarrowedStackArgument()
		{
			var arg = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word32);
			flow.StackArguments[arg] = 16;
			gcr.EnsureSignature(proc, flow);
			Assert.AreEqual("void foo(Stack uipr16 dwArg04)", proc.Signature.ToString(proc.Name));
		}

		// Ensure that UseInstructions for "out" parameters are generated even when a signature is pre-specified.
		[Test]
		public void GenerateUseInstructionsForSpecifiedSignature()
		{
            Procedure proc = new Procedure("foo", program.Architecture.CreateFrame());
			proc.Signature = new FunctionType(
				new Identifier("eax", PrimitiveType.Word32, Registers.eax),
				new Identifier [] { 
					new Identifier("ecx", PrimitiveType.Word32, Registers.ecx),
					new Identifier("edxOut", PrimitiveType.Word32, 
									  new OutArgumentStorage(proc.Frame.EnsureRegister(Registers.edx)))});
			gcr.EnsureSignature(proc, null);
			gcr.AddUseInstructionsForOutArguments(proc);
			Assert.AreEqual(1, proc.ExitBlock.Statements.Count);
			Assert.AreEqual("use edx (=> edxOut)", proc.ExitBlock.Statements[0].Instruction.ToString());
		}

		[Test]
		public void GcrStackArguments()
		{
            var f = program.Architecture.CreateFrame();
            f.ReturnAddressKnown = true;
			f.ReturnAddressSize = PrimitiveType.Word16.Size;

			f.EnsureStackVariable(Constant.Word16( 8), 2, PrimitiveType.Word16);
			f.EnsureStackVariable(Constant.Word16( 6), 2, PrimitiveType.Word16);
			f.EnsureStackVariable(Constant.Word16( 0x0E), 2, PrimitiveType.Word32);

			GlobalCallRewriter gcr = new GlobalCallRewriter(null, null, new FakeDecompilerEventListener());
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
				ProgramBuilder m = new ProgramBuilder();
				m.Add(new MainFn());
				m.Add(new Leaf());
				return m.BuildProgram();
			}

			public class MainFn : ProcedureBuilder
			{
				private FakeArchitecture arch = new FakeArchitecture();

				protected override void BuildBody()
				{
					base.Call("Leaf", 4);
					Store(Int32(0x320123), base.Register(0));
				}
			}

			public class Leaf : ProcedureBuilder
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
