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
using System;

namespace Reko.Core.Operators
{
    /// <summary>
    /// Models a binary conditional operator for unsigned integers.
    /// </summary>
	public abstract class UnsignedIntOperator : ConditionalOperator
	{
        /// <summary>
        /// Initializes a conditional operator for unsigned integers.
        /// </summary>
        /// <param name="type">Operator type.</param>
        protected UnsignedIntOperator(OperatorType type) : base(type) { }
	}

    /// <summary>
    /// Models the unsigned integer less-than operator.
    /// </summary>
	public class UltOperator : UnsignedIntOperator
	{
        internal UltOperator() : base(OperatorType.Ult) { }

        /// <inheritdoc/>
		public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
			return Constant.Bool(v1 < v2);
		}

        /// <inheritdoc/>
        public override Operator Invert() => Uge;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Ugt;

        /// <inheritdoc/>
        public override string ToString() => " <u ";
    }

    /// <summary>
    /// Models the unsigned integer greater-than operator.
    /// </summary>
    public class UgtOperator : UnsignedIntOperator
	{
        internal UgtOperator() : base(OperatorType.Ugt) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
            return Constant.Bool(v1 <= v2);
		}

        /// <inheritdoc/>
        public override Operator Invert() => Ule;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Ult;

        /// <inheritdoc/>
        public override string ToString() => " >u ";
    }

    /// <summary>
    /// Models the unsigned integer less-than-or-equal operator.
    /// </summary>
    public class UleOperator : UnsignedIntOperator
	{
        internal UleOperator() : base(OperatorType.Ule) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
            return Constant.Bool(v1 <= v2);
		}

        /// <inheritdoc/>
        public override Operator Invert() => Ugt;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Uge;

        /// <inheritdoc/>
        public override string ToString() => " <=u ";
    }

    /// <summary>
    /// Models the unsigned integer greater-than-or-equal operator.
    /// </summary>
    public class UgeOperator : UnsignedIntOperator
	{
        internal UgeOperator() : base(OperatorType.Uge) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
			return Constant.Bool(v1 >= v2);
		}

        /// <inheritdoc/>
        public override Operator Invert() => Ult;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Ule;

        /// <inheritdoc/>
        public override string ToString() => " >=u ";
    }
}
