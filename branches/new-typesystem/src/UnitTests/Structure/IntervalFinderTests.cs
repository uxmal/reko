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
	public class IntervalFinderTests : StructureTestBase
	{
		[Test]
		public void InfiIf()
		{
			RunTest("Fragments/if.asm", "Structure/InfiIf.txt");
		}

		[Test]
		public void InfiWhileBreak()
		{
			RunTest("Fragments/while_break.asm", "Structure/InfiWhileBreak.txt");
		}

		[Test]
		public void InfiWhileRepeat()
		{
			RunTest("Fragments/while_repeat.asm", "Structure/InfiWhileRepeat.txt");
		}

		[Test]
		public void InfiForkInLoop()
		{
			RunTest("Fragments/forkedloop.asm", "Structure/InfiForkInLoop.txt");
		}

		[Test]
		public void InfiNestedLoops()
		{
			RunTest("Fragments/matrix_addition.asm", "Structure/InfiNestedLoop.txt");
		}

		[Test]
		public void InfiNonreducible()
		{
			RunTest("Fragments/nonreducible.asm", "Structure/InfiNonreducible.txt");
		}

		private void RunTest(string sourceFile, string testFile)
		{
			using (FileUnitTester fut = new FileUnitTester(testFile))
			{
				this.RewriteProgram(sourceFile, new Address(0xC00, 0));
				foreach (Procedure proc in prog.Procedures.Values)
				{
					proc.Write(false, fut.TextWriter);
					IntervalFinder infi = new IntervalFinder(proc);
					IntervalCollection ii = infi.Intervals;
					foreach (Interval i in ii)
					{
						i.Write(fut.TextWriter);
						fut.TextWriter.WriteLine();
					}
					for (int i = 0; i < proc.RpoBlocks.Count; ++i)
					{
						fut.TextWriter.WriteLine("Interval of block {0}: {1}", i, infi.IntervalOf(proc.RpoBlocks[i]));
					}
					fut.TextWriter.WriteLine();
				}
				fut.AssertFilesEqual();
			}
		}
	}
}
