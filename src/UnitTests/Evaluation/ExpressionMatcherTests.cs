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

using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Evaluation
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
            Assert.IsTrue(matcher.Match(id));
        }

        private static Identifier Id(string name)
        {
            return new Identifier(name, PrimitiveType.Word32, null);
        }

        [Test]
        public void Emt_MatchConstant()
        {
            var c = Constant.Word32(4);
            Create(Constant.Word32(4));
            Assert.IsTrue(matcher.Match(c));
        }

        [Test]
        public void Emt_MatchAnyConstant()
        {
            var c = Constant.Word32(4);
            Create(ExpressionMatcher.AnyConstant("c"));
            Assert.IsTrue(matcher.Match(c));
            Assert.AreEqual("0x00000004", matcher.CapturedExpression("c").ToString());
        }

        [Test]
        public void Emt_MatchBinOp()
        {
            var b = m.IAdd(Id("esp"), 4);
            Create(m.IAdd(Id("esp"), 4));
            Assert.IsTrue(matcher.Match(b));
        }

        [Test]
        public void Emt_BinOpMismatch()
        {
            var b = m.IAdd(Id("esp"), 4);
            Create(m.ISub(Id("esp"), 4));
            Assert.IsFalse(matcher.Match(b));
        }

        [Test]
        public void Emt_MemAccess()
        {
            var mem = m.Mem16(m.IAdd(Id("ebx"), 4));
            Create(m.Mem16(m.IAdd(AnyId("idx"), AnyC("offset"))));
            Assert.IsTrue(matcher.Match(mem));
            Assert.AreEqual("ebx", matcher.CapturedExpression("idx").ToString());
        }

        [Test]
        public void Emt_MatchAnyOp()
        {
            var sum = m.IAdd(Id("ebx"), Id("ecx"));
            Create(new BinaryExpression(AnyOp("op"), sum.DataType, AnyId("left"), AnyId("right")));
            Assert.IsTrue(matcher.Match(sum));
            Assert.AreEqual(" + ", matcher.CapturedOperators("op").ToString());
        }

        [Test]
        public void Emt_MatchCondOf()
        {
            var e = m.IAdd(Id("ebx"), m.Cond(Id("ecx")));
            Create(m.IAdd(AnyId(""), m.Cond(AnyId("q"))));
            Assert.IsTrue(matcher.Match(e));
            Assert.AreEqual("ecx", matcher.CapturedExpression("q").ToString());
        }

        [Test]
        public void Emt_ArrayAccessMismatch()
        {
            var e = m.Array(PrimitiveType.Int32, Id("eax"), m.Word32(6));
            Create(m.Word32(5));
            Assert.IsFalse(matcher.Match(e));
        }

        [Test]
        public void Emt_SliceMismatch()
        {
            var e = m.Slice(PrimitiveType.Int16, Id("eax"), 16);
            Create(m.Word32(5));
            Assert.IsFalse(matcher.Match(e));
        }

        private Expression AnyC(string p)
        {
            return ExpressionMatcher.AnyConstant(p);
        }

        private Expression AnyId(string label)
        {
            return ExpressionMatcher.AnyId(label);
        }

        private Operator AnyOp(string label)
        {
            return ExpressionMatcher.AnyOperator(label);
        }
    }
}
