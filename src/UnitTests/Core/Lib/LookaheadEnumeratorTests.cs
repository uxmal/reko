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

using Reko.Core.Lib;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class LookaheadEnumeratorTests
    {
        [Test]
        public void EmptySequence()
        {
            var e = new LookaheadEnumerator<char>("".GetEnumerator());
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void GetOne()
        {
            var e = new LookaheadEnumerator<char>("a".GetEnumerator());
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual('a', e.Current);
        }

        [Test]
        public void HitTheEnd()
        {
            var e = new LookaheadEnumerator<char>("a".GetEnumerator());
            Assert.IsTrue(e.MoveNext());
            Assert.IsFalse(e.MoveNext());
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void Peek0()
        {
            var e = new LookaheadEnumerator<char>("ab".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('a', e.Peek(0));
        }

        [Test]
        public void Peek1()
        {
            var e = new LookaheadEnumerator<char>("ab".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('b', e.Peek(1));
            Assert.AreEqual('a', e.Current);
        }

        [Test]
        public void Peek1MoveNext()
        {
            var e = new LookaheadEnumerator<char>("ab".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('b', e.Peek(1));
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual('b', e.Current);
        }

        [Test]
        public void Peek2ShouldThrow()
        {
            var e = new LookaheadEnumerator<char>("ab".GetEnumerator());
            e.MoveNext();
            try
            {
                e.Peek(2);
                Assert.Fail();
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public void PeekThenEnumerate()
        {
            var e = new LookaheadEnumerator<char>("abc".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('a', e.Current);
            Assert.AreEqual('b', e.Peek(1));
            e.MoveNext();
            Assert.AreEqual('b', e.Current);
            e.MoveNext();
            Assert.AreEqual('c', e.Current);
        }

        [Test]
        public void PeekThenMoveToEnd()
        {
            var e = new LookaheadEnumerator<char>("abc".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('c', e.Peek(2));
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual('b', e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual('c', e.Current);
            Assert.IsFalse(e.MoveNext());
        }

        [Test]
        public void LAET_Regression1()
        {
            var e = new LookaheadEnumerator<char>("abcd".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('b', e.Peek(1));
            Assert.AreEqual('b', e.Peek(1));
            Assert.AreEqual('b', e.Peek(1));
            Assert.AreEqual('b', e.Peek(1));
            Assert.AreEqual('c', e.Peek(2));
            Assert.AreEqual('c', e.Peek(2));
            Assert.AreEqual('d', e.Peek(3));
            Assert.AreEqual('d', e.Peek(3));
        }
    }
}
