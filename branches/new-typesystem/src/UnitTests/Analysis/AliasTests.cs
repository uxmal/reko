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
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.Analysis;
using Decompiler.UnitTests;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class AliasTests : AnalysisTestBase
	{
		[Test]
		public void AlFactorialReg()
		{
			RunTest("Fragments/factorial_reg.asm", "Analysis/AlFactorialReg.txt");
		}

		[Test]
		public void AlAddSubCarries()
		{
			RunTest("Fragments/addsubcarries.asm", "Analysis/AlAddSubCarries.txt");
		}

		[Test]
		public void AlPreservedAlias()
		{
			RunTest("Fragments/multiple/preserved_alias.asm", "Analysis/AlPreservedAlias.txt");
		}

		[Test]
		public void AliasWideToNarrow()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Int16));
			Aliases alias = new Aliases(proc, new IntelArchitecture(ProcessorMode.Real));
			Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
			Identifier ax =  proc.Frame.EnsureRegister(Registers.ax);
			Assignment ass = alias.CreateAliasInstruction(eax.Number, ax.Number);
			Assert.AreEqual("ax = (word16) eax (alias)", ass.ToString());
		}

		[Test]
		public void AliasWideToNarrowSlice()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Int16));
			Aliases alias = new Aliases(proc, new IntelArchitecture(ProcessorMode.Real));
			Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
			Identifier ah =  proc.Frame.EnsureRegister(Registers.ah);
			Assignment ass = alias.CreateAliasInstruction(eax.Number, ah.Number);
			Assert.AreEqual("ah = SLICE(eax, byte, 8) (alias)", ass.ToString());
		}

		[Test]
		public void AliasSequenceToElement()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Int16));
			Aliases alias = new Aliases(proc, new IntelArchitecture(ProcessorMode.Real));
			Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
			Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
			Identifier edx_eax = proc.Frame.EnsureSequence(edx, eax, PrimitiveType.Word64);
			Assignment ass = alias.CreateAliasInstruction(edx_eax.Number, eax.Number);
			Assert.AreEqual("eax = (word32) edx_eax (alias)", ass.ToString());
		}

		[Test]
		public void AliasSequenceToSlice()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Int16));
			Aliases alias = new Aliases(proc, new IntelArchitecture(ProcessorMode.Real));
			Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
			Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
			Identifier edx_eax = proc.Frame.EnsureSequence(edx, eax, PrimitiveType.Word64);
			Identifier dh = proc.Frame.EnsureRegister(Registers.dh);
			Assignment ass = alias.CreateAliasInstruction(edx_eax.Number, dh.Number);
			Assert.AreEqual("dh = SLICE(edx_eax, byte, 40) (alias)", ass.ToString());
		}

		[Test]
		public void AliasSequenceToMkSequence()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Word16));
			Aliases alias = new Aliases(proc, new IntelArchitecture(ProcessorMode.Real));
			Identifier eax = proc.Frame.EnsureRegister(Registers.eax);
			Identifier edx = proc.Frame.EnsureRegister(Registers.edx);
			Identifier edx_eax = proc.Frame.EnsureSequence(edx, eax, PrimitiveType.Word64);
			Assignment ass = alias.CreateAliasInstruction(eax.Number, edx_eax.Number);
			Assert.AreEqual("edx_eax = SEQ(edx, eax) (alias)", ass.ToString());
			
		}

		[Test]
		public void AliasStackArgument()
		{
			Procedure proc = new Procedure("foo", new Frame(PrimitiveType.Word16));
			Aliases alias = new Aliases(proc, new IntelArchitecture(ProcessorMode.Real));
			Identifier argOff = proc.Frame.EnsureStackArgument(4, PrimitiveType.Word16);
			Identifier argSeg = proc.Frame.EnsureStackArgument(6, PrimitiveType.Word16);
			Identifier argPtr = proc.Frame.EnsureStackArgument(4, PrimitiveType.Pointer32);
			Assignment ass = alias.CreateAliasInstruction(argOff.Number, argPtr.Number);
			Assert.AreEqual("ptrArg04 = DPB(ptrArg04, wArg04, 0, 16) (alias)", ass.ToString());
		}

		protected override void RunTest(Program prog, FileUnitTester fut)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerHost());
			TrashedRegisterFinder trf = new TrashedRegisterFinder(prog, dfa.ProgramDataFlow);
			trf.Compute();
			RegisterLiveness rl = RegisterLiveness.Compute(prog, dfa.ProgramDataFlow);
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture, dfa.ProgramDataFlow);
				alias.Transform();
				alias.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
			}
		}
	}
}
