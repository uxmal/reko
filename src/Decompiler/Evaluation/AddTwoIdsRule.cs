#region License
/* 
 * Copyright (C) 1999-2026 John Källén.
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
	/// Rule that matches (+ id1 id1) and generates (* id1 2)
	/// </summary>
	public class AddTwoIdsRule
	{
        /// <summary>
        /// Match a binary expression that adds two identifiers together.
        /// </summary>
        /// <param name="binExp">Binary expression to match.</param>
        /// <param name="m">Expression emitter used when building expressions.</param>
        /// <returns></returns>
		public Expression? Match(BinaryExpression binExp, ExpressionEmitter m)
		{
            if (binExp.Left is not Identifier idLeft)
                return null;
            if (binExp.Right is not Identifier idRight)
                return null;
            if (idLeft != idRight || binExp.Operator.Type != OperatorType.IAdd)
                return null;
            return m.IMul(idLeft, m.Const(idLeft.DataType, 2));
        }
	}
}
