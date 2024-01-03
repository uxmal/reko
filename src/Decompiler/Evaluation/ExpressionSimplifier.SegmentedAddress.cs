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

using Reko.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reko.Evaluation
{
    public partial class ExpressionSimplifier
    {
        public virtual (Expression, bool) VisitSegmentedAddress(SegmentedPointer segptr)
        {
            var (basePtr, bChanged) = segptr.BasePointer.Accept(this);
            var (offset, oChanged) = segptr.Offset.Accept(this);
            bool changed = bChanged | oChanged;
            var e = scaledIndexRule.Match(offset, ctx);
            if (e is not null)
            {
                changed = true;
                (offset, _) = e.Accept(this);
            }
            if (basePtr is Constant cBase && offset is Constant cOffset)
            {
                var addr = ctx.MakeSegmentedAddress(cBase, cOffset);
                addr.DataType = segptr.DataType;
                return (addr, true);
            }
            var value = new SegmentedPointer(segptr.DataType, basePtr, offset);
            e = sliceSegPtr.Match(value, ctx);
            if (e is not null)
            {
                return (e, true);
            }
            return (value, changed);
        }

    }
}
