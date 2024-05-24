#region License
/* 
 * Copyright (C) 1999-2024 Pavel Tomin.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class ComplexStackVariableTransformerTests
    {
        private SsaProcedureBuilder m;
        private Identifier fp;
        private Program program;
        private IntervalTree<int, DataType> intervals;

        [SetUp]
        public void Setup()
        {
            this.m = new SsaProcedureBuilder();
            var pb = new ProgramBuilder();
            pb.Add(m);
            this.program = pb.BuildProgram();
            this.fp = m.FramePointer();
            this.intervals = new();
        }

        private DataType Ptr32(DataType dt)
        {
            return new Pointer(dt, 32);
        }

        public void RunComplexStackVariableTransformer()
        {
            var eventListener = new FakeDecompilerEventListener();
            var context = new AnalysisContext(program, m.Procedure, null, null, eventListener);
            var programFlow = new ProgramDataFlow();
            var csvt = new ComplexStackVariableTransformer(context, programFlow);
            csvt.Transform(m.Ssa, intervals);
            m.Ssa.Validate(s => Assert.Fail(s));
        }

        private void Given_FrameIntervals(
            params (int offset, DataType dt)[] intervals)
        {
            foreach(var (offset, dt) in intervals)
            {
                this.intervals.Add(offset, offset + dt.Size, dt);
            }
        }

        private void AssertProcedureCode(string expected)
        {
            ProcedureCodeVerifier.AssertCode(m.Ssa.Procedure, expected);
        }

        [Test]
        public void Csvt_TwoApplicationCalls()
        {
            var ebp = m.Reg32("ebp");
            var ebp_5 = m.Reg32("ebp_5");
            var str = new StructureType("str", 8, true)
            {
                Fields =
                {
                    { 0, PrimitiveType.Int32 },
                    { 4, PrimitiveType.Real32 },
                },
            };
            var real32 = PrimitiveType.Real32;
            m.MStore(m.ISub(fp, 4), ebp);
            m.MStore(m.ISub(fp, 8), m.Word32(1));
            m.MStore(m.ISub(fp, 12), m.Word32(2));
            m.MStore(m.ISub(fp, 16), m.ISub(fp, 12));
            m.SideEffect(m.Fn("func1", m.Mem(Ptr32(str), m.ISub(fp, 16))));
            m.MStore(m.ISub(fp, 16), m.ISub(fp, 8));
            m.SideEffect(m.Fn("func2", m.Mem(Ptr32(real32), m.ISub(fp, 16))));
            m.Assign(ebp_5, m.Mem32(m.ISub(fp, 4)));

            Given_FrameIntervals((-12, str));

            RunComplexStackVariableTransformer();

            var expected = @"
def tLoc0C
Mem3[fp - 4<32>:word32] = ebp
Mem4[&tLoc0C + 4<i32>:word32] = 1<32>
Mem5[&tLoc0C:word32] = 2<32>
Mem6[fp - 0x10<32>:ptr32] = &tLoc0C
func1(Mem7[fp - 0x10<32>:(ptr32 (struct ""str"" 0008))])
Mem8[fp - 0x10<32>:ptr32] = &tLoc0C + 4<i32>
func2(Mem9[fp - 0x10<32>:(ptr32 real32)])
ebp_5 = Mem10[fp - 4<32>:word32]
";
            AssertProcedureCode(expected);
        }
    }
}
