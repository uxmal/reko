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
using Reko.Core;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Analysis
{
    [TestFixture]
    public class SparseValuePropagationTests
    {
        private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder();
            builder(m);

            var ssa = m.Ssa;
            var program = new Program
            {
                SegmentMap = new SegmentMap(Address.Ptr32(0x00123400))
            };
            var svp = new SparseValuePropagation(ssa, program, null, new FakeDecompilerEventListener());
            svp.Transform();
            var sw = new StringWriter();
            svp.Write(sw);
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
        }

        [Test]
        public void Svp_Const()
        {
            var sExp =
            #region Expected
@"r1_1: 0x00000042
r2_2: 0x00000042
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_1 = 0x00000042
	r2_2 = r1_1
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1_1 = m.Reg32("r1_1");
                var r2_2 = m.Reg32("r2_2");
                m.Assign(r1_1, m.Word32(0x42));
                m.Assign(r2_2, r1_1);
                m.Return();
            });
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Svp_Phi()
        {
            var sExp =
            #region Expected
@"r1_1: 0x00000042
r2_2: 0x00000042
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_1 = 0x00000042
	r2_2 = r1_1
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1_1 = m.Reg32("r1_1");
                var r1_2 = m.Reg32("r1_2");

                m.Label("m1");
                m.Assign(r1_1, m.Word32(0x42));

                m.Label("m2");
                m.Phi(r1_2, (r1_1, "m1"), (r1_2, "m2"));
                m.Goto("m2");
            });
        }
    }
}
