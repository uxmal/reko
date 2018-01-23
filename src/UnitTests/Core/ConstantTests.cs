#region License
/* 
 * Copyright (C) 1999-2018 John Källén.
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
		public void Negate()
		{
			Constant c1 = Constant.Word16(0xFFFF);
            Constant c2 = c1.Negate();
			Assert.AreEqual(1, c2.ToInt32(), "-(-1) == 1");
            Assert.AreSame(PrimitiveType.Int16, c2.DataType);
		}

		[Test]
		public void ToInt32()
		{
			Constant c2 = Constant.Create(PrimitiveType.Word16, 0xFFFF);
			Assert.AreEqual(-1, c2.ToInt32());
            Constant c3 = Constant.Create(PrimitiveType.Word32, 0xFFFFFFFF);
			Assert.AreEqual(-1, c3.ToInt32());
		}

		[Test]
		public void SignExtendSignedByte()
		{
			Constant c = Constant.SByte((sbyte)-2);
			Assert.AreEqual(-2, c.ToInt32());
		}

        [Test]
        public void ConstantByte_ToInt64()
        {
            Constant c2 = Constant.Byte(0xFF);
            object o = c2.ToInt64();
            Assert.AreSame(typeof(long), o.GetType());
            Assert.AreEqual(-1L, o);
        }
	}
}
