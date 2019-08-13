#region License
/* 
 * Copyright (C) 1999-2019 John Källén.
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

using Moq;
using NUnit.Framework;
using Reko.Analysis;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using Reko.UnitTests.Mocks;

namespace Reko.UnitTests.Evaluation
{
    [TestFixture]
    public class ExpressionSimplifierTests
    {
        private ExpressionSimplifier simplifier;
        private Identifier foo;
        private ProcedureBuilder m;
        private PseudoProcedure rolc_8;
        private Mock<IProcessorArchitecture> arch;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            this.rolc_8 = new PseudoProcedure(PseudoProcedure.RolC, PrimitiveType.Byte, 3);
        }

        private void Given_ExpressionSimplifier()
        {
            var ssaIds = BuildSsaIdentifiers();
            var listener = new FakeDecompilerEventListener();
            var segmentMap = new SegmentMap(Address.Ptr32(0));
            var importResolver = new Mock<IImportResolver>();
            var ssaCtx = new SsaEvaluationContext(arch?.Object, ssaIds, importResolver.Object);
            simplifier = new ExpressionSimplifier(segmentMap, ssaCtx, listener);
        }

        private SsaIdentifierCollection BuildSsaIdentifiers()
        {
            var mrFoo = new RegisterStorage("foo", 1, 0, PrimitiveType.Word32);
            var mrBar = new RegisterStorage("bar", 2, 1, PrimitiveType.Word32);
            foo = new Identifier(mrFoo.Name, mrFoo.DataType, mrFoo);

            var coll = new SsaIdentifierCollection();
            var src = Constant.Word32(1);
            foo = coll.Add(foo, new Statement(0, new Assignment(foo, src), null), src, false).Identifier;
            return coll;
        }

        [Test]
        public void Exs_Constants()
        {
            Given_ExpressionSimplifier();
            Expression expr = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32,
                Constant.Word32(1), Constant.Word32(2));
            Constant c = (Constant)expr.Accept(simplifier);

            Assert.AreEqual(3, c.ToInt32());
        }

        [Test]
        public void Exs_OrWithSelf()
        {
            Given_ExpressionSimplifier();
            var expr = new BinaryExpression(Operator.Or, foo.DataType, foo, foo);
            var result = expr.Accept(simplifier);
            Assert.AreSame(foo, result);
        }

        [Test]
        public void Exs_AddPositiveConstantToNegative()
        {
            Given_ExpressionSimplifier();
            var expr = new BinaryExpression(
                Operator.IAdd,
                foo.DataType,
                new BinaryExpression(
                    Operator.ISub,
                    foo.DataType,
                    foo,
                    Constant.Word32(4)),
                Constant.Word32(1));
            var result = expr.Accept(simplifier);
            Assert.AreEqual("foo_0 - 0x00000003", result.ToString());
        }

        [Test]
        public void Exs_FloatIeeeConstant_Cmp()
        {
            Given_ExpressionSimplifier();
            var expr = m.FLt(foo, Constant.Word32(0xC0B00000));
            var result = expr.Accept(simplifier);
            Assert.AreEqual("foo_0 < -5.5F", result.ToString());
        }

        [Test]
        public void Exs_Cast_real()
        {
            Given_ExpressionSimplifier();
            var expr = m.Cast(PrimitiveType.Real32, Constant.Real64(1.5));
            Assert.AreEqual("1.5F", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Cast_byte_typeref()
        {
            Given_ExpressionSimplifier();
            var expr = m.Cast(new TypeReference("BYTE", PrimitiveType.Byte), Constant.Word32(0x11));
            Assert.AreEqual("0x11", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_CastCast()
        {
            Given_ExpressionSimplifier();
            var expr = m.Cast(
                PrimitiveType.Real32,
                m.Cast(
                    PrimitiveType.Real64,
                    m.Mem(PrimitiveType.Real32, m.Word32(0x123400))));
            Assert.AreEqual("Mem0[0x00123400:real32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_AddAddress32Constant()
        {
            Given_ExpressionSimplifier();
            var expr = m.Mem32(m.IAdd(Address.Ptr32(0x00123400), 0x56));
            Assert.AreEqual("Mem0[0x00123456:word32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConditionalTrue()
        {
            Given_ExpressionSimplifier();
            var expr = m.Conditional(PrimitiveType.Word32, Constant.True(), Constant.Word32(1), Constant.Word32(0));
            Assert.AreEqual("0x00000001", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConditionalFalse()
        {
            Given_ExpressionSimplifier();
            var expr = m.Conditional(PrimitiveType.Word32, Constant.False(), Constant.Word32(1), Constant.Word32(0));
            Assert.AreEqual("0x00000000", expr.Accept(simplifier).ToString());
        }
       
        [Test]
        public void Exs_UnsignedRangeComparison()
        {
            Given_ExpressionSimplifier();
            var expr = m.Ugt(m.ISub(foo, 2), m.Word32(5));
            Assert.AreEqual("foo_0 >u 0x00000007 || foo_0 <u 0x00000002", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_MulMul()
        {
            Given_ExpressionSimplifier();
            var expr = m.IMul(m.IMul(foo, 2), 2);
            Assert.AreEqual("foo_0 * 0x00000004", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_DistributedCast()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var expr = m.IAdd(m.Cast(w16, foo), m.Cast(w16, foo));
            Assert.AreEqual("(word16) (foo_0 * 0x00000002)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_DistributedSlice()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var expr = m.IAdd(m.Slice(w16, foo, 0), m.Slice(w16, foo, 0));
            Assert.AreEqual("SLICE(foo_0 * 0x00000002, word16, 0)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_RedundantCast()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var w32 = PrimitiveType.Word32;
            var expr = m.Cast(w16, (m.Cast(w16, foo)));
            Assert.AreEqual("(word16) foo_0", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Rolc_To_Shl()
        {
            Given_ExpressionSimplifier();
            var expr = m.Fn(rolc_8, foo, m.Byte(1), Constant.False());
            Assert.AreEqual("foo_0 << 0x01", expr.Accept(simplifier).ToString());
        }

        [Test(Description = "Reported in GitHub issue #733")]
        public void Exs_NormalizeSubForComparison()
        {
            Given_ExpressionSimplifier();
            var expr = m.Le0(m.ISub(m.Word32(0x02), foo));
            Assert.AreEqual("foo_0 >= 0x00000002", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConstantSequence_BigEndian()
        {
            Given_ExpressionSimplifier();
            var expr = m.Seq(m.Word32(0x3FF00000), m.Word32(0));
            Assert.AreEqual("0x3FF0000000000000", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_SeqOfSlices_Adjacent()
        {
            Given_ExpressionSimplifier();
            var expr = m.Seq(m.Slice(foo, 8, 24), m.Slice(foo, 0, 8));
            Assert.AreEqual("foo_0", expr.Accept(simplifier).ToString());
        }
    }
}