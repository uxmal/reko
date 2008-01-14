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
using Decompiler.Core.Types;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;
using System.Collections;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class LoopMemberTests
	{
		[Test]
		public void LmWhileGoto()
		{
			Procedure proc = new MockWhileGoto().Procedure;

			IntervalFinder intf = new IntervalFinder(proc);
			IEnumerator e = intf.Intervals.GetEnumerator();
			e.MoveNext();
			e.MoveNext();
			Interval i = (Interval) e.Current;
			Assert.AreEqual("0011111111", i.Blocks.ToString());
			LoopFinder lf = new LoopFinder(proc, new DominatorGraph(proc));
			Block head = proc.RpoBlocks[2];
			Block end =  lf.FindLoopEnd(head, i.Blocks);
			Loop loop =  new WhileLoop(head, end, proc.CreateBlocksBitset());
			lf.FindBlocksInLoop(loop.HeaderBlock, loop.Blocks, i.Blocks);
			Assert.AreEqual("0011110000", loop.Blocks.ToString());
			lf.AbsorbExitingBranches(loop);
			Assert.AreEqual("0011111000", loop.Blocks.ToString());

		}
	}

	public class MockWhileGoto : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier ax = Local16("ax");
			Identifier si = Local16("si");
			Identifier di = Local16("di");
			Identifier zz = Local16("zz");

			Jump("looptest");

			Label("again");
			Store(di, ax);
			BranchIf(Ne(ax, 0), "ok");

			Assign(ax, -1);
			Jump("return");

			Label("ok");
			BranchIf(Ne(ax, 0x0D), "looptest");

			Add(zz, zz, 1);
			
			Label("looptest");
			Load(ax, si);
			Add(si, si, 2);
			BranchIf(Ne(ax, 0x20), "again");

			Assign(ax, si);
			
			Label("return");
			Store(new Constant(PrimitiveType.Word16, 0x300), ax);
			Return();
		}
	}
}
