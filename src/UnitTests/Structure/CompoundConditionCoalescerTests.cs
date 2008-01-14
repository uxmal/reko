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
	public class CompoundConditionCoalescerTests : StructureTestBase
	{
		[Test]
		public void CccTest1()
		{
			RunTest("fragments/compound1.asm", "structure/CccTest1.txt");
		}

		[Test]
		public void CccTest2()
		{
			RunTest("fragments/compound_non.asm", "structure/CccTest2.txt");
		}

		[Test]
		public void CccComplexTest()
		{
			RunTest("fragments/compound_complex.asm", "structure/CccComplexText.txt");
		}

		[Test]
		public void CccAsciiHex()
		{
			RunTest("fragments/ascii_hex.asm", "Structure/CccAsciiHex.txt");
		}

		private void RunTest(string sourceFilename, string outFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				RewriteProgram(sourceFilename, new Address(0xC00, 0));
				foreach (Procedure proc in prog.Procedures.Values)
				{
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine();

					CompoundConditionCoalescer ccc = new CompoundConditionCoalescer(proc);
					ccc.Transform();
					proc.Write(false, fut.TextWriter);
					fut.TextWriter.WriteLine("================");
				}

				fut.AssertFilesEqual();
			}
		}
	}
}
