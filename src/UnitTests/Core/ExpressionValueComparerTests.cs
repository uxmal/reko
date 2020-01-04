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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class ExpressionValueComparerTests
    {
        private IEqualityComparer<Expression> eq;

        [SetUp]
        public void Setup()
        {
            eq = new ExpressionValueComparer();
        }

        [Test]
        public void EvcIdentifiers()
        {
            Identifier a = new Identifier("a", PrimitiveType.Word32, new TemporaryStorage("a", 1, PrimitiveType.Word32));
            Identifier aa = new Identifier("a", PrimitiveType.Word32, new TemporaryStorage("a", 2, PrimitiveType.Word32 ));
            Assert.IsTrue(eq.Equals(a, aa));
        }

        [Test]
        public void EvcTestCondition()
        {
            TestCondition tc1 = new TestCondition(ConditionCode.EQ, new Identifier("a", PrimitiveType.Word32, new TemporaryStorage("a", 1, PrimitiveType.Word32)));
            TestCondition tc2 = new TestCondition(ConditionCode.EQ, new Identifier("a", PrimitiveType.Word32, new TemporaryStorage("a", 1, PrimitiveType.Word32)));
            Assert.IsTrue(eq.Equals(tc1, tc2));
            Assert.AreEqual(eq.GetHashCode(tc1), eq.GetHashCode(tc2));
        }

        [Test]
        public void EvcApplication()
        {
            Identifier pfn = new Identifier("pfn", PrimitiveType.Ptr32, new TemporaryStorage("pfn", 1, PrimitiveType.Ptr32));
            Application a1 = new Application(pfn, PrimitiveType.Int32, pfn);
            Application a2 = new Application(pfn, PrimitiveType.Int32, pfn);
            Assert.IsTrue(eq.Equals(a1, a2));
            Assert.AreEqual(eq.GetHashCode(a1), eq.GetHashCode(a2));
        }

        [Test]
        public void EvcBinaryExpression()
        {
            Identifier a = new Identifier("a", PrimitiveType.Word32, new TemporaryStorage("a", 1, PrimitiveType.Word32));
            BinaryExpression a1 = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, a, a);
            BinaryExpression a2 = new BinaryExpression(Operator.IAdd, PrimitiveType.Word32, a, a);
            Assert.IsTrue(eq.Equals(a1, a2));
            Assert.AreEqual(eq.GetHashCode(a1), eq.GetHashCode(a2));
        }

        [Test]
        public void EvcConHash()
        {
            Constant c1 = Constant.Word32(3);
            Constant c2 = Constant.Word32(3);
            Assert.AreEqual(eq.GetHashCode(c1), eq.GetHashCode(c2));
        }

        [Test]
        public void EvcNegative1()
        {
            Constant c1 = Constant.Int32(-1);
            Constant c2 = Constant.Int32(-1);
            Assert.IsTrue(eq.Equals(c1, c2));
        }
    }
}
