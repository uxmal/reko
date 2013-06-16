#region License
/* 
 * Copyright (C) 1999-2013 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Expressions;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using System;

namespace Decompiler.UnitTests.Typing
{
	/// <summary>
	/// Mock that unifies a signed integer and a real into the same memory slot.
	/// </summary>
	public class UnionIntRealMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var ptr = new Identifier("ptr", 1, PrimitiveType.Word32, null);
			var i = new Identifier("i", 2, PrimitiveType.Int32, null);
			var r = new Identifier("r", 3, PrimitiveType.Real32, null);
			var x = new Identifier("x", 4, PrimitiveType.Word32, null);

			Assign(x, i);
			Assign(x, r);
			Store(ptr, i);
			Store(IAdd(ptr, Int32(4)), r);
			Return();
		}
	}
}
