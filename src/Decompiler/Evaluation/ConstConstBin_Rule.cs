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
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// Matches a binOp with two constant parameters, which usually is easily simplified.
	/// </summary>
	public class ConstConstBin_Rule
	{
		private Constant? cLeft;
		private Constant? cRight;
		private Operator? op;
        private Address? addr;
        private DataType? dtResult;

		public bool Match(BinaryExpression binExp)
		{
            addr = null;
			cLeft = binExp.Left as Constant; 
			cRight = binExp.Right as Constant;
            dtResult = binExp.DataType;
			op = binExp.Operator;
			if (cLeft != null && cRight != null)
			{
                if (op is ConditionalOperator)
                    return true;
				if (!cLeft.IsReal && !cRight.IsReal)
				{
                    if (cLeft.DataType.IsPointer)
                    {
                        dtResult = cLeft.DataType;
                    }
                    else if (cRight.DataType.IsPointer)
                    {
                        dtResult = cRight.DataType;
                    }
					return true;
				}
			}
            if (binExp.Left is Address a && cRight != null &&
                op.Type.IsAddOrSub())
            {
                addr = a;
                dtResult = a.DataType;
                return true;
            }
			return false;
		}

		public Expression Transform()
		{
            Expression result;
            if (addr is null)
            {
                result = op!.ApplyConstants(cLeft!, cRight!);
            }
            else
            {
                result = addr + cRight!.ToInt32();
            }
            result.DataType = dtResult!;
            return result;
		}
	}
}
