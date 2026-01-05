#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using Reko.Core.Memory;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.IO;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class SparseValuePropagationTests
    {
        private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder();
            builder(m);

            var ssa = m.Ssa;
            var segmentMap = new SegmentMap(Address.Ptr32(0x00123400));
            var program = new Program
            {
                SegmentMap = segmentMap,
                Memory = new ByteProgramMemory(segmentMap)
            };
            var svp = new SparseValuePropagation(ssa, program, new FakeDecompilerEventListener());
            svp.Transform();
            var sw = new StringWriter();
            sw.WriteLine();
            svp.Write(sw);
            ssa.Procedure.Write(false, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
            ssa.Validate(s => Assert.Fail(s));
        }

        [Test]
        public void Svp_Const()
        {
            var sExp =
            #region Expected
@"
r1_1: 0x42<32>
r2_2: 0x42<32>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	r1_1 = 0x42<32>
	r2_2 = 0x42<32>
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
        public void Svp_Phi()
        {
            var sExp =
            #region Expected
@"
r1_1: 0x42<32>
r2_2: 0x42<32>
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	// succ:  m1
m1:
	r1_1 = 0x42<32>
	// succ:  m2
m2:
	r2_2 = 0x42<32>
	goto m2
	// succ:  m2
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var r1_1 = m.Reg32("r1_1");
                var r2_2 = m.Reg32("r2_2");

                m.Label("m1");
                m.Assign(r1_1, m.Word32(0x42));

                m.Label("m2");
                m.Phi(r2_2, (r1_1, "m1"), (r2_2, "m2"));
                m.Goto("m2");
            });
        }

        [Test]
        public void Svp_Slices()
        {
            var sExp =
            #region Expected
@"
d3_1: SEQ(v4, 2<16>)
d3_2: r1
Mem9: <invalid>
r1: <invalid>
sp_04: v1
v1: SLICE(r1, word16, 0)
v2: v1
v3: SLICE(r1, word16, 16)
v4: SLICE(r1, word16, 16)
v5: SLICE(r1, word16, 0)
// SsaProcedureBuilder
// Return size: 0
define SsaProcedureBuilder
SsaProcedureBuilder_entry:
	def r1
	// succ:  m1
m1:
	v1 = SLICE(r1, word16, 0)
	sp_04 = v1
	v4 = SLICE(r1, word16, 16)
	d3_1 = SEQ(v4, 2<16>)
	v5 = SLICE(r1, word16, 0)
	Mem9[0x123400<32>:word16] = v5
	v2 = v1
	v3 = SLICE(r1, word16, 16)
	d3_2 = r1
	return
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
	use r1
";
            #endregion

            RunTest(sExp, m =>
            {
                var d3 = m.Reg32("r1");
                var d3_1 = m.Reg32("d3_1");
                var d3_2 = m.Reg32("d3_2");
                var sp_04_3 = m.Local16("sp_04");
                var v1 = m.Temp(PrimitiveType.Word16, "v1");
                var v2 = m.Temp(PrimitiveType.Word16, "v2");
                var v3 = m.Temp(PrimitiveType.Word16, "v3");
                var v4 = m.Temp(PrimitiveType.Word16, "v4");
                var v5 = m.Temp(PrimitiveType.Word16, "v5");

                m.AddDefToEntryBlock(d3);

                m.Label("m1");
                m.Assign(v1, m.Slice(d3, PrimitiveType.Word16));
                m.Assign(sp_04_3, v1);

                m.Assign(v4, m.Slice(d3, PrimitiveType.Word16, 16));
                m.Assign(d3_1, m.Seq(v4, m.Word16(0x02)));
                m.Assign(v5, m.Slice(d3, PrimitiveType.Word16));
                m.MStore(m.Word32(0x00123400), v5);

                m.Assign(v2, sp_04_3);
                m.Assign(v3, m.Slice(d3, PrimitiveType.Word16, 16));
                m.Assign(d3_2, m.Seq(v3, v2));
                m.Return();

                m.AddUseToExitBlock(d3);
            });
        }

    }
}
