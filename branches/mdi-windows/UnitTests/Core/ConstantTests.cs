/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
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
				Constant c1 = new Constant(1.0);
				Constant c2 = new Constant(-2.0F);
				Constant c3 = new Constant(PrimitiveType.Word16, 4);
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
			Constant c1 = new Constant(PrimitiveType.Word16, 0xFFFF);
            Constant c2 = c1.Negate();
			Assert.AreEqual(1, c2.ToInt32(), "-(-1) == 1");
            Assert.AreSame(PrimitiveType.Int16, c2.DataType);
		}

		[Test]
		public void ToInt32()
		{
			Constant c2 = new Constant(PrimitiveType.Word16, 0xFFFF);
			Assert.AreEqual(-1, c2.ToInt32());
			Constant c3 = new Constant(PrimitiveType.Word32, 0xFFFFFFFF);
			Assert.AreEqual(-1, c3.ToInt32());
		}

		[Test]
		public void SignExtendSignedByte()
		{
			Constant c = new Constant(PrimitiveType.SByte, (sbyte)-2);
			Assert.AreEqual(-2, c.ToInt32());
		}
	}
}
