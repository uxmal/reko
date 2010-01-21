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

using Decompiler.Core.Code;
using System;
using Decompiler.UnitTests.Mocks;

namespace Decompiler.UnitTests.Structure
{
	public class MockSnarl : ProcedureMock
	{
		/// <summary>
		/// [0]
		///  |\
		///  | \
		/// [1][2]
		///  | /|
		///  |/ |
		/// [3] |
		///  |\ |
		///  | \|
		///  | [4]
		///  | /
		///  |/
		/// [5]
		/// </summary>
		protected override void BuildBody()
		{
			Identifier a0 = Local32("a0");
			Identifier a2 = Local32("a2");
			Identifier a3 = Local32("a3");

			BranchIf(a0, "b2");
			SideEffect(Fn("foo", a0));
			Jump("b3");
			Label("b2");
			SideEffect(Fn("foo", a2));
			BranchIf(a2, "b4");
			Label("b3");
			SideEffect(Fn("foo", a3));
			BranchIf(a3, "b5");
			Label("b4");
			SideEffect(Fn("foo"));
			Label("b5");
			Return();
		}
	}
}
