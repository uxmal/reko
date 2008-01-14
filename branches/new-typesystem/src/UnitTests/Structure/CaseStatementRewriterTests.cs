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
using Decompiler.Structure;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class CaseStatementRewriterTests : StructureTestBase
	{
		[Test]
		public void CsrwCreation()
		{
			Procedure proc = new MockSwitch().Procedure;
			DominatorGraph dom = new DominatorGraph(proc);
			using (FileUnitTester fut = new FileUnitTester("Structure/CsrwCreation.txt"))
			{
				proc.Write(false, fut.TextWriter);
				dom.Write(fut.TextWriter);
				CaseStatementRewriter csr = new CaseStatementRewriter(proc);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void CswrIsMultinode()
		{
			Procedure proc = new MockSwitch().Procedure;
			CaseStatementRewriter csr = new CaseStatementRewriter(proc);
			Assert.IsFalse(csr.IsMultibranch(proc.RpoBlocks[0]));
			Assert.IsTrue(csr.IsMultibranch(proc.RpoBlocks[3]));
		}

		[Test]
		[Ignore("Fails when switch is guarded; need some preprocessing")]
		public void CswrFindExitNode()
		{
			Procedure proc = new MockSwitch().Procedure;
			CaseStatementRewriter csr = new CaseStatementRewriter(proc);
			Block block = csr.FindExitNode(proc.RpoBlocks[3]);
			Assert.IsNotNull(block);
			Assert.AreEqual(-1, block.RpoNumber);
		}
	}
}
