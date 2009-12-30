/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Decompiler.UnitTests.Core
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
        public void Identifiers()
        {
            Identifier a = new Identifier("a", 1, PrimitiveType.Word32, new TemporaryStorage());
            Identifier aa = new Identifier("aa", 1, PrimitiveType.Word32, new TemporaryStorage());
            Assert.IsTrue(eq.Equals(a, aa));
        }

        [Test]
        public void TestCondition()
        {
            TestCondition tc1 = new TestCondition(ConditionCode.EQ, new Identifier("a", 1, PrimitiveType.Word32, new TemporaryStorage()));
            TestCondition tc2 = new TestCondition(ConditionCode.EQ, new Identifier("a", 1, PrimitiveType.Word32, new TemporaryStorage()));
            Assert.IsTrue(eq.Equals(tc1, tc2));
            Assert.AreEqual(eq.GetHashCode(tc1), eq.GetHashCode(tc2));
        }

        [Test]
        public void Application()
        {
            Identifier pfn = new Identifier("pfn", 1, PrimitiveType.Pointer32, new TemporaryStorage());
            Application a1 = new Application(pfn, PrimitiveType.Int32, pfn);
            Application a2 = new Application(pfn, PrimitiveType.Int32, pfn);
            Assert.IsTrue(eq.Equals(a1, a2));
            Assert.AreEqual(eq.GetHashCode(a1), eq.GetHashCode(a2));
        }

        [Test]
        public void BinaryExpression()
        {
            Identifier a = new Identifier("a", 1, PrimitiveType.Word32, new TemporaryStorage());
            BinaryExpression a1 = new BinaryExpression(Operator.add, PrimitiveType.Word32, a, a);
            BinaryExpression a2 = new BinaryExpression(Operator.add, PrimitiveType.Word32, a, a);
            Assert.IsTrue(eq.Equals(a1, a2));
            Assert.AreEqual(eq.GetHashCode(a1), eq.GetHashCode(a2));

        }

        [Test]
        public void ConHash()
        {
            Constant c1 = new Constant(PrimitiveType.Word32, 3);
            Constant c2 = new Constant(PrimitiveType.Word32, 3);
            Assert.AreEqual(eq.GetHashCode(c1), eq.GetHashCode(c2));
        }


    }
}
