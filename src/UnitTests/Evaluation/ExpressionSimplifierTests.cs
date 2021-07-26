#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using System;

namespace Reko.UnitTests.Evaluation
{
    [TestFixture]
    public class ExpressionSimplifierTests
    {
        private ExpressionSimplifier simplifier;
        private Identifier foo;
        private ProcedureBuilder m;
        private IntrinsicProcedure rolc_8;
        private Mock<IProcessorArchitecture> arch;
        private SsaIdentifierCollection ssaIds;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
            this.rolc_8 = new IntrinsicProcedure(IntrinsicProcedure.RolC, true, PrimitiveType.Byte, 3);
            arch = new Mock<IProcessorArchitecture>();
        }

        private void Given_LittleEndianArchitecture()
        {
            arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
        }

        private void Given_BigEndianArchitecture()
        {
            arch.Setup(a => a.Endianness).Returns(EndianServices.Big);
        }

        private void Given_SegmentedArchitecture()
        {
            arch = new Mock<IProcessorArchitecture>();
            arch.Setup(a => a.Endianness).Returns(EndianServices.Little);
            arch.Setup(a => a.MakeSegmentedAddress(
                It.IsNotNull<Constant>(),
                It.IsNotNull<Constant>()))
                .Returns(new Func<Constant,Constant, Address>((seg, off) => Address.SegPtr(seg.ToUInt16(), off.ToUInt16())));
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
            Assert.AreEqual("foo_1 - 3<32>", result.ToString());
        }

        [Test]
        public void Exs_FloatIeeeConstant_Cmp()
        {
            Given_ExpressionSimplifier();
            arch.Setup(a => a.ReinterpretAsFloat(It.IsAny<Constant>()))
                .Returns(new Func<Constant,Constant>(c =>
                    Constant.FloatFromBitpattern(c.ToInt32())));
            var expr = m.FLt(foo, Constant.Word32(0xC0B00000));
            var result = expr.Accept(simplifier);
            Assert.AreEqual("foo_1 < -5.5F", result.ToString());
        }

