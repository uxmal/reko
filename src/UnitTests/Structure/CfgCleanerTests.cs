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
using Reko.UnitTests.Mocks;
using Reko.Core.Types;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace Reko.UnitTests.Structure
{
	[TestFixture]
	public class CfgCleanerTests : StructureTestBase
	{
        [System.Diagnostics.DebuggerHidden]
        private void RunTest(string sourceFile, string testFile)
        {
            using (FileUnitTester fut = new FileUnitTester(testFile))
            {
                this.RewriteProgramMsdos(sourceFile, Address.SegPtr(0xC00, 0));
                program.Procedures.Values[0].Write(false, fut.TextWriter);
                fut.TextWriter.WriteLine();
                var cfgc = new ControlFlowGraphCleaner(program.Procedures.Values[0]);
                cfgc.Transform();
                program.Procedures.Values[0].Write(false, fut.TextWriter);

                fut.AssertFilesEqual();
            }
        }

        [Test]
		public void CfgcIf()
		{
			RunTest("Fragments/if.asm", "Structure/CfgcIf.txt");
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

        [Test]
        public void CfgcJmpToBranch()
        {
            var m = new ProcedureBuilder();
            var c = m.Temp(PrimitiveType.Bool, "c");
            var pfn = m.Temp(PrimitiveType.Ptr32, "pfn");
            m.Label("m1");
            m.BranchIf(c, "m3");
            m.Label("m2");
            m.Goto("m3");
            m.Label("m3");
            m.SideEffect(m.Fn(pfn));
            m.Return();

            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	pfn()
	return
	// succ:  ProcedureBuilder_exit
m1:
m3:
ProcedureBuilder_exit:
";
            #endregion
            var cfgc = new ControlFlowGraphCleaner(m.Procedure);
            cfgc.Transform();
            var sw = new StringWriter();
            m.Procedure.Write(false, sw);
            Assert.AreEqual(sExp, sw.ToString());
        }
	}
}
