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
using System;
using System.Numerics;

namespace Reko.Core.Operators
{
    /// <summary>
    /// Integer addition operator.
    /// </summary>
	public class IAddOperator : BinaryOperator
	{
        internal IAddOperator() : base(OperatorType.IAdd) { }

        /// <inheritdoc/>
		public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(dt);
            var d1 = c1.DataType.Domain;
            var d2 = c2.DataType.Domain;
            var bitSize = Math.Max(c1.DataType.BitSize, c2.DataType.BitSize);
            var isPointy1 = (d1 & (Domain.Pointer | Domain.Offset)) != 0;
            var isPointy2 = (d2 & (Domain.Pointer | Domain.Offset)) != 0;

            if (isPointy1 && (c2.DataType.Domain & Domain.Integer) != 0)
            {
                dt = PrimitiveType.Create(d1, c1.DataType.BitSize);
                return Constant.Create(dt, c1.ToUInt64() + c2.ToUInt64());
            }
            else if (isPointy2 && (c1.DataType.Domain & Domain.Integer) != 0)
            {
                dt = PrimitiveType.Create(d2, c2.DataType.BitSize);
                return Constant.Create(dt, c1.ToUInt64() + c2.ToUInt64());
            }
            else
            {
                var dt1 = c1.DataType;
                var dt2 = c2.DataType;
                if ((dt1.Domain & dt2.Domain) == 0 &&
                    ((dt1.Domain | dt2.Domain) != Domain.Integer) &&
                     !(dt1.Domain | dt2.Domain).HasFlag(Domain.Pointer) &&
                     !(dt1.Domain | dt2.Domain).HasFlag(Domain.SegPointer))
                    throw new ArgumentException(string.Format("Can't add types of disjoint domains {0} and {1}", c1.DataType, c2.DataType));
            }
            if (c1.DataType.BitSize <= 64 && c2.DataType.BitSize <= 64)
            {
                return BuildConstant(c1.DataType, c2.DataType, c1.ToInt64() + c2.ToInt64());
            }
            else
            {
                return BuildConstant(c1.DataType, c2.DataType, c1.ToBigInteger() + c2.ToBigInteger());
            }
        }

        /// <inheritdoc/>
        public override string AsCompound()
        {
            return " += ";
        }

        /// <inheritdoc/>
        public override string ToString()
		{
			return " + ";
		}
	}
}
