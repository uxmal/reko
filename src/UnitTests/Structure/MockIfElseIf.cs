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

using Reko.UnitTests.Mocks;
using System;

namespace Reko.UnitTests.Structure
{
	/// <summary>
	/// A sequence of nested if-thens that should decompile to a switch-like
	/// statement.
	/// </summary>
	public class MockIfElseIf : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var r = Local32("r");
			var x = Local32("x");

			BranchIf(Ne(r, 0), "not_0");
			Assign(x, 0);
			Goto("done");

			Label("not_0");
			BranchIf(Ne(r, 1), "not_1");
			Assign(x, 1);
			Goto("done");

			Label("not_1");
			BranchIf(Ne(r, 2), "not_2");
			Assign(x, 2);
			Goto("done");

			Label("not_2");
			BranchIf(Ne(r, 3), "default");
			Assign(x, 3);
			Goto("done");

			Label("default");
			Assign(x, -1);

			Label("done");
			Return(x);
		}

	}
}
