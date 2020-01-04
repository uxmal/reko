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
    public class BitsTests
    {
		[Test]
		public void Bits_BitCount()
        {
            Assert.AreEqual(63, Bits.BitCount(~1ul));
            Assert.AreEqual(0, Bits.BitCount(0));
            Assert.AreEqual(2, Bits.BitCount(9));
            Assert.AreEqual(2, Bits.BitCount(12));
            Assert.AreEqual(3, Bits.BitCount(14));
            Assert.AreEqual(7, Bits.BitCount(254));
        }

        [Test]
        public void Bits_PowersOfTwo()
        {
            Assert.IsTrue(Bits.IsEvenPowerOfTwo(2), "2 is power of two");
            Assert.IsTrue(Bits.IsEvenPowerOfTwo(4), "4 is power of two");
            Assert.IsTrue(Bits.IsEvenPowerOfTwo(8), "8 is power of two");
            Assert.IsTrue(Bits.IsEvenPowerOfTwo(16), "16 is power of two");
            Assert.IsTrue(Bits.IsEvenPowerOfTwo(256), "256 is power of two");
            Assert.IsFalse(Bits.IsEvenPowerOfTwo(3), "3 isn't power of two");
            Assert.IsFalse(Bits.IsEvenPowerOfTwo(7), "7 isn't power of two");
            Assert.IsFalse(Bits.IsEvenPowerOfTwo(127), "127 isn't power of two");
        }

        [Test]
        public void Bits_RotateR_8bit()
        {
            Assert.AreEqual(0x12, Bits.RotateR(8, 0x12, 0));
            Assert.AreEqual(0x09, Bits.RotateR(8, 0x12, 1));
            Assert.AreEqual(0x84, Bits.RotateR(8, 0x12, 2));
        }

        [Test]
        public void Bits_CountLeadingZeros_0()
        {
            Assert.AreEqual(64, Bits.CountLeadingZeros(64, 0));
        }

        [Test]
        public void Bits_CountLeadingZeros_1()
        {
            Assert.AreEqual(63, Bits.CountLeadingZeros(64, 1));
        }

        [Test]
        public void Bits_CountLeadingZeros_1_16bitWord()
        {
            Assert.AreEqual(15, Bits.CountLeadingZeros(16, 1));
        }

        [Test]
        public void Bits_CountLeadingZeros_0x7F_7bitWord()
        {
            Assert.AreEqual(0, Bits.CountLeadingZeros(7, 0x7F));
        }

        [Test]
        public void Bits_Replicate_1_bit_pattern()
        {
            Assert.AreEqual(0b11111ul, Bits.Replicate64(3, 1, 5));
        }
    }
}
