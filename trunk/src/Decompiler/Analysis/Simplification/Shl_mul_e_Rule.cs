/* 
 * Copyright (C) 1999-2008 John Källén.
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
using Decompiler.Analysis;
using System;

namespace Decompiler.Analysis.Simplification
{
	public class Shl_mul_e_Rule
	{
		private Constant cShift;
		private Constant cMul;
		private BinaryOperator op;
		private Expression e;

		public Shl_mul_e_Rule(SsaIdentifierCollection ssaIds)
		{
		}

		public bool Match(BinaryExpression b)
		{
			if (b.op != Operator.shl)
				return false;
			cShift = b.Right as Constant;
			if (cShift == null)
				return false;

			b = b.Left as BinaryExpression;
			if (b == null)
				return false;

			if (b.op != Operator.muls && b.op != Operator.mulu && b.op != Operator.mul)
				return false;
			op = b.op;
			cMul = b.Right as Constant;
			if (cMul == null)
				return false;

			e = b.Left;
			return true;

		}

		public Expression Transform(Statement stm)
		{
			return new BinaryExpression(op, e.DataType, e, Operator.shl.ApplyConstants(cMul, cShift));
		}
	}
}
