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
using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Types;
using Reko.Scanning;
using Reko.UnitTests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Scanning
{
    [TestFixture]
    public class ValueSetEvaluatorTests
    {
        private ValueSetEvaluator vse;
        private Program program;
        private Dictionary<Expression, ValueSet> valueSets;
        private ProcedureBuilder m;
        private DataType W16;
        private DataType W32;

        [SetUp]
        public void Setup()
        {
            vse = null;
            var addr = Address.Ptr32(0x2000);
            m = new ProcedureBuilder();
            W16 = PrimitiveType.Word16;
            W32 = PrimitiveType.Word32;
            program = new Program
            {
                Architecture = m.Architecture,
                SegmentMap = new SegmentMap(
                    Address.Ptr32(0x2000),
                    new ImageSegment(
                        "blob",
                        new MemoryArea(addr, new byte[0x400]),
                        AccessMode.ReadWriteExecute),
                    new ImageSegment(
                        "segmentedBlob",
                        new MemoryArea(Address.SegPtr(0xC00, 0), new Byte[0x400]),
                        AccessMode.ReadWriteExecute))
            };

            this.valueSets = new Dictionary<Expression, ValueSet>(new ExpressionValueComparer());
        }

        private void Given_ValueSet(Identifier exp, ValueSet valueSet)
        {
            valueSets.Add(exp, valueSet);
        }

        private void Given_Evaluator()
        {
            this.vse = new ValueSetEvaluator(
                program.Architecture,
                program.SegmentMap,
                valueSets);
        }

        private void Given_UInt32Array(uint uAddr, uint[] uints)
        {
            var w = program.CreateImageWriter(program.Architecture, Address.Ptr32(uAddr));
            foreach (uint u in uints)
            {
                w.WriteUInt32(u);
            }
        }

        private void Given_UInt16Array(Address addr, ushort[] ushorts)
        {
            var w = program.CreateImageWriter(program.Architecture, addr);
            foreach (ushort u in ushorts)
            {
                w.WriteUInt16(u);
            }
        }

        private ValueSet IVS(int stride, long low, long high)
        {
            return new IntervalValueSet(PrimitiveType.Word32, StridedInterval.Create(stride, low, high));
        }

        private ValueSet CVS(params int[] values)
        {
            return new ConcreteValueSet(
                PrimitiveType.Word32,
                values
                    .Select(v => Constant.Create(PrimitiveType.Word32, v))
                    .ToArray());
        }


        private string DumpReads(Dictionary<Address, DataType> reads)
        {
            return string.Format("({0})",
                string.Join(",", reads.OrderBy(d => d.Key)));
        }

        [Test]
        public void Vse_Identifier()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, 0, 20));
            Given_Evaluator();

            var vs = vse.Evaluate(r1);
            Assert.AreEqual("4[0,14]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Sum()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, 0, 20));
            Given_Evaluator();

            var vs = vse.Evaluate(m.IAdd(r1, 9));
            Assert.AreEqual("4[9,1D]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Load()
        {
            Given_UInt32Array(0x2000, new uint[] { 0x3000, 0x3028, 0x3008 });

            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, 0x2000, 0x2008));
            Given_Evaluator();

            var vs = vse.Evaluate(m.Mem32(r1));
            Assert.AreEqual("[0x00003000,0x00003028,0x00003008]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_And()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, -4000, 4000));
            Given_Evaluator();

            var vs = vse.Evaluate(m.And(r1, 0x1F));
            Assert.AreEqual("1[0,1F]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Shl()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, -0x40, 0x40));
            Given_Evaluator();

            var vs = vse.Evaluate(m.Shl(r1, 2));
            Assert.AreEqual("10[-100,100]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Cast_Truncate_SingleValue()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(0, -0x43F, -0x43F));
            Given_Evaluator();

            var vs = vse.Evaluate(m.Cast(PrimitiveType.Byte, r1));
            Assert.AreEqual("0[-3F,-3F]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Cast_Truncate_Interval()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, -0x400, 0x400));
            Given_Evaluator();

            var vs = vse.Evaluate(m.Cast(PrimitiveType.Byte, r1));
            Assert.AreEqual("4[0,FF]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Cast_SignExtend()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, CVS(0x1FF, 0x00, 0x7F));
            Given_Evaluator();

            var vs = vse.Evaluate(m.Cast(
                PrimitiveType.Int32,
                m.Cast(PrimitiveType.Byte, r1)));
            Assert.AreEqual("[-1,0,127]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Add_self()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, IVS(4, 10, 20));
            Given_Evaluator();

            var vs = vse.Evaluate(m.IAdd(r1, r1));
            Assert.AreEqual("8[14,28]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_Add_Mul()
        {
            var r1 = m.Reg32("r1", 1);
            Given_ValueSet(r1, CVS(3, 9, 10));
            Given_Evaluator();

            var vs = vse.Evaluate(m.IMul(r1, 4));
            Assert.AreEqual("[0x0000000C,0x00000024,0x00000028]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_segmented_addrs()
        {
            program.Architecture = new Reko.Arch.X86.X86ArchitectureReal("x86-real-16");
            Given_UInt16Array(Address.SegPtr(0xC00, 4), new ushort[] {
                0x1234,
                0x0C00,
                0x2468,
                0x0C00,
                0x369C,
                0x0C00,
            });

            var bx = m.Reg16("bx", 3);
            var cs = m.Reg16("cs", 0x0C00);
            Given_ValueSet(bx, IVS(1, 0, 2));
            Given_ValueSet(cs, IVS(0, 0xC00, 0xC00));
            Given_Evaluator();

            var vs = vse.Evaluate(m.SegMem(PrimitiveType.SegPtr32, cs, m.IAdd(m.Shl(bx, 2), 4)));
            Assert.AreEqual("[0C00:1234,0C00:2468,0C00:369C]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_NestedCasts()
        {
            // Extracted from:
            // (int32) (int16) Mem0[(word32) (word16) ((word32) r0 * 0x00000002) + 0x0010EC32:word16] + 0x0010EC30

            var r0 = m.Reg32("r0", 0);

            Given_ValueSet(r0, IVS(1, 0, 3));
            Given_Evaluator();

            var exp = m.IAdd(
                m.Cast(W32, m.Cast(W16, m.IMul(m.Cast(W32, r0), 2))),
                0x00100000);
            var vs = vse.Evaluate(exp);
            Assert.AreEqual("2[100000,100006]", vs.Item1.ToString());
        }

        [Test]
        public void Vse_GetType()
        {
            Given_UInt32Array(0x2080, new uint[] { 0x1100, 0x1060, 0x1800 });
            var r0 = m.Reg32("r0", 0);
            Given_ValueSet(r0, IVS(1, 0, 2));
            Given_Evaluator();

            var exp = m.Mem32(m.IAdd(m.IMul(r0, 4), 0x2080));
            var (vs, reads) = vse.Evaluate(exp);
            Assert.AreEqual("[0x00001100,0x00001060,0x00001800]", vs.ToString());
            Assert.AreEqual("([00002080, word32],[00002084, word32],[00002088, word32])", DumpReads(reads));
        }

        [Test(Description = "Fix for GitHub issue #784")]
        public void Vse_CastOfSequence()
        {
            var r0 = m.Reg16("r0", 0);
            var r1 = m.Reg16("r1", 1);
            Given_ValueSet(r0, IVS(1, 0, 3));
            Given_ValueSet(r1, ValueSet.Any);
            Given_Evaluator();

            var exp = m.Cast(PrimitiveType.Int16, m.Seq(r1, r0));
            var (vs, reads) = vse.Evaluate(exp);
            Assert.AreEqual("1[0,3]", vs.ToString());
        }

        [Test]
        public void Vse_SliceOfSequence()
        {
            var r0 = m.Reg16("r0", 0);
            var r1 = m.Reg16("r1", 1);
            Given_ValueSet(r0, IVS(1, 0, 3));
            Given_ValueSet(r1, ValueSet.Any);
            Given_Evaluator();

            var exp = m.Slice(PrimitiveType.Int16, m.Seq(r1, r0), 0);
            var (vs, reads) = vse.Evaluate(exp);
            Assert.AreEqual("1[0,3]", vs.ToString());
        }

        [Test]
        public void Vse_DeepSequence()
        {
            var r0 = m.Reg16("r0", 0);
            var r1 = m.Reg16("r1", 1);
            var r2 = m.Reg16("r2", 2);
            Given_ValueSet(r0, IVS(1, 0, 3));
            Given_ValueSet(r1, ValueSet.Any);
            Given_ValueSet(r2, ValueSet.Any);
            Given_Evaluator();

            var exp = m.Cast(
                PrimitiveType.Int16,
                m.Seq(r2, m.Slice(m.IMul(m.Seq(r1, r0), 2), 0, 16)));
            var (vs, reads) = vse.Evaluate(exp);
            Assert.AreEqual("2[0,6]", vs.ToString());
        }
    }
}
