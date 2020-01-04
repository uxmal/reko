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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Lib
{
    [TestFixture]
    public class Float16Tests
    {
        private string F16(ushort binary16)
        {
            var f16 = new Float16(binary16);
            return f16.ToString("G", CultureInfo.InvariantCulture);
        }

        private string RoundTrip(double d)
        {
            var f16 = new Float16(d);
            return f16.ToString("G", CultureInfo.InvariantCulture);
        }

        [Test]
        public void F16_Zero()
        {
            Assert.AreEqual("0", F16(0));
        }

        [Test]
        public void F16_NegativeZero()
        {
            Assert.AreEqual("0", F16(0x8000));
        }

        [Test]
        public void F16_One()
        {
            Assert.AreEqual("1", F16(0b0_01111_0000000000));
        }

        [Test(Description = "1 + 2−10 =  (next smallest float after 1)")]
        public void F16_OnePlusEpsilon()
        {
            Assert.AreEqual("1.0009765625", F16(0b0_01111_0000000001));
        }

        [Test]
        public void F16_NegativeTwo()
        {
            Assert.AreEqual("-2", F16(0b1_10000_0000000000));
        }

        [Test]
        public void F16_NegativeTen()
        {
            Assert.AreEqual("-10", F16(0b1_10010_0100000000));
        }

        [Test]
        public void F16_MaxValue()
        {
            Assert.AreEqual("65504", F16(0b0_11110_1111111111));
        }

        [Test(Description = "2−14 ≈ 6.10352 × 10−5(minimum positive normal)")]
        public void F16_MinNormal()
        {
            Assert.AreEqual("6.103515625E-05", F16(0b0_00001_0000000000));
        }

        [Test(Description = "2−14 - 2−24 ≈ 6.09756 × 10−5 (maximum subnormal)")]
        [Ignore("Implement if needed")]
        public void F16_MaxSubnormal()
        {
            Assert.AreEqual("6.09E-05", F16(0b0_00000_1111111111));
        }

        [Test(Description = "2−24 ≈ 5.96046 × 10−8 (minimum positive subnormal)")]
        [Ignore("Implement if needed")]
        public void F16_MinSubnormal()
        {
            Assert.AreEqual("@@@", F16(0b0_00000_0000000001));
        }

        [Test]
        public void F16_PositiveInfinity()
        {
            Assert.AreEqual("Infinity", F16(0b0_11111_0000000000));
        }

        [Test]
        public void F16_NegativeInfinity()
        {
            Assert.AreEqual("-Infinity", F16(0b1_11111_0000000000));
        }

        [Test(Description = "0.333251953125 ≈ 1/3")]
        public void F16_OneThird()
        {
            Assert.AreEqual("0.333251953125", F16(0b0_01101_0101010101));
        }

        [Test]
        public void F16_RoundTripZero()
        {
            Assert.AreEqual("0", RoundTrip(0.0));
        }

        [Test]
        public void F16_RoundTripTen()
        {
            Assert.AreEqual("10", RoundTrip(10.0));
        }

    }
}
