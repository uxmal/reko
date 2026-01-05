#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Operators
{
    [TestFixture]
    public class IModOperatorTests
    {
        [Test]
        public void IMod_BigInteger()
        {
            var big = new BigInteger(0x7FFF_FFFF_FFFF_FFFF);
            big = big * big;
            var numerator = new BigConstant(PrimitiveType.Int128, big);
            var denominator = Constant.Int64(10);
            var result = Operator.IMod.ApplyConstants(denominator.DataType, numerator, denominator);
            Assert.AreEqual("9<i64>", result.ToString());
            Assert.IsInstanceOf<Constant>(result);
        }
    }
}
