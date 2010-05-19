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

using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class OutParameterTransformerTests : AnalysisTestBase
	{
		private Program prog; 

		[Test]
		public void OutpAsciiHex()
		{
			prog = RewriteFile("Fragments/ascii_hex.asm");
			FileUnitTester.RunTest("Analysis/OutpAsciiHex.txt", new FileUnitTestHandler(PerformTest));
		}

		private void PerformTest(FileUnitTester fut)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(prog, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in prog.Procedures.Values)
			{
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				SsaTransform sst = new SsaTransform(proc, proc.CreateBlockDominatorGraph(), false);
				SsaState ssa = sst.SsaState;

				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();

				OutParameterTransformer opt = new OutParameterTransformer(proc, ssa.Identifiers);
				opt.Transform();

				DeadCode.Eliminate(proc, ssa);

				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine("====================");
			}
		}

		[Test]
		public void OutpReplaceSimple()
		{
            ProcedureMock m = new ProcedureMock();
            Block block = m.Label("block");
			Identifier foo = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier pfoo = new Identifier("pfoo", 0, PrimitiveType.Pointer32, null);
            Statement stmDef = m.Assign(foo, 3);
			SsaIdentifier sid = new SsaIdentifier(foo, foo, stmDef, null, false);

			SsaIdentifierCollection ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(sid);

			OutParameterTransformer opt = new OutParameterTransformer(null, ssaIds);
			opt.ReplaceDefinitionsWithOutParameter(foo, pfoo);

			Assert.AreEqual("store(Mem0[pfoo:word32]) = 0x00000003", stmDef.Instruction.ToString());
		}

		[Test]
		public void OutpReplacePhi()
		{
            ProcedureMock m = new ProcedureMock();
			Identifier foo = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier foo1 = new Identifier("foo1", 0, PrimitiveType.Word32, null);
			Identifier foo2 = new Identifier("foo2", 1, PrimitiveType.Word32, null);
			Identifier foo3 = new Identifier("foo3", 2, PrimitiveType.Word32, null);
			Identifier pfoo = new Identifier("pfoo", 4, PrimitiveType.Pointer32, null);

            Block block1 = m.Label("block1");
            Statement stmFoo1 = m.Assign(foo1, Constant.Word32(1));
            Block block2 = m.Label("block2");
            Statement stmFoo2 = m.Assign(foo2, Constant.Word32(2));
            Block block3 = m.Label("block3");
            Statement stmFoo3 = m.Phi(foo3, foo1, foo2);

			SsaIdentifierCollection ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(new SsaIdentifier(foo1, foo, stmFoo1, null, false));
			ssaIds.Add(new SsaIdentifier(foo2, foo, stmFoo2, null, false));
			ssaIds.Add(new SsaIdentifier(foo3, foo, stmFoo3, null, false));

			OutParameterTransformer opt = new OutParameterTransformer(null, ssaIds);
			opt.ReplaceDefinitionsWithOutParameter(foo3, pfoo);

			Assert.AreEqual("store(Mem0[pfoo:word32]) = 0x00000001", stmFoo1.Instruction.ToString());
			Assert.AreEqual("store(Mem0[pfoo:word32]) = 0x00000002", stmFoo2.Instruction.ToString());
			Assert.AreEqual("foo3 = PHI(foo1, foo2)", stmFoo3.Instruction.ToString());

		}

		/// <summary>
		/// Tests the following scenario:
		///		foo = 3
		///		bar = foo
		///	and foo is an out register.
		///	This should result in:
		///		foo = 3
		///		*pfoo = foo
		///		bar = foo
		/// </summary>
		[Test]
		public void OutpReplaceManyUses()
		{
            ProcedureMock m = new ProcedureMock();
			Identifier foo = new Identifier("foo", 0, PrimitiveType.Word32, null);
			Identifier bar = new Identifier("bar", 1, PrimitiveType.Word32, null);
			Identifier pfoo = new Identifier("pfoo", 2, PrimitiveType.Pointer32, null);

            Block block = m.Label("block");
            Statement stmFoo = m.Assign(foo, 1);
            Statement stmBar = m.Assign(bar, foo);

			SsaIdentifier ssaFoo = new SsaIdentifier(foo, foo, stmFoo, ((Assignment) stmFoo.Instruction).Src, false);
			ssaFoo.Uses.Add(stmBar);
            SsaIdentifier ssaBar = new SsaIdentifier(bar, bar, stmBar, ((Assignment) stmBar.Instruction).Src, false);

			SsaIdentifierCollection ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(ssaFoo);
			ssaIds.Add(ssaBar);

			OutParameterTransformer opt = new OutParameterTransformer(m.Procedure, ssaIds);
			opt.ReplaceDefinitionsWithOutParameter(foo, pfoo);
			Assert.AreEqual(3, block.Statements.Count);
			Assert.AreEqual("foo = 0x00000001", block.Statements[0].Instruction.ToString());
			Assert.AreEqual("store(Mem0[pfoo:word32]) = foo", block.Statements[1].Instruction.ToString());
			Assert.AreEqual("bar = foo", block.Statements[2].Instruction.ToString());
		}

		[Test]
		public void OutpParameters()
		{
			prog = RewriteFile("Fragments/multiple/outparameters.asm");
			FileUnitTester.RunTest("Analysis/OutpParameters.txt", new FileUnitTestHandler(PerformTest));
		}

		[Test]
		public void OutpMutual()
		{
			prog = RewriteFile("Fragments/multiple/mutual.asm");
			FileUnitTester.RunTest("Analysis/OutpMutual.txt", new FileUnitTestHandler(PerformTest));
		}

		private class IdentifierComparer : IComparer
		{
			#region IComparer Members

			public int Compare(object x, object y)
			{
				if (x == null && y == null)
					return 0;
				Identifier a = (Identifier) x;
				Identifier b = (Identifier) y;
				return a.Name.CompareTo(b.Name);
			}

			#endregion

		}

	}
}
