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

using Decompiler.Core;
using System;
using NUnit.Framework;

namespace Decompiler.UnitTests.Core
{
    [TestFixture]
    public class AddressTests
    {
        [Test]
        public void Addr_ToString()
        {
            Address addr = Address.SegPtr(0xC00, 0x1234);
            string str = addr.ToString();
            Assert.AreEqual("0C00:1234", addr.ToString());
        }

        [Test(Description="Found this in a regression.")]
        [Category("Regressions")]
        public void Addr_Ge()
        {
            Assert.IsTrue(new Address(4001) >= new Address(4000));
            Assert.IsTrue(new Address(4000) >= new Address(4000));
            Assert.IsFalse(new Address(3999) >= new Address(4000));
        }
    }
}
