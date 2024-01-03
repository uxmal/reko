#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Expressions;
using Reko.Core.Intrinsics;
using Reko.Core.Types;
using System;
using System.Numerics;

namespace Reko.UnitTests.Core.Intrinsics
{
    [TestFixture]
    public class SimdIntrinsicTests
    {
        private static BigInteger Repeat(int bitsize, ulong value, int reps)
        {
            var seed = new BigInteger(value);
            var result = BigInteger.Zero;
            for (int i = 0; i < reps; ++i)
            {
                result = (result << bitsize) | seed;
            }
            return result;
        }

        [Test]
        public void Simd_ApplyToLanes()
        {
            var c1 = Constant.Create(PrimitiveType.Word128, new BigInteger(0x332211));
            var c2 = Constant.Create(PrimitiveType.Word128, new BigInteger(0x112233));
            var max = Simd.Max.MakeInstance(new ArrayType(PrimitiveType.Byte, 16));
            var result = max.ApplyConstants(PrimitiveType.Word128, c1, c2);
            Assert.AreEqual("0x332233<128>", result.ToString());
        }

        [Test]
        public void Simd_Abs_8()
        {
            var c = Constant.Create(PrimitiveType.Word256, Repeat(8, 0xF0, 32));
            var abs = Simd.Abs.MakeInstance(new ArrayType(PrimitiveType.Byte, 32));
            var result = abs.ApplyConstants(PrimitiveType.Word256, c);
            Assert.AreEqual("0x0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0F0<256>", c.ToString());
            Assert.AreEqual( "0x1010101010101010101010101010101010101010101010101010101010101010<256>", result.ToString());
        }

        [Test]
        public void Simd_Abs_16()
        {
            var c = Constant.Create(PrimitiveType.Word128, Repeat(32, 0xFFFA_7FFF, 4));
            var abs = Simd.Abs.MakeInstance(new ArrayType(PrimitiveType.Word16, 8));
            var result = abs.ApplyConstants(PrimitiveType.Word128, c);
            Assert.AreEqual("0x0FFFA7FFFFFFA7FFFFFFA7FFFFFFA7FFF<128>", c.ToString());
            Assert.AreEqual(    "0x67FFF00067FFF00067FFF00067FFF<128>", result.ToString());
        }
    }
}
