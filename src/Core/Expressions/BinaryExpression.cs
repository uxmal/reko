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

using Reko.Core.Operators;
using Reko.Core.Types;
using System.Collections.Generic;

namespace Reko.Core.Expressions
{
    /// <summary>
    /// Models binary expressions like integer add, floating point divide etc.
    /// </summary>
	public class BinaryExpression : Expression
	{
        /// <summary>
        /// Create an instance of the <see cref="BinaryExpression"/> class.
        /// </summary>
        /// <param name="op">Binary operation (e.g. <see cref="Operator.IAdd" /></param>
        /// <param name="dtResult">
        /// The result of the operation, which isn't necessarily neither the
        /// same type domain, not even the same size (e.g. extending multiplications
        /// on x86)</param>
        /// <param name="left">Left expression of the operand.</param>
        /// <param name="right">Right expression of the operand.</param>
		public BinaryExpression(BinaryOperator op, DataType dtResult, Expression left, Expression right) : base(dtResult)
		{
			this.Operator = op;
			this.Left = left;
			this.Right = right;
		}

        public override IEnumerable<Expression> Children {
            get {
                yield return Left;
                yield return Right;
            }
        }

        public BinaryOperator Operator { get; } 
        public Expression Left { get; }
        public Expression Right { get; set; }

        public override T Accept<T, C>(ExpressionVisitor<T, C> v, C context)
        {
            return v.VisitBinaryExpression(this, context);
        }

        public override T Accept<T>(ExpressionVisitor<T> v)
        {
            return v.VisitBinaryExpression(this);
        }

		public override void Accept(IExpressionVisitor v)
		{
			v.VisitBinaryExpression(this);
		}

		public override Expression CloneExpression()
		{
			return new BinaryExpression(Operator, DataType, Left.CloneExpression(), Right.CloneExpression());
		}

        /// <summary>
        /// Creates a <see cref="BinaryExpression"/> with the operands commuted.
        /// </summary>
        /// <returns>A new, commuted <see cref="BinaryExpression"/>.</returns>
        public BinaryExpression Commute()
        {
            return new BinaryExpression(
                Operator,
                DataType,
                Right,
                Left);
        }

		public override Expression Invert()
		{
            switch (Operator.Type)
            {
            case OperatorType.Cand:
                return new BinaryExpression(Operators.Operator.Cor, this.DataType, Left.Invert(), Right.Invert());
            case OperatorType.Cor:
                return new BinaryExpression(Operators.Operator.Cand, this.DataType, Left.Invert(), Right.Invert());
            case OperatorType.Le:
                return new BinaryExpression(Operators.Operator.Gt, this.DataType, Left, Right);
            case OperatorType.Lt:
                return new BinaryExpression(Operators.Operator.Ge, this.DataType, Left, Right);
            case OperatorType.Ge:
                return new BinaryExpression(Operators.Operator.Lt, this.DataType, Left, Right);
            case OperatorType.Gt:
                return new BinaryExpression(Operators.Operator.Le, this.DataType, Left, Right);
            case OperatorType.Ule:
                return new BinaryExpression(Operators.Operator.Ugt, this.DataType, Left, Right);
            case OperatorType.Ult:
                return new BinaryExpression(Operators.Operator.Uge, this.DataType, Left, Right);
            case OperatorType.Uge:
                return new BinaryExpression(Operators.Operator.Ult, this.DataType, Left, Right);
            case OperatorType.Ugt:
                return new BinaryExpression(Operators.Operator.Ule, this.DataType, Left, Right);
            case OperatorType.Eq:
                return new BinaryExpression(Operators.Operator.Ne, this.DataType, Left, Right);
            case OperatorType.Ne:
                return new BinaryExpression(Operators.Operator.Eq, this.DataType, Left, Right);
            case OperatorType.Feq:
                return new BinaryExpression(Operators.Operator.Fne, this.DataType, Left, Right);
            case OperatorType.Fne:
                return new BinaryExpression(Operators.Operator.Feq, this.DataType, Left, Right);
            case OperatorType.Fle:
                return new BinaryExpression(Operators.Operator.Fgt, this.DataType, Left, Right);
            case OperatorType.Flt:
                return new BinaryExpression(Operators.Operator.Fge, this.DataType, Left, Right);
            case OperatorType.Fge:
                return new BinaryExpression(Operators.Operator.Flt, this.DataType, Left, Right);
            case OperatorType.Fgt:
                return new BinaryExpression(Operators.Operator.Fle, this.DataType, Left, Right);
            }
            return new UnaryExpression(Operators.Operator.Not, PrimitiveType.Bool, this);
        }
	}
}
