#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
using Reko.Core.Types;
using System;

namespace Reko.Core.Operators
{
	/// <summary>
	/// Unsigned multiplication. 
	/// </summary>
	public class UMulOperator : IMulOperator
	{
        internal UMulOperator() : base(OperatorType.UMul) { }

        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
			try
			{
                //$BUG: we're truncating here. We need a way to pass in the expected result as a first parameter
                // dtResult.
				return Constant.Create(dt, unchecked((long) (c1.ToUInt64() * c2.ToUInt64())));
			}
			catch	//$HACK: sometimes we get -ive numbers here, at which point .NET casts fail; attempt to use signed integers instead.
			{
				return Constant.Create(dt, c1.ToInt64() * c1.ToInt64());
			}
		}

        public override string AsCompound()
        {
            return " *= ";
        }

        public override string ToString()
		{
			return " *u ";
		}
	}
}
