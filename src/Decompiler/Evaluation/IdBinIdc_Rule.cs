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
using Reko.Core.Code;
using Reko.Core.Expressions;
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// If we find a = x OP CONST followed by b = a, then replace so that b = x OP CONST.
	/// </summary>
	public class IdBinIdc_Rule
	{
		public IdBinIdc_Rule()
		{
		}

		public Expression? Match(Identifier id, EvaluationContext ctx)
		{
            if (ctx.GetValue(id) is not BinaryExpression bin)
                return null;
            if (ctx.IsUsedInPhi(id))
                return null;
            if (bin.Left is not Identifier || bin.Right is not Constant)
                return null;

            return bin!;
		}
	}
}
