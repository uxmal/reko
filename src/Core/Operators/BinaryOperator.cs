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
using Reko.Core.Lib;
using Reko.Core.Types;
using System;
using System.Numerics;

namespace Reko.Core.Operators
{
	public abstract class BinaryOperator : Operator
	{
		public BinaryOperator(OperatorType type)
            : base(type)
		{
		}

        public sealed override Expression Create(DataType dt, params Expression[] exprs)
        {
            if (exprs.Length != 2)
                throw new ArgumentException();
            return new BinaryExpression(this, dt, exprs[0], exprs[1]);
        }

        public sealed override Constant ApplyConstants(DataType dt, params Constant[] cs)
        {
            if (cs.Length != 2)
                throw new ArgumentException();
            return ApplyConstants(dt, cs[0], cs[1]);
        }

        public abstract Constant ApplyConstants(DataType dt, Constant c1, Constant c2);


        protected bool ValidArgs(Constant c1, Constant c2)
        {
            return c1.IsValid && c2.IsValid;
        }

        protected Constant BuildConstant(DataType t1, DataType t2, long val)
		{
			int bitSize = Math.Max(t1.BitSize, t2.BitSize);
			return Constant.Create(PrimitiveType.Create(t1.Domain|t2.Domain, bitSize), val);
		}

        protected Constant BuildConstant(DataType t1, DataType t2, ulong val)
        {
            //$REVIEW what to do when these resolutions fail?
            PrimitiveType p1 = t1.ResolveAs<PrimitiveType>()!;
            PrimitiveType p2 = t2.ResolveAs<PrimitiveType>()!;
            int bitSize = Math.Max(p1.BitSize, p2.BitSize);
            return Constant.Create(PrimitiveType.Create(p1.Domain | p2.Domain, bitSize), val);
        }

        protected Constant BuildConstant(DataType t1, DataType t2, double val)
        {
            PrimitiveType p1 = t1.ResolveAs<PrimitiveType>()!;
            PrimitiveType p2 = t2.ResolveAs<PrimitiveType>()!;
            int bitSize = Math.Max(p1.BitSize, p2.BitSize);
            return ConstantReal.Create(PrimitiveType.Create(p1.Domain & p2.Domain, bitSize), val);
        }

        public Constant BuildConstant(DataType t1, DataType t2, BigInteger value)
        {
            int bitSize = Math.Max(t1.BitSize, t2.BitSize);
            var dtResult = PrimitiveType.CreateWord(bitSize);
            return Constant.Create(dtResult, value);
        }
    }
}
