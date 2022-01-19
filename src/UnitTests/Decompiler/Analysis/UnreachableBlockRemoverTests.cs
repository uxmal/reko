#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.UnitTests.Mocks;
using System;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;

namespace Reko.UnitTests.Decompiler.Analysis
{
    public class UnreachableBlockRemoverTests
    {
        private void RunTest(string sExpected, SsaProcedureBuilder pb)
        {
            var urb = new UnreachableBlockRemover(pb.Ssa, new FakeDecompilerEventListener());
            urb.Transform();
            var sw = new StringWriter();
            pb.Ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
            pb.Ssa.Validate(s => { pb.Ssa.Dump(true); Assert.Fail(s); });
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Urb_Remove_false()
        {
            var m = new SsaProcedureBuilder(name: nameof(Urb_Remove_false));

            var r1_1 = m.Reg32("r1_1");
            var r1_2 = m.Reg32("r1_2");
            var r1_3 = m.Reg32("r1_3");

            m.BranchIf(Constant.False(), "m2");

            m.Label("m1");
            m.Assign(r1_1, 1);
            m.Goto("m3");

            m.Label("m2");  // dead code
            m.Assign(r1_2, 2);

            m.Label("m3");
            m.Phi(r1_3, (r1_1, "m1"), (r1_2, "m2"));
            m.Return(r1_3);

            var sExp =
            #region
@"// Urb_Remove_false
// Return size: 0
define Urb_Remove_false
Urb_Remove_false_entry:
	// succ:  l1
l1:
	// succ:  m1
m1:
	r1_1 = 1<32>
	// succ:  m3
m3:
	r1_3 = r1_1
	return r1_3
	// succ:  Urb_Remove_false_exit
Urb_Remove_false_exit:
";
            #endregion

            RunTest(sExp, m);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Urb_Remove_true()
        {
            var m = new SsaProcedureBuilder(name: nameof(Urb_Remove_true));

            var r1_1 = m.Reg32("r1_1");
            var r1_2 = m.Reg32("r1_2");
            var r1_3 = m.Reg32("r1_3");

            m.BranchIf(Constant.Word16(0x42), "m2");

            m.Label("m1");
            m.Assign(r1_1, 1);
            m.Goto("m3");

            m.Label("m2");  // dead code
            m.Assign(r1_2, 2);

            m.Label("m3");
            m.Phi(r1_3, (r1_1, "m1"), (r1_2, "m2"));
            m.Return(r1_3);

            var sExp =
            #region
@"// Urb_Remove_true
// Return size: 0
define Urb_Remove_true
Urb_Remove_true_entry:
	// succ:  l1
l1:
	// succ:  m2
m2:
	r1_2 = 2<32>
	// succ:  m3
m3:
	r1_3 = r1_2
	return r1_3
	// succ:  Urb_Remove_true_exit
Urb_Remove_true_exit:
";
            #endregion

            RunTest(sExp, m);
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Urb_Remove_many_blocks()
        {
            var m = new SsaProcedureBuilder(name: nameof(Urb_Remove_many_blocks));

            var r1_1 = m.Reg32("r1_1");
            var r1_2 = m.Reg32("r1_2");
            var r1_3 = m.Reg32("r1_3");
            var r1_4 = m.Reg32("r1_4");
            var r2 = m.Reg32("r2");

            m.AddDefToEntryBlock(r2);
            m.BranchIf(Constant.False(), "m2");

            m.Label("m1");
            m.Assign(r1_1, 1);
            m.Goto("m4");

            m.Label("m2");  // dead code
            m.BranchIf(m.Eq0(r2), "m3");

            m.Label("m2a");
            m.Assign(r1_2, 2);
            m.Goto("m4");

            m.Label("m3");
            m.Assign(r1_3, 3);
            // end of dead code.
            
            m.Label("m4");
            m.Phi(r1_4, (r1_1, "m1"), (r1_2, "m2a"), (r1_3, "m3"));
            m.Return(r1_4);

            var sExp =
            #region
@"// Urb_Remove_many_blocks
// Return size: 0
define Urb_Remove_many_blocks
Urb_Remove_many_blocks_entry:
	def r2
	// succ:  l1
l1:
	// succ:  m1
m1:
	r1_1 = 1<32>
	// succ:  m4
m4:
	r1_4 = r1_1
	return r1_4
	// succ:  Urb_Remove_many_blocks_exit
Urb_Remove_many_blocks_exit:
";
            #endregion

            RunTest(sExp, m);
        }
    }
}
