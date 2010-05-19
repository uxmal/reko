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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Types;
using Decompiler.UnitTests.Mocks;
using System;

namespace Decompiler.UnitTests.Mocks
{
	public class ArrayLoopMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier ptr = Local32("ptr");
			Assign(ptr, 0x04000000);
			Label("looptest");
			BranchIf(Uge(ptr, 0x04001000), "done");
			Store(ptr, 0);
			Add(ptr, ptr, 4);
			Jump("looptest");
			Label("done");
			Return();
		}
	}
}
