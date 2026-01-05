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
using System.Numerics;

namespace Reko.Core.Operators
{
    /// <summary>
    /// Models a binary conditional operator.
    /// </summary>
    public abstract class ConditionalOperator : BinaryOperator
    {
        /// <summary>
        /// Initializes a conditional operator.
        /// </summary>
        /// <param name="type">Operator type.</param>
        protected ConditionalOperator(OperatorType type) : base(type) { }

        /// <summary>
        /// Mirroring a conditional operator "flips" it around 0. Eg. 
        /// negating LT generates GT. This is not the
        /// same as Inverting it, which changes LT to GE.
        /// </summary>
        public abstract BinaryOperator Mirror();

        /// <summary>
        /// Converts a signed conditional operator to an unsigned one.
        /// </summary>
        /// <returns>
        /// If the operator is already unsigned, it is returned as-is.
        /// </returns>
        public ConditionalOperator ToUnsigned()
        {
            switch (base.Type)
            {
            case OperatorType.Eq:
            case OperatorType.Ne:
            case OperatorType.Ult:
            case OperatorType.Ule:
            case OperatorType.Uge:
            case OperatorType.Ugt:
                return this;
            case OperatorType.Lt:
                return Ult;
            case OperatorType.Le:
                return Ule;
            case OperatorType.Ge:
                return Uge;
            case OperatorType.Gt:
                return Ugt;
            }
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Models a short-circuiting AND operator.
    /// </summary>
    public class CandOperator : BinaryOperator
	{
        internal CandOperator() : base(OperatorType.Cand) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (c1.IsZero)
                return c1;
            return c2;
        }

        /// <inheritdoc/>
		public override string ToString()
		{
			return " && ";
		}
	}

    /// <summary>
    /// Models a short-circuiting OR operator.
    /// </summary>
	public class CorOperator : BinaryOperator
	{
        internal CorOperator() : base(OperatorType.Cor) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!c1.IsZero)
                return c1;
            return c2;
		}

        /// <inheritdoc/>
		public override string ToString()
		{
			return " || ";
		}
	}

    /// <summary>
    /// Models the equality operator.
    /// </summary>
	public class EqOperator : ConditionalOperator
	{
        internal EqOperator() : base(OperatorType.Eq) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return c1.ToInt64() == c2.ToInt64() ? Constant.True() : Constant.False();
		}

        /// <inheritdoc/>
        public override Operator Invert() => Ne;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Eq;

        /// <inheritdoc/>
        public override string ToString() => " == ";
    }

    /// <summary>
    /// Models the inequality operator.
    /// </summary>
	public class NeOperator : ConditionalOperator
	{
        internal NeOperator() : base(OperatorType.Ne) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(dt);
            return c1.ToInt64() != c2.ToInt64()
				? Constant.True() 
				: Constant.False();
		}

        /// <inheritdoc/>
        public override Operator Invert() => Eq;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Ne;

        /// <inheritdoc/>
        public override string ToString() => " != ";
    }
}
