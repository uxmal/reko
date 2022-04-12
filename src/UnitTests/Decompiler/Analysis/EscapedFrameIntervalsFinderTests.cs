#region License
/* 
 * Copyright (C) 1999-2022 Pavel Tomin.
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
using Reko.Core.Collections;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class EscapedFrameIntervalsFinderTests
    {
        private SsaProcedureBuilder m;
        private ProgramBuilder pb;
        private Identifier fp;
        private Program program;
        private ProgramDataFlow flow;
        private IntervalTree<int, DataType> intervals;
        private StructureType str;

        [SetUp]
        public void Setup()
        {
            this.m = new SsaProcedureBuilder();
            this.pb = new ProgramBuilder();
            pb.Add(m);
            this.program = pb.BuildProgram();
            this.flow = new ProgramDataFlow(program);
            this.fp = m.FramePointer();
            this.str = new StructureType("str", 8, true)
            {
                Fields =
                {
                    { 0, PrimitiveType.Int32 },
                    { 4, PrimitiveType.Real32 },
                },
            };
        }

        private static DataType Ptr32(DataType dt)
        {
            return new Pointer(dt, 32);
        }

        private void RunEscapedFrameIntervalsFinder()
        {
            var eventListener = new FakeDecompilerEventListener();
            var efif = new EscapedFrameIntervalsFinder(
                program, flow, m.Ssa, eventListener);
            this.intervals = efif.Find();
        }

        private void AssertIntervals(string expected)
        {
            var actual = string.Join(Environment.NewLine, intervals);
            actual = Environment.NewLine + actual;
            if (actual != expected)
            {
                Console.WriteLine(actual);
                Assert.AreEqual(expected, actual);
            }
        }

        [Test]
        public void Efif_TwoApplicationCalls()
        {
            var ebp = m.Reg32("ebp");
            var ebp_5 = m.Reg32("ebp_5");
            var real32 = PrimitiveType.Real32;
            m.MStore(m.ISub(fp, 4), ebp);
            m.MStore(m.ISub(fp, 8), m.Word32(1));
            m.MStore(m.ISub(fp, 12), m.Word32(2));
            m.MStore(m.ISub(fp, 16), m.ISub(fp, 12));
            m.SideEffect(m.Fn("func1", m.Mem(Ptr32(str), m.ISub(fp, 16))));
            m.MStore(m.ISub(fp, 16), m.ISub(fp, 8));
            m.SideEffect(m.Fn("func2", m.Mem(Ptr32(real32), m.ISub(fp, 16))));
            m.Assign(ebp_5, m.Mem32(m.ISub(fp, 4)));

            RunEscapedFrameIntervalsFinder();

            var expected = @"
[[-12, -4], (struct ""str"" 0008 (0 int32 dw0000) (4 real32 r0004))]";
            AssertIntervals(expected);
        }

        [Test]
        public void Efif_Use_ProcedureFlow()
        {
            var ecx = m.Reg32("ecx");
            var eax_1 = m.Reg32("eax");

            // fn4242 is the procedure we are calling.
            var proc = new Procedure(m.Architecture, "fn4242", Address.Ptr32(0x4242), m.Architecture.CreateFrame());
            var procFlow = new ProcedureFlow(proc);
            procFlow.BitsUsed.Add(ecx.Storage, ecx.Storage.GetBitRange());
            procFlow.LiveInDataTypes.Add(ecx.Storage, new Pointer(str, 32));
            pb.Add(proc);
            flow.ProcedureFlows.Add(proc, procFlow);
            
            m.MStore(m.ISub(fp, 16), m.Word32(0x1234));
            m.MStore(m.ISub(fp, 12), Constant.Real32(12.34F));
            m.Call(proc, 0,
                new (Storage, Expression)[] { (ecx.Storage, m.ISubS(fp, 16)) },
                new (Storage, Identifier)[] { (eax_1.Storage, eax_1)});

            RunEscapedFrameIntervalsFinder();

            var expected = @"
[[-16, -8], (struct ""str"" 0008 (0 int32 dw0000) (4 real32 r0004))]";
            AssertIntervals(expected);
        }

        [Test]
        public void Efif_Use_ProcedureFlow_With_TypeVariables()
        {
            var ecx = m.Reg32("ecx");
            var eax_1 = m.Reg32("eax");

            var tv1 = new TypeVariable(1) { DataType = PrimitiveType.Int32 };
            var tv2 = new TypeVariable(2) { DataType = PrimitiveType.Real32 };
            var str = new StructureType
            {
                Fields = { { 0, tv1 }, { 4, tv2 } }
            };

            // fn4242 is the procedure we are calling.
            var proc = new Procedure(m.Architecture, "fn4242", Address.Ptr32(0x4242), m.Architecture.CreateFrame());
            var procFlow = new ProcedureFlow(proc);
            procFlow.BitsUsed.Add(ecx.Storage, ecx.Storage.GetBitRange());
            procFlow.LiveInDataTypes.Add(ecx.Storage, new Pointer(str, 32));
            pb.Add(proc);
            flow.ProcedureFlows.Add(proc, procFlow);

            m.MStore(m.ISub(fp, 16), m.Word32(0x1234));
            m.MStore(m.ISub(fp, 12), Constant.Real32(12.34F));
            m.Call(proc, 0,
                new (Storage, Expression)[] { (ecx.Storage, m.ISubS(fp, 16)) },
                new (Storage, Identifier)[] { (eax_1.Storage, eax_1) });

            RunEscapedFrameIntervalsFinder();

            var expected = @"
[[-16, -8], (struct (0 T_1 t0000) (4 T_2 t0004))]";
            AssertIntervals(expected);
        }
    }
}
