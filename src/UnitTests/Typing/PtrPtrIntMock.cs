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
using Reko.UnitTests.Mocks;
using System;

namespace Reko.UnitTests.Typing
{
	public class PtrPtrIntMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			var id1 = Local32("r1");
			var id2 = Local32("r2");
			var id3 = Local32("r3");

			LoadId(id2, id1);
			LoadId(id3, id2);
			MStore(Word32(0x10000), id3);
			MStore(Word32(0x10004), id1);
			Return(id3);
		}
	}
}
