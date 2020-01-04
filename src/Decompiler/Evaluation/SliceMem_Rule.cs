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
	/// Matches SLICE(MEM[...], ..) where the slice is contained by the mem access.
	/// </summary>
	public class SliceMem_Rule
	{
		private Expression b;

		public SliceMem_Rule()
		{
		}

		public bool Match(Slice slice)
		{
			MemoryAccess acc = slice.Expression as MemoryAccess;
			if (acc == null)
				return false;

			b = acc.EffectiveAddress;
			Constant offset = Constant.Create(b.DataType, 0);
			BinaryOperator op = Operator.IAdd;
			BinaryExpression ea = b as BinaryExpression;
			if (ea != null)
			{
				Constant c= ea.Right as Constant;
				if (c != null)
				{
					offset = c; 
					b = ea.Left;
				}
			}
			else
			{
				b = acc.EffectiveAddress;
			}
			int bitBegin = slice.Offset;
			int bitEnd = bitBegin + slice.DataType.BitSize;
			if (0 <= bitBegin && bitEnd <= acc.DataType.BitSize)
			{
				offset = op.ApplyConstants(offset, Constant.Create(acc.EffectiveAddress.DataType, slice.Offset / 8));
                var newEa = new BinaryExpression(op, offset.DataType, b, offset);
                if (acc is SegmentedAccess seg)
                {
                    b = new SegmentedAccess(seg.MemoryId, seg.BasePointer, newEa, slice.DataType);
                }
                else
                {
                    b = new MemoryAccess(acc.MemoryId, newEa, slice.DataType);
                }
                return true;
			}
			return false;
		}

		public Expression Transform()
		{
			return b;
		}
	}
}
