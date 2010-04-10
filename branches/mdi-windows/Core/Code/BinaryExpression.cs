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
			return op == Operator.Add ||
				op == Operator.And ||
				op == Operator.Muls ||
				op == Operator.Mulu ||
				op == Operator.Mul ||
				op == Operator.Or ||
				op == Operator.Xor;
		}


		public override Expression Invert()
		{
			if (op == Operator.Cand)
				return new BinaryExpression(Operator.Cor, this.DataType, Left.Invert(), Right.Invert());
			if (op == Operator.Cor)
				return new BinaryExpression(Operator.Cand, this.DataType, Left.Invert(), Right.Invert());
			if (op == Operator.Le)
				return new BinaryExpression(Operator.Gt, this.DataType, Left, Right);
			if (op == Operator.Lt)
				return new BinaryExpression(Operator.Ge, this.DataType, Left, Right);
			if (op == Operator.Ge)
				return new BinaryExpression(Operator.Lt, this.DataType, Left, Right);
			if (op == Operator.Gt)
				return new BinaryExpression(Operator.Le, this.DataType, Left, Right);
			if (op == Operator.Ule)
				return new BinaryExpression(Operator.Ugt, this.DataType, Left, Right);
			if (op == Operator.Ult)
				return new BinaryExpression(Operator.Uge, this.DataType, Left, Right);
			if (op == Operator.Uge)
				return new BinaryExpression(Operator.Ult, this.DataType, Left, Right);
			if (op == Operator.Ugt)
				return new BinaryExpression(Operator.Ule, this.DataType, Left, Right);
			if (op == Operator.Eq)
				return new BinaryExpression(Operator.Ne, this.DataType, Left, Right);
			if (op == Operator.Ne)
				return new BinaryExpression(Operator.Eq, this.DataType, Left, Right);
			throw new NotImplementedException();
		}

	}
}
