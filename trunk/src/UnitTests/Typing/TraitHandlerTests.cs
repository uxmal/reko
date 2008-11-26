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
using Decompiler.Typing;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TraitHandlerTests
	{
		[Test]
		public void TrhInterfaceMethods()
		{
			TypeVariable tv = new TypeVariable(1);
			TypeVariable tv2 = new TypeVariable(2);
			TypeVariable tv3 = new TypeVariable(3);

			ITraitHandler trh = new MockTraitHandler();
			trh.FunctionTrait(tv, 4, null);
			trh.DataTypeTrait(tv, PrimitiveType.Real64);
			trh.DataTypeTrait(tv3, new UnknownType());
			trh.EqualTrait(tv, tv2);
			trh.MemAccessTrait(null, tv, 4, tv2, 4);
			trh.MemSizeTrait(null, tv, 44);
			trh.ArrayTrait(tv, 30, 6);
		}

		[Test]
		public void TrhSimple()
		{
		}


	}



}
