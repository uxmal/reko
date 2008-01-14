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
	public class MockWhileBreak : ProcedureMock
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
			Identifier r1 = Local32("r1");
			Identifier r2 = Local32("r2");
			Identifier r3 = Local32("r3");
			Assign(r2, Int32(0));
			
			Label("looptest");
			BranchIf(Eq(r1, Int32(0)), "done");

			Load(r3, r1);
			Add(r2, r2, r3);
			Load(r3, Add(r1, Int32(4)));
			BranchIf(Eq(r3, Int32(0)), "done");

			Load(r1, Add(r1, Int32(0x00C)));
			Jump("looptest");

			Label("done");
			Return(r2);


		}

	}
}
