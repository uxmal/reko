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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Evaluation;
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// (+ (+ e c1) c2) ==> (+ e (+ c1 c2))
	/// </summary>
	public class Add_e_c_cRule
	{
		private EvaluationContext ctx;
		private BinaryExpression? bin;
		private BinaryExpression? binLeft;
		private Constant? cLeftRight;
		private Constant? cRight;

        public Add_e_c_cRule(EvaluationContext ctx)
		{
			this.ctx = ctx;
		}

		public bool Match(BinaryExpression binExp, Expression left, Expression right)
		{
			bin = binExp;
			binLeft = left as BinaryExpression;
			if (binLeft == null)
				return false;
			cLeftRight = binLeft.Right as Constant;
			if (cLeftRight == null)
				return false;
			cRight = right as Constant;
			if (cRight == null)
				return false;
			if (!binExp.Operator.Type.IsAddOrSub())
				return false;
			return (!cRight.IsReal && !cRight.IsReal);
		}

		public Expression Transform(Statement stm)
		{
			if (binLeft!.Operator.Type == OperatorType.ISub)
				cLeftRight = cLeftRight!.Negate();
			if (bin!.Operator.Type == OperatorType.ISub)
				cRight = cRight!.Negate();

			BinaryOperator op = Operator.IAdd;
			Constant c = op.ApplyConstants(cLeftRight!, cRight!);
			if (c.IsNegative)
			{
				c = c.Negate();
				op = Operator.ISub;
			}
			return new BinaryExpression(op, bin.DataType, binLeft.Left, c);
		}
	}
}
