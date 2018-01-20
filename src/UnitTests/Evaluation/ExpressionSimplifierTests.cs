#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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

using Reko.Analysis;
using Reko.Evaluation;
using Reko.Core;
using Reko.Core.Code;
using Reko.Core.Expressions;
using Reko.Core.Machine;
using Reko.Core.Operators;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Reko.UnitTests.Mocks;
using System.Linq;

namespace Reko.UnitTests.Evaluation
{
    [TestFixture]
    public class ExpressionSimplifierTests
    {
        private ExpressionSimplifier simplifier;
        private Identifier foo;
        private ProcedureBuilder m;

        [SetUp]
        public void Setup()
        {
            m = new ProcedureBuilder();
        }

        private void Given_ExpressionSimplifier()
        {
            SsaIdentifierCollection ssaIds = BuildSsaIdentifiers();
            var listener = new FakeDecompilerEventListener();
            simplifier = new ExpressionSimplifier(new SsaEvaluationContext(null, ssaIds), listener);
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
                    m.Load(PrimitiveType.Real32, m.Word32(0x123400))));
            Assert.AreEqual("Mem0[0x00123400:real32]", expr.Accept(simplifier).ToString());
        }

        [Test]
        public void Exs_AddAddress32Constant()
        {
            Given_ExpressionSimplifier();
            var expr = m.LoadDw(m.IAdd(Address.Ptr32(0x00123400), 0x56));
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
        public void Ecs_UnsignedRangeComparison()
        {
            Given_ExpressionSimplifier();
            var expr = m.Ugt(m.ISub(foo, 2), m.Word32(5));
            Assert.AreEqual("foo_0 >u 0x00000007 || foo_0 <u 0x00000002", expr.Accept(simplifier).ToString());
        }
    }
}