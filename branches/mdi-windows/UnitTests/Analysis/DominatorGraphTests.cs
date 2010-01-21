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
using Decompiler.UnitTests.Mocks;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class DominatorGraphTests
	{
		[Test]
		public void DomDiamondTest()
		{
			Procedure proc = new DiamondMock().Procedure;
			DominatorGraph doms = new DominatorGraph(proc);
			Assert.IsTrue(doms.DominatesStrictly(1, 2));
			Assert.IsTrue(doms.DominatesStrictly(1, 3));
			Assert.IsTrue(doms.DominatesStrictly(1, 4));
		}

		[Test]
		public void DomDominatorCommon()
		{
			Procedure proc = new DiamondMock().Procedure;
			DominatorGraph doms = new DominatorGraph(proc);
			Block head = proc.RpoBlocks[1];
			Block f = proc.RpoBlocks[2];
			Block t = proc.RpoBlocks[3];
			Block join = proc.RpoBlocks[4];
			Assert.AreEqual("false", f.Name);
			Assert.AreEqual("true", t.Name);
			Assert.AreEqual("join", join.Name);
			Assert.IsNull(doms.CommonDominator(null), "Common denominator of no items is null");
			Assert.AreSame(head, doms.CommonDominator(new Block[] { head }), "Common dominator of single item is that item");
			Assert.AreSame(head, doms.CommonDominator(new Block[] { head, t }), "head dom true");
			Assert.AreSame(head, doms.CommonDominator(new Block[] { t, head }), "head dom true");
			Assert.AreSame(head, doms.CommonDominator(new Block[] { head, f}), "head dom true");
			Assert.AreSame(head, doms.CommonDominator(new Block[] { f, head }), "head dom true");
			Assert.AreSame(head, doms.CommonDominator(new Block[] { f, t }), "head dom true");
		}

		[Test]
		public void DomStmDominators()
		{
			Procedure proc = new DiamondMock().Procedure;
			DominatorGraph doms = new DominatorGraph(proc);
			Block head = proc.RpoBlocks[1];
			Block f = proc.RpoBlocks[2];
			Block t = proc.RpoBlocks[3];
			Block join = proc.RpoBlocks[4];

			Assert.IsTrue(doms.DominatesStrictly(join.Statements[0], join.Statements[1]), "First statement should dominate next statement"); 
			Assert.IsFalse(doms.DominatesStrictly(join.Statements[1], join.Statements[0]), "Second statement shouldn't  dominate prev statement"); 
			Assert.IsFalse(doms.DominatesStrictly(join.Statements[1], join.Statements[1]), "Statement doesn't dominate self");
			Assert.IsTrue(doms.DominatesStrictly(head.Statements[0], join.Statements[0]), "head dominates join!");
		}
	}

	public class DiamondMock : ProcedureMock
	{
		/// <code>
		/// if (x)
		///    y
		/// else 
		///    z;
		/// </code>
		protected override void BuildBody()
		{
			Identifier f = Local32("f");
			Identifier r = Local32("r");

			Label("head");
			BranchIf(f, "false");
			Label("true");
			Assign(r, 1);
			Jump("join");
			Label("false");
			Assign(r, 0);
			Label("join");
			Assign(f, Constant.True());
			Return(r);
		}

	}
}
