#region License
/* 
 * Copyright (C) 1999-2022 John Källén.
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
	public abstract class UnsignedIntOperator : ConditionalOperator
	{
        protected UnsignedIntOperator(OperatorType type) : base(type) { }
	}

	public class UltOperator : UnsignedIntOperator
	{
        internal UltOperator() : base(OperatorType.Ult) { }

		public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
			return Constant.Bool(v1 < v2);
		}

        public override Operator Negate() => Ugt;

        public override string ToString() => " <u ";
    }

    public class UgtOperator : UnsignedIntOperator
	{
        internal UgtOperator() : base(OperatorType.Ugt) { }

        public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);
            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
            return Constant.Bool(v1 <= v2);
		}

        public override Operator Negate() => Ult;

        public override string ToString() => " >u ";
    }

    public class UleOperator : UnsignedIntOperator
	{
        internal UleOperator() : base(OperatorType.Ule) { }

        public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
            return Constant.Bool(v1 <= v2);
		}

        public override Operator Negate() => Uge;

        public override string ToString() => " <=u ";
    }

    public class UgeOperator : UnsignedIntOperator
	{
        internal UgeOperator() : base(OperatorType.Uge) { }

        public override Constant ApplyConstants(Constant c1, Constant c2)
		{
            if (!ValidArgs(c1, c2))
                return InvalidConstant.Create(c1.DataType);

            ulong v1 = c1.ToUInt64();
			ulong v2 = c2.ToUInt64();
			return Constant.Bool(v1 >= v2);
		}

        public override Operator Negate() => Ule;

        public override string ToString() => " >=u ";
    }
}
