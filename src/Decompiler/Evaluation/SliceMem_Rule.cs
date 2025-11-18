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

namespace Reko.Evaluation
{
	/// <summary>
	/// Matches SLICE(MEM[...], ..) where the slice is contained by the mem access.
	/// </summary>
	public class SliceMem_Rule
	{
        /// <summary>
        /// Matches slices of memory accesses and returns smaller memory accesses.
        /// </summary>
        /// <param name="slice"></param>
        /// <param name="ctx"></param>
        /// <param name="m">Expression emitter to use.</param>
        /// <returns></returns>
		public Expression? Match(Slice slice, EvaluationContext ctx, ExpressionEmitter m)
		{
            if (slice.Expression is not MemoryAccess acc)
                return null;

            if (slice.DataType.BitSize < ctx.MemoryGranularity)
                return null;
            var ea = acc.EffectiveAddress;
            var segptr = ea as SegmentedPointer;
            if (segptr is not null)
                ea = segptr.Offset;
			Constant offset = m.Const(ea.DataType, 0);
			BinaryOperator op = Operator.IAdd;
            if (ea is BinaryExpression bin &&
                bin.Right is Constant c)
            {
                offset = c;
                ea = bin.Left;
            }
            int bitBegin = slice.Offset;
			int bitEnd = bitBegin + slice.DataType.BitSize;
			if (0 <= bitBegin && bitEnd <= acc.DataType.BitSize)
			{
                //$REVIEW: endianness?
                offset = op.ApplyConstants(offset.DataType, offset, m.Const(offset.DataType, slice.Offset / ctx.MemoryGranularity));
                Expression newEa = m.Bin(op, offset.DataType, ea, offset);
                if (segptr is not null)
                {
                    newEa = SegmentedPointer.Create(segptr.BasePointer, newEa);
                }
                ea = m.Mem(acc.MemoryId, slice.DataType, newEa);
                return ea;
			}
			return null;
		}
	}
}
