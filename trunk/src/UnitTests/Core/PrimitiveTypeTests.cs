/* 
 * Copyright (C) 1999-2007 John Källén.
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

using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class PrimitiveTypeTests
	{
		[Test]
		public void PredefinedTypes()
		{
			Assert.AreEqual("bool", PrimitiveType.Bool.ToString());
			Assert.AreEqual("byte", PrimitiveType.Byte.ToString());
			Assert.AreEqual("byte", PrimitiveType.Char.ToString());
			Assert.AreEqual("sbyte", PrimitiveType.SByte.ToString());
			Assert.AreEqual("word16", PrimitiveType.Word16.ToString());
			Assert.AreEqual("int16", PrimitiveType.Int16.ToString());
			Assert.AreEqual("uint16", PrimitiveType.UInt16.ToString());
			Assert.AreEqual("word32", PrimitiveType.Word32.ToString());
			Assert.AreEqual("int32", PrimitiveType.Int32.ToString());
			Assert.AreEqual("uint32", PrimitiveType.UInt32.ToString());
			Assert.AreEqual("real32", PrimitiveType.Real32.ToString());
			Assert.AreEqual("real64", PrimitiveType.Real64.ToString());
		}

		[Test]
		public void GetSignedEquivalent()
		{
			Assert.AreEqual("int16", PrimitiveType.Word16.GetSignedEquivalent().ToString());
			Assert.AreEqual("int16", PrimitiveType.UInt16.GetSignedEquivalent().ToString());
		}

		[Test]
		public void GetUnsignedEquivalent()
		{
			Assert.AreEqual("uint32", PrimitiveType.Word32.GetUnsignedEquivalent().ToString());
		}

	}
}
