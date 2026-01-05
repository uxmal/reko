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

namespace Reko.Evaluation;

/// <summary>
/// If we find <c>a = x OP CONST</c> followed by <c>b = a</c>, then 
/// replace so that <c>b = x OP CONST</c>.
/// </summary>
public class IdBinIdc_Rule
{
    /// <summary>
    /// Evaluates the rule for a given identifier.
    /// </summary>
    /// <param name="id">Identifier being evaluated.</param>
    /// <param name="ctx">Evaluation context</param>
    /// <returns>An expression if the </returns>
	public Expression? Match(Identifier id, EvaluationContext ctx)
	{
        if (ctx.GetValue(id) is not BinaryExpression bin)
            return null;
        if (ctx.IsUsedInPhi(id))
            return null;
        if (bin.Left is not Identifier || bin.Right is not Constant)
            return null;

        return bin;
    }
}
