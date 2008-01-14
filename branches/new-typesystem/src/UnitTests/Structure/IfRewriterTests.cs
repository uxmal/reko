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
using Decompiler.Core.Output;
using Decompiler.Structure;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	[Ignore("IfRewriter is dead and should go away")]
	public class IfRewriterTests : StructureTestBase
	{
		[Test]
		public void IfrIf()
		{
			RunTest("Fragments/if.asm", "Structure/IfrIf.txt");
		}

		[Test]
		public void IfrNestedIf()
		{
			RunTest("Fragments/nested_ifs.asm", "Structure/IfrNestedIf.txt");
		}

		private void RunTest(string sourceFilename, string outFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				RewriteProgram(sourceFilename, new Address(0xC00, 0000));
				foreach (Procedure proc in prog.Procedures.Values)
				{
					ControlFlowGraphCleaner cleaner = new ControlFlowGraphCleaner(proc);
					cleaner.Transform();
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine("-----------");

					IntervalFinder intf = new IntervalFinder(proc);
					IntervalCollection ii = intf.Intervals;
					DominatorGraph dom = new DominatorGraph(proc);
					foreach (Interval i in ii)
					{
						IfRewriter ifr = new IfRewriter(proc, dom, i.Blocks, proc.RpoBlocks[0], null, null);
						ifr.Transform();
						fut.TextWriter.WriteLine("{0}()", proc.Name);
						CodeFormatter fmt = new CodeFormatter(fut.TextWriter);
						ifr.LinearizedStatement.Accept(fmt);
					}
					fut.TextWriter.WriteLine("===========");

					fut.AssertFilesEqual();
				}

			}
		}
	}
}
