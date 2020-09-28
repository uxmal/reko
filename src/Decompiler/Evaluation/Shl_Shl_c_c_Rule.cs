#region License
/* 
 * Copyright (C) 1999-2020 John Källén.
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
using System;

namespace Reko.Evaluation
{
	public class ShiftShift_c_c_Rule
	{
		private Constant c1;
		private Constant c2;
		private Expression e;
		private Operator op;

		public ShiftShift_c_c_Rule(EvaluationContext ctx)
		{
			
		}

		public bool Match(BinaryExpression b)
		{
			op = b.Operator;
			if (op != Operator.Shl && op != Operator.Shr && op != Operator.Sar)
				return false;
			c1 = b.Right as Constant;
			if (c1 == null)
				return false;
			BinaryExpression b2 = b.Left as BinaryExpression;
			if (b2 == null)
				return false;
			if (op != b2.Operator)
				return false;
			c2 = b2.Right as Constant;
			if (c2 == null)
				return false;
			e = b2.Left;
			return true;
		}

		public Expression Transform()
		{
			return new BinaryExpression(
				op,
				e.DataType,
				e,
				Operator.IAdd.ApplyConstants(c1, c2));
		}
	}
}
