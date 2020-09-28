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
using Reko.Core.Expressions;
using Reko.Core.Types;
using NUnit.Framework;
using System;
using Reko.Core.Operators;

namespace Reko.UnitTests.Core
{
	/// <summary>
	/// Tests exercising constants.
	/// </summary>
	[TestFixture]
	public class ConstantTests
	{
		[Test]
		public void ConNegate()
		{
			using (FileUnitTester fut = new FileUnitTester("Core/ConNegate.txt"))
			{
                Constant c1 = Constant.Real64(1.0);
				Constant c2 = Constant.Real32(-2.0F);
				Constant c3 = Constant.Word16(4);
				Constant c4 = Constant.Word32(-8);
				fut.TextWriter.WriteLine(c1);
				fut.TextWriter.WriteLine(c2);
				fut.TextWriter.WriteLine(c3);
				fut.TextWriter.WriteLine(c4);

				fut.AssertFilesEqual();
			}
		}

		[Test]
		public void ConInt32()
		{
			Constant c = Constant.Word32(3);
			Assert.AreEqual(PrimitiveType.Word32, c.DataType);
			Assert.AreEqual(3, c.ToInt32());
		}

		[Test]
		public void ConBooleans()
		{
			Constant t = Constant.True();
			Constant f = Constant.False();
			Assert.AreEqual("true", t.ToString());
			Assert.AreEqual("false", f.ToString());
		}

		[Test]
		public void ConFloatFromBits()
		{
			Constant c = Constant.RealFromBitpattern(PrimitiveType.Real32, 0x3FC00000);
			Assert.AreEqual(1.5F, c.ToFloat());
		}

		[Test]
		public void ConNegate16bit()
		{
			Constant c1 = Constant.Word16(0xFFFF);
            Constant c2 = c1.Negate();
			Assert.AreEqual(1, c2.ToInt32(), "-(-1) == 1");
            Assert.AreSame(PrimitiveType.Int16, c2.DataType);
		}

		[Test]
		public void ConToInt32()
		{
			Constant c2 = Constant.Create(PrimitiveType.Word16, 0xFFFF);
			Assert.AreEqual(-1, c2.ToInt32());
            Constant c3 = Constant.Create(PrimitiveType.Word32, 0xFFFFFFFF);
			Assert.AreEqual(-1, c3.ToInt32());
		}

		[Test]
		public void ConSignExtendSignedByte()
		{
			Constant c = Constant.SByte((sbyte)-2);
			Assert.AreEqual(-2, c.ToInt32());
		}

        [Test]
        public void ConByte_ToInt64()
        {
            Constant c2 = Constant.Byte(0xFF);
            object o = c2.ToInt64();
            Assert.AreSame(typeof(long), o.GetType());
            Assert.AreEqual(-1L, o);
        }

        [Test]
        public void Con_23_bit()
        {
            var dt = PrimitiveType.CreateWord(23);
            var c = Constant.Create(dt, 23);
            Assert.AreEqual("word23", c.DataType.ToString());
        }

        [Test]
        public void Con_Add_5_bit()
        {
            var dt = PrimitiveType.Create(Domain.UnsignedInt,5);
            var c1 = Constant.Create(dt, 16);
            var c2 = Constant.Create(dt, 19);
            var sum = Operator.IAdd.ApplyConstants(c1, c2);
            Assert.AreEqual("0x03", sum.ToString(), "Silent overflow is not working.");
        }

        [Test]
        public void Con_Display_24_bit()
        {
            var dt = PrimitiveType.Create(Domain.UnsignedInt, 24);
            var c = Constant.Create(dt, 42);
            Assert.AreEqual("0x00002A", c.ToString());
        }

        [Test]
        public void Con_Display_64_bit()
        {
            var dt = PrimitiveType.Create(Domain.UnsignedInt, 64);
            var c = Constant.Create(dt, 42);
            Assert.AreEqual("0x000000000000002A", c.ToString());
        }

        [Test]
        public void ConComplement()
        {
            Assert.AreEqual("-128", Constant.SByte(0x7F).Complement().ToString());
            Assert.AreEqual("-32768", Constant.Int16(0x7FFF).Complement().ToString());
            Assert.AreEqual("-2147483648", Constant.Int32(0x7FFFFFFF).Complement().ToString());
            Assert.AreEqual("-9223372036854775808", Constant.Int64(0x7FFFFFFFFFFFFFFF).Complement().ToString());
            Assert.AreEqual("0x80", Constant.Byte(0x7F).Complement().ToString());
            Assert.AreEqual("0x8000", Constant.UInt16(0x7FFF).Complement().ToString());
            Assert.AreEqual("0x80000000", Constant.UInt32(0x7FFFFFFF).Complement().ToString());
            Assert.AreEqual("0x8000000000000000", Constant.UInt64(0x7FFFFFFFFFFFFFFF).Complement().ToString());
        }

    }
}
