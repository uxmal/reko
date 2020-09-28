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

using Reko.Core;
using System;
using NUnit.Framework;

namespace Reko.UnitTests.Core
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        [Category(Categories.UnitTests)]
        public void Addr_ToString()
        {
            Address addr = Address.SegPtr(0xC00, 0x1234);
            Assert.AreEqual("0C00:1234", addr.ToString());
        }

        [Test(Description="Found this in a regression.")]
        [Category(Categories.Regressions)]
        [Category(Categories.UnitTests)]
        public void Addr_Ge()
        {
            Assert.IsTrue(Address.Ptr32(4001) >= Address.Ptr32(4000));
            Assert.IsTrue(Address.Ptr32(4000) >= Address.Ptr32(4000));
            Assert.IsFalse(Address.Ptr32(3999) >= Address.Ptr32(4000));
        }

        [Test]
        public void Addr_Protected()
        {
            Assert.AreEqual(4096, Address.ProtectedSegPtr(0xF, 0).ToLinear());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Addr_AddToSegmented()
        {
            Assert.AreEqual("1C00:0000", (Address.SegPtr(0xC00, 0xF000) + 0x1000).ToString());
            Assert.AreEqual("1C00:1000", (Address.SegPtr(0xC00, 0xF000) + 0x2000).ToString());
        }
    }
}
