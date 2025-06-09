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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reko.Core.Expressions;

namespace Reko.Evaluation
{
    /// <summary>
    /// Reconstitutes a segmented pointer that has been sliced:
    /// <code>
    /// seg = SLICE(ptr32, 16)
    /// off = SLICE(ptr32, 0)
    /// ...Mem[seg:off]
    /// </code>
    /// becomes
    /// <code>
    /// ...Mem[ptr32]
    /// </code>
    /// </summary>
    public class SliceSegmentedPointer_Rule
    {
        public SliceSegmentedPointer_Rule()
        {
        }

        public Expression? Match(SegmentedPointer segptr, EvaluationContext ctx)
        {
            if (segptr.BasePointer is not Identifier seg)
                return null;

            Identifier? basePtr = null;
            var off = segptr.Offset;
            if (off is Identifier idOff)
            {
                // [seg:idOff] => [seg_idOff]
                basePtr = SlicedSegPointer(seg, idOff, ctx);
            }
            else
            {
                if (off is not BinaryExpression binOff)
                    return null;
                var idOff2 = binOff.Left as Identifier;
                if (idOff2 is not null)
                {
                    // [seg:idOff +/- C] => [seg_idOff + C]
                    basePtr = SlicedSegPointer(seg, idOff2, ctx);
                }
                idOff = idOff2!;
            }
            if (basePtr is null)
                return null;

            Expression ea;
            if (segptr!.Offset == idOff)
            {
                ea = basePtr;
            }
            else
            {
                var bin = (BinaryExpression) segptr.Offset;
                if (bin.Left == idOff)
                {
                    ea = new BinaryExpression(bin.Operator, basePtr!.DataType, basePtr!, bin.Right);
                }
                else
                    throw new NotImplementedException();
            }
            return ea;
        }

        /// <summary>
        /// Try to find an original 32-bit segmented pointer that may have 
        /// been SLICE'd to a segment and offset register.
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="off"></param>
        /// <returns></returns>
        private Identifier? SlicedSegPointer(Identifier seg, Identifier off, EvaluationContext ctx)
        {
            var defSeg = ctx.GetDefiningExpression(seg) as Slice;
            var defOff = ctx.GetDefiningExpression(off) as Slice;
            if (defSeg is null || defOff is null)
                return null;
            if (defSeg.Expression == defOff.Expression)
                return defSeg.Expression as Identifier;
            else
                return null;
        }
    }
}
