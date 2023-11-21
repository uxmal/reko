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
	public abstract class RealConditionalOperator : ConditionalOperator
	{
        protected RealConditionalOperator(OperatorType type) : base(type) { }
	}

    public class ReqOperator : RealConditionalOperator
    {
        internal ReqOperator() : base(OperatorType.Feq) { }

        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() == c2.ToReal64());
        }

        public override Operator Invert() => Fne;
        public override Operator Negate() => Feq;

        public override string ToString() => " == ";
    }

    public class RneOperator : RealConditionalOperator
    {
        internal RneOperator() : base(OperatorType.Fne) { }

        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
        {
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() != c2.ToReal64());
        }

        public override Operator Invert() => Feq;

        public override Operator Negate() => Fne;

        public override string ToString() => " != ";
    }

    public class RltOperator : RealConditionalOperator
	{
        internal RltOperator() : base(OperatorType.Flt) { }

		public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return Constant.Bool(c1.ToReal64() < c2.ToReal64());
		}

        public override Operator Invert() => Fge;

        public override Operator Negate() => Fgt;

        public override string ToString() => " < ";
	}

	public class RgtOperator : RealConditionalOperator
	{
        internal RgtOperator() : base(OperatorType.Fgt) { }

        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() > c2.ToReal64());
        }

        public override Operator Invert() => Fle;

        public override Operator Negate() => Flt;

        public override string ToString() => " > ";
    }

    public class RleOperator : RealConditionalOperator
	{
        internal RleOperator() : base(OperatorType.Fle) { }

        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(PrimitiveType.Bool);
            return Constant.Bool(c1.ToReal64() <= c2.ToReal64());
        }

        public override Operator Invert() => Fgt;

        public override Operator Negate() => Fge;

        public override string ToString() => " <= ";
    }

    public class RgeOperator : RealConditionalOperator
	{
        internal RgeOperator() : base(OperatorType.Fge) { }

        public override Constant ApplyConstants(DataType dt, Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            return Constant.Bool(c1.ToReal64() >= c2.ToReal64());
        }

        public override Operator Invert() => Flt;

        public override Operator Negate() => Fle;

        public override string ToString() => " >= ";

    }
}
