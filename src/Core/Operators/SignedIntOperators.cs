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
    /// Models a signed integer binary conditional operator.
    /// </summary>
	public abstract class SignedIntOperator : ConditionalOperator
	{
        /// <summary>
        /// Initializes a signed integer conditional operator.
        /// </summary>
        /// <param name="type"></param>
        protected SignedIntOperator(OperatorType type) : base(type) { }
	}

    /// <summary>
    /// Models the signed integer less-than operator.
    /// </summary>
	public class LtOperator : SignedIntOperator
	{
        internal LtOperator() : base(OperatorType.Lt) { }

        /// <inheritdoc/>
		public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            return Constant.Bool(c1.ToInt64() < c2.ToInt64());
		}

        /// <inheritdoc/>
        public override Operator Invert() => Ge;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Gt;

        /// <inheritdoc/>
        public override string ToString() => " < ";
    }

    /// <summary>
    /// Models the signed integer greater-than operator.
    /// </summary>
    public class GtOperator : SignedIntOperator
	{
        internal GtOperator() : base(OperatorType.Gt) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            return Constant.Bool(c1.ToInt64() > c2.ToInt64());
		}

        /// <inheritdoc/>
        public override Operator Invert() => Le;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Lt;

        /// <inheritdoc/>
        public override string ToString() => " > ";
    }

    /// <summary>
    /// Models the signed integer less-than-or-equal-to operator.
    /// </summary>
    public class LeOperator : SignedIntOperator
	{
        internal LeOperator() : base(OperatorType.Le) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            return Constant.Bool(c1.ToInt64() <= c2.ToInt64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Gt;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Ge;

        /// <inheritdoc/>
        public override string ToString() => " <= ";
    }

    /// <summary>
    /// Models the signed integer greater-than-or-equal-to operator.
    /// </summary>
    public class GeOperator : SignedIntOperator
	{
        internal GeOperator() : base(OperatorType.Ge) { }

        /// <inheritdoc/>
        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            return Constant.Bool(c1.ToInt64() >= c2.ToInt64());
        }

        /// <inheritdoc/>
        public override Operator Invert() => Lt;

        /// <inheritdoc/>
        public override BinaryOperator Mirror() => Le;

        /// <inheritdoc/>
        public override string ToString() => " >= ";
    }
}
