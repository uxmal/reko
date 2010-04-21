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

namespace Decompiler.UnitTests.Mocks
{
	public class FactorialMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier n = Local32("r2");
			Identifier acc = Local32("r3");
			Identifier z = Flags("Z");
			Identifier r1 = Local32("r1");
				
			Assign(acc, Int32(1));

			Label("test");
			Compare("Z", n, Int32(0));
			Branch(ConditionCode.EQ, "done");

			Assign(acc, Muls(acc, n));
			Sub(n, n, Int32(1));
			Jump("test");

			Label("done");
			Return(acc);
		}
	}

	public class FactorialCallerMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier sixBang = Local32("r1");
			Identifier stg = Local32("stg");
			Assign(sixBang, Fn("FactorialMock", Int32(6)));
			Store(stg, sixBang);
		}
	}
}