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
	/// Simulates the accessing of a near pointer in a segment from multiple places.
	/// </summary>
	public class SegmentedDoubleReferenceMock : ProcedureMock
	{
		protected override void BuildBody()
		{
			Identifier ds = Local16("ds");
			Constant offset = Int16(0x300);
			Identifier si1 = Local16("si1");
			Identifier si2 = Local16("si2");
			Identifier ax1 = Local16("ax1");
			Identifier ax2 = Local16("ax2");

			Assign(si1, SegMemW(ds, offset));
			Store(SegMemW(ds, Int16(0x100)), SegMemW(ds, Add(si1, 0x0004)));
			Assign(si2, SegMemW(ds, offset));
			Store(SegMemW(ds, Int16(0x102)), SegMemW(ds, Add(si2, 0x0004)));
		}
	}
}
