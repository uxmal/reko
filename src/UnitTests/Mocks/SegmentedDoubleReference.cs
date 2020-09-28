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
	/// Simulates the accessing of a near pointer in a segment from multiple places.
	/// </summary>
	public class SegmentedDoubleReferenceMock : ProcedureBuilder
	{
		protected override void BuildBody()
		{
			Identifier ds = Local16("ds");
			Constant offset = Word16(0x300);
			Identifier si1 = Local16("si1");
			Identifier si2 = Local16("si2");

            Assign(Frame.EnsureRegister(Architecture.StackRegister), Frame.FramePointer);
            Assign(si1, SegMem16(ds, offset));
			Store(SegMem16(ds, Word16(0x100)), SegMem16(ds, IAdd(si1, 0x0004)));
			Assign(si2, SegMem16(ds, offset));
			Store(SegMem16(ds, Word16(0x102)), SegMem16(ds, IAdd(si2, 0x0004)));
		}
	}
}
