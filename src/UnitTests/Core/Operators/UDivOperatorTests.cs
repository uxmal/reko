#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
using Reko.Core.Operators;
using Reko.Core.Types;
using System.Linq;
using System.Numerics;

namespace Reko.UnitTests.Core.Operators
{
    [TestFixture]
    public class UDivOperatorTests
    {
        [Test]
        public void UDiv_BigInteger()
        {
            var big = new BigInteger(0xFFFF_FFFF_FFFF_FFFF);
            big = big * 1000;
            var numerator = new BigConstant(PrimitiveType.Int128, big);
            var denominator = Constant.UInt64(1000);
            var result = Operator.UDiv.ApplyConstants(denominator.DataType, numerator, denominator);
            Assert.AreEqual("0xFFFFFFFFFFFFFFFF<u64>", result.ToString());
            Assert.IsInstanceOf<Constant>(result);
        }

        [Test]
        public void UDiv_BigInteger_SmallResult()
        {
            var big = new BigInteger(Enumerable.Range(0, 16).Select(_ => (byte) 0xFF).ToArray());
            var numerator = new BigConstant(PrimitiveType.Int128, big);
            var denominator = Constant.UInt64(3);
            var result = Operator.UDiv.ApplyConstants(denominator.DataType, numerator, denominator);
            Assert.AreEqual("0x5555555555555555<u64>", result.ToString());
            Assert.IsInstanceOf<Constant>(result);

        }
    }
}
