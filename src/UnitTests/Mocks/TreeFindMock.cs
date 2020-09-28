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
using System;

namespace Reko.UnitTests.Mocks
{
	/// <summary>
	/// Mock that implicitly defines a recursive data type, tree.
	/// </summary>
	public class TreeFindMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var v = Local32("v");
			var t = Local32("t");
			var vv = Local32("vv");
            Label("l0");
            Assign(Frame.EnsureRegister(Architecture.StackRegister), Frame.FramePointer);
			Label("l0_seek");
			BranchIf(Eq(t, 0), "l5_found"); 
			Assign(vv, Mem(PrimitiveType.Word32, t));
			BranchIf(Eq(v, vv), "l5_found");
			BranchIf(Lt(v, vv), "l4_lt");
			Assign(t, Mem(PrimitiveType.Word32, IAdd(t, 8)));
			Goto("l0_seek");

			Label("l4_lt");
			Assign(t, Mem(PrimitiveType.Word32, IAdd(t, 4)));

			Label("l5_found");
			Return(t);
		}
	}
}
