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
using System;

namespace Decompiler.UnitTests.Mocks
{
	public class CmpMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier r0 = Register(0);
			Identifier r1 = Register(1);

			Compare("Z", r0, Int32(0));
			Branch(ConditionCode.EQ, "skip");

			Add(r0, r0, r1);
			Label("skip");
			Store(Int32(0x10003000), r0);
			Return();
		}
	}
}
