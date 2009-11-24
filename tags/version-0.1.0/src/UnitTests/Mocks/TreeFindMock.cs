/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.UnitTests.Mocks
{
	/// <summary>
	/// Mock that implicitly defines a recursive data type, tree.
	/// </summary>
	public class TreeFindMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier v = Local32("v");
			Identifier t = Local32("t");
			Identifier vv = Local32("vv");

			Label("seek");
			BranchIf(Eq(t, 0), "found"); 
			Assign(vv, Load(PrimitiveType.Word32, t));
			BranchIf(Eq(v, vv), "found");
			BranchIf(Lt(v, vv), "lt");
			Assign(t, Load(PrimitiveType.Word32, Add(t, 8)));
			Jump("seek");

			Label("lt");
			Assign(t, Load(PrimitiveType.Word32, Add(t, 4)));

			Label("found");
			Return(t);
		}
	}
}
