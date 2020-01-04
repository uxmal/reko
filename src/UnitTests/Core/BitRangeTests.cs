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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class BitRangeTests
    {
        [Test]
        public void Bitr_Union()
        {
            var a = new BitRange(0, 16);
            var b = new BitRange(0, 8);
            var c = a | b;
            Assert.AreEqual(new BitRange(0, 16), c);
        }

        [Test]
        public void Bitr_Difference_LowOverlap()
        {
            var a = new BitRange(0, 16);
            var b = new BitRange(0, 8);
            var c = a - b;
            Assert.AreEqual(new BitRange(8, 16), c);
        }

        [Test]
        public void Bitr_Difference_HighOverlap()
        {
            var a = new BitRange(0, 16);
            var b = new BitRange(8, 16);
            var c = a - b;
            Assert.AreEqual(new BitRange(0, 8), c);
        }

        [Test]
        public void Bitr_Difference_Contains()
        {
            var a = new BitRange(0, 32);
            var b = new BitRange(8, 16);
            var c = a - b;
            Assert.AreEqual(new BitRange(0, 32), c);
        }

        [Test]
        public void Bitr_Bitmask()
        {
            var a = new BitRange(8, 16);
            Assert.AreEqual(0xFF00, a.BitMask());
        }

        [Test]
        public void Bitr_Bitmask_32bit()
        {
            var a = new BitRange(0, 32);
            Assert.AreEqual(0xFFFFFFFF, a.BitMask());
        }

        [Test]
        public void Bitr_Bitmask_64bit()
        {
            var a = new BitRange(0, 64);
            Assert.AreEqual(~0ul, a.BitMask());
        }

        [Test]
        public void Bitr_Overlaps()
        {
            var a = new BitRange(0, 16);
            var b = new BitRange(15, 32);
            Assert.IsTrue(a.Overlaps(b));
            Assert.IsTrue(b.Overlaps(a));
        }

        [Test]
        public void Bitr_DoesntOverlap()
        {
            var a = new BitRange(0, 16);
            var b = new BitRange(16, 32);
            Assert.IsFalse(a.Overlaps(b));
            Assert.IsFalse(b.Overlaps(a));
        }
    }
}
