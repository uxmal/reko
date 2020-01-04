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

using NUnit.Framework;
using Reko.Analysis;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class AdjacentBranchCollectorTests
    {
        private FakeArchitecture arch;

        [SetUp]
        public void Setup()
        {
            this.arch = new FakeArchitecture();
        }

        private void RunTest(string sExp, Action<ProcedureBuilder> builder)
        {
            var m = new ProcedureBuilder();
            builder(m);
            var abc = new AdjacentBranchCollector(m.Procedure, new FakeDecompilerEventListener());
            abc.Transform();
            var sw = new StringWriter();
            m.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sActual != sExp)
            {
                Debug.WriteLine($"** {nameof(AdjacentBranchCollectorTests)} failure");
                Debug.WriteLine(sActual);
                Assert.AreEqual(sExp, sActual);
            }
        }

        [Test]
        public void Abc_Nothing_to_do()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	// succ:  l1
l1:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                m.Return();
            });
        }

        [Test]
        public void Abc_CoalesceSuccessiveBlocks()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	// succ:  l1
l1:
	branch Test(ULT,C) m4skip
	// succ:  m1do m4skip
m1do:
	r1 = r2
	// succ:  m3do
m3do:
	r2 = 0x00000000
	// succ:  m4skip
m4skip:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var C = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("C"));
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);
                m.BranchIf(m.Test(ConditionCode.ULT, C), "m2skip");
                m.Label("m1do");
                m.Assign(r1, r2);

                m.Label("m2skip");
                m.BranchIf(m.Test(ConditionCode.ULT, C), "m4skip");

                m.Label("m3do");
                m.Assign(r2, 0);

                m.Label("m4skip");
                m.Return();
            });
        }

        [Test]
        public void Abc_Dont_Coalesce_Block_of_it_changes_flag()
        {
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	// succ:  m0
m0:
	C = cond(r1)
	branch Test(ULT,C) m2skip
	// succ:  m1do m2skip
m1do:
	r1 = r2
	CZ = cond(r1)
	// succ:  m2skip
m2skip:
	branch Test(ULT,C) m4skip
	// succ:  m3do m4skip
m3do:
	r2 = 0x00000000
	// succ:  m4skip
m4skip:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var C = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("C"));
                var CZ = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("CZ"));
                var r1 = m.Reg32("r1", 1);
                var r2 = m.Reg32("r2", 2);

                m.Label("m0");
                m.Assign(C, m.Cond(r1));
                m.BranchIf(m.Test(ConditionCode.ULT, C), "m2skip");

                m.Label("m1do");
                m.Assign(r1, r2);
                m.Assign(CZ, m.Cond(r1));

                m.Label("m2skip");
                m.BranchIf(m.Test(ConditionCode.ULT, C), "m4skip");

                m.Label("m3do");
                m.Assign(r2, 0);

                m.Label("m4skip");
                m.Return();
            });
        }

        [Test]
        public void Abc_Selfloop()
        {
            // The top "triangle" is a self loop. Avoid collecting it.
            var sExp =
            #region Expected
@"// ProcedureBuilder
// Return size: 0
define ProcedureBuilder
ProcedureBuilder_entry:
	// succ:  m1Header
m1Header:
	Mem0[0x00123400:byte] = 0x2A
	// succ:  m1Prev
m1Prev:
	branch Test(UGE,CZ) m1Prev
	// succ:  m2Block m1Prev
m2Block:
	branch Test(EQ,Z) m4Done
	// succ:  m3Leg m4Done
m3Leg:
	CZ = 0x00
	// succ:  m4Done
m4Done:
	return
	// succ:  ProcedureBuilder_exit
ProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var Z = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("Z"));
                var CZ = m.Frame.EnsureFlagGroup(arch.GetFlagGroup("CZ"));
                m.Label("m1Header");
                m.MStore(m.Word32(0x00123400), m.Byte(42));
                m.Label("m1Prev");
                m.BranchIf(m.Test(ConditionCode.UGE, CZ), "m1Prev");
                m.Label("m2Block");
                m.BranchIf(m.Test(ConditionCode.EQ, Z), "m4Done");
                m.Label("m3Leg");
                m.Assign(CZ, 0);
                m.Label("m4Done");
                m.Return();
            });
        }
    }
}
