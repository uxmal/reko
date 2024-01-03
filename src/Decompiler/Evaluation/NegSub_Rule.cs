#region License
/* 
 * Copyright (C) 1999-2024 John Källén.
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
	public class NegSub_Rule
	{
		public Expression? Match(UnaryExpression unary)
		{
            if (unary.Operator.Type == OperatorType.Neg &&
                unary.Expression is BinaryExpression bin &&
                bin.Operator.Type == OperatorType.ISub)
            {
                return new BinaryExpression(
                    bin.Operator,
                    bin.DataType,
                    bin.Right,
                    bin.Left);
            }
			return null;
		}
	}
}
