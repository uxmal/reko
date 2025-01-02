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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;

namespace Reko.Evaluation
{
    /// <summary>
    /// Rule that matches:
    ///     (+ (* id c) id) yielding (* id (+ c 1))
    ///     (- (* id c) id) yielding (* id (- c 1))
    /// </summary>
    public class Add_mul_id_c_id_Rule
	{
		public Expression? Match(BinaryExpression exp, EvaluationContext ctx)
		{
            var dt = exp.DataType.ResolveAs<PrimitiveType>();
            if (dt is null)
                return null;
            var opType = exp.Operator.Type;
            Constant cOne;
            if (opType== OperatorType.IAdd)
            {
                cOne = Constant.Create(dt, 1);
            }
            else if (opType == OperatorType.ISub)
            {
                cOne = Constant.Create(dt, -1);
            }
            else
            {
                return null;
            }

            bool swapped = false;
			var id = exp.Left as Identifier;
			var bin = exp.Right as BinaryExpression;
			if (id is null || bin is null)
			{
                // Swap the operands.
				id = exp.Right as Identifier;
				bin = exp.Left as BinaryExpression;
                swapped = true;
			}
			if (id is null || bin is null)
				return null;

			if (bin.Operator.Type != OperatorType.SMul &&
                bin.Operator.Type != OperatorType.UMul &&
                bin.Operator.Type != OperatorType.IMul)
				return null;

            if (bin.Left is not Identifier idInner || bin.Right is not Constant cInner)
                return null;

            if (idInner != id)
				return null;

            var op = Operator.IAdd;
            var c = swapped
                ? op.ApplyConstants(cOne.DataType, cOne, cInner)
                : op.ApplyConstants(cInner.DataType, cInner, cOne);

            return new BinaryExpression(bin.Operator, id.DataType, id, c);
		}
	}
}
