#region License
/* 
 * Copyright (C) 1999-2016 John Källén.
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
    public class SliceSegmentedPointer_Rule
    {
        private EvaluationContext ctx;
        private SegmentedAccess segMem;
        private Identifier seg;
        private Identifier idOff;
        private Identifier segPtr;

        public SliceSegmentedPointer_Rule(EvaluationContext ctx)
        {
            this.ctx = ctx;
        }

        public bool Match(SegmentedAccess segMem)
        {
            this.segMem = segMem;
            this.seg = segMem.BasePointer as Identifier;
            if (seg == null)
                return false;

            this.segPtr = null;
            var off = segMem.EffectiveAddress;
            this.idOff = off as Identifier;
            if (idOff != null)
            {
                segPtr = SlicedSegPointer(seg, idOff);
            }
            else 
            {
                var binOff = off as BinaryExpression;
                if (binOff == null)
                    return false;
                idOff = binOff.Left as Identifier;
                if (idOff != null)
                {
                    segPtr = SlicedSegPointer(seg, idOff);
                }
            }
            return segPtr != null;
        }

        private Identifier SlicedSegPointer(Identifier seg, Identifier off)
        {
            var defSeg = ctx.GetDefiningExpression(seg) as Slice;
            var defOff = ctx.GetDefiningExpression(off) as Cast;
            if (defSeg == null || defOff == null)
                return null;
            if (defSeg.Expression == defOff.Expression)
                return defSeg.Expression as Identifier;
            else
                return null;
        }

        public Expression Transform()
        {
            ctx.RemoveIdentifierUse(seg);
            ctx.RemoveIdentifierUse(idOff);
            Expression ea;
            if (segMem.EffectiveAddress == idOff)
            {
                ea = new MemoryAccess(segPtr, segMem.DataType);
            }
            else
            {
                var bin = (BinaryExpression)segMem.EffectiveAddress;
                if (bin.Left == idOff)
                {
                    ea = new BinaryExpression(bin.Operator, bin.DataType, segPtr, bin.Right);
                }
                else
                    throw new NotImplementedException();
            }
            ctx.UseExpression(segPtr);
            return new MemoryAccess(ea, segMem.DataType);
        }
    }
}
