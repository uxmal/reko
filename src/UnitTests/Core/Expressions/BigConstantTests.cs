#region License
/* 
 * Copyright (C) 1999-2021 John Källén.
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
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Expressions
{
    [TestFixture]
    public class BigConstantTests
    {
        [Test]
        public void Bc_Unsigned()
        {
            var c = BigConstant.CreateUnsigned(PrimitiveType.UInt16, -1);
            Assert.AreEqual(new BigInteger(0xFFFF), c.Value);
        }

        [Test]
        public void Bc_Slice()
        {
            var n = new BigInteger(-1) & ~0xFF; // mask out low byte
            Console.WriteLine(n);
            var c = BigConstant.CreateUnsigned(PrimitiveType.Word256, n);
            Console.WriteLine(c);
            var slice = c.Slice(PrimitiveType.Int32, 32);
            Assert.AreEqual(-1, slice.GetValue());
        }

        [Test]
        public void Bc_BigSlice()
        {
            var n = new BigInteger(-1) & ~0xFF; // mask out low byte
            var c = BigConstant.CreateUnsigned(PrimitiveType.Word256, n);
            var slice = c.Slice(PrimitiveType.Word128, 32);
            Assert.AreSame(PrimitiveType.Word128, slice.DataType);
            Assert.AreEqual("0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF<128>", slice.ToString());
        }

        [Test]
        public void Bc_Complement()
        {
            var n = new BigConstant(PrimitiveType.Word128, 0x3FF);
            var c = n.Complement();
            Assert.AreEqual("0x0FFFFFFFFFFFFFFFFFFFFFFFFFFFFFC00<128>", c.ToString());
        }
    }
}
