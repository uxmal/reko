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

using Decompiler.Core;
using Decompiler.Core.Absyn;
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Operators;
using Decompiler.Core.Output;
using Decompiler.Core.Types;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;


namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class LinearizerTests
	{
		[Test]
		public void LinIfThen()
		{
			Procedure proc = new MockIfThen().Procedure;
			IntervalFinder intf = new IntervalFinder(proc);
			IntervalCollection ints = intf.Intervals;
			Assert.AreEqual(1, ints.Count);

			foreach (Interval i in ints)
			{
				Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
				lin.BuildIfStatements(i.Blocks);
			}

			using (FileUnitTester fut = new FileUnitTester("Structure/LinIfThen.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void LinIfThenBranchJoin()
		{
			Procedure proc = new MockIfThen().Procedure;
			BitSet blockSet = proc.CreateBlocksBitset();
			blockSet.SetAll(true);

			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			Block b = proc.RpoBlocks[1];			// The block containing the if branch.
			Assert.AreEqual(2, b.Succ.Count);
			Block j = lin.FindBranchJoin(
				new StraightPathWalker(b.ThenBlock),
				new StraightPathWalker(b.ElseBlock), 
				blockSet,
				null);
			Assert.IsNotNull(j);
			Assert.AreEqual(3, j.RpoNumber);
		}

		/// <summary>
		/// Tests the linearization of a branch.
		/// </summary>
		[Test]
		public void LinIfThenBuildPath()
		{
			Procedure proc = new MockIfThen().Procedure;
			BitSet blockSet = proc.CreateBlocksBitset();
			blockSet.SetAll(true);

			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			Assert.AreEqual(1, proc.RpoBlocks[2].Succ.Count);
			Block.RemoveEdge(proc.RpoBlocks[1], proc.RpoBlocks[2]);
			AbsynStatementList stms = lin.LinearizeStraightPath(proc.RpoBlocks[2], proc.RpoBlocks[3]);
			Assert.AreEqual(1, stms.Count);			// There is only one statement in the block.
		}

		[Test]
		public void LinSnarlBranchJoin()
		{
			Procedure proc = new MockSnarl().Procedure;
			BitSet blockSet = proc.CreateBlocksBitset();
			blockSet.SetAll(true);

			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			Block b = proc.RpoBlocks[4];
			Assert.AreEqual(2, b.Succ.Count);
			StraightPathWalker t = new StraightPathWalker(b.ThenBlock);
			StraightPathWalker e = new StraightPathWalker(b.ElseBlock);
			Block j = lin.FindBranchJoin(t, e, blockSet, null);
			Assert.IsNotNull(j);
			Assert.AreEqual(6, j.RpoNumber);

			// The following should fail because one of the paths 
			// contains a branch.
			b = proc.RpoBlocks[2];
			Assert.AreEqual(2, b.Succ.Count);
			t = new StraightPathWalker(b.ThenBlock);
			e = new StraightPathWalker(b.ElseBlock);
			j = lin.FindBranchJoin(t, e, blockSet, null);
			Assert.IsNull(j);
		}


		[Test]
		public void LinWhileBreak()
		{
			Procedure proc = new MockWhileBreak().Procedure;
			using (FileUnitTester fut = new FileUnitTester("Structure/LinWhileBreak.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}

			proc.Write(false, Console.Out);

			Linearizer lin = new Linearizer(proc, new BlockLinearizer(proc.RpoBlocks[4]));
			lin.LoopHeader = proc.RpoBlocks[2];
			lin.LoopFollow = proc.RpoBlocks[4];
			lin.ProcedureExit = proc.RpoBlocks[4];
			BitSet blocks = proc.CreateBlocksBitset();
			blocks[3] = true;
			blocks[6] = true;

			Block b = proc.RpoBlocks[3];		// The one with the break in it.
			StraightPathWalker t = new StraightPathWalker(b.ThenBlock);
			StraightPathWalker e = new StraightPathWalker(b.ElseBlock);
			Block j = lin.FindBranchJoin(t, e, blocks, proc.RpoBlocks[4]);
			Assert.IsNull(j, "Branches shoudn't converge, we have to use a break here!");
			Assert.AreEqual("done", t.Current.Name);
			Assert.AreEqual("looptest", e.Current.Name);

			Block p = lin.PreferredUnstructuredExit(t.Current, e.Current);
			Assert.AreEqual("done", p.Name);

			AbsynStatementList stms = lin.Linearize(blocks, true);
			Assert.AreEqual(6, stms.Count);
			AbsynIf ifStm = stms[4] as AbsynIf;
			Assert.IsNotNull(ifStm);
			proc.Write(false, Console.Out);
			Assert.IsTrue(ifStm.Then is AbsynBreak);
		}

		[Test]
		public void LinSnarlGotoIfs()
		{
			Procedure proc = new MockSnarl().Procedure;
			IntervalFinder intf = new IntervalFinder(proc);
			proc.Dump(true, false);
			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));

			foreach (Interval i in intf.Intervals)
			{
				lin.BuildIfStatements(i.Blocks);
			}
			using (FileUnitTester fut = new FileUnitTester("Structure/LinSnarlGotoIfs.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void LinSnarlGotos()
		{
			Procedure proc = new MockSnarl().Procedure;
			proc.Dump(true, false);
			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			BitSet blocks = proc.CreateBlocksBitset();
			blocks.SetAll(true);

			lin.BuildIfStatements(blocks);
			using (FileUnitTester fut = new FileUnitTester("Structure/LinSnarlGotos.txt"))
			{
				proc.Write(false, fut.TextWriter);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}


		[Test]
		public void LinSnarl()
		{
			Procedure proc = new MockSnarl().Procedure;
			proc.Dump(true, false);
			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			BitSet blocks = proc.CreateBlocksBitset();
			blocks.SetAll(true);

			AbsynStatementList stms = lin.Linearize(blocks, true);
			using (FileUnitTester fut = new FileUnitTester("Structure/LinSnarl.txt"))
			{
				CodeFormatter cf = new CodeFormatter(fut.TextWriter);
				cf.WriteStatementList(stms);
				fut.TextWriter.WriteLine();
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void LinWhileFork()
		{
			Procedure proc = new MockWhileFork().Procedure;
			IntervalFinder intf = new IntervalFinder(proc);
				
			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			BitSet blocks = proc.CreateBlocksBitset();

			// Simulate the work done by a loop finder.
			blocks[5] = true;
			blocks[6] = true;
			blocks[7] = true;
			blocks[8] = true;
			Block.RemoveEdge(proc.RpoBlocks[2], proc.RpoBlocks[5]);

			AbsynStatementList stms = lin.Linearize(blocks, true);
			using (FileUnitTester fut = new FileUnitTester("Structure/LinWhileFork.txt"))
			{
				CodeFormatter cf = new CodeFormatter(fut.TextWriter);
				cf.WriteStatementList(stms);
				fut.TextWriter.WriteLine();
			}
		}

		[Test]
		public void LinStmlist()
		{
			Procedure proc = new MockIfThen().Procedure;

			Linearizer lin = new Linearizer(proc, new BlockLinearizer(null));
			BitSet region = proc.CreateBlocksBitset();
			region[2] = true;
			AbsynStatementList stms = lin.BuildStatementList(region, false);
			Assert.AreEqual(1, stms.Count);
			AbsynAssignment ass = (AbsynAssignment) stms[0];
			BinaryExpression b = (BinaryExpression) ass.Src;
			Assert.AreEqual(Operator.add, b.op);
		}

		[Test]
		public void LinShouldSwap1()
		{
			Identifier r = new Identifier("r", 1, PrimitiveType.Word32, null);
			Identifier x = new Identifier("x", 1, PrimitiveType.Word32, null);
			Linearizer lin = new Linearizer(null, new BlockLinearizer(null));
			Expression cond = MakeNeCompare(r, 0);
			AbsynStatement stmThen = new AbsynIf(MakeNeCompare(r, 1),
				new AbsynAssignment(x, Constant.Word32(-1)),
				new AbsynAssignment(x, Constant.Word32(1)));
			AbsynStatementList stmsThen = new AbsynStatementList();
			stmsThen.Add(stmThen);

			AbsynStatement stmElse = new AbsynAssignment(x, Constant.Word32(0));
			AbsynStatementList stmsElse = new AbsynStatementList();
			stmsElse.Add(stmElse);

			Assert.IsTrue(lin.ShouldSwap(cond, stmsThen, stmsElse));
		}

		private Expression MakeNeCompare(Identifier id, int val)
		{
			return new BinaryExpression(Operator.ne,  PrimitiveType.Bool, id, Constant.Word32(0));
		}

		public class MockWhileFork : ProcedureMock
		{
			/// <remarks>
			/// <code>
			/// r3 = 0;
			/// while (r1 != 0)
			///		r2 = *r1;
			///		if (r2 >= 0)
			///			++r3;
			///		else
			///			--r3;
			///		r1 = r1->ptr0004;
			///	}
			///	return r3; 
			/// </code>
			/// </remarks>
			protected override void BuildBody()
			{
				Identifier r1 = Local32("r1");
				Identifier r2 = Local32("r2");
				Identifier r3 = Local32("r3");

				Assign(r3,Int32(0));

				Label("top");
				BranchIf(Eq(r1,Int32(0)), "done");

				Load(r2,r1);
				BranchIf(Lt(r2,Int32(0)), "negative");

				Add(r3,r3,Int32(1));
				Jump("next");

				Label("negative");
				Sub(r3, r3, Int32(1));

				Label("next");
				Load(r1, Add(r1, Int32(4)));
				Jump("top");

				Label("done");
				Return(r3);
			}
		}
	}

	public class MockIfThen : ProcedureMock
	{
		/// <remarks>
		/// <code>
		/// if (a)
		///    ++a;
		/// return a;
		/// </code>
		/// </remarks>
		protected override void BuildBody()
		{
			Identifier a = Local32("r0");
			BranchIf(Not(a), "skip");
			Add(a, a, Int32(1));
			Label("skip");
			Return(a);
		}
	}
}
