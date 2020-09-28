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

using Reko.Core;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;

namespace Reko.UnitTests.Mocks
{
	public class ArrayLoopMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var ptr = Local32("ptr");
            Assign(Frame.EnsureRegister(Architecture.StackRegister), Frame.FramePointer);
			Assign(ptr, 0x04000000);
			Label("looptest");
			BranchIf(Uge(ptr, 0x04001000), "done");
			MStore(ptr, Word32(0));
			Assign(ptr, IAdd(ptr, 4));
			Goto("looptest");
			Label("done");
			Return();
		}
	}
}
