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
        private SsaIdentifierCollection ssaIds;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            this.rolc_8 = new PseudoProcedure(PseudoProcedure.RolC, PrimitiveType.Byte, 3);
        }

        private void Given_LittleEndianArchitecture()
        {
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
        }

        private void Given_BigEndianArchitecture()
        {
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Endianness).Returns(EndianServices.Big);
        }

        private void Given_ExpressionSimplifier()
        {
            this.ssaIds = BuildSsaIdentifiers();
            var listener = new FakeDecompilerEventListener();
            var segmentMap = new SegmentMap(Address.Ptr32(0));
            var dynamicLinker = new Mock<IDynamicLinker>();
            var ssaCtx = new SsaEvaluationContext(arch?.Object, ssaIds, dynamicLinker.Object);
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

        private Identifier Given_Tmp(string name, Expression defExpr)
        {
            var tmp = m.Procedure.Frame.CreateTemporary(name, defExpr.DataType);
            return ssaIds.Add(tmp, new Statement(0, new Assignment(tmp, defExpr), null), defExpr, false).Identifier;
        }

        [Test]
        public void Exs_Constants()
        {
            Given_ExpressionSimplifier();
            Expression expr = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32,
                Constant.Word32(1), Constant.Word32(2));
            Constant c = (Constant) expr.Accept(simplifier);

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
            Assert.AreEqual("foo_1 - 0x00000003", result.ToString());
        }

        [Test]
        public void Exs_FloatIeeeConstant_Cmp()
        {
            Given_ExpressionSimplifier();
            var expr = m.FLt(foo, Constant.Word32(0xC0B00000));
            var result = expr.Accept(simplifier);
            Assert.AreEqual("foo_1 < -5.5F", result.ToString());
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
            Assert.AreEqual("foo_1 >u 0x00000007 || foo_1 <u 0x00000002", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_MulMul()
        {
            Given_ExpressionSimplifier();
            var expr = m.IMul(m.IMul(foo, 2), 2);
            Assert.AreEqual("foo_1 * 0x00000004", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_DistributedCast()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var expr = m.IAdd(m.Cast(w16, foo), m.Cast(w16, foo));
            Assert.AreEqual("(word16) (foo_1 * 0x00000002)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_DistributedSlice()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var expr = m.IAdd(m.Slice(w16, foo, 0), m.Slice(w16, foo, 0));
            Assert.AreEqual("SLICE(foo_1 * 0x00000002, word16, 0)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_RedundantCast()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var w32 = PrimitiveType.Word32;
            var expr = m.Cast(w16, (m.Cast(w16, foo)));
            Assert.AreEqual("(word16) foo_1", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_CompareWithConstant()
        {
            Given_ExpressionSimplifier();
            var expr = m.Le(m.Word32(0x00123400), foo);
            Assert.AreEqual("foo_1 >= 0x00123400", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConstIntPtr()
        {
            Given_ExpressionSimplifier();
            var c1 = Constant.Create(PrimitiveType.Ptr64, 0x00123400);
            var c2 = Constant.Create(PrimitiveType.Int64, 0x00000016);
            var expr = m.IAdd(c1, c2);
            var result = expr.Accept(simplifier);
            Assert.AreSame(PrimitiveType.Ptr64, result.DataType);
            Assert.AreEqual("0x0000000000123416", result.ToString());
        }

        [Test]
        public void Exs_CompareFrameAccess()
        {
            Given_ExpressionSimplifier();
            foo.DataType = PrimitiveType.Ptr32;
            Assert.AreEqual("foo_1 == 0x00000010", m.Eq0(m.ISubS(foo, 16))
                .Accept(simplifier)
                .ToString());
        }

        [Test]
        public void Exs_Rolc_To_Shl()
        {
            Given_ExpressionSimplifier();
            var expr = m.Fn(rolc_8, foo, m.Byte(1), Constant.False());
            Assert.AreEqual("foo_1 << 0x01", expr.Accept(simplifier).ToString());
        }

        [Test(Description = "Reported in GitHub issue #733")]
        public void Exs_NormalizeSubForComparison()
        {
            Given_ExpressionSimplifier();
            var expr = m.Le0(m.ISub(m.Word32(0x02), foo));
            Assert.AreEqual("foo_1 >= 0x00000002", expr.Accept(simplifier).ToString());
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
            Assert.AreEqual("foo_1", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_SeqOfSlices_Indirect()
        {
            Given_ExpressionSimplifier();
            var t1 = Given_Tmp("t1", m.Slice(PrimitiveType.Word16, foo, 16));
            var t2 = Given_Tmp("t2", m.Slice(PrimitiveType.Word16, foo, 0));
            ssaIds[foo].Uses.Add(ssaIds[t1].DefStatement);
            ssaIds[foo].Uses.Add(ssaIds[t2].DefStatement);
            var expr = m.Seq(t1, t2);
            Assert.AreEqual("foo_1", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Slice_of_segaccess()
        {
            Given_ExpressionSimplifier();
            var ds = m.Temp(PrimitiveType.SegmentSelector, "ds");
            ssaIds.Add(ds, null, null, false);
            var expr = m.Slice(
                PrimitiveType.Word16, 
                m.SegMem(PrimitiveType.SegPtr32, ds, m.Word16(0x1234)),
                0);
            Assert.AreEqual("Mem0[ds:0x1234 + 0x0000:word16]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_redundant_cast_of_ProcedureConstant()
        {
            Given_ExpressionSimplifier();
            var pc = new ProcedureConstant(PrimitiveType.Ptr64, new ExternalProcedure("puts", new FunctionType()));

            var exp = m.Cast(PrimitiveType.Word64, pc);
            Assert.AreEqual("puts", exp.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_redundant_cast_of_word_type()
        {
            Given_ExpressionSimplifier();
            var value = Constant.Word32(0x00123400);
            value.DataType = PrimitiveType.Ptr32;

            var exp = m.Cast(PrimitiveType.Word32, value);
            Assert.AreEqual("0x00123400", exp.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_cast_of_unknown_type()
        {
            Given_ExpressionSimplifier();
            var value = foo;
            value.DataType = new UnknownType();
            var exp = m.Cast(new UnknownType(), value);

            var result = exp.Accept(simplifier);

            Assert.AreEqual("(<type-error>) foo_1", result.ToString());
        }
    }
}