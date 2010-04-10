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

using Decompiler.Core;
using Decompiler.Core.Code;
using Decompiler.Core.Operators;
using System;

namespace Decompiler.Analysis.Simplification
{
	/// <summary>
	/// Matches a binOp with two constant parameters, which usually is easily simplified.
	/// </summary>
	public class ConstConstBin_Rule
	{
		private Constant cLeft;
		private Constant cRight;
		private BinaryOperator op;

		public bool Match(BinaryExpression binExp)
		{
			cLeft = binExp.Left as Constant; 
			cRight = binExp.Right as Constant;
			if (cLeft != null && cRight != null && binExp.op != Operator.Eq)
			{
				if (!cLeft.IsReal && !cRight.IsReal)
				{
					op = binExp.op;
					return true;
				}
			}
			return false;
		}

		public Expression Transform(Statement stm)
		{
			return op.ApplyConstants(cLeft, cRight);
		}
	}
}
