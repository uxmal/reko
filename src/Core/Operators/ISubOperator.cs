#region License
/* 
 * Copyright (C) 1999-2025 John Källén.
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
    /// Models integer subtraction.
    /// </summary>
	public class ISubOperator : BinaryOperator
	{
        internal ISubOperator() : base(OperatorType.ISub) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            if (c1.DataType.IsPointer && c2.DataType.IsIntegral)
            {
                dt = PrimitiveType.Create(Domain.Pointer, c1.DataType.BitSize);
                return Constant.Create(dt, c1.ToUInt64() - c2.ToUInt64());
            }
            else if (c2.DataType.IsPointer && c1.DataType.IsIntegral)
            {
                dt = PrimitiveType.Create(Domain.Pointer, c2.DataType.BitSize);
                return Constant.Create(dt, c1.ToUInt64() - c2.ToUInt64());
            }
            else if (c1.DataType.BitSize <= 64 && c2.DataType.BitSize <= 64)
            {
                return BuildConstant(c1.DataType, c2.DataType, c1.ToInt64() - c2.ToInt64());
            }
            else
            {
                return BuildConstant(c1.DataType, c2.DataType, c1.ToBigInteger() - c2.ToBigInteger());
            }
		}

        /// <inheritdoc/>
        public override string AsCompound()
        {
            return " -= ";
        }

        /// <inheritdoc/>
        public override string ToString()
		{
			return " - ";
		}
    }

    /// <summary>
    /// Unsigned integer subtraction. Used to model the PowerPC cmpl instruction,
    /// which sets the LT, GT flags based on _unsigned_ comparison.
    /// </summary>
    public class USubOperator : BinaryOperator
    {
        internal USubOperator() : base(OperatorType.USub) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            return BuildConstant(c1.DataType, c2.DataType, (int) (c1.ToUInt64() - c2.ToUInt64()));
        }

        /// <inheritdoc/>
        public override string AsCompound()
        {
            return " -= ";
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return " -u ";
        }
    }
}
