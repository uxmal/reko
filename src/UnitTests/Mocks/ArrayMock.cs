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

using Reko.Core.Expressions;
using System;

namespace Reko.UnitTests.Mocks
{
	/// <summary>
	/// Mock procedure for reading an array of ints of the form:
	/// i = 0; while (i &lt; 100) { a[i] = 0; ++i }
	/// </summary>
	public class WhileLtIncMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var i = base.Local32("i");
			var a = base.Local32("a");
			Assign(i, 0);
			Goto("loopHdr");
			
			Label("loop");
			MStore(IAdd(a, IMul(i, 4)), Word32(0));
			Assign(i, IAdd(i, 1));
			Label("loopHdr");
			BranchIf(Lt(i, 100), "loop");
			Return();
		}
	}

	/// <summary>
	/// Mock procedure for reading an array of ints of the form:
	/// i = 100; while (i &gt; 0) { --i; a[i] = 0; }
	/// </summary>
	public class WhileGtDecMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var i = base.Local32("i");
			var a = base.Local32("a");
			Assign(i, 100);
			Goto("loopHdr");
			
			Label("loop");
			Assign(i, ISubS(i, 1));
			MStore(IAdd(a, IMul(i, 4)), Word32(0));
			Label("loopHdr");
			BranchIf(Gt(i, 0), "loop");
			Return();
		}
	}
}
