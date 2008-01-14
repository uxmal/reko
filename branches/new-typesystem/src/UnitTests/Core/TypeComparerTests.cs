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
using Decompiler.Core.Types;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Core
{
	[TestFixture]
	public class TypeComparerTests
	{
		[Test]
		public void CmpPtrIntPtrInt()
		{
			Pointer p1 = new Pointer(PrimitiveType.Int32, 4);
			Pointer p2 = new Pointer(PrimitiveType.Int32, 4);
			DataTypeComparer c = new DataTypeComparer();
			Assert.AreEqual(0, c.Compare(p1, p2));
		}
	}
}
