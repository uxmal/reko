#region License
/* 
 * Copyright (C) 1999-2023 John Källén.
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
    /// Models a binary conditional operator.
    /// </summary>
    public abstract class ConditionalOperator : BinaryOperator
    {
        protected ConditionalOperator(OperatorType type) : base(type) { }

        /// <summary>
        /// Negating a conditional operator "flips" it around 0. Eg. 
        /// negating LT generates GT. This is not the
        /// same as Inverting it, which changes LT to GE
        /// </summary>
        public abstract Operator Negate();

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

    public class CandOperator : BinaryOperator
	{
        internal CandOperator() : base(OperatorType.Cand) { }

        public override Constant ApplyConstants(Constant c1, Constant c2)
        {
            if (c1.IsZero)
                return c1;
            return c2;
        }

		public override string ToString()
		{
			return " && ";
		}
	}

	public class CorOperator : BinaryOperator
	{
        internal CorOperator() : base(OperatorType.Cor) { }

        public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!c1.IsZero)
                return c1;
            return c2;
		}

		public override string ToString()
		{
			return " || ";
		}
	}

	public class EqOperator : ConditionalOperator
	{
        internal EqOperator() : base(OperatorType.Eq) { }

        public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return c1.ToInt32() == c2.ToInt32() ? Constant.True() : Constant.False();
		}

        public override Operator Invert() => Ne;
        public override Operator Negate() => Eq;
        public override string ToString() => " == ";
    }

	public class NeOperator : ConditionalOperator
	{
        internal NeOperator() : base(OperatorType.Ne) { }

		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return c1.ToInt32() != c2.ToInt32()
				? Constant.True() 
				: Constant.False();
		}

        public override Operator Invert() => Eq;
        public override Operator Negate() => Ne;

        public override string ToString() => " != ";
    }
}
