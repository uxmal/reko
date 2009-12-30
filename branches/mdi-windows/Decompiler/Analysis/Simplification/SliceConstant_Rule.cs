/* 
 * Copyright (C) 1999-2009 John Källén.
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

namespace Decompiler.Analysis.Simplification
{
	public class SliceConstant_Rule
	{
		private Constant c;
		private Slice slice;

		public bool Match(Slice slice)
		{
			this.slice = slice;
			this.c = slice.Expression as Constant;
			return c != null;
		}

		public Expression Transform(Statement stm)
		{
			return new Constant(slice.DataType, Slice(c.ToInt32()));
		}

		public int Slice(int val)
		{
			int mask = ((1 << slice.DataType.BitSize) - 1) << slice.Offset;
			return (val & mask) >> slice.Offset;
		}
	}
}
