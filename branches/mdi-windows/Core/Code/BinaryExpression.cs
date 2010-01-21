/* 
 * Copyright (C) 1999-2010 John Källén.
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

using Decompiler.Core.Operators;
using Decompiler.Core.Types;
using System;
using System.IO;

namespace Decompiler.Core.Code
{
	public class BinaryExpression : Expression
	{
        public BinaryOperator op;
		private Expression left;
		private Expression right;

		public BinaryExpression(BinaryOperator op, DataType dt, Expression left, Expression right) : base(dt)
		{
			this.op = op;
			this.Left = left;
			this.Right = right;
		}

		public override Expression Accept(IExpressionTransformer xform)
		{
			return xform.TransformBinaryExpression(this);
		}

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitBinaryExpression(this);
		}

        public Expression Left
        {
            get { return left; }
            set { left = value; }
        }

        public Expression Right
        {
            get { return right; }
            set { right = value; }
        }

		public override Expression CloneExpression()
		{
			return new BinaryExpression(op, DataType, Left.CloneExpression(), Right.CloneExpression());
		}

        /// <summary>
        /// Creates a BinaryExpression with the operands commuted.
        /// </summary>
        /// <returns></returns>
        public BinaryExpression Commute()
        {
            return new BinaryExpression(
                op,
                DataType,
                Right,
                Left);
        }

		public bool Commutes()
		{
			return Commutes(op);
		}

		public static bool Commutes(BinaryOperator op)
		{
			return op == Operator.add ||
				op == Operator.and ||
				op == Operator.muls ||
				op == Operator.mulu ||
				op == Operator.mul ||
				op == Operator.or ||
				op == Operator.xor;
		}


		public override Expression Invert()
		{
			if (op == Operator.cand)
				return new BinaryExpression(Operator.cor, this.DataType, Left.Invert(), Right.Invert());
			if (op == Operator.cor)
				return new BinaryExpression(Operator.cand, this.DataType, Left.Invert(), Right.Invert());
			if (op == Operator.le)
				return new BinaryExpression(Operator.gt, this.DataType, Left, Right);
			if (op == Operator.lt)
				return new BinaryExpression(Operator.ge, this.DataType, Left, Right);
			if (op == Operator.ge)
				return new BinaryExpression(Operator.lt, this.DataType, Left, Right);
			if (op == Operator.gt)
				return new BinaryExpression(Operator.le, this.DataType, Left, Right);
			if (op == Operator.ule)
				return new BinaryExpression(Operator.ugt, this.DataType, Left, Right);
			if (op == Operator.ult)
				return new BinaryExpression(Operator.uge, this.DataType, Left, Right);
			if (op == Operator.uge)
				return new BinaryExpression(Operator.ult, this.DataType, Left, Right);
			if (op == Operator.ugt)
				return new BinaryExpression(Operator.ule, this.DataType, Left, Right);
			if (op == Operator.eq)
				return new BinaryExpression(Operator.ne, this.DataType, Left, Right);
			if (op == Operator.ne)
				return new BinaryExpression(Operator.eq, this.DataType, Left, Right);
			throw new NotImplementedException();
		}

	}
}
