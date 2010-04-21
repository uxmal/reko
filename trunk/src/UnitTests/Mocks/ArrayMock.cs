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
	/// <summary>
	/// Mock procedure for reading an array of ints of the form:
	/// i = 0; while (i &lt; 100) { a[i] = 0; ++i }
	/// </summary>
	public class WhileLtIncMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier i = base.Local32("i");
			Identifier a = base.Local32("a");
			Assign(i, 0);
			Jump("loopHdr");
			
			Label("loop");
			Store(Add(a, Mul(i, 4)), 0);
			Add(i, i, 1);
			Label("loopHdr");
			BranchIf(Lt(i, 100), "loop");
			Return();
		}
	}

	/// <summary>
	/// Mock procedure for reading an array of ints of the form:
	/// i = 100; while (i &gt; 0) { --i; a[i] = 0; }
	/// </summary>
	public class WhileGtDecMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier i = base.Local32("i");
			Identifier a = base.Local32("a");
			Assign(i, 100);
			Jump("loopHdr");
			
			Label("loop");
			Sub(i, i, 1);
			Store(Add(a, Mul(i, 4)), 0);
			Label("loopHdr");
			BranchIf(Gt(i, 0), "loop");
			Return();
		}
	}
}
