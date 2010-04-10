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
using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using Decompiler.Typing;
using NUnit.Framework;
using System;

namespace Decompiler.UnitTests.Typing
{
	[TestFixture]
	public class TypeVariableReplacerTests
	{
		[Test]
		public void TvrReplaceInStruct()
		{
			TypeFactory factory = new TypeFactory();
			TypeStore store = new TypeStore();
			EquivalenceClassBuilder eqb = new EquivalenceClassBuilder(factory, store);

			Identifier pptr = new Identifier("pptr", 0, PrimitiveType.Word32, null);
			Identifier ptr = new Identifier("ptr", 0, PrimitiveType.Word32, null);
		}

		private MemoryAccess MemLoad(Identifier id, int offset, PrimitiveType size)
		{
			return new MemoryAccess(MemoryIdentifier.GlobalMemory,
				new BinaryExpression(Operator.Add, PrimitiveType.Int32, id, Constant.Word32(offset)),
				size);
		}

	}
}
