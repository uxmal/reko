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
	public class MockWhileBreak : ProcedureBuilder
	{
		/// <summary>
		/// r2 = 0;
		/// while (r1 != 0)
		/// {
		///		r2 = r2 + r1->w0000;
		///		if (r1->w0004 == 0)
		///			break;
		///		r1 = r1->w000C;		
		///	}
		///	return r2;
		/// </summary>
		protected override void BuildBody()
		{
			var r1 = Local32("r1");
			var r2 = Local32("r2");
			var r3 = Local32("r3");
			Assign(r2, Int32(0));
			
			Label("looptest");
			BranchIf(Eq(r1, Int32(0)), "done");

			LoadId(r3, r1);
			Assign(r2, IAdd(r2, r3));
			LoadId(r3, IAddS(r1, 4));
			BranchIf(Eq(r3, Int32(0)), "done");

			LoadId(r1, IAddS(r1, 0x00C));
			Goto("looptest");

			Label("done");
			Return(r2);


		}

	}
}
