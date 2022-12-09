#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
using Reko.Core.Collections;
using System.Collections.Generic;

namespace Reko.UnitTests.Core.Collections
{
    [TestFixture]
    public class LookaheadEnumeratorTests
    {
        private bool PeekSucceeds(LookaheadEnumerator<char> e, int ahead, char expected)
        {
            Assert.That(e.TryPeek(ahead, out var result));
            Assert.AreEqual(expected, result);
            return true;
        }

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
            Assert.That(e.TryPeek(0, out var result));
            Assert.AreEqual('a', result);
        }

        [Test]
        public void Peek1()
        {
            var e = new LookaheadEnumerator<char>("ab".GetEnumerator());
            e.MoveNext();
            Assert.That(e.TryPeek(1, out var result));
            Assert.AreEqual('b', result);
            Assert.AreEqual('a', e.Current);
        }

        [Test]
        public void Peek1MoveNext()
        {
            var e = new LookaheadEnumerator<char>("ab".GetEnumerator());
            e.MoveNext();
            Assert.That(e.TryPeek(1, out var result));
            Assert.AreEqual('b', result);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual('b', e.Current);
        }

        [Test]
        public void Peek2ShouldReturnFalse()
        {
            var e = new LookaheadEnumerator<string>(new List<string> { "ab", "bc" }.GetEnumerator());
            e.MoveNext();
            Assert.That(!e.TryPeek(2, out var nonExistent));
        }

        [Test]
        public void PeekThenEnumerate()
        {
            var e = new LookaheadEnumerator<char>("abc".GetEnumerator());
            e.MoveNext();
            Assert.AreEqual('a', e.Current);
            Assert.That(PeekSucceeds(e, 1, 'b'));
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
            Assert.That(PeekSucceeds(e, 2, 'c'));
            Assert.That(e.MoveNext());
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
            Assert.That(PeekSucceeds(e, 1, 'b'));
            Assert.That(PeekSucceeds(e, 1, 'b'));
            Assert.That(PeekSucceeds(e, 1, 'b'));
            Assert.That(PeekSucceeds(e, 1, 'b'));
            Assert.That(PeekSucceeds(e, 2, 'c'));
            Assert.That(PeekSucceeds(e, 2, 'c'));
            Assert.That(PeekSucceeds(e, 3, 'd'));
            Assert.That(PeekSucceeds(e, 3, 'd'));
        }
    }
}
