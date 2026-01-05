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
    /// Models a floating point binary conditional operator.
    /// </summary>
	public abstract class RealConditionalOperator : ConditionalOperator
	{
        /// <summary>
        /// Initializes a floating point conditional operator.
        /// </summary>
        /// <param name="type">Type of the operator.</param>
        protected RealConditionalOperator(OperatorType type) : base(type) { }
	}

    /// <summary>
    /// Models the floating point equality operator.
    /// </summary>
    public class ReqOperator : RealConditionalOperator
    {
        internal ReqOperator() : base(OperatorType.Feq) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() == c2.ToReal64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Fne;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Feq;

        /// <inheritdoc/>
        public override string ToString() => " == ";
    }

    /// <summary>
    /// Models the floating point inequality operator.
    /// </summary>
    public class RneOperator : RealConditionalOperator
    {
        internal RneOperator() : base(OperatorType.Fne) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() != c2.ToReal64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Feq;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Fne;

        /// <inheritdoc/>
        public override string ToString() => " != ";
    }


    /// <summary>
    /// Models the floating point less-than operator.
    /// </summary>
    public class RltOperator : RealConditionalOperator
	{
        internal RltOperator() : base(OperatorType.Flt) { }

        /// <inheritdoc/>
		public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return Constant.Bool(c1.ToReal64() < c2.ToReal64());
		}

        /// <inheritdoc/>
        public override Operator Invert() => Fge;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Fgt;

        /// <inheritdoc/>
        public override string ToString() => " < ";
	}


    /// <summary>
    /// Models the floating point greater-than operator.
    /// </summary>
	public class RgtOperator : RealConditionalOperator
	{
        internal RgtOperator() : base(OperatorType.Fgt) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() > c2.ToReal64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Fle;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Flt;

        /// <inheritdoc/>
        public override string ToString() => " > ";
    }

    /// <summary>
    /// Models the floating point less-than-or-equal-to operator.
    /// </summary>
    public class RleOperator : RealConditionalOperator
	{
        internal RleOperator() : base(OperatorType.Fle) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() <= c2.ToReal64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Fgt;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Fge;

        /// <inheritdoc/>
        public override string ToString() => " <= ";
    }

    /// <summary>
    /// Models the floating point greater-than-or-equal-to operator.
    /// </summary>
    public class RgeOperator : RealConditionalOperator
	{
        internal RgeOperator() : base(OperatorType.Fge) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return Constant.Bool(c1.ToReal64() >= c2.ToReal64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Flt;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Fle;

        /// <inheritdoc/>
        public override string ToString() => " >= ";

    }
}
