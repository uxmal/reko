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

using Decompiler.Core.Code;
using Decompiler.UnitTests.Mocks;
using System;

namespace Decompiler.UnitTests.Structure
{
	/// <summary>
	/// A sequence of nested if-thens that should decompile to a switch-like
	/// statement.
	/// </summary>
	public class MockIfElseIf : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier r = Local32("r");
			Identifier x = Local32("x");

			BranchIf(Ne(r, 0), "not_0");
			Assign(x, 0);
			Jump("done");

			Label("not_0");
			BranchIf(Ne(r, 1), "not_1");
			Assign(x, 1);
			Jump("done");

			Label("not_1");
			BranchIf(Ne(r, 2), "not_2");
			Assign(x, 2);
			Jump("done");

			Label("not_2");
			BranchIf(Ne(r, 3), "default");
			Assign(x, 3);
			Jump("done");

			Label("default");
			Assign(x, -1);

			Label("done");
			Return(x);
		}

	}
}
