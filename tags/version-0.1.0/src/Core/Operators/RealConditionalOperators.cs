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

using Decompiler.Core.Code;
using Decompiler.Core.Types;
using System;

namespace Decompiler.Core.Operators
{
	public abstract class RealConditionalOperator : ConditionalOperator
	{
	}

	public class RltOperator : RealConditionalOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			return c1.ToInt32() < c2.ToInt32()
				? Constant.True() 
				: Constant.False();
		}

		public override string ToString()
		{
			return " < ";
		}
	}

	public class RgtOperator : RealConditionalOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return " > ";
		}
	}

	public class RleOperator : RealConditionalOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return " <= ";
		}
	}

	public class RgeOperator : RealConditionalOperator
	{
		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return " >= ";
		}
	}

}
