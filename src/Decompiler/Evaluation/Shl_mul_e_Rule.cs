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
using Reko.Analysis;
using System;

namespace Reko.Evaluation
{
	public class Shl_mul_e_Rule
    {
		public Shl_mul_e_Rule()
		{
		}

		public Expression? Match(BinaryExpression b)
        {
			if (b.Operator.Type != OperatorType.Shl)
				return null;
            if (b.Right is not Constant cShift)
                return null;

            if (b.Left is not BinaryExpression bLeft)
                return null;

            if (!bLeft.Operator.Type.IsIntMultiplication())
				return null;
			var op = bLeft.Operator;
            if (bLeft.Right is not Constant cMul)
                return null;

            var e = bLeft.Left;
            var dt = b.DataType;
			return new BinaryExpression(op, dt, e, Operator.Shl.ApplyConstants(cMul.DataType, cMul, cShift));
		}
	}
}
