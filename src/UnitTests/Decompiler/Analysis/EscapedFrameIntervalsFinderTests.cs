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
using Reko.Core.Expressions;
using Reko.Core.Lib;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Decompiler.Analysis
{
    [TestFixture]
    public class EscapedFrameIntervalsFinderTests
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
        }

        private static DataType Ptr32(DataType dt)
        {
            return new Pointer(dt, 32);
        }

        private void RunEscapedFrameIntervalsFinder()
        {
            var eventListener = new FakeDecompilerEventListener();
            var efif = new EscapedFrameIntervalsFinder(
                program, m.Ssa, eventListener);
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

            RunEscapedFrameIntervalsFinder();

            var expected = @"
[[-12, -4], (struct ""str"" 0008 (0 int32 dw0000) (4 real32 r0004))]";
            AssertIntervals(expected);
        }
    }
}
