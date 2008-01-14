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

using Decompiler;
using Decompiler.Analysis;
using Decompiler.Core;
using Decompiler.Scanning;
using Decompiler.Arch.Intel;
using NUnit.Framework;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Decompiler.UnitTests.Analysis
{
	[TestFixture]
	public class AnalysisTests : AnalysisTestBase
	{
		[Test]
		public void DiamondDominatorTest()
		{
			Program prog = RewriteFile("Fragments/diamond.asm");
			Procedure proc = prog.Procedures[0];
			DominatorGraph doms = new DominatorGraph(proc);
			BlockList bl = proc.RpoBlocks;
			Assert.IsTrue(doms.ImmediateDominator(bl[2]) == bl[1]);
			Assert.IsTrue(doms.ImmediateDominator(bl[3]) != bl[2]);
			Assert.IsTrue(doms.ImmediateDominator(bl[3]) == bl[1]);
		}

		[Test]
		public void LoopDominatorTest()
		{
			Program prog = RewriteFile("Fragments/while_loop.asm");
			DominatorGraph doms = new DominatorGraph(prog.Procedures[0]);
			Assert.IsTrue(doms.DominatesStrictly(0, 1));
			Assert.IsTrue(doms.DominatesStrictly(0, 2));
			Assert.IsTrue(doms.DominatesStrictly(1, 2));
			Assert.IsTrue(doms.DominatesStrictly(3, 4));
		}


		[Test]
		public void AnAliasExpanderTest()
		{
			Program prog = RewriteFile("Fragments/alias_regs.asm");
			Procedure proc = prog.Procedures[0];
			Aliases alias = new Aliases(proc, prog.Architecture);
			alias.Transform();
			using (FileUnitTester fut = new FileUnitTester("Analysis/AnAliasExpanderTest.txt"))
			{
				proc.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void AliasExpandDeadVars()
		{
			Program prog = RewriteFile("Fragments/alias_regs2.asm");
			Procedure proc = prog.Procedures[0];
			Aliases alias = new Aliases(proc, prog.Architecture);
			alias.Transform();

			using (FileUnitTester fut = new FileUnitTester("Analysis/AnAliasExpandDeadVars.txt"))
			{
				proc.Write(true, fut.TextWriter);
				fut.AssertFilesEqual();
			}
		}
	}
}