        [Test]
        public void Exs_Cast_real()
        {
            Given_ExpressionSimplifier();
            var expr = m.Convert(Constant.Real64(1.5), PrimitiveType.Real64, PrimitiveType.Real32);
            Assert.AreEqual("1.5F", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Slice_byte_typeref()
        {
            Given_ExpressionSimplifier();
            var expr = m.Slice(
                new TypeReference("BYTE", PrimitiveType.Byte),
                Constant.Word32(0x4711),
                0);
            Assert.AreEqual("0x11<8>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Slice32_LargeReal64()
        {
            Given_ExpressionSimplifier();
            var expr = m.Slice(
                PrimitiveType.Word32,
                // 0x4415AF1D78B58C40
                Constant.Real64(1e20),
                0);
            Assert.AreEqual(
                "0x78B58C40<32>",
                expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Slice16_Real32()
        {
            Given_ExpressionSimplifier();
            var expr = m.Slice(
                PrimitiveType.Word16,
                // 0x42280000
                Constant.Real32(42.0F),
                16);
            Assert.AreEqual("0x4228<16>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_CastCast()
        {
            Given_ExpressionSimplifier();
            var expr = m.Convert(
                m.Convert(
                    m.Mem(PrimitiveType.Real32, m.Ptr32(0x123400)),
                    PrimitiveType.Real32,
                    PrimitiveType.Real64),
                PrimitiveType.Real64,
                PrimitiveType.Real32);
            Assert.AreEqual("Mem0[0x00123400<p32>:real32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_AddAddress32Constant()
        {
            Given_ExpressionSimplifier();
            var expr = m.Mem32(m.IAdd(Address.Ptr32(0x00123400), 0x56));
            Assert.AreEqual("Mem0[0x00123456<p32>:word32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConditionalTrue()
        {
            Given_ExpressionSimplifier();
            var expr = m.Conditional(PrimitiveType.Word32, Constant.True(), Constant.Word32(1), Constant.Word32(0));
            Assert.AreEqual("1<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConditionalFalse()
        {
            Given_ExpressionSimplifier();
            var expr = m.Conditional(PrimitiveType.Word32, Constant.False(), Constant.Word32(1), Constant.Word32(0));
            Assert.AreEqual("0<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_UnsignedRangeComparison()
        {
            Given_ExpressionSimplifier();
            var expr = m.Ugt(m.ISub(foo, 2), m.Word32(5));
            Assert.AreEqual("foo_1 >u 7<32> || foo_1 <u 2<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_MulMul()
        {
            Given_ExpressionSimplifier();
            var expr = m.IMul(m.IMul(foo, 2), 2);
            Assert.AreEqual("foo_1 * 4<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_DistributedCast()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var expr = m.IAdd(m.Slice(w16, foo, 0), m.Slice(w16, foo, 0));
            Assert.AreEqual("SLICE(foo_1 * 2<32>, word16, 0)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_DistributedSlice()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var expr = m.IAdd(m.Slice(w16, foo, 0), m.Slice(w16, foo, 0));
            Assert.AreEqual("SLICE(foo_1 * 2<32>, word16, 0)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_RedundantCast()
        {
            Given_ExpressionSimplifier();
            var w16 = PrimitiveType.Word16;
            var w32 = PrimitiveType.Word32;
            var expr = m.Convert(m.Convert(foo, foo.DataType, w16), w16, w16);
            Assert.AreEqual("CONVERT(foo_1, word32, word16)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_CompareWithConstant()
        {
            Given_ExpressionSimplifier();
            var expr = m.Le(m.Word32(0x00123400), foo);
            Assert.AreEqual("foo_1 >= 0x123400<32>", expr.Accept(simplifier).ToString());
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
            Assert.AreEqual("0x0000000000123416<p64>", result.ToString());
        }

        [Test]
        public void Exs_CompareFrameAccess()
        {
            Given_ExpressionSimplifier();
            foo.DataType = PrimitiveType.Ptr32;
            Assert.AreEqual("foo_1 == 0x00000010<p32>", m.Eq0(m.ISubS(foo, 16))
                .Accept(simplifier)
                .ToString());
        }

        [Test]
        public void Exs_Rolc_To_Shl()
        {
            Given_ExpressionSimplifier();
            var expr = m.Fn(rolc_8, foo, m.Byte(1), Constant.False());
            Assert.AreEqual("foo_1 << 1<8>", expr.Accept(simplifier).ToString());
        }

        [Test(Description = "Reported in GitHub issue #733")]
        public void Exs_NormalizeSubForComparison()
        {
            Given_ExpressionSimplifier();
            var expr = m.Le0(m.ISub(m.Word32(0x02), foo));
            Assert.AreEqual("foo_1 >= 2<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ConstantSequence_BigEndian()
        {
            Given_ExpressionSimplifier();
            var expr = m.Seq(m.Word32(0x3FF00000), m.Word32(0));
            Assert.AreEqual("0x3FF0000000000000<64>", expr.Accept(simplifier).ToString());
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
            Assert.AreEqual("Mem0[ds:0x1234<16> + 0<16>:word16]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_redundant_cast_of_ProcedureConstant()
        {
            Given_ExpressionSimplifier();
            var pc = new ProcedureConstant(PrimitiveType.Ptr64, new ExternalProcedure("puts", new FunctionType()));

            var exp = m.Convert(pc, pc.DataType, PrimitiveType.Word64);
            Assert.AreEqual("puts", exp.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_redundant_cast_of_word_type()
        {
            Given_ExpressionSimplifier();
            var value = Constant.Word32(0x00123400);
            value.DataType = PrimitiveType.Ptr32;

            var exp = m.Convert(value, value.DataType, PrimitiveType.Word32);
            Assert.AreEqual("0x00123400<p32>", exp.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_cast_of_unknown_type()
        {
            Given_ExpressionSimplifier();
            var value = foo;
            value.DataType = new UnknownType();
            var exp = m.Convert(value, value.DataType, new UnknownType());

            var result = exp.Accept(simplifier);

            Assert.AreEqual("CONVERT(foo_1, <type-error>, <type-error>)", result.ToString());
        }

        [Test]
        public void Exs_Int32ConstantToReal32Convert()
        {
            Given_ExpressionSimplifier();
            var value = Constant.Int32(0x1);
            var exp = m.Convert(
                value, PrimitiveType.Int32, PrimitiveType.Real32);

            var result = exp.Accept(simplifier);

            Assert.AreEqual("1.0F", result.ToString());
        }

        [Test]
        public void Exs_SegMem_Constants()
        {
            Given_SegmentedArchitecture();
            Given_ExpressionSimplifier();
            var seg = m.Word16(0x1234);
            var off = m.Word16(0x5678);
            var exp = m.SegMem16(seg, off);

            var result = (MemoryAccess) exp.Accept(simplifier);

            var addr = result.EffectiveAddress as Address;
            Assert.IsNotNull(addr);
            Assert.AreEqual("1234:5678", addr.ToString());
        }

        [Test]
        public void Exs_Sequence_Constants()
        {
            var a = m.Word16(0x1234);
            var b = m.Byte(0x56);
            var c = m.Byte(0x78);
            var exp = m.Seq(a, b, c);

            Given_ExpressionSimplifier();
            var result = exp.Accept(simplifier);

            Assert.AreEqual("0x12345678<32>", result.ToString());
        }

        [Test]
        public void Exs_ReduceUnaryNotFollowedByNeg()
        {
            Given_ExpressionSimplifier();
            var expr = m.Not(m.Neg(foo));
            Assert.AreEqual("!-foo_1", expr.ToString());
            Assert.AreEqual("!foo_1", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ReduceNegComparedToZero()
        {
            Given_ExpressionSimplifier();
            var expr = m.Eq0(m.Neg(foo));
            Assert.AreEqual("-foo_1 == 0<32>", expr.ToString());
            Assert.AreEqual("foo_1 == 0<32>", expr.Accept(simplifier).ToString());

            expr = m.Eq(m.Word32(0), m.Neg(foo));
            Assert.AreEqual("0<32> == -foo_1", expr.ToString());
            Assert.AreEqual("foo_1 == 0<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ReduceArithmeticSequenceToLogicalNot()
        {
            Given_ExpressionSimplifier();
            var expr = m.IAdd(m.ISub(m.Word32(0), m.Eq0(m.Neg(foo))), m.Word32(1));
            Assert.AreEqual("0<32> - (-foo_1 == 0<32>) + 1<32>", expr.ToString());
            Assert.AreEqual("!foo_1", expr.Accept(simplifier).ToString());

            expr = m.IAdd(m.ISub(m.Word32(0), m.Eq0(foo)), m.Word32(1));
            Assert.AreEqual("0<32> - (foo_1 == 0<32>) + 1<32>", expr.ToString());
            Assert.AreEqual("!foo_1", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ZeroExtension()
        {
            Given_ExpressionSimplifier();
            var expr = m.Convert(m.Word32(0x42), PrimitiveType.Word32, PrimitiveType.UInt64);
            Assert.AreEqual("0x42<u64>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_SignExtension()
        {
            Given_ExpressionSimplifier();
            var expr = m.Convert(m.Word16(0xFFFF), PrimitiveType.Int16, PrimitiveType.Int32);
            Assert.AreEqual("-1<i32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Or_32_all_ones()
        {
            Given_ExpressionSimplifier();
            var tmp = Given_Tmp("tmp", m.Mem32(m.Word32(0x00123400)));
            var expr = m.Or(tmp, Constant.Word32(0xFFFF_FFFF));
            Assert.AreEqual("0xFFFFFFFF<32>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Or_64_all_ones()
        {
            Given_ExpressionSimplifier();
            var tmp = Given_Tmp("tmp", m.Mem64(m.Word32(0x00123400)));
            var expr = m.Or(tmp, Constant.Word64(0xFFFF_FFFF_FFFF_FFFF));
            Assert.AreEqual("0xFFFFFFFFFFFFFFFF<64>", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Xor_16_all_ones()
        {
            Given_ExpressionSimplifier();
            var tmp = Given_Tmp("tmp", m.Mem16(m.Word32(0x00123400)));
            var expr = m.Xor(tmp, Constant.Word16(0xFFFF));
            Assert.AreEqual("~tmp_2", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Shl_shr()
        {
            Given_ExpressionSimplifier();
            var expr = m.Shr(m.Shl(foo, 24), 24);
            Assert.AreEqual("CONVERT(SLICE(foo_1, byte, 0), byte, word32)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Shl_sar()
        {
            Given_ExpressionSimplifier();
            var expr = m.Sar(m.Shl(foo, 24), 24);
            Assert.AreEqual("CONVERT(SLICE(foo_1, byte, 0), byte, int32)", expr.Accept(simplifier).ToString());
        }

        [Test]
        [Ignore("This requires changes in BinaryOperator.ApplyConstants")]
        public void Exs_Slice_Constant_Multiplication()
        {
            Given_ExpressionSimplifier();
            var mul = m.UMul(m.Word32(0xAAAA_AAAA), m.Word32(0xBBBB_BBBB));
            mul.DataType = PrimitiveType.UInt64;
            var expr = m.Slice(PrimitiveType.Word32, mul, 32);
            Assert.AreEqual("@@@", expr.Accept(simplifier).ToString());
        }

        // Adjacent memory accesses can be coalesced.
        [Test]
        public void Exs_Seq_Adjacent_LE_Memory_Accesses()
        {
            Given_LittleEndianArchitecture();
            Given_ExpressionSimplifier();
            var expr = m.Seq(
                m.Mem(PrimitiveType.Word32, m.Word32(0x00123404)),
                m.Mem(PrimitiveType.Word32, m.Word32(0x00123400)));
            expr.DataType = PrimitiveType.Real64;
            Assert.AreEqual("Mem0[0x123400<32>:real64]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Seq_Adjacent_BE_Memory_Accesses()
        {
            Given_BigEndianArchitecture();
            Given_ExpressionSimplifier();
            var expr = m.Seq(
                m.Mem(PrimitiveType.Word32, m.Word32(0x00123400)),
                m.Mem(PrimitiveType.Word32, m.Word32(0x00123404)));
            expr.DataType = PrimitiveType.Real64;
            Assert.AreEqual("Mem0[0x123400<32>:real64]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Seq_Adjacent_BE_Memory_Accesses_BaseDisplacement()
        {
            Given_BigEndianArchitecture();
            Given_ExpressionSimplifier();
            var expr = m.Seq(
                m.Mem(PrimitiveType.Word32, foo),
                m.Mem(PrimitiveType.Word32, m.IAddS(foo, 4)));
            expr.DataType = PrimitiveType.Real64;
            Assert.AreEqual("Mem0[foo_1:real64]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Slice_Convert()
        {
            Given_ExpressionSimplifier();

            var expr = m.Slice(
                PrimitiveType.Char,
                m.Convert(
                    m.Mem8(foo),
                    PrimitiveType.Byte,
                    PrimitiveType.Word32),
                0);
            Assert.AreEqual("SLICE(Mem0[foo_1:byte], char, 0)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Rol_rol()
        {
            Given_ExpressionSimplifier();
            var r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word32);
            var r1 = new RegisterStorage("r1", 0, 0, PrimitiveType.Word32);
            var sigRol = FunctionType.Func(
                new Identifier("", r0.DataType, r0),
                new Identifier("value", r0.DataType, r0),
                new Identifier("sh", r0.DataType, r1));
            var rol = new IntrinsicProcedure(IntrinsicProcedure.Rol, true, sigRol);
            var exp = m.Fn(rol, m.Fn(rol, foo, m.Word32(1)), m.Word32(1));
            Assert.AreEqual("__rol(foo_1, 2<32>)", exp.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Ror_ror()
        {
            Given_ExpressionSimplifier();
            var r0 = new RegisterStorage("r0", 0, 0, PrimitiveType.Word32);
            var r1 = new RegisterStorage("r1", 0, 0, PrimitiveType.Word32);
            var sigRol = FunctionType.Func(
                new Identifier("", r0.DataType, r0),
                new Identifier("value", r0.DataType, r0),
                new Identifier("sh", r0.DataType, r1));
            var ror = new IntrinsicProcedure(IntrinsicProcedure.Ror, true, sigRol);
            var exp = m.Fn(ror, m.Fn(ror, foo, m.Word32(2)), m.Word32(1));
            Assert.AreEqual("__ror(foo_1, 3<32>)", exp.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Shifts_Used_ToAlign()
        {
            Given_ExpressionSimplifier();
            var t1 = Given_Tmp("t1", m.Mem32(m.Word32(0x123400)));
            var t2 = Given_Tmp("t2", m.Shr(t1, 4));
            ssaIds[foo].Uses.Add(ssaIds[t1].DefStatement);
            var expr = m.Shl(t2, 4);
            Assert.AreEqual("__align(t1_2, 16<i32>)", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ScaledIndex_Multiplication_LeftSide()
        {
            Given_ExpressionSimplifier();
            var t1 = Given_Tmp("t1", m.Mem32(m.Word32(0x123400)));
            var t2 = Given_Tmp("t2", m.IMul(t1, 4));
            ssaIds[foo].Uses.Add(ssaIds[t1].DefStatement);
            var expr = m.Mem32(m.IAdd(t2, 24));
            Assert.AreEqual("Mem0[t1_2 * 4<32> + 0x18<32>:word32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_ScaledIndex_Multiplication_RightSide()
        {
            Given_ExpressionSimplifier();
            var t1 = Given_Tmp("t1", m.Mem32(m.Word32(0x123400)));
            var t2 = Given_Tmp("t2", m.Shl(t1, 3));
            ssaIds[foo].Uses.Add(ssaIds[t1].DefStatement);
            var expr = m.Mem32(m.IAdd(Constant.Word32(0x00123456), t2));
            Assert.AreEqual("Mem0[(t1_2 << 3<8>) + 0x123456<32>:word32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_Dont_simplify_nans()
        {
            Given_ExpressionSimplifier();
            var expr = m.ISub(
                Constant.Create(PrimitiveType.Real32, 0xFFFFFFFF),
                Constant.Int32(-1));
            Assert.AreEqual("NaN.0F - -1<i32>", expr.Accept(simplifier).ToString());
        }
    }
}