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
	public abstract class UnaryOperator : Operator
	{
        protected UnaryOperator(OperatorType type) : base(type) { }

        public sealed override Constant ApplyConstants(DataType dt, params Constant[] cs)
        {
            if (cs.Length != 1)
                throw new ArgumentException();
            return ApplyConstant(cs[0]);
        }

        public sealed override Expression Create(DataType dt, params Expression[] exprs)
        {
            if (exprs.Length != 1)
                throw new ArgumentException();
            return new UnaryExpression(this, dt, exprs[0]);
        }

        public abstract Constant ApplyConstant(Constant c);
	}

    /// <summary>
    /// Logical not operator ("!" in C/C++).
    /// </summary>
	public class NotOperator : UnaryOperator
	{
        internal NotOperator() : base(OperatorType.Not) { }

		public override Constant ApplyConstant(Constant c)
		{
            return c.IsZero
                ? Constant.Create(c.DataType, 1)
                : Constant.Create(c.DataType, 0);
		}

		public override string ToString()
		{
			return "!";
		}
	}

	public class NegateOperator : UnaryOperator
	{
        internal NegateOperator() : base(OperatorType.Neg) { }

        public override Constant ApplyConstant(Constant c)
		{
            return c.Negate();
		}

		public override string ToString()
		{
			return "-";
		}
	}

	public class ComplementOperator : UnaryOperator
	{
        internal ComplementOperator() : base(OperatorType.Comp) { }

        public override Constant ApplyConstant(Constant c)
		{
            return c.Complement();
		}

		public override string ToString()
		{
			return "~";
		}
	}

	public class AddressOfOperator : UnaryOperator
	{
        internal AddressOfOperator() : base(OperatorType.AddrOf) { }

        public override Constant ApplyConstant(Constant c)
		{
			throw new NotImplementedException();
		}

		public override string ToString()
		{
			return "&";
		}
	}
}

