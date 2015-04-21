#region License
/* 
 * Copyright (C) 1999-2015 John Källén.
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

using Decompiler.Core.Serialization;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Decompiler.UnitTests.Core.Serialization
{
    [TestFixture]
    public class DataTypeSerializerTests
    {
        [Test]
        public void DTS_Primitive_Int32()
        {
            var pt = PrimitiveType.Int32.Accept(new DataTypeSerializer());
            Assert.AreEqual("prim(SignedInt,4)", pt.ToString());
        }

        [Test]
        public void DTS_ptr_foo()
        {
            var pt = new Pointer(PrimitiveType.Int32, 4).Accept(new DataTypeSerializer());
            Assert.AreEqual("ptr(prim(SignedInt,4))", pt.ToString());
        }

        [Test]
        public void DTS_array_ptr_code()
        {
            var pt = new ArrayType(new Pointer(new CodeType(), 4), 3).Accept(new DataTypeSerializer());
            Assert.AreEqual("arr(ptr(code),3)", pt.ToString());
        }
    }
}
