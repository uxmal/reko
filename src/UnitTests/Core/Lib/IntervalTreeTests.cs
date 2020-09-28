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
using Reko.Core.Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class IntervalTreeTests
    {
        [Test]
        public void IntTree_Add()
        {
            var tree = new IntervalTree<int, string>
            {
                { 0, 4, "Hello" },
                { 4, 8, "World" }
            };
            Assert.AreEqual("[[0, 4], Hello];[[4, 8], World]", string.Join(";", tree));
        }

        [Test]
        public void IntTree_AddOverlapped()
        {
            var tree = new IntervalTree<int, string>
            {
                { 0, 2, "loword" },
                { 2, 4, "hiword" }
            };
            var q = tree.GetIntervalsOverlappingWith(Interval.Create(1, 3));
            Assert.AreEqual("[[0, 2], loword];[[2, 4], hiword]", string.Join(";", q));
        }

        [Test]
        public void IntTree_DeleteOverlap()
        {
            var tree = new IntervalTree<int, string>
            {
                { 0, 2, "loword" },
                { 1, 3, "deleteme" },
                { 2, 4, "hiword" }
            };
            Assert.AreEqual(3, tree.Count);
            var b = tree.Delete(Interval.Create(1, 3));
            Assert.AreEqual("[[0, 2], loword];[[2, 4], hiword]", string.Join(";", tree));
        }

        [Test]
        public void IntTree_Interval_Covers()
        {
            var a = Interval.Create(-2, 2);
            Assert.IsFalse(a.Covers(Interval.Create(-4, -2)));
            Assert.IsFalse(a.Covers(Interval.Create(-4, -1)));
            Assert.IsTrue(a.Covers(Interval.Create(-1, 1)));
            Assert.IsTrue(a.Covers(Interval.Create(-2, 2)));
            Assert.IsFalse(a.Covers(Interval.Create(-2, 3)));
        }
    }
}
