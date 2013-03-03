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

using Decompiler.Core;
using Decompiler.Analysis;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Analysis
{
	/// <summary>
	/// Tests that exercise the classification of induction variables.
	/// </summary>
	[TestFixture]
	public class InductionVariableTests : AnalysisTestBase
	{
		[Test]
		[Ignore("Not implemented yet")]
		public void IndSmallLoop()
		{
			RunTest("fragments/small_loop.asm", "Analysis/IndSmallLoop.txt");
		}

		protected override void RunTest(Program prog, FileUnitTester fut)
		{
			foreach (Procedure proc in prog.Procedures.Values)
			{
				DominatorGraph gr = new DominatorGraph(proc);
				Aliases alias = new Aliases(proc, prog.Architecture);
				alias.Transform();
				SsaTransform sst = new SsaTransform(proc, gr, true);
				SsaState ssa = sst.SsaState;

				DeadCode.Eliminate(proc, ssa);

				ValueNumbering vn = new ValueNumbering(ssa.Identifiers);
				ssa.Write(fut.TextWriter);
				proc.Write(false, fut.TextWriter);
				vn.Write(fut.TextWriter);
			}
		}	
	}

}
