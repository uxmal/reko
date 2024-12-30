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
        [Category(Categories.UnitTests)]
        public void Addr_Protected()
        {
            Assert.AreEqual(4096, Address.ProtectedSegPtr(0xF, 0).ToLinear());
        }

        [Test]
        [Category(Categories.UnitTests)]
        public void Addr_AddToSegmented()
        {
            Assert.AreEqual("0C00:0000", (Address.SegPtr(0xC00, 0xF000) + 0x1000).ToString());
            Assert.AreEqual("0C00:1000", (Address.SegPtr(0xC00, 0xF000) + 0x2000).ToString());
        }

        [TestCase(0x0000_0000_0000_0000ul, 0ul)]
        [TestCase(0x3FFF_FFFF_FFFF_FFFFul, 0x3FFF_FFFF_FFFF_FFFFul)]
        [TestCase(0xC000_0000_0000_0000ul, 0xC000_0000_0000_0000ul)]
        [TestCase(0xFFFF_FFFF_FFFF_FFFFul, 0xFFFF_FFFF_FFFF_FFFFul)]
        public void Addr_Ptr64_Offset(ulong offset, ulong expectedOffset)
        {
            var addr = Address.Ptr64(offset);
            Assert.AreEqual(expectedOffset.ToString("X"), addr.Offset.ToString("X"));
        }

        [TestCase(0x0000_0000_0000_0000ul, 0ul)]
        [TestCase(0x3FFF_FFFF_FFFF_FFFFul, 0x3FFF_FFFF_FFFF_FFFFul)]
        [TestCase(0xC000_0000_0000_0000ul, 0xC000_0000_0000_0000ul)]
        [TestCase(0xFFFF_FFFF_FFFF_FFFFul, 0xFFFF_FFFF_FFFF_FFFFul)]
        public void Addr_Ptr64_ToLinear(ulong offset, ulong expectedLinear)
        {
            var addr = Address.Ptr64(offset);
            Assert.AreEqual(expectedLinear, addr.ToLinear());
        }

        [Test]
        public void Addr_Ptr64_Selector()
        {
            var addr = Address.Ptr64(0x123400);
            Assert.IsNull(addr.Selector);
        }

        [TestCase(0x0000_0000_0000_0000ul, true)]
        [TestCase(0x3FFF_FFFF_FFFF_FFFFul, false)]
        public void Addr_Ptr64_IsNull(ulong offset, bool isNull)
        {
            Assert.AreEqual(isNull, Address.Ptr64(offset).IsZero);
        }

        [TestCase(0x3FFF_FFFF_FFFF_FFFFul)]
        [TestCase(0xC000_0000_0000_0000ul)]
        public void Addr_Ptr64_NewOffset(ulong newOffset)
        {
            var addr = Address.Ptr64(0);
            var newAddr = addr.NewOffset(newOffset);
            Assert.AreEqual(newOffset, newAddr.Offset);
        }

        [TestCase(0x707ul, "0000000000000707")]
        public void Addr_Ptr64_ToString(ulong offset, string sExpected)
        {
            var addr = Address.Ptr64(offset);
            var sAddr = addr.ToString();
            Assert.AreEqual(sExpected, sAddr);
        }


        [TestCase(0x0000_0000u, 0u)]
        [TestCase(0x7FFF_FFFFu, 0x7FFF_FFFFu)]
        [TestCase(0x8000_0000u, 0x8000_0000u)]
        [TestCase(0xFFFF_FFFFu, 0xFFFF_FFFFu)]
        public void Addr_Ptr32_Offset(uint offset, uint expectedOffset)
        {
            var addr = Address.Ptr32(offset);
            Assert.AreEqual(expectedOffset, addr.Offset);
        }

        [TestCase(0x0000_0000u, 0u)]
        [TestCase(0x7FFF_FFFFu, 0x7FFF_FFFFu)]
        [TestCase(0x8000_0000u, 0x8000_0000u)]
        [TestCase(0xFFFF_FFFFu, 0xFFFF_FFFFu)]
        public void Addr_Ptr32_ToLinear(uint offset, uint expectedLinear)
        {
            var addr = Address.Ptr32(offset);
            Assert.AreEqual(expectedLinear, addr.ToLinear());
        }

        [Test]
        public void Addr_Ptr32_Selector()
        {
            var addr = Address.Ptr32(0x123400);
            Assert.IsNull(addr.Selector);
        }

        [TestCase(0x0000_0000u, true)]
        [TestCase(0x3FFF_FFFFu, false)]
        public void Addr_Ptr32_IsNull(uint offset, bool isNull)
        {
            Assert.AreEqual(isNull, Address.Ptr64(offset).IsZero);
        }

        [TestCase(0xFFFF_FFFFu)]
        public void Addr_Ptr32_NewOffset(uint newOffset)
        {
            var addr = Address.Ptr64(0);
            var newAddr = addr.NewOffset(newOffset);
            Assert.AreEqual(newOffset, newAddr.Offset);
        }

        [TestCase(0x0000, 0x0000, 0x0000, 0x0000)]
        [TestCase(0x7FFF, 0xFFFF, 0x7FFF, 0xFFFF)]
        [TestCase(0x8000, 0x0000, 0x8000, 0x0000)]
        [TestCase(0xFFFF, 0xFFFF, 0xFFFF, 0xFFFF)]
        public void Addr_ProtSegPtr_Offset(int seg, int offset, int expectedSeg, int expectedOffset)
        {
            var addr = Address.ProtectedSegPtr((ushort)seg, (uint)offset);
            Assert.AreEqual(expectedOffset, addr.Offset);
        }

        [TestCase(0x0000, 0x0000, 0x0000u)]
        [TestCase(0x7FFF, 0xFFFF, 0x100EFFFu)]
        [TestCase(0x8000, 0x0000, 0x1000000u)]
        [TestCase(0xFFFF, 0xFFFF, 0x200EFFFu)]
        public void Addr_ProtSegPtr_ToLinear(int seg, int offset, uint expectedLinear)
        {
            var addr = Address.ProtectedSegPtr((ushort) seg, (ushort) offset);
            Assert.AreEqual(expectedLinear.ToString("X"), addr.ToLinear().ToString("X"));
        }

        [Test]
        public void Addr_ProtSegPtr_Selector()
        {
            var addr = Address.ProtectedSegPtr(0x1234, 0x5678);
            Assert.AreEqual(0x1234, addr.Selector);
        }

        [TestCase(0x0000, 0x0000, true)]
        [TestCase(0x3FFF, 0x0000, false)]
        public void Addr_ProtSegPtr_IsNull(int seg, int offset, bool isNull)
        {
            Assert.AreEqual(isNull, Address.ProtectedSegPtr((ushort)seg, (uint)offset).IsZero);
        }

        [TestCase(0x0000, 0x0000, 0x0000)]
        [TestCase(0x7FFF, 0xFFFF, 0xFFFF)]
        [TestCase(0x8000, 0x0000, 0x0000)]
        [TestCase(0xFFFF, 0xFFFF, 0xFFFF)]
        public void Addr_RealSegPtr_Offset(int seg, int offset, int expectedOffset)
        {
            var addr = Address.SegPtr((ushort)seg, (ushort)offset);
            Assert.AreEqual(expectedOffset, addr.Offset);
        }

        [TestCase(0x0000, 0x0000, 0x00000u)]
        [TestCase(0x1000, 0x2345, 0x12345u)]
        [TestCase(0x7FFF, 0xFFFF, 0x8FFEFu)]
        [TestCase(0x8000, 0x0000, 0x80000u)]
        [TestCase(0xFFFF, 0xFFFF, 0x10FFEFu)]
        public void Addr_RealSegPtr_ToLinear(int seg, int offset, uint expectedLinear)
        {
            var addr = Address.SegPtr((ushort)seg, (ushort)offset);
            Assert.AreEqual(expectedLinear.ToString("X"), addr.ToLinear().ToString("X"));
        }

        [Test]
        public void Addr_RealSegPtr_Selector()
        {
            var addr = Address.ProtectedSegPtr(0x1234, 0x5678);
            Assert.AreEqual(0x1234, addr.Selector);
        }

        [TestCase(0x0000, 0x0000, true)]
        [TestCase(0x3FFF, 0x0000, false)]
        public void Addr_RealSegPtr_IsNull(int seg, int offset, bool isNull)
        {
            Assert.AreEqual(isNull, Address.SegPtr((ushort)seg, (ushort)offset).IsZero);
        }

        [TestCase(0x0017, 0xF000, "0017:F000")]
        public void Addr_RealSegPtr_ToString(int seg, int offset, string sExpected)
        {
            var addr = Address.SegPtr((ushort) seg, (ushort) offset);
            Assert.AreEqual(sExpected, addr.ToString());
        }
    }
}
