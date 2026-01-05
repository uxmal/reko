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
    /// Models a unary operator.
    /// </summary>
	public abstract class UnaryOperator : Operator
	{
        /// <summary>
        /// Initializes a unary operator.
        /// </summary>
        /// <param name="type">Operator type.</param>
        protected UnaryOperator(OperatorType type) : base(type) { }

        /// <inheritdoc/>
        public sealed override Constant ApplyConstants(DataType dt, params Constant[] cs)
        {
            if (cs.Length != 1)
                throw new ArgumentException();
            return ApplyConstant(cs[0]);
        }

        /// <inheritdoc/>
        public sealed override Expression Create(DataType dt, params Expression[] exprs)
        {
            if (exprs.Length != 1)
                throw new ArgumentException();
            return new UnaryExpression(this, dt, exprs[0]);
        }

        /// <inheritdoc/>
        public abstract Constant ApplyConstant(Constant c);
	}

    /// <summary>
    /// Logical not operator ("!" in C/C++).
    /// </summary>
	public class NotOperator : UnaryOperator
	{
        internal NotOperator() : base(OperatorType.Not) { }

        /// <inheritdoc/>
		public override Constant ApplyConstant(Constant c)
		{
            return c.IsZero
                ? Constant.Create(c.DataType, 1)
                : Constant.Create(c.DataType, 0);
		}

        /// <inheritdoc/>
		public override string ToString()
		{
			return "!";
		}
	}

    /// <summary>
    /// Negation operator ("-" in C/C++).
    /// </summary>
    public class NegateOperator : UnaryOperator
	{
        internal NegateOperator() : base(OperatorType.Neg) { }

        /// <inheritdoc/>
        public override Constant ApplyConstant(Constant c)
		{
            return c.Negate();
		}

        /// <inheritdoc/>
		public override string ToString()
		{
			return "-";
		}
	}

    /// <summary>
    /// Bitwise not operator ("~" in C/C++).
    /// </summary>
	public class ComplementOperator : UnaryOperator
	{
        internal ComplementOperator() : base(OperatorType.Comp) { }

        /// <inheritdoc/>
        public override Constant ApplyConstant(Constant c)
		{
            return c.Complement();
		}

        /// <inheritdoc/>
		public override string ToString()
		{
			return "~";
		}
	}

    /// <summary>
    /// Models the address-of operator ("<c>&amp;</c>" in C/C++).
    /// </summary>
	public class AddressOfOperator : UnaryOperator
	{
        internal AddressOfOperator() : base(OperatorType.AddrOf) { }

        /// <inheritdoc/>
        public override Constant ApplyConstant(Constant c)
		{
			throw new NotImplementedException();
		}

        /// <inheritdoc/>
		public override string ToString()
		{
			return "&";
		}
	}
}

