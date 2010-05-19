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
using Decompiler.Structure;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Structure
{
	[TestFixture]
	public class CfgCleanerTests : StructureTestBase
	{
		[Test]
		public void CfgcIf()
		{
			RunTest("fragments/if.asm", "Structure/CfgcIf.txt");
		}

		[Test]
		public void CfgcNestedIf()
		{
			RunTest("Fragments/nested_ifs.asm", "Structure/CfgcNestedIf.txt");
		}

		[Test]
		public void CfgcForkedLoop()
		{
			RunTest("Fragments/forkedloop.asm", "Structure/CfgcForkedLoop.txt");
		}

		[System.Diagnostics.DebuggerHidden]
		private void RunTest(string sourceFile, string testFile)
		{
			using (FileUnitTester fut = new FileUnitTester(testFile))
			{
				this.RewriteProgram(sourceFile, new Address(0xC00, 0));
				prog.Procedures.Values[0].Write(false, fut.TextWriter);
				ControlFlowGraphCleaner cfgc = new ControlFlowGraphCleaner(prog.Procedures.Values[0]);
				cfgc.Transform();
				prog.Procedures.Values[0].Write(false, fut.TextWriter);

				fut.AssertFilesEqual();
			}
		}
	}
}
