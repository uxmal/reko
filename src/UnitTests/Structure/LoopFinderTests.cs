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
using Decompiler.Core.Code;
using Decompiler.Core.Lib;
using Decompiler.Core.Output;
using Decompiler.Structure;
using NUnit.Framework;
using Decompiler.UnitTests.Mocks;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class LoopFinderTests : StructureTestBase
	{
		[Test]
		public void LrwNoLoop()
		{
			RunTest("fragments/if.asm", "Structure/LrwNoLoop.txt");
		}

		[Test]
		public void LrwNestedRepeats()
		{
			RunTest("fragments/nested_repeats.asm", "Structure/LrwNestedRepeats.txt");
		}

		[Test]
		public void LrwBigHead()
		{
			RunTest("Fragments/while_bighead.asm", "Structure/LrwBigHead.txt");
		}

		[Test]
		public void LrwWhileRepeat()
		{
			RunTest("Fragments/while_repeat.asm", "Structure/LrwWhileRepeat.txt");
		}

		[Test]
		public void LfBackEdges()
		{
			Procedure proc = new MultipleContinueMock().Procedure;
			LoopFinder lf = new LoopFinder(proc, new DominatorGraph(proc));
			IntervalFinder infi = new IntervalFinder(proc);
			IEnumerator e = infi.Intervals.GetEnumerator();
			e.MoveNext();
			e.MoveNext();
			Interval i = (Interval) e.Current;
			BlockList preds = lf.BackEdges(i.Header);
			Assert.AreEqual(3, preds.Count);
		}

		private class MultipleContinueMock : ProcedureMock
		{
			/// <summary>
			/// 
			/// </summary>
			/// <remarks>
			/// <code>
			/// top:
			/// while (r1 != 0)
			/// {
			///		foo();
			///		if (r1 == 1)
			///			continue;
			///		bar();
			///		if (r1 == 2)
			///			continue;
			///		r1 += 4;
			///	}
			///	return r1;
			/// </code></remarks>
			protected override void BuildBody()
			{
				Identifier r1 = Local32("r1");

				Label("top");
				BranchIf(Eq(r1, Int32(0)), "done");

				Fn("foo");
				BranchIf(Eq(r1, Int32(1)), "top");

				Fn("bar");
				BranchIf(Eq(r1, Int32(2)), "top");

				Load(r1, Add(r1,Int32(4)));
				Jump("top");

				Label("done");
				Return(r1);
			}
		}


		[Test]
		public void LfWhileExit()
		{
			Procedure proc = new WhileExitMock().Procedure;
			LoopFinder lf = new LoopFinder(proc, new DominatorGraph(proc));
			IntervalFinder infi = new IntervalFinder(proc);
			IEnumerator e = infi.Intervals.GetEnumerator();
			e.MoveNext();
			e.MoveNext();
			Interval i = (Interval) e.Current;
			Assert.AreEqual("0111111", i.Blocks.ToString());
			Loop loop = lf.FindLoop(i);
			Assert.AreEqual("l1", proc.RpoBlocks[2].Name);
			DominatorGraph doms = new DominatorGraph(proc);
			TestDom("0011111", proc.RpoBlocks[1], doms);
			TestDom("0001100", proc.RpoBlocks[2], doms);
			TestDom("0000000", proc.RpoBlocks[3], doms);
			TestDom("0000000", proc.RpoBlocks[4], doms);
			TestDom("0000001", proc.RpoBlocks[5], doms);
			Assert.AreEqual("0011100", loop.Blocks.ToString());
		}

		[Test]
		public void LrwWhileExit()
		{
			RunTest(new WhileExitMock(), "Structure/LrwWhileExit.txt");
		}

		[Test]
		public void LfRepeatExit()
		{
			Procedure proc = new RepeatExitMock().Procedure;
			LoopFinder lf = new LoopFinder(proc, new DominatorGraph(proc));
			IntervalFinder infi = new IntervalFinder(proc);
			IEnumerator e = infi.Intervals.GetEnumerator();
			e.MoveNext();
			e.MoveNext();
			Interval i = (Interval) e.Current;
			Assert.AreEqual("011111", i.Blocks.ToString());
			Loop loop = lf.FindLoop(i);
			DominatorGraph doms = new DominatorGraph(proc);
			TestDom("001111", proc.RpoBlocks[1], doms);
			TestDom("000110", proc.RpoBlocks[2], doms);
			TestDom("000000", proc.RpoBlocks[3], doms);
			TestDom("000000", proc.RpoBlocks[4], doms);
			TestDom("000000", proc.RpoBlocks[5], doms);
			Assert.AreEqual("@", loop.Blocks.ToString());
		}

		[Test]
		public void LrwRepeatExit()
		{
			RunTest(new RepeatExitMock(), "Structure/LrwRepeatExit.txt");
		}


		private void TestDom(string expected, Block b, DominatorGraph doms)
		{
			Assert.AreEqual(expected, DominatorBitSet(b, b.Procedure, doms).ToString(), "Blocks dominated by " + b.Name);

		}
		private BitSet DominatorBitSet(Block b, Procedure proc, DominatorGraph dom)
		{
			BitSet blocks = proc.CreateBlocksBitset();
			foreach (Block d in dom.GetDominatedNodes(b))
			{
				blocks[d.RpoNumber] = true;
			}
			return blocks;
		}


		private class WhileExitMock : ProcedureMock
		{
			///<code>
			/// while (p != 0) {
			///		foo();
			///		if (p == 2) {
			///			bar();
			///			break;
			///		}
			///		foo();
			///		--p;
			///	}
			///	return p;
			///</code>
			///
			protected override void BuildBody()
			{
				Identifier p = Local32("p");
				Label("loop");
				BranchIf(Eq(p, 0), "done");

				SideEffect(Fn("foo"));
				BranchIf(Ne(p, 2), "not2");

				Label("exitBlock");
				SideEffect(Fn("bar"));
				Jump("done");

				Label("not2");
				SideEffect(Fn("foo"));
				Sub(p, p, 1);
				Jump("loop");

				Label("done");
				Return(p);
			}
		}

		private class RepeatExitMock : ProcedureMock
		{
			///<code>
			/// do {
			///     foo();
			///     if (p == 2) {
			///         bar();
			///         break;
			///     }
			///     foo();
			///     --p;
			/// } while (p != 0);
			/// return p;
			protected override void BuildBody()
			{
				Identifier p = Local32("p");
				
				Label("loop");
				SideEffect(Fn("foo"));
				BranchIf(Ne(p, 2), "not2");

				Label("exitBlock");
				SideEffect(Fn("bar"));
				Jump("done");

				Label("not2");
				SideEffect(Fn("foo"));
				Sub(p, p, 1);
				BranchIf(Eq(p, 0), "loop");

				Label("done");
				Return(p);
				
			}
		}


		protected void WriteBitset(string caption, BitSet s, System.IO.TextWriter text)
		{
			text.Write(caption);
			text.Write(" [ ");
			foreach (int i in s)
			{
				text.Write("{0} ", i);
			}
			text.WriteLine("]");
		}

		private void RunTest(ProcedureMock mock, string outFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				RunTestCore(mock.Procedure, fut);
				fut.AssertFilesEqual();
			}
		}

		private void RunTestCore(Procedure proc, FileUnitTester fut)
		{
			DominatorGraph dom = new DominatorGraph(proc);
			proc.Write(false, fut.TextWriter);
			fut.TextWriter.WriteLine("-----------");

			IntervalFinder intf = new IntervalFinder(proc);
			IntervalCollection ii = intf.Intervals;
			foreach (Interval i in ii)
			{
				LoopFinder lrw = new LoopFinder(proc, dom);
				Loop loop = lrw.FindLoop(i);
				if (loop != null)
				{
					fut.TextWriter.WriteLine(loop);
					lrw.BuildLoop(loop);
				}
			}
			fut.TextWriter.WriteLine();
			CodeFormatter fmt = new CodeFormatter(fut.TextWriter);
			fmt.Write(proc);
			fut.TextWriter.WriteLine("===========");
		}

		private void RunTest(string sourceFilename, string outFilename)
		{
			RewriteProgram(sourceFilename, new Address(0xC00, 0));
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				foreach (Procedure proc in prog.Procedures.Values)
				{
					RunTestCore(proc, fut);
				}
				fut.AssertFilesEqual();
			}
		}
	}
}
