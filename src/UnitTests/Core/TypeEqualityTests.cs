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
using Reko.Core.Types;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Core
{
	/// <summary>
	/// Type equality tests for Type classes
	/// </summary>
	[TestFixture]
	public class TypeEqualityTests
	{
		[Test]
		public void TeqInts()
		{
			PrimitiveType p1 = PrimitiveType.Create(Domain.SignedInt, 16);
			PrimitiveType p2 = PrimitiveType.Create(Domain.SignedInt, 16);
			Assert.IsTrue(Object.Equals(p1, p2));
		}

		[Test]
		public void TeqNotEqualInts()
		{
			PrimitiveType p1 = PrimitiveType.Int16;
			PrimitiveType p2 = PrimitiveType.Word32;
			Assert.IsFalse(Object.Equals(p1, p2));
		}

		[Test]
		public void TeqHashInts()
		{
			PrimitiveType p1 = PrimitiveType.Word32;
			PrimitiveType p2 = PrimitiveType.Word32;
			Assert.AreEqual(p1.GetHashCode(), p2.GetHashCode());
		}
	}
}
