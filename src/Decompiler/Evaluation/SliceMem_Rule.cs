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
using Reko.Core.Machine;
using Reko.Core.Operators;
using System;

namespace Reko.Evaluation
{
	/// <summary>
	/// Matches SLICE(MEM[...], ..) where the slice is contained by the mem access.
	/// </summary>
	public class SliceMem_Rule
	{
		public SliceMem_Rule()
		{
        }

		public Expression? Match(Slice slice, EvaluationContext ctx)
		{
            if (slice.Expression is not MemoryAccess acc)
                return null;

            var ea = acc.EffectiveAddress;
            var segptr = ea as SegmentedPointer;
            if (segptr is not null)
                ea = segptr.Offset;
			Constant offset = Constant.Create(ea.DataType, 0);
			BinaryOperator op = Operator.IAdd;
            if (ea is BinaryExpression bin)
            {
                if (bin.Right is Constant c)
                {
                    offset = c;
                    ea = bin.Left;
                }
            }
            int bitBegin = slice.Offset;
			int bitEnd = bitBegin + slice.DataType.BitSize;
			if (0 <= bitBegin && bitEnd <= acc.DataType.BitSize)
			{
                //$REVIEW: endianness?
                offset = op.ApplyConstants(offset.DataType, offset, Constant.Create(ea.DataType, slice.Offset / ctx.MemoryGranularity));
                Expression newEa = new BinaryExpression(op, offset.DataType, ea, offset);
                if (segptr is not null)
                {
                    newEa = SegmentedPointer.Create(segptr.BasePointer, newEa);
                }
                ea = new MemoryAccess(acc.MemoryId, newEa, slice.DataType);
                return ea;
			}
			return null;
		}
	}
}
