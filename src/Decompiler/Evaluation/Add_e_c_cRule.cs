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

        public Add_e_c_cRule()
		{
		}

		public Expression? Match(BinaryExpression binExp, Expression left, Expression right)
		{
			var bin = binExp;
            if (left is not BinaryExpression binLeft)
                return null;
            if (binLeft.Right is not Constant cLeftRight)
                return null;
            if (right is not Constant cRight)
                return null;
            if (!binExp.Operator.Type.IsAddOrSub())
				return null;
            if (cLeftRight.IsReal || cRight.IsReal)
                return null;

			if (binLeft.Operator.Type == OperatorType.ISub)
				cLeftRight = cLeftRight!.Negate();
			if (bin.Operator.Type == OperatorType.ISub)
				cRight = cRight.Negate();

			BinaryOperator op = Operator.IAdd;
			Constant c = op.ApplyConstants(cLeftRight.DataType, cLeftRight, cRight);
			if (c.IsNegative)
			{
				c = c.Negate();
				op = Operator.ISub;
			}
			return new BinaryExpression(op, bin.DataType, binLeft.Left, c);
		}
	}
}
