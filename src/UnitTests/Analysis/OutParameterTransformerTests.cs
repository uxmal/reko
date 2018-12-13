#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.IO;
using System.Collections;

namespace Reko.UnitTests.Analysis
{
	[TestFixture]
	public class OutParameterTransformerTests : AnalysisTestBase
	{
		private Program program; 

		[Test]
		public void OutpAsciiHex()
		{
			program = RewriteFile("Fragments/ascii_hex.asm");
			FileUnitTester.RunTest("Analysis/OutpAsciiHex.txt", PerformTest);
		}

		private void PerformTest(FileUnitTester fut)
		{
			DataFlowAnalysis dfa = new DataFlowAnalysis(program, null, new FakeDecompilerEventListener());
			dfa.UntangleProcedures();
			foreach (Procedure proc in program.Procedures.Values)
			{
				Aliases alias = new Aliases(proc);
				alias.Transform();
				SsaTransform sst = new SsaTransform(
                    dfa.ProgramDataFlow,
                    proc,
                    null,
                    proc.CreateBlockDominatorGraph(),
                    program.Platform.CreateImplicitArgumentRegisters());
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
            var m = new ProcedureBuilder();
            m.Label("block");
			var foo = new Identifier("foo", PrimitiveType.Word32, null);
			var pfoo = new Identifier("pfoo", PrimitiveType.Ptr32, null);
            m.Assign(foo, 3);
			var sid = new SsaIdentifier(foo, foo, m.Block.Statements.Last, null, false);

            var ssaIds = new SsaIdentifierCollection { { foo, sid } };

			var opt = new OutParameterTransformer(null, ssaIds);
			opt.ReplaceDefinitionsWithOutParameter(foo, pfoo);

			Assert.AreEqual("*pfoo = 0x00000003", m.Block.Statements[0].ToString());
		}

		[Test]
		public void OutpReplacePhi()
		{
            var m = new ProcedureBuilder();
			var foo = new Identifier("foo", PrimitiveType.Word32, null);
			var foo1 = new Identifier("foo1", PrimitiveType.Word32, null);
			var foo2 = new Identifier("foo2", PrimitiveType.Word32, null);
			var foo3 = new Identifier("foo3", PrimitiveType.Word32, null);
			var pfoo = new Identifier("pfoo", PrimitiveType.Ptr32, null);

            m.Label("block1");
            m.Assign(foo1, Constant.Word32(1));
            Statement stmFoo1 = m.Block.Statements.Last;
            m.Label("block2");
            m.Assign(foo2, Constant.Word32(2));
            Statement stmFoo2 = m.Block.Statements.Last;
            m.Label("block3");
            Statement stmFoo3 = m.Phi(foo3, (foo1, "block1"), (foo2, "block2"));

			SsaIdentifierCollection ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(foo1, new SsaIdentifier(foo1, foo, stmFoo1, null, false));
			ssaIds.Add(foo2, new SsaIdentifier(foo2, foo, stmFoo2, null, false));
			ssaIds.Add(foo3, new SsaIdentifier(foo3, foo, stmFoo3, null, false));

			OutParameterTransformer opt = new OutParameterTransformer(null, ssaIds);
			opt.ReplaceDefinitionsWithOutParameter(foo3, pfoo);

			Assert.AreEqual("*pfoo = 0x00000001", stmFoo1.Instruction.ToString());
			Assert.AreEqual("*pfoo = 0x00000002", stmFoo2.Instruction.ToString());
			Assert.AreEqual("foo3 = PHI((foo1, block1), (foo2, block2))", stmFoo3.Instruction.ToString());

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
            ProcedureBuilder m = new ProcedureBuilder();
			Identifier foo = new Identifier("foo", PrimitiveType.Word32, null);
			Identifier bar = new Identifier("bar", PrimitiveType.Word32, null);
			Identifier pfoo = new Identifier("pfoo", PrimitiveType.Ptr32, null);

            Block block = m.Label("block");
            m.Assign(foo, 1);
            Statement stmFoo = m.Block.Statements.Last;
            m.Assign(bar, foo);
            Statement stmBar = m.Block.Statements.Last;

			SsaIdentifier ssaFoo = new SsaIdentifier(foo, foo, stmFoo, ((Assignment) stmFoo.Instruction).Src, false);
			ssaFoo.Uses.Add(stmBar);
            SsaIdentifier ssaBar = new SsaIdentifier(bar, bar, stmBar, ((Assignment) stmBar.Instruction).Src, false);

			SsaIdentifierCollection ssaIds = new SsaIdentifierCollection();
			ssaIds.Add(foo, ssaFoo);
			ssaIds.Add(bar, ssaBar);

			OutParameterTransformer opt = new OutParameterTransformer(m.Procedure, ssaIds);
			opt.ReplaceDefinitionsWithOutParameter(foo, pfoo);
			Assert.AreEqual(3, block.Statements.Count);
			Assert.AreEqual("foo = 0x00000001", block.Statements[0].Instruction.ToString());
			Assert.AreEqual("*pfoo = foo", block.Statements[1].Instruction.ToString());
			Assert.AreEqual("bar = foo", block.Statements[2].Instruction.ToString());
		}

		[Test]
		public void OutpParameters()
		{
			program = RewriteFile("Fragments/multiple/outparameters.asm");
			FileUnitTester.RunTest("Analysis/OutpParameters.txt", PerformTest);
		}

		[Test]
		public void OutpMutual()
		{
			program = RewriteFile("Fragments/multiple/mutual.asm");
			FileUnitTester.RunTest("Analysis/OutpMutual.txt", PerformTest);
		}

		private class IdentifierComparer : IComparer
		{
			public int Compare(object x, object y)
			{
				if (x == null && y == null)
					return 0;
				Identifier a = (Identifier) x;
				Identifier b = (Identifier) y;
				return a.Name.CompareTo(b.Name);
			}
		}
	}
}
