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
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Numerics;

namespace Reko.Core.Operators
{
    /// <summary>
    /// Models the unsigned division operator.
    /// </summary>
	public class UDivOperator : BinaryOperator
	{
        internal UDivOperator() : base(OperatorType.UDiv) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            if (c2.IsIntegerZero)
                return InvalidConstant.Create(c1.DataType);
            if (c1 is BigConstant bc1)
            {
                // Force operands to be unsigned.
                var left = bc1.Value & Bits.Mask(c1.DataType.BitSize);
                var right = c2.ToBigInteger() & Bits.Mask(c2.DataType.BitSize);
                return BuildConstant(bc1.DataType, c2.DataType, left / right);
            }
            return BuildConstant(c1.DataType, c2.DataType, (long) (c1.ToUInt64() / c2.ToUInt64()));
		}

        /// <inheritdoc/>
        public override string AsCompound()
        {
            return " /= ";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
			return " /u ";
		}

        /// <inheritdoc/>
        public new Constant BuildConstant(DataType dtN, DataType dtD, BigInteger value)
        {
            var dtResult = PrimitiveType.Create(Domain.UnsignedInt, Math.Min(dtN.BitSize, dtD.BitSize));
            return Constant.Create(dtResult, value);
        }
	}
}
