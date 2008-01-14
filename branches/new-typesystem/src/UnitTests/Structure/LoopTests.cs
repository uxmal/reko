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

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class LoopTests
	{
		private ProcedureMock m;

		[SetUp]
		public void Setup()
		{
			m = new ProcedureMock();
		}

		[Test]
		public void LoopClassifyRepeat()
		{
			Block b = new Block(null, "foo");
			b.Statements.Add(new SideEffect(new Application(null, PrimitiveType.Word32)));
			b.Statements.Add(new Branch(Constant.Word32(0)));
			b.RpoNumber = 0;
			Block x = new Block(null, "exit");
			x.RpoNumber = 1;

			Block.AddEdge(b, b);
			Block.AddEdge(b, x);

			BitSet s = new BitSet(2);
			s[0] = true;
			RepeatLoop loop = new RepeatLoop(b, b, s);
			Assert.IsNotNull(loop);
			Assert.AreSame(x, loop.FollowBlock);
		}

		[Test]
		public void LoopClassifyRepeat1()
		{
			Block b = m.Label("foo");
			m.SideEffect(m.Fn("bar"));
			m.BranchIf(m.Fn("baz"), "foo");

			Block x = m.Label("exit");

			m.Procedure.RenumberBlocks();
			BitSet s = new BitSet(m.Procedure.RpoBlocks.Count);
			s[b.RpoNumber] = true;

			RepeatLoop loop = new RepeatLoop(b, b, s);
			Assert.IsNotNull(loop);
			Assert.AreSame(x, loop.FollowBlock);
		}

		[Test]
		public void LoopClassifyWhile()
		{
			Block h = new Block(null, "head");
			h.Statements.Add(new Branch(Constant.Word32(0)));
			h.RpoNumber = 0;

			Block b = new Block(null, "body");
			b.Statements.Add(new SideEffect(Constant.Word32(0)));
			b.RpoNumber = 1;

			Block x = new Block(null, "exit");
			Block.AddEdge(h, b);
			Block.AddEdge(h, x);
			Block.AddEdge(b, h);

			BitSet s = new BitSet(3);
			s[0] = true;
			s[1] = true;
			WhileLoop loop = new WhileLoop(h, b, s);
			Assert.IsNotNull(loop);
			Assert.AreSame(x, loop.FollowBlock);
		}
	}
}
