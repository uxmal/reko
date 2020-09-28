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
    public class DequeueTests
    {
        private Dequeue<int> dq;

        [SetUp]
        public void SetUp()
        {
            dq = new Dequeue<int>(4);
        }
        [Test]
        public void Dequeue_Create()
        {
            Assert.AreEqual(0, dq.Count);
        }

        [Test]
        public void PushBack()
        {
            dq.PushBack(1);
            Assert.AreEqual(1, dq.Count);
        }

        [Test]
        public void PeekBack()
        {
            dq.PushBack(1);
            Assert.AreEqual(1, dq.PeekBack());
        }

        [Test]
        public void PeekBackEmpty()
        {
            try
            {
                dq.PeekBack();
                Assert.Fail("Should have thrown an exception.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [Test]
        public void PushFront()
        {
            dq.PushFront(1);
            Assert.AreEqual(1, dq.Count);
            Assert.AreEqual(1, dq.PopBack());
            Assert.AreEqual(0, dq.Count);
        }

        [Test]
        public void Enumerate()
        {
            dq.PushFront(2);
            dq.PushFront(1);
            dq.PushBack(3);
            dq.PushBack(4);
            IEnumerator<int> e = dq.GetEnumerator();
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(1, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(2, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(3, e.Current);
            Assert.IsTrue(e.MoveNext());
            Assert.AreEqual(4, e.Current);
            Assert.IsFalse(e.MoveNext());
        }
        [Test]
        public void Grow()
        {
            dq.PushBack(1);
            dq.PushBack(2);
            dq.PushBack(3);
            dq.PushBack(4);
            dq.PushBack(5);
            Assert.AreEqual(5, dq.Count);
        }

        [Test]
        public void Clear()
        {
            dq.PushFront(3);
            dq.PushBack(0);
            dq.Clear();
            Assert.AreEqual(0, dq.Count);
        }
    }
}
