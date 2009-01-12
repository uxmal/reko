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
using Decompiler.UnitTests.Mocks;
using System;

namespace Decompiler.UnitTests.Structure
{
	/// <summary>
	/// Simple mock to exercise Switch-detection. It is structured in the way that typical compiler output looks like: a 
	/// guard statement followed by an indexed indirect jump.
	/// </summary>
	public class MockSwitch : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier r1 = Local32("r1");
			Identifier r2 = Local32("r2");
			BranchIf(Ugt(r1, Int32(4)), "default");

			Switch(r1, "case0", "case1", "case2", "case3");
			Label("case0");
			Assign(r2, Int32(0));
			Jump("done");

			Label("case1");
			Assign(r2, Int32(1));
			Jump("done");

			Label("case2");
			Assign(r2, Int32(4));
			Jump("done");

			Label("case3");
			Assign(r2, Int32(9));
			Jump("done");

			Label("default");
			Assign(r2, Muls(r1, r1));

			Label("done");
			Return(r2);
		}
	}
}
