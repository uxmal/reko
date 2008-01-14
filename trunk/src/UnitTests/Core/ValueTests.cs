/* 
 * Copyright (C) 1999-2008 John Källén.
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
	[TestFixture]
	public class ValueTests
	{
		[Test]
		public void SignExtend()
		{
			Value v = new Value(PrimitiveType.Byte, 0xFF);
			Assert.AreEqual(-1, v.SignExtend(PrimitiveType.Word32));
		}

		[Test]
		public void SignExtendPositive()
		{
			Value v = new Value(PrimitiveType.Word16, 0xFF);
			Assert.AreEqual(0xFF, v.SignExtend(PrimitiveType.Word32));
		}

		[Test]
		public void SignExtendOverflow()
		{
			Value v = new Value(PrimitiveType.Word16, 0xFFFF);
			Assert.AreEqual(-1, v.SignExtend(PrimitiveType.Word16));
		}

	}
}
