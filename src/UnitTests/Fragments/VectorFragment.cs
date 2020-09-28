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

using Reko.Core.Code;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.UnitTests.Mocks;
using System;

namespace Reko.UnitTests.Fragments
{
	public class VectorFragment : ProcedureBuilder
	{
		// Compute the sum of the members of a vector.
		protected override void BuildBody()
		{
			var v = Local32("v");
			var sum = Local32("mod");
			PrimitiveType fl = PrimitiveType.Real32;

			Assign(sum, FAdd(Mem(fl, v),
						FAdd(Mem(fl, IAdd(v, 4)),
							Mem(fl, IAdd(v, 8)))));
		}
	}
}
