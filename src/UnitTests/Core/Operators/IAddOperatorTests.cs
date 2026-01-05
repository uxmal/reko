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
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.Operators
{
    [TestFixture]
    public class IAddOperatorTests
    {
        [Test]
        public void IAdd_int_ptr()
        {
            var c1 = Constant.Create(new Pointer(PrimitiveType.Int32, 32), 0x00120000);
            var c2 = Constant.Create(PrimitiveType.Int32, 0x3400);
            var sum = Operator.IAdd.ApplyConstants(c1.DataType, c1, c2);

            Assert.AreEqual("0x00123400<p32>", sum.ToString());
            Assert.AreEqual("ptr32", sum.DataType.ToString());
        }

        [Test]
        public void IAdd_mptr_int()
        {
            var c1 = Constant.Create(PrimitiveType.Offset16, 0x1200);
            var c2 = Constant.Create(PrimitiveType.Int16, 0x0034);
            var sum = Operator.IAdd.ApplyConstants(c1.DataType, c1, c2);

            Assert.AreEqual("0x1234<p16>", sum.ToString());
            Assert.AreEqual("mp16", sum.DataType.ToString());
        }

        [Test]
        public void IAdd_int_mptr()
        {
            var c1 = Constant.Create(PrimitiveType.Int16, 0x0034);
            var c2 = Constant.Create(PrimitiveType.Offset16, 0x1200);
            var sum = Operator.IAdd.ApplyConstants(c1.DataType, c1, c2);

            Assert.AreEqual("0x1234<p16>", sum.ToString());
            Assert.AreEqual("mp16", sum.DataType.ToString());
        }
    }
}
