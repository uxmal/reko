/* 
 * Copyright (C) 1999-2009 John Källén.
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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using System;

namespace Decompiler.Analysis.Simplification
{
	public class ShiftShift_c_c_Rule
	{
		private Constant c1;
		private Constant c2;
		private Expression e;
		private BinaryOperator op;

		public ShiftShift_c_c_Rule(SsaIdentifierCollection ssaIds)
		{
			
		}

		public bool Match(BinaryExpression b)
		{
			op = b.op;
			if (op != Operator.shl && op != Operator.shr && op != Operator.sar)
				return false;
			c1 = b.Right as Constant;
			if (c1 == null)
				return false;
			BinaryExpression b2 = b.Left as BinaryExpression;
			if (b2 == null)
				return false;
			if (op != b2.op)
				return false;
			c2 = b2.Right as Constant;
			if (c2 == null)
				return false;
			e = b2.Left;
			return true;
		}

		public Expression Transform(Statement stm)
		{
			return new BinaryExpression(
				op,
				e.DataType,
				e,
				Operator.add.ApplyConstants(c1, c2));
		}
	}
}
