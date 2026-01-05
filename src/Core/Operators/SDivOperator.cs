#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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

namespace Reko.Core.Operators
{
    /// <summary>
    /// Models the signed integer division operator.
    /// </summary>
	public class SDivOperator : BinaryOperator
	{
        internal SDivOperator() : base(OperatorType.SDiv) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            var denom = c2.ToInt64();
            if (denom == 0)
                return InvalidConstant.Create(c1.DataType);
            return Constant.Create(dt, c1.ToInt64() / denom);
		}

        /// <inheritdoc/>
        public override string AsCompound()
        {
            return " /= ";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
			return " / ";
		}
	}
}
