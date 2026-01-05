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
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;

namespace Reko.UnitTests.Decompiler.Evaluation
{
    [TestFixture]
    public class ExpressionMatcherTests
    {
        private ExpressionEmitter m;
        private ExpressionMatcher matcher;

        [SetUp]
        public void Setup()
        {
            m = new ExpressionEmitter();
        }

        private void Create(Expression pattern)
        {
            matcher = new ExpressionMatcher(pattern);
        }

        [Test]
        public void Emt_Identifier()
        {
            string name = "foo";
            var id = Id(name);
            Create(Id("foo"));
            Assert.IsTrue(matcher.Match(id).Success);
        }

        private static Identifier Id(string name)
        {
            return new Identifier(name, PrimitiveType.Word32, null);
        }

        [Test]
        public void Emt_MatchConstant()
        {
            var c = m.Word32(4);
            Create(m.Word32(4));
            Assert.IsTrue(matcher.Match(c).Success);
        }

        [Test]
        public void Emt_MatchAnyConstant()
        {
            var c = this.m.Word32(4);
            Create(ExpressionMatcher.AnyConstant("c"));
            var m = matcher.Match(c);
            Assert.IsTrue(m.Success);
            Assert.AreEqual("4<32>", m.CapturedExpression("c").ToString());
        }

        [Test]
        public void Emt_MatchBinOp()
        {
            var b = m.IAdd(Id("esp"), 4);
            Create(m.IAdd(Id("esp"), 4));
            Assert.IsTrue(matcher.Match(b).Success);
        }

        [Test]
        public void Emt_BinOpMismatch()
        {
            var b = m.IAdd(Id("esp"), 4);
            Create(m.ISub(Id("esp"), 4));
            Assert.IsFalse(matcher.Match(b).Success);
        }

        [Test]
        public void Emt_MemAccess()
        {
            var mem = m.Mem16(m.IAdd(Id("ebx"), 4));
            Create(m.Mem16(m.IAdd(AnyId("idx"), AnyC("Offset"))));
            var match = matcher.Match(mem);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("ebx", match.CapturedExpression("idx").ToString());
        }

        [Test]
        public void Emt_MatchAnyOp()
        {
            var sum = m.IAdd(Id("ebx"), Id("ecx"));
            Create(m.Bin(AnyBinOp("op"), sum.DataType, AnyId("left"), AnyId("right")));
            var match = matcher.Match(sum);
            Assert.IsTrue(match.Success);
            Assert.AreEqual(" + ", match.CapturedOperator("op").ToString());
        }

        [Test]
        public void Emt_MatchCondOf()
        {
            var e = m.IAdd(Id("ebx"), m.Cond(PrimitiveType.Word32, Id("ecx")));
            Create(m.IAdd(AnyId(""), m.Cond(AnyType(), AnyId("q"))));
            var match = matcher.Match(e);
            Assert.IsTrue(match.Success);
            Assert.AreEqual("ecx", match.CapturedExpression("q").ToString());
        }

        [Test]
        public void Emt_ArrayAccessMismatch()
        {
            var e = m.Array(PrimitiveType.Int32, Id("eax"), m.Word32(6));
            Create(m.Word32(5));
            Assert.IsFalse(matcher.Match(e).Success);
        }

        [Test]
        public void Emt_SliceMismatch()
        {
            var e = m.Slice(Id("eax"), PrimitiveType.Int16, 16);
            Create(m.Word32(5));
            Assert.IsFalse(matcher.Match(e).Success);
        }

        private Expression AnyC(string p)
        {
            return ExpressionMatcher.AnyConstant(p);
        }

        private Expression AnyId(string label)
        {
            return ExpressionMatcher.AnyId(label);
        }

        private BinaryOperator AnyBinOp(string label)
        {
            return ExpressionMatcher.AnyBinaryOperator(label);
        }

        private DataType AnyType()
        {
            return ExpressionMatcher.AnyDataType(null);
        }
    }
}
