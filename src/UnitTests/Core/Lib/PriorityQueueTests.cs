#region License
/* Copyright (C) 1999-2020 John Källén.
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
    public class PriorityQueueTests
    {
        [Test]
        public void AddItem()
        {
            var pq = new PriorityQueue<string>();
            pq.Enqueue(1, "foo");
            Assert.AreEqual(1, pq.Count);
        }

        [Test]
        public void EnqDeq()
        {
            var pq = new PriorityQueue<string>();
            pq.Enqueue(1, "world");
            pq.Enqueue(2, "hello");
            Assert.AreEqual("hello", pq.Dequeue());
            Assert.AreEqual("world", pq.Dequeue());
        }


    }
}
