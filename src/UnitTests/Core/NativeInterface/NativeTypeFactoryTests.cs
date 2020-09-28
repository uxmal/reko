#region License
/* 
 * Copyright (C) 1999-2020 Pavel Tomin.
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
using Reko.Core.NativeInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Core.NativeInterface
{
    [TestFixture]
    public class NativeTypeFactoryTests
    {
        [Test]
        public void Ntf_array_i32()
        {
            var ntf = new NativeTypeFactory();
            var a = ntf.ArrayOf((HExpr)BaseType.Int32, 4);
            var arr = ntf.GetRekoType(a);

            Assert.AreEqual("(arr int32 4)", arr.ToString());
        }

        [Test]
        public void Ntf_array_i32_again()
        {
            var ntf = new NativeTypeFactory();
            var a1 = ntf.ArrayOf((HExpr)BaseType.Int32, 4);

            var a2 = ntf.ArrayOf((HExpr)BaseType.Int32, 4);
            Assert.AreEqual(a1, a2, "expected to get the same handle for the same type");
        }
    }
}
