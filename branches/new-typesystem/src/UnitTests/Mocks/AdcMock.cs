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

using Decompiler.Core;
using Decompiler.Core.Code;
using System;

namespace Decompiler.UnitTests.Mocks
{
	public class AdcMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier r0 = Local32("r0");
			Identifier cf = Flags("C");
			Identifier r1 = Register(1);
			Identifier r2 = Register(2);

			Load(r0, Int32(0x01001000));
			Compare("SZC", r1, Int32(2));
			Assign(cf, Flags("SZC"));
			Add(r0,r0,r0);
			Add(r0,r0,cf);
			Store(Int32(0x01001004), r0);
			Return();
		}
	}
}
