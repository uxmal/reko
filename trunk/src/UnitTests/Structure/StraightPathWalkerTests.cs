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
using Decompiler.Core.Lib;
using Decompiler.Structure;
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class StraightPathWalkerTests
	{
		[Test]
		public void SpwCreate()
		{
			StraightPathWalker spw = new StraightPathWalker(null);
		}

		[Test]
		public void SpwIfThenCurrent()
		{
			Procedure proc = new MockIfThen().Procedure;
			StraightPathWalker spw = new StraightPathWalker(proc.EntryBlock);
			Assert.AreSame(proc.EntryBlock, spw.Current);
			Assert.IsTrue(spw.Advance());
			Assert.AreEqual(proc.RpoBlocks[1], spw.Current);
			Assert.AreEqual(1, spw.PathLength);

			Assert.IsFalse(spw.Advance());
		}

		[Test]
		public void IsBlockedBecauseBlockIsOutside()
		{
			BitSet loopBody = new BitSet(10);
			loopBody[1] = true;
			loopBody[2] = true;
			Block one = new Block(null, "one");
			Block two = new Block(null, "two");
			Block outside = new Block(null, "outside");
			Block.AddEdge(one, two);
			Block.AddEdge(two, outside);
			one.RpoNumber = 1;
			two.RpoNumber = 2;
			outside.RpoNumber = 3;

			StraightPathWalker spw = new StraightPathWalker(one);
			Assert.IsFalse(spw.IsBlocked(loopBody));
			spw.Advance();
			Assert.AreSame(two, spw.Current);
			Assert.IsFalse(spw.IsBlocked(loopBody));
			spw.Advance();
			Assert.AreEqual(outside, spw.Current);
			Assert.IsTrue(spw.IsBlocked(loopBody));
		}

		[Test]
		public void IsBlockedBecauseBlockIsBackEdgeToRepeat()
		{
			BitSet loopBody = new BitSet(10);
			loopBody[1] = true;
			loopBody[2] = true;
			loopBody[3] = true;
			Block head = new Block(null, "head"); head.RpoNumber = 1;
			Block body = new Block(null, "body"); body.RpoNumber = 2;
			Block latch = new Block(null, "latch"); latch.RpoNumber = 3;
			Block.AddEdge(head, body);		// re
			Block.AddEdge(body, latch);
			Block.AddEdge(latch, head);

			StraightPathWalker spw = new StraightPathWalker(head);
			spw.Advance();
			spw.Advance();
			Assert.IsFalse(spw.IsBlocked(loopBody));
			spw.Advance();
			Assert.AreSame(head, spw.Current);
			Assert.IsTrue(spw.IsBlocked(loopBody));
		}

	}
}
