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

using Reko.Core.Types;
using Reko.Typing;
using NUnit.Framework;
using System;

namespace Reko.UnitTests.Typing
{
	/// <summary>
	/// Tests for the pattern: (union (ptr T_1) (ptr T_2) ... (ptr T_n)) where T_1.class .. T_n.class are all structures.
	/// </summary>
	[TestFixture]
	public class UnionPointersStructuresMatcherTests
	{
		[Test]
		public void TwoPointers()
		{
			TypeVariable t1 = new TypeVariable(1);
			TypeVariable t2 = new TypeVariable(2);
			EquivalenceClass c1 = new EquivalenceClass(t1);
			EquivalenceClass c2 = new EquivalenceClass(t2);
            c1.DataType = new StructureType { Fields = { { 4, PrimitiveType.Word16 } } };
            c2.DataType = new StructureType { Fields = { { 20, PrimitiveType.Word32 } } };
			t1.Class = c1;
			t2.Class = c2;

			UnionType u = new UnionType(null, null);
			u.Alternatives.Add(new Pointer(c1, 32));
			u.Alternatives.Add(new Pointer(c2, 32));

			UnionPointersStructuresMatcher upsm = new UnionPointersStructuresMatcher();
			Assert.IsTrue(upsm.Match(u));
			Assert.AreEqual(2, upsm.EquivalenceClasses.Count);
			Assert.AreEqual(2, upsm.Structures.Count);
		}

		[Test]
		public void NotMatch()
		{
			TypeVariable t1 = new TypeVariable(1);
			TypeVariable t2 = new TypeVariable(2);
			EquivalenceClass c1 = new EquivalenceClass(t1);
			EquivalenceClass c2 = new EquivalenceClass(t2);
			c1.DataType = new StructureType{ Fields = { { 4, PrimitiveType.Word16 } } };
            c2.DataType = new StructureType { Fields = { { 20, PrimitiveType.Word32 } } };
			t1.Class = c1;
			t2.Class = c2;

			UnionType u = new UnionType(null, null);
			u.Alternatives.Add(new Pointer(c1, 32));
			u.Alternatives.Add(new Pointer(PrimitiveType.Word16, 32));

			UnionPointersStructuresMatcher upsm = new UnionPointersStructuresMatcher();
			Assert.IsFalse(upsm.Match(u));

		}
	}
}
