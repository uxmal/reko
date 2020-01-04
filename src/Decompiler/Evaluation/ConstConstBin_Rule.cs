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
	/// <summary>
	/// Matches a binOp with two constant parameters, which usually is easily simplified.
	/// </summary>
	public class ConstConstBin_Rule
	{
		private Constant cLeft;
		private Constant cRight;
		private Operator op;
        private Address addr;

		public bool Match(BinaryExpression binExp)
		{
            addr = null;
			cLeft = binExp.Left as Constant; 
			cRight = binExp.Right as Constant;
			op = binExp.Operator;
			if (cLeft != null && cRight != null && binExp.Operator != Operator.Eq)
			{
				if (!cLeft.IsReal && !cRight.IsReal)
				{
					return true;
				}
			}
            addr = binExp.Left as Address;
            if (addr != null && cRight != null &&
                (op == Operator.IAdd || op == Operator.ISub))
            {
                return true;
            }
			return false;
		}

		public Expression Transform()
		{
            if (addr == null)
            {
                return op.ApplyConstants(cLeft, cRight);
            }
            else
            {
                return addr + cRight.ToInt32();
            }
		}
	}
}
