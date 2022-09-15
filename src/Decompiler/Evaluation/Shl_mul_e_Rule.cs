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

using Reko.Core;
using Reko.Core.Expressions;
using Reko.Core.Operators;
using Reko.Core.Types;
using Reko.Analysis;
using System;

namespace Reko.Evaluation
{
	public class Shl_mul_e_Rule
	{
		private Constant? cShift;
		private Constant? cMul;
		private Operator? op;
		private Expression? e;
        private DataType? dt;

		public Shl_mul_e_Rule(EvaluationContext ctx)
		{
		}

		public bool Match(BinaryExpression b)
		{
			if (b.Operator.Type != OperatorType.Shl)
				return false;
			cShift = b.Right as Constant;
			if (cShift == null)
				return false;

            if (b.Left is not BinaryExpression bLeft)
                return false;

            //$TODO: make an OperatorTypeExtensions.IsIntMultiplication
            if (!bLeft.Operator.Type.IsIntMultiplication())
				return false;
			op = bLeft.Operator;
			cMul = bLeft.Right as Constant;
			if (cMul == null)
				return false;

			e = bLeft.Left;
            dt = b.DataType;
			return true;
		}

		public Expression Transform()
		{
			return new BinaryExpression(op!, dt!, e!, Operator.Shl.ApplyConstants(cMul!, cShift!));
		}
	}
}
