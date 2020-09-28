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
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Expressions
{
    [TestFixture]
    public class ExpressionTreeTests
    {
        private ExpressionEmitter m;
        private int n;

        [SetUp]
        public void Setup()
        {
            this.m = new ExpressionEmitter();
            this.n = 0;
        }

        private Identifier Id(string name)
        {
            ++n;
            return new Identifier(
                name,
                PrimitiveType.Word32,
                new TemporaryStorage(
                    name, 
                    n, 
                    PrimitiveType.Word32));
        }

        private void RunTest(string sExp, Expression e)
        {
            var tree = new ExpressionTree();
            var items = new DfsIterator<Expression>(tree).PostOrder(e);
            Assert.AreEqual(
                sExp,
                string.Join(", ", items));
        }

        [Test]
        public void EtDfs_Id()
        {
            var e = Id("test");

            RunTest("test", e);
        }

        [Test]
        public void EtDfs_Bin()
        {
            var a = Id("a");
            var b = Id("b");

            RunTest("a, b, a + b", m.IAdd(a, b));
        }

        [Test]
        public void EtDfs_NestedBin()
        {
            var a = Id("a");
            var b = Id("b");
            var c = Id("c");

            RunTest("a, b, c, b | c, a | (b | c)", m.Or(a, m.Or(b, c)));
        }
    }
}
