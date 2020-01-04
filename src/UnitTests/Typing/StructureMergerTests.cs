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
using System.Collections;

namespace Reko.UnitTests.Typing
{
	[TestFixture]
	public class StructureMergerTests
	{
		[Test]
		public void Merge1()
		{
			TypeVariable tv1 = new TypeVariable(1);
			TypeVariable tv2 = new TypeVariable(2);
            StructureType s1 = new StructureType { Fields = { { 4, PrimitiveType.Ptr32 } } };
            StructureType s2 = new StructureType { Fields = { { 4, PrimitiveType.Ptr32 } } };
			EquivalenceClass c1 = new EquivalenceClass(tv1);
			EquivalenceClass c2 = new EquivalenceClass(tv2);
			c1.DataType = s1;
			c2.DataType = s2;
			StructureMerger sm = new StructureMerger(new StructureType[] { s1, s2 }, new EquivalenceClass[] { c1, c2 } );
			sm.Merge();
			Assert.AreEqual("(struct (4 ptr32 ptr0004))", c1.DataType.ToString());
		}
	}
}
