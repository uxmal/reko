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
using Reko.Core.Analysis;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.IO;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class LongComparisonFuserTests
    {
        private void RunTest(string sExpected, Action<SsaProcedureBuilder> builder)
        {
            var m = new SsaProcedureBuilder();
            builder(m);
            var context = new AnalysisContext(
                new Program(), m.Ssa.Procedure, null, null, new FakeDecompilerEventListener());
            var lcf = new LongComparisonFuser(context);
            lcf.Transform(m.Ssa);
            var sw = new StringWriter();
            sw.WriteLine();
            m.Ssa.Procedure.WriteBody(true, sw);
            var sActual = sw.ToString();
            if (sExpected != sActual)
            {
                Console.WriteLine(sActual);
                Assert.AreEqual(sExpected, sActual);
            }
            m.Ssa.Validate(s => { m.Ssa.Dump(true); Assert.Fail(s); });
        }

        [Test]
        public void Lcf_RegisterPair_le_Constant()
        {
            var sExpected =
            #region Expected
@"
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	ax_1 = SLICE(dx_ax, word16, 0)
	dx_2 = SLICE(dx_ax, word16, 16)
	branch SEQ(dx_2, ax_1) <= SEQ(0<16>, 0xA<16>) m5_fail
	// succ:  m4_succeed m5_fail
m4_succeed:
	return true
	// succ:  SsaProcedureBuilder_exit
m5_fail:
	return false
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExpected, m =>
            {
                var ax = new RegisterStorage("ax", 0, 0, PrimitiveType.Word16);
                var dx = new RegisterStorage("dx", 2, 0, PrimitiveType.Word16);
                var dx_ax = m.SeqId("dx_ax", PrimitiveType.Word32, dx, ax);
                var ax_1 = m.Reg("ax_1", ax);
                var dx_2 = m.Reg("dx_2", dx);
                m.Assign(ax_1, m.Slice(dx_ax, ax.DataType, 0));
                m.Assign(dx_2, m.Slice(dx_ax, dx.DataType, 16));
                m.BranchIf(m.Lt0(dx_2), "m5_fail");

                m.BranchIf(m.Ne0(dx_2), "m4_succeed");

                m.BranchIf(m.Le(ax_1, 0xa), "m5_fail");

                m.Label("m4_succeed");
                m.Return(m.True());
                m.Label("m5_fail");
                m.Return(m.False());
            });
        }

        [Test]
        public void Lcf_RegisterPair_Ge()
        {
            var sExp =
            #region
            @"
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	ax_1 = SLICE(dx_ax, word16, 0)
	dx_2 = SLICE(dx_ax, word16, 16)
	branch SEQ(dx_2, ax_1) >= SEQ(0<16>, 0x200<16>) m5_fail
	// succ:  m4_succeed m5_fail
m4_succeed:
	return true
	// succ:  SsaProcedureBuilder_exit
m5_fail:
	return false
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var ax = new RegisterStorage("ax", 0, 0, PrimitiveType.Word16);
                var dx = new RegisterStorage("dx", 2, 0, PrimitiveType.Word16);
                var dx_ax = m.SeqId("dx_ax", PrimitiveType.Word32, dx, ax);
                var ax_1 = m.Reg("ax_1", ax);
                var dx_2 = m.Reg("dx_2", dx);
                m.Assign(ax_1, m.Slice(dx_ax, ax.DataType, 0));
                m.Assign(dx_2, m.Slice(dx_ax, dx.DataType, 16));
                m.BranchIf(m.Gt0(dx_2), "m5_fail");

                m.BranchIf(m.Lt0(dx_2), "m4_succeed");

                m.BranchIf(m.Uge(ax_1, 0x200), "m5_fail");

                m.Label("m4_succeed");
                m.Return(m.True());
                m.Label("m5_fail");
                m.Return(m.False());
            });

        }

        [Test]
        public void Lcf_RegisterPair_UGt()
        {
            var sExp =
            #region
            @"
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	ax_1 = SLICE(dx_ax, word16, 0)
	dx_2 = SLICE(dx_ax, word16, 16)
	branch SEQ(dx_2, ax_1) >=u SEQ(0<16>, 0x200<16>) m5_fail
	// succ:  m4_succeed m5_fail
m4_succeed:
	return true
	// succ:  SsaProcedureBuilder_exit
m5_fail:
	return false
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var ax = new RegisterStorage("ax", 0, 0, PrimitiveType.Word16);
                var dx = new RegisterStorage("dx", 2, 0, PrimitiveType.Word16);
                var dx_ax = m.SeqId("dx_ax", PrimitiveType.Word32, dx, ax);
                var ax_1 = m.Reg("ax_1", ax);
                var dx_2 = m.Reg("dx_2", dx);
                m.Assign(ax_1, m.Slice(dx_ax, ax.DataType, 0));
                m.Assign(dx_2, m.Slice(dx_ax, dx.DataType, 16));
                m.BranchIf(m.Ugt0(dx_2), "m5_fail");

                m.BranchIf(m.Ult0(dx_2), "m4_succeed");

                m.BranchIf(m.Uge(ax_1, 0x200), "m5_fail");

                m.Label("m4_succeed");
                m.Return(m.True());
                m.Label("m5_fail");
                m.Return(m.False());
            });
        }

        [Test]
        public void Lcf_RegisterPair_ULe()
        {
            var sExp =
            #region
            @"
SsaProcedureBuilder_entry:
	// succ:  l1
l1:
	ax_1 = SLICE(dx_ax, word16, 0)
	dx_2 = SLICE(dx_ax, word16, 16)
	branch SEQ(dx_2, ax_1) <=u SEQ(0<16>, 0x200<16>) m5_fail
	// succ:  m4_succeed m5_fail
m4_succeed:
	return true
	// succ:  SsaProcedureBuilder_exit
m5_fail:
	return false
	// succ:  SsaProcedureBuilder_exit
SsaProcedureBuilder_exit:
";
            #endregion

            RunTest(sExp, m =>
            {
                var ax = new RegisterStorage("ax", 0, 0, PrimitiveType.Word16);
                var dx = new RegisterStorage("dx", 2, 0, PrimitiveType.Word16);
                var dx_ax = m.SeqId("dx_ax", PrimitiveType.Word32, dx, ax);
                var ax_1 = m.Reg("ax_1", ax);
                var dx_2 = m.Reg("dx_2", dx);
                m.Assign(ax_1, m.Slice(dx_ax, ax.DataType, 0));
                m.Assign(dx_2, m.Slice(dx_ax, dx.DataType, 16));
                m.BranchIf(m.Ult0(dx_2), "m5_fail");

                m.BranchIf(m.Ugt0(dx_2), "m4_succeed");

                m.BranchIf(m.Ule(ax_1, 0x200), "m5_fail");

                m.Label("m4_succeed");
                m.Return(m.True());
                m.Label("m5_fail");
                m.Return(m.False());
            });
        }
    }
}
