#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
#endregion

using Reko.Core;
using Reko.Structure;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Structure
{
	[TestFixture]
	public class CompoundConditionCoalescerTests : StructureTestBase
	{
		private void RunTest(string sourceFilename, string outFilename)
		{
			using (FileUnitTester fut = new FileUnitTester(outFilename))
			{
				RewriteProgramMsdos(sourceFilename, Address.SegPtr(0xC00, 0));
				foreach (Procedure proc in program.Procedures.Values)
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

        [Test]
		public void CccTest1()
		{
			RunTest("Fragments/compound1.asm", "Structure/CccTest1.txt");
		}

		[Test]
		public void CccTest2()
		{
			RunTest("Fragments/compound_non.asm", "Structure/CccTest2.txt");
		}

		[Test]
		public void CccComplexTest()
		{
			RunTest("Fragments/compound_complex.asm", "Structure/CccComplexText.txt");
		}

		[Test]
        [Category(Categories.IntegrationTests)]
		public void CccAsciiHex()
		{
			RunTest("Fragments/ascii_hex.asm", "Structure/CccAsciiHex.txt");
		}
	}
}
