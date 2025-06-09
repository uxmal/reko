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
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// Matches a binOp with two constant parameters, which usually is easily simplified.
	/// </summary>
	public class ConstConstBin_Rule
	{
		public Expression? Match(BinaryExpression binExp)
		{
            Constant? cRight = binExp.Right as Constant;
            DataType dtResult = binExp.DataType;
			var op = binExp.Operator;
			if (binExp.Left is Constant cLeft && cRight is not null)
			{
                if (op is ConditionalOperator)
                {
                    var result = op.ApplyConstants(binExp.DataType, cLeft!, cRight);
                    result.DataType = dtResult;
                    return result;
                }
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
                    var result = op.ApplyConstants(binExp.DataType, cLeft, cRight);
                    result.DataType = dtResult!;
                    return result;
                }
            }
            if (binExp.Left is Address a && cRight is not null &&
                op.Type.IsAddOrSub())
            {
                var addend = cRight.ToInt64();
                if (op.Type != OperatorType.IAdd)
                    addend = -addend;
                var addr = a + addend;
                addr.DataType = a.DataType;
                return addr;
            }
            return null;
		}
	}
}
