#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
using Reko.Arch.Arm.AArch64;
using Reko.Core.Expressions;
using Reko.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Reko.UnitTests.Arch.Arm
{
    public class A64IntrinsicsTests
    {
        private readonly Intrinsics intrinsics;

        public A64IntrinsicsTests()
        {
            this.intrinsics = new Intrinsics();
        }

        private ArrayType aw16(int cElem) => new ArrayType(PrimitiveType.Word16, cElem);

        private ArrayType aw32(int cElem) => new ArrayType(PrimitiveType.Word32, cElem);
        private Constant C(ArrayType at, params uint[] values)
        {
            var value = BigInteger.Zero;
            for (int i = 0; i < values.Length; ++i)
            {
                value <<= at.ElementType.BitSize;
                value |= values[i];
            }
            return Constant.Create(at, value);
        }

        [Test]
        public void AArch64Intrinsic_addhn()
        {
            var c = intrinsics.addhn.MakeInstance(aw32(4), aw16(4)).ApplyConstants(
                aw16(4),
                C(aw32(4), 0x01020304, 0x808080, 0x102030), 
                C(aw32(4), 0x10203040, 0x80F0FF, 0xEFE2E3));
            Assert.AreEqual("0x112201010100<64>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_addp_vector()
        {
            var c = intrinsics.addp.MakeInstance(aw32(4)).ApplyConstants(
                aw32(4),
                C(aw32(4), 0x11223344, 0xEEDDCCBC, 0xFACEFACE, 0x05310532),
                C(aw32(4), 0x7EDCBA98, 0x01234568, 0x12345678, 0xEDCBA988));
            Assert.AreEqual("0x080000000000000000000000000000000<128>", c.ToString());
        }
    }
}
