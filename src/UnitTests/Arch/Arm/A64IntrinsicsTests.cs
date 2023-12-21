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
using Reko.Core.Intrinsics;
using Reko.Core.Types;
using Reko.UnitTests.Decompiler.Typing;
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

        private ArrayType ai32(int cElem) => new ArrayType(PrimitiveType.Int32, cElem);
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
        private Constant C(ArrayType at, string hexValue)
        {
            var value = BigInteger.Parse(hexValue, System.Globalization.NumberStyles.HexNumber, null);
            return Constant.Create(at, value);
        }

        [Test]
        public void AArch64Intrinsic_abs()
        {
            var c = Simd.Abs.MakeInstance(ai32(4)).ApplyConstants(
                ai32(4),
                C(ai32(4),  "0ED707E230AFFE01EA0CFEC7A8176C450"));
            Assert.AreEqual("0x128F81DD0AFFE01E5F3013867E893BB0<128>", c.ToString());
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

        [Test]
        public void AArch64Intrinsic_addv()
        {
            var c = intrinsics.sum.MakeInstance(aw32(4), PrimitiveType.Word32).ApplyConstants(
                PrimitiveType.Word32,
                C(aw32(4), 0x1, 0x2, 0x3, 0x4));
            Assert.AreEqual("0xA<32>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_smaxv()
        {
            var c = intrinsics.smaxv.MakeInstance(aw32(4), PrimitiveType.Word32).ApplyConstants(
                PrimitiveType.Word32,
                C(aw32(4), 0xFFFF_FFFF, 0x8000_0000, 0x3, 0x4));
            Assert.AreEqual("4<32>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_smin()
        {
            var c = intrinsics.smin.MakeInstance(aw16(4)).ApplyConstants(
                aw16(4),
                C(aw16(4), 0x2233, 0xFFFF, 0xFFFD, 0xAA00),
                C(aw16(4), 0x2236, 0xFFFE, 0x8001, 0x9988));
            Assert.AreEqual("0x2233FFFE80019988<64>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_sminv()
        {
            var c = intrinsics.sminv.MakeInstance(aw32(4), PrimitiveType.Word32).ApplyConstants(
                PrimitiveType.Word32,
                C(aw32(4), 0xFFFF_FFFF, 0x8000_0000, 0x3, 0x4));
            Assert.AreEqual("0x80000000<32>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_umin()
        {
            var c = intrinsics.umin.MakeInstance(aw16(4)).ApplyConstants(
                aw16(4),
                C(aw16(4), 0x2233, 0x4567, 0x6677, 0xAA00),
                C(aw16(4), 0x2236, 0x4455, 0x6777, 0x9988));
            Assert.AreEqual("0x2233445566779988<64>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_uminp()
        {
            var c = intrinsics.uminp.MakeInstance(aw16(4)).ApplyConstants(
                aw16(4),
                C(aw16(4), 0x2233, 0x4567, 0xAA00, 0x6677),
                C(aw16(4), 0x2236, 0x4455, 0x6777, 0x9988));
            Assert.AreEqual("0x2236677722336677<64>", c.ToString());
        }

        [Test]
        public void AArch64Intrinsic_uminv()
        {
            var c = intrinsics.uminv.MakeInstance(aw32(4), PrimitiveType.Word32).ApplyConstants(
                PrimitiveType.Word32,
                C(aw32(4), 0x1, 0xFFFFFFFF, 0x3, 0x4));
            Assert.AreEqual("1<32>", c.ToString());
        }
    }
}
