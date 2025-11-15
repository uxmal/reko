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
using System;

namespace Reko.Evaluation;

/// <summary>
/// Replaces expressions of the form <code>
/// a &lt;&lt; c1 &lt;&lt; c2
/// </code>
/// with
/// a &lt;&lt; (c1 + c2)
/// </summary>
public class ShiftShift_c_c_Rule
{
    /// <summary>
    /// Perform the match and possible replacement.
    /// </summary>
    /// <param name="b">Binary expression to match.</param>
    /// <param name="m">Expression emitter to use.</param>
    /// <returns></returns>
	public Expression? Match(BinaryExpression b, ExpressionEmitter m)
	{
		var op = b.Operator;
		if (!op.Type.IsShift())
			return null;
    if (b.Right is not Constant c1)
        return null;
    if (b.Left is not BinaryExpression b2)
        return null;
    if (op != b2.Operator)
			return null;
    if (b2.Right is not Constant c2)
        return null;
    var e = b2.Left;
		return m.Bin(
			op,
			e.DataType,
			e,
			Operator.IAdd.ApplyConstants(b.DataType, c1, c2));
	}
}
